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

        public static ushort AlreadyOnlinePacketID()
        {
            return 103;
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

        #region After Login packets
        public static ushort UsersLoggedListPacketID()
        {
            return 120;
        }

        public static ushort AlertUsersNewUserLoggedID()
        {
            return 121;
        }
        #endregion

        #region Chat
        public static ushort GlobalChatMessagePacketID()
        {
            return 200;
        }
        public static ushort RoomChatMessagePacketID()
        {
            return 200;
        }
        #endregion

        #region Challenge
        public static ushort ChallengePacketID()
        {
            return 250;
        }


        public static ushort UserAcceptedChallengePacketID()
        {
            return 257;
        }


        public static ushort UserRefusedChallengePacketID()
        {
            return 258;
        }

        public static ushort ChallengeTimeoutPacketID()
        {
            return 260;
        }


        public static ushort ChangeUserToOnline()
        {
            return 270;
        }
        public static ushort ChangeUserToChallenged()
        {
            return 271;
        }

        public static ushort ChangeUserToInGame()
        {
            return 272;
        }

        #endregion

        public static ushort LoadGameScene()
        {
            return 500;
        }

        public static ushort PlayerReady()
        {
            return 501;
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
        public static ushort LoadMainSceneFromGame()
        {
            return 995;
        }

        public static ushort OpponentQuitAfterEndGame()
        {
            return 996;
        }

        public static ushort OpponentQuitWhileInGame()
        {
            return 997;
        }

        public static ushort BackToLobby()
        {
            return 969;
        }

        public static ushort Surrender()
        {
            return 407;
        }
        public static ushort UserLogout()
        {
            return 998;
        }

        public static ushort UserDisconnected()
        {
            return 999;
        }

    }
}
