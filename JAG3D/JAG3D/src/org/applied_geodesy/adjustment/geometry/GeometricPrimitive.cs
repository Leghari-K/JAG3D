using System.Collections.Generic;
using System.Linq;

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

namespace org.applied_geodesy.adjustment.geometry
{

	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Point = org.applied_geodesy.adjustment.geometry.point.Point;
	using Restriction = org.applied_geodesy.adjustment.geometry.restriction.Restriction;
	using org.applied_geodesy.util;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using ReadOnlyObjectProperty = javafx.beans.property.ReadOnlyObjectProperty;
	using ReadOnlyObjectWrapper = javafx.beans.property.ReadOnlyObjectWrapper;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using ListChangeListener = javafx.collections.ListChangeListener;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public abstract class GeometricPrimitive : IEnumerable<FeaturePoint>, Geometrizable
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			name = new SimpleObjectProperty<string>(this, "name");
		}


		private class FeaturePointListChangeListener : ListChangeListener<FeaturePoint>
		{
			private readonly GeometricPrimitive outerInstance;

			public FeaturePointListChangeListener(GeometricPrimitive outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void onChanged<T1>(Change<T1> change) where T1 : org.applied_geodesy.adjustment.geometry.point.FeaturePoint
			{
				while (change.next())
				{
					if (change.wasRemoved())
					{
						foreach (FeaturePoint featurePoint in change.getRemoved())
						{
							featurePoint.remove(outerInstance);
							featurePoint.reset();
						}
					}
					else if (change.wasAdded())
					{
						foreach (FeaturePoint featurePoint in change.getAddedSubList())
						{
							featurePoint.add(outerInstance);
							featurePoint.reset();
						}
					}
				}
			}
		}

		//private ObservableList<FeaturePoint> featurePointList = FXCollections.observableArrayList();
		private ObservableUniqueList<FeaturePoint> featurePointList = new ObservableUniqueList<FeaturePoint>();
		private Point centerOfMass;
		private ObjectProperty<string> name;
		private static int ID_CNT = 0;
		private ReadOnlyObjectProperty<int> id;

		public abstract ICollection<UnknownParameter> UnknownParameters {get;}

		public abstract PrimitiveType PrimitiveType {get;}

		public abstract UnknownParameter getUnknownParameter(ParameterType parameterType);

		public virtual bool contains(object @object)
		{
			return this.UnknownParameters.Contains(@object);
		}

		public abstract void reverseCenterOfMass(UpperSymmPackMatrix Dp);

		public abstract void jacobianElements(FeaturePoint point, Matrix Jx, Matrix Jv, int rowIndex);

		public abstract double getMisclosure(FeaturePoint point);

		public abstract int Dimension {get;}

		public GeometricPrimitive()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.id = new ReadOnlyObjectWrapper<int>(this, "id", ID_CNT++);
			this.featurePointList.addListener(new FeaturePointListChangeListener(this));
		}

		public int Id
		{
			get
			{
				return this.id.get();
			}
		}

		public virtual ReadOnlyObjectProperty<int> idProperty()
		{
			return this.id;
		}

		public virtual ObservableUniqueList<FeaturePoint> FeaturePoints
		{
			get
			{
				return this.featurePointList;
			}
		}

		public virtual Point CenterOfMass
		{
			set
			{
				this.centerOfMass = value;
			}
			get
			{
				if (this.centerOfMass == null)
				{
					if (this.Dimension == 2)
					{
						this.centerOfMass = new Point("CENTER_OF_MASS", 0, 0);
					}
					else if (this.Dimension == 3)
					{
						this.centerOfMass = new Point("CENTER_OF_MASS", 0, 0, 0);
					}
				}
				return this.centerOfMass;
			}
		}


		public virtual ICollection<Restriction> Restrictions
		{
			get
			{
				return Enumerable.Empty<Restriction>();
			}
		}

		public virtual int NumberOfPoints
		{
			get
			{
				return this.featurePointList.size();
			}
		}

		public virtual IEnumerator<FeaturePoint> GetEnumerator()
		{
			return this.featurePointList.GetEnumerator();
		}

		public virtual ObjectProperty<string> nameProperty()
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


		public abstract string toLaTex();

		public override string ToString()
		{
			return this.Name;
		}
	}

}