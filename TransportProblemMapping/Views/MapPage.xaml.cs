using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using TransportProblemMapping.Logic;
using TransportProblemMapping.Markers;

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
            //MainMap.Markers.Add(mRoute);
        }

        private RouteCalculation Rc { get; }
        private PointLatLng PointStart { get; }
        private List<GMapMarker> Markers { get; }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var ShopMarkers = new List<GMapMarker>(Markers
                .Where(x => (x.Shape as CustomMarkerRed).Label.Content.ToString().Contains("Магазин")).ToList());
            var WarehouseMarkers = new List<GMapMarker>(Markers
                .Where(x => (x.Shape as CustomMarkerRed).Label.Content.ToString().Contains("Склад")).ToList());
            var Suppliers = new int[WarehouseMarkers.Count];
            var Shops = new int[ShopMarkers.Count];
            var Matrix = new double[WarehouseMarkers.Count, ShopMarkers.Count];
            var numberRoute = 1;
            var i = 0;
            var j = 0;
            MainMap.Markers.Clear();
            foreach (var MarkerWarehouse in WarehouseMarkers)
            {
                foreach (var MarkerShop in ShopMarkers)
                {
                    var RouteMap = Rc.GetRoute((float)MarkerWarehouse.Position.Lat, (float)MarkerWarehouse.Position.Lng,
                        (float)MarkerShop.Position.Lat, (float)MarkerShop.Position.Lng);
                    var mRoute = RouteMap.RouteMarkers;
                    {
                        mRoute.ZIndex = numberRoute;
                    }
                    numberRoute++;
                    Matrix[i, j] = RouteMap.DistanceMeters;
                    j++;
                }

                i++;
            }

            i = 0;
            foreach (var MarkerShop in ShopMarkers)
            {
                var txtShop = (MarkerShop.Shape as CustomMarkerRed).Label.ToString();
                Shops[i] = int.Parse(txtShop.Substring(txtShop.IndexOf("Товар:") + 7));
                i++;
            }

            i = 0;
            foreach (var MarkerWarehouse in WarehouseMarkers)
            {
                var txtWarehouse = (MarkerWarehouse.Shape as CustomMarkerRed).Label.ToString();
                Suppliers[i] = int.Parse(txtWarehouse.Substring(txtWarehouse.IndexOf("Товар:") + 7));
                i++;
            }

            foreach (var Marker in Markers) AddMarkerOnMap(Marker);
        }

        #region Events

        private void MainMap_MouseEnter(object sender, MouseEventArgs e)
        {
            MainMap.Focus();
        }

        private void MainMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(MainMap);
            if (Markers.Where(x => x.Position == MainMap.FromLocalToLatLng((int)p.X, (int)p.Y)).Count() == 0)
            {
                if (NameCompany.Text == "" || NameCompany.Text == null)
                {
                    DialogText.Text = "Укажите наименование фирмы/магазина!";
                    Dialog.IsOpen = true;
                }
                else if (CountProduct.Text == "" || CountProduct.Text == null)
                {
                    DialogText.Text = "Укажите количество товара!";
                    Dialog.IsOpen = true;
                }
                else if (TypePoint.SelectedIndex == -1)
                {
                    DialogText.Text = "Выберите тип точки!";
                    Dialog.IsOpen = true;
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

        #endregion

        #region Methods

        private void Clear()
        {
            NameCompany.Text = "";
            CountProduct.Text = "";
            TypePoint.SelectedIndex = -1;
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

        private void AddMarkerOnMap(GMapMarker gm)
        {
            MainMap.Markers.Add(gm);
        }

        #endregion
    }
}