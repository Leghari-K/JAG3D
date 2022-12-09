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

namespace org.applied_geodesy.adjustment.network.approximation.bundle.intersection
{

	using Point2D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D;

	public class ForwardIntersectionSet
	{

		private class PointCouple
		{
			private readonly ForwardIntersectionSet outerInstance;

			internal readonly Point2D pointA, pointB;
			public PointCouple(ForwardIntersectionSet outerInstance, Point2D pointA, Point2D pointB)
			{
				this.outerInstance = outerInstance;
				this.pointA = pointA;
				this.pointB = pointB;
			}

			public virtual Point2D PointA
			{
				get
				{
					return this.pointA;
				}
			}

			public virtual Point2D PointB
			{
				get
				{
					return this.pointB;
				}
			}

			public override int GetHashCode()
			{
				const int prime = 31;
				int result = 1;
				result = prime * result + OuterType.GetHashCode();

				if (string.CompareOrdinal(pointA.Name, pointB.Name) < 0)
				{
					result = prime * result + pointA.Name.GetHashCode();
					result = prime * result + pointB.Name.GetHashCode();
				}
				else
				{
					result = prime * result + pointB.Name.GetHashCode();
					result = prime * result + pointA.Name.GetHashCode();
				}
				return result;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (obj == null)
				{
					return false;
				}
				if (this.GetType() != obj.GetType())
				{
					return false;
				}

				PointCouple other = (PointCouple) obj;

				return this.pointA.Name.Equals(other.PointA.Name) && this.pointB.Name.Equals(other.PointB.Name) || this.pointA.Name.Equals(other.PointB.Name) && this.pointB.Name.Equals(other.PointA.Name);
			}

			internal virtual ForwardIntersectionSet OuterType
			{
				get
				{
					return outerInstance;
				}
			}
		}

		internal IDictionary<PointCouple, IDictionary<string, ForwardIntersectionEntry>> forwardIntersectionMap = new LinkedHashMap<PointCouple, IDictionary<string, ForwardIntersectionEntry>>();

		public virtual void add(Point2D fixPointA, Point2D fixPointB, string newPointId, double rAB, double rAN, double rBA, double rBN)
		{
			ForwardIntersectionEntry forwardIntersectionEntry;

			if (string.CompareOrdinal(fixPointA.Name, fixPointB.Name) > 0)
			{
				double tmpR = rAB;
				rAB = rBA;
				rBA = tmpR;

				tmpR = rAN;
				rAN = rBN;
				rBN = tmpR;

				Point2D tmpPoint = fixPointA;
				fixPointA = fixPointB;
				fixPointB = tmpPoint;
			}
			PointCouple pointCouple = new PointCouple(this, fixPointA, fixPointB);

			double alpha = rAB - rAN;
			double beta = rBN - rBA;

			if (this.forwardIntersectionMap.ContainsKey(pointCouple))
			{
				IDictionary<string, ForwardIntersectionEntry> map = this.forwardIntersectionMap[pointCouple];
				if (map.ContainsKey(newPointId))
				{
					forwardIntersectionEntry = map[newPointId];
					forwardIntersectionEntry.addAngles(alpha, beta);
				}
				else
				{
					map[newPointId] = new ForwardIntersectionEntry(fixPointA, fixPointB, newPointId, alpha, beta);
				}
			}
			else
			{
				IDictionary<string, ForwardIntersectionEntry> map = new LinkedHashMap<string, ForwardIntersectionEntry>();
				map[newPointId] = new ForwardIntersectionEntry(fixPointA, fixPointB, newPointId, alpha, beta);
				this.forwardIntersectionMap[pointCouple] = map;
			}
		}

		public virtual IDictionary<string, ForwardIntersectionEntry> getForwardIntersectionsByFixPoints(Point2D fixPointA, Point2D fixPointB)
		{
			PointCouple pointCouple = new PointCouple(this, fixPointA, fixPointB);
			return this.forwardIntersectionMap[pointCouple];
		}

		public virtual IList<Point2D> adjustForwardIntersections()
		{
			IList<Point2D> intersectionPoints = new List<Point2D>();
			foreach (IDictionary<string, ForwardIntersectionEntry> forwardIntersections in this.forwardIntersectionMap.Values)
			{
				foreach (ForwardIntersectionEntry forwardIntersection in forwardIntersections.Values)
				{
					Point2D p2d = forwardIntersection.adjust();
					if (p2d != null)
					{
						intersectionPoints.Add(p2d);
					}
				}
			}
			return intersectionPoints;
		}
	}

}