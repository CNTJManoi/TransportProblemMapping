﻿using GMap.NET.WindowsPresentation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using TransportProblemMapping.Views;

namespace TransportProblemMapping.Markers
{
    /// <summary>
    ///     Interaction logic for CustomMarkerDemo.xaml
    /// </summary>
    public partial class CustomMarkerRed
    {
        Popup Popup;
        Label Label;
        //Label Label2;
        GMapMarker Marker;
        MapPage MainWindow;

        public CustomMarkerRed(MapPage window, GMapMarker marker, string title, string data)
        {
            InitializeComponent();

            MainWindow = window;
            Marker = marker;
            title += "\n" + data;
            Popup = new Popup();
            Label = new Label();
            //Label2 = new Label();

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
            //Popup.Placement = PlacementMode.Mouse;
            //{
            //    Label.Background = Brushes.Blue;
            //    Label.Foreground = Brushes.White;
            //    Label.BorderBrush = Brushes.WhiteSmoke;
            //    Label.BorderThickness = new Thickness(2);
            //    Label.Padding = new Thickness(5);
            //    Label.FontSize = 18;
            //    Label.Content = data;
            //}
            Popup.Child = Label;
        }

        void CustomMarkerDemo_Loaded(object sender, RoutedEventArgs e)
        {
            if (Icon.Source.CanFreeze)
            {
                Icon.Source.Freeze();
            }
        }

        void CustomMarkerDemo_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Marker.Offset = new Point(-e.NewSize.Width / 2, -e.NewSize.Height);
        }

        void CustomMarkerDemo_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
            {
                var p = e.GetPosition(MainWindow.MainMap);
                Marker.Position = MainWindow.MainMap.FromLocalToLatLng((int)p.X, (int)p.Y);
            }
        }

        void CustomMarkerDemo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured)
            {
                Mouse.Capture(this);
            }
        }

        void CustomMarkerDemo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Mouse.Capture(null);
            }
        }

        void MarkerControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Marker.ZIndex -= 10000;
            Popup.IsOpen = false;
        }

        void MarkerControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Marker.ZIndex += 10000;
            Popup.IsOpen = true;
        }
    }
}