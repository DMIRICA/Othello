using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Singleton;
using Server.Game;
using Server.Protocol;

namespace Server.Packets
{
    public static class PacketHandler
    {
       
        public static void Handle(byte[] packet, Socket clientSocket)
        {
            ushort packetType = BitConverter.ToUInt16(packet, 0);
            ushort packetLength = BitConverter.ToUInt16(packet, 2);
            

            switch (packetType)
            {
                case 100: //Login
                    LoginPacket loginPacket = new LoginPacket(packet);
                    loginPacket.doLogin(clientSocket);
                    break;

                case 110: // Register
                    RegisterPacket registerPacket = new RegisterPacket(packet);
                    registerPacket.registerAccount(clientSocket);
                    break;

                case 200: // GLOBAL CHAT MESSAGE
                    MessagePacket MessagePacket = new MessagePacket(packet);
                    MessagePacket.doChat();
                    break;

                case 201: // ROOM CHAT MESSAGE
                    RoomChatPacket chatPacket = new RoomChatPacket(packet);
                    chatPacket.doChat(clientSocket, packet);
                    break;

                case 250: // CHALLENGE PACKET (USERNAME CHALLANGED YOU ... )
                    MessagePacket = new MessagePacket(packet);
                    MessagePacket.sendAfterChallengeAction();
                    break;

                case 257: // USER ACCEPTED THE CHALLENGE
                    MessagePacket = new MessagePacket(packet);
                    //TODO
                    //MessagePacket.notifyUsersAfterChallengeIgnore();
                    break;

                case 258: // USER REFUSED THE CHALLENGE
                    MessagePacket = new MessagePacket(packet);
                    MessagePacket.notifyUsersAfterChallengeRefuse();
                    break;

                case 260: // USER IGNORED THE CHALLENGE
                    MessagePacket = new MessagePacket(packet);
                    MessagePacket.notifyUsersAfterChallengeIgnore();
                    break;


                case 403: // Turn Move
                    GamePacket TurnMove = new GamePacket(packet);
                    TurnMove.doChangesAfterTurn();
                    break;

                case 998:// User logout
                    BasicPacket basicPacket = new BasicPacket(packet);
                    basicPacket.userLogout(clientSocket);
                    break;

                case 999:// User apllication close
                    basicPacket = new BasicPacket(packet);
                    basicPacket.applicationClose(clientSocket);
                    break;

                case 406://Play Again
                    ushort roomID = BitConverter.ToUInt16(packet, 4);
                    Room CurrentRoom = Singleton.Singleton.Instance.GetRoomByID(roomID);
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
                    roomID = BitConverter.ToUInt16(packet, 4);
                    CurrentRoom = Singleton.Singleton.Instance.GetRoomByID(roomID);
                    if (CurrentRoom.Player1.PlayerSocket == clientSocket)
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
