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

namespace org.applied_geodesy.juniform.ui.tree
{
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using TextField = javafx.scene.control.TextField;
	using TreeCell = javafx.scene.control.TreeCell;
	using TreeItem = javafx.scene.control.TreeItem;
	using TreeView = javafx.scene.control.TreeView;
	using KeyCode = javafx.scene.input.KeyCode;
	using KeyEvent = javafx.scene.input.KeyEvent;

	public class EditableMenuTreeCell : TreeCell<TreeItemValue<JavaToDotNetGenericWildcard>>
	{
		private TextField textField;

		public override void commitEdit<T1>(TreeItemValue<T1> item)
		{
			if (!this.isEditing() && !item.Equals(this.getItem()))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javafx.scene.control.TreeItem<TreeItemValue<?>> treeItem = getTreeItem();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				TreeItem<TreeItemValue<object>> treeItem = getTreeItem();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javafx.scene.control.TreeView<TreeItemValue<?>> treeView = getTreeView();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				TreeView<TreeItemValue<object>> treeView = getTreeView();
				if (treeView != null)
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: treeView.fireEvent(new javafx.scene.control.TreeView.EditEvent<TreeItemValue<?>>(treeView, javafx.scene.control.TreeView.editCommitEvent<TreeItemValue<?>> (), treeItem, getItem(), item));
					treeView.fireEvent(new TreeView.EditEvent<TreeItemValue<object>>(treeView, TreeView.editCommitEvent<TreeItemValue<object>> (), treeItem, getItem(), item));
				}
			}
			base.commitEdit(item);
		}

		public override void startEdit()
		{
			base.startEdit();

			if (this.textField == null)
			{
				this.createTextField();
			}

			if (this.getItem() != null)
			{
				if (this.getItem().getObject() is GeometricPrimitive)
				{
					this.setText(null);
					this.setGraphic(this.textField);
					this.textField.setText(this.String);
					Platform.runLater(() =>
					{
					textField.selectAll();
					textField.requestFocus();
					});
				}
			}
		}

		public override void cancelEdit()
		{
			base.cancelEdit();
			this.setText(this.getItem().ToString());
			this.setGraphic(this.getTreeItem().getGraphic());
		}

		public override void updateItem<T1>(TreeItemValue<T1> item, bool empty)
		{
			base.updateItem(item, empty);

			if (empty)
			{
				setText(null);
			}
			else
			{
				if (isEditing())
				{
					// Editierbare Nodes sind alle Leafs, vgl. startEditing    		      		        		
					if (this.textField != null)
					{
						this.textField.setText(this.String);
					}
					this.setText(null);
					this.setGraphic(this.textField);
				}
				else
				{
					this.setText(this.String);
					this.setGraphic(this.getTreeItem().getGraphic());
				}
			}
		}

		private void createTextField()
		{
			this.textField = new TextField("");

			this.textField.setOnAction(new EventHandlerAnonymousInnerClass(this));

			this.textField.focusedProperty().addListener(new ChangeListenerAnonymousInnerClass(this));

			this.textField.addEventFilter(KeyEvent.KEY_PRESSED, new EventHandlerAnonymousInnerClass2(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<ActionEvent>
		{
			private readonly EditableMenuTreeCell outerInstance;

			public EventHandlerAnonymousInnerClass(EditableMenuTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				if (getTreeItem() != null)
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: TreeItemValue<?> item = getTreeItem().getValue();
					TreeItemValue<object> item = getTreeItem().getValue();
					item.Name = outerInstance.textField.getText();
					outerInstance.commitEdit(item);
				}
			}
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<bool>
		{
			private readonly EditableMenuTreeCell outerInstance;

			public ChangeListenerAnonymousInnerClass(EditableMenuTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if ((!newValue).Value)
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: TreeItemValue<?> item = getTreeItem().getValue();
					TreeItemValue<object> item = getTreeItem().getValue();
					item.Name = outerInstance.textField.getText();
					outerInstance.commitEdit(item);
				}
			}
		}

		private class EventHandlerAnonymousInnerClass2 : EventHandler<KeyEvent>
		{
			private readonly EditableMenuTreeCell outerInstance;

			public EventHandlerAnonymousInnerClass2(EditableMenuTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(KeyEvent @event)
			{
				if (@event.getCode() == KeyCode.ESCAPE)
				{
					outerInstance.textField.setText(outerInstance.String);
					outerInstance.cancelEdit();
					@event.consume();
				}
			}
		}

		private string String
		{
			get
			{
				return this.getItem() == null ? "" : this.getItem().ToString();
			}
		}
	}

}