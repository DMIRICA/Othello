using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Packets
{
    class LoginPacket : PacketStructure
    {
        //USERNAME|PASSWORD
        private string Message;

        public LoginPacket(byte[] packet) : base(packet)
        {
            Message = ReadString(4, packet.Length - 4);
            string[] fields = Message.Split('|');
            string username = fields[0];
            string password = fields[1];
            bool match = Singleton.Singleton.Instance.DatabaseConnection.isPasswordRight(username, password);
            if (match)
            {
                //responseBack(clientSocket, GameProtocol.SuccesLoginPackedID());
                //Othello.Server.SendPacket(clientSocket, new BasicPacket(GameProtocol.SuccesLoginPackedID()).getData());
            }
            else
            {
                //responseBack(clientSocket, GameProtocol.FailedLoginPacketID());
                //Othello.Server.SendPacket(clientSocket, new BasicPacket(GameProtocol.FailedLoginPacketID()).getData());
              // Othello.Server.SendPacket(clientSocket, packet);
            }
        }

        public void doLogin(Socket clientSocket)
        {
            string[] fields = Message.Split('|');
            string username = fields[0];
            string password = fields[1];
            bool match = Singleton.Singleton.Instance.DatabaseConnection.isPasswordRight(username, password);
            if (match)
            {
                //responseBack(clientSocket, GameProtocol.SuccesLoginPackedID());
               
                Othello.Server.SendPacket(clientSocket, new BasicPacket(GameProtocol.SuccesLoginPackedID()).getData());
            }
            else
            {
                //responseBack(clientSocket, GameProtocol.FailedLoginPacketID());
                BasicPacket bp = new BasicPacket(GameProtocol.FailedLoginPacketID());
                Othello.Server.SendPacket(clientSocket, bp.getData());

            }
        }

        public void responseBack(Socket socket, ushort packetID)
        {
            Othello.Server.SendPacket(socket, new BasicPacket(packetID).getData());
        }
    }

}

