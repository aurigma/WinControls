// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Drawing.Printing;
using Size = System.Drawing.Size;

namespace Aurigma.GraphicsMill.WinControls
{
    internal interface IPrintItemDistributor
    {
        void Initialize(ImagePrintDocument document, PrintPageEventArgs e);

        void FillPage(PrintPageEventArgs e, System.Collections.ArrayList items, System.Collections.ArrayList coords);

        IImageProvider ImageProvider
        {
            get;
            set;
        }

        PrintOptions PrintOptions
        {
            get;
            set;
        }
    }

    internal abstract class IPrintItemDistributorImpl : IPrintItemDistributor
    {
        public abstract void FillPage(PrintPageEventArgs e, System.Collections.ArrayList items, System.Collections.ArrayList coords);

        protected IPrintItemDistributorImpl()
        {
        }

        #region "Properties & stuff"

        public IImageProvider ImageProvider
        {
            get
            {
                return _imageProvider;
            }
            set
            {
                _imageProvider = value;
            }
        }

        public PrintOptions PrintOptions
        {
            get
            {
                return _printOptions;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _printOptions = value;
            }
        }

        #endregion "Properties & stuff"

        public virtual void Initialize(ImagePrintDocument document, PrintPageEventArgs e)
        {
            if (document == null)
                throw new ArgumentNullException("pDocument");

            _document = document;
            _eventHasMoreImages = true;
        }

        protected static void GetValidatedResolution(System.Drawing.Printing.PrintPageEventArgs e, out int resolutionX, out int resolutionY)
        {
            resolutionX = e.PageSettings.PrinterResolution.X;
            resolutionY = e.PageSettings.PrinterResolution.Y;

            if (resolutionX <= 0 && resolutionY > 0)
                resolutionX = resolutionY;

            if (resolutionY <= 0 && resolutionX > 0)
                resolutionY = resolutionX;

            if (resolutionX <= 0 || resolutionY <= 0)
            {
                resolutionX = (int)e.Graphics.DpiX;
                resolutionY = (int)e.Graphics.DpiY;
            }

            if (resolutionX <= 0 || resolutionY <= 0)
                throw new ArgumentException(StringResources.GetString("InvalidPrinterResolution"), "pResolution");
        }

        protected virtual ImagePrintItem GetNextItem(PrintPageEventArgs e)
        {
            if (!_eventHasMoreImages)
                return null;

            // Getting next image & info from IImageProvider
            PrintPlaceholder imagePlaceholder;
            if (_imageProvider != null && !_imageProvider.IsEmpty())
                imagePlaceholder = _imageProvider.GetNext();
            else
                imagePlaceholder = new PrintPlaceholder();

            // Firing PrintImage event in ImagePrintDocument
            QueryImageEventArgs queryImageEventArgs = new QueryImageEventArgs();
            queryImageEventArgs.PrintPlaceholder = imagePlaceholder;
            queryImageEventArgs.PrintOptions = _printOptions;
            OnQueryImageEvent(queryImageEventArgs);

            if (queryImageEventArgs.Cancel == true || queryImageEventArgs.PrintPlaceholder.Image == null)
            {
                e.Cancel = true;
                return null;
            }

            int printerResolutionX, printerResolutionY;
            GetValidatedResolution(e, out printerResolutionX, out printerResolutionY);

            // If we didn't got image from IImageProvider or from user's handler
            // of PrintImage event - just return null.
            if (imagePlaceholder.Image == null)
                return null;

            // Filling result ImagePrintItem
            ImagePrintItem item = new ImagePrintItem(printerResolutionX, printerResolutionY);
            item.Image = imagePlaceholder.Image;
            item.ImageFitMode = _printOptions.ImageFitMode;
            item.ImageAutoRotate = _printOptions.ImageAutoRotate;
            item.InterpolationMode = _printOptions.InterpolationMode;
            item.HeaderFont = _printOptions.HeaderFont;
            item.FooterFont = _printOptions.FooterFont;
            item.HeaderColor = _printOptions.HeaderColor;
            item.FooterColor = _printOptions.FooterColor;
            item.HeaderTrimmming = _printOptions.HeaderTrimming;
            item.FooterTrimmming = _printOptions.FooterTrimming;
            item.HeaderAlignment = _printOptions.HeaderAlignment;
            item.FooterAlignment = _printOptions.FooterAlignment;

            // Header & footer
            if (_printOptions.HeaderEnabled)
                item.HeaderText = imagePlaceholder.Header;
            if (_printOptions.FooterEnabled)
                item.FooterText = imagePlaceholder.Footer;

            if (_printOptions.BorderEnabled)
            {
                int pixelWidth = PrintUtils.InchHundredthsToPixels(_printOptions.BorderWidth, printerResolutionX);
                item.BorderPen = new System.Drawing.Pen(_printOptions.BorderColor, pixelWidth);
            }

            // Setting external size. Size can be redefined in QueryImageEventHandler
            if (imagePlaceholder.Size.Width != 0 && imagePlaceholder.Size.Height != 0)
            {
                Size resizeSize = new Size();
                resizeSize.Width = PrintUtils.InchHundredthsToPixels(imagePlaceholder.Size.Width, printerResolutionX);
                resizeSize.Height = PrintUtils.InchHundredthsToPixels(imagePlaceholder.Size.Height, printerResolutionY);

                item.SetSize(resizeSize);
            }
            else if (_printOptions.PlaceholderSize.Width != 0 && _printOptions.PlaceholderSize.Height != 0)
            {
                Size resizeSize = new Size();
                resizeSize.Width = PrintUtils.InchHundredthsToPixels(_printOptions.PlaceholderSize.Width, printerResolutionX);
                resizeSize.Height = PrintUtils.InchHundredthsToPixels(_printOptions.PlaceholderSize.Height, printerResolutionY);

                item.SetSize(resizeSize);
            }

            return item;
        }

        protected virtual void OnQueryImageEvent(QueryImageEventArgs pQueryImageEventArgs)
        {
            if (_imageProvider != null)
                pQueryImageEventArgs.HasMoreImages = !_imageProvider.IsEmpty();

            _document.OnQueryImageInternal(pQueryImageEventArgs);
            _eventHasMoreImages = pQueryImageEventArgs.HasMoreImages;
        }

        protected bool HasMorePages()
        {
            return _eventHasMoreImages;
        }

        protected IImageProvider _imageProvider;
        protected PrintOptions _printOptions;
        protected ImagePrintDocument _document;
        protected bool _eventHasMoreImages;
    }
}