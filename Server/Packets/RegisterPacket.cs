using Server.Protocol;
using System.Net.Sockets;
using Server.Entities;

namespace Server.Packets
{
    class RegisterPacket : PacketStructure
    {
        //USERNAME|PASSWORD|EMAIL
        private string Message;

        public RegisterPacket(byte[] packet) : base(packet)
        {
            Message = ReadString(4, packet.Length - 4);
        }

        public void registerAccount(Socket clientSocket)
        {
            string[] fields = Message.Split('|');
            string username = fields[0];
            string password = fields[1];
            string email = fields[2];
            bool usernameUsed = Singleton.Singleton.Instance.DatabaseConnection.isUsernameUsed(username);
            if (usernameUsed)
            {
                responseBack(clientSocket, GameProtocol.UsernameAlreadyUsedPacketID());
            }
            else
            {
                bool emailUsed = Singleton.Singleton.Instance.DatabaseConnection.isEmailUsed(email);
                if (emailUsed)
                {
                    responseBack(clientSocket, GameProtocol.EmailAlreadyUsedPacketID());
                }
                else
                {
                    bool done = Singleton.Singleton.Instance.DatabaseConnection.addUser(new User(0, username, password, email));

                    if (done)
                    {
                        responseBack(clientSocket, GameProtocol.SuccesCreateAccountPacketID());
                    }
                    else
                    {
                        responseBack(clientSocket, GameProtocol.FailedCreateAccountPacketID());
                    }

                }

            }
        }

        public void responseBack(Socket socket,ushort packetID)
        {
            Othello.Server.SendPacket(socket, new BasicPacket(packetID).getData());
        }
    }
}
