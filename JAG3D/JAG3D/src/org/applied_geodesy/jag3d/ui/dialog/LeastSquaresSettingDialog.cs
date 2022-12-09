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

namespace org.applied_geodesy.jag3d.ui.dialog
{

	using DefaultValue = org.applied_geodesy.adjustment.DefaultValue;
	using EstimationType = org.applied_geodesy.adjustment.EstimationType;
	using UnscentedTransformationParameter = org.applied_geodesy.adjustment.UnscentedTransformationParameter;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DoubleSpinner = org.applied_geodesy.ui.spinner.DoubleSpinner;
	using DoubleTextField = org.applied_geodesy.ui.textfield.DoubleTextField;
	using ValueSupport = org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using CheckBox = javafx.scene.control.CheckBox;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using Spinner = javafx.scene.control.Spinner;
	using SpinnerValueFactory = javafx.scene.control.SpinnerValueFactory;
	using Tab = javafx.scene.control.Tab;
	using TabPane = javafx.scene.control.TabPane;
	using TextFormatter = javafx.scene.control.TextFormatter;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using VBox = javafx.scene.layout.VBox;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class LeastSquaresSettingDialog
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			settings = new LeastSquaresSettings(this);
		}


		public class LeastSquaresSettings
		{
			private readonly LeastSquaresSettingDialog outerInstance;

			public LeastSquaresSettings(LeastSquaresSettingDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal ObjectProperty<int> iteration = new SimpleObjectProperty<int>(50);
			internal ObjectProperty<int> principalComponents = new SimpleObjectProperty<int>(1);
			internal ObjectProperty<double> robustEstimationLimit = new SimpleObjectProperty<double>(DefaultValue.RobustEstimationLimit);
			internal BooleanProperty orientation = new SimpleBooleanProperty(true);
			internal BooleanProperty congruenceAnalysis = new SimpleBooleanProperty(false);
			internal BooleanProperty applyVarianceOfUnitWeight = new SimpleBooleanProperty(true);
			internal BooleanProperty exportCovarianceMatrix = new SimpleBooleanProperty(false);
			internal ObjectProperty<EstimationType> estimationType = new SimpleObjectProperty<EstimationType>(EstimationType.L2NORM);
			internal ObjectProperty<double> scalingParameterAlphaUT = new SimpleObjectProperty<double>(UnscentedTransformationParameter.Alpha);
			internal ObjectProperty<double> dampingParameterBetaUT = new SimpleObjectProperty<double>(UnscentedTransformationParameter.Beta);
			internal ObjectProperty<double> weightZero = new SimpleObjectProperty<double>(UnscentedTransformationParameter.WeightZero);

			public virtual ObjectProperty<int> iterationProperty()
			{
				return this.iteration;
			}

			public virtual int Iteration
			{
				get
				{
					return this.iterationProperty().get();
				}
				set
				{
					this.iterationProperty().set(value);
				}
			}


			public virtual ObjectProperty<int> principalComponentsProperty()
			{
				return this.principalComponents;
			}

			public virtual int PrincipalComponents
			{
				get
				{
					return this.principalComponentsProperty().get();
				}
				set
				{
					this.principalComponentsProperty().set(value);
				}
			}


			public virtual ObjectProperty<double> robustEstimationLimitProperty()
			{
				return this.robustEstimationLimit;
			}

			public virtual double RobustEstimationLimit
			{
				get
				{
					return this.robustEstimationLimitProperty().get();
				}
				set
				{
					this.robustEstimationLimitProperty().set(value);
				}
			}


			public virtual BooleanProperty orientationProperty()
			{
				return this.orientation;
			}

			public virtual bool Orientation
			{
				get
				{
					return this.orientationProperty().get();
				}
				set
				{
					this.orientationProperty().set(value);
				}
			}


			public virtual BooleanProperty congruenceAnalysisProperty()
			{
				return this.congruenceAnalysis;
			}

			public virtual bool CongruenceAnalysis
			{
				get
				{
					return this.congruenceAnalysisProperty().get();
				}
				set
				{
					this.congruenceAnalysisProperty().set(value);
				}
			}


			public virtual BooleanProperty applyVarianceOfUnitWeightProperty()
			{
				return this.applyVarianceOfUnitWeight;
			}

			public virtual bool ApplyVarianceOfUnitWeight
			{
				get
				{
					return this.applyVarianceOfUnitWeightProperty().get();
				}
				set
				{
					this.applyVarianceOfUnitWeightProperty().set(value);
				}
			}


			public virtual ObjectProperty<EstimationType> estimationTypeProperty()
			{
				return this.estimationType;
			}

			public virtual EstimationType EstimationType
			{
				get
				{
					return this.estimationTypeProperty().get();
				}
				set
				{
					this.estimationTypeProperty().set(value);
				}
			}


			public virtual BooleanProperty exportCovarianceMatrixProperty()
			{
				return this.exportCovarianceMatrix;
			}

			public virtual bool ExportCovarianceMatrix
			{
				get
				{
					return this.exportCovarianceMatrixProperty().get();
				}
				set
				{
					this.exportCovarianceMatrixProperty().set(value);
				}
			}


			public virtual ObjectProperty<double> scalingParameterAlphaUTProperty()
			{
				return this.scalingParameterAlphaUT;
			}

			public virtual double ScalingParameterAlphaUT
			{
				get
				{
					return this.scalingParameterAlphaUTProperty().get();
				}
				set
				{
					this.scalingParameterAlphaUTProperty().set(value);
				}
			}


			public virtual ObjectProperty<double> dampingParameterBetaUTProperty()
			{
				return this.dampingParameterBetaUT;
			}

			public virtual double DampingParameterBetaUT
			{
				get
				{
					return this.dampingParameterBetaUTProperty().get();
				}
				set
				{
					this.dampingParameterBetaUTProperty().set(value);
				}
			}


			public virtual ObjectProperty<double> weightZeroProperty()
			{
				return this.weightZero;
			}

			public virtual double WeightZero
			{
				get
				{
					return this.weightZeroProperty().get();
				}
				set
				{
					this.weightZeroProperty().set(value);
				}
			}

		}

		private class EstimationTypeChangeListener : ChangeListener<EstimationType>
		{
			private readonly LeastSquaresSettingDialog outerInstance;

			public EstimationTypeChangeListener(LeastSquaresSettingDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, EstimationType oldValue, EstimationType newValue) where T1 : org.applied_geodesy.adjustment.EstimationType
			{
				outerInstance.settings.EstimationType = newValue;
				outerInstance.estimationTypeComboBox.setValue(newValue);
			}
		}

		private bool enableUnscentedTransformation = false;
		private I18N i18n = I18N.Instance;
		private static LeastSquaresSettingDialog leastSquaresSettingDialog = new LeastSquaresSettingDialog();
		private Dialog<LeastSquaresSettings> dialog = null;
		private Window window;
		private ComboBox<EstimationType> estimationTypeComboBox;
		private LeastSquaresSettings settings;
		private Spinner<int> iterationSpinner;
		private Spinner<int> principalComponentSpinner;
		private DoubleSpinner robustSpinner;
		private DoubleTextField alphaTextField, betaTextField, weight0TextField;
		private CheckBox orientationApproximationCheckBox, congruenceAnalysisCheckBox, applyVarianceOfUnitWeightCheckBox, exportCovarianceMatrixCheckBox;
		private LeastSquaresSettingDialog()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public static Window Owner
		{
			set
			{
				leastSquaresSettingDialog.window = value;
			}
		}

		public static Optional<LeastSquaresSettings> showAndWait()
		{
			leastSquaresSettingDialog.init();
			leastSquaresSettingDialog.load();
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				leastSquaresSettingDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) leastSquaresSettingDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return leastSquaresSettingDialog.dialog.showAndWait();
		}


		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<LeastSquaresSettings>();
			this.dialog.setTitle(i18n.getString("LeastSquaresSettingDialog.title", "Least-squares"));
			this.dialog.setHeaderText(i18n.getString("LeastSquaresSettingDialog.header", "Least-squares properties"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			//		this.dialog.initStyle(StageStyle.UTILITY);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, LeastSquaresSettings>
		{
			private readonly LeastSquaresSettingDialog outerInstance;

			public CallbackAnonymousInnerClass(LeastSquaresSettingDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override LeastSquaresSettings call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					outerInstance.save();
					return outerInstance.settings;
				}
				return null;
			}
		}

		private Node createPane()
		{
			VBox contentPane = new VBox();

			this.estimationTypeComboBox = this.createEstimationTypeComboBox(EstimationType.L2NORM, i18n.getString("LeastSquaresSettingDialog.estimationtype.tooltip", "Set estimation method"));
			this.settings.estimationTypeProperty().addListener(new EstimationTypeChangeListener(this));
			this.estimationTypeComboBox.getSelectionModel().selectedItemProperty().addListener(new EstimationTypeChangeListener(this));
			VBox.setMargin(this.estimationTypeComboBox, new Insets(0, 0, 5, 0)); // oben, recht, unten, links

			contentPane.getChildren().add(this.estimationTypeComboBox);

			if (this.enableUnscentedTransformation)
			{
				TabPane tabPane = new TabPane();
				Tab tabGen = new Tab(i18n.getString("LeastSquaresSettingDialog.tab.leastsquares.label", "Least-squares"), this.createGeneralSettingPane());
				tabGen.setTooltip(new Tooltip(i18n.getString("LeastSquaresSettingDialog.tab.leastsquares.tooltip", "Least-squares options")));
				tabGen.setClosable(false);
				tabPane.getTabs().add(tabGen);

				Tab tabUT = new Tab(i18n.getString("LeastSquaresSettingDialog.tab.unscented_transformation.label", "Unscented transformation"), this.createUnscentedTransformationSettingPane());
				tabUT.setTooltip(new Tooltip(i18n.getString("LeastSquaresSettingDialog.tab.unscented_transformation.tooltip", "Unscented transformation parameters")));
				tabUT.setClosable(false);
				tabPane.getTabs().add(tabUT);

				tabPane.setPadding(new Insets(5, 0, 0, 0)); // oben, recht, unten, links
				tabPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
				tabPane.setMaxSize(double.MaxValue,double.MaxValue);

				contentPane.getChildren().add(tabPane);
			}
			else
			{
				contentPane.getChildren().add(this.createGeneralSettingPane());
			}

			contentPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			contentPane.setMaxSize(double.MaxValue,double.MaxValue);

			Platform.runLater(() =>
			{
			estimationTypeComboBox.requestFocus();
			});

			return contentPane;
		}

		private Node createUnscentedTransformationSettingPane()
		{
			Label alphaLabel = new Label(i18n.getString("LeastSquaresSettingDialog.ut.scaling.label", "Scaling \u03B1:"));
			Label betaLabel = new Label(i18n.getString("LeastSquaresSettingDialog.ut.damping.label", "Damping \u03B2:"));
			Label utWeight0Label = new Label(i18n.getString("LeastSquaresSettingDialog.ut.weight0.label", "Weight w0:"));

			this.alphaTextField = this.createDoubleTextField(UnscentedTransformationParameter.Alpha, DoubleTextField.ValueSupport.EXCLUDING_INCLUDING_INTERVAL, 0.0, 1.0, i18n.getString("LeastSquaresSettingDialog.ut.scaling.tooltip", "Defines spread of sigma points around the mean value, if \u03B1 \u2260 1"));
			this.betaTextField = this.createDoubleTextField(UnscentedTransformationParameter.Beta, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT, double.NegativeInfinity, double.PositiveInfinity, i18n.getString("LeastSquaresSettingDialog.ut.damping.tooltip", "Considers prior knowledge of the distribution, if \u03B2 \u2260 0"));
			this.weight0TextField = this.createDoubleTextField(UnscentedTransformationParameter.WeightZero, DoubleTextField.ValueSupport.INCLUDING_EXCLUDING_INTERVAL, double.NegativeInfinity, 1.0, i18n.getString("LeastSquaresSettingDialog.ut.weight0.tooltip", "Set the weight related to the zero sigma point (MUT: w0 \u003c 1, SUT: 0 \u2264 w0 \u003c 1)"));

			this.alphaTextField.numberProperty().bindBidirectional(this.settings.scalingParameterAlphaUTProperty());
			this.betaTextField.numberProperty().bindBidirectional(this.settings.dampingParameterBetaUTProperty());
			this.weight0TextField.numberProperty().bindBidirectional(this.settings.weightZeroProperty());

			alphaLabel.setLabelFor(this.alphaTextField);
			betaLabel.setLabelFor(this.betaTextField);
			utWeight0Label.setLabelFor(this.weight0TextField);

			GridPane gridPane = new GridPane();
			gridPane.setMaxSize(double.MaxValue,double.MaxValue);
			gridPane.setAlignment(Pos.TOP_CENTER);
			gridPane.setPadding(new Insets(5,15,5,15)); // oben, recht, unten, links
	//		gridPane.setGridLinesVisible(true);

			GridPane.setHgrow(alphaLabel, Priority.NEVER);
			GridPane.setHgrow(betaLabel, Priority.NEVER);
			GridPane.setHgrow(utWeight0Label, Priority.NEVER);

			GridPane.setHgrow(this.alphaTextField, Priority.ALWAYS);
			GridPane.setHgrow(this.betaTextField, Priority.ALWAYS);
			GridPane.setHgrow(this.weight0TextField, Priority.ALWAYS);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 2);
			Insets insetsRight = new Insets(5, 2, 5, 7);

			GridPane.setMargin(alphaLabel, insetsLeft);
			GridPane.setMargin(this.alphaTextField, insetsRight);

			GridPane.setMargin(betaLabel, insetsLeft);
			GridPane.setMargin(this.betaTextField, insetsRight);

			GridPane.setMargin(utWeight0Label, insetsLeft);
			GridPane.setMargin(this.weight0TextField, insetsRight);

			int row = 0;
			gridPane.add(utWeight0Label, 0, ++row);
			gridPane.add(this.weight0TextField, 1, row);

			gridPane.add(alphaLabel, 0, ++row);
			gridPane.add(this.alphaTextField, 1, row, 2, 1);

			gridPane.add(betaLabel, 0, ++row);
			gridPane.add(this.betaTextField, 1, row, 2, 1);

			return gridPane;
		}

		private Node createGeneralSettingPane()
		{
			Label iterationLabel = new Label(i18n.getString("LeastSquaresSettingDialog.iterations.label", "Maximum number of iterations:"));
			this.iterationSpinner = this.createIntegerSpinner(0, DefaultValue.MaximalNumberOfIterations, 10, i18n.getString("LeastSquaresSettingDialog.iterations.tooltip", "Set maximum permissible iteration value"));
			iterationLabel.setLabelFor(this.iterationSpinner);
			iterationLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label principalComponentLabel = new Label(i18n.getString("LeastSquaresSettingDialog.principal_components.label", "Number of principal components:"));
			this.principalComponentSpinner = this.createIntegerSpinner(0, int.MaxValue, 1, i18n.getString("LeastSquaresSettingDialog.principal_components.tooltip", "Set number of principal components to be estimated"));
			principalComponentLabel.setLabelFor(this.principalComponentSpinner);
			principalComponentLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			Label robustLabel = new Label(i18n.getString("LeastSquaresSettingDialog.robust.label", "Robust estimation limit:"));
			this.robustSpinner = this.createDoubleSpinner(1.5, Math.Max(DefaultValue.RobustEstimationLimit, 6.0), 0.5, i18n.getString("LeastSquaresSettingDialog.robust.tooltip", "Set robust estimation limit of BIBER estimator"));
			robustLabel.setLabelFor(this.robustSpinner);
			robustLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			this.orientationApproximationCheckBox = this.createCheckBox(i18n.getString("LeastSquaresSettingDialog.orientation.label", "Orientation approximation"), i18n.getString("LeastSquaresSettingDialog.orientation.tooltip", "If checked, orientation approximations of direction sets will be estimated before network adjustment starts"));

			this.congruenceAnalysisCheckBox = this.createCheckBox(i18n.getString("LeastSquaresSettingDialog.congruenceanalysis.label", "Congruence analysis"), i18n.getString("LeastSquaresSettingDialog.congruenceanalysis.tooltip", "If checked, a congruence analysis will be carry out in case of a free network adjustment"));

			this.applyVarianceOfUnitWeightCheckBox = this.createCheckBox(i18n.getString("LeastSquaresSettingDialog.applyvarianceofunitweight.label", "Variance of the unit weight"), i18n.getString("LeastSquaresSettingDialog.applyvarianceofunitweight.tooltip", "If checked, the estimated variance of the unit weight will be applied to scale the variance-covariance matrix"));

			this.exportCovarianceMatrixCheckBox = this.createCheckBox(i18n.getString("LeastSquaresSettingDialog.covariance.label", "Export variance-covariance matrix"), i18n.getString("LeastSquaresSettingDialog.covariance.tooltip", "If checked, variance-covariance matrix will be exported to the working directory"));

			this.exportCovarianceMatrixCheckBox.selectedProperty().bindBidirectional(this.settings.exportCovarianceMatrixProperty());
			this.orientationApproximationCheckBox.selectedProperty().bindBidirectional(this.settings.orientationProperty());
			this.congruenceAnalysisCheckBox.selectedProperty().bindBidirectional(this.settings.congruenceAnalysisProperty());
			this.applyVarianceOfUnitWeightCheckBox.selectedProperty().bindBidirectional(this.settings.applyVarianceOfUnitWeightProperty());
			this.iterationSpinner.getValueFactory().valueProperty().bindBidirectional(this.settings.iterationProperty());
			this.principalComponentSpinner.getValueFactory().valueProperty().bindBidirectional(this.settings.principalComponentsProperty());
			this.robustSpinner.getValueFactory().valueProperty().bindBidirectional(this.settings.robustEstimationLimitProperty());

			GridPane gridPane = new GridPane();
			gridPane.setMaxSize(double.MaxValue,double.MaxValue);

	//		gridPane.setHgap(20);
	//		gridPane.setVgap(10);
			gridPane.setAlignment(Pos.TOP_CENTER);
			gridPane.setPadding(new Insets(5,15,5,15)); // oben, recht, unten, links
			//gridPane.setGridLinesVisible(true);

			GridPane.setHgrow(iterationLabel, Priority.NEVER);
			GridPane.setHgrow(principalComponentLabel, Priority.NEVER);
			GridPane.setHgrow(robustLabel, Priority.NEVER);

			GridPane.setHgrow(this.orientationApproximationCheckBox, Priority.ALWAYS);
			GridPane.setHgrow(this.applyVarianceOfUnitWeightCheckBox, Priority.ALWAYS);
			GridPane.setHgrow(this.congruenceAnalysisCheckBox, Priority.ALWAYS);
			GridPane.setHgrow(this.exportCovarianceMatrixCheckBox, Priority.ALWAYS);

			GridPane.setHgrow(this.robustSpinner, Priority.ALWAYS);
			GridPane.setHgrow(this.principalComponentSpinner, Priority.ALWAYS);
			GridPane.setHgrow(this.iterationSpinner, Priority.ALWAYS);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsCenter = new Insets(5, 2, 5, 2);
			Insets insetsTop = new Insets(10, 2, 5, 2);
			Insets insetsLeft = new Insets(5, 7, 5, 2);
			Insets insetsRight = new Insets(5, 2, 5, 7);

			GridPane.setMargin(this.applyVarianceOfUnitWeightCheckBox, insetsTop);
			GridPane.setMargin(this.orientationApproximationCheckBox, insetsCenter);

			GridPane.setMargin(this.congruenceAnalysisCheckBox, insetsTop);
			GridPane.setMargin(this.exportCovarianceMatrixCheckBox, insetsCenter);

			GridPane.setMargin(iterationLabel, insetsLeft);
			GridPane.setMargin(this.iterationSpinner, insetsRight);

			GridPane.setMargin(robustLabel, insetsLeft);
			GridPane.setMargin(this.robustSpinner, insetsRight);

			GridPane.setMargin(principalComponentLabel, insetsLeft);
			GridPane.setMargin(this.principalComponentSpinner, insetsRight);

			int row = 0;
			gridPane.add(this.applyVarianceOfUnitWeightCheckBox, 0, ++row, 2, 1);
			gridPane.add(this.orientationApproximationCheckBox, 0, ++row, 2, 1);

			gridPane.add(iterationLabel, 0, ++row);
			gridPane.add(this.iterationSpinner, 1, row);

			gridPane.add(robustLabel, 0, ++row);
			gridPane.add(this.robustSpinner, 1, row);

			gridPane.add(principalComponentLabel, 0, ++row);
			gridPane.add(this.principalComponentSpinner, 1, row);

			gridPane.add(this.congruenceAnalysisCheckBox, 0, ++row, 2, 1);
			gridPane.add(this.exportCovarianceMatrixCheckBox, 0, ++row, 2, 1);

			return gridPane;
		}


		private ComboBox<EstimationType> createEstimationTypeComboBox(EstimationType item, string tooltip)
		{
			ComboBox<EstimationType> typeComboBox = new ComboBox<EstimationType>();
			EstimationType[] estimationTypeArray = EstimationType.values();
			if (!this.enableUnscentedTransformation)
			{
				IList<EstimationType> estimationTypeList = new List<EstimationType> {estimationTypeArray};
				estimationTypeList.Remove(EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION);
				estimationTypeList.Remove(EstimationType.MODIFIED_UNSCENTED_TRANSFORMATION);
				estimationTypeArray = ((List<EstimationType>)estimationTypeList).ToArray();
				if (item == EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION || item == EstimationType.MODIFIED_UNSCENTED_TRANSFORMATION)
				{
					item = EstimationType.L2NORM;
				}
			}
			typeComboBox.getItems().setAll(estimationTypeArray); // EstimationType.values()
			typeComboBox.getSelectionModel().select(item);
			typeComboBox.setConverter(new StringConverterAnonymousInnerClass(this));
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMinWidth(150);
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		private class StringConverterAnonymousInnerClass : StringConverter<EstimationType>
		{
			private readonly LeastSquaresSettingDialog outerInstance;

			public StringConverterAnonymousInnerClass(LeastSquaresSettingDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override string toString(EstimationType type)
			{
				if (type == null)
				{
					return null;
				}
				switch (type.innerEnumValue)
				{
				case EstimationType.InnerEnum.L1NORM:
					return outerInstance.i18n.getString("LeastSquaresSettingDialog.estimationtype.l1norm.label", "Robust estimation (L1-Norm)");
				case EstimationType.InnerEnum.L2NORM:
					return outerInstance.i18n.getString("LeastSquaresSettingDialog.estimationtype.l2norm.label", "Least-squares adjustment (L2-Norm)");
				case EstimationType.InnerEnum.SIMULATION:
					return outerInstance.i18n.getString("LeastSquaresSettingDialog.estimationtype.simulation.label", "Simulation (Pre-analysis)");
				case EstimationType.InnerEnum.MODIFIED_UNSCENTED_TRANSFORMATION:
					return outerInstance.i18n.getString("LeastSquaresSettingDialog.estimationtype.mut.label", "Modified unscented transformation (MUT)");
				case EstimationType.InnerEnum.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION:
					return outerInstance.i18n.getString("LeastSquaresSettingDialog.estimationtype.sut.label", "Spherical simplex unscented transformation (SUT)");
				}
				return null;
			}

			public override EstimationType fromString(string @string)
			{
				return EstimationType.valueOf(@string);
			}
		}

		private CheckBox createCheckBox(string title, string tooltip)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			CheckBox checkBox = new CheckBox();
			checkBox.setGraphic(label);
			checkBox.setTooltip(new Tooltip(tooltip));
			checkBox.setMinHeight(Control.USE_PREF_SIZE);
			checkBox.setMaxHeight(double.MaxValue);
			return checkBox;
		}

		private DoubleTextField createDoubleTextField(double value, DoubleTextField.ValueSupport valueSupport, double lowerBoundary, double upperBoundary, string tooltip)
		{
			DoubleTextField field = new DoubleTextField(value, CellValueType.STATISTIC, false, valueSupport, lowerBoundary, upperBoundary);
			field.setTooltip(new Tooltip(tooltip));
			field.setAlignment(Pos.CENTER_RIGHT);
			field.setMinHeight(Control.USE_PREF_SIZE);
			field.setMaxHeight(double.MaxValue);
			return field;
		}

		private Spinner<int> createIntegerSpinner(int min, int max, int amountToStepBy, string tooltip)
		{
			NumberFormat numberFormat = NumberFormat.getInstance(Locale.ENGLISH);
			numberFormat.setMaximumFractionDigits(0);
			numberFormat.setMinimumFractionDigits(0);
			numberFormat.setGroupingUsed(false);

			StringConverter<int> converter = new StringConverterAnonymousInnerClass2(this, numberFormat);

			SpinnerValueFactory.IntegerSpinnerValueFactory integerFactory = new SpinnerValueFactory.IntegerSpinnerValueFactory(min, max);
			Spinner<int> integerSpinner = new Spinner<int>();
			integerSpinner.setEditable(true);
			integerSpinner.setValueFactory(integerFactory);
			//integerSpinner.getStyleClass().add(Spinner.STYLE_CLASS_ARROWS_ON_RIGHT_HORIZONTAL);

			integerFactory.setConverter(converter);
			integerFactory.setAmountToStepBy(amountToStepBy);

			TextFormatter<int> formatter = new TextFormatter<int>(integerFactory.getConverter(), integerFactory.getValue());
			integerSpinner.getEditor().setTextFormatter(formatter);
			integerSpinner.getEditor().setAlignment(Pos.BOTTOM_RIGHT);
			integerFactory.valueProperty().bindBidirectional(formatter.valueProperty());

			integerSpinner.setMinWidth(75);
			integerSpinner.setPrefWidth(100);
			integerSpinner.setMaxWidth(double.MaxValue);
			integerSpinner.setTooltip(new Tooltip(tooltip));

			integerFactory.valueProperty().addListener(new ChangeListenerAnonymousInnerClass(this, integerFactory));

			return integerSpinner;
		}

		private class StringConverterAnonymousInnerClass2 : StringConverter<int>
		{
			private readonly LeastSquaresSettingDialog outerInstance;

			private NumberFormat numberFormat;

			public StringConverterAnonymousInnerClass2(LeastSquaresSettingDialog outerInstance, NumberFormat numberFormat)
			{
				this.outerInstance = outerInstance;
				this.numberFormat = numberFormat;
			}

			public override int? fromString(string s)
			{
				if (string.ReferenceEquals(s, null) || s.Trim().Length == 0)
				{
					return null;
				}
				else
				{
					try
					{
						return numberFormat.parse(s).intValue();
					}
					catch (Exception nfe)
					{
						Console.WriteLine(nfe.ToString());
						Console.Write(nfe.StackTrace);
					}
				}
				return null;
			}

			public override string toString(int? d)
			{
				return d == null ? "" : numberFormat.format(d);
			}
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<int>
		{
			private readonly LeastSquaresSettingDialog outerInstance;

			private SpinnerValueFactory.IntegerSpinnerValueFactory integerFactory;

			public ChangeListenerAnonymousInnerClass(LeastSquaresSettingDialog outerInstance, SpinnerValueFactory.IntegerSpinnerValueFactory integerFactory)
			{
				this.outerInstance = outerInstance;
				this.integerFactory = integerFactory;
			}

			public override void changed<T1>(ObservableValue<T1> observable, int? oldValue, int? newValue) where T1 : int
			{
				if (newValue == null)
				{
					integerFactory.setValue(oldValue);
				}
			}
		}

		private DoubleSpinner createDoubleSpinner(double min, double max, double amountToStepBy, string tooltip)
		{
			DoubleSpinner doubleSpinner = new DoubleSpinner(CellValueType.STATISTIC, min, max, amountToStepBy);
			doubleSpinner.setMinWidth(75);
			doubleSpinner.setPrefWidth(100);
			doubleSpinner.setMaxWidth(double.MaxValue);
			doubleSpinner.setTooltip(new Tooltip(tooltip));
			return doubleSpinner;
		}

		private void save()
		{
			try
			{
				SQLManager.Instance.save(this.settings);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("LeastSquaresSettingDialog.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("LeastSquaresSettingDialog.message.error.save.exception.header", "Error, could not save least-squares settings to database."), i18n.getString("LeastSquaresSettingDialog.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private void load()
		{
			try
			{
				SQLManager.Instance.load(this.settings);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("LeastSquaresSettingDialog.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("LeastSquaresSettingDialog.message.error.load.exception.header", "Error, could not load least-squares settings from database."), i18n.getString("LeastSquaresSettingDialog.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		public static bool EnableUnscentedTransformation
		{
			set
			{
				leastSquaresSettingDialog.enableUnscentedTransformation = value;
			}
		}
	}

}