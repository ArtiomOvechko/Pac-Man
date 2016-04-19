using System;
using Ghost;

namespace RedGhostBehavior.Behavior
{
    public class RedGhostBehave : BaseGhostBehavior
    {
        private const int XTargetPoint = 32;
        private const int YTargetPoint = 0;

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
                            (min.Item1, min.Item2, XTargetPoint, YTargetPoint);
                        double current = GetDistance
                            (t.Item1, t.Item2, XTargetPoint, YTargetPoint);
                        if (current < minimal)
                            min = t;
                    }
                    break;
                case Conditions.Normal:
                    foreach (Tuple<int, int> t in FreeCells)
                    {
                        double minimal = GetDistance
                            (min.Item1, min.Item2, Owner.Level.Pacman.X, Owner.Level.Pacman.Y);
                        double current = GetDistance
                            (t.Item1, t.Item2, Owner.Level.Pacman.X, Owner.Level.Pacman.Y);
                        if (current < minimal)
                            min = t;
                    }
                    break;
            }

            Push(min.Item1, min.Item2);
        }
    }
}
