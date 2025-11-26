using Autodesk.ProductInterface.PowerMILL;
using System;
using System.IO;
using System.Windows;

namespace PowerMill_API_Version1._0.Helpers
{
    public static class TemplateManager
    {
        /// <summary>
        /// Imports a PowerMill template (.ptf) from your project Template_Files folder.
        /// </summary>
        /// <param name="templateName">The file name of the template (e.g. "Rough-Grey-Profile+050.ptf").</param>
        /// <param name="_powerMill">An existing PMAutomation session (created in your app).</param>
        /// <param name="_session">The active PowerMill project session.</param>
        public static void ImportTemplate(string templateName,PMProject _session)
        {
            try
            {
                
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = Path.Combine(baseDir, "Template_Files", templateName);

                if (!File.Exists(templatePath))
                {
                    MessageBox.Show(" Template not found: " + templatePath);
                    return;
                }
                Autodesk.FileSystem.File templateFile = new(templatePath);

                //  Import into current project
                _session.ImportTemplateFile(templateFile);

            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠️ Error importing template: " + ex.Message);
            }
        }


        /// <summary>
        /// Gets the absolute path of a template file from Template_Files folder.
        /// Useful if you need the path for custom PowerMill commands (STRATEGYSELECTOR etc).
        /// </summary>
        public static string GetTemplatePath(string templateName)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string templatePath = Path.Combine(baseDir, "Template_Files", templateName);

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($" Template not found: {templatePath}");

            return templatePath;
        }
    }

    
}