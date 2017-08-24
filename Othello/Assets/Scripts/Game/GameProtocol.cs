using UnityEngine;
using System.Collections;


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

    #region Chat

    public static ushort GlobalChatMessagePacketID()
    {
        return 200;
    } 
    public static ushort RoomChatMessagePacketID()
    {
        return 201;
    }
    #endregion

    public static ushort ChallengePacketID()
    {
        return 250;
    }

    public static ushort ChallengeAcceptedPacketID()
    {
        return 257;
    }
    public static ushort ChallengeRefusedPacketID()
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


    public static ushort BoardTableGamePacketID()
    {
        return 400;
    }

    public static ushort TurnAndColor()
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

    public static ushort LoadGameScene()
    {
        return 500;
    }

    public static ushort PlayerReady()
    {
        return 501;
    }

    public static ushort BackToLobby()
    {
        return 995;
    }

    public static ushort QuitAfterGameOver()
    {
        return 996;
    }
    public static ushort QuitWhileInGame()
    {
        return 997;
    }

    public static ushort Logout()
    {
        return 998;
    }

    public static ushort UserDisconnected()
    {
        return 999;
    }
}


