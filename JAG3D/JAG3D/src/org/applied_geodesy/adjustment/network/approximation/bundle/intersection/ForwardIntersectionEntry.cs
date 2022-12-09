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

namespace org.applied_geodesy.adjustment.network.approximation.bundle.intersection
{

	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using Point = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point;
	using Point2D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D;

	public class ForwardIntersectionEntry
	{
		private readonly Point2D fixPointA, fixPointB;
		private readonly string newPointId;
		private IList<double> alpha = new List<double>(), beta = new List<double>();

		public ForwardIntersectionEntry(Point2D fixPointA, Point2D fixPointB, string newPointId, double alpha, double beta)
		{
			this.fixPointA = fixPointA;
			this.fixPointB = fixPointB;
			this.newPointId = newPointId;

			this.addAngles(alpha, beta);
		}

		public virtual void addAngles(double alpha, double beta)
		{
			this.alpha.Add(MathExtension.MOD(alpha, 2.0 * Math.PI));
			this.beta.Add(MathExtension.MOD(beta, 2.0 * Math.PI));
		}

		public virtual Point2D FixPoint
		{
			set
			{
				if (value.Name.Equals(this.fixPointA.Name))
				{
					this.fixPointA.X = value.X;
					this.fixPointA.Y = value.Y;
				}
				else if (value.Name.Equals(this.fixPointB.Name))
				{
					this.fixPointB.X = value.X;
					this.fixPointB.Y = value.Y;
				}
			}
		}

		public virtual Point2D adjust()
		{
			if (this.fixPointA == null || this.fixPointB == null)
			{
				return null;
			}

			int n = this.alpha.Count;
			IList<double> medianX = new List<double>(n);
			IList<double> medianY = new List<double>(n);
			IList<Point2D> intersectPoint = new List<Point2D>(n);

			double xA = this.fixPointA.X;
			double yA = this.fixPointA.Y;

			double xB = this.fixPointB.X;
			double yB = this.fixPointB.Y;

			double sAB = this.fixPointA.getDistance2D(this.fixPointB);
			double tAB = Math.Atan2(yB - yA, xB - xA);

			for (int i = 0; i < n; i++)
			{
				double alpha = this.alpha[i];
				double beta = this.beta[i];

				if (alpha + beta == 0.0)
				{
					continue;
				}

				// Strecken zum Neupunkt via SIN-Satz
				double sAN = sAB * Math.Sin(beta) / Math.Sin(alpha + beta);
				double sBN = sAB * Math.Sin(alpha) / Math.Sin(alpha + beta);

				// Richtungswinkel zum Neupunkt
				double tAN = tAB - alpha;
				double tBN = Math.PI + tAB + beta;

				double yNA = yA + sAN * Math.Sin(tAN);
				double xNA = xA + sAN * Math.Cos(tAN);

				double yNB = yB + sBN * Math.Sin(tBN);
				double xNB = xB + sBN * Math.Cos(tBN);

				if (Math.Abs(yNA - yNB) < 0.1 && Math.Abs(xNA - xNB) < 0.1)
				{
					double yN = 0.5 * (yNA + yNB);
					double xN = 0.5 * (xNA + xNB);
					medianX.Add(xN);
					medianY.Add(yN);
					intersectPoint.Add(new Point2D(this.newPointId, xN, yN));
				}
			}

			medianX.Sort();
			medianY.Sort();

			double x0 = medianX[medianX.Count / 2];
			double y0 = medianY[medianX.Count / 2];

			double norm2 = double.MaxValue;
			int index = 0;
			for (int i = 0; i < intersectPoint.Count; i++)
			{
				double x = intersectPoint[i].X - x0;
				double y = intersectPoint[i].Y - y0;

				double norm = Math.hypot(x,y);
				if (norm < norm2)
				{
					norm2 = norm;
					index = i;
				}
			}
			return intersectPoint[index];
		}

		public virtual Point ReferencePointA
		{
			get
			{
				return this.fixPointA;
			}
		}

		public virtual Point ReferencePointB
		{
			get
			{
				return this.fixPointB;
			}
		}

		public virtual string NewPointId
		{
			get
			{
				return this.newPointId;
			}
		}
	}

}