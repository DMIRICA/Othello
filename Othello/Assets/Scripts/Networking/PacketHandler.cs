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
                    LoginScript.runInMainThread(LoginScript.updatePopUpText("Success!"));
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
                    messagePacket.runInMainThread(messagePacket.loadMainScene());
                    break;

                case 121: //NEW USER LOGIN - ADD HIM TO LIST
                    messagePacket = new MessagePacket(packet);
                    messagePacket.runInMainThread(messagePacket.addNewUserLoggedToList());
                    break;

                case 200: // GLOBAL CHAT MESSAGE
                    messagePacket = new MessagePacket(packet);
                    messagePacket.runInMainThread(messagePacket.updateGlobalChat());
                    break;

                case 201: //CHAT MESSAGE
                    RoomChatMessage Message = new RoomChatMessage(packet);
                    break;

                case 250: //CHALLENGE MESSAGE
                    messagePacket = new MessagePacket(packet);
                    messagePacket.runInMainThread(messagePacket.getChallenged());
                    break;

                case 270: //USER IGNORED THE CHALLENGE -> SET THE USER ONLINE STATUS ( PARAM 0 )
                    messagePacket = new MessagePacket(packet);
                    messagePacket.runInMainThread(messagePacket.updateUserStatus(0));
                    break;

                case 271: //NOTIFY WITH USER WHO GOT CHALLENGED -> SET THE USER CHALLENGED STATUS ( PARAM 1 )
                    messagePacket = new MessagePacket(packet);
                    messagePacket.runInMainThread(messagePacket.updateUserStatus(1));
                    break;

                case 257: //CHALLENGE ACCEPTED
                    messagePacket = new MessagePacket(packet);
                    //TODO
                    break;

                case 258: //CHALLENGE REFUSED
                    messagePacket = new MessagePacket(packet);
                    messagePacket.runInMainThread(
                        messagePacket.displayChallengeResultPanel(messagePacket.Message+ " didn't accept your challenge request!"));
                    break;

                case 260: //CHALLENGE TIMEOUT
                    messagePacket = new MessagePacket(packet);
                    messagePacket.runInMainThread(
                        messagePacket.displayChallengeResultPanel(messagePacket.Message + 
                        " didn't respond in time to your challenge request"));
                    break;

                case 401: // STARTGAME MESSAGE EX: False|R or True|B
                    GamePacket startPacket = new GamePacket(packet);
                    break;

                case 400: // BOARDGAME ARRAY
                    BoardPacket boardPacket = new BoardPacket(packet);
                    break;

                case 402: // GET ALL LEGAL MOVES 
                    GamePacket moves = new GamePacket(packet);
                    break;

                case 404:// GET MY TURN
                    GamePacket myturn = new GamePacket(packet);
                    break;

                case 405://GAMEOVER PACKET
                    GamePacket GameOver = new GamePacket(packet);
                    break;

                case 999://USER DISCONNECTED
                    messagePacket = new MessagePacket(packet);
                    messagePacket.runInMainThread(messagePacket.userDisconnected());
                    break;
            }

        }

        
    }
}
