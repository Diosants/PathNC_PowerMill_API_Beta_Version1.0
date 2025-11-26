using System;
using Autodesk.ProductInterface.PowerMILL;


namespace P1PlateStandard.Machining
{
    [Obsolete]
    public class CompoundHole
    {
        private readonly PMAutomation _powerMill;

        public CompoundHole(PMAutomation powerMill)
        {
            _powerMill=powerMill;
        }

        public void Run()
        {

            try
            {
                _powerMill.DialogsOff();
                _powerMill.Execute("EDIT MODEL ALL SELECT ALL");
                _powerMill.Execute("CREATE FEATURESET ;");
                _powerMill.Execute("EXPLORER SELECT Featureset \"#\" NEW");
                _powerMill.Execute("EDIT FEATURECREATE TYPE HOLE EDIT FEATURECREATE CIRCULAR ON EDIT FEATURECREATE FILTER HOLES EDIT FEATURECREATE TOPDEFINE ABSOLUTE EDIT FEATURECREATE BOTTOMDEFINE ABSOLUTE FORM CANCEL FEATURE FORM CREATEHOLE");
                _powerMill.Execute("EDIT FEATURECREATE HOLES COMPOUND OFF");
                _powerMill.Execute("EDIT FEATURECREATE HOLES COMPOUND ON");
                _powerMill.Execute("EDIT FEATURECREATE HOLES FIXED OFF");
                _powerMill.Execute("EDIT FEATURECREATE HOLES FIXED ON");
                _powerMill.Execute("EDIT FEATURECREATE HOLES DIRECTION BOTH");
                _powerMill.Execute("EDIT FEATURECREATE HOLES DIRECTION DOWN");
                _powerMill.Execute("EDIT FEATURECREATE NAME  Compound hole");
                _powerMill.Execute("EDIT FEATURECREATE CREATEHOLES");
                _powerMill.Execute("FORM CANCEL CREATEHOLE");

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

