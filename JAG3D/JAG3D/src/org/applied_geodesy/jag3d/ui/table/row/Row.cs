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

namespace org.applied_geodesy.jag3d.ui.table.row
{
	using IntegerProperty = javafx.beans.property.IntegerProperty;
	using SimpleIntegerProperty = javafx.beans.property.SimpleIntegerProperty;

	public class Row
	{
		private IntegerProperty id = new SimpleIntegerProperty(-1);

		public virtual int Id
		{
			get
			{
				return this.id.get();
			}
			set
			{
				this.id.set(value);
			}
		}


		public virtual IntegerProperty idProperty()
		{
			return this.id;
		}

		public override string ToString()
		{
			return this.GetType().Name + " " + this.Id;
		}
	}

}