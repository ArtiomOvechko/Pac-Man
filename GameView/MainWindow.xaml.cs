using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GameView.Extensions;
using Ghost;
using PackMan.Core;
using PackMan.Entities;
using PackMan.Interfaces;
using System.Data.SQLite;
using System.Data;
using System.Configuration;

namespace GameView
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DispatcherTimer _stepParallelTicker = new DispatcherTimer();
        private readonly DispatcherTimer _stepTicker = new DispatcherTimer();
        private IPlayer _player;
        private List<GhostBehavior> _behaviors;
        private BitmapImage _dotImg;
        private BitmapImage _emptyImg;
        private BitmapImage _wallImg;
        private BitmapImage _cherryImg;
        private BitmapImage _bonusImg;
        private BitmapImage _inkyImg;
        private BitmapImage _clydeImg;
        private BitmapImage _blinkyImg;
        private BitmapImage _pinkyImg;
        private BitmapImage _fleeImg;
        private Image[,] _imgField;
        private Image _inkyPic;
        private Image _pacPic;
        private Image _pinkyPic;
        private Image _blinkyPic;
        private Image _clydePic;
        private string _pathPic;
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
        private const int _comboDefaultIndex = 0;
        private enum ghostType { blinkyAs, pinkyAs, inkyAs, clydeAs}

        public MainWindow()
        { 
            _pathPlug = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Plugins";
            InitBehaviors();
            InitializeComponent();
            NameGui();
            InitPictures();
            AddEventHandlers();
            DisplayRecords(SelectRecords());
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MessageBox.Show(
                Properties.Resources.GreetingsMessage);
        }

        //Display
        //
        //
        private void DrawField()
        {
            fieldCan.Children.Clear();
            foreach (var t in _player.Level.GameField.GetAllCells())
            {
                var y = t.Item2*_cellSize;
                var x = t.Item3*_cellSize;
                var i = new Image();
                var wall = t.Item1 as Wall;
                if (wall != null)
                {
                    i.Source = _wallImg;
                }
                var empty = t.Item1 as Empty;
                if (empty != null)
                {
                    i.Source = _emptyImg;
                }
                var dot = t.Item1 as Dot;
                if (dot != null)
                {
                    i.Source = _dotImg;
                }
                var cherry = t.Item1 as Cherry;
                if (cherry != null)
                {
                    i.Source = _cherryImg;
                }
                var bonus = t.Item1 as Bonus;
                if (bonus != null)
                {
                    i.Source = _bonusImg;
                }
                Canvas.SetLeft(i, x);
                Canvas.SetTop(i, y);
                _imgField[t.Item2, t.Item3] = i;
                fieldCan.Children.Add(i);
            }
        }

        private void NameGui()
        {
            newGameBtn.Content = Properties.Resources.NewGame;
            topScoreBtn.Content = Properties.Resources.Records;
            reserScoreBtn.Content = Properties.Resources.RecordsClear;
        }

        private void InitPictures()
        {
            _pathPic = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Pictures";
            _wallImg = new BitmapImage();
            _wallImg.BeginInit();
            _wallImg.UriSource = new Uri(_pathPic + "\\wallImg.jpg");
            _wallImg.CacheOption = BitmapCacheOption.OnLoad;
            _wallImg.EndInit();
            _emptyImg = new BitmapImage();
            _emptyImg.BeginInit();
            _emptyImg.UriSource = new Uri(_pathPic + "\\emptyImg.jpg");
            _emptyImg.CacheOption = BitmapCacheOption.OnLoad;
            _emptyImg.EndInit();
            _dotImg = new BitmapImage();
            _dotImg.BeginInit();
            _dotImg.UriSource = new Uri(_pathPic + "\\dotImg.jpg");
            _dotImg.CacheOption = BitmapCacheOption.OnLoad;
            _dotImg.EndInit();
            _cherryImg = new BitmapImage();
            _cherryImg.BeginInit();
            _cherryImg.UriSource = new Uri(_pathPic + "\\cherryImg.jpg");
            _cherryImg.CacheOption = BitmapCacheOption.OnLoad;
            _cherryImg.EndInit();
            _bonusImg = new BitmapImage();
            _bonusImg.BeginInit();
            _bonusImg.UriSource = new Uri(_pathPic + "\\bonusImg.jpg");
            _bonusImg.CacheOption = BitmapCacheOption.OnLoad;
            _bonusImg.EndInit();
            var pacImg = new BitmapImage();
            pacImg.BeginInit();
            pacImg.UriSource = new Uri(_pathPic + "\\pacImg.png");
            pacImg.CacheOption = BitmapCacheOption.OnLoad;
            pacImg.EndInit();
            _pacPic = new Image();
            _pacPic.Source = pacImg;
            _blinkyImg = new BitmapImage();
            _blinkyImg.BeginInit();
            _blinkyImg.UriSource = new Uri(_pathPic + "\\blinkyImg.png");
            _blinkyImg.CacheOption = BitmapCacheOption.OnLoad;
            _blinkyImg.EndInit();
            _blinkyPic = new Image();
            _blinkyPic.Source = _blinkyImg;
            _pinkyImg = new BitmapImage();
            _pinkyImg.BeginInit();
            _pinkyImg.UriSource = new Uri(_pathPic + "\\pinkyImg.png");
            _pinkyImg.CacheOption = BitmapCacheOption.OnLoad;
            _pinkyImg.EndInit();
            _pinkyPic = new Image();
            _pinkyPic.Source = _pinkyImg;
            _inkyImg = new BitmapImage();
            _inkyImg.BeginInit();
            _inkyImg.UriSource = new Uri(_pathPic + "\\inkyImg.png");
            _inkyImg.CacheOption = BitmapCacheOption.OnLoad;
            _inkyImg.EndInit();
            _inkyPic = new Image();
            _inkyPic.Source = _inkyImg;
            _clydeImg = new BitmapImage();
            _clydeImg.BeginInit();
            _clydeImg.UriSource = new Uri(_pathPic + "\\clydeImg.png");
            _clydeImg.CacheOption = BitmapCacheOption.OnLoad;
            _clydeImg.EndInit();
            _fleeImg = new BitmapImage();
            _fleeImg.BeginInit();
            _fleeImg.UriSource = new Uri(_pathPic + "\\fleeImg.png");
            _fleeImg.CacheOption = BitmapCacheOption.OnLoad;
            _fleeImg.EndInit();
            _clydePic = new Image();
            _clydePic.Source = _clydeImg;
            _imgField = new Image[_fieldSize, _fieldSize];
        }

        private void DrawEntities()
        {
            if (_player.Level.FleeTime>_criticalValue)
            {
                _blinkyPic.Source = _fleeImg;
                _pinkyPic.Source = _fleeImg;
                _inkyPic.Source = _fleeImg;
                _clydePic.Source = _fleeImg;
            }
            else
            {
                _blinkyPic.Source = _blinkyImg;
                _pinkyPic.Source = _pinkyImg;
                _inkyPic.Source = _inkyImg;
                _clydePic.Source = _clydeImg;
            }
            Canvas.SetTop(_pacPic, _player.Level.Pacman.Y*_cellSize);
            Canvas.SetLeft(_pacPic, _player.Level.Pacman.X* _cellSize);
            Canvas.SetTop(_blinkyPic, _player.Level.Blinky.Y* _cellSize);
            Canvas.SetLeft(_blinkyPic, _player.Level.Blinky.X* _cellSize);
            Canvas.SetTop(_pinkyPic, _player.Level.Pinky.Y* _cellSize);
            Canvas.SetLeft(_pinkyPic, _player.Level.Pinky.X* _cellSize);
            Canvas.SetTop(_inkyPic, _player.Level.Inky.Y* _cellSize);
            Canvas.SetLeft(_inkyPic, _player.Level.Inky.X* _cellSize);
            Canvas.SetTop(_clydePic, _player.Level.Clyde.Y* _cellSize);
            Canvas.SetLeft(_clydePic, _player.Level.Clyde.X* _cellSize);
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
            _player = new Player(new Level(f, _behaviors[0], _behaviors[1], _behaviors[2], _behaviors[3], _player));
            _player.Level.Player = _player;
        }

        private void InitLevel()
        {
            var f = new Field(new Filler());
            _player.Level = new Level(f, _behaviors[0], _behaviors[1], _behaviors[2], _behaviors[3], _player);
            _player.Level.Player = _player;
            _player.LevelNumber++;
        }

        private void DisplayPlayerStatus()
        {
            Title = String.Format("{3}: {0} {4}: {1} {5}: {2}", _player.LevelNumber, _player.Score,
                _player.Lives, Properties.Resources.Level, Properties.Resources.Score, Properties.Resources.Lives);
        }

        //Core process of gameplay
        //
        //
        private void StepTicker_Tick(object sender, EventArgs e)
        {
            _player.Level.Pacman.Move();
            DrawEntities();
            _imgField[_player.Level.Pacman.Y, _player.Level.Pacman.X].Source = _emptyImg;
            fieldCan.Refresh();
            DisplayPlayerStatus();
            if (_player.Level.GameField.Completed())
            {
                NextLevel();
                _player.Score += _levelScore;
                _player.ScoreTrack += _levelScore;
            }
            _player.CheckCondition();
            if (_player.Lives == _criticalValue)
            {
                AddRecord();
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

        private void NextLevel()
        {
            _stepParallelTicker.Stop();
            _stepTicker.Stop();
            InitLevel();
            PreLevelRenderedAction();
        }

        private void PreLevelRenderedAction()
        {
            DrawField();
            DrawEntities();
            fieldCan.Children.Add(_pacPic);
            fieldCan.Children.Add(_blinkyPic);
            fieldCan.Children.Add(_pinkyPic);
            fieldCan.Children.Add(_inkyPic);
            fieldCan.Children.Add(_clydePic);
            _stepTicker.Interval = new TimeSpan(_pacManFrameDuration);
            _stepParallelTicker.Interval = new TimeSpan(_ghostFrameDuration);
            _stepParallelTicker.Start();
            _stepTicker.Start();
        }

        private void NewGame()
        {
            InitFirstLevel();
            PreLevelRenderedAction();
        }

        //Basic events
        //
        //
        private void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
            records.Visibility = Visibility.Hidden;
            ChangeComboStatus(false);
        }

        //Arrow keys events
        //
        //
        private void fieldCan_KeyDown(object sender, KeyEventArgs e)
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

        private void fieldCan_KeyUp(object sender, KeyEventArgs e)
        {
            if(_player != null)
                _player.Level.Pacman.Moving = false;
        }

        //Combobox actions
        //
        //
        private void ghostAs_Loaded(object sender, RoutedEventArgs e)
        {
            var data = new List<string>();
            data.AddRange(Directory.EnumerateFiles(_pathPlug, "*.dll"));
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.ItemsSource = data;
                comboBox.SelectedIndex = _comboDefaultIndex;
            }
        }

        private void ghostAs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var asm = sender as ComboBox;
            var t = typeof (GhostBehavior);
            if (asm != null)
            {
                var path = asm.SelectedItem as string;
                if (path != null)
                {
                    var type =
                        Assembly.LoadFrom(path).GetTypes().FirstOrDefault(a => t.IsAssignableFrom(a) && !a.IsAbstract);
                    if (type != null)
                    {
                        ghostType ghostSelected = (ghostType) Enum.Parse(typeof (ghostType), asm.Name);
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
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.PluginsErrorMessage);
                        Close();
                    }
                }
            }
        }

        private void TopScoreBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _stepTicker.Stop();
            _stepParallelTicker.Stop();
            DisplayRecords(SelectRecords());
            ChangeComboStatus(true);
        }

        private void ResetScoreBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteRecords();
            DisplayRecords(SelectRecords());
            ChangeComboStatus(true);
        }

        private void ChangeComboStatus(bool b)
        {
            blinkyAs.IsEnabled = b;
            pinkyAs.IsEnabled = b;
            inkyAs.IsEnabled = b;
            clydeAs.IsEnabled = b;
        }

        //DataBase operations
        //
        //
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["RecordsDbString"].ConnectionString;
        }

        private void AddRecord()
        {
            using (SQLiteConnection conn = new SQLiteConnection(GetConnectionString()))
            {
                conn.Open();
                string sql = String.Format("insert into highscores (name, score) values ('{0}', {1})", Environment.MachineName, _player.Score.ToString());
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
        }

        private DataTable SelectRecords()
        {
            using (SQLiteConnection conn = new SQLiteConnection(GetConnectionString()))
            {
                conn.Open();
                string sql = "select * from highscores order by score desc";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = sql;
                SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult);
                DataTable dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
        }

        private void DisplayRecords(DataTable dt)
        {
            records.Text = string.Format("{0}: \r\n", Properties.Resources.Records);
            foreach (DataRow row in dt.Rows)
            {
                records.Text += String.Format("{0} {1} \r\n", row["name"], row["score"]);
            }
            records.Visibility = Visibility.Visible;
        }

        private void DeleteRecords()
        {
            using (SQLiteConnection conn = new SQLiteConnection(GetConnectionString()))
            {
                conn.Open();
                string sql = "delete from highscores";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
        }
    }
}