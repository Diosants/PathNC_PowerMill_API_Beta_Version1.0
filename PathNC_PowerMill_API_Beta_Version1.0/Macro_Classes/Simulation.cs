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
    public  class Simulation
    {

        internal readonly PMAutomation _powerMill;
        public Simulation(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }
        public void Run(int simulationSpeed = 100)
        {
               _powerMill.Mydialog();
            // Open simulation tab
            _powerMill.Execute("FORM RIBBON TAB \"Simulation\"");

            // Start ViewMill simulation
            _powerMill.Execute("SIMULATE VIEWMILL START");
            _powerMill.Execute("SIMULATE VIEWMILL SHADING FIXEDDIRECTION");
            _powerMill.Execute("SIMULATE VIEWMILL SHADING RAINBOW");
            _powerMill.Execute("REFRESH VIEWMILL RESIZEVIEW");

            // Set speed and playback settings
            _powerMill.Execute($"EDIT PAR 'Simulation.Speed' {simulationSpeed}");
            _powerMill.Execute("EDIT PAR 'Powermill.Simulation.Issues.PlaybackSetting' 'never'");

            // Simulate each toolpath in the folder
            _powerMill.Execute(@"
            FOREACH tp IN folder('toolpath') {
            ACTIVATE TOOLPATH $tp.Name
            SIMULATE TOOLPATH $tp.Name TOOLBAR SIMULATION RAISE
            SIMULATE PLAY
                        }
                        ");

            // Cleanup after simulation
            _powerMill.Execute("UNDRAW TOOLPATH ALL");
            _powerMill.Execute("DEACTIVATE TOOLPATH");
            _powerMill.Execute("DEACTIVATE WORKPLANE");
            _powerMill.Execute("FORM MACHINEISSUES");
            _powerMill.Execute("GRAPHICS UNLOCK");
        }
    }
}

