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

namespace org.applied_geodesy.adjustment.geometry.surface.primitive
{

	using PrimitiveType = org.applied_geodesy.adjustment.geometry.PrimitiveType;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Point = org.applied_geodesy.adjustment.geometry.point.Point;
	using ProductSumRestriction = org.applied_geodesy.adjustment.geometry.restriction.ProductSumRestriction;
	using Restriction = org.applied_geodesy.adjustment.geometry.restriction.Restriction;

	using Matrix = no.uib.cipr.matrix.Matrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class Ellipsoid : Surface
	{
		private IDictionary<ParameterType, UnknownParameter> parameters;

		private ProductSumRestriction crossXYRestriction;
		private ProductSumRestriction crossXZRestriction;
		private ProductSumRestriction crossYZRestriction;

		private ProductSumRestriction xNormalRestriction;
		private ProductSumRestriction yNormalRestriction;
		private ProductSumRestriction zNormalRestriction;

		public Ellipsoid()
		{
			this.init();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setInitialGuess(double x0, double y0, double z0, double a, double b, double c, double r11, double r12, double r13, double r21, double r22, double r23, double r31, double r32, double r33) throws IllegalArgumentException
		public virtual void setInitialGuess(double x0, double y0, double z0, double a, double b, double c, double r11, double r12, double r13, double r21, double r22, double r23, double r31, double r32, double r33)
		{
			this.parameters[ParameterType.ORIGIN_COORDINATE_X].Value0 = x0;
			this.parameters[ParameterType.ORIGIN_COORDINATE_Y].Value0 = y0;
			this.parameters[ParameterType.ORIGIN_COORDINATE_Z].Value0 = z0;

			this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT].Value0 = a;
			this.parameters[ParameterType.MIDDLE_AXIS_COEFFICIENT].Value0 = b;
			this.parameters[ParameterType.MINOR_AXIS_COEFFICIENT].Value0 = c;

			this.parameters[ParameterType.ROTATION_COMPONENT_R11].Value0 = r11;
			this.parameters[ParameterType.ROTATION_COMPONENT_R12].Value0 = r12;
			this.parameters[ParameterType.ROTATION_COMPONENT_R13].Value0 = r13;

			this.parameters[ParameterType.ROTATION_COMPONENT_R21].Value0 = r21;
			this.parameters[ParameterType.ROTATION_COMPONENT_R22].Value0 = r22;
			this.parameters[ParameterType.ROTATION_COMPONENT_R23].Value0 = r23;

			this.parameters[ParameterType.ROTATION_COMPONENT_R31].Value0 = r31;
			this.parameters[ParameterType.ROTATION_COMPONENT_R32].Value0 = r32;
			this.parameters[ParameterType.ROTATION_COMPONENT_R33].Value0 = r33;
		}

		public override void jacobianElements(FeaturePoint point, Matrix Jx, Matrix Jv, int rowIndex)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;
			double zi = point.Z - centerOfMass.Z0;

			UnknownParameter X0 = this.parameters[ParameterType.ORIGIN_COORDINATE_X];
			UnknownParameter Y0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Y];
			UnknownParameter Z0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Z];

			UnknownParameter A = this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT];
			UnknownParameter B = this.parameters[ParameterType.MIDDLE_AXIS_COEFFICIENT];
			UnknownParameter C = this.parameters[ParameterType.MINOR_AXIS_COEFFICIENT];

			UnknownParameter R11 = this.parameters[ParameterType.ROTATION_COMPONENT_R11];
			UnknownParameter R12 = this.parameters[ParameterType.ROTATION_COMPONENT_R12];
			UnknownParameter R13 = this.parameters[ParameterType.ROTATION_COMPONENT_R13];

			UnknownParameter R21 = this.parameters[ParameterType.ROTATION_COMPONENT_R21];
			UnknownParameter R22 = this.parameters[ParameterType.ROTATION_COMPONENT_R22];
			UnknownParameter R23 = this.parameters[ParameterType.ROTATION_COMPONENT_R23];

			UnknownParameter R31 = this.parameters[ParameterType.ROTATION_COMPONENT_R31];
			UnknownParameter R32 = this.parameters[ParameterType.ROTATION_COMPONENT_R32];
			UnknownParameter R33 = this.parameters[ParameterType.ROTATION_COMPONENT_R33];

			double x0 = X0.Value;
			double y0 = Y0.Value;
			double z0 = Z0.Value;

			double a = A.Value;
			double b = B.Value;
			double c = C.Value;

			double r11 = R11.Value;
			double r12 = R12.Value;
			double r13 = R13.Value;

			double r21 = R21.Value;
			double r22 = R22.Value;
			double r23 = R23.Value;

			double r31 = R31.Value;
			double r32 = R32.Value;
			double r33 = R33.Value;

			double ui = r11 * (xi - x0) + r12 * (yi - y0) + r13 * (zi - z0);
			double vi = r21 * (xi - x0) + r22 * (yi - y0) + r23 * (zi - z0);
			double wi = r31 * (xi - x0) + r32 * (yi - y0) + r33 * (zi - z0);

			if (Jx != null)
			{
				if (X0.Column >= 0)
				{
					Jx.set(rowIndex, X0.Column, -2.0 * (r11 * ui * a * a + r21 * vi * b * b + r31 * wi * c * c));
				}
				if (Y0.Column >= 0)
				{
					Jx.set(rowIndex, Y0.Column, -2.0 * (r12 * ui * a * a + r22 * vi * b * b + r32 * wi * c * c));
				}
				if (Z0.Column >= 0)
				{
					Jx.set(rowIndex, Z0.Column, -2.0 * (r13 * ui * a * a + r23 * vi * b * b + r33 * wi * c * c));
				}

				if (A.Column >= 0)
				{
					Jx.set(rowIndex, A.Column, 2.0 * a * ui * ui);
				}
				if (B.Column >= 0)
				{
					Jx.set(rowIndex, B.Column, 2.0 * b * vi * vi);
				}
				if (C.Column >= 0)
				{
					Jx.set(rowIndex, C.Column, 2.0 * c * wi * wi);
				}

				if (R11.Column >= 0)
				{
					Jx.set(rowIndex, R11.Column, 2.0 * a * a * ui * (xi - x0));
				}
				if (R12.Column >= 0)
				{
					Jx.set(rowIndex, R12.Column, 2.0 * a * a * ui * (yi - y0));
				}
				if (R13.Column >= 0)
				{
					Jx.set(rowIndex, R13.Column, 2.0 * a * a * ui * (zi - z0));
				}

				if (R21.Column >= 0)
				{
					Jx.set(rowIndex, R21.Column, 2.0 * b * b * vi * (xi - x0));
				}
				if (R22.Column >= 0)
				{
					Jx.set(rowIndex, R22.Column, 2.0 * b * b * vi * (yi - y0));
				}
				if (R23.Column >= 0)
				{
					Jx.set(rowIndex, R23.Column, 2.0 * b * b * vi * (zi - z0));
				}

				if (R31.Column >= 0)
				{
					Jx.set(rowIndex, R31.Column, 2.0 * c * c * wi * (xi - x0));
				}
				if (R32.Column >= 0)
				{
					Jx.set(rowIndex, R32.Column, 2.0 * c * c * wi * (yi - y0));
				}
				if (R33.Column >= 0)
				{
					Jx.set(rowIndex, R33.Column, 2.0 * c * c * wi * (zi - z0));
				}
			}

			if (Jv != null)
			{
				Jv.set(rowIndex, 0, 2.0 * (r11 * ui * a * a + r21 * vi * b * b + r31 * wi * c * c));
				Jv.set(rowIndex, 1, 2.0 * (r12 * ui * a * a + r22 * vi * b * b + r32 * wi * c * c));
				Jv.set(rowIndex, 2, 2.0 * (r13 * ui * a * a + r23 * vi * b * b + r33 * wi * c * c));
			}
		}

		public override double getMisclosure(FeaturePoint point)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;
			double zi = point.Z - centerOfMass.Z0;

			double x0 = this.parameters[ParameterType.ORIGIN_COORDINATE_X].Value;
			double y0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Y].Value;
			double z0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Z].Value;

			double a = this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT].Value;
			double b = this.parameters[ParameterType.MIDDLE_AXIS_COEFFICIENT].Value;
			double c = this.parameters[ParameterType.MINOR_AXIS_COEFFICIENT].Value;

			double r11 = this.parameters[ParameterType.ROTATION_COMPONENT_R11].Value;
			double r12 = this.parameters[ParameterType.ROTATION_COMPONENT_R12].Value;
			double r13 = this.parameters[ParameterType.ROTATION_COMPONENT_R13].Value;

			double r21 = this.parameters[ParameterType.ROTATION_COMPONENT_R21].Value;
			double r22 = this.parameters[ParameterType.ROTATION_COMPONENT_R22].Value;
			double r23 = this.parameters[ParameterType.ROTATION_COMPONENT_R23].Value;

			double r31 = this.parameters[ParameterType.ROTATION_COMPONENT_R31].Value;
			double r32 = this.parameters[ParameterType.ROTATION_COMPONENT_R32].Value;
			double r33 = this.parameters[ParameterType.ROTATION_COMPONENT_R33].Value;

			double ui = r11 * (xi - x0) + r12 * (yi - y0) + r13 * (zi - z0);
			double vi = r21 * (xi - x0) + r22 * (yi - y0) + r23 * (zi - z0);
			double wi = r31 * (xi - x0) + r32 * (yi - y0) + r33 * (zi - z0);

			return ui * ui * a * a + vi * vi * b * b + wi * wi * c * c - 1.0;
		}

		public override Point CenterOfMass
		{
			set
			{
				// get previous center of mass
				Point prevCenterOfMass = this.CenterOfMass;
    
				// check, if components are equal to previous point to avoid unnecessary operations
				bool equalComponents = value.equalsCoordinateComponents(prevCenterOfMass);
				base.CenterOfMass = value;
				if (equalComponents)
				{
					return;
				}
    
				// get current center of mass
				Point currCenterOfMass = this.CenterOfMass;
    
				UnknownParameter x0 = this.parameters[ParameterType.ORIGIN_COORDINATE_X];
				UnknownParameter y0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Y];
				UnknownParameter z0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Z];
    
				x0.Value = x0.Value + prevCenterOfMass.X0 - currCenterOfMass.X0;
				y0.Value = y0.Value + prevCenterOfMass.Y0 - currCenterOfMass.Y0;
				z0.Value = z0.Value + prevCenterOfMass.Z0 - currCenterOfMass.Z0;
			}
		}

		public override void reverseCenterOfMass(UpperSymmPackMatrix Dp)
		{
			Point centerOfMass = this.CenterOfMass;

			UnknownParameter x0 = this.parameters[ParameterType.ORIGIN_COORDINATE_X];
			UnknownParameter y0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Y];
			UnknownParameter z0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Z];

			x0.Value = x0.Value + centerOfMass.X0;
			y0.Value = y0.Value + centerOfMass.Y0;
			z0.Value = z0.Value + centerOfMass.Z0;
		}

		public override ICollection<Restriction> Restrictions
		{
			get
			{
				return List.of(this.crossXYRestriction, this.crossXZRestriction, this.crossYZRestriction, this.xNormalRestriction, this.yNormalRestriction, this.zNormalRestriction);
			}
		}

		public override ICollection<UnknownParameter> UnknownParameters
		{
			get
			{
				return this.parameters.Values;
			}
		}

		public override UnknownParameter getUnknownParameter(ParameterType parameterType)
		{
			return this.parameters[parameterType];
		}

		public override bool contains(object @object)
		{
			if (@object == null || !(@object is UnknownParameter))
			{
				return false;
			}
			return this.parameters[((UnknownParameter)@object).ParameterType] == @object;
		}

		private void init()
		{
			this.parameters = new LinkedHashMap<ParameterType, UnknownParameter>();

			this.parameters[ParameterType.ORIGIN_COORDINATE_X] = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_X, true);
			this.parameters[ParameterType.ORIGIN_COORDINATE_Y] = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Y, true);
			this.parameters[ParameterType.ORIGIN_COORDINATE_Z] = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Z, true);

			this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT] = new UnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT, true);
			this.parameters[ParameterType.MIDDLE_AXIS_COEFFICIENT] = new UnknownParameter(ParameterType.MIDDLE_AXIS_COEFFICIENT, true);
			this.parameters[ParameterType.MINOR_AXIS_COEFFICIENT] = new UnknownParameter(ParameterType.MINOR_AXIS_COEFFICIENT, true);

			this.parameters[ParameterType.ROTATION_COMPONENT_R11] = new UnknownParameter(ParameterType.ROTATION_COMPONENT_R11, true);
			this.parameters[ParameterType.ROTATION_COMPONENT_R12] = new UnknownParameter(ParameterType.ROTATION_COMPONENT_R12, true);
			this.parameters[ParameterType.ROTATION_COMPONENT_R13] = new UnknownParameter(ParameterType.ROTATION_COMPONENT_R13, true);

			this.parameters[ParameterType.ROTATION_COMPONENT_R21] = new UnknownParameter(ParameterType.ROTATION_COMPONENT_R21, true);
			this.parameters[ParameterType.ROTATION_COMPONENT_R22] = new UnknownParameter(ParameterType.ROTATION_COMPONENT_R22, true);
			this.parameters[ParameterType.ROTATION_COMPONENT_R23] = new UnknownParameter(ParameterType.ROTATION_COMPONENT_R23, true);

			this.parameters[ParameterType.ROTATION_COMPONENT_R31] = new UnknownParameter(ParameterType.ROTATION_COMPONENT_R31, true);
			this.parameters[ParameterType.ROTATION_COMPONENT_R32] = new UnknownParameter(ParameterType.ROTATION_COMPONENT_R32, true);
			this.parameters[ParameterType.ROTATION_COMPONENT_R33] = new UnknownParameter(ParameterType.ROTATION_COMPONENT_R33, true);

			this.parameters[ParameterType.VECTOR_LENGTH] = new UnknownParameter(ParameterType.VECTOR_LENGTH, true, 1.0, false, ProcessingType.FIXED);
			this.parameters[ParameterType.CONSTANT] = new UnknownParameter(ParameterType.CONSTANT, true, 0.0, false, ProcessingType.FIXED);

			UnknownParameter R11 = this.parameters[ParameterType.ROTATION_COMPONENT_R11];
			UnknownParameter R12 = this.parameters[ParameterType.ROTATION_COMPONENT_R12];
			UnknownParameter R13 = this.parameters[ParameterType.ROTATION_COMPONENT_R13];

			UnknownParameter R21 = this.parameters[ParameterType.ROTATION_COMPONENT_R21];
			UnknownParameter R22 = this.parameters[ParameterType.ROTATION_COMPONENT_R22];
			UnknownParameter R23 = this.parameters[ParameterType.ROTATION_COMPONENT_R23];

			UnknownParameter R31 = this.parameters[ParameterType.ROTATION_COMPONENT_R31];
			UnknownParameter R32 = this.parameters[ParameterType.ROTATION_COMPONENT_R32];
			UnknownParameter R33 = this.parameters[ParameterType.ROTATION_COMPONENT_R33];

			UnknownParameter zero = this.parameters[ParameterType.CONSTANT];
			UnknownParameter one = this.parameters[ParameterType.VECTOR_LENGTH];

			IList<UnknownParameter> xNormalVector = new List<UnknownParameter> {R11, R12, R13};
			IList<UnknownParameter> yNormalVector = new List<UnknownParameter> {R21, R22, R23};
			IList<UnknownParameter> zNormalVector = new List<UnknownParameter> {R31, R32, R33};

			this.xNormalRestriction = new ProductSumRestriction(true, xNormalVector, xNormalVector, one);
			this.yNormalRestriction = new ProductSumRestriction(true, yNormalVector, yNormalVector, one);
			this.zNormalRestriction = new ProductSumRestriction(true, zNormalVector, zNormalVector, one);

			this.crossXYRestriction = new ProductSumRestriction(true, xNormalVector, yNormalVector, zero);
			this.crossXZRestriction = new ProductSumRestriction(true, xNormalVector, zNormalVector, zero);
			this.crossYZRestriction = new ProductSumRestriction(true, yNormalVector, zNormalVector, zero);
		}

		public override PrimitiveType PrimitiveType
		{
			get
			{
				return PrimitiveType.ELLIPSOID;
			}
		}

		public override string toLaTex()
		{
			return "$a^2 u_i^2 + b^2 v_i^2 + c^2 w_i^2 = 1" + " \\\\ " + "\\left(\\begin{array}{c} u_i \\\\ v_i \\\\ w_i \\end{array}\\right) = \\mathbf{R} \\left(\\begin{array}{c} x_i-x_0 \\\\ y_i-y_0 \\\\ z_i-z_0 \\end{array}\\right);" + " \\\\ " + "\\mathbf{R} = \\left(\\begin{array}{ccc} r_{11} & r_{12} & r_{13} \\\\ r_{21} & r_{22} & r_{23} \\\\ r_{31} & r_{32} & r_{33} \\end{array}\\right)$";
		}
	}

}