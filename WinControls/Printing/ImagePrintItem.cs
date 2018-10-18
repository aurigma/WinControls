// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Aurigma.GraphicsMill.Drawing;
using Aurigma.GraphicsMill.Transforms;
using System;
using System.Diagnostics;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace Aurigma.GraphicsMill.WinControls
{
    internal enum PrintItemElement
    {
        WholeItem = 0,
        Image = 1,
        Footer = 2,
        Header = 3
    }

    internal sealed class ImagePrintItem
    {
        // Resolution should be specified in DPI.
        public ImagePrintItem(float printerResolutionX, float printerResolutionY)
        {
            _printerResolutionX = printerResolutionX;
            _printerResolutionY = printerResolutionY;

            _interpolationMode = ResizeInterpolationMode.Medium;
            _actualImageSize = Size.Empty;

            _headerColor = System.Drawing.Color.Black;
            _footerColor = System.Drawing.Color.Black;
        }

        #region "Properties"

        // Note: no re-calculations of current data will be performed when value changes.
        public System.Drawing.Printing.PrinterResolution PrinterResolution
        {
            set
            {
                _printerResolutionX = value.X;
                _printerResolutionY = value.Y;
            }
        }

        public Bitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                SetImage(value);
            }
        }

        public ResizeInterpolationMode InterpolationMode
        {
            get
            {
                return _interpolationMode;
            }
            set
            {
                _interpolationMode = value;
            }
        }

        public ImageFitMode ImageFitMode
        {
            get
            {
                return _imageFitMode;
            }
            set
            {
                _imageFitMode = value;
            }
        }

        public bool ImageAutoRotate
        {
            get
            {
                return _imageAutoRotate;
            }
            set
            {
                _imageAutoRotate = value;
            }
        }

        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                _headerText = value;
            }
        }

        public string FooterText
        {
            get
            {
                return _footerText;
            }
            set
            {
                _footerText = value;
            }
        }

        public Font HeaderFont
        {
            get
            {
                return _headerFont;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("HeaderFont");
                _headerFont = value.ToGdiPlusFont();
            }
        }

        public Font FooterFont
        {
            get
            {
                return _footerFont;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("FooterFont");
                _footerFont = value.ToGdiPlusFont();
            }
        }

        public System.Drawing.Color FooterColor
        {
            get
            {
                return _footerColor;
            }
            set
            {
                _footerColor = value;
            }
        }

        public System.Drawing.Color HeaderColor
        {
            get
            {
                return _headerColor;
            }
            set
            {
                _headerColor = value;
            }
        }

        public System.Drawing.StringTrimming HeaderTrimmming
        {
            get
            {
                return _headerTrimming;
            }
            set
            {
                _headerTrimming = value;
            }
        }

        public System.Drawing.StringTrimming FooterTrimmming
        {
            get
            {
                return _footerTrimming;
            }
            set
            {
                _footerTrimming = value;
            }
        }

        public System.Drawing.StringAlignment HeaderAlignment
        {
            get
            {
                return _headerAlignment;
            }
            set
            {
                _headerAlignment = value;
            }
        }

        public System.Drawing.StringAlignment FooterAlignment
        {
            get
            {
                return _footerAlignment;
            }
            set
            {
                _footerAlignment = value;
            }
        }

        public System.Drawing.Pen BorderPen
        {
            get
            {
                return _borderPen;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("BorderPen");
                _borderPen = value;
            }
        }

        public int BorderWidth
        {
            get
            {
                if (_borderPen != null)
                    return (int)_borderPen.Width;

                return 0;
            }
        }

        #endregion "Properties"

        public void FitSize(Size bounds, bool placeholderAutoRotate)
        {
            Size itemSize = CalculateElementSize(PrintItemElement.WholeItem);
            if (bounds.Width >= itemSize.Width && bounds.Height >= itemSize.Height)
                return;

            if (_imageFitMode == ImageFitMode.CropToFit)
            {
                SetSize(bounds);
                return;
            }

            if (!_sizeSpecified)
            {
                FitSize(bounds);
            }
            else
            {
                if (!placeholderAutoRotate)
                    SetSize(PrintUtils.FitSizeToBounds(itemSize, bounds));
                else
                    SetSize(CalculateOptimalItemSize(bounds));
            }
        }

        private Size CalculateOptimalItemSize(Size bounds)
        {
            Size itemSize = CalculateElementSize(PrintItemElement.WholeItem);

            if (itemSize.Width < bounds.Width && itemSize.Height < bounds.Height)
                throw new Aurigma.GraphicsMill.UnexpectedException();
            if (!_sizeSpecified)
                throw new Aurigma.GraphicsMill.UnexpectedException();

            Size headerSize = CalculateElementSize(PrintItemElement.Header),
                 footerSize = CalculateElementSize(PrintItemElement.Footer);

            float maxScale = float.MinValue;
            Size bestSize = Size.Empty;
            for (int i = 0; i < 2; i++)
            {
                Size fittedItemSize;
                if (i == 0)
                    fittedItemSize = PrintUtils.FitSizeToBounds(itemSize, bounds);
                else
                    fittedItemSize = PrintUtils.FitSizeToBounds(GetSizeOfRotated(), bounds);

                for (int j = 0; j < 2; j++)
                {
                    Size fittedImageSize = fittedItemSize;
                    fittedImageSize.Height -= headerSize.Height + footerSize.Height + 2 * this.BorderWidth;
                    fittedImageSize.Width -= 2 * this.BorderWidth;

                    Size imageSize;
                    float scale;
                    if (j == 0)
                    {
                        imageSize = PrintUtils.ResizeProportionally(_actualImageSize, fittedImageSize);
                        scale = (float)imageSize.Width / _actualImageSize.Width;
                    }
                    else
                    {
                        imageSize = PrintUtils.ResizeProportionally(new Size(_actualImageSize.Height, _actualImageSize.Width), fittedImageSize);
                        scale = (float)imageSize.Width / _actualImageSize.Height;
                    }

                    if (scale > maxScale)
                    {
                        maxScale = scale;
                        bestSize = fittedItemSize;
                    }

                    if (!_imageAutoRotate)
                        break;
                }
            }

            return bestSize;
        }

        private void FitSize(Size bounds)
        {
            Debug.Assert(!_sizeSpecified);

            // If item less than bounds - just exit.
            Size itemSize = CalculateElementSize(PrintItemElement.WholeItem);
            if (itemSize.Width <= bounds.Width && itemSize.Height <= bounds.Height)
                return;

            // Calculating area of bounded rectangle, where item could be placed
            Size boundedAreaSize = bounds,
                 headerSize = CalculateElementSize(PrintItemElement.Header),
                 footerSize = CalculateElementSize(PrintItemElement.Footer);

            boundedAreaSize.Width -= this.BorderWidth * 2;
            boundedAreaSize.Height -= this.BorderWidth * 2 + headerSize.Height + footerSize.Height;

            if (boundedAreaSize.Width <= 0 || boundedAreaSize.Height <= 0)
            {
                SetSize(bounds);
                return;
            }

            // Fitting size (maybe with rotation)
            Size imageSize = CalculateElementSize(PrintItemElement.Image);

            if (_imageAutoRotate)
            {
                Size rotatedSize = imageSize;
                int tmp = rotatedSize.Width;
                rotatedSize.Width = rotatedSize.Height;
                rotatedSize.Height = tmp;

                if (rotatedSize.Width <= boundedAreaSize.Width && rotatedSize.Height <= boundedAreaSize.Height)
                    imageSize = rotatedSize;
                else
                {
                    float originalScale = (float)boundedAreaSize.Width / imageSize.Width;
                    if (originalScale * imageSize.Height > boundedAreaSize.Height)
                        originalScale = (float)boundedAreaSize.Height / imageSize.Height;

                    float rotatedScale = (float)boundedAreaSize.Width / rotatedSize.Width;
                    if (rotatedScale * rotatedSize.Height > boundedAreaSize.Height)
                        rotatedScale = (float)boundedAreaSize.Height / rotatedSize.Height;

                    if (Math.Abs(1 - originalScale) >= Math.Abs(1 - rotatedScale))
                        imageSize = rotatedSize;
                }
            }

            itemSize = PrintUtils.FitSizeToBounds(imageSize, boundedAreaSize);
            itemSize.Width += this.BorderWidth * 2;
            itemSize.Height += this.BorderWidth * 2 + headerSize.Height + footerSize.Height;

            SetSize(itemSize);
        }

        public void SetSize(Size size)
        {
            if (size.IsEmpty)
                _sizeSpecified = false;
            else
            {
                _externalSize = size;
                _sizeSpecified = true;
            }
        }

        public Size GetSize()
        {
            return CalculateElementSize(PrintItemElement.WholeItem);
        }

        public Size GetSizeOfRotated()
        {
            Size size;
            if (_sizeSpecified)
            {
                size = CalculateElementSize(PrintItemElement.WholeItem);

                int interim = size.Width;
                size.Width = size.Height;
                size.Height = interim;

                size.Width = PrintUtils.RecalculateResolution(size.Width, _printerResolutionY, _printerResolutionX);
                size.Height = PrintUtils.RecalculateResolution(size.Height, _printerResolutionX, _printerResolutionY);
            }
            else
            {
                Size headerSize = CalculateElementSize(PrintItemElement.Header),
                     footerSize = CalculateElementSize(PrintItemElement.Footer);

                size = new Size();
                size.Width = PrintUtils.RecalculateResolution(_actualImageSize.Height, _printerResolutionY, _printerResolutionX);
                size.Height = PrintUtils.RecalculateResolution(_actualImageSize.Width, _printerResolutionX, _printerResolutionY);
                size.Height += headerSize.Height + footerSize.Height;

                size.Height += this.BorderWidth * 2;
                size.Width += this.BorderWidth * 2;
            }
            return size;
        }

        public void Print(System.Drawing.Graphics graphics, Point leftTopPosition)
        {
            bool rotateImage = false;

            Size itemSize = CalculateElementSize(PrintItemElement.WholeItem),
                 imageSize = CalculateElementSize(PrintItemElement.Image),
                 headerSize = CalculateElementSize(PrintItemElement.Header);

            // Printing border
            if (_borderPen != null)
                PrintBorder(graphics, leftTopPosition, itemSize);

            // Printing header & footer
            PrintHeaderAndFooter(graphics, leftTopPosition, itemSize.Height);

            // Printing image
            if (imageSize.Height > 0)
            {
                if (_imageAutoRotate && (_imageFitMode == ImageFitMode.ResizeToFit || _actualImageSize.Width > imageSize.Width || _actualImageSize.Height > imageSize.Height))
                {
                    float imageAspect = (float)_actualImageSize.Width / _actualImageSize.Height - 1,
                          sizeAspect = (float)imageSize.Width / imageSize.Height - 1;

                    if (imageAspect * sizeAspect < 0)
                        rotateImage = true;
                }

                leftTopPosition.X += this.BorderWidth;
                leftTopPosition.Y += headerSize.Height + this.BorderWidth;
                switch (_imageFitMode)
                {
                    case ImageFitMode.ShrinkToFit:
                    case ImageFitMode.ResizeToFit:
                    case ImageFitMode.ResizeToFill:
                        ResizeAndPrintImage(graphics, rotateImage, imageSize, leftTopPosition);
                        break;

                    case ImageFitMode.CropToFit:
                        CropAndPrintImage(graphics, rotateImage, imageSize, leftTopPosition);
                        break;

                    default:
                        throw new ArgumentException(StringResources.GetString("UnsupportedImageFitMode"), "ImageFitMode");
                }
            }
        }

        #region InternalPart

        private Size CalculateElementSize(PrintItemElement element)
        {
            Size res = new Size(0, 0), interimSize;

            switch (element)
            {
                case PrintItemElement.WholeItem:
                    {
                        if (_sizeSpecified)
                            return res = _externalSize;
                        else
                        {
                            res = CalculateElementSize(PrintItemElement.Image);

                            interimSize = CalculateElementSize(PrintItemElement.Header);
                            res.Height += interimSize.Height;
                            interimSize = CalculateElementSize(PrintItemElement.Footer);
                            res.Height += interimSize.Height;

                            res.Height += this.BorderWidth * 2;
                            res.Width += this.BorderWidth * 2;
                        }
                    }
                    break;

                case PrintItemElement.Image:
                    {
                        if (_image == null)
                            throw new System.ArgumentNullException("_image");

                        if (_sizeSpecified)
                        {
                            res = _externalSize;

                            interimSize = CalculateElementSize(PrintItemElement.Header);
                            res.Height -= interimSize.Height;
                            interimSize = CalculateElementSize(PrintItemElement.Footer);
                            res.Height -= interimSize.Height;

                            res.Height -= this.BorderWidth * 2;
                            res.Width -= this.BorderWidth * 2;

                            if (res.Height < 0)
                                res.Height = 0;
                        }
                        else
                            res = _actualImageSize;
                    }
                    break;

                case PrintItemElement.Header:
                    {
                        if (_sizeSpecified)
                            res.Width = _externalSize.Width - this.BorderWidth * 2;
                        else
                            res.Width = _actualImageSize.Width;

                        if (_headerText != null)
                            res.Height = (int)Math.Round(_headerFont.GetHeight(_printerResolutionY) * 1.2);
                        else
                            res.Height = 0;
                    }
                    break;

                case PrintItemElement.Footer:
                    {
                        if (_sizeSpecified)
                            res.Width = _externalSize.Width - this.BorderWidth * 2;
                        else
                            res.Width = _actualImageSize.Width;

                        if (_footerText != null)
                            res.Height = (int)Math.Round(_footerFont.GetHeight(_printerResolutionY) * 1.2);
                        else
                            res.Height = 0;
                    }
                    break;

                default:
                    throw new ArgumentException(StringResources.GetString("UnsupportedPrintItemElement"), "enElement");
            }
            return res;
        }

        private int ConvertToActualPixels(int value, float valueResolution, bool useHorizontalResolution)
        {
            float resolution = useHorizontalResolution ? _printerResolutionX : _printerResolutionY;

            double inches = (double)value / valueResolution;
            return (int)Math.Round(inches * resolution);
        }

        private void SetImage(Bitmap image)
        {
            _image = image;

            if (_image != null)
            {
                Single resolutionX = _image.DpiX,
                       resolutionY = _image.DpiY;

                if (resolutionX <= 1 || resolutionX > 10000)
                    resolutionX = 72;
                if (resolutionY <= 1 || resolutionY > 10000)
                    resolutionY = 72;

                _actualImageSize.Width = ConvertToActualPixels(_image.Width, (float)resolutionX, true);
                _actualImageSize.Height = ConvertToActualPixels(_image.Height, (float)resolutionY, false);
            }
            else
                _actualImageSize = Size.Empty;
        }

        private void CropAndPrintImage(System.Drawing.Graphics graphics, bool rotateImage, Size dstSize, Point leftTopPosition)
        {
            // Rotating source image if needed (if RotationAllowed == true && aspect ratio of rotated on 90
            // degree image is better for placing into specified sizes).
            Bitmap printImage;
            bool disposePrintImage = false;
            Size imageSize = new Size();
            if (rotateImage)
            {
                printImage = new Bitmap();

                Rotate rotate = new Rotate(90);
                printImage = rotate.Apply(_image);
                disposePrintImage = true;

                imageSize.Width = _actualImageSize.Height;
                imageSize.Height = _actualImageSize.Width;
            }
            else
            {
                printImage = _image;
                imageSize = _actualImageSize;
            }

            // Correcting crop rectangle - if DstSize is greater than ImageSize -
            // no need to crop => DstSize should be equal to ImageSize.
            if ((dstSize.Width >= imageSize.Width && dstSize.Height > imageSize.Height) ||
                (dstSize.Width > imageSize.Width && dstSize.Height >= imageSize.Height))
            {
                leftTopPosition.X += (int)Math.Round((float)(dstSize.Width - imageSize.Width) / 2.0f);
                leftTopPosition.Y += (int)Math.Round((float)(dstSize.Height - imageSize.Height) / 2.0f);

                dstSize = imageSize;
            }

            // Calculating crop parameters, correcting output coordinates
            int cropX = (int)Math.Round((float)(imageSize.Width - dstSize.Width) / 2.0),
                cropY = (int)Math.Round((float)(imageSize.Height - dstSize.Height) / 2.0),
                cropWidth = Math.Min(dstSize.Width, imageSize.Width),
                cropHeight = Math.Min(dstSize.Height, imageSize.Height);

            if (cropX < 0)
                cropX = 0;

            if (cropY < 0)
                cropY = 0;

            if (imageSize.Width < dstSize.Width)
                leftTopPosition.X += (int)Math.Round((float)(dstSize.Width - imageSize.Width) / 2);

            if (imageSize.Height < dstSize.Height)
                leftTopPosition.Y += (int)Math.Round((float)(dstSize.Height - imageSize.Height) / 2);

            // Calling StripPrint() for real printing.
            if (cropWidth > 0 && cropHeight > 0)
                StripPrint(printImage, graphics, leftTopPosition, imageSize, new Rectangle(cropX, cropY, cropWidth, cropHeight));

            // Cleanup
            if (disposePrintImage)
                printImage.Dispose();
        }

        private void ResizeAndPrintImage(System.Drawing.Graphics graphics, bool rotateImage, Size dstSize, Point leftTopPosition)
        {
            Size printImageSize;
            Bitmap printImage;
            bool disposePrintImage = false;

            // Preparing source image for printing
            if (rotateImage)
            {
                printImage = new Bitmap();

                Rotate rotate = new Rotate(90);
                printImage = rotate.Apply(_image);
                disposePrintImage = true;

                printImageSize = new Size();
                printImageSize.Width = _actualImageSize.Height;
                printImageSize.Height = _actualImageSize.Width;
            }
            else
            {
                printImage = _image;
                printImageSize = _actualImageSize;
            }

            // Calculating resize params, crop rectangles and correcting output position
            Size resizeSize = CalculateImageSizeForPrint(printImageSize, dstSize);

            leftTopPosition.X += Math.Max(0, (dstSize.Width - resizeSize.Width) / 2);
            leftTopPosition.Y += Math.Max(0, (dstSize.Height - resizeSize.Height) / 2);

            Rectangle cropRect = new Rectangle(0, 0, Math.Min(dstSize.Width, resizeSize.Width), Math.Min(dstSize.Height, resizeSize.Height));
            cropRect.X = Math.Max(0, (resizeSize.Width - dstSize.Width) / 2);
            cropRect.Y = Math.Max(0, (resizeSize.Height - dstSize.Height) / 2);

            // Printing
            if (resizeSize.Height > 0 && resizeSize.Width > 0)
                StripPrint(printImage, graphics, leftTopPosition, resizeSize, cropRect);

            // CleanUp
            if (disposePrintImage)
                printImage.Dispose();
        }

        private void PreserveScaleFactor(int sourceWidth, int sourceHeight, ref int destinationWidth, ref int destinationHeight)
        {
            if (destinationWidth <= 0 && destinationHeight <= 0)
            {
                destinationWidth = sourceWidth;
                destinationHeight = sourceHeight;
            }
            else if (destinationWidth <= 0)
            {
                destinationWidth = (int)((float)sourceWidth * ((float)destinationHeight / (float)sourceHeight));
            }
            else if (destinationHeight <= 0)
            {
                destinationHeight = (int)((float)sourceHeight * ((float)destinationWidth / (float)sourceWidth));
            }

            if (destinationWidth <= 0)
                destinationWidth = 1;

            if (destinationHeight <= 0)
                destinationHeight = 1;
        }

        private void CalculateProportionalDimensions(int srcWidth, int srcHeight, ref int destWidth, ref int destHeight, Aurigma.GraphicsMill.Transforms.ResizeMode mode)
        {
            if (srcWidth <= 0)
                throw new System.ArgumentOutOfRangeException("srcWidth");

            if (srcHeight <= 0)
                throw new System.ArgumentOutOfRangeException("srcHeight");

            if (mode == Aurigma.GraphicsMill.Transforms.ResizeMode.Shrink || mode == Aurigma.GraphicsMill.Transforms.ResizeMode.Fit)
            {
                if (destWidth <= 0)
                    throw new System.ArgumentOutOfRangeException("destWidth");
                if (destHeight <= 0)
                    throw new System.ArgumentOutOfRangeException("destHeight");
            }
            else if (destWidth <= 0 && destHeight <= 0)
                throw new System.ArgumentOutOfRangeException("One of the new width or height should have positive value.");

            if (mode == Aurigma.GraphicsMill.Transforms.ResizeMode.Resize)
            {
                if (destWidth <= 0 || destHeight <= 0)
                    PreserveScaleFactor(srcWidth, srcHeight, ref destWidth, ref destHeight);
            }
            else
            {
                double srcAspect = (double)srcWidth / srcHeight,
                        dstAspect = (double)destWidth / destHeight;

                if (mode == Aurigma.GraphicsMill.Transforms.ResizeMode.Shrink && srcWidth <= destWidth && srcHeight <= destHeight)
                {
                    destWidth = srcWidth;
                    destHeight = srcHeight;
                    return;
                }

                if (srcAspect > dstAspect)
                    destHeight = (int)((double)destWidth / srcAspect);
                else
                    destWidth = (int)((double)destHeight * srcAspect);

                if (destWidth <= 0)
                    destWidth = 1;
                if (destHeight <= 0)
                    destHeight = 1;
            }
        }

        private Size CalculateImageSizeForPrint(Size srcSize, Size dstSize)
        {
            int dstWidth, dstHeight;
            Size result = Size.Empty;

            switch (_imageFitMode)
            {
                case ImageFitMode.CropToFit:
                    throw new UnexpectedException();

                case ImageFitMode.ResizeToFit:
                    {
                        dstWidth = dstSize.Width;
                        dstHeight = dstSize.Height;
                        CalculateProportionalDimensions(srcSize.Width, srcSize.Height, ref dstWidth, ref dstHeight, Aurigma.GraphicsMill.Transforms.ResizeMode.Fit);
                        result = new Size(dstWidth, dstHeight);
                        break;
                    }

                case ImageFitMode.ShrinkToFit:
                    {
                        dstWidth = dstSize.Width;
                        dstHeight = dstSize.Height;
                        CalculateProportionalDimensions(srcSize.Width, srcSize.Height, ref dstWidth, ref dstHeight, Aurigma.GraphicsMill.Transforms.ResizeMode.Shrink);
                        result = new Size(dstWidth, dstHeight);
                        break;
                    }

                case ImageFitMode.ResizeToFill:
                    {
                        double srcK, dstK;
                        srcK = (double)srcSize.Width / srcSize.Height;
                        dstK = (double)dstSize.Width / dstSize.Height;

                        if (srcK >= dstK)
                        {
                            result.Height = dstSize.Height;
                            result.Width = (int)((double)srcSize.Width / srcSize.Height * result.Height);
                        }
                        else
                        {
                            result.Width = dstSize.Width;
                            result.Height = (int)((double)srcSize.Height / srcSize.Width * result.Width);
                        }

                        break;
                    }

                default:
                    throw new UnexpectedException(StringResources.GetString("UnsupportedPrintingFitMode"));
            }

            if (result.Width == 0)
                result.Width = 1;
            if (result.Height == 0)
                result.Height = 1;

            return result;
        }

        private void StripPrint(Bitmap image, System.Drawing.Graphics graphics, Point leftTopPosition, Size resizeSize, Rectangle cropRectangle)
        {
            const int MaxStripSize = 20000000;

            PixelFormat format = image.PixelFormat;
            if (format.IsIndexed)
                format = PixelFormat.Format32bppArgb;

            int pixelSize = Math.Max(1, format.Size);
            int stripHeight = MaxStripSize / (pixelSize * cropRectangle.Width);
            int stripCount = (int)Math.Ceiling((float)cropRectangle.Height / stripHeight);

            Rectangle objCurStrip = new Rectangle(cropRectangle.X, cropRectangle.Y, cropRectangle.Width, stripHeight);
            int curShiftY = 0;

            for (int i = 0; i < stripCount; i++)
            {
                using (var resize = new Resize(resizeSize.Width, resizeSize.Height, _interpolationMode))
                using (var crop = new Crop())
                using (var strip = new Bitmap())
                {
                    objCurStrip.Height = Math.Min(stripHeight, cropRectangle.Height - curShiftY);

                    crop.Rectangle = objCurStrip;

                    Pipeline.Run(image + resize + crop + strip);
                    strip.DrawOn(graphics, leftTopPosition.X, leftTopPosition.Y + curShiftY, objCurStrip.Width, objCurStrip.Height, CombineMode.Copy, 1.0f, ResizeInterpolationMode.NearestNeighbour);
                }

                objCurStrip.Y += objCurStrip.Height;
                curShiftY += objCurStrip.Height;
            }
        }

        private void PrintHeaderAndFooter(System.Drawing.Graphics graphics, Point itemLeftTop, int itemHeight)
        {
            Size headerSize = CalculateElementSize(PrintItemElement.Header),
                 footerSize = CalculateElementSize(PrintItemElement.Footer);

            itemLeftTop.X += this.BorderWidth;
            itemLeftTop.Y += this.BorderWidth;
            itemHeight -= this.BorderWidth * 2;

            if (headerSize.Height > 0 || footerSize.Height > 0)
            {
                System.Drawing.StringFormat format = new System.Drawing.StringFormat();
                format.LineAlignment = System.Drawing.StringAlignment.Near;

                if (headerSize.Height > 0 && itemHeight >= headerSize.Height)
                {
                    Rectangle rect = new Rectangle(itemLeftTop, headerSize);
                    System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(_headerColor);
                    format.Alignment = _headerAlignment;
                    format.Trimming = _headerTrimming;

                    graphics.DrawString(_headerText, _headerFont, brush, rect, format);
                }

                if (footerSize.Height > 0 && itemHeight >= footerSize.Height)
                {
                    Rectangle rect = new Rectangle(itemLeftTop, footerSize);
                    rect.Y += itemHeight - footerSize.Height;

                    System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(_footerColor);
                    format.Alignment = _footerAlignment;
                    format.Trimming = _footerTrimming;

                    graphics.DrawString(_footerText, _footerFont, brush, rect, format);
                }
            }
        }

        private void PrintBorder(System.Drawing.Graphics graphics, Point leftTopPosition, Size itemSize)
        {
            _borderPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
            int width = (int)_borderPen.Width;

            if (itemSize.Width >= width && itemSize.Height >= width)
                graphics.DrawRectangle(_borderPen, leftTopPosition.X, leftTopPosition.Y, itemSize.Width, itemSize.Height);
            else
            {
                float oldWidth = _borderPen.Width;

                _borderPen.Width = Math.Min(itemSize.Width, itemSize.Height);
                graphics.DrawRectangle(_borderPen, leftTopPosition.X, leftTopPosition.Y, itemSize.Width, itemSize.Height);
                _borderPen.Width = oldWidth;
            }
        }

        #endregion InternalPart

        //--------------------------------
        private float _printerResolutionX;

        private float _printerResolutionY;

        private System.Drawing.Pen _borderPen;

        private string _headerText;
        private string _footerText;
        private System.Drawing.Font _headerFont;
        private System.Drawing.Font _footerFont;
        private System.Drawing.StringAlignment _headerAlignment;
        private System.Drawing.StringAlignment _footerAlignment;
        private System.Drawing.Color _headerColor;
        private System.Drawing.Color _footerColor;
        private System.Drawing.StringTrimming _headerTrimming;
        private System.Drawing.StringTrimming _footerTrimming;

        private bool _sizeSpecified;
        private Size _externalSize;

        private ImageFitMode _imageFitMode;
        private bool _imageAutoRotate;
        private ResizeInterpolationMode _interpolationMode;

        private Bitmap _image;
        private Size _actualImageSize;
    }
}