using System;

/// <summary>
///*********************************************************************
/// Copyright by Michael Loesler, https://software.applied-geodesy.org   *
///                                                                      *
/// This program is free software; you can redistribute it and/or modify *
/// it under the terms of the GNU General Public License as published by *
/// the Free Software Foundation; either version 3 of the License, or    *
/// at your option any later version.                                    *
///                                                                      *
/// This program is distributed in the hope that it will be useful,      *
/// but WITHOUT ANY WARRANTY; without even the implied warranty of       *
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
/// GNU General Public License for more details.                         *
///                                                                      *
/// You should have received a copy of the GNU General Public License    *
/// along with this program; if not, see <http://www.gnu.org/licenses/>  *
/// or write to the                                                      *
/// Free Software Foundation, Inc.,                                      *
/// 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.            *
///                                                                      *
/// **********************************************************************
/// </summary>

namespace org.applied_geodesy.adjustment.network.approximation.bundle.point
{
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;

	public abstract class ClassicGeodeticComputation
	{

		/// <summary>
		/// Berechnet den Richtungswinkel zwischen zwei Punkten. </summary>
		/// <param name="startPoint"> Standpunkt </param>
		/// <param name="endPoint"> Zielpunkt </param>
		/// <returns> direction Richtungswinkel </returns>
		public static double DIRECTION(Point2D startPoint, Point2D endPoint)
		{
			return MathExtension.MOD(Math.Atan2((endPoint.Y - startPoint.Y), (endPoint.X - startPoint.X)), 2.0 * Math.PI);
		}

		/// <summary>
		/// Bestimmt einen 2D-Polarpunkt aus einem horizontalen Winkel und einer 2D-Strecke </summary>
		/// <param name="station"> </param>
		/// <param name="id"> </param>
		/// <param name="epsilon"> </param>
		/// <param name="dist2d"> </param>
		/// <returns> point2d </returns>
		public static Point2D POLAR(Point2D station, string id, double epsilon, double dist2d)
		{
			double y = station.Y + dist2d * Math.Sin(epsilon);
			double x = station.X + dist2d * Math.Cos(epsilon);
			return new Point2D(id, x, y);
		}

		/// <summary>
		/// Bestimmt einen 2D-Polarpunkt aus einem horizontalen Winkel und einer 2D-Strecke,
		/// die aus der 3D-Strecke und dem Zenitwinkel abgeleitet wird. </summary>
		/// <param name="station"> </param>
		/// <param name="id"> </param>
		/// <param name="epsilon"> </param>
		/// <param name="dist3d"> </param>
		/// <param name="zenith"> </param>
		/// <returns> point2d </returns>
		public static Point2D POLAR(Point2D station, string id, double epsilon, double dist3d, double zenith)
		{
			double dist2d = ClassicGeodeticComputation.DISTANCE2D(dist3d, zenith);
			return ClassicGeodeticComputation.POLAR(station, id, epsilon, dist2d);
		}

		/// <summary>
		/// Liefert die 3D-Strecke, die sich auf das Niveau des Zenitwinkels bezieht </summary>
		/// <param name="dist3d"> </param>
		/// <param name="ihDist"> </param>
		/// <param name="thDist"> </param>
		/// <param name="zenith"> </param>
		/// <param name="ihZenith"> </param>
		/// <param name="thZenith"> </param>
		/// <returns> slopeDist </returns>
		public static double SLOPEDISTANCE(double dist3d, double ihDist, double thDist, double zenith, double ihZenith, double thZenith)
		{
			if (zenith > Math.PI)
			{
				zenith = 2.0 * Math.PI - zenith;
			}
			double dh = thZenith - (ihZenith - ihDist) - thDist;
			double alpha = Math.Asin(dh * Math.Sin(zenith) / Math.Abs(dist3d));
			double beta = Math.PI - alpha - zenith;

			return Math.Sqrt(dist3d * dist3d + dh * dh - 2.0 * dist3d * dh * Math.Cos(beta));
		}

		/// <summary>
		/// Berechnet den trigon. Hoehenunterschied aus 3D-Strecke und Zenitwinkel unter
		/// Beruecksichtungung der Stand- und Zielpunkthoehe </summary>
		/// <param name="station"> </param>
		/// <param name="id"> </param>
		/// <param name="dist3d"> </param>
		/// <param name="zenith"> </param>
		/// <param name="ih"> </param>
		/// <param name="th"> </param>
		/// <returns> point1d </returns>
		public static Point1D TRIGO_HEIGHT_3D(Point1D station, string id, double dist3d, double zenith, double ih, double th)
		{
			if (zenith > Math.PI)
			{
				zenith = 2.0 * Math.PI - zenith;
			}
			return new Point1D(id, station.Z + Math.Abs(dist3d) * Math.Cos(zenith) + ih - th);
		}

		/// <summary>
		/// Berechnet den trigon. Hoehenunterschied aus 2D-Strecke und Zenitwinkel unter
		/// Beruecksichtungung der Stand- und Zielpunkthoehe beim Zenitwinkel </summary>
		/// <param name="station"> </param>
		/// <param name="id"> </param>
		/// <param name="dist2d"> </param>
		/// <param name="zenith"> </param>
		/// <param name="ih"> </param>
		/// <param name="th"> </param>
		/// <returns> point1d </returns>
		public static Point1D TRIGO_HEIGHT_2D(Point1D station, string id, double dist2d, double zenith, double ih, double th)
		{
			if (zenith > Math.PI)
			{
				zenith = 2.0 * Math.PI - zenith;
			}
			double dh = zenith == 0.0 || zenith == Math.PI ? 0.0 : Math.Abs(dist2d) * Math.Cos(zenith) / Math.Sin(zenith);
			return new Point1D(id, station.Z + dh + ih - th);
		}

		/// <summary>
		/// Liefert den geo. Hoehenunterschied </summary>
		/// <param name="station"> </param>
		/// <param name="id"> </param>
		/// <param name="deltaH"> </param>
		/// <param name="ih"> </param>
		/// <param name="th"> </param>
		/// <returns> point 1d </returns>
		public static Point1D ORTHO_HEIGHT(Point1D station, string id, double deltaH, double ih, double th)
		{
			return new Point1D(id, station.Z + deltaH + ih - th);
		}

		/// <summary>
		/// Berechnet den trigon. Hoehenunterschied aus 3D-Strecke und Zenitwinkel unter
		/// Beruecksichtungung der Stand- und Zielpunkthoehe
		/// </summary>
		/// <param name="dist3d"> </param>
		/// <param name="zenith"> </param>
		/// <param name="ih"> </param>
		/// <param name="th"> </param>
		/// <returns> h </returns>
		public static double TRIGO_HEIGHT_3D(double dist3d, double zenith, double ih, double th)
		{
			if (zenith > Math.PI)
			{
				zenith = 2.0 * Math.PI - zenith;
			}
			return Math.Abs(dist3d) * Math.Cos(zenith) + ih - th;
		}

		/// <summary>
		/// Berechnet den trigon. Hoehenunterschied aus 2D-Strecke und Zenitwinkel unter
		/// Beruecksichtungung der Stand- und Zielpunkthoehe fuer den Zenitwinkel
		/// </summary>
		/// <param name="dist2d"> </param>
		/// <param name="zenith"> </param>
		/// <param name="ih"> </param>
		/// <param name="th"> </param>
		/// <returns> h </returns>
		public static double TRIGO_HEIGHT_2D(double dist2d, double zenith, double ih, double th)
		{
			if (zenith > Math.PI)
			{
				zenith = 2.0 * Math.PI - zenith;
			}
			return zenith == 0.0 || zenith == Math.PI ? 0.0 : Math.Abs(dist2d) * Math.Cos(zenith) / Math.Sin(zenith) + ih - th;
		}

		/// <summary>
		/// Liefert den geo. Hoehenunterschied </summary>
		/// <param name="deltaH"> </param>
		/// <param name="ih"> </param>
		/// <param name="th"> </param>
		/// <returns> h </returns>
		public static double ORTHO_HEIGHT(double deltaH, double ih, double th)
		{
			return deltaH + ih - th;
		}

		/// <summary>
		/// Liefert die 2D-Strecke, die sich aus der Raumstrecke und dem Zenitwinkel ergibt. </summary>
		/// <param name="dist3d"> </param>
		/// <param name="zenith"> </param>
		/// <returns> dist2d </returns>
		public static double DISTANCE2D(double dist3d, double zenith)
		{
			if (zenith > Math.PI)
			{
				zenith = 2.0 * Math.PI - zenith;
			}
			return Math.Abs(dist3d) * Math.Sin(zenith);
		}
	}

}