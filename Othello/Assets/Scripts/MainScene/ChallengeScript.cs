using Assets.Scripts.Networking.Packets;
using Assets.Scripts.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeScript : MonoBehaviour {


    public Text TimerText;
    private int _ValueTimer;


    // Use this for initialization
    void Start () {
        
    }

    void Awake()
    {
    
    }

    private void OnEnable()
    {
        ValueTimer = 10;
        TimerText.text = "" + ValueTimer;
        StartCoroutine("counter");
    }

    // Update is called once per frame
    void Update ()
    {
        TimerText.text = "" + ValueTimer;
	}


    public void YesButtonChallenge()
    {
        //TODO 
    }

    public void NoButtonChallenge()
    {
        string[] splits = SingletonUI.Instance.ChallengeText.text.Split(' ');
        MessagePacket packet = new MessagePacket(GameProtocol.ChallengeRefusedPacketID(), Singleton.Instance.Me.Username + ":" + splits[0]);
        Singleton.Instance.Connection.SendPacket(packet.getData());
        Singleton.Instance.Me.IsChallenged = false;
        gameObject.SetActive(false);
    }

    private IEnumerator counter()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            ValueTimer--;
        }
        
    }

    public int ValueTimer
    {
        get { return _ValueTimer; }

        set
        {
            _ValueTimer = value;
            if(ValueTimer == 0)
            {
                StopCoroutine("counter");
                sendNoResponseToChallengePacket();
                SingletonUI.Instance.ChallengePanel.SetActive(false);
            }
        }
    }

    public void sendNoResponseToChallengePacket()
    {
        string[] splits = SingletonUI.Instance.ChallengeText.text.Split(' ');
        MessagePacket packet = new MessagePacket(GameProtocol.ChallengeTimeoutPacketID(),Singleton.Instance.Me.Username +":" + splits[0]);
        Singleton.Instance.Connection.SendPacket(packet.getData());
        Singleton.Instance.Me.IsChallenged = false;
    }
}
