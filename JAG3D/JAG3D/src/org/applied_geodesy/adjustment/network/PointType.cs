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

namespace org.applied_geodesy.adjustment.network
{
	public sealed class PointType
	{
		public static readonly PointType REFERENCE_POINT = new PointType("REFERENCE_POINT", InnerEnum.REFERENCE_POINT, 1);
		public static readonly PointType STOCHASTIC_POINT = new PointType("STOCHASTIC_POINT", InnerEnum.STOCHASTIC_POINT, 2);
		public static readonly PointType DATUM_POINT = new PointType("DATUM_POINT", InnerEnum.DATUM_POINT, 3);
		public static readonly PointType NEW_POINT = new PointType("NEW_POINT", InnerEnum.NEW_POINT, 4);

		private static readonly List<PointType> valueList = new List<PointType>();

		static PointType()
		{
			valueList.Add(REFERENCE_POINT);
			valueList.Add(STOCHASTIC_POINT);
			valueList.Add(DATUM_POINT);
			valueList.Add(NEW_POINT);
		}

		public enum InnerEnum
		{
			REFERENCE_POINT,
			STOCHASTIC_POINT,
			DATUM_POINT,
			NEW_POINT
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private PointType(string name, InnerEnum innerEnum, int id)
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
				return id;
			}
		}

		public static PointType getEnumByValue(int value)
		{
			foreach (PointType element in PointType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static PointType[] values()
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

		public static PointType valueOf(string name)
		{
			foreach (PointType enumInstance in PointType.valueList)
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