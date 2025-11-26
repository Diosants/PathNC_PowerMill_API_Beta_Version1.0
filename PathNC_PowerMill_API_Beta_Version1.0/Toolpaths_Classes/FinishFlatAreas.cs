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
    public  class FinishFlatAreas
    {
        internal readonly PMAutomation _powerMill;
        public FinishFlatAreas(PMAutomation powerMill)
        {
           _powerMill = powerMill;
        }


        public void Run()
        {

            try
            {


                _powerMill.Mydialog();
                // Restore block before finishing flat
                _powerMill.Execute("FORM BLOCK");
                _powerMill.Execute("EDIT BLOCK XLEN UNLOCK");
                _powerMill.Execute("EDIT BLOCK YLEN UNLOCK");
                _powerMill.Execute("EDIT BLOCK ZMIN UNLOCK");
                _powerMill.Execute("EDIT BLOCK ZMAX UNLOCK");
                _powerMill.Execute("EDIT BLOCK RESETLIMIT \"-1\"");
                _powerMill.Execute("EDIT BLOCK RESET");
                _powerMill.Execute("BLOCK ACCEPT");

                // Offset Flat Finishing
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute("STRATEGYSELECTOR STRATEGY \"Finishing/Offset-Flat-Finishing.ptf\" NEW");
                _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Finishing/Offset-Flat-Finishing.ptf\"");
                _powerMill.Execute("RENAME TOOLPATH \"#\" \"Finish-Flat\"");
                _powerMill.Execute("EDIT   TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT  TPPAGE SWBlock");
                _powerMill.Execute("EDIT  TPPAGE TOOL");
                _powerMill.Execute("ACTIVATE TOOL \"ENDMILLR D10\"");
                _powerMill.Execute("EDIT  TPPAGE SWLimit");
                _powerMill.Execute("EDIT  TPPAGE SWOffsetFinFlat");
                _powerMill.Execute("EDIT PAR 'UseAxialThickness' '1'");
                _powerMill.Execute("EDIT PAR 'Thickness' \"0\"");
                _powerMill.Execute("EDIT PAR 'AxialThickness' \"0\"");
                _powerMill.Execute("EDIT PAR 'AxialThickness' \"0\"");
                _powerMill.Execute("EDIT PAR 'AxialThickness' \"0\"");
                _powerMill.Execute("EDIT PAR 'AxialThickness' \"0\"");
                _powerMill.Execute("EDIT PAR 'RadialDepthOfCut.UserDefined' '1' Edit Par 'Stepover' \"3\"");
                _powerMill.Execute("EDIT  NOGUI TPPAGE SWHighSpeedFlat");
                _powerMill.Execute("EDIT  TPPAGE SWAutoVerifBasic");
                _powerMill.Execute("EDIT  TPPAGE SWPointDistrb");
                _powerMill.Execute("EDIT  TPPAGE SWToolRapidMv");
                _powerMill.Execute("EDIT  TPPAGE SWLeadIn");
                _powerMill.Execute("FORM PMLLEADINRAMP EDIT TOOLPATH LEADS RAMPPAGE LEADINRAMPOPT1");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN RAMPOPT ZIGANGLE \"1\"");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN RAMPOPT HEIGHT_INCREMENT \".5\"");
                _powerMill.Execute("LEADINRAMP ACCEPT");
                _powerMill.Execute("EDIT TPPAGE SWLeadOut");
                _powerMill.Execute("EDIT TPPAGE SWLink");
                _powerMill.Execute("EDIT PAR 'Connections.Link[0].Type' 'straight'");
                _powerMill.Execute("EDIT PAR 'Connections.Link.First.Constraint[0].Distance.Value' \"30\"");
                _powerMill.Execute("EDIT PAR 'Connections.Link[1].Type' 'skim'");
                _powerMill.Execute("EDIT PAR 'Connections.DefaultLink[0].Type' 'skim'");
                _powerMill.Execute("EDIT  TPPAGE SWFeedSpeed");
                _powerMill.Execute("EDIT  RPM \"2000\"");
                _powerMill.Execute("EDIT  FRATE \"1200\"");
                _powerMill.Execute("EDIT  PRATE \"6000\"");
                _powerMill.Execute("EDIT TOOLPATH \"Finish-Flat\" CALCULATE");
                _powerMill.Execute("FORM  ACCEPT SFOffsetFinFlat");
                _powerMill.Execute("DEACTIVATE TOOL");
                _powerMill.Execute("GRAPHICS UNLOCK");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Finish flat operation completed ");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FinishFlatOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;

            }
        }
    }
}

