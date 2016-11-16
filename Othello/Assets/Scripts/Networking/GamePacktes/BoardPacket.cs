using Assets.Scripts.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Networking.GamePackets
{
    public class BoardPacket : PacketStructure
    {
        public char[,] BoardTable = new char[8, 8];

        public BoardPacket(byte[] packet) : base(packet)
        {
            BoardTable = ToCharArray(packet);
            UpdateBoardMainThreadCall();
        }
        public IEnumerator UpdateBoard()
        {
            int RedCount = 0;
            int BlackCount = 0;
            #region update table
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (BoardTable[i, j])
                    {
                        case 'E':
                            if (GameBoard.gameBoard[i, j].CellColor != CellColor.Empty)
                            {
                                GameBoard.gameBoard[i, j].CellColor = CellColor.Empty;
                            }
                            break;
                        case 'R':
                            if(GameBoard.gameBoard[i, j].CellColor != CellColor.Red)
                            {
                                GameBoard.gameBoard[i, j].CellColor = CellColor.Red;
                                GameBoard.gameBoard[i, j].PlayFlipAnimation();
                            }
                            RedCount++;
                            break;
                        case 'B':
                            if(GameBoard.gameBoard[i, j].CellColor != CellColor.Black)
                            {
                                GameBoard.gameBoard[i, j].CellColor = CellColor.Black;
                                GameBoard.gameBoard[i, j].PlayFlipAnimation();
                            }
                            BlackCount++;
                            break;
                    }
                }
            }

            Singleton.Singleton.Instance.BlackChips.text = BlackCount.ToString();
            Singleton.Singleton.Instance.RedChips.text = RedCount.ToString();

            #endregion

            yield return null;
        }
        public void UpdateBoardMainThreadCall()
        {
            UnityMainThreadDispatcher.Instance().Enqueue(UpdateBoard());
        }
        public byte[] ToByteArray(char[,] nmbs)
        {
            byte[] nmbsBytes = new byte[nmbs.GetLength(0) * nmbs.GetLength(1) * 2];
            int k = 0;
            for (int i = 0; i < nmbs.GetLength(0); i++)
            {
                for (int j = 0; j < nmbs.GetLength(1); j++)
                {
                    byte[] array = BitConverter.GetBytes(nmbs[i, j]);
                    for (int m = 0; m < array.Length; m++)
                    {
                        nmbsBytes[k++] = array[m];
                    }
                }
            }
            return nmbsBytes;
        }
        public char[,] ToCharArray(byte[] nmbsBytes)
        {
            char[,] nmbs = new char[8,8];
            int k = 4;
            for (int i = 0; i < nmbs.GetLength(0); i++)
            {
                for (int j = 0; j < nmbs.GetLength(1); j++)
                {
                    nmbs[i, j] = BitConverter.ToChar(nmbsBytes, k);
                    k += 2;
                }
            }
            return nmbs;
        }

    }
}
