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

namespace org.applied_geodesy.adjustment.network
{
	public class VarianceComponent
	{
		private readonly VarianceComponentType type;
		private int numberOfObservations = 0;
		private double redundancy = 0, omega = 0;
		private double kGroup = double.PositiveInfinity;

		internal VarianceComponent(VarianceComponentType type)
		{
			this.type = type;
		}

		public virtual double Redundancy
		{
			get
			{
				return this.redundancy;
			}
			set
			{
				this.redundancy = value;
			}
		}


		public virtual double Omega
		{
			get
			{
				return this.omega;
			}
			set
			{
				this.omega = value;
			}
		}


		public virtual int NumberOfObservations
		{
			get
			{
				return this.numberOfObservations;
			}
			set
			{
				this.numberOfObservations = value;
			}
		}


		public virtual VarianceComponentType VarianceComponentType
		{
			get
			{
				return this.type;
			}
		}

		public virtual double VarianceFactorAposteriori
		{
			get
			{
				if (this.Redundancy > 0)
				{
					return this.Omega / this.Redundancy;
				}
				return 0.0;
			}
		}

		public virtual double KprioGroup
		{
			set
			{
				this.kGroup = value;
			}
			get
			{
				return this.kGroup;
			}
		}


		public virtual double TprioGroup
		{
			get
			{
				return this.redundancy > 0?this.omega / this.redundancy:1.0;
			}
		}

		public virtual bool Rejected
		{
			get
			{
				return this.TprioGroup > this.kGroup;
			}
		}

		public override string ToString()
		{
			return this.type + "\r\nr=" + this.Redundancy + "\r\nomega=" + this.Omega + "\r\nsigma=" + this.VarianceFactorAposteriori + "\r\nnumObs=" + this.NumberOfObservations;
		}
	}

}