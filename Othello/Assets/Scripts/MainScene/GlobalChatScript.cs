﻿using Assets.Scripts.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.MainScene
{
    public class GlobalChatScript : MonoBehaviour
    {

        public InputField ChatInputField;

       

        public void sendChatMessage()
        {
            if(ChatInputField.text != "")
            {
                string line = Singleton.Singleton.Instance.Me.Username + ": " + ChatInputField.text + '\n';
                SingletonUI.Instance.ChatView.text += line;

                MessagePacket packet = new MessagePacket(GameProtocol.GlobalChatMessagePacketID(), line);
                Singleton.Singleton.Instance.Connection.SendPacket(packet.getData());
                ChatInputField.text = "";
            }
            
        }

    }
}
