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

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class GNSSObservationRow : ObservationRow
	{
		private bool InstanceFieldsInitialized = false;

		public GNSSObservationRow()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			xApriori = new SimpleObjectProperty<double>(this, "xApriori", 0.0);
			yApriori = new SimpleObjectProperty<double>(this, "yApriori", 0.0);
			zApriori = new SimpleObjectProperty<double>(this, "zApriori", 0.0);
			sigmaXapriori = new SimpleObjectProperty<double>(this, "sigmaXapriori");
			sigmaYapriori = new SimpleObjectProperty<double>(this, "sigmaYapriori");
			sigmaZapriori = new SimpleObjectProperty<double>(this, "sigmaZapriori");
			xAposteriori = new SimpleObjectProperty<double>(this, "xAposteriori");
			yAposteriori = new SimpleObjectProperty<double>(this, "yAposteriori");
			zAposteriori = new SimpleObjectProperty<double>(this, "zAposteriori");
			sigmaXaposteriori = new SimpleObjectProperty<double>(this, "sigmaXaposteriori");
			sigmaYaposteriori = new SimpleObjectProperty<double>(this, "sigmaYaposteriori");
			sigmaZaposteriori = new SimpleObjectProperty<double>(this, "sigmaZaposteriori");
			minimalDetectableBiasX = new SimpleObjectProperty<double>(this, "minimalDetectableBiasX");
			minimalDetectableBiasY = new SimpleObjectProperty<double>(this, "minimalDetectableBiasY");
			minimalDetectableBiasZ = new SimpleObjectProperty<double>(this, "minimalDetectableBiasZ");
			maximumTolerableBiasX = new SimpleObjectProperty<double>(this, "maximumTolerableBiasX");
			maximumTolerableBiasY = new SimpleObjectProperty<double>(this, "maximumTolerableBiasY");
			maximumTolerableBiasZ = new SimpleObjectProperty<double>(this, "maximumTolerableBiasZ");
			residualX = new SimpleObjectProperty<double>(this, "residualX");
			residualY = new SimpleObjectProperty<double>(this, "residualY");
			residualZ = new SimpleObjectProperty<double>(this, "residualZ");
			redundancyX = new SimpleObjectProperty<double>(this, "redundancyX");
			redundancyY = new SimpleObjectProperty<double>(this, "redundancyY");
			redundancyZ = new SimpleObjectProperty<double>(this, "redundancyZ");
			grossErrorX = new SimpleObjectProperty<double>(this, "grossErrorX");
			grossErrorY = new SimpleObjectProperty<double>(this, "grossErrorY");
			grossErrorZ = new SimpleObjectProperty<double>(this, "grossErrorZ");
			influenceOnPointPositionX = new SimpleObjectProperty<double>(this, "influenceOnPointPositionX");
			influenceOnPointPositionY = new SimpleObjectProperty<double>(this, "influenceOnPointPositionY");
			influenceOnPointPositionZ = new SimpleObjectProperty<double>(this, "influenceOnPointPositionZ");
		}

		private ObjectProperty<double> xApriori;
		private ObjectProperty<double> yApriori;
		private ObjectProperty<double> zApriori;

		private ObjectProperty<double> sigmaXapriori;
		private ObjectProperty<double> sigmaYapriori;
		private ObjectProperty<double> sigmaZapriori;

		private ObjectProperty<double> xAposteriori;
		private ObjectProperty<double> yAposteriori;
		private ObjectProperty<double> zAposteriori;

		private ObjectProperty<double> sigmaXaposteriori;
		private ObjectProperty<double> sigmaYaposteriori;
		private ObjectProperty<double> sigmaZaposteriori;

		private ObjectProperty<double> minimalDetectableBiasX;
		private ObjectProperty<double> minimalDetectableBiasY;
		private ObjectProperty<double> minimalDetectableBiasZ;

		private ObjectProperty<double> maximumTolerableBiasX;
		private ObjectProperty<double> maximumTolerableBiasY;
		private ObjectProperty<double> maximumTolerableBiasZ;

		private ObjectProperty<double> residualX;
		private ObjectProperty<double> residualY;
		private ObjectProperty<double> residualZ;

		private ObjectProperty<double> redundancyX;
		private ObjectProperty<double> redundancyY;
		private ObjectProperty<double> redundancyZ;

		private ObjectProperty<double> grossErrorX;
		private ObjectProperty<double> grossErrorY;
		private ObjectProperty<double> grossErrorZ;

		private ObjectProperty<double> influenceOnPointPositionX;
		private ObjectProperty<double> influenceOnPointPositionY;
		private ObjectProperty<double> influenceOnPointPositionZ;

		public virtual ObjectProperty<double> xAprioriProperty()
		{
			return this.xApriori;
		}

		public virtual double? XApriori
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


		public virtual ObjectProperty<double> yAprioriProperty()
		{
			return this.yApriori;
		}

		public virtual double? YApriori
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


		public virtual ObjectProperty<double> zAprioriProperty()
		{
			return this.zApriori;
		}

		public virtual double? ZApriori
		{
			get
			{
				return this.zAprioriProperty().get();
			}
			set
			{
				this.zAprioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> sigmaXaprioriProperty()
		{
			return this.sigmaXapriori;
		}

		public virtual double? SigmaXapriori
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


		public virtual ObjectProperty<double> sigmaYaprioriProperty()
		{
			return this.sigmaYapriori;
		}

		public virtual double? SigmaYapriori
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


		public virtual ObjectProperty<double> sigmaZaprioriProperty()
		{
			return this.sigmaZapriori;
		}

		public virtual double? SigmaZapriori
		{
			get
			{
				return this.sigmaZaprioriProperty().get();
			}
			set
			{
				this.sigmaZaprioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> xAposterioriProperty()
		{
			return this.xAposteriori;
		}

		public virtual double? XAposteriori
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


		public virtual ObjectProperty<double> yAposterioriProperty()
		{
			return this.yAposteriori;
		}

		public virtual double? YAposteriori
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


		public virtual ObjectProperty<double> zAposterioriProperty()
		{
			return this.zAposteriori;
		}

		public virtual double? ZAposteriori
		{
			get
			{
				return this.zAposterioriProperty().get();
			}
			set
			{
				this.zAposterioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> sigmaXaposterioriProperty()
		{
			return this.sigmaXaposteriori;
		}

		public virtual double? SigmaXaposteriori
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


		public virtual ObjectProperty<double> sigmaYaposterioriProperty()
		{
			return this.sigmaYaposteriori;
		}

		public virtual double? SigmaYaposteriori
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


		public virtual ObjectProperty<double> sigmaZaposterioriProperty()
		{
			return this.sigmaZaposteriori;
		}

		public virtual double? SigmaZaposteriori
		{
			get
			{
				return this.sigmaZaposterioriProperty().get();
			}
			set
			{
				this.sigmaZaposterioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> minimalDetectableBiasXProperty()
		{
			return this.minimalDetectableBiasX;
		}

		public virtual double? MinimalDetectableBiasX
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


		public virtual ObjectProperty<double> minimalDetectableBiasYProperty()
		{
			return this.minimalDetectableBiasY;
		}

		public virtual double? MinimalDetectableBiasY
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


		public virtual ObjectProperty<double> minimalDetectableBiasZProperty()
		{
			return this.minimalDetectableBiasZ;
		}

		public virtual double? MinimalDetectableBiasZ
		{
			get
			{
				return this.minimalDetectableBiasZProperty().get();
			}
			set
			{
				this.minimalDetectableBiasZProperty().set(value);
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


		public virtual ObjectProperty<double> maximumTolerableBiasZProperty()
		{
			return this.maximumTolerableBiasZ;
		}

		public virtual double? MaximumTolerableBiasZ
		{
			get
			{
				return this.maximumTolerableBiasZProperty().get();
			}
			set
			{
				this.maximumTolerableBiasZProperty().set(value);
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


		public ObjectProperty<double> residualZProperty()
		{
			return this.residualZ;
		}

		public double? ResidualZ
		{
			get
			{
				return this.residualZProperty().get();
			}
			set
			{
				this.residualZProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> redundancyXProperty()
		{
			return this.redundancyX;
		}

		public virtual double? RedundancyX
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


		public virtual ObjectProperty<double> redundancyYProperty()
		{
			return this.redundancyY;
		}

		public virtual double? RedundancyY
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


		public virtual ObjectProperty<double> redundancyZProperty()
		{
			return this.redundancyZ;
		}

		public virtual double? RedundancyZ
		{
			get
			{
				return this.redundancyZProperty().get();
			}
			set
			{
				this.redundancyZProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> grossErrorXProperty()
		{
			return this.grossErrorX;
		}

		public virtual double? GrossErrorX
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


		public virtual ObjectProperty<double> grossErrorYProperty()
		{
			return this.grossErrorY;
		}

		public virtual double? GrossErrorY
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


		public virtual ObjectProperty<double> grossErrorZProperty()
		{
			return this.grossErrorZ;
		}

		public virtual double? GrossErrorZ
		{
			get
			{
				return this.grossErrorZProperty().get();
			}
			set
			{
				this.grossErrorZProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> influenceOnPointPositionXProperty()
		{
			return this.influenceOnPointPositionX;
		}

		public virtual double? InfluenceOnPointPositionX
		{
			get
			{
				return this.influenceOnPointPositionXProperty().get();
			}
			set
			{
				this.influenceOnPointPositionXProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> influenceOnPointPositionYProperty()
		{
			return this.influenceOnPointPositionY;
		}

		public virtual double? InfluenceOnPointPositionY
		{
			get
			{
				return this.influenceOnPointPositionYProperty().get();
			}
			set
			{
				this.influenceOnPointPositionYProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> influenceOnPointPositionZProperty()
		{
			return this.influenceOnPointPositionZ;
		}

		public virtual double? InfluenceOnPointPositionZ
		{
			get
			{
				return this.influenceOnPointPositionZProperty().get();
			}
			set
			{
				this.influenceOnPointPositionZProperty().set(value);
			}
		}


		public static GNSSObservationRow cloneRowApriori(GNSSObservationRow row)
		{
			GNSSObservationRow clone = new GNSSObservationRow();

			clone.Id = -1;
			clone.GroupId = row.GroupId;
			clone.Enable = row.Enable;

			clone.StartPointName = row.StartPointName;
			clone.EndPointName = row.EndPointName;

			clone.XApriori = row.XApriori;
			clone.YApriori = row.YApriori;
			clone.ZApriori = row.ZApriori;

			clone.SigmaXapriori = row.SigmaXapriori;
			clone.SigmaYapriori = row.SigmaYapriori;
			clone.SigmaZapriori = row.SigmaZapriori;

			return clone;
		}

		public static GNSSObservationRow scan(string str, int dim)
		{
			if (dim == 1)
			{
				return GNSSObservationRow.scan1D(str);
			}
			else if (dim == 2)
			{
				return GNSSObservationRow.scan2D(str);
			}
			else if (dim == 3)
			{
				return GNSSObservationRow.scan3D(str);
			}
			return null;
		}

		private static GNSSObservationRow scan1D(string str)
		{
			FormatterOptions options = FormatterOptions.Instance;
			Scanner scanner = new Scanner(str.Trim());

			try
			{
				scanner.useLocale(Locale.ENGLISH);
				string startPointName = "", endPointName = "";
				double x = 0, y = 0, z = 0, sigmaZ = 0;
				GNSSObservationRow row = new GNSSObservationRow();

				// station
				if (!scanner.hasNext())
				{
					return null;
				}
				startPointName = scanner.next();

				// target
				if (!scanner.hasNext())
				{
					return null;
				}
				endPointName = scanner.next();

				if (startPointName.Equals(endPointName))
				{
					return null;
				}

				row.StartPointName = startPointName;
				row.EndPointName = endPointName;

				// Y (or Z) 		
				if (!scanner.hasNextDouble())
				{
					return null;
				}
				y = z = options.convertLengthToModel(scanner.nextDouble());

				// X (or sigma Z)
				if (!scanner.hasNextDouble())
				{
					row.ZApriori = z;
					return row;
				}
				x = sigmaZ = options.convertLengthToModel(scanner.nextDouble());

				// Z
				if (!scanner.hasNextDouble())
				{
					row.ZApriori = z;
					row.SigmaZapriori = sigmaZ;
					return row;
				}
				z = options.convertLengthToModel(scanner.nextDouble());

				// sigma Z
				if (!scanner.hasNextDouble())
				{
					row.XApriori = x;
					row.YApriori = y;
					row.ZApriori = z;
					return row;
				}

				sigmaZ = options.convertLengthToModel(scanner.nextDouble());
				row.XApriori = x;
				row.YApriori = y;
				row.ZApriori = z;
				row.SigmaZapriori = sigmaZ;

				return row;
			}
			finally
			{
				scanner.close();
			}
		}

		private static GNSSObservationRow scan2D(string str)
		{
			FormatterOptions options = FormatterOptions.Instance;
			Scanner scanner = new Scanner(str.Trim());
			try
			{
				scanner.useLocale(Locale.ENGLISH);
				string startPointName = "", endPointName = "";
				double x = 0, y = 0, sigmaX = 0, sigmaY = 0;
				GNSSObservationRow row = new GNSSObservationRow();

				// station
				if (!scanner.hasNext())
				{
					return null;
				}
				startPointName = scanner.next();

				// target
				if (!scanner.hasNext())
				{
					return null;
				}
				endPointName = scanner.next();

				if (startPointName.Equals(endPointName))
				{
					return null;
				}

				row.StartPointName = startPointName;
				row.EndPointName = endPointName;

				// Y 		
				if (!scanner.hasNextDouble())
				{
					return null;
				}
				y = options.convertLengthToModel(scanner.nextDouble());

				// X
				if (!scanner.hasNextDouble())
				{
					return null;
				}
				x = options.convertLengthToModel(scanner.nextDouble());

				// sigma Y (and X)
				if (!scanner.hasNextDouble())
				{
					row.XApriori = x;
					row.YApriori = y;
					return row;
				}
				sigmaX = sigmaY = options.convertLengthToModel(scanner.nextDouble());

				// sigma Y
				if (!scanner.hasNextDouble())
				{
					row.XApriori = x;
					row.YApriori = y;
					row.SigmaXapriori = sigmaX;
					row.SigmaYapriori = sigmaY;
					return row;

				}
				sigmaX = options.convertLengthToModel(scanner.nextDouble());

				row.XApriori = x;
				row.YApriori = y;
				row.SigmaXapriori = sigmaX;
				row.SigmaYapriori = sigmaY;
				return row;
			}
			finally
			{
				scanner.close();
			}
		}

		private static GNSSObservationRow scan3D(string str)
		{
			FormatterOptions options = FormatterOptions.Instance;
			Scanner scanner = new Scanner(str.Trim());
			try
			{
				scanner.useLocale(Locale.ENGLISH);
				string startPointName = "", endPointName = "";
				double x = 0, y = 0, z = 0, sigmaX = 0, sigmaY = 0, sigmaZ = 0;
				GNSSObservationRow row = new GNSSObservationRow();

				// station
				if (!scanner.hasNext())
				{
					return null;
				}
				startPointName = scanner.next();

				// target
				if (!scanner.hasNext())
				{
					return null;
				}
				endPointName = scanner.next();

				if (startPointName.Equals(endPointName))
				{
					return null;
				}

				row.StartPointName = startPointName;
				row.EndPointName = endPointName;

				// Y 		
				if (!scanner.hasNextDouble())
				{
					return null;
				}
				y = options.convertLengthToModel(scanner.nextDouble());

				// X
				if (!scanner.hasNextDouble())
				{
					return null;
				}
				x = options.convertLengthToModel(scanner.nextDouble());

				// Z
				if (!scanner.hasNextDouble())
				{
					return null;
				}
				z = options.convertLengthToModel(scanner.nextDouble());

				// sigma Y (and X,Z)
				if (!scanner.hasNextDouble())
				{
					row.XApriori = x;
					row.YApriori = y;
					row.ZApriori = z;
					return row;
				}
				sigmaY = sigmaX = sigmaZ = options.convertLengthToModel(scanner.nextDouble());

				// sigma X (or Z)
				if (!scanner.hasNextDouble())
				{
					row.XApriori = x;
					row.YApriori = y;
					row.ZApriori = z;
					row.SigmaXapriori = sigmaX;
					row.SigmaYapriori = sigmaY;
					row.SigmaZapriori = sigmaZ;
					return row;
				}

				sigmaX = sigmaZ = options.convertLengthToModel(scanner.nextDouble());

				// sigma Y (or Z)
				if (!scanner.hasNextDouble())
				{
					row.XApriori = x;
					row.YApriori = y;
					row.ZApriori = z;
					row.SigmaXapriori = sigmaY;
					row.SigmaYapriori = sigmaY;
					row.SigmaZapriori = sigmaZ;
					return row;
				}

				sigmaZ = options.convertLengthToModel(scanner.nextDouble());

				row.XApriori = x;
				row.YApriori = y;
				row.ZApriori = z;
				row.SigmaXapriori = sigmaX;
				row.SigmaYapriori = sigmaY;
				row.SigmaZapriori = sigmaZ;
				return row;
			}
			finally
			{
				scanner.close();
			}
		}
	}

}