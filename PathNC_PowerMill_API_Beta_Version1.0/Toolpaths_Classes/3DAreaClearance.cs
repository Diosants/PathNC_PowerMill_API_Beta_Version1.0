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
    [Obsolete("This class is obsolete and will be removed in future versions. Use alternative toolpath strategies instead.")]
    public  class _3DAreaClearance
    {

        private readonly PMAutomation _powerMill;
        public _3DAreaClearance(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }

        public void Run()
        {

            try
            {

                _powerMill.Mydialog();
                // Block reset & preparation
                _powerMill.Execute("FORM BLOCK");
                _powerMill.Execute("BLOCK CANCEL");
                _powerMill.Execute("BLOCK CANCELLED");
                _powerMill.Execute("EDIT MODEL ALL SELECT ALL");
                _powerMill.Execute("FORM BLOCK");
                _powerMill.Execute("EDIT BLOCK RESET");
                _powerMill.Execute("EDIT BLOCK ZMIN LOCK");
                _powerMill.Execute("EDIT BLOCK ZMAX LOCK");
                _powerMill.Execute("BLOCK ACCEPT");

                // Strategy and toolpath
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute("STRATEGYSELECTOR CATEGORY 'Finishing' NEW");
                _powerMill.Execute("STRATEGYSELECTOR STRATEGY \"Finishing/Constant-Z-Finishing.002.ptf\" NEW");
                _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Finishing/Constant-Z-Finishing.002.ptf\"");
                _powerMill.Execute("RENAME TOOLPATH \"#\" \"Finish-Pocket-Wall\"");
                _powerMill.Execute("EDIT    TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT  TPPAGE SWBlock");
                _powerMill.Execute("EDIT  TPPAGE TOOL");
                _powerMill.Execute("ACTIVATE TOOL \"ENDMILLR D10\"");
                _powerMill.Execute("EDIT  TPPAGE SWMachineTool");
                _powerMill.Execute("EDIT  TPPAGE SWStockEngage");
                _powerMill.Execute("EDIT  TPPAGE SWLimit");
                _powerMill.Execute("EDIT  TPPAGE SWConstZFinishing");
                _powerMill.Execute("EDIT PAR 'Spiral' 0");
                _powerMill.Execute("EDIT PAR 'Spiral' 1");
                _powerMill.Execute("EDIT PAR 'CutDirection' 'any'");
                _powerMill.Execute("EDIT PAR 'Thickness' \"0\"");
                _powerMill.Execute("EDIT PAR 'AxialDepthOfCut.UserDefined' '1' EDIT PAR 'Stepdown' \"1.0\"");
                _powerMill.Execute("EDIT  TPPAGE SWHighSpeed");
                _powerMill.Execute("EDIT  TPPAGE SWAreaFilter");
                _powerMill.Execute("EDIT PAR 'UnsafeSegmentRemoval.Active' '1'");
                _powerMill.Execute("EDIT PAR 'UnsafeSegmentRemoval.Threshold' \"4\"");
                _powerMill.Execute("EDIT  TPPAGE SWAutoVerifBasic");
                _powerMill.Execute("EDIT  TPPAGE SWAreaFilter");
                _powerMill.Execute("EDIT  TPPAGE SWPointDistrb");
                _powerMill.Execute("EDIT  TPPAGE SWToolRapidMv");
                _powerMill.Execute("EDIT  TPPAGE SWToolRapidMvClear");
                _powerMill.Execute("EDIT  TPPAGE SWLeadIn");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN SNARC");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN LRAD \"2\"");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN ANGLE \"90\"");
                _powerMill.Execute("EDIT  TPPAGE SWLeadOut");
                _powerMill.Execute("EDIT  TPPAGE SWLeadIn");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADOUT COPY");
                _powerMill.Execute("EDIT TPPAGE SWLink");
                _powerMill.Execute("EDIT PAR 'Connections.Link[0].Type' 'circular_arc'");
                _powerMill.Execute("EDIT PAR 'Connections.Link.First.Constraint[0].Distance.Value' \"20\"");
                _powerMill.Execute("EDIT PAR 'Connections.Link[1].Type' 'safe'");
                _powerMill.Execute("EDIT PAR 'Connections.DefaultLink[0].Type' 'safe'");
                _powerMill.Execute("EDIT  TPPAGE SWFeedSpeed");
                _powerMill.Execute("EDIT  RPM \"3500\"");
                _powerMill.Execute("EDIT  FRATE \"2000\"");
                _powerMill.Execute("EDIT  PRATE \"2000\"");
                _powerMill.Execute("EDIT TOOLPATH \"Finish-Pocket-Wall\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFConstZFinishing");


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Finish pocket-wall operation completed ");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FinishPocketWallOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }

        }
    }
}

