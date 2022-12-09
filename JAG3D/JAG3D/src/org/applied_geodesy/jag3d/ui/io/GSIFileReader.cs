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

namespace org.applied_geodesy.jag3d.ui.io
{

	using Constant = org.applied_geodesy.adjustment.Constant;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using PointRow = org.applied_geodesy.jag3d.ui.table.row.PointRow;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using PointTreeItemValue = org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using org.applied_geodesy.util.io;

	using TreeItem = javafx.scene.control.TreeItem;
	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;

	public class GSIFileReader : SourceFileReader<TreeItem<TreeItemValue>>
	{

		private sealed class UnitType
		{
			public static readonly UnitType NONE = new UnitType("NONE", InnerEnum.NONE, -1);
			public static readonly UnitType METER = new UnitType("METER", InnerEnum.METER, 0);
			public static readonly UnitType FEET = new UnitType("FEET", InnerEnum.FEET, 1);
			public static readonly UnitType GRADIAN = new UnitType("GRADIAN", InnerEnum.GRADIAN, 2);
			public static readonly UnitType DECIMAL_DEGREE = new UnitType("DECIMAL_DEGREE", InnerEnum.DECIMAL_DEGREE, 3);
			public static readonly UnitType SEXAGESIMAL_DEGREE = new UnitType("SEXAGESIMAL_DEGREE", InnerEnum.SEXAGESIMAL_DEGREE, 4);
			public static readonly UnitType MIL = new UnitType("MIL", InnerEnum.MIL, 5);
			public static readonly UnitType METER_10 = new UnitType("METER_10", InnerEnum.METER_10, 6);
			public static readonly UnitType FEET_10000 = new UnitType("FEET_10000", InnerEnum.FEET_10000, 7);
			public static readonly UnitType METER_100 = new UnitType("METER_100", InnerEnum.METER_100, 8);

			private static readonly List<UnitType> valueList = new List<UnitType>();

			static UnitType()
			{
				valueList.Add(NONE);
				valueList.Add(METER);
				valueList.Add(FEET);
				valueList.Add(GRADIAN);
				valueList.Add(DECIMAL_DEGREE);
				valueList.Add(SEXAGESIMAL_DEGREE);
				valueList.Add(MIL);
				valueList.Add(METER_10);
				valueList.Add(FEET_10000);
				valueList.Add(METER_100);
			}

			public enum InnerEnum
			{
				NONE,
				METER,
				FEET,
				GRADIAN,
				DECIMAL_DEGREE,
				SEXAGESIMAL_DEGREE,
				MIL,
				METER_10,
				FEET_10000,
				METER_100
			}

			public readonly InnerEnum innerEnumValue;
			private readonly string nameValue;
			private readonly int ordinalValue;
			private static int nextOrdinal = 0;

			internal int id;
			internal UnitType(string name, InnerEnum innerEnum, int id)
			{
				this.id = id;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			public static UnitType getEnumByValue(int value)
			{
				foreach (UnitType element in UnitType.values())
				{
					if (element.id == value)
					{
						return element;
					}
				}
				return NONE;
			}

			public static UnitType[] values()
			{
				return valueList.ToArray();
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static UnitType valueOf(string name)
			{
				foreach (UnitType enumInstance in UnitType.valueList)
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		private readonly DimensionType dim;
		private bool isNewStation = false, isNewLevelingLine = false;
		private ISet<string> reservedNames = null;
		private IDictionary<string, PointRow> pointMap = new Dictionary<string, PointRow>();
		private string startPointName = null, endPointName = null, lastStartPointName = null, lastLevelingLineName = null;
		private LevelingData levelingData = null;

		private double ih = 0.0, th = 0.0;
		private double? rb1 = null, vb1 = null, distRb1 = null, distVb1 = null;
		private double? rb2 = null, vb2 = null, distRb2 = null, distVb2 = null;
		private double? zb = null, distZb = null, lastRb4Zb = null, distLastRb4Zb;

		private IList<PointRow> points1d = null;
		private IList<PointRow> points2d = null;
		private IList<PointRow> points3d = null;

		private IList<TerrestrialObservationRow> leveling = null;

		private IList<TerrestrialObservationRow> horizontalDistances = null;
		private IList<TerrestrialObservationRow> directions = null;

		private IList<TerrestrialObservationRow> slopeDistances = null;
		private IList<TerrestrialObservationRow> zenithAngles = null;

		private TreeItem<TreeItemValue> lastTreeItem = null;

		public GSIFileReader(DimensionType dim)
		{
			this.dim = dim;
			this.reset();
		}

		public GSIFileReader(Path p, DimensionType dim) : base(p)
		{
			this.dim = dim;
			this.reset();
		}

		public GSIFileReader(File f, DimensionType dim) : this(f.toPath(), dim)
		{
		}

		public GSIFileReader(string s, DimensionType dim) : this(new File(s), dim)
		{
		}

		public override void reset()
		{
			this.isNewStation = false;
			this.isNewLevelingLine = false;

			this.startPointName = null;
			this.endPointName = null;
			this.ih = 0.0;
			this.th = 0.0;
			this.rb1 = null;
			this.vb1 = null;
			this.rb2 = null;
			this.vb2 = null;
			this.zb = null;
			this.distRb1 = null;
			this.distVb1 = null;
			this.distRb2 = null;
			this.distVb2 = null;
			this.distZb = null;
			this.lastRb4Zb = null;
			this.distLastRb4Zb = null;
			this.levelingData = null;

			if (this.points1d == null)
			{
				this.points1d = new List<PointRow>();
			}
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
			if (this.reservedNames == null)
			{
				this.reservedNames = new HashSet<string>();
			}

			this.reservedNames.Clear();

			this.points1d.Clear();
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
			this.reservedNames = SQLManager.Instance.FullPointNameSet;
			this.lastTreeItem = null;
			this.ignoreLinesWhichStartWith("#");

			base.read();

			string itemName = this.createItemName(null, null);

			// Speichere Punkte
			if ((this.dim == DimensionType.HEIGHT || this.dim == DimensionType.PLAN_AND_HEIGHT) && this.points1d.Count > 0)
			{
				this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_1D_LEAF, this.points1d);
			}

			if ((this.dim == DimensionType.PLAN || this.dim == DimensionType.PLAN_AND_HEIGHT) && this.points2d.Count > 0)
			{
				this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_2D_LEAF, this.points2d);
			}

			if ((this.dim == DimensionType.SPATIAL || this.dim == DimensionType.PLAN_AND_HEIGHT) && this.points3d.Count > 0)
			{
				this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_3D_LEAF, this.points3d);
			}

			// Speichere Daten
			this.saveObservationGroups(true);

			this.reset();

			return this.lastTreeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void parse(String line) throws java.sql.SQLException
		public override void parse(string line)
		{
			// entferne Withspaces und fuege am Ende ein Leerzeichen an (GSI-Datensatzblockende)
			line = line.Trim() + " ";
			// Bestimme das GSI-Format GSI8 bzw. GSI16
			int gsiType = line.StartsWith("*", StringComparison.Ordinal) ? 16 : 8;

			// Wenn GSI16 - entferne fuehrenden *
			if (gsiType == 16)
			{
				line = line.Substring(1);
			}

			//String dataBlocks[] = line.split("\\s+");
			//String blockTemplate = String.format(Locale.ENGLISH, "%%-%ds", (7 + gsiType));

			string pointName = null, pointCode = "";
			double? ih = null, th = null;
			double? deltaH = null, dir = null, dist2d = null, dist3d = null, zenith = null;
			double? x = null, y = null, z = null;

			int startIdx = 0;
			int endIdx = 7 + gsiType;

			while (endIdx < line.Length)
			{
				string dataBlock = line.Substring(startIdx, endIdx - startIdx);
				startIdx = endIdx + 1;
				endIdx = startIdx + 7 + gsiType;

	//		for (String dataBlock : dataBlocks) {
	//			// Pruefe, ob Datenblock genuegend Zeichen enthaelt
	//			// 7 Schluesselzeichen, <gsiType> Datenzeichen, Leerzeichen
	//			if (dataBlock.length() < 7 + gsiType) 
	//				dataBlock = String.format(Locale.ENGLISH, blockTemplate, dataBlock);
	//			else if (dataBlock.length() > 7 + gsiType) 
	//				continue;

				// Bestimme die Laenge der Wortidentifikation 2 oder 3
				int wordIndexLength = dataBlock[2] == '.' ? 2 : 3;
				// Zerlege Zeichenkette
				try
				{
					int key = int.Parse(dataBlock.Substring(0, wordIndexLength));
					UnitType unit = UnitType.getEnumByValue(dataBlock[5] == '.'?-1:(int)char.GetNumericValue(dataBlock[5]));
					int sign = dataBlock[6] == '-' ? -1 : 1;
					string data = dataBlock.Substring(7, gsiType);

					if (key == 11 || key > 100 && key / 10 == 11)
					{
						pointName = this.removeLeadingZeros(data).Trim();
						if (pointName.Length == 0)
						{
							pointName = "0";
						}
					}

					else if (key == 41 || key > 400 && key / 10 == 41)
					{
						pointCode = this.removeLeadingZeros(data).Trim();
						this.isNewLevelingLine = pointCode.matches("^\\?\\.*\\d$") && this.dim == DimensionType.HEIGHT;
					}

					else if (key == 271 && this.dim == DimensionType.HEIGHT)
					{
						this.lastLevelingLineName = this.removeLeadingZeros(data).Trim();
					}

					else if (isNumericValue(key) && data.matches("\\d+"))
					{
						if (key == 84 || key == 85 || key == 86 || key == 331 || key == 335) //  || key == 88
						{
							this.isNewStation = true;
						}

						switch (key)
						{
						// Hz-Winkel
						case 21:
							dir = this.convertToTrueValue(unit, sign, data);
							break;
							// V-Winkel
						case 22:
							zenith = this.convertToTrueValue(unit, sign, data);
							break;
							// Dist2D
						case 32:
							dist2d = this.convertToTrueValue(unit, sign, data);
							dist2d = dist2d == 0?null:dist2d;
							break;
							// Dist3D
						case 31:
							dist3d = this.convertToTrueValue(unit, sign, data);
							dist3d = dist3d == 0?null:dist3d;
							break;
							// deltaH
						case 33:
							deltaH = this.convertToTrueValue(unit, sign, data);
							break;
							// Koordinaten
						case 81:
						case 84:
							y = this.convertToTrueValue(unit, sign, data);
							break;
						case 82:
						case 85:
							x = this.convertToTrueValue(unit, sign, data);
							break;
						case 83:
						case 86:
							z = this.convertToTrueValue(unit, sign, data);
							break;
							// Zielpunkthoehe
						case 87:
							th = this.convertToTrueValue(unit, sign, data);
							break;
							// Standpunkthoehe
						case 88:
							ih = this.convertToTrueValue(unit, sign, data);
							break;

							// Nivellement
						case 331:
							this.rb1 = this.convertToTrueValue(unit, sign, data);
							this.lastRb4Zb = this.rb1;
							break;
						case 335:
							this.rb2 = this.convertToTrueValue(unit, sign, data);
							this.lastRb4Zb = this.lastRb4Zb == null ? this.rb2 : 0.5 * (this.lastRb4Zb.Value + this.rb2.Value);
							break;
						case 332:
							this.vb1 = this.convertToTrueValue(unit, sign, data);
							break;
						case 336:
							this.vb2 = this.convertToTrueValue(unit, sign, data);
							break;
						case 333:
							this.zb = this.convertToTrueValue(unit, sign, data);
							break;
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					continue;
				}
			}

			// Reduziere Richtungen und Zenitwinkel auf Lage I
			if (this.dim != DimensionType.HEIGHT)
			{
				bool isFaceI = !(zenith != null && !double.IsNaN(zenith) && !double.IsInfinity(zenith) && zenith > Math.PI);
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
			}

			// Speichere die (neue) Standpunktnummer und speichere den bisherigen Richtungssatz des alten Standpunkes
			if (this.isNewStation && !string.ReferenceEquals(pointName, null) && !pointName.Trim().Length == 0)
			{
				this.startPointName = pointName;
				this.ih = ih == null ? 0.0 : ih.Value;
				this.th = th == null ? 0.0 : th.Value;

				// Strecken beim Niv sind hier zu reseten, da Zwischenblicke immer nach dem Vorblick kommen.
				this.distRb1 = null;
				this.distRb2 = null;
				this.distLastRb4Zb = null;

				// Speichere Daten bspw. Richtungen, da diese Satzweise zu halten sind
				this.saveObservationGroups(false);
				this.lastStartPointName = null;
			}
			else if (!string.ReferenceEquals(pointName, null) && pointName.Trim().Length > 0)
			{
				this.endPointName = pointName;
			}
			else if (this.isNewLevelingLine)
			{
				this.saveObservationGroups(this.dim == DimensionType.HEIGHT);
				this.lastLevelingLineName = null;
			}

			// Speichere Punkte aus der GSI
			if (!string.ReferenceEquals(pointName, null) && pointName.Trim().Length > 0 && !this.reservedNames.Contains(pointName) && !this.pointMap.ContainsKey(pointName))
			{
				PointRow point = new PointRow();
				point.Name = pointName;
				point.Code = pointCode;
				point.XApriori = x;
				point.YApriori = y;
				point.ZApriori = z;

				if ((this.dim == DimensionType.SPATIAL || this.dim == DimensionType.PLAN_AND_HEIGHT) && x != null && y != null && z != null)
				{
					this.pointMap[pointName] = point;
					this.points3d.Add(point);
				}
				else if ((this.dim == DimensionType.PLAN || this.dim == DimensionType.PLAN_AND_HEIGHT) && x != null && y != null)
				{
					this.pointMap[pointName] = point;
					this.points2d.Add(point);
				}
				else if ((this.dim == DimensionType.HEIGHT || this.dim == DimensionType.PLAN_AND_HEIGHT) && z != null)
				{
					this.pointMap[pointName] = point;
					this.points1d.Add(point);
				}
			}
			else if (this.pointMap.ContainsKey(pointName))
			{
				PointRow point = this.pointMap[pointName];
				if (this.hasZerosCoordinates(point))
				{
					if ((this.dim == DimensionType.SPATIAL || this.dim == DimensionType.PLAN_AND_HEIGHT) && x != null && y != null && z != null)
					{
						point.XApriori = x;
						point.YApriori = y;
						point.ZApriori = z;
					}
					else if ((this.dim == DimensionType.PLAN || this.dim == DimensionType.PLAN_AND_HEIGHT) && x != null && y != null)
					{
						point.XApriori = x;
						point.YApriori = y;
					}
					else if ((this.dim == DimensionType.HEIGHT || this.dim == DimensionType.PLAN_AND_HEIGHT) && z != null)
					{
						point.ZApriori = z;
					}
				}
			}

			// Speichere Beobachtungen in den Gruppen
			if (!string.ReferenceEquals(this.startPointName, null))
			{
				// Pruefe, ob Werte neu gesetzt wurden
				this.ih = ih == null ? this.ih : ih.Value;
				this.th = th == null ? this.th : th.Value;

				// Speichere Abstand zwischen 1. Rueckblick und Instrument
				if (this.rb1 != null && dist2d != null && this.distRb1 == null)
				{
					this.distRb1 = dist2d;
					this.distLastRb4Zb = dist2d;
					dist2d = null;
				}
				// Speichere Abstand zwischen 2. Rueckblick und Instrument
				else if (this.rb2 != null && dist2d != null && this.distRb2 == null)
				{
					this.distRb2 = dist2d;
					this.distLastRb4Zb = this.distLastRb4Zb == null ? dist2d : 0.5 * (this.distLastRb4Zb.Value + dist2d.Value);
					dist2d = null;
				}
				// Speichere Abstand zwischen 1. Vorblick und Instrument
				else if (this.vb1 != null && dist2d != null && this.distVb1 == null)
				{
					this.distVb1 = dist2d;
					dist2d = null;
				}
				// Speichere Abstand zwischen 2. Vorblick und Instrument
				else if (this.vb2 != null && dist2d != null && this.distVb2 == null)
				{
					this.distVb2 = dist2d;
					dist2d = null;
				}
				// Speichere Abstand zwischen Zwischenblick und Instrument
				else if (this.zb != null && dist2d != null && this.distZb == null)
				{
					this.distZb = dist2d;
					dist2d = null;
				}

				// Fuege Beobachtungen hinzu
				if (!string.ReferenceEquals(this.startPointName, null) && !string.ReferenceEquals(this.endPointName, null))
				{
					if (string.ReferenceEquals(this.lastStartPointName, null))
					{
						this.lastStartPointName = this.startPointName;
					}

					/// <summary>
					/// Nivellement * </summary>
					// ermittle Hoehenunterschied aus 0.5 * (RI+RII) und ZB
					if (this.lastRb4Zb != null && this.zb != null)
					{
						// speichere letzten Datensatz
						this.addLevelingData(this.levelingData);
						this.levelingData = null;

						LevelingData sideLevelingData = new LevelingData();
						sideLevelingData.addBackSightReading(this.startPointName, this.lastRb4Zb.Value, this.distLastRb4Zb != null ? this.distLastRb4Zb.Value : 0, true);
						sideLevelingData.addForeSightReading(this.endPointName, this.zb.Value, this.distZb != null ? this.distZb.Value : 0, true);

						// speichere Zwischenblick direkt, da keine zweite Messung moeglich
						this.addLevelingData(sideLevelingData);
						sideLevelingData = null;

						this.zb = null;
						this.distZb = null;
					}

					// ermittle Hoehenunterschied aus R1 und V1
					if (this.rb1 != null && this.vb1 != null)
					{
						// speichere letzten Datensatz
						this.addLevelingData(this.levelingData);
						this.levelingData = null;

						// erzeuge neuen Datensatz
						if (this.levelingData == null)
						{
							this.levelingData = new LevelingData();
						}

						this.levelingData.addBackSightReading(this.startPointName, this.rb1.Value, this.distRb1 != null ? this.distRb1.Value : 0, true);
						this.levelingData.addForeSightReading(this.endPointName, this.vb1.Value, this.distVb1 != null ? this.distVb1.Value : 0, true);

						this.rb1 = this.vb1 = null;
						this.distRb1 = this.distVb1 = null;
					}

					// ermittle Hoehenunterschied aus R2 und V2
					if (this.levelingData != null && this.rb2 != null && this.vb2 != null)
					{
						this.levelingData.addBackSightReading(this.startPointName, this.rb2.Value, this.distRb2 != null ? this.distRb2.Value : 0, false);
						this.levelingData.addForeSightReading(this.endPointName, this.vb2.Value, this.distVb2 != null ? this.distVb2.Value : 0, false);

						// speichere Datensatz
						this.addLevelingData(this.levelingData);
						this.levelingData = null;

						this.rb2 = this.vb2 = null;
						this.distRb2 = this.distVb2 = null;
					}

					/// <summary>
					/// Tachymetrie * </summary>
					if (deltaH != null)
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = this.endPointName;

						// Delta-Hs sind bereits um ih und th korrigiert, daher Null
						//obs.setInstrumentHeight(this.ih);
						//obs.setReflectorHeight(this.th);

						obs.InstrumentHeight = 0.0;
						obs.ReflectorHeight = 0.0;

						if (dist2d != null && dist2d > 0)
						{
							obs.DistanceApriori = dist2d;
						}

						obs.ValueApriori = deltaH;
						this.leveling.Add(obs);
					}

					if (dir != null)
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = this.endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = this.th;
						obs.ValueApriori = dir;
						if (this.dim == DimensionType.SPATIAL && dist3d != null && dist3d > 0)
						{
							obs.DistanceApriori = dist3d;
						}
						else if ((this.dim == DimensionType.PLAN || this.dim == DimensionType.PLAN_AND_HEIGHT) && dist2d != null && dist2d > 0)
						{
							obs.DistanceApriori = dist2d;
						}
						this.directions.Add(obs);
					}

					if (dist2d != null)
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = this.endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = this.th;
						obs.ValueApriori = dist2d;
						obs.DistanceApriori = dist2d;
						this.horizontalDistances.Add(obs);
					}

					if (dist3d != null)
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = this.endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = this.th;
						obs.ValueApriori = dist3d;
						obs.DistanceApriori = dist3d;
						this.slopeDistances.Add(obs);
					}

					if (zenith != null)
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = this.endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = this.th;
						obs.ValueApriori = zenith;
						if (dist3d != null && dist3d > 0)
						{
							obs.DistanceApriori = dist3d;
						}
						this.zenithAngles.Add(obs);
					}
				}
			}
			this.isNewStation = false;
			this.isNewLevelingLine = false;
		}

		private bool hasZerosCoordinates(PointRow point)
		{
			double? x = point.XApriori;
			double? y = point.YApriori;
			double? z = point.ZApriori;

			if (x != null && x != 0)
			{
				return false;
			}
			if (y != null && y != 0)
			{
				return false;
			}
			if (z != null && z != 0)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Bestimmt anhand der Einheit den wahren Wert in Meter bzw. RAD
		/// </summary>
		/// <param name="unit"> </param>
		/// <param name="sign"> </param>
		/// <param name="data"> </param>
		/// <returns> value </returns>
		/// <exception cref="NumberFormatException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private double convertToTrueValue(UnitType unit, int sign, String data) throws NumberFormatException
		private double convertToTrueValue(UnitType unit, int sign, string data)
		{
			data = this.removeLeadingZeros(data);
			double value = 0.0;
			if (data.Length > 0)
			{
				value = double.Parse(data);
			}

			switch (unit.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.METER:
				value = value / 1000.0;
				break;
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.FEET:
				value = value / 1000.0 * 0.3048;
				break;
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.GRADIAN:
				value = value / 100000.0 * Constant.RHO_GRAD2RAD;
				break;
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.DECIMAL_DEGREE:
				value = value / 100000.0 * Constant.RHO_DEG2RAD;
				break;
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.SEXAGESIMAL_DEGREE:
				double dd = (int)value / 100000;
				value -= dd * 100000;
				double mm = (int)value / 1000;
				value -= mm * 1000;
				double ss = value / 10.0;
				value = (dd + mm / 60 + ss / 3600) * Constant.RHO_DEG2RAD;
				break;
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.MIL:
				value = value / 10000.0 * Constant.RHO_MIL2RAD;
				break;
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.METER_10:
				value = value / 10000.0;
				break;
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.FEET_10000:
				value = value / 100000.0 * 0.3048;
				break;
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.METER_100:
				value = value / 100000.0;
				break;
			case org.applied_geodesy.jag3d.ui.io.GSIFileReader.UnitType.InnerEnum.NONE:
				break;
			}

			return sign * value;
		}

		/// <summary>
		/// Entfernt fuehrende Nullen </summary>
		/// <param name="str"> </param>
		/// <returns> nonZeroStartString </returns>
		private string removeLeadingZeros(string str)
		{
			return str.replaceAll("^0+", "");
		}

		/// <summary>
		/// Liefert true, wenn der Wert des Schluessels eine Zahl sein soll/muss </summary>
		/// <param name="key"> </param>
		/// <returns> isNumericValue </returns>
		private bool isNumericValue(int key)
		{
			return !(key % 10 == 0 || key <= 19 || key >= 41 && key <= 49 || key >= 70 && key <= 79);
		}

		public static ExtensionFilter[] ExtensionFilters
		{
			get
			{
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("GSIFileReader.extension.gsi", "Geo Serial Interface (GSI)"), "*.gsi", "*.GSI")};
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveObservationGroups(boolean forceSaving) throws java.sql.SQLException
		private void saveObservationGroups(bool forceSaving)
		{
			// Speichere Daten bspw. Richtungen, da diese Satzweise zu halten sind
			if ((this.dim == DimensionType.HEIGHT || this.dim == DimensionType.PLAN_AND_HEIGHT) && this.leveling.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.LEVELING)))
			{
				this.addLevelingData(this.levelingData);
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.LEVELING_LEAF, this.leveling);
			}

			if (this.dim != DimensionType.HEIGHT && this.directions.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.DIRECTION)))
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
				string groupName = null;
				if (!string.ReferenceEquals(this.lastLevelingLineName, null) && !this.lastLevelingLineName.Trim().Length == 0)
				{
					groupName = this.lastLevelingLineName;
				}
				else if (isGroupWithEqualStation && !string.ReferenceEquals(this.lastStartPointName, null))
				{
					groupName = this.lastStartPointName;
				}
				string itemName = this.createItemName(null, !string.ReferenceEquals(groupName, null) ? " (" + groupName + ")" : null);
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

		private void addLevelingData(LevelingData levelingData)
		{
			if (levelingData != null)
			{
				double dist2D = levelingData.Distance;
				double deltaH = levelingData.DeltaH;

				TerrestrialObservationRow obs = new TerrestrialObservationRow();
				obs.StartPointName = levelingData.StartPointName;
				obs.EndPointName = levelingData.EndPointName;

				obs.InstrumentHeight = 0.0;
				obs.ReflectorHeight = 0.0;

				if (dist2D > 0)
				{
					obs.DistanceApriori = dist2D;
				}

				obs.ValueApriori = deltaH;
				this.leveling.Add(obs);
			}
		}
	}

}