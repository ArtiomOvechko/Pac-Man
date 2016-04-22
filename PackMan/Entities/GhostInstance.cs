using PackMan.Abstract;
using PackMan.Interfaces;

namespace PackMan.Entities
{
    public class GhostInstance : IGhost
    {
        private int _x;

        private int _y;

        private int _oldx;

        private int _oldy;

        private string _condition;

        private readonly ILevel _level;

        private readonly BaseGhostBehavior _behavior;
        

        public GhostInstance(BaseGhostBehavior behaviour, ILevel level)
        {
            _condition = "Normal";
            _level = level;
            _behavior = behaviour;
        }

        public int Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }

        public int X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }
        }

        public int OldX
        {
            get
            {
                return _oldx;
            }

            set
            {
                _oldx = value;
            }
        }

        public int OldY
        {
            get
            {
                return _oldy;
            }

            set
            {
                _oldy = value;
            }
        }

        public ILevel Level
        {
            get
            {
                return _level;
            }
        }

        public string Condition
        {
            get
            {
                return _condition;
            }

            set
            {
                _condition = value;
            }
        }

        public void Move()
        {
            _behavior.Move(this);
        }

        public void PutOn(int x, int y)
        {
            _x = x;
            _y = y;
            _oldx = x;
            _oldy = y;
        }
    }
}
