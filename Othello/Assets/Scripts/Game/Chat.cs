using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Networking.GamePackets;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Networking.Packets.GamePackets;

namespace Assets.Scripts.Game
{
    public class Chat : MonoBehaviour
    {
        #region Variables
        #region Unity Elements
        public InputField InputField;
        public static Text ChatBox;
        #endregion

        private string _Message;
        #endregion

        #region Methods
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        public void setMessage(string s)
        {
            if(!String.IsNullOrEmpty(s))
            Singleton.Singleton.Instance.MessageTyped = s;
        }

        public void AppendText(string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                ChatBox.text += s;
            }
        }

        private void SendChatMessage()
        {
            //ChatMessage ChatMessage = new ChatMessage(Singleton.Singleton.Instance.RoomID, Singleton.Singleton.Instance.MessageTyped);
            string s = "You:" + Singleton.Singleton.Instance.MessageTyped + '\n';
            ChatBox.text += s;
            s = Singleton.Singleton.Instance.DiskColor+"Player: " + Singleton.Singleton.Instance.MessageTyped + '\n';
            RoomChatMessage MessageToSend = new RoomChatMessage(Singleton.Singleton.Instance.RoomID,s);
            Singleton.Singleton.Instance.Connection.SendPacket(MessageToSend.getData());
        }
        
        void OnGUI()
        {
            if (InputField.isFocused && InputField.text != "" && Input.GetKey(KeyCode.Return))
            {
                SendChatMessage();
                InputField.text = "";
            }
        }

        void Awake()
        {
            ChatBox = GetComponentInChildren<Text>();
        }
        #endregion
    }
}
