using Autodesk.ProductInterface.PowerMILL;
using PowerMill_API_Version1._0.Helpers;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace API_PowerMill_Version1._1.Helpers
{
    public static class ToolpathManager
    {
        /// <summary>
        /// Calculates the currently active toolpath using the given feature form.
        /// </summary>
        /// <param name="_powerMill">The active PowerMill automation session.</param>
        /// <param name="featureFormName">The feature form name (e.g. "SFPocketAreaClear").</param>
        /// <param name="featureGroupIndex">The feature group index (default is 1).</param>
        public static void CalculateActiveToolpath(PMAutomation _powerMill, string featureFormName, int featureGroupIndex = 1)
        {
            try
            {

                _powerMill.RunMacro("ACTIVATE TOOLPATH \"#\" FORM TOOLPATH");
                _powerMill.RunMacro("EDIT TPPAGE SWFeatures");
                _powerMill.RunMacro("RESET LOCALVARS");
                _powerMill.RunMacro("STRING selectedToolName = INPUT ENTITY TOOL \"Please select a tool from the list.\"");
                _powerMill.RunMacro("ACTIVATE TOOL $selectedToolName");
                _powerMill.RunMacro($"ACTIVATE FEATUREGROUP \"{featureGroupIndex}\"");
                _powerMill.RunMacro("EDIT TOOLPATH \"#\" QUEUE");
                _powerMill.RunMacro($"FORM ACCEPT {featureFormName}");
                _powerMill.RunMacro("BATCH PROCESS");

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $" Failed to calculate toolpath with feature '{featureFormName}': {ex.Message}", ex);
            }
        }
        public static void EditToolpath(PMAutomation _powerMill, string featureFormName)
        {
            // Select Tool
            _powerMill.RunMacro("ACTIVATE TOOLPATH \"#\" FORM TOOLPATH");
            _powerMill.RunMacro("RESET LOCALVARS");
            _powerMill.RunMacro("STRING selectedToolName = INPUT ENTITY TOOL \"Please select a tool from the list.\"");
            _powerMill.RunMacro("ACTIVATE TOOL $selectedToolName");
            _powerMill.RunMacro("EDIT PAR 'CollisionCheck' '1'");
            _powerMill.RunMacro($"FORM ACCEPT {featureFormName}");
            _powerMill.RunMacro("BATCH PROCESS");

        }
        public static void EditRestAreaClearance(PMAutomation _powerMill, string featureFormName)
        {

            _powerMill.RunMacro("ACTIVATE TOOLPATH \"#\" FORM TOOLPATH\\r");
            _powerMill.RunMacro("EDIT TOOLPATH \"#\" RECYCLE\\r");
            _powerMill.RunMacro("EDIT PAR 'Thickness' \".5\"\\r");
            _powerMill.Execute("EDIT TPPAGE SWRes");
            _powerMill.Execute("EDIT STOCKMODEL '1' ACTIVATE INDEXED_STATE '1' ");
            _powerMill.RunMacro("EDIT PAR 'AreaClearance.Rest.ThresholdThickness' \".2");
            _powerMill.RunMacro("EDIT TOOLPATH \"#\" CALCULATE");
            _powerMill.RunMacro("FORM ACCEPT SFAreaClearance\n");


          


        } public static void EditFeatureRestAreaClearance(PMAutomation _powerMill, PMProject _session, string strategyName, string FeatureFormName)
        {

            _powerMill.Execute($"ACTIVATE TOOLPATH \"{strategyName}\" FORM TOOLPATH\\r");
            _powerMill.Execute($"EDIT TOOLPATH \"{strategyName}\" RECYCLE\\r");
            _powerMill.Execute("EDIT TPPAGE SWRes");
            _powerMill.Execute("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            _powerMill.Execute("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '1' ");

            _powerMill.Execute("EDIT TOOLPATH \"#\" CALCULATE");
            _powerMill.Execute($"FORM ACCEPT {FeatureFormName}\n");

        }
        
        
        public static void PauseEditMacro(PMAutomation _powerMill, PMProject _session, string programName)
        {
            _powerMill.Execute($"EXPLORER SELECT Toolpath \"Toolpath\\{programName}\" NEW");
            _powerMill.Execute($"ACTIVATE TOOLPATH \"{programName}\" FORM TOOLPATH");
            _powerMill.Execute("EDIT TPPAGE SWFeatures");
            _powerMill.Execute("ACTIVATE FEATUREGROUP \"1\"");
            _powerMill.Execute($"EDIT TOOLPATH \"{programName}\" CALCULATE");
            //_powerMill.Execute("MESSAGE INFO \"Please,  select your Tool, Feature Group, then hit Calculate\"");
        }

        public static void CreateStockModelRoughStrategy(PMAutomation _powerMill, PMProject _session, string programName)

        {
            _powerMill.Execute("CREATE STOCKMODEL ; FORM STOCKMODEL");
            _powerMill.Execute("EDIT STOCKMODEL \"#\" REAPPLYFROMGUI");
            _powerMill.Execute("EXPLORER SELECT StockModel \"StockModel\\#\" NEW");
            _powerMill.Execute("EDIT STOCKMODEL \"#\" BLOCK ;");
            _powerMill.Execute("EDIT STOCKMODEL \"#\" CALCULATE_STATE INDEXED_STATE 0");
            _powerMill.Execute("EXPLORER SELECT Toolpath \"Toolpath\\Model_Area_Clearance\" NEW");
            _powerMill.Execute("EDIT STOCKMODEL ; INSERT_INPUT TOOLPATH \"Model_Area_Clearance\" LAST");
            _powerMill.Execute("EDIT STOCKMODEL \"#\" CALCULATE_STATE INDEXED_STATE 1");
            _powerMill.Execute("EXPLORER SELECT StockModel \"StockModel\\#\" NEW");
            _powerMill.Execute("UNDRAW STOCKMODEL \"#\"");
            _powerMill.Execute("EXPLORER SELECT Toolpath \"Toolpath\\Model_Area_Clearance\" NEW");
            _powerMill.Execute("DRAW Toolpath \"Model_Area_Clearance\"");

        }

        public static void EditMoldTemplates(PMAutomation powerMill, PMProject session, string toolpathName, string  typeOfStrategy)
        {

            powerMill.Execute($"EXPLORER SELECT Toolpath \"{toolpathName}\" ");
            powerMill.Execute($"ACTIVE TOOLPATH \"{toolpathName}\" ");
            powerMill.Execute("EDIT TPPAGE SWRest");
            powerMill.Execute("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            powerMill.Execute("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            powerMill.Execute("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '1' ");
            powerMill.Execute($"EDIT TOOLPATH \"{toolpathName}\" CALCULATE");
            powerMill.Execute($"FORM ACCEPT {typeOfStrategy}");




        }

        public   static void DeleteInactiveWorkplane(PMAutomation _powerMill, PMProject _session)
        {

            foreach (var wp in _session.Workplanes.ToList())
            {
                if (!wp.IsActive)
                {

                }
                try
                {
                    wp.Delete();
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Failed to delete Workplane{wp.Name}: {ex.Message}");
                }


            }


        }

        public static void DeletInactiveFeatureGroup(PMProject _session)
        {
            
            foreach( var fg in _session.FeatureGroups.ToList())

            {

                 if (!fg.IsActive)
                {


                    fg.Delete();
                }
            }
        }
        public static void BatchProcess(PMAutomation _powerMill)

        {
            _powerMill.Execute("BATCH PROCESS");

        }



        public static void CreateStockModelFeatureAreaClearance(PMAutomation _powerMill, PMProject _session, string programName)

        {
            _powerMill.Execute("CREATE STOCKMODEL ; FORM STOCKMODEL");
            _powerMill.Execute("EDIT STOCKMODEL \"#\" REAPPLYFROMGUI");
            _powerMill.Execute("EXPLORER SELECT StockModel \"StockModel\\#\" NEW");
            _powerMill.Execute("EDIT STOCKMODEL \"#\" BLOCK ;");
            _powerMill.Execute("EDIT STOCKMODEL \"#\" CALCULATE_STATE INDEXED_STATE 0");
            _powerMill.Execute("EXPLORER SELECT Toolpath \"Toolpath\\Feature-Area-Clearance\" NEW");
            _powerMill.Execute("EDIT STOCKMODEL ; INSERT_INPUT TOOLPATH \"Model_Area_Clearance\" LAST");
            _powerMill.Execute("EDIT STOCKMODEL \"#\" CALCULATE_STATE INDEXED_STATE 1");
            _powerMill.Execute("EXPLORER SELECT StockModel \"StockModel\\#\" NEW");
            _powerMill.Execute("UNDRAW STOCKMODEL \"#\"");
            _powerMill.Execute("EXPLORER SELECT Toolpath \"Toolpath\\Model_Area_Clearance\" NEW");
            _powerMill.Execute("DRAW Toolpath \"Model_Area_Clearance\"");

        }
    } 
}