using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;


namespace MoldAutomation.Machining
{
    public static class ConsoleHelpers
    {
        /// <summary>
        /// Shows a progress bar in the console.
        /// </summary>
        /// <param name="progress">Current step (e.g., 2)</param>
        /// <param name="total">Total steps (e.g., 5)</param>
        /// <param name="message">Status message</param>
        public static void ShowProgressBar(int progress, int total, string message)
        {
            int barWidth = 40;
            double percent = (double)progress / total;
            int filled = (int)(percent * barWidth);

            Console.CursorLeft = 0;
            Console.Write("[");
            Console.Write(new string('=', filled));
            Console.Write(new string(' ', barWidth - filled));
            Console.Write($"] {percent:P0} {message}");

            if (progress == total)
                Console.WriteLine(); // Go to next line when finished
            
        }
    }
}