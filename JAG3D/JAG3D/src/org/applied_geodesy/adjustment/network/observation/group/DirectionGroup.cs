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
	using Direction = org.applied_geodesy.adjustment.network.observation.Direction;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using Orientation = org.applied_geodesy.adjustment.network.parameter.Orientation;

	public class DirectionGroup : ObservationGroup
	{

		private Orientation orientation = new Orientation(true);

		public DirectionGroup(int id) : this(id, DefaultUncertainty.UncertaintyAngleZeroPointOffset, DefaultUncertainty.UncertaintyAngleSquareRootDistanceDependent, DefaultUncertainty.UncertaintyAngleDistanceDependent, Epoch.REFERENCE)
		{
		}

		public DirectionGroup(int id, double sigmaA, double sigmaB, double sigmaC, Epoch epoch) : base(id, sigmaA, sigmaB, sigmaC, epoch)
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
				if (observation.StartPoint.Dimension == 3 && observation.EndPoint.Dimension == 3)
				{
					dist = observation.ApproximatedCalculatedDistance3D;
				}
				else
				{
					dist = observation.ApproximatedCalculatedDistance2D;
				}
			}
			return dist > 0 ? this.StdB / Math.Sqrt(dist) : 0.0;
		}

		public override double getStdC(Observation observation)
		{
			double dist = observation.DistanceForUncertaintyModel;
			if (dist < Constant.EPS)
			{
				if (observation.StartPoint.Dimension == 3 && observation.EndPoint.Dimension == 3)
				{
					dist = observation.ApproximatedCalculatedDistance3D;
				}
				else
				{
					dist = observation.ApproximatedCalculatedDistance2D;
				}
			}
			return dist > 0 ? this.StdC / dist : 0.0;
		}

		public virtual Orientation Orientation
		{
			get
			{
				return this.orientation;
			}
		}

		public override void add(Observation direction)
		{
			Direction obs = (Direction)direction;
			obs.setOrientation(this.orientation);
			base.add(obs);
		}

		public override int numberOfAdditionalUnknownParameter()
		{
			return this.orientation.Enable ? 1 : 0;
		}
	}

}