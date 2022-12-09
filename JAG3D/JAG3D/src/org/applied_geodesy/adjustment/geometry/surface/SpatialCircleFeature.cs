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
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using ProductSumRestriction = org.applied_geodesy.adjustment.geometry.restriction.ProductSumRestriction;
	using Plane = org.applied_geodesy.adjustment.geometry.surface.primitive.Plane;
	using Sphere = org.applied_geodesy.adjustment.geometry.surface.primitive.Sphere;

	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;

	public class SpatialCircleFeature : SurfaceFeature
	{
		private readonly Sphere sphere;
		private readonly Plane plane;

		public SpatialCircleFeature() : base(true)
		{

			this.sphere = new Sphere();
			this.plane = new Plane();

			UnknownParameter nx = this.plane.getUnknownParameter(ParameterType.VECTOR_X);
			UnknownParameter ny = this.plane.getUnknownParameter(ParameterType.VECTOR_Y);
			UnknownParameter nz = this.plane.getUnknownParameter(ParameterType.VECTOR_Z);
			UnknownParameter d = this.plane.getUnknownParameter(ParameterType.LENGTH);
			UnknownParameter n = this.plane.getUnknownParameter(ParameterType.VECTOR_LENGTH);

			UnknownParameter x0 = this.sphere.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_X);
			UnknownParameter y0 = this.sphere.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_Y);
			UnknownParameter z0 = this.sphere.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_Z);

			n.Visible = false;

			IList<UnknownParameter> normalVector = new List<UnknownParameter> {nx, ny, nz};
			IList<UnknownParameter> centerPoint = new List<UnknownParameter> {x0, y0, z0};

			ProductSumRestriction centerInPlaneRestriction = new ProductSumRestriction(true, normalVector, centerPoint, d);

			this.add(this.sphere);
			this.add(this.plane);

			this.Restrictions.add(centerInPlaneRestriction);
		}

		public virtual Plane Plane
		{
			get
			{
				return this.plane;
			}
		}

		public virtual Sphere Sphere
		{
			get
			{
				return this.sphere;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, SpatialCircleFeature feature) throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, SpatialCircleFeature feature)
		{
			deriveInitialGuess(points, feature.sphere, feature.plane);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Sphere sphere, org.applied_geodesy.adjustment.geometry.surface.primitive.Plane plane) throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Sphere sphere, Plane plane)
		{
			// estimate plane of the circle
			PlaneFeature.deriveInitialGuess(points, plane);
			double nx = plane.getUnknownParameter(ParameterType.VECTOR_X).Value0;
			double ny = plane.getUnknownParameter(ParameterType.VECTOR_Y).Value0;
			double nz = plane.getUnknownParameter(ParameterType.VECTOR_Z).Value0;
			double d = plane.getUnknownParameter(ParameterType.LENGTH).Value0;

			Quaternion q = FeatureUtil.getQuaternionHz(new double[] {nx, ny, nz});
			ICollection<FeaturePoint> points2D = FeatureUtil.getRotatedFeaturePoints(points, new double[] {0, 0, 0}, q);

			// estimate circle parameters
			Circle circle = new Circle();
			CircleFeature.deriveInitialGuess(points2D, circle);

			double x0 = circle.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_X).Value0;
			double y0 = circle.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_Y).Value0;
			double r0 = circle.getUnknownParameter(ParameterType.RADIUS).Value0;

			double[] horizontalCircleCenter = new double[] {x0, y0, d};
			Quaternion cq = q.conj();
			Quaternion qR = cq.rotate(horizontalCircleCenter);

			x0 = qR.Q1;
			y0 = qR.Q2;
			double z0 = qR.Q3;

			sphere.setInitialGuess(x0, y0, z0, r0);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			HashSet<FeaturePoint> uniquePointSet = new HashSet<FeaturePoint>();
			uniquePointSet.addAll(this.sphere.FeaturePoints);
			uniquePointSet.addAll(this.plane.FeaturePoints);
			deriveInitialGuess(uniquePointSet, this.sphere, this.plane);
		}
	}

}