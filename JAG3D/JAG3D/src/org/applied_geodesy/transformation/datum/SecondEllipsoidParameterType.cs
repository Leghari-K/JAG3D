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

namespace org.applied_geodesy.transformation.datum
{
	internal sealed class SecondEllipsoidParameterType
	{
		public static readonly SecondEllipsoidParameterType MINOR_AXIS = new SecondEllipsoidParameterType("MINOR_AXIS", InnerEnum.MINOR_AXIS, 1);
		public static readonly SecondEllipsoidParameterType INVERSE_FLATTENING = new SecondEllipsoidParameterType("INVERSE_FLATTENING", InnerEnum.INVERSE_FLATTENING, 2);
		public static readonly SecondEllipsoidParameterType SQUARED_ECCENTRICITY = new SecondEllipsoidParameterType("SQUARED_ECCENTRICITY", InnerEnum.SQUARED_ECCENTRICITY, 3);

		private static readonly List<SecondEllipsoidParameterType> valueList = new List<SecondEllipsoidParameterType>();

		static SecondEllipsoidParameterType()
		{
			valueList.Add(MINOR_AXIS);
			valueList.Add(INVERSE_FLATTENING);
			valueList.Add(SQUARED_ECCENTRICITY);
		}

		public enum InnerEnum
		{
			MINOR_AXIS,
			INVERSE_FLATTENING,
			SQUARED_ECCENTRICITY
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private readonly int id;
		private SecondEllipsoidParameterType(string name, InnerEnum innerEnum, int id)
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
				return this.id;
			}
		}

		public static SecondEllipsoidParameterType getParameterById(int id)
		{
			SecondEllipsoidParameterType[] @params = SecondEllipsoidParameterType.values();
			foreach (SecondEllipsoidParameterType param in @params)
			{
				if (param.Id == id)
				{
					return param;
				}
			}
			return null;
		}

		public static SecondEllipsoidParameterType[] values()
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

		public static SecondEllipsoidParameterType valueOf(string name)
		{
			foreach (SecondEllipsoidParameterType enumInstance in SecondEllipsoidParameterType.valueList)
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