using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;

namespace MoldAutomation.Macros
{
    [Obsolete]
    public  class GetHoles
    {

        private readonly PMAutomation _powerMill;

        public GetHoles(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }

        public PMAutomation PowerMill => _powerMill;

        public void Run()
        {

            PowerMill.Mydialog();
            PowerMill.Execute("EDIT  MODEL ALL SELECT ALL");
            PowerMill.Execute("EDIT  FEATURECREATE TYPE HOLE");
            PowerMill.Execute("EDIT FEATURECREATE CIRCULAR ON");
            PowerMill.Execute("EDIT FEATURECREATE FILTER HOLES");
            PowerMill.Execute("EDIT FEATURECREATE TOPDEFINE ABSOLUTE");
            PowerMill.Execute("EDIT FEATURECREATE BOTTOMDEFINE ABSOLUTE");
            PowerMill.Execute("FORM CANCEL FEATURE");
            // Create detected holes
            PowerMill.Execute("FORM CREATEHOLE");
            PowerMill.Execute("EDIT FEATURECREATE NAME");
            PowerMill.Execute("EDIT FEATURECREATE CREATEHOLES");
            PowerMill.Execute("FORM CANCEL CREATEHOLE");
        }
    }
}
