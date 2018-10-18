// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// This class implements Undo/redo functionality for MultiLayerControl and VObjectsRubberband objects.
    /// It provides full implementation of Undo/Redo/SaveState methods and UndoStepCount/RedoStepCount/etc
    /// properties. Also the class tracks some events and automatically saves the state of the IVObjectHost
    /// after the following actions: LayerAdded / LayerRemoved / ObjectAdded / ObjectRemoved / ObjectZOrderChanged /
    /// VisibilityChanged / LockStatusChanged. All other changes should be tracked by designers or users - they
    /// should call IVObjectHost.SaveState() method after any other significant change.
    /// </summary>
    internal class UndoRedoTracker : Aurigma.GraphicsMill.IStateNavigable
    {
        #region "Internal classes"

        internal class StateStack : System.IDisposable
        {
            public StateStack(int capacity)
            {
                _index = -1;
                _array = new VObjectHostState[capacity];
            }

            public VObjectHostState Pop()
            {
                if (_disposed)
                    throw new System.ObjectDisposedException("StatesStack");
                if (_count < 1)
                    throw new Aurigma.GraphicsMill.ObjectEmptyException();

                VObjectHostState result = _array[_index];
                System.Diagnostics.Debug.Assert(result != null, "State stack cannot contain null elements!");
                _array[_index] = null;

                if (--_index < 0)
                    _index = _array.Length - 1;
                _count--;

                return result;
            }

            public void Push(VObjectHostState state)
            {
                if (_disposed)
                    throw new System.ObjectDisposedException("StatesStack");
                if (state == null)
                    throw new System.ArgumentNullException("state");
                if (state.IsEmpty)
                    throw new System.ArgumentException(StringResources.GetString("ExStrObjectCannotBeEmpty"), "state");

                if (_count < _array.Length)
                    _count++;
                if (++_index >= _array.Length)
                    _index = 0;

                if (_array[_index] != null)
                {
                    _array[_index].Dispose();
                    _array[_index] = null;
                }

                _array[_index] = state;
            }

            public void Clear()
            {
                if (_disposed)
                    throw new System.ObjectDisposedException("StateStack");

                for (int i = 0; i < _array.Length; i++)
                    if (_array[i] != null)
                        _array[i].Dispose();

                _count = 0;
            }

            public void Dispose()
            {
                try
                {
                    Clear();
                    _array = null;
                    _disposed = true;
                }
                finally
                {
                    System.GC.SuppressFinalize(this);
                }
            }

            public int Capacity
            {
                get
                {
                    if (_disposed)
                        throw new System.ObjectDisposedException("StateStack");

                    return _array.Length;
                }
                set
                {
                    if (_disposed)
                        throw new System.ObjectDisposedException("StateStack");
                    if (value < 1)
                        throw new System.ArgumentException(StringResources.GetString("ExStrValueShouldBeAboveZero"), "value");

                    if (_array.Length != value)
                    {
                        VObjectHostState[] prevArray = _array;
                        _array = new VObjectHostState[value];

                        int prevIndex = _index;
                        _index = System.Math.Min(_index, _array.Length - 1);
                        _count = System.Math.Min(_count, _array.Length);

                        for (int i = 0; i < _count; i++)
                        {
                            System.Diagnostics.Debug.Assert(prevArray[prevIndex] != null, "Error during capacity changing. State stack cannot contain null elements.");
                            _array[_index - i] = prevArray[prevIndex];
                            if (--prevIndex < 0)
                                prevIndex = prevArray.Length - 1;
                        }
                    }
                }
            }

            public int Count
            {
                get
                {
                    if (_disposed)
                        throw new System.ObjectDisposedException("StateStack");

                    System.Diagnostics.Debug.Assert(_count <= _array.Length, "Stack elements count should be less than length of the internal contents array.");
                    return _count;
                }
            }

            #region "Members variables"

            private VObjectHostState[] _array;
            private int _count;
            private int _index;
            private bool _disposed;

            #endregion "Members variables"
        }

        internal class VObjectHostState : System.IDisposable
        {
            public VObjectHostState()
            {
            }

            ~VObjectHostState()
            {
                Dispose(false);
            }

            public System.IO.Stream Stream
            {
                get
                {
                    if (_disposed)
                        throw new System.ObjectDisposedException("VObjectsState");

                    if (_filename == null)
                    {
                        _filename = System.IO.Path.GetTempFileName();
                        _stream = new System.IO.FileStream(_filename, System.IO.FileMode.Open, System.IO.FileAccess.Write);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(System.IO.File.Exists(_filename), "Cannot find underlying file for VObjectsState stream.");

                        if (_stream != null)
                            _stream.Close();

                        _stream = new System.IO.FileStream(_filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    }

                    return _stream;
                }
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

            protected void Dispose(bool disposing)
            {
                if (_disposed)
                    return;

                try
                {
                    if (_stream != null)
                    {
                        _stream.Close();
                        _stream = null;
                    }

                    System.IO.File.Delete(_filename);
                    _filename = null;
                }
                finally
                {
                    _disposed = true;
                }
            }

            public bool IsEmpty
            {
                get
                {
                    if (_disposed)
                        throw new System.ObjectDisposedException("VObjectsState");

                    return _stream == null;
                }
            }

            public int LayerIndex
            {
                get
                {
                    return _layerIndex;
                }
                set
                {
                    if (value < 0 && value != -1)
                        throw new System.ArgumentOutOfRangeException("value");

                    _layerIndex = value;
                }
            }

            #region "Members variables"

            private System.IO.FileStream _stream;
            private string _filename;
            private bool _disposed;

            private int _layerIndex;

            #endregion "Members variables"
        }

        #endregion "Internal classes"

        #region "Construction / destruction"

        public UndoRedoTracker(IVObjectHost objectHost)
        {
            if (objectHost == null)
                throw new System.ArgumentNullException("objectHost");

            _vObjectHost = objectHost;
            _maxUndoStepCount = 10;
            _trackingEnabled = true;

            RegisterInternalEvents();
        }

        private void RegisterInternalEvents()
        {
            _vObjectHost.Layers.LayerAdded += new LayerEventHandler(LayerAddedHandler);
            _vObjectHost.Layers.LayerRemoved += new LayerRemovedEventHandler(LayerRemovedHandler);
            _vObjectHost.Layers.LayerChanged += new LayerChangedEventHandler(LayerChangedHandler);
        }

        #endregion "Construction / destruction"

        #region "Events stuff"

        public event Aurigma.GraphicsMill.StateRestoringEventHandler Undoing;

        public event System.EventHandler Undone;

        public event Aurigma.GraphicsMill.StateRestoringEventHandler Redoing;

        public event System.EventHandler Redone;

        protected virtual void OnUndoing(Aurigma.GraphicsMill.StateRestoringEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (Undoing != null)
                Undoing(this, e);

            if (e.Cancel)
                throw new Aurigma.GraphicsMill.GMException("Aborted");
        }

        protected virtual void OnRedoing(Aurigma.GraphicsMill.StateRestoringEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (Redoing != null)
                Redoing(this, e);

            if (e.Cancel)
                throw new Aurigma.GraphicsMill.GMException("Aborted");
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

        #endregion "Events stuff"

        #region "Public methods"

        public void ClearUndoHistory()
        {
            if (_undoStack != null)
                _undoStack.Clear();
        }

        public void ClearRedoHistory()
        {
            if (_redoStack != null)
                _redoStack.Clear();
        }

        public void ClearHistory()
        {
            ClearRedoHistory();
            ClearUndoHistory();
        }

        public void SaveState()
        {
            if (!_undoRedoEnabled)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUndoRedoShouldBeEnabled"));

            ClearRedoHistory();

            if (_currentState != null)
                _undoStack.Push(_currentState);

            UpdateCurrentState();
        }

        public void Undo()
        {
            if (!_undoRedoEnabled)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUndoRedoShouldBeEnabled"));

            if (!this.CanUndo)
                return;

            Undo(1);
        }

        public void Undo(int undoStepCount)
        {
            if (!_undoRedoEnabled)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUndoRedoShouldBeEnabled"));
            if (undoStepCount < 1 || undoStepCount > this.UndoStepCount)
                throw new System.ArgumentOutOfRangeException("undoStepCount");

            OnUndoing(new StateRestoringEventArgs());

            while (undoStepCount-- > 0)
            {
                _redoStack.Push(_currentState);
                _currentState = _undoStack.Pop();
            }
            RestoreState(_currentState);
            OnUndone(System.EventArgs.Empty);
        }

        public void Redo()
        {
            if (!_undoRedoEnabled)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUndoRedoShouldBeEnabled"));

            if (!this.CanRedo)
                return;

            Redo(1);
        }

        public void Redo(int redoStepCount)
        {
            if (!_undoRedoEnabled)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUndoRedoShouldBeEnabled"));
            if (redoStepCount < 1 || redoStepCount > this.RedoStepCount)
                throw new System.ArgumentOutOfRangeException("undoStepCount");

            OnRedoing(new StateRestoringEventArgs());

            while (redoStepCount-- > 0)
            {
                _undoStack.Push(_currentState);
                _currentState = _redoStack.Pop();
            }
            RestoreState(_currentState);
            OnRedone(System.EventArgs.Empty);
        }

        #endregion "Public methods"

        #region "Public properties"

        public bool UndoRedoEnabled
        {
            get
            {
                return _undoRedoEnabled;
            }
            set
            {
                if (_undoRedoEnabled != value)
                {
                    _undoRedoEnabled = value;
                    UpdateUndoRedoStructures();
                }
            }
        }

        public int MaxUndoStepCount
        {
            get
            {
                return _maxUndoStepCount;
            }
            set
            {
                if (_maxUndoStepCount != value)
                {
                    _maxUndoStepCount = value;
                    UpdateUndoRedoStructures();
                }
            }
        }

        public int UndoStepCount
        {
            get
            {
                if (!_undoRedoEnabled)
                    return 0;

                return _undoStack.Count;
            }
        }

        public int RedoStepCount
        {
            get
            {
                if (!_undoRedoEnabled)
                    return 0;

                return _redoStack.Count;
            }
        }

        public bool CanUndo
        {
            get
            {
                return _undoRedoEnabled && _undoStack.Count > 0;
            }
        }

        public bool CanRedo
        {
            get
            {
                return _undoRedoEnabled && _redoStack.Count > 0;
            }
        }

        public bool UndoRedoTrackingEnabled
        {
            get
            {
                return _trackingEnabled;
            }
            set
            {
                _trackingEnabled = value;
            }
        }

        #endregion "Public properties"

        #region "Class internals"

        private void UpdateUndoRedoStructures()
        {
            if (!_undoRedoEnabled)
            {
                if (_undoStack != null)
                {
                    _undoStack.Dispose();
                    _undoStack = null;
                }
                if (_redoStack != null)
                {
                    _redoStack.Dispose();
                    _redoStack = null;
                }
                if (_currentState != null)
                {
                    _currentState.Dispose();
                    _currentState = null;
                }
            }
            else
            {
                if (_undoStack == null)
                    _undoStack = new StateStack(_maxUndoStepCount);
                else
                    _undoStack.Capacity = _maxUndoStepCount;

                if (_redoStack == null)
                    _redoStack = new StateStack(_maxUndoStepCount);
                else
                    _redoStack.Capacity = _maxUndoStepCount;

                if (_currentState == null)
                    UpdateCurrentState();
            }
        }

        private void UpdateCurrentState()
        {
            if (!_undoRedoEnabled)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUndoRedoShouldBeEnabled"));

            _currentState = new VObjectHostState();
            _vObjectHost.Layers.Serialize(_currentState.Stream);
            _currentState.LayerIndex = _vObjectHost.CurrentLayerIndex;
        }

        private void RestoreState(VObjectHostState state)
        {
            if (!_undoRedoEnabled)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUndoRedoShouldBeEnabled"));

            _restoringState = true;
            try
            {
                _vObjectHost.Layers.Deserialize(state.Stream);
                _vObjectHost.CurrentLayerIndex = state.LayerIndex;
            }
            finally
            {
                _restoringState = false;
            }
        }

        #endregion "Class internals"

        #region "Events tracking"

        private void LayerAddedHandler(object sender, LayerEventArgs e)
        {
            if (_undoRedoEnabled && !_restoringState && _trackingEnabled)
                SaveState();
        }

        private void LayerRemovedHandler(object sender, LayerRemovedEventArgs e)
        {
            if (_undoRedoEnabled && !_restoringState && _trackingEnabled)
                SaveState();
        }

        private void LayerChangedHandler(object sender, LayerChangedEventArgs e)
        {
            if (_undoRedoEnabled && !_restoringState && _trackingEnabled &&
                (e.ChangeType == LayerChangeType.ObjectAdded ||
                 e.ChangeType == LayerChangeType.ObjectRemoved ||
                 e.ChangeType == LayerChangeType.ObjectZOrderChanged ||
                 e.ChangeType == LayerChangeType.VisibilityChanged ||
                 e.ChangeType == LayerChangeType.LockStatusChanged))
                SaveState();
        }

        #endregion "Events tracking"

        #region "Member variables"

        private IVObjectHost _vObjectHost;
        private bool _undoRedoEnabled;
        private int _maxUndoStepCount;
        private VObjectHostState _currentState;
        private StateStack _undoStack;
        private StateStack _redoStack;
        private bool _trackingEnabled;
        private bool _restoringState;

        #endregion "Member variables"
    }
}