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

	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using SQLApproximationManager = org.applied_geodesy.adjustment.network.approximation.sql.SQLApproximationManager;
	using PointTypeMismatchException = org.applied_geodesy.jag3d.sql.PointTypeMismatchException;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using UnderDeterminedPointException = org.applied_geodesy.jag3d.sql.UnderDeterminedPointException;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
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
	using RadioButton = javafx.scene.control.RadioButton;
	using TextArea = javafx.scene.control.TextArea;
	using ToggleGroup = javafx.scene.control.ToggleGroup;
	using Tooltip = javafx.scene.control.Tooltip;
	using AlertType = javafx.scene.control.Alert.AlertType;
	using Region = javafx.scene.layout.Region;
	using StackPane = javafx.scene.layout.StackPane;
	using VBox = javafx.scene.layout.VBox;
	using Text = javafx.scene.text.Text;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using WindowEvent = javafx.stage.WindowEvent;

	public class ApproximationValuesDialog
	{

		private class StartAdjustmentEvent : EventHandler<ActionEvent>
		{
			private readonly ApproximationValuesDialog outerInstance;

			public StartAdjustmentEvent(ApproximationValuesDialog outerInstance)
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

		private class ApproximationEstimationTask : Task<bool>, PropertyChangeListener
		{
			private readonly ApproximationValuesDialog outerInstance;

			internal SQLApproximationManager approximationManager;
			internal ApproximationEstimationTask(ApproximationValuesDialog outerInstance, SQLApproximationManager approximationManager)
			{
				this.outerInstance = outerInstance;
				this.approximationManager = approximationManager;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override protected System.Nullable<bool> call() throws Exception
			protected internal override bool? call()
			{
				EstimationStateType status1d = EstimationStateType.ERROR_FREE_ESTIMATION;
				EstimationStateType status2d = EstimationStateType.ERROR_FREE_ESTIMATION;
				this.updateProgress(ProgressIndicator.INDETERMINATE_PROGRESS, ProgressIndicator.INDETERMINATE_PROGRESS);
				approximationManager.addPropertyChangeListener(this);
				try
				{
					outerInstance.preventClosing = true;
					outerInstance.okButton.setDisable(true);
					outerInstance.progressIndicatorPane.setVisible(true);
					outerInstance.settingPane.setDisable(true);

					// Global check - an exception occurs, if check fails
					SQLManager.Instance.checkNumberOfObersvationsPerUnknownParameter();

					if (outerInstance.transferDatumAndNewPointsResultsRadioButton.isSelected() || outerInstance.transferNewPointsResultsRadioButton.isSelected())
					{
						approximationManager.transferAposteriori2AprioriValues(outerInstance.transferDatumAndNewPointsResultsRadioButton.isSelected());
					}
					else if (outerInstance.estimateDatumAndNewPointsRadioButton.isSelected() || outerInstance.estimateNewPointsRadioButton.isSelected())
					{
						approximationManager.EstimateDatumsPoints = outerInstance.estimateDatumAndNewPointsRadioButton.isSelected();
						approximationManager.adjustApproximationValues(25.0);
					}

					status2d = approximationManager.EstimationStatus2D;
					status1d = approximationManager.EstimationStatus1D;

					return status1d == EstimationStateType.ERROR_FREE_ESTIMATION && status2d == EstimationStateType.ERROR_FREE_ESTIMATION;
				}
				finally
				{
					approximationManager.removePropertyChangeListener(this);
					outerInstance.okButton.setDisable(false);
					outerInstance.progressIndicatorPane.setVisible(false);
					outerInstance.settingPane.setDisable(false);
					if (status1d == EstimationStateType.ERROR_FREE_ESTIMATION && status2d == EstimationStateType.ERROR_FREE_ESTIMATION)
					{
						outerInstance.preventClosing = false;
					}
				}
			}

			protected internal override void failed()
			{
				outerInstance.preventClosing = false;
				base.failed();
			}

			protected internal override void cancelled()
			{
				base.cancelled();
				if (this.approximationManager != null)
				{
					this.approximationManager.interrupt();
				}
			}

			protected internal override void updateProgress(double workDone, double max)
			{
				base.updateProgress(workDone, max);
				Platform.runLater(() =>
				{
				outerInstance.progressIndicator.setProgress(workDone);
				if (workDone >= 1.0)
				{
					Node node = outerInstance.progressIndicator.lookup(".percentage");
					if (node != null && node is Text)
					{
						Text text = (Text)node;
						text.setText(outerInstance.i18n.getString("ApproximationValuesDialog.done.label", "Done"));
						outerInstance.progressIndicator.setPrefWidth(text.getLayoutBounds().getWidth());
					}
				}
				});
			}

			public override void propertyChange(PropertyChangeEvent evt)
			{
				//System.out.println(evt);
			}
		}

		private I18N i18n = I18N.Instance;
		private static ApproximationValuesDialog approximationValuesDialog = new ApproximationValuesDialog();
		private Dialog<EstimationStateType> dialog = null;
		private Window window;
		private bool preventClosing = false;
		private Button okButton;
		private ProgressIndicator progressIndicator = new ProgressIndicator(ProgressIndicator.INDETERMINATE_PROGRESS);
		private Node settingPane, progressIndicatorPane;
		private RadioButton estimateDatumAndNewPointsRadioButton, estimateNewPointsRadioButton, transferDatumAndNewPointsResultsRadioButton, transferNewPointsResultsRadioButton;
		private ApproximationEstimationTask approximationEstimationTask = null;
		private ApproximationValuesDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				approximationValuesDialog.window = value;
			}
		}

		public static Optional<EstimationStateType> showAndWait()
		{
			approximationValuesDialog.init();
			approximationValuesDialog.reset();
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				approximationValuesDialog.reset();
				approximationValuesDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) approximationValuesDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return approximationValuesDialog.dialog.showAndWait();
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

			this.settingPane = this.createSettingPane();
			this.progressIndicatorPane = this.createProgressIndicatorPane();
			this.progressIndicatorPane.setVisible(false);

			StackPane stackPane = new StackPane();
			stackPane.setAlignment(Pos.CENTER);
			stackPane.setMaxSize(double.MaxValue, Region.USE_PREF_SIZE);
			stackPane.getChildren().addAll(this.settingPane, this.progressIndicatorPane);

			this.dialog = new Dialog<EstimationStateType>();
			this.dialog.setTitle(i18n.getString("ApproximationValuesDialog.title", "Approximation values"));
			this.dialog.setHeaderText(i18n.getString("ApproximationValuesDialog.header", "Estimate approximation values using terrestrial observations"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);

			this.dialog.getDialogPane().setContent(stackPane);
			this.dialog.setResizable(true);

			this.okButton = (Button)this.dialog.getDialogPane().lookupButton(ButtonType.OK);
			this.okButton.addEventFilter(ActionEvent.ACTION, new StartAdjustmentEvent(this));

			this.dialog.getDialogPane().getScene().getWindow().setOnCloseRequest(new EventHandlerAnonymousInnerClass(this));

			this.dialog.setOnCloseRequest(new EventHandlerAnonymousInnerClass2(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<WindowEvent>
		{
			private readonly ApproximationValuesDialog outerInstance;

			public EventHandlerAnonymousInnerClass(ApproximationValuesDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WindowEvent @event)
			{
				if (outerInstance.preventClosing)
				{
					@event.consume();
					if (outerInstance.approximationEstimationTask != null)
					{
						outerInstance.approximationEstimationTask.cancelled();
					}
				}
			}
		}

		private class EventHandlerAnonymousInnerClass2 : EventHandler<DialogEvent>
		{
			private readonly ApproximationValuesDialog outerInstance;

			public EventHandlerAnonymousInnerClass2(ApproximationValuesDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(DialogEvent @event)
			{
				if (outerInstance.preventClosing)
				{
					@event.consume();
					if (outerInstance.approximationEstimationTask != null)
					{
						outerInstance.approximationEstimationTask.cancelled();
					}
				}
			}
		}

		private Node createProgressIndicatorPane()
		{
			VBox box = new VBox();
			box.setAlignment(Pos.CENTER);
			box.getChildren().setAll(this.progressIndicator);
			this.progressIndicator.setMinWidth(50);
			this.progressIndicator.setMinHeight(50);
			return box;
		}

		private Node createSettingPane()
		{
			string estimateDatumAndNewPointsRadioButtonLabel = i18n.getString("ApproximationValuesDialog.estimate.datum_and_new.label", "Derive approximations for datum and new points");
			string estimateDatumAndNewPointsRadioButtonTooltip = i18n.getString("ApproximationValuesDialog.estimate.datum_and_new.title", "If checked, datum and new points will be estimated");

			string estimateNewPointsRadioButtonLabel = i18n.getString("ApproximationValuesDialog.estimate.new.label", "Derive approximations for new points");
			string estimateNewPointsRadioButtonTooltip = i18n.getString("ApproximationValuesDialog.estimate.new.title", "If checked, new points will be estimated");

			string transferDatumAndNewPointsResultsLabel = i18n.getString("ApproximationValuesDialog.transfer.datum_and_new.label", "Transfer adjusted coordinates of datum and new points as well as additional parameters");
			string transferDatumAndNewPointsResultsTooltip = i18n.getString("ApproximationValuesDialog.transfer.datum_and_new.title", "If checked, adjusted coordinates of datum and new points as well as additional parameters will be transferred to apprixmation values");

			string transferNewPointsResultsLabel = i18n.getString("ApproximationValuesDialog.transfer.new.label", "Transfer adjusted coordinates of new points as well as additional parameters");
			string transferNewPointsResultsTooltip = i18n.getString("ApproximationValuesDialog.transfer.new.title", "If checked, adjusted coordinates of new points as well as additional parameters will be transferred to apprixmation values");

			this.estimateDatumAndNewPointsRadioButton = this.createRadioButton(estimateDatumAndNewPointsRadioButtonLabel, estimateDatumAndNewPointsRadioButtonTooltip);
			this.estimateNewPointsRadioButton = this.createRadioButton(estimateNewPointsRadioButtonLabel, estimateNewPointsRadioButtonTooltip);
			this.transferDatumAndNewPointsResultsRadioButton = this.createRadioButton(transferDatumAndNewPointsResultsLabel, transferDatumAndNewPointsResultsTooltip);
			this.transferNewPointsResultsRadioButton = this.createRadioButton(transferNewPointsResultsLabel, transferNewPointsResultsTooltip);

			ToggleGroup group = new ToggleGroup();
			group.getToggles().addAll(this.estimateDatumAndNewPointsRadioButton, this.estimateNewPointsRadioButton, this.transferDatumAndNewPointsResultsRadioButton, this.transferNewPointsResultsRadioButton);

			this.estimateDatumAndNewPointsRadioButton.setSelected(true);

			VBox box = new VBox(10);
			box.setMaxWidth(double.MaxValue);
			box.setAlignment(Pos.CENTER_LEFT);
			box.getChildren().addAll(this.estimateDatumAndNewPointsRadioButton, this.estimateNewPointsRadioButton, new Region(), this.transferDatumAndNewPointsResultsRadioButton, this.transferNewPointsResultsRadioButton);

			Platform.runLater(() =>
			{
			estimateDatumAndNewPointsRadioButton.requestFocus();
			});

			return box;
		}

		private RadioButton createRadioButton(string title, string tooltip)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			RadioButton radioButton = new RadioButton();
			radioButton.setGraphic(label);
			radioButton.setTooltip(new Tooltip(tooltip));
			radioButton.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			radioButton.setMaxWidth(double.MaxValue);
			return radioButton;
		}

		private void process()
		{
			this.reset();
			// Try to estimate approx. values
			SQLApproximationManager approximationManager = SQLManager.Instance.ApproximationManager;
			this.approximationEstimationTask = new ApproximationEstimationTask(this, approximationManager);
			this.approximationEstimationTask.setOnSucceeded(new EventHandlerAnonymousInnerClass3(this, approximationManager));

			this.approximationEstimationTask.setOnFailed(new EventHandlerAnonymousInnerClass4(this));

			Thread th = new Thread(this.approximationEstimationTask);
			th.setDaemon(true);
			th.Start();
		}

		private class EventHandlerAnonymousInnerClass3 : EventHandler<WorkerStateEvent>
		{
			private readonly ApproximationValuesDialog outerInstance;

			private SQLApproximationManager approximationManager;

			public EventHandlerAnonymousInnerClass3(ApproximationValuesDialog outerInstance, SQLApproximationManager approximationManager)
			{
				this.outerInstance = outerInstance;
				this.approximationManager = approximationManager;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.preventClosing = false;
				if (approximationManager != null)
				{
					EstimationStateType state1D = approximationManager.EstimationStatus1D;
					EstimationStateType state2D = approximationManager.EstimationStatus2D;

					if (state1D != EstimationStateType.INTERRUPT && state2D != EstimationStateType.INTERRUPT)
					{

						int subCounter1d = approximationManager.SubSystemsCounter1D;
						int subCounter2d = approximationManager.SubSystemsCounter2D;

						ISet<string> underdeterminedPointNames = new HashSet<string>(approximationManager.UnderdeterminedPointNames1D);
						underdeterminedPointNames.addAll(approximationManager.UnderdeterminedPointNames2D);

						ISet<string> outliers = new HashSet<string>(approximationManager.Outliers1D);
						outliers.addAll(approximationManager.Outliers2D);

						if (subCounter1d > 1 || subCounter2d > 1)
						{
							OptionDialog.showErrorDialog(outerInstance.i18n.getString("ApproximationValuesDialog.message.error.subsystem.title", "No consistent frame"), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.subsystem.header", "Error, could not determine a consistent reference frame."), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.subsystem.message", "Not enough pass points to transform detected subsystems to global frame."));
						}
						else if (underdeterminedPointNames.Count > 0)
						{
							TextArea textArea = new TextArea();
							int len = underdeterminedPointNames.Count;
							int cnt = 1;
							foreach (string name in underdeterminedPointNames)
							{
								textArea.appendText(name);
								if (cnt++ < len)
								{
									textArea.appendText(", ");
								}
							}
							textArea.setEditable(false);
							textArea.setWrapText(true);

							OptionDialog.showContentDialog(AlertType.ERROR, outerInstance.i18n.getString("ApproximationValuesDialog.message.error.underdetermination.title", "Underdetermined points"), String.format(outerInstance.i18n.getString("ApproximationValuesDialog.message.error.underdetermination.header", "Error, detect %d underdetermined points."), underdeterminedPointNames.Count), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.underdetermination.message", "List of underdetermined points."), textArea);
						}
						else if (outliers.Count > 0)
						{
							TextArea textArea = new TextArea();
							int len = outliers.Count;
							int cnt = 1;
							foreach (string name in outliers)
							{
								textArea.appendText(name);
								if (cnt++ < len)
								{
									textArea.appendText(", ");
								}
							}
							textArea.setEditable(false);
							textArea.setWrapText(true);

							OptionDialog.showContentDialog(AlertType.WARNING, outerInstance.i18n.getString("ApproximationValuesDialog.message.error.subsystem.exception.title", "Badly conditioned points"), String.format(outerInstance.i18n.getString("ApproximationValuesDialog.message.error.subsystem.exception.header", "Error, detect %d badly conditioned points.\r\nPlease check related observations."), outliers.Count), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.subsystem.exception.message", "List of badly conditioned points."), textArea);
						}
					}
					approximationManager.clearAll();
				}

		//				MultipleSelectionModel<TreeItem<TreeItemValue>> selectionModel = UITreeBuilder.getInstance().getTree().getSelectionModel();
		//				TreeItem<TreeItemValue> treeItem = selectionModel.getSelectedItem();
		//				selectionModel.clearSelection();
		//				if (treeItem != null)
		//					selectionModel.select(treeItem);
		//				else
		//					selectionModel.select(0);
				UITreeBuilder.Instance.handleTreeSelections();
				outerInstance.dialog.hide();
			}
		}

		private class EventHandlerAnonymousInnerClass4 : EventHandler<WorkerStateEvent>
		{
			private readonly ApproximationValuesDialog outerInstance;

			public EventHandlerAnonymousInnerClass4(ApproximationValuesDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.preventClosing = false;
				outerInstance.dialog.hide();
				Exception throwable = outerInstance.approximationEstimationTask.getException();
				if (throwable != null)
				{
					if (throwable is SQLException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("ApproximationValuesDialog.message.error.approximation_value.exception.title", "Unexpected Error"), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.approximation_value.exception.header", "Error, estimation of approximation values failed."), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.approximation_value.exception.message", "An exception has occurred during estimation of approximation."), throwable);
						});
					}
					else if (throwable is PointTypeMismatchException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("ApproximationValuesDialog.message.error.pointtypemismatch.exception.title", "Initialization error"), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.pointtypemismatch.exception.header", "Error, the project contains uncombinable point typs, i.e. datum points as well as reference or stochastic points."), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.pointtypemismatch.exception.message", "An exception has occurred during estimation of approximation."), throwable);
						});
					}
					else if (throwable is UnderDeterminedPointException)
					{
						Platform.runLater(() =>
						{
						UnderDeterminedPointException e = (UnderDeterminedPointException)throwable;
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("ApproximationValuesDialog.message.error.underdeterminded.exception.title", "Initialization error"), String.format(outerInstance.i18n.getString("ApproximationValuesDialog.message.error.underdeterminded.exception.header", "Error, the point %s of dimension %d has only %d observations and is indeterminable."), e.PointName, e.Dimension, e.NumberOfObservations), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.underdeterminded.exception.message", "An exception has occurred during estimation of approximation."), throwable);
						});
					}
					else
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("ApproximationValuesDialog.message.error.approximation_value.exception.title", "Unexpected Error"), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.approximation_value.exception.header", "Error, estimation of approximation values failed."), outerInstance.i18n.getString("ApproximationValuesDialog.message.error.approximation_value.exception.message", "An exception has occurred during estimation of approximation."), throwable);
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