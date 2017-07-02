using Assets.Scripts.Entities;
using Assets.Scripts.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.MainScene
{
    class MainSceneScript : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            addUsersToScrollViewContent();
            if (SingletonUI.Instance.ChatView.text != "")
            {
                SingletonUI.Instance.ChatView.text = "";
            }
        }


        //After login you get a list with all users logged.This function populate the list with all users.
        public static void addUsersToScrollViewContent()
        {
            if (Singleton.Singleton.Instance.UsersLoggedList.Count > 0)
            {
                if (SingletonUI.Instance.ScrollViewContent != null)
                {
                    foreach (User user in Singleton.Singleton.Instance.UsersLoggedList)
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
            }
        }

        //Filtrate the users logged list and add them to UI. Used on search option.
        public void addUsersToScrollViewContentWithFilter(string filter)
        {
            if (Singleton.Singleton.Instance.UsersLoggedList.Count > 0)
            {
                if (SingletonUI.Instance.ScrollViewContent != null)
                {
                    foreach (User user in Singleton.Singleton.Instance.UsersLoggedList)
                    {
                        if (!user.Username.Contains(filter))
                            continue;
                        GameObject userListItem = Instantiate(SingletonUI.Instance.UserItemPrefab) as GameObject;
                        UserListItemScript script = userListItem.GetComponent<UserListItemScript>();
                        script.Username.text = user.Username;
                        if (user.InGame)
                        {
                            userListItem.transform.GetComponent<Image>().sprite = SingletonUI.Instance.InGameSprite;
                            script.ChallengeImage.enabled = false;
                        }
                        else if (user.IsChallenged)
                        {
                            userListItem.transform.GetComponent<Image>().sprite = SingletonUI.Instance.ChallengedSprite;
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
            }
        }
        /*
        //Add a user to UI list. Used when someone login.
        public static void addNewUsersToScrollViewContent(User user)
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
        */
        //Update function used to search for users.
        public void searchUpdate(string text)
        {
            deleteAllUsersFromScrolViewContent();
            addUsersToScrollViewContentWithFilter(text);
        }

        //Delete users from UI list.
        public void deleteAllUsersFromScrolViewContent()
        {
            foreach (Transform child in SingletonUI.Instance.ScrollViewContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        //Open Logout panel.
        public void logoutAction()
        {
            //IF the challenge panel is active then send to the callenger refuse and close the panel
            if (SingletonUI.Instance.ChallengePanel.activeSelf)
            {
                string[] splits = SingletonUI.Instance.ChallengeText.text.Split(' ');
                MessagePacket packet = new MessagePacket(GameProtocol.ChallengeRefusedPacketID(),
                    Singleton.Singleton.Instance.Me.Username + ":" + splits[0]);
                Singleton.Singleton.Instance.Connection.SendPacket(packet.getData());
                Singleton.Singleton.Instance.Me.IsChallenged = false;
                SingletonUI.Instance.ChallengePanel.SetActive(false);
            }

            //Destroy every result panels ( if exists)
            var list = SingletonUI.Instance.Helper.GetComponentsInChildren<ChallengeResultScript>();
            foreach (ChallengeResultScript x in list)
            {
                x.destroyMyself();
            }

            SingletonUI.Instance.LogoutModalPanel.SetActive(true);

        }

        //Action for button NO on logout panel.
        public void noLogoutAction()
        {
            SingletonUI.Instance.LogoutModalPanel.SetActive(false);
        }

        //Action for button YES on logout panel.
        public void yesLogoutAction()
        {
            Singleton.Singleton.Instance.Connection.SendPacket(new BasicPacket(998).getData()); 
            SingletonUI.Instance.LogoutModalPanel.SetActive(false);
            Singleton.Singleton.Instance.UsersLoggedList.Clear();
            deleteAllUsersFromScrolViewContent();
            Singleton.Singleton.Instance.Logout = true;
            SceneManager.LoadScene("LoginScene");
        }

        //Action for button rooms from MainScene.
        public void roomButtonAction()
        {
            
        }

        public static void displayResponsePanel(string message)
        {
            //Verify if exists same resultResponsePanel
            if (!SingletonUI.Instance.LogoutModalPanel.activeSelf)
            {
                var list = SingletonUI.Instance.Helper.GetComponentsInChildren<ChallengeResultScript>();

                foreach (ChallengeResultScript x in list)
                {
                    if (x.Message.text == message)
                    {
                        x.destroyMyself();
                        break;
                    }
                }

                GameObject obj = Instantiate(SingletonUI.Instance.ChallengeResponsePrefab) as GameObject;
                ChallengeResultScript script = obj.GetComponent<ChallengeResultScript>();
                script.Message.text = message;
                obj.transform.SetParent(SingletonUI.Instance.Helper.transform, false);
            }
        }
    }
}
