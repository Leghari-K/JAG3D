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
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
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

	public class GKAFileReader : SourceFileReader<TreeItem<TreeItemValue>>
	{
		private bool isNewStation = false;
		private string startPointName = null, lastStartPointName = null;
		private double ih = 0.0;
		private double? edmRefractiveIndex = null, edmCarrierWavelength = null;
		private TreeItem<TreeItemValue> lastTreeItem = null;

		private IList<TerrestrialObservationRow> horizontalDistances = null;
		private IList<TerrestrialObservationRow> directions = null;

		private IList<TerrestrialObservationRow> slopeDistances = null;
		private IList<TerrestrialObservationRow> zenithAngles = null;

		public GKAFileReader()
		{
			this.reset();
		}

		public GKAFileReader(File f) : this(f.toPath())
		{
		}

		public GKAFileReader(string s) : this(new File(s))
		{
		}

		public GKAFileReader(Path p) : base(p)
		{
			this.reset();
		}

		public override void reset()
		{
			this.isNewStation = false;

			this.startPointName = null;
			this.lastStartPointName = null;

			this.ih = 0.0;

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
			this.ignoreLinesWhichStartWith(";");

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

			if (line.StartsWith("#GNV11", StringComparison.Ordinal))
			{
				this.isNewStation = false;
				this.startPointName = null;
				this.edmRefractiveIndex = null;
				this.edmCarrierWavelength = null;
				this.ih = 0.0;
			}

			// Parse observations
			if (!this.isNewStation && !string.ReferenceEquals(this.startPointName, null) && this.startPointName.Trim().Length > 0)
			{
				// P_targ, Targname, GPS_ww, GPS_d, GPS_ss, nSatz, npos, s, ss, h_s, r, sr, z, sz, h_z, sadd, refr, bre, I_extar, EX_tar, EY_tar, EZ_tar, dTrunnion, dSighting, nErrorFlag, dPreasure, dTemperature, dSignalstrength, nAutoLockMode, nPrismNr, nTargetID, bDR
				//1,2007_2,2207,4,345660.92,1,2,201.73478,0.00120,0.000,265.9469,0.00020000,297.0004,0.00020000,0.000,0.000,0.142,0.0,1,,,,-0.0304,-0.0003,0,1022.10,12.90,,3,,,0
				//2,2008_2,2207,4,345669.75,1,1,154.83761,0.00115,0.000,51.4744,0.00020000,104.4044,0.00020000,0.000,0.000,0.142,0.0,1,,,,0.0244,-0.0029,0,1022.00,12.90,,3,,,0
				string[] columns = line.Split(",", true);
				if (columns.Length < 15)
				{
					return;
				}

				string endPointName = columns[1].Trim();

				bool isFaceII = columns[6].Equals("2", StringComparison.OrdinalIgnoreCase);
				double th = 0.0;

				// Target height for distance measurement
				try
				{
					th = double.Parse(columns[9].Trim());
				}
				catch (System.FormatException)
				{
					th = 0.0;
				};

				double prismConst = 0;
				double scale = 1.0;
				if (columns.Length > 15)
				{
					try
					{
					prismConst = double.Parse(columns[15].Trim());
					}
				catch (System.FormatException)
				{
					prismConst = 0.0;
				};
				}

				if (columns.Length > 27)
				{
					double pressure = 99.99; // 99.99 == Not set
					double temperature = 99.99;

					try
					{
						pressure = double.Parse(columns[25].Trim());
					}
					catch (System.FormatException)
					{
						pressure = 99.99;
					};
					try
					{
						temperature = double.Parse(columns[26].Trim());
					}
					catch (System.FormatException)
					{
						temperature = 99.99;
					};

					scale = this.getFirstVelocityCorrection(temperature, pressure);
					scale = scale > 0 ? scale : 1.0;
				}

				double distance = 0;
				try
				{
					TerrestrialObservationRow obs = new TerrestrialObservationRow();
					obs.StartPointName = this.startPointName;
					obs.EndPointName = endPointName;
					obs.InstrumentHeight = this.ih;
					obs.ReflectorHeight = th;
					distance = double.Parse(columns[7].Trim());
					if (distance > 0 && (distance + prismConst) > 0)
					{
						distance = scale * distance + prismConst;
						obs.ValueApriori = distance;
						obs.DistanceApriori = distance;
						this.slopeDistances.Add(obs);
					}
				}
				catch (System.FormatException)
				{
				};

				// Target height for angle measurement
				try
				{
					th = double.Parse(columns[14].Trim());
				}
				catch (System.FormatException)
				{
					th = 0.0;
				};

				try
				{
					TerrestrialObservationRow obs = new TerrestrialObservationRow();
					obs.StartPointName = this.startPointName;
					obs.EndPointName = endPointName;
					obs.InstrumentHeight = this.ih;
					obs.ReflectorHeight = th;
					double zenithAngle = double.Parse(columns[12].Trim()) * Constant.RHO_GRAD2RAD;
					isFaceII = isFaceII || zenithAngle > Math.PI;
					if (isFaceII)
					{
						zenithAngle = MathExtension.MOD(2.0 * Math.PI - zenithAngle, 2.0 * Math.PI);
					}
					obs.ValueApriori = zenithAngle;
					if (distance > 0)
					{
						obs.DistanceApriori = distance;
					}
					this.zenithAngles.Add(obs);
				}
				catch (System.FormatException)
				{
				};

				try
				{
					TerrestrialObservationRow obs = new TerrestrialObservationRow();
					obs.StartPointName = this.startPointName;
					obs.EndPointName = endPointName;
					obs.InstrumentHeight = this.ih;
					obs.ReflectorHeight = th;
					double direction = double.Parse(columns[10].Trim()) * Constant.RHO_GRAD2RAD;
					if (isFaceII)
					{
						direction = MathExtension.MOD(direction + Math.PI, 2.0 * Math.PI);
					}
					obs.ValueApriori = direction;
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

			// Parse station
			if (this.isNewStation)
			{
				// P_tach, Tachname, orient, nsatzr, nsatzz, h_tach, I_extach, EX_tach, EY_tach, EZ_tach, ori, J,N,	nWeatherFlag, nBattery
				// 38140004,Ost,0,0,0,0.00000000,1,,,,0.0,278.77885605,80.65533842,1,100
				string[] columns = line.Split(",", true);
				if (columns.Length < 13)
				{
					return;
				}
				this.startPointName = columns[1].Trim();
				if (string.ReferenceEquals(this.lastStartPointName, null))
				{
					this.lastStartPointName = this.startPointName;
				}
				try
				{
					this.ih = double.Parse(columns[5].Trim());
				}
				catch (System.FormatException)
				{
					this.ih = 0.0;
				};
				// EDM Korrekturwerte fuer 1. Geschwindigkeitskorrektur
				try
				{
					this.edmRefractiveIndex = double.Parse(columns[11].Trim());
				}
				catch (System.FormatException)
				{
					this.edmRefractiveIndex = null;
				};
				try
				{
					this.edmCarrierWavelength = double.Parse(columns[12].Trim());
				}
				catch (System.FormatException)
				{
					this.edmCarrierWavelength = null;
				};

				this.isNewStation = false;
			}

			if (line.StartsWith("#GNV11", StringComparison.Ordinal))
			{
				// Speichere Daten bspw. Richtungen, da diese Satzweise zu halten sind
				this.saveObservationGroups(false);
				this.lastStartPointName = this.startPointName;
				this.isNewStation = true;
				this.startPointName = null;
				this.edmRefractiveIndex = null;
				this.edmCarrierWavelength = null;
				this.ih = 0.0;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveObservationGroups(boolean forceSaving) throws java.sql.SQLException
		private void saveObservationGroups(bool forceSaving)
		{
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
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("GKAFileReader.extension.gka", "GKA (Trimble)"), "*.gka", "*.GKA")};
			}
		}

		private double getFirstVelocityCorrection(double temperature, double pressure)
		{
			if (this.edmCarrierWavelength == null || this.edmRefractiveIndex == null || pressure == 99.99 || temperature == 99.99)
			{
				return 1.0;
			}
			// Trimble Access General Survey (2011, p. 581)
			return 1.0 + (this.edmRefractiveIndex.Value - this.edmCarrierWavelength.Value * pressure / (temperature + 273.16)) * 1.0E-6;
		}
	}

}