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

namespace org.applied_geodesy.adjustment.geometry.restriction
{
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using ReadOnlyObjectProperty = javafx.beans.property.ReadOnlyObjectProperty;
	using ReadOnlyObjectWrapper = javafx.beans.property.ReadOnlyObjectWrapper;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using Matrix = no.uib.cipr.matrix.Matrix;

	public abstract class Restriction
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			restrictionType = new SimpleObjectProperty<RestrictionType>(this, "restrictionType");
			regressand = new SimpleObjectProperty<UnknownParameter>(this, "regressand");
			description = new SimpleObjectProperty<string>(this, "description", null);
		}

		private ObjectProperty<RestrictionType> restrictionType;
		internal ObjectProperty<UnknownParameter> regressand;
		private ObjectProperty<string> description;
		private ReadOnlyObjectProperty<bool> indispensable;
		private int row = -1;

		internal Restriction(RestrictionType restrictionType, bool indispensable)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.RestrictionType = restrictionType;
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

		public virtual RestrictionType RestrictionType
		{
			set
			{
				this.restrictionType.set(value);
			}
			get
			{
				return this.restrictionType.get();
			}
		}


		public virtual ObjectProperty<RestrictionType> restrictionTypeProperty()
		{
			return this.restrictionType;
		}

		public virtual int Row
		{
			set
			{
				this.row = value;
			}
			get
			{
				return this.row;
			}
		}


		public virtual UnknownParameter Regressand
		{
			get
			{
				return this.regressand.get();
			}
			set
			{
				this.regressand.set(value);
			}
		}


		public virtual ObjectProperty<UnknownParameter> regressandProperty()
		{
			return this.regressand;
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

		public abstract string toLaTex();

		public abstract double Misclosure {get;}

		public abstract void transposedJacobianElements(Matrix JrT);

		public abstract bool contains(object @object);
	}

}