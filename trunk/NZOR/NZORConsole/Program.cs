using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NZORConsole
{
    class Program
    {
        static System.Timers.Timer _t = new System.Timers.Timer();

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage : NZORConsole.exe [action] [config file] [match set id] : where [action] can be Integrate, [config file] is the NZOR Integration Rule Set config file, [match set id] is the Match Rule Set in the config file to use for the matching process.");                
            }
            else
            {
                if (args[0].ToUpper() == "INTEGRATE")
                {
                    _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                    _t.Interval = 8000;
                    _t.Start();

                    XmlDocument doc = new XmlDocument();
                    doc.Load(args[1]);
                    
                    NZOR.Integration.IntegrationProcessor.RunIntegration(doc, int.Parse(args[2]));

                    while (NZOR.Integration.IntegrationProcessor.Progress != 100)
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }

        }

        static void  Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine(NZOR.Integration.IntegrationProcessor.StatusText);
            if (NZOR.Integration.IntegrationProcessor.Progress == 100)
            {
                Console.WriteLine("Completed");
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
            }
            else
            {
                _t.Start();
            }
        }

    }
}
