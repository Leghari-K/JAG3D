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

namespace org.applied_geodesy.adjustment.network.point
{
	using ConfidenceRegion = org.applied_geodesy.adjustment.ConfidenceRegion;
	using Constant = org.applied_geodesy.adjustment.Constant;
	using UnknownParameter = org.applied_geodesy.adjustment.network.parameter.UnknownParameter;
	using VerticalDeflectionX = org.applied_geodesy.adjustment.network.parameter.VerticalDeflectionX;
	using VerticalDeflectionY = org.applied_geodesy.adjustment.network.parameter.VerticalDeflectionY;
	using SphericalDeflectionParameter = org.applied_geodesy.transformation.datum.SphericalDeflectionParameter;

	public abstract class Point : UnknownParameter
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			verticalDeflectionX = new VerticalDeflectionX(this);
			verticalDeflectionY = new VerticalDeflectionY(this);
			redundancy = new double[this.Dimension];
			ep = new double[this.Dimension];
			nabla = new double[this.Dimension];
			mdb = new double[this.Dimension];
			mtb = new double[this.Dimension];
			sigma = new double[this.Dimension];
			sigma0 = new double[this.Dimension];
			confidenceAxis = new double[this.Dimension];
			principalComponents = new double[this.Dimension];
		}

		// Punktnummer
		private string name = "";
		// Zeile in Designmatrix; -1 entspricht nicht gesetzt 
		private int rowInJacobiMatrix = -1;
		// Spalte in der Designmatrix; -1 entspricht nicht gesetzt
		private int colInDesignmatrixOfModelErros = -1;

		// Lotabweichungsparameter fuer den Punkt in X/Y-Richtung
		private VerticalDeflectionX verticalDeflectionX;
		private VerticalDeflectionY verticalDeflectionY;
		// Abweichungen aufgrund eines ellipsoidischen Erdmodells
		private SphericalDeflectionParameter sphericalDeflectionParameter = new SphericalDeflectionParameter();
		// Grenzwert fuer Null
		private static readonly double ZERO = Math.Sqrt(Constant.EPS);
		protected internal readonly double[] coordinates0 = new double[3]; //coordinates0[] = new double[this.getDimension()];
		private double[] coordinates = new double[3]; private double[] redundancy; private double[] ep; private double[] nabla; private double[] mdb; private double[] mtb; private double[] sigma; private double[] sigma0; private double[] confidenceAxis; private double[] confidenceAngles = new double[3]; private double[] principalComponents; private double[] confidenceAxis2D = new double[2]; private double confidenceAngle2D = 0; private double nablaCoVarNable = 0.0; private double efsp = 0.0; private double Tprio = 0.0; private double Tpost = 0.0; private double Pprio = 0.0; private double Ppost = 0.0; private double omega = 0.0;
		private bool significant = false;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Point(String name) throws IllegalArgumentException
		public Point(string name)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (string.ReferenceEquals(name, null) || name.Trim().Length == 0)
			{
				throw new System.ArgumentException(this.GetType() + " Punktnummer ungueltig!");
			}
			this.name = name.Trim();
			for (int i = 0; i < this.Dimension; i++)
			{
				this.sigma[i] = -1.0;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		public virtual int ColInDesignmatrixOfModelErros
		{
			get
			{
				return this.colInDesignmatrixOfModelErros;
			}
			set
			{
				this.colInDesignmatrixOfModelErros = value;
			}
		}


		public abstract int Dimension {get;}

		public virtual double StdX
		{
			set
			{
				this.sigma0[0] = (this.sigma0[0] <= 0 && value>0)?value:this.sigma0[0];
				this.sigma[0] = (value > 0)?value:-1.0;
			}
			get
			{
				return this.sigma[0];
			}
		}

		public virtual double StdY
		{
			set
			{
				this.sigma0[1] = (this.sigma0[1] <= 0 && value>0)?value:this.sigma0[1];
				this.sigma[1] = (value > 0)?value:-1.0;
			}
			get
			{
				return this.sigma[1];
			}
		}

		public virtual double StdZ
		{
			set
			{
				this.sigma0[this.Dimension - 1] = (this.sigma0[this.Dimension - 1] <= 0 && value>0)?value:this.sigma0[this.Dimension - 1];
				this.sigma[this.Dimension - 1] = (value > 0)?value:-1.0;
			}
			get
			{
				return this.sigma[this.Dimension - 1];
			}
		}




		public virtual double StdXApriori
		{
			get
			{
				return this.sigma0[0];
			}
			set
			{
				if (value > 0)
				{
					this.sigma0[0] = value;
				}
			}
		}

		public virtual double StdYApriori
		{
			get
			{
				return this.sigma0[1];
			}
			set
			{
				if (value > 0)
				{
					this.sigma0[1] = value;
				}
			}
		}

		public virtual double StdZApriori
		{
			get
			{
				return this.sigma0[this.Dimension - 1];
			}
			set
			{
				if (value > 0)
				{
					this.sigma0[this.Dimension - 1] = value;
				}
			}
		}




		public virtual double X
		{
			set
			{
				this.coordinates[0] = value;
			}
			get
			{
				return this.coordinates[0];
			}
		}

		public virtual double Y
		{
			set
			{
				this.coordinates[1] = value;
			}
			get
			{
				return this.coordinates[1];
			}
		}

		public virtual double Z
		{
			set
			{
				//this.coordinates[this.getDimension()-1] = value;
				this.coordinates[2] = value;
			}
			get
			{
				//return this.coordinates[this.getDimension()-1];
				return this.coordinates[2];
			}
		}




		public virtual double X0
		{
			get
			{
				return this.coordinates0[0];
			}
			set
			{
				this.coordinates0[0] = value;
				this.X = value;
			}
		}

		public virtual double Y0
		{
			get
			{
				return this.coordinates0[1];
			}
			set
			{
				this.coordinates0[1] = value;
				this.Y = value;
			}
		}

		public virtual double Z0
		{
			get
			{
				//return this.coordinates0[this.getDimension()-1];
				return this.coordinates0[2];
			}
			set
			{
				this.coordinates0[2] = value;
				this.Z = value;
			}
		}




		public virtual void resetCoordinates()
		{
			for (int i = 0; i < this.coordinates0.Length; i++)
			{
				this.coordinates[i] = this.coordinates0[i];
			}
		}

		public virtual double getDistance3D(Point p)
		{
			return Math.Sqrt(Math.Pow(this.X - p.X,2) + Math.Pow(this.Y - p.Y,2) + Math.Pow(this.Z - p.Z,2));
		}

		public virtual double getAprioriDistance3D(Point p)
		{
			return Math.Sqrt(Math.Pow(this.X0 - p.X0,2) + Math.Pow(this.Y0 - p.Y0,2) + Math.Pow(this.Z0 - p.Z0,2));
		}

		public virtual double getDistance2D(Point p)
		{
			return Math.hypot(this.X - p.X, this.Y - p.Y);
		}

		public virtual double getAprioriDistance2D(Point p)
		{
			return Math.hypot(this.X0 - p.X0, this.Y0 - p.Y0);
		}

		public virtual int RowInJacobiMatrix
		{
			get
			{
				return this.rowInJacobiMatrix;
			}
			set
			{
				this.rowInJacobiMatrix = value;
			}
		}


		public virtual double InfluenceOnNetworkDistortion
		{
			set
			{
				this.efsp = value;
			}
			get
			{
				return this.efsp;
			}
		}


		public virtual double NablaCoVarNabla
		{
			set
			{
				if (value >= 0)
				{
					this.nablaCoVarNable = value;
				}
			}
		}

		public virtual void calcStochasticParameters(double sigma2apost, int redundancy, bool applyAposterioriVarianceOfUnitWeight)
		{
			// Bestimmung der Testgroessen
			double omega = sigma2apost * (double)redundancy;
			int dim = this.Dimension;
			double sigma2apostPoint = (redundancy - dim) > 0?(omega - this.nablaCoVarNable) / (redundancy - dim):0.0;

			this.Tprio = this.nablaCoVarNable / dim;
			this.Tpost = (applyAposterioriVarianceOfUnitWeight && sigma2apostPoint > Point.ZERO) ? this.nablaCoVarNable / (dim * sigma2apostPoint) : 0.0;

			// Berechnung der Standardabweichungen
			for (int i = 0; i < this.sigma.Length; i++)
			{
				if (sigma[i] > 0)
				{
					this.sigma[i] = Math.Sqrt(sigma2apost * this.sigma[i] * this.sigma[i]);
				}
			}
		}

		public virtual void setProbabilityValues(double pPrio, double pPost)
		{
			this.Pprio = pPrio;
			this.Ppost = pPost;
		}

		public virtual double Tprio
		{
			get
			{
				return this.Tprio < Point.ZERO?0.0:this.Tprio;
			}
		}

		public virtual double Tpost
		{
			get
			{
				return this.Tpost < Point.ZERO?0.0:this.Tpost;
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

		public virtual double[] InfluencesOnPointPosition
		{
			set
			{
				if (value.Length == this.Dimension)
				{
					this.ep = value;
				}
			}
		}

		public virtual double InfluenceOnPointPositionX
		{
			get
			{
				return this.ep[0];
			}
		}

		public virtual double InfluenceOnPointPositionY
		{
			get
			{
				return this.ep[1];
			}
		}

		public virtual double InfluenceOnPointPositionZ
		{
			get
			{
				return this.ep[this.Dimension - 1];
			}
		}

		public virtual double[] FirstPrincipalComponents
		{
			set
			{
				if (value.Length == this.Dimension)
				{
					this.principalComponents = value;
				}
			}
		}

		public virtual double FirstPrincipalComponentX
		{
			get
			{
				return this.principalComponents[0];
			}
		}

		public virtual double FirstPrincipalComponentY
		{
			get
			{
				return this.principalComponents[1];
			}
		}

		public virtual double FirstPrincipalComponentZ
		{
			get
			{
				return this.principalComponents[this.Dimension - 1];
			}
		}

		public virtual double[] Redundancy
		{
			set
			{
				if (value.Length == this.Dimension)
				{
					this.redundancy = value;
				}
			}
			get
			{
				double r = 0.0;
				for (int i = 0; i < this.redundancy.Length; i++)
				{
					r += this.redundancy[i];
				}
				return r;
			}
		}


		public virtual double RedundancyX
		{
			get
			{
				return this.redundancy[0];
			}
		}

		public virtual double RedundancyY
		{
			get
			{
				return this.redundancy[1];
			}
		}

		public virtual double RedundancyZ
		{
			get
			{
				return this.redundancy[this.Dimension - 1];
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
					this.mdb = value;
				}
			}
		}

		public virtual double MinimalDetectableBiasX
		{
			get
			{
				return this.mdb[0];
			}
		}

		public virtual double MinimalDetectableBiasY
		{
			get
			{
				return this.mdb[1];
			}
		}

		public virtual double MinimalDetectableBiasZ
		{
			get
			{
				return this.mdb[this.Dimension - 1];
			}
		}

		public virtual double[] MaximumTolerableBiases
		{
			set
			{
				if (value.Length == this.Dimension)
				{
					this.mtb = value;
				}
			}
		}

		public virtual double MaximumTolerableBiasX
		{
			get
			{
				return this.mtb[0];
			}
		}

		public virtual double MaximumTolerableBiasY
		{
			get
			{
				return this.mtb[1];
			}
		}

		public virtual double MaximumTolerableBiasZ
		{
			get
			{
				return this.mtb[this.Dimension - 1];
			}
		}

		public virtual double Omega
		{
			set
			{
				this.omega = value;
			}
			get
			{
				return this.omega;
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
    
		//			if (this.getDimension() > 1 && i < 2) 
		//				this.confidenceAxis2D[i] = value.getConfidenceAxis2D(i, false);
		//			else if (this.getDimension() == 1 && i == 0) // entspricht der Standardabweichung bei einem 1D-Punkt
		//				this.confidenceAxis2D[i] = Math.sqrt(Math.abs(value.getEigenvalue(i)));
				}
    
				if (this.Dimension > 1)
				{
					this.confidenceAngle2D = value.ConfidenceAngle2D;
					this.confidenceAngles = value.EulerAngles;
				}
			}
		}

		public virtual bool Significant
		{
			set
			{
				this.significant = value;
			}
			get
			{
				return this.significant;
			}
		}


		public virtual VerticalDeflectionX VerticalDeflectionX
		{
			get
			{
				return this.verticalDeflectionX;
			}
		}

		public virtual VerticalDeflectionY VerticalDeflectionY
		{
			get
			{
				return this.verticalDeflectionY;
			}
		}

		/// <summary>
		/// Returns <code>true</code>, if the deflections of the points are unknown parameters </summary>
		/// <returns> unknown </returns>
		public virtual bool hasUnknownDeflectionParameters()
		{
			return this.Dimension != 2 && (this.verticalDeflectionX.ColInJacobiMatrix >= 0 && this.verticalDeflectionY.ColInJacobiMatrix >= 0);
		}

		/// <summary>
		/// Returns <code>true</code>, if the deflections of the points are observed parameters </summary>
		/// <returns> observed </returns>
		public virtual bool hasObservedDeflectionParameters()
		{
			return this.Dimension != 2 && (this.verticalDeflectionX.RowInJacobiMatrix >= 0 && this.verticalDeflectionY.RowInJacobiMatrix >= 0);
		}

		/// <summary>
		/// Liefert die reinen geometrischen (festen) Anteile der Schiefstellung, wenn ein ellipsoidisches Modell verwendet wird </summary>
		/// <returns> deflections </returns>
		public virtual SphericalDeflectionParameter SphericalDeflectionParameter
		{
			get
			{
				return this.sphericalDeflectionParameter;
			}
		}
	}
}