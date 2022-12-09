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

namespace org.applied_geodesy.adjustment.geometry.curve
{

	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using CurveFeature = org.applied_geodesy.adjustment.geometry.CurveFeature;
	using QuadraticCurve = org.applied_geodesy.adjustment.geometry.curve.primitive.QuadraticCurve;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;

	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SymmPackEVD = no.uib.cipr.matrix.SymmPackEVD;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class QuadraticCurveFeature : CurveFeature
	{
		private readonly QuadraticCurve quadraticCurve;
		private static readonly double SQRT2 = Math.Sqrt(2.0);

		public QuadraticCurveFeature() : base(true)
		{

			this.quadraticCurve = new QuadraticCurve();

			UnknownParameter vectorLength = this.quadraticCurve.getUnknownParameter(ParameterType.VECTOR_LENGTH);
			vectorLength.Visible = false;

			this.add(this.quadraticCurve);
		}

		public virtual QuadraticCurve QuadraticCurve
		{
			get
			{
				return this.quadraticCurve;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, QuadraticCurveFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, QuadraticCurveFeature feature)
		{
			deriveInitialGuess(points, feature.quadraticCurve);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.curve.primitive.QuadraticCurve quadraticCurve) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, QuadraticCurve quadraticCurve)
		{
			int nop = 0;
			double x0 = 0, y0 = 0;
			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}

				nop++;
				x0 += point.X0;
				y0 += point.Y0;

				if (quadraticCurve.Dimension > point.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + quadraticCurve.Dimension + " != " + point.Dimension);
				}
			}

			if (nop < 5)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 5 points are needed.");
			}

			x0 /= nop;
			y0 /= nop;

			UpperSymmPackMatrix A1TA1 = new UpperSymmPackMatrix(3);
			UpperSymmPackMatrix A2TA2 = new UpperSymmPackMatrix(3);
			DenseMatrix A1TA2 = new DenseMatrix(3,3);

			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}

				double xi = point.X0 - x0;
				double yi = point.Y0 - y0;

				double xx = xi * xi;
				double yy = yi * yi;

				double xy = xi * yi;

				A1TA1.add(0,0, xx * xx);
				A1TA1.add(0,1, xx * yy);
				A1TA1.add(0,2, xx * SQRT2 * xy);

				A1TA1.add(1,1, yy * yy);
				A1TA1.add(1,2, yy * SQRT2 * xy);

				A1TA1.add(2,2, 2.0 * xy * xy);


				A2TA2.add(0,0, xx);
				A2TA2.add(0,1, xy);
				A2TA2.add(0,2, xi);

				A2TA2.add(1,1, yy);
				A2TA2.add(1,2, yi);

				A2TA2.add(2,2, 1.0);


				A1TA2.add(0,0, xx * xi);
				A1TA2.add(0,1, xx * yi);
				A1TA2.add(0,2, xx);

				A1TA2.add(1,0, yy * xi);
				A1TA2.add(1,1, yy * yi);
				A1TA2.add(1,2, yy);

				A1TA2.add(2,0, SQRT2 * xy * xi);
				A1TA2.add(2,1, SQRT2 * xy * yi);
				A1TA2.add(2,2, SQRT2 * xy);
			}

			// H = P - P * A2 * inv(A2' * A2) * A2' * P
			// T = A1' * H * A1
			// ignoring stochastic properties, i.e. P = I
			// T = A1' * A1  -  A1' * A2 * inv(A2' * A2) * A2' * A1
			MathExtension.inv(A2TA2);
			Matrix invA2TA2timesA1TA2 = new DenseMatrix(3,3);
			A2TA2.transBmult(A1TA2, invA2TA2timesA1TA2);

			UpperSymmPackMatrix A1TA2invA2TA2timesA1TA2 = new UpperSymmPackMatrix(3);
			A1TA2.mult(invA2TA2timesA1TA2, A1TA2invA2TA2timesA1TA2);
			A1TA1.add(-1.0, A1TA2invA2TA2timesA1TA2);


			// solving eigen-system
			SymmPackEVD evd = new SymmPackEVD(3, true, true);
			evd.factor(A1TA1);

			Matrix evec = evd.getEigenvectors();
			double[] eval = evd.getEigenvalues();

			int indexMinEval = 0;
			double minEval = Math.Abs(eval[indexMinEval]);
			for (int idx = indexMinEval + 1; idx < eval.Length; idx++)
			{
				if (minEval > Math.Abs(eval[idx]))
				{
					minEval = Math.Abs(eval[idx]);
					indexMinEval = idx;
				}
			}

			DenseVector u1 = new DenseVector(3);
			DenseVector u2 = new DenseVector(3);

			for (int i = 0; i < 3; i++)
			{
				double x = evec.get(i, indexMinEval);
				u1.set(i, x);
			}

			invA2TA2timesA1TA2.mult(-1.0, u1, u2);

			double a = u1.get(0);
			double b = u1.get(1);
			double c = u1.get(2);

			double d = u2.get(0);
			double e = u2.get(1);

			double length = u2.get(1);

			// remove center of mass
			d = d - 2.0 * (a * x0 + c / SQRT2 * y0);
			e = e - 2.0 * (c / SQRT2 * x0 + b * y0);
			length = length - (a * x0 * x0 + b * y0 * y0 + c * SQRT2 * x0 * y0 + d * x0 + e * y0);

			quadraticCurve.setInitialGuess(a, b, c, d, e, length);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.quadraticCurve.FeaturePoints, this.quadraticCurve);
		}
	}

}