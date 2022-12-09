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

	public class CongruenceAnalysisRow : GroupRow
	{
		private ObjectProperty<string> nameInReferenceEpoch = new SimpleObjectProperty<string>();
		private ObjectProperty<string> nameInControlEpoch = new SimpleObjectProperty<string>();

		private ObjectProperty<double> testStatisticApriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> testStatisticAposteriori = new SimpleObjectProperty<double>();

		private ObjectProperty<double> pValueApriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> pValueAposteriori = new SimpleObjectProperty<double>();

		private BooleanProperty significant = new SimpleBooleanProperty(false);
		private BooleanProperty enable = new SimpleBooleanProperty(true);

		private ObjectProperty<double> xAposteriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> yAposteriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> zAposteriori = new SimpleObjectProperty<double>();

		private ObjectProperty<double> sigmaXaposteriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> sigmaYaposteriori = new SimpleObjectProperty<double>();
		private ObjectProperty<double> sigmaZaposteriori = new SimpleObjectProperty<double>();

		private ObjectProperty<double> minimalDetectableBiasX = new SimpleObjectProperty<double>();
		private ObjectProperty<double> minimalDetectableBiasY = new SimpleObjectProperty<double>();
		private ObjectProperty<double> minimalDetectableBiasZ = new SimpleObjectProperty<double>();

		private ObjectProperty<double> grossErrorX = new SimpleObjectProperty<double>();
		private ObjectProperty<double> grossErrorY = new SimpleObjectProperty<double>();
		private ObjectProperty<double> grossErrorZ = new SimpleObjectProperty<double>();

		private ObjectProperty<double> confidenceA = new SimpleObjectProperty<double>();
		private ObjectProperty<double> confidenceB = new SimpleObjectProperty<double>();
		private ObjectProperty<double> confidenceC = new SimpleObjectProperty<double>();

		private ObjectProperty<double> confidenceAlpha = new SimpleObjectProperty<double>();
		private ObjectProperty<double> confidenceBeta = new SimpleObjectProperty<double>();
		private ObjectProperty<double> confidenceGamma = new SimpleObjectProperty<double>();

		public ObjectProperty<string> nameInReferenceEpochProperty()
		{
			return this.nameInReferenceEpoch;
		}

		public string NameInReferenceEpoch
		{
			get
			{
				return this.nameInReferenceEpochProperty().get();
			}
			set
			{
				this.nameInReferenceEpochProperty().set(value);
			}
		}


		public ObjectProperty<string> nameInControlEpochProperty()
		{
			return this.nameInControlEpoch;
		}

		public string NameInControlEpoch
		{
			get
			{
				return this.nameInControlEpochProperty().get();
			}
			set
			{
				this.nameInControlEpochProperty().set(value);
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


		public BooleanProperty enableProperty()
		{
			return this.enable;
		}

		public bool Enable
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


		public ObjectProperty<double> zAposterioriProperty()
		{
			return this.zAposteriori;
		}

		public double? ZAposteriori
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


		public ObjectProperty<double> sigmaZaposterioriProperty()
		{
			return this.sigmaZaposteriori;
		}

		public double? SigmaZaposteriori
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


		public ObjectProperty<double> minimalDetectableBiasZProperty()
		{
			return this.minimalDetectableBiasZ;
		}

		public double? MinimalDetectableBiasZ
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


		public ObjectProperty<double> grossErrorZProperty()
		{
			return this.grossErrorZ;
		}

		public double? GrossErrorZ
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


		public ObjectProperty<double> confidenceBProperty()
		{
			return this.confidenceB;
		}

		public double? ConfidenceB
		{
			get
			{
				return this.confidenceBProperty().get();
			}
			set
			{
				this.confidenceBProperty().set(value);
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


		public ObjectProperty<double> confidenceAlphaProperty()
		{
			return this.confidenceAlpha;
		}

		public double? ConfidenceAlpha
		{
			get
			{
				return this.confidenceAlphaProperty().get();
			}
			set
			{
				this.confidenceAlphaProperty().set(value);
			}
		}


		public ObjectProperty<double> confidenceBetaProperty()
		{
			return this.confidenceBeta;
		}

		public double? ConfidenceBeta
		{
			get
			{
				return this.confidenceBetaProperty().get();
			}
			set
			{
				this.confidenceBetaProperty().set(value);
			}
		}


		public ObjectProperty<double> confidenceGammaProperty()
		{
			return this.confidenceGamma;
		}

		public double? ConfidenceGamma
		{
			get
			{
				return this.confidenceGammaProperty().get();
			}
			set
			{
				this.confidenceGammaProperty().set(value);
			}
		}


		public static CongruenceAnalysisRow cloneRowApriori(CongruenceAnalysisRow row)
		{
			CongruenceAnalysisRow clone = new CongruenceAnalysisRow();

			clone.Id = -1;
			clone.GroupId = row.GroupId;
			clone.Enable = row.Enable;

			clone.NameInReferenceEpoch = row.NameInReferenceEpoch;
			clone.NameInControlEpoch = row.NameInControlEpoch;

			return clone;
		}

		public static CongruenceAnalysisRow scan(string str)
		{
			Scanner scanner = new Scanner(str.Trim());
			try
			{
				scanner.useLocale(Locale.ENGLISH);
				string startPointName = "", endPointName = "";

				CongruenceAnalysisRow row = new CongruenceAnalysisRow();
				// reference epoch
				if (!scanner.hasNext())
				{
					return null;
				}
				startPointName = scanner.next();

				// control epoch
				if (!scanner.hasNext())
				{
					return null;
				}
				endPointName = scanner.next();

				if (startPointName.Equals(endPointName))
				{
					return null;
				}

				row.NameInReferenceEpoch = startPointName;
				row.NameInControlEpoch = endPointName;

				return row;
			}
			finally
			{
				scanner.close();
			}
		}
	}
}