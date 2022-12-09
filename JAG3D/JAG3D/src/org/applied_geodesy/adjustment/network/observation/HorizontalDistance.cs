﻿using System;

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
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ProjectionType = org.applied_geodesy.adjustment.network.observation.reduction.ProjectionType;
	using Reduction = org.applied_geodesy.adjustment.network.observation.reduction.Reduction;
	using ReductionTaskType = org.applied_geodesy.adjustment.network.observation.reduction.ReductionTaskType;
	using AdditionalUnknownParameter = org.applied_geodesy.adjustment.network.parameter.AdditionalUnknownParameter;
	using Scale = org.applied_geodesy.adjustment.network.parameter.Scale;
	using ZeroPointOffset = org.applied_geodesy.adjustment.network.parameter.ZeroPointOffset;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public class HorizontalDistance : Observation
	{

		private Scale scale = new Scale();
		private ZeroPointOffset add = new ZeroPointOffset();

		public HorizontalDistance(int id, Point startPoint, Point endPoint, double startPointHeight, double endPointHeight, double measuringElement, double sigma, double distanceForUncertaintyModel) : base(id, startPoint, endPoint, startPointHeight, endPointHeight, measuringElement, sigma, distanceForUncertaintyModel)
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

			double scale = this.scale.Value;
			double dist2D = scale * Math.hypot(u, v);

			if (dist2D == 0.0)
			{
				return 0.0;
			}
			return -(crys * u + srxs * srys * v) / dist2D;
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

			double scale = this.scale.Value;
			double dist2D = scale * Math.hypot(u, v);

			if (dist2D == 0.0)
			{
				return 0.0;
			}
			return -(crxs * v) / dist2D;
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

			double scale = this.scale.Value;
			double dist2D = scale * Math.hypot(u, v);

			if (dist2D == 0.0)
			{
				return 0.0;
			}
			return -(srys * u - crys * srxs * v) / dist2D;
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

			double scale = this.scale.Value;
			double dist2D = scale * Math.hypot(u, v);

			if (dist2D == 0.0)
			{
				return 0.0;
			}
			return -(v * (w + ih)) / dist2D;
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

			double scale = this.scale.Value;
			double dist2D = scale * Math.hypot(u, v);

			if (dist2D == 0.0)
			{
				return 0.0;
			}
			return (u * (crys * (ze - zs) - srys * (xe - xs) + th * Math.Cos(rye - rys) * crxe) + v * srxs * (crys * (xe - xs) + srys * (ze - zs) - th * Math.Sin(rye - rys) * crxe)) / dist2D;
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

			double scale = this.scale.Value;
			double dist2D = scale * Math.hypot(u, v);

			if (dist2D == 0.0)
			{
				return 0.0;
			}
			return (th * v * (crxe * crxs + crye * crys * srxe * srxs + srxe * srxs * srye * srys) + th * u * Math.Sin(rye - rys) * srxe) / dist2D;
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

			double scale = this.scale.Value;
			double dist2D = scale * Math.hypot(u, v);

			if (dist2D == 0.0)
			{
				return 0.0;
			}
			return -th * crxe * (u * Math.Cos(rye - rys) - v * Math.Sin(rye - rys) * srxs) / dist2D;
		}

		public override double diffAdd()
		{
			return -1.0 / this.scale.Value;
		}

		public override double diffScale()
		{
			double add = this.add.Value;
			double scale = this.scale.Value;
			double sH = this.CalculatedDistance2D;
			return (-sH + add) / Math.Pow(scale, 2);
		}

		public override double ValueAposteriori
		{
			get
			{
				double scale = this.scale.Value;
				double add = this.add.Value;
				double sH = this.CalculatedDistance2D;
				return 1.0 / scale * (sH - add);
			}
		}

		public override double Correction
		{
			get
			{
				double calDist = this.ValueAposteriori;
				double obsDist = this.ValueApriori;
    
				Reduction reductions = this.Reductions;
				double R = reductions.EarthRadius;
    
				// Hoehenreduktion - Rueeger 1996, S. 99, Gl. 7.56
				if (reductions.applyReductionTask(ReductionTaskType.HEIGHT))
				{
					double h0 = reductions.PrincipalPoint.Height;
					obsDist = obsDist * R / (R + h0);
				}
    
				// Erdkruemmungsreduktion - Rueeger 1996, S. 90, Gl. 7.29
				if (reductions.applyReductionTask(ReductionTaskType.EARTH_CURVATURE))
				{
					obsDist = 2.0 * R * Math.Asin(0.5 * obsDist / R);
				}
    
				// Streckenreduktion bzgl. Projektion
				if ((reductions.ProjectionType == ProjectionType.GAUSS_KRUEGER || reductions.ProjectionType == ProjectionType.UTM) && reductions.applyReductionTask(ReductionTaskType.DISTANCE))
				{
					double m0 = reductions.ProjectionType == ProjectionType.UTM ? 0.9996 : 1.0;
					double yS = this.StartPoint.Y;
					double yE = this.EndPoint.Y;
    
					// Reduziere Rechtswert
					yS = ((yS / 1000000.0) % 1) * 1000000.0 - 500000.0;
					yE = ((yE / 1000000.0) % 1) * 1000000.0 - 500000.0;
    
					double k = (yS * yS + yS * yE + yE * yE) / 6.0 / R / R;
					obsDist = m0 * (1.0 + k) * obsDist;
				}
				return obsDist - calDist;
			}
		}

		public virtual Scale Scale
		{
			set
			{
				this.scale = value;
				this.scale.Observation = this;
			}
			get
			{
				return this.scale;
			}
		}


		public virtual ZeroPointOffset ZeroPointOffset
		{
			set
			{
				this.add = value;
				this.add.Observation = this;
			}
			get
			{
				return this.add;
			}
		}


		public override int ColInJacobiMatrixFromScale
		{
			get
			{
				return this.scale.ColInJacobiMatrix;
			}
		}

		public override int ColInJacobiMatrixFromAdd
		{
			get
			{
				return this.add.ColInJacobiMatrix;
			}
		}

		public override ObservationType ObservationType
		{
			get
			{
				return ObservationType.HORIZONTAL_DISTANCE;
			}
		}
	}

}