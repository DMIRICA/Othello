using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserListItemScript : MonoBehaviour {


    public Sprite OnlineSprite;
    public Sprite InGameSprite;
    public Image ChallengeImage;
    public Text Username;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FightButton()
    {
        Debug.Log("Click chalange");
        ChallengeImage.enabled = false;
    }

    public void mouseOverEnter(BaseEventData data)
    {
        ChallengeImage.color = new Color(1, 1, 1, 1);
    }

    public void mouseOverExit(BaseEventData data)
    {
        ChallengeImage.color = new Color(1, 1, 1, 0.5f);
    }
}
