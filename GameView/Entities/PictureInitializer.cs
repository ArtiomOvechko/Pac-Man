using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Controller.Interfaces;
using GameView.Interfaces;
using PackMan.Entities;

namespace GameView.Entities
{
    public class PictureInitializer: IPictureInitializer
    {
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
        private int currentLevel = 1;

        public void DrawImagesToCanvas(ICoreController core, Canvas fieldCan)
        {
            fieldCan.Children.Clear();
            foreach (var t in core.GetPlayer.Level.GameField.GetAllCells())
            {
                var y = t.Item2 * _cellSize;
                var x = t.Item3 * _cellSize;
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

        public void LoadPictures()
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

        public void Redraw(ICoreController core, Canvas fieldCan)
        {
            if (currentLevel < core.GetPlayer.LevelNumber)
            {
                currentLevel++;
                PreLevelRenderedAction(core, fieldCan);
                return;
            }
            if (core.GetPlayer.Level.FleeTime > _criticalValue)
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

            Canvas.SetTop(_pacPic, core.GetPlayer.Level.Pacman.Y * _cellSize);
            Canvas.SetLeft(_pacPic, core.GetPlayer.Level.Pacman.X * _cellSize);
            Canvas.SetTop(_blinkyPic, core.GetPlayer.Level.Blinky.Y * _cellSize);
            Canvas.SetLeft(_blinkyPic, core.GetPlayer.Level.Blinky.X * _cellSize);
            Canvas.SetTop(_pinkyPic, core.GetPlayer.Level.Pinky.Y * _cellSize);
            Canvas.SetLeft(_pinkyPic, core.GetPlayer.Level.Pinky.X * _cellSize);
            Canvas.SetTop(_inkyPic, core.GetPlayer.Level.Inky.Y * _cellSize);
            Canvas.SetLeft(_inkyPic, core.GetPlayer.Level.Inky.X * _cellSize);
            Canvas.SetTop(_clydePic, core.GetPlayer.Level.Clyde.Y * _cellSize);
            Canvas.SetLeft(_clydePic, core.GetPlayer.Level.Clyde.X * _cellSize);

            _imgField[core.GetPlayer.Level.Pacman.Y, core.GetPlayer.Level.Pacman.X].Source = _emptyImg;
        }

        public void SetGhostPictures(Canvas fieldCan)
        {
            fieldCan.Children.Add(_pacPic);
            fieldCan.Children.Add(_blinkyPic);
            fieldCan.Children.Add(_pinkyPic);
            fieldCan.Children.Add(_inkyPic);
            fieldCan.Children.Add(_clydePic);
        }

        public void PreLevelRenderedAction(ICoreController core, Canvas fieldCan)
        {
            DrawImagesToCanvas(core, fieldCan);
            Redraw(core, fieldCan);
            SetGhostPictures(fieldCan);
        }
    }
}
