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

namespace org.applied_geodesy.adjustment.geometry.surface
{

	using SurfaceFeature = org.applied_geodesy.adjustment.geometry.SurfaceFeature;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Point = org.applied_geodesy.adjustment.geometry.point.Point;
	using ProductSumRestriction = org.applied_geodesy.adjustment.geometry.restriction.ProductSumRestriction;
	using Plane = org.applied_geodesy.adjustment.geometry.surface.primitive.Plane;

	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SymmPackEVD = no.uib.cipr.matrix.SymmPackEVD;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class SpatialLineFeature : SurfaceFeature
	{
		private readonly Plane planeU;
		private readonly Plane planeV;

		private UnknownParameter xCoordinateInPlaneU, yCoordinateInPlaneU, zCoordinateInPlaneU;

		public SpatialLineFeature() : base(true)
		{

			this.planeU = new Plane();
			this.planeV = new Plane();

			this.xCoordinateInPlaneU = new UnknownParameter(ParameterType.COORDINATE_X, true, 0, false, ProcessingType.FIXED);
			this.yCoordinateInPlaneU = new UnknownParameter(ParameterType.COORDINATE_Y, true, 0, false, ProcessingType.FIXED);
			this.zCoordinateInPlaneU = new UnknownParameter(ParameterType.COORDINATE_Z, true, 0, false, ProcessingType.FIXED);

			UnknownParameter xOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_X, false, 0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Y, false, 0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter zOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Z, false, 0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter xNormal = new UnknownParameter(ParameterType.VECTOR_X, false, 0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yNormal = new UnknownParameter(ParameterType.VECTOR_Y, false, 0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter zNormal = new UnknownParameter(ParameterType.VECTOR_Z, false, 0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter dotProductUV = new UnknownParameter(ParameterType.LENGTH, false, 0, false, ProcessingType.FIXED);

			UnknownParameter ux = this.planeU.getUnknownParameter(ParameterType.VECTOR_X);
			UnknownParameter uy = this.planeU.getUnknownParameter(ParameterType.VECTOR_Y);
			UnknownParameter uz = this.planeU.getUnknownParameter(ParameterType.VECTOR_Z);
			UnknownParameter du = this.planeU.getUnknownParameter(ParameterType.LENGTH);
			UnknownParameter u = this.planeU.getUnknownParameter(ParameterType.VECTOR_LENGTH);

			UnknownParameter vx = this.planeV.getUnknownParameter(ParameterType.VECTOR_X);
			UnknownParameter vy = this.planeV.getUnknownParameter(ParameterType.VECTOR_Y);
			UnknownParameter vz = this.planeV.getUnknownParameter(ParameterType.VECTOR_Z);
			UnknownParameter dv = this.planeV.getUnknownParameter(ParameterType.LENGTH);
			UnknownParameter v = this.planeU.getUnknownParameter(ParameterType.VECTOR_LENGTH);

			ux.Visible = false;
			uy.Visible = false;
			uz.Visible = false;
			du.Visible = false;
			u.Visible = false;

			vx.Visible = false;
			vy.Visible = false;
			vz.Visible = false;
			dv.Visible = false;
			v.Visible = false;

			IList<UnknownParameter> normalVectorU = new List<UnknownParameter> {ux, uy, uz};
			IList<UnknownParameter> normalVectorV = new List<UnknownParameter> {vx, vy, vz};
			IList<UnknownParameter> arbitraryPoint = new List<UnknownParameter> {this.xCoordinateInPlaneU, this.yCoordinateInPlaneU, this.zCoordinateInPlaneU};

			ProductSumRestriction arbitraryPointInPlaneURestriction = new ProductSumRestriction(true, normalVectorU, arbitraryPoint, du);
			ProductSumRestriction orthogonalVectorsUVRestriction = new ProductSumRestriction(true, normalVectorU, normalVectorV, dotProductUV);

			this.add(this.planeU);
			this.add(this.planeV);

			this.Restrictions.add(arbitraryPointInPlaneURestriction);
			this.Restrictions.add(orthogonalVectorsUVRestriction);

			IList<UnknownParameter> newOrderedUnknownParameters = new List<UnknownParameter>();
			newOrderedUnknownParameters.Add(xOrigin);
			newOrderedUnknownParameters.Add(yOrigin);
			newOrderedUnknownParameters.Add(zOrigin);

			newOrderedUnknownParameters.Add(xNormal);
			newOrderedUnknownParameters.Add(yNormal);
			newOrderedUnknownParameters.Add(zNormal);

			newOrderedUnknownParameters.Add(dotProductUV);

			newOrderedUnknownParameters.Add(this.xCoordinateInPlaneU);
			newOrderedUnknownParameters.Add(this.yCoordinateInPlaneU);
			newOrderedUnknownParameters.Add(this.zCoordinateInPlaneU);

			((List<UnknownParameter>)newOrderedUnknownParameters).AddRange(this.UnknownParameters);
			this.UnknownParameters.setAll(newOrderedUnknownParameters);

			ProductSumRestriction multAddX = new ProductSumRestriction(false, new List<UnknownParameter> {ux, vx}, new List<UnknownParameter> {du, dv}, xOrigin);
			ProductSumRestriction multAddY = new ProductSumRestriction(false, new List<UnknownParameter> {uy, vy}, new List<UnknownParameter> {du, dv}, yOrigin);
			ProductSumRestriction multAddZ = new ProductSumRestriction(false, new List<UnknownParameter> {uz, vz}, new List<UnknownParameter> {du, dv}, zOrigin);

			// derive direction vector of the line
			// cross product single equation: A1 * B2 - A2 * B1  ==  C
			ProductSumRestriction crossProductX = new ProductSumRestriction(false, List.of(uy, uz), List.of(vz, vy), false, xNormal);
			ProductSumRestriction crossProductY = new ProductSumRestriction(false, List.of(uz, ux), List.of(vx, vz), false, yNormal);
			ProductSumRestriction crossProductZ = new ProductSumRestriction(false, List.of(ux, uy), List.of(vy, vx), false, zNormal);

			this.PostProcessingCalculations.addAll(multAddX, multAddY, multAddZ, crossProductX, crossProductY, crossProductZ);
		}

		public virtual Plane PlaneU
		{
			get
			{
				return this.planeU;
			}
		}

		public virtual Plane PlaneV
		{
			get
			{
				return this.planeV;
			}
		}

		public override Point CenterOfMass
		{
			set
			{
				// get previous center of mass
				Point prevCenterOfMass = this.CenterOfMass;
				if (value.equalsCoordinateComponents(prevCenterOfMass))
				{
					return;
				}
    
				base.CenterOfMass = value;
				this.adaptArbitraryPointInPlaneRestriction();
			}
		}

		public override void prepareIteration()
		{
			this.adaptArbitraryPointInPlaneRestriction();
		}

		private void adaptArbitraryPointInPlaneRestriction()
		{
			double ux = this.planeU.getUnknownParameter(ParameterType.VECTOR_X).Value;
			double uy = this.planeU.getUnknownParameter(ParameterType.VECTOR_Y).Value;
			double uz = this.planeU.getUnknownParameter(ParameterType.VECTOR_Z).Value;
			double du = this.planeU.getUnknownParameter(ParameterType.LENGTH).Value;

			double vx = this.planeV.getUnknownParameter(ParameterType.VECTOR_X).Value;
			double vy = this.planeV.getUnknownParameter(ParameterType.VECTOR_Y).Value;
			double vz = this.planeV.getUnknownParameter(ParameterType.VECTOR_Z).Value;
			double dv = this.planeV.getUnknownParameter(ParameterType.LENGTH).Value;

			// estimates a point, lying in both planes
			double x0 = ux * du + vx * dv;
			double y0 = uy * du + vy * dv;
			double z0 = uz * du + vz * dv;

			// move this position by vector v --> point lies in plane u but not in v
			x0 = x0 + vx;
			y0 = y0 + vy;
			z0 = z0 + vz;

			this.xCoordinateInPlaneU.Value = x0;
			this.yCoordinateInPlaneU.Value = y0;
			this.zCoordinateInPlaneU.Value = z0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<? extends org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, SpatialLineFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess<T1>(ICollection<T1> points, SpatialLineFeature feature) where T1 : org.applied_geodesy.adjustment.geometry.point.FeaturePoint
		{
			deriveInitialGuess(points, feature.planeU, feature.planeV);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<? extends org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Plane planeU, org.applied_geodesy.adjustment.geometry.surface.primitive.Plane planeV) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess<T1>(ICollection<T1> points, Plane planeU, Plane planeV) where T1 : org.applied_geodesy.adjustment.geometry.point.FeaturePoint
		{
			int nop = 0;
			double x0 = 0, y0 = 0, z0 = 0;
			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}

				nop++;
				x0 += point.X0;
				y0 += point.Y0;
				z0 += point.Z0;
				if (planeU.Dimension > point.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + planeU.Dimension + " != " + point.Dimension);
				}
			}

			if (nop < 2)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 2 points are needed.");
			}

			x0 /= nop;
			y0 /= nop;
			z0 /= nop;

			UpperSymmPackMatrix H = new UpperSymmPackMatrix(3);

			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}

				double xi = point.X0 - x0;
				double yi = point.Y0 - y0;
				double zi = point.Z0 - z0;

				for (int i = 0; i < 3; i++)
				{
					double hi = 0;

					if (i == 0)
					{
						hi = xi;
					}
					else if (i == 1)
					{
						hi = yi;
					}
					else if (i == 2)
					{
						hi = zi;
					}

					for (int j = i; j < 3; j++)
					{
						double hj = 0;

						if (j == 0)
						{
							hj = xi;
						}
						else if (j == 1)
						{
							hj = yi;
						}
						else if (j == 2)
						{
							hj = zi;
						}

						H.set(i, j, H.get(i,j) + hi * hj);
					}
				}
			}

			SymmPackEVD evd = new SymmPackEVD(3, true, true);
			evd.factor(H);

			Matrix eigVec = evd.getEigenvectors();
			double[] eigVal = evd.getEigenvalues();
			int indexMaxEigVal = 0;
			double maxEigVal = eigVal[indexMaxEigVal];
			for (int i = indexMaxEigVal + 1; i < eigVal.Length; i++)
			{
				if (maxEigVal < eigVal[i])
				{
					maxEigVal = eigVal[i];
					indexMaxEigVal = i;
				}
			}

			// Normal vector w of the line is eigenvector which corresponds to the largest eigenvalue
			// the orthonormal basis u x v is needed
			double[] u = new double[3];
			double[] v = new double[3];

			for (int i = 0; i < 3; i++)
			{
				if (indexMaxEigVal == 0)
				{
					u[i] = eigVec.get(i, 1);
					v[i] = eigVec.get(i, 2);
				}
				else if (indexMaxEigVal == 1)
				{
					u[i] = eigVec.get(i, 0);
					v[i] = eigVec.get(i, 2);
				}
				else
				{
					u[i] = eigVec.get(i, 0);
					v[i] = eigVec.get(i, 1);
				}
			}

			double du = u[0] * x0 + u[1] * y0 + u[2] * z0;
			double dv = v[0] * x0 + v[1] * y0 + v[2] * z0;

			planeU.setInitialGuess(u[0], u[1], u[2], du);
			planeV.setInitialGuess(v[0], v[1], v[2], dv);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			HashSet<FeaturePoint> uniquePointSet = new HashSet<FeaturePoint>();
			uniquePointSet.addAll(this.planeU.FeaturePoints);
			uniquePointSet.addAll(this.planeV.FeaturePoints);
			deriveInitialGuess(uniquePointSet, this.planeU, this.planeV);
		}
	}

}