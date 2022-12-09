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

	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using UICongruentPointTableBuilder = org.applied_geodesy.jag3d.ui.table.UICongruentPointTableBuilder;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DoubleTextField = org.applied_geodesy.ui.textfield.DoubleTextField;
	using ValueSupport = org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

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
	using CheckBox = javafx.scene.control.CheckBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using DialogEvent = javafx.scene.control.DialogEvent;
	using Label = javafx.scene.control.Label;
	using ProgressIndicator = javafx.scene.control.ProgressIndicator;
	using TableView = javafx.scene.control.TableView;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Region = javafx.scene.layout.Region;
	using StackPane = javafx.scene.layout.StackPane;
	using VBox = javafx.scene.layout.VBox;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using WindowEvent = javafx.stage.WindowEvent;

	public class CongruentPointDialog
	{

		private class CongruentPointEvent : EventHandler<ActionEvent>
		{
			private readonly CongruentPointDialog outerInstance;

			public CongruentPointEvent(CongruentPointDialog outerInstance)
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


		private class DimensionChangeListener : ChangeListener<bool>
		{
			private readonly CongruentPointDialog outerInstance;

			public DimensionChangeListener(CongruentPointDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				outerInstance.okButton.setDisable(!outerInstance.dimensionOneCheckBox.isSelected() && !outerInstance.dimensionTwoCheckBox.isSelected() && !outerInstance.dimensionThreeCheckBox.isSelected());
			}
		}

		private class ProcessSQLTask : Task<IList<TerrestrialObservationRow>>
		{
			private readonly CongruentPointDialog outerInstance;

			public ProcessSQLTask(CongruentPointDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override protected java.util.List<org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow> call() throws Exception
			protected internal override IList<TerrestrialObservationRow> call()
			{
				try
				{
					outerInstance.preventClosing = true;
					outerInstance.progressIndicatorPane.setVisible(true);
					outerInstance.settingPane.setDisable(true);
					outerInstance.okButton.setDisable(true);
					double distance = outerInstance.snapDistanceTextField.Number.Value;
					bool dimension1D = outerInstance.dimensionOneCheckBox.isSelected();
					bool dimension2D = outerInstance.dimensionTwoCheckBox.isSelected();
					bool dimension3D = outerInstance.dimensionThreeCheckBox.isSelected();
					IList<TerrestrialObservationRow> rows = SQLManager.Instance.getCongruentPoints(distance, dimension1D, dimension2D, dimension3D);
					return rows;
				}
				finally
				{
					outerInstance.reset();
				}
			}

			protected internal override void succeeded()
			{
				outerInstance.preventClosing = false;
				base.succeeded();
			}

			protected internal override void failed()
			{
				outerInstance.preventClosing = false;
				base.failed();
			}
		}

		private I18N i18n = I18N.Instance;
		private static UICongruentPointTableBuilder tableBuilder = UICongruentPointTableBuilder.Instance;
		private static CongruentPointDialog congruentPointDialog = new CongruentPointDialog();
		private Dialog<Void> dialog = null;
		private Window window;
		private DoubleTextField snapDistanceTextField;
		private ProgressIndicator progressIndicator = new ProgressIndicator(ProgressIndicator.INDETERMINATE_PROGRESS);
		private bool preventClosing = false;
		private ProcessSQLTask processSQLTask = null;
		private CheckBox dimensionOneCheckBox, dimensionTwoCheckBox, dimensionThreeCheckBox;
		private CongruentPointDialog()
		{
		}
		private Button okButton;
		private Node progressIndicatorPane, settingPane;

		public static Window Owner
		{
			set
			{
				congruentPointDialog.window = value;
			}
		}

		public static Optional<Void> showAndWait()
		{
			congruentPointDialog.init();
			congruentPointDialog.reset();
			TableView<TerrestrialObservationRow> table = tableBuilder.Table;
			table.getItems().clear();
			table.getItems().add(tableBuilder.EmptyRow);
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				congruentPointDialog.reset();
				tableBuilder.Table.getItems().clear();
				tableBuilder.Table.getItems().add(tableBuilder.EmptyRow);
				congruentPointDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) congruentPointDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return congruentPointDialog.dialog.showAndWait();
		}

		private void reset()
		{
			this.progressIndicatorPane.setVisible(false);
			this.settingPane.setDisable(false);
			this.okButton.setDisable(false);
			this.preventClosing = false;
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<Void>();
			this.dialog.setTitle(i18n.getString("CongruentPointDialog.title", "Congruent points"));
			this.dialog.setHeaderText(i18n.getString("CongruentPointDialog.header", "Find congruent points in project"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CLOSE);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
	//		this.dialog.initStyle(StageStyle.UTILITY);
			this.dialog.initOwner(window);
			this.dialog.setResizable(true);
			this.dialog.getDialogPane().getScene().getWindow().setOnCloseRequest(new EventHandlerAnonymousInnerClass(this));

			this.dialog.setOnCloseRequest(new EventHandlerAnonymousInnerClass2(this));

			this.okButton = (Button)this.dialog.getDialogPane().lookupButton(ButtonType.OK);
			this.okButton.addEventFilter(ActionEvent.ACTION, new CongruentPointEvent(this));

			this.settingPane = this.createSettingPane();
			this.progressIndicatorPane = this.createProgressIndicatorPane();
			this.progressIndicatorPane.setVisible(false);
			StackPane stackPane = new StackPane();
			stackPane.setAlignment(Pos.CENTER);
			stackPane.setMaxSize(Region.USE_PREF_SIZE, Region.USE_PREF_SIZE);
			stackPane.getChildren().addAll(this.settingPane, this.progressIndicatorPane);

			this.dialog.getDialogPane().setContent(stackPane);
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<WindowEvent>
		{
			private readonly CongruentPointDialog outerInstance;

			public EventHandlerAnonymousInnerClass(CongruentPointDialog outerInstance)
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
			private readonly CongruentPointDialog outerInstance;

			public EventHandlerAnonymousInnerClass2(CongruentPointDialog outerInstance)
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

		private Node createSettingPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMaxWidth(double.MaxValue);
			gridPane.setHgap(10);
			gridPane.setVgap(10);
			gridPane.setPadding(new Insets(5, 5, 5, 5)); // oben, recht, unten, links

			string labelSnap = i18n.getString("CongruentPointDialog.snap.label", "Snap distance:");
			string tooltipSnap = i18n.getString("CongruentPointDialog.snap.tooltip", "Set snap distance to find congruent points");

			string labelDimension = i18n.getString("CongruentPointDialog.dimension.label", "Point dimension:");
			string label1D = i18n.getString("CongruentPointDialog.dimension.1d.label", "1D");
			string label2D = i18n.getString("CongruentPointDialog.dimension.2d.label", "2D");
			string label3D = i18n.getString("CongruentPointDialog.dimension.3d.label", "3D");

			string tooltip1D = i18n.getString("CongruentPointDialog.dimension.1d.tooltip", "If checked, 1D points will be included into search");
			string tooltip2D = i18n.getString("CongruentPointDialog.dimension.2d.tooltip", "If checked, 2D points will be included into search");
			string tooltip3D = i18n.getString("CongruentPointDialog.dimension.3d.tooltip", "If checked, 3D points will be included into search");

			this.snapDistanceTextField = new DoubleTextField(0.15, CellValueType.LENGTH_RESIDUAL, true, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT);
			this.snapDistanceTextField.setTooltip(new Tooltip(tooltipSnap));
			this.snapDistanceTextField.setMinWidth(100);
			this.snapDistanceTextField.setMaxWidth(500);

			this.dimensionOneCheckBox = this.createCheckBox(label1D, tooltip1D);
			this.dimensionTwoCheckBox = this.createCheckBox(label2D, tooltip2D);
			this.dimensionThreeCheckBox = this.createCheckBox(label3D, tooltip3D);

			DimensionChangeListener dimensionChangeListener = new DimensionChangeListener(this);
			this.dimensionOneCheckBox.selectedProperty().addListener(dimensionChangeListener);
			this.dimensionTwoCheckBox.selectedProperty().addListener(dimensionChangeListener);
			this.dimensionThreeCheckBox.selectedProperty().addListener(dimensionChangeListener);

			this.dimensionTwoCheckBox.setSelected(true);
			this.dimensionThreeCheckBox.setSelected(true);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TableView<?> table = tableBuilder.getTable();
			TableView<object> table = tableBuilder.Table;
			table.setMaxWidth(double.MaxValue);

			Label snapLabel = new Label(labelSnap);
			snapLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label dimensionLabel = new Label(labelDimension);
			dimensionLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			gridPane.add(snapLabel, 0, 0);
			gridPane.add(this.snapDistanceTextField, 1, 0, 4, 1);
			gridPane.add(dimensionLabel, 0, 1);
			gridPane.add(this.dimensionOneCheckBox, 1, 1);
			gridPane.add(this.dimensionTwoCheckBox, 2, 1);
			gridPane.add(this.dimensionThreeCheckBox, 3, 1);

			VBox vbox = new VBox();
			vbox.setSpacing(10);
			vbox.setPadding(new Insets(5, 5, 5, 5)); // oben, recht, unten, links
			vbox.getChildren().setAll(gridPane, table);
			vbox.setPrefHeight(300);

			return vbox;
		}

		private Node createProgressIndicatorPane()
		{
			VBox box = new VBox();
			box.setAlignment(Pos.CENTER);
			box.getChildren().setAll(this.progressIndicator);
			return box;
		}

		private void process()
		{
			this.reset();
			this.processSQLTask = new ProcessSQLTask(this);
			this.processSQLTask.setOnSucceeded(new EventHandlerAnonymousInnerClass3(this));

			this.processSQLTask.setOnFailed(new EventHandlerAnonymousInnerClass4(this));

			Thread th = new Thread(this.processSQLTask);
			th.setDaemon(true);
			th.Start();
		}

		private class EventHandlerAnonymousInnerClass3 : EventHandler<WorkerStateEvent>
		{
			private readonly CongruentPointDialog outerInstance;

			public EventHandlerAnonymousInnerClass3(CongruentPointDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.preventClosing = false;
				IList<TerrestrialObservationRow> rows = outerInstance.processSQLTask.getValue();
				TableView<TerrestrialObservationRow> table = tableBuilder.Table;
				if (rows != null && rows.Count > 0)
				{
					table.getItems().setAll(rows);
					table.sort();
				}
				else
				{
					table.getItems().setAll(tableBuilder.EmptyRow);
				}
			}
		}

		private class EventHandlerAnonymousInnerClass4 : EventHandler<WorkerStateEvent>
		{
			private readonly CongruentPointDialog outerInstance;

			public EventHandlerAnonymousInnerClass4(CongruentPointDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.preventClosing = false;
				outerInstance.dialog.hide();
				Exception throwable = outerInstance.processSQLTask.getException();
				if (throwable != null)
				{
					Platform.runLater(() =>
					{
					OptionDialog.showThrowableDialog(outerInstance.i18n.getString("CongruentPointDialog.message.error.request.exception.title", "Unexpected SQL-Error"), outerInstance.i18n.getString("CongruentPointDialog.message.error.request.exception.header", "Error, database request is failed."), outerInstance.i18n.getString("CongruentPointDialog.message.error.request.exception.message", "An exception has occurred during database transaction."), throwable);
					});
					Console.WriteLine(throwable.ToString());
					Console.Write(throwable.StackTrace);
				}
				UITreeBuilder.Instance.Tree.getSelectionModel().select(0);
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
			checkBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			checkBox.setMaxWidth(double.MaxValue);
			return checkBox;
		}
	}

}