﻿using System;
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
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using org.applied_geodesy.util.io;
	using CSVColumnType = org.applied_geodesy.util.io.csv.CSVColumnType;
	using ColumnRange = org.applied_geodesy.util.io.csv.ColumnRange;

	using TreeItem = javafx.scene.control.TreeItem;

	public class ColumnDefinedPointFileReader : SourceFileReader<TreeItem<TreeItemValue>>
	{
		private readonly string tabulator;

		private readonly PointType pointType;
		private ISet<string> reservedNames = null;
		private TreeItemType treeItemType;
		private IList<PointRow> points = null;

		private FormatterOptions options = FormatterOptions.Instance;
		private NumberFormat numberFormat = NumberFormat.getNumberInstance(Locale.ENGLISH);

		private int lastCharacterPosition = 0;
		private IList<ColumnRange> columnRanges = new List<ColumnRange>(0);

		public ColumnDefinedPointFileReader(PointType pointType, string tabulator)
		{
			this.pointType = pointType;
			this.tabulator = tabulator;
			this.treeItemType = TreeItemType.getTreeItemTypeByPointType(pointType, 3); // dim = 3 for check
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, observation type could not be transformed to tree item type. " + pointType);
			}
			this.reset();
		}

		public ColumnDefinedPointFileReader(string fileName, PointType pointType, string tabulator) : this((new File(fileName)).toPath(), pointType, tabulator)
		{
		}

		public ColumnDefinedPointFileReader(File sf, PointType pointType, string tabulator) : this(sf.toPath(), pointType, tabulator)
		{
		}

		public ColumnDefinedPointFileReader(Path path, PointType pointType, string tabulator) : base(path)
		{
			this.pointType = pointType;
			this.tabulator = tabulator;
			this.treeItemType = TreeItemType.getTreeItemTypeByPointType(pointType, 3); // dim = 3 for check
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, observation type could not be transformed to tree item type. " + pointType);
			}
			this.reset();
		}

		public virtual IList<ColumnRange> ColumnRanges
		{
			set
			{
				this.columnRanges = value;
			}
		}

		public virtual Locale FileLocale
		{
			set
			{
				this.numberFormat = NumberFormat.getNumberInstance(value);
			}
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

			this.lastCharacterPosition = 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			this.reset();
			this.ignoreLinesWhichStartWith("#");
			this.reservedNames = SQLManager.Instance.FullPointNameSet;
			TreeItem<TreeItemValue> newTreeItem = null;

			bool hasXComponent = false;
			bool hasYComponent = false;
			bool hasZComponent = false;
			foreach (ColumnRange range in this.columnRanges)
			{
				this.lastCharacterPosition = Math.Max(this.lastCharacterPosition, range.ColumnEnd + 1);
				if (range.Type == CSVColumnType.X)
				{
					hasXComponent = true;
				}
				else if (range.Type == CSVColumnType.Y)
				{
					hasYComponent = true;
				}
				else if (range.Type == CSVColumnType.Z)
				{
					hasZComponent = true;
				}
			}

			int dimension = -1;
			if (hasXComponent && hasYComponent && hasZComponent)
			{
				dimension = 3;
			}
			else if (hasXComponent && hasYComponent)
			{
				dimension = 2;
			}
			else if (hasZComponent)
			{
				dimension = 1;
			}

			if (dimension >= 1 && dimension <= 3)
			{
				base.read();

				if (this.points.Count > 0)
				{
					this.treeItemType = TreeItemType.getTreeItemTypeByPointType(pointType, dimension);
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
						foreach (PointRow row in this.points)
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
				}
			}
			this.reset();
			return newTreeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void parse(String line) throws java.sql.SQLException
		public override void parse(string line)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			line = string.Format("%" + this.lastCharacterPosition + "s", line.replaceAll("\t", this.tabulator));

			PointRow row = new PointRow();

			foreach (ColumnRange range in this.columnRanges)
			{
				try
				{
					CSVColumnType type = range.Type;
					int startPos = range.ColumnStart;
					int endPos = range.ColumnEnd + 1;

					double value;
					switch (type)
					{
					case CSVColumnType.POINT_ID:
						string name = line.Substring(startPos, endPos - startPos).Trim();
						if (!string.ReferenceEquals(name, null) && name.Length > 0)
						{
							row.Name = name;
						}
						else
						{
							continue;
						}
						break;
					case CSVColumnType.POINT_CODE:
						row.Code = line.Substring(startPos, endPos - startPos).Trim();
						break;

					case CSVColumnType.X:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						row.XApriori = options.convertLengthToModel(value);
						break;
					case CSVColumnType.Y:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						row.YApriori = options.convertLengthToModel(value);
						break;
					case CSVColumnType.Z:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						row.ZApriori = options.convertLengthToModel(value);
						break;

					case CSVColumnType.UNCERTAINTY_X:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						row.SigmaXapriori = options.convertLengthToModel(value);
						break;
					case CSVColumnType.UNCERTAINTY_Y:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						row.SigmaYapriori = options.convertLengthToModel(value);
						break;
					case CSVColumnType.UNCERTAINTY_Z:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						row.SigmaZapriori = options.convertLengthToModel(value);
						break;

					default:
						Console.Error.WriteLine(this.GetType().Name + " Error, unsupported column type! " + type);
						break;
					}

				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					return;
				}
			}

			if (!string.ReferenceEquals(row.Name, null) && row.Name.Length > 0 && !this.reservedNames.Contains(row.Name) && (row.ZApriori != null || row.XApriori != null && row.YApriori != null))
			{
				this.points.Add(row);
				this.reservedNames.Add(row.Name);
			}
		}
	}
}