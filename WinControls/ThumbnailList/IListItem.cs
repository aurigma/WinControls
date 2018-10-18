// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Determines interface of the ThumbnailListView item.
    /// </summary>
    [ResDescription("IListItem")]
    public interface IListItem
    {
        #region Methods

        /// <summary>
        /// Returns value indicating whether the item has icon for specified view mode of the ThumbnailListView.
        /// </summary>
        /// <param name="view">View mode of the control for which icon is requested.</param>
        /// <returns>Returns value indicating whether item has icon or not.</returns>
        [ResDescription("IListItem_HasIcon")]
        bool HasIcon(View view);

        /// <summary>
        /// Returns index of the item's icon in corresponding image list for specified view mode of the ThumbnailListView.
        /// If item has no icon -1 is returned.
        /// </summary>
        /// <param name="view">View mode of the control for which icon is requested.</param>
        /// <returns>Returns index of the item's icon in corresponding image list.</returns>
        [ResDescription("IListItem_GetIconIndex")]
        object GetIconKey(View view);

        /// <summary>
        /// Returns value indicating whether the item has text information of specified type.
        /// </summary>
        /// <param name="textInfoId">Type of the requested text information.</param>
        /// <returns>Returns value indicating whether item has requested text or not.</returns>
        [ResDescription("IListItem_HasText")]
        bool HasText(int textInfoId);

        /// <summary>
        /// Returns item's text information of specified type. If item has no text of specified type, returns empty string.
        /// </summary>
        /// <param name="textInfoId">Type of requested text information.</param>
        /// <returns>Item's text of the specified type.</returns>
        [ResDescription("IListItem_GetText")]
        string GetText(int textInfoId);

        #endregion Methods

        #region Events

        /// <summary>
        /// Occurs when item's icon has changed.
        /// </summary>
        [ResDescription("IListItem_IconChanged")]
        event IconChangedEventHandler IconChanged;

        /// <summary>
        /// Occurs when item's text has changed.
        /// </summary>
        [ResDescription("IListItem_TextChanged")]
        event TextChangedEventHandler TextChanged;

        /// <summary>
        /// Occurs when item's state is changing.
        /// </summary>
        [ResDescription("IListItem_StateChanging")]
        event StateChangingEventHandler StateChanging;

        /// <summary>
        /// Occurs when item changed its state (selection, check or focus).
        /// </summary>
        [ResDescription("IListItem_StateChanged")]
        event StateChangedEventHandler StateChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Sets or gets value indicating whether item is selected or not.
        /// </summary>
        [ResDescription("IListItem_Selected")]
        bool Selected
        {
            get;
            set;
        }

        /// <summary>
        /// Sets or gets value indicating whether item has focus or not.
        /// </summary>
        [ResDescription("IListItem_Focused")]
        bool Focused
        {
            get;
            set;
        }

        /// <summary>
        /// Sets or gets value indicating whether item can be marked with a check or not.
        /// </summary>
        [ResDescription("IListItem_CheckEnabled")]
        bool CheckEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Sets or gets value indicating whether item is checked or not.
        /// </summary>
        [ResDescription("IListItem_Checked")]
        bool Checked
        {
            get;
            set;
        }

        /// <summary>
        /// Every item cannot have more than one container. Every Add() or Insert() method of the ListItemCollection
        /// set this property value. Every remove method - to null.
        /// </summary>
        [ResDescription("IListItem_Parent")]
        ThumbnailListView Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Custom user defined value.
        /// </summary>
        [ResDescription("IListItem_Tag")]
        object Tag
        {
            get;
            set;
        }

        #endregion Properties
    }
}