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

namespace org.applied_geodesy.juniform.ui.tabpane
{
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using UIPointSelectionPaneBuilder = org.applied_geodesy.juniform.ui.propertiespane.UIPointSelectionPaneBuilder;
	using UIParameterTableBuilder = org.applied_geodesy.juniform.ui.table.UIParameterTableBuilder;
	using UIPointTableBuilder = org.applied_geodesy.juniform.ui.table.UIPointTableBuilder;
	using CurveTreeItemValue = org.applied_geodesy.juniform.ui.tree.CurveTreeItemValue;
	using SurfaceTreeItemValue = org.applied_geodesy.juniform.ui.tree.SurfaceTreeItemValue;
	using TreeItemType = org.applied_geodesy.juniform.ui.tree.TreeItemType;
	using org.applied_geodesy.juniform.ui.tree;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using ObservableMap = javafx.collections.ObservableMap;
	using Side = javafx.geometry.Side;
	using Node = javafx.scene.Node;
	using SingleSelectionModel = javafx.scene.control.SingleSelectionModel;
	using Tab = javafx.scene.control.Tab;
	using TabPane = javafx.scene.control.TabPane;
	using TableColumn = javafx.scene.control.TableColumn;
	using TableView = javafx.scene.control.TableView;
	using Tooltip = javafx.scene.control.Tooltip;

	public class UITabPaneBuilder
	{
		private bool InstanceFieldsInitialized = false;

		public UITabPaneBuilder()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			tabSelectionChangeListener = new TabSelectionChangeListener(this);
		}


		private class TabSelectionChangeListener : ChangeListener<Tab>
		{
			private readonly UITabPaneBuilder outerInstance;

			public TabSelectionChangeListener(UITabPaneBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Tab oldTab, Tab newTab) where T1 : javafx.scene.control.Tab
			{
				outerInstance.lastSelectedTab = newTab == null ? outerInstance.lastSelectedTab : newTab;

				// remove old Content
				if (oldTab != null)
				{
					oldTab.setContent(null);
				}

				if (newTab != null && newTab.getUserData() is TabType)
				{
					TabType tabType = (TabType)newTab.getUserData();
					newTab.setContent(outerInstance.getNode(tabType));
				}
			}
		}

		private TabPane tabPane = null;
		private static UITabPaneBuilder tabPaneBuilder = new UITabPaneBuilder();
		private I18N i18n = I18N.Instance;
		private TabSelectionChangeListener tabSelectionChangeListener;

		private UIPointTableBuilder pointTableBuilder = UIPointTableBuilder.Instance;
		private UIParameterTableBuilder parameterTableBuilder = UIParameterTableBuilder.Instance;

		private ObservableMap<TabType, Tab> tapMap = FXCollections.observableHashMap();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private org.applied_geodesy.juniform.ui.tree.TreeItemValue<?> lastTreeItemValue = null;
		private TreeItemValue<object> lastTreeItemValue = null;
		private Tab lastSelectedTab = null;

		public static UITabPaneBuilder Instance
		{
			get
			{
				return tabPaneBuilder;
			}
		}

		public virtual TabPane TabPane
		{
			get
			{
				if (this.tabPane == null)
				{
					this.init();
				}
				return this.tabPane;
			}
		}

		private void init()
		{
			this.createTab(i18n.getString("UITabPaneBuilder.tab.point.selection.label", "Point selection"), i18n.getString("UITabPaneBuilder.tab.point.selection.title", "Select points of geometric primitive"), TabType.POINT_SELECTION, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.point.apriori.label", "Observed points"), i18n.getString("UITabPaneBuilder.tab.point.apriori.title", "Table of observed points"), TabType.APRIORI_POINT, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.point.aposteriori.label", "Estimated points"), i18n.getString("UITabPaneBuilder.tab.point.aposteriori.title", "Table of estimated points"), TabType.APOSTERIORI_POINT, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.parameter.aposteriori.label", "Estimated parameters"), i18n.getString("UITabPaneBuilder.tab.parameter.aposteriori.title", "Table of estimated parameters"), TabType.APOSTERIORI_PARAMETER, null);

			this.tabPane = new TabPane();
			this.tabPane.setSide(Side.BOTTOM);

			this.tabPane.getSelectionModel().selectedItemProperty().addListener(this.tabSelectionChangeListener);
		}

		private Tab createTab(string name, string tooltip, TabType type, Node node)
		{
			Tab tab = new Tab(name, node);
			tab.setClosable(false);
			tab.setTooltip(new Tooltip(tooltip));
			tab.setUserData(type);
			this.tapMap.put(type, tab);
			return tab;
		}

		private Node getNode(TabType tabType)
		{
			if (this.lastTreeItemValue == null || tabType == null)
			{
				return null;
			}

			Node node = null;
			TreeItemType treeItemType = this.lastTreeItemValue.TreeItemType;
			switch (treeItemType)
			{
			case TreeItemType.ADJUSTMENT:

				break;

			case TreeItemType.FEATURE:
				if (tabType == TabType.APRIORI_POINT || tabType == TabType.APOSTERIORI_POINT)
				{
					TableView<FeaturePoint> pointTableView = this.pointTableBuilder.Table;
					this.setTableColumnView(tabType, pointTableView);
					node = pointTableView;
				}
				else if (tabType == TabType.APOSTERIORI_PARAMETER)
				{
					TableView<UnknownParameter> parameterTableView = this.parameterTableBuilder.Table;
					this.setTableColumnView(tabType, parameterTableView);
					node = parameterTableView;
				}
				break;

			case TreeItemType.LINE:
			case TreeItemType.CIRCLE:
			case TreeItemType.ELLIPSE:
			case TreeItemType.QUADRATIC_CURVE:
			case TreeItemType.PLANE:
			case TreeItemType.SPHERE:
			case TreeItemType.ELLIPSOID:
			case TreeItemType.CYLINDER:
			case TreeItemType.CONE:
			case TreeItemType.PARABOLOID:
			case TreeItemType.QUADRATIC_SURFACE:
				GeometricPrimitive geometry = null;
				if (this.lastTreeItemValue is CurveTreeItemValue)
				{
					geometry = ((CurveTreeItemValue)this.lastTreeItemValue).Object;
				}
				else if (this.lastTreeItemValue is SurfaceTreeItemValue)
				{
					geometry = ((SurfaceTreeItemValue)this.lastTreeItemValue).Object;
				}

				if (tabType == TabType.APOSTERIORI_PARAMETER)
				{
					TableView<UnknownParameter> parameterTableView = this.parameterTableBuilder.getTable(geometry);
					this.setTableColumnView(tabType, parameterTableView);
					node = parameterTableView;
				}
				else if (geometry != null && tabType == TabType.POINT_SELECTION)
				{
					TableView<FeaturePoint> pointTableView = this.pointTableBuilder.Table;
					node = UIPointSelectionPaneBuilder.Instance.getNode(pointTableView.getItems(), geometry);
				}
				break;

			default:
				throw new System.ArgumentException("Error, unknown tree item type " + treeItemType + "!");
	//			node = null;
	//			break;
			}
			return node;
		}

		private void setTableColumnView<T1>(TabType tabType, TableView<T1> tableView)
		{
			int columnCount = tableView.getColumns().size();

			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TableColumn<?, ?> column = tableView.getColumns().get(columnIndex);
				TableColumn<object, object> column = tableView.getColumns().get(columnIndex);
				if (column.getUserData() is ColumnType)
				{
					ColumnType columnType = (ColumnType)column.getUserData();
					switch (columnType)
					{
					case ColumnType.VISIBLE:
						column.setVisible(true);
						break;

					case ColumnType.HIDDEN:
						column.setVisible(false);
						break;

					case ColumnType.APRIORI_POINT:
						column.setVisible(tabType == TabType.APRIORI_POINT);
						break;

					case ColumnType.APOSTERIORI_POINT:
						column.setVisible(tabType == TabType.APOSTERIORI_POINT);
						break;

					default:
						throw new System.ArgumentException("Error, unsupported column type " + columnType + "!");
					}
				}
			}
		}

		public virtual TreeItemValue<T1> TreeItemValue<T1>
		{
			set
			{
				if (this.tabPane == null)
				{
					this.init();
				}
    
				SingleSelectionModel<Tab> selectionModel = this.tabPane.getSelectionModel();
				try
				{
					//selectionModel.selectedItemProperty().removeListener(this.tabSelectionChangeListener);
					this.lastSelectedTab = this.lastSelectedTab != null ? this.lastSelectedTab : selectionModel.getSelectedItem();
					selectionModel.clearSelection();
    
					this.lastTreeItemValue = value;
					if (this.tabPane != null && value != null)
					{
						TabType[] newTabTypes = value.TabTypes;
						if (newTabTypes != null && newTabTypes.Length > 0)
						{
							Tab selectedTab = null;
    
							ObservableList<Tab> oldTabList = tabPane.getTabs();
							bool equalTabOrderAndTypes = oldTabList.size() == newTabTypes.Length;
    
							if (equalTabOrderAndTypes)
							{
								for (int idx = 0; idx < newTabTypes.Length; idx++)
								{
									Tab tab = oldTabList.get(idx);
									if (tab.getUserData() == null || tab.getUserData() != newTabTypes[idx])
									{
										equalTabOrderAndTypes = false;
										break;
									}
								}
							}
    
							if (!equalTabOrderAndTypes)
							{
								ObservableList<Tab> newTabList = FXCollections.observableArrayList();
    
								foreach (TabType tabType in newTabTypes)
								{
									if (this.tapMap.containsKey(tabType))
									{
										newTabList.add(this.tapMap.get(tabType));
										if (this.lastSelectedTab != null && this.lastSelectedTab.getUserData() == tabType)
										{
											selectedTab = this.tapMap.get(tabType);
										}
									}
								}
								this.tabPane.getTabs().clear();
								this.tabPane.getTabs().addAll(newTabList);
							}
							else
							{
								bool validLastSelectedTabType = false;
								foreach (TabType newType in newTabTypes)
								{
									if (this.lastSelectedTab.getUserData() == newType)
									{
										validLastSelectedTabType = true;
										break;
									}
								}
    
								selectedTab = validLastSelectedTabType ? this.lastSelectedTab : null;
							}
    
							if (selectedTab == null && this.tabPane.getTabs().size() > 0)
							{
								selectedTab = this.tabPane.getTabs().get(0);
							}
    
							selectionModel.select(selectedTab);
    
							// setContent() is called by TabSelectionChangeListener
							//					if (selectedTab != null && selectedTab.getUserData() instanceof TabType) {
							//						TabType tabType = (TabType)selectedTab.getUserData();
							//						selectedTab.setContent(getNode(tabType));
							//					}
						}
						else
						{
							tabPane.getTabs().clear();
							Console.WriteLine(this.GetType().Name + " : No known tab types " + newTabTypes + " for " + value);
						}
					}
				}
				finally
				{
					//selectionModel.selectedItemProperty().addListener(this.tabSelectionChangeListener);
				}
			}
		}
	}

}