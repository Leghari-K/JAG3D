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

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using GNSSObservationRow = org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow;
	using ObservationRow = org.applied_geodesy.jag3d.ui.table.row.ObservationRow;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using org.applied_geodesy.util.io;
	using CSVColumnType = org.applied_geodesy.util.io.csv.CSVColumnType;
	using ColumnRange = org.applied_geodesy.util.io.csv.ColumnRange;

	using TreeItem = javafx.scene.control.TreeItem;

	public class ColumnDefinedObservationFileReader : SourceFileReader<TreeItem<TreeItemValue>>
	{
		private readonly string tabulator;
		private readonly ObservationType observationType;
		private readonly TreeItemType treeItemType;
		private FormatterOptions options = FormatterOptions.Instance;
		private NumberFormat numberFormat = NumberFormat.getNumberInstance(Locale.ENGLISH);

		private int lastCharacterPosition = 0;
		private IList<ColumnRange> columnRanges = new List<ColumnRange>(0);
		private IList<TerrestrialObservationRow> observations = null;
		private IList<GNSSObservationRow> gnss = null;

		private bool separateGroup = true;
		private bool isGroupWithEqualStation = true;
		private string startPointName = null;

		public ColumnDefinedObservationFileReader(ObservationType observationType, string tabulator)
		{
			this.observationType = observationType;
			this.separateGroup = ImportOption.Instance.isGroupSeparation(observationType);
			this.tabulator = tabulator;
			this.treeItemType = TreeItemType.getTreeItemTypeByObservationType(observationType);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, observation type could not be transformed to tree item type. " + observationType);
			}
			this.reset();
		}

		public ColumnDefinedObservationFileReader(string fileName, ObservationType observationType, string tabulator) : this((new File(fileName)).toPath(), observationType, tabulator)
		{
		}

		public ColumnDefinedObservationFileReader(File sf, ObservationType observationType, string tabulator) : this(sf.toPath(), observationType, tabulator)
		{
		}

		public ColumnDefinedObservationFileReader(Path path, ObservationType observationType, string tabulator) : base(path)
		{
			this.observationType = observationType;
			this.tabulator = tabulator;
			this.treeItemType = TreeItemType.getTreeItemTypeByObservationType(observationType);
			if (this.treeItemType == null)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, observation type could not be transformed to tree item type. " + observationType);
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

			this.lastCharacterPosition = 0;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			this.reset();
			this.ignoreLinesWhichStartWith("#");

			foreach (ColumnRange range in this.columnRanges)
			{
				this.lastCharacterPosition = Math.Max(this.lastCharacterPosition, range.ColumnEnd + 1);
			}

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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void parse(String line) throws java.sql.SQLException
		public override void parse(string line)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			line = string.Format("%" + this.lastCharacterPosition + "s", line.replaceAll("\t", this.tabulator));

			if (this.GNSS)
			{
				this.parseGNSSObservation(line);
			}
			else
			{
				this.parseTerrestrialObservation(line);
			}
		}

		private bool GNSS
		{
			get
			{
				switch (this.observationType.innerEnumValue)
				{
				case ObservationType.InnerEnum.GNSS1D:
				case ObservationType.InnerEnum.GNSS2D:
				case ObservationType.InnerEnum.GNSS3D:
					return true;
				default:
					return false;
				}
			}
		}

		private void parseTerrestrialObservation(string line)
		{
			TerrestrialObservationRow row = new TerrestrialObservationRow();

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
					case CSVColumnType.STATION:
						string startPointName = line.Substring(startPos, endPos - startPos).Trim();
						if (!string.ReferenceEquals(startPointName, null) && startPointName.Length > 0)
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
							row.StartPointName = startPointName;
						}
						else
						{
							continue;
						}
						break;
					case CSVColumnType.TARGET:
						string endPointName = line.Substring(startPos, endPos - startPos).Trim();
						if (!string.ReferenceEquals(endPointName, null) && endPointName.Length > 0)
						{
							row.EndPointName = endPointName;
						}
						else
						{
							continue;
						}
						break;
					case CSVColumnType.INSTRUMENT_HEIGHT:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						row.InstrumentHeight = options.convertLengthToModel(value);
						break;
					case CSVColumnType.TARGET_HEIGHT:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						row.ReflectorHeight = options.convertLengthToModel(value);
						break;
					case CSVColumnType.VALUE:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						switch (this.observationType.innerEnumValue)
						{
						case ObservationType.InnerEnum.DIRECTION:
						case ObservationType.InnerEnum.ZENITH_ANGLE:
							row.ValueApriori = options.convertAngleToModel(value);
							break;
						default:
							row.ValueApriori = options.convertLengthToModel(value);
							if (this.observationType == ObservationType.HORIZONTAL_DISTANCE || this.observationType == ObservationType.SLOPE_DISTANCE)
							{
								row.DistanceApriori = options.convertLengthToModel(value);
							}
							break;
						}
						break;
					case CSVColumnType.UNCERTAINTY:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						switch (this.observationType.innerEnumValue)
						{
						case ObservationType.InnerEnum.DIRECTION:
						case ObservationType.InnerEnum.ZENITH_ANGLE:
							row.SigmaApriori = options.convertAngleToModel(value);
							break;
						default:
							row.SigmaApriori = options.convertLengthToModel(value);
							break;
						}
						break;
					case CSVColumnType.DISTANCE_FOR_UNCERTAINTY:
						value = this.numberFormat.parse(line.Substring(startPos, endPos - startPos).Trim()).doubleValue();
						row.DistanceApriori = options.convertLengthToModel(value);
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

			if (!string.ReferenceEquals(row.StartPointName, null) && !string.ReferenceEquals(row.EndPointName, null) && row.ValueApriori != null && row.StartPointName.Length > 0 && row.EndPointName.Length > 0 && !row.StartPointName.Equals(row.EndPointName))
			{
				this.observations.Add(row);
			}
		}

		private void parseGNSSObservation(string line)
		{
			GNSSObservationRow row = new GNSSObservationRow();

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
					case CSVColumnType.STATION:
						string startPointName = line.Substring(startPos, endPos - startPos).Trim();
						if (!string.ReferenceEquals(startPointName, null) && startPointName.Length > 0)
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
							row.StartPointName = startPointName;
						}
						else
						{
							continue;
						}
						break;
					case CSVColumnType.TARGET:
						string endPointName = line.Substring(startPos, endPos - startPos).Trim();
						if (!string.ReferenceEquals(endPointName, null) && endPointName.Length > 0)
						{
							row.EndPointName = endPointName;
						}
						else
						{
							continue;
						}
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

			if (!string.ReferenceEquals(row.StartPointName, null) && !string.ReferenceEquals(row.EndPointName, null) && row.StartPointName.Length > 0 && row.EndPointName.Length > 0 && !row.StartPointName.Equals(row.EndPointName) && ((this.observationType == ObservationType.GNSS3D && row.ZApriori != null && row.XApriori != null && row.YApriori != null) || (this.observationType == ObservationType.GNSS2D && row.XApriori != null && row.YApriori != null) || (this.observationType == ObservationType.GNSS1D && row.ZApriori != null)))
			{
				this.gnss.Add(row);
			}

		}

	}

}