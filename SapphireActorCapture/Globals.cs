using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapphireActorCapture.UI;

namespace SapphireActorCapture
{
    public static class Globals
    {
        public static string dbhost = "localhost";
        public static string dbuser = "root";
        public static string dbpwd = "";
        public static string dbname = "sapphire";

        public static bool DB = false;
        public static bool xmlOutput = false;
        public static bool writeChars = false;
        public static bool UI = false;
        public static bool memory = false;

        public static int ffxivPid = -1;

        public static ExdCsvReader exdreader;
        public static MapViewForm mapviewform;
    }
}
