using System;
using Autodesk.ProductInterface.PowerMILL;

namespace API_PowerMill_Version1._1.Tool_Library
{
    public class Tool_Library
    {
        private readonly PMAutomation _powerMill;
        public Tool_Library(PMAutomation powerMill)
        {

            _powerMill = powerMill;

            
        }


     public static void FaceMill( PMAutomation powerMill,double diam, double length, double tipradius, double shankLength, double overhang)
        {
            double upperDiam = 32;
            double lowerDiam = upperDiam;

            powerMill.Execute("ROTATE TRANSFORM FRONT");
            powerMill.Execute("CREATE TOOL ; TIPRADIUSED FORM TOOL");
            powerMill.Execute($" EDIT TOOL \"#\" DIAMETER{diam} ");
            powerMill.Execute($"EDIT TOOL \"#\" LENGTH{length}");
            powerMill.Execute($"EDIT TOOL \"#\" TIPRADIUS {tipradius}");
            powerMill.Execute("EDIT TOOL \"#\" SHANK_COMPONENT ADD");
            powerMill.Execute($"EDIT TOOL \"#\" SHANK_COMPONENT UPPERDIA {upperDiam}");
            powerMill.Execute($"EDIT TOOL \"#\" SHANK_COMPONENT LOWERDIA {lowerDiam}");
            powerMill.Execute($"EDIT TOOL \"#\" SHANK_COMPONENT LENGTH {shankLength}");

            powerMill.Execute("CREATE PATTERN ;");
            powerMill.Execute("EXPLORER SELECT Pattern \"Pattern\\1\" NEW");
            powerMill.Execute("EDIT PATTERN \"1\" CURVEEDITOR START");
            powerMill.Execute("CURVEEDITOR MODE LINE_MULTI");

            // -- Your PICK commands (each as a separate Execute) --
            powerMill.Execute("PICK -40.9609 -38.1025 40.9609 38.1025 18.6022 -7.87782 2.94429 -0 -1 -0 0 0 1 0 -18.4583 -2.94967 -18.4583 -2.94967");
            powerMill.Execute("PICK -40.9609 -38.1025 40.9609 38.1025 18.6022 -7.87782 2.94429 -0 -1 -0 0 0 1 0 -3.49704 -2.94967 -3.49704 -2.94967");
            powerMill.Execute("PICK -40.9609 -38.1025 40.9609 38.1025 18.6022 -7.87782 2.94429 -0 -1 -0 0 0 1 0 -0.881861 1.06432 -0.881861 1.06432");
            powerMill.Execute("PICK -244.146 -227.108 244.146 227.108 21.5395 -7.87782 -28.6734 -0 -1 -0 0 0 1 0 -3.08128 75.582 -3.08128 75.582");
            powerMill.Execute("PICK -77.2494 -71.8585 77.2494 71.8585 19.5585 -7.87782 22.4576 -0 -1 -0 0 0 1 0 4.98938 23.9146 4.98938 23.9146");
            powerMill.Execute("PICK -77.2494 -71.8585 77.2494 71.8585 19.5585 -7.87782 22.4576 -0 -1 -0 0 0 1 0 5.44817 31.5994 5.44817 31.5994");
            powerMill.Execute("PICK -77.2494 -71.8585 77.2494 71.8585 19.5585 -7.87782 22.4576 -0 -1 -0 0 0 1 0 2.12192 34.0081 2.12192 34.0081");
            powerMill.Execute("PICK -77.2494 -71.8585 77.2494 71.8585 19.5585 -7.87782 22.4576 -0 -1 -0 0 0 1 0 2.46602 38.596 2.46602 38.596");
            powerMill.Execute("PICK -77.2494 -71.8585 77.2494 71.8585 19.5585 -7.87782 22.4576 -0 -1 -0 0 0 1 0 4.98938 42.9545 4.98938 42.9545");
            powerMill.Execute("PICK -63.6515 -59.2096 63.6515 59.2096 18.6624 -7.87782 38.5412 -0 -1 -0 0 0 1 0 5.81228 34.5429 5.81228 34.5429");
            powerMill.Execute("PICK -63.6515 -59.2096 63.6515 59.2096 18.6624 -7.87782 38.5412 -0 -1 -0 0 0 1 0 -0.708814 34.4484 -0.708814 34.4484");
            powerMill.Execute("PICK -43.2152 -40.1994 43.2152 40.1994 20.4491 -7.87782 53.4613 -0 -1 -0 0 0 1 0 -2.85535 22.4257 -2.85535 22.4257");
            powerMill.Execute("PICK -89.5394 -83.2909 89.5394 83.2909 21.0275 -7.87782 76.9301 -0 -1 -0 0 0 1 0 -10.9681 73.187 -10.9681 73.187");
            powerMill.Execute("PICK -89.5394 -83.2909 89.5394 83.2909 21.0275 -7.87782 76.9301 -0 -1 -0 0 0 1 0 -17.0836 73.5858 -17.0836 73.5858");

            powerMill.Execute("CURVEEDITOR FINISH ACCEPT");
            powerMill.Execute("EXPLORER SELECT Tool \"Tool\\#\" NEW");
            powerMill.Execute("FORM TOOL \"#\"");
            powerMill.Execute("EDIT TOOL \"#\" HOLDERFROM PATTERN \"1\"");
            powerMill.Execute("EDIT TOOL \"#\" HOLDERFROM CREATE");
            powerMill.Execute($"EDIT TOOL \"#\" OVERHANG {overhang}");
            powerMill.Execute("TOOL ACCEPT");
            powerMill.Execute("STRING TName = \"Face Mill Diam \" + string(Tool.Diameter)");
            powerMill.Execute("RENAME TOOL ; $TName");
            powerMill.Execute("PRINT = Tool.Name");
            powerMill.Execute("TOOL ACCEPT");
            powerMill.Execute("EXPLORER SELECT Pattern \"Pattern\\1\" NEW");
            powerMill.Execute("DELETE PATTERN \"1\"");


        }


    }

}
