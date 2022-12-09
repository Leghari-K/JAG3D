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

namespace org.applied_geodesy.juniform.ui.table
{

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
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using EventHandler = javafx.@event.EventHandler;
	using Pos = javafx.geometry.Pos;
	using Label = javafx.scene.control.Label;
	using ListView = javafx.scene.control.ListView;
	using SelectionMode = javafx.scene.control.SelectionMode;
	using TableCell = javafx.scene.control.TableCell;
	using TableColumn = javafx.scene.control.TableColumn;
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
			this.table.getSelectionModel().setSelectionMode(SelectionMode.MULTIPLE);
			this.table.setOnKeyPressed(new TableKeyEventHandler(this));

			ListView<string> placeholderList = new ListView<string>();
			placeholderList.getItems().add("");
			placeholderList.setDisable(true);
			this.table.setPlaceholder(placeholderList);

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

		internal virtual TableColumn<T, S> getColumn<S>(ColumnTooltipHeader header, System.Func<T, ObservableValue<S>> property, Callback<TableColumn<T, S>, TableCell<T, S>> callback, ColumnType type, int columnIndex, bool editable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TableCellEvent<S> tableCellEvent = new TableCellEvent<S>(columnIndex);
			TableCellEvent<S> tableCellEvent = new TableCellEvent<S>(this, columnIndex);
			TableColumn<T, S> column = new TableColumn<T, S>();
			Label columnLabel = header.Label;
			columnLabel.getStyleClass().add("column-header-label");
			columnLabel.setMaxWidth(double.MaxValue);
			columnLabel.setTooltip(header.Tooltip);

			column.setUserData(type);
			column.setEditable(editable);
			column.setGraphic(columnLabel);
			column.setMinWidth(25);
			column.setPrefWidth(125);
			column.setCellValueFactory(new CallbackAnonymousInnerClass6(this, property));
			column.setCellFactory(callback);
			column.setOnEditCommit(tableCellEvent);
			return column;
		}

		private class CallbackAnonymousInnerClass6 : Callback<TableColumn.CellDataFeatures<T, S>, ObservableValue<S>>
		{
			private readonly UITableBuilder<T> outerInstance;

			private System.Func<T, ObservableValue<S>> property;

			public CallbackAnonymousInnerClass6(UITableBuilder<T> outerInstance, System.Func<T, ObservableValue<S>> property)
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
	}
}