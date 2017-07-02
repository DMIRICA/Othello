using Server.Game;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Packets
{
    public class RoomChatPacket : PacketStructure
    {
        public string Message;
        public ushort RoomID;
        Room CurrentRoom;

        public RoomChatPacket(byte[] packet) : base(packet)
        {
            Message = ReadString(6, packet.Length - 6);
            RoomID = ReadUShort(4);
            CurrentRoom = Singleton.Singleton.Instance.GetRoomByID(RoomID);
            
        }

        public RoomChatPacket(ushort room,string Message) : 
            base(GameProtocol.RoomChatMessagePacketID(), (ushort)(4+Message.Length))
        {
            RoomID = room;
            this.Message = Message;
            CurrentRoom = Singleton.Singleton.Instance.GetRoomByID(RoomID);
            WriteString(Message, 4);
        }

        public void doChat(Socket clientSocket,byte[] packet)
        {
            if (clientSocket == CurrentRoom.Player1.PlayerSocket)
            {
                Othello.Server.SendPacket(CurrentRoom.Player2.PlayerSocket, packet);
            }
            else if (clientSocket == CurrentRoom.Player2.PlayerSocket)
            {
                Othello.Server.SendPacket(CurrentRoom.Player1.PlayerSocket, packet);
            }
            CurrentRoom.ChatHistory.Add(Message);
        }
    }
}
