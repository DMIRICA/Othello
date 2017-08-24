using Server.Entities;
using Server.Game;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Packets
{
    class MessagePacket : PacketStructure
    {
        private string Message;
        
        private int Length;

        public MessagePacket(byte[] packet) : base(packet)
        {
            Length = ReadUShort(2);
            Message = ReadString(4, packet.Length - 4);
        }

        public MessagePacket(ushort PacketID, string Message) :
            base(PacketID, (ushort)(4 + Message.Length))
        {
            this.Message = Message;
            this.Length = 4 + Message.Length;
            WriteString(Message, 4);
        }

        

        #region Chat
        public void doGlobalChat()
        {
            string[] splits = Message.Split(':');
            foreach (User user in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (splits[0] == user.Username)
                    continue;
                Othello.Server.SendPacket(user.Socket, this.getData());
            }
        }

        
        #endregion

        #region Auth
        public void doLogin(Socket clientSocket)
        {
            string[] fields = Message.Split('|');
            string username = fields[0];
            string password = fields[1];
            User user = Singleton.Singleton.Instance.DatabaseConnection.isPasswordRight(username, password);

            if (user != null)
            {
                if (Singleton.Singleton.Instance.isUserLogged(username))
                {
                    BasicPacket bp = new BasicPacket(GameProtocol.AlreadyOnlinePacketID());
                    Othello.Server.SendPacket(clientSocket, bp.getData());
                }
                else
                {
                    user.InGame = false;
                    user.Socket = clientSocket;
                    Singleton.Singleton.Instance.ListOfUsersLogged.Add(user);
                    //Send to current user logged the list with all the users logged and the status of them
                    string PacketMessage = "";
                    foreach (User u in Singleton.Singleton.Instance.ListOfUsersLogged)
                    {
                        if (u.Username == username)
                            continue;
                        PacketMessage += u.Username + ":" + u.IsChallenged + ":" + u.InGame + "|";
                        MessagePacket messagePacket = new MessagePacket(GameProtocol.AlertUsersNewUserLoggedID(), username + ":False");
                        Othello.Server.SendPacket(u.Socket, messagePacket.getData());
                    }
                    MessagePacket packet = new MessagePacket(GameProtocol.UsersLoggedListPacketID(), PacketMessage);
                    Othello.Server.SendPacket(user.Socket, packet.getData());
                }
            }
            else
            {
                BasicPacket bp = new BasicPacket(GameProtocol.FailedLoginPacketID());
                Othello.Server.SendPacket(clientSocket, bp.getData());
            }
        }

        public void registerAccount(Socket clientSocket)
        {
            string[] fields = Message.Split('|');
            string username = fields[0];
            string password = fields[1];
            string email = fields[2];
            bool usernameUsed = Singleton.Singleton.Instance.DatabaseConnection.isUsernameUsed(username);
            if (usernameUsed)
            {
                responseBack(clientSocket, GameProtocol.UsernameAlreadyUsedPacketID());
            }
            else
            {
                bool emailUsed = Singleton.Singleton.Instance.DatabaseConnection.isEmailUsed(email);
                if (emailUsed)
                {
                    responseBack(clientSocket, GameProtocol.EmailAlreadyUsedPacketID());
                }
                else
                {
                    bool done = Singleton.Singleton.Instance.DatabaseConnection.addUser(new User(0, username, password, email));

                    if (done)
                    {
                        responseBack(clientSocket, GameProtocol.SuccesCreateAccountPacketID());
                    }
                    else
                    {
                        responseBack(clientSocket, GameProtocol.FailedCreateAccountPacketID());
                    }

                }

            }
        }

        public void responseBack(Socket socket, ushort packetID)
        {
            Othello.Server.SendPacket(socket, new BasicPacket(packetID).getData());
        }
        #endregion

        #region Challenge
        public void sendAfterChallengeAction()
        {
            string[] splits = Message.Split('|');

            User user = null;
            //Search user who got challenged
            foreach (User u in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (u.Username == splits[0])
                {
                    user = u;
                    break;
                }
            }


            if (user != null && user.IsChallenged != true && user.InGame != true)
            {
                sendTheChallenge(user, splits[1]);
                notifyUsersAfterChallenge(user);
                user.IsChallenged = true;
            }

        }

        public void sendTheChallenge(User challengedUser, string message)
        {
            MessagePacket packet = new MessagePacket(GameProtocol.ChallengePacketID(), message);
            Othello.Server.SendPacket(challengedUser.Socket, packet.getData());
        }

        public void notifyUsersAfterChallenge(User challengedUser)
        {
            MessagePacket packet = new MessagePacket(GameProtocol.ChangeUserToChallenged(), challengedUser.Username);
            foreach (User user in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (user == challengedUser)
                    continue;
                Othello.Server.SendPacket(user.Socket, packet.getData());
            }
        }

        public void notifyUsersWithInGameStatus(User User)
        {
            MessagePacket packet = new MessagePacket(GameProtocol.ChangeUserToInGame(), User.Username);
            foreach (User user in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (user == User)
                    continue;
                Othello.Server.SendPacket(user.Socket, packet.getData());
            }
        }

        public void notifyUsersAfterChallengeIgnore()
        {
            string[] splits = Message.Split(':');
            MessagePacket mp = new MessagePacket(GameProtocol.ChangeUserToOnline(), splits[0]);
            foreach (User u in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (u.Username == splits[0])
                    u.IsChallenged = false;
                else if (u.Username == splits[1])
                {
                    Othello.Server.SendPacket(u.Socket, new MessagePacket(GameProtocol.ChallengeTimeoutPacketID(), splits[0]).getData());
                }
                else
                {
                    Othello.Server.SendPacket(u.Socket, mp.getData());
                }
            }
        }

        public void notifyUsersAfterChallengeRefuse()
        {
            string[] splits = Message.Split(':');
            MessagePacket mp = new MessagePacket(GameProtocol.ChangeUserToOnline(), splits[0]);
            foreach (User u in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (u.Username == splits[0])
                    u.IsChallenged = false;
                else if (u.Username == splits[1])
                {
                    Othello.Server.SendPacket(u.Socket, new MessagePacket(GameProtocol.UserRefusedChallengePacketID(), splits[0]).getData());
                }
                else
                {
                    Othello.Server.SendPacket(u.Socket, mp.getData());
                }
            }
        }

        public void playerAcceptedTheChallenge()
        {
            string[] usernames = Message.Split(':');
            if (usernames.Length == 2)
            {
                User firstUser = null;
                User secondUser = null;
                foreach (User user in Singleton.Singleton.Instance.ListOfUsersLogged)
                {
                    if (user.Username == usernames[0])
                    {
                        firstUser = user;
                    }
                    else if (user.Username == usernames[1])
                    {
                        secondUser = user;
                    }
                }

                if (firstUser != null && secondUser != null)
                {
                    firstUser.IsChallenged = false;
                    firstUser.InGame = true;
                    secondUser.IsChallenged = false;
                    secondUser.InGame = true;

                    notifyUsersWithInGameStatus(firstUser);
                    notifyUsersWithInGameStatus(secondUser);

                    Room room = new Room(firstUser, secondUser, Singleton.Singleton.Instance.RoomIDHelper);
                    Singleton.Singleton.Instance.RoomIDHelper += 1;
                    MessageRoomPacket packet = new MessageRoomPacket(GameProtocol.LoadGameScene(), room.ID, firstUser.Username + ":" + secondUser.Username);
                    //MessagePacket packet = new MessagePacket(GameProtocol.LoadGameScene(), room.ID, )
                    //   + "|" + firstUser.Username + ":" + secondUser.Username);
                    Othello.Server.SendPacket(firstUser.Socket, packet.getData());
                    Othello.Server.SendPacket(secondUser.Socket, packet.getData());
                    Singleton.Singleton.Instance.ListOfRooms.Add(room);
                }
            }



        }
        #endregion

        
    }
}
