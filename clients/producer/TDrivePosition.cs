using System;
using System.Collections.Generic;
using System.Text;

namespace producer
{
    public class TDrivePosition
    {
        public string Name;
        public DateTime DateTime;
        public double Latitude;
        public double Longtitude;
        public double Distance;
        public TDrivePosition(string des)
        {
            var items = des.Split(',');
            Name = items[0];
            DateTime = DateTime.Parse(items[1]);
            Latitude = Double.Parse(items[2]);
            Longtitude = Double.Parse(items[3]);
        }
    }
}
