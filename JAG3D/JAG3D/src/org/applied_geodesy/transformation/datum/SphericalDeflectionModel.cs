using System;

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

namespace org.applied_geodesy.transformation.datum
{
	using Reduction = org.applied_geodesy.adjustment.network.observation.reduction.Reduction;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public class SphericalDeflectionModel
	{
		private class GeographicParameters
		{
			private readonly SphericalDeflectionModel outerInstance;

			internal double latitude, longitude, N; //height
			internal GeographicParameters(SphericalDeflectionModel outerInstance, double latitude, double longitude, double height, double N)
			{
				this.outerInstance = outerInstance;
				this.latitude = latitude;
				this.longitude = longitude;
				//this.height = height;
				this.N = N;
			}
		}

		private Reduction reductions;
		private double X0 = 0, Y0 = 0, Z0 = 0, d0 = 0;

		public SphericalDeflectionModel(Reduction reductions)
		{
			this.Reduction = reductions;
		}

		private Reduction Reduction
		{
			set
			{
				this.reductions = value;
    
				double a = this.reductions.Ellipsoid.getMajorAxis();
				double b = this.reductions.Ellipsoid.getMinorAxis();
    
				double[][] R = this.reductions.PrincipalPoint.RotationSequenceXYZtoENU;
				double latitude = this.reductions.PrincipalPoint.Latitude;
				double h0 = this.reductions.PrincipalPoint.Height;
    
				double cLatitude = Math.Cos(latitude);
				double r31 = R[2][0];
				double r32 = R[2][1];
				double r33 = R[2][2];
    
				double c = a * a / b;
				double eta2 = this.reductions.Ellipsoid.getSecondSquaredEccentricity() * cLatitude * cLatitude;
				double V0 = Math.Sqrt(1.0 + eta2);
				double N0 = c / V0;
    
				this.X0 = (N0 + h0) * r31;
				this.Y0 = (N0 + h0) * r32;
				this.Z0 = (Math.Pow(b / a, 2) * N0 + h0) * r33;
				// Abstand vom Ursprung zur Tangentialebene im Fundamentalpunkt
				this.d0 = r31 * this.X0 + r32 * this.Y0 + r33 * this.Z0;
			}
		}

		public virtual Point SphericalDeflections
		{
			set
			{
				int dim = value.Dimension;
				double x = value.X;
				double y = value.Y;
				double z = dim == 2 ? 0.0 : value.Z;
    
				double x0 = this.reductions.PrincipalPoint.X;
				double y0 = this.reductions.PrincipalPoint.Y;
				double z0 = this.reductions.PrincipalPoint.Z;
				double[][] R = this.reductions.PrincipalPoint.RotationSequenceXYZtoENU;
    
				double a = this.reductions.Ellipsoid.getMajorAxis();
				double b = this.reductions.Ellipsoid.getMinorAxis();
    
				GeographicParameters geographicParameters = this.getGeographicParameters(y - y0, x - x0, z - z0);
    
				double sLatitude = Math.Sin(geographicParameters.latitude);
				double cLatitude = Math.Cos(geographicParameters.latitude);
    
				double sLongitude = Math.Sin(geographicParameters.longitude);
				double cLongitude = Math.Cos(geographicParameters.longitude);
    
				// Hofmann-Wellenhof et al. 1994, Gl (3.11)
				double sx = cLatitude * cLongitude;
				double sy = cLatitude * sLongitude;
				double sz = sLatitude;
    
				double r11 = R[0][0];
				double r12 = R[0][1];
				double r13 = R[0][2];
    
				double r21 = R[1][0];
				double r22 = R[1][1];
				double r23 = R[1][2];
    
				double r31 = R[2][0];
				double r32 = R[2][1];
				double r33 = R[2][2];
    
				double dy = r11 * sx + r12 * sy + r13 * sz;
				double dx = r21 * sx + r22 * sy + r23 * sz;
				double dz = r31 * sx + r32 * sy + r33 * sz;
    
				// Approx. fuer kleine Netze
		//		double rx =   r11 * sx + r12 * sy + r13 * sz;
		//		double ry = -(r21 * sx + r22 * sy + r23 * sz);
    
				// Sequence Ry*Rx
				double rx = Math.Asin(dy);
				double ry = -Math.Atan2(dx, dz);
    
				// Abstand zw. Ellipsoid und Ebene - Hofmann-Wellenhof et al. 1994, Gl (3.1)
				double surfX = geographicParameters.N * sx;
				double surfY = geographicParameters.N * sy;
				double surfZ = (Math.Pow(b / a, 2) * geographicParameters.N) * sz;
    
		//	    // Abstand zwischen Punkt auf Ellipsoid und Ebene; entlang des geoz. Richtungsvektors des Punktes
		//	    double h = (this.d0 - r31 * surfX - r32 * surfY - r33 * surfZ) / (r31 * sx + r32 * sy + r33 * sz);
				// Abstand zwischen Punkt auf Ellipsoid und Ebene; entlang der Normalen des tangentialen Systems
				double h = (this.d0 - r31 * surfX - r32 * surfY - r33 * surfZ);
    
				value.SphericalDeflectionParameter.setSphericalDeflectionParameter(rx, ry, h);
			}
		}

		/// <summary>
		/// Bestimmt aus den kartesitschen Koordinaten east/north die zugehoerigen
		/// geographischen Koordinaten, die ellips. Hoehe und den Normalkruemmungsradius </summary>
		/// <param name="east"> </param>
		/// <param name="north"> </param>
		/// <param name="up">
		/// @return </param>
		private GeographicParameters getGeographicParameters(double east, double north, double up)
		{
			double a = this.reductions.Ellipsoid.getMajorAxis();
			double b = this.reductions.Ellipsoid.getMinorAxis();
			double e1 = this.reductions.Ellipsoid.getFirstSquaredEccentricity();
			double e2 = this.reductions.Ellipsoid.getSecondSquaredEccentricity();

			double[][] R = this.reductions.PrincipalPoint.RotationSequenceXYZtoENU;
			double r11 = R[0][0];
			double r12 = R[0][1];
			double r13 = R[0][2];

			double r21 = R[1][0];
			double r22 = R[1][1];
			double r23 = R[1][2];

			double r31 = R[2][0];
			double r32 = R[2][1];
			double r33 = R[2][2];

			// global XYZ from local ENU -> XYZ = P0 * R'*ENU - Hofmann-Wellenhof et al. (1994), S. 32f
			double X = this.X0 + r11 * east + r21 * north + r31 * up;
			double Y = this.Y0 + r12 * east + r22 * north + r32 * up;
			double Z = this.Z0 + r13 * east + r23 * north + r33 * up;

			double c = a * a / b;
			double p = Math.hypot(X, Y);
			double theta = Math.Atan2(Z * a, p * b);

			double longitude = Math.Atan2(Y, X);

			double latitude = Math.Atan2(Z + e2 * b * Math.Pow(Math.Sin(theta), 3), p - e1 * a * Math.Pow(Math.Cos(theta), 3));
			double cLatitude = Math.Cos(latitude);

			double V = Math.Sqrt(1.0 + e2 * cLatitude * cLatitude);
			double N = c / V;

			double h = p / cLatitude - N;

			return new GeographicParameters(this, latitude, longitude, h, N);
		}
	}

}