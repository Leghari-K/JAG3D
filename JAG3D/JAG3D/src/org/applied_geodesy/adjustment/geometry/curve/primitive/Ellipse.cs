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

namespace org.applied_geodesy.adjustment.geometry.curve.primitive
{

	using PrimitiveType = org.applied_geodesy.adjustment.geometry.PrimitiveType;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Point = org.applied_geodesy.adjustment.geometry.point.Point;

	using Matrix = no.uib.cipr.matrix.Matrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class Ellipse : Curve
	{

		private IDictionary<ParameterType, UnknownParameter> parameters = null;

		public Ellipse()
		{
			this.init();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setInitialGuess(double x1, double y1, double x2, double y2, double a) throws IllegalArgumentException
		public virtual void setInitialGuess(double x1, double y1, double x2, double y2, double a)
		{
			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X].Value0 = x1;
			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y].Value0 = y1;

			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X].Value0 = x2;
			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y].Value0 = y2;

			this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT].Value0 = a;
		}

		public override void jacobianElements(FeaturePoint point, Matrix Jx, Matrix Jv, int rowIndex)
		{
			Point centerOfMass = this.CenterOfMass;

			//Schwerpunktreduktion
			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;

			UnknownParameter x1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X];
			UnknownParameter y1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y];

			UnknownParameter x2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X];
			UnknownParameter y2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y];

			double dx1 = x1.Value - xi;
			double dy1 = y1.Value - yi;

			double dx2 = x2.Value - xi;
			double dy2 = y2.Value - yi;

			double s1 = Math.hypot(dx1, dy1);
			double s2 = Math.hypot(dx2, dy2);

			if (Jx != null)
			{
				UnknownParameter a = this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT];

				if (x1.Column >= 0)
				{
					Jx.set(rowIndex, x1.Column, dx1 / s1);
				}
				if (y1.Column >= 0)
				{
					Jx.set(rowIndex, y1.Column, dy1 / s1);
				}
				if (x2.Column >= 0)
				{
					Jx.set(rowIndex, x2.Column, dx2 / s2);
				}
				if (y2.Column >= 0)
				{
					Jx.set(rowIndex, y2.Column, dy2 / s2);
				}
				if (a.Column >= 0)
				{
					Jx.set(rowIndex, a.Column, -2.0);
				}
			}

			if (Jv != null)
			{
				Jv.set(rowIndex, 0, -dx1 / s1 - dx2 / s2);
				Jv.set(rowIndex, 1, -dy1 / s1 - dy2 / s2);
			}
		}

		public override double getMisclosure(FeaturePoint point)
		{
			Point centerOfMass = this.CenterOfMass;

			double xi = point.X - centerOfMass.X0;
			double yi = point.Y - centerOfMass.Y0;

			double x1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X].Value;
			double y1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y].Value;

			double x2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X].Value;
			double y2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y].Value;

			double a = this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT].Value;

			double s1 = Math.hypot(xi - x1, yi - y1);
			double s2 = Math.hypot(xi - x2, yi - y2);

			return s1 + s2 - 2.0 * a;
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
    
				UnknownParameter x1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X];
				UnknownParameter y1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y];
    
				UnknownParameter x2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X];
				UnknownParameter y2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y];
    
				x1.Value = x1.Value + prevCenterOfMass.X0 - currCenterOfMass.X0;
				y1.Value = y1.Value + prevCenterOfMass.Y0 - currCenterOfMass.Y0;
    
				x2.Value = x2.Value + prevCenterOfMass.X0 - currCenterOfMass.X0;
				y2.Value = y2.Value + prevCenterOfMass.Y0 - currCenterOfMass.Y0;
			}
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

		public override void reverseCenterOfMass(UpperSymmPackMatrix Dp)
		{
			Point centerOfMass = this.CenterOfMass;

			UnknownParameter x1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X];
			UnknownParameter y1 = this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y];

			UnknownParameter x2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X];
			UnknownParameter y2 = this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y];

			x1.Value = x1.Value + centerOfMass.X0;
			y1.Value = y1.Value + centerOfMass.Y0;

			x2.Value = x2.Value + centerOfMass.X0;
			y2.Value = y2.Value + centerOfMass.Y0;
		}

		private void init()
		{
			this.parameters = new LinkedHashMap<ParameterType, UnknownParameter>();

			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_X] = new UnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_X, true);
			this.parameters[ParameterType.PRIMARY_FOCAL_COORDINATE_Y] = new UnknownParameter(ParameterType.PRIMARY_FOCAL_COORDINATE_Y, true);

			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_X] = new UnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_X, true);
			this.parameters[ParameterType.SECONDARY_FOCAL_COORDINATE_Y] = new UnknownParameter(ParameterType.SECONDARY_FOCAL_COORDINATE_Y, true);

			this.parameters[ParameterType.MAJOR_AXIS_COEFFICIENT] = new UnknownParameter(ParameterType.MAJOR_AXIS_COEFFICIENT, true);
		}

		public override PrimitiveType PrimitiveType
		{
			get
			{
				return PrimitiveType.ELLIPSE;
			}
		}

		public override string toLaTex()
		{
			return "$\\sum_{j=1}^2 s_j = 2 a" + " \\\\ " + "s_j = \\vert \\mathbf{P}_i - \\mathbf{F}_j \\vert$";
		}
	}

}