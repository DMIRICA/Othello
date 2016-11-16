using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Protocol
{
    public static class GameProtocol
    {

        public static ushort ChatMessagePacketID()
        {
            return 100;
        }

        public static ushort BoardTableGamePacketID()
        {
            return 400;
        }

        public static ushort StartGamePacketID()
        {
            return 401;
        }

        public static ushort BoardMoves()
        {
            return 402;
        }

        public static ushort TurnMovePacket()
        {
            return 403;
        }

        public static ushort PlayerTurnPacket()
        {
            return 404;
        }

        public static ushort GameOverPacket()
        {
            return 405;
        }

        public static ushort PlayAgain()
        {
            return 406;
        }

        public static ushort Surrender()
        {
            return 407;
        }

    }
}
