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

namespace org.applied_geodesy.juniform.ui.dialog
{

	using Version = org.applied_geodesy.version.juniform.Version;
	using VersionType = org.applied_geodesy.version.VersionType;

	using HostServices = javafx.application.HostServices;
	using Platform = javafx.application.Platform;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using HPos = javafx.geometry.HPos;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Hyperlink = javafx.scene.control.Hyperlink;
	using Label = javafx.scene.control.Label;
	using ScrollPane = javafx.scene.control.ScrollPane;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Font = javafx.scene.text.Font;
	using FontPosture = javafx.scene.text.FontPosture;
	using FontWeight = javafx.scene.text.FontWeight;
	using Text = javafx.scene.text.Text;
	using TextAlignment = javafx.scene.text.TextAlignment;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;

	public class AboutDialog
	{
		private static AboutDialog aboutDialog = new AboutDialog();
		private Dialog<Void> dialog = null;
		private Window window;
		private HostServices hostServices;
		private AboutDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				aboutDialog.window = value;
			}
		}

		public static HostServices HostServices
		{
			set
			{
				aboutDialog.hostServices = value;
			}
		}

		public static Optional<Void> showAndWait()
		{
			aboutDialog.init();
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				aboutDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) aboutDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return aboutDialog.dialog.showAndWait();
		}


		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<Void>();
			this.dialog.setTitle("JUniForm");
			this.dialog.setHeaderText("JUniForm \u2014 Java\u00B7Unified\u00B7Form\u00B7Fitting" + (Version.ReleaseCandidate ? " (RC)" : ""));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.CLOSE);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
		}

		private Node createPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMaxWidth(double.MaxValue);
			gridPane.setHgap(15);
			gridPane.setVgap(10);
			gridPane.setAlignment(Pos.CENTER);
			gridPane.setPadding(new Insets(7,7,7,7)); // oben, recht, unten, links
	//		gridPane.setGridLinesVisible(true);

			Text applicationText = new Text("Least-Squares Analysis of Curves and Surfaces");
			applicationText.setFont(Font.font("SansSerif", FontWeight.NORMAL, FontPosture.REGULAR, 14));
			applicationText.setTextAlignment(TextAlignment.CENTER);

			// left-hand labels
			Label authorLabel = this.createLabel("Author:");
			Label licenceLabel = this.createLabel("Licence:");
			Label uiVersionLabel = this.createLabel("UI version:");
			Label coreVersionLabel = this.createLabel("AC version:");
			Label homePageLabel = this.createLabel("Homepage:");
			Label thirdPartyLabel = this.createLabel("3rd Party Libraries:");

			// right-hand labels
			Label author = this.createLabel("Michael L\u00F6sler\r\n\r\nFriedberger Str. 50D\r\nDE-61118 Bad Vilbel");
			Label licence = this.createLabel("GNU General Public License v3.0");
			Label uiVersion = this.createLabel("v" + Version.get(VersionType.USER_INTERFACE));
			Label coreVersion = this.createLabel("v" + Version.get(VersionType.ADJUSTMENT_CORE));

			Hyperlink homePageLink = new Hyperlink("software.applied-geodesy.org");
			homePageLink.setTooltip(new Tooltip("Go to software.applied-geodesy.org"));
			homePageLink.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			homePageLink.setPadding(new Insets(0));
			homePageLink.setVisited(false);
			homePageLink.setOnAction(new EventHandlerAnonymousInnerClass(this, homePageLink));

			Label thirdParty = this.createLabel("" + "\u2219 MTJ: GNU Lesser General Public License v3.0\r\n" + "\u2219 FreeMarker: Apache License v2.0\r\n" + "\u2219 netlib-java: BSD License\r\n" + "\u2219 JLAPACK: BSD License\r\n" + "\u2219 JDistlib: GNU General Public License v2.0\r\n" + "\u2219 JLaTeXMath: GNU General Public License v2.0\r\n" + "\u2219 Launch4J: BSD/MIT License");

			authorLabel.setLabelFor(author);
			licenceLabel.setLabelFor(licence);
			uiVersionLabel.setLabelFor(uiVersion);
			coreVersionLabel.setLabelFor(coreVersion);
			homePageLabel.setLabelFor(homePageLink);
			thirdPartyLabel.setLabelFor(thirdParty);

			GridPane.setHalignment(applicationText, HPos.CENTER);
			int row = 0;
			gridPane.add(applicationText, 0, row++, 2, 1);

			gridPane.add(authorLabel, 0, row);
			gridPane.add(author, 1, row++);

			gridPane.add(licenceLabel, 0, row);
			gridPane.add(licence, 1, row++);

			gridPane.add(uiVersionLabel, 0, row);
			gridPane.add(uiVersion, 1, row++);

			gridPane.add(coreVersionLabel, 0, row);
			gridPane.add(coreVersion, 1, row++);

			gridPane.add(homePageLabel, 0, row);
			gridPane.add(homePageLink, 1, row++);

			gridPane.add(thirdPartyLabel, 0, row);
			gridPane.add(thirdParty, 1, row++);

			ScrollPane scroller = new ScrollPane(gridPane);
			scroller.setPadding(new Insets(10, 10, 10, 10));
			scroller.setFitToHeight(true);
			scroller.setFitToWidth(true);

			return gridPane;
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<ActionEvent>
		{
			private readonly AboutDialog outerInstance;

			private Hyperlink homePageLink;

			public EventHandlerAnonymousInnerClass(AboutDialog outerInstance, Hyperlink homePageLink)
			{
				this.outerInstance = outerInstance;
				this.homePageLink = homePageLink;
			}

			public override void handle(ActionEvent e)
			{
				if (outerInstance.hostServices != null)
				{
					outerInstance.hostServices.showDocument("https://software.applied-geodesy.org");
				}
				homePageLink.setVisited(false);
			}
		}

		private Label createLabel(string text)
		{
			Label label = new Label(text);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(Control.USE_PREF_SIZE, double.MaxValue);
			label.setAlignment(Pos.TOP_LEFT);
			return label;
		}
	}

}