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
	using RotationX = org.applied_geodesy.adjustment.network.parameter.RotationX;
	using RotationY = org.applied_geodesy.adjustment.network.parameter.RotationY;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public abstract class GNSSBaseline3D : GNSSBaseline
	{
		private RotationX rx = new RotationX();
		private RotationY ry = new RotationY();

		public GNSSBaseline3D(int id, Point startPoint, Point endPoint, double observation, double sigma) : base(id, startPoint, endPoint, 0, 0, observation, sigma)
		{
		}

		public virtual RotationX RotationX
		{
			set
			{
				this.rx = value;
				this.rx.Observation = this;
			}
			get
			{
				return this.rx;
			}
		}

		public virtual RotationY RotationY
		{
			set
			{
				this.ry = value;
				this.ry.Observation = this;
			}
			get
			{
				return this.ry;
			}
		}



		public override int ColInJacobiMatrixFromRotationX
		{
			get
			{
				return this.rx.ColInJacobiMatrix;
			}
		}

		public override int ColInJacobiMatrixFromRotationY
		{
			get
			{
				return this.ry.ColInJacobiMatrix;
			}
		}

		public override int Dimension
		{
			get
			{
				return 3;
			}
		}

		public override ObservationType ObservationType
		{
			get
			{
				return ObservationType.GNSS3D;
			}
		}
	}
}