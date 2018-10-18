// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Determines interface of the ThumbnailListView control's image list.
    /// </summary>
    [ResDescription("IListViewImageList")]
    public interface IImageList : IDisposable
    {
        #region Methods

        /// <summary>
        /// Adds icon from to the image list.
        /// </summary>
        /// <param name="key">Item key.</param>
        /// <param name="icon">Handle of the icon to add.</param>
        /// <returns>Index of the added image.</returns>
        [ResDescription("IListViewImageList_AddImage0")]
        void AddImage(IntPtr icon, object key);

        /// <summary>
        /// Adds icon to the image list.
        /// </summary>
        /// <param name="icon">Icon to add.</param>
        /// <param name="key">Item key.</param>
        [ResDescription("IListViewImageList_AddImage0")]
        void AddImage(System.Drawing.Icon icon, object key);

        /// <summary>
        /// Adds image to the image list.
        /// </summary>
        /// <param name="icon">Image to add.</param>
        /// <param name="key">Item key.</param>
        [ResDescription("IListViewImageList_AddImage1")]
        void AddImage(Aurigma.GraphicsMill.Bitmap image, object key);

        /// <summary>
        /// Sets image list default image.
        /// </summary>
        /// <param name="icon">Default icon handle.</param>
        [ResDescription("IListViewImageList_AddDefaultImage0")]
        void SetDefaultImage(IntPtr icon);

        /// <summary>
        /// Sets image list default image.
        /// </summary>
        /// <param name="icon">Default icon handle.</param>
        [ResDescription("IListViewImageList_AddDefaultImage0")]
        void SetDefaultImage(System.Drawing.Icon icon);

        /// <summary>
        /// Sets default image.
        /// </summary>
        /// <param name="image">Default image object.</param>
        [ResDescription("IListViewImageList_AddDefaultImage1")]
        void SetDefaultImage(Aurigma.GraphicsMill.Bitmap image);

        /// <summary>
        /// Deletes all objects from the image list.
        /// </summary>
        [ResDescription("IListViewImageList_Clear")]
        void Clear();

        /// <summary>
        /// Determines whether the item with specified key is presented in the image list.
        /// </summary>
        /// <param name="key">Item key.</param>
        /// <returns>Returns true if item with specified key is presented in the image list.</returns>
        [ResDescription("IListViewImageList_ContainsKey")]
        bool ContainsKey(object key);

        /// <summary>
        /// Finds index of the item identified by the key.
        /// </summary>
        /// <param name="key">Item key.</param>
        /// <param name="useDefaultImageIndex">Specifies whether to return default image index if search fails.</param>
        /// <returns>Returns index of the item if succeeded. If fails and useDefaultImageIndex is true and default image has been specified - the method returns default image index. Otherwise returns -1.</returns>
        [ResDescription("IListViewImageList_IndexOfKey")]
        int IndexOfKey(object key, bool useDefaultImageIndex);

        /// <summary>
        /// Finds index of the item identified by the key.
        /// </summary>
        /// <param name="key">Item key.</param>
        /// <returns>Returns index of the item if succeeded. If fails and default image has been specified - the method returns default image index. Otherwise returns -1.</returns>
        [ResDescription("IListViewImageList_IndexOfKey")]
        int IndexOfKey(object key);

        /// <summary>
        /// Removes item identified by the key from the image list.
        /// </summary>
        /// <param name="key">Item key.</param>
        [ResDescription("IListViewImageList_RemoveByKey")]
        void RemoveByKey(object key);

        /// <summary>
        /// Returns copy of the bitmap stored in the image list with specified index.
        /// </summary>
        /// <param name="index">Index of the entry to retrieve.</param>
        /// <returns></returns>
        Aurigma.GraphicsMill.Bitmap GetImage(object key);

        /// <summary>
        /// Changes the entry with specified index.
        /// </summary>
        /// <param name="image">New entry value.</param>
        /// <param name="index">Index of the entry to change.</param>
        void SetImage(Aurigma.GraphicsMill.Bitmap image, object key);

        /// <summary>
        /// Gets handle to the image list's underlying WinAPI object.
        /// </summary>
        IntPtr Handle
        {
            get;
        }

        event ImageRemovedEventHandler ImageRemoved;

        #endregion Methods
    }

    #region "Image removed event class & handler"

    public class ImageRemovedEventArgs : System.EventArgs
    {
        public ImageRemovedEventArgs(object imageKey, int imageIndex)
        {
            if (imageIndex < 0)
                throw new System.ArgumentOutOfRangeException("imageIndex");

            _imageKey = imageKey;
            _imageIndex = imageIndex;
        }

        public object ImageKey
        {
            get
            {
                return _imageKey;
            }
        }

        public int ImageIndex
        {
            get
            {
                return _imageIndex;
            }
        }

        private object _imageKey;
        private int _imageIndex;
    }

    public delegate void ImageRemovedEventHandler(object sender, ImageRemovedEventArgs e);

    #endregion "Image removed event class & handler"
}