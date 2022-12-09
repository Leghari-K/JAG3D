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
	using Ellipse = org.applied_geodesy.adjustment.geometry.curve.primitive.Ellipse;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using AverageRestriction = org.applied_geodesy.adjustment.geometry.restriction.AverageRestriction;
	using ProductSumRestriction = org.applied_geodesy.adjustment.geometry.restriction.ProductSumRestriction;

	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using EVD = no.uib.cipr.matrix.EVD;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;
	using Vector = no.uib.cipr.matrix.Vector;

	public class EllipseFeature : CurveFeature
	{
		private readonly Ellipse ellipse;

		public EllipseFeature() : base(true)
		{

			this.ellipse = new Ellipse();

			UnknownParameter xFocal1 = this.ellipse.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_X);
			UnknownParameter yFocal1 = this.ellipse.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Y);

			UnknownParameter xFocal2 = this.ellipse.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_X);
			UnknownParameter yFocal2 = this.ellipse.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Y);

			UnknownParameter majorAxis = this.ellipse.getUnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT);

			UnknownParameter xOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_X, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Y, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter minorAxis = new UnknownParameter(ParameterType.MINOR_AXIS_COEFFICIENT, false, 0.0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter xEccentricity = new UnknownParameter(ParameterType.VECTOR_X, false, 0.0, false, ProcessingType.POSTPROCESSING);
			UnknownParameter yEccentricity = new UnknownParameter(ParameterType.VECTOR_Y, false, 0.0, false, ProcessingType.POSTPROCESSING);
			UnknownParameter eccentricity = new UnknownParameter(ParameterType.LENGTH, false, 0.0, false, ProcessingType.POSTPROCESSING);

			UnknownParameter one = new UnknownParameter(ParameterType.CONSTANT, false, 1.0, false, ProcessingType.FIXED);

			AverageRestriction xOriginRestriction = new AverageRestriction(false, new List<UnknownParameter> {xFocal1, xFocal2}, xOrigin);
			AverageRestriction yOriginRestriction = new AverageRestriction(false, new List<UnknownParameter> {yFocal1, yFocal2}, yOrigin);

			ProductSumRestriction xEccentricityRestriction = new ProductSumRestriction(false, List.of(xFocal1, xOrigin), List.of(one, one), false, xEccentricity);
			ProductSumRestriction yEccentricityRestriction = new ProductSumRestriction(false, List.of(yFocal1, yOrigin), List.of(one, one), false, yEccentricity);

			ProductSumRestriction eccentricityRestriction = new ProductSumRestriction(false, List.of(xEccentricity, yEccentricity), List.of(xEccentricity, yEccentricity), 0.5, eccentricity);
			ProductSumRestriction minorAxisRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {majorAxis, eccentricity}, new List<UnknownParameter> {majorAxis, eccentricity}, 0.5, false, minorAxis);

			this.add(this.ellipse);


			IList<UnknownParameter> newOrderedUnknownParameters = new List<UnknownParameter>();
			newOrderedUnknownParameters.Add(xOrigin);
			newOrderedUnknownParameters.Add(yOrigin);

			((List<UnknownParameter>)newOrderedUnknownParameters).AddRange(this.UnknownParameters);
			newOrderedUnknownParameters.Add(minorAxis);

			newOrderedUnknownParameters.Add(eccentricity);
			newOrderedUnknownParameters.Add(xEccentricity);
			newOrderedUnknownParameters.Add(yEccentricity);

			newOrderedUnknownParameters.Add(one);

			this.UnknownParameters.setAll(newOrderedUnknownParameters);

			this.PostProcessingCalculations.addAll(xOriginRestriction, yOriginRestriction, xEccentricityRestriction, yEccentricityRestriction, eccentricityRestriction, minorAxisRestriction);
		}

		public virtual Ellipse Ellipse
		{
			get
			{
				return this.ellipse;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, EllipseFeature feature) throws MatrixSingularException, IllegalArgumentException, UnsupportedOperationException, no.uib.cipr.matrix.NotConvergedException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, EllipseFeature feature)
		{
			deriveInitialGuess(points, feature.ellipse);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.curve.primitive.Ellipse ellipse) throws MatrixSingularException, IllegalArgumentException, UnsupportedOperationException, no.uib.cipr.matrix.NotConvergedException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Ellipse ellipse)
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

				if (ellipse.Dimension > point.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + ellipse.Dimension + " != " + point.Dimension);
				}
			}

			if (nop < 5)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 5 points are needed.");
			}

			x0 /= nop;
			y0 /= nop;

			// A. W. Fitzgibbon, M. Pilu and R. B. Fisher, "Direct least squares fitting of ellipses," 
			// Proceedings of 13th International Conference on Pattern Recognition, Vienna, Austria, 1996, 
			// pp. 253-257 vol.1, doi: 10.1109/ICPR.1996.546029.
			// http://autotrace.sourceforge.net/WSCG98.pdf

			// D1 = [xx xy yy]
			// D2 = [x y 1]
			// S1 = D1' * D1
			// S2 = D1' * D2
			// S3 = D2' * D2

			UpperSymmPackMatrix S1 = new UpperSymmPackMatrix(3);
			UpperSymmPackMatrix S3 = new UpperSymmPackMatrix(3);
			DenseMatrix S2 = new DenseMatrix(3,3);

			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}

				double xi = point.X0 - x0;
				double yi = point.Y0 - y0;

				double xx = xi * xi;
				double xy = xi * yi;
				double yy = yi * yi;

				S1.add(0, 0, xx * xx);
				S1.add(0, 1, xx * xy);
				S1.add(0, 2, xx * yy);

				S1.add(1, 1, xy * xy);
				S1.add(1, 2, xy * yy);

				S1.add(2, 2, yy * yy);


				S3.add(0, 0, xx);
				S3.add(0, 1, xy);
				S3.add(0, 2, xi);

				S3.add(1, 1, yy);
				S3.add(1, 2, yi);

				S3.add(2, 2, 1.0);


				S2.add(0, 0, xx * xi);
				S2.add(0, 1, xx * yi);
				S2.add(0, 2, xx);

				S2.add(1, 0, xy * xi);
				S2.add(1, 1, xy * yi);
				S2.add(1, 2, xy);

				S2.add(2, 0, yy * xi);
				S2.add(2, 1, yy * yi);
				S2.add(2, 2, yy);
			}

			// T = - inv(S3) * S2'
			// invert S3 in-place
			MathExtension.inv(S3);
			DenseMatrix T = new DenseMatrix(3,3);

			//C = alpha*A*BT
			S3.transBmult(-1.0, S2, T);

			// M = S2 * T + S1;
			DenseMatrix M = new DenseMatrix(S1, true);
			// C = A*B + C
			S2.multAdd(T, M);

			// M = inv(C) * M
			// directly storing the inverse of the condition matrix C
			//     [0  0  2              [0   0  0.5
			// C =  0 -1  0   --> invC =  0  -1   0
			//      2  0  0]             0.5  0   0]
			DenseMatrix C = new DenseMatrix(3,3);
			C.set(0, 2, 0.5);
			C.set(1, 1, -1.0);
			C.set(2, 0, 0.5);

			// overwritting S2 by permutation inv(C) * M
			C.mult(M, S2);

			// solving eigen-system
			EVD evd = new EVD(3, false, true);
			evd.factor(S2);

			DenseMatrix evec = evd.getRightEigenvectors();

			// cond = 4 * evec(1, :) * evec(3, :) - evec(2, :) * evec(2, :);
			// search for eigenvector for min. pos. eigenvalue
			// u1 = [a b c]' = evec(:, find(cond > 0)) 
			int idx = 0;
			for (int i = 0; i < 3; i++)
			{
				double cond = 4.0 * evec.get(0, i) * evec.get(2, i) - evec.get(1, i) * evec.get(1, i);
				if (cond > 0)
				{
					idx = i;
					break;
				}
			}

			// estimate u2 = [d f g]' from a1 --> u2 = T * u1
			Vector u = new DenseVector(6);
			for (int i = 0; i < 3; i++)
			{
				double ui = evec.get(i, idx);
				u.set(i, ui);

				ui = 0;
				for (int j = 0; j < 3; j++)
				{
					ui += T.get(i, j) * evec.get(j, idx);
				}
				u.add(3 + i, ui);
			}

			// invert centroid reduction
			double a = u.get(0);
			double b = u.get(1);
			double c = u.get(2);

			double d = u.get(3);
			double f = u.get(4);
			double g = u.get(5);

			// derive geometric parametrers: x0, y0, a, b and phi
			// https://mathworld.wolfram.com/Ellipse.html
			g = g + a * x0 * x0 + b * x0 * y0 + c * y0 * y0 - d * x0 - f * y0;
			f = 0.5 * (f - 2.0 * c * y0 - b * x0);
			d = 0.5 * (d - 2.0 * a * x0 - b * y0);
			b = 0.5 * b;

			double k = b * b - a * c;
			x0 = (c * d - b * f) / k;
			y0 = (a * f - b * d) / k;

			double D = Math.Sqrt(Math.Abs((a - c) * (a - c) + 4.0 * b * b));
			double an = k * (D - (a + c));
			double bn = k * (-D - (a + c));

			double z = 2.0 * (a * f * f + c * d * d + g * b * b - 2.0 * b * d * f - a * c * g);

			double major = Math.Sqrt(Math.Abs(z / an));
			double minor = Math.Sqrt(Math.Abs(z / bn));

			double phi = 0;
			if (major > minor)
			{
				phi = 0.5 * Math.PI + 0.5 * Math.Atan2(2.0 * b, a - c);
			}
			else
			{ //major < minor
				phi = 0.5 * Math.Atan2(2.0 * b, a - c);
				double tmp = major;
				major = minor;
				minor = tmp;
			}

			double ext = Math.Sqrt(major * major - minor * minor);

			double x1 = x0 + ext * Math.Cos(phi);
			double y1 = y0 + ext * Math.Sin(phi);

			double x2 = x0 - ext * Math.Cos(phi);
			double y2 = y0 - ext * Math.Sin(phi);

			ellipse.setInitialGuess(x1, y1, x2, y2, major);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, UnsupportedOperationException, no.uib.cipr.matrix.NotConvergedException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.ellipse.FeaturePoints, this.ellipse);
		}
	}

}