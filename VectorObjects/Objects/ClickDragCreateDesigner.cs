// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Base class for designers which can specify location or destination area of a vector object
    /// (e.g. text & image vector objects).
    /// </summary>
    public abstract class ClickDragCreateDesigner : IDesigner, System.IDisposable
    {
        #region "-------- Member variables ---------"

        private IVObjectHost _objectHost;
        private bool _areaOriginDefined;
        private bool _areaDefined;
        private System.Drawing.PointF[] _cornerPoints;

        private System.Drawing.Pen _borderPen;
        private IVObject[] _objects;

        #endregion "-------- Member variables ---------"

        #region "Construction / destruction"

        protected ClickDragCreateDesigner()
        {
            _objects = new IVObject[0];

            _borderPen = new System.Drawing.Pen(System.Drawing.Color.Blue, 2);
            _borderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
            _borderPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

            Reset();
        }

        public void Dispose()
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
            if (disposing && _borderPen != null)
            {
                try
                {
                    _borderPen.Dispose();
                }
                catch (System.ArgumentException)
                {
                }
                finally
                {
                    _borderPen = null;
                }
            }
        }

        protected void Reset()
        {
            _areaOriginDefined = false;
            _areaDefined = false;
            _objectHost = null;
            _cornerPoints = new System.Drawing.PointF[2];
        }

        #endregion "Construction / destruction"

        #region IDesigner Members

        public virtual void NotifyConnect(IVObjectHost objectHost)
        {
            _objectHost = objectHost;
        }

        public virtual void NotifyDisconnect()
        {
            Reset();
        }

        public virtual void Draw(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");

            if (_areaOriginDefined && _areaDefined)
            {
                g.DrawRectangle(_borderPen, _objectHost.HostViewer.WorkspaceToControl(this.Area, Aurigma.GraphicsMill.Unit.Point));
            }
        }

        public void UpdateSettings()
        {
        }

        public virtual bool NotifyMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (e.Button == System.Windows.Forms.MouseButtons.Left && _areaOriginDefined)
            {
                System.Drawing.RectangleF destinationRect;
                if (_areaDefined)
                    destinationRect = VObjectsUtils.GetBoundingRectangle(_cornerPoints);
                else
                    destinationRect = new System.Drawing.RectangleF(_cornerPoints[0], System.Drawing.SizeF.Empty);

                System.Drawing.Rectangle invalidationRect = System.Drawing.Rectangle.Empty;
                IVObject obj = CreateObject(destinationRect);
                if (obj != null)
                {
                    _objectHost.CurrentLayer.VObjects.Add(obj);
                    invalidationRect = _objectHost.HostViewer.WorkspaceToControl(obj.GetTransformedVObjectBounds(), Aurigma.GraphicsMill.Unit.Point);
                }

                if (_areaDefined)
                {
                    System.Drawing.Rectangle tmp = _objectHost.HostViewer.WorkspaceToControl(destinationRect, Aurigma.GraphicsMill.Unit.Point);
                    tmp.Inflate((int)_borderPen.Width * 2, (int)_borderPen.Width * 2);
                    invalidationRect = System.Drawing.Rectangle.Union(invalidationRect, tmp);
                    _areaDefined = false;
                }

                _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(invalidationRect, _objectHost.CurrentLayer));
                _objectHost.CurrentDesigner = _objectHost.DefaultDesigner;
            }

            return true;
        }

        public virtual bool NotifyMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _areaOriginDefined = true;
                _cornerPoints[0] = _objectHost.HostViewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Point);
            }

            return true;
        }

        public virtual bool NotifyMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (_areaOriginDefined)
            {
                System.Drawing.RectangleF invalidationRect = System.Drawing.RectangleF.Empty;
                if (_areaDefined)
                    invalidationRect = VObjectsUtils.GetBoundingRectangle(_cornerPoints);

                _areaDefined = true;
                _cornerPoints[1] = _objectHost.HostViewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Point);

                invalidationRect = System.Drawing.RectangleF.Union(invalidationRect, VObjectsUtils.GetBoundingRectangle(_cornerPoints));
                invalidationRect.Inflate((int)_borderPen.Width * 2, (int)_borderPen.Width * 2);
                _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(_objectHost.HostViewer.WorkspaceToControl(invalidationRect, Aurigma.GraphicsMill.Unit.Point)));
            }

            return true;
        }

        public virtual bool NotifyMouseDoubleClick(System.EventArgs e)
        {
            return true;
        }

        public virtual bool NotifyKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            return true;
        }

        public virtual bool NotifyKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
                SwitchToDefaultDesigner();

            return true;
        }

        public IVObject[] VObjects
        {
            get
            {
                return _objects;
            }
        }

        public bool Connected
        {
            get
            {
                return _objectHost != null;
            }
        }

        #endregion IDesigner Members

        protected abstract IVObject CreateObject(System.Drawing.RectangleF destinationRectangle);

        protected void SwitchToDefaultDesigner()
        {
            System.Drawing.RectangleF invalidationRect = VObjectsUtils.GetBoundingRectangle(_cornerPoints);
            invalidationRect.Inflate((int)_borderPen.Width * 2, (int)_borderPen.Width * 2);
            _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(_objectHost.HostViewer.WorkspaceToControl(invalidationRect, Aurigma.GraphicsMill.Unit.Point)));

            _objectHost.CurrentDesigner = _objectHost.DefaultDesigner;
        }

        protected IVObjectHost VObjectHost
        {
            get
            {
                return _objectHost;
            }
        }

        protected bool AreaOriginDefined
        {
            get
            {
                return _areaOriginDefined;
            }
        }

        protected bool AreaDefined
        {
            get
            {
                return _areaDefined;
            }
        }

        protected System.Drawing.RectangleF Area
        {
            get
            {
                if (!_areaDefined)
                    return System.Drawing.Rectangle.Empty;

                return VObjectsUtils.GetBoundingRectangle(_cornerPoints);
            }
        }

        protected System.Drawing.Pen BorderPen
        {
            get
            {
                return _borderPen;
            }
            set
            {
                if (_borderPen == null)
                    throw new System.ArgumentNullException("value");

                _borderPen = value;
            }
        }
    }
}