using Assets.Scripts.Game;
using Assets.Scripts.Networking.GamePackets;
using Assets.Scripts.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Singleton
{
    public class SingletonGame : MonoBehaviour
    {
        /// <summary>
        /// TO DO
        /// If i challenge mutiple users and someone accept it -> make other invites blocked
        /// </summary>
        private static SingletonGame _Instance;

        public Text GameOverText;
        public GameObject GameOverPanel;
        public GameObject SurrenderPanel;
        public GameObject ReadyPanel;
        public GameObject GameSceneCanvas;
        public Image GameOverImage;
        public Button ReadyButton;
        public Button PlayAgainButton;
        public Text PlayAgainPopUpText;
        public Text BlackChips;
        public Text RedChips;
        public Text ReadyText;
        public Text MyReadyText;
        public Text GameOverPopUpText;
        public List<Sprite> sprites;
        public Sprite winSprite;
        public Sprite lostSprite;
        private List<BoardPosition> _LegalMoves;
        private bool _IsYourTurn;
        private bool _DrawNumbers;
        private CellColor _DiskColor;
        private string _MessageTyped;
        public InputField InputField;
        public Text ChatBox;
        public Color defaultButtonCollor;
        public static SingletonGame Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = (SingletonGame)FindObjectOfType(typeof(SingletonGame));
                    if(_Instance != null)
                        DontDestroyOnLoad(_Instance.gameObject);
                }

                return _Instance;
            }
        }

        private void Awake()
        {
            // if the singleton hasn't been initialized yet
            if (_Instance != null && _Instance != this)
            {
                Destroy(this.gameObject);
                return;//Avoid doing anything else
            }

            _Instance = this;
            DontDestroyOnLoad(this.gameObject);
            defaultButtonCollor = PlayAgainButton.GetComponent<Image>().color;
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

        private SingletonGame()
        {
            LegalMoves = new List<BoardPosition>();
        }

        void OnApplicationQuit()
        {
            if (GameOverPanel.activeSelf)
            {
                MessageRoomPacket packet = new MessageRoomPacket(GameProtocol.QuitAfterGameOver(),Singleton.Instance.RoomID,Singleton.Instance.Me.Username);
                Singleton.Instance.Connection.SendPacket(packet.getData());
                Singleton.Instance.Connection.CloseSocket();
            }
            else
            {
                MessageRoomPacket packet = new MessageRoomPacket(GameProtocol.QuitWhileInGame(), Singleton.Instance.RoomID, Singleton.Instance.Me.Username);
                Singleton.Instance.Connection.SendPacket(packet.getData());
                Singleton.Instance.Connection.CloseSocket();
            }
        }

        public void backToLobby()
        {
            MessageRoomPacket packet = new MessageRoomPacket(GameProtocol.BackToLobby(), Singleton.Instance.RoomID, Singleton.Instance.Me.Username);
            Singleton.Instance.Connection.SendPacket(packet.getData());
            
        }



        public void PlayAgain()
        {
            MessageRoomPacket packet = new MessageRoomPacket(GameProtocol.PlayAgain(), Singleton.Instance.RoomID, Singleton.Instance.Me.Username);
            Singleton.Instance.Connection.SendPacket(packet.getData());
            PlayAgainButton.enabled = false;
            PlayAgainButton.GetComponent<Image>().color = Color.red;
            GameOverPopUpText.text = "Waiting a response from your opponent.";
        }
        
        public void resetGameOverPanel()
        {
            GameOverText.text = "";
            GameOverPopUpText.text = "";
            PlayAgainButton.enabled = true;
            PlayAgainButton.GetComponent<Image>().color = defaultButtonCollor;
            GameOverPanel.SetActive(false);
            
        }

        public void resetReadyPanel()
        {
            MyReadyText.text = "When you are ready press the button bellow!";
            ReadyText.text = "Your opponent is not ready!";
            ReadyButton.enabled = true;
            ReadyButton.GetComponent<Image>().color = defaultButtonCollor;
            ReadyPanel.SetActive(true);
        }

        public void prepareForNewGame()
        {
            resetGameOverPanel();
            resetReadyPanel();
            BlackChips.text = "0";
            RedChips.text = "0";
            ChatBox.text = "";
        }

        public void SurrenderCommand()
        {
            SurrenderPanel.SetActive(false);
            MessageRoomPacket packet = new MessageRoomPacket(GameProtocol.Surrender(), Singleton.Instance.RoomID, Singleton.Instance.Me.Username);
            Singleton.Instance.Connection.SendPacket(packet.getData());

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
            if (Message.Contains("won"))
            {
                GameOverImage.sprite = winSprite;
            }
            else if(Message.Contains("lost"))
            {
                GameOverImage.sprite = lostSprite;
            }
            GameOverPanel.SetActive(true);
            PlayAgainButton.enabled = true;
            PlayAgainButton.GetComponent<Image>().color = defaultButtonCollor;
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

        public void ReadyButtonAction()
        {
            ReadyButton.GetComponent<Image>().color = Color.red;
            ReadyButton.enabled = false;
            MyReadyText.text = "You are ready! Waiting your opponent!";

            MessageRoomPacket packet = new MessageRoomPacket(GameProtocol.PlayerReady(),
               Singleton.Instance.RoomID, Singleton.Instance.Me.Username);
            Singleton.Instance.Connection.SendPacket(packet.getData());
        }

        public void SendChatMessage()
        {
            if (InputField.text != "")
            {
                string s = Singleton.Instance.Me.Username + ": " + SingletonGame.Instance.MessageTyped + '\n';
                ChatBox.text += s;
                s = Singleton.Instance.Me.Username + ": " + SingletonGame.Instance.MessageTyped + '\n';
                MessageRoomPacket MessageToSend = new MessageRoomPacket(GameProtocol.RoomChatMessagePacketID(), Singleton.Instance.RoomID, s);
                Singleton.Instance.Connection.SendPacket(MessageToSend.getData());
                InputField.text = "";
            }
        }

    }
}
