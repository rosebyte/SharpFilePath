using System;
using System.Runtime.InteropServices;

namespace RoseByte.SharpFiles.CopyFileEx
{
    public class FileEx
    {
        private long _processed;
        private readonly Action<long, long, long> _progress;

        public FileEx(Action<long, long, long> progress)
        {
            _progress = progress;
        }
        
        private CopyProgressRoutine GetRoutine()
        {
            return CopyProgressRoutineImpl;
        }
        
        public void Copy(string source, string dest, ref int cancel)
        {
            CopyFileEx(source, dest, CopyProgressRoutineImpl, IntPtr.Zero, ref cancel, CopyFileFlags.COPY_FILE_RESTARTABLE);
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
            _progress?.Invoke(totalBytesTransferred - _processed, totalBytesTransferred, totalFileSize);
            _processed = totalBytesTransferred;
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