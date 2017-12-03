using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendDrawing.Model
{
    class ExcelExporter
    {

        public void Export(Dictionary<string, double> values)
        {
            try
            {
                var _file = File.Create(string.Format("Data_{0}.xmls", DateTime.Now.ToString("yyyyMMddHHmmss")));

                using (var excelFile = new ExcelPackage(_file))
                {
                    ExcelWorksheet worksheet = excelFile.Workbook.Worksheets.Add("capture");
                    worksheet.Cells[1, 1].Value = "Time";
                    worksheet.Cells[1, 2].Value = "Value";

                    var index = 2;
                    foreach(var item in values)
                    {
                        worksheet.Cells[index, 1].Value = item.Key;
                        worksheet.Cells[index, 2].Value = item.Value;
                        index++;
                    }

                    excelFile.Save();
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed To Export data");
            }


        }
    }
}
