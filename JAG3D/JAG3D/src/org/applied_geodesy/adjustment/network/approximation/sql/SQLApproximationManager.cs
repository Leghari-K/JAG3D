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

namespace org.applied_geodesy.adjustment.network.approximation.sql
{

	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using AutomatedApproximationAdjustment = org.applied_geodesy.adjustment.network.approximation.AutomatedApproximationAdjustment;
	using PointBundle = org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle;
	using ForwardIntersectionEntry = org.applied_geodesy.adjustment.network.approximation.bundle.intersection.ForwardIntersectionEntry;
	using ForwardIntersectionSet = org.applied_geodesy.adjustment.network.approximation.bundle.intersection.ForwardIntersectionSet;
	using ClassicGeodeticComputation = org.applied_geodesy.adjustment.network.approximation.bundle.point.ClassicGeodeticComputation;
	using Point = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point;
	using Point1D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point1D;
	using Point2D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using DataBase = org.applied_geodesy.util.sql.DataBase;

	public class SQLApproximationManager : PropertyChangeListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			change = new PropertyChangeSupport(this);
		}

		private PropertyChangeSupport change;
		private readonly DataBase dataBase;
		private bool estimateDatumPoints = false;
//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool freeNetwork = true, interrupt_Conflict = false;
		private ISet<string> underdeterminedPointNames1D = new HashSet<string>();
		private ISet<string> underdeterminedPointNames2D = new HashSet<string>();
		private ISet<string> completestationNames = new HashSet<string>();
		private ISet<string> outlier1d = new HashSet<string>();
		private ISet<string> outlier2d = new HashSet<string>();
		private IDictionary<string, ISet<string>> directionLinks = new LinkedHashMap<string, ISet<string>>();
		private PointBundle targetSystem1d, targetSystem2d;
		private int subSystemsCounter1d = 0, subSystemsCounter2d = 0;
		private EstimationStateType estimationStatus1D = EstimationStateType.ERROR_FREE_ESTIMATION;
		private EstimationStateType estimationStatus2D = EstimationStateType.ERROR_FREE_ESTIMATION;
		private AutomatedApproximationAdjustment approximationAdjustment = null;

		public SQLApproximationManager(DataBase dataBase)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (dataBase == null || !dataBase.Open)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, database must be open! " + dataBase);
			}
			this.dataBase = dataBase;
		}

		public virtual void clearAll()
		{
			this.subSystemsCounter1d = this.subSystemsCounter2d = 0;
			this.underdeterminedPointNames1D.Clear();
			this.underdeterminedPointNames2D.Clear();
			this.completestationNames.Clear();
			this.outlier1d.Clear();
			this.outlier2d.Clear();
			this.directionLinks.Clear();
			this.targetSystem1d = null;
			this.targetSystem2d = null;
			this.approximationAdjustment = null;
		}

		public virtual int SubSystemsCounter1D
		{
			get
			{
				return this.subSystemsCounter1d;
			}
		}

		public virtual int SubSystemsCounter2D
		{
			get
			{
				return this.subSystemsCounter2d;
			}
		}

		public virtual ISet<string> Outliers1D
		{
			get
			{
				return this.outlier1d;
			}
		}

		public virtual ISet<string> Outliers2D
		{
			get
			{
				return this.outlier2d;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void adjustApproximationValues(double threshold) throws java.sql.SQLException
		public virtual void adjustApproximationValues(double threshold)
		{
			if (this.dataBase == null)
			{
				return;
			}

			this.clearAll();

			// Suche alle Fest- und Anschlusspunkte,
			// da diese bereits als Zielsystem genutzt
			// werden - eine Berechnung dieser Punkte
			// entfaellt!
			this.interrupt_Conflict = false;
			this.freeNetwork = false;
			this.initTargetSystems(this.freeNetwork);
			this.freeNetwork = this.targetSystem1d == null && this.targetSystem2d == null;
			if (this.freeNetwork && !this.estimateDatumPoints)
			{
				this.initTargetSystems(true);
			}

			// Ermittle alle Punktnummern, die Standpunkte sind
			this.completestationNames = this.CompletestationNames;

			// Bestimme alle Punktnummern im Netz, um spaeter zu kontrollieren, welche noch nicht berechnet wurden
			this.initNonEstimatedPointNames();

			// Initialisiere Lageausgleichung
			IList<Point> points2d = this.getStationPointsWithSubSystems(2);
			if (points2d != null)
			{ // Hier *nicht* die Laenge der Liste pruefen, da sonst underdeterminedPointNames2D nicht um Festpunkte reduziert wird!
				int targetCounter = -1;
				bool converge = false;
				IList<PointBundle> notTransFormedBundles = new List<PointBundle>(1);
				do
				{
					converge = false;
					this.approximationAdjustment = new AutomatedApproximationAdjustment(this.targetSystem2d, points2d);
					this.approximationAdjustment.addPropertyChangeListener(this);

					if (this.interrupt_Conflict)
					{
						this.estimationStatus2D = EstimationStateType.INTERRUPT;
						this.approximationAdjustment.interrupt();
						this.interrupt_Conflict = false;
						return;
					}

					if (notTransFormedBundles != null && notTransFormedBundles.Count > 0)
					{
						this.approximationAdjustment.addSystems(notTransFormedBundles);
					}
					this.approximationAdjustment.EstimateDatumPoints = this.estimateDatumPoints;
					this.approximationAdjustment.FreeNetwork = this.freeNetwork;
					this.approximationAdjustment.Threshold = threshold;
					this.estimationStatus2D = this.approximationAdjustment.estimateApproximatedValues();
					if (this.approximationAdjustment.TargetSystem != null)
					{
						this.targetSystem2d = this.approximationAdjustment.TargetSystem;
					}

					if (this.approximationAdjustment.Systems.Count > 1)
					{
						notTransFormedBundles = this.approximationAdjustment.Systems;
					}
					else
					{
						notTransFormedBundles = new List<PointBundle>(1);
					}

					if (this.targetSystem2d != null)
					{
						for (int i = 0; i < this.targetSystem2d.size(); i++)
						{
							Point p = this.targetSystem2d.get(i);
							if (this.underdeterminedPointNames2D.Contains(p.Name))
							{
								this.underdeterminedPointNames2D.remove(p.Name);
							}
						}

						ISet<string> estimatedPointIds = new HashSet<string>(this.underdeterminedPointNames2D.Count);
						foreach (string newPointId in this.underdeterminedPointNames2D)
						{
							Point2D p = this.restockBundle(newPointId, this.targetSystem2d);

							foreach (PointBundle localBundle in notTransFormedBundles)
							{
								this.restockBundle(newPointId, localBundle);
							}

							if (p != null)
							{
								estimatedPointIds.Add(newPointId);
							}
						}

						foreach (string estimatedPointId in estimatedPointIds)
						{
							this.underdeterminedPointNames2D.remove(estimatedPointId);
						}

						this.savePoints(this.targetSystem2d);
					}
					if (this.targetSystem2d != null && this.approximationAdjustment.Systems.Count > 1 && this.targetSystem2d.size() != targetCounter)
					{
						targetCounter = this.targetSystem2d.size();
						points2d = this.getStationPointsWithSubSystems(2);
						converge = true;
					}

					this.approximationAdjustment.removePropertyChangeListener(this);
				} while (converge);

				if (this.approximationAdjustment != null)
				{
					this.subSystemsCounter2d = this.approximationAdjustment.SubSystemsCounter;
					this.outlier2d = this.approximationAdjustment.Outliers;
				}

				points2d = null;
			}

			IList<Point> points1d = this.getStationPointsWithSubSystems(1);
			if (points1d != null)
			{ // Hier *nicht* die Laenge der Liste pruefen, da sonst underdeterminedPointNames1D nicht um Festpunkte reduziert wird!
				this.approximationAdjustment = new AutomatedApproximationAdjustment(this.targetSystem1d, points1d);
				this.approximationAdjustment.addPropertyChangeListener(this);

				if (this.interrupt_Conflict)
				{
					this.estimationStatus1D = EstimationStateType.INTERRUPT;
					this.approximationAdjustment.interrupt();
					this.interrupt_Conflict = false;
					return;
				}

				this.approximationAdjustment.EstimateDatumPoints = this.estimateDatumPoints;
				this.approximationAdjustment.FreeNetwork = this.freeNetwork;
				this.approximationAdjustment.Threshold = threshold;
				this.estimationStatus1D = this.approximationAdjustment.estimateApproximatedValues();
				this.targetSystem1d = this.approximationAdjustment.TargetSystem;
				if (this.targetSystem1d != null)
				{
					for (int i = 0; i < this.targetSystem1d.size(); i++)
					{
						Point p = this.targetSystem1d.get(i);
						if (this.underdeterminedPointNames1D.Contains(p.Name))
						{
							this.underdeterminedPointNames1D.remove(p.Name);
						}
					}
					this.savePoints(this.targetSystem1d);
				}

				if (this.approximationAdjustment != null)
				{
					this.subSystemsCounter1d = this.approximationAdjustment.SubSystemsCounter;
					this.outlier1d = this.approximationAdjustment.Outliers;
				}
				this.approximationAdjustment.removePropertyChangeListener(this);
				points1d = null;
			}
		}

		public virtual void interrupt()
		{
			this.interrupt_Conflict = true;
			if (this.approximationAdjustment != null)
			{
				this.approximationAdjustment.interrupt();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D restockBundle(String newPointId, org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle bundle) throws java.sql.SQLException
		private Point2D restockBundle(string newPointId, PointBundle bundle)
		{
			if (bundle.contains(newPointId))
			{
				return (Point2D)bundle.get(newPointId);
			}

			Point2D p = this.getForwardIntersectionPoint(newPointId, bundle);

			if (p == null)
			{
				p = this.getArcIntersectionPoint(newPointId, bundle);
			}

			if (p == null)
			{
				p = this.getBackwardIntersectionPoint(newPointId, bundle);
			}

			if (p != null)
			{
				bundle.addPoint(p);
			}

			return p;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.Set<String> getCompletestationNames() throws java.sql.SQLException
		private ISet<string> CompletestationNames
		{
			get
			{
				ISet<string> completestationNames = new HashSet<string>();
				IList<string> stationNames = this.getPointNames(1, true);
				foreach (string pointName in stationNames)
				{
					completestationNames.Add(pointName);
				}
				stationNames = this.getPointNames(2, true);
				foreach (string pointName in stationNames)
				{
					completestationNames.Add(pointName);
				}
				stationNames = this.getPointNames(3, true);
				foreach (string pointName in stationNames)
				{
					completestationNames.Add(pointName);
				}
				return completestationNames;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void initNonEstimatedPointNames() throws java.sql.SQLException
		private void initNonEstimatedPointNames()
		{
			IList<string> stationNames = this.getPointNames(1, false);
			foreach (string pointName in stationNames)
			{
				this.underdeterminedPointNames1D.Add(pointName);
			}
			stationNames = this.getPointNames(2, false);
			foreach (string pointName in stationNames)
			{
				this.underdeterminedPointNames2D.Add(pointName);
			}
			stationNames = this.getPointNames(3, false);
			foreach (string pointName in stationNames)
			{
				this.underdeterminedPointNames1D.Add(pointName);
				this.underdeterminedPointNames2D.Add(pointName);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<org.applied_geodesy.adjustment.network.approximation.bundle.point.Point> getStationPointsWithSubSystems(int dim) throws java.sql.SQLException
		private IList<Point> getStationPointsWithSubSystems(int dim)
		{
			// Speichere alle Punktnummern von Standpunkten
			IList<string> stationNames;

			IList<Point> pointList = new List<Point>();
			IDictionary<string, Point> pointMap = new LinkedHashMap<string, Point>();


			if (dim != 2)
			{
				// Ermittle die Standpunkte und ihre zugehoerigen Beobachtungen fuer Hoehe
				stationNames = this.getPointNames(1, true);
				foreach (string pointName in stationNames)
				{
					Point1D point = new Point1D(pointName, 0.0);
					pointList.Add(point);
					pointMap[pointName] = point;

					this.addObservation(point);
				}

				stationNames = this.getPointNames(3, true);
				foreach (string pointName in stationNames)
				{
					Point1D point1d = new Point1D(pointName, 0.0);
					pointList.Add(point1d);
					pointMap[pointName] = point1d;

					this.addObservation(point1d);
				}
			}
			else if (dim != 1)
			{
				// Ermittle die Standpunkte und ihre zugehoerigen Beobachtungen fuer Lage
				stationNames = this.getPointNames(2, true);
				foreach (string pointName in stationNames)
				{
					Point2D point = new Point2D(pointName, 0.0, 0.0);
					pointList.Add(point);
					pointMap[pointName] = point;

					this.addObservation(point);
				}

				stationNames = this.getPointNames(3, true);
				foreach (string pointName in stationNames)
				{
					Point2D point2d = new Point2D(pointName, 0.0, 0.0);
					pointList.Add(point2d);
					pointMap[pointName] = point2d;

					this.addObservation(point2d);
				}

				// Fuege Systeme hinzu, die per Vorwaertsschnitt entstehen
				foreach (KeyValuePair<string, ISet<string>> directionLink in this.directionLinks.SetOfKeyValuePairs())
				{
					string fixPointIdA = directionLink.Key;
					Point fixPointA = pointMap[fixPointIdA];
					foreach (string fixPointIdB in directionLink.Value)
					{
						if (this.directionLinks.ContainsKey(fixPointIdB) && this.directionLinks[fixPointIdB].Contains(fixPointIdA))
						{
							// Suche nach Vorwaertsschnitten
							//System.out.println(fixPointIdA+"  "+fixPointIdB);
							Point fixPointB = pointMap[fixPointIdB];
							this.addForwardIntersectionCombinations(fixPointA, fixPointB);
						}
					}
					// Entferne die abgearbeiteten Verknüpfungen um Rückverlinkungen zu unterbinden.
					directionLink.Value.clear();
					fixPointA.joinBundles();
				}
				this.directionLinks.Clear();
			}

			pointMap.Clear();
			return pointList;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addObservation(org.applied_geodesy.adjustment.network.approximation.bundle.point.Point point) throws java.sql.SQLException
		private void addObservation(Point point)
		{
			int dim = -1;
			if (point is Point1D)
			{
				dim = 1;
			}
			else if (point is Point2D)
			{
				dim = 2;
			}
			else
			{
				return;
			}

			string pointName = point.Name;
			IList<TerrestrialObservationRow> deltaH = null, distances2D = null, distances3D = null, zenithangle = null;

			if (dim != 2)
			{
				deltaH = this.getTerrestrialObservationsIgnoreGroups(pointName, ObservationType.LEVELING);
			}
			else if (dim != 1)
			{
				distances2D = this.getTerrestrialObservationsIgnoreGroups(pointName, ObservationType.HORIZONTAL_DISTANCE);
			}

			distances3D = this.getTerrestrialObservationsIgnoreGroups(pointName, ObservationType.SLOPE_DISTANCE);
			zenithangle = this.getTerrestrialObservationsIgnoreGroups(pointName, ObservationType.ZENITH_ANGLE);

			// Ergaenze Pseudobeobachtungen Strecke2D zw. Festpunkten
			if (dim != 1 && this.targetSystem2d != null && this.targetSystem2d.get(pointName) != null)
			{
				Point startPoint = this.targetSystem2d.get(pointName);
				for (int i = 0; i < this.targetSystem2d.size(); i++)
				{
					Point endPoint = this.targetSystem2d.get(i);

					// Standpunkt == Zielpunkt oder Dimensionsfehler, dann abbrechen
					if (endPoint.Name.Equals(pointName) || endPoint.Dimension != 1)
					{
						continue;
					}

					// Pruefe, ob es bereits eine Streckenmessung zwischen den Punkten gibt
					bool hasObservedDistance = false;
					foreach (TerrestrialObservationRow dist2d in distances2D)
					{
						if (dist2d.StartPointName.Equals(startPoint.Name) && dist2d.EndPointName.Equals(endPoint.Name) || dist2d.StartPointName.Equals(endPoint.Name) && dist2d.EndPointName.Equals(startPoint.Name))
						{
							hasObservedDistance = true;
							break;
						}
					}

					// Wenn Streckenmessung noch nicht vorhanden, dann speichern
					if (!hasObservedDistance)
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = startPoint.Name;
						obs.EndPointName = endPoint.Name;
						obs.ValueApriori = startPoint.getDistance2D(endPoint);
						distances2D.Add(obs);
					}
				}
			}

			// Ergaenze Pseudobeobachtungen deltaH zw. Festpunkten
			else if (dim != 2 && this.targetSystem1d != null && this.targetSystem1d.get(pointName) != null)
			{
				Point startPoint = this.targetSystem1d.get(pointName);
				for (int i = 0; i < this.targetSystem1d.size(); i++)
				{
					Point endPoint = this.targetSystem1d.get(i);

					// Standpunkt == Zielpunkt oder Dimensionsfehler, dann abbrechen
					if (endPoint.Name.Equals(pointName) || endPoint.Dimension != 1)
					{
						continue;
					}

					// Pruefe, ob es bereits ein Hoehenunterschied zwischen den Punkten gibt
					bool hasObservedDeltaH = false;
					foreach (TerrestrialObservationRow dH in deltaH)
					{
						if (dH.StartPointName.Equals(startPoint.Name) && dH.EndPointName.Equals(endPoint.Name) || dH.StartPointName.Equals(endPoint.Name) && dH.EndPointName.Equals(startPoint.Name))
						{
							hasObservedDeltaH = true;
							break;
						}
					}

					// Wenn Hoehenunterschied noch nicht vorhanden, dann speichern
					if (!hasObservedDeltaH)
					{
						TerrestrialObservationRow obs = new TerrestrialObservationRow();
						obs.StartPointName = startPoint.Name;
						obs.EndPointName = endPoint.Name;
						obs.ValueApriori = endPoint.Z - startPoint.Z;
						deltaH.Add(obs);
					}
				}

			}

			IDictionary<string, double> medianDeltaH = null, medianDistances2D = null;
			if (dim != 2)
			{
				medianDeltaH = this.getMedianDeltaH(pointName, deltaH, distances3D, zenithangle);
			}
			else if (dim != 1)
			{
				medianDistances2D = this.getMedianDistance2D(pointName, distances2D, distances3D, zenithangle);
			}

			deltaH = distances2D = distances3D = zenithangle = null;


			if (dim != 2 && medianDeltaH != null)
			{
				Point1D point1d = (Point1D)point;
				point1d.addBundle();
				foreach (KeyValuePair<string, double> e in medianDeltaH.SetOfKeyValuePairs())
				{
					string endPointName = e.Key;
					double dh = e.Value.doubleValue();
					point1d.addObservedPoint(endPointName, dh);
				}
				point1d.joinBundles();

			}

			else if (dim != 1)
			{
				Point2D point2d = (Point2D)point;
				IDictionary<int, IList<TerrestrialObservationRow>> directions = this.getObservations(pointName, null, ObservationType.DIRECTION, -1);
				// Ergaenze Pseudobeobachtungen Richtung zw. Festpunkten
				if (this.targetSystem2d != null && this.targetSystem2d.get(point2d.Name) != null)
				{
					Point startPoint = this.targetSystem2d.get(point2d.Name);
					IList<TerrestrialObservationRow> dirSet = new List<TerrestrialObservationRow>();
					for (int i = 0; i < this.targetSystem2d.size(); i++)
					{
						Point endPoint = this.targetSystem2d.get(i);
						if (!endPoint.Name.Equals(point2d.Name) && endPoint.Dimension != 1)
						{
							double dy = endPoint.Y - startPoint.Y;
							double dx = endPoint.X - startPoint.X;

							TerrestrialObservationRow obs = new TerrestrialObservationRow();
							obs.StartPointName = startPoint.Name;
							obs.EndPointName = endPoint.Name;
							obs.ValueApriori = MathExtension.MOD(Math.Atan2(dy, dx), 2.0 * Math.PI);
							dirSet.Add(obs);
						}
					}
					if (dirSet.Count > 1)
					{
						directions[-1] = dirSet;
					}
				}

				foreach (IList<TerrestrialObservationRow> dirSet in directions.Values)
				{
					IDictionary<string, double> medianDirection = this.getMedianDirections(dirSet);

					point2d.addBundle();
					foreach (KeyValuePair<string, double> elm in medianDirection.SetOfKeyValuePairs())
					{
						string endPointName = elm.Key;
						double dir = elm.Value.doubleValue();

						if (!this.directionLinks.ContainsKey(point2d.Name))
						{
							this.directionLinks[point2d.Name] = new HashSet<string>();
						}

						if (this.completestationNames.Contains(endPointName))
						{
							this.directionLinks[point2d.Name].Add(endPointName);
						}

						if (medianDistances2D != null && medianDistances2D.ContainsKey(endPointName))
						{
							double dist2d = medianDistances2D[endPointName];
							point2d.addObservedPoint(endPointName, dir, dist2d);
						}
					}
				}
				point2d.joinBundles();
			}
		}

		private IDictionary<string, double> getMedianDeltaH(string startPointName, IList<TerrestrialObservationRow> deltaH, IList<TerrestrialObservationRow> distances3D, IList<TerrestrialObservationRow> zenithangles)
		{
			IDictionary<string, IList<double>> deltaHList = new LinkedHashMap<string, IList<double>>();

			if (deltaH != null)
			{
				foreach (TerrestrialObservationRow dh in deltaH)
				{
					string endPointName = dh.EndPointName;
					double h = ClassicGeodeticComputation.ORTHO_HEIGHT(dh.ValueApriori.Value, dh.InstrumentHeight.Value, dh.ReflectorHeight.Value);

					if (!(deltaHList.ContainsKey(endPointName)))
					{
						deltaHList[endPointName] = new List<double>();
					}

					deltaHList[endPointName].Add(h);
				}
			}

			if (distances3D != null && zenithangles != null)
			{

				for (int j = 0; j < zenithangles.Count; j++)
				{
					string endPointName = zenithangles[j].EndPointName;

					double zenith = (zenithangles[j].ValueApriori).Value;
					double ihZenith = (zenithangles[j].InstrumentHeight).Value;
					double thZenith = (zenithangles[j].ReflectorHeight).Value;

					for (int i = 0; i < distances3D.Count; i++)
					{
						if (!endPointName.Equals(distances3D[i].EndPointName))
						{
							continue;
						}

						double dist3d = (distances3D[i].ValueApriori).Value;
						double ihDist = (distances3D[i].InstrumentHeight).Value;
						double thDist = (distances3D[i].ReflectorHeight).Value;

						double slopeDist = ClassicGeodeticComputation.SLOPEDISTANCE(dist3d, ihDist, thDist, zenith, ihZenith, thZenith);
						double h = ClassicGeodeticComputation.TRIGO_HEIGHT_3D(slopeDist, zenith, ihZenith, thZenith);

						if (!(deltaHList.ContainsKey(endPointName)))
						{
							deltaHList[endPointName] = new List<double>();
						}

						deltaHList[endPointName].Add(h);
					}
					// Bestimme die Horizontalstrecke zusaetzlich aus Koordinaten um den Hoehenunterschied ueber den Z-Winkel noch abzuleiten
					if (this.targetSystem2d != null && this.targetSystem2d.contains(startPointName) && this.targetSystem2d.contains(endPointName) && !this.outlier2d.Contains(startPointName) && !this.outlier2d.Contains(endPointName))
					{
						double dist2d = this.targetSystem2d.get(startPointName).getDistance2D(this.targetSystem2d.get(endPointName));
						double h = ClassicGeodeticComputation.TRIGO_HEIGHT_2D(dist2d, zenith, ihZenith, thZenith);
						if (!(deltaHList.ContainsKey(endPointName)))
						{
							deltaHList[endPointName] = new List<double>();
						}

						deltaHList[endPointName].Add(h);
					}
				}
			}
			IDictionary<string, double> deltaHmap = new LinkedHashMap<string, double>();
			foreach (KeyValuePair<string, IList<double>> e in deltaHList.SetOfKeyValuePairs())
			{
				IList<double> list = e.Value;

				if (list.Count > 0)
				{
					list.Sort();
					deltaHmap[e.Key] = list[(int)((list.Count - 1) / 2)];
				}
			}
			return deltaHmap;
		}

		private IDictionary<string, double> getMedianDistance2D(string startPointName, IList<TerrestrialObservationRow> distances2D, IList<TerrestrialObservationRow> distances3D, IList<TerrestrialObservationRow> zenithangles)
		{
			IDictionary<string, IList<double>> dist2dList = new LinkedHashMap<string, IList<double>>();

			if (distances2D != null)
			{
				foreach (TerrestrialObservationRow dist in distances2D)
				{
					string endPointName = dist.EndPointName;

					if (!(dist2dList.ContainsKey(endPointName)))
					{
						dist2dList[endPointName] = new List<double>();
					}

					dist2dList[endPointName].Add(dist.ValueApriori.Value);
				}
			}

			if (distances3D != null && zenithangles != null)
			{
				for (int i = 0; i < distances3D.Count; i++)
				{
					string endPointName = distances3D[i].EndPointName;
					double dist3d = (distances3D[i].ValueApriori).Value;
					double ihDist = (distances3D[i].InstrumentHeight).Value;
					double thDist = (distances3D[i].ReflectorHeight).Value;
					for (int j = 0; j < zenithangles.Count; j++)
					{
						if (!endPointName.Equals(zenithangles[j].EndPointName))
						{
							continue;
						}

						double zenith = (zenithangles[j].ValueApriori).Value;
						double ihZenith = (zenithangles[j].InstrumentHeight).Value;
						double thZenith = (zenithangles[j].ReflectorHeight).Value;

						double slopeDist = ClassicGeodeticComputation.SLOPEDISTANCE(dist3d, ihDist, thDist, zenith, ihZenith, thZenith);
						double dist2d = ClassicGeodeticComputation.DISTANCE2D(slopeDist, zenith);

						if (!(dist2dList.ContainsKey(endPointName)))
						{
							dist2dList[endPointName] = new List<double>();
						}

						dist2dList[endPointName].Add(dist2d);
					}
				}
			}
			IDictionary<string, double> dist2D = new LinkedHashMap<string, double>();
			foreach (KeyValuePair<string, IList<double>> e in dist2dList.SetOfKeyValuePairs())
			{
				IList<double> list = e.Value;
				if (list.Count > 0)
				{
					list.Sort();
					dist2D[e.Key] = list[(int)((list.Count - 1) / 2)];
				}
			}
			return dist2D;
		}

		private IDictionary<string, double> getMedianDirections(IList<TerrestrialObservationRow> observations)
		{
			IDictionary<string, IList<double>> obsToPoint = new LinkedHashMap<string, IList<double>>();
			foreach (TerrestrialObservationRow observation in observations)
			{
				string pointName = observation.EndPointName;
				if (obsToPoint.ContainsKey(pointName))
				{
					double refValue = obsToPoint[pointName][0];
					obsToPoint[pointName].Add(this.formingDirectionToFaceI(refValue, observation.ValueApriori.Value));
				}
				else
				{
					IList<double> l = new List<double>();
					l.Add(observation.ValueApriori.Value);
					obsToPoint[pointName] = l;
				}
			}
			// Erzeuge neue Liste
			IDictionary<string, double> dir = new LinkedHashMap<string, double>();
			//for (TerrestrialObservationRow observation : observations) {
			foreach (KeyValuePair<string, IList<double>> elm in obsToPoint.SetOfKeyValuePairs())
			{
				IList<double> list = elm.Value;
				if (list.Count > 0)
				{
					list.Sort();
					double median = list[(int)((list.Count - 1) / 2)];
					dir[elm.Key] = median;
				}
			}
			return dir;
		}

		private double formingDirectionToFaceI(double refValue, double dir)
		{
			double azimuthMeasuredFace1 = dir;
			double azimuthMeasuredFace2 = MathExtension.MOD(dir + Math.PI, 2 * Math.PI);
			double face1 = Math.Min(Math.Abs(refValue - azimuthMeasuredFace1), Math.Abs(Math.Abs(refValue - azimuthMeasuredFace1) - 2 * Math.PI));
			double face2 = Math.Min(Math.Abs(refValue - azimuthMeasuredFace2), Math.Abs(Math.Abs(refValue - azimuthMeasuredFace2) - 2 * Math.PI));

			if (face1 > face2)
			{
				return azimuthMeasuredFace2;
			}
			else
			{
				return azimuthMeasuredFace1;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D getArcIntersectionPoint(String pointName, org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle bundle) throws java.sql.SQLException
		private Point2D getArcIntersectionPoint(string pointName, PointBundle bundle)
		{
			if (bundle.Intersection)
			{
				return null;
			}

			IList<Point2D> startPoints = new List<Point2D>();
			IList<TerrestrialObservationRow> distances2d = this.getTerrestrialObservationsIgnoreGroups(pointName, ObservationType.HORIZONTAL_DISTANCE);
			IDictionary<string, double> medianDistance2d = this.getMedianDistance2D(pointName, distances2d, null, null);

			IList<Point2D> endPoints = new List<Point2D>(medianDistance2d.Count);
			IList<double> observations = new List<double>(medianDistance2d.Count);

			foreach (KeyValuePair<string, double> dist2d in medianDistance2d.SetOfKeyValuePairs())
			{
				if (!bundle.contains(dist2d.Key))
				{
					continue;
				}

				endPoints.Add((Point2D)bundle.get(dist2d.Key));
				observations.Add(dist2d.Value);
			}

			if (endPoints.Count < 3)
			{
				return null;
			}

			// Permutieren
			for (int k = 0; k < endPoints.Count; k++)
			{
				Point2D A = endPoints[k];
				double s1 = observations[k];
				for (int l = k + 1; l < endPoints.Count; l++)
				{
					int m = l + 1 < endPoints.Count?l + 1:0;
					Point2D B = endPoints[l];
					Point2D C = endPoints[m];

					double s2 = observations[l];
					double s3 = observations[m];
					double s = A.getDistance2D(B);

					if (2.0 * s * s1 == 0)
					{
						continue;
					}

					double tAB = ClassicGeodeticComputation.DIRECTION(A, B);
					double alpha = Math.Acos((s * s + s1 * s1 - s2 * s2) / (2.0 * s * s1));

					Point2D tmp1 = ClassicGeodeticComputation.POLAR(A, pointName, tAB + alpha, s1);
					Point2D tmp2 = ClassicGeodeticComputation.POLAR(A, pointName, tAB - alpha, s1);

					if (Math.Abs(C.getDistance2D(tmp1) - s3) < Math.Abs(C.getDistance2D(tmp2) - s3))
					{
						startPoints.Add(tmp1);
					}
					else
					{
						startPoints.Add(tmp2);
					}
				}
			}
			return (Point2D)this.getMedianPoint(startPoints);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D getBackwardIntersectionPoint(String startPointName, org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle bundle) throws java.sql.SQLException
		private Point2D getBackwardIntersectionPoint(string startPointName, PointBundle bundle)
		{
			IList<Point2D> startPoints = new List<Point2D>();
			IDictionary<int, IList<TerrestrialObservationRow>> directions = this.getObservations(startPointName, null, ObservationType.DIRECTION, -1);
			foreach (IList<TerrestrialObservationRow> directionSets in directions.Values)
			{
				IDictionary<string, double> medianDirection = this.getMedianDirections(directionSets);

				IList<Point2D> endPoints = new List<Point2D>(medianDirection.Count);
				IList<double> observations = new List<double>(medianDirection.Count);

				foreach (KeyValuePair<string, double> dir in medianDirection.SetOfKeyValuePairs())
				{
					if (!bundle.contains(dir.Key))
					{
						continue;
					}

					endPoints.Add((Point2D)bundle.get(dir.Key));
					observations.Add(dir.Value);
				}

				if (endPoints.Count < 3)
				{
					return null;
				}

				// Permutieren
				for (int k = 0; k < endPoints.Count; k++)
				{
					Point2D A = endPoints[k];
					double rA = observations[k];
					for (int l = k + 1; l < endPoints.Count; l++)
					{
						Point2D B = endPoints[l];
						double rB = observations[l];
						double distAB = A.getDistance2D(B);
						double tAB = ClassicGeodeticComputation.DIRECTION(A, B);
						for (int m = l + 1; m < endPoints.Count; m++)
						{
							Point2D C = endPoints[m];
							double rC = observations[m];

							double distAC = A.getDistance2D(C);
							double tAC = ClassicGeodeticComputation.DIRECTION(A, C);

							double alpha = MathExtension.MOD(rB - rA, 2.0 * Math.PI);
							double beta = MathExtension.MOD(rC - rB, 2.0 * Math.PI);


							if (alpha + beta > Math.PI)
							{
								alpha -= Math.PI;
								beta -= Math.PI;
							}

							double sinAlphaBeta = Math.Sin(alpha + beta);

							if (sinAlphaBeta == 0)
							{
								continue;
							}

							double gamma = tAB - tAC;

							double distGH = -distAC * Math.Sin(alpha) / sinAlphaBeta * Math.Sin(beta);
							double distAG = distAC * Math.Sin(alpha) / sinAlphaBeta * Math.Cos(beta);

							double distFB = distAB * Math.Sin(gamma);
							double distAF = distAB * Math.Cos(gamma);

							double delta = Math.Atan2(distGH - distFB, distAG - distAF);
							if (delta > Math.PI)
							{
								delta = delta - Math.PI;
							}

							double tAN = tAC + delta - alpha;
							double distAN = distAC * Math.Sin(delta + beta) / sinAlphaBeta;

							Point2D N = ClassicGeodeticComputation.POLAR(A, startPointName, tAN, distAN);
							startPoints.Add(N);
						}
					}
				}

			}
			return (Point2D)this.getMedianPoint(startPoints);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D getForwardIntersectionPoint(String newPointId, org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle bundle) throws java.sql.SQLException
		private Point2D getForwardIntersectionPoint(string newPointId, PointBundle bundle)
		{
			string sql = "SELECT DISTINCT \"start_point_name\", \"group_id\" FROM \"ObservationApriori\" AS \"o1\" " + "JOIN \"ObservationGroup\" AS \"g1\" ON \"g1\".\"id\" = \"o1\".\"group_id\" AND \"g1\".\"enable\" = TRUE " + "JOIN \"PointApriori\" ON \"PointApriori\".\"name\" = \"o1\".\"start_point_name\" AND \"PointApriori\".\"enable\" = TRUE " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" AND \"PointGroup\".\"enable\" = TRUE " + "WHERE \"o1\".\"enable\" = TRUE AND \"g1\".\"type\" = ? AND \"o1\".\"end_point_name\" = ?";

			// Gruppen ID als Index
			IList<int> groups = new List<int>();
			// Gruppen ID als Index mit zugehoerigem Endpunkt und Messung
			IDictionary<int, TerrestrialObservationRow> directionSets = new LinkedHashMap<int, TerrestrialObservationRow>();
			// Ermittel gemeinsame Startpunkte von N
			PreparedStatement statement = this.dataBase.getPreparedStatement(sql);
			statement.setInt(1, ObservationType.DIRECTION.getId());
			statement.setString(2, newPointId);

			ResultSet result = statement.executeQuery();
			while (result.next())
			{
				int groupId = result.getInt("group_id");
				string startPointName = result.getString("start_point_name");

				// Wenn der Standpunkt unbekannt ist, ignoriere diesen
				if (!bundle.contains(startPointName))
				{
					continue;
				}

				// ermittel den Richtungssatz der Gruppe - Es kann nur EIN Satz sein, da groupID uebergeben wurde
				IDictionary<int, IList<TerrestrialObservationRow>> directions = this.getObservations(startPointName, null, ObservationType.DIRECTION, groupId);
				TerrestrialObservationRow orientatedDirectionAngle = this.getOrientatedDirectionAngle(newPointId, directions[groupId], bundle);
				if (orientatedDirectionAngle != null)
				{
					directionSets[groupId] = orientatedDirectionAngle;
					groups.Add(groupId);
				}
			}

			ForwardIntersectionSet forwardIntersections = new ForwardIntersectionSet();

			for (int i = 0; i < groups.Count; i++)
			{
				TerrestrialObservationRow directionAngleA = directionSets[groups[i]];
				string fixPointIdA = directionAngleA.StartPointName;
				Point2D fixPointA = (Point2D)bundle.get(fixPointIdA);
				if (string.ReferenceEquals(fixPointIdA, null))
				{
					continue;
				}

				for (int j = i + 1; j < groups.Count; j++)
				{
					TerrestrialObservationRow directionAngleB = directionSets[groups[j]];
					string fixPointIdB = directionAngleB.StartPointName;
					Point2D fixPointB = (Point2D)bundle.get(fixPointIdB);
					if (fixPointB == null || fixPointIdA.Equals(fixPointIdB))
					{
						continue;
					}

					double rAB = ClassicGeodeticComputation.DIRECTION(fixPointA, fixPointB);
					double rAN = directionAngleA.ValueApriori.Value;
					double rBA = MathExtension.MOD(rAB + Math.PI, 2.0 * Math.PI);
					double rBN = directionAngleB.ValueApriori.Value;

					forwardIntersections.add(fixPointA, fixPointB, newPointId, rAB, rAN, rBA, rBN);
				}
			}

			IList<Point2D> points = forwardIntersections.adjustForwardIntersections();
			return (Point2D)this.getMedianPoint(points);
		}

		/// <summary>
		/// Erzeugt Sub-Systeme, die per Vorwaertsschnitt entstehen. Berechnet den Neupunkt.
		/// Benoetigt die beiden Festpunkte. Der Festpunkt B wird tmp. auf 10/20 gesetzt, sodass
		/// gegenueber dem globalen System ein Massstab zu schaetzen ist!
		/// Der Neupunkt wird als Sub-Set an A und B gefuegt.
		/// </summary>
		/// <param name="fixPointA"> </param>
		/// <param name="fixPointB"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void addForwardIntersectionCombinations(org.applied_geodesy.adjustment.network.approximation.bundle.point.Point referencePointA, org.applied_geodesy.adjustment.network.approximation.bundle.point.Point referencePointB) throws java.sql.SQLException
		private void addForwardIntersectionCombinations(Point referencePointA, Point referencePointB)
		{
			int offsetX = 10, offsetY = 20;

			ForwardIntersectionSet forwardIntersections = new ForwardIntersectionSet();

			string referencePointNameA = referencePointA.Name;
			string referencePointNameB = referencePointB.Name;

			Point2D tmpReferencePointA = new Point2D(referencePointNameA, 0, 0);
			Point2D tmpReferencePointB = new Point2D(referencePointNameB, offsetX, offsetY);

			// SQL gemeinsame Endpunkte von A und B
			string sqlPoint = "SELECT DISTINCT \"end_point_name\" FROM \"ObservationApriori\" AS \"o1\" " + "JOIN \"PointApriori\" AS \"pa1\" ON \"o1\".\"end_point_name\" = \"pa1\".\"name\" AND \"pa1\".\"enable\" = TRUE " + "JOIN \"PointGroup\" AS \"pg1\" ON \"pa1\".\"group_id\" = \"pg1\".\"id\" AND \"pg1\".\"enable\" = TRUE " + "JOIN \"ObservationApriori\" AS \"o2\" ON \"o1\".\"end_point_name\" = \"o2\".\"end_point_name\" AND \"o1\".\"start_point_name\" != \"o2\".\"start_point_name\" " + "JOIN \"ObservationGroup\" AS \"g1\" ON \"g1\".\"id\" = \"o1\".\"group_id\" AND \"g1\".\"enable\" = TRUE " + "JOIN \"ObservationGroup\" AS \"g2\" ON \"g2\".\"id\" = \"o2\".\"group_id\" AND \"g1\".\"type\" = \"g2\".\"type\" AND \"g2\".\"enable\" = TRUE " + "WHERE \"g1\".\"type\" = ? AND \"o1\".\"start_point_name\" = ? AND  \"o2\".\"start_point_name\" = ?  AND \"o1\".\"enable\" = TRUE AND \"o2\".\"enable\" = TRUE";

			// SQL Richtungen von A nach B und <EndPunkt>
			string sqlObservation = "SELECT \"group_id\", \"end_point_name\", \"value_0\" FROM \"ObservationApriori\" " + "JOIN \"ObservationGroup\" ON \"ObservationGroup\".\"id\" = \"ObservationApriori\".\"group_id\" AND \"ObservationGroup\".\"enable\" = TRUE AND \"ObservationGroup\".\"type\" = ? " + "WHERE \"enable\" = TRUE AND \"start_point_name\" = ? AND \"end_point_name\" IN (?, ?) " + "ORDER BY \"group_id\", \"end_point_name\", \"id\"";

			ISet<string> identEndPoints = new LinkedHashSet<string>();

			// Ermittel gemeinsame Endpunkte von A und B und speichere in identEndPoints
			PreparedStatement statement = this.dataBase.getPreparedStatement(sqlPoint);
			statement.setInt(1, ObservationType.DIRECTION.getId());
			statement.setString(2, referencePointNameA);
			statement.setString(3, referencePointNameB);

			ResultSet result = statement.executeQuery();
			while (result.next())
			{
				string pointName = result.getString("end_point_name");
				if (!referencePointA.containsPointInBundle(pointName) || !referencePointB.containsPointInBundle(pointName))
				{
					identEndPoints.Add(pointName);
				}
			}

			statement = this.dataBase.getPreparedStatement(sqlObservation);
			statement.setInt(1, ObservationType.DIRECTION.getId());

			foreach (string pointName in identEndPoints)
			{
				statement.setString(4, pointName);

				IDictionary<int, List<TerrestrialObservationRow>> observationsA = new LinkedHashMap<int, List<TerrestrialObservationRow>>();
				IDictionary<int, List<TerrestrialObservationRow>> observationsB = new LinkedHashMap<int, List<TerrestrialObservationRow>>();

				// Ermittle Richtungen von A nach B und identEndPoints-Punkten
				statement.setString(2, referencePointNameA);
				statement.setString(3, referencePointNameB);
				result = statement.executeQuery();
				while (result.next())
				{
					int groupId = result.getInt("group_id");
					string endPointName = result.getString("end_point_name");
					double value = result.getDouble("value_0");

					if (!observationsA.ContainsKey(groupId))
					{
						observationsA[groupId] = new List<TerrestrialObservationRow>();
					}

					TerrestrialObservationRow obs = new TerrestrialObservationRow();
					obs.StartPointName = referencePointNameA;
					obs.EndPointName = endPointName;
					obs.ValueApriori = value;

					observationsA[groupId].Add(obs);
				}

				// Ermittle Richtungen von B nach A und identEndPoints-Punkten
				statement.setString(2, referencePointNameB);
				statement.setString(3, referencePointNameA);
				result = statement.executeQuery();
				while (result.next())
				{
					int groupId = result.getInt("group_id");
					string endPointName = result.getString("end_point_name");
					double value = result.getDouble("value_0");

					if (!observationsB.ContainsKey(groupId))
					{
						observationsB[groupId] = new List<TerrestrialObservationRow>();
					}

					TerrestrialObservationRow obs = new TerrestrialObservationRow();
					obs.StartPointName = referencePointNameB;
					obs.EndPointName = endPointName;
					obs.ValueApriori = value;

					observationsB[groupId].Add(obs);
				}

				// erzeuge ein Vorwaertsschnitt-Obj
				IDictionary<int, IDictionary<string, double>> medianObsA = new LinkedHashMap<int, IDictionary<string, double>>();
				IDictionary<int, IDictionary<string, double>> medianObsB = new LinkedHashMap<int, IDictionary<string, double>>();
				foreach (KeyValuePair<int, List<TerrestrialObservationRow>> groupObsA in observationsA.SetOfKeyValuePairs())
				{
					int groupId = groupObsA.Key;
					IList<TerrestrialObservationRow> obsA = groupObsA.Value;
					IDictionary<string, double> median = this.getMedianDirections(obsA);
					if (median.ContainsKey(referencePointNameB) && median.ContainsKey(pointName))
					{
						medianObsA[groupId] = median;
					}
				}

				foreach (KeyValuePair<int, List<TerrestrialObservationRow>> groupObsB in observationsB.SetOfKeyValuePairs())
				{
					int groupId = groupObsB.Key;
					IList<TerrestrialObservationRow> obsB = groupObsB.Value;
					IDictionary<string, double> median = this.getMedianDirections(obsB);
					if (median.ContainsKey(referencePointNameA) && median.ContainsKey(pointName))
					{
						medianObsB[groupId] = median;
					}
				}

				foreach (IDictionary<string, double> medianA in medianObsA.Values)
				{
					double rAB = medianA[referencePointNameB];
					double rAN = medianA[pointName];
					foreach (IDictionary<string, double> medianB in medianObsB.Values)
					{
						double rBA = medianB[referencePointNameA];
						double rBN = medianB[pointName];

						forwardIntersections.add(tmpReferencePointA, tmpReferencePointB, pointName, rAB, rAN, rBA, rBN);
					}
				}
			}

			IDictionary<string, ForwardIntersectionEntry> forwardIntersectionMap = forwardIntersections.getForwardIntersectionsByFixPoints(tmpReferencePointA, tmpReferencePointB);
			if (forwardIntersectionMap != null)
			{
				foreach (ForwardIntersectionEntry forwardIntersection in forwardIntersectionMap.Values)
				{
					Point2D intersectionPoint = forwardIntersection.adjust();

					referencePointA.addBundle(true);
					PointBundle bundle = referencePointA.CurrentBundle;
					bundle.addPoint(intersectionPoint);
					bundle.addPoint(tmpReferencePointB);

					referencePointB.addBundle(true);
					bundle = referencePointB.CurrentBundle;
					bundle.addPoint(new Point2D(intersectionPoint.Name, intersectionPoint.X - offsetX, intersectionPoint.Y - offsetY));
					bundle.addPoint(new Point2D(tmpReferencePointA.Name, tmpReferencePointA.X - offsetX, tmpReferencePointA.Y - offsetY));
				}
			}
		}


		/// <summary>
		/// Liefert die Beobachtungen mit beruecksichtigung der Gruppen. Sinnvoll bspw. bei Richtungen, da hier die 
		/// Saetze eine Orientierung haben. Die Reihenfolge Start-/Zielpunkt wird bei Strecken und Hoehenunterschieden 
		/// nicht beachtet!
		/// </summary>
		/// <param name="startPointName"> </param>
		/// <param name="endPointName"> </param>
		/// <param name="type"> </param>
		/// <returns> observations </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.Map<int, java.util.List<org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow>> getObservations(String startPointName, String endPointName, org.applied_geodesy.adjustment.network.ObservationType type, int groupId) throws java.sql.SQLException
		private IDictionary<int, IList<TerrestrialObservationRow>> getObservations(string startPointName, string endPointName, ObservationType type, int groupId)
		{
			IDictionary<int, IList<TerrestrialObservationRow>> observations = new LinkedHashMap<int, IList<TerrestrialObservationRow>>();
			bool isAngle = type == ObservationType.DIRECTION || type == ObservationType.ZENITH_ANGLE;
			string[] sqlFormatObs = new string[isAngle ? 1 : 2];

			sqlFormatObs[0] = "SELECT " + "\"ObservationApriori\".\"end_point_name\" AS \"end_point_name\", " + "\"ObservationApriori\".\"group_id\" AS \"group_id\", " + "\"ObservationApriori\".\"id\" AS \"id\", " + "\"ObservationApriori\".\"instrument_height\" AS \"instrument_height\", " + "\"ObservationApriori\".\"reflector_height\" AS \"reflector_height\", " + "\"ObservationApriori\".\"value_0\" AS \"value_0\" " + "FROM \"ObservationApriori\" INNER JOIN \"ObservationGroup\" ON " + "\"ObservationGroup\".\"id\" = \"ObservationApriori\".\"group_id\" AND " + "\"ObservationGroup\".\"type\" = ? AND " + "\"ObservationApriori\".\"start_point_name\" = ? AND " + (!string.ReferenceEquals(endPointName, null)?"\"ObservationApriori\".\"end_point_name\" = ? AND ":"") + (groupId >= 0?"\"ObservationApriori\".\"group_id\" = ? AND ":"") + "\"ObservationApriori\".\"enable\" = TRUE AND " + "\"ObservationGroup\".\"enable\" = TRUE " + "INNER JOIN \"PointApriori\" AS \"ps\" " + "ON \"ObservationApriori\".\"start_point_name\" = \"ps\".\"name\" AND \"ps\".\"enable\" = TRUE " + "INNER JOIN \"PointApriori\" AS \"pe\" " + "ON \"ObservationApriori\".\"end_point_name\" = \"pe\".\"name\" AND \"pe\".\"enable\" = TRUE " + "INNER JOIN \"PointGroup\" AS \"gs\" " + "ON \"ps\".\"group_id\" = \"gs\".\"id\" AND \"gs\".\"enable\" = TRUE " + "INNER JOIN \"PointGroup\" AS \"ge\" " + "ON \"pe\".\"group_id\" = \"ge\".\"id\" AND \"ge\".\"enable\" = TRUE " + "ORDER BY \"ObservationApriori\".\"id\" ASC";

			// Abstaende (Strecken und Hoehenunterschiede) werden im Hin- und Rueckweg ausgelesen,
			// beim Nivellement wird der Wert negiert --> -dH
			// SQL ist identisch mit o.g.; vertauscht jedoch Start- und Zielpunkt; 
			if (!isAngle)
			{
				sqlFormatObs[1] = "SELECT " + "\"ObservationApriori\".\"start_point_name\" AS \"end_point_name\", " + "\"ObservationApriori\".\"group_id\" AS \"group_id\", " + "\"ObservationApriori\".\"id\" AS \"id\", " + "\"ObservationApriori\".\"instrument_height\" AS \"reflector_height\", " + "\"ObservationApriori\".\"reflector_height\" AS \"instrument_height\", " + (type == ObservationType.LEVELING?"-":"") + "\"ObservationApriori\".\"value_0\" AS \"value_0\" " + "FROM \"ObservationApriori\" INNER JOIN \"ObservationGroup\" ON " + "\"ObservationGroup\".\"id\" = \"ObservationApriori\".\"group_id\" AND " + "\"ObservationGroup\".\"type\" = ? AND " + "\"ObservationApriori\".\"end_point_name\" = ? AND " + (!string.ReferenceEquals(endPointName, null)?"\"ObservationApriori\".\"start_point_name\" = ? AND ":"") + (groupId >= 0?"\"ObservationApriori\".\"group_id\" = ? AND ":"") + "\"ObservationApriori\".\"enable\" = TRUE AND " + "\"ObservationGroup\".\"enable\" = TRUE " + "INNER JOIN \"PointApriori\" AS \"ps\" " + "ON \"ObservationApriori\".\"start_point_name\" = \"ps\".\"name\" AND \"ps\".\"enable\" = TRUE " + "INNER JOIN \"PointApriori\" AS \"pe\" " + "ON \"ObservationApriori\".\"end_point_name\" = \"pe\".\"name\" AND \"pe\".\"enable\" = TRUE " + "INNER JOIN \"PointGroup\" AS \"gs\" " + "ON \"ps\".\"group_id\" = \"gs\".\"id\" AND \"gs\".\"enable\" = TRUE " + "INNER JOIN \"PointGroup\" AS \"ge\" " + "ON \"pe\".\"group_id\" = \"ge\".\"id\" AND \"ge\".\"enable\" = TRUE " + "ORDER BY \"ObservationApriori\".\"id\" ASC";
			}

			PreparedStatement statementPointObservations = null;
			foreach (string sqlFormatOb in sqlFormatObs)
			{
				int col = 1;
				statementPointObservations = this.dataBase.getPreparedStatement(sqlFormatOb);
				statementPointObservations.setInt(col++, type.getId());
				statementPointObservations.setString(col++, startPointName);
				if (!string.ReferenceEquals(endPointName, null))
				{
					statementPointObservations.setString(col++, endPointName);
				}
				if (groupId >= 0)
				{
					statementPointObservations.setInt(col++, groupId);
				}

				ResultSet obsSet = statementPointObservations.executeQuery();
				while (obsSet.next())
				{
					endPointName = obsSet.getString("end_point_name");
					int group_id = obsSet.getInt("group_id");
					double value = obsSet.getDouble("value_0");
					double ih = obsSet.getDouble("instrument_height");
					double th = obsSet.getDouble("reflector_height");

					TerrestrialObservationRow obs = new TerrestrialObservationRow();
					obs.StartPointName = startPointName;
					obs.EndPointName = endPointName;
					obs.InstrumentHeight = ih;
					obs.ReflectorHeight = th;
					obs.ValueApriori = value;

					if (observations.ContainsKey(group_id))
					{
						observations[group_id].Add(obs);
					}
					else
					{
						IList<TerrestrialObservationRow> obsList = new List<TerrestrialObservationRow>();
						obsList.Add(obs);
						observations[group_id] = obsList;
					}
				}
			}
			return observations;
		}

		/// <summary>
		/// Liefert die terr. Beobachtungen *ohne* Beruecksichtigung der Gruppen. Die Reihenfolge 
		/// Start-/Zielpunkt wird bei Strecken und Hoehenunterschieden nicht beachtet!
		/// </summary>
		/// <param name="startPointName"> </param>
		/// <param name="endPointName"> </param>
		/// <param name="type"> </param>
		/// <returns> observations </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow> getTerrestrialObservationsIgnoreGroups(String startPointName, org.applied_geodesy.adjustment.network.ObservationType type) throws java.sql.SQLException
		private IList<TerrestrialObservationRow> getTerrestrialObservationsIgnoreGroups(string startPointName, ObservationType type)
		{
			IList<TerrestrialObservationRow> observations = new List<TerrestrialObservationRow>();

			string sqlFormatObs = "SELECT " + "\"ObservationApriori\".\"end_point_name\" AS \"end_point_name\", " + "\"ObservationApriori\".\"group_id\" AS \"group_id\", " + "\"ObservationApriori\".\"instrument_height\" AS \"instrument_height\", " + "\"ObservationApriori\".\"reflector_height\" AS \"reflector_height\", " + "\"ObservationApriori\".\"value_0\" AS \"value_0\" " + "FROM \"ObservationApriori\" JOIN \"ObservationGroup\" ON " + "\"ObservationGroup\".\"id\" = \"ObservationApriori\".\"group_id\" AND " + "\"ObservationGroup\".\"type\" = ? AND " + "\"ObservationApriori\".\"start_point_name\" = ? AND " + "\"ObservationApriori\".\"enable\" = TRUE AND " + "\"ObservationGroup\".\"enable\" = TRUE " + "JOIN \"PointApriori\" AS \"ps\" " + "ON \"ObservationApriori\".\"start_point_name\" = \"ps\".\"name\" AND \"ps\".\"enable\" = TRUE " + "JOIN \"PointApriori\" AS \"pe\" " + "ON \"ObservationApriori\".\"end_point_name\" = \"pe\".\"name\" AND \"pe\".\"enable\" = TRUE " + "JOIN \"PointGroup\" AS \"gs\" " + "ON \"ps\".\"group_id\" = \"gs\".\"id\" AND \"gs\".\"enable\" = TRUE " + "JOIN \"PointGroup\" AS \"ge\" " + "ON \"pe\".\"group_id\" = \"ge\".\"id\" AND \"ge\".\"enable\" = TRUE";

			// Abstaende (Strecken und Hoehenunterschiede) werden im Hin- und Rueckweg ausgelesen,
			// beim Nivellement wird der Wert negiert --> -dH
			// Fuer Zenitwinkel wird die Gegenvisur bestimmt --> z = 200 - zHin
			// SQL ist identisch mit o.g.; vertauscht jedoch Start- und Zielpunkt; 
			if (type != ObservationType.DIRECTION)
			{
				sqlFormatObs += " UNION ALL SELECT " + "\"ObservationApriori\".\"start_point_name\" AS \"end_point_name\", " + "\"ObservationApriori\".\"group_id\" AS \"group_id\", " + "\"ObservationApriori\".\"instrument_height\" AS \"reflector_height\", " + "\"ObservationApriori\".\"reflector_height\" AS \"instrument_height\", " + (type == ObservationType.LEVELING ? "-" : type == ObservationType.ZENITH_ANGLE ? "PI()-" : "") + "\"ObservationApriori\".\"value_0\" AS \"value_0\" " + "FROM \"ObservationApriori\" JOIN \"ObservationGroup\" ON " + "\"ObservationGroup\".\"id\" = \"ObservationApriori\".\"group_id\" AND " + "\"ObservationGroup\".\"type\" = ? AND " + "\"ObservationApriori\".\"end_point_name\" = ? AND " + "\"ObservationApriori\".\"enable\" = TRUE AND " + "\"ObservationGroup\".\"enable\" = TRUE " + "JOIN \"PointApriori\" AS \"ps\" " + "ON \"ObservationApriori\".\"start_point_name\" = \"ps\".\"name\" AND \"ps\".\"enable\" = TRUE " + "JOIN \"PointApriori\" AS \"pe\" " + "ON \"ObservationApriori\".\"end_point_name\" = \"pe\".\"name\" AND \"pe\".\"enable\" = TRUE " + "JOIN \"PointGroup\" AS \"gs\" " + "ON \"ps\".\"group_id\" = \"gs\".\"id\" AND \"gs\".\"enable\" = TRUE " + "JOIN \"PointGroup\" AS \"ge\" " + "ON \"pe\".\"group_id\" = \"ge\".\"id\" AND \"ge\".\"enable\" = TRUE";
			}

			PreparedStatement statementPointObservations = null;
			statementPointObservations = this.dataBase.getPreparedStatement(sqlFormatObs);
			statementPointObservations.setInt(1, type.getId());
			statementPointObservations.setString(2, startPointName);
			if (type != ObservationType.DIRECTION)
			{
				statementPointObservations.setInt(3, type.getId());
				statementPointObservations.setString(4, startPointName);
			}

			ResultSet obsSet = statementPointObservations.executeQuery();
			while (obsSet.next())
			{
				string endPointName = obsSet.getString("end_point_name");
				double value = obsSet.getDouble("value_0");
				double ih = obsSet.getDouble("instrument_height");
				double th = obsSet.getDouble("reflector_height");
				TerrestrialObservationRow obs = new TerrestrialObservationRow();
				obs.StartPointName = startPointName;
				obs.EndPointName = endPointName;
				obs.InstrumentHeight = ih;
				obs.ReflectorHeight = th;
				obs.ValueApriori = value;
				observations.Add(obs);
			}
			return observations;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.List<String> getPointNames(int dim, boolean stationsOnly) throws java.sql.SQLException
		private IList<string> getPointNames(int dim, bool stationsOnly)
		{
			IList<string> pointNames = new List<string>();
			string sqlFormatPoints;

			if (stationsOnly)
			{
				sqlFormatPoints = "SELECT DISTINCT \"start_point_name\" AS \"name\" FROM \"ObservationApriori\" " + "JOIN \"PointApriori\" ON \"PointApriori\".\"name\" = \"ObservationApriori\".\"start_point_name\" " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" " + "WHERE \"ObservationApriori\".\"enable\" = TRUE AND \"PointApriori\".\"enable\" = TRUE AND \"PointGroup\".\"enable\" = TRUE AND " + "\"PointGroup\".\"dimension\" = ?";
			}
			else
			{
				sqlFormatPoints = "SELECT \"name\" FROM \"PointApriori\" " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" " + "WHERE \"PointGroup\".\"enable\" = TRUE AND \"PointApriori\".\"enable\" = TRUE AND " + "\"PointGroup\".\"dimension\" = ?";
			}

			PreparedStatement statementPoints = this.dataBase.getPreparedStatement(sqlFormatPoints);
			statementPoints.setInt(1, dim);
			ResultSet pointNameSet = statementPoints.executeQuery();
			while (pointNameSet.next())
			{
				string pointName = pointNameSet.getString("name");
				pointNames.Add(pointName);
			}
			return pointNames;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void initTargetSystems(boolean isFreeNet) throws java.sql.SQLException
		private void initTargetSystems(bool isFreeNet)
		{
			string sqlGroup = "SELECT \"id\" FROM \"PointGroup\" WHERE (\"type\" = ? OR \"type\" = ?) AND \"dimension\" = ? AND \"enable\" = TRUE ORDER BY \"id\" ASC";
			string sqlPoint = "SELECT \"name\", \"x0\", \"y0\", \"z0\" FROM \"PointApriori\" WHERE \"group_id\" = ? AND \"enable\" = TRUE ORDER BY \"id\" ASC";
			PointBundle targetSystem1d = new PointBundle(1), targetSystem2d = new PointBundle(2);

			PreparedStatement statementGroup = this.dataBase.getPreparedStatement(sqlGroup);
			PreparedStatement statementPoint = this.dataBase.getPreparedStatement(sqlPoint);

			for (int pointDim = 1; pointDim < 4; pointDim++)
			{
				if (!isFreeNet)
				{
					statementGroup.setInt(1, PointType.REFERENCE_POINT.getId());
					statementGroup.setInt(2, PointType.STOCHASTIC_POINT.getId());
				}
				else
				{
					statementGroup.setInt(1, PointType.DATUM_POINT.getId());
					statementGroup.setInt(2, PointType.DATUM_POINT.getId());
				}
				statementGroup.setInt(3, pointDim);

				ResultSet groupSet = statementGroup.executeQuery();
				while (groupSet.next())
				{
					statementPoint.setInt(1, groupSet.getInt("id"));

					ResultSet pointsSet = statementPoint.executeQuery();
					while (pointsSet.next())
					{
						if (pointDim == 1)
						{
							Point point = new Point1D(pointsSet.getString("name"), pointsSet.getDouble("z0"));
							if (targetSystem1d.get(point.Name) == null)
							{
								targetSystem1d.addPoint(point);
							}
						}
						else if (pointDim == 2)
						{
							Point point = new Point2D(pointsSet.getString("name"), pointsSet.getDouble("x0"), pointsSet.getDouble("y0"));
							if (targetSystem2d.get(point.Name) == null)
							{
								targetSystem2d.addPoint(point);
							}
						}
						else if (pointDim == 3)
						{
							Point point1d = new Point1D(pointsSet.getString("name"), pointsSet.getDouble("z0"));

							Point point2d = new Point2D(pointsSet.getString("name"), pointsSet.getDouble("x0"), pointsSet.getDouble("y0"));
							if (targetSystem1d.get(point1d.Name) == null)
							{
								targetSystem1d.addPoint(point1d);
							}
							if (targetSystem2d.get(point2d.Name) == null)
							{
								targetSystem2d.addPoint(point2d);
							}
						}
					}
				}
			}

			if (targetSystem1d != null && targetSystem1d.size() > 0)
			{
				this.targetSystem1d = targetSystem1d;
			}

			if (targetSystem2d != null && targetSystem2d.size() > 0)
			{
				this.targetSystem2d = targetSystem2d;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void savePoints(org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle bundle) throws java.sql.SQLException
		private void savePoints(PointBundle bundle)
		{
			for (int i = 0; bundle != null && i < bundle.size(); i++)
			{
				this.savePoint(bundle.get(i));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void savePoint(org.applied_geodesy.adjustment.network.approximation.bundle.point.Point p) throws java.sql.SQLException
		private void savePoint(Point p)
		{
			if (p == null)
			{
				return;
			}

			string sqlPoint = null;

			if (p.Dimension == 1)
			{
				sqlPoint = "UPDATE \"PointApriori\" SET \"z0\" = ? WHERE \"name\" = ?";
			}
			else if (p.Dimension == 2)
			{
				sqlPoint = "UPDATE \"PointApriori\" SET \"x0\" = ?, \"y0\" = ? WHERE \"name\" = ?";
			}
			else
			{
				return;
			}

			PreparedStatement statementPoint = this.dataBase.getPreparedStatement(sqlPoint);
			if (p.Dimension == 1)
			{
				statementPoint.setDouble(1,p.Z);
				statementPoint.setString(2,p.Name);
			}
			else if (p.Dimension == 2)
			{
				statementPoint.setDouble(1,p.X);
				statementPoint.setDouble(2,p.Y);
				statementPoint.setString(3,p.Name);
			}

			statementPoint.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void transferAposteriori2AprioriValues() throws java.sql.SQLException
		public virtual void transferAposteriori2AprioriValues()
		{
			string sql = "UPDATE \"AdditionalParameterApriori\" " + "SET \"value_0\" = " + "CASE WHEN " + "(SELECT \"value\" FROM \"AdditionalParameterAposteriori\" WHERE \"AdditionalParameterApriori\".\"id\" = \"AdditionalParameterAposteriori\".\"id\")  " + "ELSE \"value_0\" " + "END " + "WHERE \"enable\" = TRUE";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void transferAposteriori2AprioriValues(boolean transferDatumPoints) throws java.sql.SQLException
		public virtual void transferAposteriori2AprioriValues(bool transferDatumPoints)
		{
			string sql = "UPDATE \"AdditionalParameterApriori\" " + "SET \"value_0\" = IFNULL(" + "(SELECT \"value\" FROM \"AdditionalParameterAposteriori\" WHERE \"AdditionalParameterApriori\".\"id\" = \"AdditionalParameterAposteriori\".\"id\"),  " + "\"value_0\") " + "WHERE \"enable\" = TRUE";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.execute();

			int pointDimension = 3;
			sql = "UPDATE \"PointApriori\" SET " + "\"x0\" = IFNULL(" + "(SELECT \"x\" FROM \"PointAposteriori\" JOIN \"PointGroup\" ON \"group_id\" = \"PointGroup\".\"id\" AND \"PointGroup\".\"enable\" = TRUE AND \"PointGroup\".\"type\" IN (?, ?) WHERE \"PointApriori\".\"id\" = \"PointAposteriori\".\"id\" AND \"enable\" = TRUE), " + "\"x0\"), " + "\"y0\" = IFNULL(" + "(SELECT \"y\" FROM \"PointAposteriori\" JOIN \"PointGroup\" ON \"group_id\" = \"PointGroup\".\"id\" AND \"PointGroup\".\"enable\" = TRUE AND \"PointGroup\".\"type\" IN (?, ?) WHERE \"PointApriori\".\"id\" = \"PointAposteriori\".\"id\" AND \"enable\" = TRUE), " + "\"y0\"), " + "\"z0\" = IFNULL(" + "(SELECT \"z\" FROM \"PointAposteriori\" JOIN \"PointGroup\" ON \"group_id\" = \"PointGroup\".\"id\" AND \"PointGroup\".\"enable\" = TRUE AND \"PointGroup\".\"type\" IN (?, ?) WHERE \"PointApriori\".\"id\" = \"PointAposteriori\".\"id\" AND \"enable\" = TRUE), " + "\"z0\") " + "WHERE \"enable\" = TRUE";

			stmt = this.dataBase.getPreparedStatement(sql);
			for (int i = 0, j = 1; i < pointDimension; i++)
			{
				stmt.setInt(j++, PointType.NEW_POINT.getId());
				stmt.setInt(j++, transferDatumPoints ? PointType.DATUM_POINT.getId() : PointType.NEW_POINT.getId());
			}
			stmt.execute();

			int deflectionDimension = 2;
			sql = "UPDATE \"VerticalDeflectionApriori\" SET " + "\"x0\" = IFNULL(" + "(SELECT \"x\" FROM \"VerticalDeflectionAposteriori\" JOIN \"VerticalDeflectionGroup\" ON \"group_id\" = \"VerticalDeflectionGroup\".\"id\" AND \"VerticalDeflectionGroup\".\"enable\" = TRUE AND \"VerticalDeflectionGroup\".\"type\" IN (?, ?) WHERE \"VerticalDeflectionApriori\".\"id\" = \"VerticalDeflectionAposteriori\".\"id\" AND \"enable\" = TRUE), " + "\"x0\"), " + "\"y0\" = IFNULL(" + "(SELECT \"y\" FROM \"VerticalDeflectionAposteriori\" JOIN \"VerticalDeflectionGroup\" ON \"group_id\" = \"VerticalDeflectionGroup\".\"id\" AND \"VerticalDeflectionGroup\".\"enable\" = TRUE AND \"VerticalDeflectionGroup\".\"type\" IN (?, ?) WHERE \"VerticalDeflectionApriori\".\"id\" = \"VerticalDeflectionAposteriori\".\"id\" AND \"enable\" = TRUE), " + "\"y0\") " + "WHERE \"enable\" = TRUE";

			stmt = this.dataBase.getPreparedStatement(sql);
			for (int i = 0, j = 1; i < deflectionDimension; i++)
			{
				stmt.setInt(j++, VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION.getId());
				stmt.setInt(j++, VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION.getId());
			}
			stmt.execute();

			this.estimationStatus1D = EstimationStateType.ERROR_FREE_ESTIMATION;
			this.estimationStatus2D = EstimationStateType.ERROR_FREE_ESTIMATION;
		}

		/// <summary>
		/// Liefert das Azimut von einem Punkt - Resultierend aus einem Richtungssatz und einem Bezugssystem </summary>
		/// <param name="newPointId"> </param>
		/// <param name="directionSet"> </param>
		/// <param name="bundle">
		/// @return </param>
		private TerrestrialObservationRow getOrientatedDirectionAngle(string newPointId, IList<TerrestrialObservationRow> directionSet, PointBundle bundle)
		{
			IDictionary<string, double> medianDirectionSet = this.getMedianDirections(directionSet);
			int count = medianDirectionSet.Count;

			string startPointName = directionSet[0].StartPointName;
			Point2D startPoint = (Point2D)bundle.get(startPointName);

			if (startPoint == null)
			{
				return null;
			}

			IList<double> o = new List<double>(count);
			int i = 0;
			foreach (KeyValuePair<string, double> direction in medianDirectionSet.SetOfKeyValuePairs())
			{
				string endPointName = direction.Key;
				Point2D endPoint = (Point2D)bundle.get(endPointName);

				if (endPoint == null)
				{
					continue;
				}

				double obsDir = direction.Value;
				double calDir = ClassicGeodeticComputation.DIRECTION(startPoint, endPoint);

				double tmp_o = obsDir - calDir;
				tmp_o = MathExtension.MOD(tmp_o, 2.0 * Math.PI);
				if (i > 0 && (2.0 * Math.PI) - Math.Abs(o[i - 1] - tmp_o) < 0.5)
				{
					if (tmp_o < o[i - 1])
					{
						tmp_o += 2.0 * Math.PI;
					}
					else
					{
						tmp_o -= 2.0 * Math.PI;
					}
				}
				o.Add(tmp_o);
				i++;
			}

			if (o.Count == 0)
			{
				return null;
			}

			o.Sort();
			double ori = o[o.Count / 2];

			if (medianDirectionSet.ContainsKey(newPointId))
			{
				TerrestrialObservationRow row = new TerrestrialObservationRow();
				row.StartPointName = startPointName;
				row.EndPointName = newPointId;
				row.ValueApriori = MathExtension.MOD(medianDirectionSet[newPointId] - ori, 2.0 * Math.PI);
				return row;
			}
			return null;
		}

		private Point getMedianPoint<T1>(IList<T1> points) where T1 : org.applied_geodesy.adjustment.network.approximation.bundle.point.Point
		{
			if (points.Count == 1)
			{
				return points[0];
			}
			else if (points.Count > 1)
			{
				int index = 0;

				IList<double> medianX = new List<double>();
				IList<double> medianY = new List<double>();
				IList<double> medianZ = new List<double>();

				for (int i = 0; i < points.Count; i++)
				{
					medianX.Add(points[i].getX());
					medianY.Add(points[i].getY());
					medianZ.Add(points[i].getZ());
				}

				medianX.Sort();
				medianY.Sort();
				medianZ.Sort();

				double x0 = medianX[medianX.Count / 2];
				double y0 = medianY[medianX.Count / 2];
				double z0 = medianZ[medianX.Count / 2];

				double norm2 = double.MaxValue;
				for (int i = 0; i < points.Count; i++)
				{
					double x = points[i].getX() - x0;
					double y = points[i].getY() - y0;
					double z = points[i].getZ() - z0;

					double norm = Math.Sqrt(x * x + y * y + z * z);
					if (norm < norm2)
					{
						norm2 = norm;
						index = i;
					}
				}
				return points[index];
			}
			return null;
		}

		public virtual EstimationStateType EstimationStatus1D
		{
			get
			{
				return this.estimationStatus1D;
			}
		}

		public virtual EstimationStateType EstimationStatus2D
		{
			get
			{
				return this.estimationStatus2D;
			}
		}

		public virtual ISet<string> UnderdeterminedPointNames1D
		{
			get
			{
				return this.underdeterminedPointNames1D;
			}
		}

		public virtual ISet<string> UnderdeterminedPointNames2D
		{
			get
			{
				return this.underdeterminedPointNames2D;
			}
		}

		public virtual bool EstimateDatumsPoints
		{
			set
			{
				this.estimateDatumPoints = value;
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