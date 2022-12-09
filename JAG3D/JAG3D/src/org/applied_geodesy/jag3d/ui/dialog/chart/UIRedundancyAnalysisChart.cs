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
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightRangeType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using FXCollections = javafx.collections.FXCollections;
	using Insets = javafx.geometry.Insets;
	using Side = javafx.geometry.Side;
	using Node = javafx.scene.Node;
	using PieChart = javafx.scene.chart.PieChart;
	using ComboBox = javafx.scene.control.ComboBox;
	using Label = javafx.scene.control.Label;
	using Tooltip = javafx.scene.control.Tooltip;
	using BorderPane = javafx.scene.layout.BorderPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using Color = javafx.scene.paint.Color;
	using Text = javafx.scene.text.Text;
	using StringConverter = javafx.util.StringConverter;

	public class UIRedundancyAnalysisChart
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

		private class TerrestrialObservationTypeChangeListener : ChangeListener<TerrestrialObservationType>
		{
			private readonly UIRedundancyAnalysisChart outerInstance;

			public TerrestrialObservationTypeChangeListener(UIRedundancyAnalysisChart outerInstance)
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

		private I18N i18n = I18N.Instance;
		private TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;

		private FormatterOptions options = FormatterOptions.Instance;
		private static UIRedundancyAnalysisChart analysisChart = new UIRedundancyAnalysisChart();
		private Node analysisChartNode;
		private IDictionary<TableRowHighlightRangeType, int> chartData = new Dictionary<TableRowHighlightRangeType, int>(0);
		private readonly string INADEQUATE = "INADEQUATE"; // TableRowHighlightRangeType.INADEQUATE.name()
		private readonly string SATISFACTORY = "SATISFACTORY";
		private readonly string EXCELLENT = "EXCELLENT";

		private ComboBox<TerrestrialObservationType> terrestrialObservationTypeComboBox;
		private PieChart pieChart;

		private UIRedundancyAnalysisChart()
		{
		}

		public static UIRedundancyAnalysisChart Instance
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

			this.terrestrialObservationTypeComboBox = this.createTerrestrialObservationTypeComboBox(TerrestrialObservationType.ALL, i18n.getString("UIRedundancyAnalysisChart.observationtype.terrestrial.tooltip", "Set observational redundancy type"));
			Region spacer = new Region();
			HBox hbox = new HBox(10);
			hbox.setPadding(new Insets(5, 10, 5, 15));
			HBox.setHgrow(spacer, Priority.ALWAYS);
			hbox.getChildren().addAll(spacer, this.terrestrialObservationTypeComboBox);

			this.pieChart = this.createPieChart();

			borderPane.setTop(hbox);
			borderPane.setCenter(this.pieChart);
			borderPane.setPrefWidth(500);

			this.terrestrialObservationTypeComboBox.getSelectionModel().selectedItemProperty().addListener(new TerrestrialObservationTypeChangeListener(this));

			this.analysisChartNode = borderPane;
		}

		private PieChart createPieChart()
		{
			PieChart chart = new PieChartAnonymousInnerClass(this);
			chart.setLegendSide(Side.BOTTOM);
			chart.setLabelsVisible(true);
			//chart.setAnimated(Boolean.FALSE);
			return chart;
		}

		private class PieChartAnonymousInnerClass : PieChart
		{
			private readonly UIRedundancyAnalysisChart outerInstance;

			public PieChartAnonymousInnerClass(UIRedundancyAnalysisChart outerInstance)
			{
				this.outerInstance = outerInstance;
			}

					// https://stackoverflow.com/questions/35479375/display-additional-values-in-pie-chart
			protected internal override void layoutChartChildren(double top, double left, double contentWidth, double contentHeight)
			{
				if (getLabelsVisible())
				{
					ISet<Node> items = this.lookupAll(".chart-pie-label");
					foreach (Node item in items)
					{
						if (item is Text && ((Text)item).getText() != null)
						{
							Text text = (Text)item;
							outerInstance.ChartLabelText = text;
						}
					}
				}
				base.layoutChartChildren(top, left, contentWidth, contentHeight);
			}
		}

		private Text ChartLabelText
		{
			set
			{
				string name = value.getText();
				if (name.Equals(INADEQUATE) || name.Equals(SATISFACTORY) || name.Equals(EXCELLENT))
				{
					TableRowHighlightRangeType tableRowHighlightRangeType = TableRowHighlightRangeType.valueOf(name);
					value.setText(String.format(Locale.ENGLISH, "%d", this.chartData[tableRowHighlightRangeType]));
				}
			}
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

		private void updateChartData(TerrestrialObservationType terrestrialObservationType)
		{
			try
			{
				terrestrialObservationType = terrestrialObservationType == null ? TerrestrialObservationType.ALL : terrestrialObservationType;
				ObservationType[] observationTypes = this.getSelectedObservationTypes(terrestrialObservationType);

				double leftBoundary = this.tableRowHighlight.getLeftBoundary(TableRowHighlightType.REDUNDANCY);
				double rightBoundary = this.tableRowHighlight.getRightBoundary(TableRowHighlightType.REDUNDANCY);

				this.chartData = SQLManager.Instance.getChartData(TableRowHighlightType.REDUNDANCY, observationTypes);
				this.pieChart.getData().clear();

				IList<PieChart.Data> dataList = FXCollections.observableArrayList();
				foreach (KeyValuePair<TableRowHighlightRangeType, int> entry in this.chartData.SetOfKeyValuePairs())
				{
					if (entry.Value > 0)
					{
						dataList.Add(new PieChart.Data(entry.Key.name(), entry.Value));
					}
				}

				this.pieChart.getData().setAll(dataList);

				// set data color
				foreach (PieChart.Data data in this.pieChart.getData())
				{
					try
					{
						string name = data.getName();
						switch (name)
						{
						case INADEQUATE:
						case SATISFACTORY:
						case EXCELLENT:
							TableRowHighlightRangeType tableRowHighlightRangeType = TableRowHighlightRangeType.valueOf(name);

							Color color = this.tableRowHighlight.getColor(tableRowHighlightRangeType);
							string rgbColor = String.format(Locale.ENGLISH, "rgb(%.0f, %.0f, %.0f)", color.getRed() * 255, color.getGreen() * 255, color.getBlue() * 255);

							data.getNode().setStyle(color != null && color != Color.TRANSPARENT ? string.Format("-fx-pie-color: {0};", rgbColor) : "");
							break;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}

				// Adapt legend
				ISet<Node> nodes = this.pieChart.lookupAll("Label.chart-legend-item");
				foreach (Node node in nodes)
				{
					if (node is Label && ((Label) node).getGraphic() is Region)
					{
						Label label = (Label) node;
						Region region = (Region)label.getGraphic();

						string name = label.getText();
						if (name.Equals(INADEQUATE) || name.Equals(SATISFACTORY) || name.Equals(EXCELLENT))
						{
							TableRowHighlightRangeType tableRowHighlightRangeType = TableRowHighlightRangeType.valueOf(name);
							Color color = this.tableRowHighlight.getColor(tableRowHighlightRangeType);
							string rgbColor = String.format(Locale.ENGLISH, "rgb(%.0f, %.0f, %.0f)", color.getRed() * 255, color.getGreen() * 255, color.getBlue() * 255);
							region.setStyle(color != null && color != Color.TRANSPARENT ? string.Format("-fx-pie-color: {0};", rgbColor) : "");

							switch (tableRowHighlightRangeType.innerEnumValue)
							{
							case TableRowHighlightRangeType.InnerEnum.INADEQUATE:
								label.setText(String.format(Locale.ENGLISH, "%s \u2264 r \u003C %s", 0, options.toPercentFormat(leftBoundary, true)));
								break;
							case TableRowHighlightRangeType.InnerEnum.SATISFACTORY:
								label.setText(String.format(Locale.ENGLISH, "%s \u2264 r \u2264 %s", options.toPercentFormat(leftBoundary, false), options.toPercentFormat(rightBoundary, true)));
								break;
							case TableRowHighlightRangeType.InnerEnum.EXCELLENT:
								label.setText(String.format(Locale.ENGLISH, "%s \u003C r \u2264 %s", options.toPercentFormat(rightBoundary, false), options.toPercentFormat(1, true)));
								break;
							default:
								Console.Error.WriteLine(this.GetType().Name + ": Unsupported table row highlight type" + tableRowHighlightRangeType);
								break;
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("UIRedundancyAnalysisChart.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("UIRedundancyAnalysisChart.message.error.load.exception.header", "Error, could not load data from database."), i18n.getString("UIRedundancyAnalysisChart.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private ComboBox<TerrestrialObservationType> createTerrestrialObservationTypeComboBox(TerrestrialObservationType item, string tooltip)
		{
			ComboBox<TerrestrialObservationType> typeComboBox = new ComboBox<TerrestrialObservationType>();
			TerrestrialObservationType[] terrestrialObservationTypeArray = TerrestrialObservationType.values();

			typeComboBox.getItems().setAll(terrestrialObservationTypeArray);
			typeComboBox.getSelectionModel().select(item);
			typeComboBox.setConverter(new StringConverterAnonymousInnerClass(this));
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMinWidth(150);
			typeComboBox.setMaxWidth(double.MaxValue);

			return typeComboBox;
		}

		private class StringConverterAnonymousInnerClass : StringConverter<TerrestrialObservationType>
		{
			private readonly UIRedundancyAnalysisChart outerInstance;

			public StringConverterAnonymousInnerClass(UIRedundancyAnalysisChart outerInstance)
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
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIRedundancyAnalysisChart.TerrestrialObservationType.InnerEnum.ALL:
					return outerInstance.i18n.getString("UIRedundancyAnalysisChart.observationtype.terrestrial.all.label", "All terrestrial observations");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIRedundancyAnalysisChart.TerrestrialObservationType.InnerEnum.LEVELING:
					return outerInstance.i18n.getString("UIRedundancyAnalysisChart.observationtype.terrestrial.leveling.label", "Leveling");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIRedundancyAnalysisChart.TerrestrialObservationType.InnerEnum.DIRECTION:
					return outerInstance.i18n.getString("UIRedundancyAnalysisChart.observationtype.terrestrial.direction.label", "Directions");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIRedundancyAnalysisChart.TerrestrialObservationType.InnerEnum.HORIZONTAL_DISTANCE:
					return outerInstance.i18n.getString("UIRedundancyAnalysisChart.observationtype.terrestrial.horizontal_distance.label", "Horizontal distances");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIRedundancyAnalysisChart.TerrestrialObservationType.InnerEnum.SLOPE_DISTANCE:
					return outerInstance.i18n.getString("UIRedundancyAnalysisChart.observationtype.terrestrial.slope_distance.label", "Slope distances");
				case org.applied_geodesy.jag3d.ui.dialog.chart.UIRedundancyAnalysisChart.TerrestrialObservationType.InnerEnum.ZENITH_ANGLE:
					return outerInstance.i18n.getString("UIRedundancyAnalysisChart.observationtype.terrestrial.zenith_angle.label", "Zenith angles");
				}
				return null;
			}

			public override TerrestrialObservationType fromString(string @string)
			{
				return TerrestrialObservationType.valueOf(@string);
			}
		}

		public virtual void load()
		{
			try
			{
				SQLManager.Instance.loadTableRowHighlight();

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
				OptionDialog.showThrowableDialog(i18n.getString("UIRedundancyAnalysisChart.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("UIRedundancyAnalysisChart.message.error.load.exception.header", "Error, could not load data from database."), i18n.getString("UIRedundancyAnalysisChart.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}
	}
}