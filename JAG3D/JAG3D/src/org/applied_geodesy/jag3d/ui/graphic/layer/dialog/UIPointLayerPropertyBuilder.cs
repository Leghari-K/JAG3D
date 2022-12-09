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
	using PointLayer = org.applied_geodesy.jag3d.ui.graphic.layer.PointLayer;
	using PointSymbolType = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.PointSymbolType;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Node = javafx.scene.Node;
	using CheckBox = javafx.scene.control.CheckBox;
	using ColorPicker = javafx.scene.control.ColorPicker;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using ListCell = javafx.scene.control.ListCell;
	using ListView = javafx.scene.control.ListView;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using VBox = javafx.scene.layout.VBox;
	using Color = javafx.scene.paint.Color;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class UIPointLayerPropertyBuilder : UILayerPropertyBuilder
	{
		private class SymbolTypeChangeListener : ChangeListener<PointSymbolType>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public SymbolTypeChangeListener(UIPointLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, PointSymbolType oldValue, PointSymbolType newValue) where T1 : org.applied_geodesy.jag3d.ui.graphic.layer.symbol.PointSymbolType
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
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public SymbolSizeChangeListener(UIPointLayerPropertyBuilder outerInstance)
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
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public LineWidthChangeListener(UIPointLayerPropertyBuilder outerInstance)
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
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public ColorChangeListener(UIPointLayerPropertyBuilder outerInstance)
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

		private class FontFamilyChangeListener : ChangeListener<string>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public FontFamilyChangeListener(UIPointLayerPropertyBuilder outerInstance)
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

		private class FontColorChangeListener : ChangeListener<Color>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public FontColorChangeListener(UIPointLayerPropertyBuilder outerInstance)
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

		private class FontSizeChangeListener : ChangeListener<double>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public FontSizeChangeListener(UIPointLayerPropertyBuilder outerInstance)
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

		private class HighlightColorChangeListener : ChangeListener<Color>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public HighlightColorChangeListener(UIPointLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Color oldValue, Color newValue) where T1 : javafx.scene.paint.Color
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.HighlightColor = newValue;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class HighlightLineWidthChangeListener : ChangeListener<double>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public HighlightLineWidthChangeListener(UIPointLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.HighlightLineWidth = newValue.Value;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class HighlightTypeChangeListener : ChangeListener<TableRowHighlightType>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public HighlightTypeChangeListener(UIPointLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, TableRowHighlightType oldValue, TableRowHighlightType newValue) where T1 : org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.HighlightType = newValue;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class PointVisibleChangeListener : ChangeListener<bool>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			internal int dim;
			internal PointVisibleChangeListener(UIPointLayerPropertyBuilder outerInstance, int dim)
			{
				this.outerInstance = outerInstance;
				this.dim = dim;
			}
			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					switch (this.dim)
					{
					case 1:
						outerInstance.currentLayer.Point1DVisible = newValue.Value;
						outerInstance.layerManager.draw();
						break;
					case 2:
						outerInstance.currentLayer.Point2DVisible = newValue.Value;
						outerInstance.layerManager.draw();
						break;
					case 3:
						outerInstance.currentLayer.Point3DVisible = newValue.Value;
						outerInstance.layerManager.draw();
						break;
					}
				}
			}
		}

		private I18N i18n = I18N.Instance;
		private static UIPointLayerPropertyBuilder pointLayerPropertyBuilder = new UIPointLayerPropertyBuilder();

		private ComboBox<PointSymbolType> symbolTypeComboBox;
		private ComboBox<double> symbolSizeComboBox;
		private ComboBox<double> lineWidthComboBox;
		private ColorPicker symbolColorPicker;

		private ComboBox<string> fontFamilyComboBox;
		private ComboBox<double> fontSizeComboBox;
		private ColorPicker fontColorPicker;

		private Node highlightPane = null;
		private ComboBox<double> highlightLineWidthComboBox;
		private ColorPicker highlightColorPicker;
		private ComboBox<TableRowHighlightType> highlightTypeComboBox;

		private CheckBox point1DVisibleCheckBox;
		private CheckBox point2DVisibleCheckBox;
		private CheckBox point3DVisibleCheckBox;

		private PointLayer currentLayer = null;
		private LayerManager layerManager = null;

		private VBox propertyPane = null;

		private UIPointLayerPropertyBuilder()
		{
		}

		private void init()
		{
			if (this.propertyPane != null)
			{
				return;
			}

			this.highlightPane = this.createHighlightPane();
			this.propertyPane = new VBox(20);
			this.propertyPane.setMaxSize(double.MaxValue, double.MaxValue);
			this.propertyPane.setMinHeight(Control.USE_PREF_SIZE);
			this.propertyPane.getChildren().addAll(this.createSymbolPane(), this.createFontPane(), this.createPointDimensionVisibilityPane(), this.highlightPane);
		}

		public static Node getLayerPropertyPane(LayerManager layerManager, PointLayer layer)
		{
			pointLayerPropertyBuilder.layerManager = null;
			pointLayerPropertyBuilder.init();
			pointLayerPropertyBuilder.set(layer);
			pointLayerPropertyBuilder.layerManager = layerManager;
			return pointLayerPropertyBuilder.propertyPane;
		}

		private void set(PointLayer layer)
		{
			// set new layer
			this.currentLayer = layer;

			// Symbol properties
			this.symbolTypeComboBox.getSelectionModel().select(this.currentLayer.PointSymbolType);
			this.symbolSizeComboBox.getSelectionModel().select(this.currentLayer.SymbolSize);
			this.lineWidthComboBox.getSelectionModel().select(this.currentLayer.LineWidth);
			this.symbolColorPicker.setValue(this.currentLayer.Color);

			switch (this.currentLayer.LayerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
				this.prepareHighlightTypeComboBox(this.highlightTypeComboBox);
				this.highlightColorPicker.setValue(this.currentLayer.HighlightColor);
				this.highlightLineWidthComboBox.getSelectionModel().select(this.currentLayer.HighlightLineWidth);
				if (this.highlightTypeComboBox.getItems().contains(this.currentLayer.HighlightType))
				{
					this.highlightTypeComboBox.getSelectionModel().select(this.currentLayer.HighlightType);
				}
				else
				{
					this.highlightTypeComboBox.getSelectionModel().select(TableRowHighlightType.NONE);
				}
				this.highlightPane.setVisible(true);
				this.highlightPane.setManaged(true);
				break;

			default:
				this.highlightPane.setVisible(false);
				this.highlightPane.setManaged(false);
				break;
			}

			// Font properties
			this.fontFamilyComboBox.getSelectionModel().select(this.currentLayer.FontFamily);
			this.fontSizeComboBox.getSelectionModel().select(this.currentLayer.FontSize);
			this.fontColorPicker.setValue(this.currentLayer.FontColor);

			// Visibility properties w.r.t. dimension
			this.point1DVisibleCheckBox.setSelected(this.currentLayer.Point1DVisible);
			this.point2DVisibleCheckBox.setSelected(this.currentLayer.Point2DVisible);
			this.point3DVisibleCheckBox.setSelected(this.currentLayer.Point3DVisible);
		}

		private Node createFontPane()
		{
			GridPane gridPane = this.createGridPane();

			Label fontFamilyLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.font.family.label", "Font family:"));
			fontFamilyLabel.setMinWidth(Control.USE_PREF_SIZE);

			Label fontSizeLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.font.size.label", "Font size:"));
			fontSizeLabel.setMinWidth(Control.USE_PREF_SIZE);

			Label fontColorLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.font.color.label", "Font color:"));
			fontColorLabel.setMinWidth(Control.USE_PREF_SIZE);

			double?[] fontSizes = new double?[10];
			for (int i = 0; i < fontSizes.Length; i++)
			{
				fontSizes[i] = 6.0 + 2 * i; //fontSizes[i] = 5 + 0.5 * i;
			}

			this.fontFamilyComboBox = this.createFontFamliyComboBox(i18n.getString("UIPointLayerPropertyBuilder.font.family.tooltip", "Set font familiy"));
			this.fontSizeComboBox = this.createSizeComboBox(i18n.getString("UIPointLayerPropertyBuilder.font.size.tooltip", "Set font size"), fontSizes, 1);
			this.fontColorPicker = this.createColorPicker(Color.BLACK, i18n.getString("UIPointLayerPropertyBuilder.font.color.tooltip", "Set font color"));

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

			return this.createTitledPane(i18n.getString("UIPointLayerPropertyBuilder.font.title", "Font properties"), gridPane);
		}

		private Node createSymbolPane()
		{
			GridPane gridPane = this.createGridPane();

			Label symbolTypeLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.symbol.type.label", "Symbol type:"));
			symbolTypeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label symbolSizeLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.symbol.size.label", "Symbol size:"));
			symbolSizeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label symbolColorLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.symbol.color.label", "Symbol color:"));
			symbolColorLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label symbolLineWidthLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.symbol.linewidth.label", "Line width:"));
			symbolLineWidthLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			double?[] symbolSizes = new double?[21];
			for (int i = 0; i < symbolSizes.Length; i++)
			{
				symbolSizes[i] = 5 + 0.5 * i;
			}

			this.symbolTypeComboBox = this.createSymbolComboBox(i18n.getString("UIPointLayerPropertyBuilder.symbol.type.tooltip", "Set point symbol type"));
			this.symbolSizeComboBox = this.createSizeComboBox(i18n.getString("UIPointLayerPropertyBuilder.symbol.size.tooltip", "Set point symbol size"), symbolSizes, 1);
			this.lineWidthComboBox = this.createLineWidthComboBox(i18n.getString("UIPointLayerPropertyBuilder.symbol.linewidth.tooltip", "Set line width"));
			this.symbolColorPicker = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIPointLayerPropertyBuilder.symbol.color.tooltip", "Set symbol color"));

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

			return this.createTitledPane(i18n.getString("UIPointLayerPropertyBuilder.symbol.title", "Symbol properties"), gridPane);
		}

		private Node createHighlightPane()
		{
			GridPane gridPane = this.createGridPane();

			Label highlightColorLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.highlight.color.label", "Highlight color:"));
			highlightColorLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label highlightTypeLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.highlight.type.label", "Highlight type:"));
			highlightTypeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label highlightLineWidthLabel = new Label(i18n.getString("UIPointLayerPropertyBuilder.highlight.linewidth.label", "Line width:"));
			highlightLineWidthLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			this.highlightTypeComboBox = this.createHighlightTypeComboBox(i18n.getString("UIPointLayerPropertyBuilder.highlight.type.tooltip", "Set type of highlighting"), this.HighlightTypeStringConverter);
			this.highlightLineWidthComboBox = this.createLineWidthComboBox(i18n.getString("UIPointLayerPropertyBuilder.highlight.linewidth.tooltip", "Set line width for highlighting"));
			this.highlightColorPicker = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIPointLayerPropertyBuilder.highlight.color.tooltip", "Set color of highlighting"));

			// add listener
			this.highlightColorPicker.valueProperty().addListener(new HighlightColorChangeListener(this));
			this.highlightLineWidthComboBox.getSelectionModel().selectedItemProperty().addListener(new HighlightLineWidthChangeListener(this));
			this.highlightTypeComboBox.getSelectionModel().selectedItemProperty().addListener(new HighlightTypeChangeListener(this));

			highlightColorLabel.setLabelFor(this.highlightColorPicker);
			highlightTypeLabel.setLabelFor(this.highlightTypeComboBox);
			highlightLineWidthLabel.setLabelFor(this.highlightLineWidthComboBox);

			GridPane.setHgrow(highlightColorLabel, Priority.NEVER);
			GridPane.setHgrow(highlightTypeLabel, Priority.NEVER);
			GridPane.setHgrow(highlightLineWidthLabel, Priority.NEVER);

			GridPane.setHgrow(this.highlightColorPicker, Priority.ALWAYS);
			GridPane.setHgrow(this.highlightTypeComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.highlightLineWidthComboBox, Priority.ALWAYS);

			int row = 0;
			gridPane.add(highlightTypeLabel, 0, row);
			gridPane.add(this.highlightTypeComboBox, 1, row++);

			gridPane.add(highlightColorLabel, 0, row);
			gridPane.add(this.highlightColorPicker, 1, row++);

			gridPane.add(highlightLineWidthLabel, 0, row);
			gridPane.add(this.highlightLineWidthComboBox, 1, row++);

			return this.createTitledPane(i18n.getString("UIPointLayerPropertyBuilder.highlight.title", "Highlight properties"), gridPane);
		}

		private Node createPointDimensionVisibilityPane()
		{
			GridPane gridPane = this.createGridPane();

			this.point1DVisibleCheckBox = this.createCheckBox(i18n.getString("UIPointLayerPropertyBuilder.dimension.1d.label", "1D points"), i18n.getString("UIPointLayerPropertyBuilder.dimension.1d.tooltip", "If checked, 1D points will be drawn"));

			this.point2DVisibleCheckBox = this.createCheckBox(i18n.getString("UIPointLayerPropertyBuilder.dimension.2d.label", "2D points"), i18n.getString("UIPointLayerPropertyBuilder.dimension.2d.tooltip", "If checked, 2D points will be drawn"));

			this.point3DVisibleCheckBox = this.createCheckBox(i18n.getString("UIPointLayerPropertyBuilder.dimension.3d.label", "3D points"), i18n.getString("UIPointLayerPropertyBuilder.dimension.3d.tooltip", "If checked, 3D points will be drawn"));

			this.point1DVisibleCheckBox.selectedProperty().addListener(new PointVisibleChangeListener(this, 1));
			this.point2DVisibleCheckBox.selectedProperty().addListener(new PointVisibleChangeListener(this, 2));
			this.point3DVisibleCheckBox.selectedProperty().addListener(new PointVisibleChangeListener(this, 3));

			GridPane.setHgrow(this.point1DVisibleCheckBox, Priority.NEVER);
			GridPane.setHgrow(this.point2DVisibleCheckBox, Priority.NEVER);
			GridPane.setHgrow(this.point3DVisibleCheckBox, Priority.NEVER);

			int row = 0;
			gridPane.add(this.point1DVisibleCheckBox, 0, row++);
			gridPane.add(this.point2DVisibleCheckBox, 0, row++);
			gridPane.add(this.point3DVisibleCheckBox, 0, row++);

			Region spacer = new Region();
			GridPane.setHgrow(spacer, Priority.ALWAYS);
			gridPane.add(spacer, 1, 0, 1, row);

			return this.createTitledPane(i18n.getString("UIPointLayerPropertyBuilder.dimension.title", "Visibility properties"), gridPane);
		}

		private ComboBox<PointSymbolType> createSymbolComboBox(string tooltip)
		{
			ComboBox<PointSymbolType> typeComboBox = new ComboBox<PointSymbolType>();
			typeComboBox.getItems().setAll(PointSymbolType.values());
			typeComboBox.getSelectionModel().select(0);
			typeComboBox.setCellFactory(new CallbackAnonymousInnerClass(this));
			typeComboBox.setButtonCell(new PointSymbolTypeListCell());
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		private class CallbackAnonymousInnerClass : Callback<ListView<PointSymbolType>, ListCell<PointSymbolType>>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public CallbackAnonymousInnerClass(UIPointLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override ListCell<PointSymbolType> call(ListView<PointSymbolType> param)
			{
				return new PointSymbolTypeListCell();
			}
		}

		private void prepareHighlightTypeComboBox(ComboBox<TableRowHighlightType> typeComboBox)
		{
			LinkedHashSet<TableRowHighlightType> supportedHiglightTypes = new LinkedHashSet<TableRowHighlightType>(Arrays.asList(TableRowHighlightType.values()));
			if (this.currentLayer != null)
			{
				switch (this.currentLayer.LayerType.innerEnumValue)
				{
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
					supportedHiglightTypes.remove(TableRowHighlightType.INFLUENCE_ON_POSITION);
					supportedHiglightTypes.remove(TableRowHighlightType.REDUNDANCY);
					break;
				default:
					break;
				}
			}
			typeComboBox.getSelectionModel().clearSelection();
			typeComboBox.getItems().setAll(supportedHiglightTypes);
			typeComboBox.getSelectionModel().select(TableRowHighlightType.NONE);
		}

		private StringConverter<TableRowHighlightType> HighlightTypeStringConverter
		{
			get
			{
				return new StringConverterAnonymousInnerClass(this);
			}
		}

		private class StringConverterAnonymousInnerClass : StringConverter<TableRowHighlightType>
		{
			private readonly UIPointLayerPropertyBuilder outerInstance;

			public StringConverterAnonymousInnerClass(UIPointLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override string toString(TableRowHighlightType type)
			{
				if (type == null)
				{
					return null;
				}
				switch (type.innerEnumValue)
				{
				case TableRowHighlightType.InnerEnum.NONE:
					return outerInstance.i18n.getString("UIPointLayerPropertyBuilder.highlight.type.none.label", "None");
				case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
					return outerInstance.i18n.getString("UIPointLayerPropertyBuilder.highlight.type.test_statistic.label", "Test statistic");
				case TableRowHighlightType.InnerEnum.REDUNDANCY:
					return outerInstance.i18n.getString("UIPointLayerPropertyBuilder.highlight.type.redundancy.label", "Redundancy");
				case TableRowHighlightType.InnerEnum.INFLUENCE_ON_POSITION:
					return outerInstance.i18n.getString("UIPointLayerPropertyBuilder.highlight.type.influence_on_position.label", "Influence on position");
				case TableRowHighlightType.InnerEnum.P_PRIO_VALUE:
					return outerInstance.i18n.getString("UIPointLayerPropertyBuilder.highlight.type.p_prio.label", "p-Value (a-priori)");
				case TableRowHighlightType.InnerEnum.GROSS_ERROR:
					return outerInstance.i18n.getString("UIPointLayerPropertyBuilder.highlight.type.gross_error.label", "Gross error");
				}
				return null;
			}

			public override TableRowHighlightType fromString(string @string)
			{
				return TableRowHighlightType.valueOf(@string);
			}
		}
	}

}