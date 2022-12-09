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

	public class M5FileReader : SourceFileReader<TreeItem<TreeItemValue>>
	{
		private string startPointName = null, loopId = null, lastPointName = null;
		private double? lastRb4Zb = null, distLastRb4Zb;
		private int cnt = 0;
		private LevelingData levelingData = null;
		private ISet<string> pointNames = new HashSet<string>();
		private IList<PointRow> points1d = null;
		private IList<TerrestrialObservationRow> leveling = null;
		private TreeItem<TreeItemValue> lastTreeItem = null;

		public M5FileReader()
		{
			this.reset();
		}

		public M5FileReader(File f) : this(f.toPath())
		{
		}

		public M5FileReader(string s) : this(new File(s))
		{
		}

		public M5FileReader(Path p) : base(p)
		{
			this.reset();
		}

		public override void reset()
		{
			this.levelingData = null;

			if (this.points1d == null)
			{
				this.points1d = new List<PointRow>();
			}

			if (this.leveling == null)
			{
				this.leveling = new List<TerrestrialObservationRow>();
			}

			if (this.pointNames == null)
			{
				this.pointNames = new HashSet<string>();
			}

			this.pointNames.Clear();
			this.points1d.Clear();
			this.leveling.Clear();

			this.startPointName = null;
			this.lastPointName = null;

			this.cnt = 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			this.reset();
			this.pointNames.addAll(SQLManager.Instance.FullPointNameSet);
			this.lastTreeItem = null;
			this.ignoreLinesWhichStartWith("#");

			base.read();

			string itemName = this.createItemName(null, null);

			// Speichere Punkte
			if (this.points1d.Count > 0)
			{
				this.lastTreeItem = this.savePoints(itemName, TreeItemType.DATUM_POINT_1D_LEAF, this.points1d);
			}

			// Speichere Daten
			this.saveLevelingGroup();

			return this.lastTreeItem;
		}

		public override void parse(string line)
		{
			// an error in M5 is denoted by an E after the last |-seperator
			if (!line.ToUpper().StartsWith("FOR M5", StringComparison.Ordinal) || line.Length < 118 || !line.Trim().EndsWith("|", StringComparison.Ordinal))
			{
				return;
			}

			string T2a = line.Substring(17, 3).Trim();
			string value2 = line.Substring(21, 27);

			string T3 = line.Substring(49, 2).Trim();
			string value3 = line.Substring(52, 14);
			string unit3 = line.Substring(67, 4);

			string T4 = line.Substring(72, 2).Trim();
			string value4 = line.Substring(75, 14);
			string unit4 = line.Substring(90, 4);

			string T5 = line.Substring(95, 2).Trim();
			string value5 = line.Substring(98, 14);
			string unit5 = line.Substring(113, 4);

			if (value2.Contains("#####") || !T2a.StartsWith("KD", StringComparison.Ordinal) && !T2a.StartsWith("KN", StringComparison.Ordinal))
			{
				return;
			}

			try
			{
	//			Pattern pattern = Pattern.compile( "\\s*(\\w+).+" );
	//			Matcher matcher = pattern.matcher( value2 );
	//			if (matcher.matches()) {
	//				String pointName = matcher.group(1);

				if (value2.matches("\\s*(\\w+).+") && value2.Length >= 27)
				{
					string pointName = value2.Substring(0, 8).Trim();
					string code = value2.Substring(8, 5).Trim();
					string currentLoopId = value2.Substring(23, 4).Trim();

					if (!string.ReferenceEquals(this.loopId, null) && !this.loopId.Equals(currentLoopId))
					{
						this.saveLevelingGroup();
					}
					this.loopId = currentLoopId;

					// Rueckblick
					if (T3.Equals("Lr") || T3.Equals("Rb"))
					{
						// Pruefe, ob der Rueckblick eine Pkt-Id besitzt, 
						// verwende ansonsten die letzte guelte Bezeichnung
						// --> Manche M5-Files enthalten keine Pkt fuer Wechselpunkte
						if (pointName.Trim().Length == 0)
						{
							pointName = this.lastPointName;
							code = "JAG3D";
						}

						double d = 0;
						double r = double.Parse(value3.Trim());
						r = unit3.ToLower().Trim().Equals("ft") ? 0.3048 * r : r;

						// Pruefe, ob es eine Strecke gibt
						if (T4.Equals("E") || T4.Equals("HD"))
						{
							d = double.Parse(value4.Trim());
							d = unit4.ToLower().Trim().Equals("ft") ? 0.3048 * d : d;
						}

						if (this.levelingData == null)
						{
							this.levelingData = new LevelingData();
							this.startPointName = pointName;
							// Speichere letzten Rueckblick fuer mgl. Zwischenblicke R-I
							this.lastRb4Zb = r;
							this.distLastRb4Zb = d;
						}
						else
						{ // Speichere letzten Rueckblick fuer mgl. Zwischenblicke R-II bei bspw. RVVR
							this.lastRb4Zb = 0.5 * (this.lastRb4Zb.Value + r);
							this.distLastRb4Zb = 0.5 * (this.distLastRb4Zb.Value + d);
						}
						this.levelingData.addBackSightReading(pointName, r, d);
					}
					// Vorblick
					else if (T3.Equals("Lv") || T3.Equals("Rf"))
					{
						// Pruefe, ob der Vorblick eine Pkt-Id besitzt, 
						// verwende ansonsten die letzte guelte Bezeichnung
						// --> Manche M5-Files enthalten keine Pkt fuer Wechselpunkte
						if (pointName.Trim().Length == 0 && this.levelingData != null && !this.levelingData.hasFirstForeSightReading())
						{
							DateTime localDateTime = LocalDateTime.now(ZoneId.of("UTC"));
							int doy = localDateTime.DayOfYear;
							int musec = localDateTime.getNano() / 1000;
							int sec = localDateTime.Second;
							int min = localDateTime.Minute;
							int hour = localDateTime.Hour;
							int musod = (hour * 3600 + min * 60 + sec) * 1000000 + musec;
							//pointName = "W"+(++cnt)+"_"+this.getFile().getName().substring(0, this.getFile().getName().lastIndexOf('.'));
							pointName = String.format(Locale.ENGLISH, "%c%04d%03d%011d", 'W', ++this.cnt, doy, musod);
							this.lastPointName = pointName;
							code = "JAG3D";
						}
						double d = 0;
						double v = double.Parse(value3.Trim());
						v = unit3.ToLower().Trim().Equals("ft") ? 0.3048 * v : v;

						// Pruefe, ob es eine Strecke gibt
						if (T4.Equals("E") || T4.Equals("HD"))
						{
							d = double.Parse(value4.Trim());
							d = unit4.ToLower().Trim().Equals("ft") ? 0.3048 * d : d;
						}

						if (this.levelingData != null)
						{
							this.levelingData.addForeSightReading(pointName, v, d);
						}
					}
					// Zwischenblick
					else if (T3.Equals("Lz") || T3.Equals("Rz"))
					{
						double d = 0;
						double s = double.Parse(value3.Trim());
						s = unit3.ToLower().Trim().Equals("ft") ? 0.3048 * s : s;

						// Pruefe, ob es eine Strecke gibt
						if (T4.Equals("E") || T4.Equals("HD"))
						{
							d = double.Parse(value4.Trim());
							d = unit4.ToLower().Trim().Equals("ft") ? 0.3048 * d : d;
						}

						// Zwischenblicke werden direkt ausgewertet, sodass Z direkt verfuegbar ist
						if (T5.Equals("Z") && !this.pointNames.Contains(pointName))
						{
							double z = double.Parse(value5.Trim());
							z = unit5.ToLower().Trim().Equals("ft") ? 0.3048 * z : z;
							PointRow point = new PointRow();
							point.Name = pointName;
							point.Code = code;
							point.ZApriori = z;
							this.points1d.Add(point);
							this.pointNames.Add(pointName);
						}

						if (this.lastRb4Zb != null)
						{
							LevelingData sideLevelingData = new LevelingData();
							sideLevelingData.addBackSightReading(this.startPointName, this.lastRb4Zb.Value, this.distLastRb4Zb == null ? 0 : this.distLastRb4Zb.Value);
							sideLevelingData.addForeSightReading(pointName, s, d);
							if (sideLevelingData != null)
							{
								this.addLevelingData(sideLevelingData);
							}
						}
					}
					else if (T5.Equals("Z"))
					{
						// Pruefe, ob der Punkt eine Pkt-Id besitzt, 
						// verwende ansonsten die letzte guelte Bezeichnung
						// --> Manche M5-Files enthalten keine Pkt fuer Wechselpunkte
						if (pointName.Trim().Length == 0)
						{
							pointName = this.lastPointName;
							code = "JAG3D";
						}
						if (!this.pointNames.Contains(pointName))
						{
							double z = double.Parse(value5.Trim());
							z = unit5.ToLower().Trim().Equals("ft") ? 0.3048 * z : z;
							PointRow point = new PointRow();
							point.Name = pointName;
							point.Code = code;
							point.ZApriori = z;
							this.points1d.Add(point);
							this.pointNames.Add(pointName);
						}

						if (this.levelingData != null)
						{
							this.addLevelingData(this.levelingData);
						}

						this.levelingData = null;
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveLevelingGroup() throws java.sql.SQLException
		private void saveLevelingGroup()
		{
			if (this.leveling.Count > 0)
			{
				string prefix = this.loopId + " ";
				string itemName = this.createItemName(prefix, null);
				this.lastTreeItem = this.saveTerrestrialObservations(itemName, TreeItemType.LEVELING_LEAF, this.leveling);
				this.leveling.Clear();
				this.cnt = 0;
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
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("M5FileReader.extension.m5", "M5 (DiNi)"), "*.m5", "*.rec", "*.dat", "*.din", "*.M5", "*.REC", "*.DAT", "*.DIN")};
			}
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
					//SQLManager.getInstance().saveItem(groupId, row);
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