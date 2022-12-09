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

namespace org.applied_geodesy.adjustment.geometry.surface
{

	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using SurfaceFeature = org.applied_geodesy.adjustment.geometry.SurfaceFeature;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Sphere = org.applied_geodesy.adjustment.geometry.surface.primitive.Sphere;

	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class SphereFeature : SurfaceFeature
	{
		private readonly Sphere sphere;

		public SphereFeature() : base(true)
		{

			this.sphere = new Sphere();
			this.add(this.sphere);
		}

		public virtual Sphere Sphere
		{
			get
			{
				return this.sphere;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, SphereFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, SphereFeature feature)
		{
			deriveInitialGuess(points, feature.sphere);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Sphere sphere) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Sphere sphere)
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

				if (sphere.Dimension > point.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + sphere.Dimension + " != " + point.Dimension);
				}
			}

			if (nop < 4)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 4 points are needed.");
			}

			x0 /= nop;
			y0 /= nop;
			z0 /= nop;

			UpperSymmPackMatrix N = new UpperSymmPackMatrix(4);
			DenseVector n = new DenseVector(4);

			double r0 = 0;
			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}
				double xi = point.X0 - x0;
				double yi = point.Y0 - y0;
				double zi = point.Z0 - z0;

				double xxi = xi * xi;
				double yyi = yi * yi;
				double zzi = zi * zi;
				double xxyyzzi = xxi + yyi + zzi;

				N.set(0,0, N.get(0,0) + xxi);
				N.set(0,1, N.get(0,1) + xi * yi);
				N.set(0,2, N.get(0,2) + xi * zi);
				N.set(0,3, N.get(0,3) + xi);

				N.set(1,1, N.get(1,1) + yyi);
				N.set(1,2, N.get(1,2) + yi * zi);
				N.set(1,3, N.get(1,3) + yi);

				N.set(2,2, N.get(2,2) + zzi);
				N.set(2,3, N.get(2,3) + zi);

				N.set(3,3, N.get(3,3) + 1.0);

				n.set(0, n.get(0) + xi * xxyyzzi);
				n.set(1, n.get(1) + yi * xxyyzzi);
				n.set(2, n.get(2) + zi * xxyyzzi);
				n.set(3, n.get(3) + xxyyzzi);

				r0 += Math.Sqrt(xxyyzzi);
			}
			r0 /= nop;

			MathExtension.solve(N, n, false);
			r0 = Math.Sqrt(Math.Abs(0.25 * (n.get(0) * n.get(0) + n.get(1) * n.get(1) + n.get(2) * n.get(2)) + n.get(3)));

			x0 = x0 + 0.5 * n.get(0);
			y0 = y0 + 0.5 * n.get(1);
			z0 = z0 + 0.5 * n.get(2);

			sphere.setInitialGuess(x0, y0, z0, r0);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.sphere.FeaturePoints, this.sphere);
		}
	}

}