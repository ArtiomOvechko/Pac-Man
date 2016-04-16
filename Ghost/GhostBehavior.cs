using PackMan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ghost
{
    public abstract class GhostBehavior
    {
        protected readonly List<Tuple<int, int>> _freeCells = new List<Tuple<int, int>>();
        protected IGhost _owner;

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
            _freeCells.Clear();
            _owner = ghost;
            SeekFree();
        }

        protected void SeekFree()
        {
            int x = _owner.X;
            int y = _owner.Y;
            if (!(_owner.Level.GameField.GameField[y - 1, x] is Wall))
            {
                if (NoGhost(x, y - 1))
                    _freeCells.Add(new Tuple<int, int>(x, y - 1));
            }
            if (!(_owner.Level.GameField.GameField[y + 1, x] is Wall))
            {
                if (NoGhost(x, y + 1))
                    _freeCells.Add(new Tuple<int, int>(x, y + 1));
            }
            if (x != 0 || y != _owner.Level.GameField.Height / 2 - 1)
            {
                if (!(_owner.Level.GameField.GameField[y, x - 1] is Wall))
                {
                    if (NoGhost(x - 1, y))
                        _freeCells.Add(new Tuple<int, int>(x - 1, y));
                }
            }
            else
            {
                if (NoGhost(_owner.Level.GameField.Width-1, y))
                    _freeCells.Add(new Tuple<int, int>(_owner.Level.GameField.Width - 1, y));
            }
            if (x != _owner.Level.GameField.Width - 1 || y != _owner.Level.GameField.Height / 2 - 1)
            {
                if (!(_owner.Level.GameField.GameField[y, x + 1] is Wall))
                {
                    if (NoGhost(x + 1, y))
                        _freeCells.Add(new Tuple<int, int>(x + 1, y));
                }
            }
            else
            {
                if (NoGhost(1, y))
                    _freeCells.Add(new Tuple<int, int>(x - 30, y));
            }
            switch (_freeCells.Count())
            {
                case (int)CellsArount.None:
                    break;
                case (int)CellsArount.One:
                    Push(_freeCells[0].Item1, _freeCells[0].Item2);
                    break;
                default:
                    MarkVisited();
                    BestWay();
                    break;
            }
        }

        protected bool NoGhost(int x, int y)
        {
            if (_owner.Level.Blinky.X == x && _owner.Level.Blinky.Y == y)
                return false;
            if (_owner.Level.Pinky.X == x && _owner.Level.Pinky.Y == y)
                return false;
            if (_owner.Level.Inky.X == x && _owner.Level.Inky.Y == y)
                return false;
            if (_owner.Level.Clyde.X == x && _owner.Level.Clyde.Y == y)
                return false;
            return true;
        }

        protected void MarkVisited()
        {
            _freeCells.Remove(_freeCells.FirstOrDefault(x => x.Item1 == _owner.OldX && x.Item2 == _owner.OldY));
        }

        protected double GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        protected void Push(int x, int y)
        {
            _owner.OldX = _owner.X;
            _owner.OldY = _owner.Y;
            _owner.X = x;
            _owner.Y = y;
        }

        protected abstract void BestWay();
    }
}
