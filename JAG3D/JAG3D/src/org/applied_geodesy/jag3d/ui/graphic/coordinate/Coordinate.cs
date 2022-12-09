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

namespace org.applied_geodesy.jag3d.ui.graphic.coordinate
{
	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;

	public abstract class Coordinate
	{
		private DoubleProperty x = new SimpleDoubleProperty(0.0);
		private DoubleProperty y = new SimpleDoubleProperty(0.0);

		internal Coordinate(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public virtual DoubleProperty xProperty()
		{
			return this.x;
		}

		public virtual double X
		{
			get
			{
				return this.xProperty().get();
			}
			set
			{
				this.xProperty().set(value);
			}
		}



		public virtual DoubleProperty yProperty()
		{
			return this.y;
		}

		public virtual double Y
		{
			get
			{
				return this.yProperty().get();
			}
			set
			{
				this.yProperty().set(value);
			}
		}


		public override string ToString()
		{
			return this.GetType().Name + ": [ " + this.X + " / " + this.Y + " ]";
		}
	}

}