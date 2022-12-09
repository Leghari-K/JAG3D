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

namespace org.applied_geodesy.jag3d.ui.dialog.chart
{
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using ListCell = javafx.scene.control.ListCell;

	public class AnalysisChartTypeListCell : ListCell<AnalysisChartType>
	{
		private I18N i18n = I18N.Instance;

		protected internal override void updateItem(AnalysisChartType item, bool empty)
		{
			base.updateItem(item, empty);

			this.setGraphic(null);
			this.setText(null);

			if (item != null && !empty)
			{
				this.setText(this.getText(item));
			}
		}

		private string getText(AnalysisChartType type)
		{
			switch (type)
			{
			case org.applied_geodesy.jag3d.ui.dialog.chart.AnalysisChartType.RESIDUALS:
				return i18n.getString("AnalysisChartTypeListViewCell.residuals.label", "Residual analysis");
			case org.applied_geodesy.jag3d.ui.dialog.chart.AnalysisChartType.REDUNDANCY:
				return i18n.getString("AnalysisChartTypeListViewCell.redundancy.label", "Redundancy");
			}
			return null;
		}
	}
}