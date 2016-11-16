using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Networking;
using Assets.Scripts.Game;
using UnityEngine.UI;
using Assets.Scripts.Networking.GamePackets;

namespace Assets.Scripts.Singleton
{
    public class Singleton : MonoBehaviour
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

        #endregion

        #region Variables
        private static Singleton _Instance;
        private List<BoardPosition> _LegalMoves;
        private ClientConnection _Connection;
        private bool _IsYourTurn;
        private CellColor _DiskColor;
        private string _MessageTyped;
        private ushort _RoomID;
        #endregion

        #region Methods

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
        #endregion

        void Awake()
        {
            _Instance = this;
            LegalMoves = new List<BoardPosition>();
            _Connection = new ClientConnection();
            BlackChips.text = 0.ToString();
            RedChips.text = 0.ToString();
        }

        void OnApplicationQuit()
        {
            GamePacket Message = new GamePacket(GameProtocol.QuitGame(),RoomID, "quit");
            Connection.SendPacket(Message.getData());
            Connection.CloseSocket();
        }

        public void QuitCommand()
        {
            GamePacket Message = new GamePacket(GameProtocol.QuitGame(), RoomID, "quit");
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

        public void CallSurrenderPanel()
        {
            SurrenderPanel.SetActive(true);
        }

        public void NoSurrender()
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
        #endregion

    }
}