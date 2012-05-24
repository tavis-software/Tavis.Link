using System;
using Tavis;

namespace LinkTests
{
    public class BingMapLink : Link {

        public BingMapLink()
        {
            Target = new Uri("http://www.bing.com/maps/?v=2&cp={lat}~{long}&lvl={level}");
            SetParameter("level", 10);
        }

        public void SetCoordinates(double latitude, double longitude)
        {
            SetParameter("lat", latitude.ToString());
            SetParameter("long", longitude.ToString());
        }
       
        public void SetLevel(int level)
        {
            SetParameter("level", level.ToString());
        }
    }
}