using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Networking;
using Assets.Scripts.Game;
using UnityEngine.UI;
using Assets.Scripts.Networking.GamePackets;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Linq;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Singleton
{
    class Singleton
    {
        private static Singleton _Instance;

        private User _Me;
        private List<User> _UsersLoggedList;
        private ClientConnection _Connection;
        private bool _Logout;
        private ushort _RoomID;


        #region Methods

        private Singleton()
        {
           
            _Connection = new ClientConnection();
            UsersLoggedList = new List<User>();
            _Logout = false;
            Me = new User();
        }

        #region GET/SET
        public static Singleton Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new Singleton();
                }
                return _Instance;
            }
        }

        
        public ClientConnection Connection
        {
            get
            {
                return _Connection;
            }

        }


        internal List<User> UsersLoggedList
        {
            get
            {
                return _UsersLoggedList;
            }

            set
            {
                _UsersLoggedList = value;
            }
        }

        public bool Logout
        {
            get
            {
                return _Logout;
            }

            set
            {
                _Logout = value;
            }
        }

        public User Me
        {
            get
            {
                return _Me;
            }

            set
            {
                _Me = value;
            }
        }

        public ushort RoomID
        {
            get
            {
                return _RoomID;
            }

            set
            {
                _RoomID = value;
            }
        }

        #endregion


        public User GetUserLoogedByUsername(string username)
        {
            foreach (User u in _UsersLoggedList)
            {
                if (u.Username == username)
                {
                    return u;
                }
            }
            return null;
        }

        #endregion


        #region EncryptFunction
        public string getHashFromString(string value)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
        #endregion
    }
}