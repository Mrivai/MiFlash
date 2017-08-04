

using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct SparseChunkHeader
  {
    public ushort uChunkType;
    public ushort uReserved1;
    public uint uChunkSize;
    public uint uTotalSize;
  }
}
