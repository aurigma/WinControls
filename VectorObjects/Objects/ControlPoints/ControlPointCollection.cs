// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Control points collection. Hm, nothing else to say...
    /// </summary>
    public class ControlPointCollection : IControlPointCollection
    {
        public ControlPointCollection(ControlPoint[] points)
        {
            if (points == null)
                throw new System.ArgumentNullException("points");
            if (points.Length < 1)
                throw new System.ArgumentException(StringResources.GetString("ExStrArrayZeroLengthError"), "points");

            _points = points;
        }

        #region ControlPointCollection Members

        public ControlPoint this[int index]
        {
            get
            {
                return _points[index];
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
                return _points.Length;
            }
        }

        public void CopyTo(System.Array array, int index)
        {
            _points.CopyTo(array, index);
        }

        public void CopyTo(ControlPoint[] array, int index)
        {
            _points.CopyTo(array, index);
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
            return _points.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region "Member variables"

        private ControlPoint[] _points;

        #endregion "Member variables"
    }
}