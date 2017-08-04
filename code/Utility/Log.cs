

using System;
using System.IO;
using System.Text;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;

namespace XiaoMiFlash.code.Utility
{
  public class Log
  {
    public static void w(string deviceName, Exception ex, bool stopFlash)
    {
      w(deviceName, ex.Message);
      w(deviceName, ex.StackTrace);
      FlashingDevice.UpdateDeviceStatus(deviceName, new float?(), ex.Message, "error", stopFlash);
    }

    public static void w(string deviceName, string msg)
    {
      string str = "";
      if (FlashingDevice.flashDeviceList.Count <= 0)
        return;
      foreach (Device flashDevice in FlashingDevice.flashDeviceList)
      {
        if (flashDevice.Name == deviceName)
        {
          str = string.Format("{0}@{1}.txt", flashDevice.Name, flashDevice.StartTime.ToString("yyyyMdHms"));
          break;
        }
      }
      string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + str;
      if (!File.Exists(path))
        File.Create(path).Close();
      StreamWriter streamWriter = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.Default);
      streamWriter.WriteLine(string.Format("[{0}  {1}]:{2}", DateTime.Now.ToLongTimeString(), deviceName, msg));
      streamWriter.Close();
      if (msg.ToLower().IndexOf("error") < 0)
        return;
      FlashingDevice.UpdateDeviceStatus(deviceName, new float?(), msg, "error", true);
    }

    public static void w(string msg)
    {
      string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + string.Format("{0}@{1}.txt", "mifalsh", DateTime.Now.ToString("yyyyMd"));
      if (!File.Exists(path))
        File.Create(path).Close();
      StreamWriter streamWriter = new StreamWriter( new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.Default);
      streamWriter.WriteLine(string.Format("[{0}]:{1}", DateTime.Now.ToLongTimeString(), msg));
      streamWriter.Close();
    }

    public static void Installw(string installationPath, string msg)
    {
      string str = string.Format("{0}@{1}.txt", "mifalsh", DateTime.Now.ToString("yyyyMd"));
      string path = installationPath + "log\\" + str;
      if (!File.Exists(path))
        File.Create(path).Close();
      StreamWriter streamWriter = new StreamWriter( new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.Default);
      streamWriter.WriteLine(string.Format("[{0}]:{1}", DateTime.Now.ToLongTimeString(), msg));
      streamWriter.Close();
    }
  }
}
