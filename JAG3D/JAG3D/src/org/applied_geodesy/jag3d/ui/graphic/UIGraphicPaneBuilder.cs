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

namespace org.applied_geodesy.jag3d.ui.graphic
{
	using LayerManager = org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager;

	using Insets = javafx.geometry.Insets;
	using Label = javafx.scene.control.Label;
	using Background = javafx.scene.layout.Background;
	using BackgroundFill = javafx.scene.layout.BackgroundFill;
	using BorderPane = javafx.scene.layout.BorderPane;
	using CornerRadii = javafx.scene.layout.CornerRadii;
	using HBox = javafx.scene.layout.HBox;
	using Pane = javafx.scene.layout.Pane;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using Color = javafx.scene.paint.Color;
	using Font = javafx.scene.text.Font;

	public class UIGraphicPaneBuilder
	{
		private static UIGraphicPaneBuilder graphicPaneBuilder = new UIGraphicPaneBuilder();
		private BorderPane borderPane = null;
		private readonly LayerManager layerManager = new LayerManager();

		private UIGraphicPaneBuilder()
		{
		}

		public static UIGraphicPaneBuilder Instance
		{
			get
			{
				return graphicPaneBuilder;
			}
		}

		public virtual Pane Pane
		{
			get
			{
				this.init();
				return this.borderPane;
			}
		}

		private void init()
		{
			if (this.borderPane != null)
			{
				return;
			}

			BackgroundFill fill = new BackgroundFill(Color.rgb(255, 255, 255, 1.0), CornerRadii.EMPTY, Insets.EMPTY); //0-255
			Background background = new Background(fill);

			BorderPane plotBackgroundPane = new BorderPane();
			plotBackgroundPane.setMinSize(0, 0);
			plotBackgroundPane.setMaxSize(double.MaxValue, double.MaxValue);
			plotBackgroundPane.setBackground(background);
			plotBackgroundPane.setCenter(this.layerManager.Pane);

			this.borderPane = new BorderPane();

			this.borderPane.setMinSize(0, 0);
			this.borderPane.setMaxSize(double.MaxValue, double.MaxValue);

			this.borderPane.setTop(this.layerManager.ToolBar);
			this.borderPane.setCenter(plotBackgroundPane); // this.layerManager.getPane()

			Label coordinateLabel = this.layerManager.CoordinateLabel;
			coordinateLabel.setFont(new Font(10));
			coordinateLabel.setPadding(new Insets(10, 10, 10, 10));

			Region spacer = new Region();
			HBox hbox = new HBox(10);
			HBox.setHgrow(spacer, Priority.ALWAYS);
			hbox.getChildren().addAll(spacer, coordinateLabel);

			this.borderPane.setBottom(hbox);
		}

		public virtual LayerManager LayerManager
		{
			get
			{
				return this.layerManager;
			}
		}
	}

}