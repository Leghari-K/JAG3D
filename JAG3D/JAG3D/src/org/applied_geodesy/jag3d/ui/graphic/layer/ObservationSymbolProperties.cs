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

namespace org.applied_geodesy.jag3d.ui.graphic.layer
{
	public class ObservationSymbolProperties : GraphicComponentProperties
	{
		public sealed class ObservationType
		{
			public static readonly ObservationType LEVELING = new ObservationType("LEVELING", InnerEnum.LEVELING, 1);
			public static readonly ObservationType DIRECTION = new ObservationType("DIRECTION", InnerEnum.DIRECTION, 2);
			public static readonly ObservationType DISTANCE = new ObservationType("DISTANCE", InnerEnum.DISTANCE, 3);
			public static readonly ObservationType ZENITH_ANGLE = new ObservationType("ZENITH_ANGLE", InnerEnum.ZENITH_ANGLE, 4);
			public static readonly ObservationType GNSS = new ObservationType("GNSS", InnerEnum.GNSS, 5);

			private static readonly List<ObservationType> valueList = new List<ObservationType>();

			static ObservationType()
			{
				valueList.Add(LEVELING);
				valueList.Add(DIRECTION);
				valueList.Add(DISTANCE);
				valueList.Add(ZENITH_ANGLE);
				valueList.Add(GNSS);
			}

			public enum InnerEnum
			{
				LEVELING,
				DIRECTION,
				DISTANCE,
				ZENITH_ANGLE,
				GNSS
			}

			public readonly InnerEnum innerEnumValue;
			private readonly string nameValue;
			private readonly int ordinalValue;
			private static int nextOrdinal = 0;

			internal int id;
			internal ObservationType(string name, InnerEnum innerEnum, int id)
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

		private readonly ObservationType observationType;

		internal ObservationSymbolProperties(ObservationType observationType)
		{
			this.observationType = observationType;
		}

		public virtual ObservationType ObservationType
		{
			get
			{
				return this.observationType;
			}
		}
	}

}