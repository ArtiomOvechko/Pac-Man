using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ghost;
using PackMan.Entities;

namespace OrangeGhostBehavior
{
    public class OrangeGhostBehave: GhostBehavior
    {
        private const int _xTarget = 0;
        private const int _yTarget = 32;
        private const int _distanceOfTargetChange = 9;

        protected override void BestWay()
        {
            Tuple<int, int> min = _freeCells[0];
            Conditions currentCondition = (Conditions)Enum.Parse(typeof(Conditions), _owner.Condition);
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
                    if (GetDistance(_owner.X, _owner.Y, _owner.Level.Pacman.X, _owner.Level.Pacman.Y) < _distanceOfTargetChange)
                    {
                        foreach (Tuple<int, int> t in _freeCells)
                        {
                            double minimal = GetDistance(min.Item1, min.Item2, _owner.Level.Pacman.X, _owner.Level.Pacman.Y);
                            double current = GetDistance(t.Item1, t.Item2, _owner.Level.Pacman.X, _owner.Level.Pacman.Y);
                            if (current < minimal)
                                min = t;
                        }
                    }
                    else
                    {
                        foreach (Tuple<int, int> t in _freeCells)
                        {
                            double minimal = GetDistance(min.Item1, min.Item2, _xTarget, _yTarget);
                            double current = GetDistance(t.Item1, t.Item2, _xTarget, _yTarget);
                            if (current < minimal)
                                min = t;
                        }
                    }
                    break;
            }
            Push(min.Item1, min.Item2);
        }
    }
}
