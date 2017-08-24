using Assets.Scripts.Game;
using Assets.Scripts.Networking.Packets;
using Assets.Scripts.Singleton;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Networking.GamePackets
{
    public class MessageRoomPacket : PacketStructure
    {
        #region Variables

        public string Message;
        public ushort RoomID;

        #endregion

        #region Methods

        public MessageRoomPacket(byte[] packet) : base(packet)
        {
            Message = ReadString(6, packet.Length - 6);
            RoomID = ReadUShort(4);
        }
        public MessageRoomPacket(ushort PacketID,ushort roomID, string Message) :
            base(PacketID, (ushort)(6 + Message.Length))
        {
            RoomID = roomID;
            WriteUShort(roomID, 4);
            this.Message = Message;
            WriteString(Message, 6);
        }

        public IEnumerator DrawMoves()
        {
            string[] allMoves = Message.Split('|');
            foreach (string s in allMoves)
            {
                string[] aux = s.Split(':');
                string[] aux2 = aux[1].Split('!');

                SingletonGame.Instance.LegalMoves.Add(new BoardPosition(Int32.Parse(aux[0]), Int32.Parse(aux2[0]), Int32.Parse(aux2[1])));
            }
            foreach (BoardPosition p in SingletonGame.Instance.LegalMoves)
                {
                    GameBoard.gameBoard[p.Row,p.Column].selfTile.color = Color.white;
                    if(SingletonGame.Instance.DrawNumbers)
                        GameBoard.gameBoard[p.Row, p.Column].ChangeText.text = p.NumberOfChanges.ToString();
                    
                }
            
            yield return null;
        }
        
        public IEnumerator turnAndDiskColor()
        {
            SingletonGame.Instance.ReadyPanel.SetActive(false);
            GameBoard.RemoveDrawMoves();
            if (SingletonGame.Instance.GameOverPanel.activeSelf)
            {
                SingletonGame.Instance.GameOverPanel.SetActive(false);
                SingletonGame.Instance.GameOverPopUpText.text = "";
                
            }
            string[] message = Message.Split('|');
            SingletonGame.Instance.IsYourTurn = Convert.ToBoolean(message[0]);
            if (message[1][0] == 'B')
            {
                SingletonGame.Instance.DiskColor = CellColor.Black;
                SingletonGame.Instance.BlackChips.color = Color.green;
                SingletonGame.Instance.RedChips.color = Color.black;
            }
            else if (message[1][0] == 'R')
            {
                SingletonGame.Instance.DiskColor = CellColor.Red;
                SingletonGame.Instance.RedChips.color = Color.green;
                SingletonGame.Instance.BlackChips.color = Color.black;
            }

            yield return null;
        }

        public IEnumerator doRoomChat()
        {

            SingletonGame.Instance.ChatBox.text += Message;
            yield return null;
        }

        public IEnumerator getTurn()
        {
            SingletonGame.Instance.IsYourTurn = Convert.ToBoolean(Message);
            yield return null;
        }

        public IEnumerator loadGameScene()
        {

            Singleton.Singleton.Instance.RoomID = ReadUShort(4);
            Singleton.Singleton.Instance.Me.IsChallenged = false;
            Singleton.Singleton.Instance.Me.InGame = true;
            SingletonUI.Instance.ChallengePanel.SetActive(false);
            SceneManager.LoadScene("GameScene");
            SingletonUI.Instance.MainSceneCanvas.SetActive(false);
            if (SingletonGame.Instance != null)
            {
                SingletonGame.Instance.GameSceneCanvas.SetActive(true);
            }
            //
            SingletonUI.Instance.MainSceneCanvas.SetActive(false);
            yield return null;
        }

        public static void runInMainThread(IEnumerator function)
        {
            UnityMainThreadDispatcher.Enqueue(function);
        }
        #endregion
    }
}