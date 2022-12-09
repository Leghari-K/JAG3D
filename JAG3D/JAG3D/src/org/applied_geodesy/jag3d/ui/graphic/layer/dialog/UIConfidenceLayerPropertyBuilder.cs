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
	using org.applied_geodesy.jag3d.ui.graphic.layer;
	using LayerManager = org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Node = javafx.scene.Node;
	using ColorPicker = javafx.scene.control.ColorPicker;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using VBox = javafx.scene.layout.VBox;
	using Color = javafx.scene.paint.Color;

	public class UIConfidenceLayerPropertyBuilder : UILayerPropertyBuilder
	{

		private class LineWidthChangeListener : ChangeListener<double>
		{
			private readonly UIConfidenceLayerPropertyBuilder outerInstance;

			public LineWidthChangeListener(UIConfidenceLayerPropertyBuilder outerInstance)
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

		private class StrokeColorChangeListener : ChangeListener<Color>
		{
			private readonly UIConfidenceLayerPropertyBuilder outerInstance;

			public StrokeColorChangeListener(UIConfidenceLayerPropertyBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Color oldValue, Color newValue) where T1 : javafx.scene.paint.Color
			{
				if (outerInstance.currentLayer != null && newValue != null && outerInstance.layerManager != null)
				{
					outerInstance.currentLayer.StrokeColor = newValue;
					outerInstance.layerManager.draw();
				}
			}
		}

		private class FillColorChangeListener : ChangeListener<Color>
		{
			private readonly UIConfidenceLayerPropertyBuilder outerInstance;

			public FillColorChangeListener(UIConfidenceLayerPropertyBuilder outerInstance)
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
		private static UIConfidenceLayerPropertyBuilder confidenceLayerPropertyBuilder = new UIConfidenceLayerPropertyBuilder();

		private ComboBox<double> lineWidthComboBox;
		private ColorPicker symbolStrokeColorPicker;
		private ColorPicker symbolFillColorPicker;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private org.applied_geodesy.jag3d.ui.graphic.layer.ConfidenceLayer<?> currentLayer = null;
		private ConfidenceLayer<object> currentLayer = null;
		private VBox propertyPane = null;
		private LayerManager layerManager = null;

		private UIConfidenceLayerPropertyBuilder()
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
			this.propertyPane.setMinWidth(Control.USE_PREF_SIZE);
			this.propertyPane.getChildren().addAll(this.createSymbolPane());
		}

		public static Node getLayerPropertyPane<T1>(LayerManager layerManager, ConfidenceLayer<T1> layer)
		{
			confidenceLayerPropertyBuilder.layerManager = null;
			confidenceLayerPropertyBuilder.init();
			confidenceLayerPropertyBuilder.set(layer);
			confidenceLayerPropertyBuilder.layerManager = layerManager;
			return confidenceLayerPropertyBuilder.propertyPane;
		}

		private void set<T1>(ConfidenceLayer<T1> layer)
		{
			// set new layer
			this.currentLayer = layer;

			// Symbol properties
			this.lineWidthComboBox.getSelectionModel().select(this.currentLayer.LineWidth);
			this.symbolStrokeColorPicker.setValue(this.currentLayer.StrokeColor);
			this.symbolFillColorPicker.setValue(this.currentLayer.Color);
		}

		private Node createSymbolPane()
		{
			GridPane gridPane = this.createGridPane();

			Label symbolStrokeColorLabel = new Label(i18n.getString("UIConfidenceLayerPropertyBuilder.symbol.color.label", "Stroke color:"));
			symbolStrokeColorLabel.setMinWidth(Control.USE_PREF_SIZE);

			Label symbolFillColorLabel = new Label(i18n.getString("UIConfidenceLayerPropertyBuilder.symbol.fill_color.label", "Fill color:"));
			symbolFillColorLabel.setMinWidth(Control.USE_PREF_SIZE);

			Label symbolLineWidthLabel = new Label(i18n.getString("UIConfidenceLayerPropertyBuilder.symbol.linewidth.label", "Line width:"));
			symbolLineWidthLabel.setMinWidth(Control.USE_PREF_SIZE);

			this.lineWidthComboBox = this.createLineWidthComboBox(i18n.getString("UIConfidenceLayerPropertyBuilder.symbol.linewidth.tooltip", "Set line width"));
			this.symbolStrokeColorPicker = this.createColorPicker(Color.BLACK, i18n.getString("UIConfidenceLayerPropertyBuilder.symbol.color.tooltip", "Set stroke color"));
			this.symbolFillColorPicker = this.createColorPicker(Color.LIGHTGREY, i18n.getString("UIConfidenceLayerPropertyBuilder.symbol.fill_color.tooltip", "Set fill color"));

			// add listeners
			this.lineWidthComboBox.getSelectionModel().selectedItemProperty().addListener(new LineWidthChangeListener(this));
			this.symbolFillColorPicker.valueProperty().addListener(new FillColorChangeListener(this));
			this.symbolStrokeColorPicker.valueProperty().addListener(new StrokeColorChangeListener(this));

			symbolStrokeColorLabel.setLabelFor(this.symbolStrokeColorPicker);
			symbolFillColorLabel.setLabelFor(this.symbolFillColorPicker);
			symbolLineWidthLabel.setLabelFor(this.lineWidthComboBox);

			GridPane.setHgrow(symbolStrokeColorLabel, Priority.NEVER);
			GridPane.setHgrow(symbolFillColorLabel, Priority.NEVER);
			GridPane.setHgrow(symbolLineWidthLabel, Priority.NEVER);

			GridPane.setHgrow(this.symbolStrokeColorPicker, Priority.ALWAYS);
			GridPane.setHgrow(this.symbolFillColorPicker, Priority.ALWAYS);
			GridPane.setHgrow(this.lineWidthComboBox, Priority.ALWAYS);

			int row = 0;
			gridPane.add(symbolStrokeColorLabel, 0, row);
			gridPane.add(this.symbolStrokeColorPicker, 1, row++);

			gridPane.add(symbolFillColorLabel, 0, row);
			gridPane.add(this.symbolFillColorPicker, 1, row++);

			gridPane.add(symbolLineWidthLabel, 0, row);
			gridPane.add(this.lineWidthComboBox, 1, row++);

			return this.createTitledPane(i18n.getString("UIConfidenceLayerPropertyBuilder.symbol.title", "Symbol properties"), gridPane);
		}
	}

}