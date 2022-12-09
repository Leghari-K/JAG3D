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
	using Plane = org.applied_geodesy.adjustment.geometry.surface.primitive.Plane;

	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;

	public class SpatialEllipseFeature : SurfaceFeature
	{
		private readonly Cylinder cylinder;
		private readonly Plane plane;

		public SpatialEllipseFeature() : base(true)
		{

			this.cylinder = new Cylinder();
			this.plane = new Plane();

			UnknownParameter ux = this.plane.getUnknownParameter(ParameterType.VECTOR_X);
			UnknownParameter uy = this.plane.getUnknownParameter(ParameterType.VECTOR_Y);
			UnknownParameter uz = this.plane.getUnknownParameter(ParameterType.VECTOR_Z);
			UnknownParameter du = this.plane.getUnknownParameter(ParameterType.LENGTH);
			UnknownParameter u = this.plane.getUnknownParameter(ParameterType.VECTOR_LENGTH);

			UnknownParameter xFocal1 = this.cylinder.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_X);
			UnknownParameter yFocal1 = this.cylinder.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Y);
			UnknownParameter zFocal1 = this.cylinder.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Z);

			UnknownParameter xFocal2 = this.cylinder.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_X);
			UnknownParameter yFocal2 = this.cylinder.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Y);
			UnknownParameter zFocal2 = this.cylinder.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Z);

			UnknownParameter majorAxis = this.cylinder.getUnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT);

			UnknownParameter vx = this.cylinder.getUnknownParameter(ParameterType.VECTOR_X);
			UnknownParameter vy = this.cylinder.getUnknownParameter(ParameterType.VECTOR_Y);
			UnknownParameter vz = this.cylinder.getUnknownParameter(ParameterType.VECTOR_Z);
			UnknownParameter v = this.cylinder.getUnknownParameter(ParameterType.VECTOR_LENGTH);
			v.ProcessingType = ProcessingType.ADJUSTMENT; // because |v| and |u| are a redundant restrictions define by the plane and the cylinder

			xFocal1.Visible = false;
			yFocal1.Visible = false;
			zFocal1.Visible = false;

			xFocal2.Visible = false;
			yFocal2.Visible = false;
			zFocal2.Visible = false;

			vx.Visible = false;
			vy.Visible = false;
			vz.Visible = false;

			v.Visible = false;
			u.Visible = false;

			UnknownParameter xOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_X, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Y, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter zOrigin = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Z, false, 0.0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter xFocal1InPlane = new UnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_X, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yFocal1InPlane = new UnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Y, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter zFocal1InPlane = new UnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Z, false, 0.0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter xFocal2InPlane = new UnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_X, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter yFocal2InPlane = new UnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Y, false, 0.0, true, ProcessingType.POSTPROCESSING);
			UnknownParameter zFocal2InPlane = new UnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Z, false, 0.0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter xEccentricity = new UnknownParameter(ParameterType.VECTOR_X, false, 0.0, false, ProcessingType.POSTPROCESSING);
			UnknownParameter yEccentricity = new UnknownParameter(ParameterType.VECTOR_Y, false, 0.0, false, ProcessingType.POSTPROCESSING);
			UnknownParameter zEccentricity = new UnknownParameter(ParameterType.VECTOR_Z, false, 0.0, false, ProcessingType.POSTPROCESSING);
			UnknownParameter eccentricity = new UnknownParameter(ParameterType.LENGTH, false, 0.0, false, ProcessingType.POSTPROCESSING);

			UnknownParameter minorAxis = new UnknownParameter(ParameterType.MINOR_AXIS_COEFFICIENT, false, 0.0, true, ProcessingType.POSTPROCESSING);

			UnknownParameter one = new UnknownParameter(ParameterType.CONSTANT, false, 1.0, false, ProcessingType.FIXED);
			UnknownParameter oneHalf = new UnknownParameter(ParameterType.CONSTANT, false, 0.5, false, ProcessingType.FIXED);

			// normal vector of the plane is identical to the main axis of the cylinder
			AverageRestriction xNormalRestriction = new AverageRestriction(true, new List<UnknownParameter> {ux}, vx);
			AverageRestriction yNormalRestriction = new AverageRestriction(true, new List<UnknownParameter> {uy}, vy);
			AverageRestriction zNormalRestriction = new AverageRestriction(true, new List<UnknownParameter> {uz}, vz);

			// move focal points to plane
			ProductSumRestriction xFocal1Restriction = new ProductSumRestriction(false, new List<UnknownParameter> {xFocal1, du}, new List<UnknownParameter> {one, ux}, xFocal1InPlane);
			ProductSumRestriction yFocal1Restriction = new ProductSumRestriction(false, new List<UnknownParameter> {yFocal1, du}, new List<UnknownParameter> {one, uy}, yFocal1InPlane);
			ProductSumRestriction zFocal1Restriction = new ProductSumRestriction(false, new List<UnknownParameter> {zFocal1, du}, new List<UnknownParameter> {one, uz}, zFocal1InPlane);

			ProductSumRestriction xFocal2Restriction = new ProductSumRestriction(false, new List<UnknownParameter> {xFocal2, du}, new List<UnknownParameter> {one, ux}, xFocal2InPlane);
			ProductSumRestriction yFocal2Restriction = new ProductSumRestriction(false, new List<UnknownParameter> {yFocal2, du}, new List<UnknownParameter> {one, uy}, yFocal2InPlane);
			ProductSumRestriction zFocal2Restriction = new ProductSumRestriction(false, new List<UnknownParameter> {zFocal2, du}, new List<UnknownParameter> {one, uz}, zFocal2InPlane);

			// derive origin of the ellipse
			ProductSumRestriction xOriginRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {xFocal1, xFocal2, du}, new List<UnknownParameter> {oneHalf, oneHalf, ux}, xOrigin);
			ProductSumRestriction yOriginRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {yFocal1, yFocal2, du}, new List<UnknownParameter> {oneHalf, oneHalf, uy}, yOrigin);
			ProductSumRestriction zOriginRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {zFocal1, zFocal2, du}, new List<UnknownParameter> {oneHalf, oneHalf, uz}, zOrigin);

			// estimate minor semi-axis, i.e., b = sqrt(a*a - e*e)
			ProductSumRestriction xEccentricityRestriction = new ProductSumRestriction(false, List.of(xFocal1, xFocal2), List.of(oneHalf, oneHalf), false, xEccentricity);
			ProductSumRestriction yEccentricityRestriction = new ProductSumRestriction(false, List.of(yFocal1, yFocal2), List.of(oneHalf, oneHalf), false, yEccentricity);
			ProductSumRestriction zEccentricityRestriction = new ProductSumRestriction(false, List.of(zFocal1, zFocal2), List.of(oneHalf, oneHalf), false, zEccentricity);

			IList<UnknownParameter> eccentricityVector = new List<UnknownParameter> {xEccentricity, yEccentricity, zEccentricity};
			ProductSumRestriction eccentricityRestriction = new ProductSumRestriction(false, eccentricityVector, eccentricityVector, 0.5, eccentricity);
			ProductSumRestriction minorAxisRestriction = new ProductSumRestriction(false, new List<UnknownParameter> {majorAxis, eccentricity}, new List<UnknownParameter> {majorAxis, eccentricity}, 0.5, false, minorAxis);


			this.add(this.cylinder);
			this.add(this.plane);

			IList<UnknownParameter> newOrderedUnknownParameters = new List<UnknownParameter>();
			newOrderedUnknownParameters.Add(xOrigin);
			newOrderedUnknownParameters.Add(yOrigin);
			newOrderedUnknownParameters.Add(zOrigin);

			newOrderedUnknownParameters.Add(xFocal1InPlane);
			newOrderedUnknownParameters.Add(yFocal1InPlane);
			newOrderedUnknownParameters.Add(zFocal1InPlane);

			newOrderedUnknownParameters.Add(xFocal2InPlane);
			newOrderedUnknownParameters.Add(yFocal2InPlane);
			newOrderedUnknownParameters.Add(zFocal2InPlane);

			((List<UnknownParameter>)newOrderedUnknownParameters).AddRange(this.UnknownParameters);
			newOrderedUnknownParameters.Add(minorAxis);

			newOrderedUnknownParameters.Add(eccentricity);
			newOrderedUnknownParameters.Add(xEccentricity);
			newOrderedUnknownParameters.Add(yEccentricity);
			newOrderedUnknownParameters.Add(zEccentricity);

			newOrderedUnknownParameters.Add(one);
			newOrderedUnknownParameters.Add(oneHalf);

			this.UnknownParameters.setAll(newOrderedUnknownParameters);

			this.Restrictions.addAll(xNormalRestriction, yNormalRestriction, zNormalRestriction);

			this.PostProcessingCalculations.addAll(xOriginRestriction, yOriginRestriction, zOriginRestriction, xFocal1Restriction, yFocal1Restriction, zFocal1Restriction, xFocal2Restriction, yFocal2Restriction, zFocal2Restriction, xEccentricityRestriction, yEccentricityRestriction, zEccentricityRestriction, eccentricityRestriction, minorAxisRestriction);

		}

		public virtual Plane Plane
		{
			get
			{
				return this.plane;
			}
		}

		public virtual Cylinder Cylinder
		{
			get
			{
				return this.cylinder;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, SpatialEllipseFeature feature) throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, SpatialEllipseFeature feature)
		{
			deriveInitialGuess(points, feature.cylinder, feature.plane);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Cylinder cylinder, org.applied_geodesy.adjustment.geometry.surface.primitive.Plane plane) throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Cylinder cylinder, Plane plane)
		{
			// estimate plane of the circle
			PlaneFeature.deriveInitialGuess(points, plane);
			double nx = plane.getUnknownParameter(ParameterType.VECTOR_X).Value0;
			double ny = plane.getUnknownParameter(ParameterType.VECTOR_Y).Value0;
			double nz = plane.getUnknownParameter(ParameterType.VECTOR_Z).Value0;
			double d = plane.getUnknownParameter(ParameterType.LENGTH).Value0;

			Quaternion q = FeatureUtil.getQuaternionHz(new double[] {nx, ny, nz});
			ICollection<FeaturePoint> rotatedPoints = FeatureUtil.getRotatedFeaturePoints(points, new double[] {0, 0, 0}, q);

			// estimate ellipse parameters
			Ellipse ellipse = new Ellipse();
			EllipseFeature.deriveInitialGuess(rotatedPoints, ellipse);

			double x1 = ellipse.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_X).Value0;
			double y1 = ellipse.getUnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Y).Value0;
			double x2 = ellipse.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_X).Value0;
			double y2 = ellipse.getUnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Y).Value0;
			double a = ellipse.getUnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT).Value0;

			double z1 = d;
			double z2 = d;
			double[] principleAxis = new double[] {nx, ny, nz};

			Quaternion cq = q.conj();
			Quaternion qf1 = cq.rotate(new double[] {x1, y1, z1});
			double[] f13d = new double[] {qf1.Q1, qf1.Q2, qf1.Q3};
			Quaternion qf2 = cq.rotate(new double[] {x2, y2, z2});
			double[] f23d = new double[] {qf2.Q1, qf2.Q2, qf2.Q3};

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
			HashSet<FeaturePoint> uniquePointSet = new HashSet<FeaturePoint>();
			uniquePointSet.addAll(this.cylinder.FeaturePoints);
			uniquePointSet.addAll(this.plane.FeaturePoints);
			deriveInitialGuess(uniquePointSet, this.cylinder, this.plane);
		}
	}

}