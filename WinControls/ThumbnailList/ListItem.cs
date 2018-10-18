// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Represents base implementation of the IListItem interface.
    /// </summary>
    [ResDescription("ListItem")]
    public abstract class ListItem : IListItem
    {
        #region Construction/Destruction

        /// <summary>
        /// Initializes new instance of the ListItem class.
        /// </summary>
        [ResDescription("ListItem_ListItem")]
        protected ListItem()
        {
        }

        /// <summary>
        /// Initializes new instance of the ListItem class as a copy of the specified ListItem object.
        /// </summary>
        /// <param name="item">Source ListItem object to copy.</param>
        protected ListItem(ListItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _isChecked = item._isChecked;
            _isCheckEnabled = item._isCheckEnabled;
            _isFocused = item._isFocused;
            _isSelected = item._isSelected;
        }

        #endregion Construction/Destruction

        #region IListItem implementation

        /// <summary>
        /// Returns value indicating whether the item has icon for specified view mode of the ThumbnailListView.
        /// </summary>
        /// <param name="view">View mode of the control for which icon is requested.</param>
        /// <returns>Returns value indicating whether item has icon or not.</returns>
        [ResDescription("IListItem_HasIcon")]
        public abstract bool HasIcon(View view);

        /// <summary>
        /// Returns value indicating whether the item has text information of specified type.
        /// </summary>
        /// <param name="textInfoId">Type of the requested text information.</param>
        /// <returns>Returns value indicating whether item has requested text or not.</returns>
        [ResDescription("IListItem_HasText")]
        public virtual bool HasText(int textInfoId)
        {
            lock (this)
            {
                object obj = _texts[textInfoId];
                string strText = obj as string;
                return (strText != null && strText.Length > 0);
            }
        }

        /// <summary>
        /// Returns index of the item's icon in corresponding image list for specified view mode of the ThumbnailListView.
        /// If item has no icon -1 is returned.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [ResDescription("IListItem_GetIconIndex")]
        public abstract object GetIconKey(View view);

        /// <summary>
        /// Returns item's text information of specified type. If item has no text of specified type, returns empty string.
        /// </summary>
        /// <param name="textInfoId"></param>
        /// <returns></returns>
        [ResDescription("IListItem_GetText")]
        public virtual string GetText(int textInfoId)
        {
            string result = string.Empty;
            lock (this)
            {
                result = _texts[textInfoId] as string;
            }
            return result;
        }

        #endregion IListItem implementation

        #region Events

        /// <summary>
        /// Occurs when item's icon has changed.
        /// </summary>
        [ResDescription("IListItem_IconChanged")]
        public event IconChangedEventHandler IconChanged;

        /// <summary>
        /// Occurs when item's text has changed.
        /// </summary>
        [ResDescription("IListItem_TextChanged")]
        public event TextChangedEventHandler TextChanged;

        /// <summary>
        /// Occurs when item's state is changing.
        /// </summary>
        [ResDescription("IListItem_StateChanging")]
        public event StateChangingEventHandler StateChanging;

        /// <summary>
        /// Occurs when item changed its state (selection, check or focus).
        /// </summary>
        [ResDescription("IListItem_StateChanged")]
        public event StateChangedEventHandler StateChanged;

        /// <summary>
        /// Raises IconChanged event of the item.
        /// </summary>
        /// <param name="imageListType">Type of image list of the changed icon.</param>
        protected virtual void OnIconChanged(View view)
        {
            if (IconChanged != null)
            {
                IconChangedEventArgs args = new IconChangedEventArgs(this, view);
                IconChanged(this, args);
            }
        }

        /// <summary>
        /// Raises TextChanged event of the item.
        /// </summary>
        /// <param name="textInfoId">Type of changed text information.</param>
        protected virtual void OnTextChanged(int textInfoId)
        {
            if (TextChanged != null)
            {
                TextChangedEventArgs args = new TextChangedEventArgs(this, textInfoId);
                TextChanged(this, args);
            }
        }

        /// <summary>
        /// Raises StateChanging event of the item.
        /// </summary>
        /// <param name="stateType">Type of state that changes.</param>
        /// <returns>true if event handlers didn't cancel state changes; otherwise, false.</returns>
        protected virtual bool OnStateChanging(StateType stateType)
        {
            if (StateChanging != null)
            {
                StateChangingEventArgs args = new StateChangingEventArgs(this, stateType);
                StateChanging(this, args);
                return !args.Cancel;
            }
            return true;
        }

        /// <summary>
        /// Raises StateChanged event.
        /// </summary>
        /// <param name="stateType">Type of state that changed.</param>
        protected virtual void OnStateChanged(StateType stateType)
        {
            if (StateChanged != null)
            {
                StateChangedEventArgs args = new StateChangedEventArgs(this, stateType);
                StateChanged(this, args);
            }
        }

        #endregion Events

        #region Public Properties

        /// <summary>
        /// Sets or gets value indicating whether item is selected or not.
        /// </summary>
        [ResDescription("IListItem_Selected")]
        public virtual bool Selected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                lock (this)
                {
                    if (_isSelected != value)
                    {
                        if (!OnStateChanging(StateType.Selection))
                            return;

                        _isSelected = value;
                        OnStateChanged(StateType.Selection);
                    }
                }
            }
        }

        /// <summary>
        /// Sets or gets value indicating whether item has focus or not.
        /// </summary>
        [ResDescription("IListItem_Focused")]
        public virtual bool Focused
        {
            get
            {
                return _isFocused;
            }
            set
            {
                lock (this)
                {
                    if (_isFocused != value)
                    {
                        if (!OnStateChanging(StateType.Focus))
                            return;

                        _isFocused = value;
                        OnStateChanged(StateType.Focus);
                    }
                }
            }
        }

        /// <summary>
        /// Sets or gets value indicating whether item can be marked with a check or not.
        /// </summary>
        [ResDescription("IListItem_CheckEnabled")]
        public virtual bool CheckEnabled
        {
            get
            {
                return _isCheckEnabled;
            }
            set
            {
                lock (this)
                {
                    if (_isCheckEnabled != value)
                    {
                        _isCheckEnabled = value;
                        if (!_isCheckEnabled)
                            Checked = false;

                        OnStateChanged(StateType.Check);
                    }
                }
            }
        }

        /// <summary>
        /// Sets or gets value indicating whether item is checked or not.
        /// </summary>
        [ResDescription("IListItem_Checked")]
        public virtual bool Checked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                lock (this)
                {
                    if (_isCheckEnabled && _isChecked != value)
                    {
                        if (!OnStateChanging(StateType.Check))
                            return;

                        _isChecked = value;
                        OnStateChanged(StateType.Check);
                    }
                }
            }
        }

        /// <summary>
        ///  Custom user defined value.
        /// </summary>
        [ResDescription("IListItem_Tag")]
        public virtual object Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        /// <summary>
        /// Reference to the parent ThumbnailListView control of the item. Every item cannot have more than one container.
        /// Every Add() or Insert() method of the ListItemCollection set this property value. Every remove method - to null.
        /// </summary>
        [ResDescription("IListItem_Parent")]
        public virtual ThumbnailListView Parent
        {
            get
            {
                return _parentControl;
            }
            set
            {
                _parentControl = value;
            }
        }

        #endregion Public Properties

        #region "Protected properties"

        protected Hashtable Texts
        {
            get
            {
                return _texts;
            }
        }

        #endregion "Protected properties"

        #region Class member variables

        /// <summary>
        /// Stores item's selection state.
        /// </summary>
        private bool _isSelected;

        /// <summary>
        /// Stores item's check state.
        /// </summary>
        private bool _isChecked;

        /// <summary>
        /// Stores item's focus state.
        /// </summary>
        private bool _isFocused;

        /// <summary>
        /// Stores value that determines whether item can be checked or not.
        /// </summary>
        private bool _isCheckEnabled = true;

        /// <summary>
        /// Hash that stores text information for all TextInfoId modes.
        /// </summary>
        private Hashtable _texts = new Hashtable();

        /// <summary>
        /// Stores reference to the parent control of the column.
        /// </summary>
        private ThumbnailListView _parentControl;

        /// <summary>
        /// Stores custom defined tag value.
        /// </summary>
        private object _tag;

        #endregion Class member variables
    }
}