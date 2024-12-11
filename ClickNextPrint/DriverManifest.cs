using InfHelper;
using InfHelper.Models;
using System.Diagnostics;

namespace ClickNextPrint
{
    internal class DriverManifest
    {
        private string FileContent;

        public DriverManifest(string content)
        {
            this.FileContent = content;
        }

        /// <summary>
        /// Parse the manifest this object represents for driver names.
        /// </summary>
        /// <returns>
        /// A List of driver name strings.
        /// </returns>
        public List<string> GetDriverList()
        {
            // Build a new inf parser instance.
            InfUtil infUtil = new InfUtil();
            // Parse the string contents.
            InfData infData = infUtil.Parse(this.FileContent);

            // Get the manufacturer key. There should only be one.
            string manufacturerString = infData["Manufacturer"].Keys[0].KeyValues[0].Value;

            // Array of platforms in preferred order.
            // TODO: Option to only show 32bit drivers.
            string[] driverPlatforms = [manufacturerString + ".NTamd64", manufacturerString + ".NTamd64.6.0", manufacturerString];

            // Iterate over driver platforms and build a list of drivers from the keys in each platform section.
            List<string> drivers = new List<string>();
            foreach (string driverPlatform in driverPlatforms)
            {
                var platformDrivers = infData[driverPlatform];
                
                // If there are no matching entries for the given platform move on to the next one.
                if (platformDrivers == null) continue;

                // Add keys containing driver names to the drivers list.
                // Sometimes the keys may be mis-identified as anonymous (null), in which case the string we want is in the first value.
                drivers.AddRange(platformDrivers.Keys.Select(key => key.Id ?? key.KeyValues[0].Value));

                // Only add the first matching platform.
                break;
            }

            if (drivers.Count < 1)
            {
                throw new Exception("No supported drivers found");
            }

            // Return de-duplicated list.
            return drivers.Distinct().ToList();
        }
    }
}
