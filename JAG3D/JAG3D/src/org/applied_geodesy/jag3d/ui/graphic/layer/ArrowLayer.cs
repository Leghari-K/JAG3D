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
	using ArrowSymbolType = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.ArrowSymbolType;

	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using Color = javafx.scene.paint.Color;

	public abstract class ArrowLayer : Layer
	{
		private DoubleProperty vectorScale = new SimpleDoubleProperty(1.0);
		private ObjectProperty<ArrowSymbolType> symbolType = new SimpleObjectProperty<ArrowSymbolType>(ArrowSymbolType.FILLED_TETRAGON_ARROW);

		internal ArrowLayer(LayerType layerType) : base(layerType)
		{
			this.SymbolType = ArrowSymbolType.FILLED_TETRAGON_ARROW;
			this.Color = Color.DARKORANGE;
		}

		public virtual DoubleProperty vectorScaleProperty()
		{
			return this.vectorScale;
		}

		public virtual double VectorScale
		{
			get
			{
				return this.vectorScaleProperty().get();
			}
			set
			{
				this.vectorScaleProperty().set(value);
			}
		}


		public virtual ObjectProperty<ArrowSymbolType> symbolTypeProperty()
		{
			return this.symbolType;
		}

		public virtual ArrowSymbolType SymbolType
		{
			get
			{
				return this.symbolTypeProperty().get();
			}
			set
			{
				this.symbolTypeProperty().set(value);
			}
		}

	}

}