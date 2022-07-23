using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportProblemMapping.Models
{
    class RouteMapping
    {
        public GMapRoute RouteMarkers { get; private set; }
        public float DistanceMeters { get; private set; }
        public float DistanceKilometers { get; private set; }
        public RouteMapping(GMapRoute Routes, float Distance)
        {
            RouteMarkers = Routes;
            DistanceMeters = Distance;
            DistanceKilometers = (float)Math.Round(Distance / 1000, 2);
        }
    }
}
