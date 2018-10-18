// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Layer class. Provides functionality for grouping vector objects.
    /// </summary>
    [System.Serializable]
    public class Layer :
        System.Runtime.Serialization.ISerializable,
        System.Runtime.Serialization.IDeserializationCallback
    {
        #region "Construction / destruction"

        public Layer(string name)
        {
            if (name == null)
                throw new System.ArgumentNullException("name");

            _visible = true;
            _name = name;

            _objects = new VObjectCollection();
            _objects.VObjectAdding += new VObjectEventHandler(ObjectAddingHandler);
            _objects.VObjectAdded += new VObjectEventHandler(ObjectAddedHandler);
            _objects.VObjectRemoving += new VObjectEventHandler(ObjectRemovingHandler);
            _objects.VObjectRemoved += new VObjectEventHandler(ObjectRemovedHandler);
        }

        protected Layer(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _objects = (VObjectCollection)info.GetValue(SerializationNames.LayerObjects, typeof(VObjectCollection));
            _objects.VObjectAdding += new VObjectEventHandler(ObjectAddingHandler);
            _objects.VObjectAdded += new VObjectEventHandler(ObjectAddedHandler);
            _objects.VObjectRemoving += new VObjectEventHandler(ObjectRemovingHandler);
            _objects.VObjectRemoved += new VObjectEventHandler(ObjectRemovedHandler);

            _name = info.GetString(SerializationNames.Name);
            _locked = info.GetBoolean(SerializationNames.Locked);
            _visible = info.GetBoolean(SerializationNames.LayerVisible);
        }

        #endregion "Construction / destruction"

        #region Layer Members

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnChanged(new LayerChangedEventArgs(this, null, LayerChangeType.VisibilityChanged));
                }
            }
        }

        public bool Locked
        {
            get
            {
                return _locked;
            }
            set
            {
                if (_locked != value)
                {
                    _locked = value;
                    OnChanged(new LayerChangedEventArgs(this, null, LayerChangeType.LockStatusChanged));
                }
            }
        }

        public VObjectCollection VObjects
        {
            get
            {
                return _objects;
            }
        }

        /// <summary>
        /// Finds the top-most object of the layer's object located at a specified coordinate.
        /// </summary>
        /// <param name="point">Location for search.</param>
        /// <param name="precisionDelta">Search precision.</param>
        /// <returns>Returns the object when successful, otherwise null.</returns>
        public IVObject Find(System.Drawing.PointF point, float precisionDelta)
        {
            IVObject result = null;

            for (int i = _objects.Count - 1; i >= 0; i--)
                if (_objects[i].HitTest(point, precisionDelta))
                {
                    result = _objects[i];
                    break;
                }

            return result;
        }

        /// <summary>
        /// Finds all objects of the layer contained by specified rectangular area.
        /// </summary>
        /// <param name="area">The area for search.</param>
        /// <returns>Returns array of the objects which are located inside specified area.</returns>
        public IVObject[] Find(System.Drawing.RectangleF area, bool includeLockedObjects)
        {
            System.Collections.ArrayList acc = new System.Collections.ArrayList(16);
            for (int i = 0; i < _objects.Count; i++)
            {
                System.Drawing.RectangleF bounds = _objects[i].GetTransformedVObjectBounds();
                if (area.Contains(bounds) && (!_objects[i].Locked || includeLockedObjects))
                    acc.Add(_objects[i]);
            }

            return (IVObject[])acc.ToArray(typeof(IVObject));
        }

        public void SwapObjects(int index0, int index1)
        {
            if (_locked)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotChangeLockedLayer"));
            if (index0 == index1)
                return;

            _objects.Swap(index0, index1);

            OnChanged(new LayerChangedEventArgs(this, _objects[index0], LayerChangeType.ObjectZOrderChanged));
            OnChanged(new LayerChangedEventArgs(this, _objects[index1], LayerChangeType.ObjectZOrderChanged));
        }

        public void SwapObjects(IVObject obj0, IVObject obj1)
        {
            int index0, index1;
            index0 = _objects.IndexOf(obj0);
            index1 = _objects.IndexOf(obj1);

            if (index0 == -1)
                throw new System.ArgumentException(StringResources.GetString("ExStrLayerDoesntContainObject"), "obj0");
            if (index1 == -1)
                throw new System.ArgumentException(StringResources.GetString("ExStrLayerDoesntContainObject"), "obj0");

            SwapObjects(index0, index1);
        }

        #endregion Layer Members

        #region "LayerChanged event & protected method"

        protected virtual void OnChanged(LayerChangedEventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        public event LayerChangedEventHandler Changed;

        #endregion "LayerChanged event & protected method"

        #region "VObjects events tracking"

        private void ObjectAddingHandler(object sender, VObjectEventArgs e)
        {
            if (_locked)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotChangeLockedLayer"));
        }

        private void ObjectAddedHandler(object sender, VObjectEventArgs e)
        {
            e.VObject.Changed += new System.EventHandler(VObjectChangedHandler);
            OnChanged(new LayerChangedEventArgs(this, e.VObject, LayerChangeType.ObjectAdded));
        }

        private void ObjectRemovingHandler(object sender, VObjectEventArgs e)
        {
            if (_locked)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrCannotChangeLockedLayer"));
        }

        private void ObjectRemovedHandler(object sender, VObjectEventArgs e)
        {
            e.VObject.Changed -= new System.EventHandler(VObjectChangedHandler);
            OnChanged(new LayerChangedEventArgs(this, e.VObject, LayerChangeType.ObjectRemoved));
        }

        private void VObjectChangedHandler(object sender, System.EventArgs e)
        {
            OnChanged(new LayerChangedEventArgs(this, (IVObject)sender, LayerChangeType.ObjectChanged));
        }

        #endregion "VObjects events tracking"

        #region "Serialization implementation"

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue(SerializationNames.Name, _name);
            info.AddValue(SerializationNames.Locked, _locked);
            info.AddValue(SerializationNames.LayerVisible, _visible);

            VObjectCollection serializableObjects = new VObjectCollection();
            foreach (IVObject obj in _objects)
                if (obj.GetType().IsSerializable)
                    serializableObjects.Add(obj);

            info.AddValue(SerializationNames.LayerObjects, serializableObjects);
        }

        void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender)
        {
            foreach (IVObject obj in _objects)
                ObjectAddedHandler(null, new VObjectEventArgs(obj));
        }

        public void Serialize(System.IO.Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException("stream");
            if (!stream.CanWrite)
                throw new System.ArgumentException(StringResources.GetString("ExStrDstStreamShouldSupportWriting"), "stream");

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Binder = new VObjectSerializationBinder();
            formatter.Serialize(stream, this);
            stream.Close();
        }

        public void Deserialize(System.IO.Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new System.ArgumentException(StringResources.GetString("ExStrSrcStreamShouldSupportReading"), "stream");

            _objects.Clear();

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Binder = new VObjectSerializationBinder();
            Layer tmpLayer = (Layer)formatter.Deserialize(stream);

            _locked = false;
            for (int i = 0; i < tmpLayer.VObjects.Count; i++)
                _objects.Add(tmpLayer.VObjects[i]);

            this.Name = tmpLayer.Name;
            this.Visible = tmpLayer.Visible;
            this.Locked = tmpLayer.Locked;
        }

        #endregion "Serialization implementation"

        #region "Member variables"

        private string _name;
        private bool _visible;
        private bool _locked;
        private VObjectCollection _objects;

        #endregion "Member variables"
    }

    #region "LayerChanged event args & handler"

    public enum LayerChangeType
    {
        ObjectAdded,
        ObjectRemoved,
        ObjectChanged,
        ObjectZOrderChanged,
        VisibilityChanged,
        LockStatusChanged
    }

    public class LayerChangedEventArgs : LayerEventArgs
    {
        public LayerChangedEventArgs(Layer layer, IVObject changedObj, LayerChangeType type)
            : base(layer)
        {
            _obj = changedObj;
            _type = type;
        }

        public IVObject VObject
        {
            get
            {
                return _obj;
            }
        }

        public LayerChangeType ChangeType
        {
            get
            {
                return _type;
            }
        }

        private LayerChangeType _type;
        private IVObject _obj;
    }

    public delegate void LayerChangedEventHandler(object sender, LayerChangedEventArgs e);

    #endregion "LayerChanged event args & handler"
}