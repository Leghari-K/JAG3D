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

namespace org.applied_geodesy.adjustment.network.approximation.bundle.point
{
	using PointBundle = org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle;
	using Transformation2D = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.Transformation2D;

	public class Point2D : Point
	{

		public Point2D(string id, double x, double y) : base(id)
		{
			this.X = x;
			this.Y = y;
		}

		public override sealed int Dimension
		{
			get
			{
				return 2;
			}
		}

		public virtual void addObservedPoint(string id, double dir, double dist2d)
		{
			if (id.Equals(this.Name))
			{
				Console.Error.WriteLine(this.GetType().Name + " Fehler, Punkt kann sich nicht selbst beobachten! " + this.Name + "  " + id);
				return;
			}
			PointBundle currentBundle = this.CurrentBundle;
			Point2D p = ClassicGeodeticComputation.POLAR(this, id, dir, dist2d);
			currentBundle.addPoint(p);
		}

		public virtual void addObservedPoint(string id, double dir, double dist3d, double ihDist, double thDist, double zenith, double ihZenith, double thZenith)
		{
			if (id.Equals(this.Name))
			{
				Console.Error.WriteLine(this.GetType().Name + " Fehler, Punkt kann sich nicht selbst beobachten! " + this.Name + "  " + id);
				return;
			}
			double slopeDist = ClassicGeodeticComputation.SLOPEDISTANCE(dist3d, ihDist, thDist, zenith, ihZenith, thZenith);
			PointBundle currentBundle = this.CurrentBundle;
			Point2D p = ClassicGeodeticComputation.POLAR(this, id, dir, slopeDist, zenith);
			currentBundle.addPoint(p);
		}

		public override Transformation2D getTransformation(PointBundle b1, PointBundle b2)
		{
			return new Transformation2D(b1, b2);
		}

		public override string ToString()
		{
			return this.Name + "  " + this.X + "    " + this.Y + " ";
		}
	}


}