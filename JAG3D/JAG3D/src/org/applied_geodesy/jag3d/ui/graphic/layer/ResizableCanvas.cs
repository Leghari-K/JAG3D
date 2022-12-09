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
	using PixelCoordinate = org.applied_geodesy.jag3d.ui.graphic.coordinate.PixelCoordinate;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using Orientation = javafx.geometry.Orientation;
	using Canvas = javafx.scene.canvas.Canvas;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;

	internal class ResizableCanvas : Canvas
	{

		private class ResizingListener : ChangeListener<Number>
		{
			private readonly ResizableCanvas outerInstance;

			internal Orientation orientation;

			internal ResizingListener(ResizableCanvas outerInstance, Orientation orientation)
			{
				this.outerInstance = outerInstance;
				this.orientation = orientation;
			}
			public override void changed<T1>(ObservableValue<T1> observable, Number oldValue, Number newValue) where T1 : Number
			{
				double extentWidth = outerInstance.currentGraphicExtent.ExtentWidth;
				double extentHeight = outerInstance.currentGraphicExtent.ExtentHeight;

				double drawingBoardHeight = outerInstance.currentGraphicExtent.DrawingBoardHeight;
				double drawingBoardWidth = outerInstance.currentGraphicExtent.DrawingBoardWidth;

				switch (this.orientation)
				{
				case HORIZONTAL:
					drawingBoardWidth = oldValue.doubleValue() > newValue.doubleValue() ? oldValue.doubleValue() : newValue.doubleValue();
					break;
				case VERTICAL:
					drawingBoardHeight = oldValue.doubleValue() > newValue.doubleValue() ? oldValue.doubleValue() : newValue.doubleValue();
					break;
				}
				outerInstance.currentGraphicExtent.Scale = GraphicExtent.getScale(drawingBoardHeight, drawingBoardWidth, extentHeight, extentWidth);
				outerInstance.draw();
			}
		}

		private readonly GraphicExtent currentGraphicExtent;
		private ObservableList<Layer> layers = FXCollections.observableArrayList();

		internal ResizableCanvas(GraphicExtent currentGraphicExtent)
		{
			this.currentGraphicExtent = currentGraphicExtent;

			this.widthProperty().bind(this.currentGraphicExtent.drawingBoardWidthProperty());
			this.heightProperty().bind(this.currentGraphicExtent.drawingBoardHeightProperty());

			this.widthProperty().addListener(new ResizingListener(this, Orientation.HORIZONTAL));
			this.heightProperty().addListener(new ResizingListener(this, Orientation.VERTICAL));
		}

		public virtual ObservableList<Layer> Layers
		{
			get
			{
				return this.layers;
			}
		}

		internal virtual void draw()
		{
			this.clear();

			GraphicsContext graphicsContext = getGraphicsContext2D();

			foreach (Layer layer in this.layers)
			{
				if (!layer.Visible)
				{
					continue;
				}
				layer.draw(graphicsContext, this.CurrentGraphicExtent);
			}
		}

		internal virtual void clear()
		{
			double width = this.getWidth();
			double height = this.getHeight();

			GraphicsContext gc = this.getGraphicsContext2D();
			// remove old Graphic
			gc.clearRect(0, 0, width, height);
		}

		public virtual GraphicExtent CurrentGraphicExtent
		{
			get
			{
				return this.currentGraphicExtent;
			}
		}

		public override bool Resizable
		{
			get
			{
				return true;
			}
		}

		public override double prefWidth(double height)
		{
			return getWidth();
		}

		public override double prefHeight(double width)
		{
			return getHeight();
		}

		public virtual bool contains(PixelCoordinate pixelCoordinate)
		{
			return pixelCoordinate.X >= 0 && pixelCoordinate.X <= this.getWidth() && pixelCoordinate.Y >= 0 && pixelCoordinate.Y <= this.getHeight();
		}
	}

}