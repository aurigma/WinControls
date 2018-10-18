// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    [AdaptiveToolboxBitmapAttribute(typeof(ResourceFinder), "VObjectsRubberband.bmp")]
    public class VObjectsRubberband : UserInputController, IRubberband, IVObjectHost
    {
        #region "Construction / destruction"

        public VObjectsRubberband()
        {
            _bitmapViewer = null;
            _vObjectHost = new VObjectHost(_bitmapViewer);

            RegisterInternalEventHandlers();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                UnregisterInternalEventHandlers();
                _vObjectHost.Dispose();
                _vObjectHost = null;
                _bitmapViewer = null;
            }
        }

        private void RegisterInternalEventHandlers()
        {
            _vObjectHost.Layers.LayerChanged += new LayerChangedEventHandler(LayerChangedHander);
            _vObjectHost.Layers.LayerAdded += new LayerEventHandler(LayerAddedHander);
            _vObjectHost.Layers.LayerRemoved += new LayerRemovedEventHandler(LayerRemovedHandler);

            _vObjectHost.CurrentLayerChanged += new System.EventHandler(CurrentLayerChangedHandler);
            _vObjectHost.DesignerChanged += new DesignerChangedEventHandler(DesignerChangedHander);

            _vObjectHost.Undoing += new StateRestoringEventHandler(UndoingEventHandler);
            _vObjectHost.Redoing += new StateRestoringEventHandler(RedoingEventHandler);
            _vObjectHost.Undone += new System.EventHandler(UndoneEventHandler);
            _vObjectHost.Redone += new System.EventHandler(RedoneEventHandler);
        }

        private void UnregisterInternalEventHandlers()
        {
            _vObjectHost.Layers.LayerChanged -= new LayerChangedEventHandler(LayerChangedHander);
            _vObjectHost.Layers.LayerAdded -= new LayerEventHandler(LayerAddedHander);
            _vObjectHost.Layers.LayerRemoved -= new LayerRemovedEventHandler(LayerRemovedHandler);

            _vObjectHost.CurrentLayerChanged -= new System.EventHandler(CurrentLayerChangedHandler);
            _vObjectHost.DesignerChanged -= new DesignerChangedEventHandler(DesignerChangedHander);

            _vObjectHost.Undoing -= new StateRestoringEventHandler(UndoingEventHandler);
            _vObjectHost.Redoing -= new StateRestoringEventHandler(RedoingEventHandler);
            _vObjectHost.Undone -= new System.EventHandler(UndoneEventHandler);
            _vObjectHost.Redone -= new System.EventHandler(RedoneEventHandler);
        }

        #endregion "Construction / destruction"

        #region "UserInputController overloads"

        public override void Connect(ViewerBase viewer)
        {
            if (!(viewer is BitmapViewer))
                throw new System.ArgumentException(StringResources.GetString("ExStrVObjRubberbandAcceptsBitmapViewer"), "viewer");

            base.Connect(viewer);
            _vObjectHost.HostViewerInternal = viewer;
            _bitmapViewer = (BitmapViewer)viewer;
        }

        public override void Disconnect()
        {
            if (_vObjectHost != null)
            {
                _vObjectHost.CurrentDesigner = _vObjectHost.DefaultDesigner;
                _vObjectHost.HostViewerInternal = null;
            }

            _bitmapViewer = null;

            base.Disconnect();
        }

        protected override void OnViewerMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnViewerMouseDown(e);

            if (_vObjectHost.CurrentDesigner != null && _vObjectHost.CurrentDesigner.NotifyMouseDown(e))
                return;

            _vObjectHost.CurrentDesigner = _vObjectHost.DefaultDesigner;
            _vObjectHost.CurrentDesigner.NotifyMouseDown(e);
        }

        protected override void OnViewerMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnViewerMouseUp(e);

            if (_vObjectHost.CurrentDesigner != null && _vObjectHost.CurrentDesigner.NotifyMouseUp(e))
                return;

            _vObjectHost.CurrentDesigner = _vObjectHost.DefaultDesigner;
            _vObjectHost.CurrentDesigner.NotifyMouseUp(e);
        }

        protected override void OnViewerMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnViewerMouseMove(e);

            if (_vObjectHost.CurrentDesigner != null && _vObjectHost.CurrentDesigner.NotifyMouseMove(e))
                return;

            _vObjectHost.CurrentDesigner = _vObjectHost.DefaultDesigner;
            _vObjectHost.CurrentDesigner.NotifyMouseMove(e);
        }

        protected override void OnViewerDoubleClick(System.EventArgs e)
        {
            base.OnViewerDoubleClick(e);

            if (_vObjectHost.CurrentDesigner != null && _vObjectHost.CurrentDesigner.NotifyMouseDoubleClick(e))
                return;

            _vObjectHost.CurrentDesigner = _vObjectHost.DefaultDesigner;
            _vObjectHost.CurrentDesigner.NotifyMouseDoubleClick(e);
        }

        protected override void OnViewerKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnViewerKeyDown(e);

            if (_vObjectHost.CurrentDesigner != null && _vObjectHost.CurrentDesigner.NotifyKeyDown(e))
                return;

            _vObjectHost.CurrentDesigner = _vObjectHost.DefaultDesigner;
            _vObjectHost.CurrentDesigner.NotifyKeyDown(e);
        }

        protected override void OnViewerKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnViewerKeyUp(e);

            if (_vObjectHost.CurrentDesigner != null && _vObjectHost.CurrentDesigner.NotifyKeyUp(e))
                return;

            _vObjectHost.CurrentDesigner = _vObjectHost.DefaultDesigner;
            _vObjectHost.CurrentDesigner.NotifyKeyUp(e);
        }

        protected override void OnViewerDoubleBufferPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnViewerDoubleBufferPaint(e);

            System.Drawing.Rectangle visibleRectangle = System.Drawing.Rectangle.Intersect(e.ClipRectangle, _bitmapViewer.GetViewportBounds());

            System.Drawing.Region prevClip = e.Graphics.Clip.Clone();
            System.Drawing.Region newClip = new System.Drawing.Region(visibleRectangle);
            try
            {
                e.Graphics.Clip = newClip;
                _vObjectHost.DrawContent(e.Graphics, visibleRectangle, (ICoordinateMapper)_bitmapViewer);
            }
            finally
            {
                e.Graphics.Clip = prevClip;
                newClip.Dispose();
            }

            _vObjectHost.CurrentDesigner.Draw(e.Graphics);
        }

        #endregion "UserInputController overloads"

        #region IVObjectHost Members

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public IDesigner CurrentDesigner
        {
            get
            {
                return _vObjectHost.CurrentDesigner;
            }
            set
            {
                _vObjectHost.CurrentDesigner = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public IDesigner DefaultDesigner
        {
            get
            {
                return _vObjectHost.DefaultDesigner;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public System.Collections.Hashtable DesignerOptions
        {
            get
            {
                return _vObjectHost.DesignerOptions;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public ViewerBase HostViewer
        {
            get
            {
                return _bitmapViewer;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public LayerCollection Layers
        {
            get
            {
                return _vObjectHost.Layers;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Layer CurrentLayer
        {
            get
            {
                return _vObjectHost.CurrentLayer;
            }
            set
            {
                _vObjectHost.CurrentLayer = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int CurrentLayerIndex
        {
            get
            {
                return _vObjectHost.CurrentLayerIndex;
            }
            set
            {
                _vObjectHost.CurrentLayerIndex = value;
            }
        }

        //
        // Re-firing events from this object.
        //
        protected virtual void DesignerChangedHander(object sender, DesignerChangedEventArgs e)
        {
            OnDesignerChanged(e);
        }

        protected virtual void CurrentLayerChangedHandler(object sender, System.EventArgs e)
        {
            OnCurrentLayerChanged(e);
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

        public event System.EventHandler CurrentLayerChanged;

        public event DesignerChangedEventHandler DesignerChanged;

        #endregion IVObjectHost Members

        #region "IStateNavigable interface implementation"

        public void ClearHistory()
        {
            _vObjectHost.ClearHistory();
        }

        public void ClearUndoHistory()
        {
            _vObjectHost.ClearUndoHistory();
        }

        public void ClearRedoHistory()
        {
            _vObjectHost.ClearRedoHistory();
        }

        public void SaveState()
        {
            _vObjectHost.SaveState();
        }

        public void Undo()
        {
            _vObjectHost.Undo();
        }

        public void Undo(int undoStepCount)
        {
            _vObjectHost.Undo(undoStepCount);
        }

        public void Redo()
        {
            _vObjectHost.Redo();
        }

        public void Redo(int redoStepCount)
        {
            _vObjectHost.Redo(redoStepCount);
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("MultiLayerViewer_UndoRedoTrackingEnabled")]
        public bool UndoRedoTrackingEnabled
        {
            get
            {
                return _vObjectHost.UndoRedoTrackingEnabled;
            }
            set
            {
                _vObjectHost.UndoRedoTrackingEnabled = value;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("MultiLayerViewer_UndoRedoEnabled")]
        public bool UndoRedoEnabled
        {
            get
            {
                return _vObjectHost.UndoRedoEnabled;
            }
            set
            {
                _vObjectHost.UndoRedoEnabled = value;
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(10)]
        [ResDescription("MultiLayerViewer_MaxUndoStepCount")]
        public int MaxUndoStepCount
        {
            get
            {
                return _vObjectHost.MaxUndoStepCount;
            }
            set
            {
                _vObjectHost.MaxUndoStepCount = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int UndoStepCount
        {
            get
            {
                return _vObjectHost.UndoStepCount;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int RedoStepCount
        {
            get
            {
                return _vObjectHost.RedoStepCount;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public bool CanUndo
        {
            get
            {
                return _vObjectHost.CanUndo;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public bool CanRedo
        {
            get
            {
                return _vObjectHost.CanRedo;
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

        #region "Duplicate events for convenience"

        //
        // Following layer-related events are duplicated from LayerCollection of the control -
        // to allow user to assign event handlers in VS form designer.
        //
        public event LayerEventHandler LayerAdded;

        public event LayerRemovedEventHandler LayerRemoved;

        public event LayerChangedEventHandler LayerChanged;

        private void LayerChangedHander(object sender, LayerChangedEventArgs e)
        {
            OnLayerChanged(e);
        }

        private void LayerAddedHander(object sender, LayerEventArgs e)
        {
            OnLayerAdded(e);
        }

        private void LayerRemovedHandler(object sender, LayerRemovedEventArgs e)
        {
            OnLayerRemoved(e);
        }

        protected virtual void OnLayerChanged(LayerChangedEventArgs e)
        {
            if (LayerChanged != null)
                LayerChanged(this, e);
        }

        protected virtual void OnLayerAdded(LayerEventArgs e)
        {
            if (LayerAdded != null)
                LayerAdded(this, e);
        }

        protected virtual void OnLayerRemoved(LayerRemovedEventArgs e)
        {
            if (LayerRemoved != null)
                LayerRemoved(this, e);
        }

        #endregion "Duplicate events for convenience"

        #region "Aliases for designer properties"

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("MultiLayerViewer_ResizeProportionallyWithShift")]
        public bool ResizeProportionallyWithShift
        {
            get
            {
                if (!_vObjectHost.DesignerOptions.ContainsKey(DesignerSettingsConstants.ResizeProportionallyWithShift))
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotFindDesignerOptionsKey"));

                return (bool)_vObjectHost.DesignerOptions[DesignerSettingsConstants.ResizeProportionallyWithShift];
            }
            set
            {
                _vObjectHost.DesignerOptions[DesignerSettingsConstants.ResizeProportionallyWithShift] = value;
                _vObjectHost.CurrentDesigner.UpdateSettings();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(true)]
        [ResDescription("MultiLayerViewer_MultiSelect")]
        public bool MultiSelect
        {
            get
            {
                if (!_vObjectHost.DesignerOptions.ContainsKey(DesignerSettingsConstants.MultiSelect))
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotFindDesignerOptionsKey"));

                return (bool)_vObjectHost.DesignerOptions[DesignerSettingsConstants.MultiSelect];
            }
            set
            {
                _vObjectHost.DesignerOptions[DesignerSettingsConstants.MultiSelect] = value;
                _vObjectHost.CurrentDesigner.UpdateSettings();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DefaultValue(false)]
        [ResDescription("MultiLayerViewer_DragOnlyMultipleObjects")]
        public bool MultipleVObjectsTransformationEnabled
        {
            get
            {
                if (!_vObjectHost.DesignerOptions.ContainsKey(DesignerSettingsConstants.MultipleVObjectsTransformationEnabled))
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotFindDesignerOptionsKey"));

                return (bool)_vObjectHost.DesignerOptions[DesignerSettingsConstants.MultipleVObjectsTransformationEnabled];
            }
            set
            {
                _vObjectHost.DesignerOptions[DesignerSettingsConstants.MultipleVObjectsTransformationEnabled] = value;
                _vObjectHost.CurrentDesigner.UpdateSettings();
            }
        }

        #endregion "Aliases for designer properties"

        #region "Public methods"

        public Aurigma.GraphicsMill.Bitmap RenderWorkspace()
        {
            return RenderWorkspace(_bitmapViewer.ViewerResolution);
        }

        public Aurigma.GraphicsMill.Bitmap RenderWorkspace(float renderingResolution)
        {
            return _vObjectHost.RenderWorkspace(renderingResolution);
        }

        #endregion "Public methods"

        #region "Member variables"

        private BitmapViewer _bitmapViewer;
        private VObjectHost _vObjectHost;

        #endregion "Member variables"
    }
}