using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;
using System;
using System.Globalization;

namespace MoldAutomation.Toolpaths
{
    [Obsolete]
    public class SemiFinishZconst
    {
        private readonly PMAutomation _powerMill;

        // —— Names / templates ——
        public string ToolpathName { get; set; } = "Semi-Finish";
        public string StrategyPtf { get; set; } = "Finishing/Constant-Z-Finishing.002.ptf";
        public string ToolName { get; set; } = "CUTTER D 16";

        // —— Geometry / stock ——
        public string CutDirection { get; set; } = "any";   // "climb", "conventional", "any"
        public bool Spiral { get; set; } = true;    // Constant-Z spiral on/off
        public double Thickness { get; set; } = 0.15;    // stock to leave (mm)
        public double Stepdown { get; set; } = 0.5;     // axial stepdown (mm)

        // —— Leads (arc copy by default) ——
        public string LeadInType { get; set; } = "SNARC"; // arc lead-in
        public double LeadInRadius { get; set; } = 2.0;     // mm
        public double LeadInAngleDeg { get; set; } = 90.0;    // deg
        public string LeadOutMode { get; set; } = "COPY";  // "COPY" or "NONE"

        // —— Links ——
        public string Link0Type { get; set; } = "circular_arc";
        public double Link0ConstraintDistance { get; set; } = 20.0;
        public string Link1Type { get; set; } = "safe";
        public string DefaultLink0Type { get; set; } = "safe";

        // —— Feeds & speeds ——
        public int SpindleRPM { get; set; } = 3500;
        public double FeedRate { get; set; } = 2000.0; // mm/min
        public double PlungeRate { get; set; } = 2000.0; // mm/min
        public double RapidSpeed { get; set; } = 0.0;    // set >0 to push RSPEED

        public SemiFinishZconst(PMAutomation powerMill)
        {
            _powerMill = powerMill ?? throw new ArgumentNullException(nameof(powerMill), "PowerMill automation instance cannot be null.");
        }

        public void Run()
        {
            try
            {
                _powerMill.Mydialog();

                // ——— Block reset & prep ———
                _powerMill.Execute("FORM BLOCK");
                _powerMill.Execute("BLOCK CANCEL");
                _powerMill.Execute("BLOCK CANCELLED");
                _powerMill.Execute("EDIT MODEL ALL SELECT ALL");
                _powerMill.Execute("FORM BLOCK");
                _powerMill.Execute("EDIT BLOCK RESET");
                _powerMill.Execute("EDIT BLOCK ZMIN LOCK");
                _powerMill.Execute("EDIT BLOCK ZMAX LOCK");
                _powerMill.Execute("BLOCK ACCEPT");

                // ——— Strategy / template ———
                _powerMill.Execute("FORM STRATEGYSELECTOR");
                _powerMill.Execute("STRATEGYSELECTOR CATEGORY 'Finishing' NEW");
                _powerMill.Execute($"STRATEGYSELECTOR STRATEGY \"{StrategyPtf}\" NEW");
                _powerMill.Execute($"IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"{StrategyPtf}\"");

                // ——— Naming & tool ———
                _powerMill.Execute($"RENAME TOOLPATH \"#\" \"{ToolpathName}\"");
                _powerMill.Execute("EDIT TPPAGE SWWorkplane");
                _powerMill.Execute("EDIT TPPAGE SWBlock");
                _powerMill.Execute("EDIT TPPAGE TOOL");
                _powerMill.Execute($"ACTIVATE TOOL \"{ToolName}\"");
                _powerMill.Execute("EDIT TPPAGE SWMachineTool");
                _powerMill.Execute("EDIT TPPAGE SWStockEngage");
                _powerMill.Execute("EDIT TPPAGE SWLimit");

                // ——— Constant-Z parameters ———
                _powerMill.Execute("EDIT TPPAGE SWConstZFinishing");
                _powerMill.Execute($"EDIT PAR 'Spiral' '{(Spiral ? "1" : "0")}'");
                _powerMill.Execute($"EDIT PAR 'CutDirection' '{CutDirection}'");
                _powerMill.Execute($"EDIT PAR 'Thickness' \"{Pm(Thickness)}\"");

                // split combined line into two valid commands:
                _powerMill.Execute("EDIT PAR 'AxialDepthOfCut.UserDefined' '1'");
                _powerMill.Execute($"EDIT PAR 'Stepdown' \"{Pm(Stepdown)}\"");

                // ——— Optional pages (kept for parity) ———
                _powerMill.Execute("EDIT TPPAGE SWHighSpeed");
                _powerMill.Execute("EDIT TPPAGE SWAreaFilter");
                _powerMill.Execute("EDIT TPPAGE SWAutoVerifBasic");
                _powerMill.Execute("EDIT TPPAGE SWAreaFilter");      // (your original had this twice)
                _powerMill.Execute("EDIT TPPAGE SWPointDistrb");
                _powerMill.Execute("EDIT TPPAGE SWToolRapidMv");
                _powerMill.Execute("EDIT TPPAGE SWToolRapidMvClear");

                // ——— Leads ———
                _powerMill.Execute("EDIT TPPAGE SWLeadIn");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN {LeadInType}");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN LRAD \"{Pm(LeadInRadius)}\"");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADIN ANGLE \"{Pm(LeadInAngleDeg)}\"");

                _powerMill.Execute("EDIT TPPAGE SWLeadOut");
                _powerMill.Execute("EDIT TPPAGE SWLeadIn");
                _powerMill.Execute($"EDIT TOOLPATH LEADS LEADOUT {LeadOutMode}");

                // ——— Links ———
                _powerMill.Execute("EDIT TPPAGE SWLink");
                _powerMill.Execute($"EDIT PAR 'Connections.Link[0].Type' '{Link0Type}'");
                _powerMill.Execute($"EDIT PAR 'Connections.Link.First.Constraint[0].Distance.Value' \"{Pm(Link0ConstraintDistance)}\"");
                _powerMill.Execute($"EDIT PAR 'Connections.Link[1].Type' '{Link1Type}'");
                _powerMill.Execute($"EDIT PAR 'Connections.DefaultLink[0].Type' '{DefaultLink0Type}'");

                // ——— Feeds & speeds ———
                _powerMill.Execute("EDIT TPPAGE SWFeedSpeed");
                if (SpindleRPM > 0) _powerMill.Execute($"EDIT RPM \"{SpindleRPM}\"");
                if (FeedRate   > 0) _powerMill.Execute($"EDIT FRATE \"{Pm(FeedRate)}\"");
                if (PlungeRate > 0) _powerMill.Execute($"EDIT PRATE \"{Pm(PlungeRate)}\"");
                if (RapidSpeed > 0) _powerMill.Execute($"EDIT RSPEED \"{Pm(RapidSpeed)}\"");

                // ——— Calculate & accept ———
                _powerMill.Execute($"EDIT TOOLPATH \"{ToolpathName}\" CALCULATE");
                _powerMill.Execute("FORM ACCEPT SFConstZFinishing");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Semi-finish (Constant-Z) completed");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[SemiFinishOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }

        private static string Pm(double v) => v.ToString("0.########", CultureInfo.InvariantCulture);
    }
}
