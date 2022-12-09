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

namespace org.applied_geodesy.jag3d.ui
{

	using DefaultApplicationProperty = org.applied_geodesy.jag3d.DefaultApplicationProperty;
	using ProjectDatabaseStateChangeListener = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateChangeListener;
	using ProjectDatabaseStateEvent = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateEvent;
	using ProjectDatabaseStateType = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using AboutDialog = org.applied_geodesy.jag3d.ui.dialog.AboutDialog;
	using AnalysisChartsDialog = org.applied_geodesy.jag3d.ui.dialog.AnalysisChartsDialog;
	using ApproximationValuesDialog = org.applied_geodesy.jag3d.ui.dialog.ApproximationValuesDialog;
	using AverageDialog = org.applied_geodesy.jag3d.ui.dialog.AverageDialog;
	using ColumnImportDialog = org.applied_geodesy.jag3d.ui.dialog.ColumnImportDialog;
	using CongruentPointDialog = org.applied_geodesy.jag3d.ui.dialog.CongruentPointDialog;
	using FormatterOptionDialog = org.applied_geodesy.jag3d.ui.dialog.FormatterOptionDialog;
	using ImportOptionDialog = org.applied_geodesy.jag3d.ui.dialog.ImportOptionDialog;
	using InstrumentAndReflectorHeightAdaptionDialog = org.applied_geodesy.jag3d.ui.dialog.InstrumentAndReflectorHeightAdaptionDialog;
	using LeastSquaresSettingDialog = org.applied_geodesy.jag3d.ui.dialog.LeastSquaresSettingDialog;
	using NetworkAdjustmentDialog = org.applied_geodesy.jag3d.ui.dialog.NetworkAdjustmentDialog;
	using ProjectionAndReductionDialog = org.applied_geodesy.jag3d.ui.dialog.ProjectionAndReductionDialog;
	using RankDefectDialog = org.applied_geodesy.jag3d.ui.dialog.RankDefectDialog;
	using SearchAndReplaceDialog = org.applied_geodesy.jag3d.ui.dialog.SearchAndReplaceDialog;
	using TableRowHighlightDialog = org.applied_geodesy.jag3d.ui.dialog.TableRowHighlightDialog;
	using TestStatisticDialog = org.applied_geodesy.jag3d.ui.dialog.TestStatisticDialog;
	using UIGraphicPaneBuilder = org.applied_geodesy.jag3d.ui.graphic.UIGraphicPaneBuilder;
	using FeatureZoomDialog = org.applied_geodesy.jag3d.ui.graphic.layer.dialog.FeatureZoomDialog;
	using LayerManagerDialog = org.applied_geodesy.jag3d.ui.graphic.layer.dialog.LayerManagerDialog;
	using UIMenuBuilder = org.applied_geodesy.jag3d.ui.menu.UIMenuBuilder;
	using UITabPaneBuilder = org.applied_geodesy.jag3d.ui.tabpane.UITabPaneBuilder;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using ImageUtils = org.applied_geodesy.util.ImageUtils;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using HSQLDB = org.applied_geodesy.util.sql.HSQLDB;
	using Version = org.applied_geodesy.version.jag3d.Version;

	using Application = javafx.application.Application;
	using HostServices = javafx.application.HostServices;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Orientation = javafx.geometry.Orientation;
	using Scene = javafx.scene.Scene;
	using Button = javafx.scene.control.Button;
	using SplitPane = javafx.scene.control.SplitPane;
	using TabPane = javafx.scene.control.TabPane;
	using Tooltip = javafx.scene.control.Tooltip;
	using TreeView = javafx.scene.control.TreeView;
	using DropShadow = javafx.scene.effect.DropShadow;
	using KeyCode = javafx.scene.input.KeyCode;
	using KeyCodeCombination = javafx.scene.input.KeyCodeCombination;
	using KeyCombination = javafx.scene.input.KeyCombination;
	using KeyEvent = javafx.scene.input.KeyEvent;
	using BorderPane = javafx.scene.layout.BorderPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using Color = javafx.scene.paint.Color;
	using Font = javafx.scene.text.Font;
	using FontWeight = javafx.scene.text.FontWeight;
	using Text = javafx.scene.text.Text;
	using Stage = javafx.stage.Stage;

	public class JAG3D : Application
	{

		private class DatabaseStateChangeListener : ProjectDatabaseStateChangeListener
		{
			private readonly JAG3D outerInstance;

			public DatabaseStateChangeListener(JAG3D outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void projectDatabaseStateChanged(ProjectDatabaseStateEvent evt)
			{
				if (outerInstance.adjustmentButton != null)
				{
					bool disable = evt.EventType != ProjectDatabaseStateType.OPENED;
					outerInstance.adjustmentButton.setDisable(disable);

					if (evt.EventType == ProjectDatabaseStateType.CLOSING || evt.EventType == ProjectDatabaseStateType.CLOSED)
					{
						JAG3D.Title = null;
					}
				}
			}
		}

		private const string TITLE_TEMPLATE = "%s%sJAG3D%s \u00B7 Least-Squares Adjustment \u0026 Deformation Analysis \u00B7";
		private static Stage primaryStage;
		private Button adjustmentButton;

		public static string Title
		{
			set
			{
				if (primaryStage != null && !string.ReferenceEquals(value, null) && value.Trim().Length > 0)
				{
					primaryStage.setTitle(String.format(Locale.ENGLISH, TITLE_TEMPLATE, value, " \u2014 ", (Version.ReleaseCandidate ? " (RC)" : "")));
				}
				else if (primaryStage != null)
				{
					primaryStage.setTitle(String.format(Locale.ENGLISH, TITLE_TEMPLATE, "", "", (Version.ReleaseCandidate ? " (RC)" : "")));
				}
			}
		}

		public static void close()
		{
			primaryStage.close();
		}

		public static Stage Stage
		{
			get
			{
				return primaryStage;
			}
		}

		private void setHostServices()
		{
			HostServices hostServices = this.getHostServices();
			AboutDialog.HostServices = hostServices;
			SQLManager.HostServices = hostServices;
			UIMenuBuilder.HostServices = hostServices;
		}

		private Stage StageToDialogs
		{
			set
			{
				OptionDialog.Owner = value;
				NetworkAdjustmentDialog.Owner = value;
				FormatterOptionDialog.Owner = value;
				TestStatisticDialog.Owner = value;
				ProjectionAndReductionDialog.Owner = value;
				RankDefectDialog.Owner = value;
				CongruentPointDialog.Owner = value;
				AverageDialog.Owner = value;
				ApproximationValuesDialog.Owner = value;
				LeastSquaresSettingDialog.Owner = value;
				LayerManagerDialog.Owner = value;
				SearchAndReplaceDialog.Owner = value;
				AboutDialog.Owner = value;
				ColumnImportDialog.Owner = value;
				TableRowHighlightDialog.Owner = value;
				ImportOptionDialog.Owner = value;
				AnalysisChartsDialog.Owner = value;
				InstrumentAndReflectorHeightAdaptionDialog.Owner = value;
				FeatureZoomDialog.Owner = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void start(javafx.stage.Stage primaryStage) throws Exception
		public override void start(Stage primaryStage)
		{
			java.awt.SplashScreen splashScreen = null;
			JAG3D.primaryStage = primaryStage;

			try
			{
				try
				{
					splashScreen = java.awt.SplashScreen.getSplashScreen();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				I18N i18n = I18N.Instance;

				UIMenuBuilder menuBuilder = UIMenuBuilder.Instance;
				UITabPaneBuilder tabPaneBuilder = UITabPaneBuilder.Instance;
				UITreeBuilder treeBuilder = UITreeBuilder.Instance;

				TabPane tabPane = tabPaneBuilder.TabPane;
				TreeView<TreeItemValue> tree = treeBuilder.Tree;

				SplitPane splitPane = new SplitPane();
				splitPane.setOrientation(Orientation.HORIZONTAL);
				splitPane.getItems().addAll(tree, tabPane);
				splitPane.setDividerPositions(0.30);
				SplitPane.setResizableWithParent(tree, false);

				BorderPane border = new BorderPane();
				border.setPrefSize(900, 650);
				border.setTop(menuBuilder.MenuBar);
				border.setCenter(splitPane);

				UITreeBuilder.Instance.Tree.getSelectionModel().clearSelection();
				UITreeBuilder.Instance.Tree.getSelectionModel().selectFirst();

				this.adjustmentButton = new Button(i18n.getString("JavaGraticule3D.button.adjust.label", "Adjust network"));
				this.adjustmentButton.setTooltip(new Tooltip(i18n.getString("JavaGraticule3D.button.adjust.tooltip", "Start network adjustment process")));
				this.adjustmentButton.setOnAction(new EventHandlerAnonymousInnerClass(this));
				this.adjustmentButton.setDisable(true);

				DropShadow ds = new DropShadow();
				ds.setOffsetY(0.5f);
				ds.setColor(Color.gray(0.8));

				Text applicationName = new Text();
				applicationName.setEffect(ds);
				applicationName.setCache(true);
				applicationName.setFill(Color.GREY);
				applicationName.setText("Java\u00B7Applied\u00B7Geodesy\u00B73D");
				applicationName.setFont(Font.font("SansSerif", FontWeight.NORMAL, 17));

				Region spacer = new Region();
				HBox hbox = new HBox(10);
				hbox.setPadding(new Insets(5, 10, 5, 15));
				HBox.setHgrow(spacer, Priority.ALWAYS);
				hbox.getChildren().addAll(applicationName, spacer, this.adjustmentButton);
				border.setBottom(hbox);

				Scene scene = new Scene(border);

				scene.addEventFilter(KeyEvent.KEY_PRESSED, new EventHandlerAnonymousInnerClass2(this, tabPane));

				// add external style definitions
				try
				{
					URL cssURL = null;
					if ((cssURL = typeof(JAG3D).getClassLoader().getResource("css/")) != null)
					{
						cssURL = cssURL.toURI().resolve("jag3d.css").toURL();
						if (Files.exists(Paths.get(cssURL.toURI())))
						{
							scene.getStylesheets().add(cssURL.toExternalForm());
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

				// add icons
				try
				{
					primaryStage.getIcons().addAll(ImageUtils.getImage("JAG3D_16x16.png"), ImageUtils.getImage("JAG3D_32x32.png"), ImageUtils.getImage("JAG3D_64x64.png"));
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				primaryStage.setScene(scene);

				Title = null;

				primaryStage.show();
				primaryStage.setMaximized(DefaultApplicationProperty.startApplicationInFullScreen());
				primaryStage.toFront();

				this.StageToDialogs = primaryStage;
				this.setHostServices();
				SQLManager.Instance.addProjectDatabaseStateChangeListener(new DatabaseStateChangeListener(this));

				try
				{
					// check for command line arguments
					// --database=D:\\data\\project.script
					// --ut
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Parameters params = this.getParameters();
					Parameters @params = this.getParameters();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, String> parameterMap = params.getNamed();
					IDictionary<string, string> parameterMap = @params.getNamed();
					if (parameterMap.ContainsKey("database"))
					{
						Path path = Paths.get(parameterMap["database"]);
						string regex = "(?i)(.+?)(\\.)(backup$|data$|properties$|script$)";
						string project = Files.exists(path, LinkOption.NOFOLLOW_LINKS) ? path.toAbsolutePath().ToString().replaceFirst(regex, "$1") : null;

						if (!string.ReferenceEquals(project, null))
						{
							SQLManager.openExistingProject(new HSQLDB(project));
							JAG3D.Title = path.getFileName() == null ? null : path.getFileName().ToString().replaceFirst(regex, "$1");
						}
					}
					if (parameterMap.ContainsKey("ut") && parameterMap.GetOrDefault("ut", "FALSE").equalsIgnoreCase("TRUE"))
					{
						LeastSquaresSettingDialog.EnableUnscentedTransformation = true;
						org.applied_geodesy.juniform.ui.dialog.LeastSquaresSettingDialog.EnableUnscentedTransformation = true;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
			finally
			{
				if (splashScreen != null)
				{
					splashScreen.close();
				}
			}
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<ActionEvent>
		{
			private readonly JAG3D outerInstance;

			public EventHandlerAnonymousInnerClass(JAG3D outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				NetworkAdjustmentDialog.show();
			}
		}

		private class EventHandlerAnonymousInnerClass2 : EventHandler<KeyEvent>
		{
			private readonly JAG3D outerInstance;

			private TabPane tabPane;

			public EventHandlerAnonymousInnerClass2(JAG3D outerInstance, TabPane tabPane)
			{
				this.outerInstance = outerInstance;
				this.tabPane = tabPane;
				zoomInKeyComb = new KeyCodeCombination(KeyCode.PLUS, KeyCombination.CONTROL_DOWN);
				zoomOutKeyComb = new KeyCodeCombination(KeyCode.MINUS, KeyCombination.CONTROL_DOWN);
				zoomInKeyCombNum = new KeyCodeCombination(KeyCode.ADD, KeyCombination.CONTROL_DOWN);
				zoomOutKeyCombNum = new KeyCodeCombination(KeyCode.SUBTRACT, KeyCombination.CONTROL_DOWN);
			}

			internal readonly KeyCombination zoomInKeyComb;
			internal readonly KeyCombination zoomOutKeyComb;
			internal readonly KeyCombination zoomInKeyCombNum;
			internal readonly KeyCombination zoomOutKeyCombNum;
			public void handle(KeyEvent keyEvent)
			{
				if (keyEvent.getCode() == KeyCode.F5 && !outerInstance.adjustmentButton.isDisabled())
				{
					outerInstance.adjustmentButton.fire();
					keyEvent.consume();
				}
				else if ((zoomInKeyComb.match(keyEvent) || zoomInKeyCombNum.match(keyEvent)) && tabPane.getSelectionModel().getSelectedItem() != null && tabPane.getSelectionModel().getSelectedItem().getContent() == UIGraphicPaneBuilder.Instance.Pane)
				{
					UIGraphicPaneBuilder.Instance.LayerManager.zoomIn();
					keyEvent.consume();
				}
				else if ((zoomOutKeyComb.match(keyEvent) || zoomOutKeyCombNum.match(keyEvent)) && tabPane.getSelectionModel().getSelectedItem() != null && tabPane.getSelectionModel().getSelectedItem().getContent() == UIGraphicPaneBuilder.Instance.Pane)
				{
					UIGraphicPaneBuilder.Instance.LayerManager.zoomOut();
					keyEvent.consume();
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void stop() throws Exception
		public virtual void stop()
		{
			SQLManager.Instance.closeDataBase();
			base.stop();
		}

		public static void Main(string[] args)
		{
			//Locale.setDefault(Locale.ENGLISH);
			//Locale.setDefault(new Locale("sr", "BA"));
			try
			{
				Logger[] loggers = new Logger[] {Logger.getLogger("hsqldb.db"), Logger.getLogger("com.github.fommil.netlib.LAPACK"), Logger.getLogger("com.github.fommil.netlib.BLAS")};

				foreach (Logger logger in loggers)
				{
					logger.setUseParentHandlers(false);
					logger.setLevel(Level.OFF);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			Application.launch(typeof(JAG3D), args);
		}
	}

}