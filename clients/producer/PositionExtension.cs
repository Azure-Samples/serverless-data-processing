using System;
using System.Collections.Generic;
using System.Text;

namespace producer
{
    public static class PositionExtensions
    {
        private const double EARTH_RADIUS = 6371004; //km

        static public double DistanceTo(this TDrivePosition currentPosition, TDrivePosition position)
        {
            return DistanceBetweenPoints(currentPosition, position);
        }
        private static double DegToRad(double x)
        {
            return x * Math.PI / 180;
        }
        private static double DistanceBetweenPoints(TDrivePosition point1, TDrivePosition point2)
        {
            double dLat = DegToRad(point2.Latitude - point1.Latitude);
            double dLon = DegToRad(point2.Longtitude - point1.Longtitude);
            double lat1 = DegToRad(point1.Latitude);
            double lat2 = DegToRad(point2.Latitude);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Pow(Math.Sin(dLon / 2), 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = Math.Round(EARTH_RADIUS * c * 10) / 10;

            return d;
        }
    }
}
