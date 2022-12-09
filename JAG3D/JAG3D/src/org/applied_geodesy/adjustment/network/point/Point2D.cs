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

namespace org.applied_geodesy.adjustment.network.point
{
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;

	public class Point2D : Point
	{

		public Point2D(Point3D p3d) : this(p3d.Name, p3d.X, p3d.Y, p3d.Z)
		{
			if (p3d.StdX > 0)
			{
				this.StdX = p3d.StdX;
			}
			if (p3d.StdY > 0)
			{
				this.StdY = p3d.StdY;
			}
		}

		public Point2D(string id, double x, double y, double z, double sX, double sY) : this(id, x, y)
		{

			this.StdX = sX;
			this.StdY = sY;
		}

		public Point2D(string id, double x, double y, double sX, double sY) : this(id, x, y, 0, sX, sY)
		{
		}

		public Point2D(string id, double x, double y) : this(id, x, y, 0)
		{
		}

		public Point2D(string id, double x, double y, double z) : base(id)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;

			this.coordinates0[0] = x;
			this.coordinates0[1] = y;
			this.coordinates0[2] = z;
		}

		public override string ToString()
		{
			return new string(this.GetType() + " " + this.Name + ": " + this.X + "/" + this.Y);
		}

		public override ParameterType ParameterType
		{
			get
			{
				return ParameterType.POINT2D;
			}
		}

		public override int Dimension
		{
			get
			{
				return 2;
			}
		}
	}

}