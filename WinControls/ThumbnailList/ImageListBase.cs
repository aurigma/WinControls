// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Base implementation of IListViewImageList interface.
    /// </summary>
    internal abstract class ImageListBase : IImageList
    {
        #region "Constants"

        internal const int InitialImageListCapacity = 1024;

        #endregion "Constants"

        #region "Static part"

        internal static Aurigma.GraphicsMill.Bitmap ConvertToNonextendedRgb(Aurigma.GraphicsMill.Bitmap image)
        {
            if (image.PixelFormat.ColorSpace == Aurigma.GraphicsMill.ColorSpace.Rgb && !image.PixelFormat.IsExtended)
                return image;

            Aurigma.GraphicsMill.Transforms.ColorConverter converter = new Aurigma.GraphicsMill.Transforms.ColorConverter();
            Aurigma.GraphicsMill.Bitmap resultImage = new Aurigma.GraphicsMill.Bitmap();
            converter.DestinationPixelFormat = image.HasAlpha ? Aurigma.GraphicsMill.PixelFormat.Format32bppArgb : Aurigma.GraphicsMill.PixelFormat.Format24bppRgb;
            resultImage = converter.Apply(image);

            return resultImage;
        }

        /// <summary>
        /// Converts icon to bitmap (with alpha channel if necessary).
        /// </summary>
        /// <param name="iconHandle">Source icon handle.</param>
        /// <returns>Result bitmap.</returns>
        public static Aurigma.GraphicsMill.Bitmap IconToAlphaBitmap(System.IntPtr iconHandle)
        {
            NativeMethods.ICONINFO ii = new NativeMethods.ICONINFO();
            NativeMethods.GetIconInfo(iconHandle, out ii);
            System.Drawing.Bitmap bmp = System.Drawing.Bitmap.FromHbitmap(ii.hbmColor);

            NativeMethods.DeleteObject(ii.hbmColor);
            NativeMethods.DeleteObject(ii.hbmMask);

            if (System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) < 32)
            {
                bmp.Dispose();
                return (Aurigma.GraphicsMill.Bitmap)System.Drawing.Bitmap.FromHicon(iconHandle);
            }

            System.Drawing.Bitmap dstBitmap = null;
            System.Drawing.Bitmap result = null;
            bool isAlphaBitmap = false;

            System.Drawing.Imaging.BitmapData bmData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            try
            {
                dstBitmap = new System.Drawing.Bitmap(bmData.Width, bmData.Height, bmData.Stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, bmData.Scan0);
                for (int y = 0; y < bmData.Height; y++)
                {
                    for (int x = 0; x < bmData.Width; x++)
                    {
                        System.Drawing.Color pixelColor = System.Drawing.Color.FromArgb(System.Runtime.InteropServices.Marshal.ReadInt32(bmData.Scan0, (bmData.Stride * y) + (4 * x)));
                        if (pixelColor.A > 0 & pixelColor.A < 255)
                        {
                            isAlphaBitmap = true;
                            break;
                        }
                    }
                    if (isAlphaBitmap)
                        break;
                }

                if (isAlphaBitmap)
                    result = new System.Drawing.Bitmap(dstBitmap);
                else
                    result = System.Drawing.Bitmap.FromHicon(iconHandle);
            }
            finally
            {
                if (dstBitmap != null)
                    dstBitmap.Dispose();

                bmp.UnlockBits(bmData);

                if (bmp != null)
                    bmp.Dispose();
            }

            return new Aurigma.GraphicsMill.Bitmap(result);
        }

        /// <summary>
        /// Creates result image for image list - image of specified size, color and blended with background and foreground images.
        /// </summary>
        /// <param name="size">Desired size of the result image.</param>
        /// <param name="bkColor">Background color.</param>
        /// <param name="backImage">Background image.</param>
        /// <param name="foreImage">Foreground image.</param>
        /// <param name="image">Source image.</param>
        /// <param name="imgResult">Reference to a variable where to store result.</param>
        public static void CreateListImage(System.Drawing.Size size, System.Drawing.Color bkColor, Aurigma.GraphicsMill.Bitmap backImage, Aurigma.GraphicsMill.Bitmap foreImage, Aurigma.GraphicsMill.Bitmap image, out Aurigma.GraphicsMill.Bitmap imgResult)
        {
            imgResult = null;
            Aurigma.GraphicsMill.Bitmap newImage = new Aurigma.GraphicsMill.Bitmap(size.Width, size.Height, Aurigma.GraphicsMill.PixelFormat.Format24bppRgb, bkColor);
            if (backImage != null)
            {
                newImage.Draw(
                    backImage,
                    (size.Width - backImage.Width) / 2,
                    (size.Height - backImage.Height) / 2,
                    backImage.Width,
                    backImage.Height,
                    Aurigma.GraphicsMill.Transforms.CombineMode.Alpha,
                    1,
                    Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.Medium);
            }
            if (image != null)
            {
                newImage.Draw(
                    image,
                    (size.Width - image.Width) / 2,
                    (size.Height - image.Height) / 2,
                    image.Width,
                    image.Height,
                    Aurigma.GraphicsMill.Transforms.CombineMode.Alpha,
                    1,
                    Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.Medium);
            }
            if (foreImage != null)
            {
                newImage.Draw(
                    foreImage,
                    (size.Width - foreImage.Width) / 2,
                    (size.Height - foreImage.Height) / 2,
                    foreImage.Width,
                    foreImage.Height,
                    Aurigma.GraphicsMill.Transforms.CombineMode.Alpha,
                    0.5f,
                    Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.Medium);
            }
            imgResult = newImage;
        }

        #endregion "Static part"

        #region "Construction / destruction"

        /// <summary>
        /// Initializes new instance of the ImageListBase class.
        /// </summary>
        internal ImageListBase()
        {
            _keysHash = new System.Collections.Hashtable(InitialImageListCapacity);
            _imageList = new System.Windows.Forms.ImageList();
            _imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;

            // Workaround - we cause handle creation.
            System.IntPtr handle = _imageList.Handle;

            // Default image.
            _imageList.Images.Add(new System.Drawing.Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format24bppRgb));
        }

        ~ImageListBase()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                System.GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            lock (this)
            {
                if (_imageList == null)
                    return;

                _keysHash.Clear();
                _keysHash = null;

                _imageList.Images.Clear();
                _imageList.Dispose();
                _imageList = null;
            }
        }

        #endregion "Construction / destruction"

        #region "IListViewImageList"

        public System.IntPtr Handle
        {
            get
            {
                System.IntPtr result = System.IntPtr.Zero;
                lock (this)
                {
                    if (_imageList != null)
                        result = _imageList.Handle;
                }
                return result;
            }
        }

        /// <summary>
        /// Returns number of elements in the image list.
        /// </summary>
        public int Count
        {
            get
            {
                lock (this)
                {
                    if (_imageList == null)
                        return 0;

                    return _imageList.Images.Count;
                }
            }
        }

        /// <summary>
        /// Returns copy of the bitmap stored in the image list with specified key.
        /// </summary>
        /// <param name="key">Key of the item to retrieve.</param>
        /// <returns>Returns copy of the bitmap stored in the image list with specified key. Or null if no such entry is presented in the image list.</returns>
        public virtual Aurigma.GraphicsMill.Bitmap GetImage(object key)
        {
            Aurigma.GraphicsMill.Bitmap result = null;

            lock (this)
            {
                if (_imageList == null)
                    return null;

                int imageIndex = IndexOfKey(key, false);
                if (imageIndex == -1)
                    return null;

                result = new Aurigma.GraphicsMill.Bitmap((System.Drawing.Bitmap)_imageList.Images[imageIndex]);
            }

            return result;
        }

        /// <summary>
        /// Changes the entry with specified key.
        /// </summary>
        /// <param name="image">New entry value.</param>
        /// <param name="index">Key of the item to change.</param>
        public virtual void SetImage(Aurigma.GraphicsMill.Bitmap image, object key)
        {
            if (!_imageList.HandleCreated)
                throw new Aurigma.GraphicsMill.UnexpectedException("Handle has not been created.");

            lock (this)
            {
                if (_imageList == null)
                    return;

                int index = IndexOfKey(key, false);
                int actualIndex = _AddImageInternal(image, index);

                if (index == -1)
                    _keysHash.Add(key, actualIndex);
            }
        }

        public virtual void AddImage(System.IntPtr icon, object key)
        {
            if (key == null)
                throw new System.ArgumentNullException("key");

            lock (this)
            {
                if (_imageList == null)
                    return;

                if (!ContainsKey(key))
                {
                    int index = _AddImageInternal(icon, -1);
                    _keysHash.Add(key, index);
                }
            }
        }

        public virtual void AddImage(System.Drawing.Icon icon, object key)
        {
            if (key == null)
                throw new System.ArgumentNullException("key");

            lock (this)
            {
                if (_imageList == null)
                    return;

                if (!ContainsKey(key))
                {
                    int index = _AddImageInternal(icon, -1);
                    _keysHash.Add(key, index);
                }
            }
        }

        public virtual void AddImage(Aurigma.GraphicsMill.Bitmap image, object key)
        {
            if (!_imageList.HandleCreated)
                throw new Aurigma.GraphicsMill.UnexpectedException("Handle has not been created.");

            if (key == null)
                throw new System.ArgumentNullException("key");

            lock (this)
            {
                if (_imageList == null)
                    return;

                if (!ContainsKey(key))
                {
                    int index = _AddImageInternal(image, -1);
                    _keysHash.Add(key, index);
                }
            }
        }

        public virtual void Clear()
        {
            System.Collections.Hashtable oldKeysHash;
            lock (this)
            {
                if (_imageList == null)
                    return;

                System.Drawing.Image defaultImage = null;
                if (_defaultImageSpecified)
                    defaultImage = _imageList.Images[0];

                oldKeysHash = _keysHash;

                _imageList.Images.Clear();
                _keysHash = new System.Collections.Hashtable();

                if (_defaultImageSpecified)
                    _imageList.Images.Add(defaultImage);
                else
                    _imageList.Images.Add(new System.Drawing.Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format24bppRgb));

                OnClear(oldKeysHash);
                oldKeysHash.Clear();
            }
        }

        private void OnClear(System.Collections.Hashtable oldKeysHash)
        {
            if (ImageRemoved != null)
            {
                foreach (System.Collections.DictionaryEntry entry in oldKeysHash)
                    OnImageRemoved(entry.Key, (int)entry.Value);
            }
        }

        public virtual bool ContainsKey(object key)
        {
            lock (this)
            {
                if (_imageList == null)
                    return false;

                return _ContainsKey(key);
            }
        }

        protected bool _ContainsKey(object key)
        {
            return _keysHash.ContainsKey(key);
        }

        public virtual int IndexOfKey(object key)
        {
            return IndexOfKey(key, true);
        }

        public virtual int IndexOfKey(object key, bool useDefaultImageIndex)
        {
            lock (this)
            {
                if (_imageList == null)
                    return -1;

                return _IndexOfKey(key, useDefaultImageIndex);
            }
        }

        protected int _IndexOfKey(object key, bool useDefaultImageIndex)
        {
            if (key == null || !_keysHash.ContainsKey(key))
            {
                if (useDefaultImageIndex && _defaultImageSpecified)
                    return 0;
                else
                    return -1;
            }

            return (int)_keysHash[key];
        }

        public object GetKeyByIndex(int index)
        {
            lock (this)
            {
                if (index < 0 || index >= _imageList.Images.Count)
                    throw new System.ArgumentOutOfRangeException("index");
                if (_imageList == null)
                    return -1;

                return _GetKeyByIndex(index);
            }
        }

        protected object _GetKeyByIndex(int index)
        {
            object result = null;
            foreach (object key in _keysHash.Keys)
                if ((int)_keysHash[key] == index)
                {
                    result = key;
                    break;
                }

            return result;
        }

        public virtual void RemoveByKey(object key)
        {
            lock (this)
            {
                if (_imageList == null)
                    return;

                _RemoveByKey(key);
            }
        }

        protected void _RemoveByKey(object key)
        {
            int index = _IndexOfKey(key, true);
            if (index == -1)
                return;

            _imageList.Images.RemoveAt(index);
            _keysHash.Remove(key);

            OnImageRemoved(key, index);
        }

        public virtual void RemoveAt(int index)
        {
            if (_imageList == null)
                return;

            lock (this)
            {
                _RemoveAt(index);
            }
        }

        protected void _RemoveAt(int index)
        {
            if (index < 1 || index > _imageList.Images.Count)
                throw new System.ArgumentOutOfRangeException("index");

            _imageList.Images.RemoveAt(index);
            object key = GetKeyByIndex(index);
            if (key != null)
                _keysHash.Remove(key);

            OnImageRemoved(key, index);
        }

        //
        // Current implementation of the image list always stores default images
        // at zero index. Default images cannot be removed from the image list.
        //

        public void SetDefaultImage(System.IntPtr icon)
        {
            lock (this)
            {
                if (_imageList == null)
                    return;

                if (icon == System.IntPtr.Zero)
                    _defaultImageSpecified = false;
                else
                    _AddImageInternal(icon, 0);
            }
        }

        public void SetDefaultImage(System.Drawing.Icon icon)
        {
            lock (this)
            {
                if (_imageList == null)
                    return;

                if (icon == null)
                    _defaultImageSpecified = false;
                else
                    _AddImageInternal(icon, 0);
            }
        }

        public void SetDefaultImage(Aurigma.GraphicsMill.Bitmap image)
        {
            lock (this)
            {
                if (_imageList == null)
                    return;

                if (image == null)
                    _defaultImageSpecified = false;
                else
                {
                    _AddImageInternal(image, 0);
                    _defaultImageSpecified = true;
                }
            }
        }

        protected virtual void OnImageRemoved(object imageKey, int imageIndex)
        {
            lock (this)
            {
                if (_imageList == null)
                    return;

                if (ImageRemoved != null && imageIndex >= 0)
                    ImageRemoved(this, new ImageRemovedEventArgs(imageKey, imageIndex));
            }
        }

        public event ImageRemovedEventHandler ImageRemoved;

        #endregion "IListViewImageList"

        #region "Protected abstract methods for children to implement"

        protected abstract int _AddImageInternal(System.IntPtr icon, int destinationIndex);

        protected abstract int _AddImageInternal(System.Drawing.Icon icon, int destinationIndex);

        protected abstract int _AddImageInternal(Aurigma.GraphicsMill.Bitmap image, int destinationIndex);

        #endregion "Protected abstract methods for children to implement"

        #region Member variables

        /// <summary>
        /// Underlying image list object.
        /// </summary>
        protected System.Windows.Forms.ImageList _imageList;

        /// <summary>
        /// Hash table of the items keys.
        /// </summary>
        protected System.Collections.Hashtable _keysHash;

        protected bool _defaultImageSpecified;

        #endregion Member variables
    }
}