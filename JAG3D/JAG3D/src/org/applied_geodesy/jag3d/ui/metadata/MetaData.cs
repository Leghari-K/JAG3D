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

namespace org.applied_geodesy.jag3d.ui.metadata
{

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using SimpleStringProperty = javafx.beans.property.SimpleStringProperty;
	using StringProperty = javafx.beans.property.StringProperty;

	public class MetaData
	{
		private StringProperty name = new SimpleStringProperty();
		private StringProperty @operator = new SimpleStringProperty();
		private StringProperty customerId = new SimpleStringProperty();
		private StringProperty projectId = new SimpleStringProperty();
		private StringProperty description = new SimpleStringProperty();
		private ObjectProperty<LocalDate> date = new SimpleObjectProperty<LocalDate>(LocalDate.now());
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


		public virtual StringProperty operatorProperty()
		{
			return this.@operator;
		}

		public virtual string Operator
		{
			get
			{
				return this.operatorProperty().get();
			}
			set
			{
				this.operatorProperty().set(value);
			}
		}


		public virtual StringProperty customerIdProperty()
		{
			return this.customerId;
		}

		public virtual string CustomerId
		{
			get
			{
				return this.customerIdProperty().get();
			}
			set
			{
				this.customerIdProperty().set(value);
			}
		}


		public virtual StringProperty projectIdProperty()
		{
			return this.projectId;
		}

		public virtual string ProjectId
		{
			get
			{
				return this.projectIdProperty().get();
			}
			set
			{
				this.projectIdProperty().set(value);
			}
		}


		public virtual StringProperty descriptionProperty()
		{
			return this.description;
		}

		public virtual string Description
		{
			get
			{
				return this.descriptionProperty().get();
			}
			set
			{
				this.descriptionProperty().set(value);
			}
		}


		public virtual ObjectProperty<LocalDate> dateProperty()
		{
			return this.date;
		}

		public virtual LocalDate Date
		{
			get
			{
				return this.dateProperty().get();
			}
			set
			{
				this.dateProperty().set(value);
			}
		}

	}

}