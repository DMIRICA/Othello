using Assets.Scripts.Networking.GamePackets;
using Assets.Scripts.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Assets.Scripts.Networking.GamePacktes
{

    class MessagePacket : PacketStructure
    {
        //USERNAME|PASSWORD
        private string Message;

        public MessagePacket(byte[] packet) : base(packet)
        {
            Message = ReadString(4, packet.Length - 4);
        }

        public MessagePacket(ushort PacketID, string Message) : 
            base(PacketID, (ushort)(4+Message.Length))
        {
            this.Message = Message;
            WriteString(this.Message, 4);
        }
    }
}

