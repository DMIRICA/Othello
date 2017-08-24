using Server.Entities;
using Server.Game;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Packets
{
    public class MessageRoomPacket : PacketStructure
    {

        public string Message;
        public ushort RoomID;
        public Room CurrentRoom = null;

        public MessageRoomPacket(byte[] packet) : base(packet)
        {
            Message = ReadString(6, packet.Length - 6);
            RoomID = ReadUShort(4);
            CurrentRoom = Singleton.Singleton.Instance.GetRoomByID(RoomID);
        }
        public MessageRoomPacket(ushort PacketID, ushort RoomID, string Message) :
            base(PacketID, (ushort)(6 + Message.Length))
        {
            this.RoomID = RoomID;
            WriteUShort(RoomID, 4);
            this.Message = Message;
            WriteString(Message, 6);
        }

        public void doChangesAfterTurn()
        {
            string[] Split = Message.Split('|');
            string[] Move = Split[0].Split(':');
            CurrentRoom.Gameboard.TakeMove(Int32.Parse(Move[0]), Int32.Parse(Move[1]), Split[1][0]);
            CurrentRoom.Gameboard.AddChip(Int32.Parse(Move[0]), Int32.Parse(Move[1]), Split[1][0]);
            //Check if next player have legal moves
            if (havePlayerLegalMoves(CurrentRoom, CurrentRoom.GetNotPlayerTurn().Player)) {
                CurrentRoom.ChangePlayersTurn();
                CurrentRoom.SendGameStatsAfterMove();
            }
            //Check if other player have legal moves
            else if (havePlayerLegalMoves(CurrentRoom, CurrentRoom.GetPlayerTurn().Player))
            {
                CurrentRoom.SendGameStatsAfterMove();
            }
            // if no legal moves left game is over
            else
            {
                CurrentRoom.SendGameBoard(CurrentRoom.FirstUser);
                CurrentRoom.SendGameBoard(CurrentRoom.SecondUser);

                User winner = getWinner();
                User looser = getLooser();

                int WinnerChips = CurrentRoom.Gameboard.GetNumberOfDisk(winner.Player.DiskColor);
                int LoserChips = CurrentRoom.Gameboard.GetNumberOfDisk(looser.Player.DiskColor);
                CurrentRoom.SendGameOver(winner, "You won!\n Score: " + WinnerChips + " - " + LoserChips);
                CurrentRoom.SendGameOver(looser, "You lost!\n Score: " + LoserChips + " - " + WinnerChips);

            }
        }

        public bool havePlayerLegalMoves(Room room, Player player)
        {
            return room.Gameboard.HasALegalMove(player.DiskColor);
        }

        public User getWinner()
        {
            char winner = CurrentRoom.Gameboard.GetWinnerColor();
            if (winner == CurrentRoom.FirstUser.Player.DiskColor)
                return CurrentRoom.FirstUser;
            else
                return CurrentRoom.SecondUser;
        }

        public User getLooser()
        {
            User winner = getWinner();
            if (winner == CurrentRoom.FirstUser)
                return CurrentRoom.SecondUser;
            else
                return CurrentRoom.FirstUser;
        }

        public void backToLobby()
        {
            if (CurrentRoom.FirstUser != null && CurrentRoom.FirstUser.Username == Message)
            {
                User currentUser = Singleton.Singleton.Instance.GetUserLoogedByUsername(Message);
                String PacketMessage = "";
                foreach (User u in Singleton.Singleton.Instance.ListOfUsersLogged)
                {
                    if (u.Username == Message)
                        continue;
                    if(CurrentRoom.SecondUser != null && u.Username == CurrentRoom.SecondUser.Username)
                    {
                        MessagePacket mp = new MessagePacket(GameProtocol.BackToLobby(), Message);
                        Othello.Server.SendPacket(u.Socket, mp.getData());
                        
                    }
                    PacketMessage += u.Username + ":" + u.IsChallenged + ":" + u.InGame + "|";
                    MessagePacket messagePacket = new MessagePacket(GameProtocol.ChangeUserToOnline(), Message);
                    Othello.Server.SendPacket(u.Socket, messagePacket.getData());
                }
                MessagePacket packet = new MessagePacket(GameProtocol.LoadMainSceneFromGame(), PacketMessage);
                Othello.Server.SendPacket(currentUser.Socket, packet.getData());
                currentUser.InGame = false;
                CurrentRoom.FirstUser = null;
                if(CurrentRoom.SecondUser == null)
                {
                    Singleton.Singleton.Instance.ListOfRooms.Remove(CurrentRoom);
                }

            }
            else
            {
                User currentUser = Singleton.Singleton.Instance.GetUserLoogedByUsername(Message);
                String PacketMessage = "";
                foreach (User u in Singleton.Singleton.Instance.ListOfUsersLogged)
                {
                    if (u.Username == Message)
                        continue;
                    if (CurrentRoom.FirstUser != null && u.Username == CurrentRoom.FirstUser.Username)
                    {
                        MessagePacket mp = new MessagePacket(GameProtocol.BackToLobby(), Message);
                        Othello.Server.SendPacket(u.Socket, mp.getData());
                    }
                    PacketMessage += u.Username + ":" + u.IsChallenged + ":" + u.InGame + "|";
                    MessagePacket messagePacket = new MessagePacket(GameProtocol.ChangeUserToOnline(), Message);
                    Othello.Server.SendPacket(u.Socket, messagePacket.getData());
                }
                MessagePacket packet = new MessagePacket(GameProtocol.LoadMainSceneFromGame(), PacketMessage);
                Othello.Server.SendPacket(currentUser.Socket, packet.getData());
                currentUser.InGame = false;
                CurrentRoom.SecondUser = null;
                if (CurrentRoom.FirstUser == null)
                {
                    Singleton.Singleton.Instance.ListOfRooms.Remove(CurrentRoom);
                }
            }
        }

        public void doRoomChat(Socket clientSocket, byte[] packet)
        {
            if (clientSocket == CurrentRoom.FirstUser.Socket)
            {
                Othello.Server.SendPacket(CurrentRoom.SecondUser.Socket, packet);
            }
            else if (clientSocket == CurrentRoom.SecondUser.Socket)
            {
                Othello.Server.SendPacket(CurrentRoom.FirstUser.Socket, packet);
            }
            CurrentRoom.ChatHistory.Add(Message);
        }

        public void playerReady()
        {

            if (CurrentRoom != null)
            {
                if (CurrentRoom.FirstUser != null && CurrentRoom.FirstUser.Username == Message)
                {
                    CurrentRoom.FirstUser.Player.ReadyToStart = true;
                    if (!CurrentRoom.SecondUser.Player.ReadyToStart)
                    {
                        BasicPacket p = new BasicPacket(GameProtocol.PlayerReady());
                        Othello.Server.SendPacket(CurrentRoom.SecondUser.Socket, p.getData());
                    }
                }
                else if (CurrentRoom.SecondUser != null && CurrentRoom.SecondUser.Username == Message)
                {
                    CurrentRoom.SecondUser.Player.ReadyToStart = true;
                    if (!CurrentRoom.FirstUser.Player.ReadyToStart)
                    {
                        BasicPacket p = new BasicPacket(GameProtocol.PlayerReady());
                        Othello.Server.SendPacket(CurrentRoom.FirstUser.Socket, p.getData());
                    }

                }

                if (CurrentRoom.areBothPlayersReady())
                {
                    CurrentRoom.startGame();
                    CurrentRoom.FirstUser.Player.ReadyToStart = false;
                    CurrentRoom.SecondUser.Player.ReadyToStart = false;
                }
            }
        }



        public void playAgain()
        {

            if (CurrentRoom != null)
            {
                if (CurrentRoom.FirstUser.Username == Message)
                {
                    CurrentRoom.FirstUser.Player.PlayAgain = true;
                    if (!CurrentRoom.SecondUser.Player.PlayAgain)
                    {
                        BasicPacket bp = new BasicPacket(GameProtocol.PlayAgain());
                        Othello.Server.SendPacket(CurrentRoom.SecondUser.Socket, bp.getData());
                    }
                }
                else if (CurrentRoom.SecondUser.Username == Message)
                {
                    CurrentRoom.SecondUser.Player.PlayAgain = true;
                    if (!CurrentRoom.FirstUser.Player.PlayAgain)
                    {
                        BasicPacket bp = new BasicPacket(GameProtocol.PlayAgain());
                        Othello.Server.SendPacket(CurrentRoom.FirstUser.Socket, bp.getData());
                    }
                }

                if (CurrentRoom.isAblePlayAgain())
                {
                    CurrentRoom.PlayAgain();
                    CurrentRoom.resetPlayAgain();
                }

            }
        }
        

        public void surrender()
        {

            if (CurrentRoom.FirstUser.Username == Message)
            {
                CurrentRoom.SendGameOver(CurrentRoom.SecondUser,
                    "You won!\n Your opponent surrenders.");
                CurrentRoom.SendGameOver(CurrentRoom.FirstUser,
                    "You lost!\n You Surrendered");
            }
            else if (CurrentRoom.SecondUser.Username == Message)
            {
                CurrentRoom.SendGameOver(CurrentRoom.FirstUser,
                    "You won!\n Your opponent surrenders.");
                CurrentRoom.SendGameOver(CurrentRoom.SecondUser,
                    "You lost!\n You Surrendered");
            }
        }

        public void opponentQuitWhileInGame()
        {

            if (CurrentRoom.FirstUser != null && CurrentRoom.FirstUser.Username == Message)
            {
                Singleton.Singleton.Instance.ListOfUsersLogged.Remove(CurrentRoom.FirstUser);
                CurrentRoom.SendGameOver(CurrentRoom.SecondUser,
                    "You won!\n Your opponent got disconnected.");
                foreach (User user in Singleton.Singleton.Instance.ListOfUsersLogged)
                {

                    if(user.Username == CurrentRoom.SecondUser.Username)
                    {
                        MessagePacket pack = new MessagePacket(GameProtocol.OpponentQuitWhileInGame(), CurrentRoom.FirstUser.Username);
                        Othello.Server.SendPacket(user.Socket, pack.getData());
                        continue;
                    }
                    MessagePacket p = new MessagePacket(GameProtocol.UserDisconnected(), CurrentRoom.FirstUser.Username);
                    Othello.Server.SendPacket(user.Socket,p.getData());
                }
                CurrentRoom.FirstUser = null;             

            }
            else if (CurrentRoom.SecondUser != null && CurrentRoom.SecondUser.Username == Message)
            {
                CurrentRoom.SendGameOver(CurrentRoom.FirstUser,
                    "You won!\n Your opponent got disconnected.");
                Singleton.Singleton.Instance.ListOfUsersLogged.Remove(CurrentRoom.SecondUser);
                foreach (User user in Singleton.Singleton.Instance.ListOfUsersLogged)
                {
                    if (user.Username == CurrentRoom.FirstUser.Username)
                    {
                        MessagePacket pack = new MessagePacket(GameProtocol.OpponentQuitWhileInGame(), CurrentRoom.SecondUser.Username);
                        Othello.Server.SendPacket(user.Socket, pack.getData());
                        continue;
                    }
                    MessagePacket p = new MessagePacket(GameProtocol.UserDisconnected(), CurrentRoom.SecondUser.Username);
                    Othello.Server.SendPacket(user.Socket, p.getData());
                }
                CurrentRoom.SecondUser = null;
            }

            if (CurrentRoom.FirstUser == null && CurrentRoom.SecondUser == null)
            {
                Singleton.Singleton.Instance.ListOfRooms.Remove(CurrentRoom);
            }

        }

        public void opponentQuitAfterEndGame()
        {
            if (CurrentRoom.FirstUser != null && CurrentRoom.FirstUser.Username == Message)
            {
                Console.WriteLine("Client " + Message + " Disconnected!");
                CurrentRoom.FirstUser.Socket.Shutdown(SocketShutdown.Both);
                CurrentRoom.FirstUser.Socket.Close();
                Singleton.Singleton.Instance.ListOfUsersLogged.Remove(CurrentRoom.FirstUser);
                
                if (CurrentRoom.FirstUser == null && CurrentRoom.SecondUser == null)
                {
                    Singleton.Singleton.Instance.ListOfRooms.Remove(CurrentRoom);
                }
                else
                {
                    MessagePacket mp = new MessagePacket(GameProtocol.OpponentQuitAfterEndGame(), Message);
                    Othello.Server.SendPacket(CurrentRoom.SecondUser.Socket, mp.getData());
                    foreach (User user in Singleton.Singleton.Instance.ListOfUsersLogged)
                    {
                        if (user.Username == CurrentRoom.SecondUser.Username)
                            continue;
                        MessagePacket p = new MessagePacket(GameProtocol.UserDisconnected(), CurrentRoom.FirstUser.Username);
                        Othello.Server.SendPacket(user.Socket, p.getData());
                    }
                    CurrentRoom.FirstUser = null;
                }
               
            }
            else if (CurrentRoom.SecondUser != null && CurrentRoom.SecondUser.Username == Message)
            {
                Console.WriteLine("Client " + Message + " Disconnected!");
                CurrentRoom.SecondUser.Socket.Shutdown(SocketShutdown.Both);
                CurrentRoom.SecondUser.Socket.Close();
                Singleton.Singleton.Instance.ListOfUsersLogged.Remove(CurrentRoom.SecondUser);
                
                if (CurrentRoom.FirstUser == null && CurrentRoom.SecondUser == null)
                {
                    Singleton.Singleton.Instance.ListOfRooms.Remove(CurrentRoom);
                }
                else
                {
                    MessagePacket mp = new MessagePacket(GameProtocol.OpponentQuitAfterEndGame(), Message);
                    Othello.Server.SendPacket(CurrentRoom.FirstUser.Socket, mp.getData());
                    foreach (User user in Singleton.Singleton.Instance.ListOfUsersLogged)
                    {
                        if (user.Username == CurrentRoom.FirstUser.Username)
                            continue;
                        MessagePacket p = new MessagePacket(GameProtocol.UserDisconnected(), CurrentRoom.SecondUser.Username);
                        Othello.Server.SendPacket(user.Socket, p.getData());
                    }
                    CurrentRoom.SecondUser = null;
                }
            }

            if(CurrentRoom.FirstUser == null && CurrentRoom.SecondUser == null)
            {
                Singleton.Singleton.Instance.ListOfRooms.Remove(CurrentRoom);
            }
        }
    }
}

