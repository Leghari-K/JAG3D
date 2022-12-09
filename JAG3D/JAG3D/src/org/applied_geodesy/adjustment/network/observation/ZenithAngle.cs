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

namespace org.applied_geodesy.adjustment.network.observation
{
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ProjectionType = org.applied_geodesy.adjustment.network.observation.reduction.ProjectionType;
	using Reduction = org.applied_geodesy.adjustment.network.observation.reduction.Reduction;
	using ReductionTaskType = org.applied_geodesy.adjustment.network.observation.reduction.ReductionTaskType;
	using AdditionalUnknownParameter = org.applied_geodesy.adjustment.network.parameter.AdditionalUnknownParameter;
	using RefractionCoefficient = org.applied_geodesy.adjustment.network.parameter.RefractionCoefficient;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public class ZenithAngle : Observation
	{
		private FaceType face = FaceType.ONE;
		private RefractionCoefficient refractionCoefficient = new RefractionCoefficient();

		public ZenithAngle(int id, Point startPoint, Point endPoint, double startPointHeight, double endPointHeight, double measuringElement, double sigma, double distanceForUncertaintyModel) : base(id, startPoint, endPoint, startPointHeight, endPointHeight, measuringElement, sigma, distanceForUncertaintyModel)
		{
			if (Math.Abs((2.0 * Math.PI - this.ValueApriori) - this.ValueAposteriori) < Math.Abs(this.ValueApriori - this.ValueAposteriori))
			{
				// Speichere, wie auch bei den Richtungen, den Wert der 1. Lage
				this.ValueApriori = 2.0 * Math.PI - this.ValueApriori;
				this.Face = FaceType.TWO;
			}
		}

		public override double diffXs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

			double ih = this.StartPointHeight;
			double th = this.EndPointHeight;

			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
			{
				rxs += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rys += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionY;

				rxe += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rye += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionY;
			}

			double srxs = Math.Sin(rxs);
			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);
			double srxe = Math.Sin(rxe);

			double u = crys * (xe - xs) + srys * (ze - zs) - th * Math.Sin(rye - rys) * crxe;
			double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
			double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);

			double dist2D_times_distSqr3D = Math.hypot(u, v) * (u * u + v * v + w * w);

			if (dist2D_times_distSqr3D == 0)
			{
				return 0.0;
			}
			return -(v * (crxs * srys * v + w * srxs * srys) + u * (crxs * srys * u + w * crys)) / dist2D_times_distSqr3D;
		}

		public override double diffYs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

			double ih = this.StartPointHeight;
			double th = this.EndPointHeight;

			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
			{
				rxs += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rys += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionY;

				rxe += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rye += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionY;
			}

			double srxs = Math.Sin(rxs);
			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);
			double srxe = Math.Sin(rxe);

			double u = crys * (xe - xs) + srys * (ze - zs) - th * Math.Sin(rye - rys) * crxe;
			double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
			double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);

			double dist2D_times_distSqr3D = Math.hypot(u, v) * (u * u + v * v + w * w);

			if (dist2D_times_distSqr3D == 0)
			{
				return 0.0;
			}
			return -(v * (crxs * w - srxs * v) - srxs * u * u) / dist2D_times_distSqr3D;
		}

		public override double diffZs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

			double ih = this.StartPointHeight;
			double th = this.EndPointHeight;

			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
			{
				rxs += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rys += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionY;

				rxe += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rye += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionY;
			}

			double srxs = Math.Sin(rxs);
			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);
			double srxe = Math.Sin(rxe);

			double u = crys * (xe - xs) + srys * (ze - zs) - th * Math.Sin(rye - rys) * crxe;
			double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
			double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);

			double dist2D_times_distSqr3D = Math.hypot(u, v) * (u * u + v * v + w * w);

			if (dist2D_times_distSqr3D == 0)
			{
				return 0.0;
			}
			return (v * (crxs * crys * v + w * crys * srxs) + u * (crxs * crys * u - w * srys)) / dist2D_times_distSqr3D;
		}

		public override double diffVerticalDeflectionXs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

			double ih = this.StartPointHeight;
			double th = this.EndPointHeight;

			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
			{
				rxs += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rys += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionY;

				rxe += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rye += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionY;
			}

			double srxs = Math.Sin(rxs);
			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);
			double srxe = Math.Sin(rxe);

			double u = crys * (xe - xs) + srys * (ze - zs) - th * Math.Sin(rye - rys) * crxe;
			double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
			double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);

			double dist2D_times_distSqr3D = Math.hypot(u, v) * (u * u + v * v + w * w);

			if (dist2D_times_distSqr3D == 0)
			{
				return 0.0;
			}
			return -v * (u * u + v * v + w * w + w * ih) / dist2D_times_distSqr3D;
		}

		public override double diffVerticalDeflectionYs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

			double ih = this.StartPointHeight;
			double th = this.EndPointHeight;

			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
			{
				rxs += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rys += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionY;

				rxe += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rye += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionY;
			}

			double srxs = Math.Sin(rxs);
			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);
			double srxe = Math.Sin(rxe);

			double u = crys * (xe - xs) + srys * (ze - zs) - th * Math.Sin(rye - rys) * crxe;
			double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
			double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);

			double dist2D_times_distSqr3D = Math.hypot(u, v) * (u * u + v * v + w * w);

			double tmp11 = crys * (xe - xs) + srys * (ze - zs) - th * crxe * crys * srye + th * crxe * crye * srys;
			double tmp21 = crys * (ze - zs) - srys * (xe - xs) + th * crxe * crye * crys + th * crxe * srye * srys;

			if (dist2D_times_distSqr3D == 0)
			{
				return 0.0;
			}
			return (tmp11 * crxs * (u * u + v * v) + w * (tmp11 * srxs * v + tmp21 * u)) / dist2D_times_distSqr3D;
		}

		public override double diffVerticalDeflectionXe()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

			double ih = this.StartPointHeight;
			double th = this.EndPointHeight;

			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
			{
				rxs += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rys += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionY;

				rxe += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rye += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionY;
			}

			double srxs = Math.Sin(rxs);
			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);
			double srxe = Math.Sin(rxe);

			double u = crys * (xe - xs) + srys * (ze - zs) - th * Math.Sin(rye - rys) * crxe;
			double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
			double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);

			double dist2D_times_distSqr3D = Math.hypot(u, v) * (u * u + v * v + w * w);

			double tmp12 = th * (crxs * crye * crys * srxe - crxe * srxs + crxs * srxe * srye * srys);
			double tmp22 = th * (crxe * crxs + crye * crys * srxe * srxs + srxe * srxs * srye * srys);

			if (dist2D_times_distSqr3D == 0)
			{
				return 0.0;
			}
			return (w * (tmp22 * v + th * u * Math.Sin(rye - rys) * srxe) + tmp12 * (u * u + v * v)) / dist2D_times_distSqr3D;
		}

		public override double diffVerticalDeflectionYe()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

			double ih = this.StartPointHeight;
			double th = this.EndPointHeight;

			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
			{
				rxs += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rys += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionY;

				rxe += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionX;
				rye += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionY;
			}

			double srxs = Math.Sin(rxs);
			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);
			double srxe = Math.Sin(rxe);

			double u = crys * (xe - xs) + srys * (ze - zs) - th * Math.Sin(rye - rys) * crxe;
			double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
			double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);

			double dist2D_times_distSqr3D = Math.hypot(u, v) * (u * u + v * v + w * w);

			if (dist2D_times_distSqr3D == 0)
			{
				return 0.0;
			}
			return ((th * crxe * (Math.Sin(rye - rys) * crxs * (u * u + v * v) - w * (Math.Cos(rye - rys) * u - Math.Sin(rye - rys) * srxs * v)))) / dist2D_times_distSqr3D;
		}

		public override double ValueAposteriori
		{
			get
			{
				Reduction reductions = this.Reductions;
				double R = reductions.EarthRadius;
    
				double xs = this.StartPoint.X;
				double ys = this.StartPoint.Y;
				double zs = this.StartPoint.Z;
    
				double xe = this.EndPoint.X;
				double ye = this.EndPoint.Y;
				double ze = this.EndPoint.Z;
    
				double ih = this.StartPointHeight;
				double th = this.EndPointHeight;
    
				double rxs = this.StartPoint.VerticalDeflectionX.Value;
				double rys = this.StartPoint.VerticalDeflectionY.Value;
    
				double rxe = this.EndPoint.VerticalDeflectionX.Value;
				double rye = this.EndPoint.VerticalDeflectionY.Value;
    
				if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
				{
					rxs += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rys += this.StartPoint.SphericalDeflectionParameter.SphericalDeflectionY;
    
					rxe += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rye += this.EndPoint.SphericalDeflectionParameter.SphericalDeflectionY;
				}
    
				double srxs = Math.Sin(rxs);
				double srys = Math.Sin(rys);
				double crxs = Math.Cos(rxs);
				double crys = Math.Cos(rys);
    
				double crxe = Math.Cos(rxe);
				double crye = Math.Cos(rye);
				double srye = Math.Sin(rye);
				double srxe = Math.Sin(rxe);
    
				double u = crys * (xe - xs) + srys * (ze - zs) - th * Math.Sin(rye - rys) * crxe;
				double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
				double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);
    
				// Rueger 1996, S. 109, Gl. 8.26
				// kDeltaH = (1-k) * dist2D * dist2D / (2*R)
				// Bogenformel: (1-k) * dist2D / (2*R)
				double dist2D = Math.hypot(u, v);
				double corr = dist2D / (2.0 * R);
    
				double k = this.refractionCoefficient.Value;
				double geoCorr = -k * corr;
    
				if (reductions.applyReductionTask(ReductionTaskType.EARTH_CURVATURE))
				{
					geoCorr += corr;
				}
    
				return MathExtension.MOD(Math.Atan2(dist2D, w) + geoCorr, 2.0 * Math.PI);
			}
		}

		public override double Correction
		{
			get
			{
				double calAngle = this.ValueAposteriori;
				double obsAngle = this.ValueApriori;
    
				// Reduziere auf Lage 1
				if (Math.Abs((2.0 * Math.PI - obsAngle) - calAngle) < Math.Abs(obsAngle - calAngle))
				{
					obsAngle = 2.0 * Math.PI - obsAngle;
				}
    
				return obsAngle - calAngle;
			}
		}

		public override double diffRefCoeff()
		{
			Reduction reductions = this.Reductions;
			double R = reductions.EarthRadius;

			double dist2D = this.CalculatedDistance2D;
			return -dist2D / (2.0 * R);
		}

		public virtual FaceType Face
		{
			get
			{
				return this.face;
			}
			set
			{
				this.face = value;
			}
		}


		public virtual RefractionCoefficient RefractionCoefficient
		{
			set
			{
				this.refractionCoefficient = value;
				this.refractionCoefficient.Observation = this;
			}
			get
			{
				return this.refractionCoefficient;
			}
		}


		public override int ColInJacobiMatrixFromRefCoeff
		{
			get
			{
				return this.refractionCoefficient.ColInJacobiMatrix;
			}
		}

		public override ObservationType ObservationType
		{
			get
			{
				return ObservationType.ZENITH_ANGLE;
			}
		}
	}

}