using Server.Entities;
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
    class BasicPacket : PacketStructure
    {
        int packetID;

        public BasicPacket(byte[] packet) : base(packet)
        {
            packetID = ReadUShort(0);
        }

        public BasicPacket(ushort packetID) :
            base(packetID, 4)
        {

        }

        public void userLogout(Socket clientSocket)
        {
            User user = null;
            foreach (User u in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (u.Socket == clientSocket)
                {
                    user = u;
                    break;
                }
            }
            if (user != null)
            {
                MessagePacket packet = new MessagePacket(GameProtocol.UserDisconnected(), user.Username);
                foreach (User aux in Singleton.Singleton.Instance.ListOfUsersLogged)
                {
                    if (aux == user)
                        continue;
                    Othello.Server.SendPacket(aux.Socket, packet.getData());
                }
                Singleton.Singleton.Instance.ListOfUsersLogged.Remove(user);
            }
            Console.WriteLine("Client Logout!");
        }

        public void applicationClose(Socket clientSocket)
        {
            User user = null;
            foreach (User u in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (u.Socket == clientSocket)
                {
                    user = u;
                    break;
                }
            }

            if (user != null)
            {
                MessagePacket packet = new MessagePacket(GameProtocol.UserDisconnected(), user.Username);
                foreach (User aux in Singleton.Singleton.Instance.ListOfUsersLogged)
                {
                    if (aux == user)
                        continue;
                    Othello.Server.SendPacket(aux.Socket, packet.getData());
                }
                Singleton.Singleton.Instance.ListOfUsersLogged.Remove(user);
            }
            Console.WriteLine("Client Disconnected!");
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }



    }
}
