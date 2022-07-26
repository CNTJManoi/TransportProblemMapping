using System;
using GMap.NET.WindowsPresentation;

namespace TransportProblemMapping.Models
{
    internal class RouteMapping
    {
        public RouteMapping(GMapRoute Routes, float Distance)
        {
            RouteMarkers = Routes;
            DistanceMeters = Distance;
            DistanceKilometers = (float)Math.Round(Distance / 1000f, 2);
        }

        public GMapRoute RouteMarkers { get; }
        public float DistanceMeters { get; }
        public float DistanceKilometers { get; }
    }
}