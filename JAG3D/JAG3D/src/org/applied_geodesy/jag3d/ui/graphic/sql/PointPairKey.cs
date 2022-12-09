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
	public class PointPairKey
	{
		private string startPointName;
		private string endPointName;
		public PointPairKey(string startPointName, string endPointName)
		{
			bool interChange = string.CompareOrdinal(startPointName, endPointName) <= 0;
			this.startPointName = interChange ? startPointName : endPointName;
			this.endPointName = interChange ? endPointName : startPointName;
		}
		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((string.ReferenceEquals(endPointName, null)) ? 0 : endPointName.GetHashCode());
			result = prime * result + ((string.ReferenceEquals(startPointName, null)) ? 0 : startPointName.GetHashCode());
			return result;
		}
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			PointPairKey other = (PointPairKey) obj;
			if (!string.ReferenceEquals(this.endPointName, null) && !string.ReferenceEquals(other.endPointName, null) && this.endPointName.Equals(other.endPointName) && !string.ReferenceEquals(this.startPointName, null) && !string.ReferenceEquals(other.startPointName, null) && this.startPointName.Equals(other.startPointName))
			{
				return true;
			}
			return false;
		}
	}

}