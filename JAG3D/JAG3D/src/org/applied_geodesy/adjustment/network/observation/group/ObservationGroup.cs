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

namespace org.applied_geodesy.adjustment.network.observation.group
{

	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using Epoch = org.applied_geodesy.adjustment.network.Epoch;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ComponentType = org.applied_geodesy.adjustment.network.observation.ComponentType;
	using GNSSBaseline = org.applied_geodesy.adjustment.network.observation.GNSSBaseline;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using AdditionalUnknownParameter = org.applied_geodesy.adjustment.network.parameter.AdditionalUnknownParameter;

	public class ObservationGroup
	{

		private int groupId;
		private IList<Observation> observations = new List<Observation>();
		private readonly Epoch epoch;
		private readonly double sigmaA, sigmaB, sigmaC;
		private ISet<Observation> excludedObservationsDuringAvaraging = null;

		public ObservationGroup(int id, double sigmaA, double sigmaB, double sigmaC, Epoch epoch)
		{
			if (sigmaA <= 0)
			{
				throw new System.ArgumentException(this.GetType() + " Fehler, Gruppen ID oder Standardabweichung unzulassig!");
			}
			this.groupId = id;
			this.sigmaA = sigmaA;
			this.sigmaB = sigmaB > 0?sigmaB:0.0;
			this.sigmaC = sigmaC > 0?sigmaC:0.0;
			this.epoch = epoch;
		}

		public virtual int Id
		{
			get
			{
				return this.groupId;
			}
		}

		public virtual Epoch Epoch
		{
			get
			{
				return this.epoch;
			}
		}

		public virtual void add(Observation observation)
		{
			// Sollte die Beobachtung keine a-priori Genauigkeit haben, 
			// bekommt sie die Gruppengenauigkeit
			if (observation.Std <= 0)
			{
				observation.Std = this.getStd(observation);
				observation.useGroupUncertainty(true);
			}
			// Gehoert die Beobachtung noch keiner Gruppe an, 
			// so wird sie dieser hinzugefuegt - Referenz wird abgelegt
			if (this.Id >= 0 && (observation.ObservationGroup == null || observation.ObservationGroup.Id < 0))
			{
				observation.ObservationGroup = this;
			}
			this.observations.Add(observation);
		}

		public virtual Observation get(int index)
		{
			return this.observations[index];
		}

		public virtual int size()
		{
			return this.observations.Count;
		}

		internal virtual double StdA
		{
			get
			{
				return this.sigmaA;
			}
		}

		internal virtual double StdB
		{
			get
			{
				return this.sigmaB;
			}
		}

		internal virtual double StdC
		{
			get
			{
				return this.sigmaC;
			}
		}

		public virtual double getStdA(Observation observation)
		{
			return this.StdA;
		}

		public virtual double getStdB(Observation observation)
		{
			return this.StdB;
		}

		public virtual double getStdC(Observation observation)
		{
			return this.StdC;
		}

		public virtual double getStd(Observation observation)
		{
			double sigmaA = this.getStdA(observation);
			double sigmaB = this.getStdB(observation);
			double sigmaC = this.getStdC(observation);

			return Math.Sqrt(sigmaA * sigmaA + sigmaB * sigmaB + sigmaC * sigmaC);
		}

		public virtual int numberOfAdditionalUnknownParameter()
		{
			return 0;
		}

		protected internal virtual bool removeObservation(Observation observation)
		{
			return this.observations.Remove(observation);
		}

		protected internal virtual void clearObservations()
		{
			this.observations.Clear();
		}

		public virtual AdditionalUnknownParameter setApproximatedValue(AdditionalUnknownParameter param)
		{
			return param;
		}

		public virtual void averageDetermination(double threshold)
		{
			this.excludedObservationsDuringAvaraging = new LinkedHashSet<Observation>(Math.Max(10, this.size() / 4));
			for (int i = 0; i < this.size(); i++)
			{
				Observation avgObs = this.get(i);
				string startPointId = avgObs.StartPoint.Name;
				string endPointId = avgObs.EndPoint.Name;
				IList<Observation> avgObservations = new List<Observation>(Math.Max(10, this.size() / 4));

				double ih = avgObs.StartPointHeight;
				double th = avgObs.EndPointHeight;

				// Mittlerer Messwert
				// double avarage = avgObs.getObservationValue();
				avgObservations.Add(avgObs);
				// Type der Beobachtung
				ObservationType avgType = avgObs.ObservationType;
				ComponentType avgSubType = null;

				if (avgType == ObservationType.GNSS1D || avgType == ObservationType.GNSS2D || avgType == ObservationType.GNSS3D)
				{
					avgSubType = ((GNSSBaseline)avgObs).Component;
				}

				for (int j = i + 1; j < this.size(); j++)
				{
					Observation obs = this.get(j);
					string obsStartPointId = obs.StartPoint.Name;
					string obsEndPointId = obs.EndPoint.Name;
					ObservationType obsType = obs.ObservationType;
					ComponentType obsSubType = null;
					if (obsType == ObservationType.GNSS1D || obsType == ObservationType.GNSS2D || obsType == ObservationType.GNSS3D)
					{
						obsSubType = ((GNSSBaseline)obs).Component;
					}

					double obsIh = obs.StartPointHeight;
					double obsTh = obs.EndPointHeight;

					if (obsStartPointId.Equals(startPointId) && obsEndPointId.Equals(endPointId) && obsIh == ih && obsTh == th && (obsSubType == null && avgSubType == null || obsSubType != null && avgSubType != null && obsSubType == avgSubType))
					{
						avgObservations.Add(obs);
						// avarage += obs.getObservationValue();

						// entferne Beobachtung und korrigiere Laufindex der Schleife
						this.removeObservation(obs);
						j--;
					}
				}

				if (avgObservations.Count > 1)
				{
					// sortiere die Liste und bestimme den Median
					avgObservations.Sort(new ComparatorAnonymousInnerClass(this));
					// Mittlere Strecke 
					double avgDist = 0.0;
					double avgValue = 0.0;
					Observation medianObs = avgObservations[(avgObservations.Count - 1) / 2];
					double median = medianObs.ValueApriori;

					// Tausche Werte, da nur noch die Beobachtung avgObs in der Gruppe verbleibt
					if (avgObs != medianObs)
					{
						double value = avgObs.ValueApriori;
						medianObs.ValueApriori = value;
						avgObs.ValueApriori = median;
					}

					int counter = 0;

					foreach (Observation obs in avgObservations)
					{
						double value = obs.ValueApriori;
						// Richtungen werden auf die I. Lage reduziert beim Bestimmen der Orientierung und liegen immer zw. 0 und 400
						// Dadurch kann der Median bei ~ 0... liegen und die (reduzierte) Richtung der I. Lage bei 399... liegen und umgedreht
						// Der Messwert wird daher an den Median angepasst durch +/-400, sodass sich aus 399... auch negative -0... Werte ergeben 
						if (obs.ObservationType == ObservationType.DIRECTION && Math.Abs(value - median) > Math.Abs(Math.Abs(value - median) - 2.0 * Math.PI))
						{
							if (value > median)
							{
								value = value - 2.0 * Math.PI;
							}
							else
							{
								value = value + 2.0 * Math.PI;
							}
						}
						if (Math.Abs(value - median) <= threshold)
						{
							avgValue += value;
							avgDist += obs.DistanceForUncertaintyModel;
							counter++;
						}
						else
						{
							obs.GrossError = value - median;
							this.excludedObservationsDuringAvaraging.Add(obs);
						}
					}
					avgValue /= counter;
					avgDist /= counter;

					if (avgObs.ObservationType == ObservationType.DIRECTION || avgObs.ObservationType == ObservationType.ZENITH_ANGLE)
					{
						avgValue = MathExtension.MOD(avgValue, 2.0 * Math.PI);
					}

					avgObs.ValueApriori = avgValue;
					// Setze mittlere Strecke fuer Unsicherheitsbestimmung
					if (avgDist > 0)
					{
						avgObs.DistanceForUncertaintyModel = avgDist;
					}
				}
			}

			if (this.excludedObservationsDuringAvaraging.Count == 0)
			{
				this.excludedObservationsDuringAvaraging = null;
			}
		}

		private class ComparatorAnonymousInnerClass : IComparer<Observation>
		{
			private readonly ObservationGroup outerInstance;

			public ComparatorAnonymousInnerClass(ObservationGroup outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public int Compare(Observation o1, Observation o2)
			{
				return o1.ValueApriori > o2.ValueApriori ? 1 : -1;
			}
		}

		public virtual bool Empty
		{
			get
			{
				return this.observations.Count == 0;
			}
		}

		public virtual ISet<Observation> ExcludedObservationsDuringAvaraging
		{
			get
			{
				return this.excludedObservationsDuringAvaraging;
			}
		}

		public override string ToString()
		{
			return new string(this.GetType() + " Observation-Id: " + this.Id + " Number of Observations " + this.size());
		}
	}

}