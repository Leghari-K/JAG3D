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

namespace org.applied_geodesy.juniform.ui.menu
{

	using FeatureChangeListener = org.applied_geodesy.adjustment.geometry.FeatureChangeListener;
	using FeatureEvent = org.applied_geodesy.adjustment.geometry.FeatureEvent;
	using FeatureType = org.applied_geodesy.adjustment.geometry.FeatureType;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using FeatureEventType = org.applied_geodesy.adjustment.geometry.FeatureEvent.FeatureEventType;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using FeaturePointFileReader = org.applied_geodesy.juniform.io.FeaturePointFileReader;
	using InitialGuessFileReader = org.applied_geodesy.juniform.io.InitialGuessFileReader;
	using FTLReport = org.applied_geodesy.juniform.io.report.FTLReport;
	using JUniForm = org.applied_geodesy.juniform.ui.JUniForm;
	using org.applied_geodesy.juniform.ui.dialog;
	using UIPointTableBuilder = org.applied_geodesy.juniform.ui.table.UIPointTableBuilder;
	using UITreeBuilder = org.applied_geodesy.juniform.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DefaultFileChooser = org.applied_geodesy.ui.io.DefaultFileChooser;
	using org.applied_geodesy.util;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using ListChangeListener = javafx.collections.ListChangeListener;
	using Menu = javafx.scene.control.Menu;
	using MenuBar = javafx.scene.control.MenuBar;
	using MenuItem = javafx.scene.control.MenuItem;
	using RadioMenuItem = javafx.scene.control.RadioMenuItem;
	using SeparatorMenuItem = javafx.scene.control.SeparatorMenuItem;
	using Toggle = javafx.scene.control.Toggle;
	using ToggleGroup = javafx.scene.control.ToggleGroup;
	using KeyCode = javafx.scene.input.KeyCode;
	using KeyCodeCombination = javafx.scene.input.KeyCodeCombination;
	using KeyCombination = javafx.scene.input.KeyCombination;
	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;

	public class UIMenuBuilder : FeatureChangeListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			menuEventHandler = new MenuEventHandler(this);
			geometricPrimitiveListChangeListener = new GeometricPrimitiveListChangeListener(this);
		}

		private class GeometricPrimitiveListChangeListener : ListChangeListener<GeometricPrimitive>
		{
			private readonly UIMenuBuilder outerInstance;

			public GeometricPrimitiveListChangeListener(UIMenuBuilder outerInstance)
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
							outerInstance.addGeometricPrimitiveItem(geometry);
						}
					}
					else if (change.wasRemoved())
					{
						foreach (GeometricPrimitive geometry in change.getRemoved())
						{
							outerInstance.removeGeometricPrimitiveItem(geometry);
						}
					}
				}
			}
		}
		internal enum DisableStateType
		{
			CURVE_TYPE,
			SURFACE_TYPE,
			FEATURE
		}
		private static UIMenuBuilder menuBuilder = new UIMenuBuilder();
		private I18N i18n = I18N.Instance;
		private MenuEventHandler menuEventHandler;
		private MenuBar menuBar;
		private ToggleGroup featureToggleGroup;
		private GeometricPrimitiveListChangeListener geometricPrimitiveListChangeListener;
		private UIMenuBuilder()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}
		private Menu initialGuessMenu, reportMenu;

		public static UIMenuBuilder Instance
		{
			get
			{
				return menuBuilder;
			}
		}

		public virtual MenuBar MenuBar
		{
			get
			{
				if (this.menuBar == null)
				{
					this.init();
				}
				return this.menuBar;
			}
		}

		private void init()
		{
			this.featureToggleGroup = new ToggleGroup();

			this.menuBar = new MenuBar();
			Menu fileMenu = createMenu(i18n.getString("UIMenuBuilder.menu.file.label", "_File"), true);
			Menu curveMenu = createMenu(i18n.getString("UIMenuBuilder.menu.feature.curve.label", "_Curves"), true);
			Menu surfaceMenu = createMenu(i18n.getString("UIMenuBuilder.menu.feature.surface.label", "_Surfaces"), true);
			Menu propertiesMenu = createMenu(i18n.getString("UIMenuBuilder.menu.properties.label", "Propert_ies"), true);
			Menu adjustmentMenu = createMenu(i18n.getString("UIMenuBuilder.menu.adjustment.label", "Ad_justment"), true);
			this.reportMenu = createMenu(i18n.getString("UIMenuBuilder.menu.report.label", "Repor_t"), true);
			Menu helpMenu = createMenu(i18n.getString("UIMenuBuilder.menu.help.label", "_?"), true);

			this.createFileMenu(fileMenu);
			this.createCurveFeatureMenu(curveMenu, this.featureToggleGroup);
			this.createSurfaceFeatureMenu(surfaceMenu, this.featureToggleGroup);
			this.createPropertiesMenu(propertiesMenu);
			this.createAdjustmentMenu(adjustmentMenu);
			this.createReportMenu(this.reportMenu);
			this.createHelpMenu(helpMenu);

			this.menuBar.getMenus().addAll(fileMenu, curveMenu, surfaceMenu, propertiesMenu, adjustmentMenu, reportMenu, helpMenu);
		}

		private void createFileMenu(Menu parentMenu)
		{
			MenuItem importCurvePointsItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.file.import.points.curve.label", "Import c_urve points"), true, MenuItemType.IMPORT_CURVE_POINTS, new KeyCodeCombination(KeyCode.U, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, false);
			MenuItem importSurfacePointsItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.file.import.points.surface.label", "Imp_ort surface points"), true, MenuItemType.IMPORT_SURFACE_POINTS, new KeyCodeCombination(KeyCode.O, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, false);
			MenuItem exitItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.file.exit.label", "_Exit"), true, MenuItemType.EXIT, new KeyCodeCombination(KeyCode.E, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, false);

			this.initialGuessMenu = createMenu(i18n.getString("UIMenuBuilder.menu.file.import.initial.label", "Initial _guess"), true);
			this.initialGuessMenu.setDisable(true);

			parentMenu.getItems().addAll(importCurvePointsItem, importSurfacePointsItem, new SeparatorMenuItem(), this.initialGuessMenu, new SeparatorMenuItem(), exitItem);
		}

		private void createAdjustmentMenu(Menu parentMenu)
		{
			MenuItem leastSquaresItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.adjustment.leastsquares.label", "_Least-squares"), true, MenuItemType.LEAST_SQUARES, new KeyCodeCombination(KeyCode.L, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem teststatisticItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.adjustment.teststatistic.label", "Test st_atistic"), true, MenuItemType.TEST_STATISTIC, new KeyCodeCombination(KeyCode.T, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem preferencesItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.adjustment.preferences.label", "Preferen_ces"), true, MenuItemType.PREFERENCES, new KeyCodeCombination(KeyCode.ENTER, KeyCombination.ALT_DOWN), this.menuEventHandler, false);
			parentMenu.getItems().addAll(leastSquaresItem, teststatisticItem, new SeparatorMenuItem(), preferencesItem);
		}

		private void createPropertiesMenu(Menu parentMenu)
		{

			MenuItem featurePropertiesItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.setting.feature.label", "_Feature"), true, MenuItemType.FEATURE_PROPERTIES, new KeyCodeCombination(KeyCode.F, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem paramameterPropertiesItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.setting.parameter.label", "P_arameter"), true, MenuItemType.PARAMETER_PROPERTIES, new KeyCodeCombination(KeyCode.P, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem restrictionPropertiesItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.setting.restriction.label", "_Restriction"), true, MenuItemType.RESTRICTION_PROPERTIES, new KeyCodeCombination(KeyCode.R, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem postprocessingPropertiesItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.setting.postprocessing.label", "_Post processing"), true, MenuItemType.POSTPROCESSING_PROPERTIES, new KeyCodeCombination(KeyCode.G, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);

			parentMenu.getItems().addAll(featurePropertiesItem, paramameterPropertiesItem, restrictionPropertiesItem, postprocessingPropertiesItem);
		}

		private void createCurveFeatureMenu(Menu parentMenu, ToggleGroup toggleGroup)
		{
			// Curves
			RadioMenuItem lineItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.curve.line.label", "Line"), true, MenuItemType.LINE, null, this.menuEventHandler, true);
			RadioMenuItem circleItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.curve.circle.label", "Circle"), true, MenuItemType.CIRCLE, null, this.menuEventHandler, true);
			RadioMenuItem ellipseItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.curve.ellipse.label", "Ellipse"), true, MenuItemType.ELLIPSE, null, this.menuEventHandler, true);

			RadioMenuItem quadraticCurveItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.curve.quadric.label", "Quadratic curve"), true, MenuItemType.QUADRATIC_CURVE, null, this.menuEventHandler, true);
			RadioMenuItem modifiedCurveItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.curve.userdefined.label", "User defined curve"), true, MenuItemType.MODIFIABLE_CURVE, null, this.menuEventHandler, true);

			toggleGroup.getToggles().addAll(lineItem, circleItem, ellipseItem, quadraticCurveItem, modifiedCurveItem);

			parentMenu.getItems().addAll(lineItem, circleItem, ellipseItem, quadraticCurveItem, new SeparatorMenuItem(), modifiedCurveItem);
		}

		private void createSurfaceFeatureMenu(Menu parentMenu, ToggleGroup toggleGroup)
		{
			// Surfaces
			RadioMenuItem planeItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.plane.label", "Plane"), true, MenuItemType.PLANE, null, this.menuEventHandler, true);
			RadioMenuItem sphereItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.sphere.label", "Sphere"), true, MenuItemType.SPHERE, null, this.menuEventHandler, true);
			RadioMenuItem ellipsoidItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.ellipsoid.label", "Ellipsoid"), true, MenuItemType.ELLIPSOID, null, this.menuEventHandler, true);

			RadioMenuItem spatialLineItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.line.label", "Spatial line"), true, MenuItemType.SPATIAL_LINE, null, this.menuEventHandler, true);
			RadioMenuItem spatialCircleItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.circle.label", "Spatial circle"), true, MenuItemType.SPATIAL_CIRCLE, null, this.menuEventHandler, true);
			RadioMenuItem spatialEllipseItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.ellipse.label", "Spatial ellipse"), true, MenuItemType.SPATIAL_ELLIPSE, null, this.menuEventHandler, true);

			RadioMenuItem cicularConeItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.cone.circular.label", "Circular cone"), true, MenuItemType.CIRCULAR_CONE, null, this.menuEventHandler, true);
			RadioMenuItem coneItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.cone.elliptic.label", "Cone"), true, MenuItemType.CONE, null, this.menuEventHandler, true);

			RadioMenuItem circularParaboloidItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.paraboloid.circular.label", "Circular paraboloid"), true, MenuItemType.CIRCULAR_PARABOLOID, null, this.menuEventHandler, true);
			RadioMenuItem paraboloidItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.paraboloid.elliptic.label", "Paraboloid"), true, MenuItemType.PARABOLOID, null, this.menuEventHandler, true);

			RadioMenuItem cicularCylinderItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.cylinder.circular.label", "Circular cylinder"), true, MenuItemType.CIRCULAR_CYLINDER, null, this.menuEventHandler, true);
			RadioMenuItem cylinderItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.cylinder.elliptic.label", "Cylinder"), true, MenuItemType.CYLINDER, null, this.menuEventHandler, true);

			RadioMenuItem quadraticSurfaceItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.quadric.label", "Quadratic surface"), true, MenuItemType.QUADRATIC_SURFACE, null, this.menuEventHandler, true);

			RadioMenuItem modifiedSurfaceItem = createRadioMenuItem(i18n.getString("UIMenuBuilder.menu.feature.surface.userdefined.label", "User defined surface"), true, MenuItemType.MODIFIABLE_SURFACE, null, this.menuEventHandler, true);

			toggleGroup.getToggles().addAll(planeItem, sphereItem, ellipsoidItem, spatialLineItem, spatialCircleItem, spatialEllipseItem, cicularCylinderItem, cylinderItem, cicularConeItem, coneItem, circularParaboloidItem, paraboloidItem, quadraticSurfaceItem, modifiedSurfaceItem);

			parentMenu.getItems().addAll(spatialLineItem, spatialCircleItem, spatialEllipseItem, new SeparatorMenuItem(), planeItem, sphereItem, ellipsoidItem, cicularCylinderItem, cylinderItem, cicularConeItem, coneItem, circularParaboloidItem, paraboloidItem, quadraticSurfaceItem, new SeparatorMenuItem(), modifiedSurfaceItem);
		}

		private void createReportMenu(Menu parentMenu)
		{
			IList<File> templateFiles = FTLReport.Templates;
			if (templateFiles != null && templateFiles.Count > 0)
			{
				foreach (File templateFile in templateFiles)
				{
					MenuItem templateFileItem = createMenuItem(templateFile.getName(), false, MenuItemType.REPORT, templateFile, null, this.menuEventHandler, true);

					parentMenu.getItems().add(templateFileItem);
				}
			}
		}

		private void createHelpMenu(Menu parentMenu)
		{
			MenuItem aboutItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.help.about.label", "A_bout JAG3D"), true, MenuItemType.ABOUT, new KeyCodeCombination(KeyCode.B, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, false);
			parentMenu.getItems().addAll(aboutItem);
		}

		internal virtual FeatureType FeatureType
		{
			set
			{
				DisableStateType disableStateType = null;
				if (value == FeatureType.CURVE)
				{
					disableStateType = DisableStateType.CURVE_TYPE;
				}
				else if (value == FeatureType.SURFACE)
				{
					disableStateType = DisableStateType.SURFACE_TYPE;
				}
				else
				{
					return;
				}
    
				this.disableMenu(disableStateType);
    
				foreach (Toggle toggle in this.featureToggleGroup.getToggles())
				{
					toggle.setSelected(false);
				}
			}
		}

		internal virtual void disableMenu(DisableStateType disableStateType)
		{
			IList<Menu> menus = this.menuBar.getMenus();
			foreach (Menu menu in menus)
			{
				this.disableMenu(menu, disableStateType);
			}
		}

		private void disableMenu(Menu menu, DisableStateType disableStateType)
		{
			IList<MenuItem> items = menu.getItems();
			foreach (MenuItem item in items)
			{
				if (item is Menu)
				{
					this.disableMenu((Menu)item, disableStateType);
				}
				else if (item.getUserData() != null && item.getUserData() is MenuItemType)
				{
					MenuItemType itemType = (MenuItemType)item.getUserData();
					switch (itemType)
					{
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.FEATURE_PROPERTIES:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.PARAMETER_PROPERTIES:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.POSTPROCESSING_PROPERTIES:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.RESTRICTION_PROPERTIES:
						if (disableStateType == DisableStateType.FEATURE)
						{
							item.setDisable(false);
						}
						break;

					case org.applied_geodesy.juniform.ui.menu.MenuItemType.TEST_STATISTIC:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.LEAST_SQUARES:
						item.setDisable(disableStateType == DisableStateType.CURVE_TYPE || disableStateType == DisableStateType.SURFACE_TYPE);
						break;

					case org.applied_geodesy.juniform.ui.menu.MenuItemType.LINE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.CIRCLE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.ELLIPSE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.QUADRATIC_CURVE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.MODIFIABLE_CURVE:
						if (disableStateType == DisableStateType.CURVE_TYPE || disableStateType == DisableStateType.SURFACE_TYPE)
						{
							item.setDisable(disableStateType != DisableStateType.CURVE_TYPE);
						}
						break;
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.PLANE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.SPHERE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.ELLIPSOID:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.SPATIAL_LINE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.SPATIAL_CIRCLE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.SPATIAL_ELLIPSE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.CIRCULAR_CYLINDER:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.CYLINDER:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.CIRCULAR_CONE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.CONE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.CIRCULAR_PARABOLOID:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.PARABOLOID:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.QUADRATIC_SURFACE:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.MODIFIABLE_SURFACE:
						if (disableStateType == DisableStateType.CURVE_TYPE || disableStateType == DisableStateType.SURFACE_TYPE)
						{
							item.setDisable(disableStateType != DisableStateType.SURFACE_TYPE);
						}
						break;

					case org.applied_geodesy.juniform.ui.menu.MenuItemType.REPORT:
						this.ReportMenuDisable = true;
						break;

					case org.applied_geodesy.juniform.ui.menu.MenuItemType.EXIT:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.ABOUT:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.PREFERENCES:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.IMPORT_CURVE_POINTS:
					case org.applied_geodesy.juniform.ui.menu.MenuItemType.IMPORT_SURFACE_POINTS:
						// do nothing
						break;
					}
				}
			}
		}

		public virtual ToggleGroup ToggleGroup
		{
			get
			{
				return this.featureToggleGroup;
			}
		}

		private static Menu createMenu(string label, bool mnemonicParsing)
		{
			Menu menu = new Menu(label);
			menu.setMnemonicParsing(mnemonicParsing);
			return menu;
		}

		private static MenuItem createMenuItem(MenuItem menuItem, bool mnemonicParsing, object userDate, KeyCodeCombination keyCodeCombination, MenuEventHandler menuEventHandler, bool disable)
		{
			menuItem.setMnemonicParsing(mnemonicParsing);
			if (keyCodeCombination != null)
			{
				menuItem.setAccelerator(keyCodeCombination);
			}
			menuItem.setOnAction(menuEventHandler);
			menuItem.setDisable(disable);
			menuItem.setUserData(userDate);
			return menuItem;
		}

		private static MenuItem createMenuItem(string label, bool mnemonicParsing, MenuItemType menuItemType, KeyCodeCombination keyCodeCombination, MenuEventHandler menuEventHandler, bool disable)
		{
			MenuItem menuItem = createMenuItem(new MenuItem(label), mnemonicParsing, menuItemType, keyCodeCombination, menuEventHandler, disable);

			return menuItem;
		}

		private static MenuItem createMenuItem(string label, bool mnemonicParsing, GeometricPrimitive geometry, KeyCodeCombination keyCodeCombination, MenuEventHandler menuEventHandler, bool disable)
		{
			MenuItem menuItem = createMenuItem(new MenuItem(label), mnemonicParsing, geometry, keyCodeCombination, menuEventHandler, disable);

			return menuItem;
		}

		private static RadioMenuItem createRadioMenuItem(string label, bool mnemonicParsing, MenuItemType menuItemType, KeyCodeCombination keyCodeCombination, MenuEventHandler menuEventHandler, bool disable)
		{
			RadioMenuItem menuItem = (RadioMenuItem)createMenuItem(new RadioMenuItem(label), mnemonicParsing, menuItemType, keyCodeCombination, menuEventHandler, disable);

			return menuItem;
		}

		private static MenuItem createMenuItem(string label, bool mnemonicParsing, MenuItemType menuItemType, File file, KeyCodeCombination keyCodeCombination, MenuEventHandler menuEventHandler, bool disable)
		{
			FileMenuItem menuItem = (FileMenuItem)createMenuItem(new FileMenuItem(label), mnemonicParsing, menuItemType, keyCodeCombination, menuEventHandler, disable);
			menuItem.File = file;
			return menuItem;
		}

		private void addGeometricPrimitiveItem(GeometricPrimitive geometry)
		{
			MenuItem geometryItem = createMenuItem(geometry.Name, true, geometry, null, this.menuEventHandler, false);
			geometryItem.textProperty().bind(geometry.nameProperty());
			this.initialGuessMenu.getItems().add(geometryItem);
			this.initialGuessMenu.setDisable(false);
		}

		private void removeGeometricPrimitiveItem(GeometricPrimitive geometry)
		{
			MenuItem geometryItem = null;
			foreach (MenuItem item in this.initialGuessMenu.getItems())
			{
				if (item.getUserData() == geometry)
				{
					geometryItem = item;
					break;
				}
			}
			if (geometryItem != null)
			{
				geometryItem.textProperty().unbind();
				this.initialGuessMenu.getItems().remove(geometryItem);
			}
			this.initialGuessMenu.setDisable(this.initialGuessMenu.getItems().isEmpty());
		}

		public virtual void featureChanged(FeatureEvent evt)
		{
			if (evt.EventType == FeatureEvent.FeatureEventType.FEATURE_ADDED)
			{
				// add geometries to initial guess menu
				foreach (GeometricPrimitive geometry in evt.Source.GeometricPrimitives)
				{
					this.addGeometricPrimitiveItem(geometry);
				}
				// add listener to handle new feature
				evt.Source.GeometricPrimitives.addListener(this.geometricPrimitiveListChangeListener);
			}
			else if (evt.EventType == FeatureEvent.FeatureEventType.FEATURE_REMOVED)
			{
				// add geometries to initial guess menu
				foreach (GeometricPrimitive geometry in evt.Source.GeometricPrimitives)
				{
					this.removeGeometricPrimitiveItem(geometry);
				}
				// remove listener from old feature
				evt.Source.GeometricPrimitives.removeListener(this.geometricPrimitiveListChangeListener);
			}
		}



		internal virtual void importFile(FeaturePointFileReader fileReader, ExtensionFilter[] extensionFilters, string title)
		{
			IList<File> selectedFiles = DefaultFileChooser.showOpenMultipleDialog(JUniForm.Stage, title, null, extensionFilters);

			if (selectedFiles == null || selectedFiles.Count == 0)
			{
				return;
			}

			this.importFile(fileReader, selectedFiles);
		}

		internal virtual void importFile(FeaturePointFileReader fileReader, IList<File> selectedFiles)
		{
			if (selectedFiles == null || selectedFiles.Count == 0)
			{
				return;
			}

			ObservableUniqueList<FeaturePoint> points = null;
			ReadFileProgressDialog<ObservableUniqueList<FeaturePoint>> dialog = new ReadFileProgressDialog<ObservableUniqueList<FeaturePoint>>();
			Optional<ObservableUniqueList<FeaturePoint>> optional = dialog.showAndWait(fileReader, selectedFiles);
			if (optional.isPresent())
			{
				points = optional.get();
			}

			if (points != null)
			{
				FeatureType prevFeatureType = UITreeBuilder.Instance.FeatureAdjustment.Feature != null ? UITreeBuilder.Instance.FeatureAdjustment.Feature.FeatureType : null;

				Toggle toggle = this.featureToggleGroup.getSelectedToggle();

				this.fireFeatureTypeChanged(fileReader.FeatureType);
				UIPointTableBuilder.Instance.Table.setItems(points);

				// re-select last feature (create a new instance because file has changed)
				if (prevFeatureType == fileReader.FeatureType && toggle != null && toggle is MenuItem)
				{
					toggle.setSelected(true);
					this.menuEventHandler.handleAction((MenuItem)toggle);
				}

				UITreeBuilder.Instance.handleTreeSelections();
			}
		}

		internal virtual void importInitialGuess(GeometricPrimitive geometry)
		{
			File selectedFile = DefaultFileChooser.showOpenDialog(JUniForm.Stage, i18n.getString("UIMenuBuilder.filechooser.import.initialguess.title", "Import initial guess from flat files"), null, InitialGuessFileReader.ExtensionFilters);

			if (selectedFile == null)
			{
				return;
			}

			InitialGuessFileReader fileReader = new InitialGuessFileReader(geometry);
			ReadFileProgressDialog<GeometricPrimitive> dialog = new ReadFileProgressDialog<GeometricPrimitive>();
			Optional<GeometricPrimitive> optional = dialog.showAndWait(fileReader, new List<File> {selectedFile});
			if (optional.isPresent())
			{
				optional.get();
			}
		}

		internal virtual void createReport(File templateFile)
		{
			try
			{
				if (UITreeBuilder.Instance.FeatureAdjustment.Feature == null)
				{
					return;
				}

				Pattern pattern = Pattern.compile(".*?\\.(\\w+)\\.ftlh$", Pattern.CASE_INSENSITIVE);
				Matcher matcher = pattern.matcher(templateFile.getName().ToLower());
				string extension = "html";
				ExtensionFilter extensionFilter = new ExtensionFilter(i18n.getString("UIMenuBuilder.report.extension.html", "Hypertext Markup Language"), "*.html", "*.htm", "*.HTML", "*.HTM");
				if (matcher.find() && matcher.groupCount() == 1)
				{
					extension = matcher.group(1);
					extensionFilter = new ExtensionFilter(String.format(Locale.ENGLISH, i18n.getString("UIMenuBuilder.report.extension.template", "%s-File"), extension), "*." + extension);
				}

				string fileNameSuggestion = "report." + extension;

				FTLReport ftl = new FTLReport(UITreeBuilder.Instance.FeatureAdjustment);
				File reportFile = DefaultFileChooser.showSaveDialog(JUniForm.Stage, i18n.getString("UIMenuBuilder.filechooser.report.title", "Save adjustment report"), fileNameSuggestion, extensionFilter);
				if (reportFile != null && ftl != null)
				{
					ftl.Template = templateFile.getName();
					ftl.toFile(reportFile);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.report.exception.title", "I/O Error"), i18n.getString("UIMenuBuilder.message.error.report.exception.header", "Error, could not create adjustment report."), i18n.getString("UIMenuBuilder.message.error.report.exception.message", "An exception has occurred during report creation."), e);
			}
		}

		private void fireFeatureTypeChanged(FeatureType featureType)
		{
			this.FeatureType = featureType;
			UIPointTableBuilder.Instance.FeatureType = featureType;
			UITreeBuilder.Instance.FeatureType = featureType;
		}

		public virtual bool ReportMenuDisable
		{
			set
			{
				foreach (MenuItem item in this.reportMenu.getItems())
				{
					item.setDisable(value);
				}
			}
		}

	}

}