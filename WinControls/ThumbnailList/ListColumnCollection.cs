// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;
using System.ComponentModel;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Implementation of list columns collection functionality.
    /// Note: text alignment of the first column in the collection must be HorizontalAlignment.Left.
    /// Note: the collection cannot contain equal objects - you should remove the object from its current position or clone.
    /// </summary>
    [ResDescription("ListColumnCollection")]
    public class ListColumnCollection : CollectionBase, IList, ICollection
    {
        #region Construction/Destruction

        /// <summary>
        /// Initializes new instance of the ListColumnCollection class.
        /// </summary>
        [ResDescription("ListColumnCollection_ListColumnCollection")]
        internal ListColumnCollection(ThumbnailListView parentControl)
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
        [ResDescription("ListColumnCollection_Indexer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ListColumn this[int index]
        {
            get
            {
                return ((ListColumn)List[index]);
            }
            /*set;*/
        }

        /// <summary>
        /// Adds a column to the collection.
        /// </summary>
        /// <param name="column">A column to add.</param>
        /// <returns>The position into which the new element was added.</returns>
        [ResDescription("ListColumnCollection_Add0")]
        public int Add(ListColumn column)
        {
            if (column != null)
            {
                ListColumn[] columns = { column };

                return Add(columns);
            }
            return -1;
        }

        /// <summary>
        /// Adds array of columns to the collection.
        /// </summary>
        /// <param name="columns">Columns to add.</param>
        /// <returns>Returns index of the first added column.</returns>
        [ResDescription("ListColumnCollection_Add1")]
        public int Add(ListColumn[] columns)
        {
            if (columns == null)
                return -1;

            int firstIndex = this.Count;
            Insert(firstIndex, columns);
            return firstIndex;
        }

        /// <summary>
        /// Searches for the specified column and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="value">The column to locate in the collection.</param>
        /// <returns>Returns index of the first occurrence of specified column object in the collection, otherwise, -1.</returns>
        [ResDescription("ListColumnCollection_IndexOf")]
        public int IndexOf(ListColumn value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="column">The column to insert.</param>
        [ResDescription("ListColumnCollection_Insert0")]
        public void Insert(int index, ListColumn column)
        {
            if (column == null)
                return;

            ListColumn[] columns = { column };
            Insert(index, columns);
        }

        /// <summary>
        /// Inserts an array of columns into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="columns">Array of columns to insert.</param>
        [ResDescription("ListColumnCollection_Insert1")]
        public void Insert(int index, ListColumn[] columns)
        {
            if (columns == null)
                throw new ArgumentNullException("columns");
            if (index < 0 || index > this.Count)
                throw new ArgumentOutOfRangeException("index");

            ArrayList columnsToAdd = new ArrayList();
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i] == null)
                    continue;

                if (columns[i].ParentInternal != null)
                    throw new ArgumentException(StringResources.GetString("ColumnSingleParentException"), "columns");

                int currentIndex = index;
                if (OnColumnInserting(columns[i], currentIndex))
                {
                    columnsToAdd.Add(columns[i]);
                    currentIndex++;
                }
            }

            if (columnsToAdd.Count > 0)
            {
                ListColumn[] insertedColumns = new ListColumn[columnsToAdd.Count];
                for (int i = 0; i < columnsToAdd.Count; i++)
                {
                    insertedColumns[i] = (ListColumn)columnsToAdd[i];
                    RegisterColumn(insertedColumns[i]);
                    List.Insert(index + i, insertedColumns[i]);
                }

                OnColumnsInserted(index, insertedColumns);
            }
        }

        /// <summary>
        /// Removes specified column from the collection.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        [ResDescription("ListColumnCollection_Remove0")]
        public void Remove(ListColumn column)
        {
            if (column == null)
                return;

            ListColumn[] columns = { column };
            Remove(columns);
        }

        /// <summary>
        /// Removes specified columns from the collection.
        /// </summary>
        /// <param name="columns">Array of columns to remove.</param>
        [ResDescription("ListColumnCollection_Remove1")]
        public void Remove(ListColumn[] columns)
        {
            if (columns == null)
                return;

            ArrayList columnsToRemove = new ArrayList();
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i] != null && OnColumnRemoving(columns[i]))
                    columnsToRemove.Add(columns[i]);
            }

            if (columnsToRemove.Count > 0)
            {
                columnsToRemove.Sort(new IListIndexComparer(this));

                ListColumn[] removedColumns = new ListColumn[columnsToRemove.Count];
                int[] removedColumnsIndices = new int[columnsToRemove.Count];

                for (int i = 0; i < columnsToRemove.Count; i++)
                {
                    removedColumns[i] = (ListColumn)columnsToRemove[i];
                    removedColumnsIndices[i] = IndexOf(removedColumns[i]);
                    UnregisterColumn(removedColumns[i]);
                    List.Remove(removedColumns[i]);
                }

                OnColumnsRemoved(removedColumns, removedColumnsIndices);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        [ResDescription("ListColumnCollection_RemoveAt")]
        public new void RemoveAt(int index)
        {
            ListColumn item = this[index];
            if (item != null)
                Remove(item);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from the collection. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(ListColumn[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        /// <summary>
        /// Determines whether an element is in the collection.
        /// </summary>
        /// <param name="column">The column object to locate in the collection.</param>
        /// <returns>true if column is found in the collection; otherwise, false.</returns>
        [ResDescription("ListColumnCollection_Contains")]
        public bool Contains(ListColumn column)
        {
            // If value is not of type ListColumn, this will return false.
            return List.Contains(column);
        }

        /// <summary>
        /// Validates type of all added items.
        /// </summary>
        /// <param name="value">The item to validate.</param>
        protected override void OnValidate(Object value)
        {
            if (!(value is ListColumn))
                throw new ArgumentException(StringResources.GetString("ShouldHaveListColumnType"), "value");
        }

        #endregion CollectionBase implementation

        #region Methods

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        [ResDescription("ListColumnCollection_Clear")]
        public new void Clear()
        {
            Remove(this.ToArray());
        }

        /// <summary>
        /// Copies the elements of the collection to a new array.
        /// </summary>
        /// <returns>New array that contains all columns of the collection.</returns>
        [ResDescription("ListColumnCollection_ToArray")]
        public ListColumn[] ToArray()
        {
            ListColumn[] columns = new ListColumn[Count];
            for (int i = 0; i < Count; i++)
                columns[i] = this[i];

            return columns;
        }

        public void SetSortOrderIcon(int columnIndex, bool ascendingIcon)
        {
            _parentControl.SetSortOrderIcon(columnIndex, ascendingIcon);
        }

        public void RemoveSortOrderIcon(int columnIndex)
        {
            _parentControl.RemoveSortOrderIcon(columnIndex);
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Occurs when a column header is clicked.
        /// </summary>
        [CategoryAttribute("Action")]
        [ResDescription("VirtualListView_ColumnClick")]
        public event ColumnClickEventHandler ColumnClick;

        /// <summary>
        /// Raises ColumnClick event of the control.
        /// </summary>
        /// <param name="columnIndex">Clicked column index.</param>
        protected internal virtual void OnColumnClick(int columnIndex)
        {
            if (ColumnClick == null)
                return;

            if (columnIndex < 0 || columnIndex >= this.Count)
                throw new ArgumentOutOfRangeException("columnIndex");

            ColumnClickEventArgs args = new ColumnClickEventArgs(columnIndex, this[columnIndex]);
            ColumnClick(this, args);
        }

        /// <summary>
        /// Occurs when new column is inserting into the collection.
        /// </summary>
        [ResDescription("ListColumnCollection_ColumnInserting")]
        public event ColumnInsertingEventHandler ColumnInserting;

        /// <summary>
        /// Raises ColumnAdding event of the collection.
        /// </summary>
        /// <param name="column">Changed column object.</param>
        /// <returns>true if handlers of the event didn't cancel changes of the column; otherwise, false.</returns>
        protected virtual bool OnColumnInserting(ListColumn column, int index)
        {
            if (ColumnInserting == null)
                return true;

            ColumnInsertingEventArgs args = new ColumnInsertingEventArgs(column, index);
            ColumnInserting(this, args);
            return !args.Cancel;
        }

        /// <summary>
        /// Occurs when column is removing.
        /// </summary>
        [ResDescription("ListColumnCollection_ColumnRemoving")]
        public event ColumnRemovingEventHandler ColumnRemoving;

        /// <summary>
        /// Raises ColumnRemoving event of the collection.
        /// </summary>
        /// <param name="column">The removing column.</param>
        /// <returns>true if handlers of the event didn't cancel column removing; otherwise, false.</returns>
        protected virtual bool OnColumnRemoving(ListColumn column)
        {
            if (ColumnRemoving != null)
            {
                ColumnRemovingEventArgs args = new ColumnRemovingEventArgs(column);
                ColumnRemoving(this, args);
                return !args.Cancel;
            }
            else
                return true;
        }

        /// <summary>
        /// Occurs when columns have been inserted into the collection.
        /// </summary>
        [ResDescription("ListColumnCollection_ColumnsInserted")]
        public event ColumnsInsertedEventHandler ColumnsInserted;

        /// <summary>
        /// Raises ColumnsInserted event of the collection.
        /// </summary>
        /// <param name="index">Index of the fist inserted column.</param>
        /// <param name="columns">Array of the inserted columns.</param>
        protected virtual void OnColumnsInserted(int index, ListColumn[] columns)
        {
            if (ColumnsInserted != null)
            {
                ColumnsInsertedEventArgs args = new ColumnsInsertedEventArgs(columns, index);
                ColumnsInserted(this, args);
            }
        }

        /// <summary>
        /// Occurs when columns have been removed from the collection.
        /// </summary>
        [ResDescription("ListColumnCollection_ColumnsRemoved")]
        public event ColumnsRemovedEventHandler ColumnsRemoved;

        /// <summary>
        /// Raises ColumnsRemoved event of the collection.
        /// </summary>
        /// <param name="removedColumns">Array of removed columns.</param>
        /// <param name="removedColumnsIndices">Indices of removed columns.</param>
        protected virtual void OnColumnsRemoved(ListColumn[] removedColumns, int[] removedColumnsIndices)
        {
            if (ColumnsRemoved != null)
            {
                ColumnsRemovedEventArgs args = new ColumnsRemovedEventArgs(removedColumns, removedColumnsIndices);
                ColumnsRemoved(this, args);
            }
        }

        #endregion Events

        #region ListColumn Events

        /// <summary>
        /// Occurs when a column of the collection have been changed.
        /// </summary>
        [ResDescription("ListColumnCollection_ColumnChanged")]
        public event ColumnChangedEventHandler ColumnChanged;

        /// <summary>
        /// Raises ColumnChanged event of the collection.
        /// </summary>
        /// <param name="column">Changed column object.</param>
        /// <param name="changeType">Column change type.</param>
        protected virtual void OnColumnChanged(ListColumn column, ColumnChangeType changeType)
        {
            if (ColumnChanged != null)
            {
                ColumnChangedEventArgs args = new ColumnChangedEventArgs(column, changeType);
                ColumnChanged(this, args);
            }
        }

        /// <summary>
        /// Handler of ColumnChanged event for all columns of the collection. This method re-raises event of a single
        /// column as event of the column collection.
        /// </summary>
        internal void ColumnChangedInternal(Object sender, ColumnChangedEventArgs args)
        {
            OnColumnChanged(args.Column, args.ChangeType);
        }

        #endregion ListColumn Events

        #region Private members

        /// <summary>
        /// Registers collection's handlers for some column's events and sets Parent property.
        /// Every column of the collection should pass through this method.
        /// </summary>
        /// <param name="column">The column object whose events should be handled.</param>
        private void RegisterColumn(ListColumn column)
        {
            column.ParentInternal = _parentControl;
            column.ColumnChanged += new ColumnChangedEventHandler(ColumnChangedInternal);
        }

        /// <summary>
        /// Unregisters collection's handlers of column's events (see also RegisterEvents method).
        /// </summary>
        /// <param name="column">Column object whose events handlers should be removed.</param>
        private void UnregisterColumn(ListColumn column)
        {
            column.ColumnChanged -= new ColumnChangedEventHandler(ColumnChangedInternal);
            column.ParentInternal = null;
        }

        #endregion Private members

        #region "Private data members"

        /// <summary>
        /// Reference to the parent control of the column.
        /// </summary>
        private ThumbnailListView _parentControl;

        #endregion "Private data members"
    }
}