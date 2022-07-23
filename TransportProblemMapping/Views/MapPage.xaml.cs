using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using TransportAlgorithms;
using TransportProblemMapping.Logic;
using TransportProblemMapping.Markers;
using TransportProblemMapping.Models;

namespace TransportProblemMapping.Views
{
    public partial class MapPage : UserControl
    {
        public MapPage()
        {
            InitializeComponent();
            Rc = new RouteCalculation();
            PointStart = new PointLatLng(55.025070240194744, 82.9275512695313);
            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            MainMap.Position = PointStart;
            MainMap.MultiTouchEnabled = true;
            MainMap.MinZoom = 1;
            MainMap.MaxZoom = 20;
            MainMap.MouseWheelZoomEnabled = true;
            MainMap.Zoom = 10;

            //var mRoute = Rc.GetRoute(55.068588649680024f, 82.93997918613105f, 55.065228522013925f, 82.92722868715828f).RouteMarkers;
            //{
            //    mRoute.ZIndex = 1;
            //}

            MainMap.OnTileLoadComplete += MainMap_OnTileLoadComplete;
            MainMap.MouseMove += MainMap_MouseMove;
            MainMap.MouseLeftButtonDown += MainMap_MouseLeftButtonDown;
            MainMap.MouseEnter += MainMap_MouseEnter;

            //Маркер
            Markers = new List<GMapMarker>();

            Transport = new TransportProblem();
        }

        private RouteCalculation Rc { get; }
        private PointLatLng PointStart { get; }
        private List<GMapMarker> Markers { get; }
        private TransportProblem Transport { get; }

        #region Events

        private void MainMap_MouseEnter(object sender, MouseEventArgs e)
        {
            MainMap.Focus();
        }

        private void MainMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(MainMap);
            if (Markers.Count(x => x.Position == MainMap.FromLocalToLatLng((int)p.X, (int)p.Y)) == 0)
            {
                if (string.IsNullOrEmpty(NameCompany.Text))
                {
                    ShowMessage("Укажите наименование фирмы/магазина!");
                }
                else if (string.IsNullOrEmpty(CountProduct.Text))
                {
                    ShowMessage("Укажите количество товара!");
                }
                else if (TypePoint.SelectedIndex == -1)
                {
                    ShowMessage("Выберите тип точки!");
                }
                else
                {
                    AddMarker(p);
                    Clear();
                }
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

        private void MainMap_OnTileLoadComplete(long elapsedMilliseconds)
        {
            MainMap.ElapsedMilliseconds = elapsedMilliseconds;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            List<List<RouteMapping>> routesMappings = new List<List<RouteMapping>>();
            var shopMarkers = new List<GMapMarker>(Markers
                .Where(x => ((CustomMarkerRed)x.Shape).Label.Content.ToString().Contains("Магазин")).ToList());
            var warehouseMarkers = new List<GMapMarker>(Markers
                .Where(x => ((CustomMarkerRed)x.Shape).Label.Content.ToString().Contains("Склад")).ToList());
            if (shopMarkers.Count <= 1)
            {
                ShowMessage("Добавьте точки магазинов! Должно быть минимум 2.");
                return;
            } else
            if (warehouseMarkers.Count <= 1)
            {
                ShowMessage("Добавьте точки складов! Должно быть минимум 2.");
                return;
            }
            var suppliers = new int[warehouseMarkers.Count];
            var shops = new int[shopMarkers.Count];
            var matrix = new double[warehouseMarkers.Count, shopMarkers.Count];
            var numberRoute = 1;
            var i = 0;
            var j = 0;
            MainMap.Markers.Clear();
            foreach (var markerWarehouse in warehouseMarkers)
            {
                routesMappings.Add(new List<RouteMapping>());
                foreach (var markerShop in shopMarkers)
                {
                    string txtW = ((CustomMarkerRed)warehouseMarkers[i].Shape).Label.Content.ToString();
                    string txtS = ((CustomMarkerRed)shopMarkers[j].Shape).Label.Content.ToString();
                    var routeMap = Rc.GetRoute((float)markerWarehouse.Position.Lat, (float)markerWarehouse.Position.Lng,
                        (float)markerShop.Position.Lat, (float)markerShop.Position.Lng, this, txtW.Substring(0, txtW.IndexOf("\n")),
                        txtS.Substring(0, txtS.IndexOf("\n")));
                    if (routeMap == null)
                    {
                        foreach (var marker in Markers) AddMarkerOnMap(marker);
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
            foreach (var markerShop in shopMarkers)
            {
                var txtShop = ((CustomMarkerRed)markerShop.Shape).Label.ToString();
                shops[i] = int.Parse(txtShop.Substring(txtShop.IndexOf("Товар:") + 7));
                i++;
            }

            i = 0;
            foreach (var markerWarehouse in warehouseMarkers)
            {
                var txtWarehouse = (markerWarehouse.Shape as CustomMarkerRed).Label.ToString();
                suppliers[i] = int.Parse(txtWarehouse.Substring(txtWarehouse.IndexOf("Товар:") + 7));
                i++;
            }

            foreach (var marker in Markers) AddMarkerOnMap(marker);
            var solution = Transport.FindSolution(matrix, suppliers, shops, TypeAlgorithm.Potentials);
            string solutionMessage = "";
            for (i = 0; i < solution.GetLength(0); i++)
            {
                for (j = 0; j < solution.GetLength(1); j++)
                {
                    if (solution[i, j] != 0)
                    {
                        string txtW = ((CustomMarkerRed)warehouseMarkers[i].Shape).Label.Content.ToString();
                        string txtS = ((CustomMarkerRed)shopMarkers[j].Shape).Label.Content.ToString();
                        solutionMessage += "Из склада " + txtW.Substring(0,txtW.IndexOf("\n")) 
                                                        + " в магазин " + txtS.Substring(0,txtS.IndexOf("\n")) + " - " 
                                                        + solution[i, j].ToString() + " единиц товара.\n";
                        AddMarker(routesMappings[i][j]);
                    }
                }
            }

            solutionMessage += "Математическая модель: " + Transport.MathematicalPrice;
            SolutionBox.Text = solutionMessage;
        }

        #endregion

        #region Methods

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
                    "Тип: " + TypePoint.Text + "\nТовар: " + CountProduct.Text);
                gm.Offset = new Point(-15, -15);
            }
            Markers.Add(gm);
            MainMap.Markers.Add(gm);
        }
        private void AddMarker(RouteMapping p)
        {
            MainMap.Markers.Add(p.RouteMarkers);
        }

        private void AddMarkerOnMap(GMapMarker gm)
        {
            MainMap.Markers.Add(gm);
        }

        #endregion
    }
}