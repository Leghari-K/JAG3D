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

	using DefaultApplicationProperty = org.applied_geodesy.jag3d.DefaultApplicationProperty;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using TabType = org.applied_geodesy.jag3d.ui.tabpane.TabType;
	using UITabPaneBuilder = org.applied_geodesy.jag3d.ui.tabpane.UITabPaneBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using UIGraphicPaneBuilder = org.applied_geodesy.jag3d.ui.graphic.UIGraphicPaneBuilder;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using FXCollections = javafx.collections.FXCollections;
	using ListChangeListener = javafx.collections.ListChangeListener;
	using ObservableList = javafx.collections.ObservableList;
	using ObservableMap = javafx.collections.ObservableMap;
	using EventHandler = javafx.@event.EventHandler;
	using ButtonType = javafx.scene.control.ButtonType;
	using CheckBoxTreeItem = javafx.scene.control.CheckBoxTreeItem;
	using MultipleSelectionModel = javafx.scene.control.MultipleSelectionModel;
	using SelectionMode = javafx.scene.control.SelectionMode;
	using Tab = javafx.scene.control.Tab;
	using TreeCell = javafx.scene.control.TreeCell;
	using TreeItem = javafx.scene.control.TreeItem;
	using TreeView = javafx.scene.control.TreeView;
	using KeyCode = javafx.scene.input.KeyCode;
	using KeyEvent = javafx.scene.input.KeyEvent;
	using Callback = javafx.util.Callback;

	public class UITreeBuilder
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			treeListSelectionChangeListener = new TreeListSelectionChangeListener(this);
		}


		private class TreeCheckBoxChangeListener : ChangeListener<bool>
		{
			private readonly UITreeBuilder outerInstance;

			internal readonly TreeItemValue treeItemValue;

			internal TreeCheckBoxChangeListener(UITreeBuilder outerInstance, TreeItemValue treeItemValue)
			{
				this.outerInstance = outerInstance;
				this.treeItemValue = treeItemValue;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				bool isSaved = false;
				if (!outerInstance.IgnoreEvent)
				{
					IList<TreeItem<TreeItemValue>> treeItems = outerInstance.treeView.getSelectionModel().getSelectedItems();
					foreach (TreeItem<TreeItemValue> treeItem in treeItems)
					{
						if (treeItem.isLeaf() && treeItem.getValue().getItemType() == this.treeItemValue.ItemType)
						{
							treeItem.getValue().setEnable(newValue);
							isSaved = treeItem.getValue() == this.treeItemValue;
							outerInstance.save(treeItem.getValue());
						}
					}
					if (!isSaved)
					{
						outerInstance.save(this.treeItemValue);
					}

					// refresh network plot, if visibility of group has changed
					Tab selectedTab = UITabPaneBuilder.Instance.TabPane.getSelectionModel().getSelectedItem();
					if (selectedTab != null && selectedTab.getUserData() is TabType && ((TabType)selectedTab.getUserData()) == TabType.GRAPHIC)
					{
						Platform.runLater(() =>
						{
						UIGraphicPaneBuilder.Instance.LayerManager.redraw();
						});
					}
				}
			}
		}

		private class TreeItemNameChangeListener : ChangeListener<string>
		{
			private readonly UITreeBuilder outerInstance;

			internal readonly TreeItemValue treeItemValue;

			internal TreeItemNameChangeListener(UITreeBuilder outerInstance, TreeItemValue treeItemValue)
			{
				this.outerInstance = outerInstance;
				this.treeItemValue = treeItemValue;
			}

			public override void changed<T1>(ObservableValue<T1> observable, string oldValue, string newValue) where T1 : string
			{
				if (outerInstance.IgnoreEvent || string.ReferenceEquals(newValue, null) || newValue.Trim().Length == 0)
				{
					this.treeItemValue.Name = oldValue;
				}
				else if (!outerInstance.IgnoreEvent)
				{
					outerInstance.save(this.treeItemValue);
				}
			}
		}

		private class TreeItemExpandingChangeListener : ChangeListener<bool>
		{
			private readonly UITreeBuilder outerInstance;

			internal readonly TreeItem<TreeItemValue> treeItem;

			internal TreeItemExpandingChangeListener(UITreeBuilder outerInstance, TreeItem<TreeItemValue> treeItem)
			{
				this.outerInstance = outerInstance;
				this.treeItem = treeItem;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (newValue.Value && !outerInstance.ignoreExpanding)
				{
					outerInstance.selectChildren(treeItem);
				}
			}
		}

	//	private class TreeSelectionChangeListener implements ChangeListener<TreeItem<TreeItemValue>> {
	//		@Override
	//		public void changed(ObservableValue<? extends TreeItem<TreeItemValue>> observable, TreeItem<TreeItemValue> oldValue, TreeItem<TreeItemValue> newValue) {
	//			handleTreeSelections(newValue);
	//		}
	//	}

		private class TreeListSelectionChangeListener : ListChangeListener<TreeItem<TreeItemValue>>
		{
			private readonly UITreeBuilder outerInstance;

			public TreeListSelectionChangeListener(UITreeBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void onChanged<T1>(Change<T1> change) where T1 : javafx.scene.control.TreeItem<TreeItemValue>
			{
				if (!outerInstance.ignoreTreeSelection && change != null && change.next() && outerInstance.treeView != null && outerInstance.treeView.getSelectionModel() != null && outerInstance.treeView.getSelectionModel().getSelectedItems().size() > 0)
				{
					TreeItem<TreeItemValue> treeItem = null;
					bool hasValidTreeItem = false;

					try
					{
						if ((change.wasAdded() || change.wasReplaced()) && change.getAddedSubList() != null && !change.getAddedSubList().isEmpty())
						{
							treeItem = change.getAddedSubList().get(0);
							hasValidTreeItem = true;
						}
					}
					catch (Exception e)
					{
						hasValidTreeItem = false;
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}

					if (!hasValidTreeItem)
					{
						treeItem = outerInstance.treeView.getSelectionModel().getSelectedItem();
						int treeItemIndex = outerInstance.treeView.getSelectionModel().getSelectedIndex();
						if (treeItemIndex < 0 || treeItem == null || !outerInstance.treeView.getSelectionModel().isSelected(treeItemIndex))
						{
							treeItem = outerInstance.treeView.getSelectionModel().getSelectedItems().get(0);
						}
					}

					outerInstance.handleTreeSelections(treeItem);
				}
			}
		}

		private static UITreeBuilder treeBuilder = new UITreeBuilder();
		private I18N i18n = I18N.Instance;
		private UITabPaneBuilder tabPaneBuilder = UITabPaneBuilder.Instance;
		private ObservableMap<TreeItemType, CheckBoxTreeItem<TreeItemValue>> directoryItemMap = FXCollections.observableHashMap();
		private TreeItem<TreeItemValue> lastValidSelectedTreeItem = null;
		private TreeView<TreeItemValue> treeView;
		private bool ignoreExpanding = false;
		private BooleanProperty ignoreEvent = new SimpleBooleanProperty(false);
	//	private TreeSelectionChangeListener treeSelectionChangeListener = new TreeSelectionChangeListener();
		private TreeListSelectionChangeListener treeListSelectionChangeListener;
		private bool ignoreTreeSelection = false;
		private UITreeBuilder()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public static UITreeBuilder Instance
		{
			get
			{
				if (treeBuilder.treeView == null)
				{
					treeBuilder.init();
				}
				return treeBuilder;
			}
		}

		public virtual TreeView<TreeItemValue> Tree
		{
			get
			{
				return this.treeView;
			}
		}

		private void init()
		{
			// TreeItemType.ROOT
			TreeItem<TreeItemValue> rootItem = new TreeItem<TreeItemValue> (new RootTreeItemValue(i18n.getString("UITreeBuiler.root", "JAG3D-Project")));

			TreeItem<TreeItemValue> referencePointItem = new TreeItem<TreeItemValue> (new TreeItemValue(i18n.getString("UITreeBuiler.directory.points.reference", "Reference points")));
			CheckBoxTreeItem<TreeItemValue> referencePoint1DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.REFERENCE_POINT_1D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.reference.1d", "Reference points 1D")));
			CheckBoxTreeItem<TreeItemValue> referencePoint2DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.REFERENCE_POINT_2D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.reference.2d", "Reference points 2D")));
			CheckBoxTreeItem<TreeItemValue> referencePoint3DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.REFERENCE_POINT_3D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.reference.3d", "Reference points 3D")));
			referencePointItem.getChildren().addAll(Arrays.asList(referencePoint1DItem, referencePoint2DItem, referencePoint3DItem));

			TreeItem<TreeItemValue> stochasticPointItem = new TreeItem<TreeItemValue> (new TreeItemValue(i18n.getString("UITreeBuiler.directory.points.stochastic", "Stochastic points")));
			CheckBoxTreeItem<TreeItemValue> stochasticPoint1DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.STOCHASTIC_POINT_1D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.stochastic.1d", "Stochastic points 1D")));
			CheckBoxTreeItem<TreeItemValue> stochasticPoint2DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.STOCHASTIC_POINT_2D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.stochastic.2d", "Stochastic points 2D")));
			CheckBoxTreeItem<TreeItemValue> stochasticPoint3DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.STOCHASTIC_POINT_3D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.stochastic.3d", "Stochastic points 3D")));
			stochasticPointItem.getChildren().addAll(Arrays.asList(stochasticPoint1DItem, stochasticPoint2DItem, stochasticPoint3DItem));

			TreeItem<TreeItemValue> datumPointItem = new TreeItem<TreeItemValue> (new TreeItemValue(i18n.getString("UITreeBuiler.directory.points.datum", "Datum points")));
			CheckBoxTreeItem<TreeItemValue> datumPoint1DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.DATUM_POINT_1D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.datum.1d", "Datum points 1D")));
			CheckBoxTreeItem<TreeItemValue> datumPoint2DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.DATUM_POINT_2D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.datum.2d", "Datum points 2D")));
			CheckBoxTreeItem<TreeItemValue> datumPoint3DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.DATUM_POINT_3D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.datum.3d", "Datum points 3D")));
			datumPointItem.getChildren().addAll(Arrays.asList(datumPoint1DItem, datumPoint2DItem, datumPoint3DItem));

			TreeItem<TreeItemValue> newPointItem = new TreeItem<TreeItemValue> (new TreeItemValue(i18n.getString("UITreeBuiler.directory.points.new", "New points")));
			CheckBoxTreeItem<TreeItemValue> newPoint1DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.NEW_POINT_1D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.new.1d", "New points 1D")));
			CheckBoxTreeItem<TreeItemValue> newPoint2DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.NEW_POINT_2D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.new.2d", "New points 2D")));
			CheckBoxTreeItem<TreeItemValue> newPoint3DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.NEW_POINT_3D_DIRECTORY, i18n.getString("UITreeBuiler.directory.points.new.3d", "New points 3D")));
			newPointItem.getChildren().addAll(Arrays.asList(newPoint1DItem, newPoint2DItem, newPoint3DItem));

			TreeItem<TreeItemValue> verticalDeflectionItem = new TreeItem<TreeItemValue> (new TreeItemValue(i18n.getString("UITreeBuiler.directory.vertical_deflection", "Vertical deflection")));
			CheckBoxTreeItem<TreeItemValue> referenceVerticalDeflectionItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.REFERENCE_VERTICAL_DEFLECTION_DIRECTORY, i18n.getString("UITreeBuiler.directory.vertical_deflection.reference", "Reference deflection")));
			CheckBoxTreeItem<TreeItemValue> stochasticVerticalDeflectionItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY, i18n.getString("UITreeBuiler.directory.vertical_deflection.stochastic", "Stochastic deflection")));
			CheckBoxTreeItem<TreeItemValue> newVerticalDeflectionItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY, i18n.getString("UITreeBuiler.directory.vertical_deflection.unknown", "Unknown deflection")));
			verticalDeflectionItem.getChildren().addAll(Arrays.asList(referenceVerticalDeflectionItem, stochasticVerticalDeflectionItem, newVerticalDeflectionItem));


			TreeItem<TreeItemValue> congruenceAnalysisItem = new TreeItem<TreeItemValue> (new TreeItemValue(i18n.getString("UITreeBuiler.directory.congruenceanalysis", "Congruence analysis")));
			CheckBoxTreeItem<TreeItemValue> congruenceAnalysis1DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.CONGRUENCE_ANALYSIS_1D_DIRECTORY, i18n.getString("UITreeBuiler.directory.congruenceanalysis.1d", "Point nexus 1D")));
			CheckBoxTreeItem<TreeItemValue> congruenceAnalysis2DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.CONGRUENCE_ANALYSIS_2D_DIRECTORY, i18n.getString("UITreeBuiler.directory.congruenceanalysis.2d", "Point nexus 2D")));
			CheckBoxTreeItem<TreeItemValue> congruenceAnalysis3DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.CONGRUENCE_ANALYSIS_3D_DIRECTORY, i18n.getString("UITreeBuiler.directory.congruenceanalysis.3d", "Point nexus 3D")));
			congruenceAnalysisItem.getChildren().addAll(Arrays.asList(congruenceAnalysis1DItem, congruenceAnalysis2DItem, congruenceAnalysis3DItem));


			TreeItem<TreeItemValue> terrestrialObservationItem = new TreeItem<TreeItemValue> (new TreeItemValue(i18n.getString("UITreeBuiler.directory.observations", "Terrestrial Observations")));
			TreeItem<TreeItemValue> levelingObservationItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.LEVELING_DIRECTORY, i18n.getString("UITreeBuiler.directory.observations.leveling", "Leveling data")));
			TreeItem<TreeItemValue> directionObservationItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.DIRECTION_DIRECTORY, i18n.getString("UITreeBuiler.directory.observations.direction", "Direction sets")));
			TreeItem<TreeItemValue> distance2dObservationItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.HORIZONTAL_DISTANCE_DIRECTORY, i18n.getString("UITreeBuiler.directory.observations.horizontal_distance", "Horizontal distances")));
			TreeItem<TreeItemValue> distance3dObservationItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.SLOPE_DISTANCE_DIRECTORY, i18n.getString("UITreeBuiler.directory.observations.slope_distance", "Slope distances")));
			TreeItem<TreeItemValue> zenithObservationItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.ZENITH_ANGLE_DIRECTORY, i18n.getString("UITreeBuiler.directory.observations.zenith_angle", "Zenith angles")));
			terrestrialObservationItem.getChildren().addAll(Arrays.asList(levelingObservationItem, directionObservationItem, distance2dObservationItem, distance3dObservationItem, zenithObservationItem));

			TreeItem<TreeItemValue> gnssBaselineItem = new TreeItem<TreeItemValue> (new TreeItemValue(i18n.getString("UITreeBuiler.directory.gnss", "GNSS baselines")));
			CheckBoxTreeItem<TreeItemValue> gnssBaseline1DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.GNSS_1D_DIRECTORY, i18n.getString("UITreeBuiler.directory.gnss.1d", "GNSS baselines 1D")));
			CheckBoxTreeItem<TreeItemValue> gnssBaseline2DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.GNSS_2D_DIRECTORY, i18n.getString("UITreeBuiler.directory.gnss.2d", "GNSS baselines 2D")));
			CheckBoxTreeItem<TreeItemValue> gnssBaseline3DItem = new CheckBoxTreeItem<TreeItemValue> (new TreeItemValue(TreeItemType.GNSS_3D_DIRECTORY, i18n.getString("UITreeBuiler.directory.gnss.3d", "GNSS baselines 3D")));
			gnssBaselineItem.getChildren().addAll(Arrays.asList(gnssBaseline1DItem, gnssBaseline2DItem, gnssBaseline3DItem));

			foreach (TreeItem<TreeItemValue> item in referencePointItem.getChildren())
			{
				item.expandedProperty().addListener(new TreeItemExpandingChangeListener(this, item));
				this.directoryItemMap.put(item.getValue().getItemType(), (CheckBoxTreeItem<TreeItemValue>)item);
			}
			foreach (TreeItem<TreeItemValue> item in stochasticPointItem.getChildren())
			{
				item.expandedProperty().addListener(new TreeItemExpandingChangeListener(this, item));
				this.directoryItemMap.put(item.getValue().getItemType(), (CheckBoxTreeItem<TreeItemValue>)item);
			}
			foreach (TreeItem<TreeItemValue> item in datumPointItem.getChildren())
			{
				item.expandedProperty().addListener(new TreeItemExpandingChangeListener(this, item));
				this.directoryItemMap.put(item.getValue().getItemType(), (CheckBoxTreeItem<TreeItemValue>)item);
			}
			foreach (TreeItem<TreeItemValue> item in newPointItem.getChildren())
			{
				item.expandedProperty().addListener(new TreeItemExpandingChangeListener(this, item));
				this.directoryItemMap.put(item.getValue().getItemType(), (CheckBoxTreeItem<TreeItemValue>)item);
			}
			foreach (TreeItem<TreeItemValue> item in verticalDeflectionItem.getChildren())
			{
				item.expandedProperty().addListener(new TreeItemExpandingChangeListener(this, item));
				this.directoryItemMap.put(item.getValue().getItemType(), (CheckBoxTreeItem<TreeItemValue>)item);
			}
			foreach (TreeItem<TreeItemValue> item in congruenceAnalysisItem.getChildren())
			{
				item.expandedProperty().addListener(new TreeItemExpandingChangeListener(this, item));
				this.directoryItemMap.put(item.getValue().getItemType(), (CheckBoxTreeItem<TreeItemValue>)item);
			}
			foreach (TreeItem<TreeItemValue> item in terrestrialObservationItem.getChildren())
			{
				item.expandedProperty().addListener(new TreeItemExpandingChangeListener(this, item));
				this.directoryItemMap.put(item.getValue().getItemType(), (CheckBoxTreeItem<TreeItemValue>)item);
			}
			foreach (TreeItem<TreeItemValue> item in gnssBaselineItem.getChildren())
			{
				item.expandedProperty().addListener(new TreeItemExpandingChangeListener(this, item));
				this.directoryItemMap.put(item.getValue().getItemType(), (CheckBoxTreeItem<TreeItemValue>)item);
			}

			// Add first Level to root item
			rootItem.getChildren().addAll(Arrays.asList(referencePointItem, stochasticPointItem, datumPointItem, newPointItem, verticalDeflectionItem, congruenceAnalysisItem, terrestrialObservationItem, gnssBaselineItem));
			rootItem.setExpanded(true);

			this.treeView = new TreeView<TreeItemValue>(rootItem);
			this.treeView.setEditable(true);
			this.treeView.setCellFactory(new CallbackAnonymousInnerClass(this));
			this.treeView.getSelectionModel().select(rootItem);
			this.treeView.getSelectionModel().setSelectionMode(SelectionMode.MULTIPLE);
	//		this.treeView.getSelectionModel().selectedItemProperty().addListener(this.treeSelectionChangeListener);
			this.treeView.getSelectionModel().getSelectedItems().addListener(this.treeListSelectionChangeListener);

			this.addTreeKeyEvents();
		}

		private class CallbackAnonymousInnerClass : Callback<TreeView<TreeItemValue>, TreeCell<TreeItemValue>>
		{
			private readonly UITreeBuilder outerInstance;

			public CallbackAnonymousInnerClass(UITreeBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override TreeCell<TreeItemValue> call(TreeView<TreeItemValue> treeView)
			{
				EditableMenuCheckBoxTreeCell editableMenuCheckBoxTreeCell = new EditableMenuCheckBoxTreeCell();
				editableMenuCheckBoxTreeCell.ignoreEventProperty().bindBidirectional(outerInstance.ignoreEvent);
				return editableMenuCheckBoxTreeCell; //new EditableMenuCheckBoxTreeCell();
			}
		}

		public virtual void removeAllItems()
		{
			foreach (CheckBoxTreeItem<TreeItemValue> item in this.directoryItemMap.values())
			{
				item.getChildren().clear();
				item.setSelected(false);
			}
			//this.treeView.getSelectionModel().selectFirst();
		}

		public virtual TreeItem<TreeItemValue> addItem(TreeItemType parentType)
		{
			return this.addItem(parentType, null);
		}

		public virtual TreeItem<TreeItemValue> addItem(TreeItemType parentType, bool select)
		{
			return this.addItem(parentType, -1, null, true, select);
		}

		public virtual TreeItem<TreeItemValue> addItem(TreeItemType parentType, string name)
		{
			return this.addItem(parentType, -1, name, true, true);
		}

		public virtual TreeItem<TreeItemValue> addItem(TreeItemType parentType, int id, string name, bool enable, bool select)
		{
			TreeItemType itemType = null;
			if (parentType == null || parentType == TreeItemType.UNSPECIFIC)
			{
				return null;
			}

			if (!this.directoryItemMap.containsKey(parentType) || (itemType = TreeItemType.getLeafByDirectoryType(parentType)) == null)
			{
				Console.Error.WriteLine(this.GetType().Name + " : Error, unsupported parent tree node type " + parentType);
				return null;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TreeItemValue itemValue;
			TreeItemValue itemValue;

			int orderId = this.directoryItemMap.get(parentType).getChildren().size();

			if (TreeItemType.isPointTypeLeaf(itemType))
			{
				itemValue = new PointTreeItemValue(id, itemType, string.ReferenceEquals(name, null) || name.Trim().Length == 0 ? i18n.getString("UITreeBuiler.directory.points", "Points") : name, orderId);
			}

			else if (TreeItemType.isObservationTypeLeaf(itemType) || TreeItemType.isGNSSObservationTypeLeaf(itemType))
			{
				itemValue = new ObservationTreeItemValue(id, itemType, string.ReferenceEquals(name, null) || name.Trim().Length == 0 ? i18n.getString("UITreeBuiler.directory.observations", "Observations") : name, orderId);
			}

			else if (TreeItemType.isVerticalDeflectionTypeLeaf(itemType))
			{
				itemValue = new VerticalDeflectionTreeItemValue(id, itemType, string.ReferenceEquals(name, null) || name.Trim().Length == 0 ? i18n.getString("UITreeBuiler.directory.vertical_deflection", "Vertical deflection") : name, orderId);
			}

			else if (TreeItemType.isCongruenceAnalysisTypeLeaf(itemType))
			{
				itemValue = new CongruenceAnalysisTreeItemValue(id, itemType, string.ReferenceEquals(name, null) || name.Trim().Length == 0 ? i18n.getString("UITreeBuiler.directory.congruenceanalysis", "Point nexus") : name, orderId);
			}

			else
			{
				throw new System.ArgumentException(this.GetType().Name + " NOT IMPLEMENTED YET!");
			}

			CheckBoxTreeItem<TreeItemValue> newItem = new CheckBoxTreeItem<TreeItemValue>(itemValue);
			itemValue.Enable = enable;

			this.directoryItemMap.get(parentType).getChildren().add(newItem);
			this.treeView.getSelectionModel().clearSelection();

			this.expand(newItem, true);
			if (select)
			{
				this.treeView.getSelectionModel().select(newItem);
			}

			newItem.selectedProperty().bindBidirectional(itemValue.enableProperty());
			newItem.selectedProperty().addListener(new TreeCheckBoxChangeListener(this, newItem.getValue()));
			newItem.getValue().nameProperty().addListener(new TreeItemNameChangeListener(this, newItem.getValue()));

			return newItem;
		}

		public virtual void removeItems(IList<TreeItem<TreeItemValue>> items)
		{
			if (items != null && items.Count > 0)
			{
				TreeItemType parentItemType = null;
				foreach (TreeItem<TreeItemValue> item in items)
				{
					this.removeItem(item);
					if (parentItemType == null && item.getValue() != null)
					{
						parentItemType = TreeItemType.getDirectoryByLeafType(item.getValue().getItemType());
					}
				}
			}
		}

		public virtual void removeItem(TreeItem<TreeItemValue> treeItem)
		{
			if (treeItem == null || treeItem.getValue() == null || treeItem.getValue().getItemType() == null)
			{
				return;
			}

			TreeItemValue itemValue = treeItem.getValue();
			TreeItemType itemType = itemValue.ItemType;

			if (!TreeItemType.isPointTypeLeaf(itemType) && !TreeItemType.isObservationTypeLeaf(itemType) && !TreeItemType.isGNSSObservationTypeLeaf(itemType) && !TreeItemType.isVerticalDeflectionTypeLeaf(itemType) && !TreeItemType.isCongruenceAnalysisTypeLeaf(itemType))
			{
				return;
			}

			TreeItemType parentType = TreeItemType.getDirectoryByLeafType(itemType);

			if (parentType == null || !this.directoryItemMap.containsKey(parentType))
			{
				return;
			}

			if (this.remove(itemValue))
			{
				IgnoreEvent = true;
				CheckBoxTreeItem<TreeItemValue> parent = this.directoryItemMap.get(parentType);
				parent.getChildren().remove(treeItem);
				updateSelectionStageOfParentNode(parent);
				IgnoreEvent = false;
			}
		}

		public virtual void moveItems(TreeItemType newItemType, IList<TreeItem<TreeItemValue>> selectedItems)
		{
			TreeItemType newParentType = TreeItemType.getDirectoryByLeafType(newItemType);
			if (this.directoryItemMap.containsKey(newParentType) && selectedItems != null && selectedItems.Count > 0 && (TreeItemType.isPointTypeLeaf(newItemType) || TreeItemType.isVerticalDeflectionTypeLeaf(newItemType)))
			{
				CheckBoxTreeItem<TreeItemValue> newParent = this.directoryItemMap.get(newParentType);
				TreeItem<TreeItemValue> lastItem = null;
				this.IgnoreEvent = true;
				foreach (TreeItem<TreeItemValue> selectedItem in selectedItems)
				{
					if (selectedItem != null && selectedItem.isLeaf() && selectedItem.getValue() != null)
					{

						TreeItemValue itemValue;
						if (selectedItem.getValue() is PointTreeItemValue)
						{
							itemValue = (PointTreeItemValue)selectedItem.getValue();
						}
						else if (selectedItem.getValue() is VerticalDeflectionTreeItemValue)
						{
							itemValue = (VerticalDeflectionTreeItemValue)selectedItem.getValue();
						}
						else
						{
							continue;
						}

						TreeItemType oldItemType = itemValue.ItemType;
						TreeItemType oldParentType = TreeItemType.getDirectoryByLeafType(oldItemType);
						if (this.directoryItemMap.containsKey(oldParentType))
						{
							itemValue.ItemType = newItemType;
							if (!this.save(itemValue))
							{
								itemValue.ItemType = oldItemType;
								continue;
							}
							lastItem = selectedItem;
							CheckBoxTreeItem<TreeItemValue> oldParent = this.directoryItemMap.get(oldParentType);
							// Remove Item
							oldParent.getChildren().remove(selectedItem);
							updateSelectionStageOfParentNode(oldParent);
							// add Item
							newParent.getChildren().add(selectedItem);
							//updateSelectionStageOfParentNode(newParent);
							expand(selectedItem, true);
						}
					}
				}

				if (lastItem != null)
				{
					TreeItem<TreeItemValue> lastSelectedItem = lastItem;
					updateSelectionStageOfParentNode(newParent);
					IgnoreEvent = false;
					this.treeView.getSelectionModel().select(lastSelectedItem);
				}
			}
		}


		public virtual void addEmptyGroup(TreeItemType parentType)
		{
			if (this.directoryItemMap.containsKey(parentType))
			{
				TreeItem<TreeItemValue> newMenuItem = this.addItem(parentType);
				if (!this.save(newMenuItem.getValue()))
				{
					TreeItem<TreeItemValue> parentItem = this.directoryItemMap.get(parentType);
					parentItem.getChildren().remove(newMenuItem);
				}
			}
		}

		private void updateSelectionStageOfParentNode(CheckBoxTreeItem<TreeItemValue> parentNode)
		{
			if (parentNode.isLeaf())
			{
				parentNode.setIndeterminate(false);
				parentNode.setSelected(false);
			}
			else if (parentNode.getChildren().get(0) is CheckBoxTreeItem && parentNode.getChildren().get(0).isLeaf())
			{
				CheckBoxTreeItem<TreeItemValue> firstChild = (CheckBoxTreeItem<TreeItemValue>)parentNode.getChildren().get(0);
				firstChild.setSelected(!firstChild.isSelected());
				firstChild.setSelected(!firstChild.isSelected());
			}
		}

		private void expand(TreeItem<TreeItemValue> item, bool expand)
		{
			try
			{
				this.ignoreExpanding = true;
				if (item != null)
				{
					if (!item.isExpanded())
					{
						item.setExpanded(expand);
					}
					this.expand(item.getParent(), expand);
				}
			}
			finally
			{
				this.ignoreExpanding = false;
			}
		}

		public virtual void handleTreeSelections()
		{
			this.handleTreeSelections(this.lastValidSelectedTreeItem);
		}

		private void handleTreeSelections(TreeItem<TreeItemValue> currentTreeItem)
		{
			// Save last option
			this.lastValidSelectedTreeItem = currentTreeItem;

			if (currentTreeItem == null)
			{
				return;
			}

			MultipleSelectionModel<TreeItem<TreeItemValue>> selectionModel = this.treeView.getSelectionModel();
			try
			{
	//			selectionModel.selectedItemProperty().removeListener(this.treeSelectionChangeListener);
	//			selectionModel.getSelectedItems().removeListener(this.treeListSelectionChangeListener);
				this.ignoreTreeSelection = true;

				TreeItemType currentItemType = currentTreeItem.getValue().getItemType();
				bool isValidSelection = true;

				ObservableList<TreeItem<TreeItemValue>> treeItems = selectionModel.getSelectedItems();
				foreach (TreeItem<TreeItemValue> item in treeItems)
				{
					if (item == null || item.getValue() == null || item.getValue().getItemType() != currentItemType)
					{
						isValidSelection = false;
						break;
					}
				}

				if (!isValidSelection)
				{
					Platform.runLater(() =>
					{
					selectionModel.clearSelection();
					selectionModel.select(currentTreeItem);
					});
				}
				else if (currentTreeItem != null && currentTreeItem.getValue() != null)
				{
					TreeItemValue itemValue = currentTreeItem.getValue();
					if (!currentTreeItem.isLeaf())
					{
						switch (itemValue.ItemType.innerEnumValue)
						{
						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ROOT:
							this.load(itemValue, treeItems);
							break;

						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_1D_DIRECTORY:
						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_DIRECTORY:
						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_DIRECTORY:
						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_1D_DIRECTORY:

						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_2D_DIRECTORY:
						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_DIRECTORY:
						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_DIRECTORY:
						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_2D_DIRECTORY:

						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_3D_DIRECTORY:
						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_DIRECTORY:
						case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_DIRECTORY:
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

							this.selectChildren(currentTreeItem);
							break;

						default:
							Console.Error.WriteLine(this.GetType().Name + " : Error, unsupported TreeItemType (only directories) " + itemValue.ItemType);
							this.tabPaneBuilder.TreeItemValue = itemValue;
							break;
						}
					}
					else if (currentTreeItem.isLeaf())
					{
						this.load(itemValue, treeItems);
					}
				}
			}
			finally
			{
	//			selectionModel.selectedItemProperty().addListener(this.treeSelectionChangeListener);
	//			selectionModel.getSelectedItems().addListener(this.treeListSelectionChangeListener);
				this.ignoreTreeSelection = false;
			}
		}

		private bool save(TreeItemValue treeItemValue)
		{
			try
			{
				TreeItemType type = treeItemValue.ItemType;
				if (TreeItemType.isPointTypeLeaf(type) && treeItemValue is PointTreeItemValue)
				{
					SQLManager.Instance.saveGroup((PointTreeItemValue)treeItemValue);
				}

				else if ((TreeItemType.isGNSSObservationTypeLeaf(type) || TreeItemType.isObservationTypeLeaf(type)) && treeItemValue is ObservationTreeItemValue)
				{
					SQLManager.Instance.saveGroup((ObservationTreeItemValue)treeItemValue);
				}

				else if ((TreeItemType.isCongruenceAnalysisTypeLeaf(type)) && treeItemValue is CongruenceAnalysisTreeItemValue)
				{
					SQLManager.Instance.saveGroup((CongruenceAnalysisTreeItemValue)treeItemValue);
				}

				else if ((TreeItemType.isVerticalDeflectionTypeLeaf(type)) && treeItemValue is VerticalDeflectionTreeItemValue)
				{
					SQLManager.Instance.saveGroup((VerticalDeflectionTreeItemValue)treeItemValue);
				}

				else
				{
					Console.Error.WriteLine(this.GetType().Name + " : Error, item has no saveable properties " + treeItemValue);
					return false;
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UITreeBuiler.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("UITreeBuiler.message.error.save.exception.header", "Error, could save group properties to database."), i18n.getString("UITreeBuiler.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
				return false;
			}
			return true;
		}

		private bool remove(TreeItemValue treeItemValue)
		{
			try
			{
				TreeItemType type = treeItemValue.ItemType;
				if (TreeItemType.isPointTypeLeaf(type) || TreeItemType.isGNSSObservationTypeLeaf(type) || TreeItemType.isObservationTypeLeaf(type) || TreeItemType.isVerticalDeflectionTypeLeaf(type) || TreeItemType.isCongruenceAnalysisTypeLeaf(type))
				{
					SQLManager.Instance.removeGroup(treeItemValue);
					return true;
				}
				else
				{
					Console.Error.WriteLine(this.GetType().Name + " : Error, item has no removeable properties " + treeItemValue);
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UITreeBuiler.message.error.remove.exception.title", "Unexpected SQL-Error"), i18n.getString("UITreeBuiler.message.error.remove.exception.header", "Error, could remove group from database."), i18n.getString("UITreeBuiler.message.error.remove.exception.message", "An exception has occurred during database transaction."), e);

			}
			return false;
		}

		private void load(TreeItemValue itemValue, ObservableList<TreeItem<TreeItemValue>> treeItems)
		{
			try
			{
				TreeItemValue[] itemValues = new TreeItemValue[treeItems == null ? 0 : treeItems.size()];
				if (treeItems != null)
				{
					for (int i = 0; i < treeItems.size(); i++)
					{
						itemValues[i] = treeItems.get(i).getValue();
					}
				}
				SQLManager.Instance.loadData(itemValue, itemValues);
				this.tabPaneBuilder.TreeItemValue = itemValue;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UITreeBuiler.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("UITreeBuiler.message.error.load.exception.header", "Error, could load group properties from database."), i18n.getString("UITreeBuiler.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
			}
		}

		private void selectChildren(TreeItem<TreeItemValue> parent)
		{
			try
			{
	//			this.treeView.getSelectionModel().selectedItemProperty().removeListener(this.treeSelectionChangeListener);
	//			this.treeView.getSelectionModel().getSelectedItems().removeListener(this.treeListSelectionChangeListener);
				this.ignoreTreeSelection = true;
				if (!parent.isLeaf() && parent.isExpanded())
				{
					this.treeView.getSelectionModel().clearSelection();
					ObservableList<TreeItem<TreeItemValue>> children = parent.getChildren();
					TreeItem<TreeItemValue> lastSelectedChild = null;
					foreach (TreeItem<TreeItemValue> child in children)
					{
						this.treeView.getSelectionModel().select(child);
						lastSelectedChild = child;
					}
					if (lastSelectedChild != null)
					{
						this.load(lastSelectedChild.getValue(), parent.getChildren());
					}
				}
				else if (!parent.isLeaf() && !parent.isExpanded())
				{
					this.load(parent.getValue(), null);
				}
			}
			finally
			{
	//			this.treeView.getSelectionModel().selectedItemProperty().addListener(this.treeSelectionChangeListener);
	//			this.treeView.getSelectionModel().getSelectedItems().addListener(this.treeListSelectionChangeListener);
				this.ignoreTreeSelection = false;
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


		public virtual TreeItem<TreeItemValue> getDirectoryItemByType(TreeItemType itemType)
		{
			return this.directoryItemMap.get(itemType);
		}

		internal virtual void removeSelectedGroups()
		{
			IList<TreeItem<TreeItemValue>> selectedItems = this.treeView.getSelectionModel().getSelectedItems();
			if (selectedItems == null || selectedItems.Count == 0)
			{
				return;
			}

			bool isDeleteConfirmed = !DefaultApplicationProperty.showConfirmDialogOnDelete();

			if (!isDeleteConfirmed)
			{
				Optional<ButtonType> result = OptionDialog.showConfirmationDialog(i18n.getString("UITreeBuiler.message.confirmation.delete.title", "Delete groups"), i18n.getString("UITreeBuiler.message.confirmation.delete.header", "Delete groups permanently?"), i18n.getString("UITreeBuiler.message.confirmation.delete.message", "Are you sure you want to remove the selected groups?"));
				isDeleteConfirmed = result.isPresent() && result.get() == ButtonType.OK;
			}

			if (isDeleteConfirmed)
			{
				removeItems(new List<TreeItem<TreeItemValue>>(selectedItems));
			}
		}

		private void addTreeKeyEvents()
		{
			this.treeView.addEventFilter(KeyEvent.KEY_PRESSED, new EventHandlerAnonymousInnerClass(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<KeyEvent>
		{
			private readonly UITreeBuilder outerInstance;

			public EventHandlerAnonymousInnerClass(UITreeBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void handle(KeyEvent keyEvent)
			{
				if (keyEvent.getSource() == outerInstance.treeView && keyEvent.getTarget() == outerInstance.treeView && keyEvent.getCode() == KeyCode.DELETE)
				{
					outerInstance.removeSelectedGroups();
					keyEvent.consume();
				}
			}
		}
	}

}