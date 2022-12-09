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
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using ColumnContentType = org.applied_geodesy.jag3d.ui.table.column.ColumnContentType;
	using TableContentType = org.applied_geodesy.jag3d.ui.table.column.TableContentType;
	using AdditionalParameterRow = org.applied_geodesy.jag3d.ui.table.row.AdditionalParameterRow;
	using AbsoluteValueComparator = org.applied_geodesy.ui.table.AbsoluteValueComparator;
	using ColumnTooltipHeader = org.applied_geodesy.ui.table.ColumnTooltipHeader;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using DisplayCellFormatType = org.applied_geodesy.ui.table.DisplayCellFormatType;
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

	public class UIAdditionalParameterTableBuilder : UITableBuilder<AdditionalParameterRow>
	{
		private static UIAdditionalParameterTableBuilder tableBuilder = new UIAdditionalParameterTableBuilder();
		private bool isInitialize = false;
		private UIAdditionalParameterTableBuilder() : base()
		{
		}

		public static UIAdditionalParameterTableBuilder Instance
		{
			get
			{
				tableBuilder.init();
				return tableBuilder;
			}
		}

		private TableContentType TableContentType
		{
			get
			{
				return TableContentType.ADDITIONAL_PARAMETER;
			}
		}

		private void init()
		{
			if (this.isInitialize)
			{
				return;
			}

			TableColumn<AdditionalParameterRow, ParameterType> parameterTypeColumn = null;
			TableColumn<AdditionalParameterRow, bool> booleanColumn = null;
			TableColumn<AdditionalParameterRow, double> doubleColumn = null;

			TableContentType tableContentType = this.TableContentType;

			TableView<AdditionalParameterRow> table = this.createTable();

			// Parameter type
			int columnIndex = table.getColumns().size();
			string labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.type.label", "Parameter type");
			string tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.type.tooltip", "Type of the additional parameter");
			ColumnTooltipHeader header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
			ColumnContentType columnContentType = ColumnContentType.PARAMETER_NAME;
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			parameterTypeColumn = this.getColumn<ParameterType>(tableContentType, columnContentType, header, AdditionalParameterRow::parameterTypeProperty, ParameterTypeCallback, ColumnType.VISIBLE, columnIndex, false, true);
			parameterTypeColumn.setComparator(new NaturalOrderComparator<ParameterType>());
			table.getColumns().add(parameterTypeColumn);

			// Parameter value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.value.label", "Value");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.value.tooltip", "Estimated value of additional parameter");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
			columnContentType = ColumnContentType.VALUE_APOSTERIORI;
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, AdditionalParameterRow::valueAposterioriProperty, getDoubleValueWithUnitCallback(DisplayCellFormatType.NORMAL), ColumnType.VISIBLE, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Uncertainty
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.uncertainty.label", "\u03C3");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.uncertainty.tooltip", "A-posteriori uncertainty of additional parameter");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
			columnContentType = ColumnContentType.UNCERTAINTY_APOSTERIORI;
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, AdditionalParameterRow::sigmaAposterioriProperty, getDoubleValueWithUnitCallback(DisplayCellFormatType.UNCERTAINTY), ColumnType.VISIBLE, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Confidence
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.semiaxis.label", "a");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.semiaxis.tooltip", "Confidence interval");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
			columnContentType = ColumnContentType.CONFIDENCE_A;
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, AdditionalParameterRow::confidenceProperty, getDoubleValueWithUnitCallback(DisplayCellFormatType.UNCERTAINTY), ColumnType.VISIBLE, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// Nabla
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.grosserror.label", "\u2207");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.grosserror.tooltip", "Gross-error of additional parameter");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
			columnContentType = ColumnContentType.GROSS_ERROR;
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, AdditionalParameterRow::grossErrorProperty, getDoubleValueWithUnitCallback(DisplayCellFormatType.RESIDUAL), ColumnType.VISIBLE, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// MDB
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.minimaldetectablebias.label", "\u2207(\u03BB)");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.minimaldetectablebias.tooltip", "Minimal detectable bias of additional parameter");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
			columnContentType = ColumnContentType.MINIMAL_DETECTABLE_BIAS;
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, AdditionalParameterRow::minimalDetectableBiasProperty, getDoubleValueWithUnitCallback(DisplayCellFormatType.RESIDUAL), ColumnType.VISIBLE, columnIndex, false, true);
			table.getColumns().add(doubleColumn);

			// A-priori log(p)
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.pvalue.apriori.label", "log(Pprio)");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.pvalue.apriori.tooltip", "A-priori p-value in logarithmic representation");
			CellValueType cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, AdditionalParameterRow::pValueAprioriProperty, getDoubleCallback(cellValueType), ColumnType.VISIBLE, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// A-priori log(p)
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.pvalue.aposteriori.label", "log(Ppost)");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.pvalue.aposteriori.tooltip", "A-posteriori p-value in logarithmic representation");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.P_VALUE_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, AdditionalParameterRow::pValueAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.VISIBLE, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// A-priori test statistic
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.teststatistic.apriori.label", "Tprio");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.teststatistic.apriori.tooltip", "A-priori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APRIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, AdditionalParameterRow::testStatisticAprioriProperty, getDoubleCallback(cellValueType), ColumnType.VISIBLE, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// A-posteriori test statistic
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.teststatistic.aposteriori.label", "Tpost");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.teststatistic.aposteriori.tooltip", "A-posteriori test statistic");
			cellValueType = CellValueType.STATISTIC;
			columnContentType = ColumnContentType.TEST_STATISTIC_APOSTERIORI;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText, options.FormatterOptions[cellValueType].getUnit());
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(tableContentType, columnContentType, header, AdditionalParameterRow::testStatisticAposterioriProperty, getDoubleCallback(cellValueType), ColumnType.VISIBLE, columnIndex, false, true);
			doubleColumn.setComparator(new AbsoluteValueComparator());
			table.getColumns().add(doubleColumn);

			// Decision of test statistic
			columnIndex = table.getColumns().size();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnIndexOutlier = columnIndex;
			int columnIndexOutlier = columnIndex;
			labelText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.testdecision.label", "Significant");
			tooltipText = i18n.getString("UIAdditionalParameterTableBuilder.tableheader.testdecision.tooltip", "Checked, if null-hypothesis is rejected");
			cellValueType = CellValueType.BOOLEAN;
			columnContentType = ColumnContentType.SIGNIFICANT;
			header = new ColumnTooltipHeader(cellValueType, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			booleanColumn = this.getColumn<bool>(tableContentType, columnContentType, header, AdditionalParameterRow::significantProperty, BooleanCallback, ColumnType.VISIBLE, columnIndex, false, true);
			booleanColumn.setCellValueFactory(new CallbackAnonymousInnerClass(this, columnIndexOutlier));
			table.getColumns().add(booleanColumn);
			this.addColumnOrderSequenceListeners(tableContentType, table);

			this.table = table;
			this.isInitialize = true;
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn.CellDataFeatures<AdditionalParameterRow, bool>, ObservableValue<bool>>
		{
			private readonly UIAdditionalParameterTableBuilder outerInstance;

			private int columnIndexOutlier;

			public CallbackAnonymousInnerClass(UIAdditionalParameterTableBuilder outerInstance, int columnIndexOutlier)
			{
				this.outerInstance = outerInstance;
				this.columnIndexOutlier = columnIndexOutlier;
			}

			public override ObservableValue<bool> call(TableColumn.CellDataFeatures<AdditionalParameterRow, bool> param)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				TableCellChangeListener<bool> significantChangeListener = new TableCellChangeListener<bool>(outerInstance, columnIndexOutlier, param.getValue());
				BooleanProperty booleanProp = new SimpleBooleanProperty(param.getValue().isSignificant());
				booleanProp.addListener(significantChangeListener);
				return booleanProp;
			}
		}

		private static Callback<TableColumn<AdditionalParameterRow, ParameterType>, TableCell<AdditionalParameterRow, ParameterType>> ParameterTypeCallback
		{
			get
			{
				return new CallbackAnonymousInnerClass2();
			}
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn<AdditionalParameterRow, ParameterType>, TableCell<AdditionalParameterRow, ParameterType>>
		{
			public override TableCell<AdditionalParameterRow, ParameterType> call(TableColumn<AdditionalParameterRow, ParameterType> cell)
			{
				TableCell<AdditionalParameterRow, ParameterType> tableCell = new TextFieldTableCell<AdditionalParameterRow, ParameterType>(new StringConverterAnonymousInnerClass(this));
				tableCell.setAlignment(Pos.CENTER_LEFT);
				return tableCell;
			}

			private class StringConverterAnonymousInnerClass : StringConverter<ParameterType>
			{
				private readonly CallbackAnonymousInnerClass2 outerInstance;

				public StringConverterAnonymousInnerClass(CallbackAnonymousInnerClass2 outerInstance)
				{
					this.outerInstance = outerInstance;
				}


				public override string toString(ParameterType type)
				{
					if (type == null)
					{
						return null;
					}

					switch (type.innerEnumValue)
					{
					case ParameterType.InnerEnum.ZERO_POINT_OFFSET:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.zero_point_offset", "Zero point offset a");
					case ParameterType.InnerEnum.SCALE:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.scale", "Scale m");
					case ParameterType.InnerEnum.ORIENTATION:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.orientation", "Orientation o");
					case ParameterType.InnerEnum.REFRACTION_INDEX:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.refractionindex", "Refraction index k");

					case ParameterType.InnerEnum.ROTATION_X:
					case ParameterType.InnerEnum.STRAIN_ROTATION_X:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.rotation.x", "Rotation angle rx");
					case ParameterType.InnerEnum.ROTATION_Y:
					case ParameterType.InnerEnum.STRAIN_ROTATION_Y:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.rotation.y", "Rotation angle ry");
					case ParameterType.InnerEnum.ROTATION_Z:
					case ParameterType.InnerEnum.STRAIN_ROTATION_Z:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.rotation.z", "Rotation angle rz");

					case ParameterType.InnerEnum.STRAIN_SCALE_X:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.scale.x", "Scale mx");
					case ParameterType.InnerEnum.STRAIN_SCALE_Y:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.scale.y", "Scale my");
					case ParameterType.InnerEnum.STRAIN_SCALE_Z:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.scale.z", "Scale mz");

					case ParameterType.InnerEnum.STRAIN_SHEAR_X:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.shear.x", "Shear angle sx");
					case ParameterType.InnerEnum.STRAIN_SHEAR_Y:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.shear.y", "Shear angle sy");
					case ParameterType.InnerEnum.STRAIN_SHEAR_Z:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.shear.z", "Shear angle sz");

					case ParameterType.InnerEnum.STRAIN_TRANSLATION_X:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.translation.x", "Shift tx");
					case ParameterType.InnerEnum.STRAIN_TRANSLATION_Y:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.translation.y", "Shift ty");
					case ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
						return i18n.getString("UIAdditionalParameterTableBuilder.type.translation.z", "Shift tz");

					case ParameterType.InnerEnum.STRAIN_S11:
						break;
					case ParameterType.InnerEnum.STRAIN_S12:
						break;
					case ParameterType.InnerEnum.STRAIN_S13:
						break;
					case ParameterType.InnerEnum.STRAIN_S22:
						break;
					case ParameterType.InnerEnum.STRAIN_S23:
						break;
					case ParameterType.InnerEnum.STRAIN_S33:
						break;

					case ParameterType.InnerEnum.STRAIN_A11:
						break;
					case ParameterType.InnerEnum.STRAIN_A12:
						break;
					case ParameterType.InnerEnum.STRAIN_A21:
						break;
					case ParameterType.InnerEnum.STRAIN_A22:
						break;


					default:
						break;

					}
					return null;
				}

				public override ParameterType fromString(string @string)
				{
					return ParameterType.valueOf(@string);
				}
			}

		}

		private static Callback<TableColumn<AdditionalParameterRow, double>, TableCell<AdditionalParameterRow, double>> getDoubleValueWithUnitCallback(DisplayCellFormatType displayFormatType)
		{
			return new CallbackAnonymousInnerClass3(displayFormatType);
		}

		private class CallbackAnonymousInnerClass3 : Callback<TableColumn<AdditionalParameterRow, double>, TableCell<AdditionalParameterRow, double>>
		{
			private DisplayCellFormatType displayFormatType;

			public CallbackAnonymousInnerClass3(DisplayCellFormatType displayFormatType)
			{
				this.displayFormatType = displayFormatType;
			}

			public override TableCell<AdditionalParameterRow, double> call(TableColumn<AdditionalParameterRow, double> cell)
			{
				return new AdditionalParameterDoubleCell(displayFormatType);
			}
		}

		public override AdditionalParameterRow EmptyRow
		{
			get
			{
				return new AdditionalParameterRow();
			}
		}

		internal override void setValue(AdditionalParameterRow row, int columnIndex, object oldValue, object newValue)
		{
		}
	}

}