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
        
        private char _DiskColor;
        private bool _IsHisTurn;
        private bool _PlayAgain;
        private bool _ReadyToStart;

        public Player()
        {
            _PlayAgain      = false;
            _IsHisTurn      = false;
            _ReadyToStart   = false;
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

        public bool ReadyToStart { get => _ReadyToStart; set => _ReadyToStart = value; }
    }
}
