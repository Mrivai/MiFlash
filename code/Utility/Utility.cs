

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XiaoMiFlash.code.Utility
{
  public class Utility
  {
    public static string GetMD5HashFromFile(string fileName)
    {
      try
      {
        FileStream fileStream = new FileStream(fileName, FileMode.Open);
        byte[] hash = new MD5CryptoServiceProvider().ComputeHash(fileStream);
        fileStream.Close();
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < hash.Length; ++index)
          stringBuilder.Append(hash[index].ToString("x2"));
        return stringBuilder.ToString();
      }
      catch (Exception ex)
      {
        throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
      }
    }
  }
}
