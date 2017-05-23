using SapphireActorCapture.Packets;
using SapphireActorCapture.Packets.Receive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SapphireActorCapture
{
    static class ActorXmlWriter
    {
        public static void writeMob(ActorSpawnPacket actorSpawnPacket, uint sourceId, int currentZone, string outputFolderName)
        {
            if (File.Exists(Path.Combine(outputFolderName, $"{sourceId}.mobdef.xml")))
            {
                return;
            }

            if (actorSpawnPacket.mobAgressive != 1)
            {
                Console.WriteLine($"    -> Mob is active, not writing");
                return;
            }

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
                writer.WriteElementString("BnpcBaseId", actorSpawnPacket.bnpcBaseId.ToString());
                writer.WriteElementString("ModelId", actorSpawnPacket.model.ToString());
                writer.WriteElementString("FateId", actorSpawnPacket.fateId.ToString());
                writer.WriteElementString("ClassJob", actorSpawnPacket.classJob.ToString());
                writer.WriteElementString("DisplayFlags1", actorSpawnPacket.displayFlags1.ToString());
                writer.WriteElementString("DisplayFlags2", actorSpawnPacket.displayFlags2.ToString());
                writer.WriteElementString("Level", actorSpawnPacket.level.ToString());
                writer.WriteElementString("MaxHP", actorSpawnPacket.hPMax.ToString());
                writer.WriteElementString("MaxMP", actorSpawnPacket.mPMax.ToString());
                writer.WriteElementString("MaxTP", actorSpawnPacket.tPMax.ToString());
                writer.WriteElementString("Status", actorSpawnPacket.status.ToString());
                writer.WriteElementString("OwnerID", actorSpawnPacket.ownerId.ToString());
                writer.WriteElementString("Pos_0_0", actorSpawnPacket.posx.ToString());
                writer.WriteElementString("Pos_0_1", actorSpawnPacket.posy.ToString());
                writer.WriteElementString("Pos_0_2", actorSpawnPacket.posz.ToString());
                writer.WriteElementString("Rotation", actorSpawnPacket.rotation.ToString());
                writer.WriteElementString("MobType", actorSpawnPacket.mobType.ToString());
                writer.WriteElementString("ModelMainWeapon", actorSpawnPacket.mainWeaponModel.ToString());
                writer.WriteElementString("ModelSubWeapon", actorSpawnPacket.secWeaponModel.ToString());
                writer.WriteElementString("Look", BitConverter.ToString(actorSpawnPacket.lookdata).Replace("-", " "));
                writer.WriteElementString("Models", BitConverter.ToString(actorSpawnPacket.models).Replace("-", " "));

                writer.WriteStartElement("EffectPackets");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();

                Console.WriteLine($"    -> wrote {sourceId}.mobdef.xml");
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
                writer.WriteStartElement("Char");

                writer.WriteElementString("ID", sourceId.ToString());
                writer.WriteElementString("ZoneId", currentZone.ToString());
                writer.WriteElementString("Type", actorSpawnPacket.type.ToString());
                writer.WriteElementString("Name", actorSpawnPacket.name);
                writer.WriteElementString("Title", actorSpawnPacket.title.ToString());
                writer.WriteElementString("Icon", actorSpawnPacket.statusIcon.ToString());
                writer.WriteElementString("BnpcBaseId", actorSpawnPacket.bnpcBaseId.ToString());
                writer.WriteElementString("ModelId", actorSpawnPacket.model.ToString());
                writer.WriteElementString("ClassJob", actorSpawnPacket.classJob.ToString());
                writer.WriteElementString("CurrentMount", actorSpawnPacket.currentMount.ToString());
                writer.WriteElementString("MountBody", actorSpawnPacket.mountBody.ToString());
                writer.WriteElementString("MountColor", actorSpawnPacket.mountColor.ToString());
                writer.WriteElementString("MountFeet", actorSpawnPacket.mountFeet.ToString());
                writer.WriteElementString("MountHead", actorSpawnPacket.mountHead.ToString());
                writer.WriteElementString("DisplayFlags1", actorSpawnPacket.displayFlags1.ToString());
                writer.WriteElementString("DisplayFlags2", actorSpawnPacket.displayFlags2.ToString());
                writer.WriteElementString("Level", actorSpawnPacket.level.ToString());
                writer.WriteElementString("MaxHP", actorSpawnPacket.hPMax.ToString());
                writer.WriteElementString("MaxMP", actorSpawnPacket.mPMax.ToString());
                writer.WriteElementString("MaxTP", actorSpawnPacket.tPMax.ToString());
                writer.WriteElementString("Status", actorSpawnPacket.status.ToString());
                writer.WriteElementString("OwnerID", actorSpawnPacket.ownerId.ToString());
                writer.WriteElementString("TargetID", actorSpawnPacket.targetId.ToString());
                writer.WriteElementString("Pos_0_0", actorSpawnPacket.posx.ToString());
                writer.WriteElementString("Pos_0_1", actorSpawnPacket.posy.ToString());
                writer.WriteElementString("Pos_0_2", actorSpawnPacket.posz.ToString());
                writer.WriteElementString("Rotation", actorSpawnPacket.rotation.ToString());
                writer.WriteElementString("MobType", actorSpawnPacket.mobType.ToString());
                writer.WriteElementString("ModelMainWeapon", actorSpawnPacket.mainWeaponModel.ToString());
                writer.WriteElementString("ModelSubWeapon", actorSpawnPacket.secWeaponModel.ToString());
                writer.WriteElementString("ModelCraftTool", actorSpawnPacket.craftToolModel.ToString());
                writer.WriteElementString("Look", BitConverter.ToString(actorSpawnPacket.lookdata).Replace("-", " "));
                writer.WriteElementString("Models", BitConverter.ToString(actorSpawnPacket.models).Replace("-", " "));

                writer.WriteEndElement();
                writer.WriteEndDocument();

                Console.WriteLine($"    -> wrote {sourceId}.chardef.xml");
            }
        }

        public static void addEffect(EffectPacket effectPacket, uint sourceId, string outputFolderName)
        {
            if (!File.Exists(Path.Combine(outputFolderName, $"{sourceId}.mobdef.xml")))
            {
                Console.WriteLine($"    -> Definition file {sourceId}.mobdef.xml not found");
                return;
            }
            
            try
            {
                XDocument doc = XDocument.Parse(File.ReadAllText(Path.Combine(outputFolderName, $"{sourceId}.mobdef.xml")));

                XElement epEntry = new XElement("EffectPacket");
                epEntry.Add(new XElement("TargetID", effectPacket.targetId));
                epEntry.Add(new XElement("Unknown_1", effectPacket.unknown_1));
                epEntry.Add(new XElement("ActionAnimationID", effectPacket.actionAnimationId));
                epEntry.Add(new XElement("Unknown_2", effectPacket.unknown_2));
                epEntry.Add(new XElement("Unknown_3", effectPacket.unknown_3));
                epEntry.Add(new XElement("ActionTextID", effectPacket.actionTextId));
                epEntry.Add(new XElement("Unknown_5", effectPacket.unknown_5));
                epEntry.Add(new XElement("Unknown_6", effectPacket.unknown_6));
                epEntry.Add(new XElement("Unknown_7", effectPacket.unknown_7));
                epEntry.Add(new XElement("Rotation", effectPacket.rotation));
                epEntry.Add(new XElement("EffectTargetID", effectPacket.effectTargetId));
                epEntry.Add(new XElement("Unknown_10", effectPacket.unknown_10));
                epEntry.Add(new XElement("Unknown_11", effectPacket.unknown_11));

                XElement eEntries = new XElement("EffectEntries");

                foreach (EffectEntry e in effectPacket.effects)
                {
                    XElement thisEffect = new XElement("EffectEntry");

                    thisEffect.Add(new XElement("Unknown_1", e.unknown_1));
                    thisEffect.Add(new XElement("Unknown_2", e.unknown_2));
                    thisEffect.Add(new XElement("Unknown_3", e.unknown_3));
                    thisEffect.Add(new XElement("BonusPercent", e.bonusPercent));
                    thisEffect.Add(new XElement("Param1", e.param1));
                    thisEffect.Add(new XElement("Unknown_5", e.unknown_5));
                    thisEffect.Add(new XElement("Unknown_6", e.unknown_6));

                    eEntries.Add(thisEffect);
                }

                epEntry.Add(eEntries);
                doc.Element("Mob").Element("EffectPackets").Add(epEntry);

                doc.Save(Path.Combine(outputFolderName, $"{sourceId}.mobdef.xml"));
            }catch(Exception exc)
            {
                Console.WriteLine($"    -> writing failed: " + exc);
            }

            Console.WriteLine($"    -> updated {sourceId}.mobdef.xml");
        }
    }
}
