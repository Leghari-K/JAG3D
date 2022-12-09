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

	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using FeatureType = org.applied_geodesy.adjustment.geometry.FeatureType;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using CircleFeature = org.applied_geodesy.adjustment.geometry.curve.CircleFeature;
	using EllipseFeature = org.applied_geodesy.adjustment.geometry.curve.EllipseFeature;
	using LineFeature = org.applied_geodesy.adjustment.geometry.curve.LineFeature;
	using ModifiableCurveFeature = org.applied_geodesy.adjustment.geometry.curve.ModifiableCurveFeature;
	using QuadraticCurveFeature = org.applied_geodesy.adjustment.geometry.curve.QuadraticCurveFeature;
	using CircularConeFeature = org.applied_geodesy.adjustment.geometry.surface.CircularConeFeature;
	using CircularCylinderFeature = org.applied_geodesy.adjustment.geometry.surface.CircularCylinderFeature;
	using CircularParaboloidFeature = org.applied_geodesy.adjustment.geometry.surface.CircularParaboloidFeature;
	using ConeFeature = org.applied_geodesy.adjustment.geometry.surface.ConeFeature;
	using CylinderFeature = org.applied_geodesy.adjustment.geometry.surface.CylinderFeature;
	using EllipsoidFeature = org.applied_geodesy.adjustment.geometry.surface.EllipsoidFeature;
	using ModifiableSurfaceFeature = org.applied_geodesy.adjustment.geometry.surface.ModifiableSurfaceFeature;
	using ParaboloidFeature = org.applied_geodesy.adjustment.geometry.surface.ParaboloidFeature;
	using PlaneFeature = org.applied_geodesy.adjustment.geometry.surface.PlaneFeature;
	using QuadraticSurfaceFeature = org.applied_geodesy.adjustment.geometry.surface.QuadraticSurfaceFeature;
	using SpatialCircleFeature = org.applied_geodesy.adjustment.geometry.surface.SpatialCircleFeature;
	using SpatialEllipseFeature = org.applied_geodesy.adjustment.geometry.surface.SpatialEllipseFeature;
	using SpatialLineFeature = org.applied_geodesy.adjustment.geometry.surface.SpatialLineFeature;
	using SphereFeature = org.applied_geodesy.adjustment.geometry.surface.SphereFeature;
	using FeaturePointFileReader = org.applied_geodesy.juniform.io.FeaturePointFileReader;
	using JUniForm = org.applied_geodesy.juniform.ui.JUniForm;
	using AboutDialog = org.applied_geodesy.juniform.ui.dialog.AboutDialog;
	using FeatureDialog = org.applied_geodesy.juniform.ui.dialog.FeatureDialog;
	using FormatterOptionDialog = org.applied_geodesy.juniform.ui.dialog.FormatterOptionDialog;
	using LeastSquaresSettingDialog = org.applied_geodesy.juniform.ui.dialog.LeastSquaresSettingDialog;
	using RestrictionDialog = org.applied_geodesy.juniform.ui.dialog.RestrictionDialog;
	using TestStatisticDialog = org.applied_geodesy.juniform.ui.dialog.TestStatisticDialog;
	using UnknownParameterDialog = org.applied_geodesy.juniform.ui.dialog.UnknownParameterDialog;
	using DisableStateType = org.applied_geodesy.juniform.ui.menu.UIMenuBuilder.DisableStateType;
	using UITreeBuilder = org.applied_geodesy.juniform.ui.tree.UITreeBuilder;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using MenuItem = javafx.scene.control.MenuItem;

	internal class MenuEventHandler : EventHandler<ActionEvent>
	{
		private UIMenuBuilder menuBuilder;
		private I18N i18n = I18N.Instance;
		private UITreeBuilder treeBuilder = UITreeBuilder.Instance;

		internal MenuEventHandler(UIMenuBuilder menuBuilder)
		{
			this.menuBuilder = menuBuilder;
		}

		public override void handle(ActionEvent @event)
		{
			if (@event.getSource() is MenuItem)
			{
				MenuItem menuItem = (MenuItem)@event.getSource();
				handleAction(menuItem);
			}
		}

		internal virtual void handleAction(MenuItem menuItem)
		{
			Feature feature = null;
			MenuItemType menuItemType = null;
			File file = null;

			if (menuItem.getUserData() is GeometricPrimitive)
			{
				this.menuBuilder.importInitialGuess((GeometricPrimitive)menuItem.getUserData());
				return;
			}
			else if (menuItem.getUserData() is MenuItemType)
			{
				menuItemType = (MenuItemType)menuItem.getUserData();
				file = menuItem is FileMenuItem ? ((FileMenuItem)menuItem).File : null;
			}

			if (menuItemType == null)
			{
				return;
			}

			switch (menuItemType)
			{
			case org.applied_geodesy.juniform.ui.menu.MenuItemType.IMPORT_CURVE_POINTS:
				this.menuBuilder.importFile(new FeaturePointFileReader(FeatureType.CURVE), FeaturePointFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.point.curve.title", "Import curve points from flat files"));
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.IMPORT_SURFACE_POINTS:
				this.menuBuilder.importFile(new FeaturePointFileReader(FeatureType.SURFACE), FeaturePointFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.point.surface.title", "Import surface points from flat files"));
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.FEATURE_PROPERTIES:
				FeatureDialog.showAndWait(this.treeBuilder.FeatureAdjustment.Feature);
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.PARAMETER_PROPERTIES:
				UnknownParameterDialog.showAndWait(this.treeBuilder.FeatureAdjustment.Feature);
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.RESTRICTION_PROPERTIES:
				RestrictionDialog.showAndWait(this.treeBuilder.FeatureAdjustment.Feature, false);
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.POSTPROCESSING_PROPERTIES:
				RestrictionDialog.showAndWait(this.treeBuilder.FeatureAdjustment.Feature, true);
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.TEST_STATISTIC:
				TestStatisticDialog.showAndWait(this.treeBuilder.FeatureAdjustment.TestStatisticDefinition);
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.LEAST_SQUARES:
				LeastSquaresSettingDialog.showAndWait(this.treeBuilder.FeatureAdjustment);
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.PREFERENCES:
				FormatterOptionDialog.showAndWait();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.EXIT:
				JUniForm.close();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.LINE:
				feature = new LineFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.CIRCLE:
				feature = new CircleFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.ELLIPSE:
				feature = new EllipseFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.QUADRATIC_CURVE:
				feature = new QuadraticCurveFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.PLANE:
				feature = new PlaneFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.SPHERE:
				feature = new SphereFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.ELLIPSOID:
				feature = new EllipsoidFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.SPATIAL_CIRCLE:
				feature = new SpatialCircleFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.SPATIAL_ELLIPSE:
				feature = new SpatialEllipseFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.SPATIAL_LINE:
				feature = new SpatialLineFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.CIRCULAR_CYLINDER:
				feature = new CircularCylinderFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.CYLINDER:
				feature = new CylinderFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.CIRCULAR_CONE:
				feature = new CircularConeFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.CONE:
				feature = new ConeFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.CIRCULAR_PARABOLOID:
				feature = new CircularParaboloidFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.PARABOLOID:
				feature = new ParaboloidFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.QUADRATIC_SURFACE:
				feature = new QuadraticSurfaceFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.MODIFIABLE_CURVE:
				feature = new ModifiableCurveFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.MODIFIABLE_SURFACE:
				feature = new ModifiableSurfaceFeature();
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.REPORT:
				this.menuBuilder.createReport(file);
				break;

			case org.applied_geodesy.juniform.ui.menu.MenuItemType.ABOUT:
				AboutDialog.showAndWait();
				break;

			}

			if (feature != null)
			{
				JUniForm.Title = menuItem.getText();
				this.menuBuilder.disableMenu(DisableStateType.FEATURE);
				this.treeBuilder.FeatureAdjustment.Feature = feature;
				this.treeBuilder.handleTreeSelections();
			}
			else
			{
				JUniForm.Title = null;
			}
		}
	}

}