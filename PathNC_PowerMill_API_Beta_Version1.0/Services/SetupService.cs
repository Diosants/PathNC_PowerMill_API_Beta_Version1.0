using Autodesk.ProductInterface.PowerMILL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMill_API_Version1._0.Services
{
    public class SetupService
    {
        private readonly PMAutomation _pm;

        public SetupService(PMAutomation pm)
        {
            _pm = pm;
        }

        public async Task ImportToolsAsync(string toolFilePath)
        {
            await Task.Run(() =>
            {
                _pm.Execute($"IMPORT TOOL FILE \"{toolFilePath}\"");
            });
        }

        public async Task CreateWorkplaneAsync(string name)
        {
            await Task.Run(() =>
            {
                _pm.Execute($"CREATE WORKPLANE \"{name}\";");
            });
        }

        public async Task MirrorModelAsync(string plane)
        {
            await Task.Run(() =>
            {
                _pm.Execute($"MIRROR MODEL ALL {plane};");
            });
        }
    }
}