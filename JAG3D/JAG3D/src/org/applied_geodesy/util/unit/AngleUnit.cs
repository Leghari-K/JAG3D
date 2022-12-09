﻿using System.Collections.Generic;

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

namespace org.applied_geodesy.util.unit
{

	using Constant = org.applied_geodesy.adjustment.Constant;

	public class AngleUnit : Unit
	{
		public static AngleUnit RADIAN = new AngleUnit(UnitType.RADIAN);
		public static AngleUnit DEGREE = new AngleUnit(UnitType.DEGREE);
		public static AngleUnit DEGREE_SEXAGESIMAL = new AngleUnit(UnitType.DEGREE_SEXAGESIMAL);
		public static AngleUnit GRADIAN = new AngleUnit(UnitType.GRADIAN);
		public static AngleUnit MILLIRADIAN = new AngleUnit(UnitType.MILLIRADIAN);
		public static AngleUnit ARCSECOND = new AngleUnit(UnitType.ARCSECOND);
		public static AngleUnit MILLIGRADIAN = new AngleUnit(UnitType.MILLIGRADIAN);
		public static AngleUnit MIL6400 = new AngleUnit(UnitType.MIL6400);

		public static readonly IDictionary<UnitType, AngleUnit> UNITS;
		static AngleUnit()
		{
			UNITS = new LinkedHashMap<UnitType, AngleUnit>();
			UNITS[UnitType.RADIAN] = RADIAN;
			UNITS[UnitType.GRADIAN] = GRADIAN;
			UNITS[UnitType.DEGREE] = DEGREE;
			UNITS[UnitType.DEGREE_SEXAGESIMAL] = DEGREE_SEXAGESIMAL;
			UNITS[UnitType.MILLIRADIAN] = MILLIRADIAN;
			UNITS[UnitType.MILLIGRADIAN] = MILLIGRADIAN;
			UNITS[UnitType.ARCSECOND] = ARCSECOND;
			UNITS[UnitType.MIL6400] = MIL6400;
		}

		private double conversionFactorToRadian = 1.0;

		private AngleUnit(UnitType type) : base(type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.RADIAN:
				this.name = i18n.getString("Unit.radian.name", "Radian");
				this.abbreviation = i18n.getString("Unit.radian.abbreviation", "rad");
				this.conversionFactorToRadian = 1.0;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.DEGREE:
				this.name = i18n.getString("Unit.degree.name", "Degree");
				this.abbreviation = i18n.getString("Unit.degree.abbreviation", "\u00B0");
				this.conversionFactorToRadian = Constant.RHO_DEG2RAD;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.DEGREE_SEXAGESIMAL:
				this.name = i18n.getString("Unit.degree_sexagesimal.name", "Sexagesimal degree");
				this.abbreviation = i18n.getString("Unit.degree_sexagesimal.abbreviation", "\u00B0 \u2032 \u2033");
				this.conversionFactorToRadian = Constant.RHO_DEG2RAD;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.GRADIAN:
				this.name = i18n.getString("Unit.gradian.name", "Gradian");
				this.abbreviation = i18n.getString("Unit.gradian.abbreviation", "gon");
				this.conversionFactorToRadian = Constant.RHO_GRAD2RAD;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.MILLIRADIAN:
				this.name = i18n.getString("Unit.milliradian.name", "Milliradian");
				this.abbreviation = i18n.getString("Unit.milliradian.abbreviation", "mrad");
				this.conversionFactorToRadian = 1E-3;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.ARCSECOND:
				this.name = i18n.getString("Unit.arcsecond.name", "Arcsecond");
				this.abbreviation = i18n.getString("Unit.arcsecond.abbreviation", "\u2033");
				this.conversionFactorToRadian = Constant.RHO_DEG2RAD / 3600.0;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.MILLIGRADIAN:
				this.name = i18n.getString("Unit.milligradian.name", "Milligradian");
				this.abbreviation = i18n.getString("Unit.milligradian.abbreviation", "mgon");
				this.conversionFactorToRadian = Constant.RHO_GRAD2RAD * 1E-3;
			break;
			case org.applied_geodesy.util.unit.UnitType.InnerEnum.MIL6400:
				this.name = i18n.getString("Unit.mil6400.name", "Mil");
				this.abbreviation = i18n.getString("Unit.mil6400.abbreviation", "\u00AF");
				this.conversionFactorToRadian = Constant.RHO_MIL2RAD;
			break;
			default:
				break;
			}
		}

		public double toRadian(double d)
		{
			return d * this.conversionFactorToRadian;
		}

		public double fromRadian(double d)
		{
			return d / this.conversionFactorToRadian;
		}

		public static AngleUnit getUnit(UnitType type)
		{
			return UNITS[type];
		}
	}
}