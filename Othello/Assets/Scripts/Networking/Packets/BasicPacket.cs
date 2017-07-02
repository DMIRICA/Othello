using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Networking.Packets
{
    class BasicPacket : PacketStructure
    {
        int packetID;

        public BasicPacket(byte[] packet) : base(packet)
        {
            packetID = ReadUShort(0);
        }

        public BasicPacket(ushort packetID) :
            base(packetID, (ushort)4)
        {

        }

       
        public IEnumerator SuccessRegister()
        {
            Text popUpText = GameObject.Find("PopUpText").GetComponent<Text>();
            popUpText.text = "Successfully registered! You can go back and login.";
            yield return null;
        }

        public IEnumerator FailedRegister()
        {
            Text popUpText = GameObject.Find("PopUpText").GetComponent<Text>();
            popUpText.text = "Someting gone wrong! Please contact the administrator.";
            yield return null;
        }

        public IEnumerator UsernameUsed()
        {
            Text popUpText = GameObject.Find("PopUpText").GetComponent<Text>();
            popUpText.text = "This username is already used! Please choose other one.";
            yield return null;
        }

        public IEnumerator EmailUsed()
        {
            Text popUpText = GameObject.Find("PopUpText").GetComponent<Text>();
            popUpText.text = "This email is already associated with an other account!";
            yield return null;
        }

        public void runInMainThread(IEnumerator function)
        {
            UnityMainThreadDispatcher.Enqueue(function);
        }
    }
}
