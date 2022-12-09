using System;

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
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using SymbolBuilder = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.SymbolBuilder;

	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Color = javafx.scene.paint.Color;

	public abstract class Layer : IdentifiableLayer
	{
		internal static readonly Properties PROPERTIES = new Properties();
		static Layer()
		{
			BufferedInputStream bis = null;
			const string path = "properties/layers.default";
			try
			{
				if (typeof(Layer).getClassLoader().getResourceAsStream(path) != null)
				{
					bis = new BufferedInputStream(typeof(Layer).getClassLoader().getResourceAsStream(path));
					PROPERTIES.load(bis);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				try
				{
					if (bis != null)
					{
						bis.close();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		private readonly LayerType layerType;
		private ObjectProperty<Color> color = new SimpleObjectProperty<Color>(Color.DARKBLUE);
		private DoubleProperty lineWidth = new SimpleDoubleProperty(1.0);
		private DoubleProperty symbolSize = new SimpleDoubleProperty(SymbolBuilder.DEFAULT_SIZE);
		private BooleanProperty visible = new SimpleBooleanProperty(true);

		internal I18N i18n = I18N.Instance;

		internal Layer(LayerType layerType)
		{
			this.layerType = layerType;
		}

		public abstract void draw(GraphicsContext graphicsContext, GraphicExtent graphicExtent);

		public abstract GraphicExtent MaximumGraphicExtent {get;}

		public abstract void clearLayer();

		public abstract void drawLegendSymbol(GraphicsContext graphicsContext, GraphicExtent graphicExtent, PixelCoordinate pixelCoordinate, double symbolHeight, double symbolWidth);

		public virtual bool hasContent()
		{
			 return this.MaximumGraphicExtent != null && this.MaximumGraphicExtent.ExtentHeight > 0 && this.MaximumGraphicExtent.ExtentWidth > 0 && !double.IsInfinity(this.MaximumGraphicExtent.ExtentHeight) && !double.IsInfinity(this.MaximumGraphicExtent.ExtentWidth) && !double.IsNaN(this.MaximumGraphicExtent.ExtentHeight) && !double.IsNaN(this.MaximumGraphicExtent.ExtentWidth);
		}

		public virtual LayerType LayerType
		{
			get
			{
				return this.layerType;
			}
		}

		public virtual ObjectProperty<Color> colorProperty()
		{
			return this.color;
		}

		public virtual Color Color
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


		public virtual DoubleProperty lineWidthProperty()
		{
			return this.lineWidth;
		}

		public virtual double LineWidth
		{
			get
			{
				return this.lineWidthProperty().get();
			}
			set
			{
				this.lineWidthProperty().set(value);
			}
		}


		public DoubleProperty symbolSizeProperty()
		{
			return this.symbolSize;
		}

		public double SymbolSize
		{
			get
			{
				return this.symbolSizeProperty().get();
			}
			set
			{
				this.symbolSizeProperty().set(value);
			}
		}


		public virtual bool contains(GraphicExtent graphicExtent, PixelCoordinate pixelCoordinate)
		{
			return pixelCoordinate.X >= 0 && pixelCoordinate.X <= graphicExtent.DrawingBoardWidth && pixelCoordinate.Y >= 0 && pixelCoordinate.Y <= graphicExtent.DrawingBoardHeight;
		}

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

	}
}