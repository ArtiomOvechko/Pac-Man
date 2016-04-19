using PackMan.Interfaces;

namespace PackMan.Core
{
    public class Player : IPlayer
    {
        private  int _levelNumber;

        private  int _score;

        private int _scoreTrack;

        private  int _lives;

        private ILevel _level;

        private const int FleeTimeExpired = 0;

        private const int ScoreAmountOfNewLive = 10000;

        public ILevel Level
        {
            get { return _level; }

            set { _level = value; }
        }

        public int LevelNumber
        {
            get { return _levelNumber; }

            set { _levelNumber = value; }
        }

        public int Lives
        {
            get { return _lives; }

            set { _lives = value; }
        }

        public int Score
        {
            get { return _score; }

            set { _score = value; }
        }

        public int ScoreTrack
        {
            get { return _scoreTrack; }

            set { _scoreTrack = value; }
        }

        public Player(ILevel level)
        {
            Level = level;
            Score = 0;
            ScoreTrack = 0;
            Lives = 3;
            LevelNumber = 1;
        }

        public void CheckCondition()
        {
            if (Level.Blinky.X == Level.Pacman.X && Level.Blinky.Y == Level.Pacman.Y)
            {
                if (Level.FleeTime > FleeTimeExpired)
                {
                    Level.Blinky.PutOn(Level.GameField.Width / 2 - 1, Level.GameField.Height / 2 - 3);
                    return;
                }
                Level.PutOnDefault();
                Lives --;
            }
            if (Level.Pinky.X == Level.Pacman.X && Level.Pinky.Y == Level.Pacman.Y)
            {
                if (Level.FleeTime > FleeTimeExpired)
                {
                    Level.Pinky.PutOn(Level.GameField.Width / 2 - 2, Level.GameField.Height / 2 - 1);
                    return;
                }
                Level.PutOnDefault();
                Lives--;
            }
            if (Level.Inky.X == Level.Pacman.X && Level.Inky.Y == Level.Pacman.Y)
            {
                if (Level.FleeTime > FleeTimeExpired)
                {
                    Level.Inky.PutOn(Level.GameField.Width / 2 - 1, Level.GameField.Height / 2 - 1);
                    return;
                }
                Level.PutOnDefault();
                Lives--;
            }
            if (Level.Clyde.X == Level.Pacman.X && Level.Clyde.Y == Level.Pacman.Y)
            {
                if (Level.FleeTime > FleeTimeExpired)
                {
                    Level.Clyde.PutOn(Level.GameField.Width / 2, Level.GameField.Height / 2 - 1);
                    return;
                }
                Level.PutOnDefault();
                Lives--;
            }
            if (ScoreTrack > ScoreAmountOfNewLive)
            {
                ScoreTrack -= ScoreAmountOfNewLive;
                Lives++;
            }
        }
    }
}
