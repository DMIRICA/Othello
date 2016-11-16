using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Assets.Scripts.Singleton;

public class LoginScript : MonoBehaviour {

    private string Username;
    private string Password;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void getUsername(string Username)
    {
        this.Username = Username;
    }

    public void getPassword(string Password)
    {
        this.Password = Password;
    }

    public void CreateAccountHandler()
    {
        SceneManager.LoadScene("CreateAccountScene");
    }

    public void LoginButtonHandler()
    {
        if(Username == "ynad" && Password == "123")
        {
            SceneManager.LoadScene("GameScene");
            Singleton.Instance.IsYourTurn = false;
        }else
        {
            
        }
    }

}
