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

	public class DefaultAverageThreshold
	{
		private static readonly double LEVELING = DefaultUncertainty.UncertaintyLevelingZeroPointOffset * 10.0;

		private static readonly double DIRECTION = DefaultUncertainty.UncertaintyAngleZeroPointOffset * 10.0;
		private static readonly double ZENITH_ANGLE = DefaultUncertainty.UncertaintyAngleZeroPointOffset * 10.0;

		private static readonly double HORIZONTAL_DISTANCE = DefaultUncertainty.UncertaintyDistanceZeroPointOffset * 10.0;
		private static readonly double SLOPE_DISTANCE = DefaultUncertainty.UncertaintyDistanceZeroPointOffset * 10.0;

		private static readonly double GNSS1D = DefaultUncertainty.UncertaintyGNSSZeroPointOffset * 10.0;
		private static readonly double GNSS2D = DefaultUncertainty.UncertaintyGNSSZeroPointOffset * 10.0;
		private static readonly double GNSS3D = DefaultUncertainty.UncertaintyGNSSZeroPointOffset * 10.0;

		private static readonly Properties PROPERTIES = new Properties();

		static DefaultAverageThreshold()
		{
			BufferedInputStream bis = null;
			const string path = "properties/averagethresholds.default";
			try
			{
				if (typeof(DefaultAverageThreshold).getClassLoader().getResourceAsStream(path) != null)
				{
					bis = new BufferedInputStream(typeof(DefaultAverageThreshold).getClassLoader().getResourceAsStream(path));
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

		private DefaultAverageThreshold()
		{
		}

		public static double ThresholdLeveling
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("LEVELING"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : LEVELING;
			}
		}

		public static double ThresholdDirection
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("DIRECTION"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : DIRECTION;
			}
		}

		public static double ThresholdZenithAngle
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("ZENITH_ANGLE"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : ZENITH_ANGLE;
			}
		}

		public static double ThresholdHorizontalDistance
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("HORIZONTAL_DISTANCE"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : HORIZONTAL_DISTANCE;
			}
		}

		public static double ThresholdSlopeDistance
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("SLOPE_DISTANCE"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : SLOPE_DISTANCE;
			}
		}

		public static double ThresholdGNSSBaseline1D
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("GNSS1D"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : GNSS1D;
			}
		}

		public static double ThresholdGNSSBaseline2D
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("GNSS2D"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : GNSS2D;
			}
		}

		public static double ThresholdGNSSBaseline3D
		{
			get
			{
				double value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty("GNSS3D"));
				}
				catch (Exception)
				{
				}
				return value > 0 ? value : GNSS3D;
			}
		}

		public static double getThreshold(ObservationType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.LEVELING:
				return ThresholdLeveling;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.DIRECTION:
				return ThresholdDirection;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				return ThresholdHorizontalDistance;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.SLOPE_DISTANCE:
				return ThresholdSlopeDistance;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.ZENITH_ANGLE:
				return ThresholdZenithAngle;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS1D:
				return ThresholdGNSSBaseline1D;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS2D:
				return ThresholdGNSSBaseline2D;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS3D:
				return ThresholdGNSSBaseline3D;
			}
			return 0;
		}
	}

}