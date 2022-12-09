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
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using ColumnTooltipHeader = org.applied_geodesy.ui.table.ColumnTooltipHeader;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using org.applied_geodesy.ui.table;
	using CellValueType = org.applied_geodesy.util.CellValueType;

	using TableColumn = javafx.scene.control.TableColumn;
	using TableView = javafx.scene.control.TableView;

	public class UICongruentPointTableBuilder : UITableBuilder<TerrestrialObservationRow>
	{

		private static UICongruentPointTableBuilder tableBuilder = new UICongruentPointTableBuilder();
		private bool isInitialize = false;

		private UICongruentPointTableBuilder() : base()
		{
		}

		public static UICongruentPointTableBuilder Instance
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

			TableColumn<TerrestrialObservationRow, string> stringColumn = null;
			TableColumn<TerrestrialObservationRow, double> doubleColumn = null;

			TableView<TerrestrialObservationRow> table = this.createTable();

			// Point-A
			int columnIndex = table.getColumns().size();
			string labelText = i18n.getString("UICongruentPointTableBuilder.tableheader.point1.name.label", "Point A");
			string tooltipText = i18n.getString("UICongruentPointTableBuilder.tableheader.point1.name.tooltip", "Id of first point A");
			CellValueType cellValueType = CellValueType.STRING;
			ColumnTooltipHeader header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(header, TerrestrialObservationRow::startPointNameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, false);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// Point B
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruentPointTableBuilder.tableheader.point2.name.label", "Point B");
			tooltipText = i18n.getString("UICongruentPointTableBuilder.tableheader.point2.name.tooltip", "Id of second point B");
			cellValueType = CellValueType.STRING;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(header, TerrestrialObservationRow::endPointNameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, false);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// Distance between A-B
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruentPointTableBuilder.tableheader.distance.label", "Distance");
			tooltipText = i18n.getString("UICongruentPointTableBuilder.tableheader.distance.tooltip", "Estimated distance between point A and B");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, TerrestrialObservationRow::distanceAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, false);
			table.getColumns().add(doubleColumn);

			table.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY);
			this.table = table;
			this.isInitialize = true;
		}

		public override TerrestrialObservationRow EmptyRow
		{
			get
			{
				return new TerrestrialObservationRow();
			}
		}

		internal override void setValue(TerrestrialObservationRow row, int columnIndex, object oldValue, object newValue)
		{
		}

	}

}