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

namespace org.applied_geodesy.adjustment.network.approximation.bundle.transformation
{

	using Constant = org.applied_geodesy.adjustment.Constant;
	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using NormalEquationSystem = org.applied_geodesy.adjustment.NormalEquationSystem;
	using PointBundle = org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle;
	using Point = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point;
	using Point1D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point1D;
	using Point2D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D;
	using Point3D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point3D;

	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using MatrixNotSPDException = no.uib.cipr.matrix.MatrixNotSPDException;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;
	using Vector = no.uib.cipr.matrix.Vector;

	public abstract class BundleTransformation
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			change = new PropertyChangeSupport(this);
		}

		private PropertyChangeSupport change;
		public const double SQRT_EPS = 1.0E-5;
		private IDictionary<string, int> pointInSystemsCounter = new LinkedHashMap<string, int>();
		private ISet<string> outliers = new HashSet<string>();
		private EstimationStateType currentEstimationStatus = EstimationStateType.BUSY;
//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private int maxIteration = 50, numberOfUnknowns_Conflict = 0, numberOfObservations_Conflict = 0;

		private double maxDx = double.Epsilon;
		private double omega = 0.0, threshold = 15.0;
//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		internal bool interrupt_Conflict = false;
		private PointBundle targetSystem = null;
		private ISet<string> targetSystemPointIds = new HashSet<string>();
		private IList<PointBundle> sourceSystems = new List<PointBundle>();
		private IList<PointBundle> excludedSystems = new List<PointBundle>();

		public BundleTransformation(double threshold, IList<PointBundle> sourceSystems)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (sourceSystems.Count < 1)
			{
				throw new System.ArgumentException(this.GetType().Name + " Fehler, mind. ein System wird benoetigt! " + sourceSystems.Count);
			}
			this.Threshold = threshold;
			this.initSystems(sourceSystems);
			this.init();
		}

		public BundleTransformation(double threshold, PointBundle sourceSystem, PointBundle targetSystem)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.Threshold = threshold;
			this.targetSystem = targetSystem;
			this.sourceSystems.Add(sourceSystem);
			this.init();
		}

		public BundleTransformation(double threshold, IList<PointBundle> sourceSystems, PointBundle targetSystem)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.Threshold = threshold;
			this.targetSystem = targetSystem;
			this.sourceSystems = sourceSystems;
			this.init();
		}

		public abstract Transformation getSimpleTransformationModel(PointBundle b1, PointBundle b2);

		public abstract Transformation getSimpleTransformationModel(TransformationParameterSet transParameter);

		private void initSystems(IList<PointBundle> sourceSystems)
		{
			PointBundle bundle1 = null, bundle2 = null;
			int numIdentPoints = 0;
			for (int i = 0; i < sourceSystems.Count; i++)
			{
				PointBundle b1 = sourceSystems[i];

				for (int j = i + 1; j < sourceSystems.Count; j++)
				{
					PointBundle b2 = sourceSystems[j];
					Transformation t = this.getSimpleTransformationModel(b1, b2);

					if (t != null && t.numberOfIdenticalPoints() >= t.numberOfRequiredPoints() && t.numberOfIdenticalPoints() >= numIdentPoints)
					{
						numIdentPoints = t.numberOfIdenticalPoints();
						bundle1 = b1;
						bundle2 = b2;
					}
				}
			}
			if (bundle1 != null && bundle2 != null)
			{
				if (bundle1.size() > bundle2.size())
				{
					this.targetSystem = bundle1;
					sourceSystems.Remove(bundle1);
				}
				else
				{
					this.targetSystem = bundle2;
					sourceSystems.Remove(bundle2);
				}
			}
			else
			{
				this.targetSystem = sourceSystems[0];
				sourceSystems.Remove(sourceSystems[0]);
			}
			this.sourceSystems = sourceSystems;
		}

		public int Dimension
		{
			get
			{
				return this.targetSystem.Dimension;
			}
		}

		private void calculateTargetSystemApproximatedCoordinates()
		{
			IList<PointBundle> pointSystems = new List<PointBundle>();
			int dim = this.Dimension;
			// Fuege alle Quellsysteme hinzu
			foreach (PointBundle sourceSystem in this.sourceSystems)
			{
				PointBundle pointBundleDeepCopy = new PointBundle(dim, sourceSystem.Intersection);
				for (int i = 0; i < sourceSystem.size(); i++)
				{
					Point p = sourceSystem.get(i);
					Point cp = null;
					if (dim == 1)
					{
						cp = new Point1D(p.Name, p.Z);
					}
					else if (dim == 2)
					{
						cp = new Point2D(p.Name, p.X, p.Y);
					}
					else if (dim == 3)
					{
						cp = new Point3D(p.Name, p.X, p.Y, p.Z);
					}
					if (cp != null)
					{
						pointBundleDeepCopy.addPoint(cp);
					}
				}
				if (pointBundleDeepCopy.size() > 0)
				{
					pointSystems.Add(pointBundleDeepCopy);
				}
			}
			// Fuege Zielsystem hinzu
			PointBundle pointBundleDeepCopy = new PointBundle(dim, this.targetSystem.Intersection);
			for (int i = 0; i < this.targetSystem.size(); i++)
			{
				Point p = this.targetSystem.get(i);
				string id = p.Name;
				this.targetSystemPointIds.Add(id);
				if (this.pointInSystemsCounter.ContainsKey(id))
				{
					this.pointInSystemsCounter[id] = pointInSystemsCounter[id] + 1;
				}
				else
				{
					this.pointInSystemsCounter[id] = 1;
				}

				Point cp = null;
				if (dim == 1)
				{
					cp = new Point1D(id, p.Z);
				}
				else if (dim == 2)
				{
					cp = new Point2D(id, p.X, p.Y);
				}
				else if (dim == 3)
				{
					cp = new Point3D(id, p.X, p.Y, p.Z);
				}
				if (cp != null)
				{
					pointBundleDeepCopy.addPoint(cp);
				}
			}
			if (pointBundleDeepCopy.size() > 0)
			{
				pointSystems.Add(pointBundleDeepCopy);
			}

			int numberOfIdenticalPoints = 0;
			bool transformedSystems = true;
			while (transformedSystems && pointSystems.Count > 0)
			{
				PointBundle maxSRC = null;
				PointBundle maxTRG = null;
				numberOfIdenticalPoints = 0;
				for (int i = 0; i < pointSystems.Count; i++)
				{
					PointBundle bundleOne = pointSystems[i];

					for (int j = i + 1; j < pointSystems.Count; j++)
					{
						int numIdents = 0;
						PointBundle bundleTwo = pointSystems[j];
						// Zaehle die identischen Punkte
						for (int o = 0; o < bundleOne.size(); o++)
						{
							Point pOne = bundleOne.get(o);
							Point pTwo = bundleTwo.get(pOne.Name);
							if (pTwo != null)
							{
								numIdents++;
							}
						}
						if (numIdents > numberOfIdenticalPoints)
						{
							maxSRC = bundleOne;
							maxTRG = bundleTwo;
							numberOfIdenticalPoints = numIdents;
						}
					}
				}
				if (maxSRC != null && maxTRG != null)
				{
					bool src2trg = false;

					if (!maxSRC.Intersection && maxTRG.Intersection)
					{
						src2trg = true;
					}
					else if (maxSRC.Intersection && !maxTRG.Intersection)
					{
						src2trg = false;
					}
					else if (maxSRC.size() < maxTRG.size())
					{
						src2trg = false;
					}
					else
					{
						src2trg = true;
					}

					// Tausche Systeme
					if (src2trg)
					{
						PointBundle tmpBundle = maxTRG;
						maxTRG = maxSRC;
						maxSRC = tmpBundle;
					}

					Transformation trans = this.getSimpleTransformationModel(maxSRC, maxTRG);

					// Ist Transformation durchfuehrbar und erfolgreich ?
					if (trans != null && trans.numberOfIdenticalPoints() >= trans.numberOfRequiredPoints())
					{
						// Halte Massstab fest auf m=1 bei Naeherungswertbestimmung
						trans.setFixedParameter(TransformationParameterType.SCALE, !maxSRC.Intersection && !maxTRG.Intersection);

						// Transformierte per L2Norm und pruefe, ob Ausreisser drin sind, wenn ja fuehre LMS-Bestimmung durch
						if (trans.transformL2Norm() && trans.Omega <= this.threshold * Math.Sqrt(BundleTransformation.SQRT_EPS) || trans.transformLMS())
						{
							for (int o = 0; o < maxSRC.size(); o++)
							{
								Point src = maxSRC.get(o);
								Point trg = maxTRG.get(src.Name);
								if (trg == null)
								{
									trg = trans.transformPoint2TargetSystem(src);
									maxTRG.addPoint(trg);
								}
							}
						}
					}
					else
					{
						transformedSystems = false;
					}
					// entferne Quellsystem
					pointSystems.Remove(maxSRC);
				}
				else
				{
					transformedSystems = false;
				}
			}

			// Suche das maximale System in pointSystems;
			// tritt nur ein, wenn nicht alle Systeme transformierbar sind!!!!
			PointBundle maxScrSystem = pointSystems[0];
			for (int i = 1; i < pointSystems.Count; i++)
			{
				if (pointSystems[i].size() > maxScrSystem.size())
				{
					maxScrSystem = pointSystems[i];
				}
			}
			// Transformiere Punkte ins Zielsystem als erste Naeherung
			Transformation trans = this.getSimpleTransformationModel(maxScrSystem, this.targetSystem);

			if (trans != null && trans.numberOfIdenticalPoints() >= trans.numberOfRequiredPoints())
			{
				trans.setFixedParameter(TransformationParameterType.SCALE, !maxScrSystem.Intersection && !this.targetSystem.Intersection);
				// Transformierte per L2Norm und pruefe, ob Ausreißer drin sind, wenn ja fuehre LMS-Bestimmung durch
				if (trans.transformL2Norm() && trans.Omega <= this.threshold * Math.Sqrt(BundleTransformation.SQRT_EPS) || trans.transformLMS())
				{
					for (int o = 0; o < maxScrSystem.size(); o++)
					{
						Point src = maxScrSystem.get(o);
						Point trg = this.targetSystem.get(src.Name);
						if (trg == null)
						{
							trg = trans.transformPoint2TargetSystem(src);
							this.targetSystem.addPoint(trg);
						}
					}
				}
			}
		}

		private void calculateTransformationParameterApproximatedValues()
		{
			IList<PointBundle> srcPointSystems = new List<PointBundle>();
			foreach (PointBundle sourceSystem in this.sourceSystems)
			{
				srcPointSystems.Add(sourceSystem);

				for (int i = 0; i < sourceSystem.size(); i++)
				{
					Point p = sourceSystem.get(i);
					p.ColInJacobiMatrix = -1;
					p.RowInJacobiMatrix = -1;
					string id = p.Name;
					if (this.pointInSystemsCounter.ContainsKey(id))
					{
						this.pointInSystemsCounter[id] = this.pointInSystemsCounter[id] + 1;
					}
					else
					{
						this.pointInSystemsCounter[id] = 1;
					}
				}
			}

			for (int i = 0; i < this.targetSystem.size(); i++)
			{
				Point p = this.targetSystem.get(i);
				p.ColInJacobiMatrix = -1;
				p.RowInJacobiMatrix = -1;
	// 			Wird nun bereits in der Methode calculateTargetSystemApproximatedCoordinates() zugewiesen fuer das Zielsystem
	//			String id = p.getName();
	//			if (this.pointInSystemsCounter.containsKey(id))
	//				this.pointInSystemsCounter.put(id, pointInSystemsCounter.get(id) + 1);
	//			else
	//				this.pointInSystemsCounter.put(id, 1);
			}

			foreach (PointBundle sourceSystem in srcPointSystems)
			{
				// Transformiere System
				// --> Naeherungswertbestimmung der Trafo-Parameter
				// Sollte System nicht transformaierbar, entferne es aus Auswahl
				if (sourceSystem == null || !this.setApproximatedValues(sourceSystem))
				{
					Console.Error.WriteLine(this.GetType().Name + " Fehler, nicht genuegend identische\nPunkte vorhanden oder System nicht transformierbar!");
					if (sourceSystem != null)
					{
						this.sourceSystems.Remove(sourceSystem);
						this.excludedSystems.Add(sourceSystem);
					}
				}
			}
			((List<PointBundle>)this.sourceSystems).TrimExcess();
		}

		private bool setApproximatedValues(PointBundle sourceSystem)
		{
			int dim = this.Dimension;
			Transformation trans = this.getSimpleTransformationModel(this.targetSystem, sourceSystem);
			// Ist Transformation durchfuehrbar und erfolgreich
			if (trans == null || trans.numberOfIdenticalPoints() < trans.numberOfRequiredPoints())
			{
				return false;
			}

			// Halte Massstab fest auf m=1 bei Naeherungswertbestimmung
			trans.setFixedParameter(TransformationParameterType.SCALE, !sourceSystem.Intersection && !this.targetSystem.Intersection);
			// Transformierte per L2Norm und pruefe, ob Ausreißer drin sind, wenn ja fuehre LMS-Bestimmung durch
			if (trans.transformL2Norm() && trans.Omega <= this.threshold * Math.Sqrt(BundleTransformation.SQRT_EPS) || trans.transformLMS())
			{
				this.omega = Math.Max(this.omega, trans.Omega);
				sourceSystem.TransformationParameterSet = trans.TransformationParameterSet;

				for (int i = 0; i < sourceSystem.size(); i++)
				{
					Point pointSource = sourceSystem.get(i);

					if (this.pointInSystemsCounter[pointSource.Name] <= 1)
					{
						continue;
					}

					Point pointTarget = this.targetSystem.get(pointSource.Name);

					if (pointTarget == null)
					{
						pointTarget = trans.transformPoint2SourceSystem(pointSource);
						pointTarget.ColInJacobiMatrix = this.numberOfUnknowns_Conflict;
						pointSource.ColInJacobiMatrix = this.numberOfUnknowns_Conflict;
						this.targetSystem.addPoint(pointTarget);
						this.numberOfUnknowns_Conflict += dim;
					}
					else if (!this.targetSystemPointIds.Contains(pointSource.Name) && pointTarget.ColInJacobiMatrix < 0)
					{
						pointTarget.ColInJacobiMatrix = this.numberOfUnknowns_Conflict;
						pointSource.ColInJacobiMatrix = this.numberOfUnknowns_Conflict;
						this.numberOfUnknowns_Conflict += dim;
					}
					else
					{
						pointSource.ColInJacobiMatrix = pointTarget.ColInJacobiMatrix;
					}
					pointSource.RowInJacobiMatrix = this.numberOfObservations_Conflict;
					this.numberOfObservations_Conflict += dim;
				}
				return true;
			}
			return false;
		}

		private void init()
		{
			this.numberOfUnknowns_Conflict = 0;
			this.numberOfObservations_Conflict = 0;
			int dim = this.Dimension;

			//this.calculateApproximatedValues();

			// Bestimme Näherungskoordinatenn im Zielsystem 
			this.calculateTargetSystemApproximatedCoordinates();

			// Bestimme genaeherte Transformationsparameter
			this.calculateTransformationParameterApproximatedValues();

			// Ab hier nur noch transformierbare Systeme vorhanden
			foreach (PointBundle sourceSystem in this.sourceSystems)
			{
				// 	Hinzufuegen der Transformationsparameter als Unbekannte
				TransformationParameterSet transParameter = sourceSystem.TransformationParameterSet;

				// Keinen Netzmassstab für Gesamtausgleichung bestimmen - sofern es kein Streckenfreies System ist (Vorwaertsschnitt)
				if (sourceSystem.Intersection)
				{
					TransformationParameter m = transParameter.get(TransformationParameterType.SCALE);
					m.ColInJacobiMatrix = this.numberOfUnknowns_Conflict++;
				}

				if (dim != 1)
				{
					TransformationParameter rz = transParameter.get(TransformationParameterType.ROTATION_Z);
					rz.ColInJacobiMatrix = this.numberOfUnknowns_Conflict++;

					TransformationParameter tx = transParameter.get(TransformationParameterType.TRANSLATION_X);
					tx.ColInJacobiMatrix = this.numberOfUnknowns_Conflict++;

					TransformationParameter ty = transParameter.get(TransformationParameterType.TRANSLATION_Y);
					ty.ColInJacobiMatrix = this.numberOfUnknowns_Conflict++;
				}

				if (dim == 3)
				{
					TransformationParameter rx = transParameter.get(TransformationParameterType.ROTATION_X);
					rx.ColInJacobiMatrix = this.numberOfUnknowns_Conflict++;

					TransformationParameter ry = transParameter.get(TransformationParameterType.ROTATION_Y);
					ry.ColInJacobiMatrix = this.numberOfUnknowns_Conflict++;
				}

				if (dim != 2)
				{
					TransformationParameter tz = transParameter.get(TransformationParameterType.TRANSLATION_Z);
					tz.ColInJacobiMatrix = this.numberOfUnknowns_Conflict++;
				}
			}
		}

		public virtual EstimationStateType estimateModel()
		{
			bool isEstimated = false, isConverge = true;
			this.currentEstimationStatus = EstimationStateType.BUSY;
			this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);

			int runs = this.maxIteration - 1;
			try
			{
				do
				{
					this.currentEstimationStatus = EstimationStateType.ITERATE;
					this.change.firePropertyChange(this.currentEstimationStatus.ToString(), this.maxIteration, this.maxIteration - runs);

					if (this.interrupt_Conflict)
					{
						this.currentEstimationStatus = EstimationStateType.INTERRUPT;
						this.interrupt_Conflict = false;
						this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
						return this.currentEstimationStatus;
					}

					Vector dx = new DenseVector(this.numberOfUnknowns_Conflict);
					//	estimateCompleteModel = isEstimate;
					try
					{
						// Gleichungssystem erzeugen
						NormalEquationSystem NES = this.createNormalEquationSystem();
						if (this.interrupt_Conflict || NES == null)
						{
							this.currentEstimationStatus = EstimationStateType.INTERRUPT;
							this.interrupt_Conflict = false;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							return this.currentEstimationStatus;
						}

						UpperSymmPackMatrix N = NES.Matrix;
						DenseVector n = NES.Vector;

						if (N == null || n == null || this.numberOfObservations() < this.Dimension || this.numberOfUnknowns() == 0)
						{
							this.currentEstimationStatus = EstimationStateType.NOT_INITIALISED;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							return this.currentEstimationStatus;
						}

						this.maxDx = double.Epsilon;

						// Loese Nx=n und ueberschreibe n durch die Loesung x
						MathExtension.solve(N, n, false);
						dx = n;
					}
					catch (Exception e) when (e is MatrixSingularException || e is MatrixNotSPDException || e is System.ArgumentException || e is System.IndexOutOfRangeException)
					{
						e.printStackTrace();
						this.currentEstimationStatus = EstimationStateType.SINGULAR_MATRIX;
						this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
						return this.currentEstimationStatus;
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
						this.currentEstimationStatus = EstimationStateType.INTERRUPT;
						this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
						return this.currentEstimationStatus;
					}

					this.updateUnknownParameters(dx);

					this.currentEstimationStatus = EstimationStateType.CONVERGENCE;
					this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxDx);

					if (this.maxDx <= BundleTransformation.SQRT_EPS)
					{
						isEstimated = true;
						runs--;
					}
					else if (runs-- <= 1)
					{
						isConverge = false;
						isEstimated = true;
					}
				} while (!isEstimated);

			}
			catch (System.OutOfMemoryException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				this.currentEstimationStatus = EstimationStateType.OUT_OF_MEMORY;
				this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
				return this.currentEstimationStatus;
			}

			if (!isConverge)
			{
				this.currentEstimationStatus = EstimationStateType.NO_CONVERGENCE;
				this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxDx);
			}

			// Bestimme die Punkte, die nur in einem System vorhanden waren.
			foreach (PointBundle sourceSystem in this.sourceSystems)
			{
				Transformation trans = this.getSimpleTransformationModel(sourceSystem.TransformationParameterSet);

				for (int i = 0; i < sourceSystem.size(); i++)
				{
					Point pointSource = sourceSystem.get(i);
					Point pointTarget = this.targetSystem.get(pointSource.Name);
					if (pointTarget == null && trans != null)
					{
						pointTarget = trans.transformPoint2SourceSystem(pointSource);
						this.targetSystem.addPoint(pointTarget);
					}
				}
			}
			if (this.currentEstimationStatus.getId() == EstimationStateType.BUSY.getId())
			{
				this.currentEstimationStatus = EstimationStateType.ERROR_FREE_ESTIMATION;
				this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxDx);
			}
			return this.currentEstimationStatus;
		}

		private void updateUnknownParameters(Vector dx)
		{
			foreach (PointBundle sourceSystem in this.sourceSystems)
			{
				if (this.interrupt_Conflict)
				{
					return;
				}

				// 	Transformationsparameter des lokalen Systems
				TransformationParameterSet parameters = sourceSystem.TransformationParameterSet;
				TransformationParameter m = parameters.get(TransformationParameterType.SCALE);

				TransformationParameter rx = parameters.get(TransformationParameterType.ROTATION_X);
				TransformationParameter ry = parameters.get(TransformationParameterType.ROTATION_Y);
				TransformationParameter rz = parameters.get(TransformationParameterType.ROTATION_Z);

				TransformationParameter tx = parameters.get(TransformationParameterType.TRANSLATION_X);
				TransformationParameter ty = parameters.get(TransformationParameterType.TRANSLATION_Y);
				TransformationParameter tz = parameters.get(TransformationParameterType.TRANSLATION_Z);

				if (!m.Fixed)
				{
					int col = m.ColInJacobiMatrix;
					double oldValue = m.Value;
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					m.Value = oldValue + dx.get(col);
				}
				if (!tx.Fixed)
				{
					int col = tx.ColInJacobiMatrix;
					double oldValue = tx.Value;
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					tx.Value = oldValue + dx.get(col);
				}

				if (!ty.Fixed)
				{
					int col = ty.ColInJacobiMatrix;
					double oldValue = ty.Value;
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					ty.Value = oldValue + dx.get(col);
				}

				if (!tz.Fixed)
				{
					int col = tz.ColInJacobiMatrix;
					double oldValue = tz.Value;
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					tz.Value = oldValue + dx.get(col);
				}

				if (!rx.Fixed)
				{
					int col = rx.ColInJacobiMatrix;
					double oldValue = rx.Value;
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					rx.Value = MathExtension.MOD(oldValue + dx.get(col), 2 * Math.PI);
				}

				if (!ry.Fixed)
				{
					int col = ry.ColInJacobiMatrix;
					double oldValue = ry.Value;
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					ry.Value = MathExtension.MOD(oldValue + dx.get(col), 2 * Math.PI);
				}

				if (!rz.Fixed)
				{
					int col = rz.ColInJacobiMatrix;
					double oldValue = rz.Value;
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					rz.Value = MathExtension.MOD(oldValue + dx.get(col), 2 * Math.PI);
				}
			}
			int dim = this.Dimension;
			for (int i = 0; i < this.targetSystem.size(); i++)
			{
				if (this.interrupt_Conflict)
				{
					return;
				}

				Point point = this.targetSystem.get(i);
				int col = point.ColInJacobiMatrix;
				if (col < 0)
				{
					continue;
				}

				if (dim != 1)
				{
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					point.X = point.X + dx.get(col++);
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					point.Y = point.Y + dx.get(col++);
				}
				if (dim != 2)
				{
					this.maxDx = Math.Max(Math.Abs(dx.get(col)), this.maxDx);
					point.Z = point.Z + dx.get(col);
				}
			}
		}

		public virtual IList<PointBundle> ExcludedSystems
		{
			get
			{
				return this.excludedSystems;
			}
		}

		public int numberOfObservations()
		{
			return this.numberOfObservations_Conflict;
		}

		public int numberOfUnknowns()
		{
			return this.numberOfUnknowns_Conflict;
		}

		protected internal abstract NormalEquationSystem createNormalEquationSystem();

		public virtual PointBundle TargetSystem
		{
			get
			{
				return this.targetSystem;
			}
		}

		public virtual IList<PointBundle> SourceSystems
		{
			get
			{
				return this.sourceSystems;
			}
		}

		private double Threshold
		{
			set
			{
				this.threshold = value > 1.0 ? value : 1.0;
			}
		}

		protected internal virtual double ScaleEstimate
		{
			get
			{
				double SQRT_EPS = Math.Sqrt(Constant.EPS);
				double omega = this.omega > SQRT_EPS?this.omega:1.0;
				int dof = this.numberOfObservations() - this.numberOfUnknowns();
				if (dof > 0)
				{
					return this.threshold * 1.4826022185056 * (1.0 + 5.0 / dof) * Math.Sqrt(omega);
				}
				return 1.0;
			}
		}

		public virtual ISet<string> Outliers
		{
			get
			{
				return this.outliers;
			}
		}

		protected internal virtual void removeOutlierPoint(Point point)
		{
			string pointId = point.Name;
			if (this.outliers.Contains(pointId))
			{
				this.outliers.remove(pointId);
			}
		}

		protected internal virtual void addOutlierPoint(Point point)
		{
			string pointId = point.Name;
			if (!this.outliers.Contains(pointId))
			{
				this.outliers.Add(pointId);
			}
		}

		public virtual void interrupt()
		{
			this.interrupt_Conflict = true;
		}

		public virtual void addPropertyChangeListener(PropertyChangeListener listener)
		{
			this.change.addPropertyChangeListener(listener);
		}

		public virtual void removePropertyChangeListener(PropertyChangeListener listener)
		{
			this.change.removePropertyChangeListener(listener);
		}
	}

}