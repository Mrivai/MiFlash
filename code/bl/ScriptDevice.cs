

using System;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl
{
  public class ScriptDevice : DeviceCtrl
  {
    public override void flash()
    {
      try
      {
        string fastboot = Script.fastboot;
        string[] strArray = FileSearcher.SearchFiles(swPath, flashScript);
        if (strArray.Length == 0)
          throw new Exception("can not found file " + flashScript);
        string str = strArray[0];
        string command = string.Format("pushd \"{0}\"&&prompt $$&&set PATH=\"{1}\";%PATH%&&\"{2}\" -s {3}&&popd", swPath, Script.AndroidPath, str, deviceName);
        Log.w(deviceName, "image path:" + swPath);
        Log.w(deviceName, "env android path:" + Script.AndroidPath);
        Log.w(deviceName, "script :" + str);
        new Cmd(deviceName).Execute_returnLine(deviceName, command, 1);
      }
      catch (Exception ex)
      {
        FlashingDevice.UpdateDeviceStatus(deviceName, new float?(), ex.Message, "eror", true);
        Log.w(deviceName, ex, true);
      }
    }

    public override string[] getDevice()
    {
      return UsbDevice.GetScriptDevice();
    }
  }
}
