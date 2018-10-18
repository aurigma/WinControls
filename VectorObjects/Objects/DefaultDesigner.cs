// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Default designer. Handles user input and performs object selection & loading specific edit designers.
    /// Supports multiple selection using CompositeVObject.
    /// </summary>
    public class DefaultDesigner : IDesigner, System.IDisposable
    {
        #region "Construction / destruction"

        protected internal DefaultDesigner()
        {
            _objects = new IVObject[0];

            _multiSelect = true;
            _selectionBasePoint = _selectionCurPoint = System.Drawing.Point.Empty;
            _selectionPen = new System.Drawing.Pen(System.Drawing.Color.Indigo, 2);
            _selectionPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
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
            if (disposing && _selectionPen != null)
            {
                try
                {
                    _selectionPen.Dispose();
                }
                catch (System.ArgumentException)
                {
                }
                finally
                {
                    _selectionPen = null;
                }
            }
        }

        #endregion "Construction / destruction"

        private bool IsCurrentLayerInaccessible
        {
            get
            {
                return _objectHost.CurrentLayer == null || _objectHost.CurrentLayer.Locked || !_objectHost.CurrentLayer.Visible;
            }
        }

        #region IDesigner Members

        public void NotifyConnect(IVObjectHost objectHost)
        {
            _objectHost = objectHost;
        }

        public void NotifyDisconnect()
        {
            _objectHost = null;
        }

        public void UpdateSettings()
        {
            if (!this.Connected)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrDesignerShouldBeAttached"));

            _multiSelect = VObjectsUtils.GetBoolDesignerProperty(_objectHost, DesignerSettingsConstants.MultiSelect, _multiSelect);
        }

        public void Draw(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");

            if (_selecting)
                DrawSelectionRectangle(g);
        }

        public bool NotifyMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (this.IsCurrentLayerInaccessible)
                return true;

            if (_multiSelect && e.Button == System.Windows.Forms.MouseButtons.Left && _selecting)
            {
                _selecting = false;
                System.Drawing.Rectangle invalidateRect = this.SelectionRectangle;
                invalidateRect.Inflate(VObject.InvalidationMargin);
                _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(invalidateRect));

                ProcessSelectionRectangle();
            }
            return true;
        }

        public bool NotifyMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (this.IsCurrentLayerInaccessible)
                return true;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                IVObject activeObj = null;
                if (!this.IsCurrentLayerInaccessible)
                    activeObj = _objectHost.CurrentLayer.Find(_objectHost.HostViewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Point), VObject.SelectionPrecisionDelta / _objectHost.HostViewer.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point));

                IDesigner newDesigner = null;
                if (activeObj != null)
                {
                    newDesigner = _objectHost.CurrentDesigner = activeObj.Designer;
                }
                else if (_multiSelect)
                {
                    _selecting = true;
                    _selectionBasePoint = _selectionCurPoint = new System.Drawing.Point(e.X, e.Y);
                }

                if (newDesigner != null)
                    newDesigner.NotifyMouseDown(e);
            }
            return true;
        }

        public bool NotifyMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (_multiSelect && _selecting)
            {
                if (this.IsCurrentLayerInaccessible)
                {
                    System.Drawing.Rectangle invalidateRect = this.SelectionRectangle;
                    _selecting = false;
                    _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(invalidateRect));
                }
                else
                {
                    System.Drawing.Rectangle invalidateRect = this.SelectionRectangle;
                    _selectionCurPoint = new System.Drawing.Point(e.X, e.Y);

                    invalidateRect = System.Drawing.Rectangle.Union(invalidateRect, this.SelectionRectangle);
                    invalidateRect.Inflate(VObject.InvalidationMargin);
                    _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(invalidateRect));
                }
            }

            return true;
        }

        public bool NotifyMouseDoubleClick(System.EventArgs e)
        {
            return true;
        }

        public bool NotifyKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            return true;
        }

        public bool NotifyKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
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

        #region "Selection processing methods"

        private void DrawSelectionRectangle(System.Drawing.Graphics g)
        {
            g.DrawRectangle(_selectionPen, this.SelectionRectangle);
        }

        private void ProcessSelectionRectangle()
        {
            Layer currentLayer = _objectHost.CurrentLayer;

            IVObject[] objects = currentLayer.Find(_objectHost.HostViewer.ControlToWorkspace(this.SelectionRectangle, Aurigma.GraphicsMill.Unit.Point), false);
            if (objects.Length == 1)
            {
                _objectHost.CurrentDesigner = objects[0].Designer;
            }
            else if (objects.Length > 1)
            {
                CompositeVObject groupObj = new CompositeVObject(objects);
                _objectHost.CurrentDesigner = groupObj.Designer;
            }
        }

        #endregion "Selection processing methods"

        #region "Trivial public properties"

        public System.Drawing.Pen SelectionPen
        {
            get
            {
                return _selectionPen;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");
                _selectionPen = value;
            }
        }

        public System.Drawing.Rectangle SelectionRectangle
        {
            get
            {
                int l, t, r, b;

                if (_selectionBasePoint.X < _selectionCurPoint.X)
                {
                    l = _selectionBasePoint.X;
                    r = _selectionCurPoint.X;
                }
                else
                {
                    l = _selectionCurPoint.X;
                    r = _selectionBasePoint.X;
                }

                if (_selectionBasePoint.Y < _selectionCurPoint.Y)
                {
                    t = _selectionBasePoint.Y;
                    b = _selectionCurPoint.Y;
                }
                else
                {
                    t = _selectionCurPoint.Y;
                    b = _selectionBasePoint.Y;
                }

                return System.Drawing.Rectangle.FromLTRB(l, t, r, b);
            }
        }

        #endregion "Trivial public properties"

        #region "Member variables"

        private IVObjectHost _objectHost;
        private IVObject[] _objects;

        private bool _multiSelect;
        private bool _selecting;
        private System.Drawing.Point _selectionBasePoint;
        private System.Drawing.Point _selectionCurPoint;

        private System.Drawing.Pen _selectionPen;

        #endregion "Member variables"
    }
}