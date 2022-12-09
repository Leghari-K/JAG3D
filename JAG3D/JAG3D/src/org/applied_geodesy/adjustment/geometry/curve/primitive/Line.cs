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

	public class Line : Curve
	{

		private IDictionary<ParameterType, UnknownParameter> parameters = null;
		private ProductSumRestriction vectorLengthRestriction;

		public Line()
		{
			this.init();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setInitialGuess(double nx, double ny, double d, double l) throws IllegalArgumentException
		public virtual void setInitialGuess(double nx, double ny, double d, double l)
		{
			this.parameters[ParameterType.VECTOR_X].Value0 = nx;
			this.parameters[ParameterType.VECTOR_Y].Value0 = ny;
			this.parameters[ParameterType.LENGTH].Value0 = d;
			this.parameters[ParameterType.VECTOR_LENGTH].Value0 = l;
		}

		public override void jacobianElements(FeaturePoint point, Matrix Jx, Matrix Jv, int rowIndex)
		{
			Point centerOfMass = this.CenterOfMass;

			//Schwerpunktreduktion
			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;

			// Parameter der Geraden
			UnknownParameter nx = this.parameters[ParameterType.VECTOR_X];
			UnknownParameter ny = this.parameters[ParameterType.VECTOR_Y];

			if (Jx != null)
			{
				UnknownParameter d = this.parameters[ParameterType.LENGTH];
				if (nx.Column >= 0)
				{
					Jx.set(rowIndex, nx.Column, xi);
				}
				if (ny.Column >= 0)
				{
					Jx.set(rowIndex, ny.Column, yi);
				}
				if (d.Column >= 0)
				{
					Jx.set(rowIndex, d.Column, -1.0);
				}
			}

			if (Jv != null)
			{
				Jv.set(rowIndex, 0, nx.Value);
				Jv.set(rowIndex, 1, ny.Value);
			}
		}

		public override double getMisclosure(FeaturePoint point)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;

			double nx = this.parameters[ParameterType.VECTOR_X].Value;
			double ny = this.parameters[ParameterType.VECTOR_Y].Value;
			double d = this.parameters[ParameterType.LENGTH].Value;

			return nx * xi + ny * yi - d;
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
    
				double nx = this.parameters[ParameterType.VECTOR_X].Value;
				double ny = this.parameters[ParameterType.VECTOR_Y].Value;
				UnknownParameter d = this.parameters[ParameterType.LENGTH];
    
				double dist = d.Value;
				dist += (nx * prevCenterOfMass.X0 + ny * prevCenterOfMass.Y0);
				dist -= (nx * currCenterOfMass.X0 + ny * currCenterOfMass.Y0);
				d.Value = dist;
			}
		}

		public override void reverseCenterOfMass(UpperSymmPackMatrix Dp)
		{
			Point centerOfMass = this.CenterOfMass;

			UnknownParameter nx = this.parameters[ParameterType.VECTOR_X];
			UnknownParameter ny = this.parameters[ParameterType.VECTOR_Y];
			UnknownParameter d = this.parameters[ParameterType.LENGTH];

			if (Dp != null)
			{
				int nou = Dp.numColumns();
				Matrix J = Matrices.identity(nou);

				if (d.Column >= 0)
				{
					if (nx.Column >= 0)
					{
						J.set(d.Column, nx.Column, centerOfMass.X0);
					}
					if (ny.Column >= 0)
					{
						J.set(d.Column, ny.Column, centerOfMass.Y0);
					}
					J.set(d.Column, d.Column, 1.0);

					Matrix JDp = new DenseMatrix(nou, nou);
					J.mult(Dp, JDp);
					JDp.transBmult(J, Dp);
				}
			}

			double dist = d.Value + nx.Value * centerOfMass.X0 + ny.Value * centerOfMass.Y0;
			d.Value = dist;
		}

		public override ICollection<Restriction> Restrictions
		{
			get
			{
				return List.of(this.vectorLengthRestriction);
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
			this.parameters[ParameterType.VECTOR_X] = new UnknownParameter(ParameterType.VECTOR_X, true, 0);
			this.parameters[ParameterType.VECTOR_Y] = new UnknownParameter(ParameterType.VECTOR_Y, true, 1);
			this.parameters[ParameterType.LENGTH] = new UnknownParameter(ParameterType.LENGTH, true);
			this.parameters[ParameterType.VECTOR_LENGTH] = new UnknownParameter(ParameterType.VECTOR_LENGTH, true, 1.0, true, ProcessingType.FIXED);

			if (this.vectorLengthRestriction == null)
			{
				UnknownParameter normalX = this.parameters[ParameterType.VECTOR_X];
				UnknownParameter normalY = this.parameters[ParameterType.VECTOR_Y];
				UnknownParameter vectorLength = this.parameters[ParameterType.VECTOR_LENGTH];

				IList<UnknownParameter> normalVector = new List<UnknownParameter> {normalX, normalY};
				this.vectorLengthRestriction = new ProductSumRestriction(true, normalVector, normalVector, vectorLength);
			}
		}

		public override PrimitiveType PrimitiveType
		{
			get
			{
				return PrimitiveType.LINE;
			}
		}

		public override string toLaTex()
		{
			return "$\\mathbf{n^\\mathrm{T}} \\mathbf{P}_i = d" + " \\\\ " + "\\mathbf{n} = \\left( \\begin{array}{c} n_x \\\\ n_y \\end{array} \\right)$";
		}
	}

}