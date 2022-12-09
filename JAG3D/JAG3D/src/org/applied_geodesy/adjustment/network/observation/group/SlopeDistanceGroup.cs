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
	using SlopeDistance = org.applied_geodesy.adjustment.network.observation.SlopeDistance;
	using Scale = org.applied_geodesy.adjustment.network.parameter.Scale;
	using ZeroPointOffset = org.applied_geodesy.adjustment.network.parameter.ZeroPointOffset;

	public class SlopeDistanceGroup : ObservationGroup
	{

		private Scale scale = new Scale();
//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private ZeroPointOffset add_Conflict = new ZeroPointOffset();

		public SlopeDistanceGroup(int id) : base(id, DefaultUncertainty.UncertaintyDistanceZeroPointOffset, DefaultUncertainty.UncertaintyDistanceSquareRootDistanceDependent, DefaultUncertainty.UncertaintyDistanceDistanceDependent, Epoch.REFERENCE)
		{
		}

		public SlopeDistanceGroup(int id, double sigmaA, double sigmaB, double sigmaC, Epoch epoch) : base(id, sigmaA, sigmaB, sigmaC, epoch)
		{
		}

		public override void add(Observation distance3d)
		{
			SlopeDistance obs = (SlopeDistance)distance3d;
			obs.setScale(this.scale);
			obs.setZeroPointOffset(this.add_Conflict);
			base.add(obs);
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
			return this.StdB * Math.Sqrt(dist / 1000.0); // [km]
		}

		public override double getStdC(Observation observation)
		{
			double dist = observation.DistanceForUncertaintyModel;
			if (dist < Constant.EPS)
			{
				dist = observation.ApproximatedCalculatedDistance3D;
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

		public virtual ZeroPointOffset ZeroPointOffset
		{
			get
			{
				return this.add_Conflict;
			}
		}

		public override int numberOfAdditionalUnknownParameter()
		{
			int num = 0;
			if (this.add_Conflict.Enable)
			{
				num++;
			}
			if (this.scale.Enable)
			{
				num++;
			}
			return num;
		}
	}

}