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
	public sealed class VerticalDeflectionGroupUncertaintyType
	{

		public static readonly VerticalDeflectionGroupUncertaintyType DEFLECTION_X = new VerticalDeflectionGroupUncertaintyType("DEFLECTION_X", InnerEnum.DEFLECTION_X, 31);
		public static readonly VerticalDeflectionGroupUncertaintyType DEFLECTION_Y = new VerticalDeflectionGroupUncertaintyType("DEFLECTION_Y", InnerEnum.DEFLECTION_Y, 32);

		private static readonly List<VerticalDeflectionGroupUncertaintyType> valueList = new List<VerticalDeflectionGroupUncertaintyType>();

		static VerticalDeflectionGroupUncertaintyType()
		{
			valueList.Add(DEFLECTION_X);
			valueList.Add(DEFLECTION_Y);
		}

		public enum InnerEnum
		{
			DEFLECTION_X,
			DEFLECTION_Y
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private VerticalDeflectionGroupUncertaintyType(string name, InnerEnum innerEnum, int id)
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

		public static VerticalDeflectionGroupUncertaintyType getEnumByValue(int value)
		{
			foreach (VerticalDeflectionGroupUncertaintyType element in VerticalDeflectionGroupUncertaintyType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static VerticalDeflectionGroupUncertaintyType[] values()
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

		public static VerticalDeflectionGroupUncertaintyType valueOf(string name)
		{
			foreach (VerticalDeflectionGroupUncertaintyType enumInstance in VerticalDeflectionGroupUncertaintyType.valueList)
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