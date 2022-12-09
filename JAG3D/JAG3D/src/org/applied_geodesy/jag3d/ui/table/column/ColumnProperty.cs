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

namespace org.applied_geodesy.jag3d.ui.table.column
{
	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using IntegerProperty = javafx.beans.property.IntegerProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;
	using SimpleIntegerProperty = javafx.beans.property.SimpleIntegerProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using SortType = javafx.scene.control.TableColumn.SortType;

	public class ColumnProperty
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			prefWidth = new SimpleDoubleProperty(this, "prefWidth", PREFERRED_WIDTH);
			sortOrder = new SimpleIntegerProperty(this, "sortOrder", -1);
			columnOrder = new SimpleIntegerProperty(this, "columnOrder", -1);
			defaultColumnOrder = new SimpleIntegerProperty(this, "defaultColumnOrder", -1);
			sortType = new SimpleObjectProperty<SortType>(this, "sortType", SortType.ASCENDING);
		}

		public const double PREFERRED_WIDTH = 125;
		public const double MIN_WIDTH = 50;

		private DoubleProperty prefWidth;
		private IntegerProperty sortOrder;
		private IntegerProperty columnOrder;
		private IntegerProperty defaultColumnOrder;
		private ObjectProperty<SortType> sortType;

		private readonly ColumnContentType columnContentType;

		internal ColumnProperty(ColumnContentType columnContentType)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.columnContentType = columnContentType;
			double prefWidth = DefaultColumnProperty.getPrefWidth(this.columnContentType);
			this.setPrefWidth(prefWidth);
		}

		public virtual ColumnContentType ColumnContentType
		{
			get
			{
				return this.columnContentType;
			}
		}

		public virtual DoubleProperty prefWidthProperty()
		{
			return this.prefWidth;
		}

		public virtual SortType SortType
		{
			get
			{
				return this.sortType.get();
			}
			set
			{
				this.sortType.set(value);
			}
		}


		public virtual ObjectProperty<SortType> sortTypeProperty()
		{
			return this.sortType;
		}

		public virtual int SortOrder
		{
			get
			{
				return this.sortOrder.get();
			}
			set
			{
				this.sortOrder.set(value);
			}
		}


		public virtual IntegerProperty sortOrderProperty()
		{
			return this.sortOrder;
		}

		public virtual int DefaultColumnOrder
		{
			get
			{
				return this.defaultColumnOrder.get();
			}
			set
			{
				this.defaultColumnOrder.set(value);
			}
		}


		public virtual int ColumnOrder
		{
			get
			{
				return this.columnOrder.get();
			}
			set
			{
				this.columnOrder.set(value);
				if (this.DefaultColumnOrder < 0)
				{
					this.DefaultColumnOrder = value;
				}
			}
		}


		public virtual IntegerProperty columnOrderProperty()
		{
			return this.columnOrder;
		}

		public virtual double? PrefWidth
		{
			get
			{
				return this.prefWidth.get();
			}
			set
			{
				this.prefWidth.set(Math.Max(value, MIN_WIDTH));
			}
		}


		public override string ToString()
		{
			return "ColumnProperty [columnContentType=" + columnContentType + ", prefWidth=" + getPrefWidth() + "]";
		}
	}

}