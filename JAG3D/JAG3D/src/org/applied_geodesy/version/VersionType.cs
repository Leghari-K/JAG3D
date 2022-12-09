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

namespace org.applied_geodesy.version
{
	public sealed class VersionType
	{
		public static readonly VersionType ADJUSTMENT_CORE = new VersionType("ADJUSTMENT_CORE", InnerEnum.ADJUSTMENT_CORE, 1);
		public static readonly VersionType DATABASE = new VersionType("DATABASE", InnerEnum.DATABASE, 2);
		public static readonly VersionType USER_INTERFACE = new VersionType("USER_INTERFACE", InnerEnum.USER_INTERFACE, 3);

		private static readonly List<VersionType> valueList = new List<VersionType>();

		static VersionType()
		{
			valueList.Add(ADJUSTMENT_CORE);
			valueList.Add(DATABASE);
			valueList.Add(USER_INTERFACE);
		}

		public enum InnerEnum
		{
			ADJUSTMENT_CORE,
			DATABASE,
			USER_INTERFACE
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private VersionType(string name, InnerEnum innerEnum, int id)
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

		public static VersionType getEnumByValue(int value)
		{
			foreach (VersionType element in VersionType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static VersionType[] values()
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

		public static VersionType valueOf(string name)
		{
			foreach (VersionType enumInstance in VersionType.valueList)
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