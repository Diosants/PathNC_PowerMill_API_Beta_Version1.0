using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface;
using MoldAutomation.Helpers;


namespace MoldAutomation.Toolpaths
{
    [Obsolete]
    public class Diam_40_H7
    {

        private readonly PMAutomation _powerMill;
        public Diam_40_H7(PMAutomation powerMill)
        {

            _powerMill = powerMill;
        }
        public void Run()
        {

            try
            {
                _powerMill.Execute("GRAPHICS LOCK");
                _powerMill.Execute("DIALOGS MESSAGE OFF");
                _powerMill.Execute("DIALOGS ERROR OFF");
                _powerMill.Execute("_PICK_1389_");
                _powerMill.Execute("ACTIVE TOOL");
                string ptfPath1 = @"C:/Users/dcard/OneDrive/Desktop/DiogenixCAM/PowerMill-Templates-Strategies/DIAM40_FULL.ptf";
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute($"STRATEGYSELECTOR STRATEGY\"{ptfPath1}\"");
                _powerMill.Execute($"IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"{ptfPath1}\"");
                _powerMill.Execute("EDIT METHOD APPLY");
                _powerMill.Execute("GRAPHICS UNLOCK");


                Console.WriteLine("Drilling template imported and holes created successfully.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error importing drilling template: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}

