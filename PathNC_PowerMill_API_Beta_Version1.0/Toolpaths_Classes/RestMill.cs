using Autodesk.ProductInterface;
using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;
using System;
using System.Globalization;

namespace MoldAutomation.Toolpaths
{
    [Obsolete("This class is deprecated and may not be supported in future versions.")]
    public class RestMill
    {
        private readonly PMAutomation _powerMill;

        // ---- Tunable parameters (defaults are safe-ish) ----
        public double StepDown { get; set; } = 1.0;          // mm
        public double Stepover { get; set; } = 10.0;         // mm
        public double Tolerance { get; set; } = 0.10;        // mm
        public double Thickness { get; set; } = 0.50;        // mm stock to leave

        public double FeedRate { get; set; } = 3200.0;       // mm/min
        public double PlungeRate { get; set; } = 3200.0;     // mm/min
        public int SpindleRPM { get; set; } = 8000;       // rpm
        public double RapidSpeed { get; set; } = 8000.0;     // mm/min (if applicable to your setup)

        public double RampAngleDeg { get; set; } = 1.0;      // deg
        public double RampHeightInc { get; set; } = 0.5;     // mm

        // Tool / references
        public string ToolName { get; set; } = "CUTTER D 16";
        public string ReferenceToolpathName { get; set; } = "Rough-Tool-Diam-25mm";

        // Template/strategy paths (adjust to your library)
        public string StrategyPtf { get; set; } = "3D-Area-Clearance/Model-Area-Clearance.003.ptf";

        public RestMill(PMAutomation powerMill)
        {
            _powerMill = powerMill ?? throw new ArgumentNullException(nameof(powerMill));
        }

        public void Run()
        {
            try
            {
                // Optional: your custom UI hook
                _powerMill.Mydialog();

                // ---- Block / strategy selection ----
                _powerMill.Execute("EDIT TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT TPPAGE SWBlock");
                _powerMill.Execute("EDIT BLOCK ZMAX UNLOCK");
                _powerMill.Execute("EDIT BLOCK ZMAX \"0\"");
                _powerMill.Execute("EDIT BLOCK ZMAX LOCK");
                _powerMill.Execute("EDIT BLOCK RESETLIMIT \"2.5\"");
                _powerMill.Execute("EDIT BLOCK RESET");
                _powerMill.Execute("BLOCK ACCEPT");

                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute($"STRATEGYSELECTOR STRATEGY \"{StrategyPtf}\" NEW");
                _powerMill.Execute($"IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"{StrategyPtf}\"");

                // ---- Name & tool ----
                _powerMill.Execute("RENAME TOOLPATH \"#\" \"RestAreaClearance\"");
                _powerMill.Execute("EDIT TPPAGE TOOL");
                _powerMill.Execute($"ACTIVATE TOOL \"{ToolName}\"");

                // ---- Boundary from block ----
                _powerMill.Execute("EDIT TPPAGE SWMachineTool");
                _powerMill.Execute("EDIT TPPAGE SWLimit");
                _powerMill.Execute("ACTIVATE BOUNDARY \" \"");
                _powerMill.Execute("CREATE BOUNDARY ; BLOCK FORM BOUNDARY");
                _powerMill.Execute("EDIT BOUNDARY \"#\" CALCULATE");

                // ---- Parameters (use invariant culture for decimals) ----
                _powerMill.Execute("EDIT PAR 'AllowToolOutsideBlock' '1'");

                _powerMill.Execute("EDIT TPPAGE SWAreaClearance");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Profile.CutDirection' 'any'");
                _powerMill.Execute("EDIT PAR 'CutDirection' 'any'");
                _powerMill.Execute($"EDIT PAR 'Tolerance' \"{Pm(Tolerance)}\"");
                _powerMill.Execute($"EDIT PAR 'Thickness' \"{Pm(Thickness)}\"");

                _powerMill.Execute("EDIT PAR 'RadialDepthOfCut.UserDefined' '1'");
                _powerMill.Execute($"EDIT PAR 'Stepover' \"{Pm(Stepover)}\"");

                _powerMill.Execute("EDIT PAR 'AreaClearance.Rest.Active' '1'");
                _powerMill.Execute($"EDIT ZHEIGHTS AUTOMATIC STEPDOWN \"{Pm(StepDown)}\"");

                _powerMill.Execute("EDIT TPPAGE SWRest");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'toolpath'");
                _powerMill.Execute($"EDIT PAR 'AreaClearance.Rest.Toolpath' \"{ReferenceToolpathName}\"");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Offset.Preference' 'minimise_air_moves'");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Offset.Order' 'automatic'");

                // ---- Filters (optional) ----
                _powerMill.Execute("EDIT TPPAGE SWStepReduction");
                _powerMill.Execute("EDIT TPPAGE SWFinishAreaClr");
                _powerMill.Execute("EDIT TPPAGE SWAreaFilterAClear");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Slicer.Filter.Active' '1'");
                _powerMill.Execute("EDIT PAR 'AreaClearance.Slicer.Filter.Threshold' \"2.0\"");

                // ---- Leads / links ----
                _powerMill.Execute("EDIT TPPAGE SWApproach");
                _powerMill.Execute("EDIT TPPAGE SWAutoVerifMdlG");
                _powerMill.Execute("EDIT TPPAGE SWLeadIn");
                _powerMill.Execute("FORM PMLLEADINRAMP EDIT TOOLPATH LEADS RAMPPAGE LEADINRAMPOPT1");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN RAMPOPT ZIGANGLE \"{Pm(RampAngleDeg)}\"");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN RAMPOPT HEIGHT_INCREMENT \"{Pm(RampHeightInc)}\"");
                _powerMill.Execute("LEADINRAMP ACCEPT");

                _powerMill.Execute("EDIT TPPAGE SWLeadOut");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADOUT NONE");

                // ---- Feeds & speeds ----
                _powerMill.Execute("EDIT TPPAGE SWFeedSpeed");
                _powerMill.Execute($"EDIT RPM \"{SpindleRPM}\"");
                _powerMill.Execute($"EDIT FRATE \"{Pm(FeedRate)}\"");
                _powerMill.Execute($"EDIT PRATE \"{Pm(PlungeRate)}\"");
                _powerMill.Execute($"EDIT RSPEED \"{Pm(RapidSpeed)}\"");

                // ---- Links ----
                _powerMill.Execute("EDIT TPPAGE SWLimit");
                _powerMill.Execute("EDIT PAR 'AllowToolOutsideBlock' '1'");
                _powerMill.Execute("EDIT TPPAGE SWLeadsLinks");
                _powerMill.Execute("EDIT TPPAGE SWLeadIn");
                _powerMill.Execute("EDIT TPPAGE SWLeadOut");
                _powerMill.Execute("EDIT TPPAGE SWLinkFilter");
                _powerMill.Execute("EDIT TPPAGE SWLink");
                _powerMill.Execute("EDIT PAR 'Connections.Link[0].Type' 'circular_arc'");

                // ---- Calculate & accept ----
                _powerMill.Execute("EDIT TOOLPATH \"RestAreaClearance\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFAreaClearance");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Rest Area Clearance Done");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Rest Area Clearance] {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }

        private static string Pm(double value)
            => value.ToString("0.########", CultureInfo.InvariantCulture);
    }
}
