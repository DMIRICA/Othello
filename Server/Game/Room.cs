using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using Server.Packets;
using Server.Protocol;
using System.Threading;
using Server.Entities;

namespace Server.Game
{
    public class Room
    {
        private string _Name;
        private ushort _ID;
        private User _FirstUser;
        private User _SecondUser;
        private GameBoard _Gameboard;
        private List<string> _ChatHistory;

        public Room(User x, User y,ushort ID)
        {
            _FirstUser = x;
            _SecondUser = y;
            _ID = ID;
            _ChatHistory = new List<string>();
            Gameboard = new GameBoard();
        }

        public bool areBothPlayersReady()
        {
            return FirstUser.Player.ReadyToStart && SecondUser.Player.ReadyToStart;
        }

        public void startGame()
        {
            PickFirsTurnPlayer();
            SendStartGamePackets();
        }

        private void SendColorAndHisTurn(User x)
        {
            string s = x.Player.IsHisTurn.ToString() + '|' + x.Player.DiskColor;
            MessageRoomPacket Message = new MessageRoomPacket(GameProtocol.StartGamePacketID(),ID, s);
            Othello.Server.SendPacket(x.Socket, Message.getData());
        }
        private void SendLegalMoves(User x)
        {
            if (x.Player.IsHisTurn)
            {
                List<BoardPosition> tempList = Gameboard.GetAllLegalMoves(x.Player.DiskColor);
                string tempString = "";
                int count = 1;
                foreach (BoardPosition aux in tempList)
                {
                    if (count == tempList.Count)
                    {
                        tempString += aux.Row.ToString() + ':' + aux.Column + '!' + Gameboard.GetNumberOfChanges(aux.Row,aux.Column,x.Player.DiskColor);
                        
                        continue;
                    }
                    tempString += aux.Row.ToString() + ':' + aux.Column + '!' + Gameboard.GetNumberOfChanges(aux.Row, aux.Column, x.Player.DiskColor) +'|';
                    count++;
                }
                MessageRoomPacket movesPacket = new MessageRoomPacket(GameProtocol.BoardMoves(),ID, tempString);
                //DelayPacket(100);
                Othello.Server.SendPacket(x.Socket, movesPacket.getData());
            }
        }
        public void SendGameBoard(User x)
        {
            BoardPacket Packet = new BoardPacket(GameProtocol.BoardTableGamePacketID(), this);
            Othello.Server.SendPacket(x.Socket, Packet.getData());
        }
        public void SendPlayerFirstStuff(User x)
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
        public void SendGameStatsAfterMove()
        {
            SendPlayerTurn(FirstUser);
            DelayPacket(20);
            SendPlayerTurn(SecondUser);
            DelayPacket(20);
            SendGameBoard(FirstUser);
            DelayPacket(70);
            SendGameBoard(SecondUser);
            DelayPacket(70);
            if (FirstUser.Player.IsHisTurn)
            {
                SendLegalMoves(FirstUser);
            }
            else
            {
                SendLegalMoves(SecondUser);
            }

        }
        public User GetPlayerTurn()
        {
            if (FirstUser.Player.IsHisTurn) return FirstUser;
            else return SecondUser;
        }
        public User GetNotPlayerTurn()
        {
            if (FirstUser.Player.IsHisTurn) return SecondUser;
            else return FirstUser;
        }
        private void SendPlayerTurn(User x)
        {
            string s = x.Player.IsHisTurn.ToString();
            MessageRoomPacket Message = new MessageRoomPacket(GameProtocol.PlayerTurnPacket(),ID, s);
            Othello.Server.SendPacket(x.Socket, Message.getData());
        }
        public void ChangePlayersTurn()
        {
            if (FirstUser.Player.IsHisTurn)
            {
                FirstUser.Player.IsHisTurn = false;
                SecondUser.Player.IsHisTurn = true;
            }
            else
            {
                FirstUser.Player.IsHisTurn = true;
                SecondUser.Player.IsHisTurn = false;
            }
        }
        public void DelayPacket(int miliseconds)
        {
            Thread.Sleep(miliseconds);
        }
        public void SendAllowedMoves()
        {
            if(FirstUser.Player.IsHisTurn)
            {
                List<BoardPosition> tempList = Gameboard.GetAllLegalMoves(FirstUser.Player.DiskColor);
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
                MessageRoomPacket Packet = new MessageRoomPacket(GameProtocol.BoardMoves(),ID, tempString);
                Othello.Server.SendPacket(FirstUser.Socket, Packet.getData());

            }
            else if(SecondUser.Player.IsHisTurn)
            {
                List<BoardPosition> tempList = Gameboard.GetAllLegalMoves(SecondUser.Player.DiskColor);
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
                MessageRoomPacket Packet = new MessageRoomPacket(GameProtocol.BoardMoves(),ID, tempString);
                Othello.Server.SendPacket(SecondUser.Socket, Packet.getData());
            }
        }
        public void SendStartGamePackets()
        {
            SendPlayerFirstStuff(FirstUser);
            SendPlayerFirstStuff(SecondUser);
        }
        public User FirstUser
        {
            get { return _FirstUser;  }
            set { _FirstUser = value; }
        }
        public User SecondUser
        {
            get { return _SecondUser;  }
            set { _SecondUser = value; }
        }
        public string Name
        {
            get { return _Name;  }
            set { _Name = value; }
        }
        public GameBoard Gameboard
        {
            get { return _Gameboard;  }
            set { _Gameboard = value; }
        }
        public List<string> ChatHistory
        {
            get { return _ChatHistory;  }
            set { _ChatHistory = value; }
        }

        public ushort ID
        {
            get { return _ID;  }
            set { _ID = value; }
        }

        public int GenerateRandom()
        {
            return new Random().Next(2);
        }
        private void PickFirsTurnPlayer()
        {
            if (GenerateRandom() == 0)
            {
                FirstUser.Player.IsHisTurn = true;
                SecondUser.Player.IsHisTurn = false;
                FirstUser.Player.DiskColor = 'B';
                SecondUser.Player.DiskColor = 'R';
            }
            else
            {
                FirstUser.Player.IsHisTurn = false;
                SecondUser.Player.IsHisTurn = true;
                FirstUser.Player.DiskColor = 'R';
                SecondUser.Player.DiskColor = 'B';
            }
        }
        public void SendGameOver(User x, string message)
        {
            MessagePacket packet = new MessagePacket(GameProtocol.GameOverPacket(), message);
            Othello.Server.SendPacket(x.Socket, packet.getData());
        }
        public void PlayAgain()
        {
            Gameboard = new GameBoard();
            PickFirsTurnPlayer();
            SendStartGamePackets();
        }
        public void ResetPlayAgain()
        {
            FirstUser.Player.PlayAgain = false;
            SecondUser.Player.PlayAgain = false;
        }

        public bool isAblePlayAgain()
        {
            return FirstUser.Player.PlayAgain && SecondUser.Player.PlayAgain;
        }

        public void resetPlayAgain()
        {
            FirstUser.Player.PlayAgain = false;
            SecondUser.Player.PlayAgain = false;
        }

    }
}
