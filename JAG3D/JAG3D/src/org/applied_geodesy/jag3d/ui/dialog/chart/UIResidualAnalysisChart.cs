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

namespace org.applied_geodesy.jag3d.ui.dialog.chart
{

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using AreaChart = javafx.scene.chart.AreaChart;
	using NumberAxis = javafx.scene.chart.NumberAxis;
	using XYChart = javafx.scene.chart.XYChart;
	using CheckBox = javafx.scene.control.CheckBox;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using Tooltip = javafx.scene.control.Tooltip;
	using BorderPane = javafx.scene.layout.BorderPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using StringConverter = javafx.util.StringConverter;

	public class UIResidualAnalysisChart
	{
		private sealed class TerrestrialObservationType
		{
			public static readonly TerrestrialObservationType ALL = new TerrestrialObservationType("ALL", InnerEnum.ALL, null);
			public static readonly TerrestrialObservationType LEVELING = new TerrestrialObservationType("LEVELING", InnerEnum.LEVELING, org.applied_geodesy.adjustment.network.ObservationType.LEVELING);
			public static readonly TerrestrialObservationType DIRECTION = new TerrestrialObservationType("DIRECTION", InnerEnum.DIRECTION, org.applied_geodesy.adjustment.network.ObservationType.DIRECTION);
			public static readonly TerrestrialObservationType HORIZONTAL_DISTANCE = new TerrestrialObservationType("HORIZONTAL_DISTANCE", InnerEnum.HORIZONTAL_DISTANCE, org.applied_geodesy.adjustment.network.ObservationType.HORIZONTAL_DISTANCE);
			public static readonly TerrestrialObservationType SLOPE_DISTANCE = new TerrestrialObservationType("SLOPE_DISTANCE", InnerEnum.SLOPE_DISTANCE, org.applied_geodesy.adjustment.network.ObservationType.SLOPE_DISTANCE);
			public static readonly TerrestrialObservationType ZENITH_ANGLE = new TerrestrialObservationType("ZENITH_ANGLE", InnerEnum.ZENITH_ANGLE, org.applied_geodesy.adjustment.network.ObservationType.ZENITH_ANGLE);

			private static readonly List<TerrestrialObservationType> valueList = new List<TerrestrialObservationType>();

			static TerrestrialObservationType()
			{
				valueList.Add(ALL);
				valueList.Add(LEVELING);
				valueList.Add(DIRECTION);
				valueList.Add(HORIZONTAL_DISTANCE);
				valueList.Add(SLOPE_DISTANCE);
				valueList.Add(ZENITH_ANGLE);
			}

			public enum InnerEnum
			{
				ALL,
				LEVELING,
				DIRECTION,
				HORIZONTAL_DISTANCE,
				SLOPE_DISTANCE,
				ZENITH_ANGLE
			}

			public readonly InnerEnum innerEnumValue;
			private readonly string nameValue;
			private readonly int ordinalValue;
			private static int nextOrdinal = 0;

			internal org.applied_geodesy.adjustment.network.ObservationType observationType;
			internal TerrestrialObservationType(string name, InnerEnum innerEnum, org.applied_geodesy.adjustment.network.ObservationType observationType)
			{
				this.observationType = observationType;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			public org.applied_geodesy.adjustment.network.ObservationType ObservationType
			{
				get
				{
					return observationType;
				}
			}

			public static TerrestrialObservationType getEnumByValue(org.applied_geodesy.adjustment.network.ObservationType observationType)
			{
				foreach (TerrestrialObservationType element in TerrestrialObservationType.values())
				{
					if (element.observationType != null && element.observationType == observationType)
					{
						return element;
					}
				}
				return null;
			}

			public static TerrestrialObservationType[] values()
			{
				return valueList.ToArray();
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static TerrestrialObservationType valueOf(string name)
			{
				foreach (TerrestrialObservationType enumInstance in TerrestrialObservationType.valueList)
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		private class TickFormatChangedListener : FormatterChangedListener
		{
			private readonly UIResidualAnalysisChart outerInstance;

			public TickFormatChangedListener(UIResidualAnalysisChart outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void formatterChanged(FormatterEvent evt)
			{
				if (evt.CellType == CellValueType.STATISTIC)
				{
					outerInstance.updateTickLabels(outerInstance.histogramChart);
				}
			}
		}

		private class TerrestrialObservationTypeChangeListener : ChangeListener<TerrestrialObservationType>
		{
			private readonly UIResidualAnalysisChart outerInstance;

			public TerrestrialObservationTypeChangeListener(UIResidualAnalysisChart outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, TerrestrialObservationType oldValue, TerrestrialObservationType newValue) where T1 : TerrestrialObservationType
			{
				if (newValue != null)
				{
					outerInstance.updateChartData(newValue);
				}
			}
		}

		private class ProbabilityDensityFunctionChangeListener : ChangeListener<bool>
		{
			private readonly UIResidualAnalysisChart outerInstance;

			public ProbabilityDensityFunctionChangeListener(UIResidualAnalysisChart outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				TerrestrialObservationType type = outerInstance.terrestrialObservationTypeComboBox.getValue();
				outerInstance.updateChartData(type);
			}
		}

		private I18N i18n = I18N.Instance;
		private FormatterOptions options = FormatterOptions.Instance;
		private static UIResidualAnalysisChart analysisChart = new UIResidualAnalysisChart();
		private Node analysisChartNode;

		private ComboBox<TerrestrialObservationType> terrestrialObservationTypeComboBox;
		private CheckBox gaussianProbabilityDensityFunctionCheckBox, kernelDensityEstimationCheckBox;
		private AreaChart<Number, Number> histogramChart;

		private UIResidualAnalysisChart()
		{
		}

		public static UIResidualAnalysisChart Instance
		{
			get
			{
				analysisChart.init();
				return analysisChart;
			}
		}

		public virtual Node Node
		{
			get
			{
				return this.analysisChartNode;
			}
		}

		private void init()
		{
			if (this.analysisChartNode != null)
			{
				return;
			}

			BorderPane borderPane = new BorderPane();

			this.gaussianProbabilityDensityFunctionCheckBox = this.createCheckBox(i18n.getString("UIResidualAnalysisChart.histogram.probability_density_function.label", "Gaussian probability density function"), i18n.getString("UIResidualAnalysisChart.histogram.probability_density_function.tooltip", "If selected, the probability density function of the standard normal distribution is estimated"));
			this.kernelDensityEstimationCheckBox = this.createCheckBox(i18n.getString("UIResidualAnalysisChart.histogram.kernel_density_estimation.label", "Kernel density estimation"), i18n.getString("UIResidualAnalysisChart.histogram.kernel_density_estimation.tooltip", "If selected, the probability density function is estimated by a kernel density estimation"));
			this.gaussianProbabilityDensityFunctionCheckBox.setSelected(true);
			this.kernelDensityEstimationCheckBox.setSelected(true);
			HBox cbNode = new HBox(10);
			Region regionLeft = new Region();
			Region regionRight = new Region();
			HBox.setHgrow(regionLeft, Priority.ALWAYS);
			HBox.setHgrow(regionRight, Priority.ALWAYS);
			cbNode.setPadding(new Insets(5, 10, 5, 10));
			cbNode.getChildren().addAll(regionLeft, this.gaussianProbabilityDensityFunctionCheckBox, this.kernelDensityEstimationCheckBox, regionRight);
			ProbabilityDensityFunctionChangeListener probabilityDensityFunctionChangeListener = new ProbabilityDensityFunctionChangeListener(this);
			this.gaussianProbabilityDensityFunctionCheckBox.selectedProperty().addListener(probabilityDensityFunctionChangeListener);
			this.kernelDensityEstimationCheckBox.selectedProperty().addListener(probabilityDensityFunctionChangeListener);

			this.terrestrialObservationTypeComboBox = this.createTerrestrialObservationTypeComboBox(TerrestrialObservationType.ALL, i18n.getString("UIResidualAnalysisChart.observationtype.terrestrial.tooltip", "Set observational residual type"));
			Region spacer = new Region();
			HBox hbox = new HBox(10);
			hbox.setPadding(new Insets(5, 10, 5, 15));
			HBox.setHgrow(spacer, Priority.ALWAYS);
			hbox.getChildren().addAll(spacer, this.terrestrialObservationTypeComboBox);

			this.histogramChart = this.createHistogramChart();

			borderPane.setTop(hbox);
			borderPane.setCenter(this.histogramChart);
			borderPane.setBottom(cbNode);
			borderPane.setPrefWidth(500);

			this.terrestrialObservationTypeComboBox.getSelectionModel().selectedItemProperty().addListener(new TerrestrialObservationTypeChangeListener(this));
			this.options.addFormatterChangedListener(new TickFormatChangedListener(this));

			this.analysisChartNode = borderPane;
		}

		private AreaChart<Number, Number> createHistogramChart()
		{
			NumberAxis xAxis = new NumberAxis(-5, 5, 1);
			NumberAxis yAxis = new NumberAxis(0, 0.45, 0.1);

			xAxis.setLabel(i18n.getString("UIResidualAnalysisChart.chart.histogram.axis.x.label", "Normalized residuals"));
			xAxis.setForceZeroInRange(true);
			xAxis.setMinorTickVisible(false);
			xAxis.setAnimated(false);
			xAxis.setAutoRanging(false);

			yAxis.setLabel(i18n.getString("UIResidualAnalysisChart.chart.histogram.axis.y.label", "Probability density"));
			yAxis.setForceZeroInRange(true);
			yAxis.setMinorTickVisible(false);
			yAxis.setAnimated(false);
			yAxis.setAutoRanging(false);

			AreaChart<Number, Number> areaChart = new AreaChart<Number, Number>(xAxis, yAxis);
			areaChart.setLegendVisible(false);
			areaChart.setAnimated(false);
			areaChart.setCreateSymbols(false);
			areaChart.setVerticalZeroLineVisible(false);
			areaChart.setPadding(new Insets(0, 0, 0, 0));

			this.updateTickLabels(areaChart);

			return areaChart;
		}

		private void updateTickLabels(AreaChart<Number, Number> areaChart)
		{
			NumberAxis yAxis = (NumberAxis)areaChart.getYAxis();
			yAxis.setLabel(i18n.getString("UIResidualAnalysisChart.chart.histogram.axis.y.label", "Probability density"));

			yAxis.setTickLabelFormatter(new StringConverterAnonymousInnerClass(this));
		}

		private class StringConverterAnonymousInnerClass : StringConverter<Number>
		{
			private readonly UIResidualAnalysisChart outerInstance;

			public StringConverterAnonymousInnerClass(UIResidualAnalysisChart outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override string toString(Number number)
			{
				return outerInstance.options.toStatisticFormat(number.doubleValue());
			}

			public override Number fromString(string @string)
			{
				return null;
			}
		}

		private ComboBox<TerrestrialObservationType> createTerrestrialObservationTypeComboBox(TerrestrialObservationType item, string tooltip)
		{
			ComboBox<TerrestrialObservationType> typeComboBox = new ComboBox<TerrestrialObservationType>();
			TerrestrialObservationType[] terrestrialObservationTypeArray = TerrestrialObservationType.values();

			typeComboBox.getItems().setAll(terrestrialObservationTypeArray);
			typeComboBox.getSelectionModel().select(item);
			typeComboBox.setConverter(new StringConverterAnonymousInnerClass2(this));
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMinWidth(150);
			typeComboBox.setMaxWidth(double.MaxValue);

			return typeComboBox;
		}

		private class StringConverterAnonymousInnerClass2 : StringConverter<TerrestrialObservationType>
		{
			private readonly UIResidualAnalysisChart outerInstance;

			public StringConverterAnonymousInnerClass2(UIResidualAnalysisChart outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override string toString(TerrestrialObservationType type)
			{
				if (type == null)
				{
					return null;
				}
				switch (type.innerEnumValue)
				{
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIResidualAnalysisChart.TerrestrialObservationType.InnerEnum.ALL:
					return outerInstance.i18n.getString("UIResidualAnalysisChart.observationtype.terrestrial.all.label", "All terrestrial observations");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIResidualAnalysisChart.TerrestrialObservationType.InnerEnum.LEVELING:
					return outerInstance.i18n.getString("UIResidualAnalysisChart.observationtype.terrestrial.leveling.label", "Leveling");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIResidualAnalysisChart.TerrestrialObservationType.InnerEnum.DIRECTION:
					return outerInstance.i18n.getString("UIResidualAnalysisChart.observationtype.terrestrial.direction.label", "Directions");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIResidualAnalysisChart.TerrestrialObservationType.InnerEnum.HORIZONTAL_DISTANCE:
					return outerInstance.i18n.getString("UIResidualAnalysisChart.observationtype.terrestrial.horizontal_distance.label", "Horizontal distances");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIResidualAnalysisChart.TerrestrialObservationType.InnerEnum.SLOPE_DISTANCE:
					return outerInstance.i18n.getString("UIResidualAnalysisChart.observationtype.terrestrial.slope_distance.label", "Slope distances");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIResidualAnalysisChart.TerrestrialObservationType.InnerEnum.ZENITH_ANGLE:
					return outerInstance.i18n.getString("UIResidualAnalysisChart.observationtype.terrestrial.zenith_angle.label", "Zenith angles");
				}
				return null;
			}

			public override TerrestrialObservationType fromString(string @string)
			{
				return TerrestrialObservationType.valueOf(@string);
			}
		}

		private CheckBox createCheckBox(string title, string tooltip)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			CheckBox checkBox = new CheckBox();
			checkBox.setGraphic(label);
			checkBox.setTooltip(new Tooltip(tooltip));
			checkBox.setMinHeight(Control.USE_PREF_SIZE);
			checkBox.setMaxHeight(double.MaxValue);
			return checkBox;
		}

		private void updateChartData(TerrestrialObservationType terrestrialObservationType)
		{
			try
			{
				terrestrialObservationType = terrestrialObservationType == null ? TerrestrialObservationType.ALL : terrestrialObservationType;
				ObservationType[] observationTypes = this.getSelectedObservationTypes(terrestrialObservationType);
				IList<double> normalizedResiduals = SQLManager.Instance.getNormalizedResiduals(observationTypes);

				this.histogramChart.getData().clear();

				if (normalizedResiduals == null || normalizedResiduals.Count == 0)
				{
					return;
				}

				// Sort data
				normalizedResiduals.Sort();

				int length = normalizedResiduals.Count;
				double sampleMean = length > 1 ? this.getMean(normalizedResiduals) : 0;
				double sampleVariance = length > 1 ? this.getVariance(normalizedResiduals, sampleMean) : 1.0;

				double xRange = 0, yRange = 0;

				double[] range = this.plotHistogram(this.histogramChart, normalizedResiduals, sampleMean, sampleVariance);
				xRange = range[0];
				yRange = range[1];

				if (this.gaussianProbabilityDensityFunctionCheckBox.isSelected())
				{
					range = this.plotGaussianProbabilityDensityFunction(this.histogramChart, normalizedResiduals, sampleMean, sampleVariance);
					xRange = Math.Max(xRange, range[0]);
					yRange = Math.Max(yRange, range[1]);
				}

				if (this.kernelDensityEstimationCheckBox.isSelected())
				{
					range = this.plotKernelDensityEstimation(this.histogramChart, normalizedResiduals, sampleMean, sampleVariance);
					xRange = Math.Max(xRange, range[0]);
					yRange = Math.Max(yRange, range[1]);
				}

				xRange = Math.Ceiling(xRange + 0.05);
				yRange = Math.Ceiling((yRange + 0.005) * 10) / 10;

				NumberAxis xAxis = (NumberAxis)this.histogramChart.getXAxis();
				NumberAxis yAxis = (NumberAxis)this.histogramChart.getYAxis();

				xAxis.setLowerBound(-xRange);
				xAxis.setUpperBound(+xRange);

				yAxis.setLowerBound(0);
				yAxis.setUpperBound(yRange);

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("UIResidualAnalysisChart.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("UIResidualAnalysisChart.message.error.load.exception.header", "Error, could not load data from database."), i18n.getString("UIResidualAnalysisChart.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private double[] plotHistogram(AreaChart<Number, Number> areaChart, IList<double> data, double sampleMean, double sampleVariance)
		{
			int length = data.Count;
			double binWidth = 0;
			double iqr = this.getInterQuartileRange(data);
			double minValue = data[0];
			double maxValue = data[length - 1];
			int numberOfBins = 0;

			if (iqr > 0 || sampleVariance > 0)
			{
				binWidth = iqr > 0 ? 2.0 * iqr * Math.Pow(length, -1.0 / 3.0) : 3.49 * Math.Sqrt(sampleVariance) * Math.Pow(length, -1.0 / 3.0);
			}
				else
				{
					binWidth = (maxValue - minValue) / (long)Math.Round(2.0 * Math.Pow(length, 1.0 / 3.0), MidpointRounding.AwayFromZero);
				}

			numberOfBins = binWidth > 0 ? Math.Max((int)Math.Ceiling((maxValue - minValue) / binWidth), 1) : 1;
			int[] bins = new int[numberOfBins];

			for (int i = 0, j = 0; i < length; i++)
			{
				if (data[i] <= (minValue + (j + 1) * binWidth) && j < numberOfBins)
				{
					bins[j]++;
				}
				else
				{
					do
					{
						j++;
					} while (!(data[i] <= (minValue + (j + 1) * binWidth) && j < numberOfBins));

					if (j < numberOfBins)
					{
						bins[j]++;
					}
				}
			}

			double yRange = 0.0;

			for (int i = 0; i < bins.Length; i++)
			{
				if (bins[i] == 0)
				{
					continue;
				}

				double width = minValue + i * binWidth;
				double pdf = (double)bins[i] / (double)length / binWidth;
				XYChart.Series<Number, Number> bar = new XYChart.Series<Number, Number>();
				bar.getData().add(new XYChart.Data<Number, Number>(width, 0));
				bar.getData().add(new XYChart.Data<Number, Number>(width, pdf));
				bar.getData().add(new XYChart.Data<Number, Number>(width + binWidth, pdf));
				bar.getData().add(new XYChart.Data<Number, Number>(width + binWidth, 0));

				Platform.runLater(() =>
				{
				histogramChart.getData().add(bar);
				Node line = bar.getNode().lookup(".chart-series-area-line");
				if (line != null)
				{
					line.setStyle("-fx-stroke: rgba(75, 75, 75, 1); -fx-stroke-width: 1.0px;");
				}
				Node area = bar.getNode().lookup(".chart-series-area-fill");
				if (area != null)
				{
					area.setStyle("-fx-fill: rgba(75, 75, 75, 0.25);");
				}
				});
				yRange = Math.Max(yRange, pdf);
			}

			double xRange = Math.Max(Math.Abs(minValue), Math.Abs(maxValue + binWidth));

			return new double[] {xRange, yRange};
		}

		private double[] plotGaussianProbabilityDensityFunction(AreaChart<Number, Number> areaChart, IList<double> data, double sampleMean, double sampleVariance)
		{

			double xRange = 0, yRange = 0;
			XYChart.Series<Number, Number> probabilityDensity = new XYChart.Series<Number, Number>();
			for (double x = -3.9; x <= 3.9; x += 0.01)
			{
				//double pdf = this.getStandardGaussian(x, sampleMean, sampleVariance);
				double pdf = this.getStandardGaussian(x);
				probabilityDensity.getData().add(new XYChart.Data<Number, Number>(x, pdf));
				xRange = Math.Max(xRange, Math.Abs(x));
				yRange = Math.Max(yRange, Math.Abs(pdf));
			}

			Platform.runLater(() =>
			{
			areaChart.getData().add(probabilityDensity);

			Node line = probabilityDensity.getNode().lookup(".chart-series-area-line");
			if (line != null)
			{
				line.setStyle("-fx-stroke: rgba(200, 0, 0, 1); -fx-stroke-width: 2.5px;");
			}
			Node area = probabilityDensity.getNode().lookup(".chart-series-area-fill");
			if (area != null)
			{
				area.setStyle("-fx-fill: rgba(200, 0, 0, 0);");
			}
			});

			return new double[] {xRange, yRange};
		}

		private double[] plotKernelDensityEstimation(AreaChart<Number, Number> areaChart, IList<double> data, double sampleMean, double sampleVariance)
		{
			double iqr = this.getInterQuartileRange(data);
			int length = data.Count;
			//double mean = length > 1 ? this.getMean(data) : 0;
			//double std  = length > 1 ? Math.sqrt(this.getVariance(data, mean)) : 1.0;
			double std = sampleVariance > 0 ? Math.Sqrt(sampleVariance) : 1.0;
			double minValue = data[0];
			double maxValue = data[length - 1];
			double xRange = 0, yRange = 0;
			double bandWidth = 0;

			if (iqr > 0 && std > 0)
			{
				bandWidth = 0.9 * Math.Min(std, iqr / 1.34897950039216) * Math.Pow(length, -1.0 / 5.0); // 1.34... == norminv(0.75)*2
			}
			else
			{
				bandWidth = length > 0 ? Math.Pow(4.0 * Math.Pow(std, 5) / 3.0 / length, 1 / 5) : 1;
			}

				XYChart.Series<Number, Number> kernelEstimation = new XYChart.Series<Number, Number>();

				double t = sampleMean;
				double yt = double.MaxValue;
				double inc = 0.01;
				int cnt = 0;
				do
				{
					yt = 0;
					t = sampleMean - (cnt++) * inc;
					for (int j = 0; j < length; j++)
					{
						double x = (t - (data[j])) / bandWidth;
						double pdf = this.getStandardGaussian(x);
						yt += pdf;
					}
					yt = 1.0 / length / bandWidth * yt;
					kernelEstimation.getData().add(new XYChart.Data<Number, Number>(t, yt));
					xRange = Math.Max(xRange, Math.Abs(t));
					yRange = Math.Max(yRange, Math.Abs(yt));
				} while (Math.Abs(yt) > 0.0005 || t > minValue);

				t = sampleMean;
				yt = double.MaxValue;
				cnt = 0;
				do
				{
					yt = 0;
					t = sampleMean + (++cnt) * inc;
					for (int j = 0; j < length; j++)
					{
						double x = (t - (data[j])) / bandWidth;
						double pdf = this.getStandardGaussian(x);
						yt += pdf;
					}
					yt = 1.0 / length / bandWidth * yt;
					kernelEstimation.getData().add(new XYChart.Data<Number, Number>(t, yt));
					xRange = Math.Max(xRange, Math.Abs(t));
					yRange = Math.Max(yRange, Math.Abs(yt));
				} while (Math.Abs(yt) > 0.0005 || t < maxValue);

				Platform.runLater(() =>
				{
				areaChart.getData().add(kernelEstimation);
				if (kernelEstimation.getNode() != null)
				{
					Node line = kernelEstimation.getNode().lookup(".chart-series-area-line");
					line.setStyle("-fx-stroke: rgba(0, 0, 150, 1); -fx-stroke-width: 2.5px; -fx-stroke-dash-array: 10 7 10 7;");
					Node area = kernelEstimation.getNode().lookup(".chart-series-area-fill");
					area.setStyle("-fx-fill: rgba(0, 0, 0, 0);");
				}
				});
				return new double[] {xRange, yRange};
		}

		private ObservationType[] getSelectedObservationTypes(TerrestrialObservationType terrestrialObservationType)
		{
			ObservationType[] observationTypes;
			if (terrestrialObservationType == TerrestrialObservationType.ALL)
			{
				observationTypes = new ObservationType[] {ObservationType.LEVELING, ObservationType.DIRECTION, ObservationType.HORIZONTAL_DISTANCE, ObservationType.SLOPE_DISTANCE, ObservationType.ZENITH_ANGLE};
			}
			else
			{
				observationTypes = new ObservationType[] {terrestrialObservationType.getObservationType()};
			}
			return observationTypes;
		}


		private double getInterQuartileRange(IList<double> values)
		{
			double n = values.Count;
			double q1 = values[(int)Math.Floor(0.25 * n)];
			double q3 = values[(int)Math.Floor(0.75 * n)];
			return q3 - q1;
		}

		private double getStandardGaussian(double x)
		{
			return this.getStandardGaussian(x, 0, 1);
		}

		private double getStandardGaussian(double x, double mu, double var)
		{
			if (var <= 0)
			{
				return 0;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fac = 1.0/Math.sqrt(2.0*Math.PI*var);
			double fac = 1.0 / Math.Sqrt(2.0 * Math.PI * var);

			double dx = x - mu;
			return fac * Math.Exp(-0.5 * dx * dx / var);
		}

		private double getVariance(IList<double> values, double mean)
		{
			double var = 0;
			double n = values.Count;

			foreach (double? value in values)
			{
				double res = value.Value - mean;
				var += res * res;
			}
			return var / n;
		}

		private double getMean(IList<double> values)
		{
			double mean = 0;
			double n = values.Count;
			foreach (double? value in values)
			{
				mean += value.Value;
			}
			return mean = n > 0 ? mean / n : 0;
		}

		public virtual void load()
		{
			try
			{

				IList<ObservationType> projectObservationTypes = SQLManager.Instance.ProjectObservationTypes;
				TerrestrialObservationType[] terrestrialObservationTypeArray = new TerrestrialObservationType[projectObservationTypes.Count == 1 ? 1 : projectObservationTypes.Count + 1];
				terrestrialObservationTypeArray[0] = TerrestrialObservationType.ALL;

				TerrestrialObservationType lastSelectedTerrestrialObservationType = this.terrestrialObservationTypeComboBox.getSelectionModel().getSelectedItem();
				bool containsLastSelectedTerrestrialObservationType = false;
				if (projectObservationTypes.Count > 1)
				{
					int idx = 1;
					foreach (ObservationType obsType in projectObservationTypes)
					{
						TerrestrialObservationType type = TerrestrialObservationType.getEnumByValue(obsType);
						if (type != null)
						{
							terrestrialObservationTypeArray[idx++] = type;
							if (lastSelectedTerrestrialObservationType == type)
							{
								containsLastSelectedTerrestrialObservationType = true;
							}
						}
					}
				}

				this.terrestrialObservationTypeComboBox.getSelectionModel().clearSelection();
				this.terrestrialObservationTypeComboBox.getItems().setAll(terrestrialObservationTypeArray);
				this.terrestrialObservationTypeComboBox.getSelectionModel().select(containsLastSelectedTerrestrialObservationType ? lastSelectedTerrestrialObservationType : TerrestrialObservationType.ALL);

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("UIResidualAnalysisChart.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("UIResidualAnalysisChart.message.error.load.exception.header", "Error, could not load data from database."), i18n.getString("UIResidualAnalysisChart.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}
	}

}