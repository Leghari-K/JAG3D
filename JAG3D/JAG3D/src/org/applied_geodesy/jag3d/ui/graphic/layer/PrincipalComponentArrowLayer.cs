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
	using WorldCoordinate = org.applied_geodesy.jag3d.ui.graphic.coordinate.WorldCoordinate;
	using ArrowSymbolType = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.ArrowSymbolType;
	using SymbolBuilder = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.SymbolBuilder;
	using GraphicPoint = org.applied_geodesy.jag3d.ui.graphic.sql.GraphicPoint;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;

	using FXCollections = javafx.collections.FXCollections;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Color = javafx.scene.paint.Color;

	public class PrincipalComponentArrowLayer : ArrowLayer
	{
		private IList<PointLayer> referenceLayers = FXCollections.observableArrayList();

		internal PrincipalComponentArrowLayer(LayerType layerType) : base(layerType)
		{

			Color color;
			ArrowSymbolType arrowSymbolType;
			double symbolSize = -1, lineWidth = -1;
			bool visible = false;

			switch (layerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL:
				try
				{
					color = Color.web(PROPERTIES.getProperty("PRINCIPAL_COMPONENT_HORIZONTAL_ARROW_COLOR", "#6dc6bd"));
				}
				catch (Exception)
				{
					color = Color.web("#6dc6bd");
				}

				try
				{
					arrowSymbolType = ArrowSymbolType.valueOf(PROPERTIES.getProperty("PRINCIPAL_COMPONENT_HORIZONTAL_ARROW_SYMBOL_TYPE", "FILLED_TRIANGLE_ARROW"));
				}
				catch (Exception)
				{
					arrowSymbolType = ArrowSymbolType.FILLED_TRIANGLE_ARROW;
				}

				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("PRINCIPAL_COMPONENT_HORIZONTAL_ARROW_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("PRINCIPAL_COMPONENT_HORIZONTAL_ARROW_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("PRINCIPAL_COMPONENT_HORIZONTAL_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.PRINCIPAL_COMPONENT_VERTICAL:
				try
				{
					color = Color.web(PROPERTIES.getProperty("PRINCIPAL_COMPONENT_VERTICAL_ARROW_COLOR", "#b3e6e6"));
				}
				catch (Exception)
				{
					color = Color.web("#b3e6e6");
				}

				try
				{
					arrowSymbolType = ArrowSymbolType.valueOf(PROPERTIES.getProperty("PRINCIPAL_COMPONENT_VERTICAL_ARROW_SYMBOL_TYPE", "FILLED_TRIANGLE_ARROW"));
				}
				catch (Exception)
				{
					arrowSymbolType = ArrowSymbolType.FILLED_TRIANGLE_ARROW;
				}

				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("PRINCIPAL_COMPONENT_VERTICAL_ARROW_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("PRINCIPAL_COMPONENT_VERTICAL_ARROW_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("PRINCIPAL_COMPONENT_VERTICAL_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				break;

			default:
				throw new System.ArgumentException("Error, unsupported layer type " + layerType);
			}

			symbolSize = symbolSize >= 0 ? symbolSize : SymbolBuilder.DEFAULT_SIZE;
			lineWidth = lineWidth >= 0 ? lineWidth : 1.0;

			this.Visible = visible;
			this.SymbolType = arrowSymbolType;
			this.Color = color;
			this.SymbolSize = symbolSize;
			this.LineWidth = lineWidth;
		}

		public virtual void add(PointLayer layer)
		{
			this.referenceLayers.Add(layer);
		}

		public virtual void addAll(params PointLayer[] layers)
		{
			foreach (PointLayer layer in layers)
			{
				this.add(layer);
			}
		}

		internal virtual IList<PointLayer> ReferenceLayers
		{
			get
			{
				return this.referenceLayers;
			}
		}

		public override void draw(GraphicsContext graphicsContext, GraphicExtent graphicExtent)
		{
			if (!this.Visible || this.referenceLayers == null || this.referenceLayers.Count == 0)
			{
				return;
			}

			LayerType layerType = this.LayerType;
			ArrowSymbolType arrowSymbolType = this.SymbolType;
			double scale = this.VectorScale;
			double symbolSize = this.SymbolSize;
			double lineWidth = this.LineWidth;

			double width = graphicExtent.DrawingBoardWidth;
			double height = graphicExtent.DrawingBoardHeight;

			graphicsContext.setStroke(this.Color);
			graphicsContext.setFill(this.Color);
			graphicsContext.setLineWidth(lineWidth);
			graphicsContext.setLineDashes(null);

			foreach (PointLayer layer in this.referenceLayers)
			{
				if (layer.Visible)
				{
					// draw points
					foreach (GraphicPoint startPoint in layer.Points)
					{
						if (!startPoint.Visible)
						{
							continue;
						}

						WorldCoordinate endPoint;

						switch (layerType.innerEnumValue)
						{
						case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL:
							endPoint = new WorldCoordinate(startPoint.Coordinate.X + startPoint.PrincipalComponentX, startPoint.Coordinate.Y + startPoint.PrincipalComponentY);
							break;

						case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.PRINCIPAL_COMPONENT_VERTICAL:
							endPoint = new WorldCoordinate(startPoint.Coordinate.X + 0, startPoint.Coordinate.Y + startPoint.PrincipalComponentZ);
							break;
						default:
							continue;
						}

						PixelCoordinate pixelCoordinateStartPoint = GraphicExtent.toPixelCoordinate(startPoint.Coordinate, graphicExtent);
						PixelCoordinate pixelCoordinateEndPoint = GraphicExtent.toPixelCoordinate(endPoint, graphicExtent);

						if (!this.contains(graphicExtent, pixelCoordinateStartPoint) && !this.contains(graphicExtent, pixelCoordinateEndPoint))
						{
							continue;
						}

						double xs = pixelCoordinateStartPoint.X;
						double ys = pixelCoordinateStartPoint.Y;

						double xe = pixelCoordinateEndPoint.X;
						double ye = pixelCoordinateEndPoint.Y;

						double distance = Math.hypot(xe - xs, ye - ys);

						// skip, if symbol size larger than component
						if (distance == 0 || distance * scale < symbolSize)
						{
							continue;
						}

						distance = distance > 0 ? distance : 1;
						double dx = (xe - xs) / distance;
						double dy = (ye - ys) / distance;

						double angle = Math.Atan2(dy, dx);

						if (!this.contains(graphicExtent, pixelCoordinateStartPoint))
						{
							continue;
						}

						distance = distance * scale;

						// clipping line, if one of the points is outside
						double layerDiagoal = Math.hypot(width, height);
						if (distance > 1.05 * layerDiagoal)
						{
							distance = 1.05 * layerDiagoal;
						}

						PixelCoordinate vectorEndCoordinate = new PixelCoordinate(xs + distance * dx, ys + distance * dy);

	//					graphicsContext.setStroke(this.getColor());
	//					graphicsContext.setFill(this.getColor());
	//					graphicsContext.setLineWidth(lineWidth);
	//					graphicsContext.setLineDashes(null);

						graphicsContext.strokeLine(xs, ys, xs + distance * dx, ys + distance * dy);

						SymbolBuilder.drawSymbol(graphicsContext, vectorEndCoordinate, arrowSymbolType, symbolSize, angle);
					}
				}
			}
		}

		public override string ToString()
		{
			switch (this.LayerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL:
				return i18n.getString("PrincipalComponentArrowLayer.type.horizontal", "Horizontal principal component");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.PRINCIPAL_COMPONENT_VERTICAL:
				return i18n.getString("PrincipalComponentArrowLayer.type.vertical", "Vertical principal component");
			default:
				return "";
			}
		}

		public override GraphicExtent MaximumGraphicExtent
		{
			get
			{
				GraphicExtent extent = new GraphicExtent();
				extent.reset();
				return extent;
			}
		}

		public override void clearLayer()
		{
		}
		public override void drawLegendSymbol(GraphicsContext graphicsContext, GraphicExtent graphicExtent, PixelCoordinate pixelCoordinate, double symbolHeight, double symbolWidth)
		{
			ArrowSymbolType arrowSymbolType = this.SymbolType;
			double lineWidth = this.LineWidth;
			double symbolSize = symbolHeight;

			graphicsContext.setStroke(this.Color);
			graphicsContext.setFill(this.Color);
			graphicsContext.setLineWidth(lineWidth);
			graphicsContext.setLineDashes(null);

			graphicsContext.strokeLine(pixelCoordinate.X, pixelCoordinate.Y, pixelCoordinate.X + symbolWidth, pixelCoordinate.Y + 0);

			SymbolBuilder.drawSymbol(graphicsContext, pixelCoordinate, arrowSymbolType, symbolSize, Math.PI);
		}

		public override bool hasContent()
		{
			LayerType layerType = this.LayerType;

			foreach (PointLayer layer in this.referenceLayers)
			{
				if (layer.Visible && layer.hasContent())
				{
					foreach (GraphicPoint startPoint in layer.Points)
					{
						if (startPoint.Visible)
						{
							if ((layerType == LayerType.PRINCIPAL_COMPONENT_HORIZONTAL && startPoint.Dimension != 1) || (layerType == LayerType.PRINCIPAL_COMPONENT_VERTICAL && startPoint.Dimension != 2))
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}
	}

}