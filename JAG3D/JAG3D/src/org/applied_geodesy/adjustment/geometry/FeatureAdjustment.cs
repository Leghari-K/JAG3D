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

namespace org.applied_geodesy.adjustment.geometry
{

	using Constant = org.applied_geodesy.adjustment.Constant;
	using DefaultValue = org.applied_geodesy.adjustment.DefaultValue;
	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using EstimationType = org.applied_geodesy.adjustment.EstimationType;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using NormalEquationSystem = org.applied_geodesy.adjustment.NormalEquationSystem;
	using UnscentedTransformationParameter = org.applied_geodesy.adjustment.UnscentedTransformationParameter;
	using FeatureEventType = org.applied_geodesy.adjustment.geometry.FeatureEvent.FeatureEventType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using Point = org.applied_geodesy.adjustment.geometry.point.Point;
	using Restriction = org.applied_geodesy.adjustment.geometry.restriction.Restriction;
	using BaardaMethodTestStatistic = org.applied_geodesy.adjustment.statistic.BaardaMethodTestStatistic;
	using SidakTestStatistic = org.applied_geodesy.adjustment.statistic.SidakTestStatistic;
	using TestStatisticDefinition = org.applied_geodesy.adjustment.statistic.TestStatisticDefinition;
	using TestStatisticParameterSet = org.applied_geodesy.adjustment.statistic.TestStatisticParameterSet;
	using TestStatisticParameters = org.applied_geodesy.adjustment.statistic.TestStatisticParameters;
	using TestStatistic = org.applied_geodesy.adjustment.statistic.TestStatistic;
	using TestStatisticType = org.applied_geodesy.adjustment.statistic.TestStatisticType;
	using UnadjustedTestStatitic = org.applied_geodesy.adjustment.statistic.UnadjustedTestStatitic;

	using FXCollections = javafx.collections.FXCollections;
	using FilteredList = javafx.collections.transformation.FilteredList;
	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using Matrices = no.uib.cipr.matrix.Matrices;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixNotSPDException = no.uib.cipr.matrix.MatrixNotSPDException;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using Reason = no.uib.cipr.matrix.NotConvergedException.Reason;
	using UpperSymmBandMatrix = no.uib.cipr.matrix.UpperSymmBandMatrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;
	using UpperTriangPackMatrix = no.uib.cipr.matrix.UpperTriangPackMatrix;
	using Vector = no.uib.cipr.matrix.Vector;

	public class FeatureAdjustment
	{
		private bool InstanceFieldsInitialized = false;

		public FeatureAdjustment()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			change = new PropertyChangeSupport(this);
		}

		private PropertyChangeSupport change;
		private IList<EventListener> listenerList = new List<EventListener>();

		private Feature feature;
		private IList<UnknownParameter> parameters = new List<UnknownParameter>();
		private IList<Restriction> restrictions = new List<Restriction>();
		private IList<GeometricPrimitive> geometricPrimitives = new List<GeometricPrimitive>();

		private IList<FeaturePoint> points = new List<FeaturePoint>();

		private EstimationStateType currentEstimationStatus = EstimationStateType.BUSY;
		private EstimationType estimationType = EstimationType.L2NORM;

		private TestStatisticDefinition testStatisticDefinition = new TestStatisticDefinition(TestStatisticType.BAARDA_METHOD, DefaultValue.ProbabilityValue, DefaultValue.PowerOfTest, false);
		private TestStatisticParameters testStatisticParameters = null;

		private static double SQRT_EPS = Math.Sqrt(Constant.EPS);
		private int maximalNumberOfIterations = DefaultValue.MaximalNumberOfIterations, iterationStep = 0, numberOfModelEquations = 0, numberOfUnknownParameters = 0, maximumNumberOfGeometricPrimitivesPerPoint = 0;

//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool interrupt_Conflict = false, calculateStochasticParameters = false, adjustModelParametersOnly = false, preconditioning = true;

		private double maxAbsDx = 0.0, maxAbsRestriction = 0.0, lastValidmaxAbsDx = 0.0, dampingValue = 0.0, adaptedDampingValue = 0.0, alphaUT = UnscentedTransformationParameter.Alpha, betaUT = UnscentedTransformationParameter.Beta, weightZero = UnscentedTransformationParameter.WeightZero;

		private VarianceComponent varianceComponentOfUnitWeight = new VarianceComponent();
		private UpperSymmPackMatrix Qxx = null;


		public virtual void init()
		{
			int nog = this.geometricPrimitives.Count;

			if (nog > 0)
			{
				IList<FeaturePoint> nonUniquePoints = new List<FeaturePoint>();
				foreach (GeometricPrimitive geometry in this.geometricPrimitives)
				{
					((List<FeaturePoint>)nonUniquePoints).AddRange(geometry.FeaturePoints);
				}

				// create a unique set of points
				LinkedHashSet<FeaturePoint> uniquePoints = new LinkedHashSet<FeaturePoint>(nonUniquePoints);
				nonUniquePoints.Clear();

				// reset points
				foreach (FeaturePoint featurePoint in uniquePoints)
				{
					featurePoint.reset();
				}

				// filter enabled points
				FilteredList<FeaturePoint> enabledUniquePoints = new FilteredList<FeaturePoint>(FXCollections.observableArrayList<FeaturePoint>(uniquePoints));
				enabledUniquePoints.setPredicate((FeaturePoint featurePoint) =>
				{
						return featurePoint.isEnable();
				});
				uniquePoints.clear();

				// Unique point list
				this.points = new List<FeaturePoint>(enabledUniquePoints);

				// number of equations rows in Jacobian A, B
				this.numberOfModelEquations = 0;
				foreach (FeaturePoint featurePoint in this.points)
				{
					int numberOfGeometries = featurePoint.getNumberOfGeomtries();
					this.maximumNumberOfGeometricPrimitivesPerPoint = Math.Max(this.maximumNumberOfGeometricPrimitivesPerPoint, numberOfGeometries);
					this.numberOfModelEquations += numberOfGeometries;
					featurePoint.getTestStatistic().setVarianceComponent(this.varianceComponentOfUnitWeight);
				}

	//			// set column indices to unknown parameters in normal equation
	//			// Please note: the following condition holds this.numberOfUnknownParameters <= this.parameters.size()
	//			// because some parameters may be held fixed or will be estimated during post-processing
	//			this.numberOfUnknownParameters = 0;
	//			int column = 0;
	//			for (UnknownParameter unknownParameter : this.parameters) {
	//				if (unknownParameter.getProcessingType() == ProcessingType.ADJUSTMENT) { //  && unknownParameter.getColumn() < 0
	//					unknownParameter.setColumn(column++);
	//					this.numberOfUnknownParameters++;
	//				}
	//				else {
	//					unknownParameter.setColumn(-1);
	//				}
	//			}
	//
	//			// set row indices to restrictions, i.e., row/column of Jacobian R in normal equation (behind unknown parameters)
	//			for (Restriction restriction : this.restrictions)
	//				restriction.setRow(column++);
	//			
	//			List<Restriction> calculations = this.feature.getPostProcessingCalculations();
	//			for (Restriction restriction : calculations)
	//				restriction.setRow(-1);

				this.testStatisticParameters = this.getTestStatisticParameters(this.testStatisticDefinition);
			}
		}

		public virtual Feature Feature
		{
			get
			{
				return this.feature;
			}
			set
			{
				if (this.feature != null)
				{
					this.fireFeatureChanged(this.feature, FeatureEventType.FEATURE_REMOVED);
				}
    
				this.reset();
				this.feature = value;
    
				if (this.feature != null)
				{
					this.parameters = this.feature.UnknownParameters;
					this.restrictions = this.feature.Restrictions;
					this.geometricPrimitives = this.feature.GeometricPrimitives;
					this.fireFeatureChanged(this.feature, FeatureEventType.FEATURE_ADDED);
				}
			}
		}


		private void reset()
		{
			this.varianceComponentOfUnitWeight.Variance0 = 1.0;
			this.varianceComponentOfUnitWeight.Omega = 0.0;
			this.varianceComponentOfUnitWeight.Redundancy = 0.0;
			this.geometricPrimitives.Clear();
			this.parameters.Clear();
			this.restrictions.Clear();
			this.points.Clear();
			this.Qxx = null;
		}

		private void prepareIterationProcess(Point centerOfMass)
		{
			// set warm start solution x <-- x0
			this.feature.applyInitialGuess();
			// reset center of mass
			this.feature.CenterOfMass.setX0(0);
			this.feature.CenterOfMass.setY0(0);
			this.feature.CenterOfMass.setZ0(0);

			// set center of mass
			if (this.feature.EstimateCenterOfMass)
			{
				this.feature.CenterOfMass = centerOfMass;
			}

			// set column indices to unknown parameters in normal equation
			// Please note: the following condition holds this.numberOfUnknownParameters <= this.parameters.size()
			// because some parameters may be held fixed or will be estimated during post-processing
			this.numberOfUnknownParameters = 0;
			int parColumn = 0;
			foreach (UnknownParameter unknownParameter in this.parameters)
			{
				if (unknownParameter.ProcessingType == ProcessingType.ADJUSTMENT)
				{ //  && unknownParameter.getColumn() < 0
					unknownParameter.Column = parColumn++;
					this.numberOfUnknownParameters++;
				}
				else
				{
					unknownParameter.Column = -1;
				}
			}

			// set row indices to restrictions, i.e., row/column of Jacobian R in normal equation (behind unknown parameters)
			foreach (Restriction restriction in this.restrictions)
			{
				restriction.Row = parColumn++;
			}

			IList<Restriction> calculations = this.feature.PostProcessingCalculations;
			foreach (Restriction restriction in calculations)
			{
				restriction.Row = -1;
			}

			// reset feature points
			foreach (FeaturePoint featurePoint in this.points)
			{
				featurePoint.reset();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.applied_geodesy.adjustment.EstimationStateType estimateModel() throws NotConvergedException, MatrixSingularException, OutOfMemoryError
		public virtual EstimationStateType estimateModel()
		{
			bool applyUnscentedTransformation = this.estimationType == EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION;

			this.currentEstimationStatus = EstimationStateType.BUSY;
			this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);

			try
			{
				this.Qxx = null;
				int dim = this.feature.FeatureType == FeatureType.CURVE ? 2 : 3;
				int numObs = this.points.Count * dim;
				int numberOfEstimationSteps = applyUnscentedTransformation ? numObs + 2 : 1;

				double alpha2 = this.alphaUT * this.alphaUT;
				double weight0 = this.weightZero;
				double weighti = (1.0 - weight0) / (double)(numberOfEstimationSteps - 1.0);

				double[][] SigmaUT = null;
				Vector xUT = null, vUT = null;
				Matrix solutionVectors = null;

				Point centerOfMass = org.applied_geodesy.adjustment.geometry.Feature.deriveCenterOfMass(this.feature.FeaturePoints);

				if (applyUnscentedTransformation)
				{
					int numUnfixedParams = 0;
					foreach (UnknownParameter unknownParameter in this.parameters)
					{
						numUnfixedParams += unknownParameter.ProcessingType != ProcessingType.FIXED ? 1 : 0;
					}

					xUT = new DenseVector(numUnfixedParams); // this.numberOfUnknownParameters
					vUT = new DenseVector(numObs);
					solutionVectors = new DenseMatrix(numUnfixedParams, numberOfEstimationSteps); // this.numberOfUnknownParameters, numberOfEstimationSteps

//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: SigmaUT = new double[numObs][dim];
					SigmaUT = RectangularArrays.RectangularDoubleArray(numObs, dim);
					if (weight0 < 0 || weight0 >= 1)
					{
						throw new System.ArgumentException("Error, zero-weight is out of range. If SUT is applied, valid values are 0 <= w0 < 1! " + weight0);
					}

					weight0 = weight0 / alpha2 + (1.0 - 1.0 / alpha2);
					weighti = weighti / alpha2;
				}

				for (int estimationStep = 0; estimationStep < numberOfEstimationSteps; estimationStep++)
				{
					this.prepareIterationProcess(new Point(centerOfMass));

					this.adaptedDampingValue = this.dampingValue;
					int runs = this.maximalNumberOfIterations - 1;
					bool isEstimated = false, estimateCompleteModel = false, isFirstIteration = true;

					if (this.maximalNumberOfIterations == 0)
					{
						estimateCompleteModel = isEstimated = true;
						isFirstIteration = false;
						this.adaptedDampingValue = 0;
					}

					this.maxAbsDx = 0.0;
					this.maxAbsRestriction = 0.0;
					this.lastValidmaxAbsDx = 0.0;
					runs = this.maximalNumberOfIterations - 1;
					isEstimated = false;
					estimateCompleteModel = false;

					double sigma2apriori = this.EstimateVarianceOfUnitWeightApriori;
					this.varianceComponentOfUnitWeight.Variance0 = 1.0;
					this.varianceComponentOfUnitWeight.Omega = 0.0;
					this.varianceComponentOfUnitWeight.Redundancy = this.numberOfModelEquations - this.numberOfUnknownParameters + this.restrictions.Count;
					this.varianceComponentOfUnitWeight.Variance0 = sigma2apriori < SQRT_EPS ? SQRT_EPS : sigma2apriori;

					if (applyUnscentedTransformation)
					{
						this.currentEstimationStatus = EstimationStateType.UNSCENTED_TRANSFORMATION_STEP;
						this.change.firePropertyChange(this.currentEstimationStatus.ToString(), numberOfEstimationSteps, estimationStep + 1);
						this.prepareSphericalSimplexUnscentedTransformationObservation(estimationStep, SigmaUT, weighti);
					}

					do
					{
						this.maxAbsDx = 0.0;
						this.maxAbsRestriction = 0.0;
						this.iterationStep = this.maximalNumberOfIterations - runs;
						this.currentEstimationStatus = EstimationStateType.ITERATE;
						this.change.firePropertyChange(this.currentEstimationStatus.ToString(), this.maximalNumberOfIterations, this.iterationStep);
						this.feature.prepareIteration();

						// create the normal system of equations including restrictions
						NormalEquationSystem neq = this.createNormalEquation();

						if (this.interrupt_Conflict || neq == null)
						{
							this.currentEstimationStatus = EstimationStateType.INTERRUPT;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							this.interrupt_Conflict = false;
							return this.currentEstimationStatus;
						}

						// apply pre-conditioning to archive a stable normal equation
						if (this.preconditioning)
						{
							this.applyPrecondition(neq);
						}

						DenseVector n = neq.Vector;
						UpperSymmPackMatrix N = neq.Matrix;

						if (!isFirstIteration)
						{
							estimateCompleteModel = isEstimated;
						}

						try
						{
							if ((estimateCompleteModel && estimationStep == (numberOfEstimationSteps - 1)) || this.estimationType == EstimationType.L1NORM)
							{
								this.calculateStochasticParameters = (this.estimationType != EstimationType.L1NORM && estimateCompleteModel);

								if (this.estimationType != EstimationType.L1NORM)
								{
									this.currentEstimationStatus = EstimationStateType.INVERT_NORMAL_EQUATION_MATRIX;
									this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
								}

								// in-place estimation normal system N * x = n: N <-- Qxx, n <-- dx 
								MathExtension.solve(N, n, !applyUnscentedTransformation);
								if (!applyUnscentedTransformation)
								{
									if (this.preconditioning)
									{
										this.applyPrecondition(neq.Preconditioner, N, n);
									}

									// extract part of unknown parameters to Qxx
									if (this.numberOfUnknownParameters != N.numColumns())
									{
										this.Qxx = new UpperSymmPackMatrix(this.numberOfUnknownParameters);
										for (int r = 0; r < this.parameters.Count; r++)
										{
											UnknownParameter parameterRow = this.parameters[r];
											int row = parameterRow.Column;
											if (row < 0)
											{
												continue;
											}
											for (int c = r; c < this.parameters.Count; c++)
											{
												UnknownParameter parameterCol = this.parameters[c];
												int column = parameterCol.Column;
												if (column < 0)
												{
													continue;
												}
												this.Qxx.set(row, column, N.get(row, column));
											}
										}
									}
									else
									{
										this.Qxx = N;
									}
								}
								else
								{
									if (this.preconditioning)
									{
										this.applyPrecondition(neq.Preconditioner, null, n);
									}
								}

								if (this.calculateStochasticParameters)
								{
									this.currentEstimationStatus = EstimationStateType.ESTIAMTE_STOCHASTIC_PARAMETERS;
									this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
								}
							}
							else
							{
								// in-place estimates of N * x = n, vector n is replaced by the solution vector x
								MathExtension.solve(N, n, false);
								if (this.preconditioning)
								{
									this.applyPrecondition(neq.Preconditioner, null, n);
								}
							}

							N = null;
							// n == [dx k]' (in-place estimation)
							this.updateModel(n, estimateCompleteModel);

							n = null;

							if (applyUnscentedTransformation)
							{
								if (estimateCompleteModel)
								{
									this.addUnscentedTransformationSolution(xUT, vUT, solutionVectors, estimationStep, estimationStep < (numberOfEstimationSteps - 1) ? weighti : weight0);
								}

								// Letzter Durchlauf der UT
								// Bestimme Parameterupdate dx und Kovarianzmatrix Qxx
								if (estimateCompleteModel && estimationStep > 0 && estimationStep == (numberOfEstimationSteps - 1))
								{
									this.estimateUnscentedTransformationParameterUpdateAndDispersion(xUT, solutionVectors, vUT, weighti, weight0 + (1.0 - alpha2 + this.betaUT));
								}
							}
						}
						catch (Exception e) when (e is MatrixSingularException || e is MatrixNotSPDException || e is System.ArgumentException || e is System.IndexOutOfRangeException || e is System.NullReferenceException)
						{
							if (applyUnscentedTransformation && SigmaUT != null)
							{
								this.prepareSphericalSimplexUnscentedTransformationObservation(-1, SigmaUT, 0);
							}
							e.printStackTrace();
							this.currentEstimationStatus = EstimationStateType.SINGULAR_MATRIX;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							throw new MatrixSingularException("Error, normal equation matrix is singular!\r\n" + e.getLocalizedMessage() != null ? e.getLocalizedMessage() : "");
						}
						catch (Exception e)
						{
							if (applyUnscentedTransformation && SigmaUT != null)
							{
								this.prepareSphericalSimplexUnscentedTransformationObservation(-1, SigmaUT, 0);
							}
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
							this.currentEstimationStatus = EstimationStateType.INTERRUPT;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							return this.currentEstimationStatus;
						}

						if (this.interrupt_Conflict)
						{
							if (applyUnscentedTransformation && SigmaUT != null)
							{
								this.prepareSphericalSimplexUnscentedTransformationObservation(-1, SigmaUT, 0);
							}
							this.currentEstimationStatus = EstimationStateType.INTERRUPT;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							this.interrupt_Conflict = false;
							return this.currentEstimationStatus;
						}

						if (double.IsInfinity(this.maxAbsDx) || double.IsNaN(this.maxAbsDx))
						{
							if (applyUnscentedTransformation && SigmaUT != null)
							{
								this.prepareSphericalSimplexUnscentedTransformationObservation(-1, SigmaUT, 0);
							}
							this.currentEstimationStatus = EstimationStateType.NO_CONVERGENCE;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							throw new NotConvergedException(NotConvergedException.Reason.Breakdown, "Error, iteration process breaks down!");
						}
						else if (!isFirstIteration && this.maxAbsDx <= SQRT_EPS && this.maxAbsRestriction <= SQRT_EPS && runs > 0 && this.adaptedDampingValue == 0)
						{
							isEstimated = true;
							this.currentEstimationStatus = EstimationStateType.CONVERGENCE;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, Math.Max(this.maxAbsDx, this.maxAbsRestriction));
						}
						else if (runs-- <= 1)
						{
							if (estimateCompleteModel)
							{
								if (this.estimationType == EstimationType.L1NORM)
								{
									if (applyUnscentedTransformation && SigmaUT != null)
									{
										this.prepareSphericalSimplexUnscentedTransformationObservation(-1, SigmaUT, 0);
									}
									this.currentEstimationStatus = EstimationStateType.ROBUST_ESTIMATION_FAILED;
									this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
									throw new NotConvergedException(NotConvergedException.Reason.Iterations, "Error, euqation system does not converge! Last iterate max|dx| = " + this.maxAbsDx + " (" + SQRT_EPS + ").");
								}
								else
								{
									if (applyUnscentedTransformation && SigmaUT != null)
									{
										this.prepareSphericalSimplexUnscentedTransformationObservation(-1, SigmaUT, 0);
									}
									this.currentEstimationStatus = EstimationStateType.NO_CONVERGENCE;
									this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxAbsDx);
									throw new NotConvergedException(NotConvergedException.Reason.Iterations, "Error, euqation system does not converge! Last iterate max|dx| = " + this.maxAbsDx + " (" + SQRT_EPS + ").");
								}
							}
							isEstimated = true;
						}
						else
						{
							this.currentEstimationStatus = EstimationStateType.CONVERGENCE;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxAbsDx);
						}
						isFirstIteration = false;

						if (isEstimated || this.adaptedDampingValue <= SQRT_EPS || runs < this.maximalNumberOfIterations * 0.1 + 1)
						{
							this.adaptedDampingValue = 0.0;
						}
					} while (!estimateCompleteModel);

					// System.out.println(estimationStep + ". max(|dx|) = " + this.maxAbsDx + "; omega = " + this.varianceComponentOfUnitWeight.getOmega() + "; itr = " + this.iterationStep);
				}
			}
			catch (System.OutOfMemoryException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				this.currentEstimationStatus = EstimationStateType.OUT_OF_MEMORY;
				this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
				throw new System.OutOfMemoryException();
			}
			finally
			{
				// Reset the center of mass
				this.feature.CenterOfMass.setX0(0);
				this.feature.CenterOfMass.setY0(0);
				this.feature.CenterOfMass.setZ0(0);
			}

			if (this.currentEstimationStatus.getId() == EstimationStateType.BUSY.getId() || this.calculateStochasticParameters)
			{
				this.currentEstimationStatus = EstimationStateType.ERROR_FREE_ESTIMATION;
				this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxAbsDx);
			}

	//		System.out.println("max(|dx|) = " + this.maxAbsDx + "; omega = " + this.varianceComponentOfUnitWeight.getOmega() + "; itr = " + this.iterationStep);
	//		for (UnknownParameter unknownParameter : this.parameters)
	//			System.out.println(unknownParameter);
			return this.currentEstimationStatus;
		}

		private double EstimateVarianceOfUnitWeightApriori
		{
			get
			{
				double vari = 0;
				int cnt = 0;
				foreach (FeaturePoint point in this.points)
				{
					if (this.interrupt_Conflict)
					{
						return 1.0;
					}
    
					int dim = point.Dimension;
    
					Matrix D = point.DispersionApriori;
					for (int rowD = 0; rowD < dim; rowD++)
					{
						double var = D.get(rowD, rowD);
						vari += var;
						cnt++;
					}
				}
				return vari / cnt;
			}
		}

		private void reverseCenterOfMass()
		{
			foreach (GeometricPrimitive geometricPrimitive in this.geometricPrimitives)
			{
				geometricPrimitive.reverseCenterOfMass(this.Qxx);
			}

			// Reset the center of mass
			this.feature.CenterOfMass.setX0(0);
			this.feature.CenterOfMass.setY0(0);
			this.feature.CenterOfMass.setZ0(0);
		}

		private void postProcessing()
		{
			IList<Restriction> calculations = this.feature.PostProcessingCalculations;
			int numberOfUnknownParameters = this.numberOfUnknownParameters; // parameters to be estimated during adjustment
			foreach (Restriction restriction in calculations)
			{
				UnknownParameter parameter = restriction.Regressand;
				if (parameter.ProcessingType != ProcessingType.POSTPROCESSING)
				{
					continue;
				}

				if (this.Qxx != null)
				{
					int rows = this.Qxx.numRows();
					int columns = this.Qxx.numColumns();

					// column of parameter
					int column = -1;

					if (parameter.Column < 0)
					{
						column = columns;
						columns++;
					}
					else
					{
						column = parameter.Column;
					}

					restriction.Row = column;
					Matrix JrT = new DenseMatrix(rows, columns);
					for (int i = 0; i < rows; i++)
					{
						// skip column of parameter to avoid doubling, because 
						// transposedJacobianElements() _adds_ elements (instead of sets)
						if (i != column)
						{
							JrT.set(i, i, 1.0);
						}
					}

					restriction.transposedJacobianElements(JrT);
					Matrix DpJrT = new DenseMatrix(rows, columns);
					this.Qxx.mult(JrT, DpJrT);
					this.Qxx = new UpperSymmPackMatrix(columns);
					JrT.transAmult(DpJrT, this.Qxx);
					parameter.Column = column;
				}
				else
				{
					if (parameter.Column < 0)
					{
						parameter.Column = numberOfUnknownParameters++;
					}
				}

				double estimate = restriction.Misclosure;
				parameter.Value = estimate;
			}
		}

		private NormalEquationSystem createNormalEquation()
		{
			int nou = this.numberOfUnknownParameters;
			int nor = this.restrictions.Count;

			UpperSymmPackMatrix N = new UpperSymmPackMatrix(nou + nor);
			UpperSymmBandMatrix V = this.preconditioning ? new UpperSymmBandMatrix(nou + nor, 0) : null;
			DenseVector n = new DenseVector(nou + nor);

			foreach (FeaturePoint point in this.points)
			{
				if (this.interrupt_Conflict)
				{
					return null;
				}

				int nog = point.NumberOfGeomtries;
				int dim = point.Dimension;

				// Derive Jacobians A, B and vector of misclosures
				Matrix Jx = new DenseMatrix(nog, nou);
				Matrix Jv = new DenseMatrix(nog, dim);
				Vector misclosures = new DenseVector(nog);
				// Create a vector of the residuals
				Vector residuals = new DenseVector(dim);
				if (dim != 1)
				{
					residuals.set(0, point.ResidualX);
					residuals.set(1, point.ResidualY);
				}
				if (dim != 2)
				{
					residuals.set(dim - 1, point.ResidualZ);
				}

				int geoIdx = 0;
				foreach (GeometricPrimitive geometricPrimitive in point)
				{
					geometricPrimitive.jacobianElements(point, Jx, Jv, geoIdx);
					misclosures.set(geoIdx, geometricPrimitive.getMisclosure(point));
					geoIdx++;
				}

				// w = -B*v + w;
				Jv.multAdd(-1.0, residuals, misclosures);
				residuals = null;

				Matrix W = this.getWeightedMatrixOfMisclosures(point, Jv);
				Jv = null;

				// P * A
				Matrix WJx = new DenseMatrix(nog, nou);
				W.mult(Jx, WJx);
				W = null;

				// AT P A und AT P w
				for (int rowJxT = 0; rowJxT < this.parameters.Count; rowJxT++)
				{
					if (this.interrupt_Conflict)
					{
						return null;
					}

					int rowN = this.parameters[rowJxT].Column;
					if (rowN < 0)
					{
						continue;
					}
					for (int colWJx = rowJxT; colWJx < this.parameters.Count; colWJx++)
					{
						int colN = this.parameters[colWJx].Column;
						if (colN < 0)
						{
							continue;
						}

						double mat = 0;
						double vec = 0;
						for (int colJxT = 0; colJxT < nog; colJxT++)
						{
							mat += Jx.get(colJxT, colN) * WJx.get(colJxT, rowN);
							if (colWJx == rowJxT)
							{
								vec += -WJx.get(colJxT, rowN) * misclosures.get(colJxT);
							}
						}
						N.add(rowN, colN, mat);
						if (colWJx == rowJxT)
						{
							n.add(rowN, vec);
						}
					}
				}
			}

			foreach (Restriction restriction in this.restrictions)
			{
				if (this.interrupt_Conflict)
				{
					return null;
				}

				// set parameter restrictions behind the model equations
				restriction.transposedJacobianElements(N);
				double misclosure = restriction.Misclosure;
				this.maxAbsRestriction = Math.Max(Math.Abs(misclosure), this.maxAbsRestriction);
				n.set(restriction.Row, -misclosure);
			}

			if (this.adaptedDampingValue > 0)
			{
				foreach (UnknownParameter unknownParameter in this.parameters)
				{
					int column = unknownParameter.Column;
					if (column < 0)
					{
						continue;
					}
					N.add(column, column, this.adaptedDampingValue * N.get(column, column));
				}
			}

			if (this.preconditioning)
			{
				// Vorkonditionierer == Wurzel der Hauptdiagonale von AT*P*A
				for (int column = 0; column < N.numColumns(); column++)
				{
					if (this.interrupt_Conflict)
					{
						return null;
					}

					double value = N.get(column, column);
					V.set(column, column, value > Constant.EPS ? 1.0 / Math.Sqrt(value) : 1.0);
					//V.set(column, column, 1.0);
				}
			}
			if (this.estimationType == EstimationType.SIMULATION)
			{
				n.zero();
			}

			return new NormalEquationSystem(N, n, V);
		}

		private void applyPrecondition(NormalEquationSystem neq)
		{
			this.applyPrecondition(neq.Preconditioner, neq.Matrix, neq.Vector);
		}

		private void applyPrecondition(UpperSymmBandMatrix V, UpperSymmPackMatrix M, Vector m)
		{
			if (V == null)
			{
				return;
			}

			for (int row = 0; row < V.numRows(); row++)
			{
				if (m != null)
				{
					m.set(row, V.get(row, row) * m.get(row));
				}
				if (M != null)
				{
					for (int column = row; column < V.numColumns(); column++)
					{
						M.set(row, column, V.get(column, column) * M.get(row, column) * V.get(row, row));
					}
				}
			}
		}

		/// <summary>
		/// Perform model update, i.e., updates residuals and parameters
		/// if estimateCompleteModel == true stochastic parameters will be derived </summary>
		/// <param name="dxk"> </param>
		/// <param name="estimateCompleteModel"> </param>
		/// <exception cref="NotConvergedException"> </exception>
		/// <exception cref="IllegalArgumentException"> </exception>
		/// <exception cref="MatrixSingularException">  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void updateModel(no.uib.cipr.matrix.Vector dxk, boolean estimateCompleteModel) throws MatrixSingularException, IllegalArgumentException, no.uib.cipr.matrix.NotConvergedException
		private void updateModel(Vector dxk, bool estimateCompleteModel)
		{
			// extract model parameters from dxk (ignoring Lagrange k) 
			Vector dx = (dxk.size() == this.numberOfUnknownParameters) ? dxk : Matrices.getSubVector(dxk, Matrices.index(0, this.numberOfUnknownParameters));

			if (this.adaptedDampingValue > 0)
			{
				// reduce the step length
				if (this.adaptedDampingValue != 0)
				{
					double alpha = 0.25 * Math.Pow(this.adaptedDampingValue, -0.05);
					alpha = Math.Min(alpha, 0.75);
					dxk.scale(alpha);
				}

				double prevOmega = this.varianceComponentOfUnitWeight.Omega;
				double curOmega = this.getOmega(dx);
				prevOmega = prevOmega <= 0 ? double.MaxValue : prevOmega;
				// Pruefe aktuelle Loesung - wenn keine Konvergenz, dann verwerfen.
				bool lmaConverge = prevOmega >= curOmega;
				this.varianceComponentOfUnitWeight.Omega = curOmega;

				if (lmaConverge)
				{
					this.adaptedDampingValue *= 0.2;
				}
				else
				{
					this.adaptedDampingValue *= 5.0;

					// Um unendlich zu vermeiden
					if (this.adaptedDampingValue > 1.0 / SQRT_EPS)
					{
						this.adaptedDampingValue = 1.0 / SQRT_EPS;

						// force an update within the next iteration 
						this.varianceComponentOfUnitWeight.Omega = 0.0;
					}

					// Aktuelle Lösung ist schlechter als die letzte --> abbrechen
					this.maxAbsDx = this.lastValidmaxAbsDx;
					dxk.zero();
					return;
				}
			}

			if (this.interrupt_Conflict)
			{
				return;
			}

			// estimate and update residuals before updating the model parameters --> estimated omega
			double omega = this.updateResiduals(dx, !this.adjustModelParametersOnly && estimateCompleteModel && this.Qxx != null && this.adaptedDampingValue == 0);
			this.varianceComponentOfUnitWeight.Omega = omega;

			// updating model parameters --> estimated maxAbsDx
			this.maxAbsDx = this.updateUnknownParameters(dx);
			this.lastValidmaxAbsDx = this.maxAbsDx;
			if (this.interrupt_Conflict)
			{
				return;
			}

			if (estimateCompleteModel)
			{
				// remove reduction to center of mass
				this.reverseCenterOfMass();
				// apply post processing of geometric templates
				this.postProcessing();
				// add uncertainties
				double varianceOfUnitWeight = this.varianceComponentOfUnitWeight.ApplyAposterioriVarianceOfUnitWeight ? this.varianceComponentOfUnitWeight.Variance : this.varianceComponentOfUnitWeight.Variance0;

				// global test statistic
				TestStatisticParameterSet globalTestStatistic = this.testStatisticParameters.getTestStatisticParameter(this.varianceComponentOfUnitWeight.Redundancy, double.PositiveInfinity, true);
				double quantil = Math.Max(globalTestStatistic.Quantile, 1.0 + Math.Sqrt(Constant.EPS));
				bool significant = this.varianceComponentOfUnitWeight.Variance / this.varianceComponentOfUnitWeight.Variance0 > quantil;
				this.varianceComponentOfUnitWeight.Significant = significant;

				if (this.Qxx != null)
				{
					foreach (UnknownParameter unknownParameter in this.parameters)
					{
						int column = unknownParameter.Column;
						unknownParameter.Uncertainty = column >= 0 ? Math.Sqrt(Math.Abs(varianceOfUnitWeight * this.Qxx.get(column, column))) : 0.0;
					}
				}
			}
		}

		/// <summary>
		/// Perform update of the model parameters
		/// X = X0 + dx </summary>
		/// <param name="dx"> </param>
		/// <returns> max(|dx|) </returns>
		private double updateUnknownParameters(Vector dx)
		{
			double maxAbsDx = 0;
			foreach (UnknownParameter unknownParameter in this.parameters)
			{
				if (this.interrupt_Conflict)
				{
					return 0;
				}

				int column = unknownParameter.Column;
				if (column < 0)
				{
					continue;
				}

				double value = unknownParameter.Value;
				double dvalue = dx.get(column);
				maxAbsDx = Math.Max(Math.Abs(dvalue), maxAbsDx);
				unknownParameter.Value = value + dvalue;
			}
			return maxAbsDx;
		}

		/// <summary>
		/// Estimates the residuals of the misclosures ve = A*dx+w, where w = -B*v + w.
		/// and returns omega = ve'*Pe*ve
		/// 
		/// </summary>
		/// <param name="dx"> </param>
		/// <returns> omega = ve'Pee </returns>
		/// <exception cref="MatrixSingularException"> </exception>
		/// <exception cref="IllegalArgumentException"> </exception>
		/// <exception cref="NotConvergedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private double getOmega(no.uib.cipr.matrix.Vector dx) throws MatrixSingularException, IllegalArgumentException, no.uib.cipr.matrix.NotConvergedException
		private double getOmega(Vector dx)
		{
			double omega = 0;

			int nou = this.numberOfUnknownParameters;
			foreach (FeaturePoint point in this.points)
			{
				if (this.interrupt_Conflict)
				{
					return 0;
				}

				int nog = point.NumberOfGeomtries;
				int dim = point.Dimension;

				// Derive Jacobians A, B and vector of misclosures
				Matrix Jx = new DenseMatrix(nog, nou);
				Matrix Jv = new DenseMatrix(nog, dim);
				Vector misclosures = new DenseVector(nog);
				// Create a vector of the residuals
				Vector residuals = new DenseVector(dim);
				if (dim != 1)
				{
					residuals.set(0, point.ResidualX);
					residuals.set(1, point.ResidualY);
				}
				if (dim != 2)
				{
					residuals.set(dim - 1, point.ResidualZ);
				}

				int geoIdx = 0;
				foreach (GeometricPrimitive geometricPrimitive in point)
				{
					geometricPrimitive.jacobianElements(point, Jx, Jv, geoIdx);
					misclosures.set(geoIdx, geometricPrimitive.getMisclosure(point));
					geoIdx++;
				}

				// w = -B*v + w;
				// ve = A*dx+w;
				// v = -Qll*B'*Pww*ve;
				Jv.multAdd(-1.0, residuals, misclosures);

				// multAdd() --> y = alpha*A*x + y
				// to save space, the residuals of the misclosures are NOW stored in misclosures vector
				Jx.multAdd(dx, misclosures);

				UpperSymmPackMatrix Ww = this.getWeightedMatrixOfMisclosures(point, Jv);
				Vector Wv = new DenseVector(nog);
				Ww.mult(misclosures, Wv);

				omega += misclosures.dot(Wv);
			}
			return omega;
		}

		/// <summary>
		/// Estimates the residuals of the misclosures ve = A*dx+w, where w = -B*v + w.
		/// Transforms ve to the residuals v of the observations, i.e.,
		/// v = -Qll*B'*Pww*ve and update observations 
		/// 
		/// </summary>
		/// <param name="dx"> </param>
		/// <param name="estimateStochasticParameters"> </param>
		/// <returns> omega = v'Pv </returns>
		/// <exception cref="MatrixSingularException"> </exception>
		/// <exception cref="IllegalArgumentException"> </exception>
		/// <exception cref="NotConvergedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private double updateResiduals(no.uib.cipr.matrix.Vector dx, boolean estimateStochasticParameters) throws MatrixSingularException, IllegalArgumentException, no.uib.cipr.matrix.NotConvergedException
		private double updateResiduals(Vector dx, bool estimateStochasticParameters)
		{
			double omega = 0;

			int nou = this.numberOfUnknownParameters;
			foreach (FeaturePoint point in this.points)
			{
				if (this.interrupt_Conflict)
				{
					return 0;
				}

				int nog = point.NumberOfGeomtries;
				int dim = point.Dimension;

				// Derive Jacobians A, B and vector of misclosures
				Matrix Jx = new DenseMatrix(nog, nou);
				Matrix Jv = new DenseMatrix(nog, dim);
				Vector misclosures = new DenseVector(nog);
				// Create a vector of the residuals
				Vector residuals = new DenseVector(dim);
				if (dim != 1)
				{
					residuals.set(0, point.ResidualX);
					residuals.set(1, point.ResidualY);
				}
				if (dim != 2)
				{
					residuals.set(dim - 1, point.ResidualZ);
				}

				int geoIdx = 0;
				foreach (GeometricPrimitive geometricPrimitive in point)
				{
					geometricPrimitive.jacobianElements(point, Jx, Jv, geoIdx);
					misclosures.set(geoIdx, geometricPrimitive.getMisclosure(point));
					geoIdx++;
				}
	//			for (int geoIdx = 0; geoIdx < nog; geoIdx++) {
	//				GeometricPrimitive geometricPrimitive = point.getGeometricPrimitive(geoIdx);
	//				geometricPrimitive.jacobianElements(point, Jx, Jv, geoIdx);
	//				misclosures.set(geoIdx, geometricPrimitive.getMisclosure(point));
	//			}

				// w = -B*v + w;
				// ve = A*dx+w;
				// v = -Qll*B'*Pww*ve;
				Jv.multAdd(-1.0, residuals, misclosures);

				// multAdd() --> y = alpha*A*x + y
				// to save space, the residuals of the misclosures are NOW stored in misclosures vector
				Jx.multAdd(dx, misclosures);
				if (!estimateStochasticParameters)
				{
					Jx = null;
				}

				UpperSymmPackMatrix Ww = this.getWeightedMatrixOfMisclosures(point, Jv);
				Vector Wv = new DenseVector(nog);
				Ww.mult(misclosures, Wv);

				if (!estimateStochasticParameters)
				{
					Ww = null;
				}

				omega += misclosures.dot(Wv);

				Vector JvTWv = new DenseVector(dim);
				Jv.transMult(Wv, JvTWv);
				Wv = null;
				if (!estimateStochasticParameters)
				{
					Jv = null;
				}

				Matrix D = point.DispersionApriori;
				D.mult(-1.0 / this.varianceComponentOfUnitWeight.Variance0, JvTWv, residuals);

				if (dim != 1)
				{
					point.ResidualX = residuals.get(0);
					point.ResidualY = residuals.get(1);
				}
				if (dim != 2)
				{
					point.ResidualZ = residuals.get(dim - 1);
				}

				// residuals are estimated - needed for deriving nabla
				if (estimateStochasticParameters)
				{
					int dof = (int)this.varianceComponentOfUnitWeight.Redundancy;
					// test statistic depends on number of equation (i.e. number of geometries but not on the dimension of the point)
					TestStatisticParameterSet testStatisticParametersAprio = this.testStatisticParameters.getTestStatisticParameter(nog, double.PositiveInfinity);
					TestStatisticParameterSet testStatisticParametersApost = this.testStatisticParameters.getTestStatisticParameter(nog, dof - nog);
					double noncentralityParameter = Math.Sqrt(Math.Abs(testStatisticParametersAprio.NoncentralityParameter));

					point.FisherQuantileApriori = testStatisticParametersAprio.Quantile;
					point.FisherQuantileAposteriori = testStatisticParametersApost.Quantile;

					this.addStochasticParameters(point, Jx, Jv, Ww, noncentralityParameter);
				}
			}
			return omega;
		}

		private UpperSymmPackMatrix getDispersionOfMisclosures(FeaturePoint point, Matrix Jv)
		{
			int dim = point.Dimension;
			int nog = point.NumberOfGeomtries;
			Matrix D = point.DispersionApriori;
			UpperSymmPackMatrix Dw = new UpperSymmPackMatrix(nog);

			Matrix JvD = new DenseMatrix(nog, dim);
			Jv.mult(1.0 / this.varianceComponentOfUnitWeight.Variance0, D, JvD);
			JvD.transBmult(Jv, Dw);

			return Dw;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private no.uib.cipr.matrix.UpperSymmPackMatrix inv(no.uib.cipr.matrix.UpperSymmPackMatrix D, boolean inplace) throws MatrixSingularException, IllegalArgumentException
		private UpperSymmPackMatrix inv(UpperSymmPackMatrix D, bool inplace)
		{
			UpperSymmPackMatrix W = inplace ? D : new UpperSymmPackMatrix(D, true);

			if (D.numColumns() == 1)
			{
				double variance = W.get(0, 0);
				if (variance <= 0)
				{
					throw new MatrixSingularException("Error, dispersion matrix is singular!");
				}
				W.set(0, 0, 1.0 / variance);
			}
			else
			{
				MathExtension.inv(W);
			}

			return W;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private no.uib.cipr.matrix.UpperSymmPackMatrix getWeightedMatrixOfMisclosures(org.applied_geodesy.adjustment.geometry.point.FeaturePoint point, no.uib.cipr.matrix.Matrix Jv) throws MatrixSingularException, IllegalArgumentException
		private UpperSymmPackMatrix getWeightedMatrixOfMisclosures(FeaturePoint point, Matrix Jv)
		{
			UpperSymmPackMatrix D = this.getDispersionOfMisclosures(point, Jv);
			return this.inv(D, true);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addStochasticParameters(org.applied_geodesy.adjustment.geometry.point.FeaturePoint point, no.uib.cipr.matrix.Matrix Jx, no.uib.cipr.matrix.Matrix Jv, no.uib.cipr.matrix.UpperSymmPackMatrix Ww, double nonCentralityParameter) throws NotConvergedException, MatrixSingularException, IllegalArgumentException
		private void addStochasticParameters(FeaturePoint point, Matrix Jx, Matrix Jv, UpperSymmPackMatrix Ww, double nonCentralityParameter)
		{
			int nou = this.numberOfUnknownParameters;
			int nog = point.NumberOfGeomtries;
			int dim = point.Dimension;
			int dof = (int)this.varianceComponentOfUnitWeight.Redundancy;

			// estimate Qll = A*Qxx*AT
			Matrix QxxJxT = new DenseMatrix(nou,nog);
			this.Qxx.transBmult(Jx, QxxJxT);
			UpperSymmPackMatrix JxQxxJxT = new UpperSymmPackMatrix(nog);
			Jx.mult(QxxJxT, JxQxxJxT);
			QxxJxT = null;

			// derive a-posteriori uncertainties of the point
			// estimates Qkk = W - W * Qll * W
			Matrix JxQxxJxTW = new DenseMatrix(nog, nog);
			JxQxxJxT.mult(Ww, JxQxxJxTW);
			JxQxxJxT = null;

			UpperSymmPackMatrix WJxQxxJxTW = new UpperSymmPackMatrix(nog);
			Ww.mult(JxQxxJxTW, WJxQxxJxTW);
			JxQxxJxTW = null;

			for (int row = 0; row < nog; row++)
			{
				for (int column = row; column < nog; column++)
				{
					WJxQxxJxTW.set(row, column, Ww.get(row, column) - WJxQxxJxTW.get(row, column));
				}
			}
			Ww = null;

			// estimate dispersion of residuals Qvv = Qll*B'*Qkk*B*Qll
			Matrix Qll = point.DispersionApriori;

			Matrix JvQll = new DenseMatrix(nog, dim);
			Jv.mult(1.0 / this.varianceComponentOfUnitWeight.Variance0, Qll, JvQll);

			Matrix WJxQxxJxTWJvQll = new DenseMatrix(nog, dim);
			WJxQxxJxTW.mult(JvQll, WJxQxxJxTWJvQll);

			Matrix QllJvTWJxQxxJxTWJvQll = new UpperSymmPackMatrix(dim);
			JvQll.transAmult(WJxQxxJxTWJvQll, QllJvTWJxQxxJxTWJvQll);
			WJxQxxJxTWJvQll = null;

			// estimates redundancies R = P * Qvv
			Matrix P = point.getInvertedDispersion(false);
			Matrix R = new DenseMatrix(dim,dim);

			double redundancy = 0;
			if (dof > 0)
			{
				P.mult(this.varianceComponentOfUnitWeight.Variance0, QllJvTWJxQxxJxTWJvQll, R);
				for (int row = 0; row < dim; row++)
				{
					double r = R.get(row, row);
					if (r > 0)
					{
						if (row == 0 && dim != 1)
						{
							point.RedundancyX = r;
						}

						else if (row == 1 && dim != 1)
						{
							point.RedundancyY = r;
						}

						else if (dim != 2)
						{
							point.RedundancyZ = r;
						}

						redundancy += r;
					}
				}
			}

			// overwrite dispersion matrix by estimated one
			// Qll.add(-1.0, QllJvTWJxQxxJxTWJvQll);
			for (int row = 0; row < dim; row++)
			{
				if (row == 0 && dim != 1)
				{
					point.CofactorX = Qll.get(row, row) / this.varianceComponentOfUnitWeight.Variance0 - QllJvTWJxQxxJxTWJvQll.get(row, row);
				}

				else if (row == 1 && dim != 1)
				{
					point.CofactorY = Qll.get(row, row) / this.varianceComponentOfUnitWeight.Variance0 - QllJvTWJxQxxJxTWJvQll.get(row, row);
				}

				else if (dim != 2)
				{
					point.CofactorZ = Qll.get(row, row) / this.varianceComponentOfUnitWeight.Variance0 - QllJvTWJxQxxJxTWJvQll.get(row, row);
				}

			}
			QllJvTWJxQxxJxTWJvQll = null;

			// estimates the gross error if it is an controlled observation, i.e., r >> 0
			if (dof > 0 && redundancy > Math.Sqrt(Constant.EPS))
			{
				// estimates Qnabla = P * Qvv * P = R * Qvv
				Matrix PQvvP = new UpperSymmPackMatrix(dim);
				R.mult(this.varianceComponentOfUnitWeight.Variance0, P, PQvvP);

				Vector residuals = new DenseVector(dim);
				if (dim != 1)
				{
					residuals.set(0, point.ResidualX);
					residuals.set(1, point.ResidualY);
				}
				if (dim != 2)
				{
					residuals.set(dim - 1, point.ResidualZ);
				}

				Vector weightedResiduals = new DenseVector(dim);
				P.mult(this.varianceComponentOfUnitWeight.Variance0, residuals, weightedResiduals);

				Vector grossErrors = new DenseVector(dim);
				Matrix invPQvvP = MathExtension.pinv(PQvvP, nog);
				invPQvvP.mult(-1.0, weightedResiduals, grossErrors);
				if (dim != 1)
				{
					point.GrossErrorX = grossErrors.get(0);
					point.GrossErrorY = grossErrors.get(1);
				}
				if (dim != 2)
				{
					point.GrossErrorZ = grossErrors.get(dim - 1);
				}

				// estimate maximum tolerable/minimal detectable bias, using same orientation as gross error vector
				// nabla0 * Pnn * nabla0 == 1  or  nabla0 * Pnn * nabla0 == ncp
				Vector minimalDetectableBias = new DenseVector(grossErrors, true);
				Vector weightedMinimalDetectableBias = new DenseVector(dim);
				PQvvP.mult(1.0 / this.varianceComponentOfUnitWeight.Variance0,minimalDetectableBias, weightedMinimalDetectableBias);

				double nQn0 = minimalDetectableBias.dot(weightedMinimalDetectableBias);
				for (int j = 0; j < dim; j++)
				{
					if (nQn0 > 0)
					{
						minimalDetectableBias.set(j, minimalDetectableBias.get(j) / Math.Sqrt(nQn0));
					}
				}

				if (dim != 1)
				{
					point.MinimalDetectableBiasX = nonCentralityParameter * minimalDetectableBias.get(0);
					point.MinimalDetectableBiasY = nonCentralityParameter * minimalDetectableBias.get(1);
					point.MaximumTolerableBiasX = minimalDetectableBias.get(0);
					point.MaximumTolerableBiasY = minimalDetectableBias.get(1);
				}
				if (dim != 2)
				{
					point.MinimalDetectableBiasZ = nonCentralityParameter * minimalDetectableBias.get(dim - 1);
					point.MaximumTolerableBiasZ = minimalDetectableBias.get(dim - 1);
				}

				// change sign, i.e. residual vs. error
				double T = -weightedResiduals.dot(grossErrors);
				point.TestStatistic.FisherTestNumerator = T;
				point.TestStatistic.DegreeOfFreedom = nog;
			}
		}

		public virtual TestStatisticParameters TestStatisticParameters
		{
			get
			{
				return this.testStatisticParameters;
			}
		}

		public virtual TestStatisticDefinition TestStatisticDefinition
		{
			get
			{
				return this.testStatisticDefinition;
			}
		}

		public virtual UpperSymmPackMatrix CorrelationMatrix
		{
			get
			{
				if (this.Qxx == null)
				{
					return null;
				}
    
				int size = this.Qxx.numColumns();
				UpperSymmPackMatrix corr = new UpperSymmPackMatrix(size);
    
				for (int r = 0; r < size; r++)
				{
					UnknownParameter parameterR = this.parameters[r];
					int row = parameterR.Column;
					if (row < 0)
					{
						continue;
					}
    
					corr.set(row, row, 1.0);
    
					double varR = Math.Abs(this.Qxx.get(row, row));
					if (varR < SQRT_EPS)
					{
						continue;
					}
    
					for (int c = r + 1; c < size; c++)
					{
						UnknownParameter parameterC = this.parameters[c];
						int column = parameterC.Column;
						if (column < 0)
						{
							continue;
						}
    
						double varC = this.Qxx.get(column, column);
						if (varC < SQRT_EPS)
						{
							continue;
						}
    
						corr.set(row, column, this.Qxx.get(row, column) / Math.Sqrt(varR) / Math.Sqrt(varC));
					}
				}
				return corr;
			}
		}

		internal virtual TestStatisticParameters getTestStatisticParameters(TestStatisticDefinition testStatisticDefinition)
		{
			double alpha = testStatisticDefinition.ProbabilityValue;
			double beta = testStatisticDefinition.PowerOfTest;
			int dof = (int)this.varianceComponentOfUnitWeight.Redundancy;
			int numberOfHypotesis = this.points.Count + (dof > 0 ? 1 : 0); // add one for global test //TODO add further hypotesis tests
			int dim = this.maximumNumberOfGeometricPrimitivesPerPoint; // Reference number is equal to the point that lies in most geometries

			TestStatistic testStatistic;
			switch (testStatisticDefinition.TestStatisticType.innerEnumValue)
			{
			case TestStatisticType.InnerEnum.SIDAK:
				// alle Hypothesen + Test der Varianzkomponenten + Test der Festpunkte
				testStatistic = new SidakTestStatistic(numberOfHypotesis, alpha, beta, testStatisticDefinition.FamilywiseErrorRate);
				break;
			case TestStatisticType.InnerEnum.BAARDA_METHOD:
				testStatistic = new BaardaMethodTestStatistic(testStatisticDefinition.FamilywiseErrorRate ? dof : dim, alpha, beta);
				break;
			case TestStatisticType.InnerEnum.NONE:
				testStatistic = new UnadjustedTestStatitic(alpha, beta);
				break;
			default:
				throw new System.ArgumentException(this.GetType().Name + " Error, unknown test statistic method " + testStatisticDefinition.TestStatisticType);
			}
			return new TestStatisticParameters(testStatistic);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void prepareSphericalSimplexUnscentedTransformationObservation(int estimationStep, double[][] SigmaUT, double weight) throws IllegalArgumentException, no.uib.cipr.matrix.MatrixNotSPDException
		private void prepareSphericalSimplexUnscentedTransformationObservation(int estimationStep, double[][] SigmaUT, double weight)
		{
			int dim = this.Feature.FeatureType == FeatureType.CURVE ? 2 : 3;
			int noo = this.points.Count * dim;

			int row = 0;
			foreach (FeaturePoint point in this.points)
			{
				// Cholesky factor
				UpperTriangPackMatrix G = new UpperTriangPackMatrix(point.DispersionApriori, true);
				MathExtension.chol(G);
				Vector sigmaUT = new DenseVector(SigmaUT[row], false);
				Vector delta = new DenseVector(dim);
				G.transMult(sigmaUT, delta);

				// Reverse modification of last UT step
				if (dim != 1)
				{
					point.X0 = point.X0 - delta.get(0);
					point.Y0 = point.Y0 - delta.get(1);
				}
				if (dim != 2)
				{
					point.Z0 = point.Z0 - delta.get(dim - 1);
				}

				// derive new Sigma points, if estimationStep >= 0
				sigmaUT.zero();

				if (dim != 1)
				{
					if (estimationStep >= 0 && estimationStep < noo + 2)
					{
						if (row == estimationStep - 1)
						{
							sigmaUT.set(0, (1.0 + row) / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight));
						}
						else if (row > estimationStep - 2)
						{
							sigmaUT.set(0, -1.0 / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight));
						}
					}
					row++;
					if (estimationStep >= 0 && estimationStep < noo + 2)
					{
						if (row == estimationStep - 1)
						{
							sigmaUT.set(1, (1.0 + row) / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight));
						}
						else if (row > estimationStep - 2)
						{
							sigmaUT.set(1, -1.0 / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight));
						}
					}
					row++;
				}
				if (dim != 2)
				{
					if (estimationStep >= 0 && estimationStep < noo + 2)
					{
						if (row == estimationStep - 1)
						{
							sigmaUT.set(dim - 1, (1.0 + row) / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight));
						}
						else if (row > estimationStep - 2)
						{
							sigmaUT.set(dim - 1, -1.0 / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight));
						}
					}
					row++;
				}
				if (estimationStep >= 0)
				{
					delta = new DenseVector(dim);
					G.transMult(sigmaUT, delta);
					// Modify points
					if (dim != 1)
					{
						point.X0 = point.X0 + delta.get(0);
						point.Y0 = point.Y0 + delta.get(1);
					}
					if (dim != 2)
					{
						point.Z0 = point.Z0 + delta.get(dim - 1);
					}
				}
			}
		}

		private void addUnscentedTransformationSolution(Vector xUT, Vector vUT, Matrix solutionVectors, int solutionNumber, double weight)
		{
			foreach (UnknownParameter unknownParameter in this.parameters)
			{
				if (this.interrupt_Conflict)
				{
					return;
				}

				int col = unknownParameter.Column;
				if (col < 0)
				{
					continue;
				}

				double value = unknownParameter.Value;
				xUT.set(col, xUT.get(col) + weight * value);
				solutionVectors.set(col, solutionNumber, value);
			}

			if (vUT != null)
			{
				int row = 0;
				foreach (FeaturePoint point in this.points)
				{
					int dim = point.Dimension;

					if (dim != 1)
					{
						double vx = point.ResidualX;
						double vy = point.ResidualY;

						vUT.set(row, vUT.get(row++) + weight * vx);
						vUT.set(row, vUT.get(row++) + weight * vy);
					}

					if (dim != 2)
					{
						double vz = point.ResidualZ;

						vUT.set(row, vUT.get(row++) + weight * vz);
					}
				}
			}
		}

		private void estimateUnscentedTransformationParameterUpdateAndDispersion(Vector xUT, Matrix solutionVectors, Vector vUT, double weightN, double weightC)
		{
			int numberOfEstimationSteps = solutionVectors.numColumns();

			if (this.Qxx == null)
			{
				this.Qxx = new UpperSymmPackMatrix(xUT.size());
			}
			this.Qxx.zero();

			for (int r = 0; r < this.parameters.Count; r++)
			{
				if (this.interrupt_Conflict)
				{
					return;
				}

				UnknownParameter unknownParameterRow = this.parameters[r];
				int row = unknownParameterRow.Column;
				if (row < 0)
				{
					continue;
				}

				unknownParameterRow.Value = xUT.get(row);

				for (int c = r; c < this.parameters.Count; c++)
				{
					if (this.interrupt_Conflict)
					{
						return;
					}

					UnknownParameter unknownParameterCol = this.parameters[c];
					int col = unknownParameterCol.Column;
					if (col < 0)
					{
						continue;
					}

					// Laufindex des jeweiligen Spalten-/Zeilenvektors
					for (int estimationStep = 0; estimationStep < numberOfEstimationSteps; estimationStep++)
					{
						double weight = estimationStep == (numberOfEstimationSteps - 1) ? weightC : weightN;

						double valueCol = solutionVectors.get(col, estimationStep) - xUT.get(col);
						double valueRow = solutionVectors.get(row, estimationStep) - xUT.get(row);

						this.Qxx.set(row, col, this.Qxx.get(row, col) + valueRow * weight * valueCol);
					}
				}
			}

			solutionVectors = null;
			xUT = null;

			int row = 0;
			foreach (FeaturePoint point in this.points)
			{
				if (this.interrupt_Conflict)
				{
					return;
				}

				int dim = point.Dimension;

				if (dim != 1)
				{
					double vx = vUT.get(row++);
					double vy = vUT.get(row++);

					point.ResidualX = vx;
					point.ResidualY = vy;
				}
				if (dim != 2)
				{
					double vz = vUT.get(row++);
					point.ResidualZ = vz;
				}
			}

			// add uncertainties
			double varianceOfUnitWeight = this.varianceComponentOfUnitWeight.ApplyAposterioriVarianceOfUnitWeight ? this.varianceComponentOfUnitWeight.Variance : this.varianceComponentOfUnitWeight.Variance0;
			varianceOfUnitWeight = varianceOfUnitWeight / this.varianceComponentOfUnitWeight.Variance0;

			// global test statistic
			TestStatisticParameterSet globalTestStatistic = this.testStatisticParameters.getTestStatisticParameter(this.varianceComponentOfUnitWeight.Redundancy, double.PositiveInfinity, true);
			double quantil = Math.Max(globalTestStatistic.Quantile, 1.0 + Math.Sqrt(Constant.EPS));
			bool significant = this.varianceComponentOfUnitWeight.Variance / this.varianceComponentOfUnitWeight.Variance0 > quantil;
			this.varianceComponentOfUnitWeight.Significant = significant;
			foreach (UnknownParameter unknownParameter in this.parameters)
			{
				int column = unknownParameter.Column;
				unknownParameter.Uncertainty = column >= 0 ? Math.Sqrt(Math.Abs(varianceOfUnitWeight * this.Qxx.get(column, column))) : 0.0;
			}
		}

		public virtual VarianceComponent VarianceComponentOfUnitWeight
		{
			get
			{
				return this.varianceComponentOfUnitWeight;
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

		private void fireFeatureChanged(Feature feature, FeatureEventType eventType)
		{
			FeatureEvent evt = new FeatureEvent(feature, eventType);
			object[] listeners = this.listenerList.ToArray();
			for (int i = 0; i < listeners.Length; i++)
			{
				if (listeners[i] is FeatureChangeListener)
				{
					((FeatureChangeListener)listeners[i]).featureChanged(evt);
				}
			}
		}

		public virtual double LevenbergMarquardtDampingValue
		{
			set
			{
				this.dampingValue = Math.Abs(value);
			}
			get
			{
				return this.dampingValue;
			}
		}


		public virtual int MaximalNumberOfIterations
		{
			get
			{
				return this.maximalNumberOfIterations;
			}
			set
			{
				this.maximalNumberOfIterations = value;
			}
		}


		public virtual bool AdjustModelParametersOnly
		{
			get
			{
				return this.adjustModelParametersOnly;
			}
			set
			{
				this.adjustModelParametersOnly = value;
			}
		}


		public virtual bool Preconditioning
		{
			get
			{
				return this.preconditioning;
			}
			set
			{
				this.preconditioning = value;
			}
		}


		public virtual void addFeatureChangeListener(FeatureChangeListener l)
		{
			this.listenerList.Add(l);
		}

		public virtual void removeFeatureChangeListener(FeatureChangeListener l)
		{
			this.listenerList.Remove(l);
		}

		public virtual void interrupt()
		{
			this.interrupt_Conflict = true;
		}

		public virtual EstimationType EstimationType
		{
			get
			{
				return this.estimationType;
			}
			set
			{
				if (value == EstimationType.L2NORM || value == EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION)
				{
					this.estimationType = value;
				}
				else
				{
					throw new System.ArgumentException("Error, unsupported estimation type " + value + "!");
				}
			}
		}


		/// <summary>
		/// Liefert den Skalierungsparameter alpha der UT,
		/// der den Abstand der Sigma-Punkte um den Mittelwert
		/// steuert. Ueblicherweise ist alpha gering, d.h., 1E-3
		/// Der Defaultwert ist 1, sodass keine Skalierung
		/// vorgenommen wird und die Standard-UT resultiert </summary>
		/// <returns> alphaUT </returns>
		public virtual double UnscentedTransformationScaling
		{
			get
			{
				return this.alphaUT;
			}
			set
			{
				if (value > 0)
				{
					this.alphaUT = value;
				}
			}
		}

		/// <summary>
		/// Liefert den Daempfungsparameter beta der UT,
		/// der a-priori Informatione bzgl. der Verteilung
		/// der Daten beruecksichtigt. Fuer Gauss-Verteilung
		/// ist beta = 2 optimal </summary>
		/// <returns> betaUT </returns>
		public virtual double UnscentedTransformationDamping
		{
			get
			{
				return this.betaUT;
			}
			set
			{
				this.betaUT = value;
			}
		}

		/// <summary>
		/// Liefert die Gewichtung des Sigma-Punktes X0 bzw. Y0 = f(X0) </summary>
		/// <returns> w0 </returns>
		public virtual double UnscentedTransformationWeightZero
		{
			get
			{
				return this.weightZero;
			}
			set
			{
				if (value < 1)
				{
					this.weightZero = value;
				}
			}
		}





	//	public static void main(String[] args) throws Exception {
	//		FeatureAdjustment adjustment = new FeatureAdjustment();
	//
	//		FeaturePoint point1 = new FeaturePoint("1", 2.986, 2.496, 2.928);
	//		FeaturePoint point2 = new FeaturePoint("2", 4.370, 5.276, 3.079);
	//		FeaturePoint point3 = new FeaturePoint("3", 4.845, 6.347, 5.817);
	//		FeaturePoint point4 = new FeaturePoint("4", 2.375, 1.485, 6.589);
	//		FeaturePoint point5 = new FeaturePoint("5", 2.448, 1.462, 4.205);
	//		FeaturePoint point6 = new FeaturePoint("6", 3.049, 2.764, 8.050);
	//		FeaturePoint point7 = new FeaturePoint("7", 4.012, 4.706, 8.082);
	//		FeaturePoint point8 = new FeaturePoint("8", 4.567, 5.054, 7.141);
	//
	//		point1.setDispersionApriori(
	//				new UpperSymmPackMatrix(new DenseMatrix(new double[][] {
	//					{  16.0839803365062,  4.65534839899422, -4.38460084854308},
	//					{  4.65534839899422,  26.3272479942061,  2.47834290278709},
	//					{ -4.38460084854308,  2.47834290278709,  29.0815646040579}
	//				}))
	//				);
	//
	//		point2.setDispersionApriori(
	//				new UpperSymmPackMatrix(new DenseMatrix(new double[][] {
	//					{  17.8964457912516,  3.09111321599046,  5.64126184131749},
	//					{  3.09111321599046,  19.0624298799118,  8.08205153582906},
	//					{  5.64126184131749,  8.08205153582906,  26.1468430557483}
	//				}))
	//				);
	//
	//		point3.setDispersionApriori(
	//				new UpperSymmPackMatrix(new DenseMatrix(new double[][] {
	//					{  29.0047447515151, 0.403158357030017,  11.1766961352954},
	//					{ 0.403158357030017,  22.9345053072238, -7.29211633546859},
	//					{  11.1766961352954, -7.29211633546859,  21.2201800630207},
	//				}))
	//				);
	//
	//		point4.setDispersionApriori(
	//				new UpperSymmPackMatrix(new DenseMatrix(new double[][] {
	//					{   14.007661244179,  1.79628360816322,  4.13632502272246},
	//					{  1.79628360816322,  31.9556797927839, -2.25880229809073},
	//					{  4.13632502272246, -2.25880229809073,  27.3871830020619}
	//				}))
	//				);
	//
	//		point5.setDispersionApriori(
	//				new UpperSymmPackMatrix(new DenseMatrix(new double[][] {
	//					{  24.2029609740167, -4.29135894085001, -4.78113396533508},
	//					{ -4.29135894085001,  21.3674323634287, -1.42807065830297},
	//					{ -4.78113396533508, -1.42807065830297,  17.6619349346275}
	//				}))
	//				);
	//
	//		point6.setDispersionApriori(
	//				new UpperSymmPackMatrix(new DenseMatrix(new double[][] {
	//					{  20.7448446052198,  7.74193819492723, -3.21311943337311},
	//					{  7.74193819492723,  42.3093042517494,  -5.4060692623952},
	//					{ -3.21311943337311,  -5.4060692623952,   17.565600995021}
	//				}))
	//				);
	//
	//		point7.setDispersionApriori(
	//				new UpperSymmPackMatrix(new DenseMatrix(new double[][] {
	//					{   47.221895847014, 0.692081451296938,  3.57664252908793},
	//					{ 0.692081451296938,  15.1198359736348, -3.95299745877936},
	//					{  3.57664252908793, -3.95299745877936,  29.0659166670697}
	//				}))
	//				);
	//
	//		point8.setDispersionApriori(
	//				new UpperSymmPackMatrix(new DenseMatrix(new double[][] {
	//					{  29.3255840578623,  -7.3780624024934, -7.22152909351128},
	//					{  -7.3780624024934,  13.6447146847114, 0.978337662987412},
	//					{ -7.22152909351128, 0.978337662987412,  24.3329973857069}
	//				}))
	//				);
	//
	//		java.util.Set<FeaturePoint> points = new LinkedHashSet<FeaturePoint>();
	//		points.add(point1);
	//		points.add(point2);
	//		points.add(point3);
	//		points.add(point4);
	//		points.add(point5);
	//		points.add(point6);
	//		points.add(point7);
	//		points.add(point8);
	//
	//		org.applied_geodesy.adjustment.geometry.point.Point centerOfMass = Feature.deriveCenterOfMass(points);
	//
	//		/** Circle fit **/
	//		Feature feature = new org.applied_geodesy.adjustment.geometry.surface.SpatialCircleFeature();
	//
	//		/** Start adjustment **/
	//		for (GeometricPrimitive geometricPrimitive : feature)
	//			geometricPrimitive.getFeaturePoints().addAll(points);
	//
	//
	//		org.applied_geodesy.adjustment.geometry.restriction.FeaturePointRestriction featurePointPlaneRestriction = 
	//				new org.applied_geodesy.adjustment.geometry.restriction.FeaturePointRestriction(
	//						false, 
	//						((org.applied_geodesy.adjustment.geometry.surface.SpatialCircleFeature)feature).getPlane(), 
	//						point5
	//		);
	//		
	//		org.applied_geodesy.adjustment.geometry.restriction.FeaturePointRestriction featurePointCircleRestriction = 
	//				new org.applied_geodesy.adjustment.geometry.restriction.FeaturePointRestriction(
	//						false, 
	//						((org.applied_geodesy.adjustment.geometry.surface.SpatialCircleFeature)feature).getSphere(), 
	//						point5
	//		);
	//		
	//		adjustment.setFeature(feature);
	//		feature.deriveInitialGuess();
	//		feature.applyInitialGuess();
	//		feature.setCenterOfMass(centerOfMass);
	//		
	//		feature.getRestrictions().addAll(featurePointPlaneRestriction, featurePointCircleRestriction);
	//
	//		adjustment.init();
	//		adjustment.estimateModel();
	//		
	//		System.out.println(point5.getX0()+"  "+point5.getX());
	//		System.out.println(point5.getY0()+"  "+point5.getY());
	//		System.out.println(point5.getZ0()+"  "+point5.getZ());
	//	}
	}

}