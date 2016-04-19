using PackMan.Interfaces;

namespace PackMan.Entities
{
    public class Pac: IPac
    {
        private int _x;

        private int _y;

        private int _direction;

        private readonly IField _field;

        private bool _moving;

        private readonly ILevel _level;

        private const int InitialValue = 0;

        private const int SmallScoreBonus = 100;

        private const int NormalScoreBonus = 500;

        private IObstacle[,] GameField
        {
            get
            {
                return _field.GameField;
            }
        }


        public Pac(IField field, ILevel level)
        {
            _x = field.Width / 2 - 1;
            _y = field.Height / 2 + 8;
            _direction = 2;
            _field = field;
            _level = level;
        }

        public int X
        {
            get
            {
                return _x;
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }
        }

        public int Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        public bool Moving
        {
            get
            {
                return _moving;
            }

            set
            {
                _moving = value;
            }
        }

        public ILevel Level
        {
            get { return _level; }
        }

        public void  Move()
        {
            switch (Moving)
            {
                case true:
                    switch (Direction)
                    {
                        case 1:
                            MoveUp();
                            break;
                        case 2:
                            MoveRight();
                            break;
                        case 3:
                            MoveDown();
                            break;
                        case 4:
                            MoveLeft();
                            break;
                    }
                    break;
            }
        }

        private void MoveUp()
        {
            if ((GameField[_y - 1, _x] is Wall) == false)
            {
                EatPoint(_y - 1, _x);
                _y -= 1;
            }
        }

        private void MoveDown()
        {
            if ((GameField[_y + 1, _x] is Wall) == false)
            {
                EatPoint(_y + 1, _x);
                _y += 1;
            }
        }

        private void MoveLeft()
        {
            if(_x!=InitialValue || _y!=Level.GameField.Height/2-1)
            { 
                if ((GameField[_y, _x - 1] is Wall) == false)
                {
                    EatPoint(_y, _x - 1);
                    _x -= 1;
                }
            }
            else
            {
                _x = 31;
            }
        }

        private void MoveRight()
        {
            if (_x != Level.GameField.Width-1 || _y != Level.GameField.Height / 2 - 1)
            {
                if ((GameField[_y, _x + 1] is Wall) == false)
                {
                    EatPoint(_y, _x + 1);
                    _x += 1;
                }
            }
            else
            {
                _x = 0;
            }
        }

        public void EatPoint(int a, int b)
        {
            if ((GameField[a, b] as Dot) != null)
            {
                GameField[a, b] = new Empty();
                Level.Player.Score++;
                Level.Player.ScoreTrack++;
            }
            if ((GameField[a,b] as Cherry) != null)
            {
                int amountOfScore = NormalScoreBonus;
                GameField[a, b] = new Empty();
                Level.Player.Score+= amountOfScore;
                Level.Player.ScoreTrack+= amountOfScore;
            }
            if ((GameField[a, b] as Bonus) != null)
            {
                int amountOfScore = SmallScoreBonus;
                GameField[a, b] = new Empty();
                Level.Player.Score += amountOfScore;
                Level.Player.ScoreTrack += amountOfScore;
                Level.SetFlee();
            }
        }

        public void PutOnDefault()
        {
            _x = Level.GameField.Width / 2 - 1;
            _y = Level.GameField.Height / 2 + 8;
        }
    }
}
