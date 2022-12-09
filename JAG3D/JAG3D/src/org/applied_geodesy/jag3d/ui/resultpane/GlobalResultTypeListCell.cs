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

namespace org.applied_geodesy.jag3d.ui.resultpane
{
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Node = javafx.scene.Node;
	using ListCell = javafx.scene.control.ListCell;

	public class GlobalResultTypeListCell : ListCell<Node>
	{
		private I18N i18n = I18N.Instance;

		protected internal virtual void updateItem(Node item, bool empty)
		{
			base.updateItem(item, empty);

			this.setGraphic(null);
			this.setText(null);

			if (item != null && !empty && item.getUserData() is GlobalResultType)
			{
				this.setText(this.getText((GlobalResultType)item.getUserData()));
			}
		}

		private string getText(GlobalResultType type)
		{
			switch (type)
			{
			case org.applied_geodesy.jag3d.ui.resultpane.GlobalResultType.TEST_STATISTIC:
				return i18n.getString("GlobalResultTableListCell.test_statistic.label", "Test statistics");
			case org.applied_geodesy.jag3d.ui.resultpane.GlobalResultType.VARIANCE_COMPONENT:
				return i18n.getString("GlobalResultTableListCell.variance_component.label", "Variance components estimation");
			case org.applied_geodesy.jag3d.ui.resultpane.GlobalResultType.PRINCIPAL_COMPONENT:
				return i18n.getString("GlobalResultTableListCell.principal_component.label", "Principal component analysis");
			}
			return null;
		}
	}
}