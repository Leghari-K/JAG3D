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

	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using VerticalDeflectionRowDnD = org.applied_geodesy.jag3d.ui.dnd.VerticalDeflectionRowDnD;
	using ColumnContentType = org.applied_geodesy.jag3d.ui.table.column.ColumnContentType;
	using TableContentType = org.applied_geodesy.jag3d.ui.table.column.TableContentType;
	using VerticalDeflectionRow = org.applied_geodesy.jag3d.ui.table.row.VerticalDeflectionRow;
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightRangeType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using EditableMenuCheckBoxTreeCell = org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using VerticalDeflectionTreeItemValue = org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue;
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

	public class UIVerticalDeflectionTableBuilder : UIEditableTableBuilder<VerticalDeflectionRow>
	{
		private VerticalDeflectionTreeItemValue verticalDeflectionItemValue;
		private VerticalDeflectionType type;
		private IDictionary<VerticalDeflectionType, TableView<VerticalDeflectionRow>> tables = new Dictionary<VerticalDeflectionType, TableView<VerticalDeflectionRow>>();


		private static UIVerticalDeflectionTableBuilder tableBuilder = new UIVerticalDeflectionTableBuilder();
		private UIVerticalDeflectionTableBuilder() : base()
		{
		}

		public static UIVerticalDeflectionTableBuilder Instance
		{
			get
			{
				return tableBuilder;
			}
		}

		public virtual TableView<VerticalDeflectionRow> getTable(VerticalDeflectionTreeItemValue verticalDeflectionItemValue)
		{
			this.verticalDeflectionItemValue = verticalDeflectionItemValue;
			this.type = TreeItemType.getVerticalDeflectionTypeByTreeItemType(verticalDeflectionItemValue.ItemType);
			switch (this.type.innerEnumValue)
			{
			case VerticalDeflectionType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION:
			case VerticalDeflectionType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION:
			case VerticalDeflectionType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION:
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
				case VerticalDeflectionType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION:
					return TableContentType.REFERENCE_DEFLECTION;
				case VerticalDeflectionType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION:
					return TableContentType.STOCHASTIC_DEFLECTION;
				default:
					return TableContentType.UNKNOWN_DEFLECTION;
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

			TableColumn<VerticalDeflectionRow, bool> booleanColumn = null;
			TableColumn<VerticalDeflectionRow, string> stringColumn = null;
			TableColumn<VerticalDeflectionRow, double> doubleColumn = null;

			TableContentType tableContentType = this.TableContentType;

			TableView<VerticalDeflectionRow> table = this.createTable();

			///////////////// A-PRIORI VALUES /////////////////////////////
			// Enable/Disable
			int columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexEnable = columnIndex;
			int columnIndexEnable = columnIndex;
			string labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.enable.label", "Enable");
			string tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.enable.tooltip", "State of the vertical deflection");
			CellValueType cellValueType = CellValueType.BOOLEAN;
			ColumnContentType columnContentType = ColumnContentType.ENABLE;
			ColumnTooltipHeader header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, VerticalDeflectionRow::enableProperty, BooleanCallback, ColumnType.VISIBLE, columnIndex, true, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass(this, columnIndexEnable));
			table.getColumns().add(booleanColumn);

			// Point-ID
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.station.name.label", "Point-Id");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.station.name.tooltip", "Id of the point");
			cellValueType = CellValueType.STRING;
			columnContentType = ColumnContentType.POINT_NAME;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, VerticalDeflectionRow::nameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// A-priori Deflection params
			// Y0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.y0.label", "\u03B6y0");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.y0.tooltip", "A-priori y-component of point deflection");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.VALUE_Y_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::yAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_DEFLECTION, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// X0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.x0.label", "\u03B6x0");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.x0.tooltip", "A-priori x-component of point deflection");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.VALUE_X_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::xAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_DEFLECTION, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// A-priori Deflection uncertainties
			// Y0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.sigma.y0.label", "\u03C3y0");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.sigma.y0.tooltip", "A-priori uncertainty of y-component");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.UNCERTAINTY_Y_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::sigmaYaprioriProperty, getDoubleCallback(cellValueType), this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION ? ColumnType.APRIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, true, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// X0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.sigma.x0.label", "\u03C3x0");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.sigma.x0.tooltip", "A-priori uncertainty of x-component");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.UNCERTAINTY_X_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::sigmaXaprioriProperty, getDoubleCallback(cellValueType), this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION ? ColumnType.APRIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, true, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);



			////////////////////////A-POSTERIORI DEFLECTION ///////////////////////////////
			// y-comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.y.label", "\u03B6y");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.y.tooltip", "Y-component of deflection of the vertical");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.VALUE_Y_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::yAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_DEFLECTION, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// x-comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.x.label", "\u03B6x");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.x.tooltip", "X-component of deflection of the vertical");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.VALUE_X_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::xAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_DEFLECTION, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Uncertainty
			// y-comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.sigma.y.label", "\u03c3y");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.sigma.y.tooltip", "A-posteriori uncertainty of y-component of deflection of the vertical");
			cellValueType = CellValueType.ANGLE_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Y_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::sigmaYaposterioriProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// x-comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.sigma.x.label", "\u03c3x");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.sigma.x.tooltip", "A-posteriori uncertainty of x-component of deflection of the vertical");
			cellValueType = CellValueType.ANGLE_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_X_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::sigmaXaposterioriProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Confidence
			// A
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.semiaxis.major.label", "a");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.semiaxis.major.tooltip", "Major semi axis");
			cellValueType = CellValueType.ANGLE_UNCERTAINTY;
			columnContentType = ColumnContentType.CONFIDENCE_A;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::confidenceAProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// C
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.semiaxis.minor.label", "c");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.semiaxis.minor.tooltip", "Minor semi axis");
			cellValueType = CellValueType.ANGLE_UNCERTAINTY;
			columnContentType = ColumnContentType.CONFIDENCE_C;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::confidenceCProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Redundancy
			// ry
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.redundancy.y.label", "ry");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.redundancy.y.tooltip", "Redundancy of y-component of deflection of the vertical");
			cellValueType = CellValueType.PERCENTAGE;
			columnContentType = ColumnContentType.REDUNDANCY_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::redundancyYProperty, getDoubleCallback(cellValueType), this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// rx
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.redundancy.x.label", "rx");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.redundancy.x.tooltip", "Redundancy of x-component of deflection of the vertical");
			cellValueType = CellValueType.PERCENTAGE;
			columnContentType = ColumnContentType.REDUNDANCY_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::redundancyXProperty, getDoubleCallback(cellValueType), this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Residual
			// y-epsilon
			columnIndex = table.getColumns().size();
			labelText = this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION ? i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.residual.y.label", "\u03B5y") : i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.deviation.y.label", "\u0394y");
			tooltipText = this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION ? i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.residual.y.tooltip", "Residual of y-component of deflection of the vertical") : i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.deviation.y.tooltip", "Deviation of y-component w.r.t. y0");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::residualYProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// x-epsilon
			columnIndex = table.getColumns().size();
			labelText = this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION ? i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.residual.x.label", "\u03B5y") : i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.deviation.x.label", "\u0394x");
			tooltipText = this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION ? i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.residual.x.tooltip", "Residual of x-component of deflection of the vertical") : i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.deviation.x.tooltip", "Deviation of x-component w.r.t. x0");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::residualXProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Gross-error
			// y-Nabla
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.grosserror.y.label", "\u2207y");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.grosserror.y.tooltip", "Gross-error of deflection of the vertical in y");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.GROSS_ERROR_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::grossErrorYProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// x-Nabla
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.grosserror.x.label", "\u2207x");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.grosserror.x.tooltip", "Gross-error of deflection of the vertical in x");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.GROSS_ERROR_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::grossErrorXProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// MTB
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.maximumtolerablebias.y.label", "\u2207x(1)");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.maximumtolerablebias.y.tooltip", "Maximum tolerable bias of deflection of the vertical in y");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::maximumTolerableBiasYProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.maximumtolerablebias.x.label", "\u2207y(1)");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.maximumtolerablebias.x.tooltip", "Maximum tolerable bias of deflection of the vertical in x");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::maximumTolerableBiasXProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// MDB
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.minimaldetectablebias.y.label", "\u2207x(\u03BB)");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.minimaldetectablebias.y.tooltip", "Minimal detectable bias of deflection of the vertical in y");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::minimalDetectableBiasYProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.minimaldetectablebias.x.label", "\u2207y(\u03BB)");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.minimaldetectablebias.x.tooltip", "Minimal detectable bias of deflection of the vertical in x");
			cellValueType = CellValueType.ANGLE_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::minimalDetectableBiasXProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// vTPv
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.omega.label", "\u03A9");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.omega.tooltip", "Weighted squares of residual");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.OMEGA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::omegaProperty, getDoubleCallback(cellValueType), this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.pvalue.apriori.label", "log(Pprio)");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.pvalue.apriori.tooltip", "A-priori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::pValueAprioriProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.pvalue.aposteriori.label", "log(Ppost)");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.pvalue.aposteriori.tooltip", "A-posteriori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::pValueAposterioriProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Tprio
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.teststatistic.apriori.label", "Tprio");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.teststatistic.apriori.tooltip", "A-priori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::testStatisticAprioriProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Tpost
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.teststatistic.aposteriori.label", "Tpost");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.teststatistic.aposteriori.tooltip", "A-posteriori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VerticalDeflectionRow::testStatisticAposterioriProperty, getDoubleCallback(cellValueType), this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Decision of test statistic
			columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexOutlierDeflection = columnIndex;
			int columnIndexOutlierDeflection = columnIndex;
			labelText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.testdecision.label", "Significant");
			tooltipText = i18n.getString("UIVerticalDeflectionTableBuilder.tableheader.testdecision.tooltip", "Checked, if null-hypothesis is rejected");
			cellValueType = CellValueType.BOOLEAN;
			columnContentType = ColumnContentType.SIGNIFICANT;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, VerticalDeflectionRow::significantProperty, BooleanCallback, this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION ? ColumnType.APOSTERIORI_DEFLECTION : ColumnType.HIDDEN, columnIndex, false, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass2(this, columnIndexOutlierDeflection));
			table.getColumns().add(booleanColumn);

			this.addContextMenu(table, this.createContextMenu(false));
			this.addDynamicRowAdder(table);
			this.addColumnOrderSequenceListeners(tableContentType, table);

			this.tables[this.type] = table;
			this.table = table;
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn.CellDataFeatures<VerticalDeflectionRow, bool>, ObservableValue<bool>>
		{
			private readonly UIVerticalDeflectionTableBuilder outerInstance;

			private int columnIndexEnable;

			public CallbackAnonymousInnerClass(UIVerticalDeflectionTableBuilder outerInstance, int columnIndexEnable)
			{
				this.outerInstance = outerInstance;
				this.columnIndexEnable = columnIndexEnable;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<VerticalDeflectionRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isEnable());
				booleanProp.addListener(enableChangeListener);
				return booleanProp;
			}
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn.CellDataFeatures<VerticalDeflectionRow, bool>, ObservableValue<bool>>
		{
			private readonly UIVerticalDeflectionTableBuilder outerInstance;

			private int columnIndexOutlierDeflection;

			public CallbackAnonymousInnerClass2(UIVerticalDeflectionTableBuilder outerInstance, int columnIndexOutlierDeflection)
			{
				this.outerInstance = outerInstance;
				this.columnIndexOutlierDeflection = columnIndexOutlierDeflection;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<VerticalDeflectionRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlierDeflection, param.getValue());
				TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlierDeflection, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isSignificant());
				booleanProp.addListener(significantChangeListener);
				return booleanProp;
			}
		}

		public override VerticalDeflectionRow EmptyRow
		{
			get
			{
				return new VerticalDeflectionRow();
			}
		}

		internal override void setValue(VerticalDeflectionRow rowData, int columnIndex, object oldValue, object newValue)
		{
			bool valid = (oldValue == null || oldValue.ToString().Trim().Length == 0) && (newValue == null || newValue.ToString().Trim().Length == 0);
			switch (columnIndex)
			{

			case 0:
				rowData.Enable = newValue != null && newValue is bool? && (bool?)newValue;
				valid = true;
				break;
			case 1:
				if (newValue != null && newValue.ToString().Trim().Length > 0)
				{
					rowData.Name = newValue.ToString().Trim();
					valid = true;
				}
				else
				{
					rowData.Name = oldValue == null ? null : oldValue.ToString().Trim();
				}
				break;
			case 2:
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
			case 3:
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
			case 4:
				rowData.SigmaYapriori = newValue == null ? null : newValue is double? && (double?)newValue > 0 ? (double?)newValue : null;
				valid = true;
				break;
			case 5:
				rowData.SigmaXapriori = newValue == null ? null : newValue is double? && (double?)newValue > 0 ? (double?)newValue : null;
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
					rowData.GroupId = this.verticalDeflectionItemValue.GroupId;
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
						rowData.Name = oldValue == null ? null : oldValue.ToString().Trim();
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

		private bool isComplete(VerticalDeflectionRow row)
		{
			return !string.ReferenceEquals(row.Name, null) && !row.Name.Trim().Length == 0 && row.XApriori != null && row.YApriori != null;
		}


		internal override void enableDragSupport()
		{
			this.table.setOnDragDetected(new EventHandlerAnonymousInnerClass(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<MouseEvent>
		{
			private readonly UIVerticalDeflectionTableBuilder outerInstance;

			public EventHandlerAnonymousInnerClass(UIVerticalDeflectionTableBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void handle(MouseEvent @event)
			{
				IList<VerticalDeflectionRow> selectedRows = new List<VerticalDeflectionRow>(outerInstance.table.getSelectionModel().getSelectedItems());
				if (selectedRows != null && selectedRows.Count > 0)
				{

					IList<VerticalDeflectionRowDnD> rowsDnD = new List<VerticalDeflectionRowDnD>(selectedRows.Count);
					foreach (VerticalDeflectionRow selectedRow in selectedRows)
					{
						VerticalDeflectionRowDnD rowDnD = null;
						if (outerInstance.isComplete(selectedRow) && (rowDnD = VerticalDeflectionRowDnD.fromVerticalDeflectionRow(selectedRow)) != null)
						{
							rowsDnD.Add(rowDnD);
						}
					}

					if (rowsDnD.Count > 0)
					{
						Dragboard db = outerInstance.table.startDragAndDrop(TransferMode.MOVE);
						ClipboardContent content = new ClipboardContent();
						content.put(EditableMenuCheckBoxTreeCell.TREE_ITEM_TYPE_DATA_FORMAT, outerInstance.verticalDeflectionItemValue.ItemType);
						content.put(EditableMenuCheckBoxTreeCell.GROUP_ID_DATA_FORMAT, outerInstance.verticalDeflectionItemValue.GroupId);
						content.put(EditableMenuCheckBoxTreeCell.DIMENSION_DATA_FORMAT, outerInstance.verticalDeflectionItemValue.Dimension);
						content.put(EditableMenuCheckBoxTreeCell.VERTICAL_DEFLECTION_ROWS_DATA_FORMAT, rowsDnD);

						db.setContent(content);
					}
				}
				@event.consume();
			}
		}

		internal override void duplicateRows()
		{
			IList<VerticalDeflectionRow> selectedRows = new List<VerticalDeflectionRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<VerticalDeflectionRow> clonedRows = new List<VerticalDeflectionRow>(selectedRows.Count);
			foreach (VerticalDeflectionRow row in selectedRows)
			{
				VerticalDeflectionRow clonedRow = VerticalDeflectionRow.cloneRowApriori(row);
				if (this.isComplete(clonedRow))
				{
					try
					{
						// Generate next unique point name
						clonedRow.Name = SQLManager.Instance.getNextValidVerticalDeflectionName(row.Name);
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
				foreach (VerticalDeflectionRow clonedRow in clonedRows)
				{
					this.table.getSelectionModel().select(clonedRow);
				}
				this.table.scrollTo(clonedRows[0]);
			}
		}

		internal override void removeRows()
		{
			IList<VerticalDeflectionRow> selectedRows = new List<VerticalDeflectionRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<VerticalDeflectionRow> removedRows = new List<VerticalDeflectionRow>(selectedRows.Count);
			foreach (VerticalDeflectionRow row in selectedRows)
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

			IList<VerticalDeflectionRow> selectedRows = new List<VerticalDeflectionRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(this.verticalDeflectionItemValue.ItemType);
			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, false);
			try
			{
				SQLManager.Instance.saveGroup((VerticalDeflectionTreeItemValue)newTreeItem.getValue());
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
				int groupId = ((VerticalDeflectionTreeItemValue)newTreeItem.getValue()).GroupId;
				foreach (VerticalDeflectionRow row in selectedRows)
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
			IList<VerticalDeflectionRow> rows = this.table.getItems();

			string exportFormatString = "%15s \t";
			//String exportFormatDouble = "%+15.6f \t";
			string exportFormatDouble = "%20s \t";

			PrintWriter writer = null;

			try
			{
				writer = new PrintWriter(new StreamWriter(file));

				foreach (VerticalDeflectionRow row in rows)
				{
					if ((!row.Enable).Value)
					{
						continue;
					}

					string name = row.Name;

					if (string.ReferenceEquals(name, null) || name.Trim().Length == 0)
					{
						continue;
					}

					double? y = aprioriValues ? row.YApriori : row.YAposteriori;
					double? x = aprioriValues ? row.XApriori : row.XAposteriori;

					double? sigmaY = aprioriValues ? row.SigmaYapriori : row.SigmaYaposteriori;
					double? sigmaX = aprioriValues ? row.SigmaXapriori : row.SigmaXaposteriori;

					if (!aprioriValues && (sigmaY == null || sigmaX == null))
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
					}

					string yValue = String.format(Locale.ENGLISH, exportFormatDouble, options.toAngleFormat(y.Value, false));
					string xValue = String.format(Locale.ENGLISH, exportFormatDouble, options.toAngleFormat(x.Value, false));

					string sigmaYvalue = sigmaY != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toAngleFormat(sigmaY.Value, false)) : "";
					string sigmaXvalue = sigmaX != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toAngleFormat(sigmaX.Value, false)) : "";

					writer.println(String.format(exportFormatString, name) + yValue + xValue + sigmaYvalue + sigmaXvalue);
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

		internal override void highlightTableRow(TableRow<VerticalDeflectionRow> row)
		{
			if (row == null)
			{
				return;
			}

			TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
			TableRowHighlightType tableRowHighlightType = tableRowHighlight.SelectedTableRowHighlightType;
			double leftBoundary = tableRowHighlight.getLeftBoundary(tableRowHighlightType);
			double rightBoundary = tableRowHighlight.getRightBoundary(tableRowHighlightType);

			VerticalDeflectionRow item = row.getItem();

			if (!row.isSelected() && item != null)
			{
				switch (tableRowHighlightType.innerEnumValue)
				{
				case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
					if (this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION)
					{
						this.setTableRowHighlight(row, item.Significant ? TableRowHighlightRangeType.INADEQUATE : TableRowHighlightRangeType.EXCELLENT);
					}
					else
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
					}
					break;

				case TableRowHighlightType.InnerEnum.GROSS_ERROR:
					if (this.type != VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION)
					{
						double? grossErrorX = item.GrossErrorX;
						double? grossErrorY = item.GrossErrorY;

						double? mtbX = item.MaximumTolerableBiasX;
						double? mtbY = item.MaximumTolerableBiasY;

						double? mdbX = item.MinimalDetectableBiasX;
						double? mdbY = item.MinimalDetectableBiasY;

						double dMTB = Double.NaN;
						double dMDB = Double.NaN;

						if (grossErrorX != null && grossErrorY != null && mtbX != null && mtbY != null && mdbX != null && mdbY != null)
						{
							dMTB = Math.Max(Math.Abs(grossErrorX) - Math.Abs(mtbX), Math.Abs(grossErrorY) - Math.Abs(mtbY));
							dMDB = Math.Min(Math.Abs(mdbX) - Math.Abs(grossErrorX), Math.Abs(mdbY) - Math.Abs(grossErrorY));

							if (dMTB > 0 && dMDB >= 0)
							{
								this.setTableRowHighlight(row, TableRowHighlightRangeType.SATISFACTORY);
							}

							else if (dMDB < 0)
							{
								this.setTableRowHighlight(row, TableRowHighlightRangeType.INADEQUATE);
							}

							else // if (dMTB <= 0)
							{
								this.setTableRowHighlight(row, TableRowHighlightRangeType.EXCELLENT);
							}
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
					if (this.type == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION)
					{
						double? redundancyX = item.RedundancyX;
						double? redundancyY = item.RedundancyY;
						double? redundancy = null;

						if (redundancyY != null && redundancyX != null)
						{
							redundancy = Math.Min(redundancyY, redundancyX);
						}

						if (redundancy == null)
						{
							this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
						}
						else
						{
							this.setTableRowHighlight(row, redundancy.Value < leftBoundary ? TableRowHighlightRangeType.INADEQUATE : redundancy.Value <= rightBoundary ? TableRowHighlightRangeType.SATISFACTORY : TableRowHighlightRangeType.EXCELLENT);
						}
					}
					else
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
					}

					break;

				case TableRowHighlightType.InnerEnum.INFLUENCE_ON_POSITION:
					this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);

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