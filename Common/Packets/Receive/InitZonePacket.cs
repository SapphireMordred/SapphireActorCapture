using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireActorCapture.Packets.Receive
{
    public class InitZonePacket
    {
        public bool invalidPacket = false;

        public readonly uint unknown_0;
        public readonly uint zoneId;
        public readonly uint unknown_1;
        public readonly uint unknown_2;
        public readonly uint unknown_3;
        public readonly uint unknown_4;
        public readonly uint weatherId;
        public readonly uint bitmask;
        public readonly uint unknown_5;
        public readonly uint unknown_6;
        public readonly uint unknown_7;
        public readonly uint unknown_8;
        public readonly byte[] posx = new byte[4];
        public readonly byte[] posy = new byte[4];
        public readonly byte[] posz = new byte[4];

        public InitZonePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        unknown_0 = binReader.ReadUInt16();
                        zoneId = binReader.ReadUInt16();
                        unknown_1 = binReader.ReadUInt16();
                        unknown_2 = binReader.ReadUInt16();
                        unknown_3 = binReader.ReadUInt32();
                        unknown_4 = binReader.ReadUInt32();
                        weatherId = binReader.ReadByte();
                        bitmask = binReader.ReadByte();
                        unknown_5 = binReader.ReadUInt16();
                        unknown_6 = binReader.ReadUInt16();
                        unknown_7 = binReader.ReadUInt16();
                        unknown_8 = binReader.ReadUInt32();
                        posx = binReader.ReadBytes(4);
                        posy = binReader.ReadBytes(4);
                        posz = binReader.ReadBytes(4);
                    }
                    catch (Exception)
                    {
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}
