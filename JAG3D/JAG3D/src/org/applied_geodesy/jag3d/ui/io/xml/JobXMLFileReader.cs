using System;
using System.Collections.Generic;

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

namespace org.applied_geodesy.jag3d.ui.io.xml
{


	using Constant = org.applied_geodesy.adjustment.Constant;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using Reduction = org.applied_geodesy.adjustment.network.observation.reduction.Reduction;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using DimensionType = org.applied_geodesy.jag3d.ui.io.DimensionType;
	using ImportOption = org.applied_geodesy.jag3d.ui.io.ImportOption;
	using PointRow = org.applied_geodesy.jag3d.ui.table.row.PointRow;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using PointTreeItemValue = org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using XMLUtilities = org.applied_geodesy.util.XMLUtilities;
	using org.applied_geodesy.util.io;
	using Document = org.w3c.dom.Document;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using ErrorHandler = org.xml.sax.ErrorHandler;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;

	using TreeItem = javafx.scene.control.TreeItem;
	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;

	public class JobXMLFileReader : SourceFileReader<TreeItem<TreeItemValue>>, ErrorHandler
	{
		private class StationRecord
		{
			private readonly JobXMLFileReader outerInstance;

			internal string stationName;
			internal double ih = 0, ppm = 0, refraction = 0;
//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
			internal bool applyEarthCurveCorrection_Conflict = false;
			public StationRecord(JobXMLFileReader outerInstance, string stationName, double ih, double ppm, double refraction, bool applyEarthCurveCorrection)
			{
				this.outerInstance = outerInstance;
				this.stationName = stationName;
				this.ih = ih;
				this.ppm = ppm;
				this.refraction = refraction;
				this.applyEarthCurveCorrection_Conflict = applyEarthCurveCorrection;
			}
			public virtual double InstrumentHeight
			{
				get
				{
					return this.ih;
				}
			}
			public virtual string StationName
			{
				get
				{
					return this.stationName;
				}
			}
			public virtual double AtmospherePPMValue
			{
				get
				{
					return this.ppm;
				}
			}
			public virtual double AtmosphereRefractionValue
			{
				get
				{
					return this.refraction;
				}
			}
			public virtual bool applyEarthCurveCorrection()
			{
				return this.applyEarthCurveCorrection_Conflict;
			}
			public override string ToString()
			{
				return "StationRecord [stationName=" + stationName + ", ih=" + ih + "]";
			}
		}

		private IDictionary<string, StationRecord> stations = new LinkedHashMap<string, StationRecord>();
		private bool isValidDocument = true;
		private readonly DimensionType dim;

		private IList<TerrestrialObservationRow> leveling = null;

		private IList<TerrestrialObservationRow> horizontalDistances = null;
		private IList<TerrestrialObservationRow> directions = null;

		private IList<TerrestrialObservationRow> slopeDistances = null;
		private IList<TerrestrialObservationRow> zenithAngles = null;

		private IList<PointRow> points2d = null;
		private IList<PointRow> points3d = null;

		private ISet<string> pointNames = null;

		private TreeItem<TreeItemValue> lastTreeItem = null;

		public JobXMLFileReader(DimensionType dim)
		{
			this.dim = dim == DimensionType.PLAN ? DimensionType.PLAN : dim == DimensionType.PLAN_AND_HEIGHT ? DimensionType.PLAN_AND_HEIGHT : DimensionType.SPATIAL;
			this.reset();
		}

		public JobXMLFileReader(Path p, DimensionType dim) : base(p)
		{
			this.dim = dim == DimensionType.PLAN ? DimensionType.PLAN : dim == DimensionType.PLAN_AND_HEIGHT ? DimensionType.PLAN_AND_HEIGHT : DimensionType.SPATIAL;
			this.reset();
		}

		public JobXMLFileReader(string xmlFileName, DimensionType dim) : this(new File(xmlFileName), dim)
		{
		}

		public JobXMLFileReader(File xmlFile, DimensionType dim) : this(xmlFile.toPath(), dim)
		{
		}

		public virtual bool ValidDocument
		{
			get
			{
				return this.isValidDocument;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void error(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
		public override void error(SAXParseException e)
		{
			e.Message;
			this.isValidDocument = false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void fatalError(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
		public override void fatalError(SAXParseException e)
		{
			e.Message;
			this.isValidDocument = false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void warning(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
		public override void warning(SAXParseException e)
		{
			e.Message;
		}

		public override void parse(string line)
		{
		}
		public override void reset()
		{

			if (this.points2d == null)
			{
				this.points2d = new List<PointRow>();
			}
			if (this.points3d == null)
			{
				this.points3d = new List<PointRow>();
			}

			if (this.leveling == null)
			{
				this.leveling = new List<TerrestrialObservationRow>();
			}
			if (this.horizontalDistances == null)
			{
				this.horizontalDistances = new List<TerrestrialObservationRow>();
			}
			if (this.directions == null)
			{
				this.directions = new List<TerrestrialObservationRow>();
			}
			if (this.slopeDistances == null)
			{
				this.slopeDistances = new List<TerrestrialObservationRow>();
			}
			if (this.zenithAngles == null)
			{
				this.zenithAngles = new List<TerrestrialObservationRow>();
			}

			if (this.pointNames == null)
			{
				this.pointNames = new HashSet<string>();
			}
			if (this.stations == null)
			{
				this.stations = new LinkedHashMap<string, StationRecord>();
			}

			this.stations.Clear();

			this.pointNames.Clear();
			this.points2d.Clear();
			this.points3d.Clear();

			this.leveling.Clear();

			this.horizontalDistances.Clear();
			this.directions.Clear();

			this.slopeDistances.Clear();
			this.zenithAngles.Clear();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			this.reset();
			this.lastTreeItem = null;
			this.pointNames.addAll(SQLManager.Instance.FullPointNameSet);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double earthRadius = this.getEarthRadius();
			double earthRadius = this.EarthRadius;
			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			factory.setNamespaceAware(true);
			DocumentBuilder builder;
			try
			{
				builder = factory.newDocumentBuilder();
				File xmlFile = this.Path.toFile();
				Document document = builder.parse(xmlFile);

				JobXMLNamespaceContext namespaceContextJobXML = new JobXMLNamespaceContext(document);

				string xpathPattern = "//JOBFile/FieldBook/PointRecord/Grid | " + "//JOBFile/FieldBook/PointRecord/ComputedGrid | " + "//JOBFile/Reductions/Point/Grid";

				NodeList nodeList = (NodeList)XMLUtilities.xpathSearch(document, xpathPattern, namespaceContextJobXML, XPathConstants.NODESET);
				for (int i = 0; i < nodeList.getLength(); i++)
				{
					Node pointNode = nodeList.item(i);

					string pointName = (string)XMLUtilities.xpathSearch(pointNode, "../Name", namespaceContextJobXML, XPathConstants.STRING);
					string pointCode = (string)XMLUtilities.xpathSearch(pointNode, "../Code", namespaceContextJobXML, XPathConstants.STRING);
					string deleted = (string)XMLUtilities.xpathSearch(pointNode, "../Deleted", namespaceContextJobXML, XPathConstants.STRING);
					bool isDeleted = !string.ReferenceEquals(deleted, null) && bool.Parse(deleted);

					if (isDeleted)
					{
						continue;
					}

					double? x0 = (double?)XMLUtilities.xpathSearch(pointNode, "North", namespaceContextJobXML, XPathConstants.NUMBER);
					double? y0 = (double?)XMLUtilities.xpathSearch(pointNode, "East", namespaceContextJobXML, XPathConstants.NUMBER);
					double? z0 = (double?)XMLUtilities.xpathSearch(pointNode, "Elevation", namespaceContextJobXML, XPathConstants.NUMBER);

					if (!string.ReferenceEquals(pointName, null) && pointName.Trim().Length > 0 && !this.pointNames.Contains(pointName))
					{
						x0 = x0 == null || double.IsNaN(x0) || double.IsInfinity(x0) ? 0.0 : x0;
						y0 = y0 == null || double.IsNaN(y0) || double.IsInfinity(y0) ? 0.0 : y0;
						z0 = z0 == null || double.IsNaN(z0) || double.IsInfinity(z0) ? 0.0 : z0;

						this.pointNames.Add(pointName);

						PointRow point = new PointRow();
						point.Name = pointName;
						if (!string.ReferenceEquals(pointCode, null))
						{
							point.Code = pointCode;
						}

						if (this.dim != DimensionType.HEIGHT)
						{
							point.XApriori = x0;
							point.YApriori = y0;
						}
						if (this.dim != DimensionType.PLAN)
						{
							point.ZApriori = z0;
						}

						if (this.dim == DimensionType.PLAN)
						{
							this.points2d.Add(point);
						}
						else
						{
							this.points3d.Add(point);
						}
					}
				}

				// Bestimme Stationen
				xpathPattern = "//JOBFile/FieldBook/StationRecord";
				nodeList = (NodeList)XMLUtilities.xpathSearch(document, xpathPattern, namespaceContextJobXML, XPathConstants.NODESET);
				for (int i = 0; i < nodeList.getLength(); i++)
				{
					Node node = nodeList.item(i);
					if (node != null && node.hasChildNodes())
					{
						NamedNodeMap attr = node.getAttributes();
						string stationId = attr.getNamedItem("ID") == null ? null : attr.getNamedItem("ID").getNodeValue();
						string stationName = (string)XMLUtilities.xpathSearch(node, "StationName", namespaceContextJobXML, XPathConstants.STRING);
						double? stationHeight = (double?)XMLUtilities.xpathSearch(node, "TheodoliteHeight", namespaceContextJobXML, XPathConstants.NUMBER);
						string atmosphereId = (string)XMLUtilities.xpathSearch(node, "AtmosphereID", namespaceContextJobXML, XPathConstants.STRING);
						stationHeight = stationHeight == null || double.IsNaN(stationHeight) || double.IsInfinity(stationHeight) ? 0.0 : stationHeight;

						if (!string.ReferenceEquals(stationId, null) && stationId.Trim().Length > 0 && !string.ReferenceEquals(stationName, null) && stationName.Trim().Length > 0)
						{
							xpathPattern = "//JOBFile/FieldBook/AtmosphereRecord[@ID=\"%s\"]";
							Node atmNode = (Node)XMLUtilities.xpathSearch(document, String.format(Locale.ENGLISH, xpathPattern, atmosphereId), namespaceContextJobXML, XPathConstants.NODE);

							double? ppm = 0.0, refraction = 0.0;
							bool applyEarthCurveCorr = false;
							if (atmNode != null && atmNode.hasChildNodes())
							{
								string applyPPM = (string)XMLUtilities.xpathSearch(atmNode, "ApplyPPMToRawDistances", namespaceContextJobXML, XPathConstants.STRING);
								string applyRefraction = (string)XMLUtilities.xpathSearch(atmNode, "ApplyRefractionCorrection", namespaceContextJobXML, XPathConstants.STRING);

								string applyEarthCurve = (string)XMLUtilities.xpathSearch(atmNode, "ApplyEarthCurvatureCorrection", namespaceContextJobXML, XPathConstants.STRING);
								ppm = (double?)XMLUtilities.xpathSearch(atmNode, "PPM", namespaceContextJobXML, XPathConstants.NUMBER);
								refraction = (double?)XMLUtilities.xpathSearch(atmNode, "RefractionCoefficient", namespaceContextJobXML, XPathConstants.NUMBER);

								bool applyPPMCorr = !string.ReferenceEquals(applyPPM, null) && bool.Parse(applyPPM);
								bool applyRefractionCorr = !string.ReferenceEquals(applyRefraction, null) && bool.Parse(applyRefraction);
								applyEarthCurveCorr = !string.ReferenceEquals(applyEarthCurve, null) && bool.Parse(applyEarthCurve);

								ppm = !applyPPMCorr || ppm == null || double.IsNaN(ppm) || double.IsInfinity(ppm) ? 0.0 : ppm;
								refraction = !applyRefractionCorr || refraction == null || double.IsNaN(refraction) || double.IsInfinity(refraction) ? 0.0 : refraction;
							}
							this.stations[stationId] = new StationRecord(this, stationName, stationHeight.Value, ppm.Value, refraction.Value, applyEarthCurveCorr);
							if (!this.pointNames.Contains(stationName))
							{
								this.pointNames.Add(stationName);
								PointRow point = new PointRow();
								point.Name = stationName;

								if (this.dim != DimensionType.HEIGHT)
								{
									point.XApriori = 0.0;
									point.YApriori = 0.0;
								}
								if (this.dim != DimensionType.PLAN)
								{
									point.ZApriori = 0.0;
								}

								if (this.dim == DimensionType.PLAN)
								{
									this.points2d.Add(point);
								}
								else
								{
									this.points3d.Add(point);
								}
							}
						}
					}
				}

				xpathPattern = "//JOBFile/FieldBook/PointRecord[StationID=\"%s\"]";
				foreach (string stationId in this.stations.Keys)
				{
					StationRecord station = this.stations[stationId];
					double ppm = station.AtmospherePPMValue;
					double refraction = station.AtmosphereRefractionValue;
					bool applyEarthCurve = station.applyEarthCurveCorrection();
					nodeList = (NodeList)XMLUtilities.xpathSearch(document, String.format(Locale.ENGLISH, xpathPattern, stationId), namespaceContextJobXML, XPathConstants.NODESET);
					for (int i = 0; i < nodeList.getLength(); i++)
					{
						Node stationNode = nodeList.item(i);
						string targetID = (string)XMLUtilities.xpathSearch(stationNode, "TargetID", namespaceContextJobXML, XPathConstants.STRING);
						string targetName = (string)XMLUtilities.xpathSearch(stationNode, "Name", namespaceContextJobXML, XPathConstants.STRING);
						string deleted = (string)XMLUtilities.xpathSearch(stationNode, "Deleted", namespaceContextJobXML, XPathConstants.STRING);
						bool isDeleted = !string.ReferenceEquals(deleted, null) && bool.Parse(deleted);

						// Keine Punktnummer fuer den Zielpunkt vorhanden oder Messung als geloescht markiert
						if (isDeleted || string.ReferenceEquals(targetName, null) || targetName.Trim().Length == 0)
						{
							continue;
						}

						double? direction = (double?)XMLUtilities.xpathSearch(stationNode, "Circle/HorizontalCircle", namespaceContextJobXML, XPathConstants.NUMBER);
						double? zenithAngle = (double?)XMLUtilities.xpathSearch(stationNode, "Circle/VerticalCircle", namespaceContextJobXML, XPathConstants.NUMBER);
						double? distance3d = (double?)XMLUtilities.xpathSearch(stationNode, "Circle/EDMDistance", namespaceContextJobXML, XPathConstants.NUMBER);
						string faceType = (string)XMLUtilities.xpathSearch(stationNode, "Circle/Face", namespaceContextJobXML, XPathConstants.STRING);

						bool isFaceI = true;
						//Valid values Face1, Face2 *AND* FaceNull
						if (!string.ReferenceEquals(faceType, null) && !faceType.Equals("Face1", StringComparison.OrdinalIgnoreCase) || zenithAngle != null && !double.IsNaN(zenithAngle) && !double.IsInfinity(zenithAngle) && zenithAngle > 180.0)
						{
							isFaceI = false;
						}

						// Reduziere auf Lage I und Umrechnung in RAD
						if (direction != null && !double.IsNaN(direction) && !double.IsInfinity(direction))
						{
							direction = direction.Value * Constant.RHO_DEG2RAD;

							if (!isFaceI)
							{
								direction = MathExtension.MOD(direction.Value + Math.PI, 2.0 * Math.PI);
							}
						}

						if (zenithAngle != null && !double.IsNaN(zenithAngle) && !double.IsInfinity(zenithAngle))
						{
							zenithAngle = zenithAngle.Value * Constant.RHO_DEG2RAD;

							if (!isFaceI)
							{
								zenithAngle = MathExtension.MOD(2.0 * Math.PI - zenithAngle.Value, 2.0 * Math.PI);
							}
						}

						double? prismConstant = 0.0;
						double? targetHeight = 0.0;

						if (!string.ReferenceEquals(targetID, null) && targetID.Trim().Length > 0)
						{
							string xpathPatternTargetRecord = "//JOBFile/FieldBook/TargetRecord[@ID=\"%s\"]";
							Node targetNode = (Node)XMLUtilities.xpathSearch(document, String.format(Locale.ENGLISH, xpathPatternTargetRecord, targetID), namespaceContextJobXML, XPathConstants.NODE);

							prismConstant = (double?)XMLUtilities.xpathSearch(targetNode, "PrismConstant", namespaceContextJobXML, XPathConstants.NUMBER);
							targetHeight = (double?)XMLUtilities.xpathSearch(targetNode, "TargetHeight", namespaceContextJobXML, XPathConstants.NUMBER);

							prismConstant = prismConstant == null || double.IsNaN(prismConstant) || double.IsInfinity(prismConstant) ? 0.0 : prismConstant;
							targetHeight = targetHeight == null || double.IsNaN(targetHeight) || double.IsInfinity(targetHeight) ? 0.0 : targetHeight;
						}

						double distanceForUncertaintyModel = 0;
						if (this.dim == DimensionType.SPATIAL)
						{
							if (distance3d != null && !double.IsNaN(distance3d) && !double.IsInfinity(distance3d) && distance3d > 0)
							{
								TerrestrialObservationRow distanceRow = new TerrestrialObservationRow();
								distanceRow.InstrumentHeight = station.InstrumentHeight;
								distanceRow.StartPointName = station.StationName;
								distanceRow.ReflectorHeight = targetHeight;
								distanceRow.EndPointName = targetName;
								distanceRow.ValueApriori = (distance3d.Value * (1.0 + ppm * 1E-6)) + prismConstant.Value;
								distanceForUncertaintyModel = (distance3d.Value * (1.0 + ppm * 1E-6)) + prismConstant.Value;
								distanceRow.DistanceApriori = distanceForUncertaintyModel;
								this.slopeDistances.Add(distanceRow);
							}

							if (zenithAngle != null && !double.IsNaN(zenithAngle) && !double.IsInfinity(zenithAngle))
							{
								TerrestrialObservationRow zenithAnglesRow = new TerrestrialObservationRow();
								zenithAnglesRow.InstrumentHeight = station.InstrumentHeight;
								zenithAnglesRow.StartPointName = station.StationName;
								zenithAnglesRow.ReflectorHeight = targetHeight;
								zenithAnglesRow.EndPointName = targetName;
								zenithAnglesRow.ValueApriori = zenithAngle;
								if (distanceForUncertaintyModel > 0)
								{
									zenithAnglesRow.DistanceApriori = distanceForUncertaintyModel;
								}
								this.zenithAngles.Add(zenithAnglesRow);
							}
						}

						else if (this.dim == DimensionType.PLAN || this.dim == DimensionType.PLAN_AND_HEIGHT)
						{
							if ((zenithAngle != null && !double.IsNaN(zenithAngle) && !double.IsInfinity(zenithAngle)) && (distance3d != null && !double.IsNaN(distance3d) && !double.IsInfinity(distance3d) && distance3d > 0))
							{
								double dist3d = (distance3d.Value * (1.0 + ppm * 1E-6)) + prismConstant.Value;
								double zenith = zenithAngle.Value;

	//							// Reduziere die II. Lage auf die I. Lage
	//							if (zenith > Math.PI)
	//								zenith = 2.0*Math.PI - zenith;

	//							// Baumann 1993, S.  99 - Strecke 2D 
	//							// Baumann 1993, S. 137 - Hoehenunterschied
	//							double kDist2DRefra =  refraction*dist3d*dist3d / 2.0 / earthRadius * Math.cos(zenith);
	//							double kDeltaHRefra = -refraction*dist3d*dist3d / 2.0 / earthRadius * Math.sin(zenith);

								double dist2d = dist3d * Math.Sin(zenith);
								// Rueger (1996), S. 98, Gl (7.53)
								double kDist2DRefra = refraction * dist3d * dist3d / 4.0 / earthRadius * Math.Sin(2.0 * zenith);
								// Rueger (1996), S.109, Gl (8.26)
								double kDeltaHRefra = -refraction / 2.0 / earthRadius * dist2d * dist2d;

								double kDist2DEarth = 0.0;
								double kDeltaHEarth = 0.0;

								if (applyEarthCurve)
								{
	//								// Baumann 1993, S.  99 - Strecke 2D 
	//								// Baumann 1993, S. 137 - Hoehenunterschied
	//								kDist2DEarth = -dist3d*dist3d / earthRadius * Math.cos(zenith);
	//								kDeltaHEarth =  dist3d*dist3d / 2.0 / earthRadius * Math.sin(zenith);

									// Rueger (1996), S. 98, Gl (7.53)
									kDist2DEarth = -dist3d * dist3d / 2.0 / earthRadius * Math.Sin(2.0 * zenith);
									// Rueger (1996), S.109, Gl (8.26)
									kDeltaHEarth = 0.5 * dist2d * dist2d / earthRadius;
								}

								double distance2d = dist2d + kDist2DRefra + kDist2DEarth;
								double leveling = dist3d * Math.Cos(zenith) + kDeltaHRefra + kDeltaHEarth;

								distanceForUncertaintyModel = distance2d;

								TerrestrialObservationRow distanceRow = new TerrestrialObservationRow();
								distanceRow.InstrumentHeight = station.InstrumentHeight;
								distanceRow.StartPointName = station.StationName;
								distanceRow.ReflectorHeight = targetHeight;
								distanceRow.EndPointName = targetName;
								distanceRow.ValueApriori = distance2d;
								distanceRow.DistanceApriori = distanceForUncertaintyModel;
								this.horizontalDistances.Add(distanceRow);

								if (this.dim == DimensionType.PLAN_AND_HEIGHT)
								{
									TerrestrialObservationRow levelingRow = new TerrestrialObservationRow();
									levelingRow.InstrumentHeight = station.InstrumentHeight;
									levelingRow.StartPointName = station.StationName;
									levelingRow.ReflectorHeight = targetHeight;
									levelingRow.EndPointName = targetName;
									levelingRow.ValueApriori = leveling;
									if (distanceForUncertaintyModel > 0)
									{
										levelingRow.DistanceApriori = distanceForUncertaintyModel;
									}
									this.leveling.Add(levelingRow);
								}
							}
						}

						if (direction != null && !double.IsNaN(direction) && !double.IsInfinity(direction))
						{
							TerrestrialObservationRow directionsRow = new TerrestrialObservationRow();
							directionsRow.InstrumentHeight = station.InstrumentHeight;
							directionsRow.StartPointName = station.StationName;
							directionsRow.ReflectorHeight = targetHeight;
							directionsRow.EndPointName = targetName;
							directionsRow.ValueApriori = direction;
							if (distanceForUncertaintyModel > 0)
							{
								directionsRow.DistanceApriori = distanceForUncertaintyModel;
							}
							this.directions.Add(directionsRow);
						}
					}
					// Speichere Beobachtungen bspw. Richtungen, da diese Satzweise zu halten sind
					this.saveObservationGroups(false);
				}

				string itemName = this.createItemName(null, null);

				// Speichere Punkte
				if (this.dim == DimensionType.PLAN && this.points2d.Count > 0)
				{
					this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_2D_LEAF, this.points2d);
				}

				if ((this.dim == DimensionType.PLAN_AND_HEIGHT || this.dim == DimensionType.SPATIAL) && this.points3d.Count > 0)
				{
					this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_3D_LEAF, this.points3d);
				}

				// Speichere Beobachtungen
				this.saveObservationGroups(true);

			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				this.isValidDocument = false;
				throw new IOException(e);
			}
			catch (SQLException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				this.isValidDocument = false;
				throw new SQLException(e);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				this.isValidDocument = false;
				throw new IOException(e);
			}

			this.reset();

			return this.lastTreeItem;
		}

		public static ExtensionFilter[] ExtensionFilters
		{
			get
			{
				return new ExtensionFilter[]
				{
					new ExtensionFilter(I18N.Instance.getString("JobXMLFileReader.extension.jxl", "JobXML"), "*.jxl", "*.JXL"),
					new ExtensionFilter(I18N.Instance.getString("JobXMLFileReader.extension.xml", "Extensible Markup Language"), "*.xml", "*.XML")
				};
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveObservationGroups(boolean forceSaving) throws java.sql.SQLException
		private void saveObservationGroups(bool forceSaving)
		{
			if ((this.dim == DimensionType.HEIGHT || this.dim == DimensionType.PLAN_AND_HEIGHT) && this.leveling.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.LEVELING)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.LEVELING_LEAF, this.leveling);
			}

			if (this.directions.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.DIRECTION)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.DIRECTION_LEAF, this.directions);
			}

			if ((this.dim == DimensionType.PLAN || this.dim == DimensionType.PLAN_AND_HEIGHT) && this.horizontalDistances.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.HORIZONTAL_DISTANCE)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.HORIZONTAL_DISTANCE_LEAF, this.horizontalDistances);
			}

			if (this.dim == DimensionType.SPATIAL && this.slopeDistances.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.SLOPE_DISTANCE)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.SLOPE_DISTANCE_LEAF, this.slopeDistances);
			}

			if (this.dim == DimensionType.SPATIAL && this.zenithAngles.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.ZENITH_ANGLE)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.ZENITH_ANGLE_LEAF, this.zenithAngles);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveObservationGroup(org.applied_geodesy.jag3d.ui.tree.TreeItemType itemType, java.util.List<org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow> observations) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveObservationGroup(TreeItemType itemType, IList<TerrestrialObservationRow> observations)
		{
			TreeItem<TreeItemValue> treeItem = null;
			if (observations.Count > 0)
			{
				bool isGroupWithEqualStation = ImportOption.Instance.isGroupSeparation(TreeItemType.getObservationTypeByTreeItemType(itemType));
				string itemName = this.createItemName(null, isGroupWithEqualStation && !string.ReferenceEquals(observations[0].StartPointName, null) ? " (" + observations[0].StartPointName + ")" : null);
				treeItem = this.saveTerrestrialObservations(itemName, itemType, observations);
			}
			observations.Clear();
			return treeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveTerrestrialObservations(String itemName, org.applied_geodesy.jag3d.ui.tree.TreeItemType treeItemType, java.util.List<org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow> observations) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveTerrestrialObservations(string itemName, TreeItemType treeItemType, IList<TerrestrialObservationRow> observations)
		{
			if (observations == null || observations.Count == 0)
			{
				return null;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(treeItemType);
			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, -1, itemName, true, false);
			try
			{
				SQLManager.Instance.saveGroup((ObservationTreeItemValue)newTreeItem.getValue());
			}
			catch (SQLException e)
			{
				UITreeBuilder.Instance.removeItem(newTreeItem);
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				throw new SQLException(e);
			}

			try
			{
				int groupId = ((ObservationTreeItemValue)newTreeItem.getValue()).GroupId;
				foreach (TerrestrialObservationRow row in observations)
				{
					row.GroupId = groupId;
					SQLManager.Instance.saveItem(row);
				}
			}
			catch (SQLException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				throw new SQLException(e);
			}

			return newTreeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> savePoints(String itemName, org.applied_geodesy.jag3d.ui.tree.TreeItemType treeItemType, java.util.List<org.applied_geodesy.jag3d.ui.table.row.PointRow> points) throws java.sql.SQLException
		private TreeItem<TreeItemValue> savePoints(string itemName, TreeItemType treeItemType, IList<PointRow> points)
		{
			if (points == null || points.Count == 0)
			{
				return null;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(treeItemType);
			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, -1, itemName, true, false);
			try
			{
				SQLManager.Instance.saveGroup((PointTreeItemValue)newTreeItem.getValue());
			}
			catch (SQLException e)
			{
				UITreeBuilder.Instance.removeItem(newTreeItem);
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				throw new SQLException(e);
			}

			try
			{
				int groupId = ((PointTreeItemValue)newTreeItem.getValue()).GroupId;
				foreach (PointRow row in points)
				{
					row.GroupId = groupId;
					SQLManager.Instance.saveItem(row);
				}
			}
			catch (SQLException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				throw new SQLException(e);
			}

			return newTreeItem;
		}

		private double EarthRadius
		{
			get
			{
				double earthRadius = Constant.EARTH_RADIUS;
				try
				{
					Reduction reductions = new Reduction();
					SQLManager.Instance.load(reductions);
					earthRadius = reductions.EarthRadius;
				}
				catch (SQLException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				return earthRadius;
			}
		}
	}

}