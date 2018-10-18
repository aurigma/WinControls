// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;

namespace Aurigma.GraphicsMill.WinControls
{
    #region "Item events"
    #region "Base class for item events"

    /// <summary>
    /// Base class for all classes that contains item-related events data.
    /// </summary>
    [ResDescription("ItemEventArgs")]
    public class ItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of the BaseItemEventArgs class.
        /// </summary>
        /// <param name="item">Item concerned with the event.</param>
        [ResDescription("ItemEventArgs_ItemEventArgs")]
        public ItemEventArgs(IListItem item)
        {
            _item = item;
        }

        /// <summary>
        /// Item concerned with the event.
        /// </summary>
        [ResDescription("ItemEventArgs_Item")]
        public IListItem Item
        {
            get
            {
                return _item;
            }
        }

        private IListItem _item;
    }

    #endregion "Base class for item events"

    #region "ItemDrag event"

    /// <summary>
    /// Provides data for ItemDrag event of ThumbnailListView.
    /// </summary>
    [ResDescription("ItemDragEventArgs")]
    public class ItemDragEventArgs : ItemEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ItemDragEventArgs class.
        /// </summary>
        /// <param name="item">Index of the dragged item.</param>
        /// <param name="mouseButton">The name of the mouse button that was clicked during the drag operation.</param>
        [ResDescription("ItemDragEventArgs_ItemDragEventArgs")]
        public ItemDragEventArgs(System.Windows.Forms.MouseButtons mouseButton, IListItem item)
            : base(item)
        {
            _mouseButton = mouseButton;
        }

        /// <summary>
        /// Gets name of the mouse button that was clicked during the drag operation.
        /// </summary>
        [ResDescription("ItemDragEventArgs_MouseButton")]
        public System.Windows.Forms.MouseButtons MouseButton
        {
            get
            {
                return _mouseButton;
            }
        }

        private System.Windows.Forms.MouseButtons _mouseButton;
    }

    /// <summary>
    /// Represents the method that will handle ItemDrag event.
    /// </summary>
    public delegate void ItemDragEventHandler(Object sender, ItemDragEventArgs e);

    #endregion "ItemDrag event"

    #region TextChanged Event

    /// <summary>
    /// Provides data for TextChanged event of ThumbnailListView, IListItem and IListItemCollection.
    /// </summary>
    [ResDescription("TextChangedEventArgs")]
    public class TextChangedEventArgs : ItemEventArgs
    {
        /// <summary>
        /// Initializes new instance of the TextChangedEventArgs class.
        /// </summary>
        /// <param name="item">Item whose text information has been changed.</param>
        /// <param name="textInfoId">Specifies type of the text information that has been changed.</param>
        [ResDescription("TextChangedEventArgs_TextChangedEventArgs")]
        public TextChangedEventArgs(IListItem item, int textInfoId)
            : base(item)
        {
            _textInfoId = textInfoId;
        }

        /// <summary>
        /// Specifies type of the text information that has been changed.
        /// </summary>
        [ResDescription("TextChangedEventArgs_TextInfoId")]
        public int TextInfoId
        {
            get
            {
                return _textInfoId;
            }
        }

        private int _textInfoId;
    }

    /// <summary>
    /// Represents the method that will handle TextChanged event of ThumbnailListView, IListItem and IListItemCollection.
    /// </summary>
    public delegate void TextChangedEventHandler(Object sender, TextChangedEventArgs e);

    #endregion TextChanged Event

    #region IconChanged Event

    /// <summary>
    /// Provides data for IconChangedEvent of IListItem and IListItemCollection.
    /// </summary>
    [ResDescription("IconChangedEventArgs")]
    public class IconChangedEventArgs : ItemEventArgs
    {
        /// <summary>
        /// Initializes new instance of the IconChangedEventArgs class.
        /// </summary>
        /// <param name="item">Items whose icon changed.</param>
        /// <param name="view">View mode of the control for which the icon has been changed.
        [ResDescription("IconChangedEventArgs_IconChangedEventArgs")]
        public IconChangedEventArgs(IListItem item, View view)
            : base(item)
        {
            this._view = view;
        }

        /// <summary>
        /// View mode of the control for which the icon has been changed.
        /// </summary>
        [ResDescription("IconChangedEventArgs_View")]
        public View View
        {
            get
            {
                return _view;
            }
        }

        private View _view;
    }

    /// <summary>
    /// Represents the method that will handle IconChanged event of ThumbnailListView, IListItem and IListItemCollection.
    /// </summary>
    public delegate void IconChangedEventHandler(Object sender, IconChangedEventArgs e);

    #endregion IconChanged Event

    #region StateChanging event

    /// <summary>
    /// Provides data for StateChanging event of ThumbnailListView, IListItem and IListItemCollection.
    /// </summary>
    [ResDescription("StateChangingEventArgs")]
    public class StateChangingEventArgs : ItemEventArgs
    {
        /// <summary>
        /// Initializes new instance of the StateChangingEventArgs class.
        /// </summary>
        /// <param name="item">Item whose state is changing.</param>
        /// <param name="stateType">Type of item's state that is changing.</param>
        [ResDescription("StateChangingEventArgs_StateChangingEventArgs")]
        public StateChangingEventArgs(IListItem item, StateType stateType)
            : base(item)
        {
            _stateType = stateType;
        }

        /// <summary>
        /// Type of item's state that is changing.
        /// </summary>
        [ResDescription("StateChangingEventArgs_StateType")]
        public StateType StateType
        {
            get
            {
                return _stateType;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the changes should be canceled.
        /// </summary>
        [ResDescription("StateChangingEventArgs_Cancel")]
        public bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                _cancel = value;
            }
        }

        private StateType _stateType;
        private bool _cancel;
    }

    /// <summary>
    /// Represents the method that will handle StateChanging event of ThumbnailListView, IListItem and IListItemCollection.
    /// </summary>
    public delegate void StateChangingEventHandler(Object sender, StateChangingEventArgs e);

    #endregion StateChanging event

    #region StateChanged event

    /// <summary>
    /// Provides data for StateChanged event of ThumbnailListView, IListItem and IListItemCollection.
    /// </summary>
    [ResDescription("StateChangedEventArgs")]
    public class StateChangedEventArgs : ItemEventArgs
    {
        /// <summary>
        /// Initializes new instance of the StateChangedEventArgs class.
        /// </summary>
        /// <param name="item">Item whose state has been changed.</param>
        /// <param name="stateType">Type of item's state that has been changed.</param>
        [ResDescription("StateChangedEventArgs_StateChangedEventArgs")]
        public StateChangedEventArgs(IListItem item, StateType stateType)
            : base(item)
        {
            _stateType = stateType;
        }

        /// <summary>
        /// Type of item's state that has been changed.
        /// </summary>
        [ResDescription("StateChangedEventArgs_StateType")]
        public StateType StateType
        {
            get
            {
                return _stateType;
            }
        }

        private StateType _stateType;
    }

    /// <summary>
    /// Represents the method that will handle StateChanged event of ThumbnailListView, IListItem and IListItemCollection.
    /// </summary>
    public delegate void StateChangedEventHandler(Object sender, StateChangedEventArgs e);

    #endregion StateChanged event

    #region ViewChanged event

    /// <summary>
    /// Provides data for ViewChanged event of ThumbnailListView control.
    /// </summary>
    [ResDescription("ViewChangedEventArgs")]
    public class ViewChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of the ViewChangedEventArgs class.
        /// </summary>
        /// <param name="newView">New view mode of the control.</param>
        [ResDescription("ViewChangedEventArgs_ViewChangedEventArgs")]
        public ViewChangedEventArgs(View newView)
        {
            _newView = newView;
        }

        /// <summary>
        /// New view mode of the control.
        /// </summary>
        [ResDescription("ViewChangedEventArgs_NewView")]
        public View NewView
        {
            get
            {
                return _newView;
            }
        }

        private View _newView;
    }

    /// <summary>
    /// Represents the method that will handle ViewChanged event of ThumbnailListView control.
    /// </summary>
    public delegate void ViewChangedEventHandler(Object sender, ViewChangedEventArgs e);

    #endregion ViewChanged event

    #region ItemInserting Event

    /// <summary>
    /// Provides data for action request events of ThumbnailListView, IListItem and IListItemCollection.
    /// </summary>
    [ResDescription("ItemActionRequestEventArgs")]
    public class ItemActionRequestEventArgs : ItemEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ItemActionRequestEventArgs class.
        /// </summary>
        /// <param name="item">The item for which action request is performing.</param>
        [ResDescription("ItemActionRequestEventArgs_ItemActionRequestEventArgs")]
        public ItemActionRequestEventArgs(IListItem item)
            : base(item)
        {
        }

        /// <summary>
        /// The value determining whether the action of the event should be canceled or not.
        /// </summary>
        [ResDescription("ItemActionRequestEventArgs_Cancel")]
        public bool Cancel
        {
            set
            {
                _cancel = value;
            }
            get
            {
                return _cancel;
            }
        }

        private bool _cancel;
    }

    /// <summary>
    /// Provides data for ItemInserting event of the IListItemCollection.
    /// </summary>
    [ResDescription("ItemInsertingEventArgs")]
    public class ItemInsertingEventArgs : ItemActionRequestEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ItemInsertingEventArgs class.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="index">Destination index of the item.</param>
        [ResDescription("ItemInsertingEventArgs_ItemInsertingEventArgs")]
        public ItemInsertingEventArgs(IListItem item, int index)
            : base(item)
        {
            _index = index;
        }

        /// <summary>
        /// Destination index of the item.
        /// </summary>
        public int Index
        {
            get
            {
                return _index;
            }
        }

        private int _index;
    }

    public delegate void ItemInsertingEventHandler(object sender, ItemInsertingEventArgs e);

    #endregion ItemInserting Event

    #region ItemRemoving Event

    /// <summary>
    /// Represents the method that will handle ItemRemoving event of ThumbnailListView control.
    /// </summary>
    public delegate void ItemRemovingEventHandler(Object sender, ItemActionRequestEventArgs e);

    #endregion ItemRemoving Event

    #region ItemsAdded Event

    /// <summary>
    /// Base class for other event arguments classes which contain information about set of items.
    /// </summary>
    [ResDescription("ItemsEventArgs")]
    public abstract class ItemsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of the ItemsEventArgs class.
        /// </summary>
        /// <param name="items">Items affected by the event.</param>
        [ResDescription("ItemsEventArgs_ItemsEventArgs")]
        protected ItemsEventArgs(IListItem[] items)
        {
            if (items != null)
            {
                this._items = items;
            }
            else
            {
                this._items = new IListItem[0];
            }
        }

        /// <summary>
        /// Items affected by the event.
        /// </summary>
        [ResDescription("ItemsEventArgs_Items")]
        public IListItem[] Items
        {
            get
            {
                return _items;
            }
        }

        private IListItem[] _items;
    }

    #endregion ItemsAdded Event

    #region ItemsRemoved Event

    /// <summary>
    /// Provides data for ItemsRemoved event.
    /// </summary>
    [ResDescription("ItemsRemovedEventArgs")]
    public class ItemsRemovedEventArgs : ItemsEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ItemsRemovedEventArgs class.
        /// </summary>
        /// <param name="items">Removed items.</param>
        /// <param name="indices">Indices of removed items in ItemCollection of the ThumbnailList.</param>
        [ResDescription("ItemsRemovedEventArgs_ItemsRemovedEventArgs")]
        public ItemsRemovedEventArgs(IListItem[] items, int[] indices)
            : base(items)
        {
            _removedItemsIndices = indices;
        }

        /// <summary>
        /// Indices of removed items in ListItemCollection of the ThumbnailList.
        /// </summary>
        [ResDescription("ItemsRemovedEventArgs_RemovedItemsIndices")]
        public int[] RemovedItemsIndices
        {
            get
            {
                return _removedItemsIndices;
            }
        }

        private int[] _removedItemsIndices;
    }

    /// <summary>
    /// Represents method that will handle ItemsRemoved event of the ListItemCollection.
    /// </summary>
    public delegate void ItemsRemovedEventHandler(Object sender, ItemsRemovedEventArgs e);

    #endregion ItemsRemoved Event

    #region ItemsInserted Event

    /// <summary>
    /// Provides information for ItemsInserted event of the ListItemCollection of ThumbnailListView.
    /// </summary>
    [ResDescription("ItemsInsertedEventArgs")]
    public class ItemsInsertedEventArgs : ItemsEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ItemsInsertedEventArgs class.
        /// </summary>
        /// <param name="index">Index of the first inserted item.</param>
        /// <param name="items">Array of inserted items.</param>
        [ResDescription("ItemsInsertedEventArgs_ItemsInsertedEventArgs")]
        public ItemsInsertedEventArgs(int index, IListItem[] items)
            : base(items)
        {
            _index = index;
        }

        /// <summary>
        /// Index of the first inserted item.
        /// </summary>
        [ResDescription("ItemsInsertedEventArgs_Index")]
        public int Index
        {
            get
            {
                return _index;
            }
        }

        private int _index;
    }

    /// <summary>
    /// Represents method that will handle ItemsInserted event of the ListItemCollection.
    /// </summary>
    public delegate void ItemsInsertedEventHandler(Object sender, ItemsInsertedEventArgs e);

    #endregion ItemsInserted Event
    #endregion "Item events"

    #region "Column events"
    #region ColumnChanged Event

    /// <summary>
    /// Provides info for ColumnChanged event of ListColumn and IListColumnCollection.
    /// </summary>
    [ResDescription("ColumnChangedEventArgs")]
    public class ColumnChangedEventArgs : ColumnEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ColumnChangedEventArgs class.
        /// </summary>
        /// <param name="column">Column that has been changed.</param>
        /// <param name="changeType">Column change type.</param>
        [ResDescription("ColumnChangedEventArgs_ColumnChangedEventArgs")]
        public ColumnChangedEventArgs(ListColumn column, ColumnChangeType changeType)
            : base(column)
        {
            _changeType = changeType;
        }

        /// <summary>
        /// Column change type.
        /// </summary>
        [ResDescription("ColumnChangedEventArgs_ChangeType")]
        public ColumnChangeType ChangeType
        {
            get
            {
                return _changeType;
            }
        }

        private ColumnChangeType _changeType;
    }

    /// <summary>
    /// Represents method that will handle ColumnChanged event of the ListColumn and IListColumnCollection classes.
    /// </summary>
    public delegate void ColumnChangedEventHandler(Object sender, ColumnChangedEventArgs e);

    #endregion ColumnChanged Event

    #region ColumnClick Event

    /// <summary>
    /// Provides information for ColumnClick event of the ThumbnailListView control.
    /// </summary>
    [ResDescription("ColumnClickEventArgs")]
    public class ColumnClickEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of the ColumnClickEventArgs class.
        /// </summary>
        /// <param name="columnIndex">Index of the column that has been clicked.</param>
        /// <param name="column">Column that has been clicked.</param>
        [ResDescription("ColumnClickEventArgs_ColumnClickEventArgs")]
        public ColumnClickEventArgs(int columnIndex, ListColumn column)
        {
            _index = columnIndex;
            _column = column;
        }

        /// <summary>
        /// Index of the column that has been clicked.
        /// </summary>
        [ResDescription("ColumnClickEventArgs_Index")]
        public int Index
        {
            get
            {
                return _index;
            }
        }

        /// <summary>
        /// Column that has been clicked.
        /// </summary>
        [ResDescription("ColumnClickEventArgs_Column")]
        public ListColumn Column
        {
            get
            {
                return _column;
            }
        }

        private int _index;
        private ListColumn _column;
    }

    /// <summary>
    /// Represents method that will handle ColumnClick event of the ThumbnailListView control.
    /// </summary>
    public delegate void ColumnClickEventHandler(Object sender, ColumnClickEventArgs e);

    #endregion ColumnClick Event

    #region LabelEditing Event

    /// <summary>
    ///  Represents method that will handle LabelEditing event of the ThumbnailListView control.
    /// </summary>
    public delegate void LabelEditingEventHandler(Object sender, ItemActionRequestEventArgs e);

    #endregion LabelEditing Event

    #region LabelEdited Event

    /// <summary>
    /// Provides information for LabelEdited event of the ThumbnailListView control.
    /// </summary>
    [ResDescription("LabelEditedEventArgs")]
    public class LabelEditedEventArgs : ItemEventArgs
    {
        /// <summary>
        /// Initializes new instance of the LabelEditedEventArgs class.
        /// </summary>
        /// <param name="item">Item whose label text has been edited.</param>
        /// <param name="newText">New label text of the item.</param>
        [ResDescription("LabelEditedEventArgs_LabelEditedEventArgs")]
        public LabelEditedEventArgs(IListItem item, string newText)
            : base(item)
        {
            _newText = newText;
        }

        /// <summary>
        /// New label text of the item.
        /// </summary>
        [ResDescription("LabelEditedEventArgs_NewText")]
        public string NewText
        {
            get
            {
                return _newText;
            }
        }

        private string _newText;
    }

    /// <summary>
    /// Represents method that will handle AfterLabelEdit event of the ThumbnailListView control.
    /// </summary>
    public delegate void LabelEditedEventHandler(Object sender, LabelEditedEventArgs e);

    #endregion LabelEdited Event

    #region "ColumnEventArgs Event"

    /// <summary>
    /// Provides information for column-related events.
    /// </summary>
    [ResDescription("ColumnEventArgs")]
    public class ColumnEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of the ColumnEventArgs class.
        /// </summary>
        /// <param name="column">The column to which event is related.</param>
        [ResDescription("ColumnEventArgs_ColumnEventArgs")]
        public ColumnEventArgs(ListColumn column)
        {
            this._column = column;
        }

        /// <summary>
        /// The column to which event is related.
        /// </summary>
        [ResDescription("ColumnEventArgs_Column")]
        public ListColumn Column
        {
            get
            {
                return _column;
            }
        }

        private ListColumn _column;
    }

    #endregion "ColumnEventArgs Event"

    #region ColumnRemoving Event

    /// <summary>
    /// Provides information for ColumnRemoving event of the IListColumnCollection.
    /// </summary>
    [ResDescription("ColumnRemovingEventArgs")]
    public class ColumnRemovingEventArgs : ColumnEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ColumnRemovingEventArgs class.
        /// </summary>
        /// <param name="column">Column object to remove.</param>
        [ResDescription("ColumnRemovingEventArgs_ColumnRemovingEventArgs")]
        public ColumnRemovingEventArgs(ListColumn column)
            : base(column)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column adding should be canceled.
        /// </summary>
        [ResDescription("ColumnRemovingEventArgs_Cancel")]
        public bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                _cancel = value;
            }
        }

        private bool _cancel;
    }

    /// <summary>
    /// Represents method that will handle ColumnRemoving event of the IListColumnCollection.
    /// </summary>
    public delegate void ColumnRemovingEventHandler(Object sender, ColumnRemovingEventArgs e);

    #endregion ColumnRemoving Event

    #region	"ColumnInserting event"

    /// <summary>
    /// Provides information for ColumnInserting event of IListColumnCollection.
    /// </summary>
    [ResDescription("ColumnInsertingEventArgs")]
    public class ColumnInsertingEventArgs : ColumnRemovingEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ColumnInsertingEventArgs class.
        /// </summary>
        /// <param name="column">The column object to insert.</param>
        /// <param name="index">Destination column index.</param>
        [ResDescription("ColumnInsertingEventArgs_ColumnInsertingEventArgs")]
        public ColumnInsertingEventArgs(ListColumn column, int index)
            : base(column)
        {
            base.Cancel = false;
            _index = index;
        }

        /// <summary>
        ///
        /// </summary>
        [ResDescription("ColumnInsertingEventArgs_Index")]
        public int Index
        {
            get
            {
                return _index;
            }
        }

        private int _index;
    }

    /// <summary>
    /// Represents method that will handle ColumnInserting event of the IListColumnCollection.
    /// </summary>
    public delegate void ColumnInsertingEventHandler(Object sender, ColumnInsertingEventArgs e);

    #endregion "Column events"

    #region Columns Event

    /// <summary>
    /// Base class for other event arguments classes which contain information about set of columns.
    /// </summary>
    [ResDescription("ColumnsEventArgs")]
    public abstract class ColumnsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of the ColumnsEventArgs class.
        /// </summary>
        /// <param name="columns">Columns affected by the event.</param>
        [ResDescription("ColumnsEventArgs_ColumnsEventArgs")]
        protected ColumnsEventArgs(ListColumn[] columns)
        {
            if (columns != null)
                this._columns = columns;
            else
                this._columns = new ListColumn[0];
        }

        /// <summary>
        /// Columns affected by the event.
        /// </summary>
        [ResDescription("ColumnsEventArgs_Columns")]
        public ListColumn[] Columns
        {
            get
            {
                return _columns;
            }
        }

        private ListColumn[] _columns;
    }

    #endregion Columns Event

    #region ColumnsInserted Event

    /// <summary>
    /// Provides information for ColumnsInserted event of the IListColumnCollection.
    /// </summary>
    [ResDescription("ColumnsInsertedEventArgs")]
    public class ColumnsInsertedEventArgs : ColumnsEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ColumnsInsertedEventArgs class.
        /// </summary>
        /// <param name="columns">Array of inserted columns.</param>
        /// <param name="index">Index of the first inserted column.</param>
        [ResDescription("ColumnsInsertedEventArgs_ColumnsInsertedEventArgs")]
        public ColumnsInsertedEventArgs(ListColumn[] columns, int index)
            : base(columns)
        {
            this._index = index;
        }

        /// <summary>
        /// Index of the first inserted column.
        /// </summary>
        [ResDescription("ColumnsInsertedEventArgs_Index")]
        public int Index
        {
            get
            {
                return _index;
            }
        }

        private readonly int _index;
    }

    /// <summary>
    /// Represents method that will handle ColumnsInserted event of the IListColumnCollection.
    /// </summary>
    public delegate void ColumnsInsertedEventHandler(Object sender, ColumnsInsertedEventArgs e);

    #endregion ColumnsInserted Event

    #region ColumnsRemoved Event

    /// <summary>
    /// Provides information for ColumnsRemoved event of the IListColumnCollection.
    /// </summary>
    [ResDescription("ColumnsRemovedEventArgs")]
    public class ColumnsRemovedEventArgs : ColumnsEventArgs
    {
        /// <summary>
        /// Initializes new instance of the ColumnsRemovedEventArgs class.
        /// </summary>
        /// <param name="columns">Array of removed column.</param>
        /// <param name="removedColumnsIndices">Indices of removed columns.</param>
        [ResDescription("ColumnsRemovedEventArgs_ColumnsRemovedEventArgs")]
        public ColumnsRemovedEventArgs(ListColumn[] columns, int[] removedColumnsIndices)
            : base(columns)
        {
            _removedColumnsIndices = removedColumnsIndices;
        }

        /// <summary>
        /// Indices of removed columns.
        /// </summary>
        [ResDescription("ColumnsRemovedEventArgs_RemovedColumnsIndices")]
        public int[] RemovedColumnsIndices
        {
            get
            {
                return _removedColumnsIndices;
            }
        }

        private int[] _removedColumnsIndices;
    }

    /// <summary>
    /// Represents method that will handle ColumnsRemoved event of the IListColumnCollection.
    /// </summary>
    public delegate void ColumnsRemovedEventHandler(Object sender, ColumnsRemovedEventArgs e);

    #endregion ColumnsRemoved Event
    #endregion
}