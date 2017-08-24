using Assets.Scripts.Game;
using Assets.Scripts.Networking.GamePackets;
using Assets.Scripts.Networking.Packets;
using Assets.Scripts.Networking.Packets.GamePackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public static class PacketHandler
    {

        public static void Handle(byte[] packet, Socket clientSocket)
        {
            ushort packetType = BitConverter.ToUInt16(packet, 0);
            ushort packetLength = BitConverter.ToUInt16(packet, 2);

            switch (packetType)
            {

                case 101: //SUCCESS LOGIN
                    //LoginScript.runInMainThread(LoginScript.updatePopUpText("Success!"));
                    break;

                case 102: //FAILED LOGIN
                    LoginScript.runInMainThread(LoginScript.updatePopUpText("Username and password do not match!"));
                    break;

                case 103://USER ALREADY LOGGED
                    LoginScript.runInMainThread(LoginScript.updatePopUpText("This user is already logged!"));
                    break;

                case 111: //SUCCESS REGISTER
                    BasicPacket SuccessRegisterPacket = new BasicPacket(packet);
                    SuccessRegisterPacket.runInMainThread(SuccessRegisterPacket.SuccessRegister());
                    break;

                case 112: //FAILED REGISTER
                    BasicPacket FailedRegisterPacket = new BasicPacket(packet);
                    FailedRegisterPacket.runInMainThread(FailedRegisterPacket.FailedRegister());
                    break;

                case 113: //USERNAME ALREADY USED
                    BasicPacket UsernameUsedPacket = new BasicPacket(packet);
                    UsernameUsedPacket.runInMainThread(UsernameUsedPacket.UsernameUsed());
                    break;

                case 114: //EMAIL ALREADY USED
                    BasicPacket EmailUsedPacket = new BasicPacket(packet);
                    EmailUsedPacket.runInMainThread(EmailUsedPacket.EmailUsed());
                    break;

                case 120: //USERS LIST AFTER LOGIN
                    MessagePacket messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.loadMainScene());
                    break;

                case 121: //NEW USER LOGIN - ADD HIM TO LIST
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.addNewUserLoggedToList());
                    break;

                case 200: // GLOBAL CHAT MESSAGE
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.updateGlobalChat());
                    break;

                case 201: //CHAT MESSAGE
                    MessageRoomPacket MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.runInMainThread(MessageRoomPacket.doRoomChat());
                    break;

                case 250: //CHALLENGE MESSAGE
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.getChallenged());
                    break;

                case 270: //USER IGNORED THE CHALLENGE -> SET THE USER ONLINE STATUS ( PARAM 0 )
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.updateUserStatus(0));
                    break;

                case 271: //NOTIFY WITH USER WHO GOT CHALLENGED -> SET THE USER CHALLENGED STATUS ( PARAM 1 )
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.updateUserStatus(1));
                    break;

                case 272: //NOTIFY WITH USER WHO IS IN GAME -> SET THE USER IN GAME STATUS ( PARAM 2 )
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.updateUserStatus(2));
                    break;

                case 257: //CHALLENGE ACCEPTED
                    messagePacket = new MessagePacket(packet);
                    //TODO
                    break;

                case 258: //CHALLENGE REFUSED
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(
                        messagePacket.displayChallengeResultPanel(messagePacket.Message+ " didn't accept your challenge request!"));
                    break;

                case 260: //CHALLENGE TIMEOUT
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(
                        messagePacket.displayChallengeResultPanel(messagePacket.Message + 
                        " didn't respond in time to your challenge request"));
                    break;


                case 400: // BOARDGAME ARRAY
                    BoardPacket boardPacket = new BoardPacket(packet);
                    break;

                case 402: // GET ALL LEGAL MOVES 
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.runInMainThread(MessageRoomPacket.DrawMoves());
                    break;

                case 401: // STARTGAME MESSAGE EX: False|R or True|B
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.runInMainThread(MessageRoomPacket.turnAndDiskColor());
                    break;

                case 404:// GET MY TURN
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.runInMainThread(MessageRoomPacket.getTurn());
                    break;

                case 405://GAMEOVER PACKET
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.callGameOver());
                    break;

                case 406://OPPONENT WANT TO PLAY AGAIN
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.updatePlayAgainPopUpText("Your opponent want to play again!"));
                    break;

                case 500://LOAD GAMESCENE
                    MessageRoomPacket = new MessageRoomPacket(packet);
                    MessageRoomPacket.runInMainThread(MessageRoomPacket.loadGameScene());
                    break;

                case 501://OPPONENT IS READY
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.opponentReady());
                    break;

                case 969://OPPONENT BACK TO LOBBY
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.opponentBackToLobby());
                    
                    break;

                case 995://BACK TO LOBBY
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.backToLobby());
                    break;

                case 996: // OPPONENT QUIT AFTER GAME END
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.opponentQuitAfterGameEnd());
                    break;

                case 997: //OPPONENT QUIT WHILE GAME IS ACTIVE
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.opponentQuitWhileInGame());
                    break;

                case 999://USER DISCONNECTED
                    messagePacket = new MessagePacket(packet);
                    MessagePacket.runInMainThread(messagePacket.userDisconnected());
                    break;
            }

        }

        
    }
}
