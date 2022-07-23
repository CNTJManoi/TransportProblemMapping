using System;
using System.Collections.Generic;
using System.IO;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using Itinero;
using Itinero.Osm.Vehicles;
using TransportProblemMapping.Models;

namespace TransportProblemMapping.Logic
{
    internal class RouteCalculation
    {
        public RouteMapping GetRoute(float sh1, float dl1, float sh2, float dl2)
        {
            using (var stream = new FileInfo(@"Data\siberian-fed-district-latest.routerdb").Open(FileMode.Open))
            {
                var routeDb = RouterDb.Deserialize(stream);
                var profile = Vehicle.Car.Fastest();
                var router = new Router(routeDb);

                var start = router.Resolve(profile, sh1, dl1);

                var end = router.Resolve(profile, sh2, dl2);

                var route = router.Calculate(profile, start, end);

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
    }
}