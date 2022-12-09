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

namespace org.applied_geodesy.jag3d.ui.graphic.layer
{

	using ProjectDatabaseStateChangeListener = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateChangeListener;
	using ProjectDatabaseStateEvent = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateEvent;
	using ProjectDatabaseStateType = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using JAG3D = org.applied_geodesy.jag3d.ui.JAG3D;
	using FeatureZoomDialog = org.applied_geodesy.jag3d.ui.graphic.layer.dialog.FeatureZoomDialog;
	using LayerManagerDialog = org.applied_geodesy.jag3d.ui.graphic.layer.dialog.LayerManagerDialog;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;
	using SQLGraphicManager = org.applied_geodesy.jag3d.ui.graphic.sql.SQLGraphicManager;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DefaultFileChooser = org.applied_geodesy.ui.io.DefaultFileChooser;
	using ImageUtils = org.applied_geodesy.util.ImageUtils;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using Bindings = javafx.beans.binding.Bindings;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ObservableList = javafx.collections.ObservableList;
	using SwingFXUtils = javafx.embed.swing.SwingFXUtils;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using SnapshotParameters = javafx.scene.SnapshotParameters;
	using Button = javafx.scene.control.Button;
	using ContentDisplay = javafx.scene.control.ContentDisplay;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using Separator = javafx.scene.control.Separator;
	using Spinner = javafx.scene.control.Spinner;
	using SpinnerValueFactory = javafx.scene.control.SpinnerValueFactory;
	using TextFormatter = javafx.scene.control.TextFormatter;
	using Toggle = javafx.scene.control.Toggle;
	using ToggleButton = javafx.scene.control.ToggleButton;
	using ToggleGroup = javafx.scene.control.ToggleGroup;
	using ToolBar = javafx.scene.control.ToolBar;
	using Tooltip = javafx.scene.control.Tooltip;
	using ImageView = javafx.scene.image.ImageView;
	using WritableImage = javafx.scene.image.WritableImage;
	using Background = javafx.scene.layout.Background;
	using BackgroundFill = javafx.scene.layout.BackgroundFill;
	using CornerRadii = javafx.scene.layout.CornerRadii;
	using Pane = javafx.scene.layout.Pane;
	using StackPane = javafx.scene.layout.StackPane;
	using Color = javafx.scene.paint.Color;
	using TextAlignment = javafx.scene.text.TextAlignment;
	using Transform = javafx.scene.transform.Transform;
	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;
	using StringConverter = javafx.util.StringConverter;

	public class LayerManager
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			resizableGraphicCanvas = new ResizableCanvas(this.currentGraphicExtent);
			layers = this.resizableGraphicCanvas.Layers;
			mouseNavigationCanvas = new MouseNavigationCanvas(this);
			visibleChangeListener = new VisiblePropertyChangeListener(this);
		}

		private class DatabaseStateChangeListener : ProjectDatabaseStateChangeListener
		{
			private readonly LayerManager outerInstance;

			public DatabaseStateChangeListener(LayerManager outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void projectDatabaseStateChanged(ProjectDatabaseStateEvent evt)
			{
				if (outerInstance.layerToolbar != null)
				{
					bool disable = evt.EventType != ProjectDatabaseStateType.OPENED;
					outerInstance.layerToolbar.setDisable(disable);

					if (evt.EventType == ProjectDatabaseStateType.CLOSED)
					{
						outerInstance.currentGraphicExtent.set(Double.NaN, Double.NaN, Double.NaN, Double.NaN);
						outerInstance.clearAllLayers();
						outerInstance.draw();
					}
				}
			}
		}

		private class ToolbarActionEventHandler : EventHandler<ActionEvent>
		{
			private readonly LayerManager outerInstance;

			public ToolbarActionEventHandler(LayerManager outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				ToolbarType type = ToolbarType.NONE;
				if (@event.getSource() is Button && ((Button)@event.getSource()).getUserData() is ToolbarType)
				{
					Button button = (Button)@event.getSource();
					type = (ToolbarType)button.getUserData();
				}
				else if (@event.getSource() is ToggleButton && ((ToggleButton)@event.getSource()).getUserData() is ToolbarType)
				{
					ToggleButton button = (ToggleButton)@event.getSource();
					type = button.isSelected() ? (ToolbarType)button.getUserData() : ToolbarType.NONE;
				}
				else
				{
					return;
				}

				outerInstance.action(type);
			}
		}

		private class ScaleChangeListener : ChangeListener<double>
		{
			private readonly LayerManager outerInstance;

			internal Layer[] layers;
			internal ScaleChangeListener(LayerManager outerInstance, params Layer[] layer)
			{
				this.outerInstance = outerInstance;
				this.layers = layer;
			}
			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (newValue != null && !double.IsInfinity(newValue) && !double.IsNaN(newValue))
				{
					foreach (Layer layer in layers)
					{
						if (layer.LayerType == LayerType.POINT_SHIFT_HORIZONTAL || layer.LayerType == LayerType.POINT_SHIFT_VERTICAL || layer.LayerType == LayerType.PRINCIPAL_COMPONENT_HORIZONTAL || layer.LayerType == LayerType.PRINCIPAL_COMPONENT_VERTICAL || layer.LayerType == LayerType.POINT_RESIDUAL_HORIZONTAL || layer.LayerType == LayerType.POINT_RESIDUAL_VERTICAL)
						{
							((ArrowLayer)layer).VectorScale = newValue.Value;
						}
						else
						{
							if (layer.LayerType == LayerType.ABSOLUTE_CONFIDENCE)
							{
								((AbsoluteConfidenceLayer)layer).ConfidenceScale = newValue.Value;
							}
						}
					}
					outerInstance.save(newValue.Value);
					if (!outerInstance.ignoreChangeEvent)
					{
						outerInstance.draw();
					}
				}
			}
		}

		private class VisiblePropertyChangeListener : ChangeListener<bool>
		{
			private readonly LayerManager outerInstance;

			public VisiblePropertyChangeListener(LayerManager outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (!outerInstance.ignoreChangeEvent)
				{
					outerInstance.draw();
				}
			}
		}

		private class ToggleChangeListener : ChangeListener<Toggle>
		{
			private readonly LayerManager outerInstance;

			public ToggleChangeListener(LayerManager outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Toggle oldValue, Toggle newValue) where T1 : javafx.scene.control.Toggle
			{
				if (newValue == null && oldValue != null)
				{
					oldValue.setSelected(true);
				}
			}
		}

		private class GraphicExtentChangeListener : ChangeListener<bool>
		{
			private readonly LayerManager outerInstance;

			public GraphicExtentChangeListener(LayerManager outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				outerInstance.save(outerInstance.currentGraphicExtent);
			}
		}

		private Label coordinatePanel = new Label();
		private I18N i18n = I18N.Instance;
		private ToolBar layerToolbar = new ToolBar();
		private readonly GraphicExtent currentGraphicExtent = new GraphicExtent();
		private ResizableCanvas resizableGraphicCanvas;
		private ObservableList<Layer> layers;
		private StackPane stackPane = new StackPane(); // Mouse- and Plotlayers
		private MouseNavigationCanvas mouseNavigationCanvas;
	//	private ObjectProperty<Color> color = new SimpleObjectProperty<Color>(Color.rgb(255, 255, 255, 1.0)); //0-255
		private VisiblePropertyChangeListener visibleChangeListener;
		private bool ignoreChangeEvent = false;

		private Spinner<double> scaleSpinner;
		public LayerManager()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.init();
		}

		private void init()
		{
			this.initPane();
			this.initToolBar();
			this.initCoordinateLabel();

			this.initLayers();

	//		this.color.addListener(new ColorChangeListener());
			this.currentGraphicExtent.drawingBoardWidthProperty().bind(this.stackPane.widthProperty());
			this.currentGraphicExtent.drawingBoardHeightProperty().bind(this.stackPane.heightProperty());

			GraphicExtentChangeListener graphicExtentChangeListener = new GraphicExtentChangeListener(this);
	//		this.currentGraphicExtent.minXProperty().addListener(graphicExtentChangeListener);
	//		this.currentGraphicExtent.maxXProperty().addListener(graphicExtentChangeListener);
	//		this.currentGraphicExtent.minYProperty().addListener(graphicExtentChangeListener);
	//		this.currentGraphicExtent.maxYProperty().addListener(graphicExtentChangeListener);
			this.currentGraphicExtent.extendedProperty().addListener(graphicExtentChangeListener);

			SQLManager.Instance.addProjectDatabaseStateChangeListener(new DatabaseStateChangeListener(this));
		}

		private void initLayers()
		{
			LayerType[] layerTypes = LayerType.values();

			foreach (LayerType layerType in layerTypes)
			{
				Layer layer = null;

				// create/add layer
				switch (layerType.innerEnumValue)
				{
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.STOCHASTIC_POINT_APRIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.DATUM_POINT_APRIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.NEW_POINT_APRIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
					layer = new PointLayer(layerType);
					break;

				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.OBSERVATION_APRIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
					layer = new ObservationLayer(layerType);
					break;

				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.PRINCIPAL_COMPONENT_VERTICAL:
					layer = new PrincipalComponentArrowLayer(layerType);
					break;

				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_RESIDUAL_HORIZONTAL:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_RESIDUAL_VERTICAL:
					layer = new PointResidualArrowLayer(layerType);
					break;

				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_SHIFT_HORIZONTAL:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_SHIFT_VERTICAL:
					layer = new PointShiftArrowLayer(layerType);
					break;

				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.ABSOLUTE_CONFIDENCE:
					layer = new AbsoluteConfidenceLayer(layerType);
					break;

				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.RELATIVE_CONFIDENCE:
					layer = new RelativeConfidenceLayer(layerType);
					break;

				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.LEGEND:
					layer = new LegendLayer(layerType, this.layers);
					break;
				}

				if (layer != null)
				{
					this.add(layer);
				}
			}

			AbsoluteConfidenceLayer absoluteConfidenceLayer = (AbsoluteConfidenceLayer)this.getLayer(LayerType.ABSOLUTE_CONFIDENCE);
			absoluteConfidenceLayer.addAll((PointLayer)this.getLayer(LayerType.STOCHASTIC_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.DATUM_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.NEW_POINT_APOSTERIORI));


			PrincipalComponentArrowLayer principalComponentHorizontalArrowLayer = (PrincipalComponentArrowLayer)this.getLayer(LayerType.PRINCIPAL_COMPONENT_HORIZONTAL);
			principalComponentHorizontalArrowLayer.addAll((PointLayer)this.getLayer(LayerType.STOCHASTIC_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.DATUM_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.NEW_POINT_APOSTERIORI));

			PrincipalComponentArrowLayer principalComponentVerticalArrowLayer = (PrincipalComponentArrowLayer)this.getLayer(LayerType.PRINCIPAL_COMPONENT_VERTICAL);
			principalComponentVerticalArrowLayer.addAll((PointLayer)this.getLayer(LayerType.STOCHASTIC_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.DATUM_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.NEW_POINT_APOSTERIORI));

			PointResidualArrowLayer pointResidualHorizontalArrowLayer = (PointResidualArrowLayer)this.getLayer(LayerType.POINT_RESIDUAL_HORIZONTAL);
			pointResidualHorizontalArrowLayer.addAll((PointLayer)this.getLayer(LayerType.STOCHASTIC_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.DATUM_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.NEW_POINT_APOSTERIORI));

			PointResidualArrowLayer pointResidualVerticalArrowLayer = (PointResidualArrowLayer)this.getLayer(LayerType.POINT_RESIDUAL_VERTICAL);
			pointResidualVerticalArrowLayer.addAll((PointLayer)this.getLayer(LayerType.STOCHASTIC_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.DATUM_POINT_APOSTERIORI), (PointLayer)this.getLayer(LayerType.NEW_POINT_APOSTERIORI));

			PointShiftArrowLayer pointShiftHorizontalArrowLayer = (PointShiftArrowLayer)this.getLayer(LayerType.POINT_SHIFT_HORIZONTAL);
			PointShiftArrowLayer pointShiftVerticalArrowLayer = (PointShiftArrowLayer)this.getLayer(LayerType.POINT_SHIFT_VERTICAL);
			RelativeConfidenceLayer relativeConfidenceLayer = (RelativeConfidenceLayer)this.getLayer(LayerType.RELATIVE_CONFIDENCE);
			relativeConfidenceLayer.add(pointShiftHorizontalArrowLayer);
			relativeConfidenceLayer.add(pointShiftVerticalArrowLayer);


			// add scale change listener
			pointShiftHorizontalArrowLayer.VectorScale = this.scaleSpinner.getValueFactory().getValue();
			pointShiftVerticalArrowLayer.VectorScale = this.scaleSpinner.getValueFactory().getValue();
			principalComponentHorizontalArrowLayer.VectorScale = this.scaleSpinner.getValueFactory().getValue();
			principalComponentVerticalArrowLayer.VectorScale = this.scaleSpinner.getValueFactory().getValue();
			pointResidualHorizontalArrowLayer.VectorScale = this.scaleSpinner.getValueFactory().getValue();
			pointResidualVerticalArrowLayer.VectorScale = this.scaleSpinner.getValueFactory().getValue();
			absoluteConfidenceLayer.ConfidenceScale = this.scaleSpinner.getValueFactory().getValue();
			this.scaleSpinner.valueProperty().addListener(new ScaleChangeListener(this, pointShiftHorizontalArrowLayer, pointShiftVerticalArrowLayer, principalComponentHorizontalArrowLayer, principalComponentVerticalArrowLayer, pointResidualHorizontalArrowLayer, pointResidualVerticalArrowLayer, absoluteConfidenceLayer));

			// Bind apriori symbol size of point to observation layer
			((ObservationLayer)this.getLayer(LayerType.OBSERVATION_APRIORI)).pointSymbolSizeProperty().bind(Bindings.max(Bindings.max(Bindings.max(this.getLayer(LayerType.REFERENCE_POINT_APRIORI).symbolSizeProperty(), this.getLayer(LayerType.STOCHASTIC_POINT_APRIORI).symbolSizeProperty()), this.getLayer(LayerType.DATUM_POINT_APRIORI).symbolSizeProperty()), this.getLayer(LayerType.NEW_POINT_APRIORI).symbolSizeProperty()));

			// Bind aposteriori symbol size of point to observation layer
			((ObservationLayer)this.getLayer(LayerType.OBSERVATION_APOSTERIORI)).pointSymbolSizeProperty().bind(Bindings.max(Bindings.max(Bindings.max(this.getLayer(LayerType.REFERENCE_POINT_APOSTERIORI).symbolSizeProperty(), this.getLayer(LayerType.STOCHASTIC_POINT_APOSTERIORI).symbolSizeProperty()), this.getLayer(LayerType.DATUM_POINT_APOSTERIORI).symbolSizeProperty()), this.getLayer(LayerType.NEW_POINT_APOSTERIORI).symbolSizeProperty()));
		}

		private void add(Layer layer)
		{
			this.currentGraphicExtent.merge(layer.MaximumGraphicExtent);
			layer.visibleProperty().addListener(this.visibleChangeListener);
			// add to List
			this.layers.add(layer);
		}

		public virtual GraphicExtent CurrentGraphicExtent
		{
			get
			{
				return this.currentGraphicExtent;
			}
		}

		public virtual void clearAllLayers()
		{
			foreach (Layer layer in this.layers)
			{
				layer.clearLayer();
			}
		}

		public virtual Layer getLayer(LayerType layerType)
		{
			foreach (Layer layer in this.layers)
			{
				if (layer.LayerType == layerType)
				{
					return layer;
				}
			}
			return null;
		}

		public virtual void draw()
		{
			this.resizableGraphicCanvas.draw();
			this.mouseNavigationCanvas.draw();
		}

		public virtual void reorderLayer(IList<Layer> layers)
		{
			this.layers.clear();

			foreach (Layer layer in layers)
			{
				if (layer != null && layer.LayerType != null)
				{
					this.layers.add(layer);
				}
			}
		}

		public virtual Pane Pane
		{
			get
			{
				return this.stackPane;
			}
		}

		public virtual ToolBar ToolBar
		{
			get
			{
				return this.layerToolbar;
			}
		}

		public virtual Label CoordinateLabel
		{
			get
			{
				return this.coordinatePanel;
			}
		}

		public virtual double EllipseScale
		{
			set
			{
				this.scaleSpinner.getValueFactory().setValue(value);
			}
		}

		private void initPane()
		{
			//BackgroundFill fill = new BackgroundFill(this.getColor(), CornerRadii.EMPTY, Insets.EMPTY);
			BackgroundFill fill = new BackgroundFill(Color.TRANSPARENT, CornerRadii.EMPTY, Insets.EMPTY);
			Background background = new Background(fill);

			this.stackPane.setBackground(background);
			this.stackPane.setMinSize(0, 0);
			this.stackPane.setMaxSize(double.MaxValue, double.MaxValue);

			this.stackPane.getChildren().addAll(this.resizableGraphicCanvas, this.mouseNavigationCanvas);
		}

		private void initCoordinateLabel()
		{
			this.coordinatePanel.setTextAlignment(TextAlignment.RIGHT);
		}

		private void initToolBar()
		{
			ToolbarActionEventHandler toolbarActionEventHandler = new ToolbarActionEventHandler(this);
			ToggleGroup group = new ToggleGroup();

			ToggleButton moveButton = this.createToggleButton("graphic_move_32x32.png", i18n.getString("LayerManager.toolbar.button.move.tooltip", "Move graphic"), ToolbarType.MOVE, toolbarActionEventHandler);

			ToggleButton windowZoomButton = this.createToggleButton("graphic_window_zoom_32x32.png", i18n.getString("LayerManager.toolbar.button.window_zoom.tooltip", "Window zoom"), ToolbarType.WINDOW_ZOOM, toolbarActionEventHandler);

			Button zoomInButton = this.createButton("graphic_zoom_in_32x32.png", i18n.getString("LayerManager.toolbar.button.zoom_in.tooltip", "Zoom in"), ToolbarType.ZOOM_IN, toolbarActionEventHandler);

			Button zoomOutButton = this.createButton("graphic_zoom_out_32x32.png", i18n.getString("LayerManager.toolbar.button.zoom_out.tooltip", "Zoom out"), ToolbarType.ZOOM_OUT, toolbarActionEventHandler);

			Button expandButton = this.createButton("graphic_expand_32x32.png", i18n.getString("LayerManager.toolbar.button.expand.tooltip", "Expand graphic"), ToolbarType.EXPAND, toolbarActionEventHandler);

			Button redrawButton = this.createButton("graphic_refresh_32x32.png", i18n.getString("LayerManager.toolbar.button.refresh.tooltip", "Refresh graphic from database"), ToolbarType.REDRAW, toolbarActionEventHandler);

			Button layerButton = this.createButton("graphic_layer_32x32.png", i18n.getString("LayerManager.toolbar.button.layers.tooltip", "Layer properties"), ToolbarType.LAYER_PROPERTIES, toolbarActionEventHandler);

			Button exportButton = this.createButton("graphic_export_32x32.png", i18n.getString("LayerManager.toolbar.button.export.tooltip", "Export graphic"), ToolbarType.EXPORT, toolbarActionEventHandler);

			Button featureZoomButton = this.createButton("graphic_feature_zoom_32x32.png", i18n.getString("LayerManager.toolbar.button.featurezoom.tooltip", "Zoom to feature"), ToolbarType.FEATURE_ZOOM, toolbarActionEventHandler);

			this.scaleSpinner = this.createDoubleSpinner(1, double.MaxValue, 5000, 250, i18n.getString("LayerManager.toolbar.scale.tooltip", "Scale of arrows and confidences"), 0);
			this.scaleSpinner.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			Label scaleLabel = new Label(" : 1");
			scaleLabel.setGraphic(this.scaleSpinner);
			scaleLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			scaleLabel.setContentDisplay(ContentDisplay.LEFT);

			group.getToggles().addAll(moveButton, windowZoomButton);

			group.selectedToggleProperty().addListener(new ToggleChangeListener(this));

			this.layerToolbar.getItems().addAll(moveButton, windowZoomButton, new Separator(), zoomInButton, zoomOutButton, new Separator(), expandButton, redrawButton, featureZoomButton, exportButton, new Separator(), layerButton, new Separator(), scaleLabel);

			bool disable = !SQLManager.Instance.hasDatabase();
			windowZoomButton.setSelected(true); // pre-selection
			action(ToolbarType.WINDOW_ZOOM);
			this.layerToolbar.setDisable(disable);
		}

		private void action(ToolbarType type)
		{
			switch (type)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.EXPAND:
				this.expand();
				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.REDRAW:
				this.redraw();
				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.ZOOM_IN:
				this.zoomIn();
				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.ZOOM_OUT:
				this.zoomOut();
				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.EXPORT:
				saveSnapshot();
				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.LAYER_PROPERTIES:
				layerProperties();
				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.FEATURE_ZOOM:
				featureZoom();
				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.NONE:
			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.MOVE:
			case org.applied_geodesy.jag3d.ui.graphic.layer.ToolbarType.WINDOW_ZOOM:
				this.mouseNavigationCanvas.ToolbarType = type;
				break;

			}
		}

		public virtual void redraw()
		{
			SQLGraphicManager sqlGraphicManager = SQLManager.Instance.SQLGraphicManager;
			if (sqlGraphicManager == null)
			{
				return;
			}

			try
			{
				this.ignoreChangeEvent = true;
				sqlGraphicManager.load(this);
				sqlGraphicManager.loadEllipseScale(this);
				if (!sqlGraphicManager.load(this.CurrentGraphicExtent))
				{
					this.expand();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("LayerManager.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("LayerManager.message.error.load.exception.header", "Error, could not create plot from database."), i18n.getString("LayerManager.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
			finally
			{
				this.ignoreChangeEvent = false;
				//this.expand();
				this.draw();
			}
		}

		public virtual void featureZoom()
		{
			FeatureZoomDialog.showAndWait(this, this.layers);
		}

		public virtual void layerProperties()
		{
			LayerManagerDialog.showAndWait(this, this.layers);
		}

		public virtual void expand()
		{
			GraphicExtent maxGraphicExtent = new GraphicExtent();
			foreach (Layer layer in this.layers)
			{
				if (layer.Visible)
				{
					maxGraphicExtent.merge(layer.MaximumGraphicExtent);
				}
			}

			if (maxGraphicExtent.MinX == double.MaxValue && maxGraphicExtent.MinY == double.MaxValue && maxGraphicExtent.MaxX == double.Epsilon && maxGraphicExtent.MaxY == double.Epsilon)
			{
				//this.redraw(); 
			}
			else
			{
				if (maxGraphicExtent.MinX == maxGraphicExtent.MaxX)
				{
					maxGraphicExtent.MinX = maxGraphicExtent.MinX - 1;
					maxGraphicExtent.MaxX = maxGraphicExtent.MaxX + 1;
				}
				if (maxGraphicExtent.MinY == maxGraphicExtent.MaxY)
				{
					maxGraphicExtent.MinY = maxGraphicExtent.MinY - 1;
					maxGraphicExtent.MaxY = maxGraphicExtent.MaxY + 1;
				}
				this.currentGraphicExtent.set(maxGraphicExtent);
				//this.draw();
				this.zoomOut();
			}
		}

		public virtual void zoomIn()
		{
			double scale = this.currentGraphicExtent.Scale;
			this.currentGraphicExtent.Scale = scale / 1.15;
			this.draw();
		}

		public virtual void zoomOut()
		{
			double scale = this.currentGraphicExtent.Scale;
			this.currentGraphicExtent.Scale = scale * 1.15;
			this.draw();
		}

		public virtual void saveSnapshot()
		{
			double pixelScale = 1.5;
			double width = this.stackPane.getWidth();
			double height = this.stackPane.getHeight();

			if (height > 0 && width > 0)
			{
				ExtensionFilter[] extensionFilters = new ExtensionFilter[]
				{
					new ExtensionFilter(i18n.getString("LayerManager.extension.filled.png", "Portable Network Graphics (PNG)"), "*.png"),
					new ExtensionFilter(i18n.getString("LayerManager.extension.transparent.png", "Transparent Portable Network Graphics (PNG)"), "*.png")
				};
				File outputFile = DefaultFileChooser.showSaveDialog(JAG3D.Stage, i18n.getString("LayerManager.export.title", "Save current graphic"), null, extensionFilters);

				if (outputFile != null)
				{
					try
					{
						bool transparentImage = DefaultFileChooser.SelectedExtensionFilter == extensionFilters[1];
						SnapshotParameters snapshotParameters = new SnapshotParameters();
						snapshotParameters.setFill(transparentImage ? Color.TRANSPARENT : null);
						snapshotParameters.setTransform(Transform.scale(pixelScale, pixelScale));
						WritableImage writableImage = new WritableImage((int)Math.Round(pixelScale * width), (int)Math.Round(pixelScale * height));
						this.stackPane.snapshot(snapshotParameters, writableImage);

						BufferedImage bufferedImage = SwingFXUtils.fromFXImage(writableImage, null);
						ImageIO.write(bufferedImage, "PNG", outputFile);
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(i18n.getString("LayerManager.message.error.save.image.exception.title", "Unexpected Error"), i18n.getString("LayerManager.message.error.save.image.exception.header", "Error, could not save plot as image."), i18n.getString("LayerManager.message.error.save.image.exception.message", "An exception has occurred during image export."), e);
						});
					}
				}
			}
		}

		private ToggleButton createToggleButton(string iconName, string tooltip, ToolbarType type, ToolbarActionEventHandler toolbarActionEventHandler)
		{
			ToggleButton toggleButton = new ToggleButton();
			toggleButton.setTooltip(new Tooltip(tooltip));
			try
			{
				toggleButton.setGraphic(new ImageView(ImageUtils.getImage(iconName)));
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			toggleButton.setPadding(new Insets(0, 0, 0, 0));
			toggleButton.setUserData(type);
			toggleButton.setOnAction(toolbarActionEventHandler);
			return toggleButton;
		}

		private Button createButton(string iconName, string tooltip, ToolbarType type, ToolbarActionEventHandler toolbarActionEventHandler)
		{
			Button button = new Button();
			button.setTooltip(new Tooltip(tooltip));
			try
			{
				button.setGraphic(new ImageView(ImageUtils.getImage(iconName)));
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			button.setPadding(new Insets(0, 0, 0, 0));
			button.setUserData(type);
			button.setOnAction(toolbarActionEventHandler);
			return button;
		}
		private Spinner<double> createDoubleSpinner(double min, double max, double initValue, double amountToStepBy, string tooltip, int digits)
		{
			NumberFormat numberFormat = NumberFormat.getInstance(Locale.ENGLISH);
			numberFormat.setMaximumFractionDigits(digits);
			numberFormat.setMinimumFractionDigits(digits);
			numberFormat.setGroupingUsed(false);

			StringConverter<double> converter = new StringConverterAnonymousInnerClass(this, numberFormat);

			SpinnerValueFactory.DoubleSpinnerValueFactory doubleFactory = new SpinnerValueFactory.DoubleSpinnerValueFactory(min, max);
			Spinner<double> doubleSpinner = new Spinner<double>();
			doubleSpinner.setEditable(true);
			doubleSpinner.setValueFactory(doubleFactory);

			doubleFactory.setConverter(converter);
			doubleFactory.setAmountToStepBy(amountToStepBy);

			TextFormatter<double> formatter = new TextFormatter<double>(doubleFactory.getConverter(), doubleFactory.getValue());
			doubleSpinner.getEditor().setTextFormatter(formatter);
			doubleSpinner.getEditor().setAlignment(Pos.BOTTOM_RIGHT);
			doubleFactory.valueProperty().bindBidirectional(formatter.valueProperty());

			doubleSpinner.setPrefWidth(100);
			doubleSpinner.setMaxWidth(double.MaxValue);
			doubleSpinner.setTooltip(new Tooltip(tooltip));

			doubleFactory.setValue(initValue);

			doubleFactory.valueProperty().addListener(new ChangeListenerAnonymousInnerClass(this, doubleFactory));
			return doubleSpinner;
		}

		private class StringConverterAnonymousInnerClass : StringConverter<double>
		{
			private readonly LayerManager outerInstance;

			private NumberFormat numberFormat;

			public StringConverterAnonymousInnerClass(LayerManager outerInstance, NumberFormat numberFormat)
			{
				this.outerInstance = outerInstance;
				this.numberFormat = numberFormat;
			}

			public override double? fromString(string s)
			{
				if (string.ReferenceEquals(s, null) || s.Trim().Length == 0)
				{
					return null;
				}
				else
				{
					try
					{
						return numberFormat.parse(s).doubleValue();
					}
					catch (Exception nfe)
					{
						Console.WriteLine(nfe.ToString());
						Console.Write(nfe.StackTrace);
					}
				}
				return null;
			}

			public override string toString(double? d)
			{
				return d == null ? "" : numberFormat.format(d);
			}
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<double>
		{
			private readonly LayerManager outerInstance;

			private SpinnerValueFactory.DoubleSpinnerValueFactory doubleFactory;

			public ChangeListenerAnonymousInnerClass(LayerManager outerInstance, SpinnerValueFactory.DoubleSpinnerValueFactory doubleFactory)
			{
				this.outerInstance = outerInstance;
				this.doubleFactory = doubleFactory;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (newValue == null)
				{
					doubleFactory.setValue(oldValue);
				}
			}
		}

		private void save(double newScale)
		{
			SQLGraphicManager sqlGraphicManager = SQLManager.Instance.SQLGraphicManager;
			if (sqlGraphicManager == null || double.IsNaN(newScale) || double.IsInfinity(newScale))
			{
				return;
			}

			try
			{
				sqlGraphicManager.saveEllipseScale(newScale);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("LayerManager.message.error.save.scale.exception.title", "Unexpected SQL-Error"), i18n.getString("LayerManager.message.error.save.scale.exception.header", "Error, could not save scale parameter to database."), i18n.getString("LayerManager.message.error.save.scale.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private void save(GraphicExtent currentGraphicExtent)
		{
			SQLGraphicManager sqlGraphicManager = SQLManager.Instance.SQLGraphicManager;
			if (double.IsNaN(currentGraphicExtent.MinX) || double.IsInfinity(currentGraphicExtent.MinX) || double.IsNaN(currentGraphicExtent.MinY) || double.IsInfinity(currentGraphicExtent.MinY) || double.IsNaN(currentGraphicExtent.MaxX) || double.IsInfinity(currentGraphicExtent.MaxX) || double.IsNaN(currentGraphicExtent.MaxY) || double.IsInfinity(currentGraphicExtent.MaxY) || sqlGraphicManager == null)
			{
				return;
			}

			try
			{
				sqlGraphicManager.save(currentGraphicExtent);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("LayerManager.message.error.save.extent.exception.title", "Unexpected SQL-Error"), i18n.getString("LayerManager.message.error.save.extent.exception.header", "Error, could not save graphic extent to database."), i18n.getString("LayerManager.message.error.save.extent.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}
	}

}