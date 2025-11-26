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
    public  class PatterFromBlock
    {
        private readonly PMAutomation _powerMill;
        public PatterFromBlock( PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }
        public void Run()
        {
            _powerMill.Execute("GRAPHICS LOCK");
            _powerMill.Execute("REAL xMinus = $toolpath.Block.Limits.XMin");
            _powerMill.Execute("REAL yMinus = $toolpath.Block.Limits.YMin");
            _powerMill.Execute("REAL xPlus = $toolpath.Block.Limits.XMax");
            _powerMill.Execute("REAL yPlus = $toolpath.Block.Limits.YMax");

            _powerMill.Execute("CREATE PATTERN");
            _powerMill.Execute("EDIT PATTERN");
            _powerMill.Execute("CURVEEDITOR NOGUI START");
            _powerMill.Execute("CURVEEDITOR MODE RECTANGLE");

            _powerMill.Execute("MODE COORDINPUT COORDINATES $xMinus $yMinus 0");
            _powerMill.Execute("MODE COORDINPUT COORDINATES $xPlus $yPlus 0");

            _powerMill.Execute("CURVEEDITOR FINISH ACCEPT");
            _powerMill.Execute("GRAPHICS UNLOCK");
        }
    }
}
