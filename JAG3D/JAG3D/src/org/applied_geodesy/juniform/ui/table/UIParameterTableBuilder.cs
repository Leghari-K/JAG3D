using System;

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
	using FeatureEventType = org.applied_geodesy.adjustment.geometry.FeatureEvent.FeatureEventType;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using UnknownParameterTypeDialog = org.applied_geodesy.juniform.ui.dialog.UnknownParameterTypeDialog;
	using ColumnTooltipHeader = org.applied_geodesy.ui.table.ColumnTooltipHeader;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using DisplayCellFormatType = org.applied_geodesy.ui.table.DisplayCellFormatType;
	using org.applied_geodesy.ui.table;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using org.applied_geodesy.util;

	using Platform = javafx.application.Platform;
	using FXCollections = javafx.collections.FXCollections;
	using FilteredList = javafx.collections.transformation.FilteredList;
	using Pos = javafx.geometry.Pos;
	using TableCell = javafx.scene.control.TableCell;
	using TableColumn = javafx.scene.control.TableColumn;
	using TableView = javafx.scene.control.TableView;
	using TextFieldTableCell = javafx.scene.control.cell.TextFieldTableCell;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class UIParameterTableBuilder : UIEditableTableBuilder<UnknownParameter>, FeatureChangeListener
	{
		private static UIParameterTableBuilder tableBuilder = new UIParameterTableBuilder();
		private bool isInitialize = false;
		private ObservableUniqueList<UnknownParameter> unknownFeatureParameters;
		private System.Predicate<UnknownParameter> visiblePredicate = (UnknownParameter unknownParameter) =>
		{
		return unknownParameter.isVisible();
		};
		private UIParameterTableBuilder() : base()
		{
		}

		public static UIParameterTableBuilder Instance
		{
			get
			{
				tableBuilder.init();
				return tableBuilder;
			}
		}

		public override TableView<UnknownParameter> Table
		{
			get
			{
				return this.getTable(null);
			}
		}

		public virtual TableView<UnknownParameter> getTable(GeometricPrimitive geometry)
		{
			// show all items
			if (geometry == null)
			{
				if (this.unknownFeatureParameters != null)
				{
					FilteredList<UnknownParameter> filteredFeatureParameters = new FilteredList<UnknownParameter>(this.unknownFeatureParameters, this.visiblePredicate);
					this.table.setItems(filteredFeatureParameters);
				}
			}
			else
			{
				this.table.setItems(FXCollections.observableArrayList(geometry.UnknownParameters));
			}

			return this.table;
		}

		private void init()
		{
			if (this.isInitialize)
			{
				return;
			}

			TableColumn<UnknownParameter, ParameterType> parameterTypeColumn = null;
	//		TableColumn<UnknownParameter, ProcessingType> processingTypeColumn = null;
			TableColumn<UnknownParameter, double> doubleColumn = null;
			TableColumn<UnknownParameter, string> stringColumn = null;

			TableView<UnknownParameter> table = this.createTable();

			// Parameter name
			int columnIndex = table.getColumns().size();
			string labelText = i18n.getString("UIParameterTableBuilder.tableheader.name.label", "Name");
			string tooltipText = i18n.getString("UIParameterTableBuilder.tableheader.name.tooltip", "Name of the model parameter");
			ColumnTooltipHeader header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(header, UnknownParameter::nameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// Parameter type
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIParameterTableBuilder.tableheader.type.label", "Type");
			tooltipText = i18n.getString("UIParameterTableBuilder.tableheader.type.tooltip", "Type of the model parameter");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			parameterTypeColumn = this.getColumn<ParameterType>(header, UnknownParameter::parameterTypeProperty, ParameterTypeCallback, ColumnType.VISIBLE, columnIndex, false);
			parameterTypeColumn.setComparator(new NaturalOrderComparator<ParameterType>());
			table.getColumns().add(parameterTypeColumn);

			///////////////// A-POSTERIORI VALUES /////////////////////////////
			// Parameter value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIParameterTableBuilder.tableheader.value.aposteriori.label", "Estimated value");
			tooltipText = i18n.getString("UIParameterTableBuilder.tableheader.value.aposteriori.tooltip", "Estimated value of model parameter");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, UnknownParameter::valueProperty, getDoubleValueWithUnitCallback(DisplayCellFormatType.NORMAL), ColumnType.VISIBLE, columnIndex, false);
			table.getColumns().add(doubleColumn);

			// Uncertainty
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIParameterTableBuilder.tableheader.uncertainty.label", "\u03C3");
			tooltipText = i18n.getString("UIParameterTableBuilder.tableheader.uncertainty.tooltip", "A-posteriori uncertainty of parameter");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, UnknownParameter::uncertaintyProperty, getDoubleValueWithUnitCallback(DisplayCellFormatType.UNCERTAINTY), ColumnType.VISIBLE, columnIndex, false);
			table.getColumns().add(doubleColumn);

			// description text
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIParameterTableBuilder.tableheader.description.label", "Description");
			tooltipText = i18n.getString("UIParameterTableBuilder.tableheader.description.tooltip", "User-defined description of the parameter");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(header, UnknownParameter::descriptionProperty, StringCallback, ColumnType.VISIBLE, columnIndex, true);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			table.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY);
			this.table = table;
			this.isInitialize = true;
		}

		private static Callback<TableColumn<UnknownParameter, ParameterType>, TableCell<UnknownParameter, ParameterType>> ParameterTypeCallback
		{
			get
			{
				return new CallbackAnonymousInnerClass();
			}
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn<UnknownParameter, ParameterType>, TableCell<UnknownParameter, ParameterType>>
		{
			public override TableCell<UnknownParameter, ParameterType> call(TableColumn<UnknownParameter, ParameterType> cell)
			{
				TableCell<UnknownParameter, ParameterType> tableCell = new TextFieldTableCell<UnknownParameter, ParameterType>(new StringConverterAnonymousInnerClass(this));
				tableCell.setAlignment(Pos.CENTER_LEFT);
				return tableCell;
			}

			private class StringConverterAnonymousInnerClass : StringConverter<ParameterType>
			{
				private readonly CallbackAnonymousInnerClass outerInstance;

				public StringConverterAnonymousInnerClass(CallbackAnonymousInnerClass outerInstance)
				{
					this.outerInstance = outerInstance;
				}


				public override string toString(ParameterType parameterType)
				{
					return UnknownParameterTypeDialog.getParameterTypeLabel(parameterType);
				}

				public override ParameterType fromString(string @string)
				{
					return (ParameterType)Enum.Parse(typeof(ParameterType), @string);
				}
			}
		}

		private static Callback<TableColumn<UnknownParameter, double>, TableCell<UnknownParameter, double>> getDoubleValueWithUnitCallback(DisplayCellFormatType displayFormatType)
		{
			return new CallbackAnonymousInnerClass2(displayFormatType);
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn<UnknownParameter, double>, TableCell<UnknownParameter, double>>
		{
			private DisplayCellFormatType displayFormatType;

			public CallbackAnonymousInnerClass2(DisplayCellFormatType displayFormatType)
			{
				this.displayFormatType = displayFormatType;
			}

			public override TableCell<UnknownParameter, double> call(TableColumn<UnknownParameter, double> cell)
			{
				return new ParameterDoubleCell(displayFormatType);
			}
		}

		private Feature Feature
		{
			set
			{
				if (value == null)
				{
					this.unknownFeatureParameters = new ObservableUniqueList<UnknownParameter>(0);
				}
				else
				{
					this.unknownFeatureParameters = value.UnknownParameters;
				}
			}
		}

		internal override void setValue(UnknownParameter unknownParameter, int columnIndex, object oldValue, object newValue)
		{
			bool valid = (oldValue == null || oldValue.ToString().Trim().Length == 0) && (newValue == null || newValue.ToString().Trim().Length == 0);
			switch (columnIndex)
			{
			case 0:
				if (newValue != null && newValue.ToString().Trim().Length > 0)
				{
					unknownParameter.Name = newValue.ToString().Trim();
					valid = true;
				}
				else
				{
					unknownParameter.Name = oldValue == null ? null : oldValue.ToString().Trim();
				}
				break;

			case 4:
				if (newValue != null && newValue.ToString().Trim().Length > 0)
				{
					unknownParameter.Description = newValue.ToString().Trim();
					valid = true;
				}
				else
				{
					unknownParameter.Description = oldValue == null ? null : oldValue.ToString().Trim();
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
				table.getSelectionModel().select(unknownParameter);
				});
			}
		}

		public virtual void featureChanged(FeatureEvent evt)
		{
			if (evt.EventType == FeatureEvent.FeatureEventType.FEATURE_ADDED)
			{
				this.Feature = evt.Source;
			}
			else if (evt.EventType == FeatureEvent.FeatureEventType.FEATURE_REMOVED)
			{
				this.Feature = null;
			}
		}

	}

}