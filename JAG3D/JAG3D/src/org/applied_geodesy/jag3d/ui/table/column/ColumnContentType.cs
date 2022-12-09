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
	public sealed class ColumnContentType
	{
		public static readonly ColumnContentType DEFAULT = new ColumnContentType("DEFAULT", InnerEnum.DEFAULT, 0);

		public static readonly ColumnContentType ENABLE = new ColumnContentType("ENABLE", InnerEnum.ENABLE, 1);
		public static readonly ColumnContentType SIGNIFICANT = new ColumnContentType("SIGNIFICANT", InnerEnum.SIGNIFICANT, 2);

		public static readonly ColumnContentType POINT_NAME = new ColumnContentType("POINT_NAME", InnerEnum.POINT_NAME, 10);
		public static readonly ColumnContentType START_POINT_NAME = new ColumnContentType("START_POINT_NAME", InnerEnum.START_POINT_NAME, 11);
		public static readonly ColumnContentType END_POINT_NAME = new ColumnContentType("END_POINT_NAME", InnerEnum.END_POINT_NAME, 12);
		public static readonly ColumnContentType PARAMETER_NAME = new ColumnContentType("PARAMETER_NAME", InnerEnum.PARAMETER_NAME, 13);
		public static readonly ColumnContentType VARIANCE_COMPONENT_TYPE = new ColumnContentType("VARIANCE_COMPONENT_TYPE", InnerEnum.VARIANCE_COMPONENT_TYPE, 14);
		public static readonly ColumnContentType VARIANCE_COMPONENT_NAME = new ColumnContentType("VARIANCE_COMPONENT_NAME", InnerEnum.VARIANCE_COMPONENT_NAME, 15);
		public static readonly ColumnContentType CODE = new ColumnContentType("CODE", InnerEnum.CODE, 16);

		public static readonly ColumnContentType INSTRUMENT_HEIGHT = new ColumnContentType("INSTRUMENT_HEIGHT", InnerEnum.INSTRUMENT_HEIGHT, 20);
		public static readonly ColumnContentType REFLECTOR_HEIGHT = new ColumnContentType("REFLECTOR_HEIGHT", InnerEnum.REFLECTOR_HEIGHT, 21);

		public static readonly ColumnContentType VALUE_X_APRIORI = new ColumnContentType("VALUE_X_APRIORI", InnerEnum.VALUE_X_APRIORI, 30);
		public static readonly ColumnContentType VALUE_Y_APRIORI = new ColumnContentType("VALUE_Y_APRIORI", InnerEnum.VALUE_Y_APRIORI, 31);
		public static readonly ColumnContentType VALUE_Z_APRIORI = new ColumnContentType("VALUE_Z_APRIORI", InnerEnum.VALUE_Z_APRIORI, 32);

		public static readonly ColumnContentType VALUE_X_APOSTERIORI = new ColumnContentType("VALUE_X_APOSTERIORI", InnerEnum.VALUE_X_APOSTERIORI, 33);
		public static readonly ColumnContentType VALUE_Y_APOSTERIORI = new ColumnContentType("VALUE_Y_APOSTERIORI", InnerEnum.VALUE_Y_APOSTERIORI, 34);
		public static readonly ColumnContentType VALUE_Z_APOSTERIORI = new ColumnContentType("VALUE_Z_APOSTERIORI", InnerEnum.VALUE_Z_APOSTERIORI, 35);

		public static readonly ColumnContentType VALUE_APRIORI = new ColumnContentType("VALUE_APRIORI", InnerEnum.VALUE_APRIORI, 40);
		public static readonly ColumnContentType VALUE_APOSTERIORI = new ColumnContentType("VALUE_APOSTERIORI", InnerEnum.VALUE_APOSTERIORI, 41);

		public static readonly ColumnContentType APPROXIMATED_DISTANCE_APRIORI = new ColumnContentType("APPROXIMATED_DISTANCE_APRIORI", InnerEnum.APPROXIMATED_DISTANCE_APRIORI, 50);
		public static readonly ColumnContentType UNCERTAINTY_APRIORI = new ColumnContentType("UNCERTAINTY_APRIORI", InnerEnum.UNCERTAINTY_APRIORI, 51);
		public static readonly ColumnContentType UNCERTAINTY_X_APRIORI = new ColumnContentType("UNCERTAINTY_X_APRIORI", InnerEnum.UNCERTAINTY_X_APRIORI, 52);
		public static readonly ColumnContentType UNCERTAINTY_Y_APRIORI = new ColumnContentType("UNCERTAINTY_Y_APRIORI", InnerEnum.UNCERTAINTY_Y_APRIORI, 53);
		public static readonly ColumnContentType UNCERTAINTY_Z_APRIORI = new ColumnContentType("UNCERTAINTY_Z_APRIORI", InnerEnum.UNCERTAINTY_Z_APRIORI, 54);

		public static readonly ColumnContentType UNCERTAINTY_APOSTERIORI = new ColumnContentType("UNCERTAINTY_APOSTERIORI", InnerEnum.UNCERTAINTY_APOSTERIORI, 55);
		public static readonly ColumnContentType UNCERTAINTY_X_APOSTERIORI = new ColumnContentType("UNCERTAINTY_X_APOSTERIORI", InnerEnum.UNCERTAINTY_X_APOSTERIORI, 56);
		public static readonly ColumnContentType UNCERTAINTY_Y_APOSTERIORI = new ColumnContentType("UNCERTAINTY_Y_APOSTERIORI", InnerEnum.UNCERTAINTY_Y_APOSTERIORI, 57);
		public static readonly ColumnContentType UNCERTAINTY_Z_APOSTERIORI = new ColumnContentType("UNCERTAINTY_Z_APOSTERIORI", InnerEnum.UNCERTAINTY_Z_APOSTERIORI, 58);

		public static readonly ColumnContentType RESIDUAL = new ColumnContentType("RESIDUAL", InnerEnum.RESIDUAL, 60);
		public static readonly ColumnContentType RESIDUAL_X = new ColumnContentType("RESIDUAL_X", InnerEnum.RESIDUAL_X, 61);
		public static readonly ColumnContentType RESIDUAL_Y = new ColumnContentType("RESIDUAL_Y", InnerEnum.RESIDUAL_Y, 62);
		public static readonly ColumnContentType RESIDUAL_Z = new ColumnContentType("RESIDUAL_Z", InnerEnum.RESIDUAL_Z, 63);

		public static readonly ColumnContentType REDUNDANCY = new ColumnContentType("REDUNDANCY", InnerEnum.REDUNDANCY, 70);
		public static readonly ColumnContentType REDUNDANCY_X = new ColumnContentType("REDUNDANCY_X", InnerEnum.REDUNDANCY_X, 71);
		public static readonly ColumnContentType REDUNDANCY_Y = new ColumnContentType("REDUNDANCY_Y", InnerEnum.REDUNDANCY_Y, 72);
		public static readonly ColumnContentType REDUNDANCY_Z = new ColumnContentType("REDUNDANCY_Z", InnerEnum.REDUNDANCY_Z, 73);
		public static readonly ColumnContentType NUMBER_OF_OBSERVATION = new ColumnContentType("NUMBER_OF_OBSERVATION", InnerEnum.NUMBER_OF_OBSERVATION, 74);

		public static readonly ColumnContentType GROSS_ERROR = new ColumnContentType("GROSS_ERROR", InnerEnum.GROSS_ERROR, 80);
		public static readonly ColumnContentType GROSS_ERROR_X = new ColumnContentType("GROSS_ERROR_X", InnerEnum.GROSS_ERROR_X, 81);
		public static readonly ColumnContentType GROSS_ERROR_Y = new ColumnContentType("GROSS_ERROR_Y", InnerEnum.GROSS_ERROR_Y, 82);
		public static readonly ColumnContentType GROSS_ERROR_Z = new ColumnContentType("GROSS_ERROR_Z", InnerEnum.GROSS_ERROR_Z, 83);

		public static readonly ColumnContentType MINIMAL_DETECTABLE_BIAS = new ColumnContentType("MINIMAL_DETECTABLE_BIAS", InnerEnum.MINIMAL_DETECTABLE_BIAS, 90);
		public static readonly ColumnContentType MINIMAL_DETECTABLE_BIAS_X = new ColumnContentType("MINIMAL_DETECTABLE_BIAS_X", InnerEnum.MINIMAL_DETECTABLE_BIAS_X, 91);
		public static readonly ColumnContentType MINIMAL_DETECTABLE_BIAS_Y = new ColumnContentType("MINIMAL_DETECTABLE_BIAS_Y", InnerEnum.MINIMAL_DETECTABLE_BIAS_Y, 92);
		public static readonly ColumnContentType MINIMAL_DETECTABLE_BIAS_Z = new ColumnContentType("MINIMAL_DETECTABLE_BIAS_Z", InnerEnum.MINIMAL_DETECTABLE_BIAS_Z, 93);

		public static readonly ColumnContentType MAXIMUM_TOLERABLE_BIAS = new ColumnContentType("MAXIMUM_TOLERABLE_BIAS", InnerEnum.MAXIMUM_TOLERABLE_BIAS, 150);
		public static readonly ColumnContentType MAXIMUM_TOLERABLE_BIAS_X = new ColumnContentType("MAXIMUM_TOLERABLE_BIAS_X", InnerEnum.MAXIMUM_TOLERABLE_BIAS_X, 151);
		public static readonly ColumnContentType MAXIMUM_TOLERABLE_BIAS_Y = new ColumnContentType("MAXIMUM_TOLERABLE_BIAS_Y", InnerEnum.MAXIMUM_TOLERABLE_BIAS_Y, 152);
		public static readonly ColumnContentType MAXIMUM_TOLERABLE_BIAS_Z = new ColumnContentType("MAXIMUM_TOLERABLE_BIAS_Z", InnerEnum.MAXIMUM_TOLERABLE_BIAS_Z, 153);

		public static readonly ColumnContentType INFLUENCE_ON_POINT_POSITION = new ColumnContentType("INFLUENCE_ON_POINT_POSITION", InnerEnum.INFLUENCE_ON_POINT_POSITION, 100);
		public static readonly ColumnContentType INFLUENCE_ON_POINT_POSITION_X = new ColumnContentType("INFLUENCE_ON_POINT_POSITION_X", InnerEnum.INFLUENCE_ON_POINT_POSITION_X, 101);
		public static readonly ColumnContentType INFLUENCE_ON_POINT_POSITION_Y = new ColumnContentType("INFLUENCE_ON_POINT_POSITION_Y", InnerEnum.INFLUENCE_ON_POINT_POSITION_Y, 102);
		public static readonly ColumnContentType INFLUENCE_ON_POINT_POSITION_Z = new ColumnContentType("INFLUENCE_ON_POINT_POSITION_Z", InnerEnum.INFLUENCE_ON_POINT_POSITION_Z, 103);

		public static readonly ColumnContentType INFLUENCE_ON_NETWORK_DISTORTION = new ColumnContentType("INFLUENCE_ON_NETWORK_DISTORTION", InnerEnum.INFLUENCE_ON_NETWORK_DISTORTION, 110);

		public static readonly ColumnContentType FIRST_PRINCIPLE_COMPONENT_X = new ColumnContentType("FIRST_PRINCIPLE_COMPONENT_X", InnerEnum.FIRST_PRINCIPLE_COMPONENT_X, 120);
		public static readonly ColumnContentType FIRST_PRINCIPLE_COMPONENT_Y = new ColumnContentType("FIRST_PRINCIPLE_COMPONENT_Y", InnerEnum.FIRST_PRINCIPLE_COMPONENT_Y, 121);
		public static readonly ColumnContentType FIRST_PRINCIPLE_COMPONENT_Z = new ColumnContentType("FIRST_PRINCIPLE_COMPONENT_Z", InnerEnum.FIRST_PRINCIPLE_COMPONENT_Z, 122);

		public static readonly ColumnContentType CONFIDENCE_A = new ColumnContentType("CONFIDENCE_A", InnerEnum.CONFIDENCE_A, 130);
		public static readonly ColumnContentType CONFIDENCE_B = new ColumnContentType("CONFIDENCE_B", InnerEnum.CONFIDENCE_B, 131);
		public static readonly ColumnContentType CONFIDENCE_C = new ColumnContentType("CONFIDENCE_C", InnerEnum.CONFIDENCE_C, 132);
		public static readonly ColumnContentType CONFIDENCE_ALPHA = new ColumnContentType("CONFIDENCE_ALPHA", InnerEnum.CONFIDENCE_ALPHA, 133);
		public static readonly ColumnContentType CONFIDENCE_BETA = new ColumnContentType("CONFIDENCE_BETA", InnerEnum.CONFIDENCE_BETA, 134);
		public static readonly ColumnContentType CONFIDENCE_GAMMA = new ColumnContentType("CONFIDENCE_GAMMA", InnerEnum.CONFIDENCE_GAMMA, 135);

		public static readonly ColumnContentType OMEGA = new ColumnContentType("OMEGA", InnerEnum.OMEGA, 140);
		public static readonly ColumnContentType P_VALUE_APRIORI = new ColumnContentType("P_VALUE_APRIORI", InnerEnum.P_VALUE_APRIORI, 141);
		public static readonly ColumnContentType P_VALUE_APOSTERIORI = new ColumnContentType("P_VALUE_APOSTERIORI", InnerEnum.P_VALUE_APOSTERIORI, 142);
		public static readonly ColumnContentType TEST_STATISTIC_APRIORI = new ColumnContentType("TEST_STATISTIC_APRIORI", InnerEnum.TEST_STATISTIC_APRIORI, 143);
		public static readonly ColumnContentType TEST_STATISTIC_APOSTERIORI = new ColumnContentType("TEST_STATISTIC_APOSTERIORI", InnerEnum.TEST_STATISTIC_APOSTERIORI, 144);
		public static readonly ColumnContentType VARIANCE = new ColumnContentType("VARIANCE", InnerEnum.VARIANCE, 145);

		private static readonly List<ColumnContentType> valueList = new List<ColumnContentType>();

		static ColumnContentType()
		{
			valueList.Add(DEFAULT);
			valueList.Add(ENABLE);
			valueList.Add(SIGNIFICANT);
			valueList.Add(POINT_NAME);
			valueList.Add(START_POINT_NAME);
			valueList.Add(END_POINT_NAME);
			valueList.Add(PARAMETER_NAME);
			valueList.Add(VARIANCE_COMPONENT_TYPE);
			valueList.Add(VARIANCE_COMPONENT_NAME);
			valueList.Add(CODE);
			valueList.Add(INSTRUMENT_HEIGHT);
			valueList.Add(REFLECTOR_HEIGHT);
			valueList.Add(VALUE_X_APRIORI);
			valueList.Add(VALUE_Y_APRIORI);
			valueList.Add(VALUE_Z_APRIORI);
			valueList.Add(VALUE_X_APOSTERIORI);
			valueList.Add(VALUE_Y_APOSTERIORI);
			valueList.Add(VALUE_Z_APOSTERIORI);
			valueList.Add(VALUE_APRIORI);
			valueList.Add(VALUE_APOSTERIORI);
			valueList.Add(APPROXIMATED_DISTANCE_APRIORI);
			valueList.Add(UNCERTAINTY_APRIORI);
			valueList.Add(UNCERTAINTY_X_APRIORI);
			valueList.Add(UNCERTAINTY_Y_APRIORI);
			valueList.Add(UNCERTAINTY_Z_APRIORI);
			valueList.Add(UNCERTAINTY_APOSTERIORI);
			valueList.Add(UNCERTAINTY_X_APOSTERIORI);
			valueList.Add(UNCERTAINTY_Y_APOSTERIORI);
			valueList.Add(UNCERTAINTY_Z_APOSTERIORI);
			valueList.Add(RESIDUAL);
			valueList.Add(RESIDUAL_X);
			valueList.Add(RESIDUAL_Y);
			valueList.Add(RESIDUAL_Z);
			valueList.Add(REDUNDANCY);
			valueList.Add(REDUNDANCY_X);
			valueList.Add(REDUNDANCY_Y);
			valueList.Add(REDUNDANCY_Z);
			valueList.Add(NUMBER_OF_OBSERVATION);
			valueList.Add(GROSS_ERROR);
			valueList.Add(GROSS_ERROR_X);
			valueList.Add(GROSS_ERROR_Y);
			valueList.Add(GROSS_ERROR_Z);
			valueList.Add(MINIMAL_DETECTABLE_BIAS);
			valueList.Add(MINIMAL_DETECTABLE_BIAS_X);
			valueList.Add(MINIMAL_DETECTABLE_BIAS_Y);
			valueList.Add(MINIMAL_DETECTABLE_BIAS_Z);
			valueList.Add(MAXIMUM_TOLERABLE_BIAS);
			valueList.Add(MAXIMUM_TOLERABLE_BIAS_X);
			valueList.Add(MAXIMUM_TOLERABLE_BIAS_Y);
			valueList.Add(MAXIMUM_TOLERABLE_BIAS_Z);
			valueList.Add(INFLUENCE_ON_POINT_POSITION);
			valueList.Add(INFLUENCE_ON_POINT_POSITION_X);
			valueList.Add(INFLUENCE_ON_POINT_POSITION_Y);
			valueList.Add(INFLUENCE_ON_POINT_POSITION_Z);
			valueList.Add(INFLUENCE_ON_NETWORK_DISTORTION);
			valueList.Add(FIRST_PRINCIPLE_COMPONENT_X);
			valueList.Add(FIRST_PRINCIPLE_COMPONENT_Y);
			valueList.Add(FIRST_PRINCIPLE_COMPONENT_Z);
			valueList.Add(CONFIDENCE_A);
			valueList.Add(CONFIDENCE_B);
			valueList.Add(CONFIDENCE_C);
			valueList.Add(CONFIDENCE_ALPHA);
			valueList.Add(CONFIDENCE_BETA);
			valueList.Add(CONFIDENCE_GAMMA);
			valueList.Add(OMEGA);
			valueList.Add(P_VALUE_APRIORI);
			valueList.Add(P_VALUE_APOSTERIORI);
			valueList.Add(TEST_STATISTIC_APRIORI);
			valueList.Add(TEST_STATISTIC_APOSTERIORI);
			valueList.Add(VARIANCE);
		}

		public enum InnerEnum
		{
			DEFAULT,
			ENABLE,
			SIGNIFICANT,
			POINT_NAME,
			START_POINT_NAME,
			END_POINT_NAME,
			PARAMETER_NAME,
			VARIANCE_COMPONENT_TYPE,
			VARIANCE_COMPONENT_NAME,
			CODE,
			INSTRUMENT_HEIGHT,
			REFLECTOR_HEIGHT,
			VALUE_X_APRIORI,
			VALUE_Y_APRIORI,
			VALUE_Z_APRIORI,
			VALUE_X_APOSTERIORI,
			VALUE_Y_APOSTERIORI,
			VALUE_Z_APOSTERIORI,
			VALUE_APRIORI,
			VALUE_APOSTERIORI,
			APPROXIMATED_DISTANCE_APRIORI,
			UNCERTAINTY_APRIORI,
			UNCERTAINTY_X_APRIORI,
			UNCERTAINTY_Y_APRIORI,
			UNCERTAINTY_Z_APRIORI,
			UNCERTAINTY_APOSTERIORI,
			UNCERTAINTY_X_APOSTERIORI,
			UNCERTAINTY_Y_APOSTERIORI,
			UNCERTAINTY_Z_APOSTERIORI,
			RESIDUAL,
			RESIDUAL_X,
			RESIDUAL_Y,
			RESIDUAL_Z,
			REDUNDANCY,
			REDUNDANCY_X,
			REDUNDANCY_Y,
			REDUNDANCY_Z,
			NUMBER_OF_OBSERVATION,
			GROSS_ERROR,
			GROSS_ERROR_X,
			GROSS_ERROR_Y,
			GROSS_ERROR_Z,
			MINIMAL_DETECTABLE_BIAS,
			MINIMAL_DETECTABLE_BIAS_X,
			MINIMAL_DETECTABLE_BIAS_Y,
			MINIMAL_DETECTABLE_BIAS_Z,
			MAXIMUM_TOLERABLE_BIAS,
			MAXIMUM_TOLERABLE_BIAS_X,
			MAXIMUM_TOLERABLE_BIAS_Y,
			MAXIMUM_TOLERABLE_BIAS_Z,
			INFLUENCE_ON_POINT_POSITION,
			INFLUENCE_ON_POINT_POSITION_X,
			INFLUENCE_ON_POINT_POSITION_Y,
			INFLUENCE_ON_POINT_POSITION_Z,
			INFLUENCE_ON_NETWORK_DISTORTION,
			FIRST_PRINCIPLE_COMPONENT_X,
			FIRST_PRINCIPLE_COMPONENT_Y,
			FIRST_PRINCIPLE_COMPONENT_Z,
			CONFIDENCE_A,
			CONFIDENCE_B,
			CONFIDENCE_C,
			CONFIDENCE_ALPHA,
			CONFIDENCE_BETA,
			CONFIDENCE_GAMMA,
			OMEGA,
			P_VALUE_APRIORI,
			P_VALUE_APOSTERIORI,
			TEST_STATISTIC_APRIORI,
			TEST_STATISTIC_APOSTERIORI,
			VARIANCE
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private ColumnContentType(string name, InnerEnum innerEnum, int id)
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

		public static ColumnContentType getEnumByValue(int value)
		{
			foreach (ColumnContentType element in ColumnContentType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static ColumnContentType[] values()
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

		public static ColumnContentType valueOf(string name)
		{
			foreach (ColumnContentType enumInstance in ColumnContentType.valueList)
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