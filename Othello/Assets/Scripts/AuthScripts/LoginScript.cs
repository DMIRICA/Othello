using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Assets.Scripts.Singleton;
using UnityEngine.UI;
using System.Timers;
using UnityEngine.EventSystems;
using Assets.Scripts.Utils;
using Assets.Scripts.Entities;
using Assets.Scripts.Networking.Packets;

public class LoginScript : MonoBehaviour
{

    private string _Username;
    private string _Password;
    public static Text popUpText;
    public EventSystem eventSystem;
    

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
        if (eventSystem.currentSelectedGameObject == null || !Input.GetKeyDown(KeyCode.Tab))
            return;

        Selectable current = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
        if (current == null)
            return;

        bool up = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Selectable next = up ? current.FindSelectableOnUp() : current.FindSelectableOnDown();

        // We are at the end or the beginning, go to either, depends on the direction we are tabbing in
        // The previous version would take the logical 0 selector, which would be the highest up in your editor hierarchy
        // But not certainly the first item on your GUI, or last for that matter
        // This code tabs in the correct visual order
        if (next == null)
        {
            next = current;

            Selectable pnext;
            if (up) while ((pnext = next.FindSelectableOnDown()) != null) next = pnext;
            else while ((pnext = next.FindSelectableOnUp()) != null) next = pnext;
        }

        // Simulate Inputfield MouseClick
        InputField inputfield = next.GetComponent<InputField>();
        if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(eventSystem));

        // Select the next item in the taborder of our direction
        eventSystem.SetSelectedGameObject(next.gameObject);
    }
    #endregion


    private void Awake()
    {
        popUpText = GameObject.Find("PopUpText").GetComponent<Text>();
    }

    public void CreateAccountHandler()
    {
        SceneManager.LoadScene("CreateAccountScene");
    }

    public static void LoadMainScene()
    {
        if(SingletonUI.Instance != null)
        {

        }
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            SceneManager.LoadScene("MainScene");
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
            Singleton.Instance.Me = new User(_Username,false,false);
            MessagePacket packet = new MessagePacket(GameProtocol.LoginPacketID(), _Username + "|" + hash);
            Singleton.Instance.Connection.SendPacket(packet.getData());
        }
    }

    public bool areFieldsEmpty()
    {
        return _Username.Equals("") || _Password.Equals("");
    }

    public static IEnumerator updatePopUpText(string text)
    {
        popUpText.text = text;
        yield return null;
    }

    public static void runInMainThread(IEnumerator function)
    {
        UnityMainThreadDispatcher.Enqueue(function);
    }

}
