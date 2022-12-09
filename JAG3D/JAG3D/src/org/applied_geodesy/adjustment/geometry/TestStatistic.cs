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

namespace org.applied_geodesy.adjustment.geometry
{
	using Constant = org.applied_geodesy.adjustment.Constant;

	using ObjectBinding = javafx.beans.binding.ObjectBinding;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class TestStatistic
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			varianceComponent = new SimpleObjectProperty<VarianceComponent>(this, "varianceComponent", new VarianceComponent());
			fisherTestNumerator = new SimpleObjectProperty<double>(this, "fisherTestNumerator", 0.0);
			degreeOfFreedom = new SimpleObjectProperty<int>(this, "degreeOfFreedom", 0);
		}

		private ObjectProperty<VarianceComponent> varianceComponent;

		private ObjectProperty<double> fisherTestNumerator;
		private ObjectProperty<int> degreeOfFreedom;

		private ObjectBinding<double> testStatisticApriori;
		private ObjectBinding<double> testStatisticAposteriori;

		private ObjectBinding<double> pValueApriori;
		private ObjectBinding<double> pValueAposteriori;

		public TestStatistic()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.testStatisticApriori = new ObjectBindingAnonymousInnerClass(this);

			this.pValueApriori = new ObjectBindingAnonymousInnerClass2(this);

			this.testStatisticAposteriori = new ObjectBindingAnonymousInnerClass3(this);

			this.pValueAposteriori = new ObjectBindingAnonymousInnerClass4(this);
		}

		private class ObjectBindingAnonymousInnerClass : ObjectBinding<double>
		{
			private readonly TestStatistic outerInstance;

			public ObjectBindingAnonymousInnerClass(TestStatistic outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.fisherTestNumerator, outerInstance.degreeOfFreedom);
			}


					protected internal override double? computeValue()
					{
						double sigma2aprio = outerInstance.varianceComponent.get().getVariance0();
						return outerInstance.degreeOfFreedom.get() > 0 ? Math.Abs(outerInstance.fisherTestNumerator.get() / (double)outerInstance.degreeOfFreedom.get() / sigma2aprio) : 0.0;
					}
		}

		private class ObjectBindingAnonymousInnerClass2 : ObjectBinding<double>
		{
			private readonly TestStatistic outerInstance;

			public ObjectBindingAnonymousInnerClass2(TestStatistic outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.testStatisticApriori, outerInstance.degreeOfFreedom);
			}


					protected internal override double? computeValue()
					{
						return org.applied_geodesy.adjustment.statistic.TestStatistic.getLogarithmicProbabilityValue(outerInstance.testStatisticApriori.get(), outerInstance.degreeOfFreedom.get());
					}
		}

		private class ObjectBindingAnonymousInnerClass3 : ObjectBinding<double>
		{
			private readonly TestStatistic outerInstance;

			public ObjectBindingAnonymousInnerClass3(TestStatistic outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.fisherTestNumerator, outerInstance.degreeOfFreedom, outerInstance.varianceComponent);
			}


					protected internal override double? computeValue()
					{
						if (outerInstance.degreeOfFreedom.get() == 0 || outerInstance.fisherTestNumerator.get() == 0 || !outerInstance.varianceComponent.get().isApplyAposterioriVarianceOfUnitWeight())
						{
							return 0.0;
						}

						double omega = outerInstance.varianceComponent.get().getOmega();
						double redundancy = outerInstance.varianceComponent.get().getRedundancy();

						double unbiasedVariance = (omega - outerInstance.fisherTestNumerator.get()) / (redundancy - (double)outerInstance.degreeOfFreedom.get());
						if (unbiasedVariance < Math.Sqrt(Constant.EPS) || redundancy - (double)outerInstance.degreeOfFreedom.get() <= 0)
						{
							return 0.0;
						}

						return outerInstance.degreeOfFreedom.get() > 0 ? Math.Abs(outerInstance.fisherTestNumerator.get() / (unbiasedVariance * (double)outerInstance.degreeOfFreedom.get())) : 0.0;
					}
		}

		private class ObjectBindingAnonymousInnerClass4 : ObjectBinding<double>
		{
			private readonly TestStatistic outerInstance;

			public ObjectBindingAnonymousInnerClass4(TestStatistic outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.testStatisticAposteriori, outerInstance.degreeOfFreedom);
			}


					protected internal override double? computeValue()
					{
						if (outerInstance.degreeOfFreedom.get() == 0 || outerInstance.fisherTestNumerator.get() == 0 || !outerInstance.varianceComponent.get().isApplyAposterioriVarianceOfUnitWeight())
						{
							return 0.0;
						}

						double redundancy = outerInstance.varianceComponent.get().getRedundancy();
						return org.applied_geodesy.adjustment.statistic.TestStatistic.getLogarithmicProbabilityValue(outerInstance.testStatisticApriori.get(), outerInstance.degreeOfFreedom.get(), redundancy - (double)outerInstance.degreeOfFreedom.get());
					}
		}

		public virtual VarianceComponent VarianceComponent
		{
			set
			{
				this.varianceComponent.set(value);
			}
		}

		public virtual double FisherTestNumerator
		{
			set
			{
				this.fisherTestNumerator.set(value);
			}
		}

		public virtual int DegreeOfFreedom
		{
			set
			{
				this.degreeOfFreedom.set(value);
			}
		}

		public virtual double PValueApriori
		{
			get
			{
				return this.pValueApriori.get();
			}
		}

		public virtual double PValueAposteriori
		{
			get
			{
				return this.pValueAposteriori.get();
			}
		}

		public virtual double TestStatisticApriori
		{
			get
			{
				//return this.degreeOfFreedom > 0 ? this.fisherTestNumerator / this.degreeOfFreedom : 0;
				return testStatisticApriori.get();
			}
		}

		public virtual double TestStatisticAposteriori
		{
			get
			{
		//		if (this.degreeOfFreedom == 0 || this.fisherTestNumerator == 0 ||
		//				!this.varianceComponent.isApplyAposterioriVarianceOfUnitWeight())
		//			return 0;
		//		
		//		double omega = this.varianceComponent.getOmega();
		//		double redundancy = this.varianceComponent.getRedundancy();
		//		
		//		double unbiasedVariance = (omega - this.fisherTestNumerator) / (redundancy - this.degreeOfFreedom);
		//		if (unbiasedVariance < Math.sqrt(Constant.EPS))
		//			return 0;
		//		
		//		return this.degreeOfFreedom > 0 ? this.fisherTestNumerator / (unbiasedVariance * this.degreeOfFreedom) : 0; 
				return testStatisticAposteriori.get();
			}
		}

		public virtual ObjectProperty<double> fisherTestNumeratorProperty()
		{
			return this.fisherTestNumerator;
		}

		public virtual ObjectProperty<int> degreeOfFreedomProperty()
		{
			return this.degreeOfFreedom;
		}

		public virtual ObjectProperty<VarianceComponent> varianceComponentProperty()
		{
			return this.varianceComponent;
		}

		public virtual ObjectBinding<double> testStatisticAprioriProperty()
		{
			return this.testStatisticApriori;
		}

		public virtual ObjectBinding<double> testStatisticAposterioriProperty()
		{
			return this.testStatisticAposteriori;
		}

		public virtual ObjectBinding<double> pValueAprioriProperty()
		{
			return this.pValueApriori;
		}

		public virtual ObjectBinding<double> pValueAposterioriProperty()
		{
			return this.pValueAposteriori;
		}
	}

}