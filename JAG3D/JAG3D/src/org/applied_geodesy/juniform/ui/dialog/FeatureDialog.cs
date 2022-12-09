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

namespace org.applied_geodesy.juniform.ui.dialog
{

	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using PrimitiveType = org.applied_geodesy.adjustment.geometry.PrimitiveType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using Restriction = org.applied_geodesy.adjustment.geometry.restriction.Restriction;
	using UITreeBuilder = org.applied_geodesy.juniform.ui.tree.UITreeBuilder;
	using LaTexLabel = org.applied_geodesy.ui.tex.LaTexLabel;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using VPos = javafx.geometry.VPos;
	using Node = javafx.scene.Node;
	using Button = javafx.scene.control.Button;
	using ButtonType = javafx.scene.control.ButtonType;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using ListCell = javafx.scene.control.ListCell;
	using ListView = javafx.scene.control.ListView;
	using TextField = javafx.scene.control.TextField;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using VBox = javafx.scene.layout.VBox;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class FeatureDialog
	{

		private class GeometricPrimitiveSelectionChangeListener : ChangeListener<GeometricPrimitive>
		{
			private readonly FeatureDialog outerInstance;

			public GeometricPrimitiveSelectionChangeListener(FeatureDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, GeometricPrimitive oldValue, GeometricPrimitive newValue) where T1 : org.applied_geodesy.adjustment.geometry.GeometricPrimitive
			{
				outerInstance.GeometricPrimitive = newValue;
			}
		}

		private class ManageGeometricPrimitiveEventHandler : EventHandler<ActionEvent>
		{
			private readonly FeatureDialog outerInstance;

			public ManageGeometricPrimitiveEventHandler(FeatureDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				if (@event.getSource() == outerInstance.addPrimitiveButton && outerInstance.feature != null)
				{
					Optional<GeometricPrimitive> optional = GeometricPrimitiveDialog.showAndWait(outerInstance.feature.FeatureType);

					if (optional.isPresent())
					{
						GeometricPrimitive result = optional.get();
						outerInstance.addGeometricPrimitive(result);
					}
				}
				else if (@event.getSource() == outerInstance.removePrimitiveButton)
				{
					outerInstance.removeGeometricPrimitive();
				}
			}
		}

		private static I18N i18N = I18N.Instance;
		private static FeatureDialog featureDialog = new FeatureDialog();
		private Dialog<Feature> dialog = null;
		private TextField nameTextField;
		private ListView<GeometricPrimitive> geometricPrimitiveList;
		private Label primitiveTypeLabel;
		private Button addPrimitiveButton, removePrimitiveButton;
		private LaTexLabel latexLabel;
		private Window window;
		private Feature feature;
		private GeometricPrimitive geometricPrimitive;

		private FeatureDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				featureDialog.window = value;
			}
		}

		public static Optional<Feature> showAndWait(Feature feature)
		{
			featureDialog.init();
			featureDialog.Feature = feature;
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				featureDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) featureDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return featureDialog.dialog.showAndWait();
		}

		private Feature Feature
		{
			set
			{
				this.feature = value;
    
				if (value != null)
				{
					this.geometricPrimitiveList.getSelectionModel().clearSelection();
					this.geometricPrimitiveList.setItems(this.feature.GeometricPrimitives);
					this.addPrimitiveButton.setDisable(this.feature.Immutable);
					this.geometricPrimitiveList.requestFocus();
					if (this.geometricPrimitiveList.getItems().size() > 0)
					{
						this.GeometricPrimitive = this.geometricPrimitiveList.getItems().get(0);
						this.geometricPrimitiveList.getSelectionModel().clearAndSelect(0);
					}
				}
			}
		}

		private GeometricPrimitive GeometricPrimitive
		{
			set
			{
				if (this.geometricPrimitive != null)
				{
					this.nameTextField.textProperty().unbindBidirectional(this.geometricPrimitive.nameProperty());
					this.primitiveTypeLabel.setText(null);
				}
    
				this.geometricPrimitive = value;
				if (this.geometricPrimitive == null)
				{
					this.latexLabel.Tex = "";
					this.nameTextField.setDisable(true);
					this.primitiveTypeLabel.setText(null);
    
					this.removePrimitiveButton.setDisable(true);
				}
				else
				{
					this.nameTextField.setDisable(false);
					this.removePrimitiveButton.setDisable(false);
    
					this.latexLabel.Tex = value.toLaTex();
    
					if (this.feature.Immutable)
					{
						this.removePrimitiveButton.setDisable(true);
						this.addPrimitiveButton.setDisable(true);
					}
					else
					{
						// check, if one of the unknown parameters is bonded to a restriction
						// if yes, disable remove button
						ISet<Restriction> equations = new HashSet<Restriction>(value.Restrictions);
						foreach (Restriction restriction in this.feature.Restrictions)
						{
							if (equations.Contains(restriction))
							{
								continue;
							}
							foreach (UnknownParameter unknownParameter in value.UnknownParameters)
							{
								if (restriction.contains(unknownParameter) || restriction.contains(value))
								{
									this.removePrimitiveButton.setDisable(true);
									break;
								}
							}
						}
    
						foreach (Restriction restriction in this.feature.PostProcessingCalculations)
						{
							if (equations.Contains(restriction))
							{
								continue;
							}
							foreach (UnknownParameter unknownParameter in value.UnknownParameters)
							{
								if (restriction.contains(unknownParameter) || restriction.contains(value))
								{
									this.removePrimitiveButton.setDisable(true);
									break;
								}
							}
						}
					}
					this.primitiveTypeLabel.setText(GeometricPrimitiveDialog.getPrimitiveTypeLabel(this.geometricPrimitive.PrimitiveType));
					this.nameTextField.textProperty().bindBidirectional(this.geometricPrimitive.nameProperty());
				}
			}
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<Feature>();
			this.dialog.setTitle(i18N.getString("FeatureDialog.title", "Feature"));
			this.dialog.setHeaderText(i18N.getString("FeatureDialog.header", "Feature properties"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, Feature>
		{
			private readonly FeatureDialog outerInstance;

			public CallbackAnonymousInnerClass(FeatureDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override Feature call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					return outerInstance.feature;
				}
				return null;
			}
		}

		private Node createPane()
		{
			GridPane gridPane = DialogUtil.createGridPane();

			Label nameLabel = new Label(i18N.getString("FeatureDialog.primitive.name.label", "Name:"));
			Label typeLabel = new Label(i18N.getString("FeatureDialog.primitive.type.label", "Type:"));
			Label equationLabel = new Label(i18N.getString("FeatureDialog.equation.label", "Equation:"));

			this.nameTextField = DialogUtil.createTextField(i18N.getString("FeatureDialog.primitive.name.tooltip", "Name of geometric primitive"), i18N.getString("FeatureDialog.primitive.name.prompt", "Geometric primitive name"));

			this.nameTextField.textProperty().addListener(new ChangeListenerAnonymousInnerClass(this));

			this.latexLabel = new LaTexLabel();
			this.latexLabel.setMinSize(225, Control.USE_PREF_SIZE);
			this.latexLabel.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE);

			this.primitiveTypeLabel = new Label();
			this.geometricPrimitiveList = DialogUtil.createGeometricPrimitiveListView(createGeometricPrimitiveCellFactory());
			this.geometricPrimitiveList.getSelectionModel().selectedItemProperty().addListener(new GeometricPrimitiveSelectionChangeListener(this));

			ManageGeometricPrimitiveEventHandler manageGeometricPrimitiveEventHandler = new ManageGeometricPrimitiveEventHandler(this);
			VBox buttonBox = new VBox(3);
			this.addPrimitiveButton = DialogUtil.createButton(i18N.getString("FeatureDialog.button.add.label", "Add"), i18N.getString("FeatureDialog.button.add.tooltip", "Add further geometric primitve"));

			this.removePrimitiveButton = DialogUtil.createButton(i18N.getString("FeatureDialog.button.remove.label", "Remove"), i18N.getString("FeatureDialog.button.remove.tooltip", "Remove selected geometric primitve"));

			this.addPrimitiveButton.setOnAction(manageGeometricPrimitiveEventHandler);
			this.removePrimitiveButton.setOnAction(manageGeometricPrimitiveEventHandler);
			this.addPrimitiveButton.setDisable(true);
			this.removePrimitiveButton.setDisable(true);

			VBox.setVgrow(this.addPrimitiveButton, Priority.ALWAYS);
			VBox.setVgrow(this.removePrimitiveButton, Priority.ALWAYS);
			buttonBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			buttonBox.getChildren().addAll(this.addPrimitiveButton, this.removePrimitiveButton);

			Region spacer = new Region();

			nameLabel.setLabelFor(this.nameTextField);
			typeLabel.setLabelFor(this.primitiveTypeLabel);
			equationLabel.setLabelFor(this.latexLabel);

			nameLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			equationLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			nameLabel.setMaxWidth(double.MaxValue);
			typeLabel.setMaxWidth(double.MaxValue);
			equationLabel.setMaxWidth(double.MaxValue);

			GridPane.setHgrow(nameLabel, Priority.NEVER);
			GridPane.setHgrow(typeLabel, Priority.NEVER);
			GridPane.setHgrow(buttonBox, Priority.NEVER);
			GridPane.setHgrow(equationLabel, Priority.NEVER);

			GridPane.setHgrow(this.nameTextField, Priority.ALWAYS);
			GridPane.setHgrow(this.primitiveTypeLabel, Priority.ALWAYS);
			GridPane.setHgrow(this.latexLabel, Priority.ALWAYS);
			GridPane.setHgrow(this.geometricPrimitiveList, Priority.NEVER);

			GridPane.setVgrow(this.geometricPrimitiveList, Priority.ALWAYS);
			GridPane.setVgrow(buttonBox, Priority.NEVER);

			GridPane.setHgrow(spacer, Priority.ALWAYS);
			GridPane.setVgrow(spacer, Priority.ALWAYS);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 5);
			Insets insetsRight = new Insets(5, 0, 5, 7);

			GridPane.setMargin(this.geometricPrimitiveList, new Insets(5,10,5,0));

			GridPane.setMargin(nameLabel, insetsLeft);
			GridPane.setMargin(typeLabel, insetsLeft);
			GridPane.setMargin(equationLabel, insetsLeft);
			GridPane.setMargin(buttonBox, insetsLeft);

			GridPane.setMargin(this.nameTextField, insetsRight);
			GridPane.setMargin(this.primitiveTypeLabel, insetsRight);
			GridPane.setMargin(this.latexLabel, insetsRight);

			GridPane.setValignment(equationLabel, VPos.TOP);

			int row = 0;

			gridPane.add(nameLabel, 1, row); // column, row, columnspan, rowspan
			gridPane.add(this.nameTextField, 2, row++);

			gridPane.add(typeLabel, 1, row);
			gridPane.add(this.primitiveTypeLabel, 2, row++);

			gridPane.add(equationLabel, 1, row);
			gridPane.add(this.latexLabel, 2, row++);

			gridPane.add(spacer, 1, row++, 2, 1);
			gridPane.add(buttonBox, 1, row++);

			gridPane.add(this.geometricPrimitiveList, 0, 0, 1, row);

			Platform.runLater(() =>
			{
			geometricPrimitiveList.requestFocus();
			if (geometricPrimitiveList.getItems().size() > 0)
			{
				geometricPrimitiveList.getSelectionModel().clearAndSelect(0);
				GeometricPrimitive = geometricPrimitiveList.getItems().get(0);
			}
			});

			return gridPane;
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<string>
		{
			private readonly FeatureDialog outerInstance;

			public ChangeListenerAnonymousInnerClass(FeatureDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, string oldValue, string newValue) where T1 : string
			{
				UITreeBuilder.Instance.Tree.refresh();
			}
		}

		internal static StringConverter<PrimitiveType> createPrimitiveTypeStringConverter()
		{
			return new StringConverterAnonymousInnerClass();
		}

		private class StringConverterAnonymousInnerClass : StringConverter<PrimitiveType>
		{

			public override string toString(PrimitiveType primitiveType)
			{
				return GeometricPrimitiveDialog.getPrimitiveTypeLabel(primitiveType);
			}

			public override PrimitiveType fromString(string @string)
			{
				return PrimitiveType.valueOf(@string);
			}
		}

		internal static Callback<ListView<GeometricPrimitive>, ListCell<GeometricPrimitive>> createGeometricPrimitiveCellFactory()
		{
			return new CallbackAnonymousInnerClass2();
		}

		private class CallbackAnonymousInnerClass2 : Callback<ListView<GeometricPrimitive>, ListCell<GeometricPrimitive>>
		{
			public override ListCell<GeometricPrimitive> call(ListView<GeometricPrimitive> listView)
			{
				return new ListCellAnonymousInnerClass(this);};
			}

		private class ListCellAnonymousInnerClass : ListCell<GeometricPrimitive>
		{
			private readonly CallbackAnonymousInnerClass2 outerInstance;

			public ListCellAnonymousInnerClass(CallbackAnonymousInnerClass2 outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			protected internal override void updateItem(GeometricPrimitive geometricPrimitive, bool empty)
			{
				base.updateItem(geometricPrimitive, empty);

				// https://stackoverflow.com/questions/28549107/how-to-bind-data-in-a-javafx-cell
				this.textProperty().unbind();

				if (empty || geometricPrimitive == null)
				{
					this.setText(null);
					this.setTooltip(null);
					this.setGraphic(null);
				}
				else
				{
					this.textProperty().bind(geometricPrimitive.nameProperty());
				}
			}
		}

		private void addGeometricPrimitive(GeometricPrimitive geometricPrimitive)
		{
			if (geometricPrimitive != null)
			{
				this.feature.GeometricPrimitives.add(geometricPrimitive);
			}
		}

		private void removeGeometricPrimitive()
		{
			if (this.feature != null && this.geometricPrimitive != null)
			{
				this.feature.GeometricPrimitives.remove(this.geometricPrimitive);
			}
		}

		}

	}