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

	public class ZFileReader : SourceFileReader<TreeItem<TreeItemValue>>
	{
		private IDictionary<string, double> targetHeights = new Dictionary<string, double>();

		private ISet<string> pointNames = null;

		private IDictionary<string, string> pointNames1D = null;
		private IDictionary<string, string> pointNames2D = null;
		private IDictionary<string, string> pointNames3D = null;

		private IList<PointRow> points1d = null;
		private IList<PointRow> points2d = null;
		private IList<PointRow> points3d = null;

		private string startPointName = null, startPointCode = "";
		private double ih = 0.0;
		private int cnt = 0;
		private bool is2DStartPoint = false;

		private IList<TerrestrialObservationRow> leveling = null;

		private IList<TerrestrialObservationRow> horizontalDistances = null;
		private IList<TerrestrialObservationRow> directions = null;

		private IList<TerrestrialObservationRow> slopeDistances = null;
		private IList<TerrestrialObservationRow> zenithAngles = null;

		private LevelingData levelingData = null;
		private string lastPointId = null, project = null;
		private double? rb = null, distRb = null;

		private TreeItem<TreeItemValue> lastTreeItem = null;

		public ZFileReader()
		{
			this.reset();
		}

		public ZFileReader(string s) : this(new File(s))
		{
		}

		public ZFileReader(File f) : this(f.toPath())
		{
		}

		public ZFileReader(Path p) : base(p)
		{
			this.reset();
		}

		public override void reset()
		{
			this.levelingData = null;
			this.lastPointId = null;
			this.startPointName = null;
			this.startPointCode = "";
			this.is2DStartPoint = false;

			this.ih = 0.0;
			this.rb = null;
			this.distRb = null;

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
			if (this.pointNames == null)
			{
				this.pointNames = new HashSet<string>();
			}

			if (this.pointNames1D == null)
			{
				this.pointNames1D = new LinkedHashMap<string, string>();
			}
			if (this.pointNames2D == null)
			{
				this.pointNames2D = new LinkedHashMap<string, string>();
			}
			if (this.pointNames3D == null)
			{
				this.pointNames3D = new LinkedHashMap<string, string>();
			}

			this.pointNames.Clear();

			this.pointNames1D.Clear();
			this.pointNames2D.Clear();
			this.pointNames3D.Clear();

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
			this.pointNames.addAll(SQLManager.Instance.FullPointNameSet);
			this.lastTreeItem = null;
			this.ignoreLinesWhichStartWith("!");

			base.read();

			string itemName = this.createItemName(null, null);
			this.ignoreLinesWhichStartWith("!");

			// Speichere Daten
			this.saveObservationGroups(true);

			// Normalisiere Punkte
			this.normalizePointNames();

			// Erzeuge Punkte
			if (this.pointNames1D.Count > 0)
			{
				foreach (KeyValuePair<string, string> entry in this.pointNames1D.SetOfKeyValuePairs())
				{
					string pid = entry.Key;
					string code = entry.Value;
					if (!this.pointNames.Contains(pid))
					{
						PointRow point = new PointRow();
						point.Name = pid;
						point.Code = code;
						this.points1d.Add(point);
					}
				}
			}
			if (this.pointNames2D.Count > 0)
			{
				foreach (KeyValuePair<string, string> entry in this.pointNames2D.SetOfKeyValuePairs())
				{
					string pid = entry.Key;
					string code = entry.Value;
					if (!this.pointNames.Contains(pid))
					{
						PointRow point = new PointRow();
						point.Name = pid;
						point.Code = code;
						this.points2d.Add(point);
					}
				}
			}
			if (this.pointNames3D.Count > 0)
			{
				foreach (KeyValuePair<string, string> entry in this.pointNames3D.SetOfKeyValuePairs())
				{
					string pid = entry.Key;
					string code = entry.Value;
					if (!this.pointNames.Contains(pid))
					{
						PointRow point = new PointRow();
						point.Name = pid;
						point.Code = code;
						this.points3d.Add(point);
					}
				}
			}

			// Speichere Punkte
			if (this.pointNames1D.Count > 0)
			{
				this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_1D_LEAF, this.points1d);
			}

			if (this.pointNames2D.Count > 0)
			{
				this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_2D_LEAF, this.points2d);
			}

			if (this.pointNames3D.Count > 0)
			{
				this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_3D_LEAF, this.points3d);
			}

			this.pointNames.addAll(this.pointNames1D.Keys);
			this.pointNames.addAll(this.pointNames2D.Keys);
			this.pointNames.addAll(this.pointNames3D.Keys);

			this.pointNames1D.Clear();
			this.pointNames2D.Clear();
			this.pointNames3D.Clear();

			return this.lastTreeItem;
		}

		public override void parse(string line)
		{
			try
			{
				if (line.Length < 32 || !line.Substring(5, 2).matches("\\d{2}"))
				{
					return;
				}

				int c1 = int.Parse(line.Substring(5, 1));
				int c2 = int.Parse(line.Substring(6, 1));

				string key1 = line.Substring(8, 7).Trim();
				string key2 = line.Substring(16, 16).Trim();

				if (c1 < 1)
				{
					this.saveObservationGroups(false);
				}

				if (c1 == 0 && c2 == 4)
				{
					this.project = line.Substring(16, 16).Trim();
				}

				// 10 --> Standpunkt
				if (c1 == 1 && c2 == 0)
				{
					//Zwischenspeichern der Gruppen bspw. der Richtungen, da diese eine Orientierungsunbekannte haben
					this.saveObservationGroups(false);

					// Neuer Standpunkt
					this.startPointName = key2;
					this.startPointCode = "";
					this.ih = 0;
					this.targetHeights.Clear();

					if (key1.Substring(2).matches("\\d+"))
					{
						this.is2DStartPoint = key1.Substring(2).matches("^9+$");
						if (!this.is2DStartPoint)
						{
							try
							{
							this.ih = 0.001 * int.Parse(key1.Substring(2).Trim());
							}
						catch (Exception)
						{
						}
						}
					}

					if (line.Length > 71 && line.Substring(70, 1).Equals("|"))
					{
						string[] attributes = line.Substring(71, line.Length - 71).Split("\\|", true);
						if (attributes.Length > 0)
						{
							this.startPointCode = attributes[0].Trim();
						}
					}
				}

				else if (!string.ReferenceEquals(this.startPointName, null) && this.startPointName.Length > 0 && line.Length >= 71 && c1 >= 2 && c1 <= 8 && c2 >= 0 && c2 <= 4 && line[68] != '*')
				{
					string endPointId = key2;
					double th = 0;
					bool is2DEndPoint = key1.Substring(2).matches("^9+$");
					if (!is2DEndPoint)
					{
						if (key1.Substring(2).matches("^0+$") && this.targetHeights.ContainsKey(endPointId))
						{
							th = this.targetHeights[endPointId];
						}
						else if (key1.Substring(2).matches("\\d+"))
						{
							try
							{
								th = 0.001 * int.Parse(key1.Substring(2).Trim());
							}
							catch (Exception)
							{
							}
							this.targetHeights[endPointId] = th;
						}
					}

					string endPointCode = "";
					if (line.Length > 71 && line.Substring(70, 1).Equals("|"))
					{
						string[] attributes = line.Substring(71, line.Length - 71).Split("\\|", true);
						if (attributes.Length > 0)
						{
							endPointCode = attributes[0].Trim();
						}
					}

					// Distance 3D
					double dist = 0;
					if ((c1 == 2 || c1 == 3) && !this.is2DStartPoint && !is2DEndPoint)
					{
						double dist3D = double.Parse(line.Substring(32, 12).Trim());
						dist = dist3D;
						if (dist3D > 0)
						{
							TerrestrialObservationRow distance3D = this.getObservation(this.startPointName, endPointId, this.ih, th, dist3D);
							distance3D.DistanceApriori = dist;
							this.slopeDistances.Add(distance3D);

							// Fuege Punkte hinzu
							this.addPoint(this.startPointName, this.startPointCode, 3);
							this.addPoint(endPointId, endPointCode, 3);
						}
					}

					// Distance 2D
					if (c1 == 5 || c1 == 6)
					{
						double dist2D = double.Parse(line.Substring(32, 12).Trim());
						dist = dist2D;
						if (dist2D > 0)
						{
							TerrestrialObservationRow distance2D = this.getObservation(this.startPointName, endPointId, this.is2DStartPoint ? 0 : this.ih, is2DEndPoint ? 0 : th, dist2D);
							distance2D.DistanceApriori = dist;
							this.horizontalDistances.Add(distance2D);

							// Fuege Punkte hinzu
							this.addPoint(this.startPointName, this.startPointCode, c1 == 6 || this.is2DStartPoint ? 2 : 3);
							this.addPoint(endPointId, endPointCode, c1 == 6 || is2DEndPoint ? 2 : 3);
						}
					}

					// Direction
					if (c1 == 2 || c1 == 5 || c1 == 8)
					{
						double dir = double.Parse(line.Substring(44, 12).Trim()) * Constant.RHO_GRAD2RAD;
						TerrestrialObservationRow direction = this.getObservation(this.startPointName, endPointId, this.is2DStartPoint ? 0 : this.ih, is2DEndPoint ? 0 : th, dir);
						if (dist > 0)
						{
							direction.DistanceApriori = dist;
						}
						this.directions.Add(direction);

						// Fuege Punkte hinzu
						this.addPoint(this.startPointName, this.startPointCode, c1 == 8 || this.is2DStartPoint ? 2 : 3);
						this.addPoint(endPointId, endPointCode, c1 == 8 || is2DEndPoint ? 2 : 3);
					}

					// Zenith
					if ((c1 == 2 || c1 == 4) && !this.is2DStartPoint && !is2DEndPoint)
					{
						double zenith = double.Parse(line.Substring(56, 12).Trim()) * Constant.RHO_GRAD2RAD;
						TerrestrialObservationRow zenithangle = this.getObservation(this.startPointName, endPointId, this.ih, th, zenith);
						if (dist > 0)
						{
							zenithangle.DistanceApriori = dist;
						}
						this.zenithAngles.Add(zenithangle);

						// Fuege Punkte hinzu
						this.addPoint(this.startPointName, this.startPointCode, 3);
						this.addPoint(endPointId, endPointCode, 3);
					}

					// Delta H
					if ((c1 == 5 || c1 == 7) && !this.is2DStartPoint && !is2DEndPoint)
					{
						double deltaH = double.Parse(line.Substring(56, 12).Trim());
						TerrestrialObservationRow levelling = this.getObservation(this.startPointName, endPointId, this.ih, th, deltaH);
						if (dist > 0)
						{
							levelling.DistanceApriori = dist;
						}
						this.leveling.Add(levelling);

						// Fuege Punkte hinzu
						this.addPoint(this.startPointName, this.startPointCode, c1 == 5 ? 3 : 1);
						this.addPoint(endPointId, endPointCode, c1 == 5 ? 3 : 1);
					}
				}


				else if (line.Length >= 71 && c1 >= 2 && c1 <= 3 && c2 >= 5 && c2 <= 8)
				{
					bool precisionMode = line.Substring(8, 1).Equals("2"); // R-V-V-R
					double sign = c1 == 3 ? -1.0 : 1.0;
					string pointName = line.Substring(16, 16).Trim();
					string code = "";

					double? dist = 0.0, value1 = null, value2 = null;

					if (c2 != 8)
					{
						try
						{
						dist = double.Parse(line.Substring(32, 12).Trim());
						}
					catch (Exception)
					{
					}
					}

					value1 = sign * double.Parse(line.Substring(44, 12).Trim());
					value2 = sign * double.Parse(line.Substring(56, 12).Trim());

					if (line.Length > 71 && line.Substring(70, 1).Equals("|"))
					{
						string[] attributes = line.Substring(71, line.Length - 71).Split("\\|", true);
						if (attributes.Length > 0)
						{
							code = attributes[0].Trim();
						}
					}
					// Rueckblick = 25/35
					if (c2 == 5)
					{
						if ((string.ReferenceEquals(pointName, null) || pointName.Trim().Length == 0 || pointName.Equals("0")) && !string.ReferenceEquals(this.lastPointId, null))
						{
							pointName = this.lastPointId;
						}
						this.startPointName = pointName;
						this.startPointCode = code;
						this.levelingData = new LevelingData();

						this.startPointName = pointName;
						this.rb = value1;
						this.distRb = dist;

						this.levelingData.addBackSightReading(pointName, value1.Value, dist.Value, true);
						if (precisionMode)
						{
							this.levelingData.addBackSightReading(pointName, value2.Value, dist.Value, false);
						}

						this.addPoint(this.startPointName, this.startPointCode, 1);
					}
					// Vorblick/Zwischenblick
					if (this.levelingData != null)
					{
						if (c2 >= 6 && c2 <= 8)
						{
							if (string.ReferenceEquals(pointName, null) || pointName.Trim().Length == 0 || pointName.Equals("0"))
							{
								//pointName = "W"+(++cnt)+"_"+this.getFile().getName().substring(0, this.getFile().getName().lastIndexOf('.'));
								pointName = String.format(Locale.ENGLISH, "%c%07d", 'W', ++this.cnt);
								this.lastPointId = pointName;
							}

							// Kein Vorblick
							if (c2 > 6 && this.rb != null && !string.ReferenceEquals(this.startPointName, null))
							{
								LevelingData sideLevelingData = new LevelingData();
								sideLevelingData.addBackSightReading(this.startPointName, this.rb.Value, this.distRb == null ? 0 : this.distRb.Value);
								sideLevelingData.addForeSightReading(pointName, value1.Value, dist.Value);
								this.addLevelingData(sideLevelingData);
							}

							// Vorblick
							if (c2 == 6)
							{
								this.levelingData.addForeSightReading(pointName, value1.Value, dist.Value, true);
								if (precisionMode)
								{
									this.levelingData.addForeSightReading(pointName, value2.Value, dist.Value, false);
								}
								this.addLevelingData(this.levelingData);
								this.levelingData = null;
							}

							this.addPoint(pointName, code, 1);
						}
					}
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				return;
			}
		}

		private void normalizePointNames()
		{
			foreach (KeyValuePair<string, string> entry in this.pointNames1D.SetOfKeyValuePairs())
			{
				string pid = entry.Key;
				string code = entry.Value;

				// Wenn eine Pkt in 1D und 2D enthalten ist, ist es ein 3D-Punkt
				if (this.pointNames2D.ContainsKey(pid))
				{
					this.pointNames3D[pid] = code.Length == 0 ? this.pointNames2D[pid] : code;
				}
			}

			foreach (string pid in this.pointNames3D.Keys)
			{
				string code = this.pointNames3D[pid];
				if (this.pointNames1D.ContainsKey(pid))
				{
					this.pointNames3D[pid] = code.Length == 0 ? this.pointNames1D[pid] : code;
					this.pointNames1D.Remove(pid);
				}
				if (this.pointNames2D.ContainsKey(pid))
				{
					this.pointNames3D[pid] = code.Length == 0 ? this.pointNames2D[pid] : code;
					this.pointNames2D.Remove(pid);
				}
			}
		}

		private void addPoint(string name, string code, int dim)
		{
			if (dim == 1 && !this.pointNames1D.ContainsKey(name))
			{
				this.pointNames1D[name] = code;
			}

			else if (dim == 2 && !this.pointNames2D.ContainsKey(name))
			{
				this.pointNames2D[name] = code;
			}

			else if (dim == 3 && !this.pointNames3D.ContainsKey(name))
			{
				this.pointNames3D[name] = code;
			}
		}

		private TerrestrialObservationRow getObservation(string startPoint, string endPoint, double ih, double th, double value)
		{
			TerrestrialObservationRow obs = new TerrestrialObservationRow();
			obs.StartPointName = startPoint;
			obs.EndPointName = endPoint;
			obs.InstrumentHeight = ih;
			obs.ReflectorHeight = th;
			obs.ValueApriori = value;
			return obs;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveObservationGroups(boolean forceSaving) throws java.sql.SQLException
		private void saveObservationGroups(bool forceSaving)
		{
			// Speichere Daten
			if (this.leveling.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.LEVELING)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.LEVELING_LEAF, this.leveling);
			}

			if (this.directions.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.DIRECTION)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.DIRECTION_LEAF, this.directions);
			}

			if (this.horizontalDistances.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.HORIZONTAL_DISTANCE)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.HORIZONTAL_DISTANCE_LEAF, this.horizontalDistances);
			}

			if (this.slopeDistances.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.SLOPE_DISTANCE)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.SLOPE_DISTANCE_LEAF, this.slopeDistances);
			}

			if (this.zenithAngles.Count > 0 && (forceSaving || ImportOption.Instance.isGroupSeparation(ObservationType.ZENITH_ANGLE)))
			{
				this.lastTreeItem = this.saveObservationGroup(TreeItemType.ZENITH_ANGLE_LEAF, this.zenithAngles);
			}
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

		public static ExtensionFilter[] ExtensionFilters
		{
			get
			{
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("ZFileReader.extension.z", "Cremer Caplan"), "*.z", "*.Z")};
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveObservationGroup(org.applied_geodesy.jag3d.ui.tree.TreeItemType itemType, java.util.List<org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow> observations) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveObservationGroup(TreeItemType itemType, IList<TerrestrialObservationRow> observations)
		{
			string suffix = string.ReferenceEquals(this.project, null) ? "" : "  " + this.project;
			TreeItem<TreeItemValue> treeItem = null;
			if (observations.Count > 0)
			{
				bool isGroupWithEqualStation = ImportOption.Instance.isGroupSeparation(TreeItemType.getObservationTypeByTreeItemType(itemType));
				string itemName = this.createItemName(null, isGroupWithEqualStation && !string.ReferenceEquals(this.startPointName, null) ? " (" + this.startPointName + ")" : null) + suffix;
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
	}

}