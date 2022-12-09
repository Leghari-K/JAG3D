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

namespace org.applied_geodesy.adjustment.geometry
{

	public class FeatureEvent : EventObject
	{
		private const long serialVersionUID = 4470566122319625334L;

		public enum FeatureEventType
		{
			FEATURE_ADDED,
			FEATURE_REMOVED
		}

		private readonly FeatureEventType type;
		internal FeatureEvent(Feature feature, FeatureEventType type) : base(feature)
		{
			this.type = type;
		}

		public override Feature Source
		{
			get
			{
				return (Feature)base.getSource();
			}
		}

		public virtual FeatureEventType EventType
		{
			get
			{
				return this.type;
			}
		}
	}
}