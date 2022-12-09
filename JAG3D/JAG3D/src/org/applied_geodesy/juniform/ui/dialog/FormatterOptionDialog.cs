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

namespace org.applied_geodesy.juniform.ui.dialog
{

	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using FormatterOption = org.applied_geodesy.util.FormatterOptions.FormatterOption;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;
	using AngleUnit = org.applied_geodesy.util.unit.AngleUnit;
	using LengthUnit = org.applied_geodesy.util.unit.LengthUnit;
	using PercentUnit = org.applied_geodesy.util.unit.PercentUnit;
	using ScaleUnit = org.applied_geodesy.util.unit.ScaleUnit;
	using Unit = org.applied_geodesy.util.unit.Unit;
	using UnitType = org.applied_geodesy.util.unit.UnitType;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Accordion = javafx.scene.control.Accordion;
	using ButtonType = javafx.scene.control.ButtonType;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using DialogEvent = javafx.scene.control.DialogEvent;
	using Label = javafx.scene.control.Label;
	using Spinner = javafx.scene.control.Spinner;
	using SpinnerValueFactory = javafx.scene.control.SpinnerValueFactory;
	using TextFormatter = javafx.scene.control.TextFormatter;
	using TitledPane = javafx.scene.control.TitledPane;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using StringConverter = javafx.util.StringConverter;

	public class FormatterOptionDialog
	{

		private class DigitsChangeListener : ChangeListener<int>
		{
			private readonly FormatterOptionDialog outerInstance;

			internal CellValueType type;
			internal DigitsChangeListener(FormatterOptionDialog outerInstance, CellValueType type)
			{
				this.outerInstance = outerInstance;
				this.type = type;
			}

			public override void changed<T1>(ObservableValue<T1> observable, int? oldValue, int? newValue) where T1 : int
			{
				if (!outerInstance.ignoreEvent)
				{
					FormatterOptions.FormatterOption option = outerInstance.options.FormatterOptions[type];
					option.FractionDigits = newValue.Value;
				}
			}
		}

		private class UnitChangeListener<T> : ChangeListener<T> where T : org.applied_geodesy.util.unit.Unit
		{
			private readonly FormatterOptionDialog outerInstance;

			internal CellValueType type;

			internal UnitChangeListener(FormatterOptionDialog outerInstance, CellValueType type)
			{
				this.outerInstance = outerInstance;
				this.type = type;
			}

			public override void changed<T1>(ObservableValue<T1> observable, T oldValue, T newValue) where T1 : T
			{
				if (!outerInstance.ignoreEvent)
				{
					FormatterOptions.FormatterOption option = outerInstance.options.FormatterOptions[type];
					option.Unit = newValue;
				}
			}
		}

		private IDictionary<CellValueType, ComboBox<Unit>> unitComboBoxes = new Dictionary<CellValueType, ComboBox<Unit>>();
		private IDictionary<CellValueType, Spinner<int>> digitsSpinners = new Dictionary<CellValueType, Spinner<int>>();
		private FormatterOptions options = FormatterOptions.Instance;
		private I18N i18n = I18N.Instance;
		private static FormatterOptionDialog formatterOptionDialog = new FormatterOptionDialog();
		private Dialog<Void> dialog = null;
		private Accordion accordion = null;
		private bool ignoreEvent = false;
		private Window window;

		private FormatterOptionDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				formatterOptionDialog.window = value;
			}
		}

		public static Optional<Void> showAndWait()
		{
			formatterOptionDialog.init();
			formatterOptionDialog.load();
			formatterOptionDialog.accordion.setExpandedPane(formatterOptionDialog.accordion.getPanes().get(0));
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				formatterOptionDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) formatterOptionDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return formatterOptionDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<Void>();
			this.dialog.setTitle(i18n.getString("FormatterOptionDialog.title", "Preferences"));
			this.dialog.setHeaderText(i18n.getString("FormatterOptionDialog.header", "Formatter options and unit preferences"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);

			this.accordion = new Accordion();
			this.accordion.getPanes().addAll(this.createFormatterOptionValuesPane(), this.createFormatterOptionUncertaintiesPane(), this.createFormatterOptionResidualsPane(), this.createFormatterOptionStatisticsPane());

			this.dialog.getDialogPane().setContent(this.accordion);
			this.dialog.setResizable(true);

			this.dialog.setOnCloseRequest(new EventHandlerAnonymousInnerClass(this));

			Platform.runLater(() =>
			{
			digitsSpinners[CellValueType.LENGTH].requestFocus();
			});
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<DialogEvent>
		{
			private readonly FormatterOptionDialog outerInstance;

			public EventHandlerAnonymousInnerClass(FormatterOptionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(DialogEvent @event)
			{
				outerInstance.accordion.setExpandedPane(outerInstance.accordion.getPanes().get(0));
			}
		}

		private void load()
		{
			try
			{
				this.ignoreEvent = true;
				foreach (FormatterOptions.FormatterOption option in options.FormatterOptions.Values)
				{
					CellValueType type = option.Type;
					if (this.digitsSpinners.ContainsKey(type))
					{
						SpinnerValueFactory.IntegerSpinnerValueFactory valueFactory = (SpinnerValueFactory.IntegerSpinnerValueFactory)this.digitsSpinners[type].getValueFactory();
						int digits = option.FractionDigits;
						digits = Math.Max(Math.Min(digits, valueFactory.getMax()), valueFactory.getMin());
						this.digitsSpinners[type].getValueFactory().setValue(digits);
					}
					if (this.unitComboBoxes.ContainsKey(type))
					{
						this.unitComboBoxes[type].getSelectionModel().select(option.Unit);
					}
				}
			}
			finally
			{
				this.ignoreEvent = false;
			}
		}

		private TitledPane createFormatterOptionValuesPane()
		{
			string title = i18n.getString("FormatterOptionDialog.value.title", "Values");
			string tooltip = i18n.getString("FormatterOptionDialog.value.tooltip", "Preferences for measurement values");
			GridPane gridPane = DialogUtil.createGridPane();

			IDictionary<CellValueType, FormatterOptions.FormatterOption> formatterOptions = options.FormatterOptions;
			int row = 0;
			this.addRow(i18n.getString("FormatterOptionDialog.unit.length.value.label", "Length:"), i18n.getString("FormatterOptionDialog.unit.length.value.tooltip", "Set number of fraction digits for type length"), i18n.getString("FormatterOptionDialog.unit.length.value.tooltip.unit", "Set unit for type length value"), formatterOptions[CellValueType.LENGTH], gridPane, ++row);

			this.addRow(i18n.getString("FormatterOptionDialog.unit.angle.value.label", "Angle:"), i18n.getString("FormatterOptionDialog.unit.angle.value.tooltip.digits", "Set number of fraction digits for type angle"), i18n.getString("FormatterOptionDialog.unit.angle.value.tooltip.unit", "Set unit for type angle value"), formatterOptions[CellValueType.ANGLE], gridPane, ++row);

	//		this.addRow(
	//				i18n.getString("FormatterOptionDialog.unit.scale.value.label", "Scale:"),
	//    			i18n.getString("FormatterOptionDialog.unit.scale.value.tooltip.digits", "Set number of fraction digits for type scale"),
	//    			i18n.getString("FormatterOptionDialog.unit.scale.value.tooltip.unit", "Set unit for type scale value"),
	//				formatterOptions.get(CellValueType.SCALE),
	//				gridPane,
	//				++row);

			this.addRow(i18n.getString("FormatterOptionDialog.unit.vector.value.label", "Vector:"), i18n.getString("FormatterOptionDialog.unit.vector.value.tooltip.digits", "Set number of fraction digits for type vector"), i18n.getString("FormatterOptionDialog.unit.vector.value.tooltip.unit", "Set unit for type vector value"), formatterOptions[CellValueType.VECTOR], gridPane, ++row);

			Region spacer = new Region();
			GridPane.setVgrow(spacer, Priority.ALWAYS);
			gridPane.add(spacer, 1, ++row, 3, 1);

			return DialogUtil.createTitledPane(title, tooltip, gridPane);
		}

		private TitledPane createFormatterOptionUncertaintiesPane()
		{
			string title = i18n.getString("FormatterOptionDialog.uncertainty.title", "Uncertainties");
			string tooltip = i18n.getString("FormatterOptionDialog.uncertainty.tooltip", "Preferences for uncertainties");
			GridPane gridPane = DialogUtil.createGridPane();

			IDictionary<CellValueType, FormatterOptions.FormatterOption> formatterOptions = options.FormatterOptions;
			int row = 0;
			this.addRow(i18n.getString("FormatterOptionDialog.unit.length.uncertainty.label", "Length uncertainty:"), i18n.getString("FormatterOptionDialog.unit.length.uncertainty.tooltip.digits", "Set number of fraction digits for type length uncertainty"), i18n.getString("FormatterOptionDialog.unit.length.uncertainty.tooltip.unit", "Set unit for type length uncertainty"), formatterOptions[CellValueType.LENGTH_UNCERTAINTY], gridPane, ++row);

			this.addRow(i18n.getString("FormatterOptionDialog.unit.angle.uncertainty.label", "Angle uncertainty:"), i18n.getString("FormatterOptionDialog.unit.angle.uncertainty.tooltip.digits", "Set number of fraction digits for type angle uncertainty"), i18n.getString("FormatterOptionDialog.unit.angle.uncertainty.tooltip.unit", "Set unit for type angle uncertainty"), formatterOptions[CellValueType.ANGLE_UNCERTAINTY], gridPane, ++row);

	//		this.addRow(
	//				i18n.getString("FormatterOptionDialog.unit.scale.uncertainty.label", "Scale uncertainty:"),
	//    			i18n.getString("FormatterOptionDialog.unit.scale.uncertainty.tooltip.digits", "Set number of fraction digits for type scale uncertainty"),
	//    			i18n.getString("FormatterOptionDialog.unit.scale.uncertainty.tooltip.unit", "Set unit for type scale uncertainty"),
	//				formatterOptions.get(CellValueType.SCALE_UNCERTAINTY),
	//				gridPane,
	//				++row);

			this.addRow(i18n.getString("FormatterOptionDialog.unit.vector.uncertainty.label", "Vector uncertainty:"), i18n.getString("FormatterOptionDialog.unit.vector.uncertainty.tooltip.digits", "Set number of fraction digits for type vector uncertainty"), i18n.getString("FormatterOptionDialog.unit.vector.uncertainty.tooltip.unit", "Set unit for type vector uncertainty"), formatterOptions[CellValueType.VECTOR_UNCERTAINTY], gridPane, ++row);

			Region spacer = new Region();
			GridPane.setVgrow(spacer, Priority.ALWAYS);
			gridPane.add(spacer, 1, ++row, 3, 1);
			return DialogUtil.createTitledPane(title, tooltip, gridPane);
		}

		private TitledPane createFormatterOptionResidualsPane()
		{
			string title = i18n.getString("FormatterOptionDialog.residual.title", "Residuals");
			string tooltip = i18n.getString("FormatterOptionDialog.residual.tooltip", "Preferences for residuals");
			GridPane gridPane = DialogUtil.createGridPane();

			IDictionary<CellValueType, FormatterOptions.FormatterOption> formatterOptions = options.FormatterOptions;
			int row = 0;
			this.addRow(i18n.getString("FormatterOptionDialog.unit.length.residual.label", "Length residual:"), i18n.getString("FormatterOptionDialog.unit.length.residual.tooltip.digits", "Set number of fraction digits for type length residual"), i18n.getString("FormatterOptionDialog.unit.length.residual.tooltip.unit", "Set unit for type length residual"), formatterOptions[CellValueType.LENGTH_RESIDUAL], gridPane, ++row);

			this.addRow(i18n.getString("FormatterOptionDialog.unit.angle.residual.label", "Angle residual:"), i18n.getString("FormatterOptionDialog.unit.angle.residual.tooltip.digits", "Set number of fraction digits for type angle residual"), i18n.getString("FormatterOptionDialog.unit.angle.residual.tooltip.unit", "Set unit for type angle residual"), formatterOptions[CellValueType.ANGLE_RESIDUAL], gridPane, ++row);

	//		this.addRow(
	//				i18n.getString("FormatterOptionDialog.unit.scale.residual.label", "Scale residual:"),
	//    			i18n.getString("FormatterOptionDialog.unit.scale.residual.tooltip.digits", "Set number of fraction digits for type scale residual"),
	//    			i18n.getString("FormatterOptionDialog.unit.scale.residual.tooltip.unit", "Set unit for type scale residual"),
	//				formatterOptions.get(CellValueType.SCALE_RESIDUAL),
	//				gridPane,
	//				++row);

			this.addRow(i18n.getString("FormatterOptionDialog.unit.vector.residual.label", "Vector residual:"), i18n.getString("FormatterOptionDialog.unit.vector.residual.tooltip.digits", "Set number of fraction digits for type vector residual"), i18n.getString("FormatterOptionDialog.unit.vector.residual.tooltip.unit", "Set unit for type vector residual"), formatterOptions[CellValueType.VECTOR_RESIDUAL], gridPane, ++row);

			Region spacer = new Region();
			GridPane.setVgrow(spacer, Priority.ALWAYS);
			gridPane.add(spacer, 1, ++row, 3, 1);
			return DialogUtil.createTitledPane(title, tooltip, gridPane);
		}

		private TitledPane createFormatterOptionStatisticsPane()
		{
			string title = i18n.getString("FormatterOptionDialog.statistic.title", "Statistic values");
			string tooltip = i18n.getString("FormatterOptionDialog.statistic.tooltip", "Preferences for unitless statistics values");
			GridPane gridPane = DialogUtil.createGridPane();

			IDictionary<CellValueType, FormatterOptions.FormatterOption> formatterOptions = options.FormatterOptions;
			int row = 0;
			this.addRow(i18n.getString("FormatterOptionDialog.unit.percentage.label", "Percentages values:"), i18n.getString("FormatterOptionDialog.unit.percentage.tooltip.digits", "Set number of fraction digits for type percentage"), i18n.getString("FormatterOptionDialog.unit.percentage.tooltip.unit", "Set unit for percentage values"), formatterOptions[CellValueType.PERCENTAGE], gridPane, ++row);

			this.addRow(i18n.getString("FormatterOptionDialog.unit.statistic.label", "Statistic (unitless):"), i18n.getString("FormatterOptionDialog.unit.statistic.tooltip.digits", "Set number of fraction digits for type statistic"), null, formatterOptions[CellValueType.STATISTIC], gridPane, ++row);

			this.addRow(i18n.getString("FormatterOptionDialog.unit.floatingpoint.label", "Floating-point:"), i18n.getString("FormatterOptionDialog.unit.floatingpoint.tooltip.digits", "Set number of fraction digits for floating-point type"), null, formatterOptions[CellValueType.DOUBLE], gridPane, ++row);

			Region spacer = new Region();
			GridPane.setVgrow(spacer, Priority.ALWAYS);
			gridPane.add(spacer, 1, ++row, 3, 1);
			return DialogUtil.createTitledPane(title, tooltip, gridPane);
		}

		private void addRow(string label, string spinnerTooltip, string comboboxTooltip, FormatterOptions.FormatterOption option, GridPane parent, int row)
		{
			Label unitLabel = new Label(label);
			unitLabel.setWrapText(false);
			unitLabel.setMaxWidth(double.MaxValue);
			unitLabel.setPrefHeight(Control.USE_COMPUTED_SIZE);
			unitLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			CellValueType type = option.Type;
			Spinner<int> digitsSpinner = this.createIntegerSpinner(type, 0, 10, 1, spinnerTooltip);

			ComboBox<Unit> unitComboBox = null;
			Unit unit = option.Unit;
			if (unit != null)
			{
				if ((type == CellValueType.ANGLE || type == CellValueType.ANGLE_RESIDUAL || type == CellValueType.ANGLE_UNCERTAINTY) && AngleUnit.getUnit(unit.Type) != null)
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.Collection<? extends org.applied_geodesy.util.unit.Unit> angleUnits = new java.util.LinkedHashSet<org.applied_geodesy.util.unit.AngleUnit>(org.applied_geodesy.util.unit.AngleUnit.UNITS.values());
					IDictionary<UnitType, AngleUnit>.ValueCollection angleUnits = new LinkedHashSet<AngleUnit>(AngleUnit.UNITS.Values);
					unitComboBox = this.createUnitComboBox(type, comboboxTooltip, angleUnits);
				}
				else if ((type == CellValueType.SCALE || type == CellValueType.SCALE_RESIDUAL || type == CellValueType.SCALE_UNCERTAINTY) && ScaleUnit.getUnit(unit.Type) != null)
				{
					IList<ScaleUnit> scaleUnits = null;
					switch (type.innerEnumValue)
					{
					case CellValueType.InnerEnum.SCALE:
						scaleUnits = new List<ScaleUnit> {ScaleUnit.getUnit(UnitType.UNITLESS), ScaleUnit.getUnit(UnitType.PARTS_PER_MILLION_WRT_ONE)};
						break;
					case CellValueType.InnerEnum.SCALE_RESIDUAL:
					case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
						scaleUnits = new List<ScaleUnit> {ScaleUnit.getUnit(UnitType.UNITLESS), ScaleUnit.getUnit(UnitType.PARTS_PER_MILLION_WRT_ZERO)};
						break;
					default:
						Console.Error.WriteLine(this.GetType().Name + " Error, not a scale type " + type);
						break;
					}
					if (scaleUnits != null)
					{
						unitComboBox = this.createUnitComboBox(type, comboboxTooltip, scaleUnits);
					}
				}
				else if ((type == CellValueType.LENGTH || type == CellValueType.LENGTH_RESIDUAL || type == CellValueType.LENGTH_UNCERTAINTY || type == CellValueType.VECTOR || type == CellValueType.VECTOR_RESIDUAL || type == CellValueType.VECTOR_UNCERTAINTY) && LengthUnit.getUnit(unit.Type) != null)
				{
					unitComboBox = this.createUnitComboBox(type, comboboxTooltip, LengthUnit.UNITS.Values);
				}
				else if (type == CellValueType.PERCENTAGE && PercentUnit.getUnit(unit.Type) != null)
				{
					unitComboBox = this.createUnitComboBox(type, comboboxTooltip, PercentUnit.UNITS.Values);
				}
			}

			this.digitsSpinners[option.Type] = digitsSpinner;

			GridPane.setHgrow(unitLabel, Priority.NEVER);
			GridPane.setHgrow(digitsSpinner, Priority.SOMETIMES);

			GridPane.setMargin(unitLabel, new Insets(5,7,5,5));
			GridPane.setMargin(digitsSpinner, new Insets(5,7,5,7));

			parent.add(unitLabel, 0, row);
			parent.add(digitsSpinner, 1, row);
			if (unitComboBox != null)
			{
				GridPane.setHgrow(unitComboBox, Priority.ALWAYS);
				GridPane.setMargin(unitComboBox, new Insets(5,5,5,7));
				parent.add(unitComboBox, 2, row);
				this.unitComboBoxes[option.Type] = unitComboBox;
			}
			else
			{
				Region spacer = new Region();
				spacer.setMaxWidth(double.MaxValue);
				GridPane.setHgrow(spacer, Priority.ALWAYS);
				parent.add(spacer, 2, row);
			}
		}

		private ComboBox<Unit> createUnitComboBox<T1>(CellValueType type, string tooltip, ICollection<T1> items) where T1 : org.applied_geodesy.util.unit.Unit
		{
			ComboBox<Unit> unitComboBox = new ComboBox<Unit>();
			unitComboBox.getItems().setAll(items);
			unitComboBox.setConverter(new StringConverterAnonymousInnerClass(this));
			unitComboBox.setTooltip(new Tooltip(tooltip));
			unitComboBox.valueProperty().addListener(new UnitChangeListener<Unit>(this, type));
			unitComboBox.setMinWidth(150);
			unitComboBox.setMaxWidth(double.MaxValue);
			return unitComboBox;
		}

		private class StringConverterAnonymousInnerClass : StringConverter<Unit>
		{
			private readonly FormatterOptionDialog outerInstance;

			public StringConverterAnonymousInnerClass(FormatterOptionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override string toString(Unit unit)
			{
				return unit == null ? null : unit.Name;
			}

			public override Unit fromString(string @string)
			{
				return null;
			}
		}

		private Spinner<int> createIntegerSpinner(CellValueType type, int min, int max, int amountToStepBy, string tooltip)
		{
			NumberFormat numberFormat = NumberFormat.getInstance(Locale.ENGLISH);
			numberFormat.setMaximumFractionDigits(0);
			numberFormat.setMinimumFractionDigits(0);
			numberFormat.setGroupingUsed(false);

			StringConverter<int> converter = new StringConverterAnonymousInnerClass2(this, numberFormat);

			SpinnerValueFactory.IntegerSpinnerValueFactory integerFactory = new SpinnerValueFactory.IntegerSpinnerValueFactory(min, max);
			Spinner<int> integerSpinner = new Spinner<int>();
			integerSpinner.setUserData(type);
			integerSpinner.setEditable(true);
			integerSpinner.setValueFactory(integerFactory);
			//integerSpinner.getStyleClass().add(Spinner.STYLE_CLASS_ARROWS_ON_RIGHT_HORIZONTAL);

			integerFactory.setConverter(converter);
			integerFactory.setAmountToStepBy(amountToStepBy);

			TextFormatter<int> formatter = new TextFormatter<int>(integerFactory.getConverter(), integerFactory.getValue());
			integerSpinner.getEditor().setTextFormatter(formatter);
			integerSpinner.getEditor().setAlignment(Pos.BOTTOM_RIGHT);
			integerFactory.valueProperty().bindBidirectional(formatter.valueProperty());

			integerFactory.valueProperty().addListener(new DigitsChangeListener(this, type));
			integerSpinner.setMinWidth(75);
			integerSpinner.setPrefWidth(100);
			integerSpinner.setMaxWidth(double.MaxValue);
			integerSpinner.setTooltip(new Tooltip(tooltip));

			integerFactory.valueProperty().addListener(new ChangeListenerAnonymousInnerClass(this, integerFactory));

			return integerSpinner;
		}

		private class StringConverterAnonymousInnerClass2 : StringConverter<int>
		{
			private readonly FormatterOptionDialog outerInstance;

			private NumberFormat numberFormat;

			public StringConverterAnonymousInnerClass2(FormatterOptionDialog outerInstance, NumberFormat numberFormat)
			{
				this.outerInstance = outerInstance;
				this.numberFormat = numberFormat;
			}

			public override int? fromString(string s)
			{
				if (string.ReferenceEquals(s, null) || s.Trim().Length == 0)
				{
					return null;
				}
				else
				{
					try
					{
						return numberFormat.parse(s).intValue();
					}
					catch (Exception nfe)
					{
						Console.WriteLine(nfe.ToString());
						Console.Write(nfe.StackTrace);
					}
				}
				return null;
			}

			public override string toString(int? d)
			{
				return d == null ? "" : numberFormat.format(d);
			}
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<int>
		{
			private readonly FormatterOptionDialog outerInstance;

			private SpinnerValueFactory.IntegerSpinnerValueFactory integerFactory;

			public ChangeListenerAnonymousInnerClass(FormatterOptionDialog outerInstance, SpinnerValueFactory.IntegerSpinnerValueFactory integerFactory)
			{
				this.outerInstance = outerInstance;
				this.integerFactory = integerFactory;
			}

			public override void changed<T1>(ObservableValue<T1> observable, int? oldValue, int? newValue) where T1 : int
			{
				if (newValue == null)
				{
					integerFactory.setValue(oldValue);
				}
			}
		}
	}

}