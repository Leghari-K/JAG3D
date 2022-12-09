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
	public class SidakTestStatistic : TestStatistic
	{
		private readonly double alphaGlobal, alphaLocal, beta;

		private readonly int numberOfIndependentHypothesis;
		private double ncp = -1;

		public SidakTestStatistic() : this(int.MaxValue, 0.001, 0.8, false)
		{
		}

		public SidakTestStatistic(int numberOfIndependentHypothesis, double alpha, double beta, bool isAlphaGlobal) : base(TestStatisticType.SIDAK, alpha, beta)
		{
			this.beta = beta;
			this.numberOfIndependentHypothesis = numberOfIndependentHypothesis;

			if (isAlphaGlobal && this.numberOfIndependentHypothesis > 0)
			{
				this.alphaGlobal = alpha;
				double d = 1.0 - alpha;
				this.alphaLocal = (1.0 - Math.Pow(d, 1.0 / this.numberOfIndependentHypothesis));
				this.ncp = TestStatistic.getNoncentralityParameter(this.numberOfIndependentHypothesis, alpha, this.beta);
			}
			else if (!isAlphaGlobal && this.numberOfIndependentHypothesis > 0)
			{
				this.alphaLocal = alpha;
				double d = 1.0 - alpha;
				this.alphaGlobal = (1.0 - Math.Pow(d, this.numberOfIndependentHypothesis));
				this.ncp = TestStatistic.getNoncentralityParameter(1.0, alpha, this.beta);
			}
			else
			{
				this.alphaGlobal = this.alphaLocal = alpha;
				this.ncp = TestStatistic.getNoncentralityParameter(1.0, alpha, this.beta);
			}
		}

		public override TestStatisticParameterSet[] adjustTestStatisticParameters(TestStatisticParameterSet[] testStatisticParameterSet)
		{
			for (int i = 0; i < testStatisticParameterSet.Length; i++)
			{
				TestStatisticParameterSet parameter = testStatisticParameterSet[i];
				double n2 = parameter.NumeratorDof;
				double m2 = parameter.DenominatorDof;

				double alpha = this.alphaLocal;

				if (parameter.GlobalTestStatistic || n2 >= this.numberOfIndependentHypothesis)
				{
					alpha = this.alphaGlobal;
				}

				parameter.NoncentralityParameter = this.ncp;
				parameter.ProbabilityValue = alpha;

				if (double.IsInfinity(m2))
				{
					double quantile = TestStatistic.getQuantile(n2, alpha);
					double beta = TestStatistic.getPowerOfTest(quantile, n2, this.ncp);
					double logP = TestStatistic.getLogarithmicProbabilityValue(quantile, n2);
					parameter.Quantile = quantile;
					parameter.PowerOfTest = beta;
					parameter.LogarithmicProbabilityValue = logP;
				}
				else
				{
					double quantile = TestStatistic.getQuantile(n2, m2, alpha);
					double beta = TestStatistic.getPowerOfTest(quantile, n2, m2, this.ncp);
					double logP = TestStatistic.getLogarithmicProbabilityValue(quantile, n2, m2);
					parameter.Quantile = quantile;
					parameter.PowerOfTest = beta;
					parameter.LogarithmicProbabilityValue = logP;
				}
			}
			return testStatisticParameterSet;
		}

		public static void Main(string[] args)
		{
			int numberOfIndependentHypothesis = 100;

			SidakTestStatistic testStatistic = new SidakTestStatistic(numberOfIndependentHypothesis, 0.001, 0.8, false);

			TestStatisticParameterSet[] set = new TestStatisticParameterSet[]
			{
				new TestStatisticParameterSet(1, double.PositiveInfinity),
				new TestStatisticParameterSet(1, numberOfIndependentHypothesis - 1),
				new TestStatisticParameterSet(2, double.PositiveInfinity),
				new TestStatisticParameterSet(2, numberOfIndependentHypothesis - 2),
				new TestStatisticParameterSet(3, double.PositiveInfinity),
				new TestStatisticParameterSet(3, numberOfIndependentHypothesis - 3),
				new TestStatisticParameterSet(numberOfIndependentHypothesis, double.PositiveInfinity)
			};
			testStatistic.adjustTestStatisticParameters(set);
			foreach (TestStatisticParameterSet s in set)
			{
				Console.WriteLine(s);
			}
		}
	}

}