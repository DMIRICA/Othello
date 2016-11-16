using Assets.Scripts.Game;
using Assets.Scripts.Networking.GamePackets;
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
                case 100: // CHAT MESSAGE
                    ChatMessage Message = new ChatMessage(packet);
                    
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
