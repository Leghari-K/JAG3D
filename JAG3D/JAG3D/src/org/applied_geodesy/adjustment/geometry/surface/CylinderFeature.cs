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

	using Quaternion = org.applied_geodesy.adjustment.geometry.Quaternion;
	using SurfaceFeature = org.applied_geodesy.adjustment.geometry.SurfaceFeature;
	using EllipseFeature = org.applied_geodesy.adjustment.geometry.curve.EllipseFeature;
	using Ellipse = org.applied_geodesy.adjustment.geometry.curve.primitive.Ellipse;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using AverageRestriction = org.applied_geodesy.adjustment.geometry.restriction.AverageRestriction;
	using ProductSumRestriction = org.applied_geodesy.adjustment.geometry.restriction.ProductSumRestriction;
	using Cylinder = org.applied_geodesy.adjustment.geometry.surface.primitive.Cylinder;

	using Pair = javafx.util.Pair;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SymmPackEVD = no.uib.cipr.matrix.SymmPackEVD;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class CylinderFeature : SurfaceFeature
	{

		private readonly Cylinder cylinder;

		public CylinderFeature() : base(true)
		{

			this.cylinder = new Cylinder();

			UnknownParameter xFocal1 = this.cylinder.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_X);
			UnknownParameter yFocal1 = this.cylinder.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Y);
			UnknownParameter zFocal1 = this.cylinder.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Z);

			UnknownParameter xFocal2 = this.cylinder.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_X);
			UnknownParameter yFocal2 = this.cylinder.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Y);
			UnknownParameter zFocal2 = this.cylinder.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Z);

			UnknownParameter majorAxis = this.cylinder.getUnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT);
			UnknownParameter vectorLength = this.cylinder.getUnknownParameter(ParameterType.VECTOR_LENGTH);

			vectorLength.Visible = false;

			UnknownParameter xOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_X, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Y, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter zOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Z, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter minorAxis = new UnknownParameter(ParameterType.MINOR_AXIS_COEFFICIENT, false, 0.0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter xEccentricity = new UnknownParameter(ParameterType.VECTOR_X, false, 0.0, false, ProcessingType.POSTPROCESSING);
			UnknownParameter yEccentricity = new UnknownParameter(ParameterType.VECTOR_Y, false, 0.0, false, ProcessingType.POSTPROCESSING);
			UnknownParameter zEccentricity = new UnknownParameter(ParameterType.VECTOR_Z, false, 0.0, false, ProcessingType.POSTPROCESSING);
			UnknownParameter eccentricity = new UnknownParameter(ParameterType.LENGTH, false, 0.0, false, ProcessingType.POSTPROCESSING);

			UnknownParameter oneHalf = new UnknownParameter(ParameterType.CONSTANT, false, 0.5, false, ProcessingType.FIXED);

			AverageRestriction xOriginRestriction = new AverageRestriction(false, new List<UnknownParameter> {xFocal1, xFocal2}, xOrigin);
			AverageRestriction yOriginRestriction = new AverageRestriction(false, new List<UnknownParameter> {yFocal1, yFocal2}, yOrigin);
			AverageRestriction zOriginRestriction = new AverageRestriction(false, new List<UnknownParameter> {zFocal1, zFocal2}, zOrigin);

			ProductSumRestriction xEccentricityRestriction = new ProductSumRestriction(false, List.of(xFocal1, xFocal2), List.of(oneHalf, oneHalf), false, xEccentricity);
			ProductSumRestriction yEccentricityRestriction = new ProductSumRestriction(false, List.of(yFocal1, yFocal2), List.of(oneHalf, oneHalf), false, yEccentricity);
			ProductSumRestriction zEccentricityRestriction = new ProductSumRestriction(false, List.of(zFocal1, zFocal2), List.of(oneHalf, oneHalf), false, zEccentricity);

			IList<UnknownParameter> eccentricityVector = new List<UnknownParameter> {xEccentricity, yEccentricity, zEccentricity};
			ProductSumRestriction eccentricityRestriction = new ProductSumRestriction(false, eccentricityVector, eccentricityVector, 0.5, eccentricity);
			ProductSumRestriction minorAxisRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {majorAxis, eccentricity}, new List<UnknownParameter> {majorAxis, eccentricity}, 0.5, false, minorAxis);

			this.add(this.cylinder);

			IList<UnknownParameter> newOrderedUnknownParameters = new List<UnknownParameter>();
			newOrderedUnknownParameters.Add(xOrigin);
			newOrderedUnknownParameters.Add(yOrigin);
			newOrderedUnknownParameters.Add(zOrigin);

			((List<UnknownParameter>)newOrderedUnknownParameters).AddRange(this.UnknownParameters);
			newOrderedUnknownParameters.Add(minorAxis);

			newOrderedUnknownParameters.Add(eccentricity);
			newOrderedUnknownParameters.Add(xEccentricity);
			newOrderedUnknownParameters.Add(yEccentricity);
			newOrderedUnknownParameters.Add(zEccentricity);
	//		
			newOrderedUnknownParameters.Add(oneHalf);

			this.UnknownParameters.setAll(newOrderedUnknownParameters);

			this.PostProcessingCalculations.addAll(xOriginRestriction, yOriginRestriction, zOriginRestriction, xEccentricityRestriction, yEccentricityRestriction, zEccentricityRestriction, eccentricityRestriction, minorAxisRestriction);
		}

		public virtual Cylinder Cylinder
		{
			get
			{
				return this.cylinder;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, CylinderFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, CylinderFeature feature)
		{
			deriveInitialGuess(points, feature.cylinder);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Cylinder cylinder) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Cylinder cylinder)
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
				if (cylinder.Dimension > point.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + cylinder.Dimension + " != " + point.Dimension);
				}
			}

			if (nop < 7)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 7 points are needed.");
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

			Matrix evec = evd.getEigenvectors();

			// rotate points to xy-plane to derive axis of cylinder
			ICollection<FeaturePoint> rotatedPoints;

			Quaternion q1 = FeatureUtil.getQuaternionHz(new double[] {evec.get(0,0), evec.get(1,0), evec.get(2,0)});
			Quaternion q2 = FeatureUtil.getQuaternionHz(new double[] {evec.get(0,1), evec.get(1,1), evec.get(2,1)});
			Quaternion q3 = FeatureUtil.getQuaternionHz(new double[] {evec.get(0,2), evec.get(1,2), evec.get(2,2)});

			rotatedPoints = FeatureUtil.getRotatedFeaturePoints(points, new double[] {x0, y0, z0}, q1);
			Pair<double, Ellipse> approximation1 = getEllipse(rotatedPoints);

			rotatedPoints = FeatureUtil.getRotatedFeaturePoints(points, new double[] {x0, y0, z0}, q2);
			Pair<double, Ellipse> approximation2 = getEllipse(rotatedPoints);

			rotatedPoints = FeatureUtil.getRotatedFeaturePoints(points, new double[] {x0, y0, z0}, q3);
			Pair<double, Ellipse> approximation3 = getEllipse(rotatedPoints);

			rotatedPoints.Clear();
			rotatedPoints = null;

			Quaternion cq;
			Pair<double, Ellipse> bestApproximation;
			double[] principleAxis;
			// first principle axis == main axis
			if (approximation1.getKey() < approximation2.getKey() && approximation1.getKey() < approximation3.getKey())
			{
				cq = q1.conj();
				principleAxis = new double[] {evec.get(0,0), evec.get(1,0), evec.get(2,0)};
				bestApproximation = approximation1;
			}
			// second principle axis == main axis
			else if (approximation2.getKey() < approximation1.getKey() && approximation2.getKey() < approximation3.getKey())
			{
				cq = q2.conj();
				principleAxis = new double[] {evec.get(0,1), evec.get(1,1), evec.get(2,1)};
				bestApproximation = approximation2;
			}
			// third principle axis == main axis
			else
			{ // if (approximation3.getKey() < approximation1.getKey() && approximation3.getKey() < approximation2.getKey())
				cq = q3.conj();
				principleAxis = new double[] {evec.get(0,2), evec.get(1,2), evec.get(2,2)};
				bestApproximation = approximation3;
			}

			q1 = q2 = q3 = null;
			approximation1 = approximation2 = approximation3 = null;

			// rotate the focal points back to the spatial case 
			Ellipse ellipse = bestApproximation.getValue();
			double x1 = ellipse.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_X).Value0;
			double y1 = ellipse.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Y).Value0;
			double z1 = 0.0;
			double x2 = ellipse.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_X).Value0;
			double y2 = ellipse.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Y).Value0;
			double z2 = 0.0;
			double a = ellipse.getUnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT).Value0;

			Quaternion qf1 = cq.rotate(new double[] {x1, y1, z1});
			double[] f13d = new double[] {qf1.Q1 + x0, qf1.Q2 + y0, qf1.Q3 + z0};
			Quaternion qf2 = cq.rotate(new double[] {x2, y2, z2});
			double[] f23d = new double[] {qf2.Q1 + x0, qf2.Q2 + y0, qf2.Q3 + z0};

			// estimate focal points which are closest to the origin
			double s1 = 0, s2 = 0, tmp = 0;
			for (int i = 0; i < 3; i++)
			{
				s1 += principleAxis[i] * f13d[i];
				s2 += principleAxis[i] * f23d[i];
				tmp += principleAxis[i] * principleAxis[i];
			}

			// scale parameter
			s1 = -s1 / tmp;
			s2 = -s2 / tmp;

			double nx = principleAxis[0];
			double ny = principleAxis[1];
			double nz = principleAxis[2];

			x1 = f13d[0] + s1 * nx;
			y1 = f13d[1] + s1 * ny;
			z1 = f13d[2] + s1 * nz;

			x2 = f23d[0] + s2 * nx;
			y2 = f23d[1] + s2 * ny;
			z2 = f23d[2] + s2 * nz;

			cylinder.setInitialGuess(x1, y1, z1, x2, y2, z2, nx, ny, nz, a);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.cylinder.FeaturePoints, this.cylinder);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static javafx.util.Pair<double, org.applied_geodesy.adjustment.geometry.curve.primitive.Ellipse> getEllipse(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		private static Pair<double, Ellipse> getEllipse(ICollection<FeaturePoint> points)
		{
			Ellipse ellipse = new Ellipse();
			EllipseFeature.deriveInitialGuess(points, ellipse);

			ICollection<UnknownParameter> parameters = ellipse.UnknownParameters;
			foreach (UnknownParameter parameter in parameters)
			{
				parameter.Value = parameter.Value0;
			}

			double epsilon = 0.0;
			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}
				double res = ellipse.getMisclosure(point);
				epsilon += res * res;
			}

			return new Pair<double, Ellipse>(epsilon, ellipse);
		}
	}

}