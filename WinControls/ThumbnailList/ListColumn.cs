// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Contains all possible column change types.
    /// </summary>
    [Flags]
    public enum ColumnChangeType
    {
        /// <summary>
        /// Changed type of column's display info.
        /// </summary>
        TextInfoId = 1 << 0,

        /// <summary>
        /// Column caption was changed.
        /// </summary>
        Caption = 1 << 1,

        /// <summary>
        /// Column width was changed.
        /// </summary>
        Width = 1 << 2,

        /// <summary>
        /// Column text alignment was changed.
        /// </summary>
        TextAlignment = 1 << 3
    }

    /// <summary>
    /// Represents column of the ThumbnailListView.
    /// </summary>
    [ResDescription("ListColumn")]
    [TypeConverter(typeof(ListColumnTypeConverter))]
    public class ListColumn : ICloneable
    {
        #region Construction/Destruction

        /// <summary>
        /// Initializes new instance of the ListColumn class.
        /// </summary>
        [ResDescription("ListColumn_ListColumn")]
        public ListColumn()
            : this(ThumbnailListItem.TextInfoIdDisplayName, StringResources.GetString("ListColumnDefaultCaption"), 200, HorizontalAlignment.Left)
        {
        }

        /// <summary>
        /// Initializes new instance of the ListColumn class.
        /// </summary>
        /// <param name="textInfoIf">Type of displaying text info.</param>
        /// <param name="text">Caption of the column.</param>
        /// <param name="width">Width of the column in pixels.</param>
        /// <param name="TextAlignment">Alignment of the column.</param>
        [ResDescription("ListColumn_ListColumn")]
        public ListColumn(int textInfoId, string caption, int width, HorizontalAlignment textAlignment)
        {
            this._textInfoId = textInfoId;
            this._caption = caption;
            this._width = width;
            this._textAlignment = textAlignment;
        }

        #endregion Construction/Destruction

        #region Properties

        /// <summary>
        /// Sets or gets type of text information displaying by the column.
        /// </summary>
        [ResDescription("ListColumn_TextInfoId")]
        public int TextInfoId
        {
            get
            {
                return _textInfoId;
            }
            set
            {
                if (_textInfoId != value)
                {
                    this._textInfoId = value;
                    OnColumnChanged(ColumnChangeType.TextInfoId);
                }
            }
        }

        /// <summary>
        /// Sets or gets caption of the column.
        /// </summary>
        [ResDescription("ListColumn_Caption")]
        public string Caption
        {
            get
            {
                return this._caption;
            }
            set
            {
                if (this._caption != value)
                {
                    this._caption = value;
                    OnColumnChanged(ColumnChangeType.Caption);
                }
            }
        }

        /// <summary>
        /// Sets or gets column width.
        /// </summary>
        [ResDescription("ListColumn_Width")]
        public int Width
        {
            get
            {
                return this._width;
            }
            set
            {
                if (this._width != value)
                {
                    this._width = value;
                    OnColumnChanged(ColumnChangeType.Width);
                }
            }
        }

        /// <summary>
        /// Sets or gets column alignment.
        /// </summary>
        [ResDescription("ListColumn_TextAlignment")]
        public HorizontalAlignment TextAlignment
        {
            get
            {
                return this._textAlignment;
            }
            set
            {
                if (this._textAlignment != value)
                {
                    this._textAlignment = value;
                    OnColumnChanged(ColumnChangeType.TextAlignment);
                }
            }
        }

        /// <summary>
        /// Stores reference to the parent control of the
        /// </summary>
        [BrowsableAttribute(false)]
        public ThumbnailListView Parent
        {
            get
            {
                return _parentControl;
            }
        }

        internal ThumbnailListView ParentInternal
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

        #endregion Properties

        #region Events

        /// <summary>
        /// Occurs when the column have been changed.
        /// </summary>
        public event ColumnChangedEventHandler ColumnChanged;

        internal void OnColumnChangedInternal(ColumnChangeType changeType)
        {
            OnColumnChanged(changeType);
        }

        /// <summary>
        /// Raises ColumnChanged event of the column.
        /// </summary>
        /// <param name="changeType"></param>
        protected virtual void OnColumnChanged(ColumnChangeType changeType)
        {
            if (ColumnChanged != null)
            {
                ColumnChangedEventArgs args = new ColumnChangedEventArgs(this, changeType);
                ColumnChanged(this, args);
            }
        }

        #endregion Events

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        [ResDescription("ListColumn_Clone")]
        public object Clone()
        {
            return new ListColumn(_textInfoId, _caption, _width, _textAlignment);
        }

        #region Private members

        /// <summary>
        /// Stores column's caption.
        /// </summary>
        private string _caption;

        /// <summary>
        /// Stores column's width.
        /// </summary>
        private int _width;

        /// <summary>
        /// Stores column's alignment.
        /// </summary>
        private HorizontalAlignment _textAlignment;

        /// <summary>
        /// Stores type of displayed text info.
        /// </summary>
        private int _textInfoId;

        /// <summary>
        /// Stores reference to the parent control of the column.
        /// </summary>
        private ThumbnailListView _parentControl;

        #endregion Private members
    }
}