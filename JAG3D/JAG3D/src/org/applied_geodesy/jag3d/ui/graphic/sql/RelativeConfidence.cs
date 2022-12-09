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
	public class RelativeConfidence : PointPair
	{
		private double minorAxis = 0.0, majorAxis = 0.0, angle = 0.0, deltaHeight = 0;

		public RelativeConfidence(GraphicPoint startPoint, GraphicPoint endPoint, double deltaHeight, double majorAxis, double minorAxis, double angle, bool significant) : base(startPoint, endPoint)
		{
			this.Significant = significant;
			this.majorAxis = majorAxis;
			this.minorAxis = minorAxis;
			this.angle = angle;
			this.deltaHeight = deltaHeight;
		}

		public virtual double MinorAxis
		{
			get
			{
				return this.minorAxis;
			}
			set
			{
				this.minorAxis = value;
			}
		}


		public virtual double MajorAxis
		{
			get
			{
				return this.majorAxis;
			}
			set
			{
				this.majorAxis = value;
			}
		}


		public virtual double Angle
		{
			get
			{
				return this.angle;
			}
			set
			{
				this.angle = value;
			}
		}


		public virtual double DeltaHeight
		{
			set
			{
				this.deltaHeight = value;
			}
			get
			{
				return this.deltaHeight;
			}
		}

	}

}