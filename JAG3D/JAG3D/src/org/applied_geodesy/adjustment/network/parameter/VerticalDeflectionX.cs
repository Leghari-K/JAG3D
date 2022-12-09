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

namespace org.applied_geodesy.adjustment.network.parameter
{
	using Constant = org.applied_geodesy.adjustment.Constant;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public class VerticalDeflectionX : VerticalDeflection
	{
		private bool significant = false;
		private static readonly double ZERO = Math.Sqrt(Constant.EPS);
		private double nablaCoVarNable = 0.0, tPrio = 0.0, tPost = 0.0, pPrio = 0.0, pPost = 0.0;
		public VerticalDeflectionX(Point point) : base(point)
		{
		}

		public VerticalDeflectionX(Point point, double value) : base(point, value)
		{
		}

		public VerticalDeflectionX(Point point, double value, double std) : base(point, value, std)
		{
		}

		public override ParameterType ParameterType
		{
			get
			{
				return ParameterType.VERTICAL_DEFLECTION_X;
			}
		}

		public virtual bool Significant
		{
			set
			{
				this.significant = value;
			}
			get
			{
				return this.significant;
			}
		}


		public virtual double NablaCoVarNabla
		{
			set
			{
				if (value >= 0)
				{
					this.nablaCoVarNable = value;
				}
			}
		}

		public virtual double Tprio
		{
			set
			{
				this.tPrio = value;
			}
			get
			{
				return this.tPrio;
			}
		}

		public virtual double Tpost
		{
			set
			{
				this.tPost = value;
			}
			get
			{
				return this.tPost;
			}
		}

		public virtual double Pprio
		{
			set
			{
				this.pPrio = value;
			}
			get
			{
				return this.pPrio;
			}
		}

		public virtual double Ppost
		{
			set
			{
				this.pPost = value;
			}
			get
			{
				return this.pPost;
			}
		}





		public virtual void calcStochasticParameters(double sigma2apost, int redundancy, bool applyAposterioriVarianceOfUnitWeight)
		{
			// Bestimmung der Testgroessen
			double omega = sigma2apost * (double)redundancy;
			const int dim = 2;
			double sigma2apostDeflection = (redundancy - dim) > 0?(omega - this.nablaCoVarNable) / (redundancy - dim):0.0;

			this.Tprio = this.nablaCoVarNable / dim;
			this.Tpost = (applyAposterioriVarianceOfUnitWeight && sigma2apost > VerticalDeflectionX.ZERO) ? this.nablaCoVarNable / (dim * sigma2apostDeflection):0.0;
		}

		public override string ToString()
		{
			return "VerticalDeflectionX [point=" + this.Point + ", value0=" + this.Value0 + ", value=" + this.Value + "]";
		}
	}

}