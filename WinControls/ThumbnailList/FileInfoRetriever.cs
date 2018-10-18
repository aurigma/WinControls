// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Retrieves information (small and large icon indices, display name, type, file size, creation date and thumbnail) for specified file.
    /// </summary>
    internal class FileInfoRetriever
    {
        private FileInfoRetriever()
        {
        }

        /// <summary>
        /// Returns icon handle (small or large) for specified file.
        /// </summary>
        /// <param name="pidl">Source object for which icon should be retrieved.</param>
        /// <param name="bSmall">If value of the parameter is true small icon handle will be returned, large icon handle otherwise.</param>
        /// <returns>Returns icon handle for specified file.</returns>
        public static IntPtr GetIcon(Pidl pidl, bool small)
        {
            NativeMethods.SHFILEINFO info = new NativeMethods.SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);

            uint flags = NativeMethods.SHGFI_ICON | NativeMethods.SHGFI_PIDL | NativeMethods.SHGFI_USEFILEATTRIBUTES | (small ? NativeMethods.SHGFI_SMALLICON : NativeMethods.SHGFI_LARGEICON);
            if (NativeMethods.SHGetFileInfo(pidl.Handle, 0, out info, (uint)cbFileInfo, flags) == IntPtr.Zero)
                return IntPtr.Zero;

            return info.hIcon;
        }

        /// <summary>
        /// Retrieves general file information.
        /// </summary>
        /// <param name="pidl">Source object for which info should be retrieved.</param>
        /// <param name="displayName">Reference to variable where to store file display name.</param>
        /// <param name="type">Reference to variable where to store file type.</param>
        /// <param name="iconIndexSmall">Reference to variable where to store small icon index.</param>
        /// <param name="iconSmall">Reference to variable where to store small icon handle.</param>
        /// <param name="iconIndexLarge">Reference to variable where to store large icon index.</param>
        /// <param name="iconLarge">Reference to variable where to store large icon handle.</param>
        /// <returns>True if succeeded, false otherwise.</returns>
        public static bool RetrieveFileInfo(Pidl pidl, ref string displayName, ref string type, ref int iconIndexSmall, ref IntPtr iconSmall, ref int iconIndexLarge, ref IntPtr iconLarge)
        {
            NativeMethods.SHFILEINFO info = new NativeMethods.SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            uint flags = NativeMethods.SHGFI_PIDL |
                        NativeMethods.SHGFI_USEFILEATTRIBUTES |
                        NativeMethods.SHGFI_DISPLAYNAME |
                        NativeMethods.SHGFI_TYPENAME |
                        NativeMethods.SHGFI_SYSICONINDEX |
                        NativeMethods.SHGFI_ICON |
                        NativeMethods.SHGFI_SMALLICON;

            if (NativeMethods.SHGetFileInfo(pidl.Handle, 0, out info, (uint)cbFileInfo, flags) == IntPtr.Zero)
                return false;

            displayName = info.szDisplayName;
            type = info.szTypeName;
            iconIndexSmall = info.iIcon;
            iconSmall = info.hIcon;

            info.hIcon = IntPtr.Zero;
            flags = NativeMethods.SHGFI_PIDL |
                    NativeMethods.SHGFI_USEFILEATTRIBUTES |
                    NativeMethods.SHGFI_SYSICONINDEX |
                    NativeMethods.SHGFI_ICON |
                    NativeMethods.SHGFI_LARGEICON;

            if (NativeMethods.SHGetFileInfo(pidl.Handle, 0, out info, (uint)cbFileInfo, flags) == IntPtr.Zero)
            {
                NativeMethods.DestroyIcon(info.hIcon);
                return false;
            }

            iconIndexLarge = info.iIcon;
            iconLarge = info.hIcon;
            return true;
        }

        /// <summary>
        /// Retrieves file creation date and file size.
        /// </summary>
        /// <param name="pidl">Source object for which info should be retrieved.</param>
        /// <param name="size">Reference to variable where to store file size.</param>
        /// <param name="time">Reference to variable where to file creation date.</param>
        /// <returns>True if succeeded, false otherwise.</returns>
        public static bool RetrieveFileSizeAndTime(Pidl pidl, ref long size, ref long time)
        {
            if (pidl.Path == null || pidl.Path.Length == 0)
                return false;

            FileInfo info = new FileInfo(pidl.Path);
            if (info == null)
                return false;

            if (pidl.Type == PidlType.Folder)
                size = 0;
            else
                size = info.Length;

            time = info.CreationTime.ToFileTime();
            return true;
        }

        /// <summary>
        /// Retrieves thumbnail for specified file.
        /// </summary>
        /// <param name="pidl">Source object for which info should be retrieved.</param>
        /// <param name="width">Desired thumbnail width.</param>
        /// <param name="height">Desired thumbnail heigth</param>
        /// <param name="image">Reference to variable where to store result thumbnail.</param>
        /// <param name="icon">Reference to variable where to store large icon handle, in case when file thumbnail cannot be created.</param>
        public static void RetrieveThumbnail(Pidl pidl, int width, int height, ref Aurigma.GraphicsMill.Bitmap image, ref IntPtr icon)
        {
            image = null;
            icon = IntPtr.Zero;

            try
            {
                using (var stream = pidl.Stream)
                using (var reader = Aurigma.GraphicsMill.Codecs.ImageReader.Create(stream))
                {
                    if (reader != null)
                    {
                        image = ExtractExifThumbnail(reader, width, height);

                        if (image == null)
                        {
                            image = reader.Frames[0].GetBitmap();
                            image.Transforms.Resize(width, height, Transforms.ResizeInterpolationMode.High, Transforms.ResizeMode.Fit);
                        }
                    }
                }
            }
            catch
            {
                image = null;
            }

            if (image == null)
                icon = FileInfoRetriever.GetIcon(pidl, false);
        }

        private static Aurigma.GraphicsMill.Bitmap ExtractExifThumbnail(Aurigma.GraphicsMill.Codecs.ImageReader reader, int width, int height)
        {
            Aurigma.GraphicsMill.Codecs.ExifDictionary exif = null;
            Aurigma.GraphicsMill.Bitmap result = null;

            try
            {
                Aurigma.GraphicsMill.Codecs.JpegReader jpgReader = reader as Aurigma.GraphicsMill.Codecs.JpegReader;
                if (jpgReader != null)
                    exif = jpgReader.Exif;
                else
                {
                    Aurigma.GraphicsMill.Codecs.TiffReader tiffReader = reader as Aurigma.GraphicsMill.Codecs.TiffReader;
                    if (tiffReader != null)
                        exif = tiffReader.Exif;
                    else
                    {
                        Aurigma.GraphicsMill.Codecs.Psd.PsdReader psdReader = reader as Aurigma.GraphicsMill.Codecs.Psd.PsdReader;
                        if (psdReader != null)
                            exif = psdReader.Exif;
                    }
                }

                if (exif != null && exif.Contains(Aurigma.GraphicsMill.Codecs.ExifDictionary.Thumbnail))
                {
                    Aurigma.GraphicsMill.Bitmap exifThumbnail = (Aurigma.GraphicsMill.Bitmap)exif[Aurigma.GraphicsMill.Codecs.ExifDictionary.Thumbnail];
                    if (exifThumbnail.Width >= width || exifThumbnail.Height >= height)
                    {
                        exifThumbnail.Transforms.Resize(width, height, Transforms.ResizeInterpolationMode.High, Aurigma.GraphicsMill.Transforms.ResizeMode.Shrink);
                        result = exifThumbnail;
                    }
                    else
                        exifThumbnail.Dispose();
                }
            }
            catch
            {
                if (exif != null)
                    exif.Dispose();
                exif = null;
                result = null;
            }

            return result;
        }
    }
}