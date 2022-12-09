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

namespace org.applied_geodesy.jag3d.ui.graphic.layer.dialog
{
	using Layer = org.applied_geodesy.jag3d.ui.graphic.layer.Layer;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Insets = javafx.geometry.Insets;
	using CheckBox = javafx.scene.control.CheckBox;
	using ListCell = javafx.scene.control.ListCell;
	using HBox = javafx.scene.layout.HBox;
	using Color = javafx.scene.paint.Color;
	using Rectangle = javafx.scene.shape.Rectangle;

	public class LayerListCell : ListCell<Layer>
	{
		private CheckBox visibleCheckBox = new CheckBox();
		private Rectangle rect = new Rectangle(25, 15);

		public LayerListCell()
		{
			HBox box = new HBox(this.rect);
			box.setPadding(new Insets(0,0,0,3));
			this.rect.setStroke(Color.BLACK);
			this.visibleCheckBox.setText(null);
			this.visibleCheckBox.setGraphic(box); // this.rect

			// bind/unbind properties
			this.itemProperty().addListener(new ChangeListenerAnonymousInnerClass(this));
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<Layer>
		{
			private readonly LayerListCell outerInstance;

			public ChangeListenerAnonymousInnerClass(LayerListCell outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, Layer oldValue, Layer newValue) where T1 : org.applied_geodesy.jag3d.ui.graphic.layer.Layer
			{
				if (oldValue != null)
				{
					outerInstance.visibleCheckBox.selectedProperty().unbindBidirectional(oldValue.visibleProperty());
					outerInstance.rect.fillProperty().unbind();
				}

				if (newValue != null)
				{
					outerInstance.visibleCheckBox.selectedProperty().bindBidirectional(newValue.visibleProperty());
					outerInstance.rect.fillProperty().bind(newValue.colorProperty());
				}
			}
		}

		public override void updateItem(Layer layer, bool empty)
		{
			base.updateItem(layer, empty);
			if (!empty && layer != null)
			{
				this.setGraphic(this.visibleCheckBox);
				this.setText(layer.ToString());
			}
		}
	}
}