using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Itinero.Attributes;
using TransportAlgorithms;
using TransportProblemMapping.Logic;
using TransportProblemMapping.Markers;
using TransportProblemMapping.Models;

namespace TransportProblemMapping.Views
{
    public partial class MapPage : UserControl
    {
        #region Fields

        private Thread calcThread;
        #endregion

        public MapPage()
        {
            InitializeComponent();
            App.LanguageChanged += LanguageChanged;
            Rc = new RouteCalculation();
            PointStart = new PointLatLng(55.025070240194744, 82.9275512695313);
            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            MainMap.Position = PointStart;
            MainMap.MultiTouchEnabled = true;
            MainMap.MinZoom = 1;
            MainMap.MaxZoom = 20;
            MainMap.MouseWheelZoomEnabled = true;
            MainMap.Zoom = 10;

            MainMap.MouseMove += MainMap_MouseMove;
            MainMap.MouseLeftButtonDown += MainMap_MouseLeftButtonDown;
            MainMap.MouseEnter += MainMap_MouseEnter;

            Markers = new List<GMapMarker>();

            Transport = new TransportProblem();
            ChangeLanguage();
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            ClearAll();
        }

        #region Properties

        private RouteCalculation Rc { get; }
        private PointLatLng PointStart { get; }
        private List<GMapMarker> Markers { get; }
        private TransportProblem Transport { get; }

        #endregion

        #region Events

        private void MainMap_MouseEnter(object sender, MouseEventArgs e)
        {
            MainMap.Focus();
        }

        private void MainMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(MainMap);
            if (Markers.Count(x => x.Position == MainMap.FromLocalToLatLng((int)p.X, (int)p.Y)) == 0)
                if (CheckedFillFields())
                {
                    AddMarker(p);
                    Clear();
                }
        }

        private void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var p = e.GetPosition(MainMap);
                foreach (var marker in Markers)
                    if (marker.Position == MainMap.FromLocalToLatLng((int)p.X, (int)p.Y))
                        Markers[Markers.IndexOf(marker)].Position = MainMap.FromLocalToLatLng((int)p.X, (int)p.Y);
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            calcThread = new Thread(CalcThreading);
            WaitDialog.IsOpen = true;
            calcThread.Start();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        #endregion

        #region Methods

        private void CalcThreading()
        {
            var routesMappings = new List<List<RouteMapping>>();
            var shopMarkers = new List<GMapMarker>();
            var warehouseMarkers = new List<GMapMarker>();
            Dispatcher.Invoke(() =>
            {
                shopMarkers = new List<GMapMarker>(Markers
                    .Where(x => ((CustomMarkerRed)x.Shape).Label.Content.ToString().Contains(ReturnString("Shop")))
                    .ToList());
                warehouseMarkers = new List<GMapMarker>(Markers
                    .Where(x => ((CustomMarkerRed)x.Shape).Label.Content.ToString().Contains(ReturnString("Warehouse")))
                    .ToList());
            });
            Dispatcher.Invoke(() =>
            {
                if (shopMarkers.Count <= 1)
                {
                    calcThread.Abort();
                    ShowMessage(ReturnString("Error1"));
                    WaitDialog.IsOpen = false;
                    return;
                }

                if (warehouseMarkers.Count <= 1)
                {
                    calcThread.Abort();
                    ShowMessage(ReturnString("Error2"));
                    WaitDialog.IsOpen = false;
                }
            });
            var suppliers = new int[warehouseMarkers.Count];
            var shops = new int[shopMarkers.Count];
            var matrix = new double[warehouseMarkers.Count, shopMarkers.Count];
            var numberRoute = 1;
            var i = 0;
            var j = 0;
            Dispatcher.Invoke(() => { MainMap.Markers.Clear(); });
            foreach (var markerWarehouse in warehouseMarkers)
            {
                routesMappings.Add(new List<RouteMapping>());
                foreach (var markerShop in shopMarkers)
                {
                    var txtW = "";
                    var txtS = "";
                    Dispatcher.Invoke(() =>
                    {
                        txtW = ((CustomMarkerRed)warehouseMarkers[i].Shape).Label.Content.ToString();
                        txtS = ((CustomMarkerRed)shopMarkers[j].Shape).Label.Content.ToString();
                    });
                    var routeMap = Rc.GetRoute((float)markerWarehouse.Position.Lat, (float)markerWarehouse.Position.Lng,
                        (float)markerShop.Position.Lat, (float)markerShop.Position.Lng, this,
                        txtW.Substring(0, txtW.IndexOf("\n")),
                        txtS.Substring(0, txtS.IndexOf("\n")));
                    if (routeMap == null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            foreach (var marker in Markers) AddMarkerOnMap(marker);
                        });
                        return;
                    }

                    var mRoute = routeMap.RouteMarkers;
                    {
                        mRoute.ZIndex = numberRoute;
                    }
                    numberRoute++;
                    matrix[i, j] = routeMap.DistanceMeters;
                    routesMappings[i].Add(new RouteMapping(mRoute, routeMap.DistanceMeters));
                    j++;
                }

                j = 0;
                i++;
            }

            i = 0;
            string pr = ReturnString("Product");
            foreach (var markerShop in shopMarkers)
                Dispatcher.Invoke(() =>
                {
                    var txtShop = ((CustomMarkerRed)markerShop.Shape).Label.ToString();
                    shops[i] = int.Parse(txtShop.Substring(txtShop.IndexOf(pr) + (pr.Length+1)));
                    i++;
                });

            i = 0;
            foreach (var markerWarehouse in warehouseMarkers)
                Dispatcher.Invoke(() =>
                {
                    var txtWarehouse = (markerWarehouse.Shape as CustomMarkerRed).Label.ToString();
                    suppliers[i] = int.Parse(txtWarehouse.Substring(txtWarehouse.IndexOf(pr) + (pr.Length + 1)));
                    i++;
                });

            Dispatcher.Invoke(() =>
            {
                foreach (var marker in Markers) AddMarkerOnMap(marker);
            });
            var solution = Transport.FindSolution(matrix, suppliers, shops, TypeAlgorithm.Potentials);
            var solutionMessage = "";
            for (i = 0; i < solution.GetLength(0); i++)
            for (j = 0; j < solution.GetLength(1); j++)
                if (solution[i, j] != 0)
                {
                    var txtW = "";
                    var txtS = "";
                    Dispatcher.Invoke(() =>
                    {
                        txtW = ((CustomMarkerRed)warehouseMarkers[i].Shape).Label.Content.ToString();
                        txtS = ((CustomMarkerRed)shopMarkers[j].Shape).Label.Content.ToString();
                    });
                    solutionMessage += ReturnString("Info1") + " " + txtW.Substring(0, txtW.IndexOf("\n")) + " "
                                                             + ReturnString("Info2") + " " +
                                                             txtS.Substring(0, txtS.IndexOf("\n")) + " - "
                                                             + solution[i, j] + " " + ReturnString("Info3") + "\n";
                    Dispatcher.Invoke(() => { AddMarker(routesMappings[i][j]); });
                }

            solutionMessage += ReturnString("Mathematical") + " " + Transport.MathematicalPrice;
            Dispatcher.Invoke(() =>
            {
                SolutionBox.Text = solutionMessage;
                WaitDialog.IsOpen = false;
            });
        }

        private void Clear()
        {
            NameCompany.Text = "";
            CountProduct.Text = "";
            TypePoint.SelectedIndex = -1;
        }

        public void ShowMessage(string message)
        {
            DialogText.Text = message;
            Dialog.IsOpen = true;
        }

        private void AddMarker(Point p)
        {
            var gm = new GMapMarker(MainMap.FromLocalToLatLng((int)p.X, (int)p.Y));
            {
                gm.Shape = new CustomMarkerRed(this, gm, NameCompany.Text,
                    ReturnString("TypeButton") + ": " + TypePoint.Text + "\n" + ReturnString("Product") + ": " +
                    CountProduct.Text);
                gm.Offset = new Point(-15, -15);
            }
            Markers.Add(gm);
            MainMap.Markers.Add(gm);
        }

        private void AddMarker(RouteMapping p)
        {
            MainMap.Markers.Add(p.RouteMarkers);
        }

        public void AddMarker(CustomMarkerRed cm)
        {
            Markers.Add(cm.Marker);
            MainMap.Markers.Add(cm.Marker);
            Clear();
        }

        private void AddMarkerOnMap(GMapMarker gm)
        {
            MainMap.Markers.Add(gm);
        }

        private void ChangeLanguage()
        {
        }

        public void DeleteMarker(GMapMarker gm)
        {
            MainMap.Markers.Remove(gm);
            Markers.Remove(gm);
        }

        private void ClearAll()
        {
            MainMap.Markers.Clear();
            Markers.Clear();
        }

        public bool CheckedFillFields()
        {
            if (string.IsNullOrEmpty(NameCompany.Text))
            {
                ShowMessage(ReturnString("Error3"));
                return false;
            }

            if (string.IsNullOrEmpty(CountProduct.Text))
            {
                ShowMessage(ReturnString("Error4"));
                return false;
            }

            if (TypePoint.SelectedIndex == -1)
            {
                ShowMessage(ReturnString("Error5"));
                return false;
            }

            return true;
        }

        private string ReturnString(string Attribute)
        {
            return Application.Current.FindResource(Attribute)?.ToString();
        }
        #endregion
    }
}