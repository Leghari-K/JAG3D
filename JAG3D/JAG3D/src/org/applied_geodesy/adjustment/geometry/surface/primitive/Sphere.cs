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

namespace org.applied_geodesy.adjustment.geometry.surface.primitive
{

	using PrimitiveType = org.applied_geodesy.adjustment.geometry.PrimitiveType;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using Point = org.applied_geodesy.adjustment.geometry.point.Point;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;

	using Matrix = no.uib.cipr.matrix.Matrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;


	public class Sphere : Surface
	{
		private IDictionary<ParameterType, UnknownParameter> parameters;

		public Sphere()
		{
			this.init();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setInitialGuess(double x0, double y0, double z0, double r) throws IllegalArgumentException
		public virtual void setInitialGuess(double x0, double y0, double z0, double r)
		{
			this.parameters[ParameterType.ORIGIN_COORDINATE_X].Value0 = x0;
			this.parameters[ParameterType.ORIGIN_COORDINATE_Y].Value0 = y0;
			this.parameters[ParameterType.ORIGIN_COORDINATE_Z].Value0 = z0;
			this.parameters[ParameterType.RADIUS].Value0 = r;
		}

		public override void jacobianElements(FeaturePoint point, Matrix Jx, Matrix Jv, int rowIndex)
		{
			Point centerOfMass = this.CenterOfMass;

			//Schwerpunktreduktion
			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;
			double zi = point.Z - centerOfMass.Z0;

			// Parameter der Kugel
			UnknownParameter x0 = this.parameters[ParameterType.ORIGIN_COORDINATE_X];
			UnknownParameter y0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Y];
			UnknownParameter z0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Z];

			if (Jx != null)
			{
				UnknownParameter r = this.parameters[ParameterType.RADIUS];
				if (x0.Column >= 0)
				{
					Jx.set(rowIndex, x0.Column, -2.0 * (xi - x0.Value));
				}
				if (y0.Column >= 0)
				{
					Jx.set(rowIndex, y0.Column, -2.0 * (yi - y0.Value));
				}
				if (z0.Column >= 0)
				{
					Jx.set(rowIndex, z0.Column, -2.0 * (zi - z0.Value));
				}
				if (r.Column >= 0)
				{
					Jx.set(rowIndex, r.Column, -2.0 * r.Value);
				}
			}

			if (Jv != null)
			{
				Jv.set(rowIndex, 0, 2.0 * (xi - x0.Value));
				Jv.set(rowIndex, 1, 2.0 * (yi - y0.Value));
				Jv.set(rowIndex, 2, 2.0 * (zi - z0.Value));
			}
		}

		public override double getMisclosure(FeaturePoint point)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;
			double zi = point.Z - centerOfMass.Z0;

			double x0 = this.parameters[ParameterType.ORIGIN_COORDINATE_X].Value;
			double y0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Y].Value;
			double z0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Z].Value;
			double r = this.parameters[ParameterType.RADIUS].Value;

			return ((xi - x0) * (xi - x0) + (yi - y0) * (yi - y0) + (zi - z0) * (zi - z0)) - r * r;
		}

		public override Point CenterOfMass
		{
			set
			{
				// get previous center of mass
				Point prevCenterOfMass = this.CenterOfMass;
    
				// check, if components are equal to previous point to avoid unnecessary operations
				bool equalComponents = value.equalsCoordinateComponents(prevCenterOfMass);
				base.CenterOfMass = value;
				if (equalComponents)
				{
					return;
				}
    
				// get current center of mass
				Point currCenterOfMass = this.CenterOfMass;
    
				UnknownParameter x0 = this.parameters[ParameterType.ORIGIN_COORDINATE_X];
				UnknownParameter y0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Y];
				UnknownParameter z0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Z];
    
				x0.Value = x0.Value + prevCenterOfMass.X0 - currCenterOfMass.X0;
				y0.Value = y0.Value + prevCenterOfMass.Y0 - currCenterOfMass.Y0;
				z0.Value = z0.Value + prevCenterOfMass.Z0 - currCenterOfMass.Z0;
			}
		}

		public override void reverseCenterOfMass(UpperSymmPackMatrix Dp)
		{
			Point centerOfMass = this.CenterOfMass;

			UnknownParameter x0 = this.parameters[ParameterType.ORIGIN_COORDINATE_X];
			UnknownParameter y0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Y];
			UnknownParameter z0 = this.parameters[ParameterType.ORIGIN_COORDINATE_Z];

			x0.Value = x0.Value + centerOfMass.X0;
			y0.Value = y0.Value + centerOfMass.Y0;
			z0.Value = z0.Value + centerOfMass.Z0;
		}

		public override ICollection<UnknownParameter> UnknownParameters
		{
			get
			{
				return this.parameters.Values;
			}
		}

		public override UnknownParameter getUnknownParameter(ParameterType parameterType)
		{
			return this.parameters[parameterType];
		}

		public override bool contains(object @object)
		{
			if (@object == null || !(@object is UnknownParameter))
			{
				return false;
			}
			return this.parameters[((UnknownParameter)@object).ParameterType] == @object;
		}

		private void init()
		{
			this.parameters = new LinkedHashMap<ParameterType, UnknownParameter>();
			this.parameters[ParameterType.ORIGIN_COORDINATE_X] = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_X, true);
			this.parameters[ParameterType.ORIGIN_COORDINATE_Y] = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Y, true);
			this.parameters[ParameterType.ORIGIN_COORDINATE_Z] = new UnknownParameter(ParameterType.ORIGIN_COORDINATE_Z, true);
			this.parameters[ParameterType.RADIUS] = new UnknownParameter(ParameterType.RADIUS, true);
		}

		public override PrimitiveType PrimitiveType
		{
			get
			{
				return PrimitiveType.SPHERE;
			}
		}

		public override string toLaTex()
		{
			return "$\\vert \\mathbf{P}_i - \\mathbf{P}_0 \\vert = r" + " \\\\ " + "\\mathbf{P}_0 = \\left( \\begin{array}{c} x_0 \\\\ y_0 \\\\ z_0 \\end{array} \\right)$";
		}
	}

}