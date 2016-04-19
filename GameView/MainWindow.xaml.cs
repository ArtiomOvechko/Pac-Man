using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Controller.Core;
using Controller.Interface;

using GameView.Entity;
using GameView.Extension;
using GameView.Interface;

namespace GameView
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DispatcherTimer _refreshTicker = new DispatcherTimer();

        private readonly ICoreController _core;

        private const int ComboDefaultIndex = 0;

        private const string WindowTitle = "Pac-Man Game";

        private readonly IPictureInitializer _pictureInitializer;


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
            Title = WindowTitle;
        }

        private void DisplayPlayerStatus()
        {
            Title = $"{FindResource("Level")}: {_core.GetPlayer.LevelNumber}" +
                    $" {FindResource("Score")}: {_core.GetPlayer.Score} " +
                    $"{FindResource("Lives")}: {_core.GetPlayer.Lives}";
        }

        private void ChangeComboStatus(bool b)
        {
            BlinkyAs.IsEnabled = b;
            PinkyAs.IsEnabled = b;
            InkyAs.IsEnabled = b;
            ClydeAs.IsEnabled = b;
            LanguageBox.IsEnabled = b;
            Title = WindowTitle;
        }

        private void FormatRecords(DataTable dt)
        {
            Records.Text = $"{FindResource("TopScore")}: \r\n";
            foreach (DataRow row in dt.Rows)
            {
                Records.Text += $"{row["name"]} {row["score"]} \r\n";
            }
        }

        private void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            _core.NewGame();
            _pictureInitializer.PreLevelRenderedAction(_core, FieldCan);
            Records.Visibility = Visibility.Hidden;
            ChangeComboStatus(false);
            _refreshTicker.Interval = new TimeSpan(100000);
            _refreshTicker.Start();
        }

        private void refresherTicker_Tick(object sender, EventArgs e)
        {
            _pictureInitializer.Redraw(_core, FieldCan);
            FieldCan.Refresh();
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
                comboBox.SelectedIndex = ComboDefaultIndex;
            }
        }

        private void ghostAs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var asm = sender as ComboBox;
            if (asm != null)
            {
                _core.SetBehavior(asm.Name, asm.SelectedItem.ToString());
            }
        }

    private void LanguageBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var lang in App.Languages)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = lang.DisplayName;
                item.Tag = lang;
                LanguageBox.Items.Add(lang);
            }
            var index = LanguageBox.Items.IndexOf(App.Language);
            LanguageBox.SelectedIndex = index;
        }

        private void LanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CultureInfo item = LanguageBox.SelectedItem as CultureInfo; 
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
            Records.Visibility = Visibility.Visible;
            ChangeComboStatus(true);
            Title = WindowTitle;
        }

        private void ResetScoreBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _refreshTicker.Stop();
            //
            //Must execute ViewModel ResetScore() method here
            //
            FormatRecords(_core.SelectRecord());
            Records.Visibility = Visibility.Visible;
            ChangeComboStatus(true);
            Title = WindowTitle;
        }

        private void AddEventHandlers()
        {
            _refreshTicker.Tick += refresherTicker_Tick;
            App.LanguageChanged += LanguageChanged;
        }
    }
}