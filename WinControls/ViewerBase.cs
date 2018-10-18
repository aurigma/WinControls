// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;

namespace Aurigma.GraphicsMill.WinControls
{
    #region "Mouse event args & handler"

    public class MouseEventArgs : System.EventArgs
    {
        public MouseEventArgs(System.Windows.Forms.MouseButtons button, int clicks, float x, float y)
        {
            _button = button;
            _clicks = clicks;
            _x = x;
            _y = y;
        }

        public MouseEventArgs(System.Windows.Forms.MouseButtons button, int clicks, System.Drawing.PointF location)
            : this(button, clicks, location.X, location.Y)
        {
        }

        public System.Windows.Forms.MouseButtons Button
        {
            get
            {
                return _button;
            }
        }

        public int Clicks
        {
            get
            {
                return _clicks;
            }
        }

        public System.Drawing.PointF Location
        {
            get
            {
                return new System.Drawing.PointF(_x, _y);
            }
        }

        public float X
        {
            get
            {
                return _x;
            }
        }

        public float Y
        {
            get
            {
                return _y;
            }
        }

        #region "Member variables"

        private System.Windows.Forms.MouseButtons _button;
        private int _clicks;
        private float _x;
        private float _y;

        #endregion "Member variables"
    }

    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

    #endregion "Mouse event args & handler"

    public enum ScrollValue
    {
        PageBack,
        PageForward,
        StepBack,
        StepForward,
        Begin,
        End
    }

    public abstract class InvalidationTarget
    {
        protected InvalidationTarget(System.Drawing.Rectangle rectangle)
        {
            _rectangle = rectangle;
        }

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

        private System.Drawing.Rectangle _rectangle;
    }

    public abstract class ViewerBase : System.Windows.Forms.Control, ICoordinateMapper
    {
        #region "Constants"

        private static float eps = 0.0001f;

        #endregion "Constants"

        #region "Construction / destruction"

        protected ViewerBase()
        {
            _minZoom = 0.1f;
            _maxZoom = 16.0f;
            _zoom = 1.0f;
            _zoomMode = Aurigma.GraphicsMill.WinControls.ZoomMode.None;
            _wheelZoomAmount = 1.5f;

            _borderStyle = System.Windows.Forms.Border3DStyle.Sunken;

            ReadMonitorResolution();
        }

        private void ReadMonitorResolution()
        {
            System.IntPtr hDC = NativeMethods.GetDC(System.IntPtr.Zero);
            try
            {
                _viewerResolution = NativeMethods.GetDeviceCaps(hDC, NativeMethods.LOGPIXELSX);
            }
            finally
            {
                NativeMethods.ReleaseDC(System.IntPtr.Zero, hDC);
            }
        }

        #endregion "Construction / destruction"

        #region "Class events"

        public event System.EventHandler Zoomed;

        public event System.EventHandler Scrolled;

        public event System.EventHandler WorkspaceChanged;

        public event Aurigma.GraphicsMill.WinControls.MouseEventHandler WorkspaceMouseDown;

        public event Aurigma.GraphicsMill.WinControls.MouseEventHandler WorkspaceMouseUp;

        public event Aurigma.GraphicsMill.WinControls.MouseEventHandler WorkspaceMouseMove;

        public event System.Windows.Forms.PaintEventHandler DoubleBufferPaint;

        protected void OnZoomed(System.EventArgs e)
        {
            if (Zoomed != null)
                Zoomed(this, e);
        }

        protected void OnScrolled(System.EventArgs e)
        {
            if (Scrolled != null)
                Scrolled(this, e);
        }

        protected void OnWorkspaceChanged(System.EventArgs e)
        {
            if (WorkspaceChanged != null)
                WorkspaceChanged(this, e);
        }

        protected void OnWorkspaceMouseDown(Aurigma.GraphicsMill.WinControls.MouseEventArgs e)
        {
            if (WorkspaceMouseDown != null)
                WorkspaceMouseDown(this, e);
        }

        protected void OnWorkspaceMouseUp(Aurigma.GraphicsMill.WinControls.MouseEventArgs e)
        {
            if (WorkspaceMouseUp != null)
                WorkspaceMouseUp(this, e);
        }

        protected void OnWorkspaceMouseMove(Aurigma.GraphicsMill.WinControls.MouseEventArgs e)
        {
            if (WorkspaceMouseMove != null)
                WorkspaceMouseMove(this, e);
        }

        protected void OnDoubleBufferPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (DoubleBufferPaint != null)
                DoubleBufferPaint(this, e);
        }

        #endregion "Class events"

        public abstract bool HasContent
        {
            get;
        }

        public abstract float WorkspaceWidth
        {
            get;
            set;
        }

        public abstract float WorkspaceHeight
        {
            get;
            set;
        }

        public abstract Aurigma.GraphicsMill.Unit Unit
        {
            get;
            set;
        }

        public abstract System.Drawing.Point ScrollingPosition
        {
            get;
            set;
        }

        public abstract Aurigma.GraphicsMill.WinControls.ScrollBarsStyle ScrollBarsStyle
        {
            get;
            set;
        }

        public abstract Aurigma.GraphicsMill.WinControls.ViewportAlignment ViewportAlignment
        {
            get;
            set;
        }

        public abstract bool WorkspaceBorderEnabled
        {
            get;
            set;
        }

        public abstract System.Drawing.Color WorkspaceBorderColor
        {
            get;
            set;
        }

        public abstract int WorkspaceBorderWidth
        {
            get;
            set;
        }

        public abstract void Scroll(int horizontalDelta, int verticalDelta);

        public abstract void Scroll(bool scrollVertically, ScrollValue scrollValue);

        public abstract void InvalidateViewer(System.Drawing.Rectangle rectangle);

        public abstract void InvalidateViewer(InvalidationTarget target);

        public abstract void InvalidateViewer();

        public abstract System.Drawing.Rectangle GetCanvasBounds();

        public abstract System.Drawing.Rectangle GetViewportBounds();

        protected virtual void UpdateCanvas()
        {
            // Subclasses might want to do something here.
        }

        #region "[Public] Other properties"

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(null)]
        [ResDescription("ViewerBase_Navigator")]
        public virtual Aurigma.GraphicsMill.WinControls.INavigator Navigator
        {
            set
            {
                if (_navigator != null)
                    _navigator.Disconnect();

                _navigator = value;

                if (_navigator != null)
                    _navigator.Connect(this);

                if (_rubberband != null)
                    _rubberband.Update();
            }

            get
            {
                return (INavigator)_navigator;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(null)]
        [ResDescription("ViewerBase_Rubberband")]
        public virtual Aurigma.GraphicsMill.WinControls.IRubberband Rubberband
        {
            set
            {
                if (_rubberband != null)
                    _rubberband.Disconnect();

                _rubberband = value;

                if (_rubberband != null)
                    _rubberband.Connect(this);

                if (_navigator != null)
                    _navigator.Update();
            }
            get
            {
                return (IRubberband)_rubberband;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(System.Windows.Forms.Border3DStyle), "Sunken")]
        [ResDescription("ViewerBase_BorderStyle")]
        public System.Windows.Forms.Border3DStyle BorderStyle
        {
            get
            {
                return _borderStyle;
            }

            set
            {
                _borderStyle = value;
                InvalidateViewer();
                UpdateCanvas();
            }
        }

        [System.ComponentModel.Browsable(false)]
        public float ViewerResolution
        {
            get
            {
                return _viewerResolution;
            }
        }

        #endregion "[Public] Other properties"

        #region "[Public] ICoordinateMapper interface methods"

        public abstract System.Drawing.Point WorkspaceToControl(System.Drawing.PointF workspacePoint, Aurigma.GraphicsMill.Unit workspaceUnit);

        public abstract System.Drawing.PointF ControlToWorkspace(System.Drawing.Point controlPoint, Aurigma.GraphicsMill.Unit workspaceUnit);

        public virtual System.Drawing.Rectangle WorkspaceToControl(System.Drawing.RectangleF workspaceRectangle, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            System.Drawing.Point lt, rb;
            lt = WorkspaceToControl(workspaceRectangle.Location, workspaceUnit);
            rb = WorkspaceToControl(new System.Drawing.PointF(workspaceRectangle.Right, workspaceRectangle.Bottom), workspaceUnit);

            return System.Drawing.Rectangle.FromLTRB(lt.X, lt.Y, rb.X, rb.Y);
        }

        public virtual System.Drawing.RectangleF ControlToWorkspace(System.Drawing.Rectangle controlRectangle, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            System.Drawing.PointF lt, rb;
            lt = ControlToWorkspace(controlRectangle.Location, workspaceUnit);
            rb = ControlToWorkspace(new System.Drawing.Point(controlRectangle.Right, controlRectangle.Bottom), workspaceUnit);

            return System.Drawing.RectangleF.FromLTRB(lt.X, lt.Y, rb.X, rb.Y);
        }

        public virtual float GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            float result;
            if (workspaceUnit == Aurigma.GraphicsMill.Unit.Pixel)
                result = _zoom;
            else
            {
                float inches = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(this.ViewerResolution, 1.0f, workspaceUnit, Aurigma.GraphicsMill.Unit.Inch);
                result = inches * _viewerResolution * _zoom;
            }

            return result;
        }

        public virtual float GetControlPixelsPerUnitY(Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            return GetControlPixelsPerUnitX(workspaceUnit);
        }

        #endregion "[Public] ICoordinateMapper interface methods"

        #region "Zoom related methods"

        protected virtual void ApplyZoom(float zoomMultiplier)
        {
            if (_zoomMode == Aurigma.GraphicsMill.WinControls.ZoomMode.None)
                this.Zoom = this.Zoom * zoomMultiplier;
        }

        protected abstract void UpdateZoom(float newZoom);

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(0.1f)]
        [ResDescription("ViewerBase_MinZoom")]
        public virtual float MinZoom
        {
            get
            {
                return _minZoom;
            }

            set
            {
                _minZoom = Math.Max(eps, Math.Abs(value));
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(16.0f)]
        [ResDescription("ViewerBase_MaxZoom")]
        public virtual float MaxZoom
        {
            get
            {
                return _maxZoom;
            }

            set
            {
                _maxZoom = Math.Max(eps, Math.Abs(value));
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(1.0f)]
        [ResDescription("ViewerBase_Zoom")]
        public virtual float Zoom
        {
            get
            {
                return _zoom;
            }

            set
            {
                UpdateZoom(value);
            }
        }

        protected float ZoomInternal
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
            }
        }

        public virtual Aurigma.GraphicsMill.WinControls.ZoomMode ZoomMode
        {
            get
            {
                return _zoomMode;
            }
            set
            {
                if (_zoomMode != value)
                {
                    _zoomMode = value;
                    UpdateZoom(_zoom);
                }
            }
        }

        protected Aurigma.GraphicsMill.WinControls.ZoomMode ZoomModeInternal
        {
            get
            {
                return _zoomMode;
            }
            set
            {
                _zoomMode = value;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(1.5f)]
        [ResDescription("ViewerBase_WheelZoomAmount")]
        public virtual float WheelZoomAmount
        {
            get
            {
                return _wheelZoomAmount;
            }

            set
            {
                _wheelZoomAmount = Math.Abs(value);
            }
        }

        #endregion "Zoom related methods"

        #region "Cursors handling"

        public virtual void RestoreCursorToDefault()
        {
            this.Cursor = _defaultCursor != null ? _defaultCursor : System.Windows.Forms.Cursors.Default;
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(System.Windows.Forms.Cursor), "[Cursor: Default]")]
        [ResDescription("ViewerBase_DefaultCursor")]
        public System.Windows.Forms.Cursor DefaultCursor
        {
            get
            {
                return _defaultCursor;
            }

            set
            {
                _defaultCursor = value;

                if (_rubberband == null && _navigator == null)
                    this.Cursor = _defaultCursor;
            }
        }

        #endregion "Cursors handling"

        #region "Drawing methods"

        protected abstract Aurigma.GraphicsMill.Drawing.Graphics ControlGdiGraphics
        {
            get;
        }

        protected void DrawControlBorder(System.IntPtr hdc)
        {
            //
            // Investigated with .NET Reflector.
            //

            System.Drawing.Rectangle r = GetCanvasBounds();
            int cx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE);
            int cy = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE);
            r.Offset(-cx, -cy);

            NativeMethods.RECT rect;
            rect.left = r.Left;
            rect.top = r.Top;
            rect.right = r.Right;
            rect.bottom = r.Bottom;

            int edge = (int)_borderStyle & 15;
            int borderFlags = (int)(System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Right | System.Windows.Forms.Border3DSide.Top | System.Windows.Forms.Border3DSide.Bottom);
            borderFlags |= (int)(_borderStyle & ~(System.Windows.Forms.Border3DStyle.Sunken | System.Windows.Forms.Border3DStyle.Raised));

            if ((borderFlags & 0x2000) == 0x2000)
            {
                System.Drawing.Size borderSize = System.Windows.Forms.SystemInformation.Border3DSize;
                rect.left -= borderSize.Width;
                rect.right += borderSize.Width;
                rect.top -= borderSize.Height;
                rect.bottom += borderSize.Height;
                borderFlags &= -8193;
            }

            NativeMethods.DrawEdge(hdc, ref rect, edge, borderFlags);
        }

        #endregion "Drawing methods"

        #region "Member variables"

        private Aurigma.GraphicsMill.WinControls.IRubberband _rubberband;
        private Aurigma.GraphicsMill.WinControls.INavigator _navigator;

        private float _zoom;
        private float _minZoom;
        private float _maxZoom;
        private float _wheelZoomAmount;
        private Aurigma.GraphicsMill.WinControls.ZoomMode _zoomMode;

        private System.Windows.Forms.Cursor _defaultCursor;
        private System.Windows.Forms.Border3DStyle _borderStyle;

        private float _viewerResolution;

        #endregion "Member variables"
    }
}