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

namespace org.applied_geodesy.ui.table
{
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterEventType = org.applied_geodesy.util.FormatterEventType;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using Unit = org.applied_geodesy.util.unit.Unit;

	using Bindings = javafx.beans.binding.Bindings;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using Label = javafx.scene.control.Label;
	using Tooltip = javafx.scene.control.Tooltip;

	public class ColumnTooltipHeader : FormatterChangedListener
	{
		private ObjectProperty<string> header = new SimpleObjectProperty<string>();
		private ObjectProperty<string> labelText = new SimpleObjectProperty<string>();
		private ObjectProperty<string> unitText = new SimpleObjectProperty<string>();
		private ObjectProperty<string> tooltipText = new SimpleObjectProperty<string>();

		private readonly CellValueType type;
		private Label label = new Label();
		private Tooltip tooltip = new Tooltip();
		private bool displayUnit = false;

		public ColumnTooltipHeader(CellValueType type, string label, string tooltip) : this(type, label, tooltip, null)
		{
		}

		public ColumnTooltipHeader(CellValueType type, string label, string tooltip, Unit unit)
		{
			FormatterOptions.Instance.addFormatterChangedListener(this);
			this.type = type;
			this.displayUnit = unit != null;

			this.header.bind(Bindings.concat(this.labelText).concat(this.unitText));
			this.label.textProperty().bind(this.header);
			this.tooltip.textProperty().bind(this.tooltipText);

			this.labelText.set(label);
			this.unitText.set(this.displayUnit ? " " + unit.toFormattedAbbreviation() : "");
			this.tooltipText.set(tooltip);
		}

		public virtual void formatterChanged(FormatterEvent evt)
		{
			if (this.displayUnit && evt.EventType == FormatterEventType.UNIT_CHANGED && this.type == evt.CellType)
			{
				Unit unit = evt.NewUnit;
				this.unitText.set(" " + unit.toFormattedAbbreviation());
			}
		}

		public virtual Label Label
		{
			get
			{
				return this.label;
			}
		}

		public virtual Tooltip Tooltip
		{
			get
			{
				return this.tooltip;
			}
		}
	}

}