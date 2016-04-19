using System;

namespace PackMan.Core
{
    public sealed class SingleRandom
    {
        private static volatile SingleRandom _instance;

        private static object _locker = new object(); 

        public readonly Random RandomInstance;

        private SingleRandom()
        {
            RandomInstance = new Random();
        }

        public static SingleRandom Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if(_instance==null)
                            _instance = new SingleRandom();
                    }
                }
                return _instance;
            }
        }
    }
}
