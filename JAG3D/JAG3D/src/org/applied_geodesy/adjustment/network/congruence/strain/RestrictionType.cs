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

namespace org.applied_geodesy.adjustment.network.congruence.strain
{
	public sealed class RestrictionType
	{
		public static readonly RestrictionType UNIT_QUATERNION = new RestrictionType("UNIT_QUATERNION", InnerEnum.UNIT_QUATERNION, 0);

		public static readonly RestrictionType FIXED_TRANSLATION_X = new RestrictionType("FIXED_TRANSLATION_X", InnerEnum.FIXED_TRANSLATION_X, 1);
		public static readonly RestrictionType FIXED_TRANSLATION_Y = new RestrictionType("FIXED_TRANSLATION_Y", InnerEnum.FIXED_TRANSLATION_Y, 2);
		public static readonly RestrictionType FIXED_TRANSLATION_Z = new RestrictionType("FIXED_TRANSLATION_Z", InnerEnum.FIXED_TRANSLATION_Z, 3);

		public static readonly RestrictionType FIXED_ROTATION_X = new RestrictionType("FIXED_ROTATION_X", InnerEnum.FIXED_ROTATION_X, 4);
		public static readonly RestrictionType FIXED_ROTATION_Y = new RestrictionType("FIXED_ROTATION_Y", InnerEnum.FIXED_ROTATION_Y, 5);
		public static readonly RestrictionType FIXED_ROTATION_Z = new RestrictionType("FIXED_ROTATION_Z", InnerEnum.FIXED_ROTATION_Z, 6);

		public static readonly RestrictionType FIXED_SCALE_X = new RestrictionType("FIXED_SCALE_X", InnerEnum.FIXED_SCALE_X, 7);
		public static readonly RestrictionType FIXED_SCALE_Y = new RestrictionType("FIXED_SCALE_Y", InnerEnum.FIXED_SCALE_Y, 8);
		public static readonly RestrictionType FIXED_SCALE_Z = new RestrictionType("FIXED_SCALE_Z", InnerEnum.FIXED_SCALE_Z, 9);

		public static readonly RestrictionType FIXED_SHEAR_X = new RestrictionType("FIXED_SHEAR_X", InnerEnum.FIXED_SHEAR_X, 10);
		public static readonly RestrictionType FIXED_SHEAR_Y = new RestrictionType("FIXED_SHEAR_Y", InnerEnum.FIXED_SHEAR_Y, 11);
		public static readonly RestrictionType FIXED_SHEAR_Z = new RestrictionType("FIXED_SHEAR_Z", InnerEnum.FIXED_SHEAR_Z, 12);

		public static readonly RestrictionType IDENT_SCALES_XY = new RestrictionType("IDENT_SCALES_XY", InnerEnum.IDENT_SCALES_XY, 78);
		public static readonly RestrictionType IDENT_SCALES_XZ = new RestrictionType("IDENT_SCALES_XZ", InnerEnum.IDENT_SCALES_XZ, 79);
		public static readonly RestrictionType IDENT_SCALES_YZ = new RestrictionType("IDENT_SCALES_YZ", InnerEnum.IDENT_SCALES_YZ, 89);

		private static readonly List<RestrictionType> valueList = new List<RestrictionType>();

		static RestrictionType()
		{
			valueList.Add(UNIT_QUATERNION);
			valueList.Add(FIXED_TRANSLATION_X);
			valueList.Add(FIXED_TRANSLATION_Y);
			valueList.Add(FIXED_TRANSLATION_Z);
			valueList.Add(FIXED_ROTATION_X);
			valueList.Add(FIXED_ROTATION_Y);
			valueList.Add(FIXED_ROTATION_Z);
			valueList.Add(FIXED_SCALE_X);
			valueList.Add(FIXED_SCALE_Y);
			valueList.Add(FIXED_SCALE_Z);
			valueList.Add(FIXED_SHEAR_X);
			valueList.Add(FIXED_SHEAR_Y);
			valueList.Add(FIXED_SHEAR_Z);
			valueList.Add(IDENT_SCALES_XY);
			valueList.Add(IDENT_SCALES_XZ);
			valueList.Add(IDENT_SCALES_YZ);
		}

		public enum InnerEnum
		{
			UNIT_QUATERNION,
			FIXED_TRANSLATION_X,
			FIXED_TRANSLATION_Y,
			FIXED_TRANSLATION_Z,
			FIXED_ROTATION_X,
			FIXED_ROTATION_Y,
			FIXED_ROTATION_Z,
			FIXED_SCALE_X,
			FIXED_SCALE_Y,
			FIXED_SCALE_Z,
			FIXED_SHEAR_X,
			FIXED_SHEAR_Y,
			FIXED_SHEAR_Z,
			IDENT_SCALES_XY,
			IDENT_SCALES_XZ,
			IDENT_SCALES_YZ
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private readonly int id;
		private RestrictionType(string name, InnerEnum innerEnum, int id)
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

		public static RestrictionType getEnumByValue(int id)
		{
			RestrictionType[] restrictions = RestrictionType.values();
			foreach (RestrictionType restriction in restrictions)
			{
				if (restriction.Id == id)
				{
					return restriction;
				}
			}
			return null;
		}

		public static RestrictionType[] values()
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

		public static RestrictionType valueOf(string name)
		{
			foreach (RestrictionType enumInstance in RestrictionType.valueList)
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