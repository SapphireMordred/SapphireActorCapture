using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapphireActorCapture.Models;

namespace SapphireActorCapture
{
    public class ExdCsvReader
    {
        private List<string> bnpcnames = new List<string>();
        private List<string> placenames = new List<string>();
        private List<Territory> territories = new List<Territory>();
        private List<Map> maps = new List<Map>();


        public ExdCsvReader()
        {
            try
            {
            using (TextFieldParser parser = new TextFieldParser(@"exd\bnpcname.exh_en.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int rowCount = 0;
                while (!parser.EndOfData)
                {
                    //Processing row
                    rowCount++;
                    string[] fields = parser.ReadFields();
                    int fCount = 0;
                    foreach (string field in fields)
                    {
                        fCount++;
                        
                        if(fCount == 2)
                        {
                            bnpcnames.Add(field);
                        }
                    }
                }
                Console.WriteLine($"ExdCsvReader: {rowCount} bnpc names read");
            }

            using (TextFieldParser parser = new TextFieldParser(@"exd\placename_0.exh_en.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int rowCount = 0;
                while (!parser.EndOfData)
                {
                    //Processing row
                    rowCount++;
                    string[] fields = parser.ReadFields();
                    int fCount = 0;
                    foreach (string field in fields)
                    {
                        fCount++;

                        if (fCount == 2)
                        {
                            placenames.Add(field);
                        }
                    }
                }
                Console.WriteLine($"ExdCsvReader: {rowCount} placenames read");
            }

            using (TextFieldParser parser = new TextFieldParser(@"exd\territorytype.exh.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int rowCount = 0;
                while (!parser.EndOfData)
                {
                    //Processing row
                    rowCount++;
                    string[] fields = parser.ReadFields();
                    int fCount = 0;

                    int zoneId = -1; //field 1
                    string identifier = ""; //field 2
                    int placenameId = -1; //field 7
                    
                    foreach (string field in fields)
                    {
                        switch (fCount)
                        {
                            case 0:
                                int.TryParse(field, out zoneId);
                                break;
                            case 1:
                                identifier = field;
                                break;
                            case 6:
                                int.TryParse(field, out placenameId);
                                break;
                        }
                        fCount++;
                    }
                    //Console.WriteLine($"{zoneId} - {identifier} - {GetPlacename(placenameId)}");
                    territories.Add(new Territory(zoneId, identifier, placenameId));
                }
                Console.WriteLine($"ExdCsvReader: {rowCount} territories read");
            }


                using (TextFieldParser parser = new TextFieldParser(@"exd\map.exh.csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    int rowCount = 0;
                    while (!parser.EndOfData)
                    {
                        //Processing row
                        rowCount++;
                        string[] fields = parser.ReadFields();
                        int fCount = 0;

                        uint mapMarkerRange = 0; //field 4
                        string identifier = ""; //field 5
                        uint sizeFactor = 0; //field 6
                        uint placeNameRegion = 0; //field 9
                        uint placeNameSub = 0; //field 11
                        uint territoryType = 0; //field 14
                        uint hierarchy = 0; //field 3
                        uint placeName = 0; //field 10

                        foreach (string field in fields)
                        {
                            switch (fCount)
                            {
                                case 5:
                                    uint.TryParse(field, out mapMarkerRange);
                                    break;
                                case 6:
                                    identifier = field;
                                    break;
                                case 7:
                                    uint.TryParse(field, out sizeFactor);
                                    break;
                                case 10:
                                    uint.TryParse(field, out placeNameRegion);
                                    break;
                                case 12:
                                    uint.TryParse(field, out placeNameSub);
                                    break;
                                case 15:
                                    uint.TryParse(field, out territoryType);
                                    break;
                                case 4:
                                    uint.TryParse(field, out hierarchy);
                                    break;
                                case 11:
                                    uint.TryParse(field, out placeName);
                                    break;
                            }
                            fCount++;
                        }
                        //Console.WriteLine($"{zoneId} - {identifier} - {GetPlacename(placenameId)}");
                        maps.Add(new Map(mapMarkerRange, identifier, sizeFactor, placeNameRegion, placeNameSub, territoryType, hierarchy, placeName));
                    }
                    Console.WriteLine($"ExdCsvReader: {rowCount} maps read");

                }
            }catch(Exception exc)
            {
                Console.WriteLine("ExdCsvReader: failed to parse CSV, continuing anyways\n"+exc);
            }
        }

        public string GetBnpcName(int id)
        {
            try
            {
                return bnpcnames[id + 1];
            }
            catch
            {
                return "Unknown";
            }
            
        }

        public Territory GetTerritory(int id)
        {
            try
            {
                foreach (Territory territory in territories)
                {
                    if (territory.ZoneId == id)
                    {
                        return territory;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public string GetTerritoryName(int id)
        {
            try
            {
                foreach(Territory territory in territories)
                {
                    if (territory.ZoneId == id)
                    {
                        return GetPlacename(territory.PlacenameId);
                    }
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Gets a map with its territory id. Submap is ignored, the first map found will be chosen(usually 00). Returns null if none found.
        /// </summary>
        public Map GetMap(int id)
        {
            try
            {
                foreach (Map map in maps)
                {
                    if (map.identifier.Contains(GetTerritory(id).Identifier))
                        return map;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a map with its territory id and submap. Returns null if none found.
        /// </summary>
        public Map GetMap(int id, int submap)
        {
            try
            {
                foreach (Map map in maps)
                {
                    if (map.identifier == (GetTerritory(id).Identifier + "/0" + submap.ToString()))
                        return map;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a Map via its identifier(Format: "ident/submap"). Returns null if none found
        /// </summary>
        public Map GetMap(string identifier)
        {
            try
            {
                foreach (Map map in maps)
                {
                    if (map.identifier.Contains(identifier))
                        return map;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public string GetPlacename(int id)
        {
            try
            {
                return placenames[id + 1];
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
