using SapphireActorCapture.Packets;
using SapphireActorCapture.Packets.Receive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SapphireActorCapture
{
    static class ActorXmlWriter
    {
        public static void writeMob(ActorSpawnPacket actorSpawnPacket, uint sourceId, int currentZone, string outputFolderName)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "     ";
            settings.NewLineOnAttributes = false;

            using (XmlWriter writer = XmlWriter.Create(Path.Combine(outputFolderName, $"{sourceId}.mobdef.xml"), settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Mob");

                writer.WriteElementString("ID", sourceId.ToString());
                writer.WriteElementString("ZoneId", currentZone.ToString());
                writer.WriteElementString("Type", actorSpawnPacket.type.ToString());
                writer.WriteElementString("NameId", actorSpawnPacket.nameId.ToString());
                writer.WriteElementString("SizeId", actorSpawnPacket.sizeId.ToString());
                writer.WriteElementString("ModelId", actorSpawnPacket.model.ToString());
                writer.WriteElementString("ClassJob", actorSpawnPacket.classJob.ToString());
                writer.WriteElementString("DisplayFlags1", actorSpawnPacket.displayFlags1.ToString());
                writer.WriteElementString("DisplayFlags2", actorSpawnPacket.displayFlags2.ToString());
                writer.WriteElementString("Level", actorSpawnPacket.level.ToString());
                writer.WriteElementString("Pos_0_0", actorSpawnPacket.posx.ToString());
                writer.WriteElementString("Pos_0_1", actorSpawnPacket.posy.ToString());
                writer.WriteElementString("Pos_0_2", actorSpawnPacket.posz.ToString());
                writer.WriteElementString("Rotation", actorSpawnPacket.rotation.ToString());
                writer.WriteElementString("MobType", actorSpawnPacket.mobType.ToString());
                writer.WriteElementString("ModelMainWeapon", actorSpawnPacket.mainWeaponModel.ToString());
                writer.WriteElementString("ModelSubWeapon", actorSpawnPacket.secWeaponModel.ToString());
                writer.WriteElementString("Look", BitConverter.ToString(actorSpawnPacket.lookdata).Replace("-", " "));
                writer.WriteElementString("Models", BitConverter.ToString(actorSpawnPacket.models).Replace("-", " "));

                writer.WriteEndElement();
                writer.WriteEndDocument();

                Console.WriteLine($"    -> wrote " + $"{sourceId}.mobdef.xml");
            }
        }

        public static void writeChar(ActorSpawnPacket actorSpawnPacket, uint sourceId, int currentZone, string outputFolderName) //TODO: actually make this write things related to characters
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "     ";
            settings.NewLineOnAttributes = false;

            using (XmlWriter writer = XmlWriter.Create(Path.Combine(outputFolderName, $"{sourceId}.chardef.xml"), settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Mob");

                writer.WriteElementString("ID", sourceId.ToString());
                writer.WriteElementString("ZoneId", currentZone.ToString());
                writer.WriteElementString("Type", actorSpawnPacket.type.ToString());
                writer.WriteElementString("NameId", actorSpawnPacket.nameId.ToString());
                writer.WriteElementString("SizeId", actorSpawnPacket.sizeId.ToString());
                writer.WriteElementString("ModelId", actorSpawnPacket.model.ToString());
                writer.WriteElementString("ClassJob", actorSpawnPacket.classJob.ToString());
                writer.WriteElementString("DisplayFlags1", actorSpawnPacket.displayFlags1.ToString());
                writer.WriteElementString("DisplayFlags2", actorSpawnPacket.displayFlags2.ToString());
                writer.WriteElementString("Level", actorSpawnPacket.level.ToString());
                writer.WriteElementString("Pos_0_0", actorSpawnPacket.posx.ToString());
                writer.WriteElementString("Pos_0_1", actorSpawnPacket.posy.ToString());
                writer.WriteElementString("Pos_0_2", actorSpawnPacket.posz.ToString());
                writer.WriteElementString("Rotation", actorSpawnPacket.rotation.ToString());
                writer.WriteElementString("MobType", actorSpawnPacket.mobType.ToString());
                writer.WriteElementString("ModelMainWeapon", actorSpawnPacket.mainWeaponModel.ToString());
                writer.WriteElementString("ModelSubWeapon", actorSpawnPacket.secWeaponModel.ToString());
                writer.WriteElementString("Look", BitConverter.ToString(actorSpawnPacket.lookdata).Replace("-", " "));
                writer.WriteElementString("Models", BitConverter.ToString(actorSpawnPacket.models).Replace("-", " "));

                writer.WriteEndElement();
                writer.WriteEndDocument();

                Console.WriteLine($"    -> wrote " + $"{sourceId}.chardef.xml");
            }
        }
    }
}
