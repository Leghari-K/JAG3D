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

	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using FeaturePointRestriction = org.applied_geodesy.adjustment.geometry.restriction.FeaturePointRestriction;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;
	using UIPointSelectionPaneBuilder = org.applied_geodesy.juniform.ui.propertiespane.UIPointSelectionPaneBuilder;
	using LaTexLabel = org.applied_geodesy.ui.tex.LaTexLabel;

	using Platform = javafx.application.Platform;
	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using TextField = javafx.scene.control.TextField;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;

	public class FeaturePointRestrictionDialog
	{

		private static I18N i18N = I18N.Instance;
		private static FeaturePointRestrictionDialog restrictionTypeDialog = new FeaturePointRestrictionDialog();
		private Dialog<FeaturePointRestriction> dialog = null;
		private LaTexLabel latexLabel;
		private ComboBox<GeometricPrimitive> geometricPrimitiveComboBox;
		private ComboBox<FeaturePoint> featurePointComboBox;
		private TextField descriptionTextField;
		private Window window;
		private FeaturePointRestriction restriction;


		private FeaturePointRestrictionDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				restrictionTypeDialog.window = value;
			}
		}

		public static Optional<FeaturePointRestriction> showAndWait(ObservableList<GeometricPrimitive> geometricPrimitives, ObservableList<FeaturePoint> featurePoints, FeaturePointRestriction featurePointRestriction)
		{
			restrictionTypeDialog.init();
			restrictionTypeDialog.GeometricPrimitives = geometricPrimitives;
			restrictionTypeDialog.FeaturePoints = featurePoints;
			restrictionTypeDialog.Restriction = featurePointRestriction;
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				restrictionTypeDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) restrictionTypeDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return restrictionTypeDialog.dialog.showAndWait();
		}

		private FeaturePointRestriction Restriction
		{
			set
			{
				if (this.restriction != null)
				{
					this.geometricPrimitiveComboBox.valueProperty().unbindBidirectional(this.restriction.geometricPrimitiveProperty());
					this.featurePointComboBox.valueProperty().unbindBidirectional(this.restriction.featurePointProperty());
					this.descriptionTextField.textProperty().unbindBidirectional(this.restriction.descriptionProperty());
				}
    
				this.restriction = value;
    
				this.geometricPrimitiveComboBox.valueProperty().bindBidirectional(this.restriction.geometricPrimitiveProperty());
				this.featurePointComboBox.valueProperty().bindBidirectional(this.restriction.featurePointProperty());
				this.descriptionTextField.textProperty().bindBidirectional(this.restriction.descriptionProperty());
				this.latexLabel.Tex = this.restriction.toLaTex();
    
				this.geometricPrimitiveComboBox.setDisable(value.Indispensable);
				this.featurePointComboBox.setDisable(value.Indispensable);
    
				if (!value.Indispensable)
				{
					if (value.GeometricPrimitive != null)
					{
						this.geometricPrimitiveComboBox.getSelectionModel().select(value.GeometricPrimitive);
					}
					else
					{
						this.geometricPrimitiveComboBox.getSelectionModel().clearAndSelect(0);
					}
    
					if (value.FeaturePoint != null)
					{
						this.featurePointComboBox.getSelectionModel().select(value.FeaturePoint);
					}
					else
					{
						this.featurePointComboBox.getSelectionModel().clearAndSelect(0);
					}
				}
			}
		}

		private ObservableList<FeaturePoint> FeaturePoints
		{
			set
			{
				ObservableList<FeaturePoint> featurePointList = FXCollections.observableArrayList(value);
				this.featurePointComboBox.setItems(featurePointList);
			}
		}

		private ObservableList<GeometricPrimitive> GeometricPrimitives
		{
			set
			{
				ObservableList<GeometricPrimitive> geometricPrimitiveList = FXCollections.observableArrayList(value);
				this.geometricPrimitiveComboBox.setItems(geometricPrimitiveList);
			}
		}


		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<FeaturePointRestriction>();
			this.dialog.setTitle(i18N.getString("FeaturePointRestrictionDialog.title", "Feature point restriction"));
			this.dialog.setHeaderText(i18N.getString("FeaturePointRestrictionDialog.header", "Feature point restriction"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass3(this));
		}

		private class CallbackAnonymousInnerClass3 : Callback<ButtonType, FeaturePointRestriction>
		{
			private readonly FeaturePointRestrictionDialog outerInstance;

			public CallbackAnonymousInnerClass3(FeaturePointRestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override FeaturePointRestriction call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					return outerInstance.restriction;
				}
				return null;
			}
		}

		private Node createPane()
		{
			GridPane gridPane = DialogUtil.createGridPane();

			Label equationLabel = new Label(i18N.getString("FeaturePointRestrictionDialog.equation.label", "Equation:"));
			Label descriptionLabel = new Label(i18N.getString("FeaturePointRestrictionDialog.description.label", "Description:"));
			Label geometryLabel = new Label(i18N.getString("FeaturePointRestrictionDialog.primitive.label", "Geometric primitive:"));
			Label pointLabel = new Label(i18N.getString("FeaturePointRestrictionDialog.featurepoint.label", "Feature point:"));

			equationLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			descriptionLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			geometryLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			pointLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			this.descriptionTextField = DialogUtil.createTextField(i18N.getString("FeaturePointRestrictionDialog.description.tooltip", "Description of parameter restriction"), i18N.getString("FeaturePointRestrictionDialog.description.prompt", "Restriction description"));

			this.geometricPrimitiveComboBox = DialogUtil.createGeometricPrimitiveComboBox(FeatureDialog.createGeometricPrimitiveCellFactory(), i18N.getString("FeaturePointRestrictionDialog.primitive.tooltip", "Select geometric primitive"));

			this.featurePointComboBox = DialogUtil.createFeaturePointComboBox(UIPointSelectionPaneBuilder.createFeaturePointCellFactory(), i18N.getString("FeaturePointRestrictionDialog.featurepoint.tooltip", "Select feature point"));

			this.latexLabel = new LaTexLabel();
			this.latexLabel.setPrefSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.latexLabel.setMaxSize(double.MaxValue, double.MaxValue);

			equationLabel.setLabelFor(this.latexLabel);
			descriptionLabel.setLabelFor(this.descriptionTextField);
			geometryLabel.setLabelFor(this.geometricPrimitiveComboBox);
			pointLabel.setLabelFor(this.featurePointComboBox);


			GridPane.setHgrow(equationLabel, Priority.NEVER);
			GridPane.setHgrow(descriptionLabel, Priority.NEVER);
			GridPane.setHgrow(geometryLabel, Priority.NEVER);
			GridPane.setHgrow(pointLabel, Priority.NEVER);
			GridPane.setHgrow(this.latexLabel, Priority.ALWAYS);

			GridPane.setHgrow(this.descriptionTextField, Priority.ALWAYS);
			GridPane.setHgrow(this.geometricPrimitiveComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.featurePointComboBox, Priority.ALWAYS);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 5);
			Insets insetsRight = new Insets(5, 0, 5, 7);

			GridPane.setMargin(equationLabel, insetsLeft);
			GridPane.setMargin(descriptionLabel, insetsLeft);
			GridPane.setMargin(geometryLabel, insetsLeft);
			GridPane.setMargin(pointLabel, insetsLeft);

			GridPane.setMargin(this.latexLabel, insetsRight);
			GridPane.setMargin(this.geometricPrimitiveComboBox, insetsRight);
			GridPane.setMargin(this.featurePointComboBox, insetsRight);
			GridPane.setMargin(this.descriptionTextField, insetsRight);

			int row = 0;

			gridPane.add(equationLabel, 0, row); // column, row, columnspan, rowspan,
			gridPane.add(this.latexLabel, 1, row++);

			gridPane.add(descriptionLabel, 0, row);
			gridPane.add(this.descriptionTextField, 1, row++);

			gridPane.add(geometryLabel, 0, row);
			gridPane.add(this.geometricPrimitiveComboBox, 1, row++);

			gridPane.add(pointLabel, 0, row);
			gridPane.add(this.featurePointComboBox, 1, row++);

			return gridPane;
		}
	}

}