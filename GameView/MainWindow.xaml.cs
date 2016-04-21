using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Controller.Core;
using Controller.Interfaces;

using GameView.Entity;

namespace GameView
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var formatter = new ViewFormatter(this);
            var _core = ControllerSingleton.GetInstance.Controller;
            _core.RegisterDbObserver(formatter);
            _core.RegisterLevelChangeObserver(formatter);
            _core.RegisterMovingObserver(formatter);
            _core.RegisterPluginsObserver(formatter);
        }

        // Handling localization elements
        //
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
    }
}