using System;
using System.Collections.Generic;
using System.Linq;

using PackMan.Entities;
using PackMan.Interfaces;

namespace PackMan.Abstract
{
    public abstract class BaseGhostBehavior
    {
        protected readonly List<Tuple<int, int>> FreeCells = 
            new List<Tuple<int, int>>();

        protected IGhost Owner;

        protected enum Conditions
        {
            Flee, Normal
        }

        protected enum Directions
        {
            North=1, East=2, South=3, West=4  
        }

        private enum CellsArount
        {
            None=0, One=1
        } 

        public void Move(IGhost ghost)
        {
            FreeCells.Clear();
            Owner = ghost;
            SeekFree();
        }

        protected void SeekFree()
        {
            int x = Owner.X;
            int y = Owner.Y;
            if (!(Owner.Level.GameField.GameField[y - 1, x] is Wall))
            {
                if (NoGhost(x, y - 1))
                    FreeCells.Add(new Tuple<int, int>(x, y - 1));
            }
            if (!(Owner.Level.GameField.GameField[y + 1, x] is Wall))
            {
                if (NoGhost(x, y + 1))
                    FreeCells.Add(new Tuple<int, int>(x, y + 1));
            }
            if (x != 0 || y != Owner.Level.GameField.Height / 2 - 1)
            {
                if (!(Owner.Level.GameField.GameField[y, x - 1] is Wall))
                {
                    if (NoGhost(x - 1, y))
                        FreeCells.Add(new Tuple<int, int>(x - 1, y));
                }
            }
            else
            {
                if (NoGhost(Owner.Level.GameField.Width-1, y))
                    FreeCells.Add(new Tuple<int, int>(Owner.Level.GameField.Width - 1, y));
            }
            if (x != Owner.Level.GameField.Width - 1 || y != Owner.Level.GameField.Height / 2 - 1)
            {
                if (!(Owner.Level.GameField.GameField[y, x + 1] is Wall))
                {
                    if (NoGhost(x + 1, y))
                        FreeCells.Add(new Tuple<int, int>(x + 1, y));
                }
            }
            else
            {
                if (NoGhost(1, y))
                    FreeCells.Add(new Tuple<int, int>(x - 30, y));
            }
            switch (FreeCells.Count())
            {
                case (int)CellsArount.None:
                    break;
                case (int)CellsArount.One:
                    Push(FreeCells[0].Item1, FreeCells[0].Item2);
                    break;
                default:
                    MarkVisited();
                    BestWay();
                    break;
            }
        }

        protected bool NoGhost(int x, int y)
        {
            if (Owner.Level.Blinky.X == x && Owner.Level.Blinky.Y == y)
                return false;
            if (Owner.Level.Pinky.X == x && Owner.Level.Pinky.Y == y)
                return false;
            if (Owner.Level.Inky.X == x && Owner.Level.Inky.Y == y)
                return false;
            if (Owner.Level.Clyde.X == x && Owner.Level.Clyde.Y == y)
                return false;
            return true;
        }

        protected void MarkVisited()
        {
            FreeCells.Remove(FreeCells.FirstOrDefault
                (x => x.Item1 == Owner.OldX && x.Item2 == Owner.OldY));
        }

        protected double GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        protected void Push(int x, int y)
        {
            Owner.OldX = Owner.X;
            Owner.OldY = Owner.Y;
            Owner.X = x;
            Owner.Y = y;
        }

        protected abstract void BestWay();
    }
}
