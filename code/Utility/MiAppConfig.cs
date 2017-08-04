

using System.Configuration;
using System.Windows.Forms;
using System.Xml;

namespace XiaoMiFlash.code.Utility
{
  public class MiAppConfig
  {
    public static void Add(string key, string value)
    {
      Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      configuration.AppSettings.Settings.Add(key, value);
      configuration.Save();
    }

    public static void SetValue(string AppKey, string AppValue)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(Application.ExecutablePath + ".config");
      XmlNode xmlNode = xmlDocument.SelectSingleNode("//appSettings");
      XmlElement xmlElement = (XmlElement) xmlNode.SelectSingleNode("//add[@key='" + AppKey + "']");
      if (xmlElement != null)
      {
        xmlElement.SetAttribute("value", AppValue);
      }
      else
      {
        XmlElement element = xmlDocument.CreateElement("add");
        element.SetAttribute("key", AppKey);
        element.SetAttribute("value", AppValue);
        xmlNode.AppendChild(element);
      }
      xmlDocument.Save(Application.ExecutablePath + ".config");
      ConfigurationManager.RefreshSection(AppKey);
    }

    public static string GetAppConfig(string appKey)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(Application.ExecutablePath + ".config");
      XmlElement xmlElement = (XmlElement) xmlDocument.SelectSingleNode("//appSettings").SelectSingleNode("//add[@key='" + appKey + "']");
      if (xmlElement != null)
        return xmlElement.Attributes["value"].Value;
      return string.Empty;
    }

    public static string Get(string key)
    {
      if (ConfigurationManager.AppSettings[key] == null)
      Add(key, "");
      return ConfigurationManager.AppSettings[key].ToString();
    }
  }
}
