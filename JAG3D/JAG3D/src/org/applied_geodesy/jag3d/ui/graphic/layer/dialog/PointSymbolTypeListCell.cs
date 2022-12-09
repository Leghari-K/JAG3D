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

namespace org.applied_geodesy.jag3d.ui.graphic.layer.dialog
{
	using PixelCoordinate = org.applied_geodesy.jag3d.ui.graphic.coordinate.PixelCoordinate;
	using PointSymbolType = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.PointSymbolType;
	using SymbolBuilder = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.SymbolBuilder;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Canvas = javafx.scene.canvas.Canvas;
	using GraphicsContext = javafx.scene.canvas.GraphicsContext;
	using ListCell = javafx.scene.control.ListCell;
	using Color = javafx.scene.paint.Color;

	public class PointSymbolTypeListCell : ListCell<PointSymbolType>
	{
		private I18N i18n = I18N.Instance;

		protected internal virtual void updateItem(PointSymbolType symbolType, bool empty)
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

		private Canvas createSymbolImage(PointSymbolType symbolType)
		{
			double width = 1.7 * SymbolBuilder.DEFAULT_SIZE;
			double height = 1.7 * SymbolBuilder.DEFAULT_SIZE;
			Canvas canvas = new Canvas(width, height);
			GraphicsContext graphicsContext = canvas.getGraphicsContext2D();
			graphicsContext.setStroke(Color.BLACK);
			graphicsContext.setFill(Color.BLACK);
			SymbolBuilder.drawSymbol(graphicsContext, new PixelCoordinate(0.5 * width, 0.5 * height), symbolType, 1.5 * SymbolBuilder.DEFAULT_SIZE);
			return canvas;
		}

		private string getText(PointSymbolType symbolType)
		{
			switch (symbolType.innerEnumValue)
			{
			case PointSymbolType.InnerEnum.DOT:
				return i18n.getString("PointSymbolTypeListCell.symbol.dot.label", "Dot");
			case PointSymbolType.InnerEnum.X_CROSS:
				return i18n.getString("PointSymbolTypeListCell.symbol.x_cross.label", "Cross (x)");
			case PointSymbolType.InnerEnum.PLUS_CROSS:
				return i18n.getString("PointSymbolTypeListCell.symbol.plus_cross.label", "Cross (+)");
			case PointSymbolType.InnerEnum.CROSSED_CIRCLE:
				return i18n.getString("PointSymbolTypeListCell.symbol.crossed_circle.label", "Crossed circle");
			case PointSymbolType.InnerEnum.CROSSED_SQUARE:
				return i18n.getString("PointSymbolTypeListCell.symbol.crossed_square.label", "Crossed square");
			case PointSymbolType.InnerEnum.FILLED_CIRCLE:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_circle.label", "Filled circle");
			case PointSymbolType.InnerEnum.FILLED_DIAMAND:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_diamand.label", "Filled diamand");
			case PointSymbolType.InnerEnum.FILLED_HEPTAGON:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_heptagon.label", "Filled heptagon");
			case PointSymbolType.InnerEnum.FILLED_HEXAGON:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_hexagon.label", "Filled hexagon");
			case PointSymbolType.InnerEnum.FILLED_OCTAGON:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_octagon.label", "Filled octagon");
			case PointSymbolType.InnerEnum.FILLED_PENTAGON:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_pentagon.label", "Filled pentagon");
			case PointSymbolType.InnerEnum.FILLED_SQUARE:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_square.label", "Filled square");
			case PointSymbolType.InnerEnum.FILLED_STAR:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_star.label", "Filled star");
			case PointSymbolType.InnerEnum.FILLED_UPRIGHT_TRIANGLE:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_upright_triangle.label", "Filled upright triangle");
			case PointSymbolType.InnerEnum.FILLED_DOWNRIGHT_TRIANGLE:
				return i18n.getString("PointSymbolTypeListCell.symbol.filled_downright_triangle.label", "Filled downright triangle");
			case PointSymbolType.InnerEnum.STROKED_CIRCLE:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_circle.label", "Stroked circle");
			case PointSymbolType.InnerEnum.STROKED_DIAMAND:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_diamand.label", "Stroked diamand");
			case PointSymbolType.InnerEnum.STROKED_HEPTAGON:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_heptagon.label", "Stroked heptagon");
			case PointSymbolType.InnerEnum.STROKED_HEXAGON:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_hexagon.label", "Stroked hexagon");
			case PointSymbolType.InnerEnum.STROKED_OCTAGON:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_octagon.label", "Stroked octagon");
			case PointSymbolType.InnerEnum.STROKED_PENTAGON:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_pentagon.label", "Stroked pentagon");
			case PointSymbolType.InnerEnum.STROKED_SQUARE:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_square.label", "Stroked square");
			case PointSymbolType.InnerEnum.STROKED_STAR:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_star.label", "Stroked star");
			case PointSymbolType.InnerEnum.STROKED_UPRIGHT_TRIANGLE:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_upright_triangle.label", "Stroked upright triangle");
			case PointSymbolType.InnerEnum.STROKED_DOWNRIGHT_TRIANGLE:
				return i18n.getString("PointSymbolTypeListCell.symbol.stroked_downright_triangle.label", "Stroked downright triangle");

			}
			return null;
		}
	}
}