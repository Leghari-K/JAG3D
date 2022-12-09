using System.Collections.Generic;

namespace org.applied_geodesy.util.unit
{

	public class PercentUnit : Unit
	{
		public static PercentUnit UNITLESS = new PercentUnit(UnitType.UNITLESS);
		public static PercentUnit PERCENT = new PercentUnit(UnitType.PERCENT);

		public static readonly IDictionary<UnitType, PercentUnit> UNITS;
		static PercentUnit()
		{
			UNITS = new LinkedHashMap<UnitType, PercentUnit>();
			UNITS[UnitType.UNITLESS] = UNITLESS;
			UNITS[UnitType.PERCENT] = PERCENT;
		}

		private double conversionFactorToUnitless = 1.0;

		private PercentUnit(UnitType type) : base(type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.UNITLESS:
				this.name = i18n.getString("Unit.unitless.name", "Unitless");
				this.abbreviation = i18n.getString("Unit.unitless.abbreviation", "\u2014");
				this.conversionFactorToUnitless = 1.0;
				break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.PERCENT:
				this.name = i18n.getString("Unit.percent.name", "Percent");
				this.abbreviation = i18n.getString("Unit.percent.abbreviation", "\u0025");
				this.conversionFactorToUnitless = 1.0 / 100.0;
				break;
			default:
				break;
			}
		}

		public double toUnitless(double d)
		{
			return d * this.conversionFactorToUnitless;
		}

		public double fromUnitless(double d)
		{
			return d / this.conversionFactorToUnitless;
		}

		public static PercentUnit getUnit(UnitType type)
		{
			return UNITS[type];
		}
	}
}