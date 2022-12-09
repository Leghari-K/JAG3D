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
	using DefaultUncertainty = org.applied_geodesy.adjustment.network.DefaultUncertainty;
	using Epoch = org.applied_geodesy.adjustment.network.Epoch;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using ZenithAngle = org.applied_geodesy.adjustment.network.observation.ZenithAngle;
	using RefractionCoefficient = org.applied_geodesy.adjustment.network.parameter.RefractionCoefficient;

	public class ZenithAngleGroup : ObservationGroup
	{
		private RefractionCoefficient refractionCoefficient = new RefractionCoefficient();

		public ZenithAngleGroup(int id) : base(id, DefaultUncertainty.UncertaintyAngleZeroPointOffset, DefaultUncertainty.UncertaintyAngleSquareRootDistanceDependent, DefaultUncertainty.UncertaintyAngleDistanceDependent, Epoch.REFERENCE)
		{
		}

		public ZenithAngleGroup(int id, double sigmaA, double sigmaB, double sigmaC, Epoch epoch) : base(id, sigmaA, sigmaB, sigmaC, epoch)
		{
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
				dist = observation.ApproximatedCalculatedDistance3D;
			}
			return dist > 0 ? this.StdB / Math.Sqrt(dist) : 0.0;
		}

		public override double getStdC(Observation observation)
		{
			double dist = observation.DistanceForUncertaintyModel;
			if (dist < Constant.EPS)
			{
				dist = observation.ApproximatedCalculatedDistance3D;
			}
			return dist > 0 ? this.StdC / dist : 0.0;
		}

		public override void add(Observation zenithangle)
		{
			ZenithAngle obs = (ZenithAngle)zenithangle;
			obs.setRefractionCoefficient(this.refractionCoefficient);
			base.add(obs);
		}

		public virtual RefractionCoefficient RefractionCoefficient
		{
			get
			{
				return this.refractionCoefficient;
			}
		}

		public override int numberOfAdditionalUnknownParameter()
		{
			return this.refractionCoefficient.Enable ? 1 : 0;
		}
	}

}