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
	public sealed class LayerType
	{
		public static readonly LayerType ABSOLUTE_CONFIDENCE = new LayerType("ABSOLUTE_CONFIDENCE", InnerEnum.ABSOLUTE_CONFIDENCE, 41);
		public static readonly LayerType RELATIVE_CONFIDENCE = new LayerType("RELATIVE_CONFIDENCE", InnerEnum.RELATIVE_CONFIDENCE, 42);

		public static readonly LayerType OBSERVATION_APRIORI = new LayerType("OBSERVATION_APRIORI", InnerEnum.OBSERVATION_APRIORI, 31);
		public static readonly LayerType OBSERVATION_APOSTERIORI = new LayerType("OBSERVATION_APOSTERIORI", InnerEnum.OBSERVATION_APOSTERIORI, 32);

		public static readonly LayerType REFERENCE_POINT_APRIORI = new LayerType("REFERENCE_POINT_APRIORI", InnerEnum.REFERENCE_POINT_APRIORI, 11);
		public static readonly LayerType STOCHASTIC_POINT_APRIORI = new LayerType("STOCHASTIC_POINT_APRIORI", InnerEnum.STOCHASTIC_POINT_APRIORI, 12);
		public static readonly LayerType DATUM_POINT_APRIORI = new LayerType("DATUM_POINT_APRIORI", InnerEnum.DATUM_POINT_APRIORI, 13);
		public static readonly LayerType NEW_POINT_APRIORI = new LayerType("NEW_POINT_APRIORI", InnerEnum.NEW_POINT_APRIORI, 14);

		public static readonly LayerType POINT_SHIFT_VERTICAL = new LayerType("POINT_SHIFT_VERTICAL", InnerEnum.POINT_SHIFT_VERTICAL, 61);
		public static readonly LayerType POINT_SHIFT_HORIZONTAL = new LayerType("POINT_SHIFT_HORIZONTAL", InnerEnum.POINT_SHIFT_HORIZONTAL, 62);

		public static readonly LayerType PRINCIPAL_COMPONENT_VERTICAL = new LayerType("PRINCIPAL_COMPONENT_VERTICAL", InnerEnum.PRINCIPAL_COMPONENT_VERTICAL, 52);
		public static readonly LayerType PRINCIPAL_COMPONENT_HORIZONTAL = new LayerType("PRINCIPAL_COMPONENT_HORIZONTAL", InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL, 53);

		public static readonly LayerType POINT_RESIDUAL_VERTICAL = new LayerType("POINT_RESIDUAL_VERTICAL", InnerEnum.POINT_RESIDUAL_VERTICAL, 71);
		public static readonly LayerType POINT_RESIDUAL_HORIZONTAL = new LayerType("POINT_RESIDUAL_HORIZONTAL", InnerEnum.POINT_RESIDUAL_HORIZONTAL, 72);

		public static readonly LayerType REFERENCE_POINT_APOSTERIORI = new LayerType("REFERENCE_POINT_APOSTERIORI", InnerEnum.REFERENCE_POINT_APOSTERIORI, 21);
		public static readonly LayerType STOCHASTIC_POINT_APOSTERIORI = new LayerType("STOCHASTIC_POINT_APOSTERIORI", InnerEnum.STOCHASTIC_POINT_APOSTERIORI, 22);
		public static readonly LayerType DATUM_POINT_APOSTERIORI = new LayerType("DATUM_POINT_APOSTERIORI", InnerEnum.DATUM_POINT_APOSTERIORI, 23);
		public static readonly LayerType NEW_POINT_APOSTERIORI = new LayerType("NEW_POINT_APOSTERIORI", InnerEnum.NEW_POINT_APOSTERIORI, 24);

		public static readonly LayerType LEGEND = new LayerType("LEGEND", InnerEnum.LEGEND, 99);

		private static readonly List<LayerType> valueList = new List<LayerType>();

		static LayerType()
		{
			valueList.Add(ABSOLUTE_CONFIDENCE);
			valueList.Add(RELATIVE_CONFIDENCE);
			valueList.Add(OBSERVATION_APRIORI);
			valueList.Add(OBSERVATION_APOSTERIORI);
			valueList.Add(REFERENCE_POINT_APRIORI);
			valueList.Add(STOCHASTIC_POINT_APRIORI);
			valueList.Add(DATUM_POINT_APRIORI);
			valueList.Add(NEW_POINT_APRIORI);
			valueList.Add(POINT_SHIFT_VERTICAL);
			valueList.Add(POINT_SHIFT_HORIZONTAL);
			valueList.Add(PRINCIPAL_COMPONENT_VERTICAL);
			valueList.Add(PRINCIPAL_COMPONENT_HORIZONTAL);
			valueList.Add(POINT_RESIDUAL_VERTICAL);
			valueList.Add(POINT_RESIDUAL_HORIZONTAL);
			valueList.Add(REFERENCE_POINT_APOSTERIORI);
			valueList.Add(STOCHASTIC_POINT_APOSTERIORI);
			valueList.Add(DATUM_POINT_APOSTERIORI);
			valueList.Add(NEW_POINT_APOSTERIORI);
			valueList.Add(LEGEND);
		}

		public enum InnerEnum
		{
			ABSOLUTE_CONFIDENCE,
			RELATIVE_CONFIDENCE,
			OBSERVATION_APRIORI,
			OBSERVATION_APOSTERIORI,
			REFERENCE_POINT_APRIORI,
			STOCHASTIC_POINT_APRIORI,
			DATUM_POINT_APRIORI,
			NEW_POINT_APRIORI,
			POINT_SHIFT_VERTICAL,
			POINT_SHIFT_HORIZONTAL,
			PRINCIPAL_COMPONENT_VERTICAL,
			PRINCIPAL_COMPONENT_HORIZONTAL,
			POINT_RESIDUAL_VERTICAL,
			POINT_RESIDUAL_HORIZONTAL,
			REFERENCE_POINT_APOSTERIORI,
			STOCHASTIC_POINT_APOSTERIORI,
			DATUM_POINT_APOSTERIORI,
			NEW_POINT_APOSTERIORI,
			LEGEND
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private LayerType(string name, InnerEnum innerEnum, int id)
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

		public static LayerType getEnumByValue(int value)
		{
			foreach (LayerType element in LayerType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static LayerType[] values()
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

		public static LayerType valueOf(string name)
		{
			foreach (LayerType enumInstance in LayerType.valueList)
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