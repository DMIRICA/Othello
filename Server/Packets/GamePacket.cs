using Server.Game;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Packets
{
    public class GamePacket : PacketStructure
    {

        public string Message;
        public ushort RoomID;

        public GamePacket(byte[] packet) : base(packet)
        {
            Message = ReadString(6, packet.Length - 6);
            RoomID = ReadUShort(4);
            if(ReadUShort(0) == GameProtocol.TurnMovePacket())
            {
                Room CurrentRoom = Singleton.Singleton.Instance.GetRoomByID(RoomID);

                string[] Split = Message.Split('|');
                string[] Move = Split[0].Split(':');
                CurrentRoom.Gameboard.
                    TakeMove(Int32.Parse(Move[0]), Int32.Parse(Move[1]), Split[1][0]);
                CurrentRoom.Gameboard.AddChip(Int32.Parse(Move[0]), Int32.Parse(Move[1]), Split[1][0]);

                //IF NEXT PLAYER HAVE LEGAL MOVES DO
                if (CurrentRoom.Gameboard.HasALegalMove(
                    CurrentRoom.GetNotPlayerTurn().DiskColor))
                {
                    CurrentRoom.ChangePlayerTurn();
                    CurrentRoom.SendAfterTurn();
                }
                //ELSE
                else if(CurrentRoom.Gameboard.HasALegalMove(
                    CurrentRoom.GetPlayerTurn().DiskColor))
                {
                    CurrentRoom.SendAfterTurn();
                }
                else
                {
                    CurrentRoom.SendGameBoard(CurrentRoom.Player1);
                    CurrentRoom.SendGameBoard(CurrentRoom.Player2);

                    char winner = CurrentRoom.Gameboard.GetWinnerColor();
                    if(winner == CurrentRoom.Player1.DiskColor)
                    {
                        
                        int WinnerChips = CurrentRoom.Gameboard.GetNumberOfDisk
                            (CurrentRoom.Player1.DiskColor);
                        int LoserChips = CurrentRoom.Gameboard.GetNumberOfDisk
                            (CurrentRoom.Player2.DiskColor);
                        CurrentRoom.SendGameOver(CurrentRoom.Player1,
                            "You won!\n Score: " + WinnerChips + " - " + LoserChips);
                        CurrentRoom.SendGameOver(CurrentRoom.Player2,
                            "You lost!\n Scor: " + LoserChips + " - " + WinnerChips);
                    }
                    else
                    {
                        int WinnerChips = CurrentRoom.Gameboard.GetNumberOfDisk
                            (CurrentRoom.Player2.DiskColor);
                        int LoserChips = CurrentRoom.Gameboard.GetNumberOfDisk
                            (CurrentRoom.Player1.DiskColor);
                        CurrentRoom.SendGameOver(CurrentRoom.Player2,
                            "You won!\n Score: " + WinnerChips + " - " + LoserChips);
                        CurrentRoom.SendGameOver(CurrentRoom.Player1,
                            "You lost!\n Scor: " + LoserChips + " - " + WinnerChips);
                    }
                }
            }
        }
        public GamePacket(ushort PacketID,ushort RoomID,string Message) : 
            base(PacketID, (ushort)(6+Message.Length))
        {
            this.RoomID = RoomID;
            WriteUShort(RoomID, 4);
            this.Message = Message;
            WriteString(Message, 6);
        }

    }
}
