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

namespace org.applied_geodesy.adjustment.network.parameter
{
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using DefaultAverageThreshold = org.applied_geodesy.adjustment.network.DefaultAverageThreshold;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using Direction = org.applied_geodesy.adjustment.network.observation.Direction;
	using FaceType = org.applied_geodesy.adjustment.network.observation.FaceType;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;

	public class Orientation : AdditionalUnknownParameter
	{
		private bool estimateApproximationValue = true;

		public Orientation() : this(0.0, true)
		{
		}

		public Orientation(bool estimateApproximationValue) : this(0.0, estimateApproximationValue)
		{
		}

		public Orientation(double ori, bool estimateApproximationValue) : base(ori)
		{
			this.estimateApproximationValue = estimateApproximationValue;
		}

		public override ParameterType ParameterType
		{
			get
			{
				return ParameterType.ORIENTATION;
			}
		}

		public override int ColInJacobiMatrix
		{
			set
			{
				base.ColInJacobiMatrix = value;
				this.checkFace();
			}
		}

		private void checkFace()
		{
			if (this.Observations.size() <= 0 || !(this.Observations.get(0) is Direction))
			{
				return;
			}

			// Zuschlag zur ggf. bereits vorgegebenen Orientierung 
			double deltaOri = 0;

			if (this.Enable && this.estimateApproximationValue)
			{
				deltaOri = this.advancedOrientation();
			}

			int length = this.Observations.size();
			for (int i = 0; i < length; i++)
			{
				Direction dir = (Direction)this.Observations.get(i);
				// a-posteriori Wert beruecksichtigt bereits die vorgegebene a-priori Orientierung, 
				// sodass nur das Delta zu beruecksichtigen ist bei der a-priori Beobachtung
				double azimuthMeasuredFace1 = deltaOri + dir.ValueApriori;
				double azimuthCalculated = dir.ValueAposteriori;
				azimuthMeasuredFace1 = MathExtension.MOD(azimuthMeasuredFace1, 2.0 * Math.PI);
				double azimuthMeasuredFace2 = MathExtension.MOD(azimuthMeasuredFace1 + Math.PI, 2.0 * Math.PI);
				double face1 = Math.Min(Math.Abs(azimuthCalculated - azimuthMeasuredFace1), Math.Abs(Math.Abs(azimuthCalculated - azimuthMeasuredFace1) - 2.0 * Math.PI));
				double face2 = Math.Min(Math.Abs(azimuthCalculated - azimuthMeasuredFace2), Math.Abs(Math.Abs(azimuthCalculated - azimuthMeasuredFace2) - 2.0 * Math.PI));

				// Reduziere auf einheitliche Lage
				if (face1 > face2)
				{
					dir.ValueApriori = MathExtension.MOD(dir.ValueApriori + Math.PI, 2.0 * Math.PI);
					dir.Face = dir.Face == FaceType.ONE ? FaceType.TWO : FaceType.ONE;
				}
			}

			// Ermittle eine a-priori Orieniterung mittels der Lage-koorigierten Richtungen
			if (this.Enable && this.estimateApproximationValue)
			{
				this.Value = MathExtension.MOD(this.Value + this.advancedOrientation(), 2 * Math.PI);
			}
		}

		/// <summary>
		/// Bestimmung der genaeherten Orientierungsunbekannten
		/// durch einen Abriss des gemessenen Satzes.
		/// Um Einfluss von groben Messfehlern klein zu halten,
		/// wird der Median zurueck gegeben.
		/// </summary>
		/// <returns> advancedOrientation </returns>
		private double advancedOrientation()
		{
			int length = this.Observations.size();

			if (!(this.Observations.get(0) is Direction) || length == 0)
			{
				return 0.0;
			}

			double averageOrientation = 0;
			double maxUncertainty = double.Epsilon;
			double[] o = new double[length];
			for (int i = 0; i < length; i++)
			{
				Observation observation = this.Observations.get(i);
				double tmp_o = observation.ValueAposteriori - observation.ValueApriori;
				maxUncertainty = Math.Max(observation.StdApriori, maxUncertainty);
				tmp_o = MathExtension.MOD(tmp_o, 2.0 * Math.PI);
				if (i > 0 && (2.0 * Math.PI) - Math.Abs(o[i - 1] - tmp_o) < 0.5)
				{
					if (tmp_o < o[i - 1])
					{
						tmp_o += 2.0 * Math.PI;
					}
					else
					{
						tmp_o -= 2.0 * Math.PI;
					}
				}
				o[i] = tmp_o;
				averageOrientation += tmp_o;
			}

			Array.Sort(o);
			double medianOrientation = o[(int)((length - 1) / 2)];
			averageOrientation = averageOrientation / (double)length;
			maxUncertainty = Math.Max(DefaultAverageThreshold.ThresholdDirection, 100.0 * maxUncertainty);
			if (Math.Abs(averageOrientation - medianOrientation) < maxUncertainty)
			{
				return averageOrientation;
			}

			averageOrientation = 0;
			int count = 0;
			for (int i = 0; i < length; i++)
			{
				if (Math.Abs(o[i] - medianOrientation) < maxUncertainty)
				{
					averageOrientation += o[i];
					count++;
				}
			}

			if (count > 0)
			{
				return averageOrientation / (double)count;
			}

			return medianOrientation;
		}

		public override double ExpectationValue
		{
			get
			{
				return 0.0;
			}
		}

		public virtual bool EstimateApproximationValue
		{
			set
			{
				this.estimateApproximationValue = value;
			}
		}
	}
}