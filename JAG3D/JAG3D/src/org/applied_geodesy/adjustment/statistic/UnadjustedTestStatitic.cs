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

namespace org.applied_geodesy.adjustment.statistic
{
	public class UnadjustedTestStatitic : TestStatistic
	{
		private readonly double alpha, beta;

		public UnadjustedTestStatitic() : this(0.001, 0.8)
		{
		}

		public UnadjustedTestStatitic(double alpha, double beta) : base(TestStatisticType.NONE, alpha, beta)
		{
			this.beta = beta;
			this.alpha = alpha;
		}

		public override TestStatisticParameterSet[] adjustTestStatisticParameters(TestStatisticParameterSet[] testStatisticParameterSet)
		{
			int l = testStatisticParameterSet.Length;

			for (int i = 0; i < l; i++)
			{
				TestStatisticParameterSet parameter = testStatisticParameterSet[i];
				double n2 = parameter.NumeratorDof;
				double m2 = parameter.DenominatorDof;

				parameter.ProbabilityValue = this.alpha;
				parameter.PowerOfTest = this.beta;
				if (double.IsInfinity(m2))
				{
					double quantile = TestStatistic.getQuantile(n2, this.alpha);
					double ncp = TestStatistic.getNoncentralityParameter(n2, this.alpha, this.beta);
					double logP = TestStatistic.getLogarithmicProbabilityValue(quantile, n2);
					parameter.Quantile = quantile;
					parameter.NoncentralityParameter = ncp;
					parameter.LogarithmicProbabilityValue = logP;
				}
				else
				{
					double quantile = TestStatistic.getQuantile(n2, m2, this.alpha);
					double ncp = TestStatistic.getNoncentralityParameter(n2, m2, this.alpha, this.beta);
					double logP = TestStatistic.getLogarithmicProbabilityValue(quantile, n2, m2);
					parameter.Quantile = quantile;
					parameter.NoncentralityParameter = ncp;
					parameter.LogarithmicProbabilityValue = logP;
				}
			}
			return testStatisticParameterSet;
		}

		public static void Main(string[] args)
		{
			int dof = 10000000;

			UnadjustedTestStatitic bMeth = new UnadjustedTestStatitic(0.001, 0.8);

			TestStatisticParameterSet[] set = new TestStatisticParameterSet[]
			{
				new TestStatisticParameterSet(1, double.PositiveInfinity),
				new TestStatisticParameterSet(1, dof - 1),
				new TestStatisticParameterSet(2, double.PositiveInfinity),
				new TestStatisticParameterSet(2, dof - 2),
				new TestStatisticParameterSet(3, double.PositiveInfinity),
				new TestStatisticParameterSet(3, dof - 3),
				new TestStatisticParameterSet(dof, double.PositiveInfinity)
			};
			bMeth.adjustTestStatisticParameters(set);

			foreach (TestStatisticParameterSet s in set)
			{
				Console.WriteLine(s);
			}

		}

	}

}