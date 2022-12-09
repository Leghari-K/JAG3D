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

namespace org.applied_geodesy.adjustment
{
	public sealed class EstimationType
	{
		public static readonly EstimationType L1NORM = new EstimationType("L1NORM", InnerEnum.L1NORM, 1);
		public static readonly EstimationType L2NORM = new EstimationType("L2NORM", InnerEnum.L2NORM, 2);
		public static readonly EstimationType SIMULATION = new EstimationType("SIMULATION", InnerEnum.SIMULATION, 3);
		public static readonly EstimationType MODIFIED_UNSCENTED_TRANSFORMATION = new EstimationType("MODIFIED_UNSCENTED_TRANSFORMATION", InnerEnum.MODIFIED_UNSCENTED_TRANSFORMATION, 4);
		public static readonly EstimationType SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION = new EstimationType("SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION", InnerEnum.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION, 5);

		private static readonly List<EstimationType> valueList = new List<EstimationType>();

		static EstimationType()
		{
			valueList.Add(L1NORM);
			valueList.Add(L2NORM);
			valueList.Add(SIMULATION);
			valueList.Add(MODIFIED_UNSCENTED_TRANSFORMATION);
			valueList.Add(SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION);
		}

		public enum InnerEnum
		{
			L1NORM,
			L2NORM,
			SIMULATION,
			MODIFIED_UNSCENTED_TRANSFORMATION,
			SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private EstimationType(string name, InnerEnum innerEnum, int id)
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

		public static EstimationType getEnumByValue(int value)
		{
			foreach (EstimationType element in EstimationType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static EstimationType[] values()
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

		public static EstimationType valueOf(string name)
		{
			foreach (EstimationType enumInstance in EstimationType.valueList)
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