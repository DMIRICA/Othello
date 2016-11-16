using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Packets
{
    public class BoardPacket : PacketStructure
    {
        public char[,] BoardTable = new char[8, 8];

        public BoardPacket(byte[] packet) : base(packet)
        {
            BoardTable = ToCharArray(packet);

        }
        public BoardPacket(ushort PacketID,Room room) : 
            base(PacketID, (ushort)(4+(room.Gameboard.Gameboard.GetLength(0) * room.Gameboard.Gameboard.GetLength(1) * 2)))
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    BoardTable[i, j] = room.Gameboard.Gameboard[i, j].Color;
                }
            }
            byte[] tmp = ToByteArray(BoardTable);
            Array.Copy(tmp, 0, getData(), 4, tmp.Length);
            
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
            char[,] nmbs = new char[nmbsBytes.Length / 2 / 2, 2];
            int k = 0;
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
