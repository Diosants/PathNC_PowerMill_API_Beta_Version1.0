using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using  MoldAutomation.Helpers;

namespace MoldAutomation.Toolpaths
{
    [Obsolete]
    public  class Facing
    {

         internal readonly PMAutomation _powerMill;

        public Facing(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }

           public void Run()
        {
            try
            {
                _powerMill.Mydialog();
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute("STRATEGYSELECTOR STRATEGY \"Curve-Machining/Face-Milling.002.ptf\" NEW");
                _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Curve-Machining/Face-Milling.002.ptf\"");
                _powerMill.Execute("RENAME TOOLPATH \"#\" \"Facing\"");
                _powerMill.Execute("EDIT NOGUI TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT NOGUI TPPAGE SWBlock");
                //_powerMill.Execute("EDIT BLOCK ZMIN LOCK");
                _powerMill.Execute("EDIT  BLOCK ZMAX UNLOCK");
                _powerMill.Execute("EDIT  BLOCK ZMAX \"1\"");
                _powerMill.Execute("EDIT  BLOCK ZMAX LOCK");
                _powerMill.Execute("EDIT  BLOCK RESETLIMIT \"2.5\"");
                _powerMill.Execute("EDIT  BLOCK RESET");
                _powerMill.Execute("EDIT   TPPAGE TOOL");
                _powerMill.Execute("ACTIVATE TOOL \"CUTTER D 63\"");
                _powerMill.Execute("EDIT NOGUI TPPAGE SWMachineTool");
                _powerMill.Execute("EDIT   TPPAGE SWFaceMilling");
                _powerMill.Execute("Edit par 'Face.Z' \"0\"");
                _powerMill.Execute("Edit par 'Face.XYExpansion' \"5\"");
                _powerMill.Execute("Edit par 'PatternStyle' 'two_way'");
                _powerMill.Execute("EDIT PAR 'Tolerance' \".1\"");
                _powerMill.Execute("Edit par 'RadialDepthOfCut.UserDefined' '1' Edit par 'Stepover' \"35\"");
                _powerMill.Execute("EDIT   TPPAGE SWCutDistVert");
                _powerMill.Execute("Edit par 'CutDistances.Axial.StockDepth' \"1\"");
                _powerMill.Execute("Edit par 'AxialDepthOfCut.UserDefined' '1' Edit par 'Stepdown' \".5\"");
                _powerMill.Execute("EDIT NOGUI TPPAGE SWFinishFlrWall");
                _powerMill.Execute("EDIT NOGUI TPPAGE SWFaceRaster");
                _powerMill.Execute("EDIT  PAR 'AutomaticRasterAngle' '0'");
                _powerMill.Execute("Edit par 'RasterAngle' \"0\"");
                _powerMill.Execute("EDIT NOGUI  TPPAGE SWAutoVerifMdlG");
                _powerMill.Execute("EDIT   PAR 'CollisionCheck' '1'");
                _powerMill.Execute("EDIT  PAR 'ModelGougeCheck' '1'");
                _powerMill.Execute("EDIT NOGUI TPPAGE SWToolRapidMvClear");
                _powerMill.Execute("EDIT NOGUI  TPPAGE SWLeadsLinks");
                _powerMill.Execute("EDIT  TPPAGE SWLeadIn");
                _powerMill.Execute("EDIT  TOOLPATH LEADS LEADIN EXTENDED");
                _powerMill.Execute("EDIT   TOOLPATH LEADS LEADIN DISTANCE \"1\"");
                _powerMill.Execute("EDIT  TOOLPATH LEADS LEADOUT COPY");
                _powerMill.Execute("EDIT   TPPAGE SWLink");
                _powerMill.Execute("EDIT   PAR 'Connections.Link[0].Type' 'circular_arc'");
                _powerMill.Execute("EDIT  PAR 'Connections.Link.First.Constraint[0].Distance.Value' \"50\"");
                _powerMill.Execute("EDIT  PAR 'Connections.Link[1].Type' 'safe'");
                _powerMill.Execute("EDIT  PAR 'Connections.DefaultLink[0].Type' 'safe'");
                _powerMill.Execute("EDIT  TPPAGE SWFeedSpeed");
                _powerMill.Execute("EDIT  RPM \"1000\"");
                _powerMill.Execute("EDIT  FRATE \"1200\"");
                _powerMill.Execute("EDIT  PRATE \"5000\"");
                _powerMill.Execute("EDIT  RSPEED \"8000\"");
                _powerMill.Execute("EDIT  COOLANT AIR");
                _powerMill.Execute("EDIT  TOOLPATH \"Facing\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFFaceMilling");
                _powerMill.DialogsOff();
                //  _powerMill.Execute("GRAPHICS  UNLOCK");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Face milling operation completed ");
                Console.ResetColor();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FaceMillingOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }

        }

    }
}

