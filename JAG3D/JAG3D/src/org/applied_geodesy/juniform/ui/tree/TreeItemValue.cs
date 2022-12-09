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

namespace org.applied_geodesy.juniform.ui.tree
{
	using TabType = org.applied_geodesy.juniform.ui.tabpane.TabType;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public abstract class TreeItemValue<T>
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			name = new SimpleObjectProperty<string>(this, "name");
			@object = new SimpleObjectProperty<T>(this, "object");
		}

		private ObjectProperty<string> name;
		private ObjectProperty<T> @object;

		private readonly TreeItemType treeItemType;

		internal TreeItemValue(string name, TreeItemType treeItemType)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.Name = name;
			this.treeItemType = treeItemType;
		}

		public virtual ObjectProperty<T> objectProperty()
		{
			return this.@object;
		}

		public virtual TreeItemType TreeItemType
		{
			get
			{
				return this.treeItemType;
			}
		}

		public abstract TabType[] TabTypes {get;}

		public virtual T Object
		{
			get
			{
				return this.objectProperty().get();
			}
			set
			{
				this.objectProperty().set(value);
			}
		}


		public virtual ObjectProperty<string> nameProperty()
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


		public override string ToString()
		{
			return this.Name;
		}
	}

}