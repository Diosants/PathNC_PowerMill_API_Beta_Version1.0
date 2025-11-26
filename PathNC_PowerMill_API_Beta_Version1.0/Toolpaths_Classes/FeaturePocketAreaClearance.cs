using System;
using System.Globalization;
using Autodesk.ProductInterface.PowerMILL;

namespace MoldAutomation.Toolpaths
{
    [Obsolete]
    public class FeaturePocketAreaClearance
    {
        private readonly PMAutomation _powerMill;

        // ——— Names / template / tool ———
        public string ToolpathName { get; set; } = "Rgh-Pocket";
        public string StrategyPtf { get; set; } = "Feature-Machining/Feature-Pocket-Area-Clearance.ptf";
        public string ToolName { get; set; } = "CUTTER D 25";

        // ——— Geometry / stock ———
        public double Tolerance { get; set; } = 0.10;  // mm
        public double Thickness { get; set; } = 0.50;  // mm (stock to leave)
        public double Stepover { get; set; } = 15.0;  // mm
        public double StepDown { get; set; } = 0.50;  // mm

        // ——— Leads (ramp) ———
        public string LeadInType { get; set; } = "RAMP"; // RAMP via dialog below
        public double RampAngleDeg { get; set; } = 1.0;
        public double RampHeightInc { get; set; } = 0.5;

        // ——— Links ———
        public string Link0Type { get; set; } = "circular_arc";
        public double Link0ConstraintDistance { get; set; } = 50.0;
        public string Link1Type { get; set; } = "skim";
        public string DefaultLink0Type { get; set; } = "skim";

        // ——— UI noise control ———
        public bool SuppressDialogs { get; set; } = true;

        // ——— Optional feeds/speeds (push only if > 0) ———
        public int SpindleRPM { get; set; } = 0;
        public double FeedRate { get; set; } = 0;   // FRATE
        public double PlungeRate { get; set; } = 0;   // PRATE
        public double RapidSpeed { get; set; } = 0;   // RSPEED

        public FeaturePocketAreaClearance(PMAutomation powerMill)
        {
            _powerMill = powerMill ?? throw new ArgumentNullException(nameof(powerMill));
        }

        public void Run()
        {
            try
            {
                if (SuppressDialogs)
                {
                    _powerMill.Execute("DIALOGS MESSAGE OFF");
                    _powerMill.Execute("DIALOGS ERROR OFF");
                }

                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute("STRATEGYSELECTOR CATEGORY 'Feature-Machining' NEW");
                _powerMill.Execute($"STRATEGYSELECTOR STRATEGY \"{StrategyPtf}\" NEW");
                _powerMill.Execute($"IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"{StrategyPtf}\"");
                _powerMill.Execute($"RENAME TOOLPATH \"#\" \"{ToolpathName}\"");

                // Core parameters
                _powerMill.Execute($"EDIT PAR 'Tolerance' \"{Pm(Tolerance)}\"");
                _powerMill.Execute($"EDIT PAR 'Thickness' \"{Pm(Thickness)}\"");
                _powerMill.Execute("EDIT PAR 'RadialDepthOfCut.UserDefined' '1'");
                _powerMill.Execute($"EDIT PAR 'Stepover' \"{Pm(Stepover)}\"");
                _powerMill.Execute($"EDIT ZHEIGHTS AUTOMATIC STEPDOWN \"{Pm(StepDown)}\"");

                // Tool
                _powerMill.Execute("EDIT TPPAGE SWLimit");
                _powerMill.Execute("EDIT TPPAGE TOOL");
                _powerMill.Execute($"ACTIVATE TOOL \"{ToolName}\"");

                // Navigate key pages (kept for parity)
                _powerMill.Execute("EDIT TPPAGE SWPocketAreaClear");
                _powerMill.Execute("EDIT TPPAGE SWOffsetAreaClear");
                _powerMill.Execute("EDIT TPPAGE SWStepReduction");
                _powerMill.Execute("EDIT TPPAGE SWFinishFlrWall");
                _powerMill.Execute("EDIT TPPAGE SWAreaFilterAClear");
                _powerMill.Execute("EDIT TPPAGE SWHighSpeedAClr");
                _powerMill.Execute("EDIT TPPAGE SWAutoVerifMdlG");

                // Leads (ramp)
                _powerMill.Execute("EDIT TPPAGE SWLeadIn");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN {LeadInType}");
                _powerMill.Execute("FORM PMLLEADINRAMP EDIT TOOLPATH LEADS RAMPPAGE LEADINRAMPOPT1");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN RAMPOPT ZIGANGLE \"{Pm(RampAngleDeg)}\"");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN RAMPOPT HEIGHT_INCREMENT \"{Pm(RampHeightInc)}\"");
                _powerMill.Execute("LEADINRAMP ACCEPT");

                _powerMill.Execute("EDIT TPPAGE SWLeadOut");
                _powerMill.Execute("EDIT TOOLPATH LEADS LEADOUT NONE");

                // Links
                _powerMill.Execute("EDIT TPPAGE SWLink");
                _powerMill.Execute($"EDIT PAR 'Connections.Link[0].Type' '{Link0Type}'");
                _powerMill.Execute($"EDIT PAR 'Connections.Link.First.Constraint[0].Distance.Value' \"{Pm(Link0ConstraintDistance)}\"");
                _powerMill.Execute($"EDIT PAR 'Connections.Link[1].Type' '{Link1Type}'");
                _powerMill.Execute($"EDIT PAR 'Connections.DefaultLink[0].Type' '{DefaultLink0Type}'");

                // Optional feeds/speeds
                if (SpindleRPM > 0 || FeedRate > 0 || PlungeRate > 0 || RapidSpeed > 0)
                {
                    _powerMill.Execute("EDIT TPPAGE SWFeedSpeed");
                    if (SpindleRPM > 0) _powerMill.Execute($"EDIT RPM \"{SpindleRPM}\"");
                    if (FeedRate   > 0) _powerMill.Execute($"EDIT FRATE \"{Pm(FeedRate)}\"");
                    if (PlungeRate > 0) _powerMill.Execute($"EDIT PRATE \"{Pm(PlungeRate)}\"");
                    if (RapidSpeed > 0) _powerMill.Execute($"EDIT RSPEED \"{Pm(RapidSpeed)}\"");
                }

                // Calculate & accept
                _powerMill.Execute($"EDIT TOOLPATH \"{ToolpathName}\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFPocketAreaClear");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Pocket area clearance completed");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[PocketAreaClearanceOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }
            finally
            {
                if (SuppressDialogs)
                {
                    _powerMill.Execute("DIALOGS MESSAGE ON");
                    _powerMill.Execute("DIALOGS ERROR ON");
                }
            }
        }

        private static string Pm(double v) => v.ToString("0.########", CultureInfo.InvariantCulture);
    }
}
