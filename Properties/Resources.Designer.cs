// Decompiled with JetBrains decompiler
// Type: XiaoMiFlash.Properties.Resources
// Assembly: XiaoMiFlash, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BC1A28B0-F0EB-4D8A-87A7-1C7E69B878B5
// Assembly location: C:\XiaoMi\XiaoMiFlash\XiaoMiFlash.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace XiaoMiFlash.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) XiaoMiFlash.Properties.Resources.resourceMan, (object) null))
          XiaoMiFlash.Properties.Resources.resourceMan = new ResourceManager("XiaoMiFlash.Properties.Resources", typeof (XiaoMiFlash.Properties.Resources).Assembly);
        return XiaoMiFlash.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return XiaoMiFlash.Properties.Resources.resourceCulture;
      }
      set
      {
        XiaoMiFlash.Properties.Resources.resourceCulture = value;
      }
    }

    internal static string txtPath
    {
      get
      {
        return XiaoMiFlash.Properties.Resources.ResourceManager.GetString("txtPath", XiaoMiFlash.Properties.Resources.resourceCulture);
      }
    }

    internal Resources()
    {
    }
  }
}
