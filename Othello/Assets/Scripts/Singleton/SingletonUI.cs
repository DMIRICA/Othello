using Assets.Scripts.Entities;
using Assets.Scripts.Networking.Packets;
using Assets.Scripts.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingletonUI : MonoBehaviour
{

    private static SingletonUI _Instance;

    public GameObject ChallengePanel;
    public Text ChallengeText;
    public GameObject LogoutModalPanel;
    public GameObject ScrollViewContent;
    public GameObject UserItemPrefab;
    public GameObject ChallengeResponsePrefab;
    public GameObject Helper;
    public Text ChatView;
    public Sprite OnlineSprite;
    public Sprite InGameSprite;
    public Sprite ChallengedSprite;


    void OnApplicationQuit()
    {
        Singleton.Instance.Connection.SendPacket(new BasicPacket(999).getData());
    }

    void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this.gameObject);
        }

        //GameObject obj = Instantiate(ChallengeResultPanel) as GameObject;
    }

    public static SingletonUI Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = (SingletonUI)FindObjectOfType(typeof(SingletonUI));
                DontDestroyOnLoad(_Instance.gameObject);
            }

            return _Instance;
        }
    }

    public void addNewUsersToScrollViewContent(User user)
    {
        GameObject userListItem = Instantiate(SingletonUI.Instance.UserItemPrefab) as GameObject;
        UserListItemScript script = userListItem.GetComponent<UserListItemScript>();
        script.Username.text = user.Username;
        if (user.InGame)
        {
            userListItem.transform.GetComponent<Image>().sprite = SingletonUI.Instance.InGameSprite;
            script.ChallengeImage.enabled = false;
        }
        else
        {
            userListItem.transform.GetComponent<Image>().sprite = SingletonUI.Instance.OnlineSprite;
            script.ChallengeImage.enabled = true;
        }
        userListItem.transform.SetParent(SingletonUI.Instance.ScrollViewContent.transform);
    }
}

