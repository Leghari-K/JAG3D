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

namespace org.applied_geodesy.adjustment.network.observation.reduction
{
	public sealed class ProjectionType
	{
		public static readonly ProjectionType LOCAL_CARTESIAN = new ProjectionType("LOCAL_CARTESIAN", InnerEnum.LOCAL_CARTESIAN, 0);
		public static readonly ProjectionType GAUSS_KRUEGER = new ProjectionType("GAUSS_KRUEGER", InnerEnum.GAUSS_KRUEGER, 1);
		public static readonly ProjectionType UTM = new ProjectionType("UTM", InnerEnum.UTM, 2);
		public static readonly ProjectionType LOCAL_ELLIPSOIDAL = new ProjectionType("LOCAL_ELLIPSOIDAL", InnerEnum.LOCAL_ELLIPSOIDAL, 3);

		private static readonly List<ProjectionType> valueList = new List<ProjectionType>();

		static ProjectionType()
		{
			valueList.Add(LOCAL_CARTESIAN);
			valueList.Add(GAUSS_KRUEGER);
			valueList.Add(UTM);
			valueList.Add(LOCAL_ELLIPSOIDAL);
		}

		public enum InnerEnum
		{
			LOCAL_CARTESIAN,
			GAUSS_KRUEGER,
			UTM,
			LOCAL_ELLIPSOIDAL
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private ProjectionType(string name, InnerEnum innerEnum, int id)
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

		public static ProjectionType getEnumByValue(int value)
		{
			foreach (ProjectionType element in ProjectionType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return LOCAL_CARTESIAN;
		}

		public static ProjectionType[] values()
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

		public static ProjectionType valueOf(string name)
		{
			foreach (ProjectionType enumInstance in ProjectionType.valueList)
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