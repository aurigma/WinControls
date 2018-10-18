// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Layer collection.
    /// </summary>
    [System.Serializable]
    public class LayerCollection : System.Collections.IList, System.Runtime.Serialization.ISerializable
    {
        #region "Construction / destruction"

        public LayerCollection()
        {
            _layers = new System.Collections.ArrayList();
        }

        protected LayerCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _layers = new System.Collections.ArrayList();

            object[] layers = BinarySerializer.DeserializeObjectArray((byte[])info.GetValue(SerializationNames.LayerCollection, typeof(byte[])));
            foreach (Layer layer in layers)
                Add(layer);
        }

        #endregion "Construction / destruction"

        #region "ICollection implementation"

        public int Count
        {
            get
            {
                return _layers.Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        void System.Collections.ICollection.CopyTo(System.Array array, int index)
        {
            _layers.CopyTo(array, index);
        }

        #endregion "ICollection implementation"

        #region "IList implementation"

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return _layers[index];
            }
            set
            {
                if (value as Layer == null)
                    throw new System.ArgumentException(StringResources.GetString("ExStrIncorrectType"), "value");

                _layers[index] = value;
            }
        }

        public int Add(object value)
        {
            if (value == null)
                throw new System.ArgumentNullException("value");
            if (!(value is Layer))
                throw new System.ArgumentException(StringResources.GetString("ExStrIncorrectType"), "value");

            return _layers.Add(value);
        }

        public void Clear()
        {
            System.Array removedLayers = _layers.ToArray();

            _layers.Clear();

            int i = 0;
            foreach (Layer layer in removedLayers)
                OnLayerRemoved(new LayerRemovedEventArgs(layer, i++));
        }

        public bool Contains(object value)
        {
            return _layers.Contains(value);
        }

        public int IndexOf(object value)
        {
            return _layers.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            if (!(value is Layer))
                throw new System.ArgumentException(StringResources.GetString("ExStrIncorrectType"), "value");

            _layers.Insert(index, value);
        }

        public void Remove(object value)
        {
            this.Remove((Layer)value);
        }

        public void RemoveAt(int index)
        {
            Layer layer = (Layer)_layers[index];
            _layers.RemoveAt(index);
            OnLayerRemoved(new LayerRemovedEventArgs(layer, index));
        }

        #endregion "IList implementation"

        #region "IEnumerable implementation"

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _layers.GetEnumerator();
        }

        #endregion "IEnumerable implementation"

        #region "Serialization support"

        private object[] GetSerializableLayers()
        {
            foreach (Layer layer in _layers)
                if (!layer.GetType().IsSerializable)
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrLayersShouldBeSerializable"));

            return (object[])_layers.ToArray(typeof(object));
        }

        public void Serialize(System.IO.Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException("stream");
            if (!stream.CanWrite)
                throw new System.ArgumentException(StringResources.GetString("ExStrDstStreamShouldSupportWriting"), "stream");

            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(stream);
            try
            {
                BinarySerializer.Serialize(bw, GetSerializableLayers());
            }
            finally
            {
                bw.Close();
            }
        }

        public void Deserialize(System.IO.Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new System.ArgumentException(StringResources.GetString("ExStrSrcStreamShouldSupportReading"), "stream");

            System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
            object[] layers = BinarySerializer.DeserializeObjectArray(br);

            Clear();
            foreach (Layer layer in layers)
                Add(layer);
        }

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue(SerializationNames.LayerCollection, BinarySerializer.Serialize(GetSerializableLayers()));
        }

        #endregion "Serialization support"

        public int Add(Layer layer)
        {
            if (layer == null)
                throw new System.ArgumentNullException("layer");
            if (_layers.Contains(layer))
                throw new System.ArgumentException(StringResources.GetString("ExStrValueAlreadyPresentedInCollection"), "layer");

            int result = _layers.Add(layer);
            OnLayerAdded(new LayerEventArgs(layer));
            return result;
        }

        public void Remove(Layer layer)
        {
            if (layer == null)
                throw new System.ArgumentNullException("layer");

            int index = _layers.IndexOf(layer);
            if (index == -1)
                throw new System.ArgumentException(StringResources.GetString("ExStrCannotFindSpecifiedValue"), "layer");

            _layers.Remove(layer);
            OnLayerRemoved(new LayerRemovedEventArgs(layer, index));
        }

        public bool Contains(Layer layer)
        {
            return _layers.Contains(layer);
        }

        public int IndexOf(Layer layer)
        {
            return _layers.IndexOf(layer);
        }

        public void Insert(int index, Layer layer)
        {
            if (_layers.Contains(layer))
                throw new System.ArgumentException(StringResources.GetString("ExStrValueAlreadyPresentedInCollection"), "layer");

            _layers.Insert(index, layer);
            OnLayerAdded(new LayerEventArgs(layer));
        }

        public Layer this[int index]
        {
            get
            {
                return (Layer)_layers[index];
            }
            set
            {
                if (_layers.Contains(value))
                    throw new System.ArgumentException(StringResources.GetString("ExStrValueAlreadyPresentedInCollection"), "value");

                Layer removedLayer = (Layer)_layers[index];
                _layers[index] = value;

                OnLayerRemoved(new LayerRemovedEventArgs(removedLayer, index));
                OnLayerAdded(new LayerEventArgs(value));
            }
        }

        public void CopyTo(Layer[] array, int index)
        {
            _layers.CopyTo(array, index);
        }

        #region "Events & Events methods"

        protected virtual void OnLayerAdded(LayerEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            e.Layer.Changed += new LayerChangedEventHandler(LayerChangedHandler);

            if (LayerAdded != null)
                LayerAdded(this, e);
        }

        private void LayerChangedHandler(object sender, LayerChangedEventArgs e)
        {
            OnLayerChanged(e);
        }

        protected virtual void OnLayerRemoved(LayerRemovedEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            e.Layer.Changed -= new LayerChangedEventHandler(LayerChangedHandler);

            if (LayerRemoved != null)
                LayerRemoved(this, e);
        }

        protected virtual void OnLayerChanged(LayerChangedEventArgs e)
        {
            if (LayerChanged != null)
                LayerChanged(this, e);
        }

        public event LayerEventHandler LayerAdded;

        public event LayerRemovedEventHandler LayerRemoved;

        public event LayerChangedEventHandler LayerChanged;

        #endregion "Events & Events methods"

        #region "Member variables"

        private System.Collections.ArrayList _layers;

        #endregion "Member variables"
    }

    public class LayerEventArgs : System.EventArgs
    {
        public LayerEventArgs(Layer layer)
        {
            _layer = layer;
        }

        public Layer Layer
        {
            get
            {
                return _layer;
            }
        }

        private Layer _layer;
    }

    public class LayerRemovedEventArgs : LayerEventArgs
    {
        public LayerRemovedEventArgs(Layer layer, int index)
            : base(layer)
        {
            _index = index;
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        private int _index;
    }

    public delegate void LayerEventHandler(object sender, LayerEventArgs e);

    public delegate void LayerRemovedEventHandler(object sender, LayerRemovedEventArgs e);
}