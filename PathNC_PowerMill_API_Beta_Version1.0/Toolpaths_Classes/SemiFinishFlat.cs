using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;
using System;
using System.Globalization;

namespace MoldAutomation.Toolpaths
{
    [Obsolete]
    public class SemiFinishFlat
    {
        private readonly PMAutomation _powerMill;

        // —— Tunable params ——
        public string ToolpathName { get; set; } = "Semi-Finish-Flat";
        public string StrategyPtf { get; set; } = "Finishing/Offset-Flat-Finishing.ptf";
        public string ToolName { get; set; } = "CUTTER D 16";

        // Geometry / stock
        public bool UseAxialThickness { get; set; } = true;
        public double Thickness { get; set; } = 0.50;  // radial stock (mm)
        public double AxialThickness { get; set; } = 0.15;  // axial stock (mm)
        public double Stepover { get; set; } = 5.0;   // mm

        // Leads (ramp in)
        public double RampAngleDeg { get; set; } = 1.0;
        public double RampHeightInc { get; set; } = 0.5;

        // Feeds & speeds
        public int SpindleRPM { get; set; } = 0;        // set 0 to skip
        public double FeedRate { get; set; } = 1500.0;   // mm/min
        public double PlungeRate { get; set; } = 0.0;      // set 0 to skip
        public double RapidSpeed { get; set; } = 0.0;      // set 0 to skip

        // Links
        public string Link0Type { get; set; } = "straight"; // Connections.Link[0].Type
        public double Link0ConstraintDistance { get; set; } = 30.0;       // First.Constraint[0].Distance.Value
        public string Link1Type { get; set; } = "skim";     // Connections.Link[1].Type
        public string DefaultLink0Type { get; set; } = "skim";     // Connections.DefaultLink[0].Type

        public SemiFinishFlat(PMAutomation powerMill)
        {
            _powerMill = powerMill ?? throw new ArgumentNullException(nameof(powerMill));
        }

        public void Run()
        {
            try
            {
                _powerMill.Mydialog();

                // Strategy/template + name
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute($"STRATEGYSELECTOR STRATEGY \"{StrategyPtf}\" NEW");
                _powerMill.Execute($"IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"{StrategyPtf}\"");
                _powerMill.Execute($"RENAME TOOLPATH \"#\" \"{ToolpathName}\"");

                // Basic pages + tool
                _powerMill.Execute("EDIT TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT TPPAGE SWBlock");
                _powerMill.Execute("EDIT TPPAGE TOOL");
                _powerMill.Execute($"ACTIVATE TOOL \"{ToolName}\"");
                _powerMill.Execute("EDIT TPPAGE SWLimit");

                // Offset flat finishing params
                _powerMill.Execute("EDIT TPPAGE SWOffsetFinFlat");
                _powerMill.Execute($"EDIT PAR 'UseAxialThickness' '{(UseAxialThickness ? "1" : "0")}'");
                _powerMill.Execute($"EDIT PAR 'Thickness' \"{Pm(Thickness)}\"");
                if (UseAxialThickness)
                    _powerMill.Execute($"EDIT PAR 'AxialThickness' \"{Pm(AxialThickness)}\"");

                _powerMill.Execute("EDIT PAR 'RadialDepthOfCut.UserDefined' '1'");
                _powerMill.Execute($"EDIT PAR 'Stepover' \"{Pm(Stepover)}\"");

                // Optional pages (kept for parity with your original)
                _powerMill.Execute("EDIT TPPAGE SWHighSpeedFlat");
                _powerMill.Execute("EDIT TPPAGE SWAutoVerifBasic");
                _powerMill.Execute("EDIT TPPAGE SWPointDistrb");
                _powerMill.Execute("EDIT TPPAGE SWToolRapidMv");

                // Leads: ramp in
                _powerMill.Execute("EDIT TPPAGE SWLeadIn");
                _powerMill.Execute("FORM PMLLEADINRAMP EDIT TOOLPATH LEADS RAMPPAGE LEADINRAMPOPT1");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN RAMPOPT ZIGANGLE \"{Pm(RampAngleDeg)}\"");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN RAMPOPT HEIGHT_INCREMENT \"{Pm(RampHeightInc)}\"");
                _powerMill.Execute("LEADINRAMP ACCEPT");

                // Lead-out + links
                _powerMill.Execute("EDIT TPPAGE SWLeadOut");
                _powerMill.Execute("EDIT TPPAGE SWLink");
                _powerMill.Execute($"EDIT PAR 'Connections.Link[0].Type' '{Link0Type}'");
                _powerMill.Execute($"EDIT PAR 'Connections.Link.First.Constraint[0].Distance.Value' \"{Pm(Link0ConstraintDistance)}\"");
                _powerMill.Execute($"EDIT PAR 'Connections.Link[1].Type' '{Link1Type}'");
                _powerMill.Execute($"EDIT PAR 'Connections.DefaultLink[0].Type' '{DefaultLink0Type}'");

                // Feeds/speeds
                _powerMill.Execute("EDIT TPPAGE SWFeedSpeed");
                if (SpindleRPM > 0) _powerMill.Execute($"EDIT RPM \"{SpindleRPM}\"");
                if (FeedRate > 0) _powerMill.Execute($"EDIT FRATE \"{Pm(FeedRate)}\"");
                if (PlungeRate > 0) _powerMill.Execute($"EDIT PRATE \"{Pm(PlungeRate)}\"");
                if (RapidSpeed > 0) _powerMill.Execute($"EDIT RSPEED \"{Pm(RapidSpeed)}\"");

                // Calculate and accept
                _powerMill.Execute($"EDIT TOOLPATH \"{ToolpathName}\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFOffsetFinFlat");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Semi-finish flat operation completed");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[SemiFinishFlatOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }

        private static string Pm(double v) => v.ToString("0.########", CultureInfo.InvariantCulture);
    }
}
