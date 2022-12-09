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

	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using Matrix = no.uib.cipr.matrix.Matrix;

	public class VectorAngleRestriction : Restriction
	{
		private ObservableList<UnknownParameter> regressorsA = FXCollections.observableArrayList<UnknownParameter>();
		private ObservableList<UnknownParameter> regressorsB = FXCollections.observableArrayList<UnknownParameter>();

		public VectorAngleRestriction() : this(false, new List<UnknownParameter>(0), new List<UnknownParameter>(0), null)
		{
		}

		public VectorAngleRestriction(bool indispensable, IList<UnknownParameter> regressorsA, IList<UnknownParameter> regressorsB, UnknownParameter regressand) : base(RestrictionType.VECTOR_ANGLE, indispensable)
		{
			this.Regressand = regressand;

			if (regressorsA.Count != regressorsB.Count)
			{
				throw new System.ArgumentException("Error, unequal size of factorsA and factorsB " + regressorsA.Count + " != " + regressorsB.Count);
			}

			this.regressorsA.setAll(regressorsA);
			this.regressorsB.setAll(regressorsB);
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
				double dotAB = 0;
				double dotAA = 0;
				double dotBB = 0;
    
				int length = Math.Min(this.regressorsA.size(), this.regressorsB.size());
    
				for (int i = 0; i < length; i++)
				{
					double ai = this.regressorsA.get(i).getValue();
					double bi = this.regressorsB.get(i).getValue();
    
					dotAB += ai * bi;
					dotAA += ai * ai;
					dotBB += bi * bi;
				}
    
				return Math.Acos(dotAB / Math.Sqrt(dotAA) / Math.Sqrt(dotBB)) - this.regressand.get().getValue();
			}
		}

		public override void transposedJacobianElements(Matrix JrT)
		{
			int rowIndex = this.Row;

			double dotAB = 0;
			double dotAA = 0;
			double dotBB = 0;

			int length = Math.Min(this.regressorsA.size(), this.regressorsB.size());

			for (int i = 0; i < length; i++)
			{
				double ai = this.regressorsA.get(i).getValue();
				double bi = this.regressorsB.get(i).getValue();

				dotAB += ai * bi;
				dotAA += ai * ai;
				dotBB += bi * bi;
			}

			for (int i = 0; i < length; i++)
			{
				UnknownParameter ai = this.regressorsA.get(i);
				UnknownParameter bi = this.regressorsB.get(i);

				if (ai.Column >= 0)
				{
					JrT.add(ai.Column, rowIndex, -(bi.Value / Math.Sqrt(dotAA) / Math.Sqrt(dotBB) - ai.Value * dotAB / Math.Pow(dotAA, 1.5) / Math.Sqrt(dotBB)) / Math.Sqrt(1.0 - dotAB * dotAB / dotAA / dotBB));
				}

				if (bi.Column >= 0)
				{
					JrT.add(bi.Column, rowIndex, -(ai.Value / Math.Sqrt(dotAA) / Math.Sqrt(dotBB) - bi.Value * dotAB / Math.Sqrt(dotAA) / Math.Pow(dotBB, 1.5)) / Math.Sqrt(1.0 - dotAB * dotAB / dotAA / dotBB));
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
			return this.regressand.get() == @object || this.regressorsA.contains(@object) || this.regressorsB.contains(@object);
		}

		public override string toLaTex()
		{
			return "$\\arccos{\\left(\\frac{\\mathbf a^{\\mathrm T} \\mathbf b} {\\vert \\mathbf a \\vert \\vert \\mathbf b \\vert}\\right)} = c$";
		}
	}

}