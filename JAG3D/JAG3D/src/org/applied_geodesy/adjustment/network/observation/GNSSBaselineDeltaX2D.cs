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
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public class GNSSBaselineDeltaX2D : GNSSBaseline2D
	{

		public GNSSBaselineDeltaX2D(int id, Point startPoint, Point endPoint, double observation, double sigma) : base(id, startPoint, endPoint, observation, sigma)
		{
		}

		public override double diffXs()
		{
			double m = this.Scale.getValue();
			double phi = this.RotationZ.getValue();
			return -m * Math.Cos(phi);
		}

		public override double diffYs()
		{
			double m = this.Scale.getValue();
			double phi = this.RotationZ.getValue();
			return -m * Math.Sin(phi);
		}

		public override double diffRotZ()
		{
			double m = this.Scale.getValue();
			double phi = this.RotationZ.getValue();
			double dX = this.EndPoint.X - this.StartPoint.X;
			double dY = this.EndPoint.Y - this.StartPoint.Y;

			return m * (Math.Cos(phi) * dY - Math.Sin(phi) * dX);
		}

		public override double diffScale()
		{
			double phi = this.RotationZ.getValue();
			double dX = this.EndPoint.X - this.StartPoint.X;
			double dY = this.EndPoint.Y - this.StartPoint.Y;

			return Math.Sin(phi) * dY + Math.Cos(phi) * dX;
		}

		public override double ValueAposteriori
		{
			get
			{
				double m = this.Scale.getValue();
				double phi = this.RotationZ.getValue();
				double dX = this.EndPoint.X - this.StartPoint.X;
				double dY = this.EndPoint.Y - this.StartPoint.Y;
    
				return m * (Math.Sin(phi) * dY + Math.Cos(phi) * dX);
			}
		}

		public override double diffZs()
		{
			return 0;
		}

		public override ComponentType Component
		{
			get
			{
				return ComponentType.X;
			}
		}
	}

}