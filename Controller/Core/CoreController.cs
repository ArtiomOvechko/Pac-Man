﻿using System;
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

using Controller.Interfaces;

namespace Controller.Core
{
    public class CoreController: ICoreController, IPacManObservable
    {
        // Ghost model state updater
        private readonly DispatcherTimer _parallelTicker = new DispatcherTimer();

        // General model state updater
        private readonly DispatcherTimer _ticker = new DispatcherTimer();

        private IPlayer _player;

        private List<BaseGhostBehavior> _behaviors;

        private List<IDbObserver> _dbObservers;

        private List<IMovingObserver> _movingObservers;

        private List<ILevelChangeObserver> _levelChangeObservers;

        private List<IPluginsObserver> _pluginsObservers; 

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

        private RecordsDatabase _records;

        /// <summary>
        /// Get database instance
        /// </summary>
        public IRecordsDatabase Records
        {
            get { return _records; }
        }

        public CoreController()
        {
            // Initialization of observers collection
            _dbObservers = new List<IDbObserver>();
            _movingObservers = new List<IMovingObserver>();
            _levelChangeObservers = new List<ILevelChangeObserver>();
            _pluginsObservers = new List<IPluginsObserver>();

            // Initialization of relative path to plugins folder
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
            if (_player.Level.Pacman.Moving)
            {
                NotifyMovingObservers();
            }

            // Check whether player has completed the level
            if (_player.Level.GameField.Completed())
            {
                NextLevel();
                _player.Score += LevelScore;
                _player.ScoreTrack += LevelScore;
            }

            //Check whether player collided ghosts
            _player.CheckCondition();
            
            //Check whether player has died
            if (_player.Lives == CriticalValue)
            {
                Records.AddRecord(GetPlayer);
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
            NotifyMovingObservers();
        }

        /// <summary>
        /// Go to the nest level
        /// </summary>
        public void NextLevel()
        {
            StopGameProcess();
            InitLevel();
            StartGameProcess();
            NotifyLevelChangeObservers();
        }

        /// <summary>
        /// Begin new game
        /// </summary>
        public void NewGame()
        {
            InitFirstLevel();
            StartGameProcess();
            NotifyLevelChangeObservers();
        }

        public void MoveUp()
        {
            _player.Level.Pacman.Direction =
                (int)Direction.North;
        }

        public void MoveDown()
        {
            _player.Level.Pacman.Direction =
                (int)Direction.South;
        }

        public void MoveRight()
        {
            _player.Level.Pacman.Direction =
                (int)Direction.East;
        }

        public void MoveLeft()
        {
            _player.Level.Pacman.Direction =
                (int)Direction.West;
        }

        public void Move(bool condition)
        {
            if(GetPlayer!=null)
                GetPlayer.Level.Pacman.Moving = condition;
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

        public void SetBehavior(int behaviorIndex, string path)
        {
            var t = typeof (BaseGhostBehavior);
            var type = Assembly.LoadFrom(path).GetTypes().
                    FirstOrDefault(a => t.IsAssignableFrom(a) && !a.IsAbstract);
            _behaviors[behaviorIndex] = (BaseGhostBehavior)Activator.CreateInstance(type);
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

        public DataTable SelectRecords()
        {
            return Records.SelectRecords();
        }

        public void RegisterDbObserver(IDbObserver observer)
        {
            _dbObservers.Add(observer);
        }    

        public void RegisterLevelChangeObserver(ILevelChangeObserver observer)
        {
            _levelChangeObservers.Add(observer);
        }

        public void RegisterMovingObserver(IMovingObserver observer)
        {
            _movingObservers.Add(observer);
        }

        public void RegisterPluginsObserver(IPluginsObserver observer)
        {
            _pluginsObservers.Add(observer);
        }

        public void RemoveDbObserver(IDbObserver observer)
        {
            _dbObservers.Remove(observer);
        }

        public void RemoveLevelChangeObserver(ILevelChangeObserver observer)
        {
            _levelChangeObservers.Remove(observer);
        }

        public void RemoveMovingObservers(IMovingObserver observer)
        {
            _movingObservers.Remove(observer);
        }

        public void RemovePluginsObservers(IPluginsObserver observer)
        {
            _pluginsObservers.Remove(observer);
        }

        public void NotifyDbObservers()
        {
            foreach (IDbObserver observer in _dbObservers)
            {
                observer.Update(Records);
            }
        }

        public void NotifyMovingObservers()
        {
            foreach (IMovingObserver observer in _movingObservers)
            {
                observer.Update(GetPlayer);
            }
        }

        public void NotifyLevelChangeObservers()
        {
            foreach (ILevelChangeObserver observer in _levelChangeObservers)
            {
                observer.Update(GetPlayer);
            }
        }

        public void NotifyPluginsObservers()
        {
            foreach (IPluginsObserver observer in _pluginsObservers)
            {
                observer.Update(GetLibraries);
            }
        }
    }
}