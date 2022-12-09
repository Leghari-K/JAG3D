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
	using TrigonometricRestriction = org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction;
	using TrigonometricFunctionType = org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType;
	using LaTexLabel = org.applied_geodesy.ui.tex.LaTexLabel;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using CheckBox = javafx.scene.control.CheckBox;
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
	using StringConverter = javafx.util.StringConverter;

	public class TrigonometricRestrictionDialog
	{

		private static I18N i18N = I18N.Instance;
		private static TrigonometricRestrictionDialog restrictionTypeDialog = new TrigonometricRestrictionDialog();
		private Dialog<TrigonometricRestriction> dialog = null;
		private LaTexLabel latexLabel;
		private CheckBox invertCheckbox;
		private ComboBox<TrigonometricRestriction.TrigonometricFunctionType> trigonometricTypeComboBox;
		private ComboBox<UnknownParameter> regressorComboBox;
		private ComboBox<UnknownParameter> regressandComboBox;
		private TextField descriptionTextField;
		private Window window;
		private TrigonometricRestriction restriction;

		private TrigonometricRestrictionDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				restrictionTypeDialog.window = value;
			}
		}

		public static Optional<TrigonometricRestriction> showAndWait(ObservableList<UnknownParameter> unknownParameters, TrigonometricRestriction trigonometricRestriction)
		{
			restrictionTypeDialog.init();
			restrictionTypeDialog.UnknownParameters = unknownParameters;
			restrictionTypeDialog.Restriction = trigonometricRestriction;
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
				this.regressorComboBox.setItems(unknownParameterList);
				this.regressandComboBox.setItems(unknownParameterList);
			}
		}

		private TrigonometricRestriction Restriction
		{
			set
			{
				if (this.restriction != null)
				{
					this.descriptionTextField.textProperty().unbindBidirectional(this.restriction.descriptionProperty());
					this.regressandComboBox.valueProperty().unbindBidirectional(this.restriction.regressandProperty());
					this.regressorComboBox.valueProperty().unbindBidirectional(this.restriction.regressorProperty());
					this.invertCheckbox.selectedProperty().unbindBidirectional(this.restriction.invertProperty());
					this.trigonometricTypeComboBox.valueProperty().unbindBidirectional(this.restriction.trigonometricFunctionTypeProperty());
				}
    
				this.restriction = value;
				this.descriptionTextField.textProperty().bindBidirectional(this.restriction.descriptionProperty());
				this.regressandComboBox.valueProperty().bindBidirectional(this.restriction.regressandProperty());
				this.regressorComboBox.valueProperty().bindBidirectional(this.restriction.regressorProperty());
				this.invertCheckbox.selectedProperty().bindBidirectional(this.restriction.invertProperty());
				this.trigonometricTypeComboBox.valueProperty().bindBidirectional(this.restriction.trigonometricFunctionTypeProperty());
				this.latexLabel.Tex = this.restriction.toLaTex();
    
				this.regressorComboBox.setDisable(value.Indispensable);
				this.regressandComboBox.setDisable(value.Indispensable);
				this.trigonometricTypeComboBox.setDisable(value.Indispensable);
				this.invertCheckbox.setDisable(value.Indispensable);
    
				if (!value.Indispensable)
				{
					UnknownParameter regressand = this.restriction.Regressand;
					UnknownParameter regressor = this.restriction.Regressor;
					TrigonometricRestriction.TrigonometricFunctionType type = this.restriction.TrigonometricFunctionType;
					if (regressand == null)
					{
						this.regressandComboBox.getSelectionModel().clearAndSelect(0);
					}
					if (regressor == null)
					{
						this.regressorComboBox.getSelectionModel().clearAndSelect(0);
					}
					if (type == null)
					{
						this.trigonometricTypeComboBox.getSelectionModel().clearAndSelect(0);
					}
				}
			}
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<TrigonometricRestriction>();
			this.dialog.setTitle(i18N.getString("TrigonometricRestrictionDialog.title", "Trigonometric restriction"));
			this.dialog.setHeaderText(i18N.getString("TrigonometricRestrictionDialog.header", "Trigonometric function"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, TrigonometricRestriction>
		{
			private readonly TrigonometricRestrictionDialog outerInstance;

			public CallbackAnonymousInnerClass(TrigonometricRestrictionDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override TrigonometricRestriction call(ButtonType buttonType)
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

			Label equationLabel = new Label(i18N.getString("TrigonometricRestrictionDialog.equation.label", "Equation:"));
			Label descriptionLabel = new Label(i18N.getString("TrigonometricRestrictionDialog.description.label", "Description:"));
			Label regressandLabel = new Label(i18N.getString("TrigonometricRestrictionDialog.regressand.label", "Regressand c:"));
			Label regressorLabel = new Label(i18N.getString("TrigonometricRestrictionDialog.regressor.label", "Regressor a:"));
			Label functionTypeLabel = new Label(i18N.getString("TrigonometricRestrictionDialog.trigonometry.type.label", "Trigonometric function:"));

			regressorLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			regressorLabel.setMaxWidth(double.MaxValue);

			regressandLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			regressandLabel.setMaxWidth(double.MaxValue);

			equationLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			equationLabel.setMaxWidth(double.MaxValue);

			descriptionLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			descriptionLabel.setMaxWidth(double.MaxValue);

			functionTypeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			functionTypeLabel.setMaxWidth(double.MaxValue);

			this.descriptionTextField = DialogUtil.createTextField(i18N.getString("TrigonometricRestrictionDialog.description.tooltip", "Description of parameter restriction"), i18N.getString("TrigonometricRestrictionDialog.description.prompt", "Restriction description"));

			this.regressorComboBox = DialogUtil.createUnknownParameterComboBox(UnknownParameterDialog.createUnknownParameterCellFactory(), i18N.getString("TrigonometricRestrictionDialog.regressor.tooltip", "Select regressor a"));

			this.regressandComboBox = DialogUtil.createUnknownParameterComboBox(UnknownParameterDialog.createUnknownParameterCellFactory(), i18N.getString("TrigonometricRestrictionDialog.regressand.tooltip", "Select regressand c"));

			this.trigonometricTypeComboBox = DialogUtil.createTrigonometricFunctionTypeComboBox(createTrigonometricFunctionTypeStringConverter(), i18N.getString("TrigonometricRestrictionDialog.trigonometry.type.tooltip", "Select trigonometric function"));

			this.invertCheckbox = DialogUtil.createCheckBox(i18N.getString("TrigonometricRestrictionDialog.trigonometry.invert.label", "Inverted trigonometric function"), i18N.getString("TrigonometricRestrictionDialog.trigonometry.invert.tooltip", "If checked, the inverted trigonometric function will be used, i.e., asin(a), acos(a), atan(a), or acot(a)"));

			this.latexLabel = new LaTexLabel();
			this.latexLabel.setPrefSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.latexLabel.setMaxSize(double.MaxValue, double.MaxValue);

			equationLabel.setLabelFor(this.latexLabel);
			descriptionLabel.setLabelFor(this.descriptionTextField);
			regressandLabel.setLabelFor(this.regressandComboBox);
			regressorLabel.setLabelFor(this.regressorComboBox);
			functionTypeLabel.setLabelFor(this.trigonometricTypeComboBox);

			GridPane.setHgrow(equationLabel, Priority.NEVER);
			GridPane.setHgrow(descriptionLabel, Priority.NEVER);
			GridPane.setHgrow(regressandLabel, Priority.NEVER);
			GridPane.setHgrow(regressorLabel, Priority.NEVER);
			GridPane.setHgrow(functionTypeLabel, Priority.NEVER);

			GridPane.setHgrow(this.latexLabel, Priority.ALWAYS);
			GridPane.setHgrow(this.invertCheckbox, Priority.ALWAYS);
			GridPane.setHgrow(this.descriptionTextField, Priority.ALWAYS);
			GridPane.setHgrow(this.regressandComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.regressorComboBox, Priority.ALWAYS);
			GridPane.setVgrow(this.trigonometricTypeComboBox, Priority.ALWAYS);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 5);
			Insets insetsRight = new Insets(5, 0, 5, 7);

			GridPane.setMargin(equationLabel, insetsLeft);
			GridPane.setMargin(regressandLabel, insetsLeft);
			GridPane.setMargin(regressorLabel, insetsLeft);
			GridPane.setMargin(descriptionLabel, insetsLeft);
			GridPane.setMargin(functionTypeLabel, insetsLeft);

			GridPane.setMargin(this.latexLabel, insetsRight);
			GridPane.setMargin(this.regressandComboBox, insetsRight);
			GridPane.setMargin(this.regressorComboBox, insetsRight);
			GridPane.setMargin(this.descriptionTextField, insetsRight);
			GridPane.setMargin(this.trigonometricTypeComboBox, insetsRight);

			GridPane.setMargin(this.invertCheckbox, new Insets(5, 7, 5, 7));

			int row = 0;

			gridPane.add(equationLabel, 0, row); // column, row, columnspan, rowspan,
			gridPane.add(this.latexLabel, 1, row++);

			gridPane.add(descriptionLabel, 0, row);
			gridPane.add(this.descriptionTextField, 1, row++);

			gridPane.add(regressandLabel, 0, row);
			gridPane.add(this.regressandComboBox, 1, row++);

			gridPane.add(regressorLabel, 0, row);
			gridPane.add(this.regressorComboBox, 1, row++);

			gridPane.add(functionTypeLabel, 0, row);
			gridPane.add(this.trigonometricTypeComboBox, 1, row++);

			gridPane.add(this.invertCheckbox, 0, row++, 2, 1);

			return gridPane;
		}

		internal static StringConverter<TrigonometricRestriction.TrigonometricFunctionType> createTrigonometricFunctionTypeStringConverter()
		{
			return new StringConverterAnonymousInnerClass();
		}

		private class StringConverterAnonymousInnerClass : StringConverter<TrigonometricRestriction.TrigonometricFunctionType>
		{

			public override string toString(TrigonometricRestriction.TrigonometricFunctionType functionType)
			{
				return getTrigonometricFunctionTypeLabel(functionType);
			}

			public override TrigonometricRestriction.TrigonometricFunctionType fromString(string @string)
			{
				return (TrigonometricRestriction.TrigonometricFunctionType)Enum.Parse(typeof(TrigonometricRestriction.TrigonometricFunctionType), @string);
			}
		}

		internal static string getTrigonometricFunctionTypeLabel(TrigonometricRestriction.TrigonometricFunctionType functionType)
		{
			if (functionType == null)
			{
				return null;
			}

			switch (functionType)
			{
			case TrigonometricRestriction.TrigonometricFunctionType.SINE:
				return i18N.getString("TrigonometricRestrictionDialog.trigonometry.type.sine", "Sine sin(a)");

			case TrigonometricRestriction.TrigonometricFunctionType.COSINE:
				return i18N.getString("TrigonometricRestrictionDialog.trigonometry.type.cosine", "Cosine cos(a)");

			case TrigonometricRestriction.TrigonometricFunctionType.TANGENT:
				return i18N.getString("TrigonometricRestrictionDialog.trigonometry.type.tangent", "Tangent tan(a)");

			case TrigonometricRestriction.TrigonometricFunctionType.COTANGENT:
				return i18N.getString("TrigonometricRestrictionDialog.trigonometry.type.cotangent", "Cotangent cot(a)");
			}

			throw new System.ArgumentException("Error, unknown trigonometric function type " + functionType + "!");
		}

	}
}