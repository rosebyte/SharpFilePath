using System;
using System.Runtime.InteropServices;

namespace RoseByte.SharpFiles.CopyFileEx
{
    public class FileEx
    {
        private int _processed;
        private readonly Action<int> _progress;

        public FileEx(Action<int> progress)
        {
            _progress = progress;
        }
        
        public void Copy(string source, string dest, ref int cancel)
        {
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

            if (_processed != 100)
            {
                _progress(100);
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
            var current = 100 * totalBytesTransferred / totalFileSize;
            
            if (current > _processed)
            {
                _processed = (int)current;
                _progress?.Invoke(_processed);
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