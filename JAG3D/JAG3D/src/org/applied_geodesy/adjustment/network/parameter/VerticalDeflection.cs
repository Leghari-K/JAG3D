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

namespace org.applied_geodesy.adjustment.network.parameter
{
	using ObservationGroup = org.applied_geodesy.adjustment.network.observation.group.ObservationGroup;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public abstract class VerticalDeflection : UnknownParameter
	{
		private readonly Point point;

		private int rowInJacobiMatrix = -1;

		private double redundancy = 0.0, sigma0 = -1.0, sigma = 0.0, omega = 0.0, value0, value, nabla, mdb, mtb, confidence;

		public VerticalDeflection(Point point) : this(point, 0.0)
		{
		}

		public VerticalDeflection(Point point, double value)
		{
			this.point = point;
			this.Value0 = value;
		}

		public VerticalDeflection(Point point, double value, double std)
		{
			this.Value0 = value;
			this.point = point;
			this.Std = std;
		}

		public Point Point
		{
			get
			{
				return this.point;
			}
		}

		public virtual double Value
		{
			set
			{
				this.value = value;
			}
			get
			{
				return this.value;
			}
		}


		public virtual double Value0
		{
			set
			{
				this.value0 = value;
				this.Value = value;
			}
			get
			{
				return this.value0;
			}
		}


		public virtual void reset()
		{
			this.Value = this.value0;
		}

		public override ObservationGroup Observations
		{
			get
			{
				return this.point.Observations;
			}
		}

		public virtual double Std
		{
			set
			{
				this.sigma0 = (this.sigma0 <= 0 && value>0)?value:this.sigma0;
				this.sigma = value > 0 ? value : this.sigma;
			}
			get
			{
				return this.sigma;
			}
		}


		public virtual double StdApriori
		{
			get
			{
				return this.sigma0;
			}
			set
			{
				if (value > 0)
				{
					this.sigma0 = value;
				}
			}
		}


		public virtual int RowInJacobiMatrix
		{
			get
			{
				return this.rowInJacobiMatrix;
			}
			set
			{
				this.rowInJacobiMatrix = value;
			}
		}


		public virtual double Redundancy
		{
			get
			{
				return redundancy;
			}
			set
			{
				this.redundancy = value;
			}
		}


		public virtual double Omega
		{
			set
			{
				this.omega = value;
			}
			get
			{
				return this.omega;
			}
		}


		public virtual double GrossError
		{
			get
			{
				return nabla;
			}
			set
			{
				this.nabla = value;
			}
		}


		public virtual double MinimalDetectableBias
		{
			get
			{
				return mdb;
			}
			set
			{
				this.mdb = value;
			}
		}


		public virtual double MaximumTolerableBias
		{
			get
			{
				return this.mtb;
			}
			set
			{
				this.mtb = value;
			}
		}


		public virtual double Confidence
		{
			get
			{
				return confidence;
			}
			set
			{
				this.confidence = value;
			}
		}


		public override string ToString()
		{
			return "VerticalDeflection [point=" + point + ", value0=" + value0 + ", value=" + value + "]";
		}
	}
}