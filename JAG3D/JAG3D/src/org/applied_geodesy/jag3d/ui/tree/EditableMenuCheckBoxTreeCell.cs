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

namespace org.applied_geodesy.jag3d.ui.tree
{

	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using ProjectDatabaseStateChangeListener = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateChangeListener;
	using ProjectDatabaseStateEvent = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateEvent;
	using ProjectDatabaseStateType = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using JAG3D = org.applied_geodesy.jag3d.ui.JAG3D;
	using InstrumentAndReflectorHeightAdaptionDialog = org.applied_geodesy.jag3d.ui.dialog.InstrumentAndReflectorHeightAdaptionDialog;
	using SearchAndReplaceDialog = org.applied_geodesy.jag3d.ui.dialog.SearchAndReplaceDialog;
	using CongruenceAnalysisRowDnD = org.applied_geodesy.jag3d.ui.dnd.CongruenceAnalysisRowDnD;
	using GNSSObservationRowDnD = org.applied_geodesy.jag3d.ui.dnd.GNSSObservationRowDnD;
	using GroupTreeItemDnD = org.applied_geodesy.jag3d.ui.dnd.GroupTreeItemDnD;
	using PointRowDnD = org.applied_geodesy.jag3d.ui.dnd.PointRowDnD;
	using TerrestrialObservationRowDnD = org.applied_geodesy.jag3d.ui.dnd.TerrestrialObservationRowDnD;
	using VerticalDeflectionRowDnD = org.applied_geodesy.jag3d.ui.dnd.VerticalDeflectionRowDnD;
	using UICongruenceAnalysisTableBuilder = org.applied_geodesy.jag3d.ui.table.UICongruenceAnalysisTableBuilder;
	using UIGNSSObservationTableBuilder = org.applied_geodesy.jag3d.ui.table.UIGNSSObservationTableBuilder;
	using UIPointTableBuilder = org.applied_geodesy.jag3d.ui.table.UIPointTableBuilder;
	using UITerrestrialObservationTableBuilder = org.applied_geodesy.jag3d.ui.table.UITerrestrialObservationTableBuilder;
	using UIVerticalDeflectionTableBuilder = org.applied_geodesy.jag3d.ui.table.UIVerticalDeflectionTableBuilder;
	using CongruenceAnalysisRow = org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow;
	using GNSSObservationRow = org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow;
	using PointRow = org.applied_geodesy.jag3d.ui.table.row.PointRow;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using VerticalDeflectionRow = org.applied_geodesy.jag3d.ui.table.row.VerticalDeflectionRow;
	using TabType = org.applied_geodesy.jag3d.ui.tabpane.TabType;
	using UITabPaneBuilder = org.applied_geodesy.jag3d.ui.tabpane.UITabPaneBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DefaultFileChooser = org.applied_geodesy.ui.io.DefaultFileChooser;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using CheckBox = javafx.scene.control.CheckBox;
	using ContextMenu = javafx.scene.control.ContextMenu;
	using Menu = javafx.scene.control.Menu;
	using MenuItem = javafx.scene.control.MenuItem;
	using RadioMenuItem = javafx.scene.control.RadioMenuItem;
	using SeparatorMenuItem = javafx.scene.control.SeparatorMenuItem;
	using TabPane = javafx.scene.control.TabPane;
	using TableView = javafx.scene.control.TableView;
	using TextField = javafx.scene.control.TextField;
	using Toggle = javafx.scene.control.Toggle;
	using ToggleGroup = javafx.scene.control.ToggleGroup;
	using TreeItem = javafx.scene.control.TreeItem;
	using TreeView = javafx.scene.control.TreeView;
	using CheckBoxTreeCell = javafx.scene.control.cell.CheckBoxTreeCell;
	using ClipboardContent = javafx.scene.input.ClipboardContent;
	using DataFormat = javafx.scene.input.DataFormat;
	using DragEvent = javafx.scene.input.DragEvent;
	using Dragboard = javafx.scene.input.Dragboard;
	using KeyCode = javafx.scene.input.KeyCode;
	using KeyEvent = javafx.scene.input.KeyEvent;
	using MouseEvent = javafx.scene.input.MouseEvent;
	using TransferMode = javafx.scene.input.TransferMode;
	using Text = javafx.scene.text.Text;

	public class EditableMenuCheckBoxTreeCell : CheckBoxTreeCell<TreeItemValue>
	{
		public static readonly DataFormat TREE_PARENT_ITEM_TYPE_DATA_FORMAT = new DataFormat(typeof(TreeItemType).ToString());
		public static readonly DataFormat TREE_ITEM_TYPE_DATA_FORMAT = new DataFormat(typeof(EditableMenuCheckBoxTreeCell).ToString());
		public static readonly DataFormat GROUP_ID_DATA_FORMAT = new DataFormat(typeof(TreeItemValue).ToString());
		public static readonly DataFormat DIMENSION_DATA_FORMAT = new DataFormat(typeof(Integer).ToString());
		public static readonly DataFormat TERRESTRIAL_OBSERVATION_ROWS_DATA_FORMAT = new DataFormat(typeof(TerrestrialObservationRowDnD).ToString());
		public static readonly DataFormat GNSS_OBSERVATION_ROWS_DATA_FORMAT = new DataFormat(typeof(GNSSObservationRowDnD).ToString());
		public static readonly DataFormat VERTICAL_DEFLECTION_ROWS_DATA_FORMAT = new DataFormat(typeof(VerticalDeflectionRowDnD).ToString());
		public static readonly DataFormat POINT_ROWS_DATA_FORMAT = new DataFormat(typeof(PointRowDnD).ToString());
		public static readonly DataFormat CONGRUENCE_ANALYSIS_ROWS_DATA_FORMAT = new DataFormat(typeof(CongruenceAnalysisRowDnD).ToString());
		private static readonly DataFormat TREE_ITEMS_DATA_FORMAT = new DataFormat(typeof(GroupTreeItemDnD).ToString());

		private I18N i18n = I18N.Instance;
		private BooleanProperty ignoreEvent = new SimpleBooleanProperty(false);
		private ContextMenu contextMenu;
		private TextField textField;
		private CheckBox checkBox;

		private enum ContextMenuType
		{
			ADD,
			REMOVE,
			EXPORT,
			MERGE,
			SEARCH_AND_REPLACE,
			ADAPT_INSTRUMENT_AND_REFLECTOR_HEIGHT,
			CHANGE_TO_REFERENCE_POINT_GROUP,
			CHANGE_TO_STOCHASTIC_POINT_GROUP,
			CHANGE_TO_DATUM_POINT_GROUP,
			CHANGE_TO_NEW_POINT_GROUP,
			CHANGE_TO_REFERENCE_DEFLECTION_GROUP,
			CHANGE_TO_STOCHASTIC_DEFLECTION_GROUP,
			CHANGE_TO_UNKNOWN_DEFLECTION_GROUP
		}

		private class DatabaseStateChangeListener : ProjectDatabaseStateChangeListener
		{
			private readonly EditableMenuCheckBoxTreeCell outerInstance;

			public DatabaseStateChangeListener(EditableMenuCheckBoxTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void projectDatabaseStateChanged(ProjectDatabaseStateEvent evt)
			{
				if (outerInstance.contextMenu == null || outerInstance.contextMenu.getItems().isEmpty())
				{
					return;
				}

				bool disable = evt.EventType != ProjectDatabaseStateType.OPENED;
				IList<MenuItem> items = outerInstance.contextMenu.getItems();
				foreach (MenuItem item in items)
				{
					this.disableItem(item, disable);
				}
			}

			internal virtual void disableItem(MenuItem item, bool disable)
			{
				if (item is Menu)
				{
					this.disableItem((Menu)item, disable);
				}
				else
				{
					item.setDisable(disable);
				}
			}
		}

		private class DropMouseEventHandler : EventHandler<MouseEvent>
		{
			private readonly EditableMenuCheckBoxTreeCell outerInstance;

			public DropMouseEventHandler(EditableMenuCheckBoxTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void handle(MouseEvent @event)
			{
				if (@event.getEventType() == MouseEvent.DRAG_DETECTED)
				{
					IList<TreeItem<TreeItemValue>> draggedItems = new List<TreeItem<TreeItemValue>>(getTreeView().getSelectionModel().getSelectedItems());

					TreeItemType itemType = getItem() != null ? getItem().getItemType() : null;

					if (itemType != null)
					{
						IList<GroupTreeItemDnD> groupItemsDnD = new List<GroupTreeItemDnD>(draggedItems.Count);
						foreach (TreeItem<TreeItemValue> draggedItem in draggedItems)
						{
							if (draggedItem.getValue() == null || draggedItem.getValue().getItemType() != itemType)
							{
								return;
							}

							GroupTreeItemDnD itemDnD = GroupTreeItemDnD.fromTreeItem(draggedItem);
							if (itemDnD == null)
							{
								return;
							}

							groupItemsDnD.Add(itemDnD);
						}

						if (groupItemsDnD.Count > 0)
						{
							Dragboard db = startDragAndDrop(TransferMode.MOVE);

							TreeItemType parentType = groupItemsDnD[0].ParentType;
							int groupId = groupItemsDnD[0].GroupId;
							int dimension = groupItemsDnD[0].Dimension;

							ClipboardContent content = new ClipboardContent();
							content.put(TREE_PARENT_ITEM_TYPE_DATA_FORMAT, parentType);
							content.put(TREE_ITEM_TYPE_DATA_FORMAT, itemType);
							content.put(GROUP_ID_DATA_FORMAT, groupId);
							content.put(DIMENSION_DATA_FORMAT, dimension);
							content.put(TREE_ITEMS_DATA_FORMAT, groupItemsDnD);

							db.setContent(content);
							@event.consume();
						}
					}
				}
			}
		}

		private class DropEventHandler : EventHandler<DragEvent>
		{
			private readonly EditableMenuCheckBoxTreeCell outerInstance;

			public DropEventHandler(EditableMenuCheckBoxTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(DragEvent @event)
			{
				if (@event.getEventType() == DragEvent.DRAG_OVER)
				{
					if (acceptTransfer(@event))
					{
						@event.acceptTransferModes(TransferMode.MOVE);
					}
					//event.consume();
				}
				else if (@event.getEventType() == DragEvent.DRAG_DROPPED)
				{

					Dragboard db = @event.getDragboard();
					bool success = false;
					if (acceptTransfer(@event))
					{
						success = true;

						if (db.hasContent(TREE_ITEMS_DATA_FORMAT))
						{
							TreeItem<TreeItemValue> droppedOverItem = getTreeItem();
							bool isDirectoryNode = db.hasContent(TREE_PARENT_ITEM_TYPE_DATA_FORMAT) && droppedOverItem != null && droppedOverItem.getValue() != null && droppedOverItem.getValue().getItemType() == db.getContent(TREE_PARENT_ITEM_TYPE_DATA_FORMAT);

							TreeItem<TreeItemValue> droppedOverParentItem = isDirectoryNode ? droppedOverItem : droppedOverItem.getParent();
							IList<TreeItem<TreeItemValue>> childItems = droppedOverParentItem.getChildren();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<?> groupItemsDnD = (java.util.List<?>)db.getContent(TREE_ITEMS_DATA_FORMAT);
							IList<object> groupItemsDnD = (IList<object>)db.getContent(TREE_ITEMS_DATA_FORMAT);
							IList<TreeItem<TreeItemValue>> draggedItems = new List<TreeItem<TreeItemValue>>(groupItemsDnD.Count);

							for (int i = 0; i < groupItemsDnD.Count; i++)
							{
								if (groupItemsDnD[i] == null || !(groupItemsDnD[i] is GroupTreeItemDnD))
								{
									continue;
								}
								GroupTreeItemDnD itemDnD = (GroupTreeItemDnD)groupItemsDnD[i];
								draggedItems.Add(childItems[itemDnD.Index]);
							}
							if (draggedItems.Count > 0)
							{
								childItems.RemoveAll(draggedItems);
								int indexInParent = isDirectoryNode ? 0 : childItems.IndexOf(droppedOverItem) + 1;
								((List<TreeItem<TreeItemValue>>)childItems).AddRange(indexInParent, draggedItems);

								// Save new order in item values
								for (int orderId = 0; orderId < childItems.Count; orderId++)
								{
									TreeItem<TreeItemValue> item = childItems[orderId];
									TreeItemValue itemValue = item.getValue();

									if (!(itemValue is Sortable) || itemValue.ItemType == null)
									{
										continue;
									}

									((Sortable)itemValue).OrderId = orderId;

									try
									{
										if (TreeItemType.isObservationTypeLeaf(itemValue.ItemType))
										{
											SQLManager.Instance.saveGroup((ObservationTreeItemValue)itemValue);
										}

										else if (TreeItemType.isGNSSObservationTypeLeaf(itemValue.ItemType))
										{
											SQLManager.Instance.saveGroup((ObservationTreeItemValue)itemValue);
										}

										else if (TreeItemType.isPointTypeLeaf(itemValue.ItemType))
										{
											SQLManager.Instance.saveGroup((PointTreeItemValue)itemValue);
										}

										else if (TreeItemType.isCongruenceAnalysisTypeLeaf(itemValue.ItemType))
										{
											SQLManager.Instance.saveGroup((CongruenceAnalysisTreeItemValue)itemValue);
										}

										else if (TreeItemType.isVerticalDeflectionTypeLeaf(itemValue.ItemType))
										{
											SQLManager.Instance.saveGroup((VerticalDeflectionTreeItemValue)itemValue);
										}

									}
									catch (Exception e)
									{
										Console.WriteLine(e.ToString());
										Console.Write(e.StackTrace);
										OptionDialog.showThrowableDialog(outerInstance.i18n.getString("EditableMenuCheckBoxTreeCell.message.error.save_dropped_tree_item.exception.title", "Unexpected SQL-Error"), outerInstance.i18n.getString("EditableMenuCheckBoxTreeCell.message.error.save_dropped_tree_item.exception.header", "Error, could not drop selected item to tree menu."), outerInstance.i18n.getString("EditableMenuCheckBoxTreeCell.message.error.save_dropped_tree_item.exception.message", "An exception has occurred during database transaction."), e);
										success = false;
										break;
									}
								}

								Platform.runLater(() =>
								{
								if (draggedItems.Count > 0)
								{
									getTreeView().getSelectionModel().clearSelection();
									getTreeView().getSelectionModel().select(draggedItems[0]);
								}
								});
							}
						}
						else
						{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<?> droppedRows = null;
							IList<object> droppedRows = null;
							int targetGroupId = -1;

							if (TreeItemType.isObservationTypeLeaf(getItem().getItemType()) && db.hasContent(TERRESTRIAL_OBSERVATION_ROWS_DATA_FORMAT))
							{
								targetGroupId = ((ObservationTreeItemValue)getItem()).GroupId;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: droppedRows = (java.util.List<?>)db.getContent(TERRESTRIAL_OBSERVATION_ROWS_DATA_FORMAT);
								droppedRows = (IList<object>)db.getContent(TERRESTRIAL_OBSERVATION_ROWS_DATA_FORMAT);
							}
							else if (TreeItemType.isGNSSObservationTypeLeaf(getItem().getItemType()) && db.hasContent(GNSS_OBSERVATION_ROWS_DATA_FORMAT))
							{
								targetGroupId = ((ObservationTreeItemValue)getItem()).GroupId;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: droppedRows = (java.util.List<?>)db.getContent(GNSS_OBSERVATION_ROWS_DATA_FORMAT);
								droppedRows = (IList<object>)db.getContent(GNSS_OBSERVATION_ROWS_DATA_FORMAT);
							}
							else if (TreeItemType.isPointTypeLeaf(getItem().getItemType()) && db.hasContent(POINT_ROWS_DATA_FORMAT))
							{
								targetGroupId = ((PointTreeItemValue)getItem()).GroupId;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: droppedRows = (java.util.List<?>)db.getContent(POINT_ROWS_DATA_FORMAT);
								droppedRows = (IList<object>)db.getContent(POINT_ROWS_DATA_FORMAT);
							}
							else if (TreeItemType.isCongruenceAnalysisTypeLeaf(getItem().getItemType()) && db.hasContent(CONGRUENCE_ANALYSIS_ROWS_DATA_FORMAT))
							{
								targetGroupId = ((CongruenceAnalysisTreeItemValue)getItem()).GroupId;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: droppedRows = (java.util.List<?>)db.getContent(CONGRUENCE_ANALYSIS_ROWS_DATA_FORMAT);
								droppedRows = (IList<object>)db.getContent(CONGRUENCE_ANALYSIS_ROWS_DATA_FORMAT);
							}
							else if (TreeItemType.isVerticalDeflectionTypeLeaf(getItem().getItemType()) && db.hasContent(VERTICAL_DEFLECTION_ROWS_DATA_FORMAT))
							{
								targetGroupId = ((VerticalDeflectionTreeItemValue)getItem()).GroupId;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: droppedRows = (java.util.List<?>)db.getContent(VERTICAL_DEFLECTION_ROWS_DATA_FORMAT);
								droppedRows = (IList<object>)db.getContent(VERTICAL_DEFLECTION_ROWS_DATA_FORMAT);
							}

							if (droppedRows != null && targetGroupId >= 0)
							{
								for (int i = 0; i < droppedRows.Count; i++)
								{
									if (droppedRows[i] == null)
									{
										continue;
									}
									try
									{
										if (droppedRows[i] is TerrestrialObservationRowDnD)
										{
											TerrestrialObservationRowDnD rowDnD = (TerrestrialObservationRowDnD)droppedRows[i];
											TerrestrialObservationRow row = rowDnD.toTerrestrialObservationRow();
											row.GroupId = targetGroupId;
											SQLManager.Instance.saveItem(row);
										}
										else if (droppedRows[i] is GNSSObservationRowDnD)
										{
											GNSSObservationRowDnD rowDnD = (GNSSObservationRowDnD)droppedRows[i];
											GNSSObservationRow row = rowDnD.toGNSSObservationRow();
											row.GroupId = targetGroupId;
											SQLManager.Instance.saveItem(row);
										}
										else if (droppedRows[i] is PointRowDnD)
										{
											PointRowDnD rowDnD = (PointRowDnD)droppedRows[i];
											PointRow row = rowDnD.toPointRow();
											row.GroupId = targetGroupId;
											SQLManager.Instance.saveItem(row);
										}
										else if (droppedRows[i] is CongruenceAnalysisRowDnD)
										{
											CongruenceAnalysisRowDnD rowDnD = (CongruenceAnalysisRowDnD)droppedRows[i];
											CongruenceAnalysisRow row = rowDnD.toCongruenceAnalysisRow();
											row.GroupId = targetGroupId;
											SQLManager.Instance.saveItem(row);
										}
										else if (droppedRows[i] is VerticalDeflectionRowDnD)
										{
											VerticalDeflectionRowDnD rowDnD = (VerticalDeflectionRowDnD)droppedRows[i];
											VerticalDeflectionRow row = rowDnD.toVerticalDeflectionRow();
											row.GroupId = targetGroupId;
											SQLManager.Instance.saveItem(row);
										}
									}
									catch (Exception e)
									{
										Console.WriteLine(e.ToString());
										Console.Write(e.StackTrace);
										OptionDialog.showThrowableDialog(outerInstance.i18n.getString("EditableMenuCheckBoxTreeCell.message.error.save_dropped_table_row.exception.title", "Unexpected SQL-Error"), outerInstance.i18n.getString("EditableMenuCheckBoxTreeCell.message.error.save_dropped_table_row.exception.header", "Error, could not drop selected row to new table."), outerInstance.i18n.getString("EditableMenuCheckBoxTreeCell.message.error.save_dropped_table_row.exception.message", "An exception has occurred during database transaction."), e);
										success = false;
										break;
									}
								}
							}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javafx.scene.control.TreeItem<TreeItemValue> targetTreeItem = getTreeItem();
							TreeItem<TreeItemValue> targetTreeItem = getTreeItem();
							Platform.runLater(() =>
							{
							if (targetTreeItem != null)
							{
								getTreeView().getSelectionModel().clearSelection();
								getTreeView().getSelectionModel().select(targetTreeItem);
							}
							});
						}
					}
					@event.setDropCompleted(success);
				}
				@event.consume();
			}

			internal virtual bool acceptTransfer(DragEvent @event)
			{
				Dragboard db = @event.getDragboard();
				TreeItemValue itemValue = getItem();
				TreeItemType itemType = itemValue != null && itemValue.ItemType != null ? itemValue.ItemType : null;
				ISet<int> groupItemIdsDnD = new HashSet<int>(0);

				if (db.hasContent(TREE_ITEMS_DATA_FORMAT))
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<?> groupItemsDnD = (java.util.List<?>)db.getContent(TREE_ITEMS_DATA_FORMAT);
					IList<object> groupItemsDnD = (IList<object>)db.getContent(TREE_ITEMS_DATA_FORMAT);
					groupItemIdsDnD = new HashSet<int>(groupItemsDnD.Count);

					for (int i = 0; i < groupItemsDnD.Count; i++)
					{
						if (groupItemsDnD[i] == null || !(groupItemsDnD[i] is GroupTreeItemDnD))
						{
							continue;
						}
						GroupTreeItemDnD itemDnD = (GroupTreeItemDnD)groupItemsDnD[i];
						groupItemIdsDnD.Add(itemDnD.GroupId);
					}
				}

				return (itemType != null && (@event.getGestureSource() is TableView || @event.getGestureSource() is EditableMenuCheckBoxTreeCell) && db.hasContent(TREE_ITEM_TYPE_DATA_FORMAT) && db.hasContent(GROUP_ID_DATA_FORMAT) && db.hasContent(DIMENSION_DATA_FORMAT) && (db.hasContent(TREE_ITEMS_DATA_FORMAT) && db.hasContent(TREE_PARENT_ITEM_TYPE_DATA_FORMAT) && itemType == db.getContent(TREE_PARENT_ITEM_TYPE_DATA_FORMAT) || ((db.hasContent(TERRESTRIAL_OBSERVATION_ROWS_DATA_FORMAT) || db.hasContent(TREE_ITEMS_DATA_FORMAT)) && TreeItemType.isObservationTypeLeaf(itemType) && itemType == db.getContent(TREE_ITEM_TYPE_DATA_FORMAT) && itemValue is ObservationTreeItemValue && ((ObservationTreeItemValue)itemValue).GroupId != ((int?)db.getContent(GROUP_ID_DATA_FORMAT)).Value) && !groupItemIdsDnD.Contains(((ObservationTreeItemValue)itemValue).GroupId) || ((db.hasContent(GNSS_OBSERVATION_ROWS_DATA_FORMAT) || db.hasContent(TREE_ITEMS_DATA_FORMAT)) && TreeItemType.isGNSSObservationTypeLeaf(itemType) && itemType == db.getContent(TREE_ITEM_TYPE_DATA_FORMAT) && itemValue is ObservationTreeItemValue && ((ObservationTreeItemValue)itemValue).Dimension == ((int?)db.getContent(DIMENSION_DATA_FORMAT)).Value && ((ObservationTreeItemValue)itemValue).GroupId != ((int?)db.getContent(GROUP_ID_DATA_FORMAT)).Value) && !groupItemIdsDnD.Contains(((ObservationTreeItemValue)itemValue).GroupId) || ((db.hasContent(POINT_ROWS_DATA_FORMAT) || db.hasContent(TREE_ITEMS_DATA_FORMAT)) && TreeItemType.isPointTypeLeaf(itemType) && TreeItemType.isPointTypeLeaf((TreeItemType)db.getContent(TREE_ITEM_TYPE_DATA_FORMAT)) && itemValue is PointTreeItemValue && ((PointTreeItemValue)itemValue).Dimension == ((int?)db.getContent(DIMENSION_DATA_FORMAT)).Value && ((PointTreeItemValue)itemValue).GroupId != ((int?)db.getContent(GROUP_ID_DATA_FORMAT)).Value) && !groupItemIdsDnD.Contains(((PointTreeItemValue)itemValue).GroupId) || ((db.hasContent(VERTICAL_DEFLECTION_ROWS_DATA_FORMAT) || db.hasContent(TREE_ITEMS_DATA_FORMAT)) && TreeItemType.isVerticalDeflectionTypeLeaf(itemType) && TreeItemType.isVerticalDeflectionTypeLeaf((TreeItemType)db.getContent(TREE_ITEM_TYPE_DATA_FORMAT)) && itemValue is VerticalDeflectionTreeItemValue && ((VerticalDeflectionTreeItemValue)itemValue).GroupId != ((int?)db.getContent(GROUP_ID_DATA_FORMAT)).Value) && !groupItemIdsDnD.Contains(((VerticalDeflectionTreeItemValue)itemValue).GroupId) || ((db.hasContent(CONGRUENCE_ANALYSIS_ROWS_DATA_FORMAT) || db.hasContent(TREE_ITEMS_DATA_FORMAT)) && TreeItemType.isCongruenceAnalysisTypeLeaf(itemType) && itemType == db.getContent(TREE_ITEM_TYPE_DATA_FORMAT) && itemValue is CongruenceAnalysisTreeItemValue && ((CongruenceAnalysisTreeItemValue)itemValue).Dimension == ((int?)db.getContent(DIMENSION_DATA_FORMAT)).Value && ((CongruenceAnalysisTreeItemValue)itemValue).GroupId != ((int?)db.getContent(GROUP_ID_DATA_FORMAT)).Value) && !groupItemIdsDnD.Contains(((CongruenceAnalysisTreeItemValue)itemValue).GroupId)));
			}
		}

		private class PointGroupChangeListener : ChangeListener<Toggle>
		{
			private readonly EditableMenuCheckBoxTreeCell outerInstance;

			public PointGroupChangeListener(EditableMenuCheckBoxTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void changed<T1>(ObservableValue<T1> observable, Toggle oldValue, Toggle newValue) where T1 : javafx.scene.control.Toggle
			{

				if (!outerInstance.IgnoreEvent && newValue != null && newValue.getUserData() is ContextMenuType && getTreeItem() != null && getTreeItem().getValue() != null && getTreeItem().getValue() is PointTreeItemValue)
				{ // newValue instanceof RadioMenuItem &&
					ContextMenuType menuItemType = (ContextMenuType)newValue.getUserData();
					PointTreeItemValue itemValue = (PointTreeItemValue)getTreeItem().getValue();

					int dimension = itemValue.Dimension;
					PointType pointType = TreeItemType.getPointTypeByTreeItemType(itemValue.ItemType);

					switch (menuItemType)
					{
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.CHANGE_TO_REFERENCE_POINT_GROUP:
						if (pointType == PointType.REFERENCE_POINT)
						{
							return;
						}

						if (dimension == 1)
						{
							outerInstance.changeGroupType(TreeItemType.REFERENCE_POINT_1D_LEAF);
						}
						else if (dimension == 2)
						{
							outerInstance.changeGroupType(TreeItemType.REFERENCE_POINT_2D_LEAF);
						}
						else if (dimension == 3)
						{
							outerInstance.changeGroupType(TreeItemType.REFERENCE_POINT_3D_LEAF);
						}
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.CHANGE_TO_STOCHASTIC_POINT_GROUP:
						if (pointType == PointType.STOCHASTIC_POINT)
						{
							return;
						}

						if (dimension == 1)
						{
							outerInstance.changeGroupType(TreeItemType.STOCHASTIC_POINT_1D_LEAF);
						}
						else if (dimension == 2)
						{
							outerInstance.changeGroupType(TreeItemType.STOCHASTIC_POINT_2D_LEAF);
						}
						else if (dimension == 3)
						{
							outerInstance.changeGroupType(TreeItemType.STOCHASTIC_POINT_3D_LEAF);
						}
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.CHANGE_TO_DATUM_POINT_GROUP:
						if (pointType == PointType.DATUM_POINT)
						{
							return;
						}

						if (dimension == 1)
						{
							outerInstance.changeGroupType(TreeItemType.DATUM_POINT_1D_LEAF);
						}
						else if (dimension == 2)
						{
							outerInstance.changeGroupType(TreeItemType.DATUM_POINT_2D_LEAF);
						}
						else if (dimension == 3)
						{
							outerInstance.changeGroupType(TreeItemType.DATUM_POINT_3D_LEAF);
						}
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.CHANGE_TO_NEW_POINT_GROUP:
						if (pointType == PointType.NEW_POINT)
						{
							return;
						}

						if (dimension == 1)
						{
							outerInstance.changeGroupType(TreeItemType.NEW_POINT_1D_LEAF);
						}
						else if (dimension == 2)
						{
							outerInstance.changeGroupType(TreeItemType.NEW_POINT_2D_LEAF);
						}
						else if (dimension == 3)
						{
							outerInstance.changeGroupType(TreeItemType.NEW_POINT_3D_LEAF);
						}
						break;

					default:
						Console.Error.WriteLine(typeof(EditableMenuCheckBoxTreeCell).Name + " : Error, unsupported context menu item type " + menuItemType);
						break;
					}
				}
			}
		}


		private class VerticalDeflectionGroupChangeListener : ChangeListener<Toggle>
		{
			private readonly EditableMenuCheckBoxTreeCell outerInstance;

			public VerticalDeflectionGroupChangeListener(EditableMenuCheckBoxTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void changed<T1>(ObservableValue<T1> observable, Toggle oldValue, Toggle newValue) where T1 : javafx.scene.control.Toggle
			{

				if (!outerInstance.IgnoreEvent && newValue != null && newValue.getUserData() is ContextMenuType && getTreeItem() != null && getTreeItem().getValue() != null && getTreeItem().getValue() is VerticalDeflectionTreeItemValue)
				{ // newValue instanceof RadioMenuItem &&
					ContextMenuType menuItemType = (ContextMenuType)newValue.getUserData();
					VerticalDeflectionTreeItemValue itemValue = (VerticalDeflectionTreeItemValue)getTreeItem().getValue();

					VerticalDeflectionType verticalDeflectionType = TreeItemType.getVerticalDeflectionTypeByTreeItemType(itemValue.ItemType);

					switch (menuItemType)
					{
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.CHANGE_TO_REFERENCE_DEFLECTION_GROUP:
						if (verticalDeflectionType == VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION)
						{
							return;
						}
						outerInstance.changeGroupType(TreeItemType.REFERENCE_VERTICAL_DEFLECTION_LEAF);
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.CHANGE_TO_STOCHASTIC_DEFLECTION_GROUP:
						if (verticalDeflectionType == VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION)
						{
							return;
						}
						outerInstance.changeGroupType(TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_LEAF);
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.CHANGE_TO_UNKNOWN_DEFLECTION_GROUP:
						if (verticalDeflectionType == VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION)
						{
							return;
						}
						outerInstance.changeGroupType(TreeItemType.UNKNOWN_VERTICAL_DEFLECTION_LEAF);
						break;

					default:
						Console.Error.WriteLine(typeof(EditableMenuCheckBoxTreeCell).Name + " : Error, unsupported context menu item type " + menuItemType);
						break;
					}
				}
			}
		}



		private class ContextMenuEventHandler : EventHandler<ActionEvent>
		{
			private readonly EditableMenuCheckBoxTreeCell outerInstance;

			public ContextMenuEventHandler(EditableMenuCheckBoxTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void handle(ActionEvent @event)
			{
				if (!outerInstance.IgnoreEvent && @event.getSource() is MenuItem && ((MenuItem)@event.getSource()).getUserData() is ContextMenuType && getTreeItem() != null && getTreeItem().getValue() != null)
				{
					ContextMenuType contextMenuType = (ContextMenuType)((MenuItem)@event.getSource()).getUserData();
					TreeItem<TreeItemValue> selectedItem = getTreeItem();
					TreeItemValue itemValue = selectedItem.getValue();

					switch (contextMenuType)
					{
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.ADD:
						outerInstance.addNewEmptyGroup(itemValue.ItemType);
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.REMOVE:
						outerInstance.removeSelectedGroups();
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.EXPORT:
						outerInstance.exportTableData(itemValue);
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.SEARCH_AND_REPLACE:
						outerInstance.searchAndReplace(selectedItem);
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.ADAPT_INSTRUMENT_AND_REFLECTOR_HEIGHT:
						outerInstance.adaptInstrumentAndReflectorHeights(selectedItem);
						break;
					case org.applied_geodesy.jag3d.ui.tree.EditableMenuCheckBoxTreeCell.ContextMenuType.MERGE:
						outerInstance.mergeSelectedGroups(selectedItem);
						break;
					default:
						Console.WriteLine(this.GetType().Name + " : " + @event);
						break;

					}
				}
			}
		}

		internal EditableMenuCheckBoxTreeCell()
		{
			DropEventHandler dropEventHandler = new DropEventHandler(this);
			DropMouseEventHandler dropMouseEventHandler = new DropMouseEventHandler(this);

			this.setOnDragOver(dropEventHandler);
			this.setOnDragDropped(dropEventHandler);
			this.setOnDragDetected(dropMouseEventHandler);
		}

		private void initContextMenu(TreeItemType itemType)
		{
			bool disable = !SQLManager.Instance.hasDatabase();
			SQLManager.Instance.addProjectDatabaseStateChangeListener(new DatabaseStateChangeListener(this));
			ContextMenuEventHandler listener = new ContextMenuEventHandler(this);
			MenuItem addItem = new MenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.add", "Add item"));
			addItem.setUserData(ContextMenuType.ADD);
			addItem.setOnAction(listener);
			addItem.setDisable(disable);

			if (TreeItemType.isPointTypeDirectory(itemType) || TreeItemType.isObservationTypeDirectory(itemType) || TreeItemType.isGNSSObservationTypeDirectory(itemType) || TreeItemType.isVerticalDeflectionTypeDirectory(itemType) || TreeItemType.isCongruenceAnalysisTypeDirectory(itemType))
			{
				this.contextMenu = new ContextMenu(addItem);
				return;
			}

			MenuItem removeItem = new MenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.remove", "Remove items"));
			removeItem.setUserData(ContextMenuType.REMOVE);
			removeItem.setOnAction(listener);

			MenuItem exportItem = new MenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.export", "Export items"));
			exportItem.setUserData(ContextMenuType.EXPORT);
			exportItem.setOnAction(listener);

			MenuItem mergeItem = new MenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.merge", "Merge items"));
			mergeItem.setUserData(ContextMenuType.MERGE);
			mergeItem.setOnAction(listener);

			MenuItem searchAndReplaceItem = new MenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.search_and_replace", "Search and replace"));
			searchAndReplaceItem.setUserData(ContextMenuType.SEARCH_AND_REPLACE);
			searchAndReplaceItem.setOnAction(listener);

			this.contextMenu = new ContextMenu(addItem, removeItem, mergeItem, exportItem, new SeparatorMenuItem(), searchAndReplaceItem);

			if (TreeItemType.isObservationTypeLeaf(itemType))
			{
				MenuItem instrumentAndReflectorHeightItem = new MenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.instrument_and_reflector_height", "Height adaption"));
				instrumentAndReflectorHeightItem.setUserData(ContextMenuType.ADAPT_INSTRUMENT_AND_REFLECTOR_HEIGHT);
				instrumentAndReflectorHeightItem.setOnAction(listener);

				this.contextMenu.getItems().addAll(instrumentAndReflectorHeightItem);
			}

			else if (TreeItemType.isPointTypeLeaf(itemType))
			{
				ToggleGroup pointTypeToogleGroup = new ToggleGroup();

				RadioMenuItem referencePointMenuItem = createRadioMenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.pointgroup.reference", "Reference point group"), itemType == TreeItemType.REFERENCE_POINT_1D_LEAF || itemType == TreeItemType.REFERENCE_POINT_2D_LEAF || itemType == TreeItemType.REFERENCE_POINT_3D_LEAF, pointTypeToogleGroup, ContextMenuType.CHANGE_TO_REFERENCE_POINT_GROUP);

				RadioMenuItem stochasticPointMenuItem = createRadioMenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.pointgroup.stochastic", "Stochastic point group"), itemType == TreeItemType.STOCHASTIC_POINT_1D_LEAF || itemType == TreeItemType.STOCHASTIC_POINT_2D_LEAF || itemType == TreeItemType.STOCHASTIC_POINT_3D_LEAF, pointTypeToogleGroup, ContextMenuType.CHANGE_TO_STOCHASTIC_POINT_GROUP);

				RadioMenuItem datumPointMenuItem = createRadioMenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.pointgroup.datum", "Datum point group"), itemType == TreeItemType.DATUM_POINT_1D_LEAF || itemType == TreeItemType.DATUM_POINT_2D_LEAF || itemType == TreeItemType.DATUM_POINT_3D_LEAF, pointTypeToogleGroup, ContextMenuType.CHANGE_TO_DATUM_POINT_GROUP);

				RadioMenuItem newPointMenuItem = createRadioMenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.pointgroup.new", "New point group"), itemType == TreeItemType.NEW_POINT_1D_LEAF || itemType == TreeItemType.NEW_POINT_2D_LEAF || itemType == TreeItemType.NEW_POINT_3D_LEAF, pointTypeToogleGroup, ContextMenuType.CHANGE_TO_NEW_POINT_GROUP);

				pointTypeToogleGroup.selectedToggleProperty().addListener(new PointGroupChangeListener(this));
				this.contextMenu.getItems().addAll(new SeparatorMenuItem(), referencePointMenuItem, stochasticPointMenuItem, datumPointMenuItem, newPointMenuItem);
			}

			else if (TreeItemType.isVerticalDeflectionTypeLeaf(itemType))
			{
				ToggleGroup deflectionTypeToogleGroup = new ToggleGroup();

				RadioMenuItem referenceDeflectionMenuItem = createRadioMenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.deflectiongroup.reference", "Reference deflection group"), itemType == TreeItemType.REFERENCE_VERTICAL_DEFLECTION_LEAF, deflectionTypeToogleGroup, ContextMenuType.CHANGE_TO_REFERENCE_DEFLECTION_GROUP);

				RadioMenuItem stochasticDeflectionMenuItem = createRadioMenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.deflectiongroup.stochastic", "Stochastic deflection group"), itemType == TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_LEAF, deflectionTypeToogleGroup, ContextMenuType.CHANGE_TO_STOCHASTIC_DEFLECTION_GROUP);

				RadioMenuItem datumDeflectionMenuItem = createRadioMenuItem(i18n.getString("EditableMenuCheckBoxTreeCell.contextmenu.deflectiongroup.unknown", "Unknown deflection group"), itemType == TreeItemType.UNKNOWN_VERTICAL_DEFLECTION_LEAF, deflectionTypeToogleGroup, ContextMenuType.CHANGE_TO_UNKNOWN_DEFLECTION_GROUP);


				deflectionTypeToogleGroup.selectedToggleProperty().addListener(new VerticalDeflectionGroupChangeListener(this));
				this.contextMenu.getItems().addAll(new SeparatorMenuItem(), referenceDeflectionMenuItem, stochasticDeflectionMenuItem, datumDeflectionMenuItem);
			}

			// disable all items (until a database is selected)
			foreach (MenuItem item in this.contextMenu.getItems())
			{
				item.setDisable(disable);
			}
		}

		private static RadioMenuItem createRadioMenuItem(string label, bool selected, ToggleGroup group, ContextMenuType type)
		{
			RadioMenuItem radioMenuItem = new RadioMenuItem(label);
			radioMenuItem.setUserData(type);
			radioMenuItem.setSelected(selected);
			radioMenuItem.setToggleGroup(group);
			return radioMenuItem;
		}

		public override void commitEdit(TreeItemValue item)
		{
			if (!this.isEditing() && !item.Equals(this.getItem()))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javafx.scene.control.TreeItem<TreeItemValue> treeItem = getTreeItem();
				TreeItem<TreeItemValue> treeItem = getTreeItem();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javafx.scene.control.TreeView<TreeItemValue> treeView = getTreeView();
				TreeView<TreeItemValue> treeView = getTreeView();
				if (treeView != null)
				{
					// Inform the TreeView of the edit being ready to be committed.
					treeView.fireEvent(new TreeView.EditEvent<TreeItemValue>(treeView, TreeView.editCommitEvent<TreeItemValue> (), treeItem, getItem(), item));
					//treeView.getSelectionModel().select(treeItem);
				}
			}
			base.commitEdit(item);
		}

		public override void startEdit()
		{
			base.startEdit();

			if (this.getGraphic() != null && this.getGraphic() is CheckBox)
			{
				this.checkBox = (CheckBox)this.getGraphic();
			}

			if (this.textField == null)
			{
				this.createTextField();
			}

			if (this.getItem() != null)
			{
				switch (this.getItem().getItemType())
				{
	//			case ROOT: // root node is editable
				case REFERENCE_POINT_1D_LEAF:
				case REFERENCE_POINT_2D_LEAF:
				case REFERENCE_POINT_3D_LEAF:
				case STOCHASTIC_POINT_1D_LEAF:
				case STOCHASTIC_POINT_2D_LEAF:
				case STOCHASTIC_POINT_3D_LEAF:
				case DATUM_POINT_1D_LEAF:
				case DATUM_POINT_2D_LEAF:
				case DATUM_POINT_3D_LEAF:
				case NEW_POINT_1D_LEAF:
				case NEW_POINT_2D_LEAF:
				case NEW_POINT_3D_LEAF:
				case LEVELING_LEAF:
				case DIRECTION_LEAF:
				case HORIZONTAL_DISTANCE_LEAF:
				case SLOPE_DISTANCE_LEAF:
				case ZENITH_ANGLE_LEAF:
				case GNSS_1D_LEAF:
				case GNSS_2D_LEAF:
				case GNSS_3D_LEAF:
				case CONGRUENCE_ANALYSIS_1D_LEAF:
				case CONGRUENCE_ANALYSIS_2D_LEAF:
				case CONGRUENCE_ANALYSIS_3D_LEAF:
				case REFERENCE_VERTICAL_DEFLECTION_LEAF:
				case STOCHASTIC_VERTICAL_DEFLECTION_LEAF:
				case UNKNOWN_VERTICAL_DEFLECTION_LEAF:
					this.setText(null);
					this.setGraphic(this.textField);
					this.textField.setText(this.String);
					Platform.runLater(() =>
					{
					textField.selectAll();
					textField.requestFocus();
					});
					break;
				default:
					Console.Error.WriteLine(this.GetType().Name + " Error, changing node name is not supported for " + this.getItem().getItemType());
					break;
				}
			}
		}

		public override void cancelEdit()
		{
			base.cancelEdit();
			this.setText(this.getItem().ToString());
			this.setGraphic(this.checkBox);
		}

		public override void updateItem(TreeItemValue item, bool empty)
		{
			base.updateItem(item, empty);

			// Setze Kontextmenu
			if (item != null)
			{
				switch (item.ItemType.innerEnumValue)
				{
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_3D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_3D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.LEVELING_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DIRECTION_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_LEAF:
					this.initContextMenu(item.ItemType);
					this.setContextMenu(this.contextMenu);
					break;
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_1D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_2D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_3D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_1D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_2D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_3D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.LEVELING_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DIRECTION_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.SLOPE_DISTANCE_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ZENITH_ANGLE_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_1D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY:
					if (getTreeItem() != null && getTreeItem().isLeaf())
					{
						this.initContextMenu(item.ItemType);
						this.setContextMenu(this.contextMenu);
					}
					else
					{
						this.setContextMenu(null);
					}
					break;
				default:
					this.setContextMenu(null);
					break;
				}
			}

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
						this.textField.setText(String);
					}
					this.setText(null);
					this.setGraphic(this.textField);
				}
				else
				{
					this.setText(String);
					if (getTreeItem() != null && getTreeItem().getValue() != null && (getTreeItem().getValue().getItemType() == TreeItemType.ROOT || getTreeItem().getValue().getItemType() == TreeItemType.UNSPECIFIC))
					{
						setGraphic(new Text());
					}
					else if (this.checkBox != null)
					{
						setGraphic(this.checkBox);
					}
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
			private readonly EditableMenuCheckBoxTreeCell outerInstance;

			public EventHandlerAnonymousInnerClass(EditableMenuCheckBoxTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				if (getTreeItem() != null)
				{
					TreeItemValue item = getTreeItem().getValue();
					item.Name = outerInstance.textField.getText();
					outerInstance.commitEdit(item);
				}
			}
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<bool>
		{
			private readonly EditableMenuCheckBoxTreeCell outerInstance;

			public ChangeListenerAnonymousInnerClass(EditableMenuCheckBoxTreeCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if ((!newValue).Value)
				{
					TreeItemValue item = getTreeItem().getValue();
					item.Name = outerInstance.textField.getText();
					outerInstance.commitEdit(item);
				}
			}
		}

		private class EventHandlerAnonymousInnerClass2 : EventHandler<KeyEvent>
		{
			private readonly EditableMenuCheckBoxTreeCell outerInstance;

			public EventHandlerAnonymousInnerClass2(EditableMenuCheckBoxTreeCell outerInstance)
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

		private void changeGroupType(TreeItemType newItemType)
		{
			IList<TreeItem<TreeItemValue>> selectedItems = this.getTreeView().getSelectionModel().getSelectedItems();
			if (newItemType != null && selectedItems != null && selectedItems.Count > 0)
			{
				UITreeBuilder.Instance.moveItems(newItemType, new List<TreeItem<TreeItemValue>>(selectedItems));
			}
		}

		private void addNewEmptyGroup(TreeItemType itemType)
		{
			TreeItemType parentType = null;
			if ((parentType = TreeItemType.getDirectoryByLeafType(itemType)) != null && TreeItemType.getLeafByDirectoryType(itemType) == null)
			{
				UITreeBuilder.Instance.addEmptyGroup(parentType);
			}
			else if (TreeItemType.getDirectoryByLeafType(itemType) == null && TreeItemType.getLeafByDirectoryType(itemType) != null)
			{
				parentType = itemType;
				UITreeBuilder.Instance.addEmptyGroup(parentType);
			}
		}

		private void removeSelectedGroups()
		{
			UITreeBuilder.Instance.removeSelectedGroups();
		}

		private void mergeSelectedGroups(TreeItem<TreeItemValue> selectedItem)
		{
			IList<TreeItem<TreeItemValue>> selectedItems = this.getTreeView().getSelectionModel().getSelectedItems();
			if (selectedItem != null && selectedItem.getValue() != null && selectedItems != null && selectedItems.Count > 1)
			{
				TreeItemValue selectedItemValue = selectedItem.getValue();
				IList<TreeItem<TreeItemValue>> removeItems = new List<TreeItem<TreeItemValue>>(selectedItems.Count);
				TreeItemType itemType = selectedItemValue.ItemType;

				if (!TreeItemType.isPointTypeLeaf(itemType) && !TreeItemType.isObservationTypeLeaf(itemType) && !TreeItemType.isGNSSObservationTypeLeaf(itemType) && !TreeItemType.isCongruenceAnalysisTypeLeaf(itemType) && !TreeItemType.isVerticalDeflectionTypeLeaf(itemType) && !(selectedItemValue is Groupable))
				{
					return;
				}

				try
				{
					IList<Groupable> treeItemValues = new List<Groupable>(selectedItems.Count);
					foreach (TreeItem<TreeItemValue> treeItem in selectedItems)
					{
						if (treeItem.getValue() == null || treeItem.getValue() == selectedItemValue || treeItem.getValue().getItemType() != itemType)
						{
							continue;
						}

						TreeItemValue itemValue = treeItem.getValue();

						if (!(itemValue is Groupable) || ((Groupable)itemValue).Dimension != ((Groupable)selectedItemValue).Dimension)
						{
							continue;
						}

						treeItemValues.Add((Groupable)itemValue);
						removeItems.Add(treeItem);
					}

					SQLManager.Instance.mergeGroups(selectedItemValue, treeItemValues);
					UITreeBuilder.Instance.Tree.getSelectionModel().clearSelection();
					if (removeItems != null && removeItems.Count > 0)
					{
						UITreeBuilder.Instance.removeItems(removeItems);
					}

					// load data to refresh group-ids of row data
					//SQLManager.getInstance().loadData(selectedItemValue);
					UITreeBuilder.Instance.Tree.getSelectionModel().select(selectedItem);

				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					OptionDialog.showThrowableDialog(i18n.getString("EditableMenuCheckBoxTreeCell.message.error.merge.exception.title", "Unexpected SQL-Error"), i18n.getString("EditableMenuCheckBoxTreeCell.message.error.merge.exception.header", "Error, could not merge selected tree items."), i18n.getString("EditableMenuCheckBoxTreeCell.message.error.merge.exception.message", "An exception has occurred during database transaction."), e);
				}
			}
		}

		private void adaptInstrumentAndReflectorHeights(TreeItem<TreeItemValue> selectedItem)
		{
			IList<TreeItem<TreeItemValue>> selectedItems = this.getTreeView().getSelectionModel().getSelectedItems();

			TreeItemValue[] selectedTreeItemValues = new TreeItemValue[selectedItems != null ? selectedItems.Count : 0];
			for (int i = 0; i < selectedItems.Count; i++)
			{
				selectedTreeItemValues[i] = selectedItems[i].getValue();
			}

	//		this.getTreeView().getSelectionModel().clearSelection();
			InstrumentAndReflectorHeightAdaptionDialog.showAndWait(selectedItem.getValue(), selectedTreeItemValues);
	//		this.getTreeView().getSelectionModel().select(selectedItem);
		}

		private void searchAndReplace(TreeItem<TreeItemValue> selectedItem)
		{
			IList<TreeItem<TreeItemValue>> selectedItems = this.getTreeView().getSelectionModel().getSelectedItems();

			TreeItemValue[] selectedTreeItemValues = new TreeItemValue[selectedItems != null ? selectedItems.Count : 0];
			for (int i = 0; i < selectedItems.Count; i++)
			{
				selectedTreeItemValues[i] = selectedItems[i].getValue();
			}

	//		this.getTreeView().getSelectionModel().clearSelection();
			SearchAndReplaceDialog.showAndWait(selectedItem.getValue(), selectedTreeItemValues);
	//		this.getTreeView().getSelectionModel().select(selectedItem);
		}

		private void exportTableData(TreeItemValue itemValue)
		{
			try
			{
				TabPane tabPane = UITabPaneBuilder.Instance.TabPane;
				if (tabPane != null && tabPane.getSelectionModel().getSelectedItem() != null && tabPane.getSelectionModel().getSelectedItem().getUserData() != null && tabPane.getSelectionModel().getSelectedItem().getUserData() is TabType)
				{

					TreeItemType itemType = itemValue.ItemType;
					TabType tabType = (TabType)tabPane.getSelectionModel().getSelectedItem().getUserData();

					bool aprioriValues = tabType == TabType.RAW_DATA;

					File selectedFile = DefaultFileChooser.showSaveDialog(JAG3D.Stage, i18n.getString("EditableMenuCheckBoxTreeCell.filechooser.save.title", "Export table data"), this.getItem().getName() + (aprioriValues ? "_apriori" : "_aposteriori") + ".txt");

					if (selectedFile != null)
					{
						if (TreeItemType.isPointTypeLeaf(itemType))
						{
							UIPointTableBuilder.Instance.export(selectedFile, aprioriValues);
						}

						else if (TreeItemType.isObservationTypeLeaf(itemType))
						{
							UITerrestrialObservationTableBuilder.Instance.export(selectedFile, aprioriValues);
						}

						else if (TreeItemType.isGNSSObservationTypeLeaf(itemType))
						{
							UIGNSSObservationTableBuilder.Instance.export(selectedFile, aprioriValues);
						}

						else if (TreeItemType.isCongruenceAnalysisTypeLeaf(itemType))
						{
							UICongruenceAnalysisTableBuilder.Instance.export(selectedFile, aprioriValues);
						}

						else if (TreeItemType.isVerticalDeflectionTypeLeaf(itemType))
						{
							UIVerticalDeflectionTableBuilder.Instance.export(selectedFile, aprioriValues);
						}
					}
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("EditableMenuCheckBoxTreeCell.message.error.export.exception.title", "I/O Error"), i18n.getString("EditableMenuCheckBoxTreeCell.message.error.export.exception.header", "Error, could not export selected table data."), i18n.getString("EditableMenuCheckBoxTreeCell.message.error.export.exception.message", "An exception has occurred during data export."), e);
			}
		}

		internal BooleanProperty ignoreEventProperty()
		{
			return this.ignoreEvent;
		}

		internal bool IgnoreEvent
		{
			get
			{
				return this.ignoreEventProperty().get();
			}
			set
			{
				this.ignoreEventProperty().set(value);
			}
		}

	}

}