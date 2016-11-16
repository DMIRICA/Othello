using UnityEngine;
using System.Collections;

public class Login : MonoBehaviour {

    #region Variabiles

    public static string User = "";
    public static string Password = "";
    public static string ConfirmPassword = "";
    public static string Email = "";
    public string CurrentMenu = "Login";

    public float X;
    public float Y;
    public float Width;
    public float Height;

    private 

    #endregion


    // Use this for initialization
    void Start () {
	
	}

    //Main GUI function
    void OnGUI()
    {
        if(CurrentMenu == "Login")
        {
            LoginGUI();
        }
        else if(CurrentMenu == "CreateAccount")
        {
            CreateAccountGUI();
        }
    }


    #region Custom methods

    void LoginGUI()
    {
        GUI.Box(new Rect(280, 70, (Screen.width / 4) + 200, (Screen.height / 4) + 130), "Login");

        if (GUI.Button(new Rect(360,250,120,25), "Create Account"))
        {
            CurrentMenu = "CreateAccount";
        }
        if (GUI.Button(new Rect(510, 250, 120, 25), "Login"))
        {
            
        }
        GUI.Label(new Rect(390, 130, 220, 23),"Username:");
        User = GUI.TextField(new Rect(390,150,220,23), User);


        GUI.Label(new Rect(390, 180, 220, 23), "Password:");
        Password = GUI.TextField(new Rect(390, 200, 220, 23), Password);

    }

    void CreateAccountGUI()
    {
        GUI.Box(new Rect(280, 50, (Screen.width / 4) + 200, (Screen.height / 4) + 240), "Create Account");

        if (GUI.Button(new Rect(360, 340, 120, 25), "Create Account"))
        {
           
        }
        if (GUI.Button(new Rect(510, 340, 120, 25), "Back"))
        {
            CurrentMenu = "Login";
        }
        GUI.Label(new Rect(390, 110, 220, 23), "Username:");
        User = GUI.TextField(new Rect(390, 130, 220, 23), User);


        GUI.Label(new Rect(390, 160, 220, 23), "Password:");
        Password = GUI.TextField(new Rect(390, 180, 220, 23), Password);

        GUI.Label(new Rect(390, 210, 220, 23), "Confirm Password:");
        ConfirmPassword = GUI.TextField(new Rect(390, 230, 220, 23), Password);

        GUI.Label(new Rect(390, 260, 220, 23), "Email:");
        Email = GUI.TextField(new Rect(390, 280, 220, 23), Password);
    }

    #endregion
}
