// Decompiled with JetBrains decompiler
// Type: XiaoMiFlash.Properties.Settings
// Assembly: XiaoMiFlash, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BC1A28B0-F0EB-4D8A-87A7-1C7E69B878B5
// Assembly location: C:\XiaoMi\XiaoMiFlash\XiaoMiFlash.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace XiaoMiFlash.Properties
{
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
  [CompilerGenerated]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        return Settings.defaultInstance;
      }
    }

    [DefaultSettingValue("")]
    [UserScopedSetting]
    [DebuggerNonUserCode]
    public string txtPath
    {
      get
      {
        return (string) this["txtPath"];
      }
      set
      {
        this["txtPath"] = (object) value;
      }
    }
  }
}
