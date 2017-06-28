using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;

namespace Server.Packets
{
    class LoginPacket : PacketStructure
    {
        //USERNAME|PASSWORD
        private string Message;

        public LoginPacket(byte[] packet) : base(packet)
        {
            Message = ReadString(4, packet.Length - 4);
        }

        public void doLogin(Socket clientSocket)
        {
            string[] fields = Message.Split('|');
            string username = fields[0];
            string password = fields[1];
            User user = Singleton.Singleton.Instance.DatabaseConnection.isPasswordRight(username, password);
            if (user != null)
            {
                user.IsBusy = false;
                user.Socket = clientSocket;
                Singleton.Singleton.Instance.ListOfUsersLogged.Add(user);
                //Send to current user logged the list with all the users logged and the status of them
                string PacketMessage = "";
                foreach (User u in Singleton.Singleton.Instance.ListOfUsersLogged)
                {
                    if (u.Username == username)
                        continue;
                    PacketMessage += u.Username + ":" + u.IsBusy + "|";
                    MessagePacket messagePacket = new MessagePacket(GameProtocol.AlertUsersNewUserLoggedID(), username + ":False");
                    Othello.Server.SendPacket(u.Socket,messagePacket.getData());
                }
                MessagePacket packet = new MessagePacket(GameProtocol.UsersLoggedListPacketID(), PacketMessage);
                Othello.Server.SendPacket(user.Socket, packet.getData());
            }
            else
            {
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

