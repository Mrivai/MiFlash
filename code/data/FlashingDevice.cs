

using System.Collections.Generic;
using XiaoMiFlash.code.module;

namespace XiaoMiFlash.code.data
{
  public static class FlashingDevice
  {
    public static List<Device> flashDeviceList = new List<Device>();

    public static void UpdateDeviceStatus(string deviceName, float? progress, string status, string result, bool isDone)
    {
      foreach (Device flashDevice in flashDeviceList)
      {
        if (flashDevice.Name == deviceName)
        {
          if (progress.HasValue)
            flashDevice.Progress = progress.Value;
          if (!string.IsNullOrEmpty(status))
            flashDevice.Status = status;
          if (!string.IsNullOrEmpty(result))
            flashDevice.Result = result;
          flashDevice.IsDone = new bool?(isDone);
        }
      }
    }
  }
}
