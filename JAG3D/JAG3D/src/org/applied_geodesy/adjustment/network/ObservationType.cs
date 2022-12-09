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
	public sealed class ObservationType
	{
		public static readonly ObservationType LEVELING = new ObservationType("LEVELING", InnerEnum.LEVELING, 1);
		public static readonly ObservationType DIRECTION = new ObservationType("DIRECTION", InnerEnum.DIRECTION, 2);
		public static readonly ObservationType HORIZONTAL_DISTANCE = new ObservationType("HORIZONTAL_DISTANCE", InnerEnum.HORIZONTAL_DISTANCE, 3);
		public static readonly ObservationType SLOPE_DISTANCE = new ObservationType("SLOPE_DISTANCE", InnerEnum.SLOPE_DISTANCE, 4);
		public static readonly ObservationType ZENITH_ANGLE = new ObservationType("ZENITH_ANGLE", InnerEnum.ZENITH_ANGLE, 5);
		public static readonly ObservationType GNSS1D = new ObservationType("GNSS1D", InnerEnum.GNSS1D, 6); // 50
		public static readonly ObservationType GNSS2D = new ObservationType("GNSS2D", InnerEnum.GNSS2D, 7); // 60
		public static readonly ObservationType GNSS3D = new ObservationType("GNSS3D", InnerEnum.GNSS3D, 8);

		private static readonly List<ObservationType> valueList = new List<ObservationType>();

		static ObservationType()
		{
			valueList.Add(LEVELING);
			valueList.Add(DIRECTION);
			valueList.Add(HORIZONTAL_DISTANCE);
			valueList.Add(SLOPE_DISTANCE);
			valueList.Add(ZENITH_ANGLE);
			valueList.Add(GNSS1D);
			valueList.Add(GNSS2D);
			valueList.Add(GNSS3D);
		}

		public enum InnerEnum
		{
			LEVELING,
			DIRECTION,
			HORIZONTAL_DISTANCE,
			SLOPE_DISTANCE,
			ZENITH_ANGLE,
			GNSS1D,
			GNSS2D,
			GNSS3D
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private ObservationType(string name, InnerEnum innerEnum, int id)
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

		public static ObservationType getEnumByValue(int value)
		{
			foreach (ObservationType element in ObservationType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static ObservationType[] values()
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

		public static ObservationType valueOf(string name)
		{
			foreach (ObservationType enumInstance in ObservationType.valueList)
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