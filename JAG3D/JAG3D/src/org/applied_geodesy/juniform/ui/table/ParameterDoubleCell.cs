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

namespace org.applied_geodesy.juniform.ui.table
{
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using DisplayCellFormatType = org.applied_geodesy.ui.table.DisplayCellFormatType;
	using org.applied_geodesy.ui.table;
	using EditableDoubleCellConverter = org.applied_geodesy.ui.table.EditableDoubleCellConverter;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;

	using Pos = javafx.geometry.Pos;
	using TableRow = javafx.scene.control.TableRow;

	internal class ParameterDoubleCell : EditableCell<UnknownParameter, double>, FormatterChangedListener
	{
		private DisplayCellFormatType displayFormatType;
		private EditableDoubleCellConverter editableDoubleCellConverter;
		private FormatterOptions options = FormatterOptions.Instance;

		internal ParameterDoubleCell(DisplayCellFormatType displayFormatType) : base(new EditableDoubleCellConverter(CellValueType.LENGTH, true))
		{
			this.editableDoubleCellConverter = (EditableDoubleCellConverter)this.EditableCellConverter;
			this.displayFormatType = displayFormatType;
			this.setAlignment(Pos.CENTER_RIGHT);
			this.options.addFormatterChangedListener(this);
		}

		protected internal override void updateItem(double? value, bool empty)
		{
			int currentIndex = indexProperty().getValue();
			if (!empty && currentIndex >= 0 && currentIndex < this.getTableView().getItems().size())
			{
				UnknownParameter paramRow = this.getTableView().getItems().get(currentIndex);
				this.CellValueTypeOfUnknownParameter = paramRow;
			}
			base.updateItem(value, empty);
		}

		// https://stackoverflow.com/questions/27281370/javafx-tableview-format-one-cell-based-on-the-value-of-another-in-the-row
		private UnknownParameter CellValueTypeOfUnknownParameter
		{
			set
			{
				if (value != null && value.ParameterType != null)
				{
					switch (value.ParameterType)
					{
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.COORDINATE_X:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.COORDINATE_Y:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.COORDINATE_Z:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ORIGIN_COORDINATE_X:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ORIGIN_COORDINATE_Y:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ORIGIN_COORDINATE_Z:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.RADIUS:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.LENGTH:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.PRIMARY_FOCAL_COORDINATE_X:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.PRIMARY_FOCAL_COORDINATE_Y:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.PRIMARY_FOCAL_COORDINATE_Z:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.SECONDARY_FOCAL_COORDINATE_X:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.SECONDARY_FOCAL_COORDINATE_Y:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.SECONDARY_FOCAL_COORDINATE_Z:
    
						if (this.displayFormatType == DisplayCellFormatType.NORMAL)
						{
							this.editableDoubleCellConverter.CellValueType = CellValueType.LENGTH;
						}
						else if (this.displayFormatType == DisplayCellFormatType.UNCERTAINTY)
						{
							this.editableDoubleCellConverter.CellValueType = CellValueType.LENGTH_UNCERTAINTY;
						}
						else if (this.displayFormatType == DisplayCellFormatType.RESIDUAL)
						{
							this.editableDoubleCellConverter.CellValueType = CellValueType.LENGTH_RESIDUAL;
						}
    
						break;
    
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.VECTOR_X:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.VECTOR_Y:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.VECTOR_Z:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.VECTOR_LENGTH:
    
						if (this.displayFormatType == DisplayCellFormatType.NORMAL)
						{
							this.editableDoubleCellConverter.CellValueType = CellValueType.VECTOR;
						}
						else if (this.displayFormatType == DisplayCellFormatType.UNCERTAINTY)
						{
							this.editableDoubleCellConverter.CellValueType = CellValueType.VECTOR_UNCERTAINTY;
						}
						else if (this.displayFormatType == DisplayCellFormatType.RESIDUAL)
						{
							this.editableDoubleCellConverter.CellValueType = CellValueType.VECTOR_RESIDUAL;
						}
    
						break;
    
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ANGLE:
						if (this.displayFormatType == DisplayCellFormatType.NORMAL)
						{
							this.editableDoubleCellConverter.CellValueType = CellValueType.ANGLE;
						}
						else if (this.displayFormatType == DisplayCellFormatType.UNCERTAINTY)
						{
							this.editableDoubleCellConverter.CellValueType = CellValueType.ANGLE_UNCERTAINTY;
						}
						else if (this.displayFormatType == DisplayCellFormatType.RESIDUAL)
						{
							this.editableDoubleCellConverter.CellValueType = CellValueType.ANGLE_RESIDUAL;
						}
    
						break;
    
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.CONSTANT:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.MAJOR_AXIS_COEFFICIENT:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.MIDDLE_AXIS_COEFFICIENT:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.MINOR_AXIS_COEFFICIENT:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.POLYNOMIAL_COEFFICIENT_A:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.POLYNOMIAL_COEFFICIENT_B:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.POLYNOMIAL_COEFFICIENT_C:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.POLYNOMIAL_COEFFICIENT_D:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.POLYNOMIAL_COEFFICIENT_E:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.POLYNOMIAL_COEFFICIENT_F:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.POLYNOMIAL_COEFFICIENT_G:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.POLYNOMIAL_COEFFICIENT_H:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.POLYNOMIAL_COEFFICIENT_I:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ROTATION_COMPONENT_R11:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ROTATION_COMPONENT_R12:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ROTATION_COMPONENT_R13:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ROTATION_COMPONENT_R21:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ROTATION_COMPONENT_R22:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ROTATION_COMPONENT_R23:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ROTATION_COMPONENT_R31:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ROTATION_COMPONENT_R32:
					case org.applied_geodesy.adjustment.geometry.parameter.ParameterType.ROTATION_COMPONENT_R33:
						this.editableDoubleCellConverter.CellValueType = CellValueType.DOUBLE;
						break;
    
					default:
						throw new System.ArgumentException("Error, unknown type of parameter " + value.ParameterType);
					}
				}
			}
		}

		public virtual void formatterChanged(FormatterEvent evt)
		{
	//		int idx = this.getTableRow().getIndex();
	//		int itemSize = this.getTableView().getItems().size();
	//		if (idx >= 0 && idx < itemSize) {
	//			UnknownParameter paramRow = this.getTableView().getItems().get(idx);
	//			this.setCellValueTypeOfUnknownParameter(paramRow);
	//		}

			TableRow<UnknownParameter> tableRow = this.getTableRow();
			if (tableRow == null || tableRow.isEmpty() || this.getTableRow().getItem() == null)
			{
				return;
			}

			UnknownParameter paramRow = this.getTableRow().getItem();
			this.CellValueTypeOfUnknownParameter = paramRow;
		}
	}

}