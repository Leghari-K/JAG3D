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

	using TestStatisticDefinition = org.applied_geodesy.adjustment.statistic.TestStatisticDefinition;
	using TestStatisticType = org.applied_geodesy.adjustment.statistic.TestStatisticType;

	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterEventType = org.applied_geodesy.util.FormatterEventType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;
	using DoubleSpinner = org.applied_geodesy.ui.spinner.DoubleSpinner;

	using Platform = javafx.application.Platform;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using CheckBox = javafx.scene.control.CheckBox;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using SpinnerValueFactory = javafx.scene.control.SpinnerValueFactory;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class TestStatisticDialog : FormatterChangedListener
	{
		private static I18N i18N = I18N.Instance;
		private static TestStatisticDialog testStatisticDialog = new TestStatisticDialog();
		private FormatterOptions options = FormatterOptions.Instance;
		private Dialog<TestStatisticDefinition> dialog = null;
		private DoubleSpinner probabilityValueSpinner, testPowerSpinner;
		private ComboBox<TestStatisticType> testStatisticTypeComboBox;
		private CheckBox familywiseErrorRateCheckBox;
		private Window window;
		private TestStatisticDefinition testStatisticDefinition;
		private Label probabilityValueLabel, testPowerLabel;

		private TestStatisticDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				testStatisticDialog.window = value;
			}
		}

		public static Optional<TestStatisticDefinition> showAndWait(TestStatisticDefinition testStatisticDefinition)
		{
			testStatisticDialog.init();
			testStatisticDialog.TestStatisticDefinition = testStatisticDefinition;
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				testStatisticDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) testStatisticDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return testStatisticDialog.dialog.showAndWait();
		}

		private TestStatisticDefinition TestStatisticDefinition
		{
			set
			{
				this.testStatisticDefinition = value;
    
				this.familywiseErrorRateCheckBox.setSelected(this.testStatisticDefinition.FamilywiseErrorRate);
				this.testStatisticTypeComboBox.getSelectionModel().select(this.testStatisticDefinition.TestStatisticType);
    
				SpinnerValueFactory.DoubleSpinnerValueFactory probabilityValueSpinnerFactory = (SpinnerValueFactory.DoubleSpinnerValueFactory)this.probabilityValueSpinner.getValueFactory();
				SpinnerValueFactory.DoubleSpinnerValueFactory testPowerSpinnerFactory = (SpinnerValueFactory.DoubleSpinnerValueFactory)this.testPowerSpinner.getValueFactory();
    
				double probabilityValue = this.testStatisticDefinition.ProbabilityValue;
				double powerOfTest = this.testStatisticDefinition.PowerOfTest;
    
				probabilityValue = Math.Max(Math.Min(this.options.convertPercentToView(probabilityValue), probabilityValueSpinnerFactory.getMax()), probabilityValueSpinnerFactory.getMin());
				powerOfTest = Math.Max(Math.Min(this.options.convertPercentToView(powerOfTest), testPowerSpinnerFactory.getMax()), testPowerSpinnerFactory.getMin());
    
				probabilityValueSpinnerFactory.setValue(probabilityValue);
				testPowerSpinnerFactory.setValue(powerOfTest);
			}
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<TestStatisticDefinition>();
			this.dialog.setTitle(i18N.getString("TestStatisticDialog.title", "Test statistic"));
			this.dialog.setHeaderText(i18N.getString("TestStatisticDialog.header", "Test statistic properties"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			//		this.dialog.initStyle(StageStyle.UTILITY);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
			// add formatter listener
			this.options.addFormatterChangedListener(this);
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, TestStatisticDefinition>
		{
			private readonly TestStatisticDialog outerInstance;

			public CallbackAnonymousInnerClass(TestStatisticDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override TestStatisticDefinition call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					double probabilityValue = outerInstance.probabilityValueSpinner.Number.doubleValue();
					double powerOfTest = outerInstance.testPowerSpinner.Number.doubleValue();

					bool familywiseErrorRate = outerInstance.familywiseErrorRateCheckBox.isSelected();
					TestStatisticType testStatisticType = outerInstance.testStatisticTypeComboBox.getSelectionModel().getSelectedItem();

					outerInstance.testStatisticDefinition.TestStatisticType = testStatisticType;
					outerInstance.testStatisticDefinition.FamilywiseErrorRate = familywiseErrorRate;
					outerInstance.testStatisticDefinition.ProbabilityValue = probabilityValue;
					outerInstance.testStatisticDefinition.PowerOfTest = powerOfTest;

					return outerInstance.testStatisticDefinition;
				}
				return null;
			}
		}

		private Node createPane()
		{
			string frmPercentUnit = this.options.FormatterOptions[CellValueType.PERCENTAGE].getUnit().toFormattedAbbreviation();
			string labelProbabilityValue = String.format(Locale.ENGLISH, "%s%s:", i18N.getString("TestStatisticDialog.probability.label", "Probability value \u03B1"), frmPercentUnit.Trim().Length == 0 ? "" : " " + frmPercentUnit);
			string labelTestPower = String.format(Locale.ENGLISH, "%s%s:", i18N.getString("TestStatisticDialog.testpower.label", "Power of test 1 - \u03B2"), frmPercentUnit.Trim().Length == 0 ? "" : " " + frmPercentUnit);

			string tooltipProbabilityValue = i18N.getString("TestStatisticDialog.probability.tooltip", "Set probability value (type I error)");
			string tooltipTestPower = i18N.getString("TestStatisticDialog.testpower.tooltip", "Set power of test (type II error)");

			string labelFamilywiseErrorRate = i18N.getString("TestStatisticDialog.familywiseerror.label", "Familywise error rate");
			string tooltipFamilywiseErrorRate = i18N.getString("TestStatisticDialog.familywiseerror.tooltip", "If checked, probability value \u03B1 defines familywise error rate");

			this.probabilityValueLabel = new Label(labelProbabilityValue);
			this.testPowerLabel = new Label(labelTestPower);

			this.familywiseErrorRateCheckBox = DialogUtil.createCheckBox(labelFamilywiseErrorRate, tooltipFamilywiseErrorRate);

			this.probabilityValueSpinner = DialogUtil.createDoubleSpinner(CellValueType.PERCENTAGE, 0.0005, 0.30, 0.01, tooltipProbabilityValue);
			this.testPowerSpinner = DialogUtil.createDoubleSpinner(CellValueType.PERCENTAGE, 0.50, 0.9995, 0.01, tooltipTestPower);

			this.testStatisticTypeComboBox = DialogUtil.createTestStatisticTypeComboBox(createTestStatisticTypeStringConverter(), i18N.getString("TestStatisticDialog.type.tooltip", "Select method for type I error adaption"));

			probabilityValueLabel.setLabelFor(this.probabilityValueSpinner);
			testPowerLabel.setLabelFor(this.testPowerSpinner);

			GridPane gridPane = DialogUtil.createGridPane();

			probabilityValueLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			testPowerLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			probabilityValueLabel.setMaxWidth(double.MaxValue);
			testPowerLabel.setMaxWidth(double.MaxValue);

			GridPane.setHgrow(probabilityValueLabel, Priority.NEVER);
			GridPane.setHgrow(testPowerLabel, Priority.NEVER);

			GridPane.setHgrow(this.testStatisticTypeComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.familywiseErrorRateCheckBox, Priority.ALWAYS);

			GridPane.setHgrow(this.probabilityValueSpinner, Priority.ALWAYS);
			GridPane.setHgrow(this.testPowerSpinner, Priority.ALWAYS);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 2);
			Insets insetsRight = new Insets(5, 2, 5, 7);
			Insets insetsCenter = new Insets(5, 2, 5, 2);

			GridPane.setMargin(this.testStatisticTypeComboBox, insetsCenter);
			GridPane.setMargin(this.familywiseErrorRateCheckBox, insetsCenter);

			GridPane.setMargin(probabilityValueLabel, insetsLeft);
			GridPane.setMargin(testPowerLabel, insetsLeft);

			GridPane.setMargin(this.probabilityValueSpinner, insetsRight);
			GridPane.setMargin(this.testPowerSpinner, insetsRight);

			int row = 0;
			gridPane.add(this.testStatisticTypeComboBox, 0, ++row, 2, 1);
			gridPane.add(this.familywiseErrorRateCheckBox, 0, ++row, 2, 1);

			gridPane.add(probabilityValueLabel, 0, ++row);
			gridPane.add(this.probabilityValueSpinner, 1, row);

			gridPane.add(testPowerLabel, 0, ++row);
			gridPane.add(this.testPowerSpinner, 1, row);

			Platform.runLater(() =>
			{
			testStatisticTypeComboBox.requestFocus();
			});

			gridPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			gridPane.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // width, height

			return gridPane;
		}

		internal static StringConverter<TestStatisticType> createTestStatisticTypeStringConverter()
		{
			return new StringConverterAnonymousInnerClass();
		}

		private class StringConverterAnonymousInnerClass : StringConverter<TestStatisticType>
		{
			public override string toString(TestStatisticType type)
			{
				if (type == null)
				{
					return null;
				}
				switch (type.innerEnumValue)
				{
				case TestStatisticType.InnerEnum.BAARDA_METHOD:
					return i18N.getString("TestStatisticDialog.type.baarda", "Baardas B-method");
				case TestStatisticType.InnerEnum.SIDAK:
					return i18N.getString("TestStatisticDialog.type.sidak", "\u0160id\u00E1k correction");
				case TestStatisticType.InnerEnum.NONE:
					return i18N.getString("TestStatisticDialog.type.none", "None");
				}
				return "";
			}

			public override TestStatisticType fromString(string @string)
			{
				return TestStatisticType.valueOf(@string);
			}
		}

		public virtual void formatterChanged(FormatterEvent evt)
		{
			if (evt != null && evt.CellType == CellValueType.PERCENTAGE && evt.EventType == FormatterEventType.UNIT_CHANGED)
			{
				string frmPercentUnit = this.options.FormatterOptions[CellValueType.PERCENTAGE].getUnit().toFormattedAbbreviation();
				string labelProbabilityValue = String.format(Locale.ENGLISH, "%s%s:", i18N.getString("TestStatisticDialog.probability.label", "Probability value \u03B1"), frmPercentUnit.Trim().Length == 0 ? "" : " " + frmPercentUnit);
				string labelTestPower = String.format(Locale.ENGLISH, "%s%s:", i18N.getString("TestStatisticDialog.testpower.label", "Power of test 1 - \u03B2"), frmPercentUnit.Trim().Length == 0 ? "" : " " + frmPercentUnit);


				this.probabilityValueLabel.setText(labelProbabilityValue);
				this.testPowerLabel.setText(labelTestPower);
			}
		}
	}

}