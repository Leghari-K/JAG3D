using System;
using System.Collections.Generic;
using System.IO;

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

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using DefaultApplicationProperty = org.applied_geodesy.jag3d.DefaultApplicationProperty;
	using ProjectDatabaseStateChangeListener = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateChangeListener;
	using ProjectDatabaseStateEvent = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateEvent;
	using ProjectDatabaseStateType = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using JAG3D = org.applied_geodesy.jag3d.ui.JAG3D;
	using ColumnImportDialog = org.applied_geodesy.jag3d.ui.dialog.ColumnImportDialog;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using BeoFileReader = org.applied_geodesy.jag3d.ui.io.BeoFileReader;
	using CongruenceAnalysisFlatFileReader = org.applied_geodesy.jag3d.ui.io.CongruenceAnalysisFlatFileReader;
	using DL100FileReader = org.applied_geodesy.jag3d.ui.io.DL100FileReader;
	using DimensionType = org.applied_geodesy.jag3d.ui.io.DimensionType;
	using org.applied_geodesy.jag3d.ui.io;
	using GKAFileReader = org.applied_geodesy.jag3d.ui.io.GKAFileReader;
	using GSIFileReader = org.applied_geodesy.jag3d.ui.io.GSIFileReader;
	using M5FileReader = org.applied_geodesy.jag3d.ui.io.M5FileReader;
	using ObservationFlatFileReader = org.applied_geodesy.jag3d.ui.io.ObservationFlatFileReader;
	using PointFlatFileReader = org.applied_geodesy.jag3d.ui.io.PointFlatFileReader;
	using VerticalDeflectionFlatFileReader = org.applied_geodesy.jag3d.ui.io.VerticalDeflectionFlatFileReader;
	using ZFileReader = org.applied_geodesy.jag3d.ui.io.ZFileReader;
	using FTLReport = org.applied_geodesy.jag3d.ui.io.report.FTLReport;
	using OADBReader = org.applied_geodesy.jag3d.ui.io.sql.OADBReader;
	using HeXMLFileReader = org.applied_geodesy.jag3d.ui.io.xml.HeXMLFileReader;
	using JobXMLFileReader = org.applied_geodesy.jag3d.ui.io.xml.JobXMLFileReader;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using JUniForm = org.applied_geodesy.juniform.ui.JUniForm;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DefaultFileChooser = org.applied_geodesy.ui.io.DefaultFileChooser;
	using org.applied_geodesy.util.io;
	using HTTPPropertiesLoader = org.applied_geodesy.util.io.properties.HTTPPropertiesLoader;
	using URLParameter = org.applied_geodesy.util.io.properties.URLParameter;
	using DataBase = org.applied_geodesy.util.sql.DataBase;
	using HSQLDB = org.applied_geodesy.util.sql.HSQLDB;
	using DatabaseVersionMismatchException = org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException;

	using CoordTrans = com.derletztekick.geodesy.coordtrans.v2.gui.CoordTrans;
	using GeoTra = com.derletztekick.geodesy.geotra.gui.GeoTra;

	using HostServices = javafx.application.HostServices;
	using Platform = javafx.application.Platform;
	using ButtonType = javafx.scene.control.ButtonType;
	using Menu = javafx.scene.control.Menu;
	using MenuBar = javafx.scene.control.MenuBar;
	using MenuItem = javafx.scene.control.MenuItem;
	using SeparatorMenuItem = javafx.scene.control.SeparatorMenuItem;
	using TreeItem = javafx.scene.control.TreeItem;
	using TreeView = javafx.scene.control.TreeView;
	using KeyCode = javafx.scene.input.KeyCode;
	using KeyCodeCombination = javafx.scene.input.KeyCodeCombination;
	using KeyCombination = javafx.scene.input.KeyCombination;
	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;
	using Stage = javafx.stage.Stage;

	public class UIMenuBuilder
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			menuEventHandler = new MenuEventHandler(this);
		}

		private class DatabaseStateChangeListener : ProjectDatabaseStateChangeListener
		{
			private readonly UIMenuBuilder outerInstance;

			public DatabaseStateChangeListener(UIMenuBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal readonly ISet<MenuItemType> unDisableItemTypes = new HashSet<MenuItemType>(Arrays.asList(MenuItemType.NEW, MenuItemType.OPEN, MenuItemType.EXIT, MenuItemType.RECENTLY_USED, MenuItemType.ABOUT, MenuItemType.CHECK_UPDATES, MenuItemType.MODULE_GEOTRA, MenuItemType.MODULE_COORDTRANS, MenuItemType.MODULE_JUNIFORM));

			public virtual void projectDatabaseStateChanged(ProjectDatabaseStateEvent evt)
			{
				if (outerInstance.menuBar == null || outerInstance.menuBar.getChildrenUnmodifiable().isEmpty())
				{
					return;
				}

				bool disable = evt.EventType != ProjectDatabaseStateType.OPENED;
				IList<Menu> menus = outerInstance.menuBar.getMenus();
				foreach (Menu menu in menus)
				{
					this.disableMenu(menu, disable);
				}
			}

			internal virtual void disableMenu(Menu menu, bool disable)
			{
				IList<MenuItem> items = menu.getItems();
				foreach (MenuItem item in items)
				{

					if (item is Menu)
					{
						this.disableMenu((Menu)item, disable);
					}
					else if (!this.unDisableItemTypes.Contains(item.getUserData()))
					{
						item.setDisable(disable);
					}
				}
			}
		}

		private File historyFile = new File(System.getProperty("user.home") + File.separator + ".jag3d_history");
		private Menu historyMenu;
		private HostServices hostServices;
		private static UIMenuBuilder menuBuilder = new UIMenuBuilder();
		private I18N i18n = I18N.Instance;
		private MenuEventHandler menuEventHandler;
		private MenuBar menuBar;

		private UIMenuBuilder()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public static UIMenuBuilder Instance
		{
			get
			{
				return menuBuilder;
			}
		}

		public static HostServices HostServices
		{
			set
			{
				menuBuilder.hostServices = value;
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
			SQLManager.Instance.addProjectDatabaseStateChangeListener(new DatabaseStateChangeListener(this));

			this.historyFile = DefaultApplicationProperty.DefaultHistoryPath;

			this.menuBar = new MenuBar();

			// Create menus
			Menu projectMenu = createMenu(i18n.getString("UIMenuBuilder.menu.project.label", "_Project"), true);
			this.createProjectMenu(projectMenu);

			Menu importMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.label", "_Import"), true);
			this.createImportMenu(importMenu);

			Menu propertyMenu = createMenu(i18n.getString("UIMenuBuilder.menu.property.label", "Propertie_s"), true);
			this.createPropertyMenu(propertyMenu);

			Menu preprocessingMenu = createMenu(i18n.getString("UIMenuBuilder.menu.preprocessing.label", "P_reprocessing"), true);
			this.createPreprocessingMenu(preprocessingMenu);

			Menu analysisMenu = createMenu(i18n.getString("UIMenuBuilder.menu.analysis.label", "_Analysis"), true);
			this.createAnalysisMenu(analysisMenu);

			Menu reportMenu = createMenu(i18n.getString("UIMenuBuilder.menu.report.label", "Repor_t"), true);
			this.createReportMenu(reportMenu);

			Menu moduleMenu = createMenu(i18n.getString("UIMenuBuilder.menu.module.label", "_Module"), true);
			this.createModuleMenu(moduleMenu);

			Menu helpMenu = createMenu(i18n.getString("UIMenuBuilder.menu.help.label", "_Help"), true);
			this.createHelpMenu(helpMenu);

			this.menuBar.getMenus().addAll(projectMenu, importMenu, propertyMenu, preprocessingMenu, analysisMenu, reportMenu, moduleMenu, helpMenu);
		}

		private void createPreprocessingMenu(Menu parentMenu)
		{
			MenuItem approximationValuesItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.preprocessing.approximation.label", "Aproximation _values"), true, MenuItemType.APROXIMATE_VALUES, new KeyCodeCombination(KeyCode.M, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem averageItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.preprocessing.average.label", "Avera_ge observations"), true, MenuItemType.AVERAGE, new KeyCodeCombination(KeyCode.G, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);

			parentMenu.getItems().addAll(approximationValuesItem, averageItem);
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

		private void createModuleMenu(Menu parentMenu)
		{
			MenuItem moduleGEOTRAItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.module.geotra.label", "Coordinate converter"), true, MenuItemType.MODULE_GEOTRA, null, this.menuEventHandler, false);
			MenuItem moduleCOORDTRANSItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.module.coordtrans.label", "Coordinate transformation"), true, MenuItemType.MODULE_COORDTRANS, null, this.menuEventHandler, false);
			MenuItem moduleJUniFormItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.module.juniform.label", "Curve and surface analysis"), true, MenuItemType.MODULE_JUNIFORM, null, this.menuEventHandler, false);


			parentMenu.getItems().addAll(moduleGEOTRAItem, moduleCOORDTRANSItem, moduleJUniFormItem);
		}

		private void createHelpMenu(Menu parentMenu)
		{
			MenuItem aboutItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.help.about.label", "_About JAG3D"), true, MenuItemType.ABOUT, new KeyCodeCombination(KeyCode.W, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, false);
			MenuItem updateItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.help.check_update.label", "Check for _updates\u2026"), true, MenuItemType.CHECK_UPDATES, new KeyCodeCombination(KeyCode.U, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, false);
			parentMenu.getItems().addAll(aboutItem, updateItem);
		}

		private void createAnalysisMenu(Menu parentMenu)
		{
			MenuItem congruentPointItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.analysis.congruentpoint.label", "Congr_uent points"), true, MenuItemType.CONGRUENT_POINT, new KeyCodeCombination(KeyCode.P, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem rowHighlightItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.analysis.highlight.label", "Ro_w highlighting"), true, MenuItemType.HIGHLIGHT_TABLE_ROWS, new KeyCodeCombination(KeyCode.H, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem analysisChartsItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.analysis.chart.label", "Analysis charts"), true, MenuItemType.ANALYSIS_CHARTS, new KeyCodeCombination(KeyCode.Y, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);

			parentMenu.getItems().addAll(congruentPointItem, rowHighlightItem, analysisChartsItem);
		}

		private void createPropertyMenu(Menu parentMenu)
		{
			MenuItem formatterItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.property.formatter.label", "Formatter preferences"), true, MenuItemType.FORMATTER_PREFERENCES, new KeyCodeCombination(KeyCode.ENTER, KeyCombination.ALT_DOWN), this.menuEventHandler, true);
			MenuItem importItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.property.import.label", "Import preferences"), true, MenuItemType.IMPORT_PREFERENCES, new KeyCodeCombination(KeyCode.I, KeyCombination.SHORTCUT_DOWN, KeyCombination.ALT_DOWN), this.menuEventHandler, true);
			MenuItem leastSquaresItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.property.leastsquares.label", "Least-squares"), true, MenuItemType.LEAST_SQUARES, new KeyCodeCombination(KeyCode.Q, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem teststatisticItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.property.teststatistic.label", "Test statistic"), true, MenuItemType.TEST_STATISTIC, new KeyCodeCombination(KeyCode.T, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem projectionItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.property.projection.label", "Projection \u0026 reductions"), true, MenuItemType.PROJECTION_AND_REDUCTION, new KeyCodeCombination(KeyCode.I, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem rankDefectItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.property.rankdefect.label", "Rank defect"), true, MenuItemType.RANK_DEFECT, new KeyCodeCombination(KeyCode.R, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);

			parentMenu.getItems().addAll(leastSquaresItem, teststatisticItem, rankDefectItem, projectionItem, new SeparatorMenuItem(), formatterItem, importItem);
		}

		private void createProjectMenu(Menu parentMenu)
		{
			MenuItem newItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.project.new.label", "Create _new project"), true, MenuItemType.NEW, new KeyCodeCombination(KeyCode.N, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, false);
			MenuItem openItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.project.open.label", "_Open existing project"), true, MenuItemType.OPEN, new KeyCodeCombination(KeyCode.O, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, false);
			MenuItem migrateItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.project.migrate.label", "Migrate exi_sting project"), true, MenuItemType.MIGRATE, new KeyCodeCombination(KeyCode.M, KeyCombination.SHORTCUT_DOWN, KeyCombination.ALT_DOWN), this.menuEventHandler, true);
			MenuItem copyItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.project.copy.label", "_Copy current project and open"), true, MenuItemType.COPY, new KeyCodeCombination(KeyCode.C, KeyCombination.SHORTCUT_DOWN, KeyCombination.ALT_DOWN), this.menuEventHandler, true);
			MenuItem sqlItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.project.sql.label", "Execute S_QL script"), true, MenuItemType.SQL_SCRIPT, new KeyCodeCombination(KeyCode.X, KeyCombination.SHORTCUT_DOWN, KeyCombination.ALT_DOWN), this.menuEventHandler, true);
			MenuItem closeItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.project.close.label", "C_lose current project"), true, MenuItemType.CLOSE, new KeyCodeCombination(KeyCode.L, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, true);
			MenuItem exitItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.project.exit.label", "_Exit"), true, MenuItemType.EXIT, new KeyCodeCombination(KeyCode.E, KeyCombination.SHORTCUT_DOWN), this.menuEventHandler, false);
			Menu historyMenu = this.createHistoryMenu();

			parentMenu.getItems().addAll(newItem, openItem, migrateItem, closeItem, copyItem, sqlItem, new SeparatorMenuItem(), historyMenu, new SeparatorMenuItem(), exitItem);
		}

		private void writeProjectHistory()
		{
			if (this.historyFile == null)
			{
				return;
			}

			IList<MenuItem> items = this.historyMenu.getItems();
			if (items.Count == 0)
			{
				return;
			}

			PrintWriter pw = null;
			try
			{
				pw = new PrintWriter(new StreamWriter(this.historyFile));
				for (int i = 0 ; i < items.Count; i++)
				{
					if (items[i] is FileMenuItem)
					{
						FileMenuItem fileMenuItem = (FileMenuItem)items[i];
						pw.printf(Locale.ENGLISH, "%s\r\n", fileMenuItem.File);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				if (pw != null)
				{
					pw.close();
				}
			}
		}

		private void addHistoryFile(File file)
		{
			try
			{
				IList<MenuItem> items = new List<MenuItem>(this.historyMenu.getItems());
				items.Reverse();
				int removeIndex = -1;
				for (int i = 0 ; i < items.Count; i++)
				{
					if (items[i] is FileMenuItem)
					{
						FileMenuItem fileMenuItem = (FileMenuItem)items[i];
						if (file.getCanonicalPath().Equals(fileMenuItem.File.getCanonicalPath()))
						{
							removeIndex = i;
							break;
						}
					}
				}
				if (removeIndex >= 0)
				{
					items.RemoveAt(removeIndex);
				}


				string itemName = file.ToString();
				string parent = file.getParent();
				if (parent.Length > 60)
				{
					itemName = parent.Substring(0, 50) + "\u2026" + File.separator + file.getName();
				}

				MenuItem newFileItem = createMenuItem(itemName, false, MenuItemType.RECENTLY_USED, file, null, this.menuEventHandler, false);
				items.Add(newFileItem);

				while (items.Count >= 10)
				{
					items.RemoveAt(0);
				}

				items.Reverse();
				this.historyMenu.getItems().setAll(items);
				this.historyMenu.setDisable(items.Count == 0);
				this.writeProjectHistory();

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		private Menu createHistoryMenu()
		{
			if (this.historyMenu == null)
			{
				this.historyMenu = createMenu(i18n.getString("UIMenuBuilder.menu.project.history.label", "_Recently used projects"), true);
				IList<MenuItem> newItems = this.historyMenu.getItems();
				newItems.Clear();

				if (this.historyFile == null)
				{
					this.historyMenu.setDisable(newItems.Count == 0);
					return this.historyMenu;
				}

				string regex = "(?i)(.+?)(\\.)(backup$|data$|properties$|script$)";
				Scanner scanner = null;
				try
				{
					Path path = this.historyFile.toPath();
					if (Files.exists(path) && Files.isRegularFile(path))
					{
						scanner = new Scanner(this.historyFile);
						while (scanner.hasNext())
						{
							string @string = scanner.nextLine().Trim();
							File file = new File(@string);
							if (@string.Length > 0 && @string.matches(regex) && Files.exists(file.toPath()) && Files.isRegularFile(file.toPath()))
							{
								string itemName = file.ToString();
								string parent = file.getParent();
								if (parent.Length > 60)
								{
									itemName = parent.Substring(0, 50) + "\u2026" + File.separator + file.getName();
								}

								MenuItem fileItem = createMenuItem(itemName, false, MenuItemType.RECENTLY_USED, file, null, this.menuEventHandler, false);
								newItems.Add(fileItem);
							}
							if (newItems.Count == 10)
							{
								break;
							}
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					newItems.Clear();
				}
				finally
				{
					if (scanner != null)
					{
						scanner.close();
					}
					this.historyMenu.setDisable(newItems.Count == 0);
				}
			}
			return this.historyMenu;
		}

		private void createImportMenu(Menu parentMenu)
		{
			Menu importReferencePointFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.flat.point.reference.label", "Reference points"), true);
			MenuItem importReferencePoint1DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.reference.1d.label", "Reference points 1D"), true, MenuItemType.IMPORT_FLAT_REFERENCE_POINT_1D, null, this.menuEventHandler, true);
			MenuItem importReferencePoint2DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.reference.2d.label", "Reference points 2D"), true, MenuItemType.IMPORT_FLAT_REFERENCE_POINT_2D, null, this.menuEventHandler, true);
			MenuItem importReferencePoint3DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.reference.3d.label", "Reference points 3D"), true, MenuItemType.IMPORT_FLAT_REFERENCE_POINT_3D, null, this.menuEventHandler, true);

			Menu importStochasticPointFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.flat.point.stochastic.label", "Stochastic points"), true);
			MenuItem importStochasticPoint1DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.stochastic.1d.label", "Stochastic points 1D"), true, MenuItemType.IMPORT_FLAT_STOCHASTIC_POINT_1D, null, this.menuEventHandler, true);
			MenuItem importStochasticPoint2DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.stochastic.2d.label", "Stochastic points 2D"), true, MenuItemType.IMPORT_FLAT_STOCHASTIC_POINT_2D, null, this.menuEventHandler, true);
			MenuItem importStochasticPoint3DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.stochastic.3d.label", "Stochastic points 3D"), true, MenuItemType.IMPORT_FLAT_STOCHASTIC_POINT_3D, null, this.menuEventHandler, true);

			Menu importDatumPointFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.flat.point.datum.label", "Datum points"), true);
			MenuItem importDatumPoint1DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.datum.1d.label", "Datum points 1D"), true, MenuItemType.IMPORT_FLAT_DATUM_POINT_1D, null, this.menuEventHandler, true);
			MenuItem importDatumPoint2DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.datum.2d.label", "Datum points 2D"), true, MenuItemType.IMPORT_FLAT_DATUM_POINT_2D, null, this.menuEventHandler, true);
			MenuItem importDatumPoint3DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.datum.3d.label", "Datum points 3D"), true, MenuItemType.IMPORT_FLAT_DATUM_POINT_3D, null, this.menuEventHandler, true);

			Menu importNewPointFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.flat.point.new.label", "New points"), true);
			MenuItem importNewPoint1DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.new.1d.label", "New points 1D"), true, MenuItemType.IMPORT_FLAT_NEW_POINT_1D, null, this.menuEventHandler, true);
			MenuItem importNewPoint2DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.new.2d.label", "New points 2D"), true, MenuItemType.IMPORT_FLAT_NEW_POINT_2D, null, this.menuEventHandler, true);
			MenuItem importNewPoint3DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.point.new.3d.label", "New points 3D"), true, MenuItemType.IMPORT_FLAT_NEW_POINT_3D, null, this.menuEventHandler, true);

			Menu importVerticalDeflectionFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.flat.deflection.label", "Vertical deflection"), true);
			MenuItem importReferenceDeflectionFlatMenu = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.deflection.reference.label", "Reference deflection"), true, MenuItemType.IMPORT_FLAT_REFERENCE_VERTICAL_DEFLECTION, null, this.menuEventHandler, true);
			MenuItem importStochasticDeflectionFlatMenu = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.deflection.stochastic.label", "Stochastic deflection"), true, MenuItemType.IMPORT_FLAT_STOCASTIC_VERTICAL_DEFLECTION, null, this.menuEventHandler, true);
			MenuItem importUnknownDeflectionFlatMenu = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.deflection.unknown.label", "Approximated deflection"), true, MenuItemType.IMPORT_FLAT_UNKNOWN_VERTICAL_DEFLECTION, null, this.menuEventHandler, true);

			Menu importTerrestrialFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.flat.terrestrial.label", "Terrestrial observations"), true);
			MenuItem importLevelingFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.terrestrial.leveling.label", "Leveling data"), true, MenuItemType.IMPORT_FLAT_LEVELING, null, this.menuEventHandler, true);
			MenuItem importDirectionFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.terrestrial.direction.label", "Direction sets"), true, MenuItemType.IMPORT_FLAT_DIRECTION, null, this.menuEventHandler, true);
			MenuItem importHorizontalDistanceFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.terrestrial.horizontal_distance.label", "Horizontal distances"), true, MenuItemType.IMPORT_FLAT_HORIZONTAL_DISTANCE, null, this.menuEventHandler, true);
			MenuItem importSlopeDistanceFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.terrestrial.slope_distance.label", "Slope distances"), true, MenuItemType.IMPORT_FLAT_SLOPE_DISTANCE, null, this.menuEventHandler, true);
			MenuItem importZenithAngleFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.terrestrial.zenith_angle.label", "Zenith angles"), true, MenuItemType.IMPORT_FLAT_ZENITH_ANGLE, null, this.menuEventHandler, true);

			Menu importCongruenceAnalysisPairFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.flat.congruence_analysis.label", "Congruence Analysis"), true);
			MenuItem importCongruenceAnalysisPair1DFlatMenu = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.congruence_analysis.1d.label", "Point nexus 1D"), true, MenuItemType.IMPORT_FLAT_CONGRUENCE_ANALYSIS_PAIR_1D, null, this.menuEventHandler, true);
			MenuItem importCongruenceAnalysisPair2DFlatMenu = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.congruence_analysis.2d.label", "Point nexus 2D"), true, MenuItemType.IMPORT_FLAT_CONGRUENCE_ANALYSIS_PAIR_2D, null, this.menuEventHandler, true);
			MenuItem importCongruenceAnalysisPair3DFlatMenu = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.congruence_analysis.3d.label", "Point nexus 3D"), true, MenuItemType.IMPORT_FLAT_CONGRUENCE_ANALYSIS_PAIR_3D, null, this.menuEventHandler, true);

			Menu importGNSSFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.flat.gnss.label", "GNSS baselines"), true);
			MenuItem importGNSS1DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.gnss.1d.label", "GNSS baselines 1D"), true, MenuItemType.IMPORT_FLAT_GNSS1D, null, this.menuEventHandler, true);
			MenuItem importGNSS2DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.gnss.2d.label", "GNSS baselines 2D"), true, MenuItemType.IMPORT_FLAT_GNSS2D, null, this.menuEventHandler, true);
			MenuItem importGNSS3DFlatItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.flat.gnss.3d.label", "GNSS baselines 3D"), true, MenuItemType.IMPORT_FLAT_GNSS3D, null, this.menuEventHandler, true);

			Menu importHexagonFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.hexagon.label", "Hexagon/Leica"), true);
			MenuItem gsi1DFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.hexagon.gsi.1d.label", "GSI 1D"), true, MenuItemType.IMPORT_GSI1D, null, this.menuEventHandler, true);
			MenuItem gsi2DFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.hexagon.gsi.2d.label", "GSI 2D"), true, MenuItemType.IMPORT_GSI2D, null, this.menuEventHandler, true);
			MenuItem gsi2DHFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.hexagon.gsi.2dh.label", "GSI 2D+H"), true, MenuItemType.IMPORT_GSI2DH, null, this.menuEventHandler, true);
			MenuItem gsi3DFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.hexagon.gsi.3d.label", "GSI 3D"), true, MenuItemType.IMPORT_GSI3D, null, this.menuEventHandler, true);

			MenuItem hexml2DFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.hexagon.landxml.2d.label", "LandXML 1.2 2D"), true, MenuItemType.IMPORT_LAND_XML2D, null, this.menuEventHandler, true);
			MenuItem hexml3DFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.hexagon.landxml.3d.label", "LandXML 1.2 3D"), true, MenuItemType.IMPORT_LAND_XML3D, null, this.menuEventHandler, true);

			Menu importTrimbleFlatMenu = createMenu(i18n.getString("UIMenuBuilder.menu.import.trimble.label", "Trimble/Zeiss"), true);
			MenuItem m5FileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.trimble.m5.label", "M5 (DiNi)"), true, MenuItemType.IMPORT_M5, null, this.menuEventHandler, true);
			MenuItem gkaFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.trimble.gka.label", "GKA"), true, MenuItemType.IMPORT_GKA, null, this.menuEventHandler, true);
			MenuItem jxml2DFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.trimble.jobxml.2d.label", "JobXML 2D"), true, MenuItemType.IMPORT_JOB_XML2D, null, this.menuEventHandler, true);
			MenuItem jxml2DHFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.trimble.jobxml.2dh.label", "JobXML 2D+H"), true, MenuItemType.IMPORT_JOB_XML2DH, null, this.menuEventHandler, true);
			MenuItem jxml3DFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.trimble.jobxml.3d.label", "JobXML 3D"), true, MenuItemType.IMPORT_JOB_XML3D, null, this.menuEventHandler, true);


			MenuItem dl100FileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.topcon.dl100.label", "DL-100 (Topcon)"), true, MenuItemType.IMPORT_DL100, null, this.menuEventHandler, true);

			MenuItem zFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.z.label", "Z-File (Caplan)"), true, MenuItemType.IMPORT_Z, null, this.menuEventHandler, true);
			MenuItem beoFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.beo.label", "Beo-File (Neptan)"), true, MenuItemType.IMPORT_BEO, null, this.menuEventHandler, true);

			MenuItem columnBasedFileItem = createMenuItem(i18n.getString("UIMenuBuilder.menu.import.column_based.label", "Column-based data"), true, MenuItemType.IMPORT_COLUMN_BASED_FILES, null, this.menuEventHandler, true);

			importHexagonFlatMenu.getItems().addAll(gsi1DFileItem, gsi2DFileItem, gsi2DHFileItem, gsi3DFileItem, new SeparatorMenuItem(), hexml2DFileItem, hexml3DFileItem);

			importTrimbleFlatMenu.getItems().addAll(m5FileItem, gkaFileItem, new SeparatorMenuItem(), jxml2DFileItem, jxml2DHFileItem, jxml3DFileItem);

			importTerrestrialFlatMenu.getItems().addAll(importLevelingFlatItem, new SeparatorMenuItem(), importDirectionFlatItem, importHorizontalDistanceFlatItem, new SeparatorMenuItem(), importSlopeDistanceFlatItem, importZenithAngleFlatItem);

			importReferencePointFlatMenu.getItems().addAll(importReferencePoint1DFlatItem, importReferencePoint2DFlatItem, importReferencePoint3DFlatItem);

			importStochasticPointFlatMenu.getItems().addAll(importStochasticPoint1DFlatItem, importStochasticPoint2DFlatItem, importStochasticPoint3DFlatItem);

			importDatumPointFlatMenu.getItems().addAll(importDatumPoint1DFlatItem, importDatumPoint2DFlatItem, importDatumPoint3DFlatItem);

			importNewPointFlatMenu.getItems().addAll(importNewPoint1DFlatItem, importNewPoint2DFlatItem, importNewPoint3DFlatItem);

			importVerticalDeflectionFlatMenu.getItems().addAll(importReferenceDeflectionFlatMenu, importStochasticDeflectionFlatMenu, importUnknownDeflectionFlatMenu);

			importCongruenceAnalysisPairFlatMenu.getItems().addAll(importCongruenceAnalysisPair1DFlatMenu, importCongruenceAnalysisPair2DFlatMenu, importCongruenceAnalysisPair3DFlatMenu);

			importGNSSFlatMenu.getItems().addAll(importGNSS1DFlatItem, importGNSS2DFlatItem, importGNSS3DFlatItem);

			parentMenu.getItems().addAll(importReferencePointFlatMenu, importStochasticPointFlatMenu, importDatumPointFlatMenu, importNewPointFlatMenu, new SeparatorMenuItem(), importVerticalDeflectionFlatMenu, new SeparatorMenuItem(), importTerrestrialFlatMenu, importGNSSFlatMenu, new SeparatorMenuItem(), importCongruenceAnalysisPairFlatMenu, new SeparatorMenuItem(), importHexagonFlatMenu, importTrimbleFlatMenu, dl100FileItem, new SeparatorMenuItem(), beoFileItem, zFileItem, new SeparatorMenuItem(), columnBasedFileItem);
		}

		private static Menu createMenu(string label, bool mnemonicParsing)
		{
			Menu menu = new Menu(label);
			menu.setMnemonicParsing(mnemonicParsing);
			return menu;
		}

	//	private static RadioMenuItem createRadioMenuItem(String label, boolean mnemonicParsing, MenuItemType menuItemType, KeyCodeCombination keyCodeCombination, MenuEventHandler menuEventHandler, boolean disable) {
	//		RadioMenuItem menuItem = new RadioMenuItem(label);
	//		menuItem.setMnemonicParsing(mnemonicParsing);
	//		if (keyCodeCombination != null)
	//			menuItem.setAccelerator(keyCodeCombination);
	//		menuItem.setOnAction(menuEventHandler);
	//		menuItem.setDisable(disable);
	//		menuItem.setUserData(menuItemType);
	//		return menuItem;
	//	}

		private static MenuItem createMenuItem(MenuItem menuItem, bool mnemonicParsing, MenuItemType menuItemType, KeyCodeCombination keyCodeCombination, MenuEventHandler menuEventHandler, bool disable)
		{
			menuItem.setMnemonicParsing(mnemonicParsing);
			if (keyCodeCombination != null)
			{
				menuItem.setAccelerator(keyCodeCombination);
			}
			menuItem.setOnAction(menuEventHandler);
			menuItem.setDisable(disable);
			menuItem.setUserData(menuItemType);
			return menuItem;
		}

		private static MenuItem createMenuItem(string label, bool mnemonicParsing, MenuItemType menuItemType, KeyCodeCombination keyCodeCombination, MenuEventHandler menuEventHandler, bool disable)
		{
			MenuItem menuItem = createMenuItem(new MenuItem(label), mnemonicParsing, menuItemType, keyCodeCombination, menuEventHandler, disable);

			return menuItem;
		}

		private static MenuItem createMenuItem(string label, bool mnemonicParsing, MenuItemType menuItemType, File file, KeyCodeCombination keyCodeCombination, MenuEventHandler menuEventHandler, bool disable)
		{
			FileMenuItem menuItem = (FileMenuItem)createMenuItem(new FileMenuItem(label), mnemonicParsing, menuItemType, keyCodeCombination, menuEventHandler, disable);
			menuItem.File = file;
			return menuItem;
		}

		internal virtual void newProject()
		{
			File selectedFile = DefaultFileChooser.showSaveDialog(JAG3D.Stage, i18n.getString("UIMenuBuilder.filechooser.new.title", "Create new project"), "jag3d_project.script", new ExtensionFilter(i18n.getString("OADBReader.extension.script", "HyperSQL"), "*.script"));

			try
			{
				if (selectedFile != null)
				{
					Path path = selectedFile.toPath();
					string regex = "(?i)(.+?)(\\.)(backup$|data$|properties$|script$)";
					string project = path.toAbsolutePath().ToString().replaceFirst(regex, "$1");

					if (!string.ReferenceEquals(project, null))
					{
						SQLManager.createNewProject(new HSQLDB(project));
						JAG3D.Title = path.getFileName() == null ? null : path.getFileName().ToString().replaceFirst(regex, "$1");
						this.addHistoryFile(selectedFile);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.new.exception.title", "I/O Error"), i18n.getString("UIMenuBuilder.message.error.new.exception.header", "Error, could not create new project."), i18n.getString("UIMenuBuilder.message.error.new.exception.message", "An exception has occurred during project creation."), e);
				SQLManager.Instance.closeDataBase();
			}
		}

		internal virtual void copyProject()
		{
			try
			{
				// check for loaded db
				if (!SQLManager.Instance.hasDatabase())
				{
					return;
				}

				string[] dataBaseFileExtensions = new string[] {"data", "properties", "script", "backup"};
				string currentDataBaseName = SQLManager.Instance.DataBase != null && SQLManager.Instance.DataBase is HSQLDB ? ((HSQLDB)SQLManager.Instance.DataBase).DataBaseFileName : null;

				// check for HSQLDB type database
				if (string.ReferenceEquals(currentDataBaseName, null))
				{
					return;
				}

				File selectedFile = DefaultFileChooser.showSaveDialog(JAG3D.Stage, i18n.getString("UIMenuBuilder.filechooser.copy.title", "Create deep copy of current project"), null, OADBReader.ExtensionFilters);

				if (selectedFile == null)
				{
					return;
				}

				string regex = "(.+?)(\\.)(backup$|data$|properties$|script$)";
				string copyDataBaseName = selectedFile.getAbsolutePath().replaceAll(regex, "$1");

				// check if selected file equals to loaded file
				if (currentDataBaseName.Equals(copyDataBaseName))
				{
					return;
				}

				// close current database
				SQLManager.Instance.closeDataBase();

				// copy files
				for (int i = 0; i < dataBaseFileExtensions.Length; i++)
				{
					Path src = Paths.get(currentDataBaseName + "." + dataBaseFileExtensions[i]);
					if (!Files.exists(src, LinkOption.NOFOLLOW_LINKS))
					{
						continue;
					}

					Path trg = Paths.get(copyDataBaseName + "." + dataBaseFileExtensions[i]);
					Files.copy(src, trg, StandardCopyOption.REPLACE_EXISTING);
				}

				// open copied database
				if (Files.exists(selectedFile.toPath(), LinkOption.NOFOLLOW_LINKS) && Files.isRegularFile(selectedFile.toPath(), LinkOption.NOFOLLOW_LINKS))
				{
					this.openProject(selectedFile);
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.copy.exception.title", "I/O Error"), i18n.getString("UIMenuBuilder.message.error.copy.exception.header", "Error, could not create a deep copy of the current database."), i18n.getString("UIMenuBuilder.message.error.copy.exception.message", "An exception has occurred during project copying."), e);
			}
		}

		internal virtual void openProject()
		{
			File selectedFile = DefaultFileChooser.showOpenDialog(JAG3D.Stage, i18n.getString("UIMenuBuilder.filechooser.open.title", "Open existing project"), null, OADBReader.ExtensionFilters);

			if (selectedFile != null)
			{
				this.openProject(selectedFile);
			}
		}

		internal virtual void openProject(File selectedFile)
		{
			try
			{
				if (selectedFile != null)
				{
					Path path = selectedFile.toPath();
					string regex = "(?i)(.+?)(\\.)(backup$|data$|properties$|script$)";
					string dataBaseName = Files.exists(path, LinkOption.NOFOLLOW_LINKS) ? path.toAbsolutePath().ToString().replaceFirst(regex, "$1") : null;

					string[] dataBaseFileExtensions = new string[] {"data", "properties", "script"};
					// check for complete db-files
					for (int i = 0; i < dataBaseFileExtensions.Length; i++)
					{
						if (!string.ReferenceEquals(dataBaseName, null))
						{
							Path p = Paths.get(dataBaseName + "." + dataBaseFileExtensions[i]);
							if (!Files.exists(p, LinkOption.NOFOLLOW_LINKS))
							{
								dataBaseName = null;
								throw new FileNotFoundException("Incomplete data base detected. Could not found " + p.ToString() + ".");
							}
						}
					}

					if (!string.ReferenceEquals(dataBaseName, null))
					{
						SQLManager.openExistingProject(new HSQLDB(dataBaseName));
						JAG3D.Title = path.getFileName() == null ? null : path.getFileName().ToString().replaceFirst(regex, "$1");
						this.addHistoryFile(selectedFile);
						this.historyMenu.getParentMenu().hide();
						DefaultFileChooser.LastSelectedDirectory = selectedFile;
					}
					else
					{
						throw new FileNotFoundException(selectedFile.ToString());
					}
				}
			}
			catch (DatabaseVersionMismatchException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.version.exception.title", "Version error"), i18n.getString("UIMenuBuilder.message.error.version.exception.header", "Error, database version of the stored project is greater\r\nthan accepted database version of the application."), i18n.getString("UIMenuBuilder.message.error.version.exception.message", "An exception has occurred during project opening."), e);
				SQLManager.Instance.closeDataBase();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.open.exception.title", "I/O Error"), i18n.getString("UIMenuBuilder.message.error.open.exception.header", "Error, could not open selected project."), i18n.getString("UIMenuBuilder.message.error.open.exception.message", "An exception has occurred during project opening."), e);
				SQLManager.Instance.closeDataBase();
			}
		}

		public virtual void migrateProject()
		{
			File selectedFile = DefaultFileChooser.showOpenDialog(JAG3D.Stage, i18n.getString("UIMenuBuilder.filechooser.migrate.title", "Migrate existing JAG3D project"), null, OADBReader.ExtensionFilters);

			if (selectedFile == null)
			{
				return;
			}

			try
			{
				Path path = selectedFile.toPath();
				string regex = "(?i)(.+?)(\\.)(backup$|data$|properties$|script$)";
				string project = Files.exists(path, LinkOption.NOFOLLOW_LINKS) ? path.toAbsolutePath().ToString().replaceFirst(regex, "$1") : null;

				if (!string.ReferenceEquals(project, null))
				{
					DataBase dataBase = new HSQLDB(project);
					if ((SQLManager.Instance.DataBase.getURI().Equals(dataBase.URI)))
					{
						throw new IOException(this.GetType().Name + " : Error, cannot re-import data to the same database!");
					}
					if (!OADBReader.isCurrentOADBVersion(dataBase))
					{
						throw new DatabaseVersionMismatchException(this.GetType().Name + ": Error, database version of the stored project is unequal to the required version of the application!");
					}

					SourceFileReader<TreeItem<TreeItemValue>> fileReader = new OADBReader(dataBase);
					TreeItem<TreeItemValue> lastItem = fileReader.readAndImport();

					if (lastItem != null)
					{
						TreeView<TreeItemValue> treeView = UITreeBuilder.Instance.Tree;
						treeView.getSelectionModel().clearSelection();
						treeView.getSelectionModel().select(lastItem);
						int index = treeView.getRow(lastItem);
						treeView.scrollTo(index);
					}
				}
				else
				{
					throw new FileNotFoundException(selectedFile.ToString());
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.migrate.exception.title", "I/O Error"), i18n.getString("UIMenuBuilder.message.error.migrate.exception.header", "Error, could not migrate selected project."), i18n.getString("UIMenuBuilder.message.error.migrate.exception.message", "An exception has occurred during project migrating."), e);
			}
		}

		private void importFile(SourceFileReader<TreeItem<TreeItemValue>> fileReader, ExtensionFilter[] extensionFilters, string title)
		{
			IList<File> selectedFiles = DefaultFileChooser.showOpenMultipleDialog(JAG3D.Stage, title, null, extensionFilters);

			if (selectedFiles == null || selectedFiles.Count == 0)
			{
				return;
			}

			this.importFile(fileReader, selectedFiles);
		}

		private void importFile(SourceFileReader<TreeItem<TreeItemValue>> fileReader, IList<File> selectedFiles)
		{
			if (selectedFiles == null || selectedFiles.Count == 0)
			{
				return;
			}

			TreeItem<TreeItemValue> lastItem = null;
			foreach (File file in selectedFiles)
			{
				fileReader.Path = file.toPath();
				try
				{
					lastItem = fileReader.readAndImport();
				}
				catch (Exception e)
				{ //IOException | SQLException
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.import.exception.title", "I/O Error"), i18n.getString("UIMenuBuilder.message.error.import.exception.header", "Error, could not import selected file."), i18n.getString("UIMenuBuilder.message.error.import.exception.message", "An exception has occurred during file import."), e);
				}
			}

			if (lastItem != null)
			{
				TreeView<TreeItemValue> treeView = UITreeBuilder.Instance.Tree;
				treeView.getSelectionModel().clearSelection();
				treeView.getSelectionModel().select(lastItem);
				int index = treeView.getRow(lastItem);
				treeView.scrollTo(index);
			}
		}

		internal virtual void importFile(MenuItemType menuItemType)
		{
			switch (menuItemType)
			{
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_REFERENCE_POINT_1D:
				this.importFile(new PointFlatFileReader(PointType.REFERENCE_POINT, 1), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.reference.1d.title", "Import 1D reference point data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_REFERENCE_POINT_2D:
				this.importFile(new PointFlatFileReader(PointType.REFERENCE_POINT, 2), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.reference.2d.title", "Import 2D reference point data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_REFERENCE_POINT_3D:
				this.importFile(new PointFlatFileReader(PointType.REFERENCE_POINT, 3), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.reference.3d.title", "Import 3D reference point data from flat files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_STOCHASTIC_POINT_1D:
				this.importFile(new PointFlatFileReader(PointType.STOCHASTIC_POINT, 1), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.stochastic.1d.title", "Import 1D stochastic point data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_STOCHASTIC_POINT_2D:
				this.importFile(new PointFlatFileReader(PointType.STOCHASTIC_POINT, 2), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.stochastic.2d.title", "Import 2D stochastic point data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_STOCHASTIC_POINT_3D:
				this.importFile(new PointFlatFileReader(PointType.STOCHASTIC_POINT, 3), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.stochastic.3d.title", "Import 3D stochastic point data from flat files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_DATUM_POINT_1D:
				this.importFile(new PointFlatFileReader(PointType.DATUM_POINT, 1), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.datum.1d.title", "Import 1D datum point data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_DATUM_POINT_2D:
				this.importFile(new PointFlatFileReader(PointType.DATUM_POINT, 2), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.datum.2d.title", "Import 2D datum point data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_DATUM_POINT_3D:
				this.importFile(new PointFlatFileReader(PointType.DATUM_POINT, 3), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.datum.3d.title", "Import 3D datum point data from flat files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_NEW_POINT_1D:
				this.importFile(new PointFlatFileReader(PointType.NEW_POINT, 1), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.new.1d.title", "Import 1D new point data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_NEW_POINT_2D:
				this.importFile(new PointFlatFileReader(PointType.NEW_POINT, 2), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.new.2d.title", "Import 2D new point data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_NEW_POINT_3D:
				this.importFile(new PointFlatFileReader(PointType.NEW_POINT, 3), PointFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.point.new.3d.title", "Import 3D new point data from flat files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_REFERENCE_VERTICAL_DEFLECTION:
				this.importFile(new VerticalDeflectionFlatFileReader(VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION), VerticalDeflectionFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.deflection.reference.title", "Import reference vertical deflection data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_STOCASTIC_VERTICAL_DEFLECTION:
				this.importFile(new VerticalDeflectionFlatFileReader(VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION), VerticalDeflectionFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.deflection.stochastic.title", "Import stochastic vertical deflection data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_UNKNOWN_VERTICAL_DEFLECTION:
				this.importFile(new VerticalDeflectionFlatFileReader(VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION), VerticalDeflectionFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.deflection.unknown.title", "Import approximated vertical deflection data from flat files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_LEVELING:
				this.importFile(new ObservationFlatFileReader(ObservationType.LEVELING), ObservationFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.leveling.title", "Import leveling data from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_DIRECTION:
				this.importFile(new ObservationFlatFileReader(ObservationType.DIRECTION), ObservationFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.direction.title", "Import direction sets from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_HORIZONTAL_DISTANCE:
				this.importFile(new ObservationFlatFileReader(ObservationType.HORIZONTAL_DISTANCE), ObservationFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.horizontal_distances.title", "Import horizontal distances from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_SLOPE_DISTANCE:
				this.importFile(new ObservationFlatFileReader(ObservationType.SLOPE_DISTANCE), ObservationFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.slope_distances.title", "Import slope distances from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_ZENITH_ANGLE:
				this.importFile(new ObservationFlatFileReader(ObservationType.ZENITH_ANGLE), ObservationFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.zenith_angles.title", "Import zenith angles from flat files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_GNSS1D:
				this.importFile(new ObservationFlatFileReader(ObservationType.GNSS1D), ObservationFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.gnss.1d.title", "Import 1D gnss baselines from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_GNSS2D:
				this.importFile(new ObservationFlatFileReader(ObservationType.GNSS2D), ObservationFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.gnss.2d.title", "Import 2D gnss baselines from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_GNSS3D:
				this.importFile(new ObservationFlatFileReader(ObservationType.GNSS3D), ObservationFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.gnss.3d.title", "Import 3D gnss baselines from flat files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_CONGRUENCE_ANALYSIS_PAIR_1D:
				this.importFile(new CongruenceAnalysisFlatFileReader(1), CongruenceAnalysisFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.congruence_analysis.1d.title", "Import congruence analysis point nexus 1D from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_CONGRUENCE_ANALYSIS_PAIR_2D:
				this.importFile(new CongruenceAnalysisFlatFileReader(2), CongruenceAnalysisFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.congruence_analysis.2d.title", "Import congruence analysis point nexus 2D from flat files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_FLAT_CONGRUENCE_ANALYSIS_PAIR_3D:
				this.importFile(new CongruenceAnalysisFlatFileReader(3), CongruenceAnalysisFlatFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.flat.congruence_analysis.3d.title", "Import congruence analysis point nexus 3D from flat files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_BEO:
				this.importFile(new BeoFileReader(), BeoFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.beo.title", "Import data from BEO files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_DL100:
				this.importFile(new DL100FileReader(), DL100FileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.dl100.title", "Import data from DL-100 files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GSI1D:
				this.importFile(new GSIFileReader(DimensionType.HEIGHT), GSIFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.gsi.1d.title", "Import 1D data from GSI files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GSI2D:
				this.importFile(new GSIFileReader(DimensionType.PLAN), GSIFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.gsi.2d.title", "Import 2D data from GSI files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GSI2DH:
				this.importFile(new GSIFileReader(DimensionType.PLAN_AND_HEIGHT), GSIFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.gsi.2dh.title", "Import 2D+H data from GSI files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GSI3D:
				this.importFile(new GSIFileReader(DimensionType.SPATIAL), GSIFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.gsi.3d.title", "Import 3D data from GSI files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_JOB_XML2D:
				this.importFile(new JobXMLFileReader(DimensionType.PLAN), JobXMLFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.jobxml.2d.title", "Import 2D data from JobXML files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_JOB_XML2DH:
				this.importFile(new JobXMLFileReader(DimensionType.PLAN_AND_HEIGHT), JobXMLFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.jobxml.2dh.title", "Import 2D+H data from JobXML files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_JOB_XML3D:
				this.importFile(new JobXMLFileReader(DimensionType.SPATIAL), JobXMLFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.jobxml.3d.title", "Import 3D data from JobXML files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_LAND_XML2D:
				this.importFile(new HeXMLFileReader(DimensionType.PLAN), HeXMLFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.landxml.2d.title", "Import 2D data from LandXML/HeXML files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_LAND_XML3D:
				this.importFile(new HeXMLFileReader(DimensionType.SPATIAL), HeXMLFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.landxml.3d.title", "Import 3D data from LandXML/HeXML files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_M5:
				this.importFile(new M5FileReader(), M5FileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.m5.title", "Import data from M5 files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_GKA:
				this.importFile(new GKAFileReader(), GKAFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.gka.title", "Import data from GKA files"));
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_Z:
				this.importFile(new ZFileReader(), ZFileReader.ExtensionFilters, i18n.getString("UIMenuBuilder.filechooser.import.z.title", "Import data from Z files"));
				break;

			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.IMPORT_COLUMN_BASED_FILES:
				IList<File> selectedFiles = DefaultFileChooser.showOpenMultipleDialog(JAG3D.Stage, i18n.getString("UIMenuBuilder.filechooser.import.column_based.title", "Import user-defined column-based flat files"), null, FlatFileReader.getExtensionFilters());

				if (selectedFiles == null || selectedFiles.Count == 0)
				{
					return;
				}

				Optional<SourceFileReader<TreeItem<TreeItemValue>>> optional = ColumnImportDialog.showAndWait(selectedFiles[0]);
				if (optional.isPresent() && optional.get() != null)
				{
					SourceFileReader<TreeItem<TreeItemValue>> fileReader = optional.get();
					this.importFile(fileReader, selectedFiles);
				}
				break;

			default:
				Console.Error.WriteLine(this.GetType().Name + " Error, unknwon import-type: " + menuItemType);
				break;
			}
		}

		internal virtual void embedSQLSripts()
		{
			IList<File> selectedFiles = DefaultFileChooser.showOpenMultipleDialog(JAG3D.Stage, i18n.getString("UIMenuBuilder.filechooser.sql.title", "Execute SQL script"), null, new ExtensionFilter(I18N.Instance.getString("UIMenuBuilder.extension.sql", "Structured Query Language"), "*.sql", "*.SQL"));

			if (selectedFiles == null || selectedFiles.Count == 0)
			{
				return;
			}

			try
			{
				DataBase dataBase = SQLManager.Instance.DataBase;
				dataBase.executeFiles(selectedFiles);
				OptionDialog.showInformationDialog(i18n.getString("UIMenuBuilder.message.information.sql.title", "SQL Script executed"), i18n.getString("UIMenuBuilder.message.information.sql.header", "SQL script successfully executed."), i18n.getString("UIMenuBuilder.message.information.sql.message", "The selected SQL script was executed without errors."));
			}
			catch (Exception e)
			{ //IOException | SQLException
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.sql.exception.title", "SQL Error"), i18n.getString("UIMenuBuilder.message.error.sql.exception.header", "Error, could not embed SQL script."), i18n.getString("UIMenuBuilder.message.error.sql.exception.message", "An exception has occurred during SQL embedding."), e);
			}

			TreeView<TreeItemValue> treeView = UITreeBuilder.Instance.Tree;
			treeView.getSelectionModel().clearSelection();
			treeView.getSelectionModel().select(0);
		}

		internal virtual void createReport(File templateFile)
		{
			try
			{
				Pattern pattern = Pattern.compile(".*?\\.(\\w+)\\.ftlh$", Pattern.CASE_INSENSITIVE);
				Matcher matcher = pattern.matcher(templateFile.getName().ToLower());
				string extension = "html";
				ExtensionFilter extensionFilter = new ExtensionFilter(i18n.getString("UIMenuBuilder.extension.html", "Hypertext Markup Language"), "*.html", "*.htm", "*.HTML", "*.HTM");
				if (matcher.find() && matcher.groupCount() == 1)
				{
					extension = matcher.group(1);
					extensionFilter = new ExtensionFilter(String.format(Locale.ENGLISH, i18n.getString("UIMenuBuilder.extension.template", "%s-File"), extension), "*." + extension);
				}

				string fileNameSuggestion = "report." + extension;

				FTLReport ftl = SQLManager.Instance.FTLReport;
				File reportFile = DefaultFileChooser.showSaveDialog(JAG3D.Stage, i18n.getString("UIMenuBuilder.filechooser.report.title", "Save adjustment report"), fileNameSuggestion, extensionFilter);
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

		public virtual void checkUpdates()
		{
			const string address = "https://software.applied-geodesy.org/update.php"; //"https://software.applied-geodesy.org/update.php";
			URLParameter param = new URLParameter("checkupdate", "jag3d");
			try
			{
				bool validProperties = false;
				Properties properties = HTTPPropertiesLoader.getProperties(address, param);
				if (properties != null && properties.containsKey("VERSION"))
				{ // VERSION
					int propVersion = int.Parse(properties.getProperty("VERSION", "-1"));

					if (propVersion > 0)
					{
						validProperties = true;
						if (propVersion > Math.Max(org.applied_geodesy.version.jag3d.Version.get(), org.applied_geodesy.version.juniform.Version.get()))
						{
							Optional<ButtonType> result = OptionDialog.showConfirmationDialog(i18n.getString("UIMenuBuilder.message.confirmation.outdated_version.title", "New version available"), String.format(Locale.ENGLISH, i18n.getString("UIMenuBuilder.message.confirmation.outdated_version.header", "A new version v%d of JAG3D is available.\r\nDo you want to download the latest release?"), propVersion), i18n.getString("UIMenuBuilder.message.confirmation.outdated_version.message", "The currently used application is outdated. A new version of JAG3D is available at <software.applied-geodesy.org>."));


							if (result.get() == ButtonType.OK && this.hostServices != null)
							{
								this.hostServices.showDocument(properties.getProperty("DOWNLOAD", "https://software.applied-geodesy.org"));
							}

						}
						else
						{
							OptionDialog.showInformationDialog(i18n.getString("UIMenuBuilder.message.information.latest_version.title", "JAG3D is up-to-date"), i18n.getString("UIMenuBuilder.message.information.latest_version.header", "You are using the latest version of JAG3D."), i18n.getString("UIMenuBuilder.message.information.latest_version.message", "No update found, JAG3D software package is up-to-date."));
						}
					}
				}

				if (!validProperties)
				{
					throw new IOException("Error, could not connect to server to check for updates. Received properties are invalid. " + properties);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.check_updates.exception.title", "I/O Error"), i18n.getString("UIMenuBuilder.message.error.check_updates.exception.header", "Error, could not connect to server to check for updates."), i18n.getString("UIMenuBuilder.message.error.check_updates.exception.message", "An exception has occurred during check for updates."), e);
			}
		}

		internal virtual void showModule(MenuItemType menuItemType)
		{
			Platform.runLater(() =>
			{
			try
			{
				switch (menuItemType)
				{
				case org.applied_geodesy.jag3d.ui.menu.MenuItemType.MODULE_JUNIFORM:
					Stage stage = JUniForm.Stage;
					if (stage == null)
					{
						stage = new Stage();
						stage.initOwner(JAG3D.Stage);
					}
					JUniForm juniform = new JUniForm();
					juniform.start(stage);
					break;
				default:
					break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				OptionDialog.showThrowableDialog(i18n.getString("UIMenuBuilder.message.error.module.exception.title", "Application error"), i18n.getString("UIMenuBuilder.message.error.module.exception.header", "Error, could not start application."), i18n.getString("UIMenuBuilder.message.error.module.exception.message", "An exception has occurred during application start."), e);
			}
			});
		}

		// TODO transfer application to FX
		internal virtual void showSwingApplication(MenuItemType menuItemType)
		{
			SwingUtilities.invokeLater(() =>
			{
			switch (menuItemType)
			{
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.MODULE_COORDTRANS:
				new CoordTrans(true);
				break;
			case org.applied_geodesy.jag3d.ui.menu.MenuItemType.MODULE_GEOTRA:
				new GeoTra(true);
				break;
			default:
				break;
			}
			});
		}
	}
}