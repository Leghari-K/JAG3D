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
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using FXCollections = javafx.collections.FXCollections;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Color = javafx.scene.paint.Color;

	public abstract class ConfidenceLayer<T> : Layer where T : Layer
	{

		private ObjectProperty<Color> strokeColor = new SimpleObjectProperty<Color>(Color.BLACK);
		private IList<T> referenceLayers = FXCollections.observableArrayList();

		internal ConfidenceLayer(LayerType layerType) : base(layerType)
		{
			this.SymbolSize = 0;
			this.LineWidth = 0.5;
		}

		public virtual void add(T layer)
		{
			this.referenceLayers.Add(layer);
		}

		public virtual void addAll(IList<T> layers)
		{
			foreach (T layer in layers)
			{
				this.add(layer);
			}
		}

		internal virtual IList<T> ReferenceLayers
		{
			get
			{
				return this.referenceLayers;
			}
		}

		public virtual ObjectProperty<Color> strokeColorProperty()
		{
			return this.strokeColor;
		}

		public virtual Color StrokeColor
		{
			get
			{
				return this.strokeColorProperty().get();
			}
			set
			{
				this.strokeColorProperty().set(value);
			}
		}


		public override void clearLayer()
		{
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

		public override void drawLegendSymbol(GraphicsContext graphicsContext, GraphicExtent graphicExtent, PixelCoordinate pixelCoordinate, double symbolHeight, double symbolWidth)
		{
			if (this.contains(graphicExtent, pixelCoordinate) && Math.Min(symbolHeight, symbolWidth) > 0)
			{
				double lineWidth = this.LineWidth;
				double symbolSize = symbolHeight;

				graphicsContext.setLineWidth(lineWidth);
				graphicsContext.setStroke(this.StrokeColor);
				graphicsContext.setFill(this.Color);
				graphicsContext.setLineDashes(null);

				double majorAxis = 0.75 * symbolSize;
				double minorAxis = 0.7 * majorAxis;
				double angle = 0;

				SymbolBuilder.drawEllipse(graphicsContext, new PixelCoordinate(pixelCoordinate.X + 0.5 * symbolWidth, pixelCoordinate.Y), majorAxis, minorAxis, angle);
			}
		}
	}
}