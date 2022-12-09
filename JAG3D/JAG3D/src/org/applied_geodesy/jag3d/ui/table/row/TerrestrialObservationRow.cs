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

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class TerrestrialObservationRow : ObservationRow
	{
		private bool InstanceFieldsInitialized = false;

		public TerrestrialObservationRow()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			valueApriori = new SimpleObjectProperty<double>(this, "valueApriori", 0.0);
			valueAposteriori = new SimpleObjectProperty<double>(this, "valueAposteriori");
			distanceApriori = new SimpleObjectProperty<double>(this, "distanceApriori");
			instrumentHeight = new SimpleObjectProperty<double>(this, "instrumentHeight", 0.0);
			reflectorHeight = new SimpleObjectProperty<double>(this, "reflectorHeight", 0.0);
			sigmaApriori = new SimpleObjectProperty<double>(this, "sigmaApriori");
			sigmaAposteriori = new SimpleObjectProperty<double>(this, "sigmaAposteriori");
			residual = new SimpleObjectProperty<double>(this, "residual");
			redundancy = new SimpleObjectProperty<double>(this, "redundancy");
			grossError = new SimpleObjectProperty<double>(this, "grossError");
			influenceOnPointPosition = new SimpleObjectProperty<double>(this, "influenceOnPointPosition");
			minimalDetectableBias = new SimpleObjectProperty<double>(this, "minimalDetectableBias");
			maximumTolerableBias = new SimpleObjectProperty<double>(this, "maximumTolerableBias");
		}

		private ObjectProperty<double> valueApriori;
		private ObjectProperty<double> valueAposteriori;
		private ObjectProperty<double> distanceApriori;

		private ObjectProperty<double> instrumentHeight;
		private ObjectProperty<double> reflectorHeight;

		private ObjectProperty<double> sigmaApriori;
		private ObjectProperty<double> sigmaAposteriori;

		private ObjectProperty<double> residual;
		private ObjectProperty<double> redundancy;
		private ObjectProperty<double> grossError;
		private ObjectProperty<double> influenceOnPointPosition;
		private ObjectProperty<double> minimalDetectableBias;
		private ObjectProperty<double> maximumTolerableBias;

		public virtual ObjectProperty<double> valueAprioriProperty()
		{
			return this.valueApriori;
		}

		public virtual double? ValueApriori
		{
			get
			{
				return this.valueAprioriProperty().get();
			}
			set
			{
				this.valueAprioriProperty().set(value);
			}
		}


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


		public virtual ObjectProperty<double> distanceAprioriProperty()
		{
			return this.distanceApriori;
		}

		public virtual double? DistanceApriori
		{
			get
			{
				return this.distanceAprioriProperty().get();
			}
			set
			{
				this.distanceAprioriProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> instrumentHeightProperty()
		{
			return this.instrumentHeight;
		}

		public virtual double? InstrumentHeight
		{
			get
			{
				return this.instrumentHeightProperty().get();
			}
			set
			{
				this.instrumentHeightProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> reflectorHeightProperty()
		{
			return this.reflectorHeight;
		}

		public virtual double? ReflectorHeight
		{
			get
			{
				return this.reflectorHeightProperty().get();
			}
			set
			{
				this.reflectorHeightProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> sigmaAprioriProperty()
		{
			return this.sigmaApriori;
		}

		public virtual double? SigmaApriori
		{
			get
			{
				return this.sigmaAprioriProperty().get();
			}
			set
			{
				this.sigmaAprioriProperty().set(value);
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


		public ObjectProperty<double> residualProperty()
		{
			return this.residual;
		}

		public double? Residual
		{
			get
			{
				return this.residualProperty().get();
			}
			set
			{
				this.residualProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> redundancyProperty()
		{
			return this.redundancy;
		}

		public virtual double? Redundancy
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


		public virtual ObjectProperty<double> influenceOnPointPositionProperty()
		{
			return this.influenceOnPointPosition;
		}

		public virtual double? InfluenceOnPointPosition
		{
			get
			{
				return this.influenceOnPointPositionProperty().get();
			}
			set
			{
				this.influenceOnPointPositionProperty().set(value);
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


		public virtual ObjectProperty<double> maximumTolerableBiasProperty()
		{
			return this.maximumTolerableBias;
		}

		public virtual double? MaximumTolerableBias
		{
			get
			{
				return this.maximumTolerableBiasProperty().get();
			}
			set
			{
				this.maximumTolerableBiasProperty().set(value);
			}
		}


		public static TerrestrialObservationRow cloneRowApriori(TerrestrialObservationRow row)
		{
			TerrestrialObservationRow clone = new TerrestrialObservationRow();

			clone.Id = -1;
			clone.GroupId = row.GroupId;
			clone.Enable = row.Enable;

			clone.StartPointName = row.StartPointName;
			clone.EndPointName = row.EndPointName;

			clone.InstrumentHeight = row.InstrumentHeight;
			clone.ReflectorHeight = row.ReflectorHeight;

			clone.ValueApriori = row.ValueApriori;
			clone.SigmaApriori = row.SigmaApriori;
			clone.DistanceApriori = row.DistanceApriori;

			return clone;
		}

		public static TerrestrialObservationRow scan(string str, ObservationType type)
		{
			FormatterOptions options = FormatterOptions.Instance;
			Scanner scanner = new Scanner(str.Trim());
			try
			{
				scanner.useLocale(Locale.ENGLISH);
				string startPointName, endPointName;
				double ih = 0.0, th = 0.0;
				double value = 0.0;
				double sigma = 0.0;
				double distance = 0.0;

				TerrestrialObservationRow row = new TerrestrialObservationRow();
				// Startpunktnummer
				if (!scanner.hasNext())
				{
					return null;
				}
				startPointName = scanner.next();

				// Zielpunktnummer
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

				// Standpunkthoehe (oder bereits der Messwert) 		
				if (!scanner.hasNextDouble())
				{
					return null;
				}
				ih = value = scanner.nextDouble();
				ih = options.convertLengthToModel(ih);
				switch (type.innerEnumValue)
				{
				case ObservationType.InnerEnum.DIRECTION:
				case ObservationType.InnerEnum.ZENITH_ANGLE:
					value = options.convertAngleToModel(value);
					break;
				default:
					value = options.convertLengthToModel(value);
					break;
				}
				row.ValueApriori = value;
				if (type == ObservationType.HORIZONTAL_DISTANCE || type == ObservationType.SLOPE_DISTANCE)
				{
					row.DistanceApriori = value;
				}

				// Tafelhoehe (oder bereits die Standardabweichung)
				if (!scanner.hasNextDouble())
				{
					return row;
				}

				th = sigma = distance = scanner.nextDouble();
				th = options.convertLengthToModel(th);
				distance = options.convertLengthToModel(distance);
				switch (type.innerEnumValue)
				{
				case ObservationType.InnerEnum.DIRECTION:
				case ObservationType.InnerEnum.ZENITH_ANGLE:
					sigma = options.convertAngleToModel(sigma);
					break;
				default:
					sigma = options.convertLengthToModel(sigma);
					break;
				}

				if (distance < 1)
				{
					row.SigmaApriori = sigma;
					if (type != ObservationType.HORIZONTAL_DISTANCE && type != ObservationType.SLOPE_DISTANCE)
					{
						row.DistanceApriori = null;
					}
					else
					{
						row.DistanceApriori = value;
					}
				}
				else
				{
					row.SigmaApriori = null;
					if (type != ObservationType.HORIZONTAL_DISTANCE && type != ObservationType.SLOPE_DISTANCE)
					{
						row.DistanceApriori = distance;
					}
				}

				// Messwert
				if (!scanner.hasNextDouble())
				{
					return row;
				}

				value = scanner.nextDouble();
				switch (type.innerEnumValue)
				{
				case ObservationType.InnerEnum.DIRECTION:
				case ObservationType.InnerEnum.ZENITH_ANGLE:
					value = options.convertAngleToModel(value);
					break;
				default:
					value = options.convertLengthToModel(value);
					break;
				}

				row.InstrumentHeight = ih;
				row.ReflectorHeight = th;
				row.ValueApriori = value;
				row.SigmaApriori = null;
				if (type != ObservationType.HORIZONTAL_DISTANCE && type != ObservationType.SLOPE_DISTANCE)
				{
					row.DistanceApriori = null;
				}
				else
				{
					row.DistanceApriori = value;
				}

				// Standardabweichung
				if (!scanner.hasNextDouble())
				{
					return row;
				}

				sigma = distance = scanner.nextDouble();
				distance = options.convertLengthToModel(distance);
				switch (type.innerEnumValue)
				{
				case ObservationType.InnerEnum.DIRECTION:
				case ObservationType.InnerEnum.ZENITH_ANGLE:
					sigma = options.convertAngleToModel(sigma);
					break;
				default:
					sigma = options.convertLengthToModel(sigma);
					break;
				}

				if (distance < 1)
				{
					row.SigmaApriori = sigma;
					if (type != ObservationType.HORIZONTAL_DISTANCE && type != ObservationType.SLOPE_DISTANCE)
					{
						row.DistanceApriori = null;
					}
					else
					{
						row.DistanceApriori = value;
					}
				}
				else
				{
					row.SigmaApriori = null;
					if (type != ObservationType.HORIZONTAL_DISTANCE && type != ObservationType.SLOPE_DISTANCE)
					{
						row.DistanceApriori = distance;
					}
				}

				return row;
			}
			finally
			{
				scanner.close();
			}
		}
	}


}