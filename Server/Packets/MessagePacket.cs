using Server.Entities;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Packets
{
    class MessagePacket : PacketStructure
    {
        private string Message;
        private int PacketID;
        private int Length;

        public MessagePacket(byte[] packet) : base(packet)
        {
            PacketID = ReadUShort(0);
            Length = ReadUShort(2);
            Message = ReadString(4, packet.Length - 4);
        }

        public MessagePacket(ushort PacketID, string Message) :
            base(PacketID, (ushort)(4 + Message.Length))
        {
            this.Message = Message;
            this.PacketID = PacketID;
            this.Length = 4 + Message.Length;
            WriteString(Message, 4);
        }

        public string MessageP { get => Message; set => Message = value; }


        public void doChat()
        {
            string[] splits = Message.Split(':');
            foreach (User user in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (splits[0] == user.Username)
                    continue;
                Othello.Server.SendPacket(user.Socket, this.getData());
            }
        }

        #region Challenge
        public void sendAfterChallengeAction()
        {
            string[] splits = Message.Split('|');

            User user = null;
            foreach(User u in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if(u.Username == splits[0])
                {
                    user = u;
                    break;
                }
            }
            
            
            if(user != null && user.IsChallenged != true)
            {
                sendTheChallenge(user,splits[1]);
                notifyUsersAfterChallenge(user);
                user.IsChallenged = true;
            }
            
        }

        public void sendTheChallenge(User challengedUser,string message)
        {
            MessagePacket packet = new MessagePacket(GameProtocol.ChallengePacketID(), message);
            Othello.Server.SendPacket(challengedUser.Socket, packet.getData());
        }

        public void notifyUsersAfterChallenge(User challengedUser)
        {
            MessagePacket packet = new MessagePacket(GameProtocol.ChangeUserToChallenged(), challengedUser.Username);
            foreach(User user in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (user == challengedUser)
                    continue;
                Othello.Server.SendPacket(user.Socket, packet.getData());
            }
        }

        public void notifyUsersAfterChallengeIgnore()
        {
            string[] splits = Message.Split(':');
            MessagePacket mp = new MessagePacket(GameProtocol.ChangeUserToOnline(), splits[0]);
            foreach(User u in Singleton.Singleton.Instance.ListOfUsersLogged)
            {
                if (u.Username == splits[0])
                    u.IsChallenged = false;
                else if ( u.Username == splits[1])
                {
                    Othello.Server.SendPacket(u.Socket, new MessagePacket(GameProtocol.ChallengeTimeoutPacketID(), splits[0]).getData());
                }
                else
                {
                    Othello.Server.SendPacket(u.Socket,mp.getData());
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
        #endregion
    }
}
