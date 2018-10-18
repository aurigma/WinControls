// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;
using System.Globalization;

namespace Aurigma.GraphicsMill.WinControls
{
    #region "IComparer implementations"

    /// <summary>
    /// IComparer interface implementation, is needed to sort items descending according to it's indices in specified collection.
    /// Class is used in Remove(...) method of IListItemCollection & IListColumnCollection.
    /// </summary>
    internal class IListIndexComparer : IComparer
    {
        /// <summary>
        /// Initializes new instance of the IListIndexComparer class.
        /// </summary>
        /// <param name="collection">Collection for obtaining indices of elements to compare.</param>
        public IListIndexComparer(IList collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _collection = collection;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">First argument.</param>
        /// <param name="y">Second argument.</param>
        /// <returns>a value indicating whether one is less than, equal to or greater than the other.</returns>
        public int Compare(object x, object y)
        {
            int xIndex = _collection.IndexOf(x),
                yIndex = _collection.IndexOf(y);

            if (xIndex > yIndex)
                return -1;
            else if (xIndex < yIndex)
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Stores reference to a collection for obtaining indices of elements to compare.
        /// </summary>
        private IList _collection;
    }

    /// <summary>
    /// IComparer implementation. Sorts item according to specified TextInfoId value.
    /// </summary>
    internal class TextInfoIdComparer : IComparer
    {
        /// <summary>
        /// Initializes new instance of the TextInfoIdComparer class.
        /// </summary>
        /// <param name="textInfoId">Type of the item's info that should be used during sort.</param>
        /// <param name="sortAscending">If the value is true comparer performs ascending sort comparison; descending otherwise.</param>
        public TextInfoIdComparer(int textInfoId, bool sortAscending)
        {
            _textInfoId = textInfoId;
            _sortAscending = sortAscending;
        }

        /// <summary>
        /// Converts string containing file size info to int.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>integer value of the file size string.</returns>
        private static int StringToInt(string str)
        {
            if (str == null || str.Length == 0)
                return -1;

            int i = 0;
            while (Char.IsDigit(str[i]) || Char.IsWhiteSpace(str[i])) i++;
            return Int32.Parse(str.Substring(0, i), NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">First argument.</param>
        /// <param name="y">Second argument.</param>
        /// <returns>a value indicating whether one is less than, equal to or greater than the other.</returns>
        public int Compare(object obj0, object obj1)
        {
            IListItem item0 = (IListItem)obj0,
                      item1 = (IListItem)obj1;

            // If both objects are ThumbnailListItems - check for file/folder comparison.
            // Folders always should be first in sort order.
            ThumbnailListItem thumbnailListItem0 = obj0 as ThumbnailListItem,
                              thumbnailListItem1 = obj1 as ThumbnailListItem;
            if (thumbnailListItem0 != null && thumbnailListItem1 != null)
            {
                if (thumbnailListItem0.Pidl.Type == PidlType.File && thumbnailListItem1.Pidl.Type == PidlType.Folder)
                    return (_sortAscending ? 1 : -1);
                else if (thumbnailListItem0.Pidl.Type == PidlType.Folder && thumbnailListItem1.Pidl.Type == PidlType.File)
                    return (_sortAscending ? -1 : 1);
            }

            string str0 = item0.GetText(_textInfoId),
                   str1 = item1.GetText(_textInfoId);

            int result;
            if (_textInfoId == ThumbnailListItem.TextInfoIdFileSize)
            {
                try
                {
                    int int0 = StringToInt(str0);
                    int int1 = StringToInt(str1);

                    if (int0 > int1)
                        result = 1;
                    else if (int0 < int1)
                        result = -1;
                    else
                        result = 0;
                }
                catch
                {
                    result = str0.CompareTo(str1);
                }
            }
            else if (_textInfoId == ThumbnailListItem.TextInfoIdCreationDate)
            {
                try
                {
                    System.DateTime date0 = System.DateTime.Parse(str0, CultureInfo.CurrentCulture),
                                    date1 = System.DateTime.Parse(str1, CultureInfo.CurrentCulture);
                    result = date0.CompareTo(date1);
                }
                catch
                {
                    result = str0.CompareTo(str1);
                }
            }
            else
                result = str0.CompareTo(str1);

            if (_sortAscending)
                return result;
            else
                return result * -1;
        }

        /// <summary>
        /// Sort key.
        /// </summary>
        private int _textInfoId;

        /// <summary>
        /// Stores sort order.
        /// </summary>
        private bool _sortAscending;
    }

    #endregion "IComparer implementations"

    /// <summary>
    /// Represents items collection of the ThumbnailListView control.
    /// Note: the collection cannot contain equal objects - you should remove the object from its current position or clone.
    /// </summary>
    [ResDescription("ListItemCollection")]
    public sealed class ListItemCollection : CollectionBase, IList
    {
        #region Construction/Destruction

        /// <summary>
        /// Initializes new instance of the ListItemCollection class.
        /// </summary>
        internal ListItemCollection(ThumbnailListView parentControl)
        {
            if (parentControl == null)
                throw new ArgumentNullException("parentControl");

            _parentControl = parentControl;
        }

        #endregion Construction/Destruction

        #region CollectionBase implementation

        /// <summary>
        /// Collection indexer.
        /// </summary>
        [ResDescription("ListItemCollection_Indexer")]
        public IListItem this[int index]
        {
            get
            {
                return ((IListItem)List[index]);
            }
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        /// <summary>
        /// Adds new item into the collection.
        /// </summary>
        /// <param name="value">Item to add.</param>
        /// <returns>Returns index of the added item.</returns>
        [ResDescription("ListItemCollection_Add0")]
        public int Add(IListItem value)
        {
            if (value != null)
            {
                IListItem[] items = { value };
                return Add(items);
            }
            return -1;
        }

        /// <summary>
        /// Adds array of items into the collection.
        /// </summary>
        /// <param name="items">Array of items to add.</param>
        /// <returns>Returns index of the first added item.</returns>
        [ResDescription("ListItemCollection_Add1")]
        public int Add(IListItem[] items)
        {
            if (items == null)
                return -1;

            int firstIndex = this.Count;
            Insert(firstIndex, items);
            return firstIndex;
        }

        /// <summary>
        /// Searches for the specified item and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="value">The item to locate in the collection.</param>
        /// <returns>Returns index of the first occurrence of specified item object in the collection, otherwise, -1.</returns>
        [ResDescription("ListItemCollection_IndexOf")]
        public int IndexOf(IListItem value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The item to insert.</param>
        [ResDescription("ListItemCollection_Insert0")]
        public void Insert(int index, IListItem value)
        {
            if (value == null)
                return;

            IListItem[] values = { value };
            Insert(index, values);
        }

        /// <summary>
        /// Inserts an array of items into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which items should be inserted.</param>
        /// <param name="items">Array of items to insert.</param>
        [ResDescription("ListItemCollection_Insert1")]
        public void Insert(int index, IListItem[] items)
        {
            if (items == null)
                return;
            if (index < 0 || index > List.Count)
                throw new ArgumentOutOfRangeException("index");

            int currentIndex = index;
            ArrayList itemsToAdd = new ArrayList();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                    continue;

                if (items[i].Parent != null)
                    throw new ArgumentException(StringResources.GetString("ListItemSingleParentException"), "items");

                if (OnItemInserting(items[i], currentIndex))
                {
                    itemsToAdd.Add(items[i]);
                    currentIndex++;
                }
            }

            if (itemsToAdd.Count > 0)
            {
                IListItem[] itemsToAddArray = new IListItem[itemsToAdd.Count];
                for (int i = 0; i < itemsToAdd.Count; i++)
                {
                    itemsToAddArray[i] = (IListItem)itemsToAdd[i];
                    RegisterItem(itemsToAddArray[i]);
                    List.Insert(index + i, itemsToAddArray[i]);
                }
                OnItemsInserted(index, itemsToAddArray);
            }
        }

        /// <summary>
        /// Removes all objects from the collection instance.
        /// </summary>
        [ResDescription("ListItemCollection_Clear")]
        public new void Clear()
        {
            IListItem[] itemsToRemove = ToArray();
            Remove(itemsToRemove);
        }

        /// <summary>
        /// Removes specified item from the collection.
        /// </summary>
        /// <param name="value">The item to remove.</param>
        [ResDescription("ListItemCollection_Remove0")]
        public void Remove(IListItem value)
        {
            if (value == null)
                return;

            IListItem[] values = { value };
            Remove(values);
        }

        /// <summary>
        /// Removes specified items from the collection.
        /// </summary>
        /// <param name="values">Array of items to remove.</param>
        [ResDescription("ListItemCollection_Remove1")]
        public void Remove(IListItem[] values)
        {
            if (values == null || values.Length == 0)
                return;

            ArrayList itemsToRemove = new ArrayList();
            for (int i = 0; i < values.Length; i++)
            {
                IListItem item = (IListItem)values[i];
                if (item != null && OnItemRemoving(item))
                    itemsToRemove.Add(item);
            }

            if (itemsToRemove.Count > 0)
            {
                itemsToRemove.Sort(new IListIndexComparer(this));

                IListItem[] removedItems = new IListItem[itemsToRemove.Count];
                int[] removedItemsIndices = new int[itemsToRemove.Count];

                for (int i = 0; i < itemsToRemove.Count; i++)
                {
                    removedItems[i] = (IListItem)itemsToRemove[i];
                    removedItemsIndices[i] = this.IndexOf(removedItems[i]);
                    List.Remove(removedItems[i]);
                    UnregisterItem(removedItems[i]);
                }

                OnItemsRemoved(removedItems, removedItemsIndices);
                if (this.Count == 0)
                    OnItemsRemovedAll(System.EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        [ResDescription("ListItemCollection_RemoveAt")]
        public new void RemoveAt(int index)
        {
            IListItem item = this[index];
            if (item != null)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Determines whether an element is in the collection.
        /// </summary>
        /// <param name="value">The item to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false.</returns>
        [ResDescription("ListItemCollection_Contains")]
        public bool Contains(IListItem value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from the collection. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(IListItem[] array, int index)
        {
            List.CopyTo(array, index);
        }

        #endregion CollectionBase implementation

        #region Methods

        public void Swap(IListItem item0, IListItem item1)
        {
            if (item0 == null)
                throw new System.ArgumentNullException("item0");
            if (item1 == null)
                throw new System.ArgumentNullException("item1");

            int i0 = this.InnerList.IndexOf(item0),
                i1 = this.InnerList.IndexOf(item0);

            if (i0 < 0)
                throw new System.ArgumentException(StringResources.GetString("CannotFindSpecifiedValueInCollection"), "item0");
            if (i1 < 0)
                throw new System.ArgumentException(StringResources.GetString("CannotFindSpecifiedValueInCollection"), "item1");

            Swap(i0, i1);
        }

        public void Swap(int index0, int index1)
        {
            object obj0 = this.InnerList[index0],
                   obj1 = this.InnerList[index1];

            this.InnerList[index1] = obj0;
            this.InnerList[index0] = obj1;

            _parentControl.UpdateItem(index0);
            _parentControl.UpdateItem(index1);
        }

        public void Swap(IListItem[] items, int destinationIndex)
        {
            if (items == null)
                throw new System.ArgumentNullException("items");
            if (destinationIndex < 0 || destinationIndex > this.InnerList.Count)
                throw new System.ArgumentOutOfRangeException("destinationIndex");

            int[] indexes = new int[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                indexes[i] = this.InnerList.IndexOf(items[i]);
                if (indexes[i] < 0)
                    throw new System.ArgumentException(StringResources.GetString("CannotFindSpecifiedValueInCollection"), "items");
            }

            Swap(indexes, destinationIndex);
        }

        public void Swap(int[] indexes, int destinationIndex)
        {
            if (indexes == null)
                throw new System.ArgumentNullException("indexes");
            if (destinationIndex < 0 || destinationIndex > this.InnerList.Count)
                throw new System.ArgumentOutOfRangeException("destinationIndex");

            int i;
            IListItem[] items = new IListItem[indexes.Length];
            for (i = 0; i < indexes.Length; i++)
                items[i] = (IListItem)this.InnerList[indexes[i]];

            int firstItemForUpdate = destinationIndex, lastItemToUpdate = destinationIndex;
            for (i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] < firstItemForUpdate)
                    firstItemForUpdate = indexes[i];
                if (indexes[i] > lastItemToUpdate)
                    lastItemToUpdate = indexes[i];
            }

            int effectiveDstIndex = destinationIndex;
            for (i = 0; i < items.Length; i++)
            {
                this.InnerList.Remove(items[i]);
                if (destinationIndex > indexes[i])
                    effectiveDstIndex--;
            }

            if (effectiveDstIndex < this.InnerList.Count)
            {
                for (i = 0; i < indexes.Length; i++)
                    this.InnerList.Insert(effectiveDstIndex++, items[i]);
            }
            else
            {
                for (i = 0; i < indexes.Length; i++)
                    this.InnerList.Add(items[i]);
            }

            for (i = firstItemForUpdate; i < lastItemToUpdate; i++)
                _parentControl.UpdateItem(i);

            for (i = 0; i < this.InnerList.Count; i++)
                _parentControl.UpdateItem(i);
        }

        /// <summary>
        /// Copies the elements of the collection to a new array.
        /// </summary>
        /// <returns>New array that contains all items of the collection.</returns>
        [ResDescription("ListItemCollection_ToArray")]
        public IListItem[] ToArray()
        {
            IListItem[] items = new IListItem[Count];

            for (int i = 0; i < Count; i++)
                items[i] = this[i];

            return items;
        }

        /// <summary>
        /// Returns items with specified state value.
        /// </summary>
        [ResDescription("ListItemCollection_GetItemsByState")]
        public IListItem[] GetItemsByState(StateType itemStateType, bool valueToCompare, bool onlyFirst)
        {
            ArrayList interimArray = new ArrayList();
            for (int i = 0; i < this.Count; i++)
            {
                IListItem item = (IListItem)this[i];
                bool isGood = true;
                if ((itemStateType | StateType.Check) != 0 && item.Checked != valueToCompare)
                    isGood = false;

                if ((itemStateType | StateType.Selection) != 0 && item.Selected != valueToCompare)
                    isGood = false;

                if ((itemStateType | StateType.Focus) != 0 && item.Focused != valueToCompare)
                    isGood = false;

                if (isGood)
                {
                    interimArray.Add(item);
                    if (onlyFirst)
                        break;
                }
            }

            IListItem[] result;
            ConvertCollectionToArray(interimArray, out result);
            return result;
        }

        /// <summary>
        /// Sorts elements of the collection using specified IComparer object.
        /// </summary>
        internal void SortInternals(IComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            this.InnerList.Sort(comparer);
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Occurs when new item is adding to the collection.
        /// </summary>
        [ResDescription("ListItemCollection_ItemInserting")]
        public event ItemInsertingEventHandler ItemInserting;

        /// <summary>
        /// Raises ItemInserting event of the collection.
        /// </summary>
        /// <param name="item">New item to add.</param>
        /// <returns>true if event handlers didn't cancel item adding.</returns>
        private bool OnItemInserting(IListItem item, int index)
        {
            if (ItemInserting != null)
            {
                ItemInsertingEventArgs args = new ItemInsertingEventArgs(item, index);
                ItemInserting(this, args);
                return !args.Cancel;
            }

            return true;
        }

        /// <summary>
        /// Occurs when item is removing from the collection.
        /// </summary>
        [ResDescription("ListItemCollection_ItemRemoving")]
        public event ItemRemovingEventHandler ItemRemoving;

        /// <summary>
        /// Raises ItemRemoving event of the collection.
        /// </summary>
        /// <param name="item">The removing item.</param>
        /// <returns>true if event handlers didn't cancel item removing.</returns>
        private bool OnItemRemoving(IListItem item)
        {
            if (ItemRemoving != null)
            {
                ItemActionRequestEventArgs args = new ItemActionRequestEventArgs(item);
                ItemRemoving(this, args);
                return !args.Cancel;
            }

            return true;
        }

        /// <summary>
        /// Occurs when new items have been inserted into the collection.
        /// </summary>
        [ResDescription("ListItemCollection_ItemsInserted")]
        public event ItemsInsertedEventHandler ItemsInserted;

        /// <summary>
        /// Raises ItemsInserted event.
        /// </summary>
        /// <param name="index">Index of the first inserted item.</param>
        /// <param name="items">Array of inserted items.</param>
        private void OnItemsInserted(int index, IListItem[] items)
        {
            if (ItemsInserted != null)
            {
                ItemsInsertedEventArgs args = new ItemsInsertedEventArgs(index, (IListItem[])items.Clone());
                ItemsInserted(this, args);
            }
        }

        /// <summary>
        /// Occurs when items have been removed from the collection.
        /// </summary>
        [ResDescription("ListItemCollection_ItemsRemoved")]
        public event ItemsRemovedEventHandler ItemsRemoved;

        /// <summary>
        /// Raises ItemsRemoved event of the collection.
        /// </summary>
        /// <param name="items">Array of removed items.</param>
        /// <param name="indices">Indices of the removed items.</param>
        private void OnItemsRemoved(IListItem[] items, int[] indices)
        {
            if (ItemsRemoved != null)
            {
                ItemsRemovedEventArgs args = new ItemsRemovedEventArgs((IListItem[])items.Clone(), indices);
                ItemsRemoved(this, args);
            }
        }

        internal event EventHandler ItemsRemovedAll;

        internal void OnItemsRemovedAll(System.EventArgs e)
        {
            if (ItemsRemovedAll != null)
            {
                ItemsRemovedAll(this, e);
            }
        }

        #endregion Events

        #region IListViewItem Events

        /// <summary>
        /// Occurs when item's state is changing.
        /// </summary>
        [ResDescription("ListItemCollection_StateChanging")]
        public event StateChangingEventHandler StateChanging;

        /// <summary>
        /// Raises StateChanging event of the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="stateType"></param>
        /// <returns></returns>
        private bool OnStateChanging(IListItem item, StateType stateType)
        {
            if (StateChanging == null)
                return true;

            StateChangingEventArgs e = new StateChangingEventArgs(item, stateType);
            StateChanging(this, e);
            return !e.Cancel;
        }

        /// <summary>
        /// Occurs when icon of one of the items of the collection has changed.
        /// </summary>
        [ResDescription("ListItemCollection_IconChanged")]
        public event IconChangedEventHandler IconChanged;

        /// <summary>
        /// Raises IconChanged event of the collection.
        /// </summary>
        /// <param name="item">Item whose icon has been changed.</param>
        /// <param name="view">The view in which icon has been changed.</param>
        private void OnIconChanged(IListItem item, View view)
        {
            if (IconChanged != null)
            {
                IconChangedEventArgs args = new IconChangedEventArgs(item, view);
                IconChanged(this, args);
            }
        }

        /// <summary>
        /// Internal handler of the IconChanged event of all items of the collection. This
        /// method re-raises item's events as collection event.
        /// </summary>
        private void IconChangedInternal(Object sender, IconChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            OnIconChanged(e.Item, e.View);
        }

        private void StateChangingInternal(Object sender, StateChangingEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            e.Cancel = !OnStateChanging(e.Item, e.StateType);
        }

        /// <summary>
        /// Occurs when text of one of the items of the collection has changed.
        /// </summary>
        [ResDescription("ListItemCollection_TextChanged")]
        public event TextChangedEventHandler TextChanged;

        /// <summary>
        /// Raises TextChanged event of the collection.
        /// </summary>
        /// <param name="item">Item whose text have changed.</param>
        /// <param name="thumbInfo"></param>
        private void OnTextChanged(IListItem item, int textInfoId)
        {
            if (TextChanged != null)
            {
                TextChangedEventArgs args = new TextChangedEventArgs(item, textInfoId);
                TextChanged(this, args);
            }
        }

        /// <summary>
        /// Internal handler of the TextChanged event of all items of the collection. This
        /// method re-raises item's events as collection event.
        /// </summary>
        private void TextChangedInternal(Object sender, TextChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            OnTextChanged(e.Item, e.TextInfoId);
        }

        /// <summary>
        /// Occurs when state of one of the items of the collection has changed.
        /// </summary>
        [ResDescription("ListItemCollection_StateChanged")]
        public event StateChangedEventHandler StateChanged;

        /// <summary>
        /// Raises StateChanged event of the collection.
        /// </summary>
        /// <param name="item">Item whose state have changed.</param>
        /// <param name="stateType">State change type.</param>
        private void OnStateChanged(IListItem item, StateType stateType)
        {
            if (StateChanged != null)
            {
                StateChangedEventArgs args = new StateChangedEventArgs(item, stateType);
                StateChanged(this, args);
            }
        }

        /// <summary>
        /// Internal handler of the StateChanged event of all items of the collection. This
        /// method re-raises item's events as collection event.
        /// </summary>
        private void StateChangedInternal(Object sender, StateChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            OnStateChanged(e.Item, e.StateType);
        }

        #endregion IListViewItem Events

        #region Private members

        /// <summary>
        /// Converts contents of collection into simple array of IListItems.
        /// </summary>
        /// <param name="items">Source collection.</param>
        /// <param name="resultItems">Reference to the variable where to store result.</param>
        private static void ConvertCollectionToArray(/*ArrayList*/IList items, out IListItem[] resultItems)
        {
            if (items == null || items.Count == 0)
            {
                resultItems = new IListItem[0];
                return;
            }

            resultItems = new IListItem[items.Count];
            for (int i = 0; i < items.Count; i++)
                resultItems[i] = (IListItem)items[i];
        }

        /// <summary>
        /// Registers collection's handlers for some items's events and sets HasOwner flag.
        /// Every item of the collection should pass through this method.
        /// </summary>
        /// <param name="item">The item object whose events should be handled.</param>
        private void RegisterItem(IListItem item)
        {
            item.TextChanged += new TextChangedEventHandler(TextChangedInternal);
            item.IconChanged += new IconChangedEventHandler(IconChangedInternal);
            item.StateChanged += new StateChangedEventHandler(StateChangedInternal);
            item.StateChanging += new StateChangingEventHandler(StateChangingInternal);

            item.Parent = _parentControl;
        }

        /// <summary>
        /// Unregisters collection's handlers of items's events (see also RegisterEvents method).
        /// </summary>
        /// <param name="item">Item object whose events handlers should be removed.</param>
        private void UnregisterItem(IListItem item)
        {
            item.TextChanged -= new TextChangedEventHandler(TextChangedInternal);
            item.IconChanged -= new IconChangedEventHandler(IconChangedInternal);
            item.StateChanged -= new StateChangedEventHandler(StateChangedInternal);
            item.StateChanging -= new StateChangingEventHandler(StateChangingInternal);

            item.Parent = null;
        }

        #endregion Private members

        #region Member variables

        /// <summary>
        /// Reference to the parent control of the item.
        /// </summary>
        private ThumbnailListView _parentControl;

        #endregion Member variables
    }
}