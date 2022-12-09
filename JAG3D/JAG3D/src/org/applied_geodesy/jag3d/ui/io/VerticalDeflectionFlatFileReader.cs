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

	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using VerticalDeflectionRow = org.applied_geodesy.jag3d.ui.table.row.VerticalDeflectionRow;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using VerticalDeflectionTreeItemValue = org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue;

	using TreeItem = javafx.scene.control.TreeItem;

	public class VerticalDeflectionFlatFileReader : FlatFileReader<TreeItem<TreeItemValue>>
	{
		private ISet<string> reservedNames = null;
		private readonly TreeItemType treeItemType;

		private IList<VerticalDeflectionRow> verticalDeflections = null;

		public VerticalDeflectionFlatFileReader(VerticalDeflectionType verticalDeflectionType)
		{
			this.treeItemType = TreeItemType.getTreeItemTypeByVerticalDeflectionType(verticalDeflectionType);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, point type could not be transformed to tree item type. " + verticalDeflectionType);
			}
			this.reset();
		}

		public VerticalDeflectionFlatFileReader(string fileName, VerticalDeflectionType verticalDeflectionType) : this((new File(fileName)).toPath(), verticalDeflectionType)
		{
		}

		public VerticalDeflectionFlatFileReader(File sf, VerticalDeflectionType verticalDeflectionType) : this(sf.toPath(), verticalDeflectionType)
		{
		}

		public VerticalDeflectionFlatFileReader(Path path, VerticalDeflectionType verticalDeflectionType) : base(path)
		{
			this.treeItemType = TreeItemType.getTreeItemTypeByVerticalDeflectionType(verticalDeflectionType);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, point type could not be transformed to tree item type. " + verticalDeflectionType);
			}
			this.reset();
		}

		public override void reset()
		{
			if (this.verticalDeflections == null)
			{
				this.verticalDeflections = new List<VerticalDeflectionRow>();
			}
			if (this.reservedNames == null)
			{
				this.reservedNames = new HashSet<string>();
			}

			this.verticalDeflections.Clear();
			this.reservedNames.Clear();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			this.reset();
			this.ignoreLinesWhichStartWith("#");
			this.reservedNames = SQLManager.Instance.FullVerticalDeflectionNameSet;
			TreeItem<TreeItemValue> newTreeItem = null;

			base.read();

			if (this.verticalDeflections.Count > 0)
			{
				string itemName = this.createItemName(null, null);
				TreeItemType parentType = TreeItemType.getDirectoryByLeafType(this.treeItemType);
				newTreeItem = UITreeBuilder.Instance.addItem(parentType, -1, itemName, true, false);
				try
				{
					SQLManager.Instance.saveGroup((VerticalDeflectionTreeItemValue)newTreeItem.getValue());
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
					int groupId = ((VerticalDeflectionTreeItemValue)newTreeItem.getValue()).GroupId;
					if (this.verticalDeflections.Count > 0)
					{
						foreach (VerticalDeflectionRow row in this.verticalDeflections)
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

			VerticalDeflectionRow verticalDeflectionRow = null;
			try
			{
				verticalDeflectionRow = VerticalDeflectionRow.scan(line);
				if (verticalDeflectionRow != null && !this.reservedNames.Contains(verticalDeflectionRow.Name))
				{
					this.verticalDeflections.Add(verticalDeflectionRow);
					this.reservedNames.Add(verticalDeflectionRow.Name);
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