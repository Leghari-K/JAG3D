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

	public class PressureUnit : Unit
	{
		public static PressureUnit PASCAL = new PressureUnit(UnitType.PASCAL);
		public static PressureUnit HECTOPASCAL = new PressureUnit(UnitType.HECTOPASCAL);
		public static PressureUnit KILOPASCAL = new PressureUnit(UnitType.KILOPASCAL);

		public static PressureUnit BAR = new PressureUnit(UnitType.BAR);
		public static PressureUnit MILLIBAR = new PressureUnit(UnitType.MILLIBAR);
		public static PressureUnit TORR = new PressureUnit(UnitType.TORR);

		public static readonly IDictionary<UnitType, PressureUnit> UNITS;
		static PressureUnit()
		{
			UNITS = new LinkedHashMap<UnitType, PressureUnit>();
			UNITS[UnitType.PASCAL] = PASCAL;
			UNITS[UnitType.HECTOPASCAL] = HECTOPASCAL;
			UNITS[UnitType.KILOPASCAL] = KILOPASCAL;
			UNITS[UnitType.BAR] = BAR;
			UNITS[UnitType.MILLIBAR] = MILLIBAR;
			UNITS[UnitType.TORR] = TORR;
		}

		private double conversionFactorToHectopascal = 1.0;

		private PressureUnit(UnitType type) : base(type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.PASCAL:
				this.name = i18n.getString("Unit.pascal.name", "Pascal");
				this.abbreviation = i18n.getString("Unit.pascal.abbreviation", "Pa");
				this.conversionFactorToHectopascal = 100.0;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.KILOPASCAL:
				this.name = i18n.getString("Unit.kilopascal.name", "Kilopascal");
				this.abbreviation = i18n.getString("Unit.kilopascal.abbreviation", "kPa");
				this.conversionFactorToHectopascal = 0.1;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.HECTOPASCAL:
				this.name = i18n.getString("Unit.hectopascal.name", "Hectopascal");
				this.abbreviation = i18n.getString("Unit.hectopascal.abbreviation", "hPa");
				this.conversionFactorToHectopascal = 1.0;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.BAR:
				this.name = i18n.getString("Unit.bar.name", "Bar");
				this.abbreviation = i18n.getString("Unit.bar.abbreviation", "bar");
				this.conversionFactorToHectopascal = 1000.0;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.MILLIBAR:
				this.name = i18n.getString("Unit.millibar.name", "Millibar");
				this.abbreviation = i18n.getString("Unit.millibar.abbreviation", "mbar");
				this.conversionFactorToHectopascal = 1.0;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.TORR:
				this.name = i18n.getString("Unit.torr.name", "Torr");
				this.abbreviation = i18n.getString("Unit.torr.abbreviation", "Torr");
				this.conversionFactorToHectopascal = 1013.25 / 760.0;
			break;

			default:
				break;
			}
		}

		public double toHectopascal(double d)
		{
			return d * this.conversionFactorToHectopascal;
		}

		public double fromHectopascal(double d)
		{
			return d / this.conversionFactorToHectopascal;
		}

		public static PressureUnit getUnit(UnitType type)
		{
			return UNITS[type];
		}
	}

}