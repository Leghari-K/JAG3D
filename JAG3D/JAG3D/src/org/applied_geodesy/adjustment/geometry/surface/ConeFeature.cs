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
	using Quaternion = org.applied_geodesy.adjustment.geometry.Quaternion;
	using SurfaceFeature = org.applied_geodesy.adjustment.geometry.SurfaceFeature;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;

	using Cone = org.applied_geodesy.adjustment.geometry.surface.primitive.Cone;
	using Cylinder = org.applied_geodesy.adjustment.geometry.surface.primitive.Cylinder;
	using Plane = org.applied_geodesy.adjustment.geometry.surface.primitive.Plane;
	using QuadraticSurface = org.applied_geodesy.adjustment.geometry.surface.primitive.QuadraticSurface;

	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SymmPackEVD = no.uib.cipr.matrix.SymmPackEVD;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class ConeFeature : SurfaceFeature
	{

		private readonly Cone cone;

		public ConeFeature() : base(true)
		{

			this.cone = new Cone();

			this.add(this.cone);
		}

		public virtual Cone Cone
		{
			get
			{
				return this.cone;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, ConeFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, ConeFeature feature)
		{
			deriveInitialGuess(points, feature.cone);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Cone cone) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Cone cone)
		{
			int nop = 0;

			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}

				nop++;
				if (cone.Dimension > point.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + cone.Dimension + " != " + point.Dimension);
				}
			}

			if (nop < 8)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 8 points are needed.");
			}

			// derive initial guess
			if (nop > 8)
			{
				deriveInitialGuessByQuadraticFunction(points, cone);
			}
			else
			{
				deriveInitialGuessByEllipse(points, cone);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.cone.FeaturePoints, this.cone);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static void deriveInitialGuessByEllipse(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Cone cone) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		private static void deriveInitialGuessByEllipse(ICollection<FeaturePoint> points, Cone cone)
		{
			Cylinder cylinder = new Cylinder();
			Plane plane = new Plane();
			SpatialEllipseFeature.deriveInitialGuess(points, cylinder, plane);

			double x1 = cylinder.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_X).Value0;
			double y1 = cylinder.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Y).Value0;
			double z1 = cylinder.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Z).Value0;

			double x2 = cylinder.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_X).Value0;
			double y2 = cylinder.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Y).Value0;
			double z2 = cylinder.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Z).Value0;

			double wx = plane.getUnknownParameter(ParameterType.VECTOR_X).Value0;
			double wy = plane.getUnknownParameter(ParameterType.VECTOR_Y).Value0;
			double wz = plane.getUnknownParameter(ParameterType.VECTOR_Z).Value0;

			double ac = cylinder.getUnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT).Value0;

			double x0 = 0.5 * (x1 + x2);
			double y0 = 0.5 * (y1 + y2);
			double z0 = 0.5 * (z1 + z2);

			double ux = x2 - x1;
			double uy = y2 - y1;
			double uz = z2 - z1;

			double bc = Math.Sqrt(ac * ac - 0.25 * (ux * ux + uy * uy + uz * uz));

			double normU = Math.Sqrt(ux * ux + uy * uy + uz * uz);
			ux /= normU;
			uy /= normU;
			uz /= normU;

			double vx = wy * uz - wz * uy;
			double vy = wz * ux - wx * uz;
			double vz = wx * uy - wy * ux;

			double normV = Math.Sqrt(vx * vx + vy * vy + vz * vz);
			vx /= normV;
			vy /= normV;
			vz /= normV;

	//		R = [ux vx wx
	//		     uy vy wy
	//		     uz vz wz]

			double det = ux * vy * wz + vx * wy * uz + wx * uy * vz - wx * vy * uz - vx * uy * wz - ux * wy * vz;
			if (det < 0)
			{
				wx = -wx;
				wy = -wy;
				wz = -wz;
			}

			Quaternion q = FeatureUtil.getQuaternionHz(new double[] {wx, wy, wz});
			ICollection<FeaturePoint> rotatedPoints = FeatureUtil.getRotatedFeaturePoints(points, new double[] {0, 0, 0}, q);
			Quaternion rotatedApexQ = q.rotate(new double[] {x0, y0, z0});
			double rx0 = rotatedApexQ.Q1;
			double ry0 = rotatedApexQ.Q2;
			double rz0 = rotatedApexQ.Q3;
			double a = 0, b = 0;
			int nop = 0;
			foreach (FeaturePoint point in rotatedPoints)
			{
				if (!point.Enable)
				{
					continue;
				}

				nop++;

				double rxi = point.X0;
				double ryi = point.Y0;
				double rzi = point.Z0;

				double dx = rxi - rx0;
				double dy = ryi - ry0;
				double dz = rzi - rz0;

				a += Math.Abs(dz * dz / (dx * dx + dy * dy));
			}
			a = b = Math.Sqrt(a / nop);

			// use ratio ac:bc of cylinder to derive parameter a, b
			double avg = 0.5 * (ac + bc);

			a = ac * a / avg;
			b = bc * b / avg;

	//		cone.setInitialGuess(x0, y0, z0, a, b, ux, vx, wx, uy, vy, wy, uz, vz, wz);
			cone.setInitialGuess(x0, y0, z0, a, b, ux, uy, uz, vx, vy, vz, wx, wy, wz);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static void deriveInitialGuessByQuadraticFunction(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Cone cone) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		private static void deriveInitialGuessByQuadraticFunction(ICollection<FeaturePoint> points, Cone cone)
		{
			const int dim = 3;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double INV_SQRT2 = Math.sqrt(2.0);
			double INV_SQRT2 = Math.Sqrt(2.0);

			QuadraticSurface quadraticSurface = new QuadraticSurface();
			QuadraticSurfaceFeature.deriveInitialGuess(points, quadraticSurface);

			double a = quadraticSurface.getUnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_A).Value0;
			double b = quadraticSurface.getUnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_B).Value0;
			double c = quadraticSurface.getUnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_C).Value0;
			double d = quadraticSurface.getUnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_D).Value0;
			double e = quadraticSurface.getUnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_E).Value0;
			double f = quadraticSurface.getUnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_F).Value0;
			double g = quadraticSurface.getUnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_G).Value0;
			double h = quadraticSurface.getUnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_H).Value0;
			double i = quadraticSurface.getUnknownParameter(ParameterType.POLYNOMIAL_COEFFICIENT_I).Value0;

			DenseVector u = new DenseVector(new double[] {g, h, i}, false);
			UpperSymmPackMatrix H = new UpperSymmPackMatrix(dim);
			H.set(0, 0, a);
			H.set(0, 1, d / INV_SQRT2);
			H.set(0, 2, e / INV_SQRT2);

			H.set(1, 1, b);
			H.set(1, 2, f / INV_SQRT2);

			H.set(2, 2, c);

			UpperSymmPackMatrix U = new UpperSymmPackMatrix(H);

			// estimate shift --> stored in u (U will be overwritten)
			MathExtension.solve(U, u, false);
			double x0 = -0.5 * u.get(0);
			double y0 = -0.5 * u.get(1);
			double z0 = -0.5 * u.get(2);

			// solving eigen-system
			SymmPackEVD evd = new SymmPackEVD(dim, true, true);
			evd.factor(H);

			Matrix evec = evd.getEigenvectors();
			double[] eval = evd.getEigenvalues();

			// lambda1 > 0, lambda2 > 0 and lamda3 < 0
			// evaluate signum of eigenvalues --> main axis of cone corresponds to lamda3
			int[] order = new int[]{0, 1, 2};
			if (Math.Sign(eval[0]) == Math.Sign(eval[1]))
			{
				a = Math.Sqrt(Math.Abs(eval[0] / eval[2]));
				c = Math.Sqrt(Math.Abs(eval[1] / eval[2]));
				if (a > c)
				{
					order = new int[]{0, 1, 2};
				}
				else
				{
					order = new int[]{1, 0, 2};
					double tmp = a;
					a = c;
					c = tmp;
				}
			}
			else if (Math.Sign(eval[0]) == Math.Sign(eval[2]))
			{
				a = Math.Sqrt(Math.Abs(eval[0] / eval[1]));
				c = Math.Sqrt(Math.Abs(eval[2] / eval[1]));
				if (a > c)
				{
					order = new int[]{0, 2, 1};
				}
				else
				{
					order = new int[]{2, 0, 1};
					double tmp = a;
					a = c;
					c = tmp;
				}
			}
			else
			{ //if (Math.signum(eigVal[1]) == Math.signum(eigVal[2]))
				a = Math.Sqrt(Math.Abs(eval[1] / eval[0]));
				c = Math.Sqrt(Math.Abs(eval[2] / eval[0]));
				if (a > c)
				{
					order = new int[]{1, 2, 0};
				}
				else
				{
					order = new int[]{2, 1, 0};
					double tmp = a;
					a = c;
					c = tmp;
				}
			}

			// interchange eigen-vectors depending on the order of the eigen-values 
			Matrix rotation = new DenseMatrix(dim, dim);
			for (int row = 0; row < dim; row++)
			{
				for (int column = 0; column < dim; column++)
				{
					int idx = order[column];
					rotation.set(row, column, evec.get(row, idx));
				}
			}

			// transpose of rotation matrix
			double r11 = rotation.get(0, 0);
			double r12 = rotation.get(1, 0);
			double r13 = rotation.get(2, 0);

			double r21 = rotation.get(0, 1);
			double r22 = rotation.get(1, 1);
			double r23 = rotation.get(2, 1);

			double r31 = rotation.get(0, 2);
			double r32 = rotation.get(1, 2);
			double r33 = rotation.get(2, 2);

			cone.setInitialGuess(x0, y0, z0, a, c, r11, r12, r13, r21, r22, r23, r31, r32, r33);
		}
	}

}