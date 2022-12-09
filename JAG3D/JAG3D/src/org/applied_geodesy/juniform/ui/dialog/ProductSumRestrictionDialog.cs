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

	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using ProductSumRestriction = org.applied_geodesy.adjustment.geometry.restriction.ProductSumRestriction;
	using LaTexLabel = org.applied_geodesy.ui.tex.LaTexLabel;
	using DoubleTextField = org.applied_geodesy.ui.textfield.DoubleTextField;
	using ValueSupport = org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using Button = javafx.scene.control.Button;
	using ButtonType = javafx.scene.control.ButtonType;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using ListView = javafx.scene.control.ListView;
	using TextField = javafx.scene.control.TextField;
	using GridPane = javafx.scene.layout.GridPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using VBox = javafx.scene.layout.VBox;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class ProductSumRestrictionDialog
	{
		private class RegressorEventHandler : EventHandler<ActionEvent>
		{
			private readonly ProductSumRestrictionDialog outerInstance;

			public RegressorEventHandler(ProductSumRestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void handle(ActionEvent @event)
			{
				if (@event.getSource() == outerInstance.addParameterAButtom)
				{
					UnknownParameter parameter = outerInstance.regressorAComboBox.getValue();
					if (parameter != null)
					{
						outerInstance.regressorAListView.getItems().add(parameter);
					}
				}
				else if (@event.getSource() == outerInstance.addParameterBButtom)
				{
					UnknownParameter parameter = outerInstance.regressorBComboBox.getValue();
					if (parameter != null)
					{
						outerInstance.regressorBListView.getItems().add(parameter);
					}
				}

				else if (@event.getSource() == outerInstance.removeParameterAButtom)
				{
					UnknownParameter parameter = outerInstance.regressorAListView.getSelectionModel().getSelectedItem();
					if (parameter != null)
					{
						outerInstance.regressorAListView.getItems().remove(parameter);
					}
				}
				else if (@event.getSource() == outerInstance.removeParameterBButtom)
				{
					UnknownParameter parameter = outerInstance.regressorBListView.getSelectionModel().getSelectedItem();
					if (parameter != null)
					{
						outerInstance.regressorBListView.getItems().remove(parameter);
					}
				}

				else if (@event.getSource() == outerInstance.moveUpParameterAButtom)
				{
					int index = outerInstance.regressorAListView.getSelectionModel().getSelectedIndex();
					if (index > 0)
					{
						Collections.swap(outerInstance.regressorAListView.getItems(), index, index - 1);
					}
				}

				else if (@event.getSource() == outerInstance.moveDownParameterAButtom)
				{
					int index = outerInstance.regressorAListView.getSelectionModel().getSelectedIndex();
					if (index < outerInstance.regressorAListView.getItems().size() - 1)
					{
						Collections.swap(outerInstance.regressorAListView.getItems(), index, index + 1);
					}
				}

				else if (@event.getSource() == outerInstance.moveUpParameterBButtom)
				{
					int index = outerInstance.regressorBListView.getSelectionModel().getSelectedIndex();
					if (index > 0)
					{
						Collections.swap(outerInstance.regressorBListView.getItems(), index, index - 1);
					}
				}

				else if (@event.getSource() == outerInstance.moveDownParameterBButtom)
				{
					int index = outerInstance.regressorBListView.getSelectionModel().getSelectedIndex();
					if (index < outerInstance.regressorBListView.getItems().size() - 1)
					{
						Collections.swap(outerInstance.regressorBListView.getItems(), index, index + 1);
					}
				}
			}
		}

		private static I18N i18N = I18N.Instance;
		private static ProductSumRestrictionDialog restrictionTypeDialog = new ProductSumRestrictionDialog();
		private Dialog<ProductSumRestriction> dialog = null;
		private LaTexLabel latexLabel;
		private ComboBox<UnknownParameter> regressorAComboBox;
		private ComboBox<UnknownParameter> regressorBComboBox;
		private ComboBox<bool> arithmeticComboBox;
		private ListView<UnknownParameter> regressorAListView;
		private ListView<UnknownParameter> regressorBListView;
		private Button addParameterAButtom, removeParameterAButtom, moveUpParameterAButtom, moveDownParameterAButtom;
		private Button addParameterBButtom, removeParameterBButtom, moveUpParameterBButtom, moveDownParameterBButtom;
		private DoubleTextField exponentTextField;
		private ComboBox<UnknownParameter> regressandComboBox;
		private TextField descriptionTextField;
		private Window window;
		private ProductSumRestriction restriction;

		private ProductSumRestrictionDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				restrictionTypeDialog.window = value;
			}
		}

		public static Optional<ProductSumRestriction> showAndWait(ObservableList<UnknownParameter> unknownParameters, ProductSumRestriction productSumRestriction)
		{
			restrictionTypeDialog.init();
			restrictionTypeDialog.UnknownParameters = unknownParameters;
			restrictionTypeDialog.Restriction = productSumRestriction;
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

		private ObservableList<UnknownParameter> UnknownParameters
		{
			set
			{
				// a copy is needed because the argument list "unknownParameters" is a reference and becomes obsolete if the feature is changed 
				ObservableList<UnknownParameter> unknownParameterList = FXCollections.observableArrayList(value);
				this.regressorAComboBox.setItems(unknownParameterList);
				this.regressorBComboBox.setItems(unknownParameterList);
				this.regressandComboBox.setItems(unknownParameterList);
			}
		}

		private ProductSumRestriction Restriction
		{
			set
			{
				this.moveUpParameterAButtom.setDisable(true);
				this.moveDownParameterAButtom.setDisable(true);
    
				this.moveUpParameterBButtom.setDisable(true);
				this.moveDownParameterBButtom.setDisable(true);
    
				if (this.restriction != null)
				{
					this.arithmeticComboBox.valueProperty().unbindBidirectional(this.restriction.sumArithmeticProperty());
					this.exponentTextField.numberProperty().unbindBidirectional(this.restriction.exponentProperty());
					this.descriptionTextField.textProperty().unbindBidirectional(this.restriction.descriptionProperty());
					this.regressandComboBox.valueProperty().unbindBidirectional(this.restriction.regressandProperty());
					this.regressorAListView.setItems(FXCollections.emptyObservableList());
					this.regressorBListView.setItems(FXCollections.emptyObservableList());
				}
    
				this.restriction = value;
				this.arithmeticComboBox.valueProperty().bindBidirectional(this.restriction.sumArithmeticProperty());
				this.descriptionTextField.textProperty().bindBidirectional(this.restriction.descriptionProperty());
				this.exponentTextField.numberProperty().bindBidirectional(this.restriction.exponentProperty());
				this.regressandComboBox.valueProperty().bindBidirectional(this.restriction.regressandProperty());
				this.regressorAListView.setItems(this.restriction.RegressorsA);
				this.regressorBListView.setItems(this.restriction.RegressorsB);
				this.latexLabel.Tex = this.restriction.toLaTex();
				this.exponentTextField.Value = this.restriction.Exponent;
    
				this.exponentTextField.setDisable(value.Indispensable);
				this.arithmeticComboBox.setDisable(value.Indispensable);
    
				this.regressorAListView.setDisable(value.Indispensable);
				this.regressorBListView.setDisable(value.Indispensable);
    
				this.regressorAComboBox.setDisable(value.Indispensable);
				this.regressorBComboBox.setDisable(value.Indispensable);
    
				this.regressandComboBox.setDisable(value.Indispensable);
				this.addParameterAButtom.setDisable(value.Indispensable);
				this.removeParameterAButtom.setDisable(value.Indispensable);
				this.moveUpParameterAButtom.setDisable(value.Indispensable);
				this.moveDownParameterAButtom.setDisable(value.Indispensable);
				this.addParameterBButtom.setDisable(value.Indispensable);
				this.removeParameterBButtom.setDisable(value.Indispensable);
				this.moveUpParameterBButtom.setDisable(value.Indispensable);
				this.moveDownParameterBButtom.setDisable(value.Indispensable);
    
				if (!value.Indispensable)
				{
					UnknownParameter regressand = this.restriction.Regressand;
					if (regressand == null)
					{
						this.regressandComboBox.getSelectionModel().clearAndSelect(0);
					}
					this.regressorAComboBox.getSelectionModel().clearAndSelect(0);
					this.regressorBComboBox.getSelectionModel().clearAndSelect(0);
				}
				else
				{
					this.regressorAComboBox.getSelectionModel().clearSelection();
					this.regressorBComboBox.getSelectionModel().clearSelection();
				}
			}
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<ProductSumRestriction>();
			this.dialog.setTitle(i18N.getString("ProductSumRestrictionDialog.title", "Product sum restriction"));
			this.dialog.setHeaderText(i18N.getString("ProductSumRestrictionDialog.header", "k-th Power of product sum"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, ProductSumRestriction>
		{
			private readonly ProductSumRestrictionDialog outerInstance;

			public CallbackAnonymousInnerClass(ProductSumRestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override ProductSumRestriction call(ButtonType buttonType)
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

			Label equationLabel = new Label(i18N.getString("ProductSumRestrictionDialog.equation.label", "Equation:"));
			Label exponentLabel = new Label(i18N.getString("ProductSumRestrictionDialog.exponent.label", "Exponent k:"));
			Label descriptionLabel = new Label(i18N.getString("ProductSumRestrictionDialog.description.label", "Description:"));
			Label regressandLabel = new Label(i18N.getString("ProductSumRestrictionDialog.regressand.label", "Regressand c:"));
			Label regressorALabel = new Label(i18N.getString("ProductSumRestrictionDialog.regressor.a.label", "Regressor a:"));
			Label regressorBLabel = new Label(i18N.getString("ProductSumRestrictionDialog.regressor.b.label", "Regressor b:"));
			Label arithmeticLabel = new Label(i18N.getString("ProductSumRestrictionDialog.arithmetic.label", "Arithmetic operation:"));

			regressorALabel.setPrefSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			regressorALabel.setMaxWidth(double.MaxValue);

			regressorBLabel.setPrefSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			regressorBLabel.setMaxWidth(double.MaxValue);

			this.arithmeticComboBox = DialogUtil.createBooleanComboBox(createArithmeticOperationTypeStringConverter(), i18N.getString("ProductSumRestrictionDialog.arithmetic.tooltip", "Select arithmetic operation"));

			this.exponentTextField = DialogUtil.createDoubleTextField(CellValueType.DOUBLE, 1.0, false, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT, double.NegativeInfinity, double.PositiveInfinity, i18N.getString("ProductSumRestrictionDialog.exponent.tooltip", "Select exponent k"));

			this.descriptionTextField = DialogUtil.createTextField(i18N.getString("ProductSumRestrictionDialog.description.tooltip", "Description of parameter restriction"), i18N.getString("ProductSumRestrictionDialog.description.prompt", "Restriction description"));

			this.regressorAComboBox = DialogUtil.createUnknownParameterComboBox(UnknownParameterDialog.createUnknownParameterCellFactory(), i18N.getString("ProductSumRestrictionDialog.regressor.a.tooltip", "Select regressor a"));

			this.regressorBComboBox = DialogUtil.createUnknownParameterComboBox(UnknownParameterDialog.createUnknownParameterCellFactory(), i18N.getString("ProductSumRestrictionDialog.regressor.b.tooltip", "Select regressor b"));

			this.regressandComboBox = DialogUtil.createUnknownParameterComboBox(UnknownParameterDialog.createUnknownParameterCellFactory(), i18N.getString("ProductSumRestrictionDialog.regressand.tooltip", "Select regressand c"));

			this.latexLabel = new LaTexLabel();
			this.latexLabel.setPrefSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.latexLabel.setMaxSize(double.MaxValue, double.MaxValue);

			this.regressorAListView = DialogUtil.createParameterListView(UnknownParameterDialog.createUnknownParameterCellFactory());
			this.regressorBListView = DialogUtil.createParameterListView(UnknownParameterDialog.createUnknownParameterCellFactory());

			RegressorEventHandler regressorEventHandler = new RegressorEventHandler(this);

			this.addParameterAButtom = DialogUtil.createButton(i18N.getString("ProductSumRestrictionDialog.regressor.a.add.label", "+"), i18N.getString("ProductSumRestrictionDialog.regressor.a.add.tooltip", "Add parameter to regressor list"));

			this.removeParameterAButtom = DialogUtil.createButton(i18N.getString("ProductSumRestrictionDialog.regressor.a.remove.label", "-"), i18N.getString("ProductSumRestrictionDialog.regressor.a.remove.tooltip", "Remove regressor"));

			this.moveUpParameterAButtom = DialogUtil.createButton(i18N.getString("ProductSumRestrictionDialog.regressor.a.up.label", "\u25B2"), i18N.getString("ProductSumRestrictionDialog.regressor.a.up.tooltip", "Move up (change order)"));

			this.moveDownParameterAButtom = DialogUtil.createButton(i18N.getString("ProductSumRestrictionDialog.regressor.a.down.label", "\u25BC"), i18N.getString("ProductSumRestrictionDialog.regressor.a.down.tooltip", "Move down (change order)"));

			this.addParameterAButtom.setOnAction(regressorEventHandler);
			this.removeParameterAButtom.setOnAction(regressorEventHandler);
			this.moveUpParameterAButtom.setOnAction(regressorEventHandler);
			this.moveDownParameterAButtom.setOnAction(regressorEventHandler);

			HBox buttonBoxA = new HBox(5);
			HBox.setHgrow(this.addParameterAButtom, Priority.NEVER);
			HBox.setHgrow(this.removeParameterAButtom, Priority.NEVER);
			HBox.setHgrow(this.moveUpParameterAButtom, Priority.NEVER);
			HBox.setHgrow(this.moveDownParameterAButtom, Priority.NEVER);

			buttonBoxA.getChildren().addAll(this.addParameterAButtom, this.removeParameterAButtom, this.moveUpParameterAButtom, this.moveDownParameterAButtom);

			this.addParameterBButtom = DialogUtil.createButton(i18N.getString("ProductSumRestrictionDialog.regressor.b.add.label", "+"), i18N.getString("ProductSumRestrictionDialog.regressor.b.add.tooltip", "Add parameter to regressor list"));

			this.removeParameterBButtom = DialogUtil.createButton(i18N.getString("ProductSumRestrictionDialog.regressor.b.remove.label", "-"), i18N.getString("ProductSumRestrictionDialog.regressor.b.remove.tooltip", "Remove regressor"));

			this.moveUpParameterBButtom = DialogUtil.createButton(i18N.getString("ProductSumRestrictionDialog.regressor.b.up.label", "\u25B2"), i18N.getString("ProductSumRestrictionDialog.regressor.b.up.tooltip", "Move up (change order)"));

			this.moveDownParameterBButtom = DialogUtil.createButton(i18N.getString("ProductSumRestrictionDialog.regressor.b.down.label", "\u25BC"), i18N.getString("ProductSumRestrictionDialog.regressor.b.down.tooltip", "Move down (change order)"));

			this.addParameterBButtom.setOnAction(regressorEventHandler);
			this.removeParameterBButtom.setOnAction(regressorEventHandler);
			this.moveUpParameterBButtom.setOnAction(regressorEventHandler);
			this.moveDownParameterBButtom.setOnAction(regressorEventHandler);

			HBox buttonBoxB = new HBox(5);
			HBox.setHgrow(this.addParameterBButtom, Priority.NEVER);
			HBox.setHgrow(this.removeParameterBButtom, Priority.NEVER);
			HBox.setHgrow(this.moveUpParameterBButtom, Priority.NEVER);
			HBox.setHgrow(this.moveDownParameterBButtom, Priority.NEVER);

			buttonBoxB.getChildren().addAll(this.addParameterBButtom, this.removeParameterBButtom, this.moveUpParameterBButtom, this.moveDownParameterBButtom);

			this.regressorAListView.getSelectionModel().selectedIndexProperty().addListener(new ChangeListenerAnonymousInnerClass(this));

			this.regressorBListView.getSelectionModel().selectedIndexProperty().addListener(new ChangeListenerAnonymousInnerClass2(this));

			arithmeticLabel.setLabelFor(this.arithmeticComboBox);
			exponentLabel.setLabelFor(this.exponentTextField);
			equationLabel.setLabelFor(this.latexLabel);
			descriptionLabel.setLabelFor(this.descriptionTextField);
			regressandLabel.setLabelFor(this.regressandComboBox);
			regressorALabel.setLabelFor(this.regressorAComboBox);
			regressorBLabel.setLabelFor(this.regressorBComboBox);

			regressorALabel.setAlignment(Pos.TOP_CENTER);
			regressorBLabel.setAlignment(Pos.TOP_CENTER);

			HBox regressorsBox = new HBox(10);
			VBox regressorABox = new VBox(5);
			VBox regressorBBox = new VBox(5);
			regressorsBox.setPrefHeight(200);

			HBox.setHgrow(regressorABox, Priority.ALWAYS);
			HBox.setHgrow(regressorBBox, Priority.ALWAYS);

			VBox.setVgrow(regressorALabel, Priority.NEVER);
			VBox.setVgrow(regressorBLabel, Priority.NEVER);
			VBox.setVgrow(this.regressorAComboBox, Priority.NEVER);
			VBox.setVgrow(this.regressorBComboBox, Priority.NEVER);
			VBox.setVgrow(this.regressorAListView, Priority.ALWAYS);
			VBox.setVgrow(this.regressorBListView, Priority.ALWAYS);

			regressorABox.getChildren().addAll(regressorALabel, this.regressorAComboBox, this.regressorAListView, buttonBoxA);
			regressorBBox.getChildren().addAll(regressorBLabel, this.regressorBComboBox, this.regressorBListView, buttonBoxB);
			regressorsBox.getChildren().addAll(regressorABox, regressorBBox);

			GridPane.setHgrow(arithmeticLabel, Priority.NEVER);
			GridPane.setHgrow(exponentLabel, Priority.NEVER);
			GridPane.setHgrow(equationLabel, Priority.NEVER);
			GridPane.setHgrow(descriptionLabel, Priority.NEVER);
			GridPane.setHgrow(regressandLabel, Priority.NEVER);
			GridPane.setHgrow(this.latexLabel, Priority.ALWAYS);

			GridPane.setHgrow(this.arithmeticComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.exponentTextField, Priority.ALWAYS);
			GridPane.setHgrow(this.descriptionTextField, Priority.ALWAYS);
			GridPane.setHgrow(this.regressandComboBox, Priority.ALWAYS);
			GridPane.setHgrow(regressorsBox, Priority.ALWAYS);
			GridPane.setVgrow(regressorsBox, Priority.ALWAYS);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 5);
			Insets insetsRight = new Insets(5, 0, 5, 7);

			GridPane.setMargin(arithmeticLabel, insetsLeft);
			GridPane.setMargin(exponentLabel, insetsLeft);
			GridPane.setMargin(equationLabel, insetsLeft);
			GridPane.setMargin(regressandLabel, insetsLeft);
			GridPane.setMargin(descriptionLabel, insetsLeft);

			GridPane.setMargin(this.arithmeticComboBox, insetsRight);
			GridPane.setMargin(this.exponentTextField, insetsRight);
			GridPane.setMargin(this.latexLabel, insetsRight);
			GridPane.setMargin(this.regressandComboBox, insetsRight);
			GridPane.setMargin(this.descriptionTextField, insetsRight);

			GridPane.setMargin(regressorsBox, new Insets(10, 5, 5, 5));

			int row = 0;

			gridPane.add(equationLabel, 0, row); // column, row, columnspan, rowspan,
			gridPane.add(this.latexLabel, 1, row++);

			gridPane.add(descriptionLabel, 0, row);
			gridPane.add(this.descriptionTextField, 1, row++);

			gridPane.add(regressandLabel, 0, row);
			gridPane.add(this.regressandComboBox, 1, row++);

			gridPane.add(exponentLabel, 0, row);
			gridPane.add(this.exponentTextField, 1, row++);

			gridPane.add(arithmeticLabel, 0, row);
			gridPane.add(this.arithmeticComboBox, 1, row++);

			gridPane.add(regressorsBox, 0, row++, 2, 1);

			return gridPane;
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<Number>
		{
			private readonly ProductSumRestrictionDialog outerInstance;

			public ChangeListenerAnonymousInnerClass(ProductSumRestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Number oldValue, Number newValue) where T1 : Number
			{
				outerInstance.moveUpParameterAButtom.setDisable(newValue == null || newValue.intValue() == 0);
				outerInstance.moveDownParameterAButtom.setDisable(newValue == null || newValue.intValue() == outerInstance.regressorAListView.getItems().size() - 1);
			}
		}

		private class ChangeListenerAnonymousInnerClass2 : ChangeListener<Number>
		{
			private readonly ProductSumRestrictionDialog outerInstance;

			public ChangeListenerAnonymousInnerClass2(ProductSumRestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Number oldValue, Number newValue) where T1 : Number
			{
				outerInstance.moveUpParameterBButtom.setDisable(newValue == null || newValue.intValue() == 0);
				outerInstance.moveDownParameterBButtom.setDisable(newValue == null || newValue.intValue() == outerInstance.regressorBListView.getItems().size() - 1);
			}
		}

		internal static StringConverter<bool> createArithmeticOperationTypeStringConverter()
		{
			return new StringConverterAnonymousInnerClass();
		}

		private class StringConverterAnonymousInnerClass : StringConverter<bool>
		{

			public override string toString(bool? type)
			{
				if (type == null)
				{
					return null;
				}

				if (type != null && type == true)
				{
					return i18N.getString("ProductSumRestrictionDialog.arithmetic.type.addition", "Addition");
				}

				return i18N.getString("ProductSumRestrictionDialog.arithmetic.type.subtraction", "Subtraction");
			}

			public override bool? fromString(string @string)
			{
				return Convert.ToBoolean(@string);
			}
		}
	}

}