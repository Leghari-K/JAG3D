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

namespace org.applied_geodesy.jag3d.ui.graphic.layer.symbol
{
	public sealed class PointSymbolType
	{
		public static readonly PointSymbolType DOT = new PointSymbolType("DOT", InnerEnum.DOT, 1);
		public static readonly PointSymbolType X_CROSS = new PointSymbolType("X_CROSS", InnerEnum.X_CROSS, 2);
		public static readonly PointSymbolType PLUS_CROSS = new PointSymbolType("PLUS_CROSS", InnerEnum.PLUS_CROSS, 3);

		public static readonly PointSymbolType STROKED_CIRCLE = new PointSymbolType("STROKED_CIRCLE", InnerEnum.STROKED_CIRCLE, 101);
		public static readonly PointSymbolType STROKED_UPRIGHT_TRIANGLE = new PointSymbolType("STROKED_UPRIGHT_TRIANGLE", InnerEnum.STROKED_UPRIGHT_TRIANGLE, 102);
		public static readonly PointSymbolType STROKED_DOWNRIGHT_TRIANGLE = new PointSymbolType("STROKED_DOWNRIGHT_TRIANGLE", InnerEnum.STROKED_DOWNRIGHT_TRIANGLE, 103);
		public static readonly PointSymbolType STROKED_SQUARE = new PointSymbolType("STROKED_SQUARE", InnerEnum.STROKED_SQUARE, 104);
		public static readonly PointSymbolType STROKED_STAR = new PointSymbolType("STROKED_STAR", InnerEnum.STROKED_STAR, 105);
		public static readonly PointSymbolType STROKED_PENTAGON = new PointSymbolType("STROKED_PENTAGON", InnerEnum.STROKED_PENTAGON, 106);
		public static readonly PointSymbolType STROKED_HEXAGON = new PointSymbolType("STROKED_HEXAGON", InnerEnum.STROKED_HEXAGON, 107);
		public static readonly PointSymbolType STROKED_HEPTAGON = new PointSymbolType("STROKED_HEPTAGON", InnerEnum.STROKED_HEPTAGON, 108);
		public static readonly PointSymbolType STROKED_OCTAGON = new PointSymbolType("STROKED_OCTAGON", InnerEnum.STROKED_OCTAGON, 109);
		public static readonly PointSymbolType STROKED_DIAMAND = new PointSymbolType("STROKED_DIAMAND", InnerEnum.STROKED_DIAMAND, 110);

		public static readonly PointSymbolType CROSSED_CIRCLE = new PointSymbolType("CROSSED_CIRCLE", InnerEnum.CROSSED_CIRCLE, 4);
		public static readonly PointSymbolType CROSSED_SQUARE = new PointSymbolType("CROSSED_SQUARE", InnerEnum.CROSSED_SQUARE, 5);

		public static readonly PointSymbolType FILLED_CIRCLE = new PointSymbolType("FILLED_CIRCLE", InnerEnum.FILLED_CIRCLE, 201);
		public static readonly PointSymbolType FILLED_UPRIGHT_TRIANGLE = new PointSymbolType("FILLED_UPRIGHT_TRIANGLE", InnerEnum.FILLED_UPRIGHT_TRIANGLE, 202);
		public static readonly PointSymbolType FILLED_DOWNRIGHT_TRIANGLE = new PointSymbolType("FILLED_DOWNRIGHT_TRIANGLE", InnerEnum.FILLED_DOWNRIGHT_TRIANGLE, 203);
		public static readonly PointSymbolType FILLED_SQUARE = new PointSymbolType("FILLED_SQUARE", InnerEnum.FILLED_SQUARE, 204);
		public static readonly PointSymbolType FILLED_STAR = new PointSymbolType("FILLED_STAR", InnerEnum.FILLED_STAR, 205);
		public static readonly PointSymbolType FILLED_PENTAGON = new PointSymbolType("FILLED_PENTAGON", InnerEnum.FILLED_PENTAGON, 206);
		public static readonly PointSymbolType FILLED_HEXAGON = new PointSymbolType("FILLED_HEXAGON", InnerEnum.FILLED_HEXAGON, 207);
		public static readonly PointSymbolType FILLED_HEPTAGON = new PointSymbolType("FILLED_HEPTAGON", InnerEnum.FILLED_HEPTAGON, 208);
		public static readonly PointSymbolType FILLED_OCTAGON = new PointSymbolType("FILLED_OCTAGON", InnerEnum.FILLED_OCTAGON, 209);
		public static readonly PointSymbolType FILLED_DIAMAND = new PointSymbolType("FILLED_DIAMAND", InnerEnum.FILLED_DIAMAND, 210);

		private static readonly List<PointSymbolType> valueList = new List<PointSymbolType>();

		static PointSymbolType()
		{
			valueList.Add(DOT);
			valueList.Add(X_CROSS);
			valueList.Add(PLUS_CROSS);
			valueList.Add(STROKED_CIRCLE);
			valueList.Add(STROKED_UPRIGHT_TRIANGLE);
			valueList.Add(STROKED_DOWNRIGHT_TRIANGLE);
			valueList.Add(STROKED_SQUARE);
			valueList.Add(STROKED_STAR);
			valueList.Add(STROKED_PENTAGON);
			valueList.Add(STROKED_HEXAGON);
			valueList.Add(STROKED_HEPTAGON);
			valueList.Add(STROKED_OCTAGON);
			valueList.Add(STROKED_DIAMAND);
			valueList.Add(CROSSED_CIRCLE);
			valueList.Add(CROSSED_SQUARE);
			valueList.Add(FILLED_CIRCLE);
			valueList.Add(FILLED_UPRIGHT_TRIANGLE);
			valueList.Add(FILLED_DOWNRIGHT_TRIANGLE);
			valueList.Add(FILLED_SQUARE);
			valueList.Add(FILLED_STAR);
			valueList.Add(FILLED_PENTAGON);
			valueList.Add(FILLED_HEXAGON);
			valueList.Add(FILLED_HEPTAGON);
			valueList.Add(FILLED_OCTAGON);
			valueList.Add(FILLED_DIAMAND);
		}

		public enum InnerEnum
		{
			DOT,
			X_CROSS,
			PLUS_CROSS,
			STROKED_CIRCLE,
			STROKED_UPRIGHT_TRIANGLE,
			STROKED_DOWNRIGHT_TRIANGLE,
			STROKED_SQUARE,
			STROKED_STAR,
			STROKED_PENTAGON,
			STROKED_HEXAGON,
			STROKED_HEPTAGON,
			STROKED_OCTAGON,
			STROKED_DIAMAND,
			CROSSED_CIRCLE,
			CROSSED_SQUARE,
			FILLED_CIRCLE,
			FILLED_UPRIGHT_TRIANGLE,
			FILLED_DOWNRIGHT_TRIANGLE,
			FILLED_SQUARE,
			FILLED_STAR,
			FILLED_PENTAGON,
			FILLED_HEXAGON,
			FILLED_HEPTAGON,
			FILLED_OCTAGON,
			FILLED_DIAMAND
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private PointSymbolType(string name, InnerEnum innerEnum, int id)
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

		public static PointSymbolType getEnumByValue(int value)
		{
			foreach (PointSymbolType element in PointSymbolType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static PointSymbolType[] values()
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

		public static PointSymbolType valueOf(string name)
		{
			foreach (PointSymbolType enumInstance in PointSymbolType.valueList)
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