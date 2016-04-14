using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ghost;
using PackMan.Entities;

namespace BlueGhostBehavior
{
    public class BlueGhostBehave: GhostBehavior
    {
        private const int _xTarget = 32;
        private const int _yTarget = 32;
        private Tuple<int, int> GetPoints(int x1, int y1)
        {
            int x = 0;
            int y = 0;
            x = 2 * x1 + _owner.Level.Blinky.X;
            y = 2 * y1 + _owner.Level.Blinky.Y;
            return new Tuple<int, int>(x,y);
        }

        protected override void BestWay()
        {
            Tuple<int, int> min = _freeCells[0];
            Tuple<int, int> point = new Tuple<int, int>(0,0);
            Conditions currentCondition = (Conditions)Enum.Parse(typeof (Conditions), _owner.Condition);
            switch (currentCondition)
            {
                case Conditions.Flee:
                    foreach (Tuple<int, int> t in _freeCells)
                    {
                        double minimal = GetDistance(min.Item1, min.Item2, _xTarget, _yTarget);
                        double current = GetDistance(t.Item1, t.Item2, _xTarget, _yTarget);
                        if (current < minimal)
                            min = t;
                    }
                    break;
                case Conditions.Normal:
                    foreach (Tuple<int, int> t in _freeCells)
                    {
                        switch (_owner.Level.Pacman.Direction)
                        {
                            case (int)Directions.North:
                                point = GetPoints(_owner.Level.Pacman.X, _owner.Level.Pacman.Y - 2);
                                break;
                            case (int)Directions.East:
                                point = GetPoints(_owner.Level.Pacman.X + 2, _owner.Level.Pacman.Y ); 
                                break;
                            case (int)Directions.West:
                                point = GetPoints(_owner.Level.Pacman.X, _owner.Level.Pacman.Y+2);
                                break;
                            case (int)Directions.South:
                                point = GetPoints(_owner.Level.Pacman.X - 2, _owner.Level.Pacman.Y);
                                break;
                        }
                        double minimal = GetDistance(min.Item1, min.Item2, point.Item1, point.Item2);
                        double current = GetDistance(t.Item1, t.Item2, point.Item1, point.Item2);
                        if (current < minimal)
                            min = t;
                    }
                    break;
            }
            Push(min.Item1, min.Item2);
        }
    }
}
