using System;

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

namespace org.applied_geodesy.adjustment.network.congruence
{
	using ConfidenceRegion = org.applied_geodesy.adjustment.ConfidenceRegion;
	using Constant = org.applied_geodesy.adjustment.Constant;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public class CongruenceAnalysisPointPair
	{
		private readonly int id, dimension;
		private static readonly double ZERO = Math.Sqrt(Constant.EPS);
		private readonly Point p0, p1;
		private double[] sigma = null; private double[] confidenceAxis = null; private double[] confidenceAngles = new double[3]; private double[] grzw = null; private double[] nabla = null; private double[] confidenceAxis2D = new double[2]; private double confidenceAngle2D = 0;

		private double Tprio = 0.0, Tpost = 0.0, Pprio = 0.0, Ppost = 0.0;

		private bool significant = false;

		public CongruenceAnalysisPointPair(int id, int dimension, Point p0, Point p1)
		{
			if (p0.Dimension < dimension || p1.Dimension < dimension || p0.Dimension == 2 && p1.Dimension == 1 || p1.Dimension == 2 && p0.Dimension == 1)
			{
				throw new System.ArgumentException(this.GetType() + " Fehler, Dimension der Punkte nicht valid! " + p0.Dimension + ", " + p1.Dimension + " vs. " + dimension);
			}
			this.id = id;
			this.p0 = p0;
			this.p1 = p1;
			this.dimension = dimension;
			this.sigma = new double[this.Dimension];
			this.confidenceAxis = new double[this.Dimension];
			this.grzw = new double[this.Dimension];
			this.nabla = new double[this.Dimension];
		}

		public virtual void setProbabilityValues(double pPrio, double pPost)
		{
			this.Pprio = pPrio;
			this.Ppost = pPost;
		}

		public virtual void setTeststatisticValues(double tPrio, double tPost)
		{
			this.Tprio = tPrio;
			this.Tpost = tPost;
		}

		public virtual Point StartPoint
		{
			get
			{
				return this.p0;
			}
		}

		public virtual Point EndPoint
		{
			get
			{
				return this.p1;
			}
		}

		public int Id
		{
			get
			{
				return this.id;
			}
		}

		public int Dimension
		{
			get
			{
				return this.dimension;
			}
		}

		public virtual double Pprio
		{
			get
			{
				return this.Pprio;
			}
		}

		public virtual double Ppost
		{
			get
			{
				return this.Ppost;
			}
		}

		public virtual double Magnitude
		{
			get
			{
				int dim = this.Dimension;
				if (dim == 1)
				{
					return Math.Abs(p1.Z - p0.Z);
				}
				else if (dim == 2)
				{
					return p1.getDistance2D(p0);
				}
				else
				{
					return p1.getDistance3D(p0);
				}
			}
		}

		public virtual double GrossErrorX
		{
			get
			{
				return this.nabla[0];
			}
		}

		public virtual double GrossErrorY
		{
			get
			{
				return this.nabla[1];
			}
		}

		public virtual double GrossErrorZ
		{
			get
			{
				return this.nabla[this.Dimension - 1];
			}
		}

		public virtual double DeltaX
		{
			get
			{
				return this.p1.X - this.p0.X;
			}
		}

		public virtual double DeltaY
		{
			get
			{
				return this.p1.Y - this.p0.Y;
			}
		}

		public virtual double DeltaZ
		{
			get
			{
				return this.p1.Z - this.p0.Z;
			}
		}

		public virtual double StdX
		{
			get
			{
				return this.sigma[0];
			}
		}

		public virtual double StdY
		{
			get
			{
				return this.sigma[1];
			}
		}

		public virtual double StdZ
		{
			get
			{
				return this.sigma[this.Dimension - 1];
			}
		}

		public virtual double Tprio
		{
			get
			{
				return this.Tprio < ZERO ? 0.0 : this.Tprio;
			}
		}

		public virtual double Tpost
		{
			get
			{
				return this.Tpost < ZERO ? 0.0 : this.Tpost;
			}
		}

		public virtual bool Significant
		{
			get
			{
				return this.significant;
			}
			set
			{
				this.significant = value;
			}
		}

		public virtual double getConfidenceAxis(int i)
		{
			return this.confidenceAxis[i];
		}

		public virtual double getConfidenceAngle(int i)
		{
			return this.confidenceAngles[i];
		}

		public virtual ConfidenceRegion ConfidenceRegion
		{
			set
			{
				for (int i = 0; i < this.Dimension; i++)
				{
					this.confidenceAxis[i] = value.getConfidenceAxis(i);
    
					if (this.Dimension == 1 && i == 0 || this.Dimension > 1 && i < 2)
					{
						this.confidenceAxis2D[i] = value.getConfidenceAxis2D(i, false);
					}
				}
    
				if (this.Dimension > 1)
				{
					this.confidenceAngle2D = value.ConfidenceAngle2D;
					this.confidenceAngles = value.EulerAngles;
				}
			}
		}

		public virtual double getConfidenceAxis2D(int i)
		{
			return this.confidenceAxis2D[i];
		}

		public virtual double ConfidenceAngle2D
		{
			get
			{
				return this.confidenceAngle2D;
			}
		}


		public virtual double MinimalDetectableBiasX
		{
			get
			{
				return this.grzw[0];
			}
		}

		public virtual double MinimalDetectableBiasY
		{
			get
			{
				return this.grzw[1];
			}
		}

		public virtual double MinimalDetectableBiasZ
		{
			get
			{
				return this.grzw[this.Dimension - 1];
			}
		}

		public virtual double[] GrossErrors
		{
			set
			{
				if (value.Length == this.Dimension)
				{
					this.nabla = value;
				}
			}
		}

		public virtual double[] MinimalDetectableBiases
		{
			set
			{
				if (value.Length == this.Dimension)
				{
					this.grzw = value;
				}
			}
		}

		public virtual double[] Sigma
		{
			set
			{
				if (value.Length == this.Dimension)
				{
					this.sigma = value;
				}
			}
		}
	}

}