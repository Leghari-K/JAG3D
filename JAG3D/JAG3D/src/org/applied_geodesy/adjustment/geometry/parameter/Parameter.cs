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

namespace org.applied_geodesy.adjustment.geometry.parameter
{
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class Parameter
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			parameterType = new SimpleObjectProperty<ParameterType>(this, "parameterType");
			value0 = new SimpleObjectProperty<double>(this, "value0", 0.0);
			description = new SimpleObjectProperty<string>(this, "description", "");
			name = new SimpleObjectProperty<string>(this, "name", "");
			visible = new SimpleObjectProperty<bool>(this, "visible", true);
		}

		private ObjectProperty<ParameterType> parameterType;
		private ObjectProperty<double> value0;
		private ObjectProperty<string> description;
		private ObjectProperty<string> name;
		private ObjectProperty<bool> visible;

		public Parameter(ParameterType parameterType) : this(parameterType, true)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public Parameter(ParameterType parameterType, bool visible)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.ParameterType = parameterType;
			this.Visible = visible;
		}

		public virtual ObjectProperty<ParameterType> parameterTypeProperty()
		{
			return this.parameterType;
		}

		public virtual ParameterType ParameterType
		{
			set
			{
				this.parameterType.set(value);
			}
			get
			{
				return this.parameterType.get();
			}
		}


		public virtual bool Visible
		{
			set
			{
				this.visible.set(value);
			}
			get
			{
				return this.visible.get();
			}
		}


		public virtual ObjectProperty<bool> visibleProperty()
		{
			return this.visible;
		}

		public virtual double Value0
		{
			set
			{
				this.value0.set(value);
			}
			get
			{
				return this.value0.get();
			}
		}


		public virtual ObjectProperty<double> value0Property()
		{
			return this.value0;
		}

		public virtual string Description
		{
			set
			{
				this.description.set(value);
			}
			get
			{
				return this.description.get();
			}
		}


		public virtual ObjectProperty<string> descriptionProperty()
		{
			return this.description;
		}

		public virtual string Name
		{
			set
			{
				this.name.set(value);
			}
			get
			{
				return this.name.get();
			}
		}


		public virtual ObjectProperty<string> nameProperty()
		{
			return this.name;
		}

		public override string ToString()
		{
			return this.name.get();
		}
	}

}