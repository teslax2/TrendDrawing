using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendDrawing.Model
{
    interface ISamples
    {
        List<double> Samples { get; set; }
        String SourcePath { get; set; }

        Task ReadAsync();
        void Read();

        event EventHandler<List<double>> DataReceived;

    }
}
