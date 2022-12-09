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

	using CurveFeature = org.applied_geodesy.adjustment.geometry.CurveFeature;
	using Line = org.applied_geodesy.adjustment.geometry.curve.primitive.Line;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using ProductSumRestriction = org.applied_geodesy.adjustment.geometry.restriction.ProductSumRestriction;

	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SymmPackEVD = no.uib.cipr.matrix.SymmPackEVD;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class LineFeature : CurveFeature
	{
		private readonly Line line;

		public LineFeature() : base(true)
		{

			this.line = new Line();

			UnknownParameter ux = this.line.getUnknownParameter(ParameterType.VECTOR_X);
			UnknownParameter uy = this.line.getUnknownParameter(ParameterType.VECTOR_Y);

			UnknownParameter du = this.line.getUnknownParameter(ParameterType.LENGTH);

			ux.Visible = false;
			uy.Visible = false;
			du.Visible = false;

			UnknownParameter xOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_X, false, 0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Y, false, 0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter xNormal = new UnknownParameter(ParameterType.VECTOR_X, false, 0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yNormal = new UnknownParameter(ParameterType.VECTOR_Y, false, 0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter positiveOne = new UnknownParameter(ParameterType.CONSTANT, true, 1.0, false, ProcessingType.FIXED);
			UnknownParameter negativeOne = new UnknownParameter(ParameterType.CONSTANT, true, -1.0, false, ProcessingType.FIXED);

			ProductSumRestriction xOriginRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {ux}, new List<UnknownParameter> {du}, xOrigin);
			ProductSumRestriction yOriginRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {uy}, new List<UnknownParameter> {du}, yOrigin);

			ProductSumRestriction xNormalRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {uy}, new List<UnknownParameter> {positiveOne}, xNormal);
			ProductSumRestriction yNormalRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {ux}, new List<UnknownParameter> {negativeOne}, yNormal);

			this.add(this.line);

			IList<UnknownParameter> newOrderedUnknownParameters = new List<UnknownParameter>();
			newOrderedUnknownParameters.Add(xOrigin);
			newOrderedUnknownParameters.Add(yOrigin);

			newOrderedUnknownParameters.Add(xNormal);
			newOrderedUnknownParameters.Add(yNormal);

			((List<UnknownParameter>)newOrderedUnknownParameters).AddRange(this.UnknownParameters);

			newOrderedUnknownParameters.Add(positiveOne);
			newOrderedUnknownParameters.Add(negativeOne);

			this.UnknownParameters.setAll(newOrderedUnknownParameters);


			this.PostProcessingCalculations.addAll(xOriginRestriction, yOriginRestriction, xNormalRestriction, yNormalRestriction);
		}

		public virtual Line Line
		{
			get
			{
				return this.line;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, LineFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, LineFeature feature)
		{
			deriveInitialGuess(points, feature.line);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.curve.primitive.Line line) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Line line)
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

				if (line.Dimension > point.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + line.Dimension + " != " + point.Dimension);
				}
			}

			if (nop < 2)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 2 points are needed.");
			}

			x0 /= nop;
			y0 /= nop;

			UpperSymmPackMatrix H = new UpperSymmPackMatrix(2);

			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}

				double xi = point.X0 - x0;
				double yi = point.Y0 - y0;

				for (int i = 0; i < 2; i++)
				{
					double hi = 0;

					if (i == 0)
					{
						hi = xi;
					}
					else
					{
						hi = yi;
					}

					for (int j = i; j < 2; j++)
					{
						double hj = 0;

						if (j == 0)
						{
							hj = xi;
						}
						else
						{
							hj = yi;
						}

						H.set(i, j, H.get(i,j) + hi * hj);
					}
				}
			}

			SymmPackEVD evd = new SymmPackEVD(2, true, true);
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

			// Normal vector n of the line is eigenvector which corresponds to the smallest eigenvalue 
			double nx = eigVec.get(0, indexMinEigVal);
			double ny = eigVec.get(1, indexMinEigVal);

			double d = nx * x0 + ny * y0;

			// Change orientation so that d is a positive distance
			if (d < 0)
			{
				nx = -nx;
				ny = -ny;
				d = -d;
			}

			line.setInitialGuess(nx, ny, d, 1.0);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.line.FeaturePoints, this.line);
		}
	}

}