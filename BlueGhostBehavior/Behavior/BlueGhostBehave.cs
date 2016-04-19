using System;
using Ghost;

namespace BlueGhostBehavior.Behavior
{
    public class BlueGhostBehave: BaseGhostBehavior
    {
        private const int XTarget = 32;

        private const int YTarget = 32;

        private Tuple<int, int> GetPoints(int x1, int y1)
        {
            int x;
            int y;
            x = 2 * x1 + Owner.Level.Blinky.X;
            y = 2 * y1 + Owner.Level.Blinky.Y;
            return new Tuple<int, int>(x,y);
        }

        protected override void BestWay()
        {
            Tuple<int, int> min = FreeCells[0];
            Tuple<int, int> point = new Tuple<int, int>(0,0);
            Conditions currentCondition = (Conditions)Enum.
                Parse(typeof (Conditions), Owner.Condition);
            switch (currentCondition)
            {
                case Conditions.Flee:
                    foreach (Tuple<int, int> t in FreeCells)
                    {
                        var minimal = 
                            GetDistance(min.Item1, min.Item2, XTarget, YTarget);
                        var current = 
                            GetDistance(t.Item1, t.Item2, XTarget, YTarget);
                        if (current < minimal)
                            min = t;
                    }
                    break;
                case Conditions.Normal:
                    foreach (Tuple<int, int> t in FreeCells)
                    {
                        switch (Owner.Level.Pacman.Direction)
                        {
                            case (int)Directions.North:
                                point = GetPoints
                                    (Owner.Level.Pacman.X, Owner.Level.Pacman.Y - 2);
                                break;
                            case (int)Directions.East:
                                point = GetPoints
                                    (Owner.Level.Pacman.X + 2, Owner.Level.Pacman.Y ); 
                                break;
                            case (int)Directions.West:
                                point = GetPoints
                                    (Owner.Level.Pacman.X, Owner.Level.Pacman.Y+2);
                                break;
                            case (int)Directions.South:
                                point = GetPoints
                                    (Owner.Level.Pacman.X - 2, Owner.Level.Pacman.Y);
                                break;
                        }
                        double minimal = GetDistance
                            (min.Item1, min.Item2, point.Item1, point.Item2);
                        double current = GetDistance
                            (t.Item1, t.Item2, point.Item1, point.Item2);
                        if (current < minimal)
                            min = t;
                    }
                    break;
            }
            Push(min.Item1, min.Item2);
        }
    }
}
