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

namespace org.applied_geodesy.adjustment.geometry
{

	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Point = org.applied_geodesy.adjustment.geometry.point.Point;
	using Restriction = org.applied_geodesy.adjustment.geometry.restriction.Restriction;
	using org.applied_geodesy.util;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using ReadOnlyObjectProperty = javafx.beans.property.ReadOnlyObjectProperty;
	using ReadOnlyObjectWrapper = javafx.beans.property.ReadOnlyObjectWrapper;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using FXCollections = javafx.collections.FXCollections;
	using ListChangeListener = javafx.collections.ListChangeListener;
	using ObservableList = javafx.collections.ObservableList;
	using FilteredList = javafx.collections.transformation.FilteredList;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;

	public abstract class Feature : IEnumerable<GeometricPrimitive>, Geometrizable
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			estimateInitialGuess = new SimpleObjectProperty<bool>(this, "estimateInitialGuess", true);
			estimateCenterOfMass = new SimpleObjectProperty<bool>(this, "estimateCenterOfMass", true);
		}

		private class GeometricPrimitiveListChangeListener : ListChangeListener<GeometricPrimitive>
		{
			private readonly Feature outerInstance;

			public GeometricPrimitiveListChangeListener(Feature outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void onChanged<T1>(Change<T1> change) where T1 : GeometricPrimitive
			{
				if (outerInstance.unknownParameters == null)
				{
					return;
				}

				while (change.next())
				{
					if (change.wasRemoved())
					{
						foreach (GeometricPrimitive geometricPrimitive in change.getRemoved())
						{
							if (outerInstance.unknownParameters != null)
							{
								outerInstance.unknownParameters.removeAll(geometricPrimitive.UnknownParameters);
							}
							if (outerInstance.restrictions != null)
							{
								outerInstance.restrictions.removeAll(geometricPrimitive.Restrictions);
							}
						}
					}
					else if (change.wasAdded())
					{
						foreach (GeometricPrimitive geometricPrimitive in change.getAddedSubList())
						{
							if (outerInstance.unknownParameters != null)
							{
								outerInstance.unknownParameters.addAll(geometricPrimitive.UnknownParameters);
							}
							if (outerInstance.restrictions != null)
							{
								outerInstance.restrictions.addAll(geometricPrimitive.Restrictions);
							}
						}
					}
				}
			}
		}

		private ReadOnlyObjectProperty<bool> immutable;
		private ObservableUniqueList<GeometricPrimitive> geometricPrimitives = new ObservableUniqueList<GeometricPrimitive>();
		private ObservableUniqueList<UnknownParameter> unknownParameters = null;
		private ObservableUniqueList<Restriction> restrictions = null;
		private ObservableUniqueList<Restriction> postprocessingCalculations = new ObservableUniqueList<Restriction>();
		private ObjectProperty<bool> estimateInitialGuess;
		private ObjectProperty<bool> estimateCenterOfMass;
		private Point centerOfMass;

		internal Feature(bool immutable)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.immutable = new ReadOnlyObjectWrapper<bool>(this, "immutable", immutable);
			this.CenterOfMass = this.CenterOfMass;
			this.geometricPrimitives.addListener(new GeometricPrimitiveListChangeListener(this));
		}

		public virtual Point CenterOfMass
		{
			set
			{
				foreach (GeometricPrimitive geometricPrimitive in this.geometricPrimitives)
				{
					geometricPrimitive.CenterOfMass = value;
				}
				this.centerOfMass = value;
			}
			get
			{
				if (this.centerOfMass == null)
				{
					if (this.FeatureType == org.applied_geodesy.adjustment.geometry.FeatureType.CURVE)
					{
						this.centerOfMass = new Point("CENTER_OF_MASS", 0, 0);
					}
					else
					{
						this.centerOfMass = new Point("CENTER_OF_MASS", 0, 0, 0);
					}
				}
				return this.centerOfMass;
			}
		}

		public abstract FeatureType FeatureType {get;}


		public virtual void prepareIteration()
		{
		}

		public ObservableUniqueList<Restriction> PostProcessingCalculations
		{
			get
			{
				return this.postprocessingCalculations;
			}
		}

		public virtual ObjectProperty<bool> estimateCenterOfMassProperty()
		{
			return this.estimateCenterOfMass;
		}

		public virtual bool EstimateCenterOfMass
		{
			set
			{
				this.estimateCenterOfMass.set(value);
			}
			get
			{
				return this.estimateCenterOfMass.get();
			}
		}


		public virtual ObjectProperty<bool> estimateInitialGuessProperty()
		{
			return this.estimateInitialGuess;
		}

		public virtual bool EstimateInitialGuess
		{
			set
			{
				this.estimateInitialGuess.set(value);
			}
			get
			{
				return this.estimateInitialGuess.get();
			}
		}


		public virtual ReadOnlyObjectProperty<bool> immutableProperty()
		{
			return this.immutable;
		}

		public virtual bool Immutable
		{
			get
			{
				return this.immutable.get();
			}
		}

		public ObservableUniqueList<UnknownParameter> UnknownParameters
		{
			get
			{
				if (this.unknownParameters == null)
				{
					this.unknownParameters = new ObservableUniqueList<UnknownParameter>();
					foreach (GeometricPrimitive geometricPrimitive in this.geometricPrimitives)
					{
						this.unknownParameters.addAll(geometricPrimitive.UnknownParameters);
					}
				}
				return this.unknownParameters;
			}
		}

		public ObservableUniqueList<Restriction> Restrictions
		{
			get
			{
				if (this.restrictions == null)
				{
					this.restrictions = new ObservableUniqueList<Restriction>();
					foreach (GeometricPrimitive geometricPrimitive in this.geometricPrimitives)
					{
						this.restrictions.addAll(geometricPrimitive.Restrictions);
					}
				}
				return this.restrictions;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void add(GeometricPrimitive geometricPrimitive) throws IllegalArgumentException
		public virtual void add(GeometricPrimitive geometricPrimitive)
		{
			geometricPrimitive.CenterOfMass = this.centerOfMass;
			this.geometricPrimitives.add(geometricPrimitive);
		}

		public virtual IEnumerator<GeometricPrimitive> GetEnumerator()
		{
			return this.geometricPrimitives.GetEnumerator();
		}

		public virtual ObservableUniqueList<GeometricPrimitive> GeometricPrimitives
		{
			get
			{
				return this.geometricPrimitives;
			}
		}

		public virtual IList<FeaturePoint> FeaturePoints
		{
			get
			{
				ObservableList<FeaturePoint> nonUniquePoints = FXCollections.observableArrayList<FeaturePoint>();
				foreach (GeometricPrimitive geometry in this.geometricPrimitives)
				{
					nonUniquePoints.addAll(geometry.FeaturePoints);
				}
				FilteredList<FeaturePoint> enabledNonUniquePoints = new FilteredList<FeaturePoint>(nonUniquePoints);
				enabledNonUniquePoints.setPredicate((FeaturePoint featurePoint) =>
				{
						return featurePoint.isEnable();
				});
    
				// Unique point list
				return new List<FeaturePoint>(new LinkedHashSet<FeaturePoint>(enabledNonUniquePoints));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException;
		public abstract void deriveInitialGuess();

		public virtual void applyInitialGuess()
		{
			foreach (UnknownParameter unknownParameter in this.unknownParameters)
			{
				// set initial guess x <-- x0 because adjustment process works with x (not with x0)
				unknownParameter.Value = unknownParameter.Value0;
			}
		}

		public static Point deriveCenterOfMass(ICollection<FeaturePoint> featurePoints)
		{
			int nop = 0, dim = -1;
			double x0 = 0, y0 = 0, z0 = 0;
			foreach (FeaturePoint featurePoint in featurePoints)
			{
				if (!featurePoint.Enable)
				{
					continue;
				}

				nop++;
				x0 += featurePoint.X0;
				y0 += featurePoint.Y0;
				z0 += featurePoint.Z0;

				if (dim < 0)
				{
					dim = featurePoint.Dimension;
				}
				if (dim != featurePoint.Dimension)
				{
					throw new System.ArgumentException("Error, could not estimate center of mass because dimension of points is inconsistent, " + dim + " != " + featurePoint.Dimension);
				}
			}

			if (nop == 0)
			{
				throw new System.ArgumentException("Error, could not estimate center of mass because of an empty point list!");
			}

			x0 /= nop;
			y0 /= nop;
			z0 /= nop;

			if (dim == 2)
			{
				return new Point("CENTER_OF_MASS", x0, y0);
			}

			return new Point("CENTER_OF_MASS", x0, y0, z0);
		}
	}

}