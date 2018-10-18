// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Virtual list view implementation.
    /// </summary>
    [ResDescription("VirtualListView")]
    public abstract class VirtualListView : System.Windows.Forms.Control
    {
        #region "WinAPI: constants"

        private const int I_IMAGECALLBACK = (-1);
        private static IntPtr LPSTR_TEXTCALLBACKW = new IntPtr(-1);

        #endregion "WinAPI: constants"

        #region "WinAPI: List items manipulating methods"

        /// <summary>
        /// Sets a list-view item's attributes.
        /// </summary>
        /// <param name="mask">Set of flags that specify which members of this structure contain data to be set.</param>
        /// <param name="item">Zero-based index of the item.</param>
        /// <param name="subItem">One-based index of the subitem or zero if method refers to an item rather than a subitem.</param>
        /// <param name="pszText">New item text.</param>
        /// <param name="iImage">Index of the item's icon.</param>
        /// <param name="state">Indicates the item's state, state image, and overlay image.</param>
        /// <param name="stateMask">Value specifying which bits of the state member will be modified.</param>
        /// <param name="bSetTextCallback">If the value is true - text callback will be requested, otherwise pszText value will be set.</param>
        private void SetItem(uint mask, int item, int subItem, string text, int image, uint state, uint stateMask, bool setTextCallback)
        {
            if (!this.IsHandleCreated)
                return;

            NativeMethods.LVITEMW lvitem = new NativeMethods.LVITEMW();
            lvitem.cchTextMax = 0;
            lvitem.iImage = image;
            lvitem.iIndent = 0;
            lvitem.iItem = item;
            lvitem.iSubItem = subItem;
            lvitem.mask = mask;
            lvitem.state = state;
            lvitem.stateMask = stateMask;

            if (setTextCallback == true)
            {
                lvitem.pszText = LPSTR_TEXTCALLBACKW;
            }
            else
            {
                IntPtr ptrText = Marshal.StringToCoTaskMemAuto(text);
                _ptrToFree.Add(ptrText);
                lvitem.pszText = ptrText;
            }

            NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETITEMW, 0, ref lvitem);
        }

        /// <summary>
        /// Sets item state.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <param name="state">Indicates the item's state, state image, and overlay image.</param>
        /// <param name="stateMask">Value specifying which bits of the state member will be modified.</param>
        private void SetItemState(int index, uint state, uint stateMask)
        {
            if (!this.IsHandleCreated)
                return;

            NativeMethods.LVITEMW lvitem = new NativeMethods.LVITEMW();
            lvitem.iItem = index;
            lvitem.mask = NativeMethods.LVIF_STATE;
            lvitem.state = state;
            lvitem.stateMask = stateMask;
            NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETITEMW, 0, ref lvitem);
        }

        /// <summary>
        /// Sets item text
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <param name="textInfoId">Item's text type.</param>
        /// <param name="text">Text value. If value is null - text callback will be requested.</param>
        private void SetItemText(int index, int textInfoId, string text)
        {
            if (!this.IsHandleCreated)
                return;

            NativeMethods.LVITEMW lvitem = new NativeMethods.LVITEMW();
            lvitem.iItem = index;
            lvitem.mask = NativeMethods.LVIF_TEXT;
            IntPtr ptrText = LPSTR_TEXTCALLBACKW;
            if (text != null)
            {
                ptrText = Marshal.StringToCoTaskMemAuto(text);
                _ptrToFree.Add(ptrText);
            }
            lvitem.pszText = ptrText;

            if (_view != View.Details)
            {
                if (_iconicViewTextInfoId == textInfoId)
                {
                    lvitem.iSubItem = 0;
                    NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETITEMW, 0, ref lvitem);
                }
            }
            else
            {
                for (int i = 0; i < ColumnsInternal.Count; i++)
                {
                    ListColumn column = ColumnsInternal[i];
                    if (column != null && column.TextInfoId == textInfoId)
                    {
                        lvitem.iSubItem = i;
                        NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETITEMW, 0, ref lvitem);
                    }
                }
            }
        }

        /// <summary>
        /// Sets item image.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <param name="iconIndex">Icon index.</param>
        private void SetItemImage(int index, int iconIndex)
        {
            if (!this.IsHandleCreated)
                return;

            NativeMethods.LVITEMW lvitem = new NativeMethods.LVITEMW();
            lvitem.iItem = index;
            lvitem.iSubItem = 0;
            lvitem.mask = NativeMethods.LVIF_IMAGE;
            lvitem.iImage = iconIndex;

            NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETITEMW, 0, ref lvitem);
        }

        /// <summary>
        /// Returns bounding rectangle (in pixels) for specified list item.
        /// </summary>
        /// <param name="itemIndex">Item index.</param>
        /// <returns>Returns item's bounding rectangle.</returns>
        public Rectangle GetItemRect(int itemIndex)
        {
            if (!this.IsHandleCreated)
                return System.Drawing.Rectangle.Empty;

            NativeMethods.RECT rectStruct = new NativeMethods.RECT();
            rectStruct.left = NativeMethods.LVIR_BOUNDS;
            NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETITEMRECT, itemIndex, ref rectStruct);
            return Rectangle.FromLTRB(rectStruct.left, rectStruct.top, rectStruct.right, rectStruct.bottom);
        }

        private Rectangle GetItemRect(int itemIndex, int type)
        {
            if (!this.IsHandleCreated)
                return System.Drawing.Rectangle.Empty;

            NativeMethods.RECT rectStruct = new NativeMethods.RECT();
            rectStruct.left = (int)type;
            NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETITEMRECT, itemIndex, ref rectStruct);
            return Rectangle.FromLTRB(rectStruct.left, rectStruct.top, rectStruct.right, rectStruct.bottom);
        }

        /// <summary>
        /// Retrieves some or all of a list-view item's attributes.
        /// </summary>
        /// <param name="lvitem">Pointer to an LVITEM structure that specifies the information to retrieve and receives information about the list-view item.</param>
        /// <returns>true if succeded; otherwise, false.</returns>
        private bool GetAt(ref NativeMethods.LVITEMW lvitem)
        {
            if (!this.IsHandleCreated)
                return false;

            return NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETITEMW, 0, ref lvitem) != 0;
        }

        /// <summary>
        /// Inserts new item into virtual list view.
        /// </summary>
        /// <param name="mask">Set of flags that specify which members of this structure contain data to be set.</param>
        /// <param name="iItem">Zero-based index of the insert position.</param>
        /// <param name="iSubItem">One-based index of the subitem or zero if method refers to an item rather than a subitem.</param>
        /// <param name="pszText">Item's text.</param>
        /// <param name="iImage">Index of image's icon.</param>
        /// <param name="state">Indicates the item's state, state image, and overlay image.</param>
        /// <param name="stateMask">Value specifying which bits of the state member will be modified.</param>
        /// <param name="bSetTextCallback">If the value is true - text callback will be requested, otherwise pszText value will be set.</param>
        private void Insert(uint mask, int item, int subItem, string text, int image, uint state, uint stateMask, bool setTextCallback)
        {
            if (!this.IsHandleCreated)
                return;

            NativeMethods.LVITEMW lvitem = new NativeMethods.LVITEMW();
            lvitem.cchTextMax = 0;
            lvitem.iImage = image;
            lvitem.iIndent = 0;
            lvitem.iItem = item;
            lvitem.iSubItem = subItem;
            lvitem.mask = mask;
            lvitem.state = state;
            lvitem.stateMask = stateMask;

            if (setTextCallback)
                lvitem.pszText = LPSTR_TEXTCALLBACKW;
            else
            {
                IntPtr ptrText = Marshal.StringToCoTaskMemAuto(text);
                _ptrToFree.Add(ptrText);
                lvitem.pszText = ptrText;
            }

            int index = NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_INSERTITEMW, 0, ref lvitem);

            if (index != item)
                throw new Aurigma.GraphicsMill.GMException();
        }

        #endregion "WinAPI: List items manipulating methods"

        #region "WinAPI: ListView control manipulating"

        /// <summary>
        /// Changes list's column width.
        /// </summary>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="intWidth">New column width.</param>
        private void SetColumnWidth(int columnIndex, int width)
        {
            if (!this.IsHandleCreated)
                return;

            if (width == 0)
                throw new System.ArgumentOutOfRangeException("width");
            if (_view != View.List && _view != View.Details)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ThumbListShouldBeInDetailsOrListMode"));

            NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETCOLUMNWIDTH, columnIndex, (width & 0xffff));
        }

        /// <summary>
        /// Returns width of the specified column.
        /// </summary>
        /// <param name="columnIndex">Column index.</param>
        /// <returns>Width of the specified column.</returns>
        private int GetColumnWidth(int columnIndex)
        {
            if (_view != View.List && _view != View.Details)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ThumbListShouldBeInDetailsOrListMode"));

            if (this.IsHandleCreated)
                return NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETCOLUMNWIDTH, columnIndex, 0);
            else
                return 0;
        }

        /// <summary>
        /// Modifies style of the control's window.
        /// </summary>
        /// <param name="remove">Styles to remove.</param>
        /// <param name="add">Styles to add.</param>
        /// <param name="flags">Windows flags to set.</param>
        /// <returns>Returns true if style has been changed. If new style value the same as current - returns false.</returns>
        private bool ModifyStyle(uint remove, uint add, uint flags)
        {
            if (!this.IsHandleCreated)
                return false;

            uint style = (uint)NativeMethods.GetWindowLong(new HandleRef(this, this.Handle), NativeMethods.GWL_STYLE).ToInt32();
            uint newStyle = (style & ~remove) | add;

            if (style == newStyle)
                return false;

            NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL_STYLE, (int)newStyle);
            if (flags != 0)
                NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE | flags);

            return true;
        }

        /// <summary>
        /// Sets extended list style of the control.
        /// </summary>
        /// <param name="style">Value that specifies the extended list-view control style</param>
        /// <param name="mask">Value that specifies which styles in dwExStyle are to be affected.</param>
        /// <returns></returns>
        private int SetExtendedListViewStyle(uint style, uint mask)
        {
            if (this.IsHandleCreated)
            {
                // dwExMask = 0 means all styles
                return (int)NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETEXTENDEDLISTVIEWSTYLE, (int)mask, (int)style);
            }
            else
                return 0;
        }

        /// <summary>
        /// Changes the callback mask for a list-view control. The callback mask of a list-view control
        /// is a set of bit flags that specify the item states for which the application, rather than
        /// the control, stores the current data.
        /// </summary>
        /// <param name="nMask">Callback mask.</param>
        /// <returns>Returns true if successful, or false otherwise.</returns>
        private bool SetCallbackMask(uint nMask)
        {
            if (this.IsHandleCreated)
                return NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETCALLBACKMASK, nMask, 0) != 0;
            else
                return false;
        }

        /// <summary>
        /// Updates control's window style.
        /// </summary>
        protected new void UpdateStyles()
        {
            if (IsHandleCreated)
            {
                CreateParams params1 = this.CreateParams;
                int style = this.WindowStyle;
                int styleEx = this.WindowExStyle;

                if (style != params1.Style)
                {
                    this.WindowStyle = params1.Style;
                }
                if (styleEx != params1.ExStyle)
                {
                    this.WindowExStyle = params1.ExStyle;
                }
                NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_FRAMECHANGED);
                this.Invalidate(true);
            }
        }

        /// <summary>
        /// Updates extended list style.
        /// </summary>
        protected void UpdateExtendedStyles()
        {
            if (IsHandleCreated)
            {
                uint maskEx = NativeMethods.LVS_EX_ONECLICKACTIVATE |
                                NativeMethods.LVS_EX_TWOCLICKACTIVATE |
                                NativeMethods.LVS_EX_HEADERDRAGDROP |
                                NativeMethods.LVS_EX_CHECKBOXES |
                                NativeMethods.LVS_EX_FULLROWSELECT |
                                NativeMethods.LVS_EX_HEADERDRAGDROP |
                                NativeMethods.LVS_EX_GRIDLINES |
                                NativeMethods.LVS_EX_TRACKSELECT |
                                NativeMethods.LVS_EX_DOUBLEBUFFER;

                uint styleEx = NativeMethods.LVS_EX_DOUBLEBUFFER;
                switch (_activation)
                {
                    case ItemActivation.OneClick:
                        styleEx |= NativeMethods.LVS_EX_ONECLICKACTIVATE;
                        break;

                    case ItemActivation.TwoClick:
                        styleEx |= NativeMethods.LVS_EX_TWOCLICKACTIVATE;
                        break;
                }

                if (_borderSelection && _view == View.Thumbnails)
                    SetExtendedListViewStyle(NativeMethods.LVS_EX_BORDERSELECT, NativeMethods.LVS_EX_BORDERSELECT);
                else
                    SetExtendedListViewStyle(0, NativeMethods.LVS_EX_BORDERSELECT);

                if (_allowColumnReorder)
                    styleEx |= NativeMethods.LVS_EX_HEADERDRAGDROP;

                if (_checkBoxes)
                    styleEx |= NativeMethods.LVS_EX_CHECKBOXES;

                if (_fullRowSelect)
                    styleEx |= NativeMethods.LVS_EX_FULLROWSELECT;

                if (_gridlines)
                    styleEx |= NativeMethods.LVS_EX_GRIDLINES;

                if (_hoverSelection)
                    styleEx |= NativeMethods.LVS_EX_TRACKSELECT;

                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETEXTENDEDLISTVIEWSTYLE, maskEx, styleEx);
                base.Invalidate();
            }
        }

        /// <summary>
        /// Inserts column to the control.
        /// </summary>
        /// <param name="column">The column to insert.</param>
        /// <param name="index">The zero-based index location where the column header is inserted.</param>
        private void InsertColumn(ListColumn column, int index)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            if (!this.IsHandleCreated)
                return;

            NativeMethods.LVCOLUMNW col = new NativeMethods.LVCOLUMNW();
            col.mask = NativeMethods.LVCF_FMT | NativeMethods.LVCF_WIDTH | NativeMethods.LVCF_TEXT | NativeMethods.LVCF_SUBITEM;
            col.iSubItem = index;
            col.cx = column.Width;
            col.pszText = column.Caption;

            switch (column.TextAlignment)
            {
                case HorizontalAlignment.Left:
                    col.fmt = NativeMethods.LVCFMT_LEFT;
                    break;

                case HorizontalAlignment.Center:
                    col.fmt = NativeMethods.LVCFMT_CENTER;
                    break;

                case HorizontalAlignment.Right:
                    col.fmt = NativeMethods.LVCFMT_RIGHT;
                    break;
            }

            NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_INSERTCOLUMNW, index, ref col);
            InvalidateAllItems();
        }

        /// <summary>
        /// Invalidates all items of the control.
        /// </summary>
        private void InvalidateAllItems()
        {
            InvalidateColumn(0);
            for (int j = 1; j < ColumnsInternal.Count; j++)
            {
                InvalidateColumn(j);
            }

            Update();
        }

        /// <summary>
        /// Invalidates all items of the column.
        /// </summary>
        /// <param name="columnIndex">Column index.</param>
        private void InvalidateColumn(int columnIndex)
        {
            if (this.IsHandleCreated)
            {
                for (int i = 0; i < ItemsInternal.Count; i++)
                {
                    NativeMethods.LVITEMW lvitem = new NativeMethods.LVITEMW();
                    lvitem.iItem = i;
                    lvitem.iSubItem = columnIndex;
                    lvitem.mask = NativeMethods.LVIF_TEXT | NativeMethods.LVIF_IMAGE;
                    IntPtr ptrText = LPSTR_TEXTCALLBACKW;
                    lvitem.pszText = ptrText;
                    lvitem.iImage = I_IMAGECALLBACK;
                    NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETITEMW, 0, ref lvitem);
                }
            }
        }

        /// <summary>
        /// Sets the attributes of a list-view column.
        /// </summary>
        /// <param name="column">The column to set.</param>
        private void SetColumn(ListColumn column)
        {
            if (column == null)
                return;
            if (!this.IsHandleCreated)
                return;

            int itemIndex = this.ColumnsInternal.IndexOf(column);
            if (itemIndex < 0)
                return;

            NativeMethods.LVCOLUMNW col = new NativeMethods.LVCOLUMNW();
            col.mask = NativeMethods.LVCF_FMT | NativeMethods.LVCF_WIDTH | NativeMethods.LVCF_TEXT | NativeMethods.LVCF_SUBITEM;
            col.iSubItem = itemIndex;
            col.cx = column.Width;
            col.pszText = column.Caption;

            switch (column.TextAlignment)
            {
                case HorizontalAlignment.Left:
                    col.fmt = NativeMethods.LVCFMT_LEFT;
                    break;

                case HorizontalAlignment.Center:
                    col.fmt = NativeMethods.LVCFMT_CENTER;
                    break;

                case HorizontalAlignment.Right:
                    col.fmt = NativeMethods.LVCFMT_RIGHT;
                    break;
            }

            NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETCOLUMNW, itemIndex, ref col);
        }

        #endregion "WinAPI: ListView control manipulating"

        #region Construction/Destruction

        /// <summary>
        /// Initializes new instance of the VirtualListView class.
        /// </summary>
        [ResDescription("VirtualListView_VirtualListView")]
        protected VirtualListView()
        {
            SetStyle(ControlStyles.UserPaint, false);

            _queueManager = new QueueManager();

            _largeImageList = new SystemImageList(true);
            _smallImageList = new SystemImageList(false);
            _thumbImageList = new ThumbnailImageList();
        }

        protected override void Dispose(bool disposing)
        {
            _disposed = true;
            UnregisterEvents();
            DeleteTempFile(_backgroundImageFilename);

            if (disposing)
            {
                _queueManager.Clear();

                _thumbImageList.Dispose();
                _largeImageList.Dispose();
                _smallImageList.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion Construction/Destruction

        #region "Other service functions"

        private static uint IndexToStateImageMask(uint i)
        {
            return ((i) << 12);
        }

        #endregion "Other service functions"

        #region "Inherited from control & Message-loop functions"

        /// <summary>
        /// Gets the required creation parameters when the control handle is created.
        /// </summary>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
            get
            {
                CreateParams result = base.CreateParams;

                result.ClassName = "SysListView32";
                result.Style |= (NativeMethods.WS_TABSTOP | NativeMethods.WS_CHILD | NativeMethods.WS_CLIPSIBLINGS | NativeMethods.WS_CLIPCHILDREN);

                result.Style |= (int)NativeMethods.LVS_SHAREIMAGELISTS;

                if (this._autoArrange)
                    result.Style |= (int)NativeMethods.LVS_AUTOARRANGE;

                switch (this._borderStyle)
                {
                    case BorderStyle.FixedSingle:
                        {
                            result.Style |= (int)NativeMethods.WS_BORDER;
                            break;
                        }
                    case BorderStyle.Fixed3D:
                        {
                            result.ExStyle |= (int)NativeMethods.WS_EX_CLIENTEDGE;
                            break;
                        }
                }

                switch (this._headerStyle)
                {
                    case ColumnHeaderStyle.None:
                        result.Style |= (int)NativeMethods.LVS_NOCOLUMNHEADER;
                        break;

                    case ColumnHeaderStyle.Nonclickable:
                        result.Style |= (int)NativeMethods.LVS_NOSORTHEADER;
                        break;
                }

                if (!this._labelWrap)
                    result.Style |= (int)NativeMethods.LVS_NOLABELWRAP;

                if (!this._hideSelection)
                    result.Style |= (int)NativeMethods.LVS_SHOWSELALWAYS;

                if (!this._multiSelect)
                    result.Style |= (int)NativeMethods.LVS_SINGLESEL;

                if (this._labelEdit)
                    result.Style |= (int)NativeMethods.LVS_EDITLABELS;

                result.Style |= (int)ViewToStyle(_view);

                return result;
            }
        }

        /// <summary>
        /// Overrides Control.OnHandleCreated method to add custom operations on HandleCreated event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (bOnHandleCreatedHasBeenCalled)
                return;
            bOnHandleCreatedHasBeenCalled = true;

            SetCallbackMask(NativeMethods.LVIS_SELECTED | NativeMethods.LVIS_FOCUSED | NativeMethods.LVIS_STATEIMAGEMASK);
            UpdateExtendedStyles();

            if (_backColorSet)
            {
                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETBKCOLOR, 0, ColorTranslator.ToWin32(this.BackColor));
                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETTEXTBKCOLOR, 0, ColorTranslator.ToWin32(this.BackColor));
            }

            SetImageList(View.List, GetImageList(View.List).Handle);
            SetImageList(_view, GetImageList(_view).Handle);

            if (_view == View.List)
                UpdateListModeColumnWidth();
        }

        private bool GetShouldLockForMessage(int msg)
        {
            return (
                msg == NativeMethods.WM_ERASEBKGND ||
                msg == NativeMethods.WM_HSCROLL ||
                msg == NativeMethods.WM_VSCROLL ||
                msg == NativeMethods.WM_CHAR ||
                msg == NativeMethods.WM_DESTROY ||
                msg == NativeMethods.WM_KEYDOWN ||
                msg == NativeMethods.WM_KILLFOCUS ||
                msg == NativeMethods.WM_LBUTTONDOWN ||
                msg == NativeMethods.WM_NOTIFY ||
                msg == NativeMethods.WM_PAINT ||
                msg == NativeMethods.WM_RBUTTONDOWN ||
                msg == NativeMethods.WM_SETFOCUS ||
                msg == NativeMethods.WM_TIMER ||
                msg == NativeMethods.WM_WINDOWPOSCHANGED ||
                msg == NativeMethods.WM_SETTINGCHANGE);
        }

        /// <summary>
        /// Implementation of the window procedure of the control.
        /// </summary>
        /// <param name="m">Message to process.</param>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            bool shouldLock = GetShouldLockForMessage(m.Msg);

            if (shouldLock)
                System.Threading.Monitor.Enter(_thumbImageList);

            try
            {
                switch (m.Msg)
                {
                    case (int)NativeMethods.WM_MOUSEHOVER:
                        if (this._hoverSelection)
                            base.WndProc(ref m);

                        return;

                    case (int)NativeMethods.WM_SETFOCUS:
                        base.WndProc(ref m);
                        if (this.FocusedItem == null && this.ItemsInternal.Count > 0)
                            ItemsInternal[0].Focused = true;

                        return;

                    case (int)NativeMethods.WM_NOTIFY + 0x2000:
                        ReflectNotify(ref m);
                        break;

                    case (int)NativeMethods.WM_NOTIFY:
                        OnNotify(ref m);
                        break;

                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
            finally
            {
                if (shouldLock)
                    System.Threading.Monitor.Exit(_thumbImageList);
            }
        }

        /// <summary>
        /// Processes main ListView messages.
        /// </summary>
        /// <param name="m">Message for processing.</param>
        protected void ReflectNotify(ref System.Windows.Forms.Message message)
        {
            IListItem item;
            Aurigma.NativeMethods.LVITEMW lvItem;

            NativeMethods.NMHDR hdr = new NativeMethods.NMHDR(message.LParam);

            switch ((int)hdr.code)
            {
                case NativeMethods.LVN_GETDISPINFOW:
                    {
                        if (_modifyingItem)
                            break;

                        NativeMethods.LVDISPINFOW dispinfo = new NativeMethods.LVDISPINFOW(message.LParam);
                        if ((item = this.ItemsInternal[dispinfo.item.iItem]) == null)
                            throw new ObjectEmptyException();

                        bool changed = false;

                        if ((dispinfo.item.mask & NativeMethods.LVIF_TEXT) != 0)
                        {
                            int columnTextInfoId = _iconicViewTextInfoId;
                            if (_view == View.Details && ColumnsInternal.Count > 0)
                                columnTextInfoId = ColumnsInternal[dispinfo.item.iSubItem].TextInfoId;

                            string text = item.GetText(columnTextInfoId);
                            IntPtr ptrText = Marshal.StringToCoTaskMemAuto(text);
                            _ptrToFree.Add(ptrText);
                            lvItem = dispinfo.item;
                            lvItem.pszText = ptrText;
                            dispinfo.item = lvItem;
                            changed = true;
                        }

                        if ((dispinfo.item.mask & NativeMethods.LVIF_IMAGE) != 0)
                        {
                            int iconIndex = GetImageList(_view).IndexOfKey(item.GetIconKey(_view));
                            if (dispinfo.item.iImage != iconIndex)
                            {
                                lvItem = dispinfo.item;
                                lvItem.iImage = iconIndex;
                                dispinfo.item = lvItem;
                                changed = true;
                            }
                        }

                        if ((dispinfo.item.mask & NativeMethods.LVIF_STATE) != 0 && (dispinfo.item.iSubItem == 0 || _fullRowSelect))
                        {
                            uint uintState = 0;
                            if (this.CheckBoxes && item.CheckEnabled)
                                uintState |= IndexToStateImageMask(item.Checked ? 2u : 1u);

                            if (item.Selected)
                            {
                                uintState |= NativeMethods.LVIS_SELECTED;
                                lvItem = dispinfo.item;
                                lvItem.stateMask |= NativeMethods.LVIS_SELECTED;
                                dispinfo.item = lvItem;
                            }

                            if (item.Focused)
                            {
                                uintState |= NativeMethods.LVIS_FOCUSED;
                                lvItem = dispinfo.item;
                                lvItem.stateMask |= NativeMethods.LVIS_FOCUSED;
                                dispinfo.item = lvItem;
                            }

                            if (dispinfo.item.state != uintState)
                            {
                                uintState |= NativeMethods.LVIS_FOCUSED;
                                lvItem = dispinfo.item;
                                lvItem.state = uintState;
                                dispinfo.item = lvItem;
                                changed = true;
                            }
                        }

                        if (changed)
                            dispinfo.StructToPointer(message.LParam);

                        break;
                    }

                case NativeMethods.LVN_ITEMCHANGING:
                    {
                        message.Result = IntPtr.Zero;
                        if (_modifyingItem)
                            break;

                        NativeMethods.NMLISTVIEW itemChanged = new NativeMethods.NMLISTVIEW(message.LParam);
                        item = this.ItemsInternal[itemChanged.iItem];
                        if (item == null)
                            throw new ObjectEmptyException();

                        //
                        // Processing check boxes - changing check-state for all currently selected items.
                        //
                        if (_currentChangedItemIndex == itemChanged.iItem || !item.CheckEnabled)
                            break;

                        if ((itemChanged.uChanged & NativeMethods.LVIF_STATE) != 0)
                        {
                            bool newCheckState = ((itemChanged.uNewState & IndexToStateImageMask(2)) != 0);
                            if ((itemChanged.uNewState & (IndexToStateImageMask(1) | IndexToStateImageMask(2))) != 0 && item.Checked != newCheckState)
                            {
                                item.Checked = newCheckState;
                                if (_multiSelect && item.Selected)
                                {
                                    for (int i = 0; i < this.ItemsInternal.Count; i++)
                                    {
                                        if (this.ItemsInternal[i].Selected)
                                            this.ItemsInternal[i].Checked = newCheckState;
                                    }
                                }
                            }
                        }
                        break;
                    }

                case NativeMethods.LVN_ITEMCHANGED:
                    {
                        if (_modifyingItem)
                        {
                            message.Result = IntPtr.Zero;
                            break;
                        }

                        NativeMethods.NMLISTVIEW itemChanged = new NativeMethods.NMLISTVIEW(message.LParam);
                        item = this.ItemsInternal[itemChanged.iItem];
                        if (item == null)
                            throw new ObjectEmptyException();

                        if ((itemChanged.uChanged & NativeMethods.LVIF_STATE) != 0 && itemChanged.iSubItem == 0)
                        {
                            if ((itemChanged.uOldState & NativeMethods.LVIS_SELECTED) != (itemChanged.uNewState & NativeMethods.LVIS_SELECTED))
                            {
                                bool selectState = ((itemChanged.uNewState & NativeMethods.LVIS_SELECTED) != 0);
                                if (selectState && !_multiSelect)
                                {
                                    for (int i = 0; i < itemChanged.iItem; i++)
                                        ItemsInternal[i].Selected = false;
                                    for (int i = itemChanged.iItem + 1; i < ItemsInternal.Count; i++)
                                        ItemsInternal[i].Selected = false;
                                }
                                item.Selected = selectState;
                            }

                            if ((itemChanged.uOldState & NativeMethods.LVIS_FOCUSED) != (itemChanged.uNewState & NativeMethods.LVIS_FOCUSED))
                            {
                                bool focusState = ((itemChanged.uNewState & NativeMethods.LVIS_FOCUSED) != 0);
                                if (focusState)
                                {
                                    for (int i = 0; i < itemChanged.iItem; i++)
                                        ItemsInternal[i].Focused = false;
                                    for (int i = itemChanged.iItem + 1; i < ItemsInternal.Count; i++)
                                        ItemsInternal[i].Focused = false;
                                }
                                item.Focused = focusState;
                            }
                        }
                        break;
                    }

                case NativeMethods.LVN_COLUMNCLICK:
                    {
                        NativeMethods.NMLISTVIEW notifyInfo = new NativeMethods.NMLISTVIEW(message.LParam);
                        ColumnsInternal.OnColumnClick(notifyInfo.iSubItem);
                        break;
                    }

                case NativeMethods.LVN_BEGINLABELEDIT:
                    {
                        NativeMethods.LVDISPINFOW dispinfo = new NativeMethods.LVDISPINFOW(message.LParam);
                        if (OnLabelEditing(dispinfo.item.iItem))
                            message.Result = IntPtr.Zero;
                        else
                            message.Result = (IntPtr)1;

                        break;
                    }

                case NativeMethods.LVN_ENDLABELEDIT:
                    {
                        NativeMethods.LVDISPINFOW dispinfo = new NativeMethods.LVDISPINFOW(message.LParam);

                        string newText = null;
                        if (dispinfo.item.pszText != IntPtr.Zero)
                            newText = Marshal.PtrToStringAuto(dispinfo.item.pszText);

                        OnLabelEdited(dispinfo.item.iItem, newText);
                        break;
                    }

                case NativeMethods.LVN_ITEMACTIVATE:
                    {
                        OnItemActivate(System.EventArgs.Empty);
                        message.Result = IntPtr.Zero;
                        break;
                    }

                case NativeMethods.LVN_BEGINDRAG:
                    {
                        NativeMethods.NMLISTVIEW notifyInfo = new NativeMethods.NMLISTVIEW(message.LParam);
                        OnBeginDrag(notifyInfo.iItem, System.Windows.Forms.MouseButtons.Left);
                        break;
                    }

                case NativeMethods.LVN_BEGINRDRAG:
                    {
                        NativeMethods.NMLISTVIEW notifyInfo = new NativeMethods.NMLISTVIEW(message.LParam);
                        OnBeginDrag(notifyInfo.iItem, System.Windows.Forms.MouseButtons.Right);
                        break;
                    }

                case NativeMethods.NM_CLICK:
                    {
                        NmMouseClick(System.Windows.Forms.MouseButtons.Left);
                        break;
                    }

                case NativeMethods.NM_RCLICK:
                    {
                        NmMouseClick(System.Windows.Forms.MouseButtons.Right);
                        break;
                    }

                default:
                    base.WndProc(ref message);
                    break;
            }
        }

        private void NmMouseClick(System.Windows.Forms.MouseButtons mouseButton)
        {
            System.Drawing.Point pnt = this.PointToClient(System.Windows.Forms.Cursor.Position);

            base.OnMouseDown(new System.Windows.Forms.MouseEventArgs(mouseButton, 1, pnt.X, pnt.Y, 0));
            base.OnClick(System.EventArgs.Empty);
            base.OnMouseUp(new System.Windows.Forms.MouseEventArgs(mouseButton, 1, pnt.X, pnt.Y, 0));
        }

        private void OnNotify(ref System.Windows.Forms.Message message)
        {
            NativeMethods.NMHDR hdr = new NativeMethods.NMHDR(message.LParam);
            switch ((int)hdr.code)
            {
                case NativeMethods.HDN_ENDTRACK:
                    NativeMethods.NMHEADER header = new NativeMethods.NMHEADER(message.LParam);
                    this.ColumnsInternal[header.iItem].OnColumnChangedInternal(ColumnChangeType.Width);
                    break;

                default:
                    base.DefWndProc(ref message);
                    break;
            }
        }

        #endregion "Inherited from control & Message-loop functions"

        #region Overridable Properties

        internal abstract ListItemCollection ItemsInternal
        {
            get;
        }

        internal abstract ListColumnCollection ColumnsInternal
        {
            get;
        }

        #endregion Overridable Properties

        #region "Public methods"

        public void ScrollHorizontally(bool scrollForward, bool scrollPage)
        {
            if (this.IsHandleCreated)
            {
                int scrollConst;
                if (scrollForward)
                    scrollConst = (int)(scrollPage ? NativeMethods.SB_PAGEDOWN : NativeMethods.SB_LINEDOWN);
                else
                    scrollConst = (int)(scrollPage ? NativeMethods.SB_PAGEUP : NativeMethods.SB_LINEUP);

                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_HSCROLL, scrollConst, 0);
            }
        }

        public void ScrollVertically(bool scrollForward, bool scrollPage)
        {
            if (this.IsHandleCreated)
            {
                int scrollConst;
                if (scrollForward)
                    scrollConst = (int)(scrollPage ? NativeMethods.SB_PAGEDOWN : NativeMethods.SB_LINEDOWN);
                else
                    scrollConst = (int)(scrollPage ? NativeMethods.SB_PAGEUP : NativeMethods.SB_LINEUP);

                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_VSCROLL, scrollConst, 0);
            }
        }

        public void Scroll(int dx, int dy)
        {
            if (this.IsHandleCreated)
                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SCROLL, dx, dy);
        }

        /// <summary>
        /// Arranges items in the control when they are displayed as icons.
        /// </summary>
        /// <param name="value"></param>
        [ResDescription("VirtualListView_ArrangeIcons")]
        public void ArrangeIcons(ListViewAlignment value)
        {
            if (this.IsHandleCreated && _view != View.Details)
                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_ARRANGE, (int)value, 0);
        }

        /// <summary>
        /// Ensures that a list-view item is either entirely or partially visible, scrolling the list-view control if necessary.
        /// </summary>
        /// <param name="itemIndex">Index of the ThumbnailListView item.</param>
        /// <returns>true if successful, or false otherwise.</returns>
        [ResDescription("VirtualListView_EnsureVisible")]
        public bool EnsureVisible(int itemIndex)
        {
            if (this.IsHandleCreated)
                return (NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_ENSUREVISIBLE, itemIndex, 1) != 0);
            else
                return false;
        }

        /// <summary>
        /// Prevents changes in that window from being redrawn. Can be useful if an application must add several items to the control.
        /// </summary>
        [ResDescription("VirtualListView_BeginUpdate")]
        public void BeginUpdate()
        {
            if (this.IsHandleCreated)
                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETREDRAW, 0, 0);
        }

        /// <summary>
        /// Allows changes in that window to be redrawn. Must be called after BeginUpdate() call.
        /// </summary>
        [ResDescription("VirtualListView_EndUpdate")]
        public void EndUpdate()
        {
            if (this.IsHandleCreated)
                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETREDRAW, 1, 0);
        }

        #endregion "Public methods"

        #region "Private & Internal methods"

        /// <summary>
        /// Inserts item into virtual list control.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The zero-based index location where the item should be inserted.</param>
        private void InsertAttach(IListItem item, int index)
        {
            uint uintItemState = 0;
            if (item.Selected)
                uintItemState |= NativeMethods.LVIS_SELECTED;
            if (item.Focused)
                uintItemState |= NativeMethods.LVIS_FOCUSED;

            if (item.Checked)
                uintItemState |= IndexToStateImageMask(2);
            else if (item.CheckEnabled)
                uintItemState |= IndexToStateImageMask(1);

            int zeroColumnTextInfoId = _iconicViewTextInfoId;
            if (_view == View.Details && ColumnsInternal.Count > 0)
                zeroColumnTextInfoId = ColumnsInternal[0].TextInfoId;

            _modifyingItem = true;
            Insert(NativeMethods.LVIF_TEXT | NativeMethods.LVIF_IMAGE, index, 0, item.GetText(zeroColumnTextInfoId), I_IMAGECALLBACK, 0, 0, true);
            _modifyingItem = false;

            for (int i = 1; i < ColumnsInternal.Count; i++)
            {
                ListColumn column = ColumnsInternal[i];
                if (column == null)
                    continue;

                _modifyingItem = true;
                SetItem(NativeMethods.LVIF_TEXT, index, i, item.GetText(column.TextInfoId), 0, 0, 0, true);
                _modifyingItem = false;
            }

            _currentChangedItemIndex = index;
            SetItemState(index, uintItemState, NativeMethods.LVIS_FOCUSED | NativeMethods.LVIS_SELECTED | NativeMethods.LVIS_STATEIMAGEMASK);
            _currentChangedItemIndex = -1;
        }

        /// <summary>
        /// Converts View enumeration to list view style.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private static uint ViewToStyle(View view)
        {
            switch (view)
            {
                case View.Thumbnails:
                    return NativeMethods.LVS_ICON;

                case View.Icons:
                    return NativeMethods.LVS_ICON;

                case View.List:
                    return NativeMethods.LVS_LIST;

                case View.Details:
                    return NativeMethods.LVS_REPORT;

                default:
                    return NativeMethods.LVS_REPORT;
            }
        }

        internal void SetSortOrderIcon(int columnIndex, bool ascendingIcon)
        {
            if (this.IsHandleCreated)
            {
                IntPtr headerWnd = (IntPtr)NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETHEADER, 0, 0);

                NativeMethods.HDITEM hdItem = new NativeMethods.HDITEM();
                hdItem.mask = NativeMethods.HDI_FORMAT;
                NativeMethods.SendMessage(headerWnd, NativeMethods.HDM_GETITEM, columnIndex, ref hdItem);

                hdItem.fmt &= ~(NativeMethods.HDF_SORTUP | NativeMethods.HDF_SORTDOWN);
                hdItem.fmt |= (ascendingIcon ? NativeMethods.HDF_SORTUP : NativeMethods.HDF_SORTDOWN);
                NativeMethods.SendMessage(headerWnd, NativeMethods.HDM_SETITEM, columnIndex, ref hdItem);
            }
        }

        internal void RemoveSortOrderIcon(int columnIndex)
        {
            if (this.IsHandleCreated)
            {
                IntPtr headerWnd = (IntPtr)NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETHEADER, 0, 0);

                NativeMethods.HDITEM hdItem = new NativeMethods.HDITEM();
                hdItem.mask = NativeMethods.HDI_FORMAT;
                NativeMethods.SendMessage(headerWnd, NativeMethods.HDM_GETITEM, columnIndex, ref hdItem);

                hdItem.fmt &= ~(NativeMethods.HDF_SORTUP | NativeMethods.HDF_SORTDOWN);
                NativeMethods.SendMessage(headerWnd, NativeMethods.HDM_SETITEM, columnIndex, ref hdItem);
            }
        }

        public void UpdateItem(int index)
        {
            IListItem item = this.ItemsInternal[index];

            uint mask = NativeMethods.LVIF_IMAGE | NativeMethods.LVIF_TEXT;
            int image = GetImageList(_view).IndexOfKey(item.GetIconKey(_view));

            _modifyingItem = true;
            try
            {
                if (_view != View.Details)
                {
                    SetItem(mask, index, 0, item.GetText(_iconicViewTextInfoId), image, 0, 0, false);
                }
                else if (this.ColumnsInternal.Count > 0)
                {
                    SetItem(mask, index, 0, item.GetText(this.ColumnsInternal[0].TextInfoId), image, 0, 0, false);

                    mask = NativeMethods.LVIF_TEXT;
                    for (int i = 1; i < this.ColumnsInternal.Count; i++)
                        SetItem(mask, index, i, item.GetText(this.ColumnsInternal[i].TextInfoId), 0, 0, 0, false);
                }

                uint stateMask = NativeMethods.LVIS_STATEIMAGEMASK | NativeMethods.LVIS_FOCUSED | NativeMethods.LVIS_SELECTED;
                uint state = 0;
                if (item.Selected)
                    state |= NativeMethods.LVIS_SELECTED;
                if (item.Focused)
                    state |= NativeMethods.LVIS_FOCUSED;
                if (item.CheckEnabled)
                {
                    if (item.Checked)
                        state |= IndexToStateImageMask(2);
                    else
                        state |= IndexToStateImageMask(1);
                }

                SetItemState(index, state, stateMask);
            }
            finally
            {
                _modifyingItem = false;
            }
        }

        #endregion "Private & Internal methods"

        #region Events

        /// <summary>
        /// Occurs when the control view mode has been changed.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_ViewChanged")]
        public event ViewChangedEventHandler ViewChanged;

        /// <summary>
        /// Raises ViewChanged event of the control.
        /// </summary>
        /// <param name="view">New view mode.</param>
        protected virtual void OnViewChanged(View view)
        {
            if (ViewChanged != null)
            {
                ViewChangedEventArgs args = new ViewChangedEventArgs(view);
                ViewChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when the text of an item is about to be edited by the user.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_LabelEditing")]
        public event LabelEditingEventHandler LabelEditing;

        /// <summary>
        /// Raises BeforeLabelEdit event of the control.
        /// </summary>
        /// <param name="index">Index of the item whose label is changing.</param>
        /// <returns></returns>
        protected virtual bool OnLabelEditing(int index)
        {
            if (LabelEditing == null)
                return true;

            ItemActionRequestEventArgs args = new ItemActionRequestEventArgs(ItemsInternal[index]);
            LabelEditing(this, args);

            return !args.Cancel;
        }

        /// <summary>
        /// Occurs when the text of an item has been edited by the user.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_LabelEdited")]
        public event LabelEditedEventHandler LabelEdited;

        /// <summary>
        /// Raises LabelEdit event of the control.
        /// </summary>
        /// <param name="index">Index of the item whose label has been changed.</param>
        /// <param name="newText">Entered text.</param>
        protected virtual void OnLabelEdited(int index, string newText)
        {
            if (LabelEdited == null)
                return;

            LabelEditedEventArgs args = new LabelEditedEventArgs(ItemsInternal[index], newText);
            LabelEdited(this, args);
        }

        /// <summary>
        /// Occurs when the set of currently selected items has been changed.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_SelectedItemsChanged")]
        public event EventHandler SelectedItemsChanged;

        /// <summary>
        /// Raises SelectedItemsChanged event.
        /// </summary>
        protected virtual void OnSelectedItemsChanged(System.EventArgs e)
        {
            if (SelectedItemsChanged == null)
                return;

            SelectedItemsChanged(this, e);
        }

        /// <summary>
        /// Occurs when an item is activated.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_ItemActivate")]
        public event EventHandler ItemActivate;

        /// <summary>
        /// Raises ItemActivate event.
        /// </summary>
        protected virtual void OnItemActivate(System.EventArgs e)
        {
            if (ItemActivate != null)
                ItemActivate(this, e);
        }

        /// <summary>
        /// Occurs when the user begins dragging an item.
        /// </summary>
        [CategoryAttribute("Action")]
        [ResDescription("VirtualListView_ItemDrag")]
        public event ItemDragEventHandler ItemDrag;

        /// <summary>
        /// Raises ItemDrag event of the control.
        /// </summary>
        /// <param name="itemIndex">Index of the dragged item.</param>
        /// <param name="mouseButton">The name of the mouse button that was clicked during the drag operation.</param>
        protected virtual void OnBeginDrag(int itemIndex, System.Windows.Forms.MouseButtons mouseButton)
        {
            if (ItemDrag == null)
                return;

            ItemDragEventArgs e = new ItemDragEventArgs(mouseButton, ItemsInternal[itemIndex]);
            ItemDrag(this, e);
        }

        #endregion Events

        #region Internal Events

        /// <summary>
        /// Registers handlers for events of the items and columns collections.
        /// </summary>
        protected void RegisterEvents()
        {
            _largeImageList.ImageRemoved += new ImageRemovedEventHandler(OnImageListImageRemoved);
            _smallImageList.ImageRemoved += new ImageRemovedEventHandler(OnImageListImageRemoved);
            _thumbImageList.ImageRemoved += new ImageRemovedEventHandler(OnImageListImageRemoved);

            this.ItemsInternal.ItemsInserted += new ItemsInsertedEventHandler(ItemsInserted);
            this.ItemsInternal.ItemsRemoved += new ItemsRemovedEventHandler(ItemsRemoved);
            this.ItemsInternal.ItemsRemovedAll += new EventHandler(ItemsRemovedAll);

            this.ItemsInternal.IconChanged += new IconChangedEventHandler(IconChanged);
            this.ItemsInternal.TextChanged += new TextChangedEventHandler(TextChanged);
            this.ItemsInternal.StateChanged += new StateChangedEventHandler(StateChanged);

            this.ColumnsInternal.ColumnsInserted += new ColumnsInsertedEventHandler(ColumnsInserted);
            this.ColumnsInternal.ColumnsRemoved += new ColumnsRemovedEventHandler(ColumnsRemoved);

            this.ColumnsInternal.ColumnChanged += new ColumnChangedEventHandler(ColumnChanged);
        }

        /// <summary>
        /// Unregister event handlers of the items and columns collections (See also RegisterEvents method).
        /// </summary>
        protected void UnregisterEvents()
        {
            this.ItemsInternal.ItemsInserted -= new ItemsInsertedEventHandler(ItemsInserted);
            this.ItemsInternal.ItemsRemoved -= new ItemsRemovedEventHandler(ItemsRemoved);
            this.ItemsInternal.ItemsRemovedAll -= new EventHandler(ItemsRemovedAll);

            this.ItemsInternal.IconChanged -= new IconChangedEventHandler(IconChanged);
            this.ItemsInternal.TextChanged -= new TextChangedEventHandler(TextChanged);
            this.ItemsInternal.StateChanged -= new StateChangedEventHandler(StateChanged);

            this.ColumnsInternal.ColumnsInserted -= new ColumnsInsertedEventHandler(ColumnsInserted);
            this.ColumnsInternal.ColumnsRemoved -= new ColumnsRemovedEventHandler(ColumnsRemoved);

            this.ColumnsInternal.ColumnChanged -= new ColumnChangedEventHandler(ColumnChanged);
        }

        /// <summary>
        /// Internal handler of the ColumnsInserted event of the columns collection.
        /// Method adds columns to the virtual list view control.
        /// </summary>
        private void ColumnsInserted(Object sender, ColumnsInsertedEventArgs args)
        {
            for (int i = 0; i < args.Columns.Length; i++)
                InsertColumn(args.Columns[i], args.Index + i);
        }

        /// <summary>
        /// Internal handler of the ColumnsRemoved event of the columns collection.
        /// Method removes corresponding columns from the virtual list view control.
        /// </summary>
        private void ColumnsRemoved(Object sender, ColumnsRemovedEventArgs args)
        {
            if (this.IsHandleCreated)
            {
                foreach (int index in args.RemovedColumnsIndices)
                    NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_DELETECOLUMN, index, 0);

                ArrangeIcons(ListViewAlignment.Default);
            }
        }

        /// <summary>
        /// Internal handler of the ColumnChanged event of the columns collection.
        /// Method updates corresponding color of the virtual list view control.
        /// </summary>
        private void ColumnChanged(Object sender, ColumnChangedEventArgs args)
        {
            if (args.ChangeType != ColumnChangeType.Width)
            {
                SetColumn(args.Column);
                InvalidateColumn(ColumnsInternal.IndexOf(args.Column));
            }
        }

        /// <summary>
        /// Internal handler of the ItemsInserted event of the items collection.
        /// Method adds items into the virtual list view control.
        /// </summary>
        private void ItemsInserted(Object sender, ItemsInsertedEventArgs args)
        {
            bool selectionChanged = false;

            BeginUpdate();
            try
            {
                for (int i = 0; i < args.Items.Length; i++)
                {
                    InsertAttach(args.Items[i], args.Index + i);

                    if (args.Items[i].Selected)
                        selectionChanged = true;
                }
            }
            finally
            {
                EndUpdate();
            }

            if (selectionChanged)
                OnSelectedItemsChanged(System.EventArgs.Empty);

            ArrangeIcons(ListViewAlignment.Default);
        }

        /// <summary>
        /// Moves an item to a specified position in a list-view control (must be in icon or list view).
        /// </summary>
        /// <param name="itemIndex">Index of the item for which to set the position.</param>
        /// <param name="position">New position of the item, in list-view coordinates.</param>
        public void SetItemPosition(int itemIndex, System.Drawing.Point position)
        {
            if (_view != View.Icons && _view != View.List && _view != View.Thumbnails)
                return;
            if (!this.IsHandleCreated)
                return;

            NativeMethods.POINT winApiPoint = new NativeMethods.POINT();
            winApiPoint.x = position.X;
            winApiPoint.y = position.Y;

            NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETITEMPOSITION32, itemIndex, ref winApiPoint);
        }

        /// <summary>
        /// Retrieves the position of a list-view item.
        /// </summary>
        /// <param name="itemIndex">Index of the list-view item. </param>
        /// <param name="position">Pointer to a variable that receives the item's position in list-view coordinates.</param>
        /// <returns>returns true if successful, or false otherwise.</returns>
        public bool GetItemPosition(int itemIndex, out System.Drawing.Point position)
        {
            position = System.Drawing.Point.Empty;
            bool result = false;

            if (this.IsHandleCreated)
            {
                NativeMethods.POINT winApiPoint = new NativeMethods.POINT();
                result = (NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETITEMPOSITION, itemIndex, ref winApiPoint) != 0);
                if (result)
                {
                    position.X = winApiPoint.x;
                    position.Y = winApiPoint.y;
                }
            }

            return result;
        }

        /// <summary>
        /// Internal handler of the ItemsRemoved event of the items collection.
        /// Method removes corresponding items from the virtual list view control.
        /// </summary>
        private void ItemsRemoved(Object sender, ItemsRemovedEventArgs args)
        {
            bool selectionChanged = false;
            foreach (IListItem item in args.Items)
                if (item.Selected)
                    selectionChanged = true;

            BeginUpdate();
            _modifyingItem = true;
            try
            {
                if (this.IsHandleCreated)
                {
                    foreach (int itemIndex in args.RemovedItemsIndices)
                        NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_DELETEITEM, itemIndex, 0);
                }
            }
            finally
            {
                _modifyingItem = false;
                EndUpdate();
            }

            ArrangeIcons(ListViewAlignment.Default);

            if (selectionChanged)
                OnSelectedItemsChanged(System.EventArgs.Empty);
        }

        /// <summary>
        /// Internal handler of the ItemsRemovedAll event of the items collection.
        /// </summary>
        private void ItemsRemovedAll(Object sender, EventArgs args)
        {
            if (this.IsHandleCreated)
                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_DELETEALLITEMS, 0, 0);

            _queueManager.Clear();

            _thumbImageList.Clear();
            _largeImageList.Clear();
            _smallImageList.Clear();
        }

        /// <summary>
        /// Internal handler of the TextChanged event of the items collection.
        /// Method reflects item changes in the virtual list view control.
        /// </summary>
        private new void TextChanged(Object sender, TextChangedEventArgs args)
        {
            int itemIndex = this.ItemsInternal.IndexOf(args.Item);
            ItemTextChangedInternal(itemIndex, args.TextInfoId, args.Item.GetText(args.TextInfoId));
        }

        /// <summary>
        /// Internal handler of the IconChanged event of the items collection.
        /// Method reflects item changes in the virtual list view control.
        /// </summary>
        private void IconChanged(Object sender, IconChangedEventArgs args)
        {
            if (args.View != _view)
                return;

            int itemIndex = this.ItemsInternal.IndexOf(args.Item);
            ItemIconChangedInternal(itemIndex, GetImageList(_view).IndexOfKey(args.Item.GetIconKey(_view)));
        }

        /// <summary>
        /// Internal handler of the StateChanged event of the items collection.
        /// Method reflects item changes in the virtual list view control.
        /// </summary>
        private void StateChanged(Object sender, StateChangedEventArgs args)
        {
            int itemIndex = this.ItemsInternal.IndexOf(args.Item);
            uint state = 0;
            uint mask = 0;
            switch (args.StateType)
            {
                case StateType.Check:
                    state = args.Item.CheckEnabled ? (args.Item.Checked ? IndexToStateImageMask(2) : IndexToStateImageMask(1)) : 0;
                    mask = NativeMethods.LVIS_STATEIMAGEMASK;
                    break;

                case StateType.Focus:
                    state = args.Item.Focused ? NativeMethods.LVIS_FOCUSED : 0;
                    mask = NativeMethods.LVIS_FOCUSED;
                    break;

                case StateType.Selection:
                    state = args.Item.Selected ? NativeMethods.LVIS_SELECTED : 0;
                    mask = NativeMethods.LVIS_SELECTED;
                    OnSelectedItemsChanged(System.EventArgs.Empty);
                    break;
            }
            SetItemState(itemIndex, state, mask);
        }

        private delegate void SetItemTextInternalDelegate(int itemIndex, int textInfoId, string text);

        private delegate void SetItemIconInternalDelegate(int itemIndex, int iconIndex);

        private void SetItemTextInternal(int itemIndex, int textInfoId, string text)
        {
            if (_modifyingItem)
                return;

            _modifyingItem = true;
            SetItemText(itemIndex, textInfoId, text);
            _modifyingItem = false;

            if (_view == View.List)
                UpdateListModeColumnWidth(itemIndex, text);
        }

        /// <summary>
        /// Updates column width of the control when View == View.List.
        /// Checks that column should be wider than specified item.
        /// </summary>
        /// <param name="itemIndex">Item index.</param>
        private void UpdateListModeColumnWidth(int itemIndex, string itemText)
        {
            if (_view != View.List)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ThumbListShouldBeInListMode"));
            if (!this.IsHandleCreated)
                return;

            System.Drawing.Rectangle iconSize = GetItemRect(itemIndex, NativeMethods.LVIR_ICON);
            int textWidth = NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETSTRINGWIDTH, 0, itemText);
            int wholeItemWidth = iconSize.Width + textWidth;
            int columnWidth = GetColumnWidth(0);

            if (wholeItemWidth > columnWidth)
                SetColumnWidth(0, wholeItemWidth + 40);
        }

        private void UpdateListModeColumnWidth()
        {
            if (_view != View.List)
                throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ThumbListShouldBeInListMode"));
            if (!this.IsHandleCreated)
                return;

            if (this.ItemsInternal.Count > 0)
            {
                NativeMethods.LVITEMW itemStruct = new Aurigma.NativeMethods.LVITEMW();
                IntPtr strBuffer = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(1026);
                int maxItemWidth = -1;

                try
                {
                    for (int i = 0; i < this.ItemsInternal.Count; i++)
                    {
                        itemStruct.pszText = strBuffer;
                        itemStruct.cchTextMax = 512;

                        itemStruct.iItem = i;
                        itemStruct.mask = NativeMethods.LVIF_TEXT;
                        itemStruct.cchTextMax = 1024;

                        if (GetAt(ref itemStruct) && itemStruct.pszText != LPSTR_TEXTCALLBACKW)
                        {
                            System.Drawing.Rectangle iconSize = GetItemRect(i, NativeMethods.LVIR_ICON);
                            int textWidth = NativeMethods.SendMessage(this.Handle, (int)NativeMethods.LVM_GETSTRINGWIDTH, 0, itemStruct.pszText);
                            int wholeItemWidth = iconSize.Width + textWidth;

                            if (wholeItemWidth > maxItemWidth)
                                maxItemWidth = wholeItemWidth;
                        }
                    }
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(strBuffer);
                }

                if (maxItemWidth != -1)
                    SetColumnWidth(0, maxItemWidth + 40);
            }
            else
            {
                SetColumnWidth(0, 150);
            }
        }

        private void SetItemIconInternal(int itemIndex, int iconIndex)
        {
            if (_modifyingItem)
                return;

            _modifyingItem = true;
            try
            {
                SetItemImage(itemIndex, iconIndex);

                if (this.IsHandleCreated)
                    NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_REDRAWITEMS, itemIndex, itemIndex);
            }
            finally
            {
                _modifyingItem = false;
            }
        }

        private void ItemTextChangedInternal(int itemIndex, int textInfoId, string text)
        {
            if (_disposed)
                return;

            if (!InvokeRequired)
            {
                SetItemTextInternal(itemIndex, textInfoId, text);
            }
            else
            {
                SetItemTextInternalDelegate setItemTextInternal = new SetItemTextInternalDelegate(SetItemTextInternal);

                try
                {
                    this.BeginInvoke(setItemTextInternal, new object[] { itemIndex, textInfoId, text });
                }
                catch (System.InvalidOperationException)
                {
                    // Occurs when the form is disposed
                }
            }
        }

        private void ItemIconChangedInternal(int itemIndex, int iconIndex)
        {
            if (_disposed)
                return;

            if (!InvokeRequired)
                SetItemIconInternal(itemIndex, iconIndex);
            else
            {
                SetItemIconInternalDelegate setItemIconInternal = new SetItemIconInternalDelegate(SetItemIconInternal);

                try
                {
                    this.BeginInvoke(setItemIconInternal, new object[] { itemIndex, iconIndex });
                }
                catch (System.InvalidOperationException)
                {
                    // Occurs when the form is disposed
                }
            }
        }

        #endregion Internal Events

        #region Public properties

        /// <summary>
        /// Type of action the user must take to activate an item.
        /// </summary>
        [ResDescription("VirtualListView_Activation")]
        [CategoryAttribute("Behavior")]
        [DefaultValue(ItemActivation.Standard)]
        public ItemActivation Activation
        {
            get
            {
                return this._activation;
            }
            set
            {
                if (this._activation != value)
                {
                    this._activation = value;
                    this.UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        /// A value indicating whether the user can drag column headers to reorder columns in the control.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_AllowColumnReorder")]
        [DefaultValue(false)]
        public bool AllowColumnReorder
        {
            get
            {
                return this._allowColumnReorder;
            }
            set
            {
                if (this._allowColumnReorder != value)
                {
                    this._allowColumnReorder = value;
                    this.UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        /// Indicates whether icons are automatically kept arranged.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_AutoArrange")]
        [DefaultValue(true)]
        public bool AutoArrange
        {
            get
            {
                return this._autoArrange;
            }
            set
            {
                if (value != this._autoArrange)
                {
                    this._autoArrange = value;
                    this.UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        [ResDescription("VirtualListView_BackColor")]
        public override System.Drawing.Color BackColor
        {
            get
            {
                if (_backColorSet)
                {
                    return base.BackColor;
                }
                else
                {
                    return SystemColors.Window;
                }
            }
            set
            {
                if (!_backColorSet || base.BackColor != value)
                {
                    _backColorSet = true;
                    base.BackColor = value;
                    if (base.IsHandleCreated)
                    {
                        NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETBKCOLOR, 0, ColorTranslator.ToWin32(this.BackColor));
                        NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETTEXTBKCOLOR, 0, ColorTranslator.ToWin32(this.BackColor));
                    }
                }
            }
        }

        /// <summary>
        /// The border style of the control.
        /// </summary>
        [CategoryAttribute("Appearance")]
        [ResDescription("VirtualListView_BorderStyle")]
        [DefaultValue(BorderStyle.Fixed3D)]
        public BorderStyle BorderStyle
        {
            get
            {
                return _borderStyle;
            }
            set
            {
                if (_borderStyle != value)
                {
                    _borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Indicates whether a check boxes are displayed beside items.
        /// </summary>
        [CategoryAttribute("Appearance")]
        [ResDescription("VirtualListView_CheckBoxes")]
        [DefaultValue(false)]
        public bool CheckBoxes
        {
            get
            {
                return _checkBoxes;
            }
            set
            {
                if (_checkBoxes != value)
                {
                    _checkBoxes = value;
                    if (IsHandleCreated)
                    {
                        UpdateExtendedStyles();
                        if (this.AutoArrange)
                            this.ArrangeIcons(ListViewAlignment.Default);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color for the control.
        /// </summary>
        [ResDescription("VirtualListView_ForeColor")]
        public override System.Drawing.Color ForeColor
        {
            get
            {
                if (_foreColorSet)
                {
                    return base.ForeColor;
                }
                else
                {
                    return SystemColors.WindowText;
                }
            }
            set
            {
                if (!_foreColorSet || base.ForeColor != value)
                {
                    _foreColorSet = true;
                    base.ForeColor = value;
                    if (base.IsHandleCreated)
                        NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETTEXTCOLOR, 0, ColorTranslator.ToWin32(this.ForeColor));
                }
            }
        }

        /// <summary>
        /// Indicates whether clicking an item selects all its subitems.
        /// </summary>
        [CategoryAttribute("Appearance")]
        [ResDescription("VirtualListView_FullRowSelect")]
        [DefaultValue(false)]
        public bool FullRowSelect
        {
            get
            {
                return _fullRowSelect;
            }
            set
            {
                if (_fullRowSelect != value)
                {
                    _fullRowSelect = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        /// Indicates whether grid lines appear between the rows and columns containing the items and subitems in the control.
        /// </summary>
        [CategoryAttribute("Appearance")]
        [ResDescription("VirtualListView_GridLines")]
        [DefaultValue(false)]
        public bool Gridlines
        {
            get
            {
                return _gridlines;
            }
            set
            {
                if (_gridlines != value)
                {
                    _gridlines = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        /// Column header style.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_HeaderStyle")]
        [DefaultValue(ColumnHeaderStyle.Clickable)]
        public ColumnHeaderStyle HeaderStyle
        {
            get
            {
                return this._headerStyle;
            }
            set
            {
                if (this._headerStyle != value)
                {
                    this._headerStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Indicates whether the selected item in the control remains highlighted when the control loses focus.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_HideSelection")]
        [DefaultValue(false)]
        public bool HideSelection
        {
            get
            {
                return _hideSelection;
            }
            set
            {
                if (_hideSelection != value)
                {
                    _hideSelection = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Indicates whether an item is automatically selected when the mouse pointer remains over the item for a few seconds.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_HoverSelection")]
        [DefaultValue(false)]
        public bool HoverSelection
        {
            get
            {
                return _hoverSelection;
            }
            set
            {
                if (_hoverSelection != value)
                {
                    _hoverSelection = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        /// Indicates whether item labels wrap when items are displayed in the control as icons.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_LabelWrap")]
        [DefaultValue(true)]
        public bool LabelWrap
        {
            get
            {
                return _labelWrap;
            }
            set
            {
                if (_labelWrap != value)
                {
                    _labelWrap = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Allows item labels to be edited inplace by the users.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_LabelEdit")]
        [DefaultValue(false)]
        public bool LabelEdit
        {
            get
            {
                return _labelEdit;
            }
            set
            {
                _labelEdit = value;
                UpdateStyles();
            }
        }

        /// <summary>
        /// Indicates whether multiple items can be selected.
        /// </summary>
        [CategoryAttribute("Behavior")]
        [ResDescription("VirtualListView_MultiSelect")]
        [DefaultValue(true)]
        public bool MultiSelect
        {
            get
            {
                return _multiSelect;
            }
            set
            {
                if (_multiSelect != value)
                {
                    _multiSelect = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Indicates whether to use border rectangle to show selected items in Thumbnails mode.
        /// </summary>
        [CategoryAttribute("Appearance")]
        [ResDescription("VirtualListView_BorderSelection")]
        [DefaultValue(true)]
        public bool BorderSelection
        {
            get
            {
                return _borderSelection;
            }
            set
            {
                if (_borderSelection != value)
                {
                    _borderSelection = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        /// Specifies how items are displayed in the control.
        /// </summary>
        [CategoryAttribute("Appearance")]
        [ResDescription("VirtualListView_View")]
        [DefaultValue(View.Thumbnails)]
        public View View
        {
            get
            {
                return _view;
            }
            set
            {
                if (_view != value)
                {
                    _view = value;
                    if (this.IsHandleCreated)
                    {
                        SetImageList(_view, GetImageList(_view).Handle);
                        ModifyStyle(NativeMethods.LVS_REPORT | NativeMethods.LVS_ICON | NativeMethods.LVS_LIST, ViewToStyle(_view), 0);

                        if (_borderSelection && _view == View.Thumbnails)
                            SetExtendedListViewStyle(NativeMethods.LVS_EX_BORDERSELECT, NativeMethods.LVS_EX_BORDERSELECT);
                        else
                            SetExtendedListViewStyle(0, NativeMethods.LVS_EX_BORDERSELECT);

                        if (_view == View.List)
                            UpdateListModeColumnWidth();
                    }

                    OnViewChanged(_view);
                }
            }
        }

        /// <summary>
        /// Specifies type of text for display in iconic views (List, Icons, Thumbnails).
        /// </summary>
        [CategoryAttribute("Appearance")]
        [ResDescription("VirtualListView_IconicViewTextInfoId")]
        [DefaultValue(ThumbnailListItem.TextInfoIdDisplayName)]
        public int IconicViewTextInfoId
        {
            get
            {
                return _iconicViewTextInfoId;
            }
            set
            {
                _iconicViewTextInfoId = value;
            }
        }

        /// <summary>
        /// Gets item that has focus.
        /// </summary>
        [BrowsableAttribute(false)]
        [ResDescription("VirtualListView_FocusedItem")]
        public IListItem FocusedItem
        {
            get
            {
                if (this.IsHandleCreated)
                {
                    int index = (int)NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETNEXTITEM, -1, NativeMethods.LVNI_FOCUSED);
                    if (index != -1)
                        return ItemsInternal[index];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets queue manager of the control.
        /// </summary>
        [ResDescription("ListItemCollection_QueueManager")]
        [Browsable(false)]
        public QueueManager QueueManager
        {
            get
            {
                return _queueManager;
            }
        }

        /// <summary>
        /// Gets spacing between icons in current View. The view mode of the control should be View.Icons or View.Thumbnails.
        /// You can set System.Drawing.Point.Empty value to reset spacing to default values.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Point IconSpacing
        {
            get
            {
                if (_view != View.Icons && _view != View.Thumbnails)
                    throw new ArgumentException(StringResources.GetString("ViewShouldBeIconic"));

                System.Drawing.Point result = new System.Drawing.Point();
                if (this.IsHandleCreated)
                {
                    int spacings = NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETITEMSPACING, 0, 0);
                    result.X = (int)((uint)spacings & 0xffff);
                    result.Y = (int)(((uint)spacings & 0xffff0000) >> 16);
                }

                return result;
            }
            set
            {
                if (_view != View.Icons && _view != View.Thumbnails)
                    throw new ArgumentException(StringResources.GetString("ViewShouldBeIconic"));
                if ((value.X < 4 || value.Y < 4) && !value.IsEmpty)
                    throw new ArgumentOutOfRangeException("spacings", StringResources.GetString("IconSpacingsTooSmall"));
                if (value.X >= 0xffff || value.Y >= 0xffff)
                    throw new ArgumentOutOfRangeException("spacings", StringResources.GetString("IconSpacingsTooBig"));

                if (!this.IsHandleCreated)
                    return;

                int lParam = 0;
                if (value.IsEmpty)
                    lParam = -1;
                else
                    lParam = ((value.Y & 0xffff) << 16) | (value.X & 0xffff);

                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETICONSPACING, 0, lParam);
                this.ArrangeIcons(System.Windows.Forms.ListViewAlignment.Default);
                this.Invalidate();
            }
        }

        #endregion Public properties

        #region "Private properties"

        /// <summary>
        /// Sets or gets control's window extended style.
        /// </summary>
        private int WindowExStyle
        {
            get
            {
                if (this.IsHandleCreated)
                    return (int)NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_EXSTYLE);
                else
                    return 0;
            }
            set
            {
                if (this.IsHandleCreated)
                    NativeMethods.SetWindowLong(this.Handle, (int)NativeMethods.GWL_EXSTYLE, value);
            }
        }

        /// <summary>
        /// Sets or gets control's window style.
        /// </summary>
        private int WindowStyle
        {
            get
            {
                if (this.IsHandleCreated)
                    return (int)NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_STYLE);
                else
                    return 0;
            }
            set
            {
                if (this.IsHandleCreated)
                    NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL_STYLE, value);
            }
        }

        #endregion "Private properties"

        #region "Sort implementation"

        /// <summary>
        /// Sorts items of the ThumbnailListView control ascending according to specified TextInfoId field.
        /// </summary>
        /// <param name="textInfoId">Member of the TextInfoId enumeration which specifies sort field.</param>
        /// <param name="sortAscending">Sort order flag.</param>
        public void Sort(int textInfoId, bool sortAscending)
        {
            if (this.ItemsInternal.Count == 0)
                return;

            _itemsComparer = new TextInfoIdComparer(textInfoId, sortAscending);
            Sort(_itemsComparer);
        }

        /// <summary>
        /// Sorts items of the ThumbnailListView control using custom IComparer object.
        /// </summary>
        /// <param name="comparer">Custom IComparer implementation for use during sort.</param>
        public void Sort(IComparer comparer)
        {
            if (this.IsHandleCreated)
            {
                _itemsComparer = comparer;
                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SORTITEMSEX, 0, new NativeMethods.ListViewSortHandler(ListViewSortCallback));

                // Sorting internal items
                this.ItemsInternal.SortInternals(comparer);
                InvalidateAllItems();
            }
        }

        private int ListViewSortCallback(int index0, int index1, int dummy)
        {
            IListItem item0 = ItemsInternal[index0],
                      item1 = ItemsInternal[index1];

            return _itemsComparer.Compare(item0, item1);
        }

        #endregion "Sort implementation"

        #region "Drag&Drop related functions"

        /// <summary>
        /// Retrieves the position of the insertion mark.
        /// </summary>
        /// <param name="index">Index of the item next to which the insertion point appears.</param>
        /// <param name="afterItem">The insertion point appears after the item specified if the value is true; otherwise it appears before the specified item.</param>
        /// <returns>returns true if successful, or false otherwise.</returns>
        [ResDescription("VirtualListView_GetInsertMark")]
        public bool GetInsertMark(out int index, out bool afterItem)
        {
            index = -1;
            afterItem = false;
            bool result = false;

            if (this.IsHandleCreated)
            {
                NativeMethods.LVINSERTMARK insertMark = new NativeMethods.LVINSERTMARK();
                insertMark.cbSize = (uint)Marshal.SizeOf(insertMark);

                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETINSERTMARK, 0, ref insertMark);
                if (insertMark.iItem >= 0)
                {
                    index = insertMark.iItem;
                    afterItem = ((insertMark.dwFlags & NativeMethods.LVIM_AFTER) != 0);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the insertion point to the defined position. An insertion point can only appear if the list-view control
        /// is in icon view, small icon view, or list view. To use this API, you must provide a manifest specifying Comclt32.dll version 6.0.
        /// </summary>
        /// <param name="index">Index of the item next to which the insertion point appears.</param>
        /// <param name="afterItem">The insertion point appears after the item specified if the value is true; otherwise it appears before the specified item.</param>
        [ResDescription("VirtualListView_SetInsertMark")]
        public void SetInsertMark(int index, bool afterItem)
        {
            if (index >= ItemsInternal.Count)
                throw new ArgumentOutOfRangeException("index");

            if (this.IsHandleCreated)
            {
                NativeMethods.LVINSERTMARK insertMark = new NativeMethods.LVINSERTMARK();
                insertMark.cbSize = (uint)Marshal.SizeOf(insertMark);
                insertMark.iItem = index;
                insertMark.dwFlags = afterItem ? NativeMethods.LVIM_AFTER : 0;
                insertMark.dwReserved = 0;

                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETINSERTMARK, 0, ref insertMark);
            }
        }

        /// <summary>
        /// Determines which list-view item, if any, is at a specified position.
        /// </summary>
        /// <param name="point">The position to hit test.</param>
        /// <returns>Index of the item at specified position, if any, or -1 otherwise.</returns>
        [ResDescription("VirtualListView_HitTest")]
        public int HitTest(System.Drawing.Point point)
        {
            return HitTest(point.X, point.Y);
        }

        /// <summary>
        /// Determines which list-view item, if any, is at a specified position.
        /// </summary>
        /// <param name="x">X-coordinate of the position to hit test.</param>
        /// <param name="y">Y-coordinate of the position to hit test.</param>
        /// <returns>Index of the item at specified position, if any, or -1 otherwise.</returns>
        [ResDescription("VirtualListView_HitTest")]
        public int HitTest(int x, int y)
        {
            if (this.IsHandleCreated)
            {
                NativeMethods.LVHITTESTINFO hitTestInfo = new NativeMethods.LVHITTESTINFO();
                hitTestInfo.pt.x = x;
                hitTestInfo.pt.y = y;
                return NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_HITTEST, 0, ref hitTestInfo);
            }
            else
                return -1;
        }

        /// <summary>
        /// Retrieves the insertion point closest to a specified point.
        /// </summary>
        /// <param name="point">The position to hit test.</param>
        /// <param name="index">Variable that receives index of item next to insertion point.</param>
        /// <param name="afterItem">The insertion point appears after the item specified if the value is true; otherwise it appears before the specified item.</param>
        /// <returns>returns true if successful, or false otherwise.</returns>
        [ResDescription("VirtualListView_InsertMarkHitTest")]
        public bool InsertMarkHitTest(System.Drawing.Point point, out int index, out bool afterItem)
        {
            return InsertMarkHitTest(point.X, point.Y, out index, out afterItem);
        }

        /// <summary>
        /// Retrieves the insertion point closest to a specified point.
        /// </summary>
        /// <param name="x">X-coordinate of the position to hit test.</param>
        /// <param name="y">Y-coordinate of the position to hit test.</param>
        /// <param name="index">Variable that receives index of item next to insertion point.</param>
        /// <param name="afterItem">The insertion point appears after the item specified if the value is true; otherwise it appears before the specified item.</param>
        /// <returns>returns true if successful, or false otherwise.</returns>
        [ResDescription("VirtualListView_InsertMarkHitTest")]
        public bool InsertMarkHitTest(int x, int y, out int index, out bool afterItem)
        {
            afterItem = false;

            if (_view == View.Details || _view == View.List)
                index = HitTest(x, y);
            else
                index = FindNearestItem(x, y);

            if (index == -1)
                return false;

            System.Drawing.Rectangle itemRect = GetItemRect(index);
            int xMid = itemRect.X + itemRect.Width / 2,
                yMid = itemRect.Y + itemRect.Height / 2;

            if (_view == View.Details && y >= yMid)
                afterItem = true;
            else if (x >= xMid)
                afterItem = true;

            return true;
        }

        /// <summary>
        /// Finds the item nearest to the specified position. Search is supported only in icon and list modes.
        /// </summary>
        /// <param name="point">The position to hit test.</param>
        /// <returns>Returns the index of the item if successful, or -1 otherwise.</returns>
        [ResDescription("VirtualListView_FindNearestItem")]
        public int FindNearestItem(System.Drawing.Point point)
        {
            return FindNearestItem(point.X, point.Y);
        }

        /// <summary>
        /// Finds the item nearest to the specified position. Search is supported only in icon and list modes.
        /// </summary>
        /// <param name="x">X-coordinate of the position to hit test.</param>
        /// <param name="y">Y-coordinate of the position to hit test.</param>
        /// <returns>Returns the index of the item if successful, or -1 otherwise.</returns>
        [ResDescription("VirtualListView_FindNearestItem")]
        public int FindNearestItem(int x, int y)
        {
            if (this.IsHandleCreated)
            {
                NativeMethods.LVFINDINFO findItemInfo = new NativeMethods.LVFINDINFO();
                findItemInfo.flags = NativeMethods.LVFI_NEARESTXY;
                findItemInfo.pt.x = x;
                findItemInfo.pt.y = y;
                findItemInfo.vkDirection = NativeMethods.VK_NEXT;
                return NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_FINDITEM, -1, ref findItemInfo);
            }
            else
            {
                return -1;
            }
        }

        #endregion "Drag&Drop related functions"

        #region "BackgroundImage implementation"

        /// <summary>
        /// Raises the BackgroundImageChanged event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            if (this.IsHandleCreated)
                SetBackgroundImage();

            base.OnBackgroundImageChanged(e);
        }

        private void SetBackgroundImage()
        {
            string prevBackgroundImageFilename = _backgroundImageFilename;
            if (this.BackgroundImage != null)
            {
                _backgroundImageFilename = GetTempFilename();
                this.BackgroundImage.Save(_backgroundImageFilename, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            else
            {
                _backgroundImageFilename = null;
            }

            if (this.IsHandleCreated)
            {
                Application.OleRequired();
                NativeMethods.LVBKIMAGE lvBkImage = CreateInitializedLvBkImageStruct();
                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETBKIMAGE, 0, ref lvBkImage);
            }
            DeleteTempFile(prevBackgroundImageFilename);
        }

        private void UpdateBackgroundImage()
        {
            if (this.IsHandleCreated && _backgroundImageFilename != null)
            {
                Application.OleRequired();
                NativeMethods.LVBKIMAGE lvBgImage = CreateInitializedLvBkImageStruct();
                NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETBKIMAGE, 0, ref lvBgImage);
            }
        }

        private NativeMethods.LVBKIMAGE CreateInitializedLvBkImageStruct()
        {
            NativeMethods.LVBKIMAGE result = new NativeMethods.LVBKIMAGE();

            if (_backgroundImageFilename != null && _backgroundImageFilename.Length > 0)
            {
                result.xOffsetPercent = _backgroundImageOffset.X;
                result.yOffsetPercent = _backgroundImageOffset.Y;
                result.pszImage = _backgroundImageFilename;
                result.cchImageMax = (uint)_backgroundImageFilename.Length + 1;
                result.ulFlags = NativeMethods.LVBKIF_SOURCE_URL;
                if (_backgroundImageTiled)
                {
                    result.ulFlags |= NativeMethods.LVBKIF_STYLE_TILE;
                    result.ulFlags |= NativeMethods.LVBKIF_FLAG_TILEOFFSET;
                }
            }
            else
            {
                result.ulFlags = 0;
            }

            return result;
        }

        /// <summary>
        /// The value determining whether to fill whole background of the control with copies of the specified background image or not.
        /// </summary>
        [ResDescription("VirtualListView_BackgroundImageTiled")]
        public bool BackgroundImageTiled
        {
            get
            {
                return _backgroundImageTiled;
            }
            set
            {
                _backgroundImageTiled = value;
                UpdateBackgroundImage();
            }
        }

        /// <summary>
        /// Offset of the background image (in percents) or initial tile offset of the background (in pixels) - depends on BackgroundImageTiled property value.
        /// </summary>
        [ResDescription("VirtualListView_BackgroundImageOffset")]
        public System.Drawing.Point BackgroundImageOffset
        {
            get
            {
                return _backgroundImageOffset;
            }
            set
            {
                _backgroundImageOffset = value;
                UpdateBackgroundImage();
            }
        }

        private static string GetTempFilename()
        {
            string result;
            do
            {
                result = string.Format("{0}/{1}{2}.{3}", System.IO.Path.GetTempPath(), "ThumbListBkImage_", System.Guid.NewGuid().ToString(), "tmp");
            }
            while (System.IO.File.Exists(result));

            return result;
        }

        private static void DeleteTempFile(string tempFilename)
        {
            if (tempFilename == null || tempFilename.Length == 0)
                return;

            try
            {
                System.IO.File.Delete(tempFilename);
            }
            catch (System.IO.IOException)
            {
            }
        }

        #endregion "BackgroundImage implementation"

        #region "Workspaces functionality"

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Rectangle[] WorkAreas
        {
            get
            {
                System.Drawing.Rectangle[] result = null;
                int workAreaCount = 0;

                if (this.IsHandleCreated)
                {
                    NativeMethods.SendMessage(this.Handle, (int)NativeMethods.LVM_GETNUMBEROFWORKAREAS, 0, ref workAreaCount);
                    System.Diagnostics.Debug.Assert(workAreaCount <= (int)NativeMethods.LV_MAX_WORKAREAS, "WorkArea count cannot be greater than LV_MAX_WORKAREAS.");

                    if (workAreaCount != 0)
                    {
                        NativeMethods.RECT[] workAreas = new Aurigma.NativeMethods.RECT[workAreaCount];
                        NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_GETWORKAREAS, workAreaCount, workAreas);

                        result = new System.Drawing.Rectangle[workAreaCount];
                        for (int i = 0; i < workAreaCount; i++)
                            result[i] = System.Drawing.Rectangle.FromLTRB(workAreas[i].left, workAreas[i].top, workAreas[i].right, workAreas[i].bottom);
                    }
                }

                return result;
            }
            set
            {
                int workAreaCount = (value == null ? 0 : value.Length);
                if (workAreaCount == 0)
                {
                    NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETWORKAREAS, workAreaCount, (NativeMethods.RECT[])null);
                    this.ArrangeIcons(System.Windows.Forms.ListViewAlignment.SnapToGrid);
                }
                else if (this.IsHandleCreated)
                {
                    NativeMethods.RECT[] workAreas = new NativeMethods.RECT[workAreaCount];
                    for (int i = 0; i < workAreaCount; i++)
                    {
                        workAreas[i] = new NativeMethods.RECT();
                        workAreas[i].left = value[i].Left;
                        workAreas[i].top = value[i].Top;
                        workAreas[i].right = (int)Math.Min((Int64)value[i].Left + (Int64)value[i].Width, Int32.MaxValue);
                        workAreas[i].bottom = (int)Math.Min((Int64)value[i].Top + (Int64)value[i].Height, Int32.MaxValue);
                    }

                    NativeMethods.SendMessage(this.Handle, NativeMethods.LVM_SETWORKAREAS, workAreaCount, workAreas);
                }

                this.ArrangeIcons(System.Windows.Forms.ListViewAlignment.Default);
            }
        }

        public void MoveItemToWorkArea(int itemIndex, int workAreaIndex)
        {
            System.Drawing.Rectangle[] workAreas = this.WorkAreas;
            if (workAreaIndex < 0 || workAreas == null || workAreaIndex >= workAreas.Length)
                throw new System.ArgumentOutOfRangeException("workAreaIndex");

            SetItemPosition(itemIndex, new System.Drawing.Point(workAreas[workAreaIndex].Right - 1, workAreas[workAreaIndex].Bottom - 1));
            this.ArrangeIcons(System.Windows.Forms.ListViewAlignment.Default);
        }

        #endregion "Workspaces functionality"

        #region "ImageLists & items icons functionality"

        [ResDescription("ThumbnailListView_ListItemBackgroundColor")]
        public System.Drawing.Color ListItemBackgroundColor
        {
            set
            {
                _thumbImageList.ListItemBackgroundColor = value;
            }
            get
            {
                return _thumbImageList.ListItemBackgroundColor;
            }
        }

        /// <summary>
        /// Sets or gets item's background image for Thumbnails mode of the control.
        /// </summary>
        [ResDescription("ThumbnailListView_ListItemBackgroundImage")]
        [BrowsableAttribute(false)]
        public Aurigma.GraphicsMill.Bitmap ListItemBackgroundImage
        {
            set
            {
                _thumbImageList.ListItemBackgroundImage = value;
            }
            get
            {
                return _thumbImageList.ListItemBackgroundImage;
            }
        }

        /// <summary>
        /// Sets or gets item's foreground image for Thumbnails mode of the control.
        /// </summary>
        [ResDescription("ThumbnailListView_ListItemForegroundImage")]
        [BrowsableAttribute(false)]
        public Aurigma.GraphicsMill.Bitmap ListItemForegroundImage
        {
            set
            {
                _thumbImageList.ListItemForegroundImage = value;
            }
            get
            {
                return _thumbImageList.ListItemForegroundImage;
            }
        }

        /// <summary>
        /// Thumbnail's size in pixels in Thumbnails view.
        /// </summary>
        [CategoryAttribute("Appearance")]
        [ResDescription("ThumbnailListView_ThumbnailSize")]
        public Size ThumbnailSize
        {
            set
            {
                if (!_thumbImageList.ThumbnailSize.Equals(value))
                {
                    _thumbImageList.ThumbnailSize = value;
                    SetImageList(_view, GetImageList(_view).Handle);
                }
            }
            get
            {
                return _thumbImageList.ThumbnailSize;
            }
        }

        public IImageList GetImageList(View view)
        {
            switch (view)
            {
                case View.Details:
                case View.List:
                    return _smallImageList;

                case View.Icons:
                    return _largeImageList;

                case View.Thumbnails:
                    return _thumbImageList;

                default:
                    throw new ArgumentException(StringResources.GetString("UnsupportedViewMode"), "view");
            }
        }

        protected void SetImageList(View view, IntPtr imageList)
        {
            if (this.IsHandleCreated)
            {
                uint type = 0;
                switch (view)
                {
                    case View.Details:
                    case View.List:
                        type = NativeMethods.LVSIL_SMALL;
                        break;

                    case View.Icons:
                    case View.Thumbnails:
                        type = NativeMethods.LVSIL_NORMAL;
                        break;

                    default:
                        return;
                }

                NativeMethods.SendMessage(this.Handle, (int)NativeMethods.LVM_SETIMAGELIST, type, imageList);

                for (int i = 0; i < this.ItemsInternal.Count; i++)
                    SetItemIconInternal(i, GetImageList(_view).IndexOfKey(this.ItemsInternal[i].GetIconKey(_view)));
            }
        }

        private void OnImageListImageRemoved(object sender, ImageRemovedEventArgs e)
        {
            View viewToCheck = View.Details;

            if (sender == _smallImageList && (_view == View.List || _view == View.Details))
                viewToCheck = View.List;
            else if (sender == _largeImageList && _view == View.Icons)
                viewToCheck = View.Icons;
            else if (sender == _thumbImageList && _view == View.Thumbnails)
                viewToCheck = View.Thumbnails;

            if (viewToCheck != View.Details)
            {
                NativeMethods.LVITEMW lvItem = new Aurigma.NativeMethods.LVITEMW();
                lvItem.mask = NativeMethods.LVIF_IMAGE | NativeMethods.LVIF_NORECOMPUTE;

                for (int i = 0; i < this.ItemsInternal.Count; i++)
                {
                    lvItem.iItem = i;
                    if (GetAt(ref lvItem) && lvItem.iImage == e.ImageIndex)
                        SetItemIconInternal(i, -1);
                }
            }
        }

        #endregion "ImageLists & items icons functionality"

        #region "Member variables"

        private SystemImageList _largeImageList;
        private SystemImageList _smallImageList;
        private ThumbnailImageList _thumbImageList;

        private QueueManager _queueManager;

        private bool _backColorSet;
        private bool _foreColorSet;

        private View _view = View.Thumbnails;
        private int _iconicViewTextInfoId;

        private ItemActivation _activation = ItemActivation.Standard;
        private bool _allowColumnReorder;
        private bool _autoArrange = true;
        private bool _checkBoxes;
        private bool _fullRowSelect;
        private bool _gridlines;
        private bool _hoverSelection;
        private BorderStyle _borderStyle = BorderStyle.Fixed3D;
        private ColumnHeaderStyle _headerStyle = ColumnHeaderStyle.Clickable;
        private bool _hideSelection;
        private bool _labelWrap = true;
        private bool _multiSelect = true;
        private bool _borderSelection = true;
        private bool _labelEdit;

        private string _backgroundImageFilename;
        private bool _backgroundImageTiled;
        private System.Drawing.Point _backgroundImageOffset;

        private IComparer _itemsComparer;

        private int _currentChangedItemIndex = -1;
        private ArrayList _ptrToFree = new ArrayList(100);
        private bool _disposed;
        private bool _modifyingItem;

        // This way we are avoiding the strange behavior when WndProc is called from a worker thread
        private bool bOnHandleCreatedHasBeenCalled = false;

        #endregion "Member variables"
    }
}