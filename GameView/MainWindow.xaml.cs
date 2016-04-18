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
using GameView.Entities;
using GameView.Interfaces;

namespace GameView
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        private readonly DispatcherTimer _refreshTicker = new DispatcherTimer();
        private ICoreController _core;
        private const int _comboDefaultIndex = 0;
        private const string _windowTitle = "Pac-Man Game";
        private IPictureInitializer _pictureInitializer;


        public MainWindow()
        { 
            _core = new CoreController();
            _pictureInitializer = new PictureInitializer();
            InitializeComponent();
            _pictureInitializer.LoadPictures();
            AddEventHandlers();
            FormatRecords(_core.SelectRecord());
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MessageBox.Show(FindResource("GreetingMessage").ToString());
            Title = _windowTitle;
        }

        private void DisplayPlayerStatus()
        {
            Title = $"{FindResource("Level")}: {_core.GetPlayer.LevelNumber} {FindResource("Score")}: {_core.GetPlayer.Score} {FindResource("Lives")}: {_core.GetPlayer.Lives}";
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

        private void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            _core.NewGame();
            _pictureInitializer.PreLevelRenderedAction(_core, fieldCan);
            records.Visibility = Visibility.Hidden;
            ChangeComboStatus(false);
            _refreshTicker.Interval = new TimeSpan(100000);
            _refreshTicker.Start();
        }

        private void refresherTicker_Tick(object sender, EventArgs e)
        {
            _pictureInitializer.Redraw(_core, fieldCan);
            fieldCan.Refresh();
            DisplayPlayerStatus();
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
            //
            //Must execute ViewModel ResetScore() method here
            //
            FormatRecords(_core.SelectRecord());
            records.Visibility = Visibility.Visible;
            ChangeComboStatus(true);
            Title = _windowTitle;
        }

        private void AddEventHandlers()
        {
            _refreshTicker.Tick += refresherTicker_Tick;
            App.LanguageChanged += LanguageChanged;
        }
    }
}