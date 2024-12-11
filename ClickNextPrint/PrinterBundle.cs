using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickNextPrint
{
    internal class PrinterBundle
    {
        private string FileName { get; set; }
        private string DriverName { get; set; }
        private string PrinterName { get; set; }
        private string PrinterAddress { get; set; }
        private string DuplexingMode { get; set; }
        private bool Color { get; set; }
        private bool Collate { get; set; }


        public PrinterBundle(string fileName, string driverName, string printerName, string printerAddress, string duplexingMode, bool color, bool collate)
        {
            this.FileName = fileName;
            this.DriverName = driverName;
            this.PrinterName = printerName;
            this.PrinterAddress = printerAddress;
            this.DuplexingMode = duplexingMode;
            this.Color = color;
            this.Collate = collate;
        }
    }
}
