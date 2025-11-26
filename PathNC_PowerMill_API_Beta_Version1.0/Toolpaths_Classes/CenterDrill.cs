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
    public class CenterDrill
    {
        private readonly PMAutomation _powerMill;

        public CenterDrill(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }

        public void Run()
        {
            try
            {
                _powerMill.Mydialog();
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute("STRATEGYSELECTOR CATEGORY 'Drilling' NEW");
                _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Drilling/Break-Chip.ptf\"");
                _powerMill.Execute("RENAME TOOLPATH \"#\" \"Center_Drill\"");
                _powerMill.Execute("EDIT   TPPAGE SWDrillingHole");
                _powerMill.Execute("EDIT   TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT   TPPAGE SWBlock");
                _powerMill.Execute("EDIT   TPPAGE TOOL");
                _powerMill.Execute("ACTIVATE TOOL \"HSS_DRILL_DIA_5\"");
                _powerMill.Execute("EDIT TPPAGE SWDrilling");
                _powerMill.Execute("EDIT DRILL TYPE DRILL");
                _powerMill.Execute("EDIT DRILL TOP HOLE");
                _powerMill.Execute("EDIT DRILL DEPTH USER");
                _powerMill.Execute("EDIT DRILL DEPTH \"3\"");
                _powerMill.Execute("FORM FEATURESELECT");
                _powerMill.Execute("EDIT SELECTION TYPE DIAMETER");
                _powerMill.Execute("EDIT SELECTION STORE 0 NEW");
                _powerMill.Execute("EDIT SELECTION STORE 4 SERIES");
                _powerMill.Execute("EDIT SELECTION ADD EDIT SELECTION APPLY");
                _powerMill.Execute("EDIT SELECTION APPLY");
                _powerMill.Execute("FEATURESELECT CANCEL");
                _powerMill.Execute("EDIT TPPAGE SWDrillingRetract");
                _powerMill.Execute("EDIT TPPAGE SWDrillingFeeds");
                _powerMill.Execute("EDIT TPPAGE SWDrillingIntFeeds");
                _powerMill.Execute("EDIT TPPAGE SWToolRapidMvClear");
                _powerMill.Execute("EDIT TPPAGE SWFirstLastLeads");
                _powerMill.Execute("EDIT TPPAGE SWLink");
                _powerMill.Execute("EDIT PAR 'Connections.Link[0].Type' 'safe'");
                _powerMill.Execute("EDIT PAR 'Connections.Link[0].ApplyConstraints' '0'");
                _powerMill.Execute("EDIT PAR 'Connections.Link[1].Type' 'safe'");
                _powerMill.Execute("EDIT PAR 'Connections.DefaultLink[0].Type' 'safe'");
                _powerMill.Execute("EDIT TPPAGE SWFirstLastLeads");
                _powerMill.Execute("EDIT TPPAGE SWLeadExtensions");
                _powerMill.Execute("EDIT TPPAGE SWLinkFilter");
                _powerMill.Execute("EDIT TPPAGE SWFeedSpeed");
                _powerMill.Execute("EDIT RPM \"1000\"");
                _powerMill.Execute("EDIT PRATE \"2000\"");
                _powerMill.Execute("EDIT RSPEED \"5000\"");
                _powerMill.Execute("EDIT FRATE \"100\"");
                _powerMill.Execute("EDIT TOOLPATH \"Center_Drill\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFDrilling");


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Center drill operation completed ");
                Console.ResetColor();

            }
            catch (Exception ex)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[CenterDrillOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;

            }
        }
    }
}

