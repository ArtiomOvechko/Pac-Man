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
using System.Globalization;
using Controller.Core;
using Controller.Interfaces;

namespace GameView
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DispatcherTimer _refreshTicker = new DispatcherTimer();
        private ICoreController _core;
        private BitmapImage _dotImg;
        private BitmapImage _emptyImg;
        private BitmapImage _wallImg;
        private BitmapImage _cherryImg;
        private BitmapImage _bonusImg;
        private BitmapImage _blinkyImg;
        private BitmapImage _pinkyImg;
        private BitmapImage _inkyImg;
        private BitmapImage _clydeImg;
        private BitmapImage _fleeImg;
        private Image[,] _imgField;
        private Image _inkyPic;
        private Image _pacPic;
        private Image _pinkyPic;
        private Image _blinkyPic;
        private Image _clydePic;
        private string _pathPic;
        private const int _cellSize = 20;
        private const int _criticalValue = 0;
        private const int _fieldSize = 32;
        private const int _comboDefaultIndex = 0;
        private const string _windowTitle = "Pac-Man Game";



        public MainWindow()
        { 
            _core = new CoreController();
            InitializeComponent();
            InitPictures();
            AddEventHandlers();
            FormatRecords(_core.SelectRecord());
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MessageBox.Show(FindResource("GreetingMessage").ToString());
            Title = _windowTitle;
        }

        private void DrawField()
        {
            fieldCan.Children.Clear();
            foreach (var t in _core.GetPlayer.Level.GameField.GetAllCells())
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

        private BitmapImage InitPicture(string path)
        {
            BitmapImage result = new BitmapImage();
            result.BeginInit();
            result.UriSource = new Uri(_pathPic + path);
            result.CacheOption = BitmapCacheOption.OnLoad;
            result.EndInit();
            return result;
        }

        private Image InitImage(BitmapImage source)
        {
            Image result = new Image();
            result.Source = source;
            return result;
        }

        private void InitPictures()
        {
            _pathPic = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Pictures";
            _wallImg = InitPicture("\\wallImg.jpg");
            _emptyImg = InitPicture("\\emptyImg.jpg");
            _dotImg = InitPicture("\\dotImg.jpg");
            _cherryImg = InitPicture("\\cherryImg.jpg");
            _bonusImg = InitPicture("\\bonusImg.jpg");
            var pacImg = InitPicture("\\pacImg.png");
            _pacPic = InitImage(pacImg);
            _blinkyImg = InitPicture("\\blinkyImg.png");
            _blinkyPic = InitImage(_blinkyImg);
            _pinkyImg = InitPicture("\\pinkyImg.png");
            _pinkyPic = InitImage(_pinkyImg);
            _inkyImg = InitPicture("\\inkyImg.png");
            _inkyPic = InitImage(_inkyImg);
            _clydeImg = InitPicture("\\clydeImg.png");
            _clydePic = InitImage(_clydeImg);
            _fleeImg = InitPicture("\\fleeImg.png");
            _imgField = new Image[_fieldSize, _fieldSize];
        }

        private void DrawEntities()
        {
            if (_core.GetPlayer.Level.FleeTime>_criticalValue)
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

            Canvas.SetTop(_pacPic, _core.GetPlayer.Level.Pacman.Y*_cellSize);
            Canvas.SetLeft(_pacPic, _core.GetPlayer.Level.Pacman.X* _cellSize);
            Canvas.SetTop(_blinkyPic, _core.GetPlayer.Level.Blinky.Y* _cellSize);
            Canvas.SetLeft(_blinkyPic, _core.GetPlayer.Level.Blinky.X* _cellSize);
            Canvas.SetTop(_pinkyPic, _core.GetPlayer.Level.Pinky.Y* _cellSize);
            Canvas.SetLeft(_pinkyPic, _core.GetPlayer.Level.Pinky.X* _cellSize);
            Canvas.SetTop(_inkyPic, _core.GetPlayer.Level.Inky.Y* _cellSize);
            Canvas.SetLeft(_inkyPic, _core.GetPlayer.Level.Inky.X* _cellSize);
            Canvas.SetTop(_clydePic, _core.GetPlayer.Level.Clyde.Y* _cellSize);
            Canvas.SetLeft(_clydePic, _core.GetPlayer.Level.Clyde.X* _cellSize);
        }

        private void AddEventHandlers()
        {
            _refreshTicker.Tick += refresherTicker_Tick;
            App.LanguageChanged += LanguageChanged;
        }

        private void DisplayPlayerStatus()
        {
            Title = $"{FindResource("Level")}: {_core.GetPlayer.LevelNumber} {FindResource("Score")}: {_core.GetPlayer.Score} {FindResource("Lives")}: {_core.GetPlayer.Lives}";
        }

        private void refresherTicker_Tick(object sender, EventArgs e)
        {
            DrawEntities();
            _imgField[_core.GetPlayer.Level.Pacman.Y, _core.GetPlayer.Level.Pacman.X].Source = _emptyImg;
            fieldCan.Refresh();
            DisplayPlayerStatus();
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
        }

        private void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            _core.NewGame();
            PreLevelRenderedAction();
            records.Visibility = Visibility.Hidden;
            ChangeComboStatus(false);
            _refreshTicker.Interval = new TimeSpan(500000);
            _refreshTicker.Start();
        }

        private void LanguageChanged(Object sender, EventArgs e)
        {
            FormatRecords(_core.SelectRecord());
        }

        private void fieldCan_KeyDown(object sender, KeyEventArgs e)
        {
            _core.PressAction(e);
        }

        private void fieldCan_KeyUp(object sender, KeyEventArgs e)
        {
            _core.ReleaseAction();
        }

        private void ghostAs_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.ItemsSource = _core.GetLibraries;
                comboBox.SelectedIndex = _comboDefaultIndex;
            }
        }

        private void ghostAs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var asm = sender as ComboBox;
            _core.SetBehavior(asm.Name, asm.SelectedItem.ToString());
        }

        private void LanguageBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var lang in App.Languages)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = lang.DisplayName;
                item.Tag = lang;
                languageBox.Items.Add(lang);
            }
            var index = languageBox.Items.IndexOf(App.Language);
            languageBox.SelectedIndex = index;
        }

        private void LanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CultureInfo item = languageBox.SelectedItem as CultureInfo; 
            if (item != null)
            {
                App.Language = item;
            }
        }

        //Score buttons events
        private void TopScoreBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _refreshTicker.Stop();
            _core.StopGameProcess();
            FormatRecords(_core.SelectRecord());
            records.Visibility = Visibility.Visible;
            ChangeComboStatus(true);
            Title = _windowTitle;
        }

        private void ResetScoreBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _refreshTicker.Stop();
            _core.ResetScore();
            FormatRecords(_core.SelectRecord());
            records.Visibility = Visibility.Visible;
            ChangeComboStatus(true);
            Title = _windowTitle;
        }

        private void ChangeComboStatus(bool b)
        {
            blinkyAs.IsEnabled = b;
            pinkyAs.IsEnabled = b;
            inkyAs.IsEnabled = b;
            clydeAs.IsEnabled = b;
            languageBox.IsEnabled = b;
            Title = _windowTitle;
        }

        private void FormatRecords(DataTable dt)
        {
            records.Text = $"{FindResource("TopScore")}: \r\n";
            foreach (DataRow row in dt.Rows)
            {
                records.Text += $"{row["name"]} {row["score"]} \r\n";
            }
        }
    }
}