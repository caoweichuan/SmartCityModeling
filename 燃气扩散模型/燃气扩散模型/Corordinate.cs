using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 燃气扩散模型
{
    class Corordinate
    {
        public double Lon;
        public double Lat;
        public double RadLon;//弧度制表示
        public double RadLat;
        public double Ec;//球面修正半径
        public double Ed;
        static double Rc = 6378137;  // 赤道半径
        static double Rj = 6356725;  // 极半径
            public   Corordinate( double lon,double lat)
            {
                Lon = lon;
                Lat = lat;
                RadLon = lon * Math.PI / 180;
                 RadLat = lat * Math.PI / 180;
                 Ec = Rj + (Rc - Rj) * (90 - lat) / 90;
                 Ed = Ec * Math.Cos(RadLat);
            }
           
    }
}
