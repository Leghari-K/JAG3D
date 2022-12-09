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

namespace org.applied_geodesy.jag3d.ui.table
{

	using VarianceComponentType = org.applied_geodesy.adjustment.network.VarianceComponentType;
	using ColumnContentType = org.applied_geodesy.jag3d.ui.table.column.ColumnContentType;
	using TableContentType = org.applied_geodesy.jag3d.ui.table.column.TableContentType;
	using VarianceComponentRow = org.applied_geodesy.jag3d.ui.table.row.VarianceComponentRow;
	using ColumnTooltipHeader = org.applied_geodesy.ui.table.ColumnTooltipHeader;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using org.applied_geodesy.ui.table;
	using CellValueType = org.applied_geodesy.util.CellValueType;

	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Pos = javafx.geometry.Pos;
	using TableCell = javafx.scene.control.TableCell;
	using TableColumn = javafx.scene.control.TableColumn;
	using TableView = javafx.scene.control.TableView;
	using CellDataFeatures = javafx.scene.control.TableColumn.CellDataFeatures;
	using TextFieldTableCell = javafx.scene.control.cell.TextFieldTableCell;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class UIVarianceComponentTableBuilder : UITableBuilder<VarianceComponentRow>
	{
		public enum VarianceComponentDisplayType
		{
			OVERALL_COMPONENTS,
			SELECTED_GROUP_COMPONENTS
		}

		private static UIVarianceComponentTableBuilder tableBuilder = new UIVarianceComponentTableBuilder();
		private VarianceComponentDisplayType type;
		private IDictionary<VarianceComponentDisplayType, TableView<VarianceComponentRow>> tables = new Dictionary<VarianceComponentDisplayType, TableView<VarianceComponentRow>>();
		private UIVarianceComponentTableBuilder() : base()
		{
		}

		public static UIVarianceComponentTableBuilder Instance
		{
			get
			{
				return tableBuilder;
			}
		}

		public virtual TableView<VarianceComponentRow> getTable(VarianceComponentDisplayType varianceComponentDisplayType)
		{
			this.type = varianceComponentDisplayType;
			this.init();
			return this.table;
		}

		private TableContentType TableContentType
		{
			get
			{
				switch (this.type)
				{
				case org.applied_geodesy.jag3d.ui.table.UIVarianceComponentTableBuilder.VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS:
					return TableContentType.SELECTED_GROUP_VARIANCE_COMPONENTS;
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

			TableColumn<VarianceComponentRow, VarianceComponentType> varianceComponentTypeColumn = null;
			TableColumn<VarianceComponentRow, string> stringColumn = null;
			TableColumn<VarianceComponentRow, bool> booleanColumn = null;
			TableColumn<VarianceComponentRow, double> doubleColumn = null;
			TableColumn<VarianceComponentRow, int> integerColumn = null;

			TableContentType tableContentType = this.TableContentType;

			TableView<VarianceComponentRow> table = this.createTable();

			// Overall component type
			int columnIndex = table.getColumns().size();
			string labelText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.type.label", "Component");
			string tooltipText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.type.tooltip", "Type of estimated variance component");
			CellValueType cellValueType = CellValueType.STRING;
			ColumnContentType columnContentType = this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS ? ColumnContentType.DEFAULT : ColumnContentType.VARIANCE_COMPONENT_TYPE;
			ColumnTooltipHeader header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			varianceComponentTypeColumn = this.getColumn<VarianceComponentType>(tableContentType, columnContentType, header, VarianceComponentRow::varianceComponentTypeProperty, VarianceComponentTypeCallback, this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS ? ColumnType.VISIBLE : ColumnType.HIDDEN, columnIndex, false, this.type == VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			if (this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS)
			{
				varianceComponentTypeColumn.setMinWidth(150);
			}
			varianceComponentTypeColumn.setComparator(new NaturalOrderComparator<VarianceComponentType>());
			table.getColumns().add(varianceComponentTypeColumn);

			// Selected component type
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.name.label", "Name");
			tooltipText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.name.tooltip", "Name of variance component");
			cellValueType = CellValueType.STRING;
			columnContentType = this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS ? ColumnContentType.DEFAULT : ColumnContentType.VARIANCE_COMPONENT_NAME;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(tableContentType, columnContentType, header, VarianceComponentRow::nameProperty, StringCallback, this.type == VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS ? ColumnType.VISIBLE : ColumnType.HIDDEN, columnIndex, false, this.type == VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// number of observations
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.number_of_observations.label", "n");
			tooltipText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.number_of_observations.tooltip", "Number of observations");
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
			cellValueType = CellValueType.INTEGER;
			columnContentType = this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS ? ColumnContentType.DEFAULT : ColumnContentType.NUMBER_OF_OBSERVATION;
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			integerColumn = this.getColumn<int>(tableContentType, columnContentType, header, VarianceComponentRow::numberOfObservationsProperty, IntegerCallback, ColumnType.VISIBLE, columnIndex, false, this.type == VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			table.getColumns().add(integerColumn);

			// redundnacy
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.redundancy.label", "r");
			tooltipText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.redundancy.tooltip", "Redundancy");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS ? ColumnContentType.DEFAULT : ColumnContentType.REDUNDANCY;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VarianceComponentRow::redundancyProperty, getDoubleCallback(cellValueType), ColumnType.VISIBLE, columnIndex, false, this.type == VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			table.getColumns().add(doubleColumn);

			// Omega
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.omega.label", "\u03A9");
			tooltipText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.omega.tooltip", "Squared weigthed residuals");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS ? ColumnContentType.DEFAULT : ColumnContentType.OMEGA;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VarianceComponentRow::omegaProperty, getDoubleCallback(cellValueType), ColumnType.VISIBLE, columnIndex, false, this.type == VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			table.getColumns().add(doubleColumn);

			// Sigma a-posteriori
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.variance.label", "1 : \u03C3\u00B2");
			tooltipText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.variance.tooltip", "A-posteriori variance factor w.r.t. a-priori variance");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS ? ColumnContentType.DEFAULT : ColumnContentType.VARIANCE;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, VarianceComponentRow::sigma2aposterioriProperty, getDoubleCallback(cellValueType), ColumnType.VISIBLE, columnIndex, false, this.type == VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			table.getColumns().add(doubleColumn);

			// Decision of test statistic
			columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexOutlier = columnIndex;
			int columnIndexOutlier = columnIndex;
			labelText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.significant.label", "Significant");
			tooltipText = i18n.getString("UIVarianceComponentTableBuilder.tableheader.significant.tooltip", "Checked, if a-posteriori variance is significant w.r.t. a-priori variance");
			cellValueType = CellValueType.BOOLEAN;
			columnContentType = this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS ? ColumnContentType.DEFAULT : ColumnContentType.SIGNIFICANT;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, VarianceComponentRow::significantProperty, BooleanCallback, this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS ? ColumnType.VISIBLE : ColumnType.HIDDEN, columnIndex, false, this.type == VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass(this, columnIndexOutlier));
			table.getColumns().add(booleanColumn);
			if (this.type == VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS)
			{
				this.addColumnOrderSequenceListeners(tableContentType, table);
			}

			if (this.type == VarianceComponentDisplayType.OVERALL_COMPONENTS)
			{
				table.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY);
			}

			this.tables[this.type] = table;
			this.table = table;
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn.CellDataFeatures<VarianceComponentRow, bool>, ObservableValue<bool>>
		{
			private readonly UIVarianceComponentTableBuilder outerInstance;

			private int columnIndexOutlier;

			public CallbackAnonymousInnerClass(UIVarianceComponentTableBuilder outerInstance, int columnIndexOutlier)
			{
				this.outerInstance = outerInstance;
				this.columnIndexOutlier = columnIndexOutlier;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<VarianceComponentRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isSignificant());
				booleanProp.addListener(significantChangeListener);
				return booleanProp;
			}
		}

		private static Callback<TableColumn<VarianceComponentRow, VarianceComponentType>, TableCell<VarianceComponentRow, VarianceComponentType>> VarianceComponentTypeCallback
		{
			get
			{
				return new CallbackAnonymousInnerClass2();
			}
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn<VarianceComponentRow, VarianceComponentType>, TableCell<VarianceComponentRow, VarianceComponentType>>
		{
			public override TableCell<VarianceComponentRow, VarianceComponentType> call(TableColumn<VarianceComponentRow, VarianceComponentType> cell)
			{
				TableCell<VarianceComponentRow, VarianceComponentType> tableCell = new TextFieldTableCell<VarianceComponentRow, VarianceComponentType>(new StringConverterAnonymousInnerClass(this));
				tableCell.setAlignment(Pos.CENTER_LEFT);
				return tableCell;
			}

			private class StringConverterAnonymousInnerClass : StringConverter<VarianceComponentType>
			{
				private readonly CallbackAnonymousInnerClass2 outerInstance;

				public StringConverterAnonymousInnerClass(CallbackAnonymousInnerClass2 outerInstance)
				{
					this.outerInstance = outerInstance;
				}


				public override string toString(VarianceComponentType type)
				{
					if (type == null)
					{
						return null;
					}

					return getVarianceComponentTypeLabel(type);
				}

				public override VarianceComponentType fromString(string @string)
				{
					return VarianceComponentType.valueOf(@string);
				}
			}
		}

		public static string getVarianceComponentTypeLabel(VarianceComponentType type)
		{
			switch (type.innerEnumValue)
			{
			case VarianceComponentType.InnerEnum.GLOBAL:
				return i18n.getString("UIVarianceComponentTableBuilder.type.global.label", "Global adjustment");

			case VarianceComponentType.InnerEnum.LEVELING_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.leveling.label", "Leveling");
			case VarianceComponentType.InnerEnum.LEVELING_ZERO_POINT_OFFSET_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.leveling.zero_point_offset.label", "Leveling \u03C3a");
			case VarianceComponentType.InnerEnum.LEVELING_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.leveling.square_root_distance_dependent.label", "Leveling \u03C3b");
			case VarianceComponentType.InnerEnum.LEVELING_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.leveling.distance_dependent.label", "Leveling \u03C3c");

			case VarianceComponentType.InnerEnum.DIRECTION_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.direction.label", "Direction");
			case VarianceComponentType.InnerEnum.DIRECTION_ZERO_POINT_OFFSET_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.direction.zero_point_offset.label", "Direction \u03C3a");
			case VarianceComponentType.InnerEnum.DIRECTION_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.direction.square_root_distance_dependent.label", "Direction \u03C3b");
			case VarianceComponentType.InnerEnum.DIRECTION_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.direction.distance_dependent.label", "Direction \u03C3c");

			case VarianceComponentType.InnerEnum.HORIZONTAL_DISTANCE_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.horizontal_distance.label", "Horizontal distance");
			case VarianceComponentType.InnerEnum.HORIZONTAL_DISTANCE_ZERO_POINT_OFFSET_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.horizontal_distance.zero_point_offset.label", "Horizontal distance \u03C3a");
			case VarianceComponentType.InnerEnum.HORIZONTAL_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.horizontal_distance.square_root_distance_dependent.label", "Horizontal distance \u03C3b");
			case VarianceComponentType.InnerEnum.HORIZONTAL_DISTANCE_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.horizontal_distance.distance_dependent.label", "Horizontal distance \u03C3c");

			case VarianceComponentType.InnerEnum.SLOPE_DISTANCE_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.slope_distance.label", "Slope distance");
			case VarianceComponentType.InnerEnum.SLOPE_DISTANCE_ZERO_POINT_OFFSET_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.slope_distance.zero_point_offset.label", "Slope distance \u03C3a");
			case VarianceComponentType.InnerEnum.SLOPE_DISTANCE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.slope_distance.square_root_distance_dependent.label", "Slope distance \u03C3b");
			case VarianceComponentType.InnerEnum.SLOPE_DISTANCE_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.slope_distance.distance_dependent.label", "Slope distance \u03C3c");

			case VarianceComponentType.InnerEnum.ZENITH_ANGLE_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.zenith_angle.label", "Zenith angle");
			case VarianceComponentType.InnerEnum.ZENITH_ANGLE_ZERO_POINT_OFFSET_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.zenith_angle.zero_point_offset.label", "Zenith angle \u03C3a");
			case VarianceComponentType.InnerEnum.ZENITH_ANGLE_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.zenith_angle.square_root_distance_dependent.label", "Zenith angle \u03C3b");
			case VarianceComponentType.InnerEnum.ZENITH_ANGLE_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.zenith_angle.distance_dependent.label", "Zenith angle \u03C3c");

			case VarianceComponentType.InnerEnum.GNSS1D_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.1d.label", "GNSS baseline 1D");
			case VarianceComponentType.InnerEnum.GNSS1D_ZERO_POINT_OFFSET_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.1d.zero_point_offset.label", "GNSS baseline 1D \u03C3a");
			case VarianceComponentType.InnerEnum.GNSS1D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.1d.square_root_distance_dependent.label", "GNSS baseline 1D \u03C3b");
			case VarianceComponentType.InnerEnum.GNSS1D_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.1d.distance_dependent.label", "GNSS baseline 1D \u03C3c");

			case VarianceComponentType.InnerEnum.GNSS2D_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.2d.label", "GNSS baseline 2D");
			case VarianceComponentType.InnerEnum.GNSS2D_ZERO_POINT_OFFSET_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.2d.zero_point_offset.label", "GNSS baseline 2D \u03C3a");
			case VarianceComponentType.InnerEnum.GNSS2D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.2d.square_root_distance_dependent.label", "GNSS baseline 2D \u03C3b");
			case VarianceComponentType.InnerEnum.GNSS2D_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.2d.distance_dependent.label", "GNSS baseline 2D \u03C3c");

			case VarianceComponentType.InnerEnum.GNSS3D_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.3d.label", "GNSS baseline 3D");
			case VarianceComponentType.InnerEnum.GNSS3D_ZERO_POINT_OFFSET_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.3d.zero_point_offset.label", "GNSS baseline 3D \u03C3a");
			case VarianceComponentType.InnerEnum.GNSS3D_SQUARE_ROOT_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.3d.square_root_distance_dependent.label", "GNSS baseline 3D \u03C3b");
			case VarianceComponentType.InnerEnum.GNSS3D_DISTANCE_DEPENDENT_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.gnss.3d.distance_dependent.label", "GNSS baseline 3D \u03C3c");

			case VarianceComponentType.InnerEnum.STOCHASTIC_POINT_1D_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.point.1d.label", "Stochastic point 1D");
			case VarianceComponentType.InnerEnum.STOCHASTIC_POINT_2D_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.point.2d.label", "Stochastic point 2D");
			case VarianceComponentType.InnerEnum.STOCHASTIC_POINT_3D_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.point.3d.label", "Stochastic point 3D");

			case VarianceComponentType.InnerEnum.STOCHASTIC_DEFLECTION_COMPONENT:
				return i18n.getString("UIVarianceComponentTableBuilder.type.deflection.label", "Deflection of the vertical");
			}
			return null;
		}

		internal override void setValue(VarianceComponentRow row, int columnIndex, object oldValue, object newValue)
		{
		}
		public override VarianceComponentRow EmptyRow
		{
			get
			{
				return new VarianceComponentRow();
			}
		}
	}

}