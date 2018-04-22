using System;
using System.Runtime.InteropServices;

namespace SharpFilePath.CopyFileEx
{
    public static class FileEx
    {
        private static CopyProgressRoutine GetRoutine(Action<long, long> progress)
        {
            return (total, transferred, a, b, c, d, e, f, g) =>
            {
                progress?.Invoke(transferred, total);
                return CopyProgressResult.PROGRESS_CONTINUE;
            };;
        }
        
        public static void Copy(string source, string dest, ref int cancel, Action<long, long> progress)
        {
            CopyFileEx(source, dest, GetRoutine(progress), IntPtr.Zero, ref cancel, CopyFileFlags.COPY_FILE_RESTARTABLE);
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