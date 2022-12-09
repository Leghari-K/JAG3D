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

	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using CongruenceAnalysisRowDnD = org.applied_geodesy.jag3d.ui.dnd.CongruenceAnalysisRowDnD;
	using ColumnContentType = org.applied_geodesy.jag3d.ui.table.column.ColumnContentType;
	using TableContentType = org.applied_geodesy.jag3d.ui.table.column.TableContentType;
	using CongruenceAnalysisRow = org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow;
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightRangeType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using CongruenceAnalysisTreeItemValue = org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue;
	using EditableMenuCheckBoxTreeCell = org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell;
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

	public class UICongruenceAnalysisTableBuilder : UIEditableTableBuilder<CongruenceAnalysisRow>
	{
		private CongruenceAnalysisTreeItemValue congruenceAnalysisItemValue;
		private int dimension;
		private IDictionary<int, TableView<CongruenceAnalysisRow>> tables = new Dictionary<int, TableView<CongruenceAnalysisRow>>();

		private static UICongruenceAnalysisTableBuilder tableBuilder = new UICongruenceAnalysisTableBuilder();
		private UICongruenceAnalysisTableBuilder() : base()
		{
		}

		public static UICongruenceAnalysisTableBuilder Instance
		{
			get
			{
				return tableBuilder;
			}
		}

		public virtual TableView<CongruenceAnalysisRow> getTable(CongruenceAnalysisTreeItemValue congruenceAnalysisItemValue)
		{
			this.congruenceAnalysisItemValue = congruenceAnalysisItemValue;
			this.dimension = congruenceAnalysisItemValue.Dimension;
			this.init();
			return this.table;
		}

		private TableContentType TableContentType
		{
			get
			{
				switch (this.dimension)
				{
				case 1:
					return TableContentType.CONGRUENCE_ANALYSIS_1D;
				case 2:
					return TableContentType.CONGRUENCE_ANALYSIS_2D;
				case 3:
					return TableContentType.CONGRUENCE_ANALYSIS_3D;
				}
				return TableContentType.UNSPECIFIC;
			}
		}

		private void init()
		{
			if (this.tables.ContainsKey(this.dimension))
			{
				this.table = this.tables[this.dimension];
				return;
			}
			TableColumn<CongruenceAnalysisRow, bool> booleanColumn = null;
			TableColumn<CongruenceAnalysisRow, string> stringColumn = null;
			TableColumn<CongruenceAnalysisRow, double> doubleColumn = null;

			TableContentType tableContentType = this.TableContentType;

			TableView<CongruenceAnalysisRow> table = this.createTable();
			///////////////// A-PRIORI VALUES /////////////////////////////
			// Enable/Disable
			int columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexEnable = columnIndex;
			int columnIndexEnable = columnIndex;
			string labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.enable.label", "Enable");
			string tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.enable.tooltip", "State of the point nexus");
			CellValueType cellValueType = CellValueType.BOOLEAN;
			ColumnContentType columnContentType = ColumnContentType.ENABLE;
			ColumnTooltipHeader header = new ColumnTooltipHeader(cellValueType,labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, CongruenceAnalysisRow::enableProperty, getBooleanCallback(), ColumnType.VISIBLE, columnIndex, true, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass(this, columnIndexEnable));
			table.getColumns().add(booleanColumn);

			// POINT-ID REF
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.reference.epoch.name.label", "Point-Id (Reference epoch)");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.reference.epoch.name.tooltip", "Id of the point w.r.t. reference epoch");
			cellValueType = CellValueType.STRING;
			columnContentType = ColumnContentType.START_POINT_NAME;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, CongruenceAnalysisRow::nameInReferenceEpochProperty, getStringCallback(), ColumnType.VISIBLE, columnIndex, true, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// POINT-ID CONTR
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.control.epoch.name.label", "Point-Id (Control epoch)");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.control.epoch.name.tooltip", "Id of the point w.r.t. control epoch");
			cellValueType = CellValueType.STRING;
			columnContentType = ColumnContentType.END_POINT_NAME;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, CongruenceAnalysisRow::nameInControlEpochProperty, getStringCallback(), ColumnType.VISIBLE, columnIndex, true, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			///////////////// A-POSTERIORI VALUES /////////////////////////////
			// A-posteriori Components

			// Y-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.y.label", "y");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.y.tooltip", "A-posteriori y-component of displacement vector");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Y_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::yAposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// X-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.x.label", "x");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.x.tooltip", "A-posteriori x-component of displacement vector");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_X_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::xAposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Z-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.z.label", "z");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.z.tooltip", "A-posteriori z-component of displacement vector");
			cellValueType = CellValueType.LENGTH;
			columnContentType = ColumnContentType.VALUE_Z_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::zAposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 2 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);


			// A-posteriori Uncertainties
			// Y-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.sigma.y.label", "\u03C3y");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.sigma.y.tooltip", "A-posteriori uncertainty of y-component of displacement vector");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Y_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::sigmaYaposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// X-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.sigma.x.label", "\u03C3x");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.sigma.x.tooltip", "A-posteriori uncertainty of x-component of displacement vector");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_X_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::sigmaXaposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Z-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.sigma.z.label", "\u03C3z");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.sigma.z.tooltip", "A-posteriori uncertainty of z-component of displacement vector");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.UNCERTAINTY_Z_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::sigmaZaposterioriProperty, getDoubleCallback(cellValueType), this.dimension != 2 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Confidence
			// A
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxis.major.label", "a");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxis.major.tooltip", "Major semi axis");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.CONFIDENCE_A;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::confidenceAProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT_CONGRUENCE, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// B
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxis.middle.label", "b");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxis.middle.tooltip", "Middle semi axis");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.CONFIDENCE_B;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::confidenceBProperty, getDoubleCallback(cellValueType), this.dimension == 3 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// C
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxis.minor.label", "c");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxis.minor.tooltip", "Minor semi axis");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			columnContentType = ColumnContentType.CONFIDENCE_C;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::confidenceCProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// alpha
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxisrotation.alpha.label", "\u03B1");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxisrotation.alpha.tooltip", "Rotation angle of confidence region");
			cellValueType = CellValueType.ANGLE;
			columnContentType = ColumnContentType.CONFIDENCE_ALPHA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::confidenceAlphaProperty, getDoubleCallback(cellValueType), this.dimension == 3 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// beta
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxisrotation.beta.label", "\u03B2");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxisrotation.beta.tooltip", "Rotation angle of confidence region");
			cellValueType = CellValueType.ANGLE;
			columnContentType = ColumnContentType.CONFIDENCE_BETA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::confidenceBetaProperty, getDoubleCallback(cellValueType), this.dimension == 3 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// gamma
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxisrotation.gamma.label", "\u03B3");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.semiaxisrotation.gamma.tooltip", "Rotation angle of confidence region");
			cellValueType = CellValueType.ANGLE;
			columnContentType = ColumnContentType.CONFIDENCE_GAMMA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::confidenceGammaProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Gross-Error
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.grosserror.y.label", "\u2207y");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.grosserror.y.tooltip", "Gross-error in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::grossErrorYProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.grosserror.x.label", "\u2207x");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.grosserror.x.tooltip", "Gross-error in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::grossErrorXProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.grosserror.z.label", "\u2207z");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.grosserror.z.tooltip", "Gross-error in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.RESIDUAL_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::grossErrorZProperty, getDoubleCallback(cellValueType), this.dimension != 2 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);


			// MDB
			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.minimaldetectablebias.y.label", "\u2207y(\u03BB)");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.minimaldetectablebias.y.tooltip", "Minimal detectable bias in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_Y;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::minimalDetectableBiasYProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.minimaldetectablebias.x.label", "\u2207x(\u03BB)");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.minimaldetectablebias.x.tooltip", "Minimal detectable bias in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_X;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::minimalDetectableBiasXProperty, getDoubleCallback(cellValueType), this.dimension != 1 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.minimaldetectablebias.z.label", "\u2207z(\u03BB)");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.minimaldetectablebias.z.tooltip", "Minimal detectable bias in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS_Z;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.getFormatterOptions().get(cellValueType).getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::minimalDetectableBiasZProperty, getDoubleCallback(cellValueType), this.dimension != 2 ? ColumnType.APOSTERIORI_POINT_CONGRUENCE : ColumnType.HIDDEN, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);


			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.pvalue.apriori.label", "log(Pprio)");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.pvalue.apriori.tooltip", "A-priori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::pValueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT_CONGRUENCE, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.pvalue.aposteriori.label", "log(Ppost)");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.pvalue.aposteriori.tooltip", "A-posteriori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::pValueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT_CONGRUENCE, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Tprio
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.teststatistic.apriori.label", "Tprio");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.teststatistic.apriori.tooltip", "A-priori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::testStatisticAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT_CONGRUENCE, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Tpost
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.teststatistic.aposteriori.label", "Tpost");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.teststatistic.aposteriori.tooltip", "A-posteriori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, CongruenceAnalysisRow::testStatisticAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT_CONGRUENCE, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Decision of test statistic
			columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexOutlier = columnIndex;
			int columnIndexOutlier = columnIndex;
			labelText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.testdecision.label", "Significant");
			tooltipText = i18n.getString("UICongruenceAnalysisTableBuilder.tableheader.testdecision.tooltip", "Checked, if null-hypothesis is rejected");
			cellValueType = CellValueType.BOOLEAN;
			columnContentType = ColumnContentType.SIGNIFICANT;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, CongruenceAnalysisRow::significantProperty, getBooleanCallback(), ColumnType.APOSTERIORI_POINT_CONGRUENCE, columnIndex, false, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass2(this, columnIndexOutlier));
			table.getColumns().add(booleanColumn);

			this.addContextMenu(table, this.createContextMenu(false));
			this.addDynamicRowAdder(table);
			this.addColumnOrderSequenceListeners(tableContentType, table);

			this.tables[this.dimension] = table;
			this.table = table;
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn.CellDataFeatures<CongruenceAnalysisRow, bool>, ObservableValue<bool>>
		{
			private readonly UICongruenceAnalysisTableBuilder outerInstance;

			private int columnIndexEnable;

			public CallbackAnonymousInnerClass(UICongruenceAnalysisTableBuilder outerInstance, int columnIndexEnable)
			{
				this.outerInstance = outerInstance;
				this.columnIndexEnable = columnIndexEnable;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<CongruenceAnalysisRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isEnable());
				booleanProp.addListener(enableChangeListener);
				return booleanProp;
			}
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn.CellDataFeatures<CongruenceAnalysisRow, bool>, ObservableValue<bool>>
		{
			private readonly UICongruenceAnalysisTableBuilder outerInstance;

			private int columnIndexOutlier;

			public CallbackAnonymousInnerClass2(UICongruenceAnalysisTableBuilder outerInstance, int columnIndexOutlier)
			{
				this.outerInstance = outerInstance;
				this.columnIndexOutlier = columnIndexOutlier;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<CongruenceAnalysisRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isSignificant());
				booleanProp.addListener(significantChangeListener);
				return booleanProp;
			}
		}


		public override CongruenceAnalysisRow EmptyRow
		{
			get
			{
				return new CongruenceAnalysisRow();
			}
		}

		internal override void setValue(CongruenceAnalysisRow rowData, int columnIndex, object oldValue, object newValue)
		{
			bool valid = (oldValue == null || oldValue.ToString().Trim().Length == 0) && (newValue == null || newValue.ToString().Trim().Length == 0);
			switch (columnIndex)
			{
			case 0:
				rowData.Enable = newValue != null && newValue is bool? && (bool?)newValue;
				valid = true;
				break;
			case 1:
				if (newValue != null && newValue.ToString().Trim().Length > 0 && ((string.ReferenceEquals(rowData.NameInControlEpoch, null) || !rowData.NameInControlEpoch.Equals(newValue.ToString().Trim()))))
				{
					rowData.NameInReferenceEpoch = newValue.ToString().Trim();
					valid = true;
				}
				else
				{
					rowData.NameInReferenceEpoch = oldValue == null ? null : oldValue.ToString().Trim();
				}
				break;
			case 2:
				if (newValue != null && newValue.ToString().Trim().Length > 0 && ((string.ReferenceEquals(rowData.NameInReferenceEpoch, null) || !rowData.NameInReferenceEpoch.Equals(newValue.ToString().Trim()))))
				{
					rowData.NameInControlEpoch = newValue.ToString().Trim();
					valid = true;
				}
				else
				{
					rowData.NameInControlEpoch = oldValue == null ? null : oldValue.ToString().Trim();
				}
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
					rowData.GroupId = this.congruenceAnalysisItemValue.GroupId;
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
						rowData.NameInReferenceEpoch = oldValue == null ? null : oldValue.ToString().Trim();
						break;
					case 2:
						rowData.NameInControlEpoch = oldValue == null ? null : oldValue.ToString().Trim();
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

		private bool isComplete(CongruenceAnalysisRow row)
		{
			return !string.ReferenceEquals(row.NameInReferenceEpoch, null) && row.NameInReferenceEpoch.Trim().Length > 0 && !string.ReferenceEquals(row.NameInControlEpoch, null) && row.NameInControlEpoch.Trim().Length > 0 && !row.NameInReferenceEpoch.Equals(row.NameInControlEpoch);
		}

		internal override void enableDragSupport()
		{
			this.table.setOnDragDetected(new EventHandlerAnonymousInnerClass(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<MouseEvent>
		{
			private readonly UICongruenceAnalysisTableBuilder outerInstance;

			public EventHandlerAnonymousInnerClass(UICongruenceAnalysisTableBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void handle(MouseEvent @event)
			{
				IList<CongruenceAnalysisRow> selectedRows = new List<CongruenceAnalysisRow>(table.getSelectionModel().getSelectedItems());
				if (selectedRows != null && selectedRows.Count > 0)
				{

					IList<CongruenceAnalysisRowDnD> rowsDnD = new List<CongruenceAnalysisRowDnD>(selectedRows.Count);
					foreach (CongruenceAnalysisRow selectedRow in selectedRows)
					{
						CongruenceAnalysisRowDnD rowDnD = null;
						if (outerInstance.isComplete(selectedRow) && (rowDnD = CongruenceAnalysisRowDnD.fromCongruenceAnalysisRow(selectedRow)) != null)
						{
							rowsDnD.Add(rowDnD);
						}
					}

					if (rowsDnD.Count > 0)
					{
						Dragboard db = table.startDragAndDrop(TransferMode.MOVE);
						ClipboardContent content = new ClipboardContent();
						content.put(EditableMenuCheckBoxTreeCell.TREE_ITEM_TYPE_DATA_FORMAT, outerInstance.congruenceAnalysisItemValue.ItemType);
						content.put(EditableMenuCheckBoxTreeCell.GROUP_ID_DATA_FORMAT, outerInstance.congruenceAnalysisItemValue.GroupId);
						content.put(EditableMenuCheckBoxTreeCell.DIMENSION_DATA_FORMAT, outerInstance.dimension);
						content.put(EditableMenuCheckBoxTreeCell.CONGRUENCE_ANALYSIS_ROWS_DATA_FORMAT, rowsDnD);

						db.setContent(content);
					}
				}
				@event.consume();
			}
		}

		internal override void duplicateRows()
		{
			IList<CongruenceAnalysisRow> selectedRows = new List<CongruenceAnalysisRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<CongruenceAnalysisRow> clonedRows = new List<CongruenceAnalysisRow>(selectedRows.Count);
			foreach (CongruenceAnalysisRow row in selectedRows)
			{
				CongruenceAnalysisRow clonedRow = CongruenceAnalysisRow.cloneRowApriori(row);

				if (this.isComplete(clonedRow))
				{
					try
					{
						// Generate next unique point names
						string[] names = SQLManager.Instance.getNextValidPointNexusNames(this.congruenceAnalysisItemValue.GroupId, row.NameInReferenceEpoch, row.NameInControlEpoch);
						clonedRow.NameInReferenceEpoch = names[0];
						clonedRow.NameInControlEpoch = names[1];
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
				foreach (CongruenceAnalysisRow clonedRow in clonedRows)
				{
					this.table.getSelectionModel().select(clonedRow);
				}
				this.table.scrollTo(clonedRows[0]);
			}
		}

		internal override void removeRows()
		{
			IList<CongruenceAnalysisRow> selectedRows = new List<CongruenceAnalysisRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			IList<CongruenceAnalysisRow> removedRows = new List<CongruenceAnalysisRow>(selectedRows.Count);
			foreach (CongruenceAnalysisRow row in selectedRows)
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

			IList<CongruenceAnalysisRow> selectedRows = new List<CongruenceAnalysisRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(this.congruenceAnalysisItemValue.ItemType);
			TreeItem<TreeItemValue> newTreeItem = UITreeBuilder.Instance.addItem(parentType, false);
			try
			{
				SQLManager.Instance.saveGroup((CongruenceAnalysisTreeItemValue)newTreeItem.getValue());
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
				int groupId = ((CongruenceAnalysisTreeItemValue)newTreeItem.getValue()).GroupId;
				foreach (CongruenceAnalysisRow row in selectedRows)
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
			IList<CongruenceAnalysisRow> rows = this.table.getItems();

			string exportFormatString = "%15s \t";
			//String exportFormatDouble = "%+15.6f \t";
			string exportFormatDouble = "%20s \t";

			PrintWriter writer = null;

			try
			{
				writer = new PrintWriter(new StreamWriter(file));

				foreach (CongruenceAnalysisRow row in rows)
				{
					if (!row.Enable)
					{
						continue;
					}

					string nameInReferenceEpoch = row.NameInReferenceEpoch;
					string nameInControlEpoch = row.NameInControlEpoch;

					if (string.ReferenceEquals(nameInReferenceEpoch, null) || nameInReferenceEpoch.Trim().Length == 0 || string.ReferenceEquals(nameInControlEpoch, null) || nameInControlEpoch.Trim().Length == 0)
					{
						continue;
					}

					double? y = aprioriValues ? null : row.YAposteriori;
					double? x = aprioriValues ? null : row.XAposteriori;
					double? z = aprioriValues ? null : row.ZAposteriori;

					if (!aprioriValues && this.dimension != 2 && z == null)
					{
						continue;
					}

					if (!aprioriValues && this.dimension != 1 && (y == null || x == null))
					{
						continue;
					}

					double? sigmaY = aprioriValues ? null : row.SigmaYaposteriori;
					double? sigmaX = aprioriValues ? null : row.SigmaXaposteriori;
					double? sigmaZ = aprioriValues ? null : row.SigmaZaposteriori;

					if (!aprioriValues && (sigmaY == null || sigmaX == null || sigmaZ == null))
					{
						continue;
					}

					string yValue = this.dimension != 1 && y != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(y, false)) : "";
					string xValue = this.dimension != 1 && x != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(x, false)) : "";
					string zValue = this.dimension != 2 && z != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(z, false)) : "";

					string sigmaYvalue = this.dimension != 1 && sigmaY != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigmaY, false)) : "";
					string sigmaXvalue = this.dimension != 1 && sigmaX != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigmaX, false)) : "";
					string sigmaZvalue = this.dimension != 2 && sigmaZ != null ? String.format(Locale.ENGLISH, exportFormatDouble, options.toLengthFormat(sigmaZ, false)) : "";

					writer.println(String.format(exportFormatString, nameInReferenceEpoch) + String.format(exportFormatString, nameInControlEpoch) + yValue + xValue + zValue + sigmaYvalue + sigmaXvalue + sigmaZvalue);
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

		internal override void highlightTableRow(TableRow<CongruenceAnalysisRow> row)
		{
			if (row == null)
			{
				return;
			}

			TableRowHighlight tableRowHighlight = TableRowHighlight.Instance;
			TableRowHighlightType tableRowHighlightType = tableRowHighlight.SelectedTableRowHighlightType;
			double leftBoundary = tableRowHighlight.getLeftBoundary(tableRowHighlightType);
			double rightBoundary = tableRowHighlight.getRightBoundary(tableRowHighlightType);

			CongruenceAnalysisRow item = row.getItem();

			if (!row.isSelected() && item != null)
			{
				switch (tableRowHighlightType.innerEnumValue)
				{
				case TableRowHighlightType.InnerEnum.TEST_STATISTIC:
					this.setTableRowHighlight(row, item.Significant ? TableRowHighlightRangeType.INADEQUATE : TableRowHighlightRangeType.EXCELLENT);
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

				default:
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