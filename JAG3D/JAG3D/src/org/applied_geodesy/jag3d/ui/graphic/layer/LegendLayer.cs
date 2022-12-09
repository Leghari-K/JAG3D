using System;
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

namespace org.applied_geodesy.jag3d.ui.graphic.layer
{

	using PixelCoordinate = org.applied_geodesy.jag3d.ui.graphic.coordinate.PixelCoordinate;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;

	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using SimpleStringProperty = javafx.beans.property.SimpleStringProperty;
	using StringProperty = javafx.beans.property.StringProperty;
	using ObservableList = javafx.collections.ObservableList;
	using Bounds = javafx.geometry.Bounds;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Color = javafx.scene.paint.Color;
	using Rectangle = javafx.scene.shape.Rectangle;
	using Shape = javafx.scene.shape.Shape;
	using StrokeLineCap = javafx.scene.shape.StrokeLineCap;
	using Font = javafx.scene.text.Font;
	using FontPosture = javafx.scene.text.FontPosture;
	using FontWeight = javafx.scene.text.FontWeight;
	using Text = javafx.scene.text.Text;

	public class LegendLayer : Layer, FontLayer
	{
		private ObservableList<Layer> layers;

		private DoubleProperty fontSize = new SimpleDoubleProperty(10);
		private StringProperty fontFamily = new SimpleStringProperty(Font.getDefault().getFamily());
		private ObjectProperty<Color> fontColor = new SimpleObjectProperty<Color>(Color.SLATEGREY);
		private ObjectProperty<LegendPositionType> legendPositionType = new SimpleObjectProperty<LegendPositionType>(LegendPositionType.NORTH_EAST);

		internal LegendLayer(LayerType layerType, ObservableList<Layer> layers) : base(layerType)
		{
			if (layerType != LayerType.LEGEND)
			{
				throw new System.ArgumentException("Error, unsupported layer type " + layerType);
			}
			this.layers = layers;

			Color symbolColor, fontColor;
			string fontFamily = null;
			LegendPositionType legendPositionType;
			double lineWidth = -1, fontSize = -1;
			bool visible = true;

			fontFamily = PROPERTIES.getProperty("LEGEND_FONT_FAMILY", "System");
			try
			{
				fontColor = Color.web(PROPERTIES.getProperty("LEGEND_FONT_COLOR", "#2F4F4F"));
			}
			catch (Exception)
			{
				fontColor = Color.web("#2F4F4F");
			}

			try
			{
				symbolColor = Color.web(PROPERTIES.getProperty("LEGEND_FONT_COLOR", "#778899"));
			}
			catch (Exception)
			{
				symbolColor = Color.web("#778899");
			}

			try
			{
				legendPositionType = LegendPositionType.valueOf(PROPERTIES.getProperty("LEGEND_POSITION", "NORTH_EAST"));
			}
			catch (Exception)
			{
				legendPositionType = org.applied_geodesy.jag3d.ui.graphic.layer.LegendPositionType.NORTH_EAST;
			}

			try
			{
				visible = PROPERTIES.getProperty("LEGEND_VISIBLE").equalsIgnoreCase("TRUE");
			}
			catch (Exception)
			{
			}
			try
			{
				fontSize = double.Parse(PROPERTIES.getProperty("LEGEND_FONT_SIZE"));
			}
			catch (Exception)
			{
			}
			try
			{
				lineWidth = double.Parse(PROPERTIES.getProperty("LEGEND_LINE_WIDTH"));
			}
			catch (Exception)
			{
			}

			lineWidth = lineWidth >= 0 ? lineWidth : 0.25;
			fontSize = fontSize >= 0 ? fontSize : 12.0;
			fontFamily = !string.ReferenceEquals(fontFamily, null) ? fontFamily : "System";

			this.Color = symbolColor;
			this.LineWidth = lineWidth;
			this.LegendPositionType = legendPositionType;

			this.FontSize = fontSize;
			this.FontFamily = fontFamily;
			this.FontColor = fontColor;

			this.Visible = visible;
		}

		public override void draw(GraphicsContext graphicsContext, GraphicExtent graphicExtent)
		{
			Color fontColor = this.FontColor;
			string fontFamily = this.FontFamily;
			double fontSize = this.FontSize;
			double lineWidth = this.LineWidth;
			Color lineColor = this.Color;

			Font font = Font.font(fontFamily, FontWeight.NORMAL, FontPosture.REGULAR, fontSize);

			double sketchWidth = graphicExtent.DrawingBoardWidth;
			double sketchHeight = graphicExtent.DrawingBoardHeight;

			double xMargin = 10, yMargin = 10;
			double xPadding = 5, yPadding = 5;

			int cnt = 0;
			double textColumnWidth = 0;
			double symbolColumnWidth = 3 * 0.7 * fontSize;
			double textHeight = 0;

			IList<Layer> legendLayers = new List<Layer>(this.layers.size());

			foreach (Layer layer in this.layers)
			{
				if (layer == this || layer.LayerType == LayerType.LEGEND || !layer.Visible || !layer.hasContent())
				{
					continue;
				}

				legendLayers.Add(layer);
				double[] textSize = this.getTextSize(font, layer.ToString());
				textColumnWidth = Math.Max(textColumnWidth, textSize[0]);
				textHeight = Math.Max(textHeight, textSize[1]);
				cnt++;
			}

			textHeight = textHeight + 3;

			double legendHeight = cnt * textHeight + 2 * yPadding;
			double legendWidth = textColumnWidth + symbolColumnWidth + 3 * xPadding;

			if (cnt == 0 || (legendWidth + xMargin) > sketchWidth || (legendHeight + yMargin) > sketchHeight)
			{
				return;
			}

			graphicsContext.setLineCap(StrokeLineCap.BUTT);
			graphicsContext.setLineDashes(null);

			// Draw legend box
			graphicsContext.setLineWidth(lineWidth);
			graphicsContext.setStroke(new Color(1, 1, 1, 0.9));
			graphicsContext.setFill(new Color(1, 1, 1, 0.9));

			PixelCoordinate leftCorner = this.getLeftUpperCorner(graphicExtent, legendWidth, legendHeight, xMargin, yMargin);
			double xLeftUpperCorner = leftCorner.X;
			double yLeftUpperCorner = leftCorner.Y;

			// x, y, w, h, arcWidth, arcHeight
			graphicsContext.fillRoundRect(xLeftUpperCorner, yLeftUpperCorner, legendWidth, legendHeight, 5, 5);

			// Draw legend box border		
			graphicsContext.setStroke(lineColor);
			graphicsContext.setFill(lineColor);
			graphicsContext.strokeRoundRect(xLeftUpperCorner, yLeftUpperCorner, legendWidth, legendHeight, 5, 5);

			double x = sketchWidth - legendWidth;
			double y = yLeftUpperCorner + textHeight; //  + this.yPadding
			// draw visible legend items
			int length = legendLayers.Count;
			for (int i = length - 1; i >= 0; i--)
			{
				Layer layer = legendLayers[i];

				x = xLeftUpperCorner + xPadding;

				layer.drawLegendSymbol(graphicsContext, graphicExtent, new PixelCoordinate(x, y - 0.3 * textHeight), 0.7 * fontSize, symbolColumnWidth);

				x += symbolColumnWidth + xPadding;

				graphicsContext.setStroke(fontColor);
				graphicsContext.setFill(fontColor);
				graphicsContext.setFont(font);
				graphicsContext.fillText(layer.ToString(), x, y);

				y += textHeight;
			}
		}

		private PixelCoordinate getLeftUpperCorner(GraphicExtent graphicExtent, double legendWidth, double legendHeight, double xMargin, double yMargin)
		{
			double sketchWidth = graphicExtent.DrawingBoardWidth;
			double sketchHeight = graphicExtent.DrawingBoardHeight;

			double xLeftUpperCorner = 0, yLeftUpperCorner = 0;

			switch (this.legendPositionType.get())
			{
			case NORTH:
				xLeftUpperCorner = 0.5 * (sketchWidth - legendWidth);
				yLeftUpperCorner = yMargin;
				break;

			case SOUTH:
				xLeftUpperCorner = 0.5 * sketchWidth - 0.5 * legendWidth;
				yLeftUpperCorner = sketchHeight - (legendHeight + 5 * yMargin);
				break;

			case EAST:
				xLeftUpperCorner = sketchWidth - (legendWidth + xMargin);
				yLeftUpperCorner = 0.5 * (sketchHeight - legendHeight);
				break;

			case WEST:
				xLeftUpperCorner = xMargin;
				yLeftUpperCorner = 0.5 * (sketchHeight - legendHeight);
				break;


			case NORTH_EAST:
				xLeftUpperCorner = sketchWidth - (legendWidth + xMargin);
				yLeftUpperCorner = yMargin;
				break;

			case NORTH_WEST:
				xLeftUpperCorner = xMargin;
				yLeftUpperCorner = yMargin;
				break;

			case SOUTH_EAST:
				xLeftUpperCorner = sketchWidth - (legendWidth + xMargin);
				yLeftUpperCorner = sketchHeight - (legendHeight + 5 * yMargin);
				break;

			case SOUTH_WEST:
				xLeftUpperCorner = xMargin;
				yLeftUpperCorner = sketchHeight - (legendHeight + 5 * yMargin);
				break;
			}

			return new PixelCoordinate(xLeftUpperCorner, yLeftUpperCorner);
		}

		public override GraphicExtent MaximumGraphicExtent
		{
			get
			{
				return new GraphicExtent();
			}
		}

		public override void clearLayer()
		{
		}
		public override bool hasContent()
		{
			foreach (Layer layer in this.layers)
			{
				if (layer != this && layer.Visible && layer.hasContent())
				{
					return true;
				}
			}
			return false;
		}

		public override void drawLegendSymbol(GraphicsContext graphicsContext, GraphicExtent graphicExtent, PixelCoordinate pixelCoordinate, double symbolHeight, double symbolWidth)
		{
		}

		private double[] getTextSize(Font font, string str)
		{
			// https://stackoverflow.com/questions/32237048/javafx-fontmetrics
			Text text = new Text(str);
			text.setFont(font);
			text.setWrappingWidth(0);
			text.setLineSpacing(0);
			Bounds textBounds = text.getBoundsInLocal();
			Rectangle stencil = new Rectangle(textBounds.getMinX(), textBounds.getMinY(), textBounds.getWidth(), textBounds.getHeight());
			Shape intersection = Shape.intersect(text, stencil);
			textBounds = intersection.getBoundsInLocal();
			return new double[] {textBounds.getWidth(), textBounds.getHeight()};
		}

		public ObjectProperty<LegendPositionType> legendPositionTypeProperty()
		{
			return this.legendPositionType;
		}

		public LegendPositionType LegendPositionType
		{
			get
			{
				return this.legendPositionTypeProperty().get();
			}
			set
			{
				this.legendPositionTypeProperty().set(value);
			}
		}


		public ObjectProperty<Color> fontColorProperty()
		{
			return this.fontColor;
		}

		public Color FontColor
		{
			get
			{
				return this.fontColorProperty().get();
			}
			set
			{
				this.fontColorProperty().set(value);
			}
		}


		public DoubleProperty fontSizeProperty()
		{
			return this.fontSize;
		}

		public double FontSize
		{
			get
			{
				return this.fontSizeProperty().get();
			}
			set
			{
				this.fontSizeProperty().set(value);
			}
		}


		public virtual StringProperty fontFamilyProperty()
		{
			return this.fontFamily;
		}

		public virtual string FontFamily
		{
			get
			{
				return this.fontFamilyProperty().get();
			}
			set
			{
				this.fontFamilyProperty().set(value);
			}
		}


		public override string ToString()
		{
			return i18n.getString("LegendLayer.type", "Legende");
		}
	}

}