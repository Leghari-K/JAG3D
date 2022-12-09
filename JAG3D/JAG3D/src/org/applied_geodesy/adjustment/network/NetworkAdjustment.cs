using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

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

namespace org.applied_geodesy.adjustment.network
{

	using ConfidenceRegion = org.applied_geodesy.adjustment.ConfidenceRegion;
	using Constant = org.applied_geodesy.adjustment.Constant;
	using DefaultValue = org.applied_geodesy.adjustment.DefaultValue;
	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using EstimationType = org.applied_geodesy.adjustment.EstimationType;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using NormalEquationSystem = org.applied_geodesy.adjustment.NormalEquationSystem;
	using UnscentedTransformationParameter = org.applied_geodesy.adjustment.UnscentedTransformationParameter;
	using CongruenceAnalysisGroup = org.applied_geodesy.adjustment.network.congruence.CongruenceAnalysisGroup;
	using CongruenceAnalysisPointPair = org.applied_geodesy.adjustment.network.congruence.CongruenceAnalysisPointPair;
	using CoordinateComponent = org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent;
	using Equation = org.applied_geodesy.adjustment.network.congruence.strain.Equation;
	using RestrictionType = org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType;
	using StrainAnalysisEquations = org.applied_geodesy.adjustment.network.congruence.strain.StrainAnalysisEquations;
	using StrainParameter = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameter;
	using ComponentType = org.applied_geodesy.adjustment.network.observation.ComponentType;
	using DeltaZ = org.applied_geodesy.adjustment.network.observation.DeltaZ;
	using Direction = org.applied_geodesy.adjustment.network.observation.Direction;
	using GNSSBaseline = org.applied_geodesy.adjustment.network.observation.GNSSBaseline;
	using GNSSBaseline1D = org.applied_geodesy.adjustment.network.observation.GNSSBaseline1D;
	using GNSSBaseline2D = org.applied_geodesy.adjustment.network.observation.GNSSBaseline2D;
	using GNSSBaseline3D = org.applied_geodesy.adjustment.network.observation.GNSSBaseline3D;
	using HorizontalDistance = org.applied_geodesy.adjustment.network.observation.HorizontalDistance;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using SlopeDistance = org.applied_geodesy.adjustment.network.observation.SlopeDistance;
	using ZenithAngle = org.applied_geodesy.adjustment.network.observation.ZenithAngle;
	using ObservationGroup = org.applied_geodesy.adjustment.network.observation.group.ObservationGroup;
	using AdditionalUnknownParameter = org.applied_geodesy.adjustment.network.parameter.AdditionalUnknownParameter;
	using UnknownParameter = org.applied_geodesy.adjustment.network.parameter.UnknownParameter;
	using UnknownParameters = org.applied_geodesy.adjustment.network.parameter.UnknownParameters;
	using VerticalDeflection = org.applied_geodesy.adjustment.network.parameter.VerticalDeflection;
	using VerticalDeflectionX = org.applied_geodesy.adjustment.network.parameter.VerticalDeflectionX;
	using VerticalDeflectionY = org.applied_geodesy.adjustment.network.parameter.VerticalDeflectionY;
	using Point = org.applied_geodesy.adjustment.network.point.Point;
	using Point3D = org.applied_geodesy.adjustment.network.point.Point3D;
	using BaardaMethodTestStatistic = org.applied_geodesy.adjustment.statistic.BaardaMethodTestStatistic;
	using SidakTestStatistic = org.applied_geodesy.adjustment.statistic.SidakTestStatistic;
	using TestStatistic = org.applied_geodesy.adjustment.statistic.TestStatistic;
	using TestStatisticDefinition = org.applied_geodesy.adjustment.statistic.TestStatisticDefinition;
	using TestStatisticParameterSet = org.applied_geodesy.adjustment.statistic.TestStatisticParameterSet;
	using TestStatisticParameters = org.applied_geodesy.adjustment.statistic.TestStatisticParameters;
	using TestStatisticType = org.applied_geodesy.adjustment.statistic.TestStatisticType;
	using UnadjustedTestStatitic = org.applied_geodesy.adjustment.statistic.UnadjustedTestStatitic;
	using SphericalDeflectionModel = org.applied_geodesy.transformation.datum.SphericalDeflectionModel;

	using Pair = javafx.util.Pair;
	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using Matrices = no.uib.cipr.matrix.Matrices;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixNotSPDException = no.uib.cipr.matrix.MatrixNotSPDException;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using UpperSymmBandMatrix = no.uib.cipr.matrix.UpperSymmBandMatrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;
	using Vector = no.uib.cipr.matrix.Vector;
	using SparseVector = no.uib.cipr.matrix.sparse.SparseVector;

	public class NetworkAdjustment : ThreadStart
	{
		private bool InstanceFieldsInitialized = false;

		public NetworkAdjustment()
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
			currentMaxAbsDx = maxDx;
		}

		private IDictionary<Observation, double> adaptedObservationUncertainties = new LinkedHashMap<Observation, double>();
		private IDictionary<Point, double[]> adaptedPointUncertainties = new LinkedHashMap<Point, double[]>();
		private IDictionary<VerticalDeflection, double> adaptedVerticalDeflectionUncertainties = new LinkedHashMap<VerticalDeflection, double>();
		private PropertyChangeSupport change;
//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool calculateStochasticParameters_Conflict = false;
		private static double SQRT_EPS = Math.Sqrt(Constant.EPS);
		private EstimationType estimationType = EstimationType.L2NORM;
		private UpperSymmPackMatrix Qxx = null;
		private SphericalDeflectionModel sphericalDeflectionModel = null;

		private int maximalNumberOfIterations = DefaultValue.MaximalNumberOfIterations, iterationStep = 0, numberOfStochasticPointRows = 0, numberOfStochasticDeflectionRows = 0, numberOfFixPointRows = 0, numberOfUnknownParameters = 0, numberOfObservations = 0, numberOfHypotesis = 0, numberOfPrincipalComponents = 0;

//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool interrupt_Conflict = false, freeNetwork = false, congruenceAnalysis = false, proofOfDatumDefectDetection = false, applyAposterioriVarianceOfUnitWeight = true;

//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private double maxDx = double.Epsilon, degreeOfFreedom_Conflict = 0.0, omega = 0.0, traceCxxPoints = 0.0, finalLinearisationError = 0.0, robustEstimationLimit = DefaultValue.RobustEstimationLimit, alphaUT = UnscentedTransformationParameter.Alpha, betaUT = UnscentedTransformationParameter.Beta, weightZero = UnscentedTransformationParameter.WeightZero;

		private EstimationStateType currentEstimationStatus = EstimationStateType.BUSY;
		private double currentMaxAbsDx;

		private TestStatisticDefinition significanceTestStatisticDefinition = new TestStatisticDefinition(TestStatisticType.BAARDA_METHOD, DefaultValue.ProbabilityValue, DefaultValue.PowerOfTest, false);
		private TestStatisticParameters significanceTestStatisticParameters = null;

		private IDictionary<string, Point> allPoints = new LinkedHashMap<string, Point>();
		private IList<Point> datumPoints = new List<Point>();
		private IList<Point> stochasticPoints = new List<Point>();

		private IList<Point> pointsWithUnknownDeflection = new List<Point>();
		private IList<Point> pointsWithStochasticDeflection = new List<Point>();
		private IList<Point> pointsWithReferenceDeflection = new List<Point>();

		private IList<Point> referencePoints = new List<Point>();
		private PrincipalComponent[] principalComponents = new PrincipalComponent[0];
		private IList<CongruenceAnalysisGroup> congruenceAnalysisGroup = new List<CongruenceAnalysisGroup>();
		private IDictionary<VarianceComponentType, VarianceComponent> varianceComponents = new LinkedHashMap<VarianceComponentType, VarianceComponent>();
		private IDictionary<int, Matrix> ATQxxBP_GNSS_EP = new LinkedHashMap<int, Matrix>();
		private IDictionary<int, Matrix> PAzTQzzAzP_GNSS_EF = new LinkedHashMap<int, Matrix>();
		private UnknownParameters unknownParameters = new UnknownParameters();
		private ObservationGroup projectObservations = new ObservationGroup(-1, Double.NaN, Double.NaN, Double.NaN, Epoch.REFERENCE);
		private RankDefect rankDefect = new RankDefect();
		private string coVarExportPathAndFileName = null;

		/// <summary>
		/// Berechnung von Vor-Faktoren zur Bestimmung von EP und EF*SP. Fuer terrestrische Beobachtung wird der Vor-Faktor 
		/// temp. in EP bzw. EFSP gespeichert und spaeter mit Nabla verrechnet. Fuer GNSS ergibt sich eine Matrix, die mittels
		/// Nabla zu einem Vektor bzw. Skalar wird. Daher erfolgt die Speicherung in Maps.
		/// 
		/// Zusaetzlich werden die Normalgleichung und der Absolutgliedvektor in-situ ueberschrieben, 
		/// sodass N == Qxx (wenn invert == true) und n == dx am Ende ist.
		/// </summary>
		/// <param name="N"> NEG-Matrix </param>
		/// <param name="n"> neg-Vektor </param>
		private void estimateFactorsForOutherAccracy(UpperSymmPackMatrix N, DenseVector n)
		{
			// Indexzuordnung Submatrix vs. Gesamtmatrix
			IDictionary<int, int> idxAddParamGlobal2LocalInQxx = new LinkedHashMap<int, int>();
			IDictionary<int, int> idxPointGlobal2LocalInQxx = new LinkedHashMap<int, int>();
			IList<int> idxAddParamLocal2GlobalInQxx = new List<int>();
			IList<int> idxPointLocal2GlobalInQxx = new List<int>();

			for (int i = 0; i < this.unknownParameters.size(); i++)
			{
				UnknownParameter param = this.unknownParameters.get(i);
				if (param.ColInJacobiMatrix < 0)
				{
					continue;
				}

				if (param.ParameterType == ParameterType.POINT1D)
				{
					idxPointGlobal2LocalInQxx[param.ColInJacobiMatrix] = idxPointLocal2GlobalInQxx.Count;

					idxPointLocal2GlobalInQxx.Add(param.ColInJacobiMatrix);
				}
				else if (param.ParameterType == ParameterType.POINT2D)
				{
					idxPointGlobal2LocalInQxx[param.ColInJacobiMatrix] = idxPointLocal2GlobalInQxx.Count;
					idxPointGlobal2LocalInQxx[param.ColInJacobiMatrix + 1] = idxPointLocal2GlobalInQxx.Count + 1;

					idxPointLocal2GlobalInQxx.Add(param.ColInJacobiMatrix);
					idxPointLocal2GlobalInQxx.Add(param.ColInJacobiMatrix + 1);
				}
				else if (param.ParameterType == ParameterType.POINT3D)
				{
					idxPointGlobal2LocalInQxx[param.ColInJacobiMatrix] = idxPointLocal2GlobalInQxx.Count;
					idxPointGlobal2LocalInQxx[param.ColInJacobiMatrix + 1] = idxPointLocal2GlobalInQxx.Count + 1;
					idxPointGlobal2LocalInQxx[param.ColInJacobiMatrix + 2] = idxPointLocal2GlobalInQxx.Count + 2;

					idxPointLocal2GlobalInQxx.Add(param.ColInJacobiMatrix);
					idxPointLocal2GlobalInQxx.Add(param.ColInJacobiMatrix + 1);
					idxPointLocal2GlobalInQxx.Add(param.ColInJacobiMatrix + 2);
				}
				else if (param is AdditionalUnknownParameter)
				{
					idxAddParamGlobal2LocalInQxx[param.ColInJacobiMatrix] = idxAddParamLocal2GlobalInQxx.Count;

					idxAddParamLocal2GlobalInQxx.Add(param.ColInJacobiMatrix);
				}
			}

			UpperSymmPackMatrix Qzz = new UpperSymmPackMatrix(idxAddParamLocal2GlobalInQxx.Count);
			for (int k = 0; k < idxAddParamLocal2GlobalInQxx.Count; k++)
			{
				int rowM = idxAddParamLocal2GlobalInQxx[k];
				for (int j = k; j < idxAddParamLocal2GlobalInQxx.Count; j++)
				{
					int colM = idxAddParamLocal2GlobalInQxx[j];
					Qzz.set(k, j, N.get(rowM, colM));
				}
			}

			Matrix QzzNzx = new DenseMatrix(idxAddParamLocal2GlobalInQxx.Count, idxPointLocal2GlobalInQxx.Count);
			try
			{
				MathExtension.inv(Qzz);
				for (int k = 0; k < idxAddParamLocal2GlobalInQxx.Count; k++)
				{
					for (int j = 0; j < idxPointLocal2GlobalInQxx.Count; j++)
					{
						int colM = idxPointLocal2GlobalInQxx[j];
						for (int i = 0; i < idxAddParamLocal2GlobalInQxx.Count; i++)
						{
							int idx = idxAddParamLocal2GlobalInQxx[i];
							double qzznzx = Qzz.get(k, i) * N.get(idx, colM);
							QzzNzx.add(k, j, qzznzx);
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			// In-Situ Invertierung der NGL: N <-- Qxx, n <-- dx 
			MathExtension.solve(N, n, true);
			this.Qxx = N;

			ISet<int> gnssObsIds = new LinkedHashSet<int>();
			for (int i = 0; i < this.projectObservations.size(); i++)
			{
				Observation obs = this.projectObservations.get(i);
				bool isGNSS = obs.ObservationType == ObservationType.GNSS1D || obs.ObservationType == ObservationType.GNSS2D || obs.ObservationType == ObservationType.GNSS3D;
				if (isGNSS && gnssObsIds.Contains(obs.Id))
				{
					continue;
				}

				if (isGNSS)
				{
					gnssObsIds.Add(obs.Id);
				}

				IList<Observation> observations = null;
				if (isGNSS)
				{
					observations = ((GNSSBaseline)obs).BaselineComponents;
				}
				else
				{
					observations = new List<Observation>(1);
					observations.Add(obs);
				}

				int numOfObs = observations.Count;
				Matrix aRedu = new DenseMatrix(numOfObs, idxPointLocal2GlobalInQxx.Count);
				Matrix aQxx = new DenseMatrix(numOfObs, idxPointLocal2GlobalInQxx.Count);
				Matrix aAdd = new DenseMatrix(numOfObs, idxAddParamLocal2GlobalInQxx.Count);
				double[] weights = new double[numOfObs];

				for (int d = 0; d < numOfObs; d++)
				{
					Observation observation = observations[d];

					// Gewicht der Beobachtung
					weights[d] = 1.0 / observation.StdApriori / observation.StdApriori;

					// Bestimme Zeile aus dem Produkt: A'*Qxx
					for (int column = 0; column < QzzNzx.numColumns(); column++)
					{
						aQxx.set(d, column, this.getAQxxElement(observation, idxPointLocal2GlobalInQxx[column], true));
					}

					// X-Lotabweichung des Standpunktes
					int col = observation.StartPoint.VerticalDeflectionX.ColInJacobiMatrix;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffVerticalDeflectionXs());
					}
					// Y-Lotabweichung des Standpunktes
					col = observation.StartPoint.VerticalDeflectionY.ColInJacobiMatrix;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffVerticalDeflectionYs());
					}
					// X-Lotabweichung des Zielpunktes
					col = observation.EndPoint.VerticalDeflectionX.ColInJacobiMatrix;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffVerticalDeflectionXe());
					}
					// Y-Lotabweichung des Zielpunktes
					col = observation.EndPoint.VerticalDeflectionY.ColInJacobiMatrix;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffVerticalDeflectionYe());
					}

					// Orientierung					
					col = observation.ColInJacobiMatrixFromOrientation;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffOri());
					}

					// Massstab
					col = observation.ColInJacobiMatrixFromScale;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffScale());
					}

					// Additionskonstante
					col = observation.ColInJacobiMatrixFromAdd;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffAdd());
					}

					// Refraktion
					col = observation.ColInJacobiMatrixFromRefCoeff;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffRefCoeff());
					}

					// Rotation X
					col = observation.ColInJacobiMatrixFromRotationX;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffRotX());
					}

					// Rotation Y
					col = observation.ColInJacobiMatrixFromRotationY;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffRotY());
					}

					// Rotation Z
					col = observation.ColInJacobiMatrixFromRotationZ;
					if (col >= 0 && idxAddParamGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxAddParamGlobal2LocalInQxx[col];
						aAdd.set(d, col, observation.diffRotZ());
					}
				}

				// Bestimme um Zusatzunbekannte reduzierten Anteil der Designmatrix A
				aAdd.mult(QzzNzx, aRedu);
				aRedu = aRedu.scale(-1.0);

				for (int d = 0; d < numOfObs; d++)
				{
					Observation observation = observations[d];

					int col = observation.StartPoint.ColInJacobiMatrix;
					int dim = observation.StartPoint.Dimension;

					// Startpunkt
					if (col >= 0 && idxPointGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxPointGlobal2LocalInQxx[col];
						if (dim != 1)
						{
							aRedu.add(d, col++, observation.diffXs());
							aRedu.add(d, col++, observation.diffYs());
						}
						if (dim != 2)
						{
							aRedu.add(d, col++, observation.diffZs());
						}
					}

					col = observation.EndPoint.ColInJacobiMatrix;
					dim = observation.EndPoint.Dimension;

					// Zielpunkt
					if (col >= 0 && idxPointGlobal2LocalInQxx.ContainsKey(col))
					{
						col = idxPointGlobal2LocalInQxx[col];
						if (dim != 1)
						{
							aRedu.add(d, col++, observation.diffXe());
							aRedu.add(d, col++, observation.diffYe());
						}
						if (dim != 2)
						{
							aRedu.add(d, col++, observation.diffZe());
						}
					}
				}

				Matrix AzTQzz = new DenseMatrix(aAdd.numRows(), Qzz.numColumns());
				aAdd.mult(Qzz, AzTQzz);
				Matrix PAzTQzzAzP = new UpperSymmPackMatrix(aAdd.numRows());
				AzTQzz.transBmult(aAdd, PAzTQzzAzP);
				AzTQzz = null;

				Matrix ATQxxBP = new DenseMatrix(numOfObs,numOfObs);
				aQxx.transBmult(aRedu, ATQxxBP);

				for (int r = 0; r < numOfObs; r++)
				{
					for (int c = 0; c < numOfObs; c++)
					{
						ATQxxBP.set(r,c, weights[r] * ATQxxBP.get(r,c));
						PAzTQzzAzP.set(r,c, weights[c] * weights[r] * PAzTQzzAzP.get(r,c));
					}
				}

				if (isGNSS)
				{
					this.ATQxxBP_GNSS_EP[obs.Id] = ATQxxBP;
					this.PAzTQzzAzP_GNSS_EF[obs.Id] = PAzTQzzAzP;
				}
				else
				{
					// Geschaetzte Modellstoerung ist noch nicht bestimmt
					// Sodass hier zunachst nur der Faktor A'QxxArPi
					// zwischengespeichert wird
					obs.InfluenceOnPointPosition = ATQxxBP.get(0, 0);
					obs.InfluenceOnNetworkDistortion = PAzTQzzAzP.get(0, 0);
				}
			}
		}

		/// <summary>
		/// Liefert ein Element des Matrizenprodukts aqxx = AQ<sub>xx</sub>(i,j) </summary>
		/// <param name="observation"> </param>
		/// <param name="column"> </param>
		/// <returns> aqxx </returns>
		private double getAQxxElement(Observation observation, int column)
		{
			return this.getAQxxElement(observation, column, false);
		}
		/// <summary>
		/// Liefert ein Element des Matrizenprodukts aqxx = AQ<sub>xx</sub>(i,j) wahlweise auch ohne Zusatzparameter </summary>
		/// <param name="observation"> </param>
		/// <param name="column"> </param>
		/// <param name="withoutAdditionalParameters"> </param>
		/// <returns> aqxx </returns>
		private double getAQxxElement(Observation observation, int column, bool withoutAdditionalParameters)
		{
			if (column < 0)
			{
				return 0.0;
			}

			double aqxx = 0.0;

			int row = observation.StartPoint.ColInJacobiMatrix;
			int dim = observation.StartPoint.Dimension;

			// Startpunkt
			if (row >= 0)
			{
				if (dim != 1)
				{
					aqxx += observation.diffXs() * this.Qxx.get(row++, column);
					aqxx += observation.diffYs() * this.Qxx.get(row++, column);
				}
				if (dim != 2)
				{
					aqxx += observation.diffZs() * this.Qxx.get(row, column);
				}
			}

			row = observation.EndPoint.ColInJacobiMatrix;
			dim = observation.EndPoint.Dimension;

			// Zielpunkt
			if (row >= 0)
			{
				if (dim != 1)
				{
					aqxx += observation.diffXe() * this.Qxx.get(row++, column);
					aqxx += observation.diffYe() * this.Qxx.get(row++, column);
				}
				if (dim != 2)
				{
					aqxx += observation.diffZe() * this.Qxx.get(row, column);
				}
			}

			if (!withoutAdditionalParameters)
			{
				// Zusatzparameter
				// X-Lotabweichung des Standpunktes
				row = observation.StartPoint.VerticalDeflectionX.ColInJacobiMatrix;
				if (row >= 0)
				{
					aqxx += observation.diffVerticalDeflectionXs() * this.Qxx.get(row, column);
				}
				// Y-Lotabweichung des Standpunktes
				row = observation.StartPoint.VerticalDeflectionY.ColInJacobiMatrix;
				if (row >= 0)
				{
					aqxx += observation.diffVerticalDeflectionYs() * this.Qxx.get(row, column);
				}
				// X-Lotabweichung des Zielpunktes
				row = observation.EndPoint.VerticalDeflectionX.ColInJacobiMatrix;
				if (row >= 0)
				{
					aqxx += observation.diffVerticalDeflectionXe() * this.Qxx.get(row, column);
				}
				// Y-Lotabweichung des Zielpunktes
				row = observation.EndPoint.VerticalDeflectionY.ColInJacobiMatrix;
				if (row >= 0)
				{
					aqxx += observation.diffVerticalDeflectionYe() * this.Qxx.get(row, column);
				}
				// Orientierung
				row = observation.ColInJacobiMatrixFromOrientation;
				if (row >= 0)
				{
					aqxx += observation.diffOri() * this.Qxx.get(row, column);
				}
				// Maßstab
				row = observation.ColInJacobiMatrixFromScale;
				if (row >= 0)
				{
					aqxx += observation.diffScale() * this.Qxx.get(row, column);
				}
				// Additionskonstante
				row = observation.ColInJacobiMatrixFromAdd;
				if (row >= 0)
				{
					aqxx += observation.diffAdd() * this.Qxx.get(row, column);
				}
				// Refraktion
				row = observation.ColInJacobiMatrixFromRefCoeff;
				if (row >= 0)
				{
					aqxx += observation.diffRefCoeff() * this.Qxx.get(row, column);
				}
				// Rotation X
				row = observation.ColInJacobiMatrixFromRotationX;
				if (row >= 0)
				{
					aqxx += observation.diffRotX() * this.Qxx.get(row, column);
				}
				// Rotation Y
				row = observation.ColInJacobiMatrixFromRotationY;
				if (row >= 0)
				{
					aqxx += observation.diffRotY() * this.Qxx.get(row, column);
				}
				// Rotation Z
				row = observation.ColInJacobiMatrixFromRotationZ;
				if (row >= 0)
				{
					aqxx += observation.diffRotZ() * this.Qxx.get(row, column);
				}
			}
			return aqxx;
		}

		/// <summary>
		/// Liefert ein Element der Kofaktormatrix der Beobachtungen (a-post)
		/// qll = Q<sub>ll</sub>(i,j) = AQ<sub>xx</sub>A<sup>T</sup>(i,j) </summary>
		/// <param name="observationOne"> </param>
		/// <param name="observationTwo"> </param>
		/// <returns> qll </returns>
		private double getQllElement(Observation observationOne, Observation observationTwo)
		{

			double qll = 0;

			int col = observationTwo.StartPoint.ColInJacobiMatrix;
			int dim = observationTwo.StartPoint.Dimension;

			// Startpunkt
			if (col >= 0)
			{
				if (dim != 1)
				{
					qll += observationTwo.diffXs() * this.getAQxxElement(observationOne, col++);
					qll += observationTwo.diffYs() * this.getAQxxElement(observationOne, col++);
				}
				if (dim != 2)
				{
					qll += observationTwo.diffZs() * this.getAQxxElement(observationOne, col);
				}
			}

			col = observationTwo.EndPoint.ColInJacobiMatrix;
			dim = observationTwo.EndPoint.Dimension;

			// Endpunkt
			if (col >= 0)
			{
				if (dim != 1)
				{
					qll += observationTwo.diffXe() * this.getAQxxElement(observationOne, col++);
					qll += observationTwo.diffYe() * this.getAQxxElement(observationOne, col++);
				}
				if (dim != 2)
				{
					qll += observationTwo.diffZe() * this.getAQxxElement(observationOne, col);
				}
			}

			// Zusatzparameter
			// X-Lotabweichung des Standpunktes
			col = observationTwo.StartPoint.VerticalDeflectionX.ColInJacobiMatrix;
			if (col >= 0)
			{
				qll += observationTwo.diffVerticalDeflectionXs() * this.getAQxxElement(observationOne, col);
			}
			// Y-Lotabweichung des Standpunktes
			col = observationTwo.StartPoint.VerticalDeflectionY.ColInJacobiMatrix;
			if (col >= 0)
			{
				qll += observationTwo.diffVerticalDeflectionYs() * this.getAQxxElement(observationOne, col);
			}
			// X-Lotabweichung des Zielpunktes
			col = observationTwo.EndPoint.VerticalDeflectionX.ColInJacobiMatrix;
			if (col >= 0)
			{
				qll += observationTwo.diffVerticalDeflectionXe() * this.getAQxxElement(observationOne, col);
			}
			// Y-Lotabweichung des Zielpunktes
			col = observationTwo.EndPoint.VerticalDeflectionY.ColInJacobiMatrix;
			if (col >= 0)
			{
				qll += observationTwo.diffVerticalDeflectionYe() * this.getAQxxElement(observationOne, col);
			}
			// Orientierung
			col = observationTwo.ColInJacobiMatrixFromOrientation;
			if (col >= 0)
			{
				qll += observationTwo.diffOri() * this.getAQxxElement(observationOne, col);
			}
			// Maßstab
			col = observationTwo.ColInJacobiMatrixFromScale;
			if (col >= 0)
			{
				qll += observationTwo.diffScale() * this.getAQxxElement(observationOne, col);
			}
			// Additionskonstante
			col = observationTwo.ColInJacobiMatrixFromAdd;
			if (col >= 0)
			{
				qll += observationTwo.diffAdd() * this.getAQxxElement(observationOne, col);
			}
			// Refraktion
			col = observationTwo.ColInJacobiMatrixFromRefCoeff;
			if (col >= 0)
			{
				qll += observationTwo.diffRefCoeff() * this.getAQxxElement(observationOne, col);
			}
			// Rotation X
			col = observationTwo.ColInJacobiMatrixFromRotationX;
			if (col >= 0)
			{
				qll += observationTwo.diffRotX() * this.getAQxxElement(observationOne, col);
			}
			// Rotation Y
			col = observationTwo.ColInJacobiMatrixFromRotationY;
			if (col >= 0)
			{
				qll += observationTwo.diffRotY() * this.getAQxxElement(observationOne, col);
			}
			// Rotation Z
			col = observationTwo.ColInJacobiMatrixFromRotationZ;
			if (col >= 0)
			{
				qll += observationTwo.diffRotZ() * this.getAQxxElement(observationOne, col);
			}

			return qll;
		}

		public virtual void addSubRedundanceAndCofactor2Deflection(Point deflectionPoint, bool restoreUncertainties)
		{

			VerticalDeflection deflectionX = deflectionPoint.VerticalDeflectionX;
			VerticalDeflection deflectionY = deflectionPoint.VerticalDeflectionY;

			int colX = deflectionX.ColInJacobiMatrix;
			int colY = deflectionY.ColInJacobiMatrix;
			int[] cols = new int[] {colX, colY};

			int dim = cols.Length; // Anz der Lotparameter

			double[] rDiag = new double[dim];
			double sumDiagR = 0;

			//Matrix Qvv = new UpperSymmDenseMatrix(dim);
			//Matrix R   = new DenseMatrix(dim,dim);
			Matrix Qll_P = new DenseMatrix(dim,dim);
			Matrix PR = new UpperSymmPackMatrix(dim);
			Matrix Qnn = new DenseMatrix(dim,dim);
			Vector Pv = new DenseVector(dim);
			Vector Pv0 = new DenseVector(dim);
			Vector nabla = new DenseVector(dim);
			Matrix subPsubPQvvP = new UpperSymmPackMatrix(dim);

			double[] qll = new double[dim];
			double[] qll0 = new double[dim];

			double?[] u = new double?[dim];
			if (this.estimationType == EstimationType.L1NORM && restoreUncertainties)
			{
				if (this.adaptedVerticalDeflectionUncertainties.ContainsKey(deflectionX))
				{
					u[0] = this.adaptedVerticalDeflectionUncertainties[deflectionX];
				}
				if (this.adaptedVerticalDeflectionUncertainties.ContainsKey(deflectionY))
				{
					u[1] = this.adaptedVerticalDeflectionUncertainties[deflectionY];
				}
			}

			int diag = 0;
			double vx = (deflectionX.Value - deflectionX.Value0);
			double vy = (deflectionY.Value - deflectionY.Value0);

			qll[diag] = deflectionX.StdApriori * deflectionX.StdApriori;
			qll0[diag] = u[diag] == null ? qll[diag] : u[diag].Value;
			deflectionX.Omega = vx * vx / qll0[diag];
			Pv0.set(diag, vx / qll0[diag]);
			Pv.set(diag, vx / qll[diag++]);

			qll[diag] = deflectionY.StdApriori * deflectionY.StdApriori;
			qll0[diag] = u[diag] == null ? qll[diag] : u[diag].Value;
			deflectionY.Omega = vy * vy / qll0[diag];
			Pv0.set(diag, vy / qll0[diag]);
			Pv.set(diag, vy / qll[diag++]);

			for (int i = 0; i < dim; i++)
			{
				for (int j = i; j < dim; j++)
				{
					double qxx = this.Qxx.get(cols[i], cols[j]);
					if (i == j)
					{
						double qvv = qll[i] - qxx;

						//Qll(a-post) kann aus num. gruenden mal groesser als Qll(aprio) werden
						//In diesem Fall wird die Diagonale Qvv negativ, was nicht zulassig ist
						//Liegen Qll(a-post) und Qll(aprio) sehr dich beisammen, wird Qvv sehr klein
						//Dies kann zu einer Verzerrung von R führen. 
						qvv = Math.Abs(qvv) < Constant.EPS || qvv < 0 ? 0.0:qvv;

						//Qvv.set(i,i, qll[i] - qxx);
						//double rr = 1.0/qll[i]*Qvv.get(i,i);
						double rr = 1.0 / qll[i] * qvv;
						rDiag[i] = rr;
						sumDiagR += rr;
						//R.set(i,i, rr);
						Qll_P.set(i,i, qxx / qll[i]);
						PR.set(i,i, 1.0 / qll[i] * rr);
					}
					else
					{
						double qvv = -qxx;

						//Qll(a-post) kann aus num. gruenden mal groesser als Qll(aprio) werden
						//In diesem Fall wird die Diagonale Qvv negativ, was nicht zulassig ist
						//Liegen Qll(a-post) und Qll(aprio) sehr dich beisammen, wird Qvv sehr klein
						//Dies kann zu einer Verzerrung von R führen. 
						qvv = Math.Abs(qvv) < Constant.EPS?0.0:qvv;

						//Qvv.set(i, j, -qxx);
						//double rrIJ = 1.0/qll[i]*Qvv.get(i,j);
						double rrIJ = 1.0 / qll[i] * qvv;
						//R.set(j,i, rrIJ);
						Qll_P.set(j,i, qxx / qll[i]);
						PR.set(i,j, 1.0 / qll[j] * rrIJ);
						//double rrJI = 1.0/qll[j]*Qvv.get(i,j);
						//double rrJI = 1.0/qll[j]*qvv;
						//R.set(i,j, rrJI);
						Qll_P.set(i,j, qxx / qll[j]);
						//RP.set(j,i, 1.0/qll[i]*rrJI);
					}
				}
			}
			subPsubPQvvP = new UpperSymmPackMatrix(PR);
			subPsubPQvvP.scale(-1.0);
			for (int d = 0; d < dim; d++)
			{
				subPsubPQvvP.add(d, d, 1.0 / qll[d]);
			}

			deflectionX.Redundancy = rDiag[0];
			deflectionY.Redundancy = rDiag[1];

			this.degreeOfFreedom_Conflict += sumDiagR;
			this.omega += deflectionX.Omega + deflectionY.Omega;

			VarianceComponentType vcType = VarianceComponentType.STOCHASTIC_DEFLECTION_COMPONENT;
			if (vcType != null && this.varianceComponents.ContainsKey(vcType))
			{
				VarianceComponent vc = this.varianceComponents[vcType];
				vc.Omega = vc.Omega + deflectionX.Omega + deflectionY.Omega;
				vc.Redundancy = vc.Redundancy + sumDiagR;
				vc.NumberOfObservations = vc.NumberOfObservations + 2;
			}

			if (sumDiagR > SQRT_EPS)
			{
				bool isCalculated = false;
				ConfidenceRegion confidenceRegion = null;
				try
				{
					Qnn = MathExtension.pinv(PR, -1);
					confidenceRegion = new ConfidenceRegion(Qnn);
					isCalculated = true;
				}
				catch (NotConvergedException nce)
				{
					isCalculated = false;
					Console.WriteLine(nce.ToString());
					Console.Write(nce.StackTrace);
				}

				if (isCalculated)
				{
					if (this.estimationType == EstimationType.SIMULATION)
					{
						// Nichtzentralitaetsparameter ist noch nicht bestimmt, 
						// sodass GRZW ein vorlaeufiger Wert ist, 
						// der nabla*Pnn*nabla == 1 erfuellt.
						Vector nabla0 = new DenseVector(dim);
						for (int j = 0; j < dim; j++)
						{
							nabla0.set(j, confidenceRegion.getMinimalDetectableBias(j));
						}

						Vector subPsubPQvvPNabla0 = new DenseVector(dim);
						subPsubPQvvP.mult(nabla0, subPsubPQvvPNabla0);

						//Qll_P.mult(nabla0, ep);

						//deflectionX.setMinimalDetectableBias(nabla0.get(0));
						//deflectionY.setMinimalDetectableBias(nabla0.get(1));
						deflectionX.MaximumTolerableBias = nabla0.get(0);
						deflectionY.MaximumTolerableBias = nabla0.get(1);
					}
					else
					{
						Qnn.mult(Pv, nabla);
						double normNabla = nabla.norm(Vector.Norm.Two);
						double nablaCoVarNable = Math.Abs(nabla.dot(Pv0));
						((VerticalDeflectionX)deflectionX).NablaCoVarNabla = nablaCoVarNable;

						nabla = nabla.scale(-1.0);
						deflectionX.GrossError = nabla.get(0);
						deflectionY.GrossError = nabla.get(1);

						//Qll_P.mult(nabla, ep);

						Vector subPsubPQvvPNabla = new DenseVector(dim);
						subPsubPQvvP.mult(nabla, subPsubPQvvPNabla);

						// Bestimme Nabla auf der Grenzwertellipse mit nabla0*Pnn*nabla0 == 1
						Vector nabla0 = new DenseVector(nabla, true);
						Vector PQvvPnabla0 = new DenseVector(nabla0);
						PR.mult(nabla0, PQvvPnabla0);
						double nQn0 = nabla0.dot(PQvvPnabla0);
						if (normNabla < SQRT_EPS || nQn0 <= 0)
						{
							for (int j = 0; j < dim; j++)
							{
								nabla0.set(j, confidenceRegion.getMinimalDetectableBias(j));
							}
						}
						else
						{
							for (int j = 0; j < dim; j++)
							{
								nabla0.set(j, nabla0.get(j) / Math.Sqrt(nQn0));
							}
						}
						//deflectionX.setMinimalDetectableBias(nabla0.get(0));
						//deflectionY.setMinimalDetectableBias(nabla0.get(1));
						deflectionX.MaximumTolerableBias = nabla0.get(0);
						deflectionY.MaximumTolerableBias = nabla0.get(1);
					}
				}
			}
			if (restoreUncertainties && sumDiagR > SQRT_EPS)
			{
				this.numberOfHypotesis++;
			}
		}

		/// <summary>
		/// Berechnet die Elemente der Kofaktormatrix der Punktesbeobachtung
		/// nach der Ausgleichung aus <code>Q_ll(i,i) = A*Qxx*A'</code>, fuer i von 1 bis point.getDimension()
		/// 
		/// Berechnet die Elemente der Kofaktormatrix der Verbesserungen fuer einen Punkt
		/// <code>Q<sub>vv</sub>(i,i) = P<sup>-1</sup> - Q<sub>ll</sub></code>
		/// 
		/// Berechnet die Elemente der Redundanzmatrix der Punktbeobachtung
		/// nach der Ausgleichung aus <code>R(i,i) = P * Q<sub>vv</sub></code>
		/// 
		/// Ergaenzt die Punktbeobachtung durch ihre Kofaktoren qll und
		/// ihren Redundanzanteil r, dieser wird gleichzeitig 
		/// zur Gesamtredundanz benutzt. Ferner wird &Omega; der Gesamtausgleichung
		/// berechnet und das fuer den statistischen Test benoetigte Produkt &nabla;Q<sub>&nabla;&nabla;</sub>&nabla;,
		/// welches ebenfalls der Punktbeobachtungen uebergeben wird.
		/// 
		/// 
		/// <strong>HINWEIS</strong> !!!Das doppelte Aufrufen der Methode fuert somit zu
		/// falschen Ergebnissen beim Redundanzanteil und &Omega;. Die a-priori 
		/// Standardabweichung wird durch den Kofaktor ueberschrieben.!!!  
		/// </summary>
		/// <param name="point"> </param>
		/// <param name="restoreUncertainties"> </param>
		public virtual void addSubRedundanceAndCofactor2Point(Point point, bool restoreUncertainties)
		{
			int dim = point.Dimension;
			int col = point.ColInJacobiMatrix;
			double[] rDiag = new double[dim];
			double sumDiagR = 0;
			double omegaPoint = 0.0;
			//Matrix Qvv = new UpperSymmDenseMatrix(dim);
			//Matrix R   = new DenseMatrix(dim,dim);
			Matrix Qll_P = new DenseMatrix(dim,dim);
			Matrix PR = new UpperSymmPackMatrix(dim);
			Matrix Qnn = new DenseMatrix(dim,dim);
			Vector Pv = new DenseVector(dim);
			Vector Pv0 = new DenseVector(dim);
			Vector nabla = new DenseVector(dim);
			Vector ep = new DenseVector(dim);
			Matrix subPsubPQvvP = new UpperSymmPackMatrix(dim);

			double[] qll = new double[dim];
			double[] qll0 = new double[dim];
			double?[] u = this.estimationType == EstimationType.L1NORM && restoreUncertainties && this.adaptedPointUncertainties.ContainsKey(point) ? this.adaptedPointUncertainties[point] : new double?[dim];
			int diag = 0;
			if (dim != 1)
			{
				double vx = (point.X - point.X0);
				double vy = (point.Y - point.Y0);

				qll[diag] = point.StdXApriori * point.StdXApriori;
				qll0[diag] = u[diag] == null ? qll[diag] : u[diag].Value;
				omegaPoint += (vx * vx / qll0[diag]);
				Pv0.set(diag, vx / qll0[diag]);
				Pv.set(diag, vx / qll[diag++]);

				qll[diag] = point.StdYApriori * point.StdYApriori;
				qll0[diag] = u[diag] == null ? qll[diag] : u[diag].Value;
				omegaPoint += (vy * vy / qll0[diag]);
				Pv0.set(diag, vy / qll0[diag]);
				Pv.set(diag, vy / qll[diag++]);
			}
			if (dim != 2)
			{
				double vz = (point.Z - point.Z0);

				qll[diag] = point.StdZApriori * point.StdZApriori;
				qll0[diag] = u[diag] == null ? qll[diag] : u[diag].Value;
				omegaPoint += (vz * vz / qll0[diag]);
				Pv0.set(diag, vz / qll0[diag]);
				Pv.set(diag, vz / qll[diag++]);
			}

			for (int i = 0; i < dim; i++)
			{
				for (int j = i; j < dim; j++)
				{
					double qxx = this.Qxx.get(col + i, col + j);
					if (i == j)
					{
						double qvv = qll[i] - qxx;

						//Qll(a-post) kann aus num. gruenden mal groesser als Qll(aprio) werden
						//In diesem Fall wird die Diagonale Qvv negativ, was nicht zulassig ist
						//Liegen Qll(a-post) und Qll(aprio) sehr dich beisammen, wird Qvv sehr klein
						//Dies kann zu einer Verzerrung von R führen. 
						qvv = Math.Abs(qvv) < Constant.EPS || qvv < 0 ? 0.0:qvv;

						//Qvv.set(i,i, qll[i] - qxx);
						//double rr = 1.0/qll[i]*Qvv.get(i,i);
						double rr = 1.0 / qll[i] * qvv;
						rDiag[i] = rr;
						sumDiagR += rr;
						//R.set(i,i, rr);
						Qll_P.set(i,i, qxx / qll[i]);
						PR.set(i,i, 1.0 / qll[i] * rr);
					}
					else
					{
						double qvv = -qxx;

						//Qll(a-post) kann aus num. gruenden mal groesser als Qll(aprio) werden
						//In diesem Fall wird die Diagonale Qvv negativ, was nicht zulassig ist
						//Liegen Qll(a-post) und Qll(aprio) sehr dich beisammen, wird Qvv sehr klein
						//Dies kann zu einer Verzerrung von R führen. 
						qvv = Math.Abs(qvv) < Constant.EPS?0.0:qvv;

						//Qvv.set(i, j, -qxx);
						//double rrIJ = 1.0/qll[i]*Qvv.get(i,j);
						double rrIJ = 1.0 / qll[i] * qvv;
						//R.set(j,i, rrIJ);
						Qll_P.set(j,i, qxx / qll[i]);
						PR.set(i,j, 1.0 / qll[j] * rrIJ);
						//double rrJI = 1.0/qll[j]*Qvv.get(i,j);
						//double rrJI = 1.0/qll[j]*qvv;
						//R.set(i,j, rrJI);
						Qll_P.set(i,j, qxx / qll[j]);
						//RP.set(j,i, 1.0/qll[i]*rrJI);
					}
				}
			}
			subPsubPQvvP = new UpperSymmPackMatrix(PR);
			subPsubPQvvP.scale(-1.0);
			for (int d = 0; d < dim; d++)
			{
				subPsubPQvvP.add(d, d, 1.0 / qll[d]);
			}

			point.Redundancy = rDiag;
			this.degreeOfFreedom_Conflict += sumDiagR;
			point.Omega = omegaPoint;
			this.omega += omegaPoint;

			VarianceComponentType vcType = VarianceComponentType.getComponentTypeByPointDimension(dim);
			if (vcType != null && this.varianceComponents.ContainsKey(vcType))
			{
				VarianceComponent vc = this.varianceComponents[vcType];
				vc.Omega = vc.Omega + omegaPoint;
				vc.Redundancy = vc.Redundancy + sumDiagR;
				vc.NumberOfObservations = vc.NumberOfObservations + point.Dimension;
			}

			if (sumDiagR > SQRT_EPS)
			{
				bool isCalculated = false;
				ConfidenceRegion confidenceRegion = null;
				try
				{
					Qnn = MathExtension.pinv(PR, -1);
					confidenceRegion = new ConfidenceRegion(Qnn);
					isCalculated = true;
				}
				catch (NotConvergedException nce)
				{
					isCalculated = false;
					Console.WriteLine(nce.ToString());
					Console.Write(nce.StackTrace);
				}

				if (isCalculated)
				{
					if (this.estimationType == EstimationType.SIMULATION)
					{
						// Nichtzentralitaetsparameter ist noch nicht bestimmt, 
						// sodass GRZW ein vorlaeufiger Wert ist, 
						// der nabla*Pnn*nabla == 1 erfuellt.
						Vector nabla0 = new DenseVector(dim);
						for (int j = 0; j < dim; j++)
						{
							nabla0.set(j, confidenceRegion.getMinimalDetectableBias(j));
						}

						Vector subPsubPQvvPNabla0 = new DenseVector(dim);
						subPsubPQvvP.mult(nabla0, subPsubPQvvPNabla0);
						double efsp = Math.Sqrt(Math.Abs(subPsubPQvvPNabla0.dot(nabla0)));
	//					Vector PQvvPnabla0 = new DenseVector(nabla0);
	//					BTPQvvPB.mult(nabla0, PQvvPnabla0);
	//					double nQn0 = nabla0.dot(PQvvPnabla0);
	//					for (int j=0; j<dim; j++)
	//						if (nQn0 > 0)
	//							nabla0.set(j, nabla0.get(j)/Math.sqrt(nQn0));
						Qll_P.mult(nabla0, ep);
						//point.setMinimalDetectableBiases( Matrices.getArray(nabla0) );
						point.MaximumTolerableBiases = Matrices.getArray(nabla0);
						point.InfluencesOnPointPosition = Matrices.getArray(ep);
						point.InfluenceOnNetworkDistortion = efsp;
					}
					else
					{
						Qnn.mult(Pv, nabla);
						point.NablaCoVarNabla = Math.Abs(nabla.dot(Pv0));
						nabla = nabla.scale(-1.0);
						point.GrossErrors = Matrices.getArray(nabla);
						Qll_P.mult(nabla, ep);
						point.InfluencesOnPointPosition = Matrices.getArray(ep);

						Vector subPsubPQvvPNabla = new DenseVector(dim);
						subPsubPQvvP.mult(nabla, subPsubPQvvPNabla);
						double efsp = Math.Sqrt(Math.Abs(subPsubPQvvPNabla.dot(nabla)));

						// Bestimme Nabla auf der Grenzwertellipse mit nabla0*Pnn*nabla0 == 1
						Vector nabla0 = new DenseVector(nabla, true);
						Vector PQvvPnabla0 = new DenseVector(nabla0);
						PR.mult(nabla0, PQvvPnabla0);
						double nQn0 = nabla0.dot(PQvvPnabla0);
						double normNabla = nabla.norm(Vector.Norm.Two);
						if (normNabla < SQRT_EPS || nQn0 <= 0)
						{
							for (int j = 0; j < dim; j++)
							{
								nabla0.set(j, confidenceRegion.getMinimalDetectableBias(j));
							}
						}
						else
						{
							for (int j = 0; j < dim; j++)
							{
								nabla0.set(j, nabla0.get(j) / Math.Sqrt(nQn0));
							}
						}
						//point.setMinimalDetectableBiases(Matrices.getArray(nabla0));
						point.MaximumTolerableBiases = Matrices.getArray(nabla0);
						point.InfluenceOnNetworkDistortion = efsp;
					}
				}
			}
			if (restoreUncertainties && sumDiagR > SQRT_EPS)
			{
				this.numberOfHypotesis++;
			}
		}

		/// <summary>
		/// Berechnet die Elemente der Kofaktormatrix der Beobachtung
		/// nach der Ausgleichung aus <code>Q_ll(i,i) = A*Qxx*A'</code>, fuer i = 1
		/// 
		/// Berechnet die Elemente der Kofaktormatrix der Verbesserungen fuer eine Beobachtung
		/// <code>Q<sub>vv</sub>(i,i) = P<sup>-1</sup> - Q<sub>ll</sub></code>
		/// 
		/// Berechnet die Elemente der Redundanzmatrix der Beobachtung
		/// nach der Ausgleichung aus <code>R(i,i) = P * Q<sub>vv</sub></code>
		/// 
		/// Ergaenzt die Beobachtung durch den Kofaktor qll und 
		/// ihren Redundanzanteil r, dieser wird gleichzeitig 
		/// zur Gesamtredundanz benutzt. Ferner wird &Omega; der Gesamtausgleichung
		/// berechnet und das fuer den statistischen Test benoetigte Produkt &nabla;Q<sub>&nabla;&nabla;</sub>&nabla;,
		/// welches ebenfalls der Beobachtungen uebergeben wird.
		/// 
		/// <strong>HINWEIS</strong> !!!Das doppelte Aufrufen der Methode fuert somit zu
		/// falschen ergebnissen beim Redundanzanteil und &Omega;. Die a-priori Standardabweichung
		/// wird durch den Kofaktor ueberschrieben.!!!  
		/// </summary>
		public virtual void addSubRedundanceAndCofactor(Observation observation)
		{
			bool isGNSS = observation.ObservationType == ObservationType.GNSS1D || observation.ObservationType == ObservationType.GNSS2D || observation.ObservationType == ObservationType.GNSS3D;
			IList<Observation> observations = null;
			if (isGNSS)
			{
				observations = ((GNSSBaseline)observation).BaselineComponents;
			}
			else
			{
				observations = new List<Observation>(1);
				observations.Add(observation);
			}

			int dim = observations.Count;
			Matrix aRows = new DenseMatrix(dim, this.numberOfUnknownParameters);

			// Punkte der Beobachtung
			Point startPoint = observation.StartPoint;
			Point endPoint = observation.EndPoint;

			// Hole Indizes in Jacobi-Matrix und speichere in Liste
			ISet<int> colums = new LinkedHashSet<int>(15);

			for (int row = 0; row < dim; row++)
			{
				Observation obs = observations[row];
				int colASp = startPoint.ColInJacobiMatrix;
				int colAEp = endPoint.ColInJacobiMatrix;

				if (colASp >= 0)
				{
					if (startPoint.Dimension != 1)
					{
						colums.Add(colASp);
						aRows.set(row, colASp++, obs.diffXs());
						colums.Add(colASp);
						aRows.set(row, colASp++, obs.diffYs());
					}

					if (startPoint.Dimension != 2)
					{
						colums.Add(colASp);
						aRows.set(row, colASp, obs.diffZs());
					}
				}
				if (colAEp >= 0)
				{
					if (endPoint.Dimension != 1)
					{
						colums.Add(colAEp);
						aRows.set(row, colAEp++, obs.diffXe());
						colums.Add(colAEp);
						aRows.set(row, colAEp++, obs.diffYe());
					}

					if (endPoint.Dimension != 2)
					{
						colums.Add(colAEp);
						aRows.set(row, colAEp, obs.diffZe());
					}
				}
				// mgl. Zusatzparameter
				if (obs.StartPoint.VerticalDeflectionX.ColInJacobiMatrix >= 0)
				{
					colums.Add(obs.StartPoint.VerticalDeflectionX.ColInJacobiMatrix);
					aRows.set(row, obs.StartPoint.VerticalDeflectionX.ColInJacobiMatrix, obs.diffVerticalDeflectionXs());
				}
				if (obs.StartPoint.VerticalDeflectionY.ColInJacobiMatrix >= 0)
				{
					colums.Add(obs.StartPoint.VerticalDeflectionY.ColInJacobiMatrix);
					aRows.set(row, obs.StartPoint.VerticalDeflectionY.ColInJacobiMatrix, obs.diffVerticalDeflectionYs());
				}
				if (obs.EndPoint.VerticalDeflectionX.ColInJacobiMatrix >= 0)
				{
					colums.Add(obs.EndPoint.VerticalDeflectionX.ColInJacobiMatrix);
					aRows.set(row, obs.EndPoint.VerticalDeflectionX.ColInJacobiMatrix, obs.diffVerticalDeflectionXe());
				}
				if (obs.EndPoint.VerticalDeflectionY.ColInJacobiMatrix >= 0)
				{
					colums.Add(obs.EndPoint.VerticalDeflectionY.ColInJacobiMatrix);
					aRows.set(row, obs.EndPoint.VerticalDeflectionY.ColInJacobiMatrix, obs.diffVerticalDeflectionYe());
				}

				if (obs.ColInJacobiMatrixFromOrientation >= 0)
				{
					colums.Add(obs.ColInJacobiMatrixFromOrientation);
					aRows.set(row, obs.ColInJacobiMatrixFromOrientation, obs.diffOri());
				}

				if (obs.ColInJacobiMatrixFromAdd >= 0)
				{
					colums.Add(obs.ColInJacobiMatrixFromAdd);
					aRows.set(row, obs.ColInJacobiMatrixFromAdd, obs.diffAdd());
				}

				if (obs.ColInJacobiMatrixFromScale >= 0)
				{
					colums.Add(obs.ColInJacobiMatrixFromScale);
					aRows.set(row, obs.ColInJacobiMatrixFromScale, obs.diffScale());
				}

				if (obs.ColInJacobiMatrixFromRefCoeff >= 0)
				{
					colums.Add(obs.ColInJacobiMatrixFromRefCoeff);
					aRows.set(row, obs.ColInJacobiMatrixFromRefCoeff, obs.diffRefCoeff());
				}

				if (obs.ColInJacobiMatrixFromRotationX >= 0)
				{
					colums.Add(obs.ColInJacobiMatrixFromRotationX);
					aRows.set(row, obs.ColInJacobiMatrixFromRotationX, obs.diffRotX());
				}

				if (obs.ColInJacobiMatrixFromRotationY >= 0)
				{
					colums.Add(obs.ColInJacobiMatrixFromRotationY);
					aRows.set(row, obs.ColInJacobiMatrixFromRotationY, obs.diffRotY());
				}

				if (obs.ColInJacobiMatrixFromRotationZ >= 0)
				{
					colums.Add(obs.ColInJacobiMatrixFromRotationZ);
					aRows.set(row, obs.ColInJacobiMatrixFromRotationZ, obs.diffRotZ());
				}
			}

			Matrix subR = new DenseMatrix(dim,dim);

			// Bestimme Qvv

			// Berechne A*Qxx und Q*Qxx*A'
			for (int dc = 0; dc < dim; dc++)
			{
				foreach (int? col in colums)
				{
	//			for (int c=0; c<colums.size(); c++) {
	//				int col = colums.get(c);
					double aqxx = 0.0;
					foreach (int? row in colums)
					{
	//				for (int r=0; r<colums.size(); r++) {
	//					int row = colums.get(r);
						aqxx += aRows.get(dc, row) * this.Qxx.get(row, col);
					}

					for (int dr = 0; dr < dim; dr++)
					{
						subR.set(dr, dc, subR.get(dr, dc) + aqxx * aRows.get(dr, col));
					}
				}
			}
			// subRP == Q_ll; Drehe Vorzeichen, da Q_vv = Qll - Q_ll --> Mit Qll als Diagonalmatrix entspricht Q_vv = -Q_ll fuer i != j
			subR = subR.scale(-1.0);

			// Berechne R = Qvv*P
			double rr = 0;
			for (int i = 0; i < dim; i++)
			{
				Observation obs = observations[i];
				double qll = obs.StdApriori * obs.StdApriori;
				double q_ll = Math.Abs(subR.get(i,i)); // Negativ, da Vorzeichen gedreht
				obs.Std = Math.Sqrt(q_ll);
				double qvv = qll - q_ll;
				//Qll(a-post) kann aus num. gruenden mal groesser als Qll(aprio) werden
				//In diesem Fall wird die Diagonale Qvv negativ, was nicht zulassig ist
				//Liegen Qll(a-post) und Qll(aprio) sehr dich beisammen, wird Qvv sehr klein
				//dies kann zu einer Verzerrung von R führen.
				qvv = Math.Abs(qvv) < Constant.EPS || qvv < 0 ? 0.0:qvv;
				subR.set(i,i, qvv);

				for (int j = 0; j < dim; j++)
				{
					// R
					double r = 1.0 / qll * subR.get(j,i);
					subR.set(j,i, r);
					if (i == j)
					{
						obs.Redundancy = r;
						rr += r;
					}

					// P*R
					//subRP.set(i,j, 1.0/qll * subRP.get(i,j));
				}
			}

	//		// Berechne RP = PQvvP
	//		for (int i=0; i<dim; i++) {
	//			Observation obs = observations.get(i);
	//			double qll = obs.getStdApriori()*obs.getStdApriori();
	//				
	//			for (int j=0; j<dim; j++) {
	//				subRP.set(i,j, 1.0/qll * subRP.get(i,j));
	//			}
	//		}

			if (isGNSS)
			{
				((GNSSBaseline)observation).BaselineRedundancyMatrix = subR;
			}

			if (rr > SQRT_EPS)
			{
				this.numberOfHypotesis++;
			}

			this.degreeOfFreedom_Conflict += rr;
		}

		private void estimateRobustWeights()
		{
			double maxNV2 = double.Epsilon;
			Observation maxNVObs = null;
			Point maxNVPoint = null;
			VerticalDeflection maxNVVerticalDeflection = null;
			int maxNVPointComp = -1;
			int maxNVVerticalDeflectionComp = -1;
			int counter = 0;
			// Bestimme Beobachtung mit groesster NV
			for (int i = 0; i < this.numberOfObservations; i++)
			{
				Observation observation = this.projectObservations.get(i);
				double u = observation.StdApriori;
				double qll = u * u;
				double r = observation.Redundancy;
				double v = this.estimationType == EstimationType.SIMULATION ? 0.0 : observation.Correction;

				if (qll > 0.0 && r > SQRT_EPS)
				{
					double nv2 = v * v / qll / r;
					if (nv2 > maxNV2)
					{
						maxNV2 = nv2;
						maxNVObs = observation;
					}
				}

				// Pruefe, ob Grenzwert fuer robustes Intervall angepasst werden sollte
				if (Math.Abs(v) >= 500.0 * u)
				{
					counter++;
				}
			}

			// Bestimme stochastischen Punkt mit groesster NV
			foreach (Point point in this.pointsWithStochasticDeflection)
			{
				VerticalDeflection deflectionX = point.VerticalDeflectionX;
				VerticalDeflection deflectionY = point.VerticalDeflectionY;

				double vx = deflectionX.Value0 - deflectionX.Value;
				double vy = deflectionY.Value0 - deflectionY.Value;

				double rx = deflectionX.Redundancy;
				double ry = deflectionY.Redundancy;

				double ux = deflectionX.StdApriori;
				double uy = deflectionY.StdApriori;

				double qx = ux * ux;
				double qy = uy * uy;

				if (qx > 0 && rx >= SQRT_EPS)
				{
					double nv2 = vx * vx / qx / rx;
					if (nv2 > maxNV2)
					{
						maxNV2 = nv2;
						maxNVVerticalDeflection = deflectionX;
						maxNVVerticalDeflectionComp = 1;
					}
				}

				if (qy > 0 && ry >= SQRT_EPS)
				{
					double nv2 = vy * vy / qy / ry;
					if (nv2 > maxNV2)
					{
						maxNV2 = nv2;
						maxNVVerticalDeflection = deflectionY;
						maxNVVerticalDeflectionComp = 2;
					}
				}

				if (Math.Abs(vx) >= 500.0 * ux)
				{
					counter++;
				}

				if (Math.Abs(vy) > 500.0 * uy)
				{
					counter++;
				}

			}
			foreach (Point point in this.stochasticPoints)
			{
				int dim = point.Dimension;
				for (int d = 0; d < dim; d++)
				{
					double v = 0, qll = 0, r = 0, u = 0;
					if (dim != 1)
					{
						if (d == 0)
						{
							v = point.X0 - point.X;
							u = point.StdXApriori;
							qll = u * u;
							r = point.RedundancyX;
						}
						else if (d == 1)
						{
							v = point.Y0 - point.Y;
							u = point.StdYApriori;
							qll = u * u;
							r = point.RedundancyY;
						}
					}
					if (dim != 2 && d == dim - 1)
					{
						v = point.Z0 - point.Z;
						u = point.StdZApriori;
						qll = u * u;
						r = point.RedundancyZ;
					}

					if (qll > 0.0 && r >= SQRT_EPS)
					{
						double nv2 = v * v / qll / r;
						if (nv2 > maxNV2)
						{
							maxNV2 = nv2;
							maxNVPoint = point;
							maxNVPointComp = d;

							maxNVVerticalDeflection = null;
							maxNVVerticalDeflectionComp = -1;
						}
					}

					if (Math.Abs(v) >= 500.0 * u)
					{
						counter++;
					}
				}
			}
			bool adapteRobustBoundary = (double)counter / (double)(this.numberOfObservations + this.numberOfStochasticPointRows + this.numberOfStochasticDeflectionRows) > 0.35;
			double c = adapteRobustBoundary ? this.robustEstimationLimit + (Math.Sqrt(maxNV2) - this.robustEstimationLimit) * 0.9 : this.robustEstimationLimit;

			// Wenn maxNVPoint == null und maxNVDeflection == null, dann wurde eine Beobachtung mit max(NV) gefunden
			if (maxNVObs != null && maxNVPoint == null && maxNVVerticalDeflection == null)
			{
				double u = maxNVObs.StdApriori;
				double r = maxNVObs.Redundancy;
				double k = c * u * Math.Sqrt(r);
				double v = this.estimationType == EstimationType.SIMULATION ? 0.0 : Math.Abs(maxNVObs.Correction);

				if (v >= k && k > SQRT_EPS)
				{
					maxNVObs.StdApriori = u * Math.Sqrt(v / k);
					if (!this.adaptedObservationUncertainties.ContainsKey(maxNVObs))
					{
						this.adaptedObservationUncertainties[maxNVObs] = u;
					}
				}
			}
			// Wenn maxNVVerticalDeflectionComp != -1, dann wurde eine Lotabweichungskomponente mit max(NV) gefunden
			else if (maxNVVerticalDeflection != null && maxNVVerticalDeflectionComp >= 0)
			{
				// X-Wert
				double u = maxNVVerticalDeflection.StdApriori;
				double r = maxNVVerticalDeflection.Redundancy;
				double k = c * u * Math.Sqrt(r);
				double v = this.estimationType == EstimationType.SIMULATION ? 0.0 : Math.Abs(maxNVVerticalDeflection.Value0 - maxNVVerticalDeflection.Value);
				if (v >= k && k > SQRT_EPS)
				{
					maxNVVerticalDeflection.StdApriori = u * Math.Sqrt(v / k);
					if (!this.adaptedVerticalDeflectionUncertainties.ContainsKey(maxNVVerticalDeflection))
					{
						this.adaptedVerticalDeflectionUncertainties[maxNVVerticalDeflection] = u;
					}
				}
			}
			// Wenn maxNVPointComp != -1, dann wurde eine Koordinatenkomponente mit max(NV) gefunden
			else if (maxNVPoint != null && maxNVPointComp >= 0)
			{
				int dim = maxNVPoint.Dimension;
				double?[] u = new double?[dim];

				for (int d = 0; d < dim; d++)
				{
					double v = 0, k = 0, r = 0;
					if (d == dim - 1 && dim != 2)
					{
						u[d] = maxNVPoint.StdZApriori;
						if (maxNVPointComp == d)
						{
							r = maxNVPoint.RedundancyZ;
							k = c * u[d].Value * Math.Sqrt(r);
							v = this.estimationType == EstimationType.SIMULATION ? 0.0 : Math.Abs(maxNVPoint.Z0 - maxNVPoint.Z);
							if (v >= k && k > SQRT_EPS)
							{
								maxNVPoint.StdZApriori = u[d].Value * Math.Sqrt(v / k);
								if (!this.adaptedPointUncertainties.ContainsKey(maxNVPoint))
								{
									this.adaptedPointUncertainties[maxNVPoint] = u;
								}
							}
						}
					}
					else if (d == 0 && dim != 1)
					{
						u[d] = maxNVPoint.StdXApriori;
						if (maxNVPointComp == d)
						{
							r = maxNVPoint.RedundancyX;
							k = c * u[d].Value * Math.Sqrt(r);
							v = this.estimationType == EstimationType.SIMULATION ? 0.0 : Math.Abs(maxNVPoint.X0 - maxNVPoint.X);
							if (v >= k && k > SQRT_EPS)
							{
								maxNVPoint.StdXApriori = u[d].Value * Math.Sqrt(v / k);
								if (!this.adaptedPointUncertainties.ContainsKey(maxNVPoint))
								{
									this.adaptedPointUncertainties[maxNVPoint] = u;
								}
							}
						}
					}
					else if (d == 1 && dim != 1)
					{
						u[d] = maxNVPoint.StdYApriori;
						if (maxNVPointComp == d)
						{
							r = maxNVPoint.RedundancyY;
							k = c * u[d].Value * Math.Sqrt(r);
							v = this.estimationType == EstimationType.SIMULATION ? 0.0 : Math.Abs(maxNVPoint.Y0 - maxNVPoint.Y);
							if (v >= k && k > SQRT_EPS)
							{
								maxNVPoint.StdYApriori = u[d].Value * Math.Sqrt(v / k);
								if (!this.adaptedPointUncertainties.ContainsKey(maxNVPoint))
								{
									this.adaptedPointUncertainties[maxNVPoint] = u;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// erzeugt die Normalgleichungsmatrix N = A<sup>T</sup>PA <em>direkt</em>, d.h. ohne
		/// das explizite Aufstellen von A und P
		/// return NEQ
		/// </summary>
		public virtual NormalEquationSystem createNormalEquation()
		{
			int numberOfStrainEquations = 0; // Anzahl der zusaetzlichen Bedingungsgleichungen zur bestimmung der Strain-Parameter

			if (this.freeNetwork && this.congruenceAnalysis)
			{
				foreach (CongruenceAnalysisGroup tieGroup in this.congruenceAnalysisGroup)
				{
					StrainAnalysisEquations strainAnalysisEquations = tieGroup.StrainAnalysisEquations;
					int nou = strainAnalysisEquations.numberOfParameters();
					int nor = strainAnalysisEquations.numberOfRestrictions();
					int not = tieGroup.size(true);
					int dim = tieGroup.Dimension;

					if (strainAnalysisEquations.hasUnconstraintParameters() && not * dim + nor >= nou)
					{
						numberOfStrainEquations += not * dim + nor;
					}
				}
			}

			UpperSymmPackMatrix N = new UpperSymmPackMatrix(this.numberOfUnknownParameters + this.rankDefect.Defect + numberOfStrainEquations);
			DenseVector n = new DenseVector(N.numRows());

			if (this.estimationType == EstimationType.L1NORM)
			{
				this.estimateRobustWeights();
			}

			for (int u = 0; u < this.unknownParameters.size(); u++)
			{
				if (this.interrupt_Conflict)
				{
					return null;
				}

				UnknownParameter unknownParameterAT = this.unknownParameters.get(u);
				ObservationGroup observationGroupAT = unknownParameterAT.Observations;
				int dimAT = 1;
				switch (unknownParameterAT.ParameterType.innerEnumValue)
				{
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT2D:
					dimAT = 2;
					break;
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT3D:
					dimAT = 3;
					break;
				default:
					dimAT = 1;
					break;
				}
				for (int i = 0; i < dimAT; i++)
				{
					int colAT = unknownParameterAT.ColInJacobiMatrix + i;
					Vector aTp = new SparseVector(this.numberOfObservations, observationGroupAT.size());
					for (int j = 0; j < observationGroupAT.size(); j++)
					{
						Observation observationAT = observationGroupAT.get(j);
						int rowAT = observationAT.RowInJacobiMatrix;
						double at = 0.0;

						// Berechnet AT*P
						if (unknownParameterAT.ParameterType == ParameterType.POINT1D)
						{
							Point p = (Point)unknownParameterAT;
							if (p.Equals(observationAT.StartPoint))
							{
								at = observationAT.diffZs();
							}
							else if (p.Equals(observationAT.EndPoint))
							{
								at = observationAT.diffZe();
							}
						}
						else if (unknownParameterAT.ParameterType == ParameterType.POINT2D)
						{
							Point p = (Point)unknownParameterAT;
							if (p.Equals(observationAT.StartPoint))
							{
								if (i == 0)
								{
									at = observationAT.diffXs();
								}
								else if (i == 1)
								{
									at = observationAT.diffYs();
								}
							}
							else if (p.Equals(observationAT.EndPoint))
							{
								if (i == 0)
								{
									at = observationAT.diffXe();
								}
								else if (i == 1)
								{
									at = observationAT.diffYe();
								}
							}
						}
						else if (unknownParameterAT.ParameterType == ParameterType.POINT3D)
						{
							Point p = (Point)unknownParameterAT;
							if (p.Equals(observationAT.StartPoint))
							{
								if (i == 0)
								{
									at = observationAT.diffXs();
								}
								else if (i == 1)
								{
									at = observationAT.diffYs();
								}
								else if (i == 2)
								{
									at = observationAT.diffZs();
								}
							}
							else if (p.Equals(observationAT.EndPoint))
							{
								if (i == 0)
								{
									at = observationAT.diffXe();
								}
								else if (i == 1)
								{
									at = observationAT.diffYe();
								}
								else if (i == 2)
								{
									at = observationAT.diffZe();
								}
							}
						}

						else if (unknownParameterAT.ParameterType == ParameterType.VERTICAL_DEFLECTION_X)
						{
							VerticalDeflectionX deflection = (VerticalDeflectionX)unknownParameterAT;
							Point p = deflection.Point;
							if (p.Equals(observationAT.StartPoint))
							{
								at = observationAT.diffVerticalDeflectionXs();
							}
							else if (p.Equals(observationAT.EndPoint))
							{
								at = observationAT.diffVerticalDeflectionXe();
							}
						}
						else if (unknownParameterAT.ParameterType == ParameterType.VERTICAL_DEFLECTION_Y)
						{
							VerticalDeflectionY deflection = (VerticalDeflectionY)unknownParameterAT;
							Point p = deflection.Point;
							if (p.Equals(observationAT.StartPoint))
							{
								at = observationAT.diffVerticalDeflectionYs();
							}
							else if (p.Equals(observationAT.EndPoint))
							{
								at = observationAT.diffVerticalDeflectionYe();
							}
						}
						else if (unknownParameterAT.ParameterType == ParameterType.ORIENTATION)
						{
							at = observationAT.diffOri();
						}
						else if (unknownParameterAT.ParameterType == ParameterType.ZERO_POINT_OFFSET)
						{
							at = observationAT.diffAdd();
						}
						else if (unknownParameterAT.ParameterType == ParameterType.SCALE)
						{
							at = observationAT.diffScale();
						}
						else if (unknownParameterAT.ParameterType == ParameterType.REFRACTION_INDEX)
						{
							at = observationAT.diffRefCoeff();
						}
						else if (unknownParameterAT.ParameterType == ParameterType.ROTATION_X)
						{
							at = observationAT.diffRotX();
						}
						else if (unknownParameterAT.ParameterType == ParameterType.ROTATION_Y)
						{
							at = observationAT.diffRotY();
						}
						else if (unknownParameterAT.ParameterType == ParameterType.ROTATION_Z)
						{
							at = observationAT.diffRotZ();
						}

						// Zeile aT*p bestimmen
						double atp = at / (observationAT.StdApriori * observationAT.StdApriori);
						aTp.set(rowAT, atp);
						// Absolutgliedvektor bestimmen
						n.add(colAT, atp * observationAT.Correction);
						// Hauptdiagonalelement aT*p*a
						N.add(colAT, colAT, atp * at);
					}

					for (int uu = u; uu < this.unknownParameters.size(); uu++)
					{
						UnknownParameter unknownParameterA = this.unknownParameters.get(uu);
						ObservationGroup observationGroupA = unknownParameterA.Observations;

						int dimA = 1;
						switch (unknownParameterA.ParameterType.innerEnumValue)
						{
						case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT2D:
							dimA = 2;
							break;
						case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT3D:
							dimA = 3;
							break;
						default:
							dimA = 1;
							break;
						}
						for (int ii = uu == u?i + 1:0; ii < dimA; ii++)
						{
							int colA = unknownParameterA.ColInJacobiMatrix + ii;

							for (int jj = 0; jj < observationGroupA.size(); jj++)
							{
								Observation observationA = observationGroupA.get(jj);
								int rowA = observationA.RowInJacobiMatrix;
								// skip zero multiplications
								if (aTp.get(rowA) == 0)
								{
									continue;
								}

								double a = 0.0;
								// Berechnte Normalgleichung N=AT*P*A
								if (unknownParameterA.ParameterType == ParameterType.POINT1D)
								{
									Point p = (Point)unknownParameterA;

									if (p.Equals(observationA.StartPoint))
									{
										a = observationA.diffZs();
									}
									else if (p.Equals(observationA.EndPoint))
									{
										a = observationA.diffZe();
									}
								}
								else if (unknownParameterA.ParameterType == ParameterType.POINT2D)
								{
									Point p = (Point)unknownParameterA;
									if (p.Equals(observationA.StartPoint))
									{
										if (ii == 0)
										{
											a = observationA.diffXs();
										}
										else if (ii == 1)
										{
											a = observationA.diffYs();
										}

									}
									else if (p.Equals(observationA.EndPoint))
									{
										if (ii == 0)
										{
											a = observationA.diffXe();
										}
										else if (ii == 1)
										{
											a = observationA.diffYe();
										}
									}
								}
								else if (unknownParameterA.ParameterType == ParameterType.POINT3D)
								{
									Point p = (Point)unknownParameterA;
									if (p.Equals(observationA.StartPoint))
									{
										if (ii == 0)
										{
											a = observationA.diffXs();
										}
										else if (ii == 1)
										{
											a = observationA.diffYs();
										}
										else if (ii == 2)
										{
											a = observationA.diffZs();
										}
									}
									else if (p.Equals(observationA.EndPoint))
									{
										if (ii == 0)
										{
											a = observationA.diffXe();
										}
										else if (ii == 1)
										{
											a = observationA.diffYe();
										}
										else if (ii == 2)
										{
											a = observationA.diffZe();
										}
									}
								}

								else if (unknownParameterA.ParameterType == ParameterType.VERTICAL_DEFLECTION_X)
								{
									VerticalDeflectionX deflection = (VerticalDeflectionX)unknownParameterA;
									Point p = deflection.Point;
									if (p.Equals(observationA.StartPoint))
									{
										a = observationA.diffVerticalDeflectionXs();
									}
									else if (p.Equals(observationA.EndPoint))
									{
										a = observationA.diffVerticalDeflectionXe();
									}
								}
								else if (unknownParameterA.ParameterType == ParameterType.VERTICAL_DEFLECTION_Y)
								{
									VerticalDeflectionY deflection = (VerticalDeflectionY)unknownParameterA;
									Point p = deflection.Point;
									if (p.Equals(observationA.StartPoint))
									{
										a = observationA.diffVerticalDeflectionYs();
									}
									else if (p.Equals(observationA.EndPoint))
									{
										a = observationA.diffVerticalDeflectionYe();
									}
								}
								else if (unknownParameterA.ParameterType == ParameterType.ORIENTATION)
								{
									a = observationA.diffOri();
								}
								else if (unknownParameterA.ParameterType == ParameterType.ZERO_POINT_OFFSET)
								{
									a = observationA.diffAdd();
								}
								else if (unknownParameterA.ParameterType == ParameterType.SCALE)
								{
									a = observationA.diffScale();
								}
								else if (unknownParameterA.ParameterType == ParameterType.REFRACTION_INDEX)
								{
									a = observationA.diffRefCoeff();
								}
								else if (unknownParameterA.ParameterType == ParameterType.ROTATION_X)
								{
									a = observationA.diffRotX();
								}
								else if (unknownParameterA.ParameterType == ParameterType.ROTATION_Y)
								{
									a = observationA.diffRotY();
								}
								else if (unknownParameterA.ParameterType == ParameterType.ROTATION_Z)
								{
									a = observationA.diffRotZ();
								}
								// Berechnung von N = ATP*A 
								N.add(colAT, colA, aTp.get(rowA) * a);
							}
						}
					}
				}
			}

			// Fuege stochastische Lotabweichungen hinzu
			if (this.pointsWithStochasticDeflection != null && this.pointsWithStochasticDeflection.Count > 0)
			{
				foreach (Point point in this.pointsWithStochasticDeflection)
				{
					VerticalDeflection deflectionX = point.VerticalDeflectionX;
					int col = deflectionX.ColInJacobiMatrix;
					double qll = deflectionX.StdApriori * deflectionX.StdApriori;
					n.add(col, (deflectionX.Value0 - deflectionX.Value) / qll);
					N.add(col, col, 1.0 / qll);

					VerticalDeflection deflectionY = point.VerticalDeflectionY;
					col = deflectionY.ColInJacobiMatrix;
					qll = deflectionY.StdApriori * deflectionY.StdApriori;
					n.add(col, (deflectionY.Value0 - deflectionY.Value) / qll);
					N.add(col, col, 1.0 / qll);
				}
			}

			// Fuege stochastische Anschlusspunkte hinzu
			if (this.stochasticPoints != null && this.stochasticPoints.Count > 0)
			{
				foreach (Point point in this.stochasticPoints)
				{
					int col = point.ColInJacobiMatrix;
					if (point.Dimension != 1)
					{
						double qll = point.StdXApriori * point.StdXApriori;
						n.add(col, (point.X0 - point.X) / qll);
						N.add(col, col++, 1.0 / qll);

						qll = point.StdYApriori * point.StdYApriori;
						n.add(col, (point.Y0 - point.Y) / qll);
						N.add(col, col++, 1.0 / qll);
					}
					if (point.Dimension != 2)
					{
						double qll = point.StdZApriori * point.StdZApriori;
						n.add(col, (point.Z0 - point.Z) / qll);
						N.add(col, col, 1.0 / qll);
					}
				}
			}

			if (this.freeNetwork && this.datumPoints != null && this.datumPoints.Count > 0)
			{
				// Schwerpunkt bestimmen
				double x0 = 0, y0 = 0, z0 = 0;
				int nx = 0, ny = 0, nz = 0;
				for (int i = 0; i < this.datumPoints.Count; i++)
				{
					Point p = this.datumPoints[i];
					int dim = p.Dimension;
					if (dim != 1)
					{
						x0 += p.X;
						y0 += p.Y;
						nx++;
						ny++;
					}
					if (dim != 2)
					{
						z0 += p.Z;
						nz++;
					}
				}
				x0 = nx > 0 ? x0 / nx : 0.0;
				y0 = ny > 0 ? y0 / ny : 0.0;
				z0 = nz > 0 ? z0 / nz : 0.0;
				// Schwerpunkt aller Datumspunkte 1D+2D+3D
				Point centerPoint3D = new Point3D("c3D", x0, y0, z0);
				int row = N.numRows() - this.rankDefect.Defect - numberOfStrainEquations;
				// Positionen in der BedingungsMatrix
				int defectRow = 0;
				int tx = this.rankDefect.estimateTranslationX()?defectRow++: -1;
				int ty = this.rankDefect.estimateTranslationY()?defectRow++: -1;
				int tz = this.rankDefect.estimateTranslationZ()?defectRow++: -1;
				int rx = this.rankDefect.estimateRotationX()?defectRow++: -1;
				int ry = this.rankDefect.estimateRotationY()?defectRow++: -1;
				int rz = this.rankDefect.estimateRotationZ()?defectRow++: -1;
				int sx = this.rankDefect.estimateShearX()?defectRow++: -1;
				int sy = this.rankDefect.estimateShearY()?defectRow++: -1;
				int sz = this.rankDefect.estimateShearZ()?defectRow++: -1;
				int mx = this.rankDefect.estimateScaleX()?defectRow++: -1;
				int my = this.rankDefect.estimateScaleY()?defectRow++: -1;
				int mz = this.rankDefect.estimateScaleZ()?defectRow++: -1;
				int mxy = this.rankDefect.estimateScaleXY()?defectRow++: -1;
				int mxyz = this.rankDefect.estimateScaleXYZ()?defectRow++: -1;

				if (mxyz >= 0)
				{
					mx = mxyz;
					my = mxyz;
					mz = mxyz;
					mxy = mxyz;
				}
				else if (mxy >= 0)
				{
					mx = mxy;
					my = mxy;
				}

				// Summe der quadrierten Wert einer Spalte in der Bedingunsmatrix
				double[] normColumn = new double[this.rankDefect.Defect];

				for (int i = 0; i < this.datumPoints.Count; i++)
				{
					Point point = this.datumPoints[i];
					int dim = point.Dimension;
					int col = point.ColInJacobiMatrix;

					// Translation und Maßstab
					// 1D und 3D (Hoehe)
					if (dim != 2)
					{
						if (tz >= 0)
						{
							N.set(col + dim - 1, row + tz, 1.0);
							normColumn[tz] += 1.0;
						}
						if (mz >= 0)
						{
							double z = point.Z - centerPoint3D.Z;
							N.set(col + dim - 1, row + mz, z);
							normColumn[mz] += z * z;
						}
					}

					// 2D und 3D (Lage)
					if (dim != 1)
					{
						if (tx >= 0)
						{
							N.set(col + 0, row + tx, 1.0);
							normColumn[tx] += 1.0;
						}
						if (ty >= 0)
						{
							N.set(col + 1, row + ty, 1.0);
							normColumn[ty] += 1.0;
						}
					}

					// Rotation bei 1D-GNSS
					if (dim == 1)
					{
						double x = point.X - centerPoint3D.X;
						double y = point.Y - centerPoint3D.Y;

						if (rx >= 0)
						{
							N.set(col, row + rx, -y);
							normColumn[rx] += y * y;
						}
						if (ry >= 0)
						{
							N.set(col, row + ry, x);
							normColumn[ry] += x * x;
						}
					}

					// Rotation & Maßstab
					if (dim > 1)
					{
						double x = point.X - centerPoint3D.X;
						double y = point.Y - centerPoint3D.Y;

						if (rz >= 0)
						{
							N.set(col + 0, row + rz, y);
							N.set(col + 1, row + rz, -x);
							normColumn[rz] += x * x + y * y;
						}

						if (sz >= 0)
						{
							N.set(col + 0, row + sz, y);
							N.set(col + 1, row + sz, x);
							normColumn[sz] += x * x + y * y;
						}

						if (mx >= 0)
						{
							N.set(col + 0, row + mx, x);
							normColumn[mx] += x * x;
						}

						if (my >= 0)
						{
							N.set(col + 1, row + my, y);
							normColumn[my] += y * y;
						}

						if (dim == 3)
						{
							double z = point.Z - centerPoint3D.Z;
							if (rx >= 0)
							{
								N.set(col + 1, row + rx, z);
								N.set(col + 2, row + rx, -y);
								normColumn[rx] += z * z + y * y;
							}

							if (ry >= 0)
							{
								N.set(col + 0, row + ry, -z);
								N.set(col + 2, row + ry, x);
								normColumn[ry] += z * z + x * x;
							}

							if (sx >= 0)
							{
								N.set(col + 1, row + sx, z);
								N.set(col + 2, row + sx, y);
								normColumn[sx] += z * z + y * y;
							}

							if (sy >= 0)
							{
								N.set(col + 0, row + sy, z);
								N.set(col + 2, row + sy, x);
								normColumn[sy] += z * z + x * x;
							}
						}
					}
				}

				// Normieren der Spalten
				for (int i = 0; i < this.datumPoints.Count; i++)
				{
					Point point = this.datumPoints[i];
					int dim = point.Dimension;
					int col = point.ColInJacobiMatrix;

					// Translation und Maßstab
					// 1D und 3D (Hoehe)
					if (dim != 2)
					{
						if (tz >= 0)
						{
							N.set(col + dim - 1, row + tz, N.get(col + dim - 1, row + tz) / Math.Sqrt(normColumn[tz]));
						}
						if (mz >= 0)
						{
							N.set(col + dim - 1, row + mz, N.get(col + dim - 1, row + mz) / Math.Sqrt(normColumn[mz]));
						}
					}

					// 2D und 3D (Lage)
					if (dim != 1)
					{
						if (tx >= 0)
						{
							N.set(col + 0, row + tx, N.get(col + 0, row + tx) / Math.Sqrt(normColumn[tx]));
						}
						if (ty >= 0)
						{
							N.set(col + 1, row + ty, N.get(col + 1, row + ty) / Math.Sqrt(normColumn[ty]));
						}
					}

					// Rotation & Maßstab
					if (dim > 1)
					{
						if (rz >= 0)
						{
							N.set(col + 0, row + rz, N.get(col + 0, row + rz) / Math.Sqrt(normColumn[rz]));
							N.set(col + 1, row + rz, N.get(col + 1, row + rz) / Math.Sqrt(normColumn[rz]));
						}

						if (sz >= 0)
						{
							N.set(col + 0, row + sz, N.get(col + 0, row + sz) / Math.Sqrt(normColumn[sz]));
							N.set(col + 1, row + sz, N.get(col + 1, row + sz) / Math.Sqrt(normColumn[sz]));
						}

						if (mx >= 0)
						{
							N.set(col + 0, row + mx, N.get(col + 0, row + mx) / Math.Sqrt(normColumn[mx]));
						}

						if (my >= 0)
						{
							N.set(col + 1, row + my, N.get(col + 1, row + my) / Math.Sqrt(normColumn[my]));
						}

						if (dim == 3)
						{
							if (rx >= 0)
							{
								N.set(col + 1, row + rx, N.get(col + 1, row + rx) / Math.Sqrt(normColumn[rx]));
								N.set(col + 2, row + rx, N.get(col + 2, row + rx) / Math.Sqrt(normColumn[rx]));
							}

							if (ry >= 0)
							{
								N.set(col + 0, row + ry, N.get(col + 0, row + ry) / Math.Sqrt(normColumn[ry]));
								N.set(col + 2, row + ry, N.get(col + 2, row + ry) / Math.Sqrt(normColumn[ry]));
							}

							if (sx >= 0)
							{
								N.set(col + 1, row + sx, N.get(col + 1, row + sx) / Math.Sqrt(normColumn[sx]));
								N.set(col + 2, row + sx, N.get(col + 2, row + sx) / Math.Sqrt(normColumn[sx]));
							}

							if (sy >= 0)
							{
								N.set(col + 0, row + sy, N.get(col + 0, row + sy) / Math.Sqrt(normColumn[sy]));
								N.set(col + 2, row + sy, N.get(col + 2, row + sy) / Math.Sqrt(normColumn[sy]));
							}
						}
					}
				}

				if (this.congruenceAnalysis && numberOfStrainEquations > 0)
				{
					row = N.numRows() - numberOfStrainEquations;

					foreach (CongruenceAnalysisGroup tieGroup in this.congruenceAnalysisGroup)
					{
						StrainAnalysisEquations strainAnalysisEquations = tieGroup.StrainAnalysisEquations;
						int nou = strainAnalysisEquations.numberOfParameters();
						int nor = strainAnalysisEquations.numberOfRestrictions();
						int not = tieGroup.size(true);
						int dim = tieGroup.Dimension;
						Equation[] equations = StrainAnalysisEquations.getEquations(dim);
						CoordinateComponent[] components = StrainAnalysisEquations.getCoordinateComponents(dim);

						if (strainAnalysisEquations.hasUnconstraintParameters() && not * dim + nor >= nou)
						{

							// Ableitungen nach den Restriktionen zum fix. von Parametern
							for (int resIdx = 0; resIdx < nor; resIdx++)
							{
								RestrictionType restriction = strainAnalysisEquations.getRestriction(resIdx);
								for (int parIdx = 0; parIdx < nou; parIdx++)
								{
									StrainParameter parameter = strainAnalysisEquations.get(parIdx);
									int colInA = parameter.ColInJacobiMatrix;

									N.set(colInA, row + resIdx, strainAnalysisEquations.diff(parameter, restriction));
									n.set(row + resIdx, -strainAnalysisEquations.getContradiction(restriction));
								}
							}
							row += nor;

							for (int i = 0; i < not; i++)
							{
								CongruenceAnalysisPointPair tie = tieGroup.get(i, true);
								Point p0 = tie.StartPoint;
								Point p1 = tie.EndPoint;

								for (int eqIdx = 0; eqIdx < equations.Length; eqIdx++)
								{
									Equation equation = equations[eqIdx];

									// Ableitungen nach den Zusatzparametern
									for (int parIdx = 0; parIdx < nou; parIdx++)
									{
										StrainParameter parameter = strainAnalysisEquations.get(parIdx);
										int colInA = parameter.ColInJacobiMatrix;

										N.set(colInA, row, strainAnalysisEquations.diff(parameter, p0, equation));
										n.set(row, -strainAnalysisEquations.getContradiction(p0, p1, equation));
									}

									// Ableitungen nach den Punkten
									for (int coordIdx = 0; coordIdx < components.Length; coordIdx++)
									{
										CoordinateComponent component = components[coordIdx];
										Point p;

										if (component == CoordinateComponent.X1 || component == CoordinateComponent.Y1 || component == CoordinateComponent.Z1)
										{
											p = p0;
										}
										else
										{
											p = p1;
										}

										int colInA = p.ColInJacobiMatrix;

										if (component == CoordinateComponent.Y1 || component == CoordinateComponent.Y2)
										{
											colInA += 1;
										}

										if (component == CoordinateComponent.Z1 || component == CoordinateComponent.Z2)
										{
											colInA += p.Dimension - 1;
										}

										N.set(colInA, row, strainAnalysisEquations.diff(p0, p1, component, equation));
									}
									row++;
								}
							}
						}
					}
				}

			}
	//		// Unnoetig, da Pseudobeobachtung und berechneter Wert identisch sind, d.h., 
			// observation.setValueApriori(observation.getValueAposteriori()); in SQLAdjustmentManager  
			if (this.estimationType == EstimationType.SIMULATION)
			{
				n.zero();
			}

			return new NormalEquationSystem(N, n);
		}

		/// <summary>
		/// Bricht Iteration an der naechst moeglichen Stelle ab
		/// </summary>
		public virtual void interrupt()
		{
			this.interrupt_Conflict = true;
		}

		/// <summary>
		/// Setz die Varianzkomponenten der einzelnen Gruppen auf <em>Null</em> zurueck
		/// </summary>
		private void resetVarianceComponents()
		{
			IDictionary<VarianceComponentType, VarianceComponent> varianceComponents = new LinkedHashMap<VarianceComponentType, VarianceComponent>();
			foreach (VarianceComponent varianceEstimation in this.varianceComponents.Values)
			{
				varianceComponents[varianceEstimation.VarianceComponentType] = new VarianceComponent(varianceEstimation.VarianceComponentType);
			}
			this.varianceComponents = varianceComponents;
		}

		/// <summary>
		/// Berechnet das Ausgleichungsmodell iterativ 
		/// und gibt im Erfolgsfall true zurueck
		/// </summary>
		/// <returns> estimateStatus </returns>
		public virtual EstimationStateType estimateModel()
		{
			bool applyUnscentedTransformation = this.estimationType == EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION || this.estimationType == EstimationType.MODIFIED_UNSCENTED_TRANSFORMATION;
			this.maxDx = double.Epsilon;
			this.currentMaxAbsDx = this.maxDx;
			this.numberOfHypotesis = 0;
			this.calculateStochasticParameters_Conflict = false;
			this.currentEstimationStatus = EstimationStateType.BUSY;
			this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);

			int runs = this.maximalNumberOfIterations - 1;
			bool isEstimated = false, estimateCompleteModel = false, isConverge = true;

			if (this.maximalNumberOfIterations == 0)
			{
				estimateCompleteModel = isEstimated = true;
			}
			if (this.estimationType == EstimationType.SIMULATION)
			{
				estimateCompleteModel = isEstimated = true;
			}

			// Fuege Lotabweichungen als Unbekannte zum Modell hinzu
			this.addVerticalDeflectionToModel();
			//Fuehre stochastische Anschlusspunkte und stochastische Lotparameter ins Modell ein
			this.addStochasticPointsAndStochasticDeflectionToModel();

			// ermittle Rank-Defekt anhand der Beobachtungen
			this.freeNetwork = this.freeNetwork || (this.referencePoints == null || this.referencePoints.Count == 0) && (this.stochasticPoints == null || this.stochasticPoints.Count == 0);

			if (this.freeNetwork)
			{
				if (this.datumPoints == null || this.datumPoints.Count == 0)
				{
					this.currentEstimationStatus = EstimationStateType.SINGULAR_MATRIX;
					this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
					return this.currentEstimationStatus;
				}
				this.detectRankDefect();
			}

			// Fuehre die Strain-Parameter als mgl. Zusatzparameter mit ins Modell ein
			if (this.freeNetwork && this.congruenceAnalysis)
			{
				this.addStrainParametersToModel();
			}

			// Setze VC auf 1 fuer alle Typen
			this.resetVarianceComponents();

			// Sortiere die unbekannten Parameter so, dass die Zusatzparameter am Ende stehen
			this.unknownParameters.resortParameters();

			try
			{
				double lastStepSignum = 0.0;
				int numObs = this.numberOfObservations + this.numberOfStochasticPointRows + this.numberOfStochasticDeflectionRows;
				int numberOfEstimationSteps = this.estimationType == EstimationType.MODIFIED_UNSCENTED_TRANSFORMATION ? 2 * numObs + 1 : this.estimationType == EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION ? numObs + 2 : 1;

				double alpha2 = this.alphaUT * this.alphaUT;
				double weight0 = this.weightZero;
				double weighti = (1.0 - weight0) / (double)(numberOfEstimationSteps - 1.0);

				Vector SigmaUT = null, xUT = null, vUT = null;
				Matrix solutionVectors = null;

				if (applyUnscentedTransformation)
				{
					xUT = new DenseVector(this.numberOfUnknownParameters);
					vUT = new DenseVector(numObs);
					solutionVectors = new DenseMatrix(this.numberOfUnknownParameters, numberOfEstimationSteps);

					if (this.estimationType == EstimationType.MODIFIED_UNSCENTED_TRANSFORMATION)
					{
						SigmaUT = new DenseVector(1);
						SigmaUT.set(0, Math.Sqrt(0.5 / weighti));
					}
					else if (this.estimationType == EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION)
					{
						SigmaUT = new DenseVector(numObs);
						if (weight0 < 0 || weight0 >= 1)
						{
							throw new System.ArgumentException("Error, zero-weight is out of range. If SUT is applied, valid values are 0 <= w0 < 1! " + weight0);
						}
					}

					weight0 = weight0 / alpha2 + (1.0 - 1.0 / alpha2);
					weighti = weighti / alpha2;
				}

				for (int estimationStep = 0; estimationStep < numberOfEstimationSteps; estimationStep++)
				{
					// Reset aller Iterationseinstellungen
					this.maxDx = double.Epsilon;
					this.currentMaxAbsDx = this.maxDx;
					runs = this.maximalNumberOfIterations - 1;
					isEstimated = false;
					estimateCompleteModel = false;
					isConverge = true;

					if (applyUnscentedTransformation)
					{
						this.currentEstimationStatus = EstimationStateType.UNSCENTED_TRANSFORMATION_STEP;
						this.change.firePropertyChange(this.currentEstimationStatus.ToString(), numberOfEstimationSteps, estimationStep + 1);

						// Reset der unbekannten Datumsparameter
						if (numberOfEstimationSteps > 1 && estimationStep != (numberOfEstimationSteps - 1))
						{
							this.resetDatumPoints();
						}

						if (this.estimationType == EstimationType.MODIFIED_UNSCENTED_TRANSFORMATION)
						{
							double signum = estimationStep == (numberOfEstimationSteps - 1) ? 0.0 : estimationStep < numObs ? +1.0 : -1.0;
							int currentObsIdx = estimationStep % numObs;

							// Entferne letzte UT-Modifizierung
							if (estimationStep > 0)
							{
								int lastObsIdx = currentObsIdx - 1;
								lastObsIdx = lastObsIdx < 0 ? numObs - 1 : lastObsIdx;
								this.prepareModifiedUnscentedTransformationObservation(lastObsIdx, -lastStepSignum * SigmaUT.get(0));
							}

							// UT-Modifizierung des aktuellen Schritts
							if (estimationStep < numberOfEstimationSteps - 1)
							{
								this.prepareModifiedUnscentedTransformationObservation(currentObsIdx, signum * SigmaUT.get(0));
							}
							lastStepSignum = signum;
						}
						else if (this.estimationType == EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION)
						{
							this.prepareSphericalSimplexUnscentedTransformationObservation(estimationStep, SigmaUT, weighti);
						}
					}

					do
					{
						this.maxDx = double.Epsilon;
						this.numberOfHypotesis = 0;
						this.iterationStep = this.maximalNumberOfIterations - runs;
						this.currentEstimationStatus = EstimationStateType.ITERATE;

						this.change.firePropertyChange(this.currentEstimationStatus.ToString(), this.maximalNumberOfIterations, this.iterationStep);

						// erzeuge Normalgleichung
						this.applySphericalVerticalDeflections();
						NormalEquationSystem neq = this.createNormalEquation();
						this.resetVarianceComponents();

						if (this.interrupt_Conflict || neq == null)
						{
							this.currentEstimationStatus = EstimationStateType.INTERRUPT;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							this.interrupt_Conflict = false;
							return this.currentEstimationStatus;
						}
						DenseVector n = neq.Vector;
						UpperSymmPackMatrix N = neq.Matrix;
						Vector dx = n;

						estimateCompleteModel = isEstimated;
						try
						{
							if ((estimateCompleteModel && estimationStep == (numberOfEstimationSteps - 1)) || this.estimationType == EstimationType.L1NORM)
							{
								this.calculateStochasticParameters_Conflict = (this.estimationType != EstimationType.L1NORM && estimateCompleteModel);
								// Bestimme die Parameter der ausseren Genauigkeit und
								// ueberschreibe die Normalgleichung und den Absolutgliedvektor
								// in-situ, sodass N == Qxx und n == dx am Ende ist.
								// Wenn UT gewaehlt, wird Qxx seperat aus den Einzelloesungen bestimmt
								if (this.estimationType != EstimationType.L1NORM)
								{
									this.currentEstimationStatus = EstimationStateType.INVERT_NORMAL_EQUATION_MATRIX;
									this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
								}
								this.estimateFactorsForOutherAccracy(N, n);

								if (this.calculateStochasticParameters_Conflict)
								{
									this.currentEstimationStatus = EstimationStateType.ESTIAMTE_STOCHASTIC_PARAMETERS;
									this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
								}
							}
							else
							{
								// Loese Nx=n und ueberschreibe n durch die Loesung x
								MathExtension.solve(N, n, false);
							}

							n = null;
							N = null;
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


						if (applyUnscentedTransformation)
						{
							if (estimateCompleteModel)
							{
								this.addUnscentedTransformationSolution(dx, xUT, vUT, solutionVectors, estimationStep, estimationStep < (numberOfEstimationSteps - 1) ? weighti : weight0);
							}

							// Letzter Durchlauf der UT
							// Bestimme Parameterupdate dx und Kovarianzmatrix Qxx
							if (estimateCompleteModel && estimationStep > 0 && estimationStep == (numberOfEstimationSteps - 1))
							{
								this.estimateUnscentedTransformationParameterUpdateAndCovarianceMatrix(dx, xUT, solutionVectors, weighti, weight0 + (1.0 - alpha2 + this.betaUT));
							}
						}

						this.updateModel(dx, vUT, estimateCompleteModel && estimationStep == (numberOfEstimationSteps - 1));
						dx = null;
						vUT = null;

						if (this.interrupt_Conflict)
						{
							this.currentEstimationStatus = EstimationStateType.INTERRUPT;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							this.interrupt_Conflict = false;
							return this.currentEstimationStatus;
						}

						if (double.IsInfinity(this.maxDx) || double.IsNaN(this.maxDx))
						{
							this.currentEstimationStatus = EstimationStateType.SINGULAR_MATRIX;
							this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
							return this.currentEstimationStatus;
						}
						else if (this.maxDx <= SQRT_EPS && runs > 0)
						{
							isEstimated = true;
							this.currentEstimationStatus = EstimationStateType.CONVERGENCE;
							if (!applyUnscentedTransformation)
							{
								this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxDx);
							}
						}
						else if (runs-- <= 1)
						{
							if (estimateCompleteModel)
							{
								if (this.estimationType == EstimationType.L1NORM)
								{
									this.currentEstimationStatus = EstimationStateType.ROBUST_ESTIMATION_FAILED;
									this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
									return this.currentEstimationStatus;
								}
								else
								{
									this.currentEstimationStatus = EstimationStateType.NO_CONVERGENCE;
									this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxDx);
									isConverge = false;
								}
							}
							isEstimated = true;
						}
						else
						{
							this.currentEstimationStatus = EstimationStateType.CONVERGENCE;
							if (!applyUnscentedTransformation)
							{
								this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxDx);
							}
						}

						// Sollten nur stochastische Punkte enthalten sein, ist maxDx MIN_VALUE
						if (this.maxDx > double.Epsilon)
						{
							this.currentMaxAbsDx = this.maxDx;
						}
						else
						{
							this.maxDx = this.currentMaxAbsDx;
						}
					} while (!estimateCompleteModel);
				}
				// Exportiere CoVar (sofern aktiviert), da diese danach ueberschrieben wird
				if (!this.exportCovarianceMatrix())
				{
					Console.Error.WriteLine("Fehler, Varianz-Kovarianz-Matrix konnte nicht exportiert werden.");
				}

				// Fuehre Hauptkomponentenanalyse durch; Qxx wird hierbei zu NULL gesetzt
				if (this.numberOfPrincipalComponents > 0)
				{
					this.estimatePrincipalComponentAnalysis(this.numberOfPrincipalComponents);
				}
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
			else if (this.currentEstimationStatus.getId() == EstimationStateType.BUSY.getId() || this.calculateStochasticParameters_Conflict)
			{
				this.currentEstimationStatus = EstimationStateType.ERROR_FREE_ESTIMATION;
				this.change.firePropertyChange(this.currentEstimationStatus.ToString(), SQRT_EPS, this.maxDx);
			}

			return this.currentEstimationStatus;
		}

		/// <summary>
		/// Liefert den maximalen (absoluten) Zuzschlag der Iteration </summary>
		/// <returns> max(|DX|) </returns>
		public virtual double MaxAbsDx
		{
			get
			{
				return this.currentMaxAbsDx;
			}
		}

		/// <summary>
		/// Modifiziert die Beobachtung observation(index) fuer die
		/// Unscented Transformation (bzw. macht die Modifikation rueckgaengig) </summary>
		/// <param name="index"> </param>
		/// <param name="signum"> </param>
		/// <param name="scale"> </param>
		private void prepareSphericalSimplexUnscentedTransformationObservation(int estimationStep, Vector SigmaUT, double weight)
		{
			int noo = SigmaUT.size();

			for (int idx = 0; idx < this.numberOfObservations; idx++)
			{
				Observation observation = this.projectObservations.get(idx);
				int row = observation.RowInJacobiMatrix;
				double std = observation.StdApriori;
				double value = observation.ValueApriori;
				double sigmaUT = SigmaUT.get(row);
				// entferne letzte Modifikation
				value = value - std * sigmaUT;
				sigmaUT = 0;

				if (estimationStep < noo + 2 && row >= 0)
				{
	//				if (row == 0) {
	//					if (estimationStep == 0)
	//						sigmaUT = -1.0/Math.sqrt(2.0 * weight);
	//					else if (estimationStep == 1)
	//						sigmaUT = +1.0/Math.sqrt(2.0 * weight);
	//				}
	//				if (row >= 0) {
					if (row == estimationStep - 1)
					{
						sigmaUT = (1.0 + row) / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight);
					}
					else if (row > estimationStep - 2)
					{
						sigmaUT = -1.0 / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight);
					}
	//				}
				}
				// modifiziere Beobachtung
				value = value + std * sigmaUT;
				observation.ValueApriori = value;
				SigmaUT.set(row, sigmaUT);
			}

			foreach (Point point in this.pointsWithStochasticDeflection)
			{
				VerticalDeflection deflectionX = point.VerticalDeflectionX;
				VerticalDeflection deflectionY = point.VerticalDeflectionY;

				int rowX = deflectionX.RowInJacobiMatrix;
				int rowY = deflectionY.RowInJacobiMatrix;

				double stdX = deflectionX.StdApriori;
				double stdY = deflectionY.StdApriori;

				double valueX = deflectionX.Value0;
				double valueY = deflectionY.Value0;

				double sigmaUTX = SigmaUT.get(rowX);
				double sigmaUTY = SigmaUT.get(rowY);

				// entferne letzte Modifikation in X/Y
				valueX = valueX - stdX * sigmaUTX;
				valueY = valueY - stdY * sigmaUTY;

				sigmaUTX = 0;
				sigmaUTY = 0;

				if (estimationStep < noo + 2 && rowX >= 0 && rowY >= 0)
				{
					if (rowX == estimationStep - 1)
					{
						sigmaUTX = (1.0 + rowX) / Math.Sqrt(((1.0 + rowX) * (2.0 + rowX)) * weight);
					}
					else if (rowX > estimationStep - 2)
					{
						sigmaUTX = -1.0 / Math.Sqrt(((1.0 + rowX) * (2.0 + rowX)) * weight);
					}

					if (rowY == estimationStep - 1)
					{
						sigmaUTY = (1.0 + rowY) / Math.Sqrt(((1.0 + rowY) * (2.0 + rowY)) * weight);
					}
					else if (rowY > estimationStep - 2)
					{
						sigmaUTY = -1.0 / Math.Sqrt(((1.0 + rowY) * (2.0 + rowY)) * weight);
					}
				}

				// modifiziere X/Y
				valueX = valueX + stdX * sigmaUTX;
				valueY = valueY + stdY * sigmaUTY;

				deflectionX.Value0 = valueX;
				deflectionY.Value0 = valueY;

				SigmaUT.set(rowX, sigmaUTX);
				SigmaUT.set(rowY, sigmaUTY);
			}

			foreach (Point point in this.stochasticPoints)
			{
				int row = point.RowInJacobiMatrix;
				int dim = point.Dimension;

				if (row >= 0)
				{
					if (dim != 1)
					{
						double stdX = point.StdXApriori;
						double valueX = point.X0;
						double sigmaUTX = SigmaUT.get(row);
						// entferne letzte Modifikation in X
						valueX = valueX - stdX * sigmaUTX;
						sigmaUTX = 0;

						if (estimationStep < noo + 2)
						{
							if (row == estimationStep - 1)
							{
								sigmaUTX = (1.0 + row) / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight);
							}
							else if (row > estimationStep - 2)
							{
								sigmaUTX = -1.0 / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight);
							}
						}
						// modifiziere X
						valueX = valueX + stdX * sigmaUTX;
						point.X0 = valueX;
						SigmaUT.set(row, sigmaUTX);
						row++;

						double stdY = point.StdYApriori;
						double valueY = point.Y0;
						double sigmaUTY = SigmaUT.get(row);
						// entferne letzte Modifikation in Y
						valueY = valueY - stdY * sigmaUTY;
						sigmaUTY = 0;

						if (estimationStep < noo + 2)
						{
							if (row == estimationStep - 1)
							{
								sigmaUTY = (1.0 + row) / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight);
							}
							else if (row > estimationStep - 2)
							{
								sigmaUTY = -1.0 / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight);
							}
						}
						// modifiziere Y
						valueY = valueY + stdY * sigmaUTY;
						point.Y0 = valueY;
						SigmaUT.set(row, sigmaUTY);
						row++;
					}
					if (dim != 2)
					{
						double stdZ = point.StdZApriori;
						double valueZ = point.Z0;
						double sigmaUTZ = SigmaUT.get(row);
						// entferne letzte Modifikation in Z
						valueZ = valueZ - stdZ * sigmaUTZ;
						sigmaUTZ = 0;

						if (estimationStep < noo + 2)
						{
							if (row == estimationStep - 1)
							{
								sigmaUTZ = (1.0 + row) / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight);
							}
							else if (row > estimationStep - 2)
							{
								sigmaUTZ = -1.0 / Math.Sqrt(((1.0 + row) * (2.0 + row)) * weight);
							}
						}
						// modifiziere Z
						valueZ = valueZ + stdZ * sigmaUTZ;
						point.Z0 = valueZ;
						SigmaUT.set(row, sigmaUTZ);
					}
				}
			}
		}

		/// <summary>
		/// Modifiziert die Beobachtung observation(index) fuer die
		/// Unscented Transformation (bzw. macht die Modifikation rueckgaengig) </summary>
		/// <param name="index"> </param>
		/// <param name="signum"> </param>
		/// <param name="scale"> </param>
		private void prepareModifiedUnscentedTransformationObservation(int index, double scale)
		{
			if (index < this.numberOfObservations)
			{
				Observation observation = this.projectObservations.get(index);

				double std = observation.StdApriori;
				double value = observation.ValueApriori;

				value = value + scale * std;
				observation.ValueApriori = value;
			}
			else if (index < this.numberOfObservations + this.numberOfStochasticDeflectionRows)
			{
				int deflectionIdx = index - this.numberOfObservations;
				int deflectionType = deflectionIdx % 2; // 0 == x oder 1 == y
				deflectionIdx = deflectionIdx / 2;

				Point point = this.pointsWithStochasticDeflection[deflectionIdx];

				if (deflectionType == 0)
				{
					VerticalDeflection deflectionX = point.VerticalDeflectionX;

					double std = deflectionX.StdApriori;
					double value = deflectionX.Value0;

					value = value + scale * std;
					deflectionX.Value0 = value;
				}
				else
				{
					VerticalDeflection deflectionY = point.VerticalDeflectionY;

					double std = deflectionY.StdApriori;
					double value = deflectionY.Value0;

					value = value + scale * std;
					deflectionY.Value0 = value;
				}
			}
			else if (index < this.numberOfObservations + this.numberOfStochasticDeflectionRows + this.numberOfStochasticPointRows)
			{
				int pointIdx = index - this.numberOfObservations - this.numberOfStochasticDeflectionRows;
				for (int i = 0, j = 0; i < this.stochasticPoints.Count; i++)
				{
					Point point = this.stochasticPoints[i];
					int dim = point.Dimension;

					if (pointIdx >= j + dim)
					{
						j += dim;
						continue;
					}

					for (int d = 0; d < dim; d++)
					{
						if (pointIdx == j + d)
						{
							if (dim != 1)
							{
								if (d == 0)
								{
									double std = point.StdXApriori;
									double value = point.X0;

									value = value + scale * std;
									point.X0 = value;
								}
								else if (d == 1)
								{
									double std = point.StdYApriori;
									double value = point.Y0;

									value = value + scale * std;
									point.Y0 = value;
								}
							}
							if (dim != 2 && d == dim - 1)
							{
								double std = point.StdZApriori;
								double value = point.Z0;

								value = value + scale * std;
								point.Z0 = value;
							}
						}
					}
					break;
				}
			}
		}

		/// <summary>
		/// Ueberschreibt die Schaetzung mit den Naeherungswerten der Datumspunkte.
		/// Notwendig fuer die jeweilige Iteration mittels UnscentedTransformation
		/// </summary>
		private void resetDatumPoints()
		{
			foreach (Point p in this.datumPoints)
			{
				p.resetCoordinates();
			}
		}

		private void estimateUnscentedTransformationParameterUpdateAndCovarianceMatrix(Vector dx, Vector xUT, Matrix solutionVectors, double weightN, double weightC)
		{
			int numberOfEstimationSteps = solutionVectors.numColumns();
			this.Qxx.zero();

			for (int c = 0; c < this.unknownParameters.size(); c++)
			{
				if (this.interrupt_Conflict)
				{
					return;
				}

				UnknownParameter unknownParameterCol = this.unknownParameters.get(c);
				int col = unknownParameterCol.ColInJacobiMatrix;

				int dimCol = 1;
				switch (unknownParameterCol.ParameterType.innerEnumValue)
				{
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT2D:
					dimCol = 2;
					break;
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT3D:
					dimCol = 3;
					break;
				default:
					dimCol = 1;
					break;
				}

				if (unknownParameterCol is Point)
				{
					Point point = (Point)unknownParameterCol;
					dimCol = point.Dimension;

					// Erzeuge aus der finalen UT-Loesung den Zuschlagsvektor dx = x - x0 -  Koordinaten
					if (dimCol != 1)
					{
						dx.set(col + 0, xUT.get(col + 0) - point.X);
						dx.set(col + 1, xUT.get(col + 1) - point.Y);
					}
					if (dimCol != 2)
					{
						dx.set(col + dimCol - 1, xUT.get(col + dimCol - 1) - point.Z);
					}
				}
				// Erzeuge aus der finalen UT-Loesung den Zuschlagsvektor - Zusazuparameter
				else if (unknownParameterCol is VerticalDeflection)
				{
					VerticalDeflection deflectionParameter = (VerticalDeflection) unknownParameterCol;
					dx.set(col, xUT.get(col) - deflectionParameter.Value);
				}
				else if (unknownParameterCol is AdditionalUnknownParameter)
				{
					AdditionalUnknownParameter additionalUnknownParameter = (AdditionalUnknownParameter) unknownParameterCol;
					dx.set(col, xUT.get(col) - additionalUnknownParameter.Value);
				}
				else if (unknownParameterCol is StrainParameter)
				{
					StrainParameter strainParameter = (StrainParameter) unknownParameterCol;
					dx.set(col, xUT.get(col) - strainParameter.Value);
				}

				for (int dCol = 0; dCol < dimCol; dCol++)
				{
					for (int r = c; r < this.unknownParameters.size(); r++)
					{
						if (this.interrupt_Conflict)
						{
							return;
						}

						UnknownParameter unknownParameterRow = this.unknownParameters.get(r);
						int row = unknownParameterRow.ColInJacobiMatrix;

						int dimRow = 1;
						switch (unknownParameterRow.ParameterType.innerEnumValue)
						{
						case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT2D:
							dimRow = 2;
							break;
						case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT3D:
							dimRow = 3;
							break;
						default:
							dimRow = 1;
							break;
						}

						if (unknownParameterRow is Point)
						{
							Point point = (Point)unknownParameterRow;
							dimRow = point.Dimension;
						}

						for (int dRow = 0; dRow < dimRow; dRow++)
						{
							// Laufindex des jeweiligen Spalten-/Zeilenvektors
							for (int estimationStep = 0; estimationStep < numberOfEstimationSteps; estimationStep++)
							{
								double weight = estimationStep == (numberOfEstimationSteps - 1) ? weightC : weightN;

								double valueCol = solutionVectors.get(col + dCol, estimationStep) - xUT.get(col + dCol);
								double valueRow = solutionVectors.get(row + dRow, estimationStep) - xUT.get(row + dRow);

								this.Qxx.set(col + dCol, row + dRow, this.Qxx.get(col + dCol, row + dRow) + valueRow * weight * valueCol);
							}
						}
					}
				}
			}
			solutionVectors = null;
			xUT = null;
		}

		/// <summary>
		/// Fuegt die einzelen UT-Loesungen (transformierte SIGMA-Punkte) der Matrix solutionVectors hinzu
		/// Akkumuliert die Loesungen, sodass final das Ergebnis in dxUT steht. 
		/// !!! HINWEIS: Der Vektor dxUT enthaelt den vollstaendigen Loesungsvektor !!!
		/// !!!          und NICHT den Zuschlagsvektor.                             !!! </summary>
		/// <param name="dX">   Zuschag der letzten Iteration (theoretich ein Nullvektor) </param>
		/// <param name="dxUT"> UT-Loesung </param>
		/// <param name="solutionVectors"> Matrix mit allen UT-Loesungen </param>
		/// <param name="solutionNumber"> Aktuelle UT-Loesungsnummer </param>
		/// <param name="weight"> UT-Gewicht </param>
		private void addUnscentedTransformationSolution(Vector dX, Vector xUT, Vector vUT, Matrix solutionVectors, int solutionNumber, double weight)
		{
			for (int i = 0; i < this.unknownParameters.size(); i++)
			{
				if (this.interrupt_Conflict)
				{
					return;
				}

				UnknownParameter unknownParameter = this.unknownParameters.get(i);
				int col = unknownParameter.ColInJacobiMatrix;
				if (unknownParameter is Point)
				{
					Point point = (Point)unknownParameter;
					int dim = point.Dimension;
					if (dim != 1)
					{
						double xValue = point.X + dX.get(col);
						xUT.set(col, xUT.get(col) + weight * xValue);
						solutionVectors.set(col, solutionNumber, xValue);
						col++;

						double yValue = point.Y + dX.get(col);
						xUT.set(col, xUT.get(col) + weight * yValue);
						solutionVectors.set(col, solutionNumber, yValue);
						col++;
					}
					if (dim != 2)
					{
						double zValue = point.Z + dX.get(col);
						xUT.set(col, xUT.get(col) + weight * zValue);
						solutionVectors.set(col, solutionNumber, zValue);
						col++;
					}
				}
				else if (unknownParameter is VerticalDeflection)
				{
					VerticalDeflection deflectionParameter = (VerticalDeflection) unknownParameter;
					double value = deflectionParameter.Value + dX.get(col);
					xUT.set(col, xUT.get(col) + weight * value);
					solutionVectors.set(col, solutionNumber, value);
				}
				else if (unknownParameter is AdditionalUnknownParameter)
				{
					AdditionalUnknownParameter additionalUnknownParameter = (AdditionalUnknownParameter) unknownParameter;
					double value = additionalUnknownParameter.Value + dX.get(col);
					xUT.set(col, xUT.get(col) + weight * value);
					solutionVectors.set(col, solutionNumber, value);
				}
				else if (unknownParameter is StrainParameter)
				{
					StrainParameter strainParameter = (StrainParameter) unknownParameter;
					double value = strainParameter.Value + dX.get(col);
					xUT.set(col, xUT.get(col) + weight * value);
					solutionVectors.set(col, solutionNumber, value);
				}
			}

			if (vUT != null)
			{
				vUT.add(weight, this.Residuals);
			}
		}

		/// <summary>
		/// Aktualisiert die unbekannten Groessen nach der Iteration </summary>
		/// <param name="X"> Updatevektor </param>
		/// <param name="updateCompleteModel">  </param>
		private void updateModel(Vector dX, Vector vUT, bool updateCompleteModel)
		{
			// Bestimme Qll(a-post) fuer alle Beobachtungen, 
			// da Designamatrix hier noch verfuegbar
			this.omega = 0.0;
			this.degreeOfFreedom_Conflict = 0;
			Vector vVec = null;
			if (updateCompleteModel || this.estimationType == EstimationType.L1NORM)
			{
				this.addSubRedundanceAndCofactor2Observations();

				if (updateCompleteModel)
				{
					for (int i = 0; i < this.numberOfObservations; i++)
					{
						Observation observation = this.projectObservations.get(i);
						if (this.adaptedObservationUncertainties.ContainsKey(observation))
						{
							observation.StdApriori = this.adaptedObservationUncertainties[observation];
						}
					}
				}

				// Bestimme Verbesserungen der AGL (bevor Parameterupdate erfolgt)
				if (updateCompleteModel && this.estimationType != EstimationType.SIMULATION)
				{
					vVec = this.getCorrectionVector(dX);
				}
			}

			for (int i = 0; i < this.unknownParameters.size(); i++)
			{
				if (this.interrupt_Conflict)
				{
					return;
				}

				UnknownParameter unknownParameter = this.unknownParameters.get(i);
				int col = unknownParameter.ColInJacobiMatrix;
				if (unknownParameter is Point)
				{
					Point point = (Point)unknownParameter;
					double tmpMaxDx = this.maxDx;
					int dim = point.Dimension;
					// Updaten der stochastischen Punkte unterbinden in den Iterationen
	//				if (!updateCompleteModel && this.stochasticPoints.contains(point))
	//					continue;

					if (dim != 1)
					{
						this.maxDx = Math.Max(Math.Abs(dX.get(col)), this.maxDx);
						point.X = point.X + dX.get(col++);
						this.maxDx = Math.Max(Math.Abs(dX.get(col)), this.maxDx);
						point.Y = point.Y + dX.get(col++);
					}
					if (dim != 2)
					{
						this.maxDx = Math.Max(Math.Abs(dX.get(col)), this.maxDx);
						point.Z = point.Z + dX.get(col);
					}

					if (updateCompleteModel && this.stochasticPoints.Contains(point))
					{
						this.maxDx = tmpMaxDx;
					}

					// Hinzufuegen der stochastischen Groessen
					if ((updateCompleteModel || this.estimationType == EstimationType.L1NORM) && point.RowInJacobiMatrix >= 0)
					{
						this.addSubRedundanceAndCofactor2Point(point, updateCompleteModel);
					}
				}
				else if (unknownParameter is AdditionalUnknownParameter)
				{
					this.maxDx = Math.Max(Math.Abs(dX.get(col)), this.maxDx);
					AdditionalUnknownParameter additionalUnknownParameter = (AdditionalUnknownParameter) unknownParameter;
					additionalUnknownParameter.Value = additionalUnknownParameter.Value + dX.get(col);
				}
				else if (unknownParameter is StrainParameter)
				{
					this.maxDx = Math.Max(Math.Abs(dX.get(col)), this.maxDx);
					StrainParameter strainParameter = (StrainParameter) unknownParameter;
					strainParameter.Value = strainParameter.Value + dX.get(col);
				}
				else if (unknownParameter is VerticalDeflection)
				{
					// Hinzufuegen der stochastischen Groessen
					if (unknownParameter is VerticalDeflectionX)
					{
						if ((updateCompleteModel || this.estimationType == EstimationType.L1NORM) && ((VerticalDeflectionX)unknownParameter).RowInJacobiMatrix >= 0)
						{
							Point point = ((VerticalDeflectionX)unknownParameter).Point;
							this.addSubRedundanceAndCofactor2Deflection(point, updateCompleteModel);
						}
					}

					this.maxDx = Math.Max(Math.Abs(dX.get(col)), this.maxDx);
					VerticalDeflection deflectionParameter = (VerticalDeflection) unknownParameter;
					deflectionParameter.Value = deflectionParameter.Value + dX.get(col);
				}
			}

			if (updateCompleteModel || this.estimationType == EstimationType.L1NORM)
			{
				for (int i = 0; i < this.numberOfObservations; i++)
				{
					if (this.interrupt_Conflict)
					{
						return;
					}
					// in addSubRedundanceAndCofactor hinzugefuegt
					Observation observation = this.projectObservations.get(i);
					double qll = observation.StdApriori * observation.StdApriori;
					double r = observation.Redundancy;
					double v = this.estimationType == EstimationType.SIMULATION ? 0.0 : (vUT != null ? vUT.get(observation.RowInJacobiMatrix) : observation.Correction);
					double vv = v * v;
					double omegaObs = vv / qll;
					observation.Omega = omegaObs;
					this.omega += omegaObs;
					// Modell muss noch nicht vollstaendig geloest werden, sondern nur der Teil fuer robuste Schaetzung
					if (!updateCompleteModel)
					{
						return;
					}

					// Bestimme Varianzkomponenten der Beobachtungen
					ObservationType observationType = observation.ObservationType;
					VarianceComponentType vcType = VarianceComponentType.getVarianceComponentTypeByObservationType(observationType);
					if (vcType != null && this.varianceComponents.ContainsKey(vcType))
					{
						VarianceComponent vc = this.varianceComponents[vcType];
						vc.Omega = vc.Omega + omegaObs;
						vc.Redundancy = vc.Redundancy + r;
						vc.NumberOfObservations = vc.NumberOfObservations + 1;
					}

					// Bestimme die erweitere Varianzkomponenten der Beobachtungen
					if (observation.useGroupUncertainty())
					{
						ObservationGroup group = observation.ObservationGroup;

						double qllA = group.getStdA(observation);
						double qllB = group.getStdB(observation);
						double qllC = group.getStdC(observation);

						qllA *= qllA;
						qllB *= qllB;
						qllC *= qllC;

						if (Math.Abs(Math.Sqrt(qllA + qllB + qllC) - observation.StdApriori) > SQRT_EPS)
						{
							Console.Error.WriteLine(this.GetType().Name + " Erweitere VKS; Beobachtungsdifferenz im stoch. Modell zu gross: " + observation + "    " + Math.Abs(Math.Sqrt(qllA + qllB + qllC) - observation.StdApriori));
							continue;
						}

						// Bestimme Omega vTPv
						double omegaVKS = omegaObs / qll; // vv / qll / qll
						double omegaA = omegaVKS * qllA;
						double omegaB = omegaVKS * qllB;
						double omegaC = omegaVKS * qllC;

						// Bestimme Teilredundanz
						double redundancyVKS = r / qll;
						double redundancyA = redundancyVKS * qllA;
						double redundancyB = redundancyVKS * qllB;
						double redundancyC = redundancyVKS * qllC;

						vcType = VarianceComponentType.getZeroPointOffsetVarianceComponentTypeByObservationType(observationType);
						if (vcType != null)
						{
							if (!this.varianceComponents.ContainsKey(vcType))
							{
								this.varianceComponents[vcType] = new VarianceComponent(vcType);
							}
							VarianceComponent vc = this.varianceComponents[vcType];

							vc.Omega = vc.Omega + omegaA;
							vc.Redundancy = vc.Redundancy + redundancyA;
							vc.NumberOfObservations = vc.NumberOfObservations + 1;
						}

						vcType = VarianceComponentType.getSquareRootDistanceDependentVarianceComponentTypeByObservationType(observationType);
						if (vcType != null)
						{
							if (!this.varianceComponents.ContainsKey(vcType))
							{
								this.varianceComponents[vcType] = new VarianceComponent(vcType);
							}
							VarianceComponent vc = this.varianceComponents[vcType];

							vc.Omega = vc.Omega + omegaB;
							vc.Redundancy = vc.Redundancy + redundancyB;
							vc.NumberOfObservations = vc.NumberOfObservations + 1;
						}

						vcType = VarianceComponentType.getDistanceDependentVarianceComponentTypeByObservationType(observationType);
						if (vcType != null)
						{
							if (!this.varianceComponents.ContainsKey(vcType))
							{
								this.varianceComponents[vcType] = new VarianceComponent(vcType);
							}
							VarianceComponent vc = this.varianceComponents[vcType];

							vc.Omega = vc.Omega + omegaC;
							vc.Redundancy = vc.Redundancy + redundancyC;
							vc.NumberOfObservations = vc.NumberOfObservations + 1;
						}
					}
					if (updateCompleteModel && this.estimationType != EstimationType.SIMULATION)
					{
						double observationLinearisationProof = vVec.get(observation.RowInJacobiMatrix) + v;
						this.finalLinearisationError = Math.Max(this.finalLinearisationError, Math.Abs(observationLinearisationProof));
					}
				}

				// Es liegt nun die Gesamtredundanz und Omega vor, sodass Testgroessen
				// bestimmt werden koennen. 
				int dof = this.degreeOfFreedom();
				double sigma2apost = this.VarianceFactorAposteriori;

				if (this.estimationType != EstimationType.SIMULATION && this.significanceTestStatisticDefinition.TestStatisticType == TestStatisticType.SIDAK)
				{
					this.numberOfHypotesis += this.referencePoints.Count;
					this.numberOfHypotesis += this.pointsWithReferenceDeflection.Count;

					foreach (VarianceComponent varianceEstimation in this.varianceComponents.Values)
					{
						double r = (long)Math.Round(varianceEstimation.Redundancy * 1.0E5, MidpointRounding.AwayFromZero) / 1.0E5;
						if (r > 0)
						{
							this.numberOfHypotesis++;
						}
					}
				}

				this.significanceTestStatisticParameters = this.SignificanceTestStatisticParameters;

				// Bestimme die kritischen Werte der Standardteststatistiken
				TestStatisticParameterSet tsGlobal = this.significanceTestStatisticParameters.getTestStatisticParameter(dof, double.PositiveInfinity, true);

				// Bestimme maximalen Helmert'schen Punktfehler sigmaPointMax zur Ableitung von EF*SP
				double sigma2PointMax = 0.0;
				foreach (Point point in this.allPoints.Values)
				{
					int col = point.ColInJacobiMatrix;
					if (col < 0)
					{
						continue;
					}
					int dim = point.Dimension;
					double sigma2PointMaxi = 0.0;
					for (int d = 0; d < dim; d++)
					{
						sigma2PointMaxi += sigma2apost * this.Qxx.get(col + d, col + d);
					}
					sigma2PointMax = Math.Max(sigma2PointMaxi, sigma2PointMax);
				}

				// Fuege globales Modell zu VCE hinzu
				VarianceComponent vc = new VarianceComponent(VarianceComponentType.GLOBAL);
				vc.Redundancy = dof;
				vc.NumberOfObservations = this.numberOfObservations + this.numberOfStochasticPointRows + this.numberOfStochasticDeflectionRows;
				vc.KprioGroup = tsGlobal.Quantile;
				vc.Omega = this.omega;
				this.varianceComponents[vc.VarianceComponentType] = vc;

				IList<Point> points = null;
				if (this.congruenceAnalysis && this.freeNetwork)
				{
					// Pruefung der Stabilpunkte
					points = this.datumPoints;
				}
				else
				{
					// Pruefung der Festpunkte
					points = this.referencePoints;
				}

				for (int i = 0; i < points.Count; i++)
				{
					if (this.interrupt_Conflict)
					{
						return;
					}
					Point point = points[i];
					ObservationGroup observations = point.Observations;
					int dim = point.Dimension;
					if (!this.freeNetwork)
					{
						if (dim != 1)
						{
							point.StdX = -1.0;
							point.StdY = -1.0;
						}
						if (dim != 2)
						{
							point.StdZ = -1.0;
						}
					}

					Matrix BTPQvvPB = new DenseMatrix(dim,dim);
					Vector BTPv = new DenseVector(dim);

					// Flag zur Pruefung, ob Datumspunkt in Referenzepoche ueberhaut Beobachtungen besitzt.
					// Wenn Datumspunkt nur in Folgeepoche vorhanden, kann kein Stabilpunkttest durchgefuehrt werden.
					bool hasObservationInReferenceEpoch = !(this.congruenceAnalysis && this.freeNetwork);
					for (int k = 0; k < observations.size(); k++)
					{
						Observation observationB = observations.get(k);
						if (this.congruenceAnalysis && this.freeNetwork && observationB.ObservationGroup.getEpoch() == Epoch.REFERENCE)
						{
							hasObservationInReferenceEpoch = true;
							continue;
						}
						double qB = observationB.StdApriori * observationB.StdApriori;
						double vB = this.estimationType == EstimationType.SIMULATION ? 0.0 : -observationB.Correction;
						double b = 0.0;
						vB = Math.Abs(vB) < SQRT_EPS ? 0.0 : vB;

						for (int c = 0; c < dim; c++)
						{
							if (observationB.StartPoint.Equals(point))
							{
								if (c == 0 && dim != 1)
								{
									b = observationB.diffXs();
								}
								else if (c == 1)
								{
									b = observationB.diffYs();
								}
								else if (c == 2 || dim == 1)
								{
									b = observationB.diffZs();
								}
							}
							else if (observationB.EndPoint.Equals(point))
							{
								if (c == 0 && dim != 1)
								{
									b = observationB.diffXe();
								}
								else if (c == 1)
								{
									b = observationB.diffYe();
								}
								else if (c == 2 || dim == 1)
								{
									b = observationB.diffZe();
								}
							}
							BTPv.set(c, BTPv.get(c) + b * vB / qB);

							for (int j = 0; j < observations.size(); j++)
							{
								Observation observationBT = observations.get(j);
								if (this.congruenceAnalysis && this.freeNetwork && observationBT.ObservationGroup.getEpoch() == Epoch.REFERENCE)
								{
									continue;
								}
								double qll = this.getQllElement(observationBT, observationB);
								double qBT = observationBT.StdApriori * observationBT.StdApriori;
								// P*Qvv*P
								// P*(Qll - Q_ll)*P
								// (P*Qll - P*Q_ll)*P
								// (I - P*Q_ll)*P
								// (P - P*Q_ll*P)

								// Numerische Null wird auf Hauptdiagonale zu Null gesetzt, um Summation von "Fragmenten" zu unterbinden
								double pqvvp = k == j ? Math.Max(1.0 / qBT - qll / qBT / qB, 0.0) : -qll / qBT / qB;

								for (int r = 0; r < dim; r++)
								{
									double bT = 0.0;
									if (observationBT.StartPoint.Equals(point))
									{
										if (r == 0 && dim != 1)
										{
											bT = observationBT.diffXs();
										}
										else if (r == 1)
										{
											bT = observationBT.diffYs();
										}
										else if (r == 2 || dim == 1)
										{
											bT = observationBT.diffZs();
										}
									}
									else if (observationBT.EndPoint.Equals(point))
									{
										if (r == 0 && dim != 1)
										{
											bT = observationBT.diffXe();
										}
										else if (r == 1)
										{
											bT = observationBT.diffYe();
										}
										else if (r == 2 || dim == 1)
										{
											bT = observationBT.diffZe();
										}
									}
									BTPQvvPB.set(r,c, BTPQvvPB.get(r,c) + bT * pqvvp * b);
								}
							}
						}
					}
					Matrix Qnn = new DenseMatrix(dim, dim);
					Vector nabla = new DenseVector(dim);
					bool isCalculated = false;
					ConfidenceRegion confidenceRegion = null;

					if (hasObservationInReferenceEpoch && observations.size() >= dim)
					{
						try
						{
							Qnn = MathExtension.pinv(BTPQvvPB, -1);
							confidenceRegion = new ConfidenceRegion(Qnn);
							isCalculated = true;
						}
						catch (NotConvergedException nce)
						{
							Console.WriteLine(nce.ToString());
							Console.Write(nce.StackTrace);
							isCalculated = false;
						}
					}

					if (!isCalculated)
					{
						continue;
					}


					if (this.estimationType == EstimationType.SIMULATION)
					{
						Vector nabla0 = new DenseVector(dim);
						// Nichtzentralitaetsparameter ist noch nicht bestimmt, 
						// sodass GRZW ein vorlaeufiger Wert ist, 
						// der nabla*Pnn*nabla == 1 erfuellt.
						for (int j = 0; j < dim; j++)
						{
							nabla0.set(j, confidenceRegion.getMinimalDetectableBias(j));
						}

						//point.setMinimalDetectableBiases(Matrices.getArray(nabla0));
						point.MaximumTolerableBiases = Matrices.getArray(nabla0);
					}
					else
					{
						Qnn.mult(BTPv, nabla);
						double normNabla = nabla.norm(Vector.Norm.Two);
						point.NablaCoVarNabla = Math.Abs(nabla.dot(BTPv));
						point.GrossErrors = Matrices.getArray(nabla.scale(-1.0));

						// Bestimme Nabla auf der Grenzwertellipse mit nabla0*Pnn*nabla0 == 1
						Vector nabla0 = new DenseVector(nabla, true);
						Vector PQvvPnabla0 = new DenseVector(nabla0);
						BTPQvvPB.mult(nabla0, PQvvPnabla0);
						double nQn0 = nabla0.dot(PQvvPnabla0);
						if (normNabla < SQRT_EPS || nQn0 <= 0)
						{
							for (int j = 0; j < dim; j++)
							{
								nabla0.set(j, confidenceRegion.getMinimalDetectableBias(j));
							}
						}
						else
						{
							for (int j = 0; j < dim; j++)
							{
								nabla0.set(j, nabla0.get(j) / Math.Sqrt(nQn0));
							}
						}
						//point.setMinimalDetectableBiases(Matrices.getArray(nabla0));
						point.MaximumTolerableBiases = Matrices.getArray(nabla0);
					}
				}

				for (int i = 0; i < this.pointsWithReferenceDeflection.Count; i++)
				{
					if (this.interrupt_Conflict)
					{
						return;
					}
					Point point = this.pointsWithReferenceDeflection[i];
					bool isStation = false;

					VerticalDeflectionX deflectionX = point.VerticalDeflectionX;
					VerticalDeflectionY deflectionY = point.VerticalDeflectionY;

					ObservationGroup observations = point.Observations;
					int dim = 2;

					Matrix BTPQvvPB = new DenseMatrix(dim,dim);
					Vector BTPv = new DenseVector(dim);

					for (int k = 0; k < observations.size(); k++)
					{
						Observation observationB = observations.get(k);

						double qB = observationB.StdApriori * observationB.StdApriori;
						double vB = this.estimationType == EstimationType.SIMULATION ? 0.0 : -observationB.Correction;
						double b = 0.0;
						vB = Math.Abs(vB) < SQRT_EPS ? 0.0 : vB;

						for (int c = 0; c < dim; c++)
						{
							if (observationB.StartPoint.Equals(point))
							{
								if (c == 0)
								{
									b = observationB.diffVerticalDeflectionXs();
								}
								else if (c == 1)
								{
									b = observationB.diffVerticalDeflectionYs();
								}
								isStation = true;
							}
							else if (observationB.EndPoint.Equals(point))
							{
								if (c == 0)
								{
									b = observationB.diffVerticalDeflectionXe();
								}
								else if (c == 1)
								{
									b = observationB.diffVerticalDeflectionYe();
								}
							}
							BTPv.set(c, BTPv.get(c) + b * vB / qB);

							for (int j = 0; j < observations.size(); j++)
							{
								Observation observationBT = observations.get(j);

								double qll = this.getQllElement(observationBT, observationB);
								double qBT = observationBT.StdApriori * observationBT.StdApriori;
								// P*Qvv*P
								// P*(Qll - Q_ll)*P
								// (P*Qll - P*Q_ll)*P
								// (I - P*Q_ll)*P
								// (P - P*Q_ll*P)

								// Numerische Null wird auf Hauptdiagonale zu Null gesetzt, um Summation von "Fragmenten" zu unterbinden
								double pqvvp = k == j ? Math.Max(1.0 / qBT - qll / qBT / qB, 0.0) : -qll / qBT / qB;

								for (int r = 0; r < dim; r++)
								{
									double bT = 0.0;
									if (observationBT.StartPoint.Equals(point))
									{
										if (r == 0)
										{
											bT = observationBT.diffVerticalDeflectionXs();
										}
										else if (r == 1)
										{
											bT = observationBT.diffVerticalDeflectionYs();
										}
									}
									else if (observationBT.EndPoint.Equals(point))
									{
										if (r == 0)
										{
											bT = observationBT.diffVerticalDeflectionXe();
										}
										else if (r == 1)
										{
											bT = observationBT.diffVerticalDeflectionYe();
										}
									}
									BTPQvvPB.set(r,c, BTPQvvPB.get(r,c) + bT * pqvvp * b);
								}
							}
						}
					}
					Matrix Qnn = new DenseMatrix(dim, dim);
					Vector nabla = new DenseVector(dim);
					bool isCalculated = false;
					ConfidenceRegion confidenceRegion = null;

					if (isStation && observations.size() >= dim)
					{
						try
						{
							Qnn = MathExtension.pinv(BTPQvvPB, -1);
							confidenceRegion = new ConfidenceRegion(Qnn);
							isCalculated = true;
						}
						catch (NotConvergedException nce)
						{
							Console.WriteLine(nce.ToString());
							Console.Write(nce.StackTrace);
							isCalculated = false;
						}
					}

					if (!isCalculated)
					{
						continue;
					}

					if (this.estimationType == EstimationType.SIMULATION)
					{
						// Nichtzentralitaetsparameter ist noch nicht bestimmt, 
						// sodass GRZW ein vorlaeufiger Wert ist, 
						// der nabla*Pnn*nabla == 1 erfuellt.
						//deflectionX.setMinimalDetectableBias(confidenceRegion.getMinimalDetectableBias(0));
						//deflectionY.setMinimalDetectableBias(confidenceRegion.getMinimalDetectableBias(1));
						deflectionX.MaximumTolerableBias = confidenceRegion.getMinimalDetectableBias(0);
						deflectionY.MaximumTolerableBias = confidenceRegion.getMinimalDetectableBias(1);
					}
					else
					{
						Qnn.mult(BTPv, nabla);
						deflectionX.NablaCoVarNabla = Math.Abs(nabla.dot(BTPv));
						nabla = nabla.scale(-1.0);
						deflectionX.GrossError = nabla.get(0);
						deflectionY.GrossError = nabla.get(1);

						// Bestimme Nabla auf der Grenzwertellipse mit nabla0*Pnn*nabla0 == 1
						Vector nabla0 = new DenseVector(nabla, true);
						Vector PQvvPnabla0 = new DenseVector(nabla0);
						BTPQvvPB.mult(nabla0, PQvvPnabla0);
						double nQn0 = nabla0.dot(PQvvPnabla0);
						if (nQn0 > 0)
						{
							//deflectionX.setMinimalDetectableBias(nabla0.get(0)/Math.sqrt(nQn0));
							//deflectionY.setMinimalDetectableBias(nabla0.get(1)/Math.sqrt(nQn0));
							deflectionX.MaximumTolerableBias = nabla0.get(0) / Math.Sqrt(nQn0);
							deflectionY.MaximumTolerableBias = nabla0.get(1) / Math.Sqrt(nQn0);
						}
					}
				}

				for (int i = 0; i < this.referencePoints.Count; i++)
				{
					if (this.interrupt_Conflict)
					{
						return;
					}
					Point point = this.referencePoints[i];
					int dim = point.Dimension;
					point.calcStochasticParameters(sigma2apost, dof, this.applyAposterioriVarianceOfUnitWeight);

					TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, double.PositiveInfinity);
					TestStatisticParameterSet tsPost = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, dof - dim);
					double sqrtLambda = Math.Sqrt(Math.Abs(tsPrio.NoncentralityParameter));
					double kPrio = tsPrio.Quantile;
					double kPost = tsPost.Quantile;

					double[] mdb = new double[dim];
					if (dim != 1)
					{
						mdb[0] = point.MaximumTolerableBiasX * sqrtLambda;
						mdb[1] = point.MaximumTolerableBiasY * sqrtLambda;
					}
					if (dim != 2)
					{
						mdb[dim - 1] = point.MaximumTolerableBiasZ * sqrtLambda;
					}

					point.MinimalDetectableBiases = mdb;
					// Bei Simulation ist Nabla == GRZW, da keine Fehler bestimmbar sind
					if (this.estimationType == EstimationType.SIMULATION)
					{
						point.GrossErrors = mdb;
					}
					else
					{
						double tPrio = point.Tprio;
						double tPost = this.applyAposterioriVarianceOfUnitWeight ? point.Tpost : 0.0;
						double pPrio = TestStatistic.getLogarithmicProbabilityValue(tPrio, dim);
						double pPost = this.applyAposterioriVarianceOfUnitWeight ? TestStatistic.getLogarithmicProbabilityValue(tPost, dim, dof - dim) : 0.0;
						point.setProbabilityValues(pPrio, pPost);
						point.Significant = tPrio > kPrio || tPost > kPost;
					}
				}

				for (int i = 0; i < this.pointsWithReferenceDeflection.Count; i++)
				{
					if (this.interrupt_Conflict)
					{
						return;
					}
					Point point = this.pointsWithReferenceDeflection[i];

					VerticalDeflectionX deflectionX = point.VerticalDeflectionX;
					VerticalDeflectionY deflectionY = point.VerticalDeflectionY;

					int dim = 2;
					deflectionX.calcStochasticParameters(sigma2apost, dof, this.applyAposterioriVarianceOfUnitWeight);

					TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, double.PositiveInfinity);
					TestStatisticParameterSet tsPost = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, dof - dim);

					double lamda = Math.Sqrt(Math.Abs(tsPrio.NoncentralityParameter));
					double kPrio = tsPrio.Quantile;
					double kPost = tsPost.Quantile;

					deflectionX.MinimalDetectableBias = lamda * deflectionX.MaximumTolerableBias;
					deflectionY.MinimalDetectableBias = lamda * deflectionY.MaximumTolerableBias;

					// Bei Simulation ist Nabla == GRZW, da keine Fehler bestimmbar sind
					if (this.estimationType == EstimationType.SIMULATION)
					{
						deflectionX.GrossError = deflectionX.MinimalDetectableBias;
						deflectionY.GrossError = deflectionY.MinimalDetectableBias;
					}
					else
					{
						double tPrio = deflectionX.Tprio;
						double tPost = this.applyAposterioriVarianceOfUnitWeight ? deflectionX.Tpost : 0.0;
						double pPrio = TestStatistic.getLogarithmicProbabilityValue(tPrio, dim);
						double pPost = this.applyAposterioriVarianceOfUnitWeight ? TestStatistic.getLogarithmicProbabilityValue(tPost, dim, dof - dim) : 0.0;
						// deflectionX stellt stoch. Parameter fuer beide Lotparameter bereit
						deflectionX.Pprio = pPrio;
						deflectionX.Ppost = pPost;
						deflectionX.Significant = tPrio > kPrio || tPost > kPost;
					}
				}

				this.addStochasticParameters2Observations(sigma2apost, dof, sigma2PointMax);
				this.traceCxxPoints = 0;

				for (int i = 0; i < this.unknownParameters.size(); i++)
				{
					if (this.interrupt_Conflict)
					{
						return;
					}
					UnknownParameter unknownParameter = this.unknownParameters.get(i);
					int col = unknownParameter.ColInJacobiMatrix;
					if (unknownParameter is Point)
					{
						Point point = (Point)unknownParameter;
						int dim = point.Dimension;
						int subCol = 0;
						// Hole Submatrix aus Qxx zur Bestimmung der Konfidenzbereiche
						Matrix subQxx = new UpperSymmPackMatrix(dim);
						for (int r = 0; r < dim; r++)
						{
							for (int c = r; c < dim; c++)
							{
								double qxx = this.Qxx.get(col + r, col + c);
								subQxx.set(r, c, qxx);
								if (c == r)
								{
									this.traceCxxPoints += sigma2apost * qxx;
								}
							}
						}

						try
						{
							ConfidenceRegion confidenceRegion = new ConfidenceRegion(this.significanceTestStatisticParameters, subQxx);
							if (confidenceRegion != null)
							{
								point.ConfidenceRegion = confidenceRegion;
							}
						}
						catch (Exception e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}

						if (point.RowInJacobiMatrix >= 0 || (this.congruenceAnalysis && this.freeNetwork))
						{
							point.calcStochasticParameters(sigma2apost, dof, this.applyAposterioriVarianceOfUnitWeight);

							TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, double.PositiveInfinity);
							TestStatisticParameterSet tsPost = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, dof - dim);
							double lamda = Math.Sqrt(Math.Abs(tsPrio.NoncentralityParameter));
							double Kprio = tsPrio.Quantile;
							double Kpost = tsPost.Quantile;

							double[] mdb = new double[dim];
							double[] ep = new double[dim];
							if (dim != 1)
							{
								mdb[0] = point.MaximumTolerableBiasX * lamda;
								mdb[1] = point.MaximumTolerableBiasY * lamda;

								ep[0] = point.InfluenceOnPointPositionX * lamda;
								ep[1] = point.InfluenceOnPointPositionY * lamda;
							}
							if (dim != 2)
							{
								mdb[dim - 1] = point.MaximumTolerableBiasZ * lamda;
								ep[dim - 1] = point.InfluenceOnPointPositionZ * lamda;
							}

							point.MinimalDetectableBiases = mdb;
							// Bei Simulation ist Nabla == GRZW, da keine Fehler bestimmbar sind
							// EP und EFSP wird dann aus GRZW bestimmt
							if (this.estimationType == EstimationType.SIMULATION)
							{
								point.InfluenceOnNetworkDistortion = point.InfluenceOnNetworkDistortion * lamda * Math.Sqrt(sigma2PointMax);
								point.GrossErrors = mdb;
								point.InfluencesOnPointPosition = ep;
							}
							else
							{
								point.InfluenceOnNetworkDistortion = point.InfluenceOnNetworkDistortion * Math.Sqrt(sigma2PointMax);
								double tPrio = point.Tprio;
								double tPost = this.applyAposterioriVarianceOfUnitWeight ? point.Tpost : 0.0;
								double pPrio = TestStatistic.getLogarithmicProbabilityValue(tPrio, dim);
								double pPost = this.applyAposterioriVarianceOfUnitWeight ? TestStatistic.getLogarithmicProbabilityValue(tPost, dim, dof - dim) : 0.0;
								point.setProbabilityValues(pPrio, pPost);
								point.Significant = tPrio > Kprio || tPost > Kpost || this.adaptedPointUncertainties.ContainsKey(point);
							}
						}

						if (dim != 1)
						{
							double stdX = Math.Sqrt(sigma2apost * subQxx.get(subCol, subCol++));
							double stdY = Math.Sqrt(sigma2apost * subQxx.get(subCol, subCol++));

							point.StdX = stdX;
							point.StdY = stdY;
						}
						if (dim != 2)
						{
							double stdZ = Math.Sqrt(sigma2apost * subQxx.get(subCol, subCol++));
							point.StdZ = stdZ;
						}
					}
					else if (unknownParameter is VerticalDeflection)
					{
						// VerticalDeflectionY is taken from the VerticalDeflectionX Component
						// --> No need to modify again
						// Stochastic deflection checked for outliers
						if (unknownParameter is VerticalDeflectionX)
						{
							VerticalDeflectionX deflectionX = (VerticalDeflectionX)unknownParameter;
							VerticalDeflectionY deflectionY = deflectionX.Point.VerticalDeflectionY;

							int[] cols = new int[] {deflectionX.ColInJacobiMatrix, deflectionY.ColInJacobiMatrix};
							// Hole Submatrix aus Qxx zur Bestimmung der Konfidenzbereiche
							Matrix subQxx = new UpperSymmPackMatrix(2);
							for (int r = 0; r < cols.Length; r++)
							{
								for (int c = r; c < cols.Length; c++)
								{
									double qxx = this.Qxx.get(cols[r], cols[c]);
									subQxx.set(r, c, qxx);
								}
							}

							try
							{
								ConfidenceRegion confidence = new ConfidenceRegion(this.significanceTestStatisticParameters, subQxx);
								if (confidence != null)
								{
									deflectionX.Confidence = confidence.getConfidenceAxis(0);
									deflectionY.Confidence = confidence.getConfidenceAxis(1);
								}
							}
							catch (Exception e)
							{
								Console.WriteLine(e.ToString());
								Console.Write(e.StackTrace);
							}

							deflectionX.Std = Math.Sqrt(sigma2apost * subQxx.get(0, 0));
							deflectionY.Std = Math.Sqrt(sigma2apost * subQxx.get(1, 1));

							if ((deflectionX.RowInJacobiMatrix >= 0 && deflectionY.RowInJacobiMatrix >= 0) || (this.congruenceAnalysis && this.freeNetwork))
							{
								deflectionX.calcStochasticParameters(sigma2apost, dof, this.applyAposterioriVarianceOfUnitWeight);

								TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(2, double.PositiveInfinity);
								TestStatisticParameterSet tsPost = this.significanceTestStatisticParameters.getTestStatisticParameter(2, dof - 2);

								double lamda = Math.Sqrt(Math.Abs(tsPrio.NoncentralityParameter));
								double Kprio = tsPrio.Quantile;
								double Kpost = tsPost.Quantile;

								deflectionX.MinimalDetectableBias = deflectionX.MaximumTolerableBias * lamda;
								deflectionY.MinimalDetectableBias = deflectionY.MaximumTolerableBias * lamda;

								// Bei Simulation ist Nabla == GRZW, da keine Fehler bestimmbar sind
								if (this.estimationType == EstimationType.SIMULATION)
								{
									deflectionX.GrossError = deflectionX.MinimalDetectableBias;
									deflectionY.GrossError = deflectionY.MinimalDetectableBias;
								}
								else
								{
									double tPrio = deflectionX.Tprio;
									double tPost = this.applyAposterioriVarianceOfUnitWeight ? deflectionX.Tpost : 0.0;
									double pPrio = TestStatistic.getLogarithmicProbabilityValue(tPrio, 2);
									double pPost = this.applyAposterioriVarianceOfUnitWeight ? TestStatistic.getLogarithmicProbabilityValue(tPost, 2, dof - 2) : 0.0;
									deflectionX.Pprio = pPrio;
									deflectionX.Ppost = pPost;
									deflectionX.Significant = tPrio > Kprio || tPost > Kpost || this.adaptedVerticalDeflectionUncertainties.ContainsKey(deflectionX) || this.adaptedVerticalDeflectionUncertainties.ContainsKey(deflectionY);
								}
							}
						}
					}
					else if (unknownParameter is AdditionalUnknownParameter)
					{
						AdditionalUnknownParameter additionalUnknownParameter = (AdditionalUnknownParameter) unknownParameter;

						double qxxPrio = Math.Abs(this.Qxx.get(col, col));
						double qxxPost = sigma2apost * qxxPrio;

						TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(1, double.PositiveInfinity);
						TestStatisticParameterSet tsPost = this.significanceTestStatisticParameters.getTestStatisticParameter(1, dof);
						double lambda = tsPrio.NoncentralityParameter;
						double kPrio = tsPrio.Quantile;
						double kPost = tsPost.Quantile;

						double value = additionalUnknownParameter.Value;
						if (additionalUnknownParameter.ParameterType == ParameterType.ORIENTATION || additionalUnknownParameter.ParameterType == ParameterType.ROTATION_X || additionalUnknownParameter.ParameterType == ParameterType.ROTATION_Y || additionalUnknownParameter.ParameterType == ParameterType.ROTATION_Z)
						{
							value = MathExtension.MOD(value, 2.0 * Math.PI);
							if (Math.Abs(2.0 * Math.PI - value) < Math.Abs(value))
							{
								value = value - 2.0 * Math.PI;
							}
						}

						double nabla = value - additionalUnknownParameter.ExpectationValue;
						double tPrio = qxxPrio < Constant.EPS ? double.PositiveInfinity : nabla * nabla / qxxPrio;
						double tPost = this.applyAposterioriVarianceOfUnitWeight ? (qxxPost < Constant.EPS ? double.PositiveInfinity : nabla * nabla / qxxPost) : 0.0;
						double pPrio = TestStatistic.getLogarithmicProbabilityValue(tPrio, 1);
						double pPost = this.applyAposterioriVarianceOfUnitWeight ? TestStatistic.getLogarithmicProbabilityValue(tPost, 1, dof) : 0.0;
						double nabla0 = Math.Sign(nabla) * Math.Sqrt(Math.Abs(lambda * qxxPrio));

						additionalUnknownParameter.Tprio = tPrio;
						additionalUnknownParameter.Tpost = tPost;
						additionalUnknownParameter.Pprio = pPrio;
						additionalUnknownParameter.Ppost = pPost;
						additionalUnknownParameter.Std = Math.Sqrt(qxxPost);
						additionalUnknownParameter.GrossError = nabla;
						additionalUnknownParameter.Significant = tPrio > kPrio || tPost > kPost;
						additionalUnknownParameter.MinimalDetectableBias = nabla0;
						additionalUnknownParameter.Confidence = Math.Sqrt(qxxPost * kPrio);
					}
					// Strain-Parameter und Lotabweichungen werden gesondert bearbeitet, da teilw. Hilfsparameter bestimmt werden, die keine Interpretation zulassen
					else if (unknownParameter is StrainParameter)
					{
					}
					else
					{
						Console.Error.WriteLine(this.GetType() + " Fehler, unbekannter Parametertyp! " + unknownParameter);
					}
				}

				// DeformationsAnalyse
				if (this.congruenceAnalysisGroup != null && this.congruenceAnalysisGroup.Count > 0)
				{
					foreach (CongruenceAnalysisGroup tieGroup in this.congruenceAnalysisGroup)
					{
						int dim = tieGroup.Dimension;
						StrainAnalysisEquations strainAnalysisEquations = tieGroup.StrainAnalysisEquations;

						// Auswertung der einzelnen Deformationsvektoren (Punkt-zu-Punkt Verschiebungen)
						foreach (bool flag in new bool[] {false, true})
						{
							TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, double.PositiveInfinity);
							TestStatisticParameterSet tsPost = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, dof);
							double sqrtLambda = Math.Sqrt(Math.Abs(tsPrio.NoncentralityParameter));
							double kPrio = tsPrio.Quantile;
							double kPost = tsPost.Quantile;

							for (int tieIdx = 0; tieIdx < tieGroup.size(flag); tieIdx++)
							{
								CongruenceAnalysisPointPair tie = tieGroup.get(tieIdx, flag);
								if (this.interrupt_Conflict)
								{
									return;
								}
								Point p0 = tie.StartPoint;
								Point p1 = tie.EndPoint;

								int colP0 = p0.ColInJacobiMatrix;
								int colP1 = p1.ColInJacobiMatrix;
								Matrix tieQxx = new UpperSymmPackMatrix(dim);

								// Sind beides Festpunkte, brich ab
								if (colP0 < 0 && colP1 < 0)
								{
									continue;
								}

								// Ist nur P0 ein Festpunkt, nutze die Matrix von P1
								if (colP0 < 0)
								{
									// wenn nur die Z-Komponente zu pruefen ist,
									// passe CoVar-Index bei 3D-Punkten an
									if (dim == 1 && p1.Dimension == 3)
									{
										colP1 += 2;
									}

									for (int r = 0; r < dim; r++)
									{
										for (int c = r; c < dim; c++)
										{
											double q22 = this.Qxx.get(colP1 + r, colP1 + c);
											tieQxx.set(r, c, q22);
										}
									}
								}
								// Ist nur P1 ein Festpunkt, nutze die Matrix von P0
								else if (colP1 < 0)
								{
									// wenn nur die Z-Komponente zu pruefen ist,
									// passe CoVar-Index bei 3D-Punkten an
									if (dim == 1 && p0.Dimension == 3)
									{
										colP0 += 2;
									}

									for (int r = 0; r < dim; r++)
									{
										for (int c = r; c < dim; c++)
										{
											double q11 = this.Qxx.get(colP0 + r, colP0 + c);
											tieQxx.set(r, c, q11);
										}
									}
								}
								// Nutze die Matrix von P0 und P1
								else
								{
									// wenn nur die Z-Komponente zu pruefen ist,
									// passe CoVar-Index bei 3D-Punkten an
									if (dim == 1 && p0.Dimension == 3)
									{
										colP0 += 2;
									}
									if (dim == 1 && p1.Dimension == 3)
									{
										colP1 += 2;
									}

									for (int r = 0; r < dim; r++)
									{
										for (int c = r; c < dim; c++)
										{
											double q11 = this.Qxx.get(colP0 + r, colP0 + c);
											double q22 = this.Qxx.get(colP1 + r, colP1 + c);
											double q12 = this.Qxx.get(colP0 + r, colP1 + c);
											double q21 = this.Qxx.get(colP1 + r, colP0 + c);
											tieQxx.set(r, c, (q11 - q12 - q21 + q22));
										}
									}
								}

								double[] nabla = new double[dim];
								double[] mdb = new double[dim];
								double normNabla = 0;
								if (dim != 1)
								{
									nabla[0] = p0.X - p1.X;
									nabla[1] = p0.Y - p1.Y;
								}
								if (dim != 2)
								{
									nabla[dim - 1] = p0.Z - p1.Z;
								}

								for (int r = 0; r < dim; r++)
								{
									normNabla += nabla[r] * nabla[r];
								}
								normNabla = Math.Sqrt(normNabla);

								Matrix tiePxx = null;
								try
								{
									tiePxx = MathExtension.pinv(tieQxx, -1);
								}
								catch (NotConvergedException nce)
								{
									Console.WriteLine(nce.ToString());
									Console.Write(nce.StackTrace);
								}

								if (tiePxx != null && normNabla > SQRT_EPS)
								{
									double tPost = 0.0;
									double tPrio = 0.0;
									for (int r = 0; r < dim; r++)
									{
										double tmp = 0;
										for (int c = 0; c < dim; c++)
										{
											tmp += tiePxx.get(r, c) * nabla[c];
										}
										tPrio += tmp * nabla[r];
									}
									tPrio /= dim;
									tPost = this.applyAposterioriVarianceOfUnitWeight && sigma2apost > SQRT_EPS ? tPrio / sigma2apost : 0.0;

									Vector nabla0 = new DenseVector(nabla, true);
									Vector PxxNabla0 = new DenseVector(nabla0, true);
									tiePxx.mult(nabla0, PxxNabla0);
									double nPn0 = nabla0.dot(PxxNabla0);
									if (nPn0 > 0)
									{
										for (int j = 0; j < dim; j++)
										{
											mdb[j] = sqrtLambda * nabla0.get(j) / Math.Sqrt(nPn0);
										}
									}
									tie.setTeststatisticValues(tPrio, tPost);
								}

								double[] sigma = new double[dim];

								for (int i = 0; i < dim; i++)
								{
									sigma[i] = Math.Sqrt(sigma2apost * Math.Abs(tieQxx.get(i, i)));
								}

								try
								{
									ConfidenceRegion confidenceRegion = new ConfidenceRegion(this.significanceTestStatisticParameters, tieQxx);
									if (confidenceRegion != null)
									{
										tie.ConfidenceRegion = confidenceRegion;
										if (normNabla < SQRT_EPS)
										{ // this.estimationType == EstimationType.SIMULATION) {
											for (int i = 0; i < dim; i++)
											{
												mdb[i] = sqrtLambda * confidenceRegion.getMinimalDetectableBias(i);
												if (this.estimationType != EstimationType.SIMULATION)
												{
													nabla[i] = mdb[i];
												}
											}
										}
									}
									confidenceRegion = null;
								}
								catch (Exception e)
								{
									Console.WriteLine(e.ToString());
									Console.Write(e.StackTrace);
								}

								tie.MinimalDetectableBiases = mdb;
								tie.GrossErrors = nabla;
								tie.Sigma = sigma;

								double tPrio = tie.Tprio;
								double tPost = this.applyAposterioriVarianceOfUnitWeight ? tie.Tpost : 0.0;
								double pPrio = TestStatistic.getLogarithmicProbabilityValue(tPrio, dim);
								double pPost = this.applyAposterioriVarianceOfUnitWeight ? TestStatistic.getLogarithmicProbabilityValue(tPost, dim, dof) : 0.0;

								tie.setProbabilityValues(pPrio, pPost);
								tie.Significant = tPrio > kPrio || tPost > kPost;
							}
						}

						// Bestimmung der Modellstoerungen bei Strain-Analyse der einzelnen Vektoren; ueberschreibt GRZW, NABLA, Tprio, Tpost
						// Bestimmung der signifikanten Strain-Parameter im Modell
						int nou = strainAnalysisEquations.numberOfParameters();
						int nor = strainAnalysisEquations.numberOfRestrictions();
						int not = tieGroup.size(true);
						if (this.congruenceAnalysis && this.freeNetwork && strainAnalysisEquations.hasUnconstraintParameters() && not * dim + nor >= nou)
						{

							TestStatisticParameterSet tsPrioParam = this.significanceTestStatisticParameters.getTestStatisticParameter(1, double.PositiveInfinity);
							TestStatisticParameterSet tsPostParam = this.significanceTestStatisticParameters.getTestStatisticParameter(1, dof);
							double sqrtLambdaParam = Math.Sqrt(Math.Abs(tsPrioParam.NoncentralityParameter));
							double kPrioParam = tsPrioParam.Quantile;
							double kPostParam = tsPostParam.Quantile;

							TestStatisticParameterSet tsPrioTie = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, double.PositiveInfinity);
							TestStatisticParameterSet tsPostTie = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, dof - dim);
							double sqrtLambdaTie = Math.Sqrt(Math.Abs(tsPrioTie.NoncentralityParameter));
							double kPrioTie = tsPrioTie.Quantile;
							double kPostTie = tsPostTie.Quantile;

							Matrix subQxx = new UpperSymmPackMatrix(strainAnalysisEquations.numberOfParameters());
							for (int r = 0; r < subQxx.numRows(); r++)
							{
								int row = strainAnalysisEquations.get(r).getColInJacobiMatrix();
								for (int c = r; c < subQxx.numRows(); c++)
								{
									int col = strainAnalysisEquations.get(c).getColInJacobiMatrix();
									subQxx.set(r,c, this.Qxx.get(row, col));
								}
							}
							strainAnalysisEquations.expandParameters(sigma2apost, subQxx, this.applyAposterioriVarianceOfUnitWeight);
							for (int i = 0; i < strainAnalysisEquations.numberOfParameters(); i++)
							{
								StrainParameter parameter = strainAnalysisEquations.get(i);
								parameter.MinimalDetectableBias = parameter.MinimalDetectableBias * sqrtLambdaParam;
								double tPrio = parameter.Tprio;
								double tPost = this.applyAposterioriVarianceOfUnitWeight ? parameter.Tpost : 0.0;
								double pPrio = TestStatistic.getLogarithmicProbabilityValue(tPrio, 1);
								double pPost = this.applyAposterioriVarianceOfUnitWeight ? TestStatistic.getLogarithmicProbabilityValue(tPost, 1, dof) : 0.0;
								parameter.Confidence = parameter.Std * Math.Sqrt(kPrioParam);
								parameter.Pprio = pPrio;
								parameter.Ppost = pPost;
								parameter.Significant = tPrio > kPrioParam || tPost > kPostParam;
							}

							for (int tieIdx = 0; tieIdx < tieGroup.size(true); tieIdx++)
							{
								CongruenceAnalysisPointPair tie = tieGroup.get(tieIdx, true);
								if (this.interrupt_Conflict)
								{
									return;
								}

								Point point = tie.EndPoint;

								// Sind Festpunkte dabei, brich ab
								if (tie.StartPoint.ColInJacobiMatrix < 0 || tie.EndPoint.ColInJacobiMatrix < 0)
								{
									continue;
								}

								ObservationGroup observations = point.Observations;

								Matrix BTPQvvPB = new DenseMatrix(dim,dim);
								Vector BTPv = new DenseVector(dim);

								for (int k = 0; k < observations.size(); k++)
								{
									Observation observationB = observations.get(k);
									//if (observationB.getObservationGroup().isReferenceEpoch())
									//	continue;
									double qB = observationB.StdApriori * observationB.StdApriori;
									double vB = this.estimationType == EstimationType.SIMULATION ? 0.0 : -observationB.Correction;
									double b = 0.0;
									vB = Math.Abs(vB) < SQRT_EPS ? 0.0 : vB;

									for (int c = 0; c < dim; c++)
									{
										if (observationB.StartPoint.Equals(point))
										{
											if (c == 0 && dim != 1)
											{
												b = observationB.diffXs();
											}
											else if (c == 1)
											{
												b = observationB.diffYs();
											}
											else if (c == 2 || dim == 1)
											{
												b = observationB.diffZs();
											}
										}
										else if (observationB.EndPoint.Equals(point))
										{
											if (c == 0 && dim != 1)
											{
												b = observationB.diffXe();
											}
											else if (c == 1)
											{
												b = observationB.diffYe();
											}
											else if (c == 2 || dim == 1)
											{
												b = observationB.diffZe();
											}
										}
										BTPv.set(c, BTPv.get(c) + b * vB / qB);

										for (int j = 0; j < observations.size(); j++)
										{
											Observation observationBT = observations.get(j);
											//if (observationBT.getObservationGroup().isReferenceEpoch())
											//	continue;
											double qll = this.getQllElement(observationBT, observationB);
											double qBT = observationBT.StdApriori * observationBT.StdApriori;
											// P*Qvv*P
											// P*(Qll - Q_ll)*P
											// (P*Qll - P*Q_ll)*P
											// (I - P*Q_ll)*P
											// (P - P*Q_ll*P)

											// Numerische Null wird auf Hauptdiagonale zu Null gesetzt, um Summation von "Fragmenten" zu unterbinden
											double pqvvp = k == j ? Math.Max(1.0 / qBT - qll / qBT / qB, 0.0) : -qll / qBT / qB;

											for (int r = 0; r < dim; r++)
											{
												double bT = 0.0;
												if (observationBT.StartPoint.Equals(point))
												{
													if (r == 0 && dim != 1)
													{
														bT = observationBT.diffXs();
													}
													else if (r == 1)
													{
														bT = observationBT.diffYs();
													}
													else if (r == 2 || dim == 1)
													{
														bT = observationBT.diffZs();
													}
												}
												else if (observationBT.EndPoint.Equals(point))
												{
													if (r == 0 && dim != 1)
													{
														bT = observationBT.diffXe();
													}
													else if (r == 1)
													{
														bT = observationBT.diffYe();
													}
													else if (r == 2 || dim == 1)
													{
														bT = observationBT.diffZe();
													}
												}
												BTPQvvPB.set(r,c, BTPQvvPB.get(r,c) + bT * pqvvp * b);
											}
										}
									}
								}
								Matrix Qnn = new DenseMatrix(dim, dim);
								Vector nabla = new DenseVector(dim);
								bool isCalculated = false;
								ConfidenceRegion confidenceRegion = null;

								try
								{
									if (not * dim + nor > nou)
									{
										Qnn = MathExtension.pinv(BTPQvvPB, -1);
									}
									confidenceRegion = new ConfidenceRegion(Qnn);
									isCalculated = true;
								}
								catch (NotConvergedException)
								{
									//nce.printStackTrace();
									isCalculated = false;
								}

								if (!isCalculated)
								{
									continue;
								}

								if (this.estimationType == EstimationType.SIMULATION)
								{
									Vector nabla0 = new DenseVector(dim);
									for (int j = 0; j < dim; j++)
									{
										nabla0.set(j, sqrtLambdaTie * confidenceRegion.getMinimalDetectableBias(j));
									}
									tie.MinimalDetectableBiases = Matrices.getArray(nabla0);
								}
								else
								{
									Qnn.mult(BTPv, nabla);

									double NablaQnnNabla = BTPv.dot(nabla);
									double sigma2apostTie = (dof - dim) > 0 ? (this.omega - NablaQnnNabla) / (dof - dim) : 0;
									double tPrio = NablaQnnNabla / dim;
									double tPost = this.applyAposterioriVarianceOfUnitWeight && sigma2apostTie > SQRT_EPS ? tPrio / sigma2apostTie : 0;

									nabla = nabla.scale(-1.0);
									// Bestimme Nabla auf der Grenzwertellipse mit nabla0*Pnn*nabla0 == 1
									// und skaliere mit Nicht-Zentralitaetsparameter
									Vector nabla0 = new DenseVector(nabla, true);
									Vector PQvvPnabla0 = new DenseVector(nabla0);
									BTPQvvPB.mult(nabla0, PQvvPnabla0);
									double nQn0 = nabla0.dot(PQvvPnabla0);
									if (nQn0 > 0)
									{
										for (int j = 0; j < dim; j++)
										{
											nabla0.set(j, sqrtLambdaTie * nabla0.get(j) / Math.Sqrt(nQn0));
										}
									}
									tie.MinimalDetectableBiases = Matrices.getArray(nabla0);
									tie.GrossErrors = Matrices.getArray(nabla);

									double pPrio = TestStatistic.getLogarithmicProbabilityValue(tPrio, dim);
									double pPost = this.applyAposterioriVarianceOfUnitWeight ? TestStatistic.getLogarithmicProbabilityValue(tPost, dim, dof - dim) : 0.0;

									tie.setTeststatisticValues(tPrio, tPost);
									tie.setProbabilityValues(pPrio, pPost);
									tie.Significant = tPrio > kPrioTie || tPost > kPostTie;
								}
							}
						}
					}
				}
				else
				{
					this.congruenceAnalysisGroup = new List<CongruenceAnalysisGroup>(0);
				}

				if (this.estimationType != EstimationType.SIMULATION)
				{
					foreach (VarianceComponent varianceEstimation in this.varianceComponents.Values)
					{
						double r = (long)Math.Round(varianceEstimation.Redundancy * 1.0E5, MidpointRounding.AwayFromZero) / 1.0E5;
						if (r > 0)
						{
							TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(varianceEstimation.Redundancy, double.PositiveInfinity);
							double Kprio = tsPrio.Quantile;
							varianceEstimation.KprioGroup = Kprio;
						}
					}
				}
			}
		}

		private void addSubRedundanceAndCofactor2Observations()
		{
			ISet<int> gnssObsIds = new LinkedHashSet<int>();
			for (int i = 0; i < this.numberOfObservations; i++)
			{
				Observation observation = this.projectObservations.get(i);
				bool isGNSS = observation.ObservationType == ObservationType.GNSS1D || observation.ObservationType == ObservationType.GNSS2D || observation.ObservationType == ObservationType.GNSS3D;

				if (isGNSS && gnssObsIds.Contains(observation.Id))
				{
					continue;
				}

				if (isGNSS)
				{
					gnssObsIds.Add(observation.Id);
				}

				this.addSubRedundanceAndCofactor(observation);
			}
		}

		private void addStochasticParameters2Observations(double sigma2apost, int dof, double sigma2PointMax)
		{
			ISet<int> gnssObsIds = new LinkedHashSet<int>();
			for (int i = 0; i < this.numberOfObservations; i++)
			{
				Observation observation = this.projectObservations.get(i);
				bool isCalculated = false;
				double nPn = 0.0; // Nabla*inv(Qnn)*Nabla
				bool isGNSS = observation.ObservationType == ObservationType.GNSS1D || observation.ObservationType == ObservationType.GNSS2D || observation.ObservationType == ObservationType.GNSS3D;

				if (isGNSS && gnssObsIds.Contains(observation.Id))
				{
					continue;
				}

				if (isGNSS)
				{
					gnssObsIds.Add(observation.Id);
					GNSSBaseline gnss = (GNSSBaseline)observation;
					int dim = gnss.Dimension;

					TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, double.PositiveInfinity);
					double sqrtLambda = Math.Sqrt(Math.Abs(tsPrio.NoncentralityParameter));

					IList<Observation> baseline = gnss.BaselineComponents;

					double traceR = 0;
					Matrix subPQvvP = gnss.BaselineRedundancyMatrix;
					Matrix ATQxxBP = ATQxxBP_GNSS_EP.Remove(gnss.Id);
					Matrix PAzTQzzAzP = PAzTQzzAzP_GNSS_EF.Remove(gnss.Id);

					if (subPQvvP == null)
					{
						subPQvvP = new DenseMatrix(dim,dim);
					}
					if (ATQxxBP == null)
					{
						ATQxxBP = new DenseMatrix(dim,dim);
					}
					if (PAzTQzzAzP == null)
					{
						PAzTQzzAzP = new DenseMatrix(dim,dim);
					}

					// Berechne P - PQvvP mit PR = PQvvP
					Matrix subPsubPQvvP = new DenseMatrix(subPQvvP);
					for (int k = 0; k < dim; k++)
					{
						Observation obs = baseline[k];
						double qll = obs.StdApriori * obs.StdApriori;
						traceR += Math.Abs(subPQvvP.get(k,k));
						for (int j = 0; j < dim; j++)
						{
							double pqvvp = 1.0 / qll * subPQvvP.get(k,j);
							subPQvvP.set(k,j, pqvvp);
							subPsubPQvvP.set(k,j, (k == j) ? Math.Max(1.0 / qll - pqvvp, 0) : -pqvvp);
						}
					}

					Matrix Qnn = new DenseMatrix(dim,dim);
					ConfidenceRegion confidenceRegion = null;

					try
					{
						Qnn = MathExtension.pinv(subPQvvP, -1);
						confidenceRegion = new ConfidenceRegion(Qnn);
						isCalculated = true;
					}
					catch (NotConvergedException nce)
					{
						isCalculated = false;
						Console.WriteLine(nce.ToString());
						Console.Write(nce.StackTrace);
					}
					if (isCalculated && traceR > SQRT_EPS)
					{
						if (this.estimationType == EstimationType.SIMULATION)
						{
							// Nichtzentralitaetsparameter ist noch nicht bestimmt, 
							// sodass GRZW ein vorlaeufiger Wert ist, 
							// der nabla*Pnn*nabla == 1 erfuellt.
							Vector nabla0 = new DenseVector(dim);
							Vector ep = new DenseVector(gnss.Dimension);
							for (int j = 0; j < dim; j++)
							{
								nabla0.set(j, confidenceRegion.getMinimalDetectableBias(j) * sqrtLambda);
							}

							ATQxxBP.mult(nabla0, ep);

							Vector Mnabla0 = new DenseVector(dim);
							PAzTQzzAzP.mult(nabla0, Mnabla0);
							double uz2 = nabla0.dot(Mnabla0);
							subPsubPQvvP.mult(nabla0, Mnabla0);
							double du2 = nabla0.dot(Mnabla0);

							for (int j = 0; j < dim; j++)
							{
								double nablaJ = nabla0.get(j);
								baseline[j].MinimalDetectableBias = nablaJ;
								baseline[j].MaximumTolerableBias = confidenceRegion.getMinimalDetectableBias(j);
								baseline[j].GrossError = nablaJ;
								baseline[j].InfluenceOnPointPosition = ep.get(j);
								baseline[j].InfluenceOnNetworkDistortion = Math.Sqrt(sigma2PointMax * Math.Abs(du2 - uz2));
							}
						}
						else
						{
							Vector subPv = new DenseVector(dim);
							for (int j = 0; j < dim; j++)
							{
								subPv.set(j, baseline[j].Correction / baseline[j].StdApriori / baseline[j].StdApriori);
							}

							Vector nabla = new DenseVector(gnss.Dimension);
							Vector ep = new DenseVector(gnss.Dimension);
							Qnn.mult(subPv, nabla);
							nPn = subPv.dot(nabla);
							nabla = nabla.scale(-1.0);

							ATQxxBP.mult(nabla, ep);
							Vector Mnabla = new DenseVector(dim);
							PAzTQzzAzP.mult(nabla, Mnabla);
							double uz2 = nabla.dot(Mnabla);
							subPsubPQvvP.mult(nabla, Mnabla);
							double du2 = nabla.dot(Mnabla);

							Vector nabla0 = new DenseVector(nabla, true);
							// Bestimme Nabla auf der Grenzwertellipse mit nabla0*Pnn*nabla0 == 1
							Vector PQvvPnabla0 = new DenseVector(nabla0);
							subPQvvP.mult(nabla0, PQvvPnabla0);
							double nQn0 = nabla0.dot(PQvvPnabla0);
	//						for (int j=0; j<dim; j++)
	//							if (nQn0 > 0)
	//								nabla0.set(j, nabla0.get(j)/Math.sqrt(nQn0)*lamda);

							for (int j = 0; j < dim; j++)
							{
								double mtb = 0;
								if (nQn0 > 0)
								{
									mtb = nabla0.get(j) / Math.Sqrt(nQn0);
								}

	//							baseline.get(j).setMinimalDetectableBias( nabla0.get(j) );
								baseline[j].MinimalDetectableBias = mtb * sqrtLambda;
								baseline[j].MaximumTolerableBias = mtb;
								baseline[j].GrossError = nabla.get(j);
								baseline[j].InfluenceOnPointPosition = ep.get(j);
								baseline[j].InfluenceOnNetworkDistortion = Math.Sqrt(sigma2PointMax * Math.Abs(du2 - uz2));
							}
						}
					}

					for (int j = 0; j < dim; j++)
					{
						this.addStochasticParameters2Observation(baseline[j], sigma2apost, dof, nPn);
					}
				}
				else
				{
					TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(1, double.PositiveInfinity);
					double lamda = Math.Abs(tsPrio.NoncentralityParameter);
					double qll = observation.StdApriori * observation.StdApriori;
					double r = observation.Redundancy;
					double v = this.estimationType == EstimationType.SIMULATION ? 0.0 : observation.Correction;
					double pvv = v * v / qll;
					double mdb = r > SQRT_EPS ? Math.Sqrt(Math.Abs(lamda * qll / r)) : 0.0;
					double mtb = r > SQRT_EPS ? Math.Sqrt(Math.Abs(qll / r)) : 0.0;
					observation.MinimalDetectableBias = mdb;
					observation.MaximumTolerableBias = mtb;

					// Distance fuer Richtungen/Zenitwinkel zur Bestimmung von EP in [m]
					double dist = 1;
					if (observation.ObservationType == ObservationType.DIRECTION || observation.ObservationType == ObservationType.ZENITH_ANGLE)
					{
						if (observation.StartPoint.Dimension == 3 && observation.EndPoint.Dimension == 3)
						{
							dist = observation.CalculatedDistance3D;
						}
						else
						{
							dist = observation.CalculatedDistance2D;
						}
					}

					if (r > SQRT_EPS)
					{
						if (this.estimationType != EstimationType.SIMULATION)
						{
							nPn = pvv / r;
							double nabla = v / r;
							double uz2 = nabla * observation.InfluenceOnNetworkDistortion * nabla;
							double du2 = nabla * (1 - r) / qll * nabla;
							observation.GrossError = nabla;
							observation.InfluenceOnPointPosition = observation.InfluenceOnPointPosition * nabla * dist;
							observation.InfluenceOnNetworkDistortion = Math.Sqrt(sigma2PointMax * Math.Abs(du2 - uz2));
							observation.MinimalDetectableBias = Math.Sign(v) * mdb;
							observation.MaximumTolerableBias = Math.Sign(v) * mtb;
						}
						else
						{ // if (this.estimationType == EstimationType.SIMULATION) {
							double uz2 = mdb * observation.InfluenceOnNetworkDistortion * mdb;
							double du2 = mdb * (1 - r) / qll * mdb;

							observation.GrossError = mdb;
							observation.InfluenceOnPointPosition = observation.InfluenceOnPointPosition * mdb * dist;
							observation.InfluenceOnNetworkDistortion = Math.Sqrt(sigma2PointMax * Math.Abs(du2 - uz2));
						}
					}
					else
					{ // Redundanz zu gering
						observation.InfluenceOnPointPosition = 0.0;
						observation.InfluenceOnNetworkDistortion = 0.0;
					}

					this.addStochasticParameters2Observation(observation, sigma2apost, dof, nPn);
				}
			}
		}

		/// <summary>
		/// Berechnet die Testgroessen T<sub>prio</sub> und T<sub>post</sub>
		/// und die Standardabweichung a-posteriori fuer eine Beobachtung
		/// </summary>
		/// <param name="obs"> Beobachtung </param>
		/// <param name="sigma2apost"> Varianzfaktor a-posteriori </param>
		/// <param name="dof"> Freiheitsgrad der Gesamtausgleichung </param>
		/// <param name="nPn"> Produkt aus (&nabla;Q<sub>&nabla;&nabla;</sub>&nabla;)<sup>-1</sup> </param>
		private void addStochasticParameters2Observation(Observation obs, double sigma2apost, int dof, double nPn)
		{
			// Bestimmung der Testgroessen
			double omega = sigma2apost * (double)dof;
			double sigma2apostObs = (dof - 1.0) > 0 && omega > nPn ? (omega - nPn) / (dof - 1.0):0.0;
			sigma2apostObs = sigma2apostObs < SQRT_EPS ? 0.0 : sigma2apostObs;

			// Berechnung der entgueltigen Standardabweichungen
			double sigma = obs.Std;
			if (sigma > 0)
			{
				obs.Std = Math.Sqrt(sigma2apost * sigma * sigma);
			}

			if (this.estimationType != EstimationType.SIMULATION)
			{
				int dim = obs.ObservationType == ObservationType.GNSS1D || obs.ObservationType == ObservationType.GNSS2D || obs.ObservationType == ObservationType.GNSS3D ? ((GNSSBaseline)obs).Dimension : 1;
				double tPrio = nPn / dim;
				double tPost = (!this.applyAposterioriVarianceOfUnitWeight || sigma2apostObs == 0) ? 0.0 : tPrio / sigma2apostObs;

				double pPrio = TestStatistic.getLogarithmicProbabilityValue(tPrio, dim);
				double pPost = this.applyAposterioriVarianceOfUnitWeight ? TestStatistic.getLogarithmicProbabilityValue(tPost, dim, dof - dim) : 0.0;
				obs.setTestAndProbabilityValues(tPrio, tPost, pPrio, pPost);

				TestStatisticParameterSet tsPrio = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, double.PositiveInfinity);
				TestStatisticParameterSet tsPost = this.significanceTestStatisticParameters.getTestStatisticParameter(dim, dof - dim);
				double kPrio = tsPrio.Quantile;
				double kPost = tsPost.Quantile;

				obs.Significant = obs.Tprio > kPrio || obs.Tpost > kPost || this.adaptedObservationUncertainties.ContainsKey(obs);
			}
		}

		/// <summary>
		/// Fuegt die unbekannten Lotabweichungen zum Modell
		/// hinzu.
		/// </summary>
		private void addVerticalDeflectionToModel()
		{
			foreach (Point point in this.pointsWithUnknownDeflection)
			{
				this.addUnknownParameter(point.VerticalDeflectionX);
				this.addUnknownParameter(point.VerticalDeflectionY);
			}

			foreach (Point point in this.pointsWithStochasticDeflection)
			{
				this.addUnknownParameter(point.VerticalDeflectionX);
				this.addUnknownParameter(point.VerticalDeflectionY);
			}
		}

		/// <summary>
		/// Fuegt die stochastischen Punkte und Lotabweichungen 
		/// als Pseudo-Beobachtungen ein, dieser Schritt muss 
		/// am Ende erfolgen, damit die Punkte in der N-Matrix 
		/// zusammenhaengend stehen. 
		/// </summary>
		private void addStochasticPointsAndStochasticDeflectionToModel()
		{
			int row = this.numberOfObservations;

			foreach (Point point in this.pointsWithStochasticDeflection)
			{
				point.VerticalDeflectionX.RowInJacobiMatrix = row++;
				point.VerticalDeflectionY.RowInJacobiMatrix = row++;

				this.numberOfStochasticDeflectionRows += 2;
			}

			foreach (Point point in this.stochasticPoints)
			{
				point.RowInJacobiMatrix = row;
				row += point.Dimension;
				this.addUnknownParameter(point);
			}

			if (this.numberOfStochasticDeflectionRows > 0)
			{
				VarianceComponentType vcType = VarianceComponentType.STOCHASTIC_DEFLECTION_COMPONENT;
				if (vcType != null && !this.varianceComponents.ContainsKey(vcType))
				{
					this.varianceComponents[vcType] = new VarianceComponent(vcType);
				}
			}
		}

		/// <summary>
		/// Fuehre die Strain-Parameter als Unbekannte ein,
		/// diese werden ueber zusaetzliche Bedingungsgleichungen
		/// im Zuge der Deformationsanalyse geschaetzt.  
		/// </summary>
		private void addStrainParametersToModel()
		{
			foreach (CongruenceAnalysisGroup tieGroup in this.congruenceAnalysisGroup)
			{
				StrainAnalysisEquations strainAnalysisEquations = tieGroup.StrainAnalysisEquations;
				int nou = strainAnalysisEquations.numberOfParameters();
				int nor = strainAnalysisEquations.numberOfRestrictions();
				int not = tieGroup.size(true);
				int dim = tieGroup.Dimension;
				if (strainAnalysisEquations.hasUnconstraintParameters() && not * dim + nor >= nou)
				{
					for (int i = 0; i < strainAnalysisEquations.numberOfParameters(); i++)
					{
						if (strainAnalysisEquations.get(i).getColInJacobiMatrix() < 0)
						{
							this.addUnknownParameter(strainAnalysisEquations.get(i));
						}
					}
				}
			}
		}

		/// <summary>
		/// Fuegt einen Datumspunkt dem Projekt hinzu </summary>
		/// <param name="point"> </param>
		/// <param name="verticalDeflectionType"> </param>
		/// <returns> isAdded </returns>
		public virtual bool addDatumPoint(Point point, VerticalDeflectionType verticalDeflectionType)
		{
			if ((this.stochasticPoints == null || this.stochasticPoints.Count == 0) && (this.referencePoints == null || this.referencePoints.Count == 0) && this.addNewPoint(point, verticalDeflectionType))
			{
				this.datumPoints.Add(point);
				this.freeNetwork = true;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fuegt einen varianzfreien Punkt dem Projekt hinzu </summary>
		/// <param name="point"> </param>
		/// <param name="verticalDeflectionType"> </param>
		/// <returns> isAdded </returns>
		public virtual bool addReferencePoint(Point point, VerticalDeflectionType verticalDeflectionType)
		{
			if (this.freeNetwork || this.allPoints.ContainsKey(point.Name))
			{
				Console.Error.WriteLine(this.GetType() + " Fehler, ein Punkt mit der " + "ID " + point.Name + " existiert bereits!");
				return false;
			}

			point.ColInDesignmatrixOfModelErros = this.numberOfFixPointRows;
			this.numberOfFixPointRows += point.Dimension;
			this.referencePoints.Add(point);
			this.addObservations(point);
			this.allPoints[point.Name] = point;

			// Deflection werden nachtraeglich hinzugefuegt via addUnknownParameter()
			if (verticalDeflectionType != null)
			{
				if (verticalDeflectionType == VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION)
				{
					this.pointsWithReferenceDeflection.Add(point);
				}
				else if (verticalDeflectionType == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION)
				{
					this.pointsWithStochasticDeflection.Add(point);
				}
				else if (verticalDeflectionType == VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION)
				{
					this.pointsWithUnknownDeflection.Add(point);
				}
			}

			return true;
		}

		/// <summary>
		/// Fuegt einen stochastischen Punkt dem Projekt hinzu </summary>
		/// <param name="point"> </param>
		/// <param name="verticalDeflectionType"> </param>
		/// <returns> isAdd </returns>
		public virtual bool addStochasticPoint(Point point, VerticalDeflectionType verticalDeflectionType)
		{
			if (this.freeNetwork || this.allPoints.ContainsKey(point.Name))
			{
				Console.Error.WriteLine(this.GetType() + " Fehler, ein Punkt mit der " + "ID " + point.Name + " existiert bereits!");
				return false;
			}

			this.stochasticPoints.Add(point);
			this.allPoints[point.Name] = point;
			this.numberOfStochasticPointRows += point.Dimension;

			// Deflection werden nachtraeglich hinzugefuegt via addUnknownParameter()
			if (verticalDeflectionType != null)
			{
				if (verticalDeflectionType == VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION)
				{
					this.pointsWithReferenceDeflection.Add(point);
				}
				else if (verticalDeflectionType == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION)
				{
					this.pointsWithStochasticDeflection.Add(point);
				}
				else if (verticalDeflectionType == VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION)
				{
					this.pointsWithUnknownDeflection.Add(point);
				}
			}

			VarianceComponentType vcType = VarianceComponentType.getComponentTypeByPointDimension(point.Dimension);
			if (vcType != null && !this.varianceComponents.ContainsKey(vcType))
			{
				this.varianceComponents[vcType] = new VarianceComponent(vcType);
			}
			return true;
		}

		/// <summary>
		/// Fuegt einen Neupunkt hinzu. </summary>
		/// <param name="point"> </param>
		/// <param name="verticalDeflectionType"> </param>
		/// <returns> isAdd </returns>
		public virtual bool addNewPoint(Point point, VerticalDeflectionType verticalDeflectionType)
		{
			int dim = point.Dimension;

			if (verticalDeflectionType != null && verticalDeflectionType == VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION)
			{
				dim += 2;
			}

			if (this.allPoints.ContainsKey(point.Name))
			{
				Console.Error.WriteLine(this.GetType() + "\nFehler, ein Punkt mit der " + "ID " + point.Name + " existiert bereits!");
				return false;
			}
			else if (dim > point.Observations.size())
			{
				Console.Error.WriteLine(this.GetType() + "\nFehler, Punkt " + point.Name + " besitzt nicht genuegend " + "Beobachtungen um bestimmt zu werden! Punkt wird ignoriert. Dim = " + dim + ", Obs = " + point.Observations.size());
				return false;
			}
			this.addUnknownParameter(point);
			this.allPoints[point.Name] = point;

			// Deflection werden nachtraeglich hinzugefuegt via addUnknownParameter()
			if (verticalDeflectionType != null)
			{
				if (verticalDeflectionType == VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION)
				{
					this.pointsWithReferenceDeflection.Add(point);
				}
				else if (verticalDeflectionType == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION)
				{
					this.pointsWithStochasticDeflection.Add(point);
				}
				else if (verticalDeflectionType == VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION)
				{
					this.pointsWithUnknownDeflection.Add(point);
				}
			}

			return true;
		}

		public virtual bool addAdditionalUnknownParameter(AdditionalUnknownParameter additionalUnknownParameter)
		{
			if (this.unknownParameters.contains(additionalUnknownParameter))
			{
				Console.Error.WriteLine(this.GetType() + "\nFehler, der Parameter wurde bereits " + "hinzugefuegt!");
				return false;
			}
			// Verhindern, dass Gruppenparameter von leere Gruppen in die 
			// AGL einbezogen werden (bspw. leerer Richtungssatz --> eine Orientierung)
			else if (additionalUnknownParameter.Observations.size() == 0)
			{
				Console.Error.WriteLine(this.GetType() + "\nFehler, Parameter hat keine " + "Verknuepfung zu Beobachtungen und kann somit\nnicht bestimmt werden! " + "Parameter wird ignoriert.\n" + additionalUnknownParameter);
				return false;
			}
			this.addUnknownParameter(additionalUnknownParameter);
			return true;
		}

		/// <summary>
		/// Fuegt einen unbekannten Parameter dem Modell hinzu </summary>
		/// <param name="unknownParameter"> </param>
		private void addUnknownParameter(UnknownParameter unknownParameter)
		{
			this.addObservations(unknownParameter);
			this.unknownParameters.add(unknownParameter);
			this.numberOfUnknownParameters = this.unknownParameters.columnsInJacobi();
		}

		/// <summary>
		/// Fuegt die Beobachtungen, die die Punkte und Zusatzparameter haben,
		/// dem Modell hinzu. </summary>
		/// <param name="unknownParameter"> </param>
		private void addObservations(UnknownParameter unknownParameter)
		{
			for (int i = 0; i < unknownParameter.Observations.size(); i++)
			{
				Observation observation = unknownParameter.Observations.get(i);
				if (observation.RowInJacobiMatrix < 0)
				{
					observation.RowInJacobiMatrix = this.numberOfObservations++;
					this.projectObservations.add(observation);
					VarianceComponentType vcType = VarianceComponentType.getVarianceComponentTypeByObservationType(observation.ObservationType);
					if (vcType != null && !this.varianceComponents.ContainsKey(vcType))
					{
						this.varianceComponents[vcType] = new VarianceComponent(vcType);
					}
				}
			}
		}

		/// <summary>
		/// Fuegt eine Vektorgruppe zur Deo-Analyse dem Projekt hinzu </summary>
		/// <param name="congruenceAnalysisGroup"> </param>
		/// <returns> isAdded </returns>
		public virtual bool addCongruenceAnalysisGroup(CongruenceAnalysisGroup congruenceAnalysisGroup)
		{
			if (!this.congruenceAnalysisGroup.Contains(congruenceAnalysisGroup) && congruenceAnalysisGroup.totalSize() > 0)
			{
				this.congruenceAnalysisGroup.Add(congruenceAnalysisGroup);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Liefert alle Unbekannten im Modell </summary>
		/// <returns> unknow </returns>
		public virtual UnknownParameters UnknownParameters
		{
			get
			{
				return this.unknownParameters;
			}
		}

		/// <summary>
		/// Bestimmt den theoretischen Defekt der Normalgleichung basierend auf den terrestrischen Beobachtungen
		/// Wahlweise wird eine Defekt-Analyse durchgefuehrt. </summary>
		/// <returns> RankDefect </returns>
		/// <exception cref="IllegalArgumentException"> </exception>
		/// <exception cref="NotConvergedException"> </exception>
		public virtual RankDefect detectRankDefect()
		{
			if (!this.freeNetwork || this.rankDefect.UserDefinedRankDefect)
			{
				return this.rankDefect;
			}

			this.rankDefect.reset();

			bool is3DNet = false;
			bool is2DNet = false;
			bool is1DNet = false;

			int eigenValueDefectCounter = -1;

			for (int i = 0; i < this.datumPoints.Count; i++)
			{
				Point point = this.datumPoints[i];
				if (point.Dimension != 2)
				{
					this.rankDefect.TranslationZ = DefectType.FREE;
				}
				if (point.Dimension != 1)
				{
					this.rankDefect.TranslationX = DefectType.FREE;
					this.rankDefect.TranslationY = DefectType.FREE;
				}
				if (!is3DNet && point.Dimension == 3)
				{
					is3DNet = true;
					is2DNet = false;
					is1DNet = false;
				}
				else if (!is3DNet && !is2DNet && point.Dimension == 2)
				{
					is2DNet = true;
				}
				else if (!is3DNet && !is1DNet && point.Dimension == 1)
				{
					is1DNet = true;
				}
			}

			if (this.proofOfDatumDefectDetection)
			{
				try
				{
					int maxDefect = 0;
					if (is1DNet)
					{
						maxDefect = 2;
					}
					else if (is2DNet)
					{
						maxDefect = 4;
					}
					else
					{
						maxDefect = 7;
					}
					maxDefect = Math.Min(maxDefect, this.numberOfUnknownParameters);
					// Der Index ist Eins-Index-basierend, d.h., der kleinste Eigenwert hat den Index Eins und der groesste ist am Index n!
					NormalEquationSystem neq = this.createNormalEquation();
					UpperSymmPackMatrix N = neq.Matrix;

					Matrix[] eig = MathExtension.eig(N, this.numberOfUnknownParameters, 1, maxDefect, false);

					double[] values = new double[maxDefect];
					double threshold = 0;
					for (int i = 0; i < maxDefect; i++)
					{
						values[i] = Math.Abs(eig[0].get(i, i));
						if (is1DNet && i < 1)
						{
							threshold += values[i];
						}
						else if (is2DNet && i < 2)
						{
							threshold += values[i] / 2.0;
						}
						else if (is3DNet && i < 3)
						{
							threshold += values[i] / 3.0;
						}
					}
					threshold = 10 * (threshold + SQRT_EPS);

					eigenValueDefectCounter = 0;
					for (int i = 0; i < maxDefect; i++)
					{
						eigenValueDefectCounter += Math.Abs(eig[0].get(i, i)) < threshold ? 1 : 0;
					}
				}
				catch (Exception e) when (e is System.ArgumentException || e is NotConvergedException)
				{
					eigenValueDefectCounter = -1;
					e.printStackTrace();
				}
			}

			// Setzte alle mgl. Defekte, die von den Beobachtungen 
			// anschließend festgesetzt werden koennen. Translation
			// ist bereits festgesetzt in Schleife drueber	
			if (is1DNet)
			{
				this.rankDefect.ScaleZ = DefectType.FREE;
				this.rankDefect.RotationX = DefectType.FREE;
				this.rankDefect.RotationY = DefectType.FREE;
			}
			if (is2DNet)
			{
				this.rankDefect.ScaleXY = DefectType.FREE;
				this.rankDefect.RotationZ = DefectType.FREE;
			}
			if (is3DNet)
			{
				this.rankDefect.ScaleXYZ = DefectType.FREE;
				this.rankDefect.RotationX = DefectType.FREE;
				this.rankDefect.RotationY = DefectType.FREE;
				this.rankDefect.RotationZ = DefectType.FREE;
			}

			int deltaH2PointsWithKnownDeflections = 0;
			int zenithangleStationsWithKnownDeflections = 0;
			int gnss1DCount = 0;
			int gnss2DCount = 0;
			int gnss3DCount = 0;

			ISet<Pair<string, string>> zenithangleStationNamesWithKnownDeflections = new HashSet<Pair<string, string>>();

			for (int i = 0; i < this.numberOfObservations; i++)
			{
				Observation observation = this.projectObservations.get(i);
				ObservationGroup observationGroup = observation.ObservationGroup;
				int numberOfAddParams = observationGroup.numberOfAdditionalUnknownParameter();
				int groupSize = observationGroup.size();

				// Beobachtung (bzw. Gruppe) kann nicht zur Datumsdefinition genutzt werden
				if (groupSize <= numberOfAddParams)
				{
					continue;
				}

				if (observation is DeltaZ)
				{
					// muss dim == 1 oder 3 sein, darf keine Lotabweichungen haben oder aber besitzt Lotabweichungen die gleichzeitig aber auch Beobachtungen sind
					if ((observation.StartPoint.Dimension != 2 && observation.EndPoint.Dimension != 2) && (!observation.StartPoint.hasUnknownDeflectionParameters() || !observation.EndPoint.hasUnknownDeflectionParameters() || observation.StartPoint.hasUnknownDeflectionParameters() && observation.StartPoint.hasObservedDeflectionParameters() || observation.EndPoint.hasUnknownDeflectionParameters() && observation.EndPoint.hasObservedDeflectionParameters()))
					{

						if (!observation.StartPoint.hasUnknownDeflectionParameters())
						{
							deltaH2PointsWithKnownDeflections++;
						}
						if (!observation.EndPoint.hasUnknownDeflectionParameters())
						{
							deltaH2PointsWithKnownDeflections++;
						}
					}
					if (!is3DNet)
					{
						if (!((DeltaZ)observation).getScale().Enable)
						{
							this.rankDefect.ScaleZ = DefectType.FIXED;
						}
						else if (this.rankDefect.ScaleZ != DefectType.FIXED)
						{
							this.rankDefect.ScaleZ = DefectType.FREE;
						}

	//					this.rankDefect.setRotationX(DefectType.FIXED);
	//					this.rankDefect.setRotationY(DefectType.FIXED);
					}
					else
					{
						if (!((DeltaZ)observation).getScale().Enable)
						{
							this.rankDefect.ScaleXYZ = DefectType.FIXED;
						}
						else if (this.rankDefect.ScaleXYZ != DefectType.FIXED)
						{
							this.rankDefect.ScaleXYZ = DefectType.FREE;
						}
					}
				}
				else if (observation is Direction)
				{
					if (!((Direction)observation).getOrientation().Enable)
					{
						this.rankDefect.RotationZ = DefectType.FIXED;
					}
					else if (this.rankDefect.RotationZ != DefectType.FIXED)
					{
						this.rankDefect.RotationZ = DefectType.FREE;
					}
				}
				else if (observation is HorizontalDistance)
				{
					if (this.rankDefect.RotationZ != DefectType.FIXED)
					{
						this.rankDefect.RotationZ = DefectType.FREE;
					}

					if (!is3DNet)
					{
						if (!((HorizontalDistance)observation).getScale().Enable)
						{
							this.rankDefect.ScaleXY = DefectType.FIXED;
						}
						else if (this.rankDefect.ScaleXY != DefectType.FIXED)
						{
							this.rankDefect.ScaleXY = DefectType.FREE;
						}
					}
					else
					{
						if (!((HorizontalDistance)observation).getScale().Enable)
						{
							this.rankDefect.ScaleXYZ = DefectType.FIXED;
						}
						else if (this.rankDefect.ScaleXYZ != DefectType.FIXED)
						{
							this.rankDefect.ScaleXYZ = DefectType.FREE;
						}
					}
				}
				else if (observation is SlopeDistance)
				{
					if (this.rankDefect.RotationX != DefectType.FIXED)
					{
						this.rankDefect.RotationX = DefectType.FREE;
					}
					if (this.rankDefect.RotationY != DefectType.FIXED)
					{
						this.rankDefect.RotationY = DefectType.FREE;
					}
					if (this.rankDefect.RotationZ != DefectType.FIXED)
					{
						this.rankDefect.RotationZ = DefectType.FREE;
					}

					if (!((SlopeDistance)observation).getScale().Enable)
					{
						this.rankDefect.ScaleXYZ = DefectType.FIXED;
					}
					else if (this.rankDefect.ScaleXYZ != DefectType.FIXED)
					{
						this.rankDefect.ScaleXYZ = DefectType.FREE;
					}

				}
				else if (observation is ZenithAngle)
				{
					if (this.rankDefect.RotationZ != DefectType.FIXED)
					{
						this.rankDefect.RotationZ = DefectType.FREE;
					}

					// muss dim == 3 sein, darf keine Lotabweichungen haben oder aber besitzt Lotabweichungen die gleichzeitig auch Beobachtungen sind
	//				zenithangleStationsWithKnownDeflections += (observation.getStartPoint().getDimension() == 3 && 
	//						(!observation.getStartPoint().hasUnknownDeflectionParameters() || 
	//								observation.getStartPoint().hasUnknownDeflectionParameters() && observation.getStartPoint().hasObservedDeflectionParameters())) ? 1 : 0;
					Pair<string, string> stationAndTargetNames = new Pair<string, string>(observation.StartPoint.Name, observation.EndPoint.Name);
					if (observation.StartPoint.Dimension == 3 && !zenithangleStationNamesWithKnownDeflections.Contains(stationAndTargetNames) && (!observation.StartPoint.hasUnknownDeflectionParameters() || observation.StartPoint.hasUnknownDeflectionParameters() && observation.StartPoint.hasObservedDeflectionParameters()))
					{
						zenithangleStationsWithKnownDeflections++;
						zenithangleStationNamesWithKnownDeflections.Add(stationAndTargetNames);
					}
				}
				else if (observation is GNSSBaseline1D)
				{
					if (((GNSSBaseline1D)observation).Dimension >= 1)
					{
						double gnssZ = ((GNSSBaseline1D)observation).getBaselineComponent(ComponentType.Z).ValueApriori;
						gnss1DCount++;
						if (gnss1DCount > 2)
						{
							if (gnssZ != 0)
							{
								if (!is3DNet)
								{
									if (!((GNSSBaseline1D)observation).Scale.isEnable())
									{
										this.rankDefect.ScaleZ = DefectType.FIXED;
									}
									else if (this.rankDefect.ScaleZ != DefectType.FIXED)
									{
										this.rankDefect.ScaleZ = DefectType.FREE;
									}
								}
								else
								{
									if (!((GNSSBaseline1D)observation).Scale.isEnable())
									{
										this.rankDefect.ScaleXYZ = DefectType.FIXED;
									}
									else if (this.rankDefect.ScaleXYZ != DefectType.FIXED)
									{
										this.rankDefect.ScaleXYZ = DefectType.FREE;
									}
								}
							}

							if (!((GNSSBaseline1D)observation).RotationX.isEnable())
							{
								this.rankDefect.RotationX = DefectType.FIXED;
							}
							else if (this.rankDefect.RotationX != DefectType.FIXED)
							{
								this.rankDefect.RotationX = DefectType.FREE;
							}

							if (!((GNSSBaseline1D)observation).RotationY.isEnable())
							{
								this.rankDefect.RotationY = DefectType.FIXED;
							}
							else if (this.rankDefect.RotationY != DefectType.FIXED)
							{
								this.rankDefect.RotationY = DefectType.FREE;
							}
						}
					}
				}
				else if (observation is GNSSBaseline2D)
				{
					if (((GNSSBaseline2D)observation).Dimension >= 2)
					{ // Ist vollstaendige Baseline X und Y vorhanden
						double gnssX = ((GNSSBaseline2D)observation).getBaselineComponent(ComponentType.X).ValueApriori;
						double gnssY = ((GNSSBaseline2D)observation).getBaselineComponent(ComponentType.Y).ValueApriori;
						if (gnssX != 0 || gnssY != 0)
						{
							gnss2DCount++;
							if (gnss2DCount > 1)
							{
								if (!((GNSSBaseline2D)observation).RotationZ.isEnable())
								{
									this.rankDefect.RotationZ = DefectType.FIXED;
								}
								else if (this.rankDefect.RotationZ != DefectType.FIXED)
								{
									this.rankDefect.RotationZ = DefectType.FREE;
								}

								if (!is3DNet)
								{
									if (!((GNSSBaseline2D)observation).Scale.isEnable())
									{
										this.rankDefect.ScaleXY = DefectType.FIXED;
									}
									else if (this.rankDefect.ScaleXY != DefectType.FIXED)
									{
										this.rankDefect.ScaleXY = DefectType.FREE;
									}
								}
								else
								{
									if (!((GNSSBaseline2D)observation).Scale.isEnable())
									{
										this.rankDefect.ScaleXYZ = DefectType.FIXED;
									}
									else if (this.rankDefect.ScaleXYZ != DefectType.FIXED)
									{
										this.rankDefect.ScaleXYZ = DefectType.FREE;
									}
								}
							}
						}
					}
				}
				else if (observation is GNSSBaseline3D)
				{
					if (((GNSSBaseline3D)observation).Dimension >= 3)
					{ // Ist vollstaendige Baseline X, Y und Z vorhanden
						double gnssX = ((GNSSBaseline3D)observation).getBaselineComponent(ComponentType.X).ValueApriori;
						double gnssY = ((GNSSBaseline3D)observation).getBaselineComponent(ComponentType.Y).ValueApriori;
						double gnssZ = ((GNSSBaseline3D)observation).getBaselineComponent(ComponentType.Z).ValueApriori;
						if (gnssX != 0 || gnssY != 0 || gnssZ != 0)
						{
							gnss3DCount++;
							if (gnss3DCount > 3)
							{
								if (!((GNSSBaseline3D)observation).Scale.isEnable())
								{
									this.rankDefect.ScaleXYZ = DefectType.FIXED;
								}
								else if (this.rankDefect.ScaleXYZ != DefectType.FIXED)
								{
									this.rankDefect.ScaleXYZ = DefectType.FREE;
								}

								if (gnssY != 0 || gnssZ != 0)
								{
									if (!((GNSSBaseline3D)observation).RotationX.isEnable())
									{
										this.rankDefect.RotationX = DefectType.FIXED;
									}
									else if (this.rankDefect.RotationX != DefectType.FIXED)
									{
										this.rankDefect.RotationX = DefectType.FREE;
									}
								}

								if (gnssX != 0 || gnssZ != 0)
								{
									if (!((GNSSBaseline3D)observation).RotationY.isEnable())
									{
										this.rankDefect.RotationY = DefectType.FIXED;
									}
									else if (this.rankDefect.RotationY != DefectType.FIXED)
									{
										this.rankDefect.RotationY = DefectType.FREE;
									}
								}

								if (gnssX != 0 || gnssY != 0)
								{
									if (!((GNSSBaseline3D)observation).RotationZ.isEnable())
									{
										this.rankDefect.RotationZ = DefectType.FIXED;
									}
									else if (this.rankDefect.RotationZ != DefectType.FIXED)
									{
										this.rankDefect.RotationZ = DefectType.FREE;
									}
								}
							}
						}
					}
				}
			}

			if (deltaH2PointsWithKnownDeflections > 1)
			{
				this.rankDefect.RotationX = DefectType.FIXED;
				this.rankDefect.RotationY = DefectType.FIXED;
			}

			if (zenithangleStationsWithKnownDeflections > 1)
			{
				this.rankDefect.RotationX = DefectType.FIXED;
				this.rankDefect.RotationY = DefectType.FIXED;
			}

			if (!is1DNet && deltaH2PointsWithKnownDeflections == 1)
			{
				if (this.rankDefect.RotationX == DefectType.FREE)
				{
					this.rankDefect.RotationX = DefectType.FIXED;
				}
				else if (this.rankDefect.RotationY == DefectType.FREE)
				{
					this.rankDefect.RotationY = DefectType.FIXED;
				}
			}

			if (zenithangleStationsWithKnownDeflections == 1)
			{
				if (this.rankDefect.RotationX == DefectType.FREE)
				{
					this.rankDefect.RotationX = DefectType.FIXED;
				}
				else if (this.rankDefect.RotationY == DefectType.FREE)
				{
					this.rankDefect.RotationY = DefectType.FIXED;
				}
			}

			// In einem reinen Richtungs- und/oder Zenitwinkelnetz gibts 
			// keine Strecken, die den Massstab festhalten. Er muss demnach 
			// als Bedingung in die freie Ausgleichung eingefuehrt werden
			if (is2DNet && this.rankDefect.ScaleXY == DefectType.NOT_SET)
			{
				this.rankDefect.ScaleXY = DefectType.FREE;
			}
			if (is3DNet && this.rankDefect.ScaleXYZ == DefectType.NOT_SET)
			{
				this.rankDefect.ScaleXYZ = DefectType.FREE;
			}

			if (this.proofOfDatumDefectDetection && eigenValueDefectCounter != -1 && eigenValueDefectCounter != this.rankDefect.Defect)
			{
				Console.Error.WriteLine("Error, the defect of the normal equations system is not " + "equal to its theoretical value: " + eigenValueDefectCounter + " vs. " + this.rankDefect.Defect);
			}

			return this.rankDefect;
		}

		public virtual RankDefect RankDefect
		{
			get
			{
				return this.rankDefect;
			}
		}

		/// <summary>
		/// Liefert die Verbesserungen der AGL v = A*dX - dl </summary>
		/// <param name="dx"> </param>
		/// <returns> v </returns>
		private Vector getCorrectionVector(Vector dx)
		{
			Vector v = new DenseVector(this.numberOfStochasticPointRows + this.numberOfObservations + this.numberOfStochasticDeflectionRows);

			for (int i = 0; i < this.numberOfObservations; i++)
			{
				Observation observation = this.projectObservations.get(i);
				ObservationGroup observationGroup = observation.ObservationGroup;
				Point startPoint = observation.StartPoint;
				Point endPoint = observation.StartPoint;

				int row = observation.RowInJacobiMatrix;

				int colInJacobiStart = startPoint.ColInJacobiMatrix;
				int colInJacobiEnd = endPoint.ColInJacobiMatrix;

				int dimStart = startPoint.Dimension;
				int dimEnd = endPoint.Dimension;

				double aDx = 0.0;
				// Bestimme A*dx fuer eine Beobachtung -> Ableitung nach den Punkte
				if (colInJacobiStart >= 0)
				{
					int dim = 0;
					if (dimStart != 1)
					{
						aDx += dx.get(colInJacobiStart + dim) * observation.diffXs();
						dim++;
						aDx += dx.get(colInJacobiStart + dim) * observation.diffYs();
						dim++;
					}
					if (dimStart != 2)
					{
						aDx += dx.get(colInJacobiStart + dim) * observation.diffZs();
					}
				}
				if (colInJacobiEnd >= 0)
				{
					int dim = 0;
					if (dimEnd != 1)
					{
						aDx += dx.get(colInJacobiEnd + dim) * observation.diffXe();
						dim++;
						aDx += dx.get(colInJacobiEnd + dim) * observation.diffYe();
						dim++;
					}
					if (dimEnd != 2)
					{
						aDx += dx.get(colInJacobiEnd + dim) * observation.diffZe();
					}
				}
				// Lotabweichung des Standpunktes
				if (startPoint.hasUnknownDeflectionParameters())
				{
					VerticalDeflection deflX = startPoint.VerticalDeflectionX;
					VerticalDeflection deflY = startPoint.VerticalDeflectionY;

					aDx += dx.get(deflX.ColInJacobiMatrix) * observation.diffVerticalDeflectionXs();
					aDx += dx.get(deflY.ColInJacobiMatrix) * observation.diffVerticalDeflectionYs();
				}
				// Lotabweichung des Zielpunktes
				if (endPoint.hasUnknownDeflectionParameters())
				{
					VerticalDeflection deflX = endPoint.VerticalDeflectionX;
					VerticalDeflection deflY = endPoint.VerticalDeflectionY;

					aDx += dx.get(deflX.ColInJacobiMatrix) * observation.diffVerticalDeflectionXe();
					aDx += dx.get(deflY.ColInJacobiMatrix) * observation.diffVerticalDeflectionYe();
				}
				// Zusatzparameter
				if (observationGroup.numberOfAdditionalUnknownParameter() > 0)
				{

					if (observation is DeltaZ)
					{
						AdditionalUnknownParameter addPar = ((DeltaZ)observation).getScale();
						if (addPar.Enable)
						{
							aDx += dx.get(addPar.ColInJacobiMatrix) * observation.diffScale();
						}
					}
					else if (observation is Direction)
					{
						AdditionalUnknownParameter addPar = ((Direction)observation).getOrientation();
						if (addPar.Enable)
						{
							aDx += dx.get(addPar.ColInJacobiMatrix) * observation.diffOri();
						}
					}
					else if (observation is HorizontalDistance)
					{
						AdditionalUnknownParameter addPar = ((HorizontalDistance)observation).getScale();
						if (addPar.Enable)
						{
							aDx += dx.get(addPar.ColInJacobiMatrix) * observation.diffScale();
						}
						addPar = ((HorizontalDistance)observation).getZeroPointOffset();
						if (addPar.Enable)
						{
							aDx += dx.get(addPar.ColInJacobiMatrix) * observation.diffAdd();
						}
					}
					else if (observation is SlopeDistance)
					{
						AdditionalUnknownParameter addPar = ((SlopeDistance)observation).getScale();
						if (addPar.Enable)
						{
							aDx += dx.get(addPar.ColInJacobiMatrix) * observation.diffScale();
						}
						addPar = ((SlopeDistance)observation).getZeroPointOffset();
						if (addPar.Enable)
						{
							aDx += dx.get(addPar.ColInJacobiMatrix) * observation.diffAdd();
						}
					}
					else if (observation is ZenithAngle)
					{
						AdditionalUnknownParameter addPar = ((ZenithAngle)observation).getRefractionCoefficient();
						if (addPar.Enable)
						{
							aDx += dx.get(addPar.ColInJacobiMatrix) * observation.diffRefCoeff();
						}
					}

				}
				v.set(row, aDx - observation.Correction);
			}

			foreach (Point point in this.pointsWithStochasticDeflection)
			{
				int col = point.VerticalDeflectionX.ColInJacobiMatrix;
				int row = point.VerticalDeflectionX.RowInJacobiMatrix;
				v.set(row, dx.get(col));

				col = point.VerticalDeflectionY.ColInJacobiMatrix;
				row = point.VerticalDeflectionY.RowInJacobiMatrix;
				v.set(row, dx.get(col));
			}

			foreach (Point point in this.stochasticPoints)
			{
				int row = point.RowInJacobiMatrix;
				int col = point.ColInJacobiMatrix;
				int dim = point.Dimension;
				if (row >= 0)
				{
					for (int j = 0; j < dim; j++)
					{
						v.set(row + j, dx.get(col + j));
					}
				}
			}

			return v;
		}

		/// <summary>
		/// Liefert die Teststatitikparameter fuer den Signifikanztest (Modellstoerungen) </summary>
		/// <returns> significanceTestStatisticParameters </returns>
		public virtual TestStatisticParameters SignificanceTestStatisticParameters
		{
			get
			{
				if (this.significanceTestStatisticParameters != null)
				{
					return this.significanceTestStatisticParameters;
				}
    
				return this.significanceTestStatisticParameters = this.getTestStatisticParameters(this.significanceTestStatisticDefinition);
			}
		}

		/// <summary>
		/// Liefert die Teststatitikparameter </summary>
		/// <returns> testStatiticParams </returns>
		internal virtual TestStatisticParameters getTestStatisticParameters(TestStatisticDefinition testStatisticDefinition)
		{
			double alpha = testStatisticDefinition.ProbabilityValue;
			double beta = testStatisticDefinition.PowerOfTest;
			int dof = this.degreeOfFreedom();

			TestStatistic testStatistic;
			switch (testStatisticDefinition.TestStatisticType.innerEnumValue)
			{
			case TestStatisticType.InnerEnum.SIDAK:
				// alle Hypothesen + Test der Varianzkomponenten + Test der Festpunkte
				testStatistic = new SidakTestStatistic(this.numberOfHypotesis, alpha, beta, testStatisticDefinition.FamilywiseErrorRate);
				break;
			case TestStatisticType.InnerEnum.BAARDA_METHOD:
				testStatistic = new BaardaMethodTestStatistic(testStatisticDefinition.FamilywiseErrorRate ? dof : 1, alpha, beta);
				break;
			case TestStatisticType.InnerEnum.NONE:
				testStatistic = new UnadjustedTestStatitic(alpha, beta);
				break;
			default:
				throw new System.ArgumentException(this.GetType().Name + " Error, unknown test statistic method " + testStatisticDefinition.TestStatisticType);
			}
			return new TestStatisticParameters(testStatistic);
		}

		/// <summary>
		/// Liefert den aktuellen Status </summary>
		/// <returns> state </returns>
		public virtual EstimationStateType EstimationStateType
		{
			get
			{
				return this.currentEstimationStatus;
			}
		}

		/// <summary>
		/// Liefert <code>true</code>, wenn das Modell berechenbar war </summary>
		/// <returns> isEstimateModel </returns>
		public virtual bool EstimateModel
		{
			get
			{
				return this.currentEstimationStatus == EstimationStateType.ERROR_FREE_ESTIMATION;
			}
		}

		/// <summary>
		/// Liefert den aktuellen Iterationsschritt </summary>
		/// <returns> iterationStep </returns>
		public virtual int CurrentIterationStep
		{
			get
			{
				return this.iterationStep < 0?0:this.iterationStep;
			}
		}

		/// <summary>
		/// Liefert den Freiheitsgrad nach der Ausgleichung aus der Redundanzmatrix
		/// <code>f = n-u+d = spur(R) </summary>
		/// <returns> f </returns>
		public virtual int degreeOfFreedom()
		{
			return (int)Math.Round(this.degreeOfFreedom_Conflict);
		}

		/// <summary>
		/// Liefert die Quadratsumme der Verbesserungen &Omega; = vTPv </summary>
		/// <returns> &Omega; </returns>
		public virtual double Omega
		{
			get
			{
				return this.omega;
			}
		}

		public override void run()
		{
			this.estimateModel();
		}

		public virtual bool calculateStochasticParameters()
		{
			return this.calculateStochasticParameters_Conflict;
		}

		/// <summary>
		/// Setzt die Teststatistik fuer Signifikanztests
		/// </summary>
		public virtual TestStatisticDefinition SignificanceTestStatisticDefinition
		{
			set
			{
				this.significanceTestStatisticDefinition = value;
			}
		}

		/// <summary>
		/// Liefert den Grenzwert, mit dem der Konvergenzfortschritt bemessen wird </summary>
		/// <returns> EPS </returns>
		public virtual double ConvergenceThreshold
		{
			get
			{
				return SQRT_EPS;
			}
		}

		/// <summary>
		/// Liefert die max. Anzahl an Iterationen, die durchgefuehrt werden </summary>
		/// <returns> maxIter </returns>
		public virtual int MaximalNumberOfIterations
		{
			get
			{
				return this.maximalNumberOfIterations;
			}
			set
			{
				if (value < 0 || value > DefaultValue.MaximalNumberOfIterations)
				{
					this.maximalNumberOfIterations = DefaultValue.MaximalNumberOfIterations;
				}
				else
				{
					this.maximalNumberOfIterations = value;
				}
			}
		}

		/// <summary>
		/// Liefert das Schaetzverfahren </summary>
		/// <returns> estimationType </returns>
		public virtual EstimationType EstimationType
		{
			get
			{
				return this.estimationType;
			}
			set
			{
				this.estimationType = value;
			}
		}



		/// <summary>
		/// Beruecksichtigung des geschaetzten Varianzfaktors zur Skallierung der Kovarianzmatrix </summary>
		/// <param name="applyAposterioriVarianceOfUnitWeight"> </param>
		public virtual bool ApplyAposterioriVarianceOfUnitWeight
		{
			set
			{
				this.applyAposterioriVarianceOfUnitWeight = value;
			}
		}

		/// <summary>
		/// Deformationsanalyse durchfuehren </summary>
		/// <param name="congruenceAnalysis"> </param>
		public virtual bool CongruenceAnalysis
		{
			set
			{
				this.congruenceAnalysis = value;
			}
		}

		/// <summary>
		/// Maximale Abweichung der AGL-Probe </summary>
		/// <returns> finalLinearisationError </returns>
		public virtual double FinalLinearisationError
		{
			get
			{
				return this.finalLinearisationError;
			}
		}

		/// <summary>
		/// Legt die Intervallsgrenze fuer robuste Ausgleichung fest </summary>
		/// <param name="robustEstimationLimit"> </param>
		public virtual double RobustEstimationLimit
		{
			set
			{
				this.robustEstimationLimit = value > 0 ? value : DefaultValue.RobustEstimationLimit;
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




		/// <summary>
		/// Liefert die Jacobimatrix der Ausgleichung </summary>
		/// <returns> A </returns>
		/// @deprecated - Nur fuer DEBUG-Ausgabe 
		internal virtual Matrix JacobiMatrix
		{
			get
			{
				Matrix A = new DenseMatrix(this.numberOfObservations + this.numberOfStochasticPointRows + this.numberOfStochasticDeflectionRows, this.numberOfUnknownParameters);
				for (int u = 0; u < this.unknownParameters.size(); u++)
				{
					UnknownParameter unknownParameterA = this.unknownParameters.get(u);
					ObservationGroup observaionGroupA = unknownParameterA.Observations;
					int colA = unknownParameterA.ColInJacobiMatrix;
    
					int dimA = 1;
					switch (unknownParameterA.ParameterType.innerEnumValue)
					{
					case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT2D:
						dimA = 2;
						break;
					case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.POINT3D:
						dimA = 3;
						break;
					default:
						dimA = 1;
						break;
					}
    
					for (int i = 0; i < dimA; i++)
					{
						//colA += i;
						colA = unknownParameterA.ColInJacobiMatrix + i;
    
						for (int j = 0; j < observaionGroupA.size(); j++)
						{
							Observation observationA = observaionGroupA.get(j);
							double a = 0.0;
							int rowA = observationA.RowInJacobiMatrix;
    
							if (unknownParameterA.ParameterType == ParameterType.POINT1D)
							{
								Point p = (Point)unknownParameterA;
								if (p.Equals(observationA.StartPoint))
								{
									a = observationA.diffZs();
								}
								else if (p.Equals(observationA.EndPoint))
								{
									a = observationA.diffZe();
								}
							}
							else if (unknownParameterA.ParameterType == ParameterType.POINT2D)
							{
								Point p = (Point)unknownParameterA;
								if (p.Equals(observationA.StartPoint))
								{
									if (i == 0)
									{
										a = observationA.diffXs();
									}
									else if (i == 1)
									{
										a = observationA.diffYs();
									}
								}
								else if (p.Equals(observationA.EndPoint))
								{
									if (i == 0)
									{
										a = observationA.diffXe();
									}
									else if (i == 1)
									{
										a = observationA.diffYe();
									}
								}
							}
							else if (unknownParameterA.ParameterType == ParameterType.POINT3D)
							{
								Point p = (Point)unknownParameterA;
								if (p.Equals(observationA.StartPoint))
								{
									if (i == 0)
									{
										a = observationA.diffXs();
									}
									else if (i == 1)
									{
										a = observationA.diffYs();
									}
									else if (i == 2)
									{
										a = observationA.diffZs();
									}
								}
								else if (p.Equals(observationA.EndPoint))
								{
									if (i == 0)
									{
										a = observationA.diffXe();
									}
									else if (i == 1)
									{
										a = observationA.diffYe();
									}
									else if (i == 2)
									{
										a = observationA.diffZe();
									}
								}
							}
							else if (unknownParameterA.ParameterType == ParameterType.VERTICAL_DEFLECTION_X)
							{
								VerticalDeflectionX deflection = (VerticalDeflectionX)unknownParameterA;
								Point p = deflection.Point;
								if (p.Equals(observationA.StartPoint))
								{
									a = observationA.diffVerticalDeflectionXs();
								}
								else if (p.Equals(observationA.EndPoint))
								{
									a = observationA.diffVerticalDeflectionXe();
								}
							}
							else if (unknownParameterA.ParameterType == ParameterType.VERTICAL_DEFLECTION_Y)
							{
								VerticalDeflectionY deflection = (VerticalDeflectionY)unknownParameterA;
								Point p = deflection.Point;
								if (p.Equals(observationA.StartPoint))
								{
									a = observationA.diffVerticalDeflectionYs();
								}
								else if (p.Equals(observationA.EndPoint))
								{
									a = observationA.diffVerticalDeflectionYe();
								}
							}
							else if (unknownParameterA.ParameterType == ParameterType.ORIENTATION)
							{
								a = observationA.diffOri();
							}
							else if (unknownParameterA.ParameterType == ParameterType.ZERO_POINT_OFFSET)
							{
								a = observationA.diffAdd();
							}
							else if (unknownParameterA.ParameterType == ParameterType.SCALE)
							{
								a = observationA.diffScale();
							}
							else if (unknownParameterA.ParameterType == ParameterType.REFRACTION_INDEX)
							{
								a = observationA.diffRefCoeff();
							}
							else if (unknownParameterA.ParameterType == ParameterType.ROTATION_X)
							{
								a = observationA.diffRotX();
							}
							else if (unknownParameterA.ParameterType == ParameterType.ROTATION_Y)
							{
								a = observationA.diffRotY();
							}
							else if (unknownParameterA.ParameterType == ParameterType.ROTATION_Z)
							{
								a = observationA.diffRotZ();
							}
							A.set(rowA, colA, a);
						}
					}
				}
    
				// Stochastische Punkte hinzufuegen
				foreach (Point point in this.pointsWithStochasticDeflection)
				{
					VerticalDeflection deflectionX = point.VerticalDeflectionX;
					VerticalDeflection deflectionY = point.VerticalDeflectionY;
    
					A.set(deflectionX.RowInJacobiMatrix, deflectionX.ColInJacobiMatrix, 1.0);
					A.set(deflectionY.RowInJacobiMatrix, deflectionY.ColInJacobiMatrix, 1.0);
				}
    
				// Stochastische Punkte hinzufuegen
				foreach (Point point in this.stochasticPoints)
				{
					int col = point.ColInJacobiMatrix;
					int row = point.RowInJacobiMatrix;
    
					if (point.Dimension != 1)
					{
						A.set(row++, col++, 1.0);
						A.set(row++, col++, 1.0);
					}
					if (point.Dimension != 2)
					{
						A.set(row, col, 1.0);
					}
				}
				return A;
			}
		}

		/// <summary>
		/// Liefert die Hauptdiagonale der a-priori Gewichtsmatrix </summary>
		/// <returns> W </returns>
		/// @deprecated - Nur fuer DEBUG-Ausgabe 
		internal virtual Vector WeightedMatrix
		{
			get
			{
				Vector W = new DenseVector(this.numberOfObservations + this.numberOfStochasticPointRows + this.numberOfStochasticDeflectionRows);
				for (int i = 0; i < this.numberOfObservations; i++)
				{
					Observation observation = this.projectObservations.get(i);
					W.set(observation.RowInJacobiMatrix, 1.0 / observation.StdApriori / observation.StdApriori);
				}
    
				foreach (Point point in this.pointsWithStochasticDeflection)
				{
					VerticalDeflection deflectionX = point.VerticalDeflectionX;
					VerticalDeflection deflectionY = point.VerticalDeflectionY;
    
					W.set(deflectionX.RowInJacobiMatrix, 1.0 / deflectionX.StdApriori / deflectionX.StdApriori);
					W.set(deflectionY.RowInJacobiMatrix, 1.0 / deflectionY.StdApriori / deflectionY.StdApriori);
				}
    
				foreach (Point point in this.stochasticPoints)
				{
					int dim = point.Dimension;
					int row = point.RowInJacobiMatrix;
    
					if (dim != 1)
					{
						W.set(row++, 1.0 / point.StdXApriori / point.StdXApriori);
						W.set(row++, 1.0 / point.StdYApriori / point.StdYApriori);
					}
					if (dim != 2)
					{
						W.set(row++, 1.0 / point.StdZApriori / point.StdZApriori);
					}
				}
				return W;
			}
		}

		/// <summary>
		/// Liefert die Verbesserungen nach der Ausgleichung IST - SOLL </summary>
		/// <returns> e </returns>
		private Vector Residuals
		{
			get
			{
				Vector e = new DenseVector(this.numberOfObservations + this.numberOfStochasticPointRows + this.numberOfStochasticDeflectionRows);
				for (int i = 0; i < this.numberOfObservations; i++)
				{
					Observation observation = this.projectObservations.get(i);
					e.set(observation.RowInJacobiMatrix, observation.Correction);
				}
    
				foreach (Point point in this.pointsWithStochasticDeflection)
				{
					VerticalDeflection deflectionX = point.VerticalDeflectionX;
					VerticalDeflection deflectionY = point.VerticalDeflectionY;
    
					if (deflectionX.RowInJacobiMatrix < 0 || deflectionY.RowInJacobiMatrix < 0)
					{
						continue;
					}
    
					e.set(deflectionX.RowInJacobiMatrix, deflectionX.Value0 - deflectionX.Value);
					e.set(deflectionY.RowInJacobiMatrix, deflectionY.Value0 - deflectionY.Value);
				}
    
				foreach (Point point in this.stochasticPoints)
				{
					int dim = point.Dimension;
					int row = point.RowInJacobiMatrix;
    
					if (dim != 1)
					{
						e.set(row++, point.X0 - point.X);
						e.set(row++, point.Y0 - point.Y);
					}
					if (dim != 2)
					{
						e.set(row++, point.Z0 - point.Z);
					}
				}
				return e;
			}
		}

		/// <summary>
		/// Liefert den Varianzfaktor nach der Ausgleichung
		/// Ist der Freiheitsgrad oder die Verbesserungsquadratsumme 
		/// gleich Null, so wird 1 (Varianzfaktor a-priori) 
		/// zurueckgegeben
		/// Bei einer Praeanalyse wird immer 1 ausgegeben </summary>
		/// <returns> sigma2apost </returns>
		public virtual double VarianceFactorAposteriori
		{
			get
			{
				return this.degreeOfFreedom_Conflict > 0 && this.omega > 0 && this.estimationType != EstimationType.SIMULATION && this.applyAposterioriVarianceOfUnitWeight ? Math.Abs(this.omega / this.degreeOfFreedom_Conflict) : 1.0;
			}
		}

		/// <summary>
		/// Liefert die Varianzanteile der einzelnen Beobachtungstypen </summary>
		/// <returns> vks </returns>
		public virtual IDictionary<VarianceComponentType, VarianceComponent> VarianceComponents
		{
			get
			{
				return this.varianceComponents;
			}
		}

		/// <summary>
		/// Liefert die mittlere Redundanz (Bedingungsdichte) im Netz r<sub>m</sub> = 1-u/(n+d) </summary>
		/// <returns> r<sub>m</sub> </returns>
		public virtual double MeanRedundancy
		{
			get
			{
				int nd = this.degreeOfFreedom() + this.numberOfUnknownParameters;
				return nd > 0?1.0 - (double)this.numberOfUnknownParameters / nd:0.0;
			}
		}

		/// <summary>
		/// Liefert die trace(Qxx) fuer den Punkt-bezogenen Anteil aus Qxx </summary>
		/// <returns> trace(Qxx) </returns>
		public virtual double TraceOfCovarianceMatrixOfPoints
		{
			get
			{
				return this.traceCxxPoints;
			}
		}

		/// <summary>
		/// Setzt die Anzahl der zubestimmenden Hauptkomponenten </summary>
		/// <param name="numberOfPrincipalComponents"> </param>
		public virtual int NumberOfPrincipalComponents
		{
			set
			{
				this.numberOfPrincipalComponents = value < 0 ? 0 : value;
			}
		}

		/// <summary>
		/// Liefert die ersten n. Hauptkomponente der Hauptkomponentenanalyse (maximale Eigenwerte von Cxx) </summary>
		/// <returns> fpc </returns>
		public virtual PrincipalComponent[] PrincipalComponents
		{
			get
			{
				return this.principalComponents;
			}
		}

		/// <summary>
		/// Fuehrt eine Hauptkomponentenanalyse durch und bestimmt die k Hauptkomponente. Hierbei wird Qxx ueberschrieben.
		/// Um Fehler zu vermeiden, wird Qxx = null am Ende gesetzt. </summary>
		/// <param name="numberOfComponents"> Anzahl der zu bestimmenden Komponenten </param>
		private void estimatePrincipalComponentAnalysis(int numberOfComponents)
		{
			try
			{
				this.currentEstimationStatus = EstimationStateType.PRINCIPAL_COMPONENT_ANALYSIS;
				this.change.firePropertyChange(this.currentEstimationStatus.ToString(), false, true);
				double sigma2apost = this.VarianceFactorAposteriori;
				int n = this.unknownParameters.columnsOfPoints();

				if (this.Qxx != null && n > 0 && numberOfComponents > 0)
				{
					// Der Index ist Eins-Index-basierend, d.h., der kleinste Eigenwert hat den Index Eins und der groesste ist am Index n!
					Matrix[] evalEvec = MathExtension.eig(this.Qxx, n, Math.Max(n - numberOfComponents + 1, 1), n, true);
					Matrix eval = (UpperSymmBandMatrix)evalEvec[0];
					Matrix evec = (DenseMatrix)evalEvec[1];
					// Anzahl der tatsaechlich bestimmten Komponenten
					numberOfComponents = eval.numColumns();
					double sqrtFPC = 0.0;
					this.principalComponents = new PrincipalComponent[numberOfComponents];
					for (int i = 0; i < numberOfComponents; i++)
					{
						PrincipalComponent principalComponent = new PrincipalComponent(n - i, Math.Abs(sigma2apost * eval.get(numberOfComponents - 1 - i, numberOfComponents - 1 - i)));
						this.principalComponents[i] = principalComponent;
						if (i == 0)
						{
							sqrtFPC = Math.Sqrt(principalComponent.Value);
						}
					}

					for (int i = 0; i < this.unknownParameters.size(); i++)
					{
						if (this.interrupt_Conflict)
						{
							return;
						}

						UnknownParameter unknownParameter = this.unknownParameters.get(i);
						int row = unknownParameter.ColInJacobiMatrix;
						int col = numberOfComponents - 1;
						if (unknownParameter is Point && row >= 0)
						{
							Point point = (Point)unknownParameter;
							int dim = point.Dimension;
							double[] principalComponents = new double[dim];
							if (dim != 1)
							{
								principalComponents[0] = sqrtFPC * evec.get(row++, col);
								principalComponents[1] = sqrtFPC * evec.get(row++, col);
							}
							if (dim != 2)
							{
								principalComponents[dim - 1] = sqrtFPC * evec.get(row++, col);
							}
							point.FirstPrincipalComponents = principalComponents;
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				this.Qxx = null;
			}
		}

		private void applySphericalVerticalDeflections()
		{
			if (this.sphericalDeflectionModel != null)
			{
				foreach (Point point in this.allPoints.Values)
				{
					this.sphericalDeflectionModel.SphericalDeflections = point;
				}
			}
		}

		public virtual SphericalDeflectionModel SphericalDeflectionModel
		{
			set
			{
				this.sphericalDeflectionModel = value;
			}
		}

		public virtual bool hasCovarianceExportPathAndBaseName()
		{
			return !string.ReferenceEquals(this.coVarExportPathAndFileName, null) && this.coVarExportPathAndFileName.Length > 0;
		}

		public virtual string CovarianceExportPathAndBaseName
		{
			set
			{
				this.coVarExportPathAndFileName = value;
			}
		}

		private bool exportCovarianceMatrix()
		{
			if (string.ReferenceEquals(this.coVarExportPathAndFileName, null))
			{
				return true;
			}


			File coVarMatrixFile = new File(this.coVarExportPathAndFileName + ".cxx");
			File coVarInfoFile = new File(this.coVarExportPathAndFileName + ".info");

			return this.exportCovarianceMatrixInfoToFile(coVarInfoFile) && this.exportCovarianceMatrixToFile(coVarMatrixFile);
		}

		/// <summary>
		/// Schreibt punktbezogene Informationen zur CoVar raus </summary>
		/// <param name="f"> </param>
		/// <returns> isWritten </returns>
		private bool exportCovarianceMatrixInfoToFile(File f)
		{
			// noch keine Loesung vorhanden
			if (f == null || this.Qxx == null)
			{
				return false;
			}

			this.currentEstimationStatus = EstimationStateType.EXPORT_COVARIANCE_INFORMATION;
			this.change.firePropertyChange(this.currentEstimationStatus.ToString(), null, f.ToString());

			bool isComplete = false;
			PrintWriter pw = null;
			try
			{
				pw = new PrintWriter(new StreamWriter(f));
								//Pkt,Type(XYZ),Coord,Row in NGL, Num Obs
				string format = "%25s\t%5s\t%35.15f\t%10d\t%10d%n";
				foreach (Point point in this.allPoints.Values)
				{
					if (this.interrupt_Conflict)
					{
						break;
					}

					int colInJacobi = point.ColInJacobiMatrix;
					int rowInJacobi = point.RowInJacobiMatrix;
					int dim = point.Dimension;
					int numberOfObservations = point.numberOfObservations() + (rowInJacobi >= 0 ? dim : 0);
					if (colInJacobi < 0)
					{
						continue;
					}

					if (dim != 1)
					{
						pw.printf(Locale.ENGLISH, format, point.Name, 'X', point.X, colInJacobi++, numberOfObservations);
						pw.printf(Locale.ENGLISH, format, point.Name, 'Y', point.Y, colInJacobi++, numberOfObservations);
					}
					if (dim != 2)
					{
						pw.printf(Locale.ENGLISH, format, point.Name, 'Z', point.Z, colInJacobi++, numberOfObservations);
					}

				}
				isComplete = true;
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				if (pw != null)
				{
					pw.close();
				}
			}

			return isComplete;
		}

		/// <summary>
		/// Schreibt die CoVar raus </summary>
		/// <param name="f"> </param>
		/// <returns> isWritten </returns>
		private bool exportCovarianceMatrixToFile(File f)
		{
			// noch keine Loesung vorhanden
			if (f == null || this.Qxx == null || this.Qxx.numRows() < this.numberOfUnknownParameters)
			{
				return false;
			}

			this.currentEstimationStatus = EstimationStateType.EXPORT_COVARIANCE_MATRIX;
			this.change.firePropertyChange(this.currentEstimationStatus.ToString(), null, f.ToString());

			bool isComplete = false;
			bool isDEBUG = false;
			PrintWriter pw = null;
			double sigma2apost = this.VarianceFactorAposteriori;

			try
			{
				pw = new PrintWriter(new StreamWriter(f));

				for (int i = 0; i < this.numberOfUnknownParameters; i++)
				{
					if (this.interrupt_Conflict)
					{
						break;
					}

					for (int j = 0; j < this.numberOfUnknownParameters; j++)
					{
						if (this.interrupt_Conflict)
						{
							break;
						}

						pw.printf(Locale.ENGLISH, "%+35.25f  ", sigma2apost * this.Qxx.get(i,j));
					}
					pw.println();
				}

				if (isDEBUG)
				{
					pw.println("--------DEBUG - SIGMA2APOST ------------");
					pw.printf(Locale.ENGLISH, "%+35.15f%n", sigma2apost);

					pw.println("--------DEBUG - COFACTOR ------------");
					int size = this.Qxx.numRows();
					for (int i = 0; i < size; i++)
					{
						for (int j = 0; j < size; j++)
						{
							pw.printf(Locale.ENGLISH, "%+35.15f  ", this.Qxx.get(i,j));
						}
						pw.println();
					}

					Matrix A = this.JacobiMatrix;
					pw.println("--------DEBUG - JACOBI ------------");
					for (int i = 0; i < A.numRows(); i++)
					{
						for (int j = 0; j < A.numColumns(); j++)
						{
							pw.printf(Locale.ENGLISH, "%+35.15f  ", A.get(i,j));
						}
						pw.println();
					}

					pw.println("--------DEBUG - WEIGHTS ------------");
					Vector W = this.WeightedMatrix;
					for (int i = 0; i < W.size(); i++)
					{
						pw.printf(Locale.ENGLISH, "%+35.15f%n", W.get(i));
					}

					pw.println("--------DEBUG - RESIDUALS ------------");
					Vector e = this.Residuals;
					for (int i = 0; i < W.size(); i++)
					{
						pw.printf(Locale.ENGLISH, "%+35.15f%n", e.get(i));
					}
				}

				isComplete = true;
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				if (pw != null)
				{
					pw.close();
				}
			}
			return isComplete;
		}

		public virtual void addPropertyChangeListener(PropertyChangeListener listener)
		{
			this.change.addPropertyChangeListener(listener);
		}

		public virtual void removePropertyChangeListener(PropertyChangeListener listener)
		{
			this.change.removePropertyChangeListener(listener);
		}

		public virtual void clearMatrices()
		{
			this.Qxx = null;
			this.ATQxxBP_GNSS_EP = null;
			this.PAzTQzzAzP_GNSS_EF = null;
		}
	}

}