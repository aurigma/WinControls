// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Composite object - object which is composed of other IVObjects.
    /// The object is used to provides multi-selection functionality.
    /// </summary>
    [System.Serializable]
    public class CompositeVObject : VObject
    {
        #region "Member variables"

        private System.Drawing.RectangleF _baseRect;
        private System.Drawing.Drawing2D.Matrix _matrix;

        private VObjectCollection _children;
        private System.Drawing.Drawing2D.Matrix[] _childMatrices;
        private bool _updatingChildren;
        private bool _multipleVObjectsTransformationEnabled;

        private IDesigner _designer;

        #endregion "Member variables"

        #region "Construction / destruction"

        public CompositeVObject()
        {
            _multipleVObjectsTransformationEnabled = true;
            _matrix = new System.Drawing.Drawing2D.Matrix();
            _children = new VObjectCollection();

            _children.VObjectAdded += new VObjectEventHandler(ObjectAddedHandler);
            _children.VObjectRemoved += new VObjectEventHandler(ObjectRemovedHandler);
        }

        public CompositeVObject(IVObject[] children)
            : this()
        {
            if (children == null)
                throw new System.ArgumentNullException("children");
            if (children.Length < 1)
                throw new System.ArgumentException(StringResources.GetString("ExStrArrayZeroLengthError"), "children");

            for (int i = 0; i < children.Length; i++)
                _children.Add(children[i]);
        }

        protected CompositeVObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new System.NotImplementedException();
        }

        #endregion "Construction / destruction"

        #region "Internal events tracking"

        private void ObjectAddedHandler(object sender, VObjectEventArgs e)
        {
            e.VObject.Changed += new System.EventHandler(VObjectChangedHandler);
            ProcessVObjectCollectionChanges();
        }

        private void ObjectRemovedHandler(object sender, VObjectEventArgs e)
        {
            e.VObject.Changed -= new System.EventHandler(VObjectChangedHandler);
            ProcessVObjectCollectionChanges();
        }

        private void VObjectChangedHandler(object sender, System.EventArgs e)
        {
            if (_updatingChildren)
                return;

            VObject vobj = sender as VObject;
            if (vobj != null && vobj.IsDisposed)
                _children.Remove(vobj);

            _matrix.Reset();

            UpdateBaseRectangle();
            UpdateChildMatrices();
            UpdateControlPointsState();
            base.OnChanged(System.EventArgs.Empty);
        }

        private void ProcessVObjectCollectionChanges()
        {
            _matrix.Reset();

            UpdateBaseRectangle();
            UpdateChildMatrices();
            UpdateControlPointsState();
            base.OnChanged(System.EventArgs.Empty);
        }

        #endregion "Internal events tracking"

        #region "Functionality implementation"

        private void SynchronizeChildMatrices()
        {
            if (_childMatrices != null && !_matrix.IsIdentity)
            {
                _updatingChildren = true;
                try
                {
                    for (int i = 0; i < _children.Count; i++)
                    {
                        System.Drawing.Drawing2D.Matrix m = (System.Drawing.Drawing2D.Matrix)_childMatrices[i].Clone();
                        m.Multiply(_matrix, System.Drawing.Drawing2D.MatrixOrder.Append);
                        _children[i].Transform = m;
                    }
                }
                finally
                {
                    _updatingChildren = false;
                }
            }
        }

        private void UpdateBaseRectangle()
        {
            if (_children.Count > 0)
            {
                _baseRect = VObjectsUtils.GetBoundingRectangle(_children[0].GetVObjectBounds(), _children[0].Transform);
                for (int i = 1; i < _children.Count; i++)
                    _baseRect = System.Drawing.RectangleF.Union(_baseRect, VObjectsUtils.GetBoundingRectangle(_children[i].GetVObjectBounds(), _children[i].Transform));
            }
        }

        private void UpdateChildMatrices()
        {
            if (_childMatrices != null)
            {
                for (int i = 0; i < _childMatrices.Length; i++)
                    _childMatrices[i].Dispose();
            }

            _childMatrices = new System.Drawing.Drawing2D.Matrix[_children.Count];
            for (int i = 0; i < _children.Count; i++)
                _childMatrices[i] = _children[i].Transform.Clone();
        }

        private void UpdateControlPointsState()
        {
            bool enableResize = true, enableSkew = true, enableRotate = true, enableDrag = true;
            ResizeMode resizeMode = ResizeMode.Arbitrary;

            foreach (IVObject obj in _children)
            {
                IControlPointsProvider icpp = obj as IControlPointsProvider;
                if (icpp != null)
                {
                    // Updating resize options
                    IVObjectAction action = icpp.SupportedActions[VObjectAction.Resize];
                    if (_multipleVObjectsTransformationEnabled && action != null)
                    {
                        enableResize &= action.Enabled;

                        if (((ResizeVObjectAction)action).ResizeMode == ResizeMode.None)
                            resizeMode = ResizeMode.None;
                        if (resizeMode != ResizeMode.None && ((ResizeVObjectAction)action).ResizeMode == ResizeMode.Proportional)
                            resizeMode = ResizeMode.Proportional;
                    }
                    else
                        enableResize = false;

                    // And skew options
                    action = icpp.SupportedActions[VObjectAction.Skew];
                    if (_multipleVObjectsTransformationEnabled && action != null)
                        enableSkew &= action.Enabled;
                    else
                        enableSkew = false;

                    // And rotations options
                    action = icpp.SupportedActions[VObjectAction.Rotate];
                    if (_multipleVObjectsTransformationEnabled && action != null)
                        enableRotate &= action.Enabled;
                    else
                        enableRotate = false;

                    // And rotations options
                    action = icpp.SupportedActions[VObjectAction.Drag];
                    if (action != null)
                        enableDrag &= action.Enabled;
                    else
                        enableDrag = false;
                }
                else
                {
                    enableResize = false;
                    enableSkew = false;
                    enableRotate = false;
                }
            }

            this.SupportedActions[VObjectAction.Rotate].Enabled = enableRotate;
            this.SupportedActions[VObjectAction.Skew].Enabled = enableSkew;
            this.SupportedActions[VObjectAction.Resize].Enabled = enableResize;
            this.SupportedActions[VObjectAction.Drag].Enabled = enableDrag;
            ((ResizeVObjectAction)this.SupportedActions[VObjectAction.Resize]).ResizeMode = resizeMode;
        }

        internal void EnableChangesTracking(bool enable)
        {
            _children.VObjectAdded -= new VObjectEventHandler(ObjectAddedHandler);
            _children.VObjectRemoved -= new VObjectEventHandler(ObjectRemovedHandler);

            foreach (IVObject obj in _children)
                obj.Changed -= new System.EventHandler(VObjectChangedHandler);

            if (enable)
            {
                foreach (IVObject obj in _children)
                    obj.Changed += new System.EventHandler(VObjectChangedHandler);

                _children.VObjectAdded += new VObjectEventHandler(ObjectAddedHandler);
                _children.VObjectRemoved += new VObjectEventHandler(ObjectRemovedHandler);
            }
        }

        #endregion "Functionality implementation"

        #region "IBaseRectangle interface implementation"

        public override System.Drawing.RectangleF GetVObjectBounds()
        {
            return _baseRect;
        }

        public override System.Drawing.Drawing2D.Matrix Transform
        {
            get
            {
                return _matrix;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _matrix = value;
            }
        }

        #endregion "IBaseRectangle interface implementation"

        #region IVObject interface implementation

        public override void Draw(System.Drawing.Rectangle renderingRect, System.Drawing.Graphics g, ICoordinateMapper coordinateMapper)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");
            if (coordinateMapper == null)
                throw new System.ArgumentNullException("coordinateMapper");

            SynchronizeChildMatrices();
            for (int i = 0; i < _children.Count; i++)
                _children[i].Draw(renderingRect, g, coordinateMapper);
        }

        public override bool HitTest(System.Drawing.PointF point, float precisionDelta)
        {
            bool result = false;
            for (int i = 0; !result && i < _children.Count; i++)
                result = _children[i].HitTest(point, precisionDelta);

            return result;
        }

        public override System.Drawing.RectangleF GetTransformedVObjectBounds()
        {
            if (_children.Count == 0)
                return System.Drawing.RectangleF.Empty;

            SynchronizeChildMatrices();

            System.Drawing.RectangleF result = _children[0].GetTransformedVObjectBounds();
            for (int i = 1; i < _children.Count; i++)
                result = System.Drawing.RectangleF.Union(result, _children[i].GetTransformedVObjectBounds());

            return result;
        }

        public override IDesigner Designer
        {
            get
            {
                if (_designer == null)
                    _designer = new CompositeVObjectEditDesigner(this);

                return _designer;
            }
        }

        public override VObjectDrawMode DrawMode
        {
            get
            {
                return base.DrawMode;
            }
            set
            {
                base.DrawMode = value;
                foreach (IVObject child in _children)
                    child.DrawMode = value;
            }
        }

        public override void Update()
        {
            SynchronizeChildMatrices();
            base.Update();
        }

        #endregion IVObject interface implementation

        #region "Serialization support"

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new System.NotImplementedException();
        }

        #endregion "Serialization support"

        #region "Trivial properties of the object"

        public VObjectCollection Children
        {
            get
            {
                return _children;
            }
        }

        internal bool MultipleVObjectsTransformationEnabled
        {
            get
            {
                return _multipleVObjectsTransformationEnabled;
            }
            set
            {
                _multipleVObjectsTransformationEnabled = value;
                UpdateControlPointsState();
            }
        }

        #endregion "Trivial properties of the object"
    }
}