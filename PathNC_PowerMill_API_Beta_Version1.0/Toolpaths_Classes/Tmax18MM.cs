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
    [Obsolete("This class is deprecated and may not be supported in future versions.")]
    public class Tmax18MM
    {
        private readonly PMAutomation _powerMill;   
        public Tmax18MM(PMAutomation powerMill)
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
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute("STRATEGYSELECTOR STRATEGY \"Drilling/Deep-Drill.ptf\" NEW");
                _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Drilling/Deep-Drill.ptf\"");

                _powerMill.Execute("EDIT TPPAGE SWDrillingHole");
                _powerMill.Execute("EDIT TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT TPPAGE SWBlock");
                _powerMill.Execute("EDIT TPPAGE TOOL");
                _powerMill.Execute("ACTIVATE TOOL \"HSS_DRILL_DIA_18\"");
                _powerMill.Execute("RENAME TOOLPATH \"#\" \"Rgh-Drill-Column\"");
                _powerMill.Execute("EDIT TPPAGE SWMachineTool");
                _powerMill.Execute("EDIT TPPAGE SWDrilling");
                _powerMill.Execute("EDIT DRILL TOP BLOCK");
                _powerMill.Execute("EDIT DRILL DEPTH THRU");
                _powerMill.Execute("EDIT PAR 'AxialDepthOfCut.UserDefined' '1' EDIT DRILL PECK_DEPTH \"1\"");
                _powerMill.Execute("FORM FEATURESELECT");
                _powerMill.Execute("EDIT SELECTION DELETE");
                _powerMill.Execute("EDIT SELECTION TYPE COLOUR");
                _powerMill.Execute("EDIT SELECTION TYPE DIAMETER");
                _powerMill.Execute("EDIT SELECTION STORE 4 NEW");
                _powerMill.Execute("EDIT SELECTION ADD EDIT SELECTION APPLY");
                _powerMill.Execute("EDIT SELECTION APPLY");
                _powerMill.Execute("FEATURESELECT CANCEL");
                _powerMill.Execute("EDIT TPPAGE SWDrillingRetract");
                _powerMill.Execute("EDIT TPPAGE SWToolRapidMv");
                _powerMill.Execute("EDIT TPPAGE SWLeadsLinks");
                _powerMill.Execute("EDIT TPPAGE SWFirstLastLeads");
                _powerMill.Execute("EDIT TPPAGE SWLeadExtensions");
                _powerMill.Execute("EDIT TPPAGE SWLinkFilter");
                _powerMill.Execute("EDIT TPPAGE SWLink");
                _powerMill.Execute("EDIT PAR 'Connections.Link[0].Type' 'safe'");
                _powerMill.Execute("EDIT PAR 'Connections.Link[1].Type' 'safe'");
                _powerMill.Execute("EDIT PAR 'Connections.DefaultLink[0].Type' 'safe'");
                _powerMill.Execute("EDIT TPPAGE SWFeedSpeed");
                _powerMill.Execute("EDIT RPM \"350\"");
                _powerMill.Execute("EDIT FRATE \"25\"");
                _powerMill.Execute("EDIT PRATE \"3000\"");
                _powerMill.Execute("EDIT COOLANT FLOOD");
                _powerMill.Execute("EDIT TOOLPATH \"Rgh-Drill-Column\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFDrilling");
                _powerMill.Execute("GRAPHICS UNLOCK");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Deep drill operation completed ");
                Console.ResetColor();


            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[DeepDrillOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }
    }
}
