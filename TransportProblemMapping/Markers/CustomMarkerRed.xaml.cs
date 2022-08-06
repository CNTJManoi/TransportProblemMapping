using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using GMap.NET.WindowsPresentation;
using TransportProblemMapping.Views;

namespace TransportProblemMapping.Markers
{
    /// <summary>
    ///     Interaction logic for CustomMarkerDemo.xaml
    /// </summary>
    public partial class CustomMarkerRed
    {
        public CustomMarkerRed(MapPage window, GMapMarker marker, string title, string data)
        {
            InitializeComponent();

            MainWindow = window;
            Marker = marker;
            title += "\n" + data;
            Popup = new Popup();
            Label = new Label();

            Loaded += CustomMarkerDemo_Loaded;
            SizeChanged += CustomMarkerDemo_SizeChanged;
            MouseEnter += MarkerControl_MouseEnter;
            MouseLeave += MarkerControl_MouseLeave;
            MouseMove += CustomMarkerDemo_MouseMove;
            MouseLeftButtonUp += CustomMarkerDemo_MouseLeftButtonUp;
            MouseLeftButtonDown += CustomMarkerDemo_MouseLeftButtonDown;

            Popup.Placement = PlacementMode.Mouse;
            {
                Label.Background = Brushes.Blue;
                Label.Foreground = Brushes.White;
                Label.BorderBrush = Brushes.WhiteSmoke;
                Label.BorderThickness = new Thickness(2);
                Label.Padding = new Thickness(5);
                Label.FontSize = 22;
                Label.Content = title;
            }
            Popup.Child = Label;
        }

        private Popup Popup { get; }
        public Label Label { get; }
        public GMapMarker Marker { get; }
        private MapPage MainWindow { get; }

        private void CustomMarkerDemo_Loaded(object sender, RoutedEventArgs e)
        {
            if (Icon.Source.CanFreeze) Icon.Source.Freeze();
        }

        private void CustomMarkerDemo_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Marker.Offset = new Point(-e.NewSize.Width / 2, -e.NewSize.Height);
        }

        private void CustomMarkerDemo_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
            {
                var p = e.GetPosition(MainWindow.MainMap);
                Marker.Position = MainWindow.MainMap.FromLocalToLatLng((int)p.X, (int)p.Y);
            }
        }

        private void CustomMarkerDemo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured) Mouse.Capture(this);
        }

        private void CustomMarkerDemo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured) Mouse.Capture(null);
        }

        private void MarkerControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Marker.ZIndex -= 10000;
            Popup.IsOpen = false;
        }

        private void MarkerControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Marker.ZIndex += 10000;
            Popup.IsOpen = true;
        }

        private void DeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.DeleteMarker(MainWindow.MainMap.Markers.Where(x => x.Shape == this).First());
        }

        private void EditMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CheckedFillFields())
            {
                MainWindow.DeleteMarker(MainWindow.MainMap.Markers.Where(x => x.Shape == this).First());
                var title = MainWindow.NameCompany.Text;
                var data = ReturnString("TypeButton") + ": " + MainWindow.TypePoint.Text + "\n" + ReturnString("Product") + ": " +
                    MainWindow.CountProduct.Text;
                title += "\n" + data;
                Label.Content = title;
                Marker.Shape = this;
                MainWindow.AddMarker(this);
            }
        }
        private string ReturnString(string Attribute)
        {
            return Application.Current.FindResource(Attribute)?.ToString();
        }
    }
}