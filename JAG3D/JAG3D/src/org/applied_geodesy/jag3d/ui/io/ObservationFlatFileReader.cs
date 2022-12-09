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

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using GNSSObservationRow = org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow;
	using ObservationRow = org.applied_geodesy.jag3d.ui.table.row.ObservationRow;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;

	using TreeItem = javafx.scene.control.TreeItem;

	public class ObservationFlatFileReader : FlatFileReader<TreeItem<TreeItemValue>>
	{
		private readonly ObservationType observationType;
		private readonly TreeItemType treeItemType;

		private bool separateGroup = true;

		private IList<TerrestrialObservationRow> observations = null;
		private IList<GNSSObservationRow> gnss = null;

		private bool isGroupWithEqualStation = true;
		private string startPointName = null;

		public ObservationFlatFileReader(ObservationType observationType)
		{
			this.observationType = observationType;
			this.separateGroup = ImportOption.Instance.isGroupSeparation(observationType);
			this.treeItemType = TreeItemType.getTreeItemTypeByObservationType(observationType);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, observation type could not be transformed to tree item type. " + observationType);
			}
			this.reset();
		}

		public ObservationFlatFileReader(string fileName, ObservationType observationType) : this((new File(fileName)).toPath(), observationType)
		{
		}

		public ObservationFlatFileReader(File sf, ObservationType observationType) : this(sf.toPath(), observationType)
		{
		}

		public ObservationFlatFileReader(Path path, ObservationType observationType) : base(path)
		{
			this.observationType = observationType;
			this.treeItemType = TreeItemType.getTreeItemTypeByObservationType(observationType);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, observation type could not be transformed to tree item type. " + observationType);
			}
			this.reset();
		}

		public override void reset()
		{
			if (this.observations == null)
			{
				this.observations = new List<TerrestrialObservationRow>();
			}
			if (this.gnss == null)
			{
				this.gnss = new List<GNSSObservationRow>();
			}

			this.observations.Clear();
			this.gnss.Clear();

			this.isGroupWithEqualStation = true;
			this.startPointName = null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			this.reset();
			this.ignoreLinesWhichStartWith("#");
			TreeItem<TreeItemValue> newTreeItem = null;

			base.read();

			if (this.observations.Count > 0 || this.gnss.Count > 0)
			{
				if (this.observations.Count > 0)
				{
					newTreeItem = this.saveGroup(this.treeItemType, this.observations);
				}
				else if (this.gnss.Count > 0)
				{
					newTreeItem = this.saveGroup(this.treeItemType, this.gnss);
				}
			}
			this.reset();
			return newTreeItem;
		}

		public override void parse(string line)
		{
			line = line.Trim();

			TerrestrialObservationRow terrestrialObservationRow = null;
			GNSSObservationRow gnssObservationRow = null;

			try
			{
				switch (this.observationType.innerEnumValue)
				{
				case ObservationType.InnerEnum.GNSS1D:
					gnssObservationRow = GNSSObservationRow.scan(line, 1);
					break;

				case ObservationType.InnerEnum.GNSS2D:
					gnssObservationRow = GNSSObservationRow.scan(line, 2);
					break;

				case ObservationType.InnerEnum.GNSS3D:
					gnssObservationRow = GNSSObservationRow.scan(line, 3);
					break;

				case ObservationType.InnerEnum.LEVELING:
				case ObservationType.InnerEnum.DIRECTION:
				case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				case ObservationType.InnerEnum.ZENITH_ANGLE:
				case ObservationType.InnerEnum.SLOPE_DISTANCE:
					terrestrialObservationRow = TerrestrialObservationRow.scan(line, this.observationType);
					break;
				}

				string startPointName = null;
				if (terrestrialObservationRow != null)
				{
					startPointName = terrestrialObservationRow.StartPointName;
				}
				else if (gnssObservationRow != null)
				{
					startPointName = gnssObservationRow.StartPointName;
				}

				if (!string.ReferenceEquals(startPointName, null))
				{
					if (string.ReferenceEquals(this.startPointName, null))
					{
						this.startPointName = startPointName;
					}
					this.isGroupWithEqualStation = this.isGroupWithEqualStation && this.startPointName.Equals(startPointName);

					if (this.separateGroup && !this.startPointName.Equals(startPointName))
					{
						this.isGroupWithEqualStation = true;
						this.saveGroup(this.treeItemType, this.observations);
						this.startPointName = startPointName;
					}

					if (terrestrialObservationRow != null)
					{
						this.observations.Add(terrestrialObservationRow);
					}
					else if (gnssObservationRow != null)
					{
						this.gnss.Add(gnssObservationRow);
					}
				}
			}
			catch (Exception)
			{
				return;
				// err.printStackTrace();
				// nichts, Beobachtung unbrauchbar...
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveGroup(org.applied_geodesy.jag3d.ui.tree.TreeItemType itemType, java.util.List<? extends org.applied_geodesy.jag3d.ui.table.row.ObservationRow> observations) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveGroup<T1>(TreeItemType itemType, IList<T1> observations) where T1 : org.applied_geodesy.jag3d.ui.table.row.ObservationRow
		{
			TreeItem<TreeItemValue> treeItem = null;
			if (observations.Count > 0)
			{
				string itemName = this.createItemName(null, this.isGroupWithEqualStation ? " (" + this.startPointName + ")" : null);
				treeItem = this.saveObservations(itemName, itemType);
			}
			observations.Clear();
			this.startPointName = null;
			this.isGroupWithEqualStation = true;
			return treeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveObservations(String itemName, org.applied_geodesy.jag3d.ui.tree.TreeItemType treeItemType) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveObservations(string itemName, TreeItemType treeItemType)
		{
			if ((this.observations == null || this.observations.Count == 0) && (this.gnss == null || this.gnss.Count == 0))
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
				if (this.observations.Count > 0)
				{
					foreach (TerrestrialObservationRow row in this.observations)
					{
						row.GroupId = groupId;
						SQLManager.Instance.saveItem(row);
					}
				}
				else if (this.gnss.Count > 0)
				{
					foreach (GNSSObservationRow row in this.gnss)
					{
						row.GroupId = groupId;
						SQLManager.Instance.saveItem(row);
					}
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