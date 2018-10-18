// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Drawing;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Implementation of the image list interface.
    /// </summary>
    internal class SystemImageList : ImageListBase
    {
        internal SystemImageList(bool large)
        {
            if (large == true)
            {
                _imageList.ImageSize = new Size(32, 32);
            }
            else
            {
                _imageList.ImageSize = new Size(16, 16);
            }
        }

        protected override int _AddImageInternal(IntPtr icon, int destinationIndex)
        {
            if (icon == IntPtr.Zero)
                return -1;

            int result = -1;
            lock (this)
            {
                if (_imageList == null)
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("InternalImageListCannotBeNull"));

                System.Drawing.Icon bitmap = System.Drawing.Icon.FromHandle(icon);
                if (_imageList.ImageSize != new Size(bitmap.Width, bitmap.Height))
                    _imageList.ImageSize = new Size(bitmap.Width, bitmap.Height);

                if (destinationIndex != -1)
                {
                    _imageList.Images[destinationIndex] = bitmap.ToBitmap();
                    bitmap.Dispose();
                    result = destinationIndex;
                }
                else
                {
                    _imageList.Images.Add(bitmap);
                    result = _imageList.Images.Count - 1;
                }
            }

            return result;
        }

        protected override int _AddImageInternal(System.Drawing.Icon icon, int destinationIndex)
        {
            throw new NotImplementedException(StringResources.GetString("NotImplementedInSystemImageList"));
        }

        protected override int _AddImageInternal(Aurigma.GraphicsMill.Bitmap image, int destinationIndex)
        {
            if (image == null)
                return -1;

            int result = -1;

            lock (this)
            {
                if (_imageList == null)
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("InternalImageListCannotBeNull"));

                bool disposeImage = false;
                if (_imageList.ImageSize.Width != image.Width || _imageList.ImageSize.Height != image.Height)
                {
                    Aurigma.GraphicsMill.Bitmap imageCopy = ConvertToNonextendedRgb(image);
                    if (imageCopy == image)
                        imageCopy = new Aurigma.GraphicsMill.Bitmap(image);

                    imageCopy.Transforms.Resize(_imageList.ImageSize.Width, _imageList.ImageSize.Height, Transforms.ResizeInterpolationMode.High, Transforms.ResizeMode.Shrink);

                    int destX = (_imageList.ImageSize.Width - imageCopy.Width) / 2,
                        destY = (_imageList.ImageSize.Height - imageCopy.Height) / 2;

                    Aurigma.GraphicsMill.Bitmap resultImage = new Aurigma.GraphicsMill.Bitmap(
                        _imageList.ImageSize.Width,
                        _imageList.ImageSize.Height,
                        Aurigma.GraphicsMill.PixelFormat.Format32bppArgb,
                        new Aurigma.GraphicsMill.RgbColor(0xff, 0xff, 0xff, 0x0));

                    resultImage.Draw(imageCopy, destX, destY, image.Width, image.Height, Aurigma.GraphicsMill.Transforms.CombineMode.Copy, Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.Low);
                    imageCopy.Dispose();

                    image = resultImage;
                    disposeImage = true;
                }

                if (destinationIndex != -1)
                {
                    _imageList.Images[destinationIndex] = image.ToGdiPlusBitmap();
                    result = destinationIndex;
                }
                else
                {
                    _imageList.Images.Add(image.ToGdiPlusBitmap());
                    result = _imageList.Images.Count - 1;
                }

                if (disposeImage)
                    image.Dispose();
            }

            return result;
        }
    }
}