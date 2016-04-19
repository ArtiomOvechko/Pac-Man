using System;
using Ghost;

namespace PinkGhostBehavior.Behavior
{
    class PinkGhostBehave : BaseGhostBehavior
    {
        private const int XTargetPoint = 1;
        private const int YTargetPoint = 1;
        protected override void BestWay()
        {
            Tuple<int, int> min = FreeCells[0];
            int x;
            int y;
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
                        switch (Owner.Level.Pacman.Direction)
                        {
                            case 2:
                                x = Owner.Level.Pacman.X + 4;
                                y = Owner.Level.Pacman.Y;
                                break;
                            case 3:
                                x = Owner.Level.Pacman.X;
                                y = Owner.Level.Pacman.Y + 4;
                                break;
                            case 4:
                                x = Owner.Level.Pacman.X - 4;
                                y = Owner.Level.Pacman.Y;
                                break;
                            default:
                                x = Owner.Level.Pacman.X - 4;
                                y = Owner.Level.Pacman.Y - 4;
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
