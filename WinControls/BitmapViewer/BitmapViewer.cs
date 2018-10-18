// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Aurigma.GraphicsMill.Transforms;
using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Aurigma.GraphicsMill.WinControls
{
    [ResDescription("Enum_ScrollBarsStyle")]
    public enum ScrollBarsStyle
    {
        [ResDescription("Enum_ScrollBarsStyle_None")]
        None = 0,

        [ResDescription("Enum_ScrollBarsStyle_Always")]
        Always = 1,

        [ResDescription("Enum_ScrollBarsStyle_Auto")]
        Auto = 2
    }

    [ResDescription("Enum_ViewportAlignment")]
    public enum ViewportAlignment
    {
        [ResDescription("Enum_ViewportAlignment_LeftTop")]
        LeftTop = 0,

        [ResDescription("Enum_ViewportAlignment_LeftCenter")]
        LeftCenter = 1,

        [ResDescription("Enum_ViewportAlignment_LeftBottom")]
        LeftBottom = 2,

        [ResDescription("Enum_ViewportAlignment_CenterTop")]
        CenterTop = 3,

        [ResDescription("Enum_ViewportAlignment_CenterCenter")]
        CenterCenter = 4,

        [ResDescription("Enum_ViewportAlignment_CenterBottom")]
        CenterBottom = 5,

        [ResDescription("Enum_ViewportAlignment_RightTop")]
        RightTop = 6,

        [ResDescription("Enum_ViewportAlignment_RightCenter")]
        RightCenter = 7,

        [ResDescription("ViewportAlignment_RightBottom")]
        RightBottom = 8
    }

    [ResDescription("Enum_WorkspaceBackgroundStyle")]
    public enum WorkspaceBackgroundStyle
    {
        [ResDescription("Enum_WorkspaceBackgroundStyle_None")]
        None = 0,

        [ResDescription("Enum_WorkspaceBackgroundStyle_Solid")]
        Solid = 1,

        [ResDescription("Enum_WorkspaceBackgroundStyle_Grid")]
        Grid = 2
    }

    [ResDescription("Enum_ZoomMode")]
    public enum ZoomMode
    {
        [ResDescription("Enum_ZoomMode_None")]
        None = 0,

        [ResDescription("Enum_ZoomMode_BestFit")]
        BestFit = 1,

        [ResDescription("Enum_ZoomMode_BestFitShrinkOnly")]
        BestFitShrinkOnly = 2,

        [ResDescription("Enum_ZoomMode_FitToWidth")]
        FitToWidth = 3,

        [ResDescription("Enum_ZoomMode_FitToHeight")]
        FitToHeight = 4,

        [ResDescription("Enum_ZoomMode_ZoomControl")]
        ZoomControl = 5,

        [ResDescription("Enum_ZoomMode_FitToWidthShrinkOnly")]
        FitToWidthShrinkOnly = 6,

        [ResDescription("Enum_ZoomMode_FitToHeightShrinkOnly")]
        FitToHeightShrinkOnly = 7
    }

    [ResDescription("Enum_ZoomQuality")]
    public enum ZoomQuality
    {
        [ResDescription("Enum_ZoomQuality_Low")]
        Low = 0,

        [ResDescription("Enum_ZoomQuality_Medium")]
        Medium = 1,

        [ResDescription("Enum_ZoomQuality_High")]
        High = 2,

        [ResDescription("Enum_ZoomQuality_ShrinkHighStretchLow")]
        ShrinkHighStretchLow = 3
    }

    [ResDescription("BitmapViewer")]
    [AdaptiveToolboxBitmapAttribute(typeof(Aurigma.GraphicsMill.WinControls.BitmapViewer), "BitmapViewer.bmp")]
    public class BitmapViewer : ViewerBase
    {
        private static int backgroundGridCell = 10;
        private static float eps = 0.0001f;
        private static int wheelValue = 120;
        private bool _alphaEnabled;
        private bool _autoUpdate;
        private Aurigma.GraphicsMill.Bitmap _bitmap;
        private Aurigma.GraphicsMill.Bitmap _canvasBitmap;
        private Aurigma.GraphicsMill.Bitmap _canvasGrid;
        private Aurigma.GraphicsMill.Transforms.ColorManagementEngine _colorManagementEngine;
        private System.ComponentModel.Container _components;
        private System.Drawing.Size _contentSize;
        private Aurigma.GraphicsMill.Bitmap _cropBitmap;
        private int _horizontalLargeChange;
        private float _horizontalScale;
        private bool _horizontalScrollBarEnabled;
        private bool _horizontalScrollBarShow;
        private int _horizontalSmallChange;
        private bool _isSizeChanging;
        private Aurigma.GraphicsMill.Transforms.Resize _resize;
        private float _reverseHorizontalScale;
        private float _reverseVerticalScale;
        private float _reverseZoom;
        private bool _scaleToActualSize;
        private bool _scrollBarsInitialized;
        private Aurigma.GraphicsMill.WinControls.ScrollBarsStyle _scrollBarsStyle;
        private System.Drawing.Point _scrollingPosition;
        private System.Drawing.Point _scrollingShift;
        private UndoRedoBitmapCollection _undoRedoCollection = new UndoRedoBitmapCollection();
        private Aurigma.GraphicsMill.Unit _unit;
        private int _verticalLargeChange;
        private float _verticalScale;
        private bool _verticalScrollBarEnabled;
        private bool _verticalScrollBarShow;
        private int _verticalSmallChange;
        private Aurigma.GraphicsMill.WinControls.ViewportAlignment _viewportAlignment;
        private Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle _viewportBackgroundStyle;
        private int _viewportBorderWidth;
        private System.Drawing.Color _workspaceBackColor1;
        private System.Drawing.Color _workspaceBackColor2;
        private System.Drawing.Color _workspaceBorderColor;
        private bool _workspaceBorderEnabled;
        private Aurigma.GraphicsMill.WinControls.ZoomQuality _zoomQuality;

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _components = new System.ComponentModel.Container();
        }

        #endregion Component Designer generated code

        public BitmapViewer()
        {
            InitializeComponent();

            this.Width = 200;
            this.Height = 200;
            this.BackColor = System.Drawing.Color.White;
            _reverseZoom = 1.0f;
            _horizontalScale = 1.0f;
            _reverseHorizontalScale = 1.0f;
            _verticalScale = 1.0f;
            _reverseVerticalScale = 1.0f;
            _contentSize = new System.Drawing.Size(0, 0);
            _scrollingShift = new System.Drawing.Point(0, 0);
            _scrollingPosition = new System.Drawing.Point(0, 0);
            _scrollBarsStyle = Aurigma.GraphicsMill.WinControls.ScrollBarsStyle.Auto;
            _verticalSmallChange = 5;
            _verticalLargeChange = 25;
            _horizontalSmallChange = 5;
            _horizontalLargeChange = 25;
            _resize = new Aurigma.GraphicsMill.Transforms.Resize();
            _cropBitmap = new Aurigma.GraphicsMill.Bitmap();
            _workspaceBorderEnabled = true;
            _workspaceBorderColor = System.Drawing.Color.Blue;
            _viewportBorderWidth = 2;
            _viewportAlignment = Aurigma.GraphicsMill.WinControls.ViewportAlignment.CenterCenter;
            _alphaEnabled = true;
            _zoomQuality = Aurigma.GraphicsMill.WinControls.ZoomQuality.Low;
            _autoUpdate = true;
            _viewportBackgroundStyle = WorkspaceBackgroundStyle.Grid;
            _workspaceBackColor1 = System.Drawing.Color.LightGray;
            _workspaceBackColor2 = System.Drawing.Color.White;
            _unit = Aurigma.GraphicsMill.Unit.Pixel;
        }

        public void Update()
        {
            UpdateBitmap();
            OnWorkspaceChanged(System.EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                    _components = null;
                }

                if (_resize != null)
                {
                    _resize.Dispose();
                    _resize = null;
                }

                if (_cropBitmap != null)
                {
                    _cropBitmap.Dispose();
                    _cropBitmap = null;
                }

                base.Rubberband = null;

                base.Navigator = null;
            }

            base.Dispose(disposing);
        }

        #region Public Properties Of Control

        [System.ComponentModel.Browsable(false)]
        public float ActualSizeHorizontalScale
        {
            get
            {
                return _horizontalScale;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public float ActualSizeVerticalScale
        {
            get
            {
                return _verticalScale;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("BitmapViewer_AlphaEnabled")]
        public bool AlphaEnabled
        {
            get
            {
                return _alphaEnabled;
            }

            set
            {
                _alphaEnabled = value;
                UpdateBitmap();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("BitmapViewer_AutoUpdate")]
        public bool AutoUpdate
        {
            get
            {
                return _autoUpdate;
            }

            set
            {
                _autoUpdate = value;

                if (_autoUpdate)
                {
                    UpdateBitmap();
                }
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override System.Drawing.Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Aurigma.GraphicsMill.Bitmap Bitmap
        {
            get
            {
                return _Bitmap;
            }
            set
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate { _Bitmap = value; });
                }
                else
                {
                    _Bitmap = value;
                }
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(Aurigma.GraphicsMill.Transforms.ColorManagementEngine.None)]
        [ResDescription("BitmapViewer_ColorManagementEngine")]
        public Aurigma.GraphicsMill.Transforms.ColorManagementEngine ColorManagementEngine
        {
            get
            {
                return _colorManagementEngine;
            }

            set
            {
                _colorManagementEngine = value;
                InvalidateBitmap();
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override System.Drawing.Font Font
        {
            get
            {
                return base.Font;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override System.Drawing.Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public override bool HasContent
        {
            get
            {
                return !this.BitmapIsEmpty();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(25)]
        [ResDescription("BitmapViewer_HorizontalLargeChange")]
        public int HorizontalLargeChange
        {
            get
            {
                return _horizontalLargeChange;
            }

            set
            {
                _horizontalLargeChange = Math.Abs(value);
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(5)]
        [ResDescription("BitmapViewer_HorizontalSmallChange")]
        public int HorizontalSmallChange
        {
            get
            {
                return _horizontalSmallChange;
            }

            set
            {
                _horizontalSmallChange = Math.Abs(value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override System.Windows.Forms.RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(false)]
        [ResDescription("BitmapViewer_ActualSize")]
        public bool ScaleToActualSize
        {
            get
            {
                return _scaleToActualSize;
            }

            set
            {
                _scaleToActualSize = value;
                UpdateBitmap();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.ScrollBarsStyle), "Auto")]
        [ResDescription("BitmapViewer_ScrollBarsStyle")]
        public override Aurigma.GraphicsMill.WinControls.ScrollBarsStyle ScrollBarsStyle
        {
            get
            {
                return _scrollBarsStyle;
            }

            set
            {
                _scrollBarsStyle = value;
                InvalidateBitmap();
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override System.Drawing.Point ScrollingPosition
        {
            get
            {
                return _scrollingPosition;
            }

            set
            {
                _scrollingPosition = value;
                UpdateBitmap();
            }
        }

        [System.ComponentModel.Browsable(false)]
        public System.Drawing.Size ScrollingSize
        {
            get
            {
                System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();
                return new System.Drawing.Size(Math.Max(0, _contentSize.Width - canvasRectangle.Width), Math.Max(0, _contentSize.Height - canvasRectangle.Height));
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.Unit), "Pixel")]
        [ResDescription("BitmapViewer_Unit")]
        public override Aurigma.GraphicsMill.Unit Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(25)]
        [ResDescription("BitmapViewer_VerticalLargeChange")]
        public int VerticalLargeChange
        {
            get
            {
                return _verticalLargeChange;
            }

            set
            {
                _verticalLargeChange = Math.Abs(value);
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(5)]
        [ResDescription("BitmapViewer_VerticalSmallChange")]
        public int VerticalSmallChange
        {
            get
            {
                return _verticalSmallChange;
            }

            set
            {
                _verticalSmallChange = Math.Abs(value);
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.ViewportAlignment), "CenterCenter")]
        [ResDescription("BitmapViewer_ViewportAlignment")]
        public override Aurigma.GraphicsMill.WinControls.ViewportAlignment ViewportAlignment
        {
            get
            {
                return _viewportAlignment;
            }

            set
            {
                _viewportAlignment = value;
                InvalidateViewer();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [DefaultValue(typeof(System.Drawing.Color), "LightGray")]
        [ResDescription("BitmapViewer_WorkspaceBackColor1")]
        public System.Drawing.Color WorkspaceBackColor1
        {
            get
            {
                return _workspaceBackColor1;
            }

            set
            {
                _workspaceBackColor1 = value;
                UpdateBitmap();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [DefaultValue(typeof(System.Drawing.Color), "White")]
        [ResDescription("BitmapViewer_WorkspaceBackColor2")]
        public System.Drawing.Color WorkspaceBackColor2
        {
            get
            {
                return _workspaceBackColor2;
            }

            set
            {
                _workspaceBackColor2 = value;
                UpdateBitmap();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle), "Grid")]
        [ResDescription("BitmapViewer_WorkspaceBackgroundStyle")]
        public Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle WorkspaceBackgroundStyle
        {
            get
            {
                return _viewportBackgroundStyle;
            }

            set
            {
                _viewportBackgroundStyle = value;
                UpdateBitmap();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [DefaultValue(typeof(System.Drawing.Color), "Blue")]
        [ResDescription("BitmapViewer_WorkspaceBorderColor")]
        public override System.Drawing.Color WorkspaceBorderColor
        {
            get
            {
                return _workspaceBorderColor;
            }

            set
            {
                _workspaceBorderColor = value;
                UpdateBitmap();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("BitmapViewer_WorkspaceBorderEnabled")]
        public override bool WorkspaceBorderEnabled
        {
            get
            {
                return _workspaceBorderEnabled;
            }

            set
            {
                _workspaceBorderEnabled = value;
                InvalidateBitmap();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(2)]
        [ResDescription("BitmapViewer_WorkspaceBorderWidth")]
        public override int WorkspaceBorderWidth
        {
            get
            {
                return _viewportBorderWidth;
            }

            set
            {
                _viewportBorderWidth = Math.Abs(value);
                UpdateBitmap();
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public override float WorkspaceHeight
        {
            get
            {
                if (BitmapIsEmpty())
                    return 0;
                else
                {
                    float resolution = (_scaleToActualSize && !BitmapIsEmpty()) ? this.Bitmap.DpiY : base.ViewerResolution;
                    return ConvertPixelsToUnits(resolution, this.Bitmap.Height, _unit);
                }
            }
            set
            {
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("BitmapViewerDoesntImplementWorkspaceHeight"));
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public override float WorkspaceWidth
        {
            get
            {
                if (BitmapIsEmpty())
                    return 0;
                else
                {
                    float resolution = (_scaleToActualSize && !BitmapIsEmpty()) ? this.Bitmap.DpiX : base.ViewerResolution;
                    return ConvertPixelsToUnits(resolution, this.Bitmap.Width, _unit);
                }
            }
            set
            {
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("BitmapViewerDoesntImplementWorkspaceWidth"));
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.ZoomQuality), "Low")]
        [ResDescription("BitmapViewer_ZoomQuality")]
        public Aurigma.GraphicsMill.WinControls.ZoomQuality ZoomQuality
        {
            get
            {
                return _zoomQuality;
            }

            set
            {
                _zoomQuality = value;
                InvalidateBitmap();
            }
        }

        private Aurigma.GraphicsMill.Bitmap _Bitmap
        {
            get
            {
                return _bitmap;
            }
            set
            {
                _bitmap = value;

                _undoRedoCollection.Add(value);

                OnContentChanged(System.EventArgs.Empty);

                System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();
                if (canvasRectangle.Width > 0 && canvasRectangle.Height > 0)
                {
                    _canvasBitmap = new Aurigma.GraphicsMill.Bitmap(canvasRectangle.Width, canvasRectangle.Height, PixelFormat.Format24bppRgb);
                    _canvasBitmap.ColorProfile = Aurigma.GraphicsMill.ColorProfile.FromScreen();
                    _canvasBitmap.ColorManagement.ColorManagementEngine = ColorManagementEngine.None;
                }

                if (_autoUpdate)
                {
                    UpdateBitmap();
                    OnWorkspaceChanged(System.EventArgs.Empty);
                }
            }
        }

        #endregion Public Properties Of Control

        #region Events

        public event System.EventHandler ContentChanged;

        private void OnContentChanged(System.EventArgs e)
        {
            if (this.ContentChanged != null)
                this.ContentChanged(this, e);
        }

        #endregion Events

        #region Undo/Redo

        public bool CanRedo
        {
            get
            {
                return _undoRedoCollection.CanRedo;
            }
        }

        public bool CanUndo
        {
            get
            {
                return _undoRedoCollection.CanUndo;
            }
        }

        public int UndoRedoMaxStepCount
        {
            get
            {
                return _undoRedoCollection.MaxCount - 1;
            }
            set
            {
                if (value < 0)
                    throw new System.ArgumentException("UndoRedoMaxStepCount can not be negative.");

                _undoRedoCollection.MaxCount = value + 1;
            }
        }

        public void Redo()
        {
            Bitmap next = _undoRedoCollection.Redo();

            if (next != null)
            {
                _bitmap = next;

                UpdateBitmap();
                OnWorkspaceChanged(System.EventArgs.Empty);
                OnContentChanged(System.EventArgs.Empty);
            }
        }

        public void Undo()
        {
            Bitmap prev = _undoRedoCollection.Undo();

            if (prev != null)
            {
                _bitmap = prev;

                UpdateBitmap();
                OnWorkspaceChanged(System.EventArgs.Empty);
                OnContentChanged(System.EventArgs.Empty);
            }
        }

        #endregion Undo/Redo

        #region Events Handlers

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            Focus();

            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();
            System.Drawing.Point point = new System.Drawing.Point(e.X, e.Y);
            System.Drawing.Point movedPoint = new System.Drawing.Point(e.X - canvasRectangle.X, e.Y - canvasRectangle.Y);
            if (GetCanvasOutputBounds().Contains(movedPoint))
                OnWorkspaceMouseDown(new Aurigma.GraphicsMill.WinControls.MouseEventArgs(e.Button, e.Clicks, ControlToWorkspace(point, _unit)));

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();
            System.Drawing.Point point = new System.Drawing.Point(e.X, e.Y);
            System.Drawing.Point movedPoint = new System.Drawing.Point(e.X - canvasRectangle.X, e.Y - canvasRectangle.Y);
            if (GetCanvasOutputBounds().Contains(movedPoint))
                OnWorkspaceMouseMove(new Aurigma.GraphicsMill.WinControls.MouseEventArgs(e.Button, e.Clicks, ControlToWorkspace(point, _unit)));

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);

            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();
            System.Drawing.Point point = new System.Drawing.Point(e.X, e.Y);
            System.Drawing.Point movedPoint = new System.Drawing.Point(e.X - canvasRectangle.X, e.Y - canvasRectangle.Y);
            if (GetCanvasOutputBounds().Contains(movedPoint))
                OnWorkspaceMouseUp(new Aurigma.GraphicsMill.WinControls.MouseEventArgs(e.Button, e.Clicks, ControlToWorkspace(point, _unit)));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                using (var gr = _canvasBitmap.GetGdiPlusGraphics())
                using (var aGr = _canvasBitmap.GetGraphics())
                {
                    DrawBitmap(aGr);
                    DrawWorkspaceBorder(aGr);
                    if (base.BorderStyle != System.Windows.Forms.Border3DStyle.Flat)
                        DrawControlBorder(aGr.GetDC());
                    OnDoubleBufferPaint(new System.Windows.Forms.PaintEventArgs(gr, GetCanvasOutputBounds()));
                }

                _canvasBitmap.DrawOn(e.Graphics, GetCanvasBoundsWithBorder(), CombineMode.Copy, 1.0f, ResizeInterpolationMode.Medium);
            }
            finally
            {
                base.OnPaint(e);
            }

            return;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (_isSizeChanging)
                return;
            _isSizeChanging = true;
            UpdateCanvas();

            base.OnSizeChanged(e);
            _isSizeChanging = false;
        }

        [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)NativeMethods.WM_HSCROLL:
                    {
                        if (m.LParam != IntPtr.Zero)
                        {
                            base.WndProc(ref m);
                            break;
                        }

                        HorizontalScrollHandler(System.Convert.ToUInt32(m.WParam.ToInt64() & 0xFFFFL), System.Convert.ToInt32(m.WParam.ToInt64() >> 16 & 0xFFFF));
                        break;
                    }

                case (int)NativeMethods.WM_VSCROLL:
                    {
                        if (m.LParam != IntPtr.Zero)
                        {
                            base.WndProc(ref m);
                            break;
                        }

                        VerticalScrollHandler(System.Convert.ToUInt32(m.WParam.ToInt64() & 0xFFFF), System.Convert.ToInt32(m.WParam.ToInt64() >> 16 & 0xFFFF));
                        break;
                    }

                case (int)NativeMethods.WM_MOUSEWHEEL:
                    {
                        // An ugly but portable (looks like it) way to convert bits 16-31 to Int16
                        int delta = System.Convert.ToInt32(m.WParam.ToInt64() >> 16);
                        if (delta >= (1 << 15))
                            delta -= (1 << 16);

                        MouseWheelHandler(delta, System.Convert.ToInt32(m.WParam.ToInt64() & 0xFFFF));
                        base.WndProc(ref m);
                        break;
                    }

                case (int)NativeMethods.WM_NCCREATE:
                    {
                        NativeMethods.CREATESTRUCT pStruct = (NativeMethods.CREATESTRUCT)System.Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.CREATESTRUCT));

                        pStruct.style |= NativeMethods.WS_HSCROLL | NativeMethods.WS_VSCROLL;
                        System.Runtime.InteropServices.Marshal.StructureToPtr(pStruct, m.LParam, true);
                        base.WndProc(ref m);
                        break;
                    }

                case (int)NativeMethods.WM_CREATE:
                    {
                        NativeMethods.INITCOMMONCONTROLSEX objControls = new NativeMethods.INITCOMMONCONTROLSEX(NativeMethods.ICC_PAGESCROLLER_CLASS | NativeMethods.ICC_STANDARD_CLASSES | NativeMethods.ICC_LINK_CLASS);

                        NativeMethods.InitCommonControlsEx(ref objControls);
                        base.WndProc(ref m);
                        break;
                    }

                default:
                    {
                        base.WndProc(ref m);
                        break;
                    }
            }
        }

        private void HorizontalScrollHandler(uint cmd, int position)
        {
            switch (cmd)
            {
                case NativeMethods.SB_THUMBTRACK:
                case NativeMethods.SB_THUMBPOSITION:

                    NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO(NativeMethods.SIF_TRACKPOS, 0, 0, 0, 0);
                    if (NativeMethods.GetScrollInfo(this.Handle, NativeMethods.SB_HORZ, ref si))
                        position = si.nTrackPos;

                    ScrollHorizontal(position - _scrollingPosition.X);
                    break;

                case NativeMethods.SB_LEFT:
                    ScrollToBeginOrEnd(false, false);
                    break;

                case NativeMethods.SB_LINELEFT:
                    ScrollStep(false, false);
                    break;

                case NativeMethods.SB_PAGELEFT:
                    ScrollPage(false, false);
                    break;

                case NativeMethods.SB_RIGHT:
                    ScrollToBeginOrEnd(false, true);
                    break;

                case NativeMethods.SB_LINERIGHT:
                    ScrollStep(false, true);
                    break;

                case NativeMethods.SB_PAGERIGHT:
                    ScrollPage(false, true);
                    break;
            }
        }

        private void MouseWheelHandler(int amount, int buttonsState)
        {
            int delta = amount / wheelValue;

            if (buttonsState == 0)
                ScrollVertical(-delta * _verticalSmallChange);
            else if (buttonsState == NativeMethods.MK_CONTROL)
                ScrollHorizontal(-delta * _horizontalSmallChange);
            else if (buttonsState == NativeMethods.MK_SHIFT)
                if (delta < 0)
                    ApplyZoom(base.WheelZoomAmount * -delta);
                else if (delta > 0)
                    ApplyZoom(1.0f / (base.WheelZoomAmount * delta));
        }

        private void VerticalScrollHandler(uint cmd, int position)
        {
            switch (cmd)
            {
                case NativeMethods.SB_THUMBTRACK:
                case NativeMethods.SB_THUMBPOSITION:

                    NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO(NativeMethods.SIF_TRACKPOS, 0, 0, 0, 0);
                    if (NativeMethods.GetScrollInfo(Handle, NativeMethods.SB_VERT, ref si))
                        position = si.nTrackPos;

                    ScrollVertical(position - _scrollingPosition.Y);
                    break;

                case NativeMethods.SB_LEFT:
                    ScrollToBeginOrEnd(true, false);
                    break;

                case NativeMethods.SB_LINELEFT:
                    ScrollStep(true, false);
                    break;

                case NativeMethods.SB_PAGELEFT:
                    ScrollPage(true, false);
                    break;

                case NativeMethods.SB_RIGHT:
                    ScrollToBeginOrEnd(true, true);
                    break;

                case NativeMethods.SB_LINERIGHT:
                    ScrollStep(true, true);
                    break;

                case NativeMethods.SB_PAGERIGHT:
                    ScrollPage(true, true);
                    break;
            }
        }

        #endregion Events Handlers

        #region Scroll Bars Handling

        public override void Scroll(int horizontalDelta, int verticalDelta)
        {
            bool needToInvalidate = false;
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

            if (_scrollingShift.X == 0 && horizontalDelta != 0 && (base.ZoomModeInternal == Aurigma.GraphicsMill.WinControls.ZoomMode.None || base.ZoomModeInternal == Aurigma.GraphicsMill.WinControls.ZoomMode.FitToHeight))
            {
                if (horizontalDelta > 0)
                    horizontalDelta = Math.Max(0, Math.Min(horizontalDelta, _contentSize.Width - _scrollingPosition.X - canvasRectangle.Width));
                else if (horizontalDelta < 0)
                    horizontalDelta = Math.Max(horizontalDelta, -_scrollingPosition.X);

                _scrollingPosition.X += horizontalDelta;

                if (horizontalDelta < canvasRectangle.Width)
                    _scrollingShift.X += horizontalDelta;

                needToInvalidate = true;
            }

            if (_scrollingShift.Y == 0 && verticalDelta != 0 && (base.ZoomModeInternal == Aurigma.GraphicsMill.WinControls.ZoomMode.None || base.ZoomModeInternal == Aurigma.GraphicsMill.WinControls.ZoomMode.FitToWidth))
            {
                if (verticalDelta > 0)
                    verticalDelta = Math.Max(0, Math.Min(verticalDelta, _contentSize.Height - _scrollingPosition.Y - canvasRectangle.Height));
                else if (verticalDelta < 0)
                    verticalDelta = Math.Max(verticalDelta, -_scrollingPosition.Y);

                _scrollingPosition.Y += verticalDelta;

                if (verticalDelta < canvasRectangle.Height)
                    _scrollingShift.Y += verticalDelta;

                needToInvalidate = true;
            }

            if (needToInvalidate)
            {
                OnScrolled(System.EventArgs.Empty);
                InvalidateBitmap();
            }
        }

        public override void Scroll(bool scrollVertically, ScrollValue scrollValue)
        {
            switch (scrollValue)
            {
                case ScrollValue.Begin:
                    ScrollToBeginOrEnd(scrollVertically, false);
                    break;

                case ScrollValue.End:
                    ScrollToBeginOrEnd(scrollVertically, true);
                    break;

                case ScrollValue.PageBack:
                    ScrollPage(scrollVertically, false);
                    break;

                case ScrollValue.PageForward:
                    ScrollPage(scrollVertically, true);
                    break;

                case ScrollValue.StepBack:
                    ScrollStep(scrollVertically, false);
                    break;

                case ScrollValue.StepForward:
                    ScrollStep(scrollVertically, true);
                    break;

                default:
                    throw new System.ArgumentException(StringResources.GetString("ExStrUnexpectedScrollValue"), "scrollValue");
            }
        }

        public void ScrollHorizontal(int scrollDelta)
        {
            if (_scrollingShift.X == 0 && scrollDelta != 0 && (base.ZoomModeInternal == Aurigma.GraphicsMill.WinControls.ZoomMode.None || base.ZoomModeInternal == Aurigma.GraphicsMill.WinControls.ZoomMode.FitToHeight))
            {
                System.Drawing.Rectangle canvasRectangle = this.GetCanvasBounds();

                if (scrollDelta > 0)
                    scrollDelta = Math.Max(0, Math.Min(scrollDelta, _contentSize.Width - _scrollingPosition.X - canvasRectangle.Width));
                else if (scrollDelta < 0)
                    scrollDelta = Math.Max(scrollDelta, -_scrollingPosition.X);

                _scrollingPosition.X += scrollDelta;

                if (scrollDelta < canvasRectangle.Width)
                    _scrollingShift.X += scrollDelta;

                OnScrolled(System.EventArgs.Empty);

                InvalidateBitmap();
            }
        }

        public void ScrollVertical(int scrollDelta)
        {
            if (_scrollingShift.Y == 0 && scrollDelta != 0 && (base.ZoomModeInternal == Aurigma.GraphicsMill.WinControls.ZoomMode.None || base.ZoomModeInternal == Aurigma.GraphicsMill.WinControls.ZoomMode.FitToWidth))
            {
                System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

                if (scrollDelta > 0)
                    scrollDelta = Math.Max(0, Math.Min(scrollDelta, _contentSize.Height - _scrollingPosition.Y - canvasRectangle.Height));
                else if (scrollDelta < 0)
                    scrollDelta = Math.Max(scrollDelta, -_scrollingPosition.Y);

                _scrollingPosition.Y += scrollDelta;

                if (scrollDelta < canvasRectangle.Height)
                    _scrollingShift.Y += scrollDelta;

                OnScrolled(System.EventArgs.Empty);

                InvalidateBitmap();
            }
        }

        private void ScrollPage(bool scrollVertically, bool scrollForward)
        {
            System.Drawing.Point objNewShift = new System.Drawing.Point();

            if (scrollVertically)
                objNewShift = new System.Drawing.Point(_scrollingPosition.X, _scrollingPosition.Y + (scrollForward ? _verticalLargeChange : -_verticalLargeChange));
            else
                objNewShift = new System.Drawing.Point(_scrollingPosition.X + (scrollForward ? _horizontalLargeChange : -_horizontalLargeChange), _scrollingPosition.Y);

            Scroll(objNewShift.X - _scrollingPosition.X, objNewShift.Y - _scrollingPosition.Y);
        }

        private void ScrollStep(bool scrollVertically, bool scrollForward)
        {
            System.Drawing.Point objNewShift = new System.Drawing.Point();
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

            if (scrollVertically)
            {
                objNewShift = new System.Drawing.Point(_scrollingPosition.X, _scrollingPosition.Y + (scrollForward ? _verticalSmallChange : -_verticalSmallChange));
                if ((scrollForward && _contentSize.Height - objNewShift.Y < canvasRectangle.Height) || (!scrollForward && objNewShift.Y < 0))
                {
                    ScrollToBeginOrEnd(scrollVertically, scrollForward);
                    return;
                }
            }
            else
            {
                objNewShift = new System.Drawing.Point(_scrollingPosition.X + (scrollForward ? _horizontalSmallChange : -_horizontalSmallChange), _scrollingPosition.Y);
                if ((scrollForward && _contentSize.Width - objNewShift.X < canvasRectangle.Width) || (!scrollForward && objNewShift.X < 0))
                {
                    ScrollToBeginOrEnd(scrollVertically, scrollForward);
                    return;
                }
            }

            Scroll(objNewShift.X - _scrollingPosition.X, objNewShift.Y - _scrollingPosition.Y);
        }

        private void ScrollToBeginOrEnd(bool scrollVertically, bool scrollForward)
        {
            System.Drawing.Point objNewShift = new System.Drawing.Point();
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

            if (scrollVertically)
            {
                if (scrollForward)
                {
                    if (_contentSize.Height < canvasRectangle.Height)
                        ScrollToBeginOrEnd(scrollVertically, false);
                    else
                        objNewShift = new System.Drawing.Point(_scrollingPosition.X, _contentSize.Height - canvasRectangle.Height);
                }
                else
                {
                    objNewShift = new System.Drawing.Point(_scrollingPosition.X, 0);
                }
            }
            else
            {
                if (scrollForward)
                {
                    if (_contentSize.Width < canvasRectangle.Width)
                        ScrollToBeginOrEnd(scrollVertically, false);
                    else
                        objNewShift = new System.Drawing.Point(_contentSize.Width - canvasRectangle.Width, _scrollingPosition.Y);
                }
                else
                {
                    objNewShift = new System.Drawing.Point(0, _scrollingPosition.Y);
                }
            }

            Scroll(objNewShift.X - _scrollingPosition.X, objNewShift.Y - _scrollingPosition.Y);
        }

        private void UpdateScrollBars()
        {
            if (!_scrollBarsInitialized)
            {
                NativeMethods.SCROLLINFO objVerticalScroll = new NativeMethods.SCROLLINFO(NativeMethods.SIF_RANGE | NativeMethods.SIF_POS, 0, 100, 10, 10);
                NativeMethods.SetScrollInfo(Handle, NativeMethods.SB_BOTH, ref objVerticalScroll, true);
                _scrollBarsInitialized = true;
            }

            UpdateScrollBarsState();
            UpdateScrollBarsRange();
        }

        private void UpdateScrollBarsRange()
        {
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

            _scrollingPosition.X = Math.Max(0, Math.Min(_scrollingPosition.X, _contentSize.Width - canvasRectangle.Width));
            if (_horizontalScrollBarShow)
            {
                if (Math.Max(0, _contentSize.Width - canvasRectangle.Width) > 0)
                {
                    NativeMethods.SCROLLINFO objHorizontalScroll = new NativeMethods.SCROLLINFO(NativeMethods.SIF_RANGE | NativeMethods.SIF_POS | NativeMethods.SIF_PAGE, 0, _contentSize.Width, canvasRectangle.Width, _scrollingPosition.X);
                    NativeMethods.SetScrollInfo(Handle, NativeMethods.SB_HORZ, ref objHorizontalScroll, true);
                }
                NativeMethods.EnableScrollBar(Handle, NativeMethods.SB_HORZ, _horizontalScrollBarEnabled ? NativeMethods.ESB_ENABLE_BOTH : NativeMethods.ESB_DISABLE_BOTH);
            }

            _scrollingPosition.Y = Math.Max(0, Math.Min(_scrollingPosition.Y, _contentSize.Height - canvasRectangle.Height));
            if (_verticalScrollBarShow)
            {
                if (Math.Max(0, _contentSize.Height - canvasRectangle.Height) > 0)
                {
                    NativeMethods.SCROLLINFO objVerticalScroll = new NativeMethods.SCROLLINFO(NativeMethods.SIF_RANGE | NativeMethods.SIF_POS | NativeMethods.SIF_PAGE, 0, _contentSize.Height, canvasRectangle.Height, _scrollingPosition.Y);
                    NativeMethods.SetScrollInfo(Handle, NativeMethods.SB_VERT, ref objVerticalScroll, true);
                }
                NativeMethods.EnableScrollBar(Handle, NativeMethods.SB_VERT, _verticalScrollBarEnabled ? NativeMethods.ESB_ENABLE_BOTH : NativeMethods.ESB_DISABLE_BOTH);
            }
        }

        private void UpdateScrollBarsState()
        {
            if (!IsHandleCreated)
                return;

            _horizontalScrollBarShow = true;
            _horizontalScrollBarEnabled = true;
            _verticalScrollBarShow = true;
            _verticalScrollBarEnabled = true;

            System.Drawing.Rectangle objCanvasTotalRectangle = GetControlBoundsWithoutBorder();
            if (_scrollBarsStyle == ScrollBarsStyle.Auto)
            {
                _horizontalScrollBarShow = _contentSize.Width > objCanvasTotalRectangle.Width;
                _verticalScrollBarShow = _contentSize.Height > objCanvasTotalRectangle.Height;

                System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();
                if (!_horizontalScrollBarShow && canvasRectangle.Width < _contentSize.Width)
                    _horizontalScrollBarShow = true;

                canvasRectangle = GetCanvasBounds();
                if (!_verticalScrollBarShow && canvasRectangle.Height < _contentSize.Height)
                    _verticalScrollBarShow = true;
            }
            else if (_scrollBarsStyle == ScrollBarsStyle.Always)
            {
                _horizontalScrollBarEnabled = _contentSize.Width > objCanvasTotalRectangle.Width;
                _verticalScrollBarEnabled = _contentSize.Height > objCanvasTotalRectangle.Height;
            }
            else
            {
                _horizontalScrollBarShow = false;
                _verticalScrollBarShow = false;
            }

            if (_verticalScrollBarShow && _horizontalScrollBarShow)
            {
                NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_BOTH, true);
            }
            else
            {
                if (_verticalScrollBarShow)
                {
                    NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_VERT, true);
                }
                else
                {
                    NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_VERT, false);
                    _scrollingPosition.Y = Math.Min(_scrollingPosition.Y, _contentSize.Height - objCanvasTotalRectangle.Height);
                }

                if (_horizontalScrollBarShow)
                {
                    NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_HORZ, true);
                }
                else
                {
                    NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_HORZ, false);
                    _scrollingPosition.X = Math.Min(_scrollingPosition.X, _contentSize.Width - objCanvasTotalRectangle.Width);
                }
            }
        }

        #endregion Scroll Bars Handling

        #region Bitmap Handling

        public void UpdateControlAlongWithBitmap()
        {
            UpdateBitmap();
            InvalidateBitmap();
        }

        protected override void UpdateZoom(float newZoom)
        {
            UpdateZoom(newZoom, true);
        }

        private bool BitmapIsEmpty()
        {
            if (this.Bitmap != null && !this.Bitmap.IsEmpty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void InvalidateBitmap()
        {
            UpdateScrollBars();
            InvalidateViewer();
        }

        private void UpdateBitmap()
        {
            if (!BitmapIsEmpty())
            {
                if (_scaleToActualSize)
                {
                    System.IntPtr hDc = NativeMethods.GetDC(System.IntPtr.Zero);
                    float monitorResX = NativeMethods.GetDeviceCaps(hDc, NativeMethods.LOGPIXELSX);
                    float monitorResY = NativeMethods.GetDeviceCaps(hDc, NativeMethods.LOGPIXELSY);
                    NativeMethods.ReleaseDC(System.IntPtr.Zero, hDc);

                    float resX = this.Bitmap.DpiX;
                    float resY = this.Bitmap.DpiY;

                    _horizontalScale = resX > eps ? monitorResX / resX : 1.0f;
                    _verticalScale = resY > eps ? monitorResY / resY : 1.0f;
                }
                else
                {
                    _horizontalScale = 1.0f;
                    _verticalScale = 1.0f;
                }
                _reverseHorizontalScale = 1.0f / _horizontalScale;
                _reverseVerticalScale = 1.0f / _verticalScale;

                float zoom = this.Zoom;
                System.Drawing.Rectangle canvasTotalRectangle = GetControlBoundsWithoutBorder();
                int scrollBarCx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);
                float horizontalZoomWithoutScrollBars = (float)canvasTotalRectangle.Width / ((float)this.Bitmap.Width * _horizontalScale);
                float verticalZoomWithoutScrollBars = (float)canvasTotalRectangle.Height / ((float)this.Bitmap.Height * _verticalScale);
                float horizontalZoom = (float)(canvasTotalRectangle.Width - scrollBarCx + (base.BorderStyle != System.Windows.Forms.Border3DStyle.Flat ? NativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE) : 0)) / ((float)this.Bitmap.Width * _horizontalScale);
                float verticalZoom = (float)(canvasTotalRectangle.Height - scrollBarCx + (base.BorderStyle != System.Windows.Forms.Border3DStyle.Flat ? NativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE) : 0)) / ((float)this.Bitmap.Height * _verticalScale);
                switch (base.ZoomModeInternal)
                {
                    case Aurigma.GraphicsMill.WinControls.ZoomMode.BestFit:
                        zoom = Math.Min(horizontalZoomWithoutScrollBars, verticalZoomWithoutScrollBars);
                        break;

                    case Aurigma.GraphicsMill.WinControls.ZoomMode.BestFitShrinkOnly:
                        zoom = Math.Min(1.0f, Math.Min(horizontalZoomWithoutScrollBars, verticalZoomWithoutScrollBars));
                        break;

                    case Aurigma.GraphicsMill.WinControls.ZoomMode.FitToHeight:
                        if (_horizontalScale * verticalZoomWithoutScrollBars * this.Bitmap.Width > canvasTotalRectangle.Width)
                        {
                            if (_horizontalScale * verticalZoom * this.Bitmap.Width < canvasTotalRectangle.Width)
                                zoom = horizontalZoomWithoutScrollBars;
                            else
                                zoom = verticalZoom;
                        }
                        else
                        {
                            zoom = verticalZoomWithoutScrollBars;
                        }
                        break;

                    case Aurigma.GraphicsMill.WinControls.ZoomMode.FitToHeightShrinkOnly:
                        if (_horizontalScale * verticalZoomWithoutScrollBars * this.Bitmap.Width > canvasTotalRectangle.Width)
                        {
                            if (_horizontalScale * verticalZoom * this.Bitmap.Width < canvasTotalRectangle.Width)
                                zoom = horizontalZoomWithoutScrollBars;
                            else
                                zoom = verticalZoom;
                        }
                        else
                        {
                            zoom = verticalZoomWithoutScrollBars;
                        }
                        zoom = Math.Min(1.0f, zoom);
                        break;

                    case Aurigma.GraphicsMill.WinControls.ZoomMode.FitToWidth:
                        if (_verticalScale * horizontalZoomWithoutScrollBars * this.Bitmap.Height > canvasTotalRectangle.Height)
                        {
                            if (_verticalScale * horizontalZoom * this.Bitmap.Height < canvasTotalRectangle.Height)
                                zoom = verticalZoomWithoutScrollBars;
                            else
                                zoom = horizontalZoom;
                        }
                        else
                        {
                            zoom = horizontalZoomWithoutScrollBars;
                        }
                        break;

                    case Aurigma.GraphicsMill.WinControls.ZoomMode.FitToWidthShrinkOnly:
                        if (_verticalScale * horizontalZoomWithoutScrollBars * this.Bitmap.Height > canvasTotalRectangle.Height)
                        {
                            if (_verticalScale * horizontalZoom * this.Bitmap.Height < canvasTotalRectangle.Height)
                                zoom = verticalZoomWithoutScrollBars;
                            else
                                zoom = horizontalZoom;
                        }
                        else
                        {
                            zoom = horizontalZoomWithoutScrollBars;
                        }
                        zoom = Math.Min(1.0f, zoom);
                        break;

                    case Aurigma.GraphicsMill.WinControls.ZoomMode.ZoomControl:
                        zoom = 1;
                        break;
                }
                UpdateZoom(zoom, false);

                _reverseZoom = 1.0f / base.Zoom;
                _contentSize = new System.Drawing.Size(System.Convert.ToInt32(_horizontalScale * base.Zoom * this.Bitmap.Width), System.Convert.ToInt32(_verticalScale * base.Zoom * this.Bitmap.Height));

                if (base.ZoomModeInternal == Aurigma.GraphicsMill.WinControls.ZoomMode.ZoomControl)
                    SetControlSize(new System.Drawing.Size(_contentSize.Width, _contentSize.Height));
            }
            else
            {
                _contentSize = new System.Drawing.Size(0, 0);
            }

            InvalidateBitmap();
        }

        private void UpdateZoom(float newZoom, bool updateBitmap)
        {
            float zoom = Math.Min(Math.Max(newZoom, base.MinZoom), base.MaxZoom);

            if (Math.Abs(base.ZoomInternal - zoom) > eps || updateBitmap)
            {
                if (updateBitmap)
                {
                    System.Drawing.Rectangle renderingRectangle = GetViewportBounds();
                    System.Drawing.PointF workspacePoint = ControlToWorkspace(new System.Drawing.Point(renderingRectangle.Left + renderingRectangle.Width / 2, renderingRectangle.Top + renderingRectangle.Height / 2), Aurigma.GraphicsMill.Unit.Point);

                    base.ZoomInternal = zoom;
                    UpdateBitmap();

                    renderingRectangle = GetViewportBounds();
                    System.Drawing.Point controlPoint = WorkspaceToControl(workspacePoint, Aurigma.GraphicsMill.Unit.Point);
                    Scroll(controlPoint.X - (renderingRectangle.Left + renderingRectangle.Width / 2), controlPoint.Y - (renderingRectangle.Top + renderingRectangle.Height / 2));
                }
                else
                {
                    base.ZoomInternal = zoom;
                }

                OnZoomed(System.EventArgs.Empty);
            }
        }

        #endregion Bitmap Handling

        #region "ICoordinateMapper interface implementation"

        public override System.Drawing.PointF ControlToWorkspace(System.Drawing.Point controlPoint, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            System.Drawing.Rectangle viewportBounds = GetViewportBounds();

            System.Drawing.PointF result = new System.Drawing.PointF();
            controlPoint.Offset(-viewportBounds.X, -viewportBounds.Y);

            result.X = _reverseZoom * (controlPoint.X + _scrollingPosition.X);
            result.Y = _reverseZoom * (controlPoint.Y + _scrollingPosition.Y);

            if (workspaceUnit == Aurigma.GraphicsMill.Unit.Pixel)
            {
                result.X *= _reverseHorizontalScale;
                result.Y *= _reverseVerticalScale;
            }
            else
            {
                result.X = ConvertPixelsToUnits(this.ViewerResolution, result.X, workspaceUnit);
                result.Y = ConvertPixelsToUnits(this.ViewerResolution, result.Y, workspaceUnit);
            }

            return result;
        }

        public override float GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            if (workspaceUnit == Aurigma.GraphicsMill.Unit.Pixel)
                return base.ZoomInternal * _horizontalScale;

            return base.GetControlPixelsPerUnitX(workspaceUnit);
        }

        public override float GetControlPixelsPerUnitY(Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            if (workspaceUnit == Aurigma.GraphicsMill.Unit.Pixel)
                return base.ZoomInternal * _verticalScale;

            return base.GetControlPixelsPerUnitY(workspaceUnit);
        }

        public override System.Drawing.Point WorkspaceToControl(System.Drawing.PointF workspacePoint, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            System.Drawing.Point result = new System.Drawing.Point();
            if (workspaceUnit == Aurigma.GraphicsMill.Unit.Pixel)
            {
                result.X = System.Convert.ToInt32(base.Zoom * _horizontalScale * workspacePoint.X);
                result.Y = System.Convert.ToInt32(base.Zoom * _verticalScale * workspacePoint.Y);
            }
            else
            {
                result.X = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToPixels(this.ViewerResolution, workspacePoint.X * base.Zoom, workspaceUnit);
                result.Y = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToPixels(this.ViewerResolution, workspacePoint.Y * base.Zoom, workspaceUnit);
            }

            System.Drawing.Rectangle viewportBounds = GetViewportBounds();
            result.Offset(viewportBounds.X, viewportBounds.Y);
            result.Offset(-_scrollingPosition.X, -_scrollingPosition.Y);

            return result;
        }

        private float ConvertPixelsToUnits(float resolution, float value, Aurigma.GraphicsMill.Unit unit)
        {
            if (resolution < eps)
                throw new System.ArgumentOutOfRangeException("resolution");

            if (unit == Aurigma.GraphicsMill.Unit.Pixel)
                return value;

            float inches = value / resolution;
            return Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(resolution, inches, Aurigma.GraphicsMill.Unit.Inch, unit);
        }

        #endregion "ICoordinateMapper interface implementation"

        #region "Window & bitmaps size handling"

        public override System.Drawing.Rectangle GetCanvasBounds()
        {
            System.Drawing.Rectangle ret = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);

            if (this.Width > 0 && this.Height > 0)
            {
                int scrollBarCx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);

                if (_verticalScrollBarShow)
                    ret.Width -= scrollBarCx;
                if (_horizontalScrollBarShow)
                    ret.Height -= scrollBarCx;

                if (base.BorderStyle != System.Windows.Forms.Border3DStyle.Flat)
                {
                    int cx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE);
                    int cy = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE);

                    ret.X += cx;
                    ret.Y += cy;

                    ret.Height -= 2 * cy;
                    ret.Width -= 2 * cx;
                }
            }

            return ret;
        }

        public System.Drawing.Rectangle GetCanvasBoundsWithBorder()
        {
            System.Drawing.Rectangle objRet = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);

            if (this.Width > 0 && this.Height > 0)
            {
                int intScrollBarCx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);

                if (_verticalScrollBarShow)
                    objRet.Width -= intScrollBarCx;
                if (_horizontalScrollBarShow)
                    objRet.Height -= intScrollBarCx;
            }

            return objRet;
        }

        public override System.Drawing.Rectangle GetViewportBounds()
        {
            System.Drawing.Rectangle objRet = GetCanvasOutputBounds();
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();
            objRet.Offset(canvasRectangle.X, canvasRectangle.Y);
            return objRet;
        }

        protected override void UpdateCanvas()
        {
            UpdateScrollBarsState();

            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

            if (_canvasBitmap != null)
            {
                _canvasBitmap.Dispose();
                _canvasBitmap = null;
            }

            if (canvasRectangle.Width > 0 && canvasRectangle.Height > 0)
            {
                _canvasBitmap = new Aurigma.GraphicsMill.Bitmap(canvasRectangle.Width, canvasRectangle.Height, PixelFormat.Format24bppRgb, this.BackColor);
                _canvasBitmap.ColorProfile = Aurigma.GraphicsMill.ColorProfile.FromScreen();
                _canvasBitmap.ColorManagement.ColorManagementEngine = ColorManagementEngine.None;
            }

            UpdateScrollBarsRange();
            UpdateBitmap();
        }

        private System.Drawing.Rectangle GetCanvasOutputBounds()
        {
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();
            System.Drawing.Rectangle result = new System.Drawing.Rectangle(0, 0, canvasRectangle.Width, canvasRectangle.Height);

            if (result.Width > _contentSize.Width || result.Height > _contentSize.Height)
            {
                int horizontalDelta = Math.Max(0, result.Width - _contentSize.Width);
                int verticalDelta = Math.Max(0, result.Height - _contentSize.Height);
                result.Width = Math.Min(result.Width, _contentSize.Width);
                result.Height = Math.Min(result.Height, _contentSize.Height);

                switch (_viewportAlignment)
                {
                    case ViewportAlignment.LeftTop:
                        break;

                    case ViewportAlignment.LeftCenter:
                        result.Offset(0, verticalDelta / 2);
                        break;

                    case ViewportAlignment.LeftBottom:
                        result.Offset(0, verticalDelta);
                        break;

                    case ViewportAlignment.CenterTop:
                        result.Offset(horizontalDelta / 2, 0);
                        break;

                    case ViewportAlignment.CenterCenter:
                        result.Offset(horizontalDelta / 2, verticalDelta / 2);
                        break;

                    case ViewportAlignment.CenterBottom:
                        result.Offset(horizontalDelta / 2, verticalDelta);
                        break;

                    case ViewportAlignment.RightTop:
                        result.Offset(horizontalDelta, 0);
                        break;

                    case ViewportAlignment.RightCenter:
                        result.Offset(horizontalDelta, verticalDelta / 2);
                        break;

                    case ViewportAlignment.RightBottom:
                        result.Offset(horizontalDelta, verticalDelta);
                        break;
                }
            }

            return result;
        }

        private System.Drawing.Rectangle GetControlBoundsWithoutBorder()
        {
            System.Drawing.Rectangle ret = new System.Drawing.Rectangle(0, 0, Width, Height);
            int cx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE);
            int cy = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE);

            if (base.BorderStyle != System.Windows.Forms.Border3DStyle.Flat)
            {
                ret.X += cx;
                ret.Y += cy;
                ret.Height -= 2 * cy;
                ret.Width -= 2 * cx;
            }

            return ret;
        }

        private void SetControlSize(System.Drawing.Size canvasSize)
        {
            int width = canvasSize.Width;
            int height = canvasSize.Height;
            int cx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE);
            int cy = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE);

            if (base.BorderStyle != System.Windows.Forms.Border3DStyle.Flat)
            {
                width += 2 * cx;
                height += 2 * cy;
            }

            this.Width = width;
            this.Height = height;
        }

        #endregion "Window & bitmaps size handling"

        #region Drawing Of Control Parts

        protected override Aurigma.GraphicsMill.Drawing.Graphics ControlGdiGraphics
        {
            get
            {
                return _canvasBitmap.GetGraphics();
            }
        }

        public override void InvalidateViewer()
        {
            UpdateScrollBars();
            base.Invalidate();
        }

        public override void InvalidateViewer(System.Drawing.Rectangle rectangle)
        {
            UpdateScrollBars();
            base.Invalidate(rectangle);
        }

        public override void InvalidateViewer(InvalidationTarget target)
        {
            if (target == null)
                throw new System.ArgumentNullException("target");

            if (target.Rectangle.IsEmpty)
                base.Invalidate();
            else
                base.Invalidate(target.Rectangle);
        }

        private void DrawBitmap(Aurigma.GraphicsMill.Drawing.Graphics graphics)
        {
            if (BitmapIsEmpty())
            {
                DrawControlBackground(graphics);
                return;
            }

            Aurigma.GraphicsMill.Transforms.CombineMode combineMode = Aurigma.GraphicsMill.Transforms.CombineMode.Copy;
            Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode interpolationMode;
            System.Drawing.Rectangle clientRectangle = GetCanvasOutputBounds();

            if (_alphaEnabled && this.Bitmap.HasAlpha)
                combineMode = Aurigma.GraphicsMill.Transforms.CombineMode.Alpha;

            if (_zoomQuality == ZoomQuality.ShrinkHighStretchLow)
            {
                if (base.ZoomInternal < 1.0)
                    interpolationMode = Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.High;
                else
                    interpolationMode = Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.Low;
            }
            else if (_zoomQuality == ZoomQuality.High)
            {
                interpolationMode = Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.High;
            }
            else if (_zoomQuality == ZoomQuality.Medium)
            {
                interpolationMode = Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.Medium;
            }
            else
            {
                interpolationMode = Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.Low;
            }

            float dx = base.ZoomInternal * _horizontalScale;
            float dy = base.ZoomInternal * _verticalScale;

            _resize.Width = clientRectangle.Width;
            _resize.Height = clientRectangle.Height;
            _resize.InterpolationMode = interpolationMode;

            DrawControlBackground(graphics);
            DrawBitmapBackground(graphics, clientRectangle, clientRectangle);

            var crop = new System.Drawing.Rectangle((int)(_scrollingPosition.X / dx), (int)(_scrollingPosition.Y / dy), (int)((float)clientRectangle.Width / dx), (int)((float)clientRectangle.Height / dy));

            if ((crop.X + crop.Width) > this.Bitmap.Width)
            {
                int shiftX = (crop.X + crop.Width) - this.Bitmap.Width;
                crop.X = Math.Max(0, crop.X - shiftX);
                crop.Width -= shiftX;
            }

            if ((crop.Y + crop.Height) > this.Bitmap.Height)
            {
                int shiftY = (crop.Y + crop.Height) - this.Bitmap.Height;
                crop.Y = Math.Max(0, crop.Y - shiftY);
                crop.Height -= shiftY;
            }

            using (var cropBitmap = Crop.Apply(this.Bitmap, crop))
            {
                cropBitmap.ApplyTransform(_resize);
                cropBitmap.ColorManagement.ColorManagementEngine = this.Bitmap.ColorManagement.ColorManagementEngine;
                cropBitmap.DrawOn(graphics.GetDC(), clientRectangle, combineMode, 1.0f, interpolationMode);
            }

            _scrollingShift.X = 0;
            _scrollingShift.Y = 0;
        }

        private void DrawBitmapBackground(Aurigma.GraphicsMill.Drawing.Graphics graphics, System.Drawing.Rectangle renderingRectangle, System.Drawing.Rectangle partRectangle)
        {
            if (_alphaEnabled && this.Bitmap != null && this.Bitmap.HasAlpha)
            {
                graphics.SetClip(partRectangle);

                Aurigma.GraphicsMill.Drawing.SolidBrush brush1 = new Aurigma.GraphicsMill.Drawing.SolidBrush(_workspaceBackColor1);
                Aurigma.GraphicsMill.Drawing.SolidBrush brush2 = new Aurigma.GraphicsMill.Drawing.SolidBrush(_workspaceBackColor2);

                try
                {
                    if (_viewportBackgroundStyle == WorkspaceBackgroundStyle.Grid)
                    {
                        renderingRectangle.Inflate(_scrollingPosition.X % (2 * backgroundGridCell), _scrollingPosition.Y % (2 * backgroundGridCell));

                        if (_canvasGrid == null || _canvasGrid.Width != this.Width + 4 * backgroundGridCell || _canvasGrid.Height != this.Height + 4 * backgroundGridCell)
                        {
                            if (_canvasGrid != null)
                                _canvasGrid.Dispose();

                            using (var tmpBitmap = new Aurigma.GraphicsMill.Bitmap(this.Width + backgroundGridCell * 6, backgroundGridCell * 2, Aurigma.GraphicsMill.PixelFormat.Format24bppRgb))
                            using (var tmpCanvas = tmpBitmap.GetGraphics())
                            {
                                tmpBitmap.Fill(_workspaceBackColor2);

                                for (int i = 0; i < tmpBitmap.Width; i += 2 * backgroundGridCell)
                                {
                                    tmpCanvas.FillRectangle(brush1, i, 0, backgroundGridCell, backgroundGridCell);
                                    tmpCanvas.FillRectangle(brush1, i + backgroundGridCell, backgroundGridCell, backgroundGridCell, backgroundGridCell);
                                }

                                _canvasGrid = new Aurigma.GraphicsMill.Bitmap(this.Width + 4 * backgroundGridCell, this.Height + 4 * backgroundGridCell, Aurigma.GraphicsMill.PixelFormat.Format24bppRgb);

                                using (var canvasGraphics = _canvasGrid.GetGraphics())
                                {
                                    for (int y = 0; y < _canvasGrid.Height; y += 2 * backgroundGridCell)
                                    {
                                        canvasGraphics.DrawImage(tmpBitmap, 0, y, CombineMode.Copy);
                                    }
                                }
                            }
                        }

                        using (var crop = new Crop(0, 0, renderingRectangle.Width, renderingRectangle.Height))
                        using (var croppedGrid = crop.Apply(_canvasGrid))
                        {
                            graphics.DrawImage(croppedGrid, renderingRectangle.Left, renderingRectangle.Top, CombineMode.Copy);
                        }
                    }
                    else if (_viewportBackgroundStyle == WorkspaceBackgroundStyle.Solid)
                    {
                        graphics.FillRectangle(brush1.ToGdiPlusBrush(), partRectangle);
                    }
                    else
                    {
                        graphics.FillRectangle(new System.Drawing.SolidBrush(BackColor), partRectangle);
                    }
                }
                finally
                {
                    graphics.ResetClip();
                }
            }
        }

        private void DrawControlBackground(Aurigma.GraphicsMill.Drawing.Graphics graphics)
        {
            graphics.Clear(this.BackColor);
        }

        private void DrawWorkspaceBorder(Aurigma.GraphicsMill.Drawing.Graphics graphics)
        {
            if (_workspaceBorderEnabled && WorkspaceBorderWidth > 0)
            {
                System.Drawing.Pen pen = new System.Drawing.Pen(_workspaceBorderColor, _viewportBorderWidth);
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

                System.Drawing.Rectangle bitmapRectangle = GetCanvasOutputBounds();
                bitmapRectangle = new System.Drawing.Rectangle(bitmapRectangle.X - _scrollingPosition.X, bitmapRectangle.Y - _scrollingPosition.Y, _contentSize.Width - 1, _contentSize.Height - 1);

                if (bitmapRectangle.Width > 0 || bitmapRectangle.Height > 0)
                {
                    bitmapRectangle.Inflate(_viewportBorderWidth, _viewportBorderWidth);
                    graphics.DrawRectangle(pen, bitmapRectangle);
                }
            }
        }

        private void UpdateBitmapColorManagement(Aurigma.GraphicsMill.Bitmap bitmap)
        {
            bitmap.ColorManagement.ColorManagementEngine = _colorManagementEngine;
        }

        #endregion Drawing Of Control Parts
    }
}