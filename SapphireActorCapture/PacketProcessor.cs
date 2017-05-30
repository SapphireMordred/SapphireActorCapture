using SapphireActorCapture.Models;
using SapphireActorCapture.Packets;
using SapphireActorCapture.Packets.Receive;
using SapphireActorCapture.Packets.Send;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace SapphireActorCapture
{
    class PacketProcessor
    {
        public enum PPType
        {
            Recv,
            Send
        }

        MySqlConnection dbconnection;

        private int currentZone = 0;

        private string outputFolderName;
        private string outputKey;

        public PacketProcessor()
        {
            if (Globals.DB){
                string connectionString = $"SERVER={Globals.dbhost};DATABASE={Globals.dbname};UID={Globals.dbuser};PASSWORD={Globals.dbpwd};";

                dbconnection = new MySqlConnection(connectionString);
                dbconnection.Open();
                Console.WriteLine("PacketProcessor: Connected to database, " + connectionString);
            }

            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");

            outputKey = DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss");
            if (Globals.xmlOutput)
            {
                try
                {
                    if (!Directory.Exists(Path.Combine("output", outputKey)))
                        Directory.CreateDirectory(Path.Combine("output", outputKey));
                }catch(Exception exc)
                {
                    Console.WriteLine("Could not setup output folder!\n" + exc);
                    Environment.Exit(0);
                }

                outputFolderName = Path.Combine("output", outputKey);
            }

            if (Globals.outputOverride != "")
            {
                outputFolderName = Path.Combine("output", Globals.outputOverride);
                outputKey = Globals.outputOverride;
            }
        }

        public void ProcessPacket(BasePacket packet, PPType type)
        {
            if (packet.header.isCompressed == 0x01)
                BasePacket.DecompressPacket(ref packet);

            List<SubPacket> subPackets = packet.GetSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                //Normal Game Opcode
                if (subpacket.header.type == 0x03)
                {
                    switch (type)
                    {
                        case PPType.Recv: HandleRecv(subpacket);
                            break;
                        case PPType.Send: HandleSend(subpacket);
                            break;
                    }
                }
            }
        }

        private void HandleRecv(SubPacket subpacket)
        {
            switch (subpacket.gameMessage.opcode)
            {
                /* SERVER OPCODES */

                case 0x190: /* ACTOR_SPAWN */
                    if (Globals.memory)
                    {
                        currentZone = Memory.getZoneId();
                    }

                    Console.WriteLine("\n-> ACTOR_SPAWN");
                    ActorSpawnPacket actorSpawnPacket = new ActorSpawnPacket(subpacket.data, currentZone, subpacket.header.sourceId);

                    switch (actorSpawnPacket.type)
                    {
                        case 1:
                            if (actorSpawnPacket.spawnIndex == 0)
                            {
                                Console.WriteLine($"    -> OWN CHARACTER: {actorSpawnPacket.name}   Zone:{currentZone}   EntryLength:{subpacket.data.Length}");
                                System.Threading.Thread.Sleep(100);

                                /* replaced with init_zone
                                currentZone = Memory.ReadZoneId();
                                Console.WriteLine($"    -> New Zone: {currentZone} - {exdreader.GetTerritoryName(currentZone)}");
                                */
                            }
                            else
                            {
                                Console.WriteLine($"    -> CHARACTER: {actorSpawnPacket.name}   Zone:{currentZone}   EntryLength:{subpacket.data.Length}");

                                if (Globals.xmlOutput & Globals.writeChars & currentZone != 0 & !actorSpawnPacket.invalidPacket)
                                {
                                    ActorWriter.writeChar(actorSpawnPacket, subpacket.header.sourceId, currentZone, outputFolderName);
                                }
                                else
                                {
                                    if (Globals.writeChars)
                                        Console.WriteLine($"    -> currentZone==0(change your zone once to fix) or invalid packet");
                                }
                            }
                            break;
                        case 0:
                            Console.WriteLine($"    -> EMPTY: {actorSpawnPacket.name}   Zone:{currentZone}   EntryLength:{subpacket.data.Length}");
                            break;
                        default:
                            Console.WriteLine($"    -> NPC({actorSpawnPacket.nameId}): {Globals.exdreader.GetBnpcName(actorSpawnPacket.nameId)}   Zone:{currentZone}   EntryLength:{subpacket.data.Length}");

                            if (actorSpawnPacket.fateId != 0)
                            {
                                Console.WriteLine($"    -> FATE NPC: {Globals.exdreader.GetFateName(actorSpawnPacket.fateId)}({actorSpawnPacket.fateId})");
                            }

                            if (Globals.UI)
                            {
                                Globals.mapviewform.BeginInvoke((MethodInvoker)delegate () {
                                    Globals.mapviewform.addActor(actorSpawnPacket);
                                    Globals.mapviewform.invalidateMap();
                                });
                            }

                            //actorSpawnPacket.debugPrintUnknown();
                            //File.WriteAllBytes(Path.Combine(outputFolderName, $"{subpacket.header.sourceId}.dat"), subpacket.data);

                            if (Globals.xmlOutput & currentZone != 0 & !actorSpawnPacket.invalidPacket)
                            {
                                ActorWriter.writeMob(actorSpawnPacket, subpacket.header.sourceId, currentZone, outputFolderName);
                            }
                            else
                            {
                                if (Globals.xmlOutput)
                                    Console.WriteLine($"    -> currentZone==0(change your zone once to fix) or invalid packet");
                            }

                            if (Globals.csvOutput & currentZone != 0 & !actorSpawnPacket.invalidPacket)
                            {
                                try
                                {
                                    ActorWriter.addCSVEntry(actorSpawnPacket, subpacket.header.sourceId, currentZone, Path.Combine("output", outputKey + ".csv"));
                                }catch(Exception exc)
                                {
                                    Console.WriteLine($"    -> " + exc);
                                }
                                
                            }
                            else
                            {
                                if (Globals.csvOutput)
                                    Console.WriteLine($"    -> currentZone==0(change your zone once to fix) or invalid packet");
                            }

                            if (Globals.DB & currentZone != 0 & !actorSpawnPacket.invalidPacket)
                            {
                                using (MySqlCommand command = new MySqlCommand())
                                {
                                    command.Connection = dbconnection;
                                    command.CommandText = "INSERT INTO dbbattlenpc (Id, ZoneId, Type, NameId, SizeId, ModelId, ClassJob, DisplayFlags1, DisplayFlags2, Level, Pos_0_0, Pos_0_1, Pos_0_2, Rotation, MobType, Behaviour, ModelMainWeapon, ModelSubWeapon, Look, Models) VALUES (?Id, ?ZoneId, ?Type, ?NameId, ?SizeId, ?ModelId, ?ClassJob, ?DisplayFlags1, ?DisplayFlags2, ?Level, ?Pos_0_0, ?Pos_0_1, ?Pos_0_2, ?Rotation, ?MobType, ?Behaviour, ?ModelMainWeapon, ?ModelSubWeapon, ?Look, ?Models);";
                                    List<MySqlParameter> parameters = new List<MySqlParameter>();
                                    parameters.Add(new MySqlParameter("?Id", MySqlDbType.Int32, 11));
                                    parameters[0].Value = subpacket.header.sourceId;
                                    parameters.Add(new MySqlParameter("?ZoneId", MySqlDbType.Int32, 10));
                                    parameters[1].Value = currentZone;
                                    parameters.Add(new MySqlParameter("?Type", MySqlDbType.Int32, 11));
                                    parameters[2].Value = actorSpawnPacket.type;
                                    parameters.Add(new MySqlParameter("?NameId", MySqlDbType.Int32, 10));
                                    parameters[3].Value = actorSpawnPacket.nameId;
                                    parameters.Add(new MySqlParameter("?BnpcBaseId", MySqlDbType.Int32, 10));
                                    parameters[4].Value = actorSpawnPacket.bnpcBaseId;
                                    parameters.Add(new MySqlParameter("?ModelId", MySqlDbType.Int32, 10));
                                    parameters[5].Value = actorSpawnPacket.model;
                                    parameters.Add(new MySqlParameter("?ClassJob", MySqlDbType.Int32, 3));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.classJob;
                                    parameters.Add(new MySqlParameter("?DisplayFlags1", MySqlDbType.Int32, 3));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.displayFlags1;
                                    parameters.Add(new MySqlParameter("?DisplayFlags2", MySqlDbType.Int32, 3));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.displayFlags2;
                                    parameters.Add(new MySqlParameter("?Level", MySqlDbType.Int32, 3));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.level;
                                    parameters.Add(new MySqlParameter("?Pos_0_0", MySqlDbType.Float));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.posx;
                                    parameters.Add(new MySqlParameter("?Pos_0_1", MySqlDbType.Float));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.posy;
                                    parameters.Add(new MySqlParameter("?Pos_0_2", MySqlDbType.Float));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.posz;
                                    parameters.Add(new MySqlParameter("?Rotation", MySqlDbType.Int32, 10));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.rotation;
                                    parameters.Add(new MySqlParameter("?MobType", MySqlDbType.Int32, 3));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.mobType;
                                    parameters.Add(new MySqlParameter("?Behaviour", MySqlDbType.Int32, 3));
                                    parameters[parameters.Count - 1].Value = 0; //?
                                    parameters.Add(new MySqlParameter("?ModelMainWeapon", MySqlDbType.Int32, 20));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.mainWeaponModel;
                                    parameters.Add(new MySqlParameter("?ModelSubWeapon", MySqlDbType.Int32, 20));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.secWeaponModel;
                                    parameters.Add(new MySqlParameter("?Look", MySqlDbType.Blob, actorSpawnPacket.lookdata.Length));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.lookdata;
                                    parameters.Add(new MySqlParameter("?Models", MySqlDbType.Blob, actorSpawnPacket.models.Length));
                                    parameters[parameters.Count - 1].Value = actorSpawnPacket.models;

                                    foreach (MySqlParameter parameter in parameters)
                                        command.Parameters.Add(parameter);

                                    try
                                    {
                                        command.Prepare();
                                        command.ExecuteNonQuery();
                                        Console.WriteLine(command.CommandText);
                                    }catch(Exception exc)
                                    {
                                        Console.WriteLine($"    -> Error writing to DB: " + exc.Message);
                                    }


                                }
                            }
                            else
                            {
                                if(Globals.DB)
                                    Console.WriteLine($"    -> currentZone==0(change your zone once to fix) or invalid packet");
                            }
                            break;
                    }
                    break;
                case 0x19A: /* INIT_ZONE */
                    Console.WriteLine("\n-> INIT_ZONE");
                    InitZonePacket initZonePacket = new InitZonePacket(subpacket.data);

                    currentZone = Convert.ToInt32(initZonePacket.zoneId);

                    if (Globals.UI)
                    {
                        Globals.mapviewform.BeginInvoke((MethodInvoker)delegate () { Globals.mapviewform.SetMapWithId(currentZone, 00); });
                    }
                    
                    Console.WriteLine($"    -> New Zone({initZonePacket.zoneId}): {Globals.exdreader.GetTerritoryName(currentZone)}      EntryLength:{subpacket.data.Length}");
                    break;

                case 0x146: /* EFFECT */
                    Console.WriteLine("\n-> EFFECT");
                    EffectPacket effectPacket = new EffectPacket(subpacket.data);

                    Console.WriteLine($"    -> ACTION: {Globals.exdreader.GetActionName((int)effectPacket.actionTextId)}   SourceId:{subpacket.header.sourceId}   TargetId:{subpacket.header.targetId}   Zone:{currentZone}   EntryLength:{subpacket.data.Length}");

                    if (Globals.xmlOutput)
                        ActorWriter.addEffect(effectPacket, subpacket.header.sourceId, outputFolderName);

                    break;
            }
        }

        private void HandleSend(SubPacket subpacket)
        {
            switch (subpacket.gameMessage.opcode)
            {
                case 0x10d: /* POS_UPDATE */
                    Console.WriteLine("-> POS_UPDATE");
                    UpdatePlayerPositionPacket pospacket = new UpdatePlayerPositionPacket(subpacket.data);

                    Console.WriteLine($"    -> {pospacket.x} {pospacket.y} {pospacket.z}");

                    if (Globals.UI)
                    {
                        Globals.mapviewform.BeginInvoke((MethodInvoker)delegate () {
                            Globals.mapviewform.setPos(Convert.ToInt32(pospacket.x), Convert.ToInt32(pospacket.y));
                            Globals.mapviewform.invalidateMap();
                        });
                    }
                    break;
            }
        }
    }
}
