using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Threading;

using Ghost;
using PackMan.Core;
using PackMan.Interfaces;
using RecordsDb.Core;
using RecordsDb.Interface;

using Controller.Interface;

namespace Controller.Core
{
    public class CoreController: ICoreController
    {
        // Ghost model state updater
        private readonly DispatcherTimer _parallelTicker = new DispatcherTimer();

        // General model state updater
        private readonly DispatcherTimer _ticker = new DispatcherTimer();

        private IPlayer _player;

        private List<BaseGhostBehavior> _behaviors;

        private readonly string _pathPlug;

        private const int LevelScore = 1000;

        private const int CriticalValue = 0;

        // Higher value reduse game speed
        private const int PacManFrameDuration = 750000;
        
        // Higher value reduse game speed
        private const int GhostFrameDuration = 2500000;

        private enum  Direction
        {
            North = 1,
            East = 2,
            South = 3,
            West = 4
        }

        // Use it to specify which ghost was selected when applying plugin
        private enum GhostType { BlinkyAs, PinkyAs, InkyAs, ClydeAs } 

        private IRecordsDatabase _records;

        private ICommand _resetScore;

        public CoreController()
        {
            // Getting relative path to plugins folder
            _pathPlug = Path.GetDirectoryName(Process.
                GetCurrentProcess().MainModule.FileName) + @"\Plugins";
            // Initializing top score database instance
            _records = new RecordsDatabase();
            _behaviors = new List<BaseGhostBehavior>(new BaseGhostBehavior[4]);
            AddEventHandlers();
        }

        public IPlayer GetPlayer
        {
            get { return _player; }
        }

        private void AddEventHandlers()
        {
            _ticker.Tick += StepTicker_Tick;
            _parallelTicker.Tick += StepParallelTicker_Tick;
        }

        /// <summary>
        /// Creates new game field and player
        /// </summary>
        private void InitFirstLevel()
        {
            var f = new Field(new Filler());
            _player = new Player(new Level
                (f, _behaviors[0], _behaviors[1], _behaviors[2], _behaviors[3]));
            _player.Level.Player = _player;
        }

        /// <summary>
        /// Creates new level for existing player
        /// </summary>
        private void InitLevel()
        {           
            var f = new Field(new Filler());
            _player.Level = new Level
                (f, _behaviors[0], _behaviors[1], _behaviors[2], _behaviors[3])
                { Player = _player};
            _player.LevelNumber++;
        }

        // Core process of gameplay
        private void StepTicker_Tick(object sender, EventArgs e)
        {
            _player.Level.Pacman.Move();

            // Check whether player has completed the level
            if (_player.Level.GameField.Completed())
            {
                NextLevel();
                _player.Score += LevelScore;
                _player.ScoreTrack += LevelScore;
            }

            //Check whether player collided ghosts and act
            _player.CheckCondition();
            
            //Check whether player has died
            if (_player.Lives == CriticalValue)
            {
                _records.AddRecord(GetPlayer);
                NewGame();
            }
        }

        private void StepParallelTicker_Tick(object sender, EventArgs e)
        {
            //Move all ghosts
            _player.Level.Blinky.Move();
            _player.Level.Pinky.Move();
            _player.Level.Inky.Move();
            _player.Level.Clyde.Move();
            if (_player.Level.FleeTime > CriticalValue)
                _player.Level.FleeTime--;
            else
                _player.Level.SetNormal();
        }

        /// <summary>
        /// Go to the nest level
        /// </summary>
        public void NextLevel()
        {
            StopGameProcess();
            InitLevel();
            StartGameProcess();
        }

        /// <summary>
        /// Begin new game
        /// </summary>
        public void NewGame()
        {
            InitFirstLevel();
            StartGameProcess();
        }

        /// <summary>
        /// Handle key input and choose direction of pacman to move
        /// </summary>
        /// <param name="e"></param>
        public void PressAction(KeyEventArgs e)
        {
            if (_player != null)
            {
                if (e.Key == Key.Up)
                {
                    _player.Level.Pacman.Moving = true;
                    _player.Level.Pacman.Direction = 
                        (int)Direction.North;
                    e.Handled = true;
                }
                if (e.Key == Key.Down)
                {
                    _player.Level.Pacman.Moving = true;
                    _player.Level.Pacman.Direction = 
                        (int)Direction.South;
                    e.Handled = true;
                }
                if (e.Key == Key.Right)
                {
                    _player.Level.Pacman.Moving = true;
                    _player.Level.Pacman.Direction = 
                        (int)Direction.East;
                    e.Handled = true;
                }
                if (e.Key == Key.Left)
                {
                    _player.Level.Pacman.Moving = true;
                    _player.Level.Pacman.Direction = 
                        (int)Direction.West;
                    e.Handled = true;
                }
            }
        }

        public void ReleaseAction()
        {
            if (_player != null)
                _player.Level.Pacman.Moving = false;
        }

        public List<string> GetLibraries
        {
            get
            {
                var data = new List<string>();
                data.AddRange(Directory.EnumerateFiles(_pathPlug, "*.dll"));
                return data;
            }
        }

        public void SetBehavior(string ghostName, string path)
        {
            var t = typeof(BaseGhostBehavior);
            if (path != null)
            {
                var type = Assembly.LoadFrom(path).GetTypes().
                    FirstOrDefault(a => t.IsAssignableFrom(a) && !a.IsAbstract);
                if (type != null)
                {
                    GhostType ghostSelected = (GhostType)Enum.
                        Parse(typeof (GhostType), ghostName);
                    switch (ghostSelected)
                    {
                        case GhostType.BlinkyAs:
                            _behaviors[0] = (BaseGhostBehavior) 
                                Activator.CreateInstance(type);
                            break;
                        case GhostType.PinkyAs:
                            _behaviors[1] = (BaseGhostBehavior) 
                                Activator.CreateInstance(type);
                            break;
                        case GhostType.InkyAs:
                            _behaviors[2] = (BaseGhostBehavior) 
                                Activator.CreateInstance(type);
                            break;
                        case GhostType.ClydeAs:
                            _behaviors[3] = (BaseGhostBehavior) 
                                Activator.CreateInstance(type);
                            break;
                    }
                    return;
                }
            }
            throw new ArgumentException();
        }

        private void StartGameProcess()
        {
            _ticker.Interval = new TimeSpan(PacManFrameDuration);
            _parallelTicker.Interval = new TimeSpan(GhostFrameDuration);
            _ticker.Start();
            _parallelTicker.Start();
        }

        public void StopGameProcess()
        {
            _ticker.Stop();
            _parallelTicker.Stop();
        }

        public DataTable SelectRecord()
        {
            return _records.SelectRecords();
        }

        public ICommand ResetScore
        {
            get
            {
                return _resetScore
                       ?? (_resetScore = new ActionCommand(() =>
                       {
                          _records.DeleteRecords();
                       }));
            }
        }
    }
}