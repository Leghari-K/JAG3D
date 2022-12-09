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
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using PointSymbolType = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.PointSymbolType;
	using SymbolBuilder = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.SymbolBuilder;
	using GraphicPoint = org.applied_geodesy.jag3d.ui.graphic.sql.GraphicPoint;

	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using SimpleStringProperty = javafx.beans.property.SimpleStringProperty;
	using StringProperty = javafx.beans.property.StringProperty;
	using FXCollections = javafx.collections.FXCollections;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Color = javafx.scene.paint.Color;
	using StrokeLineCap = javafx.scene.shape.StrokeLineCap;
	using Font = javafx.scene.text.Font;
	using FontPosture = javafx.scene.text.FontPosture;
	using FontWeight = javafx.scene.text.FontWeight;

	public class PointLayer : Layer, HighlightableLayer, FontLayer
	{
		private DoubleProperty fontSize = new SimpleDoubleProperty(10);
		private StringProperty fontFamily = new SimpleStringProperty(Font.getDefault().getFamily());
		private ObjectProperty<Color> fontColor = new SimpleObjectProperty<Color>(Color.DIMGREY);
		private ObjectProperty<PointSymbolType> pointSymbolType = new SimpleObjectProperty<PointSymbolType>(PointSymbolType.STROKED_CIRCLE);
		private IList<GraphicPoint> points = FXCollections.observableArrayList();

		private ObjectProperty<Color> highlightColor = new SimpleObjectProperty<Color>(Color.ORANGERED); //#FF4500
		private DoubleProperty highlightLineWidth = new SimpleDoubleProperty(2.5);
		private ObjectProperty<TableRowHighlightType> highlightType = new SimpleObjectProperty<TableRowHighlightType>(TableRowHighlightType.NONE);

		private BooleanProperty point1DVisible = new SimpleBooleanProperty(false);
		private BooleanProperty point2DVisible = new SimpleBooleanProperty(true);
		private BooleanProperty point3DVisible = new SimpleBooleanProperty(true);

		internal PointLayer(LayerType layerType) : base(layerType)
		{

			Color symbolColor, fontColor, highlightColor;
			TableRowHighlightType highlightType;
			PointSymbolType pointSymbolType;
			string fontFamily = null;
			double symbolSize = -1, lineWidth = -1, fontSize = -1, highlightLineWidth = -1;
			bool visible = true;

			switch (layerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
				fontFamily = PROPERTIES.getProperty("REFERENCE_POINT_APRIORI_FONT_FAMILY", "System");

				try
				{
					fontColor = Color.web(PROPERTIES.getProperty("REFERENCE_POINT_APRIORI_FONT_COLOR", "#696969"));
				}
				catch (Exception)
				{
					fontColor = Color.web("#696969");
				}

				try
				{
					symbolColor = Color.web(PROPERTIES.getProperty("REFERENCE_POINT_APRIORI_COLOR", "#90ee90"));
				}
				catch (Exception)
				{
					symbolColor = Color.web("#90ee90");
				}

				try
				{
					pointSymbolType = PointSymbolType.valueOf(PROPERTIES.getProperty("REFERENCE_POINT_APRIORI_SYMBOL_TYPE", "STROKED_SQUARE"));
				}
				catch (Exception)
				{
					pointSymbolType = PointSymbolType.STROKED_SQUARE;
				}

				try
				{
					fontSize = double.Parse(PROPERTIES.getProperty("REFERENCE_POINT_APRIORI_FONT_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("REFERENCE_POINT_APRIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("REFERENCE_POINT_APRIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("REFERENCE_POINT_APRIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
				fontFamily = PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_FONT_FAMILY", "System");

				try
				{
					fontColor = Color.web(PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_FONT_COLOR", "#696969"));
				}
				catch (Exception)
				{
					fontColor = Color.web("#696969");
				}

				try
				{
					symbolColor = Color.web(PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_COLOR", "#006400"));
				}
				catch (Exception)
				{
					symbolColor = Color.web("#006400");
				}

				try
				{
					highlightType = TableRowHighlightType.valueOf(PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_HIGHLIGHT_TYPE", "NONE"));
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
					highlightColor = Color.web(PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_HIGHLIGHT_COLOR", "#FF4500"));
				}
				catch (Exception)
				{
					highlightColor = Color.web("#FF4500");
				}

				try
				{
					pointSymbolType = PointSymbolType.valueOf(PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_SYMBOL_TYPE", "STROKED_SQUARE"));
				}
				catch (Exception)
				{
					pointSymbolType = PointSymbolType.STROKED_SQUARE;
				}

				try
				{
					fontSize = double.Parse(PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_FONT_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				try
				{
					highlightLineWidth = double.Parse(PROPERTIES.getProperty("REFERENCE_POINT_APOSTERIORI_HIGHLIGHT_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				this.HighlightLineWidth = highlightLineWidth >= 0 ? highlightLineWidth : 2.5;
				this.HighlightColor = highlightColor;
				this.HighlightType = highlightType;

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.STOCHASTIC_POINT_APRIORI:
				fontFamily = PROPERTIES.getProperty("STOCHASTIC_POINT_APRIORI_FONT_FAMILY", "System");

				try
				{
					fontColor = Color.web(PROPERTIES.getProperty("STOCHASTIC_POINT_APRIORI_FONT_COLOR", "#696969"));
				}
				catch (Exception)
				{
					fontColor = Color.web("#696969");
				}

				try
				{
					symbolColor = Color.web(PROPERTIES.getProperty("STOCHASTIC_POINT_APRIORI_COLOR", "#ffd700"));
				}
				catch (Exception)
				{
					symbolColor = Color.web("#ffd700");
				}

				try
				{
					pointSymbolType = PointSymbolType.valueOf(PROPERTIES.getProperty("STOCHASTIC_POINT_APRIORI_SYMBOL_TYPE", "STROKED_UPRIGHT_TRIANGLE"));
				}
				catch (Exception)
				{
					pointSymbolType = PointSymbolType.STROKED_SQUARE;
				}

				try
				{
					fontSize = double.Parse(PROPERTIES.getProperty("STOCHASTIC_POINT_APRIORI_FONT_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("STOCHASTIC_POINT_APRIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("STOCHASTIC_POINT_APRIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("STOCHASTIC_POINT_APRIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
				fontFamily = PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_FONT_FAMILY", "System");

				try
				{
					fontColor = Color.web(PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_FONT_COLOR", "#696969"));
				}
				catch (Exception)
				{
					fontColor = Color.web("#696969");
				}

				try
				{
					symbolColor = Color.web(PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_COLOR", "#daa520"));
				}
				catch (Exception)
				{
					symbolColor = Color.web("#daa520");
				}

				try
				{
					highlightType = TableRowHighlightType.valueOf(PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_HIGHLIGHT_TYPE", "NONE"));
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
					highlightColor = Color.web(PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_HIGHLIGHT_COLOR", "#FF4500"));
				}
				catch (Exception)
				{
					highlightColor = Color.web("#FF4500");
				}

				try
				{
					pointSymbolType = PointSymbolType.valueOf(PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_SYMBOL_TYPE", "STROKED_UPRIGHT_TRIANGLE"));
				}
				catch (Exception)
				{
					pointSymbolType = PointSymbolType.STROKED_SQUARE;
				}

				try
				{
					fontSize = double.Parse(PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_FONT_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				try
				{
					highlightLineWidth = double.Parse(PROPERTIES.getProperty("STOCHASTIC_POINT_APOSTERIORI_HIGHLIGHT_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				this.HighlightLineWidth = highlightLineWidth >= 0 ? highlightLineWidth : 2.5;
				this.HighlightColor = highlightColor;
				this.HighlightType = highlightType;

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.DATUM_POINT_APRIORI:
				fontFamily = PROPERTIES.getProperty("DATUM_POINT_APRIORI_FONT_FAMILY", "System");

				try
				{
					fontColor = Color.web(PROPERTIES.getProperty("DATUM_POINT_APRIORI_FONT_COLOR", "#696969"));
				}
				catch (Exception)
				{
					fontColor = Color.web("#696969");
				}

				try
				{
					symbolColor = Color.web(PROPERTIES.getProperty("DATUM_POINT_APRIORI_COLOR", "#87ceeb"));
				}
				catch (Exception)
				{
					symbolColor = Color.web("#87ceeb");
				}

				try
				{
					pointSymbolType = PointSymbolType.valueOf(PROPERTIES.getProperty("DATUM_POINT_APRIORI_SYMBOL_TYPE", "STROKED_HEXAGON"));
				}
				catch (Exception)
				{
					pointSymbolType = PointSymbolType.STROKED_SQUARE;
				}

				try
				{
					fontSize = double.Parse(PROPERTIES.getProperty("DATUM_POINT_APRIORI_FONT_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("DATUM_POINT_APRIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("DATUM_POINT_APRIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("DATUM_POINT_APRIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
				fontFamily = PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_FONT_FAMILY", "System");

				try
				{
					fontColor = Color.web(PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_FONT_COLOR", "#696969"));
				}
				catch (Exception)
				{
					fontColor = Color.web("#696969");
				}

				try
				{
					symbolColor = Color.web(PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_COLOR", "#1e90ff"));
				}
				catch (Exception)
				{
					symbolColor = Color.web("#1e90ff");
				}

				try
				{
					highlightType = TableRowHighlightType.valueOf(PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_HIGHLIGHT_TYPE", "NONE"));
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
					highlightColor = Color.web(PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_HIGHLIGHT_COLOR", "#FF4500"));
				}
				catch (Exception)
				{
					highlightColor = Color.web("#FF4500");
				}

				try
				{
					pointSymbolType = PointSymbolType.valueOf(PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_SYMBOL_TYPE", "STROKED_HEXAGON"));
				}
				catch (Exception)
				{
					pointSymbolType = PointSymbolType.STROKED_SQUARE;
				}

				try
				{
					fontSize = double.Parse(PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_FONT_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				try
				{
					highlightLineWidth = double.Parse(PROPERTIES.getProperty("DATUM_POINT_APOSTERIORI_HIGHLIGHT_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				this.HighlightLineWidth = highlightLineWidth >= 0 ? highlightLineWidth : 2.5;
				this.HighlightColor = highlightColor;
				this.HighlightType = highlightType;

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.NEW_POINT_APRIORI:
				fontFamily = PROPERTIES.getProperty("NEW_POINT_APRIORI_FONT_FAMILY", "System");

				try
				{
					fontColor = Color.web(PROPERTIES.getProperty("NEW_POINT_APRIORI_FONT_COLOR", "#696969"));
				}
				catch (Exception)
				{
					fontColor = Color.web("#696969");
				}

				try
				{
					symbolColor = Color.web(PROPERTIES.getProperty("NEW_POINT_APRIORI_COLOR", "#c480c4"));
				}
				catch (Exception)
				{
					symbolColor = Color.web("#c480c4");
				}

				try
				{
					pointSymbolType = PointSymbolType.valueOf(PROPERTIES.getProperty("NEW_POINT_APRIORI_SYMBOL_TYPE", "STROKED_CIRCLE"));
				}
				catch (Exception)
				{
					pointSymbolType = PointSymbolType.STROKED_SQUARE;
				}

				try
				{
					fontSize = double.Parse(PROPERTIES.getProperty("NEW_POINT_APRIORI_FONT_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("NEW_POINT_APRIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("NEW_POINT_APRIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("NEW_POINT_APRIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				break;

			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
				fontFamily = PROPERTIES.getProperty("NEW_POINT_APOSTERIORI_FONT_FAMILY", "System");

				try
				{
					fontColor = Color.web(PROPERTIES.getProperty("NEW_POINT_APOSTERIORI_FONT_COLOR", "#696969"));
				}
				catch (Exception)
				{
					fontColor = Color.web("#696969");
				}

				try
				{
					symbolColor = Color.web(PROPERTIES.getProperty("NEW_POINT_APOSTERIORI_COLOR", "#dda0dd"));
				}
				catch (Exception)
				{
					symbolColor = Color.web("#dda0dd");
				}

				try
				{
					pointSymbolType = PointSymbolType.valueOf(PROPERTIES.getProperty("NEW_POINT_APOSTERIORI_SYMBOL_TYPE", "STROKED_CIRCLE"));
				}
				catch (Exception)
				{
					pointSymbolType = PointSymbolType.STROKED_SQUARE;
				}

				try
				{
					fontSize = double.Parse(PROPERTIES.getProperty("NEW_POINT_APOSTERIORI_FONT_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					symbolSize = double.Parse(PROPERTIES.getProperty("NEW_POINT_APOSTERIORI_SYMBOL_SIZE"));
				}
				catch (Exception)
				{
				}
				try
				{
					lineWidth = double.Parse(PROPERTIES.getProperty("NEW_POINT_APOSTERIORI_LINE_WIDTH"));
				}
				catch (Exception)
				{
				}
				try
				{
					visible = PROPERTIES.getProperty("NEW_POINT_APOSTERIORI_VISIBLE").equalsIgnoreCase("TRUE");
				}
				catch (Exception)
				{
				}

				break;

			default:
				throw new System.ArgumentException("Error, unsupported layer type " + layerType);
			}

			symbolSize = symbolSize >= 0 ? symbolSize : SymbolBuilder.DEFAULT_SIZE;
			lineWidth = lineWidth >= 0 ? lineWidth : 1.5;
			fontSize = fontSize >= 0 ? fontSize : 10.0;
			fontFamily = !string.ReferenceEquals(fontFamily, null) ? fontFamily : "System";

			this.SymbolType = pointSymbolType;
			this.Color = symbolColor;
			this.LineWidth = lineWidth;
			this.SymbolSize = symbolSize;

			this.FontSize = fontSize;
			this.FontFamily = fontFamily;
			this.FontColor = fontColor;

			this.Visible = visible;
		}

		public override void draw(GraphicsContext graphicsContext, GraphicExtent graphicExtent)
		{
			if (!this.Visible || this.points.Count == 0)
			{
				return;
			}

	//		graphicsContext.setLineCap(StrokeLineCap.BUTT);
	//		graphicsContext.setLineDashes(null);

			PointSymbolType symbolType = this.PointSymbolType;
			double symbolSize = this.SymbolSize;
			double fontSize = this.FontSize;
			string fontFamily = this.FontFamily;

			// draw points
			foreach (GraphicPoint point in this.points)
			{
				if (!point.Visible)
				{
					continue;
				}

				PixelCoordinate pixelCoordinate = GraphicExtent.toPixelCoordinate(point.Coordinate, graphicExtent);

				if (this.contains(graphicExtent, pixelCoordinate))
				{
					// set layer color and line width properties
					Color symbolColor = this.Color;
					Color fontColor = this.FontColor;
					double lineWidth = this.LineWidth;

					TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
					double leftBoundary = tableRowHighlight.getLeftBoundary(this.HighlightType);
					double rightBoundary = tableRowHighlight.getRightBoundary(this.HighlightType);

					switch (this.HighlightType.innerEnumValue)
					{
					case TableRowHighlightType.InnerEnum.INFLUENCE_ON_POSITION:
						if (point.MaxInfluenceOnPosition > rightBoundary)
						{
							symbolColor = this.HighlightColor;
							fontColor = this.HighlightColor;
							lineWidth = this.HighlightLineWidth;
						}
						break;
					case TableRowHighlightType.InnerEnum.P_PRIO_VALUE:
						if (point.Pprio < Math.Log(leftBoundary / 100.0))
						{
							symbolColor = this.HighlightColor;
							fontColor = this.HighlightColor;
							lineWidth = this.HighlightLineWidth;
						}
						break;
					case TableRowHighlightType.InnerEnum.REDUNDANCY:
						if (point.MinRedundancy < leftBoundary)
						{
							symbolColor = this.HighlightColor;
							fontColor = this.HighlightColor;
							lineWidth = this.HighlightLineWidth;
						}
						break;
					case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
						if (point.Significant)
						{
							symbolColor = this.HighlightColor;
							fontColor = this.HighlightColor;
							lineWidth = this.HighlightLineWidth;
						}
						break;
					case TableRowHighlightType.InnerEnum.GROSS_ERROR:
						if (point.GrossErrorExceeded)
						{
							symbolColor = this.HighlightColor;
							fontColor = this.HighlightColor;
							lineWidth = this.HighlightLineWidth;
						}
						break;
					case TableRowHighlightType.InnerEnum.NONE: // DEFAULT
						symbolColor = this.Color;
						fontColor = this.FontColor;
						lineWidth = this.LineWidth;
						break;
					}

					this.drawPointSymbol(graphicsContext, pixelCoordinate, symbolColor, symbolType, symbolSize, lineWidth);
					this.drawPointText(graphicsContext, pixelCoordinate, point.Name.Trim(), fontColor, fontFamily, symbolSize, lineWidth, fontSize);
				}
			}
		}

		private void drawPointSymbol(GraphicsContext graphicsContext, PixelCoordinate pixelCoordinate, Color color, PointSymbolType symbolType, double symbolSize, double lineWidth)
		{
			graphicsContext.setLineCap(StrokeLineCap.BUTT);
			graphicsContext.setLineDashes(null);

			graphicsContext.setLineWidth(lineWidth);
			graphicsContext.setStroke(color);
			graphicsContext.setFill(color);

			SymbolBuilder.drawSymbol(graphicsContext, pixelCoordinate, symbolType, symbolSize);
		}

		private void drawPointText(GraphicsContext graphicsContext, PixelCoordinate pixelCoordinate, string name, Color color, string fontFamily, double symbolSize, double lineWidth, double fontSize)
		{
			// set text color
			graphicsContext.setStroke(color);
			graphicsContext.setFill(color);
			graphicsContext.setFont(Font.font(fontFamily, FontWeight.NORMAL, FontPosture.REGULAR, fontSize));
			graphicsContext.fillText(name, pixelCoordinate.X + 0.5 * (symbolSize + lineWidth + 1), pixelCoordinate.Y + 0.5 * (symbolSize + fontSize + lineWidth + 1));
		}

		public override void clearLayer()
		{
			this.points.Clear();
		}

		public override GraphicExtent MaximumGraphicExtent
		{
			get
			{
				GraphicExtent graphicExtent = new GraphicExtent();
				graphicExtent.reset();
				if (this.points != null)
				{
					foreach (GraphicPoint point in this.points)
					{
						if (point.Visible)
						{
							graphicExtent.merge(point.Coordinate);
						}
					}
				}
				return graphicExtent;
			}
		}

		public virtual IList<GraphicPoint> Points
		{
			set
			{
				this.points.Clear();
				if (value != null)
				{
					foreach (GraphicPoint point in value)
					{
						int dimension = point.Dimension;
						switch (dimension)
						{
						case 1:
							point.Visible = this.Point1DVisible;
							break;
						case 2:
							point.Visible = this.Point2DVisible;
							break;
						case 3:
							point.Visible = this.Point3DVisible;
							break;
						default:
							continue;
						}
						this.points.Add(point);
					}
				}
			}
			get
			{
				return this.points;
			}
		}


		private void setPointVisible(int dimension, bool visible)
		{
			foreach (GraphicPoint point in this.points)
			{
				if (point.Dimension == dimension)
				{
					point.Visible = visible;
				}
			}
		}

		public virtual ObjectProperty<PointSymbolType> pointSymbolTypeProperty()
		{
			return this.pointSymbolType;
		}

		public virtual PointSymbolType PointSymbolType
		{
			get
			{
				return this.pointSymbolTypeProperty().get();
			}
		}

		public virtual in PointSymbolType SymbolType
		{
			set
			{
				this.pointSymbolTypeProperty().set(value);
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
			switch (this.LayerType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
				return i18n.getString("PointLayer.type.datum.aposteriori", "Datum points (a-posteriori)");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.DATUM_POINT_APRIORI:
				return i18n.getString("PointLayer.type.datum.apriori", "Datum points (a-priori)");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
				return i18n.getString("PointLayer.type.new.aposteriori", "New points (a-posteriori)");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.NEW_POINT_APRIORI:
				return i18n.getString("PointLayer.type.new.apriori", "New points (a-priori)");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
				return i18n.getString("PointLayer.type.reference.aposteriori", "Reference points (a-posteriori)");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
				return i18n.getString("PointLayer.type.reference.apriori", "Reference points (a-priori)");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
				return i18n.getString("PointLayer.type.stochastic.aposteriori", "Stochastic points (a-posteriori)");
			case org.applied_geodesy.jag3d.ui.graphic.layer.LayerType.InnerEnum.STOCHASTIC_POINT_APRIORI:
				return i18n.getString("PointLayer.type.stochastic.apriori", "Stochastic points (a-priori)");
			default:
				return "";
			}
		}

		public virtual BooleanProperty point1DVisibleProperty()
		{
			return this.point1DVisible;
		}

		public virtual bool Point1DVisible
		{
			get
			{
				return this.point1DVisibleProperty().get();
			}
			set
			{
				this.setPointVisible(1, value);
				this.point1DVisibleProperty().set(value);
			}
		}


		public virtual BooleanProperty point2DVisibleProperty()
		{
			return this.point2DVisible;
		}

		public virtual bool Point2DVisible
		{
			get
			{
				return this.point2DVisibleProperty().get();
			}
			set
			{
				this.setPointVisible(2, value);
				this.point2DVisibleProperty().set(value);
			}
		}


		public virtual BooleanProperty point3DVisibleProperty()
		{
			return this.point3DVisible;
		}

		public virtual bool Point3DVisible
		{
			get
			{
				return this.point3DVisibleProperty().get();
			}
			set
			{
				this.setPointVisible(3, value);
				this.point3DVisibleProperty().set(value);
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


		public override void drawLegendSymbol(GraphicsContext graphicsContext, GraphicExtent graphicExtent, PixelCoordinate pixelCoordinate, double symbolHeight, double symbolWidth)
		{
			if (this.contains(graphicExtent, pixelCoordinate))
			{
				PointSymbolType symbolType = this.PointSymbolType;
				Color symbolColor = this.Color;
				double lineWidth = this.LineWidth;
				double symbolSize = symbolHeight;

				this.drawPointSymbol(graphicsContext, new PixelCoordinate(pixelCoordinate.X + 0.5 * symbolWidth, pixelCoordinate.Y), symbolColor, symbolType, symbolSize, lineWidth);
			}
		}

		public override bool hasContent()
		{
			return base.hasContent() && this.points != null && this.points.Count > 0;
		}
	}

}