using InfHelper;
using InfHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickNextPrint
{
    internal class DriverManifest
    {
        private string FileContent;

        public DriverManifest(string content)
        {
            this.FileContent = content;
        }

        public String[] GetDriverList()
        {
            InfUtil infUtil = new InfUtil();
            InfData infData = infUtil.Parse(this.FileContent);

            Key manufacturerData = infData["Manufacturer"].Keys[0];

            string[] driverCategories = [];
            for (int i = 0; i < manufacturerData.KeyValues.Count; i++)
            {
                if (i == 0)
                {
                    driverCategories.Append(manufacturerData.KeyValues[0].Value);
                    continue;
                }
                driverCategories.Append(manufacturerData.KeyValues[0].Value + "." + manufacturerData.KeyValues[i].Value);
            }

            // TODO: Iterate over driver categories and build a list of drivers from the keys in each category.
            string[] drivers = [];
            return drivers;
        }
    }
}
