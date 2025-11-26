using Autodesk.ProductInterface.PowerMILL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMill_API_Version1._0.Macros
{
    public class SaveProject
    {
        private readonly PMAutomation _powerMill;

        public SaveProject(PMAutomation powerMill)
        {

            _powerMill = powerMill;
        }

        public void Run()
        {


            // Extract model name and first 8 characters
            _powerMill.Execute("STRING $MODELNAME = FOLDER('model')[0].Name");
            _powerMill.Execute("STRING $SAVENAME = SUBSTRING($MODELNAME , 0 , 7)");
            _powerMill.Execute(@"STRING $SAVEDIRECTORY = ""C:\CAMProjects\PowerMill\Desktop\"" + $SAVENAME");

            // Save project (Save As if folder doesn’t exist, else Save)
            _powerMill.Execute("IF NOT DIR_EXISTS($SAVEDIRECTORY) {");
            _powerMill.Execute("    PROJECT SAVE AS $SAVEDIRECTORY");
            _powerMill.Execute("} ELSE {");
            _powerMill.Execute("    PROJECT SAVE");
            _powerMill.Execute("}");

            // Create NCProgram if it doesn’t exist
            _powerMill.Execute("IF NOT ENTITY_EXISTS('NCPROGRAM' , $SAVENAME) {");
            _powerMill.Execute("CREATE NCPROGRAM");
            _powerMill.Execute("EDIT NCPROGRAM");
            _powerMill.Execute("QUIT FORM NCTOOLPATH");
            _powerMill.Execute("RENAME NCPROGRAM # $SAVENAME");
            _powerMill.Execute("NCTOOLPATH CANCEL FORM ACCEPT NCTOOLPATHLIST FORM ACCEPT NCTOOLLIST FORM ACCEPT PROBINGNCOPTS");
            _powerMill.Execute("}");

        }

    }
}
