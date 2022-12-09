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

namespace org.applied_geodesy.jag3d.ui.graphic.sql
{
	public class PointPair
	{
		private GraphicPoint startPoint, endPoint;
		private bool significant = false, grossErrorExceeded = false;
		private double redundancy = 1.0, influenceOnPosition = 0.0, pPrio = 0;

		public PointPair(GraphicPoint startPoint, GraphicPoint endPoint)
		{
			this.startPoint = startPoint;
			this.endPoint = endPoint;
		}

		public virtual GraphicPoint StartPoint
		{
			get
			{
				return this.startPoint;
			}
		}

		public virtual GraphicPoint EndPoint
		{
			get
			{
				return this.endPoint;
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


		public virtual bool GrossErrorExceeded
		{
			get
			{
				return this.grossErrorExceeded;
			}
			set
			{
				this.grossErrorExceeded = value;
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
				this.redundancy = value;
			}
		}


		public virtual double InfluenceOnPosition
		{
			get
			{
				return this.influenceOnPosition;
			}
			set
			{
				this.influenceOnPosition = value;
			}
		}


		public virtual double Pprio
		{
			get
			{
				return this.pPrio;
			}
			set
			{
				this.pPrio = value;
			}
		}

	}

}