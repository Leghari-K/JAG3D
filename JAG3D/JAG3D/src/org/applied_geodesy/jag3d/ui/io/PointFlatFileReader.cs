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

	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using PointRow = org.applied_geodesy.jag3d.ui.table.row.PointRow;
	using PointTreeItemValue = org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;

	using TreeItem = javafx.scene.control.TreeItem;

	public class PointFlatFileReader : FlatFileReader<TreeItem<TreeItemValue>>
	{
		private readonly int dimension;
		private ISet<string> reservedNames = null;
		private readonly TreeItemType treeItemType;

		private IList<PointRow> points = null;

		public PointFlatFileReader(PointType pointType, int dimension)
		{
			this.dimension = dimension;
			this.treeItemType = TreeItemType.getTreeItemTypeByPointType(pointType, dimension);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, point type could not be transformed to tree item type. " + pointType);
			}
			this.reset();
		}

		public PointFlatFileReader(string fileName, PointType pointType, int dimension) : this((new File(fileName)).toPath(), pointType, dimension)
		{
		}

		public PointFlatFileReader(File sf, PointType pointType, int dimension) : this(sf.toPath(), pointType, dimension)
		{
		}

		public PointFlatFileReader(Path path, PointType pointType, int dimension) : base(path)
		{
			this.dimension = dimension;
			this.treeItemType = TreeItemType.getTreeItemTypeByPointType(pointType, dimension);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, point type could not be transformed to tree item type. " + pointType);
			}
			this.reset();
		}

		public override void reset()
		{
			if (this.points == null)
			{
				this.points = new List<PointRow>();
			}
			if (this.reservedNames == null)
			{
				this.reservedNames = new HashSet<string>();
			}

			this.points.Clear();
			this.reservedNames.Clear();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			this.reset();
			this.ignoreLinesWhichStartWith("#");
			this.reservedNames = SQLManager.Instance.FullPointNameSet;
			TreeItem<TreeItemValue> newTreeItem = null;

			base.read();

			if (this.points.Count > 0)
			{
				string itemName = this.createItemName(null, null);
				TreeItemType parentType = TreeItemType.getDirectoryByLeafType(this.treeItemType);
				newTreeItem = UITreeBuilder.Instance.addItem(parentType, -1, itemName, true, false);
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
					if (this.points.Count > 0)
					{
						foreach (PointRow row in this.points)
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
			}

			this.reset();
			return newTreeItem;
		}

		public override void parse(string line)
		{
			line = line.Trim();

			PointRow pointRow = null;
			try
			{
				pointRow = PointRow.scan(line, this.dimension);
				if (pointRow != null && !this.reservedNames.Contains(pointRow.Name))
				{
					this.points.Add(pointRow);
					this.reservedNames.Add(pointRow.Name);
				}
			}
			catch (Exception)
			{
				return;
				// err.printStackTrace();
				// nichts, Beobachtung unbrauchbar...
			}
		}
	}

}