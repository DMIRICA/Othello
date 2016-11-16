using Assets.Scripts.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace Assets.Scripts.Networking.GamePackets
{
    public class ChatMessage : PacketStructure
    {

        public string Message;
        public ushort RoomID;

        public ChatMessage(byte[] packet) : base(packet)
        {
            Message = ReadString(6, packet.Length - 6);
            RoomID = ReadUShort(4);

            UnityThreadHelper.Dispatcher.Dispatch(() =>
            {
                Chat.ChatBox.text += Message;
                
            });
        }

        public ChatMessage(ushort roomID,string Message) :
            base(GameProtocol.ChatMessagePacketID(),(ushort)(6 + Message.Length))
        {
            RoomID = roomID;
            WriteUShort(RoomID, 4);
            this.Message = Message;
            WriteString(Message, 6);
        }

    }
}
