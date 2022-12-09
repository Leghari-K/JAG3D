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