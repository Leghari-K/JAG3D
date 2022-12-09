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

namespace org.applied_geodesy.jag3d.ui.graphic.layer.symbol
{
	public sealed class ArrowSymbolType
	{
		public static readonly ArrowSymbolType STROKED_TRIANGLE_ARROW = new ArrowSymbolType("STROKED_TRIANGLE_ARROW", InnerEnum.STROKED_TRIANGLE_ARROW, 11);
		public static readonly ArrowSymbolType STROKED_TETRAGON_ARROW = new ArrowSymbolType("STROKED_TETRAGON_ARROW", InnerEnum.STROKED_TETRAGON_ARROW, 12);
		public static readonly ArrowSymbolType STROKED_ARROW = new ArrowSymbolType("STROKED_ARROW", InnerEnum.STROKED_ARROW, 13);

		public static readonly ArrowSymbolType FILLED_TRIANGLE_ARROW = new ArrowSymbolType("FILLED_TRIANGLE_ARROW", InnerEnum.FILLED_TRIANGLE_ARROW, 21);
		public static readonly ArrowSymbolType FILLED_TETRAGON_ARROW = new ArrowSymbolType("FILLED_TETRAGON_ARROW", InnerEnum.FILLED_TETRAGON_ARROW, 22);

		private static readonly List<ArrowSymbolType> valueList = new List<ArrowSymbolType>();

		static ArrowSymbolType()
		{
			valueList.Add(STROKED_TRIANGLE_ARROW);
			valueList.Add(STROKED_TETRAGON_ARROW);
			valueList.Add(STROKED_ARROW);
			valueList.Add(FILLED_TRIANGLE_ARROW);
			valueList.Add(FILLED_TETRAGON_ARROW);
		}

		public enum InnerEnum
		{
			STROKED_TRIANGLE_ARROW,
			STROKED_TETRAGON_ARROW,
			STROKED_ARROW,
			FILLED_TRIANGLE_ARROW,
			FILLED_TETRAGON_ARROW
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private ArrowSymbolType(string name, InnerEnum innerEnum, int id)
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

		public static ArrowSymbolType getEnumByValue(int value)
		{
			foreach (ArrowSymbolType element in ArrowSymbolType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static ArrowSymbolType[] values()
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

		public static ArrowSymbolType valueOf(string name)
		{
			foreach (ArrowSymbolType enumInstance in ArrowSymbolType.valueList)
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