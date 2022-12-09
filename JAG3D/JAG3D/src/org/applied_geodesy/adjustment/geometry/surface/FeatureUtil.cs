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

	using Constant = org.applied_geodesy.adjustment.Constant;
	using Quaternion = org.applied_geodesy.adjustment.geometry.Quaternion;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;

	internal class FeatureUtil
	{

		private FeatureUtil()
		{
		}

		/// <summary>
		/// estimate the Quaternion, which describes a rotation between the principle-axis and the z-axis </summary>
		/// <param name="principleAxis"> </param>
		/// <returns> Quaternion </returns>
		internal static Quaternion getQuaternionHz(double[] principleAxis)
		{
			// rotational axis u = cross(n0, z)
			double[] u = new double[] {+principleAxis[1], -principleAxis[0], 0.0};

			// rotation angle between principle-axis and z-axis
			// phi = acos(nz) 
			double phi = Math.Acos(principleAxis[2]);

			// if geometry is already in canonical representation, i.e., n = [0 0 1]', 
			// no orthogonal base can be derived because cross(n,z) = [0 0 0]'
			// --> an arbitrary base is used
			if (Math.Abs(phi) < Constant.EPS || Math.Abs(Math.Abs(phi) - Math.PI) < Constant.EPS)
			{
				u = new double[] {1.0, 1.0, 0.0};
			}

			return new Quaternion(u, phi);
		}

		/// <summary>
		/// rotates the points w.r.t. to z-axis </summary>
		/// <param name="points"> </param>
		/// <param name="centerOfMass"> </param>
		/// <param name="q"> </param>
		/// <returns> horizontal points </returns>
		internal static ICollection<FeaturePoint> getRotatedFeaturePoints(ICollection<FeaturePoint> points, double[] centerOfMass, Quaternion q)
		{

			ICollection<FeaturePoint> rotatedPoints = new List<FeaturePoint>();
			// rotate points into xy-plane
			foreach (FeaturePoint point in points)
			{
				if (!point.Enable)
				{
					continue;
				}

				double xi = point.X0 - centerOfMass[0];
				double yi = point.Y0 - centerOfMass[1];
				double zi = point.Z0 - centerOfMass[2];

				Quaternion resultQ = q.rotate(new double[] {xi, yi, zi});

				rotatedPoints.Add(new FeaturePoint(point.Name, resultQ.Q1, resultQ.Q2, resultQ.Q3));
			}
			return rotatedPoints;
		}
	}

}