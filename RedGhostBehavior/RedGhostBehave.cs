using System;
using System.Collections.Generic;
using System.Linq;
using Ghost;
using PackMan.Entities;

namespace RedGhostBehavior
{
    public class RedGhostBehave : GhostBehavior
    {
        private const int xTargetPoint = 32;
        private const int yTargetPoint = 0;
        protected override void BestWay()
        {
            Tuple<int, int> min = _freeCells[0];
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
                        double minimal = GetDistance(min.Item1, min.Item2, _owner.Level.Pacman.X, _owner.Level.Pacman.Y);
                        double current = GetDistance(t.Item1, t.Item2, _owner.Level.Pacman.X, _owner.Level.Pacman.Y);
                        if (current < minimal)
                            min = t;
                    }
                    break;
            }
            Push(min.Item1, min.Item2);
        }
    }
}
