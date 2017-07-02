using Assets.Scripts.Game;
using Assets.Scripts.Networking.Packets;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Networking.GamePackets
{
    public class GamePacket : PacketStructure
    {
        #region Variables

        public string Message;
        public ushort RoomID;

        #endregion

        #region Methods

        public GamePacket(byte[] packet) : base(packet)
        {
            
            Message = ReadString(6, packet.Length - 6);
            
            #region LegalMoves
            if (ReadUShort(0) == GameProtocol.BoardMoves())
            {
                DrawMovesMainThread();
            }
            #endregion

            #region Turn and Color
            //IF IS PACKET WITH HIS TURN AND DISKCOLOR DO
            if (ReadUShort(0) == GameProtocol.TurnAndColor())
            {
                RoomID = ReadUShort(4);
                Singleton.Singleton.Instance.RoomID = ReadUShort(4);
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    GameBoard.RemoveDrawMoves();
                    if (Singleton.Singleton.Instance.GameOverPanel.activeSelf)
                    {
                        Singleton.Singleton.Instance.GameOverPanel.SetActive(false);
                    }
                    string[] message = Message.Split('|');
                    Singleton.Singleton.Instance.IsYourTurn = Convert.ToBoolean(message[0]);
                    if (message[1][0] == 'B')
                    {
                        Singleton.Singleton.Instance.DiskColor = CellColor.Black;
                        Singleton.Singleton.Instance.BlackChips.color = Color.green;
                        Singleton.Singleton.Instance.RedChips.color = Color.black;
                    }
                    else if (message[1][0] == 'R')
                    {
                        Singleton.Singleton.Instance.DiskColor = CellColor.Red;
                        Singleton.Singleton.Instance.RedChips.color = Color.green;
                        Singleton.Singleton.Instance.BlackChips.color = Color.black;
                    } 
                });
            }
            #endregion

            #region Turn
            if(ReadUShort(0) == GameProtocol.PlayerTurnPacket())
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Singleton.Singleton.Instance.IsYourTurn = Convert.ToBoolean(Message);
                });
            }
            #endregion

            #region GameOver
            if(ReadUShort(0) == GameProtocol.GameOverPacket())
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    if (Singleton.Singleton.Instance.SurrenderPanel.activeSelf)
                    {
                        Singleton.Singleton.Instance.SurrenderPanel.SetActive(false);
                    }
                    Singleton.Singleton.Instance.CallGameOver(Message);
                });
            }
            #endregion

        }
        public GamePacket(ushort PacketID,ushort roomID, string Message) :
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
               
                Singleton.Singleton.Instance.LegalMoves.Add(new BoardPosition(Int32.Parse(aux[0]), Int32.Parse(aux2[0]), Int32.Parse(aux2[1])));
            }
            foreach (BoardPosition p in Singleton.Singleton.Instance.LegalMoves)
                {
                    GameBoard.gameBoard[p.Row,p.Column].selfTile.color = Color.white;
                    if(Singleton.Singleton.Instance.DrawNumbers)
                        GameBoard.gameBoard[p.Row, p.Column].ChangeText.text = p.NumberOfChanges.ToString();
                    
                }
            
            yield return null;
        }
        public void DrawMovesMainThread()
        {
            UnityMainThreadDispatcher.Enqueue(DrawMoves());
        }

        #endregion
    }
}