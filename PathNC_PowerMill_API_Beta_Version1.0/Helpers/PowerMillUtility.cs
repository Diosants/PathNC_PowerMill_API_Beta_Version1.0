using Autodesk.ProductInterface;
using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MoldAutomation.Helpers;

namespace MoldAutomation.Helpers
{
    [Obsolete]
    public class PowerMillUtility
    {
        private readonly PMAutomation _powerMill;
        public PowerMillUtility(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }
    }
    [Obsolete]

    public static class PowerMillExtensions
    {
        public static void Mydialog(this PMAutomation powerMill)
        {
          
           // powerMill.Execute("DIALOGS ERROR OFF");

        }

        [Obsolete]
        public static void ViewSetup(this PMAutomation powerMill)
        {
            powerMill.Execute("ROTATE TRANSFORM ISO1");
            powerMill.Execute("DELETE SCALE");
            powerMill.Execute("VIEW MODEL; SHADE RAINBOW");

        }

        [Obsolete]
        //  ListBox message
        public static void Log(ListBox logBox, string message)
        {

            if (logBox == null) return;
            logBox.Dispatcher.Invoke(() =>
            {
                logBox.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");

            });

        }
        public static void ImportModel(PMAutomation powerMill, string modelPath)
        {
            if (powerMill == null) throw new ArgumentNullException(nameof(powerMill));
            PMProject session = powerMill.ActiveProject;
            if (session == null)
            {
                MessageBox.Show("No active PowerMill project found. Please open a project first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            session.Initialise();
            Autodesk.FileSystem.File model = new Autodesk.FileSystem.File(@"C:\Users\dcard\OneDrive\Desktop\DiogenixCAM\P1Plate-sem-filete.x_t");
            PMModel myModel = session.Models.CreateModel(model);
        }

        [Obsolete]
        // Calculate Block
        public static void CalculateBlock(PMAutomation powerMill)
        {

            if (powerMill == null) throw new ArgumentException(nameof(powerMill));
            // Block reset & preparation
            powerMill.Execute("FORM BLOCK");
            powerMill.Execute("BLOCK CANCEL");
            powerMill.Execute("BLOCK CANCELLED");
            powerMill.Execute("EDIT MODEL ALL SELECT ALL");
            powerMill.Execute("FORM BLOCK");
            powerMill.Execute("EDIT BLOCK RESET");
            powerMill.Execute("EDIT BLOCK ZMIN LOCK");
            powerMill.Execute("EDIT BLOCK ZMAX LOCK");
            powerMill.Execute("BLOCK ACCEPT");

        }

        [Obsolete]

        // Create Stock Model
        public static void CreateStockModel(PMAutomation powerMill)
        {

            if (powerMill == null) throw new ArgumentException(nameof(powerMill));

            {
               // ExtendBlock(powerMill, 5.0);

                powerMill.Execute("GRAPHICS LOCK");
                powerMill.Execute("DIALOGS MESSAGE OFF");
                powerMill.Execute("DIALOGS ERROR OFF");
                powerMill.Execute("CREATE STOCKMODEL ; FORM STOCKMODEL");
                powerMill.Execute("RENAME STOCKMODEL # \"STOCK\"");
                powerMill.Execute("EDIT STOCKMODEL \"STOCK\" TOLERANCE .1");
                powerMill.Execute("EDIT STOCKMODEL \"STOCK\" STEPOVER .8");
                powerMill.Execute("EDIT STOCKMODEL \"STOCK\" RESTTHICKNESS .2");
                powerMill.Execute("EDIT STOCKMODEL \"STOCK\" REAPPLYFROMGUI");
                powerMill.Execute("EDIT STOCKMODEL \"STOCK\" BLOCK ;");

                powerMill.Execute("EDIT STOCKMODEL \"STOCK\" CALCULATE_STATE INDEXED_STATE 0");
                powerMill.Execute("EXPLORER SELECT StockModel \"StockModel\\STOCK\" NEW");
                powerMill.Execute("EDIT STOCKMODEL \"STOCK\" SHADING DEFAULT");
                powerMill.Execute("UNDRAW StockModel \"STOCK\"");
                powerMill.Execute("BLOCK RESET");
                powerMill.Execute("GRAPHICS UNLOCK");
            }
        }

        [Obsolete]
        // Reset Block
        public static void ResetBlock(PMAutomation powerMill)
        {
            if (powerMill == null) throw new ArgumentException(nameof(powerMill));

            {

                powerMill.Execute("GRAPHICS LOCK");
                powerMill.Execute("DIALOGS MESSAGE OFF");
                powerMill.Execute("FORM BLOCK");
                powerMill.Execute("BLOCK CANCEL");
                powerMill.Execute("BLOCK CANCELLED");
                powerMill.Execute("EDIT MODEL ALL SELECT ALL");
                powerMill.Execute("FORM BLOCK");
                powerMill.Execute("EDIT BLOCK RESET");
                powerMill.Execute("EDIT BLOCK ZMIN LOCK");
                powerMill.Execute("EDIT BLOCK ZMAX LOCK");
                powerMill.Execute("BLOCK ACCEPT");


            }

        }
        [Obsolete]

        // Verify Collision
        public static void VerifyCollision(PMAutomation powerMill)
        {


            if (powerMill ==null) throw new ArgumentException(nameof(powerMill));

            powerMill.Execute("GRAPHICS LOCK");
            powerMill.Execute("DIALOGS MESSAGE OFF");
            powerMill.Execute("DIALOGS ERROR OFF");
            powerMill.Execute("EDIT COLLISION TYPE GOUGE");
            powerMill.Execute("EDIT COLLISION SPLIT_TOOLPATH Y");
            powerMill.Execute("EDIT COLLISION APPLY");

            powerMill.Execute("EDIT COLLISION TYPE COLLISION");
            powerMill.Execute("EDIT COLLISION SHANK_CLEARANCE \"0.005\"");
            powerMill.Execute("EDIT COLLISION HOLDER_CLEARANCE \"0.075\"");
            powerMill.Execute("EDIT COLLISION SPLIT_TOOLPATH Y");
            powerMill.Execute("EDIT COLLISION DEPTH Y");
            powerMill.Execute("EDIT COLLISION ADJUST_TOOL Y");
            powerMill.Execute("EDIT COLLISION APPLY");

            powerMill.Execute("FORM DATUM");
            powerMill.Execute("EDIT TOOLPATH START TYPE POINT_SAFE");
            powerMill.Execute("RESET TOOLPATH START_END");
            powerMill.Execute("DATUM ACCEPT");
            powerMill.Execute("GRAPHICS UNLOCK");
        }

        [Obsolete]
        public static void Simulation(PMAutomation powerMill, int simulationSpeed = 100)
        {
            if (powerMill == null) throw new ArgumentException(nameof(powerMill));

            powerMill.Execute("GRAPHICS LOCK");
            powerMill.Execute("DIALOGS MESSAGE OFF");
            powerMill.Execute("DIALOGS ERROR OFF");
            // Open simulation tab
            powerMill.Execute("FORM RIBBON TAB \"Simulation\"");

            // Start ViewMill simulation
            powerMill.Execute("SIMULATE VIEWMILL START");
            powerMill.Execute("SIMULATE VIEWMILL SHADING FIXEDDIRECTION");
            powerMill.Execute("SIMULATE VIEWMILL SHADING SHINY");
            powerMill.Execute("REFRESH VIEWMILL RESIZEVIEW");

            // Set speed and playback settings
            powerMill.Execute($"EDIT PAR 'Simulation.Speed' {simulationSpeed}");
            powerMill.Execute("EDIT PAR 'Powermill.Simulation.Issues.PlaybackSetting' 'never'");

            // Simulate each toolpath in the folder
            powerMill.Execute(@"
                FOREACH tp IN folder('toolpath') {
                    ACTIVATE TOOLPATH $tp.Name
                    SIMULATE TOOLPATH $tp.Name TOOLBAR SIMULATION RAISE
                    SIMULATE PLAY
                }
            ");

            // Cleanup after simulation
            powerMill.Execute("UNDRAW TOOLPATH ALL");
            powerMill.Execute("DEACTIVATE TOOLPATH");
            powerMill.Execute("DEACTIVATE WORKPLANE");
            powerMill.Execute("FORM MACHINEISSUES");
            powerMill.Execute("GRAPHICS UNLOCK");
        }

        [Obsolete]

        public static void ExtendBlock(PMAutomation powerMill, double extendValue)
        {
            if (powerMill == null) throw new ArgumentException(nameof(powerMill));

            powerMill.Execute("GRAPHICS LOCK");
            powerMill.Execute("EDIT TPPAGE SWWorkplane");
            powerMill.Execute("EDIT TPPAGE SWBlock");
            powerMill.Execute("EDIT BLOCK ZMAX UNLOCK");
            powerMill.Execute("EDIT BLOCK ZMAX \"2\"");
            powerMill.Execute("EDIT BLOCK ZMAX LOCK");
            powerMill.Execute($"EDIT BLOCK RESETLIMIT \"{extendValue}\"");
            powerMill.Execute("EDIT BLOCK RESET");
            powerMill.Execute("BLOCK ACCEPT");
        }

        [Obsolete]

        public static void SetupSheet(PMAutomation powerMill)
        {

            powerMill.Mydialog();
            // Ask user to enter NC program name
            powerMill.Execute("STRING $ncName = INPUT \"Enter NC Program Name\"");
            powerMill.Execute("IF $ncName == \"\" { $ncName = \"setup sheet\" }");

            // Clean up old data
            powerMill.Execute("UNDRAW TOOLPATH ALL");
            powerMill.Execute("UNDRAW TOOL ALL");
            powerMill.Execute("DELETE NCPROGRAM ALL");

            // Create NC program and rename
            powerMill.Execute("CREATE NCPROGRAM ; EDIT NCPROGRAM ; QUIT FORM NCTOOLPATH");
            powerMill.Execute("RENAME NCPROGRAM \"1\" $ncName");

            // Append all toolpaths
            powerMill.Execute("NCTOOLPATH APPLY");
            powerMill.Execute("EDIT NCPROGRAM ; APPEND TOOLPATH ALL");
            powerMill.Execute("NCTOOLPATH APPLY");
            powerMill.Execute("NCTOOLPATH ACCEPT FORM ACCEPT NCTOOLPATHLIST FORM ACCEPT NCTOOLLIST");

            // Setup Sheet Configuration
            powerMill.Execute("FORM NCSETUPSHEETS");
            powerMill.Execute("SETUPSHEETS TEMPLATE ENABLE NCPROGRAM_HEADER");
            powerMill.Execute("SETUPSHEETS TEMPLATE DISABLE NCPROGRAM_HEADER");
            powerMill.Execute("SETUPSHEETS TEMPLATE ENABLE TOOLPATH");
            powerMill.Execute("SETUPSHEETS TEMPLATE DISABLE TOOLPATH");
            powerMill.Execute("SETUPSHEETS TEMPLATE ENABLE NCPROGRAM_SUMMARY");
            powerMill.Execute("SETUPSHEETS TEMPLATE DISABLE NCPROGRAM_SUMMARY");
            powerMill.Execute("SETUPSHEETS TEMPLATE ENABLE PROJECT_HEADER");
            powerMill.Execute("SETUPSHEETS TEMPLATE DISABLE PROJECT_HEADER");
            powerMill.Execute("SETUPSHEETS TEMPLATE DISABLE PROJECT_SUMMARY");
            powerMill.Execute("SETUPSHEETS TEMPLATE ENABLE PROJECT_SUMMARY");

            // Load external HTML template
            powerMill.Execute($"EDIT NCPROGRAM PREFERENCES SETUPSHEETS PROJECT_SUMMARY FILEOPEN \"\""); // put a valid path here

            // Finalize setup sheet
            powerMill.Execute("NCSETUPSHEETS ACCEPT");
            powerMill.Execute("KEEP SNAPSHOT PROJECT CURRENT");
            powerMill.Execute("TEXTINFO ACCEPT");
            powerMill.Execute("KEEP SETUPSHEETS NCPROGRAM ALL");
            powerMill.Execute("TEXTINFO ACCEPT");
            powerMill.Execute("SPLITTER TABEXPLORE");
            powerMill.Execute("GRAPHICS UNLOCK");
        }

        [Obsolete]

        public static void CreateCompoundHole(PMAutomation powerMill)
        {
            if (powerMill == null) throw new ArgumentNullException(nameof(powerMill));
            try
            {
                powerMill.Execute("GRAPHICS LOCK");
                powerMill.Execute("DIALOGS MESSAGE OFF");
                powerMill.Execute("DIALOGS ERROR OFF");
                powerMill.DialogsOff();
                powerMill.Execute("EDIT MODEL ALL SELECT ALL");
                powerMill.Execute("CREATE FEATURESET ;");
                powerMill.Execute("EXPLORER SELECT Featureset \"#\" NEW");
                powerMill.Execute("EDIT FEATURECREATE TYPE HOLE EDIT FEATURECREATE CIRCULAR ON EDIT FEATURECREATE FILTER HOLES EDIT FEATURECREATE TOPDEFINE ABSOLUTE EDIT FEATURECREATE BOTTOMDEFINE ABSOLUTE FORM CANCEL FEATURE FORM CREATEHOLE");
                powerMill.Execute("EDIT FEATURECREATE HOLES COMPOUND OFF");
                powerMill.Execute("EDIT FEATURECREATE HOLES COMPOUND ON");
                powerMill.Execute("EDIT FEATURECREATE HOLES FIXED OFF");
                powerMill.Execute("EDIT FEATURECREATE HOLES FIXED ON");
                powerMill.Execute("EDIT FEATURECREATE HOLES DIRECTION BOTH");
                powerMill.Execute("EDIT FEATURECREATE HOLES DIRECTION DOWN");
                powerMill.Execute("EDIT FEATURECREATE NAME  Compound hole");
                powerMill.Execute("EDIT FEATURECREATE CREATEHOLES");
                powerMill.Execute("FORM CANCEL CREATEHOLE");
                powerMill.Execute("GRAPHICS UNLOCK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[HoleFeatureCreationOperation ERROR] {ex.Message}");
                Console.ResetColor();
            }
        }

        [Obsolete("Use CreateHoles instead.")]
        public static void CreateHoles(PMAutomation powerMill)
        {
            if (powerMill == null) throw new ArgumentNullException(nameof(powerMill));
            try
            {
                powerMill.Execute("GRAPHICS LOCK");
                powerMill.Execute("DIALOGS MESSAGE OFF");
                powerMill.Execute("DIALOGS ERROR OFF");
                powerMill.Execute("EDIT MODEL ALL SELECT ALL");
                powerMill.Execute("EDIT FEATURECREATE TYPE HOLE");
                powerMill.Execute("EDIT FEATURECREATE CIRCULAR ON");
                powerMill.Execute("EDIT FEATURECREATE FILTER HOLES");
                powerMill.Execute("EDIT FEATURECREATE TOPDEFINE ABSOLUTE");
                powerMill.Execute("EDIT FEATURECREATE BOTTOMDEFINE ABSOLUTE");
                powerMill.Execute("FORM CANCEL FEATURE");
                // Create detected holes
                powerMill.Execute("FORM CREATEHOLE");
                powerMill.Execute("EDIT FEATURECREATE NAME");
                powerMill.Execute("EDIT FEATURECREATE CREATEHOLES");
                powerMill.Execute("FORM CANCEL CREATEHOLE");
                powerMill.Execute("GRAPHICS UNLOCK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[CreateHolesOperation ERROR] {ex.Message}");
                Console.ResetColor();
            }
        }
        [Obsolete]
        public static void PatternFromBlock(PMAutomation powerMill)
        {
            if (powerMill == null) throw new ArgumentNullException(nameof(powerMill));


            powerMill.Execute("GRAPHICS LOCK");
            powerMill.Execute("REAL xMinus = $toolpath.Block.Limits.XMin");
            powerMill.Execute("REAL yMinus = $toolpath.Block.Limits.YMin");
            powerMill.Execute("REAL xPlus = $toolpath.Block.Limits.XMax");
            powerMill.Execute("REAL yPlus = $toolpath.Block.Limits.YMax");

            powerMill.Execute("CREATE PATTERN");
            powerMill.Execute("EDIT PATTERN");
            powerMill.Execute("CURVEEDITOR NOGUI START");
            powerMill.Execute("CURVEEDITOR MODE RECTANGLE");

            powerMill.Execute("MODE COORDINPUT COORDINATES $xMinus $yMinus 0");
            powerMill.Execute("MODE COORDINPUT COORDINATES $xPlus $yPlus 0");

            powerMill.Execute("CURVEEDITOR FINISH ACCEPT");
            powerMill.Execute("GRAPHICS UNLOCK");


        }
    }
    [Obsolete]
    public static class WorkplaneService
    {
        /// <summary>
        /// Creates one workplane interactively (user selects a point and swaps axes),
        /// resets/updates the block, and restores the naming template.
        /// </summary>
        /// <param name="pm">Connected PMAutomation instance.</param>
        /// <param name="nameTemplate">Temporary naming template for the new workplane (e.g., "OP").</param>
        /// <param name="blockTolerance">Block tolerance used when rebuilding from the named WP.</param>
        /// <param name="resetBlockBefore">If true, reset/accept block before interactive create.</param>
        /// <param name="rebuildBlockFromNamed">If true, rebuilds block from the new named workplane.</param>

        [Obsolete("Use CreateWorkplaneInteractive instead.")]
        public static void CreateWorkplaneInteractive(
            PMAutomation pm,
            string nameTemplate = "OP",
            double blockTolerance = 0.01,
            bool resetBlockBefore = true,
            bool rebuildBlockFromNamed = true)
        {
            if (pm == null) throw new ArgumentNullException(nameof(pm));

            // Keep dialogs ON because we need interactive picks (MACRO PAUSE)
            // pm.DialogsOff(); // do NOT disable here

            // Set naming template for the new WP
            pm.Execute($"EDIT ENTATTRIBUTE TEMPLATE WORKPLANE '{nameTemplate}'");

            // Optional pre-reset of block
            if (resetBlockBefore)
            {
                pm.Execute("DELETE SELECTION");
                pm.Execute("FORM BLOCK");
                pm.Execute("EDIT BLOCK RESET");
                pm.Execute("BLOCK ACCEPT");
            }

            // Interactive creation on the block
            pm.Execute("MODE WORKPLANE_CREATE ; INTERACTIVE BLOCK");
            pm.Execute("MACRO PAUSE \"Select point for new Workplane\"");

            // Activate the just-created workplane (“#” = last)
            pm.Execute("ACTIVATE WORKPLANE #");

            // Let user adjust axes, then accept
            pm.Execute("MODE WORKPLANE_EDIT START ;");
            pm.Execute("MODE WORKPLANE_EDIT SWAP_AXES");
            pm.Execute("MACRO PAUSE \"Change Direction for new Workplane\"");
            pm.Execute("MODE WORKPLANE_EDIT FINISH ACCEPT");

            // Optional: rebuild block from the newly named WP
            if (rebuildBlockFromNamed)
            {
                pm.Execute("DELETE SELECTION");
                pm.Execute("FORM BLOCK");
                pm.Execute("EDIT BLOCK COORDINATE NAMED");
                pm.Execute("EDIT BLOCK NAMEDWORKPLANE ;");
                pm.Execute($"EDIT BLOCK TOLERANCE \"{blockTolerance}\"");
                pm.Execute("EDIT BLOCK RESET");
                pm.Execute("BLOCK ACCEPT");
            }

            // Restore default naming template
            pm.Execute("EDIT ENTATTRIBUTE TEMPLATE WORKPLANE ;");
        }

        /// <summary>
        /// Creates N workplanes interactively in a row (user clicks for each).
        /// </summary>
        /// [obsolete]
        public static void CreateMultipleWorkplanesInteractive(
            PMAutomation pm,
            int count,
            string nameTemplate = "OP",
            double blockTolerance = 0.01,
            bool resetBlockBeforeFirst = true,
            bool rebuildBlockFromNamedEach = true)
        {
            if (pm == null) throw new ArgumentNullException(nameof(pm));
            if (count <= 0) return;

            for (int i = 0; i < count; i++)
            {
                CreateWorkplaneInteractive(
                    pm,
                    nameTemplate,
                    blockTolerance,
                    resetBlockBefore: resetBlockBeforeFirst && i == 0,
                    rebuildBlockFromNamed: rebuildBlockFromNamedEach
                );
            }
        }
    }
    [Obsolete]
    // Add this helper class to fix CS0103: The name 'MacroRunner' does not exist in the current context
    public static class MacroRunner
    {
        public static void Run(PMAutomation powerMill, string macroPath)
        {
            if (powerMill == null) throw new ArgumentNullException(nameof(powerMill));
            if (string.IsNullOrWhiteSpace(macroPath)) throw new ArgumentException("Macro path cannot be null or empty.", nameof(macroPath));
            powerMill.RunMacro(macroPath);
        }

    public static void ConnectPowerMill(PMAutomation powerMill)
        {
            if (powerMill == null) throw new ArgumentNullException(nameof(powerMill));
            PMProject session = powerMill.ActiveProject;
            if (session == null)
            {
                MessageBox.Show("No active PowerMill project found. Please open a project first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                MessageBox.Show("Connected to PowerMill successfully!");
            }
        }

      

    }
}









