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
	using Transformation1D = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.Transformation1D;

	public class Point1D : Point
	{

		public Point1D(string id, double z) : base(id)
		{
			this.Z = z;
		}

		public override sealed int Dimension
		{
			get
			{
				return 1;
			}
		}

		public override Transformation1D getTransformation(PointBundle b1, PointBundle b2)
		{
			return new Transformation1D(b1, b2);
		}

		public virtual void addObservedPoint(string id, double dist3d, double ihDist, double thDist, double zenith, double ihZenith, double thZenith)
		{
			if (id.Equals(this.Name))
			{
				Console.Error.WriteLine(this.GetType().Name + " Fehler, Punkt kann sich nicht selbst beobachten! " + this.Name + "  " + id);
				return;
			}
			double slopeDist = ClassicGeodeticComputation.SLOPEDISTANCE(dist3d, ihDist, thDist, zenith, ihZenith, thZenith);
			PointBundle currentBundle = this.CurrentBundle;
			Point1D p = ClassicGeodeticComputation.TRIGO_HEIGHT_3D(this, id, slopeDist, zenith, ihZenith, thZenith);
			currentBundle.addPoint(p);
		}

		public virtual void addObservedPoint(string id, double dist2d, double zenith, double ihZenith, double thZenith)
		{
			if (id.Equals(this.Name))
			{
				Console.Error.WriteLine(this.GetType().Name + " Fehler, Punkt kann sich nicht selbst beobachten! " + this.Name + "  " + id);
				return;
			}
			PointBundle currentBundle = this.CurrentBundle;
			Point1D p = ClassicGeodeticComputation.TRIGO_HEIGHT_2D(this, id, dist2d, zenith, ihZenith, thZenith);
			currentBundle.addPoint(p);
		}

		public virtual void addObservedPoint(string id, double deltaH, double ih, double th)
		{
			if (id.Equals(this.Name))
			{
				Console.Error.WriteLine(this.GetType().Name + " Fehler, Punkt kann sich nicht selbst beobachten! " + this.Name + "  " + id);
				return;
			}
			PointBundle currentBundle = this.CurrentBundle;
			Point1D p = ClassicGeodeticComputation.ORTHO_HEIGHT(this, id, deltaH, ih, th);
			currentBundle.addPoint(p);
		}

		public virtual void addObservedPoint(string id, double deltaH)
		{
			if (id.Equals(this.Name))
			{
				Console.Error.WriteLine(this.GetType().Name + " Fehler, Punkt kann sich nicht selbst beobachten! " + this.Name + "  " + id);
				return;
			}
			PointBundle currentBundle = this.CurrentBundle;
			Point1D p = new Point1D(id, this.Z + deltaH);
			currentBundle.addPoint(p);
		}

		public override string ToString()
		{
			return this.Name + "  [ z = " + this.Z + " ]";
		}
	}


}