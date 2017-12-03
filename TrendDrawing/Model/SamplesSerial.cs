using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrendDrawing.Model
{
    class SamplesSerial : ISamples
    {
        public List<double> Samples { get; set; }
        public string SourcePath { get; set; }

        public event EventHandler<List<double>> DataReceived;

        private int _baudRate;
        private Parity _parity;
        private StopBits _stopBits;
        private int _dataBits;
        private string _portName = string.Empty;
        private SerialPort _port;

        public SamplesSerial(int baudRate, Parity parity, StopBits stopBits, int dataBits, string portName)
        {
            _baudRate = baudRate;
            _parity = parity;
            _stopBits = stopBits;
            _dataBits = dataBits;
            _portName = portName;
        }

        public void Read()
        {
            Task.Run(() =>
            {
                using (_port = new SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits))
                {
                    try
                    {
                        while (true)
                        {
                            byte[] buffer = new byte[512];
                            if(!_port.IsOpen)
                                    _port.Open();
                            int actualLenght = _port.BaseStream.Read(buffer, 0, buffer.Length);
                            var received = new byte[actualLenght];
                            Buffer.BlockCopy(buffer, 0, received, 0, actualLenght);
                            var converted = ConvertBytesToDoubles(received);
                            if(converted.Count > 0)
                                DataReceived(this, converted);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            });

        }

        public async Task ReadAsync()
        {
          using(_port = new SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits))
            {
                try
                {
                    byte[] buffer = new byte[512];
                    if (!_port.IsOpen)
                        _port.Open();
                    int actualLenght = await _port.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                    var received = new byte[actualLenght];
                    Buffer.BlockCopy(buffer, 0, received, 0, actualLenght);
                    var converted = ConvertBytesToDoubles(received);
                    if(converted.Count > 0)
                        DataReceived(this, converted);
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return;
                }
            }
                await ReadAsync();
        }

        private List<double> ConvertBytesToDoubles(byte[] received)
        {
            if (received != null)
            {
                if (received.Length > 0)
                {
                    var fullString = Encoding.UTF8.GetString(received);
                    var splitString = fullString.Split(',');
                    double conversionResult;
                    List<double> convertedValues = new List<double>();
                    foreach(var item in splitString)
                    {
                        var success = Double.TryParse(item, out conversionResult);
                        if(success)
                            convertedValues.Add(conversionResult);
                    }
                    return convertedValues;
                }
            }
            return new List<double>();
        }
    }
}
