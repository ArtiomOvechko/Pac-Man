using System;
using Ghost;

namespace OrangeGhostBehavior.Behavior
{
    public class OrangeGhostBehave: BaseGhostBehavior
    {
        private const int XTarget = 0;

        private const int YTarget = 32;

        private const int DistanceOfTargetChange = 9;

        protected override void BestWay()
        {
            Tuple<int, int> min = FreeCells[0];
            Conditions currentCondition = (Conditions)Enum.
                Parse(typeof(Conditions), Owner.Condition);
            switch (currentCondition)
            {
                case Conditions.Flee:
                    foreach (Tuple<int, int> t in FreeCells)
                    {
                        double minimal = GetDistance
                            (min.Item1, min.Item2, XTarget, YTarget);
                        double current = GetDistance
                            (t.Item1, t.Item2, XTarget, YTarget);
                        if (current < minimal)
                            min = t;
                    }
                    break;
                case Conditions.Normal:
                    if (GetDistance(Owner.X, Owner.Y, Owner.Level.Pacman.X, Owner.Level.Pacman.Y) <
                        DistanceOfTargetChange)
                    {
                        foreach (Tuple<int, int> t in FreeCells)
                        {
                            double minimal = GetDistance
                                (min.Item1, min.Item2, Owner.Level.Pacman.X, Owner.Level.Pacman.Y);
                            double current = GetDistance
                                (t.Item1, t.Item2, Owner.Level.Pacman.X, Owner.Level.Pacman.Y);
                            if (current < minimal)
                                min = t;
                        }
                    }
                    else
                    {
                        foreach (Tuple<int, int> t in FreeCells)
                        {
                            double minimal = GetDistance
                                (min.Item1, min.Item2, XTarget, YTarget);
                            double current = GetDistance
                                (t.Item1, t.Item2, XTarget, YTarget);
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
