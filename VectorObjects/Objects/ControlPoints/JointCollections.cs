// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Control point collection for JointControlPointsProvider.
    /// </summary>
    internal class JointControlPointCollection : IControlPointCollection
    {
        #region "Construction / destruction"

        public JointControlPointCollection(JointControlPointsProvider host)
        {
            if (host == null)
                throw new System.ArgumentNullException("host");

            _host = host;
        }

        #endregion "Construction / destruction"

        #region ControlPointCollection Members

        public ControlPoint this[int index]
        {
            get
            {
                int translatedIndex;
                IControlPointsProvider provider = _host.GetProviderByIndex(index, out translatedIndex);
                return provider.ControlPoints[translatedIndex];
            }
        }

        #endregion ControlPointCollection Members

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
                int result = 0;
                foreach (IControlPointsProvider provider in _host.ControlPointsProviders)
                    result += provider.ControlPoints.Count;

                return result;
            }
        }

        public void CopyTo(System.Array array, int index)
        {
            int startIndex = index;
            foreach (IControlPointsProvider provider in _host.ControlPointsProviders)
            {
                provider.ControlPoints.CopyTo(array, startIndex);
                startIndex += provider.ControlPoints.Count;
            }
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
            return new JointEnumerator(_host.ControlPointsProviders);
        }

        #endregion IEnumerable Members

        #region "Member variables"

        private JointControlPointsProvider _host;

        #endregion "Member variables"
    }

    internal class JointEnumerator : System.Collections.IEnumerator
    {
        public JointEnumerator(System.Collections.ArrayList srcCollections)
        {
            _srcCollections = srcCollections;
            Reset();
        }

        public object Current
        {
            get
            {
                return _curEnumerator.Current;
            }
        }

        public bool MoveNext()
        {
            if (_curEnumerator.MoveNext())
                return true;

            while (true)
            {
                _curCollectionIndex++;
                _curEnumerator = GetEnumerator(_srcCollections[_curCollectionIndex]);

                if (_curEnumerator.MoveNext())
                    return true;
                if (_curCollectionIndex == _srcCollections.Count - 1)
                    break;
            }

            return false;
        }

        public void Reset()
        {
            _curCollectionIndex = 0;
            _curEnumerator = ((System.Collections.IEnumerable)_srcCollections[_curCollectionIndex]).GetEnumerator();
        }

        protected virtual System.Collections.IEnumerator GetEnumerator(object collectionItem)
        {
            return ((System.Collections.IEnumerable)collectionItem).GetEnumerator();
        }

        #region "Member variables"

        private System.Collections.ArrayList _srcCollections;
        private int _curCollectionIndex;
        private System.Collections.IEnumerator _curEnumerator;

        #endregion "Member variables"
    }

    public class JointVObjectActionCollection : IVObjectActionCollection
    {
        public JointVObjectActionCollection(JointControlPointsProvider host)
        {
            if (host == null)
                throw new System.ArgumentNullException("host");

            _host = host;
        }

        #region IVObjectActionCollection Members

        public IVObjectAction this[int actionId]
        {
            get
            {
                IVObjectAction result = null;
                foreach (IControlPointsProvider provider in _host.ControlPointsProviders)
                    if (provider.SupportedActions.Contains(actionId))
                    {
                        result = provider.SupportedActions[actionId];
                        break;
                    }

                return result;
            }
        }

        public bool Contains(int actionId)
        {
            foreach (IControlPointsProvider provider in _host.ControlPointsProviders)
                if (provider.SupportedActions.Contains(actionId))
                    return true;

            return false;
        }

        #endregion IVObjectActionCollection Members

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
                int result = 0;
                foreach (IControlPointsProvider provider in _host.ControlPointsProviders)
                    result += provider.SupportedActions.Count;

                return result;
            }
        }

        public void CopyTo(System.Array array, int index)
        {
            throw new System.NotImplementedException();
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
            return new JointVObjectActionCollectionEnumerator(_host.ControlPointsProviders);
        }

        #endregion IEnumerable Members

        #region "Member variables"

        private JointControlPointsProvider _host;

        #endregion "Member variables"
    }

    internal class JointVObjectActionCollectionEnumerator : JointEnumerator
    {
        public JointVObjectActionCollectionEnumerator(System.Collections.ArrayList srcCollections)
            : base(srcCollections)
        {
        }

        protected override System.Collections.IEnumerator GetEnumerator(object collectionItem)
        {
            return ((IControlPointsProvider)collectionItem).SupportedActions.GetEnumerator();
        }
    }
}