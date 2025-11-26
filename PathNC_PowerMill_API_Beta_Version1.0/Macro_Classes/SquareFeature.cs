using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface;
using MoldAutomation.Helpers;

namespace MoldAutomation.Macros
{
    [Obsolete]
    public class SquareFeature
    {

        internal readonly PMAutomation _powerMill;
        public SquareFeature(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }
        public void Run()
        {

            try
            {
               _powerMill.Mydialog();
                // Create and accept feature group
                _powerMill.Execute("CREATE FEATUREGROUP ;");
                _powerMill.Execute("FORM ACCEPT SFAreaClearance");

                // Boss feature definition
                _powerMill.Execute("EXPLORER SELECT FeatureGroup \"2\" NEW");
                _powerMill.Execute("ACTIVATE FEATUREGROUP \"2\" MODE FEATUREEDITOR START");
                _powerMill.Execute("MODE FEATUREEDITOR MODE CREATE RECT_BOSS");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT NAME \"Boss\"");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT LENGTH \"156\"");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT WIDTH \"156\"");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT DRAFT \"0\"");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT HEIGHT \"25\"");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT CURVEPOSITION BOTTOM");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT CREATION SINGLE");
                _powerMill.Execute("MODE FEATUREEDITOR TAB CORNERS");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT EXTERNALCORNERRADIUS \"0\"");
                _powerMill.Execute("MODE FEATUREEDITOR TAB FILLETS");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT TOPTYPE CHAMFER");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT CHAMFER TOP WIDTH \"1\"");
                _powerMill.Execute("MODE FEATUREEDITOR TAB ORIGIN");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT ORIENTATION \"0\"");
                _powerMill.Execute("PICK -158.444 -82.0515 158.444 82.0515 -115.905 -22.2224 -8.56561 -0.583201 -0.683348 0.439218 0.217143 0.389872 0.894901 0 77.185 42.2141 77.185 42.2141");
                _powerMill.Execute("MODE FEATUREEDITOR TAB DIMENSIONS");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT TOP \"0\"");
                _powerMill.Execute("MODE FEATUREEDITOR EDIT BOTTOM \"-25\"");
                _powerMill.Execute("FORM APPLY FEEDIT");
                _powerMill.Execute("FORM CANCEL FEEDIT");
                _powerMill.Execute("MODE FEATUREEDITOR FINISH ACCEPT");

                // Expandir bloco
                _powerMill.Execute("FORM BLOCK");
                _powerMill.Execute("PICK -751.591 -278.625 751.591 278.625 267.068 -96.7139 64 0 0 1 0 1 0 0 -433.775 212.056 -137.434 -107.907");
                _powerMill.Execute("EDIT BLOCK ZMIN UNLOCK");
                _powerMill.Execute("EDIT BLOCK ZMAX LOCK");
                _powerMill.Execute("EDIT BLOCK ZMAX UNLOCK");
                _powerMill.Execute("EDIT BLOCK RESETLIMIT \"0\"");
                _powerMill.Execute("EDIT BLOCK RESET");
                _powerMill.Execute("EDIT BLOCK ZMAX LOCK");
                _powerMill.Execute("EDIT BLOCK ZMIN LOCK");
                _powerMill.Execute("EDIT BLOCK RESETLIMIT \"20\"");
                _powerMill.Execute("EDIT BLOCK RESET");
                _powerMill.Execute("BLOCK ACCEPT");
                _powerMill.Execute("GRAPHICS UNLOCK");

                // Print check symbol on completion
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Boss machining operation completed ✔");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[BossMachiningOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }
    }
}

