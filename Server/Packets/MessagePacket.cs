using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Packets
{
    class MessagePacket : PacketStructure
    {
        private string Message;
        private int PacketID;
        private int Length;

        public MessagePacket(byte[] packet) : base(packet)
        {
            PacketID    = ReadUShort(0);
            Length      = ReadUShort(2);
            Message     = ReadString(4, packet.Length - 4);
        }

        public MessagePacket(ushort PacketID, string Message) : 
            base(PacketID, (ushort)(4+Message.Length))
        {
            this.Message = Message;
            this.PacketID = PacketID;
            this.Length = 4 + Message.Length;
            WriteString(Message, 4);
        }

        public string MessageP { get => Message; set => Message = value; }
    }
}
