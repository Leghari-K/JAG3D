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

	public class Cylinder : Surface
	{
		private IDictionary<ParameterType, UnknownParameter> parameters;

		private ProductSumRestriction vectorLengthRestriction;
		private ProductSumRestriction primaryFocalRestriction;
		private ProductSumRestriction secondaryFocalRestriction;

		public Cylinder()
		{
			this.init();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setInitialGuess(double x1, double y1, double z1, double x2, double y2, double z2, double nx, double ny, double nz, double a) throws IllegalArgumentException
		public virtual void setInitialGuess(double x1, double y1, double z1, double x2, double y2, double z2, double nx, double ny, double nz, double a)
		{
			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X].Value0 = x1;
			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y].Value0 = y1;
			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Z].Value0 = z1;

			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X].Value0 = x2;
			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y].Value0 = y2;
			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Z].Value0 = z2;

			this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT].Value0 = a;

			this.parameters[ParameterType.VECTOR_X].Value0 = nx;
			this.parameters[ParameterType.VECTOR_Y].Value0 = ny;
			this.parameters[ParameterType.VECTOR_Z].Value0 = nz;
		}

		public override void jacobianElements(FeaturePoint point, Matrix Jx, Matrix Jv, int rowIndex)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;
			double zi = point.Z - centerOfMass.Z0;

			UnknownParameter x1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X];
			UnknownParameter y1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y];
			UnknownParameter z1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Z];

			UnknownParameter x2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X];
			UnknownParameter y2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y];
			UnknownParameter z2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Z];

			UnknownParameter a = this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT];

			UnknownParameter nx = this.parameters[ParameterType.VECTOR_X];
			UnknownParameter ny = this.parameters[ParameterType.VECTOR_Y];
			UnknownParameter nz = this.parameters[ParameterType.VECTOR_Z];

			double c11 = nz.Value * (yi - y1.Value) - ny.Value * (zi - z1.Value);
			double c12 = nx.Value * (zi - z1.Value) - nz.Value * (xi - x1.Value);
			double c13 = ny.Value * (xi - x1.Value) - nx.Value * (yi - y1.Value);

			double c21 = nz.Value * (yi - y2.Value) - ny.Value * (zi - z2.Value);
			double c22 = nx.Value * (zi - z2.Value) - nz.Value * (xi - x2.Value);
			double c23 = ny.Value * (xi - x2.Value) - nx.Value * (yi - y2.Value);

			double PcrossF1 = Math.Sqrt(c11 * c11 + c12 * c12 + c13 * c13);
			double PcrossF2 = Math.Sqrt(c21 * c21 + c22 * c22 + c23 * c23);

			if (Jx != null)
			{
				if (x1.Column >= 0)
				{
					Jx.set(rowIndex, x1.Column, (nz.Value * c12 - ny.Value * c13) / PcrossF1);
				}
				if (y1.Column >= 0)
				{
					Jx.set(rowIndex, y1.Column, (nx.Value * c13 - nz.Value * c11) / PcrossF1);
				}
				if (z1.Column >= 0)
				{
					Jx.set(rowIndex, z1.Column, (ny.Value * c11 - nx.Value * c12) / PcrossF1);
				}

				if (x2.Column >= 0)
				{
					Jx.set(rowIndex, x2.Column, (nz.Value * c22 - ny.Value * c23) / PcrossF2);
				}
				if (y2.Column >= 0)
				{
					Jx.set(rowIndex, y2.Column, (nx.Value * c23 - nz.Value * c21) / PcrossF2);
				}
				if (z2.Column >= 0)
				{
					Jx.set(rowIndex, z2.Column, (ny.Value * c21 - nx.Value * c22) / PcrossF2);
				}

				if (a.Column >= 0)
				{
					Jx.set(rowIndex, a.Column, -2.0);
				}

				if (nx.Column >= 0)
				{
					Jx.set(rowIndex, nx.Column, ((zi - z2.Value) * c22 + (y2.Value - yi) * c23) / PcrossF2 + ((zi - z1.Value) * c12 + (y1.Value - yi) * c13) / PcrossF1);
				}
				if (ny.Column >= 0)
				{
					Jx.set(rowIndex, ny.Column, ((z2.Value - zi) * c21 + (xi - x2.Value) * c23) / PcrossF2 + ((z1.Value - zi) * c11 + (xi - x1.Value) * c13) / PcrossF1);
				}
				if (nz.Column >= 0)
				{
					Jx.set(rowIndex, nz.Column, ((yi - y2.Value) * c21 + (x2.Value - xi) * c22) / PcrossF2 + ((yi - y1.Value) * c11 + (x1.Value - xi) * c12) / PcrossF1);
				}
			}

			if (Jv != null)
			{
				Jv.set(rowIndex, 0, (ny.Value * c23 - nz.Value * c22) / PcrossF2 + (ny.Value * c13 - nz.Value * c12) / PcrossF1);
				Jv.set(rowIndex, 1, (nz.Value * c21 - nx.Value * c23) / PcrossF2 + (nz.Value * c11 - nx.Value * c13) / PcrossF1);
				Jv.set(rowIndex, 2, (nx.Value * c22 - ny.Value * c21) / PcrossF2 + (nx.Value * c12 - ny.Value * c11) / PcrossF1);
			}
		}

		public override double getMisclosure(FeaturePoint point)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;
			double zi = point.Z - centerOfMass.Z0;

			double x1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X].Value;
			double y1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y].Value;
			double z1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Z].Value;

			double x2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X].Value;
			double y2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y].Value;
			double z2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Z].Value;

			double a = this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT].Value;

			double nx = this.parameters[ParameterType.VECTOR_X].Value;
			double ny = this.parameters[ParameterType.VECTOR_Y].Value;
			double nz = this.parameters[ParameterType.VECTOR_Z].Value;


			double c11 = nz * (yi - y1) - ny * (zi - z1);
			double c12 = nx * (zi - z1) - nz * (xi - x1);
			double c13 = ny * (xi - x1) - nx * (yi - y1);

			double c21 = nz * (yi - y2) - ny * (zi - z2);
			double c22 = nx * (zi - z2) - nz * (xi - x2);
			double c23 = ny * (xi - x2) - nx * (yi - y2);

			double PcrossF1 = Math.Sqrt(c11 * c11 + c12 * c12 + c13 * c13);
			double PcrossF2 = Math.Sqrt(c21 * c21 + c22 * c22 + c23 * c23);

			return PcrossF1 + PcrossF2 - 2.0 * a;
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
    
				// Bestimme Aufpunkt
				UnknownParameter x1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X];
				UnknownParameter y1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y];
				UnknownParameter z1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Z];
    
				UnknownParameter x2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X];
				UnknownParameter y2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y];
				UnknownParameter z2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Z];
    
				// Shift
				double xf1 = x1.Value + prevCenterOfMass.X0 - currCenterOfMass.X0;
				double yf1 = y1.Value + prevCenterOfMass.Y0 - currCenterOfMass.Y0;
				double zf1 = z1.Value + prevCenterOfMass.Z0 - currCenterOfMass.Z0;
    
				double xf2 = x2.Value + prevCenterOfMass.X0 - currCenterOfMass.X0;
				double yf2 = y2.Value + prevCenterOfMass.Y0 - currCenterOfMass.Y0;
				double zf2 = z2.Value + prevCenterOfMass.Z0 - currCenterOfMass.Z0;
    
				// bestimme Punkt mit kuerzesten Abstand zum Ursprung
				double nx = this.parameters[ParameterType.VECTOR_X].Value;
				double ny = this.parameters[ParameterType.VECTOR_Y].Value;
				double nz = this.parameters[ParameterType.VECTOR_Z].Value;
    
				double dotNormalNormal = nx * nx + ny * ny + nz * nz;
    
				if (dotNormalNormal != 0)
				{
					double dotNormalF1 = nx * xf1 + ny * yf1 + nz * zf1;
					double dotNormalF2 = nx * xf2 + ny * yf2 + nz * zf2;
    
					double s1 = -dotNormalF1 / dotNormalNormal;
					double s2 = -dotNormalF2 / dotNormalNormal;
    
					xf1 = xf1 + s1 * nx;
					yf1 = yf1 + s1 * ny;
					zf1 = zf1 + s1 * nz;
    
					xf2 = xf2 + s2 * nx;
					yf2 = yf2 + s2 * ny;
					zf2 = zf2 + s2 * nz;
				}
    
				x1.Value = xf1;
				y1.Value = yf1;
				z1.Value = zf1;
    
				x2.Value = xf2;
				y2.Value = yf2;
				z2.Value = zf2;
			}
		}

		public override void reverseCenterOfMass(UpperSymmPackMatrix Dp)
		{
			Point centerOfMass = this.CenterOfMass;

			// Bestimme Aufpunkt
			UnknownParameter x1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X];
			UnknownParameter y1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y];
			UnknownParameter z1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Z];

			UnknownParameter x2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X];
			UnknownParameter y2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y];
			UnknownParameter z2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Z];

			UnknownParameter nX = this.parameters[ParameterType.VECTOR_X];
			UnknownParameter nY = this.parameters[ParameterType.VECTOR_Y];
			UnknownParameter nZ = this.parameters[ParameterType.VECTOR_Z];

			// Shift
			double xf1 = x1.Value + centerOfMass.X0;
			double yf1 = y1.Value + centerOfMass.Y0;
			double zf1 = z1.Value + centerOfMass.Z0;

			double xf2 = x2.Value + centerOfMass.X0;
			double yf2 = y2.Value + centerOfMass.Y0;
			double zf2 = z2.Value + centerOfMass.Z0;

			// bestimme Punkt mit kuerzesten Abstand zum Ursprung
			double nx = nX.Value;
			double ny = nY.Value;
			double nz = nZ.Value;

			double dotNormalNormal = nx * nx + ny * ny + nz * nz;
			double squaredDotNormalNormal = dotNormalNormal * dotNormalNormal;

			double dotNormalF1 = nx * xf1 + ny * yf1 + nz * zf1;
			double dotNormalF2 = nx * xf2 + ny * yf2 + nz * zf2;

			double s1 = -dotNormalF1 / dotNormalNormal;
			double s2 = -dotNormalF2 / dotNormalNormal;

			if (Dp != null)
			{
				int nou = Dp.numColumns();
				Matrix J = Matrices.identity(nou);

				if (x1.Column >= 0)
				{
					if (x1.Column >= 0)
					{
						J.set(x1.Column, x1.Column, 1.0 - nx * nx / dotNormalNormal);
					}
					if (y1.Column >= 0)
					{
						J.set(x1.Column, y1.Column, - nx * ny / dotNormalNormal);
					}
					if (z1.Column >= 0)
					{
						J.set(x1.Column, z1.Column, - nx * nz / dotNormalNormal);
					}

					if (nX.Column >= 0)
					{
						J.set(x1.Column, nX.Column, (2.0 * nx * nx * dotNormalF1) / squaredDotNormalNormal - (nx * xf1) / dotNormalNormal + s1);
					}
					if (nY.Column >= 0)
					{
						J.set(x1.Column, nY.Column, (2.0 * nx * ny * dotNormalF1) / squaredDotNormalNormal - (nx * yf1) / dotNormalNormal);
					}
					if (nZ.Column >= 0)
					{
						J.set(x1.Column, nZ.Column, (2.0 * nx * nz * dotNormalF1) / squaredDotNormalNormal - (nx * zf1) / dotNormalNormal);
					}
				}

				if (y1.Column >= 0)
				{
					if (x1.Column >= 0)
					{
						J.set(y1.Column, x1.Column, - ny * nx / dotNormalNormal);
					}
					if (y1.Column >= 0)
					{
						J.set(y1.Column, y1.Column, 1.0 - ny * ny / dotNormalNormal);
					}
					if (z1.Column >= 0)
					{
						J.set(y1.Column, z1.Column, - ny * nz / dotNormalNormal);
					}

					if (nX.Column >= 0)
					{
						J.set(y1.Column, nX.Column, (2.0 * ny * nx * dotNormalF1) / squaredDotNormalNormal - (ny * xf1) / dotNormalNormal);
					}
					if (nY.Column >= 0)
					{
						J.set(y1.Column, nY.Column, (2.0 * ny * ny * dotNormalF1) / squaredDotNormalNormal - (ny * yf1) / dotNormalNormal + s1);
					}
					if (nZ.Column >= 0)
					{
						J.set(y1.Column, nZ.Column, (2.0 * ny * nz * dotNormalF1) / squaredDotNormalNormal - (ny * zf1) / dotNormalNormal);
					}
				}

				if (z1.Column >= 0)
				{
					if (x1.Column >= 0)
					{
						J.set(z1.Column, x1.Column, - nz * nx / dotNormalNormal);
					}
					if (y1.Column >= 0)
					{
						J.set(z1.Column, y1.Column, - nz * ny / dotNormalNormal);
					}
					if (z1.Column >= 0)
					{
						J.set(z1.Column, z1.Column, 1.0 - nz * nz / dotNormalNormal);
					}

					if (nX.Column >= 0)
					{
						J.set(z1.Column, nX.Column, (2.0 * nz * nx * dotNormalF1) / squaredDotNormalNormal - (nz * xf1) / dotNormalNormal);
					}
					if (nY.Column >= 0)
					{
						J.set(z1.Column, nY.Column, (2.0 * nz * ny * dotNormalF1) / squaredDotNormalNormal - (nz * yf1) / dotNormalNormal);
					}
					if (nZ.Column >= 0)
					{
						J.set(z1.Column, nZ.Column, (2.0 * nz * nz * dotNormalF1) / squaredDotNormalNormal - (nz * zf1) / dotNormalNormal + s1);
					}
				}

				if (x2.Column >= 0)
				{
					if (x2.Column >= 0)
					{
						J.set(x2.Column, x2.Column, 1.0 - nx * nx / dotNormalNormal);
					}
					if (y2.Column >= 0)
					{
						J.set(x2.Column, y2.Column, - nx * ny / dotNormalNormal);
					}
					if (z2.Column >= 0)
					{
						J.set(x2.Column, z2.Column, - nx * nz / dotNormalNormal);
					}

					if (nX.Column >= 0)
					{
						J.set(x2.Column, nX.Column, (2.0 * nx * nx * dotNormalF2) / squaredDotNormalNormal - (nx * xf2) / dotNormalNormal + s2);
					}
					if (nY.Column >= 0)
					{
						J.set(x2.Column, nY.Column, (2.0 * nx * ny * dotNormalF2) / squaredDotNormalNormal - (nx * yf2) / dotNormalNormal);
					}
					if (nZ.Column >= 0)
					{
						J.set(x2.Column, nZ.Column, (2.0 * nx * nz * dotNormalF2) / squaredDotNormalNormal - (nx * zf2) / dotNormalNormal);
					}
				}

				if (y2.Column >= 0)
				{
					if (x2.Column >= 0)
					{
						J.set(y2.Column, x2.Column, - ny * nx / dotNormalNormal);
					}
					if (y2.Column >= 0)
					{
						J.set(y2.Column, y2.Column, 1.0 - ny * ny / dotNormalNormal);
					}
					if (z2.Column >= 0)
					{
						J.set(y2.Column, z2.Column, - ny * nz / dotNormalNormal);
					}

					if (nX.Column >= 0)
					{
						J.set(y2.Column, nX.Column, (2.0 * ny * nx * dotNormalF2) / squaredDotNormalNormal - (ny * xf2) / dotNormalNormal);
					}
					if (nY.Column >= 0)
					{
						J.set(y2.Column, nY.Column, (2.0 * ny * ny * dotNormalF2) / squaredDotNormalNormal - (ny * yf2) / dotNormalNormal + s2);
					}
					if (nZ.Column >= 0)
					{
						J.set(y2.Column, nZ.Column, (2.0 * ny * nz * dotNormalF2) / squaredDotNormalNormal - (ny * zf2) / dotNormalNormal);
					}
				}

				if (z2.Column >= 0)
				{
					if (x2.Column >= 0)
					{
						J.set(z2.Column, x2.Column, - nz * nx / dotNormalNormal);
					}
					if (y2.Column >= 0)
					{
						J.set(z2.Column, y2.Column, - nz * ny / dotNormalNormal);
					}
					if (z2.Column >= 0)
					{
						J.set(z2.Column, z2.Column, 1.0 - nz * nz / dotNormalNormal);
					}

					if (nX.Column >= 0)
					{
						J.set(z2.Column, nX.Column, (2.0 * nz * nx * dotNormalF2) / squaredDotNormalNormal - (nz * xf2) / dotNormalNormal);
					}
					if (nY.Column >= 0)
					{
						J.set(z2.Column, nY.Column, (2.0 * nz * ny * dotNormalF2) / squaredDotNormalNormal - (nz * yf2) / dotNormalNormal);
					}
					if (nZ.Column >= 0)
					{
						J.set(z2.Column, nZ.Column, (2.0 * nz * nz * dotNormalF2) / squaredDotNormalNormal - (nz * zf2) / dotNormalNormal + s2);
					}
				}

				Matrix JDp = new DenseMatrix(nou, nou);
				J.mult(Dp, JDp);
				JDp.transBmult(J, Dp);
			}

			xf1 = xf1 + s1 * nx;
			yf1 = yf1 + s1 * ny;
			zf1 = zf1 + s1 * nz;

			xf2 = xf2 + s2 * nx;
			yf2 = yf2 + s2 * ny;
			zf2 = zf2 + s2 * nz;

			x1.Value = xf1;
			y1.Value = yf1;
			z1.Value = zf1;

			x2.Value = xf2;
			y2.Value = yf2;
			z2.Value = zf2;
		}

		public override ICollection<Restriction> Restrictions
		{
			get
			{
				return List.of(this.primaryFocalRestriction, this.secondaryFocalRestriction, this.vectorLengthRestriction);
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

			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X] = new UnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_X, true);
			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y] = new UnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Y, true);
			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Z] = new UnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Z, true);

			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X] = new UnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_X, true);
			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y] = new UnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Y, true);
			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Z] = new UnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Z, true);

			this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT] = new UnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT, true);

			this.parameters[ParameterType.VECTOR_X] = new UnknownParameter(ParameterType.VECTOR_X, true, 0);
			this.parameters[ParameterType.VECTOR_Y] = new UnknownParameter(ParameterType.VECTOR_Y, true, 0);
			this.parameters[ParameterType.VECTOR_Z] = new UnknownParameter(ParameterType.VECTOR_Z, true, 1);

			this.parameters[ParameterType.VECTOR_LENGTH] = new UnknownParameter(ParameterType.VECTOR_LENGTH, true, 1.0, true, ProcessingType.FIXED);
			this.parameters[ParameterType.CONSTANT] = new UnknownParameter(ParameterType.CONSTANT, true, 0.0, false, ProcessingType.FIXED);


			UnknownParameter zero = this.parameters[ParameterType.CONSTANT];

			UnknownParameter xFocal1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X];
			UnknownParameter yFocal1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y];
			UnknownParameter zFocal1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Z];

			UnknownParameter xFocal2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X];
			UnknownParameter yFocal2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y];
			UnknownParameter zFocal2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Z];

			UnknownParameter normalX = this.parameters[ParameterType.VECTOR_X];
			UnknownParameter normalY = this.parameters[ParameterType.VECTOR_Y];
			UnknownParameter normalZ = this.parameters[ParameterType.VECTOR_Z];
			UnknownParameter vectorLength = this.parameters[ParameterType.VECTOR_LENGTH];

			IList<UnknownParameter> normalVector = new List<UnknownParameter> {normalX, normalY, normalZ};
			this.vectorLengthRestriction = new ProductSumRestriction(true, normalVector, normalVector, vectorLength);

			IList<UnknownParameter> primaryFocalVector = new List<UnknownParameter> {xFocal1, yFocal1, zFocal1};
			this.primaryFocalRestriction = new ProductSumRestriction(true, primaryFocalVector, normalVector, zero);

			IList<UnknownParameter> secondaryFocalVector = new List<UnknownParameter> {xFocal2, yFocal2, zFocal2};
			this.secondaryFocalRestriction = new ProductSumRestriction(true, secondaryFocalVector, normalVector, zero);
		}

		public override PrimitiveType PrimitiveType
		{
			get
			{
				return PrimitiveType.CYLINDER;
			}
		}

		public override string toLaTex()
		{
			return "$\\sum_{j=1}^2 s_j = 2 a" + " \\\\ " + "s_j = \\vert \\mathbf{n} \\times \\left( \\mathbf{P}_i - \\mathbf{F}_j \\right) \\vert$";
		}
	}

}