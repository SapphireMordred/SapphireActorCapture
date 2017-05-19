using SapphireActorCapture.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireActorCapture
{
    public class RemoteMon : MarshalByRefObject
    {

        private PacketProcessor processor = new PacketProcessor();
        //private int lastPartialSize;
        private byte[] buffer = new byte[4096];

        public void IsInstalled(int pid)
        {
            Console.WriteLine("RemoteMon: Dll has been installed!");
        }

        public void SendMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void GetRecv(byte[] buffer)
        {
            int bytesRead = buffer.Length;

            if (bytesRead >= 0)
            {
                uint offset = 0;             

                //Build packets until can no longer or out of data
                while (true)
                {
                    BasePacket basePacket = BuildPacket(ref offset, buffer, bytesRead);

                    //If can't build packet, break, else process another
                    if (basePacket == null)
                        break;
                    else
                        processor.ProcessPacket(basePacket, PacketProcessor.PPType.Recv);
                }
            }
        }

        public void GetSend(byte[] buffer)
        {
            int bytesRead = buffer.Length;
            
            uint offset = 0;

            BasePacket basePacket = BuildPacketS(ref offset, buffer, bytesRead);

            processor.ProcessPacket(basePacket, PacketProcessor.PPType.Recv);
        }

        public void ExceptionHandler(Exception e)
        {
            Console.WriteLine("=========================================================\nException in DLL!\n"+e);
        }

        public BasePacket BuildPacket(ref uint offset, byte[] buffer, int bytesRead)
        {
            BasePacket newPacket = null;

            //Too small to even get length
            if (bytesRead <= offset)
            {
                return null;
            }

            uint packetSize;
            try
            {
                packetSize = BitConverter.ToUInt32(buffer, (int)(offset + (8 * 3)));
            }
            catch (Exception)
            {
                return null;
            }
            

            //Too small to whole packet
            if (bytesRead < offset + packetSize)
            {
                return null;
            }

            if (buffer.Length < offset + packetSize)
            {
                return null;
            }

            try
            {
                newPacket = new BasePacket(buffer, ref offset);
            }
            catch (OverflowException)
            {
                return null;
            }

            return newPacket;
        }

        public BasePacket BuildPacketS(ref uint offset, byte[] buffer, int bytesRead)
        {
            BasePacket newPacket = null;

            try
            {
                uint e = 0;
                newPacket = new BasePacket(buffer, ref e);
            }
            catch (Exception exc)
            {
                Console.WriteLine("\nBuildPacket failed: Couldn't build BasePacket!\n" + exc + "\n" +  Utils.ByteArrayToHex(buffer));
                return null;
            }

            return newPacket;
        }

    }
}
