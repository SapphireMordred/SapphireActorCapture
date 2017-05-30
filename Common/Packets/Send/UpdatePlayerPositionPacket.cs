using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireActorCapture.Packets.Send
{
    public class UpdatePlayerPositionPacket
    {
        bool invalidPacket = false;

        public float x, y, z;

        public UpdatePlayerPositionPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        binReader.ReadBytes(0xC);
                        x = binReader.ReadSingle();
                        z = binReader.ReadSingle();
                        y = binReader.ReadSingle();
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
