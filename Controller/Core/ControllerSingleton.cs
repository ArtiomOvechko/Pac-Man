using System.Windows.Input;

namespace Controller.Core
{
    public class ControllerSingleton
    {
        private static ControllerSingleton _instance;

        private static object _locker = new object();

        private enum Ghosts
        {
            Blinky,
            Pinky,
            Inky,
            Clyde
        }

        private ICommand _resetScore;

        private ICommand _showScore;

        private ICommand _beginGame;

        private ICommand _moveUp;

        private ICommand _moveDown;

        private ICommand _moveRight;

        private ICommand _moveLeft;

        private ICommand _stop;

        private ICommand _loadPlugins;

        private ICommand _setBlinkyPlugin;

        private ICommand _setPinkyPlugin;

        private ICommand _setInkyPlugin;

        private ICommand _setClydePlugin;

        /// <summary>
        /// Game core controller
        /// </summary>
        public readonly CoreController Controller;

        public ControllerSingleton()
        {
            Controller = new CoreController();
            _instance = this;
        }

        public static ControllerSingleton GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new ControllerSingleton();
                        }
                    }
                }
                return _instance;
            }
        }

        public ICommand ResetScore
        {
            get
            {
                return _resetScore
                       ?? (_resetScore = new ActionCommand(() =>
                       {
                           GetInstance.Controller.StopGameProcess();
                           GetInstance.Controller.Records.DeleteRecords();
                           GetInstance.Controller.NotifyDbObservers();
                       }));
            }
        }

        public ICommand ShowScore
        {
            get
            {
                return _showScore
                       ?? (_showScore = new ActionCommand(() =>
                       {
                           GetInstance.Controller.StopGameProcess();
                           GetInstance.Controller.NotifyDbObservers();
                       }));
            }
        }

        public ICommand BeginGame
        {
            get
            {
                return _beginGame
                       ?? (_beginGame = new ActionCommand(() =>
                       {
                           GetInstance.Controller.NewGame();
                           GetInstance.Controller.NotifyLevelChangeObservers();;
                       }));
            }
        }

        public ICommand MoveUp
        {
            get
            {
                return _moveUp
                       ?? (_moveUp = new ActionCommand(() =>
                       {
                           GetInstance.Controller.Move(true);
                           GetInstance.Controller.MoveUp();
                       }));
            }
        }

        public ICommand MoveDown
        {
            get
            {
                return _moveDown
                       ?? (_moveDown = new ActionCommand(() =>
                       {
                           GetInstance.Controller.Move(true);
                           GetInstance.Controller.MoveDown();
                       }));
            }
        }

        public ICommand MoveRight
        {
            get
            {
                return _moveRight
                       ?? (_moveRight = new ActionCommand(() =>
                       {
                           GetInstance.Controller.Move(true);
                           GetInstance.Controller.MoveRight();
                       }));
            }
        }

        public ICommand MoveLeft
        {
            get
            {
                return _moveLeft
                       ?? (_moveLeft = new ActionCommand(() =>
                       {
                           GetInstance.Controller.Move(true);
                           GetInstance.Controller.MoveLeft();
                       }));
            }
        }

        public ICommand Stop
        {
            get
            {
                return _stop
                       ?? (_stop = new ActionCommand(() =>
                       {
                           GetInstance.Controller.Move(false);
                       }));
            }
        }


        public ICommand LoadPlugins
        {
            get
            {
                return _loadPlugins
                       ?? (_loadPlugins = new ActionCommand(() =>
                       {
                           GetInstance.Controller.NotifyPluginsObservers();
                       }));
            }
        }

        public ICommand SetBlinkyPlugin
        {
            get
            {
                return _setBlinkyPlugin
                       ?? (_setBlinkyPlugin = new RelayCommand((object o)=>
                       {
                           GetInstance.Controller.SetBehavior((int)Ghosts.Blinky, o.ToString());
                       }));
            }
        }

        public ICommand SetPinkyPlugin
        {
            get
            {
                return _setPinkyPlugin
                       ?? (_setPinkyPlugin = new RelayCommand((object o) =>
                       {
                           GetInstance.Controller.SetBehavior((int)Ghosts.Pinky, o.ToString());
                       }));
            }
        }

        public ICommand SetInkyPlugin
        {
            get
            {
                return _setInkyPlugin
                       ?? (_setInkyPlugin = new RelayCommand((object o) =>
                       {
                           GetInstance.Controller.SetBehavior((int)Ghosts.Inky, o.ToString());
                       }));
            }
        }

        public ICommand SetClydePlugin
        {
            get
            {
                return _setClydePlugin
                       ?? (_setClydePlugin = new RelayCommand((object o) =>
                       {
                           GetInstance.Controller.SetBehavior((int)Ghosts.Clyde, o.ToString());
                       }));
            }
        }
    }
}
