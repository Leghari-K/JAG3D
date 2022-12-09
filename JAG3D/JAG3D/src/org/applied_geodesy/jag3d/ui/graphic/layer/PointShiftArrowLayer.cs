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
	using RelativeConfidence = org.applied_geodesy.jag3d.ui.graphic.sql.RelativeConfidence;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;

	using FXCollections = javafx.collections.FXCollections;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Color = javafx.scene.paint.Color;

	public class PointShiftArrowLayer : ArrowLayer
	{
		private IList<RelativeConfidence> relativeConfidences = FXCollections.observableArrayList();

		internal PointShiftArrowLayer(LayerType layerType) : base(layerType)
		{

			Color color;
			ArrowSymbolType arrowSymbolType;
			double symbolSize = -1, lineWidth = -1;
			bool visible = true;

			switch (layerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_SHIFT_HORIZONTAL:
				try
				{
					color = Color.web(PROPERTIES.getProperty("POINT_SHIFT_HORIZONTAL_ARROW_COLOR", "#ff8c00"));
				}
				catch (Exception)
				{
					color = Color.web("#ff8c00");
				}

				try
				{
					arrowSymbolType = ArrowSymbolType.valueOf(PROPERTIES.getProperty("POINT_SHIFT_HORIZONTAL_ARROW_SYMBOL_TYPE", "FILLED_TETRAGON_ARROW"));
				}
				catch (Exception)
				{
					arrowSymbolType = ArrowSymbolType.FILLED_TETRAGON_ARROW;
				}

				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("POINT_SHIFT_HORIZONTAL_ARROW_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("POINT_SHIFT_HORIZONTAL_ARROW_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("POINT_SHIFT_HORIZONTAL_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_SHIFT_VERTICAL:
				try
				{
					color = Color.web(PROPERTIES.getProperty("POINT_SHIFT_VERTICAL_ARROW_COLOR", "#ffaf00"));
				}
				catch (Exception)
				{
					color = Color.web("#ffaf00");
				}

				try
				{
					arrowSymbolType = ArrowSymbolType.valueOf(PROPERTIES.getProperty("POINT_SHIFT_VERTICAL_ARROW_SYMBOL_TYPE", "FILLED_TETRAGON_ARROW"));
				}
				catch (Exception)
				{
					arrowSymbolType = ArrowSymbolType.FILLED_TETRAGON_ARROW;
				}

				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("POINT_SHIFT_VERTICAL_ARROW_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("POINT_SHIFT_VERTICAL_ARROW_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("POINT_SHIFT_VERTICAL_VISIBLE").equalsIgnoreCase("TRUE");
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

			this.SymbolType = arrowSymbolType;
			this.Color = color;
			this.SymbolSize = symbolSize;
			this.LineWidth = lineWidth;

			this.Visible = visible;
		}

		public override void draw(GraphicsContext graphicsContext, GraphicExtent graphicExtent)
		{
			if (!this.Visible || this.relativeConfidences == null || this.relativeConfidences.Count == 0)
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
			double layerDiagonal = Math.hypot(width, height);

			graphicsContext.setStroke(this.Color);
			graphicsContext.setFill(this.Color);
			graphicsContext.setLineWidth(lineWidth);
			graphicsContext.setLineDashes(null);

			foreach (RelativeConfidence relativeConfidence in this.relativeConfidences)
			{
				GraphicPoint startPoint = relativeConfidence.StartPoint;
				GraphicPoint endPoint = relativeConfidence.EndPoint;
				double deltaHeight = relativeConfidence.DeltaHeight;

				if (!startPoint.Visible || !endPoint.Visible)
				{
					continue;
				}

				PixelCoordinate pixelCoordinateStartPoint = GraphicExtent.toPixelCoordinate(startPoint.Coordinate, graphicExtent);
				PixelCoordinate pixelCoordinateEndPoint = GraphicExtent.toPixelCoordinate(endPoint.Coordinate, graphicExtent);

				if (!this.contains(graphicExtent, pixelCoordinateStartPoint) && !this.contains(graphicExtent, pixelCoordinateEndPoint))
				{
					continue;
				}

				double xs = pixelCoordinateStartPoint.X;
				double ys = pixelCoordinateStartPoint.Y;

				double xe = pixelCoordinateEndPoint.X;
				double ye = pixelCoordinateEndPoint.Y;

				double pxDistance = Math.hypot(xe - xs, ye - ys);

				double avgX = 0.5 * (xs + xe);
				double avgY = 0.5 * (ys + ye);

				PixelCoordinate vectorStartCoordinate = new PixelCoordinate(avgX, avgY);
				if (!this.contains(graphicExtent, vectorStartCoordinate))
				{
					continue;
				}

				switch (layerType.innerEnumValue)
				{
				// Draw horizontal components
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_SHIFT_HORIZONTAL:
					// skip, if symbol size larger than component
					if (startPoint.Dimension != 1 && endPoint.Dimension != 1 && pxDistance > 0 && pxDistance * scale >= symbolSize)
					{

						pxDistance = pxDistance > 0 ? pxDistance : 1;
						double dx = (xe - xs) / pxDistance;
						double dy = (ye - ys) / pxDistance;

						double angle = Math.Atan2(dy, dx);

						pxDistance = pxDistance * scale;

						// clipping line, if one of the points is outside
						if (pxDistance > 1.05 * layerDiagonal)
						{
							pxDistance = 1.05 * layerDiagonal;
						}

						PixelCoordinate vectorEndCoordinate = new PixelCoordinate(avgX + pxDistance * dx, avgY + pxDistance * dy);

						graphicsContext.strokeLine(avgX, avgY, avgX + pxDistance * dx, avgY + pxDistance * dy);

						SymbolBuilder.drawSymbol(graphicsContext, vectorEndCoordinate, arrowSymbolType, symbolSize, angle);
					}

					break;

				// Draw vertical component
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_SHIFT_VERTICAL:
					PixelCoordinate pixelCoordinateHeightComponent = GraphicExtent.toPixelCoordinate(new WorldCoordinate(startPoint.Coordinate.X, startPoint.Coordinate.Y + deltaHeight), graphicExtent);
					double pxHeightComponent = pixelCoordinateHeightComponent.Y - pixelCoordinateStartPoint.Y;

					// skip, if symbol size larger than component
					if (startPoint.Dimension != 2 && endPoint.Dimension != 2 && Math.Abs(pxHeightComponent) > 0 && Math.Abs(pxHeightComponent) * scale >= symbolSize)
					{
						// North or south direction
						double angle = 0.5 * Math.Sign(pxHeightComponent) * Math.PI;

						pxHeightComponent = pxHeightComponent * scale;

						// clipping line, if one of the points is outside
						if (Math.Abs(pxHeightComponent) > 1.05 * layerDiagonal)
						{
							pxHeightComponent = 1.05 * Math.Sign(pxHeightComponent) * layerDiagonal;
						}

						PixelCoordinate vectorEndCoordinate = new PixelCoordinate(avgX, avgY + pxHeightComponent);

						graphicsContext.strokeLine(avgX, avgY, avgX, avgY + pxHeightComponent);

						SymbolBuilder.drawSymbol(graphicsContext, vectorEndCoordinate, arrowSymbolType, symbolSize, angle);
					}

					break;

				default:
					continue;
				}
			}
		}

		public virtual IList<RelativeConfidence> RelativeConfidences
		{
			set
			{
				GraphicExtent graphicExtent = this.MaximumGraphicExtent;
				graphicExtent.reset();
				this.relativeConfidences.Clear();
				if (value != null)
				{
					foreach (RelativeConfidence relativeConfidence in value)
					{
						GraphicPoint startPoint = relativeConfidence.StartPoint;
						GraphicPoint endPoint = relativeConfidence.EndPoint;
    
						graphicExtent.merge(startPoint.Coordinate);
						graphicExtent.merge(endPoint.Coordinate);
					}
					((List<RelativeConfidence>)this.relativeConfidences).AddRange(value);
				}
			}
			get
			{
				return this.relativeConfidences;
			}
		}


		public override string ToString()
		{
			switch (this.LayerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_SHIFT_HORIZONTAL:
				return i18n.getString("PointShiftArrowLayer.type.horizontal", "Horizontal point shift");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.POINT_SHIFT_VERTICAL:
				return i18n.getString("PointShiftArrowLayer.type.vertical", "Vertical point shift");
			default:
				return "";
			}
		}

		public override void clearLayer()
		{
			this.relativeConfidences.Clear();
		}

		public override GraphicExtent MaximumGraphicExtent
		{
			get
			{
				GraphicExtent graphicExtent = new GraphicExtent();
				graphicExtent.reset();
				if (this.relativeConfidences != null)
				{
					foreach (RelativeConfidence relativeConfidence in this.relativeConfidences)
					{
						GraphicPoint startPoint = relativeConfidence.StartPoint;
						GraphicPoint endPoint = relativeConfidence.EndPoint;
    
						if (startPoint.Visible)
						{
							graphicExtent.merge(startPoint.Coordinate);
						}
						if (endPoint.Visible)
						{
							graphicExtent.merge(endPoint.Coordinate);
						}
					}
				}
				return graphicExtent;
			}
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
			foreach (RelativeConfidence relativeConfidence in this.relativeConfidences)
			{
				GraphicPoint startPoint = relativeConfidence.StartPoint;
				GraphicPoint endPoint = relativeConfidence.EndPoint;

				if (startPoint.Visible && endPoint.Visible)
				{
					if ((layerType == LayerType.POINT_SHIFT_HORIZONTAL && startPoint.Dimension != 1 && endPoint.Dimension != 1) || (layerType == LayerType.POINT_SHIFT_VERTICAL && startPoint.Dimension != 2 && endPoint.Dimension != 2))
					{
						return true;
					}
				}
			}
			return false;
		}
	}

}