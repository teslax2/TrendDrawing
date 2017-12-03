using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendDrawing.Model
{
    class SamplesCSV : ISamples
    {
        public List<double> Samples { get; set; }
        public string SourcePath { get; set; }
        public char Separator { get; set; }
        public int ColumnToRead { get; set; }
        public event EventHandler<List<double>> DataReceived;

        public SamplesCSV(string sourcePath)
        {
            Samples = new List<double>();
            Separator = ',';
            ColumnToRead = 0;
            SourcePath = sourcePath;
        }

        public SamplesCSV()
        {
            Samples = new List<double>();
            Separator = ',';
            ColumnToRead = 0;
        }

        public void Read()
        {
            using (var reader = new StreamReader(SourcePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(Separator);

                    if (values[ColumnToRead] != null)
                    {
                        double sample;
                        if (double.TryParse(values[ColumnToRead], out sample))
                            Samples.Add(sample);
                    }
                }
            }

            if (Samples != null)
            {
                if (Samples.Count > 0)
                    OnDataReceived(Samples);
            }
        }

        public async Task ReadAsync()
        {
            using(var reader = new StreamReader(SourcePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(Separator);

                    if (values[ColumnToRead] != null)
                    {
                        double sample;
                        if (double.TryParse(values[ColumnToRead], out sample))
                            Samples.Add(sample);
                    }
                }
            }

            if (Samples != null)
            {
                if (Samples.Count > 0)
                    OnDataReceived(Samples);
            }
        }

        private void OnDataReceived(List<double> samples)
        {
            EventHandler<List<double>> dataReceived = DataReceived;
            if (dataReceived != null)
                dataReceived(this, samples);

        }
    }
}
