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
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;

	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Color = javafx.scene.paint.Color;

	public class AbsoluteConfidenceLayer : ConfidenceLayer<PointLayer>
	{
		private DoubleProperty confidenceScale = null;

		internal AbsoluteConfidenceLayer(LayerType layerType) : base(layerType)
		{

			Color fillColor, strokeColor;
			double lineWidth = -1;
			bool visible = true;

			try
			{
				fillColor = Color.web(PROPERTIES.getProperty("ABSOLUTE_CONFIDENCE_FILL_COLOR", "#cccccc"));
			}
			catch (Exception)
			{
				fillColor = Color.web("#cccccc");
			}

			try
			{
				strokeColor = Color.web(PROPERTIES.getProperty("ABSOLUTE_CONFIDENCE_STROKE_COLOR", "#000000"));
			}
			catch (Exception)
			{
				strokeColor = Color.web("#000000");
			}

			try
			{
				visible = PROPERTIES.getProperty("ABSOLUTE_CONFIDENCE_VISIBLE").equalsIgnoreCase("TRUE");
			}
			catch (Exception)
			{
			}
			try
			{
				lineWidth = double.Parse(PROPERTIES.getProperty("ABSOLUTE_CONFIDENCE_LINE_WIDTH"));
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

		public override void add(PointLayer layer)
		{
			base.add(layer);
		}

		public virtual void addAll(params PointLayer[] layers)
		{
			foreach (PointLayer layer in layers)
			{
				this.add(layer);
			}
		}

		public override void draw(GraphicsContext graphicsContext, GraphicExtent graphicExtent)
		{
			if (!this.Visible || graphicExtent.Scale <= 0)
			{
				return;
			}

			IList<PointLayer> referenceLayers = this.ReferenceLayers;
			double scale = graphicExtent.Scale;
			double ellipseScale = this.ConfidenceScale / scale;
			double lineWidth = this.LineWidth;
			graphicsContext.setLineWidth(lineWidth);
			graphicsContext.setStroke(this.StrokeColor);
			graphicsContext.setFill(this.Color);
			graphicsContext.setLineDashes(null);

			foreach (PointLayer layer in referenceLayers)
			{
				if (layer.Visible)
				{
					// draw points
					foreach (GraphicPoint point in layer.Points)
					{
						if (!point.Visible)
						{
							continue;
						}

						PixelCoordinate pixelCoordinate = GraphicExtent.toPixelCoordinate(point.Coordinate, graphicExtent);
						if (this.contains(graphicExtent, pixelCoordinate) && point.MajorAxis > 0)
						{
							double majorAxis = ellipseScale * point.MajorAxis;
							double minorAxis = ellipseScale * point.MinorAxis;
							double angle = point.Angle;

							SymbolBuilder.drawEllipse(graphicsContext, pixelCoordinate, majorAxis, minorAxis, angle);
						}
					}
				}
			}
		}

		public virtual DoubleProperty confidenceScaleProperty()
		{
			if (this.confidenceScale == null)
			{
				this.confidenceScale = new SimpleDoubleProperty(1.0);
			}
			return this.confidenceScale;
		}

		public virtual double ConfidenceScale
		{
			get
			{
				return this.confidenceScaleProperty().get();
			}
			set
			{
				this.confidenceScaleProperty().set(value);
			}
		}


		public override string ToString()
		{
			return i18n.getString("AbsoluteConfidenceLayer.type", "Point confidences");
		}

		public override bool hasContent()
		{
			IList<PointLayer> referenceLayers = this.ReferenceLayers;
			foreach (PointLayer layer in referenceLayers)
			{
				if (layer.Visible && layer.hasContent())
				{
					foreach (GraphicPoint point in layer.Points)
					{
						if (point.Visible)
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