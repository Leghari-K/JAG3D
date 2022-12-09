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
	using Transformation = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.Transformation;

	public class Point3D : Point
	{

		public Point3D(string id, double x, double y, double z) : base(id)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public override sealed int Dimension
		{
			get
			{
				return 2;
			}
		}

		public override Transformation getTransformation(PointBundle b1, PointBundle b2)
		{
			return null;
		}

		public override string ToString()
		{
			return this.Name + "  [ x = " + this.X + " / y = " + this.Y + " / z = " + this.Z + " ]";
		}
	}
}