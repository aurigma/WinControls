// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Common implementation of the IVObjectHost interface for VObjectsRubberband
    /// and MultiLayerViewer. They just delegate interface calls to this object.
    /// </summary>
    internal class VObjectHost : IVObjectHost, System.IDisposable
    {
        #region "Construction / destruction"

        public VObjectHost(ViewerBase hostControl)
        {
            _hostViewer = hostControl;

            _layers = new LayerCollection();
            _layers.Add(new Layer("layer0"));
            _currentLayer = _layers[0];
            _undoRedoTracker = new UndoRedoTracker(this);

            RegisterInternalEventHandlers();

            InitializeDesignerOptions();
            _defaultDesigner = new DefaultDesigner();
            this.CurrentDesigner = _defaultDesigner;
        }

        private void InitializeDesignerOptions()
        {
            _designerOptions = new System.Collections.Hashtable();

            _designerOptions[DesignerSettingsConstants.ResizeProportionallyWithShift] = true;
            _designerOptions[DesignerSettingsConstants.MultiSelect] = true;
            _designerOptions[DesignerSettingsConstants.MultipleVObjectsTransformationEnabled] = true;
        }

        private void RegisterInternalEventHandlers()
        {
            _layers.LayerChanged += new LayerChangedEventHandler(LayerChangedHandler);
            _layers.LayerAdded += new LayerEventHandler(LayerAddedHandler);
            _layers.LayerRemoved += new LayerRemovedEventHandler(LayerRemovedHandler);

            _undoRedoTracker.Undoing += new StateRestoringEventHandler(UndoingEventHandler);
            _undoRedoTracker.Redoing += new StateRestoringEventHandler(RedoingEventHandler);
            _undoRedoTracker.Undone += new EventHandler(UndoneEventHandler);
            _undoRedoTracker.Redone += new EventHandler(RedoneEventHandler);
        }

        public void Dispose()
        {
            try
            {
                if (_defaultDesigner != null)
                {
                    _defaultDesigner.Dispose();
                    _defaultDesigner = null;
                }

                if (_designerOptions != null)
                {
                    _designerOptions.Clear();
                    _designerOptions = null;
                }
            }
            finally
            {
                System.GC.SuppressFinalize(this);
            }
        }

        #endregion "Construction / destruction"

        #region "IObjectHost interface implementation"

        public IDesigner CurrentDesigner
        {
            get
            {
                return _currentDesigner;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");
                if (_currentLayer == null && value != this.DefaultDesigner)
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotChangeDesignerWhileNoCurLayer"));
                if (value.VObjects.Length != 0 && !_currentLayer.VObjects.Contains(value.VObjects[0]))
                    throw new System.ArgumentException(StringResources.GetString("ExStrObjectShouldBelongToCurLayer"), "value");
                if (value != this.DefaultDesigner && (!_currentLayer.Visible || _currentLayer.Locked))
                    throw new System.ArgumentException(StringResources.GetString("ExStrObjectShouldBelongToEnabledLayer"), "value");

                IDesigner prevDesigner = _currentDesigner;
                if (_currentDesigner != null)
                    this.CurrentDesigner.NotifyDisconnect();

                _currentDesigner = value;
                _currentDesigner.NotifyConnect(this);
                _currentDesigner.UpdateSettings();

                OnDesignerChanged(new DesignerChangedEventArgs(prevDesigner, _currentDesigner));
            }
        }

        public IDesigner DefaultDesigner
        {
            get
            {
                return _defaultDesigner;
            }
        }

        public System.Collections.Hashtable DesignerOptions
        {
            get
            {
                return _designerOptions;
            }
        }

        public ViewerBase HostViewer
        {
            get
            {
                return _hostViewer;
            }
        }

        public LayerCollection Layers
        {
            get
            {
                return _layers;
            }
        }

        public Layer CurrentLayer
        {
            get
            {
                return _currentLayer;
            }
            set
            {
                if (value != null && !_layers.Contains(value))
                    throw new System.ArgumentException(StringResources.GetString("ExStrCurLayerShouldBeInVObjectHost"), "value");

                ChangeCurrentLayer(value);
            }
        }

        public int CurrentLayerIndex
        {
            get
            {
                if (_currentLayer != null)
                    return _layers.IndexOf(_currentLayer);

                return -1;
            }
            set
            {
                if (value >= _layers.Count)
                    throw new System.ArgumentOutOfRangeException("value");

                if (value >= 0)
                    ChangeCurrentLayer(_layers[value]);
                else
                    ChangeCurrentLayer(null);
            }
        }

        private void ChangeCurrentLayer(Layer newLayer)
        {
            if (newLayer == _currentLayer)
                return;

            this.CurrentDesigner = this.DefaultDesigner;

            _currentLayer = newLayer;
            OnCurrentLayerChanged(System.EventArgs.Empty);
        }

        protected virtual void OnCurrentLayerChanged(System.EventArgs e)
        {
            if (CurrentLayerChanged != null)
                CurrentLayerChanged(this, e);
        }

        protected virtual void OnDesignerChanged(DesignerChangedEventArgs e)
        {
            if (DesignerChanged != null)
                DesignerChanged(this, e);
        }

        public event System.EventHandler CurrentLayerChanged;

        public event DesignerChangedEventHandler DesignerChanged;

        #endregion "IObjectHost interface implementation"

        #region "Layers & objects changes tracking"

        private void LayerChangedHandler(object sender, LayerChangedEventArgs e)
        {
            if (e.ChangeType == LayerChangeType.ObjectChanged || e.ChangeType == LayerChangeType.ObjectAdded || e.ChangeType == LayerChangeType.ObjectRemoved || e.ChangeType == LayerChangeType.ObjectZOrderChanged)
            {
                if (e.ChangeType == Aurigma.GraphicsMill.WinControls.LayerChangeType.ObjectRemoved)
                {
                    foreach (IVObject obj in _currentDesigner.VObjects)
                        if (obj == e.VObject)
                        {
                            this.CurrentDesigner = _defaultDesigner;
                            break;
                        }
                }

                InvalidateVObject(e.Layer, e.VObject);
            }

            if (e.ChangeType == LayerChangeType.VisibilityChanged)
            {
                if (e.Layer == _currentLayer && _currentDesigner != _defaultDesigner)
                    this.CurrentDesigner = _defaultDesigner;

                if (_hostViewer != null)
                    _hostViewer.InvalidateViewer();
            }

            if (e.ChangeType == LayerChangeType.LockStatusChanged && e.Layer == _currentLayer && _currentDesigner != _defaultDesigner)
            {
                this.CurrentDesigner = _defaultDesigner;
            }
        }

        private void LayerAddedHandler(object sender, LayerEventArgs e)
        {
            if (_hostViewer != null)
                _hostViewer.InvalidateViewer();
        }

        private void LayerRemovedHandler(object sender, LayerRemovedEventArgs e)
        {
            if (e.Layer == _currentLayer)
            {
                if (_layers.Count > 0)
                {
                    int newIndex = e.Index - 1;
                    ChangeCurrentLayer(_layers[VObjectsUtils.FitToBounds(newIndex, 0, _layers.Count - 1)]);
                }
                else
                    ChangeCurrentLayer(null);
            }

            if (_hostViewer != null)
                _hostViewer.InvalidateViewer();
        }

        private void InvalidateVObject(Layer layer, IVObject obj)
        {
            if (_hostViewer != null)
            {
                System.Drawing.Rectangle invalidationRect = _hostViewer.WorkspaceToControl(obj.GetTransformedVObjectBounds(), Aurigma.GraphicsMill.Unit.Point);
                invalidationRect.Inflate(VObject.InvalidationMargin);
                _hostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(invalidationRect, layer));
            }
        }

        #endregion "Layers & objects changes tracking"

        #region "Rendering functinality"

        public void DrawContent(System.Drawing.Graphics g, System.Drawing.Rectangle rect, ICoordinateMapper coordinateMapper)
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                if (!_layers[i].Visible)
                    continue;

                for (int j = 0; j < _layers[i].VObjects.Count; j++)
                {
                    IVObject obj = _layers[i].VObjects[j];
                    if (rect.IntersectsWith(coordinateMapper.WorkspaceToControl(obj.GetTransformedVObjectBounds(), Aurigma.GraphicsMill.Unit.Point)))
                        obj.Draw(rect, g, coordinateMapper);
                }
            }
        }

        public Aurigma.GraphicsMill.Bitmap RenderWorkspace(float renderingResolution)
        {
            if (renderingResolution < VObject.Eps)
                throw new System.ArgumentOutOfRangeException("renderingResolution", StringResources.GetString("ExStrValueShouldBeAboveZero"));

            float workspaceWidth, workspaceHeight;
            Aurigma.GraphicsMill.Unit prevUnit = _hostViewer.Unit;
            _hostViewer.Unit = Aurigma.GraphicsMill.Unit.Point;
            try
            {
                workspaceWidth = _hostViewer.WorkspaceWidth;
                workspaceHeight = _hostViewer.WorkspaceHeight;
            }
            finally
            {
                _hostViewer.Unit = prevUnit;
            }

            System.Drawing.Rectangle controlRectangle = CoordinateMapper.WorkspaceToControl(new System.Drawing.RectangleF(0, 0, workspaceWidth, workspaceHeight), 1.0f, System.Drawing.Point.Empty, Aurigma.GraphicsMill.Unit.Point, renderingResolution);
            controlRectangle.Width = System.Math.Max(1, controlRectangle.Width);
            controlRectangle.Height = System.Math.Max(1, controlRectangle.Height);

            Aurigma.GraphicsMill.Bitmap result = new Aurigma.GraphicsMill.Bitmap(controlRectangle.Width, controlRectangle.Height, Aurigma.GraphicsMill.PixelFormat.Format32bppArgb, Aurigma.GraphicsMill.RgbColor.Transparent);
            try
            {
                using (var g = result.GetGdiPlusGraphics())
                {
                    CoordinateMapper coordMapper = new CoordinateMapper();
                    coordMapper.Resolution = renderingResolution;
                    coordMapper.Zoom = 1.0f;
                    coordMapper.Viewport = controlRectangle;

                    _alwaysUseGdiPlus = true;
                    DrawContent(g, controlRectangle, coordMapper);
                    _alwaysUseGdiPlus = false;
                }
            }
            catch
            {
                if (result != null)
                    result.Dispose();

                throw;
            }

            result.DpiX = renderingResolution;
            result.DpiY = renderingResolution;

            return result;
        }

        #endregion "Rendering functinality"

        #region "IStateNavigable interface implementation"

        public event Aurigma.GraphicsMill.StateRestoringEventHandler Undoing;

        public event System.EventHandler Undone;

        public event Aurigma.GraphicsMill.StateRestoringEventHandler Redoing;

        public event System.EventHandler Redone;

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

        public void ClearHistory()
        {
            _undoRedoTracker.ClearHistory();
        }

        public void ClearUndoHistory()
        {
            _undoRedoTracker.ClearUndoHistory();
        }

        public void ClearRedoHistory()
        {
            _undoRedoTracker.ClearRedoHistory();
        }

        public void SaveState()
        {
            _undoRedoTracker.SaveState();
        }

        public void Undo()
        {
            _undoRedoTracker.Undo();
        }

        public void Undo(int undoStepCount)
        {
            _undoRedoTracker.Undo(undoStepCount);
        }

        public void Redo()
        {
            _undoRedoTracker.Redo();
        }

        public void Redo(int redoStepCount)
        {
            _undoRedoTracker.Redo(redoStepCount);
        }

        public bool UndoRedoEnabled
        {
            get
            {
                return _undoRedoTracker.UndoRedoEnabled;
            }
            set
            {
                _undoRedoTracker.UndoRedoEnabled = value;
            }
        }

        public int MaxUndoStepCount
        {
            get
            {
                return _undoRedoTracker.MaxUndoStepCount;
            }
            set
            {
                _undoRedoTracker.MaxUndoStepCount = value;
            }
        }

        public int UndoStepCount
        {
            get
            {
                return _undoRedoTracker.UndoStepCount;
            }
        }

        public int RedoStepCount
        {
            get
            {
                return _undoRedoTracker.RedoStepCount;
            }
        }

        public bool CanUndo
        {
            get
            {
                return _undoRedoTracker.CanUndo;
            }
        }

        public bool CanRedo
        {
            get
            {
                return _undoRedoTracker.CanRedo;
            }
        }

        public bool UndoRedoTrackingEnabled
        {
            get
            {
                return _undoRedoTracker.UndoRedoTrackingEnabled;
            }
            set
            {
                _undoRedoTracker.UndoRedoTrackingEnabled = value;
            }
        }

        #endregion "IStateNavigable interface implementation"

        #region "Internal properties"

        internal ViewerBase HostViewerInternal
        {
            get
            {
                return _hostViewer;
            }
            set
            {
                _hostViewer = value;
            }
        }

        #endregion "Internal properties"

        #region "Member variables"

        private ViewerBase _hostViewer;

        private LayerCollection _layers;
        private Layer _currentLayer;

        private UndoRedoTracker _undoRedoTracker;
        private bool _alwaysUseGdiPlus = false;

        private System.Collections.Hashtable _designerOptions;
        private IDesigner _currentDesigner;
        private DefaultDesigner _defaultDesigner;

        #endregion "Member variables"
    }
}