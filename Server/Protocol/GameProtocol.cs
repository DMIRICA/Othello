using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Protocol
{
    public static class GameProtocol
    {
        #region Login Packets IDs

        public static ushort LoginPacketID()
        {
            return 100;
        }
        public static ushort SuccesLoginPackedID()
        {
            return 101;
        }
        public static ushort FailedLoginPacketID()
        {
            return 102;
        }


        #endregion

        #region Register Packets IDs
        public static ushort CreateAccountPacketID()
        {
            return 110;
        }

        public static ushort SuccesCreateAccountPacketID()
        {
            return 111;
        }

        public static ushort FailedCreateAccountPacketID()
        {
            return 112;
        }

        public static ushort UsernameAlreadyUsedPacketID()
        {
            return 113;
        }

        public static ushort EmailAlreadyUsedPacketID()
        {
            return 114;
        }
        #endregion

        

        public static ushort ChatMessagePacketID()
        {
            return 200;
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

        public static ushort PlayerQuit()
        {
            return 999;
        }

    }
}
