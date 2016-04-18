using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Threading;
using Ghost;
using PackMan.Core;
using PackMan.Interfaces;
using RecordsDb.Core;
using RecordsDb.Interfaces;
using System.Diagnostics;
using Controller.Interfaces;

namespace Controller.Core
{
    public class CoreController: ICoreController
    {
        private readonly DispatcherTimer _stepParallelTicker = new DispatcherTimer();
        private readonly DispatcherTimer _stepTicker = new DispatcherTimer();
        private IPlayer _player;
        private List<GhostBehavior> _behaviors;
        private readonly string _pathPlug;
        private const int _cellSize = 20;
        private const int _levelScore = 1000;
        private const int _criticalValue = 0;
        private const int _fieldSize = 32;
        private const int _pacManFrameDuration = 750000;
        private const int _ghostFrameDuration = 2500000;
        private const int _northDirection = 1;
        private const int _eastDirection = 2;
        private const int _southDirection = 3;
        private const int _westDirection = 4;
        private enum ghostType { blinkyAs, pinkyAs, inkyAs, clydeAs }
        private IRecordsDatabase _records;

        public CoreController()
        {
            _pathPlug = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Plugins";
            _records = new RecordsDatabase();
            InitBehaviors();
            AddEventHandlers();
        }

        public IPlayer GetPlayer
        {
            get { return _player; }
        }

        private void AddEventHandlers()
        {
            _stepTicker.Tick += StepTicker_Tick;
            _stepParallelTicker.Tick += StepParallelTicker_Tick;
        }

        private void InitBehaviors()
        {
            _behaviors = new List<GhostBehavior>(new GhostBehavior[4]);
        }

        private void InitFirstLevel()
        {
            var f = new Field(new Filler());
            _player = new Player(new Level(f, _behaviors[0], _behaviors[1], _behaviors[2], _behaviors[3]));
            _player.Level.Player = _player;
        }

        private void InitLevel()
        {
            var f = new Field(new Filler());
            _player.Level = new Level(f, _behaviors[0], _behaviors[1], _behaviors[2], _behaviors[3]);
            _player.Level.Player = _player;
            _player.LevelNumber++;
        }

        //Core process of gameplay
        //
        //
        private void StepTicker_Tick(object sender, EventArgs e)
        {
            _player.Level.Pacman.Move();
            if (_player.Level.GameField.Completed())
            {
                NextLevel();
                _player.Score += _levelScore;
                _player.ScoreTrack += _levelScore;
            }
            _player.CheckCondition();
            if (_player.Lives == _criticalValue)
            {
                _records.AddRecord(GetPlayer);
                NewGame();
            }
        }

        private void StepParallelTicker_Tick(object sender, EventArgs e)
        {
            _player.Level.Blinky.Move();
            _player.Level.Pinky.Move();
            _player.Level.Inky.Move();
            _player.Level.Clyde.Move();
            if (_player.Level.FleeTime > _criticalValue)
                _player.Level.FleeTime--;
            else
                _player.Level.SetNormal();
        }

        public void NextLevel()
        {
            StopGameProcess();
            InitLevel();
            StartGameProcess();
        }

        public void NewGame()
        {
            InitFirstLevel();
            StartGameProcess();
        }

        public void PressAction(KeyEventArgs e)
        {
            if (_player != null)
            {
                if (e.Key == Key.Up)
                {
                    _player.Level.Pacman.Moving = true;
                    _player.Level.Pacman.Direction = _northDirection;
                    e.Handled = true;
                }
                if (e.Key == Key.Down)
                {
                    _player.Level.Pacman.Moving = true;
                    _player.Level.Pacman.Direction = _southDirection;
                    e.Handled = true;
                }
                if (e.Key == Key.Right)
                {
                    _player.Level.Pacman.Moving = true;
                    _player.Level.Pacman.Direction = _eastDirection;
                    e.Handled = true;
                }
                if (e.Key == Key.Left)
                {
                    _player.Level.Pacman.Moving = true;
                    _player.Level.Pacman.Direction = _westDirection;
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
            var t = typeof(GhostBehavior);
            if (path != null)
            {
                var type = Assembly.LoadFrom(path).GetTypes().FirstOrDefault(a => t.IsAssignableFrom(a) && !a.IsAbstract);
                if (type != null)
                {
                    ghostType ghostSelected = (ghostType) Enum.Parse(typeof (ghostType), ghostName);
                    switch (ghostSelected)
                    {
                        case ghostType.blinkyAs:
                            _behaviors[0] = (GhostBehavior) Activator.CreateInstance(type);
                            break;
                        case ghostType.pinkyAs:
                            _behaviors[1] = (GhostBehavior) Activator.CreateInstance(type);
                            break;
                        case ghostType.inkyAs:
                            _behaviors[2] = (GhostBehavior) Activator.CreateInstance(type);
                            break;
                        case ghostType.clydeAs:
                            _behaviors[3] = (GhostBehavior) Activator.CreateInstance(type);
                            break;
                    }
                    return;
                }
            }
            throw new ArgumentException();
        }

        private void StartGameProcess()
        {
            _stepTicker.Interval = new TimeSpan(_pacManFrameDuration);
            _stepParallelTicker.Interval = new TimeSpan(_ghostFrameDuration);
            _stepTicker.Start();
            _stepParallelTicker.Start();
        }

        public void StopGameProcess()
        {
            _stepTicker.Stop();
            _stepParallelTicker.Stop();
        }

        public void ResetScore()
        {
            _records.DeleteRecords();
        }

        public DataTable SelectRecord()
        {
            return _records.SelectRecords();
        }
    }
}