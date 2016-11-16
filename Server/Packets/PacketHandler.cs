using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Singleton;
using Server.Game;

namespace Server.Packets
{
    public static class PacketHandler
    {
       
        public static void Handle(byte[] packet, Socket clientSocket)
        {
            ushort packetType = BitConverter.ToUInt16(packet, 0);
            ushort packetLength = BitConverter.ToUInt16(packet, 2);
            ushort roomID = BitConverter.ToUInt16(packet, 4);
            Room CurrentRoom = Singleton.Singleton.Instance.GetRoomByID(roomID);

            switch (packetType)
            {
                case 100: // CHAT MESSAGE
                    ChatMessage Message = new ChatMessage(packet);
                    if (clientSocket == CurrentRoom.Player1.PlayerSocket)
                    {
                        ReversiServer.Server.SendPacket(CurrentRoom.Player2.PlayerSocket, packet);
                    }
                    else if(clientSocket == CurrentRoom.Player2.PlayerSocket)
                    {
                        ReversiServer.Server.SendPacket(CurrentRoom.Player1.PlayerSocket, packet);
                    }
                    CurrentRoom.ChatHistory.Add(Message.Message);

                    break;

                case 403: // Turn Move
                    GamePacket TurnMove = new GamePacket(packet);
                    break;

                case 999:// Player Quit
                    foreach (Player p in Singleton.Singleton.Instance.ListOfPlayers)
                    {
                        if (p.PlayerSocket == clientSocket)
                        {
                            Singleton.Singleton.Instance.ListOfPlayers.Remove(p);
                            break;
                        }
                    }
                    ReversiServer.Server._ClientSockets.Remove(clientSocket);
                    Console.WriteLine("Client Disconnected!");
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    break;

                case 406://Play Again
                    if (CurrentRoom.Player1.PlayerSocket == clientSocket)
                    {
                        CurrentRoom.Player1.PlayAgain = true;
                        if (CurrentRoom.Player2.PlayAgain)
                        {
                            CurrentRoom.PlayAgain();
                            CurrentRoom.ResetPlayAgain();
                        }
                    }
                    else if(CurrentRoom.Player2.PlayerSocket == clientSocket)
                    {
                        CurrentRoom.Player2.PlayAgain = true;
                        if (CurrentRoom.Player1.PlayAgain)
                        {
                            CurrentRoom.PlayAgain();
                            CurrentRoom.ResetPlayAgain();
                        }
                    }
                    break;

                case 407://Surrender
                    if(CurrentRoom.Player1.PlayerSocket == clientSocket)
                    {
                        CurrentRoom.SendGameOver(CurrentRoom.Player2,
                            "You won!\n Your opponent surrenders.");
                        CurrentRoom.SendGameOver(CurrentRoom.Player1,
                            "You lost!\n You Surrendered");
                    }
                    else if(CurrentRoom.Player2.PlayerSocket == clientSocket)
                    {
                        CurrentRoom.SendGameOver(CurrentRoom.Player1,
                            "You won!\n Your opponent surrenders.");
                        CurrentRoom.SendGameOver(CurrentRoom.Player2,
                            "You lost!\n You Surrendered");
                    }
                    break;
            }

        }
    }
}
