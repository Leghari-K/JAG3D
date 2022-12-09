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

namespace org.applied_geodesy.jag3d.ui.table
{
	using PrincipalComponentRow = org.applied_geodesy.jag3d.ui.table.row.PrincipalComponentRow;
	using ColumnTooltipHeader = org.applied_geodesy.ui.table.ColumnTooltipHeader;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using CellValueType = org.applied_geodesy.util.CellValueType;

	using TableColumn = javafx.scene.control.TableColumn;
	using TableView = javafx.scene.control.TableView;

	public class UIPrincipalComponentTableBuilder : UITableBuilder<PrincipalComponentRow>
	{
		private static UIPrincipalComponentTableBuilder tableBuilder = new UIPrincipalComponentTableBuilder();
		private bool isInitialize = false;
		private UIPrincipalComponentTableBuilder() : base()
		{
		}

		public static UIPrincipalComponentTableBuilder Instance
		{
			get
			{
				tableBuilder.init();
				return tableBuilder;
			}
		}

		private void init()
		{
			if (this.isInitialize)
			{
				return;
			}

			TableView<PrincipalComponentRow> table = this.createTable();

			TableColumn<PrincipalComponentRow, int> integerColumn = null;
			TableColumn<PrincipalComponentRow, double> doubleColumn = null;

			// Index of eigenvalue
			CellValueType cellValueType = CellValueType.INTEGER;
			int columnIndex = table.getColumns().size();
			string labelText = i18n.getString("UIPrincipalComponentTableBuilder.tableheader.index.label", "k");
			string tooltipText = i18n.getString("UIPrincipalComponentTableBuilder.tableheader.index.tooltip", "k-te index of eigenvalue");
			ColumnTooltipHeader header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			integerColumn = this.getColumn<int>(header, PrincipalComponentRow::indexProperty, IntegerCallback, ColumnType.VISIBLE, columnIndex, false);
			table.getColumns().add(integerColumn);

			// Eigenvalue
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPrincipalComponentTableBuilder.tableheader.square_root_eigenvalue.label", "\u221A\u03BB(k)");
			tooltipText = i18n.getString("UIPrincipalComponentTableBuilder.tableheader.square_root_eigenvalue.tooltip", "Square-root eigenvalue of covariance matrix");
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, PrincipalComponentRow::valueProperty, getDoubleCallback(cellValueType), ColumnType.VISIBLE, columnIndex, false);
			table.getColumns().add(doubleColumn);

			// ratio eigenvalue vs. trace(Cxx)
			cellValueType = CellValueType.PERCENTAGE;
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPrincipalComponentTableBuilder.tableheader.ratio.label", "\u03BB(k)/trace(Cxx)");
			tooltipText = i18n.getString("UIPrincipalComponentTableBuilder.tableheader.ratio.tooltip", "Ratio of eigenvalue \u03BB(k) w.r.t. trace of variance-covariance-matrix");
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, PrincipalComponentRow::ratioProperty, getDoubleCallback(cellValueType), ColumnType.VISIBLE, columnIndex, false);
			table.getColumns().add(doubleColumn);

			table.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY);
			this.table = table;
			this.isInitialize = true;
		}

		internal override void setValue(PrincipalComponentRow row, int columnIndex, object oldValue, object newValue)
		{
		}
		public override PrincipalComponentRow EmptyRow
		{
			get
			{
				return new PrincipalComponentRow();
			}
		}
	}

}