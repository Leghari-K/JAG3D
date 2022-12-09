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
	public sealed class ParameterType
	{

		public static readonly ParameterType ORIENTATION = new ParameterType("ORIENTATION", InnerEnum.ORIENTATION, 1);
		public static readonly ParameterType SCALE = new ParameterType("SCALE", InnerEnum.SCALE, 2);
		public static readonly ParameterType ZERO_POINT_OFFSET = new ParameterType("ZERO_POINT_OFFSET", InnerEnum.ZERO_POINT_OFFSET, 3);
		public static readonly ParameterType REFRACTION_INDEX = new ParameterType("REFRACTION_INDEX", InnerEnum.REFRACTION_INDEX, 4);

		public static readonly ParameterType POINT1D = new ParameterType("POINT1D", InnerEnum.POINT1D, 5);
		public static readonly ParameterType POINT2D = new ParameterType("POINT2D", InnerEnum.POINT2D, 6);
		public static readonly ParameterType POINT3D = new ParameterType("POINT3D", InnerEnum.POINT3D, 7);

		public static readonly ParameterType ROTATION_X = new ParameterType("ROTATION_X", InnerEnum.ROTATION_X, 8);
		public static readonly ParameterType ROTATION_Y = new ParameterType("ROTATION_Y", InnerEnum.ROTATION_Y, 9);
		public static readonly ParameterType ROTATION_Z = new ParameterType("ROTATION_Z", InnerEnum.ROTATION_Z, 10);

		public static readonly ParameterType VERTICAL_DEFLECTION_X = new ParameterType("VERTICAL_DEFLECTION_X", InnerEnum.VERTICAL_DEFLECTION_X, 11);
		public static readonly ParameterType VERTICAL_DEFLECTION_Y = new ParameterType("VERTICAL_DEFLECTION_Y", InnerEnum.VERTICAL_DEFLECTION_Y, 12);

		public static readonly ParameterType STRAIN_A11 = new ParameterType("STRAIN_A11", InnerEnum.STRAIN_A11, 101);
		public static readonly ParameterType STRAIN_A12 = new ParameterType("STRAIN_A12", InnerEnum.STRAIN_A12, 102);
		public static readonly ParameterType STRAIN_A21 = new ParameterType("STRAIN_A21", InnerEnum.STRAIN_A21, 103);
		public static readonly ParameterType STRAIN_A22 = new ParameterType("STRAIN_A22", InnerEnum.STRAIN_A22, 104);

		public static readonly ParameterType STRAIN_Q0 = new ParameterType("STRAIN_Q0", InnerEnum.STRAIN_Q0, 111);
		public static readonly ParameterType STRAIN_Q1 = new ParameterType("STRAIN_Q1", InnerEnum.STRAIN_Q1, 112);
		public static readonly ParameterType STRAIN_Q2 = new ParameterType("STRAIN_Q2", InnerEnum.STRAIN_Q2, 113);
		public static readonly ParameterType STRAIN_Q3 = new ParameterType("STRAIN_Q3", InnerEnum.STRAIN_Q3, 114);

		public static readonly ParameterType STRAIN_S11 = new ParameterType("STRAIN_S11", InnerEnum.STRAIN_S11, 121);
		public static readonly ParameterType STRAIN_S12 = new ParameterType("STRAIN_S12", InnerEnum.STRAIN_S12, 122);
		public static readonly ParameterType STRAIN_S13 = new ParameterType("STRAIN_S13", InnerEnum.STRAIN_S13, 123);
		public static readonly ParameterType STRAIN_S22 = new ParameterType("STRAIN_S22", InnerEnum.STRAIN_S22, 124);
		public static readonly ParameterType STRAIN_S23 = new ParameterType("STRAIN_S23", InnerEnum.STRAIN_S23, 125);
		public static readonly ParameterType STRAIN_S33 = new ParameterType("STRAIN_S33", InnerEnum.STRAIN_S33, 126);

		public static readonly ParameterType STRAIN_TRANSLATION_X = new ParameterType("STRAIN_TRANSLATION_X", InnerEnum.STRAIN_TRANSLATION_X, 131);
		public static readonly ParameterType STRAIN_TRANSLATION_Y = new ParameterType("STRAIN_TRANSLATION_Y", InnerEnum.STRAIN_TRANSLATION_Y, 132);
		public static readonly ParameterType STRAIN_TRANSLATION_Z = new ParameterType("STRAIN_TRANSLATION_Z", InnerEnum.STRAIN_TRANSLATION_Z, 133);

		public static readonly ParameterType STRAIN_SCALE_X = new ParameterType("STRAIN_SCALE_X", InnerEnum.STRAIN_SCALE_X, 141);
		public static readonly ParameterType STRAIN_SCALE_Y = new ParameterType("STRAIN_SCALE_Y", InnerEnum.STRAIN_SCALE_Y, 142);
		public static readonly ParameterType STRAIN_SCALE_Z = new ParameterType("STRAIN_SCALE_Z", InnerEnum.STRAIN_SCALE_Z, 143);

		public static readonly ParameterType STRAIN_ROTATION_X = new ParameterType("STRAIN_ROTATION_X", InnerEnum.STRAIN_ROTATION_X, 151);
		public static readonly ParameterType STRAIN_ROTATION_Y = new ParameterType("STRAIN_ROTATION_Y", InnerEnum.STRAIN_ROTATION_Y, 152);
		public static readonly ParameterType STRAIN_ROTATION_Z = new ParameterType("STRAIN_ROTATION_Z", InnerEnum.STRAIN_ROTATION_Z, 153);

		public static readonly ParameterType STRAIN_SHEAR_X = new ParameterType("STRAIN_SHEAR_X", InnerEnum.STRAIN_SHEAR_X, 161);
		public static readonly ParameterType STRAIN_SHEAR_Y = new ParameterType("STRAIN_SHEAR_Y", InnerEnum.STRAIN_SHEAR_Y, 162);
		public static readonly ParameterType STRAIN_SHEAR_Z = new ParameterType("STRAIN_SHEAR_Z", InnerEnum.STRAIN_SHEAR_Z, 163);

		private static readonly List<ParameterType> valueList = new List<ParameterType>();

		static ParameterType()
		{
			valueList.Add(ORIENTATION);
			valueList.Add(SCALE);
			valueList.Add(ZERO_POINT_OFFSET);
			valueList.Add(REFRACTION_INDEX);
			valueList.Add(POINT1D);
			valueList.Add(POINT2D);
			valueList.Add(POINT3D);
			valueList.Add(ROTATION_X);
			valueList.Add(ROTATION_Y);
			valueList.Add(ROTATION_Z);
			valueList.Add(VERTICAL_DEFLECTION_X);
			valueList.Add(VERTICAL_DEFLECTION_Y);
			valueList.Add(STRAIN_A11);
			valueList.Add(STRAIN_A12);
			valueList.Add(STRAIN_A21);
			valueList.Add(STRAIN_A22);
			valueList.Add(STRAIN_Q0);
			valueList.Add(STRAIN_Q1);
			valueList.Add(STRAIN_Q2);
			valueList.Add(STRAIN_Q3);
			valueList.Add(STRAIN_S11);
			valueList.Add(STRAIN_S12);
			valueList.Add(STRAIN_S13);
			valueList.Add(STRAIN_S22);
			valueList.Add(STRAIN_S23);
			valueList.Add(STRAIN_S33);
			valueList.Add(STRAIN_TRANSLATION_X);
			valueList.Add(STRAIN_TRANSLATION_Y);
			valueList.Add(STRAIN_TRANSLATION_Z);
			valueList.Add(STRAIN_SCALE_X);
			valueList.Add(STRAIN_SCALE_Y);
			valueList.Add(STRAIN_SCALE_Z);
			valueList.Add(STRAIN_ROTATION_X);
			valueList.Add(STRAIN_ROTATION_Y);
			valueList.Add(STRAIN_ROTATION_Z);
			valueList.Add(STRAIN_SHEAR_X);
			valueList.Add(STRAIN_SHEAR_Y);
			valueList.Add(STRAIN_SHEAR_Z);
		}

		public enum InnerEnum
		{
			ORIENTATION,
			SCALE,
			ZERO_POINT_OFFSET,
			REFRACTION_INDEX,
			POINT1D,
			POINT2D,
			POINT3D,
			ROTATION_X,
			ROTATION_Y,
			ROTATION_Z,
			VERTICAL_DEFLECTION_X,
			VERTICAL_DEFLECTION_Y,
			STRAIN_A11,
			STRAIN_A12,
			STRAIN_A21,
			STRAIN_A22,
			STRAIN_Q0,
			STRAIN_Q1,
			STRAIN_Q2,
			STRAIN_Q3,
			STRAIN_S11,
			STRAIN_S12,
			STRAIN_S13,
			STRAIN_S22,
			STRAIN_S23,
			STRAIN_S33,
			STRAIN_TRANSLATION_X,
			STRAIN_TRANSLATION_Y,
			STRAIN_TRANSLATION_Z,
			STRAIN_SCALE_X,
			STRAIN_SCALE_Y,
			STRAIN_SCALE_Z,
			STRAIN_ROTATION_X,
			STRAIN_ROTATION_Y,
			STRAIN_ROTATION_Z,
			STRAIN_SHEAR_X,
			STRAIN_SHEAR_Y,
			STRAIN_SHEAR_Z
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private int id;
		private ParameterType(string name, InnerEnum innerEnum, int id)
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

		public static ParameterType getEnumByValue(int value)
		{
			foreach (ParameterType element in ParameterType.values())
			{
				if (element.id == value)
				{
					return element;
				}
			}
			return null;
		}

		public static ParameterType[] values()
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

		public static ParameterType valueOf(string name)
		{
			foreach (ParameterType enumInstance in ParameterType.valueList)
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