using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using TransportProblemMapping.Markers;

namespace TransportProblemMapping.Views
{
    /// <summary>
    /// Логика взаимодействия для MapPage.xaml
    /// </summary>
    public partial class MapPage : UserControl
    {
        PointLatLng PointStart { get; set; }
        // marker
        List<GMapMarker> Markers { get; set; }
        public MapPage()
        {
            InitializeComponent();
            PointStart = new PointLatLng(55.025070240194744, 82.9275512695313);
            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            MainMap.Position = PointStart;
            MainMap.MultiTouchEnabled = true;
            MainMap.MinZoom = 1;
            MainMap.MaxZoom = 15;
            MainMap.MouseWheelZoomEnabled = true;
            MainMap.Zoom = 10;

            // map events
            MainMap.OnTileLoadComplete += MainMap_OnTileLoadComplete;
            MainMap.MouseMove += MainMap_MouseMove;
            MainMap.MouseLeftButtonDown += MainMap_MouseLeftButtonDown;
            MainMap.MouseEnter += MainMap_MouseEnter;

            //Маркер
            Markers = new List<GMapMarker>();
        }

        #region Events
        void MainMap_MouseEnter(object sender, MouseEventArgs e)
        {
            MainMap.Focus();
        }
        void MainMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
        void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var p = e.GetPosition(MainMap);
                foreach (var marker in Markers)
                {
                    if(marker.Position == MainMap.FromLocalToLatLng((int)p.X, (int)p.Y)) 
                    {
                        Markers[Markers.IndexOf(marker)].Position = MainMap.FromLocalToLatLng((int)p.X, (int)p.Y);
                    }
                }
            }
        }
        void MainMap_OnTileLoadComplete(long elapsedMilliseconds)
        {
            MainMap.ElapsedMilliseconds = elapsedMilliseconds;
        }
        #endregion

        #region Methods
        void Clear()
        {
            NameCompany.Text = "";
            CountProduct.Text = "";
            TypePoint.SelectedIndex = -1;
        }
        void AddMarker(Point p)
        {
            GMapMarker gm = new GMapMarker(MainMap.FromLocalToLatLng((int)p.X, (int)p.Y));
            {
                gm.Shape = new CustomMarkerRed(this, gm, NameCompany.Text, "Тип: " + TypePoint.Text + "\nТовар: " + CountProduct.Text);
                gm.Offset = new Point(-15, -15);
            }
            Markers.Add(gm);
            MainMap.Markers.Add(gm);
        }

        #endregion
    }
}
