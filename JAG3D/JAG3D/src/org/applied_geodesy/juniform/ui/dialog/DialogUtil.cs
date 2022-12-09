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

namespace org.applied_geodesy.juniform.ui.dialog
{

	using EstimationType = org.applied_geodesy.adjustment.EstimationType;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using PrimitiveType = org.applied_geodesy.adjustment.geometry.PrimitiveType;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using RestrictionType = org.applied_geodesy.adjustment.geometry.restriction.RestrictionType;
	using TrigonometricFunctionType = org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType;
	using TestStatisticType = org.applied_geodesy.adjustment.statistic.TestStatisticType;
	using DoubleSpinner = org.applied_geodesy.ui.spinner.DoubleSpinner;
	using DoubleTextField = org.applied_geodesy.ui.textfield.DoubleTextField;
	using ValueSupport = org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport;
	using CellValueType = org.applied_geodesy.util.CellValueType;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using Button = javafx.scene.control.Button;
	using CheckBox = javafx.scene.control.CheckBox;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using ListCell = javafx.scene.control.ListCell;
	using ListView = javafx.scene.control.ListView;
	using RadioButton = javafx.scene.control.RadioButton;
	using SelectionMode = javafx.scene.control.SelectionMode;
	using Spinner = javafx.scene.control.Spinner;
	using SpinnerValueFactory = javafx.scene.control.SpinnerValueFactory;
	using TextField = javafx.scene.control.TextField;
	using TextFormatter = javafx.scene.control.TextFormatter;
	using TitledPane = javafx.scene.control.TitledPane;
	using ToggleGroup = javafx.scene.control.ToggleGroup;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	internal class DialogUtil
	{
		private DialogUtil()
		{
		}

		internal static ComboBox<EstimationType> createEstimationTypeComboBox(StringConverter<EstimationType> estimationTypeStringConverter, string tooltip)
		{
			ComboBox<EstimationType> typeComboBox = new ComboBox<EstimationType>();
			typeComboBox.getItems().setAll(EstimationType.values());
			typeComboBox.setConverter(estimationTypeStringConverter);
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeComboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return typeComboBox;
		}

		internal static ComboBox<GeometricPrimitive> createGeometricPrimitiveComboBox(Callback<ListView<GeometricPrimitive>, ListCell<GeometricPrimitive>> geometricPrimitiveCellFactory, string tooltip)
		{
			ComboBox<GeometricPrimitive> comboBox = new ComboBox<GeometricPrimitive>();
			comboBox.setCellFactory(geometricPrimitiveCellFactory);
			comboBox.setButtonCell(geometricPrimitiveCellFactory.call(null));
			comboBox.setTooltip(new Tooltip(tooltip));
			comboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			comboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return comboBox;
		}

		internal static ComboBox<FeaturePoint> createFeaturePointComboBox(Callback<ListView<FeaturePoint>, ListCell<FeaturePoint>> featurePointCellFactory, string tooltip)
		{
			ComboBox<FeaturePoint> comboBox = new ComboBox<FeaturePoint>();
			comboBox.setCellFactory(featurePointCellFactory);
			comboBox.setButtonCell(featurePointCellFactory.call(null));
			comboBox.setTooltip(new Tooltip(tooltip));
			comboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			comboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return comboBox;
		}

		internal static ComboBox<PrimitiveType> createPrimitiveTypeComboBox(StringConverter<PrimitiveType> primitiveTypeStringConverter, string tooltip)
		{
			ComboBox<PrimitiveType> typeComboBox = new ComboBox<PrimitiveType>();
			typeComboBox.getItems().setAll(PrimitiveType.values());
			typeComboBox.setConverter(primitiveTypeStringConverter);
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeComboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return typeComboBox;
		}

		internal static ComboBox<RestrictionType> createRestrictionTypeComboBox(StringConverter<RestrictionType> restrictionTypeStringConverter, string tooltip)
		{
			ComboBox<RestrictionType> typeComboBox = new ComboBox<RestrictionType>();
			typeComboBox.getItems().setAll((RestrictionType[])Enum.GetValues(typeof(RestrictionType)));
			typeComboBox.setConverter(restrictionTypeStringConverter);
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.getSelectionModel().select(RestrictionType.AVERAGE);
			typeComboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeComboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return typeComboBox;
		}

		internal static ComboBox<UnknownParameter> createUnknownParameterComboBox(Callback<ListView<UnknownParameter>, ListCell<UnknownParameter>> unknownParameterCellFactory, string tooltip)
		{
			ComboBox<UnknownParameter> comboBox = new ComboBox<UnknownParameter>();
			comboBox.setCellFactory(unknownParameterCellFactory);
			comboBox.setButtonCell(unknownParameterCellFactory.call(null));

			comboBox.setTooltip(new Tooltip(tooltip));
			comboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			comboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return comboBox;
		}

		internal static ComboBox<TrigonometricFunctionType> createTrigonometricFunctionTypeComboBox(StringConverter<TrigonometricFunctionType> trigonometricFunctionTypeStringConverter, string tooltip)
		{
			ComboBox<TrigonometricFunctionType> typeComboBox = new ComboBox<TrigonometricFunctionType>();
			typeComboBox.getItems().setAll((TrigonometricFunctionType[])Enum.GetValues(typeof(TrigonometricFunctionType)));
			typeComboBox.setConverter(trigonometricFunctionTypeStringConverter);
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.getSelectionModel().select(TrigonometricFunctionType.TANGENT);
			typeComboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeComboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return typeComboBox;
		}

		internal static GridPane createGridPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMaxWidth(double.MaxValue);
			gridPane.setMinWidth(Control.USE_PREF_SIZE); // 300
	//		gridPane.setHgap(20);
	//		gridPane.setVgap(10);
			gridPane.setAlignment(Pos.TOP_CENTER);
			gridPane.setPadding(new Insets(5,15,5,15)); // oben, recht, unten, links
			return gridPane;
		}

		internal static Button createButton(string title, string tooltip)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,0));
			Button button = new Button();
			button.setGraphic(label);
			button.setTooltip(new Tooltip(tooltip));
			button.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			button.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return button;
		}

		internal static ListView<UnknownParameter> createParameterListView(Callback<ListView<UnknownParameter>, ListCell<UnknownParameter>> unknownParameterCellFactory)
		{
			ListView<UnknownParameter> list = new ListView<UnknownParameter>();
			list.setCellFactory(unknownParameterCellFactory);
			list.getSelectionModel().setSelectionMode(SelectionMode.SINGLE);
			ListView<string> placeholderList = new ListView<string>();
			placeholderList.getItems().add("");
			placeholderList.setDisable(true);
			list.setPlaceholder(placeholderList);

			return list;
		}

		internal static ListView<GeometricPrimitive> createGeometricPrimitiveListView(Callback<ListView<GeometricPrimitive>, ListCell<GeometricPrimitive>> geometricPrimitiveCellFactory)
		{
			ListView<GeometricPrimitive> list = new ListView<GeometricPrimitive>();
			list.setCellFactory(geometricPrimitiveCellFactory);
			list.getSelectionModel().setSelectionMode(SelectionMode.SINGLE);
			ListView<string> placeholderList = new ListView<string>();
			placeholderList.getItems().add("");
			placeholderList.setDisable(true);
			list.setPlaceholder(placeholderList);

			return list;
		}

		internal static TextField createTextField(string tooltip, string prompt)
		{
			TextField textField = new TextField();
			textField.setTooltip(new Tooltip(tooltip));
			textField.setPromptText(prompt);
	//		textField.setMinWidth(100);
	//		textField.setPrefWidth(150);
	//		textField.setMaxWidth(Double.MAX_VALUE);
			textField.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			textField.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return textField;
		}

		internal static DoubleTextField createDoubleTextField(CellValueType cellValueType, double value, string tooltip)
		{
			DoubleTextField textField = new DoubleTextField(value, cellValueType, true, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT);
			textField.setTooltip(new Tooltip(tooltip));
			textField.setAlignment(Pos.CENTER_RIGHT);
			textField.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
	//		textField.setPrefWidth(150);
			textField.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return textField;
		}

		internal static DoubleTextField createDoubleTextField(CellValueType cellValueType, double value, bool displayUnit, DoubleTextField.ValueSupport valueSupport, double lowerBoundary, double upperBoundary, string tooltip)
		{
			DoubleTextField textField = new DoubleTextField(value, cellValueType, displayUnit, valueSupport, lowerBoundary, upperBoundary);
			textField.setTooltip(new Tooltip(tooltip));
			textField.setAlignment(Pos.CENTER_RIGHT);
			textField.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			textField.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			return textField;
		}

		internal static ComboBox<ParameterType> createParameterTypeComboBox(StringConverter<ParameterType> parameterTypeStringConverter, string tooltip)
		{
			ComboBox<ParameterType> typeComboBox = new ComboBox<ParameterType>();
			typeComboBox.getItems().setAll((ParameterType[])Enum.GetValues(typeof(ParameterType)));
			typeComboBox.setConverter(parameterTypeStringConverter);
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.getSelectionModel().select(ParameterType.LENGTH);
	//		typeComboBox.setMinWidth(150);
	//		typeComboBox.setPrefWidth(200);
	//		typeComboBox.setMaxWidth(Double.MAX_VALUE);
			typeComboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeComboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height

			return typeComboBox;
		}

		internal static ComboBox<bool> createBooleanComboBox(StringConverter<bool> booleanTypeStringConverter, string tooltip)
		{
			ComboBox<bool> typeComboBox = new ComboBox<bool>();
			typeComboBox.getItems().setAll(List.of(true, false));
			typeComboBox.setConverter(booleanTypeStringConverter);
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.getSelectionModel().select(true);
			typeComboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeComboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height

			return typeComboBox;
		}

		internal static CheckBox createCheckBox(string title, string tooltip)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			CheckBox checkBox = new CheckBox();
			checkBox.setGraphic(label);
			checkBox.setTooltip(new Tooltip(tooltip));
			checkBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			checkBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE);
			return checkBox;
		}

		internal static RadioButton createRadioButton(string title, string tooltip, ToggleGroup toggleGroup)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			RadioButton radioButton = new RadioButton();
			radioButton.setGraphic(label);
			radioButton.setTooltip(new Tooltip(tooltip));
			radioButton.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			radioButton.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE);
			if (toggleGroup != null)
			{
				radioButton.setToggleGroup(toggleGroup);
			}
			return radioButton;
		}

		internal static ComboBox<TestStatisticType> createTestStatisticTypeComboBox(StringConverter<TestStatisticType> testStatisticTypeStringConverter, string tooltip)
		{
			ComboBox<TestStatisticType> typeComboBox = new ComboBox<TestStatisticType>();
			typeComboBox.getItems().setAll(TestStatisticType.values());
			typeComboBox.setConverter(testStatisticTypeStringConverter);
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.getSelectionModel().select(TestStatisticType.NONE);
	//		typeComboBox.setMinWidth(150);
	//		typeComboBox.setPrefWidth(200);
			typeComboBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeComboBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		internal static DoubleSpinner createDoubleSpinner(CellValueType cellValueType, double min, double max, double amountToStepBy, string tooltip)
		{
			DoubleSpinner doubleSpinner = new DoubleSpinner(cellValueType, min, max, amountToStepBy);
			doubleSpinner.setMinWidth(75);
			doubleSpinner.setPrefWidth(100);
			doubleSpinner.setMaxWidth(double.MaxValue);
			doubleSpinner.setTooltip(new Tooltip(tooltip));
			return doubleSpinner;
		}

		internal static Spinner<int> createIntegerSpinner(int min, int max, int amountToStepBy, string tooltip)
		{
			NumberFormat numberFormat = NumberFormat.getInstance(Locale.ENGLISH);
			numberFormat.setMaximumFractionDigits(0);
			numberFormat.setMinimumFractionDigits(0);
			numberFormat.setGroupingUsed(false);

			StringConverter<int> converter = new StringConverterAnonymousInnerClass(numberFormat);

			SpinnerValueFactory.IntegerSpinnerValueFactory integerFactory = new SpinnerValueFactory.IntegerSpinnerValueFactory(min, max);
			Spinner<int> integerSpinner = new Spinner<int>();
			integerSpinner.setEditable(true);
			integerSpinner.setValueFactory(integerFactory);
			//integerSpinner.getStyleClass().add(Spinner.STYLE_CLASS_ARROWS_ON_RIGHT_HORIZONTAL);

			integerFactory.setConverter(converter);
			integerFactory.setAmountToStepBy(amountToStepBy);

			TextFormatter<int> formatter = new TextFormatter<int>(integerFactory.getConverter(), integerFactory.getValue());
			integerSpinner.getEditor().setTextFormatter(formatter);
			integerSpinner.getEditor().setAlignment(Pos.BOTTOM_RIGHT);
			integerFactory.valueProperty().bindBidirectional(formatter.valueProperty());

			integerSpinner.setMinWidth(75);
			integerSpinner.setPrefWidth(100);
			integerSpinner.setMaxWidth(double.MaxValue);
			integerSpinner.setTooltip(new Tooltip(tooltip));

			integerFactory.valueProperty().addListener(new ChangeListenerAnonymousInnerClass(integerFactory));

			return integerSpinner;
		}

		private class StringConverterAnonymousInnerClass : StringConverter<int>
		{
			private NumberFormat numberFormat;

			public StringConverterAnonymousInnerClass(NumberFormat numberFormat)
			{
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
			private SpinnerValueFactory.IntegerSpinnerValueFactory integerFactory;

			public ChangeListenerAnonymousInnerClass(SpinnerValueFactory.IntegerSpinnerValueFactory integerFactory)
			{
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

		internal static ComboBox<ProcessingType> createProcessingTypeComboBox(StringConverter<ProcessingType> processingTypeStringConverter, string tooltip)
		{
			ComboBox<ProcessingType> typeComboBox = new ComboBox<ProcessingType>();
			typeComboBox.setConverter(processingTypeStringConverter);
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.getSelectionModel().select(ProcessingType.ADJUSTMENT);
			typeComboBox.setMinWidth(150);
			typeComboBox.setPrefWidth(200);
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		internal static TitledPane createTitledPane(string title, string tooltip, Node content)
		{
			Label label = new Label(title);
			label.setTooltip(new Tooltip(tooltip));
			TitledPane titledPane = new TitledPane();
			titledPane.setGraphic(label);
			titledPane.setCollapsible(true);
			titledPane.setAnimated(true);
			titledPane.setContent(content);
			titledPane.setPadding(new Insets(0, 10, 5, 10)); // oben, links, unten, rechts
	//		titledPane.setText(title);
	//		titledPane.setTooltip(new Tooltip(tooltip));
			return titledPane;
		}
	}

}