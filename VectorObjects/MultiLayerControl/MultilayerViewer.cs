// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System.ComponentModel;
using System.Security.Permissions;

namespace Aurigma.GraphicsMill
{
    public delegate void StateRestoringEventHandler(object sender, StateRestoringEventArgs e);

    public interface IStateNavigable
    {
        void ClearUndoHistory();

        void ClearRedoHistory();

        void ClearHistory();

        void Undo();

        void Undo(int steps);

        void Redo();

        void Redo(int steps);

        bool CanRedo
        {
            get;
        }

        int UndoStepCount
        {
            get;
        }

        void SaveState();

        int MaxUndoStepCount
        {
            get;
            set;
        }

        bool UndoRedoEnabled
        {
            get;
            set;
        }

        bool UndoRedoTrackingEnabled
        {
            get;
            set;
        }

        event StateRestoringEventHandler Undoing;

        event System.EventHandler Undone;

        event StateRestoringEventHandler Redoing;

        event System.EventHandler Redone;
    }

    public class StateRestoringEventArgs : System.EventArgs
    {
        public bool Cancel { get; set; }
    }
}

namespace Aurigma.GraphicsMill.WinControls
{
    public class MultiLayerViewerInvalidationTarget : InvalidationTarget
    {
        private Layer _layer;
        private bool _invalidateDesigner;

        public MultiLayerViewerInvalidationTarget(System.Drawing.Rectangle rectangle)
            : base(rectangle)
        {
            _invalidateDesigner = true;
            _layer = null;
        }

        public MultiLayerViewerInvalidationTarget(System.Drawing.Rectangle rectangle, Layer layer)
            : base(rectangle)
        {
            if (layer == null)
                throw new System.ArgumentNullException("layer");

            _invalidateDesigner = false;
            _layer = layer;
        }

        public bool InvalidateDesigner
        {
            get
            {
                return _invalidateDesigner;
            }
        }

        public Layer Layer
        {
            get
            {
                return _layer;
            }
        }
    }

    /// <summary>
    /// MultiLayerViewer control.
    /// </summary>
    [AdaptiveToolboxBitmapAttribute(typeof(ResourceFinder), "MultiLayerViewer.bmp")]
    public class MultiLayerViewer : ViewerBase, IVObjectHost, Aurigma.GraphicsMill.IStateNavigable
    {
        #region "Constants"

        private const int WheelDelta = 120;

        #region "Member variables"

        private VObjectHost _objectHost;
        private MultipleLayerRenderer _viewportRenderer;
        private bool _invalidateLayers;

        // Workspace dimensions are stored in points.
        private float _workspaceWidth;

        private float _workspaceHeight;
        private Aurigma.GraphicsMill.Unit _unit;

        private System.Drawing.Size _contentSize;
        private Aurigma.GraphicsMill.WinControls.ViewportAlignment _viewportAlignment;

        private Aurigma.GraphicsMill.Bitmap _viewportCanvas;

        private Aurigma.GraphicsMill.Bitmap _controlBitmap;
        private Aurigma.GraphicsMill.Drawing.Graphics _controlGdiGraphics;

        #region "Scrollbars handling variables"

        private Aurigma.GraphicsMill.WinControls.ScrollBarsStyle _scrollBarsStyle;
        private int _verticalSmallChange;
        private int _verticalLargeChange;
        private int _horizontalSmallChange;
        private int _horizontalLargeChange;
        private System.Drawing.Point _scrollingPosition;
        private bool _horizontalScrollBarShow;
        private bool _horizontalScrollBarEnabled;
        private bool _verticalScrollBarShow;
        private bool _verticalScrollBarEnabled;

        #endregion "Scrollbars handling variables"

        private System.Drawing.Point _prevMousePosition;

        private Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle _backgroundStyle;
        private System.Drawing.Color _workspaceBackgroundColor1;
        private System.Drawing.Color _workspaceBackgroundColor2;

        private bool _workspaceBorderEnabled;
        private int _workspaceBorderWidth;
        private System.Drawing.Color _workspaceBorderColor;

        #endregion "Member variables"

        #endregion "Constants"

        #region "Generated code"

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MultiLayerViewer()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            InitializeViewer();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }

                if (_viewportRenderer != null)
                {
                    _viewportRenderer.Dispose();
                    _viewportRenderer = null;
                }

                if (_objectHost != null)
                {
                    _objectHost.Dispose();
                    _objectHost = null;
                }

                if (_controlGdiGraphics != null)
                {
                    _controlGdiGraphics.Dispose();
                    _controlGdiGraphics = null;
                }

                if (_controlBitmap != null)
                {
                    _controlBitmap.Dispose();
                    _controlBitmap = null;
                }

                if (_viewportCanvas != null)
                {
                    _viewportCanvas.Dispose();
                    _viewportCanvas = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion Component Designer generated code

        #endregion "Generated code"

        #region "Events"

        //
        // Following layers events are duplicated from LayerCollection of the control -
        // to allow user to assign event handlers in VS form designer.
        //
        public event LayerEventHandler LayerAdded;

        public event LayerRemovedEventHandler LayerRemoved;

        public event LayerChangedEventHandler LayerChanged;

        //
        // IVObjectHost interface events
        //
        public event System.EventHandler CurrentLayerChanged;

        public event DesignerChangedEventHandler DesignerChanged;

        #region "Protected event firing methods"

        protected virtual void OnLayerChanged(LayerChangedEventArgs e)
        {
            if (LayerChanged != null)
                LayerChanged(this, e);
        }

        protected virtual void OnLayerAdded(LayerEventArgs e)
        {
            _viewportRenderer.SetLayers(_objectHost.Layers);

            if (LayerAdded != null)
                LayerAdded(this, e);
        }

        protected virtual void OnLayerRemoved(LayerRemovedEventArgs e)
        {
            _viewportRenderer.SetLayers(_objectHost.Layers);

            if (LayerRemoved != null)
                LayerRemoved(this, e);
        }

        protected virtual void OnDesignerChanged(DesignerChangedEventArgs e)
        {
            if (this.DesignerChanged != null)
                this.DesignerChanged(this, e);
        }

        protected virtual void OnCurrentLayerChanged(System.EventArgs e)
        {
            if (this.CurrentLayerChanged != null)
                this.CurrentLayerChanged(this, e);
        }

        #endregion "Protected event firing methods"

        #endregion "Events"

        #region "Initialization"

        private void InitializeViewer()
        {
            _objectHost = new VObjectHost(this);
            _viewportRenderer = new MultipleLayerRenderer(true, this.ViewerResolution);
            _viewportRenderer.SetLayers(_objectHost.Layers);

            RegisterInternalEventHandlers();

            this.Width = 200;
            this.Height = 200;
            this.BackColor = System.Drawing.Color.Orange;

            _workspaceWidth = 1024;
            _workspaceHeight = 768;
            _unit = Aurigma.GraphicsMill.Unit.Point;

            _viewportAlignment = Aurigma.GraphicsMill.WinControls.ViewportAlignment.CenterCenter;

            _scrollBarsStyle = Aurigma.GraphicsMill.WinControls.ScrollBarsStyle.Auto;
            _verticalSmallChange = 5;
            _verticalLargeChange = 25;
            _horizontalSmallChange = 5;
            _horizontalLargeChange = 25;
            _scrollingPosition = System.Drawing.Point.Empty;

            _prevMousePosition = new System.Drawing.Point(-1, -1);

            _backgroundStyle = Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle.Grid;
            _workspaceBackgroundColor1 = System.Drawing.Color.LightGray;
            _workspaceBackgroundColor2 = System.Drawing.Color.White;

            _workspaceBorderEnabled = true;
            _workspaceBorderColor = System.Drawing.Color.Blue;
            _workspaceBorderWidth = 2;

            UpdateContentSize();
            UpdateControlBitmap();
            UpdateViewportCanvas();
        }

        #region "Internal events tracking"

        private void RegisterInternalEventHandlers()
        {
            _objectHost.Layers.LayerChanged += new LayerChangedEventHandler(LayerChangedHandler);
            _objectHost.Layers.LayerAdded += new LayerEventHandler(LayerAddedHandler);
            _objectHost.Layers.LayerRemoved += new LayerRemovedEventHandler(LayerRemovedHandler);

            _objectHost.CurrentLayerChanged += new System.EventHandler(CurrentLayerChangedHandler);
            _objectHost.DesignerChanged += new DesignerChangedEventHandler(DesignerChangedHandler);

            _objectHost.Undoing += new StateRestoringEventHandler(UndoingEventHandler);
            _objectHost.Redoing += new StateRestoringEventHandler(RedoingEventHandler);
            _objectHost.Undone += new System.EventHandler(UndoneEventHandler);
            _objectHost.Redone += new System.EventHandler(RedoneEventHandler);
        }

        private void LayerChangedHandler(object sender, LayerChangedEventArgs e)
        {
            OnWorkspaceChanged(System.EventArgs.Empty);
            OnLayerChanged(e);
        }

        private void LayerAddedHandler(object sender, LayerEventArgs e)
        {
            OnWorkspaceChanged(System.EventArgs.Empty);
            OnLayerAdded(e);
        }

        private void LayerRemovedHandler(object sender, LayerRemovedEventArgs e)
        {
            OnLayerRemoved(e);
        }

        private void CurrentLayerChangedHandler(object sender, System.EventArgs e)
        {
            OnCurrentLayerChanged(e);
        }

        private void DesignerChangedHandler(object sender, DesignerChangedEventArgs e)
        {
            OnDesignerChanged(e);
        }

        #endregion "Internal events tracking"

        protected override void OnHandleCreated(System.EventArgs e)
        {
            base.OnHandleCreated(e);

            NativeMethods.SCROLLINFO scrollInfo = new NativeMethods.SCROLLINFO(NativeMethods.SIF_RANGE | NativeMethods.SIF_POS, 0, 100, 10, 10);
            NativeMethods.SetScrollInfo(this.Handle, NativeMethods.SB_BOTH, ref scrollInfo, true);
        }

        #endregion "Initialization"

        #region "Public properties"

        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle), "Grid")]
        [ResDescription("MultiLayerViewer_WorkspaceBackgroundStyle")]
        public Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle WorkspaceBackgroundStyle
        {
            get
            {
                return _backgroundStyle;
            }

            set
            {
                _backgroundStyle = value;
                InvalidateViewer();
            }
        }

        [DefaultValue(typeof(System.Drawing.Color), "LightGray")]
        [ResDescription("MultiLayerViewer_WorkspaceBackColor1")]
        public System.Drawing.Color WorkspaceBackColor1
        {
            get
            {
                return _workspaceBackgroundColor1;
            }

            set
            {
                _workspaceBackgroundColor1 = value;
                InvalidateViewer();
            }
        }

        [DefaultValue(typeof(System.Drawing.Color), "White")]
        [ResDescription("MultiLayerViewer_WorkspaceBackColor2")]
        public System.Drawing.Color WorkspaceBackColor2
        {
            get
            {
                return _workspaceBackgroundColor2;
            }

            set
            {
                _workspaceBackgroundColor2 = value;
                InvalidateViewer();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("MultiLayerViewer_WorkspaceBorderEnabled")]
        public override bool WorkspaceBorderEnabled
        {
            get
            {
                return _workspaceBorderEnabled;
            }

            set
            {
                _workspaceBorderEnabled = value;
                InvalidateViewer();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [DefaultValue(typeof(System.Drawing.Color), "Blue")]
        [ResDescription("MultiLayerViewer_WorkspaceBorderColor")]
        public override System.Drawing.Color WorkspaceBorderColor
        {
            get
            {
                return _workspaceBorderColor;
            }

            set
            {
                _workspaceBorderColor = value;
                InvalidateViewer();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(2)]
        [ResDescription("MultiLayerViewer_WorkspaceBorderWidth")]
        public override int WorkspaceBorderWidth
        {
            get
            {
                return _workspaceBorderWidth;
            }

            set
            {
                if (value < 0)
                    throw new System.ArgumentOutOfRangeException("value", StringResources.GetString("ExStrValueShouldBeAboveZero"));

                _workspaceBorderWidth = value;
                InvalidateViewer();
            }
        }

        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.ScrollBarsStyle), "Auto")]
        [ResDescription("MultiLayerViewer_ScrollBarsStyle")]
        public override Aurigma.GraphicsMill.WinControls.ScrollBarsStyle ScrollBarsStyle
        {
            get
            {
                return _scrollBarsStyle;
            }

            set
            {
                _scrollBarsStyle = value;
                InvalidateViewer();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.ViewportAlignment), "CenterCenter")]
        [ResDescription("MultiLayerViewer_ViewportAlignment")]
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

        #region "Aliases for designer properties"

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("MultiLayerViewer_ResizeProportionallyWithShift")]
        public bool ResizeProportionallyWithShift
        {
            get
            {
                if (!_objectHost.DesignerOptions.ContainsKey(DesignerSettingsConstants.ResizeProportionallyWithShift))
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotFindDesignerOptionsKey"));

                return (bool)_objectHost.DesignerOptions[DesignerSettingsConstants.ResizeProportionallyWithShift];
            }
            set
            {
                _objectHost.DesignerOptions[DesignerSettingsConstants.ResizeProportionallyWithShift] = value;
                _objectHost.CurrentDesigner.UpdateSettings();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("MultiLayerViewer_MultiSelect")]
        public bool MultiSelect
        {
            get
            {
                if (!_objectHost.DesignerOptions.ContainsKey(DesignerSettingsConstants.MultiSelect))
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotFindDesignerOptionsKey"));

                return (bool)_objectHost.DesignerOptions[DesignerSettingsConstants.MultiSelect];
            }
            set
            {
                _objectHost.DesignerOptions[DesignerSettingsConstants.MultiSelect] = value;
                _objectHost.CurrentDesigner.UpdateSettings();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(false)]
        [ResDescription("MultiLayerViewer_DragOnlyMultipleObjects")]
        public bool MultipleVObjectsTransformationEnabled
        {
            get
            {
                if (!_objectHost.DesignerOptions.ContainsKey(DesignerSettingsConstants.MultipleVObjectsTransformationEnabled))
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotFindDesignerOptionsKey"));

                return (bool)_objectHost.DesignerOptions[DesignerSettingsConstants.MultipleVObjectsTransformationEnabled];
            }
            set
            {
                _objectHost.DesignerOptions[DesignerSettingsConstants.MultipleVObjectsTransformationEnabled] = value;
                _objectHost.CurrentDesigner.UpdateSettings();
            }
        }

        #endregion "Aliases for designer properties"

        #endregion "Public properties"

        #region "Message processing"

        [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case (int)NativeMethods.WM_HSCROLL:
                    {
                        if (m.LParam != System.IntPtr.Zero)
                        {
                            base.WndProc(ref m);
                            break;
                        }

                        HorizontalScrollHandler((uint)(m.WParam.ToInt32() & 0xFFFF), m.WParam.ToInt32() >> 16 & 0xFFFF);
                        break;
                    }

                case (int)NativeMethods.WM_VSCROLL:
                    {
                        if (m.LParam != System.IntPtr.Zero)
                        {
                            base.WndProc(ref m);
                            break;
                        }

                        VerticalScrollHandler((uint)(m.WParam.ToInt32() & 0xFFFF), m.WParam.ToInt32() >> 16 & 0xFFFF);
                        break;
                    }

                case (int)NativeMethods.WM_MOUSEWHEEL:
                    {
                        MouseWheelHandler((int)m.WParam.ToInt64() >> 16, (int)m.WParam.ToInt64() & 0xFFFF);
                        base.WndProc(ref m);
                        break;
                    }

                case (int)NativeMethods.WM_NCCREATE:
                    {
                        NativeMethods.CREATESTRUCT structure = (NativeMethods.CREATESTRUCT)System.Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.CREATESTRUCT));

                        structure.style |= NativeMethods.WS_HSCROLL | NativeMethods.WS_VSCROLL;
                        System.Runtime.InteropServices.Marshal.StructureToPtr(structure, m.LParam, true);
                        base.WndProc(ref m);
                        break;
                    }

                case (int)NativeMethods.WM_CREATE:
                    {
                        NativeMethods.INITCOMMONCONTROLSEX controls = new NativeMethods.INITCOMMONCONTROLSEX(NativeMethods.ICC_PAGESCROLLER_CLASS | NativeMethods.ICC_STANDARD_CLASSES | NativeMethods.ICC_LINK_CLASS);

                        NativeMethods.InitCommonControlsEx(ref controls);
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

        protected override void OnSizeChanged(System.EventArgs e)
        {
            if (this.Width > 0 && this.Height > 0)
            {
                UpdateControlBitmap();
                UpdateViewportCanvas();
                InvalidateViewer();
            }

            UpdateZoom(base.ZoomInternal);

            base.OnSizeChanged(e);
        }

        private void MouseWheelHandler(int amount, int buttonsState)
        {
            int delta = amount / WheelDelta;

            if (buttonsState == 0)
                Scroll(0, -delta * _verticalSmallChange);
            else if (buttonsState == NativeMethods.MK_CONTROL)
                Scroll(-delta * _horizontalSmallChange, 0);
            else if (buttonsState == NativeMethods.MK_SHIFT)
            {
                if (delta < 0)
                    ApplyZoom(base.WheelZoomAmount * -delta);
                else if (delta > 0)
                    ApplyZoom(1.0f / (base.WheelZoomAmount * delta));
            }
        }

        private bool IsDesignerActive
        {
            get
            {
                return base.Navigator == null && base.Rubberband == null && this.CurrentDesigner != null;
            }
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (e.X == _prevMousePosition.X && e.Y == _prevMousePosition.Y)
                return;
            _prevMousePosition = new System.Drawing.Point(e.X, e.Y);

            base.OnMouseMove(e);

            System.Drawing.Point clickPoint = new System.Drawing.Point(e.X, e.Y);
            if (this.GetViewportBounds().Contains(clickPoint))
                OnWorkspaceMouseMove(new Aurigma.GraphicsMill.WinControls.MouseEventArgs(e.Button, e.Clicks, ControlToWorkspace(clickPoint, _unit)));

            if (this.IsDesignerActive)
            {
                if (this.CurrentDesigner.NotifyMouseMove(e))
                    return;

                this.CurrentDesigner = _objectHost.DefaultDesigner;
                this.CurrentDesigner.NotifyMouseMove(e);
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            base.OnMouseUp(e);

            System.Drawing.Point clickPoint = new System.Drawing.Point(e.X, e.Y);
            if (this.GetViewportBounds().Contains(clickPoint))
                OnWorkspaceMouseUp(new Aurigma.GraphicsMill.WinControls.MouseEventArgs(e.Button, e.Clicks, ControlToWorkspace(clickPoint, _unit)));

            if (this.IsDesignerActive)
            {
                if (this.CurrentDesigner.NotifyMouseUp(e))
                    return;

                this.CurrentDesigner = _objectHost.DefaultDesigner;
                this.CurrentDesigner.NotifyMouseUp(e);
            }
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            this.Focus();

            System.Drawing.Point clickPoint = new System.Drawing.Point(e.X, e.Y);
            if (this.GetViewportBounds().Contains(clickPoint))
                OnWorkspaceMouseDown(new Aurigma.GraphicsMill.WinControls.MouseEventArgs(e.Button, e.Clicks, ControlToWorkspace(clickPoint, _unit)));

            if (this.IsDesignerActive)
            {
                if (this.CurrentDesigner.NotifyMouseDown(e))
                    return;

                this.CurrentDesigner = _objectHost.DefaultDesigner;
                this.CurrentDesigner.NotifyMouseDown(e);
            }

            base.OnMouseDown(e);
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            base.OnKeyUp(e);

            if (this.IsDesignerActive)
            {
                if (this.CurrentDesigner.NotifyKeyUp(e))
                    return;

                this.CurrentDesigner = _objectHost.DefaultDesigner;
                this.CurrentDesigner.NotifyKeyUp(e);
            }
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (this.IsDesignerActive)
            {
                if (this.CurrentDesigner.NotifyKeyDown(e))
                    return;

                this.CurrentDesigner = _objectHost.DefaultDesigner;
                this.CurrentDesigner.NotifyKeyDown(e);
            }
        }

        #endregion "Message processing"

        #region "Drawing functionality implementation"

        protected override Aurigma.GraphicsMill.Drawing.Graphics ControlGdiGraphics
        {
            get
            {
                return _controlGdiGraphics;
            }
        }

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
        {
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                UpdateScrollBars();

                System.IntPtr hdc = e.Graphics.GetHdc();

                DrawBackground(e.ClipRectangle);
                DrawWorkspaceBorder();
                DrawControlBorder(hdc);
                DrawLayers(e.ClipRectangle);
                DrawDesigner();

                using (System.Drawing.Graphics g = _controlBitmap.GetGdiPlusGraphics())
                    OnDoubleBufferPaint(new System.Windows.Forms.PaintEventArgs(g, e.ClipRectangle));

                e.Graphics.ReleaseHdc(hdc);

                using (var ct = new Aurigma.GraphicsMill.Transforms.Crop(e.ClipRectangle))
                using (var clip = ct.Apply(_controlBitmap))
                {
                    clip.DrawOn(e.Graphics, e.ClipRectangle.Left, e.ClipRectangle.Top, Aurigma.GraphicsMill.Transforms.CombineMode.Copy);
                }
            }
            finally
            {
                base.OnPaint(e);
            }
        }

        private void UpdateControlBitmap()
        {
            if (_controlGdiGraphics != null)
            {
                _controlGdiGraphics.Dispose();
                _controlGdiGraphics = null;
            }

            _controlBitmap = new Aurigma.GraphicsMill.Bitmap(this.Width, this.Height, Aurigma.GraphicsMill.PixelFormat.Format24bppRgb);

            _controlGdiGraphics = _controlBitmap.GetGraphics();
        }

        private void UpdateViewportCanvas()
        {
            if (_workspaceWidth > 0 && _workspaceHeight > 0)
            {
                System.Drawing.Rectangle renderingCanvasRect = GetViewportBounds();

                if (renderingCanvasRect.Width < 1 || renderingCanvasRect.Height < 1)
                    return;

                if (_viewportCanvas == null || renderingCanvasRect.Width != _viewportCanvas.Width || renderingCanvasRect.Height != _viewportCanvas.Height)
                {
                    _viewportCanvas = new Aurigma.GraphicsMill.Bitmap(renderingCanvasRect.Width, renderingCanvasRect.Height, Aurigma.GraphicsMill.PixelFormat.Format24bppRgb);
                }
            }
        }

        private void DrawBackground(System.Drawing.Rectangle invalidationRectangle)
        {
            if (invalidationRectangle.IsEmpty)
                _controlGdiGraphics.FillRectangle(new Aurigma.GraphicsMill.Drawing.SolidBrush(this.BackColor), 0, 0, this.Width, this.Height);
            else
                _controlGdiGraphics.FillRectangle(new Aurigma.GraphicsMill.Drawing.SolidBrush(this.BackColor), invalidationRectangle);
        }

        private void DrawWorkspaceBorder()
        {
            if (_workspaceBorderEnabled)
            {
                System.Drawing.Pen pen = new System.Drawing.Pen(_workspaceBorderColor, _workspaceBorderWidth);
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

                System.Drawing.Rectangle viewportBorder = this.GetViewportBounds();
                if (viewportBorder.Width > 0 || viewportBorder.Height > 0)
                {
                    viewportBorder.Inflate(_workspaceBorderWidth, _workspaceBorderWidth);
                    _controlGdiGraphics.DrawRectangle(pen, viewportBorder);
                }
            }
        }

        private void DrawLayers(System.Drawing.Rectangle invalidationRectangle)
        {
            if (_contentSize.Width < 1 || _contentSize.Height < 1)
                return;

            System.Drawing.Rectangle renderingCanvasRect = GetViewportBounds();
            invalidationRectangle.Intersect(renderingCanvasRect);

            // Now invalidationRectangle stores part of the canvasRenderingRectangle which should be updated.
            System.Drawing.Rectangle viewport = renderingCanvasRect;
            viewport.X = _scrollingPosition.X;
            viewport.Y = _scrollingPosition.Y;

            System.Drawing.Rectangle renderingRegion, canvasSourceRectangle;
            if (invalidationRectangle.Width < 1 || invalidationRectangle.Height < 1 || invalidationRectangle.Equals(renderingCanvasRect))
            {
                // we need to redraw the whole canvasRenderingRectangle
                renderingRegion = viewport;
                invalidationRectangle = renderingCanvasRect;
                canvasSourceRectangle = viewport;
                canvasSourceRectangle.Location = System.Drawing.Point.Empty;
            }
            else
            {
                renderingRegion = invalidationRectangle;

                renderingRegion.Offset(-renderingCanvasRect.X, -renderingCanvasRect.Y);
                canvasSourceRectangle = renderingRegion;
                renderingRegion.Offset(viewport.Location);
            }

            _viewportRenderer.BackColor = this.BackColor;
            _viewportRenderer.WorkspaceBackgroundStyle = this.WorkspaceBackgroundStyle;
            _viewportRenderer.WorkspaceBackColor1 = this.WorkspaceBackColor1;
            _viewportRenderer.WorkspaceBackColor2 = this.WorkspaceBackColor2;

            _viewportRenderer.Render(_viewportCanvas, base.ZoomInternal, viewport, renderingRegion);

            using (var ct = new Aurigma.GraphicsMill.Transforms.Crop(canvasSourceRectangle))
            using (var cropResult = ct.Apply(_viewportCanvas))
            {
                _controlGdiGraphics.DrawImage(cropResult, invalidationRectangle.Left, invalidationRectangle.Top, Aurigma.GraphicsMill.Transforms.CombineMode.Copy);
            }
        }

        private void DrawDesigner()
        {
            if (_objectHost.CurrentDesigner != null)
            {
                using (System.Drawing.Graphics g = _controlBitmap.GetGdiPlusGraphics())
                {
                    _objectHost.CurrentDesigner.Draw(g);
                }
            }
        }

        public override void InvalidateViewer()
        {
            base.Invalidate();
        }

        public override void InvalidateViewer(System.Drawing.Rectangle rectangle)
        {
            base.Invalidate(rectangle);
        }

        public override void InvalidateViewer(InvalidationTarget target)
        {
            if (target == null)
                throw new System.ArgumentNullException("target");

            MultiLayerViewerInvalidationTarget castedTarget = (MultiLayerViewerInvalidationTarget)target;
            if (castedTarget.InvalidateDesigner)
                InvalidateDesigner(castedTarget.Rectangle);
            else if (castedTarget.Layer != null)
                InvalidateLayer(castedTarget.Layer, castedTarget.Rectangle);
        }

        #endregion "Drawing functionality implementation"

        #region "Methods for rendering workspace to bitmap"

        public Aurigma.GraphicsMill.Bitmap RenderWorkspace()
        {
            return RenderWorkspace(this.ViewerResolution);
        }

        public Aurigma.GraphicsMill.Bitmap RenderWorkspace(float renderingResolution)
        {
            return _objectHost.RenderWorkspace(renderingResolution);
        }

        #endregion "Methods for rendering workspace to bitmap"

        #region "Scrolling functionality implementation"

        private void UpdateScrollBars()
        {
            UpdateScrollBarsRange();
            UpdateScrollBarsState();
        }

        private void UpdateScrollBarsState()
        {
            if (!IsHandleCreated)
                return;

            _horizontalScrollBarShow = true;
            _horizontalScrollBarEnabled = true;
            _verticalScrollBarShow = true;
            _verticalScrollBarEnabled = true;

            System.Drawing.Rectangle canvasTotalRectangle = GetControlBoundsWithoutBorder();
            if (_scrollBarsStyle == ScrollBarsStyle.Auto)
            {
                _horizontalScrollBarShow = _contentSize.Width > canvasTotalRectangle.Width;
                _verticalScrollBarShow = _contentSize.Height > canvasTotalRectangle.Height;

                System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();
                if (!_horizontalScrollBarShow && canvasRectangle.Width < _contentSize.Width)
                    _horizontalScrollBarShow = true;

                canvasRectangle = GetCanvasBounds();
                if (!_verticalScrollBarShow && canvasRectangle.Height < _contentSize.Height)
                    _verticalScrollBarShow = true;
            }
            else if (_scrollBarsStyle == ScrollBarsStyle.Always)
            {
                _horizontalScrollBarEnabled = _contentSize.Width > canvasTotalRectangle.Width;
                _verticalScrollBarEnabled = _contentSize.Height > canvasTotalRectangle.Height;
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
                    NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_VERT, true);
                else
                    NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_VERT, false);

                if (_horizontalScrollBarShow)
                    NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_HORZ, true);
                else
                    NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_HORZ, false);
            }
        }

        private void UpdateScrollBarsRange()
        {
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

            _scrollingPosition.X = System.Math.Max(0, System.Math.Min(_scrollingPosition.X, _contentSize.Width - canvasRectangle.Width));
            if (_horizontalScrollBarShow)
            {
                if (System.Math.Max(0, _contentSize.Width - canvasRectangle.Width) > 0)
                {
                    NativeMethods.SCROLLINFO horizontalScroll = new NativeMethods.SCROLLINFO(NativeMethods.SIF_RANGE | NativeMethods.SIF_POS | NativeMethods.SIF_PAGE, 0, _contentSize.Width, canvasRectangle.Width, _scrollingPosition.X);
                    NativeMethods.SetScrollInfo(Handle, NativeMethods.SB_HORZ, ref horizontalScroll, true);
                }
                NativeMethods.EnableScrollBar(Handle, NativeMethods.SB_HORZ, _horizontalScrollBarEnabled ? NativeMethods.ESB_ENABLE_BOTH : NativeMethods.ESB_DISABLE_BOTH);
            }

            _scrollingPosition.Y = System.Math.Max(0, System.Math.Min(_scrollingPosition.Y, _contentSize.Height - canvasRectangle.Height));
            if (_verticalScrollBarShow)
            {
                if (System.Math.Max(0, _contentSize.Height - canvasRectangle.Height) > 0)
                {
                    NativeMethods.SCROLLINFO verticalScroll = new NativeMethods.SCROLLINFO(NativeMethods.SIF_RANGE | NativeMethods.SIF_POS | NativeMethods.SIF_PAGE, 0, _contentSize.Height, canvasRectangle.Height, _scrollingPosition.Y);
                    NativeMethods.SetScrollInfo(Handle, NativeMethods.SB_VERT, ref verticalScroll, true);
                }
                NativeMethods.EnableScrollBar(Handle, NativeMethods.SB_VERT, _verticalScrollBarEnabled ? NativeMethods.ESB_ENABLE_BOTH : NativeMethods.ESB_DISABLE_BOTH);
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

                    Scroll(position - _scrollingPosition.X, 0);
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

        private void VerticalScrollHandler(uint cmd, int position)
        {
            switch (cmd)
            {
                case NativeMethods.SB_THUMBTRACK:
                case NativeMethods.SB_THUMBPOSITION:

                    NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO(NativeMethods.SIF_TRACKPOS, 0, 0, 0, 0);
                    if (NativeMethods.GetScrollInfo(Handle, NativeMethods.SB_VERT, ref si))
                        position = si.nTrackPos;

                    Scroll(0, position - _scrollingPosition.Y);
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

        public override void Scroll(int horizontalDelta, int verticalDelta)
        {
            bool needToInvalidate = false;
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

            if (horizontalDelta != 0 && (base.ZoomMode == Aurigma.GraphicsMill.WinControls.ZoomMode.None || base.ZoomMode == Aurigma.GraphicsMill.WinControls.ZoomMode.FitToHeight))
            {
                if (horizontalDelta > 0)
                    horizontalDelta = System.Math.Max(0, System.Math.Min(horizontalDelta, _contentSize.Width - _scrollingPosition.X - canvasRectangle.Width));
                else if (horizontalDelta < 0)
                    horizontalDelta = System.Math.Max(horizontalDelta, -_scrollingPosition.X);

                _scrollingPosition.X += horizontalDelta;
                needToInvalidate = true;
            }

            if (verticalDelta != 0 && (base.ZoomMode == Aurigma.GraphicsMill.WinControls.ZoomMode.None || base.ZoomMode == Aurigma.GraphicsMill.WinControls.ZoomMode.FitToWidth))
            {
                if (verticalDelta > 0)
                    verticalDelta = System.Math.Max(0, System.Math.Min(verticalDelta, _contentSize.Height - _scrollingPosition.Y - canvasRectangle.Height));
                else if (verticalDelta < 0)
                    verticalDelta = System.Math.Max(verticalDelta, -_scrollingPosition.Y);

                _scrollingPosition.Y += verticalDelta;
                needToInvalidate = true;
            }

            if (needToInvalidate)
            {
                OnScrolled(System.EventArgs.Empty);
                InvalidateViewer(GetViewportBounds());
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

        private void ScrollStep(bool scrollVertically, bool scrollForward)
        {
            if (scrollVertically)
                Scroll(0, scrollForward ? _verticalSmallChange : -_verticalSmallChange);
            else
                Scroll(scrollForward ? _horizontalSmallChange : -_horizontalSmallChange, 0);
        }

        private void ScrollPage(bool scrollVertically, bool scrollForward)
        {
            if (scrollVertically)
                Scroll(0, scrollForward ? _verticalLargeChange : -_verticalLargeChange);
            else
                Scroll(scrollForward ? _horizontalLargeChange : -_horizontalLargeChange, 0);
        }

        private void ScrollToBeginOrEnd(bool scrollVertically, bool scrollForward)
        {
            System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

            if (scrollVertically)
                Scroll(0, scrollForward ? _contentSize.Height - _scrollingPosition.Y - canvasRectangle.Height : -_scrollingPosition.Y);
            else
                Scroll(scrollForward ? _contentSize.Width - _scrollingPosition.X - canvasRectangle.Width : -_scrollingPosition.X, 0);
        }

        #endregion "Scrolling functionality implementation"

        #region "Zoom-related routines"

        protected override void UpdateZoom(float newZoom)
        {
            float zoom = System.Math.Min(System.Math.Max(newZoom, base.MinZoom), base.MaxZoom);

            System.Drawing.Rectangle renderingRectangle = this.GetViewportBounds();
            System.Drawing.PointF workspacePoint = ControlToWorkspace(new System.Drawing.Point(renderingRectangle.Left + renderingRectangle.Width / 2, renderingRectangle.Top + renderingRectangle.Height / 2), Aurigma.GraphicsMill.Unit.Point);

            if (base.ZoomMode == Aurigma.GraphicsMill.WinControls.ZoomMode.None)
                base.ZoomInternal = zoom;
            else
                base.ZoomInternal = CalculateZoomFromZoomMode();

            UpdateContentSize();
            UpdateViewportCanvas();

            renderingRectangle = GetViewportBounds();
            System.Drawing.Point controlPoint = WorkspaceToControl(workspacePoint, Aurigma.GraphicsMill.Unit.Point);
            Scroll(controlPoint.X - (renderingRectangle.Left + renderingRectangle.Width / 2), controlPoint.Y - (renderingRectangle.Top + renderingRectangle.Height / 2));

            InvalidateViewer();

            OnZoomed(System.EventArgs.Empty);
        }

        private float CalculateZoomFromZoomMode()
        {
            System.Drawing.Rectangle canvasTotalRect = GetControlBoundsWithoutBorder();

            int scrollBarCx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);

            System.Drawing.SizeF workspaceSize = GetWorkspacePixelSize();

            float result;
            float zoomWithoutScrollbarsX = canvasTotalRect.Width / workspaceSize.Width;
            float zoomWithoutScrollbarsY = canvasTotalRect.Height / workspaceSize.Height;

            float zoomX = (float)(canvasTotalRect.Width - scrollBarCx + (BorderStyle != System.Windows.Forms.Border3DStyle.Flat ? NativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE) : 0)) / workspaceSize.Width;
            float zoomY = (float)(canvasTotalRect.Height - scrollBarCx + (BorderStyle != System.Windows.Forms.Border3DStyle.Flat ? NativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE) : 0)) / workspaceSize.Height;

            switch (base.ZoomMode)
            {
                case Aurigma.GraphicsMill.WinControls.ZoomMode.BestFit:
                    result = System.Math.Min(zoomWithoutScrollbarsX, zoomWithoutScrollbarsY);
                    break;

                case Aurigma.GraphicsMill.WinControls.ZoomMode.BestFitShrinkOnly:
                    result = System.Math.Min(1.0f, System.Math.Min(zoomWithoutScrollbarsX, zoomWithoutScrollbarsY));
                    break;

                case Aurigma.GraphicsMill.WinControls.ZoomMode.FitToHeight:
                    if (zoomWithoutScrollbarsY * workspaceSize.Width < canvasTotalRect.Width)
                        result = zoomWithoutScrollbarsY;
                    else
                        result = zoomY;
                    break;

                case Aurigma.GraphicsMill.WinControls.ZoomMode.FitToHeightShrinkOnly:
                    if (zoomWithoutScrollbarsY * workspaceSize.Width < canvasTotalRect.Width)
                        result = zoomWithoutScrollbarsY;
                    else
                        result = zoomY;
                    result = System.Math.Min(1.0f, result);
                    break;

                case Aurigma.GraphicsMill.WinControls.ZoomMode.FitToWidth:
                    if (zoomWithoutScrollbarsX * workspaceSize.Height < canvasTotalRect.Height)
                        result = zoomWithoutScrollbarsX;
                    else
                        result = zoomX;
                    break;

                case Aurigma.GraphicsMill.WinControls.ZoomMode.FitToWidthShrinkOnly:
                    if (zoomWithoutScrollbarsX * workspaceSize.Height < canvasTotalRect.Height)
                        result = zoomWithoutScrollbarsX;
                    else
                        result = zoomX;
                    result = System.Math.Min(1.0f, result);
                    break;

                case Aurigma.GraphicsMill.WinControls.ZoomMode.ZoomControl:
                    throw new System.NotImplementedException();

                case Aurigma.GraphicsMill.WinControls.ZoomMode.None:
                    result = base.ZoomInternal;
                    break;

                default:
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUnexpectedZoomMode"));
            }

            return result;
        }

        #endregion "Zoom-related routines"

        #region "Dimension & coordinate handling"

        private System.Drawing.SizeF GetWorkspacePixelSize()
        {
            float width = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(base.ViewerResolution, _workspaceWidth, Aurigma.GraphicsMill.Unit.Point, Aurigma.GraphicsMill.Unit.Pixel);
            float height = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(base.ViewerResolution, _workspaceHeight, Aurigma.GraphicsMill.Unit.Point, Aurigma.GraphicsMill.Unit.Pixel);

            return new System.Drawing.SizeF(width, height);
        }

        private void UpdateContentSize()
        {
            System.Drawing.SizeF workspaceSize = GetWorkspacePixelSize();
            _contentSize = new System.Drawing.Size((int)(workspaceSize.Width * base.ZoomInternal), (int)(workspaceSize.Height * base.ZoomInternal));
        }

        private System.Drawing.Rectangle GetControlBoundsWithoutBorder()
        {
            System.Drawing.Rectangle result = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);
            int cx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE);
            int cy = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE);

            if (BorderStyle != System.Windows.Forms.Border3DStyle.Flat)
                result.Inflate(-cx, -cy);

            return result;
        }

        #endregion "Dimension & coordinate handling"

        #region "IVObjectHost Members"

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public IDesigner CurrentDesigner
        {
            get
            {
                return _objectHost.CurrentDesigner;
            }
            set
            {
                _objectHost.CurrentDesigner = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public IDesigner DefaultDesigner
        {
            get
            {
                return _objectHost.DefaultDesigner;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public System.Collections.Hashtable DesignerOptions
        {
            get
            {
                return _objectHost.DesignerOptions;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public ViewerBase HostViewer
        {
            get
            {
                return this;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public LayerCollection Layers
        {
            get
            {
                return _objectHost.Layers;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Layer CurrentLayer
        {
            get
            {
                return _objectHost.CurrentLayer;
            }
            set
            {
                _objectHost.CurrentLayer = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int CurrentLayerIndex
        {
            get
            {
                return _objectHost.CurrentLayerIndex;
            }
            set
            {
                _objectHost.CurrentLayerIndex = value;
            }
        }

        #endregion "IVObjectHost Members"

        #region ViewerBase Members

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(typeof(Aurigma.GraphicsMill.Unit), "Point")]
        [ResDescription("MultiLayerViewer_Unit")]
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

        [System.ComponentModel.Browsable(false)]
        public override float WorkspaceWidth
        {
            get
            {
                return Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(base.ViewerResolution, _workspaceWidth, Aurigma.GraphicsMill.Unit.Point, _unit);
            }
            set
            {
                float valueInPoints = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(base.ViewerResolution, value, _unit, Aurigma.GraphicsMill.Unit.Point);
                if (valueInPoints < 0)
                    throw new System.ArgumentOutOfRangeException("value", StringResources.GetString("ExStrValueShouldBeAboveZero"));

                _workspaceWidth = valueInPoints;

                UpdateContentSize();
                UpdateControlBitmap();
                UpdateViewportCanvas();
                OnWorkspaceChanged(System.EventArgs.Empty);

                this.InvalidateViewer();
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override float WorkspaceHeight
        {
            get
            {
                return Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(base.ViewerResolution, _workspaceHeight, Aurigma.GraphicsMill.Unit.Point, _unit);
            }
            set
            {
                float valueInPoints = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(base.ViewerResolution, value, _unit, Aurigma.GraphicsMill.Unit.Point);
                if (valueInPoints < 0)
                    throw new System.ArgumentOutOfRangeException("value", StringResources.GetString("ExStrValueShouldBeAboveZero"));

                _workspaceHeight = valueInPoints;

                UpdateContentSize();
                UpdateControlBitmap();
                UpdateViewportCanvas();
                OnWorkspaceChanged(System.EventArgs.Empty);

                this.InvalidateViewer();
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public override bool HasContent
        {
            get
            {
                return _objectHost.Layers.Count > 0;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Point ScrollingPosition
        {
            get
            {
                return _scrollingPosition;
            }
            set
            {
                System.Drawing.Rectangle canvasRectangle = GetCanvasBounds();

                _scrollingPosition.X = System.Math.Max(0, System.Math.Min(value.X, _contentSize.Width - canvasRectangle.Width));
                _scrollingPosition.Y = System.Math.Max(0, System.Math.Min(value.Y, _contentSize.Height - canvasRectangle.Height));
                InvalidateViewer();
            }
        }

        public override System.Drawing.Rectangle GetViewportBounds()
        {
            System.Drawing.Rectangle canvasRect = GetCanvasBounds();

            int deltaX = System.Math.Max(0, canvasRect.Width - _contentSize.Width);
            int deltaY = System.Math.Max(0, canvasRect.Height - _contentSize.Height);

            if (deltaX > 0 || deltaY > 0)
            {
                System.Drawing.Rectangle result = canvasRect;
                result.Width = System.Math.Min(canvasRect.Width, _contentSize.Width);
                result.Height = System.Math.Min(canvasRect.Height, _contentSize.Height);

                switch (_viewportAlignment)
                {
                    case ViewportAlignment.LeftTop:
                        break;

                    case ViewportAlignment.LeftCenter:
                        result.Offset(0, deltaY / 2);
                        break;

                    case ViewportAlignment.LeftBottom:
                        result.Offset(0, deltaY);
                        break;

                    case ViewportAlignment.CenterTop:
                        result.Offset(deltaX / 2, 0);
                        break;

                    case ViewportAlignment.CenterCenter:
                        result.Offset(deltaX / 2, deltaY / 2);
                        break;

                    case ViewportAlignment.CenterBottom:
                        result.Offset(deltaX / 2, deltaY);
                        break;

                    case ViewportAlignment.RightTop:
                        result.Offset(deltaX, 0);
                        break;

                    case ViewportAlignment.RightCenter:
                        result.Offset(deltaX, deltaY / 2);
                        break;

                    case ViewportAlignment.RightBottom:
                        result.Offset(deltaX, deltaY);
                        break;
                }

                return result;
            }

            return canvasRect;
        }

        public override System.Drawing.Rectangle GetCanvasBounds()
        {
            System.Drawing.Rectangle result = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);

            if (this.Width > 0 && this.Height > 0)
            {
                int scrollBarCx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);

                if (_verticalScrollBarShow)
                    result.Width -= scrollBarCx;
                if (_horizontalScrollBarShow)
                    result.Height -= scrollBarCx;

                if (BorderStyle != System.Windows.Forms.Border3DStyle.Flat)
                {
                    int cx = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE);
                    int cy = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE);

                    result.X += cx;
                    result.Y += cy;

                    if (!_horizontalScrollBarShow)
                        result.Height -= 2 * cy;

                    if (!_verticalScrollBarShow)
                        result.Width -= 2 * cx;
                }
            }

            return result;
        }

        private void InvalidateLayer(Layer layer, System.Drawing.Rectangle rect)
        {
            _viewportRenderer.InvalidateLayerRegion(layer, ControlToWorkspace(rect, Aurigma.GraphicsMill.Unit.Point));
            _invalidateLayers = false;
            InvalidateViewer(rect);
        }

        private void InvalidateLayer(Layer layer)
        {
            _viewportRenderer.InvalidateLayerRegion(layer, new System.Drawing.RectangleF(0, 0, _workspaceWidth, _workspaceHeight));
            _invalidateLayers = false;
            InvalidateViewer();
        }

        private void InvalidateDesigner(System.Drawing.Rectangle rect)
        {
            _invalidateLayers = false;
            InvalidateViewer(rect);
        }

        private void InvalidateDesigner()
        {
            _invalidateLayers = false;
            InvalidateViewer();
        }

        protected override void OnInvalidated(System.Windows.Forms.InvalidateEventArgs e)
        {
            try
            {
                if (_invalidateLayers)
                {
                    if (e.InvalidRect.IsEmpty)
                        _viewportRenderer.InvalidateLayerRegion(null, new System.Drawing.RectangleF(0, 0, _workspaceWidth, _workspaceHeight));
                    else
                        _viewportRenderer.InvalidateLayerRegion(null, this.ControlToWorkspace(e.InvalidRect, Aurigma.GraphicsMill.Unit.Point));
                }
                base.OnInvalidated(e);
            }
            finally
            {
                _invalidateLayers = true;
            }
        }

        #endregion ViewerBase Members

        #region "ICoordinateMapper Members"

        public override System.Drawing.PointF ControlToWorkspace(System.Drawing.Point point, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            System.Drawing.Rectangle viewportBounds = GetViewportBounds();

            float x = (float)(point.X - viewportBounds.X + _scrollingPosition.X) / base.ZoomInternal;
            float y = (float)(point.Y - viewportBounds.Y + _scrollingPosition.Y) / base.ZoomInternal;

            if (workspaceUnit != Aurigma.GraphicsMill.Unit.Pixel)
            {
                float inches = x / base.ViewerResolution;
                x = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(base.ViewerResolution, inches, Aurigma.GraphicsMill.Unit.Inch, workspaceUnit);

                inches = y / base.ViewerResolution;
                y = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(base.ViewerResolution, inches, Aurigma.GraphicsMill.Unit.Inch, workspaceUnit);
            }

            return new System.Drawing.PointF(x, y);
        }

        public override System.Drawing.Point WorkspaceToControl(System.Drawing.PointF point, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            System.Drawing.Point result = new System.Drawing.Point();
            if (workspaceUnit == Aurigma.GraphicsMill.Unit.Pixel)
            {
                result.X = System.Convert.ToInt32(base.ZoomInternal * point.X);
                result.Y = System.Convert.ToInt32(base.ZoomInternal * point.Y);
            }
            else
            {
                result.X = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToPixels(base.ViewerResolution, point.X * base.ZoomInternal, workspaceUnit);
                result.Y = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToPixels(base.ViewerResolution, point.Y * base.ZoomInternal, workspaceUnit);
            }

            System.Drawing.Rectangle viewportBounds = GetViewportBounds();
            result.Offset(viewportBounds.X, viewportBounds.Y);
            result.Offset(-_scrollingPosition.X, -_scrollingPosition.Y);
            return result;
        }

        #endregion "ICoordinateMapper Members"

        #region "IStateNavigable interface implementation"

        public void ClearHistory()
        {
            _objectHost.ClearHistory();
        }

        public void ClearUndoHistory()
        {
            _objectHost.ClearUndoHistory();
        }

        public void ClearRedoHistory()
        {
            _objectHost.ClearRedoHistory();
        }

        public void SaveState()
        {
            _objectHost.SaveState();
        }

        public void Undo()
        {
            _objectHost.Undo();
        }

        public void Undo(int undoStepCount)
        {
            _objectHost.Undo(undoStepCount);
        }

        public void Redo()
        {
            _objectHost.Redo();
        }

        public void Redo(int redoStepCount)
        {
            _objectHost.Redo(redoStepCount);
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("MultiLayerViewer_UndoRedoTrackingEnabled")]
        public bool UndoRedoTrackingEnabled
        {
            get
            {
                return _objectHost.UndoRedoTrackingEnabled;
            }
            set
            {
                _objectHost.UndoRedoTrackingEnabled = value;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("MultiLayerViewer_UndoRedoEnabled")]
        public bool UndoRedoEnabled
        {
            get
            {
                return _objectHost.UndoRedoEnabled;
            }
            set
            {
                _objectHost.UndoRedoEnabled = value;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(10)]
        [ResDescription("MultiLayerViewer_MaxUndoStepCount")]
        public int MaxUndoStepCount
        {
            get
            {
                return _objectHost.MaxUndoStepCount;
            }
            set
            {
                _objectHost.MaxUndoStepCount = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int UndoStepCount
        {
            get
            {
                return _objectHost.UndoStepCount;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int RedoStepCount
        {
            get
            {
                return _objectHost.RedoStepCount;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public bool CanUndo
        {
            get
            {
                return _objectHost.CanUndo;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public bool CanRedo
        {
            get
            {
                return _objectHost.CanRedo;
            }
        }

        private void UndoingEventHandler(object sender, Aurigma.GraphicsMill.StateRestoringEventArgs e)
        {
            OnUndoing(e);
        }

        private void RedoingEventHandler(object sender, Aurigma.GraphicsMill.StateRestoringEventArgs e)
        {
            OnRedoing(e);
        }

        private void UndoneEventHandler(object sender, System.EventArgs e)
        {
            OnUndone(e);
        }

        private void RedoneEventHandler(object sender, System.EventArgs e)
        {
            OnRedone(e);
        }

        protected virtual void OnUndoing(Aurigma.GraphicsMill.StateRestoringEventArgs e)
        {
            if (Undoing != null)
                Undoing(this, e);
        }

        protected virtual void OnRedoing(Aurigma.GraphicsMill.StateRestoringEventArgs e)
        {
            if (Redoing != null)
                Redoing(this, e);
        }

        protected virtual void OnUndone(System.EventArgs e)
        {
            if (Undone != null)
                Undone(this, e);
        }

        protected virtual void OnRedone(System.EventArgs e)
        {
            if (Redone != null)
                Redone(this, e);
        }

        public event Aurigma.GraphicsMill.StateRestoringEventHandler Undoing;

        public event System.EventHandler Undone;

        public event Aurigma.GraphicsMill.StateRestoringEventHandler Redoing;

        public event System.EventHandler Redone;

        #endregion "IStateNavigable interface implementation"
    }
}