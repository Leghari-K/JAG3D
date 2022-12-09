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
	using DefaultValue = org.applied_geodesy.adjustment.DefaultValue;

	public class TestStatisticDefinition
	{
		private TestStatisticType testStatisticType;
		private double probabilityValue, powerOfTest;
		private bool familywiseErrorRate = false;

		public TestStatisticDefinition() : this(org.applied_geodesy.adjustment.statistic.TestStatisticType.BAARDA_METHOD, DefaultValue.ProbabilityValue, DefaultValue.PowerOfTest, false)
		{
		}

		public TestStatisticDefinition(TestStatisticType testStatisticType, double probabilityValue, double powerOfTest, bool familywiseErrorRate)
		{
			this.testStatisticType = testStatisticType == null ? org.applied_geodesy.adjustment.statistic.TestStatisticType.BAARDA_METHOD : testStatisticType;
			this.probabilityValue = probabilityValue > 0 && probabilityValue < 1 ? probabilityValue : DefaultValue.ProbabilityValue;
			this.powerOfTest = powerOfTest > 0 && powerOfTest < 1 ? powerOfTest : DefaultValue.PowerOfTest;
			this.familywiseErrorRate = familywiseErrorRate;
		}

		public virtual TestStatisticType TestStatisticType
		{
			get
			{
				return this.testStatisticType;
			}
			set
			{
				this.testStatisticType = value;
			}
		}

		public virtual double ProbabilityValue
		{
			get
			{
				return this.probabilityValue;
			}
			set
			{
				this.probabilityValue = value;
			}
		}

		public virtual double PowerOfTest
		{
			get
			{
				return this.powerOfTest;
			}
			set
			{
				this.powerOfTest = value;
			}
		}

		public virtual bool FamilywiseErrorRate
		{
			get
			{
				return this.familywiseErrorRate;
			}
			set
			{
				this.familywiseErrorRate = value;
			}
		}





		public override string ToString()
		{
			return "TestStatisticDefinition [testStatisticType=" + testStatisticType + ", probabilityValue=" + probabilityValue + ", powerOfTest=" + powerOfTest + ", familywiseErrorRate=" + familywiseErrorRate + "]";
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			long temp;
			temp = System.BitConverter.DoubleToInt64Bits(probabilityValue);
			result = prime * result + (int)(temp ^ ((long)((ulong)temp >> 32)));
			temp = System.BitConverter.DoubleToInt64Bits(powerOfTest);
			result = prime * result + (int)(temp ^ ((long)((ulong)temp >> 32)));
			result = prime * result + (familywiseErrorRate ? 1231 : 1237);
			result = prime * result + ((testStatisticType == null) ? 0 : testStatisticType.GetHashCode());
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			TestStatisticDefinition other = (TestStatisticDefinition) obj;
			if (System.BitConverter.DoubleToInt64Bits(probabilityValue) != System.BitConverter.DoubleToInt64Bits(other.probabilityValue))
			{
				return false;
			}
			if (System.BitConverter.DoubleToInt64Bits(powerOfTest) != System.BitConverter.DoubleToInt64Bits(other.powerOfTest))
			{
				return false;
			}
			if (familywiseErrorRate != other.familywiseErrorRate)
			{
				return false;
			}
			if (testStatisticType != other.testStatisticType)
			{
				return false;
			}
			return true;
		}
	}

}