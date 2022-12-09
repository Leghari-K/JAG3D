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

namespace org.applied_geodesy.juniform.ui
{

	using FeatureAdjustment = org.applied_geodesy.adjustment.geometry.FeatureAdjustment;
	using FeatureChangeListener = org.applied_geodesy.adjustment.geometry.FeatureChangeListener;
	using FeatureEvent = org.applied_geodesy.adjustment.geometry.FeatureEvent;
	using FeatureEventType = org.applied_geodesy.adjustment.geometry.FeatureEvent.FeatureEventType;
	using JAG3D = org.applied_geodesy.jag3d.ui.JAG3D;
	using FTLReport = org.applied_geodesy.juniform.io.report.FTLReport;
	using AboutDialog = org.applied_geodesy.juniform.ui.dialog.AboutDialog;
	using AverageRestrictionDialog = org.applied_geodesy.juniform.ui.dialog.AverageRestrictionDialog;
	using FeatureAdjustmentDialog = org.applied_geodesy.juniform.ui.dialog.FeatureAdjustmentDialog;
	using FeatureDialog = org.applied_geodesy.juniform.ui.dialog.FeatureDialog;
	using FeaturePointRestrictionDialog = org.applied_geodesy.juniform.ui.dialog.FeaturePointRestrictionDialog;
	using FormatterOptionDialog = org.applied_geodesy.juniform.ui.dialog.FormatterOptionDialog;
	using GeometricPrimitiveDialog = org.applied_geodesy.juniform.ui.dialog.GeometricPrimitiveDialog;
	using LeastSquaresSettingDialog = org.applied_geodesy.juniform.ui.dialog.LeastSquaresSettingDialog;
	using MatrixDialog = org.applied_geodesy.juniform.ui.dialog.MatrixDialog;
	using ProductSumRestrictionDialog = org.applied_geodesy.juniform.ui.dialog.ProductSumRestrictionDialog;
	using org.applied_geodesy.juniform.ui.dialog;
	using RestrictionDialog = org.applied_geodesy.juniform.ui.dialog.RestrictionDialog;
	using RestrictionTypeDialog = org.applied_geodesy.juniform.ui.dialog.RestrictionTypeDialog;
	using TestStatisticDialog = org.applied_geodesy.juniform.ui.dialog.TestStatisticDialog;
	using TrigonometricRestrictionDialog = org.applied_geodesy.juniform.ui.dialog.TrigonometricRestrictionDialog;
	using UnknownParameterDialog = org.applied_geodesy.juniform.ui.dialog.UnknownParameterDialog;
	using UnknownParameterTypeDialog = org.applied_geodesy.juniform.ui.dialog.UnknownParameterTypeDialog;
	using VectorAngleRestrictionDialog = org.applied_geodesy.juniform.ui.dialog.VectorAngleRestrictionDialog;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;
	using UIMenuBuilder = org.applied_geodesy.juniform.ui.menu.UIMenuBuilder;
	using UIParameterTableBuilder = org.applied_geodesy.juniform.ui.table.UIParameterTableBuilder;
	using UIPointTableBuilder = org.applied_geodesy.juniform.ui.table.UIPointTableBuilder;
	using UITabPaneBuilder = org.applied_geodesy.juniform.ui.tabpane.UITabPaneBuilder;
	using org.applied_geodesy.juniform.ui.tree;
	using UITreeBuilder = org.applied_geodesy.juniform.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using ImageUtils = org.applied_geodesy.util.ImageUtils;
	using Version = org.applied_geodesy.version.juniform.Version;

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
	using BorderPane = javafx.scene.layout.BorderPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using Color = javafx.scene.paint.Color;
	using Font = javafx.scene.text.Font;
	using FontWeight = javafx.scene.text.FontWeight;
	using Text = javafx.scene.text.Text;
	using Stage = javafx.stage.Stage;

	public class JUniForm : Application
	{
		private class AdjustmentFeatureChangedListener : FeatureChangeListener
		{
			private readonly JUniForm outerInstance;

			public AdjustmentFeatureChangedListener(JUniForm outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void featureChanged(FeatureEvent evt)
			{
				outerInstance.adjustmentButton.setDisable(evt.EventType == FeatureEvent.FeatureEventType.FEATURE_REMOVED);
			}
		}

		private const string TITLE_TEMPLATE = "%s%sJUniForm%s \u00B7 Curves and Surfaces Fitting \u00B7";
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
			FTLReport.HostServices = hostServices;
			AboutDialog.HostServices = hostServices;
		}

		private Stage StageToDialogs
		{
			set
			{
				OptionDialog.Owner = value;
				TestStatisticDialog.Owner = value;
				UnknownParameterDialog.Owner = value;
				UnknownParameterTypeDialog.Owner = value;
				RestrictionDialog.Owner = value;
				RestrictionTypeDialog.Owner = value;
				AverageRestrictionDialog.Owner = value;
				ProductSumRestrictionDialog.Owner = value;
				FeatureDialog.Owner = value;
				GeometricPrimitiveDialog.Owner = value;
				MatrixDialog.Owner = value;
				FeatureAdjustmentDialog.Owner = value;
				LeastSquaresSettingDialog.Owner = value;
				ReadFileProgressDialog.setOwner(value);
				FormatterOptionDialog.Owner = value;
				AboutDialog.Owner = value;
				FeaturePointRestrictionDialog.Owner = value;
				VectorAngleRestrictionDialog.Owner = value;
				TrigonometricRestrictionDialog.Owner = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void start(javafx.stage.Stage primaryStage) throws Exception
		public override void start(Stage primaryStage)
		{
			if (JUniForm.primaryStage != primaryStage)
			{
				I18N i18n = I18N.Instance;

				JUniForm.primaryStage = primaryStage;

				UITabPaneBuilder tabPaneBuilder = UITabPaneBuilder.Instance;
				UIMenuBuilder menuBuilder = UIMenuBuilder.Instance;
				UITreeBuilder treeBuilder = UITreeBuilder.Instance;

				this.adjustmentButton = new Button(i18n.getString("JUniForm.button.adjust.label", "Adjust feature"));
				this.adjustmentButton.setTooltip(new Tooltip(i18n.getString("JUniForm.button.adjust.tooltip", "Start feature adjustment process")));
				this.adjustmentButton.setOnAction(new EventHandlerAnonymousInnerClass(this));
				this.adjustmentButton.setDisable(true);

				TabPane tabPane = tabPaneBuilder.TabPane;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.TreeView<org.applied_geodesy.juniform.ui.tree.TreeItemValue<?>> tree = treeBuilder.getTree();
				TreeView<TreeItemValue<object>> tree = treeBuilder.Tree;

				SplitPane splitPane = new SplitPane();
				splitPane.setOrientation(Orientation.HORIZONTAL);
				splitPane.getItems().addAll(tree, tabPane);
				splitPane.setDividerPositions(0.30);
				SplitPane.setResizableWithParent(tree, false);

				DropShadow ds = new DropShadow();
				ds.setOffsetY(0.5f);
				ds.setColor(Color.gray(0.8));

				Text applicationName = new Text();
				applicationName.setEffect(ds);
				applicationName.setCache(true);
				applicationName.setFill(Color.GREY);
				applicationName.setText("Java\u00B7Unified\u00B7Form\u00B7Fitting");
				applicationName.setFont(Font.font("SansSerif", FontWeight.NORMAL, 17));

				Region spacer = new Region();
				HBox hbox = new HBox(10);
				hbox.setPadding(new Insets(5, 10, 5, 15));
				HBox.setHgrow(spacer, Priority.ALWAYS);
				hbox.getChildren().addAll(applicationName, spacer, this.adjustmentButton);


				BorderPane border = new BorderPane();
				border.setPrefSize(900, 650);
				border.setTop(menuBuilder.MenuBar);
				border.setCenter(splitPane);
				border.setBottom(hbox);

				Scene scene = new Scene(border);

				// add external style definitions
				try
				{
					URL cssURL = null;
					if ((cssURL = typeof(JAG3D).getClassLoader().getResource("css/")) != null)
					{
						cssURL = cssURL.toURI().resolve("juniform.css").toURL();
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
					primaryStage.getIcons().addAll(ImageUtils.getImage("JUniForm_16x16.png"), ImageUtils.getImage("JUniForm_32x32.png"), ImageUtils.getImage("JUniForm_64x64.png"));
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				primaryStage.setScene(scene);

				Title = null;

				primaryStage.show();
				primaryStage.toFront();

				this.setHostServices();
				this.StageToDialogs = primaryStage;

				// add listener to UI components
				FeatureAdjustment adjustment = treeBuilder.FeatureAdjustment;
				adjustment.addFeatureChangeListener(treeBuilder);
				adjustment.addFeatureChangeListener(UIMenuBuilder.Instance);
				adjustment.addFeatureChangeListener(UIParameterTableBuilder.Instance);
				adjustment.addFeatureChangeListener(UIPointTableBuilder.Instance);
				adjustment.addFeatureChangeListener(new AdjustmentFeatureChangedListener(this));
			}
			else
			{
				primaryStage.show();
				primaryStage.toFront();
			}
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<ActionEvent>
		{
			private readonly JUniForm outerInstance;

			public EventHandlerAnonymousInnerClass(JUniForm outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				FeatureAdjustmentDialog.show();
			}
		}

		public static void Main(string[] args)
		{
			//Locale.setDefault(Locale.GERMAN);
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
			Application.launch(typeof(JUniForm), args);
		}
	}

}