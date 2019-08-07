
//
// class untuk mendekteksi perangkat atau handphone 
// dalam mode EDl atau Fastboot
//
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
        
        // fungsi untuk mendekteksi hp
        // dalam keadaan edl
        // melalui ComPortCtrl class
       public static List<Device> GetDevice()
       {
            //list devices
          List<Device> deviceList = new List<Device>();
            //check devices in edl mode
          foreach (string str in ComPortCtrl.getDevicesQc())
            //save device
            deviceList.Add(new Device()
            {
              Name = str,
              DeviceCtrl = new SerialPortDevice()
            });
          //chek device in fastboot mode
          foreach (string str in GetScriptDevice())
            deviceList.Add(new Device()
            {
              Name = str,
              DeviceCtrl = new ScriptDevice()
            });
          return deviceList;
        }
        // fungsi untuk mendekteksi hp
        // dalam keadaan fastboot
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
