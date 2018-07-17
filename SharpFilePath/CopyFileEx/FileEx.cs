using System;
using System.Runtime.InteropServices;

namespace RoseByte.SharpFiles.CopyFileEx
{
    public class FileEx
    {
        private long _processed;
        private long _size;
        private readonly Action<long, long, long> _progress;

        public FileEx(Action<long, long, long> progress)
        {
            _progress = progress;
        }
        
        private CopyProgressRoutine GetRoutine()
        {
            return CopyProgressRoutineImpl;
        }
        
        public void Copy(string source, string dest, long size, ref int cancel)
        {
            var success = CopyFileEx(
                source, dest, CopyProgressRoutineImpl, IntPtr.Zero, ref cancel, CopyFileFlags.COPY_FILE_RESTARTABLE);
            
            if (!success)
            {
                throw new Exception($"File '{source}' could not be copied to '{dest}'");
            }

            if (_processed != size)
            {
                _progress(size - _processed, size, size);
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
            var current = totalBytesTransferred - _processed;

            if (current > 512 && 100 * current / totalFileSize >= 1)
            {
                _progress?.Invoke(totalBytesTransferred - _processed, totalBytesTransferred, totalFileSize);
                _processed = totalBytesTransferred;
            }
            
            return CopyProgressResult.PROGRESS_CONTINUE;
        }
        
        #region DLL Import
    
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, 
            CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel, CopyFileFlags dwCopyFlags);
    
        delegate CopyProgressResult CopyProgressRoutine(
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