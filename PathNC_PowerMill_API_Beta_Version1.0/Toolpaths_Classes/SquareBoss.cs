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
    public class SquareBoss
    {

        private readonly PMAutomation _powerMill;
        public SquareBoss(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }

        public void Run()
        {



            _powerMill.Mydialog();
            _powerMill.Execute("FORM STRATEGYSELECTOR");
            _powerMill.Execute("STRATEGYSELECTOR CATEGORY 'Feature-Machining' NEW");
            _powerMill.Execute("STRATEGYSELECTOR STRATEGY \"Feature-Machining/Feature-Profile.ptf\" NEW");
            _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Feature-Machining/Feature-Profile.ptf\"");
            _powerMill.Execute("RENAME TOOLPATH \"#\" \"Rgh-Square\"");
            _powerMill.Execute("EDIT TPPAGE SWFeatures");
            _powerMill.Execute("EDIT TPFEATURES LISTUPDATE");
            _powerMill.Execute("0 NEW");
            _powerMill.Execute("EDIT TPPAGE TOOL");
            _powerMill.Execute("ACTIVATE TOOL \"CUTTER D 25\"");
            _powerMill.Execute("EDIT TPPAGE SWMachineTool");
            _powerMill.Execute("EDIT TPPAGE SWLimit");
            _powerMill.Execute("ACTIVATE BOUNDARY \" \"");
            _powerMill.Execute("EDIT TPPAGE SWStepReduction");
            _powerMill.Execute("EDIT TPPAGE SWCutDistFinSOv");
            _powerMill.Execute("EDIT TPPAGE SWFinishFlrWall");
            _powerMill.Execute("EDIT TPPAGE SWAreaFilterAClear");
            _powerMill.Execute("EDIT TPPAGE SWHighSpeedAClr");
            _powerMill.Execute("EDIT TPPAGE SWOrderBasic");
            _powerMill.Execute("EDIT TPPAGE SWApproach");
            _powerMill.Execute("EDIT TPPAGE SWAutoVerifMdlG");
            _powerMill.Execute("EDIT PAR 'CollisionCheck' '0'");
            _powerMill.Execute("EDIT TPPAGE SWCutterComp");
            _powerMill.Execute("EDIT TPPAGE SWPointDistrb");
            _powerMill.Execute("EDIT TPPAGE SWToolRapidMvClear");
            _powerMill.Execute("EDIT TPPAGE SWLeadIn");
            _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN HARC");
            _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN ANGLE \"90\"");
            _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN LRAD \"5\"");
            _powerMill.Execute("EDIT TOOLPATH LEADS LEADOUT COPY");
            _powerMill.Execute("EDIT TPPAGE SWLink");
            _powerMill.Execute("EDIT PAR 'Connections.Link[0].Type' 'circular_arc'");
            _powerMill.Execute("EDIT PAR 'Connections.Link.First.Constraint[0].Distance.Value' \"50\"");
            _powerMill.Execute("EDIT TPPAGE SWAreaClearance");
            _powerMill.Execute("EDIT PAR 'Thickness' \".5\"");
            _powerMill.Execute("EDIT PAR 'Tolerance' \".1\"");
            _powerMill.Execute("EDIT PAR 'AreaClearance.Profile.CutDirection' 'climb'");
            _powerMill.Execute("EDIT ZHEIGHTS AUTOMATIC STEPDOWN \".75\"");
            _powerMill.Execute("EDIT TOOLPATH \"Rgh-Square\" CALCULATE");
            _powerMill.Execute("FORM ACCEPT SFAreaClearance");


            _powerMill.Execute("FORM BLOCK");
            _powerMill.Execute("EDIT BLOCK XLEN UNLOCK");
            _powerMill.Execute("EDIT BLOCK YLEN UNLOCK");
            _powerMill.Execute("EDIT BLOCK ZMIN UNLOCK");
            _powerMill.Execute("EDIT BLOCK ZMAX UNLOCK");
            _powerMill.Execute("EDIT BLOCK RESETLIMIT \"0\"");
            _powerMill.Execute("EDIT BLOCK RESET");
            _powerMill.Execute("BLOCK ACCEPT");
            _powerMill.Execute("GRAPHICS UNLOCK");






        }
    }
}

