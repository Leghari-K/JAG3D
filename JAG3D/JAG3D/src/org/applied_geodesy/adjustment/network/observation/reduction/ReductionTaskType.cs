﻿using System.Collections.Generic;

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

namespace org.applied_geodesy.adjustment.network.observation.reduction
{
	public sealed class ReductionTaskType
	{
		public static readonly ReductionTaskType DISTANCE = new ReductionTaskType("DISTANCE", InnerEnum.DISTANCE, 1);
		public static readonly ReductionTaskType HEIGHT = new ReductionTaskType("HEIGHT", InnerEnum.HEIGHT, 2);
		public static readonly ReductionTaskType DIRECTION = new ReductionTaskType("DIRECTION", InnerEnum.DIRECTION, 3);
		public static readonly ReductionTaskType EARTH_CURVATURE = new ReductionTaskType("EARTH_CURVATURE", InnerEnum.EARTH_CURVATURE, 4);

		private static readonly List<ReductionTaskType> valueList = new List<ReductionTaskType>();

		static ReductionTaskType()
		{
			valueList.Add(DISTANCE);
			valueList.Add(HEIGHT);
			valueList.Add(DIRECTION);
			valueList.Add(EARTH_CURVATURE);
		}

		public enum InnerEnum
		{
			DISTANCE,
			HEIGHT,
			DIRECTION,
			EARTH_CURVATURE
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private ReductionTaskType(string name, InnerEnum innerEnum, int id)
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

		public static ReductionTaskType getEnumByValue(int value)
		{
			foreach (ReductionTaskType element in ReductionTaskType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static ReductionTaskType[] values()
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

		public static ReductionTaskType valueOf(string name)
		{
			foreach (ReductionTaskType enumInstance in ReductionTaskType.valueList)
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