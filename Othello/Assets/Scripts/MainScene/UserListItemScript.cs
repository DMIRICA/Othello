using Assets.Scripts.Networking.Packets;
using Assets.Scripts.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserListItemScript : MonoBehaviour {

    public Image ChallengeImage;
    public Text Username;


    public void FightButton()
    {
        MessagePacket packet = new MessagePacket(
            GameProtocol.ChallengePacketID(),Username.text + "|" +Singleton.Instance.Me.Username + " challenged you to a game!");
        Singleton.Instance.Connection.SendPacket(packet.getData());

        var list = SingletonUI.Instance.Helper.GetComponentsInChildren<ChallengeResultScript>();

        //If is already a challange result from that user close it.
        foreach (ChallengeResultScript x in list)
        {
            if (x.Message.text.Split(' ')[0] == Username.text)
            {
                x.destroyMyself();
                break;
            }
        }
    }

    public void mouseOverEnter(BaseEventData data)
    {
        ChallengeImage.color = new Color(1, 1, 1, 1);
    }

    public void mouseOverExit(BaseEventData data)
    {
        ChallengeImage.color = new Color(1, 1, 1, 0.5f);
    }

    public void destroyMyself()
    {
        Destroy(gameObject);
    }
}
