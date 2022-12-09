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

	public class ScaleUnit : Unit
	{
		public static ScaleUnit UNITLESS = new ScaleUnit(UnitType.UNITLESS);
		public static ScaleUnit PARTS_PER_MILLION_WRT_ZERO = new ScaleUnit(UnitType.PARTS_PER_MILLION_WRT_ZERO);
		public static ScaleUnit PARTS_PER_MILLION_WRT_ONE = new ScaleUnit(UnitType.PARTS_PER_MILLION_WRT_ONE);

		public static readonly IDictionary<UnitType, ScaleUnit> UNITS;
		static ScaleUnit()
		{
			UNITS = new LinkedHashMap<UnitType, ScaleUnit>();
			UNITS[UnitType.UNITLESS] = UNITLESS;
			UNITS[UnitType.PARTS_PER_MILLION_WRT_ZERO] = PARTS_PER_MILLION_WRT_ZERO;
			UNITS[UnitType.PARTS_PER_MILLION_WRT_ONE] = PARTS_PER_MILLION_WRT_ONE;
		}

		private double a = 0.0, s = 1.0;

		private ScaleUnit(UnitType type) : base(type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.UNITLESS:
				this.name = i18n.getString("Unit.unitless.name", "Unitless");
				this.abbreviation = i18n.getString("Unit.unitless.abbreviation", "");
				this.a = 0.0;
				this.s = 1.0;
				break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.PARTS_PER_MILLION_WRT_ZERO:
				this.name = i18n.getString("Unit.ppm.one.name", "Parts per million");
				this.abbreviation = i18n.getString("Unit.ppm.one.abbreviation", "ppm");
				this.a = 0.0; //1.0;
				this.s = 1E-6;
				break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.PARTS_PER_MILLION_WRT_ONE:
				this.name = i18n.getString("Unit.ppm.zero.name", "Parts per million");
				this.abbreviation = i18n.getString("Unit.ppm.zero.abbreviation", "ppm");
				this.a = 1.0;
				this.s = 1E-6;
				break;
			default:
				break;
			}
		}

		public double toUnitless(double d)
		{
			return this.a + d * this.s;
		}

		public double fromUnitless(double d)
		{
			return (d - this.a) / this.s;
		}

		public static ScaleUnit getUnit(UnitType type)
		{
			return UNITS[type];
		}
	}
}