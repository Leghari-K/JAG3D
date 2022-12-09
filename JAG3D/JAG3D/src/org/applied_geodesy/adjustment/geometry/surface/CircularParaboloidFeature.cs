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

	using Quaternion = org.applied_geodesy.adjustment.geometry.Quaternion;
	using SurfaceFeature = org.applied_geodesy.adjustment.geometry.SurfaceFeature;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using AverageRestriction = org.applied_geodesy.adjustment.geometry.restriction.AverageRestriction;
	using Paraboloid = org.applied_geodesy.adjustment.geometry.surface.primitive.Paraboloid;
	using Plane = org.applied_geodesy.adjustment.geometry.surface.primitive.Plane;
	using Sphere = org.applied_geodesy.adjustment.geometry.surface.primitive.Sphere;

	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SVD = no.uib.cipr.matrix.SVD;

	public class CircularParaboloidFeature : SurfaceFeature
	{

		private readonly Paraboloid paraboloid;

		public CircularParaboloidFeature() : base(true)
		{

			this.paraboloid = new Paraboloid();

			UnknownParameter A = this.paraboloid.getUnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT);
			UnknownParameter C = this.paraboloid.getUnknownParameter(ParameterType.MINOR_AXIS_COEFFICIENT);

			UnknownParameter R21 = this.paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R21);

			A.Visible = false;
			C.Visible = false;
			R21.ProcessingType = ProcessingType.FIXED;

			UnknownParameter B = new UnknownParameter(ParameterType.MIDDLE_AXIS_COEFFICIENT, true, 0.0, true, ProcessingType.POSTPROCESSING);
			AverageRestriction AequalsBRestriction = new AverageRestriction(true, new List<UnknownParameter> {A}, C);
			AverageRestriction radiusRestriction = new AverageRestriction(true, new List<UnknownParameter> {A, C}, B);

			this.add(this.paraboloid);

			this.UnknownParameters.add(B);
			this.Restrictions.add(AequalsBRestriction);
			this.PostProcessingCalculations.add(radiusRestriction);
		}

		public virtual Paraboloid Paraboloid
		{
			get
			{
				return this.paraboloid;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, CircularParaboloidFeature feature) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, CircularParaboloidFeature feature)
		{
			deriveInitialGuess(points, feature.paraboloid);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void deriveInitialGuess(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Paraboloid paraboloid) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public static void deriveInitialGuess(ICollection<FeaturePoint> points, Paraboloid paraboloid)
		{
			int nop = 0;

			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}

				nop++;
				if (paraboloid.Dimension > point.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + paraboloid.Dimension + " != " + point.Dimension);
				}
			}

			if (nop < 6)
			{
				throw new System.ArgumentException("Error, the number of points is not sufficient; at least 6 points are needed.");
			}

			// derive initial guess
			if (nop > 8)
			{
				ParaboloidFeature.deriveInitialGuess(points, paraboloid);
				UnknownParameter A = paraboloid.getUnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT);
				UnknownParameter C = paraboloid.getUnknownParameter(ParameterType.MINOR_AXIS_COEFFICIENT);

				UnknownParameter R11 = paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R11);
				UnknownParameter R12 = paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R12);
				UnknownParameter R13 = paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R13);

				UnknownParameter R21 = paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R21);
				UnknownParameter R22 = paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R22);
				UnknownParameter R23 = paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R23);

				UnknownParameter R31 = paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R31);
				UnknownParameter R32 = paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R32);
				UnknownParameter R33 = paraboloid.getUnknownParameter(ParameterType.ROTATION_COMPONENT_R33);

				double b = 0.5 * (A.Value0 + C.Value0);

				double rx = Math.Atan2(R32.Value0, R33.Value0);
				double ry = Math.Atan2(-R31.Value0, Math.hypot(R32.Value0, R33.Value0));

				// derive rotation sequence without rz, i.e., rz = 0 --> R = Ry*Rx
				double r11 = Math.Cos(ry);
				double r12 = Math.Sin(rx) * Math.Sin(ry);
				double r13 = Math.Cos(rx) * Math.Sin(ry);

				double r21 = 0.0;
				double r22 = Math.Cos(rx);
				double r23 = -Math.Sin(rx);

				double r31 = -Math.Sin(ry);
				double r32 = Math.Sin(rx) * Math.Cos(ry);
				double r33 = Math.Cos(rx) * Math.Cos(ry);

				A.Value0 = b;
				C.Value0 = b;

				R11.Value0 = r11;
				R12.Value0 = r12;
				R13.Value0 = r13;

				R21.Value0 = r21;
				R22.Value0 = r22;
				R23.Value0 = r23;

				R31.Value0 = r31;
				R32.Value0 = r32;
				R33.Value0 = r33;
			}

			else
			{
				deriveInitialGuessByCircle(points, paraboloid);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			deriveInitialGuess(this.paraboloid.FeaturePoints, this.paraboloid);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static void deriveInitialGuessByCircle(java.util.Collection<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> points, org.applied_geodesy.adjustment.geometry.surface.primitive.Paraboloid paraboloid) throws IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		private static void deriveInitialGuessByCircle(ICollection<FeaturePoint> points, Paraboloid paraboloid)
		{
			Sphere sphere = new Sphere();
			Plane plane = new Plane();
			SpatialCircleFeature.deriveInitialGuess(points, sphere, plane);

			double x0 = sphere.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_X).Value0;
			double y0 = sphere.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_Y).Value0;
			double z0 = sphere.getUnknownParameter(ParameterType.ORIGIN_COORDINATE_Z).Value0;

			double wx = plane.getUnknownParameter(ParameterType.VECTOR_X).Value0;
			double wy = plane.getUnknownParameter(ParameterType.VECTOR_Y).Value0;
			double wz = plane.getUnknownParameter(ParameterType.VECTOR_Z).Value0;

			DenseMatrix w = new DenseMatrix(1, 3, new double[] {wx, wy, wz}, true);
			SVD svd = SVD.factorize(w);
			Matrix V = svd.getVt();

			double ux = V.get(0, 1);
			double uy = V.get(1, 1);
			double uz = V.get(2, 1);

			double vx = V.get(0, 2);
			double vy = V.get(1, 2);
			double vz = V.get(2, 2);

			double det = ux * vy * wz + vx * wy * uz + wx * uy * vz - wx * vy * uz - vx * uy * wz - ux * wy * vz;
			if (det < 0)
			{
				wx = -wx;
				wy = -wy;
				wz = -wz;
			}

			double rx = Math.Atan2(wy, wz);
			double ry = Math.Atan2(-wx, Math.hypot(wy, wz));

			// derive rotation sequence without rz, i.e., rz = 0 --> R = Ry*Rx
			ux = Math.Cos(ry);
			uy = Math.Sin(rx) * Math.Sin(ry);
			uz = Math.Cos(rx) * Math.Sin(ry);

			vx = 0.0;
			vy = Math.Cos(rx);
			vz = -Math.Sin(rx);

			wx = -Math.Sin(ry);
			wy = Math.Sin(rx) * Math.Cos(ry);
			wz = Math.Cos(rx) * Math.Cos(ry);

			Quaternion q = FeatureUtil.getQuaternionHz(new double[] {wx, wy, wz});
			ICollection<FeaturePoint> rotatedPoints = FeatureUtil.getRotatedFeaturePoints(points, new double[] {0, 0, 0}, q);
			Quaternion rotatedApexQ = q.rotate(new double[] {x0, y0, z0});
			double rx0 = rotatedApexQ.Q1;
			double ry0 = rotatedApexQ.Q2;
			double rz0 = rotatedApexQ.Q3;
			double r = 0;
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

				r += Math.Abs(dz / (dx * dx + dy * dy));
			}
			r = Math.Sqrt(r / nop);
			paraboloid.setInitialGuess(x0, y0, z0, r, r, ux, uy, uz, vx, vy, vz, wx, wy, wz);
		}
	}
}