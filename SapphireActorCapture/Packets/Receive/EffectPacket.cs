using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireActorCapture.Packets.Receive
{
    class EffectPacket
    {
        public bool invalidPacket = false;

        public readonly uint targetId;
        public readonly uint unknown_1;
        public readonly uint actionAnimationId;
        public readonly byte unknown_2;
        public readonly byte unknown_3;
        public readonly uint actionTextId;
        public readonly uint unknown_5;
        public readonly uint unknown_6;
        /* padding */
        public readonly byte unknown_7;
        /* padding */
        public readonly uint rotation;
        public readonly uint effectTargetId;
        public readonly uint unknown_10;
        public readonly uint unknown_11;
        public readonly EffectEntry[] effects = new EffectEntry[8];
        public readonly ulong unknown_8;

        public EffectPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        targetId = binReader.ReadUInt32();
                        unknown_1 = binReader.ReadUInt32();
                        actionAnimationId = binReader.ReadUInt16();
                        unknown_2 = binReader.ReadByte();
                        unknown_3 = binReader.ReadByte();
                        actionTextId = binReader.ReadUInt32();
                        unknown_5 = binReader.ReadUInt32();
                        unknown_6 = binReader.ReadUInt32();
                        binReader.ReadUInt32(); /* padding */
                        unknown_7 = binReader.ReadByte();
                        binReader.ReadByte(); /* padding */
                        rotation = binReader.ReadUInt16();
                        effectTargetId = binReader.ReadUInt32();
                        unknown_10 = binReader.ReadUInt32();
                        unknown_11 = binReader.ReadUInt32();
                        
                        for (int i = 0; i < 8; i++)
                        {
                            effects[i] = new EffectEntry(binReader.ReadByte(), binReader.ReadByte(), binReader.ReadByte(), binReader.ReadByte(), binReader.ReadInt16(), binReader.ReadByte(), binReader.ReadByte());
                        }

                        unknown_8 = BitConverter.ToUInt64(binReader.ReadBytes(8), 0);
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                        Console.WriteLine(Utils.ByteArrayToHex(data));
                        invalidPacket = true;
                    }
                }
            }
        }
    }

    public class EffectEntry
    {
        public readonly byte unknown_1;
        public readonly byte unknown_2;
        public readonly byte unknown_3;
        public readonly byte bonusPercent;
        public readonly int param1;
        public readonly byte unknown_5;
        public readonly byte unknown_6;

        public EffectEntry(byte unknown_1, byte unknown_2, byte unknown_3, byte bonusPercent, int param1, byte unknown_5, byte unknown_6)
        {
            this.unknown_1 = unknown_1;
            this.unknown_2 = unknown_2;
            this.unknown_3 = unknown_3;
            this.bonusPercent = bonusPercent;
            this.param1 = param1;
            this.unknown_5 = unknown_5;
            this.unknown_6 = unknown_6;
        }
    }
}
