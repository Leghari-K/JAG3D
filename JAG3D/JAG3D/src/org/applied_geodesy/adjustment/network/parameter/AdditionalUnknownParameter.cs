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
	using ObservationGroup = org.applied_geodesy.adjustment.network.observation.group.ObservationGroup;

	public abstract class AdditionalUnknownParameter : UnknownParameter
	{
		private double value, tPrio = 0.0, tPost = 0.0, pPrio = 0.0, pPost = 0.0, nabla = 0.0, mdb = 0.0, confidence = 0.0;

		// Standardabweichung des Zusatzparameters; -1 == nicht gesetzt
		private double sigma = -1;
		private bool significant = false;
		private ObservationGroup observationGroup;

		public AdditionalUnknownParameter(double value)
		{
			this.value = value;
		}

		public virtual double Value
		{
			set
			{
				this.value = value;
			}
			get
			{
				return this.value;
			}
		}


		public virtual double Std
		{
			get
			{
				return this.sigma;
			}
			set
			{
				this.sigma = (value > 0)?value:-1.0;
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


		public virtual bool Enable
		{
			get
			{
				return this.ColInJacobiMatrix >= 0;
			}
		}

		public virtual double Tprio
		{
			set
			{
				this.tPrio = value;
			}
			get
			{
				return this.tPrio;
			}
		}

		public virtual double Tpost
		{
			set
			{
				this.tPost = value;
			}
			get
			{
				return this.tPost;
			}
		}

		public virtual double Pprio
		{
			set
			{
				this.pPrio = value;
			}
			get
			{
				return this.pPrio;
			}
		}

		public virtual double Ppost
		{
			set
			{
				this.pPost = value;
			}
			get
			{
				return this.pPost;
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


		public virtual double Confidence
		{
			get
			{
				return confidence;
			}
			set
			{
				this.confidence = value;
			}
		}


		public virtual ObservationGroup ObservationGroup
		{
			set
			{
				this.observationGroup = value;
			}
			get
			{
				return this.observationGroup;
			}
		}


		public override int ColInJacobiMatrix
		{
			set
			{
				base.ColInJacobiMatrix = value;
				if (value >= 0 && this.observationGroup != null)
				{
					this.observationGroup.ApproximatedValue = this;
				}
			}
		}

		public abstract double ExpectationValue {get;}

		public override string ToString()
		{
			return this.GetType().Name + ":  " + this.ParameterType + "  " + this.Value + "  " + this.Enable;
		}
	}

}