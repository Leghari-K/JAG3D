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

namespace org.applied_geodesy.jag3d.ui.table
{

	using DefaultApplicationProperty = org.applied_geodesy.jag3d.DefaultApplicationProperty;
	using GroupRow = org.applied_geodesy.jag3d.ui.table.row.GroupRow;
	using Groupable = org.applied_geodesy.jag3d.ui.tree.Groupable;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;

	using Platform = javafx.application.Platform;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using ButtonType = javafx.scene.control.ButtonType;
	using ContextMenu = javafx.scene.control.ContextMenu;
	using Menu = javafx.scene.control.Menu;
	using MenuItem = javafx.scene.control.MenuItem;
	using SeparatorMenuItem = javafx.scene.control.SeparatorMenuItem;
	using TableView = javafx.scene.control.TableView;
	using TreeItem = javafx.scene.control.TreeItem;
	using KeyCode = javafx.scene.input.KeyCode;
	using KeyEvent = javafx.scene.input.KeyEvent;

	public abstract class UIEditableTableBuilder<T> : UITableBuilder<T> where T : org.applied_geodesy.jag3d.ui.table.row.GroupRow
	{
		private bool InstanceFieldsInitialized = false;

		public UIEditableTableBuilder()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			listener = new ContextMenuEventHandler(this);
		}


		internal enum ContextMenuType
		{
			REMOVE,
			DUPLICATE,
			MOVETO,

			SELECT_GROUPS,

			MOVETO_NEW,
			MOVETO_REFERENCE,
			MOVETO_STOCHASTIC,
			MOVETO_DATUM
		}

		private class ContextMenuEventHandler : EventHandler<ActionEvent>
		{
			private readonly UIEditableTableBuilder<T> outerInstance;

			public ContextMenuEventHandler(UIEditableTableBuilder<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				if (@event.getSource() is MenuItem && ((MenuItem)@event.getSource()).getUserData() is ContextMenuType)
				{
					ContextMenuType contextMenuType = (ContextMenuType)((MenuItem)@event.getSource()).getUserData();
					switch (contextMenuType)
					{
					case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.REMOVE:
						outerInstance.removeTableRows();
						break;
					case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.SELECT_GROUPS:
						outerInstance.selectGroups();
						goto case MOVETO;
					case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO:
					case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_REFERENCE:
					case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_STOCHASTIC:
					case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_DATUM:
					case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_NEW:
						outerInstance.moveRows(contextMenuType);
						break;
					case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.DUPLICATE:
						outerInstance.duplicateRows();
						break;
					}
				}
			}
		}

		private ContextMenuEventHandler listener;

		internal override TableView<T> createTable()
		{
			this.table = base.createTable();
			this.table.setEditable(true);
			this.enableDragSupport();
			this.addTableKeyEvents();

			return this.table;
		}

		internal virtual ContextMenu createContextMenu(bool isPointTable)
		{
			MenuItem removeMenuItem = this.createMenuItem(i18n.getString("UIEditableTableBuilder.contextmenu.remove", "Remove selected items"), ContextMenuType.REMOVE, listener);

			MenuItem duplicateMenuItem = this.createMenuItem(i18n.getString("UIEditableTableBuilder.contextmenu.duplicate", "Duplicate selected items"), ContextMenuType.DUPLICATE, listener);

			MenuItem selectGroupsMenuItem = this.createMenuItem(i18n.getString("UIEditableTableBuilder.contextmenu.select", "Select item groups"), ContextMenuType.SELECT_GROUPS, listener);

			ContextMenu contextMenu = new ContextMenu(removeMenuItem, duplicateMenuItem);

			if (!isPointTable)
			{
				MenuItem moveToMenuItem = this.createMenuItem(i18n.getString("UIEditableTableBuilder.contextmenu.moveto", "Move selected items"), ContextMenuType.MOVETO, listener);
				contextMenu.getItems().add(moveToMenuItem);
			}
			else
			{
				Menu moveToMenu = this.createMenu(i18n.getString("UIEditableTableBuilder.contextmenu.moveto", "Move selected items"));

				MenuItem moveToReferenceMenuItem = this.createMenuItem(i18n.getString("UIEditableTableBuilder.contextmenu.moveto.reference", "Reference point group"), ContextMenuType.MOVETO_REFERENCE, listener);

				MenuItem moveToStochasticMenuItem = this.createMenuItem(i18n.getString("UIEditableTableBuilder.contextmenu.moveto.stochastic", "Stochastic point group"), ContextMenuType.MOVETO_STOCHASTIC, listener);

				MenuItem moveToDatumMenuItem = this.createMenuItem(i18n.getString("UIEditableTableBuilder.contextmenu.moveto.datum", "Datum point group"), ContextMenuType.MOVETO_DATUM, listener);

				MenuItem moveToNewMenuItem = this.createMenuItem(i18n.getString("UIEditableTableBuilder.contextmenu.moveto.new", "New point group"), ContextMenuType.MOVETO_NEW, listener);



				moveToMenu.getItems().addAll(moveToReferenceMenuItem, moveToStochasticMenuItem, moveToDatumMenuItem, moveToNewMenuItem);
				contextMenu.getItems().add(moveToMenu);
			}

			contextMenu.getItems().addAll(new SeparatorMenuItem(), selectGroupsMenuItem);
			return contextMenu;
		}

		internal virtual void raiseErrorMessage(ContextMenuType type, Exception e)
		{
			Platform.runLater(() =>
			{
			switch (type)
			{
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.DUPLICATE:
				OptionDialog.showThrowableDialog(i18n.getString("UIEditableTableBuilder.message.error.duplicate.exception.title", "Unexpected SQL-Error"), i18n.getString("UIEditableTableBuilder.message.error.duplicate.exception.header", "Error, could not save copies of selected items."), i18n.getString("UIEditableTableBuilder.message.error.duplicate.exception.message", "An exception has occurred during database transaction."), e);
				break;
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.REMOVE:
				OptionDialog.showThrowableDialog(i18n.getString("UIEditableTableBuilder.message.error.remove.exception.title", "Unexpected SQL-Error"), i18n.getString("UIEditableTableBuilder.message.error.remove.exception.header", "Error, could not remove selected items."), i18n.getString("UIEditableTableBuilder.message.error.remove.exception.message", "An exception has occurred during database transaction."), e);
				break;
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO:
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_DATUM:
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_NEW:
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_REFERENCE:
			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.MOVETO_STOCHASTIC:
				OptionDialog.showThrowableDialog(i18n.getString("UIEditableTableBuilder.message.error.moveto.exception.title", "Unexpected SQL-Error"), i18n.getString("UIEditableTableBuilder.message.error.moveto.exception.header", "Error, could not move selected items to new table."), i18n.getString("UIEditableTableBuilder.message.error.moveto.exception.message", "An exception has occurred during database transaction."), e);
				break;

			case org.applied_geodesy.jag3d.ui.table.UIEditableTableBuilder.ContextMenuType.SELECT_GROUPS:
				break;
			}
			});
		}

		internal virtual void raiseErrorMessageSaveValue(Exception e)
		{
			Platform.runLater(() =>
			{
			OptionDialog.showThrowableDialog(i18n.getString("UIEditableTableBuilder.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("UIEditableTableBuilder.message.error.save.exception.header", "Error, could not save value to database."), i18n.getString("UIEditableTableBuilder.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
			});
		}

		private MenuItem createMenuItem(string label, ContextMenuType type, ContextMenuEventHandler listener)
		{
			MenuItem item = new MenuItem(label);
			item.setUserData(type);
			item.setOnAction(listener);
			return item;
		}

		private Menu createMenu(string label)
		{
			Menu menu = new Menu(label);
			return menu;
		}

		internal abstract void enableDragSupport();
		internal abstract void removeRows();
		internal abstract void duplicateRows();
		internal abstract void moveRows(ContextMenuType type);
		private void selectGroups()
		{
			try
			{
				IList<GroupRow> selectedRows = new List<GroupRow>(this.table.getSelectionModel().getSelectedItems());
				ISet<int> groupIds = new HashSet<int>();
				foreach (GroupRow selectedRow in selectedRows)
				{
					groupIds.Add(selectedRow.GroupId);
				}

				if (groupIds == null || groupIds.Count == 0)
				{
					return;
				}

				IList<TreeItem<TreeItemValue>> selectedTreeItems = new List<TreeItem<TreeItemValue>>(UITreeBuilder.Instance.Tree.getSelectionModel().getSelectedItems());
				if (selectedTreeItems.Count > 1)
				{
					IList<int> selectedTreeItemIndices = new List<int>(UITreeBuilder.Instance.Tree.getSelectionModel().getSelectedIndices());
					int[] indices = new int[groupIds.Count];

					for (int i = 0, j = 0; i < selectedTreeItems.Count; i++)
					{
						TreeItem<TreeItemValue> selectedTreeItem = selectedTreeItems[i];
						if (selectedTreeItem.getValue() is Groupable && groupIds.Contains(((Groupable)selectedTreeItem.getValue()).GroupId))
						{
							indices[j++] = selectedTreeItemIndices[i];
							if (j == indices.Length) // check against IndexOutOfBoundException
							{
								break;
							}
						}
					}
					Platform.runLater(() =>
					{
					UITreeBuilder.Instance.Tree.getSelectionModel().clearSelection();
					UITreeBuilder.Instance.Tree.getSelectionModel().selectIndices(indices[0], indices);
					});
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		private void removeTableRows()
		{
			IList<GroupRow> selectedRows = new List<GroupRow>(this.table.getSelectionModel().getSelectedItems());
			if (selectedRows == null || selectedRows.Count == 0)
			{
				return;
			}

			bool isDeleteConfirmed = !DefaultApplicationProperty.showConfirmDialogOnDelete();
			if (!isDeleteConfirmed)
			{
				Optional<ButtonType> result = OptionDialog.showConfirmationDialog(i18n.getString("UIEditableTableBuilder.message.confirmation.delete.title", "Delete items"), i18n.getString("UIEditableTableBuilder.message.confirmation.delete.header", "Delete items permanently?"), i18n.getString("UIEditableTableBuilder.message.confirmation.delete.message", "Are you sure you want to remove the selected items?"));
				isDeleteConfirmed = result.isPresent() && result.get() == ButtonType.OK;
			}

			if (isDeleteConfirmed)
			{
				removeRows();
			}
		}

		private void addTableKeyEvents()
		{
			this.table.addEventFilter(KeyEvent.KEY_PRESSED, new EventHandlerAnonymousInnerClass(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<KeyEvent>
		{
			private readonly UIEditableTableBuilder<T> outerInstance;

			public EventHandlerAnonymousInnerClass(UIEditableTableBuilder<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void handle(KeyEvent keyEvent)
			{
				if (keyEvent.getSource() == outerInstance.table && keyEvent.getTarget() == outerInstance.table && keyEvent.getCode() == KeyCode.DELETE)
				{
					outerInstance.removeTableRows();
					keyEvent.consume();
				}
			}
		}
	}

}