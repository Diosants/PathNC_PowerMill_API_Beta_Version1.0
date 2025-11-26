using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface;
using System.Security.Cryptography.X509Certificates;


namespace PowerMill_API_Version1._0.Helpers
{
    public static  class Logger
    {

        public static void Info(string message)
        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{message}");
            Console.ResetColor();
        }

        public static void  Error(string message)
        {
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine($"{message}");
            Console.ResetColor();

        }
    }
}
