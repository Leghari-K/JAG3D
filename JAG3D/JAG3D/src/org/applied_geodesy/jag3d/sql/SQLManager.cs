using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

namespace org.applied_geodesy.jag3d.sql
{

	using EstimationType = org.applied_geodesy.adjustment.EstimationType;
	using DefaultAverageThreshold = org.applied_geodesy.adjustment.network.DefaultAverageThreshold;
	using DefectType = org.applied_geodesy.adjustment.network.DefectType;
	using ObservationGroupUncertaintyType = org.applied_geodesy.adjustment.network.ObservationGroupUncertaintyType;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using PointGroupUncertaintyType = org.applied_geodesy.adjustment.network.PointGroupUncertaintyType;
	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using RankDefect = org.applied_geodesy.adjustment.network.RankDefect;
	using VarianceComponentType = org.applied_geodesy.adjustment.network.VarianceComponentType;
	using VerticalDeflectionGroupUncertaintyType = org.applied_geodesy.adjustment.network.VerticalDeflectionGroupUncertaintyType;
	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using SQLApproximationManager = org.applied_geodesy.adjustment.network.approximation.sql.SQLApproximationManager;
	using RestrictionType = org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType;
	using ProjectionType = org.applied_geodesy.adjustment.network.observation.reduction.ProjectionType;
	using Reduction = org.applied_geodesy.adjustment.network.observation.reduction.Reduction;
	using ReductionTaskType = org.applied_geodesy.adjustment.network.observation.reduction.ReductionTaskType;
	using SQLAdjustmentManager = org.applied_geodesy.adjustment.network.sql.SQLAdjustmentManager;
	using TestStatisticDefinition = org.applied_geodesy.adjustment.statistic.TestStatisticDefinition;
	using TestStatisticType = org.applied_geodesy.adjustment.statistic.TestStatisticType;
	using LeastSquaresSettings = org.applied_geodesy.jag3d.ui.dialog.LeastSquaresSettingDialog.LeastSquaresSettings;
	using ScopeType = org.applied_geodesy.jag3d.ui.dialog.ScopeType;
	using UIGraphicPaneBuilder = org.applied_geodesy.jag3d.ui.graphic.UIGraphicPaneBuilder;
	using SQLGraphicManager = org.applied_geodesy.jag3d.ui.graphic.sql.SQLGraphicManager;
	using ImportOption = org.applied_geodesy.jag3d.ui.io.ImportOption;
	using FTLReport = org.applied_geodesy.jag3d.ui.io.report.FTLReport;
	using MetaData = org.applied_geodesy.jag3d.ui.metadata.MetaData;
	using UIMetaDataPaneBuilder = org.applied_geodesy.jag3d.ui.metadata.UIMetaDataPaneBuilder;
	using UICongruenceAnalysisPropertiesPane = org.applied_geodesy.jag3d.ui.propertiespane.UICongruenceAnalysisPropertiesPane;
	using UICongruenceAnalysisPropertiesPaneBuilder = org.applied_geodesy.jag3d.ui.propertiespane.UICongruenceAnalysisPropertiesPaneBuilder;
	using UIObservationPropertiesPane = org.applied_geodesy.jag3d.ui.propertiespane.UIObservationPropertiesPane;
	using UIObservationPropertiesPaneBuilder = org.applied_geodesy.jag3d.ui.propertiespane.UIObservationPropertiesPaneBuilder;
	using UIPointPropertiesPane = org.applied_geodesy.jag3d.ui.propertiespane.UIPointPropertiesPane;
	using UIPointPropertiesPaneBuilder = org.applied_geodesy.jag3d.ui.propertiespane.UIPointPropertiesPaneBuilder;
	using UIVerticalDeflectionPropertiesPane = org.applied_geodesy.jag3d.ui.propertiespane.UIVerticalDeflectionPropertiesPane;
	using UIVerticalDeflectionPropertiesPaneBuilder = org.applied_geodesy.jag3d.ui.propertiespane.UIVerticalDeflectionPropertiesPaneBuilder;
	using UIAdditionalParameterTableBuilder = org.applied_geodesy.jag3d.ui.table.UIAdditionalParameterTableBuilder;
	using UICongruenceAnalysisTableBuilder = org.applied_geodesy.jag3d.ui.table.UICongruenceAnalysisTableBuilder;
	using UIGNSSObservationTableBuilder = org.applied_geodesy.jag3d.ui.table.UIGNSSObservationTableBuilder;
	using UIPointTableBuilder = org.applied_geodesy.jag3d.ui.table.UIPointTableBuilder;
	using UIPrincipalComponentTableBuilder = org.applied_geodesy.jag3d.ui.table.UIPrincipalComponentTableBuilder;
	using UITerrestrialObservationTableBuilder = org.applied_geodesy.jag3d.ui.table.UITerrestrialObservationTableBuilder;
	using UITestStatisticTableBuilder = org.applied_geodesy.jag3d.ui.table.UITestStatisticTableBuilder;
	using UIVarianceComponentTableBuilder = org.applied_geodesy.jag3d.ui.table.UIVarianceComponentTableBuilder;
	using UIVerticalDeflectionTableBuilder = org.applied_geodesy.jag3d.ui.table.UIVerticalDeflectionTableBuilder;
	using ColumnContentType = org.applied_geodesy.jag3d.ui.table.column.ColumnContentType;
	using ColumnPropertiesManager = org.applied_geodesy.jag3d.ui.table.column.ColumnPropertiesManager;
	using ColumnProperty = org.applied_geodesy.jag3d.ui.table.column.ColumnProperty;
	using TableContentType = org.applied_geodesy.jag3d.ui.table.column.TableContentType;
	using AdditionalParameterRow = org.applied_geodesy.jag3d.ui.table.row.AdditionalParameterRow;
	using CongruenceAnalysisRow = org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow;
	using GNSSObservationRow = org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow;
	using PointRow = org.applied_geodesy.jag3d.ui.table.row.PointRow;
	using PrincipalComponentRow = org.applied_geodesy.jag3d.ui.table.row.PrincipalComponentRow;
	using Row = org.applied_geodesy.jag3d.ui.table.row.Row;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using TestStatisticRow = org.applied_geodesy.jag3d.ui.table.row.TestStatisticRow;
	using VarianceComponentRow = org.applied_geodesy.jag3d.ui.table.row.VarianceComponentRow;
	using VerticalDeflectionRow = org.applied_geodesy.jag3d.ui.table.row.VerticalDeflectionRow;
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightRangeType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using CongruenceAnalysisTreeItemValue = org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue;
	using Groupable = org.applied_geodesy.jag3d.ui.tree.Groupable;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using PointTreeItemValue = org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using VerticalDeflectionTreeItemValue = org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue;
	using Ellipsoid = org.applied_geodesy.transformation.datum.Ellipsoid;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using FormatterOption = org.applied_geodesy.util.FormatterOptions.FormatterOption;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using DataBase = org.applied_geodesy.util.sql.DataBase;
	using AngleUnit = org.applied_geodesy.util.unit.AngleUnit;
	using LengthUnit = org.applied_geodesy.util.unit.LengthUnit;
	using PercentUnit = org.applied_geodesy.util.unit.PercentUnit;
	using ScaleUnit = org.applied_geodesy.util.unit.ScaleUnit;
	using Unit = org.applied_geodesy.util.unit.Unit;
	using UnitType = org.applied_geodesy.util.unit.UnitType;
	using DatabaseVersionMismatchException = org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException;
	using Version = org.applied_geodesy.version.jag3d.Version;
	using VersionType = org.applied_geodesy.version.VersionType;

	using HostServices = javafx.application.HostServices;
	using FXCollections = javafx.collections.FXCollections;
	using ButtonType = javafx.scene.control.ButtonType;
	using SortType = javafx.scene.control.TableColumn.SortType;
	using TableView = javafx.scene.control.TableView;
	using TreeItem = javafx.scene.control.TreeItem;
	using Color = javafx.scene.paint.Color;
	using Pair = javafx.util.Pair;

	public class SQLManager
	{
		private I18N i18n = I18N.Instance;
		private DataBase dataBase;
		private HostServices hostServices;
		private IList<EventListener> listenerList = new List<EventListener>();
		private static SQLManager SQL_MANAGER = new SQLManager();
		public const double EQUAL_VALUE_TRESHOLD = 0.0001;

		private SQLManager()
		{
		}

		public static SQLManager Instance
		{
			get
			{
				return SQL_MANAGER;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static SQLManager openExistingProject(org.applied_geodesy.util.sql.DataBase db) throws ClassNotFoundException, SQLException, org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException
		public static SQLManager openExistingProject(DataBase db)
		{
			SQL_MANAGER.DataBase = db;
			SQL_MANAGER.prepareDataBase(false);
			SQL_MANAGER.loadExistingDataBase();

			UITreeBuilder.Instance.Tree.getSelectionModel().clearSelection();
			UITreeBuilder.Instance.Tree.getSelectionModel().selectFirst();

			return SQL_MANAGER;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static SQLManager createNewProject(org.applied_geodesy.util.sql.DataBase db) throws ClassNotFoundException, SQLException, org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException
		public static SQLManager createNewProject(DataBase db)
		{
			SQL_MANAGER.DataBase = db;
			SQL_MANAGER.prepareDataBase(true);

			UITreeBuilder.Instance.Tree.getSelectionModel().clearSelection();
			UITreeBuilder.Instance.Tree.getSelectionModel().selectFirst();

			return SQL_MANAGER;
		}

		public static HostServices HostServices
		{
			set
			{
				SQL_MANAGER.hostServices = value;
			}
		}

		public static void closeProject()
		{
			SQL_MANAGER.closeDataBase();

			UITreeBuilder.Instance.Tree.getSelectionModel().clearSelection();
			UITreeBuilder.Instance.removeAllItems();
			UITreeBuilder.Instance.Tree.getSelectionModel().selectFirst();
		}

		public virtual FTLReport FTLReport
		{
			get
			{
				if (this.hasDatabase() && this.dataBase.Open)
				{
					FTLReport ftlReport = new FTLReport(this.dataBase, this.hostServices);
					return ftlReport;
				}
				return null;
			}
		}

		public virtual SQLAdjustmentManager AdjustmentManager
		{
			get
			{
				if (this.hasDatabase() && this.dataBase.Open)
				{
					return new SQLAdjustmentManager(this.dataBase);
				}
				return null;
			}
		}

		public virtual SQLGraphicManager SQLGraphicManager
		{
			get
			{
				if (this.hasDatabase() && this.dataBase.Open)
				{
					return new SQLGraphicManager(this.dataBase);
				}
				return null;
			}
		}

		public virtual SQLApproximationManager ApproximationManager
		{
			get
			{
				if (this.hasDatabase() && this.dataBase.Open)
				{
					return new SQLApproximationManager(this.dataBase);
				}
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void setDataBase(org.applied_geodesy.util.sql.DataBase db) throws ClassNotFoundException, java.sql.SQLException
		private DataBase DataBase
		{
			set
			{
				this.closeDataBase();
				this.fireDatabaseStateChanged(ProjectDatabaseStateType.OPENING);
				this.dataBase = value;
				this.dataBase.open();
				this.fireDatabaseStateChanged(this.hasDatabase() ? ProjectDatabaseStateType.OPENED : ProjectDatabaseStateType.CLOSED);
			}
			get
			{
				return this.dataBase;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadExistingDataBase() throws java.sql.SQLException
		private void loadExistingDataBase()
		{
			this.loadTableRowHighlight();
			this.loadTableColumnProperties();
			this.loadImportPreferences();

			this.loadPointGroups();
			this.loadObservationGroups();
			this.loadGNSSObservationGroups();
			this.loadCongruenceAnalysisGroups();
			this.loadVerticalDeflectioGroups();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void prepareDataBase(boolean newProject) throws SQLException, org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException
		private void prepareDataBase(bool newProject)
		{
			UITreeBuilder.Instance.Tree.getSelectionModel().clearSelection();
			UITreeBuilder.Instance.removeAllItems();

			UIGraphicPaneBuilder.Instance.LayerManager.clearAllLayers();
			bool transferOADB3FxT0FX = false;
			string[] initSqls = new string[0];
			if (!newProject && this.OADBVersionFX)
			{ //&& false // use false to force update
				initSqls = new string[] {"SET SCHEMA \"OpenAdjustment\";"};
			}
			else
			{
				// Update old DB to the last state of 3.x
				if (!newProject && this.OADBVersion3x)
				{
					Optional<ButtonType> result = OptionDialog.showConfirmationDialog(i18n.getString("SQLManager.database.version.3x.title", "JAG3D v3.x project detected"), i18n.getString("SQLManager.database.version.3x.header", "Do you want to update the selected database?"), i18n.getString("SQLManager.database.version.3x.message", "The selected database contains an existing project, created by an earlier version 3.x of JAG3D. JAG3D can try to update the existing project to the current version. Please create a backup of the database bevor running the update."));
					if (result.get() == ButtonType.OK)
					{
						SQLManager3x.updateOADB3x(this.dataBase);
						transferOADB3FxT0FX = true;
					}
					else
					{
						this.closeDataBase(); // Stop openning process
						// return
					}
				}

				initSqls = new string[] {"SET FILES NIO SIZE 8192;", "DROP SCHEMA \"OpenAdjustment\" IF EXISTS CASCADE;", "CREATE SCHEMA \"OpenAdjustment\";", "SET INITIAL SCHEMA \"OpenAdjustment\";", "SET SCHEMA \"OpenAdjustment\";", "CREATE " + SQLDatabase.TABLE_STORAGE_TYPE + " TABLE \"Version\"(\"type\" SMALLINT NOT NULL PRIMARY KEY, \"version\" DOUBLE NOT NULL);", "INSERT INTO \"Version\" (\"type\", \"version\") VALUES (" + VersionType.ADJUSTMENT_CORE.getId() + ", 0), (" + VersionType.DATABASE.getId() + ", 0), (" + VersionType.USER_INTERFACE.getId() + ", 0);"};
			}

			foreach (string sql in initSqls)
			{
				PreparedStatement statment = this.dataBase.getPreparedStatement(sql);
				statment.execute();
			}

			if (transferOADB3FxT0FX && !newProject && this.OADBVersion3x)
			{
				SQLManager3x.transferOADB3xToFX(this.dataBase);
			}

			this.updateDatabase();
			this.initFormatterOptions();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void updateDatabase() throws SQLException, org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException
		private void updateDatabase()
		{
			PreparedStatement stmt = null;
			const string sqlSelectVersion = "SELECT \"version\" FROM \"Version\" WHERE \"type\" = ?";
			const string sqlUpdateVersion = "UPDATE \"Version\" SET \"version\" = ? WHERE \"type\" = ?";

			stmt = this.dataBase.getPreparedStatement(sqlSelectVersion);
			stmt.setInt(1, VersionType.DATABASE.getId());
			ResultSet rs = stmt.executeQuery();

			double databaseVersion = -1;
			if (rs.next())
			{
				databaseVersion = rs.getDouble("version");
				if (rs.wasNull())
				{
					throw new SQLException(this.GetType().Name + " : Error, could not detect database version. Database update failed!");
				}

				if (databaseVersion > Version.get(VersionType.DATABASE))
				{
					//this.closeDataBase();
					throw new DatabaseVersionMismatchException("Error, database version of the stored project is greater than accepted database version of the application: " + databaseVersion + " > " + Version.get(VersionType.DATABASE));
				}

				IDictionary<double, string> querys = SQLDatabase.dataBase();
				foreach (KeyValuePair<double, string> query in querys.SetOfKeyValuePairs())
				{
					double subDBVersion = query.Key;
					string sql = query.Value;
					if (subDBVersion > databaseVersion && subDBVersion <= Version.get(VersionType.DATABASE))
					{
						stmt = this.dataBase.getPreparedStatement(sql);
						stmt.execute();

						// Speichere die Version des DB-Updates
						stmt = this.dataBase.getPreparedStatement(sqlUpdateVersion);
						stmt.setDouble(1, subDBVersion);
						stmt.setInt(2, VersionType.DATABASE.getId());
						stmt.execute();
					}
				}

				stmt = this.dataBase.getPreparedStatement(sqlUpdateVersion);
				stmt.setDouble(1, Version.get(VersionType.DATABASE));
				stmt.setInt(2, VersionType.DATABASE.getId());
				stmt.execute();

				stmt.setDouble(1, Version.get(VersionType.USER_INTERFACE));
				stmt.setInt(2, VersionType.USER_INTERFACE.getId());
				stmt.execute();

			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private boolean isOADBVersionFX() throws java.sql.SQLException
		private bool OADBVersionFX
		{
			get
			{
				string sql = "SELECT TRUE AS \"exists\" " + "FROM \"INFORMATION_SCHEMA\".\"COLUMNS\" " + "WHERE \"TABLE_SCHEMA\" = 'OpenAdjustment' AND " + "\"TABLE_NAME\"  = 'Version' AND " + "\"COLUMN_NAME\" = 'type'";
    
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				ResultSet rs = stmt.executeQuery();
    
				if (rs.next())
				{
					bool exists = rs.getBoolean("exists");
					return !rs.wasNull() && exists;
				}
				return false;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private boolean isOADBVersion3x() throws java.sql.SQLException
		private bool OADBVersion3x
		{
			get
			{
				string sql = "SELECT TRUE AS \"exists\" " + "FROM \"INFORMATION_SCHEMA\".\"COLUMNS\" " + "WHERE \"TABLE_SCHEMA\" = 'PUBLIC' AND " + "\"TABLE_NAME\"  = 'GeneralSetting' AND " + "\"COLUMN_NAME\" = 'database_version'";
    
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				ResultSet rs = stmt.executeQuery();
    
				if (rs.next())
				{
					bool exists = rs.getBoolean("exists");
					return !rs.wasNull() && exists;
				}
				return false;
			}
		}

		public virtual bool hasDatabase()
		{
			return this.dataBase != null;
		}

		public virtual void closeDataBase()
		{
			if (this.dataBase != null && this.dataBase.Open)
			{
				this.fireDatabaseStateChanged(ProjectDatabaseStateType.CLOSING);

				// save user-defined settings
				try
				{
					this.saveTableColumnProperties();
				}
				catch (SQLException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

				this.dataBase.close();
				this.dataBase = null;
				this.fireDatabaseStateChanged(ProjectDatabaseStateType.CLOSED);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void initFormatterOptions() throws java.sql.SQLException
		private void initFormatterOptions()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "SELECT \"unit\", \"digits\" FROM \"FormatterOption\" WHERE \"type\" = ?";

			IDictionary<CellValueType, FormatterOptions.FormatterOption> options = FormatterOptions.Instance.FormatterOptions;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			foreach (FormatterOptions.FormatterOption option in options.Values)
			{
				CellValueType type = option.Type;
				int idx = 1;
				stmt.setInt(idx++, type.getId());

				ResultSet rs = stmt.executeQuery();
				if (rs.next())
				{
					Unit unit = null;
					int digits = rs.getInt("digits");
					UnitType unitType = UnitType.getEnumByValue(rs.getInt("unit"));
					if (!rs.wasNull() && unitType != null)
					{
						if ((type == CellValueType.LENGTH || type == CellValueType.LENGTH_RESIDUAL || type == CellValueType.LENGTH_UNCERTAINTY) && unit == null)
						{
							unit = LengthUnit.getUnit(unitType);
						}
						if ((type == CellValueType.ANGLE || type == CellValueType.ANGLE_RESIDUAL || type == CellValueType.ANGLE_UNCERTAINTY) && unit == null)
						{
							unit = AngleUnit.getUnit(unitType);
						}
						if ((type == CellValueType.SCALE || type == CellValueType.SCALE_RESIDUAL || type == CellValueType.SCALE_UNCERTAINTY) && unit == null)
						{
							unit = ScaleUnit.getUnit(unitType);
						}
						if (type == CellValueType.PERCENTAGE && unit == null)
						{
							unit = PercentUnit.getUnit(unitType);
						}
					}

					if (digits >= 0)
					{
						option.FractionDigits = digits;
					}
					if (unit != null)
					{
						option.Unit = unit;
					}
				}
				else
				{
					this.saveFormatterOption(option);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadMetaData(org.applied_geodesy.jag3d.ui.metadata.MetaData metaData) throws java.sql.SQLException
		private void loadMetaData(MetaData metaData)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "SELECT \"name\", \"operator\", \"description\", \"date\", \"customer_id\", \"project_id\" " + "FROM \"ProjectMetadata\" " + "WHERE \"id\" = 1 LIMIT 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				metaData.Name = rs.getString("name");
				metaData.Operator = rs.getString("operator");
				metaData.CustomerId = rs.getString("customer_id");
				metaData.ProjectId = rs.getString("project_id");
				metaData.Description = rs.getString("description");
				metaData.Date = rs.getTimestamp("date").toLocalDateTime().toLocalDate();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadObservationGroups() throws java.sql.SQLException
		private void loadObservationGroups()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UITreeBuilder treeBuilder = UITreeBuilder.Instance;
			string sql = "SELECT \"id\", \"name\", \"enable\" " + "FROM \"ObservationGroup\" " + "WHERE \"type\" = ? " + "ORDER BY \"order\" ASC, \"id\" ASC";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			TreeItemType[] types = new TreeItemType[] {TreeItemType.LEVELING_DIRECTORY, TreeItemType.DIRECTION_DIRECTORY, TreeItemType.HORIZONTAL_DISTANCE_DIRECTORY, TreeItemType.SLOPE_DISTANCE_DIRECTORY, TreeItemType.ZENITH_ANGLE_DIRECTORY};

			foreach (TreeItemType parentType in types)
			{
				ObservationType obsType = TreeItemType.getObservationTypeByTreeItemType(parentType);
				if (obsType == null)
				{
					Console.Error.WriteLine(this.GetType().Name + " : Error, cannot convert tree item type to observation type " + parentType);
					continue;
				}

				stmt.setInt(1, obsType.getId());
				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					int id = rs.getInt("id");
					bool enable = rs.getBoolean("enable");
					string name = rs.getString("name");

					treeBuilder.addItem(parentType, id, name, enable, false);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadGNSSObservationGroups() throws java.sql.SQLException
		private void loadGNSSObservationGroups()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UITreeBuilder treeBuilder = UITreeBuilder.Instance;
			string sql = "SELECT \"id\", \"name\", \"enable\" " + "FROM \"ObservationGroup\" " + "WHERE \"type\" = ? " + "ORDER BY \"order\" ASC, \"id\" ASC";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			TreeItemType[] types = new TreeItemType[] {TreeItemType.GNSS_1D_DIRECTORY, TreeItemType.GNSS_2D_DIRECTORY, TreeItemType.GNSS_3D_DIRECTORY};

			foreach (TreeItemType parentType in types)
			{
				ObservationType obsType = TreeItemType.getObservationTypeByTreeItemType(parentType);
				if (obsType == null)
				{
					Console.Error.WriteLine(this.GetType().Name + " : Error, cannot convert tree item type to observation type " + parentType);
					continue;
				}

				stmt.setInt(1, obsType.getId());
				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					int id = rs.getInt("id");
					bool enable = rs.getBoolean("enable");
					string name = rs.getString("name");

					treeBuilder.addItem(parentType, id, name, enable, false);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadPointGroups() throws java.sql.SQLException
		private void loadPointGroups()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UITreeBuilder treeBuilder = UITreeBuilder.Instance;
			string sql = "SELECT \"id\", \"name\", \"enable\" " + "FROM \"PointGroup\" " + "WHERE \"type\" = ? AND \"dimension\" = ? " + "ORDER BY \"order\" ASC, \"id\" ASC";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			TreeItemType[] types = new TreeItemType[] {TreeItemType.REFERENCE_POINT_1D_DIRECTORY, TreeItemType.REFERENCE_POINT_2D_DIRECTORY, TreeItemType.REFERENCE_POINT_3D_DIRECTORY, TreeItemType.STOCHASTIC_POINT_1D_DIRECTORY, TreeItemType.STOCHASTIC_POINT_2D_DIRECTORY, TreeItemType.STOCHASTIC_POINT_3D_DIRECTORY, TreeItemType.DATUM_POINT_1D_DIRECTORY, TreeItemType.DATUM_POINT_2D_DIRECTORY, TreeItemType.DATUM_POINT_3D_DIRECTORY, TreeItemType.NEW_POINT_1D_DIRECTORY, TreeItemType.NEW_POINT_2D_DIRECTORY, TreeItemType.NEW_POINT_3D_DIRECTORY};

			foreach (TreeItemType parentType in types)
			{
				int dimension = -1;
				switch (parentType.innerEnumValue)
				{
				case TreeItemType.InnerEnum.REFERENCE_POINT_1D_DIRECTORY:
				case TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_DIRECTORY:
				case TreeItemType.InnerEnum.DATUM_POINT_1D_DIRECTORY:
				case TreeItemType.InnerEnum.NEW_POINT_1D_DIRECTORY:
					dimension = 1;
					break;
				case TreeItemType.InnerEnum.REFERENCE_POINT_2D_DIRECTORY:
				case TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_DIRECTORY:
				case TreeItemType.InnerEnum.DATUM_POINT_2D_DIRECTORY:
				case TreeItemType.InnerEnum.NEW_POINT_2D_DIRECTORY:
					dimension = 2;
					break;
				case TreeItemType.InnerEnum.REFERENCE_POINT_3D_DIRECTORY:
				case TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_DIRECTORY:
				case TreeItemType.InnerEnum.DATUM_POINT_3D_DIRECTORY:
				case TreeItemType.InnerEnum.NEW_POINT_3D_DIRECTORY:
					dimension = 3;
					break;
				default:
					dimension = -1;
					break;
				}


				PointType pointType = TreeItemType.getPointTypeByTreeItemType(parentType);
				if (pointType == null)
				{
					Console.Error.WriteLine(this.GetType().Name + " : Error, cannot convert tree item type to point type " + parentType);
					continue;
				}

				stmt.setInt(1, pointType.getId());
				stmt.setInt(2, dimension);
				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					int id = rs.getInt("id");
					bool enable = rs.getBoolean("enable");
					string name = rs.getString("name");

					treeBuilder.addItem(parentType, id, name, enable, false);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadVerticalDeflectioGroups() throws java.sql.SQLException
		private void loadVerticalDeflectioGroups()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UITreeBuilder treeBuilder = UITreeBuilder.Instance;
			string sql = "SELECT \"id\", \"name\", \"enable\" " + "FROM \"VerticalDeflectionGroup\" " + "WHERE \"type\" = ? " + "ORDER BY \"order\" ASC, \"id\" ASC";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			TreeItemType[] types = new TreeItemType[] {TreeItemType.REFERENCE_VERTICAL_DEFLECTION_DIRECTORY, TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY, TreeItemType.UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY};

			foreach (TreeItemType parentType in types)
			{
				VerticalDeflectionType verticalDeflectionType = TreeItemType.getVerticalDeflectionTypeByTreeItemType(parentType);
				if (verticalDeflectionType == null)
				{
					Console.Error.WriteLine(this.GetType().Name + " : Error, cannot convert tree item type to vertical deflection type " + parentType);
					continue;
				}

				stmt.setInt(1, verticalDeflectionType.getId());
				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					int id = rs.getInt("id");
					bool enable = rs.getBoolean("enable");
					string name = rs.getString("name");

					treeBuilder.addItem(parentType, id, name, enable, false);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadCongruenceAnalysisGroups() throws java.sql.SQLException
		private void loadCongruenceAnalysisGroups()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UITreeBuilder treeBuilder = UITreeBuilder.Instance;

			string sql = "SELECT \"id\", \"name\", \"enable\" " + "FROM \"CongruenceAnalysisGroup\" " + "WHERE \"dimension\" = ? " + "ORDER BY \"order\" ASC, \"id\" ASC";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			TreeItemType[] types = new TreeItemType[] {TreeItemType.CONGRUENCE_ANALYSIS_1D_DIRECTORY, TreeItemType.CONGRUENCE_ANALYSIS_2D_DIRECTORY, TreeItemType.CONGRUENCE_ANALYSIS_3D_DIRECTORY};

			foreach (TreeItemType parentType in types)
			{
				int dimension = -1;
				switch (parentType.innerEnumValue)
				{
				case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_DIRECTORY:
					dimension = 1;
					break;
				case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_DIRECTORY:
					dimension = 2;
					break;
				case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_DIRECTORY:
					dimension = 3;
					break;
				default:
					dimension = -1;
					break;
				}

				stmt.setInt(1, dimension);
				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					int id = rs.getInt("id");
					bool enable = rs.getBoolean("enable");
					string name = rs.getString("name");

					treeBuilder.addItem(parentType, id, name, enable, false);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void loadData(org.applied_geodesy.jag3d.ui.tree.TreeItemValue itemValue, org.applied_geodesy.jag3d.ui.tree.TreeItemValue... selectedTreeItemValues) throws java.sql.SQLException
		public virtual void loadData(TreeItemValue itemValue, params TreeItemValue[] selectedTreeItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			TreeItemType treeItemType = itemValue.ItemType;

			switch (treeItemType.innerEnumValue)
			{
			case TreeItemType.InnerEnum.ROOT:
				MetaData metaData = UIMetaDataPaneBuilder.Instance.MetaData;
				this.loadMetaData(metaData);
				this.loadTestStatistics();
				this.loadVarianceComponents();
				this.loadPrincipalComponents();
				break;
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
				if (itemValue is CongruenceAnalysisTreeItemValue)
				{
					CongruenceAnalysisTreeItemValue congruenceAnalysisTreeItemValue = (CongruenceAnalysisTreeItemValue)itemValue;
					CongruenceAnalysisTreeItemValue[] selectedCongruenceAnalysisItemValuesArray = null;
					ISet<CongruenceAnalysisTreeItemValue> selectedCongruenceAnalysisItemValues = new LinkedHashSet<CongruenceAnalysisTreeItemValue>();
					selectedCongruenceAnalysisItemValues.Add(congruenceAnalysisTreeItemValue);

					if (selectedTreeItemValues != null)
					{
						foreach (TreeItemValue selectedItem in selectedTreeItemValues)
						{
							if (selectedItem is CongruenceAnalysisTreeItemValue)
							{
								selectedCongruenceAnalysisItemValues.Add((CongruenceAnalysisTreeItemValue)selectedItem);
							}
						}
					}
					selectedCongruenceAnalysisItemValuesArray = selectedCongruenceAnalysisItemValues.ToArray();

					//UICongruenceAnalysisPropertiesPaneBuilder.getInstance().getCongruenceAnalysisPropertiesPane(congruenceAnalysisTreeItemValue.getItemType()).reset();

					this.loadCongruenceAnalysisPointPair(congruenceAnalysisTreeItemValue, selectedCongruenceAnalysisItemValuesArray);
					this.loadStrainParameterRestrictions(congruenceAnalysisTreeItemValue, selectedCongruenceAnalysisItemValuesArray);
					this.loadStrainParameters(congruenceAnalysisTreeItemValue, selectedCongruenceAnalysisItemValuesArray);
				}

				break;

			case TreeItemType.InnerEnum.REFERENCE_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.REFERENCE_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.REFERENCE_POINT_3D_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
			case TreeItemType.InnerEnum.DATUM_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.DATUM_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.DATUM_POINT_3D_LEAF:
			case TreeItemType.InnerEnum.NEW_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.NEW_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.NEW_POINT_3D_LEAF:

				if (itemValue is PointTreeItemValue)
				{
					PointTreeItemValue pointItemValue = (PointTreeItemValue)itemValue;
					PointTreeItemValue[] selectedPointItemValuesArray = null;
					ISet<PointTreeItemValue> selectedPointItemValues = new LinkedHashSet<PointTreeItemValue>();
					selectedPointItemValues.Add(pointItemValue);

					if (selectedTreeItemValues != null)
					{
						foreach (TreeItemValue selectedItem in selectedTreeItemValues)
						{
							if (selectedItem is PointTreeItemValue)
							{
								selectedPointItemValues.Add((PointTreeItemValue)selectedItem);
							}
						}
					}
					selectedPointItemValuesArray = selectedPointItemValues.ToArray();
					this.loadPoints(pointItemValue, selectedPointItemValuesArray);

					//UIPointPropertiesPaneBuilder.getInstance().getPointPropertiesPane(pointItemValue.getItemType()).reset();

					switch (treeItemType.innerEnumValue)
					{
					case TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
					case TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
					case TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:

						this.loadUncertainties(pointItemValue, selectedPointItemValuesArray);
						this.loadVarianceComponents(pointItemValue, selectedPointItemValuesArray);

						break;
					default:
						break;
					}
				}

				break;

			case TreeItemType.InnerEnum.LEVELING_LEAF:
			case TreeItemType.InnerEnum.DIRECTION_LEAF:
			case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
			case TreeItemType.InnerEnum.GNSS_1D_LEAF:
			case TreeItemType.InnerEnum.GNSS_2D_LEAF:
			case TreeItemType.InnerEnum.GNSS_3D_LEAF:

				if (itemValue is ObservationTreeItemValue)
				{
					ObservationTreeItemValue observationItemValue = (ObservationTreeItemValue)itemValue;
					ObservationTreeItemValue[] selectedObservationItemValuesArray = null;
					ISet<ObservationTreeItemValue> selectedObservationItemValues = new LinkedHashSet<ObservationTreeItemValue>();
					selectedObservationItemValues.Add(observationItemValue);

					if (selectedTreeItemValues != null)
					{
						foreach (TreeItemValue selectedItem in selectedTreeItemValues)
						{
							if (selectedItem is ObservationTreeItemValue)
							{
								selectedObservationItemValues.Add((ObservationTreeItemValue)selectedItem);
							}
						}
					}
					selectedObservationItemValuesArray = selectedObservationItemValues.ToArray();

					switch (treeItemType.innerEnumValue)
					{

					case TreeItemType.InnerEnum.LEVELING_LEAF:
					case TreeItemType.InnerEnum.DIRECTION_LEAF:
					case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
					case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
					case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
						this.loadObservations(observationItemValue, selectedObservationItemValuesArray);
						break;
					case TreeItemType.InnerEnum.GNSS_1D_LEAF:
					case TreeItemType.InnerEnum.GNSS_2D_LEAF:
					case TreeItemType.InnerEnum.GNSS_3D_LEAF:
						this.loadGNSSObservations(observationItemValue, selectedObservationItemValuesArray);
						break;
					default:
						break;
					}

					//UIObservationPropertiesPaneBuilder.getInstance().getObservationPropertiesPane(observationItemValue.getItemType()).reset();
					this.loadUncertainties(observationItemValue, selectedObservationItemValuesArray);
					this.loadEpoch(observationItemValue, selectedObservationItemValuesArray);
					this.loadAdditionalParameters(observationItemValue, selectedObservationItemValuesArray);
					this.loadVarianceComponents(observationItemValue, selectedObservationItemValuesArray);
				}

				break;

			case TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_LEAF:
			case TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_LEAF:
				if (itemValue is VerticalDeflectionTreeItemValue)
				{
					VerticalDeflectionTreeItemValue verticalDeflectionTreeItemValue = (VerticalDeflectionTreeItemValue)itemValue;
					VerticalDeflectionTreeItemValue[] selectedVerticalDeflectionItemValuesArray = null;
					ISet<VerticalDeflectionTreeItemValue> selectedVerticalDeflectionTreeItemValues = new LinkedHashSet<VerticalDeflectionTreeItemValue>();
					selectedVerticalDeflectionTreeItemValues.Add(verticalDeflectionTreeItemValue);

					if (selectedTreeItemValues != null)
					{
						foreach (TreeItemValue selectedItem in selectedTreeItemValues)
						{
							if (selectedItem is VerticalDeflectionTreeItemValue)
							{
								selectedVerticalDeflectionTreeItemValues.Add((VerticalDeflectionTreeItemValue)selectedItem);
							}
						}
					}
					selectedVerticalDeflectionItemValuesArray = selectedVerticalDeflectionTreeItemValues.ToArray();

					this.loadVerticalDeflections(verticalDeflectionTreeItemValue, selectedVerticalDeflectionItemValuesArray);

					if (treeItemType == TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_LEAF)
					{
						this.loadUncertainties(verticalDeflectionTreeItemValue, selectedVerticalDeflectionItemValuesArray);
						this.loadVarianceComponents(verticalDeflectionTreeItemValue, selectedVerticalDeflectionItemValuesArray);
					}
				}
			break;

			default:
				break;

			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadVarianceComponents(org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue pointItemValue, org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue... selectedPointItemValues) throws java.sql.SQLException
		private void loadVarianceComponents(PointTreeItemValue pointItemValue, params PointTreeItemValue[] selectedPointItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			PointType pointType = TreeItemType.getPointTypeByTreeItemType(pointItemValue.ItemType);
			int dimension = pointItemValue.Dimension;

			if (pointType != PointType.STOCHASTIC_POINT)
			{
				return;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedPointItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			UIVarianceComponentTableBuilder tableBuilder = UIVarianceComponentTableBuilder.Instance;
			TableView<VarianceComponentRow> table = tableBuilder.getTable(UIVarianceComponentTableBuilder.VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			IList<VarianceComponentRow> tableModel = FXCollections.observableArrayList();

			string sumRedundancy;
			VarianceComponentType varianceComponentType = null;
			switch (dimension)
			{
			case 2:
				varianceComponentType = VarianceComponentType.STOCHASTIC_POINT_2D_COMPONENT;
				sumRedundancy = "SUM(\"redundancy_x\" + \"redundancy_y\")";
				break;
			case 3:
				varianceComponentType = VarianceComponentType.STOCHASTIC_POINT_3D_COMPONENT;
				sumRedundancy = "SUM(\"redundancy_x\" + \"redundancy_y\" + \"redundancy_z\")";
				break;
			default:
				varianceComponentType = VarianceComponentType.STOCHASTIC_POINT_1D_COMPONENT;
				sumRedundancy = "SUM(\"redundancy_z\")";
				break;
			}

			string sql = "SELECT " + "\"name\", \"number_of_observations\", \"omega\", \"redundancy\", \"sigma2apost\", \"order\" FROM ( " + "SELECT " + "NULL AS \"name\", COUNT(\"id\") * ? AS \"number_of_observations\", " + "SUM(\"omega\") AS \"omega\", " + sumRedundancy + " AS \"redundancy\", " + "CASEWHEN(" + sumRedundancy + " > 0, SUM(\"omega\")/" + sumRedundancy + ", 0) AS \"sigma2apost\", " + "-1 AS \"order\" " + "FROM \"PointAposteriori\" " + "JOIN \"PointApriori\" ON \"PointAposteriori\".\"id\" = \"PointApriori\".\"id\" " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" " + "WHERE \"PointGroup\".\"type\" = ? AND " + "\"PointApriori\".\"enable\" = TRUE AND " + "\"PointGroup\".\"enable\" = TRUE AND " + "\"group_id\" IN (" + inArrayValues + ") " + "UNION ALL " + "SELECT " + "\"PointGroup\".\"name\" AS \"name\", COUNT(\"id\") * ? AS \"number_of_observations\", " + "SUM(\"omega\") AS \"omega\", " + sumRedundancy + " AS \"redundancy\", " + "CASEWHEN(" + sumRedundancy + " > 0, SUM(\"omega\")/" + sumRedundancy + ", 0) AS \"sigma2apost\", " + "\"order\" " + "FROM \"PointAposteriori\" " + "JOIN \"PointApriori\" ON \"PointAposteriori\".\"id\" = \"PointApriori\".\"id\" " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" " + "WHERE \"PointGroup\".\"type\" = ? AND " + "\"PointApriori\".\"enable\" = TRUE AND " + "\"PointGroup\".\"enable\" = TRUE AND " + "\"group_id\" IN (" + inArrayValues + ") " + "GROUP BY \"group_id\", \"PointGroup\".\"name\", \"order\" " + ") AS \"UnionTable\" " + "ORDER BY \"order\" ASC";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			int idx = 1;
			stmt.setInt(idx++, dimension);
			stmt.setInt(idx++, pointType.getId());
			for (int i = 0; i < selectedPointItemValues.Length; i++)
			{
				stmt.setInt(idx++, selectedPointItemValues[i].GroupId);
			}

			stmt.setInt(idx++, dimension);
			stmt.setInt(idx++, pointType.getId());
			for (int i = 0; i < selectedPointItemValues.Length; i++)
			{
				stmt.setInt(idx++, selectedPointItemValues[i].GroupId);
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				string name = rs.getString("name");
				if (rs.wasNull())
				{
					name = UIVarianceComponentTableBuilder.getVarianceComponentTypeLabel(varianceComponentType);
				}

				int numberOfObservations = rs.getInt("number_of_observations");
				double redundancy = rs.getDouble("redundancy");

				if (numberOfObservations == 0 || redundancy == 0)
				{
					continue;
				}

				double omega = rs.getDouble("omega");
				double sigma2apost = rs.getDouble("sigma2apost");

				VarianceComponentRow row = new VarianceComponentRow();

				row.Id = varianceComponentType.getId();
				row.Name = name;
				row.VarianceComponentType = varianceComponentType;
				row.NumberOfObservations = numberOfObservations;
				row.Redundancy = redundancy;
				row.Omega = omega;
				row.Sigma2aposteriori = sigma2apost;

				tableModel.Add(row);
			}

			if (tableModel.Count == 2) // the group and the total vce are identical
			{
				tableModel.RemoveAt(0);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadVarianceComponents(org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue verticalDeflectionItemValue, org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue... selectedVerticalDeflectionItemValues) throws java.sql.SQLException
		private void loadVarianceComponents(VerticalDeflectionTreeItemValue verticalDeflectionItemValue, params VerticalDeflectionTreeItemValue[] selectedVerticalDeflectionItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			VerticalDeflectionType verticalDeflectionType = TreeItemType.getVerticalDeflectionTypeByTreeItemType(verticalDeflectionItemValue.ItemType);
			if (verticalDeflectionType != VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION)
			{
				return;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedVerticalDeflectionItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			UIVarianceComponentTableBuilder tableBuilder = UIVarianceComponentTableBuilder.Instance;
			TableView<VarianceComponentRow> table = tableBuilder.getTable(UIVarianceComponentTableBuilder.VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			IList<VarianceComponentRow> tableModel = FXCollections.observableArrayList();

			VarianceComponentType varianceComponentType = VarianceComponentType.STOCHASTIC_DEFLECTION_COMPONENT;
			int dimension = verticalDeflectionItemValue.Dimension;

			string sql = "SELECT " + "\"name\", \"number_of_observations\", \"omega\", \"redundancy\", \"sigma2apost\", \"order\" FROM ( " + "SELECT " + "NULL AS \"name\", COUNT(\"id\") * ? AS \"number_of_observations\", " + "SUM(\"omega\") AS \"omega\", SUM(\"redundancy_x\" + \"redundancy_y\") AS \"redundancy\", " + "CASEWHEN(SUM(\"redundancy_x\" + \"redundancy_y\") > 0, SUM(\"omega\")/SUM(\"redundancy_x\" + \"redundancy_y\"), 0) AS \"sigma2apost\", " + "-1 AS \"order\" " + "FROM \"VerticalDeflectionAposteriori\" " + "JOIN \"VerticalDeflectionApriori\" ON \"VerticalDeflectionAposteriori\".\"id\" = \"VerticalDeflectionApriori\".\"id\" " + "JOIN \"VerticalDeflectionGroup\" ON \"VerticalDeflectionApriori\".\"group_id\" = \"VerticalDeflectionGroup\".\"id\" " + "WHERE \"VerticalDeflectionGroup\".\"type\" = ? AND " + "\"VerticalDeflectionApriori\".\"enable\" = TRUE AND " + "\"VerticalDeflectionGroup\".\"enable\" = TRUE AND " + "\"group_id\" IN (" + inArrayValues + ") " + "UNION ALL " + "SELECT " + "\"VerticalDeflectionGroup\".\"name\" AS \"name\", COUNT(\"id\") * ? AS \"number_of_observations\", " + "SUM(\"omega\") AS \"omega\", SUM(\"redundancy_x\" + \"redundancy_y\") AS \"redundancy\", " + "CASEWHEN(SUM(\"redundancy_x\" + \"redundancy_y\") > 0, SUM(\"omega\")/SUM(\"redundancy_x\" + \"redundancy_y\"), 0) AS \"sigma2apost\", " + "\"order\" " + "FROM \"VerticalDeflectionAposteriori\" " + "JOIN \"VerticalDeflectionApriori\" ON \"VerticalDeflectionAposteriori\".\"id\" = \"VerticalDeflectionApriori\".\"id\" " + "JOIN \"VerticalDeflectionGroup\" ON \"VerticalDeflectionApriori\".\"group_id\" = \"VerticalDeflectionGroup\".\"id\" " + "WHERE \"VerticalDeflectionGroup\".\"type\" = ? AND " + "\"VerticalDeflectionApriori\".\"enable\" = TRUE AND " + "\"VerticalDeflectionGroup\".\"enable\" = TRUE AND " + "\"group_id\" IN (" + inArrayValues + ") " + "GROUP BY \"group_id\", \"VerticalDeflectionGroup\".\"name\", \"order\" " + ") AS \"UnionTable\" " + "ORDER BY \"order\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			int idx = 1;
			stmt.setInt(idx++, dimension);
			stmt.setInt(idx++, verticalDeflectionType.getId());
			for (int i = 0; i < selectedVerticalDeflectionItemValues.Length; i++)
			{
				stmt.setInt(idx++, selectedVerticalDeflectionItemValues[i].GroupId);
			}

			stmt.setInt(idx++, dimension);
			stmt.setInt(idx++, verticalDeflectionType.getId());
			for (int i = 0; i < selectedVerticalDeflectionItemValues.Length; i++)
			{
				stmt.setInt(idx++, selectedVerticalDeflectionItemValues[i].GroupId);
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				string name = rs.getString("name");
				if (rs.wasNull())
				{
					name = UIVarianceComponentTableBuilder.getVarianceComponentTypeLabel(varianceComponentType);
				}

				int numberOfObservations = rs.getInt("number_of_observations");
				double redundancy = rs.getDouble("redundancy");

				if (numberOfObservations == 0 || redundancy == 0)
				{
					continue;
				}

				double omega = rs.getDouble("omega");
				double sigma2apost = rs.getDouble("sigma2apost");

				VarianceComponentRow row = new VarianceComponentRow();
				row.Id = varianceComponentType.getId();
				row.Name = name;
				row.VarianceComponentType = varianceComponentType;
				row.NumberOfObservations = numberOfObservations;
				row.Redundancy = redundancy;
				row.Omega = omega;
				row.Sigma2aposteriori = sigma2apost;

				tableModel.Add(row);
			}

			if (tableModel.Count == 2) // the group and the total vce are identical
			{
				tableModel.RemoveAt(0);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadVarianceComponents(org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue observationItemValue, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedObservationItemValues) throws java.sql.SQLException
		private void loadVarianceComponents(ObservationTreeItemValue observationItemValue, params ObservationTreeItemValue[] selectedObservationItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			ObservationType observationType = TreeItemType.getObservationTypeByTreeItemType(observationItemValue.ItemType);
			int dimension = observationItemValue.Dimension;
			if (observationType == null)
			{
				return;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedObservationItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			UIVarianceComponentTableBuilder tableBuilder = UIVarianceComponentTableBuilder.Instance;
			TableView<VarianceComponentRow> table = tableBuilder.getTable(UIVarianceComponentTableBuilder.VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			IList<VarianceComponentRow> tableModel = FXCollections.observableArrayList();

			string tableApriori = "ObservationApriori";
			string tableAposteriori = "ObservationAposteriori";
			string sumRedundancy = "SUM(\"redundancy\")";

			VarianceComponentType varianceComponentType = null;
			switch (observationType.innerEnumValue)
			{
			case ObservationType.InnerEnum.LEVELING:
				varianceComponentType = VarianceComponentType.LEVELING_COMPONENT;
				break;
			case ObservationType.InnerEnum.DIRECTION:
				varianceComponentType = VarianceComponentType.DIRECTION_COMPONENT;
				break;
			case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				varianceComponentType = VarianceComponentType.HORIZONTAL_DISTANCE_COMPONENT;
				break;
			case ObservationType.InnerEnum.SLOPE_DISTANCE:
				varianceComponentType = VarianceComponentType.SLOPE_DISTANCE_COMPONENT;
				break;
			case ObservationType.InnerEnum.ZENITH_ANGLE:
				varianceComponentType = VarianceComponentType.ZENITH_ANGLE_COMPONENT;
				break;
			case ObservationType.InnerEnum.GNSS1D:
				varianceComponentType = VarianceComponentType.GNSS1D_COMPONENT;
				tableApriori = "GNSSObservationApriori";
				tableAposteriori = "GNSSObservationAposteriori";
				sumRedundancy = "SUM(\"redundancy_z\")";
				break;
			case ObservationType.InnerEnum.GNSS2D:
				varianceComponentType = VarianceComponentType.GNSS2D_COMPONENT;
				tableApriori = "GNSSObservationApriori";
				tableAposteriori = "GNSSObservationAposteriori";
				sumRedundancy = "SUM(\"redundancy_x\" + \"redundancy_y\")";
				break;
			case ObservationType.InnerEnum.GNSS3D:
				varianceComponentType = VarianceComponentType.GNSS3D_COMPONENT;
				tableApriori = "GNSSObservationApriori";
				tableAposteriori = "GNSSObservationAposteriori";
				sumRedundancy = "SUM(\"redundancy_x\" + \"redundancy_y\" + \"redundancy_z\")";
				break;
			}

			if (varianceComponentType == null)
			{
				return;
			}

			string sql = "SELECT " + "\"name\", \"number_of_observations\", \"omega\", \"redundancy\", \"sigma2apost\", \"order\" FROM ( " + "SELECT " + "NULL AS \"name\", COUNT(\"id\") * ? AS \"number_of_observations\", " + "SUM(\"omega\") AS \"omega\", " + sumRedundancy + " AS \"redundancy\", " + "CASEWHEN(" + sumRedundancy + " > 0, SUM(\"omega\")/" + sumRedundancy + ", 0) AS \"sigma2apost\", " + "-1 AS \"order\" " + "FROM \"" + tableAposteriori + "\" " + "JOIN \"" + tableApriori + "\" ON \"" + tableAposteriori + "\".\"id\" = \"" + tableApriori + "\".\"id\" " + "JOIN \"ObservationGroup\" ON \"" + tableApriori + "\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationGroup\".\"type\" = ? AND " + "\"" + tableApriori + "\".\"enable\" = TRUE AND " + "\"ObservationGroup\".\"enable\" = TRUE AND " + "\"group_id\" IN (" + inArrayValues + ") " + "UNION ALL " + "SELECT " + "\"name\", COUNT(\"id\") * ? AS \"number_of_observations\", " + "SUM(\"omega\") AS \"omega\", " + sumRedundancy + " AS \"redundancy\", " + "CASEWHEN(" + sumRedundancy + " > 0, SUM(\"omega\")/" + sumRedundancy + ", 0) AS \"sigma2apost\", " + "\"order\" " + "FROM \"" + tableAposteriori + "\" " + "JOIN \"" + tableApriori + "\" ON \"" + tableAposteriori + "\".\"id\" = \"" + tableApriori + "\".\"id\" " + "JOIN \"ObservationGroup\" ON \"" + tableApriori + "\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationGroup\".\"type\" = ? AND " + "\"" + tableApriori + "\".\"enable\" = TRUE AND " + "\"ObservationGroup\".\"enable\" = TRUE AND " + "\"group_id\" IN (" + inArrayValues + ") " + "GROUP BY \"group_id\", \"name\", \"order\" " + ") AS \"UnionTable\" " + "ORDER BY \"order\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			int idx = 1;
			stmt.setInt(idx++, dimension);
			stmt.setInt(idx++, observationType.getId());
			for (int i = 0; i < selectedObservationItemValues.Length; i++)
			{
				stmt.setInt(idx++, selectedObservationItemValues[i].GroupId);
			}

			stmt.setInt(idx++, dimension);
			stmt.setInt(idx++, observationType.getId());
			for (int i = 0; i < selectedObservationItemValues.Length; i++)
			{
				stmt.setInt(idx++, selectedObservationItemValues[i].GroupId);
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				string name = rs.getString("name");
				if (rs.wasNull())
				{
					name = UIVarianceComponentTableBuilder.getVarianceComponentTypeLabel(varianceComponentType);
				}

				int numberOfObservations = rs.getInt("number_of_observations");
				double redundancy = rs.getDouble("redundancy");

				if (numberOfObservations == 0 || redundancy == 0)
				{
					continue;
				}

				double omega = rs.getDouble("omega");
				double sigma2apost = rs.getDouble("sigma2apost");

				VarianceComponentRow row = new VarianceComponentRow();

				row.Id = varianceComponentType.getId();
				row.Name = name;
				row.VarianceComponentType = varianceComponentType;
				row.NumberOfObservations = numberOfObservations;
				row.Redundancy = redundancy;
				row.Omega = omega;
				row.Sigma2aposteriori = sigma2apost;

				tableModel.Add(row);
			}

			if (tableModel.Count == 2) // the group and the total vce are identical
			{
				tableModel.RemoveAt(0);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadVarianceComponents() throws java.sql.SQLException
		private void loadVarianceComponents()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UIVarianceComponentTableBuilder tableBuilder = UIVarianceComponentTableBuilder.Instance;
			TableView<VarianceComponentRow> table = tableBuilder.getTable(UIVarianceComponentTableBuilder.VarianceComponentDisplayType.OVERALL_COMPONENTS);
			IList<VarianceComponentRow> tableModel = FXCollections.observableArrayList();

			string sql = "SELECT " + "\"type\",\"redundancy\",\"omega\",\"sigma2apost\",\"number_of_observations\", \"quantile\", \"sigma2apost\" > \"quantile\" AS \"significant\" " + "FROM \"VarianceComponent\" " + "JOIN \"TestStatistic\" ON \"VarianceComponent\".\"redundancy\" = \"TestStatistic\".\"d1\" " + "WHERE \"redundancy\" > 0 " + "AND \"d2\" + 1 = \"d2\" " + "ORDER BY \"type\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				VarianceComponentType varianceComponentType = VarianceComponentType.getEnumByValue(rs.getInt("type"));

				if (varianceComponentType == null)
				{
					continue;
				}

				int numberOfObservations = rs.getInt("number_of_observations");
				double redundancy = rs.getDouble("redundancy");

				double omega = rs.getDouble("omega");
				double sigma2apost = rs.getDouble("sigma2apost");

				bool significant = rs.getBoolean("significant");

				VarianceComponentRow row = new VarianceComponentRow();

				row.Id = varianceComponentType.getId();
				row.VarianceComponentType = varianceComponentType;
				row.NumberOfObservations = numberOfObservations;
				row.Redundancy = redundancy;
				row.Omega = omega;
				row.Sigma2aposteriori = sigma2apost;
				row.Significant = significant;

				tableModel.Add(row);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadPrincipalComponents() throws java.sql.SQLException
		private void loadPrincipalComponents()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UIPrincipalComponentTableBuilder tableBuilder = UIPrincipalComponentTableBuilder.Instance;
			TableView<PrincipalComponentRow> table = tableBuilder.Table;
			IList<PrincipalComponentRow> tableModel = FXCollections.observableArrayList();

			string sql = "SELECT " + "\"index\", SQRT(ABS(\"value\")) AS \"value\", \"ratio\" " + "FROM \"PrincipalComponent\" " + "ORDER BY \"index\" DESC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			int id = 1;
			while (rs.next())
			{
				int index = rs.getInt("index");
				double value = rs.getDouble("value");
				double ratio = rs.getDouble("ratio");

				PrincipalComponentRow row = new PrincipalComponentRow();

				row.Id = id++;
				row.Index = index;
				row.Value = value;
				row.Ratio = ratio;

				tableModel.Add(row);
			}

			if (tableModel.Count > 0)
			{
				table.getItems().setAll(tableModel);
			}
			else
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadTestStatistics() throws java.sql.SQLException
		private void loadTestStatistics()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UITestStatisticTableBuilder tableBuilder = UITestStatisticTableBuilder.Instance;
			TableView<TestStatisticRow> table = tableBuilder.Table;
			IList<TestStatisticRow> tableModel = FXCollections.observableArrayList();

			string sql = "SELECT " + "ABS(\"d1\") AS \"d1\", ABS(\"d2\") AS \"d2\"," + "\"probability_value\",\"power_of_test\",\"quantile\"," + "\"non_centrality_parameter\",\"p_value\" " + "FROM \"TestStatistic\" " + "ORDER BY ABS(\"d1\") ASC, ABS(\"d2\") DESC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			int id = 1;
			while (rs.next())
			{
				double numeratorDegreeOfFreedom = rs.getDouble("d1");
				double denominatorDegreeOfFreedom = rs.getDouble("d2");

				double probabilityValue = rs.getDouble("probability_value");
				double powerOfTest = rs.getDouble("power_of_test");

				double quantile = rs.getDouble("quantile");
				double pValue = rs.getDouble("p_value");

				double noncentralityParameter = rs.getDouble("non_centrality_parameter");


				TestStatisticRow row = new TestStatisticRow();

				row.Id = id++;
				row.NumeratorDegreeOfFreedom = numeratorDegreeOfFreedom;
				row.DenominatorDegreeOfFreedom = denominatorDegreeOfFreedom;
				row.ProbabilityValue = probabilityValue;
				row.PowerOfTest = powerOfTest;
				row.Quantile = quantile;
				row.PValue = pValue;
				row.NoncentralityParameter = noncentralityParameter;

				tableModel.Add(row);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadAdditionalParameters(org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue observationItemValue, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedObservationItemValues) throws java.sql.SQLException
		private void loadAdditionalParameters(ObservationTreeItemValue observationItemValue, params ObservationTreeItemValue[] selectedObservationItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UIObservationPropertiesPaneBuilder propertiesPaneBuilder = UIObservationPropertiesPaneBuilder.Instance;
			UIObservationPropertiesPane propertiesPane = propertiesPaneBuilder.getObservationPropertiesPane(observationItemValue.ItemType);
			propertiesPane.setTreeItemValue(observationItemValue.Name, selectedObservationItemValues);

			ParameterType[] parameterTypes = ObservationTreeItemValue.getParameterTypes(observationItemValue.ItemType);
			StringBuilder inTypeArrayValues = new StringBuilder("?");
			for (int i = 1; i < parameterTypes.Length; i++)
			{
				inTypeArrayValues.Append(",?");
			}

			StringBuilder inGroupArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedObservationItemValues.Length; i++)
			{
				inGroupArrayValues.Append(",?");
			}

			string sqlAdditionalParameter = "SELECT \"AdditionalParameterApriori\".\"id\" AS \"id\", \"enable\", \"group_id\", \"type\", " + "\"value_0\", \"value\", \"sigma\", \"t_prio\", \"t_post\", \"p_prio\", \"p_post\", \"confidence\", \"gross_error\", \"minimal_detectable_bias\", \"significant\" " + "FROM \"AdditionalParameterApriori\" " + "LEFT JOIN \"AdditionalParameterAposteriori\" ON \"AdditionalParameterApriori\".\"id\" = \"AdditionalParameterAposteriori\".\"id\" " + "WHERE \"group_id\" IN (" + inGroupArrayValues + ") AND \"type\" IN (" + inTypeArrayValues + ") ORDER BY \"group_id\" ASC, \"type\" ASC";

			PreparedStatement stmtAdditionalParameter = this.dataBase.getPreparedStatement(sqlAdditionalParameter);
			int idx = 1;
			for (int i = 0; i < selectedObservationItemValues.Length; i++)
			{
				stmtAdditionalParameter.setInt(idx++, selectedObservationItemValues[i].GroupId);
			}

			for (int i = 0; i < parameterTypes.Length; i++)
			{
				stmtAdditionalParameter.setInt(idx++, parameterTypes[i].getId());
			}

			string sqlIdentical = "SELECT " + "COUNT(\"value_0\") AS \"counter\" " + "FROM \"AdditionalParameterApriori\" " + "WHERE " + "\"group_id\" IN (" + inGroupArrayValues + ") AND " + "\"type\" = ? AND " + "\"enable\" = ? AND " + "CASEWHEN(?, 0, CASEWHEN(GREATEST(ABS(?), ABS(\"value_0\")) > 0, " + "ABS(\"value_0\" - ?) / GREATEST(ABS(?), ABS(\"value_0\")), 0)) < ?";

			idx = 1;
			PreparedStatement stmtIdentical = this.dataBase.getPreparedStatement(sqlIdentical);
			for (int i = 0; i < selectedObservationItemValues.Length; i++)
			{
				stmtIdentical.setInt(idx++, selectedObservationItemValues[i].GroupId);
			}

			UIAdditionalParameterTableBuilder tableBuilder = UIAdditionalParameterTableBuilder.Instance;
			TableView<AdditionalParameterRow> table = tableBuilder.Table;
			IList<AdditionalParameterRow> tableModel = FXCollections.observableArrayList();

			ResultSet rsAdditionalParameter = stmtAdditionalParameter.executeQuery();
			while (rsAdditionalParameter.next())
			{
				ParameterType paramType = null;
				int type = rsAdditionalParameter.getInt("type");
				if (rsAdditionalParameter.wasNull() || (paramType = ParameterType.getEnumByValue(type)) == null)
				{
					continue;
				}

				int paramId = rsAdditionalParameter.getInt("id");
				int groupId = rsAdditionalParameter.getInt("group_id");
				double value0 = rsAdditionalParameter.getDouble("value_0");
				bool enable = rsAdditionalParameter.getBoolean("enable");

				// Settings/Properties of selected Item
				if (observationItemValue.GroupId == groupId)
				{
					bool isIdenticalGroupSetting = false;
					stmtIdentical.setInt(idx, paramType.getId());
					stmtIdentical.setBoolean(idx + 1, enable);
					stmtIdentical.setBoolean(idx + 2, enable);
					stmtIdentical.setDouble(idx + 3, value0);
					stmtIdentical.setDouble(idx + 4, value0);
					stmtIdentical.setDouble(idx + 5, value0);
					stmtIdentical.setDouble(idx + 6, EQUAL_VALUE_TRESHOLD);

					ResultSet rsIdentical = stmtIdentical.executeQuery();
					if (rsIdentical.next())
					{
						int cnt = rsIdentical.getInt("counter");
						isIdenticalGroupSetting = cnt == selectedObservationItemValues.Length;
					}
					propertiesPane.setAdditionalParameter(paramType, value0, enable, !isIdenticalGroupSetting);
				}


				// Result of all selected Items
				if (enable)
				{
					AdditionalParameterRow row = new AdditionalParameterRow();
					row.Id = paramId;
					row.ParameterType = paramType;
					double value = rsAdditionalParameter.getDouble("value");
					if (!rsAdditionalParameter.wasNull())
					{
						row.ValueAposteriori = value;
					}
					else
					{
						continue;
					}

					value = rsAdditionalParameter.getDouble("sigma");
					if (!rsAdditionalParameter.wasNull())
					{
						row.SigmaAposteriori = value;
					}
					else
					{
						continue;
					}

					value = rsAdditionalParameter.getDouble("confidence");
					if (!rsAdditionalParameter.wasNull())
					{
						row.Confidence = value;
					}
					else
					{
						continue;
					}

					value = rsAdditionalParameter.getDouble("gross_error");
					if (!rsAdditionalParameter.wasNull())
					{
						row.GrossError = value;
					}
					else
					{
						continue;
					}

					value = rsAdditionalParameter.getDouble("minimal_detectable_bias");
					if (!rsAdditionalParameter.wasNull())
					{
						row.MinimalDetectableBias = value;
					}
					else
					{
						continue;
					}

					value = rsAdditionalParameter.getDouble("p_prio");
					if (!rsAdditionalParameter.wasNull())
					{
						row.PValueApriori = value;
					}
					else
					{
						continue;
					}

					value = rsAdditionalParameter.getDouble("p_post");
					if (!rsAdditionalParameter.wasNull())
					{
						row.PValueAposteriori = value;
					}
					else
					{
						continue;
					}

					value = rsAdditionalParameter.getDouble("t_prio");
					if (!rsAdditionalParameter.wasNull())
					{
						row.TestStatisticApriori = value;
					}
					else
					{
						continue;
					}

					value = rsAdditionalParameter.getDouble("t_post");
					if (!rsAdditionalParameter.wasNull())
					{
						row.TestStatisticAposteriori = value;
					}
					else
					{
						continue;
					}

					bool significant = rsAdditionalParameter.getBoolean("significant");
					row.Significant = !rsAdditionalParameter.wasNull() && significant == true;

					tableModel.Add(row);
				}
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadStrainParameterRestrictions(org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue congruenceAnalysisItemValue, org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue... selectedCongruenceAnalysisItemValues) throws java.sql.SQLException
		private void loadStrainParameterRestrictions(CongruenceAnalysisTreeItemValue congruenceAnalysisItemValue, params CongruenceAnalysisTreeItemValue[] selectedCongruenceAnalysisItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UICongruenceAnalysisPropertiesPaneBuilder propertiesPaneBuilder = UICongruenceAnalysisPropertiesPaneBuilder.Instance;
			UICongruenceAnalysisPropertiesPane propertiesPane = propertiesPaneBuilder.getCongruenceAnalysisPropertiesPane(congruenceAnalysisItemValue.ItemType);
			propertiesPane.setTreeItemValue(congruenceAnalysisItemValue.Name, selectedCongruenceAnalysisItemValues);

			RestrictionType[] restrictionTypes = CongruenceAnalysisTreeItemValue.getRestrictionTypes(congruenceAnalysisItemValue.ItemType);
			StringBuilder inTypeArrayValues = new StringBuilder("?");
			for (int i = 1; i < restrictionTypes.Length; i++)
			{
				inTypeArrayValues.Append(",?");
			}

			string sql = "SELECT " + "\"type\", \"enable\" " + "FROM \"CongruenceAnalysisStrainParameterRestriction\" " + "WHERE \"group_id\" = ? AND \"type\" IN (" + inTypeArrayValues + ")";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setInt(idx++, congruenceAnalysisItemValue.GroupId);

			for (int i = 0; i < restrictionTypes.Length; i++)
			{
				stmt.setInt(idx++, restrictionTypes[i].getId());
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				RestrictionType restrictionType = null;
				int type = rs.getInt("type");
				bool enable = rs.getBoolean("enable");
				if (rs.wasNull() || (restrictionType = RestrictionType.getEnumByValue(type)) == null)
				{
					continue;
				}

				propertiesPane.setStrainParameter(restrictionType, enable);
			}

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadStrainParameters(org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue congruenceAnalysisItemValue, org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue... selectedCongruenceAnalysisItemValues) throws java.sql.SQLException
		private void loadStrainParameters(CongruenceAnalysisTreeItemValue congruenceAnalysisItemValue, params CongruenceAnalysisTreeItemValue[] selectedCongruenceAnalysisItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UICongruenceAnalysisPropertiesPaneBuilder propertiesPaneBuilder = UICongruenceAnalysisPropertiesPaneBuilder.Instance;
			UICongruenceAnalysisPropertiesPane propertiesPane = propertiesPaneBuilder.getCongruenceAnalysisPropertiesPane(congruenceAnalysisItemValue.ItemType);
			propertiesPane.setTreeItemValue(congruenceAnalysisItemValue.Name, selectedCongruenceAnalysisItemValues);

			ParameterType[] parameterTypes = CongruenceAnalysisTreeItemValue.getParameterTypes(congruenceAnalysisItemValue.ItemType);
			StringBuilder inTypeArrayValues = new StringBuilder("?");
			for (int i = 1; i < parameterTypes.Length; i++)
			{
				inTypeArrayValues.Append(",?");
			}

			StringBuilder inGroupArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedCongruenceAnalysisItemValues.Length; i++)
			{
				inGroupArrayValues.Append(",?");
			}

			string sql = "SELECT " + "\"type\",\"value\",\"sigma\"," + "\"confidence\",\"gross_error\",\"minimal_detectable_bias\"," + "\"p_prio\",\"p_post\",\"t_prio\",\"t_post\",\"significant\" " + "FROM \"CongruenceAnalysisStrainParameterAposteriori\" " + "WHERE \"group_id\" IN (" + inGroupArrayValues + ") AND \"type\" IN (" + inTypeArrayValues + ") ORDER BY \"group_id\" ASC, \"type\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			for (int i = 0; i < selectedCongruenceAnalysisItemValues.Length; i++)
			{
				stmt.setInt(idx++, selectedCongruenceAnalysisItemValues[i].GroupId);
			}

			for (int i = 0; i < parameterTypes.Length; i++)
			{
				stmt.setInt(idx++, parameterTypes[i].getId());
			}

			UIAdditionalParameterTableBuilder tableBuilder = UIAdditionalParameterTableBuilder.Instance;
			TableView<AdditionalParameterRow> table = tableBuilder.Table;
			IList<AdditionalParameterRow> tableModel = FXCollections.observableArrayList();

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				ParameterType paramType = null;
				int type = rs.getInt("type");
				if (rs.wasNull() || (paramType = ParameterType.getEnumByValue(type)) == null)
				{
					continue;
				}

				// Result of all selected Items
				AdditionalParameterRow row = new AdditionalParameterRow();

				row.Id = -1; // Strain parameter hasn't an id
				row.ParameterType = paramType;

				double value = rs.getDouble("value");
				if (!rs.wasNull())
				{
					row.ValueAposteriori = value;
				}

				value = rs.getDouble("sigma");
				if (!rs.wasNull())
				{
					row.SigmaAposteriori = value > 0 ? value : 0.0;
				}

				value = rs.getDouble("confidence");
				if (!rs.wasNull())
				{
					row.Confidence = value;
				}

				value = rs.getDouble("gross_error");
				if (!rs.wasNull())
				{
					row.GrossError = value;
				}

				value = rs.getDouble("minimal_detectable_bias");
				if (!rs.wasNull())
				{
					row.MinimalDetectableBias = value;
				}

				value = rs.getDouble("p_prio");
				if (!rs.wasNull())
				{
					row.PValueApriori = value;
				}

				value = rs.getDouble("p_post");
				if (!rs.wasNull())
				{
					row.PValueAposteriori = value;
				}

				value = rs.getDouble("t_prio");
				if (!rs.wasNull())
				{
					row.TestStatisticApriori = value;
				}

				value = rs.getDouble("t_post");
				if (!rs.wasNull())
				{
					row.TestStatisticAposteriori = value;
				}

				bool significant = rs.getBoolean("significant");
				row.Significant = !rs.wasNull() && significant == true;
				tableModel.Add(row);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadEpoch(org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue observationItemValue, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedObservationItemValues) throws java.sql.SQLException
		private void loadEpoch(ObservationTreeItemValue observationItemValue, params ObservationTreeItemValue[] selectedObservationItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UIObservationPropertiesPaneBuilder propertiesPaneBuilder = UIObservationPropertiesPaneBuilder.Instance;
			UIObservationPropertiesPane propertiesPane = propertiesPaneBuilder.getObservationPropertiesPane(observationItemValue.ItemType);
			propertiesPane.setTreeItemValue(observationItemValue.Name, selectedObservationItemValues);

			bool referenceEpoch = true;
			string sql = "SELECT \"reference_epoch\" " + "FROM \"ObservationGroup\" " + "WHERE \"id\" = ?";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			stmt.setInt(1, observationItemValue.GroupId);

			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				referenceEpoch = rs.getBoolean("reference_epoch");
			}

			propertiesPane.ReferenceEpoch = referenceEpoch;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadUncertainties(org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue verticalDeflectionItemValue, org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue... selectedVerticalDeflectionItemValues) throws java.sql.SQLException
		private void loadUncertainties(VerticalDeflectionTreeItemValue verticalDeflectionItemValue, params VerticalDeflectionTreeItemValue[] selectedVerticalDeflectionItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UIVerticalDeflectionPropertiesPaneBuilder propertiesPaneBuilder = UIVerticalDeflectionPropertiesPaneBuilder.Instance;
			UIVerticalDeflectionPropertiesPane propertiesPane = propertiesPaneBuilder.getVerticalDeflectionPropertiesPane(verticalDeflectionItemValue.ItemType);
			propertiesPane.setTreeItemValue(verticalDeflectionItemValue.Name, selectedVerticalDeflectionItemValues);

			string sql = "SELECT \"type\", \"value\" " + "FROM \"VerticalDeflectionGroupUncertainty\" " + "WHERE \"group_id\" = ?";

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedVerticalDeflectionItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			string sqlIdentical = "SELECT " + "COUNT(\"value\") AS \"counter\" " + "FROM \"VerticalDeflectionGroupUncertainty\" " + "WHERE " + "\"group_id\" IN (" + inArrayValues + ") AND " + "\"VerticalDeflectionGroupUncertainty\".\"type\" = ? AND " + "CASEWHEN(GREATEST(?, \"value\") > 0, ABS(\"value\" - ?) / GREATEST(?, \"value\"), 0) < ?";

			PreparedStatement stmtIdentical = this.dataBase.getPreparedStatement(sqlIdentical);
			int idx = 1;
			for (int i = 0; i < selectedVerticalDeflectionItemValues.Length; i++)
			{
				stmtIdentical.setInt(idx++, selectedVerticalDeflectionItemValues[i].GroupId);
			}

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, verticalDeflectionItemValue.GroupId);

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				VerticalDeflectionGroupUncertaintyType type = VerticalDeflectionGroupUncertaintyType.getEnumByValue(rs.getInt("type"));
				if (type != null)
				{
					double value = rs.getDouble("value");
					bool isIdenticalGroupSetting = false;

					stmtIdentical.setInt(idx, type.getId());
					stmtIdentical.setDouble(idx + 1, value);
					stmtIdentical.setDouble(idx + 2, value);
					stmtIdentical.setDouble(idx + 3, value);
					stmtIdentical.setDouble(idx + 4, EQUAL_VALUE_TRESHOLD);

					ResultSet rsIdentical = stmtIdentical.executeQuery();
					if (rsIdentical.next())
					{
						int cnt = rsIdentical.getInt("counter");
						isIdenticalGroupSetting = cnt == selectedVerticalDeflectionItemValues.Length;
					}
					propertiesPane.setUncertainty(type, value, !isIdenticalGroupSetting);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadVerticalDeflections(org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue verticalDeflectionItemValue, org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue... selectedVerticalDeflectionItemValues) throws java.sql.SQLException
		private void loadVerticalDeflections(VerticalDeflectionTreeItemValue verticalDeflectionItemValue, params VerticalDeflectionTreeItemValue[] selectedVerticalDeflectionItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedVerticalDeflectionItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			UIVerticalDeflectionTableBuilder tableBuilder = UIVerticalDeflectionTableBuilder.Instance;
			TableView<VerticalDeflectionRow> table = tableBuilder.getTable(verticalDeflectionItemValue);
			IList<VerticalDeflectionRow> tableModel = FXCollections.observableArrayList();

			string sql = "SELECT " + "\"VerticalDeflectionApriori\".\"id\", \"name\", \"enable\", \"group_id\", \"type\", " + "\"x0\", \"y0\", " + "\"VerticalDeflectionApriori\".\"sigma_y0\", \"VerticalDeflectionApriori\".\"sigma_x0\", " + "\"y\", \"x\",  \"sigma_y\", \"sigma_x\", " + "\"confidence_major_axis\", \"confidence_minor_axis\", " + "\"residual_y\", \"residual_x\", " + "\"redundancy_y\", \"redundancy_x\", " + "\"gross_error_y\", \"gross_error_x\", " + "\"minimal_detectable_bias_y\", \"minimal_detectable_bias_x\", " + "\"maximum_tolerable_bias_y\", \"maximum_tolerable_bias_x\", " + "\"omega\", \"significant\", " + "\"t_prio\", \"t_post\", \"p_prio\", \"p_post\" " + "FROM \"VerticalDeflectionApriori\" " + "JOIN \"VerticalDeflectionGroup\" ON \"VerticalDeflectionApriori\".\"group_id\" = \"VerticalDeflectionGroup\".\"id\" " + "LEFT JOIN \"VerticalDeflectionAposteriori\" ON \"VerticalDeflectionApriori\".\"id\" = \"VerticalDeflectionAposteriori\".\"id\" " + "WHERE \"VerticalDeflectionGroup\".\"type\" = ? " + "AND \"VerticalDeflectionGroup\".\"id\" IN (" + inArrayValues + ") " + "ORDER BY \"VerticalDeflectionGroup\".\"order\" ASC, \"VerticalDeflectionGroup\".\"id\" ASC, \"VerticalDeflectionApriori\".\"id\" ASC";


			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			stmt.setInt(1, TreeItemType.getVerticalDeflectionTypeByTreeItemType(verticalDeflectionItemValue.ItemType).getId());

			for (int i = 0; i < selectedVerticalDeflectionItemValues.Length; i++)
			{
				stmt.setInt(i + 2, selectedVerticalDeflectionItemValues[i].GroupId);
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				VerticalDeflectionRow row = new VerticalDeflectionRow();

				// a-priori-Values
				row.Id = rs.getInt("id");
				row.GroupId = rs.getInt("group_id");
				row.Name = rs.getString("name");
				row.Enable = rs.getBoolean("enable");

				double value;
				value = rs.getDouble("x0");
				row.XApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("y0");
				row.YApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("sigma_x0");
				row.SigmaXapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_y0");
				row.SigmaYapriori = rs.wasNull() || value <= 0 ? null : value;


				// a-posteriori
				value = rs.getDouble("x");
				row.XAposteriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("y");
				row.YAposteriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("sigma_x");
				row.SigmaXaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				value = rs.getDouble("sigma_y");
				row.SigmaYaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				// Confidence
				value = rs.getDouble("confidence_major_axis");
				row.ConfidenceA = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("confidence_minor_axis");
				row.ConfidenceC = rs.wasNull() ? null : Math.Abs(value);

				// Residual
				value = rs.getDouble("residual_x");
				row.ResidualX = rs.wasNull() ? null : value;

				value = rs.getDouble("residual_y");
				row.ResidualY = rs.wasNull() ? null : value;

				// Redundancy
				value = rs.getDouble("redundancy_x");
				row.RedundancyX = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("redundancy_y");
				row.RedundancyY = rs.wasNull() ? null : Math.Abs(value);

				// Gross error
				value = rs.getDouble("gross_error_x");
				row.GrossErrorX = rs.wasNull() ? null : value;

				value = rs.getDouble("gross_error_y");
				row.GrossErrorY = rs.wasNull() ? null : value;

				// MDB
				value = rs.getDouble("minimal_detectable_bias_x");
				row.MinimalDetectableBiasX = rs.wasNull() ? null : value;

				value = rs.getDouble("minimal_detectable_bias_y");
				row.MinimalDetectableBiasY = rs.wasNull() ? null : value;

				// MTB
				value = rs.getDouble("maximum_tolerable_bias_x");
				row.MaximumTolerableBiasX = rs.wasNull() ? null : value;

				value = rs.getDouble("maximum_tolerable_bias_y");
				row.MaximumTolerableBiasY = rs.wasNull() ? null : value;

				// Statistics
				value = rs.getDouble("omega");
				row.Omega = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("p_prio");
				row.PValueApriori = rs.wasNull() ? null : value;

				value = rs.getDouble("p_post");
				row.PValueAposteriori = rs.wasNull() ? null : value;

				value = rs.getDouble("t_prio");
				row.TestStatisticApriori = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("t_post");
				row.TestStatisticAposteriori = rs.wasNull() ? null : Math.Abs(value);

				bool significant = rs.getBoolean("significant");
				row.Significant = !rs.wasNull() && significant == true;

				tableModel.Add(row);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadUncertainties(org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue observationItemValue, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedObservationItemValues) throws java.sql.SQLException
		private void loadUncertainties(ObservationTreeItemValue observationItemValue, params ObservationTreeItemValue[] selectedObservationItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UIObservationPropertiesPaneBuilder propertiesPaneBuilder = UIObservationPropertiesPaneBuilder.Instance;
			UIObservationPropertiesPane propertiesPane = propertiesPaneBuilder.getObservationPropertiesPane(observationItemValue.ItemType);
			propertiesPane.setTreeItemValue(observationItemValue.Name, selectedObservationItemValues);

			string sqlUncertainty = "SELECT \"type\", \"value\" " + "FROM \"ObservationGroupUncertainty\" " + "WHERE \"group_id\" = ?";

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedObservationItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			string sqlIdentical = "SELECT " + "COUNT(\"value\") AS \"counter\" " + "FROM \"ObservationGroupUncertainty\" " + "WHERE " + "\"group_id\" IN (" + inArrayValues + ") AND " + "\"ObservationGroupUncertainty\".\"type\" = ? AND " + "CASEWHEN(GREATEST(?, \"value\") > 0, ABS(\"value\" - ?) / GREATEST(?, \"value\"), 0) < ?";

			PreparedStatement stmtIdentical = this.dataBase.getPreparedStatement(sqlIdentical);
			int idx = 1;
			for (int i = 0; i < selectedObservationItemValues.Length; i++)
			{
				stmtIdentical.setInt(idx++, selectedObservationItemValues[i].GroupId);
			}

			PreparedStatement stmtUncertainty = this.dataBase.getPreparedStatement(sqlUncertainty);
			stmtUncertainty.setInt(1, observationItemValue.GroupId);
			ResultSet rsUncertainty = stmtUncertainty.executeQuery();
			while (rsUncertainty.next())
			{
				ObservationGroupUncertaintyType type = ObservationGroupUncertaintyType.getEnumByValue(rsUncertainty.getInt("type"));
				if (type != null)
				{
					double value = rsUncertainty.getDouble("value");
					bool isIdenticalGroupSetting = false;

					stmtIdentical.setInt(idx, type.getId());
					stmtIdentical.setDouble(idx + 1, value);
					stmtIdentical.setDouble(idx + 2, value);
					stmtIdentical.setDouble(idx + 3, value);
					stmtIdentical.setDouble(idx + 4, EQUAL_VALUE_TRESHOLD);

					ResultSet rsIdentical = stmtIdentical.executeQuery();
					if (rsIdentical.next())
					{
						int cnt = rsIdentical.getInt("counter");
						isIdenticalGroupSetting = cnt == selectedObservationItemValues.Length;
					}
					propertiesPane.setUncertainty(type, value, !isIdenticalGroupSetting);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadObservations(org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue observationItemValue, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedObservationItemValues) throws java.sql.SQLException
		private void loadObservations(ObservationTreeItemValue observationItemValue, params ObservationTreeItemValue[] selectedObservationItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedObservationItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			UITerrestrialObservationTableBuilder tableBuilder = UITerrestrialObservationTableBuilder.Instance;
			TableView<TerrestrialObservationRow> table = tableBuilder.getTable(observationItemValue);
			IList<TerrestrialObservationRow> tableModel = FXCollections.observableArrayList();
			string sql = "SELECT " + "\"ObservationApriori\".\"id\", \"group_id\", \"start_point_name\", \"end_point_name\", \"instrument_height\", \"reflector_height\", \"value_0\", \"distance_0\", \"ObservationApriori\".\"sigma_0\" AS \"sigma_0\", \"enable\", " + "\"ObservationAposteriori\".\"value\", \"sigma\", \"residual\", \"redundancy\", \"gross_error\", \"influence_on_position\", \"influence_on_network_distortion\", \"minimal_detectable_bias\", \"maximum_tolerable_bias\", \"omega\", \"t_prio\", \"t_post\", \"p_prio\", \"p_post\", \"significant\" " + "FROM \"ObservationApriori\" " + "INNER JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "LEFT JOIN \"ObservationAposteriori\" ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "WHERE \"ObservationGroup\".\"type\" = ? " + "AND \"ObservationGroup\".\"id\" IN (" + inArrayValues + ") " + "ORDER BY \"ObservationGroup\".\"order\" ASC, \"ObservationGroup\".\"id\" ASC, \"ObservationApriori\".\"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			stmt.setInt(1, TreeItemType.getObservationTypeByTreeItemType(observationItemValue.ItemType).getId());
			for (int i = 0; i < selectedObservationItemValues.Length; i++)
			{
				stmt.setInt(i + 2, selectedObservationItemValues[i].GroupId);
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				TerrestrialObservationRow row = new TerrestrialObservationRow();

				// Apriori-Values
				row.Id = rs.getInt("id");
				row.GroupId = rs.getInt("group_id");
				row.StartPointName = rs.getString("start_point_name");
				row.EndPointName = rs.getString("end_point_name");
				row.Enable = rs.getBoolean("enable");
				row.ValueApriori = rs.getDouble("value_0");

				double value = rs.getDouble("instrument_height");
				if (!rs.wasNull())
				{
					row.InstrumentHeight = value;
				}

				value = rs.getDouble("reflector_height");
				if (!rs.wasNull())
				{
					row.ReflectorHeight = value;
				}

				value = rs.getDouble("distance_0");
				if (!rs.wasNull())
				{
					row.DistanceApriori = value > 0 ? value : null;
				}

				value = rs.getDouble("sigma_0");
				if (!rs.wasNull())
				{
					row.SigmaApriori = value > 0 ? value : null;
				}

				// Aposterior-Values
				value = rs.getDouble("value");
				if (!rs.wasNull())
				{
					row.ValueAposteriori = value;
				}

				value = rs.getDouble("sigma");
				if (!rs.wasNull())
				{
					row.SigmaAposteriori = value > 0 ? value : 0.0;
				}

				value = rs.getDouble("residual");
				if (!rs.wasNull())
				{
					row.Residual = value;
				}

				value = rs.getDouble("redundancy");
				if (!rs.wasNull())
				{
					row.Redundancy = Math.Abs(value);
				}

				value = rs.getDouble("gross_error");
				if (!rs.wasNull())
				{
					row.GrossError = value;
				}

				value = rs.getDouble("influence_on_position");
				if (!rs.wasNull())
				{
					row.InfluenceOnPointPosition = value;
				}

				value = rs.getDouble("influence_on_network_distortion");
				if (!rs.wasNull())
				{
					row.InfluenceOnNetworkDistortion = value;
				}

				value = rs.getDouble("minimal_detectable_bias");
				if (!rs.wasNull())
				{
					row.MinimalDetectableBias = value;
				}

				value = rs.getDouble("maximum_tolerable_bias");
				if (!rs.wasNull())
				{
					row.MaximumTolerableBias = value;
				}

				value = rs.getDouble("omega");
				if (!rs.wasNull())
				{
					row.Omega = value;
				}

				value = rs.getDouble("p_prio");
				if (!rs.wasNull())
				{
					row.PValueApriori = value;
				}

				value = rs.getDouble("p_post");
				if (!rs.wasNull())
				{
					row.PValueAposteriori = value;
				}

				value = rs.getDouble("t_prio");
				if (!rs.wasNull())
				{
					row.TestStatisticApriori = value;
				}

				value = rs.getDouble("t_post");
				if (!rs.wasNull())
				{
					row.TestStatisticAposteriori = value;
				}

				bool significant = rs.getBoolean("significant");
				if (!rs.wasNull())
				{
					row.Significant = significant;
				}

				tableModel.Add(row);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadGNSSObservations(org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue observationGNSSItemValue, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedGNSSObservationItemValues) throws java.sql.SQLException
		private void loadGNSSObservations(ObservationTreeItemValue observationGNSSItemValue, params ObservationTreeItemValue[] selectedGNSSObservationItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedGNSSObservationItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			UIGNSSObservationTableBuilder tableBuilder = UIGNSSObservationTableBuilder.Instance;
			TableView<GNSSObservationRow> table = tableBuilder.getTable(observationGNSSItemValue);
			IList<GNSSObservationRow> tableModel = FXCollections.observableArrayList();
			string sql = "SELECT " + "\"GNSSObservationApriori\".\"id\", \"group_id\", \"start_point_name\", \"end_point_name\", \"y0\", \"x0\", \"z0\", \"GNSSObservationApriori\".\"sigma_y0\" AS \"sigma_y0\", \"GNSSObservationApriori\".\"sigma_x0\" AS \"sigma_x0\", \"GNSSObservationApriori\".\"sigma_z0\" AS \"sigma_z0\", \"enable\", " + "\"y\", \"x\", \"z\",  \"sigma_y\", \"sigma_x\", \"sigma_z\", " + "\"residual_y\", \"residual_x\", \"residual_z\", " + "\"redundancy_y\", \"redundancy_x\", \"redundancy_z\", " + "\"gross_error_y\", \"gross_error_x\", \"gross_error_z\", " + "\"minimal_detectable_bias_y\", \"minimal_detectable_bias_x\", \"minimal_detectable_bias_z\", " + "\"maximum_tolerable_bias_y\", \"maximum_tolerable_bias_x\", \"maximum_tolerable_bias_z\", " + "\"influence_on_position_y\", \"influence_on_position_x\", \"influence_on_position_z\", \"influence_on_network_distortion\", " + "\"omega\", \"p_prio\", \"p_post\", \"t_prio\", \"t_post\", \"significant\" " + "FROM \"GNSSObservationApriori\" " + "INNER JOIN \"ObservationGroup\" ON \"GNSSObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "LEFT JOIN \"GNSSObservationAposteriori\" ON \"GNSSObservationApriori\".\"id\" = \"GNSSObservationAposteriori\".\"id\" " + "WHERE \"ObservationGroup\".\"type\" = ? " + "AND \"ObservationGroup\".\"id\" IN (" + inArrayValues + ") " + "ORDER BY \"ObservationGroup\".\"order\" ASC, \"ObservationGroup\".\"id\" ASC, \"GNSSObservationApriori\".\"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			stmt.setInt(1, TreeItemType.getObservationTypeByTreeItemType(observationGNSSItemValue.ItemType).getId());
			for (int i = 0; i < selectedGNSSObservationItemValues.Length; i++)
			{
				stmt.setInt(i + 2, selectedGNSSObservationItemValues[i].GroupId);
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				GNSSObservationRow row = new GNSSObservationRow();
				// Apriori-Values
				row.Id = rs.getInt("id");
				row.GroupId = rs.getInt("group_id");
				row.StartPointName = rs.getString("start_point_name");
				row.EndPointName = rs.getString("end_point_name");
				row.Enable = rs.getBoolean("enable");
				double value;
				value = rs.getDouble("x0");
				row.XApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("y0");
				row.YApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("z0");
				row.ZApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("sigma_x0");
				row.SigmaXapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_y0");
				row.SigmaYapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_z0");
				row.SigmaZapriori = rs.wasNull() || value <= 0 ? null : value;


				// Aposteriori
				value = rs.getDouble("x");
				row.XAposteriori = rs.wasNull() ? null : value;

				value = rs.getDouble("y");
				row.YAposteriori = rs.wasNull() ? null : value;

				value = rs.getDouble("z");
				row.ZAposteriori = rs.wasNull() ? null : value;

				value = rs.getDouble("sigma_x");
				row.SigmaXaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				value = rs.getDouble("sigma_y");
				row.SigmaYaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				value = rs.getDouble("sigma_z");
				row.SigmaZaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				// Residuals
				value = rs.getDouble("residual_x");
				row.ResidualX = rs.wasNull() ? null : value;

				value = rs.getDouble("residual_y");
				row.ResidualY = rs.wasNull() ? null : value;

				value = rs.getDouble("residual_z");
				row.ResidualZ = rs.wasNull() ? null : value;

				// Redundancy
				value = rs.getDouble("redundancy_x");
				row.RedundancyX = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("redundancy_y");
				row.RedundancyY = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("redundancy_z");
				row.RedundancyZ = rs.wasNull() ? null : Math.Abs(value);

				// Gross error
				value = rs.getDouble("gross_error_x");
				row.GrossErrorX = rs.wasNull() ? null : value;

				value = rs.getDouble("gross_error_y");
				row.GrossErrorY = rs.wasNull() ? null : value;

				value = rs.getDouble("gross_error_z");
				row.GrossErrorZ = rs.wasNull() ? null : value;

				// MDB
				value = rs.getDouble("minimal_detectable_bias_x");
				row.MinimalDetectableBiasX = rs.wasNull() ? null : value;

				value = rs.getDouble("minimal_detectable_bias_y");
				row.MinimalDetectableBiasY = rs.wasNull() ? null : value;

				value = rs.getDouble("minimal_detectable_bias_z");
				row.MinimalDetectableBiasZ = rs.wasNull() ? null : value;

				// MTB
				value = rs.getDouble("maximum_tolerable_bias_x");
				row.MaximumTolerableBiasX = rs.wasNull() ? null : value;

				value = rs.getDouble("maximum_tolerable_bias_y");
				row.MaximumTolerableBiasY = rs.wasNull() ? null : value;

				value = rs.getDouble("maximum_tolerable_bias_z");
				row.MaximumTolerableBiasZ = rs.wasNull() ? null : value;

				// EP + EFSPmax
				value = rs.getDouble("influence_on_position_x");
				row.InfluenceOnPointPositionX = rs.wasNull() ? null : value;

				value = rs.getDouble("influence_on_position_y");
				row.InfluenceOnPointPositionY = rs.wasNull() ? null : value;

				value = rs.getDouble("influence_on_position_z");
				row.InfluenceOnPointPositionZ = rs.wasNull() ? null : value;

				value = rs.getDouble("influence_on_network_distortion");
				row.InfluenceOnNetworkDistortion = rs.wasNull() ? null : value;

				// Statistics
				value = rs.getDouble("omega");
				row.Omega = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("p_prio");
				row.PValueApriori = rs.wasNull() ? null : value;

				value = rs.getDouble("p_post");
				row.PValueAposteriori = rs.wasNull() ? null : value;

				value = rs.getDouble("t_prio");
				row.TestStatisticApriori = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("t_post");
				row.TestStatisticAposteriori = rs.wasNull() ? null : Math.Abs(value);

				bool significantPoint = rs.getBoolean("significant");
				row.Significant = !rs.wasNull() && significantPoint == true;

				tableModel.Add(row);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadUncertainties(org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue pointItemValue, org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue... selectedPointItemValues) throws java.sql.SQLException
		private void loadUncertainties(PointTreeItemValue pointItemValue, params PointTreeItemValue[] selectedPointItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			UIPointPropertiesPaneBuilder propertiesPaneBuilder = UIPointPropertiesPaneBuilder.Instance;
			UIPointPropertiesPane propertiesPane = propertiesPaneBuilder.getPointPropertiesPane(pointItemValue.ItemType);
			propertiesPane.setTreeItemValue(pointItemValue.Name, selectedPointItemValues);

			string sqlUncertainty = "SELECT \"type\", \"value\" " + "FROM \"PointGroupUncertainty\" " + "WHERE \"group_id\" = ?";

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedPointItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			string sqlIdentical = "SELECT " + "COUNT(\"value\") AS \"counter\" " + "FROM \"PointGroupUncertainty\" " + "WHERE " + "\"group_id\" IN (" + inArrayValues + ") AND " + "\"PointGroupUncertainty\".\"type\" = ? AND " + "CASEWHEN(GREATEST(?, \"value\") > 0, ABS(\"value\" - ?) / GREATEST(?, \"value\"), 0) < ?";

			PreparedStatement stmtIdentical = this.dataBase.getPreparedStatement(sqlIdentical);
			int idx = 1;
			for (int i = 0; i < selectedPointItemValues.Length; i++)
			{
				stmtIdentical.setInt(idx++, selectedPointItemValues[i].GroupId);
			}

			PreparedStatement stmtUncertainty = this.dataBase.getPreparedStatement(sqlUncertainty);
			stmtUncertainty.setInt(1, pointItemValue.GroupId);

			ResultSet rsUncertainty = stmtUncertainty.executeQuery();
			while (rsUncertainty.next())
			{
				PointGroupUncertaintyType type = PointGroupUncertaintyType.getEnumByValue(rsUncertainty.getInt("type"));
				if (type != null)
				{
					double value = rsUncertainty.getDouble("value");
					bool isIdenticalGroupSetting = false;

					stmtIdentical.setInt(idx, type.getId());
					stmtIdentical.setDouble(idx + 1, value);
					stmtIdentical.setDouble(idx + 2, value);
					stmtIdentical.setDouble(idx + 3, value);
					stmtIdentical.setDouble(idx + 4, EQUAL_VALUE_TRESHOLD);

					ResultSet rsIdentical = stmtIdentical.executeQuery();
					if (rsIdentical.next())
					{
						int cnt = rsIdentical.getInt("counter");
						isIdenticalGroupSetting = cnt == selectedPointItemValues.Length;
					}
					propertiesPane.setUncertainty(type, value, !isIdenticalGroupSetting);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadPoints(org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue pointItemValue, org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue... selectedPointItemValues) throws java.sql.SQLException
		private void loadPoints(PointTreeItemValue pointItemValue, params PointTreeItemValue[] selectedPointItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedPointItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			UIPointTableBuilder tableBuilder = UIPointTableBuilder.Instance;
			TableView<PointRow> table = tableBuilder.getTable(pointItemValue);
			IList<PointRow> tableModel = FXCollections.observableArrayList();

			string sql = "SELECT " + "\"PointApriori\".\"id\", \"name\", \"code\", \"enable\"," + "\"x0\", \"y0\", \"z0\", " + "\"PointApriori\".\"sigma_y0\", \"PointApriori\".\"sigma_x0\", \"PointApriori\".\"sigma_z0\", " + "\"y\", \"x\", \"z\",  \"sigma_y\", \"sigma_x\", \"sigma_z\", " + "\"PointAposteriori\".\"confidence_major_axis\", \"PointAposteriori\".\"confidence_middle_axis\", \"PointAposteriori\".\"confidence_minor_axis\", \"confidence_alpha\", \"confidence_beta\", \"confidence_gamma\", " + "\"residual_y\", \"residual_x\", \"residual_z\", " + "\"redundancy_y\", \"redundancy_x\", \"redundancy_z\", " + "\"gross_error_y\", \"gross_error_x\", \"gross_error_z\", \"influence_on_position_y\", \"influence_on_position_x\", \"influence_on_position_z\", " + "\"influence_on_network_distortion\", " + "\"minimal_detectable_bias_y\", \"minimal_detectable_bias_x\", \"minimal_detectable_bias_z\", " + "\"maximum_tolerable_bias_y\", \"maximum_tolerable_bias_x\", \"maximum_tolerable_bias_z\", " + "\"first_principal_component_y\", \"first_principal_component_x\", \"first_principal_component_z\", " + "\"omega\", \"significant\", " + "\"t_prio\", \"t_post\", \"p_prio\", \"p_post\", " + "\"group_id\", \"type\", \"dimension\" " + "FROM \"PointApriori\" " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" " + "LEFT JOIN \"PointAposteriori\" ON \"PointApriori\".\"id\" = \"PointAposteriori\".\"id\" " + "WHERE \"PointGroup\".\"type\" = ? AND \"PointGroup\".\"dimension\" = ? " + "AND \"PointGroup\".\"id\" IN (" + inArrayValues + ") " + "ORDER BY \"PointGroup\".\"order\" ASC, \"PointGroup\".\"id\" ASC, \"PointApriori\".\"id\" ASC";


			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			stmt.setInt(1, TreeItemType.getPointTypeByTreeItemType(pointItemValue.ItemType).getId());
			stmt.setInt(2, pointItemValue.Dimension);

			for (int i = 0; i < selectedPointItemValues.Length; i++)
			{
				stmt.setInt(i + 3, selectedPointItemValues[i].GroupId);
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				PointRow row = new PointRow();
				// POINT

				// Apriori-Values
				row.Id = rs.getInt("id");
				row.GroupId = rs.getInt("group_id");
				row.Name = rs.getString("name");
				row.Code = rs.getString("code");
				row.Enable = rs.getBoolean("enable");

				double value;
				value = rs.getDouble("x0");
				row.XApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("y0");
				row.YApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("z0");
				row.ZApriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("sigma_x0");
				row.SigmaXapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_y0");
				row.SigmaYapriori = rs.wasNull() || value <= 0 ? null : value;

				value = rs.getDouble("sigma_z0");
				row.SigmaZapriori = rs.wasNull() || value <= 0 ? null : value;


				// Aposteriori
				value = rs.getDouble("x");
				row.XAposteriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("y");
				row.YAposteriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("z");
				row.ZAposteriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("sigma_x");
				row.SigmaXaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				value = rs.getDouble("sigma_y");
				row.SigmaYaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				value = rs.getDouble("sigma_z");
				row.SigmaZaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				// Confidence
				value = rs.getDouble("confidence_major_axis");
				row.ConfidenceA = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("confidence_middle_axis");
				row.ConfidenceB = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("confidence_minor_axis");
				row.ConfidenceC = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("confidence_alpha");
				row.ConfidenceAlpha = rs.wasNull() ? null : value;

				value = rs.getDouble("confidence_beta");
				row.ConfidenceBeta = rs.wasNull() ? null : value;

				value = rs.getDouble("confidence_gamma");
				row.ConfidenceGamma = rs.wasNull() ? null : value;

				// Residual
				value = rs.getDouble("residual_x");
				row.ResidualX = rs.wasNull() ? null : value;

				value = rs.getDouble("residual_y");
				row.ResidualY = rs.wasNull() ? null : value;

				value = rs.getDouble("residual_z");
				row.ResidualZ = rs.wasNull() ? null : value;

				// Redundancy
				value = rs.getDouble("redundancy_x");
				row.RedundancyX = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("redundancy_y");
				row.RedundancyY = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("redundancy_z");
				row.RedundancyZ = rs.wasNull() ? null : Math.Abs(value);

				// Gross error
				value = rs.getDouble("gross_error_x");
				row.GrossErrorX = rs.wasNull() ? null : value;

				value = rs.getDouble("gross_error_y");
				row.GrossErrorY = rs.wasNull() ? null : value;

				value = rs.getDouble("gross_error_z");
				row.GrossErrorZ = rs.wasNull() ? null : value;

				// MDB
				value = rs.getDouble("minimal_detectable_bias_x");
				row.MinimalDetectableBiasX = rs.wasNull() ? null : value;

				value = rs.getDouble("minimal_detectable_bias_y");
				row.MinimalDetectableBiasY = rs.wasNull() ? null : value;

				value = rs.getDouble("minimal_detectable_bias_z");
				row.MinimalDetectableBiasZ = rs.wasNull() ? null : value;

				// MTB
				value = rs.getDouble("maximum_tolerable_bias_x");
				row.MaximumTolerableBiasX = rs.wasNull() ? null : value;

				value = rs.getDouble("maximum_tolerable_bias_y");
				row.MaximumTolerableBiasY = rs.wasNull() ? null : value;

				value = rs.getDouble("maximum_tolerable_bias_z");
				row.MaximumTolerableBiasZ = rs.wasNull() ? null : value;

				// PCA
				value = rs.getDouble("first_principal_component_x");
				row.FirstPrincipalComponentX = rs.wasNull() ? null : value;

				value = rs.getDouble("first_principal_component_y");
				row.FirstPrincipalComponentY = rs.wasNull() ? null : value;

				value = rs.getDouble("first_principal_component_z");
				row.FirstPrincipalComponentZ = rs.wasNull() ? null : value;

				// EP + EFSPmax
				value = rs.getDouble("influence_on_position_x");
				row.InfluenceOnPointPositionX = rs.wasNull() ? null : value;

				value = rs.getDouble("influence_on_position_y");
				row.InfluenceOnPointPositionY = rs.wasNull() ? null : value;

				value = rs.getDouble("influence_on_position_z");
				row.InfluenceOnPointPositionZ = rs.wasNull() ? null : value;

				value = rs.getDouble("influence_on_network_distortion");
				row.InfluenceOnNetworkDistortion = rs.wasNull() ? null : value;

				// Statistics
				value = rs.getDouble("omega");
				row.Omega = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("p_prio");
				row.PValueApriori = rs.wasNull() ? null : value;

				value = rs.getDouble("p_post");
				row.PValueAposteriori = rs.wasNull() ? null : value;

				value = rs.getDouble("t_prio");
				row.TestStatisticApriori = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("t_post");
				row.TestStatisticAposteriori = rs.wasNull() ? null : Math.Abs(value);

				bool significantPoint = rs.getBoolean("significant");
				row.Significant = !rs.wasNull() && significantPoint == true;

				tableModel.Add(row);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadCongruenceAnalysisPointPair(org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue congruenceAnalysisItemValue, org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue... selectedCongruenceAnalysisItemValues) throws java.sql.SQLException
		private void loadCongruenceAnalysisPointPair(CongruenceAnalysisTreeItemValue congruenceAnalysisItemValue, params CongruenceAnalysisTreeItemValue[] selectedCongruenceAnalysisItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedCongruenceAnalysisItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			UICongruenceAnalysisTableBuilder tableBuilder = UICongruenceAnalysisTableBuilder.Instance;
			TableView<CongruenceAnalysisRow> table = tableBuilder.getTable(congruenceAnalysisItemValue);
			IList<CongruenceAnalysisRow> tableModel = FXCollections.observableArrayList();

			string sql = "SELECT " + "\"CongruenceAnalysisPointPairApriori\".\"id\", \"start_point_name\", \"end_point_name\", \"CongruenceAnalysisPointPairApriori\".\"enable\", " + "\"y\", \"x\", \"z\", \"sigma_y\", \"sigma_x\", \"sigma_z\", \"confidence_major_axis\", \"confidence_middle_axis\", \"confidence_minor_axis\", " + "\"confidence_alpha\", \"confidence_beta\", \"confidence_gamma\", \"gross_error_y\", \"gross_error_x\", \"gross_error_z\", \"minimal_detectable_bias_y\", " + "\"minimal_detectable_bias_x\", \"minimal_detectable_bias_z\", \"p_prio\", \"p_post\", \"t_prio\", \"t_post\", \"significant\", " + "\"group_id\", \"dimension\" FROM \"CongruenceAnalysisPointPairApriori\" " + "JOIN \"CongruenceAnalysisGroup\" ON \"CongruenceAnalysisPointPairApriori\".\"group_id\" = \"CongruenceAnalysisGroup\".\"id\" " + "LEFT JOIN \"CongruenceAnalysisPointPairAposteriori\" ON \"CongruenceAnalysisPointPairApriori\".\"id\" = \"CongruenceAnalysisPointPairAposteriori\".\"id\" " + "WHERE \"CongruenceAnalysisGroup\".\"dimension\" = ? " + "AND \"CongruenceAnalysisGroup\".\"id\" IN (" + inArrayValues + ") " + "ORDER BY \"CongruenceAnalysisGroup\".\"order\" ASC, \"CongruenceAnalysisGroup\".\"id\" ASC, \"CongruenceAnalysisPointPairApriori\".\"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			int idx = 1;
			stmt.setInt(idx++, congruenceAnalysisItemValue.Dimension);

			for (int i = 0; i < selectedCongruenceAnalysisItemValues.Length; i++)
			{
				stmt.setInt(idx++, selectedCongruenceAnalysisItemValues[i].GroupId);
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				CongruenceAnalysisRow row = new CongruenceAnalysisRow();

				// Apriori-Values
				row.Id = rs.getInt("id");
				row.GroupId = rs.getInt("group_id");
				row.NameInReferenceEpoch = rs.getString("start_point_name");
				row.NameInControlEpoch = rs.getString("end_point_name");
				row.Enable = rs.getBoolean("enable");

				double value;
				// Aposteriori
				value = rs.getDouble("x");
				row.XAposteriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("y");
				row.YAposteriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("z");
				row.ZAposteriori = rs.wasNull() ? 0 : value;

				value = rs.getDouble("sigma_x");
				row.SigmaXaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				value = rs.getDouble("sigma_y");
				row.SigmaYaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				value = rs.getDouble("sigma_z");
				row.SigmaZaposteriori = rs.wasNull() ? null : value > 0 ? value : 0.0;

				// Confidence
				value = rs.getDouble("confidence_major_axis");
				row.ConfidenceA = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("confidence_middle_axis");
				row.ConfidenceB = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("confidence_minor_axis");
				row.ConfidenceC = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("confidence_alpha");
				row.ConfidenceAlpha = rs.wasNull() ? null : value;

				value = rs.getDouble("confidence_beta");
				row.ConfidenceBeta = rs.wasNull() ? null : value;

				value = rs.getDouble("confidence_gamma");
				row.ConfidenceGamma = rs.wasNull() ? null : value;

				// Gross error
				value = rs.getDouble("gross_error_x");
				row.GrossErrorX = rs.wasNull() ? null : value;

				value = rs.getDouble("gross_error_y");
				row.GrossErrorY = rs.wasNull() ? null : value;

				value = rs.getDouble("gross_error_z");
				row.GrossErrorZ = rs.wasNull() ? null : value;

				// MDB
				value = rs.getDouble("minimal_detectable_bias_x");
				row.MinimalDetectableBiasX = rs.wasNull() ? null : value;

				value = rs.getDouble("minimal_detectable_bias_y");
				row.MinimalDetectableBiasY = rs.wasNull() ? null : value;

				value = rs.getDouble("minimal_detectable_bias_z");
				row.MinimalDetectableBiasZ = rs.wasNull() ? null : value;

				// Statistics
				value = rs.getDouble("p_prio");
				row.PValueApriori = rs.wasNull() ? null : value;

				value = rs.getDouble("p_post");
				row.PValueAposteriori = rs.wasNull() ? null : value;

				value = rs.getDouble("t_prio");
				row.TestStatisticApriori = rs.wasNull() ? null : Math.Abs(value);

				value = rs.getDouble("t_post");
				row.TestStatisticAposteriori = rs.wasNull() ? null : Math.Abs(value);

				bool significantPoint = rs.getBoolean("significant");
				row.Significant = !rs.wasNull() && significantPoint == true;

				tableModel.Add(row);
			}

			table.getItems().setAll(tableModel);
			if (tableModel.Count == 0)
			{
				table.getItems().setAll(tableBuilder.EmptyRow);
			}
			table.sort();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void remove(org.applied_geodesy.jag3d.ui.table.row.Row rowData) throws java.sql.SQLException
		public virtual void remove(Row rowData)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = null;
			if (rowData is TerrestrialObservationRow)
			{
				sql = "DELETE FROM \"ObservationApriori\" WHERE \"id\" = ? LIMIT 1";
			}
			else if (rowData is GNSSObservationRow)
			{
				sql = "DELETE FROM \"GNSSObservationApriori\" WHERE \"id\" = ? LIMIT 1";
			}
			else if (rowData is PointRow)
			{
				sql = "DELETE FROM \"PointApriori\" WHERE \"id\" = ? LIMIT 1";
			}
			else if (rowData is CongruenceAnalysisRow)
			{
				sql = "DELETE FROM \"CongruenceAnalysisPointPairApriori\" WHERE \"id\" = ? LIMIT 1";
			}
			else if (rowData is VerticalDeflectionRow)
			{
				sql = "DELETE FROM \"VerticalDeflectionApriori\" WHERE \"id\" = ? LIMIT 1";
			}

			if (!string.ReferenceEquals(sql, null))
			{
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				stmt.setInt(1, rowData.Id);
				stmt.execute();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.jag3d.ui.metadata.MetaData metaData) throws java.sql.SQLException
		public virtual void save(MetaData metaData)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"ProjectMetadata\" USING (VALUES " + "(CAST(? AS INT), ?, ?, ?, CAST(? AS TIMESTAMP), ?, ?) " + ") AS \"vals\" (\"id\", \"name\", \"operator\", \"description\", \"date\", \"customer_id\", \"project_id\") ON \"ProjectMetadata\".\"id\" = \"vals\".\"id\" AND \"ProjectMetadata\".\"id\" = 1 " + "WHEN MATCHED THEN UPDATE SET " + "\"ProjectMetadata\".\"name\"        = \"vals\".\"name\", " + "\"ProjectMetadata\".\"operator\"    = \"vals\".\"operator\", " + "\"ProjectMetadata\".\"description\" = \"vals\".\"description\", " + "\"ProjectMetadata\".\"date\"        = \"vals\".\"date\", " + "\"ProjectMetadata\".\"customer_id\" = \"vals\".\"customer_id\", " + "\"ProjectMetadata\".\"project_id\"  = \"vals\".\"project_id\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"name\", " + "\"vals\".\"operator\", " + "\"vals\".\"description\", " + "\"vals\".\"date\", " + "\"vals\".\"customer_id\", " + "\"vals\".\"project_id\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			stmt.setInt(idx++, 1); // default ID
			stmt.setString(idx++, metaData.Name);
			stmt.setString(idx++, metaData.Operator);
			stmt.setString(idx++, metaData.Description);
			stmt.setTimestamp(idx++, Timestamp.valueOf(metaData.Date.atStartOfDay()));

			stmt.setString(idx++, metaData.CustomerId);
			stmt.setString(idx++, metaData.ProjectId);

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void mergeGroups(org.applied_geodesy.jag3d.ui.tree.TreeItemValue referenceItemValue, java.util.List<? extends org.applied_geodesy.jag3d.ui.tree.Groupable> selectedGroups) throws java.sql.SQLException
		public virtual void mergeGroups<T1>(TreeItemValue referenceItemValue, IList<T1> selectedGroups) where T1 : org.applied_geodesy.jag3d.ui.tree.Groupable
		{
			if (!this.hasDatabase() || !this.dataBase.Open || referenceItemValue == null || !(referenceItemValue is Groupable) || selectedGroups == null || selectedGroups.Count == 0)
			{
				return;
			}

			TreeItemType referenceItemType = referenceItemValue.ItemType;
			int referenceGroupId = ((Groupable)referenceItemValue).GroupId;

			if (referenceGroupId < 0)
			{
				throw new System.ArgumentException("Error, could not merge groups! Unsupported group id " + referenceGroupId);
			}

			string tableName = null;
			if (TreeItemType.isPointTypeLeaf(referenceItemType))
			{
				tableName = "PointApriori";
			}
			else if (TreeItemType.isObservationTypeLeaf(referenceItemType))
			{
				tableName = "ObservationApriori";
			}
			else if (TreeItemType.isGNSSObservationTypeLeaf(referenceItemType))
			{
				tableName = "GNSSObservationApriori";
			}
			else if (TreeItemType.isVerticalDeflectionTypeLeaf(referenceItemType))
			{
				tableName = "VerticalDeflectionTreeItemValue";
			}
			else if (TreeItemType.isCongruenceAnalysisTypeLeaf(referenceItemType))
			{
				tableName = "CongruenceAnalysisPointPairApriori";
			}
			else
			{
				throw new System.ArgumentException("Error, could not merge groups! Unsupported item type " + referenceItemType);
			}


			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedGroups.Count; i++)
			{
				inArrayValues.Append(",?");
			}

			string sql = "UPDATE \"" + tableName + "\" SET \"group_id\" = ? WHERE \"group_id\" IN (" + inArrayValues + ")";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setInt(idx++, referenceGroupId);
			for (int i = 0; i < selectedGroups.Count; i++)
			{
				int groupId = selectedGroups[i].getGroupId();
				stmt.setInt(idx++, groupId);
			}
			stmt.execute();
		}

		// http://hsqldb.org/doc/2.0/guide/dataaccess-chapt.html#dac_merge_statement
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveItem(org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow rowData) throws java.sql.SQLException
		public virtual void saveItem(TerrestrialObservationRow rowData)
		{
			if (!this.hasDatabase() || !this.dataBase.Open || rowData == null || string.ReferenceEquals(rowData.StartPointName, null) || string.ReferenceEquals(rowData.EndPointName, null) || rowData.StartPointName.Equals(rowData.EndPointName) || rowData.ValueApriori == null || rowData.StartPointName.Trim().Length == 0 || rowData.EndPointName.Trim().Length == 0)
			{
				return;
			}

			string sql = "MERGE INTO \"ObservationApriori\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), ?, ?, CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"id\", \"group_id\", \"start_point_name\", \"end_point_name\", \"instrument_height\", \"reflector_height\", \"value_0\", \"sigma_0\", \"distance_0\", \"enable\") ON \"ObservationApriori\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"ObservationApriori\".\"group_id\"           = \"vals\".\"group_id\", " + "\"ObservationApriori\".\"start_point_name\"   = \"vals\".\"start_point_name\", " + "\"ObservationApriori\".\"end_point_name\"     = \"vals\".\"end_point_name\", " + "\"ObservationApriori\".\"instrument_height\"  = \"vals\".\"instrument_height\", " + "\"ObservationApriori\".\"reflector_height\"   = \"vals\".\"reflector_height\", " + "\"ObservationApriori\".\"value_0\"            = \"vals\".\"value_0\", " + "\"ObservationApriori\".\"sigma_0\"            = \"vals\".\"sigma_0\", " + "\"ObservationApriori\".\"distance_0\"         = \"vals\".\"distance_0\", " + "\"ObservationApriori\".\"enable\"             = \"vals\".\"enable\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"group_id\", " + "\"vals\".\"start_point_name\", " + "\"vals\".\"end_point_name\"," + "\"vals\".\"instrument_height\", " + "\"vals\".\"reflector_height\", " + "\"vals\".\"value_0\", " + "\"vals\".\"sigma_0\", " + "\"vals\".\"distance_0\", " + "\"vals\".\"enable\"";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			if (rowData.Id < 0)
			{
				stmt.setNull(idx++, Types.INTEGER);
			}
			// Update existing item
			else
			{
				stmt.setInt(idx++, rowData.Id);
			}

			stmt.setInt(idx++, rowData.GroupId);
			stmt.setString(idx++, rowData.StartPointName);
			stmt.setString(idx++, rowData.EndPointName);

			stmt.setDouble(idx++, rowData.InstrumentHeight);
			stmt.setDouble(idx++, rowData.ReflectorHeight);

			stmt.setDouble(idx++, rowData.ValueApriori);

			stmt.setDouble(idx++, rowData.SigmaApriori == null || rowData.SigmaApriori < 0 ? 0 : rowData.SigmaApriori);
			stmt.setDouble(idx++, rowData.DistanceApriori == null || rowData.DistanceApriori < 0 ? 0 : rowData.DistanceApriori);

			stmt.setBoolean(idx++, rowData.Enable);

			stmt.execute();

			if (rowData.Id < 0)
			{
				int id = this.dataBase.LastInsertId;
				rowData.Id = id;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveItem(org.applied_geodesy.jag3d.ui.table.row.VerticalDeflectionRow rowData) throws java.sql.SQLException
		public virtual void saveItem(VerticalDeflectionRow rowData)
		{
			if (!this.hasDatabase() || !this.dataBase.Open || string.ReferenceEquals(rowData.Name, null) || rowData.Name.Trim().Length == 0 || (rowData.YApriori == null && rowData.XApriori == null))
			{
				return;
			}

			string sql = "MERGE INTO \"VerticalDeflectionApriori\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), ?, CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"id\", \"group_id\", \"name\", \"y0\", \"x0\", \"sigma_y0\", \"sigma_x0\", \"enable\") ON \"VerticalDeflectionApriori\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"VerticalDeflectionApriori\".\"group_id\"  = \"vals\".\"group_id\", " + "\"VerticalDeflectionApriori\".\"name\"      = \"vals\".\"name\", " + "\"VerticalDeflectionApriori\".\"y0\"        = \"vals\".\"y0\", " + "\"VerticalDeflectionApriori\".\"x0\"        = \"vals\".\"x0\", " + "\"VerticalDeflectionApriori\".\"sigma_y0\"  = \"vals\".\"sigma_y0\", " + "\"VerticalDeflectionApriori\".\"sigma_x0\"  = \"vals\".\"sigma_x0\", " + "\"VerticalDeflectionApriori\".\"enable\"    = \"vals\".\"enable\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"group_id\", " + "\"vals\".\"name\", " + "\"vals\".\"y0\", " + "\"vals\".\"x0\", " + "\"vals\".\"sigma_y0\", " + "\"vals\".\"sigma_x0\", " + "\"vals\".\"enable\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			if (rowData.Id < 0)
			{
				stmt.setNull(idx++, Types.INTEGER);
			}
			// Update existing item
			else
			{
				stmt.setInt(idx++, rowData.Id);
			}
			stmt.setInt(idx++, rowData.GroupId);

			stmt.setString(idx++, rowData.Name);

			stmt.setDouble(idx++, rowData.YApriori == null ? 0.0 : rowData.YApriori);
			stmt.setDouble(idx++, rowData.XApriori == null ? 0.0 : rowData.XApriori);

			stmt.setDouble(idx++, rowData.SigmaYapriori == null ? 0.0 : rowData.SigmaYapriori);
			stmt.setDouble(idx++, rowData.SigmaXapriori == null ? 0.0 : rowData.SigmaXapriori);

			stmt.setBoolean(idx++, rowData.Enable);

			stmt.execute();

			if (rowData.Id < 0)
			{
				int id = this.dataBase.LastInsertId;
				rowData.Id = id;
			}
		}

		// http://hsqldb.org/doc/2.0/guide/dataaccess-chapt.html#dac_merge_statement
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveItem(org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow rowData) throws java.sql.SQLException
		public virtual void saveItem(GNSSObservationRow rowData)
		{
			if (!this.hasDatabase() || !this.dataBase.Open || rowData == null || string.ReferenceEquals(rowData.StartPointName, null) || string.ReferenceEquals(rowData.EndPointName, null) || rowData.StartPointName.Equals(rowData.EndPointName) || (rowData.YApriori == null && rowData.XApriori == null && rowData.ZApriori == null) || rowData.StartPointName.Trim().Length == 0 || rowData.EndPointName.Trim().Length == 0)
			{
				return;
			}

			string sql = "MERGE INTO \"GNSSObservationApriori\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), ?, ?, CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"id\", \"group_id\", \"start_point_name\", \"end_point_name\", \"y0\", \"x0\", \"z0\", \"sigma_y0\", \"sigma_x0\", \"sigma_z0\", \"enable\") ON \"GNSSObservationApriori\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"GNSSObservationApriori\".\"group_id\"         = \"vals\".\"group_id\", " + "\"GNSSObservationApriori\".\"start_point_name\" = \"vals\".\"start_point_name\", " + "\"GNSSObservationApriori\".\"end_point_name\"   = \"vals\".\"end_point_name\", " + "\"GNSSObservationApriori\".\"y0\"               = \"vals\".\"y0\", " + "\"GNSSObservationApriori\".\"x0\"               = \"vals\".\"x0\", " + "\"GNSSObservationApriori\".\"z0\"               = \"vals\".\"z0\", " + "\"GNSSObservationApriori\".\"sigma_y0\"         = \"vals\".\"sigma_y0\", " + "\"GNSSObservationApriori\".\"sigma_x0\"         = \"vals\".\"sigma_x0\", " + "\"GNSSObservationApriori\".\"sigma_z0\"         = \"vals\".\"sigma_z0\", " + "\"GNSSObservationApriori\".\"enable\"           = \"vals\".\"enable\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"group_id\", " + "\"vals\".\"start_point_name\", " + "\"vals\".\"end_point_name\"," + "\"vals\".\"y0\", " + "\"vals\".\"x0\", " + "\"vals\".\"z0\", " + "\"vals\".\"sigma_y0\", " + "\"vals\".\"sigma_x0\", " + "\"vals\".\"sigma_z0\", " + "\"vals\".\"enable\"";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			if (rowData.Id < 0)
			{
				stmt.setNull(idx++, Types.INTEGER);
			}
			// Update existing item
			else
			{
				stmt.setInt(idx++, rowData.Id);
			}

			stmt.setInt(idx++, rowData.GroupId);
			stmt.setString(idx++, rowData.StartPointName);
			stmt.setString(idx++, rowData.EndPointName);

			stmt.setDouble(idx++, rowData.YApriori == null ? 0.0 : rowData.YApriori);
			stmt.setDouble(idx++, rowData.XApriori == null ? 0.0 : rowData.XApriori);
			stmt.setDouble(idx++, rowData.ZApriori == null ? 0.0 : rowData.ZApriori);

			stmt.setDouble(idx++, rowData.SigmaYapriori == null || rowData.SigmaYapriori < 0 ? 0 : rowData.SigmaYapriori);
			stmt.setDouble(idx++, rowData.SigmaXapriori == null || rowData.SigmaXapriori < 0 ? 0 : rowData.SigmaXapriori);
			stmt.setDouble(idx++, rowData.SigmaZapriori == null || rowData.SigmaZapriori < 0 ? 0 : rowData.SigmaZapriori);

			stmt.setBoolean(idx++, rowData.Enable);

			stmt.execute();

			if (rowData.Id < 0)
			{
				int id = this.dataBase.LastInsertId;
				rowData.Id = id;
			}
		}

		// http://hsqldb.org/doc/2.0/guide/dataaccess-chapt.html#dac_merge_statement 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveItem(org.applied_geodesy.jag3d.ui.table.row.PointRow rowData) throws java.sql.SQLException
		public virtual void saveItem(PointRow rowData)
		{
			if (!this.hasDatabase() || !this.dataBase.Open || string.ReferenceEquals(rowData.Name, null) || rowData.Name.Trim().Length == 0 || (rowData.YApriori == null && rowData.XApriori == null && rowData.ZApriori == null))
			{
				return;
			}

			string sql = "MERGE INTO \"PointApriori\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), ?, ?, CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"id\", \"group_id\", \"name\", \"code\", \"y0\", \"x0\", \"z0\", \"sigma_y0\", \"sigma_x0\", \"sigma_z0\", \"enable\") ON \"PointApriori\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"PointApriori\".\"group_id\"  = \"vals\".\"group_id\", " + "\"PointApriori\".\"name\"      = \"vals\".\"name\", " + "\"PointApriori\".\"code\"      = \"vals\".\"code\", " + "\"PointApriori\".\"y0\"        = \"vals\".\"y0\", " + "\"PointApriori\".\"x0\"        = \"vals\".\"x0\", " + "\"PointApriori\".\"z0\"        = \"vals\".\"z0\", " + "\"PointApriori\".\"sigma_y0\"  = \"vals\".\"sigma_y0\", " + "\"PointApriori\".\"sigma_x0\"  = \"vals\".\"sigma_x0\", " + "\"PointApriori\".\"sigma_z0\"  = \"vals\".\"sigma_z0\", " + "\"PointApriori\".\"enable\"    = \"vals\".\"enable\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"group_id\", " + "\"vals\".\"name\", " + "\"vals\".\"code\"," + "\"vals\".\"y0\", " + "\"vals\".\"x0\", " + "\"vals\".\"z0\", " + "\"vals\".\"sigma_y0\", " + "\"vals\".\"sigma_x0\", " + "\"vals\".\"sigma_z0\", " + "\"vals\".\"enable\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			if (rowData.Id < 0)
			{
				stmt.setNull(idx++, Types.INTEGER);
			}
			// Update existing item
			else
			{
				stmt.setInt(idx++, rowData.Id);
			}
			stmt.setInt(idx++, rowData.GroupId);

			stmt.setString(idx++, rowData.Name);
			stmt.setString(idx++, string.ReferenceEquals(rowData.Code, null) || rowData.Code.Trim().Length == 0 ? "" : rowData.Code);

			stmt.setDouble(idx++, rowData.YApriori == null ? 0.0 : rowData.YApriori);
			stmt.setDouble(idx++, rowData.XApriori == null ? 0.0 : rowData.XApriori);
			stmt.setDouble(idx++, rowData.ZApriori == null ? 0.0 : rowData.ZApriori);

			stmt.setDouble(idx++, rowData.SigmaYapriori == null ? 0.0 : rowData.SigmaYapriori);
			stmt.setDouble(idx++, rowData.SigmaXapriori == null ? 0.0 : rowData.SigmaXapriori);
			stmt.setDouble(idx++, rowData.SigmaZapriori == null ? 0.0 : rowData.SigmaZapriori);

			stmt.setBoolean(idx++, rowData.Enable);

			stmt.execute();

			if (rowData.Id < 0)
			{
				int id = this.dataBase.LastInsertId;
				rowData.Id = id;
			}
		}

		// http://hsqldb.org/doc/2.0/guide/dataaccess-chapt.html#dac_merge_statement
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveItem(org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow rowData) throws java.sql.SQLException
		public virtual void saveItem(CongruenceAnalysisRow rowData)
		{
			if (!this.hasDatabase() || !this.dataBase.Open || string.ReferenceEquals(rowData.NameInControlEpoch, null) || string.ReferenceEquals(rowData.NameInReferenceEpoch, null) || rowData.NameInControlEpoch.Trim().Length == 0 || rowData.NameInReferenceEpoch.Trim().Length == 0)
			{
				return;
			}

			string sql = "MERGE INTO \"CongruenceAnalysisPointPairApriori\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), ?, ?, CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"id\", \"group_id\", \"start_point_name\", \"end_point_name\", \"enable\") ON \"CongruenceAnalysisPointPairApriori\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"CongruenceAnalysisPointPairApriori\".\"group_id\"          = \"vals\".\"group_id\", " + "\"CongruenceAnalysisPointPairApriori\".\"start_point_name\"  = \"vals\".\"start_point_name\", " + "\"CongruenceAnalysisPointPairApriori\".\"end_point_name\"    = \"vals\".\"end_point_name\", " + "\"CongruenceAnalysisPointPairApriori\".\"enable\"            = \"vals\".\"enable\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"group_id\", " + "\"vals\".\"start_point_name\", " + "\"vals\".\"end_point_name\", " + "\"vals\".\"enable\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			if (rowData.Id < 0)
			{
				stmt.setNull(idx++, Types.INTEGER);
			}
			// Update existing item
			else
			{
				stmt.setInt(idx++, rowData.Id);
			}
			stmt.setInt(idx++, rowData.GroupId);

			stmt.setString(idx++, rowData.NameInReferenceEpoch);
			stmt.setString(idx++, rowData.NameInControlEpoch);

			stmt.setBoolean(idx++, rowData.Enable);

			stmt.execute();

			if (rowData.Id < 0)
			{
				int id = this.dataBase.LastInsertId;
				rowData.Id = id;
			}
		}

		// http://hsqldb.org/doc/2.0/guide/dataaccess-chapt.html#dac_merge_statement
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveGroup(org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue pointTreeItemValue) throws java.sql.SQLException
		public virtual void saveGroup(PointTreeItemValue pointTreeItemValue)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"PointGroup\" USING (VALUES " + "(CAST(? AS INT), ?, CAST(? AS INT), CAST(? AS INT), CAST(? AS BOOLEAN), CAST(? AS INT)) " + ") AS \"vals\" (\"id\", \"name\", \"type\", \"dimension\", \"enable\", \"order\") ON \"PointGroup\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"PointGroup\".\"name\"                = \"vals\".\"name\", " + "\"PointGroup\".\"type\"                = \"vals\".\"type\", " + "\"PointGroup\".\"dimension\"           = \"vals\".\"dimension\", " + "\"PointGroup\".\"enable\"              = \"vals\".\"enable\", " + "\"PointGroup\".\"order\"               = \"vals\".\"order\" " + "WHEN NOT MATCHED THEN INSERT " + "(\"id\", \"name\", \"type\", \"dimension\", \"enable\", \"order\") " + "VALUES " + "\"vals\".\"id\", " + "\"vals\".\"name\", " + "\"vals\".\"type\", " + "\"vals\".\"dimension\", " + "\"vals\".\"enable\", " + "\"vals\".\"order\" ";

			int groupId = pointTreeItemValue.GroupId;
			string name = pointTreeItemValue.Name.Trim();
			int dimension = pointTreeItemValue.Dimension;
			PointType type = TreeItemType.getPointTypeByTreeItemType(pointTreeItemValue.ItemType);
			bool enable = pointTreeItemValue.Enable;
			int orderId = pointTreeItemValue.OrderId;

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			if (groupId < 0)
			{
				stmt.setNull(idx++, Types.INTEGER);
			}
			// Update existing item
			else
			{
				stmt.setInt(idx++, groupId);
			}

			stmt.setString(idx++, name);
			stmt.setInt(idx++, type.getId());
			stmt.setInt(idx++, dimension);
			stmt.setBoolean(idx++, enable);
			stmt.setInt(idx++, orderId);
			//stmt.setBoolean(idx++, false); // consider_deflection: not used for insert/update

			stmt.execute();

			// insert new group
			if (groupId < 0)
			{
				int id = this.dataBase.LastInsertId;
				pointTreeItemValue.GroupId = id;

				PointGroupUncertaintyType[] uncertaintyTypes = PointGroupUncertaintyType.values();
				foreach (PointGroupUncertaintyType uncertaintyType in uncertaintyTypes)
				{
					this.saveUncertainty(uncertaintyType, PointTreeItemValue.getDefaultUncertainty(uncertaintyType), pointTreeItemValue);
				}
			}
		}

		// http://hsqldb.org/doc/2.0/guide/dataaccess-chapt.html#dac_merge_statement
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveGroup(org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue observationTreeItemValue) throws java.sql.SQLException
		public virtual void saveGroup(ObservationTreeItemValue observationTreeItemValue)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"ObservationGroup\" USING (VALUES " + "(CAST(? AS INT), ?, CAST(? AS INT), CAST(? AS BOOLEAN), CAST(? AS INT)) " + ") AS \"vals\" (\"id\", \"name\", \"type\", \"enable\", \"order\") ON \"ObservationGroup\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"ObservationGroup\".\"name\"           = \"vals\".\"name\", " + "\"ObservationGroup\".\"type\"           = \"vals\".\"type\", " + "\"ObservationGroup\".\"enable\"         = \"vals\".\"enable\", " + "\"ObservationGroup\".\"order\"          = \"vals\".\"order\" " + "WHEN NOT MATCHED THEN INSERT " + "(\"id\", \"name\", \"type\", \"enable\", \"order\", \"reference_epoch\") " + "VALUES " + "\"vals\".\"id\", " + "\"vals\".\"name\", " + "\"vals\".\"type\", " + "\"vals\".\"enable\", " + "\"vals\".\"order\", " + "DEFAULT"; // reference_epoch type is set to DEFAULT

			int groupId = observationTreeItemValue.GroupId;
			string name = observationTreeItemValue.Name.Trim();
			ObservationType type = TreeItemType.getObservationTypeByTreeItemType(observationTreeItemValue.ItemType);
			bool enable = observationTreeItemValue.Enable;
			int orderId = observationTreeItemValue.OrderId;

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			if (groupId < 0)
			{
				stmt.setNull(idx++, Types.INTEGER);
			}
			// Update existing item
			else
			{
				stmt.setInt(idx++, groupId);
			}

			stmt.setString(idx++, name);
			stmt.setInt(idx++, type.getId());
			stmt.setBoolean(idx++, enable);
			stmt.setInt(idx++, orderId);

			//stmt.setBoolean(idx++, true); // reference epoch; not used for update/insert

			stmt.execute();

			// insert new group
			if (groupId < 0)
			{
				groupId = this.dataBase.LastInsertId;
				observationTreeItemValue.GroupId = groupId;

				ParameterType[] parameters = ObservationTreeItemValue.getParameterTypes(observationTreeItemValue.ItemType);
				foreach (ParameterType parameterType in parameters)
				{
					this.saveAdditionalParameter(parameterType, parameterType == ParameterType.ORIENTATION ? true : false, parameterType == ParameterType.SCALE ? 1.0 : 0.0, observationTreeItemValue);
				}

				ObservationGroupUncertaintyType[] uncertaintyTypes = ObservationGroupUncertaintyType.values();
				foreach (ObservationGroupUncertaintyType uncertaintyType in uncertaintyTypes)
				{
					this.saveUncertainty(uncertaintyType, ObservationTreeItemValue.getDefaultUncertainty(observationTreeItemValue.ItemType, uncertaintyType), observationTreeItemValue);
				}
			}
		}

		// http://hsqldb.org/doc/2.0/guide/dataaccess-chapt.html#dac_merge_statement
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveGroup(org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue verticalDeflectionTreeItemValue) throws java.sql.SQLException
		public virtual void saveGroup(VerticalDeflectionTreeItemValue verticalDeflectionTreeItemValue)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"VerticalDeflectionGroup\" USING (VALUES " + "(CAST(? AS INT), ?, CAST(? AS INT), CAST(? AS BOOLEAN), CAST(? AS INT)) " + ") AS \"vals\" (\"id\", \"name\", \"type\", \"enable\", \"order\") ON \"VerticalDeflectionGroup\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"VerticalDeflectionGroup\".\"name\"   = \"vals\".\"name\", " + "\"VerticalDeflectionGroup\".\"type\"   = \"vals\".\"type\", " + "\"VerticalDeflectionGroup\".\"enable\" = \"vals\".\"enable\", " + "\"VerticalDeflectionGroup\".\"order\"  = \"vals\".\"order\" " + "WHEN NOT MATCHED THEN INSERT " + "(\"id\", \"name\", \"type\", \"enable\", \"order\") " + "VALUES " + "\"vals\".\"id\", " + "\"vals\".\"name\", " + "\"vals\".\"type\", " + "\"vals\".\"enable\", " + "\"vals\".\"order\"";

			int groupId = verticalDeflectionTreeItemValue.GroupId;
			string name = verticalDeflectionTreeItemValue.Name.Trim();
			VerticalDeflectionType type = TreeItemType.getVerticalDeflectionTypeByTreeItemType(verticalDeflectionTreeItemValue.ItemType);
			bool enable = verticalDeflectionTreeItemValue.Enable;
			int orderId = verticalDeflectionTreeItemValue.OrderId;

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			if (groupId < 0)
			{
				stmt.setNull(idx++, Types.INTEGER);
			}
			// Update existing item
			else
			{
				stmt.setInt(idx++, groupId);
			}

			stmt.setString(idx++, name);
			stmt.setInt(idx++, type.getId());
			stmt.setBoolean(idx++, enable);
			stmt.setInt(idx++, orderId);

			stmt.execute();

			// insert new group
			if (groupId < 0)
			{
				int id = this.dataBase.LastInsertId;
				verticalDeflectionTreeItemValue.GroupId = id;

				VerticalDeflectionGroupUncertaintyType[] uncertaintyTypes = VerticalDeflectionGroupUncertaintyType.values();
				foreach (VerticalDeflectionGroupUncertaintyType uncertaintyType in uncertaintyTypes)
				{
					this.saveUncertainty(uncertaintyType, VerticalDeflectionTreeItemValue.getDefaultUncertainty(uncertaintyType), verticalDeflectionTreeItemValue);
				}
			}
		}

		// http://hsqldb.org/doc/2.0/guide/dataaccess-chapt.html#dac_merge_statement
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveGroup(org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue congruenceAnalysisTreeItemValue) throws java.sql.SQLException
		public virtual void saveGroup(CongruenceAnalysisTreeItemValue congruenceAnalysisTreeItemValue)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"CongruenceAnalysisGroup\" USING (VALUES " + "(CAST(? AS INT), ?, CAST(? AS INT), CAST(? AS BOOLEAN), CAST(? AS INT)) " + ") AS \"vals\" (\"id\", \"name\", \"dimension\", \"enable\", \"order\") ON \"CongruenceAnalysisGroup\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"CongruenceAnalysisGroup\".\"name\"   = \"vals\".\"name\", " + "\"CongruenceAnalysisGroup\".\"enable\" = \"vals\".\"enable\", " + "\"CongruenceAnalysisGroup\".\"order\"  = \"vals\".\"order\" " + "WHEN NOT MATCHED THEN INSERT " + "(\"id\", \"name\", \"dimension\", \"enable\", \"order\") " + "VALUES " + "\"vals\".\"id\", " + "\"vals\".\"name\", " + "\"vals\".\"dimension\", " + "\"vals\".\"enable\", " + "\"vals\".\"order\"";

			int groupId = congruenceAnalysisTreeItemValue.GroupId;
			string name = congruenceAnalysisTreeItemValue.Name.Trim();
			int dimension = congruenceAnalysisTreeItemValue.Dimension;
			bool enable = congruenceAnalysisTreeItemValue.Enable;
			int orderId = congruenceAnalysisTreeItemValue.OrderId;

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			if (groupId < 0)
			{
				stmt.setNull(idx++, Types.INTEGER);
			}
			// Update existing item
			else
			{
				stmt.setInt(idx++, groupId);
			}

			stmt.setString(idx++, name);
			stmt.setInt(idx++, dimension);
			stmt.setBoolean(idx++, enable);
			stmt.setInt(idx++, orderId);

			stmt.execute();

			// insert new group
			if (groupId < 0)
			{
				int id = this.dataBase.LastInsertId;
				congruenceAnalysisTreeItemValue.GroupId = id;

				RestrictionType[] restrictions = CongruenceAnalysisTreeItemValue.getRestrictionTypes(congruenceAnalysisTreeItemValue.ItemType);
				foreach (RestrictionType restrictionType in restrictions)
				{
					switch (restrictionType.innerEnumValue)
					{
					case RestrictionType.InnerEnum.IDENT_SCALES_XY:
					case RestrictionType.InnerEnum.IDENT_SCALES_XZ:
					case RestrictionType.InnerEnum.IDENT_SCALES_YZ:
						this.saveStrainParameter(restrictionType, true, congruenceAnalysisTreeItemValue);
						break;
					default:
						this.saveStrainParameter(restrictionType, false, congruenceAnalysisTreeItemValue);
						break;
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void removeGroup(org.applied_geodesy.jag3d.ui.tree.TreeItemValue treeItemValue) throws java.sql.SQLException
		public virtual void removeGroup(TreeItemValue treeItemValue)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = null;
			int id = -1;
			if (treeItemValue is PointTreeItemValue)
			{
				sql = "DELETE FROM \"PointGroup\" WHERE \"id\" = ? LIMIT 1";
				id = ((PointTreeItemValue)treeItemValue).GroupId;
			}
			else if (treeItemValue is ObservationTreeItemValue)
			{
				sql = "DELETE FROM \"ObservationGroup\" WHERE \"id\" = ? LIMIT 1";
				id = ((ObservationTreeItemValue)treeItemValue).GroupId;
			}
			else if (treeItemValue is CongruenceAnalysisTreeItemValue)
			{
				sql = "DELETE FROM \"CongruenceAnalysisGroup\" WHERE \"id\" = ? LIMIT 1";
				id = ((CongruenceAnalysisTreeItemValue)treeItemValue).GroupId;
			}
			else if (treeItemValue is VerticalDeflectionTreeItemValue)
			{
				sql = "DELETE FROM \"VerticalDeflectionGroup\" WHERE \"id\" = ? LIMIT 1";
				id = ((VerticalDeflectionTreeItemValue)treeItemValue).GroupId;
			}
			else
			{
				Console.Error.WriteLine(this.GetType().Name + " : Error, cannot remove item, because type is unknwon " + treeItemValue.ItemType);
				return;
			}

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, id);

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveEpoch(boolean referenceEpoch, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedObservationItemValues) throws java.sql.SQLException
		public virtual void saveEpoch(bool referenceEpoch, params ObservationTreeItemValue[] selectedObservationItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < selectedObservationItemValues.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			string sql = "UPDATE \"ObservationGroup\" SET \"reference_epoch\" = ? WHERE \"id\" IN (" + inArrayValues + ")";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setBoolean(idx++, referenceEpoch);
			for (int i = 0; i < selectedObservationItemValues.Length; i++)
			{
				int groupId = selectedObservationItemValues[i].GroupId;
				stmt.setInt(idx++, groupId);
			}

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveUncertainty(org.applied_geodesy.adjustment.network.PointGroupUncertaintyType uncertaintyType, double value, org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue... selectedPointItemValues) throws java.sql.SQLException
		public virtual void saveUncertainty(PointGroupUncertaintyType uncertaintyType, double value, params PointTreeItemValue[] selectedPointItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder values = new StringBuilder("(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE))");
			for (int i = 1; i < selectedPointItemValues.Length; i++)
			{
				values.Append(",(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE))");
			}

			string sql = "MERGE INTO \"PointGroupUncertainty\" USING (VALUES " + values + ") AS \"vals\" (\"group_id\", \"type\",\"value\") ON \"PointGroupUncertainty\".\"group_id\" = \"vals\".\"group_id\" AND \"PointGroupUncertainty\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"PointGroupUncertainty\".\"value\" = \"vals\".\"value\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"group_id\", " + "\"vals\".\"type\", " + "\"vals\".\"value\" ";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			for (int i = 0; i < selectedPointItemValues.Length; i++)
			{
				int groupId = selectedPointItemValues[i].GroupId;
				stmt.setInt(idx++, groupId);
				stmt.setInt(idx++, uncertaintyType.getId());
				stmt.setDouble(idx++, value);
			}

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveUncertainty(org.applied_geodesy.adjustment.network.VerticalDeflectionGroupUncertaintyType uncertaintyType, double value, org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue... selectedVerticalDeflectionItemValues) throws java.sql.SQLException
		public virtual void saveUncertainty(VerticalDeflectionGroupUncertaintyType uncertaintyType, double value, params VerticalDeflectionTreeItemValue[] selectedVerticalDeflectionItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder values = new StringBuilder("(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE))");
			for (int i = 1; i < selectedVerticalDeflectionItemValues.Length; i++)
			{
				values.Append(",(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE))");
			}

			string sql = "MERGE INTO \"VerticalDeflectionGroupUncertainty\" USING (VALUES " + values + ") AS \"vals\" (\"group_id\", \"type\",\"value\") ON \"VerticalDeflectionGroupUncertainty\".\"group_id\" = \"vals\".\"group_id\" AND \"VerticalDeflectionGroupUncertainty\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"VerticalDeflectionGroupUncertainty\".\"value\" = \"vals\".\"value\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"group_id\", " + "\"vals\".\"type\", " + "\"vals\".\"value\" ";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			for (int i = 0; i < selectedVerticalDeflectionItemValues.Length; i++)
			{
				int groupId = selectedVerticalDeflectionItemValues[i].GroupId;
				stmt.setInt(idx++, groupId);
				stmt.setInt(idx++, uncertaintyType.getId());
				stmt.setDouble(idx++, value);
			}

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveUncertainty(org.applied_geodesy.adjustment.network.ObservationGroupUncertaintyType uncertaintyType, double value, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedObservationItemValues) throws java.sql.SQLException
		public virtual void saveUncertainty(ObservationGroupUncertaintyType uncertaintyType, double value, params ObservationTreeItemValue[] selectedObservationItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder values = new StringBuilder("(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE))");
			for (int i = 1; i < selectedObservationItemValues.Length; i++)
			{
				values.Append(",(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE))");
			}

			string sql = "MERGE INTO \"ObservationGroupUncertainty\" USING (VALUES " + values + ") AS \"vals\" (\"group_id\", \"type\",\"value\") ON \"ObservationGroupUncertainty\".\"group_id\" = \"vals\".\"group_id\" AND \"ObservationGroupUncertainty\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"ObservationGroupUncertainty\".\"value\" = \"vals\".\"value\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"group_id\", " + "\"vals\".\"type\", " + "\"vals\".\"value\" ";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			for (int i = 0; i < selectedObservationItemValues.Length; i++)
			{
				int groupId = selectedObservationItemValues[i].GroupId;
				if (groupId < 0)
				{
					continue;
				}
				stmt.setInt(idx++, groupId);
				stmt.setInt(idx++, uncertaintyType.getId());
				stmt.setDouble(idx++, value);
			}

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveStrainParameter(org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType parameterType, boolean enable, org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue... selectedCongruenceAnalysisItemValues) throws java.sql.SQLException
		public virtual void saveStrainParameter(RestrictionType parameterType, bool enable, params CongruenceAnalysisTreeItemValue[] selectedCongruenceAnalysisItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder values = new StringBuilder("(CAST(? AS INT), CAST(? AS INT), CAST(? AS BOOLEAN))");
			for (int i = 1; i < selectedCongruenceAnalysisItemValues.Length; i++)
			{
				values.Append(",(CAST(? AS INT), CAST(? AS INT), CAST(? AS BOOLEAN))");
			}

			string sql = "MERGE INTO \"CongruenceAnalysisStrainParameterRestriction\" USING (VALUES " + values + ") AS \"vals\" (\"group_id\",\"type\",\"enable\") ON \"CongruenceAnalysisStrainParameterRestriction\".\"group_id\" = \"vals\".\"group_id\" AND \"CongruenceAnalysisStrainParameterRestriction\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"CongruenceAnalysisStrainParameterRestriction\".\"enable\" = \"vals\".\"enable\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"group_id\", " + "\"vals\".\"type\", " + "\"vals\".\"enable\" ";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			for (int i = 0; i < selectedCongruenceAnalysisItemValues.Length; i++)
			{
				int groupId = selectedCongruenceAnalysisItemValues[i].GroupId;
				if (groupId < 0)
				{
					continue;
				}
				stmt.setInt(idx++, groupId);
				stmt.setInt(idx++, parameterType.getId());
				stmt.setBoolean(idx++, enable);
			}

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveAdditionalParameter(org.applied_geodesy.adjustment.network.ParameterType parameterType, boolean enable, double value, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedObservationItemValues) throws java.sql.SQLException
		public virtual void saveAdditionalParameter(ParameterType parameterType, bool enable, double value, params ObservationTreeItemValue[] selectedObservationItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			StringBuilder values = new StringBuilder("(CAST(? AS INT), CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS BOOLEAN))");
			for (int i = 1; i < selectedObservationItemValues.Length; i++)
			{
				values.Append(",(CAST(? AS INT), CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS BOOLEAN))");
			}

			string sql = "MERGE INTO \"AdditionalParameterApriori\" USING (VALUES " + values + ") AS \"vals\" (\"id\",\"group_id\",\"type\",\"value_0\",\"enable\") ON \"AdditionalParameterApriori\".\"group_id\" = \"vals\".\"group_id\" AND \"AdditionalParameterApriori\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"AdditionalParameterApriori\".\"value_0\" = \"vals\".\"value_0\", " + "\"AdditionalParameterApriori\".\"enable\" = \"vals\".\"enable\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"group_id\", " + "\"vals\".\"type\", " + "\"vals\".\"value_0\", " + "\"vals\".\"enable\" ";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			for (int i = 0; i < selectedObservationItemValues.Length; i++)
			{
				int groupId = selectedObservationItemValues[i].GroupId;
				if (groupId < 0)
				{
					continue;
				}
				stmt.setNull(idx++, Types.INTEGER); // ID, is not used for insert/update
				stmt.setInt(idx++, groupId);
				stmt.setInt(idx++, parameterType.getId());
				stmt.setDouble(idx++, value);
				stmt.setBoolean(idx++, enable);
			}

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getNextValidVerticalDeflectionName(String name) throws java.sql.SQLException
		public virtual string getNextValidVerticalDeflectionName(string name)
		{
			if (!this.hasDatabase() || !this.dataBase.Open || string.ReferenceEquals(name, null) || name.Trim().Length == 0)
			{
				return null;
			}


			string regexp = "^\\Q" + name + "\\E\\s+\\((\\d+)\\)$";
			string sql = "SELECT MAX(CAST(REGEXP_REPLACE(\"name\", ?, '$1') AS INTEGER)) AS \"cnt\" FROM \"VerticalDeflectionApriori\" WHERE REGEXP_MATCHES(\"name\", ?)";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setString(1, regexp);
			stmt.setString(2, regexp);

			int cnt = 0;
			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				cnt = rs.getInt("cnt");
				if (rs.wasNull())
				{
					cnt = 0;
				}
			}
			return String.format(Locale.ENGLISH, "%s (%d)", name, cnt + 1);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getNextValidPointName(String name) throws java.sql.SQLException
		public virtual string getNextValidPointName(string name)
		{
			if (!this.hasDatabase() || !this.dataBase.Open || string.ReferenceEquals(name, null) || name.Trim().Length == 0)
			{
				return null;
			}

			string regexp = "^\\Q" + name + "\\E\\s+\\((\\d+)\\)$";
			string sql = "SELECT MAX(CAST(REGEXP_REPLACE(\"name\", ?, '$1') AS INTEGER)) AS \"cnt\" FROM \"PointApriori\" WHERE REGEXP_MATCHES(\"name\", ?)";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setString(1, regexp);
			stmt.setString(2, regexp);

			int cnt = 0;
			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				cnt = rs.getInt("cnt");
				if (rs.wasNull())
				{
					cnt = 0;
				}
			}
			return String.format(Locale.ENGLISH, "%s (%d)", name, cnt + 1);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String[] getNextValidPointNexusNames(int groupId, String firstPointName, String secondPointName) throws java.sql.SQLException
		public virtual string[] getNextValidPointNexusNames(int groupId, string firstPointName, string secondPointName)
		{
			if (!this.hasDatabase() || !this.dataBase.Open || groupId < 0 || string.ReferenceEquals(firstPointName, null) || firstPointName.Trim().Length == 0 || string.ReferenceEquals(secondPointName, null) || secondPointName.Trim().Length == 0)
			{
				return new string[] {firstPointName, secondPointName};
			}

			string regexp = "^\\Q" + secondPointName + "\\E\\s+\\((\\d+)\\)$";
			string sql = "SELECT MAX(CAST(REGEXP_REPLACE(\"end_point_name\", ?, '$1') AS INTEGER)) AS \"cnt\" FROM \"CongruenceAnalysisPointPairApriori\" WHERE \"group_id\" = ? AND \"start_point_name\" = ? AND REGEXP_MATCHES(\"end_point_name\", ?)";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setString(1, regexp);
			stmt.setInt(2, groupId);
			stmt.setString(3, firstPointName);
			stmt.setString(4, regexp);

			int cnt = 0;
			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				cnt = rs.getInt("cnt");
				if (rs.wasNull())
				{
					cnt = 0;
				}
			}
			return new string[] {firstPointName, String.format(Locale.ENGLISH, "%s (%d)", secondPointName, cnt + 1)};
		}

		/// <summary>
		/// Dialogs * </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void checkNumberOfObersvationsPerUnknownParameter() throws SQLException, PointTypeMismatchException, UnderDeterminedPointException
		public virtual void checkNumberOfObersvationsPerUnknownParameter()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sqlCountPointGroups = "SELECT COUNT(\"PointApriori\".\"id\") AS \"number_of_points\", \"PointGroup\".\"type\" FROM \"PointApriori\" JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" WHERE \"PointApriori\".\"enable\" = TRUE AND \"PointGroup\".\"enable\" = TRUE AND \"PointGroup\".\"type\" IN (?,?,?) GROUP BY \"PointGroup\".\"type\" ORDER BY  \"PointGroup\".\"type\" ASC";

			string sqlCountObservations = "SELECT " + "\"UnionTable\".\"name\", SUM(\"UnionTable\".\"number_of_observations\") AS \"number_of_observations\", \"UnionTable\".\"dimension\" FROM ( " + "SELECT" + "\"name\", 0 AS \"number_of_observations\", " + "CAST(CASE WHEN \"VerticalDeflectionGroup\".\"type\" = ? THEN \"PointGroup\".\"dimension\" + 2 ELSE \"PointGroup\".\"dimension\" END AS INTEGER) AS \"dimension\" " + "FROM \"PointApriori\" " + "JOIN \"PointGroup\" ON \"PointGroup\".\"id\" = \"PointApriori\".\"group_id\" " + "LEFT JOIN \"VerticalDeflectionApriori\" ON \"VerticalDeflectionApriori\".\"name\" = \"PointApriori\".\"name\" AND \"VerticalDeflectionApriori\".\"enable\" = TRUE " + "LEFT JOIN \"VerticalDeflectionGroup\" ON \"VerticalDeflectionApriori\".\"group_id\" = \"VerticalDeflectionGroup\".\"id\"  AND \"VerticalDeflectionGroup\".\"enable\" = TRUE " + "WHERE \"PointGroup\".\"enable\" = TRUE AND \"PointApriori\".\"enable\" = TRUE " + "AND \"PointGroup\".\"type\" IN (?, ?) " + "UNION ALL " + "SELECT " + "\"start_point_name\" AS \"name\", COUNT(\"start_point_name\") AS \"number_of_observations\", " + "CAST(CASE WHEN \"VerticalDeflectionGroup\".\"type\" = ? THEN \"StartPointGroup\".\"dimension\" + 2 ELSE \"StartPointGroup\".\"dimension\" END AS INTEGER) AS \"dimension\" " + "FROM \"ObservationApriori\" " + "JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "JOIN \"PointApriori\" AS \"StartPointApriori\" ON \"ObservationApriori\".\"start_point_name\" = \"StartPointApriori\".\"name\" " + "JOIN \"PointApriori\" AS \"EndPointApriori\" ON \"ObservationApriori\".\"end_point_name\" = \"EndPointApriori\".\"name\" " + "JOIN \"PointGroup\" AS \"StartPointGroup\" ON \"StartPointGroup\".\"id\" = \"StartPointApriori\".\"group_id\" " + "JOIN \"PointGroup\" AS \"EndPointGroup\" ON \"EndPointGroup\".\"id\" = \"EndPointApriori\".\"group_id\" " + "LEFT JOIN \"VerticalDeflectionApriori\" ON \"VerticalDeflectionApriori\".\"name\" = \"StartPointApriori\".\"name\" AND \"VerticalDeflectionApriori\".\"enable\" = TRUE " + "LEFT JOIN \"VerticalDeflectionGroup\" ON \"VerticalDeflectionApriori\".\"group_id\" = \"VerticalDeflectionGroup\".\"id\"  AND \"VerticalDeflectionGroup\".\"enable\" = TRUE " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"StartPointApriori\".\"enable\" = TRUE AND \"EndPointApriori\".\"enable\" = TRUE AND \"StartPointGroup\".\"enable\" = TRUE AND \"EndPointGroup\".\"enable\" = TRUE " + "AND \"StartPointGroup\".\"type\" IN (?, ?) " + "GROUP BY \"start_point_name\", \"StartPointGroup\".\"dimension\", \"VerticalDeflectionGroup\".\"type\" " + "UNION ALL " + "SELECT " + "\"end_point_name\" AS \"name\", COUNT(\"end_point_name\") AS \"number_of_observations\", " + "CAST(CASE WHEN \"VerticalDeflectionGroup\".\"type\" = ? THEN \"EndPointGroup\".\"dimension\" + 2 ELSE \"EndPointGroup\".\"dimension\" END AS INTEGER) AS \"dimension\" " + "FROM \"ObservationApriori\" " + "JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "JOIN \"PointApriori\" AS \"StartPointApriori\" ON \"ObservationApriori\".\"start_point_name\" = \"StartPointApriori\".\"name\" " + "JOIN \"PointApriori\" AS \"EndPointApriori\" ON \"ObservationApriori\".\"end_point_name\" = \"EndPointApriori\".\"name\" " + "JOIN \"PointGroup\" AS \"StartPointGroup\" ON \"StartPointGroup\".\"id\" = \"StartPointApriori\".\"group_id\" " + "JOIN \"PointGroup\" AS \"EndPointGroup\" ON \"EndPointGroup\".\"id\" = \"EndPointApriori\".\"group_id\" " + "LEFT JOIN \"VerticalDeflectionApriori\" ON \"VerticalDeflectionApriori\".\"name\" = \"EndPointApriori\".\"name\" AND \"VerticalDeflectionApriori\".\"enable\" = TRUE " + "LEFT JOIN \"VerticalDeflectionGroup\" ON \"VerticalDeflectionApriori\".\"group_id\" = \"VerticalDeflectionGroup\".\"id\"  AND \"VerticalDeflectionGroup\".\"enable\" = TRUE " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"StartPointApriori\".\"enable\" = TRUE AND \"EndPointApriori\".\"enable\" = TRUE AND \"StartPointGroup\".\"enable\" = TRUE AND \"EndPointGroup\".\"enable\" = TRUE " + "AND \"EndPointGroup\".\"type\" IN (?, ?) " + "GROUP BY \"end_point_name\", \"EndPointGroup\".\"dimension\", \"VerticalDeflectionGroup\".\"type\" " + "UNION ALL " + "SELECT " + "\"start_point_name\" AS \"name\", CAST(CASE WHEN \"ObservationGroup\".\"type\" = ? THEN 3*COUNT(\"start_point_name\") WHEN \"ObservationGroup\".\"type\" = ? THEN 2*COUNT(\"start_point_name\") ELSE COUNT(\"start_point_name\") END AS INTEGER) AS \"number_of_observations\", " + "CAST(CASE WHEN \"VerticalDeflectionGroup\".\"type\" = ? THEN \"StartPointGroup\".\"dimension\" + 2 ELSE \"StartPointGroup\".\"dimension\" END AS INTEGER) AS \"dimension\" " + "FROM \"GNSSObservationApriori\" " + "JOIN \"ObservationGroup\" ON \"GNSSObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "JOIN \"PointApriori\" AS \"StartPointApriori\" ON \"GNSSObservationApriori\".\"start_point_name\" = \"StartPointApriori\".\"name\" " + "JOIN \"PointApriori\" AS \"EndPointApriori\" ON \"GNSSObservationApriori\".\"end_point_name\" = \"EndPointApriori\".\"name\" " + "JOIN \"PointGroup\" AS \"StartPointGroup\" ON \"StartPointGroup\".\"id\" = \"StartPointApriori\".\"group_id\" " + "JOIN \"PointGroup\" AS \"EndPointGroup\" ON \"EndPointGroup\".\"id\" = \"EndPointApriori\".\"group_id\" " + "LEFT JOIN \"VerticalDeflectionApriori\" ON \"VerticalDeflectionApriori\".\"name\" = \"StartPointApriori\".\"name\" AND \"VerticalDeflectionApriori\".\"enable\" = TRUE " + "LEFT JOIN \"VerticalDeflectionGroup\" ON \"VerticalDeflectionApriori\".\"group_id\" = \"VerticalDeflectionGroup\".\"id\"  AND \"VerticalDeflectionGroup\".\"enable\" = TRUE " + "WHERE \"GNSSObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"StartPointApriori\".\"enable\" = TRUE AND \"EndPointApriori\".\"enable\" = TRUE AND \"StartPointGroup\".\"enable\" = TRUE AND \"EndPointGroup\".\"enable\" = TRUE " + "AND \"StartPointGroup\".\"type\" IN (?, ?) " + "GROUP BY \"start_point_name\", \"StartPointGroup\".\"dimension\", \"ObservationGroup\".\"type\", \"VerticalDeflectionGroup\".\"type\" " + "UNION ALL " + "SELECT " + "\"end_point_name\" AS \"name\", CAST(CASE WHEN \"ObservationGroup\".\"type\" = ? THEN 3*COUNT(\"end_point_name\") WHEN \"ObservationGroup\".\"type\" = ? THEN 2*COUNT(\"end_point_name\") ELSE COUNT(\"end_point_name\") END AS INTEGER) AS \"number_of_observations\", " + "CAST(CASE WHEN \"VerticalDeflectionGroup\".\"type\" = ? THEN \"EndPointGroup\".\"dimension\" + 2 ELSE \"EndPointGroup\".\"dimension\" END AS INTEGER) AS \"dimension\" " + "FROM \"GNSSObservationApriori\" " + "JOIN \"ObservationGroup\" ON \"GNSSObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "JOIN \"PointApriori\" AS \"StartPointApriori\" ON \"GNSSObservationApriori\".\"start_point_name\" = \"StartPointApriori\".\"name\" " + "JOIN \"PointApriori\" AS \"EndPointApriori\" ON \"GNSSObservationApriori\".\"end_point_name\" = \"EndPointApriori\".\"name\" " + "JOIN \"PointGroup\" AS \"StartPointGroup\" ON \"StartPointGroup\".\"id\" = \"StartPointApriori\".\"group_id\" " + "JOIN \"PointGroup\" AS \"EndPointGroup\" ON \"EndPointGroup\".\"id\" = \"EndPointApriori\".\"group_id\" " + "LEFT JOIN \"VerticalDeflectionApriori\" ON \"VerticalDeflectionApriori\".\"name\" = \"EndPointApriori\".\"name\" AND \"VerticalDeflectionApriori\".\"enable\" = TRUE " + "LEFT JOIN \"VerticalDeflectionGroup\" ON \"VerticalDeflectionApriori\".\"group_id\" = \"VerticalDeflectionGroup\".\"id\"  AND \"VerticalDeflectionGroup\".\"enable\" = TRUE " + "WHERE \"GNSSObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"StartPointApriori\".\"enable\" = TRUE AND \"EndPointApriori\".\"enable\" = TRUE AND \"StartPointGroup\".\"enable\" = TRUE AND \"EndPointGroup\".\"enable\" = TRUE " + "AND \"EndPointGroup\".\"type\" IN (?, ?) " + "GROUP BY \"end_point_name\", \"EndPointGroup\".\"dimension\", \"ObservationGroup\".\"type\", \"VerticalDeflectionGroup\".\"type\" " + ") AS \"UnionTable\" " + "GROUP BY \"UnionTable\".\"name\", \"UnionTable\".\"dimension\" HAVING SUM(\"UnionTable\".\"number_of_observations\") < \"UnionTable\".\"dimension\" ORDER BY \"number_of_observations\", \"UnionTable\".\"name\" ASC LIMIT 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sqlCountPointGroups);
			stmt.setInt(1, PointType.DATUM_POINT.getId());
			stmt.setInt(2, PointType.REFERENCE_POINT.getId());
			stmt.setInt(3, PointType.STOCHASTIC_POINT.getId());

			ResultSet rs = stmt.executeQuery();
			int numberOfDatumPoints = 0, numberOfReferencePoints = 0;

			// Gibt es neben Datumspunkten auch Fest/Anschlusspunkte?
			while (rs.next())
			{
				int cnt = rs.getInt("number_of_points");
				PointType type = PointType.getEnumByValue(rs.getInt("type"));
				if (type == PointType.DATUM_POINT)
				{
					numberOfDatumPoints += cnt;
				}
				else
				{
					numberOfReferencePoints += cnt;
				}
			}

			if (numberOfReferencePoints > 0 && numberOfDatumPoints > 0)
			{
				throw new PointTypeMismatchException("Error, the project contains reference points as well as datum points.");
			}

			// Gibt es Punkte, die weniger Beobachtungen haben als deren 
			// Dimension erfordert? Pruefe nur Neu- und Datumspunkte.

			stmt = this.dataBase.getPreparedStatement(sqlCountObservations);
			int idx = 1;
			// Alle Punkte abfragen, die bestimmt werden muessen
			stmt.setInt(idx++, VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION.getId());
			stmt.setInt(idx++, PointType.DATUM_POINT.getId());
			stmt.setInt(idx++, PointType.NEW_POINT.getId());

			// Alle Standpunkte abfragen und terr. Beobachtungen zaehlen
			stmt.setInt(idx++, VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION.getId());
			stmt.setInt(idx++, PointType.DATUM_POINT.getId());
			stmt.setInt(idx++, PointType.NEW_POINT.getId());

			// Alle Zielpunkte abfragen und terr. Beobachtungen zaehlen
			stmt.setInt(idx++, VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION.getId());
			stmt.setInt(idx++, PointType.DATUM_POINT.getId());
			stmt.setInt(idx++, PointType.NEW_POINT.getId());

			// Alle Standpunkte abfragen und GNSS-Beobachtungen zaehlen
			stmt.setInt(idx++, ObservationType.GNSS3D.getId());
			stmt.setInt(idx++, ObservationType.GNSS2D.getId());
			stmt.setInt(idx++, VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION.getId());
			stmt.setInt(idx++, PointType.DATUM_POINT.getId());
			stmt.setInt(idx++, PointType.NEW_POINT.getId());

			// Alle Zielpunkte abfragen und GNSS-Beobachtungen zaehlen
			stmt.setInt(idx++, ObservationType.GNSS3D.getId());
			stmt.setInt(idx++, ObservationType.GNSS2D.getId());
			stmt.setInt(idx++, VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION.getId());
			stmt.setInt(idx++, PointType.DATUM_POINT.getId());
			stmt.setInt(idx++, PointType.NEW_POINT.getId());

			rs = stmt.executeQuery();

			if (rs.next())
			{
				string pointName = rs.getString("name");
				int dimension = rs.getInt("dimension");
				int numberOfObservations = rs.getInt("number_of_observations");

				throw new UnderDeterminedPointException("Error, the point " + pointName + " of dimension " + dimension + " has not enough observations to be estimable.", pointName, dimension, numberOfObservations);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.util.List<org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow> getCongruentPoints(double snapDistance, boolean include1D, boolean include2D, boolean include3D) throws java.sql.SQLException
		public virtual IList<TerrestrialObservationRow> getCongruentPoints(double snapDistance, bool include1D, bool include2D, bool include3D)
		{
			IList<TerrestrialObservationRow> rows = new List<TerrestrialObservationRow>();

			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return rows;
			}

			if (!include1D && !include2D && !include3D)
			{
				return rows;
			}

			string sql = "SELECT " + "\"point_prio_1\".\"name\" AS \"point_name_1\", \"point_prio_2\".\"name\" AS \"point_name_2\", " + "SQRT( POWER(\"point_post_1\".\"x\" - \"point_post_2\".\"x\",2) + POWER(\"point_post_1\".\"y\" - \"point_post_2\".\"y\",2) + POWER(\"point_post_1\".\"z\" - \"point_post_2\".\"z\", 2) ) AS \"slope_distance\" " + "FROM \"PointApriori\" AS \"point_prio_1\", " + "\"PointApriori\" AS \"point_prio_2\" " + "JOIN \"PointAposteriori\" AS \"point_post_1\" ON " + "\"point_prio_1\".\"id\" = \"point_post_1\".\"id\" AND \"point_prio_1\".\"enable\" = TRUE " + "JOIN \"PointAposteriori\" AS \"point_post_2\" ON " + "\"point_prio_2\".\"id\" = \"point_post_2\".\"id\" AND \"point_prio_2\".\"enable\" = TRUE " + "JOIN \"PointGroup\" AS \"group_1\" ON " + "\"point_prio_1\".\"group_id\" = \"group_1\".\"id\" AND \"group_1\".\"enable\" = TRUE " + "JOIN \"PointGroup\" AS \"group_2\" ON " + "\"point_prio_2\".\"group_id\" = \"group_2\".\"id\" AND \"group_2\".\"enable\" = TRUE AND \"group_2\".\"dimension\" = \"group_1\".\"dimension\" AND \"group_1\".\"dimension\" IN (?,?,?)" + "WHERE \"point_prio_1\".\"id\" < \"point_prio_2\".\"id\" AND " + "SQRT( POWER(\"point_post_1\".\"x\" - \"point_post_2\".\"x\", 2) + POWER(\"point_post_1\".\"y\" - \"point_post_2\".\"y\", 2) + POWER(\"point_post_1\".\"z\" - \"point_post_2\".\"z\", 2) ) < ? " + "ORDER BY \"slope_distance\" ASC LIMIT 15";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, include1D ? 1 : -1);
			stmt.setInt(idx++, include2D ? 2 : -2);
			stmt.setInt(idx++, include3D ? 3 : -3);
			stmt.setDouble(idx++, snapDistance);

			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				string pointNameA = rs.getString("point_name_1");
				string pointNameB = rs.getString("point_name_2");
				double distance = rs.getDouble("slope_distance");

				TerrestrialObservationRow row = new TerrestrialObservationRow();
				row.StartPointName = pointNameA;
				row.EndPointName = pointNameB;
				row.DistanceApriori = distance;
				row.ValueApriori = distance;
				row.ValueAposteriori = distance;

				rows.Add(row);
			}

			return rows;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveFormatterOption(org.applied_geodesy.util.FormatterOptions.FormatterOption option) throws java.sql.SQLException
		public virtual void saveFormatterOption(FormatterOptions.FormatterOption option)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"FormatterOption\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), CAST(? AS INT)) " + ") AS \"vals\" (\"type\",\"unit\",\"digits\") ON \"FormatterOption\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"FormatterOption\".\"unit\"   = \"vals\".\"unit\", " + "\"FormatterOption\".\"digits\" = \"vals\".\"digits\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"type\", " + "\"vals\".\"unit\", " + "\"vals\".\"digits\" ";

			CellValueType type = option.Type;
			Unit unit = option.Unit;
			int digits = option.FractionDigits;

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, type.getId());
			if (unit == null)
			{
				stmt.setNull(idx++, Types.SMALLINT);
			}
			else
			{
				stmt.setInt(idx++, unit.Type.getId());
			}
			stmt.setInt(idx++, digits);
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.adjustment.statistic.TestStatisticDefinition testStatistic) throws java.sql.SQLException
		public virtual void save(TestStatisticDefinition testStatistic)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"TestStatisticDefinition\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"id\", \"type\",\"probability_value\",\"power_of_test\",\"familywise_error_rate\") ON \"TestStatisticDefinition\".\"id\" = \"vals\".\"id\" AND \"TestStatisticDefinition\".\"id\" = 1 " + "WHEN MATCHED THEN UPDATE SET " + "\"TestStatisticDefinition\".\"type\"                  = \"vals\".\"type\", " + "\"TestStatisticDefinition\".\"probability_value\"     = \"vals\".\"probability_value\", " + "\"TestStatisticDefinition\".\"power_of_test\"         = \"vals\".\"power_of_test\", " + "\"TestStatisticDefinition\".\"familywise_error_rate\" = \"vals\".\"familywise_error_rate\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"type\", " + "\"vals\".\"probability_value\", " + "\"vals\".\"power_of_test\", " + "\"vals\".\"familywise_error_rate\" ";


			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, 1);

			if (testStatistic.TestStatisticType == null)
			{
				stmt.setNull(idx++, Types.SMALLINT);
			}
			else
			{
				stmt.setInt(idx++, testStatistic.TestStatisticType.getId());
			}

			stmt.setDouble(idx++, testStatistic.ProbabilityValue);
			stmt.setDouble(idx++, testStatistic.PowerOfTest);

			stmt.setBoolean(idx++, testStatistic.FamilywiseErrorRate);
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.applied_geodesy.adjustment.statistic.TestStatisticDefinition getTestStatisticDefinition() throws java.sql.SQLException
		public virtual TestStatisticDefinition TestStatisticDefinition
		{
			get
			{
				if (this.hasDatabase() && this.dataBase.Open)
				{
    
					string sql = "SELECT " + "\"type\",\"probability_value\",\"power_of_test\",\"familywise_error_rate\" " + "FROM \"TestStatisticDefinition\" " + "WHERE \"id\" = 1 LIMIT 1";
    
					PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
					ResultSet rs = stmt.executeQuery();
    
					if (rs.next())
					{
						TestStatisticType testStatisticType = TestStatisticType.getEnumByValue(rs.getInt("type"));
						if (testStatisticType != null)
						{
							double probabilityValue = rs.getDouble("probability_value");
							double powerOfTest = rs.getDouble("power_of_test");
							bool familywiseErrorRate = rs.getBoolean("familywise_error_rate");
    
							return new TestStatisticDefinition(testStatisticType, probabilityValue, powerOfTest, familywiseErrorRate);
						}
					}
				}
				return new TestStatisticDefinition();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.adjustment.network.observation.reduction.Reduction reduction) throws java.sql.SQLException
		public virtual void save(Reduction reduction)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"ReductionDefinition\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE)) " + ") AS \"vals\" (\"id\", \"projection_type\", \"reference_latitude\", \"reference_longitude\", \"reference_height\", \"major_axis\", \"minor_axis\", \"x0\", \"y0\", \"z0\") ON \"ReductionDefinition\".\"id\" = \"vals\".\"id\" AND \"ReductionDefinition\".\"id\" = 1 " + "WHEN MATCHED THEN UPDATE SET " + "\"ReductionDefinition\".\"projection_type\"     = \"vals\".\"projection_type\", " + "\"ReductionDefinition\".\"reference_latitude\"  = \"vals\".\"reference_latitude\", " + "\"ReductionDefinition\".\"reference_longitude\" = \"vals\".\"reference_longitude\", " + "\"ReductionDefinition\".\"reference_height\"    = \"vals\".\"reference_height\", " + "\"ReductionDefinition\".\"major_axis\"          = \"vals\".\"major_axis\", " + "\"ReductionDefinition\".\"minor_axis\"          = \"vals\".\"minor_axis\", " + "\"ReductionDefinition\".\"x0\"                  = \"vals\".\"x0\", " + "\"ReductionDefinition\".\"y0\"                  = \"vals\".\"y0\", " + "\"ReductionDefinition\".\"z0\"                  = \"vals\".\"z0\"  " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"projection_type\", " + "\"vals\".\"reference_latitude\", " + "\"vals\".\"reference_longitude\", " + "\"vals\".\"reference_height\", " + "\"vals\".\"major_axis\", " + "\"vals\".\"minor_axis\", " + "\"vals\".\"x0\", " + "\"vals\".\"y0\", " + "\"vals\".\"z0\" ";


			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, 1);

			stmt.setInt(idx++, reduction.ProjectionType.getId());
			stmt.setDouble(idx++, reduction.PrincipalPoint.Latitude);
			stmt.setDouble(idx++, reduction.PrincipalPoint.Longitude);
			stmt.setDouble(idx++, reduction.PrincipalPoint.Height);
			stmt.setDouble(idx++, reduction.Ellipsoid.getMajorAxis());
			stmt.setDouble(idx++, reduction.Ellipsoid.getMinorAxis());
			stmt.setDouble(idx++, reduction.PrincipalPoint.X);
			stmt.setDouble(idx++, reduction.PrincipalPoint.Y);
			stmt.setDouble(idx++, reduction.PrincipalPoint.Z);
			stmt.execute();

			this.clearReductionTasks();
			this.saveReductionTasks(reduction);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void clearReductionTasks() throws java.sql.SQLException
		private void clearReductionTasks()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			PreparedStatement stmt = this.dataBase.getPreparedStatement("DELETE FROM \"ReductionTask\" WHERE \"reduction_id\" = 1");
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveReductionTasks(org.applied_geodesy.adjustment.network.observation.reduction.Reduction reduction) throws java.sql.SQLException
		private void saveReductionTasks(Reduction reduction)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			bool hasBatch = false;

			string sql = "INSERT INTO \"ReductionTask\" " + "(\"reduction_id\", \"type\") " + "VALUES (?, ?) ";

			try
			{
				this.dataBase.AutoCommit = false;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

				foreach (ReductionTaskType type in ReductionTaskType.values())
				{
					if (!reduction.applyReductionTask(type))
					{
						continue;
					}

					int idx = 1;

					stmt.setInt(idx++, 1);
					stmt.setInt(idx++, type.getId());

					stmt.addBatch();
					hasBatch = true;
				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public double getAverageThreshold(org.applied_geodesy.adjustment.network.ObservationType type) throws java.sql.SQLException
		public virtual double getAverageThreshold(ObservationType type)
		{
			double value = 0;
			if (this.hasDatabase() && this.dataBase.Open)
			{

				string sql = "SELECT \"value\" " + "FROM \"AverageThreshold\" " + "WHERE \"type\" = ? LIMIT 1";

				int idx = 1;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				stmt.setInt(idx++, type.getId());
				ResultSet rs = stmt.executeQuery();

				if (rs.next())
				{
					value = rs.getDouble("value");
				}
			}
			return value > 0 ? value : DefaultAverageThreshold.getThreshold(type);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.adjustment.network.ObservationType type, double threshold) throws java.sql.SQLException
		public virtual void save(ObservationType type, double threshold)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"AverageThreshold\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS DOUBLE)) " + ") AS \"vals\" (\"type\",\"value\") " + "ON \"AverageThreshold\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"AverageThreshold\".\"value\" = \"vals\".\"value\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"type\", " + "\"vals\".\"value\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, type.getId());
			stmt.setDouble(idx++, threshold > 0 ? threshold : DefaultAverageThreshold.getThreshold(type));
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void load(org.applied_geodesy.adjustment.network.observation.reduction.Reduction reductions) throws java.sql.SQLException
		public virtual void load(Reduction reductions)
		{
			if (this.hasDatabase() && this.dataBase.Open)
			{
				string sql = "SELECT " + "\"projection_type\", " + "\"reference_latitude\", \"reference_longitude\", \"reference_height\", " + "\"major_axis\", \"minor_axis\", " + "\"x0\", \"y0\", \"z0\", \"type\" AS \"task_type\" " + "FROM \"ReductionTask\" " + "RIGHT JOIN \"ReductionDefinition\" " + "ON \"ReductionTask\".\"reduction_id\" = \"ReductionDefinition\".\"id\" " + "WHERE \"ReductionDefinition\".\"id\" = 1";

				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				ResultSet rs = stmt.executeQuery();

				while (rs.next())
				{
					int taskTypeId = rs.getInt("task_type");
					bool hasTaskType = !rs.wasNull();

					ProjectionType projectionType = ProjectionType.getEnumByValue(rs.getInt("projection_type"));
					double referenceLatitude = rs.getDouble("reference_latitude");
					double referenceLongitude = rs.getDouble("reference_longitude");
					double referenceHeight = rs.getDouble("reference_height");
					double majorAxis = rs.getDouble("major_axis");
					double minorAxis = rs.getDouble("minor_axis");
					double x0 = rs.getDouble("x0");
					double y0 = rs.getDouble("y0");
					double z0 = rs.getDouble("z0");

					reductions.ProjectionType = projectionType;
					reductions.Ellipsoid = Ellipsoid.createEllipsoidFromMinorAxis(majorAxis, minorAxis);
					reductions.PrincipalPoint.setCoordinates(x0, y0, z0, referenceLatitude, referenceLongitude, referenceHeight);

					if (hasTaskType)
					{
						ReductionTaskType taskType = ReductionTaskType.getEnumByValue(taskTypeId);
						reductions.addReductionTaskType(taskType);
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(boolean userDefined, org.applied_geodesy.adjustment.network.RankDefect rankDefect) throws java.sql.SQLException
		public virtual void save(bool userDefined, RankDefect rankDefect)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"RankDefect\" USING (VALUES " + "(CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"id\",\"user_defined\",\"ty\",\"tx\",\"tz\",\"ry\",\"rx\",\"rz\",\"sy\",\"sx\",\"sz\",\"my\",\"mx\",\"mz\",\"mxy\",\"mxyz\") " + "ON \"RankDefect\".\"id\" = \"vals\".\"id\" AND \"RankDefect\".\"id\" = 1 " + "WHEN MATCHED THEN UPDATE SET " + "\"RankDefect\".\"user_defined\" = \"vals\".\"user_defined\", " + "\"RankDefect\".\"ty\"           = \"vals\".\"ty\", " + "\"RankDefect\".\"tx\"           = \"vals\".\"tx\", " + "\"RankDefect\".\"tz\"           = \"vals\".\"tz\", " + "\"RankDefect\".\"ry\"           = \"vals\".\"ry\", " + "\"RankDefect\".\"rx\"           = \"vals\".\"rx\", " + "\"RankDefect\".\"rz\"           = \"vals\".\"rz\", " + "\"RankDefect\".\"sy\"           = \"vals\".\"sy\", " + "\"RankDefect\".\"sx\"           = \"vals\".\"sx\", " + "\"RankDefect\".\"sz\"           = \"vals\".\"sz\", " + "\"RankDefect\".\"my\"           = \"vals\".\"my\", " + "\"RankDefect\".\"mx\"           = \"vals\".\"mx\", " + "\"RankDefect\".\"mz\"           = \"vals\".\"mz\", " + "\"RankDefect\".\"mxy\"          = \"vals\".\"mxy\", " + "\"RankDefect\".\"mxyz\"         = \"vals\".\"mxyz\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"user_defined\", " + "\"vals\".\"ty\", " + "\"vals\".\"tx\", " + "\"vals\".\"tz\", " + "\"vals\".\"ry\", " + "\"vals\".\"rx\", " + "\"vals\".\"rz\", " + "\"vals\".\"sy\", " + "\"vals\".\"sx\", " + "\"vals\".\"sz\", " + "\"vals\".\"my\", " + "\"vals\".\"mx\", " + "\"vals\".\"mz\", " + "\"vals\".\"mxy\", " + "\"vals\".\"mxyz\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, 1);
			stmt.setBoolean(idx++, userDefined);

			stmt.setBoolean(idx++, rankDefect.estimateTranslationY());
			stmt.setBoolean(idx++, rankDefect.estimateTranslationX());
			stmt.setBoolean(idx++, rankDefect.estimateTranslationZ());

			stmt.setBoolean(idx++, rankDefect.estimateRotationY());
			stmt.setBoolean(idx++, rankDefect.estimateRotationX());
			stmt.setBoolean(idx++, rankDefect.estimateRotationZ());

			stmt.setBoolean(idx++, rankDefect.estimateShearY());
			stmt.setBoolean(idx++, rankDefect.estimateShearX());
			stmt.setBoolean(idx++, rankDefect.estimateShearZ());

			stmt.setBoolean(idx++, rankDefect.estimateScaleY());
			stmt.setBoolean(idx++, rankDefect.estimateScaleX());
			stmt.setBoolean(idx++, rankDefect.estimateScaleZ());

			stmt.setBoolean(idx++, rankDefect.estimateScaleXY());
			stmt.setBoolean(idx++, rankDefect.estimateScaleXYZ());

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.applied_geodesy.adjustment.network.RankDefect getRankDefectDefinition() throws java.sql.SQLException
		public virtual RankDefect RankDefectDefinition
		{
			get
			{
				RankDefect rankDefect = new RankDefect();
    
				if (this.hasDatabase() && this.dataBase.Open)
				{
    
					string sql = "SELECT " + "\"user_defined\",\"ty\",\"tx\",\"tz\",\"ry\",\"rx\",\"rz\",\"sy\",\"sx\",\"sz\",\"my\",\"mx\",\"mz\",\"mxy\",\"mxyz\" " + "FROM \"RankDefect\" " + "WHERE \"id\" = 1 LIMIT 1";
    
					PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
					ResultSet rs = stmt.executeQuery();
    
					if (rs.next())
					{
						if (rs.getBoolean("user_defined"))
						{
							if (rs.getBoolean("ty"))
							{
								rankDefect.TranslationYDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("tx"))
							{
								rankDefect.TranslationXDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("tz"))
							{
								rankDefect.TranslationZDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("ry"))
							{
								rankDefect.RotationYDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("rx"))
							{
								rankDefect.RotationXDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("rz"))
							{
								rankDefect.RotationZDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("sy"))
							{
								rankDefect.ShearYDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("sx"))
							{
								rankDefect.ShearXDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("sz"))
							{
								rankDefect.ShearZDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("my"))
							{
								rankDefect.ScaleYDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("mx"))
							{
								rankDefect.ScaleXDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("mz"))
							{
								rankDefect.ScaleZDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("mxy"))
							{
								rankDefect.ScaleXYDefectType = DefectType.FREE;
							}
    
							if (rs.getBoolean("mxyz"))
							{
								rankDefect.ScaleXYZDefectType = DefectType.FREE;
							}
						}
					}
				}
				return rankDefect;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void load(org.applied_geodesy.jag3d.ui.dialog.LeastSquaresSettingDialog.LeastSquaresSettings settings) throws java.sql.SQLException
		public virtual void load(LeastSquaresSettings settings)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "SELECT " + "\"type\", \"number_of_iterations\", \"robust_estimation_limit\", " + "\"number_of_principal_components\", \"apply_variance_of_unit_weight\", " + "\"estimate_direction_set_orientation_approximation\", " + "\"congruence_analysis\", \"export_covariance_matrix\", " + "\"scaling\", \"damping\", \"weight_zero\" " + "FROM \"AdjustmentDefinition\" " + "JOIN \"UnscentedTransformation\" " + "ON \"AdjustmentDefinition\".\"id\" = \"UnscentedTransformation\".\"id\" " + "WHERE \"id\" = 1 LIMIT 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				EstimationType type = EstimationType.getEnumByValue(rs.getInt("type"));
				if (type != null)
				{
					settings.EstimationType = type;
					settings.Iteration = rs.getInt("number_of_iterations");
					settings.RobustEstimationLimit = rs.getDouble("robust_estimation_limit");
					settings.PrincipalComponents = rs.getInt("number_of_principal_components");
					settings.ApplyVarianceOfUnitWeight = rs.getBoolean("apply_variance_of_unit_weight");
					settings.Orientation = rs.getBoolean("estimate_direction_set_orientation_approximation");
					settings.CongruenceAnalysis = rs.getBoolean("congruence_analysis");
					settings.ExportCovarianceMatrix = rs.getBoolean("export_covariance_matrix");

					settings.ScalingParameterAlphaUT = rs.getDouble("scaling");
					settings.DampingParameterBetaUT = rs.getDouble("damping");
					settings.WeightZero = rs.getDouble("weight_zero");
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.jag3d.ui.dialog.LeastSquaresSettingDialog.LeastSquaresSettings settings) throws java.sql.SQLException
		public virtual void save(LeastSquaresSettings settings)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"AdjustmentDefinition\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS INT), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"id\", \"type\", \"number_of_iterations\", \"robust_estimation_limit\", \"number_of_principal_components\", \"apply_variance_of_unit_weight\", \"estimate_direction_set_orientation_approximation\", \"congruence_analysis\", \"export_covariance_matrix\") ON \"AdjustmentDefinition\".\"id\" = \"vals\".\"id\" AND \"AdjustmentDefinition\".\"id\" = 1 " + "WHEN MATCHED THEN UPDATE SET " + "\"AdjustmentDefinition\".\"type\"                            = \"vals\".\"type\", " + "\"AdjustmentDefinition\".\"number_of_iterations\"            = \"vals\".\"number_of_iterations\", " + "\"AdjustmentDefinition\".\"robust_estimation_limit\"         = \"vals\".\"robust_estimation_limit\", " + "\"AdjustmentDefinition\".\"number_of_principal_components\"  = \"vals\".\"number_of_principal_components\", " + "\"AdjustmentDefinition\".\"apply_variance_of_unit_weight\"   = \"vals\".\"apply_variance_of_unit_weight\", " + "\"AdjustmentDefinition\".\"estimate_direction_set_orientation_approximation\" = \"vals\".\"estimate_direction_set_orientation_approximation\", " + "\"AdjustmentDefinition\".\"congruence_analysis\"      = \"vals\".\"congruence_analysis\", " + "\"AdjustmentDefinition\".\"export_covariance_matrix\" = \"vals\".\"export_covariance_matrix\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"type\", " + "\"vals\".\"number_of_iterations\", " + "\"vals\".\"robust_estimation_limit\", " + "\"vals\".\"number_of_principal_components\", " + "\"vals\".\"apply_variance_of_unit_weight\", " + "\"vals\".\"estimate_direction_set_orientation_approximation\", " + "\"vals\".\"congruence_analysis\", " + "\"vals\".\"export_covariance_matrix\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			stmt.setInt(idx++, 1); // default ID
			stmt.setInt(idx++, settings.EstimationType.getId());
			stmt.setInt(idx++, settings.Iteration);
			stmt.setDouble(idx++, settings.RobustEstimationLimit);
			stmt.setInt(idx++, settings.PrincipalComponents);

			stmt.setBoolean(idx++, settings.ApplyVarianceOfUnitWeight);
			stmt.setBoolean(idx++, settings.Orientation);
			stmt.setBoolean(idx++, settings.CongruenceAnalysis);
			stmt.setBoolean(idx++, settings.ExportCovarianceMatrix);

			stmt.execute();


			sql = "MERGE INTO \"UnscentedTransformation\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE)) " + ") AS \"vals\" (\"id\", \"scaling\", \"damping\", \"weight_zero\") ON \"UnscentedTransformation\".\"id\" = \"vals\".\"id\" AND \"UnscentedTransformation\".\"id\" = 1 " + "WHEN MATCHED THEN UPDATE SET " + "\"UnscentedTransformation\".\"scaling\"     = \"vals\".\"scaling\", " + "\"UnscentedTransformation\".\"damping\"     = \"vals\".\"damping\", " + "\"UnscentedTransformation\".\"weight_zero\" = \"vals\".\"weight_zero\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"scaling\", " + "\"vals\".\"damping\", " + "\"vals\".\"weight_zero\" ";


			idx = 1;
			stmt = this.dataBase.getPreparedStatement(sql);
			// Insert new item
			stmt.setInt(idx++, 1); // default ID
			stmt.setDouble(idx++, settings.ScalingParameterAlphaUT);
			stmt.setDouble(idx++, settings.DampingParameterBetaUT);
			stmt.setDouble(idx++, settings.WeightZero);

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int adaptInstrumentAndReflectorHeights(String stationNameRegex, String targetNameRegex, System.Nullable<double> instrumentHeight, System.Nullable<double> reflectorHeight, org.applied_geodesy.jag3d.ui.dialog.ScopeType scopeType, org.applied_geodesy.jag3d.ui.tree.TreeItemValue itemValue, org.applied_geodesy.jag3d.ui.tree.TreeItemValue... selectedTreeItemValues) throws java.sql.SQLException
		public virtual int adaptInstrumentAndReflectorHeights(string stationNameRegex, string targetNameRegex, double? instrumentHeight, double? reflectorHeight, ScopeType scopeType, TreeItemValue itemValue, params TreeItemValue[] selectedTreeItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return 0;
			}

			if (instrumentHeight == null && reflectorHeight == null)
			{
				return 0;
			}

			int cnt = 0;
			if (scopeType != ScopeType.SELECTION)
			{
				UITreeBuilder treeBuilder = UITreeBuilder.Instance;
				TreeItemType[] itemTypes = TreeItemType.values();
				foreach (TreeItemType itemType in itemTypes)
				{
					if (TreeItemType.isObservationTypeDirectory(itemType))
					{

						TreeItem<TreeItemValue> parent = treeBuilder.getDirectoryItemByType(itemType);

						if (parent != null && !parent.getChildren().isEmpty())
						{
							IList<TreeItem<TreeItemValue>> items = parent.getChildren();
							TreeItemValue[] itemValues = new TreeItemValue[items.Count];
							for (int i = 0; i < itemValues.Length; i++)
							{
								itemValues[i] = items[i].getValue();
							}

							cnt += this.scopedInstrumentAndReflectorHeightsAdaption(scopeType, stationNameRegex, targetNameRegex, instrumentHeight, reflectorHeight, itemValues[0], itemValues);
						}
					}
				}

				IList<TreeItem<TreeItemValue>> selectedItems = new List<TreeItem<TreeItemValue>>(treeBuilder.Tree.getSelectionModel().getSelectedItems());
				if (selectedItems != null && selectedItems.Count > 0)
				{
					treeBuilder.Tree.getSelectionModel().clearSelection();
					foreach (TreeItem<TreeItemValue> selectedItem in selectedItems)
					{
						treeBuilder.Tree.getSelectionModel().select(selectedItem);
					}
				}
			}
			else
			{
				cnt += this.scopedInstrumentAndReflectorHeightsAdaption(scopeType, stationNameRegex, targetNameRegex, instrumentHeight, reflectorHeight, itemValue, selectedTreeItemValues);
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int scopedInstrumentAndReflectorHeightsAdaption(org.applied_geodesy.jag3d.ui.dialog.ScopeType scopeType, String stationNameRegex, String targetNameRegex, System.Nullable<double> instrumentHeight, System.Nullable<double> reflectorHeight, org.applied_geodesy.jag3d.ui.tree.TreeItemValue itemValue, org.applied_geodesy.jag3d.ui.tree.TreeItemValue... selectedTreeItemValues) throws java.sql.SQLException
		private int scopedInstrumentAndReflectorHeightsAdaption(ScopeType scopeType, string stationNameRegex, string targetNameRegex, double? instrumentHeight, double? reflectorHeight, TreeItemValue itemValue, params TreeItemValue[] selectedTreeItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return 0;
			}

			int cnt = 0;
			TreeItemType treeItemType = itemValue.ItemType;

			switch (treeItemType.innerEnumValue)
			{
			case TreeItemType.InnerEnum.LEVELING_LEAF:
			case TreeItemType.InnerEnum.DIRECTION_LEAF:
			case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				if (itemValue is ObservationTreeItemValue)
				{
					ObservationTreeItemValue observationItemValue = (ObservationTreeItemValue)itemValue;
					ObservationTreeItemValue[] selectedObservationItemValuesArray = null;
					ISet<ObservationTreeItemValue> selectedObservationItemValues = new LinkedHashSet<ObservationTreeItemValue>();
					selectedObservationItemValues.Add(observationItemValue);

					if (selectedTreeItemValues != null)
					{
						foreach (TreeItemValue selectedItem in selectedTreeItemValues)
						{
							if (selectedItem is ObservationTreeItemValue)
							{
								selectedObservationItemValues.Add((ObservationTreeItemValue)selectedItem);
							}
						}
					}
					selectedObservationItemValuesArray = selectedObservationItemValues.ToArray();

					StringBuilder sql = new StringBuilder("UPDATE \"ObservationApriori\" SET ");

					if (instrumentHeight != null)
					{
						sql.Append("\"instrument_height\" = ? ");
						if (reflectorHeight != null)
						{
							sql.Append(", ");
						}
					}
					if (reflectorHeight != null)
					{
						sql.Append("\"reflector_height\" = ? ");
					}

					sql.Append("WHERE REGEXP_MATCHES(\"start_point_name\", ?) AND REGEXP_MATCHES(\"end_point_name\", ?) AND \"group_id\" = ? AND (SELECT \"reference_epoch\" FROM \"ObservationGroup\" WHERE \"id\" = ?) IN (?,?) ");

					foreach (ObservationTreeItemValue observationTreeItemValue in selectedObservationItemValues)
					{
						PreparedStatement stmt = this.dataBase.getPreparedStatement(sql.ToString());
						int idx = 1;

						if (instrumentHeight != null)
						{
							stmt.setDouble(idx++, instrumentHeight.Value);
						}

						if (reflectorHeight != null)
						{
							stmt.setDouble(idx++, reflectorHeight.Value);
						}

						stmt.setString(idx++, stationNameRegex);
						stmt.setString(idx++, targetNameRegex);

						stmt.setInt(idx++, observationTreeItemValue.GroupId);
						stmt.setInt(idx++, observationTreeItemValue.GroupId);

						stmt.setBoolean(idx++, scopeType == ScopeType.REFERENCE_EPOCH);
						stmt.setBoolean(idx++, scopeType != ScopeType.CONTROL_EPOCH);

						cnt += stmt.executeUpdate();
					}
					this.loadObservations(observationItemValue, selectedObservationItemValuesArray);
				}
				break;

			default:
				break;
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int searchAndReplacePointNames(String searchRegex, String replaceRegex, org.applied_geodesy.jag3d.ui.dialog.ScopeType scopeType, org.applied_geodesy.jag3d.ui.tree.TreeItemValue itemValue, org.applied_geodesy.jag3d.ui.tree.TreeItemValue... selectedTreeItemValues) throws java.sql.SQLException
		public virtual int searchAndReplacePointNames(string searchRegex, string replaceRegex, ScopeType scopeType, TreeItemValue itemValue, params TreeItemValue[] selectedTreeItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return 0;
			}

			int cnt = 0;
			if (scopeType != ScopeType.SELECTION)
			{
				UITreeBuilder treeBuilder = UITreeBuilder.Instance;
				TreeItemType[] itemTypes = TreeItemType.values();
				foreach (TreeItemType itemType in itemTypes)
				{
					if (TreeItemType.isPointTypeDirectory(itemType) && scopeType == ScopeType.PROJECT || TreeItemType.isObservationTypeDirectory(itemType) || TreeItemType.isGNSSObservationTypeDirectory(itemType) || TreeItemType.isVerticalDeflectionTypeDirectory(itemType) && scopeType == ScopeType.PROJECT || TreeItemType.isCongruenceAnalysisTypeDirectory(itemType) && scopeType == ScopeType.PROJECT)
					{

						TreeItem<TreeItemValue> parent = treeBuilder.getDirectoryItemByType(itemType);

						if (parent != null && !parent.getChildren().isEmpty())
						{
							IList<TreeItem<TreeItemValue>> items = parent.getChildren();
							TreeItemValue[] itemValues = new TreeItemValue[items.Count];
							for (int i = 0; i < itemValues.Length; i++)
							{
								itemValues[i] = items[i].getValue();
							}

							cnt += this.scopedSearchAndReplacePointNames(scopeType, searchRegex, replaceRegex, itemValues[0], itemValues);
						}
					}
				}

				IList<TreeItem<TreeItemValue>> selectedItems = new List<TreeItem<TreeItemValue>>(treeBuilder.Tree.getSelectionModel().getSelectedItems());
				if (selectedItems != null && selectedItems.Count > 0)
				{
					treeBuilder.Tree.getSelectionModel().clearSelection();
					foreach (TreeItem<TreeItemValue> selectedItem in selectedItems)
					{
						treeBuilder.Tree.getSelectionModel().select(selectedItem);
					}
				}
			}
			else
			{
				cnt += this.scopedSearchAndReplacePointNames(scopeType, searchRegex, replaceRegex, itemValue, selectedTreeItemValues);
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int scopedSearchAndReplacePointNames(org.applied_geodesy.jag3d.ui.dialog.ScopeType scopeType, String searchRegex, String replaceRegex, org.applied_geodesy.jag3d.ui.tree.TreeItemValue itemValue, org.applied_geodesy.jag3d.ui.tree.TreeItemValue... selectedTreeItemValues) throws java.sql.SQLException
		private int scopedSearchAndReplacePointNames(ScopeType scopeType, string searchRegex, string replaceRegex, TreeItemValue itemValue, params TreeItemValue[] selectedTreeItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return 0;
			}

			int cnt = 0;
			TreeItemType treeItemType = itemValue.ItemType;

			switch (treeItemType.innerEnumValue)
			{
			case TreeItemType.InnerEnum.REFERENCE_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.REFERENCE_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.REFERENCE_POINT_3D_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
			case TreeItemType.InnerEnum.DATUM_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.DATUM_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.DATUM_POINT_3D_LEAF:
			case TreeItemType.InnerEnum.NEW_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.NEW_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.NEW_POINT_3D_LEAF:
				if (itemValue is PointTreeItemValue)
				{
					PointTreeItemValue pointItemValue = (PointTreeItemValue)itemValue;
					PointTreeItemValue[] selectedPointItemValuesArray = null;
					ISet<PointTreeItemValue> selectedPointItemValues = new LinkedHashSet<PointTreeItemValue>();
					selectedPointItemValues.Add(pointItemValue);

					if (selectedTreeItemValues != null)
					{
						foreach (TreeItemValue selectedItem in selectedTreeItemValues)
						{
							if (selectedItem is PointTreeItemValue)
							{
								selectedPointItemValues.Add((PointTreeItemValue)selectedItem);
							}
						}
					}
					selectedPointItemValuesArray = selectedPointItemValues.ToArray();
					cnt += this.scopedSearchAndReplacePointNames(scopeType, searchRegex, replaceRegex, selectedPointItemValuesArray);
					this.loadPoints(pointItemValue, selectedPointItemValuesArray);
				}
				break;

			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
				if (itemValue is CongruenceAnalysisTreeItemValue)
				{
					CongruenceAnalysisTreeItemValue congruenceAnalysisTreeItemValue = (CongruenceAnalysisTreeItemValue)itemValue;
					CongruenceAnalysisTreeItemValue[] selectedCongruenceAnalysisItemValuesArray = null;
					ISet<CongruenceAnalysisTreeItemValue> selectedCongruenceAnalysisItemValues = new LinkedHashSet<CongruenceAnalysisTreeItemValue>();
					selectedCongruenceAnalysisItemValues.Add(congruenceAnalysisTreeItemValue);

					if (selectedTreeItemValues != null)
					{
						foreach (TreeItemValue selectedItem in selectedTreeItemValues)
						{
							if (selectedItem is CongruenceAnalysisTreeItemValue)
							{
								selectedCongruenceAnalysisItemValues.Add((CongruenceAnalysisTreeItemValue)selectedItem);
							}
						}
					}
					selectedCongruenceAnalysisItemValuesArray = selectedCongruenceAnalysisItemValues.ToArray();
					cnt += this.scopedSearchAndReplacePointNames(scopeType, searchRegex, replaceRegex, selectedCongruenceAnalysisItemValuesArray);
					this.loadCongruenceAnalysisPointPair(congruenceAnalysisTreeItemValue, selectedCongruenceAnalysisItemValuesArray);
				}
				break;

			case TreeItemType.InnerEnum.LEVELING_LEAF:
			case TreeItemType.InnerEnum.DIRECTION_LEAF:
			case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
			case TreeItemType.InnerEnum.GNSS_1D_LEAF:
			case TreeItemType.InnerEnum.GNSS_2D_LEAF:
			case TreeItemType.InnerEnum.GNSS_3D_LEAF:
				if (itemValue is ObservationTreeItemValue)
				{
					ObservationTreeItemValue observationItemValue = (ObservationTreeItemValue)itemValue;
					ObservationTreeItemValue[] selectedObservationItemValuesArray = null;
					ISet<ObservationTreeItemValue> selectedObservationItemValues = new LinkedHashSet<ObservationTreeItemValue>();
					selectedObservationItemValues.Add(observationItemValue);

					if (selectedTreeItemValues != null)
					{
						foreach (TreeItemValue selectedItem in selectedTreeItemValues)
						{
							if (selectedItem is ObservationTreeItemValue)
							{
								selectedObservationItemValues.Add((ObservationTreeItemValue)selectedItem);
							}
						}
					}
					selectedObservationItemValuesArray = selectedObservationItemValues.ToArray();
					cnt += this.scopedSearchAndReplacePointNames(scopeType, searchRegex, replaceRegex, selectedObservationItemValuesArray);
					if (TreeItemType.isObservationTypeLeaf(treeItemType))
					{
						this.loadObservations(observationItemValue, selectedObservationItemValuesArray);
					}
					else if (TreeItemType.isGNSSObservationTypeLeaf(treeItemType))
					{
						this.loadGNSSObservations(observationItemValue, selectedObservationItemValuesArray);
					}
				}
				break;

			case TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_LEAF:
			case TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_LEAF:
				if (itemValue is VerticalDeflectionTreeItemValue)
				{
					VerticalDeflectionTreeItemValue verticalDeflectionItemValue = (VerticalDeflectionTreeItemValue)itemValue;
					VerticalDeflectionTreeItemValue[] selectedVerticalDeflectionItemValuesArray = null;
					ISet<VerticalDeflectionTreeItemValue> selectedVerticalDeflectionItemValues = new LinkedHashSet<VerticalDeflectionTreeItemValue>();
					selectedVerticalDeflectionItemValues.Add(verticalDeflectionItemValue);

					if (selectedTreeItemValues != null)
					{
						foreach (TreeItemValue selectedItem in selectedTreeItemValues)
						{
							if (selectedItem is VerticalDeflectionTreeItemValue)
							{
								selectedVerticalDeflectionItemValues.Add((VerticalDeflectionTreeItemValue)selectedItem);
							}
						}
					}
					selectedVerticalDeflectionItemValuesArray = selectedVerticalDeflectionItemValues.ToArray();
					cnt += this.scopedSearchAndReplacePointNames(scopeType, searchRegex, replaceRegex, selectedVerticalDeflectionItemValuesArray);
					this.loadVerticalDeflections(verticalDeflectionItemValue, selectedVerticalDeflectionItemValuesArray);
				}
				break;

			default:
				break;
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int scopedSearchAndReplacePointNames(org.applied_geodesy.jag3d.ui.dialog.ScopeType scopeType, String searchRegex, String replaceRegex, org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue... selectedTreeItemValues) throws java.sql.SQLException
		private int scopedSearchAndReplacePointNames(ScopeType scopeType, string searchRegex, string replaceRegex, params VerticalDeflectionTreeItemValue[] selectedTreeItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return 0;
			}

			string sql = "SELECT \"name\" FROM \"VerticalDeflectionApriori\" WHERE REGEXP_MATCHES(\"name\", ?) AND \"group_id\" = ?";

			int cnt = 0;
			foreach (VerticalDeflectionTreeItemValue verticalDeflectionTreeItemValue in selectedTreeItemValues)
			{
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				int idx = 1;
				stmt.setString(idx++, searchRegex);
				stmt.setInt(idx++, verticalDeflectionTreeItemValue.GroupId);

				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					string oldName = rs.getString("name");
					string newName = oldName.replaceFirst(searchRegex, replaceRegex);
					newName = newName.Substring(0, Math.Min(newName.Length, 255));

					if (this.isNameCollisions(verticalDeflectionTreeItemValue, newName))
					{
						continue;
					}

					cnt += this.replacePointName(verticalDeflectionTreeItemValue, oldName, newName);
				}
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int scopedSearchAndReplacePointNames(org.applied_geodesy.jag3d.ui.dialog.ScopeType scopeType, String searchRegex, String replaceRegex, org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue... selectedTreeItemValues) throws java.sql.SQLException
		private int scopedSearchAndReplacePointNames(ScopeType scopeType, string searchRegex, string replaceRegex, params PointTreeItemValue[] selectedTreeItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return 0;
			}

			string sql = "SELECT \"name\" FROM \"PointApriori\" WHERE REGEXP_MATCHES(\"name\", ?) AND \"group_id\" = ?";

			int cnt = 0;
			foreach (PointTreeItemValue pointTreeItemValue in selectedTreeItemValues)
			{
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				int idx = 1;
				stmt.setString(idx++, searchRegex);
				stmt.setInt(idx++, pointTreeItemValue.GroupId);

				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					string oldName = rs.getString("name");
					string newName = oldName.replaceFirst(searchRegex, replaceRegex);
					newName = newName.Substring(0, Math.Min(newName.Length, 255));

					if (this.isNameCollisions(pointTreeItemValue, newName))
					{
						continue;
					}

					cnt += this.replacePointName(pointTreeItemValue, oldName, newName);
				}
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int scopedSearchAndReplacePointNames(org.applied_geodesy.jag3d.ui.dialog.ScopeType scopeType, String searchRegex, String replaceRegex, org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue... selectedTreeItemValues) throws java.sql.SQLException
		private int scopedSearchAndReplacePointNames(ScopeType scopeType, string searchRegex, string replaceRegex, params ObservationTreeItemValue[] selectedTreeItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return 0;
			}

			string sql = "SELECT " + "DISTINCT \"end_point_name\" AS \"name\" FROM \"%s\" JOIN \"ObservationGroup\" ON \"group_id\" = \"ObservationGroup\".\"id\" WHERE \"reference_epoch\" IN (?,?) AND REGEXP_MATCHES(\"end_point_name\", ?) AND \"group_id\" = ? " + "UNION ALL " + "SELECT " + "DISTINCT \"start_point_name\" AS \"name\" FROM \"%s\" JOIN \"ObservationGroup\" ON \"group_id\" = \"ObservationGroup\".\"id\" WHERE \"reference_epoch\" IN (?,?) AND REGEXP_MATCHES(\"start_point_name\", ?) AND \"group_id\" = ?";

			int cnt = 0;
			foreach (ObservationTreeItemValue observationTreeItemValue in selectedTreeItemValues)
			{
				string tableName = TreeItemType.isObservationTypeLeaf(observationTreeItemValue.ItemType) ? "ObservationApriori" : "GNSSObservationApriori";
				PreparedStatement stmt = this.dataBase.getPreparedStatement(String.format(sql, tableName, tableName));
				int idx = 1;
	//			stmt.setBoolean(idx++,   scopeType == ScopeType.SELECTION || scopeType == ScopeType.PROJECT || scopeType == ScopeType.REFERENCE_EPOCH);
	//			stmt.setBoolean(idx++, !(scopeType == ScopeType.SELECTION || scopeType == ScopeType.PROJECT || scopeType == ScopeType.CONTROL_EPOCH));
				stmt.setBoolean(idx++, scopeType == ScopeType.REFERENCE_EPOCH);
				stmt.setBoolean(idx++, scopeType != ScopeType.CONTROL_EPOCH);
				stmt.setString(idx++, searchRegex);
				stmt.setInt(idx++, observationTreeItemValue.GroupId);

				stmt.setBoolean(idx++, scopeType == ScopeType.REFERENCE_EPOCH);
				stmt.setBoolean(idx++, scopeType != ScopeType.CONTROL_EPOCH);
				stmt.setString(idx++, searchRegex);
				stmt.setInt(idx++, observationTreeItemValue.GroupId);

				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					string oldName = rs.getString("name");
					string newName = oldName.replaceFirst(searchRegex, replaceRegex);
					newName = newName.Substring(0, Math.Min(newName.Length, 255));

					if (this.isNameCollisions(observationTreeItemValue, oldName, newName))
					{
						continue;
					}

					cnt += this.replacePointName(observationTreeItemValue, oldName, newName);
				}
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int scopedSearchAndReplacePointNames(org.applied_geodesy.jag3d.ui.dialog.ScopeType scopeType, String searchRegex, String replaceRegex, org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue... selectedTreeItemValues) throws java.sql.SQLException
		private int scopedSearchAndReplacePointNames(ScopeType scopeType, string searchRegex, string replaceRegex, params CongruenceAnalysisTreeItemValue[] selectedTreeItemValues)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return 0;
			}

			string sql = "SELECT " + "DISTINCT \"end_point_name\" AS \"name\" FROM \"CongruenceAnalysisPointPairApriori\" WHERE REGEXP_MATCHES(\"end_point_name\", ?) AND \"group_id\" = ? " + "UNION ALL " + "SELECT " + "DISTINCT \"start_point_name\" AS \"name\" FROM \"CongruenceAnalysisPointPairApriori\" WHERE REGEXP_MATCHES(\"start_point_name\", ?) AND \"group_id\" = ?";

			int cnt = 0;
			foreach (CongruenceAnalysisTreeItemValue congruenceAnalysisTreeItemValue in selectedTreeItemValues)
			{
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				int idx = 1;
				stmt.setString(idx++, searchRegex);
				stmt.setInt(idx++, congruenceAnalysisTreeItemValue.GroupId);
				stmt.setString(idx++, searchRegex);
				stmt.setInt(idx++, congruenceAnalysisTreeItemValue.GroupId);

				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					string oldName = rs.getString("name");
					string newName = oldName.replaceFirst(searchRegex, replaceRegex);
					newName = newName.Substring(0, Math.Min(newName.Length, 255));

					if (this.isNameCollisions(congruenceAnalysisTreeItemValue, oldName, newName))
					{
						continue;
					}

					cnt += this.replacePointName(congruenceAnalysisTreeItemValue, oldName, newName);
				}
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int replacePointName(org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue verticalDeflectionTreeItemValue, String oldName, String newName) throws java.sql.SQLException
		private int replacePointName(VerticalDeflectionTreeItemValue verticalDeflectionTreeItemValue, string oldName, string newName)
		{
			string sql = "UPDATE \"VerticalDeflectionApriori\" SET \"name\" = ? WHERE \"name\" = ? AND \"group_id\" = ?";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setString(idx++, newName);
			stmt.setString(idx++, oldName);
			stmt.setInt(idx++, verticalDeflectionTreeItemValue.GroupId);
			return stmt.executeUpdate();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int replacePointName(org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue pointTreeItemValue, String oldName, String newName) throws java.sql.SQLException
		private int replacePointName(PointTreeItemValue pointTreeItemValue, string oldName, string newName)
		{
			string sql = "UPDATE \"PointApriori\" SET \"name\" = ? WHERE \"name\" = ? AND \"group_id\" = ?";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setString(idx++, newName);
			stmt.setString(idx++, oldName);
			stmt.setInt(idx++, pointTreeItemValue.GroupId);
			return stmt.executeUpdate();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int replacePointName(org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue observationTreeItemValue, String oldName, String newName) throws java.sql.SQLException
		private int replacePointName(ObservationTreeItemValue observationTreeItemValue, string oldName, string newName)
		{
			string[] sqls = new string[] {"UPDATE \"%s\" SET \"start_point_name\" = ? WHERE \"start_point_name\" = ? AND \"group_id\" = ?", "UPDATE \"%s\" SET \"end_point_name\"   = ? WHERE \"end_point_name\"   = ? AND \"group_id\" = ?"};
			int cnt = 0;
			string tableName = TreeItemType.isObservationTypeLeaf(observationTreeItemValue.ItemType) ? "ObservationApriori" : "GNSSObservationApriori";
			foreach (string sql in sqls)
			{
				PreparedStatement stmt = this.dataBase.getPreparedStatement(String.format(sql, tableName));
				int idx = 1;
				stmt.setString(idx++, newName);
				stmt.setString(idx++, oldName);
				stmt.setInt(idx++, observationTreeItemValue.GroupId);
				cnt += stmt.executeUpdate();
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int replacePointName(org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue congruenceAnalysisTreeItemValue, String oldName, String newName) throws java.sql.SQLException
		private int replacePointName(CongruenceAnalysisTreeItemValue congruenceAnalysisTreeItemValue, string oldName, string newName)
		{
			string[] sqls = new string[] {"UPDATE \"CongruenceAnalysisPointPairApriori\" SET \"start_point_name\" = ? WHERE \"start_point_name\" = ? AND \"group_id\" = ?", "UPDATE \"CongruenceAnalysisPointPairApriori\" SET \"end_point_name\"   = ? WHERE \"end_point_name\"   = ? AND \"group_id\" = ?"};
			int cnt = 0;
			foreach (string sql in sqls)
			{
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				int idx = 1;
				stmt.setString(idx++, newName);
				stmt.setString(idx++, oldName);
				stmt.setInt(idx++, congruenceAnalysisTreeItemValue.GroupId);
				cnt += stmt.executeUpdate();
			}
			return cnt;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private boolean isNameCollisions(org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue verticalDeflectionTreeItemValue, String proposedName) throws java.sql.SQLException
		private bool isNameCollisions(VerticalDeflectionTreeItemValue verticalDeflectionTreeItemValue, string proposedName)
		{
			string sql = "SELECT COUNT(\"id\") > 0 AS \"collisions\" FROM \"VerticalDeflectionApriori\" WHERE \"name\" = ? AND \"group_id\" = ?";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setString(idx++, proposedName);
			stmt.setInt(idx++, verticalDeflectionTreeItemValue.GroupId);
			ResultSet rs = stmt.executeQuery();

			bool collisions = false;
			if (rs.next())
			{
				collisions = rs.getBoolean("collisions");
			}
			return collisions;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private boolean isNameCollisions(org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue pointTreeItemValue, String proposedName) throws java.sql.SQLException
		private bool isNameCollisions(PointTreeItemValue pointTreeItemValue, string proposedName)
		{
			string sql = "SELECT COUNT(\"id\") > 0 AS \"collisions\" FROM \"PointApriori\" WHERE \"name\" = ?"; // AND \"group_id\" = ?";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setString(idx++, proposedName);
			//stmt.setInt(idx++, pointTreeItemValue.getGroupId());
			ResultSet rs = stmt.executeQuery();

			bool collisions = false;
			if (rs.next())
			{
				collisions = rs.getBoolean("collisions");
			}
			return collisions;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private boolean isNameCollisions(org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue observationTreeItemValue, String oldName, String newName) throws java.sql.SQLException
		private bool isNameCollisions(ObservationTreeItemValue observationTreeItemValue, string oldName, string newName)
		{
			string sql = "SELECT COUNT(\"id\") > 0 AS \"collisions\" FROM \"%s\" " + "WHERE ((\"start_point_name\" = ? AND \"end_point_name\" = ?) OR (\"start_point_name\" = ? AND \"end_point_name\" = ?)) " + "AND \"group_id\" = ?";
			string tableName = TreeItemType.isObservationTypeLeaf(observationTreeItemValue.ItemType) ? "ObservationApriori" : "GNSSObservationApriori";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(String.format(sql, tableName));
			int idx = 1;
			stmt.setString(idx++, oldName);
			stmt.setString(idx++, newName);
			stmt.setString(idx++, newName);
			stmt.setString(idx++, oldName);
			stmt.setInt(idx++, observationTreeItemValue.GroupId);
			ResultSet rs = stmt.executeQuery();

			bool collisions = false;
			if (rs.next())
			{
				collisions = rs.getBoolean("collisions");
			}
			return collisions;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private boolean isNameCollisions(org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue congruenceAnalysisTreeItemValue, String oldName, String newName) throws java.sql.SQLException
		private bool isNameCollisions(CongruenceAnalysisTreeItemValue congruenceAnalysisTreeItemValue, string oldName, string newName)
		{
			string sql = "SELECT COUNT(\"id\") > 0 AS \"collisions\" FROM " + "\"CongruenceAnalysisPointPairApriori\"" + "WHERE ((\"start_point_name\" = ? AND \"end_point_name\" = ?) OR (\"start_point_name\" = ? AND \"end_point_name\" = ?)) " + "AND \"group_id\" = ?";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setString(idx++, oldName);
			stmt.setString(idx++, newName);
			stmt.setString(idx++, newName);
			stmt.setString(idx++, oldName);
			stmt.setInt(idx++, congruenceAnalysisTreeItemValue.GroupId);
			ResultSet rs = stmt.executeQuery();

			bool collisions = false;
			if (rs.next())
			{
				collisions = rs.getBoolean("collisions");
			}
			return collisions;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.util.Set<String> getFullPointNameSet() throws java.sql.SQLException
		public virtual ISet<string> FullPointNameSet
		{
			get
			{
				ISet<string> names = new HashSet<string>();
				if (!this.hasDatabase() || !this.dataBase.Open)
				{
					return null;
				}
    
				string sql = "SELECT " + "\"name\" " + "FROM \"PointApriori\" " + "ORDER BY \"id\" ASC";
    
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
    
				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					string name = rs.getString("name");
					names.Add(name);
				}
    
				return names;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.util.Set<String> getFullVerticalDeflectionNameSet() throws java.sql.SQLException
		public virtual ISet<string> FullVerticalDeflectionNameSet
		{
			get
			{
				ISet<string> names = new HashSet<string>();
				if (!this.hasDatabase() || !this.dataBase.Open)
				{
					return null;
				}
    
				string sql = "SELECT " + "\"name\" " + "FROM \"VerticalDeflectionApriori\" " + "ORDER BY \"id\" ASC";
    
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
    
				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					string name = rs.getString("name");
					names.Add(name);
				}
    
				return names;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadTableColumnProperties() throws java.sql.SQLException
		private void loadTableColumnProperties()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			ColumnPropertiesManager columnPropertiesManager = ColumnPropertiesManager.Instance;
			columnPropertiesManager.clearOrder();

			SortedDictionary<int, ColumnContentType> sortOrderSequence = new SortedDictionary<int, ColumnContentType>(new ComparatorAnonymousInnerClass(this));

			SortedDictionary<int, ColumnContentType> columnOrderSequence = new SortedDictionary<int, ColumnContentType>(new ComparatorAnonymousInnerClass2(this));

			string sql = "SELECT " + "\"table_type\", \"column_type\", \"width\", " + "\"sort_type\", \"sort_order\", \"column_order\" " + "FROM \"TableColumnProperty\" ORDER BY \"table_type\" ASC, \"column_order\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			TableContentType lastTableType = null;
			while (rs.next())
			{
				TableContentType tableType = TableContentType.getEnumByValue(rs.getInt("table_type"));
				ColumnContentType columnType = ColumnContentType.getEnumByValue(rs.getInt("column_type"));
				double width = rs.getDouble("width");
				int sortType = rs.getInt("sort_type");
				int sortOrder = rs.getInt("sort_order");
				int columnOrder = rs.getInt("column_order");

				if (tableType == null || tableType == TableContentType.UNSPECIFIC || columnType == null || columnType == ColumnContentType.DEFAULT || width <= 0)
				{
					continue;
				}

				ColumnProperty columnProperty = columnPropertiesManager.getProperty(tableType, columnType);
				columnProperty.setPrefWidth(width);
				columnProperty.SortType = sortType < 0 ? SortType.DESCENDING : SortType.ASCENDING;
				columnProperty.SortOrder = sortOrder;
				columnProperty.ColumnOrder = columnOrder;

				if (lastTableType != tableType)
				{
					if (lastTableType != null)
					{
						columnPropertiesManager.getSortOrder(lastTableType).setAll(sortOrderSequence.Values);
						columnPropertiesManager.getColumnsOrder(lastTableType).setAll(columnOrderSequence.Values);
					}

					sortOrderSequence.Clear();
					columnOrderSequence.Clear();
					lastTableType = tableType;
				}

				if (sortOrder >= 0)
				{
					sortOrderSequence[sortOrder] = columnType;
				}
				columnOrderSequence[columnOrder] = columnType;
			}
			if (lastTableType != null)
			{
				columnPropertiesManager.getSortOrder(lastTableType).setAll(sortOrderSequence.Values);
				columnPropertiesManager.getColumnsOrder(lastTableType).setAll(columnOrderSequence.Values);
			}
		}

		private class ComparatorAnonymousInnerClass : IComparer<int>
		{
			private readonly SQLManager outerInstance;

			public ComparatorAnonymousInnerClass(SQLManager outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public int Compare(int? o1, int? o2)
			{
				return o1.compareTo(o2);
			}
		}

		private class ComparatorAnonymousInnerClass2 : IComparer<int>
		{
			private readonly SQLManager outerInstance;

			public ComparatorAnonymousInnerClass2(SQLManager outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public int Compare(int? o1, int? o2)
			{
				return o1.compareTo(o2);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveTableColumnProperties() throws java.sql.SQLException
		public virtual void saveTableColumnProperties()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"TableColumnProperty\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS INT), CAST(? AS INT), CAST(? AS INT)) " + ") AS \"vals\" (\"table_type\", \"column_type\", \"width\", \"sort_type\", \"sort_order\", \"column_order\") ON \"TableColumnProperty\".\"table_type\" = \"vals\".\"table_type\" AND \"TableColumnProperty\".\"column_type\" = \"vals\".\"column_type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"TableColumnProperty\".\"width\"        = \"vals\".\"width\", " + "\"TableColumnProperty\".\"sort_type\"    = \"vals\".\"sort_type\", " + "\"TableColumnProperty\".\"sort_order\"   = \"vals\".\"sort_order\", " + "\"TableColumnProperty\".\"column_order\" = \"vals\".\"column_order\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"table_type\", " + "\"vals\".\"column_type\", " + "\"vals\".\"width\", " + "\"vals\".\"sort_type\", " + "\"vals\".\"sort_order\", " + "\"vals\".\"column_order\" ";

			bool hasBatch = false;

			try
			{
				ColumnPropertiesManager columnPropertiesManager = ColumnPropertiesManager.Instance;

				this.dataBase.AutoCommit = false;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

				foreach (KeyValuePair<Pair<TableContentType, ColumnContentType>, ColumnProperty> entry in columnPropertiesManager)
				{
					TableContentType tableType = entry.Key.getKey();
					ColumnProperty columnProperty = entry.Value;
					ColumnContentType columnType = columnProperty.ColumnContentType;
					double width = columnProperty.getPrefWidth().Value;

					int idx = 1;
					stmt.setInt(idx++, tableType.getId());
					stmt.setInt(idx++, columnType.getId());
					stmt.setDouble(idx++, width);
					stmt.setInt(idx++, columnProperty.SortType == SortType.DESCENDING ? -1 : +1);
					stmt.setInt(idx++, columnProperty.SortOrder);
					stmt.setInt(idx++, columnProperty.ColumnOrder);
					stmt.addBatch();
					hasBatch = true;
				}

				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void loadTableRowHighlight() throws java.sql.SQLException
		public virtual void loadTableRowHighlight()
		{
			TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;

			this.loadTableRowHighlightProperties(tableRowHighlight);
			this.loadTableRowHighlightType(tableRowHighlight);
			this.loadTableRowHighlightRange(tableRowHighlight);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadTableRowHighlightProperties(org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight tableRowHighlight) throws java.sql.SQLException
		private void loadTableRowHighlightProperties(TableRowHighlight tableRowHighlight)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "SELECT " + "\"red\", \"green\", \"blue\" " + "FROM \"TableRowHighlightProperty\" " + "WHERE \"type\" = ? LIMIT 1";

			foreach (TableRowHighlightRangeType tableRowHighlightRangeType in TableRowHighlightRangeType.values())
			{
				int idx = 1;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				stmt.setInt(idx++, tableRowHighlightRangeType.getId());
				ResultSet rs = stmt.executeQuery();
				if (rs.next())
				{
					double opacity = 1.0;

					double red = rs.getDouble("red");
					red = Math.Min(Math.Max(0, red), 1);

					double green = rs.getDouble("green");
					green = Math.Min(Math.Max(0, green), 1);

					double blue = rs.getDouble("blue");
					blue = Math.Min(Math.Max(0, blue), 1);

					tableRowHighlight.setColor(tableRowHighlightRangeType, new Color(red, green, blue, opacity));
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadTableRowHighlightRange(org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight tableRowHighlight) throws java.sql.SQLException
		private void loadTableRowHighlightRange(TableRowHighlight tableRowHighlight)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "SELECT " + "\"left_boundary\", \"right_boundary\" " + "FROM \"TableRowHighlightRange\" " + "WHERE \"type\" = ? LIMIT 1";

			foreach (TableRowHighlightType tableRowHighlightType in TableRowHighlightType.values())
			{
				int idx = 1;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				stmt.setInt(idx++, tableRowHighlightType.getId());

				ResultSet rs = stmt.executeQuery();
				if (rs.next())
				{
					double leftBoundary = rs.getDouble("left_boundary");
					double rightBoundary = rs.getDouble("right_boundary");
					tableRowHighlight.setRange(tableRowHighlightType, leftBoundary, rightBoundary);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadTableRowHighlightType(org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight tableRowHighlight) throws java.sql.SQLException
		private void loadTableRowHighlightType(TableRowHighlight tableRowHighlight)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "SELECT " + "\"type\" " + "FROM \"TableRowHighlightScheme\" " + "WHERE \"id\" = 1 LIMIT 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				TableRowHighlightType tableRowHighlightType = TableRowHighlightType.getEnumByValue(rs.getInt("type"));
				if (tableRowHighlightType != null)
				{
					tableRowHighlight.SelectedTableRowHighlightType = tableRowHighlightType;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveTableRowHighlight() throws java.sql.SQLException
		public virtual void saveTableRowHighlight()
		{
			TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;

			// save color properties
			this.save(TableRowHighlightRangeType.EXCELLENT, tableRowHighlight.getColor(TableRowHighlightRangeType.EXCELLENT));
			this.save(TableRowHighlightRangeType.SATISFACTORY, tableRowHighlight.getColor(TableRowHighlightRangeType.SATISFACTORY));
			this.save(TableRowHighlightRangeType.INADEQUATE, tableRowHighlight.getColor(TableRowHighlightRangeType.INADEQUATE));

			// save range and display options
			foreach (TableRowHighlightType tableRowHighlightType in TableRowHighlightType.values())
			{
				this.save(tableRowHighlightType, tableRowHighlight.getLeftBoundary(tableRowHighlightType), tableRowHighlight.getRightBoundary(tableRowHighlightType));
			}

			this.save(tableRowHighlight.SelectedTableRowHighlightType);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void save(org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType tableRowHighlightRangeType, javafx.scene.paint.Color color) throws java.sql.SQLException
		private void save(TableRowHighlightRangeType tableRowHighlightRangeType, Color color)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"TableRowHighlightProperty\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE)) " + ") AS \"vals\" (\"type\", \"red\", \"green\", \"blue\") ON \"TableRowHighlightProperty\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"TableRowHighlightProperty\".\"red\"   = \"vals\".\"red\", " + "\"TableRowHighlightProperty\".\"green\" = \"vals\".\"green\", " + "\"TableRowHighlightProperty\".\"blue\"  = \"vals\".\"blue\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"type\", " + "\"vals\".\"red\", " + "\"vals\".\"green\", " + "\"vals\".\"blue\"";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, tableRowHighlightRangeType.getId());
			stmt.setDouble(idx++, color.getRed());
			stmt.setDouble(idx++, color.getGreen());
			stmt.setDouble(idx++, color.getBlue());
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void save(org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType tableRowHighlightType, double leftBoundary, double rightBoundary) throws java.sql.SQLException
		private void save(TableRowHighlightType tableRowHighlightType, double leftBoundary, double rightBoundary)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"TableRowHighlightRange\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE)) " + ") AS \"vals\" (\"type\", \"left_boundary\", \"right_boundary\") ON \"TableRowHighlightRange\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"TableRowHighlightRange\".\"left_boundary\"  = \"vals\".\"left_boundary\", " + "\"TableRowHighlightRange\".\"right_boundary\" = \"vals\".\"right_boundary\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"type\", " + "\"vals\".\"left_boundary\", " + "\"vals\".\"right_boundary\"";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, tableRowHighlightType.getId());
			stmt.setDouble(idx++, leftBoundary);
			stmt.setDouble(idx++, rightBoundary);
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void save(org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType tableRowHighlightType) throws java.sql.SQLException
		private void save(TableRowHighlightType tableRowHighlightType)
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"TableRowHighlightScheme\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT)) " + ") AS \"vals\" (\"id\", \"type\") ON \"TableRowHighlightScheme\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"TableRowHighlightScheme\".\"type\" = \"vals\".\"type\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"type\"";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, 1); // default id
			stmt.setInt(idx++, tableRowHighlightType.getId());
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void loadImportPreferences() throws java.sql.SQLException
		public virtual void loadImportPreferences()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			ImportOption importOption = ImportOption.Instance;
			string sql = "SELECT " + "\"id\", \"separate\" " + "FROM \"ImportSeparation\"";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				int id = rs.getInt("id");
				ObservationType observationType = ObservationType.getEnumByValue(id);
				if (observationType == null)
				{
					continue;
				}

				bool separate = rs.getBoolean("separate");
				importOption.setGroupSeparation(observationType, separate);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveImportPreferences() throws java.sql.SQLException
		public virtual void saveImportPreferences()
		{
			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return;
			}

			string sql = "MERGE INTO \"ImportSeparation\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"id\", \"separate\") ON \"ImportSeparation\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"ImportSeparation\".\"separate\" = \"vals\".\"separate\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"separate\"";

			ImportOption importOption = ImportOption.Instance;
			bool hasBatch = false;

			try
			{
				this.dataBase.AutoCommit = false;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

				foreach (ObservationType observationType in ObservationType.values())
				{
					int idx = 1;
					stmt.setInt(idx++, observationType.getId());
					stmt.setBoolean(idx++, importOption.isGroupSeparation(observationType));
					stmt.addBatch();
					hasBatch = true;
				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.util.List<org.applied_geodesy.adjustment.network.ObservationType> getProjectObservationTypes() throws java.sql.SQLException
		public virtual IList<ObservationType> ProjectObservationTypes
		{
			get
			{
				IList<ObservationType> observationTypes = new List<ObservationType>(ObservationType.values().Length);
    
				if (!this.hasDatabase() || !this.dataBase.Open)
				{
					return observationTypes;
				}
    
				string sql = "SELECT DISTINCT \"type\" " + "FROM \"ObservationGroup\" " + "JOIN \"ObservationApriori\" " + "ON \"ObservationGroup\".\"id\" = \"ObservationApriori\".\"group_id\" AND \"ObservationApriori\".\"enable\" = TRUE " + "JOIN \"ObservationAposteriori\" " + "ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "WHERE \"ObservationGroup\".\"enable\" = TRUE";
    
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				ResultSet rs = stmt.executeQuery();
				while (rs.next())
				{
					int id = rs.getInt("type");
					ObservationType observationType = ObservationType.getEnumByValue(id);
					if (observationType == null)
					{
						continue;
					}
    
					observationTypes.Add(observationType);
				}
    
				return observationTypes;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.util.Map<org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType, int> getChartData(org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType tableRowHighlightType, org.applied_geodesy.adjustment.network.ObservationType[] observationTypes) throws java.sql.SQLException
		public virtual IDictionary<TableRowHighlightRangeType, int> getChartData(TableRowHighlightType tableRowHighlightType, ObservationType[] observationTypes)
		{
			// Default values
			IDictionary<TableRowHighlightRangeType, int> chartData = new LinkedHashMap<TableRowHighlightRangeType, int>();
			chartData[TableRowHighlightRangeType.INADEQUATE] = 0;
			chartData[TableRowHighlightRangeType.SATISFACTORY] = 0;
			chartData[TableRowHighlightRangeType.EXCELLENT] = 0;

			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return chartData;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < observationTypes.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			string sql = "SELECT " + "CASE " + "WHEN \"redundancy\" < (SELECT \"left_boundary\" FROM \"TableRowHighlightRange\" WHERE \"type\" = ? LIMIT 1) THEN CAST(? AS INT) " + "WHEN \"redundancy\" BETWEEN (SELECT \"left_boundary\" FROM \"TableRowHighlightRange\" WHERE \"type\" = ? LIMIT 1) AND (SELECT \"right_boundary\" FROM \"TableRowHighlightRange\" WHERE \"type\" = ? LIMIT 1) THEN CAST(? AS INT) " + "WHEN \"redundancy\" > (SELECT \"right_boundary\" FROM \"TableRowHighlightRange\" WHERE \"type\" = ? LIMIT 1) THEN CAST(? AS INT) " + "END AS \"range\", " + "COUNT(\"id\") AS \"data_count\" " + "FROM \"ObservationAposteriori\" " + "JOIN \"ObservationApriori\" " + "ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "JOIN \"ObservationGroup\" " + "ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationGroup\".\"enable\" = TRUE " + "AND \"ObservationApriori\".\"enable\" = TRUE " + "AND \"type\" IN (" + inArrayValues + ") " + "GROUP BY \"range\" " + "ORDER BY \"range\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			int idx = 1;
			stmt.setInt(idx++, tableRowHighlightType.getId()); // INADEQUATE range
			stmt.setInt(idx++, TableRowHighlightRangeType.INADEQUATE.getId()); // INADEQUATE

			stmt.setInt(idx++, tableRowHighlightType.getId()); // SATISFACTORY - left range
			stmt.setInt(idx++, tableRowHighlightType.getId()); // SATISFACTORY - right range
			stmt.setInt(idx++, TableRowHighlightRangeType.SATISFACTORY.getId()); // SATISFACTORY

			stmt.setInt(idx++, tableRowHighlightType.getId()); // EXCELLENT range
			stmt.setInt(idx++, TableRowHighlightRangeType.EXCELLENT.getId()); // EXCELLENT

			foreach (ObservationType type in observationTypes)
			{
				stmt.setInt(idx++, type.getId());
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				int range = rs.getInt("range");
				int count = rs.getInt("data_count");

				TableRowHighlightRangeType tableRowHighlightRangeType = TableRowHighlightRangeType.getEnumByValue(range);
				if (tableRowHighlightRangeType != null && tableRowHighlightRangeType != TableRowHighlightRangeType.NONE)
				{
					chartData[tableRowHighlightRangeType] = count;
				}
			}

			return chartData;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.util.List<double> getNormalizedResiduals(org.applied_geodesy.adjustment.network.ObservationType[] observationTypes) throws java.sql.SQLException
		public virtual IList<double> getNormalizedResiduals(ObservationType[] observationTypes)
		{
			IList<double> normalizedResiduals = new List<double>();

			if (!this.hasDatabase() || !this.dataBase.Open)
			{
				return normalizedResiduals;
			}

			StringBuilder inArrayValues = new StringBuilder("?");
			for (int i = 1; i < observationTypes.Length; i++)
			{
				inArrayValues.Append(",?");
			}

			string sql = "SELECT SIGN(\"residual\") * SQRT(\"t_prio\") AS \"normalized_residual\" " + "FROM \"ObservationAposteriori\" JOIN \"ObservationApriori\" " + "ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "JOIN \"ObservationGroup\" " + "ON \"ObservationGroup\".\"id\" = \"ObservationApriori\".\"group_id\" " + "WHERE \"ObservationGroup\".\"enable\" = TRUE " + "AND \"ObservationApriori\".\"enable\" = TRUE " + "AND \"redundancy\" > 0 " + "AND \"type\" IN (" + inArrayValues + ") " + "ORDER BY \"normalized_residual\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			int idx = 1;
			foreach (ObservationType type in observationTypes)
			{
				stmt.setInt(idx++, type.getId());
			}

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				double nv = rs.getDouble("normalized_residual");
				normalizedResiduals.Add(nv);
			}

			return normalizedResiduals;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void executeStatement(String sql) throws java.sql.SQLException
		public virtual void executeStatement(string sql)
		{
			if (!this.hasDatabase() || !this.dataBase.Open || string.ReferenceEquals(sql, null) || sql.Trim().Length == 0 || sql.Length == 0)
			{
				return;
			}

			try
			{
				this.dataBase.AutoCommit = false;
				Statement stmt = this.dataBase.Statement;
				stmt.execute(sql);
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}


		private void fireDatabaseStateChanged(ProjectDatabaseStateType stateType)
		{
			ProjectDatabaseStateEvent evt = new ProjectDatabaseStateEvent(this, stateType);
			object[] listeners = this.listenerList.ToArray();
			for (int i = 0; i < listeners.Length; i++)
			{
				if (listeners[i] is ProjectDatabaseStateChangeListener)
				{
					((ProjectDatabaseStateChangeListener)listeners[i]).projectDatabaseStateChanged(evt);
				}
			}
		}

		public virtual void addProjectDatabaseStateChangeListener(ProjectDatabaseStateChangeListener l)
		{
			this.listenerList.Add(l);
		}

		public virtual void removeProjectDatabaseStateChangeListener(ProjectDatabaseStateChangeListener l)
		{
			this.listenerList.Remove(l);
		}
	}

}