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
	public sealed class VerticalDeflectionType
	{
		public static readonly VerticalDeflectionType REFERENCE_VERTICAL_DEFLECTION = new VerticalDeflectionType("REFERENCE_VERTICAL_DEFLECTION", InnerEnum.REFERENCE_VERTICAL_DEFLECTION, 1);
		public static readonly VerticalDeflectionType STOCHASTIC_VERTICAL_DEFLECTION = new VerticalDeflectionType("STOCHASTIC_VERTICAL_DEFLECTION", InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION, 2);
		public static readonly VerticalDeflectionType UNKNOWN_VERTICAL_DEFLECTION = new VerticalDeflectionType("UNKNOWN_VERTICAL_DEFLECTION", InnerEnum.UNKNOWN_VERTICAL_DEFLECTION, 3);

		private static readonly List<VerticalDeflectionType> valueList = new List<VerticalDeflectionType>();

		static VerticalDeflectionType()
		{
			valueList.Add(REFERENCE_VERTICAL_DEFLECTION);
			valueList.Add(STOCHASTIC_VERTICAL_DEFLECTION);
			valueList.Add(UNKNOWN_VERTICAL_DEFLECTION);
		}

		public enum InnerEnum
		{
			REFERENCE_VERTICAL_DEFLECTION,
			STOCHASTIC_VERTICAL_DEFLECTION,
			UNKNOWN_VERTICAL_DEFLECTION
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private VerticalDeflectionType(string name, InnerEnum innerEnum, int id)
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

		public static VerticalDeflectionType getEnumByValue(int value)
		{
			foreach (VerticalDeflectionType element in VerticalDeflectionType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static VerticalDeflectionType[] values()
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

		public static VerticalDeflectionType valueOf(string name)
		{
			foreach (VerticalDeflectionType enumInstance in VerticalDeflectionType.valueList)
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