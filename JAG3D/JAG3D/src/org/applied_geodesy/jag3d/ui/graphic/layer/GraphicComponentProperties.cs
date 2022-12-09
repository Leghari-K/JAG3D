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

namespace org.applied_geodesy.jag3d.ui.graphic.layer
{
	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using Color = javafx.scene.paint.Color;

	public class GraphicComponentProperties
	{
		private BooleanProperty visible = new SimpleBooleanProperty(true);
		private ObjectProperty<Color> color = new SimpleObjectProperty<Color>(Color.BLACK);

		public BooleanProperty visibleProperty()
		{
			return this.visible;
		}

		public bool Visible
		{
			get
			{
				return this.visibleProperty().get();
			}
			set
			{
				this.visibleProperty().set(value);
			}
		}


		public ObjectProperty<Color> colorProperty()
		{
			return this.color;
		}

		public Color Color
		{
			get
			{
				return this.colorProperty().get();
			}
			set
			{
				this.colorProperty().set(value);
			}
		}

	}

}