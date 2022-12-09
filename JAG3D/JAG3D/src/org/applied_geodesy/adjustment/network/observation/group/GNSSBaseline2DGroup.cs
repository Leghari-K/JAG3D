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
	using GNSSBaseline2D = org.applied_geodesy.adjustment.network.observation.GNSSBaseline2D;
	using GNSSBaselineDeltaX2D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaX2D;
	using GNSSBaselineDeltaY2D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaY2D;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using AdditionalUnknownParameter = org.applied_geodesy.adjustment.network.parameter.AdditionalUnknownParameter;
	using RotationZ = org.applied_geodesy.adjustment.network.parameter.RotationZ;
	using Scale = org.applied_geodesy.adjustment.network.parameter.Scale;
	using Point = org.applied_geodesy.adjustment.network.point.Point;
	using Point2D = org.applied_geodesy.adjustment.network.point.Point2D;
	using PointGroup = org.applied_geodesy.adjustment.network.point.group.PointGroup;

	public class GNSSBaseline2DGroup : ObservationGroup
	{

		private Scale scale = new Scale();
		private RotationZ rz = new RotationZ();
		private bool hasApproxValues = false;
		private double scaleParam = 1.0, rzParam = 0.0;

		public GNSSBaseline2DGroup(int id) : this(id, DefaultUncertainty.UncertaintyGNSSZeroPointOffset, DefaultUncertainty.UncertaintyGNSSSquareRootDistanceDependent, DefaultUncertainty.UncertaintyGNSSDistanceDependent, Epoch.REFERENCE)
		{
		}

		public GNSSBaseline2DGroup(int id, double sigmaA, double sigmaB, double sigmaC, Epoch epoch) : base(id, sigmaA, sigmaB, sigmaC, epoch)
		{
			this.scale.ObservationGroup = this;
			this.rz.ObservationGroup = this;
		}

		public override void add(Observation gnss2D)
		{
			throw new System.NotSupportedException(this.GetType().Name + " Fehler, GPS-Basislinien bestehen nicht aus 2D-Beobachtungen!");
		}

		private void add(GNSSBaseline2D gnss2D)
		{
			gnss2D.Scale = this.scale;
			gnss2D.RotationZ = this.rz;
			base.add(gnss2D);
		}

		public virtual void add(GNSSBaselineDeltaX2D gpsX2D, GNSSBaselineDeltaY2D gpsY2D)
		{
			if (gpsX2D.Id == gpsY2D.Id && gpsX2D.StartPoint.Name.Equals(gpsY2D.StartPoint.Name) && gpsX2D.EndPoint.Name.Equals(gpsY2D.EndPoint.Name))
			{
				this.add(gpsX2D);
				this.add(gpsY2D);
				gpsX2D.addAssociatedBaselineComponent(gpsY2D);
				gpsY2D.addAssociatedBaselineComponent(gpsX2D);
			}
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

		public virtual RotationZ RotationZ
		{
			get
			{
				return this.rz;
			}
		}

		public override int numberOfAdditionalUnknownParameter()
		{
			int num = 0;
			if (this.rz.Enable)
			{
				num++;
			}
			if (this.scale.Enable)
			{
				num++;
			}
			return num;
		}

		public override AdditionalUnknownParameter setApproximatedValue(AdditionalUnknownParameter param)
		{
			if (!this.hasApproxValues)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double SQRT_EPS = Math.sqrt(org.applied_geodesy.adjustment.Constant.EPS);
				double SQRT_EPS = Math.Sqrt(Constant.EPS);
				PointGroup sourceSystem = new PointGroup(1);
				PointGroup targetSystem = new PointGroup(1);

				for (int i = 0; i < this.size(); i++)
				{
					GNSSBaseline baseline = (GNSSBaseline)this.get(i);
					string id = baseline.Id.ToString();

					Point trgPoint = targetSystem.get(id);
					if (trgPoint == null)
					{
						trgPoint = new Point2D(id,0,0);
						targetSystem.add(trgPoint);
					}

					if (baseline.Component == ComponentType.X)
					{
						trgPoint.X = baseline.ValueApriori;
					}
					else if (baseline.Component == ComponentType.Y)
					{
						trgPoint.Y = baseline.ValueApriori;
					}

					Point srcPoint = sourceSystem.get(id);
					if (srcPoint == null)
					{
						Point pS = baseline.StartPoint;
						Point pE = baseline.EndPoint;
						srcPoint = new Point2D(id,pE.X - pS.X,pE.Y - pS.Y);
						sourceSystem.add(srcPoint);
					}
				}

				double o = 0.0, a = 0.0, oa = 0.0;
				for (int i = 0; i < sourceSystem.size(); i++)
				{
					Point pS = sourceSystem.get(i);
					Point pT = targetSystem.get(pS.Name);
					if (pT == null)
					{
						continue;
					}

					o += pS.X * pT.Y - pS.Y * pT.X;
					a += pS.X * pT.X + pS.Y * pT.Y;

					oa += pS.X * pS.X + pS.Y * pS.Y;
				}

				if (oa > SQRT_EPS)
				{
					o /= oa;
					a /= oa;
					this.scaleParam = Math.hypot(o, a);
					this.scaleParam = this.scaleParam > SQRT_EPS ? this.scaleParam : 1.0;
					this.rzParam = MathExtension.MOD(-Math.Atan2(o, a), 2.0 * Math.PI);
				}
				else
				{
					this.scaleParam = 1.0;
				}

				this.hasApproxValues = true;
			}

			if (param == this.scale)
			{
				param.Value = this.scaleParam;
			}
			else if (param == this.rz)
			{
				param.Value = this.rzParam;
			}

			return param;
		}
	}

}