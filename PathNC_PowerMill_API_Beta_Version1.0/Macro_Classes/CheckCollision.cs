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
    [Obsolete("This class is deprecated and may not be supported in future versions.")]
    public class CheckCollision
    {
        private readonly PMAutomation _powerMill;
        public CheckCollision(PMAutomation powerMill)
        {
            _powerMill = powerMill?? throw new ArgumentNullException(nameof(powerMill), "PowerMill automation instance cannot be null.");


        }
        public void Run()
        {
            _powerMill.Execute("GRAPHICS LOCK");
            _powerMill.Execute("DIALOGS MESSAGE OFF");
            _powerMill.Execute("DIALOGS ERROR OFF");
            _powerMill.Execute("EDIT COLLISION TYPE GOUGE");
            _powerMill.Execute("EDIT COLLISION SPLIT_TOOLPATH Y");
            _powerMill.Execute("EDIT COLLISION APPLY");

            _powerMill.Execute("EDIT COLLISION TYPE COLLISION");
            _powerMill.Execute("EDIT COLLISION SHANK_CLEARANCE \"0.005\"");
            _powerMill.Execute("EDIT COLLISION HOLDER_CLEARANCE \"0.075\"");
            _powerMill.Execute("EDIT COLLISION SPLIT_TOOLPATH Y");
            _powerMill.Execute("EDIT COLLISION DEPTH Y");
            _powerMill.Execute("EDIT COLLISION ADJUST_TOOL Y");
            _powerMill.Execute("EDIT COLLISION APPLY");

            _powerMill.Execute("FORM DATUM");
            _powerMill.Execute("EDIT TOOLPATH START TYPE POINT_SAFE");
            _powerMill.Execute("RESET TOOLPATH START_END");
            _powerMill.Execute("DATUM ACCEPT");
            _powerMill.Execute("GRAPHICS UNLOCK");

        }
    }
}
