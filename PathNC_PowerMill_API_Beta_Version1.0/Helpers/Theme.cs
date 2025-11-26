using Autodesk.ProductInterface.PowerMILL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMill_API_Version1._0.Helpers
{
    public static class RibbonThemeService
    {
        public enum RibbonTheme
        {
            Default,
            Office2016White,
            Office2016Colorful,
            Office2016DarkGray
        }

        [Obsolete]
        public static void Apply(PMAutomation pm, RibbonTheme theme)
        {
            if (pm == null) throw new ArgumentNullException(nameof(pm));
            if (pm.ActiveProject == null) pm.Execute("PROJECT NEW");

            string key = theme switch
            {
                RibbonTheme.Default => "DEFAULT",
                RibbonTheme.Office2016White => "OFFICE_2016_WHITE",
                RibbonTheme.Office2016Colorful => "OFFICE_2016_COLORFUL",
                RibbonTheme.Office2016DarkGray => "OFFICE_2016_DARK_GRAY",
                _ => "DEFAULT"
            };

            pm.Execute($"FORM RIBBON STYLE {key}");
        }

        [Obsolete]
        public static void Apply(PMAutomation pm, string themeName)
        {
            if (pm == null) throw new ArgumentNullException(nameof(pm));
            if (string.IsNullOrWhiteSpace(themeName)) throw new ArgumentException("Empty theme name.", nameof(themeName));
            if (pm.ActiveProject == null) pm.Execute("PROJECT NEW");

            // Friendly names -> macro keys
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"]               = "DEFAULT",
                ["office 2016 white"]     = "OFFICE_2016_WHITE",
                ["white"]                 = "OFFICE_2016_WHITE",
                ["office 2016 colorful"]  = "OFFICE_2016_COLORFUL",
                ["colorful"]              = "OFFICE_2016_COLORFUL",
                ["office 2016 dark gray"] = "OFFICE_2016_DARK_GRAY",
                ["dark gray"]             = "OFFICE_2016_DARK_GRAY",
                ["darkgrey"]              = "OFFICE_2016_DARK_GRAY"
            };

            if (!map.TryGetValue(themeName.Trim(), out var key))
                throw new ArgumentOutOfRangeException(nameof(themeName), $"Unknown theme: {themeName}");

            pm.Execute($"FORM RIBBON STYLE {key}");
        }
    }
}