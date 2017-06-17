using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Assets.Scripts.Singleton;
using UnityEngine.UI;
using System.Timers;
using Assets.Scripts.Networking.GamePacktes;
using UnityEngine.EventSystems;
using Assets.Scripts.Utils;

public class LoginScript : MonoBehaviour
{

    private string _Username;
    public string _Password;
    public static Text popUpText;
    private EventSystem eventSystem;
    

    

    public void getUsername(string username)
    {
        _Username = username;
    }

    public void getPassword(string un)
    {
        _Password = un;
    }


    // Use this for initialization
    void Start()
    {
        this.eventSystem = EventSystem.current;
        _Username = "";
        _Password = "";
    }

    // Update is called once per frame
    void Update()
    {
        #region Use tab to navigate beetwen fields
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = null;
            Selectable current = null;

            // Figure out if we have a valid current selected gameobject
            if (eventSystem.currentSelectedGameObject != null)
            {
                // Unity doesn't seem to "deselect" an object that is made inactive
                if (eventSystem.currentSelectedGameObject.activeInHierarchy)
                {
                    current = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
                }
            }

            if (current != null)
            {
                // When SHIFT is held along with tab, go backwards instead of forwards
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    next = current.FindSelectableOnLeft();
                    if (next == null)
                    {
                        next = current.FindSelectableOnUp();
                    }
                }
                else
                {
                    next = current.FindSelectableOnRight();
                    if (next == null)
                    {
                        next = current.FindSelectableOnDown();
                    }
                }
            }
            else
            {
                // If there is no current selected gameobject, select the first one
                if (Selectable.allSelectables.Count > 0)
                {
                    next = Selectable.allSelectables[0];
                }
            }

            if (next != null)
            {
                next.Select();
            }
        }
        #endregion
    }

    private void Awake()
    {
        popUpText = GameObject.Find("PopUpText").GetComponent<Text>();
    }

    public void CreateAccountHandler()
    {
        SceneManager.LoadScene("CreateAccountScene");
    }

    public static void LoadGameScene()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            SceneManager.LoadScene("GameScene");
        });
    }

    public void LoginButtonHandler()
    {
        if (areFieldsEmpty())
        {
            popUpText.text = "Complete all fields!";
        }
        else
        {
            string hash = Singleton.Instance.getHashFromString(_Password);
            MessagePacket packet = new MessagePacket(GameProtocol.LoginPacketID(), _Username + "|" + hash);
            Singleton.Instance.Connection.SendPacket(packet.getData());
        }
    }

    public bool areFieldsEmpty()
    {
        return _Username.Equals("") || _Password.Equals("");
    }

    public static void HidePopUpText()
    {
        UnityMainThreadDispatcher.Enqueue(hideText());
        
    }

    public static IEnumerator hideText()
    {
        popUpText.text = "";
        yield return null;
    }

    public static void LoginFailed()
    {
        UnityMainThreadDispatcher.Enqueue(authProblem());
        
    }
   
    public static IEnumerator authProblem()
    {
        popUpText.text = "Username and password do not match";    
        yield return null;
    }

}
