

using System;
using System.Xml;

namespace XiaoMiFlash.code.lan
{
  public class LanguageProvider
  {
    private string languageType = "";

    public LanguageProvider(string lanType)
    {
      this.languageType = lanType;
    }

    public string GetLanguage(string ctrlID)
    {
      XmlDocument xmlDocument = new XmlDocument();
      new XmlReaderSettings().IgnoreComments = true;
      XmlReader reader = XmlReader.Create(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\LanguageLibrary.xml");
      xmlDocument.Load(reader);
      XmlNodeList childNodes = xmlDocument.SelectSingleNode("LanguageLibrary").ChildNodes;
      string str = "";
      foreach (XmlElement xmlElement in childNodes)
      {
        if (!(xmlElement.Name.ToLower() != "lan") && xmlElement.Attributes["CTRLID"].Value == ctrlID)
        {
          str = xmlElement.Attributes[languageType].Value;
          break;
        }
      }
      return str;
    }
  }
}
