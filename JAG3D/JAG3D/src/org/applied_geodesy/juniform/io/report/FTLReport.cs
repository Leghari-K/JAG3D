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

namespace org.applied_geodesy.juniform.io.report
{

	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using FeatureAdjustment = org.applied_geodesy.adjustment.geometry.FeatureAdjustment;
	using FeatureType = org.applied_geodesy.adjustment.geometry.FeatureType;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using VarianceComponent = org.applied_geodesy.adjustment.geometry.VarianceComponent;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using TestStatisticDefinition = org.applied_geodesy.adjustment.statistic.TestStatisticDefinition;
	using TestStatisticParameterSet = org.applied_geodesy.adjustment.statistic.TestStatisticParameterSet;
	using TestStatisticType = org.applied_geodesy.adjustment.statistic.TestStatisticType;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using FormatterOption = org.applied_geodesy.util.FormatterOptions.FormatterOption;
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
	using Matrix = no.uib.cipr.matrix.Matrix;

	public class FTLReport
	{
		private static readonly Version VERSION = Configuration.VERSION_2_3_31;
		private FormatterOptions options = FormatterOptions.Instance;
		private Template template = null;
		private static HostServices hostServices;
		private IDictionary<string, object> data = new Dictionary<string, object>();
		public const string TEMPLATE_PATH = "ftl/juniform/";
		private readonly Configuration cfg = new Configuration(VERSION);
		private FeatureAdjustment adjustment;
		public FTLReport(FeatureAdjustment adjustment)
		{
			this.adjustment = adjustment;
			this.init();
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

		public static HostServices HostServices
		{
			set
			{
				FTLReport.hostServices = value;
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
//ORIGINAL LINE: private void createReport() throws ClassNotFoundException, freemarker.template.TemplateModelException
		private void createReport()
		{
			this.data.Clear();
			BeansWrapper wrapper = (new BeansWrapperBuilder(VERSION)).build();
			TemplateHashModel staticModels = wrapper.getStaticModels();
			TemplateHashModel mathStatics = (TemplateHashModel) staticModels.get("java.lang.Math");
			this.data["Math"] = mathStatics;

			this.initFormatterOptions();
			this.addMetaData();

			this.addGeometricPrimitives();
			this.addTeststatistics();
			this.addVarianceEstimation();

			this.addPoints();
			this.addUnknownFeatureParameters();
			this.addCorrelationMatrix();
		}

		public virtual string SuggestedFileName
		{
			get
			{
				return this.adjustment.Feature != null ? this.adjustment.Feature.FeatureType.name() : null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void toFile(java.io.File report) throws ClassNotFoundException, TemplateException, java.io.IOException
		public virtual void toFile(File report)
		{
			if (report != null)
			{
				this.createReport();
				Writer file = new StreamWriter(new FileStream(report, FileMode.Create, FileAccess.Write), Encoding.UTF8);
				this.template.process(this.data, file);
				file.flush();
				file.close();

				if (hostServices != null)
				{
					hostServices.showDocument(report.getAbsolutePath());
				}
			}
		}

		private void initFormatterOptions()
		{
			IDictionary<CellValueType, FormatterOptions.FormatterOption> options = FormatterOptions.Instance.FormatterOptions;

			foreach (FormatterOptions.FormatterOption option in options.Values)
			{
				string keyDigits = null;
				string keyUnit = null;
				string keySexagesimal = null;
				CellValueType cellValueType = option.Type;
				switch (cellValueType.innerEnumValue)
				{
				case CellValueType.InnerEnum.ANGLE:
					keyDigits = "digits_angle";
					keyUnit = "unit_abbr_angle";
					keySexagesimal = option.Unit.getType() == UnitType.DEGREE_SEXAGESIMAL ? "sexagesimal_angle" : null;
					break;
				case CellValueType.InnerEnum.ANGLE_RESIDUAL:
					keyDigits = "digits_angle_residual";
					keyUnit = "unit_abbr_angle_residual";
					keySexagesimal = option.Unit.getType() == UnitType.DEGREE_SEXAGESIMAL ? "sexagesimal_angle_residual" : null;
					break;
				case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
					keyDigits = "digits_angle_uncertainty";
					keyUnit = "unit_abbr_angle_uncertainty";
					keySexagesimal = option.Unit.getType() == UnitType.DEGREE_SEXAGESIMAL ? "sexagesimal_angle_uncertainty" : null;
					break;

				case CellValueType.InnerEnum.LENGTH:
					keyDigits = "digits_length";
					keyUnit = "unit_abbr_length";
					break;
				case CellValueType.InnerEnum.LENGTH_RESIDUAL:
					keyDigits = "digits_length_residual";
					keyUnit = "unit_abbr_length_residual";
					break;
				case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
					keyDigits = "digits_length_uncertainty";
					keyUnit = "unit_abbr_length_uncertainty";
					break;

				case CellValueType.InnerEnum.SCALE:
					keyDigits = "digits_scale";
					keyUnit = "unit_abbr_scale";
					break;
				case CellValueType.InnerEnum.SCALE_RESIDUAL:
					keyDigits = "digits_scale_residual";
					keyUnit = "unit_abbr_scale_residual";
					break;
				case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
					keyDigits = "digits_scale_uncertainty";
					keyUnit = "unit_abbr_scale_uncertainty";
					break;

				case CellValueType.InnerEnum.VECTOR:
					keyDigits = "digits_vector";
					keyUnit = "unit_abbr_vector";
					break;
				case CellValueType.InnerEnum.VECTOR_RESIDUAL:
					keyDigits = "digits_vector_residual";
					keyUnit = "unit_abbr_vector_residual";
					break;
				case CellValueType.InnerEnum.VECTOR_UNCERTAINTY:
					keyDigits = "digits_vector_uncertainty";
					keyUnit = "unit_abbr_vector_uncertainty";
					break;

				case CellValueType.InnerEnum.PERCENTAGE:
					keyDigits = "digits_percentage";
					keyUnit = "unit_abbr_percentage";
					break;

				case CellValueType.InnerEnum.STATISTIC:
					keyDigits = "digits_statistic";
					break;

				case CellValueType.InnerEnum.DOUBLE:
					keyDigits = "digits_double";
					break;

				default:
					continue;
					// break;
				}
				if (!string.ReferenceEquals(keyDigits, null))
				{
					this.setParam(keyDigits, option.Formatter.format(0.0));
				}
				if (!string.ReferenceEquals(keyUnit, null))
				{
					this.setParam(keyUnit, option.Unit.getAbbreviation());
				}
				if (!string.ReferenceEquals(keySexagesimal, null))
				{
					this.setParam(keySexagesimal, true);
				}
			}
		}

		private void addMetaData()
		{
			this.setParam("report_creation_date", new DateTime(DateTimeHelper.CurrentUnixTimeMillis()));
			this.setParam("version", org.applied_geodesy.version.juniform.Version.get());
			this.setParam("estimation_type", this.adjustment.EstimationType);
		}

		private void addGeometricPrimitives()
		{
			Feature feature = this.adjustment.Feature;
			IList<Dictionary<string, object>> geometries = new List<Dictionary<string, object>>();
			foreach (GeometricPrimitive geometricPrimitive in feature)
			{
				Dictionary<string, object> geometry = new Dictionary<string, object>();
				geometry["name"] = geometricPrimitive.Name;
				geometry["type"] = geometricPrimitive.PrimitiveType.name();
				geometry["unknown_parameters"] = this.UnknownParameters;
				geometries.Add(geometry);
			}
			this.setParam("geometric_primitives", geometries);
		}

		private void addTeststatistics()
		{
			TestStatisticDefinition testStatisticDefinition = this.adjustment.TestStatisticDefinition;
			if (this.adjustment.TestStatisticDefinition == null || this.adjustment.TestStatisticParameters == null)
			{
				return;
			}

			TestStatisticType type = testStatisticDefinition.TestStatisticType;
			double powerOfTest = testStatisticDefinition.PowerOfTest;
			double probabilityValue = testStatisticDefinition.ProbabilityValue;

			this.setParam("test_statistic_method", type.ToString());
			this.setParam("test_statistic_probability_value", options.convertPercentToView(probabilityValue));
			this.setParam("test_statistic_power_of_test", options.convertPercentToView(powerOfTest));

			IList<Dictionary<string, Number>> testStatistics = new List<Dictionary<string, Number>>();
			TestStatisticParameterSet[] testStatisticParameterSets = this.adjustment.TestStatisticParameters.TestStatisticParameterSets;
			foreach (TestStatisticParameterSet testStatisticParameterSet in testStatisticParameterSets)
			{
				if (testStatisticParameterSet.NumeratorDof >= 0 && testStatisticParameterSet.DenominatorDof > 0)
				{
					Dictionary<string, Number> h = new Dictionary<string, Number>();
					h["d1"] = testStatisticParameterSet.NumeratorDof;
					h["d2"] = testStatisticParameterSet.DenominatorDof;
					h["probability_value"] = options.convertPercentToView(testStatisticParameterSet.ProbabilityValue);
					h["power_of_test"] = options.convertPercentToView(testStatisticParameterSet.PowerOfTest);
					h["quantile"] = testStatisticParameterSet.Quantile;
					h["p_value"] = testStatisticParameterSet.LogarithmicProbabilityValue;
					h["non_centrality_parameter"] = testStatisticParameterSet.NoncentralityParameter;
					testStatistics.Add(h);
				}
			}

			if (testStatistics.Count > 0)
			{
				this.setParam("test_statistic_params", testStatistics);
			}
		}

		private void addUnknownFeatureParameters()
		{
			this.setParam("unknown_feature_parameters", this.UnknownParameters);
		}

		private void addCorrelationMatrix()
		{
			Matrix correlationMatrix = this.adjustment.CorrelationMatrix;

			if (correlationMatrix == null)
			{
				return;
			}

			IList<Dictionary<string, object>> matrix = new List<Dictionary<string, object>>();
			IList<UnknownParameter> unknownParameters = this.adjustment.Feature.UnknownParameters;

			int cols = correlationMatrix.numColumns();
			int rows = correlationMatrix.numRows();

			for (int r = 0; r < rows; r++)
			{
				Dictionary<string, object> row = new Dictionary<string, object>(2);
				IList<double> rowVec = new List<double>(rows);
				for (int c = 0; c < cols; c++)
				{
					rowVec.Add(correlationMatrix.get(r, c));
				}

				row["parameter"] = this.getUnknownParameter(unknownParameters[r]);
				row["data"] = rowVec;

				matrix.Add(row);
			}

			this.setParam("correlation_matrix", matrix);
		}

		private IList<Dictionary<string, object>> UnknownParameters
		{
			get
			{
				IList<Dictionary<string, object>> parameters = new List<Dictionary<string, object>>();
				IList<UnknownParameter> unknownParameters = this.adjustment.Feature.UnknownParameters;
				foreach (UnknownParameter unknownParameter in unknownParameters)
				{
					parameters.Add(this.getUnknownParameter(unknownParameter));
				}
				return parameters;
			}
		}

		private Dictionary<string, object> getUnknownParameter(UnknownParameter unknownParameter)
		{
			Dictionary<string, object> parameter = new Dictionary<string, object>();

			ParameterType type = unknownParameter.ParameterType;
			parameter["name"] = unknownParameter.Name;
			parameter["description"] = unknownParameter.Description;
			parameter["processing_type"] = unknownParameter.ProcessingType.name();
			parameter["parameter_type"] = type.ToString();
			parameter["visible"] = unknownParameter.Visible;
			parameter["indispensable"] = unknownParameter.Indispensable;
			parameter["column"] = unknownParameter.Column;

			double value = unknownParameter.Value;
			double sigma = unknownParameter.Uncertainty;

			switch (type)
			{
			case ParameterType.COORDINATE_X:
			case ParameterType.COORDINATE_Y:
			case ParameterType.COORDINATE_Z:
			case ParameterType.LENGTH:
			case ParameterType.RADIUS:
			case ParameterType.ORIGIN_COORDINATE_X:
			case ParameterType.ORIGIN_COORDINATE_Y:
			case ParameterType.ORIGIN_COORDINATE_Z:
			case ParameterType.PRIMARY_FOCAL_COORDINATE_X:
			case ParameterType.PRIMARY_FOCAL_COORDINATE_Y:
			case ParameterType.PRIMARY_FOCAL_COORDINATE_Z:
			case ParameterType.SECONDARY_FOCAL_COORDINATE_X:
			case ParameterType.SECONDARY_FOCAL_COORDINATE_Y:
			case ParameterType.SECONDARY_FOCAL_COORDINATE_Z:
				parameter["value"] = options.convertLengthToView(value);
				parameter["sigma"] = options.convertLengthUncertaintyToView(sigma);
				parameter["unit_type"] = "LENGTH";
				break;
			case ParameterType.VECTOR_LENGTH:
			case ParameterType.VECTOR_X:
			case ParameterType.VECTOR_Y:
			case ParameterType.VECTOR_Z:
				parameter["value"] = options.convertVectorToView(value);
				parameter["sigma"] = options.convertVectorUncertaintyToView(sigma);
				parameter["unit_type"] = "VECTOR";
				break;

			case ParameterType.ANGLE:
				parameter["value"] = options.convertAngleToView(value);
				parameter["sigma"] = options.convertAngleUncertaintyToView(sigma);
				parameter["unit_type"] = "ANGLE";
				break;

			default:
				parameter["value"] = value;
				parameter["sigma"] = sigma;
				parameter["unit_type"] = "DOUBLE";
				break;
			}

			return parameter;
		}

		private void addPoints()
		{
			Dictionary<string, object> points = new Dictionary<string, object>();
			IList<Dictionary<string, object>> pointList = new List<Dictionary<string, object>>();
			IList<FeaturePoint> featurePoints = this.adjustment.Feature.FeaturePoints;

			int dimension = -1;
			double redundancyGroupX = 0;
			double redundancyGroupY = 0;
			double redundancyGroupZ = 0;

			double maxGrossErrorGroupX = 0;
			double maxGrossErrorGroupY = 0;
			double maxGrossErrorGroupZ = 0;

			bool significantGroup = false;

			foreach (FeaturePoint point in featurePoints)
			{
				Dictionary<string, object> h = new Dictionary<string, object>();
				dimension = point.Dimension;
				bool significant = point.Significant;

				if (!significantGroup && significant)
				{
					significantGroup = true;
				}

				h["name"] = point.Name;

				h["t_prio"] = point.TestStatistic.TestStatisticApriori;
				h["t_post"] = point.TestStatistic.TestStatisticAposteriori;

				h["p_prio"] = point.TestStatistic.PValueApriori;
				h["p_post"] = point.TestStatistic.PValueAposteriori;

				h["significant"] = significant;

				h["dimension"] = dimension;


				if (point.Dimension != 1)
				{
					h["x0"] = options.convertLengthToView(point.X0);
					h["y0"] = options.convertLengthToView(point.Y0);

					h["x"] = options.convertLengthToView(point.X);
					h["y"] = options.convertLengthToView(point.Y);

					h["sigma_x"] = options.convertLengthUncertaintyToView(point.UncertaintyX);
					h["sigma_y"] = options.convertLengthUncertaintyToView(point.UncertaintyY);

					h["residual_x"] = options.convertLengthResidualToView(point.ResidualX);
					h["residual_y"] = options.convertLengthResidualToView(point.ResidualY);

					h["minimal_detectable_bias_x"] = options.convertLengthResidualToView(point.MinimalDetectableBiasX);
					h["minimal_detectable_bias_y"] = options.convertLengthResidualToView(point.MinimalDetectableBiasY);

					h["maximum_tolerable_bias_x"] = options.convertLengthResidualToView(point.MaximumTolerableBiasX);
					h["maximum_tolerable_bias_y"] = options.convertLengthResidualToView(point.MaximumTolerableBiasY);

					double grossErrorX = options.convertLengthResidualToView(point.GrossErrorX);
					double grossErrorY = options.convertLengthResidualToView(point.GrossErrorY);

					double redundancyX = point.RedundancyX;
					double redundancyY = point.RedundancyY;

					h["gross_error_x"] = grossErrorX;
					h["gross_error_y"] = grossErrorY;

					h["redundancy_x"] = options.convertPercentToView(redundancyX);
					h["redundancy_y"] = options.convertPercentToView(redundancyY);

					redundancyGroupX += redundancyX;
					redundancyGroupY += redundancyY;

					maxGrossErrorGroupX = Math.Abs(grossErrorX) > Math.Abs(maxGrossErrorGroupX) ? grossErrorX : maxGrossErrorGroupX;
					maxGrossErrorGroupY = Math.Abs(grossErrorY) > Math.Abs(maxGrossErrorGroupY) ? grossErrorY : maxGrossErrorGroupY;


				}
				if (point.Dimension != 2)
				{
					h["z0"] = options.convertLengthToView(point.Z0);

					h["z"] = options.convertLengthToView(point.Z);

					h["sigma_z"] = options.convertLengthUncertaintyToView(point.UncertaintyZ);

					h["residual_z"] = options.convertLengthResidualToView(point.ResidualZ);

					h["minimal_detectable_bias_z"] = options.convertLengthResidualToView(point.MinimalDetectableBiasZ);

					h["maximum_tolerable_bias_z"] = options.convertLengthResidualToView(point.MaximumTolerableBiasZ);

					double grossErrorZ = options.convertLengthResidualToView(point.GrossErrorZ);

					double redundancyZ = point.RedundancyZ;

					h["gross_error_z"] = grossErrorZ;

					h["redundancy_z"] = options.convertPercentToView(redundancyZ);

					redundancyGroupZ += redundancyZ;
					maxGrossErrorGroupZ = Math.Abs(grossErrorZ) > Math.Abs(maxGrossErrorGroupZ) ? grossErrorZ : maxGrossErrorGroupZ;
				}

				pointList.Add(h);
			}

			if (pointList != null && pointList.Count > 0)
			{
				points["points"] = pointList;
				points["significant"] = significantGroup;
				points["dimension"] = dimension;

				points["redundancy_x"] = redundancyGroupX;
				points["redundancy_y"] = redundancyGroupY;
				points["redundancy_z"] = redundancyGroupZ;
				points["redundancy"] = redundancyGroupX + redundancyGroupY + redundancyGroupZ;

				points["max_gross_error_x"] = maxGrossErrorGroupX;
				points["max_gross_error_y"] = maxGrossErrorGroupY;
				points["max_gross_error_z"] = maxGrossErrorGroupZ;

				this.setParam("feature_points", points);
			}
		}

		private void addVarianceEstimation()
		{
			if (this.adjustment.TestStatisticParameters == null)
			{
				return;
			}

			IList<IDictionary<string, object>> vces = new List<IDictionary<string, object>>();
			IDictionary<string, object> vce = new Dictionary<string, object>(6);
			VarianceComponent varianceComponentOfUnitWeight = this.adjustment.VarianceComponentOfUnitWeight;
			int dof = (int)varianceComponentOfUnitWeight.Redundancy;
			int numberOfPoints = this.adjustment.Feature.FeaturePoints.Count;
			int dim = this.adjustment.Feature.FeatureType == FeatureType.CURVE ? 2 : 3;
			double sigma2apost = varianceComponentOfUnitWeight.Variance / varianceComponentOfUnitWeight.Variance0;
			double omega = varianceComponentOfUnitWeight.Omega / varianceComponentOfUnitWeight.Variance0;
			bool significant = varianceComponentOfUnitWeight.Significant;
			double quantile = this.adjustment.TestStatisticParameters.getTestStatisticParameter(dof > 0.000001 ? dof : 0, double.PositiveInfinity).Quantile;

			vce["type"] = "GLOBAL";
			vce["omega"] = omega;
			vce["number_of_observations"] = dim * numberOfPoints;
			vce["redundancy"] = dof;
			vce["sigma2apost"] = sigma2apost;
			vce["quantile"] = quantile;
			vce["significant"] = significant;

			vces.Add(vce);

			if (vces.Count > 0)
			{
				this.setParam("vce", vces);
			}
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