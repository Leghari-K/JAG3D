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

namespace org.applied_geodesy.jag3d.ui.tree
{
	using TabType = org.applied_geodesy.jag3d.ui.tabpane.TabType;

	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleStringProperty = javafx.beans.property.SimpleStringProperty;
	using StringProperty = javafx.beans.property.StringProperty;

	public class TreeItemValue
	{
		private TreeItemType type;
		private StringProperty name = new SimpleStringProperty();
		private BooleanProperty enable = new SimpleBooleanProperty(true);

		internal TreeItemValue(string name) : this(TreeItemType.UNSPECIFIC, name)
		{
		}

		internal TreeItemValue(TreeItemType type, string name)
		{
			this.ItemType = type;
			this.Name = name;
		}

		public virtual TreeItemType ItemType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}


		public virtual StringProperty nameProperty()
		{
			return this.name;
		}

		public virtual string Name
		{
			get
			{
				return this.nameProperty().get();
			}
			set
			{
				this.nameProperty().set(value);
			}
		}


		public virtual TabType[] TabTypes
		{
			get
			{
				return null;
			}
		}

		internal virtual BooleanProperty enableProperty()
		{
			return this.enable;
		}

		public virtual bool Enable
		{
			get
			{
				return this.enableProperty().get();
			}
			set
			{
				this.enableProperty().set(value);
			}
		}


		public override string ToString()
		{
			return this.Name;
		}
	}

}