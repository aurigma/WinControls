// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Base implementation of the edit-designer. Supports control-points drawing & dragging, object dragging (using GripsProvider object).
    /// </summary>
    public class GenericVObjectEditDesigner : IDesigner, System.IDisposable
    {
        #region "Construction / destruction / initialization"

        protected GenericVObjectEditDesigner()
        {
            _dragPointIndex = GripsProvider.InvalidPointHandle;
            _multiSelect = true;
            _objectBorderPen = new System.Drawing.Pen(System.Drawing.Color.DarkGray, 1.0f);
        }

        public GenericVObjectEditDesigner(IVObject obj)
            : this()
        {
            if (obj == null)
                throw new System.ArgumentNullException("obj");

            _obj = obj;
            _obj.Changed += new System.EventHandler(ObjectChangedHandler);
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
                if (_gripsProvider != null)
                {
                    _gripsProvider.Dispose();
                    _gripsProvider = null;
                }

                if (_contextMenu != null)
                {
                    _contextMenu.Dispose();
                    _contextMenu = null;
                }
            }
        }

        #endregion "Construction / destruction / initialization"

        #region IDesigner Members

        public virtual void NotifyConnect(IVObjectHost objectHost)
        {
            if (objectHost == null)
                throw new System.ArgumentNullException("objectHost");

            _objectHost = objectHost;
            _gripsProvider = new GripsProvider(_obj, _objectHost.HostViewer);
            _gripsProvider.VObjectBorderPen = _objectBorderPen;

            InvalidateObjectArea();
        }

        public virtual void NotifyDisconnect()
        {
            if (!Connected)
                return;

            System.Drawing.Rectangle invalidRect = _gripsProvider.GetInvalidationRectangle();
            _gripsProvider = null;
            _objectHost.HostViewer.RestoreCursorToDefault();
            _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(invalidRect));
            _objectHost = null;
        }

        public virtual void UpdateSettings()
        {
            _resizeProportionallyWithShift = VObjectsUtils.GetBoolDesignerProperty(_objectHost, DesignerSettingsConstants.ResizeProportionallyWithShift, _resizeProportionallyWithShift);
            _multiSelect = VObjectsUtils.GetBoolDesignerProperty(_objectHost, DesignerSettingsConstants.MultiSelect, _multiSelect);
        }

        public virtual void Draw(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");

            if (_gripsProvider != null)
            {
                _gripsProvider.DrawGrips(g);
            }
        }

        public virtual bool NotifyMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (_dragging && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _obj.DrawMode = VObjectDrawMode.Normal;
                _dragging = false;
                _dragPointIndex = GripsProvider.InvalidPointHandle;

                if (_objectHost.UndoRedoEnabled && _objectHost.UndoRedoTrackingEnabled && (System.Math.Abs(e.X - _dragBeginPoint.X) > 0 || System.Math.Abs(e.Y - _dragBeginPoint.Y) > 0))
                    _objectHost.SaveState();

                _obj.Update();
            }

            return true;
        }

        public virtual bool NotifyMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            _dragging = false;
            System.Drawing.Point clickedPoint = new System.Drawing.Point(e.X, e.Y);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //
                // Check if a control points was clicked
                //
                if (_gripsProvider != null)
                {
                    _dragPointIndex = _gripsProvider.TestPoint(clickedPoint);
                    if (_dragPointIndex != GripsProvider.InvalidPointHandle)
                    {
                        _dragging = true;
                        _dragBeginPoint = clickedPoint;
                        return true;
                    }
                }

                //
                // Check for a click on another object
                //
                // If MultiSelect option is on we should also process Ctrl+Click action. If another
                // object has been clicked - it should be added to the selected objects.
                IVObject clickedObj = _objectHost.CurrentLayer.Find(_objectHost.HostViewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Point), VObject.SelectionPrecisionDelta / _objectHost.HostViewer.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point));
                if (clickedObj != _obj)
                {
                    if (_multiSelect && (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control && clickedObj != null && !clickedObj.Locked && !_obj.Locked)
                    {
                        _objectHost.CurrentDesigner = new CompositeVObjectEditDesigner(new IVObject[] { _obj, clickedObj });
                        return true;
                    }
                    else if (clickedObj != null)
                    {
                        _objectHost.CurrentDesigner = clickedObj.Designer;
                        return true;
                    }
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //
                // Context menu handling
                //
                if (_contextMenu != null)
                {
                    IVObject clickedObj = _objectHost.CurrentLayer.Find(_objectHost.HostViewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Point), VObject.SelectionPrecisionDelta / _objectHost.HostViewer.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point));
                    if (clickedObj == _obj)
                        _contextMenu.Show(_objectHost.HostViewer, clickedPoint);
                }

                return true;
            }

            return false;
        }

        public virtual bool NotifyMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (_gripsProvider == null)
                return false;

            if (!_obj.Locked && _dragging && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.Rectangle invalidRect = GetObjectInvalidationArea();
                IControlPointsProvider icpp = _obj as IControlPointsProvider;

                ResizeMode prevResizeMode = ResizeMode.Arbitrary;
                bool resizeProportionally = false;
                if (_resizeProportionallyWithShift && (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift && icpp != null && icpp.SupportedActions.Contains(VObjectAction.Resize))
                {
                    ResizeVObjectAction resizeAction = (ResizeVObjectAction)icpp.SupportedActions[VObjectAction.Resize];
                    if (resizeAction.ResizeMode != ResizeMode.None)
                    {
                        resizeProportionally = true;
                        prevResizeMode = resizeAction.ResizeMode;
                        resizeAction.ResizeMode = ResizeMode.Proportional;
                    }
                }

                _obj.DrawMode = VObjectDrawMode.Draft;
                _gripsProvider.DragPoint(_dragPointIndex, new System.Drawing.Point(e.X, e.Y));

                if (resizeProportionally)
                    ((ResizeVObjectAction)icpp.SupportedActions[VObjectAction.Resize]).ResizeMode = prevResizeMode;

                invalidRect = System.Drawing.Rectangle.Union(invalidRect, GetObjectInvalidationArea());
                _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(invalidRect, _objectHost.CurrentLayer));
            }
            else
            {
                UpdateCursor(e.X, e.Y);
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
            return true;
        }

        public virtual IVObject[] VObjects
        {
            get
            {
                return new IVObject[] { _obj };
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

        #region "Other members - unsorted"

        private System.Drawing.Rectangle GetObjectInvalidationArea()
        {
            System.Drawing.Rectangle result = GripsProvider.GetInvalidationRectangle();
            result = System.Drawing.Rectangle.Union(result, _objectHost.HostViewer.WorkspaceToControl(_obj.GetTransformedVObjectBounds(), Aurigma.GraphicsMill.Unit.Point));
            result.Inflate(VObject.InvalidationMargin);
            return result;
        }

        protected void InvalidateObjectArea()
        {
            _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(GetObjectInvalidationArea(), _objectHost.CurrentLayer));
        }

        protected void InvalidateDesigner()
        {
            System.Drawing.Rectangle invalidationRect = GripsProvider.GetInvalidationRectangle();
            invalidationRect.Inflate(VObject.InvalidationMargin);
            _objectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(invalidationRect));
        }

        private void UpdateCursor(int x, int y)
        {
            int point = _gripsProvider.TestPoint(new System.Drawing.Point(x, y));
            if (point != GripsProvider.InvalidPointHandle)
                _objectHost.HostViewer.Cursor = GripsProvider.GetCursor(point);
            else
                _objectHost.HostViewer.RestoreCursorToDefault();
        }

        protected virtual void ObjectChangedHandler(object sender, System.EventArgs e)
        {
            if (this.Connected)
                InvalidateObjectArea();
        }

        #endregion "Other members - unsorted"

        #region "Trivial properties"

        protected IVObject ActualVObject
        {
            get
            {
                return _obj;
            }
            set
            {
                _obj = value;
            }
        }

        protected IVObjectHost VObjectHost
        {
            get
            {
                return _objectHost;
            }
        }

        protected bool Dragging
        {
            get
            {
                return _dragging;
            }
            set
            {
                _dragging = value;
            }
        }

        protected int DraggingPointIndex
        {
            get
            {
                return _dragPointIndex;
            }
            set
            {
                _dragPointIndex = value;
            }
        }

        protected bool MultiSelect
        {
            get
            {
                return _multiSelect;
            }
            set
            {
                _multiSelect = value;
            }
        }

        internal GripsProvider GripsProvider
        {
            get
            {
                return _gripsProvider;
            }
        }

        public System.Windows.Forms.ContextMenu ContextMenu
        {
            get
            {
                return _contextMenu;
            }
            set
            {
                _contextMenu = value;
            }
        }

        public System.Drawing.Pen ObjectBorderPen
        {
            get
            {
                return _objectBorderPen;
            }
            set
            {
                _objectBorderPen = value;
                if (_gripsProvider != null)
                    _gripsProvider.VObjectBorderPen = value;
            }
        }

        #endregion "Trivial properties"

        #region "Member variables"

        private IVObject _obj;

        private GripsProvider _gripsProvider;
        private IVObjectHost _objectHost;

        private bool _dragging;
        private int _dragPointIndex;
        private System.Drawing.Point _dragBeginPoint;

        private bool _multiSelect;
        private bool _resizeProportionallyWithShift;

        private System.Windows.Forms.ContextMenu _contextMenu;
        private System.Drawing.Pen _objectBorderPen;

        #endregion "Member variables"
    }
}