// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Image vector object designer. Image size can be specified during object creation.
    /// </summary>
    public class ImageVObjectCreateDesigner : ClickDragCreateDesigner
    {
        public ImageVObjectCreateDesigner()
        {
            _scaleToActualSize = true;
        }

        protected new void Reset()
        {
            base.Reset();
            _bitmap = null;
        }

        public override void NotifyConnect(IVObjectHost objectHost)
        {
            if (objectHost == null)
                throw new System.ArgumentNullException("objectHost");

            base.NotifyConnect(objectHost);
        }

        protected override IVObject CreateObject(System.Drawing.RectangleF destinationRectangle)
        {
            IVObject obj = null;
            try
            {
                obj = new ImageVObject(_bitmap, _scaleToActualSize, 0, 0);
                System.Drawing.RectangleF bounds = obj.GetTransformedVObjectBounds();

                if (destinationRectangle.Width != 0 && destinationRectangle.Height != 0)
                {
                    float scaleX = (float)destinationRectangle.Width / bounds.Width,
                          scaleY = (float)destinationRectangle.Height / bounds.Height;

                    obj.Transform.Scale(scaleX, scaleY, System.Drawing.Drawing2D.MatrixOrder.Append);
                }

                obj.Transform.Translate(destinationRectangle.Left, destinationRectangle.Top, System.Drawing.Drawing2D.MatrixOrder.Append);
            }
            catch
            {
                obj = null;
            }

            return obj;
        }

        public bool ScaleToActualSize
        {
            get
            {
                return _scaleToActualSize;
            }
            set
            {
                _scaleToActualSize = value;
            }
        }

        public Aurigma.GraphicsMill.Bitmap Bitmap
        {
            get
            {
                return _bitmap;
            }
            set
            {
                _bitmap = value;
            }
        }

        #region "Member variables"

        private Aurigma.GraphicsMill.Bitmap _bitmap;
        private bool _scaleToActualSize;

        #endregion "Member variables"
    }
}