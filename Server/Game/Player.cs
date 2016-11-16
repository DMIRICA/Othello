using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Player
    {
        private Socket _PlayerSocket;
        private char _DiskColor;
        private bool _IsHisTurn;
        private bool _PlayAgain;

        public Player(Socket s)
        {
            PlayerSocket = s;
            _PlayAgain = false;
        }
        public char DiskColor
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
        public bool IsHisTurn
        {
            get
            {
                return _IsHisTurn;
            }
            set
            {
                _IsHisTurn = value;
            }
        }
        public Socket PlayerSocket
        {
            get
            {
                return _PlayerSocket;
            }
            set
            {
                _PlayerSocket = value;
            }
        }
        public bool PlayAgain
        {
            get
            {
                return _PlayAgain;
            }
            set
            {
                _PlayAgain = value;
            }
        }
    }
}
