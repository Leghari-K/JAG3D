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

namespace org.applied_geodesy.jag3d.ui.table.column
{
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using TableColumn = javafx.scene.control.TableColumn;

	public class ContentColumn<S, T> : TableColumn<S, T>
	{
		private class WidthChangListener : ChangeListener<Number>
		{
			private readonly ContentColumn<S, T> outerInstance;

			public WidthChangListener(ContentColumn<S, T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> ov, Number oldValue, Number newValue) where T1 : Number
			{
				if (newValue != null && outerInstance.columnProperty.ColumnContentType != ColumnContentType.DEFAULT)
				{
					outerInstance.columnProperty.setPrefWidth(newValue.doubleValue());
				}
			}
		}

		private ColumnProperty columnProperty;

		public ContentColumn(ColumnProperty columnProperty)
		{
			this.columnProperty = columnProperty;
			this.setMinWidth(org.applied_geodesy.jag3d.ui.table.column.ColumnProperty.MIN_WIDTH);
			this.setPrefWidth(columnProperty.getPrefWidth());

			if (columnProperty.ColumnContentType != ColumnContentType.DEFAULT)
			{
				this.prefWidthProperty().bindBidirectional(columnProperty.prefWidthProperty());
				this.widthProperty().addListener(new WidthChangListener(this));
				this.sortTypeProperty().bindBidirectional(columnProperty.sortTypeProperty());
			}

		}

		public virtual ColumnProperty ColumnProperty
		{
			get
			{
				return this.columnProperty;
			}
		}
	}

}