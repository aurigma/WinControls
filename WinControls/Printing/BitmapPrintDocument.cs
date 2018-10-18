// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Printing;

using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Aurigma.GraphicsMill.WinControls
{
    [ResDescription("PlacementMode")]
    public enum PlacementMode
    {
        [ResDescription("PlacementMode_SingleImage")]
        SingleImage,

        [ResDescription("PlacementMode_MultipleImages")]
        MultipleImages,

        [ResDescription("PlacementMode_Manual")]
        Manual,

        [ResDescription("PlacementMode_Auto")]
        Auto
    }

    [ResDescription("ImageFitMode")]
    public enum ImageFitMode
    {
        [ResDescription("ImageFitMode_CropToFit")]
        CropToFit,

        [ResDescription("ImageFitMode_ResizeToFit")]
        ResizeToFit,

        [ResDescription("ImageFitMode_ShrinkToFit")]
        ShrinkToFit,

        [ResDescription("ImageFitMode_ResizeToFill")]
        ResizeToFill
    }

    [ResDescription("QueryImageEventHandler")]
    public delegate void QueryImageEventHandler(Object sender, QueryImageEventArgs e);

    [AdaptiveToolboxBitmapAttribute(typeof(Aurigma.GraphicsMill.WinControls.ImagePrintDocument), "ImagePrintDocument.bmp")]
    [ResDescription("ImagePrintDocument")]
    public class ImagePrintDocument : PrintDocument
    {
        public ImagePrintDocument()
        {
            base.OriginAtMargins = false;

            _items = new System.Collections.ArrayList();
            _coords = new System.Collections.ArrayList();

            _placementMode = PlacementMode.SingleImage;
            _printOptions = new PrintOptions();
        }

        protected virtual void OnQueryImage(QueryImageEventArgs e)
        {
            if (this.QueryImage != null)
                QueryImage(this, e);
        }

        internal void OnQueryImageInternal(QueryImageEventArgs e)
        {
            OnQueryImage(e);
        }

        protected override void OnBeginPrint(System.Drawing.Printing.PrintEventArgs e)
        {
            if (_imagesSource == null && QueryImage == null)
                e.Cancel = true;

            base.OnBeginPrint(e);
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            if (BeforePrintPage != null)
                BeforePrintPage(this, e);

            DoPrint(e);
            base.OnPrintPage(e);
        }

        protected override void OnEndPrint(PrintEventArgs e)
        {
            _distributor = null;
            _items.Clear();
            _coords.Clear();

            base.OnEndPrint(e);
        }

        protected void DoPrint(PrintPageEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (_distributor == null)
            {
                _distributor = CreateDistributor();
                _distributor.Initialize(this, e);
            }

            if (e.Cancel == true)
            {
                e.HasMorePages = false;
                return;
            }

            System.Drawing.GraphicsUnit originUnit = e.Graphics.PageUnit;
            System.Drawing.Drawing2D.Matrix originTransform = e.Graphics.Transform;
            try
            {
                e.Graphics.PageUnit = System.Drawing.GraphicsUnit.Pixel;
                e.Graphics.Transform = new System.Drawing.Drawing2D.Matrix();

                _distributor.FillPage(e, _items, _coords);
                if (e.Cancel == true)
                {
                    _items.Clear();
                    _coords.Clear();
                    return;
                }

                if (_items.Count != _coords.Count)
                    throw new UnexpectedException();

                for (int i = 0; i < _items.Count; i++)
                    ((ImagePrintItem)_items[i]).Print(e.Graphics, (Point)_coords[i]);
            }
            finally
            {
                e.Graphics.PageUnit = originUnit;
                e.Graphics.Transform = originTransform;
                _items.Clear();
                _coords.Clear();
            }
        }

        internal IPrintItemDistributor CreateDistributor()
        {
            IPrintItemDistributor distributor;
            switch (_placementMode)
            {
                case PlacementMode.SingleImage:
                    distributor = new SingleItemDistributor();
                    break;

                case PlacementMode.MultipleImages:
                    distributor = new MultipleItemsDistributor();
                    break;

                case PlacementMode.Manual:
                    distributor = new ManualItemDistributor();
                    break;

                case PlacementMode.Auto:
                    distributor = new BestFitDistributor();
                    break;

                default:
                    throw new ArgumentException(StringResources.GetString("UnsupportedPlacementMode"), "PlacementMode");
            }

            distributor.PrintOptions = _printOptions;
            distributor.ImageProvider = CreateImageProvider(_imagesSource);

            return distributor;
        }

        internal static IImageProvider CreateImageProvider(object source)
        {
            if (source == null)
                return null;

            IEnumerator enumerator;
            if (source is Bitmap || source is PrintPlaceholder)
            {
                object[] interimArray = new object[1];
                interimArray[0] = source;
                enumerator = interimArray.GetEnumerator();
            }
            else
                enumerator = source as IEnumerator;

            return new FromIEnumeratorProvider(enumerator);
        }

        #region "Events"

        [Browsable(true)]
        [ResDescription("ImagePrintDocument_QueryImage")]
        public event QueryImageEventHandler QueryImage;

        [Browsable(true)]
        [ResDescription("ImagePrintDocument_BeforePrintPage")]
        public event PrintPageEventHandler BeforePrintPage;

        #endregion "Events"

        #region "Properties"

        [Browsable(false)]
        public object Source
        {
            get
            {
                return _imagesSource;
            }
            set
            {
                if (value != null && !(value is Bitmap || value is PrintPlaceholder || value is IEnumerator))
                    throw new ArgumentException(StringResources.GetString("UnexpectedParameterType"), "Source");

                _imagesSource = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.PlacementMode), "SingleImage")]
        [ResDescription("ImagePrintDocument_PlacementMode")]
        public PlacementMode PlacementMode
        {
            get
            {
                return _placementMode;
            }
            set
            {
                _placementMode = value;
            }
        }

        internal IImageProvider ImageProvider
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

        [Browsable(true)]
        [ResDescription("ImagePrintDocument_PrintOptions")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
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

        #endregion "Properties"

        //--------------------------------
        private object _imagesSource;

        private PlacementMode _placementMode;

        internal IPrintItemDistributor _distributor;
        internal IImageProvider _imageProvider;
        private PrintOptions _printOptions;

        private System.Collections.ArrayList _items;
        private System.Collections.ArrayList _coords;
    }

    [ResDescription("PrintPlaceholder")]
    public class PrintPlaceholder
    {
        public PrintPlaceholder()
        {
        }

        public PrintPlaceholder(Bitmap image, string header, string footer, Point location, Size size)
        {
            _image = image;
            _header = header;
            _footer = footer;
            _location = location;
            _size = size;
        }

        #region "Properties & stuff"

        public Bitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        public Point Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }

        public Size Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        public string Footer
        {
            get
            {
                return _footer;
            }
            set
            {
                _footer = value;
            }
        }

        #endregion "Properties & stuff"

        //----------------------------
        private Bitmap _image;

        private string _header;
        private string _footer;
        private Point _location;
        private Size _size;
    }

    public class QueryImageEventArgs : System.EventArgs
    {
        public QueryImageEventArgs()
        {
            _hasMoreImages = true;
        }

        #region "Properties & stuff"

        public bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                _cancel = value;
            }
        }

        public bool HasMoreImages
        {
            get
            {
                return _hasMoreImages;
            }
            set
            {
                _hasMoreImages = value;
            }
        }

        public bool BeginNewPage
        {
            get
            {
                return _beginNewPage;
            }
            set
            {
                _beginNewPage = value;
            }
        }

        public PrintPlaceholder PrintPlaceholder
        {
            get
            {
                return _imagePrintInfo;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _imagePrintInfo = value;
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

        //-------------------------------
        private bool _cancel;

        private bool _beginNewPage;
        private bool _hasMoreImages;
        private PrintPlaceholder _imagePrintInfo;
        private PrintOptions _printOptions;
    }
}