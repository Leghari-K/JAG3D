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

namespace org.applied_geodesy.util
{
	public sealed class CellValueType
	{
		public static readonly CellValueType OBJECT = new CellValueType("OBJECT", InnerEnum.OBJECT, 0);
		public static readonly CellValueType BOOLEAN = new CellValueType("BOOLEAN", InnerEnum.BOOLEAN, 1);
		public static readonly CellValueType INTEGER = new CellValueType("INTEGER", InnerEnum.INTEGER, 2);
		public static readonly CellValueType DOUBLE = new CellValueType("DOUBLE", InnerEnum.DOUBLE, 3);
		public static readonly CellValueType STRING = new CellValueType("STRING", InnerEnum.STRING, 4);
		public static readonly CellValueType DATE = new CellValueType("DATE", InnerEnum.DATE, 5);
		public static readonly CellValueType IMAGE = new CellValueType("IMAGE", InnerEnum.IMAGE, 6);

		public static readonly CellValueType ANGLE = new CellValueType("ANGLE", InnerEnum.ANGLE, 10);
		public static readonly CellValueType ANGLE_UNCERTAINTY = new CellValueType("ANGLE_UNCERTAINTY", InnerEnum.ANGLE_UNCERTAINTY, 11);
		public static readonly CellValueType ANGLE_RESIDUAL = new CellValueType("ANGLE_RESIDUAL", InnerEnum.ANGLE_RESIDUAL, 12);

		public static readonly CellValueType LENGTH = new CellValueType("LENGTH", InnerEnum.LENGTH, 20);
		public static readonly CellValueType LENGTH_UNCERTAINTY = new CellValueType("LENGTH_UNCERTAINTY", InnerEnum.LENGTH_UNCERTAINTY, 21);
		public static readonly CellValueType LENGTH_RESIDUAL = new CellValueType("LENGTH_RESIDUAL", InnerEnum.LENGTH_RESIDUAL, 22);

		public static readonly CellValueType SCALE = new CellValueType("SCALE", InnerEnum.SCALE, 30);
		public static readonly CellValueType SCALE_UNCERTAINTY = new CellValueType("SCALE_UNCERTAINTY", InnerEnum.SCALE_UNCERTAINTY, 31);
		public static readonly CellValueType SCALE_RESIDUAL = new CellValueType("SCALE_RESIDUAL", InnerEnum.SCALE_RESIDUAL, 32);

		public static readonly CellValueType VECTOR = new CellValueType("VECTOR", InnerEnum.VECTOR, 40);
		public static readonly CellValueType VECTOR_UNCERTAINTY = new CellValueType("VECTOR_UNCERTAINTY", InnerEnum.VECTOR_UNCERTAINTY, 41);
		public static readonly CellValueType VECTOR_RESIDUAL = new CellValueType("VECTOR_RESIDUAL", InnerEnum.VECTOR_RESIDUAL, 42);

		public static readonly CellValueType STATISTIC = new CellValueType("STATISTIC", InnerEnum.STATISTIC, 50);
		public static readonly CellValueType PERCENTAGE = new CellValueType("PERCENTAGE", InnerEnum.PERCENTAGE, 51);

		public static readonly CellValueType TEMPERATURE = new CellValueType("TEMPERATURE", InnerEnum.TEMPERATURE, 60);
		public static readonly CellValueType PRESSURE = new CellValueType("PRESSURE", InnerEnum.PRESSURE, 61);

		private static readonly List<CellValueType> valueList = new List<CellValueType>();

		static CellValueType()
		{
			valueList.Add(OBJECT);
			valueList.Add(BOOLEAN);
			valueList.Add(INTEGER);
			valueList.Add(DOUBLE);
			valueList.Add(STRING);
			valueList.Add(DATE);
			valueList.Add(IMAGE);
			valueList.Add(ANGLE);
			valueList.Add(ANGLE_UNCERTAINTY);
			valueList.Add(ANGLE_RESIDUAL);
			valueList.Add(LENGTH);
			valueList.Add(LENGTH_UNCERTAINTY);
			valueList.Add(LENGTH_RESIDUAL);
			valueList.Add(SCALE);
			valueList.Add(SCALE_UNCERTAINTY);
			valueList.Add(SCALE_RESIDUAL);
			valueList.Add(VECTOR);
			valueList.Add(VECTOR_UNCERTAINTY);
			valueList.Add(VECTOR_RESIDUAL);
			valueList.Add(STATISTIC);
			valueList.Add(PERCENTAGE);
			valueList.Add(TEMPERATURE);
			valueList.Add(PRESSURE);
		}

		public enum InnerEnum
		{
			OBJECT,
			BOOLEAN,
			INTEGER,
			DOUBLE,
			STRING,
			DATE,
			IMAGE,
			ANGLE,
			ANGLE_UNCERTAINTY,
			ANGLE_RESIDUAL,
			LENGTH,
			LENGTH_UNCERTAINTY,
			LENGTH_RESIDUAL,
			SCALE,
			SCALE_UNCERTAINTY,
			SCALE_RESIDUAL,
			VECTOR,
			VECTOR_UNCERTAINTY,
			VECTOR_RESIDUAL,
			STATISTIC,
			PERCENTAGE,
			TEMPERATURE,
			PRESSURE
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private CellValueType(string name, InnerEnum innerEnum, int id)
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

		public static CellValueType getEnumByValue(int value)
		{
			foreach (CellValueType element in CellValueType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static CellValueType[] values()
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

		public static CellValueType valueOf(string name)
		{
			foreach (CellValueType enumInstance in CellValueType.valueList)
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