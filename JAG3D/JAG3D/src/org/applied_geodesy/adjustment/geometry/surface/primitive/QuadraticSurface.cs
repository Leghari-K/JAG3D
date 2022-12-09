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

	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using Matrices = no.uib.cipr.matrix.Matrices;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class QuadraticSurface : Surface
	{
		private IDictionary<ParameterType, UnknownParameter> parameters;
		private static readonly double SQRT2 = Math.Sqrt(2.0);

		private ProductSumRestriction normalizeRestriction;

		public QuadraticSurface()
		{
			this.init();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setInitialGuess(double a, double b, double c, double d, double e, double f, double g, double h, double i, double length) throws IllegalArgumentException
		public virtual void setInitialGuess(double a, double b, double c, double d, double e, double f, double g, double h, double i, double length)
		{
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A].Value0 = a;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B].Value0 = b;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C].Value0 = c;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D].Value0 = d;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E].Value0 = e;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_F].Value0 = f;

			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_G].Value0 = g;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_H].Value0 = h;
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_I].Value0 = i;

			this.parameters[ParameterType.LENGTH].Value0 = length;
		}

		public override void jacobianElements(FeaturePoint point, Matrix Jx, Matrix Jv, int rowIndex)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;
			double zi = point.Z - centerOfMass.Z0;

			UnknownParameter a = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A];
			UnknownParameter b = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B];
			UnknownParameter c = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C];
			UnknownParameter d = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D];
			UnknownParameter e = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E];
			UnknownParameter f = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_F];

			UnknownParameter g = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_G];
			UnknownParameter h = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_H];
			UnknownParameter i = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_I];

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
					Jx.set(rowIndex, c.Column, zi * zi);
				}
				if (d.Column >= 0)
				{
					Jx.set(rowIndex, d.Column, SQRT2 * xi * yi);
				}
				if (e.Column >= 0)
				{
					Jx.set(rowIndex, e.Column, SQRT2 * xi * zi);
				}
				if (f.Column >= 0)
				{
					Jx.set(rowIndex, f.Column, SQRT2 * yi * zi);
				}

				if (g.Column >= 0)
				{
					Jx.set(rowIndex, g.Column, xi);
				}
				if (h.Column >= 0)
				{
					Jx.set(rowIndex, h.Column, yi);
				}
				if (i.Column >= 0)
				{
					Jx.set(rowIndex, i.Column, zi);
				}

				if (length.Column >= 0)
				{
					Jx.set(rowIndex, length.Column, 1.0);
				}
			}

			if (Jv != null)
			{
				Jv.set(rowIndex, 0, 2.0 * a.Value * xi + d.Value * SQRT2 * yi + e.Value * SQRT2 * zi + g.Value);
				Jv.set(rowIndex, 1, 2.0 * b.Value * yi + d.Value * SQRT2 * xi + f.Value * SQRT2 * zi + h.Value);
				Jv.set(rowIndex, 2, 2.0 * c.Value * zi + e.Value * SQRT2 * xi + f.Value * SQRT2 * yi + i.Value);
			}
		}

		public override double getMisclosure(FeaturePoint point)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;
			double zi = point.Z - centerOfMass.Z0;

			double a = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A].Value;
			double b = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B].Value;
			double c = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C].Value;
			double d = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D].Value;
			double e = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E].Value;
			double f = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_F].Value;

			double g = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_G].Value;
			double h = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_H].Value;
			double i = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_I].Value;

			double length = this.parameters[ParameterType.LENGTH].Value;

			return a * xi * xi + b * yi * yi + c * zi * zi + d * SQRT2 * xi * yi + e * SQRT2 * xi * zi + f * SQRT2 * yi * zi + g * xi + h * yi + i * zi + length;
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
    
				UnknownParameter G = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_G];
				UnknownParameter H = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_H];
				UnknownParameter I = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_I];
    
				UnknownParameter Length = this.parameters[ParameterType.LENGTH];
    
				double a = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A].Value;
				double b = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B].Value;
				double c = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C].Value;
				double d = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D].Value;
				double e = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E].Value;
				double f = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_F].Value;
    
				// remove prev center of mass
				double x0 = prevCenterOfMass.X0;
				double y0 = prevCenterOfMass.Y0;
				double z0 = prevCenterOfMass.Z0;
    
				double g = G.Value - 2.0 * (a * x0 + d / SQRT2 * y0 + e / SQRT2 * z0);
				double h = H.Value - 2.0 * (d / SQRT2 * x0 + b * y0 + f / SQRT2 * z0);
				double i = I.Value - 2.0 * (e / SQRT2 * x0 + f / SQRT2 * y0 + c * z0);
				double length = Length.Value - (a * x0 * x0 + b * y0 * y0 + c * z0 * z0 + d * SQRT2 * x0 * y0 + e * SQRT2 * x0 * z0 + f * SQRT2 * y0 * z0 + g * x0 + h * y0 + i * z0);
    
				// set new center of mass
				x0 = currCenterOfMass.X0;
				y0 = currCenterOfMass.Y0;
				z0 = currCenterOfMass.Z0;
    
				length = length + (a * x0 * x0 + b * y0 * y0 + c * z0 * z0 + d * SQRT2 * x0 * y0 + e * SQRT2 * x0 * z0 + f * SQRT2 * y0 * z0 + g * x0 + h * y0 + i * z0);
				g = g + 2.0 * (a * x0 + d / SQRT2 * y0 + e / SQRT2 * z0);
				h = h + 2.0 * (d / SQRT2 * x0 + b * y0 + f / SQRT2 * z0);
				i = i + 2.0 * (e / SQRT2 * x0 + f / SQRT2 * y0 + c * z0);
    
				G.Value = g;
				H.Value = h;
				I.Value = i;
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
			UnknownParameter f = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_F];

			UnknownParameter g = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_G];
			UnknownParameter h = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_H];
			UnknownParameter i = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_I];

			UnknownParameter length = this.parameters[ParameterType.LENGTH];

			double x0 = centerOfMass.X0;
			double y0 = centerOfMass.Y0;
			double z0 = centerOfMass.Z0;

			if (Dp != null)
			{
				int nou = Dp.numColumns();
				Matrix J = Matrices.identity(nou);

				if (g.Column >= 0)
				{
					if (a.Column >= 0)
					{
						J.set(g.Column, a.Column, -2.0 * x0);
					}
					if (d.Column >= 0)
					{
						J.set(g.Column, d.Column, -2.0 / SQRT2 * y0);
					}
					if (e.Column >= 0)
					{
						J.set(g.Column, e.Column, -2.0 / SQRT2 * z0);
					}
					J.set(g.Column, g.Column, 1.0);
				}
				if (h.Column >= 0)
				{
					if (d.Column >= 0)
					{
						J.set(h.Column, d.Column, -2.0 / SQRT2 * x0);
					}
					if (b.Column >= 0)
					{
						J.set(h.Column, b.Column, -2.0 * y0);
					}
					if (f.Column >= 0)
					{
						J.set(h.Column, f.Column, -2.0 / SQRT2 * z0);
					}
					J.set(h.Column, h.Column, 1.0);
				}
				if (i.Column >= 0)
				{
					if (d.Column >= 0)
					{
						J.set(i.Column, e.Column, -2.0 / SQRT2 * x0);
					}
					if (b.Column >= 0)
					{
						J.set(i.Column, f.Column, -2.0 / SQRT2 * y0);
					}
					if (f.Column >= 0)
					{
						J.set(i.Column, c.Column, -2.0 * z0);
					}
					J.set(i.Column, i.Column, 1.0);
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
						J.set(length.Column, c.Column, z0 * z0);
					}
					if (d.Column >= 0)
					{
						J.set(length.Column, d.Column, SQRT2 * x0 * y0);
					}
					if (e.Column >= 0)
					{
						J.set(length.Column, e.Column, SQRT2 * x0 * z0);
					}
					if (f.Column >= 0)
					{
						J.set(length.Column, f.Column, SQRT2 * y0 * z0);
					}
					if (g.Column >= 0)
					{
						J.set(length.Column, g.Column, -x0);
					}
					if (h.Column >= 0)
					{
						J.set(length.Column, h.Column, -y0);
					}
					if (i.Column >= 0)
					{
						J.set(length.Column, i.Column, -z0);
					}
					J.set(length.Column, length.Column, 1.0);
				}

				Matrix JDp = new DenseMatrix(nou, nou);
				J.mult(Dp, JDp);
				JDp.transBmult(J, Dp);
			}

			length.Value = length.Value + (a.Value * x0 * x0 + b.Value * y0 * y0 + c.Value * z0 * z0 + d.Value * SQRT2 * x0 * y0 + e.Value * SQRT2 * x0 * z0 + f.Value * SQRT2 * y0 * z0 - g.Value * x0 - h.Value * y0 - i.Value * z0);
			g.Value = g.Value - 2.0 * (a.Value * x0 + d.Value / SQRT2 * y0 + e.Value / SQRT2 * z0);
			h.Value = h.Value - 2.0 * (d.Value / SQRT2 * x0 + b.Value * y0 + f.Value / SQRT2 * z0);
			i.Value = i.Value - 2.0 * (e.Value / SQRT2 * x0 + f.Value / SQRT2 * y0 + c.Value * z0);
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
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_F] = new UnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_F, true);

			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_G] = new UnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_G, true);
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_H] = new UnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_H, true);
			this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_I] = new UnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_I, true);

			this.parameters[ParameterType.LENGTH] = new UnknownParameter(ParameterType.LENGTH, true);

			this.parameters[ParameterType.VECTOR_LENGTH] = new UnknownParameter(ParameterType.VECTOR_LENGTH, true, 1.0, true, ProcessingType.FIXED);

			UnknownParameter A = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_A];
			UnknownParameter B = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_B];
			UnknownParameter C = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_C];

			UnknownParameter D = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_D];
			UnknownParameter E = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_E];
			UnknownParameter F = this.parameters[ParameterType.POLYNOMIAL_COEFFICIENT_F];

			UnknownParameter one = this.parameters[ParameterType.VECTOR_LENGTH];

			IList<UnknownParameter> quadraticCoeficients = new List<UnknownParameter> {A, B, C, D, E, F};

			this.normalizeRestriction = new ProductSumRestriction(true, quadraticCoeficients, quadraticCoeficients, one);
		}

		public override PrimitiveType PrimitiveType
		{
			get
			{
				return PrimitiveType.QUADRATIC_SURFACE;
			}
		}

		public override string toLaTex()
		{
			return "$\\mathbf{P}^\\mathrm{T}_i \\mathbf{U} \\mathbf{P}_i + \\mathbf{P}^\\mathrm{T}_i \\mathbf{u} + u_0 = 0" + " \\\\ " + "\\mathbf{U} = \\left( \\begin{array}{ccc} a_1 & \\frac{a_4}{\\sqrt{2}} & \\frac{a_5}{\\sqrt{2}} \\\\ \\frac{a_4}{\\sqrt{2}} & a_2 & \\frac{a_6}{\\sqrt{2}} \\\\  \\frac{a_5}{\\sqrt{2}} & \\frac{a_6}{\\sqrt{2}} & a_3 \\end{array} \\right);" + " \\\\ " + "\\mathbf{u} = \\left( \\begin{array}{cc} a_7 \\\\ a_8 \\\\ a_9 \\end{array} \\right);\\,u_0 = d$";
		}
	}
}