using System.Globalization;
using System.Windows;

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

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            Transitioner.SelectedIndex = 1;
            MenuToggleButton.IsChecked = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}