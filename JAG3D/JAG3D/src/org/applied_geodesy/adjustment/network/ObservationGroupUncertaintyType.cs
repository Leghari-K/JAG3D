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
	public sealed class ObservationGroupUncertaintyType
	{
		public static readonly ObservationGroupUncertaintyType ZERO_POINT_OFFSET = new ObservationGroupUncertaintyType("ZERO_POINT_OFFSET", InnerEnum.ZERO_POINT_OFFSET, 10);
		public static readonly ObservationGroupUncertaintyType SQUARE_ROOT_DISTANCE_DEPENDENT = new ObservationGroupUncertaintyType("SQUARE_ROOT_DISTANCE_DEPENDENT", InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT, 11);
		public static readonly ObservationGroupUncertaintyType DISTANCE_DEPENDENT = new ObservationGroupUncertaintyType("DISTANCE_DEPENDENT", InnerEnum.DISTANCE_DEPENDENT, 12);

		private static readonly List<ObservationGroupUncertaintyType> valueList = new List<ObservationGroupUncertaintyType>();

		static ObservationGroupUncertaintyType()
		{
			valueList.Add(ZERO_POINT_OFFSET);
			valueList.Add(SQUARE_ROOT_DISTANCE_DEPENDENT);
			valueList.Add(DISTANCE_DEPENDENT);
		}

		public enum InnerEnum
		{
			ZERO_POINT_OFFSET,
			SQUARE_ROOT_DISTANCE_DEPENDENT,
			DISTANCE_DEPENDENT
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;


		private int id;
		private ObservationGroupUncertaintyType(string name, InnerEnum innerEnum, int id)
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

		public static ObservationGroupUncertaintyType getEnumByValue(int value)
		{
			foreach (ObservationGroupUncertaintyType element in ObservationGroupUncertaintyType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static ObservationGroupUncertaintyType[] values()
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

		public static ObservationGroupUncertaintyType valueOf(string name)
		{
			foreach (ObservationGroupUncertaintyType enumInstance in ObservationGroupUncertaintyType.valueList)
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