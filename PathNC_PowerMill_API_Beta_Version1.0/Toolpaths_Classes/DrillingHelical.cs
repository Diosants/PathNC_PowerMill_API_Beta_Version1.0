using System;
using Autodesk.ProductInterface.PowerMILL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;



namespace P1PlateStandard.Machining

{
    [Obsolete]
    public  class DrillingHelical
    {
        private readonly PMAutomation _powerMill;

        public DrillingHelical(PMAutomation powerMill)
        {
            _powerMill=powerMill;
        }
        public void Run()
        {

            try
            {
                
          
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute("STRATEGYSELECTOR CATEGORY 'Drilling' NEW");
                _powerMill.Execute("STRATEGYSELECTOR STRATEGY \"Drilling/Helical.ptf\" NEW");
                _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Drilling/Helical.ptf\"");
                _powerMill.Execute("RENAME TOOLPATH \"#\" \"Rgh-Helice-Column\"");
                _powerMill.Execute("EDIT  TPPAGE SWDrillingHole");
                _powerMill.Execute("EDIT  TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT  TPPAGE SWBlock");
                _powerMill.Execute("EDIT   TPPAGE TOOL");
                _powerMill.Execute("ACTIVATE TOOL \"CUTTER D 16\"");
                _powerMill.Execute("EDIT   TPPAGE SWDrilling");
                _powerMill.Execute("EDIT   DRILL TYPE HELICAL");
                _powerMill.Execute("EDIT  DRILL TOP BLOCK");
                _powerMill.Execute("EDIT   DRILL DEPTH THRU");
                _powerMill.Execute("EDIT  PAR 'AxialDepthOfCut.UserDefined' '1' EDIT DRILL PECK_DEPTH \"1.0\"");
                _powerMill.Execute("EDIT   TOOLPATH LEADS PLUNGEDIST \"1\"");
                _powerMill.Execute("EDIT  DRILL ISTAZ \"0.5\"");
                _powerMill.Execute("EDIT   DRILL THICKNESS \".25\"");
                _powerMill.Execute("EDIT  TPPAGE SWDrillingRetract");
                _powerMill.Execute("EDIT  TPPAGE SWDrillingHelical");
                _powerMill.Execute("EDIT   TPPAGE SWOrderBasic");
                _powerMill.Execute("EDIT  TPPAGE SWAutoVerifMdlG");

                _powerMill.Execute("EDIT PAR 'Drill.GougeCheck' '0'");
                _powerMill.Execute("EDIT PAR 'CollisionCheck'' '0'");


                _powerMill.Execute("EDIT   TPPAGE SWCutterComp");
                _powerMill.Execute("EDIT   TPPAGE SWFirstLastLeads");
                _powerMill.Execute("EDIT   TPPAGE SWLeadExtensions");
                _powerMill.Execute("EDIT   TPPAGE SWLink");
                _powerMill.Execute("EDIT PAR 'Connections.Link[0].Type' 'safe'");
                _powerMill.Execute("EDIT PAR 'Connections.Link[1].Type' 'safe'");
                _powerMill.Execute("EDIT PAR 'Connections.DefaultLink[0].Type' 'safe'");
                _powerMill.Execute("EDIT   TPPAGE SWLinkFilter");
                _powerMill.Execute("EDIT   TPPAGE SWFeedSpeed");
                _powerMill.Execute("EDIT   RPM \"3500\"");
                _powerMill.Execute("EDIT   FRATE \"2000\"");
                _powerMill.Execute("EDIT  PRATE \"2000\"");
                _powerMill.Execute("EDIT   COOLANT AIR");
                _powerMill.Execute("EDIT  TOOLPATH LEADS PLUNGEDIST \"1\"");
                _powerMill.Execute("EDIT  TOOLPATH \"Rgh-Helice-Column\" CALCULATE");
                _powerMill.Execute("EDIT  NOGUI TPPAGE SWDrilling");
                _powerMill.Execute("FORM FEATURESELECT");
                _powerMill.Execute("EDIT   SELECTION DELETE");
                _powerMill.Execute("EDIT   SELECTION STORE 4 NEW");
                _powerMill.Execute("EDIT   SELECTION ADD EDIT SELECTION APPLY");
                _powerMill.Execute("EDIT  SELECTION APPLY");
                _powerMill.Execute("FEATURESELECT CANCEL");
                _powerMill.Execute("EDIT   TPPAGE SWDrillingHole");
                _powerMill.Execute("EDIT   TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT  TPPAGE SWDrillingHole");
                _powerMill.Execute("EDIT  TPPAGE SWDrilling");
                _powerMill.Execute("EDIT  SELECTION FILTER DIAMETER MIN \" 40\"");
                _powerMill.Execute("EDIT  SELECTION FILTER DIAMETER MAX \" 42\"");
                _powerMill.Execute("EDIT SELECTION APPLY");
                _powerMill.Execute("FEATURESELECT CANCEL");


                _powerMill.Execute("EDIT TOOLPATH \"Rgh-Helice-Column\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFDrilling");
               // _powerMill.Execute("GRAPHICS UNLOCK");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Helical drill operation completed ");
                Console.ResetColor();
            }
            catch (Exception ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[HelicalDrillOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }
    }
}
