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

namespace org.applied_geodesy.adjustment.statistic
{
	public sealed class TestStatisticType
	{

		public static readonly TestStatisticType NONE = new TestStatisticType("NONE", InnerEnum.NONE, 1);
		public static readonly TestStatisticType BAARDA_METHOD = new TestStatisticType("BAARDA_METHOD", InnerEnum.BAARDA_METHOD, 2);
		public static readonly TestStatisticType SIDAK = new TestStatisticType("SIDAK", InnerEnum.SIDAK, 3);

		private static readonly List<TestStatisticType> valueList = new List<TestStatisticType>();

		static TestStatisticType()
		{
			valueList.Add(NONE);
			valueList.Add(BAARDA_METHOD);
			valueList.Add(SIDAK);
		}

		public enum InnerEnum
		{
			NONE,
			BAARDA_METHOD,
			SIDAK
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private readonly int id;
		private TestStatisticType(string name, InnerEnum innerEnum, int id)
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

		public static TestStatisticType getEnumByValue(int id)
		{
			foreach (TestStatisticType type in TestStatisticType.values())
			{
				if (type.Id == id)
				{
					return type;
				}
			}
			return NONE;
		}

		public static TestStatisticType[] values()
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

		public static TestStatisticType valueOf(string name)
		{
			foreach (TestStatisticType enumInstance in TestStatisticType.valueList)
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