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
	public sealed class Epoch
	{
		public static readonly Epoch REFERENCE = new Epoch("REFERENCE", InnerEnum.REFERENCE, true);
		public static readonly Epoch CONTROL = new Epoch("CONTROL", InnerEnum.CONTROL, false);

		private static readonly List<Epoch> valueList = new List<Epoch>();

		static Epoch()
		{
			valueList.Add(REFERENCE);
			valueList.Add(CONTROL);
		}

		public enum InnerEnum
		{
			REFERENCE,
			CONTROL
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private bool id;
		private Epoch(string name, InnerEnum innerEnum, bool id)
		{
			this.id = id;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public bool Id
		{
			get
			{
				return id;
			}
		}

		public static Epoch getEnumByValue(bool value)
		{
			foreach (Epoch element in Epoch.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static Epoch[] values()
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

		public static Epoch valueOf(string name)
		{
			foreach (Epoch enumInstance in Epoch.valueList)
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