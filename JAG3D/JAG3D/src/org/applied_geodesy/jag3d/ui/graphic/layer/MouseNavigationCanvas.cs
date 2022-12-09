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
	using WorldCoordinate = org.applied_geodesy.jag3d.ui.graphic.coordinate.WorldCoordinate;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterEventType = org.applied_geodesy.util.FormatterEventType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using EventHandler = javafx.@event.EventHandler;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using Label = javafx.scene.control.Label;
	using MouseButton = javafx.scene.input.MouseButton;
	using MouseEvent = javafx.scene.input.MouseEvent;
	using ScrollEvent = javafx.scene.input.ScrollEvent;
	using Color = javafx.scene.paint.Color;
	using Font = javafx.scene.text.Font;
	using FontPosture = javafx.scene.text.FontPosture;
	using FontWeight = javafx.scene.text.FontWeight;
	using Text = javafx.scene.text.Text;

	internal class MouseNavigationCanvas : ResizableCanvas, FormatterChangedListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			rubberbandingAndPanEventHandler = new RubberbandingAndPanEventHandler(this);
			scrollEventHandler = new ScrollEventHandler(this);
		}


		private class ScrollEventHandler : EventHandler<ScrollEvent>
		{
			private readonly MouseNavigationCanvas outerInstance;

			public ScrollEventHandler(MouseNavigationCanvas outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ScrollEvent @event)
			{
				double x = @event.getX();
				double y = @event.getY();

				GraphicExtent graphicExtent = outerInstance.CurrentGraphicExtent;
				WorldCoordinate mouseWorldCoord = GraphicExtent.toWorldCoordinate(new PixelCoordinate(x, y), outerInstance.CurrentGraphicExtent);

				double mouseX = mouseWorldCoord.X;
				double mouseY = mouseWorldCoord.Y;

				double minX = graphicExtent.MinX;
				double maxX = graphicExtent.MaxX;
				double minY = graphicExtent.MinY;
				double maxY = graphicExtent.MaxY;

				double extentWidth = graphicExtent.ExtentWidth;
				double extentHeight = graphicExtent.ExtentHeight;


				// Verteilung der Strecken
				double fxMin = Math.Abs(minX - mouseX) / extentWidth;
				double fxMax = Math.Abs(maxX - mouseX) / extentWidth;

				double fyMin = Math.Abs(minY - mouseY) / extentHeight;
				double fyMax = Math.Abs(maxY - mouseY) / extentHeight;

				if (@event.getDeltaY() < 0)
				{
					extentWidth *= 1.15;
					extentHeight *= 1.15;
				}
				else if (@event.getDeltaY() > 0)
				{
					extentWidth /= 1.15;
					extentHeight /= 1.15;
				}
				else
				{
					return;
				}

				double newMinX = mouseX - fxMin * extentWidth;
				double newMaxX = mouseX + fxMax * extentWidth;

				double newMinY = mouseY - fyMin * extentHeight;
				double newMaxY = mouseY + fyMax * extentHeight;

				outerInstance.CurrentGraphicExtent.set(newMinX, newMinY, newMaxX, newMaxY);

				outerInstance.layerManager.draw();
			}
		}

		private class RubberbandingAndPanEventHandler : EventHandler<MouseEvent>
		{
			private readonly MouseNavigationCanvas outerInstance;

			public RubberbandingAndPanEventHandler(MouseNavigationCanvas outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal double xStart = -1, yStart = -1, xEnd = 0, yEnd = 0;
			public override void handle(MouseEvent @event)
			{
				MouseButton mouseButton = @event.getButton();
				this.xEnd = @event.getX();
				this.yEnd = @event.getY();
				outerInstance.CurrentCoordinate = new PixelCoordinate(this.xEnd, this.yEnd);

				if (outerInstance.toolbarType == null || outerInstance.toolbarType == ToolbarType.NONE)
				{
					return;
				}

				// expand plot
				if (mouseButton == MouseButton.PRIMARY && @event.getClickCount() >= 2)
				{
					outerInstance.layerManager.expand();
				}

				// zoom by window
				else if (outerInstance.toolbarType == ToolbarType.WINDOW_ZOOM && @event.getEventType() == MouseEvent.MOUSE_DRAGGED && mouseButton == MouseButton.PRIMARY && @event.isPrimaryButtonDown() && this.xStart >= 0 && this.yStart >= 0)
				{
					outerInstance.drawRect(Math.Min(this.xStart,this.xEnd), Math.Min(this.yStart,this.yEnd), Math.Abs(this.xStart - this.xEnd), Math.Abs(this.yStart - this.yEnd));
				}

				// move map
				else if ((mouseButton == MouseButton.SECONDARY || outerInstance.toolbarType == ToolbarType.MOVE) && @event.getEventType() == MouseEvent.MOUSE_DRAGGED && this.xStart >= 0 && this.yStart >= 0)
				{

					GraphicExtent currentExtent = outerInstance.CurrentGraphicExtent;

					WorldCoordinate startWorldCoordinate = GraphicExtent.toWorldCoordinate(new PixelCoordinate(this.xStart, this.yStart), outerInstance.CurrentGraphicExtent);
					WorldCoordinate endWorldCoordinate = GraphicExtent.toWorldCoordinate(new PixelCoordinate(this.xEnd, this.yEnd), outerInstance.CurrentGraphicExtent);


					double startX = startWorldCoordinate.X;
					double startY = startWorldCoordinate.Y;

					double endX = endWorldCoordinate.X;
					double endY = endWorldCoordinate.Y;


					double minX = currentExtent.MinX;
					double minY = currentExtent.MinY;
					double maxX = currentExtent.MaxX;
					double maxY = currentExtent.MaxY;

					currentExtent.set(minX - (endX - startX), minY - (endY - startY), maxX - (endX - startX), maxY - (endY - startY));

					outerInstance.layerManager.draw();

					this.xStart = @event.getX();
					this.yStart = @event.getY();
				}

				else if (@event.getEventType() == MouseEvent.MOUSE_PRESSED)
				{
					this.xStart = @event.getX();
					this.yStart = @event.getY();
				}

				else if (mouseButton == MouseButton.PRIMARY && @event.getEventType() == MouseEvent.MOUSE_RELEASED && this.xStart >= 0 && this.yStart >= 0)
				{
					if (outerInstance.toolbarType == ToolbarType.WINDOW_ZOOM && this.xStart != this.xEnd && this.yStart != this.yEnd)
					{
						WorldCoordinate startWorldCoord = GraphicExtent.toWorldCoordinate(new PixelCoordinate(this.xStart, this.yStart), outerInstance.CurrentGraphicExtent);
						WorldCoordinate endWorldCoord = GraphicExtent.toWorldCoordinate(new PixelCoordinate(this.xEnd, this.yEnd), outerInstance.CurrentGraphicExtent);
						outerInstance.CurrentGraphicExtent.set(startWorldCoord, endWorldCoord);
					}

					outerInstance.layerManager.draw();

					this.xStart = -1;
					this.yStart = -1;
				}
			}
		}

		private FormatterOptions options = FormatterOptions.Instance;
		private ToolbarType toolbarType = null;
		private Label coordinatePanel;
		private RubberbandingAndPanEventHandler rubberbandingAndPanEventHandler;
		private LayerManager layerManager;
		private ObjectProperty<Color> color = new SimpleObjectProperty<Color>(Color.DARKBLUE);
		private DoubleProperty lineWidth = new SimpleDoubleProperty(1.0);
		private ScrollEventHandler scrollEventHandler;

		internal MouseNavigationCanvas(LayerManager layerManager) : base(layerManager.CurrentGraphicExtent)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			FormatterOptions.Instance.addFormatterChangedListener(this);

			this.layerManager = layerManager;
			this.coordinatePanel = layerManager.CoordinateLabel;

			this.addEventHandler(MouseEvent.MOUSE_MOVED, this.rubberbandingAndPanEventHandler);
			this.addEventHandler(MouseEvent.MOUSE_PRESSED, this.rubberbandingAndPanEventHandler);
			this.addEventHandler(MouseEvent.MOUSE_DRAGGED, this.rubberbandingAndPanEventHandler);
			this.addEventHandler(MouseEvent.MOUSE_RELEASED, this.rubberbandingAndPanEventHandler);

			this.addEventHandler(ScrollEvent.SCROLL_STARTED, this.scrollEventHandler);
			this.addEventHandler(ScrollEvent.SCROLL, this.scrollEventHandler);
			this.addEventHandler(ScrollEvent.SCROLL_FINISHED, this.scrollEventHandler);

			this.Color = Color.BLACK;
			this.LineWidth = 1.25;
		}

		public override void draw()
		{
			base.draw();
			this.drawScaleBar();
		}

		private void drawRect(double x, double y, double w, double h)
		{
			this.clear();

			GraphicsContext gc = this.getGraphicsContext2D();

			// set layer color
			gc.setStroke(this.Color);

			// line properties
			gc.setLineWidth(this.LineWidth);
			gc.setLineDashes(3,5,3,5);
			gc.strokeRect(x, y, w, h);
		}

		private void drawScaleBar()
		{
			GraphicsContext gc = this.getGraphicsContext2D();

			// set layer color
			gc.setStroke(Color.BLACK);

			// set font 
			gc.setFont(Font.font("Monospaced", FontWeight.NORMAL, FontPosture.REGULAR, 10));

			// line properties
			gc.setLineWidth(0.5);
			gc.setLineDashes(null);

			double extentScale = this.CurrentGraphicExtent.Scale;

			if (double.IsInfinity(extentScale) || double.IsNaN(extentScale) || extentScale <= 0)
			{
				return;
			}

			int scaleSegments = 3;
			double scaleBarWidth = 50;
			double scaleBarHeight = 5;
			double padding = 25;

			double x = this.getWidth() - padding;
			double y = this.getHeight() - padding;

			double worldScale = this.options.convertLengthToView(extentScale * scaleBarWidth);
			int exponent = (int)Math.Log10(worldScale);
			double magnitude = Math.Pow(10, exponent);
			double ratio = Math.Ceiling(worldScale / magnitude);

			if ((worldScale / magnitude) < 0.5)
			{ // exponent <= 0 &&
				exponent--;
				magnitude = Math.Pow(10, exponent);
				ratio = Math.Ceiling(worldScale / magnitude);
			}

			scaleBarWidth = this.options.convertLengthToModel(ratio * magnitude / extentScale);
			for (int i = 1; i <= scaleSegments; i++)
			{
				double xi = x - scaleBarWidth * i;
				gc.setFill(i % 2 == 0 ? Color.WHITE : Color.DARKGRAY);
				gc.fillRect(xi, y, scaleBarWidth, scaleBarHeight);
				gc.strokeRect(xi, y, scaleBarWidth, scaleBarHeight);
			}

			string format = "%f %s";
			if (exponent <= -6 || exponent >= 6)
			{
				format = "%.3e %s";
			}
			else if (exponent >= 0)
			{
				format = "%.0f %s";
			}
			else if (exponent < 0)
			{
				format = "%." + Math.Abs(exponent) + "f %s";
			}

			// estimate text size
			Text scaleLabel = new Text(String.format(Locale.ENGLISH, format, scaleSegments * ratio * magnitude, this.options.FormatterOptions[CellValueType.LENGTH].getUnit().getAbbreviation()));
			scaleLabel.setFont(gc.getFont());
			double scaleLabelWidth = scaleLabel.getBoundsInLocal().getWidth();

			gc.setFill(Color.BLACK);
			gc.fillText("0", x - scaleBarWidth * scaleSegments, y - scaleBarHeight);
			gc.fillText(scaleLabel.getText(), x - scaleLabelWidth, y - scaleBarHeight);
		}

		public virtual ToolbarType ToolbarType
		{
			set
			{
				this.toolbarType = value;
			}
			get
			{
				return this.toolbarType;
			}
		}


		private PixelCoordinate CurrentCoordinate
		{
			set
			{
				WorldCoordinate worldCoordinate = GraphicExtent.toWorldCoordinate(value, this.CurrentGraphicExtent);
				if (double.IsNaN(worldCoordinate.X) || double.IsNaN(worldCoordinate.Y))
				{
					this.coordinatePanel.setText(null);
				}
				else
				{
					string x = this.options.toLengthFormat(worldCoordinate.X, true);
					string y = this.options.toLengthFormat(worldCoordinate.Y, true);
					// x,y (math. System)
					this.coordinatePanel.setText(string.Format("[y = {0} / x = {1}]", x, y));
				}
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


		public virtual void formatterChanged(FormatterEvent evt)
		{
			if (evt.EventType == FormatterEventType.UNIT_CHANGED && evt.CellType == CellValueType.LENGTH)
			{
				this.draw();
			}
		}
	}

}