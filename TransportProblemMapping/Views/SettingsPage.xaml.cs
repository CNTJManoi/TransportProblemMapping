using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using TransportAlgorithms;

namespace TransportProblemMapping.Views
{
    /// <summary>
    ///     Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var um = UnitOfMeasurement.Meters;
            var lang = Languages.Russian;
            var useFuel = true;
            var ta = TypeAlgorithm.NorthWest;
            var priceFuel = 57.25f;
            var consumptionFuel = 12.0f;

            switch (ListBoxMeasurement.SelectedIndex)
            {
                case 0:
                    um = UnitOfMeasurement.Meters;
                    break;
                case 1:
                    um = UnitOfMeasurement.Kilometers;
                    break;
            }

            switch (ListBoxLanguage.SelectedIndex)
            {
                case 0:
                    lang = Languages.Russian;
                    App.Language = new CultureInfo("ru-RU");
                    break;
                case 1:
                    lang = Languages.English;
                    App.Language = new CultureInfo("en-US");
                    break;
            }

            switch (ListBoxUseFuel.SelectedIndex)
            {
                case 0:
                    useFuel = true;
                    break;
                case 1:
                    useFuel = false;
                    break;
            }

            switch (ListBoxMethods.SelectedIndex)
            {
                case 0:
                    ta = TypeAlgorithm.NorthWest;
                    break;
                case 1:
                    ta = TypeAlgorithm.Potentials;
                    break;
            }

            if (PriceBox.Text == "" || ConsumptionBox.Text == "")
            {
                ShowError(ReturnString("Error8"));
                return;
            }

            try
            {
                priceFuel = float.Parse(PriceBox.Text.Replace('.', ','));
            }
            catch
            {
                ShowError(ReturnString("Error6"));
                return;
            }

            try
            {
                consumptionFuel = float.Parse(ConsumptionBox.Text.Replace('.', ','));
            }
            catch
            {
                ShowError(ReturnString("Error7"));
                return;
            }

            App.Measurement = um;
            App.ConsiderFuel = useFuel;
            App.Methods = ta;
            App.PriceFuel = priceFuel;
            App.ConsumptionFuel = consumptionFuel;
        }

        private void ShowError(string text)
        {
            DialogText.Text = text;
            Dialog.IsOpen = true;
        }

        private string ReturnString(string Attribute)
        {
            return Application.Current.FindResource(Attribute)?.ToString();
        }
    }
}