using Microsoft.VisualBasic.FileIO;
using SapphireActorCapture.Packets.Receive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "-x": doXMLRead(args[1]); break;
                case "-t": doCSVRead(args[1]); break;
            }
            Console.ReadKey();
        }

        static void doXMLRead(string folderName)
        {

        }

        static void doCSVRead(string fileName)
        {
            List<string> templated = new List<string>();
            StringBuilder templates = new StringBuilder();
            StringBuilder spawns = new StringBuilder();

            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int rowCount = 0;
                int pEntries = 0;
                string lastZone = "";
                while (!parser.EndOfData)
                {
                    //Processing row
                    rowCount++;
                    if(rowCount < 2)
                    {
                        continue;
                    }
                    string[] fields = parser.ReadFields();
                    
                    int fCount = 0;
                    

                    string sourceId = "";
                    string baseId = "";
                    string nameId = "";
                    string modelId = "";
                    string fateId = "";
                    string level = "";
                    //hp
                    //mp
                    //tp
                    string territory = "";
                    string posx = "";
                    string posy = "";
                    string posz = "";
                    string rot = "";
                    string info = "";
                    foreach (string field in fields)
                    {
                        fCount++;

                        switch (fCount)
                        {
                            case 1: sourceId = field; break;
                            case 2: baseId = field; break;
                            case 3: nameId = field; break;
                            case 4: modelId = field; break;
                            case 5: fateId = field; break;
                            case 6: level = field; break;
                            case 10: territory = field; break;
                            case 11: posx = field; break;
                            case 12: posy = field; break;
                            case 13: posz = field; break;
                            case 14: rot = field; break;
                            case 15: info = field; break;
                        }
                    }

                    if (fateId == "0" && !templated.Contains(baseId))
                    {
                        string output = $"registerTemplate( \"{info.Substring(0, info.IndexOf("-") - 1).Replace(" ", "_").ToLower()}\", {nameId}, {modelId}, {baseId}, \"DefaultAI\" ); //{info}";
                        templated.Add(baseId);
                        templates.AppendLine(output);
                        pEntries++;
                    }

                    if (fateId == "0")
                    {
                        if(lastZone != territory)
                        {
                            spawns.AppendLine($"\n// Zone {territory} - {info.Substring(info.IndexOf("-") + 2).Replace(" ", "_").ToLower()}\n");
                            lastZone = territory;
                        }
                        spawns.AppendLine($"addBnpcSpawn( \"{info.Substring(0, info.IndexOf("-") - 1).Replace(" ", "_").ToLower()}\", {level}, {posx}, {posy}, {posz}, {rot} )");
                    }
                }
                Console.Write(templates.ToString());
                Console.WriteLine($"\n{rowCount} csv entries, {pEntries} unique");
                Console.WriteLine(spawns.ToString());
                System.IO.File.WriteAllText($"{fileName}-templates.chai", templates.ToString());
                System.IO.File.WriteAllText($"{fileName}-spawns.chai", spawns.ToString());
            }
        }
    }
}
