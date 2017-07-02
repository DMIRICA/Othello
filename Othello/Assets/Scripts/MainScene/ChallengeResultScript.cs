using Assets.Scripts.Networking.GamePacktes;
using Assets.Scripts.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeResultScript : MonoBehaviour {


    public Text TimerText;
    public Text Message;
    private int _ValueTimer;
    public Button RechallengeButton;

    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {

    }

    private void OnEnable()
    {
        ValueTimer = 5;
        TimerText.text = "" + ValueTimer;
        StartCoroutine("counter");
    }

    // Update is called once per frame
    void Update()
    {
        TimerText.text = "" + ValueTimer;
    }


    public void YesButtonChallenge()
    {
        //TODO 
    }

    public void NoButtonChallenge()
    {
        //TODO
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
            if (ValueTimer == 0)
            {
                StopCoroutine("counter");
                destroyMyself();
            }
        }
    }

    public void closeButtonAction()
    {
        destroyMyself();
    }

    public void resendChallengeAction()
    {
        string[] splits = Message.text.Split(' ');
        MessagePacket packet = new MessagePacket(GameProtocol.ChallengePacketID(), splits[0] + "|" + Singleton.Instance.Me.Username + " challenged you to a game!");
        Singleton.Instance.Connection.SendPacket(packet.getData());
        destroyMyself();
    }

    public void destroyMyself()
    {
        Destroy(gameObject);
    }
}
