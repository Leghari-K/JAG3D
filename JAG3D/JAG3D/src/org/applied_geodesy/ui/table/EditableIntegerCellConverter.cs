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
	public class EditableIntegerCellConverter : EditableCellConverter<int>
	{

		public EditableIntegerCellConverter()
		{
		}

		public override string toEditorString(int? value)
		{
			try
			{
				if (value == null)
				{
					return "";
				}
				return value.ToString();
			}
			catch (System.ArgumentException iae)
			{
				Console.WriteLine(iae.ToString());
				Console.Write(iae.StackTrace);
			}
			return "";
		}

		public override string toString(int? value)
		{
			if (value == null)
			{
				return "";
			}
			return value.ToString();
		}

		public override int? fromString(string @string)
		{
			if (!string.ReferenceEquals(@string, null) && @string.Trim().Length > 0)
			{
				try
				{
					return int.Parse(@string);
				}
				catch (System.FormatException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
			return null;
		}
	}

}