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

namespace org.applied_geodesy.jag3d.ui.table.rowhighlight
{

	using UICongruenceAnalysisTableBuilder = org.applied_geodesy.jag3d.ui.table.UICongruenceAnalysisTableBuilder;
	using UIGNSSObservationTableBuilder = org.applied_geodesy.jag3d.ui.table.UIGNSSObservationTableBuilder;
	using UIPointTableBuilder = org.applied_geodesy.jag3d.ui.table.UIPointTableBuilder;
	using UITerrestrialObservationTableBuilder = org.applied_geodesy.jag3d.ui.table.UITerrestrialObservationTableBuilder;
	using UIVerticalDeflectionTableBuilder = org.applied_geodesy.jag3d.ui.table.UIVerticalDeflectionTableBuilder;

	using Color = javafx.scene.paint.Color;

	public class TableRowHighlight
	{

		private static TableRowHighlight tableRowHighlight = new TableRowHighlight();
		private Color excellentColor = DefaultTableRowHighlightValue.ExcellentColor;
		private Color satisfactoryColor = DefaultTableRowHighlightValue.SatisfactoryColor;
		private Color inadequateColor = DefaultTableRowHighlightValue.InadequateColor;

		private IDictionary<TableRowHighlightType, double> leftBoundaries = new Dictionary<TableRowHighlightType, double>(TableRowHighlightType.values().Length);
		private IDictionary<TableRowHighlightType, double> rightBoundaries = new Dictionary<TableRowHighlightType, double>(TableRowHighlightType.values().Length);

		private TableRowHighlightType tableRowHighlightType = TableRowHighlightType.NONE;

		private TableRowHighlight()
		{
			foreach (TableRowHighlightType tableRowHighlightType in TableRowHighlightType.values())
			{
				double[] range = DefaultTableRowHighlightValue.getRange(tableRowHighlightType);
				if (range != null && range.Length >= 2)
				{
					this.setRange(tableRowHighlightType, range[0], range[1]);
				}
			}
		}

		public static TableRowHighlight Instance
		{
			get
			{
				return tableRowHighlight;
			}
		}

		public virtual TableRowHighlightType SelectedTableRowHighlightType
		{
			set
			{
				this.tableRowHighlightType = value;
			}
			get
			{
				return this.tableRowHighlightType;
			}
		}

		public virtual void setRange(TableRowHighlightType tableRowHighlightType, double leftBoundary, double rightBoundary)
		{
			double left = Math.Min(leftBoundary, rightBoundary);
			double right = Math.Max(leftBoundary, rightBoundary);
			this.leftBoundaries[tableRowHighlightType] = left;
			this.rightBoundaries[tableRowHighlightType] = right;
		}

		public virtual double getLeftBoundary(TableRowHighlightType tableRowHighlightType)
		{
			return this.leftBoundaries.ContainsKey(tableRowHighlightType) ? this.leftBoundaries[tableRowHighlightType] : double.NegativeInfinity;
		}

		public virtual double getRightBoundary(TableRowHighlightType tableRowHighlightType)
		{
			return this.rightBoundaries.ContainsKey(tableRowHighlightType) ? this.rightBoundaries[tableRowHighlightType] : double.PositiveInfinity;
		}


		public virtual void setColor(TableRowHighlightRangeType type, Color color)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.EXCELLENT:
				this.excellentColor = color;
				break;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.SATISFACTORY:
				this.satisfactoryColor = color;
				break;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.INADEQUATE:
				this.inadequateColor = color;
				break;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.NONE:
				break;
			}
		}

		public virtual Color getColor(TableRowHighlightRangeType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.EXCELLENT:
				return this.excellentColor;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.SATISFACTORY:
				return this.satisfactoryColor;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.INADEQUATE:
				return this.inadequateColor;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.NONE:
				return Color.TRANSPARENT;
			}

			return Color.TRANSPARENT;
		}

		public virtual void refreshTables()
		{
			// Update tables
			UITerrestrialObservationTableBuilder.Instance.refreshTable();
			UIGNSSObservationTableBuilder.Instance.refreshTable();
			UIPointTableBuilder.Instance.refreshTable();
			UICongruenceAnalysisTableBuilder.Instance.refreshTable();
			UIVerticalDeflectionTableBuilder.Instance.refreshTable();
		}

		public override string ToString()
		{
			return "TableRowHighlight [excellentColor=" + excellentColor + ", satisfactoryColor=" + satisfactoryColor + ", inadequateColor=" + inadequateColor + ", leftBoundaries=" + leftBoundaries + ", rightBoundaries=" + rightBoundaries + ", tableRowHighlightType=" + tableRowHighlightType + "]";
		}
	}

}