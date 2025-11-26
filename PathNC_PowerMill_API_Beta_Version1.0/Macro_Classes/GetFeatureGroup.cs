using Autodesk.ProductInterface.PowerMILL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoldAutomation.Macros
{
    [Obsolete]
    public class GetFeatureGroup
    {
        private readonly PMAutomation _powerMill;
        public GetFeatureGroup(PMAutomation powerMill)
        {
            _powerMill=powerMill;
        }

        public PMAutomation PowerMill => _powerMill;

        public void Run()
        {
            try
            {

                PowerMill.Execute("DIALOGS ERROR OFF");
                PowerMill.DialogsOff();
                PowerMill.Execute("CREATE FEATUREGROUP ;");
                PowerMill.Execute("EXPLORER SELECT FeatureGroup \"FeatureGroup\\#\" NEW");
                PowerMill.Execute("ACTIVATE FEATUREGROUP \"#\" MODE FEATUREEDITOR START");
                PowerMill.Execute("MODE  FEATUREEDITOR MODE DETECT");
                PowerMill.Execute("MODE FEATURECHOOSER CHOOSE ALL");
                PowerMill.Execute("FORM ACCEPT FEATUREFILTER");
                PowerMill.Execute("MODE FEATUREEDITOR FINISH ACCEPT");


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Feature detection completed ");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FeatureDetectionOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }

        }
    }
}
