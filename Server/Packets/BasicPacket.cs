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

        public void playerQuit(Socket clientSocket)
        {
            if(packetID == GameProtocol.PlayerQuit())
            {
                foreach (Player p in Singleton.Singleton.Instance.ListOfPlayers)
                {
                    if (p.PlayerSocket == clientSocket)
                    {
                        Singleton.Singleton.Instance.ListOfPlayers.Remove(p);
                        break;
                    }
                }
                Othello.Server._ClientSockets.Remove(clientSocket);
                Console.WriteLine("Client Disconnected!");
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    
    }
}
