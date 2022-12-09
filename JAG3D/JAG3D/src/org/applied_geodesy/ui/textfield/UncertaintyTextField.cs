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

namespace org.applied_geodesy.ui.textfield
{
	using CellValueType = org.applied_geodesy.util.CellValueType;

	public class UncertaintyTextField : DoubleTextField
	{

		public UncertaintyTextField(double? value, CellValueType type) : this(value, type, false)
		{
		}

		public UncertaintyTextField(double? value, CellValueType type, bool displayUnit) : this(value, type, displayUnit, ValueSupport.EXCLUDING_INCLUDING_INTERVAL) // GREATER_THAN_OR_EQUAL_TO_ZERO);
		{
		}

		public UncertaintyTextField(double? value, CellValueType type, bool displayUnit, ValueSupport valueSupport) : base(value, type, displayUnit, valueSupport, 0.0, double.PositiveInfinity)
		{
		}
	}


}