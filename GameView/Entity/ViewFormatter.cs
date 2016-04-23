using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Controller.Interfaces;
using PackMan.Entities;
using PackMan.Interfaces;

namespace GameView.Entity
{
    public class ViewFormatter: IMovingObserver, ILevelChangeObserver, IDbObserver, IPluginsObserver, IExceptionObserver
    {
        private readonly MainWindow _window;

        private readonly Canvas _fieldCanvas;

        private readonly BitmapImage _dotImg;

        private readonly BitmapImage _emptyImg;

        private readonly BitmapImage _wallImg;

        private readonly BitmapImage _cherryImg;

        private readonly BitmapImage _bonusImg;

        private readonly BitmapImage _blinkyImg;

        private readonly BitmapImage _pinkyImg;

        private readonly BitmapImage _inkyImg;

        private readonly BitmapImage _clydeImg;

        private readonly BitmapImage _fleeImg;

        private readonly Image[,] _imgField;

        private readonly Image _inkyPic;

        private readonly Image _pacPic;

        private readonly Image _pinkyPic;

        private readonly Image _blinkyPic;

        private readonly Image _clydePic;

        private readonly string _pathPic;

        private const int CellSize = 20;

        private const int CriticalValue = 0;

        private const int FieldSize = 32;

        private const string WindowBasicTitle = "Pac-Man Game";

        private const int ComboDefaultIndex = 0;

        public ViewFormatter(MainWindow window)
        {
            _window = window;
            _fieldCanvas = window.FieldCan;
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
            _imgField = new Image[FieldSize, FieldSize];
            MessageBox.Show(_window.FindResource("GreetingMessage").ToString());
        }

        private void SetGhostPictures()
        {
            _fieldCanvas.Children.Add(_pacPic);
            _fieldCanvas.Children.Add(_blinkyPic);
            _fieldCanvas.Children.Add(_pinkyPic);
            _fieldCanvas.Children.Add(_inkyPic);
            _fieldCanvas.Children.Add(_clydePic);
        }

        private void DrawImagesToCanvas(IPlayer player)
        {
            _fieldCanvas.Children.Clear();
            foreach (var t in player.Level.GameField.GetAllCells())
            {
                var y = t.Item2 * CellSize;
                var x = t.Item3 * CellSize;
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
                _fieldCanvas.Children.Add(i);
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

        private void Redraw(IPlayer player)
        {
            if (player.Level.FleeTime > CriticalValue)
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

            Canvas.SetTop(_pacPic, player.Level.Pacman.Y * CellSize);
            Canvas.SetLeft(_pacPic, player.Level.Pacman.X * CellSize);
            Canvas.SetTop(_blinkyPic, player.Level.Blinky.Y * CellSize);
            Canvas.SetLeft(_blinkyPic, player.Level.Blinky.X * CellSize);
            Canvas.SetTop(_pinkyPic, player.Level.Pinky.Y * CellSize);
            Canvas.SetLeft(_pinkyPic, player.Level.Pinky.X * CellSize);
            Canvas.SetTop(_inkyPic, player.Level.Inky.Y * CellSize);
            Canvas.SetLeft(_inkyPic, player.Level.Inky.X * CellSize);
            Canvas.SetTop(_clydePic, player.Level.Clyde.Y * CellSize);
            Canvas.SetLeft(_clydePic, player.Level.Clyde.X * CellSize);

            _imgField[player.Level.Pacman.Y, player.Level.Pacman.X].Source = _emptyImg;
        }

        private void PreLevelRenderedAction(IPlayer player)
        {
            DrawImagesToCanvas(player);
            Redraw(player);
            SetGhostPictures();
        }

        private void FormatRecords(DataTable dt)
        {
            _window.Records.Text = $"{_window.FindResource("TopScore")}: \r\n";
            foreach (DataRow row in dt.Rows)
            {
                _window.Records.Text += $"{row["name"]} {row["score"]} \r\n";
            }
        }

        private void ChangeComboStatus(bool b)
        {
            _window.BlinkyAs.IsEnabled = b;
            _window.PinkyAs.IsEnabled = b;
            _window.InkyAs.IsEnabled = b;
            _window.ClydeAs.IsEnabled = b;
            _window.LanguageBox.IsEnabled = b;
            _window.Title = WindowBasicTitle;
        }

        private void FillLibraries(ComboBox box, List<string> itemsList)
        {
            if (box != null)
            {
                box.ItemsSource = itemsList;
                box.SelectedIndex = ComboDefaultIndex;
            }
        }

        void IMovingObserver.Update(IPlayer player)
        {
            Redraw(player);
            _window.Title = $"{_window.FindResource("Level")}: {player.LevelNumber}" +
                            $" {_window.FindResource("Score")}: {player.Score} " +
                            $"{_window.FindResource("Lives")}: {player.Lives}";
        }

        void ILevelChangeObserver.Update(IPlayer player)
        {
            PreLevelRenderedAction(player);
            ChangeComboStatus(false);
            _window.Records.Visibility = Visibility.Hidden;
        }

        void IDbObserver.Update(DataTable data)
        {
            ChangeComboStatus(true);
            FormatRecords(data);
            _window.Records.Visibility = Visibility.Visible;
        }

        void IPluginsObserver.Update(List<string> itemList)
        {
            FillLibraries(_window.BlinkyAs, itemList);
            FillLibraries(_window.PinkyAs, itemList);
            FillLibraries(_window.InkyAs, itemList);
            FillLibraries(_window.ClydeAs, itemList);
        }

        public void Update(string exceptionLocalizationResourceKey)
        {
            MessageBox.Show(_window.FindResource(exceptionLocalizationResourceKey).ToString());
            _window.Close();
        }
    }
}
