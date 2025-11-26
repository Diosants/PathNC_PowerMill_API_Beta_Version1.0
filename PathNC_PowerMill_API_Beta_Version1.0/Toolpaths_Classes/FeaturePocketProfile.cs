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
    public class FeaturePocketProfile
    {

        //  initialize  Constructors

        public string ToolName { get; set; } = "CUTTER D 16"; // Default tool name
        public double Tolerance { get; set; } 
        public double Thickness { get; set; }

        // Stepover and Depth of cut
        public double Stepover { get; set; }
        public double StepDown { get; set; }

        private readonly PMAutomation _powerMill;
        public FeaturePocketProfile(PMAutomation powerMill, string toolName, double tolerance, double thickness, double stepOver, double stepDown)
        {
            _powerMill = powerMill;
            ToolName = toolName;
            Tolerance = tolerance;
            Thickness = thickness;
            Stepover = stepOver;
            StepDown = stepDown;



        }
        public void Run()
        {

            try
            {
                _powerMill.Execute("DIALOGS MESSAGE OFF");
                _powerMill.Execute("DIALOGS ERROR OFF");
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute("STRATEGYSELECTOR STRATEGY \"Feature-Machining/Feature-Pocket-Profile.ptf\" NEW");
                _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Feature-Machining/Feature-Pocket-Profile.ptf\"");
                _powerMill.Execute("RENAME TOOLPATH \"#\" \"Rest-Pocket-clearance\"");
                _powerMill.Execute("EDIT TPPAGE SWFeatures");
                _powerMill.Execute("EDIT TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT TPPAGE SWBlock");
                _powerMill.Execute("EDIT TPPAGE TOOL");
                _powerMill.Execute("ACTIVATE TOOL \"CUTTER D 16\"");
                _powerMill.Execute("EDIT TPPAGE SWMachineTool");
                _powerMill.Execute("EDIT TPPAGE SWLimit");
                _powerMill.Execute("ACTIVATE BOUNDARY \" \"");
                _powerMill.Execute("EDIT TPPAGE SWPocketProfile");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Profile.CutDirection' 'any'");
                _powerMill.Execute("EDIT PAR 'Tolerance' \".05\"");
                _powerMill.Execute("EDIT PAR 'Thickness' \".25\"");
                _powerMill.Execute("EDIT PAR 'RadialDepthOfCut.UserDefined' '1' EDIT PAR 'Stepover' \"5\"");
                _powerMill.Execute("EDIT ZHEIGHTS AUTOMATIC STEPDOWN \".5\"");
                _powerMill.Execute("EDIT TPPAGE SWStepReduction");
                _powerMill.Execute("EDIT TPPAGE SWCutDistFinSOv");
                _powerMill.Execute("EDIT AREACLEAROFFSET FINITESTEPOVER OFF");
                _powerMill.Execute("EDIT TPPAGE SWFinishFlrWall");
                _powerMill.Execute("EDIT PAR 'CutDistances.FloorFinishing.Active' '0'");
                _powerMill.Execute("EDIT TPPAGE SWAreaFilterAClear");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Slicer.Filter.Active' '0'");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Slicer.Filter.Active' '1'");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Slicer.Filter.Threshold' \".95\"");
                _powerMill.Execute("EDIT TPPAGE SWLeadIn");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN SNARC");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN DISTANCE \"0\"");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN ANGLE \"90\"");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN LRAD \"1\"");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADOUT COPY");
                _powerMill.Execute("EDIT TPPAGE SWLink");
                _powerMill.Execute("EDIT PAR 'Connections.Link[0].Type' 'circular_arc'");
                _powerMill.Execute("EDIT PAR 'Connections.Link.First.Constraint[0].Distance.Value' \"30\"");
                _powerMill.Execute("EDIT PAR 'Connections.Link[1].Type' 'skim'");
                _powerMill.Execute("EDIT PAR 'Connections.DefaultLink[0].Type' 'skim'");
                _powerMill.Execute("EDIT TPPAGE SWFeedSpeed");
                _powerMill.Execute("EDIT RPM \"3500\"");
                _powerMill.Execute("EDIT FRATE \"3000\"");
                _powerMill.Execute("EDIT TOOLPATH \"Rest-Pocket-clearance\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFPocketProfile");


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Rest pocket profile operation completed ");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[RestPocketProfileOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;

            }

        }
    }
}

