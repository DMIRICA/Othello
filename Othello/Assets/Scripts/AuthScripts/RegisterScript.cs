using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Assets.Scripts.Singleton;
using UnityEngine.EventSystems;
using Assets.Scripts.Networking.GamePacktes;

namespace AuthScripts
{
    public class RegisterScript : MonoBehaviour
    {

        private string _Username;
        private string _Password;
        private string _CPassword;
        private string _Email;
        private Text _PopUpText;
        private EventSystem eventSystem;

        // Use this for initialization
        void Start()
        {
            _Username = "";
            _Password = "";
            _CPassword = "";
            _Email = "";
            _PopUpText = GameObject.Find("PopUpText").GetComponent<Text>();
            this.eventSystem = EventSystem.current;
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




        public void getUsername(string Username)
        {
            this._Username = Username;
        }
        public void getPassword(string Password)
        {
            this._Password = Password;
        }

        public void getCPassword(string CPassword)
        {
            this._CPassword = CPassword;
        }

        public void getEmail(string Email)
        {
            this._Email = Email;
        }

        static bool IsValidEmail(string strIn)
        {

            return Regex.IsMatch(strIn, @"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$");
        }

        public void BackButtonHandler()
        {
            SceneManager.LoadScene("LoginScene");
        }


        public void RegisterButtonHandler()
        {
            if (!areFieldsEmpty())
            {
                if (doPasswordMatch())
                {
                    if (isPasswordValid(_Password))
                    {
                        if (IsValidEmail(_Email))
                        {
                            string hash = Singleton.Instance.getHashFromString(_Password);
                            MessagePacket packet = new MessagePacket(GameProtocol.CreateAccountPacketID(),
                                _Username + "|" + hash + "|" + _Email);
                            Singleton.Instance.Connection.SendPacket(packet.getData());

                        }
                        else
                        {
                            _PopUpText.text = "Please anter a valid email!";
                        }
                    }
                    else
                    {
                        _PopUpText.text = "Password must have 6 characters or more!";
                    }
                }
                else
                {
                    _PopUpText.text = "Both passwords must be same!";
                }
            }
            else
            {
                _PopUpText.text = "Complete all fields please!";
            }
        }

        public bool areFieldsEmpty()
        {
            return _Username.Equals("") || _Password.Equals("") || _CPassword.Equals("") || _Email.Equals("");
        }

        public bool doPasswordMatch()
        {
            return _Password.Equals(_CPassword);
        }

        public bool isPasswordValid(string strIn)
        {
            return strIn.Length >= 6;
        }

    }
}