using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using  MoldAutomation.Helpers;


namespace MoldAutomation.Toolpaths
{
    [Obsolete]
    public class Deburring
    {
        private readonly PMAutomation _powerMill;
        public Deburring(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }

        public PMAutomation PowerMill => _powerMill;

        public void Run()
        {
            try
            {


                PowerMill.Mydialog();
                PowerMill.Execute("FORM STRATEGYSELECTOR");
                PowerMill.Execute("STRATEGYSELECTOR STRATEGY \"Feature-Machining/Feature-Chamfer-Milling.ptf\" NEW");
                PowerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Feature-Machining/Feature-Chamfer-Milling.ptf\"");
                PowerMill.Execute("RENAME TOOLPATH \"#\" \"Deburring\"");

                PowerMill.Execute("EDIT   TPPAGE SWFeatures");
                PowerMill.Execute("EDIT  TPPAGE SWWorkplane");
                PowerMill.Execute("EDIT   TPPAGE SWBlock");
                PowerMill.Execute("EDIT  TPPAGE SWMachineTool");
                PowerMill.Execute("EDIT   TPPAGE TOOL");
                PowerMill.Execute("ACTIVATE TOOL \"BALLNOSE_D6\"");
                PowerMill.Execute("EDIT   TPPAGE SWLimit");
                PowerMill.Execute("EDIT   TPPAGE SWOrderBasic");
                PowerMill.Execute("EDIT   TPPAGE SWFeatureChamfer");
                PowerMill.Execute("EDIT   PAR 'FeatureChamfer.Axial.NumberOfCuts' \"1\"");
                PowerMill.Execute("EDIT  PAR 'Chamfer.ToolPosition.AxialDepth' \"1\"");
                PowerMill.Execute("EDIT   PAR 'FeatureChamfer.Radial.NumberOfCuts' \"1\"");
                PowerMill.Execute("EDIT   PAR 'AreaClearance.Profile.CutDirection' 'any'");
                PowerMill.Execute("EDIT  PAR 'Tolerance' \".1\"");
                PowerMill.Execute("EDIT  TPPAGE SWOrderBasic");
                PowerMill.Execute("EDIT  TPPAGE SWAutoVerifMdlG");
                PowerMill.Execute("EDIT  TPPAGE SWLeadIn");
                PowerMill.Execute("EDIT  TOOLPATH LEADS LEADIN SNARC");
                PowerMill.Execute("EDIT  TOOLPATH LEADS LEADOUT COPY");
                PowerMill.Execute("EDIT  TPPAGE SWLink");
                PowerMill.Execute("EDIT  PAR 'Connections.Link[0].Type' 'safe'");
                PowerMill.Execute("EDIT PAR 'Connections.DefaultLink[0].Type' 'skim'");
                PowerMill.Execute("EDIT  TPPAGE SWFeedSpeed");
                PowerMill.Execute("EDIT  RPM \"2000\"");
                PowerMill.Execute("EDIT  FRATE \"1500\"");
                PowerMill.Execute("EDIT TOOLPATH \"Deburring\" CALCULATE");
                PowerMill.Execute("FORM ACCEPT SFFeatureChamfer");
                // _powerMill.Execute("GRAPHICS UNLOCK");
            }
            catch (Exception ex)
            {

                // Optional: Add logging or throw up to caller
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error in DeburringChamfer: {ex.Message}");
                Console.ResetColor();

                throw;

            }
        }
    }
}

