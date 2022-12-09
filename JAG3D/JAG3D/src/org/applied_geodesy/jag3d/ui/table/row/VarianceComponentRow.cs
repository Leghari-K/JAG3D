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
	using VarianceComponentType = org.applied_geodesy.adjustment.network.VarianceComponentType;

	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class VarianceComponentRow : Row
	{
		private bool InstanceFieldsInitialized = false;

		public VarianceComponentRow()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			varianceComponentType = new SimpleObjectProperty<VarianceComponentType>(this, "varianceComponentType");
			name = new SimpleObjectProperty<string>(this, "name");
			numberOfObservations = new SimpleObjectProperty<int>(this, "numberOfObservations");
			redundancy = new SimpleObjectProperty<double>(this, "redundancy");
			omega = new SimpleObjectProperty<double>(this, "omega");
			sigma2aposteriori = new SimpleObjectProperty<double>(this, "sigma2aposteriori");
			significant = new SimpleBooleanProperty(this, "significant");
		}

		private ObjectProperty<VarianceComponentType> varianceComponentType;
		private ObjectProperty<string> name;
		private ObjectProperty<int> numberOfObservations;
		private ObjectProperty<double> redundancy;
		private ObjectProperty<double> omega;
		private ObjectProperty<double> sigma2aposteriori;
		private BooleanProperty significant;

		public ObjectProperty<VarianceComponentType> varianceComponentTypeProperty()
		{
			return this.varianceComponentType;
		}

		public VarianceComponentType VarianceComponentType
		{
			get
			{
				return this.varianceComponentTypeProperty().get();
			}
			set
			{
				this.varianceComponentTypeProperty().set(value);
			}
		}


		public ObjectProperty<string> nameProperty()
		{
			return this.name;
		}

		public string Name
		{
			get
			{
				return this.nameProperty().get();
			}
			set
			{
				this.nameProperty().set(value);
			}
		}


		public ObjectProperty<int> numberOfObservationsProperty()
		{
			return this.numberOfObservations;
		}

		public int? NumberOfObservations
		{
			get
			{
				return this.numberOfObservationsProperty().get();
			}
			set
			{
				this.numberOfObservationsProperty().set(value);
			}
		}


		public ObjectProperty<double> redundancyProperty()
		{
			return this.redundancy;
		}

		public double? Redundancy
		{
			get
			{
				return this.redundancyProperty().get();
			}
			set
			{
				this.redundancyProperty().set(value);
			}
		}


		public ObjectProperty<double> omegaProperty()
		{
			return this.omega;
		}

		public double? Omega
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


		public ObjectProperty<double> sigma2aposterioriProperty()
		{
			return this.sigma2aposteriori;
		}

		public double? Sigma2aposteriori
		{
			get
			{
				return this.sigma2aposterioriProperty().get();
			}
			set
			{
				this.sigma2aposterioriProperty().set(value);
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