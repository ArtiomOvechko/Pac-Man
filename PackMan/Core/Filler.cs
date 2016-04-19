using System;
using System.Collections.Generic;
using System.Linq;

using PackMan.Entities;
using PackMan.Interfaces;

namespace PackMan.Core
{
    public class Filler : IFiller
    {
        private readonly SingleRandom _randomizer;

        private int _dots;

        private int _empty;

        private const int InitialValue = 0;

        private const int TargetEmptyValue = 200;

        private const int RandomLowerBorder = 1;

        private const int RandomUpperBorder = 31;

        private const int RandomUpperSecondBorder = 10;

        private const int CertainNumberOfRandomRange = 2;

        private const int TargetDotValue = 100;

        private enum Quantity
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4
        }

        public Filler()
        {
            _randomizer = SingleRandom.Instance;
            _dots = InitialValue;
            _empty = InitialValue;
        }

        public void Fill(IField field)
        {
            FillFrame(field);
            FillBaseInner(field);
            FillBaseRoute(field);
            SearchNewRoute(field);
            FillWalls(field);
            SearchNewEmptyRoute(field);
            AddCherries(field);
            AddBonuses(field);
            CleanSpecial(field);
        }

        private void FillFrame(IField field)
        {
            //creating frame
            for (int i = 0; i < field.Height; i++)
            {
                for (int j = 0; j < field.Width; j++)
                {
                    if (i == 0 || i == field.Height - 1 || j == 0 || j == field.Width - 1)
                    {
                        field.GameField[i, j] = new Wall();
                    }
                    else
                        field.GameField[i, j] = new Empty();
                }
            }
        }

        private void FillBaseInner(IField field)
        {
            //creating ghost cage
            for (int i = field.Height / 2 - 2; i < field.Height / 2 + 1; i++)
            {
                for (int j = field.Width / 2 - 3; j < field.Width / 2 + 2; j++)
                {
                    if (i == field.Height / 2 - 1 && j > field.Width / 2 - 3 && j < field.Width / 2 + 1)
                    {
                        field.GameField[i, j] = new Empty();
                        continue;
                    }
                    field.GameField[i, j] = new Wall();
                }
            }

        }
        private void FillBaseRoute(IField field)
        {
            //Filling horizontal line of dots before ghost cage
            for (int j = 1; j < field.Width / 2 - 3; j++)
            {
                field.GameField[field.Width / 2 - 1, j] = new Dot();
            }

            //Filling horizontal line of dots after ghost cage
            for (int j = field.Width / 2 + 2; j < 31; j++)
            {
                field.GameField[field.Width / 2 - 1, j] = new Dot();
            }

            //Filling horizontal line of dots below cage
            for (int j = field.Width / 2 - 4; j < field.Width / 2 + 3; j++)
            {
                field.GameField[field.Width / 2 + 1, j] = new Dot();
            }

            //Filling horizontal line of dots above cage
            for (int j = field.Width / 2 - 4; j < field.Width / 2 + 3; j++)
            {
                field.GameField[field.Width / 2 - 3, j] = new Dot();
            }

            for (int i = field.Width / 2 + 2; i < field.Height / 2 + 8; i++)
            {
                field.GameField[i, field.Width / 2 - 1] = new Dot();
            }

            //Filling other dots beyond cage
            field.GameField[field.Width / 2 - 4, field.Width / 2 - 1] = new Dot();
            field.GameField[field.Width / 2 - 2, field.Width / 2 - 4] = new Dot();
            field.GameField[field.Width / 2, field.Width / 2 - 4] = new Dot();
            field.GameField[field.Width / 2 - 2, field.Width / 2 + 2] = new Dot();
            field.GameField[field.Width / 2, field.Width / 2 + 2] = new Dot();

            //Replacing ghost and player spawn points by wall
            field.GameField[field.Height / 2 - 3, field.Width / 2 - 1] = new Wall();
            field.GameField[field.Height / 2 + 8, field.Width / 2 - 1] = new Wall();
        }

        private void MakeRandomRoute(IField field, int x, int y)
        {

            int i = InitialValue;
            int j = InitialValue;
            List<int> sides = new List<int>();
            if ((field.GameField[y - 1, x] as Empty) != null && !ManyNeighbours(field, y - 1, x))
            {
                sides.Add(1);
            }
            if ((field.GameField[y + 1, x] as Empty) != null && !ManyNeighbours(field, y + 1, x))
            {
                sides.Add(2);
            }
            if ((field.GameField[y, x - 1] as Empty) != null && !ManyNeighbours(field, y, x - 1))
            {
                sides.Add(3);
            }
            if ((field.GameField[y, x + 1] as Empty) != null && !ManyNeighbours(field, y, x + 1))
            {
                sides.Add(4);
            }
            if (!sides.Any())
            {
                SearchNewRoute(field);
                return;
            }

            int rand = _randomizer.RandomInstance.Next(sides.Count());
            switch (sides[rand])
            {
                case (int)Quantity.One:
                    i = y - 1;
                    j = x;
                    break;
                case (int)Quantity.Two:
                    i = y + 1;
                    j = x;
                    break;
                case (int)Quantity.Three:
                    i = y;
                    j = x - 1;
                    break;
                case (int)Quantity.Four:
                    i = y;
                    j = x + 1;
                    break;
            }
            field.GameField[i, j] = new Dot();
            if (++_empty == TargetEmptyValue)
                return;
            if (_randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder) == CertainNumberOfRandomRange)
            {
                SearchNewRoute(field);
                return;
            }
            MakeRandomRoute(field, j, i);
        }

        private void MakeRandomEmptyRoute(IField field, int x, int y)
        {
            int i = InitialValue;
            int j = InitialValue;
            List<int> sides = new List<int>();
            if ((field.GameField[y - 1, x] as DynamicWall) != null && !ManyNeighbours(field, y - 1, x) && !ManyEmpty(field, y - 1, x))
            {
                sides.Add(1);
            }
            if ((field.GameField[y + 1, x] as DynamicWall) != null && !ManyNeighbours(field, y + 1, x) && !ManyEmpty(field, y + 1, x))
            {
                sides.Add(2);
            }
            if ((field.GameField[y, x - 1] as DynamicWall) != null && !ManyNeighbours(field, y, x - 1) && !ManyEmpty(field, y + 1, x))
            {
                sides.Add(3);
            }
            if ((field.GameField[y, x + 1] as DynamicWall) != null && !ManyNeighbours(field, y, x + 1) && !ManyEmpty(field, y + 1, x))
            {
                sides.Add(4);
            }
            if (sides.Count() == 0)
            {
                SearchNewEmptyRoute(field);
                return;
            }

            int rand = _randomizer.RandomInstance.Next(sides.Count());
            switch (sides[rand])
            {
                case (int)Quantity.One:
                    i = y - 1;
                    j = x;
                    break;
                case (int)Quantity.Two:
                    i = y + 1;
                    j = x;
                    break;
                case (int)Quantity.Three:
                    i = y;
                    j = x - 1;
                    break;
                case (int)Quantity.Four:
                    i = y;
                    j = x + 1;
                    break;
            }
            field.GameField[i, j] = new Empty();
            if (++_dots == TargetDotValue)
                return;
            if (_randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperSecondBorder) == CertainNumberOfRandomRange)
            {
                SearchNewEmptyRoute(field);
                return;
            }
            MakeRandomEmptyRoute(field, j, i);
        }

        private void SearchNewRoute(IField field)
        {
            int x = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            int y = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            while ((field.GameField[y, x] as Dot) == null)
            {
                x = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
                y = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            }
            MakeRandomRoute(field, x, y);
        }

        private void SearchNewEmptyRoute(IField field)
        {
            int x = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            int y = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            while ((field.GameField[y, x] as Dot) == null && (field.GameField[y, x] as Empty) == null)
            {
                x = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
                y = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            }
            MakeRandomEmptyRoute(field, x, y);
        }

        private void AddCherries(IField field)
        {
            int x = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            int y = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            for (int i = 0; i < 4; i++)
            {
                while ((field.GameField[y, x] as Empty) == null)
                {
                    x = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
                    y = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
                }
                field.GameField[y, x] = new Cherry();
            }
        }

        private void AddBonuses(IField field)
        {
            int x = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            int y = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
            for (int i = 0; i < 4; i++)
            {
                while ((field.GameField[y, x] as Empty) == null)
                {
                    x = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
                    y = _randomizer.RandomInstance.Next(RandomLowerBorder, RandomUpperBorder);
                }
                field.GameField[y, x] = new Bonus();
            }
        }

        private bool ManyNeighbours(IField field, int y, int x)
        {
            if ((field.GameField[y - 1, x] as Dot) != null && (field.GameField[y - 1, x - 1] as Dot) != null && (field.GameField[y, x - 1] as Dot) != null)
            {
                return true;
            }
            if ((field.GameField[y + 1, x] as Dot) != null && (field.GameField[y + 1, x - 1] as Dot) != null && (field.GameField[y, x - 1] as Dot) != null)
            {
                return true;
            }
            if ((field.GameField[y, x - 1] as Dot) != null && (field.GameField[y + 1, x - 1] as Dot) != null && (field.GameField[y + 1, x] as Dot) != null)
            {
                return true;
            }
            if ((field.GameField[y, x + 1] as Dot) != null && (field.GameField[y + 1, x + 1] as Dot) != null && (field.GameField[y + 1, x] as Dot) != null)
            {
                return true;
            }
            return false;
        }

        private bool ManyEmpty(IField field, int y, int x)
        {
            if (y == field.Height - 1 || y == InitialValue || x == field.Width - 1 || x == InitialValue)
            {
                return false;
            }
            else
            {
                if ((field.GameField[y - 1, x] as Empty) != null && (field.GameField[y - 1, x - 1] as Empty) != null &&
                    (field.GameField[y, x - 1] as Empty) != null)
                {
                    return true;
                }
                if ((field.GameField[y + 1, x] as Empty) != null && (field.GameField[y + 1, x - 1] as Empty) != null &&
                    (field.GameField[y, x - 1] as Empty) != null)
                {
                    return true;
                }
                if ((field.GameField[y, x - 1] as Empty) != null && (field.GameField[y + 1, x - 1] as Empty) != null &&
                    (field.GameField[y + 1, x] as Empty) != null)
                {
                    return true;
                }
                if ((field.GameField[y, x + 1] as Empty) != null && (field.GameField[y + 1, x + 1] as Empty) != null &&
                    (field.GameField[y + 1, x] as Empty) != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void FillWalls(IField field)
        {
            foreach(Tuple<IObstacle, int, int> t in field.GetAllCells())
            {
                if((t.Item1 as Empty)!= null)
                {
                    field.GameField[t.Item2, t.Item3] = new DynamicWall();
                }
            }
        }

        private void CleanSpecial(IField field)
        {
            //Room exit
            field.GameField[field.Height / 2 - 1, 0] = new Empty();
            field.GameField[field.Height / 2 - 1, 31] = new Empty();

            //blinky spawn
            field.GameField[field.Height / 2 - 3, field.Width / 2 - 1] = new Empty();

            //Cage exit
            field.GameField[field.Height / 2 - 2, field.Width / 2 - 1] = new Empty();

            //Pac spawn
            field.GameField[field.Height / 2 + 8, field.Width / 2 - 1] = new Empty();
            
            //pinky, inky, clyde spawn
            field.GameField[field.Height / 2 - 1, field.Width / 2 - 2] = new Empty();
            field.GameField[field.Height / 2 - 1, field.Width / 2 - 1] = new Empty();
            field.GameField[field.Height / 2 - 1, field.Width / 2 ] = new Empty();
        }
    }
}