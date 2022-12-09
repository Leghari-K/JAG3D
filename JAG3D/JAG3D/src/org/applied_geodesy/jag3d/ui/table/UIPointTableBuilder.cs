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

	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using PointRowDnD = org.applied_geodesy.jag3d.ui.dnd.PointRowDnD;
	using ColumnContentType = org.applied_geodesy.jag3d.ui.table.column.ColumnContentType;
	using TableContentType = org.applied_geodesy.jag3d.ui.table.column.TableContentType;
	using PointRow = org.applied_geodesy.jag3d.ui.table.row.PointRow;
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightRangeType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using EditableMenuCheckBoxTreeCell = org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell;
	using PointTreeItemValue = org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue;
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

	public class UIPointTableBuilder : UIEditableTableBuilder<PointRow>
	{
		private class TableKey
		{
			private readonly UIPointTableBuilder outerInstance;

			internal readonly PointType type;
			internal readonly int dimension;
			internal TableKey(UIPointTableBuilder outerInstance, PointType type, int dimension)
			{
				this.outerInstance = outerInstance;
				this.dimension = dimension;
				this.type = type;
			}

			public override int GetHashCode()
			{
				const int prime = 31;
				int result = 1;
				result = prime * result + dimension;
				result = prime * result + ((type == null) ? 0 : type.GetHashCode());
				return result;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}

				if (obj == null)
				{
					return false;
				}

				if (this.GetType() != obj.GetType())
				{
					return false;
				}

				TableKey other = (TableKey) obj;
				if (dimension != other.dimension || type != other.type)
				{
					return false;
				}

				return true;
			}
		}

		private PointTreeItemValue pointItemValue;
		private PointType type;
		private int dimension;
		private IDictionary<TableKey, TableView<PointRow>> tables = new Dictionary<TableKey, TableView<PointRow>>();

		private static UIPointTableBuilder tableBuilder = new UIPointTableBuilder();
		private UIPointTableBuilder() : base()
		{
		}

		public static UIPointTableBuilder Instance
		{
			get
			{
				return tableBuilder;
			}
		}

		public virtual TableView<PointRow> getTable(PointTreeItemValue pointItemValue)
		{
			this.pointItemValue = pointItemValue;
			this.type = TreeItemType.getPointTypeByTreeItemType(pointItemValue.ItemType);
			this.dimension = pointItemValue.Dimension;
			this.init();
			return this.table;
		}

		private TableContentType TableContentType
		{
			get
			{
				switch (this.type.innerEnumValue)
				{
				case PointType.InnerEnum.REFERENCE_POINT:
					switch (this.dimension)
					{
					case 1:
						return TableContentType.REFERENCE_POINT_1D;
					case 2:
						return TableContentType.REFERENCE_POINT_2D;
					default:
						return TableContentType.REFERENCE_POINT_3D;
					}
				case PointType.InnerEnum.STOCHASTIC_POINT:
					switch (this.dimension)
					{
					case 1:
						return TableContentType.STOCHASTIC_POINT_1D;
					case 2:
						return TableContentType.STOCHASTIC_POINT_2D;
					default:
						return TableContentType.STOCHASTIC_POINT_3D;
					}
				case PointType.InnerEnum.DATUM_POINT:
					switch (this.dimension)
					{
					case 1:
						return TableContentType.DATUM_POINT_1D;
					case 2:
						return TableContentType.DATUM_POINT_2D;
					default:
						return TableContentType.DATUM_POINT_3D;
					}
				case PointType.InnerEnum.NEW_POINT:
					switch (this.dimension)
					{
					case 1:
						return TableContentType.NEW_POINT_1D;
					case 2:
						return TableContentType.NEW_POINT_2D;
					default:
						return TableContentType.NEW_POINT_3D;
					}
				default:
					return TableContentType.UNSPECIFIC;
				}
			}
		}

		private void init()
		{
			if (this.tables.ContainsKey(new TableKey(this, this.type, this.dimension)))
			{
				this.table = this.tables[new TableKey(this, this.type, this.dimension)];
				return;
			}
			TableColumn<PointRow, bool> booleanColumn = null;
			TableColumn<PointRow, string> stringColumn = null;
			TableColumn<PointRow, double> doubleColumn = null;

			TableContentType tableContentType = this.TableContentType;

			TableView<PointRow> table = this.createTable();
			///////////////// A-PRIORI VALUES /////////////////////////////
			// Enable/Disable
			int columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexEnable = columnIndex;
			int columnIndexEnable = columnIndex;
			string labelText = i18n.getString("UIPointTableBuilder.tableheader.enable.label", "Enable");
			string tooltipText = i18n.getString("UIPointTableBuilder.tableheader.enable.tooltip", "State of the point");
			CellValueType cellValueType = CellValueType.BOOLEAN;
			ColumnContentType columnContentType = ColumnContentType.ENABLE;
			ColumnTooltipHeader header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, PointRow::enableProperty, BooleanCallback, ColumnType.VISIBLE, columnIndex, true, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass(this, columnIndexEnable));
			table.getColumns().add(booleanColumn);

			// Point-ID
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.station.name.label", "Point-Id");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.station.name.tooltip", "Id of the point");
			cellValueType = CellValueType.STRING;
			columnContentType = ColumnContentType.POINT_NAME;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, PointRow::nameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// Code
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.station.code.label", "Code");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.station.code.tooltip", "Code of the point");
			cellValueType = CellValueType.STRING;
			columnContentType = ColumnContentType.CODE;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, PointRow::codeProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// A-priori Components
			// Y0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.y0.label", "y0");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.y0.tooltip", "A-priori y-component of the point");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Y_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::yAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_POINT, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// X0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.x0.label", "x0");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.x0.tooltip", "A-priori x-component of the point");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_X_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::xAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_POINT, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// Z0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.z0.label", "z0");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.z0.tooltip", "A-priori z-component of the point");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Z_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::zAprioriProperty, getDoubleCallback(cellValueType), this.dimension != 2 ? ColumnType.APRIORI_POINT : ColumnType.HIDDEN, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// A-priori Uncertainties
			// Y0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.sigma.y0.label", "\u03C3y0");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.sigma.y0.tooltip", "A-priori uncertainty of y-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Y_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::sigmaYaprioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 && type == PointType.STOCHASTIC_POINT ? ColumnType.APRIORI_POINT : ColumnType.HIDDEN, columnIndex, true, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// X0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.sigma.x0.label", "\u03C3x0");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.sigma.x0.tooltip", "A-priori uncertainty of x-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_X_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::sigmaXaprioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 && type == PointType.STOCHASTIC_POINT ? ColumnType.APRIORI_POINT : ColumnType.HIDDEN, columnIndex, true, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Z0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.sigma.z0.label", "\u03C3z0");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.sigma.z0.tooltip", "A-priori uncertainty of z-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Z_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::sigmaZaprioriProperty, getDoubleCallback(cellValueType), this.dimension != 2 && type == PointType.STOCHASTIC_POINT ? ColumnType.APRIORI_POINT : ColumnType.HIDDEN, columnIndex, true, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			///////////////// A-POSTERIORI VALUES /////////////////////////////
			// A-posteriori Components

			// Y-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.y.label", "y");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.y.tooltip", "A-posteriori y-component of the point");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Y_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::yAposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// X-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.x.label", "x");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.x.tooltip", "A-posteriori x-component of the point");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_X_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::xAposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Z-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.z.label", "z");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.z.tooltip", "A-posteriori z-component of the point");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Z_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::zAposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 2 ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);


			// A-posteriori Uncertainties
			// Y-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.sigma.y.label", "\u03C3y");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.sigma.y.tooltip", "A-posteriori uncertainty of y-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Y_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::sigmaYaposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 && type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// X-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.sigma.x.label", "\u03C3x");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.sigma.x.tooltip", "A-posteriori uncertainty of x-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_X_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::sigmaXaposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 && type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Z-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.sigma.z.label", "\u03C3z");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.sigma.z.tooltip", "A-posteriori uncertainty of z-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Z_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::sigmaZaposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 2 && type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Confidence
			// A
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.semiaxis.major.label", "a");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.semiaxis.major.tooltip", "Major semi axis");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.CONFIDENCE_A;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::confidenceAProperty, getDoubleCallback(cellValueType), this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// B
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.semiaxis.middle.label", "b");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.semiaxis.middle.tooltip", "Middle semi axis");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.CONFIDENCE_B;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::confidenceBProperty, getDoubleCallback(cellValueType), this.dimension == 3 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// C
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.semiaxis.minor.label", "c");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.semiaxis.minor.tooltip", "Minor semi axis");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.CONFIDENCE_C;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::confidenceCProperty, getDoubleCallback(cellValueType), this.dimension != 1 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// alpha
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.semiaxisrotation.alpha.label", "\u03B1");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.semiaxisrotation.alpha.tooltip", "Rotation angle of confidence region");
			cellValueType = CellValueType.ANGLE;
			columnContentType = ColumnContentType.CONFIDENCE_ALPHA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::confidenceAlphaProperty, getDoubleCallback(cellValueType), this.dimension == 3 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// beta
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.semiaxisrotation.beta.label", "\u03B2");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.semiaxisrotation.beta.tooltip", "Rotation angle of confidence region");
			cellValueType = CellValueType.ANGLE;
			columnContentType = ColumnContentType.CONFIDENCE_BETA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::confidenceBetaProperty, getDoubleCallback(cellValueType), this.dimension == 3 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// gamma
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.semiaxisrotation.gamma.label", "\u03B3");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.semiaxisrotation.gamma.tooltip", "Rotation angle of confidence region");
			cellValueType = CellValueType.ANGLE;
			columnContentType = ColumnContentType.CONFIDENCE_GAMMA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::confidenceGammaProperty, getDoubleCallback(cellValueType), this.dimension != 1 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Redundancy
			// ry
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.y.label", "ry");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.y.tooltip", "Redundancy of y-component");
			cellValueType = CellValueType.PERCENTAGE;
			columnContentType = ColumnContentType.REDUNDANCY_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::redundancyYProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT && this.dimension != 1 ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// rx
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.x.label", "rx");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.x.tooltip", "Redundancy of x-component");
			cellValueType = CellValueType.PERCENTAGE;
			columnContentType = ColumnContentType.REDUNDANCY_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::redundancyXProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT && this.dimension != 1 ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// rz
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.z.label", "rz");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.z.tooltip", "Redundancy of z-component");
			cellValueType = CellValueType.PERCENTAGE;
			columnContentType = ColumnContentType.REDUNDANCY_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::redundancyZProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT && this.dimension != 2 ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Residual 
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = this.type == PointType.STOCHASTIC_POINT ? i18n.getString("UIPointTableBuilder.tableheader.residual.y.label", "\u03B5y") : i18n.getString("UIPointTableBuilder.tableheader.deviation.y.label", "\u0394y");
			tooltipText = this.type == PointType.STOCHASTIC_POINT ? i18n.getString("UIPointTableBuilder.tableheader.residual.y.tooltip", "Residual of y-component") : i18n.getString("UIPointTableBuilder.tableheader.deviation.y.tooltip", "Deviation of y-component w.r.t. y0");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::residualYProperty, getDoubleCallback(cellValueType), this.dimension != 1 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = this.type == PointType.STOCHASTIC_POINT ? i18n.getString("UIPointTableBuilder.tableheader.residual.x.label", "\u03B5x") : i18n.getString("UIPointTableBuilder.tableheader.deviation.x.label", "\u0394x");
			tooltipText = this.type == PointType.STOCHASTIC_POINT ? i18n.getString("UIPointTableBuilder.tableheader.residual.x.tooltip", "Residual of x-component") : i18n.getString("UIPointTableBuilder.tableheader.deviation.x.tooltip", "Deviation of x-component w.r.t. x0");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::residualXProperty, getDoubleCallback(cellValueType), this.dimension != 1 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = this.type == PointType.STOCHASTIC_POINT ? i18n.getString("UIPointTableBuilder.tableheader.residual.z.label", "\u03B5z") : i18n.getString("UIPointTableBuilder.tableheader.deviation.z.label", "\u0394z");
			tooltipText = this.type == PointType.STOCHASTIC_POINT ? i18n.getString("UIPointTableBuilder.tableheader.residual.z.tooltip", "Residual of z-component") : i18n.getString("UIPointTableBuilder.tableheader.deviation.z.tooltip", "Deviation of z-component w.r.t. z0");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::residualZProperty, getDoubleCallback(cellValueType), this.dimension != 2 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Gross-Error
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.y.label", "\u2207y");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.y.tooltip", "Gross-error in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.GROSS_ERROR_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::grossErrorYProperty, getDoubleCallback(cellValueType), this.dimension != 1 && (this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT) ? ColumnType.APOSTERIORI_POINT : this.dimension != 1 && this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.x.label", "\u2207x");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.x.tooltip", "Gross-error in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.GROSS_ERROR_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::grossErrorXProperty, getDoubleCallback(cellValueType), this.dimension != 1 && (this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT) ? ColumnType.APOSTERIORI_POINT : this.dimension != 1 && this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.z.label", "\u2207z");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.z.tooltip", "Gross-error in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.GROSS_ERROR_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::grossErrorZProperty, getDoubleCallback(cellValueType), this.dimension != 2 && (this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT) ? ColumnType.APOSTERIORI_POINT : this.dimension != 2 && this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// MTB
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.y.label", "\u2207y(1)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.y.tooltip", "Maximum tolerable bias in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::maximumTolerableBiasYProperty, getDoubleCallback(cellValueType), this.dimension != 1 && (this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT) ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.x.label", "\u2207x(1)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.x.tooltip", "Maximum tolerable bias in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::maximumTolerableBiasXProperty, getDoubleCallback(cellValueType), this.dimension != 1 && (this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT) ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.z.label", "\u2207z(1)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.z.tooltip", "Maximum tolerable bias in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::maximumTolerableBiasZProperty, getDoubleCallback(cellValueType), this.dimension != 2 && (this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT) ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// MDB
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.y.label", "\u2207y(\u03BB)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.y.tooltip", "Minimal detectable bias in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::minimalDetectableBiasYProperty, getDoubleCallback(cellValueType), this.dimension != 1 && (this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT) ? ColumnType.APOSTERIORI_POINT : this.dimension != 1 && this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.x.label", "\u2207x(\u03BB)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.x.tooltip", "Minimal detectable bias in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::minimalDetectableBiasXProperty, getDoubleCallback(cellValueType), this.dimension != 1 && (this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT) ? ColumnType.APOSTERIORI_POINT : this.dimension != 1 && this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.z.label", "\u2207z(\u03BB)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.z.tooltip", "Minimal detectable bias in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::minimalDetectableBiasZProperty, getDoubleCallback(cellValueType), this.dimension != 2 && (this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT) ? ColumnType.APOSTERIORI_POINT : this.dimension != 2 && this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Influence on point position (EP)
			// EP-y
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.influenceonposition.y.label", "EPy");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.influenceonposition.y.tooltip", "Influence on point position due to an undetected gross-error in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_POINT_POSITION_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::influenceOnPointPositionYProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT && this.dimension != 1 ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// EP-x
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.influenceonposition.x.label", "EPx");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.influenceonposition.x.tooltip", "Influence on point position due to an undetected gross-error in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_POINT_POSITION_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::influenceOnPointPositionXProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT && this.dimension != 1 ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// EP-z
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.influenceonposition.z.label", "EPz");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.influenceonposition.z.tooltip", "Influence on point position due to an undetected gross-error in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_POINT_POSITION_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::influenceOnPointPositionZProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT && this.dimension != 2 ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// EFSP
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.influenceonnetworkdistortion.label", "EF\u00B7SPmax");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.influenceonnetworkdistortion.tooltip", "Maximum influence on network distortion");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_NETWORK_DISTORTION;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::influenceOnNetworkDistortionProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// PCA
			// y-comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.principalcomponent.y.label", "\u03C2y");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.principalcomponent.y.tooltip", "Y-component of first principal component analysis");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.FIRST_PRINCIPLE_COMPONENT_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::firstPrincipalComponentYProperty, getDoubleCallback(cellValueType), this.dimension != 1 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.principalcomponent.x.label", "\u03C2x");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.principalcomponent.x.tooltip", "X-component of first principal component analysis");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.FIRST_PRINCIPLE_COMPONENT_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::firstPrincipalComponentXProperty, getDoubleCallback(cellValueType), this.dimension != 1 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.principalcomponent.z.label", "\u03C2z");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.principalcomponent.z.tooltip", "Z-component of first principal component analysis");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.FIRST_PRINCIPLE_COMPONENT_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::firstPrincipalComponentZProperty, getDoubleCallback(cellValueType), this.dimension != 2 && this.type != PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// vTPv
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.omega.label", "\u03A9");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.omega.tooltip", "Weighted squares of residual");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.OMEGA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::omegaProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.pvalue.apriori.label", "log(Pprio)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.pvalue.apriori.tooltip", "A-priori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::pValueAprioriProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.pvalue.aposteriori.label", "log(Ppost)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.pvalue.aposteriori.tooltip", "A-posteriori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::pValueAposterioriProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Tprio
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.teststatistic.apriori.label", "Tprio");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.teststatistic.apriori.tooltip", "A-priori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::testStatisticAprioriProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Tpost
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.teststatistic.aposteriori.label", "Tpost");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.teststatistic.aposteriori.tooltip", "A-posteriori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, PointRow::testStatisticAposterioriProperty, getDoubleCallback(cellValueType), this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Decision of test statistic
			columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexOutlier = columnIndex;
			int columnIndexOutlier = columnIndex;
			labelText = i18n.getString("UIPointTableBuilder.tableheader.testdecision.label", "Significant");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.testdecision.tooltip", "Checked, if null-hypothesis is rejected");
			cellValueType = CellValueType.BOOLEAN;
			columnContentType = ColumnContentType.SIGNIFICANT;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, PointRow::significantProperty, BooleanCallback, this.type == PointType.STOCHASTIC_POINT || this.type == PointType.REFERENCE_POINT ? ColumnType.APOSTERIORI_POINT : this.type == PointType.DATUM_POINT ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass2(this, columnIndexOutlier));
			table.getColumns().add(booleanColumn);

			this.addContextMenu(table, this.createContextMenu(true));
			this.addDynamicRowAdder(table);
			this.addColumnOrderSequenceListeners(tableContentType, table);

			this.tables[new TableKey(this, this.type, this.dimension)] = table;
			this.table = table;
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn.CellDataFeatures<PointRow, bool>, ObservableValue<bool>>
		{
			private readonly UIPointTableBuilder outerInstance;

			private int columnIndexEnable;

			public CallbackAnonymousInnerClass(UIPointTableBuilder outerInstance, int columnIndexEnable)
			{
				this.outerInstance = outerInstance;
				this.columnIndexEnable = columnIndexEnable;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<PointRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isEnable());
				booleanProp.addListener(enableChangeListener);
				return booleanProp;
			}
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn.CellDataFeatures<PointRow, bool>, ObservableValue<bool>>
		{
			private readonly UIPointTableBuilder outerInstance;

			private int columnIndexOutlier;

			public CallbackAnonymousInnerClass2(UIPointTableBuilder outerInstance, int columnIndexOutlier)
			{
				this.outerInstance = outerInstance;
				this.columnIndexOutlier = columnIndexOutlier;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<PointRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isSignificant());
				booleanProp.addListener(significantChangeListener);
				return booleanProp;
			}
		}


		public override PointRow EmptyRow
		{
			get
			{
				return new PointRow();
			}
		}

		internal override void setValue(PointRow rowData, int columnIndex, object oldValue, object newValue)
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
				rowData.Code = newValue != null ? newValue.ToString().Trim() : null;
				valid = true;
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
					rowData.GroupId = this.pointItemValue.GroupId;
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

		private bool isComplete(PointRow row)
		{
			return !string.ReferenceEquals(row.Name, null) && row.Name.Trim().Length > 0 && row.XApriori != null && row.YApriori != null && row.ZApriori != null;
		}

		internal override void enableDragSupport()
		{
			this.table.setOnDragDetected(new EventHandlerAnonymousInnerClass(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<MouseEvent>
		{
			private readonly UIPointTableBuilder outerInstance;

			public EventHandlerAnonymousInnerClass(UIPointTableBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void handle(MouseEvent @event)
			{
				IList<PointRow> selectedRows = new List<PointRow>(outerInstance.table.getSelectionModel().getSelectedItems());
				if (selectedRows != null && selectedRows.Count > 0)
				{

					IList<PointRowDnD> rowsDnD = new List<PointRowDnD>(selectedRows.Count);
					foreach (PointRow selectedRow in selectedRows)
					{
						PointRowDnD rowDnD = null;
						if (outerInstance.isComplete(selectedRow) && (rowDnD = PointRowDnD.fromPointRow(selectedRow)) != null)
						{
							rowsDnD.Add(rowDnD);
						}
					}

					if (rowsDnD.Count > 0)
					{
						Dragboard db = outerInstance.table.startDragAndDrop(TransferMode.MOVE);
						ClipboardContent content = new ClipboardContent();
						content.put(EditableMenuCheckBoxTreeCell.TREE_ITEM_TYPE_DATA_FORMAT, outerInstance.pointItemValue.ItemType);
						content.put(EditableMenuCheckBoxTreeCell.GROUP_ID_DATA_FORMAT, outerInstance.pointItemValue.GroupId);
						content.put(EditableMenuCheckBoxTreeCell.DIMENSION_DATA_FORMAT, outerInstance.dimension);
						content.put(EditableMenuCheckBoxTreeCell.POINT_ROWS_DATA_FORMAT, rowsDnD);

						db.setContent(content);
					}
				}
				@event.consume();
			}
		}

		internal override void duplicateRows()
		{
			IList<PointRow> selectedRows = new List<PointRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<PointRow> clonedRows = new List<PointRow>(selectedRows.Count);
			foreach (PointRow row in selectedRows)
			{
				PointRow clonedRow = PointRow.cloneRowApriori(row);

				if (this.isComplete(clonedRow))
				{
					try
					{
						// Generate next unique point name
						clonedRow.Name = SQLManager.Instance.getNextValidPointName(row.Name);
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
				foreach (PointRow clonedRow in clonedRows)
				{
					this.table.getSelectionModel().select(clonedRow);
				}
				this.table.scrollTo(clonedRows[0]);
			}
		}

		internal override void removeRows()
		{
			IList<PointRow> selectedRows = new List<PointRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<PointRow> removedRows = new List<PointRow>(selectedRows.Count);
			foreach (PointRow row in selectedRows)
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
			TreeItemType parentType = null;

			switch (type)
			{
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_DATUM:
				if (this.dimension == 1)
				{
					parentType = TreeItemType.DATUM_POINT_1D_DIRECTORY;
				}
				else if (this.dimension == 2)
				{
					parentType = TreeItemType.DATUM_POINT_2D_DIRECTORY;
				}
				else if (this.dimension == 3)
				{
					parentType = TreeItemType.DATUM_POINT_3D_DIRECTORY;
				}
				break;
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_NEW:
				if (this.dimension == 1)
				{
					parentType = TreeItemType.NEW_POINT_1D_DIRECTORY;
				}
				else if (this.dimension == 2)
				{
					parentType = TreeItemType.NEW_POINT_2D_DIRECTORY;
				}
				else if (this.dimension == 3)
				{
					parentType = TreeItemType.NEW_POINT_3D_DIRECTORY;
				}
				break;
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_REFERENCE:
				if (this.dimension == 1)
				{
					parentType = TreeItemType.REFERENCE_POINT_1D_DIRECTORY;
				}
				else if (this.dimension == 2)
				{
					parentType = TreeItemType.REFERENCE_POINT_2D_DIRECTORY;
				}
				else if (dimension == 3)
				{
					parentType = TreeItemType.REFERENCE_POINT_3D_DIRECTORY;
				}
				break;
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_STOCHASTIC:
				if (this.dimension == 1)
				{
					parentType = TreeItemType.STOCHASTIC_POINT_1D_DIRECTORY;
				}
				else if (this.dimension == 2)
				{
					parentType = TreeItemType.STOCHASTIC_POINT_2D_DIRECTORY;
				}
				else if (this.dimension == 3)
				{
					parentType = TreeItemType.STOCHASTIC_POINT_3D_DIRECTORY;
				}
				break;
			default:
				parentType = null;
				break;
			}

			IList<PointRow> selectedRows = new List<PointRow>(this.table.getSelectionModel().getSelectedItems());
			if (parentType == null || selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, false);
			try
			{
				SQLManager.Instance.saveGroup((PointTreeItemValue)newTreeItem.getValue());
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
				int groupId = ((PointTreeItemValue)newTreeItem.getValue()).GroupId;
				foreach (PointRow row in selectedRows)
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
			IList<PointRow> rows = this.table.getItems();

			string exportFormatString = "%15s \t";
			//String exportFormatDouble = "%+15.6f \t";
			string exportFormatDouble = "%20s \t";

			PrintWriter writer = null;

			try
			{
				writer = new PrintWriter(new StreamWriter(file));

				foreach (PointRow row in rows)
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
					double? z = aprioriValues ? row.ZApriori : row.ZAposteriori;

					if (this.dimension != 2 && z == null)
					{
						continue;
					}

					if (this.dimension != 1 && (y == null || x == null))
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

					string yValue = y != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(y.Value, false)) : "";
					string xValue = x != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(x.Value, false)) : "";
					string zValue = this.dimension != 2 && z != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(z.Value, false)) : "";

					string sigmaYvalue = this.dimension != 1 && sigmaY != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigmaY.Value, false)) : "";
					string sigmaXvalue = this.dimension != 1 && sigmaX != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigmaX.Value, false)) : "";
					string sigmaZvalue = this.dimension != 2 && sigmaZ != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigmaZ.Value, false)) : "";

					writer.println(String.format(exportFormatString, name) + yValue + xValue + zValue + sigmaYvalue + sigmaXvalue + sigmaZvalue);
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

		internal override void highlightTableRow(TableRow<PointRow> row)
		{
			if (row == null)
			{
				return;
			}

			TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
			TableRowHighlightType tableRowHighlightType = tableRowHighlight.SelectedTableRowHighlightType;
			double leftBoundary = tableRowHighlight.getLeftBoundary(tableRowHighlightType);
			double rightBoundary = tableRowHighlight.getRightBoundary(tableRowHighlightType);

			PointRow item = row.getItem();

			if (!row.isSelected() && item != null)
			{
				switch (tableRowHighlightType.innerEnumValue)
				{
				case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
					if (this.type != PointType.NEW_POINT)
					{
						this.setTableRowHighlight(row, item.Significant ? TableRowHighlightRangeType.INADEQUATE : TableRowHighlightRangeType.EXCELLENT);
					}
					else
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
					}
					break;

				case TableRowHighlightType.InnerEnum.GROSS_ERROR:
					if (this.type != PointType.NEW_POINT)
					{
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

						if (this.dimension == 1 && grossErrorZ != null && mtbZ != null && mdbZ != null)
						{
							dMTB = Math.Abs(grossErrorZ) - Math.Abs(mtbZ);
							dMDB = Math.Abs(mdbZ) - Math.Abs(grossErrorZ);
						}

						else if (this.dimension == 2 && grossErrorX != null && grossErrorY != null && mtbX != null && mtbY != null && mdbX != null && mdbY != null)
						{
							dMTB = Math.Max(Math.Abs(grossErrorX) - Math.Abs(mtbX), Math.Abs(grossErrorY) - Math.Abs(mtbY));
							dMDB = Math.Min(Math.Abs(mdbX) - Math.Abs(grossErrorX), Math.Abs(mdbY) - Math.Abs(grossErrorY));
						}

						else if (this.dimension == 3 && grossErrorX != null && grossErrorY != null && grossErrorZ != null && mtbZ != null && mtbX != null && mtbY != null && mdbZ != null && mdbX != null && mdbY != null)
						{
							dMTB = Math.Max(Math.Max(Math.Abs(grossErrorX) - Math.Abs(mtbX), Math.Abs(grossErrorY) - Math.Abs(mtbY)), Math.Abs(grossErrorZ) - Math.Abs(mtbZ));
							dMDB = Math.Min(Math.Min(Math.Abs(mdbX) - Math.Abs(grossErrorX), Math.Abs(mdbY) - Math.Abs(grossErrorY)), Math.Abs(mdbZ) - Math.Abs(grossErrorZ));
						}

						if (!double.IsNaN(dMTB) && !double.IsNaN(dMDB))
						{
							if (dMDB < 0)
							{
								this.setTableRowHighlight(row, TableRowHighlightRangeType.INADEQUATE);
							}

							else if (dMTB > 0 && dMDB >= 0)
							{
								this.setTableRowHighlight(row, TableRowHighlightRangeType.SATISFACTORY);
							}

							else // (dMTB <= 0)
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
					if (this.type == PointType.STOCHASTIC_POINT)
					{
						double? redundancyX = item.RedundancyX;
						double? redundancyY = item.RedundancyY;
						double? redundancyZ = item.RedundancyZ;
						double? redundancy = null;

						if (this.dimension == 1 && redundancyZ != null)
						{
							redundancy = redundancyZ;
						}

						else if (this.dimension == 2 && redundancyY != null && redundancyX != null)
						{
							redundancy = Math.Min(redundancyY, redundancyX);
						}

						else if (this.dimension == 3 && redundancyY != null && redundancyX != null && redundancyZ != null)
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
					}
					else
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
					}

					break;

				case TableRowHighlightType.InnerEnum.INFLUENCE_ON_POSITION:
					if (this.type == PointType.STOCHASTIC_POINT)
					{
						double? influenceOnPositionX = item.InfluenceOnPointPositionX;
						double? influenceOnPositionY = item.InfluenceOnPointPositionY;
						double? influenceOnPositionZ = item.InfluenceOnPointPositionZ;
						double? influenceOnPosition = null;

						if (this.dimension == 1 && influenceOnPositionZ != null)
						{
							influenceOnPosition = influenceOnPositionZ;
						}

						else if (this.dimension == 2 && influenceOnPositionY != null && influenceOnPositionX != null)
						{
							influenceOnPosition = Math.Max(influenceOnPositionY, influenceOnPositionX);
						}

						else if (this.dimension == 3 && influenceOnPositionY != null && influenceOnPositionX != null && influenceOnPositionZ != null)
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
					}
					else
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
					}

					break;

				case TableRowHighlightType.InnerEnum.P_PRIO_VALUE:
					if (this.type != PointType.NEW_POINT)
					{
						double? pValue = item.PValueApriori;
						if (pValue == null)
						{
							this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
						}
						else
						{
							this.setTableRowHighlight(row, pValue < Math.Log(leftBoundary / 100.0) ? TableRowHighlightRangeType.INADEQUATE : pValue <= Math.Log(rightBoundary / 100.0) ? TableRowHighlightRangeType.SATISFACTORY : TableRowHighlightRangeType.EXCELLENT);
						}
					}
					else
					{
						this.setTableRowHighlight(row, TableRowHighlightRangeType.NONE);
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