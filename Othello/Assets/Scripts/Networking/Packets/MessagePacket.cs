using Assets.Scripts.Entities;
using Assets.Scripts.MainScene;
using Assets.Scripts.Singleton;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Networking.Packets
{
    class MessagePacket : PacketStructure
    {
        public string Message;
        private int PacketID;
        private int Length;

        public MessagePacket(byte[] packet) : base(packet)
        {
            PacketID = ReadUShort(0);
            Length = ReadUShort(2);
            Message = ReadString(4, packet.Length - 4);
        }

        public MessagePacket(ushort PacketID, string Message) :
            base(PacketID, (ushort)(4 + Message.Length))
        {
            this.Message = Message;
            WriteString(Message, 4);
        }

        //Function called when your login acction succeed
        public IEnumerator loadMainScene()
        {
            
            if (Message != "")
            {
                string[] splits = Message.Split('|');
                foreach (string s in splits)
                {
                    if (s == "")
                        continue;
                    string[] users = s.Split(':');
                    User user = new User();
                    user.Username = users[0];
                    if (users[1] == "true" || users[1] == "True")
                        user.IsChallenged = true;
                    else
                        user.IsChallenged = false;

                    if (users[2] == "true" || users[2] == "True")
                        user.InGame = true;
                    else
                        user.InGame = false;

                    Singleton.Singleton.Instance.UsersLoggedList.Add(user);

                }
                Singleton.Singleton.Instance.UsersLoggedList.Sort((x, y) => string.Compare(x.Username, y.Username));
            }
            SceneManager.LoadScene("MainScene");
            if (Singleton.Singleton.Instance.Logout)
            {
                foreach (User user in Singleton.Singleton.Instance.UsersLoggedList)
                    SingletonUI.Instance.addNewUsersToScrollViewContent(user);
            }

            yield return null;
        }

        //Function called when new user come online. Add him to the UI list.
        public IEnumerator addNewUserLoggedToList()
        {
            if (Message != "")
            {
                string[] words = Message.Split(':');
                User user = new User();
                user.Username = words[0];
                user.InGame = (words[1] == "True" || words[1] == "true") ? true : false;
                Singleton.Singleton.Instance.UsersLoggedList.Add(user);
                SingletonUI.Instance.addNewUsersToScrollViewContent(user);
            }
            yield return null;
        }

        //Function called when you get a global chat message.
        public IEnumerator updateGlobalChat()
        {
            SingletonUI.Instance.ChatView.text += Message;
            yield return null;
        }

        //Function called when you get a challenge.
        public IEnumerator getChallenged()
        {
            if (!SingletonUI.Instance.LogoutModalPanel.activeSelf)
            {
                Singleton.Singleton.Instance.Me.IsChallenged = true;
                SingletonUI.Instance.AcceptChallengeButton.enabled = true;
                SingletonUI.Instance.AcceptChallengeButton.GetComponent<Image>().color = SingletonUI.Instance.defaultButtonCollor;
                SingletonUI.Instance.ChallengePanel.SetActive(true);
                SingletonUI.Instance.ChallengeText.text = Message;
            }
            else
            {
                //If u get challenge when logout panel is active refuse them and dont show the challenge panel.
                string[] splits = Message.Split(' ');
                MessagePacket packet = new MessagePacket(GameProtocol.ChallengeRefusedPacketID(),
                    Singleton.Singleton.Instance.Me.Username + ":" + splits[0]);
                Singleton.Singleton.Instance.Connection.SendPacket(packet.getData());
                Singleton.Singleton.Instance.Me.IsChallenged = false;
                SingletonUI.Instance.ChallengePanel.SetActive(false);
            }

            yield return null;
        }

        //Function called when you get a packet with the user who got challenged.
        //param number = 0 set it to online
        //param number = 1 set it to inGame
        //param number = 2 set it to challenged
        public IEnumerator updateUserStatus(int number)
        {
            var chlidren = SingletonUI.Instance.ScrollViewContent.GetComponentsInChildren<UserListItemScript>();

            foreach (UserListItemScript user in chlidren)
            {
                Text text = user.GetComponentInChildren<Text>();
                if (text.text == Message)
                {
                    if (number == 0)
                    {
                        user.transform.GetComponent<Image>().sprite = SingletonUI.Instance.OnlineSprite;
                        var prefabImageChildren = user.GetComponentsInChildren<Image>();
                        foreach (Image img in prefabImageChildren)
                        {
                            if (img.name == "ChallengeImage")
                            {
                                img.enabled = true;
                                break;
                            }
                        }
                        foreach (User u in Singleton.Singleton.Instance.UsersLoggedList)
                        {
                            if (u.Username == Message)
                            {
                                u.IsChallenged = false;
                                u.InGame = false;
                                break;
                            }

                        }
                    }
                    else if (number == 1)
                    {
                        user.transform.GetComponent<Image>().sprite = SingletonUI.Instance.ChallengedSprite;
                        var prefabImageChildren = user.GetComponentsInChildren<Image>();
                        foreach (Image img in prefabImageChildren)
                        {
                            if (img.name == "ChallengeImage")
                            {
                                img.enabled = false;
                                break;
                            }
                        }
                        foreach (User u in Singleton.Singleton.Instance.UsersLoggedList)
                        {
                            if (u.Username == Message)
                            {
                                u.IsChallenged = true;
                                break;
                            }

                        }
                        //Check if is result challenge is active with that user.
                        var list = SingletonUI.Instance.Helper.GetComponentsInChildren<ChallengeResultScript>();

                        foreach (ChallengeResultScript x in list)
                        {
                            if (x.Message.text.Split(' ')[0] == Message)
                            {
                                x.RechallengeButton.GetComponent<Image>().color = Color.red;
                                x.RechallengeButton.enabled = false;
                                break;
                            }
                        }
                    }
                    else if (number == 2)
                    {
                        user.transform.GetComponent<Image>().sprite = SingletonUI.Instance.InGameSprite;
                        var prefabImageChildren = user.GetComponentsInChildren<Image>();
                        foreach (Image img in prefabImageChildren)
                        {
                            if (img.name == "ChallengeImage")
                            {
                                img.enabled = false;
                                break;
                            }
                        }
                        foreach (User u in Singleton.Singleton.Instance.UsersLoggedList)
                        {
                            if (u.Username == Message)
                            {
                                u.InGame = true;
                                u.IsChallenged = false;
                                break;
                            }

                        }

                        //Check if is result challenge is active with that user.
                        var list = SingletonUI.Instance.Helper.GetComponentsInChildren<ChallengeResultScript>();

                        foreach (ChallengeResultScript x in list)
                        {
                            if (x.Message.text.Split(' ')[0] == Message)
                            {
                                x.RechallengeButton.GetComponent<Image>().color = Color.red;
                                x.RechallengeButton.enabled = false;
                                break;
                            }
                        }

                        if (SingletonUI.Instance.ChallengePanel.activeSelf)
                        {
                            if(SingletonUI.Instance.ChallengeText.text.Split(' ')[0] == Message)
                            {
                                SingletonUI.Instance.AcceptChallengeButton.enabled = false;
                                SingletonUI.Instance.AcceptChallengeButton.GetComponent<Image>().color = Color.red;
                            }
                        }
                    }
                    break;
                }
            }


            yield return null;
        }

        //Function called when someone ignored the challenge.
        public IEnumerator updateInUsersListAfterChallengeResult()
        {
            var chlidren = SingletonUI.Instance.ScrollViewContent.GetComponentsInChildren<UserListItemScript>();

            foreach (UserListItemScript user in chlidren)
            {
                Text text = user.GetComponentInChildren<Text>();
                if (text.text == Message)
                {
                    user.transform.GetComponent<Image>().sprite = SingletonUI.Instance.OnlineSprite;
                    var prefabImageChildren = user.GetComponentsInChildren<Image>();
                    foreach (Image img in prefabImageChildren)
                    {
                        if (img.name == "ChallengeImage")
                        {
                            img.enabled = true;
                            break;
                        }
                    }
                    break;
                }
            }
            Singleton.Singleton.Instance.UsersLoggedList.Single(p => p.Username == Message).IsChallenged = false;
            yield return null;
        }

        //Function which show the response from a challenge.
        public IEnumerator displayChallengeResultPanel(string str)
        {

            var chlidren = SingletonUI.Instance.ScrollViewContent.GetComponentsInChildren<UserListItemScript>();

            foreach (UserListItemScript user in chlidren)
            {
                Text text = user.GetComponentInChildren<Text>();
                if (text.text == Message)
                {
                    user.transform.GetComponent<Image>().sprite = SingletonUI.Instance.OnlineSprite;
                    var prefabImageChildren = user.GetComponentsInChildren<Image>();
                    foreach (Image img in prefabImageChildren)
                    {
                        if (img.name == "ChallengeImage")
                        {
                            img.enabled = true;
                            break;
                        }
                    }
                    break;
                }
            }
            Singleton.Singleton.Instance.UsersLoggedList.Single(p => p.Username == Message).IsChallenged = false;

            MainSceneScript.displayResponsePanel(str);
            yield return null;
        }

        //Function called when someone got disconnected.
        public IEnumerator userDisconnected()
        {
            var chlidren = SingletonUI.Instance.ScrollViewContent.GetComponentsInChildren<UserListItemScript>();

            foreach (UserListItemScript user in chlidren)
            {
                Text text = user.GetComponentInChildren<Text>();
                if (text.text == Message)
                {
                    user.destroyMyself();
                    break;
                }
            }
            var list = SingletonUI.Instance.Helper.GetComponentsInChildren<ChallengeResultScript>();

            foreach (ChallengeResultScript x in list)
            {
                if (x.Message.text.Split(' ')[0] == Message)
                {
                    x.RechallengeButton.GetComponent<Image>().color = Color.red;
                    x.RechallengeButton.enabled = false;
                    break;
                }
            }

            //If the disconnected player challanged me ->> close the challange
            if (SingletonUI.Instance.ChallengePanel.activeSelf)
            {
                String name = SingletonUI.Instance.ChallengeText.text.Split(' ')[0];
                if (name == Message)
                {
                    SingletonUI.Instance.ChallengePanel.SetActive(false);
                }
            }

            Singleton.Singleton.Instance.UsersLoggedList.RemoveAll(x => (x.Username == Message));
            yield return null;
        }

        public IEnumerator loadGameScene()
        {
            string[] split = Message.Split('|');
            //SingletonGame.Instance.RoomID = (ushort)Int32.Parse(split[0]);
            Singleton.Singleton.Instance.Me.IsChallenged = false;
            Singleton.Singleton.Instance.Me.InGame = true;
            SingletonUI.Instance.ChallengePanel.SetActive(false);
            SceneManager.LoadScene("GameScene");
            //SingletonGame.Instance.GameSceneCanvas.SetActive(true);
            SingletonUI.Instance.MainSceneCanvas.SetActive(false);
            yield return null;
        }

        public IEnumerator opponentReady()
        {
            SingletonGame.Instance.ReadyText.text = "Your opponent is ready!";
            yield return null;
        }

        public IEnumerator callGameOver()
        {
            SingletonGame.Instance.CallGameOver(Message);
            yield return null;
        }

        public IEnumerator updatePlayAgainPopUpText(string text)
        {
            SingletonGame.Instance.PlayAgainPopUpText.text = text;
            yield return null;
        }

        public IEnumerator opponentBackToLobby()
        {
            if (SingletonGame.Instance.GameOverPanel.activeSelf)
            {
                SingletonGame.Instance.GameOverPopUpText.text = "Your opponent left the room!";
                SingletonGame.Instance.PlayAgainButton.enabled = false;
                SingletonGame.Instance.PlayAgainButton.GetComponent<Image>().color = Color.red;
            }

            yield return null;
        }

        public IEnumerator backToLobby()
        {
            Singleton.Singleton.Instance.UsersLoggedList.Clear();
            Singleton.Singleton.Instance.Me.InGame = false;
            if (Message != "")
            {
                string[] splits = Message.Split('|');
                foreach (string s in splits)
                {
                    if (s == "")
                        continue;
                    string[] users = s.Split(':');
                    User user = new User();
                    user.Username = users[0];
                    if (users[1] == "true" || users[1] == "True")
                        user.IsChallenged = true;
                    else
                        user.IsChallenged = false;

                    if (users[2] == "true" || users[2] == "True")
                        user.InGame = true;
                    else
                        user.InGame = false;

                    Singleton.Singleton.Instance.UsersLoggedList.Add(user);

                }
                Singleton.Singleton.Instance.UsersLoggedList.Sort((x, y) => string.Compare(x.Username, y.Username));
            }
            SingletonGame.Instance.GameSceneCanvas.SetActive(false);
            SceneManager.LoadScene("MainScene");
            SingletonUI.Instance.MainSceneCanvas.SetActive(true);
            SingletonGame.Instance.prepareForNewGame();
            MainSceneScript.deleteAllUsersFromScrolViewContent();
            MainSceneScript.addUsersToScrollViewContent();

            //Destroy every restult challenge panel (if exists)
            var list = SingletonUI.Instance.Helper.GetComponentsInChildren<ChallengeResultScript>();
            foreach (ChallengeResultScript x in list)
            {
                x.destroyMyself();
            }
            //SingletonGame.Instance.GameOverPanel.SetActive(false);
            yield return null;
           
        }

        public IEnumerator opponentQuitAfterGameEnd()
        {
            SingletonGame.Instance.GameOverPopUpText.text = "Your opponent left the room!";
            SingletonGame.Instance.PlayAgainButton.enabled = false;
            SingletonGame.Instance.PlayAgainButton.GetComponent<Image>().color = Color.red;
            Singleton.Singleton.Instance.UsersLoggedList.RemoveAll(x => x.Username == Message);
            yield return null;
        }

        public IEnumerator opponentQuitWhileInGame()
        {
            SingletonGame.Instance.GameOverPopUpText.text = "Your opponent left the room!";
            SingletonGame.Instance.PlayAgainButton.enabled = false;
            SingletonGame.Instance.PlayAgainButton.GetComponent<Image>().color = Color.red;
            Singleton.Singleton.Instance.UsersLoggedList.RemoveAll(x => x.Username == Message);
            yield return null;
        }

        //Function used to run code in the main thread cuz you can't make changes in UI if you are not in the main thread.
        public static void runInMainThread(IEnumerator function)
        {
            UnityMainThreadDispatcher.Enqueue(function);
        }

    }
}
