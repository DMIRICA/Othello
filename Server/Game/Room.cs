using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using Server.Packets;
using Server.Protocol;
using System.Threading;

namespace Server.Game
{
    public class Room
    {
        private string _Name;
        private ushort _ID;
        private Player _Player1;
        private Player _Player2;
        private GameBoard _Gameboard;
        private List<string> _ChatHistory;

        public Room(Player x, Player y,ushort ID)
        {
            Player1 = x;
            Player2 = y;
            this.ID = ID;
            _ChatHistory = new List<string>();
            Gameboard = new GameBoard();
            PickFirsTurnPlayer();
            SendStartGamePackets();
        }

        private void SendColorAndHisTurn(Player x)
        {
            string s = x.IsHisTurn.ToString() + '|' + x.DiskColor;
            GamePacket Message = new GamePacket(GameProtocol.StartGamePacketID(),ID, s);
            ReversiServer.Server.SendPacket(x.PlayerSocket, Message.getData());
        }
        private void SendLegalMoves(Player x)
        {
            if (x.IsHisTurn)
            {
                List<BoardPosition> tempList = Gameboard.GetAllLegalMoves(x.DiskColor);
                string tempString = "";
                int count = 1;
                foreach (BoardPosition aux in tempList)
                {
                    if (count == tempList.Count)
                    {
                        tempString += aux.Row.ToString() + ':' + aux.Column + '!' + Gameboard.GetNumberOfChanges(aux.Row,aux.Column,x.DiskColor);
                        
                        continue;
                    }
                    tempString += aux.Row.ToString() + ':' + aux.Column + '!' + Gameboard.GetNumberOfChanges(aux.Row, aux.Column, x.DiskColor) +'|';
                    count++;
                }
                GamePacket movesPacket = new GamePacket(GameProtocol.BoardMoves(),ID, tempString);
                //DelayPacket(100);
                ReversiServer.Server.SendPacket(x.PlayerSocket, movesPacket.getData());
            }
        }
        public void SendGameBoard(Player x)
        {
            BoardPacket Packet = new BoardPacket(GameProtocol.BoardTableGamePacketID(), this);
            ReversiServer.Server.SendPacket(x.PlayerSocket, Packet.getData());
        }
        public void SendPlayerFirstStuff(Player x)
        {
            //Send if is his turn and his disk color
            //DelayPacket(100);
            SendColorAndHisTurn(x);
            //Delay 100ms between packets
            DelayPacket(50);
            //IF IS HIS TURN SEND HIM HIS LEGAL MOVES
            SendLegalMoves(x);
            //Delay 10 ms between packets
            DelayPacket(100);
            // THEN SEND A PACKET WITH ROOM GAMEBOARD
            SendGameBoard(x);

            

            #region SEND PACKET
            //string PacketMessage = "";

            //#region Compose HIS TUR AND DISKCOLOR
            //PacketMessage += Convert.ToString(x.IsHisTurn) + '|' + x.DiskColor + '|';
            ////byte[] temp1 = Encoding.ASCII.GetBytes(Mesaj);
            //#endregion

            //#region Compose Gameboard
            //for(int i = 0; i < 8; i++)
            //{
            //    for(int j = 0; j < 8; j++)
            //    {
            //        if(i == 7 && j == 7)
            //        {
            //            PacketMessage += Gameboard.Gameboard[i, j].Color;
            //            PacketMessage += '|';
            //            continue;
            //        }
            //        PacketMessage += Gameboard.Gameboard[i, j].Color;
            //        PacketMessage +='-';
            //    }
            //}
            //#endregion

            //#region Compose LEGAL MOVES if histurn

            //if (x.IsHisTurn)
            //{                
            //    List<BoardPosition> tempList = Gameboard.GetAllLegalMoves(x.DiskColor);
            //    int count = 1;
            //    foreach (BoardPosition aux in tempList)
            //    {
            //        if (count == tempList.Count)
            //        {
            //            PacketMessage += aux.Row.ToString() + ':' + aux.Column;
            //            continue;
            //        }
            //        PacketMessage += aux.Row.ToString() + ':' + aux.Column + '!';
            //        count++;
            //    }
            //}
            //#endregion

            //GamePacket packet = new GamePacket(1000, PacketMessage);
            //ReversiServer.Server.SendPacket(x.PlayerSocket, packet.getData());

            #endregion

        }
        public void SendAfterTurn()
        {
            SendPlayerTurn(Player1);
            DelayPacket(20);
            SendPlayerTurn(Player2);
            DelayPacket(20);
            SendGameBoard(Player1);
            DelayPacket(70);
            SendGameBoard(Player2);
            DelayPacket(70);
            if (Player1.IsHisTurn)
            {
                SendLegalMoves(Player1);
            }
            else
            {
                SendLegalMoves(Player2);
            }

        }
        public Player GetPlayerTurn()
        {
            if (Player1.IsHisTurn) return Player1;
            else return Player2;
        }
        public Player GetNotPlayerTurn()
        {
            if (Player1.IsHisTurn) return Player2;
            else return Player1;
        }
        private void SendPlayerTurn(Player x)
        {
            string s = x.IsHisTurn.ToString();
            GamePacket Message = new GamePacket(GameProtocol.PlayerTurnPacket(),ID, s);
            ReversiServer.Server.SendPacket(x.PlayerSocket, Message.getData());
        }
        public void ChangePlayerTurn()
        {
            if (Player1.IsHisTurn)
            {
                Player1.IsHisTurn = false;
                Player2.IsHisTurn = true;
            }
            else
            {
                Player1.IsHisTurn = true;
                Player2.IsHisTurn = false;
            }
        }
        public void DelayPacket(int miliseconds)
        {
            Thread.Sleep(miliseconds);
        }
        public void SendAllowedMoves()
        {
            if(Player1.IsHisTurn)
            {
                List<BoardPosition> tempList = Gameboard.GetAllLegalMoves(Player1.DiskColor);
                string tempString = "";
                int count = 1;
                foreach(BoardPosition aux in tempList)
                {
                    if (count == tempList.Count)
                    {
                        tempString += aux.Row.ToString() + ':' + aux.Column;
                        
                        continue;
                    }
                    tempString += aux.Row.ToString() + ':' + aux.Column+'|';
                    count++;
                }
                GamePacket Packet = new GamePacket(GameProtocol.BoardMoves(),ID, tempString);
                ReversiServer.Server.SendPacket(Player1.PlayerSocket, Packet.getData());

            }
            else if(Player2.IsHisTurn)
            {
                List<BoardPosition> tempList = Gameboard.GetAllLegalMoves(Player2.DiskColor);
                string tempString = "";
                int count = 1;
                foreach (BoardPosition aux in tempList)
                {
                    if(count == tempList.Count)
                    {
                        tempString += aux.Row.ToString() + ':' + aux.Column;
                     
                        continue;
                    }
                    tempString += aux.Row.ToString() + ':' + aux.Column + '|';
                    count++;
                }
                GamePacket Packet = new GamePacket(GameProtocol.BoardMoves(),ID, tempString);
                ReversiServer.Server.SendPacket(Player2.PlayerSocket, Packet.getData());
            }
        }
        public void SendStartGamePackets()
        {
            SendPlayerFirstStuff(Player1);
            SendPlayerFirstStuff(Player2);
        }
        public Player Player1
        {
            get
            {
                return _Player1;
            }
            set
            {
                _Player1 = value;
            }
        }
        public Player Player2
        {
            get
            {
                return _Player2;
            }
            set
            {
                _Player2 = value;
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public GameBoard Gameboard
        {
            get
            {
                return _Gameboard;
            }
            set
            {
                _Gameboard = value;
            }
        }
        public List<string> ChatHistory
        {
            get
            {
                return _ChatHistory;
            }

            set
            {
                _ChatHistory = value;
            }
        }

        public ushort ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        public int GenerateRandom()
        {
            return new Random().Next(2);
        }
        private void PickFirsTurnPlayer()
        {
            if (GenerateRandom() == 0)
            {
                Player1.IsHisTurn = true;
                Player2.IsHisTurn = false;
                Player1.DiskColor = 'B';
                Player2.DiskColor = 'R';
            }
            else
            {
                Player1.IsHisTurn = false;
                Player2.IsHisTurn = true;
                Player1.DiskColor = 'R';
                Player2.DiskColor = 'B';
            }
        }
        public void SendGameOver(Player x, string message)
        {
            GamePacket Message = new GamePacket(GameProtocol.GameOverPacket(),ID, message);
            ReversiServer.Server.SendPacket(x.PlayerSocket, Message.getData());
        }
        public void PlayAgain()
        {
            Gameboard = new GameBoard();
            PickFirsTurnPlayer();
            SendStartGamePackets();
        }
        public void ResetPlayAgain()
        {
            Player1.PlayAgain = false;
            Player2.PlayAgain = false;
        }

    }
}
