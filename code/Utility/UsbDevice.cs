

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using XiaoMiFlash.code.bl;
using XiaoMiFlash.code.module;

namespace XiaoMiFlash.code.Utility
{
  public class UsbDevice
  {
    public static List<Device> GetDevice()
    {
      List<Device> deviceList = new List<Device>();
      foreach (string str in ComPortCtrl.getDevicesQc())
        deviceList.Add(new Device()
        {
          Name = str,
          DeviceCtrl = new SerialPortDevice()
        });
      foreach (string str in GetScriptDevice())
        deviceList.Add(new Device()
        {
          Name = str,
          DeviceCtrl = new ScriptDevice()
        });
      return deviceList;
    }

    public static string[] GetScriptDevice()
    {
      List<string> stringList = new List<string>();
      string fastboot = Script.fastboot;
      Log.w("fastboot path: " + fastboot);
      if (!File.Exists(fastboot))
        throw new Exception("no fastboot.");
      string[] strArray = Regex.Split(new Cmd("").Execute(null, fastboot + " devices"), "\r\n");
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!string.IsNullOrEmpty(strArray[index]))
          stringList.Add(Regex.Split(strArray[index], "\t")[0]);
      }
      return stringList.ToArray();
    }
  }
}
