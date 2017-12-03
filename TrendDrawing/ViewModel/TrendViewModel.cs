using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Threading;
using TrendDrawing.Model;

namespace TrendDrawing.ViewModel
{
    class TrendViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public SeriesCollection SamplesCollection { get; set; }
        private List<string> _labelsCollection;
        public List<string> LabelsCollection { get { return _labelsCollection; } set { _labelsCollection = value; OnPropertyChanged("LabelsCollection"); } }
        private ISamples _dataFromFile;
        private SamplesSerial _dataFromSerial;
        public string FileName { get; set; }
        private DispatcherTimer _oneSecondTimer;
        private ScreenCapturer _capture;
        private ExcelExporter _export;
        private CancellationTokenSource _cancelTask;
        private const string _settingsPath = "dupa.xml";
        private MySettings _settings;
        public Languages Language { get { return _settings.Language; } }

        private void OnPropertyChanged(string property)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public TrendViewModel()
        {
            _settings = new MySettings()
            {
                Language = Languages.English,
                PortBaudRate = 9600,
                PortParity = System.IO.Ports.Parity.None,
                PortStopBits = System.IO.Ports.StopBits.One,
                PortDataBits = 8,
                PortName = "COM3",
            };
            ReadSettings();

            _dataFromFile = new SamplesCSV();
            _dataFromFile.DataReceived += _data_DataReceived;

            _dataFromSerial = new SamplesSerial(_settings.PortBaudRate, _settings.PortParity, _settings.PortStopBits, _settings.PortDataBits, _settings.PortName);
            _dataFromSerial.DataReceived += _data_DataReceived;

            InitializeChart();

            _oneSecondTimer = new DispatcherTimer();
            _oneSecondTimer.Interval = TimeSpan.FromSeconds(1);
            _oneSecondTimer.Tick += _oneSecondTimer_Tick;

            _capture = new ScreenCapturer();
            _export = new ExcelExporter();

        }

        /// <summary>
        /// If chart is running and no data received, last value is copied
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _oneSecondTimer_Tick(object sender, EventArgs e)
        {
            if (SamplesCollection[0].Values. Count > 0)
                SamplesCollection[0].Values.Add(SamplesCollection[0].Values[SamplesCollection[0].Values.Count-1]);
            else
                SamplesCollection[0].Values.Add(0.0);
            LabelsCollection.Add(DateTime.Now.ToString());
        }

        private void InitializeChart()
        {
            SamplesCollection = new SeriesCollection
            { new LineSeries
                { 
                  Values = new ChartValues<double>(),
                  PointGeometrySize = 3,
                  LineSmoothness = 0,
                }
            };
            _labelsCollection = new List<string>();
        }


        /// <summary>
        /// Main point of collection data, all class from ISamples send dataReceived event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _data_DataReceived(object sender, List<double> e)
        {
            _oneSecondTimer.Stop();

            var newSamples = new ObservableCollection<double>(e);
            foreach (var item in newSamples)
            {
                SamplesCollection[0].Values.Add(item);
                LabelsCollection.Add(DateTime.Now.ToString());
            }

            _oneSecondTimer.Start();
        }

        /// <summary>
        /// Open CSV file 
        /// </summary>
        /// <returns></returns>
        public async Task Open()
        {
            _dataFromFile.SourcePath = FileName;
            await _dataFromFile.ReadAsync();
        }

        /// <summary>
        /// Run serial data collection asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task RunSerialAsync()
        {
            await _dataFromSerial.ReadAsync();
        }
        /// <summary>
        /// Run serial in new task
        /// </summary>
        public void RunSerial()
        {
            _dataFromSerial.Read();
        }

        /// <summary>
        /// Caputre chart to bitmap
        /// </summary>
        public void CaptureBitmap()
        {
            var image = _capture.Capture();

            using (image)
            {
                image.Save(string.Format("dupa_{0}.png", DateTime.Now.ToString("yyyyMMddHHmmss"), System.Drawing.Imaging.ImageFormat.Png));
            }
        }

        /// <summary>
        /// Export data to excel
        /// </summary>
        public void ExcelExport()
        {
            var dataToExport = new Dictionary<string, double>();
            for(int i=0; i < SamplesCollection[0].Values.Count; i++)
            {
                try
                {
                    dataToExport.Add(LabelsCollection[i], (double)SamplesCollection[0].Values[i]);
                }
                catch(ArgumentException)
                {
                    dataToExport.Add(LabelsCollection[i]+i.ToString(), (double)SamplesCollection[0].Values[i]);
                }
            }
            _export.Export(dataToExport);
        }

        public void ReadSettings()
        {
            try
            {
                _settings.Read(_settingsPath);
            }
            catch (Exception)
            {
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
          _settings.Save(_settingsPath);
        }
    }
}
