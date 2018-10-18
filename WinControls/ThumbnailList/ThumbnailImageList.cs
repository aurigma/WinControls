// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Aurigma.GraphicsMill.Transforms;
using System;
using System.Drawing;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Thumbnails image list.
    /// </summary>
    internal class ThumbnailImageList : ImageListBase
    {
        #region "Construction / destruction"

        internal ThumbnailImageList(int thumbnailWidth, int thumbnailHeight)
        {
            _nextIndexToRewrite = new System.Collections.Queue(ImageListBase.InitialImageListCapacity);
            _thumbnailSize = new Size(thumbnailWidth, thumbnailHeight);
            _imageList.ImageSize = _thumbnailSize;

            _listItemBackgroundColor = System.Drawing.SystemColors.Window;

            ResetDefaultImage();
        }

        public ThumbnailImageList()
            : this(100, 100)
        {
        }

        #endregion "Construction / destruction"

        protected override int _AddImageInternal(IntPtr icon, int destinationIndex)
        {
            throw new NotImplementedException(StringResources.GetString("NotImplementedInThumbnailImageList"));
        }

        protected override int _AddImageInternal(System.Drawing.Icon icon, int destinationIndex)
        {
            throw new NotImplementedException(StringResources.GetString("NotImplementedInThumbnailImageList"));
        }

        protected override int _AddImageInternal(Bitmap image, int destinationIndex)
        {
            if (_imageList == null)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("InternalImageListCannotBeNull"));

            int result = -1;
            Aurigma.GraphicsMill.Bitmap imgResult = null;

            try
            {
                CreateListImage(_imageList.ImageSize, _listItemBackgroundColor, _listItemBackgroundImage, _listItemForegroundImage, image, out imgResult);

                try
                {
                    if (destinationIndex != -1)
                    {
                        _imageList.Images[destinationIndex] = imgResult.ToGdiPlusBitmap();
                        result = destinationIndex;
                    }
                    else
                    {
                        _imageList.Images.Add(imgResult.ToGdiPlusBitmap());
                        result = _imageList.Images.Count - 1;
                    }
                }
                catch (System.InvalidOperationException)
                {
                    if (_nextIndexToRewrite.Count > 0)
                    {
                        int index = (int)_nextIndexToRewrite.Dequeue();
                        ReplaceImage(index, imgResult.ToGdiPlusBitmap());
                        result = index;
                    }
                    else
                        throw;
                }

                if (result != -1 && result != 0)
                    _nextIndexToRewrite.Enqueue(result);
            }
            catch
            {
                result = -1;
            }
            finally
            {
                if (imgResult != null)
                    imgResult.Dispose();
            }

            return result;
        }

        private void ReplaceImage(int index, System.Drawing.Bitmap newImage)
        {
            object key = GetKeyByIndex(index);

            _keysHash.Remove(key);
            _imageList.Images[index] = newImage;

            OnImageRemoved(key, index);
        }

        private void ResetDefaultImage()
        {
            _defaultImageSpecified = false;
            System.Drawing.Bitmap defaultImage = new System.Drawing.Bitmap(_thumbnailSize.Width, _thumbnailSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            if (_imageList.Images.Count > 0)
                _imageList.Images[0] = defaultImage;
            else
                _imageList.Images.Add(defaultImage);
        }

        #region "Public properties"

        /// <summary>
        /// Sets of gets item background color.
        /// </summary>
        public System.Drawing.Color ListItemBackgroundColor
        {
            get
            {
                return _listItemBackgroundColor;
            }
            set
            {
                _listItemBackgroundColor = value;
            }
        }

        /// <summary>
        /// Sets of gets background image. The image will be resized to image list's thumbnail size.
        /// </summary>
        public Aurigma.GraphicsMill.Bitmap ListItemBackgroundImage
        {
            set
            {
                lock (this)
                {
                    _listItemBackgroundImage = value;
                    if (_listItemBackgroundImage != null)
                    {
                        if (_listItemBackgroundImage.Width != _thumbnailSize.Width || _listItemBackgroundImage.Height != _thumbnailSize.Height)
                        {
                            Aurigma.GraphicsMill.Bitmap resizedBitmap = new Aurigma.GraphicsMill.Bitmap();

                            Resize resizeTransform = new Resize(_thumbnailSize);
                            resizedBitmap = resizeTransform.Apply(_listItemBackgroundImage);

                            _listItemBackgroundImage = resizedBitmap;
                        }

                        _listItemBackgroundImage = ImageListBase.ConvertToNonextendedRgb(_listItemBackgroundImage);
                    }
                }
            }
            get
            {
                return _listItemBackgroundImage;
            }
        }

        /// <summary>
        /// Sets of gets foreground image. The image will be resized to image list's thumbnail size.
        /// </summary>
        public Aurigma.GraphicsMill.Bitmap ListItemForegroundImage
        {
            set
            {
                lock (this)
                {
                    _listItemForegroundImage = value;
                    if (_listItemForegroundImage != null)
                    {
                        if (_listItemForegroundImage.Width != _thumbnailSize.Width || _listItemForegroundImage.Height != _thumbnailSize.Height)
                        {
                            Aurigma.GraphicsMill.Bitmap resizedBitmap = new Aurigma.GraphicsMill.Bitmap();

                            Resize resizeTransform = new Resize(_thumbnailSize);
                            resizedBitmap = resizeTransform.Apply(_listItemForegroundImage);

                            _listItemForegroundImage = resizedBitmap;
                        }

                        _listItemForegroundImage = ImageListBase.ConvertToNonextendedRgb(_listItemForegroundImage);
                    }
                }
            }
            get
            {
                return _listItemForegroundImage;
            }
        }

        /// <summary>
        /// Sets or gets image list's thumbnail size.
        /// </summary>
        public Size ThumbnailSize
        {
            set
            {
                lock (this)
                {
                    if (_imageList == null)
                        return;

                    if (_imageList.Images.Count > 1)
                        throw new System.InvalidOperationException(StringResources.GetString("CannotChangeThumbnailSize"));

                    if (_thumbnailSize != value)
                    {
                        _thumbnailSize = value;
                        _imageList.ImageSize = _thumbnailSize;
                        ResetDefaultImage();

                        if (_listItemBackgroundImage != null && (_listItemBackgroundImage.Width != _thumbnailSize.Width || _listItemBackgroundImage.Height != _thumbnailSize.Height))
                        {
                            Aurigma.GraphicsMill.Bitmap resizedBitmap = new Aurigma.GraphicsMill.Bitmap();
                            using (Resize resizeTransform = new Resize(_thumbnailSize))
                                resizedBitmap = resizeTransform.Apply(_listItemBackgroundImage);

                            _listItemBackgroundImage = resizedBitmap;
                        }

                        if (_listItemForegroundImage != null && (_listItemForegroundImage.Width != _thumbnailSize.Width || _listItemForegroundImage.Height != _thumbnailSize.Height))
                        {
                            Aurigma.GraphicsMill.Bitmap resizedBitmap = new Aurigma.GraphicsMill.Bitmap();
                            using (Resize resizeTransform = new Resize(_thumbnailSize))
                                resizedBitmap = resizeTransform.Apply(_listItemForegroundImage);

                            _listItemForegroundImage = resizedBitmap;
                        }
                    }
                }
            }
            get
            {
                return _thumbnailSize;
            }
        }

        #endregion "Public properties"

        #region Private variables

        /// <summary>
        /// Stores resized (if necessary) background image.
        /// </summary>
        private Aurigma.GraphicsMill.Bitmap _listItemBackgroundImage;

        /// <summary>
        /// Stores resized (if necessary) foreground image.
        /// </summary>
        private Aurigma.GraphicsMill.Bitmap _listItemForegroundImage;

        /// <summary>
        /// Background color for the image list's items.
        /// </summary>
        private System.Drawing.Color _listItemBackgroundColor;

        /// <summary>
        /// Stores thumbnail size.
        /// </summary>
        private Size _thumbnailSize;

        private System.Collections.Queue _nextIndexToRewrite;

        #endregion Private variables
    }
}