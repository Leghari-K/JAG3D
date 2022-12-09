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
	using Constant = org.applied_geodesy.adjustment.Constant;

	public class Ellipsoid
	{
		private readonly double majorAxis, minorAxis, inverseFlattening, flattening;
		private double firstSquaredEccentricity, secondSquaredEccentricity;
		private bool sphereModel = false;

		public static readonly Ellipsoid SPHERE = Ellipsoid.createEllipsoidFromMinorAxis(Constant.EARTH_RADIUS, Constant.EARTH_RADIUS);
		public static readonly Ellipsoid WGS84 = Ellipsoid.createEllipsoidFromSquaredEccentricity(6378137.0, 0.00669437999013);
		public static readonly Ellipsoid GRS80 = Ellipsoid.createEllipsoidFromSquaredEccentricity(6378137.0, 0.00669438002290);
		public static readonly Ellipsoid BESSEL1941 = Ellipsoid.createEllipsoidFromMinorAxis(Math.Exp(6.8046434637 * Math.Log(10)), Math.Exp(6.8031892839 * Math.Log(10)));
		public static readonly Ellipsoid KRASSOWSKI = Ellipsoid.createEllipsoidFromInverseFlattening(6378245.0, 298.3);
		public static readonly Ellipsoid HAYFORD = Ellipsoid.createEllipsoidFromInverseFlattening(6378388.0, 297.0);

		private Ellipsoid(double majorAxis, double secondEllipsoidParameterValue, SecondEllipsoidParameterType parameterType)
		{
			this.majorAxis = majorAxis;

			switch (parameterType.innerEnumValue)
			{
			case org.applied_geodesy.transformation.datum.SecondEllipsoidParameterType.InnerEnum.SQUARED_ECCENTRICITY:
				this.firstSquaredEccentricity = secondEllipsoidParameterValue;
				this.sphereModel = this.firstSquaredEccentricity == 0;
				this.minorAxis = this.sphereModel ? this.majorAxis : this.majorAxis * Math.Sqrt(1.0 - this.firstSquaredEccentricity);
				this.flattening = this.sphereModel ? 0.0 : 1.0 - Math.Sqrt(1.0 - this.firstSquaredEccentricity);
				this.inverseFlattening = this.sphereModel ? double.PositiveInfinity : 1.0 / (1.0 - Math.Sqrt(1.0 - firstSquaredEccentricity));
				this.secondSquaredEccentricity = this.sphereModel ? 0.0 : this.firstSquaredEccentricity / (1.0 - this.firstSquaredEccentricity);
				break;

			case org.applied_geodesy.transformation.datum.SecondEllipsoidParameterType.InnerEnum.INVERSE_FLATTENING:
				this.inverseFlattening = secondEllipsoidParameterValue;
				this.sphereModel = double.PositiveInfinity == this.inverseFlattening;
				this.flattening = this.sphereModel ? 0.0 : 1.0 / this.inverseFlattening;
				this.minorAxis = this.sphereModel ? this.majorAxis : this.majorAxis - this.majorAxis / this.inverseFlattening;
				this.firstSquaredEccentricity = this.sphereModel ? 0.0 : (2.0 - 1.0 / this.inverseFlattening) / this.inverseFlattening;
				this.secondSquaredEccentricity = this.sphereModel ? 0.0 : this.flattening * (2.0 - this.flattening) / Math.Pow(1.0 - this.flattening, 2);
				break;

			default: // MINOR_AXIS
				this.minorAxis = secondEllipsoidParameterValue;
				this.sphereModel = this.minorAxis == this.majorAxis;
				this.flattening = this.sphereModel ? 0.0 : 1.0 - this.minorAxis / this.majorAxis;
				this.inverseFlattening = this.sphereModel ? double.PositiveInfinity : this.majorAxis / (this.majorAxis - this.minorAxis);
				this.firstSquaredEccentricity = this.sphereModel ? 0.0 : 1.0 - ((this.minorAxis * this.minorAxis) / (this.majorAxis * this.majorAxis));
				this.secondSquaredEccentricity = this.sphereModel ? 0.0 : (this.majorAxis * this.majorAxis - this.minorAxis * this.minorAxis) / (this.minorAxis * this.minorAxis);

				break;
			}
		}

		public virtual bool SphereModel
		{
			get
			{
				return this.sphereModel;
			}
		}

		public static Ellipsoid createEllipsoidFromMinorAxis(double majorAxis, double minorAxis)
		{
			return new Ellipsoid(majorAxis, minorAxis, SecondEllipsoidParameterType.MINOR_AXIS);
		}

		public static Ellipsoid createEllipsoidFromInverseFlattening(double majorAxis, double inverseFlattening)
		{
			return new Ellipsoid(majorAxis, inverseFlattening, SecondEllipsoidParameterType.INVERSE_FLATTENING);
		}

		public static Ellipsoid createEllipsoidFromSquaredEccentricity(double majorAxis, double eccentricity)
		{
			return new Ellipsoid(majorAxis, eccentricity, SecondEllipsoidParameterType.SQUARED_ECCENTRICITY);
		}

		public virtual double getRadiusOfConformalSphere(double latitude)
		{
			if (this.SphereModel)
			{
				return this.majorAxis;
			}

			double sin = Math.Sin(latitude);
			double e2 = this.FirstSquaredEccentricity;
			return this.majorAxis * Math.Sqrt(1.0 - e2) / (1.0 - e2 * sin * sin);
		}

		public virtual double FirstSquaredEccentricity
		{
			get
			{
				return this.firstSquaredEccentricity;
			}
		}

		public virtual double SecondSquaredEccentricity
		{
			get
			{
				return this.secondSquaredEccentricity;
			}
		}

		public virtual double Flattening
		{
			get
			{
				return this.flattening;
			}
		}

		public virtual double InverseFlattening
		{
			get
			{
				return this.inverseFlattening;
			}
		}

		public double MajorAxis
		{
			get
			{
				return this.majorAxis;
			}
		}

		public double MinorAxis
		{
			get
			{
				return this.minorAxis;
			}
		}

		public override string ToString()
		{
			return "Ellipsoid: [" + this.majorAxis + ", " + this.minorAxis + "]";
		}
	}

}