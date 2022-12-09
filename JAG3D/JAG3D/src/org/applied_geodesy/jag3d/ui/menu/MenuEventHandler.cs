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

namespace org.applied_geodesy.jag3d.ui.menu
{

	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using JAG3D = org.applied_geodesy.jag3d.ui.JAG3D;
	using AboutDialog = org.applied_geodesy.jag3d.ui.dialog.AboutDialog;
	using AnalysisChartsDialog = org.applied_geodesy.jag3d.ui.dialog.AnalysisChartsDialog;
	using ApproximationValuesDialog = org.applied_geodesy.jag3d.ui.dialog.ApproximationValuesDialog;
	using AverageDialog = org.applied_geodesy.jag3d.ui.dialog.AverageDialog;
	using CongruentPointDialog = org.applied_geodesy.jag3d.ui.dialog.CongruentPointDialog;
	using FormatterOptionDialog = org.applied_geodesy.jag3d.ui.dialog.FormatterOptionDialog;
	using ImportOptionDialog = org.applied_geodesy.jag3d.ui.dialog.ImportOptionDialog;
	using LeastSquaresSettingDialog = org.applied_geodesy.jag3d.ui.dialog.LeastSquaresSettingDialog;
	using ProjectionAndReductionDialog = org.applied_geodesy.jag3d.ui.dialog.ProjectionAndReductionDialog;
	using RankDefectDialog = org.applied_geodesy.jag3d.ui.dialog.RankDefectDialog;
	using TableRowHighlightDialog = org.applied_geodesy.jag3d.ui.dialog.TableRowHighlightDialog;
	using TestStatisticDialog = org.applied_geodesy.jag3d.ui.dialog.TestStatisticDialog;

	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using MenuItem = javafx.scene.control.MenuItem;

	public class MenuEventHandler : EventHandler<ActionEvent>
	{
		private UIMenuBuilder menuBuilder;
		internal MenuEventHandler(UIMenuBuilder menuBuilder)
		{
			this.menuBuilder = menuBuilder;
		}

		public override void handle(ActionEvent @event)
		{
			if (@event.getSource() is MenuItem)
			{
				MenuItem menuItem = (MenuItem)@event.getSource();
				if (menuItem.getUserData() is MenuItemType)
				{
					MenuItemType menuItemType = (MenuItemType)menuItem.getUserData();
					File file = menuItem is FileMenuItem ? ((FileMenuItem)menuItem).File : null;
					handleAction(menuItemType, file);
				}
			}
		}

		private void handleAction(MenuItemType menuItemType, File file)
		{
			switch (menuItemType)
			{
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.EXIT:
				JAG3D.close();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.CLOSE:
				SQLManager.closeProject();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.ABOUT:
				AboutDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.NEW:
				this.menuBuilder.newProject();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.OPEN:
				this.menuBuilder.openProject();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.MIGRATE:
				this.menuBuilder.migrateProject();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.COPY:
				this.menuBuilder.copyProject();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.SQL_SCRIPT:
				this.menuBuilder.embedSQLSripts();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.FORMATTER_PREFERENCES:
				FormatterOptionDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_PREFERENCES:
				ImportOptionDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.LEAST_SQUARES:
				LeastSquaresSettingDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.TEST_STATISTIC:
				TestStatisticDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.RANK_DEFECT:
				RankDefectDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.PROJECTION_AND_REDUCTION:
				ProjectionAndReductionDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.CONGRUENT_POINT:
				CongruentPointDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.APROXIMATE_VALUES:
				ApproximationValuesDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.AVERAGE:
				AverageDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.CHECK_UPDATES:
				this.menuBuilder.checkUpdates();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_DIRECTION:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_GNSS1D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_GNSS2D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_GNSS3D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_HORIZONTAL_DISTANCE:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_LEVELING:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_SLOPE_DISTANCE:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_ZENITH_ANGLE:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_BEO:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GSI1D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GSI2D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GSI2DH:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GSI3D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_LAND_XML2D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_LAND_XML3D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_M5:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GKA:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_JOB_XML2D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_JOB_XML2DH:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_JOB_XML3D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_DL100:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_Z:

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_REFERENCE_POINT_1D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_REFERENCE_POINT_2D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_REFERENCE_POINT_3D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_STOCHASTIC_POINT_1D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_STOCHASTIC_POINT_2D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_STOCHASTIC_POINT_3D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_DATUM_POINT_1D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_DATUM_POINT_2D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_DATUM_POINT_3D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_NEW_POINT_1D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_NEW_POINT_2D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_NEW_POINT_3D:

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_CONGRUENCE_ANALYSIS_PAIR_1D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_CONGRUENCE_ANALYSIS_PAIR_2D:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_CONGRUENCE_ANALYSIS_PAIR_3D:

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_REFERENCE_VERTICAL_DEFLECTION:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_STOCASTIC_VERTICAL_DEFLECTION:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_UNKNOWN_VERTICAL_DEFLECTION:

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_COLUMN_BASED_FILES:
				this.menuBuilder.importFile(menuItemType);
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.HIGHLIGHT_TABLE_ROWS:
				TableRowHighlightDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.ANALYSIS_CHARTS:
				AnalysisChartsDialog.showAndWait();
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.MODULE_COORDTRANS:
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.MODULE_GEOTRA:
				this.menuBuilder.showSwingApplication(menuItemType);
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.MODULE_JUNIFORM:
				this.menuBuilder.showModule(menuItemType);
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.REPORT:
				this.menuBuilder.createReport(file);
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.RECENTLY_USED:
				this.menuBuilder.openProject(file);
				break;
			}
		}
	}


}