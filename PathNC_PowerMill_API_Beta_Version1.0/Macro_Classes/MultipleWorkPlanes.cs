using System;
using Autodesk.ProductInterface.PowerMILL;

namespace MoldAutomation.Macros
{
    [Obsolete]
    public class WorkplaneCreatorNoFunctions
    {
        private readonly PMAutomation _powerMill;

        public WorkplaneCreatorNoFunctions(PMAutomation powerMill)
        {
            _powerMill = powerMill ?? throw new ArgumentNullException(nameof(powerMill));
        }

        public void Run()
        {
            var macro = @"
MACRO BLOCK BEGIN
RESET LOCALVARS
STRING prompt = ""Create another workplane?""
BOOL b = 1

WHILE $b {
    // Change the workplane naming scheme
    EDIT ENTATTRIBUTE TEMPLATE WORKPLANE 'OP'

    // Run your macro
    DELETE SELECTION
    FORM BLOCK
    EDIT BLOCK RESET
    BLOCK ACCEPT

    MODE WORKPLANE_CREATE ; INTERACTIVE BLOCK
    MACRO PAUSE ""Select point for new Workplane""

    ACTIVATE WORKPLANE #
    STRING newWp = ENTITY('workplane','').Name

    MODE WORKPLANE_EDIT START ;
    MODE WORKPLANE_EDIT SWAP_AXES
    MACRO PAUSE ""Change Direction for new Workplane""
    MODE WORKPLANE_EDIT FINISH ACCEPT

    DELETE SELECTION

    FORM BLOCK
    EDIT BLOCK COORDINATE NAMED
    EDIT BLOCK NAMEDWORKPLANE ;
    EDIT BLOCK TOLERANCE ""0.01""
    EDIT BLOCK RESET
    BLOCK ACCEPT

    // Reset the naming scheme
    EDIT ENTATTRIBUTE TEMPLATE WORKPLANE ;

    // ask to create another
    $b = QUERY $prompt
}
MACRO BLOCK END
";
            _powerMill.Execute(macro);
        }
    }
}
