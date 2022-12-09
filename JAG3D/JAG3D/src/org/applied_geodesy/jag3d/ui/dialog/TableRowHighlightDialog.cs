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

namespace org.applied_geodesy.jag3d.ui.dialog
{

	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using DefaultTableRowHighlightValue = org.applied_geodesy.jag3d.ui.table.rowhighlight.DefaultTableRowHighlightValue;
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightRangeType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DoubleTextField = org.applied_geodesy.ui.textfield.DoubleTextField;
	using MinMaxDoubleTextField = org.applied_geodesy.ui.textfield.MinMaxDoubleTextField;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterEventType = org.applied_geodesy.util.FormatterEventType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using Accordion = javafx.scene.control.Accordion;
	using ButtonType = javafx.scene.control.ButtonType;
	using ColorPicker = javafx.scene.control.ColorPicker;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using DialogEvent = javafx.scene.control.DialogEvent;
	using Label = javafx.scene.control.Label;
	using RadioButton = javafx.scene.control.RadioButton;
	using TitledPane = javafx.scene.control.TitledPane;
	using Toggle = javafx.scene.control.Toggle;
	using ToggleGroup = javafx.scene.control.ToggleGroup;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Color = javafx.scene.paint.Color;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;

	public class TableRowHighlightDialog : FormatterChangedListener
	{

		private class HighlightRangeChangeListener : ChangeListener<double>
		{
			private readonly TableRowHighlightDialog outerInstance;

			internal readonly TableRowHighlightType tableRowHighlightType;

			public HighlightRangeChangeListener(TableRowHighlightDialog outerInstance, TableRowHighlightType tableRowHighlightType)
			{
				this.outerInstance = outerInstance;
				this.tableRowHighlightType = tableRowHighlightType;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (!outerInstance.ignoreChangeEvent && newValue != null && outerInstance.highlightRangeFieldMap.ContainsKey(this.tableRowHighlightType))
				{
					TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;

					if (outerInstance.highlightRangeFieldMap[this.tableRowHighlightType] != null)
					{
						DoubleTextField[] fields = outerInstance.highlightRangeFieldMap[tableRowHighlightType];
						double leftBoundary = (fields[0].Number).Value;
						double rightBoundary = (fields[1].Number).Value;
						tableRowHighlight.SelectedTableRowHighlightType = this.tableRowHighlightType;
						tableRowHighlight.setRange(this.tableRowHighlightType, leftBoundary, rightBoundary);
					}

					outerInstance.ignoreChangeEvent = true;
					try
					{
						foreach (Toggle toggleButton in outerInstance.highlightOptionGroup.getToggles())
						{
							if (toggleButton.getUserData() == this.tableRowHighlightType && toggleButton is RadioButton)
							{
								RadioButton radioButton = (RadioButton)toggleButton;
								radioButton.setSelected(true);
								break;
							}
						}
					}
					finally
					{
						outerInstance.ignoreChangeEvent = false;
					}
					outerInstance.save();
				}
			}
		}

		private class HighlightTypeChangeListener : ChangeListener<Toggle>
		{
			private readonly TableRowHighlightDialog outerInstance;

			public HighlightTypeChangeListener(TableRowHighlightDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Toggle oldValue, Toggle newValue) where T1 : javafx.scene.control.Toggle
			{
				if (!outerInstance.ignoreChangeEvent && newValue != null && newValue.isSelected() && newValue.getUserData() != null && newValue.getUserData() is TableRowHighlightType && newValue is RadioButton)
				{
					TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
					TableRowHighlightType tableRowHighlightType = (TableRowHighlightType)newValue.getUserData();
					tableRowHighlight.SelectedTableRowHighlightType = tableRowHighlightType;

					if (outerInstance.highlightRangeFieldMap.ContainsKey(tableRowHighlightType) && outerInstance.highlightRangeFieldMap[tableRowHighlightType] != null)
					{
						DoubleTextField[] fields = outerInstance.highlightRangeFieldMap[tableRowHighlightType];
						double leftBoundary = (fields[0].Number).Value;
						double rightBoundary = (fields[1].Number).Value;
						tableRowHighlight.setRange(tableRowHighlightType, leftBoundary, rightBoundary);
					}
					outerInstance.save();
				}
			}
		}

		private class HighlightColorChangeListener : ChangeListener<Color>
		{
			private readonly TableRowHighlightDialog outerInstance;

			internal readonly TableRowHighlightRangeType tableRowHighlightRangeType;

			public HighlightColorChangeListener(TableRowHighlightDialog outerInstance, TableRowHighlightRangeType tableRowHighlightRangeType)
			{
				this.outerInstance = outerInstance;
				this.tableRowHighlightRangeType = tableRowHighlightRangeType;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Color oldValue, Color newValue) where T1 : javafx.scene.paint.Color
			{
				if (!outerInstance.ignoreChangeEvent && newValue != null)
				{
					TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
					tableRowHighlight.setColor(this.tableRowHighlightRangeType, newValue);
					outerInstance.save();
				}
			}
		}

		private I18N i18n = I18N.Instance;
		private FormatterOptions options = FormatterOptions.Instance;
		private static TableRowHighlightDialog tableRowHighlightDialog = new TableRowHighlightDialog();
		private Dialog<TableRowHighlight> dialog = null;
		private Window window;
		private IDictionary<TableRowHighlightType, DoubleTextField[]> highlightRangeFieldMap = new Dictionary<TableRowHighlightType, DoubleTextField[]>(5);

		private bool ignoreChangeEvent = false;
		private ToggleGroup highlightOptionGroup = new ToggleGroup();
		private ColorPicker excellentColorPicker;
		private ColorPicker satisfactoryColorPicker;
		private ColorPicker inadequateColorPicker;

		private Accordion accordion = new Accordion();
		private Label rightBoundaryRedundancy;
		private Label rightBoundaryPValue;

		private TableRowHighlightDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				tableRowHighlightDialog.window = value;
			}
		}

		public static Optional<TableRowHighlight> showAndWait()
		{
			tableRowHighlightDialog.init();
			tableRowHighlightDialog.load();
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				tableRowHighlightDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) tableRowHighlightDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return tableRowHighlightDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.ignoreChangeEvent = true;
			try
			{
				this.options.addFormatterChangedListener(this);

				this.dialog = new Dialog<TableRowHighlight>();
				this.dialog.setTitle(i18n.getString("TableRowHighlightDialog.title", "Highlighting table rows"));
				this.dialog.setHeaderText(i18n.getString("TableRowHighlightDialog.header", "Highlighting of specific table rows"));
				this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.CLOSE);
				this.dialog.initModality(Modality.APPLICATION_MODAL);
				this.dialog.initOwner(window);
				this.dialog.setResizable(true);

				this.dialog.initModality(Modality.APPLICATION_MODAL);
				this.dialog.initOwner(window);
				this.dialog.getDialogPane().setContent(this.createPane());
				this.dialog.setResizable(true);
				this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));

				this.dialog.setOnCloseRequest(new EventHandlerAnonymousInnerClass(this));
			}
			finally
			{
				this.ignoreChangeEvent = false;
			}
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, TableRowHighlight>
		{
			private readonly TableRowHighlightDialog outerInstance;

			public CallbackAnonymousInnerClass(TableRowHighlightDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override TableRowHighlight call(ButtonType buttonType)
			{
				return TableRowHighlight.Instance;
			}
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<DialogEvent>
		{
			private readonly TableRowHighlightDialog outerInstance;

			public EventHandlerAnonymousInnerClass(TableRowHighlightDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(DialogEvent @event)
			{
				outerInstance.accordion.setExpandedPane(outerInstance.accordion.getPanes().get(0));
			}
		}

		private Node createPane()
		{
			this.accordion.getPanes().addAll((TitledPane)this.createRangePane(), (TitledPane)this.createColorPropertiesPane());
			this.accordion.setExpandedPane(accordion.getPanes().get(0));
			this.accordion.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.accordion.setMaxSize(double.MaxValue, double.MaxValue);

			if (highlightOptionGroup.getSelectedToggle() is RadioButton)
			{
				((RadioButton)highlightOptionGroup.getSelectedToggle()).requestFocus();
			}

			return accordion;
		}

		private Node createRangePane()
		{
			GridPane gridPane = this.createGridPane();

			int row = 0;
			this.addRow(gridPane, row++, TableRowHighlightType.NONE, this.highlightOptionGroup);
			this.addRow(gridPane, row++, TableRowHighlightType.TEST_STATISTIC, this.highlightOptionGroup);
			this.addRow(gridPane, row++, TableRowHighlightType.GROSS_ERROR, this.highlightOptionGroup);
			this.addRow(gridPane, row++, TableRowHighlightType.REDUNDANCY, this.highlightOptionGroup);
			this.addRow(gridPane, row++, TableRowHighlightType.P_PRIO_VALUE, this.highlightOptionGroup);
			this.addRow(gridPane, row++, TableRowHighlightType.INFLUENCE_ON_POSITION, this.highlightOptionGroup);

			this.highlightOptionGroup.selectedToggleProperty().addListener(new HighlightTypeChangeListener(this));

			TitledPane titledPane = this.createTitledPane(i18n.getString("TableRowHighlightDialog.range.label", "Range options"), i18n.getString("TableRowHighlightDialog.range.tooltip", "Specify range of table row highlighting"), gridPane);

			return titledPane;
		}

		private Node createColorPropertiesPane()
		{
			GridPane gridPane = this.createGridPane();

			this.excellentColorPicker = this.createColorPicker(TableRowHighlightRangeType.EXCELLENT, DefaultTableRowHighlightValue.getColor(TableRowHighlightRangeType.EXCELLENT));
			this.satisfactoryColorPicker = this.createColorPicker(TableRowHighlightRangeType.SATISFACTORY, DefaultTableRowHighlightValue.getColor(TableRowHighlightRangeType.SATISFACTORY));
			this.inadequateColorPicker = this.createColorPicker(TableRowHighlightRangeType.INADEQUATE, DefaultTableRowHighlightValue.getColor(TableRowHighlightRangeType.INADEQUATE));

			Label excellentColorLabel = new Label(i18n.getString("TableRowHighlightDialog.color.excellent.label", "Excellent color:"));
			Label satisfactoryColorLabel = new Label(i18n.getString("TableRowHighlightDialog.color.satisfactory.label", "Satisfactory color:"));
			Label inadequateColorLabel = new Label(i18n.getString("TableRowHighlightDialog.color.inadequate.label", "Inadequate color:"));

			excellentColorLabel.setLabelFor(this.excellentColorPicker);
			satisfactoryColorLabel.setLabelFor(this.satisfactoryColorPicker);
			inadequateColorLabel.setLabelFor(this.inadequateColorPicker);

			GridPane.setHgrow(excellentColorLabel, Priority.NEVER);
			GridPane.setHgrow(satisfactoryColorLabel, Priority.NEVER);
			GridPane.setHgrow(inadequateColorLabel, Priority.NEVER);

			GridPane.setHgrow(this.excellentColorPicker, Priority.ALWAYS);
			GridPane.setHgrow(this.satisfactoryColorPicker, Priority.ALWAYS);
			GridPane.setHgrow(this.inadequateColorPicker, Priority.ALWAYS);

			int row = 0;
			gridPane.add(excellentColorLabel, 0, row);
			gridPane.add(this.excellentColorPicker, 1, row++);

			gridPane.add(satisfactoryColorLabel, 0, row);
			gridPane.add(this.satisfactoryColorPicker, 1, row++);

			gridPane.add(inadequateColorLabel, 0, row);
			gridPane.add(this.inadequateColorPicker, 1, row++);

			TitledPane titledPane = this.createTitledPane(i18n.getString("TableRowHighlightDialog.color.label", "Color options"), i18n.getString("TableRowHighlightDialog.color.tooltip", "Specify color properties of table row highlighting"), gridPane);

			return titledPane;
		}

		private void addRow(GridPane parent, int row, TableRowHighlightType tableRowHighlightType, ToggleGroup group)
		{
			string radioButtonLabelText = null, radioButtonToolTipText = null;
			Label leftBoundary = null, middleBoundaray = null, rightBoundary = null;

			TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
			DoubleTextField leftRangeField = null, rightRangeField = null;

			switch (tableRowHighlightType.innerEnumValue)
			{
			case TableRowHighlightType.InnerEnum.NONE:
				radioButtonLabelText = i18n.getString("TableRowHighlightDialog.range.none.label", "None highlighting");
				radioButtonToolTipText = i18n.getString("TableRowHighlightDialog.range.none.tooltip", "None specific highlighting of table rows");
				break;

			case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
				radioButtonLabelText = i18n.getString("TableRowHighlightDialog.range.test_statistic.label", "Test statistic Tprio \u2228 Tpost");
				radioButtonToolTipText = i18n.getString("TableRowHighlightDialog.range.test_statistic.tooltip", "Highlighting table rows depending on test statistic decisions");
				break;

			case TableRowHighlightType.InnerEnum.GROSS_ERROR:
				radioButtonLabelText = i18n.getString("TableRowHighlightDialog.range.gross_error.label", "Gross error \u2207(1) \u2264 \u2207 \u2264 \u2207(\u03bb)");
				radioButtonToolTipText = i18n.getString("TableRowHighlightDialog.range.gross_error.tooltip", "Highlighting table rows depending on estimated gross error");
				break;

			case TableRowHighlightType.InnerEnum.REDUNDANCY:
				radioButtonLabelText = String.format(Locale.ENGLISH, "%s %s", i18n.getString("TableRowHighlightDialog.range.redundancy.label", "Redundancy r"), options.FormatterOptions[CellValueType.PERCENTAGE].getUnit().toFormattedAbbreviation()).Trim();
				radioButtonToolTipText = i18n.getString("TableRowHighlightDialog.range.redundancy.tooltip", "Highlighting table rows depending on redundancy");
				leftBoundary = new Label("0 \u2264 ");
				middleBoundaray = new Label(" \u003C ");
				rightBoundary = new Label(" \u2264 " + (long)Math.Round(options.convertPercentToView(1), MidpointRounding.AwayFromZero));

				leftRangeField = this.createMinMaxDoubleTextField(tableRowHighlightType, tableRowHighlight.getLeftBoundary(tableRowHighlightType), 0, 1, CellValueType.PERCENTAGE, false, false);
				rightRangeField = this.createMinMaxDoubleTextField(tableRowHighlightType, tableRowHighlight.getRightBoundary(tableRowHighlightType), 0, 1, CellValueType.PERCENTAGE, false, false);
				this.rightBoundaryRedundancy = rightBoundary;
				break;

			case TableRowHighlightType.InnerEnum.P_PRIO_VALUE:
				radioButtonLabelText = String.format(Locale.ENGLISH, "%s %s", i18n.getString("TableRowHighlightDialog.range.probability_value.label", "Probability value p"), options.FormatterOptions[CellValueType.PERCENTAGE].getUnit().toFormattedAbbreviation()).Trim();
				radioButtonToolTipText = i18n.getString("TableRowHighlightDialog.range.probability_value.tooltip", "Highlighting table rows depending on (a-priori) p-value");
				leftBoundary = new Label("0 \u003C ");
				middleBoundaray = new Label(" \u003C ");
				rightBoundary = new Label(" \u003C " + (long)Math.Round(options.convertPercentToView(1), MidpointRounding.AwayFromZero));

				leftRangeField = this.createMinMaxDoubleTextField(tableRowHighlightType, tableRowHighlight.getLeftBoundary(tableRowHighlightType), 0, 1, CellValueType.PERCENTAGE, true, true);
				rightRangeField = this.createMinMaxDoubleTextField(tableRowHighlightType, tableRowHighlight.getRightBoundary(tableRowHighlightType), 0, 1, CellValueType.PERCENTAGE, true, true);
				this.rightBoundaryPValue = rightBoundary;
				break;

			case TableRowHighlightType.InnerEnum.INFLUENCE_ON_POSITION:
				radioButtonLabelText = String.format(Locale.ENGLISH, "%s %s", i18n.getString("TableRowHighlightDialog.range.influenceonposition.label", "Influence on point position EP"), options.FormatterOptions[CellValueType.LENGTH_RESIDUAL].getUnit().toFormattedAbbreviation()).Trim();
				radioButtonToolTipText = i18n.getString("TableRowHighlightDialog.range.influenceonposition.tooltip", "Highlighting table rows depending on influence on point position due to an undetected gross-error");
				leftBoundary = new Label("0 \u2264 ");
				middleBoundaray = new Label(" \u003C ");
				rightBoundary = new Label(" \u003C \u221E");

				leftRangeField = this.createMinMaxDoubleTextField(tableRowHighlightType, tableRowHighlight.getLeftBoundary(tableRowHighlightType), 0, double.PositiveInfinity, CellValueType.LENGTH_RESIDUAL, false, false);
				rightRangeField = this.createMinMaxDoubleTextField(tableRowHighlightType, tableRowHighlight.getRightBoundary(tableRowHighlightType), 0, double.PositiveInfinity, CellValueType.LENGTH_RESIDUAL, false, false);
				break;

			}

			if (tableRowHighlightType != null && !string.ReferenceEquals(radioButtonLabelText, null) && !string.ReferenceEquals(radioButtonToolTipText, null))
			{
				int column = 0;

				RadioButton radioButton = this.createRadioButton(radioButtonLabelText, radioButtonToolTipText, tableRowHighlightType, group);
				radioButton.setSelected(tableRowHighlightType == TableRowHighlightType.NONE);

				GridPane.setMargin(radioButton, new Insets(0,10,0,0));
				GridPane.setHgrow(radioButton, Priority.SOMETIMES);
				parent.add(radioButton, column++, row);

				if (leftRangeField != null && rightRangeField != null && leftBoundary != null && middleBoundaray != null && rightBoundary != null)
				{
					GridPane.setHgrow(leftBoundary, Priority.NEVER);
					GridPane.setHgrow(middleBoundaray, Priority.NEVER);
					GridPane.setHgrow(rightBoundary, Priority.NEVER);
					GridPane.setHgrow(leftRangeField, Priority.ALWAYS);
					GridPane.setHgrow(rightRangeField, Priority.ALWAYS);

					parent.add(leftBoundary, column++, row);
					parent.add(leftRangeField, column++, row);
					parent.add(middleBoundaray, column++, row);
					parent.add(rightRangeField, column++, row);
					parent.add(rightBoundary, column++, row);

					this.highlightRangeFieldMap[tableRowHighlightType] = new DoubleTextField[] {leftRangeField, rightRangeField};
				}
			}
		}

		private void load()
		{
			this.ignoreChangeEvent = true;

			TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;

			try
			{
				SQLManager.Instance.loadTableRowHighlight();

				TableRowHighlightType selectedTableRowHighlightType = tableRowHighlight.SelectedTableRowHighlightType;

				foreach (Toggle toggleButton in this.highlightOptionGroup.getToggles())
				{
					if (toggleButton.getUserData() == selectedTableRowHighlightType && toggleButton is RadioButton)
					{
						RadioButton radioButton = (RadioButton)toggleButton;
						radioButton.setSelected(true);
						radioButton.requestFocus();
						break;
					}
				}

				foreach (TableRowHighlightType tableRowHighlightType in TableRowHighlightType.values())
				{
					if (tableRowHighlightType != TableRowHighlightType.NONE && this.highlightRangeFieldMap.ContainsKey(tableRowHighlightType) && this.highlightRangeFieldMap[tableRowHighlightType] != null)
					{
						DoubleTextField[] fields = this.highlightRangeFieldMap[tableRowHighlightType];
						double leftBoundary = tableRowHighlight.getLeftBoundary(tableRowHighlightType);
						double rightBoundary = tableRowHighlight.getRightBoundary(tableRowHighlightType);

						fields[0].Number = leftBoundary;
						fields[1].Number = rightBoundary;
					}
				}

				this.excellentColorPicker.setValue(tableRowHighlight.getColor(TableRowHighlightRangeType.EXCELLENT));
				this.satisfactoryColorPicker.setValue(tableRowHighlight.getColor(TableRowHighlightRangeType.SATISFACTORY));
				this.inadequateColorPicker.setValue(tableRowHighlight.getColor(TableRowHighlightRangeType.INADEQUATE));

				Platform.runLater(() =>
				{
				if (highlightOptionGroup.getSelectedToggle() is RadioButton)
				{
					((RadioButton)highlightOptionGroup.getSelectedToggle()).requestFocus();
				}
				});
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("TableRowHighlightDialog.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("TableRowHighlightDialog.message.error.load.exception.header", "Error, could not load table row highlight properties from database."), i18n.getString("TableRowHighlightDialog.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
			finally
			{
				tableRowHighlight.refreshTables();
				this.ignoreChangeEvent = false;
			}
		}

		private void save()
		{
			try
			{
				SQLManager.Instance.saveTableRowHighlight();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("TableRowHighlightDialog.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("TableRowHighlightDialog.message.error.save.exception.header", "Error, could not save table row highlight properties to database."), i18n.getString("TableRowHighlightDialog.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
			finally
			{
				TableRowHighlight.Instance.refreshTables();
			}
		}

		private RadioButton createRadioButton(string title, string tooltip, TableRowHighlightType tableRowHighlightType, ToggleGroup group)
		{
			Label label = new Label(title);
			label.setPadding(new Insets(0,0,0,3));
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			RadioButton radioButton = new RadioButton();
			radioButton.setGraphic(label);
			radioButton.setTooltip(new Tooltip(tooltip));
			radioButton.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			radioButton.setMaxWidth(double.MaxValue);
			radioButton.setUserData(tableRowHighlightType);
			radioButton.setToggleGroup(group);
			return radioButton;
		}

		private DoubleTextField createMinMaxDoubleTextField(TableRowHighlightType tableRowHighlightType, double value, double min, double max, CellValueType valueType, bool exclusiveMin, bool exclusiveMax)
		{
			MinMaxDoubleTextField doubleTextField = new MinMaxDoubleTextField(value, min, max, valueType, false, exclusiveMin, exclusiveMax);
			doubleTextField.setPrefColumnCount(3);
			doubleTextField.setMaxWidth(double.MaxValue);
			doubleTextField.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			doubleTextField.setUserData(tableRowHighlightType);
			doubleTextField.numberProperty().addListener(new HighlightRangeChangeListener(this, tableRowHighlightType));
			return doubleTextField;
		}

		private GridPane createGridPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			gridPane.setMaxSize(double.MaxValue,double.MaxValue);
			gridPane.setAlignment(Pos.TOP_CENTER);
			gridPane.setHgap(5);
			gridPane.setVgap(7);
			gridPane.setPadding(new Insets(7, 25, 0, 25)); // oben, recht, unten, links
			return gridPane;
		}

		private TitledPane createTitledPane(string title, string tooltip, Node content)
		{
			Label label = new Label(title);
			label.setTooltip(new Tooltip(tooltip));
			TitledPane titledPane = new TitledPane();
			titledPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			titledPane.setMaxSize(double.MaxValue,double.MaxValue);
			titledPane.setGraphic(label);
			titledPane.setCollapsible(true);
			titledPane.setAnimated(true);
			titledPane.setContent(content);
			titledPane.setPadding(new Insets(5, 5, 5, 5)); // oben, links, unten, rechts
			return titledPane;
		}

		private ColorPicker createColorPicker(TableRowHighlightRangeType tableRowHighlightRangeType, Color color)
		{
			ColorPicker colorPicker = new ColorPicker(color);
			colorPicker.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			colorPicker.setMaxWidth(double.MaxValue);
			colorPicker.getStyleClass().add("split-button");
			colorPicker.valueProperty().addListener(new HighlightColorChangeListener(this, tableRowHighlightRangeType));
			colorPicker.setUserData(tableRowHighlightRangeType);
			return colorPicker;
		}

		public virtual void formatterChanged(FormatterEvent evt)
		{
			if (evt.EventType == FormatterEventType.UNIT_CHANGED)
			{
				if (evt.CellType == CellValueType.PERCENTAGE)
				{
					this.rightBoundaryRedundancy.setText(" \u2264 " + (long)Math.Round(options.convertPercentToView(1), MidpointRounding.AwayFromZero));
					this.rightBoundaryPValue.setText(" \u003C " + (long)Math.Round(options.convertPercentToView(1), MidpointRounding.AwayFromZero));
				}

				foreach (Toggle toggleButton in this.highlightOptionGroup.getToggles())
				{
					if ((evt.CellType == CellValueType.LENGTH_RESIDUAL || evt.CellType == CellValueType.PERCENTAGE) && (toggleButton.getUserData() == TableRowHighlightType.INFLUENCE_ON_POSITION || toggleButton.getUserData() == TableRowHighlightType.P_PRIO_VALUE || toggleButton.getUserData() == TableRowHighlightType.REDUNDANCY) && toggleButton is RadioButton)
					{

						RadioButton radioButton = (RadioButton)toggleButton;

						string labelText = null;
						string unitAbbr = null;
						if (evt.CellType == CellValueType.LENGTH_RESIDUAL && toggleButton.getUserData() == TableRowHighlightType.INFLUENCE_ON_POSITION)
						{
							labelText = i18n.getString("TableRowHighlightDialog.range.influenceonposition.label", "Influence on point position");
							unitAbbr = options.FormatterOptions[CellValueType.LENGTH_RESIDUAL].getUnit().toFormattedAbbreviation();
						}
						else if (evt.CellType == CellValueType.PERCENTAGE && toggleButton.getUserData() == TableRowHighlightType.P_PRIO_VALUE)
						{
							labelText = i18n.getString("TableRowHighlightDialog.range.probability_value.label", "Probability value p");
							unitAbbr = options.FormatterOptions[CellValueType.PERCENTAGE].getUnit().toFormattedAbbreviation();
						}
						else if (evt.CellType == CellValueType.PERCENTAGE && toggleButton.getUserData() == TableRowHighlightType.REDUNDANCY)
						{
							labelText = i18n.getString("TableRowHighlightDialog.range.redundancy.label", "Redundancy r");
							unitAbbr = options.FormatterOptions[CellValueType.PERCENTAGE].getUnit().toFormattedAbbreviation();
						}
						else
						{
							continue;
						}

						if (radioButton.getGraphic() is Label)
						{
							((Label)radioButton.getGraphic()).setText(String.format(Locale.ENGLISH, "%s %s", labelText, unitAbbr).Trim());
						}
						else
						{
							radioButton.setText(String.format(Locale.ENGLISH, "%s %s", labelText, unitAbbr).Trim());
						}
					}
				}
			}
		}
	}

}