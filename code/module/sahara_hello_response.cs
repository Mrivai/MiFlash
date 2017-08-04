

using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct sahara_hello_response
  {
    public uint Command;
    public uint Length;
    public uint Version;
    public uint Version_min;
    public uint Status;
    public uint Mode;
    public uint[] Reserved;
  }
}
