using System;
using System.IO;
using Autodesk.ProductInterface.PowerMILL;

namespace PowerMill_API_Version1._0.Helpers
{
    public static class MacroManager
    {
        public static void RunMacroFromProject(PMAutomation powerMill, string macroName)
        {
            if (powerMill == null)
                throw new ArgumentNullException(nameof(powerMill));

            // Get the folder where the .exe is running
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Look for a "Macro_Files" folder next to your exe
            string macroDir = Path.Combine(baseDir, "Macro_Files");
            string macroPath = Path.Combine(macroDir, macroName);

            if (!File.Exists(macroPath))
            {
                throw new FileNotFoundException(
                    $"Macro not found: {macroPath}\n" +
                    $"Did you set 'Copy to Output Directory = Copy if newer' in the file properties?");
            }

            // Execute the macro
            powerMill.Execute($"Macro \"{macroPath}\"");
        }
    }
}
