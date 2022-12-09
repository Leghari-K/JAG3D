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

namespace org.applied_geodesy.adjustment.network.observation.group
{
	using Constant = org.applied_geodesy.adjustment.Constant;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using DefaultUncertainty = org.applied_geodesy.adjustment.network.DefaultUncertainty;
	using Epoch = org.applied_geodesy.adjustment.network.Epoch;
	using ComponentType = org.applied_geodesy.adjustment.network.observation.ComponentType;
	using GNSSBaseline = org.applied_geodesy.adjustment.network.observation.GNSSBaseline;
	using GNSSBaselineDeltaZ1D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaZ1D;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using AdditionalUnknownParameter = org.applied_geodesy.adjustment.network.parameter.AdditionalUnknownParameter;
	using RotationX = org.applied_geodesy.adjustment.network.parameter.RotationX;
	using RotationY = org.applied_geodesy.adjustment.network.parameter.RotationY;
	using Scale = org.applied_geodesy.adjustment.network.parameter.Scale;
	using Point = org.applied_geodesy.adjustment.network.point.Point;
	using Point1D = org.applied_geodesy.adjustment.network.point.Point1D;
	using Point3D = org.applied_geodesy.adjustment.network.point.Point3D;
	using PointGroup = org.applied_geodesy.adjustment.network.point.group.PointGroup;

	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;


	public class GNSSBaseline1DGroup : ObservationGroup
	{

		private Scale scale = new Scale();
		private RotationX rx = new RotationX();
		private RotationY ry = new RotationY();

		private bool hasApproxValues = false;
		private double scaleParam = 1.0, rxParam = 0.0, ryParam = 0.0;

		public GNSSBaseline1DGroup(int id) : this(id, DefaultUncertainty.UncertaintyGNSSZeroPointOffset, DefaultUncertainty.UncertaintyGNSSSquareRootDistanceDependent, DefaultUncertainty.UncertaintyGNSSDistanceDependent, Epoch.REFERENCE)
		{
		}

		public GNSSBaseline1DGroup(int id, double sigmaA, double sigmaB, double sigmaC, Epoch epoch) : base(id, sigmaA, sigmaB, sigmaC, epoch)
		{
			this.scale.ObservationGroup = this;
			this.rx.ObservationGroup = this;
			this.ry.ObservationGroup = this;
		}

		public override void add(Observation gnss1D)
		{
			GNSSBaselineDeltaZ1D gnssZ1D = (GNSSBaselineDeltaZ1D)gnss1D;
			this.add(gnssZ1D);
		}

		public virtual void add(GNSSBaselineDeltaZ1D gnssZ1D)
		{
			gnssZ1D.Scale = this.scale;
			gnssZ1D.RotationX = this.rx;
			gnssZ1D.RotationY = this.ry;
			base.add(gnssZ1D);
		}

		public override double getStdA(Observation observation)
		{
			return this.StdA;
		}

		public override double getStdB(Observation observation)
		{
			double dist = observation.DistanceForUncertaintyModel;
			if (dist < Constant.EPS)
			{
				dist = Math.Abs(observation.ValueApriori); // should be equal to distance for uncertainty model
			}
			return this.StdB * Math.Sqrt(dist / 1000.0); // [km]
		}

		public override double getStdC(Observation observation)
		{
			double dist = observation.DistanceForUncertaintyModel;
			if (dist < Constant.EPS)
			{
				dist = Math.Abs(observation.ValueApriori); // should be equal to distance for uncertainty model
			}
			return this.StdC * dist;
		}

		public virtual Scale Scale
		{
			get
			{
				return this.scale;
			}
		}

		public virtual RotationX RotationX
		{
			get
			{
				return this.rx;
			}
		}

		public virtual RotationY RotationY
		{
			get
			{
				return this.ry;
			}
		}

		public override int numberOfAdditionalUnknownParameter()
		{
			int num = 0;
			if (this.rx.Enable)
			{
				num++;
			}
			if (this.ry.Enable)
			{
				num++;
			}
			if (this.scale.Enable)
			{
				num++;
			}
			return num;
		}

		// fz = m*(  Math.sin(ry)*dX - Math.sin(rx)*Math.cos(ry)*dY + Math.cos(rx)*Math.cos(ry)*dZ)
		// fz = m*(  A*dX + B*dY + C*dZ )
		public override AdditionalUnknownParameter setApproximatedValue(AdditionalUnknownParameter param)
		{
			if (!this.hasApproxValues)
			{
				PointGroup sourceSystem = new PointGroup(1);
				PointGroup targetSystem = new PointGroup(1);

				for (int i = 0; i < this.size(); i++)
				{
					GNSSBaseline baseline = (GNSSBaseline)this.get(i);
					string id = baseline.Id.ToString();

					Point trgPoint = targetSystem.get(id);
					if (trgPoint == null)
					{
						trgPoint = new Point1D(id,0);
						targetSystem.add(trgPoint);
					}

					if (baseline.Component == ComponentType.Z)
					{
						trgPoint.Z = baseline.ValueApriori + baseline.EndPointHeight - baseline.StartPointHeight;
					}

					Point srcPoint = sourceSystem.get(id);
					if (srcPoint == null)
					{
						Point pS = baseline.StartPoint;
						Point pE = baseline.EndPoint;
						srcPoint = new Point3D(id,pE.X - pS.X,pE.Y - pS.Y,pE.Z - pS.Z);
						sourceSystem.add(srcPoint);
					}
				}
				UpperSymmPackMatrix F = new UpperSymmPackMatrix(3);
				DenseVector f = new DenseVector(3);
				DenseVector x = new DenseVector(3);
				for (int k = 0; k < sourceSystem.size(); k++)
				{
					Point pS = sourceSystem.get(k);
					Point pT = targetSystem.get(pS.Name);
					if (pT == null)
					{
						continue;
					}

					double dx = pS.X;
					double dy = pS.Y;
					double dz = pS.Z;

					F.add(0, 0, dx * dx);
					F.add(0, 1, dx * dy);
					F.add(0, 2, dx * dz);

					F.add(1, 1, dy * dy);
					F.add(1, 2, dy * dz);

					F.add(2, 2, dz * dz);

					f.add(0, dx * pT.Z);
					f.add(1, dy * pT.Z);
					f.add(2, dz * pT.Z);
				}

				try
				{
					MathExtension.pinv(F, -1).mult(f, x);
					this.rxParam = Math.Atan2(x.get(1), x.get(2));
					this.ryParam = Math.Atan2(x.get(0) * Math.Cos(this.rxParam), x.get(2));
					this.scaleParam = Math.Cos(rxParam) * Math.Cos(ryParam) > Math.Sqrt(Constant.EPS) ? x.get(2) / Math.Cos(rxParam) * Math.Cos(ryParam) : 1.0;
				}
				catch (NotConvergedException)
				{
					this.rxParam = 0.0;
					this.ryParam = 0.0;
					this.scaleParam = 1.0;
				}

				this.hasApproxValues = true;
			}

			if (param == this.scale)
			{
				param.Value = this.scaleParam;
			}
			else if (param == this.rx)
			{
				param.Value = this.rxParam;
			}
			else if (param == this.ry)
			{
				param.Value = this.ryParam;
			}

			return param;
		}
	}

}