using System;
using System.Collections.Generic;
using System.IO;

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

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using GNSSObservationRowDnD = org.applied_geodesy.jag3d.ui.dnd.GNSSObservationRowDnD;
	using ColumnContentType = org.applied_geodesy.jag3d.ui.table.column.ColumnContentType;
	using TableContentType = org.applied_geodesy.jag3d.ui.table.column.TableContentType;
	using GNSSObservationRow = org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow;
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightRangeType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using EditableMenuCheckBoxTreeCell = org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using AbsoluteValueComparator = org.applied_geodesy.ui.table.AbsoluteValueComparator;
	using ColumnTooltipHeader = org.applied_geodesy.ui.table.ColumnTooltipHeader;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using org.applied_geodesy.ui.table;
	using CellValueType = org.applied_geodesy.util.CellValueType;

	using Platform = javafx.application.Platform;
	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using EventHandler = javafx.@event.EventHandler;
	using TableColumn = javafx.scene.control.TableColumn;
	using TableRow = javafx.scene.control.TableRow;
	using TableView = javafx.scene.control.TableView;
	using TreeItem = javafx.scene.control.TreeItem;
	using CellDataFeatures = javafx.scene.control.TableColumn.CellDataFeatures;
	using ClipboardContent = javafx.scene.input.ClipboardContent;
	using Dragboard = javafx.scene.input.Dragboard;
	using MouseEvent = javafx.scene.input.MouseEvent;
	using TransferMode = javafx.scene.input.TransferMode;
	using Callback = javafx.util.Callback;

	public class UIGNSSObservationTableBuilder : UIEditableTableBuilder<GNSSObservationRow>
	{
		private ObservationTreeItemValue observationItemValue;
		private ObservationType type;
		private IDictionary<ObservationType, TableView<GNSSObservationRow>> tables = new Dictionary<ObservationType, TableView<GNSSObservationRow>>();

		private static UIGNSSObservationTableBuilder tableBuilder = new UIGNSSObservationTableBuilder();
		private UIGNSSObservationTableBuilder() : base()
		{
		}

		public static UIGNSSObservationTableBuilder Instance
		{
			get
			{
				return tableBuilder;
			}
		}

		public virtual TableView<GNSSObservationRow> getTable(ObservationTreeItemValue observationItemValue)
		{
			this.observationItemValue = observationItemValue;
			this.type = TreeItemType.getObservationTypeByTreeItemType(observationItemValue.ItemType);
			switch (this.type.innerEnumValue)
			{
			case ObservationType.InnerEnum.GNSS1D:
			case ObservationType.InnerEnum.GNSS2D:
			case ObservationType.InnerEnum.GNSS3D:
				this.init();
				return this.table;
			default:
				throw new System.ArgumentException(this.GetType().Name + " : Error, unsuported observation type " + type);
			}
		}

		private TableContentType TableContentType
		{
			get
			{
				switch (this.type.innerEnumValue)
				{
				case ObservationType.InnerEnum.GNSS1D:
					return TableContentType.GNSS_1D;
				case ObservationType.InnerEnum.GNSS2D:
					return TableContentType.GNSS_2D;
				default:
					return TableContentType.GNSS_3D;
				}
			}
		}

		private void init()
		{
			if (this.tables.ContainsKey(this.type))
			{
				this.table = this.tables[this.type];
				return;
			}
			TableColumn<GNSSObservationRow, bool> booleanColumn = null;
			TableColumn<GNSSObservationRow, string> stringColumn = null;
			TableColumn<GNSSObservationRow, double> doubleColumn = null;

			TableContentType tableContentType = this.TableContentType;

			TableView<GNSSObservationRow> table = this.createTable();
			///////////////// A-PRIORI VALUES /////////////////////////////

			// Enable/Disable
			int columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexEnable = columnIndex;
			int columnIndexEnable = columnIndex;
			string labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.enable.label", "Enable");
			string tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.enable.tooltip", "State of the baseline");
			CellValueType cellValueType = CellValueType.BOOLEAN;
			ColumnContentType columnContentType = ColumnContentType.ENABLE;
			ColumnTooltipHeader header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, GNSSObservationRow::enableProperty, BooleanCallback, ColumnType.VISIBLE, columnIndex, true, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass(this, columnIndexEnable));
			table.getColumns().add(booleanColumn);

			// Station-ID
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.station.name.label", "Station-Id");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.station.name.tooltip", "Id of station");
			cellValueType = CellValueType.STRING;
			columnContentType = ColumnContentType.START_POINT_NAME;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, GNSSObservationRow::startPointNameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// Target-ID
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.target.name.label", "Target-Id");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.target.name.tooltip", "Id of target point");
			cellValueType = CellValueType.STRING;
			columnContentType = ColumnContentType.END_POINT_NAME;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, GNSSObservationRow::endPointNameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);


			// A-priori Components
			// Y0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.y0.label", "y0");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.y0.tooltip", "A-priori y-component of the baseline");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Y_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::yAprioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APRIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// X0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.x0.label", "x0");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.x0.tooltip", "A-priori x-component of the baseline");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_X_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::xAprioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APRIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// Z0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.z0.label", "z0");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.z0.tooltip", "A-priori z-component of the baseline");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Z_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::zAprioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ? ColumnType.APRIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// A-priori Uncertainties
			// Y0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.y0.label", "\u03C3y0");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.y0.tooltip", "A-priori uncertainty of y-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Y_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::sigmaYaprioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APRIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, true, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// X0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.x0.label", "\u03C3x0");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.x0.tooltip", "A-priori uncertainty of x-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_X_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::sigmaXaprioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APRIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, true, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Z0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.z0.label", "\u03C3z0");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.z0.tooltip", "A-priori uncertainty of z-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Z_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::sigmaZaprioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ? ColumnType.APRIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, true, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			///////////////// A-POSTERIORI VALUES /////////////////////////////
			// A-posteriori Components

			// Y-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.y.label", "y");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.y.tooltip", "A-posteriori y-component of the baseline");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Y_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::yAposterioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// X-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.x.label", "x");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.x.tooltip", "A-posteriori x-component of the baseline");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_X_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::xAposterioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Z-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.z.label", "z");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.z.tooltip", "A-posteriori z-component of the baseline");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Z_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::zAposterioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);


			// A-posteriori Uncertainties
			// Y-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.y.label", "\u03C3y");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.y.tooltip", "A-posteriori uncertainty of y-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Y_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::sigmaYaposterioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// X-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.x.label", "\u03C3x");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.x.tooltip", "A-posteriori uncertainty of x-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_X_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::sigmaXaposterioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Z-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.z.label", "\u03C3z");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.sigma.z.tooltip", "A-posteriori uncertainty of z-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Z_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::sigmaZaposterioriProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Redundancy
			// ry
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.redundancy.y.label", "ry");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.redundancy.y.tooltip", "Redundancy of y-component");
			cellValueType = CellValueType.PERCENTAGE;
			columnContentType = ColumnContentType.REDUNDANCY_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::redundancyYProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// rx
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.redundancy.x.label", "rx");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.redundancy.x.tooltip", "Redundancy of x-component");
			cellValueType = CellValueType.PERCENTAGE;
			columnContentType = ColumnContentType.REDUNDANCY_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::redundancyXProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// rz
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.redundancy.z.label", "rz");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.redundancy.z.tooltip", "Redundancy of z-component");
			cellValueType = CellValueType.PERCENTAGE;
			columnContentType = ColumnContentType.REDUNDANCY_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::redundancyZProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ?ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Residual
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.residual.y.label", "\u03B5y");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.residual.y.tooltip", "Residual of y-component");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::residualYProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.residual.x.label", "\u03B5x");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.residual.x.tooltip", "Residual of x-component");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::residualXProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.residual.z.label", "\u03B5z");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.residual.z.tooltip", "Residual of z-component");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::residualZProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Gross-Error
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.grosserror.y.label", "\u2207y");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.grosserror.y.tooltip", "Gross-error in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.GROSS_ERROR_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::grossErrorYProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.grosserror.x.label", "\u2207x");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.grosserror.x.tooltip", "Gross-error in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.GROSS_ERROR_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::grossErrorXProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.grosserror.z.label", "\u2207z");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.grosserror.z.tooltip", "Gross-error in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.GROSS_ERROR_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::grossErrorZProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// MTB
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.maximumtolerablebias.y.label", "\u2207y(1)");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.maximumtolerablebias.y.tooltip", "Maximum tolerable bias in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::maximumTolerableBiasYProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.maximumtolerablebias.x.label", "\u2207x(1)");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.maximumtolerablebias.x.tooltip", "Maximum tolerable bias in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::maximumTolerableBiasXProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.maximumtolerablebias.z.label", "\u2207z(1)");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.maximumtolerablebias.z.tooltip", "Maximum tolerable bias in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::maximumTolerableBiasZProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// MDB
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.minimaldetectablebias.y.label", "\u2207y(\u03BB)");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.minimaldetectablebias.y.tooltip", "Minimal detectable bias in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::minimalDetectableBiasYProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.minimaldetectablebias.x.label", "\u2207x(\u03BB)");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.minimaldetectablebias.x.tooltip", "Minimal detectable bias in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::minimalDetectableBiasXProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.minimaldetectablebias.z.label", "\u2207z(\u03BB)");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.minimaldetectablebias.z.tooltip", "Minimal detectable bias in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::minimalDetectableBiasZProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Influence on point position (EP)
			// EP-y
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.influenceonposition.y.label", "EPy");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.influenceonposition.y.tooltip", "Influence on point position due to an undetected gross-error in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_POINT_POSITION_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::influenceOnPointPositionYProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// EP-x
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.influenceonposition.x.label", "EPx");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.influenceonposition.x.tooltip", "Influence on point position due to an undetected gross-error in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_POINT_POSITION_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::influenceOnPointPositionXProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS1D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// EP-z
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.influenceonposition.z.label", "EPz");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.influenceonposition.z.tooltip", "Influence on point position due to an undetected gross-error in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_POINT_POSITION_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::influenceOnPointPositionZProperty, getDoubleCallback(cellValueType), this.type != ObservationType.GNSS2D ? ColumnType.APOSTERIORI_GNSS_OBSERVATION : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// EFSP
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.influenceonnetworkdistortion.label", "EF\u00B7SPmax");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.influenceonnetworkdistortion.tooltip", "Maximal influence on network distortion");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_NETWORK_DISTORTION;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::influenceOnNetworkDistortionProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_GNSS_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// vTPv
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.omega.label", "\u03A9");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.omega.tooltip", "Weighted squares of residual");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.OMEGA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::omegaProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_GNSS_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.pvalue.apriori.label", "log(Pprio)");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.pvalue.apriori.tooltip", "A-priori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::pValueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_GNSS_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.pvalue.aposteriori.label", "log(Ppost)");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.pvalue.aposteriori.tooltip", "A-posteriori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::pValueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_GNSS_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Tprio
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.teststatistic.apriori.label", "Tprio");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.teststatistic.apriori.tooltip", "A-priori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::testStatisticAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_GNSS_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Tpost
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.teststatistic.aposteriori.label", "Tpost");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.teststatistic.aposteriori.tooltip", "A-posteriori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, GNSSObservationRow::testStatisticAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_GNSS_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Decision of test statistic
			columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexOutlier = columnIndex;
			int columnIndexOutlier = columnIndex;
			labelText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.testdecision.label", "Significant");
			tooltipText = i18n.getString("UIGNSSObservationTableBuilder.tableheader.testdecision.tooltip", "Checked, if null-hypothesis is rejected");
			cellValueType = CellValueType.BOOLEAN;
			columnContentType = ColumnContentType.SIGNIFICANT;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, GNSSObservationRow::significantProperty, BooleanCallback, ColumnType.APOSTERIORI_GNSS_OBSERVATION, columnIndex, false, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass2(this, columnIndexOutlier));
			table.getColumns().add(booleanColumn);

			this.addContextMenu(table, this.createContextMenu(false));
			this.addDynamicRowAdder(table);
			this.addColumnOrderSequenceListeners(tableContentType, table);

			this.tables[this.type] = table;
			this.table = table;
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn.CellDataFeatures<GNSSObservationRow, bool>, ObservableValue<bool>>
		{
			private readonly UIGNSSObservationTableBuilder outerInstance;

			private int columnIndexEnable;

			public CallbackAnonymousInnerClass(UIGNSSObservationTableBuilder outerInstance, int columnIndexEnable)
			{
				this.outerInstance = outerInstance;
				this.columnIndexEnable = columnIndexEnable;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<GNSSObservationRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isEnable());
				booleanProp.addListener(enableChangeListener);
				return booleanProp;
			}
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn.CellDataFeatures<GNSSObservationRow, bool>, ObservableValue<bool>>
		{
			private readonly UIGNSSObservationTableBuilder outerInstance;

			private int columnIndexOutlier;

			public CallbackAnonymousInnerClass2(UIGNSSObservationTableBuilder outerInstance, int columnIndexOutlier)
			{
				this.outerInstance = outerInstance;
				this.columnIndexOutlier = columnIndexOutlier;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<GNSSObservationRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isSignificant());
				booleanProp.addListener(significantChangeListener);
				return booleanProp;
			}
		}

		internal override void setValue(GNSSObservationRow rowData, int columnIndex, object oldValue, object newValue)
		{
			bool valid = (oldValue == null || oldValue.ToString().Trim().Length == 0) && (newValue == null || newValue.ToString().Trim().Length == 0);
			switch (columnIndex)
			{

			case 0:
				rowData.Enable = newValue != null && newValue is bool? && (bool?)newValue;
				valid = true;
				break;
			case 1:
				if (newValue != null && newValue.ToString().Trim().Length > 0 && ((string.ReferenceEquals(rowData.EndPointName, null) || !rowData.EndPointName.Equals(newValue.ToString().Trim()))))
				{
					rowData.StartPointName = newValue.ToString().Trim();
					valid = true;
				}
				else
				{
					rowData.StartPointName = oldValue == null ? null : oldValue.ToString().Trim();
				}
				break;
			case 2:
				if (newValue != null && newValue.ToString().Trim().Length > 0 && ((string.ReferenceEquals(rowData.StartPointName, null) || !rowData.StartPointName.Equals(newValue.ToString().Trim()))))
				{
					rowData.EndPointName = newValue.ToString().Trim();
					valid = true;
				}
				else
				{
					rowData.EndPointName = oldValue == null ? null : oldValue.ToString().Trim();
				}
				break;
			case 3:
				if (newValue != null && newValue is Double)
				{
					rowData.YApriori = (double?)newValue;
					valid = true;
				}
				else
				{
					rowData.YApriori = oldValue != null && oldValue is double? ? (double?)oldValue : null;
				}
				break;
			case 4:
				if (newValue != null && newValue is Double)
				{
					rowData.XApriori = (double?)newValue;
					valid = true;
				}
				else
				{
					rowData.XApriori = oldValue != null && oldValue is double? ? (double?)oldValue : null;
				}
				break;
			case 5:
				if (newValue != null && newValue is Double)
				{
					rowData.ZApriori = (double?)newValue;
					valid = true;
				}
				else
				{
					rowData.ZApriori = oldValue != null && oldValue is double? ? (double?)oldValue : null;
				}
				break;
			case 6:
				rowData.SigmaYapriori = newValue == null ? null : newValue is double? && (double?)newValue > 0 ? (double?)newValue : null;
				valid = true;
				break;
			case 7:
				rowData.SigmaXapriori = newValue == null ? null : newValue is double? && (double?)newValue > 0 ? (double?)newValue : null;
				valid = true;
				break;
			case 8:
				rowData.SigmaZapriori = newValue == null ? null : newValue is double? && (double?)newValue > 0 ? (double?)newValue : null;
				valid = true;
				break;
			default:
				Console.Error.WriteLine(this.GetType().Name + " : Editable column exceed " + columnIndex);
				valid = false;
				break;
			}

			if (valid && this.isComplete(rowData))
			{
				// Set observation group id, if not exists
				if (rowData.GroupId < 0)
				{
					rowData.GroupId = this.observationItemValue.GroupId;
				}

				try
				{
					SQLManager.Instance.saveItem(rowData);
				}
				catch (Exception e)
				{
					switch (columnIndex)
					{
					case 1:
						rowData.StartPointName = oldValue == null ? null : oldValue.ToString().Trim();
						break;
					case 2:
						rowData.EndPointName = oldValue == null ? null : oldValue.ToString().Trim();
						break;
					default:
						break;
					}
					valid = false;
					raiseErrorMessageSaveValue(e);
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
	//		this.table.refresh();
	//		this.table.requestFocus();
	//		this.table.getSelectionModel().clearSelection();
	//		this.table.getSelectionModel().select(rowData);

			if (!valid)
			{
				Platform.runLater(() =>
				{
				table.refresh();
				table.requestFocus();
				table.getSelectionModel().clearSelection();
				table.getSelectionModel().select(rowData);
				});
			}
		}

		public override GNSSObservationRow EmptyRow
		{
			get
			{
				return new GNSSObservationRow();
			}
		}

		private bool isComplete(GNSSObservationRow row)
		{
			return !string.ReferenceEquals(row.StartPointName, null) && row.StartPointName.Trim().Length > 0 && !string.ReferenceEquals(row.EndPointName, null) && row.EndPointName.Trim().Length > 0 && !row.StartPointName.Equals(row.EndPointName) && row.XApriori != null && row.YApriori != null && row.ZApriori != null;
		}

		internal override void enableDragSupport()
		{
			this.table.setOnDragDetected(new EventHandlerAnonymousInnerClass(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<MouseEvent>
		{
			private readonly UIGNSSObservationTableBuilder outerInstance;

			public EventHandlerAnonymousInnerClass(UIGNSSObservationTableBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void handle(MouseEvent @event)
			{
				IList<GNSSObservationRow> selectedRows = new List<GNSSObservationRow>(outerInstance.table.getSelectionModel().getSelectedItems());
				if (selectedRows != null && selectedRows.Count > 0)
				{

					IList<GNSSObservationRowDnD> rowsDnD = new List<GNSSObservationRowDnD>(selectedRows.Count);
					foreach (GNSSObservationRow selectedRow in selectedRows)
					{
						GNSSObservationRowDnD rowDnD = null;
						if (outerInstance.isComplete(selectedRow) && (rowDnD = GNSSObservationRowDnD.fromGNSSObservationRow(selectedRow)) != null)
						{
							rowsDnD.Add(rowDnD);
						}
					}

					if (rowsDnD.Count > 0)
					{
						Dragboard db = outerInstance.table.startDragAndDrop(TransferMode.MOVE);
						ClipboardContent content = new ClipboardContent();
						content.put(EditableMenuCheckBoxTreeCell.TREE_ITEM_TYPE_DATA_FORMAT, outerInstance.observationItemValue.ItemType);
						content.put(EditableMenuCheckBoxTreeCell.GROUP_ID_DATA_FORMAT, outerInstance.observationItemValue.GroupId);
						content.put(EditableMenuCheckBoxTreeCell.DIMENSION_DATA_FORMAT, outerInstance.observationItemValue.Dimension);
						content.put(EditableMenuCheckBoxTreeCell.GNSS_OBSERVATION_ROWS_DATA_FORMAT, rowsDnD);

						db.setContent(content);
					}
				}
				@event.consume();
			}
		}

		internal override void duplicateRows()
		{
			IList<GNSSObservationRow> selectedRows = new List<GNSSObservationRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<GNSSObservationRow> clonedRows = new List<GNSSObservationRow>(selectedRows.Count);
			foreach (GNSSObservationRow row in selectedRows)
			{
				GNSSObservationRow clonedRow = GNSSObservationRow.cloneRowApriori(row);
				if (this.isComplete(clonedRow))
				{
					try
					{
						SQLManager.Instance.saveItem(clonedRow);
					}
					catch (Exception e)
					{
						raiseErrorMessage(ContextMenuType.DUPLICATE, e);
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
						break;
					}
				}
				clonedRows.Add(clonedRow);
			}

			if (clonedRows != null && clonedRows.Count > 0)
			{
				this.table.getItems().addAll(clonedRows);
				this.table.getSelectionModel().clearSelection();
				foreach (GNSSObservationRow clonedRow in clonedRows)
				{
					this.table.getSelectionModel().select(clonedRow);
				}
				this.table.scrollTo(clonedRows[0]);
			}
		}

		internal override void removeRows()
		{
			IList<GNSSObservationRow> selectedRows = new List<GNSSObservationRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<GNSSObservationRow> removedRows = new List<GNSSObservationRow>(selectedRows.Count);
			foreach (GNSSObservationRow row in selectedRows)
			{
				if (this.isComplete(row))
				{
					try
					{
						SQLManager.Instance.remove(row);
					}
					catch (Exception e)
					{
						raiseErrorMessage(ContextMenuType.REMOVE, e);
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
						break;
					}
				}
				removedRows.Add(row);
			}
			if (removedRows != null && removedRows.Count > 0)
			{
				this.table.getSelectionModel().clearSelection();
				this.table.getItems().removeAll(removedRows);
				if (this.table.getItems().isEmpty())
				{
					this.table.getItems().setAll(EmptyRow);
				}
			}
		}

		internal override void moveRows(ContextMenuType type)
		{
			if (type != ContextMenuType.MOVETO)
			{
				return;
			}

			IList<GNSSObservationRow> selectedRows = new List<GNSSObservationRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(this.observationItemValue.ItemType);
			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, false);
			try
			{
				SQLManager.Instance.saveGroup((ObservationTreeItemValue)newTreeItem.getValue());
			}
			catch (Exception e)
			{
				raiseErrorMessage(type, e);
				UITreeBuilder.Instance.removeItem(newTreeItem);
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				return;
			}

			try
			{
				int groupId = ((ObservationTreeItemValue)newTreeItem.getValue()).GroupId;
				foreach (GNSSObservationRow row in selectedRows)
				{
					row.GroupId = groupId;
					SQLManager.Instance.saveItem(row);
				}

			}
			catch (Exception e)
			{
				raiseErrorMessage(type, e);
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				return;
			}

			UITreeBuilder.Instance.Tree.getSelectionModel().select(newTreeItem);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void export(java.io.File file, boolean aprioriValues) throws java.io.IOException
		public virtual void export(File file, bool aprioriValues)
		{
			IList<GNSSObservationRow> rows = this.table.getItems();

			string exportFormatString = "%15s \t";
			//String exportFormatDouble = "%+15.6f \t";
			string exportFormatDouble = "%20s \t";

			PrintWriter writer = null;

			try
			{
				writer = new PrintWriter(new StreamWriter(file));

				foreach (GNSSObservationRow row in rows)
				{
					if (!row.Enable)
					{
						continue;
					}

					string startPointName = row.StartPointName;
					string endPointName = row.EndPointName;

					if (string.ReferenceEquals(startPointName, null) || startPointName.Trim().Length == 0 || string.ReferenceEquals(endPointName, null) || endPointName.Trim().Length == 0)
					{
						continue;
					}

					double? y = aprioriValues ? row.YApriori : row.YAposteriori;
					double? x = aprioriValues ? row.XApriori : row.XAposteriori;
					double? z = aprioriValues ? row.ZApriori : row.ZAposteriori;

					if (this.type != ObservationType.GNSS2D && z == null)
					{
						continue;
					}

					if (this.type != ObservationType.GNSS1D && (y == null || x == null))
					{
						continue;
					}

					double? sigmaY = aprioriValues ? row.SigmaYapriori : row.SigmaYaposteriori;
					double? sigmaX = aprioriValues ? row.SigmaXapriori : row.SigmaXaposteriori;
					double? sigmaZ = aprioriValues ? row.SigmaZapriori : row.SigmaZaposteriori;

					if (!aprioriValues && (sigmaY == null || sigmaX == null || sigmaZ == null))
					{
						continue;
					}

					if (aprioriValues)
					{
						if (sigmaY != null && sigmaY <= 0)
						{
							sigmaY = null;
						}
						if (sigmaX != null && sigmaX <= 0)
						{
							sigmaX = null;
						}
						if (sigmaZ != null && sigmaZ <= 0)
						{
							sigmaZ = null;
						}
					}

					string yValue = this.type != ObservationType.GNSS1D && y != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(y.Value, false)) : "";
					string xValue = this.type != ObservationType.GNSS1D && x != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(x.Value, false)) : "";
					string zValue = this.type != ObservationType.GNSS2D && z != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(z.Value, false)) : "";

					string sigmaYvalue = this.type != ObservationType.GNSS1D && sigmaY != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigmaY.Value, false)) : "";
					string sigmaXvalue = this.type != ObservationType.GNSS1D && sigmaX != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigmaX.Value, false)) : "";
					string sigmaZvalue = this.type != ObservationType.GNSS2D && sigmaZ != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigmaZ.Value, false)) : "";

					writer.println(String.format(exportFormatString, startPointName) + String.format(exportFormatString, endPointName) + yValue + xValue + zValue + sigmaYvalue + sigmaXvalue + sigmaZvalue);
				}
			}
			finally
			{
				if (writer != null)
				{
					writer.close();
				}
			}
		}

		internal override void highlightTableRow(TableRow<GNSSObservationRow> row)
		{
			if (row == null)
			{
				return;
			}

			TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
			TableRowHighlightType tableRowHighlightType = tableRowHighlight.SelectedTableRowHighlightType;
			double leftBoundary = tableRowHighlight.getLeftBoundary(tableRowHighlightType);
			double rightBoundary = tableRowHighlight.getRightBoundary(tableRowHighlightType);

			GNSSObservationRow item = row.getItem();

			if (!row.isSelected() && item != null)
			{
				switch (tableRowHighlightType.innerEnumValue)
				{
				case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
					this.setTableRowHighlight(row, item.Significant ? TableRowHighlightRangeType.INADEQUATE : TableRowHighlightRangeType.EXCELLENT);
					break;

				case TableRowHighlightType.InnerEnum.GROSS_ERROR:
					double? grossErrorX = item.GrossErrorX;
					double? grossErrorY = item.GrossErrorY;
					double? grossErrorZ = item.GrossErrorZ;

					double? mtbX = item.MaximumTolerableBiasX;
					double? mtbY = item.MaximumTolerableBiasY;
					double? mtbZ = item.MaximumTolerableBiasZ;

					double? mdbX = item.MinimalDetectableBiasX;
					double? mdbY = item.MinimalDetectableBiasY;
					double? mdbZ = item.MinimalDetectableBiasZ;

					double dMTB = Double.NaN;
					double dMDB = Double.NaN;

					if (this.type == ObservationType.GNSS1D && grossErrorZ != null && mtbZ != null && mdbZ != null)
					{
						dMTB = Math.Abs(grossErrorZ) - Math.Abs(mtbZ);
						dMDB = Math.Abs(mdbZ) - Math.Abs(grossErrorZ);
					}

					else if (this.type == ObservationType.GNSS2D && grossErrorX != null && grossErrorY != null && mtbX != null && mtbY != null && mdbX != null && mdbY != null)
					{
						dMTB = Math.Max(Math.Abs(grossErrorX) - Math.Abs(mtbX), Math.Abs(grossErrorY) - Math.Abs(mtbY));
						dMDB = Math.Min(Math.Abs(mdbX) - Math.Abs(grossErrorX), Math.Abs(mdbY) - Math.Abs(grossErrorY));
					}

					else if (this.type == ObservationType.GNSS3D && grossErrorX != null && grossErrorY != null && grossErrorZ != null && mtbZ != null && mtbX != null && mtbY != null && mdbZ != null && mdbX != null && mdbY != null)
					{
						dMTB = Math.Max(Math.Max(Math.Abs(grossErrorX) - Math.Abs(mtbX), Math.Abs(grossErrorY) - Math.Abs(mtbY)), Math.Abs(grossErrorZ) - Math.Abs(mtbZ));
						dMDB = Math.Min(Math.Min(Math.Abs(mdbX) - Math.Abs(grossErrorX), Math.Abs(mdbY) - Math.Abs(grossErrorY)), Math.Abs(mdbZ) - Math.Abs(grossErrorZ));
					}

					if (!double.IsNaN(dMTB) && !double.IsNaN(dMDB))
					{
						if (dMTB <= 0)
						{
							this.setTableRowHighlight(row, TableRowHighlightRangeType.EXCELLENT);
						}

						else if (dMTB > 0 && dMDB >= 0)
						{
							this.setTableRowHighlight(row, TableRowHighlightRangeType.SATISFACTORY);
						}

						else if (dMDB < 0)
						{
							this.setTableRowHighlight(row, TableRowHighlightRangeType.INADEQUATE);
						}

						else
						{
							this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
						}
					}
					else
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
					}

					break;

				case TableRowHighlightType.InnerEnum.REDUNDANCY:
					double? redundancyX = item.RedundancyX;
					double? redundancyY = item.RedundancyY;
					double? redundancyZ = item.RedundancyZ;
					double? redundancy = null;

					if (this.type == ObservationType.GNSS1D && redundancyZ != null)
					{
						redundancy = redundancyZ;
					}

					else if (this.type == ObservationType.GNSS2D && redundancyY != null && redundancyX != null)
					{
						redundancy = Math.Min(redundancyY, redundancyX);
					}

					else if (this.type == ObservationType.GNSS3D && redundancyY != null && redundancyX != null && redundancyZ != null)
					{
						redundancy = Math.Min(redundancyZ, Math.Min(redundancyY, redundancyX));
					}

					if (redundancy == null)
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
					}
					else
					{
						this.setTableRowHighlight(row, redundancy.Value < leftBoundary ? TableRowHighlightRangeType.INADEQUATE : redundancy.Value <= rightBoundary ? TableRowHighlightRangeType.SATISFACTORY : TableRowHighlightRangeType.EXCELLENT);
					}

					break;

				case TableRowHighlightType.InnerEnum.INFLUENCE_ON_POSITION:
					double? influenceOnPositionX = item.InfluenceOnPointPositionX;
					double? influenceOnPositionY = item.InfluenceOnPointPositionY;
					double? influenceOnPositionZ = item.InfluenceOnPointPositionZ;
					double? influenceOnPosition = null;

					if (this.type == ObservationType.GNSS1D && influenceOnPositionZ != null)
					{
						influenceOnPosition = influenceOnPositionZ;
					}

					else if (this.type == ObservationType.GNSS2D && influenceOnPositionY != null && influenceOnPositionX != null)
					{
						influenceOnPosition = Math.Max(influenceOnPositionY, influenceOnPositionX);
					}

					else if (this.type == ObservationType.GNSS3D && influenceOnPositionY != null && influenceOnPositionX != null && influenceOnPositionZ != null)
					{
						influenceOnPosition = Math.Max(influenceOnPositionZ, Math.Max(influenceOnPositionY, influenceOnPositionX));
					}

					if (influenceOnPosition == null)
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
					}
					else
					{
						this.setTableRowHighlight(row, Math.Abs(influenceOnPosition) < leftBoundary ? TableRowHighlightRangeType.EXCELLENT : Math.Abs(influenceOnPosition) <= rightBoundary ? TableRowHighlightRangeType.SATISFACTORY : TableRowHighlightRangeType.INADEQUATE);
					}

					break;

				case TableRowHighlightType.InnerEnum.P_PRIO_VALUE:
					double? pValue = item.PValueApriori;
					if (pValue == null)
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
					}
					else
					{
						this.setTableRowHighlight(row, pValue < Math.Log(leftBoundary / 100.0) ? TableRowHighlightRangeType.INADEQUATE : pValue <= Math.Log(rightBoundary / 100.0) ? TableRowHighlightRangeType.SATISFACTORY : TableRowHighlightRangeType.EXCELLENT);
					}

					break;

				case TableRowHighlightType.InnerEnum.NONE:
					this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);

					break;
				}

			}
			else
			{
				this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
			}
		}
	}
}