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

namespace org.applied_geodesy.jag3d.ui.graphic.layer.dialog
{

	using LayerManager = org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager;
	using ObservationLayer = org.applied_geodesy.jag3d.ui.graphic.layer.ObservationLayer;
	using ObservationSymbolProperties = org.applied_geodesy.jag3d.ui.graphic.layer.ObservationSymbolProperties;
	using ObservationType = org.applied_geodesy.jag3d.ui.graphic.layer.ObservationSymbolProperties.ObservationType;
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
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using VBox = javafx.scene.layout.VBox;
	using Color = javafx.scene.paint.Color;
	using StringConverter = javafx.util.StringConverter;

	public class UIObservationLayerPropertyBuilder : UILayerPropertyBuilder
	{
		private class SymbolSizeChangeListener : ChangeListener<double>
		{
			private readonly UIObservationLayerPropertyBuilder outerInstance;

			public SymbolSizeChangeListener(UIObservationLayerPropertyBuilder outerInstance)
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
			private readonly UIObservationLayerPropertyBuilder outerInstance;

			public LineWidthChangeListener(UIObservationLayerPropertyBuilder outerInstance)
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
			private readonly UIObservationLayerPropertyBuilder outerInstance;

			public ColorChangeListener(UIObservationLayerPropertyBuilder outerInstance)
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

		private class HighlightColorChangeListener : ChangeListener<Color>
		{
			private readonly UIObservationLayerPropertyBuilder outerInstance;

			public HighlightColorChangeListener(UIObservationLayerPropertyBuilder outerInstance)
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

		private class HighlightTypeChangeListener : ChangeListener<TableRowHighlightType>
		{
			private readonly UIObservationLayerPropertyBuilder outerInstance;

			public HighlightTypeChangeListener(UIObservationLayerPropertyBuilder outerInstance)
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

		private class HighlightLineWidthChangeListener : ChangeListener<double>
		{
			private readonly UIObservationLayerPropertyBuilder outerInstance;

			public HighlightLineWidthChangeListener(UIObservationLayerPropertyBuilder outerInstance)
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

		private class ObservationColorChangeListener : ChangeListener<Color>
		{
			private readonly UIObservationLayerPropertyBuilder outerInstance;

			internal ObservationSymbolProperties.ObservationType observationType;
			internal ObservationColorChangeListener(UIObservationLayerPropertyBuilder outerInstance, ObservationSymbolProperties.ObservationType observationType)
			{
				this.outerInstance = outerInstance;
				this.observationType = observationType;
			}
			public override void changed<T1>(ObservableValue<T1> observable, Color oldValue, Color newValue) where T1 : javafx.scene.paint.Color
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					ObservationSymbolProperties properties = outerInstance.currentLayer.getObservationSymbolProperties(this.observationType);
					if (properties != null)
					{
						properties.Color = newValue;
						outerInstance.layerManager.draw();
					}
				}
			}
		}

		private class ObservationEnableChangeListener : ChangeListener<bool>
		{
			private readonly UIObservationLayerPropertyBuilder outerInstance;

			internal ObservationSymbolProperties.ObservationType observationType;
			internal ObservationEnableChangeListener(UIObservationLayerPropertyBuilder outerInstance, ObservationSymbolProperties.ObservationType observationType)
			{
				this.outerInstance = outerInstance;
				this.observationType = observationType;
			}
			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					ObservationSymbolProperties properties = outerInstance.currentLayer.getObservationSymbolProperties(this.observationType);
					if (properties != null)
					{
						properties.Visible = newValue.Value;
						outerInstance.layerManager.draw();
					}
				}
			}
		}

		private I18N i18n = I18N.Instance;
		private static UIObservationLayerPropertyBuilder observationLayerPropertyBuilder = new UIObservationLayerPropertyBuilder();

		private ComboBox<double> symbolSizeComboBox;
		private ComboBox<double> lineWidthComboBox;
		private ColorPicker symbolColorPicker;

		private Node highlightPane = null;
		private ComboBox<double> highlightLineWidthComboBox;
		private ComboBox<TableRowHighlightType> highlightTypeComboBox;
		private ColorPicker highlightColorPicker;

		private IDictionary<ObservationSymbolProperties.ObservationType, ColorPicker> observationColorPickerMap = new Dictionary<ObservationSymbolProperties.ObservationType, ColorPicker>();
		private IDictionary<ObservationSymbolProperties.ObservationType, CheckBox> observationCheckBoxMap = new Dictionary<ObservationSymbolProperties.ObservationType, CheckBox>();

		private ObservationLayer currentLayer = null;
		private LayerManager layerManager = null;

		private VBox propertyPane = null;

		private UIObservationLayerPropertyBuilder()
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
			this.propertyPane.setMinWidth(Control.USE_PREF_SIZE);
			this.propertyPane.getChildren().addAll(this.createSymbolPane(), this.createObservationPaintPane(), this.highlightPane);
		}

		public static Node getLayerPropertyPane(LayerManager layerManager, ObservationLayer layer)
		{
			observationLayerPropertyBuilder.layerManager = null;
			observationLayerPropertyBuilder.init();
			observationLayerPropertyBuilder.set(layer);
			observationLayerPropertyBuilder.layerManager = layerManager;
			return observationLayerPropertyBuilder.propertyPane;
		}

		private void set(ObservationLayer layer)
		{
			// set new layer
			this.currentLayer = layer;

			// Symbol properties
			this.symbolSizeComboBox.getSelectionModel().select(this.currentLayer.SymbolSize);
			this.lineWidthComboBox.getSelectionModel().select(this.currentLayer.LineWidth);
			this.symbolColorPicker.setValue(this.currentLayer.Color);

			switch (this.currentLayer.LayerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
				this.highlightColorPicker.setValue(this.currentLayer.HighlightColor);
				this.highlightLineWidthComboBox.getSelectionModel().select(this.currentLayer.HighlightLineWidth);
				this.highlightTypeComboBox.getSelectionModel().select(this.currentLayer.HighlightType);
				this.highlightPane.setVisible(true);
				this.highlightPane.setManaged(true);
				break;

			default:
				this.highlightPane.setVisible(false);
				this.highlightPane.setManaged(false);
				break;
			}

			ObservationSymbolProperties.ObservationType[] observationTypes = ObservationSymbolProperties.ObservationType.values();
			foreach (ObservationSymbolProperties.ObservationType observationType in observationTypes)
			{
				ObservationSymbolProperties properties = this.currentLayer.getObservationSymbolProperties(observationType);
				CheckBox checkBox = this.observationCheckBoxMap[observationType];
				ColorPicker colorPicker = this.observationColorPickerMap[observationType];

				checkBox.setSelected(properties.Visible);
				colorPicker.setValue(properties.Color);
			}
		}

		private Node createObservationPaintPane()
		{
			GridPane gridPane = this.createGridPane();

			ObservationSymbolProperties.ObservationType[] observationTypes = ObservationSymbolProperties.ObservationType.values();
			foreach (ObservationSymbolProperties.ObservationType observationType in observationTypes)
			{
				switch (observationType.innerEnumValue)
				{
				case ObservationSymbolProperties.ObservationType.InnerEnum.LEVELING:
					this.observationCheckBoxMap[observationType] = this.createCheckBox(i18n.getString("UIObservationLayerPropertyBuilder.symbol.leveling.enable.label", "Leveling:"), i18n.getString("UIObservationLayerPropertyBuilder.symbol.leveling.enable.tooltip", "If checked, leveling will be drawn"));
					this.observationColorPickerMap[observationType] = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIObservationLayerPropertyBuilder.color.leveling.tooltip", "Set color of leveling data"));
					break;

				case ObservationSymbolProperties.ObservationType.InnerEnum.DIRECTION:
					this.observationCheckBoxMap[observationType] = this.createCheckBox(i18n.getString("UIObservationLayerPropertyBuilder.symbol.direction.enable.label", "Direction:"), i18n.getString("UIObservationLayerPropertyBuilder.symbol.direction.enable.tooltip", "If checked, directions will be drawn"));
					this.observationColorPickerMap[observationType] = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIObservationLayerPropertyBuilder.color.direction.tooltip", "Set color of direction measurements"));
					break;

				case ObservationSymbolProperties.ObservationType.InnerEnum.DISTANCE:
					this.observationCheckBoxMap[observationType] = this.createCheckBox(i18n.getString("UIObservationLayerPropertyBuilder.symbol.distance.enable.label", "Distance:"), i18n.getString("UIObservationLayerPropertyBuilder.symbol.distance.enable.tooltip", "If checked, distances will be drawn"));
					this.observationColorPickerMap[observationType] = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIObservationLayerPropertyBuilder.color.distance.tooltip", "Set color of distance measurements"));
					break;
				case ObservationSymbolProperties.ObservationType.InnerEnum.ZENITH_ANGLE:
					this.observationCheckBoxMap[observationType] = this.createCheckBox(i18n.getString("UIObservationLayerPropertyBuilder.symbol.zenith_angle.enable.label", "Zenith angle:"), i18n.getString("UIObservationLayerPropertyBuilder.symbol.zenith_angle.enable.tooltip", "If checked, zenith angles will be drawn"));
					this.observationColorPickerMap[observationType] = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIObservationLayerPropertyBuilder.color.zenith_angle.tooltip", "Set color of zenith angle measurements"));
					break;

				case ObservationSymbolProperties.ObservationType.InnerEnum.GNSS:
					this.observationCheckBoxMap[observationType] = this.createCheckBox(i18n.getString("UIObservationLayerPropertyBuilder.symbol.gnss.enable.label", "GNSS Baseline:"), i18n.getString("UIObservationLayerPropertyBuilder.symbol.gnss.enable.tooltip", "If checked, GNSS baselines will be drawn"));
					this.observationColorPickerMap[observationType] = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIObservationLayerPropertyBuilder.color.gnss.tooltip", "Set color of GNSS measurements"));
					break;
				}
			}

			int row = 0;
			foreach (ObservationSymbolProperties.ObservationType observationType in observationTypes)
			{
				CheckBox checkBox = this.observationCheckBoxMap[observationType];
				ColorPicker colorPicker = this.observationColorPickerMap[observationType];

				checkBox.selectedProperty().addListener(new ObservationEnableChangeListener(this, observationType));
				colorPicker.valueProperty().addListener(new ObservationColorChangeListener(this, observationType));

				GridPane.setHgrow(checkBox, Priority.NEVER);
				GridPane.setHgrow(colorPicker, Priority.ALWAYS);

				gridPane.add(checkBox, 0, row);
				gridPane.add(colorPicker, 1, row++);
			}

			return this.createTitledPane(i18n.getString("UIObservationLayerPropertyBuilder.observation.title", "Observation properties"), gridPane);
		}

		private Node createSymbolPane()
		{
			GridPane gridPane = this.createGridPane();

			Label symbolSizeLabel = new Label(i18n.getString("UIObservationLayerPropertyBuilder.symbol.size.label", "Symbol size:"));
			symbolSizeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label symbolColorLabel = new Label(i18n.getString("UIObservationLayerPropertyBuilder.symbol.color.label", "Symbol color:"));
			symbolColorLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label symbolLineWidthLabel = new Label(i18n.getString("UIObservationLayerPropertyBuilder.symbol.linewidth.label", "Line width:"));
			symbolLineWidthLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			double?[] symbolSizes = new double?[21];
			for (int i = 0; i < symbolSizes.Length; i++)
			{
				symbolSizes[i] = 5 + 0.5 * i;
			}

			this.symbolSizeComboBox = this.createSizeComboBox(i18n.getString("UIObservationLayerPropertyBuilder.symbol.size.tooltip", "Set symbol size"), symbolSizes, 1);
			this.lineWidthComboBox = this.createLineWidthComboBox(i18n.getString("UIObservationLayerPropertyBuilder.symbol.linewidth.tooltip", "Set line width"));
			this.symbolColorPicker = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIObservationLayerPropertyBuilder.symbol.color.tooltip", "Set symbol color"));

			// add listeners
			this.symbolSizeComboBox.getSelectionModel().selectedItemProperty().addListener(new SymbolSizeChangeListener(this));
			this.lineWidthComboBox.getSelectionModel().selectedItemProperty().addListener(new LineWidthChangeListener(this));
			this.symbolColorPicker.valueProperty().addListener(new ColorChangeListener(this));

			symbolSizeLabel.setLabelFor(this.symbolSizeComboBox);
			symbolColorLabel.setLabelFor(this.symbolColorPicker);
			symbolLineWidthLabel.setLabelFor(this.lineWidthComboBox);

			GridPane.setHgrow(symbolSizeLabel, Priority.NEVER);
			GridPane.setHgrow(symbolColorLabel, Priority.NEVER);
			GridPane.setHgrow(symbolLineWidthLabel, Priority.NEVER);

			GridPane.setHgrow(this.symbolSizeComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.symbolColorPicker, Priority.ALWAYS);
			GridPane.setHgrow(this.lineWidthComboBox, Priority.ALWAYS);

			int row = 0;
			gridPane.add(symbolSizeLabel, 0, row);
			gridPane.add(this.symbolSizeComboBox, 1, row++);

			gridPane.add(symbolColorLabel, 0, row);
			gridPane.add(this.symbolColorPicker, 1, row++);

			gridPane.add(symbolLineWidthLabel, 0, row);
			gridPane.add(this.lineWidthComboBox, 1, row++);

			return this.createTitledPane(i18n.getString("UIObservationLayerPropertyBuilder.symbol.title", "Symbol properties"), gridPane);
		}

		private Node createHighlightPane()
		{
			GridPane gridPane = this.createGridPane();

			Label highlightColorLabel = new Label(i18n.getString("UIObservationLayerPropertyBuilder.highlight.color.label", "Highlight color:"));
			highlightColorLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label highlightTypeLabel = new Label(i18n.getString("UIObservationLayerPropertyBuilder.highlight.type.label", "Highlight type:"));
			highlightTypeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label highlightLineWidthLabel = new Label(i18n.getString("UIObservationLayerPropertyBuilder.highlight.linewidth.label", "Line width:"));
			highlightLineWidthLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			this.highlightTypeComboBox = this.createHighlightTypeComboBox(i18n.getString("UIObservationLayerPropertyBuilder.highlight.type.tooltip", "Set type of highlighting"), this.HighlightTypeStringConverter);
			this.highlightLineWidthComboBox = this.createLineWidthComboBox(i18n.getString("UIObservationLayerPropertyBuilder.highlight.linewidth.tooltip", "Set line width for highlighting"));
			this.highlightColorPicker = this.createColorPicker(Color.DARKBLUE, i18n.getString("UIObservationLayerPropertyBuilder.highlight.color.tooltip", "Set color of highlighting"));

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

			return this.createTitledPane(i18n.getString("UIObservationLayerPropertyBuilder.highlight.title", "Highlight properties"), gridPane);
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
			private readonly UIObservationLayerPropertyBuilder outerInstance;

			public StringConverterAnonymousInnerClass(UIObservationLayerPropertyBuilder outerInstance)
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
					return outerInstance.i18n.getString("UIObservationLayerPropertyBuilder.highlight.type.none.label", "None");
				case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
					return outerInstance.i18n.getString("UIObservationLayerPropertyBuilder.highlight.type.test_statistic.label", "Test statistic");
				case TableRowHighlightType.InnerEnum.REDUNDANCY:
					return outerInstance.i18n.getString("UIObservationLayerPropertyBuilder.highlight.type.redundancy.label", "Redundancy");
				case TableRowHighlightType.InnerEnum.INFLUENCE_ON_POSITION:
					return outerInstance.i18n.getString("UIObservationLayerPropertyBuilder.highlight.type.influence_on_position.label", "Influence on position");
				case TableRowHighlightType.InnerEnum.P_PRIO_VALUE:
					return outerInstance.i18n.getString("UIObservationLayerPropertyBuilder.highlight.type.p_prio.label", "p-Value (a-priori)");
				case TableRowHighlightType.InnerEnum.GROSS_ERROR:
					return outerInstance.i18n.getString("UIObservationLayerPropertyBuilder.highlight.type.gross_error.label", "Gross error");
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