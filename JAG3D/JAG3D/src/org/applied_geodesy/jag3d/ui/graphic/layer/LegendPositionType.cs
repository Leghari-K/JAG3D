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

namespace org.applied_geodesy.jag3d.ui.graphic.layer
{
	public sealed class LegendPositionType
	{
		public static readonly LegendPositionType NORTH = new LegendPositionType("NORTH", InnerEnum.NORTH, 1);
		public static readonly LegendPositionType SOUTH = new LegendPositionType("SOUTH", InnerEnum.SOUTH, 2);
		public static readonly LegendPositionType EAST = new LegendPositionType("EAST", InnerEnum.EAST, 3);
		public static readonly LegendPositionType WEST = new LegendPositionType("WEST", InnerEnum.WEST, 4);
		public static readonly LegendPositionType NORTH_EAST = new LegendPositionType("NORTH_EAST", InnerEnum.NORTH_EAST, 13);
		public static readonly LegendPositionType NORTH_WEST = new LegendPositionType("NORTH_WEST", InnerEnum.NORTH_WEST, 14);
		public static readonly LegendPositionType SOUTH_EAST = new LegendPositionType("SOUTH_EAST", InnerEnum.SOUTH_EAST, 23);
		public static readonly LegendPositionType SOUTH_WEST = new LegendPositionType("SOUTH_WEST", InnerEnum.SOUTH_WEST, 24);

		private static readonly List<LegendPositionType> valueList = new List<LegendPositionType>();

		static LegendPositionType()
		{
			valueList.Add(NORTH);
			valueList.Add(SOUTH);
			valueList.Add(EAST);
			valueList.Add(WEST);
			valueList.Add(NORTH_EAST);
			valueList.Add(NORTH_WEST);
			valueList.Add(SOUTH_EAST);
			valueList.Add(SOUTH_WEST);
		}

		public enum InnerEnum
		{
			NORTH,
			SOUTH,
			EAST,
			WEST,
			NORTH_EAST,
			NORTH_WEST,
			SOUTH_EAST,
			SOUTH_WEST
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private LegendPositionType(string name, InnerEnum innerEnum, int id)
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

		public static LegendPositionType getEnumByValue(int value)
		{
			foreach (LegendPositionType element in LegendPositionType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static LegendPositionType[] values()
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

		public static LegendPositionType valueOf(string name)
		{
			foreach (LegendPositionType enumInstance in LegendPositionType.valueList)
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