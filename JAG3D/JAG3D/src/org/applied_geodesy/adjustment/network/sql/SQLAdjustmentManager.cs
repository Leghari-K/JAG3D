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

namespace org.applied_geodesy.adjustment.network.sql
{

	using Constant = org.applied_geodesy.adjustment.Constant;
	using EstimationType = org.applied_geodesy.adjustment.EstimationType;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using DefaultAverageThreshold = org.applied_geodesy.adjustment.network.DefaultAverageThreshold;
	using DefaultUncertainty = org.applied_geodesy.adjustment.network.DefaultUncertainty;
	using DefectType = org.applied_geodesy.adjustment.network.DefectType;
	using Epoch = org.applied_geodesy.adjustment.network.Epoch;
	using NetworkAdjustment = org.applied_geodesy.adjustment.network.NetworkAdjustment;
	using ObservationGroupUncertaintyType = org.applied_geodesy.adjustment.network.ObservationGroupUncertaintyType;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using PointGroupUncertaintyType = org.applied_geodesy.adjustment.network.PointGroupUncertaintyType;
	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using PrincipalComponent = org.applied_geodesy.adjustment.network.PrincipalComponent;
	using RankDefect = org.applied_geodesy.adjustment.network.RankDefect;
	using VarianceComponent = org.applied_geodesy.adjustment.network.VarianceComponent;
	using VarianceComponentType = org.applied_geodesy.adjustment.network.VarianceComponentType;
	using VerticalDeflectionGroupUncertaintyType = org.applied_geodesy.adjustment.network.VerticalDeflectionGroupUncertaintyType;
	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using CongruenceAnalysisGroup = org.applied_geodesy.adjustment.network.congruence.CongruenceAnalysisGroup;
	using CongruenceAnalysisPointPair = org.applied_geodesy.adjustment.network.congruence.CongruenceAnalysisPointPair;
	using RestrictionType = org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType;
	using StrainAnalysisEquations = org.applied_geodesy.adjustment.network.congruence.strain.StrainAnalysisEquations;
	using StrainParameter = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameter;
	using ComponentType = org.applied_geodesy.adjustment.network.observation.ComponentType;
	using DeltaZ = org.applied_geodesy.adjustment.network.observation.DeltaZ;
	using Direction = org.applied_geodesy.adjustment.network.observation.Direction;
	using FaceType = org.applied_geodesy.adjustment.network.observation.FaceType;
	using GNSSBaseline = org.applied_geodesy.adjustment.network.observation.GNSSBaseline;
	using GNSSBaselineDeltaX2D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaX2D;
	using GNSSBaselineDeltaX3D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaX3D;
	using GNSSBaselineDeltaY2D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaY2D;
	using GNSSBaselineDeltaY3D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaY3D;
	using GNSSBaselineDeltaZ1D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaZ1D;
	using GNSSBaselineDeltaZ3D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaZ3D;
	using HorizontalDistance = org.applied_geodesy.adjustment.network.observation.HorizontalDistance;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using SlopeDistance = org.applied_geodesy.adjustment.network.observation.SlopeDistance;
	using ZenithAngle = org.applied_geodesy.adjustment.network.observation.ZenithAngle;
	using DeltaZGroup = org.applied_geodesy.adjustment.network.observation.group.DeltaZGroup;
	using DirectionGroup = org.applied_geodesy.adjustment.network.observation.group.DirectionGroup;
	using GNSSBaseline1DGroup = org.applied_geodesy.adjustment.network.observation.group.GNSSBaseline1DGroup;
	using GNSSBaseline2DGroup = org.applied_geodesy.adjustment.network.observation.group.GNSSBaseline2DGroup;
	using GNSSBaseline3DGroup = org.applied_geodesy.adjustment.network.observation.group.GNSSBaseline3DGroup;
	using HorizontalDistanceGroup = org.applied_geodesy.adjustment.network.observation.group.HorizontalDistanceGroup;
	using ObservationGroup = org.applied_geodesy.adjustment.network.observation.group.ObservationGroup;
	using SlopeDistanceGroup = org.applied_geodesy.adjustment.network.observation.group.SlopeDistanceGroup;
	using ZenithAngleGroup = org.applied_geodesy.adjustment.network.observation.group.ZenithAngleGroup;
	using ProjectionType = org.applied_geodesy.adjustment.network.observation.reduction.ProjectionType;
	using Reduction = org.applied_geodesy.adjustment.network.observation.reduction.Reduction;
	using ReductionTaskType = org.applied_geodesy.adjustment.network.observation.reduction.ReductionTaskType;
	using AdditionalUnknownParameter = org.applied_geodesy.adjustment.network.parameter.AdditionalUnknownParameter;
	using Orientation = org.applied_geodesy.adjustment.network.parameter.Orientation;
	using Point = org.applied_geodesy.adjustment.network.point.Point;
	using Point1D = org.applied_geodesy.adjustment.network.point.Point1D;
	using Point2D = org.applied_geodesy.adjustment.network.point.Point2D;
	using Point3D = org.applied_geodesy.adjustment.network.point.Point3D;
	using TestStatisticDefinition = org.applied_geodesy.adjustment.statistic.TestStatisticDefinition;
	using TestStatisticParameterSet = org.applied_geodesy.adjustment.statistic.TestStatisticParameterSet;
	using TestStatisticParameters = org.applied_geodesy.adjustment.statistic.TestStatisticParameters;
	using TestStatisticType = org.applied_geodesy.adjustment.statistic.TestStatisticType;
	using Ellipsoid = org.applied_geodesy.transformation.datum.Ellipsoid;
	using SphericalDeflectionModel = org.applied_geodesy.transformation.datum.SphericalDeflectionModel;
	using DataBase = org.applied_geodesy.util.sql.DataBase;
	using HSQLDB = org.applied_geodesy.util.sql.HSQLDB;
	using DatabaseVersionMismatchException = org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException;
	using Version = org.applied_geodesy.version.jag3d.Version;
	using VersionType = org.applied_geodesy.version.VersionType;

	public class SQLAdjustmentManager
	{
		private readonly DataBase dataBase;

		private IDictionary<string, Point> completePoints = new LinkedHashMap<string, Point>();
		private IDictionary<string, Point> completeNewPoints = new LinkedHashMap<string, Point>();

		private IDictionary<string, Point> completePointsWithReferenceDeflections = new LinkedHashMap<string, Point>();
		private IDictionary<string, Point> completePointsWithStochasticDeflections = new LinkedHashMap<string, Point>();
		private IDictionary<string, Point> completePointsWithUnknownDeflections = new LinkedHashMap<string, Point>();

		private IDictionary<int, AdditionalUnknownParameter> additionalParametersToBeEstimated = new LinkedHashMap<int, AdditionalUnknownParameter>();

		private IList<CongruenceAnalysisGroup> congruenceAnalysisGroups = new List<CongruenceAnalysisGroup>();
		private IList<ObservationGroup> completeObservationGroups = new List<ObservationGroup>();

		private Reduction reductions = new Reduction();

		private NetworkAdjustment networkAdjustment = null;

		private EstimationType estimationType = null;

		private bool freeNetwork = false, congruenceAnalysis = false, pure1DNetwork = true, estimateOrientationApproximation = true, applicableHorizontalProjection = true;

		public SQLAdjustmentManager(DataBase dataBase)
		{
			if (dataBase == null || !dataBase.Open)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, database must be open! " + dataBase);
			}
			this.dataBase = dataBase;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void setDataBaseSchema() throws java.sql.SQLException
		private void setDataBaseSchema()
		{
			this.dataBase.getPreparedStatement("SET SCHEMA \"OpenAdjustment\"").execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void checkDatabaseVersion() throws SQLException, org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException
		private void checkDatabaseVersion()
		{
			const string sql = "SELECT \"version\" FROM \"Version\" WHERE \"type\" = ? LIMIT 1";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			stmt.setInt(1, VersionType.DATABASE.getId());

			int databaseVersion = -1;
			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				databaseVersion = rs.getInt("version");
			}

			if (databaseVersion != Version.get(VersionType.DATABASE))
			{
				throw new DatabaseVersionMismatchException("Error, database version of the stored project is unequal to the accepted database version of the application: " + databaseVersion + " != " + Version.get(VersionType.DATABASE));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.applied_geodesy.adjustment.network.NetworkAdjustment getNetworkAdjustment() throws SQLException, DatabaseVersionMismatchException, IllegalProjectionPropertyException
		public virtual NetworkAdjustment NetworkAdjustment
		{
			get
			{
				this.clear();
    
				this.setDataBaseSchema();
				this.checkDatabaseVersion();
				this.setReductionDefinition();
    
				this.networkAdjustment = new NetworkAdjustment();
				this.addAdjustmentDefinition(this.networkAdjustment);
    
				// Definition of test statistic
				TestStatisticDefinition testStatisticDefinition = this.TestStatisticDefinition;
				if (testStatisticDefinition != null)
				{
					this.networkAdjustment.SignificanceTestStatisticDefinition = testStatisticDefinition;
				}
    
				IDictionary<string, Point> newPoints = this.getPointsByType(PointType.NEW_POINT);
				IDictionary<string, Point> datumPoints = this.getPointsByType(PointType.DATUM_POINT);
    
				IDictionary<string, Point> stochasticPoints = new LinkedHashMap<string, Point>(0);
				IDictionary<string, Point> referencePoints = new LinkedHashMap<string, Point>(0);
				this.completeNewPoints.PutAll(newPoints);
    
				this.freeNetwork = datumPoints != null && datumPoints.Count > 0;
				if (!this.freeNetwork)
				{
					stochasticPoints = this.getPointsByType(PointType.STOCHASTIC_POINT);
					referencePoints = this.getPointsByType(PointType.REFERENCE_POINT);
				}
    
				// add vertical deflection to points (if any)
				this.addVerticalDeflections();
    
				if (this.pure1DNetwork)
				{
					this.applicableHorizontalProjection = false;
				}
    
				// Abbildungsreduktionen sind bei einer Diagnoseauswertung unzulaessig, da es keine realen Beobachtungen gibt 
				if (this.estimationType == EstimationType.SIMULATION && this.reductions.size() > 0)
				{
					throw new IllegalProjectionPropertyException("Projection cannot applied to pseudo-observations in diagnosis adjustment (simulation)!");
				}
    
				// wenn 2D Projektionen nicht moeglich sind, werden keine Reduktionen durchgefuehrt
				if ((this.reductions.ProjectionType == ProjectionType.GAUSS_KRUEGER || this.reductions.ProjectionType == ProjectionType.UTM) && (this.reductions.applyReductionTask(ReductionTaskType.DIRECTION) || this.reductions.applyReductionTask(ReductionTaskType.DISTANCE)) && !this.applicableHorizontalProjection)
				{
					if (this.pure1DNetwork)
					{
						throw new IllegalProjectionPropertyException("Projection cannot applied to observations of leveling network! " + this.reductions.ProjectionType);
					}
					else
					{
						throw new IllegalProjectionPropertyException("Projection cannot applied to observations because the coordinates are invalid, e.g. missing zone number! " + this.reductions.ProjectionType);
					}
				}
    
				if (this.reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
				{
					SphericalDeflectionModel sphericalDeflectionModel = new SphericalDeflectionModel(this.reductions);
					foreach (Point point in this.completePoints.Values)
					{
						sphericalDeflectionModel.SphericalDeflections = point;
					}
					this.networkAdjustment.SphericalDeflectionModel = sphericalDeflectionModel;
				}
    
				// Fuege Beobachtungen zu den Punkten hinzu
				((List<ObservationGroup>)this.completeObservationGroups).AddRange(this.ObservationGroups);
    
				// Fuege Punkt der Netzausgleichung zu
				foreach (Point point in newPoints.Values)
				{
					string name = point.Name;
					int dimension = point.Dimension;
					VerticalDeflectionType type = null;
					if (dimension != 2)
					{ // dimension == 3
						if (this.completePointsWithReferenceDeflections.ContainsKey(name))
						{
							type = VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION;
						}
						else if (this.completePointsWithStochasticDeflections.ContainsKey(name))
						{
							type = VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION;
						}
						else if (this.completePointsWithUnknownDeflections.ContainsKey(name))
						{
							type = VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION;
						}
					}
					this.networkAdjustment.addNewPoint(point, type);
				}
    
				// Nutze Deformationsvektoren zur Bildung von relativen Konfidenzbereichen (nicht nur bei freier AGL/Defo.-Analyse)
				((List<CongruenceAnalysisGroup>)this.congruenceAnalysisGroups).AddRange(this.CongruenceAnalysisGroups);
    
				foreach (CongruenceAnalysisGroup congruenceAnalysisGroup in this.congruenceAnalysisGroups)
				{
					this.networkAdjustment.addCongruenceAnalysisGroup(congruenceAnalysisGroup);
				}
    
				if (this.freeNetwork)
				{
					this.addUserDefinedRankDefect(this.networkAdjustment.RankDefect);
    
					foreach (Point point in datumPoints.Values)
					{
						string name = point.Name;
						int dimension = point.Dimension;
						VerticalDeflectionType type = null;
						if (dimension != 2)
						{ // dimension == 3
							if (this.completePointsWithReferenceDeflections.ContainsKey(name))
							{
								type = VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION;
							}
							else if (this.completePointsWithStochasticDeflections.ContainsKey(name))
							{
								type = VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION;
							}
							else if (this.completePointsWithUnknownDeflections.ContainsKey(name))
							{
								type = VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION;
							}
						}
						this.networkAdjustment.addDatumPoint(point, type);
					}
    
				}
				else
				{
					foreach (Point point in referencePoints.Values)
					{
						string name = point.Name;
						int dimension = point.Dimension;
						VerticalDeflectionType type = null;
						if (dimension != 2)
						{ // dimension == 3
							if (this.completePointsWithReferenceDeflections.ContainsKey(name))
							{
								type = VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION;
							}
							else if (this.completePointsWithStochasticDeflections.ContainsKey(name))
							{
								type = VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION;
							}
							else if (this.completePointsWithUnknownDeflections.ContainsKey(name))
							{
								type = VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION;
							}
						}
						this.networkAdjustment.addReferencePoint(point, type);
					}
    
					foreach (Point point in stochasticPoints.Values)
					{
						string name = point.Name;
						int dimension = point.Dimension;
						VerticalDeflectionType type = null;
						if (dimension != 2)
						{ // dimension == 3
							if (this.completePointsWithReferenceDeflections.ContainsKey(name))
							{
								type = VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION;
							}
							else if (this.completePointsWithStochasticDeflections.ContainsKey(name))
							{
								type = VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION;
							}
							else if (this.completePointsWithUnknownDeflections.ContainsKey(name))
							{
								type = VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION;
							}
						}
						this.networkAdjustment.addStochasticPoint(point, type);
					}
				}
				// Auszugleichende Zusatzparameter
				foreach (AdditionalUnknownParameter parameter in this.additionalParametersToBeEstimated.Values)
				{
					this.networkAdjustment.addAdditionalUnknownParameter(parameter);
				}
    
				return networkAdjustment;
			}
		}

		public virtual void clear()
		{
			this.completePoints.Clear();
			this.completeNewPoints.Clear();
			this.completePointsWithReferenceDeflections.Clear();
			this.completePointsWithStochasticDeflections.Clear();
			this.completePointsWithUnknownDeflections.Clear();

			this.additionalParametersToBeEstimated.Clear();

			this.congruenceAnalysisGroups.Clear();
			this.completeObservationGroups.Clear();

			this.reductions.ProjectionType = ProjectionType.LOCAL_CARTESIAN;
			this.reductions.PrincipalPoint.setCoordinates(0, 0, 0, 0, 0, 0);
			this.reductions.clear();

			if (this.networkAdjustment != null)
			{
				this.networkAdjustment.clearMatrices();
				this.networkAdjustment = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void setReductionDefinition() throws java.sql.SQLException
		private void setReductionDefinition()
		{
			string sql = "SELECT " + "\"projection_type\", " + "\"reference_latitude\", \"reference_longitude\", \"reference_height\", " + "\"major_axis\", \"minor_axis\", " + "\"x0\", \"y0\", \"z0\", \"type\" AS \"task_type\" " + "FROM \"ReductionTask\" " + "RIGHT JOIN \"ReductionDefinition\" " + "ON \"ReductionTask\".\"reduction_id\" = \"ReductionDefinition\".\"id\" " + "WHERE \"ReductionDefinition\".\"id\" = 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				int taskTypeId = rs.getInt("task_type");
				bool hasTaskType = !rs.wasNull();

				ProjectionType projectionType = ProjectionType.getEnumByValue(rs.getInt("projection_type"));
				double referenceLatitude = rs.getDouble("reference_latitude");
				double referenceLongitude = rs.getDouble("reference_longitude");
				double referenceHeight = rs.getDouble("reference_height");
				double majorAxis = rs.getDouble("major_axis");
				double minorAxis = rs.getDouble("minor_axis");
				double x0 = rs.getDouble("x0");
				double y0 = rs.getDouble("y0");
				double z0 = rs.getDouble("z0");

				this.reductions.ProjectionType = projectionType;
				this.reductions.PrincipalPoint.setCoordinates(x0, y0, z0, referenceLatitude, referenceLongitude, referenceHeight);
				this.reductions.Ellipsoid = Ellipsoid.createEllipsoidFromMinorAxis(Math.Max(majorAxis, minorAxis), Math.Min(majorAxis, minorAxis));

				if (hasTaskType && projectionType != ProjectionType.LOCAL_ELLIPSOIDAL)
				{
					ReductionTaskType taskType = ReductionTaskType.getEnumByValue(taskTypeId);
					this.reductions.addReductionTaskType(taskType);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private org.applied_geodesy.adjustment.statistic.TestStatisticDefinition getTestStatisticDefinition() throws java.sql.SQLException
		private TestStatisticDefinition TestStatisticDefinition
		{
			get
			{
				string sql = "SELECT \"type\", \"probability_value\", \"power_of_test\", \"familywise_error_rate\" FROM \"TestStatisticDefinition\" WHERE \"id\" = 1 LIMIT 1";
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
    
				ResultSet rs = stmt.executeQuery();
				if (rs.next())
				{
					TestStatisticType type = TestStatisticType.getEnumByValue(rs.getInt("type"));
					double probabilityValue = rs.getDouble("probability_value");
					double powerOfTest = rs.getDouble("power_of_test");
					bool familywiseErrorRate = rs.getBoolean("familywise_error_rate");
    
					if (type != null && probabilityValue > 0 && probabilityValue < 100 && powerOfTest > 0 && powerOfTest < 100)
					{
						return new TestStatisticDefinition(type, probabilityValue, powerOfTest, familywiseErrorRate);
					}
				}
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addAdjustmentDefinition(org.applied_geodesy.adjustment.network.NetworkAdjustment adjustment) throws java.sql.SQLException
		private void addAdjustmentDefinition(NetworkAdjustment adjustment)
		{
			string sql = "SELECT " + "\"type\", \"number_of_iterations\", \"robust_estimation_limit\", " + "\"number_of_principal_components\", \"apply_variance_of_unit_weight\", " + "\"estimate_direction_set_orientation_approximation\", " + "\"congruence_analysis\", \"export_covariance_matrix\", " + "\"scaling\", \"damping\", \"weight_zero\" " + "FROM \"AdjustmentDefinition\" " + "JOIN \"UnscentedTransformation\" " + "ON \"AdjustmentDefinition\".\"id\" = \"UnscentedTransformation\".\"id\" " + "WHERE \"id\" = 1 LIMIT 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				EstimationType type = EstimationType.getEnumByValue(rs.getInt("type"));
				int maximalNumberOfIterations = rs.getInt("number_of_iterations");
				double robustEstimationLimit = rs.getDouble("robust_estimation_limit");
				int numberOfPrincipalComponents = rs.getInt("number_of_principal_components");
				this.estimateOrientationApproximation = rs.getBoolean("estimate_direction_set_orientation_approximation");
				this.congruenceAnalysis = rs.getBoolean("congruence_analysis");
				bool exportCovarianceMatrix = rs.getBoolean("export_covariance_matrix");
				bool applyVarianceOfUnitWeight = rs.getBoolean("apply_variance_of_unit_weight");

				double scalingUT = rs.getDouble("scaling");
				double dampingUT = rs.getDouble("damping");
				double weight0UT = rs.getDouble("weight_zero");

				this.estimationType = type == null ? EstimationType.L2NORM : type;
				// Keine Schaetzung moeglich bei Simulationen 
				//applyVarianceOfUnitWeight = !(this.estimationType == EstimationType.L1NORM || this.estimationType == EstimationType.L2NORM) ? false : applyVarianceOfUnitWeight;
				applyVarianceOfUnitWeight = this.estimationType == EstimationType.SIMULATION ? false : applyVarianceOfUnitWeight;

				adjustment.MaximalNumberOfIterations = maximalNumberOfIterations;
				adjustment.RobustEstimationLimit = robustEstimationLimit;
				adjustment.NumberOfPrincipalComponents = numberOfPrincipalComponents;
				adjustment.EstimationType = this.estimationType;
				adjustment.CongruenceAnalysis = this.congruenceAnalysis;
				adjustment.ApplyAposterioriVarianceOfUnitWeight = applyVarianceOfUnitWeight;

				adjustment.UnscentedTransformationScaling = scalingUT;
				adjustment.UnscentedTransformationDamping = dampingUT;
				adjustment.UnscentedTransformationWeightZero = weight0UT;

				// export path of covariance matrix
				if (exportCovarianceMatrix && this.dataBase is HSQLDB)
				{
					this.networkAdjustment.CovarianceExportPathAndBaseName = ((HSQLDB)this.dataBase).DataBaseFileName;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addUserDefinedRankDefect(org.applied_geodesy.adjustment.network.RankDefect rankDefect) throws java.sql.SQLException
		private void addUserDefinedRankDefect(RankDefect rankDefect)
		{
			string sql = "SELECT " + "\"user_defined\", " + "\"ty\",\"tx\",\"tz\", " + "\"ry\",\"rx\",\"rz\", " + "\"sy\",\"sx\",\"sz\", " + "\"my\",\"mx\",\"mz\", " + "\"mxy\",\"mxyz\" " + "FROM \"RankDefect\" WHERE \"id\" = 1 LIMIT 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			ResultSet rs = stmt.executeQuery();
			if (rs.next())
			{
				bool userDefined = rs.getBoolean("user_defined");
				if (userDefined)
				{
					rankDefect.reset();

					if (rs.getBoolean("ty"))
					{
						rankDefect.TranslationYDefectType = DefectType.FREE;
					}
					if (rs.getBoolean("tx"))
					{
						rankDefect.TranslationXDefectType = DefectType.FREE;
					}
					if (rs.getBoolean("tz"))
					{
						rankDefect.TranslationZDefectType = DefectType.FREE;
					}

					if (rs.getBoolean("ry"))
					{
						rankDefect.RotationYDefectType = DefectType.FREE;
					}
					if (rs.getBoolean("rx"))
					{
						rankDefect.RotationXDefectType = DefectType.FREE;
					}
					if (rs.getBoolean("rz"))
					{
						rankDefect.RotationZDefectType = DefectType.FREE;
					}

					if (rs.getBoolean("sy"))
					{
						rankDefect.ShearYDefectType = DefectType.FREE;
					}
					if (rs.getBoolean("sx"))
					{
						rankDefect.ShearXDefectType = DefectType.FREE;
					}
					if (rs.getBoolean("sz"))
					{
						rankDefect.ShearZDefectType = DefectType.FREE;
					}

					if (rs.getBoolean("my"))
					{
						rankDefect.ScaleYDefectType = DefectType.FREE;
					}
					if (rs.getBoolean("mx"))
					{
						rankDefect.ScaleXDefectType = DefectType.FREE;
					}
					if (rs.getBoolean("mz"))
					{
						rankDefect.ScaleZDefectType = DefectType.FREE;
					}

					if (rs.getBoolean("mxy"))
					{
						rankDefect.ScaleXYDefectType = DefectType.FREE;
					}
					if (rs.getBoolean("mxyz"))
					{
						rankDefect.ScaleXYZDefectType = DefectType.FREE;
					}

				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.Map<String, org.applied_geodesy.adjustment.network.point.Point> getPointsByType(org.applied_geodesy.adjustment.network.PointType type) throws java.sql.SQLException
		private IDictionary<string, Point> getPointsByType(PointType type)
		{

			string sql = "SELECT \"name\", \"y0\", \"x0\", \"z0\", \"dimension\", " + "IFNULL(CASEWHEN( \"sigma_y0\" > 0, \"sigma_y0\", (SELECT \"value\" FROM \"PointGroupUncertainty\" WHERE \"group_id\" = \"PointApriori\".\"group_id\" AND \"type\" = ?)), ?) AS \"sigma_y0\", " + "IFNULL(CASEWHEN( \"sigma_x0\" > 0, \"sigma_x0\", (SELECT \"value\" FROM \"PointGroupUncertainty\" WHERE \"group_id\" = \"PointApriori\".\"group_id\" AND \"type\" = ?)), ?) AS \"sigma_x0\", " + "IFNULL(CASEWHEN( \"sigma_z0\" > 0, \"sigma_z0\", (SELECT \"value\" FROM \"PointGroupUncertainty\" WHERE \"group_id\" = \"PointApriori\".\"group_id\" AND \"type\" = ?)), ?) AS \"sigma_z0\"  " + "FROM \"PointApriori\" " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" " + "WHERE \"type\" = ? AND \"PointGroup\".\"enable\" = TRUE AND \"PointApriori\".\"enable\" = TRUE " + "ORDER BY \"dimension\" ASC, \"PointGroup\".\"id\" ASC, \"PointApriori\".\"id\" ASC";

			IDictionary<string, Point> points = new LinkedHashMap<string, Point>();

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			stmt.setInt(idx++, PointGroupUncertaintyType.COMPONENT_Y.getId());
			stmt.setDouble(idx++, DefaultUncertainty.UncertaintyY);

			stmt.setInt(idx++, PointGroupUncertaintyType.COMPONENT_X.getId());
			stmt.setDouble(idx++, DefaultUncertainty.UncertaintyX);

			stmt.setInt(idx++, PointGroupUncertaintyType.COMPONENT_Z.getId());
			stmt.setDouble(idx++, DefaultUncertainty.UncertaintyZ);

			stmt.setInt(idx++, type.getId());

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				string name = rs.getString("name");
				int dimension = rs.getInt("dimension");

				double y0 = rs.getDouble("y0");
				double x0 = rs.getDouble("x0");
				double z0 = rs.getDouble("z0");

				double sigmaY0 = rs.getDouble("sigma_y0");
				double sigmaX0 = rs.getDouble("sigma_x0");
				double sigmaZ0 = rs.getDouble("sigma_z0");

				Point point = null;

				switch (dimension)
				{
				case 1:
					point = new Point1D(name, x0, y0, z0, sigmaZ0);
					break;
				case 2:
					point = new Point2D(name, x0, y0, z0, sigmaX0, sigmaY0);


					this.pure1DNetwork = false;
					if (y0 < 1100000 || y0 > 59800000)
					{
						this.applicableHorizontalProjection = false;
					}
					break;
				case 3:
					point = new Point3D(name, x0, y0, z0, sigmaX0, sigmaY0, sigmaZ0);

					this.pure1DNetwork = false;
					if (y0 < 1100000 || y0 > 59800000)
					{
						this.applicableHorizontalProjection = false;
					}
					break;
				}
				if (point != null && !this.completePoints.ContainsKey(point.Name))
				{
					if (this.estimationType == EstimationType.SIMULATION)
					{
						point.X = point.X0;
						point.Y = point.Y0;
						point.Z = point.Z0;
					}
					points[point.Name] = point;
					this.completePoints[point.Name] = point;
				}
			}
			return points;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addVerticalDeflections() throws java.sql.SQLException
		private void addVerticalDeflections()
		{
			string sql = "SELECT \"name\", \"y0\", \"x0\", \"VerticalDeflectionGroup\".\"type\" AS \"vertical_deflection_type\", " + "IFNULL(CASEWHEN( \"sigma_y0\" > 0, \"sigma_y0\", (SELECT \"value\" FROM \"VerticalDeflectionGroupUncertainty\" WHERE \"group_id\" = \"VerticalDeflectionApriori\".\"group_id\" AND \"type\" = ?)), ?) AS \"sigma_y0\", " + "IFNULL(CASEWHEN( \"sigma_x0\" > 0, \"sigma_x0\", (SELECT \"value\" FROM \"VerticalDeflectionGroupUncertainty\" WHERE \"group_id\" = \"VerticalDeflectionApriori\".\"group_id\" AND \"type\" = ?)), ?) AS \"sigma_x0\"  " + "FROM \"VerticalDeflectionApriori\" " + "JOIN \"VerticalDeflectionGroup\" ON \"VerticalDeflectionApriori\".\"group_id\" = \"VerticalDeflectionGroup\".\"id\" " + "JOIN \"PointApriori\" ON \"VerticalDeflectionApriori\".\"name\" = \"PointApriori\".\"name\" " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" " + "WHERE \"VerticalDeflectionGroup\".\"enable\" = TRUE AND \"VerticalDeflectionApriori\".\"enable\" = TRUE " + "AND \"PointGroup\".\"enable\" = TRUE AND \"PointApriori\".\"enable\" = TRUE AND \"PointGroup\".\"dimension\" IN (1, 3) " + "ORDER BY \"VerticalDeflectionGroup\".\"id\" ASC, \"VerticalDeflectionApriori\".\"id\" ASC";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

			stmt.setInt(idx++, VerticalDeflectionGroupUncertaintyType.DEFLECTION_Y.getId());
			stmt.setDouble(idx++, DefaultUncertainty.UncertaintyDeflectionY);

			stmt.setInt(idx++, VerticalDeflectionGroupUncertaintyType.DEFLECTION_X.getId());
			stmt.setDouble(idx++, DefaultUncertainty.UncertaintyDeflectionX);

			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				VerticalDeflectionType type = VerticalDeflectionType.getEnumByValue(rs.getInt("vertical_deflection_type"));
				if (type == null)
				{
					continue;
				}

				string name = rs.getString("name");

				Point point = this.completePoints[name];
				if (point == null || point.Dimension == 2) // point.getDimension() != 3
				{
					continue;
				}

				double y0 = rs.getDouble("y0");
				double x0 = rs.getDouble("x0");

				double sigmaY0 = rs.getDouble("sigma_y0");
				double sigmaX0 = rs.getDouble("sigma_x0");

				point.VerticalDeflectionY.Value0 = y0;
				point.VerticalDeflectionX.Value0 = x0;

				point.VerticalDeflectionY.StdApriori = sigmaY0;
				point.VerticalDeflectionX.StdApriori = sigmaX0;

				switch (type.innerEnumValue)
				{
				case VerticalDeflectionType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION:
					this.completePointsWithUnknownDeflections[name] = point;
					break;
				case VerticalDeflectionType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION:
					this.completePointsWithReferenceDeflections[name] = point;
					break;
				case VerticalDeflectionType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION:
					this.completePointsWithStochasticDeflections[name] = point;
					break;
				}
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<org.applied_geodesy.adjustment.network.observation.group.ObservationGroup> getObservationGroups() throws java.sql.SQLException
		private IList<ObservationGroup> ObservationGroups
		{
			get
			{
				IList<ObservationGroup> observationGroups = new List<ObservationGroup>();
    
				string sql = "SELECT \"id\", \"type\", \"reference_epoch\", " + "\"UncertaintyZP\".\"value\"   AS \"sigma_0_zero_point\", " + "\"UncertaintySRD\".\"value\"  AS \"sigma_0_square_root_distance\", " + "\"UncertaintyDIST\".\"value\" AS \"sigma_0_distance\" " + "FROM \"ObservationGroup\" " + "LEFT JOIN \"ObservationGroupUncertainty\" AS \"UncertaintyZP\"   ON \"UncertaintyZP\".\"group_id\"   = \"ObservationGroup\".\"id\" AND \"UncertaintyZP\".\"type\"   = ? " + "LEFT JOIN \"ObservationGroupUncertainty\" AS \"UncertaintySRD\"  ON \"UncertaintySRD\".\"group_id\"  = \"ObservationGroup\".\"id\" AND \"UncertaintySRD\".\"type\"  = ? " + "LEFT JOIN \"ObservationGroupUncertainty\" AS \"UncertaintyDIST\" ON \"UncertaintyDIST\".\"group_id\" = \"ObservationGroup\".\"id\" AND \"UncertaintyDIST\".\"type\" = ? " + "WHERE \"ObservationGroup\".\"enable\" = TRUE " + "ORDER BY \"ObservationGroup\".\"type\" ASC, \"ObservationGroup\".\"id\" ASC";
    
				int idx = 1;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				stmt.setInt(idx++, ObservationGroupUncertaintyType.ZERO_POINT_OFFSET.getId());
				stmt.setInt(idx++, ObservationGroupUncertaintyType.SQUARE_ROOT_DISTANCE_DEPENDENT.getId());
				stmt.setInt(idx++, ObservationGroupUncertaintyType.DISTANCE_DEPENDENT.getId());
    
				ResultSet rs = stmt.executeQuery();
    
				while (rs.next())
				{
					ObservationType type = ObservationType.getEnumByValue(rs.getInt("type"));
    
					if (type == null)
					{
						continue;
					}
    
					int groupId = rs.getInt("id");
					Epoch epoch = rs.getBoolean("reference_epoch") ? Epoch.REFERENCE : Epoch.CONTROL;
    
					double sigmaZeroPointOffset = -1;
					double sigmaSquareRootDistance = -1;
					double sigmaDistance = -1;
    
					ObservationGroup group = null;
					switch (type.innerEnumValue)
					{
					case ObservationType.InnerEnum.LEVELING:
						sigmaZeroPointOffset = rs.getDouble("sigma_0_zero_point");
						if (rs.wasNull() || sigmaZeroPointOffset <= 0)
						{
							sigmaZeroPointOffset = DefaultUncertainty.UncertaintyLevelingZeroPointOffset;
						}
    
						sigmaSquareRootDistance = rs.getDouble("sigma_0_square_root_distance");
						if (rs.wasNull() || sigmaSquareRootDistance < 0)
						{
							sigmaSquareRootDistance = DefaultUncertainty.UncertaintyLevelingSquareRootDistanceDependent;
						}
    
						sigmaDistance = rs.getDouble("sigma_0_distance");
						if (rs.wasNull() || sigmaDistance < 0)
						{
							sigmaDistance = DefaultUncertainty.UncertaintyLevelingDistanceDependent;
						}
    
						group = new DeltaZGroup(groupId, sigmaZeroPointOffset, sigmaSquareRootDistance, sigmaDistance, epoch);
    
						this.addAdditionalGroupParameters(group);
						this.addTerrestrialObservations(group);
						break;
    
					case ObservationType.InnerEnum.DIRECTION:
					case ObservationType.InnerEnum.ZENITH_ANGLE:
						sigmaZeroPointOffset = rs.getDouble("sigma_0_zero_point");
						if (rs.wasNull() || sigmaZeroPointOffset <= 0)
						{
							sigmaZeroPointOffset = DefaultUncertainty.UncertaintyAngleZeroPointOffset;
						}
    
						sigmaSquareRootDistance = rs.getDouble("sigma_0_square_root_distance");
						if (rs.wasNull() || sigmaSquareRootDistance < 0)
						{
							sigmaSquareRootDistance = DefaultUncertainty.UncertaintyAngleSquareRootDistanceDependent;
						}
    
						sigmaDistance = rs.getDouble("sigma_0_distance");
						if (rs.wasNull() || sigmaDistance < 0)
						{
							sigmaDistance = DefaultUncertainty.UncertaintyAngleDistanceDependent;
						}
    
						if (type == ObservationType.DIRECTION)
						{
							group = new DirectionGroup(groupId, sigmaZeroPointOffset, sigmaSquareRootDistance, sigmaDistance, epoch);
						}
						else
						{
							group = new ZenithAngleGroup(groupId, sigmaZeroPointOffset, sigmaSquareRootDistance, sigmaDistance, epoch);
						}
    
						this.addAdditionalGroupParameters(group);
						this.addTerrestrialObservations(group);
						break;
    
					case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
					case ObservationType.InnerEnum.SLOPE_DISTANCE:
						sigmaZeroPointOffset = rs.getDouble("sigma_0_zero_point");
						if (rs.wasNull() || sigmaZeroPointOffset <= 0)
						{
							sigmaZeroPointOffset = DefaultUncertainty.UncertaintyDistanceZeroPointOffset;
						}
    
						sigmaSquareRootDistance = rs.getDouble("sigma_0_square_root_distance");
						if (rs.wasNull() || sigmaSquareRootDistance < 0)
						{
							sigmaSquareRootDistance = DefaultUncertainty.UncertaintyDistanceSquareRootDistanceDependent;
						}
    
						sigmaDistance = rs.getDouble("sigma_0_distance");
						if (rs.wasNull() || sigmaDistance < 0)
						{
							sigmaDistance = DefaultUncertainty.UncertaintyDistanceDistanceDependent;
						}
    
						if (type == ObservationType.HORIZONTAL_DISTANCE)
						{
							group = new HorizontalDistanceGroup(groupId, sigmaZeroPointOffset, sigmaSquareRootDistance, sigmaDistance, epoch);
						}
						else
						{
							group = new SlopeDistanceGroup(groupId, sigmaZeroPointOffset, sigmaSquareRootDistance, sigmaDistance, epoch);
						}
    
						this.addAdditionalGroupParameters(group);
						this.addTerrestrialObservations(group);
						break;
    
					case ObservationType.InnerEnum.GNSS1D:
					case ObservationType.InnerEnum.GNSS2D:
					case ObservationType.InnerEnum.GNSS3D:
						sigmaZeroPointOffset = rs.getDouble("sigma_0_zero_point");
						if (rs.wasNull() || sigmaZeroPointOffset <= 0)
						{
							sigmaZeroPointOffset = DefaultUncertainty.UncertaintyGNSSZeroPointOffset;
						}
    
						sigmaSquareRootDistance = rs.getDouble("sigma_0_square_root_distance");
						if (rs.wasNull() || sigmaSquareRootDistance < 0)
						{
							sigmaSquareRootDistance = DefaultUncertainty.UncertaintyGNSSSquareRootDistanceDependent;
						}
    
						sigmaDistance = rs.getDouble("sigma_0_distance");
						if (rs.wasNull() || sigmaDistance < 0)
						{
							sigmaDistance = DefaultUncertainty.UncertaintyGNSSDistanceDependent;
						}
    
						if (type == ObservationType.GNSS1D)
						{
							group = new GNSSBaseline1DGroup(groupId, sigmaZeroPointOffset, sigmaSquareRootDistance, sigmaDistance, epoch);
						}
						else if (type == ObservationType.GNSS2D)
						{
							group = new GNSSBaseline2DGroup(groupId, sigmaZeroPointOffset, sigmaSquareRootDistance, sigmaDistance, epoch);
						}
						else
						{
							group = new GNSSBaseline3DGroup(groupId, sigmaZeroPointOffset, sigmaSquareRootDistance, sigmaDistance, epoch);
						}
    
						this.addAdditionalGroupParameters(group);
						this.addGNSSObservations(group);
						break;
					}
    
					if (group != null && !group.Empty)
					{
						observationGroups.Add(group);
					}
				}
    
				return observationGroups;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addTerrestrialObservations(org.applied_geodesy.adjustment.network.observation.group.ObservationGroup observationGroup) throws java.sql.SQLException
		private void addTerrestrialObservations(ObservationGroup observationGroup)
		{
			string sql = "SELECT \"id\", \"start_point_name\", \"end_point_name\", \"instrument_height\", \"reflector_height\", \"value_0\", \"sigma_0\", \"distance_0\" " + "FROM \"ObservationApriori\" " + "WHERE \"group_id\" = ? AND \"enable\" = TRUE " + "ORDER BY \"id\" ASC";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, observationGroup.Id);

			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{

				int id = rs.getInt("id");

				string startPointName = rs.getString("start_point_name");
				string endPointName = rs.getString("end_point_name");

				double startPointHeight = rs.getDouble("instrument_height");
				double endPointHeight = rs.getDouble("reflector_height");

				double observation0 = rs.getDouble("value_0");
				double sigma0 = rs.getDouble("sigma_0");

				double distanceForUncertaintyModel = rs.getDouble("distance_0");

				Point startPoint = this.completePoints[startPointName];
				Point endPoint = this.completePoints[endPointName];

				if (startPoint == null || endPoint == null)
				{
					continue;
				}

				Observation observation = null;

				if (observationGroup is DeltaZGroup && startPoint.Dimension != 2 && endPoint.Dimension != 2)
				{
					observation = new DeltaZ(id, startPoint, endPoint, startPointHeight, endPointHeight, observation0, sigma0, distanceForUncertaintyModel);
				}

				else if (observationGroup is DirectionGroup && startPoint.Dimension != 1 && endPoint.Dimension != 1)
				{
					observation = new Direction(id, startPoint, endPoint, startPointHeight, endPointHeight, observation0, sigma0, distanceForUncertaintyModel);
				}

				else if (observationGroup is HorizontalDistanceGroup && startPoint.Dimension != 1 && endPoint.Dimension != 1)
				{
					observation = new HorizontalDistance(id, startPoint, endPoint, startPointHeight, endPointHeight, observation0, sigma0, distanceForUncertaintyModel);
				}

				else if (observationGroup is SlopeDistanceGroup && startPoint.Dimension == 3 && endPoint.Dimension == 3)
				{
					observation = new SlopeDistance(id, startPoint, endPoint, startPointHeight, endPointHeight, observation0, sigma0, distanceForUncertaintyModel);
				}

				else if (observationGroup is ZenithAngleGroup && startPoint.Dimension == 3 && endPoint.Dimension == 3)
				{
					observation = new ZenithAngle(id, startPoint, endPoint, startPointHeight, endPointHeight, observation0, sigma0, distanceForUncertaintyModel);
				}

				if (this.estimationType == EstimationType.SIMULATION)
				{
					observation.ValueApriori = observation.ValueAposteriori;
				}

				if (observation != null)
				{
					observation.Reduction = this.reductions;
					observationGroup.add(observation);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addGNSSObservations(org.applied_geodesy.adjustment.network.observation.group.ObservationGroup observationGroup) throws java.sql.SQLException
		private void addGNSSObservations(ObservationGroup observationGroup)
		{
			string sql = "SELECT \"id\", \"start_point_name\", \"end_point_name\", \"y0\", \"x0\", \"z0\", \"sigma_y0\", \"sigma_x0\", \"sigma_z0\" " + "FROM \"GNSSObservationApriori\" " + "WHERE \"group_id\" = ? AND \"enable\" = TRUE " + "ORDER BY \"id\" ASC";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, observationGroup.Id);

			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{

				int id = rs.getInt("id");

				string startPointName = rs.getString("start_point_name");
				string endPointName = rs.getString("end_point_name");

				double y0 = rs.getDouble("y0");
				double x0 = rs.getDouble("x0");
				double z0 = rs.getDouble("z0");

				double sigmaY0 = rs.getDouble("sigma_y0");
				double sigmaX0 = rs.getDouble("sigma_x0");
				double sigmaZ0 = rs.getDouble("sigma_z0");

				Point startPoint = this.completePoints[startPointName];
				Point endPoint = this.completePoints[endPointName];

				if (startPoint == null || endPoint == null)
				{
					continue;
				}


				if (observationGroup is GNSSBaseline1DGroup && startPoint.Dimension != 2 && endPoint.Dimension != 2)
				{
					GNSSBaselineDeltaZ1D gnssZ = new GNSSBaselineDeltaZ1D(id, startPoint, endPoint, z0, sigmaZ0);

					gnssZ.Reduction = this.reductions;
					if (this.estimationType == EstimationType.SIMULATION)
					{
						gnssZ.ValueApriori = gnssZ.ValueAposteriori;
					}

					((GNSSBaseline1DGroup)observationGroup).add(gnssZ);
				}
				else if (observationGroup is GNSSBaseline2DGroup && startPoint.Dimension != 1 && endPoint.Dimension != 1)
				{
					GNSSBaselineDeltaY2D gnssY = new GNSSBaselineDeltaY2D(id, startPoint, endPoint, y0, sigmaY0);
					GNSSBaselineDeltaX2D gnssX = new GNSSBaselineDeltaX2D(id, startPoint, endPoint, x0, sigmaX0);

					gnssX.Reduction = this.reductions;
					gnssY.Reduction = this.reductions;
					if (this.estimationType == EstimationType.SIMULATION)
					{
						gnssX.ValueApriori = gnssX.ValueAposteriori;
						gnssY.ValueApriori = gnssY.ValueAposteriori;
					}

					((GNSSBaseline2DGroup)observationGroup).add(gnssX, gnssY);

				}
				else if (observationGroup is GNSSBaseline3DGroup && startPoint.Dimension == 3 && endPoint.Dimension == 3)
				{
					GNSSBaselineDeltaY3D gnssY = new GNSSBaselineDeltaY3D(id, startPoint, endPoint, y0, sigmaY0);
					GNSSBaselineDeltaX3D gnssX = new GNSSBaselineDeltaX3D(id, startPoint, endPoint, x0, sigmaX0);
					GNSSBaselineDeltaZ3D gnssZ = new GNSSBaselineDeltaZ3D(id, startPoint, endPoint, z0, sigmaZ0);

					gnssX.Reduction = this.reductions;
					gnssY.Reduction = this.reductions;
					gnssZ.Reduction = this.reductions;
					if (this.estimationType == EstimationType.SIMULATION)
					{
						gnssX.ValueApriori = gnssX.ValueAposteriori;
						gnssY.ValueApriori = gnssY.ValueAposteriori;
						gnssZ.ValueApriori = gnssZ.ValueAposteriori;
					}

					((GNSSBaseline3DGroup)observationGroup).add(gnssX, gnssY, gnssZ);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addAdditionalGroupParameters(org.applied_geodesy.adjustment.network.observation.group.ObservationGroup observationGroup) throws java.sql.SQLException
		private void addAdditionalGroupParameters(ObservationGroup observationGroup)
		{
			string sql = "SELECT \"id\", \"type\", \"value_0\", \"enable\" " + "FROM \"AdditionalParameterApriori\" WHERE \"group_id\" = ? " + "ORDER BY \"id\" ASC";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(1, observationGroup.Id);
			// additionalParametersToBeEstimated
			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				ParameterType type = ParameterType.getEnumByValue(rs.getInt("type"));

				int id = rs.getInt("id");
				double value0 = rs.getDouble("value_0");
				bool enable = rs.getBoolean("enable");

				if (type == null || this.additionalParametersToBeEstimated.ContainsKey(id))
				{
					continue;
				}

				AdditionalUnknownParameter parameter = null;

				switch (type.innerEnumValue)
				{
				case ParameterType.InnerEnum.ORIENTATION:
					if (observationGroup is DirectionGroup)
					{
						parameter = ((DirectionGroup)observationGroup).Orientation;
						parameter.Value = value0;
						((Orientation)parameter).EstimateApproximationValue = this.estimateOrientationApproximation && this.estimationType != EstimationType.SIMULATION;
					}
					break;

				case ParameterType.InnerEnum.ZERO_POINT_OFFSET:
					if (observationGroup is HorizontalDistanceGroup)
					{
						parameter = ((HorizontalDistanceGroup)observationGroup).ZeroPointOffset;
						parameter.Value = value0;
					}

					else if (observationGroup is SlopeDistanceGroup)
					{
						parameter = ((SlopeDistanceGroup)observationGroup).ZeroPointOffset;
						parameter.Value = value0;
					}
					break;

				case ParameterType.InnerEnum.SCALE:
					if (observationGroup is DeltaZGroup)
					{
						parameter = ((DeltaZGroup)observationGroup).Scale;
						parameter.Value = value0;
					}
					else if (observationGroup is HorizontalDistanceGroup)
					{
						parameter = ((HorizontalDistanceGroup)observationGroup).Scale;
						parameter.Value = value0;
					}
					else if (observationGroup is SlopeDistanceGroup)
					{
						parameter = ((SlopeDistanceGroup)observationGroup).Scale;
						parameter.Value = value0;
					}
					else if (observationGroup is GNSSBaseline1DGroup)
					{
						parameter = ((GNSSBaseline1DGroup)observationGroup).Scale;
						parameter.Value = value0;
					}
					else if (observationGroup is GNSSBaseline2DGroup)
					{
						parameter = ((GNSSBaseline2DGroup)observationGroup).Scale;
						parameter.Value = value0;
					}
					else if (observationGroup is GNSSBaseline3DGroup)
					{
						parameter = ((GNSSBaseline3DGroup)observationGroup).Scale;
						parameter.Value = value0;
					}
					break;

				case ParameterType.InnerEnum.REFRACTION_INDEX:
					if (observationGroup is ZenithAngleGroup)
					{
						parameter = ((ZenithAngleGroup)observationGroup).RefractionCoefficient;
						parameter.Value = value0;
					}
					break;

				case ParameterType.InnerEnum.ROTATION_X:
					if (observationGroup is GNSSBaseline1DGroup)
					{
						parameter = ((GNSSBaseline1DGroup)observationGroup).RotationX;
						parameter.Value = value0;
					}
					else if (observationGroup is GNSSBaseline3DGroup)
					{
						parameter = ((GNSSBaseline3DGroup)observationGroup).RotationX;
						parameter.Value = value0;
					}
					break;

				case ParameterType.InnerEnum.ROTATION_Y:
					if (observationGroup is GNSSBaseline1DGroup)
					{
						parameter = ((GNSSBaseline1DGroup)observationGroup).RotationY;
						parameter.Value = value0;
					}
					else if (observationGroup is GNSSBaseline3DGroup)
					{
						parameter = ((GNSSBaseline3DGroup)observationGroup).RotationY;
						parameter.Value = value0;
					}
					break;

				case ParameterType.InnerEnum.ROTATION_Z:
					if (observationGroup is GNSSBaseline2DGroup)
					{
						parameter = ((GNSSBaseline2DGroup)observationGroup).RotationZ;
						parameter.Value = value0;
					}
					else if (observationGroup is GNSSBaseline3DGroup)
					{
						parameter = ((GNSSBaseline3DGroup)observationGroup).RotationZ;
						parameter.Value = value0;
					}
					break;


				default:
					break;

				}

				if (parameter != null && enable)
				{
					this.additionalParametersToBeEstimated[id] = parameter;
				}
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<org.applied_geodesy.adjustment.network.congruence.CongruenceAnalysisGroup> getCongruenceAnalysisGroups() throws java.sql.SQLException
		private IList<CongruenceAnalysisGroup> CongruenceAnalysisGroups
		{
			get
			{
				IList<CongruenceAnalysisGroup> congruenceAnalysisGroups = new List<CongruenceAnalysisGroup>();
    
				string sql = "SELECT \"id\", \"dimension\" " + "FROM \"CongruenceAnalysisGroup\" " + "WHERE \"enable\" = TRUE";
    
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				ResultSet rs = stmt.executeQuery();
    
				while (rs.next())
				{
					int id = rs.getInt("id");
					int dimension = rs.getInt("dimension");
					if (dimension > 0 && dimension < 4)
					{
						CongruenceAnalysisGroup congruenceAnalysisGroup = new CongruenceAnalysisGroup(id, dimension);
						this.addCongruenceAnalysisPointPair(congruenceAnalysisGroup);
						if (!congruenceAnalysisGroup.Empty)
						{
							if (this.freeNetwork && this.congruenceAnalysis)
							{
								this.StrainAnalysisRestrictionsToGroup = congruenceAnalysisGroup;
							}
							congruenceAnalysisGroups.Add(congruenceAnalysisGroup);
						}
					}
				}
    
				return congruenceAnalysisGroups;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addCongruenceAnalysisPointPair(org.applied_geodesy.adjustment.network.congruence.CongruenceAnalysisGroup congruenceAnalysisGroup) throws java.sql.SQLException
		private void addCongruenceAnalysisPointPair(CongruenceAnalysisGroup congruenceAnalysisGroup)
		{
			string sql = "SELECT \"id\", \"start_point_name\", \"end_point_name\" " + "FROM \"CongruenceAnalysisPointPairApriori\" WHERE \"group_id\" = ? AND \"enable\" = TRUE " + "ORDER BY \"id\" ASC";

			int dimension = congruenceAnalysisGroup.Dimension;

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, congruenceAnalysisGroup.Id);
			ResultSet rs = stmt.executeQuery();
			while (rs.next())
			{
				int id = rs.getInt("id");
				string startPointName = rs.getString("start_point_name");
				string endPointName = rs.getString("end_point_name");

				if (!this.completePoints.ContainsKey(startPointName) || !this.completePoints.ContainsKey(endPointName))
				{
					continue;
				}

				Point startPoint = this.completePoints[startPointName];
				Point endPoint = this.completePoints[endPointName];

				if (startPoint.Dimension < dimension || endPoint.Dimension < dimension || startPoint.Dimension == 2 && endPoint.Dimension == 1 || startPoint.Dimension == 1 && endPoint.Dimension == 2)
				{
					continue;
				}

				CongruenceAnalysisPointPair pointPointPair = new CongruenceAnalysisPointPair(id, dimension, startPoint, endPoint);
				bool isAnalysable = this.completeNewPoints.ContainsKey(startPointName) && this.completeNewPoints.ContainsKey(endPointName);
				congruenceAnalysisGroup.add(pointPointPair, isAnalysable);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void setStrainAnalysisRestrictionsToGroup(org.applied_geodesy.adjustment.network.congruence.CongruenceAnalysisGroup group) throws java.sql.SQLException
		private CongruenceAnalysisGroup StrainAnalysisRestrictionsToGroup
		{
			set
			{
				string sql = "SELECT " + "\"type\", \"enable\" " + "FROM \"CongruenceAnalysisStrainParameterRestriction\" " + "WHERE \"group_id\" = ?";
    
				int idx = 1;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				stmt.setInt(idx++, value.Id);
    
				ResultSet rs = stmt.executeQuery();
    
				while (rs.next())
				{
					int type = rs.getInt("type");
					bool enable = rs.getBoolean("enable");
					RestrictionType restriction = RestrictionType.getEnumByValue(type);
					if (restriction != null && !enable)
					{
						value.StrainRestrictions = restriction;
					}
				}
			}
		}

		/// <summary>
		/// SAVE RESULTS * </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveResults() throws java.sql.SQLException
		public virtual void saveResults()
		{
			try
			{
				if (this.networkAdjustment != null)
				{
					// Tabelle fuer Daten nach der AGL leeren
					this.clearAposterioriTables();

					this.savePoints();
					this.saveVerticalDeflections();

					this.saveObservations();
					this.saveAdditionalParameters();

					this.saveCongruenceAnalysisPointPair();
					this.saveStrainParameters();

					this.savePrincipalComponentAnalysis(this.networkAdjustment.PrincipalComponents);
					this.saveRankDefect(this.networkAdjustment.RankDefect);
					this.saveTestStatistic(this.networkAdjustment.SignificanceTestStatisticParameters);
					this.saveVarianceComponents(this.networkAdjustment.VarianceComponents);

					this.saveVersion();
				}
			}
			finally
			{
				this.clear();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveVersion() throws java.sql.SQLException
		private void saveVersion()
		{
			string sql = "UPDATE \"Version\" SET \"version\" = ? WHERE \"type\" = ?";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setDouble(idx++, Version.get(VersionType.ADJUSTMENT_CORE));
			stmt.setInt(idx++, VersionType.ADJUSTMENT_CORE.getId());
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void clearAposterioriTables() throws java.sql.SQLException
		private void clearAposterioriTables()
		{
			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"PointAposteriori\"").execute();
			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"VerticalDeflectionAposteriori\"").execute();
			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"ObservationAposteriori\"").execute();
			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"GNSSObservationAposteriori\"").execute();
			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"AdditionalParameterAposteriori\"").execute();
			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"CongruenceAnalysisPointPairAposteriori\"").execute();
			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"CongruenceAnalysisStrainParameterAposteriori\"").execute();

			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"VarianceComponent\"").execute();
			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"TestStatistic\"").execute();
			this.dataBase.getPreparedStatement("TRUNCATE TABLE \"PrincipalComponent\"").execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void savePoints() throws java.sql.SQLException
		private void savePoints()
		{
			bool hasBatch = false;

			string sql = "INSERT INTO \"PointAposteriori\" (" + "\"id\",\"y\",\"x\",\"z\"," + "\"sigma_y0\",\"sigma_x0\",\"sigma_z0\"," + "\"sigma_y\",\"sigma_x\",\"sigma_z\"," + "\"confidence_major_axis\",\"confidence_middle_axis\",\"confidence_minor_axis\"," + "\"confidence_alpha\",\"confidence_beta\",\"confidence_gamma\"," + "\"helmert_major_axis\",\"helmert_minor_axis\",\"helmert_alpha\"," + "\"residual_y\",\"residual_x\",\"residual_z\"," + "\"redundancy_y\",\"redundancy_x\",\"redundancy_z\"," + "\"gross_error_y\",\"gross_error_x\",\"gross_error_z\"," + "\"minimal_detectable_bias_y\",\"minimal_detectable_bias_x\",\"minimal_detectable_bias_z\"," + "\"maximum_tolerable_bias_y\",\"maximum_tolerable_bias_x\",\"maximum_tolerable_bias_z\", " + "\"influence_on_position_y\",\"influence_on_position_x\",\"influence_on_position_z\"," + "\"influence_on_network_distortion\", " + "\"first_principal_component_y\",\"first_principal_component_x\",\"first_principal_component_z\"," + "\"omega\",\"p_prio\",\"p_post\",\"t_prio\",\"t_post\",\"significant\",\"covar_index\") VALUES (" + "(SELECT \"id\" FROM \"PointApriori\" WHERE \"name\" = ?), " + "?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

			try
			{
				this.dataBase.AutoCommit = false;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				foreach (Point point in this.completePoints.Values)
				{
					int idx = 1;
					int dimension = point.Dimension;

					stmt.setString(idx++, point.Name);

					stmt.setDouble(idx++, point.Y);
					stmt.setDouble(idx++, point.X);
					stmt.setDouble(idx++, dimension != 2 ? point.Z : 0.0);

					stmt.setDouble(idx++, dimension != 1 && point.StdYApriori > 0 ? point.StdYApriori : 0.0);
					stmt.setDouble(idx++, dimension != 1 && point.StdXApriori > 0 ? point.StdXApriori : 0.0);
					stmt.setDouble(idx++, dimension != 2 && point.StdZApriori > 0 ? point.StdZApriori : 0.0);

					stmt.setDouble(idx++, dimension != 1 && point.StdY > 0 ? point.StdY : 0.0);
					stmt.setDouble(idx++, dimension != 1 && point.StdX > 0 ? point.StdX : 0.0);
					stmt.setDouble(idx++, dimension != 2 && point.StdZ > 0 ? point.StdZ : 0.0);

					stmt.setDouble(idx++, point.getConfidenceAxis(0));
					stmt.setDouble(idx++, dimension == 3 ? point.getConfidenceAxis(1) : 0.0);
					stmt.setDouble(idx++, dimension != 1 ? point.getConfidenceAxis(dimension - 1) : 0.0);

					stmt.setDouble(idx++, dimension > 2 ? point.getConfidenceAngle(0) : 0.0);
					stmt.setDouble(idx++, dimension > 2 ? point.getConfidenceAngle(1) : 0.0);
					stmt.setDouble(idx++, dimension > 1 ? point.getConfidenceAngle(2) : 0.0);

					stmt.setDouble(idx++, point.getConfidenceAxis2D(0));
					stmt.setDouble(idx++, dimension != 1 ? point.getConfidenceAxis2D(1) : 0.0);
					stmt.setDouble(idx++, dimension != 1 ? point.ConfidenceAngle2D : 0.0);

					stmt.setDouble(idx++, dimension != 1 ? point.Y0 - point.Y : 0.0);
					stmt.setDouble(idx++, dimension != 1 ? point.X0 - point.X : 0.0);
					stmt.setDouble(idx++, dimension != 2 ? point.Z0 - point.Z : 0.0);

					stmt.setDouble(idx++, dimension != 1 ? point.RedundancyY : 0.0);
					stmt.setDouble(idx++, dimension != 1 ? point.RedundancyX : 0.0);
					stmt.setDouble(idx++, dimension != 2 ? point.RedundancyZ : 0.0);

					stmt.setDouble(idx++, dimension != 1 ? point.GrossErrorY : 0.0);
					stmt.setDouble(idx++, dimension != 1 ? point.GrossErrorX : 0.0);
					stmt.setDouble(idx++, dimension != 2 ? point.GrossErrorZ : 0.0);

					stmt.setDouble(idx++, dimension != 1 ? point.MinimalDetectableBiasY : 0.0);
					stmt.setDouble(idx++, dimension != 1 ? point.MinimalDetectableBiasX : 0.0);
					stmt.setDouble(idx++, dimension != 2 ? point.MinimalDetectableBiasZ : 0.0);

					stmt.setDouble(idx++, dimension != 1 ? point.MaximumTolerableBiasY : 0.0);
					stmt.setDouble(idx++, dimension != 1 ? point.MaximumTolerableBiasX : 0.0);
					stmt.setDouble(idx++, dimension != 2 ? point.MaximumTolerableBiasZ : 0.0);

					stmt.setDouble(idx++, dimension != 1 ? point.InfluenceOnPointPositionY : 0.0);
					stmt.setDouble(idx++, dimension != 1 ? point.InfluenceOnPointPositionX : 0.0);
					stmt.setDouble(idx++, dimension != 2 ? point.InfluenceOnPointPositionZ : 0.0);

					stmt.setDouble(idx++, point.InfluenceOnNetworkDistortion);

					stmt.setDouble(idx++, dimension != 1 ? point.FirstPrincipalComponentY : 0.0);
					stmt.setDouble(idx++, dimension != 1 ? point.FirstPrincipalComponentX : 0.0);
					stmt.setDouble(idx++, dimension != 2 ? point.FirstPrincipalComponentZ : 0.0);

					stmt.setDouble(idx++, point.Omega);

					stmt.setDouble(idx++, point.Pprio);
					stmt.setDouble(idx++, point.Ppost);

					stmt.setDouble(idx++, point.Tprio);
					stmt.setDouble(idx++, point.Tpost);

					stmt.setBoolean(idx++, point.Significant);

					stmt.setInt(idx++, point.ColInJacobiMatrix);

					stmt.addBatch();
					hasBatch = true;
				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveVerticalDeflections() throws java.sql.SQLException
		private void saveVerticalDeflections()
		{
			bool hasBatch = false;
			string sql = "INSERT INTO \"VerticalDeflectionAposteriori\" (" + "\"id\",\"y\",\"x\"," + "\"sigma_y0\",\"sigma_x0\"," + "\"sigma_y\",\"sigma_x\"," + "\"confidence_major_axis\",\"confidence_minor_axis\"," + "\"residual_y\",\"residual_x\"," + "\"redundancy_y\",\"redundancy_x\"," + "\"gross_error_y\",\"gross_error_x\"," + "\"minimal_detectable_bias_y\",\"minimal_detectable_bias_x\"," + "\"maximum_tolerable_bias_y\",\"maximum_tolerable_bias_x\"," + "\"omega\",\"p_prio\",\"p_post\",\"t_prio\",\"t_post\",\"significant\",\"covar_index\") VALUES (" + "(SELECT \"id\" FROM \"VerticalDeflectionApriori\" WHERE \"name\" = ?), " + "?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

			try
			{
				this.dataBase.AutoCommit = false;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				IDictionary<string, Point> completePointsWithDeflections = new Dictionary<string, Point>();
				completePointsWithDeflections.PutAll(this.completePointsWithReferenceDeflections);
				completePointsWithDeflections.PutAll(this.completePointsWithStochasticDeflections);
				completePointsWithDeflections.PutAll(this.completePointsWithUnknownDeflections);

				foreach (Point point in completePointsWithDeflections.Values)
				{
					int idx = 1;
					int dimension = point.Dimension;

					if (dimension == 2) // if (dimension != 3)
					{
						continue;
					}

					stmt.setString(idx++, point.Name);

					stmt.setDouble(idx++, point.VerticalDeflectionY.Value);
					stmt.setDouble(idx++, point.VerticalDeflectionX.Value);

					stmt.setDouble(idx++, (point.VerticalDeflectionY.StdApriori > 0 ? point.VerticalDeflectionY.StdApriori : 0.0));
					stmt.setDouble(idx++, (point.VerticalDeflectionX.StdApriori > 0 ? point.VerticalDeflectionX.StdApriori : 0.0));

					stmt.setDouble(idx++, point.VerticalDeflectionY.Std > 0 ? point.VerticalDeflectionY.Std : 0.0);
					stmt.setDouble(idx++, point.VerticalDeflectionX.Std > 0 ? point.VerticalDeflectionX.Std : 0.0);

					stmt.setDouble(idx++, Math.Max(point.VerticalDeflectionX.Confidence, point.VerticalDeflectionY.Confidence));
					stmt.setDouble(idx++, Math.Min(point.VerticalDeflectionX.Confidence, point.VerticalDeflectionY.Confidence));

					stmt.setDouble(idx++, point.VerticalDeflectionY.Value0 - point.VerticalDeflectionY.Value);
					stmt.setDouble(idx++, point.VerticalDeflectionX.Value0 - point.VerticalDeflectionX.Value);

					stmt.setDouble(idx++, point.VerticalDeflectionY.Redundancy);
					stmt.setDouble(idx++, point.VerticalDeflectionX.Redundancy);

					stmt.setDouble(idx++, point.VerticalDeflectionY.GrossError);
					stmt.setDouble(idx++, point.VerticalDeflectionX.GrossError);

					stmt.setDouble(idx++, point.VerticalDeflectionY.MinimalDetectableBias);
					stmt.setDouble(idx++, point.VerticalDeflectionX.MinimalDetectableBias);

					stmt.setDouble(idx++, point.VerticalDeflectionY.MaximumTolerableBias);
					stmt.setDouble(idx++, point.VerticalDeflectionX.MaximumTolerableBias);

					stmt.setDouble(idx++, point.VerticalDeflectionX.Omega + point.VerticalDeflectionY.Omega);

					// Statistische Groessen in X abgelegt
					stmt.setDouble(idx++, point.VerticalDeflectionX.Pprio);
					stmt.setDouble(idx++, point.VerticalDeflectionX.Ppost);

					stmt.setDouble(idx++, point.VerticalDeflectionX.Tprio);
					stmt.setDouble(idx++, point.VerticalDeflectionX.Tpost);

					stmt.setBoolean(idx++, point.VerticalDeflectionX.Significant);

					stmt.setInt(idx++, Math.Min(point.VerticalDeflectionX.ColInJacobiMatrix, point.VerticalDeflectionY.ColInJacobiMatrix));

					stmt.addBatch();

					hasBatch = true;

				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveObservations() throws java.sql.SQLException
		private void saveObservations()
		{
			bool hasBatch = false;
			string sqlTerObs = "INSERT INTO \"ObservationAposteriori\" (" + "\"id\", \"value\", " + "\"sigma_0\", \"sigma\", " + "\"residual\", " + "\"redundancy\", " + "\"gross_error\", \"minimal_detectable_bias\", \"maximum_tolerable_bias\", " + "\"influence_on_position\", \"influence_on_network_distortion\", " + "\"omega\", " + "\"p_prio\", \"p_post\", " + "\"t_prio\", \"t_post\", " + "\"significant\") VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

			string sqlGNSSObs = "INSERT INTO \"GNSSObservationAposteriori\" (" + "\"id\", \"y\", \"x\", \"z\", " + "\"sigma_y0\", \"sigma_x0\", \"sigma_z0\", " + "\"sigma_y\", \"sigma_x\", \"sigma_z\", " + "\"residual_y\", \"residual_x\", \"residual_z\", " + "\"redundancy_y\", \"redundancy_x\", \"redundancy_z\", " + "\"gross_error_y\", \"gross_error_x\", \"gross_error_z\", " + "\"minimal_detectable_bias_y\",\"minimal_detectable_bias_x\",\"minimal_detectable_bias_z\", " + "\"maximum_tolerable_bias_y\",\"maximum_tolerable_bias_x\",\"maximum_tolerable_bias_z\", " + "\"influence_on_position_y\", \"influence_on_position_x\", \"influence_on_position_z\", " + "\"influence_on_network_distortion\", " + "\"omega\", \"p_prio\", \"p_post\", \"t_prio\", \"t_post\", \"significant\") " + "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ? ,? ,?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

			try
			{
				this.dataBase.AutoCommit = false;
				ISet<int> gnssObservationIds = new LinkedHashSet<int>();
				foreach (ObservationGroup observationGroup in this.completeObservationGroups)
				{
					gnssObservationIds.Clear();
					bool isGNSS = (observationGroup is GNSSBaseline1DGroup || observationGroup is GNSSBaseline2DGroup || observationGroup is GNSSBaseline3DGroup);
					PreparedStatement stmt;
					if (isGNSS)
					{
						stmt = this.dataBase.getPreparedStatement(sqlGNSSObs);
					}
					else
					{
						stmt = this.dataBase.getPreparedStatement(sqlTerObs);
					}

					int len = observationGroup.size();

					for (int i = 0; i < len; i++)
					{
						Observation observation = observationGroup.get(i);
						int idx = 1;
						if (isGNSS)
						{
							if (gnssObservationIds.Contains(observation.Id))
							{
								continue;
							}
							gnssObservationIds.Add(observation.Id);

							GNSSBaseline gnssBaseline = (GNSSBaseline)observation;

							GNSSBaseline gnssY = gnssBaseline.getBaselineComponent(ComponentType.Y);
							GNSSBaseline gnssX = gnssBaseline.getBaselineComponent(ComponentType.X);
							GNSSBaseline gnssZ = gnssBaseline.getBaselineComponent(ComponentType.Z);

							double omega = 0;
							omega += gnssY == null ? 0.0 : gnssY.Omega;
							omega += gnssX == null ? 0.0 : gnssX.Omega;
							omega += gnssZ == null ? 0.0 : gnssZ.Omega;

							stmt.setInt(idx++, gnssBaseline.Id);

							stmt.setDouble(idx++, gnssY == null ? 0.0 : gnssY.ValueAposteriori);
							stmt.setDouble(idx++, gnssX == null ? 0.0 : gnssX.ValueAposteriori);
							stmt.setDouble(idx++, gnssZ == null ? 0.0 : gnssZ.ValueAposteriori);

							stmt.setDouble(idx++, gnssY == null || gnssY.StdApriori < 0 ? 0.0 : gnssY.StdApriori);
							stmt.setDouble(idx++, gnssX == null || gnssX.StdApriori < 0 ? 0.0 : gnssX.StdApriori);
							stmt.setDouble(idx++, gnssZ == null || gnssZ.StdApriori < 0 ? 0.0 : gnssZ.StdApriori);

							stmt.setDouble(idx++, gnssY == null || gnssY.Std < 0 ? 0.0 : gnssY.Std);
							stmt.setDouble(idx++, gnssX == null || gnssX.Std < 0 ? 0.0 : gnssX.Std);
							stmt.setDouble(idx++, gnssZ == null || gnssZ.Std < 0 ? 0.0 : gnssZ.Std);

							stmt.setDouble(idx++, gnssY == null ? 0.0 : gnssY.Correction);
							stmt.setDouble(idx++, gnssX == null ? 0.0 : gnssX.Correction);
							stmt.setDouble(idx++, gnssZ == null ? 0.0 : gnssZ.Correction);

							stmt.setDouble(idx++, gnssY == null ? 0.0 : gnssY.Redundancy);
							stmt.setDouble(idx++, gnssX == null ? 0.0 : gnssX.Redundancy);
							stmt.setDouble(idx++, gnssZ == null ? 0.0 : gnssZ.Redundancy);

							stmt.setDouble(idx++, gnssY == null ? 0.0 : gnssY.GrossError);
							stmt.setDouble(idx++, gnssX == null ? 0.0 : gnssX.GrossError);
							stmt.setDouble(idx++, gnssZ == null ? 0.0 : gnssZ.GrossError);

							stmt.setDouble(idx++, gnssY == null ? 0.0 : gnssY.MinimalDetectableBias);
							stmt.setDouble(idx++, gnssX == null ? 0.0 : gnssX.MinimalDetectableBias);
							stmt.setDouble(idx++, gnssZ == null ? 0.0 : gnssZ.MinimalDetectableBias);

							stmt.setDouble(idx++, gnssY == null ? 0.0 : gnssY.MaximumTolerableBias);
							stmt.setDouble(idx++, gnssX == null ? 0.0 : gnssX.MaximumTolerableBias);
							stmt.setDouble(idx++, gnssZ == null ? 0.0 : gnssZ.MaximumTolerableBias);

							stmt.setDouble(idx++, gnssY == null ? 0.0 : gnssY.InfluenceOnPointPosition);
							stmt.setDouble(idx++, gnssX == null ? 0.0 : gnssX.InfluenceOnPointPosition);
							stmt.setDouble(idx++, gnssZ == null ? 0.0 : gnssZ.InfluenceOnPointPosition);

							stmt.setDouble(idx++, gnssBaseline.InfluenceOnNetworkDistortion);

							stmt.setDouble(idx++, omega);

							stmt.setDouble(idx++, gnssBaseline.Pprio);
							stmt.setDouble(idx++, gnssBaseline.Ppost);

							stmt.setDouble(idx++, gnssBaseline.Tprio);
							stmt.setDouble(idx++, gnssBaseline.Tpost);

							stmt.setBoolean(idx++, gnssBaseline.Significant);
						}
						else
						{
							double value = observation.ValueAposteriori;

							if (observation.ObservationType == ObservationType.DIRECTION || observation.ObservationType == ObservationType.ZENITH_ANGLE)
							{
								if (observation is Direction)
								{
									double face = ((Direction)observation).Face == FaceType.ONE ? 0.0 : 1.0;
									value = MathExtension.MOD(value - face * Math.PI, 2.0 * Math.PI);
								}
								else if (observation is ZenithAngle)
								{
									double face = ((ZenithAngle)observation).Face == FaceType.ONE ? 0.0 : 1.0;
									value = Math.Abs(face * 2.0 * Math.PI - value);
								}
							}

							stmt.setInt(idx++, observation.Id);

							stmt.setDouble(idx++, value);
							stmt.setDouble(idx++, observation.StdApriori > 0 ? observation.StdApriori : 0.0);
							stmt.setDouble(idx++, observation.Std > 0 ? observation.Std : 0.0);

							stmt.setDouble(idx++, observation.Correction);
							stmt.setDouble(idx++, observation.Redundancy);

							stmt.setDouble(idx++, observation.GrossError);
							stmt.setDouble(idx++, observation.MinimalDetectableBias);
							stmt.setDouble(idx++, observation.MaximumTolerableBias);

							stmt.setDouble(idx++, observation.InfluenceOnPointPosition);
							stmt.setDouble(idx++, observation.InfluenceOnNetworkDistortion);

							stmt.setDouble(idx++, observation.Omega);

							stmt.setDouble(idx++, observation.Pprio);
							stmt.setDouble(idx++, observation.Ppost);

							stmt.setDouble(idx++, observation.Tprio);
							stmt.setDouble(idx++, observation.Tpost);

							stmt.setBoolean(idx++, observation.Significant);
						}
						stmt.addBatch();
						hasBatch = true;
					}
					if (hasBatch)
					{
						stmt.executeLargeBatch();
					}
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveAdditionalParameters() throws java.sql.SQLException
		private void saveAdditionalParameters()
		{
			bool hasBatch = false;
			string sql = "INSERT INTO \"AdditionalParameterAposteriori\" (" + "\"id\",\"value\",\"sigma\",\"confidence\",\"gross_error\",\"minimal_detectable_bias\",\"p_prio\",\"p_post\",\"t_prio\",\"t_post\",\"significant\"" + ") VALUES (?,?,?,?,?,?,?,?,?,?,?)";

			try
			{
				this.dataBase.AutoCommit = false;

				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

				foreach (KeyValuePair<int, AdditionalUnknownParameter> parameterItem in this.additionalParametersToBeEstimated.SetOfKeyValuePairs())
				{
					int paramId = parameterItem.Key;
					AdditionalUnknownParameter parameter = parameterItem.Value;

					if (parameter.ColInJacobiMatrix <= 0)
					{
						continue;
					}

					double value = parameter.Value;
					switch (parameter.ParameterType.innerEnumValue)
					{
					case ParameterType.InnerEnum.ORIENTATION:
					case ParameterType.InnerEnum.ROTATION_X:
					case ParameterType.InnerEnum.ROTATION_Y:
					case ParameterType.InnerEnum.ROTATION_Z:
						value = MathExtension.MOD(value, 2.0 * Math.PI);
						break;
					default:
						break;
					}

					int idx = 1;

					stmt.setInt(idx++, paramId);

					stmt.setDouble(idx++, value); // parameter.getValue()
					stmt.setDouble(idx++, parameter.Std > 0 ? parameter.Std : 0.0);
					stmt.setDouble(idx++, parameter.Confidence);

					stmt.setDouble(idx++, parameter.GrossError);
					stmt.setDouble(idx++, parameter.MinimalDetectableBias);

					stmt.setDouble(idx++, parameter.Pprio);
					stmt.setDouble(idx++, parameter.Ppost);

					stmt.setDouble(idx++, parameter.Tprio);
					stmt.setDouble(idx++, parameter.Tpost);

					stmt.setBoolean(idx++, parameter.Significant);

					stmt.addBatch();
					hasBatch = true;
				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveTestStatistic(org.applied_geodesy.adjustment.statistic.TestStatisticParameters testStatisticParameters) throws java.sql.SQLException
		private void saveTestStatistic(TestStatisticParameters testStatisticParameters)
		{
			bool hasBatch = false;

			string sql = "INSERT INTO \"TestStatistic\" (" + "\"d1\", \"d2\", \"probability_value\", \"power_of_test\", \"quantile\", \"non_centrality_parameter\", \"p_value\"" + ") VALUES (?,?,?,?,?,?,?)";

			try
			{
				this.dataBase.AutoCommit = false;

				TestStatisticParameterSet[] parameters = testStatisticParameters.TestStatisticParameterSets;

				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				for (int i = 0; i < parameters.Length; i++)
				{
					int idx = 1;
					TestStatisticParameterSet set = parameters[i];
					stmt.setDouble(idx++, set.NumeratorDof);
					stmt.setDouble(idx++, set.DenominatorDof);
					stmt.setDouble(idx++, set.ProbabilityValue);
					stmt.setDouble(idx++, set.PowerOfTest);
					// Speichere fuer den kritischen Wert des Tests k = max(1.0, k), 
					// da nur damit die Forderung nach sigam2aprio == sigma2apost eingehalten werden kann
					stmt.setDouble(idx++, Math.Max(1.0 + Constant.EPS, set.Quantile));
					stmt.setDouble(idx++, set.NoncentralityParameter);
					stmt.setDouble(idx++, set.LogarithmicProbabilityValue);

					stmt.addBatch();
					hasBatch = true;
				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}

			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveVarianceComponents(java.util.Map<org.applied_geodesy.adjustment.network.VarianceComponentType, org.applied_geodesy.adjustment.network.VarianceComponent> varianceComponents) throws java.sql.SQLException
		private void saveVarianceComponents(IDictionary<VarianceComponentType, VarianceComponent> varianceComponents)
		{
			bool hasBatch = false;

			string sql = "INSERT INTO \"VarianceComponent\" (" + "\"type\", \"redundancy\", \"omega\", \"sigma2apost\", \"number_of_observations\"" + ") VALUES (?,?,?,?,?)";

			try
			{
				this.dataBase.AutoCommit = false;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				foreach (VarianceComponent vc in varianceComponents.Values)
				{
					int idx = 1;
					stmt.setInt(idx++, vc.VarianceComponentType.getId());
					stmt.setDouble(idx++, vc.Redundancy);
					stmt.setDouble(idx++, vc.Omega);
					stmt.setDouble(idx++, vc.VarianceFactorAposteriori);
					stmt.setInt(idx++, vc.NumberOfObservations);

					stmt.addBatch();
					hasBatch = true;
				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void savePrincipalComponentAnalysis(org.applied_geodesy.adjustment.network.PrincipalComponent[] principalComponents) throws java.sql.SQLException
		private void savePrincipalComponentAnalysis(PrincipalComponent[] principalComponents)
		{
			if (principalComponents == null || principalComponents.Length == 0)
			{
				return;
			}

			bool hasBatch = false;

			string sql = "INSERT INTO \"PrincipalComponent\" (" + "\"index\", \"value\", \"ratio\"" + ") VALUES (?,?,?)";

			try
			{
				double traceCxx = this.networkAdjustment.TraceOfCovarianceMatrixOfPoints;
				this.dataBase.AutoCommit = false;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				foreach (PrincipalComponent principalComponent in principalComponents)
				{
					int idx = 1;

					int index = principalComponent.Index;
					double value = principalComponent.Value;
					double ratio = traceCxx > 0 ? value / traceCxx : 0;

					stmt.setInt(idx++, index);
					stmt.setDouble(idx++, value);
					stmt.setDouble(idx++, ratio);

					stmt.addBatch();
					hasBatch = true;
				}

				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveRankDefect(org.applied_geodesy.adjustment.network.RankDefect rankDefect) throws java.sql.SQLException
		private void saveRankDefect(RankDefect rankDefect)
		{
			string sql = "UPDATE \"RankDefect\" SET " + "\"ty\" = ?,\"tx\" = ?,\"tz\" = ?," + "\"ry\" = ?,\"rx\" = ?,\"rz\" = ?," + "\"sy\" = ?,\"sx\" = ?,\"sz\" = ?," + "\"my\" = ?,\"mx\" = ?,\"mz\" = ?," + "\"mxy\" = ?,\"mxyz\" = ? WHERE \"id\" = 1";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;

			stmt.setBoolean(idx++, rankDefect.estimateTranslationY());
			stmt.setBoolean(idx++, rankDefect.estimateTranslationX());
			stmt.setBoolean(idx++, rankDefect.estimateTranslationZ());

			stmt.setBoolean(idx++, rankDefect.estimateRotationY());
			stmt.setBoolean(idx++, rankDefect.estimateRotationX());
			stmt.setBoolean(idx++, rankDefect.estimateRotationZ());

			stmt.setBoolean(idx++, rankDefect.estimateShearY());
			stmt.setBoolean(idx++, rankDefect.estimateShearX());
			stmt.setBoolean(idx++, rankDefect.estimateShearZ());

			stmt.setBoolean(idx++, rankDefect.estimateScaleY());
			stmt.setBoolean(idx++, rankDefect.estimateScaleX());
			stmt.setBoolean(idx++, rankDefect.estimateScaleZ());

			stmt.setBoolean(idx++, rankDefect.estimateScaleXY());
			stmt.setBoolean(idx++, rankDefect.estimateScaleXYZ());

			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveCongruenceAnalysisPointPair() throws java.sql.SQLException
		private void saveCongruenceAnalysisPointPair()
		{
			bool hasBatch = false;

			string sql = "INSERT INTO \"CongruenceAnalysisPointPairAposteriori\" (" + "\"id\",\"y\",\"x\",\"z\", " + "\"sigma_y\",\"sigma_x\",\"sigma_z\", " + "\"confidence_major_axis\",\"confidence_middle_axis\",\"confidence_minor_axis\", " + "\"confidence_alpha\",\"confidence_beta\",\"confidence_gamma\", " + "\"confidence_major_axis_2d\",\"confidence_minor_axis_2d\",\"confidence_alpha_2d\", " + "\"gross_error_y\",\"gross_error_x\",\"gross_error_z\", " + "\"minimal_detectable_bias_y\",\"minimal_detectable_bias_x\",\"minimal_detectable_bias_z\", " + "\"p_prio\",\"p_post\", " + "\"t_prio\",\"t_post\",\"significant\"" + ") VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

			try
			{
				this.dataBase.AutoCommit = false;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				bool[] analysablePointPairFlag = new bool[] {true, false};
				foreach (CongruenceAnalysisGroup congruenceAnalysisGroup in this.congruenceAnalysisGroups)
				{
					foreach (bool analysablePointPair in analysablePointPairFlag)
					{
						int length = congruenceAnalysisGroup.size(analysablePointPair);
						for (int i = 0; i < length; i++)
						{
							CongruenceAnalysisPointPair pointPair = congruenceAnalysisGroup.get(i, analysablePointPair);
							int dimension = pointPair.Dimension;
							int idx = 1;

							stmt.setInt(idx++, pointPair.Id);

							stmt.setDouble(idx++, dimension != 1 ? pointPair.DeltaY : 0.0);
							stmt.setDouble(idx++, dimension != 1 ? pointPair.DeltaX : 0.0);
							stmt.setDouble(idx++, dimension != 2 ? pointPair.DeltaZ : 0.0);

							stmt.setDouble(idx++, dimension != 1 && pointPair.StdY > 0 ? pointPair.StdY : 0.0);
							stmt.setDouble(idx++, dimension != 1 && pointPair.StdX > 0 ? pointPair.StdX : 0.0);
							stmt.setDouble(idx++, dimension != 2 && pointPair.StdZ > 0 ? pointPair.StdZ : 0.0);

							stmt.setDouble(idx++, pointPair.getConfidenceAxis(0));
							stmt.setDouble(idx++, dimension == 3 ? pointPair.getConfidenceAxis(1) : 0.0);
							stmt.setDouble(idx++, dimension != 1 ? pointPair.getConfidenceAxis(dimension - 1) : 0.0);

							stmt.setDouble(idx++, dimension > 2 ? pointPair.getConfidenceAngle(0) : 0.0);
							stmt.setDouble(idx++, dimension > 2 ? pointPair.getConfidenceAngle(1) : 0.0);
							stmt.setDouble(idx++, dimension > 1 ? pointPair.getConfidenceAngle(2) : 0.0);

							stmt.setDouble(idx++, pointPair.getConfidenceAxis2D(0));
							stmt.setDouble(idx++, dimension != 1 ? pointPair.getConfidenceAxis2D(1) : 0.0);
							stmt.setDouble(idx++, dimension != 1 ? pointPair.ConfidenceAngle2D : 0.0);

							stmt.setDouble(idx++, dimension != 1 ? pointPair.GrossErrorY : 0.0);
							stmt.setDouble(idx++, dimension != 1 ? pointPair.GrossErrorX : 0.0);
							stmt.setDouble(idx++, dimension != 2 ? pointPair.GrossErrorZ : 0.0);

							stmt.setDouble(idx++, dimension != 1 ? pointPair.MinimalDetectableBiasY : 0.0);
							stmt.setDouble(idx++, dimension != 1 ? pointPair.MinimalDetectableBiasX : 0.0);
							stmt.setDouble(idx++, dimension != 2 ? pointPair.MinimalDetectableBiasZ : 0.0);

							stmt.setDouble(idx++, pointPair.Pprio);
							stmt.setDouble(idx++, pointPair.Ppost);

							stmt.setDouble(idx++, pointPair.Tprio);
							stmt.setDouble(idx++, pointPair.Tpost);

							stmt.setBoolean(idx++, pointPair.Significant);

							stmt.addBatch();
							hasBatch = true;
						}
					}
				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveStrainParameters() throws java.sql.SQLException
		private void saveStrainParameters()
		{
			bool hasBatch = false;

			string sql = "INSERT INTO \"CongruenceAnalysisStrainParameterAposteriori\" (" + "\"group_id\",\"type\"," + "\"value\",\"sigma\",\"confidence\"," + "\"gross_error\",\"minimal_detectable_bias\"," + "\"p_prio\",\"p_post\"," + "\"t_prio\",\"t_post\",\"significant\"" + ") VALUES (?,?,?,?,?,?,?,?,?,?,?,?)";

			try
			{
				this.dataBase.AutoCommit = false;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);

				foreach (CongruenceAnalysisGroup congruenceAnalysisGroup in this.congruenceAnalysisGroups)
				{
					int dimension = congruenceAnalysisGroup.Dimension;
					StrainAnalysisEquations strainAnalysisEquations = congruenceAnalysisGroup.StrainAnalysisEquations;

					int nou = strainAnalysisEquations.numberOfParameters() - strainAnalysisEquations.numberOfExpandedParameters();
					int nor = strainAnalysisEquations.numberOfRestrictions();
					int not = congruenceAnalysisGroup.size(true);

					if (this.freeNetwork && this.congruenceAnalysis && strainAnalysisEquations.hasUnconstraintParameters() && not * dimension + nor >= nou)
					{
						IDictionary<ParameterType, RestrictionType> parameterTyps = new Dictionary<ParameterType, RestrictionType>(12);
						if (dimension != 1)
						{
							parameterTyps[ParameterType.STRAIN_TRANSLATION_X] = RestrictionType.FIXED_TRANSLATION_X;
							parameterTyps[ParameterType.STRAIN_TRANSLATION_Y] = RestrictionType.FIXED_TRANSLATION_Y;
							parameterTyps[ParameterType.STRAIN_ROTATION_Z] = RestrictionType.FIXED_ROTATION_Z;
							parameterTyps[ParameterType.STRAIN_SCALE_X] = RestrictionType.FIXED_SCALE_X;
							parameterTyps[ParameterType.STRAIN_SCALE_Y] = RestrictionType.FIXED_SCALE_Y;
							parameterTyps[ParameterType.STRAIN_SHEAR_Z] = RestrictionType.FIXED_SHEAR_Z;
						}
						if (dimension != 2)
						{
							parameterTyps[ParameterType.STRAIN_TRANSLATION_Z] = RestrictionType.FIXED_TRANSLATION_Z;
							parameterTyps[ParameterType.STRAIN_SCALE_Z] = RestrictionType.FIXED_SCALE_Z;
						}
						if (dimension == 3)
						{
							parameterTyps[ParameterType.STRAIN_ROTATION_X] = RestrictionType.FIXED_ROTATION_X;
							parameterTyps[ParameterType.STRAIN_ROTATION_Y] = RestrictionType.FIXED_ROTATION_Y;
							parameterTyps[ParameterType.STRAIN_SHEAR_X] = RestrictionType.FIXED_SHEAR_X;
							parameterTyps[ParameterType.STRAIN_SHEAR_Y] = RestrictionType.FIXED_SHEAR_Y;
						}

						for (int i = 0; i < strainAnalysisEquations.numberOfParameters(); i++)
						{
							StrainParameter strainParameter = strainAnalysisEquations.get(i);
							ParameterType type = strainParameter.ParameterType;
							if (parameterTyps.ContainsKey(type) && !strainAnalysisEquations.isRestricted(parameterTyps[type]) && strainParameter.Std >= 0)
							{
								double value = strainParameter.Value;
								switch (strainParameter.ParameterType.innerEnumValue)
								{
								case ParameterType.InnerEnum.STRAIN_ROTATION_X:
								case ParameterType.InnerEnum.STRAIN_ROTATION_Y:
								case ParameterType.InnerEnum.STRAIN_ROTATION_Z:
								case ParameterType.InnerEnum.STRAIN_SHEAR_X:
								case ParameterType.InnerEnum.STRAIN_SHEAR_Y:
								case ParameterType.InnerEnum.STRAIN_SHEAR_Z:
									value = MathExtension.MOD(value, 2.0 * Math.PI);
									break;
								default:
									break;
								}

								int idx = 1;
								stmt.setInt(idx++, congruenceAnalysisGroup.Id);
								stmt.setInt(idx++, type.getId());

								stmt.setDouble(idx++, value); // strainParameter.getValue()
								stmt.setDouble(idx++, strainParameter.Std > 0 ? strainParameter.Std : 0.0);
								stmt.setDouble(idx++, strainParameter.Confidence);

								stmt.setDouble(idx++, strainParameter.GrossError);
								stmt.setDouble(idx++, strainParameter.MinimalDetectableBias);

								stmt.setDouble(idx++, strainParameter.Pprio);
								stmt.setDouble(idx++, strainParameter.Ppost);

								stmt.setDouble(idx++, strainParameter.Tprio);
								stmt.setDouble(idx++, strainParameter.Tpost);

								stmt.setBoolean(idx++, strainParameter.Significant);

								stmt.addBatch();
								hasBatch = true;
							}
						}
					}
				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

		/// <summary>
		/// Average observations per group </summary>
		/// <param name="saveAvarageValues"> </param>
		/// <returns> observations </returns>
		/// <exception cref="SQLException"> </exception>
		/// <exception cref="IllegalProjectionPropertyException"> </exception>
		/// <exception cref="DatabaseVersionMismatchException">  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.util.List<org.applied_geodesy.adjustment.network.observation.Observation> averageDetermination(boolean saveAvarageValues) throws SQLException, IllegalProjectionPropertyException, org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException
		public virtual IList<Observation> averageDetermination(bool saveAvarageValues)
		{
			// erzeuge ein Objekt zur Netzausgleichung
			// Hierdurch werden alle Punkte, Beobachtungen und Zusatzparameter 
			// geladen und inizialisiert.
			IList<Observation> observations = new List<Observation>();
			NetworkAdjustment networkAdjustment = this.NetworkAdjustment;
			if (networkAdjustment == null)
			{
				return observations;
			}

			// Mittelwertbildung (gruppenweise)
			foreach (ObservationGroup observationGroup in this.completeObservationGroups)
			{
				double threshold = 1.0;

				if (observationGroup is GNSSBaseline1DGroup)
				{
					threshold = this.getAverageThreshold(ObservationType.GNSS1D);
				}
				else if (observationGroup is GNSSBaseline2DGroup)
				{
					threshold = this.getAverageThreshold(ObservationType.GNSS2D);
				}
				else if (observationGroup is GNSSBaseline3DGroup)
				{
					threshold = this.getAverageThreshold(ObservationType.GNSS3D);
				}
				else if (observationGroup is DirectionGroup)
				{
					threshold = this.getAverageThreshold(ObservationType.DIRECTION);
				}
				else if (observationGroup is DeltaZGroup)
				{
					threshold = this.getAverageThreshold(ObservationType.LEVELING);
				}
				else if (observationGroup is HorizontalDistanceGroup)
				{
					threshold = this.getAverageThreshold(ObservationType.HORIZONTAL_DISTANCE);
				}
				else if (observationGroup is SlopeDistanceGroup)
				{
					threshold = this.getAverageThreshold(ObservationType.SLOPE_DISTANCE);
				}
				else if (observationGroup is ZenithAngleGroup)
				{
					threshold = this.getAverageThreshold(ObservationType.ZENITH_ANGLE);
				}
				else
				{
					Console.Error.WriteLine(this.GetType().Name + " Fehler, unbekannte Beobachtungsgruppe! " + observationGroup);
					continue;
				}
				observationGroup.averageDetermination(threshold);
				ISet<Observation> excludedObservations = observationGroup.ExcludedObservationsDuringAvaraging;
				if (excludedObservations != null && excludedObservations.Count > 0)
				{
					((List<Observation>)observations).AddRange(excludedObservations);
				}
			}

			// Speichere nur, wenn der Nutzer dies explizit wollte oder es keinen Konflikt gab mit den Grenzwerten
			if (saveAvarageValues || observations.Count == 0)
			{
				foreach (Observation observation in observations)
				{
					this.deleteObservation(observation);
				}

				// Speichere GNSS-IDs
				ISet<int> gnssIDs = new HashSet<int>();

				// Speichere Ergebnis
				PreparedStatement statement = null;

				// SQL-Statements
				string sqlFormatDelObs = "DELETE FROM \"ObservationApriori\" " + "WHERE \"group_id\" = ? AND \"enable\" = TRUE " + "AND (SELECT \"enable\" FROM \"ObservationGroup\" WHERE \"id\" = \"group_id\") = TRUE " + "AND (SELECT \"enable\" FROM \"PointApriori\" WHERE \"name\" = \"start_point_name\") = TRUE " + "AND (SELECT \"enable\" FROM \"PointApriori\" WHERE \"name\" = \"end_point_name\") = TRUE " + "AND (SELECT \"enable\" FROM \"PointGroup\" WHERE \"id\" = (SELECT \"group_id\" FROM \"PointApriori\" WHERE \"name\" = \"start_point_name\")) = TRUE " + "AND (SELECT \"enable\" FROM \"PointGroup\" WHERE \"id\" = (SELECT \"group_id\" FROM \"PointApriori\" WHERE \"name\" = \"end_point_name\")) = TRUE";

				string sqlFormatDelGNSSObs = "DELETE FROM \"GNSSObservationApriori\" " + "WHERE \"group_id\" = ? AND \"enable\" = TRUE " + "AND (SELECT \"enable\" FROM \"ObservationGroup\" WHERE \"id\" = \"group_id\") = TRUE " + "AND (SELECT \"enable\" FROM \"PointApriori\" WHERE \"name\" = \"start_point_name\") = TRUE " + "AND (SELECT \"enable\" FROM \"PointApriori\" WHERE \"name\" = \"end_point_name\") = TRUE " + "AND (SELECT \"enable\" FROM \"PointGroup\" WHERE \"id\" = (SELECT \"group_id\" FROM \"PointApriori\" WHERE \"name\" = \"start_point_name\")) = TRUE " + "AND (SELECT \"enable\" FROM \"PointGroup\" WHERE \"id\" = (SELECT \"group_id\" FROM \"PointApriori\" WHERE \"name\" = \"end_point_name\")) = TRUE";

				string sqlFormatInsObs = "INSERT INTO \"ObservationApriori\"     (\"group_id\", \"start_point_name\", \"end_point_name\", \"instrument_height\", \"reflector_height\", \"value_0\", \"sigma_0\", \"distance_0\", \"enable\") VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
				string sqlFormatInsGNSSObs = "INSERT INTO \"GNSSObservationApriori\" (\"group_id\", \"start_point_name\", \"end_point_name\", \"y0\", \"x0\", \"z0\", \"sigma_y0\", \"sigma_x0\", \"sigma_z0\", \"enable\") VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

				foreach (ObservationGroup observationGroup in this.completeObservationGroups)
				{
					// Loesche alle Beobachtungen der Gruppe, die aktiv waren/sind
					int groupId = observationGroup.Id;
					bool isGNSS = observationGroup is GNSSBaseline1DGroup || observationGroup is GNSSBaseline2DGroup || observationGroup is GNSSBaseline3DGroup;
					if (!isGNSS)
					{
						statement = this.dataBase.getPreparedStatement(sqlFormatDelObs);
					}
					else
					{
						statement = this.dataBase.getPreparedStatement(sqlFormatDelGNSSObs);
					}

					statement.setInt(1, groupId);
					statement.execute();

					// Speichere die gemittelten Daten
					for (int i = 0; i < observationGroup.size(); i++)
					{
						int idx = 1;
						Observation observation = observationGroup.get(i);
						if (!isGNSS)
						{
							statement = this.dataBase.getPreparedStatement(sqlFormatInsObs);

							statement.setInt(idx++, groupId);
							statement.setString(idx++, observation.StartPoint.Name);
							statement.setString(idx++, observation.EndPoint.Name);

							statement.setDouble(idx++, observation.StartPointHeight);
							statement.setDouble(idx++, observation.EndPointHeight);

							statement.setDouble(idx++, observation.ValueApriori);
							statement.setDouble(idx++, 0.0);
							statement.setDouble(idx++, observation.DistanceForUncertaintyModel);

							statement.setBoolean(idx++, true);

							statement.execute();
						}
						else
						{
							GNSSBaseline gnssBaseline = (GNSSBaseline)observation;
							if (gnssIDs.Contains(gnssBaseline.Id))
							{
								continue;
							}

							gnssIDs.Add(gnssBaseline.Id);

							GNSSBaseline gnssY = gnssBaseline.getBaselineComponent(ComponentType.Y);
							GNSSBaseline gnssX = gnssBaseline.getBaselineComponent(ComponentType.X);
							GNSSBaseline gnssZ = gnssBaseline.getBaselineComponent(ComponentType.Z);

							statement = this.dataBase.getPreparedStatement(sqlFormatInsGNSSObs);

							statement.setInt(idx++, groupId);
							statement.setString(idx++, gnssBaseline.StartPoint.Name);
							statement.setString(idx++, gnssBaseline.EndPoint.Name);

							statement.setDouble(idx++, gnssY == null ? 0.0 : gnssY.ValueApriori);
							statement.setDouble(idx++, gnssX == null ? 0.0 : gnssX.ValueApriori);
							statement.setDouble(idx++, gnssZ == null ? 0.0 : gnssZ.ValueApriori);

							statement.setDouble(idx++, 0.0);
							statement.setDouble(idx++, 0.0);
							statement.setDouble(idx++, 0.0);

							statement.setBoolean(idx++, true);

							statement.execute();
						}
					}
				}
			}
			networkAdjustment.clearMatrices();
			networkAdjustment = null;
			return observations;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void deleteObservation(org.applied_geodesy.adjustment.network.observation.Observation observation) throws java.sql.SQLException
		private void deleteObservation(Observation observation)
		{
			bool isGNSS = observation.ObservationType == ObservationType.GNSS1D || observation.ObservationType == ObservationType.GNSS2D || observation.ObservationType == ObservationType.GNSS3D;
			string sql = "DELETE FROM " + (isGNSS ? "\"GNSSObservationApriori\"" : "\"ObservationApriori\"") + " WHERE \"id\" = ? LIMIT 1";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setInt(idx++, observation.Id);
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private double getAverageThreshold(org.applied_geodesy.adjustment.network.ObservationType type) throws java.sql.SQLException
		private double getAverageThreshold(ObservationType type)
		{
			string sql = "SELECT \"value\" " + "FROM \"AverageThreshold\" " + "WHERE \"type\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, type.getId());
			ResultSet rs = stmt.executeQuery();

			double value = 0;
			if (rs.next())
			{
				value = rs.getDouble("value");
			}

			return value > 0 ? value : DefaultAverageThreshold.getThreshold(type);
		}
	}

}