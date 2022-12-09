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

namespace org.applied_geodesy.util
{

	using AngleUnit = org.applied_geodesy.util.unit.AngleUnit;
	using LengthUnit = org.applied_geodesy.util.unit.LengthUnit;
	using PercentUnit = org.applied_geodesy.util.unit.PercentUnit;
	using PressureUnit = org.applied_geodesy.util.unit.PressureUnit;
	using ScaleUnit = org.applied_geodesy.util.unit.ScaleUnit;
	using TemperatureUnit = org.applied_geodesy.util.unit.TemperatureUnit;
	using Unit = org.applied_geodesy.util.unit.Unit;
	using UnitType = org.applied_geodesy.util.unit.UnitType;

	public class FormatterOptions
	{
		public class FormatterOption
		{
			private readonly FormatterOptions outerInstance;

			internal readonly CellValueType type;
			internal NumberFormat format;
			internal Unit unit = null;
			internal FormatterOption(FormatterOptions outerInstance, CellValueType type, NumberFormat format, Unit unit)
			{
				this.outerInstance = outerInstance;
				this.type = type;
				this.format = format;
				this.unit = unit;
			}
			public virtual Unit Unit
			{
				set
				{
					if (this.unit == null || this.unit.GetType().Equals(value.GetType()))
					{
						Unit oldUnit = this.unit;
						if (!this.unit.Equals(value))
						{
							this.unit = value;
							outerInstance.fireUnitChanged(this.type, oldUnit, value, this.FractionDigits);
						}
					}
				}
				get
				{
					return this.unit;
				}
			}
			public virtual int FractionDigits
			{
				get
				{
					return this.format.getMaximumFractionDigits();
				}
				set
				{
					if (value < 0)
					{
						return;
					}
					int o = this.format.getMaximumFractionDigits();
					if (o != value)
					{
						this.format.setMaximumFractionDigits(value);
						this.format.setMinimumFractionDigits(value);
						outerInstance.fireResolutionChanged(this.type, this.unit, o, value);
					}
				}
			}
			public CellValueType Type
			{
				get
				{
					return this.type;
				}
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Number parse(String source, java.text.ParsePosition parsePosition) throws java.text.ParseException
			public virtual Number parse(string source, ParsePosition parsePosition)
			{
				return this.format.parse(source, parsePosition);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Number parse(String source) throws java.text.ParseException
			public virtual Number parse(string source)
			{
				return this.format.parse(source);
			}
			public virtual NumberFormat Formatter
			{
				get
				{
					return this.format;
				}
			}
		}

		private IList<EventListener> listenerList = new List<EventListener>();
		private IDictionary<CellValueType, FormatterOption> formatterOptions = new Dictionary<CellValueType, FormatterOption>();
		private static FormatterOptions options = new FormatterOptions();

		private FormatterOptions()
		{
			this.init();
		}

		private void init()
		{
			LengthUnit LENGTH_UNIT = DefaultFormatterOption.LengthUnit;
			AngleUnit ANGLE_UNIT = DefaultFormatterOption.AngleUnit;
			ScaleUnit SCALE_UNIT = DefaultFormatterOption.ScaleUnit;
			LengthUnit VECTOR_UNIT = DefaultFormatterOption.VectorUnit;

			LengthUnit LENGTH_UNCERTAINTY_UNIT = DefaultFormatterOption.LengthUncertaintyUnit;
			AngleUnit ANGLE_UNCERTAINTY_UNIT = DefaultFormatterOption.AngleUncertaintyUnit;
			ScaleUnit SCALE_UNCERTAINTY_UNIT = DefaultFormatterOption.ScaleUncertaintyUnit;
			LengthUnit VECTOR_UNCERTAINTY_UNIT = DefaultFormatterOption.VectorUncertaintyUnit;

			LengthUnit LENGTH_RESIDUAL_UNIT = DefaultFormatterOption.LengthResidualUnit;
			AngleUnit ANGLE_RESIDUAL_UNIT = DefaultFormatterOption.AngleResidualUnit;
			ScaleUnit SCALE_RESIDUAL_UNIT = DefaultFormatterOption.ScaleResidualUnit;
			LengthUnit VECTOR_RESIDUAL_UNIT = DefaultFormatterOption.VectorResidualUnit;

			TemperatureUnit TEMPERATURE_UNIT = DefaultFormatterOption.TemperatureUnit;
			PressureUnit PRESSURE_UNIT = DefaultFormatterOption.PressureUnit;
			PercentUnit PERCENT_UNIT = DefaultFormatterOption.PercentUnit;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat lengthFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat lengthFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat angleFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat angleFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat scaleFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat scaleFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat vectorFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat vectorFormatter = NumberFormat.getInstance(Locale.ENGLISH);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat lengthUncertaintyFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat lengthUncertaintyFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat angleUncertaintyFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat angleUncertaintyFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat scaleUncertaintyFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat scaleUncertaintyFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat vectorUncertaintyFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat vectorUncertaintyFormatter = NumberFormat.getInstance(Locale.ENGLISH);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat lengthResidualFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat lengthResidualFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat angleResidualFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat angleResidualFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat scaleResidualFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat scaleResidualFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat vectorResidualFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat vectorResidualFormatter = NumberFormat.getInstance(Locale.ENGLISH);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat statisticFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat statisticFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat doubleFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat doubleFormatter = NumberFormat.getInstance(Locale.ENGLISH);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat temperatureFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat temperatureFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat pressureFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat pressureFormatter = NumberFormat.getInstance(Locale.ENGLISH);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat percentFormatter = java.text.NumberFormat.getInstance(java.util.Locale.ENGLISH);
			NumberFormat percentFormatter = NumberFormat.getInstance(Locale.ENGLISH);

			lengthFormatter.setGroupingUsed(false);
			angleFormatter.setGroupingUsed(false);
			scaleFormatter.setGroupingUsed(false);
			vectorFormatter.setGroupingUsed(false);
			statisticFormatter.setGroupingUsed(false);
			doubleFormatter.setGroupingUsed(false);

			lengthUncertaintyFormatter.setGroupingUsed(false);
			angleUncertaintyFormatter.setGroupingUsed(false);
			scaleUncertaintyFormatter.setGroupingUsed(false);
			vectorUncertaintyFormatter.setGroupingUsed(false);

			lengthResidualFormatter.setGroupingUsed(false);
			angleResidualFormatter.setGroupingUsed(false);
			scaleResidualFormatter.setGroupingUsed(false);
			vectorResidualFormatter.setGroupingUsed(false);

			temperatureFormatter.setGroupingUsed(false);
			pressureFormatter.setGroupingUsed(false);
			percentFormatter.setGroupingUsed(false);

			lengthFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			angleFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			scaleFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			vectorFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			statisticFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			doubleFormatter.setRoundingMode(RoundingMode.HALF_EVEN);

			lengthUncertaintyFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			angleUncertaintyFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			scaleUncertaintyFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			vectorUncertaintyFormatter.setRoundingMode(RoundingMode.HALF_EVEN);

			lengthResidualFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			angleResidualFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			scaleResidualFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			vectorResidualFormatter.setRoundingMode(RoundingMode.HALF_EVEN);

			temperatureFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			pressureFormatter.setRoundingMode(RoundingMode.HALF_EVEN);
			percentFormatter.setRoundingMode(RoundingMode.HALF_EVEN);

			this.setFractionDigits(lengthFormatter, DefaultFormatterOption.LengthFractionDigits);
			this.setFractionDigits(angleFormatter, DefaultFormatterOption.AngleFractionDigits);
			this.setFractionDigits(scaleFormatter, DefaultFormatterOption.ScaleFractionDigits);
			this.setFractionDigits(vectorFormatter, DefaultFormatterOption.VectorFractionDigits);
			this.setFractionDigits(statisticFormatter, DefaultFormatterOption.StatisticFractionDigits);
			this.setFractionDigits(doubleFormatter, DefaultFormatterOption.DoubleFractionDigits);

			this.setFractionDigits(lengthUncertaintyFormatter, DefaultFormatterOption.LengthUncertaintyFractionDigits);
			this.setFractionDigits(angleUncertaintyFormatter, DefaultFormatterOption.AngleUncertaintyFractionDigits);
			this.setFractionDigits(scaleUncertaintyFormatter, DefaultFormatterOption.ScaleUncertaintyFractionDigits);
			this.setFractionDigits(vectorUncertaintyFormatter, DefaultFormatterOption.VectorUncertaintyFractionDigits);

			this.setFractionDigits(lengthResidualFormatter, DefaultFormatterOption.LengthResidualFractionDigits);
			this.setFractionDigits(angleResidualFormatter, DefaultFormatterOption.AngleResidualFractionDigits);
			this.setFractionDigits(scaleResidualFormatter, DefaultFormatterOption.ScaleResidualFractionDigits);
			this.setFractionDigits(vectorResidualFormatter, DefaultFormatterOption.VectorResidualFractionDigits);

			this.setFractionDigits(temperatureFormatter, DefaultFormatterOption.TemperatureFractionDigits);
			this.setFractionDigits(pressureFormatter, DefaultFormatterOption.PressureFractionDigits);
			this.setFractionDigits(percentFormatter, DefaultFormatterOption.PercentFractionDigits);

			this.formatterOptions[CellValueType.LENGTH] = new FormatterOption(this, CellValueType.LENGTH, lengthFormatter, LENGTH_UNIT);
			this.formatterOptions[CellValueType.LENGTH_UNCERTAINTY] = new FormatterOption(this, CellValueType.LENGTH_UNCERTAINTY, lengthUncertaintyFormatter, LENGTH_UNCERTAINTY_UNIT);
			this.formatterOptions[CellValueType.LENGTH_RESIDUAL] = new FormatterOption(this, CellValueType.LENGTH_RESIDUAL, lengthResidualFormatter, LENGTH_RESIDUAL_UNIT);

			this.formatterOptions[CellValueType.ANGLE] = new FormatterOption(this, CellValueType.ANGLE, angleFormatter, ANGLE_UNIT);
			this.formatterOptions[CellValueType.ANGLE_UNCERTAINTY] = new FormatterOption(this, CellValueType.ANGLE_UNCERTAINTY, angleUncertaintyFormatter, ANGLE_UNCERTAINTY_UNIT);
			this.formatterOptions[CellValueType.ANGLE_RESIDUAL] = new FormatterOption(this, CellValueType.ANGLE_RESIDUAL, angleResidualFormatter, ANGLE_RESIDUAL_UNIT);

			this.formatterOptions[CellValueType.SCALE] = new FormatterOption(this, CellValueType.SCALE, scaleFormatter, SCALE_UNIT);
			this.formatterOptions[CellValueType.SCALE_UNCERTAINTY] = new FormatterOption(this, CellValueType.SCALE_UNCERTAINTY, scaleUncertaintyFormatter, SCALE_UNCERTAINTY_UNIT);
			this.formatterOptions[CellValueType.SCALE_RESIDUAL] = new FormatterOption(this, CellValueType.SCALE_RESIDUAL, vectorResidualFormatter, SCALE_RESIDUAL_UNIT);

			this.formatterOptions[CellValueType.VECTOR] = new FormatterOption(this, CellValueType.VECTOR, vectorFormatter, VECTOR_UNIT);
			this.formatterOptions[CellValueType.VECTOR_UNCERTAINTY] = new FormatterOption(this, CellValueType.VECTOR_UNCERTAINTY, vectorUncertaintyFormatter, VECTOR_UNCERTAINTY_UNIT);
			this.formatterOptions[CellValueType.VECTOR_RESIDUAL] = new FormatterOption(this, CellValueType.VECTOR_RESIDUAL, vectorUncertaintyFormatter, VECTOR_RESIDUAL_UNIT);

			this.formatterOptions[CellValueType.STATISTIC] = new FormatterOption(this, CellValueType.STATISTIC, statisticFormatter, null);
			this.formatterOptions[CellValueType.DOUBLE] = new FormatterOption(this, CellValueType.DOUBLE, doubleFormatter, null);

			this.formatterOptions[CellValueType.TEMPERATURE] = new FormatterOption(this, CellValueType.TEMPERATURE, temperatureFormatter, TEMPERATURE_UNIT);
			this.formatterOptions[CellValueType.PRESSURE] = new FormatterOption(this, CellValueType.PRESSURE, pressureFormatter, PRESSURE_UNIT);
			this.formatterOptions[CellValueType.PERCENTAGE] = new FormatterOption(this, CellValueType.PERCENTAGE, percentFormatter, PERCENT_UNIT);

		}

		private void setFractionDigits(NumberFormat format, int d)
		{
			if (d < 0)
			{
				return;
			}
			format.setMaximumFractionDigits(d);
			format.setMinimumFractionDigits(d);
		}

		public virtual IDictionary<CellValueType, FormatterOption> FormatterOptions
		{
			get
			{
				return this.formatterOptions;
			}
		}

		public static FormatterOptions Instance
		{
			get
			{
				return options;
			}
		}


		public virtual double convertTemperatureToView(double d)
		{
			return ((TemperatureUnit)this.formatterOptions[CellValueType.TEMPERATURE].Unit).fromDegreeCelsius(d);
		}

		public virtual double convertTemperatureToModel(double d)
		{
			return ((TemperatureUnit)this.formatterOptions[CellValueType.TEMPERATURE].Unit).toDegreeCelsius(d);
		}

		public virtual double convertPressureToView(double d)
		{
			return ((PressureUnit)this.formatterOptions[CellValueType.PRESSURE].Unit).fromHectopascal(d);
		}

		public virtual double convertPressureToModel(double d)
		{
			return ((PressureUnit)this.formatterOptions[CellValueType.PRESSURE].Unit).toHectopascal(d);
		}

		public virtual double convertPercentToView(double d)
		{
			return ((PercentUnit)this.formatterOptions[CellValueType.PERCENTAGE].Unit).fromUnitless(d);
		}

		public virtual double convertPercentToModel(double d)
		{
			return ((PercentUnit)this.formatterOptions[CellValueType.PERCENTAGE].Unit).toUnitless(d);
		}

		public virtual double convertScaleToView(double d)
		{
			return ((ScaleUnit)this.formatterOptions[CellValueType.SCALE].Unit).fromUnitless(d);
		}

		public virtual double convertScaleToModel(double d)
		{
			return ((ScaleUnit)this.formatterOptions[CellValueType.SCALE].Unit).toUnitless(d);
		}

		public virtual double convertLengthToView(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.LENGTH].Unit).fromMeter(d);
		}

		public virtual double convertLengthToModel(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.LENGTH].Unit).toMeter(d);
		}

		public virtual double convertLengthResidualToView(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.LENGTH_RESIDUAL].Unit).fromMeter(d);
		}

		public virtual double convertLengthResidualToModel(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.LENGTH_RESIDUAL].Unit).toMeter(d);
		}

		public virtual double convertAngleToView(double d)
		{
			return ((AngleUnit)this.formatterOptions[CellValueType.ANGLE].Unit).fromRadian(d);
		}

		public virtual double convertAngleToModel(double d)
		{
			return ((AngleUnit)this.formatterOptions[CellValueType.ANGLE].Unit).toRadian(d);
		}

		public virtual double convertAngleResidualToView(double d)
		{
			return ((AngleUnit)this.formatterOptions[CellValueType.ANGLE_RESIDUAL].Unit).fromRadian(d);
		}

		public virtual double convertAngleResidualToModel(double d)
		{
			return ((AngleUnit)this.formatterOptions[CellValueType.ANGLE_RESIDUAL].Unit).toRadian(d);
		}

		public virtual double convertVectorToView(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.VECTOR].Unit).fromMeter(d);
		}

		public virtual double convertVectorToModel(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.VECTOR].Unit).toMeter(d);
		}

		public virtual double convertLengthUncertaintyToView(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.LENGTH_UNCERTAINTY].Unit).fromMeter(d);
		}

		public virtual double convertLengthUncertaintyToModel(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.LENGTH_UNCERTAINTY].Unit).toMeter(d);
		}

		public virtual double convertAngleUncertaintyToView(double d)
		{
			return ((AngleUnit)this.formatterOptions[CellValueType.ANGLE_UNCERTAINTY].Unit).fromRadian(d);
		}

		public virtual double convertAngleUncertaintyToModel(double d)
		{
			return ((AngleUnit)this.formatterOptions[CellValueType.ANGLE_UNCERTAINTY].Unit).toRadian(d);
		}

		public virtual double convertScaleUncertaintyToView(double d)
		{
			return ((ScaleUnit)this.formatterOptions[CellValueType.SCALE_UNCERTAINTY].Unit).fromUnitless(d);
		}

		public virtual double convertScaleUncertaintyToModel(double d)
		{
			return ((ScaleUnit)this.formatterOptions[CellValueType.SCALE_UNCERTAINTY].Unit).toUnitless(d);
		}

		public virtual double convertScaleResidualToView(double d)
		{
			return ((ScaleUnit)this.formatterOptions[CellValueType.SCALE_RESIDUAL].Unit).fromUnitless(d);
		}

		public virtual double convertScaleResidualToModel(double d)
		{
			return ((ScaleUnit)this.formatterOptions[CellValueType.SCALE_RESIDUAL].Unit).toUnitless(d);
		}

		public virtual double convertVectorUncertaintyToView(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.VECTOR_UNCERTAINTY].Unit).fromMeter(d);
		}

		public virtual double convertVectorUncertaintyToModel(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.VECTOR_UNCERTAINTY].Unit).toMeter(d);
		}

		public virtual double convertVectorResidualToView(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.VECTOR_RESIDUAL].Unit).fromMeter(d);
		}

		public virtual double convertVectorResidualToModel(double d)
		{
			return ((LengthUnit)this.formatterOptions[CellValueType.VECTOR_RESIDUAL].Unit).toMeter(d);
		}


		public virtual string toTemperatureFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.TEMPERATURE, this.convertTemperatureToView(d), displayUnit);
		}

		public virtual string toPressureFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.PRESSURE, this.convertPressureToView(d), displayUnit);
		}

		public virtual string toPercentFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.PERCENTAGE, this.convertPercentToView(d), displayUnit);
		}


		public virtual string toAngleFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.ANGLE, this.convertAngleToView(d), displayUnit);
		}

		public virtual string toAngleUncertaintyFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.ANGLE_UNCERTAINTY, this.convertAngleUncertaintyToView(d), displayUnit);
		}

		public virtual string toAngleResidualFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.ANGLE_RESIDUAL, this.convertAngleResidualToView(d), displayUnit);
		}

		public virtual string toLengthFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.LENGTH, this.convertLengthToView(d), displayUnit);
		}

		public virtual string toScaleFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.SCALE, this.convertScaleToView(d), displayUnit);
		}

		public virtual string toVectorFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.VECTOR, this.convertVectorToView(d), displayUnit);
		}

		public virtual string toStatisticFormat(double d)
		{
			return this.formatterOptions[CellValueType.STATISTIC].Formatter.format(d).Trim();
		}

		public virtual string toDoubleFormat(double d)
		{
			return this.formatterOptions[CellValueType.DOUBLE].Formatter.format(d).Trim();
		}

		public virtual string toLengthUncertaintyFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.LENGTH_UNCERTAINTY, this.convertLengthUncertaintyToView(d), displayUnit);
		}

	//	public String toSquareRootLengthUncertaintyFormat(double d, boolean displayUnit) {
	//		return String.format(Locale.ENGLISH, "%s %s",
	//				this.formatterOptions.get(CellValueType.LENGTH_UNCERTAINTY).getFormatter().format(this.convertLengthUncertaintyToView(d)), 
	//				displayUnit ? "\u221A"+this.formatterOptions.get(CellValueType.LENGTH_UNCERTAINTY).getUnit().getAbbreviation():"").trim();
	//	}

		public virtual string toLengthResidualFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.LENGTH_RESIDUAL, this.convertLengthResidualToView(d), displayUnit);
		}

		public virtual string toScaleUncertaintyFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.SCALE_UNCERTAINTY, this.convertScaleUncertaintyToView(d), displayUnit);
		}

		public virtual string toScaleResidualFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.SCALE_RESIDUAL, this.convertScaleResidualToView(d), displayUnit);
		}

	//	public String toVectorUncertaintyFormat(double d, boolean displayUnit) {
	//		return String.format(Locale.ENGLISH, "%s %s",
	//				this.formatterOptions.get(CellValueType.VECTOR_UNCERTAINTY).getFormatter().format(this.convertVectorUncertaintyToView(d)), 
	//				displayUnit ? this.formatterOptions.get(CellValueType.SCALE_UNCERTAINTY).getUnit().getAbbreviation():"").trim();
	//	}

		public virtual string toVectorUncertaintyFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.VECTOR_UNCERTAINTY, this.convertVectorUncertaintyToView(d), displayUnit);
		}

		public virtual string toVectorResidualFormat(double d, bool displayUnit)
		{
			return this.toViewFormat(CellValueType.VECTOR_RESIDUAL, this.convertVectorResidualToView(d), displayUnit);
		}

		public virtual string toViewFormat(CellValueType type, double d, bool displayUnit)
		{
			Unit unit = this.formatterOptions[type].Unit;
			if (unit.Type == UnitType.DEGREE_SEXAGESIMAL)
			{
				double[] sigdms = this.toSexagesimalDegree(d);

				double sign = sigdms[0];
				int degrees = (int)sigdms[1];
				int minutes = (int)sigdms[2];
				double seconds = sigdms[3];

				return String.format(Locale.ENGLISH, "%s%d \u00B7 %02d \u00B7 %s %s", sign < 0 ? "-":"", degrees, minutes, this.formatterOptions[type].Formatter.format(100 + seconds).Substring(1), displayUnit ? unit.Abbreviation:"").Trim();
			}

			// in any other case
			return String.format(Locale.ENGLISH, "%s %s", this.formatterOptions[type].Formatter.format(d), displayUnit ? this.formatterOptions[type].Unit.getAbbreviation():"").Trim();
		}

		private double[] toSexagesimalDegree(double d)
		{
			double sign = Math.Sign(d);
			d = sign * d;
			int degrees = (int) Math.Floor(d);
			double fractions = ((d - degrees) * 60.0);
			int minutes = (int) Math.Floor(fractions);
			double seconds = (fractions - minutes) * 60.0;

			if (seconds == 60.0)
			{
				minutes++;
				seconds = 0;
			}
			if (minutes == 60.0)
			{
				degrees++;
				minutes = 0;
			}

			return new double[] {sign, degrees, minutes, seconds};
		}

		protected internal virtual void fireUnitChanged(CellValueType type, Unit oldUnit, Unit newUnit, int res)
		{
			FormatterEvent evt = new FormatterEvent(this, FormatterEventType.UNIT_CHANGED, type, oldUnit, newUnit, res);
			object[] listeners = this.listenerList.ToArray();
			for (int i = 0; i < listeners.Length; i++)
			{
				if (listeners[i] is FormatterChangedListener)
				{
					((FormatterChangedListener)listeners[i]).formatterChanged(evt);
				}
			}
		}

		protected internal virtual void fireResolutionChanged(CellValueType type, Unit unit, int oldValue, int newValue)
		{
			FormatterEvent evt = new FormatterEvent(this, FormatterEventType.RESOLUTION_CHANGED, type, unit, oldValue, newValue);
			object[] listeners = this.listenerList.ToArray();
			for (int i = 0; i < listeners.Length; i++)
			{
				if (listeners[i] is FormatterChangedListener)
				{
					((FormatterChangedListener)listeners[i]).formatterChanged(evt);
				}
			}
		}

		public virtual void addFormatterChangedListener(FormatterChangedListener l)
		{
			this.listenerList.Add(l);
		}

		public virtual void removeFormatterChangedListener(FormatterChangedListener l)
		{
			this.listenerList.Remove(l);
		}
	}

}