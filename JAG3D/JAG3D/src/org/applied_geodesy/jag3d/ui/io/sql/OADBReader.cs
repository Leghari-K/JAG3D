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

namespace org.applied_geodesy.jag3d.ui.io.sql
{

	using ObservationGroupUncertaintyType = org.applied_geodesy.adjustment.network.ObservationGroupUncertaintyType;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using PointGroupUncertaintyType = org.applied_geodesy.adjustment.network.PointGroupUncertaintyType;
	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using VerticalDeflectionGroupUncertaintyType = org.applied_geodesy.adjustment.network.VerticalDeflectionGroupUncertaintyType;
	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using RestrictionType = org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using CongruenceAnalysisRow = org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow;
	using GNSSObservationRow = org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow;
	using ObservationRow = org.applied_geodesy.jag3d.ui.table.row.ObservationRow;
	using PointRow = org.applied_geodesy.jag3d.ui.table.row.PointRow;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using VerticalDeflectionRow = org.applied_geodesy.jag3d.ui.table.row.VerticalDeflectionRow;
	using CongruenceAnalysisTreeItemValue = org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using PointTreeItemValue = org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using VerticalDeflectionTreeItemValue = org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue;
	using org.applied_geodesy.util.io;
	using DataBase = org.applied_geodesy.util.sql.DataBase;
	using Version = org.applied_geodesy.version.jag3d.Version;
	using VersionType = org.applied_geodesy.version.VersionType;

	using TreeItem = javafx.scene.control.TreeItem;
	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;

	public class OADBReader : SourceFileReader<TreeItem<TreeItemValue>>
	{
		private ISet<string> reservedPointNames = null;
		private ISet<string> reservedVerticalDeflectionNames = null;
		private DataBase dataBase = null;

		public OADBReader()
		{
			this.reset();
		}

		public OADBReader(DataBase dataBase)
		{
			this.dataBase = dataBase;
			this.reset();
		}

		public override void reset()
		{
			if (this.dataBase != null)
			{
				this.dataBase.close();
			}

			if (this.reservedPointNames == null)
			{
				this.reservedPointNames = new HashSet<string>();
			}

			if (this.reservedVerticalDeflectionNames == null)
			{
				this.reservedVerticalDeflectionNames = new HashSet<string>();
			}

			this.reservedPointNames.Clear();
			this.reservedVerticalDeflectionNames.Clear();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> readAndImport() throws IOException, java.sql.SQLException
		public override TreeItem<TreeItemValue> readAndImport()
		{
			TreeItem<TreeItemValue> lastTreeItem = null;

			this.reset();

			try
			{
				if (SQLManager.Instance.DataBase.getURI().Equals(this.dataBase.URI))
				{
					throw new IOException(this.GetType().Name + " : Error, cannot re-import data to the same database!");
				}

				this.dataBase.open();
				if (OADBReader.isCurrentOADBVersion(this.dataBase))
				{
					this.reservedPointNames = SQLManager.Instance.FullPointNameSet;
					this.reservedVerticalDeflectionNames = SQLManager.Instance.FullVerticalDeflectionNameSet;

					TreeItem<TreeItemValue> lastPointTreeItem = this.transferPointGroups();
					TreeItem<TreeItemValue> lastObservationTreeItem = this.transferObservationGroups();
					TreeItem<TreeItemValue> lastCongruenceAnalysisTreeItem = this.transferCongruenceAnalysisGroups();
					TreeItem<TreeItemValue> lastVerticalDeflectionTreeItem = this.transferVerticalDeflectionGroups();

					if (lastPointTreeItem != null)
					{
						lastTreeItem = lastPointTreeItem;
					}
					else if (lastObservationTreeItem != null)
					{
						lastTreeItem = lastObservationTreeItem;
					}
					else if (lastCongruenceAnalysisTreeItem != null)
					{
						lastTreeItem = lastCongruenceAnalysisTreeItem;
					}
					else if (lastVerticalDeflectionTreeItem != null)
					{
						lastTreeItem = lastVerticalDeflectionTreeItem;
					}
				}
			}
			catch (SQLException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				throw new SQLException(e);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				throw new IOException(e);
			}
			finally
			{
				if (dataBase != null)
				{
					dataBase.close();
				}
			}

			// Clear all lists
			this.reset();
			return lastTreeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void parse(String line) throws java.sql.SQLException
		public override void parse(string line)
		{
			throw new System.ArgumentException(this.GetType().Name + " : Error, database content cannot be parsed!");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> transferVerticalDeflectionGroups() throws java.sql.SQLException
		private TreeItem<TreeItemValue> transferVerticalDeflectionGroups()
		{
			TreeItem<TreeItemValue> lastValidTreeItem = null;

			string sql = "SELECT " + "\"id\", \"name\", \"type\", \"enable\" " + "FROM \"VerticalDeflectionGroup\" " + "ORDER BY \"order\" ASC, \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				VerticalDeflectionType verticalDeflectionType = VerticalDeflectionType.getEnumByValue(rs.getInt("type"));
				if (verticalDeflectionType == null)
				{
					continue;
				}

				int groupId = rs.getInt("id");
				string groupName = rs.getString("name");
				bool enable = rs.getBoolean("enable");

				TreeItemType treeItemType = TreeItemType.getTreeItemTypeByVerticalDeflectionType(verticalDeflectionType);

				IList<VerticalDeflectionRow> verticalDeflections = null;

				if (TreeItemType.isVerticalDeflectionTypeLeaf(treeItemType))
				{
					verticalDeflections = this.getVerticalDeflections(groupId);
				}

				if (verticalDeflections == null)
				{
					continue;
				}

				TreeItem<TreeItemValue> treeItem = this.saveVerticalDeflections(treeItemType, groupName, enable, verticalDeflections);
				if (treeItem != null)
				{
					this.transferUncertainties(groupId, (VerticalDeflectionTreeItemValue)treeItem.getValue());

					lastValidTreeItem = treeItem;
				}
			}
			return lastValidTreeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> transferPointGroups() throws java.sql.SQLException
		private TreeItem<TreeItemValue> transferPointGroups()
		{
			TreeItem<TreeItemValue> lastValidTreeItem = null;

			string sql = "SELECT " + "\"id\", \"name\", \"type\", \"enable\", \"dimension\" " + "FROM \"PointGroup\" " + "ORDER BY \"order\" ASC, \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				PointType pointType = PointType.getEnumByValue(rs.getInt("type"));
				if (pointType == null)
				{
					continue;
				}

				int groupId = rs.getInt("id");
				int dimension = rs.getInt("dimension");
				string groupName = rs.getString("name");
				bool enable = rs.getBoolean("enable");

				TreeItemType treeItemType = TreeItemType.getTreeItemTypeByPointType(pointType, dimension);

				IList<PointRow> points = null;

				if (TreeItemType.isPointTypeLeaf(treeItemType))
				{
					points = this.getPoints(groupId);
				}

				if (points == null)
				{
					continue;
				}

				TreeItem<TreeItemValue> treeItem = this.savePoints(treeItemType, groupName, enable, points);
				if (treeItem != null)
				{
					this.transferUncertainties(groupId, (PointTreeItemValue)treeItem.getValue());

					lastValidTreeItem = treeItem;
				}
			}
			return lastValidTreeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> transferObservationGroups() throws java.sql.SQLException
		private TreeItem<TreeItemValue> transferObservationGroups()
		{
			TreeItem<TreeItemValue> lastValidTreeItem = null;

			string sql = "SELECT " + "\"id\", \"name\", \"type\", \"enable\" " + "FROM \"ObservationGroup\" " + "ORDER BY \"order\" ASC, \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				ObservationType observationType = ObservationType.getEnumByValue(rs.getInt("type"));
				if (observationType == null)
				{
					continue;
				}

				int groupId = rs.getInt("id");
				string groupName = rs.getString("name");
				bool enable = rs.getBoolean("enable");

				TreeItemType treeItemType = TreeItemType.getTreeItemTypeByObservationType(observationType);

				IList<ObservationRow> observations = null;

				if (TreeItemType.isObservationTypeLeaf(treeItemType))
				{
					observations = this.getTerrestrialObservations(groupId);
				}
				else if (TreeItemType.isGNSSObservationTypeLeaf(treeItemType))
				{
					observations = this.getGNSSObservations(groupId);
				}

				if (observations == null)
				{
					continue;
				}

				TreeItem<TreeItemValue> treeItem = this.saveObservations(treeItemType, groupName, enable, observations);
				if (treeItem != null)
				{
					this.transferUncertainties(groupId, (ObservationTreeItemValue)treeItem.getValue());
					this.transferEpoch(groupId, (ObservationTreeItemValue)treeItem.getValue());
					this.transferAdditionalParameters(groupId, (ObservationTreeItemValue)treeItem.getValue());

					lastValidTreeItem = treeItem;
				}
			}
			return lastValidTreeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> transferCongruenceAnalysisGroups() throws java.sql.SQLException
		private TreeItem<TreeItemValue> transferCongruenceAnalysisGroups()
		{
			TreeItem<TreeItemValue> lastValidTreeItem = null;

			string sql = "SELECT " + "\"id\", \"name\", \"enable\", \"dimension\" " + "FROM \"CongruenceAnalysisGroup\" " + "ORDER BY \"order\" ASC, \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				int groupId = rs.getInt("id");
				int dimension = rs.getInt("dimension");
				string groupName = rs.getString("name");
				bool enable = rs.getBoolean("enable");

				TreeItemType treeItemType = TreeItemType.getTreeItemTypeByCongruenceAnalysisDimension(dimension);

				IList<CongruenceAnalysisRow> pairs = null;

				if (TreeItemType.isCongruenceAnalysisTypeLeaf(treeItemType))
				{
					pairs = this.getCongruenceAnalysisPairs(groupId);
				}

				if (pairs == null)
				{
					continue;
				}

				TreeItem<TreeItemValue> treeItem = this.saveCongruenceAnalysisPairs(treeItemType, groupName, enable, pairs);
				if (treeItem != null)
				{
					this.transferStrainParameters(groupId, (CongruenceAnalysisTreeItemValue)treeItem.getValue());

					lastValidTreeItem = treeItem;
				}
			}
			return lastValidTreeItem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<org.applied_geodesy.jag3d.ui.table.row.PointRow> getPoints(int groupId) throws java.sql.SQLException
		private IList<PointRow> getPoints(int groupId)
		{
			IList<PointRow> points = new List<PointRow>();

			string sql = "SELECT " + "\"name\", \"code\", \"enable\"," + "\"x0\", \"y0\", \"z0\", " + "\"sigma_y0\", \"sigma_x0\", \"sigma_z0\" " + "FROM \"PointApriori\" " + "WHERE \"group_id\" = ? " + "ORDER BY \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, groupId);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				PointRow point = new PointRow();

				string name = rs.getString("name");
				if (this.reservedPointNames.Contains(name))
				{
					continue;
				}

				point.Name = name;
				point.Code = rs.getString("code");
				point.Enable = rs.getBoolean("enable");

				double value;
				value = rs.getDouble("x0");
				point.XApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("y0");
				point.YApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("z0");
				point.ZApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("sigma_x0");
				point.SigmaXapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_y0");
				point.SigmaYapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_z0");
				point.SigmaZapriori = rs.wasNull() || value <= 0 ? null : value;

				points.Add(point);
			}
			return points;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<org.applied_geodesy.jag3d.ui.table.row.VerticalDeflectionRow> getVerticalDeflections(int groupId) throws java.sql.SQLException
		private IList<VerticalDeflectionRow> getVerticalDeflections(int groupId)
		{
			IList<VerticalDeflectionRow> verticalDeflections = new List<VerticalDeflectionRow>();

			string sql = "SELECT " + "\"name\", \"enable\"," + "\"x0\", \"y0\", " + "\"sigma_y0\", \"sigma_x0\" " + "FROM \"VerticalDeflectionApriori\" " + "WHERE \"group_id\" = ? " + "ORDER BY \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, groupId);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				VerticalDeflectionRow verticalDeflection = new VerticalDeflectionRow();

				string name = rs.getString("name");
				if (this.reservedVerticalDeflectionNames.Contains(name))
				{
					continue;
				}

				verticalDeflection.Name = name;
				verticalDeflection.Enable = rs.getBoolean("enable");

				double value;
				value = rs.getDouble("x0");
				verticalDeflection.XApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("y0");
				verticalDeflection.YApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("sigma_x0");
				verticalDeflection.SigmaXapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_y0");
				verticalDeflection.SigmaYapriori = rs.wasNull() || value <= 0 ? null : value;

				verticalDeflections.Add(verticalDeflection);
			}
			return verticalDeflections;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<org.applied_geodesy.jag3d.ui.table.row.ObservationRow> getTerrestrialObservations(int groupId) throws java.sql.SQLException
		private IList<ObservationRow> getTerrestrialObservations(int groupId)
		{
			IList<ObservationRow> observations = new List<ObservationRow>();

			string sql = "SELECT " + "\"start_point_name\", \"end_point_name\", \"instrument_height\", \"reflector_height\", \"value_0\", \"distance_0\", \"sigma_0\" AS \"sigma_0\", \"enable\" " + "FROM \"ObservationApriori\" " + "WHERE \"group_id\" = ? " + "ORDER BY \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, groupId);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				TerrestrialObservationRow observation = new TerrestrialObservationRow();

				observation.StartPointName = rs.getString("start_point_name");
				observation.EndPointName = rs.getString("end_point_name");
				observation.Enable = rs.getBoolean("enable");
				observation.ValueApriori = rs.getDouble("value_0");

				double value = rs.getDouble("instrument_height");
				if (!rs.wasNull())
				{
					observation.InstrumentHeight = value;
				}

				value = rs.getDouble("reflector_height");
				if (!rs.wasNull())
				{
					observation.ReflectorHeight = value;
				}

				value = rs.getDouble("distance_0");
				if (!rs.wasNull())
				{
					observation.DistanceApriori = value > 0 ? value : null;
				}

				value = rs.getDouble("sigma_0");
				if (!rs.wasNull())
				{
					observation.SigmaApriori = value > 0 ? value : null;
				}

				observations.Add(observation);
			}
			return observations;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow> getCongruenceAnalysisPairs(int groupId) throws java.sql.SQLException
		private IList<CongruenceAnalysisRow> getCongruenceAnalysisPairs(int groupId)
		{
			IList<CongruenceAnalysisRow> pairs = new List<CongruenceAnalysisRow>();

			string sql = "SELECT " + "\"start_point_name\", \"end_point_name\", \"enable\" " + "FROM \"CongruenceAnalysisPointPairApriori\" " + "WHERE \"group_id\" = ? " + "ORDER BY \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, groupId);

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				CongruenceAnalysisRow pair = new CongruenceAnalysisRow();

				pair.NameInReferenceEpoch = rs.getString("start_point_name");
				pair.NameInControlEpoch = rs.getString("end_point_name");
				pair.Enable = rs.getBoolean("enable");

				pairs.Add(pair);
			}
			return pairs;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<org.applied_geodesy.jag3d.ui.table.row.ObservationRow> getGNSSObservations(int groupId) throws java.sql.SQLException
		private IList<ObservationRow> getGNSSObservations(int groupId)
		{
			IList<ObservationRow> observations = new List<ObservationRow>();

			string sql = "SELECT " + "\"start_point_name\", \"end_point_name\", \"y0\", \"x0\", \"z0\", \"sigma_y0\", \"sigma_x0\", \"sigma_z0\", \"enable\" " + "FROM \"GNSSObservationApriori\" " + "WHERE \"group_id\" = ? " + "ORDER BY \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, groupId);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				GNSSObservationRow observation = new GNSSObservationRow();

				// Apriori-Values
				observation.StartPointName = rs.getString("start_point_name");
				observation.EndPointName = rs.getString("end_point_name");
				observation.Enable = rs.getBoolean("enable");
				double value;
				value = rs.getDouble("x0");
				observation.XApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("y0");
				observation.YApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("z0");
				observation.ZApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("sigma_x0");
				observation.SigmaXapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_y0");
				observation.SigmaYapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_z0");
				observation.SigmaZapriori = rs.wasNull() || value <= 0 ? null : value;

				observations.Add(observation);
			}
			return observations;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void transferUncertainties(int srcGroupId, org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue destPointItemValue) throws java.sql.SQLException
		private void transferUncertainties(int srcGroupId, PointTreeItemValue destPointItemValue)
		{
			string sql = "SELECT \"type\", \"value\" " + "FROM \"PointGroupUncertainty\" " + "WHERE \"group_id\" = ?";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, srcGroupId);

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				PointGroupUncertaintyType uncertaintyType = PointGroupUncertaintyType.getEnumByValue(rs.getInt("type"));
				if (uncertaintyType == null)
				{
					continue;
				}

				double value = rs.getDouble("value");
				SQLManager.Instance.saveUncertainty(uncertaintyType, value, destPointItemValue);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void transferUncertainties(int srcGroupId, org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue destVerticalDeflectionItemValue) throws java.sql.SQLException
		private void transferUncertainties(int srcGroupId, VerticalDeflectionTreeItemValue destVerticalDeflectionItemValue)
		{
			string sql = "SELECT \"type\", \"value\" " + "FROM \"VerticalDeflectionGroupUncertainty\" " + "WHERE \"group_id\" = ?";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, srcGroupId);

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				VerticalDeflectionGroupUncertaintyType uncertaintyType = VerticalDeflectionGroupUncertaintyType.getEnumByValue(rs.getInt("type"));
				if (uncertaintyType == null)
				{
					continue;
				}

				double value = rs.getDouble("value");
				SQLManager.Instance.saveUncertainty(uncertaintyType, value, destVerticalDeflectionItemValue);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void transferUncertainties(int srcGroupId, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue destObservationItemValue) throws java.sql.SQLException
		private void transferUncertainties(int srcGroupId, ObservationTreeItemValue destObservationItemValue)
		{
			string sql = "SELECT \"type\", \"value\" " + "FROM \"ObservationGroupUncertainty\" " + "WHERE \"group_id\" = ?";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, srcGroupId);

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				ObservationGroupUncertaintyType uncertaintyType = ObservationGroupUncertaintyType.getEnumByValue(rs.getInt("type"));
				if (uncertaintyType == null)
				{
					continue;
				}

				double value = rs.getDouble("value");
				SQLManager.Instance.saveUncertainty(uncertaintyType, value, destObservationItemValue);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void transferEpoch(int srcGroupId, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue destObservationItemValue) throws java.sql.SQLException
		private void transferEpoch(int srcGroupId, ObservationTreeItemValue destObservationItemValue)
		{
			string sql = "SELECT \"reference_epoch\" " + "FROM \"ObservationGroup\" " + "WHERE \"id\" = ?";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, srcGroupId);

			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				bool referenceEpoch = rs.getBoolean("reference_epoch");
				SQLManager.Instance.saveEpoch(referenceEpoch, destObservationItemValue);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void transferAdditionalParameters(int srcGroupId, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue destObservationItemValue) throws java.sql.SQLException
		private void transferAdditionalParameters(int srcGroupId, ObservationTreeItemValue destObservationItemValue)
		{
			string sql = "SELECT \"type\", \"value_0\", \"enable\" " + "FROM \"AdditionalParameterApriori\" " + "WHERE \"group_id\" = ? ";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, srcGroupId);

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				ParameterType parameterType = ParameterType.getEnumByValue(rs.getInt("type"));
				if (parameterType == null)
				{
					continue;
				}

				bool enable = rs.getBoolean("enable");
				double value = rs.getDouble("value_0");
				SQLManager.Instance.saveAdditionalParameter(parameterType, enable, value, destObservationItemValue);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void transferStrainParameters(int srcGroupId, org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue destCongruenceAnalysisItemValue) throws java.sql.SQLException
		private void transferStrainParameters(int srcGroupId, CongruenceAnalysisTreeItemValue destCongruenceAnalysisItemValue)
		{
			string sql = "SELECT \"type\", \"enable\" " + "FROM \"CongruenceAnalysisStrainParameterRestriction\" " + "WHERE \"group_id\" = ? ";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, srcGroupId);

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				RestrictionType restrictionType = RestrictionType.getEnumByValue(rs.getInt("type"));
				if (restrictionType == null)
				{
					continue;
				}

				bool enable = rs.getBoolean("enable");
				SQLManager.Instance.saveStrainParameter(restrictionType, enable, destCongruenceAnalysisItemValue);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static boolean isCurrentOADBVersion(org.applied_geodesy.util.sql.DataBase dataBase) throws SQLException, ClassNotFoundException
		public static bool isCurrentOADBVersion(DataBase dataBase)
		{
			bool isOpen = dataBase.Open;

			try
			{
				if (!isOpen)
				{
					dataBase.open();
				}

				string sql = "SELECT TRUE AS \"exists\" " + "FROM \"INFORMATION_SCHEMA\".\"COLUMNS\" " + "WHERE \"TABLE_SCHEMA\" = 'OpenAdjustment' " + "AND \"TABLE_NAME\" = 'Version' " + "AND \"COLUMN_NAME\" = 'version'";

				PreparedStatement stmt = dataBase.getPreparedStatement(sql);
				ResultSet rs = stmt.executeQuery();

				if (rs.next())
				{
					bool exists = rs.getBoolean("exists");

					if (!rs.wasNull() && exists)
					{
						sql = "SET SCHEMA \"OpenAdjustment\";";
						stmt = dataBase.getPreparedStatement(sql);
						stmt.execute();

						sql = "SELECT \"version\" FROM \"Version\" WHERE \"type\" = ?";
						stmt = dataBase.getPreparedStatement(sql);
						stmt.setInt(1, VersionType.DATABASE.getId());
						rs = stmt.executeQuery();

						if (rs.next())
						{
							double databaseVersion = rs.getDouble("version");
							if (rs.wasNull())
							{
								throw new SQLException("Error, could not detect database version!");
							}

							return databaseVersion == Version.get(VersionType.DATABASE);
						}
					}
				}
			}
			finally
			{
				if (!isOpen)
				{
					dataBase.close();
				}
			}
			return false;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveVerticalDeflections(org.applied_geodesy.jag3d.ui.tree.TreeItemType treeItemType, String itemName, boolean enable, java.util.List<org.applied_geodesy.jag3d.ui.table.row.VerticalDeflectionRow> verticalDeflections) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveVerticalDeflections(TreeItemType treeItemType, string itemName, bool enable, IList<VerticalDeflectionRow> verticalDeflections)
		{
			if (verticalDeflections == null || verticalDeflections.Count == 0)
			{
				return null;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(treeItemType);
			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, -1, itemName, enable, false);
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
				foreach (VerticalDeflectionRow row in verticalDeflections)
				{
					row.GroupId = groupId;
					SQLManager.Instance.saveItem(row);
					this.reservedVerticalDeflectionNames.Add(row.Name);
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
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> savePoints(org.applied_geodesy.jag3d.ui.tree.TreeItemType treeItemType, String itemName, boolean enable, java.util.List<org.applied_geodesy.jag3d.ui.table.row.PointRow> points) throws java.sql.SQLException
		private TreeItem<TreeItemValue> savePoints(TreeItemType treeItemType, string itemName, bool enable, IList<PointRow> points)
		{
			if (points == null || points.Count == 0)
			{
				return null;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(treeItemType);
			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, -1, itemName, enable, false);
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
					this.reservedPointNames.Add(row.Name);
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
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveObservations(org.applied_geodesy.jag3d.ui.tree.TreeItemType treeItemType, String groupName, boolean enable, java.util.List<org.applied_geodesy.jag3d.ui.table.row.ObservationRow> observations) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveObservations(TreeItemType treeItemType, string groupName, bool enable, IList<ObservationRow> observations)
		{
			if (observations == null || observations.Count == 0)
			{
				return null;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(treeItemType);
			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, -1, groupName, enable, false);

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
				foreach (ObservationRow row in observations)
				{
					row.GroupId = groupId;
					if (TreeItemType.isObservationTypeLeaf(treeItemType))
					{
						SQLManager.Instance.saveItem((TerrestrialObservationRow)row);
					}
					else if (TreeItemType.isGNSSObservationTypeLeaf(treeItemType))
					{
						SQLManager.Instance.saveItem((GNSSObservationRow)row);
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
//ORIGINAL LINE: private javafx.scene.control.TreeItem<org.applied_geodesy.jag3d.ui.tree.TreeItemValue> saveCongruenceAnalysisPairs(org.applied_geodesy.jag3d.ui.tree.TreeItemType treeItemType, String groupName, boolean enable, java.util.List<org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow> pairs) throws java.sql.SQLException
		private TreeItem<TreeItemValue> saveCongruenceAnalysisPairs(TreeItemType treeItemType, string groupName, bool enable, IList<CongruenceAnalysisRow> pairs)
		{
			if (pairs == null || pairs.Count == 0)
			{
				return null;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(treeItemType);
			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, -1, groupName, enable, false);

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
				foreach (CongruenceAnalysisRow row in pairs)
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
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("OADBReader.extension.script", "HyperSQL"), "*.script")};
			}
		}
	}

}