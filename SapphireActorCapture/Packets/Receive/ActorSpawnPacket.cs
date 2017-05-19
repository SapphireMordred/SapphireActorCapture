using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireActorCapture.Packets.Receive
{
    public class ActorSpawnPacket //This should be redone at some point cause it doesn't work
    {
        public bool invalidPacket = false;

        public readonly Int32 unknown_0;
        public readonly Int32 nameId;
        public readonly Int32 sizeId;
        public readonly Int32[] unknown_C = new Int32[2];
        public readonly Int32 ownerId;
        public readonly Int32[] unknown_E = new Int32[2];
        public readonly Int64 targetId;
        public readonly Int32 unknown_48;
        public readonly Byte unknown_2C;
        public readonly Byte spawnIndex;
        public readonly Byte status;
        public readonly Byte pose;
        public readonly Byte mobAgressive;
        public readonly Byte mobType;
        public readonly Byte type;
        public readonly Byte unknown_33;
        public readonly Int32 unknown_34;
        public readonly Int64 unknown_38;
        public readonly Int64 unknown_90;
        public readonly Int64 mainWeaponModel;
        public readonly Int64 secWeaponModel;
        public readonly Int64 craftToolModel;
        public readonly Int16 rotation;
        public readonly Int16 model;
        public readonly Int16 title;
        public readonly Byte unknown_5E;
        public readonly Byte minion;
        public readonly Byte unknown_60;
        public readonly Byte unknown_61;
        public readonly Byte level;
        public readonly Byte classJob;
        public readonly Int32 hPCurr;
        public readonly Int16 mPCurr;
        public readonly Int16 tPCurr;
        public readonly Int32 hPMax;
        public readonly Int16 mPMax;
        public readonly Int16 tPMax;
        public readonly Byte statusIcon;
        public readonly byte[] unknown_B0 = new byte[17];
        public readonly Byte persistantPose;
        public readonly Byte unknown_C0;
        public readonly Byte displayFlags1;
        public readonly Byte displayFlags2;
        public readonly Int16 unknown_C1;
        public readonly byte[] statusEffects = new byte[360];
        public readonly Byte currentMount;
        public readonly Byte mountHead;
        public readonly Byte mountBody;
        public readonly Byte mountFeet;
        public readonly Int32 mountColor;
        public readonly Int32 unknown_236;
        public readonly string name;
        public readonly byte[] lookdata = new byte[28];
        public readonly byte[] models = new byte[40];
        public readonly float posx;
        public readonly float posy;
        public readonly float posz;
        public readonly string fcTag;
        public readonly byte[] unknown_250 = new byte[10];
        public readonly int territoryId = 0;
        public readonly uint id = 0;
        

        public ActorSpawnPacket(byte[] buffer, int territoryId, uint id)
        {
            this.territoryId = territoryId;
            this.id = id;

            using (MemoryStream memStream = new MemoryStream(buffer))
            {
                using (BinaryReader binReader = new BinaryReader(memStream))
                {
                    try
                    {
                        unknown_0 = binReader.ReadInt32();
                        nameId = binReader.ReadInt32();
                        sizeId = binReader.ReadInt32();
                        unknown_C[0] = binReader.ReadInt32();
                        unknown_C[1] = binReader.ReadInt32();
                        ownerId = binReader.ReadInt32();
                        unknown_E[0] = binReader.ReadInt32();
                        unknown_E[1] = binReader.ReadInt32();
                        targetId = binReader.ReadInt64();
                        unknown_48 = binReader.ReadInt32();
                        unknown_2C = binReader.ReadByte();
                        spawnIndex = binReader.ReadByte();
                        status = binReader.ReadByte();
                        pose = binReader.ReadByte();
                        mobAgressive = binReader.ReadByte();
                        mobType = binReader.ReadByte();
                        type = binReader.ReadByte();
                        unknown_33 = binReader.ReadByte();
                        unknown_34 = binReader.ReadInt32();
                        unknown_38 = binReader.ReadInt64();
                        unknown_90 = binReader.ReadInt64();
                        mainWeaponModel = binReader.ReadInt64();
                        secWeaponModel = binReader.ReadInt64();
                        craftToolModel = binReader.ReadInt64();
                        rotation = binReader.ReadInt16();
                        model = binReader.ReadInt16();
                        title = binReader.ReadInt16();
                        unknown_5E = binReader.ReadByte();
                        minion = binReader.ReadByte();
                        unknown_60 = binReader.ReadByte();
                        unknown_61 = binReader.ReadByte();
                        level = binReader.ReadByte();
                        classJob = binReader.ReadByte();
                        hPCurr = binReader.ReadInt32();
                        mPCurr = binReader.ReadInt16();
                        tPCurr = binReader.ReadInt16();
                        hPMax = binReader.ReadInt32();
                        mPMax = binReader.ReadInt16();
                        tPMax = binReader.ReadInt16();
                        statusIcon = binReader.ReadByte();
                        unknown_B0 = binReader.ReadBytes(17);
                        persistantPose = binReader.ReadByte();
                        unknown_C0 = binReader.ReadByte();
                        displayFlags1 = binReader.ReadByte();
                        displayFlags2 = binReader.ReadByte();
                        unknown_C1 = binReader.ReadInt16();
                        statusEffects = binReader.ReadBytes(360); //status effects
                        currentMount = binReader.ReadByte();
                        mountHead = binReader.ReadByte();
                        mountBody = binReader.ReadByte();
                        mountFeet = binReader.ReadByte();
                        mountColor = binReader.ReadInt32();
                        unknown_236 = binReader.ReadInt32();
                        name = Encoding.ASCII.GetString(binReader.ReadBytes(32)).TrimEnd('\x00');
                        lookdata = binReader.ReadBytes(28);
                        models = binReader.ReadBytes(40);
                        posx = binReader.ReadSingle();
                        posy = binReader.ReadSingle();
                        posz = binReader.ReadSingle();
                        fcTag = Encoding.ASCII.GetString(binReader.ReadBytes(6)).TrimEnd('\x00');
                        unknown_250 = binReader.ReadBytes(10);
                        binReader.ReadBytes(22); //?
                    }
                    catch (Exception exc)
                    {
                        invalidPacket = true;
                        Console.WriteLine(exc);
                    }
                }
            }
        }
    }
}
