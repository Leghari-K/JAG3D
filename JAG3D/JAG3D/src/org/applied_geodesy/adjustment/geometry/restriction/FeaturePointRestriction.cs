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

	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using Matrix = no.uib.cipr.matrix.Matrix;

	public class FeaturePointRestriction : Restriction
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			featurePoint = new SimpleObjectProperty<FeaturePoint>(this, "featurePoint");
			geometricPrimitive = new SimpleObjectProperty<GeometricPrimitive>(this, "geometricPrimitive");
		}

		private ObjectProperty<FeaturePoint> featurePoint;
		private ObjectProperty<GeometricPrimitive> geometricPrimitive;

		public FeaturePointRestriction() : this(false, null, null)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public FeaturePointRestriction(bool indispensable, GeometricPrimitive geometricPrimitive, FeaturePoint featurePoint) : base(RestrictionType.FEATURE_POINT, indispensable)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}

			this.GeometricPrimitive = geometricPrimitive;
			this.FeaturePoint = featurePoint;
		}

		public virtual GeometricPrimitive GeometricPrimitive
		{
			set
			{
				this.geometricPrimitive.set(value);
			}
			get
			{
				return this.geometricPrimitive.get();
			}
		}


		public virtual ObjectProperty<GeometricPrimitive> geometricPrimitiveProperty()
		{
			return this.geometricPrimitive;
		}

		public virtual FeaturePoint FeaturePoint
		{
			set
			{
				this.featurePoint.set(value);
			}
			get
			{
				return this.featurePoint.get();
			}
		}


		public virtual ObjectProperty<FeaturePoint> featurePointProperty()
		{
			return this.featurePoint;
		}

		public override double Misclosure
		{
			get
			{
				return this.GeometricPrimitive.getMisclosure(this.FeaturePoint);
			}
		}

		public override void transposedJacobianElements(Matrix JrT)
		{
			ICollection<UnknownParameter> unknownParameters = this.GeometricPrimitive.getUnknownParameters();

			int rowIndex = this.Row;
			// store values temp. in a one row matrix
			DenseMatrix jx = new DenseMatrix(1, JrT.numRows());
			this.GeometricPrimitive.jacobianElements(this.FeaturePoint, jx, null, 0);

			// copy jx values to restriction matrix
			foreach (UnknownParameter unknownParameter in unknownParameters)
			{
				int column = unknownParameter.Column;
				if (column < 0)
				{
					continue;
				}
				JrT.set(column, rowIndex, jx.get(0, column));
			}
		}

		public override bool contains(object @object)
		{
			if (@object == null)
			{
				return false;
			}
			return this.FeaturePoint == @object || this.GeometricPrimitive == @object;
		}

		public override string toLaTex()
		{
			return "$f\\left(x, p\\right) = 0$";
		}
	}

}