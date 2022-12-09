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
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;

	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class AdditionalParameterRow : Row
	{
		private bool InstanceFieldsInitialized = false;

		public AdditionalParameterRow()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			valueAposteriori = new SimpleObjectProperty<double>(this, "valueAposteriori");
			grossError = new SimpleObjectProperty<double>(this, "grossError");
			minimalDetectableBias = new SimpleObjectProperty<double>(this, "minimalDetectableBias");
			parameterType = new SimpleObjectProperty<ParameterType>(this, "parameterType");
			sigmaAposteriori = new SimpleObjectProperty<double>(this, "sigmaAposteriori");
			confidence = new SimpleObjectProperty<double>(this, "confidence");
			testStatisticApriori = new SimpleObjectProperty<double>(this, "testStatisticApriori");
			testStatisticAposteriori = new SimpleObjectProperty<double>(this, "testStatisticAposteriori");
			pValueApriori = new SimpleObjectProperty<double>(this, "pValueApriori");
			pValueAposteriori = new SimpleObjectProperty<double>(this, "pValueAposteriori");
			significant = new SimpleBooleanProperty(this, "significant", false);
		}


		private ObjectProperty<double> valueAposteriori;

		private ObjectProperty<double> grossError;
		private ObjectProperty<double> minimalDetectableBias;

		private ObjectProperty<ParameterType> parameterType;

		private ObjectProperty<double> sigmaAposteriori;
		private ObjectProperty<double> confidence;

		private ObjectProperty<double> testStatisticApriori;
		private ObjectProperty<double> testStatisticAposteriori;
		private ObjectProperty<double> pValueApriori;
		private ObjectProperty<double> pValueAposteriori;

		private BooleanProperty significant;


		public virtual ObjectProperty<double> valueAposterioriProperty()
		{
			return this.valueAposteriori;
		}

		public virtual double? ValueAposteriori
		{
			get
			{
				return this.valueAposterioriProperty().get();
			}
			set
			{
				this.valueAposterioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> grossErrorProperty()
		{
			return this.grossError;
		}

		public virtual double? GrossError
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


		public virtual ObjectProperty<double> minimalDetectableBiasProperty()
		{
			return this.minimalDetectableBias;
		}

		public virtual double? MinimalDetectableBias
		{
			get
			{
				return this.minimalDetectableBiasProperty().get();
			}
			set
			{
				this.minimalDetectableBiasProperty().set(value);
			}
		}


		public virtual ObjectProperty<ParameterType> parameterTypeProperty()
		{
			return this.parameterType;
		}

		public virtual ParameterType ParameterType
		{
			get
			{
				return this.parameterTypeProperty().get();
			}
			set
			{
				this.parameterTypeProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> sigmaAposterioriProperty()
		{
			return this.sigmaAposteriori;
		}


		public virtual double? SigmaAposteriori
		{
			get
			{
				return this.sigmaAposterioriProperty().get();
			}
			set
			{
				this.sigmaAposterioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> confidenceProperty()
		{
			return this.confidence;
		}

		public virtual double? Confidence
		{
			get
			{
				return this.confidenceProperty().get();
			}
			set
			{
				this.confidenceProperty().set(value);
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


		public virtual BooleanProperty significantProperty()
		{
			return this.significant;
		}

		public virtual bool Significant
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