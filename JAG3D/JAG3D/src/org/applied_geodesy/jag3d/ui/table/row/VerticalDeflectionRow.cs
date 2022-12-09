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

namespace org.applied_geodesy.jag3d.ui.table.row
{
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class VerticalDeflectionRow : GroupRow
	{
		private bool InstanceFieldsInitialized = false;

		public VerticalDeflectionRow()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			maximumTolerableBiasX = new SimpleObjectProperty<double>(this, "maximumTolerableBiasX");
			maximumTolerableBiasY = new SimpleObjectProperty<double>(this, "maximumTolerableBiasY");
		}

		private ObjectProperty<string> name = new SimpleObjectProperty<string>();

		/// <summary>
		///  Parameter * </summary>
		private ObjectProperty<double> xApriori = new SimpleObjectProperty<double>(0.0);
		private ObjectProperty<double> yApriori = new SimpleObjectProperty<double>(0.0);

		private ObjectProperty<double> sigmaXapriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> sigmaYapriori = new SimpleObjectProperty<double>();

		private ObjectProperty<double> xAposteriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> yAposteriori = new SimpleObjectProperty<double>();

		private ObjectProperty<double> sigmaXaposteriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> sigmaYaposteriori = new SimpleObjectProperty<double>();

		private ObjectProperty<double> minimalDetectableBiasX = new SimpleObjectProperty<double>();
		private ObjectProperty<double> minimalDetectableBiasY = new SimpleObjectProperty<double>();

		private ObjectProperty<double> maximumTolerableBiasX;
		private ObjectProperty<double> maximumTolerableBiasY;

		private ObjectProperty<double> residualX = new SimpleObjectProperty<double>();
		private ObjectProperty<double> residualY = new SimpleObjectProperty<double>();

		private ObjectProperty<double> redundancyX = new SimpleObjectProperty<double>();
		private ObjectProperty<double> redundancyY = new SimpleObjectProperty<double>();

		private ObjectProperty<double> grossErrorX = new SimpleObjectProperty<double>();
		private ObjectProperty<double> grossErrorY = new SimpleObjectProperty<double>();

		private ObjectProperty<double> testStatisticApriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> testStatisticAposteriori = new SimpleObjectProperty<double>();

		private ObjectProperty<double> pValueApriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> pValueAposteriori = new SimpleObjectProperty<double>();

		private ObjectProperty<double> confidenceA = new SimpleObjectProperty<double>();
		private ObjectProperty<double> confidenceC = new SimpleObjectProperty<double>();

		private ObjectProperty<double> omega = new SimpleObjectProperty<double>();

		private BooleanProperty significant = new SimpleBooleanProperty(false);
		private BooleanProperty enable = new SimpleBooleanProperty(true);

		public virtual BooleanProperty enableProperty()
		{
			return this.enable;
		}

		public virtual bool? Enable
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


		public virtual ObjectProperty<string> nameProperty()
		{
			return this.name;
		}

		public virtual string Name
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


		public ObjectProperty<double> xAprioriProperty()
		{
			return this.xApriori;
		}

		public double? XApriori
		{
			get
			{
				return this.xAprioriProperty().get();
			}
			set
			{
				this.xAprioriProperty().set(value);
			}
		}


		public ObjectProperty<double> yAprioriProperty()
		{
			return this.yApriori;
		}

		public double? YApriori
		{
			get
			{
				return this.yAprioriProperty().get();
			}
			set
			{
				this.yAprioriProperty().set(value);
			}
		}


		public ObjectProperty<double> sigmaXaprioriProperty()
		{
			return this.sigmaXapriori;
		}

		public double? SigmaXapriori
		{
			get
			{
				return this.sigmaXaprioriProperty().get();
			}
			set
			{
				this.sigmaXaprioriProperty().set(value);
			}
		}


		public ObjectProperty<double> sigmaYaprioriProperty()
		{
			return this.sigmaYapriori;
		}

		public double? SigmaYapriori
		{
			get
			{
				return this.sigmaYaprioriProperty().get();
			}
			set
			{
				this.sigmaYaprioriProperty().set(value);
			}
		}


		public ObjectProperty<double> xAposterioriProperty()
		{
			return this.xAposteriori;
		}

		public double? XAposteriori
		{
			get
			{
				return this.xAposterioriProperty().get();
			}
			set
			{
				this.xAposterioriProperty().set(value);
			}
		}


		public ObjectProperty<double> yAposterioriProperty()
		{
			return this.yAposteriori;
		}

		public double? YAposteriori
		{
			get
			{
				return this.yAposterioriProperty().get();
			}
			set
			{
				this.yAposterioriProperty().set(value);
			}
		}


		public ObjectProperty<double> sigmaXaposterioriProperty()
		{
			return this.sigmaXaposteriori;
		}

		public double? SigmaXaposteriori
		{
			get
			{
				return this.sigmaXaposterioriProperty().get();
			}
			set
			{
				this.sigmaXaposterioriProperty().set(value);
			}
		}


		public ObjectProperty<double> sigmaYaposterioriProperty()
		{
			return this.sigmaYaposteriori;
		}

		public double? SigmaYaposteriori
		{
			get
			{
				return this.sigmaYaposterioriProperty().get();
			}
			set
			{
				this.sigmaYaposterioriProperty().set(value);
			}
		}


		public ObjectProperty<double> minimalDetectableBiasXProperty()
		{
			return this.minimalDetectableBiasX;
		}

		public double? MinimalDetectableBiasX
		{
			get
			{
				return this.minimalDetectableBiasXProperty().get();
			}
			set
			{
				this.minimalDetectableBiasXProperty().set(value);
			}
		}


		public ObjectProperty<double> minimalDetectableBiasYProperty()
		{
			return this.minimalDetectableBiasY;
		}

		public double? MinimalDetectableBiasY
		{
			get
			{
				return this.minimalDetectableBiasYProperty().get();
			}
			set
			{
				this.minimalDetectableBiasYProperty().set(value);
			}
		}

		public virtual ObjectProperty<double> maximumTolerableBiasXProperty()
		{
			return this.maximumTolerableBiasX;
		}

		public virtual double? MaximumTolerableBiasX
		{
			get
			{
				return this.maximumTolerableBiasXProperty().get();
			}
			set
			{
				this.maximumTolerableBiasXProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> maximumTolerableBiasYProperty()
		{
			return this.maximumTolerableBiasY;
		}

		public virtual double? MaximumTolerableBiasY
		{
			get
			{
				return this.maximumTolerableBiasYProperty().get();
			}
			set
			{
				this.maximumTolerableBiasYProperty().set(value);
			}
		}



		public ObjectProperty<double> residualXProperty()
		{
			return this.residualX;
		}

		public double? ResidualX
		{
			get
			{
				return this.residualXProperty().get();
			}
			set
			{
				this.residualXProperty().set(value);
			}
		}


		public ObjectProperty<double> residualYProperty()
		{
			return this.residualY;
		}

		public double? ResidualY
		{
			get
			{
				return this.residualYProperty().get();
			}
			set
			{
				this.residualYProperty().set(value);
			}
		}


		public ObjectProperty<double> redundancyXProperty()
		{
			return this.redundancyX;
		}

		public double? RedundancyX
		{
			get
			{
				return this.redundancyXProperty().get();
			}
			set
			{
				this.redundancyXProperty().set(value);
			}
		}


		public ObjectProperty<double> redundancyYProperty()
		{
			return this.redundancyY;
		}

		public double? RedundancyY
		{
			get
			{
				return this.redundancyYProperty().get();
			}
			set
			{
				this.redundancyYProperty().set(value);
			}
		}


		public ObjectProperty<double> grossErrorXProperty()
		{
			return this.grossErrorX;
		}

		public double? GrossErrorX
		{
			get
			{
				return this.grossErrorXProperty().get();
			}
			set
			{
				this.grossErrorXProperty().set(value);
			}
		}


		public ObjectProperty<double> grossErrorYProperty()
		{
			return this.grossErrorY;
		}

		public double? GrossErrorY
		{
			get
			{
				return this.grossErrorYProperty().get();
			}
			set
			{
				this.grossErrorYProperty().set(value);
			}
		}


		public ObjectProperty<double> testStatisticAprioriProperty()
		{
			return this.testStatisticApriori;
		}

		public double? TestStatisticApriori
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


		public ObjectProperty<double> testStatisticAposterioriProperty()
		{
			return this.testStatisticAposteriori;
		}

		public double? TestStatisticAposteriori
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


		public ObjectProperty<double> pValueAprioriProperty()
		{
			return this.pValueApriori;
		}

		public double? PValueApriori
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


		public ObjectProperty<double> pValueAposterioriProperty()
		{
			return this.pValueAposteriori;
		}

		public double? PValueAposteriori
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


		public ObjectProperty<double> confidenceAProperty()
		{
			return this.confidenceA;
		}

		public double? ConfidenceA
		{
			get
			{
				return this.confidenceAProperty().get();
			}
			set
			{
				this.confidenceAProperty().set(value);
			}
		}


		public ObjectProperty<double> confidenceCProperty()
		{
			return this.confidenceC;
		}

		public double? ConfidenceC
		{
			get
			{
				return this.confidenceCProperty().get();
			}
			set
			{
				this.confidenceCProperty().set(value);
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



		public static VerticalDeflectionRow cloneRowApriori(VerticalDeflectionRow row)
		{
			VerticalDeflectionRow clone = new VerticalDeflectionRow();

			clone.Id = -1;
			clone.GroupId = row.GroupId;
			clone.Enable = row.Enable;

			clone.Name = row.Name;

			clone.XApriori = row.XApriori;
			clone.YApriori = row.YApriori;

			clone.SigmaXapriori = row.SigmaXapriori;
			clone.SigmaYapriori = row.SigmaYapriori;

			return clone;
		}

		public static VerticalDeflectionRow scan(string str)
		{
			FormatterOptions options = FormatterOptions.Instance;

			try
			{
				string[] data = str.Trim().Split("[\\s;]+", true);

				if (data.Length < 3)
				{
					return null;
				}

				VerticalDeflectionRow row = new VerticalDeflectionRow();

				// Name of point
				string name = data[0];
				double x = 0.0, y = 0.0;
				double sigmaY = 0.0, sigmaX = 0.0;

				row.Name = name;

				// Y 		
				y = options.convertAngleToModel(double.Parse(data[1].Replace(',', '.')));

				// X 		
				x = options.convertAngleToModel(double.Parse(data[2].Replace(',', '.')));

				// sigma X (or sigmaX/Y)
				if (data.Length < 4)
				{
					row.YApriori = y;
					row.XApriori = x;
					return row;
				}

				sigmaY = sigmaX = options.convertAngleToModel(double.Parse(data[3].Replace(',', '.')));

				// sigma Y
				if (data.Length < 5)
				{
					row.YApriori = y;
					row.XApriori = x;
					row.SigmaYapriori = sigmaY;
					row.SigmaXapriori = sigmaX;
					return row;
				}

				sigmaX = options.convertAngleToModel(double.Parse(data[4].Replace(',', '.')));

				row.YApriori = y;
				row.XApriori = x;
				row.SigmaYapriori = sigmaY;
				row.SigmaXapriori = sigmaX;

				return row;
			}
			catch (Exception)
			{
				// e.printStackTrace();
			}
			return null;
		}
	}

}