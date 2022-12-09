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
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public class GNSSBaselineDeltaZ1D : GNSSBaseline1D
	{
		public GNSSBaselineDeltaZ1D(int id, Point startPoint, Point endPoint, double observation, double sigma) : base(id, startPoint, endPoint, observation, sigma)
		{
		}
		//	fz = m*(  Math.sin(ry)*dX - Math.sin(rx)*Math.cos(ry)*dY + Math.cos(rx)*Math.cos(ry)*dZ)

		public override double diffXs()
		{
			double m = this.Scale.getValue();
			double ry = this.RotationY.getValue();
			return -m * Math.Sin(ry);
		}

		public override double diffYs()
		{
			double m = this.Scale.getValue();
			double rx = this.RotationX.getValue();
			double ry = this.RotationY.getValue();

			return m * Math.Sin(rx) * Math.Cos(ry);
		}

		public override double diffZs()
		{
			double m = this.Scale.getValue();
			double rx = this.RotationX.getValue();
			double ry = this.RotationY.getValue();

			return -m * Math.Cos(rx) * Math.Cos(ry);
		}

		public override double diffRotX()
		{
			double m = this.Scale.getValue();
			double rx = this.RotationX.getValue();
			double ry = this.RotationY.getValue();

			double dY = this.EndPoint.Y - this.StartPoint.Y;
			double dZ = this.EndPoint.Z + this.EndPointHeight - this.StartPoint.Z - this.StartPointHeight;

			return m * (-Math.Cos(rx) * Math.Cos(ry) * dY - Math.Sin(rx) * Math.Cos(ry) * dZ);
		}

		public override double diffRotY()
		{
			double m = this.Scale.getValue();
			double rx = this.RotationX.getValue();
			double ry = this.RotationY.getValue();

			double dX = this.EndPoint.X - this.StartPoint.X;
			double dY = this.EndPoint.Y - this.StartPoint.Y;
			double dZ = this.EndPoint.Z + this.EndPointHeight - this.StartPoint.Z - this.StartPointHeight;

			return m * (Math.Cos(ry) * dX + Math.Sin(rx) * Math.Sin(ry) * dY - Math.Cos(rx) * Math.Sin(ry) * dZ);
		}

		public override double diffScale()
		{
			double rx = this.RotationX.getValue();
			double ry = this.RotationY.getValue();

			double dX = this.EndPoint.X - this.StartPoint.X;
			double dY = this.EndPoint.Y - this.StartPoint.Y;
			double dZ = this.EndPoint.Z + this.EndPointHeight - this.StartPoint.Z - this.StartPointHeight;

			return Math.Sin(ry) * dX - Math.Sin(rx) * Math.Cos(ry) * dY + Math.Cos(rx) * Math.Cos(ry) * dZ;
		}

		public override double ValueAposteriori
		{
			get
			{
				double m = this.Scale.getValue();
				double rx = this.RotationX.getValue();
				double ry = this.RotationY.getValue();
    
				double dX = this.EndPoint.X - this.StartPoint.X;
				double dY = this.EndPoint.Y - this.StartPoint.Y;
				double dZ = this.EndPoint.Z + this.EndPointHeight - this.StartPoint.Z - this.StartPointHeight;
    
				return m * (Math.Sin(ry) * dX - Math.Sin(rx) * Math.Cos(ry) * dY + Math.Cos(rx) * Math.Cos(ry) * dZ);
			}
		}

		public override ComponentType Component
		{
			get
			{
				return ComponentType.Z;
			}
		}
	}

}