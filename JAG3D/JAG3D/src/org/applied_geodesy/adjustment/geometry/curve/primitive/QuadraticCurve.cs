using System;
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

namespace org.applied_geodesy.adjustment.geometry.curve.primitive
{

	using PrimitiveType = org.applied_geodesy.adjustment.geometry.PrimitiveType;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Point = org.applied_geodesy.adjustment.geometry.point.Point;
	using ProductSumRestriction = org.applied_geodesy.adjustment.geometry.restriction.ProductSumRestriction;
	using Restriction = org.applied_geodesy.adjustment.geometry.restriction.Restriction;

	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using Matrices = no.uib.cipr.matrix.Matrices;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class QuadraticCurve : Curve
	{
		private IDictionary<ParameterType, UnknownParameter> parameters;
		private static readonly double SQRT2 = Math.Sqrt(2.0);

		private ProductSumRestriction normalizeRestriction;

		public QuadraticCurve()
		{
			this.init();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setInitialGuess(double a, double b, double c, double d, double e, double length) throws IllegalArgumentException
		public virtual void setInitialGuess(double a, double b, double c, double d, double e, double length)
		{
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A].Value0 = a;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B].Value0 = b;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C].Value0 = c;

			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D].Value0 = d;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E].Value0 = e;

			this.parameters[ParameterType.LENGTH].Value0 = length;
		}

		public override void jacobianElements(FeaturePoint point, Matrix Jx, Matrix Jv, int rowIndex)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;

			UnknownParameter a = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A];
			UnknownParameter b = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B];
			UnknownParameter c = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C];

			UnknownParameter d = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D];
			UnknownParameter e = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E];

			UnknownParameter length = this.parameters[ParameterType.LENGTH];

			if (Jx != null)
			{
				if (a.Column >= 0)
				{
					Jx.set(rowIndex, a.Column, xi * xi);
				}
				if (b.Column >= 0)
				{
					Jx.set(rowIndex, b.Column, yi * yi);
				}
				if (c.Column >= 0)
				{
					Jx.set(rowIndex, c.Column, SQRT2 * xi * yi);
				}

				if (d.Column >= 0)
				{
					Jx.set(rowIndex, d.Column, xi);
				}
				if (e.Column >= 0)
				{
					Jx.set(rowIndex, e.Column, yi);
				}

				if (length.Column >= 0)
				{
					Jx.set(rowIndex, length.Column, 1.0);
				}
			}

			if (Jv != null)
			{
				Jv.set(rowIndex, 0, 2.0 * a.Value * xi + c.Value * SQRT2 * yi + d.Value);
				Jv.set(rowIndex, 1, 2.0 * b.Value * yi + c.Value * SQRT2 * xi + e.Value);
			}
		}

		public override double getMisclosure(FeaturePoint point)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;

			double a = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A].Value;
			double b = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B].Value;
			double c = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C].Value;

			double d = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D].Value;
			double e = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E].Value;

			double length = this.parameters[ParameterType.LENGTH].Value;

			return a * xi * xi + b * yi * yi + c * SQRT2 * xi * yi + d * xi + e * yi + length;
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
    
				UnknownParameter D = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D];
				UnknownParameter E = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E];
    
				UnknownParameter Length = this.parameters[ParameterType.LENGTH];
    
				double a = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A].Value;
				double b = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B].Value;
				double c = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C].Value;
    
				// remove prev center of mass
				double x0 = prevCenterOfMass.X0;
				double y0 = prevCenterOfMass.Y0;
    
				double d = D.Value - 2.0 * (a * x0 + c / SQRT2 * y0);
				double e = E.Value - 2.0 * (c / SQRT2 * x0 + b * y0);
				double length = Length.Value - (a * x0 * x0 + b * y0 * y0 + c * SQRT2 * x0 * y0 + d * x0 + e * y0);
    
				// set new center of mass
				x0 = currCenterOfMass.X0;
				y0 = currCenterOfMass.Y0;
    
				length = length + (a * x0 * x0 + b * y0 * y0 + c * SQRT2 * x0 * y0 + d * x0 + e * y0);
				d = d + 2.0 * (a * x0 + c / SQRT2 * y0);
				e = e + 2.0 * (c / SQRT2 * x0 + b * y0);
    
				D.Value = d;
				E.Value = e;
				Length.Value = length;
			}
		}

		public override void reverseCenterOfMass(UpperSymmPackMatrix Dp)
		{
			Point centerOfMass = this.CenterOfMass;

			UnknownParameter a = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A];
			UnknownParameter b = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B];
			UnknownParameter c = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C];

			UnknownParameter d = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D];
			UnknownParameter e = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E];

			UnknownParameter length = this.parameters[ParameterType.LENGTH];

			double x0 = centerOfMass.X0;
			double y0 = centerOfMass.Y0;

			if (Dp != null)
			{
				int nou = Dp.numColumns();
				Matrix J = Matrices.identity(nou);
				if (d.Column >= 0)
				{
					if (a.Column >= 0)
					{
						J.set(d.Column, a.Column, -2.0 * x0);
					}
					if (c.Column >= 0)
					{
						J.set(d.Column, c.Column, -2.0 / SQRT2 * y0);
					}
					J.set(d.Column, d.Column, 1.0);
				}
				if (e.Column >= 0)
				{
					if (c.Column >= 0)
					{
						J.set(e.Column, c.Column, -2.0 / SQRT2 * x0);
					}
					if (b.Column >= 0)
					{
						J.set(e.Column, b.Column, -2.0 * y0);
					}
					J.set(e.Column, e.Column, 1.0);
				}
				if (length.Column >= 0)
				{
					if (a.Column >= 0)
					{
						J.set(length.Column, a.Column, x0 * x0);
					}
					if (b.Column >= 0)
					{
						J.set(length.Column, b.Column, y0 * y0);
					}
					if (c.Column >= 0)
					{
						J.set(length.Column, c.Column, SQRT2 * x0 * y0);
					}
					if (d.Column >= 0)
					{
						J.set(length.Column, d.Column, -x0);
					}
					if (e.Column >= 0)
					{
						J.set(length.Column, e.Column, -y0);
					}
					J.set(length.Column, length.Column, 1.0);
				}

				Matrix JDp = new DenseMatrix(nou, nou);
				J.mult(Dp, JDp);
				JDp.transBmult(J, Dp);
			}

			length.Value = length.Value + a.Value * x0 * x0 + b.Value * y0 * y0 + c.Value * SQRT2 * x0 * y0 - d.Value * x0 - e.Value * y0;
			d.Value = d.Value - 2.0 * (a.Value * x0 + c.Value / SQRT2 * y0);
			e.Value = e.Value - 2.0 * (c.Value / SQRT2 * x0 + b.Value * y0);
		}

		public override ICollection<Restriction> Restrictions
		{
			get
			{
				return List.of(this.normalizeRestriction);
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

			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A] = new UnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_A, true);
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B] = new UnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_B, true);
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C] = new UnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_C, true);

			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D] = new UnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_D, true);
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E] = new UnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_E, true);

			this.parameters[ParameterType.LENGTH] = new UnknownParameter(ParameterType.LENGTH, true);

			this.parameters[ParameterType.VECTOR_LENGTH] = new UnknownParameter(ParameterType.VECTOR_LENGTH, true, 1.0, true, ProcessingType.FIXED);

			UnknownParameter A = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A];
			UnknownParameter B = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B];
			UnknownParameter C = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C];

			UnknownParameter one = this.parameters[ParameterType.VECTOR_LENGTH];

			IList<UnknownParameter> quadraticCoeficients = new List<UnknownParameter> {A, B, C};

			this.normalizeRestriction = new ProductSumRestriction(true, quadraticCoeficients, quadraticCoeficients, one);
		}

		public override PrimitiveType PrimitiveType
		{
			get
			{
				return PrimitiveType.QUADRATIC_CURVE;
			}
		}

		public override string toLaTex()
		{
			return "$\\mathbf{P}^\\mathrm{T}_i \\mathbf{U} \\mathbf{P}_i + \\mathbf{P}^\\mathrm{T}_i \\mathbf{u} + u_0 = 0" + " \\\\ " + "\\mathbf{U} = \\left( \\begin{array}{cc} a_1 & \\frac{a_3}{\\sqrt{2}} \\\\ \\frac{a_3}{\\sqrt{2}} & a_2 \\end{array} \\right);" + " \\\\ " + "\\mathbf{u} = \\left( \\begin{array}{c} a_4 \\\\ a_5 \\end{array} \\right);\\,u_0 = d$";
		}
	}
}