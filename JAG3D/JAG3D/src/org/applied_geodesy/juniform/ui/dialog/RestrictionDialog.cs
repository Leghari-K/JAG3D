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

	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using AverageRestriction = org.applied_geodesy.adjustment.geometry.restriction.AverageRestriction;
	using FeaturePointRestriction = org.applied_geodesy.adjustment.geometry.restriction.FeaturePointRestriction;
	using ProductSumRestriction = org.applied_geodesy.adjustment.geometry.restriction.ProductSumRestriction;
	using Restriction = org.applied_geodesy.adjustment.geometry.restriction.Restriction;
	using RestrictionType = org.applied_geodesy.adjustment.geometry.restriction.RestrictionType;
	using TrigonometricRestriction = org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction;
	using VectorAngleRestriction = org.applied_geodesy.adjustment.geometry.restriction.VectorAngleRestriction;
	using LaTexLabel = org.applied_geodesy.ui.tex.LaTexLabel;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;
	using UIPointTableBuilder = org.applied_geodesy.juniform.ui.table.UIPointTableBuilder;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ObservableList = javafx.collections.ObservableList;
	using FilteredList = javafx.collections.transformation.FilteredList;
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
	using SelectionMode = javafx.scene.control.SelectionMode;
	using TextField = javafx.scene.control.TextField;
	using GridPane = javafx.scene.layout.GridPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using VBox = javafx.scene.layout.VBox;
	using Color = javafx.scene.paint.Color;
	using Text = javafx.scene.text.Text;
	using TextAlignment = javafx.scene.text.TextAlignment;
	using TextFlow = javafx.scene.text.TextFlow;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;

	public class RestrictionDialog
	{
		private class PostProcessingPredicate : System.Predicate<UnknownParameter>
		{
			private readonly RestrictionDialog outerInstance;

			public PostProcessingPredicate(RestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override bool test(UnknownParameter unknownParameter)
			{
				return outerInstance.enablePostProcessing || unknownParameter.ProcessingType != ProcessingType.POSTPROCESSING;
			}
		}

		private class RestrictionEventHandler : EventHandler<ActionEvent>
		{
			private readonly RestrictionDialog outerInstance;

			public RestrictionEventHandler(RestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void handle(ActionEvent @event)
			{
				if (@event.getSource() == outerInstance.addRestrictionButton)
				{
					Optional<RestrictionType> optional = RestrictionTypeDialog.showAndWait();

					if (optional.isPresent())
					{
						RestrictionType restrictionType = optional.get();
						if (restrictionType == null)
						{
							return;
						}

						Restriction restriction = outerInstance.createRestriction(restrictionType);
						if (restriction == null)
						{
							return;
						}

						outerInstance.restrictionList.getItems().add(restriction);
						outerInstance.restrictionList.getSelectionModel().clearSelection();
						outerInstance.restrictionList.getSelectionModel().select(restriction);

						outerInstance.editRestriction(restriction);
						outerInstance.editRestrictionButton.setDisable(outerInstance.restrictionList.getItems().size() == 0);
						outerInstance.removeRestrictionButton.setDisable(outerInstance.restrictionList.getItems().size() == 0);
					}
				}
				else if (@event.getSource() == outerInstance.editRestrictionButton)
				{
					Restriction restriction = outerInstance.restrictionList.getSelectionModel().getSelectedItem();
					if (restriction != null)
					{
						outerInstance.editRestriction(restriction);
					}
				}
				else if (@event.getSource() == outerInstance.removeRestrictionButton)
				{
					Restriction restriction = outerInstance.restrictionList.getSelectionModel().getSelectedItem();
					if (restriction != null)
					{
						outerInstance.removeRestriction(restriction);
					}
				}
				else if (@event.getSource() == outerInstance.moveUpRestrictionButtom)
				{
					int index = outerInstance.restrictionList.getSelectionModel().getSelectedIndex();
					if (index > 0)
					{
						// Collections.swap(restrictionList.getItems(), index, index-1);
						// A unique list does not support Collection.swap()
						// thus, just a tmp deep copy is used of the selected element
						Restriction dummyElement = outerInstance.createRestriction(outerInstance.restrictionList.getSelectionModel().getSelectedItem().getRestrictionType());
						Restriction firstElement = outerInstance.restrictionList.getItems().set(index, dummyElement);
						Restriction secondElement = outerInstance.restrictionList.getItems().set(index - 1, firstElement);
						outerInstance.restrictionList.getItems().set(index, secondElement);
						outerInstance.restrictionList.getSelectionModel().clearAndSelect(index - 1);

					}
				}
				else if (@event.getSource() == outerInstance.moveDownRestrictionButtom)
				{
					int index = outerInstance.restrictionList.getSelectionModel().getSelectedIndex();
					if (index < outerInstance.restrictionList.getItems().size() - 1)
					{
						// Collections.swap(restrictionList.getItems(), index, index+1);
						// A unique list does not support Collection.swap()
						// thus, just a tmp deep copy is used of the selected element
						Restriction dummyElement = outerInstance.createRestriction(outerInstance.restrictionList.getSelectionModel().getSelectedItem().getRestrictionType());
						Restriction firstElement = outerInstance.restrictionList.getItems().set(index, dummyElement);
						Restriction secondElement = outerInstance.restrictionList.getItems().set(index + 1, firstElement);
						outerInstance.restrictionList.getItems().set(index, secondElement);
						outerInstance.restrictionList.getSelectionModel().clearAndSelect(index + 1);
					}
				}
			}
		}
		private I18N i18n = I18N.Instance;
		private static RestrictionDialog restrictionDialog = new RestrictionDialog();
		private Dialog<Void> dialog = null;
		private LaTexLabel latexLabel;
		private TextField descriptionTextField;
		private Label restrictionTypeLabel;
		private Label parameterOwner;
		private ListView<Restriction> restrictionList;
		private Button addRestrictionButton, editRestrictionButton, removeRestrictionButton, moveUpRestrictionButtom, moveDownRestrictionButtom;
		private Restriction restriction;
		private Window window;
		private Feature feature;
		private bool enablePostProcessing = false;
		private FilteredList<UnknownParameter> filteredUnknownParameters = null;
		private RestrictionDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				restrictionDialog.window = value;
			}
		}

		public static Optional<Void> showAndWait(Feature feature, bool enablePostProcessing)
		{
			restrictionDialog.init();
			restrictionDialog.setFeature(feature, enablePostProcessing);
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				restrictionDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) restrictionDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return restrictionDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<Void>();
			this.dialog.setTitle(i18n.getString("RestrictionDialog.title", "Restrictions"));
			this.dialog.setHeaderText(i18n.getString("RestrictionDialog.header", "Parameter restrictions"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, Void>
		{
			private readonly RestrictionDialog outerInstance;

			public CallbackAnonymousInnerClass(RestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override Void call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{

				}
				return null;
			}
		}

		private Restriction Restriction
		{
			set
			{
				// property un-binding
				if (this.restriction != null)
				{
					this.descriptionTextField.textProperty().unbindBidirectional(this.restriction.descriptionProperty());
				}
    
				this.restriction = value;
    
				if (value == null)
				{
					this.descriptionTextField.setDisable(true);
					this.editRestrictionButton.setDisable(true);
					this.removeRestrictionButton.setDisable(true);
    
					this.latexLabel.Tex = "";
					this.restrictionTypeLabel.setText("");
					this.parameterOwner.setText("");
    
					this.moveUpRestrictionButtom.setDisable(true);
					this.moveDownRestrictionButtom.setDisable(true);
				}
				else
				{
					bool indispensable = this.restriction.Indispensable;
    
					this.descriptionTextField.setDisable(false);
					this.editRestrictionButton.setDisable(false);
					this.removeRestrictionButton.setDisable(indispensable);
    
					this.moveUpRestrictionButtom.setDisable(indispensable || this.restrictionList.getItems().isEmpty() || !this.restrictionList.getItems().isEmpty() && this.restrictionList.getSelectionModel().getSelectedIndex() == 0);
					this.moveDownRestrictionButtom.setDisable(indispensable || this.restrictionList.getItems().isEmpty() || !this.restrictionList.getItems().isEmpty() && this.restrictionList.getSelectionModel().getSelectedIndex() == this.restrictionList.getItems().size() - 1);
    
					this.latexLabel.Tex = value.toLaTex();
    
					this.restrictionTypeLabel.setText(RestrictionTypeDialog.getRestrictionTypeLabel(this.restriction.RestrictionType));
    
					// property binding
					this.descriptionTextField.textProperty().bindBidirectional(this.restriction.descriptionProperty());
    
					bool foundOwner = false;
					foreach (GeometricPrimitive geometry in this.feature.GeometricPrimitives)
					{
						if (geometry.Restrictions.Contains(value))
						{
							this.parameterOwner.setText(geometry.Name);
							foundOwner = true;
							break;
						}
					}
					if (!foundOwner)
					{
						this.parameterOwner.setText(i18n.getString("RestrictionDialog.restriction.owner.default", "Auxially restriction"));
					}
    
				}
			}
		}

		private Node createPane()
		{
			GridPane gridPane = DialogUtil.createGridPane();

			Label ownerLabel = new Label(i18n.getString("RestrictionDialog.restriction.owner.label", "Owner:"));
			Label equationLabel = new Label(i18n.getString("RestrictionDialog.equation.label", "Equation:"));
			Label restrictionTypeLabel = new Label(i18n.getString("RestrictionDialog.restriction.type.label", "Restriction type:"));
			Label descriptionLabel = new Label(i18n.getString("RestrictionDialog.description.label", "Description:"));

			this.restrictionTypeLabel = new Label("");

			this.restrictionList = this.createRestrictionListView();

			this.descriptionTextField = DialogUtil.createTextField(i18n.getString("RestrictionDialog.description.tooltip", "Description of parameter restriction"), i18n.getString("RestrictionDialog.description.prompt", "Restriction description"));

			this.parameterOwner = new Label();
			this.parameterOwner.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.parameterOwner.setMaxWidth(double.MaxValue);

			this.latexLabel = new LaTexLabel();
			this.latexLabel.setMinSize(225, Control.USE_PREF_SIZE);
			this.latexLabel.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE);

			RestrictionEventHandler restrictionEventHandler = new RestrictionEventHandler(this);
			VBox buttonBox = new VBox(3);
			this.addRestrictionButton = DialogUtil.createButton(i18n.getString("RestrictionDialog.restriction.add.label", "Add"), i18n.getString("RestrictionDialog.restriction.add.tooltip", "Add new parameter restriction"));

			this.editRestrictionButton = DialogUtil.createButton(i18n.getString("RestrictionDialog.restriction.edit.label", "Edit"), i18n.getString("RestrictionDialog.restriction.edit.tooltip", "Edit selected restriction"));

			this.removeRestrictionButton = DialogUtil.createButton(i18n.getString("RestrictionDialog.restriction.remove.label", "Remove"), i18n.getString("RestrictionDialog.restriction.remove.tooltip", "Remove selected restriction"));

			TextFlow noticeFlowPane = new TextFlow();
			noticeFlowPane.setPadding(new Insets(10, 5, 10, 5));
			noticeFlowPane.setTextAlignment(TextAlignment.JUSTIFY);
			noticeFlowPane.setLineSpacing(2.0);
			Text noticeLabel = new Text(i18n.getString("RestrictionDialog.warning.initial_guess.label", "Please note:"));
			noticeLabel.setFill(Color.DARKRED);
			Text noticeMessage = new Text(i18n.getString("RestrictionDialog.warning.initial_guess.message", "For some options the automatic estimation of an initial guess must be disabled, and appropriate approximations have to be specified manually."));
			noticeMessage.setFill(Color.BLACK);
			noticeFlowPane.getChildren().addAll(noticeLabel, new Text(" "), noticeMessage);

			this.addRestrictionButton.setOnAction(restrictionEventHandler);
			this.editRestrictionButton.setOnAction(restrictionEventHandler);
			this.removeRestrictionButton.setOnAction(restrictionEventHandler);

			VBox.setVgrow(this.addRestrictionButton, Priority.ALWAYS);
			VBox.setVgrow(this.editRestrictionButton, Priority.ALWAYS);
			VBox.setVgrow(this.removeRestrictionButton, Priority.ALWAYS);
			buttonBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			buttonBox.getChildren().addAll(this.addRestrictionButton, this.editRestrictionButton, this.removeRestrictionButton);

			HBox orderButtonBox = new HBox(3);
			this.moveUpRestrictionButtom = DialogUtil.createButton(i18n.getString("RestrictionDialog.restriction.up.label", "\u25B2"), i18n.getString("RestrictionDialog.restriction.up.tooltip", "Move up (change order)"));
			this.moveDownRestrictionButtom = DialogUtil.createButton(i18n.getString("RestrictionDialog.restriction.down.label", "\u25BC"), i18n.getString("RestrictionDialog.restriction.down.tooltip", "Move down (change order)"));

			this.moveUpRestrictionButtom.setOnAction(restrictionEventHandler);
			this.moveDownRestrictionButtom.setOnAction(restrictionEventHandler);

			HBox.setHgrow(this.moveUpRestrictionButtom, Priority.NEVER);
			HBox.setHgrow(this.moveDownRestrictionButtom, Priority.NEVER);
			orderButtonBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			orderButtonBox.getChildren().addAll(this.moveUpRestrictionButtom, this.moveDownRestrictionButtom);

			ownerLabel.setLabelFor(this.parameterOwner);
			equationLabel.setLabelFor(this.latexLabel);
			restrictionTypeLabel.setLabelFor(this.restrictionTypeLabel);
			descriptionLabel.setLabelFor(this.descriptionTextField);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 5);
			Insets insetsRight = new Insets(5, 0, 5, 7);

			GridPane.setMargin(this.restrictionList, new Insets(5,10,5,0));
			GridPane.setMargin(orderButtonBox, new Insets(5,10,5,0));

			GridPane.setMargin(restrictionTypeLabel, insetsLeft);
			GridPane.setMargin(descriptionLabel, insetsLeft);
			GridPane.setMargin(equationLabel, insetsLeft);
			GridPane.setMargin(buttonBox, insetsLeft);
			GridPane.setMargin(ownerLabel, insetsLeft);

			GridPane.setMargin(this.restrictionTypeLabel, insetsRight);
			GridPane.setMargin(this.descriptionTextField, insetsRight);
			GridPane.setMargin(this.latexLabel, insetsRight);
			GridPane.setMargin(this.parameterOwner, insetsRight);

			this.restrictionTypeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			restrictionTypeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			descriptionLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			equationLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			ownerLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			noticeFlowPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			noticeFlowPane.setPrefSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			this.restrictionTypeLabel.setMaxWidth(double.MaxValue);
			restrictionTypeLabel.setMaxWidth(double.MaxValue);
			descriptionLabel.setMaxWidth(double.MaxValue);
			equationLabel.setMaxWidth(double.MaxValue);
			ownerLabel.setMaxWidth(double.MaxValue);

			Region spacer = new Region();

			GridPane.setHgrow(equationLabel, Priority.NEVER);
			GridPane.setHgrow(restrictionTypeLabel, Priority.NEVER);
			GridPane.setHgrow(descriptionLabel, Priority.NEVER);
			GridPane.setHgrow(ownerLabel, Priority.NEVER);

			GridPane.setHgrow(this.latexLabel, Priority.ALWAYS);
			GridPane.setHgrow(this.restrictionTypeLabel, Priority.ALWAYS);
			GridPane.setHgrow(this.parameterOwner, Priority.ALWAYS);
			GridPane.setHgrow(this.restrictionList, Priority.SOMETIMES);
			GridPane.setHgrow(buttonBox, Priority.NEVER);
			GridPane.setHgrow(orderButtonBox, Priority.NEVER);
			GridPane.setHgrow(noticeFlowPane, Priority.ALWAYS);

			GridPane.setVgrow(this.restrictionList, Priority.ALWAYS);
			GridPane.setVgrow(this.latexLabel, Priority.NEVER);
			GridPane.setVgrow(this.restrictionTypeLabel, Priority.NEVER);
			GridPane.setVgrow(buttonBox, Priority.NEVER);
			GridPane.setVgrow(orderButtonBox, Priority.NEVER);
			GridPane.setHgrow(noticeFlowPane, Priority.NEVER);

			GridPane.setHgrow(spacer, Priority.ALWAYS);
			GridPane.setVgrow(spacer, Priority.ALWAYS);

			GridPane.setValignment(equationLabel, VPos.TOP);

			int row = 0;

			gridPane.add(ownerLabel, 1, row); // column, row, columnspan, rowspan
			gridPane.add(this.parameterOwner, 2, row++);

			gridPane.add(equationLabel, 1, row);
			gridPane.add(this.latexLabel, 2, row++);

			gridPane.add(restrictionTypeLabel, 1, row);
			gridPane.add(this.restrictionTypeLabel, 2, row++);

			gridPane.add(descriptionLabel, 1, row);
			gridPane.add(this.descriptionTextField, 2, row++);

			gridPane.add(noticeFlowPane, 1, row++, 2, 1);

			gridPane.add(spacer, 1, row++, 2, 1); // column, row, columnspan, rowspan
			gridPane.add(buttonBox, 1, row++);

			gridPane.add(this.restrictionList, 0, 0, 1, row++); // column, row, columnspan, rowspan
			gridPane.add(orderButtonBox, 0, row);

			Platform.runLater(() =>
			{
			restrictionList.requestFocus();
			if (restrictionList.getItems().size() > 0)
			{
				restrictionList.getSelectionModel().clearAndSelect(0);
				Restriction = restrictionList.getItems().get(0);
			}
			});
			return gridPane;
		}

		private ListView<Restriction> createRestrictionListView()
		{
			ListView<Restriction> list = new ListView<Restriction>();
			list.getSelectionModel().selectedItemProperty().addListener(new ChangeListenerAnonymousInnerClass(this));

			list.setCellFactory(new CallbackAnonymousInnerClass2(this));
			list.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			list.setMaxSize(double.MaxValue, double.MaxValue);

			list.getSelectionModel().setSelectionMode(SelectionMode.SINGLE);

			ListView<string> placeholderList = new ListView<string>();
			placeholderList.getItems().add("");
			placeholderList.setDisable(true);
			list.setPlaceholder(placeholderList);

			return list;
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<Restriction>
		{
			private readonly RestrictionDialog outerInstance;

			public ChangeListenerAnonymousInnerClass(RestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void changed<T1>(ObservableValue<T1> observable, Restriction oldValue, Restriction newValue) where T1 : org.applied_geodesy.adjustment.geometry.restriction.Restriction
			{
				outerInstance.Restriction = newValue;
			}
		}

		private class CallbackAnonymousInnerClass2 : Callback<ListView<Restriction>, ListCell<Restriction>>
		{
			private readonly RestrictionDialog outerInstance;

			public CallbackAnonymousInnerClass2(RestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override ListCell<Restriction> call(ListView<Restriction> listView)
			{
				return new ListCellAnonymousInnerClass(this);};
			}

		private class ListCellAnonymousInnerClass : ListCell<Restriction>
		{
			private readonly CallbackAnonymousInnerClass2 outerInstance;

			public ListCellAnonymousInnerClass(CallbackAnonymousInnerClass2 outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			protected internal override void updateItem(Restriction restriction, bool empty)
			{
				base.updateItem(restriction, empty);
				if (empty || restriction == null)
				{
					this.setText(null);
				}
				else
				{
					this.setText(RestrictionTypeDialog.getRestrictionTypeLabel(restriction.RestrictionType));
				}
			}
		}

		private void editRestriction(Restriction restriction)
		{
			RestrictionType restrictionType = restriction.RestrictionType;

			switch (restrictionType)
			{
			case RestrictionType.AVERAGE:
				AverageRestrictionDialog.showAndWait(this.filteredUnknownParameters, (AverageRestriction)restriction);
				break;

			case RestrictionType.PRODUCT_SUM:
				ProductSumRestrictionDialog.showAndWait(this.filteredUnknownParameters, (ProductSumRestriction)restriction);
				break;

			case RestrictionType.FEATURE_POINT:
				FeaturePointRestrictionDialog.showAndWait(this.feature.GeometricPrimitives, UIPointTableBuilder.Instance.Table.getItems(), (FeaturePointRestriction)restriction);
				break;

			case RestrictionType.VECTOR_ANGLE:
				VectorAngleRestrictionDialog.showAndWait(this.filteredUnknownParameters, (VectorAngleRestriction)restriction);
				break;

			case RestrictionType.TRIGONOMERTIC_FUNCTION:
				TrigonometricRestrictionDialog.showAndWait(this.filteredUnknownParameters, (TrigonometricRestriction)restriction);
				break;
			}
		}

		private void removeRestriction(Restriction restriction)
		{
			if (restriction.Indispensable)
			{
				return;
			}

			this.restrictionList.getItems().remove(restriction);
			this.removeRestrictionButton.setDisable(this.restrictionList.getItems().size() == 0);
		}

		private Restriction createRestriction(RestrictionType restrictionType)
		{
			switch (restrictionType)
			{
			case RestrictionType.AVERAGE:
				return new AverageRestriction();

			case RestrictionType.PRODUCT_SUM:
				return new ProductSumRestriction();

			case RestrictionType.FEATURE_POINT:
				return new FeaturePointRestriction();

			case RestrictionType.VECTOR_ANGLE:
				return new VectorAngleRestriction();

			case RestrictionType.TRIGONOMERTIC_FUNCTION:
				return new TrigonometricRestriction();
			}
			return null;
		}

		private ObservableList<UnknownParameter> UnknownParameters
		{
			set
			{
				if (value != null)
				{
					this.filteredUnknownParameters = new FilteredList<UnknownParameter>(value, new PostProcessingPredicate());
				}
			}
		}

		private void setFeature(Feature feature, bool enablePostProcessing)
		{
			this.enablePostProcessing = enablePostProcessing;
			this.feature = feature;

			if (this.feature != null)
			{
				if (enablePostProcessing)
				{
					this.UnknownParameters = feature.UnknownParameters;
					this.Equations = feature.PostProcessingCalculations;
				}
				else
				{
					this.UnknownParameters = feature.UnknownParameters;
					this.Equations = feature.Restrictions;
				}
			}
		}

		private ObservableList<Restriction> Equations
		{
			set
			{
				if (value != null)
				{
					this.editRestrictionButton.setDisable(value.size() == 0);
					this.removeRestrictionButton.setDisable(value.size() == 0);
					this.restrictionList.setItems(value);
					this.restrictionList.requestFocus();
					if (this.restrictionList.getItems().size() > 0)
					{
						this.Restriction = this.restrictionList.getItems().get(0);
						this.restrictionList.getSelectionModel().clearAndSelect(0);
					}
				}
			}
		}
		}

	}