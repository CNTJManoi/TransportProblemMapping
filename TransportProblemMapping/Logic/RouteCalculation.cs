using GMap.NET;
using GMap.NET.WindowsPresentation;
using Itinero;
using Itinero.Osm.Vehicles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportProblemMapping.Models;

namespace TransportProblemMapping.Logic
{
    class RouteCalculation
    {
        public RouteMapping GetRoute(Single sh1, Single dl1, Single sh2, Single dl2)
        {
            using (var stream = new FileInfo(@"Data\siberian-fed-district-latest.routerdb").Open(FileMode.Open))
            {
                var routeDb = RouterDb.Deserialize(stream);
                var profile = Vehicle.Car.Fastest();
                var router = new Router(routeDb);

                var start = router.Resolve(profile, sh1, dl1);

                var end = router.Resolve(profile, sh2, dl2);

                var route = router.Calculate(profile, start, end);

                List<PointLatLng> put = new List<PointLatLng>();
                foreach (var item in route.Shape)
                {
                    PointLatLng pt = new PointLatLng(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude));
                    put.Add(pt);
                }
                var k = route.TotalDistance;
                return new RouteMapping(new GMapRoute(put), route.TotalDistance);
            }
        }
    }
}
