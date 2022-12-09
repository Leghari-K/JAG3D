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
	using CircleFeature = org.applied_geodesy.adjustment.geometry.curve.CircleFeature;
	using Circle = org.applied_geodesy.adjustment.geometry.curve.primitive.Circle;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using AverageRestriction = org.applied_geodesy.adjustment.geometry.restriction.AverageRestriction;
	using Cylinder = org.applied_geodesy.adjustment.geometry.surface.primitive.Cylinder;

	using Pair = javafx.util.Pair;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SymmPackEVD = no.uib.cipr.matrix.SymmPackEVD;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class CircularCylinderFeature : SurfaceFeature
	{

		private readonly Cylinder cylinder;

		public CircularCylinderFeature() : base(true)
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

			xFocal1.Visible = false;
			yFocal1.Visible = false;
			zFocal1.Visible = false;

			xFocal2.Visible = false;
			yFocal2.Visible = false;
			zFocal2.Visible = false;

			majorAxis.Visible = false;
			vectorLength.Visible = false;

			UnknownParameter xOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_X, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Y, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter zOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Z, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter radius = new UnknownParameter(ParameterType.RADIUS, false, 0.0, true, ProcessingType.POSTPROCESSING);

			// F1 == F1 (condition on z-component is already full-filled, cf. 
			// Loesler, M.: Modellierung und Bestimmung eines elliptischen Zylinders. 
			// avn, Vol. 127(2), S. 87-93, 2020.
			AverageRestriction xFocalRestriction = new AverageRestriction(true, new List<UnknownParameter> {xFocal1}, xFocal2);
			AverageRestriction yFocalRestriction = new AverageRestriction(true, new List<UnknownParameter> {yFocal1}, yFocal2);

			// origin is identical to F1 and F2
			AverageRestriction xOriginRestriction = new AverageRestriction(false, new List<UnknownParameter> {xFocal1, xFocal2}, xOrigin);
			AverageRestriction yOriginRestriction = new AverageRestriction(false, new List<UnknownParameter> {yFocal1, yFocal2}, yOrigin);
			AverageRestriction zOriginRestriction = new AverageRestriction(false, new List<UnknownParameter> {zFocal1, zFocal2}, zOrigin);
			AverageRestriction radiusRestriction = new AverageRestriction(false, new List<UnknownParameter> {majorAxis}, radius);

			this.add(this.cylinder);

			IList<UnknownParameter> newOrderedUnknownParameters = new List<UnknownParameter>();
			newOrderedUnknownParameters.Add(xOrigin);
			newOrderedUnknownParameters.Add(yOrigin);
			newOrderedUnknownParameters.Add(zOrigin);

			((List<UnknownParameter>)newOrderedUnknownParameters).AddRange(this.UnknownParameters);
			newOrderedUnknownParameters.Add(radius);

			this.UnknownParameters.setAll(newOrderedUnknownParameters);


			this.Restrictions.addAll(xFocalRestriction, yFocalRestriction);

			this.PostProcessingCalculations.addAll(xOriginRestriction, yOriginRestriction, zOriginRestriction, radiusRestriction);
		}

		public virtual Cylinder Cylinder
		{
			get
			{
				return this.cylinder;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, CircularCylinderFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, CircularCylinderFeature feature)
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

			if (nop < 5)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 5 points are needed.");
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
			Pair<double, Circle> approximation1 = getCircle(rotatedPoints);

			rotatedPoints = FeatureUtil.getRotatedFeaturePoints(points, new double[] {x0, y0, z0}, q2);
			Pair<double, Circle> approximation2 = getCircle(rotatedPoints);

			rotatedPoints = FeatureUtil.getRotatedFeaturePoints(points, new double[] {x0, y0, z0}, q3);
			Pair<double, Circle> approximation3 = getCircle(rotatedPoints);

			rotatedPoints.Clear();
			rotatedPoints = null;

			Quaternion cq;
			Pair<double, Circle> bestApproximation;
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
			Circle circle = bestApproximation.getValue();
			double x1 = circle.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_X).Value0;
			double y1 = circle.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_Y).Value0;
			double z1 = 0.0;
			double r = circle.getUnknownParameter(ParameterType.RADIUS).Value0;

			Quaternion qf1 = cq.rotate(new double[] {x1, y1, z1});
			double[] f13d = new double[] {qf1.Q1 + x0, qf1.Q2 + y0, qf1.Q3 + z0};

			// estimate focal points which are closest to the origin
			double s1 = 0, tmp = 0;
			for (int i = 0; i < 3; i++)
			{
				s1 += principleAxis[i] * f13d[i];
				tmp += principleAxis[i] * principleAxis[i];
			}

			// scale parameter
			s1 = -s1 / tmp;

			double nx = principleAxis[0];
			double ny = principleAxis[1];
			double nz = principleAxis[2];

			x1 = f13d[0] + s1 * nx;
			y1 = f13d[1] + s1 * ny;
			z1 = f13d[2] + s1 * nz;

			cylinder.setInitialGuess(x1, y1, z1, x1, y1, z1, nx, ny, nz, r);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.cylinder.FeaturePoints, this.cylinder);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static javafx.util.Pair<double, org.applied_geodesy.adjustment.geometry.curve.primitive.Circle> getCircle(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		private static Pair<double, Circle> getCircle(ICollection<FeaturePoint> points)
		{
			Circle circle = new Circle();
			CircleFeature.deriveInitialGuess(points, circle);

			ICollection<UnknownParameter> parameters = circle.UnknownParameters;
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
				double res = circle.getMisclosure(point);
				epsilon += res * res;
			}

			return new Pair<double, Circle>(epsilon, circle);
		}


	}

}