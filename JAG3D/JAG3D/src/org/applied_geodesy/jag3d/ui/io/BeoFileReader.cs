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
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using org.applied_geodesy.util.io;

	using TreeItem = javafx.scene.control.TreeItem;
	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;

	public class BeoFileReader : SourceFileReader<TreeItem<TreeItemValue>>
	{
		private bool isNewStation = false;
		private string startPointName = null, lastStartPointName = null;
		private double ih = 0.0;
		private TreeItem<TreeItemValue> lastTreeItem = null;

		private IList<TerrestrialObservationRow> leveling = null;

		private IList<TerrestrialObservationRow> horizontalDistances = null;
		private IList<TerrestrialObservationRow> directions = null;

		private IList<TerrestrialObservationRow> slopeDistances = null;
		private IList<TerrestrialObservationRow> zenithAngles = null;

		public BeoFileReader()
		{
			this.reset();
		}

		public BeoFileReader(File f) : this(f.toPath())
		{
		}

		public BeoFileReader(string s) : this(new File(s))
		{
		}

		public BeoFileReader(Path p) : base(p)
		{
			this.reset();
		}

		public override void reset()
		{
			this.isNewStation = false;

			this.startPointName = null;
			this.lastStartPointName = null;

			this.ih = 0.0;

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
			this.lastTreeItem = null;
			this.reset();
			this.ignoreLinesWhichStartWith("#");

			base.read();

			// Speichere Daten
			this.saveObservationGroups(true);

			// Clear all lists
			this.reset();

			return this.lastTreeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void parse(String line) throws java.sql.SQLException
		public override void parse(string line)
		{
			line = line.Trim();

			if (line.Length < 89)
			{
				return;
			}

			string key = line.Substring(0, 3).Trim();
			// Standpunkt
			if (key.Equals("10.", StringComparison.OrdinalIgnoreCase))
			{
				this.startPointName = line.Substring(15, 13).Trim();
				if (this.startPointName.Length == 0)
				{
					return;
				}


				this.isNewStation = !string.ReferenceEquals(this.startPointName, null);
				try
				{
					this.ih = double.Parse(line.Substring(79, line.Length - 79).Trim().replaceAll("\\s+.*", ""));
					this.ih = this.ih == -1000.0 ? 0 : this.ih;
				}
				catch (System.FormatException)
				{
				};
			}

			else if (!string.ReferenceEquals(this.startPointName, null) && this.startPointName.Length > 0 && (key.Equals("20.", StringComparison.OrdinalIgnoreCase) || key.Equals("21.", StringComparison.OrdinalIgnoreCase) || key.Equals("24.", StringComparison.OrdinalIgnoreCase) || key.Equals("31.", StringComparison.OrdinalIgnoreCase)))
			{
				if (this.isNewStation)
				{
					this.saveObservationGroups(false);
					this.lastStartPointName = null;
				}
				this.isNewStation = false;

				string endPointName = line.Substring(15, 13).Trim();
				if (endPointName.Length == 0 || this.startPointName.Equals(endPointName))
				{
					return;
				}

				double th = 0.0;
				try
				{
					th = double.Parse(line.Substring(79, line.Length - 79).Trim().replaceAll("\\s+.*", ""));
					th = th == -1000.0 ? 0 : th;
				}
				catch (System.FormatException)
				{
				};

				if (string.ReferenceEquals(this.lastStartPointName, null))
				{
					this.lastStartPointName = this.startPointName;
				}

				// Strecke3D
				string value = line.Substring(33, 16).Trim();
				double distance = 0;
				if (value.Length > 0 && !value.StartsWith("-1000.", StringComparison.Ordinal))
				{
					try
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = th;
						distance = double.Parse(value);
						if (distance > 0)
						{
							obs.ValueApriori = distance;
							obs.DistanceApriori = distance;
							this.slopeDistances.Add(obs);
						}
					}
					catch (System.FormatException)
					{
					};
				}
				// Richtung
				value = line.Substring(49, 15).Trim();
				if (value.Length > 0 && !value.StartsWith("-1000.", StringComparison.Ordinal))
				{
					try
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = th;
						double dir = double.Parse(value) * Constant.RHO_GRAD2RAD;
						obs.ValueApriori = dir;
						if (distance > 0)
						{
							obs.DistanceApriori = distance;
						}
						this.directions.Add(obs);
					}
					catch (System.FormatException)
					{
					};
				}
				// Zenitwinkel
				value = line.Substring(64, 15).Trim();
				if (value.Length > 0 && !value.StartsWith("-1000.", StringComparison.Ordinal))
				{
					try
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = th;
						double zenith = double.Parse(value) * Constant.RHO_GRAD2RAD;
						obs.ValueApriori = zenith;
						if (distance > 0)
						{
							obs.DistanceApriori = distance;
						}
						this.zenithAngles.Add(obs);
					}
					catch (System.FormatException)
					{
					};
				}
			}
			else if (!string.ReferenceEquals(this.startPointName, null) && this.startPointName.Length > 0 && key.Equals("50.", StringComparison.OrdinalIgnoreCase))
			{
				if (this.isNewStation)
				{
					this.saveObservationGroups(false);
					this.lastStartPointName = null;
				}
				this.isNewStation = false;

				string endPointName = line.Substring(15, 13).Trim();
				if (endPointName.Length == 0)
				{
					return;
				}

				double th = 0.0;
				try
				{
					th = double.Parse(line.Substring(79, line.Length - 79).Trim().replaceAll("\\s+.*", ""));
					th = th == -1000.0 ? 0 : th;
				}
				catch (System.FormatException)
				{
				};

				if (string.ReferenceEquals(this.lastStartPointName, null))
				{
					this.lastStartPointName = this.startPointName;
				}

				// Strecke2D
				double distance = 0.0; // Wird ggf. fuer delta-H mitverwendet
				string value = line.Substring(33, 16).Trim();
				if (value.Length > 0 && !value.StartsWith("-1000.", StringComparison.Ordinal))
				{
					try
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = th;
						distance = double.Parse(value);
						if (distance > 0)
						{
							obs.ValueApriori = distance;
							obs.DistanceApriori = distance;
							this.horizontalDistances.Add(obs);
						}
					}
					catch (System.FormatException)
					{
					};
				}
				// Richtung
				value = line.Substring(49, 15).Trim();
				if (value.Length > 0 && !value.StartsWith("-1000.", StringComparison.Ordinal))
				{
					try
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = th;
						double dir = double.Parse(value) * Constant.RHO_GRAD2RAD;
						obs.ValueApriori = dir;
						if (distance > 0)
						{
							obs.DistanceApriori = distance;
						}
						this.directions.Add(obs);
					}
					catch (System.FormatException)
					{
					};
				}
				// Hoehenunterschied
				value = line.Substring(64, 15).Trim();
				if (value.Length > 0 && !value.StartsWith("-1000.", StringComparison.Ordinal))
				{
					try
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = th;
						double dh = double.Parse(value);
						obs.ValueApriori = dh;
						if (distance > 0)
						{
							obs.DistanceApriori = distance;
						}
						this.leveling.Add(obs);
					}
					catch (System.FormatException)
					{
					};
				}
			}
			else if (!string.ReferenceEquals(this.startPointName, null) && this.startPointName.Length > 0 && key.Equals("70.", StringComparison.OrdinalIgnoreCase))
			{
				if (this.isNewStation)
				{
					this.saveObservationGroups(false);
					this.lastStartPointName = null;
				}
				this.isNewStation = false;

				string endPointName = line.Substring(15, 13).Trim();
				if (endPointName.Length == 0)
				{
					return;
				}

				double th = 0.0;
				try
				{
					th = double.Parse(line.Substring(79, line.Length - 79).Trim().replaceAll("\\s+.*", ""));
					th = th == -1000.0 ? 0 : th;
				}
				catch (System.FormatException)
				{
				};

				// Hoehenunterschied
				string value = line.Substring(64, 15).Trim();
				if (value.Length > 0 && !value.StartsWith("-1000.", StringComparison.Ordinal))
				{
					try
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = this.startPointName;
						obs.EndPointName = endPointName;
						obs.InstrumentHeight = this.ih;
						obs.ReflectorHeight = th;
						double dh = double.Parse(value);
						obs.ValueApriori = dh;
						try
						{
							obs.DistanceApriori = double.Parse(line.Substring(33, 16).Trim());
						}
						catch (System.FormatException)
						{
						};
						this.leveling.Add(obs);
					}
					catch (System.FormatException)
					{
					};
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveObservationGroups(boolean forceSaving) throws java.sql.SQLException
		private void saveObservationGroups(bool forceSaving)
		{
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveObservationGroup(org.applied_geodesy.jag3d.ui.tree.TreeItemType itemType, java.util.List<org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow> observations) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveObservationGroup(TreeItemType itemType, IList<TerrestrialObservationRow> observations)
		{
			TreeItem<TreeItemValue> treeItem = null;
			if (observations.Count > 0)
			{
				bool isGroupWithEqualStation = ImportOption.Instance.isGroupSeparation(TreeItemType.getObservationTypeByTreeItemType(itemType));
				string itemName = this.createItemName(null, isGroupWithEqualStation && !string.ReferenceEquals(this.lastStartPointName, null) ? " (" + this.lastStartPointName + ")" : null);
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

		public static ExtensionFilter[] ExtensionFilters
		{
			get
			{
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("BeoFileReader.extension.beo", "Beo (Neptan)"), "*.beo", "*.BEO")};
			}
		}
	}

}