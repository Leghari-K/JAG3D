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

namespace org.applied_geodesy.adjustment.network.approximation
{

	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using PointBundle = org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle;
	using Point = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point;
	using BundleTransformation = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.BundleTransformation;
	using BundleTransformation1D = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.BundleTransformation1D;
	using BundleTransformation2D = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.BundleTransformation2D;
	using Transformation = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.Transformation;

	public class AutomatedApproximationAdjustment : PropertyChangeListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			change = new PropertyChangeSupport(this);
		}

		private PropertyChangeSupport change;
		private IList<PointBundle> systems;
		private BundleTransformation bundleTransformation = null;
		private EstimationStateType currentEstimationStatus = EstimationStateType.BUSY;
		private int systemsCounter = 0;
		private PointBundle targetSystem;
		private double threshold = 15.0;
		private bool estimateDatumPoints = false, freeNetwork = true;
		private ISet<string> outliers = new HashSet<string>();

		public AutomatedApproximationAdjustment(PointBundle targetSystem, IList<Point> points)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.systems = this.getBundles(points);
			this.targetSystem = targetSystem;
		}

		public virtual void addSystems(IList<PointBundle> bundles)
		{
			((List<PointBundle>)this.systems).AddRange(bundles);
		}

		public virtual ISet<string> Outliers
		{
			get
			{
				return this.outliers;
			}
		}

		public virtual bool FreeNetwork
		{
			set
			{
				this.freeNetwork = value;
			}
		}

		public virtual bool EstimateDatumPoints
		{
			set
			{
				this.estimateDatumPoints = value;
			}
		}

		public virtual PointBundle TargetSystem
		{
			get
			{
				return this.targetSystem;
			}
		}

		public virtual EstimationStateType CurrentEstimationStatus
		{
			get
			{
				return this.currentEstimationStatus;
			}
		}

		public virtual int SubSystemsCounter
		{
			get
			{
				return this.systemsCounter;
			}
		}

		private IList<PointBundle> getBundles(IList<Point> points)
		{
			IList<PointBundle> systems = new List<PointBundle>();
			foreach (Point p in points)
			{
				IList<PointBundle> bundles = p.PointBundles;
				foreach (PointBundle bundle in bundles)
				{
					systems.Add(bundle);
				}
			}
			return systems;
		}

		public virtual EstimationStateType estimateApproximatedValues()
		{
			EstimationStateType status = EstimationStateType.BUSY;
			this.bundleTransformation = null;

			if (this.systems != null && this.systems.Count > 0)
			{
				if (this.targetSystem != null && (!this.freeNetwork || (this.freeNetwork && !this.estimateDatumPoints)))
				{
					this.systems.Add(this.targetSystem);
				}

				this.systemsCounter = 0;

				if (this.systems == null || this.systems.Count < 1)
				{
					return EstimationStateType.NOT_INITIALISED;
				}
				else if (this.systems.Count == 1 && this.targetSystem == null)
				{
					this.targetSystem = this.systems[0];
					return EstimationStateType.ERROR_FREE_ESTIMATION;
				}
				int dim = this.systems[0].Dimension;

				while (this.systemsCounter != this.systems.Count && this.systems.Count > 1)
				{
					this.systemsCounter = this.systems.Count;
					if (dim == 1)
					{
						this.bundleTransformation = new BundleTransformation1D(this.threshold, this.systems);
					}
					else if (dim == 2)
					{
						this.bundleTransformation = new BundleTransformation2D(this.threshold, this.systems);
					}
					else
					{
						return EstimationStateType.NOT_INITIALISED;
					}

					this.bundleTransformation.addPropertyChangeListener(this);
					status = this.bundleTransformation.estimateModel();

					this.systems = this.bundleTransformation.ExcludedSystems;
					this.systems.Add(this.bundleTransformation.TargetSystem);

					ISet<string> outliers = this.bundleTransformation.Outliers;
					if (outliers.Count > 0)
					{
						this.outliers.addAll(outliers);
					}
				}
				this.systemsCounter = this.systems.Count;

				if (this.bundleTransformation != null && this.systems.Count > 0)
				{
					// Wenn es eine freie AGL ist,
					// nimm das groesste verbleibende System als 
					// finales Zielsystem
					if (this.freeNetwork && this.estimateDatumPoints || this.targetSystem == null)
					{
						this.targetSystem = this.LargestPointBundle; //systems.get(0);
					}
					// Wenn Anschluss vorgegeben, dann
					// transformiere auf FP-Feld
					else
					{
						Transformation trans = this.bundleTransformation.getSimpleTransformationModel(this.LargestPointBundle, this.targetSystem);

						if (trans != null && trans.transformL2Norm())
						{
							this.targetSystem = trans.TransformdPoints;
						}
						else
						{
							this.targetSystem = null;
						}
					}
				}

				if (this.bundleTransformation != null)
				{
					this.bundleTransformation.removePropertyChangeListener(this);
				}
				this.bundleTransformation = null;

				return status;
			}
			else
			{
				status = EstimationStateType.ERROR_FREE_ESTIMATION;
			}

			return status;
		}

		public virtual PointBundle LargestPointBundle
		{
			get
			{
				PointBundle maxBundle = this.systems[0];
				foreach (PointBundle bundle in this.systems)
				{
					if (maxBundle.size() < bundle.size())
					{
						maxBundle = bundle;
					}
				}
				return maxBundle;
			}
		}

		public virtual IList<PointBundle> Systems
		{
			get
			{
				return this.systems;
			}
		}

		public virtual void interrupt()
		{
			if (this.bundleTransformation != null)
			{
				this.bundleTransformation.interrupt();
			}
		}

		public virtual double Threshold
		{
			set
			{
				this.threshold = value >= 1.0 ? value:1.0;
			}
			get
			{
				return this.threshold;
			}
		}


		public virtual void addPropertyChangeListener(PropertyChangeListener listener)
		{
			this.change.addPropertyChangeListener(listener);
		}

		public virtual void removePropertyChangeListener(PropertyChangeListener listener)
		{
			this.change.removePropertyChangeListener(listener);
		}

		public override void propertyChange(PropertyChangeEvent @event)
		{
			this.change.firePropertyChange(@event);
		}
	}
}