using System;

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

namespace org.applied_geodesy.jag3d.ui.dnd
{
	using CongruenceAnalysisTreeItemValue = org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using PointTreeItemValue = org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using VerticalDeflectionTreeItemValue = org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue;

	using TreeItem = javafx.scene.control.TreeItem;

	[Serializable]
	public class GroupTreeItemDnD : DnD
	{
		private const long serialVersionUID = 8338527371608133004L;

		private int dimension, itemIndex;
		private TreeItemType itemType, parentType;

		internal virtual TreeItemType ItemType
		{
			set
			{
				this.itemType = value;
			}
			get
			{
				return this.itemType;
			}
		}
		internal virtual TreeItemType ParentType
		{
			set
			{
				this.parentType = value;
			}
			get
			{
				return this.parentType;
			}
		}
		internal virtual int Dimension
		{
			set
			{
				this.dimension = value;
			}
			get
			{
				return this.dimension;
			}
		}
		internal virtual int Index
		{
			set
			{
				this.itemIndex = value;
			}
			get
			{
				return this.itemIndex;
			}
		}

		public static GroupTreeItemDnD fromTreeItem(TreeItem<TreeItemValue> treeItem)
		{
			TreeItemValue itemValue = treeItem.getValue();
			if (treeItem.getParent() == null || treeItem.getParent().getValue() == null || itemValue == null)
			{
				return null;
			}

			TreeItem<TreeItemValue> parent = treeItem.getParent();
			TreeItemType itemType = itemValue.ItemType;
			TreeItemType parentType = parent.getValue().getItemType();
			int groupId = -1, dimension = -1;

			if (itemType == null || parentType == null)
			{
				return null;
			}

			if (TreeItemType.isObservationTypeLeaf(itemType))
			{
				groupId = ((ObservationTreeItemValue)itemValue).GroupId;
				dimension = ((ObservationTreeItemValue)itemValue).Dimension;
			}
			else if (TreeItemType.isGNSSObservationTypeLeaf(itemType))
			{
				groupId = ((ObservationTreeItemValue)itemValue).GroupId;
				dimension = ((ObservationTreeItemValue)itemValue).Dimension;
			}
			else if (TreeItemType.isPointTypeLeaf(itemType))
			{
				groupId = ((PointTreeItemValue)itemValue).GroupId;
				dimension = ((PointTreeItemValue)itemValue).Dimension;
			}
			else if (TreeItemType.isCongruenceAnalysisTypeLeaf(itemType))
			{
				groupId = ((CongruenceAnalysisTreeItemValue)itemValue).GroupId;
				dimension = ((CongruenceAnalysisTreeItemValue)itemValue).Dimension;
			}
			else if (TreeItemType.isVerticalDeflectionTypeLeaf(itemType))
			{
				groupId = ((VerticalDeflectionTreeItemValue)itemValue).GroupId;
				dimension = ((VerticalDeflectionTreeItemValue)itemValue).Dimension;
			}
			else
			{
				return null;
			}

			int itemIndex = treeItem.getParent().getChildren().IndexOf(treeItem);
			if (groupId >= 0 && dimension >= 0 && itemIndex >= 0)
			{
				GroupTreeItemDnD itemDnD = new GroupTreeItemDnD();
				itemDnD.GroupId = groupId;
				itemDnD.ParentType = parentType;
				itemDnD.ItemType = itemType;
				itemDnD.Dimension = dimension;
				itemDnD.Index = itemIndex;
				return itemDnD;
			}
			return null;
		}
	}
}