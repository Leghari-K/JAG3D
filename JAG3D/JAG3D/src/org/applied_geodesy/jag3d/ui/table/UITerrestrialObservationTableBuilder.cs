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
	using TerrestrialObservationRowDnD = org.applied_geodesy.jag3d.ui.dnd.TerrestrialObservationRowDnD;
	using ColumnContentType = org.applied_geodesy.jag3d.ui.table.column.ColumnContentType;
	using TableContentType = org.applied_geodesy.jag3d.ui.table.column.TableContentType;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
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
	using CellDataFeatures = javafx.scene.control.TableColumn.CellDataFeatures;
	using ClipboardContent = javafx.scene.input.ClipboardContent;
	using Dragboard = javafx.scene.input.Dragboard;
	using MouseEvent = javafx.scene.input.MouseEvent;
	using TransferMode = javafx.scene.input.TransferMode;
	using TableView = javafx.scene.control.TableView;
	using TreeItem = javafx.scene.control.TreeItem;
	using Callback = javafx.util.Callback;

	public class UITerrestrialObservationTableBuilder : UIEditableTableBuilder<TerrestrialObservationRow>
	{
		private ObservationTreeItemValue observationItemValue;
		private ObservationType type;
		private IDictionary<ObservationType, TableView<TerrestrialObservationRow>> tables = new Dictionary<ObservationType, TableView<TerrestrialObservationRow>>();

		private static UITerrestrialObservationTableBuilder tableBuilder = new UITerrestrialObservationTableBuilder();
		private UITerrestrialObservationTableBuilder() : base()
		{
		}

		public static UITerrestrialObservationTableBuilder Instance
		{
			get
			{
				return tableBuilder;
			}
		}

		public virtual TableView<TerrestrialObservationRow> getTable(ObservationTreeItemValue observationItemValue)
		{
			this.observationItemValue = observationItemValue;
			this.type = TreeItemType.getObservationTypeByTreeItemType(observationItemValue.ItemType);
			switch (this.type.innerEnumValue)
			{
			case ObservationType.InnerEnum.LEVELING:
			case ObservationType.InnerEnum.DIRECTION:
			case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
			case ObservationType.InnerEnum.SLOPE_DISTANCE:
			case ObservationType.InnerEnum.ZENITH_ANGLE:
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
				case ObservationType.InnerEnum.LEVELING:
					return TableContentType.LEVELING;
				case ObservationType.InnerEnum.DIRECTION:
					return TableContentType.DIRECTION;
				case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
					return TableContentType.HORIZONTAL_DISTANCE;
				case ObservationType.InnerEnum.SLOPE_DISTANCE:
					return TableContentType.SLOPE_DISTANCE;
				case ObservationType.InnerEnum.ZENITH_ANGLE:
					return TableContentType.ZENITH_ANGLE;
				default:
					return TableContentType.UNSPECIFIC;
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
			TableColumn<TerrestrialObservationRow, bool> booleanColumn = null;
			TableColumn<TerrestrialObservationRow, string> stringColumn = null;
			TableColumn<TerrestrialObservationRow, double> doubleColumn = null;

			TableContentType tableContentType = TableContentType;

			TableView<TerrestrialObservationRow> table = this.createTable();
			///////////////// A-PRIORI VALUES /////////////////////////////

			// Enable/Disable
			int columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexEnable = columnIndex;
			int columnIndexEnable = columnIndex;
			string labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.enable.label", "Enable");
			string tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.enable.tooltip", "State of the observation");
			CellValueType cellValueType = CellValueType.BOOLEAN;
			ColumnContentType columnContentType = ColumnContentType.ENABLE;
			ColumnTooltipHeader header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, TerrestrialObservationRow::enableProperty, BooleanCallback, ColumnType.VISIBLE, columnIndex, true, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass(this, columnIndexEnable));
			table.getColumns().add(booleanColumn);

			// Station-ID
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.station.name.label", "Station-Id");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.station.name.tooltip", "Id of station");
			cellValueType = CellValueType.STRING;
			columnContentType = ColumnContentType.START_POINT_NAME;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, TerrestrialObservationRow::startPointNameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// Target-ID
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.target.name.label", "Target-Id");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.target.name.tooltip", "Id of target point");
			cellValueType = CellValueType.STRING;
			columnContentType = ColumnContentType.END_POINT_NAME;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, TerrestrialObservationRow::endPointNameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// Station height
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.station.height.label", "ih");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.station.height.tooltip", "Instrument height");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.INSTRUMENT_HEIGHT;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::instrumentHeightProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// Target height
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.target.height.label", "th");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.target.height.tooltip", "Target height");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.REFLECTOR_HEIGHT;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::reflectorHeightProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			// Terrestrial Observation a-priori and uncertainties
			switch (this.type.innerEnumValue)
			{
			case ObservationType.InnerEnum.LEVELING:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.leveling.label", "\u03B4h0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.leveling.tooltip", "A-priori height difference");
				cellValueType = CellValueType.LENGTH;
				columnContentType = ColumnContentType.VALUE_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.leveling.label", "\u03C3h0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.leveling.tooltip", "A-priori uncertainty of height difference");
				cellValueType = CellValueType.LENGTH_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			case ObservationType.InnerEnum.DIRECTION:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.direction.label", "t0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.direction.tooltip", "A-priori direction");
				cellValueType = CellValueType.ANGLE;
				columnContentType = ColumnContentType.VALUE_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.direction.label", "\u03C3t0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.direction.tooltip", "A-priori uncertainty of direction");
				cellValueType = CellValueType.ANGLE_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.horizontal_distance.label", "sh0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.horizontal_distance.tooltip", "A-priori horizontal distance");
				cellValueType = CellValueType.LENGTH;
				columnContentType = ColumnContentType.VALUE_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.horizontal_distance.label", "\u03C3sh0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.horizontal_distance.tooltip", "A-priori uncertainty of horizontal distance");
				cellValueType = CellValueType.LENGTH_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			case ObservationType.InnerEnum.SLOPE_DISTANCE:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.slope_distance.label", "s0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.slope_distance.tooltip", "A-priori slope distance");
				cellValueType = CellValueType.LENGTH;
				columnContentType = ColumnContentType.VALUE_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.slope_distance.label", "\u03C3s0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.slope_distance.tooltip", "A-priori uncertainty of slope distance");
				cellValueType = CellValueType.LENGTH_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			case ObservationType.InnerEnum.ZENITH_ANGLE:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.zenith_angle.label", "v0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value0.zenith_angle.tooltip", "A-priori zenith angle");
				cellValueType = CellValueType.ANGLE;
				columnContentType = ColumnContentType.VALUE_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.zenith_angle.label", "\u03C3v0");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty0.zenith_angle.tooltip", "A-priori uncertainty of zenith angle");
				cellValueType = CellValueType.ANGLE_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APRIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			default:
				Console.Error.WriteLine(this.GetType().Name + " : Unsupported observation type " + type);
				break;
			}

			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.distance.label", "d0");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.distance.tooltip", "Length approximation for distance dependent uncertainty calculation");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.APPROXIMATED_DISTANCE_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::distanceAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APRIORI_TERRESTRIAL_OBSERVATION, columnIndex, true, true);
			table.getColumns().add(doubleColumn);

			//////////////////// ADJUSTED VALUES //////////////////////////

			// Terrestrial Observation a-posteriori and uncertainties; Gross error and minimal detectable bias
			switch (type.innerEnumValue)
			{
			case ObservationType.InnerEnum.LEVELING:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.leveling.label", "\u03B4h");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.leveling.tooltip", "A-posteriori height difference");
				cellValueType = CellValueType.LENGTH;
				columnContentType = ColumnContentType.VALUE_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.leveling.label", "\u03C3h");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.leveling.tooltip", "A-posteriori uncertainty of height difference");
				cellValueType = CellValueType.LENGTH_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			case ObservationType.InnerEnum.DIRECTION:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.direction.label", "t");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.direction.tooltip", "A-posteriori direction");
				cellValueType = CellValueType.ANGLE;
				columnContentType = ColumnContentType.VALUE_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.direction.label", "\u03C3t");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.direction.tooltip", "A-posteriori uncertainty of direction");
				cellValueType = CellValueType.ANGLE_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.horizontal_distance.label", "sh");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.horizontal_distance.tooltip", "A-posteriori horizontal distance");
				cellValueType = CellValueType.LENGTH;
				columnContentType = ColumnContentType.VALUE_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.horizontal_distance.label", "\u03C3sh");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.horizontal_distance.tooltip", "A-posteriori uncertainty of horizontal distance");
				cellValueType = CellValueType.LENGTH_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			case ObservationType.InnerEnum.SLOPE_DISTANCE:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.slope_distance.label", "s");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.slope_distance.tooltip", "A-posteriori slope distance");
				cellValueType = CellValueType.LENGTH;
				columnContentType = ColumnContentType.VALUE_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.slope_distance.label", "\u03C3s");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.slope_distance.tooltip", "A-posteriori uncertainty of slope distance");
				cellValueType = CellValueType.LENGTH_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			case ObservationType.InnerEnum.ZENITH_ANGLE:
				// Value
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.zenith_angle.label", "v");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.value.zenith_angle.tooltip", "A-posteriori zenith angle");
				cellValueType = CellValueType.ANGLE;
				columnContentType = ColumnContentType.VALUE_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::valueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				table.getColumns().add(doubleColumn);

				// Uncertainty
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.zenith_angle.label", "\u03C3v");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.uncertainty.zenith_angle.tooltip", "A-posteriori uncertainty of zenith angle");
				cellValueType = CellValueType.ANGLE_UNCERTAINTY;
				columnContentType = ColumnContentType.UNCERTAINTY_APOSTERIORI;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::sigmaAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			default:
				Console.Error.WriteLine(this.GetType().Name + " : Unsupported observation type " + type);
				break;
			}

			// Redundance
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.redundancy.label", "r");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.redundancy.tooltip", "Redundancy");
			cellValueType = CellValueType.PERCENTAGE;
			columnContentType = ColumnContentType.REDUNDANCY;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::redundancyProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Estimated gross error and minimal detectable bias
			switch (type.innerEnumValue)
			{
			case ObservationType.InnerEnum.LEVELING:
			case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
			case ObservationType.InnerEnum.SLOPE_DISTANCE:
				// Residual
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.residual.label", "\u03B5");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.residual.tooltip", "Residual");
				cellValueType = CellValueType.LENGTH_RESIDUAL;
				columnContentType = ColumnContentType.RESIDUAL;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::residualProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				// Gross error
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.grosserror.label", "\u2207");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.grosserror.tooltip", "Estimated gross error");
				cellValueType = CellValueType.LENGTH_RESIDUAL;
				columnContentType = ColumnContentType.GROSS_ERROR;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::grossErrorProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				// Maximum tolerable bias
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.maximumtolerablebias.label", "\u2207(1)");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.maximumtolerablebias.tooltip", "Maximum tolerable bias");
				cellValueType = CellValueType.LENGTH_RESIDUAL;
				columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::maximumTolerableBiasProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				// Minimal detectable bias
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.minimaldetectablebias.label", "\u2207(\u03BB)");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.minimaldetectablebias.tooltip", "Minimal detectable bias");
				cellValueType = CellValueType.LENGTH_RESIDUAL;
				columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::minimalDetectableBiasProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			case ObservationType.InnerEnum.DIRECTION:
			case ObservationType.InnerEnum.ZENITH_ANGLE:
				// Residual
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.residual.label", "\u03B5");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.residual.tooltip", "Residual");
				cellValueType = CellValueType.ANGLE_RESIDUAL;
				columnContentType = ColumnContentType.RESIDUAL;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::residualProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				// Gross error
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.grosserror.label", "\u2207");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.grosserror.tooltip", "Estimated gross error");
				cellValueType = CellValueType.ANGLE_RESIDUAL;
				columnContentType = ColumnContentType.GROSS_ERROR;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::grossErrorProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				// Maximum tolerable bias
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.maximumtolerablebias.label", "\u2207(1)");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.maximumtolerablebias.tooltip", "Maximum tolerable bias");
				cellValueType = CellValueType.ANGLE_RESIDUAL;
				columnContentType = ColumnContentType.MAXIMUM_TOLERABLE_BIAS;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::maximumTolerableBiasProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				// Minimal detectable bias
				columnIndex = table.getColumns().size();
				labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.minimaldetectablebias.label", "\u2207(\u03BB)");
				tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.minimaldetectablebias.tooltip", "Minimal detectable bias");
				cellValueType = CellValueType.ANGLE_RESIDUAL;
				columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS;
				header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
				doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::minimalDetectableBiasProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
				doubleColumn.setComparator(new AbsoluteValueComparator());
				table.getColumns().add(doubleColumn);

				break;
			default:
				Console.Error.WriteLine(this.GetType().Name + " : Unsupported observation type " + type);
				break;
			}

			// Influence on point position
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.influenceonposition.label", "EP");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.influenceonposition.tooltip", "Influence on point position due to an undetected gross-error");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_POINT_POSITION;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::influenceOnPointPositionProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Influence on network distortion
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.influenceonnetworkdistortion.label", "EF\u00B7SPmax");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.influenceonnetworkdistortion.tooltip", "Maximal influence on network distortion");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.INFLUENCE_ON_NETWORK_DISTORTION;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::influenceOnNetworkDistortionProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Omega
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.omega.label", "\u03A9");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.omega.tooltip", "Weighted squares of residual");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.OMEGA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::omegaProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// A-priori log(p)
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.pvalue.apriori.label", "log(Pprio)");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.pvalue.apriori.tooltip", "A-priori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::pValueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// A-priori log(p)
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.pvalue.aposteriori.label", "log(Ppost)");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.pvalue.aposteriori.tooltip", "A-posteriori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::pValueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// A-priori test statistic
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.teststatistic.apriori.label", "Tprio");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.teststatistic.apriori.tooltip", "A-priori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::testStatisticAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// A-posteriori test statistic
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.teststatistic.aposteriori.label", "Tpost");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.teststatistic.aposteriori.tooltip", "A-posteriori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, TerrestrialObservationRow::testStatisticAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Decision of test statistic
			columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexOutlier = columnIndex;
			int columnIndexOutlier = columnIndex;
			labelText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.testdecision.label", "Significant");
			tooltipText = i18n.getString("UITerrestrialObservationTableBuilder.tableheader.testdecision.tooltip", "Checked, if null-hypothesis is rejected");
			cellValueType = CellValueType.BOOLEAN;
			columnContentType = ColumnContentType.SIGNIFICANT;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, TerrestrialObservationRow::significantProperty, BooleanCallback, ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION, columnIndex, false, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass2(this, columnIndexOutlier));
			table.getColumns().add(booleanColumn);

			this.addContextMenu(table, this.createContextMenu(false));
			this.addDynamicRowAdder(table);
			this.addColumnOrderSequenceListeners(tableContentType, table);

			this.tables[this.type] = table;
			this.table = table;

		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn.CellDataFeatures<TerrestrialObservationRow, bool>, ObservableValue<bool>>
		{
			private readonly UITerrestrialObservationTableBuilder outerInstance;

			private int columnIndexEnable;

			public CallbackAnonymousInnerClass(UITerrestrialObservationTableBuilder outerInstance, int columnIndexEnable)
			{
				this.outerInstance = outerInstance;
				this.columnIndexEnable = columnIndexEnable;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<TerrestrialObservationRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isEnable());
				booleanProp.addListener(enableChangeListener);
				return booleanProp;
			}
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn.CellDataFeatures<TerrestrialObservationRow, bool>, ObservableValue<bool>>
		{
			private readonly UITerrestrialObservationTableBuilder outerInstance;

			private int columnIndexOutlier;

			public CallbackAnonymousInnerClass2(UITerrestrialObservationTableBuilder outerInstance, int columnIndexOutlier)
			{
				this.outerInstance = outerInstance;
				this.columnIndexOutlier = columnIndexOutlier;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<TerrestrialObservationRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isSignificant());
				booleanProp.addListener(significantChangeListener);
				return booleanProp;
			}
		}

		internal override void setValue(TerrestrialObservationRow rowData, int columnIndex, object oldValue, object newValue)
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
					rowData.StartPointName = oldValue == null ? null : oldValue.ToString().Trim();
				}
				break;
			case 3:
				if (newValue != null && newValue is Double)
				{
					rowData.InstrumentHeight = (double?)newValue;
					valid = true;
				}
				else
				{
					rowData.InstrumentHeight = 0.0;
				}
				break;
			case 4:
				if (newValue != null && newValue is Double)
				{
					rowData.ReflectorHeight = (double?)newValue;
					valid = true;
				}
				else
				{
					rowData.InstrumentHeight = 0.0;
				}
				break;
			case 5:
				if (newValue != null && newValue is Double)
				{
					rowData.ValueApriori = (double?)newValue;
					valid = true;
				}
				else
				{
					rowData.ValueApriori = oldValue != null && oldValue is double? ? (double?)oldValue : null;
				}
				break;
			case 6:
				rowData.SigmaApriori = newValue == null ? null : newValue is double? && (double?)newValue > 0 ? (double?)newValue : null;
				valid = true;
				break;
			case 7:
				rowData.DistanceApriori = newValue == null ? null : newValue is double? && (double?)newValue > 0 ? (double?)newValue : null;
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

		private bool isComplete(TerrestrialObservationRow row)
		{
			return !string.ReferenceEquals(row.StartPointName, null) && !row.StartPointName.Trim().Length == 0 && !string.ReferenceEquals(row.EndPointName, null) && !row.EndPointName.Trim().Length == 0 && !row.StartPointName.Equals(row.EndPointName) && row.InstrumentHeight != null && row.ReflectorHeight != null && row.ValueApriori != null;
		}

		public override TerrestrialObservationRow EmptyRow
		{
			get
			{
				TerrestrialObservationRow emptyRow = new TerrestrialObservationRow();
				// use the start point name of an existing row as default value, if observations are set of directions
				if (this.type == ObservationType.DIRECTION && !this.table.getItems().isEmpty())
				{
					IList<TerrestrialObservationRow> items = this.table.getItems();
					string startPointName = null;
					foreach (TerrestrialObservationRow observationRow in items)
					{
						if (!this.isComplete(observationRow))
						{
							continue;
						}
    
						if (string.ReferenceEquals(startPointName, null))
						{
							startPointName = observationRow.StartPointName;
							break;
						}
					}
					if (!string.ReferenceEquals(startPointName, null))
					{
						emptyRow.StartPointName = startPointName;
					}
				}
				return emptyRow;
			}
		}

		internal override void enableDragSupport()
		{
			this.table.setOnDragDetected(new EventHandlerAnonymousInnerClass(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<MouseEvent>
		{
			private readonly UITerrestrialObservationTableBuilder outerInstance;

			public EventHandlerAnonymousInnerClass(UITerrestrialObservationTableBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void handle(MouseEvent @event)
			{
				IList<TerrestrialObservationRow> selectedRows = new List<TerrestrialObservationRow>(outerInstance.table.getSelectionModel().getSelectedItems());
				if (selectedRows != null && selectedRows.Count > 0)
				{

					IList<TerrestrialObservationRowDnD> rowsDnD = new List<TerrestrialObservationRowDnD>(selectedRows.Count);
					foreach (TerrestrialObservationRow selectedRow in selectedRows)
					{
						TerrestrialObservationRowDnD rowDnD = null;
						if (outerInstance.isComplete(selectedRow) && (rowDnD = TerrestrialObservationRowDnD.fromTerrestrialObservationRow(selectedRow)) != null)
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
						content.put(EditableMenuCheckBoxTreeCell.TERRESTRIAL_OBSERVATION_ROWS_DATA_FORMAT, rowsDnD);

						db.setContent(content);
					}
				}
				@event.consume();
			}
		}

		internal override void duplicateRows()
		{
			IList<TerrestrialObservationRow> selectedRows = new List<TerrestrialObservationRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<TerrestrialObservationRow> clonedRows = new List<TerrestrialObservationRow>(selectedRows.Count);
			foreach (TerrestrialObservationRow row in selectedRows)
			{
				TerrestrialObservationRow clonedRow = TerrestrialObservationRow.cloneRowApriori(row);
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
				foreach (TerrestrialObservationRow clonedRow in clonedRows)
				{
					this.table.getSelectionModel().select(clonedRow);
				}
				this.table.scrollTo(clonedRows[0]);
			}
		}

		internal override void removeRows()
		{
			IList<TerrestrialObservationRow> selectedRows = new List<TerrestrialObservationRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<TerrestrialObservationRow> removedRows = new List<TerrestrialObservationRow>(selectedRows.Count);
			foreach (TerrestrialObservationRow row in selectedRows)
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

			IList<TerrestrialObservationRow> selectedRows = new List<TerrestrialObservationRow>(this.table.getSelectionModel().getSelectedItems());

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
				foreach (TerrestrialObservationRow row in selectedRows)
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
			IList<TerrestrialObservationRow> rows = this.table.getItems();

			string exportFormatString = "%15s \t";
			//String exportFormatDouble = "%+15.6f \t";
			string exportFormatDouble = "%20s \t";

			PrintWriter writer = null;

			try
			{
				writer = new PrintWriter(new StreamWriter(file));

				foreach (TerrestrialObservationRow row in rows)
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

					double? value = aprioriValues ? row.ValueApriori : row.ValueAposteriori;
					double? distance = aprioriValues ? row.DistanceApriori : null;
					double? sigma = aprioriValues ? row.SigmaApriori : row.SigmaAposteriori;
					double? ih = row.InstrumentHeight;
					double? th = row.ReflectorHeight;

					if (value == null || (!aprioriValues && sigma == null))
					{
						continue;
					}

					string observationValue, sigmaValue, distanceValue;

					string instrumentHeight = ih != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(ih.Value, false)) : "";
					string reflectorHeight = th != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(th.Value, false)) : "";

					if (aprioriValues)
					{
						if (sigma != null && sigma <= 0)
						{
							sigma = null;
						}
						if (sigma != null && sigma > 0)
						{
							distance = null;
						}
						if (distance != null && distance <= 0)
						{
							distance = null;
						}
					}

					switch (this.type.innerEnumValue)
					{
					case ObservationType.InnerEnum.DIRECTION:
					case ObservationType.InnerEnum.ZENITH_ANGLE:
						observationValue = value != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toAngleFormat(value.Value, false)) : "";
						sigmaValue = sigma != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toAngleFormat(sigma.Value, false)) : "";
						break;

					default:
						observationValue = value != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(value.Value, false)) : "";
						sigmaValue = sigma != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigma.Value, false)) : "";
						break;

					}
					distanceValue = distance != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(distance.Value, false)) : "";

					writer.println(String.format(exportFormatString, startPointName) + String.format(exportFormatString, endPointName) + instrumentHeight + reflectorHeight + observationValue + sigmaValue + distanceValue);

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

		internal override void highlightTableRow(TableRow<TerrestrialObservationRow> row)
		{
			if (row == null)
			{
				return;
			}

			TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
			TableRowHighlightType tableRowHighlightType = tableRowHighlight.SelectedTableRowHighlightType;
			double leftBoundary = tableRowHighlight.getLeftBoundary(tableRowHighlightType);
			double rightBoundary = tableRowHighlight.getRightBoundary(tableRowHighlightType);

			TerrestrialObservationRow item = row.getItem();

			if (!row.isSelected() && item != null)
			{
				switch (tableRowHighlightType.innerEnumValue)
				{
				case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
					this.setTableRowHighlight(row, item.Significant ? TableRowHighlightRangeType.INADEQUATE : TableRowHighlightRangeType.EXCELLENT);
					break;

				case TableRowHighlightType.InnerEnum.GROSS_ERROR:
					double? grossError = item.GrossError;
					double? mtb = item.MaximumTolerableBias;
					double? mdb = item.MinimalDetectableBias;

					double dMTB = Double.NaN;
					double dMDB = Double.NaN;

					if (grossError != null && mtb != null && mdb != null)
					{
						dMTB = Math.Abs(grossError) - Math.Abs(mtb);
						dMDB = Math.Abs(mdb) - Math.Abs(grossError);

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

					break;

				case TableRowHighlightType.InnerEnum.REDUNDANCY:
					double? redundancy = item.Redundancy;
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
					double? influenceOnPosition = item.InfluenceOnPointPosition;
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

	//	private static void setSortOrder(TableView<TerrestrialObservationRow> table, List<ColumnContentType> columnTypes) {
	//		ObservableList<TableColumn<TerrestrialObservationRow, ?>> sortOrder = FXCollections.observableArrayList();
	//		
	//		ObservableList<TableColumn<TerrestrialObservationRow, ?>> columns = table.getColumns();
	//		int idx = 0;
	//		for (ColumnContentType columnType : columnTypes) {
	//			for (TableColumn<TerrestrialObservationRow, ?> column : columns) {
	//				if (column instanceof ContentColumn) {
	//					ColumnContentType currentColumnType = ((ContentColumn<TerrestrialObservationRow,?>)column).getColumnProperty().getColumnContentType();
	//					if (currentColumnType == columnType) {
	//						sortOrder.add(column);
	//						((ContentColumn<TerrestrialObservationRow,?>)column).getColumnProperty().setSortType(idx++);
	//						break;
	//					}
	//				}
	//			}
	//		}
	//		
	//		table.getSortOrder().setAll(sortOrder);
	//	}
	}

}