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
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class PrincipalComponentRow : Row
	{
		private bool InstanceFieldsInitialized = false;

		public PrincipalComponentRow()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			index = new SimpleObjectProperty<int>(this, "index");
			value = new SimpleObjectProperty<double>(this, "value");
			ratio = new SimpleObjectProperty<double>(this, "ratio");
		}


		private ObjectProperty<int> index;
		private ObjectProperty<double> value;
		private ObjectProperty<double> ratio;
		public virtual ObjectProperty<int> indexProperty()
		{
			return this.index;
		}

		public virtual int? Index
		{
			get
			{
				return this.indexProperty().get();
			}
			set
			{
				this.indexProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> valueProperty()
		{
			return this.value;
		}

		public virtual double? Value
		{
			get
			{
				return this.valueProperty().get();
			}
			set
			{
				this.valueProperty().set(value);
			}
		}


		public virtual ObjectProperty<double> ratioProperty()
		{
			return this.ratio;
		}

		public virtual double? Ratio
		{
			get
			{
				return this.ratioProperty().get();
			}
			set
			{
				this.ratioProperty().set(value);
			}
		}

	}

}