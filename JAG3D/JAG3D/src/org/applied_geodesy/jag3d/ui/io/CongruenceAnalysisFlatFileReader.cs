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
	using CongruenceAnalysisRow = org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow;
	using CongruenceAnalysisTreeItemValue = org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;

	using TreeItem = javafx.scene.control.TreeItem;

	public class CongruenceAnalysisFlatFileReader : FlatFileReader<TreeItem<TreeItemValue>>
	{
		private class CongruenceAnalysisPointPair
		{
			private readonly CongruenceAnalysisFlatFileReader outerInstance;

			internal readonly string name1, name2;
			internal CongruenceAnalysisPointPair(CongruenceAnalysisFlatFileReader outerInstance, string name1, string name2)
			{
				this.outerInstance = outerInstance;
				this.name1 = name1;
				this.name2 = name2;
			}
			public override int GetHashCode()
			{
				return name1.GetHashCode() + name2.GetHashCode();
			}
			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}

				if (obj == null)
				{
					return false;
				}

				if (!(obj is CongruenceAnalysisPointPair))
				{
					return false;
				}

				CongruenceAnalysisPointPair other = (CongruenceAnalysisPointPair) obj;

				if (this.name1.Equals(other.name1) && this.name2.Equals(other.name2) || this.name1.Equals(other.name2) && this.name2.Equals(other.name1))
				{
					return true;
				}

				return false;
			}
		}

		private ISet<CongruenceAnalysisPointPair> reservedNamePairs = null;
		private readonly TreeItemType treeItemType;
		private IList<CongruenceAnalysisRow> congruenceAnalysisPairs = null;

		public CongruenceAnalysisFlatFileReader(int dimension)
		{
			this.treeItemType = TreeItemType.getTreeItemTypeByCongruenceAnalysisDimension(dimension);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, dimension could not be transformed to tree item type. " + dimension);
			}

			this.reset();
		}

		public CongruenceAnalysisFlatFileReader(string fileName, int dimension) : this((new File(fileName)).toPath(), dimension)
		{
		}

		public CongruenceAnalysisFlatFileReader(File sf, int dimension) : this(sf.toPath(), dimension)
		{
		}

		public CongruenceAnalysisFlatFileReader(Path path, int dimension) : base(path)
		{
			this.treeItemType = TreeItemType.getTreeItemTypeByCongruenceAnalysisDimension(dimension);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, dimension could not be transformed to tree item type. " + dimension);
			}

			this.reset();
		}

		public override void reset()
		{
			if (this.congruenceAnalysisPairs == null)
			{
				this.congruenceAnalysisPairs = new List<CongruenceAnalysisRow>();
			}
			if (this.reservedNamePairs == null)
			{
				this.reservedNamePairs = new HashSet<CongruenceAnalysisPointPair>();
			}

			this.congruenceAnalysisPairs.Clear();
			this.reservedNamePairs.Clear();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			this.reset();
			this.ignoreLinesWhichStartWith("#");
			TreeItem<TreeItemValue> newTreeItem = null;

			base.read();

			if (this.congruenceAnalysisPairs.Count > 0)
			{
				string itemName = this.createItemName(null, null);
				TreeItemType parentType = TreeItemType.getDirectoryByLeafType(this.treeItemType);
				newTreeItem = UITreeBuilder.Instance.addItem(parentType, -1, itemName, true, false);
				try
				{
					SQLManager.Instance.saveGroup((CongruenceAnalysisTreeItemValue)newTreeItem.getValue());
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
					int groupId = ((CongruenceAnalysisTreeItemValue)newTreeItem.getValue()).GroupId;
					if (this.congruenceAnalysisPairs.Count > 0)
					{
						foreach (CongruenceAnalysisRow row in this.congruenceAnalysisPairs)
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

			CongruenceAnalysisRow congruenceAnalysisRow = null;
			try
			{
				congruenceAnalysisRow = CongruenceAnalysisRow.scan(line);

				if (congruenceAnalysisRow == null)
				{
					return;
				}

				string name1 = congruenceAnalysisRow.NameInReferenceEpoch;
				string name2 = congruenceAnalysisRow.NameInControlEpoch;

				if (string.ReferenceEquals(name1, null) || string.ReferenceEquals(name2, null) || name1.Trim().Length == 0 || name2.Trim().Length == 0 || name1.Equals(name2))
				{
					return;
				}

				CongruenceAnalysisPointPair pair = new CongruenceAnalysisPointPair(this, name1, name2);

				if (!this.reservedNamePairs.Contains(pair))
				{
					this.congruenceAnalysisPairs.Add(congruenceAnalysisRow);
					this.reservedNamePairs.Add(pair);
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