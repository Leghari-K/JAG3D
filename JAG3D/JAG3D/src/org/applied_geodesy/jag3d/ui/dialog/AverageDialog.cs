using System;
using System.Collections.Generic;
using System.Threading;

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

	using DefaultAverageThreshold = org.applied_geodesy.adjustment.network.DefaultAverageThreshold;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using IllegalProjectionPropertyException = org.applied_geodesy.adjustment.network.sql.IllegalProjectionPropertyException;
	using SQLAdjustmentManager = org.applied_geodesy.adjustment.network.sql.SQLAdjustmentManager;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using UIAverageObservationTableBuilder = org.applied_geodesy.jag3d.ui.table.UIAverageObservationTableBuilder;
	using AveragedObservationRow = org.applied_geodesy.jag3d.ui.table.row.AveragedObservationRow;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DoubleTextField = org.applied_geodesy.ui.textfield.DoubleTextField;
	using ValueSupport = org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using DatabaseVersionMismatchException = org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Task = javafx.concurrent.Task;
	using WorkerStateEvent = javafx.concurrent.WorkerStateEvent;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using Button = javafx.scene.control.Button;
	using ButtonType = javafx.scene.control.ButtonType;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using DialogEvent = javafx.scene.control.DialogEvent;
	using Label = javafx.scene.control.Label;
	using ProgressIndicator = javafx.scene.control.ProgressIndicator;
	using TableView = javafx.scene.control.TableView;
	using TitledPane = javafx.scene.control.TitledPane;
	using AlertType = javafx.scene.control.Alert.AlertType;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using StackPane = javafx.scene.layout.StackPane;
	using VBox = javafx.scene.layout.VBox;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using WindowEvent = javafx.stage.WindowEvent;

	public class AverageDialog
	{

		private class DoubleValueChangeListener : ChangeListener<double>
		{
			private readonly AverageDialog outerInstance;

			internal ObservationType observationType;

			internal DoubleValueChangeListener(AverageDialog outerInstance, ObservationType observationType)
			{
				this.outerInstance = outerInstance;
				this.observationType = observationType;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (!outerInstance.ignoreChanges && newValue != null && newValue > 0)
				{
					outerInstance.save(this.observationType, newValue.Value);
				}
			}
		}

		private class AverageEvent : EventHandler<ActionEvent>
		{
			private readonly AverageDialog outerInstance;

			public AverageEvent(AverageDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				outerInstance.okButton.setDisable(true);
				outerInstance.process();
				@event.consume();
			}
		}

		private class AverageTask : Task<IList<Observation>>
		{
			private readonly AverageDialog outerInstance;

			internal SQLAdjustmentManager dataBaseManager;
			internal AverageTask(AverageDialog outerInstance, SQLAdjustmentManager dataBaseManager)
			{
				this.outerInstance = outerInstance;
				this.dataBaseManager = dataBaseManager;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override protected java.util.List<org.applied_geodesy.adjustment.network.observation.Observation> call() throws Exception
			protected internal override IList<Observation> call()
			{
				IList<Observation> observations = null;
				try
				{
					outerInstance.preventClosing = true;
					outerInstance.okButton.setDisable(true);
					outerInstance.progressIndicatorPane.setVisible(true);
					outerInstance.settingPane.setDisable(true);

					observations = this.dataBaseManager.averageDetermination(false);
					return observations;
				}

				finally
				{
					outerInstance.okButton.setDisable(false);
					outerInstance.progressIndicatorPane.setVisible(false);
					outerInstance.settingPane.setDisable(false);
					if (observations == null || observations.Count == 0)
					{
						outerInstance.preventClosing = false;
					}
				}
			}
		}

		private I18N i18n = I18N.Instance;
		private static AverageDialog averageDialog = new AverageDialog();
		private Dialog<Void> dialog = null;
		private Window window;
		private bool ignoreChanges = false;
		private ProgressIndicator progressIndicator = new ProgressIndicator(ProgressIndicator.INDETERMINATE_PROGRESS);
		private bool preventClosing = false;
		private AverageTask averageTask = null;
		private Button okButton;
		private IDictionary<ObservationType, DoubleTextField> thresholdFieldMap = new Dictionary<ObservationType, DoubleTextField>(10);
		private UIAverageObservationTableBuilder tableBuilder = UIAverageObservationTableBuilder.Instance;
		private Node settingPane, progressIndicatorPane;

		private AverageDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				averageDialog.window = value;
			}
		}

		public static Optional<Void> showAndWait()
		{
			averageDialog.init();
			averageDialog.load();
			averageDialog.reset();
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				averageDialog.reset();
				averageDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) averageDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return averageDialog.dialog.showAndWait();
		}

		private void reset()
		{
			this.preventClosing = false;
			this.settingPane.setDisable(false);
			this.progressIndicatorPane.setVisible(false);
			this.okButton.setDisable(false);
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<Void>();
			this.dialog.setTitle(i18n.getString("AverageDialog.title", "Averaging"));
			this.dialog.setHeaderText(i18n.getString("AverageDialog.header", "Averaging repeated observations"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CLOSE);
			this.okButton = (Button)this.dialog.getDialogPane().lookupButton(ButtonType.OK);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.setResizable(true);

			this.dialog.getDialogPane().getScene().getWindow().setOnCloseRequest(new EventHandlerAnonymousInnerClass(this));

			this.dialog.setOnCloseRequest(new EventHandlerAnonymousInnerClass2(this));

			this.settingPane = this.createSettingPane();
			this.progressIndicatorPane = this.createProgressIndicatorPane();
			this.progressIndicatorPane.setVisible(false);

			StackPane stackPane = new StackPane();
			stackPane.setAlignment(Pos.CENTER);
			stackPane.setMaxSize(double.MaxValue, Region.USE_PREF_SIZE);
			stackPane.getChildren().addAll(this.settingPane, this.progressIndicatorPane);

			this.okButton.addEventFilter(ActionEvent.ACTION, new AverageEvent(this));
			this.dialog.getDialogPane().setContent(stackPane);
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<WindowEvent>
		{
			private readonly AverageDialog outerInstance;

			public EventHandlerAnonymousInnerClass(AverageDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WindowEvent @event)
			{
				if (outerInstance.preventClosing)
				{
					@event.consume();
				}
			}
		}

		private class EventHandlerAnonymousInnerClass2 : EventHandler<DialogEvent>
		{
			private readonly AverageDialog outerInstance;

			public EventHandlerAnonymousInnerClass2(AverageDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(DialogEvent @event)
			{
				if (outerInstance.preventClosing)
				{
					@event.consume();
				}
			}
		}

		private Node createProgressIndicatorPane()
		{
			VBox box = new VBox();
			box.setAlignment(Pos.CENTER);
			box.getChildren().setAll(this.progressIndicator);
			return box;
		}

		private VBox createSettingPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMaxWidth(double.MaxValue);
			gridPane.setHgap(20);
			gridPane.setVgap(5);
			gridPane.setPadding(new Insets(5, 5, 5, 5)); // oben, recht, unten, links
			gridPane.setAlignment(Pos.TOP_CENTER);

			int row = 0;
			this.addRow(gridPane, row++, ObservationType.LEVELING);
			this.addRow(gridPane, row++, ObservationType.DIRECTION);
			this.addRow(gridPane, row++, ObservationType.HORIZONTAL_DISTANCE);
			this.addRow(gridPane, row++, ObservationType.SLOPE_DISTANCE);
			this.addRow(gridPane, row++, ObservationType.ZENITH_ANGLE);

			this.addRow(gridPane, row++, ObservationType.GNSS1D);
			this.addRow(gridPane, row++, ObservationType.GNSS2D);
			this.addRow(gridPane, row++, ObservationType.GNSS3D);

			Label warningLabel = new Label(i18n.getString("AverageDialog.warning.label", "Please note: Averaging repeated measurements will reduce\r\nthe number of observations and is an irreversible process"));
			warningLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			warningLabel.setWrapText(true);

			VBox box = new VBox();
			VBox.setVgrow(gridPane, Priority.ALWAYS);
			box.setPadding(new Insets(5, 10, 5, 10));
			box.setSpacing(10);
			box.getChildren().addAll(warningLabel, this.createTitledPane(i18n.getString("AverageDialog.threshold.title", "Threshold w.r.t median"), gridPane));
			return box;
		}

		private void addRow(GridPane parent, int row, ObservationType observationType)
		{
			CellValueType valueType = null;
			string labelText = null;
			double value = DefaultAverageThreshold.getThreshold(observationType);
			switch (observationType.innerEnumValue)
			{
			case ObservationType.InnerEnum.LEVELING:
				labelText = i18n.getString("AverageDialog.leveling.label", "Leveling:");
				valueType = CellValueType.LENGTH_RESIDUAL;
				break;

			case ObservationType.InnerEnum.DIRECTION:
				labelText = i18n.getString("AverageDialog.direction.label", "Direction:");
				valueType = CellValueType.ANGLE_RESIDUAL;
				break;

			case ObservationType.InnerEnum.ZENITH_ANGLE:
				labelText = i18n.getString("AverageDialog.zenith_angle.label", "Zenith angle:");
				valueType = CellValueType.ANGLE_RESIDUAL;
				break;

			case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				labelText = i18n.getString("AverageDialog.horizontal_distance.label", "Horizontal distance:");
				valueType = CellValueType.LENGTH_RESIDUAL;
				break;

			case ObservationType.InnerEnum.SLOPE_DISTANCE:
				labelText = i18n.getString("AverageDialog.slope_distance.label", "Slope distance:");
				valueType = CellValueType.LENGTH_RESIDUAL;
				break;

			case ObservationType.InnerEnum.GNSS1D:
				labelText = i18n.getString("AverageDialog.gnss.1d.label", "GNSS baseline 1D:");
				valueType = CellValueType.LENGTH_RESIDUAL;
				break;

			case ObservationType.InnerEnum.GNSS2D:
				labelText = i18n.getString("AverageDialog.gnss.2d.label", "GNSS baseline 2D:");
				valueType = CellValueType.LENGTH_RESIDUAL;
				break;

			case ObservationType.InnerEnum.GNSS3D:
				labelText = i18n.getString("AverageDialog.gnss.3d.label", "GNSS baseline 3D:");
				valueType = CellValueType.LENGTH_RESIDUAL;
				break;
			}

			if (!string.ReferenceEquals(labelText, null) && valueType != null && value > 0)
			{
				DoubleTextField thresholdField = new DoubleTextField(value, valueType, true, DoubleTextField.ValueSupport.EXCLUDING_INCLUDING_INTERVAL);
				thresholdField.setMinWidth(100);
				thresholdField.setMaxWidth(double.MaxValue);
				thresholdField.numberProperty().addListener(new DoubleValueChangeListener(this, observationType));
				this.thresholdFieldMap[observationType] = thresholdField;
				Label label = new Label(labelText);
				label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

				GridPane.setHgrow(label, Priority.SOMETIMES);
				GridPane.setHgrow(thresholdField, Priority.ALWAYS);

				parent.add(label, 0, row);
				parent.add(thresholdField, 1, row);
			}
		}

		private TitledPane createTitledPane(string title, Node content)
		{
			TitledPane titledPane = new TitledPane();
			titledPane.setCollapsible(false);
			titledPane.setAnimated(false);
			titledPane.setContent(content);
			titledPane.setPadding(new Insets(0, 10, 5, 10)); // oben, links, unten, rechts
			titledPane.setText(title);
			return titledPane;
		}

		private void load()
		{
			try
			{
				okButton.setDisable(false);

				this.ignoreChanges = true;
				foreach (KeyValuePair<ObservationType, DoubleTextField> item in this.thresholdFieldMap.SetOfKeyValuePairs())
				{
					ObservationType type = item.Key;
					DoubleTextField field = item.Value;
					double value = SQLManager.Instance.getAverageThreshold(type);
					value = value > 0 ? value : DefaultAverageThreshold.getThreshold(type);
					field.Value = value;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("AverageDialog.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("AverageDialog.message.error.load.exception.header", "Error, could not load averaging properties from database."), i18n.getString("AverageDialog.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
			finally
			{
				this.ignoreChanges = false;
			}
		}

		private void save(ObservationType observationType, double value)
		{
			try
			{
				SQLManager.Instance.save(observationType, value);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("AverageDialog.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("AverageDialog.message.error.save.exception.header", "Error, could not save averaging properties to database."), i18n.getString("AverageDialog.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private void process()
		{
			this.reset();
			this.averageTask = new AverageTask(this, SQLManager.Instance.AdjustmentManager);
			this.averageTask.setOnSucceeded(new EventHandlerAnonymousInnerClass3(this));

			this.averageTask.setOnFailed(new EventHandlerAnonymousInnerClass4(this));



			Thread th = new Thread(this.averageTask);
			th.setDaemon(true);
			th.Start();
		}

		private class EventHandlerAnonymousInnerClass3 : EventHandler<WorkerStateEvent>
		{
			private readonly AverageDialog outerInstance;

			public EventHandlerAnonymousInnerClass3(AverageDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.preventClosing = false;
				IList<Observation> observations = outerInstance.averageTask.getValue();
				if (observations != null && observations.Count > 0)
				{
					IList<AveragedObservationRow> averagedObservationRows = new List<AveragedObservationRow>(observations.Count);
					foreach (Observation observation in observations)
					{
						averagedObservationRows.Add(new AveragedObservationRow(observation));
					}

					TableView<AveragedObservationRow> table = outerInstance.tableBuilder.Table;
					table.getItems().setAll(averagedObservationRows);
					table.setPrefHeight(200);
					table.setPrefWidth(250);
					table.sort();
					string title = outerInstance.i18n.getString("AverageDialog.message.error.threshold.title", "Exceeding thresholds");
					string header = outerInstance.i18n.getString("AverageDialog.message.error.threshold.header", "Error, averaging could not be finished due to exceeded threshold values.\r\nPlease correct the listed observations or increase the threshold value");
					string message = outerInstance.i18n.getString("AverageDialog.message.error.threshold.message","List of exceeded observations");

					Platform.runLater(() =>
					{
					OptionDialog.showContentDialog(AlertType.ERROR, title, header, message, table);
					});
				}
				else
				{
		//					MultipleSelectionModel<TreeItem<TreeItemValue>> selectionModel = UITreeBuilder.getInstance().getTree().getSelectionModel();
		//					TreeItem<TreeItemValue> treeItem = selectionModel.getSelectedItem();
		//					selectionModel.clearSelection();
		//					if (treeItem != null)
		//						selectionModel.select(treeItem);
		//					else
		//						selectionModel.select(0);
					UITreeBuilder.Instance.handleTreeSelections();
					outerInstance.dialog.hide();
				}
			}

		}

		private class EventHandlerAnonymousInnerClass4 : EventHandler<WorkerStateEvent>
		{
			private readonly AverageDialog outerInstance;

			public EventHandlerAnonymousInnerClass4(AverageDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.preventClosing = false;
				outerInstance.dialog.hide();
				Exception throwable = outerInstance.averageTask.getException();
				if (throwable != null)
				{
					if (throwable is IllegalProjectionPropertyException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("AverageDialog.message.error.projection.exception.title", "Initialization error"), outerInstance.i18n.getString("AverageDialog.message.error.projection.exception.header", "Error, the project contains unsupported projection properties."), outerInstance.i18n.getString("AverageDialog.message.error.projection.exception.message", "An exception has occurred during estimation of approximation."), throwable);
						});
					}
					else if (throwable is DatabaseVersionMismatchException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("AverageDialog.message.error.databaseversion.exception.title", "Version error"), outerInstance.i18n.getString("AverageDialog.message.error.databaseversion.exception.header", "Error, the database version is unsupported."), outerInstance.i18n.getString("AverageDialog.message.error.databaseversion.exception.message", "An exception has occurred during network adjustment."), throwable);
						});
					}
					else
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("AverageDialog.message.error.averaging.exception.title", "Unexpected Error"), outerInstance.i18n.getString("AverageDialog.message.error.averaging.exception.header", "Error, averaging failed. An unexpected exception occurred."), outerInstance.i18n.getString("AverageDialog.message.error.averaging.exception.message", "An exception has occurred during averaging process."), throwable);
						});
					}
					Console.WriteLine(throwable.ToString());
					Console.Write(throwable.StackTrace);
				}
				UITreeBuilder.Instance.Tree.getSelectionModel().select(0);
			}
		}
	}

}