

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace XiaoMiFlash.code.Utility
{
  public class Driver
  {
    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupCopyOEMInf(string SourceInfFileName, string OEMSourceMediaLocation, OemSourceMediaType OEMSourceMediaType, OemCopyStyle CopyStyle, StringBuilder DestinationInfFileName, int DestinationInfFileNameSize, int RequiredSize, out string DestinationInfFileNameComponent);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupUninstallOEMInf(string InfFileName, SetupUOInfFlags Flags, IntPtr Reserved);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetWindowsDirectory(StringBuilder path, int pathLen);

    public static string SetupOEMInf(string infPath, out string destinationInfFileName, out string destinationInfFileNameComponent, out bool success)
    {
      string str = "";
      StringBuilder DestinationInfFileName = new StringBuilder(260);
      success = SetupCopyOEMInf(infPath, "", OemSourceMediaType.SPOST_PATH, OemCopyStyle.SP_COPY_NEWER, DestinationInfFileName, DestinationInfFileName.Capacity, 0, out destinationInfFileNameComponent);
      if (!success)
        str = new Win32Exception(Marshal.GetLastWin32Error()).Message;
      destinationInfFileName = DestinationInfFileName.ToString();
      return str;
    }

    public static string UninstallInf(string infFileName, out bool success)
    {
      string str = "";
      success = SetupUninstallOEMInf(infFileName, SetupUOInfFlags.SUOI_FORCEDELETE, IntPtr.Zero);
      if (!success)
        str = new Win32Exception(Marshal.GetLastWin32Error()).Message;
      return str;
    }

    public static string UninstallInfByText(string text, out bool success)
    {
      success = false;
      StringBuilder path1 = new StringBuilder(256);
      if ((int)GetWindowsDirectory(path1, path1.Capacity) == 0)
        return "UninstallInfByText: GetWindowsDirectory failed with system error code " + Marshal.GetLastWin32Error().ToString();
      string[] files = Directory.GetFiles(path1.ToString() + "\\inf", "*.inf");
      string str = "";
      foreach (string path2 in files)
      {
        if (File.ReadAllText(path2).Contains(text))
        {
          string InfFileName = path2.Remove(0, path2.LastIndexOf('\\') + 1);
          if (!SetupUninstallOEMInf(InfFileName, SetupUOInfFlags.SUOI_FORCEDELETE, IntPtr.Zero))
            str = str + "UninstallInfByText: SetupUninstallOEMInf failed with code " + Marshal.GetLastWin32Error().ToString() + " for file " + InfFileName;
          else
            success = true;
        }
      }
      if (str.Length > 0)
        return str;
      return null;
    }
  }
}
