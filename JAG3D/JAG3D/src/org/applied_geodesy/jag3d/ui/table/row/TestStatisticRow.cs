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
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class TestStatisticRow : Row
	{
		private bool InstanceFieldsInitialized = false;

		public TestStatisticRow()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			numeratorDegreeOfFreedom = new SimpleObjectProperty<double>(this, "numeratorDegreeOfFreedom");
			denominatorDegreeOfFreedom = new SimpleObjectProperty<double>(this, "denominatorDegreeOfFreedom");
			powerOfTest = new SimpleObjectProperty<double>(this, "powerOfTest");
			probabilityValue = new SimpleObjectProperty<double>(this, "probabilityValue");
			noncentralityParameter = new SimpleObjectProperty<double>(this, "noncentralityParameter");
			pValue = new SimpleObjectProperty<double>(this, "pValue");
			quantile = new SimpleObjectProperty<double>(this, "quantile");
		}


		private ObjectProperty<double> numeratorDegreeOfFreedom;
		private ObjectProperty<double> denominatorDegreeOfFreedom;

		private ObjectProperty<double> powerOfTest;
		private ObjectProperty<double> probabilityValue;

		private ObjectProperty<double> noncentralityParameter;

		private ObjectProperty<double> pValue;
		private ObjectProperty<double> quantile;

		public ObjectProperty<double> numeratorDegreeOfFreedomProperty()
		{
			return this.numeratorDegreeOfFreedom;
		}

		public double? NumeratorDegreeOfFreedom
		{
			get
			{
				return this.numeratorDegreeOfFreedomProperty().get();
			}
			set
			{
				this.numeratorDegreeOfFreedomProperty().set(value);
			}
		}


		public ObjectProperty<double> denominatorDegreeOfFreedomProperty()
		{
			return this.denominatorDegreeOfFreedom;
		}

		public double? DenominatorDegreeOfFreedom
		{
			get
			{
				return this.denominatorDegreeOfFreedomProperty().get();
			}
			set
			{
				this.denominatorDegreeOfFreedomProperty().set(value);
			}
		}


		public ObjectProperty<double> powerOfTestProperty()
		{
			return this.powerOfTest;
		}

		public double? PowerOfTest
		{
			get
			{
				return this.powerOfTestProperty().get();
			}
			set
			{
				this.powerOfTestProperty().set(value);
			}
		}


		public ObjectProperty<double> probabilityValueProperty()
		{
			return this.probabilityValue;
		}

		public double? ProbabilityValue
		{
			get
			{
				return this.probabilityValueProperty().get();
			}
			set
			{
				this.probabilityValueProperty().set(value);
			}
		}


		public ObjectProperty<double> noncentralityParameterProperty()
		{
			return this.noncentralityParameter;
		}

		public double? NoncentralityParameter
		{
			get
			{
				return this.noncentralityParameterProperty().get();
			}
			set
			{
				this.noncentralityParameterProperty().set(value);
			}
		}


		public ObjectProperty<double> pValueProperty()
		{
			return this.pValue;
		}

		public double? PValue
		{
			get
			{
				return this.pValueProperty().get();
			}
			set
			{
				this.pValueProperty().set(value);
			}
		}


		public ObjectProperty<double> quantileProperty()
		{
			return this.quantile;
		}

		public double? Quantile
		{
			get
			{
				return this.quantileProperty().get();
			}
			set
			{
				this.quantileProperty().set(value);
			}
		}

	}

}