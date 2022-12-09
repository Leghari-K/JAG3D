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

namespace org.applied_geodesy.util
{

	using Unit = org.applied_geodesy.util.unit.Unit;

	public class FormatterEvent : EventObject
	{
		private const long serialVersionUID = 7098232117005493587L;

		private readonly CellValueType cellType;
		private readonly FormatterEventType type;
		private readonly Unit oldUnit, newUnit;
		private readonly int oldRes, newRes;

		public FormatterEvent(FormatterOptions formatterOptions, FormatterEventType type, CellValueType cellType, Unit oldUnit, Unit newUnit, int res) : base(formatterOptions)
		{
			this.cellType = cellType;
			this.type = type;
			this.oldUnit = oldUnit;
			this.newUnit = newUnit;
			this.oldRes = res;
			this.newRes = res;
		}

		public FormatterEvent(FormatterOptions formatterOptions, FormatterEventType type, CellValueType cellType, Unit unit, int oldRes, int newRes) : base(formatterOptions)
		{
			this.cellType = cellType;
			this.type = type;
			this.oldUnit = unit;
			this.newUnit = unit;
			this.oldRes = oldRes;
			this.newRes = newRes;
		}

		public override FormatterOptions Source
		{
			get
			{
				return (FormatterOptions)base.getSource();
			}
		}

		public virtual Unit OldUnit
		{
			get
			{
				return this.oldUnit;
			}
		}

		public virtual Unit NewUnit
		{
			get
			{
				return this.newUnit;
			}
		}

		public virtual int OldResultion
		{
			get
			{
				return this.oldRes;
			}
		}

		public virtual int NewResultion
		{
			get
			{
				return this.newRes;
			}
		}

		public virtual FormatterEventType EventType
		{
			get
			{
				return this.type;
			}
		}

		public virtual CellValueType CellType
		{
			get
			{
				return this.cellType;
			}
		}

		public override string ToString()
		{
			return "FormatterEvent [cellType=" + cellType + ", type=" + type + ", oldUnit=" + oldUnit + ", newUnit=" + newUnit + ", oldRes=" + oldRes + ", newRes=" + newRes + "]";
		}
	}

}