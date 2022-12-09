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
	public sealed class PointGroupUncertaintyType
	{
		public static readonly PointGroupUncertaintyType COMPONENT_X = new PointGroupUncertaintyType("COMPONENT_X", InnerEnum.COMPONENT_X, 20);
		public static readonly PointGroupUncertaintyType COMPONENT_Y = new PointGroupUncertaintyType("COMPONENT_Y", InnerEnum.COMPONENT_Y, 21);
		public static readonly PointGroupUncertaintyType COMPONENT_Z = new PointGroupUncertaintyType("COMPONENT_Z", InnerEnum.COMPONENT_Z, 22);

		private static readonly List<PointGroupUncertaintyType> valueList = new List<PointGroupUncertaintyType>();

		static PointGroupUncertaintyType()
		{
			valueList.Add(COMPONENT_X);
			valueList.Add(COMPONENT_Y);
			valueList.Add(COMPONENT_Z);
		}

		public enum InnerEnum
		{
			COMPONENT_X,
			COMPONENT_Y,
			COMPONENT_Z
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private PointGroupUncertaintyType(string name, InnerEnum innerEnum, int id)
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

		public static PointGroupUncertaintyType getEnumByValue(int value)
		{
			foreach (PointGroupUncertaintyType element in PointGroupUncertaintyType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static PointGroupUncertaintyType[] values()
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

		public static PointGroupUncertaintyType valueOf(string name)
		{
			foreach (PointGroupUncertaintyType enumInstance in PointGroupUncertaintyType.valueList)
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