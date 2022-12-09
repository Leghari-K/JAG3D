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
	using LayerManager = org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager;
	using LegendLayer = org.applied_geodesy.jag3d.ui.graphic.layer.LegendLayer;
	using LegendPositionType = org.applied_geodesy.jag3d.ui.graphic.layer.LegendPositionType;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Node = javafx.scene.Node;
	using ColorPicker = javafx.scene.control.ColorPicker;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using VBox = javafx.scene.layout.VBox;
	using Color = javafx.scene.paint.Color;
	using StringConverter = javafx.util.StringConverter;

	public class UILegendLayerPropertyBuilder : UILayerPropertyBuilder
	{

		private class FontSizeChangeListener : ChangeListener<double>
		{
			private readonly UILegendLayerPropertyBuilder outerInstance;

			public FontSizeChangeListener(UILegendLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.FontSize = newValue.Value;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class FontColorChangeListener : ChangeListener<Color>
		{
			private readonly UILegendLayerPropertyBuilder outerInstance;

			public FontColorChangeListener(UILegendLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Color oldValue, Color newValue) where T1 : javafx.scene.paint.Color
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.FontColor = newValue;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class FontFamilyChangeListener : ChangeListener<string>
		{
			private readonly UILegendLayerPropertyBuilder outerInstance;

			public FontFamilyChangeListener(UILegendLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, string oldValue, string newValue) where T1 : string
			{
				if (outerInstance.currentLayer != null && !string.ReferenceEquals(newValue, null) && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.FontFamily = newValue;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class LineWidthChangeListener : ChangeListener<double>
		{
			private readonly UILegendLayerPropertyBuilder outerInstance;

			public LineWidthChangeListener(UILegendLayerPropertyBuilder outerInstance)
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
			private readonly UILegendLayerPropertyBuilder outerInstance;

			public ColorChangeListener(UILegendLayerPropertyBuilder outerInstance)
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

		private class LegendPositionTypeChangeListener : ChangeListener<LegendPositionType>
		{
			private readonly UILegendLayerPropertyBuilder outerInstance;

			public LegendPositionTypeChangeListener(UILegendLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, LegendPositionType oldValue, LegendPositionType newValue) where T1 : org.applied_geodesy.jag3d.ui.graphic.layer.LegendPositionType
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.LegendPositionType = newValue;
					outerInstance.legendPositionTypeComboBox.setValue(newValue);
					outerInstance.layerManager.draw();
				}
			}
		}

		private I18N i18n = I18N.Instance;
		private static UILegendLayerPropertyBuilder legendLayerPropertyBuilder = new UILegendLayerPropertyBuilder();


		private ColorPicker fontColorPicker;
		private ComboBox<string> fontFamilyComboBox;
		private ComboBox<double> fontSizeComboBox;

		private ComboBox<double> lineWidthComboBox;
		private ColorPicker borderColorPicker;
		private ComboBox<LegendPositionType> legendPositionTypeComboBox;

		private LegendLayer currentLayer = null;
		private LayerManager layerManager = null;

		private VBox propertyPane = null;

		private UILegendLayerPropertyBuilder()
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
			this.propertyPane.setMinHeight(Control.USE_PREF_SIZE);
			this.propertyPane.getChildren().addAll(this.createSymbolPane(), this.createFontPane());
		}

		public static Node getLayerPropertyPane(LayerManager layerManager, LegendLayer layer)
		{
			legendLayerPropertyBuilder.layerManager = null;
			legendLayerPropertyBuilder.init();
			legendLayerPropertyBuilder.set(layer);
			legendLayerPropertyBuilder.layerManager = layerManager;
			return legendLayerPropertyBuilder.propertyPane;
		}

		private void set(LegendLayer layer)
		{
			// set new layer
			this.currentLayer = layer;

			// Font properties
			this.fontFamilyComboBox.getSelectionModel().select(this.currentLayer.FontFamily);
			this.fontSizeComboBox.getSelectionModel().select(this.currentLayer.FontSize);
			this.fontColorPicker.setValue(this.currentLayer.FontColor);

			// border properties
			this.lineWidthComboBox.getSelectionModel().select(this.currentLayer.LineWidth);
			this.borderColorPicker.setValue(this.currentLayer.Color);
			this.legendPositionTypeComboBox.getSelectionModel().select(this.currentLayer.LegendPositionType);
		}

		private Node createSymbolPane()
		{
			GridPane gridPane = this.createGridPane();

			Label borderColorLabel = new Label(i18n.getString("UILegendLayerPropertyBuilder.border.color.label", "Border color:"));
			borderColorLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label borderLineWidthLabel = new Label(i18n.getString("UILegendLayerPropertyBuilder.border.linewidth.label", "Border width:"));
			borderLineWidthLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label legendPositionTypeLabel = new Label(i18n.getString("UILegendLayerPropertyBuilder.legendposition.label", "Position:"));
			legendPositionTypeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			double?[] symbolSizes = new double?[21];
			for (int i = 0; i < symbolSizes.Length; i++)
			{
				symbolSizes[i] = 5 + 0.5 * i;
			}

			this.lineWidthComboBox = this.createLineWidthComboBox(i18n.getString("UILegendLayerPropertyBuilder.border.linewidth.tooltip", "Set line width"));
			this.legendPositionTypeComboBox = this.createEstimationTypeComboBox(LegendPositionType.NORTH_EAST, i18n.getString("UILegendLayerPropertyBuilder.legendposition.tooltip", "Set legend position"));
			this.borderColorPicker = this.createColorPicker(Color.LIGHTSLATEGREY, i18n.getString("UILegendLayerPropertyBuilder.border.color.tooltip", "Set legend border color"));

			// add listeners
			this.lineWidthComboBox.getSelectionModel().selectedItemProperty().addListener(new LineWidthChangeListener(this));
			this.legendPositionTypeComboBox.getSelectionModel().selectedItemProperty().addListener(new LegendPositionTypeChangeListener(this));
			this.borderColorPicker.valueProperty().addListener(new ColorChangeListener(this));

			borderColorLabel.setLabelFor(this.borderColorPicker);
			borderLineWidthLabel.setLabelFor(this.lineWidthComboBox);
			legendPositionTypeLabel.setLabelFor(this.legendPositionTypeComboBox);

			GridPane.setHgrow(borderColorLabel, Priority.NEVER);
			GridPane.setHgrow(borderLineWidthLabel, Priority.NEVER);
			GridPane.setHgrow(legendPositionTypeLabel, Priority.NEVER);

			GridPane.setHgrow(this.borderColorPicker, Priority.ALWAYS);
			GridPane.setHgrow(this.lineWidthComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.legendPositionTypeComboBox, Priority.ALWAYS);

			int row = 0;
			gridPane.add(borderColorLabel, 0, row);
			gridPane.add(this.borderColorPicker, 1, row++);

			gridPane.add(borderLineWidthLabel, 0, row);
			gridPane.add(this.lineWidthComboBox, 1, row++);

			gridPane.add(legendPositionTypeLabel, 0, row);
			gridPane.add(this.legendPositionTypeComboBox, 1, row++);

			return this.createTitledPane(i18n.getString("UILegendLayerPropertyBuilder.border.title", "Border properties"), gridPane);
		}

		private Node createFontPane()
		{
			GridPane gridPane = this.createGridPane();

			Label fontFamilyLabel = new Label(i18n.getString("UILegendLayerPropertyBuilder.font.family.label", "Font family:"));
			fontFamilyLabel.setMinWidth(Control.USE_PREF_SIZE);

			Label fontSizeLabel = new Label(i18n.getString("UILegendLayerPropertyBuilder.font.size.label", "Font size:"));
			fontSizeLabel.setMinWidth(Control.USE_PREF_SIZE);

			Label fontColorLabel = new Label(i18n.getString("UILegendLayerPropertyBuilder.font.color.label", "Font color:"));
			fontColorLabel.setMinWidth(Control.USE_PREF_SIZE);

			double?[] fontSizes = new double?[10];
			for (int i = 0; i < fontSizes.Length; i++)
			{
				fontSizes[i] = 6.0 + 2 * i; //fontSizes[i] = 5 + 0.5 * i;
			}

			this.fontFamilyComboBox = this.createFontFamliyComboBox(i18n.getString("UILegendLayerPropertyBuilder.font.family.tooltip", "Set font familiy"));
			this.fontSizeComboBox = this.createSizeComboBox(i18n.getString("UILegendLayerPropertyBuilder.font.size.tooltip", "Set font size"), fontSizes, 1);
			this.fontColorPicker = this.createColorPicker(Color.SLATEGREY, i18n.getString("UILegendLayerPropertyBuilder.font.color.tooltip", "Set font color"));

			this.fontFamilyComboBox.getSelectionModel().selectedItemProperty().addListener(new FontFamilyChangeListener(this));
			this.fontSizeComboBox.getSelectionModel().selectedItemProperty().addListener(new FontSizeChangeListener(this));
			this.fontColorPicker.valueProperty().addListener(new FontColorChangeListener(this));

			fontFamilyLabel.setLabelFor(this.fontFamilyComboBox);
			fontSizeLabel.setLabelFor(this.fontSizeComboBox);
			fontColorLabel.setLabelFor(this.fontColorPicker);

			GridPane.setHgrow(fontFamilyLabel, Priority.NEVER);
			GridPane.setHgrow(fontSizeLabel, Priority.NEVER);
			GridPane.setHgrow(fontColorLabel, Priority.NEVER);

			GridPane.setHgrow(this.fontFamilyComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.fontSizeComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.fontColorPicker, Priority.ALWAYS);

			int row = 0;
			gridPane.add(fontFamilyLabel, 0, row);
			gridPane.add(this.fontFamilyComboBox, 1, row++);

			gridPane.add(fontSizeLabel, 0, row);
			gridPane.add(this.fontSizeComboBox, 1, row++);

			gridPane.add(fontColorLabel, 0, row);
			gridPane.add(this.fontColorPicker, 1, row++);

			return this.createTitledPane(i18n.getString("UILegendLayerPropertyBuilder.font.title", "Font properties"), gridPane);
		}

		private ComboBox<LegendPositionType> createEstimationTypeComboBox(LegendPositionType item, string tooltip)
		{
			ComboBox<LegendPositionType> typeComboBox = new ComboBox<LegendPositionType>();
			typeComboBox.getItems().setAll(LegendPositionType.values());
			typeComboBox.getSelectionModel().select(item);
			typeComboBox.setConverter(new StringConverterAnonymousInnerClass(this));
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		private class StringConverterAnonymousInnerClass : StringConverter<LegendPositionType>
		{
			private readonly UILegendLayerPropertyBuilder outerInstance;

			public StringConverterAnonymousInnerClass(UILegendLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override string toString(LegendPositionType type)
			{
				if (type == null)
				{
					return null;
				}
				switch (type.innerEnumValue)
				{

				case LegendPositionType.InnerEnum.NORTH:
					return outerInstance.i18n.getString("UILegendLayerPropertyBuilder.legendposition.north.label", "Noth");
				case LegendPositionType.InnerEnum.EAST:
					return outerInstance.i18n.getString("UILegendLayerPropertyBuilder.legendposition.east.label", "East");
				case LegendPositionType.InnerEnum.SOUTH:
					return outerInstance.i18n.getString("UILegendLayerPropertyBuilder.legendposition.south.label", "South");
				case LegendPositionType.InnerEnum.WEST:
					return outerInstance.i18n.getString("UILegendLayerPropertyBuilder.legendposition.west.label", "West");

				case LegendPositionType.InnerEnum.NORTH_EAST:
					return outerInstance.i18n.getString("UILegendLayerPropertyBuilder.legendposition.northeast.label", "Northeast");
				case LegendPositionType.InnerEnum.NORTH_WEST:
					return outerInstance.i18n.getString("UILegendLayerPropertyBuilder.legendposition.northwest.label", "Northwest");
				case LegendPositionType.InnerEnum.SOUTH_EAST:
					return outerInstance.i18n.getString("UILegendLayerPropertyBuilder.legendposition.southeast.label", "Southeast");
				case LegendPositionType.InnerEnum.SOUTH_WEST:
					return outerInstance.i18n.getString("UILegendLayerPropertyBuilder.legendposition.southwest.label", "Southwest");

				}
				return null;
			}

			public override LegendPositionType fromString(string @string)
			{
				return LegendPositionType.valueOf(@string);
			}
		}
	}
}