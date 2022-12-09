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

namespace org.applied_geodesy.ui.table
{

	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	public class EditableDoubleCellConverter : EditableCellConverter<double>
	{
		private CellValueType cellValueType;
		private readonly FormatterOptions options = FormatterOptions.Instance;
		private readonly NumberFormat editorNumberFormat = NumberFormat.getInstance(Locale.ENGLISH);
		private bool displayUnit = false;
		private const int EXTRA_DIGITS_ON_EDIT = 7;

		public EditableDoubleCellConverter(CellValueType cellValueType) : this(cellValueType, false)
		{
		}

		public EditableDoubleCellConverter(CellValueType cellValueType, bool displayUnit)
		{
			this.cellValueType = cellValueType;
			this.displayUnit = displayUnit;

			int fracDigits = options.FormatterOptions[this.cellValueType].getFractionDigits();
			this.editorNumberFormat.setGroupingUsed(false);
			this.editorNumberFormat.setMaximumFractionDigits(fracDigits + EXTRA_DIGITS_ON_EDIT);
			this.editorNumberFormat.setMinimumFractionDigits(fracDigits);
		}

		public virtual CellValueType CellValueType
		{
			set
			{
				this.cellValueType = value;
				int fracDigits = options.FormatterOptions[this.cellValueType].getFractionDigits();
				this.editorNumberFormat.setGroupingUsed(false);
				this.editorNumberFormat.setMaximumFractionDigits(fracDigits + EXTRA_DIGITS_ON_EDIT);
				this.editorNumberFormat.setMinimumFractionDigits(fracDigits);
			}
		}

		public override string toEditorString(double? value)
		{
			try
			{
				if (value == null)
				{
					return "";
				}

				int fracDigits = options.FormatterOptions[this.cellValueType].getFractionDigits();
				if (this.editorNumberFormat.getMinimumFractionDigits() != fracDigits)
				{
					this.editorNumberFormat.setMaximumFractionDigits(fracDigits + EXTRA_DIGITS_ON_EDIT);
					this.editorNumberFormat.setMinimumFractionDigits(fracDigits);
				}

				switch (this.cellValueType.innerEnumValue)
				{
				case CellValueType.InnerEnum.ANGLE:
					return this.editorNumberFormat.format(options.convertAngleToView(value.Value));
				case CellValueType.InnerEnum.ANGLE_RESIDUAL:
					return this.editorNumberFormat.format(options.convertAngleResidualToView(value.Value));
				case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
					return this.editorNumberFormat.format(options.convertAngleUncertaintyToView(value.Value));
				case CellValueType.InnerEnum.LENGTH:
					return this.editorNumberFormat.format(options.convertLengthToView(value.Value));
				case CellValueType.InnerEnum.LENGTH_RESIDUAL:
					return this.editorNumberFormat.format(options.convertLengthResidualToView(value.Value));
				case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
					return this.editorNumberFormat.format(options.convertLengthUncertaintyToView(value.Value));
				case CellValueType.InnerEnum.SCALE:
					return this.editorNumberFormat.format(options.convertScaleToView(value.Value));
				case CellValueType.InnerEnum.SCALE_RESIDUAL:
					return this.editorNumberFormat.format(options.convertScaleResidualToView(value.Value));
				case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
					return this.editorNumberFormat.format(options.convertScaleUncertaintyToView(value.Value));
				case CellValueType.InnerEnum.STATISTIC:
				case CellValueType.InnerEnum.DOUBLE:
					return this.editorNumberFormat.format(value.Value);
				case CellValueType.InnerEnum.VECTOR:
					return this.editorNumberFormat.format(options.convertVectorToView(value.Value));
				case CellValueType.InnerEnum.VECTOR_RESIDUAL:
					return this.editorNumberFormat.format(options.convertVectorResidualToView(value.Value));
				case CellValueType.InnerEnum.VECTOR_UNCERTAINTY:
					return this.editorNumberFormat.format(options.convertVectorUncertaintyToView(value.Value));
				case CellValueType.InnerEnum.TEMPERATURE:
					return this.editorNumberFormat.format(options.convertTemperatureToView(value.Value));
				case CellValueType.InnerEnum.PRESSURE:
					return this.editorNumberFormat.format(options.convertPressureToView(value.Value));
				case CellValueType.InnerEnum.PERCENTAGE:
					return this.editorNumberFormat.format(options.convertPercentToView(value.Value));
				default:
					Console.Error.WriteLine(this.GetType().Name + " : Unsupported cell value type " + this.cellValueType);
					return value.ToString();
				}
			}
			catch (System.ArgumentException iae)
			{
				Console.WriteLine(iae.ToString());
				Console.Write(iae.StackTrace);
			}
			return "";
		}

		public override string toString(double? value)
		{
			if (value == null)
			{
				return "";
			}

			switch (this.cellValueType.innerEnumValue)
			{
			case CellValueType.InnerEnum.ANGLE:
				return options.toAngleFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.ANGLE_RESIDUAL:
				return options.toAngleResidualFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
				return options.toAngleUncertaintyFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.LENGTH:
				return options.toLengthFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.LENGTH_RESIDUAL:
				return options.toLengthResidualFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
				return options.toLengthUncertaintyFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.SCALE:
				return options.toScaleFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.SCALE_RESIDUAL:
				return options.toScaleResidualFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
				return options.toScaleUncertaintyFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.STATISTIC:
				return options.toStatisticFormat(value.Value);
			case CellValueType.InnerEnum.DOUBLE:
				return options.toDoubleFormat(value.Value);
			case CellValueType.InnerEnum.VECTOR:
				return options.toVectorFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.VECTOR_RESIDUAL:
				return options.toVectorResidualFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.VECTOR_UNCERTAINTY:
				return options.toVectorUncertaintyFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.TEMPERATURE:
				return options.toTemperatureFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.PRESSURE:
				return options.toPressureFormat(value.Value, displayUnit);
			case CellValueType.InnerEnum.PERCENTAGE:
				return options.toPercentFormat(value.Value, displayUnit);
			default:
				Console.Error.WriteLine(this.GetType().Name + " : Unsupported cell value type " + this.cellValueType);
				return value.ToString();
			}
		}

		public override double? fromString(string @string)
		{
			if (!string.ReferenceEquals(@string, null) && @string.Trim().Length > 0)
			{
				try
				{
					@string = @string.replaceAll(",", ".");
					double value = options.FormatterOptions[this.cellValueType].parse(@string.Trim()).doubleValue();
					switch (this.cellValueType.innerEnumValue)
					{
					case CellValueType.InnerEnum.ANGLE:
						return options.convertAngleToModel(value);
					case CellValueType.InnerEnum.ANGLE_RESIDUAL:
						return options.convertAngleResidualToModel(value);
					case CellValueType.InnerEnum.ANGLE_UNCERTAINTY:
						return options.convertAngleUncertaintyToModel(value);
					case CellValueType.InnerEnum.LENGTH:
						return options.convertLengthToModel(value);
					case CellValueType.InnerEnum.LENGTH_RESIDUAL:
						return options.convertLengthResidualToModel(value);
					case CellValueType.InnerEnum.LENGTH_UNCERTAINTY:
						return options.convertLengthUncertaintyToModel(value);
					case CellValueType.InnerEnum.SCALE:
						return options.convertScaleToModel(value);
					case CellValueType.InnerEnum.SCALE_RESIDUAL:
						return options.convertScaleResidualToModel(value);
					case CellValueType.InnerEnum.SCALE_UNCERTAINTY:
						return options.convertScaleUncertaintyToModel(value);
					case CellValueType.InnerEnum.STATISTIC:
					case CellValueType.InnerEnum.DOUBLE:
						return value;
					case CellValueType.InnerEnum.VECTOR:
						return options.convertVectorToModel(value);
					case CellValueType.InnerEnum.VECTOR_RESIDUAL:
						return options.convertVectorResidualToModel(value);
					case CellValueType.InnerEnum.VECTOR_UNCERTAINTY:
						return options.convertVectorUncertaintyToModel(value);
					case CellValueType.InnerEnum.TEMPERATURE:
						return options.convertTemperatureToModel(value);
					case CellValueType.InnerEnum.PRESSURE:
						return options.convertPressureToModel(value);
					case CellValueType.InnerEnum.PERCENTAGE:
						return options.convertPercentToModel(value);
					default:
						Console.Error.WriteLine(this.GetType().Name + " : Unsupported cell value type " + this.cellValueType);
						return value;
					}

				}
				catch (ParseException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
			return null;
		}

	//	@Override
	//	public void formatterChanged(FormatterEvent evt) {
	//		switch(evt.getEventType()) {
	//		case RESOLUTION_CHANGED:
	//			this.editorNumberFormat.setMinimumFractionDigits(evt.getNewResultion());
	//			this.editorNumberFormat.setMaximumFractionDigits(evt.getNewResultion() + EXTRA_DIGITS_ON_EDIT);
	//			break;
	//		case UNIT_CHANGED:
	//			break;
	//		}
	//
	//		if (this.tableCell != null)
	//			this.tableCell.setText(this.toString((Double)this.tableCell.getItem()));
	//	}
	}

}