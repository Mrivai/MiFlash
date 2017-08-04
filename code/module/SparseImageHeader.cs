﻿

using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct SparseImageHeader
  {
    public uint uMagic;
    public ushort uMajorVersion;
    public ushort uMinorVersion;
    public ushort uFileHeaderSize;
    public ushort uChunkHeaderSize;
    public uint uBlockSize;
    public uint uTotalBlocks;
    public uint uTotalChunks;
    public uint uImageChecksum;
  }
}
