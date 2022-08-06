using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using TransportAlgorithms;
using TransportProblemMapping.Properties;

namespace TransportProblemMapping
{
    public enum UnitOfMeasurement
    {
        Meters,
        Kilometers
    }

    public enum Languages
    {
        Russian,
        English
    }

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            LanguageChanged += App_LanguageChanged;
            Language = Settings.Default.DefaultLanguage;

            Languages.Clear();
            Languages.Add(new CultureInfo("ru-RU"));
            Languages.Add(new CultureInfo("en-US"));
            Measurement = UnitOfMeasurement.Meters;
            ConsiderFuel = true;
            Methods = TypeAlgorithm.Potentials;
            PriceFuel = 57.25f;
            ConsumptionFuel = 12.0f;
        }

        #region Settings

        public static UnitOfMeasurement Measurement { get; set; }
        public static bool ConsiderFuel { get; set; }
        public static TypeAlgorithm Methods { get; set; }
        public static float PriceFuel { get; set; }
        public static float ConsumptionFuel { get; set; }

        #endregion

        #region Language

        public static List<CultureInfo> Languages { get; } = new List<CultureInfo>();

        public static event EventHandler LanguageChanged;

        public static CultureInfo Language
        {
            get => Thread.CurrentThread.CurrentUICulture;
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == Thread.CurrentThread.CurrentUICulture) return;
                Thread.CurrentThread.CurrentUICulture = value;
                var dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "en-US":
                        dict.Source = new Uri(string.Format("Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("Resources/lang.xaml", UriKind.Relative);
                        break;
                }

                var oldDict = (from d in Current.Resources.MergedDictionaries
                    where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
                    select d).First();
                if (oldDict != null)
                {
                    var ind = Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Current.Resources.MergedDictionaries.Remove(oldDict);
                    Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Current.Resources.MergedDictionaries.Add(dict);
                }

                LanguageChanged(Current, new EventArgs());
            }
        }

        private void App_LanguageChanged(object sender, EventArgs e)
        {
            Settings.Default.DefaultLanguage = Language;
            Settings.Default.Save();
        }

        #endregion
    }
}