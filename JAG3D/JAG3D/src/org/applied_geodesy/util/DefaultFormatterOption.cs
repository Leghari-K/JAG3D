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

namespace org.applied_geodesy.util
{

	using AngleUnit = org.applied_geodesy.util.unit.AngleUnit;
	using LengthUnit = org.applied_geodesy.util.unit.LengthUnit;
	using PercentUnit = org.applied_geodesy.util.unit.PercentUnit;
	using PressureUnit = org.applied_geodesy.util.unit.PressureUnit;
	using ScaleUnit = org.applied_geodesy.util.unit.ScaleUnit;
	using TemperatureUnit = org.applied_geodesy.util.unit.TemperatureUnit;
	using UnitType = org.applied_geodesy.util.unit.UnitType;

	public class DefaultFormatterOption
	{

		private static readonly LengthUnit LENGTH_UNIT = LengthUnit.METER;
		private static readonly AngleUnit ANGLE_UNIT = AngleUnit.GRADIAN;
		private static readonly ScaleUnit SCALE_UNIT = ScaleUnit.PARTS_PER_MILLION_WRT_ONE;
		private static readonly LengthUnit VECTOR_UNIT = LengthUnit.METER;

		private static readonly LengthUnit LENGTH_UNCERTAINTY_UNIT = LengthUnit.MILLIMETER;
		private static readonly AngleUnit ANGLE_UNCERTAINTY_UNIT = AngleUnit.MILLIGRADIAN;
		private static readonly ScaleUnit SCALE_UNCERTAINTY_UNIT = ScaleUnit.PARTS_PER_MILLION_WRT_ZERO;
		private static readonly LengthUnit VECTOR_UNCERTAINTY_UNIT = LengthUnit.MILLIMETER;

		private static readonly LengthUnit LENGTH_RESIDUAL_UNIT = LengthUnit.MILLIMETER;
		private static readonly AngleUnit ANGLE_RESIDUAL_UNIT = AngleUnit.MILLIGRADIAN;
		private static readonly ScaleUnit SCALE_RESIDUAL_UNIT = ScaleUnit.PARTS_PER_MILLION_WRT_ZERO;
		private static readonly LengthUnit VECTOR_RESIDUAL_UNIT = LengthUnit.MILLIMETER;

		private static readonly TemperatureUnit TEMPERATURE_UNIT = TemperatureUnit.DEGREE_CELSIUS;
		private static readonly PressureUnit PRESSURE_UNIT = PressureUnit.HECTOPASCAL;
		private static readonly PercentUnit PERCENT_UNIT = PercentUnit.PERCENT;


		private const int LENGTH_FRACTION_DIGITS = 4;
		private const int ANGLE_FRACTION_DIGITS = 5;
		private const int SCALE_FRACTION_DIGITS = 2;
		private const int VECTOR_FRACTION_DIGITS = 7;

		private const int LENGTH_UNCERTAINTY_FRACTION_DIGITS = 1;
		private const int ANGLE_UNCERTAINTY_FRACTION_DIGITS = 2;
		private const int SCALE_UNCERTAINTY_FRACTION_DIGITS = 1;
		private const int VECTOR_UNCERTAINTY_FRACTION_DIGITS = 1;

		private const int LENGTH_RESIDUAL_FRACTION_DIGITS = 1;
		private const int ANGLE_RESIDUAL_FRACTION_DIGITS = 2;
		private const int SCALE_RESIDUAL_FRACTION_DIGITS = 2;
		private const int VECTOR_RESIDUAL_FRACTION_DIGITS = 2;

		private const int TEMPERATURE_FRACTION_DIGITS = 1;
		private const int PRESSURE_FRACTION_DIGITS = 2;
		private const int PERCENT_FRACTION_DIGITS = 2;

		private const int STATISTIC_FRACTION_DIGITS = 2;
		private const int DOUBLE_FRACTION_DIGITS = 5;

		private static readonly Properties PROPERTIES = new Properties();

		static DefaultFormatterOption()
		{
			BufferedInputStream bis = null;
			const string path = "properties/formatteroptions.default";
			try
			{
				if (typeof(DefaultFormatterOption).getClassLoader().getResourceAsStream(path) != null)
				{
					bis = new BufferedInputStream(typeof(DefaultFormatterOption).getClassLoader().getResourceAsStream(path));
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

		private DefaultFormatterOption()
		{
		}

		public static int LengthFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("LENGTH_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : LENGTH_FRACTION_DIGITS;
			}
		}

		public static int AngleFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("ANGLE_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : ANGLE_FRACTION_DIGITS;
			}
		}

		public static int ScaleFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("SCALE_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : SCALE_FRACTION_DIGITS;
			}
		}

		public static int VectorFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("VECTOR_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : VECTOR_FRACTION_DIGITS;
			}
		}

		public static int LengthUncertaintyFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("LENGTH_UNCERTAINTY_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : LENGTH_UNCERTAINTY_FRACTION_DIGITS;
			}
		}

		public static int AngleUncertaintyFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("ANGLE_UNCERTAINTY_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : ANGLE_UNCERTAINTY_FRACTION_DIGITS;
			}
		}

		public static int ScaleUncertaintyFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("SCALE_UNCERTAINTY_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : SCALE_UNCERTAINTY_FRACTION_DIGITS;
			}
		}

		public static int VectorUncertaintyFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("VECTOR_UNCERTAINTY_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : VECTOR_UNCERTAINTY_FRACTION_DIGITS;
			}
		}

		public static int LengthResidualFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("LENGTH_RESIDUAL_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : LENGTH_RESIDUAL_FRACTION_DIGITS;
			}
		}

		public static int AngleResidualFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("ANGLE_RESIDUAL_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : ANGLE_RESIDUAL_FRACTION_DIGITS;
			}
		}

		public static int ScaleResidualFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("SCALE_RESIDUAL_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : SCALE_RESIDUAL_FRACTION_DIGITS;
			}
		}

		public static int VectorResidualFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("VECTOR_RESIDUAL_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : VECTOR_RESIDUAL_FRACTION_DIGITS;
			}
		}

		public static int TemperatureFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("TEMPERATURE_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : TEMPERATURE_FRACTION_DIGITS;
			}
		}

		public static int PressureFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("PRESSURE_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : PRESSURE_FRACTION_DIGITS;
			}
		}

		public static int PercentFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("PERCENT_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : PERCENT_FRACTION_DIGITS;
			}
		}

		public static int StatisticFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("STATISTIC_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : STATISTIC_FRACTION_DIGITS;
			}
		}

		public static int DoubleFractionDigits
		{
			get
			{
				int value = -1;
				try
				{
					value = int.Parse(PROPERTIES.getProperty("DOUBLE_FRACTION_DIGITS"));
				}
				catch (Exception)
				{
				}
				return value >= 0 ? value : DOUBLE_FRACTION_DIGITS;
			}
		}

		public static LengthUnit LengthUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("LENGTH_UNIT"));
					if (unitType != null && LengthUnit.UNITS.ContainsKey(unitType))
					{
						return LengthUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return LENGTH_UNIT;
			}
		}

		public static AngleUnit AngleUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("ANGLE_UNIT"));
					if (unitType != null && AngleUnit.UNITS.ContainsKey(unitType))
					{
						return AngleUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return ANGLE_UNIT;
			}
		}

		public static ScaleUnit ScaleUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("SCALE_UNIT"));
					if (unitType != null && ScaleUnit.UNITS.ContainsKey(unitType))
					{
						return ScaleUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return SCALE_UNIT;
			}
		}

		public static LengthUnit VectorUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("VECTOR_UNIT"));
					if (unitType != null && LengthUnit.UNITS.ContainsKey(unitType))
					{
						return LengthUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return VECTOR_UNIT;
			}
		}

		public static LengthUnit LengthUncertaintyUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("LENGTH_UNCERTAINTY_UNIT"));
					if (unitType != null && LengthUnit.UNITS.ContainsKey(unitType))
					{
						return LengthUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return LENGTH_UNCERTAINTY_UNIT;
			}
		}

		public static AngleUnit AngleUncertaintyUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("ANGLE_UNCERTAINTY_UNIT"));
					if (unitType != null && AngleUnit.UNITS.ContainsKey(unitType))
					{
						return AngleUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return ANGLE_UNCERTAINTY_UNIT;
			}
		}

		public static ScaleUnit ScaleUncertaintyUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("SCALE_UNCERTAINTY_UNIT"));
					if (unitType != null && ScaleUnit.UNITS.ContainsKey(unitType))
					{
						return ScaleUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return SCALE_UNCERTAINTY_UNIT;
			}
		}

		public static LengthUnit VectorUncertaintyUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("VECTOR_UNCERTAINTY_UNIT"));
					if (unitType != null && LengthUnit.UNITS.ContainsKey(unitType))
					{
						return LengthUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return VECTOR_UNCERTAINTY_UNIT;
			}
		}

		public static LengthUnit LengthResidualUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("LENGTH_RESIDUAL_UNIT"));
					if (unitType != null && LengthUnit.UNITS.ContainsKey(unitType))
					{
						return LengthUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return LENGTH_RESIDUAL_UNIT;
			}
		}

		public static AngleUnit AngleResidualUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("ANGLE_RESIDUAL_UNIT"));
					if (unitType != null && AngleUnit.UNITS.ContainsKey(unitType))
					{
						return AngleUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return ANGLE_RESIDUAL_UNIT;
			}
		}

		public static ScaleUnit ScaleResidualUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("SCALE_RESIDUAL_UNIT"));
					if (unitType != null && ScaleUnit.UNITS.ContainsKey(unitType))
					{
						return ScaleUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return SCALE_RESIDUAL_UNIT;
			}
		}

		public static LengthUnit VectorResidualUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("VECTOR_RESIDUAL_UNIT"));
					if (unitType != null && LengthUnit.UNITS.ContainsKey(unitType))
					{
						return LengthUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return VECTOR_RESIDUAL_UNIT;
			}
		}

		public static TemperatureUnit TemperatureUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("TEMPERATURE_UNIT"));
					if (unitType != null && TemperatureUnit.UNITS.ContainsKey(unitType))
					{
						return TemperatureUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return TEMPERATURE_UNIT;
			}
		}

		public static PressureUnit PressureUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("PRESSURE_UNIT"));
					if (unitType != null && PressureUnit.UNITS.ContainsKey(unitType))
					{
						return PressureUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return PRESSURE_UNIT;
			}
		}

		public static PercentUnit PercentUnit
		{
			get
			{
				try
				{
					UnitType unitType = UnitType.valueOf(PROPERTIES.getProperty("PERCENT_UNIT"));
					if (unitType != null && PercentUnit.UNITS.ContainsKey(unitType))
					{
						return PercentUnit.getUnit(unitType);
					}
				}
				catch (Exception)
				{
				}
				return PERCENT_UNIT;
			}
		}
	}

}