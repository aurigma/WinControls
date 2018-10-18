// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Image vector object.
    /// All matrix transformations are performed with GDI+.
    /// </summary>
    [System.Serializable]
    public class ImageVObject : VObject
    {
        #region "Construction / destruction"

        public ImageVObject(Aurigma.GraphicsMill.Bitmap image, bool scaleToActualSize, float x, float y)
        {
            if (image == null)
                throw new System.ArgumentNullException("image");

            if (image.IsEmpty)
                throw new System.ArgumentException(StringResources.GetString("ExStrBitmapCannotBeEmpty"), "image");

            base.Name = "image";
            _image = image;

            _scaleToActualSize = scaleToActualSize;

            float widthInPoints = Aurigma.GraphicsMill.UnitConverter.ConvertPixelsToUnits(this.DrawnImage.HorizontalResolution, this.DrawnImage.Width, Aurigma.GraphicsMill.Unit.Point);
            float heightInPoints = Aurigma.GraphicsMill.UnitConverter.ConvertPixelsToUnits(this.DrawnImage.VerticalResolution, this.DrawnImage.Height, Aurigma.GraphicsMill.Unit.Point);

            _rect = new RectangleVObject(0, 0, widthInPoints, heightInPoints);
            _rect.Brush = System.Drawing.Brushes.Transparent;
            _rect.Pen = null;
            _rect.Transform.Translate(x, y);
        }

        protected ImageVObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _image = (Aurigma.GraphicsMill.Bitmap)info.GetValue(SerializationNames.ImageBitmap, typeof(Aurigma.GraphicsMill.Bitmap));
            _rect = (RectangleVObject)info.GetValue(SerializationNames.ImageRectangle, typeof(RectangleVObject));
            _scaleToActualSize = info.GetBoolean(SerializationNames.ImageScaleToActual);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }
                if (_rect != null)
                {
                    _rect.Dispose();
                    _rect = null;
                }
            }
        }

        #endregion "Construction / destruction"

        #region "IVObject implementation"

        public override IDesigner Designer
        {
            get
            {
                if (_editDesigner == null)
                    _editDesigner = new GenericVObjectEditDesigner(this);

                return _editDesigner;
            }
        }

        public override System.Drawing.Drawing2D.Matrix Transform
        {
            get
            {
                return _rect.Transform;
            }
            set
            {
                _rect.Transform = value;
            }
        }

        public override VObjectDrawMode DrawMode
        {
            get
            {
                return _rect.DrawMode;
            }
            set
            {
                _rect.DrawMode = value;
            }
        }

        public override System.Drawing.RectangleF GetVObjectBounds()
        {
            return _rect.GetVObjectBounds();
        }

        public override System.Drawing.RectangleF GetTransformedVObjectBounds()
        {
            return _rect.GetTransformedVObjectBounds();
        }

        public override bool HitTest(System.Drawing.PointF point, float precisionDelta)
        {
            return _rect.HitTest(point, precisionDelta);
        }

        public override void Draw(System.Drawing.Rectangle renderingRect, System.Drawing.Graphics g, ICoordinateMapper coordinateMapper)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");
            if (coordinateMapper == null)
                throw new System.ArgumentNullException("coordinateMapper");

            System.Drawing.Rectangle screenBounds = coordinateMapper.WorkspaceToControl(_rect.GetTransformedVObjectBounds(), Aurigma.GraphicsMill.Unit.Point);
            System.Drawing.Rectangle visiblePart = System.Drawing.Rectangle.Intersect(screenBounds, renderingRect);

            if (visiblePart.Width > 0 && visiblePart.Height > 0)
                DrawInternal(g, coordinateMapper, renderingRect);
        }

        #endregion "IVObject implementation"

        #region "Draw methods"

        private System.Drawing.Bitmap DrawnImage
        {
            get
            {
                if (_drawnImage == null)
                    _drawnImage = CreateDrawnImage();

                return _drawnImage;
            }
        }

        private System.Drawing.Bitmap CreateDrawnImage()
        {
            var converter = new Aurigma.GraphicsMill.Transforms.ColorConverter();
            Aurigma.GraphicsMill.Bitmap interimImage = null;
            System.Drawing.Bitmap result = null;

            System.IntPtr dc = NativeMethods.GetDC(System.IntPtr.Zero);
            float monitorResX = NativeMethods.GetDeviceCaps(dc, NativeMethods.LOGPIXELSX);
            float monitorResY = NativeMethods.GetDeviceCaps(dc, NativeMethods.LOGPIXELSY);
            NativeMethods.ReleaseDC(System.IntPtr.Zero, dc);

            float scaleX, scaleY;
            if (_scaleToActualSize)
            {
                float resX = _image.DpiX;
                float resY = _image.DpiY;

                scaleX = resX > VObject.Eps ? monitorResX / resX : 1.0f;
                scaleY = resY > VObject.Eps ? monitorResY / resY : 1.0f;
            }
            else
            {
                scaleX = 1.0f;
                scaleY = 1.0f;
            }
            bool performScale = System.Math.Abs(scaleX - 1.0f) > VObject.Eps || System.Math.Abs(scaleY - 1.0f) > VObject.Eps;

            try
            {
                if (_image.PixelFormat == Aurigma.GraphicsMill.PixelFormat.Format24bppRgb ||
                    _image.PixelFormat == Aurigma.GraphicsMill.PixelFormat.Format32bppRgb ||
                    _image.PixelFormat == Aurigma.GraphicsMill.PixelFormat.Format32bppArgb)
                {
                    if (!performScale)
                    {
                        result = _image.ToGdiPlusBitmap();
                    }
                    else
                    {
                        interimImage = new Aurigma.GraphicsMill.Bitmap();
                        using (Aurigma.GraphicsMill.Transforms.Resize resize = new Aurigma.GraphicsMill.Transforms.Resize())
                        {
                            resize.Width = (int)(_image.Width * scaleX);
                            resize.Height = (int)(_image.Height * scaleY);
                            resize.InterpolationMode = scaleToActualSizeInterpolationMode;
                            interimImage = resize.Apply(_image);
                        }

                        result = interimImage.ToGdiPlusBitmap();
                    }
                }
                else
                {
                    if (_image.PixelFormat.IsIndexed || _image.HasAlpha)
                        converter.DestinationPixelFormat = Aurigma.GraphicsMill.PixelFormat.Format32bppArgb;
                    else
                        converter.DestinationPixelFormat = Aurigma.GraphicsMill.PixelFormat.Format24bppRgb;

                    interimImage = new Aurigma.GraphicsMill.Bitmap();
                    interimImage = converter.Apply(_image);

                    if (performScale)
                        interimImage.Transforms.Resize((int)(_image.Width * scaleX), (int)(_image.Height * scaleY));

                    result = interimImage.ToGdiPlusBitmap();
                }
            }
            catch
            {
                if (result != null)
                    result.Dispose();
                throw;
            }
            finally
            {
                converter.Dispose();

                if (interimImage != null)
                    interimImage.Dispose();
            }

            result.SetResolution(monitorResX, monitorResY);
            return result;
        }

        private void DrawInternal(System.Drawing.Graphics g, ICoordinateMapper coordinateMapper, System.Drawing.Rectangle renderingRect)
        {
            System.Drawing.Region oldClip = g.Clip;
            g.SetClip(renderingRect);

            System.Drawing.Drawing2D.Matrix oldMatrix = (System.Drawing.Drawing2D.Matrix)g.Transform.Clone();

            try
            {
                if (_rect.DrawMode == VObjectDrawMode.Normal)
                {
                    g.SmoothingMode = normalSmoothingMode;
                    g.InterpolationMode = normalInterpolationMode;
                }
                else
                {
                    g.SmoothingMode = draftSmoothingMode;
                    g.InterpolationMode = draftInterpolationMode;
                }

                System.Drawing.RectangleF bounds = _rect.GetVObjectBounds();
                System.Drawing.RectangleF transformedBounds = _rect.GetTransformedVObjectBounds();
                System.Drawing.Rectangle mappedBounds = coordinateMapper.WorkspaceToControl(transformedBounds, Aurigma.GraphicsMill.Unit.Point);

                if (transformedBounds.Width > VObject.Eps && transformedBounds.Height > VObject.Eps)
                {
                    float scaleX = mappedBounds.Width / transformedBounds.Width,
                          scaleY = mappedBounds.Height / transformedBounds.Height;

                    using (System.Drawing.Drawing2D.Matrix outputMatrix = (System.Drawing.Drawing2D.Matrix)_rect.Transform.Clone())
                    {
                        outputMatrix.Translate(-transformedBounds.X, -transformedBounds.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
                        outputMatrix.Scale(scaleX, scaleY, System.Drawing.Drawing2D.MatrixOrder.Append);
                        outputMatrix.Translate(mappedBounds.X, mappedBounds.Y, System.Drawing.Drawing2D.MatrixOrder.Append);

                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.Transform = outputMatrix;
                        g.DrawImage(this.DrawnImage, 0, 0, bounds.Width, bounds.Height);
                    }
                }
            }
            finally
            {
                g.Transform = oldMatrix;
                g.SetClip(oldClip, System.Drawing.Drawing2D.CombineMode.Replace);
            }
        }

        #endregion "Draw methods"

        #region "ISerializable implementation"

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            base.GetObjectData(info, context);

            info.AddValue(SerializationNames.ImageBitmap, _image);
            info.AddValue(SerializationNames.ImageScaleToActual, _scaleToActualSize);
            info.AddValue(SerializationNames.ImageRectangle, _rect);
        }

        #endregion "ISerializable implementation"

        #region "Member variables & constants"

        private static System.Drawing.Drawing2D.InterpolationMode draftInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        private static System.Drawing.Drawing2D.InterpolationMode normalInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
        private static System.Drawing.Drawing2D.SmoothingMode draftSmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        private static System.Drawing.Drawing2D.SmoothingMode normalSmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        private static Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode scaleToActualSizeInterpolationMode = Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.Anisotropic4;

        private IDesigner _editDesigner;

        private bool _scaleToActualSize;

        private Aurigma.GraphicsMill.Bitmap _image;

        private RectangleVObject _rect;

        private System.Drawing.Bitmap _drawnImage;

        #endregion "Member variables & constants"
    }
}