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
    public class StockModel
    {

         private  readonly PMAutomation _powerMill;
        public StockModel(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }
        public void Create()
        {
            _powerMill.Execute("GRAPHICS LOCK");
            _powerMill.Execute("DIALOGS MESSAGE OFF");
            _powerMill.Execute("DIALOGS ERROR OFF");
            _powerMill.Execute("CREATE STOCKMODEL ; FORM STOCKMODEL");
            _powerMill.Execute("RENAME STOCKMODEL # \"STOCK\"");
            _powerMill.Execute("EDIT STOCKMODEL \"STOCK\" TOLERANCE .1");
            _powerMill.Execute("EDIT STOCKMODEL \"STOCK\" STEPOVER .8");
            _powerMill.Execute("EDIT STOCKMODEL \"STOCK\" RESTTHICKNESS .2");
            _powerMill.Execute("EDIT STOCKMODEL \"STOCK\" REAPPLYFROMGUI");
            _powerMill.Execute("EDIT STOCKMODEL \"STOCK\" BLOCK ;");

            _powerMill.Execute("EDIT STOCKMODEL \"STOCK\" CALCULATE_STATE INDEXED_STATE 0");
            _powerMill.Execute("EXPLORER SELECT StockModel \"StockModel\\STOCK\" NEW");
            _powerMill.Execute("EDIT STOCKMODEL \"STOCK\" SHADING DEFAULT");
            _powerMill.Execute("UNDRAW StockModel \"STOCK\"");
            _powerMill.Execute("BLOCK RESET");
            _powerMill.Execute("GRAPHICS UNLOCK");
        }
    }
}
