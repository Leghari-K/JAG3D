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

namespace org.applied_geodesy.jag3d.ui.table
{
	using AdditionalParameterRow = org.applied_geodesy.jag3d.ui.table.row.AdditionalParameterRow;
	using DisplayCellFormatType = org.applied_geodesy.ui.table.DisplayCellFormatType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	using Pos = javafx.geometry.Pos;
	using TableCell = javafx.scene.control.TableCell;

	internal class AdditionalParameterDoubleCell : TableCell<AdditionalParameterRow, double>, FormatterChangedListener
	{
		private DisplayCellFormatType displayFormatType;
		private FormatterOptions options = FormatterOptions.Instance;
		internal double? item;
		internal AdditionalParameterDoubleCell(DisplayCellFormatType displayFormatType)
		{
			this.displayFormatType = displayFormatType;
			this.setAlignment(Pos.CENTER_RIGHT);
			this.options.addFormatterChangedListener(this);
		}

		protected internal override void updateItem(double? value, bool empty)
		{
			int currentIndex = indexProperty().getValue();

			if (!empty && currentIndex >= 0 && currentIndex < this.getTableView().getItems().size())
			{
				AdditionalParameterRow paramRow = this.getTableView().getItems().get(currentIndex);
				this.item = value;
				this.toFormattedString(paramRow, this.item);
			}
			else
			{
				setText(value == null ? null : value.ToString());
			}
		}

		// https://stackoverflow.com/questions/27281370/javafx-tableview-format-one-cell-based-on-the-value-of-another-in-the-row
		private void toFormattedString(AdditionalParameterRow paramRow, double? value)
		{
			if (value != null && paramRow != null && paramRow.ParameterType != null)
			{
				switch (paramRow.ParameterType.innerEnumValue)
				{
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.ZERO_POINT_OFFSET:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_TRANSLATION_X:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_TRANSLATION_Y:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
					if (this.displayFormatType == DisplayCellFormatType.NORMAL)
					{
						this.setText(options.toLengthFormat(value.Value, true));
					}
					else if (this.displayFormatType == DisplayCellFormatType.UNCERTAINTY)
					{
						this.setText(options.toLengthUncertaintyFormat(value.Value, true));
					}
					else if (this.displayFormatType == DisplayCellFormatType.RESIDUAL)
					{
						this.setText(options.toLengthResidualFormat(value.Value, true));
					}

					break;
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.SCALE:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_SCALE_X:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_SCALE_Y:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_SCALE_Z:

					if (this.displayFormatType == DisplayCellFormatType.NORMAL)
					{
						this.setText(options.toScaleFormat(value.Value, true));
					}
					else if (this.displayFormatType == DisplayCellFormatType.UNCERTAINTY)
					{
						this.setText(options.toScaleUncertaintyFormat(value.Value, true));
					}
					else if (this.displayFormatType == DisplayCellFormatType.RESIDUAL)
					{
						this.setText(options.toScaleResidualFormat(value.Value, true));
					}

					break;
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.ORIENTATION:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.ROTATION_X:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.ROTATION_Y:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.ROTATION_Z:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_ROTATION_X:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_ROTATION_Y:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_ROTATION_Z:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_SHEAR_X:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_SHEAR_Y:
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.STRAIN_SHEAR_Z:
					if (this.displayFormatType == DisplayCellFormatType.NORMAL)
					{
						this.setText(options.toAngleFormat(value.Value, true));
					}
					else if (this.displayFormatType == DisplayCellFormatType.UNCERTAINTY)
					{
						this.setText(options.toAngleUncertaintyFormat(value.Value, true));
					}
					else if (this.displayFormatType == DisplayCellFormatType.RESIDUAL)
					{
						this.setText(options.toAngleResidualFormat(value.Value, true));
					}

					break;
				case org.applied_geodesy.adjustment.network.ParameterType.InnerEnum.REFRACTION_INDEX:
					this.setText(options.toStatisticFormat(value.Value));
					break;

				default:
					Console.Error.WriteLine(typeof(UIAdditionalParameterTableBuilder).Name + " : Error, unknown parameter type " + paramRow.ParameterType);
					setText(null);
					break;

				}
			}
			else
			{
				setText(null);
			}
		}

		public virtual void formatterChanged(FormatterEvent evt)
		{
			int idx = this.getTableRow().getIndex();
			int itemSize = this.getTableView().getItems().size();
			if (idx >= 0 && idx < itemSize)
			{
				AdditionalParameterRow paramRow = this.getTableView().getItems().get(idx);
				this.toFormattedString(paramRow, this.item);
			}
		}
	}

}