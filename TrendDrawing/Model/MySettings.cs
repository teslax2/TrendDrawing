using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Xml.Serialization;

namespace TrendDrawing.Model
{
    [Serializable]
    public class MySettings
    {
        public Languages Language { get; set; }
        public Size WindowSize { get; set; } 
        public int PortBaudRate { get; set; }
        public Parity PortParity { get; set; }
        public StopBits PortStopBits { get; set; }
        public int PortDataBits { get; set; }
        public string PortName { get; set; }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(MySettings));
                xmls.Serialize(sw, this);
            }
        }
        public MySettings Read(string filename)
        {
            using (StreamReader sw = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(MySettings));
                return xmls.Deserialize(sw) as MySettings;
            }
        }
    }

    public enum Languages
    {
        English,
        Polish,
    }
}
