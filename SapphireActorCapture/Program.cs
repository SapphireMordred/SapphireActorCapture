using EasyHook;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SapphireActorCapture
{
    class Program
    {
        public static String channelName = null;

        static void Main(string[] args)
        {
            bool show_help = false;

            var options = new OptionSet() {
        { "i|ui", "Enable UI",
            v => Globals.UI = v != null },
        { "c|characters", "Write characters to database",
            v => Globals.writeChars = v != null },
        { "d|db", "Connect to a database",
            v => Globals.DB = v != null },
        { "x|xml", "Write to mobdef XMLs",
            v => Globals.xmlOutput = v != null },
        { "h|help",  "Show this message and exit",
            v => show_help = v != null },
        { "dbhost=", "MySQL server to write actors into, default: localhost",     v => Globals.dbhost = v },
        { "dbuser=", "MySQL username, default: root",     v => Globals.dbuser = v },
        { "dbpwd=", "MySQL password, default: empty",     v => Globals.dbpwd = v },
        { "dbname=", "MySQL database name, default: sapphire",     v => Globals.dbname = v },
        };

            List<string> extra;
            try
            {
                extra = options.Parse(args);

            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `--help' for more information.");
                return;
            }

            if (show_help)
            {
                Console.WriteLine("Usage: [OPTIONS]+");
                Console.WriteLine("\nOptions:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            Globals.exdreader = new ExdCsvReader();

            if (Globals.UI)
            {
                Console.WriteLine("Main: Opening UI");

                Thread t = new Thread(new ThreadStart(StartNewStaThread));
                t.Start();

            }

            if (!System.IO.File.Exists("hook.dll"))
            {
                Console.WriteLine("Main: Could not find DLL!");
                Environment.Exit(0);
            }

            Console.WriteLine("Main: Looking for ffxiv pid...");
            
            try { 
                RemoteHooking.IpcCreateServer<RemoteMon>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);

                int pid = -1;

                foreach (Process p in Process.GetProcessesByName("ffxiv"))
                {
                    pid = p.Id;
                }

                if (pid == -1)
                {
                    foreach (Process p in Process.GetProcessesByName("ffxiv_dx11"))
                    {
                        pid = p.Id;
                    }
                }

                if (pid == -1)
                {
                    Console.WriteLine("Main: Could not find pid for ffxiv!");

                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Main: PID: {0}, injecting", pid);

                RemoteHooking.Inject(pid, InjectionOptions.DoNotRequireStrongName, "hook.dll", "hook.dll", new String[] { channelName });
            }
            catch (Exception ExtInfo)
            { 
                Console.WriteLine("Main: There was an error while connecting to target:\r\n{0}", ExtInfo.ToString());
                Console.ReadLine();
            }

            while (true)
            {
                Thread.Sleep(1000);
            }



        }

        [STAThread]
        private static void StartNewStaThread()
        {
            Globals.mapviewform = new UI.MapViewForm();
            Application.Run(Globals.mapviewform);
        }
    }
}
