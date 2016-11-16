using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{

    public class GameBoard
    {

        public BoardPosition[,] Gameboard;

        public GameBoard()
        {
            InitBoard();
            DrawBoard();
        }
        public void InitBoard()
        {
            Gameboard = new BoardPosition[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Gameboard[i, j] = new BoardPosition(i, j, 'E');
                }
            }

            PlaceIntitCell();
        }
        private void PlaceIntitCell()
        {
            Gameboard[4, 4].Color = 'R';
            Gameboard[3, 4].Color = 'B';
            Gameboard[4, 3].Color = 'B';
            Gameboard[3, 3].Color = 'R';
        }
        private void DrawBoard()
        {
            Console.WriteLine();
            Console.WriteLine();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(Gameboard[i, j].Color + "  ");
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }
        public bool IsMoveLegal(int x, int y, char color)
        {
            for (int yIdx = -1; yIdx < 2; yIdx++)
            {
                for (int xIdx = -1; xIdx < 2; xIdx++)
                {
                    if ((yIdx != 0) || (xIdx != 0))
                    {
                        int testX = x + xIdx;
                        int testY = y + yIdx;
                        bool firstCell = true;
                        bool done = false;
                        while (!done && (testX >= 0) && (testX <= 7) && (testY >= 0) && (testY <= 7))
                        {

                            if (firstCell)
                            {
                                if ((Gameboard[testX, testY].Color == color) ||
                                    (Gameboard[testX, testY].Color == 'E'))
                                {
                                    done = true;
                                }
                                firstCell = false;
                            }
                            else if (Gameboard[testX, testY].Color == 'E')
                            {
                                done = true;
                            }
                            else if (Gameboard[testX, testY].Color == color)
                            {
                                return true;
                            }
                            testX += xIdx;
                            testY += yIdx;
                        }
                    }
                }
            }
            return false;
        }
        public List<BoardPosition> GetAllLegalMoves(char color)
        {
            List<BoardPosition> moves = new List<BoardPosition>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (Gameboard[x, y].Color == 'E')
                    {
                        if (IsMoveLegal(x, y, color))
                        {
                            moves.Add(Gameboard[x, y]);
                        }
                    }
                }
            }

            return moves;
        }

        public int GetNumberOfChanges(int x , int y, char color)
        {
            //int count = 0;
            List<BoardPosition> flipPoints = FindToFlip(x,y, color);         
            return flipPoints.Count;
        }
        public bool HasALegalMove(char color)
        {
            return GetAllLegalMoves(color).Count > 0;
        }
        public void TakeMove(int x, int y, char color)
        {

            List<BoardPosition> flipPoints = FindToFlip(x, y, color);
            while (flipPoints.Count > 0)
            {
                BoardPosition chipPos = flipPoints[0];
                flipPoints.RemoveAt(0);
                if (Gameboard[chipPos.Row, chipPos.Column].Color != color)
                { // might have already been visited	
                    FlipOneChip(chipPos.Row, chipPos.Column, color);

                    //flipPoints.AddRange(FindToFlip((int)chipPos.x,(int)chipPos.y,color));
                }
            }


        }
        public void FlipOneChip(int x, int y, char color)
        {
            if (Gameboard[x, y].Color == 'E')
            {
                throw new System.ArgumentException("Cannot flip empty cell");
            }

            if (Gameboard[x, y].Color == color)
            {
                throw new System.ArgumentException("Error cannot flip cell of same color at " + x + "," + y + " color=" + color);
            }
            Gameboard[x, y].Color = color;
            //gameBoard[x, y].PlayFlipAnimation();

        }
        public void AddChip(int x, int y, char color)
        {
            if (Gameboard[x, y].Color == 'E')
            {
                Gameboard[x, y].Color = color;
                DrawBoard();
            }
        }
        private List<BoardPosition> FindToFlip(int x, int y, char color)
        {
            //if (color == 'E')
            //{
            //    throw new System.ArgumentException("'Empty' is not a valid color");
            //}
            List<BoardPosition> flipPoints = new List<BoardPosition>();
            for (int xIdex = -1; xIdex < 2; xIdex++)
            {
                for (int yIdex = -1; yIdex < 2; yIdex++)
                {
                    if ((xIdex != 0) || (yIdex != 0))
                    {
                        int testX = x + xIdex;
                        int testY = y + yIdex;
                        bool done = false;
                        List<BoardPosition> tempPointList = new List<BoardPosition>();
                        while (!done && (testY >= 0) && (testY <= 7) && (testX >= 0) && (testX <= 7))
                        {
                            if (Gameboard[testX, testY].Color == color)
                            { // end of run success
                                flipPoints.AddRange(tempPointList);
                                done = true;
                            }
                            else if (Gameboard[testX, testY].Color == 'E')
                            { // end of run failure]
                                done = true;
                            }
                            else
                            {
                                tempPointList.Add(new BoardPosition(testX, testY, color));
                                //		Debug.Log("added "+testX+","+testY+" to flip list");
                                testX += xIdex;
                                testY += yIdex;
                            }
                        }
                    }
                }
            }
            return flipPoints;
        }
        public char GetWinnerColor()
        {
            int countRed = 0;
            int countBlack = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (Gameboard[i, j].Color)
                    {
                        case 'R':
                            countRed++;
                            break;

                        case 'B':
                            countBlack++;
                            break;

                        default:
                            continue;

                    }

                }
            }
            if (countBlack > countRed)
            {
                return 'B';
            }
            else
            {
                return 'R';
            }
        }
        public int GetNumberOfDisk(char color)
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(Gameboard[i,j].Color == color)
                    {
                        count++;
                    }
                }
            }
            return count;
        } 

    }
}
