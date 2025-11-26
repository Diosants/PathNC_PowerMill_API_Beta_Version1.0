using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface;
using MoldAutomation.Helpers;


namespace MoldAutomation.Machining
{
    [Obsolete]
    public class CompoudHoles
    {
        private readonly PMAutomation _powerMill;

        public CompoudHoles(PMAutomation powerMill)
        {

            _powerMill = powerMill;
        }

        public PMAutomation PowerMill => _powerMill;

        public void Run()
        {
            try
            {
                PowerMill.Mydialog();
                PowerMill.Execute("EDIT MODEL ALL SELECT ALL");
                PowerMill.Execute("CREATE FEATURESET ;");
                PowerMill.Execute("EXPLORER SELECT Featureset \"#\" NEW");
                PowerMill.Execute("EDIT FEATURECREATE TYPE HOLE EDIT FEATURECREATE CIRCULAR ON EDIT FEATURECREATE FILTER HOLES EDIT FEATURECREATE TOPDEFINE ABSOLUTE EDIT FEATURECREATE BOTTOMDEFINE ABSOLUTE FORM CANCEL FEATURE FORM CREATEHOLE");
                PowerMill.Execute("EDIT FEATURECREATE HOLES COMPOUND OFF");
                PowerMill.Execute("EDIT FEATURECREATE HOLES COMPOUND ON");
                PowerMill.Execute("EDIT FEATURECREATE HOLES FIXED OFF");
                PowerMill.Execute("EDIT FEATURECREATE HOLES FIXED ON");
                PowerMill.Execute("EDIT FEATURECREATE HOLES DIRECTION BOTH");
                PowerMill.Execute("EDIT FEATURECREATE HOLES DIRECTION DOWN");
                PowerMill.Execute("EDIT FEATURECREATE NAME  Compound hole");
                PowerMill.Execute("EDIT FEATURECREATE CREATEHOLES");
                PowerMill.Execute("FORM CANCEL CREATEHOLE");


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Hole feature creation completed ");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[HoleFeatureCreationOperation ERROR] {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }
    }
}


