﻿/// <summary>
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

	using I18N = org.applied_geodesy.util.i18n.I18N;

	public abstract class Unit
	{
		internal readonly I18N i18n = new I18N(Locale.getDefault(), "i18n/units");
		internal readonly UnitType type;
		internal string abbreviation, name;

		internal Unit(UnitType type)
		{
			this.type = type;
		}

		public virtual string Abbreviation
		{
			get
			{
				return this.abbreviation;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		public override string ToString()
		{
			return this.name + " [" + this.abbreviation + "]";
		}

		public virtual UnitType Type
		{
			get
			{
				return this.type;
			}
		}

		public virtual string toFormattedAbbreviation()
		{
			return this.abbreviation.Trim().Length == 0 ? "" : String.format(Locale.ENGLISH, i18n.getString("Unit.abbreviation.format", "in %s"), this.abbreviation);
		}
	}

}