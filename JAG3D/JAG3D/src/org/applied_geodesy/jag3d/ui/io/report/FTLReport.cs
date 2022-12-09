using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

namespace org.applied_geodesy.jag3d.ui.io.report
{

	using EstimationType = org.applied_geodesy.adjustment.EstimationType;
	using ObservationGroupUncertaintyType = org.applied_geodesy.adjustment.network.ObservationGroupUncertaintyType;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using PointGroupUncertaintyType = org.applied_geodesy.adjustment.network.PointGroupUncertaintyType;
	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using VarianceComponentType = org.applied_geodesy.adjustment.network.VarianceComponentType;
	using VerticalDeflectionGroupUncertaintyType = org.applied_geodesy.adjustment.network.VerticalDeflectionGroupUncertaintyType;
	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using ProjectionType = org.applied_geodesy.adjustment.network.observation.reduction.ProjectionType;
	using ReductionTaskType = org.applied_geodesy.adjustment.network.observation.reduction.ReductionTaskType;
	using TestStatisticType = org.applied_geodesy.adjustment.statistic.TestStatisticType;
	using LayerType = org.applied_geodesy.jag3d.ui.graphic.layer.LayerType;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using Ellipsoid = org.applied_geodesy.transformation.datum.Ellipsoid;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using FormatterOption = org.applied_geodesy.util.FormatterOptions.FormatterOption;
	using DataBase = org.applied_geodesy.util.sql.DataBase;
	using UnitType = org.applied_geodesy.util.unit.UnitType;

	using BeansWrapper = freemarker.ext.beans.BeansWrapper;
	using BeansWrapperBuilder = freemarker.ext.beans.BeansWrapperBuilder;
	using Configuration = freemarker.template.Configuration;
	using MalformedTemplateNameException = freemarker.template.MalformedTemplateNameException;
	using Template = freemarker.template.Template;
	using TemplateException = freemarker.template.TemplateException;
	using TemplateExceptionHandler = freemarker.template.TemplateExceptionHandler;
	using TemplateHashModel = freemarker.template.TemplateHashModel;
	using TemplateModelException = freemarker.template.TemplateModelException;
	using TemplateNotFoundException = freemarker.template.TemplateNotFoundException;
	using Version = freemarker.template.Version;
	using HostServices = javafx.application.HostServices;

	public class FTLReport
	{
		private static readonly Version VERSION = Configuration.VERSION_2_3_31;
		private FormatterOptions options = FormatterOptions.Instance;
		private Template template = null;
		private readonly DataBase dataBase;
		private readonly HostServices hostServices;
		private IDictionary<string, object> data = new Dictionary<string, object>();
		public const string TEMPLATE_PATH = "ftl/jag3d/";
		private readonly Configuration cfg = new Configuration(VERSION);

		public FTLReport(DataBase dataBase, HostServices hostServices)
		{
			if (dataBase == null || !dataBase.Open)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, database must be open! " + dataBase);
			}
			this.dataBase = dataBase;
			this.hostServices = hostServices;
			this.init();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void setDataBaseSchema() throws java.sql.SQLException
		private void setDataBaseSchema()
		{
			this.dataBase.getPreparedStatement("SET SCHEMA \"OpenAdjustment\"").execute();
		}

		private void init()
		{
			try
			{
				File path = new File(typeof(FTLReport).getClassLoader().getResource(TEMPLATE_PATH).toURI());

				this.cfg.setDirectoryForTemplateLoading(path);
				this.cfg.setTemplateExceptionHandler(TemplateExceptionHandler.RETHROW_HANDLER);
				this.cfg.setLogTemplateExceptions(false);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		private void setParam(string key, object value)
		{
			this.data[key] = value;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setTemplate(String template) throws TemplateNotFoundException, MalformedTemplateNameException, ParseException, java.io.IOException
		public virtual string Template
		{
			set
			{
				this.template = this.cfg.getTemplate(value);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void createReport() throws ClassNotFoundException, SQLException, freemarker.template.TemplateModelException
		private void createReport()
		{
			this.data.Clear();
			BeansWrapper wrapper = (new BeansWrapperBuilder(VERSION)).build();
			TemplateHashModel staticModels = wrapper.getStaticModels();
			TemplateHashModel mathStatics = (TemplateHashModel) staticModels.get("java.lang.Math");
			this.data["Math"] = mathStatics;

			this.setDataBaseSchema();
			this.initFormatterOptions();
			this.addAdjustmentDefinitions();

			this.addVersion();
			this.addMetaData();

			this.addRankDefect();
			this.addProjectionAndReductions();

			this.addPrincipalComponent();
			this.addTeststatistics();
			this.addVarianceComponents();

			this.addPointGroups();
			this.addObservations();
			this.addCongruenceAnalysis();
			this.addVerticalDefelctionGroups();

			this.addChartAndStatisticValues();
			this.addLayerProperties();
			this.addVectorScale();
		}

		public virtual string SuggestedFileName
		{
			get
			{
				return this.dataBase != null ? this.dataBase.URI : null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void toFile(java.io.File report) throws ClassNotFoundException, SQLException, TemplateException, java.io.IOException
		public virtual void toFile(File report)
		{
			if (report != null)
			{
				this.createReport();
				Writer file = new StreamWriter(new FileStream(report, FileMode.Create, FileAccess.Write), Encoding.UTF8);
				this.template.process(this.data, file);
				file.flush();
				file.close();

				if (this.hostServices != null)
				{
					this.hostServices.showDocument(report.getAbsolutePath());
				}
			}
		}


		/// <summary>
		///****** Datenbankabfragen ************ </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void initFormatterOptions() throws java.sql.SQLException
		private void initFormatterOptions()
		{
			FormatterOptions options = FormatterOptions.Instance;
			IDictionary<CellValueType, FormatterOptions.FormatterOption> formatterOptions = options.FormatterOptions;

			foreach (FormatterOptions.FormatterOption option in formatterOptions.Values)
			{
				string keyUnitType = null;
				string keyUnitConversion = null;
				string keyUnitDigits = null;
				string keyUnitAbbr = null;
				string keySexagesimal = null;
				double conversionFactor = 1.0;
				CellValueType cellValueType = option.Type;
				switch (cellValueType.innerEnumValue)
				{
				case CellValueType.InnerEnum.ANGLE:
					keyUnitType = "unit_type_angle";
					keyUnitConversion = "unit_conversion_angle";
					keyUnitDigits = "digits_angle";
					keyUnitAbbr = "unit_abbr_angle";
					conversionFactor = options.convertAngleToModel(1.0);
					keySexagesimal = option.Unit.getType() == UnitType.DEGREE_SEXAGESIMAL ? "sexagesimal_angle" : null;
					break;
				case CellValueType.InnerEnum.ANGLE_RESIDUAL:
					keyUnitType = "unit_type_angle_residual";
					keyUnitConversion = "unit_conversion_angle_residual";
					keyUnitDigits = "digits_angle_residual";
					keyUnitAbbr = "unit_abbr_angle_residual";
					conversionFactor = options.convertAngleResidualToModel(1.0);
					keySexagesimal = option.Unit.getType() == UnitType.DEGREE_SEXAGESIMAL ? "sexagesimal_angle_residual" : null;
					break;
				case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
					keyUnitType = "unit_type_angle_uncertainty";
					keyUnitConversion = "unit_conversion_angle_uncertainty";
					keyUnitDigits = "digits_angle_uncertainty";
					keyUnitAbbr = "unit_abbr_angle_uncertainty";
					conversionFactor = options.convertAngleUncertaintyToModel(1.0);
					keySexagesimal = option.Unit.getType() == UnitType.DEGREE_SEXAGESIMAL ? "sexagesimal_angle_uncertainty" : null;
					break;

				case CellValueType.InnerEnum.LENGTH:
					keyUnitType = "unit_type_length";
					keyUnitConversion = "unit_conversion_length";
					keyUnitDigits = "digits_length";
					keyUnitAbbr = "unit_abbr_length";
					conversionFactor = options.convertLengthToModel(1.0);
					break;
				case CellValueType.InnerEnum.LENGTH_RESIDUAL:
					keyUnitType = "unit_type_length_residual";
					keyUnitConversion = "unit_conversion_length_residual";
					keyUnitDigits = "digits_length_residual";
					keyUnitAbbr = "unit_abbr_length_residual";
					conversionFactor = options.convertLengthResidualToModel(1.0);
					break;
				case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
					keyUnitType = "unit_type_length_uncertainty";
					keyUnitConversion = "unit_conversion_length_uncertainty";
					keyUnitDigits = "digits_length_uncertainty";
					keyUnitAbbr = "unit_abbr_length_uncertainty";
					conversionFactor = options.convertLengthUncertaintyToModel(1.0);
					break;

				case CellValueType.InnerEnum.SCALE:
					keyUnitType = "unit_type_scale";
					keyUnitConversion = "unit_conversion_scale";
					keyUnitDigits = "digits_scale";
					keyUnitAbbr = "unit_abbr_scale";
					conversionFactor = options.convertScaleToModel(1.0);
					break;
				case CellValueType.InnerEnum.SCALE_RESIDUAL:
					keyUnitType = "unit_type_scale_residual";
					keyUnitConversion = "unit_conversion_scale_residual";
					keyUnitDigits = "digits_scale_residual";
					keyUnitAbbr = "unit_abbr_scale_residual";
					conversionFactor = options.convertScaleResidualToModel(1.0);
					break;
				case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
					keyUnitType = "unit_type_scale_uncertainty";
					keyUnitConversion = "unit_conversion_scale_uncertainty";
					keyUnitDigits = "digits_scale_uncertainty";
					keyUnitAbbr = "unit_abbr_scale_uncertainty";
					conversionFactor = options.convertScaleUncertaintyToModel(1.0);
					break;

				case CellValueType.InnerEnum.PERCENTAGE:
					keyUnitType = "unit_type_percentage";
					keyUnitConversion = "unit_conversion_percentage";
					keyUnitDigits = "digits_percentage";
					keyUnitAbbr = "unit_abbr_percentage";
					conversionFactor = options.convertPercentToModel(1.0);
					break;

				case CellValueType.InnerEnum.STATISTIC:
					keyUnitDigits = "digits_statistic";
					keyUnitConversion = "unit_conversion_statistic";
					conversionFactor = 1.0;
					break;

				default:
					continue;
					// break;
				}

				if (!string.ReferenceEquals(keyUnitType, null) && option.Unit != null)
				{
					this.setParam(keyUnitType, option.Unit.getType().name());
				}
				if (!string.ReferenceEquals(keyUnitConversion, null))
				{
					this.setParam(keyUnitConversion, conversionFactor);
				}
				if (!string.ReferenceEquals(keyUnitDigits, null))
				{
					this.setParam(keyUnitDigits, option.Formatter.format(0.0));
				}
				if (!string.ReferenceEquals(keyUnitAbbr, null) && option.Unit != null)
				{
					this.setParam(keyUnitAbbr, option.Unit.getAbbreviation());
				}
				if (!string.ReferenceEquals(keySexagesimal, null))
				{
					this.setParam(keySexagesimal, true);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addAdjustmentDefinitions() throws java.sql.SQLException
		private void addAdjustmentDefinitions()
		{
			string sql = "SELECT " + "\"type\", \"number_of_iterations\", " + "\"robust_estimation_limit\", \"estimate_direction_set_orientation_approximation\", " + "\"congruence_analysis\", \"export_covariance_matrix\" " + "FROM \"AdjustmentDefinition\" WHERE \"id\" = 1 LIMIT 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				EstimationType estimationType = EstimationType.getEnumByValue(rs.getInt("type"));
				if (estimationType == null)
				{
					estimationType = EstimationType.L2NORM;
				}

				this.setParam("estimation_type", estimationType.ToString()); // L1NORM, L2NORM or SIMULATION
				this.setParam("robust_estimation_limit", rs.getDouble("robust_estimation_limit"));
				this.setParam("congruence_analysis", rs.getBoolean("congruence_analysis"));
				this.setParam("number_of_iterations", rs.getBoolean("number_of_iterations"));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addVersion() throws java.sql.SQLException
		private void addVersion()
		{
			string sql = "SELECT " + "MAX(\"version\") AS \"version\" " + "FROM \"Version\"";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				this.setParam("version", rs.getInt("version"));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addMetaData() throws java.sql.SQLException
		private void addMetaData()
		{
			string sql = "SELECT " + "\"name\", \"operator\", " + "\"description\", \"date\", " + "\"customer_id\", \"project_id\" " + "FROM \"ProjectMetadata\" WHERE \"id\" = 1 LIMIT 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				this.setParam("project_name", rs.getString("name"));
				this.setParam("project_operator", rs.getString("operator"));
				this.setParam("project_description", rs.getString("description"));
				this.setParam("project_customer_id", rs.getString("customer_id"));
				this.setParam("project_project_id", rs.getString("project_id"));
				this.setParam("project_date", new DateTime(rs.getTimestamp("date").getTime()));
			}

			this.setParam("report_creation_date", new DateTime(DateTimeHelper.CurrentUnixTimeMillis()));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addPrincipalComponent() throws java.sql.SQLException
		private void addPrincipalComponent()
		{
			IList<Dictionary<string, Number>> principalComponents = new List<Dictionary<string, Number>>();
			string sql = "SELECT \"index\", SQRT(ABS(\"value\")) AS \"value\", \"ratio\" FROM \"PrincipalComponent\" ORDER BY \"index\" DESC";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				Dictionary<string, Number> component = new Dictionary<string, Number>();
				int index = rs.getInt("index");
				double value = options.convertLengthResidualToView(rs.getDouble("value"));
				double ratio = options.convertPercentToView(rs.getDouble("ratio"));
				component["index"] = index;
				component["value"] = value;
				component["ratio"] = ratio;
				principalComponents.Add(component);
			}

			if (principalComponents.Count > 0)
			{
				this.setParam("principal_components", principalComponents);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addProjectionAndReductions() throws java.sql.SQLException
		private void addProjectionAndReductions()
		{
			string sql = "SELECT " + "\"projection_type\", " + "\"reference_latitude\", \"reference_longitude\", \"reference_height\", " + "\"major_axis\", \"minor_axis\", " + "\"x0\", \"y0\", \"z0\", \"type\" AS \"task_type\" " + "FROM \"ReductionTask\" " + "RIGHT JOIN \"ReductionDefinition\" ON \"ReductionTask\".\"reduction_id\" = \"ReductionDefinition\".\"id\" " + "WHERE \"ReductionDefinition\".\"id\" = 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				ProjectionType projectionType = ProjectionType.getEnumByValue(rs.getInt("projection_type"));
				ReductionTaskType taskType = ReductionTaskType.getEnumByValue(rs.getInt("task_type"));
				double referenceLatitude = rs.getDouble("reference_latitude");
				double referenceLongitude = rs.getDouble("reference_longitude");
				double referenceHeight = rs.getDouble("reference_height");
				double majorAxis = rs.getDouble("major_axis");
				double minorAxis = rs.getDouble("minor_axis");
				double principalPointX0 = rs.getDouble("x0");
				double principalPointY0 = rs.getDouble("y0");
				double principalPointZ0 = rs.getDouble("z0");

				// set default value
				this.setParam("projection_type", ProjectionType.LOCAL_CARTESIAN);

				if (projectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
				{
					this.setParam("projection_type", projectionType.ToString());
					this.setParam("projection_reference_latitude", options.convertAngleToView(referenceLatitude));
					this.setParam("projection_reference_longitude", options.convertAngleToView(referenceLongitude));
					this.setParam("projection_reference_height", options.convertLengthToView(referenceHeight));
					this.setParam("projection_major_axis", options.convertLengthToView(majorAxis));
					this.setParam("projection_minor_axis", options.convertLengthToView(minorAxis));
					this.setParam("projection_principal_point_x0", options.convertLengthToView(principalPointX0));
					this.setParam("projection_principal_point_y0", options.convertLengthToView(principalPointY0));
					this.setParam("projection_principal_point_z0", options.convertLengthToView(principalPointZ0));
				}
				else
				{
					if (taskType == null)
					{
						continue;
					}

					this.setParam("projection_type", projectionType.ToString());
					this.setParam("projection_reference_height", options.convertLengthToView(referenceHeight));
					this.setParam("projection_reference_latitude", options.convertAngleToView(referenceLatitude));
					this.setParam("projection_earth_radius", options.convertLengthToView(Ellipsoid.createEllipsoidFromMinorAxis(majorAxis, minorAxis).getRadiusOfConformalSphere(referenceLatitude)));

					switch (taskType.innerEnumValue)
					{
					case ReductionTaskType.InnerEnum.DIRECTION:
						this.setParam("reduction_direction", projectionType != ProjectionType.LOCAL_CARTESIAN);
						break;
					case ReductionTaskType.InnerEnum.DISTANCE:
						this.setParam("reduction_distance", projectionType != ProjectionType.LOCAL_CARTESIAN);
						break;
					case ReductionTaskType.InnerEnum.EARTH_CURVATURE:
						this.setParam("reduction_earth_curvature", true);
						break;
					case ReductionTaskType.InnerEnum.HEIGHT:
						this.setParam("reduction_height", true);
						break;
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addRankDefect() throws java.sql.SQLException
		private void addRankDefect()
		{
			string sql = "SELECT \"ty\",\"tx\",\"tz\",\"ry\",\"rx\",\"rz\",\"sy\",\"sx\",\"sz\",\"my\",\"mx\",\"mz\",\"mxy\",\"mxyz\" FROM \"RankDefect\" WHERE \"id\" = 1 LIMIT 1";
			IDictionary<string, bool> defects = new Dictionary<string, bool>(14);
			int count = 0;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				ResultSetMetaData rsmd = rs.getMetaData();
				int cnt = rsmd.getColumnCount();
				for (int i = 1; i <= cnt; i++)
				{
					string key = rsmd.getColumnLabel(i);
					bool defect = rs.getBoolean(key);
					defects[key] = defect;
					count += defect ? 1 : 0;
				}
				this.setParam("rank_defect", defects);
			}

			if (count > 0)
			{
				this.setParam("rank_defect_count", count);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addTeststatistics() throws java.sql.SQLException
		private void addTeststatistics()
		{
			string sqlDefinition = "SELECT \"type\", \"probability_value\", \"power_of_test\" FROM \"TestStatisticDefinition\" WHERE \"id\" = 1 LIMIT 1";

			string sqlTestStatistic = "SELECT " + "ABS(\"d1\") AS \"d1\", ABS(\"d2\") AS \"d2\"," + "\"probability_value\",\"power_of_test\"," + "\"quantile\",\"non_centrality_parameter\",\"p_value\" " + "FROM \"TestStatistic\" " + "ORDER BY ABS(\"d1\") ASC, ABS(\"d2\") DESC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sqlDefinition);
			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				TestStatisticType type = TestStatisticType.getEnumByValue(rs.getInt("type"));
				double probabilityValue = options.convertPercentToView(rs.getDouble("probability_value"));
				double powerOfTest = options.convertPercentToView(rs.getDouble("power_of_test"));
				IList<Dictionary<string, Number>> testStatistics = new List<Dictionary<string, Number>>();
				stmt = this.dataBase.getPreparedStatement(sqlTestStatistic);
				rs = stmt.executeQuery();
				ResultSetMetaData rsmd = rs.getMetaData();
				int cnt = rsmd.getColumnCount();
				while (rs.next())
				{
					Dictionary<string, Number> h = new Dictionary<string, Number>();
					for (int i = 1; i <= cnt; i++)
					{
						string key = rsmd.getColumnLabel(i);
						switch (key)
						{
						case "probability_value":
						case "power_of_test":
							h[key] = options.convertPercentToView(rs.getDouble(i));
							break;
						default:
							h[key] = rs.getDouble(i);
							break;
						}
					}
					testStatistics.Add(h);
				}
				this.setParam("test_statistic_method", type.ToString());
				this.setParam("test_statistic_probability_value", probabilityValue);
				this.setParam("test_statistic_power_of_test", powerOfTest);

				if (testStatistics.Count > 0)
				{
					this.setParam("test_statistic_params", testStatistics);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addVarianceComponents() throws java.sql.SQLException
		private void addVarianceComponents()
		{
			IList<IDictionary<string, object>> varianceComponents = new List<IDictionary<string, object>>();

			string sql = "SELECT " + "\"type\",\"redundancy\",\"omega\",\"sigma2apost\",\"number_of_observations\", \"quantile\", " + "\"sigma2apost\" > \"quantile\" AS \"significant\" " + "FROM \"VarianceComponent\" " + "JOIN \"TestStatistic\" ON \"VarianceComponent\".\"redundancy\" = \"TestStatistic\".\"d1\" " + "WHERE \"redundancy\" > 0 " + "AND \"d2\" + 1 = \"d2\" " + "ORDER BY \"type\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			ResultSetMetaData rsmd = rs.getMetaData();
			int cnt = rsmd.getColumnCount();

			while (rs.next())
			{
				VarianceComponentType varianceComponentType = VarianceComponentType.getEnumByValue(rs.getInt("type"));
				if (varianceComponentType == null)
				{
					continue;
				}

				IDictionary<string, object> varianceComponent = new Dictionary<string, object>(6);
				for (int i = 1; i <= cnt; i++)
				{
					string key = rsmd.getColumnLabel(i);
					int type = rsmd.getColumnType(i);
					if (type == Types.INTEGER)
					{
						varianceComponent[key] = rs.getInt(key);
					}
					else if (type == Types.DOUBLE)
					{
						varianceComponent[key] = rs.getDouble(key);
					}
					else if (type == Types.BOOLEAN)
					{
						varianceComponent[key] = rs.getBoolean(key);
					}
					else if (type == Types.VARCHAR)
					{
						varianceComponent[key] = rs.getString(key);
					}
				}
				varianceComponent["type"] = varianceComponentType.ToString();
				varianceComponents.Add(varianceComponent);
			}
			if (varianceComponents.Count > 0)
			{
				this.setParam("vce", varianceComponents);
			}

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addPointGroups() throws java.sql.SQLException
		private void addPointGroups()
		{
			IList<Dictionary<string, object>> groups = new List<Dictionary<string, object>>();

			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(1, PointType.REFERENCE_POINT));
			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(2, PointType.REFERENCE_POINT));
			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(3, PointType.REFERENCE_POINT));

			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(1, PointType.STOCHASTIC_POINT));
			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(2, PointType.STOCHASTIC_POINT));
			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(3, PointType.STOCHASTIC_POINT));

			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(1, PointType.DATUM_POINT));
			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(2, PointType.DATUM_POINT));
			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(3, PointType.DATUM_POINT));

			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(1, PointType.NEW_POINT));
			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(2, PointType.NEW_POINT));
			((List<Dictionary<string, object>>)groups).AddRange(this.getPointGroups(3, PointType.NEW_POINT));

			this.setParam("point_groups", groups);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addVerticalDefelctionGroups() throws java.sql.SQLException
		private void addVerticalDefelctionGroups()
		{
			IList<Dictionary<string, object>> groups = new List<Dictionary<string, object>>();

			((List<Dictionary<string, object>>)groups).AddRange(this.getVerticalDeflectionGroups(VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION));
			((List<Dictionary<string, object>>)groups).AddRange(this.getVerticalDeflectionGroups(VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION));
			((List<Dictionary<string, object>>)groups).AddRange(this.getVerticalDeflectionGroups(VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION));

			this.setParam("vertical_deflection_groups", groups);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addObservations() throws java.sql.SQLException
		private void addObservations()
		{
			IList<Dictionary<string, object>> groups = new List<Dictionary<string, object>>();

			((List<Dictionary<string, object>>)groups).AddRange(this.getObservationGroups(ObservationType.LEVELING));

			((List<Dictionary<string, object>>)groups).AddRange(this.getObservationGroups(ObservationType.DIRECTION));
			((List<Dictionary<string, object>>)groups).AddRange(this.getObservationGroups(ObservationType.HORIZONTAL_DISTANCE));

			((List<Dictionary<string, object>>)groups).AddRange(this.getObservationGroups(ObservationType.SLOPE_DISTANCE));
			((List<Dictionary<string, object>>)groups).AddRange(this.getObservationGroups(ObservationType.ZENITH_ANGLE));

			((List<Dictionary<string, object>>)groups).AddRange(this.getGNSSObservationGroups(ObservationType.GNSS1D));
			((List<Dictionary<string, object>>)groups).AddRange(this.getGNSSObservationGroups(ObservationType.GNSS2D));
			((List<Dictionary<string, object>>)groups).AddRange(this.getGNSSObservationGroups(ObservationType.GNSS3D));

			this.setParam("observation_groups", groups);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addCongruenceAnalysis() throws java.sql.SQLException
		private void addCongruenceAnalysis()
		{
			IList<Dictionary<string, object>> groups = new List<Dictionary<string, object>>();

			((List<Dictionary<string, object>>)groups).AddRange(this.getCongruenceAnalysisPointPairGroups(1));
			((List<Dictionary<string, object>>)groups).AddRange(this.getCongruenceAnalysisPointPairGroups(2));
			((List<Dictionary<string, object>>)groups).AddRange(this.getCongruenceAnalysisPointPairGroups(3));

			this.setParam("congruence_analysis_groups", groups);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addChartAndStatisticValues() throws java.sql.SQLException
		private void addChartAndStatisticValues()
		{
			ObservationType[] terrestrialObservationTypes = new ObservationType[] {ObservationType.LEVELING, ObservationType.DIRECTION, ObservationType.HORIZONTAL_DISTANCE, ObservationType.SLOPE_DISTANCE, ObservationType.ZENITH_ANGLE};

			Dictionary<string, List<Dictionary<string, object>>> chartValues = new Dictionary<string, List<Dictionary<string, object>>>();
			Dictionary<string, List<Dictionary<string, object>>> reliabilitySummary = new Dictionary<string, List<Dictionary<string, object>>>();

			chartValues["redundancy"] = new List<Dictionary<string, object>>(10);
			chartValues["p_prio"] = new List<Dictionary<string, object>>(10);
			chartValues["influence_on_position"] = new List<Dictionary<string, object>>(10);

			reliabilitySummary["redundancy"] = new List<Dictionary<string, object>>(10);
			reliabilitySummary["p_prio"] = new List<Dictionary<string, object>>(10);
			reliabilitySummary["influence_on_position"] = new List<Dictionary<string, object>>(10);

			foreach (ObservationType observationType in terrestrialObservationTypes)
			{
				chartValues["redundancy"].Add(this.getRelativeAndAbsoluteFrequency(observationType, TableRowHighlightType.REDUNDANCY));
				chartValues["p_prio"].Add(this.getRelativeAndAbsoluteFrequency(observationType, TableRowHighlightType.P_PRIO_VALUE));
				chartValues["influence_on_position"].Add(this.getRelativeAndAbsoluteFrequency(observationType, TableRowHighlightType.INFLUENCE_ON_POSITION));

				reliabilitySummary["redundancy"].AddRange(this.getReliabilitySummary(observationType, TableRowHighlightType.REDUNDANCY));
				reliabilitySummary["p_prio"].AddRange(this.getReliabilitySummary(observationType, TableRowHighlightType.P_PRIO_VALUE));
				reliabilitySummary["influence_on_position"].AddRange(this.getReliabilitySummary(observationType, TableRowHighlightType.INFLUENCE_ON_POSITION));
			}

			this.setParam("chart_values", chartValues);
			this.setParam("reliability_summary", reliabilitySummary);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addVectorScale() throws java.sql.SQLException
		private void addVectorScale()
		{
			string sql = "SELECT " + "\"value\" " + "FROM \"LayerEllipseScale\" WHERE \"id\" = 1 LIMIT 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();
			this.setParam("vector_scale", rs.next() ? rs.getDouble("value") : 1.0);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addLayerProperties() throws java.sql.SQLException
		private void addLayerProperties()
		{
			LinkedHashMap<string, Dictionary<string, object>> layers = new LinkedHashMap<string, Dictionary<string, object>>();

			string sql = "SELECT " + "\"type\", \"visible\", \"order\" " + "FROM \"Layer\" " + "ORDER BY \"order\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				LayerType layerType = LayerType.getEnumByValue(rs.getInt("type"));
				if (layerType == null)
				{
					continue;
				}

				bool visible = rs.getBoolean("visible") && !rs.wasNull();
				int order = rs.getInt("order");

				Dictionary<string, object> layer = new Dictionary<string, object>();
				layer["visible"] = visible;
				layer["order"] = order;

				layers.put(layerType.ToString(), layer);
			}

			this.setParam("layers", layers);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private boolean isPointDimensionVisibility(org.applied_geodesy.adjustment.network.PointType pointType, int dim) throws java.sql.SQLException
		private bool isPointDimensionVisibility(PointType pointType, int dim)
		{
			LayerType layerType = null;

			if (pointType == PointType.REFERENCE_POINT)
			{
				layerType = LayerType.REFERENCE_POINT_APOSTERIORI;
			}
			else if (pointType == PointType.STOCHASTIC_POINT)
			{
				layerType = LayerType.STOCHASTIC_POINT_APOSTERIORI;
			}
			else if (pointType == PointType.DATUM_POINT)
			{
				layerType = LayerType.DATUM_POINT_APOSTERIORI;
			}
			else if (pointType == PointType.NEW_POINT)
			{
				layerType = LayerType.NEW_POINT_APOSTERIORI;
			}

			if (layerType != null)
			{
				string sql = "SELECT " + "\"point_1d_visible\", \"point_2d_visible\", \"point_3d_visible\" " + "FROM \"PointLayerProperty\" WHERE \"layer\" = ? LIMIT 1";

				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				stmt.setInt(1, layerType.getId());
				ResultSet rs = stmt.executeQuery();

				if (rs.next())
				{
					if (dim == 1)
					{
						return rs.getBoolean("point_1d_visible");
					}
					else if (dim == 2)
					{
						return rs.getBoolean("point_2d_visible");
					}
					else if (dim == 3)
					{
						return rs.getBoolean("point_3d_visible");
					}
				}
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<java.util.HashMap<String, Object>> getReliabilitySummary(org.applied_geodesy.adjustment.network.ObservationType observationType, org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType tableRowHighlightType) throws java.sql.SQLException
		private IList<Dictionary<string, object>> getReliabilitySummary(ObservationType observationType, TableRowHighlightType tableRowHighlightType)
		{
			IList<Dictionary<string, object>> summary = new List<Dictionary<string, object>>();

			IDictionary<TableRowHighlightType, string[]> subQueryParts = new IDictionary<TableRowHighlightType, string[]>
			{
				{
					TableRowHighlightType.REDUNDANCY, new string[] {"\"redundancy\"", "\"redundancy\"", "MIN(\"redundancy\")", "AVG(\"redundancy\")", "\"redundancy\" > 0"}
				},
				{
					TableRowHighlightType.P_PRIO_VALUE, new string[] {"\"p_prio\"", "\"p_prio\"", "MIN(\"p_prio\")", "AVG(\"p_prio\")", ""}
				},
				{
					TableRowHighlightType.INFLUENCE_ON_POSITION, new string[] {"\"influence_on_position\"", "ABS(\"influence_on_position\")", "MAX(ABS(\"influence_on_position\"))", "AVG(ABS(\"influence_on_position\"))", ""}
				}
			};

			if (!subQueryParts.ContainsKey(tableRowHighlightType))
			{
				return summary;
			}

			string type = subQueryParts[tableRowHighlightType][0];
			string selectType = subQueryParts[tableRowHighlightType][1];
			string minMaxType = subQueryParts[tableRowHighlightType][2];
			string avgType = subQueryParts[tableRowHighlightType][3];
			string constraint = subQueryParts[tableRowHighlightType][4];

			if (string.ReferenceEquals(constraint, null) || constraint.Length == 0)
			{
				constraint = "";
			}
			else
			{
				constraint = " AND " + constraint + " ";
			}

			string template = "SELECT " + "\"start_point_name\", " + "\"end_point_name\", " + "%s AS \"value\", " + "\"name\" AS \"group_name\", " + "(SELECT %s FROM \"ObservationApriori\" " + "JOIN \"ObservationAposteriori\" ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"type\" = ? %s) AS \"average\" " + "FROM \"ObservationApriori\" " + "JOIN \"ObservationAposteriori\" ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"type\" = ? %s AND " + "%s = (SELECT %s FROM \"ObservationApriori\" " + "JOIN \"ObservationAposteriori\" ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"type\" = ? %s)";

			string sql = String.format(Locale.ENGLISH, template, type, avgType, constraint, constraint, selectType, minMaxType, constraint);

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, observationType.getId());
			stmt.setInt(idx++, observationType.getId());
			stmt.setInt(idx++, observationType.getId());

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				Dictionary<string, object> statistics = new Dictionary<string, object>();
				statistics["start_point_name"] = rs.getString("start_point_name");
				statistics["end_point_name"] = rs.getString("end_point_name");
				statistics["group_name"] = rs.getString("group_name");
				statistics["type"] = observationType.ToString();

				if (tableRowHighlightType == TableRowHighlightType.INFLUENCE_ON_POSITION)
				{
					statistics["value"] = options.convertLengthResidualToView(rs.getDouble("value"));
					statistics["average"] = options.convertLengthResidualToView(rs.getDouble("average"));
				}
				else if (tableRowHighlightType == TableRowHighlightType.REDUNDANCY)
				{
					statistics["value"] = options.convertPercentToView(rs.getDouble("value"));
					statistics["average"] = options.convertPercentToView(rs.getDouble("average"));
				}
				else
				{
					statistics["value"] = rs.getDouble("value");
					statistics["average"] = rs.getDouble("average");
				}

				summary.Add(statistics);
			}

			return summary;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private double[] getTableRowHighlightRange(org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType tableRowHighlightType) throws java.sql.SQLException
		private double[] getTableRowHighlightRange(TableRowHighlightType tableRowHighlightType)
		{
			string sql = "SELECT " + "\"left_boundary\", \"right_boundary\" " + "FROM \"TableRowHighlightRange\" " + "WHERE \"type\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, tableRowHighlightType.getId());

			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				double leftBoundary = rs.getDouble("left_boundary");
				double rightBoundary = rs.getDouble("right_boundary");
				return new double[] {leftBoundary, rightBoundary};
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.HashMap<String, Object> getRelativeAndAbsoluteFrequency(org.applied_geodesy.adjustment.network.ObservationType observationType, org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType tableRowHighlightType) throws java.sql.SQLException
		private Dictionary<string, object> getRelativeAndAbsoluteFrequency(ObservationType observationType, TableRowHighlightType tableRowHighlightType)
		{
			Dictionary<string, object> part = new Dictionary<string, object>();

			IDictionary<TableRowHighlightType, string> columnNames = new IDictionary<TableRowHighlightType, string>
			{
				{TableRowHighlightType.REDUNDANCY, "\"redundancy\""},
				{TableRowHighlightType.P_PRIO_VALUE, "\"p_prio\""},
				{TableRowHighlightType.INFLUENCE_ON_POSITION, "ABS(\"influence_on_position\")"}
			};
			double[] range = this.getTableRowHighlightRange(tableRowHighlightType);
			if (range == null || range.Length < 2 || !columnNames.ContainsKey(tableRowHighlightType))
			{
				return part;
			}

			part["type"] = observationType.ToString();
			part["left_boundary"] = tableRowHighlightType == TableRowHighlightType.INFLUENCE_ON_POSITION ? options.convertLengthResidualToView(range[0]) : options.convertPercentToView(range[0]);
			part["right_boundary"] = tableRowHighlightType == TableRowHighlightType.INFLUENCE_ON_POSITION ? options.convertLengthResidualToView(range[1]) : options.convertPercentToView(range[1]);

			if (tableRowHighlightType == TableRowHighlightType.P_PRIO_VALUE)
			{
				range[0] = Math.Log(range[0]);
				range[1] = Math.Log(range[1]);
			}

			string sql = "SELECT COUNT(*) AS \"value\" FROM \"ObservationApriori\" " + "JOIN \"ObservationAposteriori\" ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"type\" = ? " + "UNION ALL " + "SELECT COUNT(*) FROM \"ObservationApriori\" " + "JOIN \"ObservationAposteriori\" ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"type\" = ? AND " + columnNames[tableRowHighlightType] + " < ? " + "UNION ALL " + "SELECT COUNT(*) FROM \"ObservationApriori\" " + "JOIN \"ObservationAposteriori\" ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"type\" = ? AND " + columnNames[tableRowHighlightType] + " BETWEEN ? AND ? " + "UNION ALL " + "SELECT COUNT(*) FROM \"ObservationApriori\" " + "JOIN \"ObservationAposteriori\" ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "JOIN \"ObservationGroup\" ON \"ObservationApriori\".\"group_id\" = \"ObservationGroup\".\"id\" " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE AND \"type\" = ? AND " + columnNames[tableRowHighlightType] + " > ? ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, observationType.getId()); // number of values

			stmt.setInt(idx++, observationType.getId()); // inadequate values
			stmt.setDouble(idx++, range[0]);

			stmt.setInt(idx++, observationType.getId()); // satisfactory values
			stmt.setDouble(idx++, range[0]);
			stmt.setDouble(idx++, range[1]);

			stmt.setInt(idx++, observationType.getId()); // excellent values
			stmt.setDouble(idx++, range[1]);

			ResultSet rs = stmt.executeQuery();

			int[] absoluteFrequency = new int[4];
			int i = 0;
			while (rs.next() && i < absoluteFrequency.Length)
			{
				absoluteFrequency[i++] = rs.getInt("value");
			}

			int numberOfValues = absoluteFrequency[0];
			part["number_of_observations"] = numberOfValues;

			for (int j = 1; numberOfValues > 0 && j < absoluteFrequency.Length; j++)
			{
				part["absolute_frequency_part_" + j] = absoluteFrequency[j];
				part["relative_frequency_part_" + j] = numberOfValues > 0 ? (100.0 * absoluteFrequency[j]) / (double)numberOfValues : 0;
			}

			return part;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<java.util.HashMap<String, Object>> getPointGroups(int dim, org.applied_geodesy.adjustment.network.PointType pointType) throws java.sql.SQLException
		private IList<Dictionary<string, object>> getPointGroups(int dim, PointType pointType)
		{
			IList<Dictionary<string, object>> groups = new List<Dictionary<string, object>>();

			string sqlGroup = "SELECT \"id\", \"name\" FROM \"PointGroup\" WHERE \"dimension\" = ? AND \"type\" = ? AND \"enable\" = TRUE";

			string sqlUncertainty = "SELECT \"type\", \"value\" FROM \"PointGroupUncertainty\" WHERE \"group_id\" = ?";

			string sqlPoint = "SELECT " + "\"name\", \"code\", " + "\"x0\", \"y0\", \"z0\", " + "\"PointAposteriori\".\"sigma_x0\" AS \"sigma_x0\", " + "\"PointAposteriori\".\"sigma_y0\" AS \"sigma_y0\", " + "\"PointAposteriori\".\"sigma_z0\" AS \"sigma_z0\", " + "\"x\", \"y\", \"z\", " + "CASEWHEN(\"sigma_x\" < 0, 0.0, \"sigma_x\") AS \"sigma_x\", " + "CASEWHEN(\"sigma_y\" < 0, 0.0, \"sigma_y\") AS \"sigma_y\", " + "CASEWHEN(\"sigma_z\" < 0, 0.0, \"sigma_z\") AS \"sigma_z\", " + "\"helmert_major_axis\", \"helmert_minor_axis\", \"helmert_alpha\", " + "\"confidence_major_axis\", \"confidence_middle_axis\", \"confidence_minor_axis\", " + "\"confidence_alpha\", \"confidence_beta\", \"confidence_gamma\", " + "\"redundancy_x\", \"redundancy_y\", \"redundancy_z\", " + "\"residual_y\", \"residual_x\", \"residual_z\", " + "\"gross_error_x\", \"gross_error_y\", \"gross_error_z\", " + "\"influence_on_position_x\", \"influence_on_position_y\", \"influence_on_position_z\", \"influence_on_network_distortion\", " + "\"minimal_detectable_bias_x\", \"minimal_detectable_bias_y\", \"minimal_detectable_bias_z\", " + "\"maximum_tolerable_bias_x\", \"maximum_tolerable_bias_y\", \"maximum_tolerable_bias_z\", " + "\"first_principal_component_y\", \"first_principal_component_x\", \"first_principal_component_z\", " + "\"omega\", \"p_prio\", \"p_post\", \"t_prio\", \"t_post\", \"significant\" " + "FROM \"PointApriori\" " + "JOIN \"PointAposteriori\" ON \"PointApriori\".\"id\" = \"PointAposteriori\".\"id\" " + "WHERE \"PointApriori\".\"group_id\" = ? AND \"PointApriori\".\"enable\" = TRUE " + "ORDER BY \"PointApriori\".\"id\" ASC";

			PreparedStatement stmtGroup = this.dataBase.getPreparedStatement(sqlGroup);
			PreparedStatement stmtPoint = this.dataBase.getPreparedStatement(sqlPoint);
			PreparedStatement stmtUncertainty = this.dataBase.getPreparedStatement(sqlUncertainty);

			stmtGroup.setInt(1, dim);
			stmtGroup.setInt(2, pointType.getId());

			ResultSet groupSet = stmtGroup.executeQuery();
			while (groupSet.next())
			{
				double omegaGroup = 0.0, redundancyGroupX = 0.0, redundancyGroupY = 0.0, redundancyGroupZ = 0.0;
				double maxGrossErrorGroupX = 0.0, maxGrossErrorGroupY = 0.0, maxGrossErrorGroupZ = 0.0;
				double maxResidualGroupX = 0.0, maxResidualGroupY = 0.0, maxResidualGroupZ = 0.0;
				bool significantGroup = false;
				int groupId = groupSet.getInt("id");
				stmtPoint.setInt(1, groupId);
				stmtUncertainty.setInt(1, groupId);

				Dictionary<string, object> groupParam = new Dictionary<string, object>();
				Dictionary<string, object> groupUncertainties = new Dictionary<string, object>();
				IList<Dictionary<string, object>> points = new List<Dictionary<string, object>>();

				if (pointType == PointType.STOCHASTIC_POINT)
				{
					ResultSet uncertaintySet = stmtUncertainty.executeQuery();
					while (uncertaintySet.next())
					{
						PointGroupUncertaintyType uncertaintyType = PointGroupUncertaintyType.getEnumByValue(uncertaintySet.getInt("type"));
						double value = uncertaintySet.getDouble("value");
						if (uncertaintyType != null && value > 0)
						{
							groupUncertainties[uncertaintyType.ToString().ToLower()] = options.convertLengthUncertaintyToView(value);
						}
					}
				}

				ResultSet pointSet = stmtPoint.executeQuery();
				ResultSetMetaData rsmd = pointSet.getMetaData();
				int cnt = rsmd.getColumnCount();

				while (pointSet.next())
				{
					Dictionary<string, object> h = new Dictionary<string, object>();

					for (int i = 1; i <= cnt; i++)
					{
						string key = rsmd.getColumnLabel(i);
						switch (key)
						{
						case "x0":
						case "y0":
						case "z0":
						case "x":
						case "y":
						case "z":
							h[key] = options.convertLengthToView(pointSet.getDouble(i));
							break;

						case "sigma_x0":
						case "sigma_y0":
						case "sigma_z0":
						case "sigma_x":
						case "sigma_y":
						case "sigma_z":
						case "confidence_major_axis":
						case "confidence_middle_axis":
						case "confidence_minor_axis":
						case "helmert_major_axis":
						case "helmert_minor_axis":
							h[key] = options.convertLengthUncertaintyToView(pointSet.getDouble(i));
							break;

						case "confidence_alpha":
						case "confidence_beta":
						case "confidence_gamma":
						case "helmert_alpha":
							h[key] = options.convertAngleToView(pointSet.getDouble(i));
							break;

						case "residual_x":
						case "residual_y":
						case "residual_z":
						case "gross_error_x":
						case "gross_error_y":
						case "gross_error_z":
						case "influence_on_position_x":
						case "influence_on_position_y":
						case "influence_on_position_z":
						case "influence_on_network_distortion":
						case "minimal_detectable_bias_x":
						case "minimal_detectable_bias_y":
						case "minimal_detectable_bias_z":
						case "maximum_tolerable_bias_x":
						case "maximum_tolerable_bias_y":
						case "maximum_tolerable_bias_z":
						case "first_principal_component_x":
						case "first_principal_component_y":
						case "first_principal_component_z":
							h[key] = options.convertLengthResidualToView(pointSet.getDouble(i));
							break;

						case "redundancy_x":
						case "redundancy_y":
						case "redundancy_z":
							h[key] = options.convertPercentToView(pointSet.getDouble(i));
							break;

						case "significant":
							h[key] = pointSet.getBoolean(i);
							break;

						default: // Name, code, statistics, etc.
							int type = rsmd.getColumnType(i);
							if (type == Types.CHAR || type == Types.VARCHAR)
							{
								h[key] = pointSet.getString(i);
							}
							else if (type == Types.INTEGER)
							{
								h[key] = pointSet.getInt(i);
							}
							else if (type == Types.DOUBLE)
							{
								h[key] = pointSet.getDouble(i);
							}
							else if (type == Types.BOOLEAN)
							{
								h[key] = pointSet.getBoolean(i);
							}
							break;
						}
					}

					bool significant = pointSet.getBoolean("significant");

					double redundancyX = pointSet.getDouble("redundancy_x");
					double redundancyY = pointSet.getDouble("redundancy_y");
					double redundancyZ = pointSet.getDouble("redundancy_z");

					double grossErrorX = pointSet.getDouble("gross_error_x");
					double grossErrorY = pointSet.getDouble("gross_error_y");
					double grossErrorZ = pointSet.getDouble("gross_error_z");

					double residualX = pointSet.getDouble("residual_x");
					double residualY = pointSet.getDouble("residual_y");
					double residualZ = pointSet.getDouble("residual_z");

					omegaGroup += pointSet.getDouble("omega");
					redundancyGroupX += redundancyX;
					redundancyGroupY += redundancyY;
					redundancyGroupZ += redundancyZ;

					maxGrossErrorGroupX = Math.Abs(grossErrorX) > Math.Abs(maxGrossErrorGroupX) ? grossErrorX : maxGrossErrorGroupX;
					maxGrossErrorGroupY = Math.Abs(grossErrorY) > Math.Abs(maxGrossErrorGroupY) ? grossErrorY : maxGrossErrorGroupY;
					maxGrossErrorGroupZ = Math.Abs(grossErrorZ) > Math.Abs(maxGrossErrorGroupZ) ? grossErrorZ : maxGrossErrorGroupZ;

					maxResidualGroupX = Math.Abs(residualX) > Math.Abs(maxResidualGroupX) ? residualX : maxResidualGroupX;
					maxResidualGroupY = Math.Abs(residualY) > Math.Abs(maxResidualGroupY) ? residualY : maxResidualGroupY;
					maxResidualGroupZ = Math.Abs(residualZ) > Math.Abs(maxResidualGroupZ) ? residualZ : maxResidualGroupZ;

					if (!significantGroup && significant)
					{
						significantGroup = true;
					}

					points.Add(h);
				}

				if (points != null && points.Count > 0)
				{
					groupParam["id"] = groupId;
					groupParam["name"] = groupSet.getString("name");
					groupParam["points"] = points;
					groupParam["omega"] = omegaGroup;
					groupParam["significant"] = significantGroup;
					groupParam["dimension"] = dim;
					groupParam["type"] = pointType.ToString();

					groupParam["redundancy_x"] = redundancyGroupX;
					groupParam["redundancy_y"] = redundancyGroupY;
					groupParam["redundancy_z"] = redundancyGroupZ;
					groupParam["redundancy"] = redundancyGroupX + redundancyGroupY + redundancyGroupZ;

					groupParam["max_gross_error_x"] = options.convertLengthResidualToView(maxGrossErrorGroupX);
					groupParam["max_gross_error_y"] = options.convertLengthResidualToView(maxGrossErrorGroupY);
					groupParam["max_gross_error_z"] = options.convertLengthResidualToView(maxGrossErrorGroupZ);

					groupParam["max_residual_x"] = options.convertLengthResidualToView(maxResidualGroupX);
					groupParam["max_residual_y"] = options.convertLengthResidualToView(maxResidualGroupY);
					groupParam["max_residual_z"] = options.convertLengthResidualToView(maxResidualGroupZ);

					if (groupUncertainties != null && groupUncertainties.Count > 0)
					{
						groupParam["uncertainties"] = groupUncertainties;
					}

					groupParam["visible"] = this.isPointDimensionVisibility(pointType, dim);

					groups.Add(groupParam);
				}
			}

			return groups;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<java.util.HashMap<String, Object>> getVerticalDeflectionGroups(org.applied_geodesy.adjustment.network.VerticalDeflectionType verticalDeflectionType) throws java.sql.SQLException
		private IList<Dictionary<string, object>> getVerticalDeflectionGroups(VerticalDeflectionType verticalDeflectionType)
		{
			IList<Dictionary<string, object>> groups = new List<Dictionary<string, object>>();

			string sqlGroup = "SELECT \"id\", \"name\" FROM \"VerticalDeflectionGroup\" WHERE \"type\" = ? AND \"enable\" = TRUE";

			string sqlUncertainty = "SELECT \"type\", \"value\" FROM \"VerticalDeflectionGroupUncertainty\" WHERE \"group_id\" = ?";

			string sqlDeflection = "SELECT" + "\"name\", " + "\"x0\", \"y0\", " + "\"x\",  \"y\", " + "\"VerticalDeflectionAposteriori\".\"sigma_x0\" AS \"sigma_x0\", " + "\"VerticalDeflectionAposteriori\".\"sigma_y0\" AS \"sigma_y0\", " + "\"redundancy_x\", \"redundancy_y\", " + "\"gross_error_x\", \"gross_error_y\", " + "\"residual_x\", \"residual_y\", " + "\"minimal_detectable_bias_x\", \"minimal_detectable_bias_y\", " + "\"maximum_tolerable_bias_x\", \"maximum_tolerable_bias_y\", " + "CASEWHEN(\"sigma_x\" < 0, 0.0, \"sigma_x\") AS \"sigma_x\", " + "CASEWHEN(\"sigma_y\" < 0, 0.0, \"sigma_y\") AS \"sigma_y\", " + "\"confidence_major_axis\", \"confidence_minor_axis\", " + "\"omega\", \"t_prio\", \"t_post\", \"p_prio\", \"p_post\", \"significant\" " + "FROM \"VerticalDeflectionApriori\" " + "JOIN \"VerticalDeflectionAposteriori\" ON \"VerticalDeflectionApriori\".\"id\" = \"VerticalDeflectionAposteriori\".\"id\" " + "WHERE \"VerticalDeflectionApriori\".\"enable\" = TRUE AND \"VerticalDeflectionApriori\".\"group_id\" = ? " + "ORDER BY \"id\" ASC";


			PreparedStatement stmtGroup = this.dataBase.getPreparedStatement(sqlGroup);
			PreparedStatement stmtDeflection = this.dataBase.getPreparedStatement(sqlDeflection);
			PreparedStatement stmtUncertainty = this.dataBase.getPreparedStatement(sqlUncertainty);

			stmtGroup.setInt(1, verticalDeflectionType.getId());

			ResultSet groupSet = stmtGroup.executeQuery();
			while (groupSet.next())
			{
				double omegaGroup = 0.0, redundancyGroupX = 0.0, redundancyGroupY = 0.0;
				double maxGrossErrorGroupX = 0.0, maxGrossErrorGroupY = 0.0;
				double maxResidualGroupX = 0.0, maxResidualGroupY = 0.0;
				bool significantGroup = false;
				int groupId = groupSet.getInt("id");

				Dictionary<string, object> groupParam = new Dictionary<string, object>();
				Dictionary<string, object> groupUncertainties = new Dictionary<string, object>();
				IList<Dictionary<string, object>> deflections = new List<Dictionary<string, object>>();

				stmtDeflection.setInt(1, groupId);
				stmtUncertainty.setInt(1, groupId);

				if (verticalDeflectionType == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION)
				{
					ResultSet uncertaintySet = stmtUncertainty.executeQuery();
					while (uncertaintySet.next())
					{
						VerticalDeflectionGroupUncertaintyType uncertaintyType = VerticalDeflectionGroupUncertaintyType.getEnumByValue(uncertaintySet.getInt("type"));
						double value = uncertaintySet.getDouble("value");
						if (uncertaintyType != null && value > 0)
						{
							groupUncertainties[uncertaintyType.ToString().ToLower()] = options.convertAngleUncertaintyToView(value);
						}
					}
				}

				ResultSet verticalDeflectionSet = stmtDeflection.executeQuery();
				ResultSetMetaData rsmd = verticalDeflectionSet.getMetaData();

				int cnt = rsmd.getColumnCount();
				while (verticalDeflectionSet.next())
				{
					Dictionary<string, object> h = new Dictionary<string, object>();
					for (int i = 1; i <= cnt; i++)
					{
						string key = rsmd.getColumnLabel(i);
						switch (key)
						{
						case "sigma_x0":
						case "sigma_y0":
						case "sigma_x":
						case "sigma_y":
						case "confidence_major_axis":
						case "confidence_minor_axis":
							h[key] = options.convertAngleUncertaintyToView(verticalDeflectionSet.getDouble(i));
							break;

						case "x0":
						case "y0":
						case "x":
						case "y":
						case "residual_y":
						case "residual_x":
						case "gross_error_x":
						case "gross_error_y":
						case "minimal_detectable_bias_x":
						case "minimal_detectable_bias_y":
						case "maximum_tolerable_bias_x":
						case "maximum_tolerable_bias_y":
							h[key] = options.convertAngleResidualToView(verticalDeflectionSet.getDouble(i));
							break;

						case "redundancy_x":
						case "redundancy_y":
							h[key] = options.convertPercentToView(verticalDeflectionSet.getDouble(i));
							break;

						default: // Statistics
							int type = rsmd.getColumnType(i);
							if (type == Types.CHAR || type == Types.VARCHAR)
							{
								h[key] = verticalDeflectionSet.getString(i);
							}
							else if (type == Types.INTEGER)
							{
								h[key] = verticalDeflectionSet.getInt(i);
							}
							else if (type == Types.DOUBLE)
							{
								h[key] = verticalDeflectionSet.getDouble(i);
							}
							else if (type == Types.BOOLEAN)
							{
								h[key] = verticalDeflectionSet.getBoolean(i);
							}
							break;
						}
					}

					bool significant = verticalDeflectionSet.getBoolean("significant");

					double redundancyX = verticalDeflectionSet.getDouble("redundancy_x");
					double redundancyY = verticalDeflectionSet.getDouble("redundancy_y");

					double grossErrorX = verticalDeflectionSet.getDouble("gross_error_x");
					double grossErrorY = verticalDeflectionSet.getDouble("gross_error_y");

					double residualX = verticalDeflectionSet.getDouble("residual_x");
					double residualY = verticalDeflectionSet.getDouble("residual_y");


					omegaGroup += verticalDeflectionSet.getDouble("omega");
					redundancyGroupX += redundancyX;
					redundancyGroupY += redundancyY;

					maxGrossErrorGroupX = Math.Abs(grossErrorX) > Math.Abs(maxGrossErrorGroupX) ? grossErrorX : maxGrossErrorGroupX;
					maxGrossErrorGroupY = Math.Abs(grossErrorY) > Math.Abs(maxGrossErrorGroupY) ? grossErrorY : maxGrossErrorGroupY;

					maxResidualGroupX = Math.Abs(residualX) > Math.Abs(maxResidualGroupX) ? residualX : maxResidualGroupX;
					maxResidualGroupY = Math.Abs(residualY) > Math.Abs(maxResidualGroupY) ? residualY : maxResidualGroupY;

					if (!significantGroup && significant)
					{
						significantGroup = true;
					}

					deflections.Add(h);
				}

				if (deflections != null && deflections.Count > 0)
				{
					groupParam["id"] = groupId;
					groupParam["name"] = groupSet.getString("name");
					groupParam["deflections"] = deflections;
					groupParam["omega"] = omegaGroup;
					groupParam["significant"] = significantGroup;
					groupParam["type"] = verticalDeflectionType.ToString();

					groupParam["redundancy_x"] = redundancyGroupX;
					groupParam["redundancy_y"] = redundancyGroupY;
					groupParam["redundancy"] = redundancyGroupX + redundancyGroupY;

					groupParam["max_gross_error_x"] = options.convertAngleResidualToView(maxGrossErrorGroupX);
					groupParam["max_gross_error_y"] = options.convertAngleResidualToView(maxGrossErrorGroupY);

					groupParam["max_residual_x"] = options.convertAngleResidualToView(maxResidualGroupX);
					groupParam["max_residual_y"] = options.convertAngleResidualToView(maxResidualGroupY);

					if (groupUncertainties != null && groupUncertainties.Count > 0)
					{
						groupParam["uncertainties"] = groupUncertainties;
					}

					groups.Add(groupParam);
				}
			}
			return groups;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<java.util.HashMap<String, Object>> getObservationGroups(org.applied_geodesy.adjustment.network.ObservationType obsType) throws java.sql.SQLException
		private IList<Dictionary<string, object>> getObservationGroups(ObservationType obsType)
		{
			bool isGNSS = obsType == ObservationType.GNSS1D || obsType == ObservationType.GNSS2D || obsType == ObservationType.GNSS3D;
			if (isGNSS)
			{
				return this.getGNSSObservationGroups(obsType);
			}

			IList<Dictionary<string, object>> groups = new List<Dictionary<string, object>>();

			string sqlGroup = "SELECT \"id\", \"name\" FROM \"ObservationGroup\" WHERE \"type\" = ? AND \"enable\" = TRUE";

			string sqlUncertainty = "SELECT \"type\", \"value\" FROM \"ObservationGroupUncertainty\" WHERE \"group_id\" = ?";

			string sqlObservation = "SELECT " + "\"start_point_name\",\"end_point_name\",\"instrument_height\",\"reflector_height\", " + "\"value_0\",\"distance_0\", " + "\"ObservationAposteriori\".\"sigma_0\" AS \"sigma_0\", " + "\"value\",\"redundancy\",\"residual\",\"gross_error\",\"minimal_detectable_bias\",\"maximum_tolerable_bias\", " + "CASEWHEN(\"sigma\" < 0, 0.0, \"sigma\") AS \"sigma\", " + "\"influence_on_position\",\"influence_on_network_distortion\", " + "\"omega\",\"p_prio\",\"p_post\",\"t_prio\",\"t_post\",\"significant\" " + "FROM \"ObservationApriori\" " + "JOIN \"ObservationAposteriori\" ON \"ObservationApriori\".\"id\" = \"ObservationAposteriori\".\"id\" " + "WHERE \"ObservationApriori\".\"group_id\" = ? " + "AND \"ObservationApriori\".\"enable\" = TRUE " + "ORDER BY \"ObservationApriori\".\"id\" ASC";

			PreparedStatement stmtGroup = this.dataBase.getPreparedStatement(sqlGroup);
			PreparedStatement stmtObservation = this.dataBase.getPreparedStatement(sqlObservation);
			PreparedStatement stmtUncertainty = this.dataBase.getPreparedStatement(sqlUncertainty);

			stmtGroup.setInt(1, obsType.getId());

			ResultSet groupSet = stmtGroup.executeQuery();
			while (groupSet.next())
			{
				bool significantGroup = false;
				double omegaGroup = 0.0, redundancyGroup = 0.0;
				double maxGrossErrorGroup = 0.0, maxResidualGroup = 0.0;
				Dictionary<string, object> groupParam = new Dictionary<string, object>();
				IList<Dictionary<string, object>> observations = new List<Dictionary<string, object>>();
				int groupId = groupSet.getInt("id");
				stmtObservation.setInt(1, groupId);
				stmtUncertainty.setInt(1, groupId);

				ResultSet uncertaintySet = stmtUncertainty.executeQuery();
				Dictionary<string, object> groupUncertainties = new Dictionary<string, object>();
				while (uncertaintySet.next())
				{
					ObservationGroupUncertaintyType uncertaintyType = ObservationGroupUncertaintyType.getEnumByValue(uncertaintySet.getInt("type"));
					if (uncertaintyType != null)
					{
						double value = uncertaintySet.getDouble("value");
						if (value <= 0)
						{
							continue;
						}

						switch (obsType.innerEnumValue)
						{
						case ObservationType.InnerEnum.DIRECTION:
						case ObservationType.InnerEnum.ZENITH_ANGLE:
							switch (uncertaintyType.innerEnumValue)
							{
							case ObservationGroupUncertaintyType.InnerEnum.ZERO_POINT_OFFSET:
								groupUncertainties[uncertaintyType.ToString().ToLower()] = options.convertAngleUncertaintyToView(value);
								break;

							case ObservationGroupUncertaintyType.InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT:
							case ObservationGroupUncertaintyType.InnerEnum.DISTANCE_DEPENDENT:
								groupUncertainties[uncertaintyType.ToString().ToLower()] = options.convertLengthUncertaintyToView(value);
								break;
							}

							break;
						case ObservationType.InnerEnum.LEVELING:
						case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
						case ObservationType.InnerEnum.SLOPE_DISTANCE:
							switch (uncertaintyType.innerEnumValue)
							{
							case ObservationGroupUncertaintyType.InnerEnum.ZERO_POINT_OFFSET:
							case ObservationGroupUncertaintyType.InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT:
								groupUncertainties[uncertaintyType.ToString().ToLower()] = options.convertLengthUncertaintyToView(value);
								break;

							case ObservationGroupUncertaintyType.InnerEnum.DISTANCE_DEPENDENT:
								groupUncertainties[uncertaintyType.ToString().ToLower()] = options.convertScaleUncertaintyToView(value);
								break;
							}

							break;
						default: // GNSS
							break;
						}
					}
				}

				ResultSet observationSet = stmtObservation.executeQuery();
				ResultSetMetaData rsmd = observationSet.getMetaData();
				int cnt = rsmd.getColumnCount();

				while (observationSet.next())
				{
					Dictionary<string, object> h = new Dictionary<string, object>();
					for (int i = 1; i <= cnt; i++)
					{
						string key = rsmd.getColumnLabel(i);
						switch (key)
						{

						case "instrument_height":
						case "reflector_height":
						case "distance_0":
							h[key] = options.convertLengthToView(observationSet.getDouble(i));
							break;

						case "value_0":
						case "value":
							switch (obsType.innerEnumValue)
							{
							case ObservationType.InnerEnum.DIRECTION:
							case ObservationType.InnerEnum.ZENITH_ANGLE:
								h[key] = options.convertAngleToView(observationSet.getDouble(i));
								break;
							default:
								h[key] = options.convertLengthToView(observationSet.getDouble(i));
								break;
							}
							break;

						case "sigma_0":
						case "sigma":
							switch (obsType.innerEnumValue)
							{
							case ObservationType.InnerEnum.DIRECTION:
							case ObservationType.InnerEnum.ZENITH_ANGLE:
								h[key] = options.convertAngleUncertaintyToView(observationSet.getDouble(i));
								break;
							default:
								h[key] = options.convertLengthUncertaintyToView(observationSet.getDouble(i));
								break;
							}
							break;

						case "gross_error":
						case "minimal_detectable_bias":
						case "maximum_tolerable_bias":
						case "residual":
							switch (obsType.innerEnumValue)
							{
							case ObservationType.InnerEnum.DIRECTION:
							case ObservationType.InnerEnum.ZENITH_ANGLE:
								h[key] = options.convertAngleResidualToView(observationSet.getDouble(i));
								break;
							default:
								h[key] = options.convertLengthResidualToView(observationSet.getDouble(i));
								break;
							}
							break;

						case "influence_on_position":
						case "influence_on_network_distortion":
							h[key] = options.convertLengthResidualToView(observationSet.getDouble(i));
							break;

						case "redundancy":
							h[key] = options.convertPercentToView(observationSet.getDouble(i));
							break;

						default: // Point names, statistics, etc.
							int type = rsmd.getColumnType(i);
							if (type == Types.CHAR || type == Types.VARCHAR)
							{
								h[key] = observationSet.getString(i);
							}
							else if (type == Types.INTEGER)
							{
								h[key] = observationSet.getInt(i);
							}
							else if (type == Types.DOUBLE)
							{
								h[key] = observationSet.getDouble(i);
							}
							else if (type == Types.BOOLEAN)
							{
								h[key] = observationSet.getBoolean(i);
							}
							break;
						}
					}

					bool significant = observationSet.getBoolean("significant");
					double redundancy = observationSet.getDouble("redundancy");
					double grossError = observationSet.getDouble("gross_error");
					double residual = observationSet.getDouble("residual");

					omegaGroup += observationSet.getDouble("omega");
					redundancyGroup += redundancy;
					maxGrossErrorGroup = Math.Abs(grossError) > Math.Abs(maxGrossErrorGroup) ? grossError : maxGrossErrorGroup;
					maxResidualGroup = Math.Abs(residual) > Math.Abs(maxResidualGroup) ? residual : maxResidualGroup;
					if (!significantGroup && significant)
					{
						significantGroup = true;
					}

					observations.Add(h);
				}

				if (observations != null && observations.Count > 0)
				{
					switch (obsType.innerEnumValue)
					{
					case ObservationType.InnerEnum.DIRECTION:
					case ObservationType.InnerEnum.ZENITH_ANGLE:
						groupParam["type"] = obsType.ToString();
						groupParam["max_gross_error"] = options.convertAngleResidualToView(maxGrossErrorGroup);
						groupParam["max_residual"] = options.convertAngleResidualToView(maxResidualGroup);
						break;

					case ObservationType.InnerEnum.LEVELING:
					case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
					case ObservationType.InnerEnum.SLOPE_DISTANCE:
						groupParam["type"] = obsType.ToString();
						groupParam["max_gross_error"] = options.convertLengthResidualToView(maxGrossErrorGroup);
						groupParam["max_residual"] = options.convertLengthResidualToView(maxResidualGroup);
						break;
					default:
						continue;
						//break;
					}

					groupParam["id"] = groupId;
					groupParam["name"] = groupSet.getString("name");
					groupParam["observations"] = observations;
					groupParam["dimension"] = 1;
					groupParam["omega"] = omegaGroup;
					groupParam["redundancy"] = redundancyGroup;
					groupParam["significant"] = significantGroup;

					if (groupUncertainties != null && groupUncertainties.Count > 0)
					{
						groupParam["uncertainties"] = groupUncertainties;
					}

					IList<Dictionary<string, object>> parameters = this.getAddionalParameters(groupId);

					if (parameters != null && parameters.Count > 0)
					{
						groupParam["unknown_parameters"] = parameters;
					}

					groups.Add(groupParam);
				}
			}
			return groups;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<java.util.HashMap<String, Object>> getGNSSObservationGroups(org.applied_geodesy.adjustment.network.ObservationType obsType) throws java.sql.SQLException
		private IList<Dictionary<string, object>> getGNSSObservationGroups(ObservationType obsType)
		{
			int dim = obsType == ObservationType.GNSS1D ? 1 : obsType == ObservationType.GNSS2D ? 2 : 3;
			IList<Dictionary<string, object>> groups = new List<Dictionary<string, object>>();

			string sqlGroup = "SELECT \"id\", \"name\" FROM \"ObservationGroup\" WHERE \"type\" = ? AND \"enable\" = TRUE";

			string sqlUncertainty = "SELECT \"type\", \"value\" FROM \"ObservationGroupUncertainty\" WHERE \"group_id\" = ?";

			string sqlObservation = "SELECT " + "\"start_point_name\",\"end_point_name\",\"y0\",\"x0\",\"z0\", " + "0 AS \"instrument_height\", " + "0 AS \"reflector_height\", " + "SQRT(\"y0\"*\"y0\" + \"x0\"*\"x0\" + \"z0\"*\"z0\") AS \"distance_0\", " + "\"GNSSObservationAposteriori\".\"sigma_x0\" AS \"sigma_x0\", " + "\"GNSSObservationAposteriori\".\"sigma_y0\" AS \"sigma_y0\", " + "\"GNSSObservationAposteriori\".\"sigma_z0\" AS \"sigma_z0\", " + "CASEWHEN(\"sigma_x\" < 0, 0.0, \"sigma_x\") AS \"sigma_x\", " + "CASEWHEN(\"sigma_y\" < 0, 0.0, \"sigma_y\") AS \"sigma_y\", " + "CASEWHEN(\"sigma_z\" < 0, 0.0, \"sigma_z\") AS \"sigma_z\", " + "\"y\",\"x\",\"z\"," + "\"residual_y\", \"residual_x\", \"residual_z\", " + "\"redundancy_y\",\"redundancy_x\",\"redundancy_z\"," + "\"gross_error_y\",\"gross_error_x\",\"gross_error_z\"," + "\"minimal_detectable_bias_y\",\"minimal_detectable_bias_x\",\"minimal_detectable_bias_z\"," + "\"maximum_tolerable_bias_y\",\"maximum_tolerable_bias_x\",\"maximum_tolerable_bias_z\"," + "\"influence_on_position_y\",\"influence_on_position_x\",\"influence_on_position_z\"," + "\"influence_on_network_distortion\"," + "\"omega\",\"p_prio\",\"p_post\",\"t_prio\",\"t_post\",\"significant\" " + "FROM \"GNSSObservationApriori\" " + "JOIN \"GNSSObservationAposteriori\" ON \"GNSSObservationApriori\".\"id\" = \"GNSSObservationAposteriori\".\"id\" " + "WHERE \"GNSSObservationApriori\".\"group_id\" = ? " + "AND \"GNSSObservationApriori\".\"enable\" = TRUE " + "ORDER BY \"GNSSObservationApriori\".\"id\" ASC";

			PreparedStatement stmtGroup = this.dataBase.getPreparedStatement(sqlGroup);
			PreparedStatement stmtObservation = this.dataBase.getPreparedStatement(sqlObservation);
			PreparedStatement stmtUncertainty = this.dataBase.getPreparedStatement(sqlUncertainty);

			stmtGroup.setInt(1, obsType.getId());

			ResultSet groupSet = stmtGroup.executeQuery();
			while (groupSet.next())
			{
				double omegaGroup = 0.0, redundancyGroupX = 0.0, redundancyGroupY = 0.0, redundancyGroupZ = 0.0;
				double maxGrossErrorGroupX = 0.0, maxGrossErrorGroupY = 0.0, maxGrossErrorGroupZ = 0.0;
				double maxResidualGroupX = 0.0, maxResidualGroupY = 0.0, maxResidualGroupZ = 0.0;
				bool significantGroup = false;
				Dictionary<string, object> groupParam = new Dictionary<string, object>();
				IList<Dictionary<string, object>> observations = new List<Dictionary<string, object>>();

				int groupId = groupSet.getInt("id");
				stmtObservation.setInt(1, groupId);
				stmtUncertainty.setInt(1, groupId);

				ResultSet uncertaintySet = stmtUncertainty.executeQuery();
				Dictionary<string, object> groupUncertainties = new Dictionary<string, object>();
				while (uncertaintySet.next())
				{
					ObservationGroupUncertaintyType uncertaintyType = ObservationGroupUncertaintyType.getEnumByValue(uncertaintySet.getInt("type"));
					if (uncertaintyType != null)
					{
						double value = uncertaintySet.getDouble("value");
						if (value <= 0)
						{
							continue;
						}

						switch (obsType.innerEnumValue)
						{
						case ObservationType.InnerEnum.GNSS1D:
						case ObservationType.InnerEnum.GNSS2D:
						case ObservationType.InnerEnum.GNSS3D:
							switch (uncertaintyType.innerEnumValue)
							{
							case ObservationGroupUncertaintyType.InnerEnum.ZERO_POINT_OFFSET:
							case ObservationGroupUncertaintyType.InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT:
								groupUncertainties[uncertaintyType.ToString().ToLower()] = options.convertLengthUncertaintyToView(value);
								break;

							case ObservationGroupUncertaintyType.InnerEnum.DISTANCE_DEPENDENT:
								groupUncertainties[uncertaintyType.ToString().ToLower()] = options.convertScaleUncertaintyToView(value);
								break;
							}

							break;
						default: // terrestrial observations
							break;
						}
					}
				}

				ResultSet observationSet = stmtObservation.executeQuery();
				ResultSetMetaData rsmd = observationSet.getMetaData();
				int cnt = rsmd.getColumnCount();

				while (observationSet.next())
				{

					Dictionary<string, object> h = new Dictionary<string, object>();
					for (int i = 1; i <= cnt; i++)
					{
						string key = rsmd.getColumnLabel(i);
						switch (key)
						{
						case "instrument_height":
						case "reflector_height":
						case "distance_0":
						case "x0":
						case "y0":
						case "z0":
						case "x":
						case "y":
						case "z":
							h[key] = options.convertLengthToView(observationSet.getDouble(i));
							break;

						case "sigma_x0":
						case "sigma_y0":
						case "sigma_z0":
						case "sigma_x":
						case "sigma_y":
						case "sigma_z":
							h[key] = options.convertLengthUncertaintyToView(observationSet.getDouble(i));
							break;

						case "gross_error_x":
						case "gross_error_y":
						case "gross_error_z":
						case "residual_x":
						case "residual_y":
						case "residual_z":
						case "minimal_detectable_bias_x":
						case "minimal_detectable_bias_y":
						case "minimal_detectable_bias_z":
						case "maximum_tolerable_bias_x":
						case "maximum_tolerable_bias_y":
						case "maximum_tolerable_bias_z":
						case "influence_on_position_x":
						case "influence_on_position_y":
						case "influence_on_position_z":
						case "influence_on_network_distortion":
							h[key] = options.convertLengthResidualToView(observationSet.getDouble(i));
							break;

						case "redundancy_x":
						case "redundancy_y":
						case "redundancy_z":
							h[key] = options.convertPercentToView(observationSet.getDouble(i));
							break;

						default: // Point names, statistics, etc.
							int type = rsmd.getColumnType(i);
							if (type == Types.CHAR || type == Types.VARCHAR)
							{
								h[key] = observationSet.getString(i);
							}
							else if (type == Types.INTEGER)
							{
								h[key] = observationSet.getInt(i);
							}
							else if (type == Types.DOUBLE)
							{
								h[key] = observationSet.getDouble(i);
							}
							else if (type == Types.BOOLEAN)
							{
								h[key] = observationSet.getBoolean(i);
							}
							break;
						}
					}

					bool significant = observationSet.getBoolean("significant");

					double redundancyX = observationSet.getDouble("redundancy_x");
					double redundancyY = observationSet.getDouble("redundancy_y");
					double redundancyZ = observationSet.getDouble("redundancy_z");

					double grossErrorX = observationSet.getDouble("gross_error_x");
					double grossErrorY = observationSet.getDouble("gross_error_y");
					double grossErrorZ = observationSet.getDouble("gross_error_z");

					double residualX = observationSet.getDouble("residual_x");
					double residualY = observationSet.getDouble("residual_y");
					double residualZ = observationSet.getDouble("residual_z");

					omegaGroup += observationSet.getDouble("omega");
					redundancyGroupX += redundancyX;
					redundancyGroupY += redundancyY;
					redundancyGroupZ += redundancyZ;

					maxGrossErrorGroupX = Math.Abs(grossErrorX) > Math.Abs(maxGrossErrorGroupX) ? grossErrorX : maxGrossErrorGroupX;
					maxGrossErrorGroupY = Math.Abs(grossErrorY) > Math.Abs(maxGrossErrorGroupY) ? grossErrorY : maxGrossErrorGroupY;
					maxGrossErrorGroupZ = Math.Abs(grossErrorZ) > Math.Abs(maxGrossErrorGroupZ) ? grossErrorZ : maxGrossErrorGroupZ;

					maxResidualGroupX = Math.Abs(residualX) > Math.Abs(maxResidualGroupX) ? residualX : maxResidualGroupX;
					maxResidualGroupY = Math.Abs(residualY) > Math.Abs(maxResidualGroupY) ? residualY : maxResidualGroupY;
					maxResidualGroupZ = Math.Abs(residualZ) > Math.Abs(maxResidualGroupZ) ? residualZ : maxResidualGroupZ;

					if (!significantGroup && significant)
					{
						significant = true;
					}

					observations.Add(h);
				}

				if (observations != null && observations.Count > 0)
				{
					groupParam["id"] = groupId;
					groupParam["name"] = groupSet.getString("name");
					groupParam["observations"] = observations;
					groupParam["omega"] = omegaGroup;
					groupParam["significant"] = significantGroup;
					groupParam["dimension"] = dim;

					switch (obsType.innerEnumValue)
					{
					case ObservationType.InnerEnum.GNSS1D:
					case ObservationType.InnerEnum.GNSS2D:
					case ObservationType.InnerEnum.GNSS3D:
						groupParam["type"] = obsType.ToString();
						break;
					default:
						continue;
						//break;
					}

					groupParam["redundancy_x"] = redundancyGroupX;
					groupParam["redundancy_y"] = redundancyGroupY;
					groupParam["redundancy_z"] = redundancyGroupZ;
					groupParam["redundancy"] = redundancyGroupX + redundancyGroupY + redundancyGroupZ;

					groupParam["max_gross_error_x"] = options.convertLengthResidualToView(maxGrossErrorGroupX);
					groupParam["max_gross_error_y"] = options.convertLengthResidualToView(maxGrossErrorGroupY);
					groupParam["max_gross_error_z"] = options.convertLengthResidualToView(maxGrossErrorGroupZ);

					groupParam["max_residual_x"] = options.convertLengthResidualToView(maxResidualGroupX);
					groupParam["max_residual_y"] = options.convertLengthResidualToView(maxResidualGroupY);
					groupParam["max_residual_z"] = options.convertLengthResidualToView(maxResidualGroupZ);

					if (groupUncertainties != null && groupUncertainties.Count > 0)
					{
						groupParam["uncertainties"] = groupUncertainties;
					}

					IList<Dictionary<string, object>> parameters = this.getAddionalParameters(groupId);

					if (parameters != null && parameters.Count > 0)
					{
						groupParam["unknown_parameters"] = parameters;
					}

					groups.Add(groupParam);
				}
			}
			return groups;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<java.util.HashMap<String, Object>> getAddionalParameters(int groupId) throws java.sql.SQLException
		private IList<Dictionary<string, object>> getAddionalParameters(int groupId)
		{
			IList<Dictionary<string, object>> parameters = new List<Dictionary<string, object>>(5);

			string sql = "SELECT " + "\"type\",\"value_0\", " + "\"value\", \"sigma\",\"confidence\",\"gross_error\",\"minimal_detectable_bias\"," + "\"p_prio\",\"p_post\",\"t_prio\",\"t_post\",\"significant\" " + "FROM \"AdditionalParameterApriori\" " + "JOIN \"AdditionalParameterAposteriori\" " + "ON \"AdditionalParameterApriori\".\"id\" = \"AdditionalParameterAposteriori\".\"id\" " + "WHERE \"AdditionalParameterApriori\".\"group_id\" = ? " + "AND \"AdditionalParameterApriori\".\"enable\" = TRUE " + "ORDER BY \"AdditionalParameterApriori\".\"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setInt(idx++, groupId);
			ResultSet rs = stmt.executeQuery();
			ResultSetMetaData rsmd = rs.getMetaData();
			int cnt = rsmd.getColumnCount();

			while (rs.next())
			{
				ParameterType parameterType = ParameterType.getEnumByValue(rs.getInt("type"));
				if (parameterType == null)
				{
					continue;
				}

				Dictionary<string, object> h = new Dictionary<string, object>();
				for (int i = 1; i <= cnt; i++)
				{
					string key = rsmd.getColumnLabel(i);
					switch (key)
					{
					case "type":
						switch (parameterType.innerEnumValue)
						{
						case ParameterType.InnerEnum.ORIENTATION:
							h[key] = "ORIENTATION";
							break;

						case ParameterType.InnerEnum.REFRACTION_INDEX:
							h[key] = "REFRACTION_INDEX";
							break;

						case ParameterType.InnerEnum.SCALE:
							h[key] = "SCALE";
							break;

						case ParameterType.InnerEnum.ZERO_POINT_OFFSET:
							h[key] = "ZERO_POINT_OFFSET";
							break;

						case ParameterType.InnerEnum.ROTATION_X:
							h[key] = "ROTATION_X";
							break;

						case ParameterType.InnerEnum.ROTATION_Y:
							h[key] = "ROTATION_Y";
							break;

						case ParameterType.InnerEnum.ROTATION_Z:
							h[key] = "ROTATION_Z";
							break;

						default:
							break;

						}
						break;

					case "value_0":
					case "value":
						switch (parameterType.innerEnumValue)
						{
						case ParameterType.InnerEnum.ORIENTATION:
						case ParameterType.InnerEnum.ROTATION_X:
						case ParameterType.InnerEnum.ROTATION_Y:
						case ParameterType.InnerEnum.ROTATION_Z:
							h[key] = options.convertAngleToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.ZERO_POINT_OFFSET:
							h[key] = options.convertLengthToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.SCALE:
							h[key] = options.convertScaleToView(rs.getDouble(i));
							break;

						default:
							h[key] = rs.getDouble(i);
							break;
						}

						break;

					case "sigma":
					case "confidence":
						switch (parameterType.innerEnumValue)
						{
						case ParameterType.InnerEnum.ORIENTATION:
						case ParameterType.InnerEnum.ROTATION_X:
						case ParameterType.InnerEnum.ROTATION_Y:
						case ParameterType.InnerEnum.ROTATION_Z:
							h[key] = options.convertAngleUncertaintyToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.ZERO_POINT_OFFSET:
							h[key] = options.convertLengthUncertaintyToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.SCALE:
							h[key] = options.convertScaleUncertaintyToView(rs.getDouble(i));
							break;

						default:
							h[key] = rs.getDouble(i);
							break;
						}

						break;

					case "gross_error":
					case "minimal_detectable_bias":
						switch (parameterType.innerEnumValue)
						{
						case ParameterType.InnerEnum.ORIENTATION:
						case ParameterType.InnerEnum.ROTATION_X:
						case ParameterType.InnerEnum.ROTATION_Y:
						case ParameterType.InnerEnum.ROTATION_Z:
							h[key] = options.convertAngleResidualToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.ZERO_POINT_OFFSET:
							h[key] = options.convertLengthResidualToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.SCALE:
							h[key] = options.convertScaleResidualToView(rs.getDouble(i));
							break;

						default:
							h[key] = rs.getDouble(i);
							break;
						}

						break;

					default: // statistics, etc.
						int type = rsmd.getColumnType(i);
						if (type == Types.CHAR || type == Types.VARCHAR)
						{
							h[key] = rs.getString(i);
						}
						else if (type == Types.INTEGER)
						{
							h[key] = rs.getInt(i);
						}
						else if (type == Types.DOUBLE)
						{
							h[key] = rs.getDouble(i);
						}
						else if (type == Types.BOOLEAN)
						{
							h[key] = rs.getBoolean(i);
						}
						break;
					}
				}

				parameters.Add(h);
			}

			return parameters;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<java.util.HashMap<String, Object>> getCongruenceAnalysisPointPairGroups(int dim) throws java.sql.SQLException
		private IList<Dictionary<string, object>> getCongruenceAnalysisPointPairGroups(int dim)
		{
			IList<Dictionary<string, object>> groups = new List<Dictionary<string, object>>();

			string sqlGroup = "SELECT \"id\", \"name\" " + "FROM \"CongruenceAnalysisGroup\" " + "WHERE \"enable\" = TRUE AND \"dimension\" = ?";

			string sqlPointPairs = "SELECT " + "\"start_point_name\", \"end_point_name\", " + "\"y\",\"x\",\"z\"," + "\"sigma_y\",\"sigma_x\",\"sigma_z\"," + "\"confidence_major_axis\",\"confidence_middle_axis\",\"confidence_minor_axis\"," + "\"confidence_alpha\",\"confidence_beta\",\"confidence_gamma\"," + "\"confidence_major_axis_2d\",\"confidence_minor_axis_2d\"," + "\"confidence_alpha_2d\"," + "\"gross_error_y\",\"gross_error_x\",\"gross_error_z\"," + "\"minimal_detectable_bias_y\",\"minimal_detectable_bias_x\",\"minimal_detectable_bias_z\"," + "\"p_prio\",\"p_post\",\"t_prio\",\"t_post\",\"significant\" " + "FROM \"CongruenceAnalysisPointPairApriori\" " + "JOIN \"CongruenceAnalysisPointPairAposteriori\" ON \"CongruenceAnalysisPointPairApriori\".\"id\" = \"CongruenceAnalysisPointPairAposteriori\".\"id\" " + "WHERE \"CongruenceAnalysisPointPairApriori\".\"group_id\" = ? " + "AND \"CongruenceAnalysisPointPairApriori\".\"enable\" = TRUE " + "ORDER BY \"CongruenceAnalysisPointPairApriori\".\"id\" ASC";

			PreparedStatement stmtGroup = this.dataBase.getPreparedStatement(sqlGroup);
			stmtGroup.setInt(1, dim);

			ResultSet groupSet = stmtGroup.executeQuery();
			while (groupSet.next())
			{
				bool significantGroup = false;
				double maxGrossErrorGroupX = 0, maxGrossErrorGroupY = 0, maxGrossErrorGroupZ = 0;
				Dictionary<string, object> groupParam = new Dictionary<string, object>();
				int groupId = groupSet.getInt("id");

				IList<Dictionary<string, object>> pointPairs = new List<Dictionary<string, object>>();
				PreparedStatement stmtPointPair = this.dataBase.getPreparedStatement(sqlPointPairs);
				stmtPointPair.setInt(1, groupId);

				ResultSet pointPairSet = stmtPointPair.executeQuery();
				ResultSetMetaData rsmd = pointPairSet.getMetaData();
				int cnt = rsmd.getColumnCount();

				while (pointPairSet.next())
				{
					Dictionary<string, object> h = new Dictionary<string, object>();

					for (int i = 1; i <= cnt; i++)
					{
						string key = rsmd.getColumnLabel(i);
						switch (key)
						{
						case "x":
						case "y":
						case "z":
							h[key] = options.convertLengthToView(pointPairSet.getDouble(i));
							break;

						case "sigma_x":
						case "sigma_y":
						case "sigma_z":
						case "confidence_major_axis":
						case "confidence_middle_axis":
						case "confidence_minor_axis":
						case "confidence_major_axis_2d":
						case "confidence_minor_axis_2d":
							h[key] = options.convertLengthUncertaintyToView(pointPairSet.getDouble(i));
							break;

						case "confidence_alpha":
						case "confidence_beta":
						case "confidence_gamma":
						case "confidence_alpha_2d":
							h[key] = options.convertAngleToView(pointPairSet.getDouble(i));
							break;

						case "gross_error_x":
						case "gross_error_y":
						case "gross_error_z":
						case "minimal_detectable_bias_x":
						case "minimal_detectable_bias_y":
						case "minimal_detectable_bias_z":
							h[key] = options.convertLengthResidualToView(pointPairSet.getDouble(i));
							break;

						default: // Point names, statistics, etc.
							int type = rsmd.getColumnType(i);
							if (type == Types.CHAR || type == Types.VARCHAR)
							{
								h[key] = pointPairSet.getString(i);
							}
							else if (type == Types.INTEGER)
							{
								h[key] = pointPairSet.getInt(i);
							}
							else if (type == Types.DOUBLE)
							{
								h[key] = pointPairSet.getDouble(i);
							}
							else if (type == Types.BOOLEAN)
							{
								h[key] = pointPairSet.getBoolean(i);
							}
							break;
						}
					}

					bool significant = pointPairSet.getBoolean("significant");

					double grossErrorX = pointPairSet.getDouble("gross_error_x");
					double grossErrorY = pointPairSet.getDouble("gross_error_y");
					double grossErrorZ = pointPairSet.getDouble("gross_error_z");

					maxGrossErrorGroupX = Math.Abs(grossErrorX) > Math.Abs(maxGrossErrorGroupX) ? grossErrorX : maxGrossErrorGroupX;
					maxGrossErrorGroupY = Math.Abs(grossErrorY) > Math.Abs(maxGrossErrorGroupY) ? grossErrorY : maxGrossErrorGroupY;
					maxGrossErrorGroupZ = Math.Abs(grossErrorZ) > Math.Abs(maxGrossErrorGroupZ) ? grossErrorZ : maxGrossErrorGroupZ;

					if (!significantGroup && significant)
					{
						significantGroup = true;
					}

					pointPairs.Add(h);
				}

				if (pointPairs != null && pointPairs.Count > 0)
				{
					groupParam["id"] = groupId;
					groupParam["name"] = groupSet.getString("name");
					groupParam["dimension"] = dim;
					groupParam["point_pairs"] = pointPairs;
					groupParam["significant"] = significantGroup;
					groupParam["max_gross_error_x"] = options.convertLengthResidualToView(maxGrossErrorGroupX);
					groupParam["max_gross_error_y"] = options.convertLengthResidualToView(maxGrossErrorGroupY);
					groupParam["max_gross_error_z"] = options.convertLengthResidualToView(maxGrossErrorGroupZ);

					IList<Dictionary<string, object>> strainParams = this.getStrainParameters(groupId);
					if (strainParams != null && strainParams.Count > 0)
					{
						groupParam["strain_parameters"] = strainParams;
					}

					groups.Add(groupParam);
				}
			}
			return groups;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<java.util.HashMap<String, Object>> getStrainParameters(int groupId) throws java.sql.SQLException
		private IList<Dictionary<string, object>> getStrainParameters(int groupId)
		{
			IList<Dictionary<string, object>> @params = new List<Dictionary<string, object>>();

			string sql = "SELECT " + "\"type\",\"value\",\"sigma\",\"confidence\"," + "\"gross_error\",\"minimal_detectable_bias\"," + "\"p_prio\",\"p_post\",\"t_prio\",\"t_post\",\"significant\" " + "FROM \"CongruenceAnalysisStrainParameterAposteriori\" " + "WHERE \"group_id\" = ? ORDER BY \"type\" ASC";


			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setInt(idx++, groupId);

			ResultSet rs = stmt.executeQuery();
			ResultSetMetaData rsmd = rs.getMetaData();
			int cnt = rsmd.getColumnCount();

			while (rs.next())
			{
				Dictionary<string, object> h = new Dictionary<string, object>();

				ParameterType parameterType = ParameterType.getEnumByValue(rs.getInt("type"));
				if (parameterType == null)
				{
					continue;
				}

				for (int i = 1; i <= cnt; i++)
				{
					string key = rsmd.getColumnLabel(i);
					switch (key)
					{

					case "type":
						h[key] = parameterType.ToString();
						break;

					case "value":
						switch (parameterType.innerEnumValue)
						{
						case ParameterType.InnerEnum.STRAIN_ROTATION_X:
						case ParameterType.InnerEnum.STRAIN_ROTATION_Y:
						case ParameterType.InnerEnum.STRAIN_ROTATION_Z:
						case ParameterType.InnerEnum.STRAIN_SHEAR_X:
						case ParameterType.InnerEnum.STRAIN_SHEAR_Y:
						case ParameterType.InnerEnum.STRAIN_SHEAR_Z:
							h[key] = options.convertAngleToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.STRAIN_SCALE_X:
						case ParameterType.InnerEnum.STRAIN_SCALE_Y:
						case ParameterType.InnerEnum.STRAIN_SCALE_Z:
							h[key] = options.convertScaleToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.STRAIN_TRANSLATION_X:
						case ParameterType.InnerEnum.STRAIN_TRANSLATION_Y:
						case ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
							h[key] = options.convertLengthToView(rs.getDouble(i));
							break;

						default:
							continue;
							// break;
						}
						break;

					case "sigma":
					case "confidence":
						switch (parameterType.innerEnumValue)
						{
						case ParameterType.InnerEnum.STRAIN_ROTATION_X:
						case ParameterType.InnerEnum.STRAIN_ROTATION_Y:
						case ParameterType.InnerEnum.STRAIN_ROTATION_Z:
						case ParameterType.InnerEnum.STRAIN_SHEAR_X:
						case ParameterType.InnerEnum.STRAIN_SHEAR_Y:
						case ParameterType.InnerEnum.STRAIN_SHEAR_Z:
							h[key] = options.convertAngleUncertaintyToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.STRAIN_SCALE_X:
						case ParameterType.InnerEnum.STRAIN_SCALE_Y:
						case ParameterType.InnerEnum.STRAIN_SCALE_Z:
							h[key] = options.convertScaleUncertaintyToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.STRAIN_TRANSLATION_X:
						case ParameterType.InnerEnum.STRAIN_TRANSLATION_Y:
						case ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
							h[key] = options.convertLengthUncertaintyToView(rs.getDouble(i));
							break;

						default:
							continue;
							// break;
						}
						break;

					case "gross_error":
					case "minimal_detectable_bias":
						switch (parameterType.innerEnumValue)
						{
						case ParameterType.InnerEnum.STRAIN_ROTATION_X:
						case ParameterType.InnerEnum.STRAIN_ROTATION_Y:
						case ParameterType.InnerEnum.STRAIN_ROTATION_Z:
						case ParameterType.InnerEnum.STRAIN_SHEAR_X:
						case ParameterType.InnerEnum.STRAIN_SHEAR_Y:
						case ParameterType.InnerEnum.STRAIN_SHEAR_Z:
							h[key] = options.convertAngleResidualToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.STRAIN_SCALE_X:
						case ParameterType.InnerEnum.STRAIN_SCALE_Y:
						case ParameterType.InnerEnum.STRAIN_SCALE_Z:
							h[key] = options.convertScaleResidualToView(rs.getDouble(i));
							break;

						case ParameterType.InnerEnum.STRAIN_TRANSLATION_X:
						case ParameterType.InnerEnum.STRAIN_TRANSLATION_Y:
						case ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
							h[key] = options.convertLengthResidualToView(rs.getDouble(i));
							break;

						default:
							continue;
							// break;
						}
						break;

					default: // Point names, statistics, etc.
						int type = rsmd.getColumnType(i);
						if (type == Types.CHAR || type == Types.VARCHAR)
						{
							h[key] = rs.getString(i);
						}
						else if (type == Types.INTEGER)
						{
							h[key] = rs.getInt(i);
						}
						else if (type == Types.DOUBLE)
						{
							h[key] = rs.getDouble(i);
						}
						else if (type == Types.BOOLEAN)
						{
							h[key] = rs.getBoolean(i);
						}
						break;
					}
				}
				@params.Add(h);
			}
			return @params;
		}

		public static IList<File> Templates
		{
			get
			{
				File root = null;
				try
				{
					root = new File(typeof(FTLReport).getClassLoader().getResource(FTLReport.TEMPLATE_PATH).toURI());
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					return null;
				}
    
				File[] files = root.listFiles(new FilenameFilterAnonymousInnerClass());
    
				if (files == null)
				{
					return null;
				}
				IList<File> templates = new List<File>(files.Length);
				for (int i = 0; i < files.Length; i++)
				{
					if (files[i].exists() && files[i].isFile() && files[i].canRead())
					{
						templates.Add(files[i]);
					}
				}
				return templates;
			}
		}

		private class FilenameFilterAnonymousInnerClass : FilenameFilter
		{
			public override bool accept(File dir, string name)
			{
				return name.ToLower().EndsWith(".ftlh", StringComparison.Ordinal);
			}
		}
	}

}