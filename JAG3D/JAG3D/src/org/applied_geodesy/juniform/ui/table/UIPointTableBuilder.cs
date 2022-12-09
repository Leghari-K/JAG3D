using System;
using System.Collections.Generic;

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

namespace org.applied_geodesy.juniform.ui.table
{

	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using FeatureChangeListener = org.applied_geodesy.adjustment.geometry.FeatureChangeListener;
	using FeatureEvent = org.applied_geodesy.adjustment.geometry.FeatureEvent;
	using FeatureType = org.applied_geodesy.adjustment.geometry.FeatureType;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using FeatureEventType = org.applied_geodesy.adjustment.geometry.FeatureEvent.FeatureEventType;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using MatrixDialog = org.applied_geodesy.juniform.ui.dialog.MatrixDialog;
	using AbsoluteValueComparator = org.applied_geodesy.ui.table.AbsoluteValueComparator;
	using ColumnTooltipHeader = org.applied_geodesy.ui.table.ColumnTooltipHeader;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using org.applied_geodesy.ui.table;
	using CellValueType = org.applied_geodesy.util.CellValueType;

	using Platform = javafx.application.Platform;
	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Button = javafx.scene.control.Button;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using TableCell = javafx.scene.control.TableCell;
	using TableColumn = javafx.scene.control.TableColumn;
	using TableView = javafx.scene.control.TableView;
	using Tooltip = javafx.scene.control.Tooltip;
	using CellDataFeatures = javafx.scene.control.TableColumn.CellDataFeatures;
	using Callback = javafx.util.Callback;
	using Matrix = no.uib.cipr.matrix.Matrix;

	public class UIPointTableBuilder : UIEditableTableBuilder<FeaturePoint>, FeatureChangeListener
	{
		private FeatureType featureType = FeatureType.CURVE;
		private IDictionary<FeatureType, TableView<FeaturePoint>> tables = new Dictionary<FeatureType, TableView<FeaturePoint>>(2);

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

		public virtual FeatureType FeatureType
		{
			set
			{
				if (this.featureType != value)
				{
					this.table.getItems().clear();
					this.table.refresh();
					this.featureType = value;
				}
			}
		}

		public override TableView<FeaturePoint> Table
		{
			get
			{
				this.init();
				return this.table;
			}
		}

		private void init()
		{
			if (this.tables.ContainsKey(this.featureType))
			{
				this.table = this.tables[this.featureType];
				return;
			}
			TableColumn<FeaturePoint, bool> booleanColumn = null;
			TableColumn<FeaturePoint, string> stringColumn = null;
			TableColumn<FeaturePoint, double> doubleColumn = null;
			TableColumn<FeaturePoint, Matrix> matrixColumn = null;

			TableView<FeaturePoint> table = this.createTable();
			///////////////// A-PRIORI VALUES /////////////////////////////
			// Enable/Disable
			int columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexEnable = columnIndex;
			int columnIndexEnable = columnIndex;
			string labelText = i18n.getString("UIPointTableBuilder.tableheader.enable.label", "Enable");
			string tooltipText = i18n.getString("UIPointTableBuilder.tableheader.enable.tooltip", "State of the point");
			CellValueType cellValueType = CellValueType.BOOLEAN;
			ColumnTooltipHeader header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(header, FeaturePoint::enableProperty, BooleanCallback, ColumnType.VISIBLE, columnIndex, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass(this, columnIndexEnable));
			table.getColumns().add(booleanColumn);

			// Point-ID
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.station.name.label", "Point-Id");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.station.name.tooltip", "Id of the point");
			cellValueType = CellValueType.STRING;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(header, FeaturePoint::nameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// A-priori Components
			// X0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.x0.label", "x0");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.x0.tooltip", "A-priori x-component of the point");
			cellValueType = CellValueType.LENGTH;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::x0Property, getDoubleCallback(cellValueType), ColumnType.APRIORI_POINT, columnIndex, true);
			table.getColumns().add(doubleColumn);

			// Y0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.y0.label", "y0");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.y0.tooltip", "A-priori y-component of the point");
			cellValueType = CellValueType.LENGTH;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::y0Property, getDoubleCallback(cellValueType), ColumnType.APRIORI_POINT, columnIndex, true);
			table.getColumns().add(doubleColumn);

			// Z0-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.z0.label", "z0");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.z0.tooltip", "A-priori z-component of the point");
			cellValueType = CellValueType.LENGTH;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::z0Property, getDoubleCallback(cellValueType), this.featureType == FeatureType.SURFACE ? ColumnType.APRIORI_POINT : ColumnType.HIDDEN, columnIndex, true);
			table.getColumns().add(doubleColumn);


			// Covariance
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.covariance.label", "Uncertainties");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.covariance.tooltip", "A-priori variance-covariance matrix");
			cellValueType = CellValueType.STATISTIC;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			matrixColumn = this.getColumn<Matrix>(header, FeaturePoint::dispersionAprioriProperty, MatrixCallback, ColumnType.APRIORI_POINT, columnIndex, true);
			table.getColumns().add(matrixColumn);


			///////////////// A-POSTERIORI VALUES /////////////////////////////
			// A-posteriori Components

			// X-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.x.label", "x");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.x.tooltip", "A-posteriori x-component of the point");
			cellValueType = CellValueType.LENGTH;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::xProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			table.getColumns().add(doubleColumn);

			// Y-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.y.label", "y");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.y.tooltip", "A-posteriori y-component of the point");
			cellValueType = CellValueType.LENGTH;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::yProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			table.getColumns().add(doubleColumn);

			// Z-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.z.label", "z");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.z.tooltip", "A-posteriori z-component of the point");
			cellValueType = CellValueType.LENGTH;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::zProperty, getDoubleCallback(cellValueType), this.featureType == FeatureType.SURFACE ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false);
			table.getColumns().add(doubleColumn);

			// A-posteriori Uncertainties
			// X-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.uncertainty.x.label", "\u03C3x");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.uncertainty.x.tooltip", "A-posteriori uncertainty of x-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::uncertaintyXProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Y-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.uncertainty.y.label", "\u03C3y");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.uncertainty.y.tooltip", "A-posteriori uncertainty of y-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::uncertaintyYProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Z-Comp.
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.uncertainty.z.label", "\u03C3z");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.uncertainty.z.tooltip", "A-posteriori uncertainty of z-component");
			cellValueType = CellValueType.LENGTH_UNCERTAINTY;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::uncertaintyZProperty, getDoubleCallback(cellValueType), this.featureType == FeatureType.SURFACE ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Redundancy
			// rx
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.x.label", "rx");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.x.tooltip", "Redundancy of x-component");
			cellValueType = CellValueType.PERCENTAGE;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::redundancyXProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// ry
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.y.label", "ry");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.y.tooltip", "Redundancy of y-component");
			cellValueType = CellValueType.PERCENTAGE;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::redundancyYProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// rz
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.z.label", "rz");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.redundancy.z.tooltip", "Redundancy of z-component");
			cellValueType = CellValueType.PERCENTAGE;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::redundancyZProperty, getDoubleCallback(cellValueType), this.featureType == FeatureType.SURFACE ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Residual 
			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.residual.x.label", "\u03B5x");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.residual.x.tooltip", "Residual of x-component");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::residualXProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.residual.y.label", "\u03B5y");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.residual.y.tooltip", "Residual of y-component");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::residualYProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.residual.z.label", "\u03B5z");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.residual.z.tooltip", "Residual of z-component");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::residualZProperty, getDoubleCallback(cellValueType), this.featureType == FeatureType.SURFACE ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Gross-Error
			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.x.label", "\u2207x");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.x.tooltip", "Gross-error in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::grossErrorXProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.y.label", "\u2207y");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.y.tooltip", "Gross-error in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::grossErrorYProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.z.label", "\u2207z");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.grosserror.z.tooltip", "Gross-error in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::grossErrorZProperty, getDoubleCallback(cellValueType), this.featureType == FeatureType.SURFACE ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// MTB
			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.x.label", "\u2207x(1)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.x.tooltip", "Maximum tolerable bias in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::maximumTolerableBiasXProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.y.label", "\u2207y(1)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.y.tooltip", "Maximum tolerable bias in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::maximumTolerableBiasYProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.z.label", "\u2207z(1)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.maximumtolerablebias.z.tooltip", "Maximum tolerable bias in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::maximumTolerableBiasZProperty, getDoubleCallback(cellValueType), this.featureType == FeatureType.SURFACE ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// MDB
			// x-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.x.label", "\u2207x(\u03BB)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.x.tooltip", "Minimal detectable bias in x");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::minimalDetectableBiasXProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// y-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.y.label", "\u2207y(\u03BB)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.y.tooltip", "Minimal detectable bias in y");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::minimalDetectableBiasYProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// z-Comp
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.z.label", "\u2207z(\u03BB)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.minimaldetectablebias.z.tooltip", "Minimal detectable bias in z");
			cellValueType = CellValueType.LENGTH_RESIDUAL;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::minimalDetectableBiasZProperty, getDoubleCallback(cellValueType), this.featureType == FeatureType.SURFACE ? ColumnType.APOSTERIORI_POINT : ColumnType.HIDDEN, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.pvalue.apriori.label", "log(Pprio)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.pvalue.apriori.tooltip", "A-priori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::pValueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// p-Value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.pvalue.aposteriori.label", "log(Ppost)");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.pvalue.aposteriori.tooltip", "A-posteriori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::pValueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Tprio
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.teststatistic.apriori.label", "Tprio");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.teststatistic.apriori.tooltip", "A-priori test statistic");
			cellValueType = CellValueType.STATISTIC;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::testStatisticAprioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Tpost
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIPointTableBuilder.tableheader.teststatistic.aposteriori.label", "Tpost");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.teststatistic.aposteriori.tooltip", "A-posteriori test statistic");
			cellValueType = CellValueType.STATISTIC;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, FeaturePoint::testStatisticAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.APOSTERIORI_POINT, columnIndex, false);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Decision of test statistic
			columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexSignificant = columnIndex;
			int columnIndexSignificant = columnIndex;
			labelText = i18n.getString("UIPointTableBuilder.tableheader.testdecision.label", "Significant");
			tooltipText = i18n.getString("UIPointTableBuilder.tableheader.testdecision.tooltip", "Checked, if null-hypothesis is rejected");
			cellValueType = CellValueType.BOOLEAN;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(header, FeaturePoint::significantProperty, BooleanCallback, ColumnType.APOSTERIORI_POINT, columnIndex, false);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass2(this, columnIndexSignificant));
			table.getColumns().add(booleanColumn);

			this.tables[this.featureType] = table;
			this.table = table;
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn.CellDataFeatures<FeaturePoint, bool>, ObservableValue<bool>>
		{
			private readonly UIPointTableBuilder outerInstance;

			private int columnIndexEnable;

			public CallbackAnonymousInnerClass(UIPointTableBuilder outerInstance, int columnIndexEnable)
			{
				this.outerInstance = outerInstance;
				this.columnIndexEnable = columnIndexEnable;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<FeaturePoint, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				TableCellChangeListener<bool> enableChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexEnable, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isEnable());
				booleanProp.addListener(enableChangeListener);
				return booleanProp;
			}
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn.CellDataFeatures<FeaturePoint, bool>, ObservableValue<bool>>
		{
			private readonly UIPointTableBuilder outerInstance;

			private int columnIndexSignificant;

			public CallbackAnonymousInnerClass2(UIPointTableBuilder outerInstance, int columnIndexSignificant)
			{
				this.outerInstance = outerInstance;
				this.columnIndexSignificant = columnIndexSignificant;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<FeaturePoint, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexSignificant, param.getValue());
				TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexSignificant, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isSignificant());
				booleanProp.addListener(significantChangeListener);
				return booleanProp;
			}
		}

		internal override void setValue(FeaturePoint featurePoint, int columnIndex, object oldValue, object newValue)
		{
			bool valid = (oldValue == null || oldValue.ToString().Trim().Length == 0) && (newValue == null || newValue.ToString().Trim().Length == 0);
			switch (columnIndex)
			{
			case 0:
				featurePoint.Enable = newValue != null && newValue is bool? && (bool?)newValue;
				valid = true;
				break;
			case 1:
				if (newValue != null && newValue.ToString().Trim().Length > 0)
				{
					featurePoint.Name = newValue.ToString().Trim();
					valid = true;
				}
				else
				{
					featurePoint.Name = oldValue == null ? null : oldValue.ToString().Trim();
				}
				break;
			case 2:
				if (newValue != null && newValue is Double)
				{
					featurePoint.X0 = ((double?)newValue).Value;
					valid = true;
				}
				else
				{
					featurePoint.X0 = (oldValue != null && oldValue is double? ? (double?)oldValue : null).Value;
				}
				break;
			case 3:
				if (newValue != null && newValue is Double)
				{
					featurePoint.Y0 = ((double?)newValue).Value;
					valid = true;
				}
				else
				{
					featurePoint.Y0 = (oldValue != null && oldValue is double? ? (double?)oldValue : null).Value;
				}
				break;
			case 5:
				if (newValue != null && newValue is Double)
				{
					featurePoint.Z0 = ((double?)newValue).Value;
					valid = true;
				}
				else
				{
					featurePoint.Z0 = (oldValue != null && oldValue is double? ? (double?)oldValue : null).Value;
				}
				break;
			default:
				Console.Error.WriteLine(this.GetType().Name + " : Editable column exceed " + columnIndex);
				valid = false;
				break;
			}

			if (!valid)
			{
				Platform.runLater(() =>
				{
				table.refresh();
				table.requestFocus();
				table.getSelectionModel().clearSelection();
				table.getSelectionModel().select(featurePoint);
				});
			}
		}

		private Feature PointsToFeature
		{
			set
			{
				foreach (FeaturePoint featurePoint in this.table.getItems())
				{
					featurePoint.clear();
				}
				if (value != null)
				{
					foreach (GeometricPrimitive geometricPrimitive in value)
					{
						geometricPrimitive.FeaturePoints.setAll(this.table.getItems());
					}
				}
			}
		}

		public virtual void featureChanged(FeatureEvent evt)
		{
			if (evt.EventType == FeatureEvent.FeatureEventType.FEATURE_ADDED)
			{
				this.PointsToFeature = evt.Source;
			}
			else if (evt.EventType == FeatureEvent.FeatureEventType.FEATURE_REMOVED)
			{
				this.PointsToFeature = null;
			}
		}

		private static Callback<TableColumn<FeaturePoint, Matrix>, TableCell<FeaturePoint, Matrix>> MatrixCallback
		{
			get
			{
				return new CallbackAnonymousInnerClass3();
			}
		}

		private class CallbackAnonymousInnerClass3 : Callback<TableColumn<FeaturePoint, Matrix>, TableCell<FeaturePoint, Matrix>>
		{
			public override TableCell<FeaturePoint, Matrix> call(TableColumn<FeaturePoint, Matrix> cell)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javafx.scene.control.TableCell<org.applied_geodesy.adjustment.geometry.point.FeaturePoint, no.uib.cipr.matrix.Matrix> tableCell = new javafx.scene.control.TableCell<org.applied_geodesy.adjustment.geometry.point.FeaturePoint, no.uib.cipr.matrix.Matrix>()
				TableCell<FeaturePoint, Matrix> tableCell = new TableCellAnonymousInnerClass(this);
				tableCell.setAlignment(Pos.CENTER);
				return tableCell;
			}

			private class TableCellAnonymousInnerClass : TableCell<FeaturePoint, Matrix>
			{
				private readonly CallbackAnonymousInnerClass3 outerInstance;

				public TableCellAnonymousInnerClass(CallbackAnonymousInnerClass3 outerInstance)
				{
					this.outerInstance = outerInstance;
					button = createButton(i18n.getString("UIPointTableBuilder.dispersion.button.label", "Dispersion"),"");
				}


				internal readonly Button button;

				public override void updateItem(Matrix item, bool empty)
				{
					base.updateItem(item, empty);
					if (empty)
					{
						setGraphic(null);
						setText(null);
					}
					else
					{
						this.button.getTooltip().setText(String.format(Locale.ENGLISH, i18n.getString("UIPointTableBuilder.dispersion.button.tooltip", "Show dispersion of point %s"), getTableView().getItems().get(getIndex()).getName()));
						this.button.setOnAction(new EventHandlerAnonymousInnerClass(this));
						this.setGraphic(this.button);
						this.setText(null);
					}
				}

				private class EventHandlerAnonymousInnerClass : EventHandler<ActionEvent>
				{
					private readonly TableCellAnonymousInnerClass outerInstance;

					public EventHandlerAnonymousInnerClass(TableCellAnonymousInnerClass outerInstance)
					{
						this.outerInstance = outerInstance;
					}

					public override void handle(ActionEvent @event)
					{
						FeaturePoint featurePoint = getTableRow().getItem();
						MatrixDialog.showAndWait(featurePoint);
					}

				}
			}
		}

		internal static Button createButton(string title, string tooltip)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setAlignment(Pos.CENTER);
			label.setPadding(new Insets(0,0,0,0));
			Button button = new Button();
			button.setGraphic(label);
			button.setTooltip(new Tooltip(tooltip));
	//		button.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
	//		button.setMaxSize(Double.MAX_VALUE, Control.USE_PREF_SIZE); // width, height
			return button;
		}
	}

}