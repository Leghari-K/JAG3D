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

	public class MinMaxDoubleTextField : DoubleTextField
	{
		private double min = double.NegativeInfinity, max = double.PositiveInfinity;
		private bool exclusiveMin = true, exclusiveMax = true;

		public MinMaxDoubleTextField(double? value, double min, double max, CellValueType type, bool displayUnit, bool exclusiveMin, bool exclusiveMax) : base(value, type, displayUnit, ValueSupport.NON_NULL_VALUE_SUPPORT)
		{
			this.exclusiveMin = exclusiveMin;
			this.exclusiveMax = exclusiveMax;
			this.min = Math.Min(min, max);
			this.max = Math.Max(min, max);
			this.Number = value;
			this.setText(this.getRendererFormat(value));
		}

		public override bool check(double? value)
		{
			bool validNumber = base.check(value);

			if (!validNumber)
			{
				return false;
			}

			if (value == null)
			{
				return true;
			}

			return (this.exclusiveMin ? this.min < value.Value : this.min <= value.Value) && (this.exclusiveMax ? value.Value < this.max : value.Value <= this.max);
		}
	}

}