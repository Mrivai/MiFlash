
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using XiaoMiFlash.code.data;

namespace XiaoMiFlash.code.Utility
{
  public class Comm
  {
    public bool ignoreResponse = true;
    public int MAX_SECTOR_STR_LEN = 20;
    public int SECTOR_SIZE_UFS = 4096;
    public int SECTOR_SIZE_EMMC = 512;
    public bool isDump;
    public SerialPort serialPort;
    public Thread thread;
    private volatile bool _keepReading;
    public byte[] recData;
    private long received_count;
    public int m_dwBufferSectors;
    public int intSectorSize;

    public bool IsOpen
    {
      get
      {
        int num = 10;
        while (num-- > 0 && !serialPort.IsOpen)
        {
          Log.w(serialPort.PortName, "wait for port open.");
          Thread.Sleep(1000);
        }
        return serialPort.IsOpen;
      }
    }

    public Comm()
    {
      serialPort = new SerialPort();
      thread = null;
      _keepReading = false;
    }

    public void StartReading()
    {
      if (_keepReading)
        return;
      _keepReading = true;
      thread = new Thread(new ThreadStart(ReadPort));
      thread.Start();
    }

    public void StopReading()
    {
      if (!_keepReading)
        return;
      _keepReading = false;
      thread.Join();
      thread = null;
    }

    private void ReadPort()
    {
      while (_keepReading)
      {
        if (serialPort.IsOpen)
        {
          int bytesToRead = serialPort.BytesToRead;
          if (bytesToRead > 0)
          {
            byte[] buffer = new byte[bytesToRead];
            try
            {
              serialPort.Read(buffer, 0, bytesToRead);
            }
            catch (TimeoutException ex)
            {
              Log.w(serialPort.PortName, ex, false);
            }
          }
        }
      }
    }

    public byte[] ReadPortData()
    {
      byte[] buffer = null;
      if (serialPort.IsOpen)
      {
        int bytesToRead = serialPort.BytesToRead;
        if (bytesToRead > 0)
        {
          buffer = new byte[bytesToRead];
          try
          {
            serialPort.Read(buffer, 0, bytesToRead);
          }
          catch (TimeoutException ex)
          {
            Log.w(serialPort.PortName, ex, false);
          }
        }
      }
      return buffer;
    }

    public byte[] ReadPortData(int offset, int count)
    {
      byte[] buffer = new byte[count];
      try
      {
        serialPort.Read(buffer, offset, count);
      }
      catch (TimeoutException ex)
      {
        Log.w(serialPort.PortName, ex, false);
      }
      return buffer;
    }

    public void Open()
    {
      Close();
      serialPort.Open();
      if (serialPort.IsOpen)
        return;
      string str = "open serial port failed!";
      Log.w(serialPort.PortName, str);
      FlashingDevice.UpdateDeviceStatus(serialPort.PortName, new float?(), str, "error", true);
    }

    private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      int bytesToRead = serialPort.BytesToRead;
      recData = new byte[bytesToRead];
      received_count += bytesToRead;
      serialPort.Read(recData, 0, bytesToRead);
    }

    public void Close()
    {
      StopReading();
      serialPort.Close();
    }

    public void WritePort(byte[] send, int offSet, int count)
    {
      if (!IsOpen)
        return;
      int num = 100;
      Exception ex1 = new TimeoutException();
      bool flag = false;
      while (num-- > 0 && ex1 != null)
      {
        if (ex1.GetType() == typeof (TimeoutException))
        {
          try
          {
            serialPort.WriteTimeout = 1000;
            serialPort.Write(send, offSet, count);
            flag = true;
            if (isDump)
            Dump(send);
            ex1 = null;
          }
          catch (TimeoutException ex2)
          {
            ex1 = ex2;
            Log.w(serialPort.PortName, "write time out try agian " + (100 - num));
            Thread.Sleep(500);
          }
          catch (Exception ex2)
          {
            Log.w(serialPort.PortName, "write failed:" + ex2.Message);
          }
        }
        else
          break;
      }
      if (flag)
        return;
      Log.w(serialPort.PortName, ex1, true);
    }

    public bool SendCommand(string command)
    {
      return SendCommand(command, false);
    }

    public bool SendCommand(string command, bool checkAck)
    {
      byte[] bytes = Encoding.Default.GetBytes(command);
      if (isDump || checkAck)
        Log.w(serialPort.PortName, "send command:" + command);
      WritePort(bytes, 0, bytes.Length);
      if (checkAck)
        return GetResponse(checkAck);
      return false;
    }

    public byte[] getRecData()
    {
      byte[] binary = ReadDataFromPort();
      if (binary == null)
        throw new Exception("can not read from port " + serialPort.PortName);
      if (binary.Length > 0 && isDump)
      {
        Log.w(serialPort.PortName, "read from port:");
        Dump(binary);
      }
      return binary;
    }

    private byte[] ReadDataFromPort()
    {
      int num = 10;
      recData = null;
      for (recData = ReadPortData(); num-- > 0 && recData == null; recData = ReadPortData())
        Thread.Sleep(500);
      return recData;
    }

    private bool WaitForAck()
    {
      bool flag = false;
      int num = 10;
      while (num-- > 0 && !flag)
      {
        string[] strArray = Dump(ReadDataFromPort());
        flag = strArray.Length == 2 && strArray[1].IndexOf("<response value=\"ACK\" />") >= 0;
        Thread.Sleep(500);
      }
      return flag;
    }

    public bool GetResponse(bool waiteACK)
    {
      bool flag = false;
      Log.w(serialPort.PortName, "get response from target");
      if (!waiteACK)
        return ReadDataFromPort() != null;
      int num = 2;
      if (waiteACK)
        num = 10;
      while (num-- > 0 && !flag)
      {
        List<XmlDocument> responseXml = GetResponseXml(waiteACK);
        int count = responseXml.Count;
        foreach (XmlNode xmlNode in responseXml)
        {
          foreach (XmlNode childNode in xmlNode.SelectSingleNode("data").ChildNodes)
          {
            foreach (XmlAttribute attribute in childNode.Attributes)
            {
              if (attribute.Name.ToLower() == "maxpayloadsizetotargetinbytes")
                m_dwBufferSectors = Convert.ToInt32(attribute.Value) / intSectorSize;
              if (attribute.Value.ToLower() == "ack")
                flag = true;
            }
          }
        }
        if (waiteACK)
          Thread.Sleep(500);
      }
      return flag;
    }

    private List<XmlDocument> GetResponseXml()
    {
      return GetResponseXml(false);
    }

    private List<XmlDocument> GetResponseXml(bool waiteACK)
    {
      List<XmlDocument> xmlDocumentList = new List<XmlDocument>();
      string[] strArray = Dump(ReadDataFromPort(), waiteACK);
      if (strArray.Length >= 2)
      {
        foreach (string str in ((IEnumerable<string>) Regex.Split(strArray[1], "\\<\\?xml")).ToList())
        {
          if (!string.IsNullOrEmpty(str))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<?xml " + str);
            xmlDocumentList.Add(xmlDocument);
          }
        }
      }
      return xmlDocumentList;
    }

    private string GetResponseXmlStr()
    {
      return Dump(ReadDataFromPort())[1];
    }

    private string[] Dump(byte[] binary)
    {
      return Dump(binary, false);
    }

    private string[] Dump(byte[] binary, bool waitACK)
    {
      Log.w(serialPort.PortName, "dump:");
      if (binary == null)
      {
        Log.w(serialPort.PortName, "no Binary dump");
        return new string[2]{ "", "" };
      }
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      StringBuilder stringBuilder3 = new StringBuilder();
      StringBuilder stringBuilder4 = new StringBuilder();
      int num = 0;
      while (num < binary.Length)
      {
        for (int index = 0; index < 16; ++index)
        {
          if (num + index < binary.Length)
            stringBuilder4.Append(Convert.ToChar(binary[num + index]).ToString());
          else
            stringBuilder4.Append(" ");
        }
        stringBuilder2.Append(stringBuilder4);
        stringBuilder3.Length = 0;
        stringBuilder4.Length = 0;
        num += 16;
      }
      if (isDump || waitACK)
        Log.w(serialPort.PortName, stringBuilder2.ToString() + "\r\n\r\n");
      return new string[2]
      {
        stringBuilder1.ToString(),
        stringBuilder2.ToString()
      };
    }
  }
}
