// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Contains values for all possible view modes for the control.
    /// </summary>
    [ResDescription("View")]
    public enum View
    {
        /// <summary>
        /// Control displays items as set thumbnails of specified size.
        /// </summary>
        [ResDescription("View_Thumbnails")]
        Thumbnails = 0,

        /// <summary>
        /// Control displays items as set of large icons.
        /// </summary>
        [ResDescription("View_Icons")]
        Icons = 1,

        /// <summary>
        /// Control displays items as list of small icons.
        /// </summary>
        [ResDescription("View_List")]
        List = 2,

        /// <summary>
        /// Control displays items in a table with specified columns.
        /// </summary>
        [ResDescription("View_Details")]
        Details = 3
    }

    /// <summary>
    /// Contains values which specify possible item states.
    /// </summary>
    [ResDescription("StateType")]
    public enum StateType
    {
        /// <summary>
        /// Selection state.
        /// </summary>
        [ResDescription("StateType_Selection")]
        Selection = 0,

        /// <summary>
        /// Check state.
        /// </summary>
        [ResDescription("StateType_Check")]
        Check = 1,

        /// <summary>
        /// Focus state.
        /// </summary>
        [ResDescription("StateType_Focus")]
        Focus = 2
    }
}