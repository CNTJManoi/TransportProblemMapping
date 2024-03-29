﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using Itinero;
using Itinero.Osm.Vehicles;
using TransportProblemMapping.Models;
using TransportProblemMapping.Views;

namespace TransportProblemMapping.Logic
{
    internal class RouteCalculation
    {
        public RouteMapping GetRoute(float sh1, float dl1, float sh2, float dl2, MapPage mp, string startStr,
            string endStr)
        {
            if (!File.Exists(@"Data\siberian-fed-district-latest.routerdb"))
            {
                mp.Dispatcher.Invoke(() => { mp.ShowMessage(ReturnString("Error11")); });
                return null;
            }

            using (var stream = new FileInfo(@"Data\siberian-fed-district-latest.routerdb").Open(FileMode.Open))
            {
                var routeDb = RouterDb.Deserialize(stream);
                var profile = Vehicle.Car.Fastest();
                var router = new Router(routeDb);
                RouterPoint start;
                RouterPoint end;
                Route route;
                try
                {
                    start = router.Resolve(profile, sh1, dl1);
                }
                catch
                {
                    mp.Dispatcher.Invoke(() =>
                    {
                        mp.ShowMessage(ReturnString("Error9") + startStr +
                                       "*");
                    });
                    return null;
                }

                try
                {
                    end = router.Resolve(profile, sh2, dl2);
                }
                catch
                {
                    mp.Dispatcher.Invoke(() =>
                    {
                        mp.ShowMessage(ReturnString("Error9") + endStr +
                                       "*");
                    });
                    return null;
                }

                try
                {
                    route = router.Calculate(profile, start, end);
                }
                catch
                {
                    mp.Dispatcher.Invoke(() =>
                    {
                        mp.ShowMessage(
                            ReturnString("Error10"));
                    });
                    return null;
                }

                var put = new List<PointLatLng>();
                foreach (var item in route.Shape)
                {
                    var pt = new PointLatLng(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude));
                    put.Add(pt);
                }

                var k = route.TotalDistance;
                return new RouteMapping(new GMapRoute(put), route.TotalDistance);
            }
        }

        private string ReturnString(string Attribute)
        {
            return Application.Current.FindResource(Attribute)?.ToString();
        }
    }
}