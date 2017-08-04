

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace XiaoMiFlash.code.bl
{
  public class ImageValidation
  {
    public static string Validate(string path)
    {
      bool flag = true;
      string str1 = "";
      string[] strArray = new string[3]
      {
        "system.img",
        "userdata.img",
        "cust.img"
      };
      Hashtable hashtable1 = new Hashtable();
      DirectoryInfo directoryInfo = new DirectoryInfo(path);
      foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
      {
        if (directory.Name.ToLower() == "images")
        {
          directoryInfo = directory;
          break;
        }
      }
      string fullName = directoryInfo.FullName;
      foreach (string str2 in ((IEnumerable<string>) strArray).ToList())
      {
        string fileName = directoryInfo.FullName + string.Format("\\{0}", str2);
        hashtable1[str2] = Utility.Utility.GetMD5HashFromFile(fileName);
      }
      Hashtable hashtable2 = new Hashtable();
      string str3 = directoryInfo.FullName + "\\md5sum.xml";
      if (File.Exists(str3))
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlReader reader = XmlReader.Create(str3, new XmlReaderSettings()
        {
          IgnoreComments = true
        });
        xmlDocument.Load(reader);
        foreach (XmlElement childNode in xmlDocument.SelectSingleNode("data").ChildNodes)
        {
          foreach (XmlAttribute attribute in (XmlNamedNodeMap) childNode.Attributes)
          {
            if (attribute.Name.ToLower() == "name")
            {
              foreach (string str2 in ((IEnumerable<string>) strArray).ToList())
              {
                if (attribute.Value.ToLower() == str2)
                {
                  flag &= hashtable1[str2].ToString() == childNode.Value.ToString();
                  if (!flag)
                    str1 = string.Format("{0} md5 validate failed!", str2);
                }
              }
            }
          }
          if (!flag)
            break;
        }
      }
      return str1;
    }
  }
}
