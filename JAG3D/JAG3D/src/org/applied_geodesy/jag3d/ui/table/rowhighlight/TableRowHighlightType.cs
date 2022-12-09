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
	public sealed class TableRowHighlightType
	{
		public static readonly TableRowHighlightType NONE = new TableRowHighlightType("NONE", InnerEnum.NONE, 0);
		public static readonly TableRowHighlightType TEST_STATISTIC = new TableRowHighlightType("TEST_STATISTIC", InnerEnum.TEST_STATISTIC, 1);
		public static readonly TableRowHighlightType REDUNDANCY = new TableRowHighlightType("REDUNDANCY", InnerEnum.REDUNDANCY, 2);
		public static readonly TableRowHighlightType INFLUENCE_ON_POSITION = new TableRowHighlightType("INFLUENCE_ON_POSITION", InnerEnum.INFLUENCE_ON_POSITION, 3);
		public static readonly TableRowHighlightType P_PRIO_VALUE = new TableRowHighlightType("P_PRIO_VALUE", InnerEnum.P_PRIO_VALUE, 4);
		public static readonly TableRowHighlightType GROSS_ERROR = new TableRowHighlightType("GROSS_ERROR", InnerEnum.GROSS_ERROR, 5);

		private static readonly List<TableRowHighlightType> valueList = new List<TableRowHighlightType>();

		static TableRowHighlightType()
		{
			valueList.Add(NONE);
			valueList.Add(TEST_STATISTIC);
			valueList.Add(REDUNDANCY);
			valueList.Add(INFLUENCE_ON_POSITION);
			valueList.Add(P_PRIO_VALUE);
			valueList.Add(GROSS_ERROR);
		}

		public enum InnerEnum
		{
			NONE,
			TEST_STATISTIC,
			REDUNDANCY,
			INFLUENCE_ON_POSITION,
			P_PRIO_VALUE,
			GROSS_ERROR
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private TableRowHighlightType(string name, InnerEnum innerEnum, int id)
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

		public static TableRowHighlightType getEnumByValue(int value)
		{
			foreach (TableRowHighlightType element in TableRowHighlightType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return NONE;
		}

		public static TableRowHighlightType[] values()
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

		public static TableRowHighlightType valueOf(string name)
		{
			foreach (TableRowHighlightType enumInstance in TableRowHighlightType.valueList)
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