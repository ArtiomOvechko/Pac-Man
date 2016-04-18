using PackMan.Interfaces;
using PackMan.Entities;
using Ghost;
using System;

namespace PackMan.Core
{
    public class Level: ILevel
    {
        private IField _gameField;
        private IPac _pacman;
        private IGhost _blinky;
        private IGhost _pinky;
        private IGhost _inky;
        private IGhost _clyde;
        private IPlayer _player;
        private int _fleeTime;

        public IField GameField
        {
            get
            {
                return _gameField;
            }

            set
            {
                _gameField = value;
            }
        }

        public IPac Pacman
        {
            get
            {
                return _pacman;
            }

            set
            {
                _pacman = value;
            }
        }

        public IGhost Blinky
        {
            get
            {
                return _blinky;
            }
        }

        public IGhost Pinky
        {
            get
            {
                return _pinky;
            }
        }

        public IGhost Inky
        {
            get
            {
                return _inky;
            }
        }

        public IGhost Clyde
        {
            get
            {
                return _clyde;
            }
        }

        public IPlayer Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public int FleeTime
        {
            get { return _fleeTime; }

            set { _fleeTime = value; }
        }

        public Level()
        {

        }

        public Level(IField field, GhostBehavior blinky, GhostBehavior pinky, GhostBehavior inky, GhostBehavior clyde)
        {
            FleeTime = 0;
            GameField = field;
            Pacman = new Pac(GameField, this);
            _blinky = new GhostInstance(blinky, this);
            _pinky = new GhostInstance(pinky, this);
            _inky = new GhostInstance(inky, this);
            _clyde = new GhostInstance(clyde, this);
            PutOnDefault();
        }

        public void PutOnDefault()
        {
            _blinky.PutOn(GameField.Width / 2 - 1, GameField.Height / 2 - 3);
            _pinky.PutOn(GameField.Width / 2 - 2, GameField.Height / 2 - 1);
            _inky.PutOn(GameField.Width / 2 - 1, GameField.Height / 2 - 1);
            _clyde.PutOn(GameField.Width / 2, GameField.Height / 2 - 1);
            Pacman.PutOnDefault();
        }

        public void SetFlee()
        {
            string condition = "Flee";
            FleeTime += 80;
            Blinky.Condition = condition;
            Pinky.Condition = condition;
            Inky.Condition = condition;
            Clyde.Condition = condition;
        }

        public void SetNormal()
        {
            string condition = "Normal";
            Blinky.Condition = condition;
            Pinky.Condition = condition;
            Inky.Condition = condition;
            Clyde.Condition = condition;
        }
    }
}
