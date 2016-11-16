using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class BoardPosition
    {
        private int _Row;
        private int _Column;
        private char _Color;

        public BoardPosition(int ROW, int COLUMN , char COLOR)
        {
            Row = ROW;
            Column = COLUMN;
            Color = COLOR;
        }

        public int Row
        {
            get
            {
                return _Row;
            }
            set
            {
                _Row = value;
            }
        }
        public int Column
        {
            get
            {
                return _Column;
            }
            set
            {
                _Column = value;
            }
        }
        public char Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
            }
        }
    }
}
