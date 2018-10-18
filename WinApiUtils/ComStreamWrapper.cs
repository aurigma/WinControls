// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Runtime.InteropServices;

namespace Aurigma
{
    #region Enums

    /// <summary>
    /// The STREAM_SEEK enumeration values specify the origin from which to calculate the new seek-pointer location
    /// </summary>
    internal enum STREAM_SEEK : int
    {
        /// <summary>
        /// The new seek pointer is an offset relative to the beginning of the stream.
        /// </summary>
        STREAM_SEEK_SET = 0,

        /// <summary>
        /// The new seek pointer is an offset relative to the current seek pointer location.
        /// </summary>
        STREAM_SEEK_CUR = 1,

        /// <summary>
        /// The new seek pointer is an offset relative to the end of the stream.
        /// </summary>
        STREAM_SEEK_END = 2
    }

    /// <summary>
    /// The STGC enumeration constants specify the conditions for performing the commit operation in the IStorage::Commit and IStream::Commit methods.
    /// </summary>
    [Flags]
    internal enum STGC : int
    {
        /// <summary>
        /// You can specify this condition with STGC_CONSOLIDATE, or some combination of the other three flags in this list of elements. Use this value to increase the readability of code.
        /// </summary>
        STGC_DEFAULT = 0,

        /// <summary>
        /// The commit operation can overwrite existing data to reduce overall space requirements.
        /// </summary>
        STGC_OVERWRITE = 1,

        /// <summary>
        /// Prevents multiple users of a storage object from overwriting each another's changes.
        /// </summary>
        STGC_ONLYIFCURRENT = 2,

        /// <summary>
        /// Commits the changes to a write-behind disk cache, but does not save the cache to the disk.
        /// </summary>
        STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,

        /// <summary>
        /// Windows 2000 and Windows XP: Indicates that a storage should be consolidated after it is committed, resulting in a smaller file on disk. This flag is valid only on the outermost storage object that has been opened in transacted mode. It is not valid for streams. The STGC_CONSOLIDATE flag can be combined with any other STGC flags.
        /// </summary>
        STGC_CONSOLIDATE = 8
    }

    /// <summary>
    /// The STATFLAG enumeration values indicate whether the method should try to return a name in the pwcsName member of the STATSTG structure.
    /// </summary>
    [Flags]
    internal enum STATFLAG : int
    {
        /// <summary>
        /// Requests that the statistics include the pwcsName member of the STATSTG structure.
        /// </summary>
        STATFLAG_DEFAULT = 0,

        /// <summary>
        /// Requests that the statistics not include the pwcsName member of the STATSTG structure.
        /// </summary>
        STATFLAG_NONAME = 1,

        /// <summary>
        /// Not implemented.
        /// </summary>
        STATFLAG_NOOPEN = 2
    }

    #endregion Enums

    #region Interfaces

    /// <summary>
    /// The IStream interface lets you read and write data to stream objects. Stream objects contain the data in a structured storage object, where storages provide the structure.
    /// </summary>
    [ComImportAttribute()]
    [GuidAttribute("0000000c-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IStream
    {
        /// <summary>
        /// This method reads a specified number of bytes from the stream object into memory, starting at the current seek pointer
        /// </summary>
        /// <param name="pv">Pointer to the buffer into which the stream data is read. If an error occurs, this value is NULL.</param>
        /// <param name="cb">Number of bytes of data to attempt to read from the stream object.</param>
        /// <param name="pcbRead">Pointer to a ULONG variable that receives the actual number of bytes read from the stream object</param>
        /// <returns>If the method succeeds, the return value is S_OK.</returns>
        [PreserveSig()]
        uint Read(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv,
            int cb,
            ref int pcbRead);

        /// <summary>
        /// This method writes a specified number of bytes into the stream object starting at the current seek pointer.
        /// </summary>
        /// <param name="pv">Pointer to the memory buffer.</param>
        /// <param name="cb">Number of bytes of data to write from the stream object.</param>
        /// <param name="pcbWritten">Pointer to a ULONG variable that receives the actual number of bytes written from the stream object.</param>
        /// <returns>If the method succeeds, the return value is S_OK.</returns>
        [PreserveSig()]
        uint Write(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv,
            int cb,
            ref int pcbWritten);

        /// <summary>
        /// This method changes the seek pointer to a new location relative to the current seek pointer or the beginning or end of the stream.
        /// </summary>
        /// <param name="dlibMove">Displacement to be added to the location indicated by the dwOrigin parameter. If dwOrigin is STREAM_SEEK_SET, this is interpreted as an unsigned value rather than signed.</param>
        /// <param name="dwOrigin">Origin for the displacement specified in dlibMove. The origin can be the beginning of the file, the current seek pointer, or the end of the file.</param>
        /// <param name="plibNewPosition">Pointer to the location where this method writes the value of the new seek pointer from the beginning of the stream.</param>
        /// <returns>Returns S_OK if the seek pointer has been successfully adjusted.</returns>
        [PreserveSig()]
        uint Seek(
            long dlibMove,
            STREAM_SEEK dwOrigin,
            ref long plibNewPosition);

        /// <summary>
        /// This method resizes the stream object.
        /// </summary>
        /// <param name="libNewSize">New size, in bytes, of the stream.</param>
        /// <returns>Returns S_OK if the size of the stream object was successfully changed.</returns>
        [PreserveSig()]
        uint SetSize(
            long libNewSize);

        /// <summary>
        /// This method copies a specified number of bytes from the current seek pointer in the stream to the current seek pointer in another stream.
        /// </summary>
        /// <param name="pstm">Pointer to the destination stream.</param>
        /// <param name="cb">Number of bytes to copy from the source stream.</param>
        /// <param name="pcbRead">Pointer to the location where this method writes the actual number of bytes read from the source</param>
        /// <param name="pcbWritten">Pointer to the location where this method writes the actual number of bytes written to the destination.</param>
        /// <returns>Returns S_OK if the stream object was successfully copied.</returns>
        [PreserveSig()]
        uint CopyTo(
            [In()]
            ref IStream pstm,
            long cb,
            ref long pcbRead,
            ref long pcbWritten);

        /// <summary>
        /// This method ensures that any changes made to a stream object opened in transacted mode are reflected in the parent storage.
        /// </summary>
        /// <param name="grfCommitFlags">Value that controls how the changes for the stream object are committed.</param>
        /// <returns>Returns S_OK if changes to the stream object were successfully committed to the parent level.</returns>
        [PreserveSig()]
        uint Commit(/*ref */STGC grfCommitFlags);

        /// <summary>
        /// This method discards all changes that have been made to a transacted stream since the last IStream::Commit call.
        /// </summary>
        /// <returns>Returns S_OK if the stream was successfully reverted to its previous version.</returns>
        [PreserveSig()]
        uint Revert();

        /// <summary>
        /// This method restricts access to a specified range of bytes in the stream.
        /// </summary>
        /// <param name="libOffset">Integer that specifies the byte offset for the beginning of the range.</param>
        /// <param name="cb">Integer that specifies the length of the range, in bytes, to be restricted.</param>
        /// <param name="dwLockType">Specifies the restrictions being requested on accessing the range.</param>
        /// <returns>Returns S_OK if the specified range of bytes was locked.</returns>
        [PreserveSig()]
        uint LockRegion(long libOffset, long cb, int dwLockType);

        /// <summary>
        /// This method removes the access restriction on a range of bytes previously restricted with IStream::LockRegion.
        /// </summary>
        /// <param name="libOffset">Byte offset for the beginning of the range.</param>
        /// <param name="cb">Length, in bytes,of the range to be restricted.</param>
        /// <param name="dwLockType">Access restrictions previously placed on the range.</param>
        /// <returns>Returns S_OK if the byte range was unlocked.</returns>
        [PreserveSig()]
        uint UnlockRegion(long libOffset, long cb, int dwLockType);

        /// <summary>
        /// This method retrieves the STATSTG structure for this stream object.
        /// </summary>
        /// <param name="pstatstg">Pointer to a STATSTG structure where this method places information about this stream object. This pointer is NULL if an error occurs.</param>
        /// <param name="grfStatFlag">Value that specifies that this method does not return some of the members in the STATSTG structure. This saves a memory allocation operation.</param>
        /// <returns>Returns S_OK if the STATSTG structure was successfully returned at the specified location.</returns>
        [PreserveSig()]
        uint Stat(ref STATSTG pstatstg, STATFLAG grfStatFlag);

        /// <summary>
        /// This method creates a new stream object with its own seek pointer that references the same bytes as the original stream.
        /// </summary>
        /// <param name="ppstm">When successful, pointer to the location of an IStream interface pointer to the new stream object</param>
        /// <returns>Returns S_OK if the stream was successfully cloned.</returns>
        [PreserveSig()]
        uint Clone(out IStream ppstm);
    }

    #endregion Interfaces

    /// <summary>
    /// The ComStreamWrapper class wraps COM IStream interface.
    /// </summary>
    internal class ComStreamWrapper : System.IO.Stream
    {
        #region Construction/Destruction

        /// <summary>
        /// Initializes a new instance of the ComStreamWrapper class.
        /// </summary>
        /// <param name="stream">COM IStream object to be wrapped.</param>
        public ComStreamWrapper(IStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            this._iStream = stream;
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        ~ComStreamWrapper()
        {
            Close();
        }

        #endregion Construction/Destruction

        #region Stream implementation

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count- 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream</param>
        /// <returns>Returns the total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_iStream == null)
                throw new ObjectDisposedException("ComStreamWrapper");

            int bytesRead = 0;
            if (offset != 0)
            {
                byte[] tmpBuffer = new byte[count];
                _iStream.Read(tmpBuffer, count, ref bytesRead);
                System.Array.Copy(tmpBuffer, 0, buffer, offset, bytesRead);
            }
            else
            {
                uint hr = _iStream.Read(buffer, count, ref bytesRead);
                if (hr != 0)
                    throw new System.IO.IOException("Error while reading from stream.");
            }
            return bytesRead;
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_iStream == null)
                throw new ObjectDisposedException("ComStreamWrapper");

            int written = 0;
            if (offset != 0)
            {
                int buffSize = buffer.Length - offset;
                byte[] tmpBuffer = new Byte[buffSize];
                System.Array.Copy(buffer, offset, tmpBuffer, 0, buffSize);
                _iStream.Write(tmpBuffer, buffSize, ref written);
            }
            else
            {
                _iStream.Write(buffer, count, ref written);
            }
        }

        /// <summary>
        /// Sets the position within the current stream
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>Returns the new position within the current stream.</returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            if (_iStream == null)
                throw new ObjectDisposedException("ComStreamWrapper");

            long curPosition = 0;
            _iStream.Seek(offset, (STREAM_SEEK)origin, ref curPosition);
            return curPosition;
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            if (_iStream == null)
                throw new ObjectDisposedException("ComStreamWrapper");

            _iStream.SetSize(value);
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            if (_iStream == null)
                throw new ObjectDisposedException("ComStreamWrapper");

            _iStream.Commit(STGC.STGC_DEFAULT);
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// </summary>
        public override void Close()
        {
            if (_iStream != null)
            {
                _iStream.Commit(STGC.STGC_DEFAULT);
                while (Marshal.ReleaseComObject(_iStream) != 0) ;
                _iStream = null;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get
            {
                if (_iStream == null)
                    throw new ObjectDisposedException("ComStreamWrapper");

                STATSTG statstg = new STATSTG();
                _iStream.Stat(ref statstg, STATFLAG.STATFLAG_NONAME /* STATFLAG_NONAME */);
                return statstg.cbSize;
            }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return Seek(0, System.IO.SeekOrigin.Current);
            }
            set
            {
                Seek(value, System.IO.SeekOrigin.Begin);
            }
        }

        #endregion Stream implementation

        #region Private members

        /// <summary>
        /// COM IStream object reference.
        /// </summary>
        private IStream _iStream;

        #endregion Private members
    }
}