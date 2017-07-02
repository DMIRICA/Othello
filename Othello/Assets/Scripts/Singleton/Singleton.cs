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

        #region UI Elements

        #region GameOver elements

        public Text GameOverText;
        public GameObject GameOverPanel;

        #endregion


        #region Surrender elements
        public GameObject SurrenderPanel;
        #endregion

        #region Score elements
        public Text BlackChips;
        public Text RedChips;
        #endregion

        //Sprites
        public List<Sprite> sprites;
        private User _Me;
        private List<User> _UsersLoggedList;

        #endregion

        #region Variables
        private static Singleton _Instance;
        private List<BoardPosition> _LegalMoves;
        private ClientConnection _Connection;
        private bool _Logout;
        private bool _IsYourTurn;
        private bool _DrawNumbers;
        private CellColor _DiskColor;
        private string _MessageTyped;
        private ushort _RoomID;
        #endregion

        #region Methods

        private Singleton()
        {
            _LegalMoves = new List<BoardPosition>();
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

        public CellColor DiskColor
        {
            get
            {
                return _DiskColor;
            }
            set
            {
                _DiskColor = value;
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

        public ClientConnection Connection
        {
            get
            {
                return _Connection;
            }

        }

        public bool IsYourTurn
        {
            get
            {
                return _IsYourTurn;
            }
            set
            {
                _IsYourTurn = value;
            }
        }

        public List<BoardPosition> LegalMoves
        {
            get
            {
                return _LegalMoves;
            }
            set
            {
                _LegalMoves = value;
            }
        }

        public string MessageTyped
        {
            get
            {
                return _MessageTyped;
            }

            set
            {
                _MessageTyped = value;
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

        public bool DrawNumbers
        {
            get
            {
                return _DrawNumbers;
            }

            set
            {
                _DrawNumbers = value;
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

        #endregion

        // void Awake()
        // {
        //    //_Instance = this;
        //_LegalMoves = new List<BoardPosition>();
        // _Connection = new ClientConnection();
        //BlackChips.text = 0.ToString();
        //RedChips.text = 0.ToString();
        //}



        void OnApplicationQuit()
        {
            GamePacket Message = new GamePacket(GameProtocol.UserDisconnected(),RoomID, "quit");
            Connection.SendPacket(Message.getData());
            Connection.CloseSocket();
        }

        public void QuitCommand()
        {
            GamePacket Message = new GamePacket(GameProtocol.UserDisconnected(), RoomID, "quit");
            Connection.SendPacket(Message.getData());
            Connection.CloseSocket();
            Application.Quit();
        }

        public void PlayAgain()
        {
            GamePacket PlayAgainPacket = new GamePacket(GameProtocol.PlayAgain(), RoomID, "play again");
            Connection.SendPacket(PlayAgainPacket.getData());
        }

        public void SurrenderCommand()
        {
            SurrenderPanel.SetActive(false);
            GamePacket SurrenderPacket = new GamePacket(GameProtocol.Surrender(), RoomID, "");
            Connection.SendPacket(SurrenderPacket.getData());

        }

        public void ShowSurrenderPanel()
        {
            SurrenderPanel.SetActive(true);
        }

        public void HideSurrenderPanel()
        {
            SurrenderPanel.SetActive(false);
        }

        public void CallGameOver(string Message)
        {
            GameOverText.text = Message;
            GameOverPanel.SetActive(true);
            LegalMoves.Clear();
            GameBoard.RemoveDrawMoves();
        }
        
        public void DisplayNumbers(bool boolean)
        {
            if (!boolean)
            {
                DrawNumbers = false;
                foreach (BoardPosition bp in LegalMoves)
                {
                    GameBoard.gameBoard[bp.Row, bp.Column].ChangeText.text = "";
                }
            }
            else
            {
                DrawNumbers = true;
                foreach (BoardPosition bp in LegalMoves)
                {
                    GameBoard.gameBoard[bp.Row, bp.Column].ChangeText.text = bp.NumberOfChanges.ToString();
                }
            }
            
           
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