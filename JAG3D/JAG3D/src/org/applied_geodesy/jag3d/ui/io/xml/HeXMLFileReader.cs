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
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using DimensionType = org.applied_geodesy.jag3d.ui.io.DimensionType;
	using ImportOption = org.applied_geodesy.jag3d.ui.io.ImportOption;
	using GNSSObservationRow = org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow;
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

	public class HeXMLFileReader : SourceFileReader<TreeItem<TreeItemValue>>, ErrorHandler
	{

		private class InstrumentSetup
		{
			private readonly HeXMLFileReader outerInstance;

			internal string instrPointName;
			internal double ih;
			public InstrumentSetup(HeXMLFileReader outerInstance, string instrPointName, double ih)
			{
				this.outerInstance = outerInstance;
				this.instrPointName = instrPointName;
				this.ih = ih;
			}
			public virtual double InstrumentHeight
			{
				get
				{
					return this.ih;
				}
			}
			public virtual string SetupPointName
			{
				get
				{
					return this.instrPointName;
				}
			}
			public override string ToString()
			{
				return "InstrumentSetup [instrPointName=" + instrPointName + ", ih=" + ih + "]";
			}
		}

		private LengthUnit lengthUnit = LengthUnit.METER;
		private AngularUnit angularUnit = AngularUnit.RADIAN;
		private IDictionary<string, InstrumentSetup> setups = new LinkedHashMap<string, InstrumentSetup>();
		private bool isValidDocument = true;
		private readonly DimensionType dim;

		private IList<GNSSObservationRow> gnss2D = null;
		private IList<GNSSObservationRow> gnss3D = null;

		private IList<TerrestrialObservationRow> horizontalDistances = null;
		private IList<TerrestrialObservationRow> directions = null;

		private IList<TerrestrialObservationRow> slopeDistances = null;
		private IList<TerrestrialObservationRow> zenithAngles = null;

		private IList<PointRow> points2d = null;
		private IList<PointRow> points3d = null;

		private ISet<string> pointNames = null;
		private ISet<string> point3DName = null;
		private ISet<string> reservedNames = null;

		private TreeItem<TreeItemValue> lastTreeItem = null;

		public HeXMLFileReader(DimensionType dim)
		{
			this.dim = dim == DimensionType.PLAN ? DimensionType.PLAN : DimensionType.SPATIAL;
			this.reset();
		}

		public HeXMLFileReader(Path p, DimensionType dim) : base(p)
		{
			this.dim = dim == DimensionType.PLAN ? DimensionType.PLAN : DimensionType.SPATIAL;
			this.reset();
		}

		public HeXMLFileReader(string xmlFileName, DimensionType dim) : this(new File(xmlFileName), dim)
		{
		}

		public HeXMLFileReader(File xmlFile, DimensionType dim) : this(xmlFile.toPath(), dim)
		{
		}

		// "decimal dd.mm.ss": "45.3025123" representing 45 degrees 30 minutes and 25.123 seconds
		private double convertToRadian(double d)
		{
			switch (this.angularUnit)
			{
				case org.applied_geodesy.jag3d.ui.io.xml.AngularUnit.GRADIAN:
					return d * Constant.RHO_GRAD2RAD;
				case org.applied_geodesy.jag3d.ui.io.xml.AngularUnit.DEGREE:
					return d * Constant.RHO_DEG2RAD;
				case org.applied_geodesy.jag3d.ui.io.xml.AngularUnit.DDMMSSss:
					int dd = (int)d;
					d = 100.0 * (d - dd);
					int mm = (int)d;
					double ss = 100.0 * (d - mm);
					return ((double)dd + (double)mm / 60.0 + ss / 3600.0) * Constant.RHO_DEG2RAD;
				default: // RAD
					return d;
			}
		}

		private double convertToMeter(double d)
		{
			switch (this.lengthUnit)
			{
				case org.applied_geodesy.jag3d.ui.io.xml.LengthUnit.MILLIMETER:
					return 0.001 * d;
				case org.applied_geodesy.jag3d.ui.io.xml.LengthUnit.CENTIMETER:
					return 0.01 * d;
				case org.applied_geodesy.jag3d.ui.io.xml.LengthUnit.KILOMETER:
					return 1000.0 * d;
				case org.applied_geodesy.jag3d.ui.io.xml.LengthUnit.FOOT:
					return 12.0 * 0.0254 * d;
				case org.applied_geodesy.jag3d.ui.io.xml.LengthUnit.INCH:
					return 0.0254 * d;
				case org.applied_geodesy.jag3d.ui.io.xml.LengthUnit.US_SURVEY_FOOT:
					return 1200.0 / 3937.0 * d;
				case org.applied_geodesy.jag3d.ui.io.xml.LengthUnit.MILE:
					return 1609.344 * d;
				default: // METER
					return d;
			}
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
			if (this.setups == null)
			{
				this.setups = new LinkedHashMap<string, InstrumentSetup>();
			}

			if (this.points2d == null)
			{
				this.points2d = new List<PointRow>();
			}
			if (this.points3d == null)
			{
				this.points3d = new List<PointRow>();
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

			if (this.gnss2D == null)
			{
				this.gnss2D = new List<GNSSObservationRow>();
			}
			if (this.gnss3D == null)
			{
				this.gnss3D = new List<GNSSObservationRow>();
			}

			if (this.reservedNames == null)
			{
				this.reservedNames = new HashSet<string>();
			}

			if (this.pointNames == null)
			{
				this.pointNames = new HashSet<string>();
			}

			if (this.point3DName == null)
			{
				this.point3DName = new HashSet<string>();
			}

			this.setups.Clear();

			this.reservedNames.Clear();
			this.pointNames.Clear();
			this.point3DName.Clear();

			this.points2d.Clear();
			this.points3d.Clear();

			this.horizontalDistances.Clear();
			this.directions.Clear();

			this.slopeDistances.Clear();
			this.zenithAngles.Clear();

			this.gnss2D.Clear();
			this.gnss3D.Clear();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			this.reset();
			this.lastTreeItem = null;
			this.reservedNames = SQLManager.Instance.FullPointNameSet;

			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			factory.setNamespaceAware(true);
			DocumentBuilder builder;

			try
			{
				builder = factory.newDocumentBuilder();
				File xmlFile = this.Path.toFile();
				Document document = builder.parse(xmlFile);

				HeXMLNamespaceContext namespaceContext = new HeXMLNamespaceContext(document);

				// Bestimme Applikation auf dem Instrument
				string xpathPattern = "//landxml:LandXML/landxml:Application/@name";
				string applicationName = (string)XMLUtilities.xpathSearch(document, xpathPattern, namespaceContext, XPathConstants.STRING);

				// Bestimme Einheiten
				xpathPattern = "//landxml:LandXML/landxml:Units/*[1]";
				Node units = (Node)XMLUtilities.xpathSearch(document, xpathPattern, namespaceContext, XPathConstants.NODE);
				if (units != null)
				{
					NamedNodeMap attr = units.getAttributes();
					// ENUM { millimeter, centimeter, meter, kilometer, foot, USSurveyFoot, inch, mile }
					string lengthUnit = attr.getNamedItem("linearUnit") == null ? "meter" : attr.getNamedItem("linearUnit").getNodeValue();
					if (lengthUnit.Equals("millimeter", StringComparison.OrdinalIgnoreCase))
					{
						this.lengthUnit = LengthUnit.MILLIMETER;
					}
					else if (lengthUnit.Equals("centimeter", StringComparison.OrdinalIgnoreCase))
					{
						this.lengthUnit = LengthUnit.CENTIMETER;
					}
					else if (lengthUnit.Equals("kilometer", StringComparison.OrdinalIgnoreCase))
					{
						this.lengthUnit = LengthUnit.KILOMETER;
					}
					else if (lengthUnit.Equals("foot", StringComparison.OrdinalIgnoreCase))
					{
						this.lengthUnit = LengthUnit.FOOT;
					}
					else if (lengthUnit.Equals("USSurveyFoot", StringComparison.OrdinalIgnoreCase))
					{
						this.lengthUnit = LengthUnit.US_SURVEY_FOOT;
					}
					else if (lengthUnit.Equals("inch", StringComparison.OrdinalIgnoreCase))
					{
						this.lengthUnit = LengthUnit.INCH;
					}
					else if (lengthUnit.Equals("mile", StringComparison.OrdinalIgnoreCase))
					{
						this.lengthUnit = LengthUnit.MILE;
					}
					else //if (lengthUnit.equalsIgnoreCase("meter"))
					{
						this.lengthUnit = LengthUnit.METER;
					}

					// ENUM { radians, grads, decimal degrees, decimal dd.mm.ss }
					string angularUnit = attr.getNamedItem("angularUnit") == null ? "radians" : attr.getNamedItem("angularUnit").getNodeValue();
					if (angularUnit.Equals("grads", StringComparison.OrdinalIgnoreCase))
					{
						this.angularUnit = AngularUnit.GRADIAN;
					}
					else if (angularUnit.Equals("decimal degrees", StringComparison.OrdinalIgnoreCase))
					{
						this.angularUnit = AngularUnit.DEGREE;
					}
					else if (angularUnit.Equals("decimal dd.mm.ss", StringComparison.OrdinalIgnoreCase))
					{
						this.angularUnit = AngularUnit.DDMMSSss;
					}
					else // if (angularUnit.equalsIgnoreCase("radians"))
					{
						this.angularUnit = AngularUnit.RADIAN;
					}
				}

				// Ermittle alle Punkte (Koordinaten) im Projekt und die SetupIDs
				xpathPattern = "//landxml:LandXML/landxml:CgPoints/landxml:CgPoint[@name] | " + "//landxml:LandXML/landxml:Survey//landxml:Backsight/landxml:BacksightPoint[@name] | " + "//landxml:LandXML/landxml:Survey//landxml:TargetPoint[@name] |" + "//landxml:LandXML/landxml:Survey//landxml:InstrumentSetup[@stationName]/landxml:InstrumentPoint";

				NodeList nodeList = (NodeList)XMLUtilities.xpathSearch(document, xpathPattern, namespaceContext, XPathConstants.NODESET);
				for (int i = 0; i < nodeList.getLength(); i++)
				{
					Node node = nodeList.item(i);
					if (node.hasChildNodes() && node.getFirstChild().getNodeType() == Node.TEXT_NODE && !node.getFirstChild().getNodeValue().Trim().isEmpty())
					{
						NamedNodeMap attr = node.getAttributes();
						string pointName = "";
						string code = null;
						string setupId = "";
						double ih = 0.0;
						PointRow point = null;
						bool isDeleted = false;
						if (node.getNodeName().equalsIgnoreCase("CgPoint"))
						{
							pointName = attr.getNamedItem("oID") == null ? attr.getNamedItem("name").getNodeValue() : attr.getNamedItem("oID").getNodeValue();
							code = attr.getNamedItem("code") == null ? null : attr.getNamedItem("code").getNodeValue();
							// boolean isReferencePoint = attr.getNamedItem("role") != null && attr.getNamedItem("role").getNodeValue().equalsIgnoreCase("control point");
						}
						else if (node.getNodeName().equalsIgnoreCase("InstrumentPoint"))
						{
							NamedNodeMap parentNodeAttr = node.getParentNode().getAttributes();
							pointName = parentNodeAttr.getNamedItem("stationName").getNodeValue();
							Node status = parentNodeAttr.getNamedItem("status");
							isDeleted = status != null && status.getNodeValue() != null && status.getNodeValue().equalsIgnoreCase("deleted");
							// ermittle die Setup-ID
							setupId = parentNodeAttr.getNamedItem("id") == null ? "" : parentNodeAttr.getNamedItem("id").getNodeValue();

							//try {ih = attr.getNamedItem("instrumentHeight") == null ? 0.0 : this.convertToMeter(Double.parseDouble(attr.getNamedItem("instrumentHeight").getNodeValue()));}catch (NumberFormatException e) {ih = 0.0;}
							try
							{
								ih = parentNodeAttr.getNamedItem("instrumentHeight") == null ? 0.0 : this.convertToMeter(double.Parse(parentNodeAttr.getNamedItem("instrumentHeight").getNodeValue()));
							}
							catch (System.FormatException)
							{
								ih = 0.0;
							}
						}
						else
						{
							pointName = attr.getNamedItem("name").getNodeValue();
						}

						if (!isDeleted && pointName.Length > 0 && !this.pointNames.Contains(pointName))
						{
							this.pointNames.Add(pointName);
							string[] pointContent = node.getFirstChild().getNodeValue().Trim().Split("\\s+");
							double[] xyz = new double[pointContent.Length];
							try
							{
								for (int p = 0; p < pointContent.Length; p++)
								{
									xyz[p] = this.convertToMeter(double.Parse(pointContent[p]));
								}
							}
							catch (System.FormatException)
							{
								xyz = null;
							}

							if (xyz != null && pointContent.Length > 1)
							{
								point = new PointRow();
								point.Name = pointName;
								point.Code = code;
								double x = 0, y = 0, z = 0;
								if (xyz.Length != 1)
								{
									x = xyz[0];
									y = xyz[1];

									point.XApriori = x;
									point.YApriori = y;
								}
								if (xyz.Length != 2)
								{
									z = xyz[xyz.Length - 1];
									point.ZApriori = z;
								}

								if (xyz.Length == 2 || this.dim == DimensionType.PLAN)
								{
									if (!this.reservedNames.Contains(pointName))
									{
										this.points2d.Add(point);
									}
								}
								else
								{
									if (!this.reservedNames.Contains(pointName))
									{
										this.points3d.Add(point);
									}
									this.point3DName.Add(pointName);
								}
							}
						}
						if (setupId.Length > 0 && pointName.Length > 0 && this.pointNames.Contains(pointName))
						{
							this.setups[setupId] = new InstrumentSetup(this, pointName, ih);
						}
					}
				}
				// ermittle Beobachtungen zwischen den Punkten
				xpathPattern = "//landxml:LandXML/landxml:Survey//landxml:RawObservation[@setupID=\"%s\"]";
				bool applyAtmosphericCorrection = string.ReferenceEquals(applicationName, null) || !applicationName.Equals("LandXML Export", StringComparison.OrdinalIgnoreCase);
				foreach (string setupId in this.setups.Keys)
				{
					InstrumentSetup setup = this.setups[setupId];
					nodeList = (NodeList)XMLUtilities.xpathSearch(document, String.format(Locale.ENGLISH, xpathPattern, setupId), namespaceContext, XPathConstants.NODESET);
					for (int i = 0; i < nodeList.getLength(); i++)
					{
						Node node = nodeList.item(i);
						double th = 0.0;
						double? dir = null, zenith = null, dist2d = null, dist3d = null;
						bool? isDeleted = false;

						NamedNodeMap attr = node.getAttributes();
						try
						{
							isDeleted = attr.getNamedItem("status") == null ? false : attr.getNamedItem("status").getNodeValue().equalsIgnoreCase("deleted");
						}
						catch (Exception)
						{
							isDeleted = false;
						}
						if (isDeleted.Value)
						{
							continue;
						}

						try
						{
							th = attr.getNamedItem("targetHeight") == null ? 0.0 : this.convertToMeter(double.Parse(attr.getNamedItem("targetHeight").getNodeValue()));
						}
						catch (System.FormatException)
						{
							th = 0.0;
						}
						try
						{
							dir = attr.getNamedItem("horizAngle") == null ? null : this.convertToRadian(double.Parse(attr.getNamedItem("horizAngle").getNodeValue()));
						}
						catch (System.FormatException)
						{
							dir = null;
						}
						try
						{
							zenith = attr.getNamedItem("zenithAngle") == null ? null : this.convertToRadian(double.Parse(attr.getNamedItem("zenithAngle").getNodeValue()));
						}
						catch (System.FormatException)
						{
							zenith = null;
						}
						try
						{
							dist2d = attr.getNamedItem("horizDistance") == null ? null : this.convertToMeter(double.Parse(attr.getNamedItem("horizDistance").getNodeValue()));
						}
						catch (System.FormatException)
						{
							dist2d = null;
						}
						try
						{
							dist3d = attr.getNamedItem("slopeDistance") == null ? null : this.convertToMeter(double.Parse(attr.getNamedItem("slopeDistance").getNodeValue()));
						}
						catch (System.FormatException)
						{
							dist3d = null;
						}

						bool isFaceI = true;
						if (attr.getNamedItem("directFace") != null && !attr.getNamedItem("directFace").getNodeValue().equalsIgnoreCase("TRUE") || zenith != null && !double.IsNaN(zenith) && !double.IsInfinity(zenith) && zenith > Math.PI)
						{
							isFaceI = false;
						}

						// Reduziere auf Lage I
						if (!isFaceI)
						{
							if (dir != null)
							{
								dir = MathExtension.MOD(dir.Value + Math.PI, 2.0 * Math.PI);
							}
							if (this.dim == DimensionType.SPATIAL && zenith != null && zenith > Math.PI)
							{
								zenith = MathExtension.MOD(2.0 * Math.PI - zenith.Value, 2.0 * Math.PI);
							}
						}

						string xpath = "./landxml:TargetPoint/@name";
						string endPointName = (string)XMLUtilities.xpathSearch(node, xpath, namespaceContext, XPathConstants.STRING);

						int targetPointDim = this.point3DName.Contains(endPointName) ? 3 : this.pointNames.Contains(endPointName) ? 2 : this.dim == DimensionType.PLAN ? 2 : 3;
						int startPointDim = this.point3DName.Contains(setup.SetupPointName) ? 3 : this.pointNames.Contains(setup.SetupPointName) ? 2 : this.dim == DimensionType.PLAN ? 2 : 3;
						int obsDim = Math.Min(targetPointDim, startPointDim);

	//					System.out.println("DIM " +dim+"   "+obsDim);
	//					System.out.println("Polar " +dir+"  "+zenit+"  "+dist2d+"  "+dist3d);
						double distanceForUncertaintyModel = 0;
						if (this.dim == DimensionType.SPATIAL && obsDim == 3)
						{
							// Bestimme Korrekturparameter fuer 3D-Strecke, sofern es nicht das 1200er System ist
							if (dist3d != null)
							{
								xpath = "./landxml:Feature[@code=\"observationInfo\"]/landxml:Property[@label=\"TPSCorrectionRef\"]/@value";
								string tpsCorr = (string)XMLUtilities.xpathSearch(node, xpath, namespaceContext, XPathConstants.STRING);

								xpath = "./landxml:TargetPoint/@pntRef";
								tpsCorr = string.ReferenceEquals(tpsCorr, null) || tpsCorr.Length == 0 ? (string)XMLUtilities.xpathSearch(node, xpath, namespaceContext, XPathConstants.STRING) : tpsCorr;

								// HeXML
								xpath = "1.0 + //landxml:LandXML/hexml:HexagonLandXML/hexml:Survey/hexml:TPSCorrection[@uniqueID = ./../hexml:InstrumentSetup[@uniqueID=\"%s\"]/hexml:RawObservation[@targetPntRef=\"%s\"]/@tpsCorrectionRef ]/@atmosphericPPM * 0.000001";
								double? scale = (double?)XMLUtilities.xpathSearch(document, String.format(Locale.ENGLISH, xpath, setupId, tpsCorr), namespaceContext, XPathConstants.NUMBER);

								xpath = "//landxml:LandXML/hexml:HexagonLandXML/hexml:Survey/hexml:InstrumentSetup[@uniqueID=\"%s\"]/hexml:RawObservation[@targetPntRef=\"%s\"]/@reflectorConstant";
								double? add = (double?)XMLUtilities.xpathSearch(document, String.format(Locale.ENGLISH, xpath, setupId, tpsCorr), namespaceContext, XPathConstants.NUMBER);

								// LandXML
								xpath = "1.0 + //landxml:LandXML/landxml:Survey//landxml:Corrections/landxml:Feature[@code=\"TPSCorrection\"]/landxml:Property[@label=\"oID\"][@value = \"%s\"]/../landxml:Property[@label=\"atmosphericPPM\"]/@value * 0.000001";
								scale = scale == null || double.IsNaN(scale) ? (double?)XMLUtilities.xpathSearch(node, String.format(Locale.ENGLISH, xpath, tpsCorr), namespaceContext, XPathConstants.NUMBER) : scale;

								xpath = "./landxml:Feature[@code=\"observationInfo\"]/landxml:Property[@label=\"reflectorConstant\"]/@value";
								add = add == null || double.IsNaN(add) ? (double?)XMLUtilities.xpathSearch(node, xpath, namespaceContext, XPathConstants.NUMBER) : add;

								// Validiere Korrekturwerte
								scale = !applyAtmosphericCorrection || scale == null || double.IsNaN(scale) ? 1.0 : scale;
								add = !applyAtmosphericCorrection || add == null || double.IsNaN(add) ? 0.0 : add;
								add = this.convertToMeter(add.Value);

								// Koorigiere Schraegstrecke
								dist3d = (dist3d.Value - add.Value) * scale.Value + add.Value;
								distanceForUncertaintyModel = dist3d.Value;

								TerrestrialObservationRow slopeDistances = new TerrestrialObservationRow();
								slopeDistances.InstrumentHeight = setup.InstrumentHeight;
								slopeDistances.StartPointName = setup.SetupPointName;
								slopeDistances.ReflectorHeight = th;
								slopeDistances.EndPointName = endPointName;
								slopeDistances.ValueApriori = dist3d;
								slopeDistances.DistanceApriori = distanceForUncertaintyModel;
								if (dist3d > 0)
								{
									this.slopeDistances.Add(slopeDistances);
								}
							}
							if (zenith != null)
							{
								TerrestrialObservationRow zenithAngles = new TerrestrialObservationRow();
								zenithAngles.InstrumentHeight = setup.InstrumentHeight;
								zenithAngles.StartPointName = setup.SetupPointName;
								zenithAngles.ReflectorHeight = th;
								zenithAngles.EndPointName = endPointName;
								zenithAngles.ValueApriori = zenith;
								if (distanceForUncertaintyModel > 0)
								{
									zenithAngles.DistanceApriori = distanceForUncertaintyModel;
								}
								this.zenithAngles.Add(zenithAngles);
							}
						}
						else if (dist2d != null)
						{
							distanceForUncertaintyModel = dist2d.Value;
							TerrestrialObservationRow horizontalDistances = new TerrestrialObservationRow();
							horizontalDistances.InstrumentHeight = setup.InstrumentHeight;
							horizontalDistances.StartPointName = setup.SetupPointName;
							horizontalDistances.ReflectorHeight = th;
							horizontalDistances.EndPointName = endPointName;
							horizontalDistances.ValueApriori = dist2d;
							horizontalDistances.DistanceApriori = distanceForUncertaintyModel;
							if (dist2d > 0)
							{
								this.horizontalDistances.Add(horizontalDistances);
							}
						}

						if (dir != null)
						{
							TerrestrialObservationRow directions = new TerrestrialObservationRow();
							directions.InstrumentHeight = setup.InstrumentHeight;
							directions.StartPointName = setup.SetupPointName;
							directions.ReflectorHeight = th;
							directions.EndPointName = endPointName;
							directions.ValueApriori = dir;
							if (distanceForUncertaintyModel > 0)
							{
								directions.DistanceApriori = distanceForUncertaintyModel;
							}
							this.directions.Add(directions);
						}
					}
					// Speichere Richtungen, da diese Satzweise zu halten sind
					this.saveObservationGroups(false);
				}

				// GNSS-Vector
				xpathPattern = "//landxml:LandXML/landxml:Survey//landxml:GPSVector";
				nodeList = (NodeList)XMLUtilities.xpathSearch(document, xpathPattern, namespaceContext, XPathConstants.NODESET);
				for (int i = 0; i < nodeList.getLength(); i++)
				{
					Node node = nodeList.item(i);
					NamedNodeMap attr = node.getAttributes();

					string startPointName = attr.getNamedItem("setupID_A") == null ? null : attr.getNamedItem("setupID_A").getNodeValue();
					string endPointName = attr.getNamedItem("setupID_B") == null ? null : attr.getNamedItem("setupID_B").getNodeValue();

					if (string.ReferenceEquals(startPointName, null) || string.ReferenceEquals(endPointName, null))
					{
						continue;
					}

					xpathPattern = "//landxml:LandXML/landxml:Survey//landxml:GPSSetup[@id=\"%s\"]//landxml:TargetPoint[1]";
					Node startNode = (Node)XMLUtilities.xpathSearch(document, String.format(Locale.ENGLISH, xpathPattern, startPointName), namespaceContext, XPathConstants.NODE);
					Node targetNode = (Node)XMLUtilities.xpathSearch(document, String.format(Locale.ENGLISH, xpathPattern, endPointName), namespaceContext, XPathConstants.NODE);

					if (startNode == null || targetNode == null)
					{
						continue;
					}

					// Bestimme Punktnummern der Basislinien
					startPointName = startNode.getAttributes().getNamedItem("name") == null ? null : startNode.getAttributes().getNamedItem("name").getNodeValue();
					endPointName = targetNode.getAttributes().getNamedItem("name") == null ? null : targetNode.getAttributes().getNamedItem("name").getNodeValue();

					if (string.ReferenceEquals(startPointName, null) || string.ReferenceEquals(endPointName, null))
					{
						continue;
					}

					// Bestimme Koordinaten *in Gebrauchslage* und berechne dx, dy, dz - nutze *nicht* WGS84-Werte, da diese ggf. Undulationen nicht beruecksichtigen
					string[] startPointContent = startNode.getFirstChild().getNodeValue().Trim().Split("\\s+");
					string[] targetPointContent = targetNode.getFirstChild().getNodeValue().Trim().Split("\\s+");

					int vecdim = Math.Min(Math.Min(startPointContent.Length, targetPointContent.Length), this.dim == DimensionType.PLAN ? 2 : 3);

					double[] sxyz = new double[vecdim];
					double[] txyz = new double[vecdim];
					try
					{
						for (int p = 0; p < vecdim; p++)
						{
							sxyz[p] = this.convertToMeter(double.Parse(startPointContent[p]));
							txyz[p] = this.convertToMeter(double.Parse(targetPointContent[p]));
						}
					}
					catch (System.FormatException)
					{
						sxyz = null;
						txyz = null;
					}

					// erzeuge Vektor
					if (sxyz != null && txyz != null)
					{
						GNSSObservationRow gnss = new GNSSObservationRow();
						gnss.StartPointName = startPointName;
						gnss.EndPointName = endPointName;
						if (vecdim != 1)
						{
							gnss.XApriori = this.convertToMeter(txyz[0] - sxyz[0]);
							gnss.YApriori = this.convertToMeter(txyz[1] - sxyz[1]);
						}
						if (vecdim != 2)
						{
							gnss.ZApriori = this.convertToMeter(txyz[vecdim - 1] - sxyz[vecdim - 1]);
						}

						if (vecdim == 2)
						{
							this.gnss2D.Add(gnss);
						}
						else
						{
							this.gnss3D.Add(gnss);
						}
					}
				}

				string itemName = this.createItemName(null, null);

				// Speichere Punkte
				if (this.points2d.Count > 0)
				{
					this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_2D_LEAF, this.points2d);
				}

				if (this.dim == DimensionType.SPATIAL && this.points3d.Count > 0)
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
					new ExtensionFilter(I18N.Instance.getString("HeXMLReader.extension.hexml", "LandXML/HeXML"), "*.hexml", "*.HEXML"),
					new ExtensionFilter(I18N.Instance.getString("HeXMLReader.extension.xml", "Extensible Markup Language"), "*.xml", "*.XML")
				};
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveObservationGroups(boolean forceSaving) throws java.sql.SQLException
		private void saveObservationGroups(bool forceSaving)
		{
			// Import von terrestrischen Beobachtungen
			if (this.directions.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.DIRECTION)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.DIRECTION_LEAF, this.directions);
			}

			if (this.horizontalDistances.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.HORIZONTAL_DISTANCE)))
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

			// Import von (relativen) GNSS-Messungen
			if (this.gnss2D.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.GNSS2D)))
			{
				this.lastTreeItem = this.saveGNSSGroup(TreeItemType.GNSS_2D_LEAF, this.gnss2D);
			}

			if (this.dim == DimensionType.SPATIAL && this.gnss3D.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.GNSS3D)))
			{
				this.lastTreeItem = this.saveGNSSGroup(TreeItemType.GNSS_3D_LEAF, this.gnss3D);
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
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveGNSSGroup(org.applied_geodesy.jag3d.ui.tree.TreeItemType itemType, java.util.List<org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow> observations) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveGNSSGroup(TreeItemType itemType, IList<GNSSObservationRow> observations)
		{
			TreeItem<TreeItemValue> treeItem = null;
			if (observations.Count > 0)
			{
				bool isGroupWithEqualStation = ImportOption.Instance.isGroupSeparation(TreeItemType.getObservationTypeByTreeItemType(itemType));
				string itemName = this.createItemName(null, isGroupWithEqualStation && !string.ReferenceEquals(observations[0].StartPointName, null) ? " (" + observations[0].StartPointName + ")" : null);
				treeItem = this.saveGNSSObservations(itemName, itemType, observations);
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
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveGNSSObservations(String itemName, org.applied_geodesy.jag3d.ui.tree.TreeItemType treeItemType, java.util.List<org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow> gnssObservations) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveGNSSObservations(string itemName, TreeItemType treeItemType, IList<GNSSObservationRow> gnssObservations)
		{
			if (gnssObservations == null || gnssObservations.Count == 0)
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
				foreach (GNSSObservationRow row in gnssObservations)
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
	}

}