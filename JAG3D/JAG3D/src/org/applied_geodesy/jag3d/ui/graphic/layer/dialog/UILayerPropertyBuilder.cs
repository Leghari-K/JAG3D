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

namespace org.applied_geodesy.jag3d.ui.graphic.layer.dialog
{

	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using CheckBox = javafx.scene.control.CheckBox;
	using ColorPicker = javafx.scene.control.ColorPicker;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using ListCell = javafx.scene.control.ListCell;
	using ListView = javafx.scene.control.ListView;
	using TitledPane = javafx.scene.control.TitledPane;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Color = javafx.scene.paint.Color;
	using Font = javafx.scene.text.Font;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public abstract class UILayerPropertyBuilder
	{

		internal virtual ComboBox<double> createLineWidthComboBox(string tooltip)
		{
			double?[] values = new double?[17];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = 0.25 + 0.25 * i;
			}

			ComboBox<double> lineWidthComboBox = new ComboBox<double>();
			lineWidthComboBox.getItems().setAll(values);
			lineWidthComboBox.getSelectionModel().select(0);
			lineWidthComboBox.setCellFactory(new CallbackAnonymousInnerClass(this));
			lineWidthComboBox.setButtonCell(new LineWidthListCell());
			lineWidthComboBox.setTooltip(new Tooltip(tooltip));
			lineWidthComboBox.setMaxWidth(double.MaxValue);
			return lineWidthComboBox;
		}

		private class CallbackAnonymousInnerClass : Callback<ListView<double>, ListCell<double>>
		{
			private readonly UILayerPropertyBuilder outerInstance;

			public CallbackAnonymousInnerClass(UILayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override ListCell<double> call(ListView<double> param)
			{
				return new LineWidthListCell();
			}
		}

		internal virtual ComboBox<double> createSizeComboBox(string tooltip, double?[] values, int digits)
		{
			NumberFormat numberFormat = NumberFormat.getInstance(Locale.ENGLISH);
			numberFormat.setMaximumFractionDigits(digits);
			numberFormat.setMinimumFractionDigits(digits);
			numberFormat.setGroupingUsed(false);

			ComboBox<double> symbolSizeComboBox = new ComboBox<double>();
			symbolSizeComboBox.getItems().setAll(values);

			symbolSizeComboBox.setEditable(true);
			symbolSizeComboBox.valueProperty().addListener(new ChangeListenerAnonymousInnerClass(this, symbolSizeComboBox));

			symbolSizeComboBox.setConverter(new StringConverterAnonymousInnerClass(this, numberFormat));

			symbolSizeComboBox.getSelectionModel().select(0);
			symbolSizeComboBox.setTooltip(new Tooltip(tooltip));
			symbolSizeComboBox.setMaxWidth(double.MaxValue);
			symbolSizeComboBox.setMinWidth(65);
			symbolSizeComboBox.setPrefWidth(75);
			return symbolSizeComboBox;
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<double>
		{
			private readonly UILayerPropertyBuilder outerInstance;

			private ComboBox<double> symbolSizeComboBox;

			public ChangeListenerAnonymousInnerClass(UILayerPropertyBuilder outerInstance, ComboBox<double> symbolSizeComboBox)
			{
				this.outerInstance = outerInstance;
				this.symbolSizeComboBox = symbolSizeComboBox;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (newValue == null)
				{
					symbolSizeComboBox.getSelectionModel().select(oldValue);
				}
			}
		}

		private class StringConverterAnonymousInnerClass : StringConverter<double>
		{
			private readonly UILayerPropertyBuilder outerInstance;

			private NumberFormat numberFormat;

			public StringConverterAnonymousInnerClass(UILayerPropertyBuilder outerInstance, NumberFormat numberFormat)
			{
				this.outerInstance = outerInstance;
				this.numberFormat = numberFormat;
			}

			public override string toString(double? value)
			{
				return value == null ? null : numberFormat.format(value);
			}

			public override double? fromString(string @string)
			{
				if (string.ReferenceEquals(@string, null) || @string.Length == 0)
				{
					return null;
				}
				try
				{
					return numberFormat.parse(@string).doubleValue();
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		internal virtual GridPane createGridPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMaxWidth(double.MaxValue);
			gridPane.setHgap(20);
			gridPane.setVgap(10);
			gridPane.setAlignment(Pos.CENTER);
			gridPane.setPadding(new Insets(5,15,5,15)); // oben, recht, unten, links
			//gridPane.setGridLinesVisible(true);
			return gridPane;
		}

		internal virtual TitledPane createTitledPane(string title, Node content)
		{
			TitledPane titledPane = new TitledPane();
			titledPane.setCollapsible(false);
			titledPane.setAnimated(false);
			titledPane.setContent(content);
			titledPane.setPadding(new Insets(0, 10, 5, 10)); // oben, links, unten, rechts
			titledPane.setText(title);
			return titledPane;
		}

		internal virtual CheckBox createCheckBox(string title, string tooltip)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			CheckBox checkBox = new CheckBox();
			checkBox.setGraphic(label);
			checkBox.setTooltip(new Tooltip(tooltip));
			checkBox.setMinSize(Control.USE_PREF_SIZE,Control.USE_PREF_SIZE);
			checkBox.setMaxWidth(double.MaxValue);
			return checkBox;
		}

		internal virtual ComboBox<string> createFontFamliyComboBox(string tooltip)
		{
			ComboBox<string> typeComboBox = new ComboBox<string>();
			typeComboBox.getItems().setAll(Font.getFamilies());
			typeComboBox.getSelectionModel().select(Font.getDefault().getFamily());
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		internal virtual ComboBox<TableRowHighlightType> createHighlightTypeComboBox(string tooltip, StringConverter<TableRowHighlightType> converter)
		{
			ComboBox<TableRowHighlightType> typeComboBox = new ComboBox<TableRowHighlightType>();
			typeComboBox.getItems().setAll(TableRowHighlightType.values());
			typeComboBox.getSelectionModel().select(TableRowHighlightType.NONE);
			typeComboBox.setConverter(converter);
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMinWidth(150);
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		internal virtual ColorPicker createColorPicker(Color color, string tooltip)
		{
			ColorPicker colorPicker = new ColorPicker(color);
			colorPicker.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			colorPicker.setMaxWidth(double.MaxValue);
			colorPicker.getStyleClass().add("split-button");
			colorPicker.setTooltip(new Tooltip(tooltip));
			return colorPicker;
		}
	}

}