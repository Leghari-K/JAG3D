using System;

namespace org.applied_geodesy.adjustment
{
    public sealed class Constant
    {

        private Constant()
        {
        }

        public static readonly double RHO_DEG2RAD = Math.PI / 180.0;

        public static readonly double RHO_RAD2DEG = 180.0 / Math.PI;

        public const double RHO_DEG2GRAD = 200.0 / 180.0;

        public const double RHO_GRAD2DEG = 180.0 / 200.0;

        public static readonly double RHO_GRAD2RAD = Math.PI / 200.0;

        public static readonly double RHO_RAD2GRAD = 200.0 / Math.PI;

        public static readonly double RHO_MIL2RAD = Math.PI / 3200.0;

        public const double RHO_MIL2GRAD = 200.0 / 3200.0;

        public const double RHO_MIL2DEG = 180.0 / 3200.0;

        public static readonly double RHO_RAD2MIL = 3200.0 / Math.PI;

        public const double RHO_GRAD2MIL = 3200.0 / 200.0;

        public const double RHO_DEG2MIL = 3200.0 / 180.0;


        public const double TORR2HEKTOPASCAL = 1013.25 / 760.0;

        public const double HEKTOPASCAL2TORR = 760.0 / 1013.25;


        public const double EARTH_RADIUS = 6371007.0;


        public static readonly double EPS = Constant.mashEPS();

        /// <summary>
        /// Methode zum Berechnen der relativen EPS-Genauigkeit. </summary>
        /// <returns> Relative EPS-Genauigkeit </returns>
        /// <seealso cref="Gisela Engeln-Muellges und Fritz Reutter: Formelsammlung zur Numerischen Mathematik mit C-Programmen"/>
        private static double mashEPS()
        {
            double eps = 1.0, x = 2.0, y = 1.0;
            while (y < x)
            {
                eps *= 0.5;
                x = 1.0 + eps;
            }
            return eps;
        }
    }

}