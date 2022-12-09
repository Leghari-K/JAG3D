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
	using ObjectBinding = javafx.beans.binding.ObjectBinding;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class VarianceComponent
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			applyAposterioriVarianceOfUnitWeight = new SimpleObjectProperty<bool>(this, "applyAposterioriVarianceOfUnitWeight", true);
			variance0 = new SimpleObjectProperty<double>(this, "variance0", 1.0);
			redundancy = new SimpleObjectProperty<double>(this, "redundancy", 0.0);
			omega = new SimpleObjectProperty<double>(this, "omega", 0.0);
			significant = new SimpleObjectProperty<bool>(this, "significant", false);
		}

		private ObjectProperty<bool> applyAposterioriVarianceOfUnitWeight;
		private ObjectProperty<double> variance0;
		private ObjectProperty<double> redundancy;
		private ObjectProperty<double> omega;
		private ObjectProperty<bool> significant;
		private ObjectBinding<double> variance;

		public VarianceComponent()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.variance = new ObjectBindingAnonymousInnerClass(this);
		}

		private class ObjectBindingAnonymousInnerClass : ObjectBinding<double>
		{
			private readonly VarianceComponent outerInstance;

			public ObjectBindingAnonymousInnerClass(VarianceComponent outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.redundancy, outerInstance.omega);
			}


					protected internal override double? computeValue()
					{
						return outerInstance.omega.get() > 0 && outerInstance.redundancy.get() > 0 ? outerInstance.omega.get() / outerInstance.redundancy.get() : outerInstance.variance0.get();
					}
		}

		public virtual double Variance0
		{
			get
			{
				return this.variance0.get();
			}
			set
			{
				this.variance0.set(value);
			}
		}


		public virtual double Variance
		{
			get
			{
				return this.variance.get();
			}
		}

		public virtual double Omega
		{
			get
			{
				return this.omega.get();
			}
			set
			{
				this.omega.set(value);
			}
		}


		public virtual double Redundancy
		{
			get
			{
				return this.redundancy.get();
			}
			set
			{
				this.redundancy.set(value);
			}
		}


		public virtual bool Significant
		{
			set
			{
				this.significant.set(value);
			}
			get
			{
				return this.significant.get();
			}
		}


		public virtual bool ApplyAposterioriVarianceOfUnitWeight
		{
			get
			{
				return this.applyAposterioriVarianceOfUnitWeight.get();
			}
			set
			{
				this.applyAposterioriVarianceOfUnitWeight.set(value);
			}
		}


		public virtual ObjectProperty<double> variance0Property()
		{
			return this.variance0;
		}

		public virtual ObjectBinding<double> varianceProperty()
		{
			return this.variance;
		}

		public virtual ObjectProperty<double> omegaProperty()
		{
			return this.omega;
		}

		public virtual ObjectProperty<double> redundancyProperty()
		{
			return this.redundancy;
		}

		public virtual ObjectProperty<bool> applyAposterioriVarianceOfUnitWeightProperty()
		{
			return this.applyAposterioriVarianceOfUnitWeight;
		}

		public virtual ObjectProperty<bool> significantProperty()
		{
			return this.significant;
		}
	}

}