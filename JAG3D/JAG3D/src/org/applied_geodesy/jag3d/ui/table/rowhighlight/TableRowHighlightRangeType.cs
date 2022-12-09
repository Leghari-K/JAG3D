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

namespace org.applied_geodesy.jag3d.ui.table.rowhighlight
{
	public sealed class TableRowHighlightRangeType
	{
		public static readonly TableRowHighlightRangeType NONE = new TableRowHighlightRangeType("NONE", InnerEnum.NONE, 0);
		public static readonly TableRowHighlightRangeType INADEQUATE = new TableRowHighlightRangeType("INADEQUATE", InnerEnum.INADEQUATE, 1);
		public static readonly TableRowHighlightRangeType SATISFACTORY = new TableRowHighlightRangeType("SATISFACTORY", InnerEnum.SATISFACTORY, 2);
		public static readonly TableRowHighlightRangeType EXCELLENT = new TableRowHighlightRangeType("EXCELLENT", InnerEnum.EXCELLENT, 3);

		private static readonly List<TableRowHighlightRangeType> valueList = new List<TableRowHighlightRangeType>();

		static TableRowHighlightRangeType()
		{
			valueList.Add(NONE);
			valueList.Add(INADEQUATE);
			valueList.Add(SATISFACTORY);
			valueList.Add(EXCELLENT);
		}

		public enum InnerEnum
		{
			NONE,
			INADEQUATE,
			SATISFACTORY,
			EXCELLENT
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;
		private int id;
		private TableRowHighlightRangeType(string name, InnerEnum innerEnum, int id)
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

		public static TableRowHighlightRangeType getEnumByValue(int value)
		{
			foreach (TableRowHighlightRangeType element in TableRowHighlightRangeType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return NONE;
		}

		public static TableRowHighlightRangeType[] values()
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

		public static TableRowHighlightRangeType valueOf(string name)
		{
			foreach (TableRowHighlightRangeType enumInstance in TableRowHighlightRangeType.valueList)
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