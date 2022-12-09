using System.Collections.Generic;

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

	using ObservationType = org.applied_geodesy.jag3d.ui.graphic.layer.ObservationSymbolProperties.ObservationType;

	public class ObservableMeasurement : PointPair
	{
		private ISet<ObservationType> observationTypesStartPoint = new LinkedHashSet<ObservationType>(10);
		private ISet<ObservationType> observationTypesEndPoint = new LinkedHashSet<ObservationType>(10);

		public ObservableMeasurement(GraphicPoint startPoint, GraphicPoint endPoint) : base(startPoint, endPoint)
		{
		}

		public virtual void addStartPointObservationType(ObservationType type)
		{
			this.observationTypesStartPoint.Add(type);
		}

		public virtual void addEndPointObservationType(ObservationType type)
		{
			this.observationTypesEndPoint.Add(type);
		}

		public virtual ISet<ObservationType> StartPointObservationType
		{
			get
			{
				return this.observationTypesStartPoint;
			}
		}

		public virtual ISet<ObservationType> EndPointObservationType
		{
			get
			{
				return this.observationTypesEndPoint;
			}
		}
	}

}