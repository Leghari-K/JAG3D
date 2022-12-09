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
	using SymbolBuilder = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.SymbolBuilder;
	using GraphicPoint = org.applied_geodesy.jag3d.ui.graphic.sql.GraphicPoint;
	using RelativeConfidence = org.applied_geodesy.jag3d.ui.graphic.sql.RelativeConfidence;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;

	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Color = javafx.scene.paint.Color;

	public class RelativeConfidenceLayer : ConfidenceLayer<PointShiftArrowLayer>
	{

		internal RelativeConfidenceLayer(LayerType layerType) : base(layerType)
		{

			Color fillColor, strokeColor;
			double lineWidth = -1;
			bool visible = true;

			try
			{
				fillColor = Color.web(PROPERTIES.getProperty("RELATIVE_CONFIDENCE_FILL_COLOR", "#ffffe0"));
			}
			catch (Exception)
			{
				fillColor = Color.web("#ffffe0");
			}

			try
			{
				strokeColor = Color.web(PROPERTIES.getProperty("RELATIVE_CONFIDENCE_STROKE_COLOR", "#000000"));
			}
			catch (Exception)
			{
				strokeColor = Color.web("#000000");
			}

			try
			{
				visible = PROPERTIES.getProperty("RELATIVE_CONFIDENCE_VISIBLE").equalsIgnoreCase("TRUE");
			}
			catch (Exception)
			{
			}
			try
			{
				lineWidth = double.Parse(PROPERTIES.getProperty("RELATIVE_CONFIDENCE_LINE_WIDTH"));
			}
			catch (Exception)
			{
			}
			lineWidth = lineWidth >= 0 ? lineWidth : 0.5;

			this.StrokeColor = strokeColor;
			this.Color = fillColor;
			this.LineWidth = lineWidth;

			this.Visible = visible;
		}

		public override void draw(GraphicsContext graphicsContext, GraphicExtent graphicExtent)
		{
			if (!this.Visible || graphicExtent.Scale <= 0)
			{
				return;
			}

			IList<PointShiftArrowLayer> referenceLayers = this.ReferenceLayers;
			double scale = graphicExtent.Scale;
			double lineWidth = this.LineWidth;
			graphicsContext.setLineWidth(lineWidth);
			graphicsContext.setStroke(this.StrokeColor);
			graphicsContext.setFill(this.Color);
			graphicsContext.setLineDashes(null);

			foreach (PointShiftArrowLayer layer in referenceLayers)
			{
				if (layer.Visible)
				{
					double ellipseScale = layer.VectorScale / scale;
					foreach (RelativeConfidence relativeConfidence in layer.RelativeConfidences)
					{
						GraphicPoint startPoint = relativeConfidence.StartPoint;
						GraphicPoint endPoint = relativeConfidence.EndPoint;

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

						double avgX = 0.5 * (xs + xe);
						double avgY = 0.5 * (ys + ye);

						PixelCoordinate vectorStartCoordinate = new PixelCoordinate(avgX, avgY);

						if (this.contains(graphicExtent, vectorStartCoordinate) && relativeConfidence.MajorAxis > 0)
						{
							double majorAxis = ellipseScale * relativeConfidence.MajorAxis;
							double minorAxis = ellipseScale * relativeConfidence.MinorAxis;
							double angle = relativeConfidence.Angle;

							SymbolBuilder.drawEllipse(graphicsContext, vectorStartCoordinate, majorAxis, minorAxis, angle);
						}
					}
				}
			}
		}

		public override string ToString()
		{
			return i18n.getString("RelativeConfidenceLayer.type", "Relative confidences");
		}

		public override bool hasContent()
		{
			IList<PointShiftArrowLayer> referenceLayers = this.ReferenceLayers;

			foreach (PointShiftArrowLayer layer in referenceLayers)
			{
				if (layer.Visible && layer.hasContent())
				{
					foreach (RelativeConfidence relativeConfidence in layer.RelativeConfidences)
					{
						GraphicPoint startPoint = relativeConfidence.StartPoint;
						GraphicPoint endPoint = relativeConfidence.EndPoint;
						if (startPoint.Visible && endPoint.Visible)
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}

}