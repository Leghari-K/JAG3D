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

namespace org.applied_geodesy.ui.table
{

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ActionEvent = javafx.@event.ActionEvent;
	using Event = javafx.@event.Event;
	using EventHandler = javafx.@event.EventHandler;
	using ContentDisplay = javafx.scene.control.ContentDisplay;
	using TableCell = javafx.scene.control.TableCell;
	using TableColumn = javafx.scene.control.TableColumn;
	using CellEditEvent = javafx.scene.control.TableColumn.CellEditEvent;
	using TablePosition = javafx.scene.control.TablePosition;
	using TableView = javafx.scene.control.TableView;
	using TextField = javafx.scene.control.TextField;
	using KeyCode = javafx.scene.input.KeyCode;
	using KeyEvent = javafx.scene.input.KeyEvent;


	public class EditableCell<T, S> : TableCell<T, S>
	{

		private readonly TextField textField = new TextField();
		private readonly EditableCellConverter<S> converter;

		public EditableCell(EditableCellConverter<S> converter)
		{
			this.converter = converter;
			this.converter.EditableCell = this;

			this.init();
		}

		public virtual EditableCellConverter<S> EditableCellConverter
		{
			get
			{
				return this.converter;
			}
		}

		private void init()
		{
			this.itemProperty().addListener(new ChangeListenerAnonymousInnerClass(this));

			this.setGraphic(this.textField);
			this.setContentDisplay(ContentDisplay.TEXT_ONLY);

			this.textField.setOnAction(new EventHandlerAnonymousInnerClass(this));

			this.textField.focusedProperty().addListener(new ChangeListenerAnonymousInnerClass2(this));

			this.textField.addEventFilter(KeyEvent.KEY_PRESSED, new EventHandlerAnonymousInnerClass2(this));
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<S>
		{
			private readonly EditableCell<T, S> outerInstance;

			public ChangeListenerAnonymousInnerClass(EditableCell<T, S> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, S oldValue, S newValue) where T1 : S
			{
				setText(newValue == null ? null : outerInstance.converter.toString(newValue));
			}
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<ActionEvent>
		{
			private readonly EditableCell<T, S> outerInstance;

			public EventHandlerAnonymousInnerClass(EditableCell<T, S> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				outerInstance.commitEdit(outerInstance.converter.fromString(outerInstance.textField.getText()));
			}
		}

		private class ChangeListenerAnonymousInnerClass2 : ChangeListener<bool>
		{
			private readonly EditableCell<T, S> outerInstance;

			public ChangeListenerAnonymousInnerClass2(EditableCell<T, S> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if ((!newValue).Value)
				{
					outerInstance.commitEdit(outerInstance.converter.fromString(outerInstance.textField.getText()));
				}
			}
		}

		private class EventHandlerAnonymousInnerClass2 : EventHandler<KeyEvent>
		{
			private readonly EditableCell<T, S> outerInstance;

			public EventHandlerAnonymousInnerClass2(EditableCell<T, S> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(KeyEvent @event)
			{
				if (@event.getCode() == KeyCode.ESCAPE)
				{
					outerInstance.textField.setText(outerInstance.converter.toEditorString(getItem()));
					outerInstance.cancelEdit();
					@event.consume();
				}

				else if (@event.getCode() == KeyCode.TAB)
				{
					outerInstance.commitEdit(outerInstance.converter.fromString(outerInstance.textField.getText()));
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TableColumn<T, ?> nextColumn = getNextColumn(!event.isShiftDown());
					TableColumn<T, object> nextColumn = outerInstance.getNextColumn(!@event.isShiftDown());
					TableView<T> table = getTableView();
					if (table != null && nextColumn != null)
					{
						table.requestFocus();
						table.getSelectionModel().select(getTableRow().getIndex(), nextColumn);
						table.edit(getTableRow().getIndex(), nextColumn);
					}
					@event.consume();
				}
			}
		}

		public override void startEdit()
		{
			base.startEdit();
			this.textField.setText(this.converter.toEditorString(getItem()));
			this.setContentDisplay(ContentDisplay.GRAPHIC_ONLY);
			this.textField.requestFocus();
			this.textField.selectAll();
		}

		public override void cancelEdit()
		{
			base.cancelEdit();
			this.setContentDisplay(ContentDisplay.TEXT_ONLY);
		}

		public override void commitEdit(S item)
		{
			if (item != null && getItem() != null && !this.isEditing() && !item.Equals(getItem()))
			{
				TableView<T> table = getTableView();
				if (table != null)
				{
					TableColumn<T, S> column = getTableColumn();
					TableColumn.CellEditEvent<T, S> @event = new TableColumn.CellEditEvent<T, S>(table, new TablePosition<T, S>(table, getIndex(), column), TableColumn.editCommitEvent(), item);
					Event.fireEvent(column, @event);
				}
			}
			base.commitEdit(item);
			this.setContentDisplay(ContentDisplay.TEXT_ONLY);
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.TableColumn<T, ?> getNextColumn(boolean forward)
		private TableColumn<T, object> getNextColumn(bool forward)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<javafx.scene.control.TableColumn<T, ?>> columns = new java.util.ArrayList<>();
			IList<TableColumn<T, object>> columns = new List<TableColumn<T, object>>();
			TableView<T> table = getTableView();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (javafx.scene.control.TableColumn<T, ?> column : table.getColumns())
			foreach (TableColumn<T, object> column in table.getColumns())
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: columns.addAll(getLeaves(column));
				((List<TableColumn<T, object>>)columns).AddRange(getLeaves(column));
			}

			if (columns.Count < 2)
			{
				return null;
			}

			int currentIndex = columns.IndexOf(getTableColumn());
			int nextIndex = currentIndex;

			if (forward)
			{
				nextIndex++;
				if (nextIndex > columns.Count - 1)
				{
					nextIndex = 0;
				}
			}
			else
			{
				nextIndex--;
				if (nextIndex < 0)
				{
					nextIndex = columns.Count - 1;
				}
			}

			return columns[nextIndex];
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private java.util.List<javafx.scene.control.TableColumn<T, ?>> getLeaves(javafx.scene.control.TableColumn<T, ?> root)
		private IList<TableColumn<T, object>> getLeaves<T1>(TableColumn<T1> root)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<javafx.scene.control.TableColumn<T, ?>> columns = new java.util.ArrayList<>();
			IList<TableColumn<T, object>> columns = new List<TableColumn<T, object>>();
			if (root.getColumns().isEmpty())
			{
				if (root.isEditable() && root.isVisible() && !(root.getCellObservableValue(0).getValue() is Boolean))
				{
					columns.Add(root);
				}
				return columns;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (javafx.scene.control.TableColumn<T, ?> column : root.getColumns())
				foreach (TableColumn<T, object> column in root.getColumns())
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: columns.addAll(getLeaves(column));
					((List<TableColumn<T, object>>)columns).AddRange(getLeaves(column));
				}
				return columns;
			}
		}
	}
}