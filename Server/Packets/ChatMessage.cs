using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Packets
{
    public class ChatMessage : PacketStructure
    {
        public string Message;
        public ushort RoomID;


        public ChatMessage(byte[] packet) : base(packet)
        {
            Message = ReadString(6, packet.Length - 6);
            RoomID = ReadUShort(4);
        }

        public ChatMessage(ushort room,string Message) : 
            base(GameProtocol.ChatMessagePacketID(), (ushort)(4+Message.Length))
        {
            RoomID = room;
            this.Message = Message;
            WriteString(Message, 4);
        }

    }
}
