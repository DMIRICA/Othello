using Assets.Scripts.Entities;
using Assets.Scripts.MainScene;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Networking.Packets
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
            WriteString(Message, 4);
        }

        public IEnumerator loadMainScene()
        {
            if (Message != "")
            {

                string[] splits = Message.Split('|');
                foreach (string s in splits)
                {
                    if (s == "")
                        continue;
                    string[] users = s.Split(':');
                    User user = new User();
                    user.Username = users[0];
                    if (users[1] == "true" || users[1] == "True") 
                        user.InGame = true;
                    else
                        user.InGame = false;
                    Singleton.Singleton.Instance.UsersLoggedList.Add(user);
                    Singleton.Singleton.Instance.UsersLoggedList.Sort((x, y) => string.Compare(x.Username, y.Username));
                }
            }
            SceneManager.LoadScene("MainScene");
            yield return null;
        }

        public IEnumerator addNewUserLoggedToList()
        {
            if (Message != "")
            {
                string[] words = Message.Split(':');
                User user = new User();
                user.Username = words[0];
                user.InGame = (words[1] == "True" || words[1] == "true") ? true : false;
                Singleton.Singleton.Instance.UsersLoggedList.Add(user);
                MainSceneScript.addNewUsersToScrollViewContent(user);
            }
            yield return null;
        }

        public void RunInMainThread(IEnumerator function)
        {
            UnityMainThreadDispatcher.Enqueue(function);
        }

    }
}
