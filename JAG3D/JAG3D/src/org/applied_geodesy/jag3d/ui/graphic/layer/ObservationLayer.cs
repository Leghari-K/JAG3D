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
	using ObservationType = org.applied_geodesy.jag3d.ui.graphic.layer.ObservationSymbolProperties.ObservationType;
	using SymbolBuilder = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.SymbolBuilder;
	using GraphicPoint = org.applied_geodesy.jag3d.ui.graphic.sql.GraphicPoint;
	using ObservableMeasurement = org.applied_geodesy.jag3d.ui.graphic.sql.ObservableMeasurement;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;

	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using FXCollections = javafx.collections.FXCollections;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Color = javafx.scene.paint.Color;
	using StrokeLineCap = javafx.scene.shape.StrokeLineCap;

	public class ObservationLayer : Layer, HighlightableLayer
	{
		// size of point symbol of point layers 
		private DoubleProperty pointSymbolSize = new SimpleDoubleProperty(SymbolBuilder.DEFAULT_SIZE);
		private IList<ObservableMeasurement> observableMeasurements = FXCollections.observableArrayList();
		private IDictionary<ObservationType, ObservationSymbolProperties> symbolPropertiesMap = new Dictionary<ObservationType, ObservationSymbolProperties>(ObservationType.values().Length);

		private ObjectProperty<Color> highlightColor = new SimpleObjectProperty<Color>(Color.ORANGERED); //#FF4500
		private DoubleProperty highlightLineWidth = new SimpleDoubleProperty(2.5);
		private ObjectProperty<TableRowHighlightType> highlightType = new SimpleObjectProperty<TableRowHighlightType>(TableRowHighlightType.NONE);
		private ISet<ObservationType> projectObservationTypes = new LinkedHashSet<ObservationType>(10); // Store all given ObservationType of the project

		internal ObservationLayer(LayerType layerType) : base(layerType)
		{
			TableRowHighlightType highlightType;
			Color color, highlightColor;
			double symbolSize = -1, lineWidth = -1, highlightLineWidth = -1;
			bool visible = true;

			switch (layerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.OBSERVATION_APRIORI:
				try
				{
					color = Color.web(PROPERTIES.getProperty("OBSERVATION_APRIORI_COLOR", "#b0c4de"));
				}
				catch (Exception)
				{
					color = Color.web("#b0c4de");
				}

				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("OBSERVATION_APRIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("OBSERVATION_APRIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("OBSERVATION_APRIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
				try
				{
					highlightType = TableRowHighlightType.valueOf(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_HIGHLIGHT_TYPE", "NONE"));
					if (highlightType == null)
					{
						highlightType = TableRowHighlightType.NONE;
					}
				}
				catch (Exception)
				{
					highlightType = TableRowHighlightType.NONE;
				}

				try
				{
					highlightColor = Color.web(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_HIGHLIGHT_COLOR", "#FF4500"));
				}
				catch (Exception)
				{
					highlightColor = Color.web("#FF4500");
				}

				try
				{
					color = Color.web(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_COLOR", "#778899"));
				}
				catch (Exception)
				{
					color = Color.web("#778899");
				}

				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}
				try
				{
					highlightLineWidth = double.Parse(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_HIGHLIGHT_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				this.HighlightLineWidth = highlightLineWidth >= 0 ? highlightLineWidth : 2.5;
				this.HighlightColor = highlightColor;
				this.HighlightType = highlightType;

				break;
			default:
				throw new System.ArgumentException("Error, unsupported layer type " + layerType);
			}

			symbolSize = symbolSize >= 0 ? symbolSize : SymbolBuilder.DEFAULT_SIZE;
			lineWidth = lineWidth >= 0 ? lineWidth : 1.0;

			this.LineWidth = lineWidth;
			this.SymbolSize = symbolSize;
			this.Color = color;

			this.Visible = visible;

			this.initSymbolProperties();
		}

		private void initSymbolProperties()
		{
			ObservationType[] observationTypes = ObservationType.values();
			ObservationSymbolProperties properties = null;
			foreach (ObservationType observationType in observationTypes)
			{
				properties = new ObservationSymbolProperties(observationType);
				properties.Visible = true;
				Color color = Color.BLACK;
				switch (this.LayerType.innerEnumValue)
				{
				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.OBSERVATION_APRIORI:
					switch (observationType.innerEnumValue)
					{
					case ObservationType.InnerEnum.LEVELING:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APRIORI_COLOR_LEVELING", "#c195c1"));
						}
						catch (Exception)
						{
							color = Color.web("#c195c1");
						}
						break;

					case ObservationType.InnerEnum.DIRECTION:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APRIORI_COLOR_DIRECTION", "#ffff7d"));
						}
						catch (Exception)
						{
							color = Color.web("#ffff7d");
						}
						break;

					case ObservationType.InnerEnum.DISTANCE:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APRIORI_COLOR_DISTANCE", "#6d93ff"));
						}
						catch (Exception)
						{
							color = Color.web("#6d93ff");
						}
						break;

					case ObservationType.InnerEnum.ZENITH_ANGLE:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APRIORI_COLOR_ZENITH_ANGLE", "#f28282"));
						}
						catch (Exception)
						{
							color = Color.web("#f28282");
						}
						break;

					case ObservationType.InnerEnum.GNSS:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APRIORI_COLOR_GNSS", "#70b770"));
						}
						catch (Exception)
						{
							color = Color.web("#70b770");
						}
						break;
					}

					break;

				case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
					switch (observationType.innerEnumValue)
					{
					case ObservationType.InnerEnum.LEVELING:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_COLOR_LEVELING", "#875187"));
						}
						catch (Exception)
						{
							color = Color.web("#875187");
						}
						break;

					case ObservationType.InnerEnum.DIRECTION:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_COLOR_DIRECTION", "#d7d722"));
						}
						catch (Exception)
						{
							color = Color.web("#d7d722");
						}
						break;

					case ObservationType.InnerEnum.DISTANCE:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_COLOR_DISTANCE", "#4872e9"));
						}
						catch (Exception)
						{
							color = Color.web("#4872e9");
						}
						break;

					case ObservationType.InnerEnum.ZENITH_ANGLE:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_COLOR_ZENITH_ANGLE", "#d23737"));
						}
						catch (Exception)
						{
							color = Color.web("#d23737");
						}
						break;

					case ObservationType.InnerEnum.GNSS:
						try
						{
							color = Color.web(PROPERTIES.getProperty("OBSERVATION_APOSTERIORI_COLOR_GNSS", "#32a332"));
						}
						catch (Exception)
						{
							color = Color.web("#32a332");
						}
						break;

					}
					break;

				default:
					break;
				}

				properties.Color = color;

				this.symbolPropertiesMap[observationType] = properties;
			}
		}

		public override void draw(GraphicsContext graphicsContext, GraphicExtent graphicExtent)
		{
			if (!this.Visible || this.observableMeasurements.Count == 0)
			{
				return;
			}

			double symbolSize = this.SymbolSize;
			graphicsContext.setLineCap(StrokeLineCap.BUTT);
			graphicsContext.setLineWidth(this.LineWidth);

			const double maxLength = 125;
			double pointSymbolSize = this.PointSymbolSize;

			double width = graphicExtent.DrawingBoardWidth;
			double height = graphicExtent.DrawingBoardHeight;

			foreach (ObservableMeasurement observableLink in this.observableMeasurements)
			{
				GraphicPoint startPoint = observableLink.StartPoint;
				GraphicPoint endPoint = observableLink.EndPoint;

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

				if (!this.contains(graphicExtent, pixelCoordinateStartPoint) && !this.contains(graphicExtent, pixelCoordinateEndPoint))
				{
					continue;
				}

				double xs = pixelCoordinateStartPoint.X;
				double ys = pixelCoordinateStartPoint.Y;

				double xe = pixelCoordinateEndPoint.X;
				double ye = pixelCoordinateEndPoint.Y;

				double distance = Math.hypot(xe - xs, ye - ys);
				distance = distance > 0 ? distance : 1;
				double dx = (xe - xs) / distance;
				double dy = (ye - ys) / distance;

				Color color = this.Color;
				double lineWidth = this.LineWidth;

				TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
				double leftBoundary = tableRowHighlight.getLeftBoundary(this.HighlightType);
				double rightBoundary = tableRowHighlight.getRightBoundary(this.HighlightType);

				switch (this.HighlightType.innerEnumValue)
				{
				case TableRowHighlightType.InnerEnum.INFLUENCE_ON_POSITION:
					if (observableLink.InfluenceOnPosition > rightBoundary)
					{
						color = this.HighlightColor;
						lineWidth = this.HighlightLineWidth;
					}
					break;
				case TableRowHighlightType.InnerEnum.P_PRIO_VALUE:
					if (observableLink.Pprio < Math.Log(leftBoundary / 100.0))
					{
						color = this.HighlightColor;
						lineWidth = this.HighlightLineWidth;
					}
					break;
				case TableRowHighlightType.InnerEnum.REDUNDANCY:
					if (observableLink.Redundancy < leftBoundary)
					{
						color = this.HighlightColor;
						lineWidth = this.HighlightLineWidth;
					}
					break;
				case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
					if (observableLink.Significant)
					{
						color = this.HighlightColor;
						lineWidth = this.HighlightLineWidth;
					}
					break;
				case TableRowHighlightType.InnerEnum.GROSS_ERROR:
					if (observableLink.GrossErrorExceeded)
					{
						color = this.HighlightColor;
						lineWidth = this.HighlightLineWidth;
					}
					break;
				case TableRowHighlightType.InnerEnum.NONE: // DEFAULT
					color = this.Color;
					lineWidth = this.LineWidth;
					break;
				}

				graphicsContext.setStroke(color);
				graphicsContext.setLineWidth(lineWidth);
				graphicsContext.setLineDashes(null);

				// clipping line, if one of the points is outside
				double layerDiagoal = Math.hypot(width, height);
				if (distance > 1.005 * layerDiagoal)
				{
					distance = 1.005 * layerDiagoal;

					if (!this.contains(graphicExtent, pixelCoordinateStartPoint))
					{
						xs = xe - distance * dx;
						ys = ye - distance * dy;
					}

					else if (!this.contains(graphicExtent, pixelCoordinateEndPoint))
					{
						xe = xs + distance * dx;
						ye = ys + distance * dy;
					}
				}

				double si = 0.0, ei = 0.0;
				if (this.contains(graphicExtent, pixelCoordinateStartPoint) && observableLink.StartPointObservationType.Count > 0)
				{
					si = 1.0;
					graphicsContext.strokeLine(xs, ys, xs + Math.Min(0.35 * distance, maxLength) * dx, ys + Math.Min(0.35 * distance, maxLength) * dy);
				}

				if (this.contains(graphicExtent, pixelCoordinateEndPoint) && observableLink.EndPointObservationType.Count > 0)
				{
					ei = 1.0;
					graphicsContext.strokeLine(xe, ye, xe - Math.Min(0.35 * distance, maxLength) * dx, ye - Math.Min(0.35 * distance, maxLength) * dy);
				}

				graphicsContext.setLineDashes(2.5, 3.5);
				graphicsContext.strokeLine(xs + si * (Math.Min(0.35 * distance, maxLength) * dx), ys + si * (Math.Min(0.35 * distance, maxLength) * dy), xe - ei * (Math.Min(0.35 * distance, maxLength) * dx), ye - ei * (Math.Min(0.35 * distance, maxLength) * dy));

				// check, if drawable
				if (distance > 3.0 * symbolSize)
				{
					double scale = distance > 4.0 * pointSymbolSize + symbolSize ? 1.5 * pointSymbolSize + 0.5 * symbolSize : 0.5 * (distance - symbolSize);
					if (observableLink.StartPointObservationType.Count > 0)
					{
						PixelCoordinate coordinate = new PixelCoordinate(xs - 0.5 * symbolSize + scale * dx, ys - 0.5 * symbolSize + scale * dy);
						SymbolBuilder.drawSymbol(graphicsContext, coordinate, this.symbolPropertiesMap, observableLink.StartPointObservationType, symbolSize);
					}

					if (observableLink.EndPointObservationType.Count > 0)
					{
						PixelCoordinate coordinate = new PixelCoordinate(xe - 0.5 * symbolSize - scale * dx, ye - 0.5 * symbolSize - scale * dy);
						SymbolBuilder.drawSymbol(graphicsContext, coordinate, this.symbolPropertiesMap, observableLink.EndPointObservationType, symbolSize);
					}
				}
			}
		}

		public virtual IList<ObservableMeasurement> ObservableMeasurements
		{
			set
			{
				GraphicExtent graphicExtent = this.MaximumGraphicExtent;
				graphicExtent.reset();
				this.observableMeasurements.Clear();
				this.projectObservationTypes.Clear();
				if (value != null)
				{
					foreach (ObservableMeasurement observableMeasurement in value)
					{
						GraphicPoint startPoint = observableMeasurement.StartPoint;
						GraphicPoint endPoint = observableMeasurement.EndPoint;
    
						graphicExtent.merge(startPoint.Coordinate);
						graphicExtent.merge(endPoint.Coordinate);
    
						this.projectObservationTypes.addAll(observableMeasurement.StartPointObservationType);
						this.projectObservationTypes.addAll(observableMeasurement.EndPointObservationType);
    
					}
					((List<ObservableMeasurement>)this.observableMeasurements).AddRange(value);
				}
			}
		}

		public DoubleProperty pointSymbolSizeProperty()
		{
			return this.pointSymbolSize;
		}

		public double PointSymbolSize
		{
			get
			{
				return this.pointSymbolSizeProperty().get();
			}
			set
			{
				this.pointSymbolSizeProperty().set(value);
			}
		}


		public virtual ObservationSymbolProperties getObservationSymbolProperties(ObservationType observationType)
		{
			return this.symbolPropertiesMap[observationType];
		}

		public override string ToString()
		{
			switch (this.LayerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
				return i18n.getString("ObservationAprioriLayer.type.aposteriori", "Observations (a-posteriori)");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.OBSERVATION_APRIORI:
				return i18n.getString("ObservationAprioriLayer.type.apriori", "Observations (a-priori)");
			default:
				return "";
			}
		}

		public override void clearLayer()
		{
			this.observableMeasurements.Clear();
			this.projectObservationTypes.Clear();
		}

		public override GraphicExtent MaximumGraphicExtent
		{
			get
			{
				GraphicExtent graphicExtent = new GraphicExtent();
				graphicExtent.reset();
				if (this.observableMeasurements != null)
				{
					foreach (ObservableMeasurement observableMeasurement in this.observableMeasurements)
					{
						GraphicPoint startPoint = observableMeasurement.StartPoint;
						GraphicPoint endPoint = observableMeasurement.EndPoint;
    
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

		public ObjectProperty<Color> highlightColorProperty()
		{
			return this.highlightColor;
		}

		public Color HighlightColor
		{
			get
			{
				return this.highlightColorProperty().get();
			}
			set
			{
				this.highlightColorProperty().set(value);
			}
		}


		public DoubleProperty highlightLineWidthProperty()
		{
			return this.highlightLineWidth;
		}

		public double HighlightLineWidth
		{
			get
			{
				return this.highlightLineWidthProperty().get();
			}
			set
			{
				this.highlightLineWidthProperty().set(value);
			}
		}


		public virtual ObjectProperty<TableRowHighlightType> highlightTypeProperty()
		{
			return this.highlightType;
		}

		public virtual TableRowHighlightType HighlightType
		{
			get
			{
				return this.highlightTypeProperty().get();
			}
			set
			{
				this.highlightTypeProperty().set(value);
			}
		}


		public override void drawLegendSymbol(GraphicsContext graphicsContext, GraphicExtent graphicExtent, PixelCoordinate pixelCoordinateStartPoint, double symbolHeight, double symbolWidth)
		{
			PixelCoordinate pixelCoordinateEndPoint = new PixelCoordinate(pixelCoordinateStartPoint.X + 70, pixelCoordinateStartPoint.Y);

			if (!this.contains(graphicExtent, pixelCoordinateStartPoint) && !this.contains(graphicExtent, pixelCoordinateEndPoint))
			{
				return;
			}

			ISet<ObservationType> observationTypes = new LinkedHashSet<ObservationType>(10);

			foreach (ObservableMeasurement observableLink in this.observableMeasurements)
			{
				GraphicPoint startPoint = observableLink.StartPoint;
				GraphicPoint endPoint = observableLink.EndPoint;

				if (!startPoint.Visible || !endPoint.Visible)
				{
					continue;
				}

				observationTypes.addAll(observableLink.StartPointObservationType);
				observationTypes.addAll(observableLink.EndPointObservationType);

				if (observationTypes.Count == this.projectObservationTypes.Count)
				{
					bool containsAll = true;
					foreach (ObservationType type in observationTypes)
					{
						if (!this.projectObservationTypes.Contains(type))
						{
							containsAll = false;
							break;
						}
					}
					if (containsAll)
					{
						break;
					}
				}
			}

			if (observationTypes.Count == 0)
			{
				return;
			}

			graphicsContext.setLineWidth(this.LineWidth);
			graphicsContext.setStroke(this.Color);
			graphicsContext.setLineDashes(null);
			graphicsContext.strokeLine(pixelCoordinateStartPoint.X, pixelCoordinateStartPoint.Y, pixelCoordinateStartPoint.X + symbolWidth, pixelCoordinateStartPoint.Y);

			PixelCoordinate coordinate = new PixelCoordinate(pixelCoordinateStartPoint.X + 0.5 * (symbolWidth - symbolHeight), pixelCoordinateStartPoint.Y - 0.5 * symbolHeight);
			SymbolBuilder.drawSymbol(graphicsContext, coordinate, this.symbolPropertiesMap, observationTypes, symbolHeight);
		}

		public override bool hasContent()
		{
			return base.hasContent() && this.observableMeasurements != null && this.observableMeasurements.Count > 0;
		}
	}

}