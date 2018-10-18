// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Aurigma.GraphicsMill.WinControls
{
    #region Public Enums

    /// <summary>
    /// Contains identifiers of standard folders.
    /// </summary>
    public enum StandardFolder
    {
        /// <summary>
        /// Desktop folder identifier.
        /// </summary>
        Desktop = 0x0000,

        /// <summary>
        /// Printers folder identfier.
        /// </summary>
        Printers = 0x0004,

        /// <summary>
        /// MyDocuments folder identifier.
        /// </summary>
        MyDocuments = 0x0005,

        /// <summary>
        /// Favorites folder identifier.
        /// </summary>
        Favorites = 0x0006,

        /// <summary>
        /// Recent folder identifier.
        /// </summary>
        Recent = 0x0008,

        /// <summary>
        /// SendTo folder identifier.
        /// </summary>
        SendTo = 0x0009,

        /// <summary>
        /// StartMenu folder identifier.
        /// </summary>
        StartMenu = 0x000b,

        /// <summary>
        /// MyComputer folder identifier.
        /// </summary>
        MyComputer = 0x0011,

        /// <summary>
        /// NetworkNeighborhood folder identifier.
        /// </summary>
        NetworkNeighborhood = 0x0012,

        /// <summary>
        /// Templates folder identifier.
        /// </summary>
        Templates = 0x0015,

        /// <summary>
        /// MyPictures folder identifier.
        /// </summary>
        MyPictures = 0x0027,

        /// <summary>
        /// NetAndDialUpConnections folder identifier.
        /// </summary>
        NetAndDialupConnections = 0x0031,
    }

    public enum PidlType
    {
        Folder = 0,
        File = 1
    }

    #endregion Public Enums

    /// <summary>
    /// Pidl is a wrapper class for Shell32 ITEMIDLIST structures chain management.
    /// </summary>
    public sealed class Pidl : IDisposable
    {
        #region Constants

        private const int MAX_PATH = 260;
        private const int S_OK = 0;

        #endregion Constants

        #region Construction/Destruction

        /// <summary>
        /// Creates Pidl object according to specified StandardFolder enumeration member.
        /// </summary>
        /// <param name="standardFolder">StandardFolder identifier.</param>
        /// <returns>Returns new Pidl object.</returns>
        public static Pidl Create(StandardFolder standardFolder)
        {
            IntPtr pidl = IntPtr.Zero;
            int result = NativeMethods.SHGetSpecialFolderLocation(IntPtr.Zero, (int)standardFolder, out pidl);
            if (result != S_OK || pidl == IntPtr.Zero)
            {
                return null;
            }
            return new Pidl(pidl);
        }

        /// <summary>
        /// Creates Pidl object according to specified PIDL object.
        /// </summary>
        /// <param name="pidl">PIDL object, which represents file or folder.</param>
        /// <returns>Returns new Pidl object.</returns>
        /// <remarks>The new duplicate of specified <paramref name="pidl"/> is created.</remarks>
        public static Pidl Create(IntPtr pidl)
        {
            if (pidl == IntPtr.Zero)
                return null;

            IntPtr pidlNew = IntPtr.Zero;
            LowLevelPidlWorks.Duplicate(pidl, out pidlNew);

            return new Pidl(pidlNew);
        }

        public static Pidl Create(Pidl pidl)
        {
            if (pidl == null)
            {
                return null;
            }
            IntPtr pidlNew = IntPtr.Zero;
            LowLevelPidlWorks.Duplicate(pidl._pidl, out pidlNew);
            return new Pidl(pidlNew);
        }

        /// <summary>
        /// Creates Pidl object according to specified path.
        /// </summary>
        /// <param name="path">File or folder path.</param>
        /// <returns>Returns new Pidl object.</returns>
        public static Pidl Create(string path)
        {
            IntPtr pidlNew = IntPtr.Zero;
            uint flagsIn = 0;
            uint flagsOut = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append(path);
            int result = NativeMethods.SHParseDisplayName(sb, IntPtr.Zero, out pidlNew, flagsIn, out flagsOut);
            if (result != S_OK)
                return null;

            return new Pidl(pidlNew);
        }

        /// <summary>
        /// Internal Pidl object constructor.
        /// </summary>
        /// <param name="pidl">File or Folder PIDL.</param>
        private Pidl(IntPtr pidl)
        {
            this._pidl = pidl;
            this._type = LowLevelPidlWorks.IsStreamable(pidl) ? PidlType.File : PidlType.Folder;
        }

        /// <summary>
        /// Internal Pidl object constructor.
        /// </summary>
        /// <param name="pidl">File or Folder PIDL.</param>
        /// <param name="type">The type of the item.</param>
        private Pidl(IntPtr pidl, PidlType type)
        {
            this._pidl = pidl;
            this._type = type;
        }

        /// <summary>
        /// Pidl's destructor.
        /// </summary>
        ~Pidl()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged resources used by Pidl object.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                System.GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposing)
        {
            if (this._pidl != IntPtr.Zero)
            {
                NativeMethods.CoTaskMemFree(this._pidl);
                this._pidl = IntPtr.Zero;
            }
        }

        #endregion Construction/Destruction

        #region Properties

        /// <summary>
        /// File system path of the object (if it presents).
        /// </summary>
        public string Path
        {
            get
            {
                if (_pathInitialized == false)
                {
                    StringBuilder sb = new StringBuilder(MAX_PATH);
                    if (NativeMethods.SHGetPathFromIDList(_pidl, sb) != 0)
                        _path = sb.ToString();

                    _pathInitialized = true;
                }

                return _path;
            }
        }

        /// <summary>
        /// Stream object of media represented by Pidl.
        /// </summary>
        public Stream Stream
        {
            get
            {
                Stream stream = null;
                try
                {
                    LowLevelPidlWorks.GetStream(this._pidl, out stream);
                }
                catch
                {
                    stream = null;
                }

                return stream;
            }
        }

        /// <summary>
        /// Shell PIDL object representing the item.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return this._pidl;
            }
        }

        /// <summary>
        /// An array of subitems (files & folders)
        /// </summary>
        public Pidl[] Items
        {
            get
            {
                _children = new Pidl[0];
                GetChildren(this._pidl, true, true, ref _children);
                return _children;
            }
        }

        /// <summary>
        /// An array of subfolders
        /// </summary>
        public Pidl[] Folders
        {
            get
            {
                _children = new Pidl[0];
                GetChildren(this._pidl, true, false, ref _children);
                return _children;
            }
        }

        /// <summary>
        /// An array of subitems
        /// </summary>
        public Pidl[] Files
        {
            get
            {
                _children = new Pidl[0];
                GetChildren(this._pidl, false, true, ref _children);
                return _children;
            }
        }

        public Pidl Parent
        {
            get
            {
                if (IsRoot)
                    return null;

                IntPtr parentPidl = IntPtr.Zero;
                LowLevelPidlWorks.CutTail(_pidl, out parentPidl);
                return new Pidl(parentPidl);
            }
        }

        /// <summary>
        /// Equals to true if object is the Desktop folder
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return PidlSize <= 2;
            }
        }

        public PidlType Type
        {
            get
            {
                return this._type;
            }
        }

        private int PidlSize
        {
            get
            {
                if (_pidl == IntPtr.Zero)
                {
                    return 0;
                }

                if (_pidlSizeInitialized == false)
                {
                    _pidlSizeInitialized = true;
                    _pidlSize = LowLevelPidlWorks.GetSize(_pidl);
                }
                return this._pidlSize;
            }
        }

        #endregion Properties

        #region Internal methods

        private static void GetIShellFolderForPidl(IntPtr pidl, out NativeMethods.IShellFolder result)
        {
            result = null;
            NativeMethods.IShellFolder desktopFolder = null;

            if (NativeMethods.SHGetDesktopFolder(out desktopFolder) != 0)
            {
                return;
            }

            Guid guid = new Guid("000214E6-0000-0000-C000-000000000046");
            IntPtr ppv = IntPtr.Zero;
            uint res = desktopFolder.BindToObject(pidl, IntPtr.Zero, ref guid, out ppv);
            Marshal.ReleaseComObject(desktopFolder);
            if (res != 0)
                return;

            IntPtr shellFolder = IntPtr.Zero;
            Marshal.QueryInterface(ppv, ref guid, out shellFolder);
            Marshal.Release(ppv);
            if (shellFolder == IntPtr.Zero)
                return;

            object unk = Marshal.GetObjectForIUnknown(shellFolder);
            Marshal.Release(shellFolder);
            if (unk == null)
                return;

            result = unk as NativeMethods.IShellFolder;
        }

        private static void GetChildren(IntPtr pidl, bool getFolders, bool getFiles, ref Pidl[] _children)
        {
            NativeMethods.IShellFolder folder = null;

            if (LowLevelPidlWorks.GetSize(pidl) <= 2)
            {
                if (NativeMethods.SHGetDesktopFolder(out folder) != 0)
                    return;
            }
            else
            {
                GetIShellFolderForPidl(pidl, out folder);
            }

            if (folder == null)
                return;

            try
            {
                NativeMethods.IEnumIDList enumIdList = null;
                ArrayList items = new ArrayList();

                if (getFolders)
                {
                    uint itemsToInclude = NativeMethods.SHCONTF_FOLDERS/* | WinApiProvider.SHCONTF.SHCONTF_INCLUDEHIDDEN*/;
                    if (folder.EnumObjects(IntPtr.Zero, itemsToInclude, out enumIdList) == S_OK)
                    {
                        try
                        {
                            IntPtr newPidl = IntPtr.Zero;
                            IntPtr fetched = IntPtr.Zero;
                            enumIdList.Reset();
                            while (enumIdList.Next(1, out newPidl, out fetched) == 0)
                            {
                                IntPtr pidlConcat = IntPtr.Zero;
                                LowLevelPidlWorks.Concatenate(pidl, newPidl, out pidlConcat);
                                NativeMethods.CoTaskMemFree(newPidl);

                                Pidl item = new Pidl(pidlConcat, PidlType.Folder);
                                items.Add(item);

                                newPidl = IntPtr.Zero;
                            }
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(enumIdList);
                        }
                    }
                }

                if (getFiles)
                {
                    uint itemsToInclude = NativeMethods.SHCONTF_NONFOLDERS /*| WinApiProvider.SHCONTF.SHCONTF_INCLUDEHIDDEN |  WinApiProvider.SHCONTF.SHCONTF_STORAGE*/;
                    if (folder.EnumObjects(IntPtr.Zero, itemsToInclude, out enumIdList) == S_OK)
                    {
                        try
                        {
                            IntPtr newPidl = IntPtr.Zero;
                            IntPtr fetched = IntPtr.Zero;
                            enumIdList.Reset();
                            while (enumIdList.Next(1, out newPidl, out fetched) == 0)
                            {
                                IntPtr pidlConcat = IntPtr.Zero;
                                LowLevelPidlWorks.Concatenate(pidl, newPidl, out pidlConcat);
                                NativeMethods.CoTaskMemFree(newPidl);

                                if (LowLevelPidlWorks.HasSuchAttributes(pidlConcat, NativeMethods.SFGAO_STREAM))
                                {
                                    Pidl item = new Pidl(pidlConcat, PidlType.File);
                                    items.Add(item);
                                }
                                else
                                {
                                    NativeMethods.CoTaskMemFree(pidlConcat);
                                }

                                newPidl = IntPtr.Zero;
                            }
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(enumIdList);
                        }
                    }
                }

                _children = new Pidl[items.Count];
                for (int i = 0; i < _children.Length; i++)
                    _children[i] = (Pidl)items[i];
            }
            finally
            {
                Marshal.ReleaseComObject(folder);
            }
        }

        #endregion Internal methods

        #region LowLevelPidlWorks class

        internal class LowLevelPidlWorks
        {
            private static object getStreamSyncObject = new object();

            private LowLevelPidlWorks()
            {
            }

            internal static int GetSize(IntPtr pidl)
            {
                int cbTotal = 0;
                int offset = 0;
                if (pidl != IntPtr.Zero)
                {
                    Int16 cb = Marshal.ReadInt16(pidl, 0);
                    while (cb > 0 && cbTotal < 32000)
                    {
                        cbTotal += cb;
                        offset += cb;
                        cb = Marshal.ReadInt16(pidl, offset);
                    }
                    cbTotal += 2 * 1 /*sizeof(byte)*/;
                }
                return cbTotal;
            }

            internal static void Create(int size, out IntPtr resultPidl)
            {
                resultPidl = NativeMethods.CoTaskMemAlloc((uint)size);
            }

            internal static void Duplicate(IntPtr pidlSrc, out IntPtr pidlDest)
            {
                pidlDest = IntPtr.Zero;
                if (pidlSrc == IntPtr.Zero)
                {
                    return;
                }
                int size = GetSize(pidlSrc);
                if (size == 0)
                {
                    return;
                }
                Create(size, out pidlDest);
                if (pidlDest == IntPtr.Zero)
                {
                    return;
                }
                NativeMethods.CopyMemory(pidlDest, pidlSrc, size);
            }

            internal static void Concatenate(IntPtr pidl1, IntPtr pidl2, out IntPtr pidl)
            {
                int cb1 = 0;
                int cb2 = 0;

                if (pidl1 != IntPtr.Zero)
                    cb1 = GetSize(pidl1) - (2 * 1/*sizeof(BYTE)*/);

                cb2 = GetSize(pidl2);
                Create(cb1 + cb2, out pidl);
                if (pidl != IntPtr.Zero)
                {
                    byte[] interimArray = null;
                    if (pidl1 != IntPtr.Zero)
                    {
                        interimArray = new byte[cb1];
                        Marshal.Copy(pidl1, interimArray, 0, cb1);
                        Marshal.Copy(interimArray, 0, pidl, cb1);
                    }

                    IntPtr dstPtr = new IntPtr(pidl.ToInt64() + cb1);

                    interimArray = new byte[cb2];
                    Marshal.Copy(pidl2, interimArray, 0, cb2);
                    Marshal.Copy(interimArray, 0, dstPtr, cb2);
                }
            }

            internal static void CutTail(IntPtr srcPidl, out IntPtr dstPidl)
            {
                dstPidl = IntPtr.Zero;

                IntPtr srcPtr = srcPidl;
                Int16 cbCur = Marshal.ReadInt16(srcPtr);
                int copySize = 0;

                while (true)
                {
                    if (cbCur == 0)
                        break;

                    srcPtr = new IntPtr(srcPtr.ToInt64() + cbCur);
                    Int16 cbNext = Marshal.ReadInt16(srcPtr);
                    if (cbNext == 0)
                        break;

                    copySize += cbCur;
                    cbCur = cbNext;
                }

                if (copySize == 0)
                {
                    byte[] interimArray = new byte[2];
                    interimArray[0] = 0;
                    interimArray[1] = 0;

                    Create(2, out dstPidl);
                    Marshal.Copy(interimArray, 0, dstPidl, copySize + 2);
                }
                else
                {
                    byte[] interimArray = new byte[copySize + 2];
                    interimArray[copySize] = 0;
                    interimArray[copySize + 1] = 0;

                    Create(copySize + 2, out dstPidl);
                    Marshal.Copy(srcPidl, interimArray, 0, copySize);
                    Marshal.Copy(interimArray, 0, dstPidl, copySize + 2);
                }
            }

            internal static void GetAttributes(IntPtr pidl, ref uint uintAttributes)
            {
                if (pidl == IntPtr.Zero)
                {
                    uintAttributes = 0;
                    return;
                }
                NativeMethods.SHFILEINFO sfi = new NativeMethods.SHFILEINFO(true);
                sfi.dwAttributes = uintAttributes;
                int cbFileInfo = Marshal.SizeOf(sfi);
                uint flags = NativeMethods.SHGFI_ATTRIBUTES | NativeMethods.SHGFI_PIDL;
                IntPtr res = NativeMethods.SHGetFileInfo(pidl, NativeMethods.FILE_ATTRIBUTE_NORMAL, out sfi, (uint)cbFileInfo, flags);
                if (res == IntPtr.Zero)
                {
                    uintAttributes = 0;
                    return;
                }
                uintAttributes &= sfi.dwAttributes;
            }

            internal static bool HasSuchAttributes(IntPtr pidl, uint uintAttributes)
            {
                uint uintRecevedAttributes = uintAttributes;
                GetAttributes(pidl, ref uintRecevedAttributes);
                if (uintAttributes == (uintRecevedAttributes & uintAttributes))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            internal static bool IsStreamable(IntPtr pidl)
            {
                return HasSuchAttributes(pidl, NativeMethods.SFGAO_STREAM);
            }

            internal static bool GetStream(IntPtr pidl, out Stream stream)
            {
                lock (getStreamSyncObject)
                {
                    stream = null;
                    if (pidl == IntPtr.Zero)
                        return false;

                    IStream piStream = null;

                    if (HasSuchAttributes(pidl, NativeMethods.SFGAO_STREAM))
                    {
                        NativeMethods.IShellFolder desktopFolder = null;

                        if (NativeMethods.SHGetDesktopFolder(out desktopFolder) != 0)
                            return false;

                        Guid guid = new Guid("0000000c-0000-0000-C000-000000000046");
                        IntPtr ppv = IntPtr.Zero;
                        uint res = desktopFolder.BindToObject(pidl, IntPtr.Zero, ref guid, out ppv);
                        Marshal.ReleaseComObject(desktopFolder);
                        if (res != 0)
                            return false;

                        object objUnk = Marshal.GetObjectForIUnknown(ppv);

                        Marshal.Release(ppv);
                        if (objUnk == null)
                            return false;

                        piStream = objUnk as IStream;
                        if (piStream == null)
                            return false;
                    }

                    if (piStream == null)
                        return false;

                    ComStreamWrapper comStreamWrapper = new ComStreamWrapper(piStream);
                    comStreamWrapper.Seek(0, System.IO.SeekOrigin.Begin);
                    stream = (Stream)comStreamWrapper;
                    return true;
                }
            }
        }

        #endregion LowLevelPidlWorks class

        #region Members

        private IntPtr _pidl = IntPtr.Zero;
        private bool _pathInitialized;
        private string _path = string.Empty;
        private Pidl[] _children = new Pidl[0];
        private bool _pidlSizeInitialized;
        private int _pidlSize;
        private PidlType _type;

        #endregion Members
    }
}