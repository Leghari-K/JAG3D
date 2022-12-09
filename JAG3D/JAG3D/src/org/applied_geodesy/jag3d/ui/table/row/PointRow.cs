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

	public class PointRow : GroupRow
	{
		private bool InstanceFieldsInitialized = false;

		public PointRow()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			name = new SimpleObjectProperty<string>(this, "name");
			code = new SimpleObjectProperty<string>(this, "code", "0");
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
			omega = new SimpleObjectProperty<double>(this, "omega");
			testStatisticApriori = new SimpleObjectProperty<double>(this, "testStatisticApriori");
			testStatisticAposteriori = new SimpleObjectProperty<double>(this, "testStatisticAposteriori");
			pValueApriori = new SimpleObjectProperty<double>(this, "pValueApriori");
			pValueAposteriori = new SimpleObjectProperty<double>(this, "pValueAposteriori");
			confidenceA = new SimpleObjectProperty<double>(this, "confidenceA");
			confidenceB = new SimpleObjectProperty<double>(this, "confidenceB");
			confidenceC = new SimpleObjectProperty<double>(this, "confidenceC");
			confidenceAlpha = new SimpleObjectProperty<double>(this, "confidenceAlpha");
			confidenceBeta = new SimpleObjectProperty<double>(this, "confidenceBeta");
			confidenceGamma = new SimpleObjectProperty<double>(this, "confidenceGamma");
			significant = new SimpleBooleanProperty(this, "significant", false);
			enable = new SimpleBooleanProperty(this, "enable", true);
			firstPrincipalComponentX = new SimpleObjectProperty<double>(this, "firstPrincipalComponentX");
			firstPrincipalComponentY = new SimpleObjectProperty<double>(this, "firstPrincipalComponentY");
			firstPrincipalComponentZ = new SimpleObjectProperty<double>(this, "firstPrincipalComponentZ");
			influenceOnPointPositionX = new SimpleObjectProperty<double>(this, "influenceOnPointPositionX");
			influenceOnPointPositionY = new SimpleObjectProperty<double>(this, "influenceOnPointPositionY");
			influenceOnPointPositionZ = new SimpleObjectProperty<double>(this, "influenceOnPointPositionZ");
			influenceOnNetworkDistortion = new SimpleObjectProperty<double>(this, "influenceOnNetworkDistortion");
		}

		private ObjectProperty<string> name;
		private ObjectProperty<string> code;

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

		private ObjectProperty<double> omega;

		private ObjectProperty<double> testStatisticApriori;
		private ObjectProperty<double> testStatisticAposteriori;

		private ObjectProperty<double> pValueApriori;
		private ObjectProperty<double> pValueAposteriori;

		private ObjectProperty<double> confidenceA;
		private ObjectProperty<double> confidenceB;
		private ObjectProperty<double> confidenceC;

		private ObjectProperty<double> confidenceAlpha;
		private ObjectProperty<double> confidenceBeta;
		private ObjectProperty<double> confidenceGamma;

		private BooleanProperty significant;
		private BooleanProperty enable;

		private ObjectProperty<double> firstPrincipalComponentX;
		private ObjectProperty<double> firstPrincipalComponentY;
		private ObjectProperty<double> firstPrincipalComponentZ;

		private ObjectProperty<double> influenceOnPointPositionX;
		private ObjectProperty<double> influenceOnPointPositionY;
		private ObjectProperty<double> influenceOnPointPositionZ;

		private ObjectProperty<double> influenceOnNetworkDistortion;

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


		public virtual ObjectProperty<string> codeProperty()
		{
			return this.code;
		}

		public virtual string Code
		{
			get
			{
				return this.codeProperty().get();
			}
			set
			{
				this.codeProperty().set(value);
			}
		}


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


		public virtual ObjectProperty<double> confidenceAProperty()
		{
			return this.confidenceA;
		}

		public virtual double? ConfidenceA
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


		public virtual ObjectProperty<double> confidenceBProperty()
		{
			return this.confidenceB;
		}

		public virtual double? ConfidenceB
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


		public virtual ObjectProperty<double> confidenceCProperty()
		{
			return this.confidenceC;
		}

		public virtual double? ConfidenceC
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


		public virtual ObjectProperty<double> confidenceAlphaProperty()
		{
			return this.confidenceAlpha;
		}

		public virtual double? ConfidenceAlpha
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


		public virtual ObjectProperty<double> confidenceBetaProperty()
		{
			return this.confidenceBeta;
		}

		public virtual double? ConfidenceBeta
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


		public virtual ObjectProperty<double> confidenceGammaProperty()
		{
			return this.confidenceGamma;
		}

		public virtual double? ConfidenceGamma
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


		public virtual ObjectProperty<double> firstPrincipalComponentXProperty()
		{
			return this.firstPrincipalComponentX;
		}

		public virtual double? FirstPrincipalComponentX
		{
			get
			{
				return this.firstPrincipalComponentXProperty().get();
			}
			set
			{
				this.firstPrincipalComponentXProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> firstPrincipalComponentYProperty()
		{
			return this.firstPrincipalComponentY;
		}

		public virtual double? FirstPrincipalComponentY
		{
			get
			{
				return this.firstPrincipalComponentYProperty().get();
			}
			set
			{
				this.firstPrincipalComponentYProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> firstPrincipalComponentZProperty()
		{
			return this.firstPrincipalComponentZ;
		}

		public virtual double? FirstPrincipalComponentZ
		{
			get
			{
				return this.firstPrincipalComponentZProperty().get();
			}
			set
			{
				this.firstPrincipalComponentZProperty().set(value);
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


		public static PointRow cloneRowApriori(PointRow row)
		{
			PointRow clone = new PointRow();

			clone.Id = -1;
			clone.GroupId = row.GroupId;
			clone.Enable = row.Enable;

			clone.Name = row.Name;
			clone.Code = row.Code;

			clone.XApriori = row.XApriori;
			clone.YApriori = row.YApriori;
			clone.ZApriori = row.ZApriori;

			clone.SigmaXapriori = row.SigmaXapriori;
			clone.SigmaYapriori = row.SigmaYapriori;
			clone.SigmaZapriori = row.SigmaZapriori;

			return clone;
		}

		private static PointRow scan1D(string str)
		{
			FormatterOptions options = FormatterOptions.Instance;
			try
			{
				string[] data = str.Trim().Split("[\\s;]+", true);

				if (data.Length < 2)
				{
					return null;
				}

				PointRow row = new PointRow();

				string name = data[0];
				double x = 0.0, y = 0.0, z = 0.0;
				double sigmaZ = 0.0;

				row.Name = name;

				// Y or Z
				y = z = options.convertLengthToModel(double.Parse(data[1].Replace(',', '.')));

				if (data.Length < 3)
				{
					row.ZApriori = z;
					return row;
				}

				x = sigmaZ = options.convertLengthToModel(double.Parse(data[2].Replace(',', '.')));

				// Z
				if (data.Length < 4)
				{
					row.ZApriori = z;
					row.SigmaZapriori = sigmaZ;
					return row;
				}

				z = options.convertLengthToModel(double.Parse(data[3].Replace(',', '.')));

				// Sigma
				if (data.Length < 5)
				{
					row.YApriori = y;
					row.XApriori = x;
					row.ZApriori = z;
					return row;
				}

				sigmaZ = options.convertLengthToModel(double.Parse(data[4].Replace(',', '.')));

				row.YApriori = y;
				row.XApriori = x;
				row.ZApriori = z;
				row.SigmaZapriori = sigmaZ;

				return row;
			}
			catch (Exception)
			{
				// e.printStackTrace();
			}
			return null;
		}

		private static PointRow scan2D(string str)
		{
			FormatterOptions options = FormatterOptions.Instance;

			try
			{
				string[] data = str.Trim().Split("[\\s;]+", true);

				if (data.Length < 3)
				{
					return null;
				}

				PointRow row = new PointRow();

				// Name of point
				string name = data[0];
				double x = 0.0, y = 0.0;
				double sigmaY = 0.0, sigmaX = 0.0;

				row.Name = name;

				// Y 		
				y = options.convertLengthToModel(double.Parse(data[1].Replace(',', '.')));

				// X 		
				x = options.convertLengthToModel(double.Parse(data[2].Replace(',', '.')));

				// sigma X (or sigmaX/Y)
				if (data.Length < 4)
				{
					row.YApriori = y;
					row.XApriori = x;
					return row;
				}

				sigmaY = sigmaX = options.convertLengthToModel(double.Parse(data[3].Replace(',', '.')));

				// sigma Y
				if (data.Length < 5)
				{
					row.YApriori = y;
					row.XApriori = x;
					row.SigmaYapriori = sigmaY;
					row.SigmaXapriori = sigmaX;
					return row;
				}

				sigmaX = options.convertLengthToModel(double.Parse(data[4].Replace(',', '.')));

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

		private static PointRow scan3D(string str)
		{
			FormatterOptions options = FormatterOptions.Instance;

			try
			{
				string[] data = str.Trim().Split("[\\s;]+", true);

				if (data.Length < 4)
				{
					return null;
				}

				PointRow row = new PointRow();

				// Name of point
				string name = data[0];
				double y = 0.0, x = 0.0, z = 0.0;
				double sigmaY = 0.0, sigmaX = 0.0, sigmaZ = 0.0;

				row.Name = name;

				// Y 		
				y = options.convertLengthToModel(double.Parse(data[1].Replace(',', '.')));

				// X 		
				x = options.convertLengthToModel(double.Parse(data[2].Replace(',', '.')));

				// Z 		
				z = options.convertLengthToModel(double.Parse(data[3].Replace(',', '.')));

				// sigma X (or sigma x/y/z)
				if (data.Length < 5)
				{
					row.YApriori = y;
					row.XApriori = x;
					row.ZApriori = z;
					return row;
				}

				sigmaY = sigmaX = sigmaZ = options.convertLengthToModel(double.Parse(data[4].Replace(',', '.')));

				// sigma Y (or sigma Z)
				if (data.Length < 6)
				{
					row.YApriori = y;
					row.XApriori = x;
					row.ZApriori = z;

					row.SigmaYapriori = sigmaY;
					row.SigmaXapriori = sigmaX;
					row.SigmaZapriori = sigmaZ;

					return row;
				}

				sigmaX = sigmaZ = options.convertLengthToModel(double.Parse(data[5].Replace(',', '.')));

				// sigma Z
				if (data.Length < 7)
				{
					row.YApriori = y;
					row.XApriori = x;
					row.ZApriori = z;

					row.SigmaYapriori = sigmaY;
					row.SigmaXapriori = sigmaY;
					row.SigmaZapriori = sigmaZ;

					return row;
				}

				sigmaZ = options.convertLengthToModel(double.Parse(data[6].Replace(',', '.')));

				row.YApriori = y;
				row.XApriori = x;
				row.ZApriori = z;

				row.SigmaYapriori = sigmaY;
				row.SigmaXapriori = sigmaX;
				row.SigmaZapriori = sigmaZ;

				return row;
			}
			catch (Exception)
			{
				// e.printStackTrace();
			}
			return null;
		}

		public static PointRow scan(string str, int dimension)
		{
			if (dimension == 1)
			{
				return scan1D(str);
			}
			else if (dimension == 2)
			{
				return scan2D(str);
			}
			else if (dimension == 3)
			{
				return scan3D(str);
			}
			return null;
		}
	}

}