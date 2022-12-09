using System.Collections.Generic;
using System.Text;

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

	using ColumnContentType = org.applied_geodesy.jag3d.ui.table.column.ColumnContentType;
	using ColumnPropertiesManager = org.applied_geodesy.jag3d.ui.table.column.ColumnPropertiesManager;
	using ColumnProperty = org.applied_geodesy.jag3d.ui.table.column.ColumnProperty;
	using org.applied_geodesy.jag3d.ui.table.column;
	using TableContentType = org.applied_geodesy.jag3d.ui.table.column.TableContentType;
	using TableRowHighlight = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlight;
	using TableRowHighlightRangeType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType;
	using ColumnTooltipHeader = org.applied_geodesy.ui.table.ColumnTooltipHeader;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using org.applied_geodesy.ui.table;
	using EditableDoubleCellConverter = org.applied_geodesy.ui.table.EditableDoubleCellConverter;
	using EditableIntegerCellConverter = org.applied_geodesy.ui.table.EditableIntegerCellConverter;
	using EditableStringCellConverter = org.applied_geodesy.ui.table.EditableStringCellConverter;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Bindings = javafx.beans.binding.Bindings;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using FXCollections = javafx.collections.FXCollections;
	using ListChangeListener = javafx.collections.ListChangeListener;
	using ObservableList = javafx.collections.ObservableList;
	using EventHandler = javafx.@event.EventHandler;
	using Pos = javafx.geometry.Pos;
	using ContextMenu = javafx.scene.control.ContextMenu;
	using Label = javafx.scene.control.Label;
	using SelectionMode = javafx.scene.control.SelectionMode;
	using TableCell = javafx.scene.control.TableCell;
	using TableColumn = javafx.scene.control.TableColumn;
	using TableRow = javafx.scene.control.TableRow;
	using CellDataFeatures = javafx.scene.control.TableColumn.CellDataFeatures;
	using CellEditEvent = javafx.scene.control.TableColumn.CellEditEvent;
	using TablePosition = javafx.scene.control.TablePosition;
	using TableView = javafx.scene.control.TableView;
	using CheckBoxTableCell = javafx.scene.control.cell.CheckBoxTableCell;
	using Clipboard = javafx.scene.input.Clipboard;
	using ClipboardContent = javafx.scene.input.ClipboardContent;
	using KeyCode = javafx.scene.input.KeyCode;
	using KeyCodeCombination = javafx.scene.input.KeyCodeCombination;
	using KeyCombination = javafx.scene.input.KeyCombination;
	using KeyEvent = javafx.scene.input.KeyEvent;
	using MouseButton = javafx.scene.input.MouseButton;
	using MouseEvent = javafx.scene.input.MouseEvent;
	using Color = javafx.scene.paint.Color;
	using Callback = javafx.util.Callback;

	public abstract class UITableBuilder<T>
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			numberAndUnitFormatterChangedListener = new NumberAndUnitFormatterChangedListener(this);
			table = this.createTable();
		}


		internal class NumberAndUnitFormatterChangedListener : FormatterChangedListener
		{
			private readonly UITableBuilder<T> outerInstance;

			public NumberAndUnitFormatterChangedListener(UITableBuilder<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void formatterChanged(FormatterEvent evt)
			{
				if (outerInstance.table != null)
				{
					outerInstance.table.refresh();
				}
			}
		}

		internal class TableKeyEventHandler : EventHandler<KeyEvent>
		{
			private readonly UITableBuilder<T> outerInstance;

			public TableKeyEventHandler(UITableBuilder<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal KeyCodeCombination copyKeyCodeCompination = new KeyCodeCombination(KeyCode.C, KeyCombination.CONTROL_ANY);
			public virtual void handle(in KeyEvent keyEvent)
			{

				if (copyKeyCodeCompination.match(keyEvent))
				{
					if (keyEvent.getSource() is TableView)
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: copySelectionToClipboard((javafx.scene.control.TableView<?>) keyEvent.getSource());
						copySelectionToClipboard((TableView<object>) keyEvent.getSource());
						keyEvent.consume();
					}
				}
			}
		}

		internal class TableCellChangeListener<S> : ChangeListener<S>
		{
			private readonly UITableBuilder<T> outerInstance;

			internal readonly int columnIndex;
			internal readonly T rowData;
			internal TableCellChangeListener(UITableBuilder<T> outerInstance, int columnIndex, T rowData)
			{
				this.outerInstance = outerInstance;
				this.columnIndex = columnIndex;
				this.rowData = rowData;
			}

			public override void changed<T1>(ObservableValue<T1> observable, S oldValue, S newValue) where T1 : S
			{
				if (this.rowData != null)
				{
					IList<T> selectedItems = outerInstance.table.getSelectionModel().getSelectedItems();
					if (selectedItems == null || selectedItems.Count == 0 || !selectedItems.Contains(this.rowData))
					{
						selectedItems = new List<T>(1);
						selectedItems.Add(this.rowData);
						outerInstance.table.getSelectionModel().clearSelection();
						outerInstance.table.getSelectionModel().select(this.rowData);
					}

					foreach (T item in selectedItems)
					{
						outerInstance.setValue(item, columnIndex, oldValue, newValue);
					}

					if (selectedItems.Count > 1)
					{
						outerInstance.table.refresh();
					}

	//				setValue(rowData, columnIndex, oldValue, newValue);
				}
			}
		}

		internal class TableCellEvent<S> : EventHandler<TableColumn.CellEditEvent<T, S>>
		{
			private readonly UITableBuilder<T> outerInstance;

			internal readonly int columnIndex;
			internal TableCellEvent(UITableBuilder<T> outerInstance, int columnIndex)
			{
				this.outerInstance = outerInstance;
				this.columnIndex = columnIndex;
			}

			public override void handle(TableColumn.CellEditEvent<T, S> @event)
			{
				if (@event.getTableColumn().isEditable())
				{
					outerInstance.setValue(@event.getRowValue(), this.columnIndex, @event.getOldValue(), @event.getNewValue());
				}
			}
		}

		private class SortOrderChangeListener : ListChangeListener<TableColumn<T, JavaToDotNetGenericWildcard>>
		{
			private readonly UITableBuilder<T> outerInstance;

			public SortOrderChangeListener(UITableBuilder<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void onChanged<T1, T2>(Change<T1, T2> change) where T1 : javafx.scene.control.TableColumn<T, T1>
			{
				while (change.next())
				{
					if (change.wasRemoved())
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (javafx.scene.control.TableColumn<T, ?> removedColumnItem : change.getRemoved())
						foreach (TableColumn<T, object> removedColumnItem in change.getRemoved())
						{
							if (removedColumnItem is ContentColumn)
							{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T,?>)removedColumnItem).getColumnProperty().setSortOrder(-1);
								((ContentColumn<T, object>)removedColumnItem).ColumnProperty.SortOrder = -1;
							}
						}
					}
				}

				int idx = 0;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (javafx.scene.control.TableColumn<T, ?> addedColumnItem : change.getList())
				foreach (TableColumn<T, object> addedColumnItem in change.getList())
				{
					if (addedColumnItem is ContentColumn)
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T,?>)addedColumnItem).getColumnProperty().setSortOrder(idx++);
						((ContentColumn<T, object>)addedColumnItem).ColumnProperty.SortOrder = idx++;
					}
				}
			}
		}

		private class ColumnsOrderChangeListener : ListChangeListener<TableColumn<T, JavaToDotNetGenericWildcard>>
		{
			private readonly UITableBuilder<T> outerInstance;

			public ColumnsOrderChangeListener(UITableBuilder<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void onChanged<T1, T2>(Change<T1, T2> change) where T1 : javafx.scene.control.TableColumn<T, T1>
			{
				int idx = 0;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (javafx.scene.control.TableColumn<T, ?> addedColumnItem : change.getList())
				foreach (TableColumn<T, object> addedColumnItem in change.getList())
				{
					if (addedColumnItem is ContentColumn)
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T,?>)addedColumnItem).getColumnProperty().setColumnOrder(idx++);
						((ContentColumn<T, object>)addedColumnItem).ColumnProperty.ColumnOrder = idx++;
					}
				}
			}
		}

		private class SortOrderSequenceChangeListener : ListChangeListener<ColumnContentType>
		{
			private readonly UITableBuilder<T> outerInstance;


			internal readonly TableView<T> tableView;
			public SortOrderSequenceChangeListener(UITableBuilder<T> outerInstance, TableView<T> tableView)
			{
				this.outerInstance = outerInstance;
				this.tableView = tableView;
			}

			public override void onChanged<T1>(Change<T1> change) where T1 : org.applied_geodesy.jag3d.ui.table.column.ColumnContentType
			{
				outerInstance.setSortOrderSequence(this.tableView, change.getList());
			}
		}

		private class ColumnsOrderSequenceChangeListener : ListChangeListener<ColumnContentType>
		{
			private readonly UITableBuilder<T> outerInstance;

			internal readonly TableView<T> tableView;
			public ColumnsOrderSequenceChangeListener(UITableBuilder<T> outerInstance, TableView<T> tableView)
			{
				this.outerInstance = outerInstance;
				this.tableView = tableView;
			}

			public override void onChanged<T1>(Change<T1> change) where T1 : org.applied_geodesy.jag3d.ui.table.column.ColumnContentType
			{
				outerInstance.setColumnsOrderSequence(this.tableView, change.getList());
			}
		}

		internal NumberAndUnitFormatterChangedListener numberAndUnitFormatterChangedListener;
		internal static FormatterOptions options = FormatterOptions.Instance;
		internal static I18N i18n = I18N.Instance;
		internal TableView<T> table;

		internal UITableBuilder()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			options.addFormatterChangedListener(this.numberAndUnitFormatterChangedListener);
		}

		internal virtual TableView<T> createTable()
		{
			this.table = new TableView<T>();
			ObservableList<T> tableModel = FXCollections.observableArrayList();
			this.table.setItems(tableModel);
			this.table.setColumnResizePolicy(TableView.UNCONSTRAINED_RESIZE_POLICY);
			this.table.setTableMenuButtonVisible(false);
			//		this.table.setPlaceholder(new Text(i18n.getString("UITableBuilder.emptytable", "No content in table.")));
			this.table.getSelectionModel().setSelectionMode(SelectionMode.MULTIPLE);
			this.table.setOnKeyPressed(new TableKeyEventHandler(this));
			tableModel.add(this.EmptyRow);
			return table;
		}

		internal static Callback<TableColumn<T, int>, TableCell<T, int>> getIntegerCallback<T>()
		{
			get
			{
				return new CallbackAnonymousInnerClass();
			}
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn<T, int>, TableCell<T, int>>
		{
			public override TableCell<T, int> call(TableColumn<T, int> cell)
			{
				TableCell<T, int> tableCell = new EditableCell<T, int>(new EditableIntegerCellConverter());
				tableCell.setAlignment(Pos.CENTER_RIGHT);
				return tableCell;
			}
		}

		internal static Callback<TableColumn<T, double>, TableCell<T, double>> getDoubleCallback<T>(CellValueType cellValueType)
		{
			return new CallbackAnonymousInnerClass2(cellValueType);
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn<T, double>, TableCell<T, double>>
		{
			private CellValueType cellValueType;

			public CallbackAnonymousInnerClass2(CellValueType cellValueType)
			{
				this.cellValueType = cellValueType;
			}

			public override TableCell<T, double> call(TableColumn<T, double> cell)
			{
				TableCell<T, double> tableCell = new EditableCell<T, double>(new EditableDoubleCellConverter(cellValueType));
				tableCell.setAlignment(Pos.CENTER_RIGHT);
				return tableCell;
			}
		}

		internal static Callback<TableColumn<T, double>, TableCell<T, double>> getDoubleCallback<T>(CellValueType cellValueType, bool displayUnit)
		{
			return new CallbackAnonymousInnerClass3(cellValueType, displayUnit);
		}

		private class CallbackAnonymousInnerClass3 : Callback<TableColumn<T, double>, TableCell<T, double>>
		{
			private CellValueType cellValueType;
			private bool displayUnit;

			public CallbackAnonymousInnerClass3(CellValueType cellValueType, bool displayUnit)
			{
				this.cellValueType = cellValueType;
				this.displayUnit = displayUnit;
			}

			public override TableCell<T, double> call(TableColumn<T, double> cell)
			{
				TableCell<T, double> tableCell = new EditableCell<T, double>(new EditableDoubleCellConverter(cellValueType, displayUnit));
				tableCell.setAlignment(Pos.CENTER_RIGHT);
				return tableCell;
			}
		}

		internal static Callback<TableColumn<T, string>, TableCell<T, string>> getStringCallback<T>()
		{
			get
			{
				return new CallbackAnonymousInnerClass4();
			}
		}

		private class CallbackAnonymousInnerClass4 : Callback<TableColumn<T, string>, TableCell<T, string>>
		{
			public override TableCell<T, string> call(TableColumn<T, string> cell)
			{
				TableCell<T, string> tableCell = new EditableCell<T, string>(new EditableStringCellConverter());
				tableCell.setAlignment(Pos.CENTER_LEFT);
				return tableCell;
			}
		}

		internal static Callback<TableColumn<T, bool>, TableCell<T, bool>> getBooleanCallback<T>()
		{
			get
			{
				return new CallbackAnonymousInnerClass5();
			}
		}

		private class CallbackAnonymousInnerClass5 : Callback<TableColumn<T, bool>, TableCell<T, bool>>
		{
			public override TableCell<T, bool> call(TableColumn<T, bool> cell)
			{
				TableCell<T, bool> tableCell = new CheckBoxTableCell<T, bool>();
				tableCell.setAlignment(Pos.CENTER);
				return tableCell;
			}
		}

		internal virtual void addContextMenu(TableView<T> table, ContextMenu contextMenu)
		{
			table.setRowFactory(new CallbackAnonymousInnerClass6(this, contextMenu));
		}

		private class CallbackAnonymousInnerClass6 : Callback<TableView<T>, TableRow<T>>
		{
			private readonly UITableBuilder<T> outerInstance;

			private ContextMenu contextMenu;

			public CallbackAnonymousInnerClass6(UITableBuilder<T> outerInstance, ContextMenu contextMenu)
			{
				this.outerInstance = outerInstance;
				this.contextMenu = contextMenu;
			}

			public override TableRow<T> call(TableView<T> tableView)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javafx.scene.control.TableRow<T> row = new javafx.scene.control.TableRow<T>()
				TableRow<T> row = new TableRowAnonymousInnerClass(this);
				// Set context menu on row, but use a binding to make it only show for non-empty rows:  
				row.contextMenuProperty().bind(Bindings.when(row.emptyProperty()).then((ContextMenu)null).otherwise(contextMenu));
				return row;
			}

			private class TableRowAnonymousInnerClass : TableRow<T>
			{
				private readonly CallbackAnonymousInnerClass6 outerInstance;

				public TableRowAnonymousInnerClass(CallbackAnonymousInnerClass6 outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public override void updateItem(T item, bool empty)
				{
					base.updateItem(item, empty);
					// highlight current row
					outerInstance.outerInstance.highlightTableRow(this);
				}
			}


		}

		internal virtual void addDynamicRowAdder(TableView<T> table)
		{
			table.setOnMouseClicked(new EventHandlerAnonymousInnerClass(this, table));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<MouseEvent>
		{
			private readonly UITableBuilder<T> outerInstance;

			private TableView<T> table;

			public EventHandlerAnonymousInnerClass(UITableBuilder<T> outerInstance, TableView<T> table)
			{
				this.outerInstance = outerInstance;
				this.table = table;
			}

			public override void handle(MouseEvent @event)
			{

				if (@event.getButton().Equals(MouseButton.PRIMARY) && @event.getSource() == table)
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: if (event.getTarget() instanceof javafx.scene.control.TableCell && table.getItems() != null && ((javafx.scene.control.TableCell<?, ?>)event.getTarget()).getIndex() >= table.getItems().size())
					if (@event.getTarget() is TableCell && table.getItems() != null && ((TableCell<object, object>)@event.getTarget()).getIndex() >= table.getItems().size())
					{
						table.getSelectionModel().clearAndSelect(table.getItems().size() - 1);
					}
					else if (@event.getTarget() is TableCell && table.getItems() != null && table.getItems().size() == table.getSelectionModel().getSelectedIndex() + 1)
					{
						table.getItems().add(outerInstance.EmptyRow);
						//						table.scrollTo(emptyRow);
						//						table.getSelectionModel().clearAndSelect(table.getItems().size() - 1);
						//						table.getSelectionModel().select(emptyRow);

					}
				}
			}
		}

		internal virtual void addColumnOrderSequenceListeners(TableContentType tableContentType, TableView<T> table)
		{
			if (tableContentType != TableContentType.UNSPECIFIC)
			{
				ObservableList<ColumnContentType> sortOrder = ColumnPropertiesManager.Instance.getSortOrder(tableContentType);
				ObservableList<ColumnContentType> columnsOrder = ColumnPropertiesManager.Instance.getColumnsOrder(tableContentType);
				this.setSortOrderSequence(table, sortOrder);
				this.setColumnsOrderSequence(table, columnsOrder);
				table.getSortOrder().addListener(new SortOrderChangeListener(this));
				table.getColumns().addListener(new ColumnsOrderChangeListener(this));
				sortOrder.addListener(new SortOrderSequenceChangeListener(this, table));
				columnsOrder.addListener(new ColumnsOrderSequenceChangeListener(this, table));
			}
		}

		internal virtual TableColumn<T, S> getColumn<S>(ColumnTooltipHeader header, System.Func<T, ObservableValue<S>> property, Callback<TableColumn<T, S>, TableCell<T, S>> callback, ColumnType type, int columnIndex, bool editable)
		{
			return getColumn(TableContentType.UNSPECIFIC, ColumnContentType.DEFAULT, header, property, callback, type, columnIndex, editable, false);
		}

		internal virtual TableColumn<T, S> getColumn<S>(TableContentType tableType, ColumnContentType columnType, ColumnTooltipHeader header, System.Func<T, ObservableValue<S>> property, Callback<TableColumn<T, S>, TableCell<T, S>> callback, ColumnType type, int columnIndex, bool editable, bool reorderable)
		{
			ColumnPropertiesManager columnPropertiesManager = ColumnPropertiesManager.Instance;
			ColumnProperty columnProperty = columnPropertiesManager.getProperty(tableType, columnType);
			columnProperty.ColumnOrder = columnIndex;
			columnProperty.DefaultColumnOrder = columnIndex;
			// Sets width properties within columnProperty
			ContentColumn<T, S> column = new ContentColumn<T, S>(columnProperty);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellEvent<S> tableCellEvent = new TableCellEvent<S>(columnIndex);
			TableCellEvent<S> tableCellEvent = new TableCellEvent<S>(this, columnIndex);
			Label columnLabel = header.Label;
			columnLabel.getStyleClass().add("column-header-label");
			columnLabel.setMaxWidth(double.MaxValue);
			columnLabel.setTooltip(header.Tooltip);
			column.setUserData(type);
			column.setEditable(editable);
			column.setReorderable(reorderable);
			column.setGraphic(columnLabel);

			column.setCellValueFactory(new CallbackAnonymousInnerClass7(this, property));
			column.setCellFactory(callback);
			column.setOnEditCommit(tableCellEvent);
			return column;
		}

		private class CallbackAnonymousInnerClass7 : Callback<TableColumn.CellDataFeatures<T, S>, ObservableValue<S>>
		{
			private readonly UITableBuilder<T> outerInstance;

			private System.Func<T, ObservableValue<S>> property;

			public CallbackAnonymousInnerClass7(UITableBuilder<T> outerInstance, System.Func<T, ObservableValue<S>> property)
			{
				this.outerInstance = outerInstance;
				this.property = property;
			}

			public override ObservableValue<S> call(TableColumn.CellDataFeatures<T, S> param)
			{
				ObservableValue<S> observableValue = property(param.getValue());
				return observableValue;
			}
		}

		public virtual void refreshTable()
		{
			if (this.table != null)
			{
				this.table.refresh();
			}
		}

		internal virtual void highlightTableRow(TableRow<T> row)
		{
		}

		internal virtual void setTableRowHighlight(TableRow<T> row, TableRowHighlightRangeType type)
		{
			Color color = TableRowHighlight.Instance.getColor(type);
			Color darkerColor = color.darker();

			string rgbColor = String.format(Locale.ENGLISH, "rgb(%.0f, %.0f, %.0f)", color.getRed() * 255, color.getGreen() * 255, color.getBlue() * 255);
			string rgbDarkerColor = String.format(Locale.ENGLISH, "rgb(%.0f, %.0f, %.0f)", darkerColor.getRed() * 255, darkerColor.getGreen() * 255, darkerColor.getBlue() * 255);

			row.setStyle(color != null && color != Color.TRANSPARENT ? string.Format("-fx-background-color: {0};", row.getIndex() % 2 == 0 ? rgbColor : rgbDarkerColor) : "");
		}

		public abstract T EmptyRow {get;}

		internal abstract void setValue(T row, int columnIndex, object oldValue, object newValue);

		public virtual TableView<T> Table
		{
			get
			{
				return this.table;
			}
		}

		public static void copySelectionToClipboard<T1>(TableView<T1> table)
		{
			StringBuilder clipboardString = new StringBuilder();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (javafx.scene.control.TablePosition<?,?> position : table.getSelectionModel().getSelectedCells())
			foreach (TablePosition<object, object> position in table.getSelectionModel().getSelectedCells())
			{
				int rowIndex = position.getRow();

				if (rowIndex < 0 || rowIndex >= table.getItems().size())
				{
					continue;
				}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (javafx.scene.control.TableColumn<?, ?> column : table.getColumns())
				foreach (TableColumn<object, object> column in table.getColumns())
				{
					if (!column.isVisible())
					{
						continue;
					}

					object cell = column.getCellData(rowIndex);

					if (cell == null)
					{
						cell = " ";
					}

					clipboardString.Append(cell.ToString()).Append('\t');
				}
				clipboardString.AppendLine();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javafx.scene.input.ClipboardContent clipboardContent = new javafx.scene.input.ClipboardContent();
			ClipboardContent clipboardContent = new ClipboardContent();
			clipboardContent.putString(clipboardString.ToString());
			Clipboard.getSystemClipboard().setContent(clipboardContent);
		}

		public static object getValueAt<T1>(TableView<T1> table, int columnIndex, int rowIndex)
		{
			return table.getColumns().get(columnIndex).getCellObservableValue(rowIndex).getValue();
		}

		private void setSortOrderSequence<T1>(TableView<T> table, IList<T1> columnTypes) where T1 : org.applied_geodesy.jag3d.ui.table.column.ColumnContentType
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<javafx.scene.control.TableColumn<T, ?>> sortedColumns = new java.util.ArrayList<javafx.scene.control.TableColumn<T, ?>>();
			IList<TableColumn<T, object>> sortedColumns = new List<TableColumn<T, object>>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<javafx.scene.control.TableColumn<T, ?>> columns = new java.util.ArrayList<javafx.scene.control.TableColumn<T, ?>>(table.getColumns());
			IList<TableColumn<T, object>> columns = new List<TableColumn<T, object>>(table.getColumns());

			// Reset sort order
			if (columnTypes.Count == 0)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (javafx.scene.control.TableColumn<T, ?> column : columns)
				foreach (TableColumn<T, object> column in columns)
				{
					if (column != null && (column is ContentColumn))
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T, ?>)column).getColumnProperty().setSortOrder(-1);
						((ContentColumn<T, object>)column).ColumnProperty.SortOrder = -1;
					}
				}
			}

			int idx = 0;
			foreach (ColumnContentType columnType in columnTypes)
			{
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.ListIterator<javafx.scene.control.TableColumn<T, ?>> columnIterator = columns.listIterator();
				IEnumerator<TableColumn<T, object>> columnIterator = columns.GetEnumerator();
				while (columnIterator.MoveNext())
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TableColumn<T, ?> column = columnIterator.Current;
					TableColumn<T, object> column = columnIterator.Current;
					if (column is ContentColumn)
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: org.applied_geodesy.jag3d.ui.table.column.ColumnContentType currentColumnType = ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T,?>)column).getColumnProperty().getColumnContentType();
						ColumnContentType currentColumnType = ((ContentColumn<T, object>)column).ColumnProperty.ColumnContentType;
						if (currentColumnType == columnType)
						{
							sortedColumns.Add(column);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T,?>)column).getColumnProperty().setSortOrder(idx++);
							((ContentColumn<T, object>)column).ColumnProperty.SortOrder = idx++;
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
							columnIterator.remove();
							break;
						}
					}
				}
			}

			table.getSortOrder().setAll(sortedColumns);
		}

		private void setColumnsOrderSequence<T1>(TableView<T> table, IList<T1> columnTypes) where T1 : org.applied_geodesy.jag3d.ui.table.column.ColumnContentType
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<javafx.scene.control.TableColumn<T, ?>> orderedColumns = new java.util.ArrayList<javafx.scene.control.TableColumn<T, ?>>();
			IList<TableColumn<T, object>> orderedColumns = new List<TableColumn<T, object>>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<javafx.scene.control.TableColumn<T, ?>> columns = new java.util.ArrayList<javafx.scene.control.TableColumn<T, ?>>(table.getColumns());
			IList<TableColumn<T, object>> columns = new List<TableColumn<T, object>>(table.getColumns());

			// Reset column order
			if (columnTypes.Count == 0)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (javafx.scene.control.TableColumn<T, ?> column : columns)
				foreach (TableColumn<T, object> column in columns)
				{
					if (column != null && (column is ContentColumn))
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: int defaultValue = ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T, ?>)column).getColumnProperty().getDefaultColumnOrder();
						int defaultValue = ((ContentColumn<T, object>)column).ColumnProperty.DefaultColumnOrder;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T, ?>)column).getColumnProperty().setColumnOrder(defaultValue);
						((ContentColumn<T, object>)column).ColumnProperty.ColumnOrder = defaultValue;
					}
				}
				columns.Sort(new ComparatorAnonymousInnerClass(this));
			}

			int idx = 0;
			foreach (ColumnContentType columnType in columnTypes)
			{
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.ListIterator<javafx.scene.control.TableColumn<T, ?>> columnIterator = columns.listIterator();
				IEnumerator<TableColumn<T, object>> columnIterator = columns.GetEnumerator();
				while (columnIterator.MoveNext())
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TableColumn<T, ?> column = columnIterator.Current;
					TableColumn<T, object> column = columnIterator.Current;
					if (column is ContentColumn)
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: org.applied_geodesy.jag3d.ui.table.column.ColumnContentType currentColumnType = ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T,?>)column).getColumnProperty().getColumnContentType();
						ColumnContentType currentColumnType = ((ContentColumn<T, object>)column).ColumnProperty.ColumnContentType;
						if (currentColumnType == columnType)
						{
							orderedColumns.Add(column);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T,?>)column).getColumnProperty().setColumnOrder(idx++);
							((ContentColumn<T, object>)column).ColumnProperty.ColumnOrder = idx++;
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
							columnIterator.remove();
							break;
						}
					}
				}
			}
			// Add columns, which are not part of columnTypes list, i.e. if meanwhile,
			// a new column is defined to the table not stored to the database)
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: orderedColumns.addAll(columns);
			((List<TableColumn<T, object>>)orderedColumns).AddRange(columns);
			table.getColumns().setAll(orderedColumns);
		}

		private class ComparatorAnonymousInnerClass : IComparer<TableColumn<T, JavaToDotNetGenericWildcard>>
		{
			private readonly UITableBuilder<T> outerInstance;

			public ComparatorAnonymousInnerClass(UITableBuilder<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public int compare<T1, T2>(TableColumn<T1> column1, TableColumn<T2> column2)
			{
				if (column1 == null || !(column1 is ContentColumn))
				{
					return -1;
				}
				if (column2 == null || !(column2 is ContentColumn))
				{
					return +1;
				}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: int defaultValue1 = ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T, ?>)column1).getColumnProperty().getDefaultColumnOrder();
				int defaultValue1 = ((ContentColumn<T, object>)column1).ColumnProperty.DefaultColumnOrder;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: int defaultValue2 = ((org.applied_geodesy.jag3d.ui.table.column.ContentColumn<T, ?>)column2).getColumnProperty().getDefaultColumnOrder();
				int defaultValue2 = ((ContentColumn<T, object>)column2).ColumnProperty.DefaultColumnOrder;
				return defaultValue1 - defaultValue2;
			}
		}
	}
}