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
	using ArrowLayer = org.applied_geodesy.jag3d.ui.graphic.layer.ArrowLayer;
	using LayerManager = org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager;
	using ArrowSymbolType = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.ArrowSymbolType;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Node = javafx.scene.Node;
	using ColorPicker = javafx.scene.control.ColorPicker;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using ListCell = javafx.scene.control.ListCell;
	using ListView = javafx.scene.control.ListView;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using VBox = javafx.scene.layout.VBox;
	using Color = javafx.scene.paint.Color;
	using Callback = javafx.util.Callback;

	public class UIArrowLayerPropertyBuilder : UILayerPropertyBuilder
	{
		private class SymbolTypeChangeListener : ChangeListener<ArrowSymbolType>
		{
			private readonly UIArrowLayerPropertyBuilder outerInstance;

			public SymbolTypeChangeListener(UIArrowLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, ArrowSymbolType oldValue, ArrowSymbolType newValue) where T1 : org.applied_geodesy.jag3d.ui.graphic.layer.symbol.ArrowSymbolType
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.SymbolType = newValue;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class SymbolSizeChangeListener : ChangeListener<double>
		{
			private readonly UIArrowLayerPropertyBuilder outerInstance;

			public SymbolSizeChangeListener(UIArrowLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.SymbolSize = newValue.Value;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class LineWidthChangeListener : ChangeListener<double>
		{
			private readonly UIArrowLayerPropertyBuilder outerInstance;

			public LineWidthChangeListener(UIArrowLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.LineWidth = newValue.Value;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class ColorChangeListener : ChangeListener<Color>
		{
			private readonly UIArrowLayerPropertyBuilder outerInstance;

			public ColorChangeListener(UIArrowLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Color oldValue, Color newValue) where T1 : javafx.scene.paint.Color
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.Color = newValue;
					outerInstance.layerManager.draw();
				}
			}
		}

		private I18N i18n = I18N.Instance;
		private static UIArrowLayerPropertyBuilder arrowLayerPropertyBuilder = new UIArrowLayerPropertyBuilder();
		private ComboBox<ArrowSymbolType> symbolTypeComboBox;
		private ComboBox<double> symbolSizeComboBox;
		private ComboBox<double> lineWidthComboBox;
		private ColorPicker symbolColorPicker;
		private ArrowLayer currentLayer = null;
		private LayerManager layerManager = null;
		private VBox propertyPane = null;

		private UIArrowLayerPropertyBuilder()
		{
		}

		private void init()
		{
			if (this.propertyPane != null)
			{
				return;
			}

			this.propertyPane = new VBox(20);
			this.propertyPane.setMaxSize(double.MaxValue, double.MaxValue);
			this.propertyPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.propertyPane.getChildren().addAll(this.createSymbolPane());
		}

		public static Node getLayerPropertyPane(LayerManager layerManager, ArrowLayer layer)
		{
			arrowLayerPropertyBuilder.layerManager = null;
			arrowLayerPropertyBuilder.init();
			arrowLayerPropertyBuilder.set(layer);
			arrowLayerPropertyBuilder.layerManager = layerManager;
			return arrowLayerPropertyBuilder.propertyPane;
		}

		private void set(ArrowLayer layer)
		{
			// set new layer
			this.currentLayer = layer;

			// Symbol properties
			this.symbolSizeComboBox.getSelectionModel().select(this.currentLayer.SymbolSize);
			this.lineWidthComboBox.getSelectionModel().select(this.currentLayer.LineWidth);
			this.symbolColorPicker.setValue(this.currentLayer.Color);
			this.symbolTypeComboBox.setValue(this.currentLayer.SymbolType);

			// Font properties
	//		this.fontFamilyComboBox.getSelectionModel().select(this.currentLayer.getFontFamily());
	//		this.fontSizeComboBox.getSelectionModel().select(this.currentLayer.getFontSize());
	//		this.fontColorPicker.setValue(this.currentLayer.getFontColor());
		}

		private Node createSymbolPane()
		{
			GridPane gridPane = this.createGridPane();

			Label symbolTypeLabel = new Label(i18n.getString("UIArrowLayerPropertyBuilder.symbol.type.label", "Symbol type:"));
			symbolTypeLabel.setMinWidth(Control.USE_PREF_SIZE);

			Label symbolSizeLabel = new Label(i18n.getString("UIArrowLayerPropertyBuilder.symbol.size.label", "Symbol size:"));
			symbolSizeLabel.setMinWidth(Control.USE_PREF_SIZE);

			Label symbolColorLabel = new Label(i18n.getString("UIArrowLayerPropertyBuilder.symbol.color.label", "Symbol color:"));
			symbolColorLabel.setMinWidth(Control.USE_PREF_SIZE);

			Label symbolLineWidthLabel = new Label(i18n.getString("UIArrowLayerPropertyBuilder.symbol.linewidth.label", "Line width:"));
			symbolLineWidthLabel.setMinWidth(Control.USE_PREF_SIZE);

			double?[] symbolSizes = new double?[21];
			for (int i = 0; i < symbolSizes.Length; i++)
			{
				symbolSizes[i] = 5 + 0.5 * i;
			}

			this.symbolTypeComboBox = this.createSymbolComboBox(i18n.getString("UIArrowLayerPropertyBuilder.symbol.type.tooltip", "Set arrow symbol type"));
			this.symbolSizeComboBox = this.createSizeComboBox(i18n.getString("UIArrowLayerPropertyBuilder.symbol.size.tooltip", "Set arrow symbol size"), symbolSizes, 1);
			this.lineWidthComboBox = this.createLineWidthComboBox(i18n.getString("UIArrowLayerPropertyBuilder.symbol.linewidth.tooltip", "Set line width"));
			this.symbolColorPicker = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIArrowLayerPropertyBuilder.symbol.color.tooltip", "Set symbol color"));

			// add listeners
			this.symbolTypeComboBox.getSelectionModel().selectedItemProperty().addListener(new SymbolTypeChangeListener(this));
			this.symbolSizeComboBox.getSelectionModel().selectedItemProperty().addListener(new SymbolSizeChangeListener(this));
			this.lineWidthComboBox.getSelectionModel().selectedItemProperty().addListener(new LineWidthChangeListener(this));
			this.symbolColorPicker.valueProperty().addListener(new ColorChangeListener(this));

			symbolTypeLabel.setLabelFor(this.symbolTypeComboBox);
			symbolSizeLabel.setLabelFor(this.symbolSizeComboBox);
			symbolColorLabel.setLabelFor(this.symbolColorPicker);
			symbolLineWidthLabel.setLabelFor(this.lineWidthComboBox);

			GridPane.setHgrow(symbolTypeLabel, Priority.NEVER);
			GridPane.setHgrow(symbolSizeLabel, Priority.NEVER);
			GridPane.setHgrow(symbolColorLabel, Priority.NEVER);
			GridPane.setHgrow(symbolLineWidthLabel, Priority.NEVER);

			GridPane.setHgrow(this.symbolTypeComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.symbolSizeComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.symbolColorPicker, Priority.ALWAYS);
			GridPane.setHgrow(this.lineWidthComboBox, Priority.ALWAYS);

			int row = 0;
			gridPane.add(symbolTypeLabel, 0, row);
			gridPane.add(this.symbolTypeComboBox, 1, row++);

			gridPane.add(symbolSizeLabel, 0, row);
			gridPane.add(this.symbolSizeComboBox, 1, row++);

			gridPane.add(symbolColorLabel, 0, row);
			gridPane.add(this.symbolColorPicker, 1, row++);

			gridPane.add(symbolLineWidthLabel, 0, row);
			gridPane.add(this.lineWidthComboBox, 1, row++);

			return this.createTitledPane(i18n.getString("UIArrowLayerPropertyBuilder.symbol.title", "Symbol properties"), gridPane);
		}

		private ComboBox<ArrowSymbolType> createSymbolComboBox(string tooltip)
		{
			ComboBox<ArrowSymbolType> typeComboBox = new ComboBox<ArrowSymbolType>();
			typeComboBox.getItems().setAll(ArrowSymbolType.values());
			typeComboBox.getSelectionModel().select(0);
			typeComboBox.setCellFactory(new CallbackAnonymousInnerClass(this));
			typeComboBox.setButtonCell(new ArrowSymbolTypeListCell());
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		private class CallbackAnonymousInnerClass : Callback<ListView<ArrowSymbolType>, ListCell<ArrowSymbolType>>
		{
			private readonly UIArrowLayerPropertyBuilder outerInstance;

			public CallbackAnonymousInnerClass(UIArrowLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override ListCell<ArrowSymbolType> call(ListView<ArrowSymbolType> param)
			{
				return new ArrowSymbolTypeListCell();
			}
		}
	}

}