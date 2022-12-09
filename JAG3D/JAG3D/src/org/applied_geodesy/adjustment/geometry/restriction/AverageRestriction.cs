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

	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using Matrix = no.uib.cipr.matrix.Matrix;

	public class AverageRestriction : Restriction
	{
		private ObservableList<UnknownParameter> regressors = FXCollections.observableArrayList<UnknownParameter>();

		public AverageRestriction() : this(false, new List<UnknownParameter>(0), null)
		{
		}

		/// <summary>
		/// (A1 + A2 + ... + An) / n ==  C
		/// </summary>
		public AverageRestriction(bool indispensable, IList<UnknownParameter> regressors, UnknownParameter regressand) : base(RestrictionType.AVERAGE, indispensable)
		{
			this.Regressand = regressand;
			this.regressors.setAll(regressors);
		}

		public virtual ObservableList<UnknownParameter> Regressors
		{
			get
			{
				return this.regressors;
			}
		}

		public override double Misclosure
		{
			get
			{
				double sum = 0;
				double length = this.regressors.size();
				for (int i = 0; i < length; i++)
				{
					sum += this.regressors.get(i).getValue();
				}
    
				return sum / length - this.regressand.get().getValue();
			}
		}

		public override void transposedJacobianElements(Matrix JrT)
		{
			int rowIndex = this.Row;
			double length = this.regressors.size();

			// inner part
			for (int i = 0; i < length; i++)
			{
				// (A1 + A2 + ... + An) / n  ==  C
				if (this.regressors.get(i).getColumn() >= 0)
				{
					JrT.add(this.regressors.get(i).getColumn(), rowIndex, 1.0 / length);
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
			return this.regressand.get() == @object || this.regressors.contains(@object);
		}

		public override string toLaTex()
		{
			return "$\\frac{1}{n} \\left(a_1 + a_i + \\cdots + a_n\\right) = c$";
		}
	}

}