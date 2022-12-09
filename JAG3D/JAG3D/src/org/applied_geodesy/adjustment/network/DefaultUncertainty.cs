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

namespace org.applied_geodesy.adjustment.network
{

	using Constant = org.applied_geodesy.adjustment.Constant;

	public sealed class DefaultUncertainty
	{
		private static readonly double ANGLE_ZERO_POINT_OFFSET = 0.0003 * Constant.RHO_GRAD2RAD;
		private const double ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT = 0.0;
		private const double ANGLE_DISTANCE_DEPENDENT = 0.0005;

		private const double DISTANCE_ZERO_POINT_OFFSET = 0.002;
		private const double DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT = 0.0;
		private const double DISTANCE_DISTANCE_DEPENDENT = 0.000002;

		private const double LEVELING_ZERO_POINT_OFFSET = 0.0001;
		private const double LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT = 0.001;
		private const double LEVELING_DISTANCE_DEPENDENT = 0.0;

		private const double GNSS_ZERO_POINT_OFFSET = 0.025;
		private const double GNSS_SQUARE_ROOT_DISTANCE_DEPENDENT = 0.0;
		private const double GNSS_DISTANCE_DEPENDENT = 0.000050;

		private const double X = 0.002;
		private const double Y = 0.002;
		private const double Z = 0.002;

		private static readonly double DEFLECTION_X = 0.001 * Constant.RHO_GRAD2RAD;
		private static readonly double DEFLECTION_Y = 0.001 * Constant.RHO_GRAD2RAD;

		private static readonly Properties PROPERTIES = new Properties();

		static DefaultUncertainty()
		{
			BufferedInputStream bis = null;
			const string path = "properties/uncertainties.default";
			try
			{
				if (typeof(DefaultUncertainty).getClassLoader().getResourceAsStream(path) != null)
				{
					bis = new BufferedInputStream(typeof(DefaultUncertainty).getClassLoader().getResourceAsStream(path));
					PROPERTIES.load(bis);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				try
				{
					if (bis != null)
					{
						bis.close();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		private DefaultUncertainty()
		{
		}

		public static double UncertaintyAngleZeroPointOffset
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("ANGLE_ZERO_POINT_OFFSET"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : ANGLE_ZERO_POINT_OFFSET;
			}
		}
		public static double UncertaintyAngleSquareRootDistanceDependent
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT;
			}
		}
		public static double UncertaintyAngleDistanceDependent
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("ANGLE_DISTANCE_DEPENDENT"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : ANGLE_DISTANCE_DEPENDENT;
			}
		}
		public static double UncertaintyDistanceZeroPointOffset
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("DISTANCE_ZERO_POINT_OFFSET"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : DISTANCE_ZERO_POINT_OFFSET;
			}
		}
		public static double UncertaintyDistanceSquareRootDistanceDependent
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT;
			}
		}
		public static double UncertaintyDistanceDistanceDependent
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("DISTANCE_DISTANCE_DEPENDENT"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : DISTANCE_DISTANCE_DEPENDENT;
			}
		}
		public static double UncertaintyLevelingZeroPointOffset
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("LEVELING_ZERO_POINT_OFFSET"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : LEVELING_ZERO_POINT_OFFSET;
			}
		}
		public static double UncertaintyLevelingSquareRootDistanceDependent
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT;
			}
		}
		public static double UncertaintyLevelingDistanceDependent
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("LEVELING_DISTANCE_DEPENDENT"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : LEVELING_DISTANCE_DEPENDENT;
			}
		}
		public static double UncertaintyGNSSZeroPointOffset
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("GNSS_ZERO_POINT_OFFSET"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : GNSS_ZERO_POINT_OFFSET;
			}
		}
		public static double UncertaintyGNSSSquareRootDistanceDependent
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("GNSS_SQUARE_ROOT_DISTANCE_DEPENDENT"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : GNSS_SQUARE_ROOT_DISTANCE_DEPENDENT;
			}
		}
		public static double UncertaintyGNSSDistanceDependent
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("GNSS_DISTANCE_DEPENDENT"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : GNSS_DISTANCE_DEPENDENT;
			}
		}
		public static double UncertaintyX
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("X"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : X;
			}
		}
		public static double UncertaintyY
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("Y"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : Y;
			}
		}
		public static double UncertaintyZ
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("Z"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : Z;
			}
		}
		public static double UncertaintyDeflectionX
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("DEFLECTION_X"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : DEFLECTION_X;
			}
		}
		public static double UncertaintyDeflectionY
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("DEFLECTION_Y"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : DEFLECTION_Y;
			}
		}
	}

}