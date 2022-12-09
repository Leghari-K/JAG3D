using System;
using System.Collections.Generic;

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

	public class TestStatisticParameters
	{

		private class KeySet : IComparable<KeySet>
		{
			private readonly TestStatisticParameters outerInstance;

			public readonly double f1, f2;
			public KeySet(TestStatisticParameters outerInstance, double f1, double f2)
			{
				this.outerInstance = outerInstance;
				this.f1 = f1;
				this.f2 = f2;
			}
			public override int GetHashCode()
			{
				const int prime = 31;
				int result = 1;
				result = prime * result + OuterType.GetHashCode();
				long temp;
				temp = System.BitConverter.DoubleToInt64Bits(f1);
				result = prime * result + (int)(temp ^ ((long)((ulong)temp >> 32)));
				temp = System.BitConverter.DoubleToInt64Bits(f2);
				result = prime * result + (int)(temp ^ ((long)((ulong)temp >> 32)));
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
				KeySet other = (KeySet) obj;
				if (!OuterType.Equals(other.OuterType))
				{
					return false;
				}
				if (System.BitConverter.DoubleToInt64Bits(f1) != System.BitConverter.DoubleToInt64Bits(other.f1))
				{
					return false;
				}
				if (System.BitConverter.DoubleToInt64Bits(f2) != System.BitConverter.DoubleToInt64Bits(other.f2))
				{
					return false;
				}
				return true;
			}
			internal virtual TestStatisticParameters OuterType
			{
				get
				{
					return outerInstance;
				}
			}
			public virtual int CompareTo(KeySet keySet)
			{
				if (this.Equals(keySet))
				{
					return 0;
				}

				if (this.f1 < keySet.f1 || this.f1 == keySet.f1 && this.f2 > keySet.f2)
				{
					return -1;
				}

				if (this.f1 > keySet.f1 || this.f1 == keySet.f1 && this.f2 < keySet.f2)
				{
					return 1;
				}

				return 0;
			}
		}

		public TestStatistic testStatistic;
		private SortedDictionary<KeySet, TestStatisticParameterSet> @params = Collections.synchronizedSortedMap(new SortedDictionary<KeySet, TestStatisticParameterSet>());
		public TestStatisticParameters(TestStatistic testStatistic)
		{
			this.testStatistic = testStatistic;
		}

		public virtual TestStatistic TestStatistic
		{
			get
			{
				return this.testStatistic;
			}
		}

		public virtual TestStatisticParameterSet[] TestStatisticParameterSets
		{
			get
			{
				TestStatisticParameterSet[] arr = new TestStatisticParameterSet[this.@params.Count];
				int i = 0;
				foreach (TestStatisticParameterSet set in this.@params.Values)
				{
					arr[i++] = set;
				}
				return arr;
			}
		}

		public virtual TestStatisticParameterSet getTestStatisticParameter(double f1, double f2)
		{
			return this.getTestStatisticParameter(f1, f2, false);
		}

		public virtual TestStatisticParameterSet getTestStatisticParameter(double f1, double f2, bool isGlobalTestStatistic)
		{
			KeySet key = new KeySet(this, f1,f2);
			if (!this.@params.ContainsKey(key))
			{
				this.@params[key] = this.testStatistic.adjustTestStatisticParameter(new TestStatisticParameterSet(f1, f2, isGlobalTestStatistic));
			}
			return this.@params[key];
		}
	}

}