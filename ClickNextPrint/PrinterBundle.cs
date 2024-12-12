using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClickNextPrint
{
    internal class PrinterBundle
    {
        private string InfFileName { get; set; }
        private string DriverName { get; set; }
        private string PrinterName { get; set; }
        private string PrinterAddress { get; set; }
        private string DuplexingMode { get; set; }
        private bool Color { get; set; }
        private bool Collate { get; set; }
        private string DriverPath { get; set; }
        private string SaveFileName { get; set; }


        public PrinterBundle(string infFileName, string driverName, string printerName, string printerAddress, string duplexingMode, bool color, bool collate, string driverPath, string saveFileName)
        {
            this.InfFileName = infFileName;
            this.DriverName = driverName;
            this.PrinterName = printerName;
            this.PrinterAddress = printerAddress;
            this.DuplexingMode = duplexingMode;
            this.Color = color;
            this.Collate = collate;
            this.DriverPath = driverPath;
            this.SaveFileName = saveFileName;
        }

        private string getPSBool(bool nativeBool)
        {
            return nativeBool ? "$true" : "$false";
        }

        private void copyDrivers(string src, string dst)
        {
            foreach (string dir in Directory.GetDirectories(src))
            {
                string newDir = Path.Combine(dst, Path.GetFileName(dir));
                Directory.CreateDirectory(newDir);
                this.copyDrivers(dir, newDir);
            }

            foreach (string file in Directory.GetFiles(src))
            {
                File.Copy(file, Path.Combine(dst, Path.GetFileName(file)));
            }
        }

        public void Build(string destinationFolder)
        {
            // Verify the intune util app is available.
            string intuneUtilPath = Path.Combine(AppContext.BaseDirectory, "IntuneWinAppUtil.exe");
            if (!Path.Exists(intuneUtilPath))
            {
                throw new Exception("Missing IntuneWinAppUtil.exe - Download it from https://github.com/microsoft/Microsoft-Win32-Content-Prep-Tool and place it in the same folder as this application");
            }

            // Replace placeholder values in embedded templates.
            string installScript = Properties.Resources.InstallPrinterDriver
                .Replace("{{VAR_INFFILENAME}}", this.InfFileName)
                .Replace("{{VAR_DRIVERNAME}}", this.DriverName)
                .Replace("{{VAR_PRINTERNAME}}", this.PrinterName)
                .Replace("{{VAR_PRINTERADDRESS}}", this.PrinterAddress)
                .Replace("{{VAR_DUPLEXINGMODE}}", this.DuplexingMode)
                .Replace("{{VAR_COLOR}}", this.getPSBool(this.Color))
                .Replace("{{VAR_COLLATE}}", this.getPSBool(this.Collate));
            string uninstallScript = Properties.Resources.UninstallPrinterDriver
                .Replace("{{VAR_PRINTERNAME}}", this.PrinterName);
            
            // Copy templates and drivers into a build temp folder.
            string buildDirectory = Path.Combine(new []{ Path.GetTempPath(), "ClickNext", "Build", Guid.NewGuid().ToString() });
            Directory.CreateDirectory(buildDirectory);

            File.WriteAllText(Path.Combine(buildDirectory, this.SaveFileName + ".ps1"), installScript);
            File.WriteAllText(Path.Combine(buildDirectory, "Uninstall.ps1"), uninstallScript);

            Directory.CreateDirectory(Path.Combine(buildDirectory, "Drivers"));
            this.copyDrivers(this.DriverPath, Path.Combine(buildDirectory, "Drivers"));

            // Start IntuneWinAppUtil.exe with temp folder as source and destination folder as out.
            ProcessStartInfo processStartInfo = new ProcessStartInfo(intuneUtilPath)
            {
                Arguments = @"-c " + buildDirectory + @" -s " + Path.Combine(buildDirectory, this.SaveFileName + ".ps1") + @" -o " + destinationFolder + @" -q",
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            Process.Start(processStartInfo);
        }
    }
}
