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

namespace org.applied_geodesy.jag3d.ui.graphic.layer.dialog
{

	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using ArrowLayer = org.applied_geodesy.jag3d.ui.graphic.layer.ArrowLayer;
	using org.applied_geodesy.jag3d.ui.graphic.layer;
	using Layer = org.applied_geodesy.jag3d.ui.graphic.layer.Layer;
	using LayerManager = org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager;
	using LayerType = org.applied_geodesy.jag3d.ui.graphic.layer.LayerType;
	using LegendLayer = org.applied_geodesy.jag3d.ui.graphic.layer.LegendLayer;
	using ObservationLayer = org.applied_geodesy.jag3d.ui.graphic.layer.ObservationLayer;
	using PointLayer = org.applied_geodesy.jag3d.ui.graphic.layer.PointLayer;
	using SQLGraphicManager = org.applied_geodesy.jag3d.ui.graphic.sql.SQLGraphicManager;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ObservableList = javafx.collections.ObservableList;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using Button = javafx.scene.control.Button;
	using ButtonType = javafx.scene.control.ButtonType;
	using Dialog = javafx.scene.control.Dialog;
	using DialogEvent = javafx.scene.control.DialogEvent;
	using ListCell = javafx.scene.control.ListCell;
	using ListView = javafx.scene.control.ListView;
	using ScrollPane = javafx.scene.control.ScrollPane;
	using SelectionMode = javafx.scene.control.SelectionMode;
	using ToolBar = javafx.scene.control.ToolBar;
	using Tooltip = javafx.scene.control.Tooltip;
	using BorderPane = javafx.scene.layout.BorderPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;

	public class LayerManagerDialog
	{

		private class LayerIndexChangeListener : ChangeListener<Number>
		{
			private readonly LayerManagerDialog outerInstance;

			internal ListView<Layer> listView;
			internal LayerIndexChangeListener(LayerManagerDialog outerInstance, ListView<Layer> listView)
			{
				this.outerInstance = outerInstance;
				this.listView = listView;
			}
			public override void changed<T1>(ObservableValue<T1> observable, Number oldValue, Number newValue) where T1 : Number
			{
				if (newValue == null || newValue.intValue() < 0)
				{
					outerInstance.upButton.setDisable(true);
					outerInstance.downButton.setDisable(true);
				}
				else
				{
					outerInstance.upButton.setDisable(newValue.intValue() == 0);
					outerInstance.downButton.setDisable(newValue.intValue() == this.listView.getItems().size() - 1);

					Layer selectedLayer = this.listView.getItems().get(newValue.intValue());
					if (selectedLayer != null && selectedLayer.LayerType != null && outerInstance.layerManager != null)
					{
						LayerType type = selectedLayer.LayerType;

						Node propertiesNode = null;
						switch (type.innerEnumValue)
						{
						case LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
						case LayerType.InnerEnum.DATUM_POINT_APRIORI:
						case LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
						case LayerType.InnerEnum.NEW_POINT_APRIORI:
						case LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
						case LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
						case LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
						case LayerType.InnerEnum.STOCHASTIC_POINT_APRIORI:
							propertiesNode = UIPointLayerPropertyBuilder.getLayerPropertyPane(outerInstance.layerManager, (PointLayer)selectedLayer);
							break;

						case LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
						case LayerType.InnerEnum.OBSERVATION_APRIORI:
							propertiesNode = UIObservationLayerPropertyBuilder.getLayerPropertyPane(outerInstance.layerManager, (ObservationLayer)selectedLayer);
							break;

						case LayerType.InnerEnum.ABSOLUTE_CONFIDENCE:
						case LayerType.InnerEnum.RELATIVE_CONFIDENCE:
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: propertiesNode = UIConfidenceLayerPropertyBuilder.getLayerPropertyPane(layerManager, (org.applied_geodesy.jag3d.ui.graphic.layer.ConfidenceLayer<?>)selectedLayer);
							propertiesNode = UIConfidenceLayerPropertyBuilder.getLayerPropertyPane(outerInstance.layerManager, (ConfidenceLayer<object>)selectedLayer);
							break;

						case LayerType.InnerEnum.POINT_SHIFT_HORIZONTAL:
						case LayerType.InnerEnum.POINT_SHIFT_VERTICAL:
						case LayerType.InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL:
						case LayerType.InnerEnum.PRINCIPAL_COMPONENT_VERTICAL:
						case LayerType.InnerEnum.POINT_RESIDUAL_HORIZONTAL:
						case LayerType.InnerEnum.POINT_RESIDUAL_VERTICAL:
							propertiesNode = UIArrowLayerPropertyBuilder.getLayerPropertyPane(outerInstance.layerManager, (ArrowLayer)selectedLayer);
							break;

						case LayerType.InnerEnum.LEGEND:
							propertiesNode = UILegendLayerPropertyBuilder.getLayerPropertyPane(outerInstance.layerManager, (LegendLayer)selectedLayer);
							break;
						}

						if (propertiesNode != null)
						{
							ScrollPane scroller = new ScrollPane(propertiesNode);
							scroller.setPadding(new Insets(10, 10, 10, 10)); // oben, links, unten, rechts
							scroller.setFitToHeight(true);
							scroller.setFitToWidth(true);
							outerInstance.layerPropertyBorderPane.setCenter(scroller);
						}
					}
				}
			}
		}

		private class UpDownEventHandler : EventHandler<ActionEvent>
		{
			private readonly LayerManagerDialog outerInstance;

			public UpDownEventHandler(LayerManagerDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				Layer selectedLayer = outerInstance.layerListView.getSelectionModel().getSelectedItem();
				bool moveUp = @event.getSource() == outerInstance.upButton;
				int selectedIndex = -1;
				if (selectedLayer != null && (selectedIndex = outerInstance.layerListView.getItems().IndexOf(selectedLayer)) >= 0)
				{
					Collections.swap(outerInstance.layerListView.getItems(), selectedIndex + (moveUp ? -1 : 1), selectedIndex);
					IList<Layer> reorderedList = new List<Layer>(outerInstance.layerListView.getItems());
					reorderedList.Reverse();
					outerInstance.layerManager.reorderLayer(reorderedList);

					//layerListView.getSelectionModel().clearSelection();
					outerInstance.layerListView.getSelectionModel().select(selectedLayer);

					outerInstance.layerManager.draw();
				}
			}
		}

		private I18N i18n = I18N.Instance;
		private static LayerManagerDialog layerManagerDialog = new LayerManagerDialog();
		private Dialog<Void> dialog = null;
		private static Window window;
		private LayerManager layerManager;
		private BorderPane layerPropertyBorderPane = new BorderPane();
		private Button upButton, downButton;
		private ListView<Layer> layerListView;
		private ObservableList<Layer> layers;
		private LayerManagerDialog()
		{
		}


		public static Window Owner
		{
			set
			{
				window = value;
			}
		}

		public static Optional<Void> showAndWait(LayerManager layerManager, ObservableList<Layer> layers)
		{
			layerManagerDialog.layerManager = layerManager;
			layerManagerDialog.layers = layers;
			layerManagerDialog.init();
			layerManagerDialog.load();
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				layerManagerDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) layerManagerDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return layerManagerDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<Void>();

			this.dialog.setTitle(i18n.getString("LayerManagerDialog.title", "Layer manager"));
			this.dialog.setHeaderText(i18n.getString("LayerManagerDialog.header", "Layer properties"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.CLOSE);
			this.dialog.initModality(Modality.APPLICATION_MODAL);

			this.dialog.initOwner(window);

			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);

			this.dialog.setOnCloseRequest(new EventHandlerAnonymousInnerClass(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<DialogEvent>
		{
			private readonly LayerManagerDialog outerInstance;

			public EventHandlerAnonymousInnerClass(LayerManagerDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(DialogEvent @event)
			{
				outerInstance.save();
				outerInstance.layerListView.getSelectionModel().select(0);
			}
		}

		private Node createPane()
		{
			this.layerPropertyBorderPane.setTop(this.createLayerOrderToolBar());

			BorderPane rootNode = new BorderPane();
			this.layerListView = this.createLayerListPane();
			rootNode.setLeft(this.layerListView);
			rootNode.setCenter(this.layerPropertyBorderPane);
			return rootNode;
		}

		private ToolBar createLayerOrderToolBar()
		{
			UpDownEventHandler upDownEventHandler = new UpDownEventHandler(this);
			this.upButton = new Button(i18n.getString("LayerManagerDialog.up.label", "\u25B2"));
			this.upButton.setTooltip(new Tooltip(i18n.getString("LayerManagerDialog.up.tooltip", "Move selected layer up")));
			this.upButton.setOnAction(upDownEventHandler);
			this.downButton = new Button(i18n.getString("LayerManagerDialog.down.label", "\u25BC"));
			this.downButton.setTooltip(new Tooltip(i18n.getString("LayerManagerDialog.down.tooltip", "Move selected layer down")));
			this.downButton.setOnAction(upDownEventHandler);

			ToolBar toolBar = new ToolBar();
			HBox spacer = new HBox();
			HBox.setHgrow(spacer, Priority.ALWAYS);
			toolBar.getItems().addAll(spacer, this.upButton, this.downButton);
			return toolBar;
		}

		private ListView<Layer> createLayerListPane()
		{
			ListView<Layer> listView = new ListView<Layer>();

			int length = this.layers.size();
			for (int i = length - 1; i >= 0; i--)
			{
				listView.getItems().add(this.layers.get(i));
			}

			listView.setCellFactory(new CallbackAnonymousInnerClass(this));
			listView.getSelectionModel().setSelectionMode(SelectionMode.SINGLE);
			listView.getSelectionModel().selectedIndexProperty().addListener(new LayerIndexChangeListener(this, listView));
			listView.getSelectionModel().select(0);

			return listView;
		}

		private class CallbackAnonymousInnerClass : Callback<ListView<Layer>, ListCell<Layer>>
		{
			private readonly LayerManagerDialog outerInstance;

			public CallbackAnonymousInnerClass(LayerManagerDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override ListCell<Layer> call(ListView<Layer> list)
			{
				return new LayerListCell();
			}
		}

		private void load()
		{
			SQLGraphicManager sqlGraphicManager = SQLManager.Instance.SQLGraphicManager;
			if (sqlGraphicManager == null)
			{
				return;
			}

			try
			{
				sqlGraphicManager.initLayer(this.layerManager);

				ListView<Layer> listView = new ListView<Layer>();
				int length = this.layers.size();
				Layer firstLayerNotLegend = null;
				for (int i = length - 1; i >= 0; i--)
				{
					Layer layer = this.layers.get(i);
					listView.getItems().add(layer);
					if (firstLayerNotLegend == null && layer.LayerType != LayerType.LEGEND)
					{
						firstLayerNotLegend = layer;
					}
				}
				this.layerListView.getSelectionModel().clearSelection();
				this.layerListView.getItems().setAll(listView.getItems());
				if (firstLayerNotLegend != null)
				{
					this.layerListView.getSelectionModel().select(firstLayerNotLegend);
				}
				else
				{
					this.layerListView.getSelectionModel().select(0);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("LayerManagerDialog.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("LayerManagerDialog.message.error.load.exception.header", "Error, could not load layer properties from database."), i18n.getString("LayerManagerDialog.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private void save()
		{
			SQLGraphicManager sqlGraphicManager = SQLManager.Instance.SQLGraphicManager;
			if (sqlGraphicManager == null)
			{
				return;
			}

			try
			{
				int order = 0;
				foreach (Layer layer in this.layers)
				{
					LayerType layerType = layer.LayerType;

					switch (layerType.innerEnumValue)
					{
					case LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
					case LayerType.InnerEnum.DATUM_POINT_APRIORI:
					case LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
					case LayerType.InnerEnum.NEW_POINT_APRIORI:
					case LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
					case LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
					case LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
					case LayerType.InnerEnum.STOCHASTIC_POINT_APRIORI:
						sqlGraphicManager.save((PointLayer)layer, order);
						break;

					case LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
					case LayerType.InnerEnum.OBSERVATION_APRIORI:
						sqlGraphicManager.save((ObservationLayer)layer, order);
						break;

					case LayerType.InnerEnum.RELATIVE_CONFIDENCE:
					case LayerType.InnerEnum.ABSOLUTE_CONFIDENCE:
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: sqlGraphicManager.save((org.applied_geodesy.jag3d.ui.graphic.layer.ConfidenceLayer<?>)layer, order);
						sqlGraphicManager.save((ConfidenceLayer<object>)layer, order);
						break;

					case LayerType.InnerEnum.POINT_SHIFT_HORIZONTAL:
					case LayerType.InnerEnum.POINT_SHIFT_VERTICAL:
					case LayerType.InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL:
					case LayerType.InnerEnum.PRINCIPAL_COMPONENT_VERTICAL:
					case LayerType.InnerEnum.POINT_RESIDUAL_HORIZONTAL:
					case LayerType.InnerEnum.POINT_RESIDUAL_VERTICAL:
						sqlGraphicManager.save((ArrowLayer)layer, order);
						break;

					case LayerType.InnerEnum.LEGEND:
						sqlGraphicManager.save((LegendLayer)layer, order);
						break;
					}
					order++;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("LayerManagerDialog.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("LayerManagerDialog.message.error.save.exception.header", "Error, could not save layer properties to database."), i18n.getString("LayerManagerDialog.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}
	}
}