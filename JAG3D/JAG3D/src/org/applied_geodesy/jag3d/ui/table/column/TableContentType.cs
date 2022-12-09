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

namespace org.applied_geodesy.jag3d.ui.table.column
{
	public sealed class TableContentType
	{
		public static readonly TableContentType UNSPECIFIC = new TableContentType("UNSPECIFIC", InnerEnum.UNSPECIFIC, 0);

		public static readonly TableContentType LEVELING = new TableContentType("LEVELING", InnerEnum.LEVELING, 1);
		public static readonly TableContentType DIRECTION = new TableContentType("DIRECTION", InnerEnum.DIRECTION, 2);
		public static readonly TableContentType HORIZONTAL_DISTANCE = new TableContentType("HORIZONTAL_DISTANCE", InnerEnum.HORIZONTAL_DISTANCE, 3);
		public static readonly TableContentType SLOPE_DISTANCE = new TableContentType("SLOPE_DISTANCE", InnerEnum.SLOPE_DISTANCE, 4);
		public static readonly TableContentType ZENITH_ANGLE = new TableContentType("ZENITH_ANGLE", InnerEnum.ZENITH_ANGLE, 5);

		public static readonly TableContentType GNSS_1D = new TableContentType("GNSS_1D", InnerEnum.GNSS_1D, 6);
		public static readonly TableContentType GNSS_2D = new TableContentType("GNSS_2D", InnerEnum.GNSS_2D, 7);
		public static readonly TableContentType GNSS_3D = new TableContentType("GNSS_3D", InnerEnum.GNSS_3D, 8);

		public static readonly TableContentType REFERENCE_POINT_1D = new TableContentType("REFERENCE_POINT_1D", InnerEnum.REFERENCE_POINT_1D, 10);
		public static readonly TableContentType REFERENCE_POINT_2D = new TableContentType("REFERENCE_POINT_2D", InnerEnum.REFERENCE_POINT_2D, 11);
		public static readonly TableContentType REFERENCE_POINT_3D = new TableContentType("REFERENCE_POINT_3D", InnerEnum.REFERENCE_POINT_3D, 12);

		public static readonly TableContentType STOCHASTIC_POINT_1D = new TableContentType("STOCHASTIC_POINT_1D", InnerEnum.STOCHASTIC_POINT_1D, 20);
		public static readonly TableContentType STOCHASTIC_POINT_2D = new TableContentType("STOCHASTIC_POINT_2D", InnerEnum.STOCHASTIC_POINT_2D, 21);
		public static readonly TableContentType STOCHASTIC_POINT_3D = new TableContentType("STOCHASTIC_POINT_3D", InnerEnum.STOCHASTIC_POINT_3D, 22);

		public static readonly TableContentType DATUM_POINT_1D = new TableContentType("DATUM_POINT_1D", InnerEnum.DATUM_POINT_1D, 30);
		public static readonly TableContentType DATUM_POINT_2D = new TableContentType("DATUM_POINT_2D", InnerEnum.DATUM_POINT_2D, 31);
		public static readonly TableContentType DATUM_POINT_3D = new TableContentType("DATUM_POINT_3D", InnerEnum.DATUM_POINT_3D, 32);

		public static readonly TableContentType NEW_POINT_1D = new TableContentType("NEW_POINT_1D", InnerEnum.NEW_POINT_1D, 40);
		public static readonly TableContentType NEW_POINT_2D = new TableContentType("NEW_POINT_2D", InnerEnum.NEW_POINT_2D, 41);
		public static readonly TableContentType NEW_POINT_3D = new TableContentType("NEW_POINT_3D", InnerEnum.NEW_POINT_3D, 42);

		public static readonly TableContentType CONGRUENCE_ANALYSIS_1D = new TableContentType("CONGRUENCE_ANALYSIS_1D", InnerEnum.CONGRUENCE_ANALYSIS_1D, 50);
		public static readonly TableContentType CONGRUENCE_ANALYSIS_2D = new TableContentType("CONGRUENCE_ANALYSIS_2D", InnerEnum.CONGRUENCE_ANALYSIS_2D, 51);
		public static readonly TableContentType CONGRUENCE_ANALYSIS_3D = new TableContentType("CONGRUENCE_ANALYSIS_3D", InnerEnum.CONGRUENCE_ANALYSIS_3D, 52);

		public static readonly TableContentType REFERENCE_DEFLECTION = new TableContentType("REFERENCE_DEFLECTION", InnerEnum.REFERENCE_DEFLECTION, 60);
		public static readonly TableContentType STOCHASTIC_DEFLECTION = new TableContentType("STOCHASTIC_DEFLECTION", InnerEnum.STOCHASTIC_DEFLECTION, 61);
		public static readonly TableContentType UNKNOWN_DEFLECTION = new TableContentType("UNKNOWN_DEFLECTION", InnerEnum.UNKNOWN_DEFLECTION, 62);

		public static readonly TableContentType ADDITIONAL_PARAMETER = new TableContentType("ADDITIONAL_PARAMETER", InnerEnum.ADDITIONAL_PARAMETER, 70);

		public static readonly TableContentType SELECTED_GROUP_VARIANCE_COMPONENTS = new TableContentType("SELECTED_GROUP_VARIANCE_COMPONENTS", InnerEnum.SELECTED_GROUP_VARIANCE_COMPONENTS, 80);

		private static readonly List<TableContentType> valueList = new List<TableContentType>();

		static TableContentType()
		{
			valueList.Add(UNSPECIFIC);
			valueList.Add(LEVELING);
			valueList.Add(DIRECTION);
			valueList.Add(HORIZONTAL_DISTANCE);
			valueList.Add(SLOPE_DISTANCE);
			valueList.Add(ZENITH_ANGLE);
			valueList.Add(GNSS_1D);
			valueList.Add(GNSS_2D);
			valueList.Add(GNSS_3D);
			valueList.Add(REFERENCE_POINT_1D);
			valueList.Add(REFERENCE_POINT_2D);
			valueList.Add(REFERENCE_POINT_3D);
			valueList.Add(STOCHASTIC_POINT_1D);
			valueList.Add(STOCHASTIC_POINT_2D);
			valueList.Add(STOCHASTIC_POINT_3D);
			valueList.Add(DATUM_POINT_1D);
			valueList.Add(DATUM_POINT_2D);
			valueList.Add(DATUM_POINT_3D);
			valueList.Add(NEW_POINT_1D);
			valueList.Add(NEW_POINT_2D);
			valueList.Add(NEW_POINT_3D);
			valueList.Add(CONGRUENCE_ANALYSIS_1D);
			valueList.Add(CONGRUENCE_ANALYSIS_2D);
			valueList.Add(CONGRUENCE_ANALYSIS_3D);
			valueList.Add(REFERENCE_DEFLECTION);
			valueList.Add(STOCHASTIC_DEFLECTION);
			valueList.Add(UNKNOWN_DEFLECTION);
			valueList.Add(ADDITIONAL_PARAMETER);
			valueList.Add(SELECTED_GROUP_VARIANCE_COMPONENTS);
		}

		public enum InnerEnum
		{
			UNSPECIFIC,
			LEVELING,
			DIRECTION,
			HORIZONTAL_DISTANCE,
			SLOPE_DISTANCE,
			ZENITH_ANGLE,
			GNSS_1D,
			GNSS_2D,
			GNSS_3D,
			REFERENCE_POINT_1D,
			REFERENCE_POINT_2D,
			REFERENCE_POINT_3D,
			STOCHASTIC_POINT_1D,
			STOCHASTIC_POINT_2D,
			STOCHASTIC_POINT_3D,
			DATUM_POINT_1D,
			DATUM_POINT_2D,
			DATUM_POINT_3D,
			NEW_POINT_1D,
			NEW_POINT_2D,
			NEW_POINT_3D,
			CONGRUENCE_ANALYSIS_1D,
			CONGRUENCE_ANALYSIS_2D,
			CONGRUENCE_ANALYSIS_3D,
			REFERENCE_DEFLECTION,
			STOCHASTIC_DEFLECTION,
			UNKNOWN_DEFLECTION,
			ADDITIONAL_PARAMETER,
			SELECTED_GROUP_VARIANCE_COMPONENTS
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private TableContentType(string name, InnerEnum innerEnum, int id)
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

		public static TableContentType getEnumByValue(int value)
		{
			foreach (TableContentType element in TableContentType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return UNSPECIFIC;
		}

		public static TableContentType[] values()
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

		public static TableContentType valueOf(string name)
		{
			foreach (TableContentType enumInstance in TableContentType.valueList)
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