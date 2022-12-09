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

	using Layer = org.applied_geodesy.jag3d.ui.graphic.layer.Layer;
	using LayerManager = org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager;
	using PointLayer = org.applied_geodesy.jag3d.ui.graphic.layer.PointLayer;
	using GraphicPoint = org.applied_geodesy.jag3d.ui.graphic.sql.GraphicPoint;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using org.applied_geodesy.ui.table;

	using Platform = javafx.application.Platform;
	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using FilteredList = javafx.collections.transformation.FilteredList;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using ComboBox = javafx.scene.control.ComboBox;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using Slider = javafx.scene.control.Slider;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;

	public class FeatureZoomDialog
	{

		private I18N i18n = I18N.Instance;
		private static FeatureZoomDialog featureZoomDialog = new FeatureZoomDialog();
		private Dialog<GraphicPoint> dialog = null;
		private static Window window;
		private LayerManager layerManager;
		private ObservableList<Layer> layers;
		private ComboBox<GraphicPoint> pointsComboBox;
		private FeatureZoomDialog()
		{
		}
		private Slider scaleSlider;

		public static Window Owner
		{
			set
			{
				window = value;
			}
		}

		public static Optional<GraphicPoint> showAndWait(LayerManager layerManager, ObservableList<Layer> layers)
		{
			featureZoomDialog.layerManager = layerManager;
			featureZoomDialog.layers = layers;
			featureZoomDialog.init();
			featureZoomDialog.load();
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				featureZoomDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) featureZoomDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return featureZoomDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<GraphicPoint>();
			this.dialog.setTitle(i18n.getString("FeatureZoomDialog.title", "Feature zoom"));
			this.dialog.setHeaderText(i18n.getString("FeatureZoomDialog.header", "Zoom and center to point"));

			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			//		this.dialog.initStyle(StageStyle.UTILITY);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));

			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, GraphicPoint>
		{
			private readonly FeatureZoomDialog outerInstance;

			public CallbackAnonymousInnerClass(FeatureZoomDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override GraphicPoint call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					GraphicPoint selectedPoint = outerInstance.pointsComboBox.getValue();
					if (selectedPoint != null)
					{
						//List<GraphicPoint> selectedPoints = List.of(selectedPoint);
						outerInstance.zoomToFeature(selectedPoint);
					}

					return selectedPoint;
				}
				return null;
			}
		}

		private Node createPane()
		{
			Label pointsLabel = new Label(i18n.getString("FeatureZoomDialog.feature.points.label", "Adjusted points:"));
			Label scaleLabel = new Label(i18n.getString("FeatureZoomDialog.scale.label", "Scaling factor:"));

			this.pointsComboBox = this.createPointComboBox(i18n.getString("FeatureZoomDialog.feature.points.tooltip", "Select point to zoom"));
			this.scaleSlider = this.createScalingSlider(0.25, 1.75, 1.0, i18n.getString("FeatureZoomDialog.scale.tooltip", "Set scaling factor to zoom in or to zoom out"));

			pointsLabel.setLabelFor(this.pointsComboBox);
			scaleLabel.setLabelFor(this.scaleSlider);

			GridPane.setHgrow(pointsLabel, Priority.NEVER);
			GridPane.setHgrow(scaleLabel, Priority.NEVER);

			GridPane.setHgrow(this.pointsComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.scaleSlider, Priority.ALWAYS);

			GridPane gridPane = new GridPane();
			gridPane.setMaxWidth(double.MaxValue);
			gridPane.setHgap(10);
			gridPane.setVgap(10);
			gridPane.setAlignment(Pos.CENTER);
			gridPane.setPadding(new Insets(5,15,5,15));

			int row = 1;
			gridPane.add(pointsLabel, 0, row);
			gridPane.add(this.pointsComboBox, 1, row++);
			gridPane.add(scaleLabel, 0, row);
			gridPane.add(this.scaleSlider, 1, row++);

			return gridPane;
		}

		private ComboBox<GraphicPoint> createPointComboBox(string tooltip)
		{
			ComboBox<GraphicPoint> typeComboBox = new ComboBox<GraphicPoint>();

			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMinWidth(150);
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		private Slider createScalingSlider(double min, double max, double value, string tooltip)
		{
			Slider slider = new Slider(min, max, value);

			slider.setShowTickLabels(true);
			slider.setShowTickMarks(true);
			slider.setMajorTickUnit(0.5);
			slider.setMinorTickCount(1);
			slider.setBlockIncrement(0.25);
			slider.setMinWidth(150);
			slider.setMaxWidth(double.MaxValue);
			slider.setTooltip(new Tooltip(tooltip));
			return slider;
		}

		private void load()
		{
			ObservableList<GraphicPoint> graphicPoints = FXCollections.observableArrayList();
			foreach (Layer layer in this.layers)
			{
				if (!layer.Visible)
				{
					continue;
				}
				switch (layer.LayerType.innerEnumValue)
				{
	//			case REFERENCE_POINT_APRIORI:
	//			case STOCHASTIC_POINT_APRIORI:
	//			case DATUM_POINT_APRIORI:
	//			case NEW_POINT_APRIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
					PointLayer pointLayer = (PointLayer)layer;
					graphicPoints.addAll(pointLayer.Points);
					break;
				default:
					break;

				}
			}

			if (!graphicPoints.isEmpty())
			{
				Collections.sort(graphicPoints, new NaturalOrderComparator<GraphicPoint>());
				FilteredList<GraphicPoint> filteredPoints = new FilteredList<GraphicPoint>(graphicPoints, (GraphicPoint point) =>
				{
				return point.isVisible();
				});

				int selectedIndex = this.pointsComboBox.getSelectionModel().getSelectedIndex();
				this.pointsComboBox.getItems().setAll(filteredPoints);
				if (filteredPoints.size() > selectedIndex && selectedIndex >= 0)
				{
					this.pointsComboBox.getSelectionModel().clearAndSelect(selectedIndex);
				}
				else if (!filteredPoints.isEmpty())
				{
					this.pointsComboBox.getSelectionModel().clearAndSelect(0);
				}
			}
		}

		private void zoomToFeature(GraphicPoint selectedPoint)
		{
			GraphicExtent graphicExtent = this.layerManager.CurrentGraphicExtent;

			double pointX = selectedPoint.Coordinate.X;
			double pointY = selectedPoint.Coordinate.Y;

			double extentWidth = graphicExtent.ExtentWidth;
			double extentHeight = graphicExtent.ExtentHeight;
			double scale = graphicExtent.Scale;

			double newMinX = pointX - 0.5 * extentWidth;
			double newMaxX = pointX + 0.5 * extentWidth;

			double newMinY = pointY + 0.5 * extentHeight;
			double newMaxY = pointY - 0.5 * extentHeight;

			graphicExtent.set(newMinX, newMinY, newMaxX, newMaxY);
			graphicExtent.Scale = scale * this.scaleSlider.getValue();

			this.layerManager.draw();
		}
	}

}