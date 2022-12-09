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

namespace org.applied_geodesy.jag3d.ui.table.row
{
	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class ObservationRow : GroupRow
	{
		private ObjectProperty<string> startPointName = new SimpleObjectProperty<string>();
		private ObjectProperty<string> endPointName = new SimpleObjectProperty<string>();

		private ObjectProperty<double> influenceOnNetworkDistortion = new SimpleObjectProperty<double>();

		private ObjectProperty<double> omega = new SimpleObjectProperty<double>();

		private ObjectProperty<double> testStatisticApriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> testStatisticAposteriori = new SimpleObjectProperty<double>();

		private ObjectProperty<double> pValueApriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> pValueAposteriori = new SimpleObjectProperty<double>();

		private BooleanProperty significant = new SimpleBooleanProperty(false);
		private BooleanProperty enable = new SimpleBooleanProperty(true);

		public virtual ObjectProperty<string> startPointNameProperty()
		{
			return this.startPointName;
		}

		public virtual string StartPointName
		{
			get
			{
				return this.startPointNameProperty().get();
			}
			set
			{
				this.startPointNameProperty().set(value);
			}
		}


		public virtual ObjectProperty<string> endPointNameProperty()
		{
			return this.endPointName;
		}

		public virtual string EndPointName
		{
			get
			{
				return this.endPointNameProperty().get();
			}
			set
			{
				this.endPointNameProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> influenceOnNetworkDistortionProperty()
		{
			return this.influenceOnNetworkDistortion;
		}

		public virtual double? InfluenceOnNetworkDistortion
		{
			get
			{
				return this.influenceOnNetworkDistortionProperty().get();
			}
			set
			{
				this.influenceOnNetworkDistortionProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> omegaProperty()
		{
			return this.omega;
		}

		public virtual double? Omega
		{
			get
			{
				return this.omegaProperty().get();
			}
			set
			{
				this.omegaProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> testStatisticAprioriProperty()
		{
			return this.testStatisticApriori;
		}

		public virtual double? TestStatisticApriori
		{
			get
			{
				return this.testStatisticAprioriProperty().get();
			}
			set
			{
				this.testStatisticAprioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> testStatisticAposterioriProperty()
		{
			return this.testStatisticAposteriori;
		}

		public virtual double? TestStatisticAposteriori
		{
			get
			{
				return this.testStatisticAposterioriProperty().get();
			}
			set
			{
				this.testStatisticAposterioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> pValueAprioriProperty()
		{
			return this.pValueApriori;
		}

		public virtual double? PValueApriori
		{
			get
			{
				return this.pValueAprioriProperty().get();
			}
			set
			{
				this.pValueAprioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> pValueAposterioriProperty()
		{
			return this.pValueAposteriori;
		}

		public virtual double? PValueAposteriori
		{
			get
			{
				return this.pValueAposterioriProperty().get();
			}
			set
			{
				this.pValueAposterioriProperty().set(value);
			}
		}


		public virtual BooleanProperty enableProperty()
		{
			return this.enable;
		}

		public virtual bool Enable
		{
			get
			{
				return this.enableProperty().get();
			}
			set
			{
				this.enableProperty().set(value);
			}
		}


		public BooleanProperty significantProperty()
		{
			return this.significant;
		}

		public bool Significant
		{
			get
			{
				return this.significantProperty().get();
			}
			set
			{
				this.significantProperty().set(value);
			}
		}

	}

}