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
                    MessagePacket MessagePacket = new MessagePacket(packet);
                    MessagePacket.doLogin(clientSocket);
                    break;

                case 110: // Register
                    MessagePacket = new MessagePacket(packet);
                    MessagePacket.registerAccount(clientSocket);
                    break;

                case 200: // GLOBAL CHAT MESSAGE
                    MessagePacket = new MessagePacket(packet);
                    MessagePacket.doGlobalChat();
                    break;

                case 201: // ROOM CHAT MESSAGE
                    MessageRoomPacket MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.doRoomChat(clientSocket, packet);
                    break;

                case 250: // CHALLENGE PACKET (USERNAME CHALLANGED YOU ... )
                    MessagePacket = new MessagePacket(packet);
                    MessagePacket.sendAfterChallengeAction();
                    break;

                case 257: // USER ACCEPTED THE CHALLENGE
                    MessagePacket = new MessagePacket(packet);
                    MessagePacket.playerAcceptedTheChallenge();
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
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.doChangesAfterTurn();
                    break;

                case 406://Play Again
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.playAgain();
                    break;

                case 407://Surrender
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.surrender();
                    break;

                case 501: // PLAYER READY
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.playerReady();
                    break;

                case 995:// Back to lobby after gameover
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.backToLobby();
                    break;

                case 997:// User quit while in game -> send surr to opponent and disc to other users
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.opponentQuitWhileInGame();
                    break;

                case 996:// User quit after game -> send user left the room
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.opponentQuitAfterEndGame();
                    break;

                case 998:// User logout
                    BasicPacket basicPacket = new BasicPacket(packet);
                    basicPacket.userLogout(clientSocket);
                    break;

                case 999:// User apllication close
                    basicPacket = new BasicPacket(packet);
                    basicPacket.applicationClose(clientSocket);
                    break;

                
            }

        }
    }
}
