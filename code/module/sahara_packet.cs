﻿
using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct sahara_packet
  {
    public uint Command;
    public uint Length;
  }
}
