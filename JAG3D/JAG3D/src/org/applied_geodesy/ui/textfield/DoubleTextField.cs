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

namespace org.applied_geodesy.ui.textfield
{

	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using TextField = javafx.scene.control.TextField;
	using TextFormatter = javafx.scene.control.TextFormatter;
	using Change = javafx.scene.control.TextFormatter.Change;

	public class DoubleTextField : TextField, FormatterChangedListener
	{
		public enum ValueSupport
		{
			INCLUDING_INCLUDING_INTERVAL,
			INCLUDING_EXCLUDING_INTERVAL,
			EXCLUDING_INCLUDING_INTERVAL,
			EXCLUDING_EXCLUDING_INTERVAL,

			NULL_VALUE_SUPPORT,
			NON_NULL_VALUE_SUPPORT
		}

		private FormatterOptions options = FormatterOptions.Instance;
		private const int EDITOR_ADDITIONAL_DIGITS = 10;
		private double lowerBoundary = double.NegativeInfinity, upperBoundary = double.PositiveInfinity;
		private NumberFormat editorNumberFormat;
		private CellValueType type;
		private readonly bool displayUnit;
		private readonly ValueSupport valueSupport;
		private ObjectProperty<double> number = new SimpleObjectProperty<double>();
		private bool typeChanged = false;
		public DoubleTextField(CellValueType type) : this(null, type, false)
		{
		}

		public DoubleTextField(CellValueType type, bool displayUnit) : this(null, type, displayUnit)
		{
		}

		public DoubleTextField(double? value, CellValueType type, bool displayUnit) : this(value, type, displayUnit, org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport.NULL_VALUE_SUPPORT)
		{
		}

		public DoubleTextField(double? value, CellValueType type, bool displayUnit, ValueSupport valueSupport) : this(value, type, displayUnit, valueSupport, double.NegativeInfinity, double.PositiveInfinity)
		{
		}

		public DoubleTextField(double? value, CellValueType type, bool displayUnit, ValueSupport valueSupport, double lowerBoundary, double upperBoundary) : base()
		{

			this.type = type;
			this.valueSupport = valueSupport;
			this.displayUnit = displayUnit;
			this.lowerBoundary = lowerBoundary;
			this.upperBoundary = upperBoundary;

			if (this.check(value))
			{
				this.Number = value;
			}

			this.prepareEditorNumberFormat();
			this.initHandlers();
			this.setTextFormatter(this.createTextFormatter());

			if (this.check(value))
			{
				this.setText(this.getRendererFormat(value));
			}

			this.options.addFormatterChangedListener(this);
		}

		public virtual CellValueType CellValueType
		{
			set
			{
				this.type = value;
				this.typeChanged = true;
    
				double value = this.Number.Value;
    
				if (this.check(value))
				{
					this.Number = value;
				}
    
				this.prepareEditorNumberFormat();
				this.setTextFormatter(this.createTextFormatter());
    
				if (this.check(value))
				{
					this.setText(this.getRendererFormat(value));
				}
			}
			get
			{
				return this.type;
			}
		}

		private void prepareEditorNumberFormat()
		{
			this.editorNumberFormat = (NumberFormat)this.options.FormatterOptions[this.type].getFormatter().clone();
			this.editorNumberFormat.setMinimumFractionDigits(this.editorNumberFormat.getMaximumFractionDigits());
			this.editorNumberFormat.setMaximumFractionDigits(this.editorNumberFormat.getMaximumFractionDigits() + EDITOR_ADDITIONAL_DIGITS);
		}

		public virtual bool check(double? value)
		{
			switch (this.valueSupport)
			{
			case org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport.INCLUDING_INCLUDING_INTERVAL:
				return value != null && this.lowerBoundary <= value.Value && value.Value <= this.upperBoundary;
			case org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport.EXCLUDING_EXCLUDING_INTERVAL:
				return value != null && this.lowerBoundary < value.Value && value.Value < this.upperBoundary;
			case org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport.INCLUDING_EXCLUDING_INTERVAL:
				return value != null && this.lowerBoundary <= value.Value && value.Value < this.upperBoundary;
			case org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport.EXCLUDING_INCLUDING_INTERVAL:
				return value != null && this.lowerBoundary < value.Value && value.Value <= this.upperBoundary;
			case org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT:
				return value != null;
			default: // NULL_VALUE_SUPPORT:
				return true;
			}
		}

		private TextFormatter<double> createTextFormatter()
		{
			//Pattern decimalPattern = Pattern.compile("^[-|+]?\\d*?\\D*\\d{0,"+this.editorNumberFormat.getMaximumFractionDigits()+"}\\s*\\D*$");
			Pattern decimalPattern = Pattern.compile("^[+|-]?[\\d\\D]*?\\d{0," + this.editorNumberFormat.getMaximumFractionDigits() + "}\\s*\\D*$");
			System.Func<TextFormatter.Change, TextFormatter.Change> filter = (TextFormatter.Change change) =>
			{

			if (!change.isContentChange())
			{
				return change;
			}

			try
			{
				string input = change.getControlNewText();
				if (string.ReferenceEquals(input, null) || input.Trim().Length == 0 || decimalPattern.matcher(input.Trim()).matches())
				{
					return change;
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return null;
			};

			return new TextFormatter<double>(filter);
		}

		private void initHandlers()
		{

			this.setOnAction(new EventHandlerAnonymousInnerClass(this));

			this.focusedProperty().addListener(new ChangeListenerAnonymousInnerClass(this));

			numberProperty().addListener(new ChangeListenerAnonymousInnerClass2(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<ActionEvent>
		{
			private readonly DoubleTextField outerInstance;

			public EventHandlerAnonymousInnerClass(DoubleTextField outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				outerInstance.parseAndFormatInput();
			}
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<bool>
		{
			private readonly DoubleTextField outerInstance;

			public ChangeListenerAnonymousInnerClass(DoubleTextField outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (outerInstance.typeChanged)
				{
					outerInstance.typeChanged = false;
					setText(outerInstance.getEditorFormat(outerInstance.Number));
				}
				if (!newValue.Value)
				{
					outerInstance.parseAndFormatInput();
					setText(outerInstance.getRendererFormat(outerInstance.Number));
				}
				else
				{
					setText(outerInstance.getEditorFormat(outerInstance.Number));
				}
			}
		}

		private class ChangeListenerAnonymousInnerClass2 : ChangeListener<double>
		{
			private readonly DoubleTextField outerInstance;

			public ChangeListenerAnonymousInnerClass2(DoubleTextField outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> obserable, double? oldValue, double? newValue) where T1 : double
			{
				setText(outerInstance.getEditorFormat(outerInstance.Number));
				//setText(getRendererFormat(newValue));
			}
		}

		private string getEditorFormat(double? value)
		{
			if (!this.check(value))
			{
				return null;
			}

			value = this.Number;
			if (value == null)
			{
				return null;
			}

			switch (this.type.innerEnumValue)
			{
			case CellValueType.InnerEnum.ANGLE:
				return this.editorNumberFormat.format(this.options.convertAngleToView(value.Value));

			case CellValueType.InnerEnum.ANGLE_RESIDUAL:
				return this.editorNumberFormat.format(this.options.convertAngleResidualToView(value.Value));

			case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
				return this.editorNumberFormat.format(this.options.convertAngleUncertaintyToView(value.Value));

			case CellValueType.InnerEnum.LENGTH:
				return this.editorNumberFormat.format(this.options.convertLengthToView(value.Value));

			case CellValueType.InnerEnum.LENGTH_RESIDUAL:
				return this.editorNumberFormat.format(this.options.convertLengthResidualToView(value.Value));

			case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
				return this.editorNumberFormat.format(this.options.convertLengthUncertaintyToView(value.Value));

			case CellValueType.InnerEnum.SCALE:
				return this.editorNumberFormat.format(this.options.convertScaleToView(value.Value));

			case CellValueType.InnerEnum.SCALE_RESIDUAL:
				return this.editorNumberFormat.format(this.options.convertScaleResidualToView(value.Value));

			case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
				return this.editorNumberFormat.format(this.options.convertScaleUncertaintyToView(value.Value));

			case CellValueType.InnerEnum.STATISTIC:
			case CellValueType.InnerEnum.DOUBLE:
				return this.editorNumberFormat.format(value.Value);

			case CellValueType.InnerEnum.VECTOR:
				return this.editorNumberFormat.format(this.options.convertVectorToView(value.Value));

			case CellValueType.InnerEnum.VECTOR_RESIDUAL:
				return this.editorNumberFormat.format(this.options.convertVectorResidualToView(value.Value));

			case CellValueType.InnerEnum.VECTOR_UNCERTAINTY:
				return this.editorNumberFormat.format(this.options.convertVectorUncertaintyToView(value.Value));

			case CellValueType.InnerEnum.TEMPERATURE:
				return this.editorNumberFormat.format(this.options.convertTemperatureToView(value.Value));

			case CellValueType.InnerEnum.PRESSURE:
				return this.editorNumberFormat.format(this.options.convertPressureToView(value.Value));

			case CellValueType.InnerEnum.PERCENTAGE:
				return this.editorNumberFormat.format(this.options.convertPercentToView(value.Value));

			default:
				return this.editorNumberFormat.format(value.Value);
			}
		}

		internal virtual string getRendererFormat(double? value)
		{
			if (!this.check(value))
			{
				return null;
			}

			value = this.Number;
			if (value == null)
			{
				return null;
			}

			switch (this.type.innerEnumValue)
			{
			case CellValueType.InnerEnum.ANGLE:
				return this.options.toAngleFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.ANGLE_RESIDUAL:
				return this.options.toAngleResidualFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
				return this.options.toAngleUncertaintyFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.LENGTH:
				return this.options.toLengthFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.LENGTH_RESIDUAL:
				return this.options.toLengthResidualFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
				return this.options.toLengthUncertaintyFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.SCALE:
				return this.options.toScaleFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.SCALE_RESIDUAL:
				return this.options.toScaleResidualFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
				return this.options.toScaleUncertaintyFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.STATISTIC:
				return this.options.toStatisticFormat(value.Value);

			case CellValueType.InnerEnum.VECTOR:
				return this.options.toVectorFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.VECTOR_RESIDUAL:
				return this.options.toVectorResidualFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.VECTOR_UNCERTAINTY:
				return this.options.toVectorUncertaintyFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.TEMPERATURE:
				return this.options.toTemperatureFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.PRESSURE:
				return this.options.toPressureFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.PERCENTAGE:
				return this.options.toPercentFormat(value.Value, this.displayUnit);

			case CellValueType.InnerEnum.DOUBLE:
				return this.options.toDoubleFormat(value.Value);

			default:
				return null;
			}
		}

		/// <summary>
		/// Tries to parse the user input to a number according to the provided
		/// NumberFormat
		/// </summary>
		private void parseAndFormatInput()
		{
			try
			{
				string input = this.getText();
				if (!string.ReferenceEquals(input, null) && input.Trim().Length > 0)
				{
					input = input.replaceAll(",", ".");
					ParsePosition parsePosition = new ParsePosition(0);
					double? newValue = this.options.FormatterOptions[this.type].parse(input, parsePosition).doubleValue();
					// check if value is not null and if the complete value is error-free parsed 
					// https://www.ibm.com/developerworks/library/j-numberformat/index.html
					// https://stackoverflow.com/questions/14194888/validating-decimal-numbers-in-a-locale-sensitive-way-in-java
					if (newValue != null && parsePosition.getErrorIndex() < 0 && parsePosition.getIndex() == input.Length)
					{
						switch (this.type.innerEnumValue)
						{
						case CellValueType.InnerEnum.ANGLE:
							newValue = this.options.convertAngleToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.ANGLE_RESIDUAL:
							newValue = this.options.convertAngleResidualToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
							newValue = this.options.convertAngleUncertaintyToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.LENGTH:
							newValue = this.options.convertLengthToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.LENGTH_RESIDUAL:
							newValue = this.options.convertLengthResidualToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
							newValue = this.options.convertLengthUncertaintyToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.SCALE:
							newValue = this.options.convertScaleToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.SCALE_RESIDUAL:
							newValue = this.options.convertScaleResidualToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
							newValue = this.options.convertScaleUncertaintyToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.DOUBLE:
						case CellValueType.InnerEnum.STATISTIC:
							newValue = newValue.Value;
							break;
						case CellValueType.InnerEnum.VECTOR:
							newValue = this.options.convertVectorToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.VECTOR_RESIDUAL:
							newValue = this.options.convertVectorResidualToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.VECTOR_UNCERTAINTY:
							newValue = this.options.convertVectorUncertaintyToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.TEMPERATURE:
							newValue = this.options.convertTemperatureToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.PRESSURE:
							newValue = this.options.convertPressureToModel(newValue.Value);
							break;
						case CellValueType.InnerEnum.PERCENTAGE:
							newValue = this.options.convertPercentToModel(newValue.Value);
							break;
						default:
							newValue = newValue.Value;
							break;
						}
						// set new value, if valid
						this.Number = !this.check(newValue) ? this.Number : newValue;
					}
				}
				else if ((string.ReferenceEquals(input, null) || input.Trim().Length == 0) && this.check(null))
				{
					this.Number = null;
				}
				this.selectAll();

			}
			catch (Exception)
			{
				this.setText(this.getRendererFormat(this.Number));
			}
		}

		public double? Number
		{
			get
			{
				return this.number.get();
			}
			set
			{
				this.number.set(value);
			}
		}


		public double? Value
		{
			set
			{
				this.number.set(value);
				this.setText(this.getRendererFormat(value));
			}
		}

		public virtual ObjectProperty<double> numberProperty()
		{
			return this.number;
		}


		public virtual bool DisplayUnit
		{
			get
			{
				return this.displayUnit;
			}
		}

		public virtual ValueSupport ValueSupport
		{
			get
			{
				return this.valueSupport;
			}
		}

		public virtual void formatterChanged(FormatterEvent evt)
		{
			this.prepareEditorNumberFormat();
			this.setText(this.getRendererFormat(this.Number));
		}
	}
}