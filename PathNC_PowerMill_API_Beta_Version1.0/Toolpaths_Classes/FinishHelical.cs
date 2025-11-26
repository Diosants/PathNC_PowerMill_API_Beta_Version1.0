using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;

namespace MoldAutomation.Toolpaths
{
    [Obsolete]
     public  class FinishHelical
    {

        internal readonly PMAutomation _powerMill;
        public  FinishHelical( PMAutomation powerMill)
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
                _powerMill.Execute("STRATEGYSELECTOR STRATEGY \"Drilling/Helical.ptf\" NEW");
                _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Drilling/Helical.ptf\"");
                _powerMill.Execute("RENAME TOOLPATH \"#\" \"Finishing_Column\"");
                _powerMill.Execute("EDIT  TPPAGE SWDrillingHole");
                _powerMill.Execute("EDIT  TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT  TPPAGE SWBlock");
                _powerMill.Execute("EDIT  TPPAGE TOOL");
                _powerMill.Execute("ACTIVATE TOOL \"ENDMILLR D10\"");
                _powerMill.Execute("EDIT  TPPAGE SWDrilling");
                _powerMill.Execute("EDIT  DRILL TYPE HELICAL");
                _powerMill.Execute("EDIT  DRILL TOP BLOCK");
                _powerMill.Execute("EDIT  DRILL DEPTH THRU");
                _powerMill.Execute("EDIT  PAR 'AxialDepthOfCut.UserDefined' '1' EDIT DRILL PECK_DEPTH \"1.0\"");
                _powerMill.Execute("EDIT  TOOLPATH LEADS PLUNGEDIST \"1\"");
                _powerMill.Execute("EDIT  DRILL ISTAZ \".5\"");
                _powerMill.Execute("EDIT  DRILL THICKNESS \"0.0\"");
                _powerMill.Execute("EDIT  TPPAGE SWDrillingRetract");
                _powerMill.Execute("EDIT  TPPAGE SWDrillingHelical");
                _powerMill.Execute("EDIT  TPPAGE SWOrderBasic");
                _powerMill.Execute("EDIT  TPPAGE SWAutoVerifMdlG");
                _powerMill.Execute("EDIT  TPPAGE SWCutterComp");
                _powerMill.Execute("EDIT  TPPAGE SWFirstLastLeads");
                _powerMill.Execute("EDIT  TPPAGE SWLeadExtensions");
                _powerMill.Execute("EDIT  TPPAGE SWLink");
                _powerMill.Execute("EDIT  PAR 'Connections.Link[0].Type' 'safe'"); // Connections opt1
                _powerMill.Execute("EDIT  PAR 'Connections.Link[1].Type' 'safe'"); // Connections opt2
                _powerMill.Execute("EDIT  PAR 'Connections.DefaultLink[0].Type' 'safe'"); // Connections Opt3
                _powerMill.Execute("EDIT  TPPAGE SWLinkFilter");
                _powerMill.Execute("EDIT  TPPAGE SWFeedSpeed");

                //  RPM  and Feeds
                _powerMill.Execute("EDIT  RPM \"2000\"");  // RPM
                _powerMill.Execute("EDIT  FRATE \"1500\""); // FeedRate
                _powerMill.Execute("EDIT  PRATE \"8000\"");  // Plunge Rate
                _powerMill.Execute("EDIT  COOLANT AIR");   // Type of Coolant
                _powerMill.Execute("EDIT  TOOLPATH LEADS PLUNGEDIST \"1\"");
                _powerMill.Execute("EDIT  TOOLPATH \"Rgh-Helice-Column\" CALCULATE");
                _powerMill.Execute("EDIT  TPPAGE SWDrilling");
                _powerMill.Execute("FORM FEATURESELECT");
                _powerMill.Execute("EDIT  SELECTION DELETE");
                _powerMill.Execute("EDIT  SELECTION STORE 4 NEW");
                _powerMill.Execute("EDIT  SELECTION ADD EDIT SELECTION APPLY");
                _powerMill.Execute("EDIT  SELECTION APPLY");
                _powerMill.Execute("FEATURESELECT CANCEL");
                _powerMill.Execute("EDIT  TPPAGE SWDrillingHole");
                _powerMill.Execute("EDIT  TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT TPPAGE SWDrillingHole");
                _powerMill.Execute("EDIT  TPPAGE SWDrilling");
                _powerMill.Execute("EDIT  SELECTION FILTER DIAMETER MIN \" 35\"");
                _powerMill.Execute("EDIT  SELECTION FILTER DIAMETER MAX \" 42\"");
                _powerMill.Execute("EDIT  SELECTION APPLY");
                _powerMill.Execute("FEATURESELECT CANCEL");
                _powerMill.Execute("EDIT  TOOLPATH \"Finishing_Column\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFDrilling");


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Finish column operation completed ");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FinishColumnOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }
    }
}
