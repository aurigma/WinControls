// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    public sealed class VObjectActionCollection : IVObjectActionCollection
    {
        public VObjectActionCollection(IVObjectAction[] actions)
        {
            if (actions == null)
                throw new System.ArgumentNullException("actions");

            _content = new System.Collections.Hashtable(actions.Length);

            foreach (IVObjectAction action in actions)
                _content.Add(action.Id, action);
        }

        public IVObjectAction this[int actionId]
        {
            get
            {
                return (IVObjectAction)_content[actionId];
            }
        }

        public bool Contains(int actionId)
        {
            return _content.ContainsKey(actionId);
        }

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
                return _content.Count;
            }
        }

        public void CopyTo(System.Array array, int index)
        {
            _content.CopyTo(array, index);
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
            return _content.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region "Member variables"

        private System.Collections.Hashtable _content;

        #endregion "Member variables"
    }
}