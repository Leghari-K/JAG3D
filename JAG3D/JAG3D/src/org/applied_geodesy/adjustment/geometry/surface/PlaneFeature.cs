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
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Plane = org.applied_geodesy.adjustment.geometry.surface.primitive.Plane;

	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SymmPackEVD = no.uib.cipr.matrix.SymmPackEVD;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class PlaneFeature : SurfaceFeature
	{
		private readonly Plane plane;

		public PlaneFeature() : base(true)
		{

			this.plane = new Plane();

			UnknownParameter vectorLength = this.plane.getUnknownParameter(ParameterType.VECTOR_LENGTH);
			vectorLength.Visible = false;

			this.add(this.plane);
		}

		public virtual Plane Plane
		{
			get
			{
				return this.plane;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, PlaneFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, PlaneFeature feature)
		{
			deriveInitialGuess(points, feature.plane);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Plane plane) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Plane plane)
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
				if (plane.Dimension > point.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + plane.Dimension + " != " + point.Dimension);
				}
			}

			if (nop < 3)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 3 points are needed.");
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
					else
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
						else
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

			int indexMinEigVal = 0;
			double minEigVal = eigVal[indexMinEigVal];
			for (int i = indexMinEigVal + 1; i < eigVal.Length; i++)
			{
				if (minEigVal > eigVal[i])
				{
					minEigVal = eigVal[i];
					indexMinEigVal = i;
				}
			}

			// Normal vector n of the plane is eigenvector which corresponds to the smallest eigenvalue 
			double nx = eigVec.get(0, indexMinEigVal);
			double ny = eigVec.get(1, indexMinEigVal);
			double nz = eigVec.get(2, indexMinEigVal);
			double d = nx * x0 + ny * y0 + nz * z0;

			// Change orientation so that d is a positive distance
			if (d < 0)
			{
				nx = -nx;
				ny = -ny;
				nz = -nz;
				d = -d;
			}

			plane.setInitialGuess(nx, ny, nz, d);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.plane.FeaturePoints, this.plane);
		}
	}

}