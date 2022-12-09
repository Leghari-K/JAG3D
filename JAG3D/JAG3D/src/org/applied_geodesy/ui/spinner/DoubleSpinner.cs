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

namespace org.applied_geodesy.ui.spinner
{

	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Pos = javafx.geometry.Pos;
	using Spinner = javafx.scene.control.Spinner;
	using SpinnerValueFactory = javafx.scene.control.SpinnerValueFactory;
	using TextFormatter = javafx.scene.control.TextFormatter;
	using StringConverter = javafx.util.StringConverter;

	public class DoubleSpinner : Spinner<double>, FormatterChangedListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			doubleFactory = (SpinnerValueFactory.DoubleSpinnerValueFactory)this.getValueFactory();
		}

		private FormatterOptions options = FormatterOptions.Instance;
		private readonly CellValueType cellValueType;
		private readonly double min, max, amountToStepBy;
		private double? number;
		private NumberFormat editorNumberFormat;
		private const int EDITOR_ADDITIONAL_DIGITS = 10;
		private SpinnerValueFactory.DoubleSpinnerValueFactory doubleFactory;

		public DoubleSpinner(CellValueType cellValueType, double min, double max, double amountToStepBy) : this(cellValueType, min, max, min, amountToStepBy)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public DoubleSpinner(CellValueType cellValueType, double min, double max, double initialValue, double amountToStepBy)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.options.addFormatterChangedListener(this);
			this.min = min;
			this.max = max;
			this.number = initialValue;
			this.amountToStepBy = amountToStepBy;
			this.cellValueType = cellValueType;
			this.init();
		}

		private void prepareEditorNumberFormat()
		{
			this.editorNumberFormat = (NumberFormat)this.options.FormatterOptions[this.cellValueType].getFormatter().clone();
			this.editorNumberFormat.setMinimumFractionDigits(this.editorNumberFormat.getMaximumFractionDigits());
			this.editorNumberFormat.setMaximumFractionDigits(this.editorNumberFormat.getMaximumFractionDigits() + EDITOR_ADDITIONAL_DIGITS);
		}

		private TextFormatter<double> createTextFormatter()
		{
			StringConverter<double> converter = new StringConverterAnonymousInnerClass(this);
			return new TextFormatter<double>(converter, this.doubleFactory.getValue());
		}

		private class StringConverterAnonymousInnerClass : StringConverter<double>
		{
			private readonly DoubleSpinner outerInstance;

			public StringConverterAnonymousInnerClass(DoubleSpinner outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override double? fromString(string s)
			{
				if (string.ReferenceEquals(s, null) || s.Trim().Length == 0)
				{
					return null;
				}
				else
				{
					try
					{
						return outerInstance.editorNumberFormat.parse(s.replaceAll(",", ".")).doubleValue();
					}
					catch (Exception nfe)
					{
						Console.WriteLine(nfe.ToString());
						Console.Write(nfe.StackTrace);
					}
				}
				return null;
			}

			public override string toString(double? d)
			{
				return d == null ? "" : outerInstance.editorNumberFormat.format(d);
			}
		}

		private void init()
		{
			this.initDoubleValueFactory();
			this.prepareEditorNumberFormat();

			TextFormatter<double> formatter = this.createTextFormatter();
			this.doubleFactory.setConverter(formatter.getValueConverter());

			this.setEditable(true);
			this.setValueFactory(this.doubleFactory);

			this.getEditor().setTextFormatter(formatter);
			this.getEditor().setAlignment(Pos.BOTTOM_RIGHT);

	//		this.doubleFactory.valueProperty().bindBidirectional(formatter.valueProperty());
			this.doubleFactory.valueProperty().addListener(new ChangeListenerAnonymousInnerClass(this));
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<double>
		{
			private readonly DoubleSpinner outerInstance;

			public ChangeListenerAnonymousInnerClass(DoubleSpinner outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (newValue == null)
				{
					outerInstance.doubleFactory.setValue(oldValue);
					return;
				}

				switch (outerInstance.cellValueType.innerEnumValue)
				{
				case CellValueType.InnerEnum.ANGLE:
					outerInstance.number = outerInstance.options.convertAngleToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.ANGLE_RESIDUAL:
					outerInstance.number = outerInstance.options.convertAngleResidualToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
					outerInstance.number = outerInstance.options.convertAngleUncertaintyToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.LENGTH:
					outerInstance.number = outerInstance.options.convertLengthToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.LENGTH_RESIDUAL:
					outerInstance.number = outerInstance.options.convertLengthResidualToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
					outerInstance.number = outerInstance.options.convertLengthUncertaintyToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.SCALE:
					outerInstance.number = outerInstance.options.convertScaleToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.SCALE_RESIDUAL:
					outerInstance.number = outerInstance.options.convertScaleResidualToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
					outerInstance.number = outerInstance.options.convertScaleUncertaintyToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.VECTOR:
					outerInstance.number = outerInstance.options.convertVectorToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.VECTOR_RESIDUAL:
					outerInstance.number = outerInstance.options.convertVectorResidualToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.VECTOR_UNCERTAINTY:
					outerInstance.number = outerInstance.options.convertVectorUncertaintyToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.TEMPERATURE:
					outerInstance.number = outerInstance.options.convertTemperatureToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.PRESSURE:
					outerInstance.number = outerInstance.options.convertPressureToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.PERCENTAGE:
					outerInstance.number = outerInstance.options.convertPercentToModel(newValue.Value);
					break;
				case CellValueType.InnerEnum.STATISTIC:
				case CellValueType.InnerEnum.DOUBLE:
					outerInstance.number = newValue;
					break;
				default:
					outerInstance.number = newValue;
					break;

				}
				//System.out.println("CHANGED " + oldValue+"   "+newValue+"   STORED " + number+"    "+doubleFactory.getValue());
			}
		}

		private void initDoubleValueFactory()
		{
			double vmin, vmax, vamountToStepBy, vvalue;

			switch (this.cellValueType.innerEnumValue)
			{
			case CellValueType.InnerEnum.ANGLE:
				vmin = this.options.convertAngleToView(this.min);
				vmax = this.options.convertAngleToView(this.max);
				vvalue = this.options.convertAngleToView(this.number.Value);
				vamountToStepBy = this.options.convertAngleToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.ANGLE_RESIDUAL:
				vmin = this.options.convertAngleResidualToView(this.min);
				vmax = this.options.convertAngleResidualToView(this.max);
				vvalue = this.options.convertAngleResidualToView(this.number.Value);
				vamountToStepBy = this.options.convertAngleResidualToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
				vmin = this.options.convertAngleUncertaintyToView(this.min);
				vmax = this.options.convertAngleUncertaintyToView(this.max);
				vvalue = this.options.convertAngleUncertaintyToView(this.number.Value);
				vamountToStepBy = this.options.convertAngleUncertaintyToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.LENGTH:
				vmin = this.options.convertLengthToView(this.min);
				vmax = this.options.convertLengthToView(this.max);
				vvalue = this.options.convertLengthToView(this.number.Value);
				vamountToStepBy = this.options.convertLengthToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.LENGTH_RESIDUAL:
				vmin = this.options.convertLengthResidualToView(this.min);
				vmax = this.options.convertLengthResidualToView(this.max);
				vvalue = this.options.convertLengthResidualToView(this.number.Value);
				vamountToStepBy = this.options.convertLengthResidualToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
				vmin = this.options.convertLengthUncertaintyToView(this.min);
				vmax = this.options.convertLengthUncertaintyToView(this.max);
				vvalue = this.options.convertLengthUncertaintyToView(this.number.Value);
				vamountToStepBy = this.options.convertLengthUncertaintyToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.SCALE:
				vmin = this.options.convertScaleToView(this.min);
				vmax = this.options.convertScaleToView(this.max);
				vvalue = this.options.convertScaleToView(this.number.Value);
				vamountToStepBy = this.options.convertScaleToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.SCALE_RESIDUAL:
				vmin = this.options.convertScaleResidualToView(this.min);
				vmax = this.options.convertScaleResidualToView(this.max);
				vvalue = this.options.convertScaleResidualToView(this.number.Value);
				vamountToStepBy = this.options.convertScaleResidualToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
				vmin = this.options.convertScaleUncertaintyToView(this.min);
				vmax = this.options.convertScaleUncertaintyToView(this.max);
				vvalue = this.options.convertScaleUncertaintyToView(this.number.Value);
				vamountToStepBy = this.options.convertScaleUncertaintyToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.VECTOR:
				vmin = this.options.convertVectorToView(this.min);
				vmax = this.options.convertVectorToView(this.max);
				vvalue = this.options.convertVectorToView(this.number.Value);
				vamountToStepBy = this.options.convertVectorToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.VECTOR_RESIDUAL:
				vmin = this.options.convertVectorResidualToView(this.min);
				vmax = this.options.convertVectorResidualToView(this.max);
				vvalue = this.options.convertVectorResidualToView(this.number.Value);
				vamountToStepBy = this.options.convertVectorResidualToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.VECTOR_UNCERTAINTY:
				vmin = this.options.convertVectorUncertaintyToView(this.min);
				vmax = this.options.convertVectorUncertaintyToView(this.max);
				vvalue = this.options.convertVectorUncertaintyToView(this.number.Value);
				vamountToStepBy = this.options.convertVectorUncertaintyToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.TEMPERATURE:
				vmin = this.options.convertTemperatureToView(this.min);
				vmax = this.options.convertTemperatureToView(this.max);
				vvalue = this.options.convertTemperatureToView(this.number.Value);
				vamountToStepBy = this.options.convertTemperatureToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.PRESSURE:
				vmin = this.options.convertPressureToView(this.min);
				vmax = this.options.convertPressureToView(this.max);
				vvalue = this.options.convertPressureToView(this.number.Value);
				vamountToStepBy = this.options.convertPressureToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.PERCENTAGE:
				vmin = this.options.convertPercentToView(this.min);
				vmax = this.options.convertPercentToView(this.max);
				vvalue = this.options.convertPercentToView(this.number.Value);
				vamountToStepBy = this.options.convertPercentToView(this.amountToStepBy);
				break;
			case CellValueType.InnerEnum.STATISTIC:
			case CellValueType.InnerEnum.DOUBLE:
				vmin = this.min;
				vmax = this.max;
				vvalue = this.number.Value;
				vamountToStepBy = this.amountToStepBy;
				break;
			default:
				vmin = this.min;
				vmax = this.max;
				vvalue = this.number.Value;
				vamountToStepBy = this.amountToStepBy;
				break;
			}

			if (this.doubleFactory == null)
			{
				this.doubleFactory = new SpinnerValueFactory.DoubleSpinnerValueFactory(vmin, vmax, vvalue, vamountToStepBy);
			}
			else
			{
				this.doubleFactory.setMin(vmin);
				this.doubleFactory.setMax(vmax);
				this.doubleFactory.setAmountToStepBy(vamountToStepBy);
				this.doubleFactory.setValue(vvalue);
			}
		}

		public virtual Number Number
		{
			get
			{
				return number;
			}
		}

		public virtual void formatterChanged(FormatterEvent evt)
		{
			this.initDoubleValueFactory();
			this.prepareEditorNumberFormat();
			TextFormatter<double> formatter = this.createTextFormatter();
			this.getEditor().setTextFormatter(formatter);
			this.doubleFactory.setConverter(formatter.getValueConverter());
		}
	}

}