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

namespace org.applied_geodesy.adjustment.geometry
{

	public sealed class PrimitiveType
	{
		public static readonly PrimitiveType LINE = new PrimitiveType("LINE", InnerEnum.LINE, FeatureType.CURVE);
		public static readonly PrimitiveType CIRCLE = new PrimitiveType("CIRCLE", InnerEnum.CIRCLE, FeatureType.CURVE);
		public static readonly PrimitiveType ELLIPSE = new PrimitiveType("ELLIPSE", InnerEnum.ELLIPSE, FeatureType.CURVE);
		public static readonly PrimitiveType QUADRATIC_CURVE = new PrimitiveType("QUADRATIC_CURVE", InnerEnum.QUADRATIC_CURVE, FeatureType.CURVE);

		public static readonly PrimitiveType PLANE = new PrimitiveType("PLANE", InnerEnum.PLANE, FeatureType.SURFACE);
		public static readonly PrimitiveType SPHERE = new PrimitiveType("SPHERE", InnerEnum.SPHERE, FeatureType.SURFACE);
		public static readonly PrimitiveType ELLIPSOID = new PrimitiveType("ELLIPSOID", InnerEnum.ELLIPSOID, FeatureType.SURFACE);
		public static readonly PrimitiveType CYLINDER = new PrimitiveType("CYLINDER", InnerEnum.CYLINDER, FeatureType.SURFACE);
		public static readonly PrimitiveType CONE = new PrimitiveType("CONE", InnerEnum.CONE, FeatureType.SURFACE);
		public static readonly PrimitiveType PARABOLOID = new PrimitiveType("PARABOLOID", InnerEnum.PARABOLOID, FeatureType.SURFACE);
		public static readonly PrimitiveType QUADRATIC_SURFACE = new PrimitiveType("QUADRATIC_SURFACE", InnerEnum.QUADRATIC_SURFACE, FeatureType.SURFACE);

		private static readonly List<PrimitiveType> valueList = new List<PrimitiveType>();

		static PrimitiveType()
		{
			valueList.Add(LINE);
			valueList.Add(CIRCLE);
			valueList.Add(ELLIPSE);
			valueList.Add(QUADRATIC_CURVE);
			valueList.Add(PLANE);
			valueList.Add(SPHERE);
			valueList.Add(ELLIPSOID);
			valueList.Add(CYLINDER);
			valueList.Add(CONE);
			valueList.Add(PARABOLOID);
			valueList.Add(QUADRATIC_SURFACE);
		}

		public enum InnerEnum
		{
			LINE,
			CIRCLE,
			ELLIPSE,
			QUADRATIC_CURVE,
			PLANE,
			SPHERE,
			ELLIPSOID,
			CYLINDER,
			CONE,
			PARABOLOID,
			QUADRATIC_SURFACE
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private readonly FeatureType featureType;
		private PrimitiveType(string name, InnerEnum innerEnum, FeatureType featureType)
		{
			this.featureType = featureType;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public FeatureType FeatureType
		{
			get
			{
				return this.featureType;
			}
		}

		public static PrimitiveType[] values(FeatureType featureType)
		{
			IList<PrimitiveType> primitiveTypeList = new List<PrimitiveType>();
			foreach (PrimitiveType primitiveType in values())
			{
				if (primitiveType.FeatureType == featureType)
				{
					primitiveTypeList.Add(primitiveType);
				}
			}

			PrimitiveType[] primitiveTypeArray = new PrimitiveType[primitiveTypeList.Count];
			return primitiveTypeList.toArray(primitiveTypeArray);
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static PrimitiveType valueOf(string name)
		{
			foreach (PrimitiveType enumInstance in PrimitiveType.valueList)
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