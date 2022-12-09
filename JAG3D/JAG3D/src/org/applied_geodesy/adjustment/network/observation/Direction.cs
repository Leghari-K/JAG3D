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
	using Orientation = org.applied_geodesy.adjustment.network.parameter.Orientation;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public class Direction : Observation
	{
		private FaceType face = FaceType.ONE;
		private Orientation orientation = new Orientation(true);

		public Direction(int id, Point startPoint, Point endPoint, double startPointHeight, double endPointHeight, double measuringElement, double sigma, double distanceForUncertaintyModel) : base(id, startPoint, endPoint, startPointHeight, endPointHeight, measuringElement, sigma, distanceForUncertaintyModel)
		{
		}

		public override double diffXs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

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

			double distSqr2D = u * u + v * v;

			if (distSqr2D == 0.0)
			{
				return 0.0;
			}
			return (crxs * (crys * ye - crys * ys + crys * srxe * th) - srxs * (ze - zs + crxe * crye * th)) / distSqr2D;
		}

		public override double diffYs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

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

			double distSqr2D = u * u + v * v;

			if (distSqr2D == 0.0)
			{
				return 0.0;
			}
			return -(u * crxs) / distSqr2D;
		}

		public override double diffZs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

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

			double distSqr2D = u * u + v * v;

			if (distSqr2D == 0.0)
			{
				return 0.0;
			}
			return (crxs * (srys * (ye - ys) + srxe * srys * th) - srxs * (xs - xe + crxe * srye * th)) / distSqr2D;
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

			double distSqr2D = u * u + v * v;

			if (distSqr2D == 0.0)
			{
				return 0.0;
			}
			return -(w + ih) * u / distSqr2D;
		}

		public override double diffVerticalDeflectionYs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

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

			double distSqr2D = u * u + v * v;

			if (distSqr2D == 0.0)
			{
				return 0.0;
			}
			return (srxs * u * u - v * (crys * (ze - zs) - srys * (xe - xs)) - th * v * Math.Cos(rye - rys) * crxe) / distSqr2D;
		}

		public override double diffVerticalDeflectionXe()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

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

			double distSqr2D = u * u + v * v;

			if (distSqr2D == 0.0)
			{
				return 0.0;
			}
			return (th * (u * (crxe * crxs + crye * crys * srxe * srxs + srxe * srxs * srye * srys) - v * Math.Sin(rye - rys) * srxe)) / distSqr2D;
		}

		public override double diffVerticalDeflectionYe()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

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

			double distSqr2D = u * u + v * v;

			if (distSqr2D == 0.0)
			{
				return 0.0;
			}
			return th * crxe * (v * Math.Cos(rye - rys) + u * Math.Sin(rye - rys) * srxs) / distSqr2D;
		}

		public override double diffOri()
		{
			return -1.0;
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


		public override double Correction
		{
			get
			{
				Reduction reductions = this.Reductions;
    
				double calDir = this.ValueAposteriori;
				double obsDir = this.ValueApriori;
    
				// Richtungsreduktion
				if ((reductions.ProjectionType == ProjectionType.GAUSS_KRUEGER || reductions.ProjectionType == ProjectionType.UTM) && reductions.applyReductionTask(ReductionTaskType.DIRECTION))
				{
					double R = reductions.EarthRadius;
					double scale = reductions.ProjectionType == ProjectionType.UTM ? 0.9996 : 1.0;
					double yS = this.StartPoint.Y;
					double xS = this.StartPoint.X;
					double yE = this.EndPoint.Y;
					double xE = this.EndPoint.X;
    
					// Reduziere Rechtswert
					yS = ((yS / 1000000.0) % 1) * 1000000.0 - 500000.0;
					yE = ((yE / 1000000.0) % 1) * 1000000.0 - 500000.0;
    
					double k = -(xE - xS) * (2.0 * yS + yE) / (6.0 * R * R * scale * scale);
					obsDir = MathExtension.MOD(obsDir + k, 2.0 * Math.PI);
				}
    
				double diffDir = obsDir - calDir;
    
				// Pruefung ist fuer RiWis, die Fast 0 bzw. 400 gon sind, um eine kleine Verbesserung zu bekommen.
				if (2.0 * Math.PI - Math.Abs(diffDir) < Math.Abs(diffDir))
				{
					if (calDir < obsDir)
					{
						calDir += 2.0 * Math.PI;
					}
					else
					{
						calDir -= 2.0 * Math.PI;
					}
				}
    
				return obsDir - calDir;
			}
		}

		public override double ValueAposteriori
		{
			get
			{
				double xs = this.StartPoint.X;
				double ys = this.StartPoint.Y;
				double zs = this.StartPoint.Z;
    
				double xe = this.EndPoint.X;
				double ye = this.EndPoint.Y;
				double ze = this.EndPoint.Z;
    
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
    
				double ori = this.orientation.Value;
    
				return MathExtension.MOD(Math.Atan2(v, u) - ori, 2.0 * Math.PI);
			}
		}

		public virtual Orientation Orientation
		{
			set
			{
				this.orientation = value;
				this.orientation.Observation = this;
			}
			get
			{
				return this.orientation;
			}
		}


		public override int ColInJacobiMatrixFromOrientation
		{
			get
			{
				return this.orientation.ColInJacobiMatrix;
			}
		}

		public override ObservationType ObservationType
		{
			get
			{
				return ObservationType.DIRECTION;
			}
		}
	}

}