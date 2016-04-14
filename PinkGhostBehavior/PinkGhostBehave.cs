using System;
using Ghost;
using System.Collections.Generic;
using PackMan.Entities;
using System.Linq;

namespace PinkGhostBehavior
{
    class PinkGhostBehave : GhostBehavior
    {
        private const int xTargetPoint = 1;
        private const int yTargetPoint = 1;
        protected override void BestWay()
        {
            Tuple<int, int> min = _freeCells[0];
            int x = 0;
            int y = 0;
            Conditions currentCondition = (Conditions)Enum.Parse(typeof(Conditions), _owner.Condition);
            switch (currentCondition)
            {
                case Conditions.Flee:
                    foreach (Tuple<int, int> t in _freeCells)
                    {
                        double minimal = GetDistance(min.Item1, min.Item2, xTargetPoint, yTargetPoint);
                        double current = GetDistance(t.Item1, t.Item2, xTargetPoint, yTargetPoint);
                        if (current < minimal)
                            min = t;
                    }
                    break;
                case Conditions.Normal:
                    foreach (Tuple<int, int> t in _freeCells)
                    {
                        switch (_owner.Level.Pacman.Direction)
                        {
                            case 2:
                                x = _owner.Level.Pacman.X + 4;
                                y = _owner.Level.Pacman.Y;
                                break;
                            case 3:
                                x = _owner.Level.Pacman.X;
                                y = _owner.Level.Pacman.Y + 4;
                                break;
                            case 4:
                                x = _owner.Level.Pacman.X - 4;
                                y = _owner.Level.Pacman.Y;
                                break;
                            default:
                                x = _owner.Level.Pacman.X - 4;
                                y = _owner.Level.Pacman.Y - 4;
                                break;
                        }
                        double minimal = GetDistance(min.Item1, min.Item2, x, y);
                        double current = GetDistance(t.Item1, t.Item2, x, y);
                        if (current < minimal)
                            min = t;
                    }
                    break;
            }
            Push(min.Item1, min.Item2);
        }
    }
}
