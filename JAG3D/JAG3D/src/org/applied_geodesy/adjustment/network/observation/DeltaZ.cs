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
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ProjectionType = org.applied_geodesy.adjustment.network.observation.reduction.ProjectionType;
	using AdditionalUnknownParameter = org.applied_geodesy.adjustment.network.parameter.AdditionalUnknownParameter;
	using Scale = org.applied_geodesy.adjustment.network.parameter.Scale;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public class DeltaZ : Observation
	{
		private Scale scale = new Scale();

		public DeltaZ(int id, Point startPoint, Point endPoint, double startPointHeight, double endPointHeight, double observation, double sigma, double distanceForUncertaintyModel) : base(id, startPoint, endPoint, startPointHeight, endPointHeight, observation, sigma, distanceForUncertaintyModel)
		{
		}

		public override double diffXs()
		{
			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double scale = this.scale.Value;

			return (crxs * srys) / scale;
		}

		public override double diffYs()
		{
			double rxs = this.StartPoint.VerticalDeflectionX.Value;

			double srxs = Math.Sin(rxs);
			double scale = this.scale.Value;

			return -srxs / scale;
		}

		public override double diffZs()
		{
			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double crys = Math.Cos(rys);
			double crxs = Math.Cos(rxs);
			double scale = this.scale.Value;

			return -(crxs * crys) / scale;
		}


		public override double diffVerticalDeflectionXs()
		{
			double xs = this.StartPoint.X;
			double ys = this.StartPoint.Y;
			double zs = this.StartPoint.Z;

			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double srxs = Math.Sin(rxs);
			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double scale = this.scale.Value;

			return -(ys * crxs - zs * crys * srxs + xs * srxs * srys) / scale;
		}

		public override double diffVerticalDeflectionYs()
		{
			double xs = this.StartPoint.X;
			double zs = this.StartPoint.Z;

			double rxs = this.StartPoint.VerticalDeflectionX.Value;
			double rys = this.StartPoint.VerticalDeflectionY.Value;

			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double scale = this.scale.Value;

			return (crxs * (xs * crys + zs * srys)) / scale;
		}

		public override double diffXe()
		{
			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			double srye = Math.Sin(rye);
			double crxe = Math.Cos(rxe);
			double scale = this.scale.Value;

			return -(crxe * srye) / scale;
		}

		public override double diffYe()
		{
			double rxe = this.EndPoint.VerticalDeflectionX.Value;

			double srxe = Math.Sin(rxe);
			double scale = this.scale.Value;

			return srxe / scale;
		}

		public override double diffZe()
		{
			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			double crye = Math.Cos(rye);
			double crxe = Math.Cos(rxe);
			double scale = this.scale.Value;

			return (crxe * crye) / scale;
		}

		public override double diffVerticalDeflectionXe()
		{
			double xe = this.EndPoint.X;
			double ye = this.EndPoint.Y;
			double ze = this.EndPoint.Z;

			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);
			double srxe = Math.Sin(rxe);

			double scale = this.scale.Value;

			return (ye * crxe - ze * crye * srxe + xe * srxe * srye) / scale;
		}

		public override double diffVerticalDeflectionYe()
		{
			double xe = this.EndPoint.X;
			double ze = this.EndPoint.Z;

			double rxe = this.EndPoint.VerticalDeflectionX.Value;
			double rye = this.EndPoint.VerticalDeflectionY.Value;

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);

			double scale = this.scale.Value;

			return -(crxe * (xe * crye + ze * srye)) / scale;
		}


		public override double diffScale()
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

			double srxs = Math.Sin(rxs);
			double srys = Math.Sin(rys);
			double crxs = Math.Cos(rxs);
			double crys = Math.Cos(rys);

			double crxe = Math.Cos(rxe);
			double crye = Math.Cos(rye);
			double srye = Math.Sin(rye);
			double srxe = Math.Sin(rxe);

			double ws = -xs * crxs * srys + ys * srxs + zs * crxs * crys - ih;
			double we = -xe * crxe * srye + ye * srxe + ze * crxe * crye - th;

			double dN = 0;
			if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
			{
				double hs = this.StartPoint.SphericalDeflectionParameter.FrameIntersectionHeight;
				double he = this.EndPoint.SphericalDeflectionParameter.FrameIntersectionHeight;
				dN = he - hs;
			}

			double scale = this.scale.Value;
			double dh = we - ws + dN;
			return -dh / (scale * scale);
		}

		public virtual AdditionalUnknownParameter Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
				this.scale.Observation = this;
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
    
				double ih = this.StartPointHeight;
				double th = this.EndPointHeight;
    
				double rxs = this.StartPoint.VerticalDeflectionX.Value;
				double rys = this.StartPoint.VerticalDeflectionY.Value;
    
				double rxe = this.EndPoint.VerticalDeflectionX.Value;
				double rye = this.EndPoint.VerticalDeflectionY.Value;
    
				double srxs = Math.Sin(rxs);
				double srys = Math.Sin(rys);
				double crxs = Math.Cos(rxs);
				double crys = Math.Cos(rys);
    
				double crxe = Math.Cos(rxe);
				double crye = Math.Cos(rye);
				double srye = Math.Sin(rye);
				double srxe = Math.Sin(rxe);
    
				double ws = -xs * crxs * srys + ys * srxs + zs * crxs * crys - ih;
				double we = -xe * crxe * srye + ye * srxe + ze * crxe * crye - th;
    
				double dN = 0;
				if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
				{
					double hs = this.StartPoint.SphericalDeflectionParameter.FrameIntersectionHeight;
					double he = this.EndPoint.SphericalDeflectionParameter.FrameIntersectionHeight;
					dN = he - hs;
				}
    
				double scale = this.scale.Value;
		//		double dh = ze - zs + dN;
				double dh = we - ws + dN;
				return dh / scale;
			}
		}

		public override ObservationType ObservationType
		{
			get
			{
				return ObservationType.LEVELING;
			}
		}
	}

}