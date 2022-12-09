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
	using ReadOnlyObjectProperty = javafx.beans.property.ReadOnlyObjectProperty;
	using ReadOnlyObjectWrapper = javafx.beans.property.ReadOnlyObjectWrapper;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class UnknownParameter : Parameter
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			column = new SimpleObjectProperty<int>(this, "column", -1);
			value = new SimpleObjectProperty<double>(this, "value", 0.0);
			uncertainty = new SimpleObjectProperty<double>(this, "uncertainty", 0.0);
		}

		/// <summary>
		/// Value: -1 == not set, Integer.MIN_VALUE == fixed, else column in normal equation system * </summary>
		private ObjectProperty<ProcessingType> processingType = new SimpleObjectProperty<ProcessingType>(ProcessingType.ADJUSTMENT);

		private ObjectProperty<int> column;
		private ObjectProperty<double> value;
		private ObjectProperty<double> uncertainty;
		private ReadOnlyObjectProperty<bool> indispensable;

		public UnknownParameter(ParameterType parameterType, bool indispensable) : this(parameterType, indispensable, 0, true)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public UnknownParameter(ParameterType parameterType, bool indispensable, bool visible) : this(parameterType, indispensable, 0, visible)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public UnknownParameter(ParameterType parameterType, bool indispensable, double value) : this(parameterType, indispensable, value, true)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public UnknownParameter(ParameterType parameterType, bool indispensable, double value, bool visible) : this(parameterType, indispensable, value, visible, -1, org.applied_geodesy.adjustment.geometry.parameter.ProcessingType.ADJUSTMENT)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public UnknownParameter(ParameterType parameterType, bool indispensable, double value, bool visible, ProcessingType processingType) : this(parameterType, indispensable, value, visible, -1, processingType)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public UnknownParameter(ParameterType parameterType, bool indispensable, double value, bool visible, int column, ProcessingType processingType) : base(parameterType)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.Value0 = value;
			this.Column = column;
			this.ProcessingType = processingType;
			this.Visible = visible;
			this.indispensable = new ReadOnlyObjectWrapper<bool>(this, "indispensable", indispensable);
		}

		public virtual ReadOnlyObjectProperty<bool> indispensableProperty()
		{
			return this.indispensable;
		}

		public virtual bool Indispensable
		{
			get
			{
				return this.indispensable.get();
			}
		}

		public virtual int Column
		{
			set
			{
				this.column.set(value);
			}
			get
			{
				return this.column.get();
			}
		}


		public virtual ObjectProperty<int> columnProperty()
		{
			return this.column;
		}

		public virtual ProcessingType ProcessingType
		{
			get
			{
				return this.processingType.get();
			}
			set
			{
				this.processingType.set(value);
				if (value != org.applied_geodesy.adjustment.geometry.parameter.ProcessingType.ADJUSTMENT)
				{
					this.column.set(-1);
				}
			}
		}


		public virtual ObjectProperty<ProcessingType> processingTypeProperty()
		{
			return this.processingType;
		}

		public virtual double Value
		{
			set
			{
				this.value.set(value);
			}
			get
			{
				return this.value.get();
			}
		}


		public virtual ObjectProperty<double> valueProperty()
		{
			return this.value;
		}

		public virtual double Uncertainty
		{
			set
			{
				this.uncertainty.set(value);
			}
			get
			{
				return this.uncertainty.get();
			}
		}


		public virtual ObjectProperty<double> uncertaintyProperty()
		{
			return this.uncertainty;
		}

		public override string ToString()
		{
			return "UnknownParameter [Name=" + Name + ", ParameterType=" + ParameterType + ", Value0=" + Value0 + ", Column=" + Column + ", ProcessingType=" + ProcessingType + ", Value=" + Value + "]";
		}
	}

}