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

namespace org.applied_geodesy.adjustment.network.observation
{
	using Constant = org.applied_geodesy.adjustment.Constant;
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using ObservationGroup = org.applied_geodesy.adjustment.network.observation.group.ObservationGroup;
	using ProjectionType = org.applied_geodesy.adjustment.network.observation.reduction.ProjectionType;
	using Reduction = org.applied_geodesy.adjustment.network.observation.reduction.Reduction;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	public abstract class Observation
	{
	//	class SphericalDeflectionParameters {
	//		private double rxs = 0, rys = 0, rxe = 0, rye = 0;
	//		
	//		private SphericalDeflectionParameters(Observation observation) {
	//			this(observation, Boolean.FALSE); 
	//		}
	//		
	//		private SphericalDeflectionParameters(Observation observation, boolean aprioriValues) {
	//			this.deriveSphericalDeflectionParameters(observation, aprioriValues); 
	//		}
	//		
	//		private void deriveSphericalDeflectionParameters(Observation observation, boolean aprioriValues) {
	//			if (!aprioriValues) {
	//				this.rxs = observation.getStartPoint().getSphericalDeflectionParameter().getSphericalDeflectionX();
	//				this.rys = observation.getStartPoint().getSphericalDeflectionParameter().getSphericalDeflectionY();
	//
	//				this.rxe = observation.getEndPoint().getSphericalDeflectionParameter().getSphericalDeflectionX();
	//				this.rye = observation.getEndPoint().getSphericalDeflectionParameter().getSphericalDeflectionY();
	//			}
	//			else {
	//			
	//			Reduction reductions = observation.getReductions();
	//
	//			if (reductions == null || reductions.getProjectionType() != ProjectionType.LOCAL_ELLIPSOIDAL)
	//				return;
	//
	//			double xs = aprioriValues ? observation.getStartPoint().getX0() : observation.getStartPoint().getX();
	//			double ys = aprioriValues ? observation.getStartPoint().getY0() : observation.getStartPoint().getY();
	//			//double zs = aprioriValues ? observation.getStartPoint().getZ0() : observation.getStartPoint().getZ();
	//
	//			double xe = aprioriValues ? observation.getEndPoint().getX0() : observation.getEndPoint().getX();
	//			double ye = aprioriValues ? observation.getEndPoint().getY0() : observation.getEndPoint().getY();
	//			//double ze = aprioriValues ? observation.getEndPoint().getZ0() : observation.getEndPoint().getZ();
	//
	//			double x0 = reductions.getPivotPoint().getX0();
	//			double y0 = reductions.getPivotPoint().getY0();
	//			double z0 = reductions.getPivotPoint().getZ0();
	//
	//			double R0 = reductions.getEarthRadius();
	//			double h0 = reductions.getReferenceHeight();
	//
	//			double R = R0 + h0 - z0;
	//
	//			this.rxs =  (ys - y0) / R;
	//			this.rys = -(xs - x0) / R;
	//
	//			this.rxe =  (ye - y0) / R;
	//			this.rye = -(xe - x0) / R;
	//			
	//			}
	//		}
	//		
	//		public double getStartPointSphericalDeflectionX() {
	//			return this.rxs;
	//		}
	//		
	//		public double getStartPointSphericalDeflectionY() {
	//			return this.rys;
	//		}
	//		
	//		public double getEndPointSphericalDeflectionX() {
	//			return this.rxe;
	//		}
	//		
	//		public double getEndPointSphericalDeflectionY() {
	//			return this.rye;
	//		}
	//		
	//		@Override
	//		public String toString() {
	//			return "SphericalDeflectionParameters [rxs=" + rxs + ", rys=" + rys + ", rxe=" + rxe + ", rye=" + rye + "]";
	//		}
	//		
	//	}
		// ID der Beobachtung
		private readonly int obsID;

		// Start und Endpunkt der betrefflichen Beobachtung	
		private readonly Point startPoint, endPoint;

		// Stand- und Zielpunkthoehe
		private double startPointHeight = 0.0, endPointHeight = 0.0, observation = 0.0;

		// Grenzwert fuer Null
		private static readonly double ZERO = Math.Sqrt(Constant.EPS);

		//Beobachtungsgruppe (erste)
		private ObservationGroup observationGroup = null;

		// Projektion der Beobachtung
		private Reduction reductions = new Reduction();

		private double redundancy = 0.0, sigma = -1.0, sigma0 = -1.0, nabla = 0.0, Tprio = 0.0, Tpost = 0.0, Pprio = 0.0, Ppost = 0.0, omega = 0.0, mdb = 0.0, mtb = 0.0, ep = 0.0, ef = 0.0;

		// Zeile in A-Matrix der Beobachtung; -1 == nicht gesetzt
		private int rowInJacobiMatrix = -1;

//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool significant = false, useGroupUncertainty_Conflict = false;
		private double distanceForUncertaintyModel = -1;
		public Observation(int id, Point startPoint, Point endPoint, double startPointHeight, double endPointHeight, double observation, double sigma, double distanceForUncertaintyModel)
		{
			if (startPoint.Name.Equals(endPoint.Name))
			{
				throw new System.ArgumentException("Fehler, Start- und Zielpunkt sind identisch. " + startPoint.Name + " / " + endPoint.Name);
			}
			this.obsID = id;
			this.startPoint = startPoint;
			this.endPoint = endPoint;
			this.startPointHeight = startPointHeight;
			this.endPointHeight = endPointHeight;
			this.observation = observation;
			this.Std = sigma;

			this.startPoint.Observation = this;
			this.endPoint.Observation = this;
			this.distanceForUncertaintyModel = distanceForUncertaintyModel > Constant.EPS ? distanceForUncertaintyModel : -1;
		}

		public virtual int Id
		{
			get
			{
				return this.obsID;
			}
		}

		public virtual Point StartPoint
		{
			get
			{
				return this.startPoint;
			}
		}

		public virtual Point EndPoint
		{
			get
			{
				return this.endPoint;
			}
		}

		public virtual double StartPointHeight
		{
			get
			{
				return this.startPointHeight;
			}
		}

		public virtual double EndPointHeight
		{
			get
			{
				return this.endPointHeight;
			}
		}

		public virtual double ValueApriori
		{
			get
			{
				return this.observation;
			}
			set
			{
				this.observation = value;
			}
		}


		public abstract double ValueAposteriori {get;}

		public virtual double Std
		{
			get
			{
				if (this.sigma > 0)
				{
					return this.sigma;
				}
				return -1.0;
			}
			set
			{
				this.sigma0 = (this.sigma0 <= 0 && value>0)?value:this.sigma0;
				this.sigma = (value > 0)?value:-1;
			}
		}

		public virtual double StdApriori
		{
			get
			{
				if (this.sigma0 > 0)
				{
					return this.sigma0;
				}
				return -1.0;
			}
			set
			{
				if (value > 0)
				{
					this.sigma0 = value;
				}
			}
		}




		public virtual double Correction
		{
			get
			{
				return this.ValueApriori - this.ValueAposteriori;
			}
		}

		public virtual double CalculatedDistance2D
		{
			get
			{
				double xs = this.startPoint.X;
				double ys = this.startPoint.Y;
				double zs = this.startPoint.Z;
    
				double xe = this.endPoint.X;
				double ye = this.endPoint.Y;
				double ze = this.endPoint.Z;
    
				double th = this.endPointHeight;
    
				double rxs = this.startPoint.VerticalDeflectionX.Value;
				double rys = this.startPoint.VerticalDeflectionY.Value;
    
				double rxe = this.endPoint.VerticalDeflectionX.Value;
				double rye = this.endPoint.VerticalDeflectionY.Value;
    
				if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
				{
					rxs += this.startPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rys += this.startPoint.SphericalDeflectionParameter.SphericalDeflectionY;
    
					rxe += this.endPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rye += this.endPoint.SphericalDeflectionParameter.SphericalDeflectionY;
				}
    
				double srxs = Math.Sin(rxs);
				double srys = Math.Sin(rys);
				double crxs = Math.Cos(rxs);
				double crys = Math.Cos(rys);
    
				double crxe = Math.Cos(rxe);
				double crye = Math.Cos(rye);
				double srye = Math.Sin(rye);
				double srxe = Math.Sin(rxe);
    
				double u = th * (crxe * crye * srys - crxe * crys * srye) + crys * (xe - xs) + srys * (ze - zs);
				double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
    
				return Math.Sqrt(u * u + v * v);
			}
		}

		public virtual double ApproximatedCalculatedDistance2D
		{
			get
			{
				double xs = this.startPoint.X0;
				double ys = this.startPoint.Y0;
				double zs = this.startPoint.Z0;
    
				double xe = this.endPoint.X0;
				double ye = this.endPoint.Y0;
				double ze = this.endPoint.Z0;
    
				double th = this.endPointHeight;
    
				double rxs = this.startPoint.VerticalDeflectionX.Value0;
				double rys = this.startPoint.VerticalDeflectionY.Value0;
    
				double rxe = this.endPoint.VerticalDeflectionX.Value0;
				double rye = this.endPoint.VerticalDeflectionY.Value0;
    
				if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
				{
					rxs += this.startPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rys += this.startPoint.SphericalDeflectionParameter.SphericalDeflectionY;
    
					rxe += this.endPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rye += this.endPoint.SphericalDeflectionParameter.SphericalDeflectionY;
				}
    
				double srxs = Math.Sin(rxs);
				double srys = Math.Sin(rys);
				double crxs = Math.Cos(rxs);
				double crys = Math.Cos(rys);
    
				double crxe = Math.Cos(rxe);
				double crye = Math.Cos(rye);
				double srye = Math.Sin(rye);
				double srxe = Math.Sin(rxe);
    
				double u = th * (crxe * crye * srys - crxe * crys * srye) + crys * (xe - xs) + srys * (ze - zs);
				double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
    
				return Math.Sqrt(u * u + v * v);
			}
		}

		public virtual double CalculatedDistance3D
		{
			get
			{
				double xs = this.startPoint.X;
				double ys = this.startPoint.Y;
				double zs = this.startPoint.Z;
    
				double xe = this.endPoint.X;
				double ye = this.endPoint.Y;
				double ze = this.endPoint.Z;
    
				double ih = this.startPointHeight;
				double th = this.endPointHeight;
    
				double rxs = this.startPoint.VerticalDeflectionX.Value;
				double rys = this.startPoint.VerticalDeflectionY.Value;
    
				double rxe = this.endPoint.VerticalDeflectionX.Value;
				double rye = this.endPoint.VerticalDeflectionY.Value;
    
				if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
				{
					rxs += this.startPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rys += this.startPoint.SphericalDeflectionParameter.SphericalDeflectionY;
    
					rxe += this.endPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rye += this.endPoint.SphericalDeflectionParameter.SphericalDeflectionY;
				}
    
				double srxs = Math.Sin(rxs);
				double srys = Math.Sin(rys);
				double crxs = Math.Cos(rxs);
				double crys = Math.Cos(rys);
    
				double crxe = Math.Cos(rxe);
				double crye = Math.Cos(rye);
				double srye = Math.Sin(rye);
				double srxe = Math.Sin(rxe);
    
				double u = th * (crxe * crye * srys - crxe * crys * srye) + crys * (xe - xs) + srys * (ze - zs);
				double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
				double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);
    
				return Math.Sqrt(u * u + v * v + w * w);
			}
		}

		public virtual double ApproximatedCalculatedDistance3D
		{
			get
			{
				double xs = this.startPoint.X0;
				double ys = this.startPoint.Y0;
				double zs = this.startPoint.Z0;
    
				double xe = this.endPoint.X0;
				double ye = this.endPoint.Y0;
				double ze = this.endPoint.Z0;
    
				double ih = this.startPointHeight;
				double th = this.endPointHeight;
    
				double rxs = this.startPoint.VerticalDeflectionX.Value0;
				double rys = this.startPoint.VerticalDeflectionY.Value0;
    
				double rxe = this.endPoint.VerticalDeflectionX.Value0;
				double rye = this.endPoint.VerticalDeflectionY.Value0;
    
				if (this.Reductions.ProjectionType == ProjectionType.LOCAL_ELLIPSOIDAL)
				{
					rxs += this.startPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rys += this.startPoint.SphericalDeflectionParameter.SphericalDeflectionY;
    
					rxe += this.endPoint.SphericalDeflectionParameter.SphericalDeflectionX;
					rye += this.endPoint.SphericalDeflectionParameter.SphericalDeflectionY;
				}
    
				double srxs = Math.Sin(rxs);
				double srys = Math.Sin(rys);
				double crxs = Math.Cos(rxs);
				double crys = Math.Cos(rys);
    
				double crxe = Math.Cos(rxe);
				double crye = Math.Cos(rye);
				double srye = Math.Sin(rye);
				double srxe = Math.Sin(rxe);
    
				double u = th * (crxe * crye * srys - crxe * crys * srye) + crys * (xe - xs) + srys * (ze - zs);
				double v = crxs * (ye - ys) - th * (crxe * crye * crys * srxs - crxs * srxe + crxe * srxs * srye * srys) - crys * srxs * (ze - zs) + srxs * srys * (xe - xs);
				double w = th * (srxe * srxs + crxe * crxs * crye * crys + crxe * crxs * srye * srys) - ih + srxs * (ye - ys) + crxs * crys * (ze - zs) - crxs * srys * (xe - xs);
    
				return Math.Sqrt(u * u + v * v + w * w);
			}
		}

		public virtual ObservationGroup ObservationGroup
		{
			set
			{
				if (value.Id >= 0 && this.observationGroup == null)
				{
					this.observationGroup = value;
				}
			}
			get
			{
				return this.observationGroup;
			}
		}


		public abstract double diffXs();

		public abstract double diffYs();

		public abstract double diffZs();

		public abstract double diffVerticalDeflectionXs();

		public abstract double diffVerticalDeflectionYs();

		public abstract double diffVerticalDeflectionXe();

		public abstract double diffVerticalDeflectionYe();

		public virtual double diffXe()
		{
			return -this.diffXs();
		}

		public virtual double diffYe()
		{
			return -this.diffYs();
		}

		public virtual double diffZe()
		{
			return -this.diffZs();
		}

		public virtual double diffOri()
		{
			return 0.0;
		}

		public virtual double diffScale()
		{
			return 0.0;
		}

		public virtual double diffAdd()
		{
			return 0.0;
		}

		public virtual double diffRefCoeff()
		{
			return 0.0;
		}

		public virtual double diffRotX()
		{
			return 0.0;
		}

		public virtual double diffRotY()
		{
			return 0.0;
		}

		public virtual double diffRotZ()
		{
			return 0.0;
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


		public virtual int ColInJacobiMatrixFromScale
		{
			get
			{
				return -1;
			}
		}

		public virtual int ColInJacobiMatrixFromAdd
		{
			get
			{
				return -1;
			}
		}

		public virtual int ColInJacobiMatrixFromRefCoeff
		{
			get
			{
				return -1;
			}
		}

		public virtual int ColInJacobiMatrixFromOrientation
		{
			get
			{
				return -1;
			}
		}

		public virtual int ColInJacobiMatrixFromRotationX
		{
			get
			{
				return -1;
			}
		}

		public virtual int ColInJacobiMatrixFromRotationY
		{
			get
			{
				return -1;
			}
		}

		public virtual int ColInJacobiMatrixFromRotationZ
		{
			get
			{
				return -1;
			}
		}

		public virtual double Redundancy
		{
			get
			{
				return this.redundancy;
			}
			set
			{
				if (0 <= value && value <= 1)
				{
					this.redundancy = value;
				}
			}
		}


		public virtual void setTestAndProbabilityValues(double tPrio, double tPost, double pPrio, double pPost)
		{
			this.Tprio = tPrio;
			this.Tpost = tPost;

			this.Pprio = pPrio;
			this.Ppost = pPost;
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

		public virtual double GrossError
		{
			get
			{
				return this.nabla;
			}
			set
			{
				this.nabla = value;
			}
		}


		public virtual double InfluenceOnPointPosition
		{
			set
			{
				this.ep = value;
			}
			get
			{
				return this.ep;
			}
		}


		public virtual double InfluenceOnNetworkDistortion
		{
			set
			{
				this.ef = value;
			}
			get
			{
				return this.ef;
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


		public virtual bool useGroupUncertainty()
		{
			return this.useGroupUncertainty_Conflict;
		}

		public virtual void useGroupUncertainty(bool useGroupUncertainty)
		{
			this.useGroupUncertainty_Conflict = useGroupUncertainty;
		}

		public virtual double MinimalDetectableBias
		{
			get
			{
				return this.mdb;
			}
			set
			{
				this.mdb = value;
			}
		}


		public virtual double MaximumTolerableBias
		{
			get
			{
				return this.mtb;
			}
			set
			{
				this.mtb = value;
			}
		}


		public abstract ObservationType ObservationType {get;}

		public virtual Reduction Reductions
		{
			get
			{
				return this.reductions;
			}
		}

		public virtual Reduction Reduction
		{
			set
			{
				this.reductions = value;
			}
		}

		public virtual double DistanceForUncertaintyModel
		{
			get
			{
				return this.distanceForUncertaintyModel > Constant.EPS ? this.distanceForUncertaintyModel : 0.0;
			}
			set
			{
				this.distanceForUncertaintyModel = value > Constant.EPS ? value : -1;
			}
		}


		public override string ToString()
		{
			return this.GetType().Name + ": " + this.startPoint.Name + " - " + this.endPoint.Name + ": " + this.observation;
		}
	}

}