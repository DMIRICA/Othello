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
            
        }
        public GamePacket(ushort PacketID,ushort RoomID,string Message) : 
            base(PacketID, (ushort)(6+Message.Length))
        {
            this.RoomID = RoomID;
            WriteUShort(RoomID, 4);
            this.Message = Message;
            WriteString(Message, 6);
        }

        public void doChangesAfterTurn()
        {
            Room CurrentRoom = Singleton.Singleton.Instance.GetRoomByID(RoomID);
            string[] Split = Message.Split('|');
            string[] Move = Split[0].Split(':');
            CurrentRoom.Gameboard.TakeMove(Int32.Parse(Move[0]), Int32.Parse(Move[1]), Split[1][0]);
            CurrentRoom.Gameboard.AddChip(Int32.Parse(Move[0]), Int32.Parse(Move[1]), Split[1][0]);
            //Check if next player have legal moves
            if (havePlayerLegalMoves(CurrentRoom, CurrentRoom.GetNotPlayerTurn())){
                CurrentRoom.ChangePlayersTurn();
                CurrentRoom.SendGameStatsAfterMove();
            }
            //Check if other player have legal moves
            else if(havePlayerLegalMoves(CurrentRoom, CurrentRoom.GetPlayerTurn()))
            {
                CurrentRoom.SendGameStatsAfterMove();
            }
            // if no legal moves left game is over
            else
            {
                CurrentRoom.SendGameBoard(CurrentRoom.Player1);
                CurrentRoom.SendGameBoard(CurrentRoom.Player2);

                Player winner = getWinner(CurrentRoom);
                Player looser = getLooser(CurrentRoom);

                int WinnerChips = CurrentRoom.Gameboard.GetNumberOfDisk(winner.DiskColor);
                int LoserChips = CurrentRoom.Gameboard.GetNumberOfDisk(looser.DiskColor);
                CurrentRoom.SendGameOver(winner,"You won!\n Score: " + WinnerChips + " - " + LoserChips);
                CurrentRoom.SendGameOver(looser, "You lost!\n Scor: " + LoserChips + " - " + WinnerChips);

            }
        }

        public bool havePlayerLegalMoves(Room room,Player player)
        {
            return room.Gameboard.HasALegalMove(player.DiskColor);
        }

        public Player getWinner(Room room)
        {
            char winner = room.Gameboard.GetWinnerColor();
            if (winner == room.Player1.DiskColor)
                return room.Player1;
            else
                return room.Player2;
        }

        public Player getLooser(Room room)
        {
            Player player = getWinner(room);
            if (player == room.Player1)
                return room.Player2;
            else
                return room.Player1;
        }

        

    }
}
