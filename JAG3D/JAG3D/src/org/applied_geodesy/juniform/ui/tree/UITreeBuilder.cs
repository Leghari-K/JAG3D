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

namespace org.applied_geodesy.juniform.ui.tree
{

	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using FeatureAdjustment = org.applied_geodesy.adjustment.geometry.FeatureAdjustment;
	using FeatureChangeListener = org.applied_geodesy.adjustment.geometry.FeatureChangeListener;
	using FeatureEvent = org.applied_geodesy.adjustment.geometry.FeatureEvent;
	using FeatureType = org.applied_geodesy.adjustment.geometry.FeatureType;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using PrimitiveType = org.applied_geodesy.adjustment.geometry.PrimitiveType;
	using FeatureEventType = org.applied_geodesy.adjustment.geometry.FeatureEvent.FeatureEventType;
	using Curve = org.applied_geodesy.adjustment.geometry.curve.primitive.Curve;
	using Surface = org.applied_geodesy.adjustment.geometry.surface.primitive.Surface;
	using GeometricPrimitiveDialog = org.applied_geodesy.juniform.ui.dialog.GeometricPrimitiveDialog;
	using UITabPaneBuilder = org.applied_geodesy.juniform.ui.tabpane.UITabPaneBuilder;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using ListChangeListener = javafx.collections.ListChangeListener;
	using TreeCell = javafx.scene.control.TreeCell;
	using TreeItem = javafx.scene.control.TreeItem;
	using TreeView = javafx.scene.control.TreeView;
	using Callback = javafx.util.Callback;

	public class UITreeBuilder : FeatureChangeListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			geometricPrimitiveListChangeListener = new GeometricPrimitiveListChangeListener(this);
			treeListSelectionChangeListener = new TreeListSelectionChangeListener(this);
		}

		private class TreeListSelectionChangeListener : ListChangeListener<TreeItem<TreeItemValue<JavaToDotNetGenericWildcard>>>
		{
			private readonly UITreeBuilder outerInstance;

			public TreeListSelectionChangeListener(UITreeBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void onChanged<T1, T2>(Change<T1, T2> change) where T1 : javafx.scene.control.TreeItem<TreeItemValue<T1>>
			{
				if (change != null && change.next() && outerInstance.treeView != null && outerInstance.treeView.getSelectionModel() != null && outerInstance.treeView.getSelectionModel().getSelectedItems().size() > 0)
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> treeItem = null;
					TreeItem<TreeItemValue<object>> treeItem = null;
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

		private class GeometricPrimitiveListChangeListener : ListChangeListener<GeometricPrimitive>
		{
			private readonly UITreeBuilder outerInstance;

			public GeometricPrimitiveListChangeListener(UITreeBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void onChanged<T1>(Change<T1> change) where T1 : org.applied_geodesy.adjustment.geometry.GeometricPrimitive
			{
				while (change.next())
				{
					if (change.wasAdded())
					{
						foreach (GeometricPrimitive geometry in change.getAddedSubList())
						{
							outerInstance.addItem(geometry);
						}
					}
					else if (change.wasRemoved())
					{
						foreach (GeometricPrimitive geometry in change.getRemoved())
						{
							outerInstance.removeItem(geometry);
						}
					}
				}
			}
		}

		private readonly I18N i18n = I18N.Instance;
		private static UITreeBuilder treeBuilder = new UITreeBuilder();
		private UITabPaneBuilder tabPaneBuilder = UITabPaneBuilder.Instance;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeView<TreeItemValue<?>> treeView;
		private TreeView<TreeItemValue<object>> treeView;
		private GeometricPrimitiveListChangeListener geometricPrimitiveListChangeListener;
		private TreeListSelectionChangeListener treeListSelectionChangeListener;
		private FeatureType featureType = FeatureType.SURFACE;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<TreeItemValue<?>> lastValidSelectedTreeItem = null;
		private TreeItem<TreeItemValue<object>> lastValidSelectedTreeItem = null;

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
				treeBuilder.init();
				return treeBuilder;
			}
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: public javafx.scene.control.TreeView<TreeItemValue<?>> getTree()
		public virtual TreeView<TreeItemValue<object>> Tree
		{
			get
			{
				return this.treeView;
			}
		}

		private void init()
		{
			if (this.treeView != null)
			{
				return;
			}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: this.treeView = new javafx.scene.control.TreeView<TreeItemValue<?>>();
			this.treeView = new TreeView<TreeItemValue<object>>();
			this.treeView.setEditable(true);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: this.treeView.setCellFactory(new javafx.util.Callback<javafx.scene.control.TreeView<TreeItemValue<?>>, javafx.scene.control.TreeCell<TreeItemValue<?>>>()
			this.treeView.setCellFactory(new CallbackAnonymousInnerClass(this));

			AdjustmentTreeItemValue adjustmentTreeItemValue = new AdjustmentTreeItemValue(i18n.getString("UITreeBuilder.root", "JUniForm"));
			FeatureTreeItemValue featureTreeItemValue = new FeatureTreeItemValue(i18n.getString("UITreeBuilder.feature.label", "Feature"));

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> rootItem = this.createItem(adjustmentTreeItemValue);
			TreeItem<TreeItemValue<object>> rootItem = this.createItem(adjustmentTreeItemValue);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> featureItem = this.createItem(featureTreeItemValue);
			TreeItem<TreeItemValue<object>> featureItem = this.createItem(featureTreeItemValue);

			rootItem.getChildren().addAll(Arrays.asList(featureItem));
			this.treeView.setRoot(rootItem);

			this.treeView.setShowRoot(false);

			this.treeView.getSelectionModel().getSelectedItems().addListener(this.treeListSelectionChangeListener);
			this.treeView.getSelectionModel().select(0);
		}

		private class CallbackAnonymousInnerClass : Callback<TreeView<TreeItemValue<JavaToDotNetGenericWildcard>>, TreeCell<TreeItemValue<JavaToDotNetGenericWildcard>>>
		{
			private readonly UITreeBuilder outerInstance;

			public CallbackAnonymousInnerClass(UITreeBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: @Override public javafx.scene.control.TreeCell<TreeItemValue<?>> call(javafx.scene.control.TreeView<TreeItemValue<?>> treeView)
			public override TreeCell<TreeItemValue<object>> call<T1>(TreeView<T1> treeView)
			{
				return new EditableMenuTreeCell();
			}
		}

		public virtual void handleTreeSelections()
		{
			this.handleTreeSelections(this.lastValidSelectedTreeItem);
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<TreeItemValue<?>> createItem(TreeItemValue<?> value)
		private TreeItem<TreeItemValue<object>> createItem<T1>(TreeItemValue<T1> value)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> item = new javafx.scene.control.TreeItem<TreeItemValue<?>>(value);
			TreeItem<TreeItemValue<object>> item = new TreeItem<TreeItemValue<object>>(value);
			item.setExpanded(true);
			return item;
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<TreeItemValue<?>> searchTreeItem(javafx.scene.control.TreeItem<TreeItemValue<?>> item, Object object)
		private TreeItem<TreeItemValue<object>> searchTreeItem<T1>(TreeItem<T1> item, object @object)
		{
			if (item == null)
			{
				return null;
			}

			if (item.getValue().getObject() == @object)
			{
				return item;
			}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> result = null;
			TreeItem<TreeItemValue<object>> result = null;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for(javafx.scene.control.TreeItem<TreeItemValue<?>> child : item.getChildren())
			foreach (TreeItem<TreeItemValue<object>> child in item.getChildren())
			{
				result = searchTreeItem(child, @object);
				if (result != null)
				{
					return result;
				}
			}

			return result;
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<TreeItemValue<?>> searchTreeItem(javafx.scene.control.TreeItem<TreeItemValue<?>> item, TreeItemType treeItemType)
		private TreeItem<TreeItemValue<object>> searchTreeItem<T1>(TreeItem<T1> item, TreeItemType treeItemType)
		{
			if (item == null)
			{
				return null;
			}

			if (item.getValue().getTreeItemType() == treeItemType)
			{
				return item;
			}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> result = null;
			TreeItem<TreeItemValue<object>> result = null;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for(javafx.scene.control.TreeItem<TreeItemValue<?>> child : item.getChildren())
			foreach (TreeItem<TreeItemValue<object>> child in item.getChildren())
			{
				result = searchTreeItem(child, treeItemType);
				if (result != null)
				{
					return result;
				}
			}

			return result;
		}

		public virtual GeometricPrimitive SelectedGeometry
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> item = this.treeView.getSelectionModel().getSelectedItem();
				TreeItem<TreeItemValue<object>> item = this.treeView.getSelectionModel().getSelectedItem();
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	//ORIGINAL LINE: TreeItemValue<?> itemValue = item.getValue();
				TreeItemValue<object> itemValue = item.getValue();
    
				if (itemValue is CurveTreeItemValue)
				{
					return ((CurveTreeItemValue)item.getValue()).Object;
				}
    
				else if (itemValue is SurfaceTreeItemValue)
				{
					return ((SurfaceTreeItemValue)item.getValue()).Object;
				}
    
				return null;
			}
		}

		public virtual FeatureAdjustment FeatureAdjustment
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	//ORIGINAL LINE: javafx.scene.control.TreeItem<?> rootItem = this.treeView.getRoot();
				TreeItem<object> rootItem = this.treeView.getRoot();
				if (rootItem != null)
				{
					return ((AdjustmentTreeItemValue)rootItem.getValue()).Object;
				}
    
				return null;
			}
		}

		public virtual FeatureType FeatureType
		{
			set
			{
				this.Feature = null;
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> featureItem = this.searchTreeItem(this.treeView.getRoot(), TreeItemType.FEATURE);
				TreeItem<TreeItemValue<object>> featureItem = this.searchTreeItem(this.treeView.getRoot(), TreeItemType.FEATURE);
				if (featureItem != null)
				{
					featureItem.getValue().setName(i18n.getString("UITreeBuilder.feature.label", "Feature"));
				}
    
				this.FeatureAdjustment.Feature = null;
				this.featureType = value;
			}
		}

		private Feature Feature
		{
			set
			{
				this.treeView.getSelectionModel().select(0);
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> featureItem = null;
				TreeItem<TreeItemValue<object>> featureItem = null;
				featureItem = this.searchTreeItem(this.treeView.getRoot(), TreeItemType.FEATURE);
    
				if (featureItem != null)
				{
					if (value != null)
					{
						this.featureType = value.FeatureType;
					}
    
					// change name of value tree item
					featureItem.getValue().setName(this.featureType == FeatureType.CURVE ? i18n.getString("UITreeBuilder.feature.curves", "Curves") : i18n.getString("UITreeBuilder.feature.surfaces", "Surfaces"));
    
					// cleaning tree
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	//ORIGINAL LINE: for (javafx.scene.control.TreeItem<TreeItemValue<?>> primitives : featureItem.getChildren())
					foreach (TreeItem<TreeItemValue<object>> primitives in featureItem.getChildren())
					{
						primitives.getChildren().clear();
					}
    
					featureItem.getChildren().clear();
    
					// adding geometric primitives of value
					if (value != null)
					{
						foreach (GeometricPrimitive geometricPrimitive in value.GeometricPrimitives)
						{
							this.addItem(geometricPrimitive);
						}
					}
					else if (value == null && this.FeatureAdjustment.Feature != null)
					{
						this.FeatureAdjustment.Feature.GeometricPrimitives.removeListener(this.geometricPrimitiveListChangeListener);
					}
    
					this.treeView.getSelectionModel().select(featureItem);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<TreeItemValue<?>> removeItem(org.applied_geodesy.adjustment.geometry.GeometricPrimitive geometry)
		private TreeItem<TreeItemValue<object>> removeItem(GeometricPrimitive geometry)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> treeItem = this.searchTreeItem(this.treeView.getRoot(), geometry);
			TreeItem<TreeItemValue<object>> treeItem = this.searchTreeItem(this.treeView.getRoot(), geometry);
			if (treeItem == null)
			{
				return null;
			}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> parent = treeItem.getParent();
			TreeItem<TreeItemValue<object>> parent = treeItem.getParent();
			parent.getChildren().remove(treeItem);
			if (parent.getChildren().size() == 0)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> featureItem = parent.getParent();
				TreeItem<TreeItemValue<object>> featureItem = parent.getParent();
				featureItem.getChildren().remove(parent);
			}
			return treeItem;
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.TreeItem<TreeItemValue<?>> addItem(org.applied_geodesy.adjustment.geometry.GeometricPrimitive geometry)
		private TreeItem<TreeItemValue<object>> addItem(GeometricPrimitive geometry)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> featureItem = this.searchTreeItem(this.treeView.getRoot(), TreeItemType.FEATURE);
			TreeItem<TreeItemValue<object>> featureItem = this.searchTreeItem(this.treeView.getRoot(), TreeItemType.FEATURE);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> newItem = null;
			TreeItem<TreeItemValue<object>> newItem = null;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeItem<TreeItemValue<?>> parent = null;
			TreeItem<TreeItemValue<object>> parent = null;

			if (featureItem == null)
			{
				return null;
			}

			if (geometry != null)
			{
				string geometryName = geometry.Name;
				if (string.ReferenceEquals(geometryName, null) || geometryName.Trim().Length == 0)
				{
					GeometricPrimitiveDialog.DefaultName = geometry;
				}

				TreeItemType treeItemType = null;
				string parentLabel = null;

				if (this.featureType == FeatureType.CURVE)
				{
					if (geometry.PrimitiveType == PrimitiveType.LINE)
					{
						treeItemType = TreeItemType.LINE;
						parentLabel = i18n.getString("UITreeBuilder.curves.lines", "Lines");

					}
					else if (geometry.PrimitiveType == PrimitiveType.CIRCLE)
					{
						treeItemType = TreeItemType.CIRCLE;
						parentLabel = i18n.getString("UITreeBuilder.curves.circles", "Circles");
					}

					else if (geometry.PrimitiveType == PrimitiveType.ELLIPSE)
					{
						treeItemType = TreeItemType.ELLIPSE;
						parentLabel = i18n.getString("UITreeBuilder.curves.ellipses", "Ellipses");
					}

					else if (geometry.PrimitiveType == PrimitiveType.QUADRATIC_CURVE)
					{
						treeItemType = TreeItemType.QUADRATIC_CURVE;
						parentLabel = i18n.getString("UITreeBuilder.curves.quadrics", "Quadratic curves");
					}

					else
					{
						throw new System.ArgumentException("Error, unknown primitive type " + geometry.PrimitiveType + "!");
					}


					parent = this.searchTreeItem(featureItem, treeItemType);
					if (parent == null)
					{
						parent = this.createItem(new GeometricPrimitivesTreeItemValue(parentLabel, treeItemType));
						featureItem.getChildren().addAll(Arrays.asList(parent));
					}

					CurveTreeItemValue itemValue = new CurveTreeItemValue(geometry.Name, treeItemType);
					itemValue.Object = (Curve)geometry;
					newItem = this.createItem(itemValue);

				}
				else if (this.featureType == FeatureType.SURFACE)
				{
					if (geometry.PrimitiveType == PrimitiveType.PLANE)
					{
						treeItemType = TreeItemType.PLANE;
						parentLabel = i18n.getString("UITreeBuilder.surfaces.planes", "Planes");
					}

					else if (geometry.PrimitiveType == PrimitiveType.SPHERE)
					{
						treeItemType = TreeItemType.SPHERE;
						parentLabel = i18n.getString("UITreeBuilder.surfaces.spheres", "Spheres");
					}

					else if (geometry.PrimitiveType == PrimitiveType.ELLIPSOID)
					{
						treeItemType = TreeItemType.ELLIPSOID;
						parentLabel = i18n.getString("UITreeBuilder.surfaces.ellipsoids", "Ellipsoids");
					}

					else if (geometry.PrimitiveType == PrimitiveType.CYLINDER)
					{
						treeItemType = TreeItemType.CYLINDER;
						parentLabel = i18n.getString("UITreeBuilder.surfaces.cylinders", "Cylinders");
					}

					else if (geometry.PrimitiveType == PrimitiveType.CONE)
					{
						treeItemType = TreeItemType.CONE;
						parentLabel = i18n.getString("UITreeBuilder.surfaces.cones", "Cones");
					}

					else if (geometry.PrimitiveType == PrimitiveType.PARABOLOID)
					{
						treeItemType = TreeItemType.PARABOLOID;
						parentLabel = i18n.getString("UITreeBuilder.surfaces.paraboloids", "Paraboloids");
					}

					else if (geometry.PrimitiveType == PrimitiveType.QUADRATIC_SURFACE)
					{
						treeItemType = TreeItemType.QUADRATIC_SURFACE;
						parentLabel = i18n.getString("UITreeBuilder.surfaces.quadrics", "Quadratic surfaces");
					}

					else
					{
						throw new System.ArgumentException("Error, unknown primitive type " + geometry.PrimitiveType + "!");
					}

					parent = this.searchTreeItem(featureItem, treeItemType);
					if (parent == null)
					{
						parent = this.createItem(new GeometricPrimitivesTreeItemValue(parentLabel, treeItemType));
						featureItem.getChildren().addAll(Arrays.asList(parent));
					}

					SurfaceTreeItemValue itemValue = new SurfaceTreeItemValue(geometry.Name, treeItemType);
					itemValue.Object = (Surface)geometry;
					newItem = this.createItem(itemValue);
				}
				else
				{
					newItem = null;
				}
			}

			if (parent != null && newItem != null && parent.getChildren().add(newItem))
			{
				geometry.nameProperty().bindBidirectional(newItem.getValue().nameProperty());
				parent.setExpanded(true);
			}
			return newItem;
		}

		private void handleTreeSelections<T1>(TreeItem<T1> currentTreeItem)
		{
			// Save last option
			this.lastValidSelectedTreeItem = currentTreeItem;

			if (currentTreeItem == null)
			{
				return;
			}

			this.tabPaneBuilder.TreeItemValue = currentTreeItem.getValue();

		}

		public virtual void featureChanged(FeatureEvent evt)
		{
			if (evt.EventType == FeatureEvent.FeatureEventType.FEATURE_ADDED)
			{
				this.Feature = evt.Source;
				// add listener to handle new feature
				evt.Source.GeometricPrimitives.addListener(this.geometricPrimitiveListChangeListener);
			}
			else if (evt.EventType == FeatureEvent.FeatureEventType.FEATURE_REMOVED)
			{
				// remove listener from old feature
				evt.Source.GeometricPrimitives.removeListener(this.geometricPrimitiveListChangeListener);
			}
		}
	}

}