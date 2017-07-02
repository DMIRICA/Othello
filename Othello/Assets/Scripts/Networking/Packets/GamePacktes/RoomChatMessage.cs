using Assets.Scripts.Game;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace Assets.Scripts.Networking.Packets.GamePackets
{
    public class RoomChatMessage : PacketStructure
    {

        public string Message;
        public ushort RoomID;

        public RoomChatMessage(byte[] packet) : base(packet)
        {
            Message = ReadString(6, packet.Length - 6);
            RoomID = ReadUShort(4);

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                Chat.ChatBox.text += Message;
                
            });
        }

        public RoomChatMessage(ushort roomID,string Message) :
            base(GameProtocol.RoomChatMessagePacketID(),(ushort)(6 + Message.Length))
        {
            RoomID = roomID;
            WriteUShort(RoomID, 4);
            this.Message = Message;
            WriteString(Message, 6);
        }

        

    }
}
