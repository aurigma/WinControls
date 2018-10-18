// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    ///	Base class for create designers of path-based vector objects.
    ///	Proposed implementation provides code of pen/brush properties, base implementation
    ///	of the attach/detach actions. Inheritors should implement at least
    ///	CreateObject(), GetInvalidationRectangle() and IDesigner.Draw() methods.
    /// </summary>
    public abstract class PathVObjectCreateDesigner : IDesigner, System.IDisposable
    {
        #region "Static part"

        internal static System.Drawing.Drawing2D.Matrix AdaptBrushToViewport(System.Drawing.Brush brush, Aurigma.GraphicsMill.WinControls.ICoordinateMapper coordinateMapper)
        {
            System.Drawing.Drawing2D.Matrix originalMatrix = null;

            if (brush != null && brush.GetType() != typeof(System.Drawing.SolidBrush) && brush.GetType() != typeof(System.Drawing.Drawing2D.HatchBrush))
            {
                originalMatrix = VObjectsUtils.GetBrushMatrix(brush);

                System.Drawing.Point viewportTranslation = coordinateMapper.WorkspaceToControl(System.Drawing.PointF.Empty, Aurigma.GraphicsMill.Unit.Pixel);
                float scale = coordinateMapper.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point);

                System.Drawing.Drawing2D.Matrix brushMatrix = new System.Drawing.Drawing2D.Matrix();
                brushMatrix.Scale(scale, scale, System.Drawing.Drawing2D.MatrixOrder.Append);
                brushMatrix.Translate(viewportTranslation.X, viewportTranslation.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
                brushMatrix.Multiply(originalMatrix, System.Drawing.Drawing2D.MatrixOrder.Prepend);

                VObjectsUtils.SetBrushMatrix(brush, brushMatrix);
            }

            return originalMatrix;
        }

        #endregion "Static part"

        #region "Construction / destruction"

        protected PathVObjectCreateDesigner()
        {
            _objects = new IVObject[0];

            _pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0, 0, 0), 1.0f);
            _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
            _brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 255, 255));

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
            if (disposing)
            {
                if (_pen != null)
                {
                    try
                    {
                        _pen.Dispose();
                    }
                    catch (System.ArgumentException)
                    {
                    }
                    finally
                    {
                        _pen = null;
                    }
                }
                if (_brush != null)
                {
                    try
                    {
                        _brush.Dispose();
                    }
                    catch (System.ArgumentException)
                    {
                    }
                    finally
                    {
                        _brush = null;
                    }
                }
            }
        }

        #endregion "Construction / destruction"

        protected void Reset()
        {
            _objectHost = null;
        }

        #region IDesigner Members

        public virtual void NotifyConnect(IVObjectHost objectHost)
        {
            _objectHost = objectHost;
        }

        public virtual void UpdateSettings()
        {
        }

        public virtual void NotifyDisconnect()
        {
            Reset();
        }

        public abstract void Draw(System.Drawing.Graphics g);

        public virtual bool NotifyMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            return true;
        }

        public virtual bool NotifyMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            return true;
        }

        public virtual bool NotifyMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
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

        public bool Connected
        {
            get
            {
                return _objectHost != null;
            }
        }

        public IVObject[] VObjects
        {
            get
            {
                return _objects;
            }
        }

        #endregion IDesigner Members

        protected abstract IVObject CreateObject();

        protected abstract System.Drawing.Rectangle GetInvalidationRectangle();

        protected void CreateObjectAndDetach()
        {
            IVObject obj = CreateObject();

            if (obj != null)
            {
                _objectHost.CurrentLayer.VObjects.Add(obj);
                InvalidateObject(obj);
            }

            SwitchToDefaultDesigner();
        }

        protected void SwitchToDefaultDesigner()
        {
            _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(GetInvalidationRectangle()));
            _objectHost.CurrentDesigner = _objectHost.DefaultDesigner;
        }

        protected void InvalidateObject(IVObject obj)
        {
            if (obj == null)
                throw new System.ArgumentNullException("obj");

            System.Drawing.Rectangle areaToInvalidate = _objectHost.HostViewer.WorkspaceToControl(obj.GetTransformedVObjectBounds(), Aurigma.GraphicsMill.Unit.Point);
            areaToInvalidate.Inflate(VObject.InvalidationMargin);
            _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(areaToInvalidate, _objectHost.CurrentLayer));
        }

        public System.Drawing.Pen Pen
        {
            get
            {
                return _pen;
            }
            set
            {
                if (_pen != value)
                {
                    _pen = value;

                    if (this.Connected)
                        _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(GetInvalidationRectangle()));
                }
            }
        }

        protected System.Drawing.Pen PenInternal
        {
            get
            {
                return _pen;
            }
            set
            {
                _pen = value;
            }
        }

        public System.Drawing.Brush Brush
        {
            get
            {
                return _brush;
            }
            set
            {
                _brush = value;
                if (Connected)
                    _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(GetInvalidationRectangle()));
            }
        }

        protected System.Drawing.Brush BrushInternal
        {
            get
            {
                return _brush;
            }
            set
            {
                _brush = value;
            }
        }

        protected IVObjectHost VObjectHost
        {
            get
            {
                return _objectHost;
            }
        }

        protected System.Drawing.Pen CreateViewportPen()
        {
            if (_pen == null)
                return null;

            System.Drawing.Pen result = (System.Drawing.Pen)_pen.Clone();
            result.Width = _pen.Width * _objectHost.HostViewer.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point);
            return result;
        }

        #region "-------- Member variables ---------"

        private IVObject[] _objects;
        private IVObjectHost _objectHost;

        private System.Drawing.Pen _pen;
        private System.Drawing.Brush _brush;

        #endregion "-------- Member variables ---------"
    }
}