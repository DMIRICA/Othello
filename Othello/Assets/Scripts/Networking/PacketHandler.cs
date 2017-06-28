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
                    LoginScript.LoadGameScene();
                    break;

                case 102: //FAILED LOGIN
                    LoginScript.LoginFailed();
                    break;

                case 111: //SUCCESS REGISTER
                    BasicPacket SuccessRegisterPacket = new BasicPacket(packet);
                    SuccessRegisterPacket.RunInMainThread(SuccessRegisterPacket.SuccessRegister());
                    break;

                case 112: //FAILED REGISTER
                    BasicPacket FailedRegisterPacket = new BasicPacket(packet);
                    FailedRegisterPacket.RunInMainThread(FailedRegisterPacket.FailedRegister());
                    break;

                case 113: //USERNAME ALREADY USED
                    BasicPacket UsernameUsedPacket = new BasicPacket(packet);
                    UsernameUsedPacket.RunInMainThread(UsernameUsedPacket.UsernameUsed());
                    break;

                case 114: //EMAIL ALREADY USED
                    BasicPacket EmailUsedPacket = new BasicPacket(packet);
                    EmailUsedPacket.RunInMainThread(EmailUsedPacket.EmailUsed());
                    break;

                case 120: //USERS LIST AFTER LOGIN
                    MessagePacket messagePacket = new MessagePacket(packet);
                    messagePacket.RunInMainThread(messagePacket.loadMainScene());
                    break;

                case 121: //NEW USER LOGIN - ADD HIM TO LIST
                    messagePacket = new MessagePacket(packet);
                    messagePacket.RunInMainThread(messagePacket.addNewUserLoggedToList());
                    break;

                case 200: //CHAT MESSAGE
                    RoomChatMessage Message = new RoomChatMessage(packet);
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
            }

        }

        
    }
}
