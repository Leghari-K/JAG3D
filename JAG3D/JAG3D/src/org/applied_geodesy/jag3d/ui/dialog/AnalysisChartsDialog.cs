using System;

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

namespace org.applied_geodesy.jag3d.ui.dialog
{

	using AnalysisChartType = org.applied_geodesy.jag3d.ui.dialog.chart.AnalysisChartType;
	using AnalysisChartTypeListCell = org.applied_geodesy.jag3d.ui.dialog.chart.AnalysisChartTypeListCell;
	using UIRedundancyAnalysisChart = org.applied_geodesy.jag3d.ui.dialog.chart.UIRedundancyAnalysisChart;
	using UIResidualAnalysisChart = org.applied_geodesy.jag3d.ui.dialog.chart.UIResidualAnalysisChart;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using Dialog = javafx.scene.control.Dialog;
	using ListCell = javafx.scene.control.ListCell;
	using ListView = javafx.scene.control.ListView;
	using Tooltip = javafx.scene.control.Tooltip;
	using BorderPane = javafx.scene.layout.BorderPane;
	using Region = javafx.scene.layout.Region;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;

	public class AnalysisChartsDialog
	{

		private class AnalysisChartTypeChangeListener : ChangeListener<AnalysisChartType>
		{
			private readonly AnalysisChartsDialog outerInstance;

			public AnalysisChartTypeChangeListener(AnalysisChartsDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void changed<T1>(ObservableValue<T1> observable, AnalysisChartType oldType, AnalysisChartType newType) where T1 : org.applied_geodesy.jag3d.ui.dialog.chart.AnalysisChartType
			{
				if (newType != null)
				{
					outerInstance.loadChart(newType);
				}
			}
		}

		private static AnalysisChartsDialog analysisChartsDialog = new AnalysisChartsDialog();
		private I18N i18n = I18N.Instance;
		private Dialog<Void> dialog = null;
		private Window window;
		private ListView<AnalysisChartType> analysisChartTypeList;
		private AnalysisChartType lastSelectedAnalysisChartType = AnalysisChartType.RESIDUALS;
		private BorderPane chartPane;
		private AnalysisChartsDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				analysisChartsDialog.window = value;
			}
		}

		public static Optional<Void> showAndWait()
		{
			analysisChartsDialog.init();
			analysisChartsDialog.analysisChartTypeList.getSelectionModel().clearSelection();

			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				analysisChartsDialog.analysisChartTypeList.getSelectionModel().select(analysisChartsDialog.lastSelectedAnalysisChartType);
				analysisChartsDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) analysisChartsDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return analysisChartsDialog.dialog.showAndWait();
		}


		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<Void>();
			this.dialog.setTitle(i18n.getString("AnalysisChartsDialog.title", "Analysis charts"));
			this.dialog.setHeaderText(i18n.getString("AnalysisChartsDialog.chart.header", "Analysis charts"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.CLOSE);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
		}

		private BorderPane createPane()
		{
			this.chartPane = new BorderPane();

			this.analysisChartTypeList = new ListView<AnalysisChartType>();
			this.analysisChartTypeList.getItems().addAll((AnalysisChartType[])Enum.GetValues(typeof(AnalysisChartType)));
			this.analysisChartTypeList.getSelectionModel().selectedItemProperty().addListener(new AnalysisChartTypeChangeListener(this));
			//this.analysisChartTypeList.getSelectionModel().clearAndSelect(0); // fire event

			this.analysisChartTypeList.setTooltip(new Tooltip(i18n.getString("AnalysisChartsDialog.chart.type.tooltip", "Select analysis chart type")));
			this.analysisChartTypeList.setCellFactory(new CallbackAnonymousInnerClass(this));

			this.chartPane.setLeft(this.analysisChartTypeList);

			Region spacer = new Region();
			spacer.setPrefWidth(450);
			this.chartPane.setCenter(spacer);

			return this.chartPane;
		}

		private class CallbackAnonymousInnerClass : Callback<ListView<AnalysisChartType>, ListCell<AnalysisChartType>>
		{
			private readonly AnalysisChartsDialog outerInstance;

			public CallbackAnonymousInnerClass(AnalysisChartsDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override ListCell<AnalysisChartType> call(ListView<AnalysisChartType> analysisChartTypeList)
			{
				return new AnalysisChartTypeListCell();
			}
		}

		private Node Chart
		{
			set
			{
				if (value != null)
				{
					this.chartPane.setCenter(value);
				}
			}
		}

		private void loadChart(AnalysisChartType analysisChartType)
		{
			Node node = null;
			switch (analysisChartType)
			{
			case AnalysisChartType.RESIDUALS:
				this.dialog.setHeaderText(i18n.getString("AnalysisChartsDialog.chart.type.residuals.header", "Histogram of normalized residuals"));
				node = UIResidualAnalysisChart.Instance.Node;
				this.Chart = node;

				Platform.runLater(() =>
				{
				UIResidualAnalysisChart.Instance.load();
				});

				break;

			case AnalysisChartType.REDUNDANCY:
				this.dialog.setHeaderText(i18n.getString("AnalysisChartsDialog.chart.type.redundancy.header", "Pie chart of redundancy"));
				node = UIRedundancyAnalysisChart.Instance.Node;
				this.Chart = node;

				Platform.runLater(() =>
				{
				UIRedundancyAnalysisChart.Instance.load();
				});

				break;
			}
			this.lastSelectedAnalysisChartType = analysisChartType;
		}
	}

}