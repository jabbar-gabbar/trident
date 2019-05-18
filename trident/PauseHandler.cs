using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace trident
{
    public class PauseHandler
    {
        public static bool Pause = false;
        private static Thread t;

        public static void SpinUpPause()
        {
            if (t == null || t.IsAlive == false)
            {
                t = new Thread(new ThreadStart(WaitingForPauseSignal));
                t.IsBackground = true;
                t.Start();
            }
        }

        static void WaitingForPauseSignal()
        {
            string input = string.Empty;
           
            while (!input.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                Console.SetCursorPosition(0, 7);
                Console.Write(new string(' ', Console.WindowWidth - 1));
                Console.CursorLeft = 0;
                //Console.Write(new string(' ', Console.WindowWidth));
                Console.Write("Type \"y\" and \"Enter\" to pause upload: ");
                input = Console.ReadLine();
            }
            Console.WriteLine("Upload will pause after current upload job finish.");
            Console.WriteLine("When the trident runs again next time, it will resume from next file.");
            Pause = true;
        }
    }
}
