﻿using System;
using System.Runtime.InteropServices;

namespace RoseByte.SharpFiles.CopyFileEx
{
    public class FileEx
    {
        private long _transferred;
        private long _size;
        private readonly Action<long, long> _progress;

        public FileEx(Action<long, long> progress)
        {
            _progress = progress;
        }
        
        public void Copy(FsFile source, FsFile dest)
        {
            _size = source.Size;
            var cancel = 0;
            var success = CopyFileEx(
                source, 
                dest, 
                CopyProgressRoutineImpl, 
                IntPtr.Zero, 
                ref cancel, 
                CopyFileFlags.COPY_FILE_RESTARTABLE);
            
            if (!success)
            {
                throw new Exception($"File '{source}' could not be copied to '{dest}'");
            }

            if (_transferred != _size)
            {
                _progress(_size - _transferred, _size);
            }
        }

        private CopyProgressResult CopyProgressRoutineImpl(
            long totalFileSize,
            long totalBytesTransferred,
            long streamSize,
            long streamBytesTransferred,
            uint dwStreamNumber,
            CopyProgressCallbackReason dwCallbackReason,
            IntPtr hSourceFile,
            IntPtr hDestinationFile,
            IntPtr lpData)
        {
            var current = totalBytesTransferred - _transferred;        
            
            if (current > 1000000)
            {
                _progress?.Invoke(current, totalBytesTransferred);
                _transferred = totalBytesTransferred;
            }
            
            return CopyProgressResult.PROGRESS_CONTINUE;
        }
        
        #region DLL Import
    
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, 
            CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel, CopyFileFlags dwCopyFlags);
    
        private delegate CopyProgressResult CopyProgressRoutine(
            long totalFileSize,
            long totalBytesTransferred,
            long streamSize,
            long streamBytesTransferred,
            uint dwStreamNumber,
            CopyProgressCallbackReason dwCallbackReason,
            IntPtr hSourceFile,
            IntPtr hDestinationFile,
            IntPtr lpData);
        #endregion
    }
}