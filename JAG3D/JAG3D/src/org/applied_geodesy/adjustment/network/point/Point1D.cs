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

	public class Point1D : Point
	{

		public Point1D(Point3D p3d) : this(p3d.Name, p3d.Z)
		{
			if (p3d.StdZ > 0.0)
			{
				this.StdZ = p3d.StdZ;
			}
		}

		public Point1D(string id, double z, double sZ) : this(id, z)
		{
			this.StdZ = sZ;
		}

		public Point1D(string id, double z) : this(id, 0.0, 0.0, z)
		{
		}

		public Point1D(string id, double x, double y, double z) : this(id, x, y, z, 0.0)
		{
		}

		public Point1D(string id, double x, double y, double z, double sZ) : base(id)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.coordinates0[0] = x;
			this.coordinates0[1] = y;
			this.coordinates0[2] = z;
			this.StdZ = sZ;
		}

		public override string ToString()
		{
			return new string(this.GetType() + " " + this.Name + ": " + this.X + "/" + this.Y + "/" + this.Z);
		}

		public sealed override int Dimension
		{
			get
			{
				return 1;
			}
		}

		public override ParameterType ParameterType
		{
			get
			{
				return ParameterType.POINT1D;
			}
		}
	}

}