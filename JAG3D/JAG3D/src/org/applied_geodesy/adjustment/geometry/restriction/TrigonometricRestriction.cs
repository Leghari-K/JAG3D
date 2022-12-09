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

namespace org.applied_geodesy.adjustment.geometry.restriction
{
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using Matrix = no.uib.cipr.matrix.Matrix;

	public class TrigonometricRestriction : Restriction
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			trigonometricFunctionType = new SimpleObjectProperty<TrigonometricFunctionType>(this, "trigonometricFunctionType", org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.TANGENT);
			regressor = new SimpleObjectProperty<UnknownParameter>(this, "regressor");
			invert = new SimpleObjectProperty<bool>(this, "invert", false);
		}

		public enum TrigonometricFunctionType
		{
			 SINE,
			 COSINE,
			 TANGENT,
			 COTANGENT
		}

		private ObjectProperty<TrigonometricFunctionType> trigonometricFunctionType;
		private ObjectProperty<UnknownParameter> regressor;
		private ObjectProperty<bool> invert;

		public TrigonometricRestriction() : this(false, org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.TANGENT, false, null, null)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// tri(a) ==  c
		/// </summary>
		public TrigonometricRestriction(bool indispensable, TrigonometricFunctionType trigonometricFunctionType, bool invert, UnknownParameter regressor, UnknownParameter regressand) : base(RestrictionType.TRIGONOMERTIC_FUNCTION, indispensable)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.Regressand = regressand;
			this.Regressor = regressor;
			this.Invert = invert;
			this.TrigonometricFunctionType = trigonometricFunctionType;
		}

		public virtual UnknownParameter Regressor
		{
			get
			{
				return this.regressor.get();
			}
			set
			{
				this.regressor.set(value);
			}
		}


		public virtual ObjectProperty<UnknownParameter> regressorProperty()
		{
			return this.regressor;
		}

		public virtual TrigonometricFunctionType TrigonometricFunctionType
		{
			get
			{
				return this.trigonometricFunctionType.get();
			}
			set
			{
				this.trigonometricFunctionType.set(value);
			}
		}


		public virtual ObjectProperty<TrigonometricFunctionType> trigonometricFunctionTypeProperty()
		{
			return this.trigonometricFunctionType;
		}

		public virtual bool Invert
		{
			set
			{
				this.invert.set(value);
			}
			get
			{
				return this.invert.get();
			}
		}


		public virtual ObjectProperty<bool> invertProperty()
		{
			return this.invert;
		}

		public override double Misclosure
		{
			get
			{
				double a = this.regressor.get().getValue();
				double c = this.regressand.get().getValue();
				TrigonometricFunctionType type = this.trigonometricFunctionType.get();
    
				switch (type)
				{
				case org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.SINE:
					return this.Invert ? Math.Asin(a) - c : Math.Sin(a) - c;
    
				case org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.COSINE:
					return this.Invert ? Math.Acos(a) - c : Math.Cos(a) - c;
    
				case org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.TANGENT:
					return this.Invert ? Math.Atan(a) - c : Math.Tan(a) - c;
    
				case org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.COTANGENT:
					return this.Invert ? MathExtension.acot(a) - c : 1.0 / MathExtension.cot(a) - c;
				}
    
				throw new System.ArgumentException("Error, unsupported trigonometric function type " + this.TrigonometricFunctionType + "!");
			}
		}

		public override void transposedJacobianElements(Matrix JrT)
		{
			int rowIndex = this.Row;
			TrigonometricFunctionType type = this.trigonometricFunctionType.get();

			if (this.regressor.get().getColumn() >= 0)
			{
				int columnIndex = this.regressor.get().getColumn();
				double a = this.regressor.get().getValue();

				switch (type)
				{
				case org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.SINE:
					JrT.add(columnIndex, rowIndex, this.Invert ? 1.0 / Math.Sqrt(1.0 - a * a) : Math.Cos(a));
					break;

				case org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.COSINE:
					JrT.add(columnIndex, rowIndex, this.Invert ? -1.0 / Math.Sqrt(1.0 - a * a) : -Math.Sin(a));
					break;

				case org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.TANGENT:
					JrT.add(columnIndex, rowIndex, this.Invert ? 1.0 / (1.0 + a * a) : 1.0 / Math.Cos(a) / Math.Cos(a));
					break;

				case org.applied_geodesy.adjustment.geometry.restriction.TrigonometricRestriction.TrigonometricFunctionType.COTANGENT:
					JrT.add(columnIndex, rowIndex, this.Invert ? -1.0 / (1.0 + a * a) : -1.0 / Math.Sin(a) / Math.Sin(a));
					break;

				default:
					throw new System.ArgumentException("Error, unsupported trigonometric function type " + this.TrigonometricFunctionType + "!");
				}
			}

			if (this.regressand.get().getColumn() >= 0)
			{
				JrT.add(this.regressand.get().getColumn(), rowIndex, -1.0);
			}
		}

		public override bool contains(object @object)
		{
			if (@object == null || !(@object is UnknownParameter))
			{
				return false;
			}
			return this.regressand.get() == @object || this.regressor.get() == @object;
		}

		public override string toLaTex()
		{
			return "$\\mathrm{trigon} \\left( a \\right) = c$";
		}
	}

}