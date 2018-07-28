using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using RoseByte.SharpFiles.Internal.Handle;

public static class FileUtil
{
    const int RmRebootReasonNone = 0;
    
    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    static extern int RmRegisterResources(uint pSessionHandle, UInt32 nFiles, string[] rgsFilenames, 
        UInt32 nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, UInt32 nServices, string[] rgsServiceNames);

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
    static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

    [DllImport("rstrtmgr.dll")]
    static extern int RmEndSession(uint pSessionHandle);

    [DllImport("rstrtmgr.dll")]
    static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo, 
        [In, Out] RM_PROCESS_INFO[] rgAffectedApps, ref uint lpdwRebootReasons);

    /// <summary>
    /// Find out what process(es) have a lock on the specified file.
    /// </summary>
    /// <param name="path">Path of the file.</param>
    /// <returns>Processes locking the file</returns>
    /// <remarks>See also:
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
    /// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
    /// 
    /// </remarks>
    static public List<Process> WhoIsLocking(string path)
    {
        uint handle;
        string key = Guid.NewGuid().ToString();
        List<Process> processes = new List<Process>();

        int res = RmStartSession(out handle, 0, key);
        if (res != 0) throw new Exception("Could not begin restart session.  Unable to determine file locker.");

        try
        {
            const int ERROR_MORE_DATA = 234;
            uint pnProcInfoNeeded = 0,
                 pnProcInfo = 0,
                 lpdwRebootReasons = RmRebootReasonNone;

            string[] resources = new string[] { path }; // Just checking on one resource.

            res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

            if (res != 0) throw new Exception("Could not register resource.");                                    

            //Note: there's a race condition here -- the first call to RmGetList() returns
            //      the total number of process. However, when we call RmGetList() again to get
            //      the actual processes this number may have increased.
            res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

            if (res == ERROR_MORE_DATA)
            {
                // Create an array to store the process results
                RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                pnProcInfo = pnProcInfoNeeded;

                // Get the list
                res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
                if (res == 0)
                {
                    processes = new List<Process>((int)pnProcInfo);

                    // Enumerate all of the results and add them to the 
                    // list to be returned
                    for (int i = 0; i < pnProcInfo; i++)
                    {
                        try
                        {
                            processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
                        }
                        // catch the error -- in case the process is no longer running
                        catch (ArgumentException) { }
                    }
                }
                else throw new Exception("Could not list processes locking resource.");                    
            }
            else if (res != 0) throw new Exception("Could not list processes locking resource. Failed to get size of result.");                    
        }
        finally
        {
            RmEndSession(handle);
        }

        return processes;
    }
}