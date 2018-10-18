// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;

namespace Aurigma.GraphicsMill.WinControls
{
    [ResDescription("IUserInputController")]
    public interface IUserInputController
    {
        void Connect(Aurigma.GraphicsMill.WinControls.ViewerBase viewer);

        void Disconnect();

        void Update();
    }

    [ResDescription("INavigator")]
    public interface INavigator : IUserInputController
    {
    }

    [ResDescription("IRubberband")]
    public interface IRubberband : IUserInputController
    {
    }

    [ResDescription("UserInputController")]
    public abstract class UserInputController : System.ComponentModel.Component, IUserInputController
    {
        internal Aurigma.GraphicsMill.WinControls.ViewerBase _viewer;

        protected Aurigma.GraphicsMill.WinControls.ViewerBase Viewer
        {
            get
            {
                return _viewer;
            }
        }

        public virtual void Update()
        {
            UpdateCursor(false, System.Drawing.Point.Empty);
        }

        public virtual void Connect(Aurigma.GraphicsMill.WinControls.ViewerBase viewer)
        {
            _viewer = viewer;

            if (_viewer != null)
            {
                _viewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ViewerMouseMoveEventHandler);
                _viewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewerMouseDownEventHandler);
                _viewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ViewerMouseUpEventHandler);
                _viewer.Zoomed += new System.EventHandler(this.ViewerZoomedEventHandler);
                _viewer.WorkspaceChanged += new System.EventHandler(this.ViewerBitmapChangedEventHandler);
                _viewer.DoubleBufferPaint += new System.Windows.Forms.PaintEventHandler(this.ViewerDoubleBufferPaintEventHandler);
                _viewer.Scrolled += new System.EventHandler(this.ViewerScrolledEventHandler);
                _viewer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewerKeyUpEventHandler);
                _viewer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewerKeyDownEventHandler);
                _viewer.DoubleClick += new System.EventHandler(this.ViewerDoubleClickEventHandler);
                _viewer.SizeChanged += new System.EventHandler(this.ViewerSizeChangedEventHandler);

                Update();
            }
        }

        public virtual void Disconnect()
        {
            if (_viewer != null)
            {
                UpdateCursor(true, System.Drawing.Point.Empty);

                _viewer.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.ViewerMouseMoveEventHandler);
                _viewer.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.ViewerMouseDownEventHandler);
                _viewer.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.ViewerMouseUpEventHandler);
                _viewer.Zoomed -= new System.EventHandler(this.ViewerZoomedEventHandler);
                _viewer.WorkspaceChanged -= new System.EventHandler(this.ViewerBitmapChangedEventHandler);
                _viewer.DoubleBufferPaint -= new System.Windows.Forms.PaintEventHandler(this.ViewerDoubleBufferPaintEventHandler);
                _viewer.Scrolled -= new System.EventHandler(this.ViewerScrolledEventHandler);
                _viewer.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.ViewerKeyUpEventHandler);
                _viewer.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.ViewerKeyDownEventHandler);
                _viewer.DoubleClick -= new System.EventHandler(this.ViewerDoubleClickEventHandler);
                _viewer.SizeChanged -= new System.EventHandler(this.ViewerSizeChangedEventHandler);

                _viewer.RestoreCursorToDefault();
                _viewer.InvalidateViewer();
            }

            _viewer = null;
        }

        private void ViewerMouseMoveEventHandler(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.IsUserInputEnabled)
                OnViewerMouseMove(e);
        }

        private void ViewerMouseDownEventHandler(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.IsUserInputEnabled)
                OnViewerMouseDown(e);
        }

        private void ViewerMouseUpEventHandler(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.IsUserInputEnabled)
                OnViewerMouseUp(e);
        }

        private void ViewerZoomedEventHandler(object sender, System.EventArgs e)
        {
            OnViewerZoomed(e);
        }

        private void ViewerBitmapChangedEventHandler(object sender, System.EventArgs e)
        {
            OnViewerContentChanged(e);
        }

        private void ViewerDoubleBufferPaintEventHandler(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            OnViewerDoubleBufferPaint(e);
        }

        private void ViewerScrolledEventHandler(object sender, System.EventArgs e)
        {
            OnViewerScrolled(e);
        }

        private void ViewerKeyDownEventHandler(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (this.IsUserInputEnabled)
                OnViewerKeyDown(e);
        }

        private void ViewerKeyUpEventHandler(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (this.IsUserInputEnabled)
                OnViewerKeyUp(e);
        }

        private void ViewerDoubleClickEventHandler(object sender, System.EventArgs e)
        {
            if (this.IsUserInputEnabled)
                OnViewerDoubleClick(e);
        }

        private void ViewerSizeChangedEventHandler(object sender, System.EventArgs e)
        {
            OnViewerSizeChanged(e);
        }

        protected virtual void OnViewerMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
        }

        protected virtual void OnViewerMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
        }

        protected virtual void OnViewerMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
        }

        protected virtual void OnViewerZoomed(System.EventArgs e)
        {
        }

        protected virtual void OnViewerContentChanged(System.EventArgs e)
        {
            Update();
        }

        protected virtual void OnViewerDoubleBufferPaint(System.Windows.Forms.PaintEventArgs e)
        {
        }

        protected virtual void OnViewerScrolled(System.EventArgs e)
        {
        }

        protected virtual void OnViewerKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
        }

        protected virtual void OnViewerKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
        }

        protected virtual void OnViewerDoubleClick(System.EventArgs e)
        {
        }

        protected virtual void OnViewerSizeChanged(System.EventArgs e)
        {
            Update();
        }

        protected bool IsViewerAttached
        {
            get
            {
                return _viewer != null;
            }
        }

        protected bool ViewerHasContent
        {
            get
            {
                return (_viewer != null && _viewer.HasContent);
            }
        }

        protected bool IsUserInputEnabled
        {
            get
            {
                return (_viewer != null && _viewer.Navigator == this) || (_viewer != null && _viewer.Navigator == null);
            }
        }

        internal static int FitToBounds(int value, int minValue, int maxValue)
        {
            return Math.Min(Math.Max(minValue, value), maxValue);
        }

        protected virtual void UpdateCursor(bool isCursorDefault, System.Drawing.Point point)
        {
            if (this.IsUserInputEnabled)
                _viewer.RestoreCursorToDefault();
        }
    }

    [ResDescription("PanNavigator")]
    [AdaptiveToolboxBitmapAttribute(typeof(Aurigma.GraphicsMill.WinControls.PanNavigator), "PanNavigator.bmp")]
    public sealed class PanNavigator : UserInputController, INavigator
    {
        private System.Drawing.Point _beginPoint;
        private System.Windows.Forms.Cursor _cursor;

        public PanNavigator()
        {
            _cursor = new System.Windows.Forms.Cursor(GetType(), "BitmapViewer.Pan.cur");
        }

        protected override void OnViewerMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _viewer.Scroll(_beginPoint.X - e.X, _beginPoint.Y - e.Y);
                _beginPoint = new System.Drawing.Point(e.X, e.Y);
            }
        }

        protected override void OnViewerMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            _beginPoint = new System.Drawing.Point(e.X, e.Y);
        }

        protected override void UpdateCursor(bool isCursorDefault, System.Drawing.Point point)
        {
            if (this.IsUserInputEnabled)
            {
                if (isCursorDefault)
                    _viewer.RestoreCursorToDefault();
                else
                    _viewer.Cursor = _cursor;
            }
        }
    }

    [ResDescription("ZoomInNavigator")]
    [AdaptiveToolboxBitmapAttribute(typeof(Aurigma.GraphicsMill.WinControls.ZoomInNavigator), "ZoomInNavigator.bmp")]
    public sealed class ZoomInNavigator : UserInputController, INavigator
    {
        private System.Windows.Forms.Cursor _cursor;

        public ZoomInNavigator()
        {
            _cursor = new System.Windows.Forms.Cursor(GetType(), "BitmapViewer.ZoomIn.cur");
        }

        protected override void OnViewerMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            if (_viewer.ZoomMode != Aurigma.GraphicsMill.WinControls.ZoomMode.ZoomControl)
                _viewer.ZoomMode = Aurigma.GraphicsMill.WinControls.ZoomMode.None;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.Rectangle rectangle = _viewer.GetViewportBounds();
                System.Drawing.Point objCenter = new System.Drawing.Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
                System.Drawing.Point position = _viewer.ScrollingPosition;

                position.X += e.X - objCenter.X;
                position.Y += e.Y - objCenter.Y;

                _viewer.ScrollingPosition = position;
                _viewer.Zoom *= _viewer.WheelZoomAmount;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                System.Drawing.Rectangle rectangle = _viewer.GetViewportBounds();
                System.Drawing.Point objCenter = new System.Drawing.Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
                System.Drawing.Point position = _viewer.ScrollingPosition;

                position.X += e.X - objCenter.X;
                position.Y += e.Y - objCenter.Y;

                _viewer.ScrollingPosition = position;
                _viewer.Zoom /= _viewer.WheelZoomAmount;
            }
        }

        protected override void OnViewerZoomed(System.EventArgs e)
        {
            if (!ViewerHasContent)
                return;

            UpdateCursor(false, System.Drawing.Point.Empty);
        }

        protected override void OnViewerContentChanged(System.EventArgs e)
        {
            if (!ViewerHasContent)
                return;

            UpdateCursor(false, System.Drawing.Point.Empty);
        }

        protected override void UpdateCursor(bool isCursorDefault, System.Drawing.Point point)
        {
            if (this.IsUserInputEnabled)
            {
                if (isCursorDefault || !_viewer.HasContent)
                    _viewer.RestoreCursorToDefault();
                else
                    _viewer.Cursor = _cursor;
            }
        }
    }

    [ResDescription("ZoomOutNavigator")]
    [AdaptiveToolboxBitmapAttribute(typeof(Aurigma.GraphicsMill.WinControls.ZoomOutNavigator), "ZoomOutNavigator.bmp")]
    public sealed class ZoomOutNavigator : UserInputController, INavigator
    {
        private System.Windows.Forms.Cursor _cursor;

        public ZoomOutNavigator()
        {
            _cursor = new System.Windows.Forms.Cursor(GetType(), "BitmapViewer.ZoomOut.cur");
        }

        protected override void OnViewerMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            if (_viewer.ZoomMode != Aurigma.GraphicsMill.WinControls.ZoomMode.ZoomControl)
                _viewer.ZoomMode = Aurigma.GraphicsMill.WinControls.ZoomMode.None;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.Rectangle rectangle = _viewer.GetViewportBounds();
                System.Drawing.Point objCenter = new System.Drawing.Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
                System.Drawing.Point position = _viewer.ScrollingPosition;

                position.X += e.X - objCenter.X;
                position.Y += e.Y - objCenter.Y;

                _viewer.ScrollingPosition = position;
                _viewer.Zoom /= _viewer.WheelZoomAmount;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                System.Drawing.Rectangle rectangle = _viewer.GetViewportBounds();
                System.Drawing.Point objCenter = new System.Drawing.Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
                System.Drawing.Point position = _viewer.ScrollingPosition;

                position.X += e.X - objCenter.X;
                position.Y += e.Y - objCenter.Y;

                _viewer.ScrollingPosition = position;
                _viewer.Zoom *= _viewer.WheelZoomAmount;
            }
        }

        protected override void OnViewerZoomed(System.EventArgs e)
        {
            if (!ViewerHasContent)
                return;

            UpdateCursor(false, System.Drawing.Point.Empty);
        }

        protected override void OnViewerContentChanged(System.EventArgs e)
        {
            if (!ViewerHasContent)
                return;

            UpdateCursor(false, System.Drawing.Point.Empty);
        }

        protected override void UpdateCursor(bool isCursorDefault, System.Drawing.Point point)
        {
            if (this.IsUserInputEnabled)
            {
                if (isCursorDefault || !_viewer.HasContent)
                    _viewer.RestoreCursorToDefault();
                else
                    _viewer.Cursor = _cursor;
            }
        }
    }

    [ResDescription("Enum_ResizeMode")]
    public enum ResizeMode
    {
        [ResDescription("Enum_ResizeMode_None")]
        None = 0,

        [ResDescription("Enum_ResizeMode_Proportional")]
        Proportional = 1,

        [ResDescription("Enum_ResizeMode_Arbitrary")]
        Arbitrary = 2
    }

    [ResDescription("Enum_RectangleChangeMode")]
    public enum RectangleChangeMode
    {
        [ResDescription("Enum_RectangleChangeMode_Changed")]
        Changed = 0,

        [ResDescription("Enum_RectangleChangeMode_Moving")]
        Moving = 1,

        [ResDescription("Enum_RectangleChangeMode_Resizing")]
        Resizing = 2
    }

    [ResDescription("RectangleEventArgs")]
    public class RectangleEventArgs : System.EventArgs
    {
        internal System.Drawing.Rectangle _rectangle;
        internal Aurigma.GraphicsMill.WinControls.RectangleChangeMode _changeMode;

        public RectangleEventArgs(System.Drawing.Rectangle rectangle, Aurigma.GraphicsMill.WinControls.RectangleChangeMode changeMode)
        {
            _rectangle = rectangle;
            _changeMode = changeMode;
        }

        [System.ComponentModel.Browsable(false)]
        public System.Drawing.Rectangle Rectangle
        {
            get
            {
                return _rectangle;
            }

            set
            {
                _rectangle = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public Aurigma.GraphicsMill.WinControls.RectangleChangeMode ChangeMode
        {
            get
            {
                return _changeMode;
            }

            set
            {
                _changeMode = value;
            }
        }
    }

    public delegate void RectangleEventHandler(object sender, RectangleEventArgs e);

    [ResDescription("Enum_MaskStyle")]
    public enum MaskStyle
    {
        [ResDescription("Enum_MaskStyle_None")]
        None = 0,

        [ResDescription("Enum_MaskStyle_Always")]
        Always = 1,

        [ResDescription("Enum_MaskStyle_HideOnChange")]
        HideOnChange = 2
    }

    [ResDescription("RectangleController")]
    public abstract class RectangleController : UserInputController
    {
        internal enum HandlingMode
        {
            None = 0,
            DragSelect = 1,
            Select = 2,
            LeftSelectResize = 3,
            RightSelectResize = 4,
            TopSelectResize = 5,
            BottomSelectResize = 6,
            LeftTopSelectResize = 7,
            RightTopSelectResize = 8,
            LeftBottomSelectResize = 9,
            RightBottomSelectResize = 10
        }

        internal System.Windows.Forms.Cursor _cursor;
        internal int _gripSize = 8;
        internal System.Drawing.Color _gripColor = System.Drawing.Color.FromArgb(0, 153, 0);
        internal System.Drawing.Pen _outlinePen1;
        internal System.Drawing.Pen _outlinePen2;
        internal System.Drawing.Color _outlineColor1 = System.Drawing.Color.FromArgb(62, 154, 222);
        internal System.Drawing.Color _outlineColor2 = System.Drawing.Color.FromArgb(255, 255, 255);
        internal int _outlineWidth = 1;
        internal System.Drawing.Drawing2D.DashStyle _outlineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        internal System.Drawing.Rectangle _rectangleOnBitmap = System.Drawing.Rectangle.Empty;
        internal HandlingMode _handlingMode = HandlingMode.None;
        internal System.Drawing.Point _point1 = System.Drawing.Point.Empty;
        internal System.Drawing.Point _point2 = System.Drawing.Point.Empty;
        internal System.Drawing.Point _point3 = System.Drawing.Point.Empty;
        internal System.Drawing.Rectangle _leftTopGrip = System.Drawing.Rectangle.Empty;
        internal System.Drawing.Rectangle _topGrip = System.Drawing.Rectangle.Empty;
        internal System.Drawing.Rectangle _rightTopGrip = System.Drawing.Rectangle.Empty;
        internal System.Drawing.Rectangle _rightGrip = System.Drawing.Rectangle.Empty;
        internal System.Drawing.Rectangle _rightBottomGrip = System.Drawing.Rectangle.Empty;
        internal System.Drawing.Rectangle _bottomGrip = System.Drawing.Rectangle.Empty;
        internal System.Drawing.Rectangle _leftBottomGrip = System.Drawing.Rectangle.Empty;
        internal System.Drawing.Rectangle _leftGrip = System.Drawing.Rectangle.Empty;
        internal bool _shiftPressed;
        internal bool _ctrlPressed;
        internal double _curSelectionRatio;
        internal double _selectionRatio;
        internal bool _movable = true;
        internal bool _erasable = true;
        internal Aurigma.GraphicsMill.WinControls.ResizeMode _resizeMode = Aurigma.GraphicsMill.WinControls.ResizeMode.Arbitrary;
        internal bool _gripsVisible = true;
        internal Aurigma.GraphicsMill.WinControls.MaskStyle _maskStyle = Aurigma.GraphicsMill.WinControls.MaskStyle.Always;
        internal System.Drawing.Color _maskColor = System.Drawing.Color.FromArgb(127, 255, 51, 0);
        internal System.Drawing.Rectangle _selectionDrawingRectangle;

        protected RectangleController()
        {
            _rightBottomGrip = System.Drawing.Rectangle.Empty;
            _movable = true;
            _erasable = true;
            _resizeMode = Aurigma.GraphicsMill.WinControls.ResizeMode.Arbitrary;
            _cursor = new System.Windows.Forms.Cursor(typeof(RectangleController), "BitmapViewer.Select.cur");

            UpdateOutlinePen();
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(System.Drawing.Color), "62; 154; 222")]
        [ResDescription("RectangleController_OutlineColor1")]
        public System.Drawing.Color OutlineColor1
        {
            get
            {
                return _outlineColor1;
            }

            set
            {
                _outlineColor1 = value;
                UpdateOutlinePen();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(System.Drawing.Color), "255; 255; 255")]
        [ResDescription("RectangleController_OutlineColor2")]
        public System.Drawing.Color OutlineColor2
        {
            get
            {
                return _outlineColor2;
            }

            set
            {
                _outlineColor2 = value;
                UpdateOutlinePen();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(1)]
        [ResDescription("RectangleController_OutlineWidth")]
        public int OutlineWidth
        {
            get
            {
                return _outlineWidth;
            }

            set
            {
                _outlineWidth = Math.Abs(value);
                UpdateOutlinePen();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(System.Drawing.Drawing2D.DashStyle), "Dash")]
        [ResDescription("RectangleController_OutlineStyle")]
        public System.Drawing.Drawing2D.DashStyle OutlineStyle
        {
            get
            {
                return _outlineStyle;
            }

            set
            {
                _outlineStyle = value;
                UpdateOutlinePen();
            }
        }

        private void UpdateOutlinePen()
        {
            _outlinePen2 = new System.Drawing.Pen(_outlineColor2, _outlineWidth);
            _outlinePen2.Alignment = System.Drawing.Drawing2D.PenAlignment.Outset;

            _outlinePen1 = new System.Drawing.Pen(_outlineColor1, _outlineWidth);
            _outlinePen1.DashStyle = _outlineStyle;
            _outlinePen1.Alignment = System.Drawing.Drawing2D.PenAlignment.Outset;
        }

        internal void EraseInternal()
        {
            AssignInternal(System.Drawing.Rectangle.Empty);
        }

        internal void AssignInternal(System.Drawing.Rectangle rectangle)
        {
            _handlingMode = HandlingMode.None;
            _point1 = System.Drawing.Point.Empty;
            _point2 = System.Drawing.Point.Empty;
            _point3 = System.Drawing.Point.Empty;
            _rectangleOnBitmap = rectangle;
            _ctrlPressed = false;
            _shiftPressed = false;
            _curSelectionRatio = 1.0;
            _selectionRatio = 0.0;

            UpdateGrips(rectangle);

            if (ViewerHasContent)
                _viewer.InvalidateViewer();

            NotifySelected();
        }

        private System.Drawing.Size GetWorkspacePixelSize()
        {
            int width = 0, height = 0;

            Aurigma.GraphicsMill.Unit prevUnit = _viewer.Unit;
            try
            {
                _viewer.Unit = Aurigma.GraphicsMill.Unit.Pixel;
                width = (int)_viewer.WorkspaceWidth;
                height = (int)_viewer.WorkspaceHeight;
            }
            finally
            {
                _viewer.Unit = prevUnit;
            }

            return new System.Drawing.Size(width, height);
        }

        protected override void OnViewerMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (!ViewerHasContent)
                return;

            if (_resizeMode == Aurigma.GraphicsMill.WinControls.ResizeMode.Proportional)
            {
                if (_selectionRatio <= 0.0)
                {
                    if (_handlingMode == HandlingMode.None && _rectangleOnBitmap.Width > 0 && _rectangleOnBitmap.Height > 0)
                        _curSelectionRatio = (double)_rectangleOnBitmap.Width / (double)_rectangleOnBitmap.Height;
                    else if (_handlingMode != HandlingMode.None && Math.Abs(_point1.X - _point2.X) > 0 && Math.Abs(_point1.Y - _point2.Y) > 0)
                        _curSelectionRatio = Math.Abs((double)(_point1.X - _point2.X) / (double)(_point1.Y - _point2.Y));
                    else
                        _curSelectionRatio = 1.0;
                }
                else
                {
                    _curSelectionRatio = _selectionRatio;
                }
            }

            if (_handlingMode == HandlingMode.None && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.Point point = new System.Drawing.Point(e.X, e.Y);

                if ((int)_resizeMode > 0 && _leftTopGrip.Contains(point))
                {
                    _point1 = new System.Drawing.Point(_rectangleOnBitmap.Right, _rectangleOnBitmap.Bottom);
                    _point2 = new System.Drawing.Point(_rectangleOnBitmap.X, _rectangleOnBitmap.Y);
                    _handlingMode = HandlingMode.LeftTopSelectResize;
                }
                else if ((int)_resizeMode > 1 && _topGrip.Contains(point))
                {
                    _point1 = new System.Drawing.Point(_rectangleOnBitmap.Right, _rectangleOnBitmap.Bottom);
                    _point2 = new System.Drawing.Point(_rectangleOnBitmap.X, _rectangleOnBitmap.Y);
                    _handlingMode = HandlingMode.TopSelectResize;
                }
                else if ((int)_resizeMode > 0 && _rightTopGrip.Contains(point))
                {
                    _point1 = new System.Drawing.Point(_rectangleOnBitmap.X, _rectangleOnBitmap.Bottom);
                    _point2 = new System.Drawing.Point(_rectangleOnBitmap.Right, _rectangleOnBitmap.Y);
                    _handlingMode = HandlingMode.RightTopSelectResize;
                }
                else if ((int)_resizeMode > 1 && _rightGrip.Contains(point))
                {
                    _point1 = new System.Drawing.Point(_rectangleOnBitmap.X, _rectangleOnBitmap.Bottom);
                    _point2 = new System.Drawing.Point(_rectangleOnBitmap.Right, _rectangleOnBitmap.Y);
                    _handlingMode = HandlingMode.RightSelectResize;
                }
                else if ((int)_resizeMode > 0 && _rightBottomGrip.Contains(point))
                {
                    _point1 = new System.Drawing.Point(_rectangleOnBitmap.X, _rectangleOnBitmap.Y);
                    _point2 = new System.Drawing.Point(_rectangleOnBitmap.Right, _rectangleOnBitmap.Bottom);
                    _handlingMode = HandlingMode.RightBottomSelectResize;
                }
                else if ((int)_resizeMode > 1 && _bottomGrip.Contains(point))
                {
                    _point1 = new System.Drawing.Point(_rectangleOnBitmap.X, _rectangleOnBitmap.Y);
                    _point2 = new System.Drawing.Point(_rectangleOnBitmap.Right, _rectangleOnBitmap.Bottom);
                    _handlingMode = HandlingMode.BottomSelectResize;
                }
                else if ((int)_resizeMode > 0 && _leftBottomGrip.Contains(point))
                {
                    _point1 = new System.Drawing.Point(_rectangleOnBitmap.Right, _rectangleOnBitmap.Y);
                    _point2 = new System.Drawing.Point(_rectangleOnBitmap.X, _rectangleOnBitmap.Bottom);
                    _handlingMode = HandlingMode.LeftBottomSelectResize;
                }
                else if ((int)_resizeMode > 1 && _leftGrip.Contains(point))
                {
                    _point1 = new System.Drawing.Point(_rectangleOnBitmap.Right, _rectangleOnBitmap.Y);
                    _point2 = new System.Drawing.Point(_rectangleOnBitmap.X, _rectangleOnBitmap.Bottom);
                    _handlingMode = HandlingMode.LeftSelectResize;
                }
                else if (_movable && (_viewer.WorkspaceToControl(_rectangleOnBitmap, Aurigma.GraphicsMill.Unit.Pixel).Contains(point) || _ctrlPressed))
                {
                    _point1 = new System.Drawing.Point(_rectangleOnBitmap.X, _rectangleOnBitmap.Y);
                    _point2 = new System.Drawing.Point(_rectangleOnBitmap.Right, _rectangleOnBitmap.Bottom);
                    _point3 = point;
                    _handlingMode = HandlingMode.DragSelect;
                }
                else if (_erasable)
                {
                    System.Drawing.Point objBitmapPoint = System.Drawing.Point.Truncate(_viewer.ControlToWorkspace(point, Aurigma.GraphicsMill.Unit.Pixel));

                    System.Drawing.Size workspaceSize = GetWorkspacePixelSize();

                    objBitmapPoint.X = FitToBounds(objBitmapPoint.X, 0, workspaceSize.Width);
                    objBitmapPoint.Y = FitToBounds(objBitmapPoint.Y, 0, workspaceSize.Height);

                    _handlingMode = HandlingMode.Select;
                    _point1 = _point2 = objBitmapPoint;
                }
            }
        }

        protected override void OnViewerMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (!ViewerHasContent)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (_erasable && _point1 == _point2 && _handlingMode == HandlingMode.Select)
                {
                    EraseInternal();
                }
                else if (_handlingMode != HandlingMode.None)
                {
                    UpdateSelectionRectangle();
                    NotifySelected();
                }

                _handlingMode = HandlingMode.None;
                _viewer.InvalidateViewer();
            }
        }

        protected override void OnViewerMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            System.Drawing.Size workspaceSize = GetWorkspacePixelSize();

            if (_handlingMode == HandlingMode.Select || _handlingMode == HandlingMode.LeftTopSelectResize || _handlingMode == HandlingMode.RightTopSelectResize || _handlingMode == HandlingMode.RightBottomSelectResize || _handlingMode == HandlingMode.LeftBottomSelectResize)
            {
                System.Drawing.Point point = System.Drawing.Point.Truncate(_viewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Pixel));

                if (_shiftPressed || _resizeMode == Aurigma.GraphicsMill.WinControls.ResizeMode.Proportional)
                {
                    int deltaX = point.X - _point1.X;
                    int deltaY = point.Y - _point1.Y;
                    int deltaXSign = Math.Sign(deltaX);
                    int deltaYSign = Math.Sign(deltaY);

                    deltaX = deltaX < 0 ? -FitToBounds(-deltaX, 0, _point1.X) : FitToBounds(deltaX, 0, workspaceSize.Width - _point1.X);
                    deltaY = (int)((double)Math.Abs(deltaX) / _curSelectionRatio) * deltaYSign;

                    if (deltaYSign < 0 && Math.Abs(deltaY) > _point1.Y)
                    {
                        deltaY = -FitToBounds(-deltaY, 0, _point1.Y);
                        deltaX = (int)((double)Math.Abs(deltaY) * _curSelectionRatio) * deltaXSign;
                    }
                    else if (deltaYSign > 0 && Math.Abs(deltaY) > workspaceSize.Height - _point1.Y)
                    {
                        deltaY = FitToBounds(deltaY, 0, workspaceSize.Height - _point1.Y);
                        deltaX = (int)((double)Math.Abs(deltaY) * _curSelectionRatio) * deltaXSign;
                    }

                    _point2.X = _point1.X + deltaX;
                    _point2.Y = _point1.Y + deltaY;
                }
                else
                {
                    point.X = FitToBounds(point.X, 0, workspaceSize.Width);
                    point.Y = FitToBounds(point.Y, 0, workspaceSize.Height);
                    _point2 = point;
                }
            }
            else if (_handlingMode == HandlingMode.TopSelectResize)
            {
                System.Drawing.Point point = System.Drawing.Point.Truncate(_viewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Pixel));
                point.Y = FitToBounds(point.Y, 0, workspaceSize.Height);

                _point2.Y = point.Y;
            }
            else if (_handlingMode == HandlingMode.RightSelectResize)
            {
                System.Drawing.Point point = System.Drawing.Point.Truncate(_viewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Pixel));
                point.X = FitToBounds(point.X, 0, workspaceSize.Width);

                _point2.X = point.X;
            }
            else if (_handlingMode == HandlingMode.BottomSelectResize)
            {
                System.Drawing.Point point = System.Drawing.Point.Truncate(_viewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Pixel));
                point.Y = FitToBounds(point.Y, 0, workspaceSize.Height);

                _point2.Y = point.Y;
            }
            else if (_handlingMode == HandlingMode.LeftSelectResize)
            {
                System.Drawing.Point point = System.Drawing.Point.Truncate(_viewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Pixel));
                point.X = FitToBounds(point.X, 0, workspaceSize.Width);

                _point2.X = point.X;
            }
            else if (_handlingMode == HandlingMode.DragSelect)
            {
                System.Drawing.Point point1 = System.Drawing.Point.Truncate(_viewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Pixel));
                System.Drawing.Point point2 = System.Drawing.Point.Truncate(_viewer.ControlToWorkspace(_point3, Aurigma.GraphicsMill.Unit.Pixel));

                System.Drawing.Point selectionPoint1 = _point1;
                System.Drawing.Point selectionPoint2 = _point2;
                NormalizeRectanglePoints(ref selectionPoint1, ref selectionPoint2);

                int xOffset = point1.X - point2.X;
                int yOffset = point1.Y - point2.Y;
                xOffset = Math.Max(Math.Min(xOffset, workspaceSize.Width - selectionPoint2.X), -selectionPoint1.X);
                yOffset = Math.Max(Math.Min(yOffset, workspaceSize.Height - selectionPoint2.Y), -selectionPoint1.Y);

                _point1.Offset(xOffset, yOffset);
                _point2.Offset(xOffset, yOffset);
                _point3 = new System.Drawing.Point(e.X, e.Y);
            }

            if ((int)_handlingMode > 1)
            {
                System.Drawing.Rectangle canvasRenderingRect = _viewer.GetViewportBounds();
                int scrollX = 0, scrollY = 0;

                if (e.X < canvasRenderingRect.X)
                    scrollX = e.X - canvasRenderingRect.X;
                else if (e.X > canvasRenderingRect.Right)
                    scrollX = e.X - canvasRenderingRect.Right;

                if (e.Y < canvasRenderingRect.Y)
                    scrollY = e.Y - canvasRenderingRect.Y;
                else if (e.Y > canvasRenderingRect.Bottom)
                    scrollY = e.Y - canvasRenderingRect.Bottom;

                _viewer.Scroll(scrollX, scrollY);
            }

            if (_handlingMode != HandlingMode.None)
            {
                NotifySelecting(_handlingMode == HandlingMode.DragSelect ? Aurigma.GraphicsMill.WinControls.RectangleChangeMode.Moving : Aurigma.GraphicsMill.WinControls.RectangleChangeMode.Resizing);
                _viewer.InvalidateViewer();
            }

            UpdateCursor(false, new System.Drawing.Point(e.X, e.Y));
        }

        protected override void OnViewerDoubleClick(System.EventArgs e)
        {
            if (!ViewerHasContent)
                return;

            if (_erasable)
                EraseInternal();
        }

        protected override void OnViewerDoubleBufferPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            DrawSelection(e.Graphics);
        }

        protected override void OnViewerKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            _ctrlPressed = e.Control;
            _shiftPressed = e.Shift;

            if (_shiftPressed && _curSelectionRatio == 0.0)
            {
                if (_selectionRatio <= 0.0)
                {
                    if (_handlingMode == HandlingMode.None && _rectangleOnBitmap.Width > 0 && _rectangleOnBitmap.Height > 0)
                        _curSelectionRatio = (double)_rectangleOnBitmap.Width / (double)_rectangleOnBitmap.Height;
                    else if (_handlingMode != HandlingMode.None && Math.Abs(_point1.X - _point2.X) > 0 && Math.Abs(_point1.Y - _point2.Y) > 0)
                        _curSelectionRatio = Math.Abs((double)(_point1.X - _point2.X) / (double)(_point1.Y - _point2.Y));
                    else
                        _curSelectionRatio = 1.0;
                }
                else
                {
                    _curSelectionRatio = _selectionRatio;
                }
            }

            if (_ctrlPressed)
                UpdateCursor(false, System.Drawing.Point.Empty);
        }

        protected override void OnViewerKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            _ctrlPressed = false;
            _shiftPressed = false;
            UpdateCursor(false, System.Drawing.Point.Empty);

            if (_resizeMode != Aurigma.GraphicsMill.WinControls.ResizeMode.Proportional)
            {
                _curSelectionRatio = 0.0;

                System.Drawing.Point point = _viewer.PointToClient(System.Windows.Forms.Cursor.Position);
                OnViewerMouseMove(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, point.X, point.Y, 0));
            }
        }

        internal virtual void DrawSelection(System.Drawing.Graphics graphics)
        {
            _selectionDrawingRectangle = System.Drawing.Rectangle.Empty;

            System.Drawing.Size workspaceSize = GetWorkspacePixelSize();

            if (this.ViewerHasContent)
                _rectangleOnBitmap.Intersect(new System.Drawing.Rectangle(0, 0, workspaceSize.Width, workspaceSize.Height));

            if (!_rectangleOnBitmap.IsEmpty)
            {
                _selectionDrawingRectangle = _viewer.WorkspaceToControl(_rectangleOnBitmap, Aurigma.GraphicsMill.Unit.Pixel);
                _selectionDrawingRectangle.Intersect(_viewer.GetViewportBounds());

                if (_selectionDrawingRectangle.Width >= _outlineWidth)
                    _selectionDrawingRectangle.Width -= _outlineWidth;

                if (_selectionDrawingRectangle.Height >= _outlineWidth)
                    _selectionDrawingRectangle.Height -= _outlineWidth;

                BeforeDrawingSelection(graphics);

                if (_maskStyle == Aurigma.GraphicsMill.WinControls.MaskStyle.Always || (_maskStyle == Aurigma.GraphicsMill.WinControls.MaskStyle.HideOnChange && _handlingMode == HandlingMode.None))
                {
                    System.Drawing.SolidBrush maskBrush = new System.Drawing.SolidBrush(_maskColor);
                    ExcludeMaskRegion(graphics);
                    graphics.FillRectangle(maskBrush, _viewer.GetViewportBounds());
                    graphics.ResetClip();
                    maskBrush.Dispose();
                }

                AfterDrawingSelection(graphics);

                if (_selectionDrawingRectangle.Width > 0 && _selectionDrawingRectangle.Height > 0)
                {
                    graphics.DrawRectangle(_outlinePen2, _selectionDrawingRectangle);
                    graphics.DrawRectangle(_outlinePen1, _selectionDrawingRectangle);
                }
                else
                {
                    graphics.DrawLine(_outlinePen2, _selectionDrawingRectangle.Left, _selectionDrawingRectangle.Top, _selectionDrawingRectangle.Right, _selectionDrawingRectangle.Bottom);
                    graphics.DrawLine(_outlinePen1, _selectionDrawingRectangle.Left, _selectionDrawingRectangle.Top, _selectionDrawingRectangle.Right, _selectionDrawingRectangle.Bottom);
                }

                if (_handlingMode == HandlingMode.None)
                {
                    UpdateGrips(_viewer.WorkspaceToControl(_rectangleOnBitmap, Aurigma.GraphicsMill.Unit.Pixel));

                    if (_gripsVisible)
                    {
                        if ((int)_resizeMode > 0)
                        {
                            DrawGrip(graphics, _leftTopGrip);
                            DrawGrip(graphics, _rightTopGrip);
                            DrawGrip(graphics, _rightBottomGrip);
                            DrawGrip(graphics, _leftBottomGrip);
                        }
                        if ((int)_resizeMode > 1)
                        {
                            DrawGrip(graphics, _topGrip);
                            DrawGrip(graphics, _rightGrip);
                            DrawGrip(graphics, _bottomGrip);
                            DrawGrip(graphics, _leftGrip);
                        }
                    }
                }
            }
        }

        private void UpdateGrips(System.Drawing.Rectangle selectionRectangle)
        {
            if (selectionRectangle.Width > 0 || selectionRectangle.Height > 0)
            {
                int secondaryGripSize = _gripSize * 3 / 4;

                if (_gripsVisible)
                {
                    _leftTopGrip = new System.Drawing.Rectangle(selectionRectangle.X - _gripSize / 2, selectionRectangle.Y - _gripSize / 2, _gripSize, _gripSize);
                    _topGrip = new System.Drawing.Rectangle(selectionRectangle.X + selectionRectangle.Width / 2 - secondaryGripSize / 2, selectionRectangle.Y - secondaryGripSize / 2, secondaryGripSize, secondaryGripSize);
                    _rightTopGrip = new System.Drawing.Rectangle(selectionRectangle.Right - _gripSize / 2, selectionRectangle.Y - _gripSize / 2, _gripSize, _gripSize);
                    _rightGrip = new System.Drawing.Rectangle(selectionRectangle.Right - secondaryGripSize / 2, selectionRectangle.Y + selectionRectangle.Height / 2 - secondaryGripSize / 2, secondaryGripSize, secondaryGripSize);
                    _rightBottomGrip = new System.Drawing.Rectangle(selectionRectangle.Right - _gripSize / 2, selectionRectangle.Bottom - _gripSize / 2, _gripSize, _gripSize);
                    _bottomGrip = new System.Drawing.Rectangle(selectionRectangle.X + selectionRectangle.Width / 2 - secondaryGripSize / 2, selectionRectangle.Bottom - secondaryGripSize / 2, secondaryGripSize, secondaryGripSize);
                    _leftBottomGrip = new System.Drawing.Rectangle(selectionRectangle.X - _gripSize / 2, selectionRectangle.Bottom - _gripSize / 2, _gripSize, _gripSize);
                    _leftGrip = new System.Drawing.Rectangle(selectionRectangle.X - secondaryGripSize / 2, selectionRectangle.Y + selectionRectangle.Height / 2 - secondaryGripSize / 2, secondaryGripSize, secondaryGripSize);
                }
                else
                {
                    _leftTopGrip = new System.Drawing.Rectangle(selectionRectangle.X - _gripSize / 2, selectionRectangle.Y - _gripSize / 2, _gripSize, _gripSize);
                    _topGrip = new System.Drawing.Rectangle(selectionRectangle.X + _gripSize / 2, selectionRectangle.Y - _gripSize / 2, selectionRectangle.Width - _gripSize, _gripSize);
                    _rightTopGrip = new System.Drawing.Rectangle(selectionRectangle.Right - _gripSize / 2, selectionRectangle.Y - _gripSize / 2, _gripSize, _gripSize);
                    _rightGrip = new System.Drawing.Rectangle(selectionRectangle.Right - _gripSize / 2, selectionRectangle.Y + _gripSize / 2, _gripSize, selectionRectangle.Height - _gripSize);
                    _rightBottomGrip = new System.Drawing.Rectangle(selectionRectangle.Right - _gripSize / 2, selectionRectangle.Bottom - _gripSize / 2, _gripSize, _gripSize);
                    _bottomGrip = new System.Drawing.Rectangle(selectionRectangle.X + _gripSize / 2, selectionRectangle.Bottom - _gripSize / 2, selectionRectangle.Width - _gripSize, _gripSize);
                    _leftBottomGrip = new System.Drawing.Rectangle(selectionRectangle.X - _gripSize / 2, selectionRectangle.Bottom - _gripSize / 2, _gripSize, _gripSize);
                    _leftGrip = new System.Drawing.Rectangle(selectionRectangle.X - _gripSize / 2, selectionRectangle.Y + _gripSize / 2, _gripSize, selectionRectangle.Height - _gripSize);
                }
            }
            else
            {
                _leftTopGrip = System.Drawing.Rectangle.Empty;
                _topGrip = System.Drawing.Rectangle.Empty;
                _rightTopGrip = System.Drawing.Rectangle.Empty;
                _rightGrip = System.Drawing.Rectangle.Empty;
                _rightBottomGrip = System.Drawing.Rectangle.Empty;
                _bottomGrip = System.Drawing.Rectangle.Empty;
                _leftBottomGrip = System.Drawing.Rectangle.Empty;
                _leftGrip = System.Drawing.Rectangle.Empty;
            }
        }

        private void DrawGrip(System.Drawing.Graphics graphics, System.Drawing.Rectangle rectangle)
        {
            System.Drawing.Pen gripPen1 = new System.Drawing.Pen(_gripColor, 1);
            System.Drawing.Pen gripPen2 = new System.Drawing.Pen(System.Drawing.Color.FromArgb(_gripColor.A, FitToBounds(_gripColor.R + 100, 0, 255), FitToBounds(_gripColor.G + 100, 0, 255), FitToBounds(_gripColor.B + 100, 0, 255)), 1);
            System.Drawing.Brush gripBrush1 = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(_gripColor.A, FitToBounds(_gripColor.R + 150, 0, 255), FitToBounds(_gripColor.G + 150, 0, 255), FitToBounds(_gripColor.B + 150, 0, 255)));
            System.Drawing.Brush gripBrush2 = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(_gripColor.A, FitToBounds(_gripColor.R + 50, 0, 255), FitToBounds(_gripColor.G + 50, 0, 255), FitToBounds(_gripColor.B + 50, 0, 255)));
            System.Drawing.Drawing2D.LinearGradientBrush gripBrush3 = new System.Drawing.Drawing2D.LinearGradientBrush(rectangle, System.Drawing.Color.White, _gripColor, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);

            graphics.FillRectangle(gripBrush3, rectangle);
            graphics.DrawLine(gripPen1, rectangle.Right, rectangle.Top, rectangle.Right, rectangle.Bottom);
            graphics.DrawLine(gripPen1, rectangle.Right, rectangle.Bottom, rectangle.Left, rectangle.Bottom);
            graphics.DrawLine(gripPen2, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Top);
            graphics.DrawLine(gripPen2, rectangle.Left, rectangle.Top, rectangle.Left, rectangle.Bottom);
            graphics.FillRectangle(gripBrush1, rectangle.Left, rectangle.Top, 1, 1);
            graphics.FillRectangle(gripBrush1, rectangle.Left, rectangle.Bottom, 1, 1);
            graphics.FillRectangle(gripBrush1, rectangle.Right, rectangle.Top, 1, 1);
            graphics.FillRectangle(gripBrush2, rectangle.Right, rectangle.Bottom, 1, 1);

            gripPen1.Dispose();
            gripPen2.Dispose();
            gripBrush1.Dispose();
            gripBrush2.Dispose();
            gripBrush3.Dispose();
        }

        internal static void NormalizeRectanglePoints(ref System.Drawing.Point point1, ref System.Drawing.Point point2)
        {
            System.Drawing.Point tmpPoint1 = point1;
            System.Drawing.Point tmpPoint2 = point2;

            point1.X = Math.Min(tmpPoint1.X, tmpPoint2.X);
            point2.X = Math.Max(tmpPoint1.X, tmpPoint2.X);
            point1.Y = Math.Min(tmpPoint1.Y, tmpPoint2.Y);
            point2.Y = Math.Max(tmpPoint1.Y, tmpPoint2.Y);
        }

        internal static System.Drawing.Rectangle GetSelectionRectangle(System.Drawing.Point point1, System.Drawing.Point point2)
        {
            System.Drawing.Rectangle rect;

            if (point1 == point2)
            {
                rect = new System.Drawing.Rectangle(point1.X, point1.Y, 0, 0);
            }
            else
            {
                System.Drawing.Point pt1 = point1;
                System.Drawing.Point pt2 = point2;
                NormalizeRectanglePoints(ref pt1, ref pt2);

                rect = new System.Drawing.Rectangle(pt1.X, pt1.Y, pt2.X - pt1.X, pt2.Y - pt1.Y);
            }

            return rect;
        }

        internal void UpdateSelectionRectangle()
        {
            _rectangleOnBitmap = GetSelectionRectangle(_point1, _point2);
        }

        internal void UpdateSelectionPoints(System.Drawing.Rectangle rect)
        {
            if (_point1.X < _point2.X)
            {
                _point1.X = rect.Left;
                _point2.X = rect.Right;
            }
            else
            {
                _point2.X = rect.Left;
                _point1.X = rect.Right;
            }

            if (_point1.Y < _point2.Y)
            {
                _point1.Y = rect.Top;
                _point2.Y = rect.Bottom;
            }
            else
            {
                _point2.Y = rect.Top;
                _point1.Y = rect.Bottom;
            }
        }

        protected override void UpdateCursor(bool isCursorDefault, System.Drawing.Point point)
        {
            if (this.IsUserInputEnabled)
            {
                if ((int)_resizeMode > 0 && (_leftTopGrip.Contains(point) || _rightBottomGrip.Contains(point) || _handlingMode == HandlingMode.LeftTopSelectResize || _handlingMode == HandlingMode.RightBottomSelectResize))
                    _viewer.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
                else if ((int)_resizeMode > 1 && (_topGrip.Contains(point) || _bottomGrip.Contains(point) || _handlingMode == HandlingMode.TopSelectResize || _handlingMode == HandlingMode.BottomSelectResize))
                    _viewer.Cursor = System.Windows.Forms.Cursors.SizeNS;
                else if ((int)_resizeMode > 0 && (_rightTopGrip.Contains(point) || _leftBottomGrip.Contains(point) || _handlingMode == HandlingMode.RightTopSelectResize || _handlingMode == HandlingMode.LeftBottomSelectResize))
                    _viewer.Cursor = System.Windows.Forms.Cursors.SizeNESW;
                else if ((int)_resizeMode > 1 && (_rightGrip.Contains(point) || _leftGrip.Contains(point) || _handlingMode == HandlingMode.RightSelectResize || _handlingMode == HandlingMode.LeftSelectResize))
                    _viewer.Cursor = System.Windows.Forms.Cursors.SizeWE;
                else if (_movable && (_viewer.WorkspaceToControl(_rectangleOnBitmap, Aurigma.GraphicsMill.Unit.Pixel).Contains(point) || _handlingMode == HandlingMode.DragSelect || _ctrlPressed))
                    _viewer.Cursor = System.Windows.Forms.Cursors.SizeAll;
                else if (_viewer.GetViewportBounds().Contains(point) && _erasable)
                    _viewer.Cursor = _cursor;
                else
                    _viewer.RestoreCursorToDefault();
            }
        }

        public override void Update()
        {
            base.Update();

            _handlingMode = HandlingMode.None;
            _curSelectionRatio = 0.0;

            if (_viewer != null)
                _viewer.InvalidateViewer();
        }

        internal virtual void NotifySelecting(RectangleChangeMode mode)
        {
            UpdateSelectionRectangle();
        }

        internal virtual void NotifySelected()
        {
        }

        internal virtual void ExcludeMaskRegion(System.Drawing.Graphics graphics)
        {
            graphics.ExcludeClip(_selectionDrawingRectangle);
        }

        internal virtual void BeforeDrawingSelection(System.Drawing.Graphics graphics)
        {
        }

        internal virtual void AfterDrawingSelection(System.Drawing.Graphics graphics)
        {
        }
    }

    [ResDescription("RectangleRubberband")]
    [AdaptiveToolboxBitmapAttribute(typeof(Aurigma.GraphicsMill.WinControls.RectangleRubberband), "RectangleRubberband.bmp")]
    public class RectangleRubberband : RectangleController, IRubberband
    {
        [System.ComponentModel.Browsable(true)]
        [ResDescription("RectangleRubberband_RectangleChanging")]
        public event RectangleEventHandler RectangleChanging;

        [System.ComponentModel.Browsable(true)]
        [ResDescription("RectangleRubberband_RectangleChanged")]
        public event RectangleEventHandler RectangleChanged;

        public RectangleRubberband()
        {
            _movable = true;
            _erasable = true;
            _resizeMode = Aurigma.GraphicsMill.WinControls.ResizeMode.Arbitrary;
            _cursor = new System.Windows.Forms.Cursor(typeof(RectangleRubberband), "BitmapViewer.Select.cur");
        }

        [System.ComponentModel.Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return !(_rectangleOnBitmap.Width > 0 && _rectangleOnBitmap.Height > 0);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public System.Drawing.Rectangle Rectangle
        {
            get
            {
                return _rectangleOnBitmap;
            }

            set
            {
                AssignInternal(value);
                Update();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(0.0)]
        [ResDescription("RectangleRubberband_Ratio")]
        public double Ratio
        {
            get
            {
                return _selectionRatio;
            }

            set
            {
                _selectionRatio = Math.Abs(value);
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.ResizeMode), "Arbitrary")]
        [ResDescription("RectangleRubberband_ResizeMode")]
        public Aurigma.GraphicsMill.WinControls.ResizeMode ResizeMode
        {
            get
            {
                return _resizeMode;
            }

            set
            {
                _resizeMode = value;
                Update();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("RectangleRubberband_Movable")]
        public bool Movable
        {
            get
            {
                return _movable;
            }

            set
            {
                _movable = value;
                Update();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("RectangleRubberband_Erasable")]
        public bool Erasable
        {
            get
            {
                return _erasable;
            }

            set
            {
                _erasable = value;
                Update();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("RectangleRubberband_GripsVisible")]
        public bool GripsVisible
        {
            get
            {
                return _gripsVisible;
            }

            set
            {
                _gripsVisible = value;
                Update();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(8)]
        [ResDescription("RectangleRubberband_GripSize")]
        public int GripSize
        {
            get
            {
                return _gripSize;
            }

            set
            {
                _gripSize = Math.Abs(value);
                Update();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(System.Drawing.Color), "0; 153; 0")]
        [ResDescription("RectangleRubberband_GripColor")]
        public System.Drawing.Color GripColor
        {
            get
            {
                return _gripColor;
            }

            set
            {
                _gripColor = value;
                Update();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.MaskStyle), "Always")]
        [ResDescription("RectangleRubberband_MaskStyle")]
        public Aurigma.GraphicsMill.WinControls.MaskStyle MaskStyle
        {
            get
            {
                return _maskStyle;
            }

            set
            {
                _maskStyle = value;
                Update();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(System.Drawing.Color), "127; 255; 51; 0")]
        [ResDescription("RectangleRubberband_MaskColor")]
        public System.Drawing.Color MaskColor
        {
            get
            {
                return _maskColor;
            }

            set
            {
                _maskColor = value;
                Update();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [ResDescription("RectangleRubberband_MaskOpacity")]
        public int MaskOpacity
        {
            get
            {
                return _maskColor.A;
            }

            set
            {
                _maskColor = System.Drawing.Color.FromArgb(value, _maskColor.R, _maskColor.G, _maskColor.B);
                Update();
            }
        }

        public void Erase()
        {
            EraseInternal();
        }

        internal override void NotifySelecting(RectangleChangeMode mode)
        {
            if (this.RectangleChanging != null)
            {
                System.Drawing.Rectangle rect = GetSelectionRectangle(_point1, _point2);
                RectangleEventArgs args = new RectangleEventArgs(rect, mode);

                RectangleChanging(this, args);

                UpdateSelectionPoints(args.Rectangle);
            }

            base.NotifySelecting(mode);
        }

        internal override void NotifySelected()
        {
            if (this.RectangleChanged != null)
                RectangleChanged(this, new RectangleEventArgs(_rectangleOnBitmap, Aurigma.GraphicsMill.WinControls.RectangleChangeMode.Changed));
        }
    }

    [ResDescription("EllipseRubberband")]
    [AdaptiveToolboxBitmapAttribute(typeof(Aurigma.GraphicsMill.WinControls.EllipseRubberband), "EllipseRubberband.bmp")]
    public sealed class EllipseRubberband : RectangleRubberband
    {
        internal override void AfterDrawingSelection(System.Drawing.Graphics graphics)
        {
            base.AfterDrawingSelection(graphics);

            System.Drawing.Drawing2D.SmoothingMode mode = graphics.SmoothingMode;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.Drawing.Rectangle ellipseRectangle = _viewer.WorkspaceToControl(_rectangleOnBitmap, Aurigma.GraphicsMill.Unit.Pixel);
            graphics.DrawEllipse(_outlinePen2, ellipseRectangle);
            graphics.DrawEllipse(_outlinePen1, ellipseRectangle);

            graphics.SmoothingMode = mode;
        }

        internal override void ExcludeMaskRegion(System.Drawing.Graphics graphics)
        {
            System.Drawing.Rectangle ellipseRectangle = _viewer.WorkspaceToControl(_rectangleOnBitmap, Aurigma.GraphicsMill.Unit.Pixel);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddEllipse(ellipseRectangle);
            System.Drawing.Region region = new System.Drawing.Region(path);

            graphics.ExcludeClip(region);

            path.Dispose();
            region.Dispose();
        }
    }

    [ResDescription("ZoomRectangleNavigator")]
    [AdaptiveToolboxBitmapAttribute(typeof(Aurigma.GraphicsMill.WinControls.ZoomRectangleNavigator), "ZoomRectangleNavigator.bmp")]
    public sealed class ZoomRectangleNavigator : RectangleController, INavigator
    {
        public ZoomRectangleNavigator()
        {
            _resizeMode = Aurigma.GraphicsMill.WinControls.ResizeMode.None;
            _movable = false;
            _erasable = true;
            _maskStyle = Aurigma.GraphicsMill.WinControls.MaskStyle.None;

            _cursor = new System.Windows.Forms.Cursor(this.GetType(), "BitmapViewer.SelectZoom.cur");
        }

        protected override void OnViewerMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            base.OnViewerMouseUp(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left && _rectangleOnBitmap.Width > 0 && _rectangleOnBitmap.Height > 0)
            {
                _viewer.ZoomMode = ZoomMode.None;
                System.Drawing.Rectangle rectangle = _viewer.GetCanvasBounds();

                System.Drawing.RectangleF controlRect = _viewer.ControlToWorkspace(rectangle, Aurigma.GraphicsMill.Unit.Pixel);
                System.Drawing.RectangleF frameRect = _viewer.WorkspaceToControl(_rectangleOnBitmap, Aurigma.GraphicsMill.Unit.Pixel);
                float horizontalScale = ((float)rectangle.Width / _viewer.Zoom) / controlRect.Width;
                float verticalScale = ((float)rectangle.Height / _viewer.Zoom) / controlRect.Height;

                float zoomWidth = controlRect.Width / (float)_rectangleOnBitmap.Width;
                float zoomHeight = controlRect.Height / (float)_rectangleOnBitmap.Height;
                float zoom, offsetWidth = 0, offsetHeight = 0;
                zoom = zoomWidth < zoomHeight ? zoomWidth : zoomHeight;
                System.Drawing.Point position = _viewer.ScrollingPosition;
                _viewer.Zoom *= zoom;
                offsetHeight = (((float)rectangle.Height / _viewer.Zoom) / verticalScale - (float)_rectangleOnBitmap.Height) / 2f;
                offsetWidth = (((float)rectangle.Width / _viewer.Zoom) / horizontalScale - (float)_rectangleOnBitmap.Width) / 2f;

                System.Drawing.RectangleF scrollPosRect = _viewer.WorkspaceToControl(new System.Drawing.RectangleF(0, 0, _rectangleOnBitmap.X - offsetWidth, _rectangleOnBitmap.Y - offsetHeight), Aurigma.GraphicsMill.Unit.Pixel);
                position.X = (int)scrollPosRect.Width;
                position.Y = (int)scrollPosRect.Height;

                _viewer.ScrollingPosition = position;
                EraseInternal();
            }
            else
            {
                EraseInternal();
            }
        }

        protected override void OnViewerMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!ViewerHasContent)
                return;

            base.OnViewerMouseDown(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                System.Drawing.Rectangle rectangle = _viewer.GetViewportBounds();
                System.Drawing.Point objCenter = new System.Drawing.Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
                System.Drawing.Point position = _viewer.ScrollingPosition;

                position.X += e.X - objCenter.X;
                position.Y += e.Y - objCenter.Y;

                _viewer.ScrollingPosition = position;
                _viewer.Zoom /= _viewer.WheelZoomAmount;
            }
        }
    }
}