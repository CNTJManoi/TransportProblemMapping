using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TransportProblemMapping.Views;

namespace TransportProblemMapping
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void ButtonTransit_Click(object sender, RoutedEventArgs e)
        {
            var but = (Button)sender;
            var name = but.Name;
            if (name == "HomeButton" && Transitioner.SelectedIndex != 0)
            {
                Transitioner.SelectedIndex = 0;
                MenuToggleButton.IsChecked = !MenuToggleButton.IsChecked;
            }
            else if (name == "MapButton" && Transitioner.SelectedIndex != 1)
            {
                Transitioner.SelectedIndex = 1;
                MenuToggleButton.IsChecked = !MenuToggleButton.IsChecked;
            }
            else if (name == "InfoButton" && Transitioner.SelectedIndex != 2)
            {
                Transitioner.SelectedIndex = 2;
                MenuToggleButton.IsChecked = !MenuToggleButton.IsChecked;
            }
            else if (name == "SettingsButton" && Transitioner.SelectedIndex != 3)
            {
                Transitioner.SelectedIndex = 3;
                MenuToggleButton.IsChecked = !MenuToggleButton.IsChecked;
            }
            else if (name == "AboutButton" && Transitioner.SelectedIndex != 4)
            {
                Transitioner.Items[4] = new AboutPage();
                Transitioner.SelectedIndex = 4;
                MenuToggleButton.IsChecked = !MenuToggleButton.IsChecked;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}