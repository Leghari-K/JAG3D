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

	public class DL100FileReader : SourceFileReader<TreeItem<TreeItemValue>>
	{
		private LevelingData levelingData = null;
		private ISet<string> pointNames = null;
		private IList<PointRow> points1d = null;
		private IList<TerrestrialObservationRow> leveling = null;
		private TreeItem<TreeItemValue> lastTreeItem = null;

		private DateTime loopDate = null;
		private SimpleDateFormat dateFormatIn = new SimpleDateFormat("yyMMddHHmm");
		private SimpleDateFormat dateFormatOut = new SimpleDateFormat("yyyy-MM-dd HH:mm");
		private string startPointName = null, loopId = null;
		private double? rb = null, distRb = null;

		public DL100FileReader()
		{
			this.reset();
		}

		public DL100FileReader(File f) : this(f.toPath())
		{
		}

		public DL100FileReader(string s) : this(new File(s))
		{
		}

		public DL100FileReader(Path p) : base(p)
		{
			this.reset();
		}

		public override void reset()
		{
			this.levelingData = null;
			this.loopDate = null;

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
			this.rb = null;
			this.distRb = null;
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
			try
			{
				string[] data = line.Split(",", true);

				// Zugstart/Zugende 
				if (line.ToUpper().StartsWith("B", StringComparison.Ordinal) || line.ToUpper().StartsWith("C", StringComparison.Ordinal) || line.ToUpper().StartsWith("W", StringComparison.Ordinal) || line.ToUpper().StartsWith("T", StringComparison.Ordinal) || line.ToUpper().StartsWith("Z", StringComparison.Ordinal))
				{
					bool startLevellingLoop = line.ToUpper().StartsWith("B", StringComparison.Ordinal) || line.ToUpper().StartsWith("C", StringComparison.Ordinal);
					// Zugbeginn: B = RV; C = RVVR/RRVV
					// b,28,J13,100021,+0,1410231113,,,,V,
					// c,28,J20,1,2.0,,,,,,,,,,,,,,111,+200000,1410300709,,,,S,

					if (startLevellingLoop && data.Length > 2)
					{
						this.saveLevelingGroup();
						this.loopId = data[2].Trim();

						// Bestimme Anschlusshoehe vom ersten Rueckblick
						char unit = data[1].Trim()[1];
						string pointName = null;
						double? z0 = null;
						if (line.ToUpper().StartsWith("B", StringComparison.Ordinal) && data.Length > 5)
						{
							if (data[5].Trim().Length > 0)
							{
								this.loopDate = this.dateFormatIn.parse(data[5].Trim());
							}
							if (data[3].Trim().Length > 0)
							{
								pointName = data[3].Trim();
								if (!this.pointNames.Contains(pointName) && data[4].Trim().Length > 0)
								{
									z0 = this.convertInputToTrueValue(unit, data[4].Trim());
								}
							}
						}
						else if (line.ToUpper().StartsWith("C", StringComparison.Ordinal) && data.Length > 20)
						{
							if (data[20].Trim().Length > 0)
							{
								this.loopDate = this.dateFormatIn.parse(data[20].Trim());
							}
							if (data[18].Trim().Length > 0)
							{
								pointName = data[18].Trim();
								if (!this.pointNames.Contains(pointName) && data[19].Trim().Length > 0)
								{
									z0 = this.convertInputToTrueValue(unit, data[19].Trim());
								}
							}
						}
						if (!string.ReferenceEquals(pointName, null) && z0 != null && pointName.Length > 0 && !this.pointNames.Contains(pointName))
						{
							PointRow point = new PointRow();
							point.Name = pointName;
							point.ZApriori = z0;
							this.points1d.Add(point);
							this.pointNames.Add(pointName);
						}
					}
					// Zugende - Fuege letzte Messung zur Gruppe hinzu
					this.addLevelingData(this.levelingData);

					this.levelingData = null;
					this.startPointName = null;
					this.rb = null;
					this.distRb = null;
				}

				if (!string.ReferenceEquals(this.loopId, null) && data.Length > 7)
				{
					// Rueckblick g == r1; h == r2; Vorblick i == v1; j == v2.
					// g,28,+77616,+12012,+77616,3,0,1,1,1314,D,
					// g,28,+81288,+13125,+81463,3,0,2,1,1316,H,
					if (line.ToUpper().StartsWith("G", StringComparison.Ordinal) || line.ToUpper().StartsWith("H", StringComparison.Ordinal) || line.ToUpper().StartsWith("I", StringComparison.Ordinal) || line.ToUpper().StartsWith("J", StringComparison.Ordinal) || line.ToUpper().StartsWith("K", StringComparison.Ordinal))
					{
						bool isFirstBackSightMeasurment = line.ToUpper().StartsWith("G", StringComparison.Ordinal);
						bool isBackSightMeasurment = isFirstBackSightMeasurment || line.ToUpper().StartsWith("H", StringComparison.Ordinal);

						bool isFirstForeSightMeasurment = line.ToUpper().StartsWith("I", StringComparison.Ordinal);
						//boolean isSecondForeSightMeasurment = line.toUpperCase().startsWith("J");

						bool isSideShot = line.ToUpper().StartsWith("K", StringComparison.Ordinal) && !string.ReferenceEquals(this.startPointName, null) && this.rb != null;

						if (data[1].Trim().Length < 2 || data[7].Trim().Length == 0)
						{
							return;
						}

						string pointName = data[7].Trim();
						char unit = data[1].Trim()[1];
						//String information = String.format("%8s", Integer.toBinaryString((int)data[1].trim().charAt(0))).replace(' ', '0');				
						//System.out.println(information.substring(0, 4)+"  "+information.substring(4,6)+"  "+information.substring(6,8));	

						// Messwert, Strecke, Koordinate
						double m = 0.0, d = 0.0, z = 0.0;

						if (data[2].Trim().Length == 0)
						{
							return;
						}
						m = this.convertMeasurmentToTrueValue(unit, data[2], true);

						if (data[3].Trim().Length > 0)
						{
							d = this.convertMeasurmentToTrueValue(unit, data[3], false);
						}

						if (data[4].Trim().Length > 0)
						{
							z = this.convertMeasurmentToTrueValue(unit, data[4], true);
						}

						if (isFirstBackSightMeasurment)
						{
							// Fuege zunaechst den letzten Datenblock zur Gruppe hinzu, 
							// bevor neuer Datensatz erzeugt wird.
							this.addLevelingData(this.levelingData);

							this.levelingData = new LevelingData();
							this.startPointName = pointName;
							this.rb = m;
							this.distRb = d;
						}
						// Zwischenblick
						if (isSideShot)
						{
							LevelingData sideLevelingData = new LevelingData();
							sideLevelingData.addBackSightReading(this.startPointName, this.rb.Value, this.distRb == null ? 0 : this.distRb.Value);
							sideLevelingData.addForeSightReading(pointName, m, d);

							if (!this.pointNames.Contains(pointName))
							{
								PointRow point = new PointRow();
								point.Name = pointName;
								point.ZApriori = z;
								this.points1d.Add(point);
								this.pointNames.Add(pointName);
							}
							this.addLevelingData(sideLevelingData);
						}
						// Normale Messung
						else if (this.levelingData != null)
						{
							if (isBackSightMeasurment)
							{
								this.levelingData.addBackSightReading(pointName, m, d, isFirstBackSightMeasurment);
							}
							else
							{
								this.levelingData.addForeSightReading(pointName, m, d, isFirstForeSightMeasurment);
							}

							if (!this.pointNames.Contains(pointName))
							{
								PointRow point = new PointRow();
								point.Name = pointName;
								point.ZApriori = z;
								this.points1d.Add(point);
								this.pointNames.Add(pointName);
							}
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveLevelingGroup() throws java.sql.SQLException
		private void saveLevelingGroup()
		{
			if (this.leveling.Count > 0 && !string.ReferenceEquals(this.loopId, null))
			{
				string prefix = this.loopId + " " + (this.loopDate == null ? "" : "(" + this.dateFormatOut.format(this.loopDate) + ") ");
				string itemName = this.createItemName(prefix, null);
				this.lastTreeItem = this.saveTerrestrialObservations(itemName, TreeItemType.LEVELING_LEAF, this.leveling);
				this.leveling.Clear();
			}
		}

		private void addLevelingData(LevelingData levelingData)
		{
			if (levelingData != null && !string.ReferenceEquals(levelingData.StartPointName, null) && !string.ReferenceEquals(levelingData.EndPointName, null) && !levelingData.StartPointName.Equals(levelingData.EndPointName))
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

		/// <summary>
		/// Bestimmt anhand der kodierten Einheit den wahren Wert in Meter
		/// </summary>
		/// <param name="unit"> </param>
		/// <param name="data"> </param>
		/// <param name="isStaffReading"> </param>
		/// <returns> value </returns>
		/// <exception cref="NumberFormatException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private double convertMeasurmentToTrueValue(char unit, String data, boolean isStaffReading) throws NumberFormatException
		private double convertMeasurmentToTrueValue(char unit, string data, bool isStaffReading)
		{
			double u1 = 1.0;
			double u2 = this.getUnitConversionFactor(unit);
			double d = double.Parse(data.Trim());

			if (isStaffReading)
			{ // Staff Readings
				switch (char.ToUpper(unit))
				{
					case '1': // 1
						u1 = 1.0E-3;
					break;
					case '2': // 0.1
					case '7':
						u1 = 1.0E-4;
					break;
					case '3': // 0.01
					case '4':
					case '8':
						u1 = 1.0E-5;
					break;
					case '5': // 0.001
					case '9':
						u1 = 1.0E-6;
					break;
					case '6': // 0.0001
					case 'A':
						u1 = 1.0E-7;
					break;
					default:
					throw new System.FormatException(this.GetType().Name + ": Fehler " + "beim Konvertieren des Messwertes " + data + ". Einheit " + unit + " unbekannt.");
				}
			}
			else
			{ // Distance to Staff
				switch (char.ToUpper(unit))
				{
					case '1':
					case '2':
					case '7':
						u1 = 1.0E-2;
					break;
					case '3':
					case '8':
						u1 = 1.0E-3;
					break;
					case '4':
					case '5':
					case '6':
					case '9':
					case 'A':
						u1 = 1.0E-5;
					break;
					default:
					throw new System.FormatException(this.GetType().Name + ": Fehler " + "beim Konvertieren des Messwertes " + data + ". Einheit " + unit + " unbekannt.");
				}
			}

			return u1 * u2 * d;
		}

		/// <summary>
		/// Bestimmt anhand der kodierten Einheit den wahren Wert in Meter
		/// </summary>
		/// <param name="unit"> </param>
		/// <param name="data"> </param>
		/// <param name="isStaffReading"> </param>
		/// <returns> value </returns>
		/// <exception cref="NumberFormatException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private double convertInputToTrueValue(char unit, String data) throws NumberFormatException
		private double convertInputToTrueValue(char unit, string data)
		{
			double u1 = 1.0;
			double u2 = this.getUnitConversionFactor(unit);
			double d = double.Parse(data.Trim());

			switch (char.ToUpper(unit))
			{
				case '1': // 1
				case '2':
					u1 = 1.0E-3;
				break;
				case '3': // 0.1
				case '7':
					u1 = 1.0E-4;
				break;
				case '4': // 0.01
				case '5':
				case '8':
					u1 = 1.0E-5;
				break;
				case '6': // 0.001
				case '9':
					u1 = 1.0E-6;
				break;
				case 'A': // 0.0001
					u1 = 1.0E-7;
				break;
				default:
				throw new System.FormatException(this.GetType().Name + ": Fehler " + "beim Konvertieren des Messwertes " + data + ". Einheit " + unit + " unbekannt.");
			}

			return u1 * u2 * d;
		}

		private double getUnitConversionFactor(char unit)
		{
			double u = 1.0;
			switch (char.ToUpper(unit))
			{
				case '4':
				case '5':
				case '6':
				case '9':
				case 'A':
					u = 0.3048;
				break;
			}
			return u;
		}

		public static ExtensionFilter[] ExtensionFilters
		{
			get
			{
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("DL100FileReader.extension.dl100", "DL-100 (Topcon)"), "*.l", "*.top", "*.L", "*.TOP")};
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