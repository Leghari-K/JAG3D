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

namespace org.applied_geodesy.jag3d.ui.graphic.util
{
	using PixelCoordinate = org.applied_geodesy.jag3d.ui.graphic.coordinate.PixelCoordinate;
	using WorldCoordinate = org.applied_geodesy.jag3d.ui.graphic.coordinate.WorldCoordinate;

	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using DoubleProperty = javafx.beans.property.DoubleProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using SimpleDoubleProperty = javafx.beans.property.SimpleDoubleProperty;

	public class GraphicExtent
	{
		private DoubleProperty minX = new SimpleDoubleProperty(0.0);
		private DoubleProperty minY = new SimpleDoubleProperty(0.0);
		private DoubleProperty maxX = new SimpleDoubleProperty(0.0);
		private DoubleProperty maxY = new SimpleDoubleProperty(0.0);

//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private BooleanProperty extended_Conflict = new SimpleBooleanProperty(false);

		private DoubleProperty drawingBoardWidth = new SimpleDoubleProperty(0.0);
		private DoubleProperty drawingBoardHeight = new SimpleDoubleProperty(0.0);

		public GraphicExtent()
		{
			this.reset();
		}

		public virtual GraphicExtent merge(GraphicExtent graphicExtent)
		{
			bool isExtended = false;

			if (this.MinX > graphicExtent.MinX)
			{
				this.MinX = graphicExtent.MinX;
				isExtended = true;
			}

			if (this.MinY > graphicExtent.MinY)
			{
				this.MinY = graphicExtent.MinY;
				isExtended = true;
			}

			if (this.MaxX < graphicExtent.MaxX)
			{
				this.MaxX = graphicExtent.MaxX;
				isExtended = true;
			}

			if (this.MaxY < graphicExtent.MaxY)
			{
				this.MaxY = graphicExtent.MaxY;
				isExtended = true;
			}

			if (isExtended)
			{
				this.extended();
			}

			return this;
		}

		public virtual void reset()
		{
			bool isExtended = false;

			if (this.MinX != double.PositiveInfinity)
			{ // MAX_VALUE
				this.MinX = double.PositiveInfinity;
				isExtended = true;
			}

			if (this.MinY != double.PositiveInfinity)
			{ // MAX_VALUE
				this.MinY = double.PositiveInfinity;
				isExtended = true;
			}

			if (this.MaxX != double.NegativeInfinity)
			{ // -MAX_VALUE
				this.MaxX = double.NegativeInfinity;
				isExtended = true;
			}

			if (this.MaxY != double.NegativeInfinity)
			{ // -MAX_VALUE
				this.MaxY = double.NegativeInfinity;
				isExtended = true;
			}

			if (isExtended)
			{
				this.extended();
			}
		}

		public virtual double CentreX
		{
			get
			{
				return this.MinX + 0.5 * this.ExtentWidth;
			}
		}

		public virtual double CentreY
		{
			get
			{
				return this.MinY + 0.5 * this.ExtentHeight;
			}
		}

		public virtual GraphicExtent merge(WorldCoordinate worldCoordinate)
		{
			return this.merge(worldCoordinate.X, worldCoordinate.Y);
		}

		public virtual GraphicExtent merge(double x, double y)
		{
			bool isExtended = false;

			if (this.MinX > x)
			{
				this.MinX = x;
				isExtended = true;
			}

			if (this.MaxX < x)
			{
				this.MaxX = x;
				isExtended = true;
			}

			if (this.MinY > y)
			{
				this.MinY = y;
				isExtended = true;
			}

			if (this.MaxY < y)
			{
				this.MaxY = y;
				isExtended = true;
			}

			if (isExtended)
			{
				this.extended();
			}

			return this;
		}

		public virtual void set(GraphicExtent graphicExtent)
		{
			double xmin = graphicExtent.MinX;
			double ymin = graphicExtent.MinY;
			double xmax = graphicExtent.MaxX;
			double ymax = graphicExtent.MaxY;
			this.set(xmin, ymin, xmax, ymax);
		}

		public virtual void set(WorldCoordinate minCoordinate, WorldCoordinate maxCoordinate)
		{
			double xmin = minCoordinate.X;
			double ymin = minCoordinate.Y;
			double xmax = maxCoordinate.X;
			double ymax = maxCoordinate.Y;
			this.set(xmin, ymin, xmax, ymax);
		}

		public virtual void set(double xmin, double ymin, double xmax, double ymax)
		{
			bool isExtended = false;

			if (xmin > xmax)
			{
				isExtended = this.MinX != xmax || this.MaxX != xmin;
				this.MinX = xmax;
				this.MaxX = xmin;

			}
			else
			{
				isExtended = this.MinX != xmin || this.MaxX != xmax;
				this.MinX = xmin;
				this.MaxX = xmax;
			}

			if (ymin > ymax)
			{
				isExtended = this.MinY != ymax || this.MaxY != ymin;
				this.MinY = ymax;
				this.MaxY = ymin;
			}
			else
			{
				isExtended = this.MinY != ymin || this.MaxY != ymax;
				this.MinY = ymin;
				this.MaxY = ymax;
			}

			if (isExtended)
			{
				this.extended();
			}
		}

		public virtual double Scale
		{
			get
			{
				return getScale(this.DrawingBoardHeight, this.DrawingBoardWidth, this.ExtentHeight, this.ExtentWidth);
			}
			set
			{
				double pixelCentreX = 0.5 * this.DrawingBoardWidth;
				double pixelCentreY = 0.5 * this.DrawingBoardHeight;
    
				double newMinX = pixelCentreX - 0.5 * this.DrawingBoardWidth;
				double newMaxX = pixelCentreX + 0.5 * this.DrawingBoardWidth;
    
				double newMinY = pixelCentreY + 0.5 * this.DrawingBoardHeight;
				double newMaxY = pixelCentreY - 0.5 * this.DrawingBoardHeight;
    
				WorldCoordinate minWorldCoord = toWorldCoordinate(new PixelCoordinate(newMinX, newMinY), this, value);
				WorldCoordinate maxWorldCoord = toWorldCoordinate(new PixelCoordinate(newMaxX, newMaxY), this, value);
    
				this.set(minWorldCoord, maxWorldCoord);
			}
		}


		public virtual bool contains(double x, double y)
		{
			return x >= this.MinX && x <= this.MaxX && y >= this.MinY && y <= this.MaxY;
		}

		public virtual double ExtentWidth
		{
			get
			{
				return Math.Abs(this.MaxX - this.MinX);
			}
		}

		public virtual double ExtentHeight
		{
			get
			{
				return Math.Abs(this.MaxY - this.MinY);
			}
		}

		public override string ToString()
		{
			return "GraphicExtent [Min=(" + this.MinX + ", " + this.MinY + ") Max=(" + this.MaxX + ", " + this.MaxY + ")]";
		}

		public virtual DoubleProperty minXProperty()
		{
			return this.minX;
		}

		public virtual double MinX
		{
			get
			{
				return this.minXProperty().get();
			}
			set
			{
				this.minXProperty().set(value);
			}
		}


		public virtual DoubleProperty minYProperty()
		{
			return this.minY;
		}

		public virtual double MinY
		{
			get
			{
				return this.minYProperty().get();
			}
			set
			{
				this.minYProperty().set(value);
			}
		}


		public virtual DoubleProperty maxXProperty()
		{
			return this.maxX;
		}

		public virtual double MaxX
		{
			get
			{
				return this.maxXProperty().get();
			}
			set
			{
				this.maxXProperty().set(value);
			}
		}


		public virtual DoubleProperty maxYProperty()
		{
			return this.maxY;
		}

		public virtual double MaxY
		{
			get
			{
				return this.maxYProperty().get();
			}
			set
			{
				this.maxYProperty().set(value);
			}
		}


		public virtual DoubleProperty drawingBoardHeightProperty()
		{
			return this.drawingBoardHeight;
		}

		public virtual double DrawingBoardHeight
		{
			get
			{
				return this.drawingBoardHeightProperty().get();
			}
			set
			{
				this.drawingBoardHeightProperty().set(value);
			}
		}


		public virtual DoubleProperty drawingBoardWidthProperty()
		{
			return this.drawingBoardWidth;
		}

		public virtual double DrawingBoardWidth
		{
			get
			{
				return this.drawingBoardWidthProperty().get();
			}
			set
			{
				this.drawingBoardWidthProperty().set(value);
			}
		}


		public static PixelCoordinate toPixelCoordinate(WorldCoordinate coordinate, GraphicExtent graphicExtent)
		{
			return toPixelCoordinate(coordinate, graphicExtent, graphicExtent.Scale);
		}

		public static WorldCoordinate toWorldCoordinate(PixelCoordinate coordinate, GraphicExtent graphicExtent)
		{
			return toWorldCoordinate(coordinate, graphicExtent, graphicExtent.Scale);
		}

		public static WorldCoordinate toWorldCoordinate(PixelCoordinate coordinate, GraphicExtent graphicExtent, double scale)
		{
			double x = coordinate.X;
			double y = coordinate.Y;

			return new WorldCoordinate(GraphicUtils.toWorldCoordinate(x - 0.5 * graphicExtent.DrawingBoardWidth, graphicExtent.CentreX, scale), GraphicUtils.toWorldCoordinate(0.5 * graphicExtent.DrawingBoardHeight - y, graphicExtent.CentreY, scale));
		}

		public static PixelCoordinate toPixelCoordinate(WorldCoordinate coordinate, GraphicExtent graphicExtent, double scale)
		{
			double x = coordinate.X;
			double y = coordinate.Y;

			return new PixelCoordinate(0.5 * graphicExtent.DrawingBoardWidth + GraphicUtils.toPixelCoordinate(x, graphicExtent.CentreX, scale), 0.5 * graphicExtent.DrawingBoardHeight - GraphicUtils.toPixelCoordinate(y, graphicExtent.CentreY, scale));
		}

		public static double getScale(double drawingBoardHeight, double drawingBoardWidth, double extentHeight, double extentWidth)
		{
			double scaleHeight = drawingBoardHeight > 0 ? extentHeight / drawingBoardHeight : -1;
			double scaleWidth = drawingBoardWidth > 0 ? extentWidth / drawingBoardWidth : -1;
			return Math.Max(scaleHeight, scaleWidth);
		}

		public BooleanProperty extendedProperty()
		{
			return this.extended_Conflict;
		}

		private bool Extended
		{
			get
			{
				return this.extendedProperty().get();
			}
			set
			{
				this.extendedProperty().set(value);
			}
		}


		private void extended()
		{
			this.Extended = !this.Extended;
		}
	}

}