using System;

namespace PackMan.Core
{
    public sealed class SingleRandom
    {
        private static volatile SingleRandom _instance;
        private static object locker = new object(); 

        public readonly Random _randomInstance;

        private SingleRandom()
        {
            _randomInstance = new Random();
        }

        public static SingleRandom Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
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
