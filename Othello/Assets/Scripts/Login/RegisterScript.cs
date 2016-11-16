using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RegisterScript : MonoBehaviour {

    private string Username;
    private string Password;
    private string CPassword;
    private string Email;

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

    public void getCPassword(string CPassword)
    {
        this.CPassword = CPassword;
    }

    public void getEmail(string Email)
    {
        this.Email = Email;
    }

    public void BackButtonHandler()
    {
        SceneManager.LoadScene("LoginScene");
    }

}
