

using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct FlashType
  {
    public static string CleanAll = "flash_all.bat";
    public static string SaveUserData = "flash_all_except_storage.bat";
    public static string CleanAllAndLock = "flash_all_lock.bat";
  }
}
