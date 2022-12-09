using System;
using System.Collections.Generic;

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
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using Matrix = no.uib.cipr.matrix.Matrix;

	public class ProductSumRestriction : Restriction
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			exponent = new SimpleObjectProperty<double>(this, "exponent", 1.0);
			sumArithmetic = new SimpleObjectProperty<bool>(this, "sumArithmetic", true);
		}


		private ObservableList<UnknownParameter> regressorsA = FXCollections.observableArrayList<UnknownParameter>();
		private ObservableList<UnknownParameter> regressorsB = FXCollections.observableArrayList<UnknownParameter>();
		private ObjectProperty<double> exponent;
		private ObjectProperty<bool> sumArithmetic;

		public ProductSumRestriction() : this(false, new List<UnknownParameter>(0), new List<UnknownParameter>(0), 1.0, true, null)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public ProductSumRestriction(bool indispensable, IList<UnknownParameter> regressorsA, IList<UnknownParameter> regressorsB, UnknownParameter regressand) : this(indispensable, regressorsA, regressorsB, 1.0, true, regressand)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public ProductSumRestriction(bool indispensable, IList<UnknownParameter> regressorsA, IList<UnknownParameter> regressorsB, double exponent, UnknownParameter regressand) : this(indispensable, regressorsA, regressorsB, exponent, true, regressand)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public ProductSumRestriction(bool indispensable, IList<UnknownParameter> regressorsA, IList<UnknownParameter> regressorsB, bool sumArithmetic, UnknownParameter regressand) : this(indispensable, regressorsA, regressorsB, 1.0, sumArithmetic, regressand)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// (A1 * B1 +/- A2 * B2 +/- ... +/- An * Bn) ^ (EXP)  ==  C
		/// </summary>
		public ProductSumRestriction(bool indispensable, IList<UnknownParameter> regressorsA, IList<UnknownParameter> regressorsB, double exponent, bool sumArithmetic, UnknownParameter regressand) : base(RestrictionType.PRODUCT_SUM, indispensable)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}

			if (regressorsA.Count != regressorsB.Count)
			{
				throw new System.ArgumentException("Error, unequal size of factorsA and factorsB " + regressorsA.Count + " != " + regressorsB.Count);
			}

			this.Regressand = regressand;
			this.regressorsA.setAll(regressorsA);
			this.regressorsB.setAll(regressorsB);
			this.Exponent = exponent;
			this.SumArithmetic = sumArithmetic;
		}

		public virtual double Exponent
		{
			set
			{
				this.exponent.set(value);
			}
			get
			{
				return this.exponent.get();
			}
		}


		public virtual ObjectProperty<double> exponentProperty()
		{
			return this.exponent;
		}

		public virtual bool SumArithmetic
		{
			set
			{
				this.sumArithmetic.set(value);
			}
			get
			{
				return this.sumArithmetic.get();
			}
		}


		public virtual ObjectProperty<bool> sumArithmeticProperty()
		{
			return this.sumArithmetic;
		}

		public virtual ObservableList<UnknownParameter> RegressorsA
		{
			get
			{
				return this.regressorsA;
			}
		}

		public virtual ObservableList<UnknownParameter> RegressorsB
		{
			get
			{
				return this.regressorsB;
			}
		}

		public override double Misclosure
		{
			get
			{
				double sign = this.SumArithmetic ? +1.0 : -1.0;
				double d = 0;
				int length = Math.Min(this.regressorsA.size(), this.regressorsB.size());
				for (int i = 0; i < length; i++)
				{
					d += (i == 0 ? +1.0 : sign) * this.regressorsA.get(i).getValue() * this.regressorsB.get(i).getValue();
				}
    
				return Math.Pow(d, this.exponent.get()) - this.regressand.get().getValue();
			}
		}

		public override void transposedJacobianElements(Matrix JrT)
		{
			int rowIndex = this.Row;
			double sign = this.SumArithmetic ? +1.0 : -1.0;
			int length = Math.Min(this.regressorsA.size(), this.regressorsB.size());

			// inner part
			for (int i = 0; i < length; i++)
			{
				// (A1 * B1 +/- A2 * B2 +/- ... +/- An * Bn) ^ (EXP)  ==  C
				if (this.regressorsA.get(i).getColumn() >= 0)
				{
					JrT.add(this.regressorsA.get(i).getColumn(), rowIndex, (i == 0 ? +1.0 : sign) * this.regressorsB.get(i).getValue());
				}

				if (this.regressorsB.get(i).getColumn() >= 0)
				{
					JrT.add(this.regressorsB.get(i).getColumn(), rowIndex, (i == 0 ? +1.0 : sign) * this.regressorsA.get(i).getValue());
				}
			}

			if (this.regressand.get().getColumn() >= 0)
			{
				JrT.add(this.regressand.get().getColumn(), rowIndex, -1.0);
			}

			// inner and outer
			if (this.exponent.get() != 1)
			{
				// outer part
				double outer = 0;
				for (int i = 0; i < length; i++)
				{
					outer += (i == 0 ? +1.0 : sign) * this.regressorsA.get(i).getValue() * this.regressorsB.get(i).getValue();
				}
				outer = this.exponent.get() * Math.Pow(outer, this.exponent.get() - 1.0);

				// inner * outer
				for (int columnIndex = 0; columnIndex < JrT.numRows(); columnIndex++)
				{
					JrT.set(columnIndex, rowIndex, JrT.get(columnIndex, rowIndex) * outer);
				}
			}
		}

		public override bool contains(object @object)
		{
			if (@object == null || !(@object is UnknownParameter))
			{
				return false;
			}
			return this.regressand.get() == @object || this.regressorsA.contains(@object) || this.regressorsB.contains(@object);
		}

		public override string toLaTex()
		{
			return "$\\left(a_1 b_1 \\pm a_i b_i \\pm \\cdots \\pm a_n b_n\\right)^{k} = c$";
		}
	}
}