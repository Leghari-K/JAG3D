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

namespace org.applied_geodesy.adjustment.statistic
{
	public class TestStatisticParameterSet
	{
		private double quantile, ncp, alpha, beta, logP;
		private readonly double f1, f2;
		private readonly bool isGlobalTestStatistic;

		public TestStatisticParameterSet(double f1, double f2, bool isGlobalTestStatistic)
		{
			this.f1 = f1;
			this.f2 = f2;
			this.isGlobalTestStatistic = isGlobalTestStatistic;
		}
		public TestStatisticParameterSet(double f1, double f2) : this(f1, f2, false)
		{
		}
		public virtual double Quantile
		{
			get
			{
				return this.quantile;
			}
			set
			{
				this.quantile = value;
			}
		}
		public virtual double NoncentralityParameter
		{
			get
			{
				return this.ncp;
			}
			set
			{
				this.ncp = value;
			}
		}
		public virtual double ProbabilityValue
		{
			get
			{
				return this.alpha;
			}
			set
			{
				this.alpha = value;
			}
		}
		public virtual double PowerOfTest
		{
			get
			{
				return this.beta;
			}
			set
			{
				this.beta = value;
			}
		}
		public virtual double NumeratorDof
		{
			get
			{
				return this.f1;
			}
		}
		public virtual double DenominatorDof
		{
			get
			{
				return this.f2;
			}
		}
		public virtual double LogarithmicProbabilityValue
		{
			set
			{
				this.logP = value;
			}
			get
			{
				return this.logP;
			}
		}
		public virtual bool GlobalTestStatistic
		{
			get
			{
				return this.isGlobalTestStatistic;
			}
		}
		public override string ToString()
		{
			return "TestStatisticParameterSet [quantile=" + quantile + ", ncp=" + ncp + ", alpha=" + alpha + ", beta=" + beta + ", logP=" + logP + ", f1=" + f1 + ", f2=" + f2 + "]";
		}
	}
}