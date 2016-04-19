using System;
using System.Collections.Generic;

using PackMan.Entities;
using PackMan.Interfaces;

namespace PackMan.Core
{
    public class Field: IField
    {
        private IObstacle[,] _gameField;

        private const int height = 32;

        private const int width = 32;

        private IEnumerable<IObstacle> GetAll()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    yield return GameField[i, j];
                }
            }
        }

        public IObstacle[,] GameField
        {
            get
            {
                return _gameField;
            }

            set
            {
                _gameField = value;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public Field(IFiller filler)
        {
            GameField = new IObstacle[Height, Width];
            filler.Fill(this);
        }

        public IEnumerable<Tuple<IObstacle, int, int>> GetAllCells()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    yield return new Tuple<IObstacle, int, int>(GameField[i, j], i, j);
                }
            }
        }

        public bool Completed()
        {
            foreach (var o in GetAll())
            {
                if ((o as Dot) != null)
                    return false;
            }
            return true;
        }
    }
}
