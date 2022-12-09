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
	public sealed class VarianceComponentType
	{
		public static readonly VarianceComponentType GLOBAL = new VarianceComponentType("GLOBAL", InnerEnum.GLOBAL, 0);

		public static readonly VarianceComponentType STOCHASTIC_POINT_1D_COMPONENT = new VarianceComponentType("STOCHASTIC_POINT_1D_COMPONENT", InnerEnum.STOCHASTIC_POINT_1D_COMPONENT, 101);
		public static readonly VarianceComponentType STOCHASTIC_POINT_2D_COMPONENT = new VarianceComponentType("STOCHASTIC_POINT_2D_COMPONENT", InnerEnum.STOCHASTIC_POINT_2D_COMPONENT, 102);
		public static readonly VarianceComponentType STOCHASTIC_POINT_3D_COMPONENT = new VarianceComponentType("STOCHASTIC_POINT_3D_COMPONENT", InnerEnum.STOCHASTIC_POINT_3D_COMPONENT, 103);

		public static readonly VarianceComponentType STOCHASTIC_DEFLECTION_COMPONENT = new VarianceComponentType("STOCHASTIC_DEFLECTION_COMPONENT", InnerEnum.STOCHASTIC_DEFLECTION_COMPONENT, 201);

		public static readonly VarianceComponentType LEVELING_COMPONENT = new VarianceComponentType("LEVELING_COMPONENT", InnerEnum.LEVELING_COMPONENT, 10);
		public static readonly VarianceComponentType LEVELING_ZERO_POINT_OFFSET_COMPONENT = new VarianceComponentType("LEVELING_ZERO_POINT_OFFSET_COMPONENT", InnerEnum.LEVELING_ZERO_POINT_OFFSET_COMPONENT, 11);
		public static readonly VarianceComponentType LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT, 12);
		public static readonly VarianceComponentType LEVELING_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("LEVELING_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.LEVELING_DISTANCE_DEPENDENT_COMPONENT, 13);

		public static readonly VarianceComponentType DIRECTION_COMPONENT = new VarianceComponentType("DIRECTION_COMPONENT", InnerEnum.DIRECTION_COMPONENT, 20);
		public static readonly VarianceComponentType DIRECTION_ZERO_POINT_OFFSET_COMPONENT = new VarianceComponentType("DIRECTION_ZERO_POINT_OFFSET_COMPONENT", InnerEnum.DIRECTION_ZERO_POINT_OFFSET_COMPONENT, 21);
		public static readonly VarianceComponentType DIRECTION_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("DIRECTION_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.DIRECTION_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT, 22);
		public static readonly VarianceComponentType DIRECTION_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("DIRECTION_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.DIRECTION_DISTANCE_DEPENDENT_COMPONENT, 23);

		public static readonly VarianceComponentType HORIZONTAL_DISTANCE_COMPONENT = new VarianceComponentType("HORIZONTAL_DISTANCE_COMPONENT", InnerEnum.HORIZONTAL_DISTANCE_COMPONENT, 30);
		public static readonly VarianceComponentType HORIZONTAL_DISTANCE_ZERO_POINT_OFFSET_COMPONENT = new VarianceComponentType("HORIZONTAL_DISTANCE_ZERO_POINT_OFFSET_COMPONENT", InnerEnum.HORIZONTAL_DISTANCE_ZERO_POINT_OFFSET_COMPONENT, 31);
		public static readonly VarianceComponentType HORIZONTAL_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("HORIZONTAL_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.HORIZONTAL_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT, 32);
		public static readonly VarianceComponentType HORIZONTAL_DISTANCE_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("HORIZONTAL_DISTANCE_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.HORIZONTAL_DISTANCE_DISTANCE_DEPENDENT_COMPONENT, 33);

		public static readonly VarianceComponentType SLOPE_DISTANCE_COMPONENT = new VarianceComponentType("SLOPE_DISTANCE_COMPONENT", InnerEnum.SLOPE_DISTANCE_COMPONENT, 40);
		public static readonly VarianceComponentType SLOPE_DISTANCE_ZERO_POINT_OFFSET_COMPONENT = new VarianceComponentType("SLOPE_DISTANCE_ZERO_POINT_OFFSET_COMPONENT", InnerEnum.SLOPE_DISTANCE_ZERO_POINT_OFFSET_COMPONENT, 41);
		public static readonly VarianceComponentType SLOPE_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("SLOPE_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.SLOPE_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT, 42);
		public static readonly VarianceComponentType SLOPE_DISTANCE_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("SLOPE_DISTANCE_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.SLOPE_DISTANCE_DISTANCE_DEPENDENT_COMPONENT, 43);

		public static readonly VarianceComponentType ZENITH_ANGLE_COMPONENT = new VarianceComponentType("ZENITH_ANGLE_COMPONENT", InnerEnum.ZENITH_ANGLE_COMPONENT, 50);
		public static readonly VarianceComponentType ZENITH_ANGLE_ZERO_POINT_OFFSET_COMPONENT = new VarianceComponentType("ZENITH_ANGLE_ZERO_POINT_OFFSET_COMPONENT", InnerEnum.ZENITH_ANGLE_ZERO_POINT_OFFSET_COMPONENT, 51);
		public static readonly VarianceComponentType ZENITH_ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("ZENITH_ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.ZENITH_ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT, 52);
		public static readonly VarianceComponentType ZENITH_ANGLE_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("ZENITH_ANGLE_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.ZENITH_ANGLE_DISTANCE_DEPENDENT_COMPONENT, 53);

		public static readonly VarianceComponentType GNSS1D_COMPONENT = new VarianceComponentType("GNSS1D_COMPONENT", InnerEnum.GNSS1D_COMPONENT, 60);
		public static readonly VarianceComponentType GNSS1D_ZERO_POINT_OFFSET_COMPONENT = new VarianceComponentType("GNSS1D_ZERO_POINT_OFFSET_COMPONENT", InnerEnum.GNSS1D_ZERO_POINT_OFFSET_COMPONENT, 61);
		public static readonly VarianceComponentType GNSS1D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("GNSS1D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.GNSS1D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT, 62);
		public static readonly VarianceComponentType GNSS1D_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("GNSS1D_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.GNSS1D_DISTANCE_DEPENDENT_COMPONENT, 63);

		public static readonly VarianceComponentType GNSS2D_COMPONENT = new VarianceComponentType("GNSS2D_COMPONENT", InnerEnum.GNSS2D_COMPONENT, 70);
		public static readonly VarianceComponentType GNSS2D_ZERO_POINT_OFFSET_COMPONENT = new VarianceComponentType("GNSS2D_ZERO_POINT_OFFSET_COMPONENT", InnerEnum.GNSS2D_ZERO_POINT_OFFSET_COMPONENT, 71);
		public static readonly VarianceComponentType GNSS2D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("GNSS2D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.GNSS2D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT, 72);
		public static readonly VarianceComponentType GNSS2D_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("GNSS2D_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.GNSS2D_DISTANCE_DEPENDENT_COMPONENT, 73);

		public static readonly VarianceComponentType GNSS3D_COMPONENT = new VarianceComponentType("GNSS3D_COMPONENT", InnerEnum.GNSS3D_COMPONENT, 80);
		public static readonly VarianceComponentType GNSS3D_ZERO_POINT_OFFSET_COMPONENT = new VarianceComponentType("GNSS3D_ZERO_POINT_OFFSET_COMPONENT", InnerEnum.GNSS3D_ZERO_POINT_OFFSET_COMPONENT, 81);
		public static readonly VarianceComponentType GNSS3D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("GNSS3D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.GNSS3D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT, 82);
		public static readonly VarianceComponentType GNSS3D_DISTANCE_DEPENDENT_COMPONENT = new VarianceComponentType("GNSS3D_DISTANCE_DEPENDENT_COMPONENT", InnerEnum.GNSS3D_DISTANCE_DEPENDENT_COMPONENT, 83);

		private static readonly List<VarianceComponentType> valueList = new List<VarianceComponentType>();

		static VarianceComponentType()
		{
			valueList.Add(GLOBAL);
			valueList.Add(STOCHASTIC_POINT_1D_COMPONENT);
			valueList.Add(STOCHASTIC_POINT_2D_COMPONENT);
			valueList.Add(STOCHASTIC_POINT_3D_COMPONENT);
			valueList.Add(STOCHASTIC_DEFLECTION_COMPONENT);
			valueList.Add(LEVELING_COMPONENT);
			valueList.Add(LEVELING_ZERO_POINT_OFFSET_COMPONENT);
			valueList.Add(LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(LEVELING_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(DIRECTION_COMPONENT);
			valueList.Add(DIRECTION_ZERO_POINT_OFFSET_COMPONENT);
			valueList.Add(DIRECTION_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(DIRECTION_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(HORIZONTAL_DISTANCE_COMPONENT);
			valueList.Add(HORIZONTAL_DISTANCE_ZERO_POINT_OFFSET_COMPONENT);
			valueList.Add(HORIZONTAL_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(HORIZONTAL_DISTANCE_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(SLOPE_DISTANCE_COMPONENT);
			valueList.Add(SLOPE_DISTANCE_ZERO_POINT_OFFSET_COMPONENT);
			valueList.Add(SLOPE_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(SLOPE_DISTANCE_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(ZENITH_ANGLE_COMPONENT);
			valueList.Add(ZENITH_ANGLE_ZERO_POINT_OFFSET_COMPONENT);
			valueList.Add(ZENITH_ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(ZENITH_ANGLE_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(GNSS1D_COMPONENT);
			valueList.Add(GNSS1D_ZERO_POINT_OFFSET_COMPONENT);
			valueList.Add(GNSS1D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(GNSS1D_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(GNSS2D_COMPONENT);
			valueList.Add(GNSS2D_ZERO_POINT_OFFSET_COMPONENT);
			valueList.Add(GNSS2D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(GNSS2D_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(GNSS3D_COMPONENT);
			valueList.Add(GNSS3D_ZERO_POINT_OFFSET_COMPONENT);
			valueList.Add(GNSS3D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT);
			valueList.Add(GNSS3D_DISTANCE_DEPENDENT_COMPONENT);
		}

		public enum InnerEnum
		{
			GLOBAL,
			STOCHASTIC_POINT_1D_COMPONENT,
			STOCHASTIC_POINT_2D_COMPONENT,
			STOCHASTIC_POINT_3D_COMPONENT,
			STOCHASTIC_DEFLECTION_COMPONENT,
			LEVELING_COMPONENT,
			LEVELING_ZERO_POINT_OFFSET_COMPONENT,
			LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT,
			LEVELING_DISTANCE_DEPENDENT_COMPONENT,
			DIRECTION_COMPONENT,
			DIRECTION_ZERO_POINT_OFFSET_COMPONENT,
			DIRECTION_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT,
			DIRECTION_DISTANCE_DEPENDENT_COMPONENT,
			HORIZONTAL_DISTANCE_COMPONENT,
			HORIZONTAL_DISTANCE_ZERO_POINT_OFFSET_COMPONENT,
			HORIZONTAL_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT,
			HORIZONTAL_DISTANCE_DISTANCE_DEPENDENT_COMPONENT,
			SLOPE_DISTANCE_COMPONENT,
			SLOPE_DISTANCE_ZERO_POINT_OFFSET_COMPONENT,
			SLOPE_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT,
			SLOPE_DISTANCE_DISTANCE_DEPENDENT_COMPONENT,
			ZENITH_ANGLE_COMPONENT,
			ZENITH_ANGLE_ZERO_POINT_OFFSET_COMPONENT,
			ZENITH_ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT,
			ZENITH_ANGLE_DISTANCE_DEPENDENT_COMPONENT,
			GNSS1D_COMPONENT,
			GNSS1D_ZERO_POINT_OFFSET_COMPONENT,
			GNSS1D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT,
			GNSS1D_DISTANCE_DEPENDENT_COMPONENT,
			GNSS2D_COMPONENT,
			GNSS2D_ZERO_POINT_OFFSET_COMPONENT,
			GNSS2D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT,
			GNSS2D_DISTANCE_DEPENDENT_COMPONENT,
			GNSS3D_COMPONENT,
			GNSS3D_ZERO_POINT_OFFSET_COMPONENT,
			GNSS3D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT,
			GNSS3D_DISTANCE_DEPENDENT_COMPONENT
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private VarianceComponentType(string name, InnerEnum innerEnum, int id)
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

		public static VarianceComponentType getEnumByValue(int value)
		{
			foreach (VarianceComponentType element in VarianceComponentType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static VarianceComponentType getVarianceComponentTypeByObservationType(ObservationType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.LEVELING:
				return LEVELING_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.DIRECTION:
				return DIRECTION_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				return HORIZONTAL_DISTANCE_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.SLOPE_DISTANCE:
				return SLOPE_DISTANCE_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.ZENITH_ANGLE:
				return ZENITH_ANGLE_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS1D:
				return GNSS1D_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS2D:
				return GNSS2D_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS3D:
				return GNSS3D_COMPONENT;
			}
			return null;
		}

		public static VarianceComponentType getZeroPointOffsetVarianceComponentTypeByObservationType(ObservationType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.LEVELING:
				return LEVELING_ZERO_POINT_OFFSET_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.DIRECTION:
				return DIRECTION_ZERO_POINT_OFFSET_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				return HORIZONTAL_DISTANCE_ZERO_POINT_OFFSET_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.SLOPE_DISTANCE:
				return SLOPE_DISTANCE_ZERO_POINT_OFFSET_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.ZENITH_ANGLE:
				return ZENITH_ANGLE_ZERO_POINT_OFFSET_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS1D:
				return GNSS1D_ZERO_POINT_OFFSET_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS2D:
				return GNSS2D_ZERO_POINT_OFFSET_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS3D:
				return GNSS3D_ZERO_POINT_OFFSET_COMPONENT;
			}
			return null;
		}

		public static VarianceComponentType getSquareRootDistanceDependentVarianceComponentTypeByObservationType(ObservationType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.LEVELING:
				return LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.DIRECTION:
				return DIRECTION_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				return HORIZONTAL_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.SLOPE_DISTANCE:
				return SLOPE_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.ZENITH_ANGLE:
				return ZENITH_ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS1D:
				return GNSS1D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS2D:
				return GNSS2D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS3D:
				return GNSS3D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT;
			}
			return null;
		}

		public static VarianceComponentType getDistanceDependentVarianceComponentTypeByObservationType(ObservationType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.LEVELING:
				return LEVELING_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.DIRECTION:
				return DIRECTION_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				return HORIZONTAL_DISTANCE_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.SLOPE_DISTANCE:
				return SLOPE_DISTANCE_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.ZENITH_ANGLE:
				return ZENITH_ANGLE_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS1D:
				return GNSS1D_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS2D:
				return GNSS2D_DISTANCE_DEPENDENT_COMPONENT;
			case org.applied_geodesy.adjustment.network.ObservationType.InnerEnum.GNSS3D:
				return GNSS3D_DISTANCE_DEPENDENT_COMPONENT;
			}
			return null;
		}

		public static VarianceComponentType getComponentTypeByPointDimension(int dimension)
		{
			switch (dimension)
			{
			case 1:
				return STOCHASTIC_POINT_1D_COMPONENT;
			case 2:
				return STOCHASTIC_POINT_2D_COMPONENT;
			case 3:
				return STOCHASTIC_POINT_3D_COMPONENT;
			}
			return null;
		}

		public static VarianceComponentType[] values()
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

		public static VarianceComponentType valueOf(string name)
		{
			foreach (VarianceComponentType enumInstance in VarianceComponentType.valueList)
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