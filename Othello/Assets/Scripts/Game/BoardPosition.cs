using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Game
{
    public class BoardPosition
    {

        private int _Row;
        private int _Column;
        private int _NumberOfChanges;

        public BoardPosition(int row, int column)
        {
            Row = row;
            Column = column;
            
        }


        public BoardPosition(int row, int column,int numberofchanges)
        {
            Row = row;
            Column = column;
            NumberOfChanges = numberofchanges;
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

        public int NumberOfChanges
        {
            get
            {
                return _NumberOfChanges;
            }

            set
            {
                _NumberOfChanges = value;
            }
        }
    }
}
