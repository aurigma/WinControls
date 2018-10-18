// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    [System.Serializable]
    public class VObjectCollection : System.Collections.IList, System.IDisposable
    {
        #region "Construction / destruction"

        internal VObjectCollection()
        {
            _objects = new System.Collections.ArrayList(32);
            _objectHash = new System.Collections.Hashtable(32);
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (IVObject obj in _objects)
                {
                    System.IDisposable disposableObj = obj as System.IDisposable;
                    if (disposableObj != null)
                        disposableObj.Dispose();
                }

                _objects.Clear();
                _objectHash.Clear();
            }
        }

        #endregion "Construction / destruction"

        #region ICollection Members

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public int Count
        {
            get
            {
                return _objects.Count;
            }
        }

        public void CopyTo(System.Array array, int index)
        {
            _objects.CopyTo(array, index);
        }

        public void CopyTo(IVObject[] array, int index)
        {
            _objects.CopyTo(array, index);
        }

        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion ICollection Members

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region IList Members

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
                return this[index];
            }
            set
            {
                if (!(value is IVObject))
                    throw new System.ArgumentException(StringResources.GetString("ExStrIncorrectType"), "value");

                this[index] = (IVObject)value;
            }
        }

        public bool Contains(object value)
        {
            return Contains((IVObject)value);
        }

        public bool Contains(IVObject value)
        {
            if (value == null)
                throw new System.ArgumentNullException("value");

            return _objectHash.ContainsKey(value);
        }

        public int IndexOf(object value)
        {
            return IndexOf((IVObject)value);
        }

        public int Add(object value)
        {
            return Add((IVObject)value);
        }

        public void Insert(int index, object value)
        {
            Insert(index, (IVObject)value);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _objects.Count)
                throw new System.ArgumentOutOfRangeException("index");

            IVObject obj = (IVObject)_objects[index];
            OnVObjectRemoving(obj);
            _objects.RemoveAt(index);
            _objectHash.Remove(obj);
            OnVObjectRemoved(obj);
        }

        public void Remove(object value)
        {
            Remove((IVObject)value);
        }

        public void Clear()
        {
            for (int i = _objects.Count - 1; i >= 0; i--)
            {
                IVObject obj = (IVObject)_objects[i];
                OnVObjectRemoving(obj);
                _objectHash.Remove(obj);
                _objects.RemoveAt(i);
                OnVObjectRemoved(obj);
            }
        }

        #endregion IList Members

        #region "VObject specific add/remove/etc methods"

        public IVObject this[int index]
        {
            get
            {
                return (IVObject)_objects[index];
            }
            set
            {
                if (index < 0 || index > _objects.Count)
                    throw new System.ArgumentOutOfRangeException("index");

                IVObject obj = (IVObject)_objects[index];
                OnVObjectRemoving(obj);
                OnVObjectAdding(value);

                _objects[index] = value;
                _objectHash.Remove(obj);
                _objectHash.Add(value, -1/*dummy*/);

                OnVObjectRemoved(obj);
                OnVObjectAdded(value);
            }
        }

        public int IndexOf(IVObject value)
        {
            return _objects.IndexOf(value);
        }

        public int Add(IVObject value)
        {
            if (value == null)
                throw new System.ArgumentNullException("value");

            int result = -1;
            OnVObjectAdding(value);

            result = _objects.Add(value);
            _objectHash.Add(value, -1/*dummy*/);

            OnVObjectAdded(value);
            return result;
        }

        public void Insert(int index, IVObject value)
        {
            if (index < 0 || index > _objects.Count)
                throw new System.ArgumentOutOfRangeException("index");
            if (value == null)
                throw new System.ArgumentNullException("value");

            OnVObjectAdding(value);

            _objects.Insert(index, value);
            _objectHash.Add(value, -1/*dummy*/);

            OnVObjectAdded(value);
        }

        public void Remove(IVObject value)
        {
            if (!Contains(value))
                return;

            OnVObjectRemoving(value);

            _objects.Remove(value);
            _objectHash.Remove(value);

            OnVObjectRemoved(value);
        }

        public IVObject[] ToArray()
        {
            return (IVObject[])_objects.ToArray(typeof(IVObject));
        }

        #endregion "VObject specific add/remove/etc methods"

        #region "Internal methods"

        internal void Swap(int index0, int index1)
        {
            if (index0 < 0 || index0 >= _objects.Count)
                throw new System.ArgumentOutOfRangeException("index0");
            if (index1 < 0 || index1 >= _objects.Count)
                throw new System.ArgumentOutOfRangeException("index1");
            if (index0 == index1)
                return;

            object tmp = _objects[index0];
            _objects[index0] = _objects[index1];
            _objects[index1] = tmp;
        }

        internal void Swap(IVObject obj0, IVObject obj1)
        {
            int index0, index1;
            index0 = _objects.IndexOf(obj0);
            index1 = _objects.IndexOf(obj1);

            if (index0 == -1)
                throw new System.ArgumentException(StringResources.GetString("ExStrLayerDoesntContainObject"), "obj0");
            if (index1 == -1)
                throw new System.ArgumentException(StringResources.GetString("ExStrLayerDoesntContainObject"), "obj1");

            Swap(index0, index1);
        }

        #endregion "Internal methods"

        #region "Protected events methods"

        protected void OnVObjectAdding(IVObject obj)
        {
            if (VObjectAdding != null)
                VObjectAdding(this, new VObjectEventArgs(obj));
        }

        protected void OnVObjectAdded(IVObject obj)
        {
            if (VObjectAdded != null)
                VObjectAdded(this, new VObjectEventArgs(obj));
        }

        protected void OnVObjectRemoving(IVObject obj)
        {
            if (VObjectRemoving != null)
                VObjectRemoving(this, new VObjectEventArgs(obj));
        }

        protected void OnVObjectRemoved(IVObject obj)
        {
            if (VObjectRemoved != null)
                VObjectRemoved(this, new VObjectEventArgs(obj));
        }

        #endregion "Protected events methods"

        #region "Internal events declaration"

        public event VObjectEventHandler VObjectAdding;

        public event VObjectEventHandler VObjectAdded;

        public event VObjectEventHandler VObjectRemoving;

        public event VObjectEventHandler VObjectRemoved;

        #endregion "Internal events declaration"

        #region "Member variables"

        private System.Collections.ArrayList _objects;
        private System.Collections.Hashtable _objectHash;

        #endregion "Member variables"
    }

    #region "VObject events args & handlers"

    public class VObjectEventArgs : System.EventArgs
    {
        public VObjectEventArgs(IVObject obj)
        {
            _object = obj;
        }

        public IVObject VObject
        {
            get
            {
                return _object;
            }
        }

        private IVObject _object;
    }

    public delegate void VObjectEventHandler(object sender, VObjectEventArgs e);

    #endregion "VObject events args & handlers"
}