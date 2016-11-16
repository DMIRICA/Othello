using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Networking.GamePackets
{
    public abstract class PacketStructure
    {
        private byte[] _Bufffer;

        public PacketStructure(ushort packetID, ushort length)
        {
            _Bufffer = new byte[length];
            WriteUShort(packetID, 0);
            WriteUShort(length, 2);

        }

        public PacketStructure(byte[] packet)
        {
            _Bufffer = packet;
        }

        public void WriteUShort(ushort value, int offset)
        {
            byte[] temp = new byte[2];
            temp = BitConverter.GetBytes(value);
            Buffer.BlockCopy(temp, 0, _Bufffer, offset, 2);
        }

        public ushort ReadUShort(int offset)
        {
            return BitConverter.ToUInt16(_Bufffer, offset);
        }

        public void WriteUInt(uint value, int offset)
        {
            byte[] temp = new byte[4];
            temp = BitConverter.GetBytes(value);
            Buffer.BlockCopy(temp, 0, _Bufffer, offset, 4);
        }

        public void WriteString(string message, int offset)
        {
            byte[] temp = new byte[message.Length];
            temp = Encoding.UTF8.GetBytes(message);
            Buffer.BlockCopy(temp, 0, _Bufffer, offset, message.Length);
        }

        public string ReadString(int offset, int count)
        {
            return Encoding.UTF8.GetString(_Bufffer, offset, count);
        }

        public byte[] getData()
        {
            return _Bufffer;
        }
    }
}

