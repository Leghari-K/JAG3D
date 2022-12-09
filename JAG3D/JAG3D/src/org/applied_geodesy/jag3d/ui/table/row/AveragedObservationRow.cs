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
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ComponentType = org.applied_geodesy.adjustment.network.observation.ComponentType;
	using Direction = org.applied_geodesy.adjustment.network.observation.Direction;
	using FaceType = org.applied_geodesy.adjustment.network.observation.FaceType;
	using GNSSBaseline = org.applied_geodesy.adjustment.network.observation.GNSSBaseline;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using ZenithAngle = org.applied_geodesy.adjustment.network.observation.ZenithAngle;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class AveragedObservationRow : Row
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			startPointName = new SimpleObjectProperty<string>(this, "startPointName");
			endPointName = new SimpleObjectProperty<string>(this, "endPointName");
			observationType = new SimpleObjectProperty<ObservationType>(this, "observationType");
			componentType = new SimpleObjectProperty<ComponentType>(this, "componentType");
			value = new SimpleObjectProperty<double>(this, "value");
			grossError = new SimpleObjectProperty<double>(this, "grossError");
		}

		private ObjectProperty<string> startPointName;
		private ObjectProperty<string> endPointName;

		private ObjectProperty<ObservationType> observationType;
		private ObjectProperty<ComponentType> componentType;

		private ObjectProperty<double> value;
		private ObjectProperty<double> grossError;

		public AveragedObservationRow()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public AveragedObservationRow(Observation observation)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.StartPointName = observation.StartPoint.Name;
			this.EndPointName = observation.EndPoint.Name;

			ComponentType compType = null;
			ObservationType obsType = observation.ObservationType;

			double value = observation.ValueApriori;
			if (obsType == ObservationType.DIRECTION && ((Direction)observation).Face == FaceType.TWO)
			{
				value = MathExtension.MOD(value + Math.PI, 2.0 * Math.PI);
			}
			else if (obsType == ObservationType.ZENITH_ANGLE && ((ZenithAngle)observation).Face == FaceType.TWO)
			{
				value = MathExtension.MOD(2.0 * Math.PI - value, 2.0 * Math.PI);
			}
			else if (obsType == ObservationType.GNSS1D || obsType == ObservationType.GNSS2D || obsType == ObservationType.GNSS3D)
			{
				compType = ((GNSSBaseline)observation).Component;
			}

			this.ComponentType = compType;
			this.ObservationType = obsType;
			this.Value = value;
			this.GrossError = observation.GrossError;
		}

		public ObjectProperty<string> startPointNameProperty()
		{
			return this.startPointName;
		}

		public string StartPointName
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


		public ObjectProperty<string> endPointNameProperty()
		{
			return this.endPointName;
		}

		public string EndPointName
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


		public ObjectProperty<ObservationType> observationTypeProperty()
		{
			return this.observationType;
		}

		public ObservationType ObservationType
		{
			get
			{
				return this.observationTypeProperty().get();
			}
			set
			{
				this.observationTypeProperty().set(value);
			}
		}


		public ObjectProperty<double> valueProperty()
		{
			return this.value;
		}

		public double? Value
		{
			get
			{
				return this.valueProperty().get();
			}
			set
			{
				this.valueProperty().set(value);
			}
		}


		public ObjectProperty<double> grossErrorProperty()
		{
			return this.grossError;
		}

		public double? GrossError
		{
			get
			{
				return this.grossErrorProperty().get();
			}
			set
			{
				this.grossErrorProperty().set(value);
			}
		}


		public ObjectProperty<ComponentType> componentTypeProperty()
		{
			return this.componentType;
		}


		public ComponentType ComponentType
		{
			get
			{
				return this.componentTypeProperty().get();
			}
			set
			{
				this.componentTypeProperty().set(value);
			}
		}



	}

}