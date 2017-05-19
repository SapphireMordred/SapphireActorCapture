using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireActorCapture.Models
{
    public class Territory
    {
        int zoneId;
        string identifier;
        int placenameId;

        public Territory(int zoneId, string identifier, int placenameId)
        {
            ZoneId = zoneId;
            Identifier = identifier;
            PlacenameId = placenameId;
        }

        public int ZoneId
        {
            get
            {
                return zoneId;
            }

            set
            {
                zoneId = value;
            }
        }

        public string Identifier
        {
            get
            {
                return identifier;
            }

            set
            {
                identifier = value;
            }
        }

        public int PlacenameId
        {
            get
            {
                return placenameId;
            }

            set
            {
                placenameId = value;
            }
        }
    }
}
