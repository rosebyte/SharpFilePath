using System;

namespace RoseByte.SharpFiles.CopyFileEx
{
    [Flags]
    enum CopyFileFlags : uint
    {
        COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
        COPY_FILE_RESTARTABLE = 0x00000002,
        COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
        COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
    }
}