using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;
using System;
using System.Globalization;

namespace MoldAutomation.Toolpaths
{
    [Obsolete]
    public class ModelAreraClearance
    {
        private readonly PMAutomation _powerMill;

        // ---------- Tunable parameters (defaults mirror your current script) ----------
        public string ToolpathName { get; set; } = "Rough-Tool-Diam-25mm";
        public string StrategyPtf { get; set; } = "3D-Area-Clearance/Model-Area-Clearance.003.ptf";
        public string ToolName { get; set; } = "CUTTER D 25";

        public double Tolerance { get; set; } = 0.10;  // mm
        public double Thickness { get; set; } = 0.50;  // mm stock to leave
        public double Stepover { get; set; } = 15.0;  // mm
        public double StepDown { get; set; } = 1.0;   // mm

        public int SpindleRPM { get; set; } = 2500;   // rpm
        public double FeedRate { get; set; } = 3200.0; // mm/min
        public double PlungeRate { get; set; } = 3200.0; // mm/min
        public double RapidSpeed { get; set; } = 8000.0; // mm/min

        public double RampAngleDeg { get; set; } = 1.0;  // deg
        public double RampHeightInc { get; set; } = 0.5;  // mm

        public bool AllowToolOutsideBlock { get; set; } = true;

        // Offsets / ordering
        public string OffsetPreference { get; set; } = "minimise_air_moves"; // e.g., minimise_air_moves
        public string OffsetOrder { get; set; } = "outside_in";         // e.g., outside_in / automatic

        // Filters
        public bool SlicerFilterActive { get; set; } = true;
        public double SlicerFilterThreshold { get; set; } = 2.0;

        // Boundary creation
        public bool CreateBoundaryFromBlock { get; set; } = true;

        public PMAutomation PowerMill => _powerMill;

        public ModelAreraClearance(PMAutomation powerMill)
        {
            _powerMill = powerMill ?? throw new ArgumentNullException(nameof(powerMill));
        }

        public void Run()
        {
            try
            {
                PowerMill.Mydialog();

                // ----- Block reset -----
               // PowerMill.Execute("GUI OFF", "SPLITTER TABBROWSER WIDTH 1", "EXPLORER LOWER", "STATUS LOWER", "FORM RIBBON MINIMISE");
              
                PowerMill.Execute("EDIT TPPAGE SWWorkplane");
                PowerMill.Execute("EDIT TPPAGE SWBlock");
                PowerMill.Execute("EDIT BLOCK ZMAX UNLOCK");
                PowerMill.Execute("EDIT BLOCK ZMAX \"0\"");
                PowerMill.Execute("EDIT BLOCK ZMAX LOCK");
                PowerMill.Execute("EDIT BLOCK RESETLIMIT \"2.5\"");
                PowerMill.Execute("EDIT BLOCK RESET");
                PowerMill.Execute("BLOCK ACCEPT");

                // ----- Strategy / template -----
                PowerMill.Execute("FORM STRATEGYSELECTOR");
                PowerMill.Execute($"STRATEGYSELECTOR STRATEGY \"{StrategyPtf}\" NEW");
                PowerMill.Execute($"IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"{StrategyPtf}\"");

                // ----- Name & tool -----
                PowerMill.Execute($"RENAME TOOLPATH \"#\" \"{ToolpathName}\"");
                PowerMill.Execute("EDIT TPPAGE TOOL");
                PowerMill.Execute($"ACTIVATE TOOL \"{ToolName}\"");

                // ----- Limits / boundary -----
                PowerMill.Execute("EDIT TPPAGE SWMachineTool");
                PowerMill.Execute("EDIT TPPAGE SWLimit");
                PowerMill.Execute("ACTIVATE BOUNDARY \" \"");

                if (CreateBoundaryFromBlock)
                {
                    PowerMill.Execute("CREATE BOUNDARY ; BLOCK FORM BOUNDARY");
                    PowerMill.Execute("EDIT BOUNDARY \"#\" CALCULATE");
                    PowerMill.Execute("EDIT BOUNDARY \"#\" ACCEPT BOUNDARY ACCEPT");
                }

                // ----- Area clearance params -----
                PowerMill.Execute("EDIT  TPPAGE SWAreaClearance");
                PowerMill.Execute("EDIT PAR 'AreaClearance.Profile.CutDirection' 'any'");
                PowerMill.Execute("EDIT PAR 'CutDirection' 'any'");
                PowerMill.Execute($"EDIT PAR 'Tolerance' \"{Pm(Tolerance)}\"");
                PowerMill.Execute($"EDIT PAR 'Thickness' \"{Pm(Thickness)}\"");

                PowerMill.Execute("EDIT PAR 'RadialDepthOfCut.UserDefined' '1'");
                PowerMill.Execute($"EDIT PAR 'Stepover' \"{Pm(Stepover)}\"");

                PowerMill.Execute($"EDIT ZHEIGHTS AUTOMATIC STEPDOWN \"{Pm(StepDown)}\"");

                // Offsets
                PowerMill.Execute("EDIT  TPPAGE SWOffsetAreaClear");
                PowerMill.Execute($"EDIT PAR 'AreaClearance.Offset.Preference' '{OffsetPreference}'");
                PowerMill.Execute($"EDIT PAR 'AreaClearance.Offset.Order' '{OffsetOrder}'");

                // Optional pages (keep sequence consistent)
                PowerMill.Execute("EDIT  TPPAGE SWStepReduction");
                PowerMill.Execute("EDIT  TPPAGE SWFinishAreaClr");
                PowerMill.Execute("EDIT  TPPAGE SWAreaFilterAClear");
                if (SlicerFilterActive)
                {
                    PowerMill.Execute("EDIT PAR 'AreaClearance.Slicer.Filter.Active' '1'");
                    PowerMill.Execute($"EDIT PAR 'AreaClearance.Slicer.Filter.Threshold' \"{Pm(SlicerFilterThreshold)}\"");
                }
                else
                {
                    PowerMill.Execute("EDIT PAR 'AreaClearance.Slicer.Filter.Active' '0'");
                }

                PowerMill.Execute("EDIT TPPAGE SWAreaFlatAClear");
                PowerMill.Execute("EDIT  TPPAGE SWApproach");
                PowerMill.Execute("EDIT  TPPAGE SWAutoVerifMdlG");

                // ----- Leads: ramp in, no lead-out -----
                PowerMill.Execute("EDIT  TPPAGE SWLeadIn");
                PowerMill.Execute("FORM PMLLEADINRAMP EDIT TOOLPATH LEADS RAMPPAGE LEADINRAMPOPT1");
                PowerMill.Execute($"EDIT  TOOLPATH LEADS LEADIN RAMPOPT ZIGANGLE \"{Pm(RampAngleDeg)}\"");
                PowerMill.Execute($"EDIT  TOOLPATH LEADS LEADIN RAMPOPT HEIGHT_INCREMENT \"{Pm(RampHeightInc)}\"");
                PowerMill.Execute("LEADINRAMP ACCEPT");

                PowerMill.Execute("EDIT  TPPAGE SWLeadOut");
                PowerMill.Execute("EDIT  TOOLPATH LEADS LEADOUT NONE");

                // ----- Feeds & speeds -----
                PowerMill.Execute("EDIT  TPPAGE SWFeedSpeed");
                PowerMill.Execute($"EDIT  RPM \"{SpindleRPM}\"");
                PowerMill.Execute($"EDIT   FRATE \"{Pm(FeedRate)}\"");
                PowerMill.Execute($"EDIT  PRATE \"{Pm(PlungeRate)}\"");
                PowerMill.Execute($"EDIT  RSPEED \"{Pm(RapidSpeed)}\"");

                // ----- Limits / links -----
                PowerMill.Execute("EDIT   TPPAGE SWLimit");
                PowerMill.Execute($"EDIT PAR 'AllowToolOutsideBlock' '{(AllowToolOutsideBlock ? "1" : "0")}'");

                PowerMill.Execute("EDIT  TPPAGE SWLeadsLinks");
                PowerMill.Execute("EDIT  TPPAGE SWLeadIn");
                PowerMill.Execute("EDIT  TPPAGE SWLeadOut");
                PowerMill.Execute("EDIT  TPPAGE SWLinkFilter");
                PowerMill.Execute("EDIT  TPPAGE SWLink");
                PowerMill.Execute("EDIT  PAR 'Connections.Link[0].Type' 'circular_arc'");

                // ----- Calculate & accept -----
                PowerMill.Execute($"EDIT TOOLPATH \"{ToolpathName}\" CALCULATE");
                PowerMill.Execute("FORM ACCEPT SFAreaClearance");
                PowerMill.RefreshOn();
                //  PowerMill.Execute("GUI ON", "SPLITTER TABBROWSER WIDTH 240", "EXPLORER RAISE", "STATUS RAISE", "FORM RIBBON MAXIMISE");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("3D Area Clearance completed");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ModelAreaClearance ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }

        private static string Pm(double v) => v.ToString("0.########", CultureInfo.InvariantCulture);
    }
}
