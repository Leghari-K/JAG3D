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

namespace org.applied_geodesy.jag3d.ui.dialog
{

	using TestStatisticDefinition = org.applied_geodesy.adjustment.statistic.TestStatisticDefinition;
	using TestStatisticType = org.applied_geodesy.adjustment.statistic.TestStatisticType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DoubleSpinner = org.applied_geodesy.ui.spinner.DoubleSpinner;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterEventType = org.applied_geodesy.util.FormatterEventType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using CheckBox = javafx.scene.control.CheckBox;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using SpinnerValueFactory = javafx.scene.control.SpinnerValueFactory;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class TestStatisticDialog : FormatterChangedListener
	{
		private I18N i18n = I18N.Instance;
		private static TestStatisticDialog testStatisticDialog = new TestStatisticDialog();
		private FormatterOptions options = FormatterOptions.Instance;
		private Dialog<TestStatisticDefinition> dialog = null;
		private DoubleSpinner probabilityValueSpinner, testPowerSpinner;
		private ComboBox<TestStatisticType> testStatisticTypeComboBox;
		private CheckBox familywiseErrorRateCheckBox;
		private Window window;
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

		public static Optional<TestStatisticDefinition> showAndWait()
		{
			testStatisticDialog.init();
			testStatisticDialog.load();
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

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<TestStatisticDefinition>();
			this.dialog.setTitle(i18n.getString("TestStatisticDialog.title", "Test statistic"));
			this.dialog.setHeaderText(i18n.getString("TestStatisticDialog.header", "Test statistic properties"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			//		this.dialog.initStyle(StageStyle.UTILITY);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
			// add formatter listener
			options.addFormatterChangedListener(this);
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
					TestStatisticDefinition testStatistic = new TestStatisticDefinition(testStatisticType, probabilityValue, powerOfTest, familywiseErrorRate);
					outerInstance.save(testStatistic);
					return testStatistic;
				}
				return null;
			}
		}

		private Node createPane()
		{
			string frmPercentUnit = this.options.FormatterOptions[CellValueType.PERCENTAGE].getUnit().toFormattedAbbreviation();
			string labelProbabilityValue = String.format(Locale.ENGLISH, "%s%s:", i18n.getString("TestStatisticDialog.probability.label", "Probability value \u03B1"), frmPercentUnit.Trim().Length == 0 ? "" : " " + frmPercentUnit);
			string labelTestPower = String.format(Locale.ENGLISH, "%s%s:", i18n.getString("TestStatisticDialog.testpower.label", "Power of test 1 - \u03B2"), frmPercentUnit.Trim().Length == 0 ? "" : " " + frmPercentUnit);

			string tooltipProbabilityValue = i18n.getString("TestStatisticDialog.probability.tooltip", "Set probability value (type I error)");
			string tooltipTestPower = i18n.getString("TestStatisticDialog.testpower.tooltip", "Set power of test (type II error)");

			string labelFamilywiseErrorRate = i18n.getString("TestStatisticDialog.familywiseerror.label", "Familywise error rate");
			string tooltipFamilywiseErrorRate = i18n.getString("TestStatisticDialog.familywiseerror.tooltip", "If checked, probability value \u03B1 defines familywise error rate");

			this.probabilityValueLabel = new Label(labelProbabilityValue);
			this.testPowerLabel = new Label(labelTestPower);

			this.familywiseErrorRateCheckBox = this.createCheckBox(labelFamilywiseErrorRate, tooltipFamilywiseErrorRate);

			this.probabilityValueSpinner = this.createDoubleSpinner(0.0005, 0.30, 0.01, tooltipProbabilityValue);
			this.testPowerSpinner = this.createDoubleSpinner(0.50, 0.9995, 0.01, tooltipTestPower);

			this.testStatisticTypeComboBox = this.createTestStatisticTypeComboBox();

			GridPane gridPane = new GridPane();
			gridPane.setMaxWidth(double.MaxValue);
			gridPane.setMinWidth(300);
			gridPane.setHgap(20);
			gridPane.setVgap(10);
			gridPane.setAlignment(Pos.CENTER);
			gridPane.setPadding(new Insets(5,15,5,15)); // oben, recht, unten, links
			//gridPane.setGridLinesVisible(true);

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

			return gridPane;
		}

		private ComboBox<TestStatisticType> createTestStatisticTypeComboBox()
		{
			ComboBox<TestStatisticType> typeComboBox = new ComboBox<TestStatisticType>();

			typeComboBox.getItems().setAll(TestStatisticType.values());
			typeComboBox.setConverter(new StringConverterAnonymousInnerClass(this));
			typeComboBox.setTooltip(new Tooltip(i18n.getString("TestStatisticDialog.type.tooltip", "Select method for type I error adaption")));
			typeComboBox.getSelectionModel().select(TestStatisticType.NONE);
			typeComboBox.setMinWidth(150);
			typeComboBox.setPrefWidth(200);
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		private class StringConverterAnonymousInnerClass : StringConverter<TestStatisticType>
		{
			private readonly TestStatisticDialog outerInstance;

			public StringConverterAnonymousInnerClass(TestStatisticDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override string toString(TestStatisticType type)
			{
				if (type == null)
				{
					return null;
				}
				switch (type.innerEnumValue)
				{
				case TestStatisticType.InnerEnum.BAARDA_METHOD:
					return outerInstance.i18n.getString("TestStatisticDialog.type.baarda", "Baardas B-method");
				case TestStatisticType.InnerEnum.SIDAK:
					return outerInstance.i18n.getString("TestStatisticDialog.type.sidak", "\u0160id\u00E1k correction");
				case TestStatisticType.InnerEnum.NONE:
					return outerInstance.i18n.getString("TestStatisticDialog.type.none", "None");
				}
				return "";
			}

			public override TestStatisticType fromString(string @string)
			{
				return TestStatisticType.valueOf(@string);
			}
		}

		private DoubleSpinner createDoubleSpinner(double min, double max, double amountToStepBy, string tooltip)
		{
			DoubleSpinner doubleSpinner = new DoubleSpinner(CellValueType.PERCENTAGE, min, max, amountToStepBy);
			doubleSpinner.setMinWidth(75);
			doubleSpinner.setPrefWidth(100);
			doubleSpinner.setMaxWidth(double.MaxValue);
			doubleSpinner.setTooltip(new Tooltip(tooltip));
			return doubleSpinner;
		}

		private void save(TestStatisticDefinition testStatistic)
		{
			try
			{
				SQLManager.Instance.save(testStatistic);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("TestStatisticDialog.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("TestStatisticDialog.message.error.save.exception.header", "Error, could not save test statistics to database."), i18n.getString("TestStatisticDialog.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private void load()
		{
			try
			{
				TestStatisticDefinition testStatistic = SQLManager.Instance.TestStatisticDefinition;
				this.familywiseErrorRateCheckBox.setSelected(testStatistic.FamilywiseErrorRate);
				this.testStatisticTypeComboBox.getSelectionModel().select(testStatistic.TestStatisticType);

				SpinnerValueFactory.DoubleSpinnerValueFactory probabilityValueSpinnerFactory = (SpinnerValueFactory.DoubleSpinnerValueFactory)this.probabilityValueSpinner.getValueFactory();
				SpinnerValueFactory.DoubleSpinnerValueFactory testPowerSpinnerFactory = (SpinnerValueFactory.DoubleSpinnerValueFactory)this.testPowerSpinner.getValueFactory();

				double probabilityValue = testStatistic.ProbabilityValue;
				double powerOfTest = testStatistic.PowerOfTest;

				probabilityValue = Math.Max(Math.Min(this.options.convertPercentToView(probabilityValue), probabilityValueSpinnerFactory.getMax()), probabilityValueSpinnerFactory.getMin());
				powerOfTest = Math.Max(Math.Min(this.options.convertPercentToView(powerOfTest), testPowerSpinnerFactory.getMax()), testPowerSpinnerFactory.getMin());

				probabilityValueSpinnerFactory.setValue(probabilityValue);
				testPowerSpinnerFactory.setValue(powerOfTest);

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("TestStatisticDialog.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("TestStatisticDialog.message.error.load.exception.header", "Error, could not load test statistics from database."), i18n.getString("TestStatisticDialog.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
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

		public virtual void formatterChanged(FormatterEvent evt)
		{
			if (evt != null && evt.CellType == CellValueType.PERCENTAGE && evt.EventType == FormatterEventType.UNIT_CHANGED)
			{
				string frmPercentUnit = this.options.FormatterOptions[CellValueType.PERCENTAGE].getUnit().toFormattedAbbreviation();
				string labelProbabilityValue = String.format(Locale.ENGLISH, "%s%s:", i18n.getString("TestStatisticDialog.probability.label", "Probability value \u03B1"), frmPercentUnit.Trim().Length == 0 ? "" : " " + frmPercentUnit);
				string labelTestPower = String.format(Locale.ENGLISH, "%s%s:", i18n.getString("TestStatisticDialog.testpower.label", "Power of test 1 - \u03B2"), frmPercentUnit.Trim().Length == 0 ? "" : " " + frmPercentUnit);

				this.probabilityValueLabel.setText(labelProbabilityValue);
				this.testPowerLabel.setText(labelTestPower);
			}
		}
	}

}