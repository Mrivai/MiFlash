

using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.lan
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct LanguageType
  {
    public static string chn_s = "CHN_S";
    public static string chn_t = "CHN_T";
    public static string eng = "ENG";
  }
}
