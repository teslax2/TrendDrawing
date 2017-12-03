using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrendDrawing.ViewModel;

namespace TrendDrawing.View
{
    /// <summary>
    /// Interaction logic for Trend.xaml
    /// </summary>
    public partial class Trend : Window
    {
        TrendViewModel viewModel;

        public Trend()
        {
            SetLanguageDictionary();
            InitializeComponent();

            viewModel = this.FindResource("viewModel") as TrendViewModel;
        }

        public SeriesCollection seriesCollection { get; private set; }

        private async void FileName_ClickAsync(object sender, RoutedEventArgs e)
        {
            var openFile = new Microsoft.Win32.OpenFileDialog();

            if (openFile.ShowDialog() == true)
            {
                if (openFile.FileName != null)
                    viewModel.FileName = openFile.FileName;
                    await viewModel.Open();
            }
        }

        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "pl-PL":
                    dict.Source = new Uri("..\\Resources\\StringResources.pl-PL.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\StringResources.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void SerialPort_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RunSerial();
        }

        private async void SerialPortAsync_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.RunSerialAsync();
        }

        private void CaptureBitmap_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CaptureBitmap();
        }

        private void SaveExcel_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ExcelExport();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
