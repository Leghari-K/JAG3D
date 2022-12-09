﻿/// <summary>
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

namespace org.applied_geodesy.jag3d.ui.graphic.layer.dialog
{
	using PixelCoordinate = org.applied_geodesy.jag3d.ui.graphic.coordinate.PixelCoordinate;
	using ArrowSymbolType = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.ArrowSymbolType;
	using SymbolBuilder = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.SymbolBuilder;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Canvas = javafx.scene.canvas.Canvas;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using ListCell = javafx.scene.control.ListCell;
	using Color = javafx.scene.paint.Color;

	public class ArrowSymbolTypeListCell : ListCell<ArrowSymbolType>
	{
		private I18N i18n = I18N.Instance;

		protected internal virtual void updateItem(ArrowSymbolType symbolType, bool empty)
		{
			base.updateItem(symbolType, empty);

			this.setGraphic(null);
			this.setText(null);

			if (symbolType != null)
			{
				this.setGraphic(this.createSymbolImage(symbolType));
				this.setText(this.getText(symbolType));
			}
		}

		private Canvas createSymbolImage(ArrowSymbolType symbolType)
		{
			double width = 4.0 * SymbolBuilder.DEFAULT_SIZE;
			double height = 1.5 * SymbolBuilder.DEFAULT_SIZE;
			Canvas canvas = new Canvas(width, height);
			GraphicsContext graphicsContext = canvas.getGraphicsContext2D();
			graphicsContext.setStroke(Color.BLACK);
			graphicsContext.setFill(Color.BLACK);
			graphicsContext.strokeLine(0.05 * width, 0.5 * height, 0.75 * width, 0.5 * height);
			SymbolBuilder.drawSymbol(graphicsContext, new PixelCoordinate(0.05 * width, 0.5 * height), symbolType, 1.3 * SymbolBuilder.DEFAULT_SIZE, Math.PI);
			return canvas;
		}

		private string getText(ArrowSymbolType symbolType)
		{
			switch (symbolType.innerEnumValue)
			{
			case ArrowSymbolType.InnerEnum.FILLED_TETRAGON_ARROW:
				return i18n.getString("ArrowSymbolTypeListCell.symbol.filled_tetragon_arrow.label", "Filled tetragon arrow");
			case ArrowSymbolType.InnerEnum.FILLED_TRIANGLE_ARROW:
				return i18n.getString("ArrowSymbolTypeListCell.symbol.filled_triangle_arrow.label", "Filled triangle arrow");
			case ArrowSymbolType.InnerEnum.STROKED_ARROW:
				return i18n.getString("ArrowSymbolTypeListCell.symbol.stroked_arrow.label", "Stroked arrow");
			case ArrowSymbolType.InnerEnum.STROKED_TETRAGON_ARROW:
				return i18n.getString("ArrowSymbolTypeListCell.symbol.stroked_tetragon_arrow.label", "Stroked tetragon arrow");
			case ArrowSymbolType.InnerEnum.STROKED_TRIANGLE_ARROW:
				return i18n.getString("ArrowSymbolTypeListCell.symbol.stroked_triangle_arrow.label", "Stroked triangle arrow");
			}
			return null;
		}
	}
}