using Autodesk.ProductInterface.PowerMILL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API_PowerMill_Version1._1;

namespace API_PowerMill_Version1._1.Helpers
{
    public class BlockForm
    {

        private readonly PMAutomation _powerMill;

        public BlockForm(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }


      public static void DeleteExistingBlocks(PMAutomation powerMill)
        {
            powerMill.Execute("FORM BLOCK");
            powerMill.Execute("DELETE BLOCK");
            powerMill.Execute("EDIT BLOCK XLEN UNLOCK");
            powerMill.Execute("EDIT BLOCK YLEN UNLOCK");
            powerMill.Execute("EDIT BLOCK ZLEN UNLOCK");
            powerMill.Execute("EDIT BLOCK RESETLIMIT \"0\"");
            powerMill.Execute("BLOCK ACCEPT");

        }

        public static void CreateBlockFromPattern(PMAutomation powerMill)
        {

            // ========================================
            //   BLOCK CREATION AUTOMATION IN C#
            // ========================================

            string[] blockTypeOptions = { "Box", "Cylinder" };

            // Ask the user which block type to create
            string blockTypeChoice = Microsoft.VisualBasic.Interaction.InputBox(
                "Select Block Type:\n\n0 - Box\n1 - Cylinder",
                "Block Type Selection",
                "0");

            int blockTypeSelected;
            if (!int.TryParse(blockTypeChoice, out blockTypeSelected) || blockTypeSelected < 0 || blockTypeSelected > 1)
            {
                System.Windows.MessageBox.Show("Invalid choice. Defaulting to Box.");
                blockTypeSelected = 0;
            }

            // Get user parameters
            string expansionInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Block Expansion (must be greater than tool radius)", "Block Expansion", "5");
            string depthInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Block Depth (must be negative)", "Block Depth", "-100");

            double blockExpansion = double.TryParse(expansionInput, out double e) ? e : 5;
            double blockDepth = double.TryParse(depthInput, out double d) ? d : -100;

            // --- EXECUTE COMMANDS IN POWERMILL ---

            powerMill.Execute("FORM BLOCK");
            powerMill.Execute("DELETE BLOCK");

            // --- CHOOSE BLOCK TYPE ---
            if (blockTypeSelected == 0)
                powerMill.Execute("EDIT BLOCKTYPE BOX");
            else
                powerMill.Execute("EDIT BLOCKTYPE CYLINDER");

            // --- DEFINE PARAMETERS ---
            powerMill.Execute("EDIT BLOCK XLEN UNLOCK");
            powerMill.Execute("EDIT BLOCK YLEN UNLOCK");
            powerMill.Execute("EDIT BLOCK ZLEN UNLOCK");
            powerMill.Execute("EDIT BLOCK COORDINATE WORKPLANE");
            powerMill.Execute("EDIT BLOCK LIMITTYPE PATTERN");
            powerMill.Execute("EDIT BLOCK RESET");
            powerMill.Execute("EDIT BLOCK ZMIN LOCK");
            powerMill.Execute("EDIT BLOCK ZMAX LOCK");

            powerMill.Execute($"EDIT BLOCK RESETLIMIT \"{blockExpansion}\"");
            powerMill.Execute("EDIT BLOCK RESET");
            powerMill.Execute("EDIT BLOCK ZMIN UNLOCK");
            powerMill.Execute($"EDIT BLOCK ZMIN \"{blockDepth}\"");
            powerMill.Execute("EDIT BLOCK ZMIN LOCK");

            powerMill.Execute("BLOCK ACCEPT");

            // --- FEEDBACK MESSAGE ---
            System.Windows.MessageBox.Show(
                $"Block successfully created!\n\n" +
                $"Type: {blockTypeOptions[blockTypeSelected]}\n" +
                $"Expansion: {blockExpansion} mm\n" +
                $"Depth: {blockDepth} mm",
                "PowerMill Block Setup Complete",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information
            );



        }


    }
}

