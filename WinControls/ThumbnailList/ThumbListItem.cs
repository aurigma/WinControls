// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Represents list item of the ThumbnailListView control.
    /// </summary>
    [ResDescription("ThumbnailListItem")]
    public class ThumbnailListItem : ListItem, IQueueItem, System.ICloneable
    {
        #region "Constants"

        public const int TextInfoIdPath = -1;
        public const int TextInfoIdDisplayName = 0;
        public const int TextInfoIdFileSize = 1;
        public const int TextInfoIdFileType = 2;
        public const int TextInfoIdCreationDate = 3;

        #endregion "Constants"

        #region "Static members"

        /// <summary>
        /// Creates array of ThumbListItems, whose each element is based on the corresponding element of the pidls array.
        /// </summary>
        /// <param name="pidls">Array of source Pidl objects.</param>
        /// <returns>Result array of ThumbListItems.</returns>
        [ResDescription("ThumbnailListItem_Create")]
        public static ThumbnailListItem[] Create(Pidl[] pidls)
        {
            if (pidls == null)
            {
                return new ThumbnailListItem[0];
            }
            else
            {
                ThumbnailListItem[] resultArray = new ThumbnailListItem[pidls.Length];
                for (int i = 0; i < pidls.Length; i++)
                {
                    resultArray[i] = new ThumbnailListItem(pidls[i]);
                }
                return resultArray;
            }
        }

        #endregion "Static members"

        #region Construction/Destruction

        /// <summary>
        /// Initializes new instance of the ThumbnailListItem class. The instance represents object pointed by the Pidl.
        /// </summary>
        /// <param name="pidl">The Pidl object.</param>
        [ResDescription("ThumbnailListItem_ThumbnailListItem0")]
        public ThumbnailListItem(Pidl pidl)
        {
            if (pidl == null)
                throw new System.ArgumentNullException("pidl");

            InitializeIQueueItemVariables(_methodCountInternal);
            _pidl = Pidl.Create(pidl);
        }

        /// <summary>
        /// Initializes new instance of the ThumbnailListItem class. The instance represents file system object pointed by the path string.
        /// </summary>
        /// <param name="path">Path to the file system object.</param>
        [ResDescription("ThumbnailListItem_ThumbnailListItem1")]
        public ThumbnailListItem(string path)
        {
            if (path == null || path.Length == 0)
                throw new System.ArgumentNullException("path");

            InitializeIQueueItemVariables(_methodCountInternal);
            _pidl = Pidl.Create(path);
        }

        /// <summary>
        /// Initializes new instance of the ThumbnailListItem class. The instance represents one of the system standard folders.
        /// </summary>
        /// <param name="standardFolder">Enum value that specifies standard folder.</param>
        [ResDescription("ThumbnailListItem_ThumbnailListItem2")]
        public ThumbnailListItem(Aurigma.GraphicsMill.WinControls.StandardFolder standardFolder)
        {
            InitializeIQueueItemVariables(_methodCountInternal);
            _pidl = Pidl.Create(standardFolder);
        }

        /// <summary>
        /// Initializes new instance of the ThumbnailListItem as a copy of the specified item.
        /// </summary>
        /// <param name="item">Source item.</param>
        protected ThumbnailListItem(ThumbnailListItem item)
            : base(item)
        {
            if (item == null)
                throw new System.ArgumentNullException("item");

            _pidl = Pidl.Create(item._pidl);
            _kilobytesText = (string)item._kilobytesText.Clone();
            _methodCount = item._methodCount;

            InitializeIQueueItemVariables(_methodCountInternal);
        }

        /// <summary>
        /// Initializes internal variables of the object that are related to the implementation of IQueueItem interface.
        /// </summary>
        /// <param name="methodCount">Number of methods that are associated with the item. </param>
        protected void InitializeIQueueItemVariables(int methodCount)
        {
            _methodCount = methodCount;
            _methodState = new QueueItemMethodState[methodCount];
            for (int i = 0; i < methodCount; i++)
            {
                _methodState[i] = QueueItemMethodState.NotStarted;
            }
        }

        /// <summary>
        /// Adds empty handlers for item events. Because of multi threading execution
        /// following check maybe not correct:
        ///
        /// if (this.IconChanged)
        ///	   this.IconChanged(sender, e);
        ///
        ///	because after first operation, but before the second external handler may be
        ///	removed and we'll got NullReferenceException.
        ///
        /// </summary>
        private void InitializeDummyHandlers()
        {
            this.IconChanged += new IconChangedEventHandler(DummyIconChangedHandler);
            this.TextChanged += new TextChangedEventHandler(DummyTextChangedHandler);
        }

        private void DummyIconChangedHandler(object sender, IconChangedEventArgs e)
        {
        }

        private void DummyTextChangedHandler(object sender, TextChangedEventArgs e)
        {
        }

        #endregion Construction/Destruction

        #region Public properties

        [ResDescription("ThumbnailListItem_Pidl")]
        public Pidl Pidl
        {
            get
            {
                return _pidl;
            }
        }

        /// <summary>
        /// Reference to parent ThumbnailListView control of the item.
        /// </summary>
        [ResDescription("IListItem_Parent")]
        public override ThumbnailListView Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                ChangeParent(value);
            }
        }

        #endregion Public properties

        #region "Queue processing"

        /// <summary>
        /// Returns item methods count. Each method is supposed to be executed in separate thread asynchronously.
        /// </summary>
        /// <returns>Returns item methods count.</returns>
        [ResDescription("IQueueItem_MethodCount")]
        public int MethodCount
        {
            get
            {
                return _methodCount;
            }
        }

        /// <summary>
        /// Returns execution state of the specified method.
        /// </summary>
        /// <param name="methodIndex">Index of the method.</param>
        [ResDescription("IQueueItem_GetMethodState")]
        public virtual QueueItemMethodState GetMethodState(int methodIndex)
        {
            QueueItemMethodState result;

            lock (this)
            {
                result = _methodState[methodIndex];
            }
            return result;
        }

        /// <summary>
        /// Sets execution state of the specified method.
        /// </summary>
        /// <param name="methodIndex">Index of the method.</param>
        /// <param name="value">Execution state of the method.</param>
        protected virtual void SetMethodState(int methodIndex, QueueItemMethodState state)
        {
            lock (this)
            {
                _methodState[methodIndex] = state;
            }
        }

        /// <summary>
        /// Starts execution of the specified method.
        /// </summary>
        /// <param name="methodIndex">Method index.</param>
        [ResDescription("IQueueItem_EvaluateMethod")]
        public void EvaluateMethod(int methodIndex)
        {
            SetMethodState(methodIndex, QueueItemMethodState.Started);

            bool res = false;
            switch (methodIndex)
            {
                case 0:
                    res = EvalMethod0();
                    break;

                case 1:
                    res = EvalMethod1();
                    break;

                case 2:
                    res = EvalMethod2();
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException("methodIndex");
            }

            SetMethodState(methodIndex, QueueItemMethodState.Finished);

            if (res)
            {
                switch (methodIndex)
                {
                    case 0:
                        FireEvalMethod0Events();
                        break;

                    case 1:
                        FireEvalMethod1Events();
                        break;

                    case 2:
                        FireEvalMethod2Events();
                        break;
                }
            }
        }

        #endregion "Queue processing"

        #region "Item data manipulating"

        public override bool HasIcon(View view)
        {
            return (_imageIndexKeys.ContainsKey(view) && this.Parent.GetImageList(view).ContainsKey(_imageIndexKeys[view]));
        }

        public override object GetIconKey(View view)
        {
            IImageList imageList = this.Parent.GetImageList(view);

            if (_imageIndexKeys.ContainsKey(view) && imageList.ContainsKey(_imageIndexKeys[view]))
                return _imageIndexKeys[view];

            int methodIndex = 0;
            if (view == View.Thumbnails)
                methodIndex = 2;

            if (GetMethodState(methodIndex) != QueueItemMethodState.Started)
            {
                lock (this)
                {
                    if (base.Parent != null)
                    {
                        base.Parent.QueueManager.MoveToHead(this, methodIndex);
                        base.Parent.QueueManager.StartQueues();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns item's text information of specified type. If item has no text of specified type, returns empty string.
        /// </summary>
        /// <param name="thumbInfo">Type of requested text information.</param>
        /// <returns>Item's text of the specified type.</returns>
        [ResDescription("IListItem_GetText")]
        public override string GetText(int textInfoId)
        {
            if (textInfoId == ThumbnailListItem.TextInfoIdPath)
                return _pidl.Path;

            int methodIndex = ThumbInfoToMethodIndex(textInfoId);
            if (methodIndex < 0)
                return string.Empty;
            QueueItemMethodState methodState = GetMethodState(methodIndex);

            if (methodState == QueueItemMethodState.Finished)
                return base.GetText(textInfoId);

            if (methodState == QueueItemMethodState.Started)
                return string.Empty;

            lock (this)
            {
                if (base.Parent == null)
                    return string.Empty;

                base.Parent.QueueManager.MoveToHead(this, methodIndex);
                base.Parent.QueueManager.StartQueues();
            }

            return string.Empty;
        }

        public Aurigma.GraphicsMill.Bitmap GetThumbnail()
        {
            if (!HasIcon(View.Thumbnails))
                return null;

            return this.Parent.GetImageList(View.Thumbnails).GetImage(_imageIndexKeys[View.Thumbnails]);
        }

        public void SetThumbnail(Aurigma.GraphicsMill.Bitmap thumbnail)
        {
            if (thumbnail == null)
                throw new System.ArgumentNullException("thumbnail");

            IImageList imageList = this.Parent.GetImageList(View.Thumbnails);
            if (!HasIcon(View.Thumbnails))
            {
                lock (imageList)
                {
                    lock (this)
                    {
                        if (base.Parent != null)
                            base.Parent.QueueManager.Remove((IQueueItem)this);

                        imageList.AddImage(thumbnail, this);
                        _imageIndexKeys[View.Thumbnails] = this;
                    }
                }
            }
            else
            {
                imageList.SetImage(thumbnail, this);
            }

            OnIconChanged(View.Thumbnails);
        }

        public void Reload()
        {
            ChangeParent(base.Parent);
        }

        #endregion "Item data manipulating"

        #region "Private & protected members"

        /// <summary>
        ///	Reinitializes queue manager of the item. Method detaches item from previous manager and attaches it to next.
        /// </summary>
        /// <param name="prevParent"></param>
        /// <param name="newParent"></param>
        private void ChangeParent(ThumbnailListView newParent)
        {
            if (base.Parent != null)
            {
                base.Parent.QueueManager.Remove(this);

                _imageIndexKeys.Clear();
                this.Texts.Clear();
            }

            for (int i = 0; i < _methodCount; i++)
                _methodState[i] = QueueItemMethodState.NotStarted;

            base.Parent = newParent;

            if (newParent != null)
            {
                newParent.QueueManager.Add(this);
                newParent.QueueManager.StartQueues();
            }
        }

        /// <summary>
        /// First method associated with the item.
        /// It retrieves:
        ///    object's display name,
        ///    object's type,
        ///    object's small icon index,
        ///    object's small icon handle,
        ///    object's large icon index,
        ///    object's large icon handle.
        /// </summary>
        /// <returns>true if succeded; otherwise, false.</returns>
        private bool EvalMethod0()
        {
            if (this.Parent == null)
                return false;

            string displayName = string.Empty;
            string type = string.Empty;
            int iconIndexSmall = -1;
            int iconIndexLarge = -1;
            System.IntPtr iconSmall = System.IntPtr.Zero;
            System.IntPtr iconLarge = System.IntPtr.Zero;

            if (FileInfoRetriever.RetrieveFileInfo(_pidl, ref displayName, ref type, ref iconIndexSmall, ref iconSmall, ref iconIndexLarge, ref iconLarge) && this.Parent != null)
            {
                IImageList listImageList = this.Parent.GetImageList(View.List);
                IImageList iconImageList = this.Parent.GetImageList(View.Icons);

                if (listImageList != null)
                    System.Threading.Monitor.Enter(listImageList);

                try
                {
                    if (iconImageList != null)
                        System.Threading.Monitor.Enter(iconImageList);

                    try
                    {
                        lock (this)
                        {
                            try
                            {
                                // Saving display name
                                this.Texts[ThumbnailListItem.TextInfoIdDisplayName] = displayName;

                                // Saving file type
                                this.Texts[ThumbnailListItem.TextInfoIdFileType] = type;

                                if (this.Parent != null)
                                {
                                    // Adding small icon to control ImageList (for List & Details view)
                                    string indexKey = iconIndexSmall.ToString();
                                    _imageIndexKeys[View.List] = indexKey;
                                    listImageList.AddImage(iconSmall, indexKey);
                                    _imageIndexKeys[View.Details] = indexKey;

                                    // Adding large icon to control ImageList (for Icon view)
                                    indexKey = iconIndexLarge.ToString();
                                    iconImageList.AddImage(iconLarge, indexKey);
                                    _imageIndexKeys[View.Icons] = indexKey;
                                }
                            }
                            finally
                            {
                                NativeMethods.DestroyIcon(iconSmall);
                                NativeMethods.DestroyIcon(iconLarge);
                            }
                        }
                    }
                    finally
                    {
                        if (iconImageList != null)
                            System.Threading.Monitor.Exit(iconImageList);
                    }
                }
                finally
                {
                    if (listImageList != null)
                        System.Threading.Monitor.Exit(listImageList);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Fires events of the first method.
        /// </summary>
        private void FireEvalMethod0Events()
        {
            OnTextChanged(ThumbnailListItem.TextInfoIdDisplayName);
            OnTextChanged(ThumbnailListItem.TextInfoIdFileType);
            OnIconChanged(View.Icons);
            OnIconChanged(View.List);
            OnIconChanged(View.Details);
        }

        /// <summary>
        /// Second method associated with the item.
        /// It retrieves:
        ///	    file size,
        ///	    file creation date.
        /// </summary>
        /// <returns>true if succeded; otherwise, false.</returns>
        private bool EvalMethod1()
        {
            long fileSize = 0;
            long fileTime = 0;
            if (FileInfoRetriever.RetrieveFileSizeAndTime(_pidl, ref fileSize, ref fileTime))
            {
                fileSize = (long)System.Math.Ceiling((double)fileSize / 1024);
                string strFileSize = fileSize.ToString(System.Globalization.CultureInfo.CurrentCulture);
                System.DateTime creationTime = System.DateTime.FromFileTime(fileTime);

                string strFileTime = creationTime.ToShortDateString();
                lock (this)
                {
                    if (this.Texts[ThumbnailListItem.TextInfoIdFileSize] == null)
                    {
                        string strFileSizeFull = string.Empty;
                        if (_pidl.Type != PidlType.Folder)
                        {
                            strFileSizeFull = strFileSize + " " + _kilobytesText;
                        }
                        this.Texts.Add(ThumbnailListItem.TextInfoIdFileSize, strFileSizeFull);
                    }
                    if (this.Texts[ThumbnailListItem.TextInfoIdCreationDate] == null)
                    {
                        this.Texts.Add(ThumbnailListItem.TextInfoIdCreationDate, strFileTime);
                    }
                }
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Fires events of the second method.
        /// </summary>
        private void FireEvalMethod1Events()
        {
            OnTextChanged(ThumbnailListItem.TextInfoIdFileSize);
            OnTextChanged(ThumbnailListItem.TextInfoIdCreationDate);
        }

        /// <summary>
        /// Third method associated with the item. It retrieves file thumbnail.
        /// </summary>
        /// <returns>true if succeded; otherwise, false.</returns>
        private bool EvalMethod2()
        {
            if (this.Parent == null)
                return false;

            Aurigma.GraphicsMill.Bitmap image = null;
            System.IntPtr icon = System.IntPtr.Zero;

            try
            {
                if (this.Parent != null)
                {
                    ThumbnailImageList imageList = (ThumbnailImageList)this.Parent.GetImageList(View.Thumbnails);
                    FileInfoRetriever.RetrieveThumbnail(_pidl, imageList.ThumbnailSize.Width, imageList.ThumbnailSize.Height, ref image, ref icon);
                    if (image == null && icon != System.IntPtr.Zero)
                        image = ImageListBase.IconToAlphaBitmap(icon);

                    OnThumbnailLoaded(image);

                    lock (imageList)
                    {
                        lock (this)
                        {
                            _imageIndexKeys[View.Thumbnails] = this;

                            if (this.Parent != null)
                            {
                                _imageIndexKeys[View.Thumbnails] = this;
                                if (imageList.ContainsKey(this))
                                    imageList.SetImage(image, this);
                                else
                                    imageList.AddImage(image, this);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (image != null)
                    image.Dispose();

                NativeMethods.DestroyIcon(icon);
            }

            return true;
        }

        protected virtual void OnThumbnailLoaded(Aurigma.GraphicsMill.Bitmap loadedThumbnail)
        {
        }

        /// <summary>
        /// Fires events of the third method.
        /// </summary>
        private void FireEvalMethod2Events()
        {
            OnIconChanged(View.Thumbnails);
        }

        /// <summary>
        /// Returns number of the item's method which retrieves specified text info type.
        /// </summary>
        /// <param name="thumbInfo">Type of the text info.</param>
        /// <returns>Returns IQueueItem method index.</returns>
        private static int ThumbInfoToMethodIndex(int textInfoId)
        {
            switch (textInfoId)
            {
                case ThumbnailListItem.TextInfoIdDisplayName:
                case ThumbnailListItem.TextInfoIdFileType:
                    return 0;

                case ThumbnailListItem.TextInfoIdFileSize:
                case ThumbnailListItem.TextInfoIdCreationDate:
                    return 1;
            }
            return -1;
        }

        #endregion "Private & protected members"

        #region "ICloneable implementation"

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        [ResDescription("ThumbnailListItem_Clone")]
        public object Clone()
        {
            return new ThumbnailListItem(this);
        }

        #endregion "ICloneable implementation"

        #region Private member variables

        /// <summary>
        /// Keys of the item images for all views.
        /// </summary>
        private System.Collections.Hashtable _imageIndexKeys = new System.Collections.Hashtable(3);

        /// <summary>
        /// Pidl of the underlying file system object.
        /// </summary>
        private Pidl _pidl;

        /// <summary>
        /// Number of associated with item asynchronous methods.
        /// </summary>
        private static int _methodCountInternal = 3;

        /// <summary>
        /// Stores KB string.
        /// </summary>
        private string _kilobytesText = "kb";

        /// <summary>
        /// Array of values that determine is corresponding method finished or not.
        /// </summary>
        private QueueItemMethodState[] _methodState;

        /// <summary>
        /// Methods count (for IQueueItem interface implementation).
        /// </summary>
        private int _methodCount;

        #endregion Private member variables
    }
}