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
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Ellipsoid = org.applied_geodesy.adjustment.geometry.surface.primitive.Ellipsoid;
	using QuadraticSurface = org.applied_geodesy.adjustment.geometry.surface.primitive.QuadraticSurface;

	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SymmPackEVD = no.uib.cipr.matrix.SymmPackEVD;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class EllipsoidFeature : SurfaceFeature
	{
		private readonly Ellipsoid ellipsoid;

		public EllipsoidFeature() : base(true)
		{

			this.ellipsoid = new Ellipsoid();
			this.add(this.ellipsoid);
		}

		public virtual Ellipsoid Ellipsoid
		{
			get
			{
				return this.ellipsoid;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, EllipsoidFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, EllipsoidFeature feature)
		{
			deriveInitialGuess(points, feature.ellipsoid);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Ellipsoid ellipsoid) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Ellipsoid ellipsoid)
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
			double length = quadraticSurface.getUnknownParameter(ParameterType.LENGTH).Value0;

			DenseVector invUu = new DenseVector(new double[] {g, h, i}, false);
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
			MathExtension.solve(U, invUu, false);
			double x0 = -0.5 * invUu.get(0);
			double y0 = -0.5 * invUu.get(1);
			double z0 = -0.5 * invUu.get(2);

			// solving eigen-system
			SymmPackEVD evd = new SymmPackEVD(dim, true, true);
			evd.factor(H);

			Matrix evec = evd.getEigenvectors();
			double[] eval = evd.getEigenvalues();

			d = -0.25 * u.dot(invUu) + length;

			// Halbachsen
			a = Math.Sqrt(Math.Abs(eval[0] / d));
			b = Math.Sqrt(Math.Abs(eval[1] / d));
			c = Math.Sqrt(Math.Abs(eval[2] / d));

			// transpose of rotation matrix
			double r11 = evec.get(0, 0);
			double r12 = evec.get(1, 0);
			double r13 = evec.get(2, 0);

			double r21 = evec.get(0, 1);
			double r22 = evec.get(1, 1);
			double r23 = evec.get(2, 1);

			double r31 = evec.get(0, 2);
			double r32 = evec.get(1, 2);
			double r33 = evec.get(2, 2);

			// check det(R) = +1
			double det = r11 * r22 * r33 + r12 * r23 * r31 + r13 * r21 * r32 - r13 * r22 * r31 - r12 * r21 * r33 - r11 * r23 * r32;
			if (det < 0)
			{
				r31 = -r31;
				r32 = -r32;
				r33 = -r33;
			}

			ellipsoid.setInitialGuess(x0, y0, z0, a, c, b, r11, r12, r13, r21, r22, r23, r31, r32, r33);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.ellipsoid.FeaturePoints, this.ellipsoid);
		}
	}

}