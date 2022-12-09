using System.Collections.Generic;

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

namespace org.applied_geodesy.util.unit
{

	public class LengthUnit : Unit
	{
		public static LengthUnit METER = new LengthUnit(UnitType.METER);
		public static LengthUnit MILLIMETER = new LengthUnit(UnitType.MILLIMETER);
		public static LengthUnit MICROMETER = new LengthUnit(UnitType.MICROMETER);
		public static LengthUnit NANOMETER = new LengthUnit(UnitType.NANOMETER);
		public static LengthUnit INCH = new LengthUnit(UnitType.INCH);
		public static LengthUnit FOOT = new LengthUnit(UnitType.FOOT);

		public static readonly IDictionary<UnitType, LengthUnit> UNITS;
		static LengthUnit()
		{
			UNITS = new LinkedHashMap<UnitType, LengthUnit>();
			UNITS[UnitType.METER] = METER;
			UNITS[UnitType.MILLIMETER] = MILLIMETER;
			UNITS[UnitType.MICROMETER] = MICROMETER;
			UNITS[UnitType.NANOMETER] = NANOMETER;
			UNITS[UnitType.INCH] = INCH;
			UNITS[UnitType.FOOT] = FOOT;
		}

		private double conversionFactorToMeter = 1.0;

		private LengthUnit(UnitType type) : base(type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.METER:
				this.name = i18n.getString("Unit.meter.name", "Meter");
				this.abbreviation = i18n.getString("Unit.meter.abbreviation", "m");
				this.conversionFactorToMeter = 1.0;
				break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.MILLIMETER:
				this.name = i18n.getString("Unit.millimeter.name", "Millimeter");
				this.abbreviation = i18n.getString("Unit.millimeter.abbreviation", "mm");
				this.conversionFactorToMeter = 1E-3;
				break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.MICROMETER:
				this.name = i18n.getString("Unit.micrometer.name", "Micrometer");
				this.abbreviation = i18n.getString("Unit.micrometer.abbreviation", "\u03BCm");
				this.conversionFactorToMeter = 1E-6;
				break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.NANOMETER:
				this.name = i18n.getString("Unit.nanometer.name", "Nanometer");
				this.abbreviation = i18n.getString("Unit.nanometer.abbreviation", "nm");
				this.conversionFactorToMeter = 1E-9;
				break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.INCH:
				this.name = i18n.getString("Unit.inch.name", "Inch");
				this.abbreviation = i18n.getString("Unit.inch.abbreviation", "in");
				this.conversionFactorToMeter = 0.0254;
				break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.FOOT:
				this.name = i18n.getString("Unit.foot.name", "Foot");
				this.abbreviation = i18n.getString("Unit.foot.abbreviation", "ft");
				this.conversionFactorToMeter = 0.3048;
				break;
			default:
				break;
			}
		}

		public double toMeter(double d)
		{
			return d * this.conversionFactorToMeter;
		}

		public double fromMeter(double d)
		{
			return d / this.conversionFactorToMeter;
		}

		public static LengthUnit getUnit(UnitType type)
		{
			return UNITS[type];
		}
	}

}