// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Thumbnail list view class.
    /// </summary>
    [ResDescription("ThumbnailListView")]
    [AdaptiveToolboxBitmapAttribute(typeof(Aurigma.GraphicsMill.WinControls.ThumbnailListView), "ThumbnailList.bmp")]
    public class ThumbnailListView : VirtualListView
    {
        /// <summary>
        /// Initializes new instance of the ThumbnailListView class.
        /// </summary>
        [ResDescription("ThumbnailListView_ThumbnailListView")]
        public ThumbnailListView()
        {
            _items = new ListItemCollection(this);
            _columns = new ListColumnCollection(this);

            RegisterEvents();

            _columns.Add(new ListColumn());
            _columns.Clear();

            // UseDragToMove mode variables.
            _allowDropPrevious = this.AllowDrop;
            _insertItemIndex = -1;

            this.SortOnColumnClick = true;

            // Ugly hacks.
            IntPtr uglyHack = this.Handle;
        }

        #region Public properties

        /// <summary>
        /// Gets items collection.
        /// </summary>
        [ResDescription("ThumbnailListView_Items")]
        [Browsable(false)]
        public ListItemCollection Items
        {
            get
            {
                return this._items;
            }
        }

        /// <summary>
        /// Gets array of currently selected items.
        /// </summary>
        [ResDescription("ThumbnailListView_SelectedItems")]
        [Browsable(false)]
        public IListItem[] SelectedItems
        {
            get
            {
                int selectedItemCount = 0;
                foreach (IListItem item in ItemsInternal)
                    if (item.Selected)
                        selectedItemCount++;

                IListItem[] result = new IListItem[selectedItemCount];

                int i = 0;
                foreach (IListItem item in ItemsInternal)
                    if (item.Selected)
                        result[i++] = item;

                return result;
            }
        }

        /// <summary>
        /// Gets array of the indices of the selected items.
        /// </summary>
        [ResDescription("ThumbnailListView_SelectedIndices")]
        [Browsable(false)]
        public int[] SelectedIndices
        {
            get
            {
                int selectedItemCount = 0;
                foreach (IListItem item in ItemsInternal)
                    if (item.Selected)
                        selectedItemCount++;

                int[] result = new int[selectedItemCount];

                int k = 0;
                for (int i = 0; i < ItemsInternal.Count; i++)
                    if (ItemsInternal[i].Selected)
                        result[k++] = i;

                return result;
            }
        }

        /// <summary>
        /// Gets array of currently selected items.
        /// </summary>
        [ResDescription("ThumbnailListView_CheckedItems")]
        [Browsable(false)]
        public IListItem[] CheckedItems
        {
            get
            {
                int checkedItemCount = 0;
                foreach (IListItem item in ItemsInternal)
                    if (item.Checked)
                        checkedItemCount++;

                IListItem[] result = new IListItem[checkedItemCount];

                int i = 0;
                foreach (IListItem item in ItemsInternal)
                    if (item.Checked)
                        result[i++] = item;

                return result;
            }
        }

        /// <summary>
        /// Gets array of the indices of the checked items.
        /// </summary>
        [ResDescription("ThumbnailListView_CheckedIndices")]
        [Browsable(false)]
        public int[] CheckedIndices
        {
            get
            {
                int checkedItemCount = 0;
                foreach (IListItem item in ItemsInternal)
                    if (item.Checked)
                        checkedItemCount++;

                int[] result = new int[checkedItemCount];

                int k = 0;
                for (int i = 0; i < ItemsInternal.Count; i++)
                    if (ItemsInternal[i].Checked)
                        result[k++] = i;

                return result;
            }
        }

        /// <summary>
        /// The columns shown in Details view.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [CategoryAttribute("Behavior")]
        [ResDescription("ThumbnailListView_Columns")]
        public ListColumnCollection Columns
        {
            get
            {
                return this._columns;
            }
        }

        #endregion Public properties

        #region Public methods

        /// <summary>
        /// Select all items.
        /// </summary>
        public void SelectAll()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].Selected = true;
        }

        /// <summary>
        /// Deselect all items.
        /// </summary>
        public void DeselectAll()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].Selected = false;
        }

        /// <summary>
        /// Check all items.
        /// </summary>
        public void CheckAll()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].Checked = true;
        }

        /// <summary>
        /// Uncheck all items.
        /// </summary>
        public void UncheckAll()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].Checked = false;
        }

        #endregion Public methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Overrides internal items collection property.
        /// </summary>
        internal override ListItemCollection ItemsInternal
        {
            get
            {
                return this._items;
            }
        }

        /// <summary>
        /// Overrides internal columns collection property.
        /// </summary>
        internal override ListColumnCollection ColumnsInternal
        {
            get
            {
                return this._columns;
            }
        }

        #region "DragToMove implementation"

        /// <summary>
        /// Determines whether user can use drag&drop operations to change items order in the control.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("ThumbnailListView_UseDragToMove")]
        public bool UseDragToMove
        {
            get
            {
                return _useDragToMove;
            }
            set
            {
                if (_useDragToMove != value)
                {
                    _useDragToMove = value;

                    if (_useDragToMove)
                        AssignDragToMoveHandlers();
                    else
                        RemoveDragToMoveHandlers();
                }
            }
        }

        /// <summary>
        /// Assigns internal event handlers to drag&drop events to supply "UseDragToMove" mode.
        /// </summary>
        private void AssignDragToMoveHandlers()
        {
            _allowDropPrevious = this.AllowDrop;
            this.AllowDrop = true;

            this.ItemDrag += new ItemDragEventHandler(ItemDragInternal);
            this.DragEnter += new DragEventHandler(DragEnterInternal);
            this.DragOver += new DragEventHandler(DragOverInternal);
            this.DragDrop += new DragEventHandler(DragDropInternal);
        }

        /// <summary>
        /// Removes internal event handlers to drag&drop events that supplies "UseDragToMove" mode.
        /// </summary>
        private void RemoveDragToMoveHandlers()
        {
            this.AllowDrop = _allowDropPrevious;

            this.ItemDrag -= new ItemDragEventHandler(ItemDragInternal);
            this.DragEnter -= new DragEventHandler(DragEnterInternal);
            this.DragOver -= new DragEventHandler(DragOverInternal);
            this.DragDrop -= new DragEventHandler(DragDropInternal);
        }

        /// <summary>
        /// Internal handler for ItemDrag event. Supplies "UseDragToMove" mode.
        /// </summary>
        private void ItemDragInternal(object sender, Aurigma.GraphicsMill.WinControls.ItemDragEventArgs e)
        {
            int[] indices = this.SelectedIndices;
            this.DoDragDrop(indices, System.Windows.Forms.DragDropEffects.Move);
            _insertItemIndex = -1;
            this.SetInsertMark(-1, false);
        }

        /// <summary>
        /// Internal handler for DragEnter event. Supplies "UseDragToMove" mode.
        /// </summary>
        private void DragEnterInternal(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetData(typeof(int[])) != null)
                e.Effect = System.Windows.Forms.DragDropEffects.Move;
            else
                e.Effect = System.Windows.Forms.DragDropEffects.None;
        }

        /// <summary>
        /// Internal handler for DragOver event. Supplies "UseDragToMove" mode.
        /// </summary>
        private void DragOverInternal(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Effect != System.Windows.Forms.DragDropEffects.Move)
                return;

            e.Effect = System.Windows.Forms.DragDropEffects.Move;
            System.Drawing.Point clientPoint = this.PointToClient(new System.Drawing.Point(e.X, e.Y));

            int index;
            bool afterItem;

            InsertMarkHitTest(clientPoint, out index, out afterItem);
            SetInsertMark(index, afterItem);

            _insertItemIndex = index;
            if (afterItem)
                _insertItemIndex++;

            if (_insertItemIndex >= this.Items.Count)
                _insertItemIndex = -1;
        }

        /// <summary>
        /// Internal handler for DragDrop event. Supplies "UseDragToMove" mode.
        /// </summary>
        private void DragDropInternal(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Effect != System.Windows.Forms.DragDropEffects.Move)
            {
                _insertItemIndex = -1;
                this.SetInsertMark(-1, false);
                return;
            }

            int[] srcIndices = (int[])e.Data.GetData(typeof(int[]));

            if (_insertItemIndex != -1)
            {
                foreach (int i in srcIndices)
                    if (_insertItemIndex == i)
                    {
                        _insertItemIndex = -1;
                        this.SetInsertMark(-1, false);
                        return;
                    }
            }

            this.ItemsInternal.Swap(srcIndices, (_insertItemIndex < 0 ? this.ItemsInternal.Count : _insertItemIndex));
            this.SetInsertMark(-1, false);
            _insertItemIndex = -1;
        }

        #endregion "DragToMove implementation"

        #region "SortOnColumnClick implementation"

        /// <summary>
        /// Determines whether items of the control should be sorted with column header click.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("ThumbnailListView_SortOnColumnClick")]
        public bool SortOnColumnClick
        {
            get
            {
                return _sortOnColumnClick;
            }
            set
            {
                if (_sortOnColumnClick != value)
                {
                    _sortOnColumnClick = value;

                    if (_sortOnColumnClick)
                        AssignSortOnColumnClickHandlers();
                    else
                        RemoveSortOnColumnClickHandlers();
                }
            }
        }

        private void AssignSortOnColumnClickHandlers()
        {
            this.ColumnsInternal.ColumnClick += new ColumnClickEventHandler(ThumbnailListView_ColumnClick);
        }

        private void RemoveSortOnColumnClickHandlers()
        {
            this.ColumnsInternal.ColumnClick -= new ColumnClickEventHandler(ThumbnailListView_ColumnClick);
        }

        private void ThumbnailListView_ColumnClick(Object sender, ColumnClickEventArgs e)
        {
            if (_lastSortColumn != null)
            {
                int index = this.ColumnsInternal.IndexOf(_lastSortColumn);
                if (index != -1)
                    RemoveSortOrderIcon(index);
            }

            if (_lastSortColumn == null || _lastSortColumn != e.Column)
            {
                _lastSortColumn = e.Column;
                _lastSortOrderAscending = false;
            }

            _lastSortOrderAscending = !_lastSortOrderAscending;
            this.Sort(e.Column.TextInfoId, _lastSortOrderAscending);
            this.ColumnsInternal.SetSortOrderIcon(e.Index, _lastSortOrderAscending);
        }

        #endregion "SortOnColumnClick implementation"

        #region Private member variables

        /// <summary>
        /// Stores items collection.
        /// </summary>
        private ListItemCollection _items;

        /// <summary>
        /// Stores columns collection.
        /// </summary>
        private ListColumnCollection _columns;

        private bool _useDragToMove;
        private bool _allowDropPrevious;
        private int _insertItemIndex;
        private bool _sortOnColumnClick;
        private ListColumn _lastSortColumn;
        private bool _lastSortOrderAscending;

        #endregion Private member variables
    }
}