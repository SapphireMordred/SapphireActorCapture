using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace SapphireActorCapture.Packets
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BasePacketHeader
    {
        public ulong garbo1, garbo2;
        public ulong timestamp; //Miliseconds        
        public uint packetSize;
        public ushort unknown1;
        public ushort numSubpackets;
        public byte isAuthenticated;
        public byte isCompressed;
        //public ushort connectionType;
        public uint unknown2;
    }

    public class BasePacket
    {
        public const int TYPE_ZONE = 1;
        public const int TYPE_CHAT = 2;
        public const int BASEPACKET_SIZE = 0x28;
        public byte[] data;

        public BasePacketHeader header;

        //Loads a sniffed packet from a file
        public unsafe BasePacket(string path)
        {
            var bytes = File.ReadAllBytes(path);

            if (bytes.Length < BASEPACKET_SIZE)
                throw new OverflowException("Packet Error: Packet was too small");

            fixed (byte* pdata = &bytes[0])
            {
                header = (BasePacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(BasePacketHeader));
            }

            if (bytes.Length < header.packetSize)
                throw new OverflowException("Packet Error: Packet size didn't equal given size");

            uint packetSize = header.packetSize;

            if (packetSize - BASEPACKET_SIZE != 0)
            {
                data = new byte[packetSize - BASEPACKET_SIZE];
                Array.Copy(bytes, BASEPACKET_SIZE, data, 0, packetSize - BASEPACKET_SIZE);
            }
            else
                data = new byte[0];
        }

        //Loads a sniffed packet from a byte array
        public unsafe BasePacket(byte[] bytes)
        {
            if (bytes.Length < BASEPACKET_SIZE)
                throw new OverflowException("Packet Error: Packet was too small");

            fixed (byte* pdata = &bytes[0])
            {
                header = (BasePacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(BasePacketHeader));
            }

            if (bytes.Length < header.packetSize)
                throw new OverflowException("Packet Error: Packet size didn't equal given size");

            uint packetSize = header.packetSize;

            data = new byte[packetSize - BASEPACKET_SIZE];
            Array.Copy(bytes, BASEPACKET_SIZE, data, 0, packetSize - BASEPACKET_SIZE);
        }

        public unsafe BasePacket(byte[] bytes, ref uint offset)
        {
            if (bytes.Length < offset + BASEPACKET_SIZE)
                throw new OverflowException("Packet Error: Packet was too small");

            fixed (byte* pdata = &bytes[offset])
            {
                header = (BasePacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(BasePacketHeader));
            }

            uint packetSize = header.packetSize;

            if (bytes.Length < offset + header.packetSize)
                throw new OverflowException("Packet Error: Packet size didn't equal given size");

            data = new byte[packetSize - BASEPACKET_SIZE];
            Array.Copy(bytes, offset + BASEPACKET_SIZE, data, 0, packetSize - BASEPACKET_SIZE);

            offset += packetSize;
        }

        public BasePacket(BasePacketHeader header, byte[] data)
        {
            this.header = header;
            this.data = data;
        }

        public List<SubPacket> GetSubpackets()
        {
            var subpackets = new List<SubPacket>(header.numSubpackets);

            uint offset = 0;

            while (offset < data.Length)
                subpackets.Add(new SubPacket(data, ref offset));

            return subpackets;
        }

        public static unsafe BasePacketHeader GetHeader(byte[] bytes)
        {
            BasePacketHeader header;
            if (bytes.Length < BASEPACKET_SIZE)
                throw new OverflowException("Packet Error: Packet was too small");

            fixed (byte* pdata = &bytes[0])
            {
                header = (BasePacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(BasePacketHeader));
            }

            return header;
        }

        public byte[] GetHeaderBytes()
        {
            var size = Marshal.SizeOf(header);
            var arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(header, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public byte[] GetPacketBytes()
        {
            var outBytes = new byte[header.packetSize];
            Array.Copy(GetHeaderBytes(), 0, outBytes, 0, BASEPACKET_SIZE);
            Array.Copy(data, 0, outBytes, BASEPACKET_SIZE, data.Length);
            return outBytes;
        }

        //Replaces all instances of the sniffed actorID with the given one
        public void ReplaceActorID(uint actorID)
        {
            using (var mem = new MemoryStream(data))
            {
                using (var binWriter = new BinaryWriter(mem))
                {
                    using (var binreader = new BinaryReader(mem))
                    {
                        while (binreader.BaseStream.Position + 4 < data.Length)
                        {
                            var read = binreader.ReadUInt32();
                            if (read == 0x029B2941 || read == 0x02977DC7 || read == 0x0297D2C8 || read == 0x0230d573 ||
                                read == 0x23317df || read == 0x23344a3 || read == 0x1730bdb || read == 0x6c)
                                //Original ID
                            {
                                binWriter.BaseStream.Seek(binreader.BaseStream.Position - 0x4, SeekOrigin.Begin);
                                binWriter.Write(actorID);
                            }
                        }
                    }
                }
            }
        }

        //Replaces all instances of the sniffed actorID with the given one
        public void ReplaceActorID(uint fromActorID, uint actorID)
        {
            using (var mem = new MemoryStream(data))
            {
                using (var binWriter = new BinaryWriter(mem))
                {
                    using (var binreader = new BinaryReader(mem))
                    {
                        while (binreader.BaseStream.Position + 4 < data.Length)
                        {
                            var read = binreader.ReadUInt32();
                            if (read == fromActorID) //Original ID
                            {
                                binWriter.BaseStream.Seek(binreader.BaseStream.Position - 0x4, SeekOrigin.Begin);
                                binWriter.Write(actorID);
                            }
                        }
                    }
                }
            }
        }
        
        public void DebugPrintPacket()
        {
#if DEBUG            

            foreach (var sub in GetSubpackets())
            {
                sub.DebugPrintSubPacket();
            }
#endif
        }
        
        #region Utility Functions

        public static BasePacket CreatePacket(List<SubPacket> subpackets, bool isAuthed, bool isCompressed)
        {
            //Create Header
            var header = new BasePacketHeader();
            byte[] data = null;

            header.isAuthenticated = isAuthed ? (byte) 1 : (byte) 0;
            header.isCompressed = isCompressed ? (byte) 1 : (byte) 0;
            header.numSubpackets = (ushort) subpackets.Count;
            header.packetSize = BASEPACKET_SIZE;
            header.timestamp = Utils.MilisUnixTimeStampUTC();

            //Get packet size
            foreach (var subpacket in subpackets)
                header.packetSize += subpacket.header.subpacketSize;

            data = new byte[header.packetSize - 0x10];

            //Add Subpackets
            var offset = 0;
            foreach (var subpacket in subpackets)
            {
                var subpacketData = subpacket.GetBytes();
                Array.Copy(subpacketData, 0, data, offset, subpacketData.Length);
                offset += (ushort) subpacketData.Length;
            }

            Debug.Assert(data != null && offset == data.Length && header.packetSize == 0x10 + offset);

            var packet = new BasePacket(header, data);
            return packet;
        }

        public static BasePacket CreatePacket(SubPacket subpacket, bool isAuthed, bool isCompressed)
        {
            //Create Header
            var header = new BasePacketHeader();
            byte[] data = null;

            header.isAuthenticated = isAuthed ? (byte) 1 : (byte) 0;
            header.isCompressed = isCompressed ? (byte) 1 : (byte) 0;
            header.numSubpackets = 1;
            header.packetSize = BASEPACKET_SIZE;
            header.timestamp = Utils.MilisUnixTimeStampUTC();

            //Get packet size
            header.packetSize += subpacket.header.subpacketSize;

            data = new byte[header.packetSize - 0x10];

            //Add Subpackets
            var subpacketData = subpacket.GetBytes();
            Array.Copy(subpacketData, 0, data, 0, subpacketData.Length);

            Debug.Assert(data != null);

            var packet = new BasePacket(header, data);
            return packet;
        }

        public static BasePacket CreatePacket(byte[] data, bool isAuthed, bool isCompressed)
        {
            Debug.Assert(data != null);

            //Create Header
            var header = new BasePacketHeader();

            header.isAuthenticated = isAuthed ? (byte) 1 : (byte) 0;
            header.isCompressed = isCompressed ? (byte) 1 : (byte) 0;
            header.numSubpackets = 1;
            header.packetSize = BASEPACKET_SIZE;
            header.timestamp = Utils.MilisUnixTimeStampUTC();

            //Get packet size
            header.packetSize += (ushort) data.Length;

            var packet = new BasePacket(header, data);
            return packet;
        }

        public static void DecompressPacket(ref BasePacket packet)
        {
            using (var compressedStream = new MemoryStream(packet.data))
            using (var zipStream = new ZlibStream(compressedStream, Ionic.Zlib.CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                packet.data = resultStream.ToArray();
            }
        }

        #endregion
    }

}