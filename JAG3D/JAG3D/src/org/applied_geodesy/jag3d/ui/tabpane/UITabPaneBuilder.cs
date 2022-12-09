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

namespace org.applied_geodesy.jag3d.ui.tabpane
{
	using UIGraphicPaneBuilder = org.applied_geodesy.jag3d.ui.graphic.UIGraphicPaneBuilder;
	using UIMetaDataPaneBuilder = org.applied_geodesy.jag3d.ui.metadata.UIMetaDataPaneBuilder;
	using UICongruenceAnalysisPropertiesPaneBuilder = org.applied_geodesy.jag3d.ui.propertiespane.UICongruenceAnalysisPropertiesPaneBuilder;
	using UIObservationPropertiesPaneBuilder = org.applied_geodesy.jag3d.ui.propertiespane.UIObservationPropertiesPaneBuilder;
	using UIPointPropertiesPaneBuilder = org.applied_geodesy.jag3d.ui.propertiespane.UIPointPropertiesPaneBuilder;
	using UIVerticalDeflectionPropertiesPaneBuilder = org.applied_geodesy.jag3d.ui.propertiespane.UIVerticalDeflectionPropertiesPaneBuilder;
	using UIGlobalResultPaneBuilder = org.applied_geodesy.jag3d.ui.resultpane.UIGlobalResultPaneBuilder;
	using UIAdditionalParameterTableBuilder = org.applied_geodesy.jag3d.ui.table.UIAdditionalParameterTableBuilder;
	using UICongruenceAnalysisTableBuilder = org.applied_geodesy.jag3d.ui.table.UICongruenceAnalysisTableBuilder;
	using UIGNSSObservationTableBuilder = org.applied_geodesy.jag3d.ui.table.UIGNSSObservationTableBuilder;
	using UIPointTableBuilder = org.applied_geodesy.jag3d.ui.table.UIPointTableBuilder;
	using UITerrestrialObservationTableBuilder = org.applied_geodesy.jag3d.ui.table.UITerrestrialObservationTableBuilder;
	using UIVarianceComponentTableBuilder = org.applied_geodesy.jag3d.ui.table.UIVarianceComponentTableBuilder;
	using UIVerticalDeflectionTableBuilder = org.applied_geodesy.jag3d.ui.table.UIVerticalDeflectionTableBuilder;
	using AdditionalParameterRow = org.applied_geodesy.jag3d.ui.table.row.AdditionalParameterRow;
	using CongruenceAnalysisRow = org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow;
	using GNSSObservationRow = org.applied_geodesy.jag3d.ui.table.row.GNSSObservationRow;
	using PointRow = org.applied_geodesy.jag3d.ui.table.row.PointRow;
	using Row = org.applied_geodesy.jag3d.ui.table.row.Row;
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;
	using VarianceComponentRow = org.applied_geodesy.jag3d.ui.table.row.VarianceComponentRow;
	using VerticalDeflectionRow = org.applied_geodesy.jag3d.ui.table.row.VerticalDeflectionRow;
	using CongruenceAnalysisTreeItemValue = org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using PointTreeItemValue = org.applied_geodesy.jag3d.ui.tree.PointTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using VerticalDeflectionTreeItemValue = org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
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

	//				// re-draw network plot
	//				if (lastTreeItemValue != null && tabType != null && lastTreeItemValue.getItemType() == TreeItemType.ROOT && tabType == TabType.GRAPHIC)
	//					UIGraphicPaneBuilder.getInstance().getLayerManager().redraw();
				}
			}
		}

		private TabPane tabPane = null;
		private static UITabPaneBuilder tabPaneBuilder = new UITabPaneBuilder();
		private I18N i18n = I18N.Instance;
		private TabSelectionChangeListener tabSelectionChangeListener;

		private UIPointTableBuilder pointTableBuilder = UIPointTableBuilder.Instance;
		private UITerrestrialObservationTableBuilder observationTableBuilder = UITerrestrialObservationTableBuilder.Instance;
		private UIGNSSObservationTableBuilder gnssObservationTableBuilder = UIGNSSObservationTableBuilder.Instance;
		private UICongruenceAnalysisTableBuilder congruenceAnalysisTableBuilder = UICongruenceAnalysisTableBuilder.Instance;
		private UIVerticalDeflectionTableBuilder verticalDeflectionTableBuilder = UIVerticalDeflectionTableBuilder.Instance;

		private ObservableMap<TabType, Tab> tapMap = FXCollections.observableHashMap();
		private TreeItemValue lastTreeItemValue = null;
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

			this.createTab(i18n.getString("UITabPaneBuilder.tab.metadata.label", "Metadata"), i18n.getString("UITabPaneBuilder.tab.metadata.tooltip", "Project-specific metadata"), TabType.META_DATA, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.graphic.label", "Graphic"), i18n.getString("UITabPaneBuilder.tab.graphic.tooltip", "Graphical visualisation of the network"), TabType.GRAPHIC, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.raw.label", "Raw data"), i18n.getString("UITabPaneBuilder.tab.raw.tooltip", "Table of raw data"), TabType.RAW_DATA, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.properties.label", "Properties"), i18n.getString("UITabPaneBuilder.tab.properties.tooltip", "Properties of group"), TabType.PROPERTIES, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.result.label", "Result data"), i18n.getString("UITabPaneBuilder.tab.result.tooltip", "Table of estimated data"), TabType.RESULT_DATA, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.param.label", "Additional parameters"), i18n.getString("UITabPaneBuilder.tab.param.tooltip", "Table of additional parameters"), TabType.ADDITIONAL_PARAMETER, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.congruence.point.label", "Congruence of points"), i18n.getString("UITabPaneBuilder.tab.congruence.point.tooltip", "Result of congruence analysis of point"), TabType.RESULT_CONGRUENCE_ANALYSIS, null);

			this.createTab(i18n.getString("UITabPaneBuilder.tab.variance_component.label", "Variance components"), i18n.getString("UITabPaneBuilder.tab.variance_component.tooltip", "Table of variance components estimation"), TabType.VARIANCE_COMPONENT, null);

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

		// show tabs defined in TreeItemValue
		private Node getNode(TabType tabType)
		{
			if (this.lastTreeItemValue == null || tabType == null)
			{
				return null;
			}

			Node node = null;
			TreeItemType treeItemType = this.lastTreeItemValue.ItemType;
			switch (treeItemType.innerEnumValue)
			{
			case TreeItemType.InnerEnum.ROOT:
				if (tabType == TabType.META_DATA)
				{
					node = UIMetaDataPaneBuilder.Instance.Node;
				}
				else if (tabType == TabType.RESULT_DATA)
				{
					node = UIGlobalResultPaneBuilder.Instance.Node;
				}
				else if (tabType == TabType.GRAPHIC)
				{
					UIGraphicPaneBuilder graphicPaneBuilder = UIGraphicPaneBuilder.Instance;
					node = graphicPaneBuilder.Pane;
					// re-draw network plot
					Platform.runLater(() =>
					{
					graphicPaneBuilder.LayerManager.redraw();
					});
				}
				break;

			case TreeItemType.InnerEnum.REFERENCE_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.DATUM_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.NEW_POINT_1D_LEAF:
			case TreeItemType.InnerEnum.REFERENCE_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.DATUM_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.NEW_POINT_2D_LEAF:
			case TreeItemType.InnerEnum.REFERENCE_POINT_3D_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
			case TreeItemType.InnerEnum.DATUM_POINT_3D_LEAF:
			case TreeItemType.InnerEnum.NEW_POINT_3D_LEAF:
				PointTreeItemValue pointItemValue = (PointTreeItemValue)this.lastTreeItemValue;
				if (tabType == TabType.RAW_DATA || tabType == TabType.RESULT_DATA || tabType == TabType.RESULT_CONGRUENCE_ANALYSIS)
				{
					TableView<PointRow> pointTableView = this.pointTableBuilder.getTable(pointItemValue);
					this.setTableColumnView(tabType, pointTableView);
					node = pointTableView;
				}
				else if (tabType == TabType.PROPERTIES)
				{
					UIPointPropertiesPaneBuilder pointPropertiesBuilder = UIPointPropertiesPaneBuilder.Instance;
					node = pointPropertiesBuilder.getPointPropertiesPane(pointItemValue.ItemType).Node;
				}
				else if (tabType == TabType.VARIANCE_COMPONENT)
				{
					UIVarianceComponentTableBuilder tableBuilder = UIVarianceComponentTableBuilder.Instance;
					TableView<VarianceComponentRow> table = tableBuilder.getTable(UIVarianceComponentTableBuilder.VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
					this.setTableColumnView(tabType, table);
					node = table;
				}

				break;
			case TreeItemType.InnerEnum.LEVELING_LEAF:
			case TreeItemType.InnerEnum.DIRECTION_LEAF:
			case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
			case TreeItemType.InnerEnum.GNSS_1D_LEAF:
			case TreeItemType.InnerEnum.GNSS_2D_LEAF:
			case TreeItemType.InnerEnum.GNSS_3D_LEAF:
				ObservationTreeItemValue observationItemValue = (ObservationTreeItemValue)this.lastTreeItemValue;

				if (tabType == TabType.RAW_DATA || tabType == TabType.RESULT_DATA)
				{
					switch (treeItemType.innerEnumValue)
					{
					case TreeItemType.InnerEnum.LEVELING_LEAF:
					case TreeItemType.InnerEnum.DIRECTION_LEAF:
					case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
					case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
					case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
						TableView<TerrestrialObservationRow> observationTableView = this.observationTableBuilder.getTable(observationItemValue);
						this.setTableColumnView(tabType, observationTableView);
						node = observationTableView;
						break;
					case TreeItemType.InnerEnum.GNSS_1D_LEAF:
					case TreeItemType.InnerEnum.GNSS_2D_LEAF:
					case TreeItemType.InnerEnum.GNSS_3D_LEAF:
						TableView<GNSSObservationRow> gnssObservationTableView = this.gnssObservationTableBuilder.getTable(observationItemValue);
						this.setTableColumnView(tabType, gnssObservationTableView);
						node = gnssObservationTableView;
						break;
					default:
						break;
					}
				}
				else if (tabType == TabType.PROPERTIES)
				{
					UIObservationPropertiesPaneBuilder terrestrialPropertiesBuilder = UIObservationPropertiesPaneBuilder.Instance;
					node = terrestrialPropertiesBuilder.getObservationPropertiesPane(observationItemValue.ItemType).Node;
				}
				else if (tabType == TabType.ADDITIONAL_PARAMETER)
				{
					UIAdditionalParameterTableBuilder tableBuilder = UIAdditionalParameterTableBuilder.Instance;
					TableView<AdditionalParameterRow> table = tableBuilder.Table;
					node = table;
				}
				else if (tabType == TabType.VARIANCE_COMPONENT)
				{
					UIVarianceComponentTableBuilder tableBuilder = UIVarianceComponentTableBuilder.Instance;
					TableView<VarianceComponentRow> table = tableBuilder.getTable(UIVarianceComponentTableBuilder.VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
					this.setTableColumnView(tabType, table);
					node = table;
				}
				break;

			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
				CongruenceAnalysisTreeItemValue congruenceAnalysisItemValue = (CongruenceAnalysisTreeItemValue)this.lastTreeItemValue;
				if (tabType == TabType.RAW_DATA || tabType == TabType.RESULT_CONGRUENCE_ANALYSIS)
				{
					TableView<CongruenceAnalysisRow> congruenceAnalysisTableView = this.congruenceAnalysisTableBuilder.getTable(congruenceAnalysisItemValue);
					this.setTableColumnView(tabType, congruenceAnalysisTableView);
					node = congruenceAnalysisTableView;
				}
				else if (tabType == TabType.PROPERTIES)
				{
					UICongruenceAnalysisPropertiesPaneBuilder congruenceAnalysisPropertiesBuilder = UICongruenceAnalysisPropertiesPaneBuilder.Instance;
					node = congruenceAnalysisPropertiesBuilder.getCongruenceAnalysisPropertiesPane(congruenceAnalysisItemValue.ItemType).Node;
				}
				else if (tabType == TabType.ADDITIONAL_PARAMETER)
				{
					UIAdditionalParameterTableBuilder tableBuilder = UIAdditionalParameterTableBuilder.Instance;
					TableView<AdditionalParameterRow> table = tableBuilder.Table;
					node = table;
				}
				break;

			case TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_LEAF:
			case TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_LEAF:
				VerticalDeflectionTreeItemValue verticalDeflectionTreeItemValue = (VerticalDeflectionTreeItemValue)this.lastTreeItemValue;
				if (tabType == TabType.RAW_DATA || tabType == TabType.RESULT_DATA)
				{
					TableView<VerticalDeflectionRow> verticalDeflectionTableView = this.verticalDeflectionTableBuilder.getTable(verticalDeflectionTreeItemValue);
					this.setTableColumnView(tabType, verticalDeflectionTableView);
					node = verticalDeflectionTableView;
				}
				else if (tabType == TabType.PROPERTIES)
				{
					UIVerticalDeflectionPropertiesPaneBuilder verticalDeflectionPropertiesPaneBuilder = UIVerticalDeflectionPropertiesPaneBuilder.Instance;
					node = verticalDeflectionPropertiesPaneBuilder.getVerticalDeflectionPropertiesPane(verticalDeflectionTreeItemValue.ItemType).Node;
				}
				else if (tabType == TabType.VARIANCE_COMPONENT)
				{
					UIVarianceComponentTableBuilder tableBuilder = UIVarianceComponentTableBuilder.Instance;
					TableView<VarianceComponentRow> table = tableBuilder.getTable(UIVarianceComponentTableBuilder.VarianceComponentDisplayType.SELECTED_GROUP_COMPONENTS);
					this.setTableColumnView(tabType, table);
					node = table;
				}

				break;

			default:
				node = null;
				break;
			}
			return node;
		}

		private void setTableColumnView<T1>(TabType tabType, TableView<T1> tableView) where T1 : org.applied_geodesy.jag3d.ui.table.row.Row
		{
			int columnCount = tableView.getColumns().size();

			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TableColumn<? extends org.applied_geodesy.jag3d.ui.table.row.Row, ?> column = tableView.getColumns().get(columnIndex);
				TableColumn<Row, object> column = tableView.getColumns().get(columnIndex);
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

					case ColumnType.APRIORI_TERRESTRIAL_OBSERVATION:
					case ColumnType.APRIORI_GNSS_OBSERVATION:
					case ColumnType.APRIORI_POINT_CONGRUENCE:
					case ColumnType.APRIORI_POINT:
					case ColumnType.APRIORI_DEFLECTION:
						column.setVisible(tabType == TabType.RAW_DATA);

						break;

					case ColumnType.APOSTERIORI_TERRESTRIAL_OBSERVATION:
					case ColumnType.APOSTERIORI_GNSS_OBSERVATION:
					case ColumnType.APOSTERIORI_POINT:
					case ColumnType.APOSTERIORI_DEFLECTION:
						column.setVisible(tabType == TabType.RESULT_DATA);

						break;

					case ColumnType.APOSTERIORI_POINT_CONGRUENCE:
						column.setVisible(tabType == TabType.RESULT_CONGRUENCE_ANALYSIS);

						break;

					default:
						break;
					}
				}
			}
		}

		public virtual TreeItemValue TreeItemValue
		{
			set
			{
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
							//System.out.println(this.getClass().getSimpleName() + " : No known tab types " + newTabTypes + " for " + value);
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