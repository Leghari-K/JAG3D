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
	public sealed class UnitType
	{
		public static readonly UnitType METER = new UnitType("METER", InnerEnum.METER, 10);
		public static readonly UnitType MILLIMETER = new UnitType("MILLIMETER", InnerEnum.MILLIMETER, 11);
		public static readonly UnitType MICROMETER = new UnitType("MICROMETER", InnerEnum.MICROMETER, 12);
		public static readonly UnitType INCH = new UnitType("INCH", InnerEnum.INCH, 13);
		public static readonly UnitType FOOT = new UnitType("FOOT", InnerEnum.FOOT, 14);
		public static readonly UnitType NANOMETER = new UnitType("NANOMETER", InnerEnum.NANOMETER, 15);

		public static readonly UnitType UNITLESS = new UnitType("UNITLESS", InnerEnum.UNITLESS, 20);
		public static readonly UnitType PARTS_PER_MILLION_WRT_ONE = new UnitType("PARTS_PER_MILLION_WRT_ONE", InnerEnum.PARTS_PER_MILLION_WRT_ONE, 21);
		public static readonly UnitType PARTS_PER_MILLION_WRT_ZERO = new UnitType("PARTS_PER_MILLION_WRT_ZERO", InnerEnum.PARTS_PER_MILLION_WRT_ZERO, 22);
		public static readonly UnitType PERCENT = new UnitType("PERCENT", InnerEnum.PERCENT, 23);

		public static readonly UnitType RADIAN = new UnitType("RADIAN", InnerEnum.RADIAN, 30);
		public static readonly UnitType DEGREE = new UnitType("DEGREE", InnerEnum.DEGREE, 31);
		public static readonly UnitType GRADIAN = new UnitType("GRADIAN", InnerEnum.GRADIAN, 32);
		public static readonly UnitType MILLIRADIAN = new UnitType("MILLIRADIAN", InnerEnum.MILLIRADIAN, 33);
		public static readonly UnitType ARCSECOND = new UnitType("ARCSECOND", InnerEnum.ARCSECOND, 34);
		public static readonly UnitType MILLIGRADIAN = new UnitType("MILLIGRADIAN", InnerEnum.MILLIGRADIAN, 35);
		public static readonly UnitType MIL6400 = new UnitType("MIL6400", InnerEnum.MIL6400, 36);
		public static readonly UnitType DEGREE_SEXAGESIMAL = new UnitType("DEGREE_SEXAGESIMAL", InnerEnum.DEGREE_SEXAGESIMAL, 37);

		public static readonly UnitType DEGREE_CELSIUS = new UnitType("DEGREE_CELSIUS", InnerEnum.DEGREE_CELSIUS, 50);
		public static readonly UnitType KELVIN = new UnitType("KELVIN", InnerEnum.KELVIN, 51);
		public static readonly UnitType DEGREE_FAHRENHEIT = new UnitType("DEGREE_FAHRENHEIT", InnerEnum.DEGREE_FAHRENHEIT, 52);

		public static readonly UnitType PASCAL = new UnitType("PASCAL", InnerEnum.PASCAL, 60);
		public static readonly UnitType HECTOPASCAL = new UnitType("HECTOPASCAL", InnerEnum.HECTOPASCAL, 61);
		public static readonly UnitType KILOPASCAL = new UnitType("KILOPASCAL", InnerEnum.KILOPASCAL, 62);
		public static readonly UnitType BAR = new UnitType("BAR", InnerEnum.BAR, 63);
		public static readonly UnitType MILLIBAR = new UnitType("MILLIBAR", InnerEnum.MILLIBAR, 64);
		public static readonly UnitType TORR = new UnitType("TORR", InnerEnum.TORR, 65);

		private static readonly List<UnitType> valueList = new List<UnitType>();

		static UnitType()
		{
			valueList.Add(METER);
			valueList.Add(MILLIMETER);
			valueList.Add(MICROMETER);
			valueList.Add(INCH);
			valueList.Add(FOOT);
			valueList.Add(NANOMETER);
			valueList.Add(UNITLESS);
			valueList.Add(PARTS_PER_MILLION_WRT_ONE);
			valueList.Add(PARTS_PER_MILLION_WRT_ZERO);
			valueList.Add(PERCENT);
			valueList.Add(RADIAN);
			valueList.Add(DEGREE);
			valueList.Add(GRADIAN);
			valueList.Add(MILLIRADIAN);
			valueList.Add(ARCSECOND);
			valueList.Add(MILLIGRADIAN);
			valueList.Add(MIL6400);
			valueList.Add(DEGREE_SEXAGESIMAL);
			valueList.Add(DEGREE_CELSIUS);
			valueList.Add(KELVIN);
			valueList.Add(DEGREE_FAHRENHEIT);
			valueList.Add(PASCAL);
			valueList.Add(HECTOPASCAL);
			valueList.Add(KILOPASCAL);
			valueList.Add(BAR);
			valueList.Add(MILLIBAR);
			valueList.Add(TORR);
		}

		public enum InnerEnum
		{
			METER,
			MILLIMETER,
			MICROMETER,
			INCH,
			FOOT,
			NANOMETER,
			UNITLESS,
			PARTS_PER_MILLION_WRT_ONE,
			PARTS_PER_MILLION_WRT_ZERO,
			PERCENT,
			RADIAN,
			DEGREE,
			GRADIAN,
			MILLIRADIAN,
			ARCSECOND,
			MILLIGRADIAN,
			MIL6400,
			DEGREE_SEXAGESIMAL,
			DEGREE_CELSIUS,
			KELVIN,
			DEGREE_FAHRENHEIT,
			PASCAL,
			HECTOPASCAL,
			KILOPASCAL,
			BAR,
			MILLIBAR,
			TORR
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private UnitType(string name, InnerEnum innerEnum, int id)
		{
			this.id = id;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public int Id
		{
			get
			{
				return this.id;
			}
		}

		public static UnitType getEnumByValue(int value)
		{
			foreach (UnitType element in UnitType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static UnitType[] values()
		{
			return valueList.ToArray();
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static UnitType valueOf(string name)
		{
			foreach (UnitType enumInstance in UnitType.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}