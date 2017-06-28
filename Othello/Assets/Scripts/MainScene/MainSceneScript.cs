using Assets.Scripts.Entities;
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
        public GameObject ScrollViewContent;
        public GameObject ModalPanel;
        public GameObject UserItemPrefab;
        public static GameObject UserItemPrefabStatic;
        public static GameObject ScrollViewContentStatic;

        // Use this for initialization
        void Start()
        {
            addUsersToScrollViewContent();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Awake()
        {
            UserItemPrefabStatic = UserItemPrefab;
            ScrollViewContentStatic = ScrollViewContent;
        }

        public void addUsersToScrollViewContent()
        {
            if (Singleton.Singleton.Instance.UsersLoggedList.Count > 0)
            {
                if (ScrollViewContent != null)
                {
                    foreach (User user in Singleton.Singleton.Instance.UsersLoggedList)
                    {
                        GameObject userListItem = Instantiate(UserItemPrefab) as GameObject;
                        UserListItemScript script = userListItem.GetComponent<UserListItemScript>();
                        script.Username.text = user.Username;
                        if (user.InGame)
                        {
                            userListItem.transform.GetComponent<Image>().sprite = script.InGameSprite;
                            script.ChallengeImage.enabled = false;
                        }
                        else
                        {
                            userListItem.transform.GetComponent<Image>().sprite = script.OnlineSprite;
                            script.ChallengeImage.enabled = true;
                        }
                        userListItem.transform.SetParent(ScrollViewContent.transform);
                    }
                }
            }
        }

        public void addUsersToScrollViewContentWithFilter(string filter)
        {
            if (Singleton.Singleton.Instance.UsersLoggedList.Count > 0)
            {
                if (ScrollViewContent != null)
                {
                    foreach (User user in Singleton.Singleton.Instance.UsersLoggedList)
                    {
                        if (!user.Username.Contains(filter))
                            continue;
                        GameObject userListItem = Instantiate(UserItemPrefab) as GameObject;
                        UserListItemScript script = userListItem.GetComponent<UserListItemScript>();
                        script.Username.text = user.Username;
                        if (user.InGame)
                        {
                            userListItem.transform.GetComponent<Image>().sprite = script.InGameSprite;
                            script.ChallengeImage.enabled = false;
                        }
                        else
                        {
                            userListItem.transform.GetComponent<Image>().sprite = script.OnlineSprite;
                            script.ChallengeImage.enabled = true;
                        }
                        userListItem.transform.SetParent(ScrollViewContent.transform);
                    }
                }
            }
        }

        public static void addNewUsersToScrollViewContent(User user)
        {
            GameObject userListItem = Instantiate(UserItemPrefabStatic) as GameObject;
            UserListItemScript script = userListItem.GetComponent<UserListItemScript>();
            script.Username.text = user.Username;
            if (user.InGame)
            {
                userListItem.transform.GetComponent<Image>().sprite = script.InGameSprite;
                script.ChallengeImage.enabled = false;
            }
            else
            {
                userListItem.transform.GetComponent<Image>().sprite = script.OnlineSprite;
                script.ChallengeImage.enabled = true;
            }
            userListItem.transform.SetParent(ScrollViewContentStatic.transform);
        }

        public void searchUpdate(string text)
        {
            deleteAllUsersFromScrolViewContent();
            addUsersToScrollViewContentWithFilter(text);
        }

        public void deleteAllUsersFromScrolViewContent()
        {
            foreach (Transform child in ScrollViewContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void logoutAction()
        {
            ModalPanel.SetActive(true);
        }

        public void noLogoutAction()
        {
            ModalPanel.SetActive(false);
        }

        public void yesLogoutAction()
        {
            ModalPanel.SetActive(false);
            Singleton.Singleton.Instance.UsersLoggedList.Clear();
            SceneManager.LoadScene("LoginScene");
        }
    }
}
