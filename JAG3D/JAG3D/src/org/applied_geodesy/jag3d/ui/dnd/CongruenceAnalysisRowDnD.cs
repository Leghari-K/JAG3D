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

namespace org.applied_geodesy.jag3d.ui.dnd
{
	using CongruenceAnalysisRow = org.applied_geodesy.jag3d.ui.table.row.CongruenceAnalysisRow;

	[Serializable]
	public class CongruenceAnalysisRowDnD : TableRowDnD
	{
		private const long serialVersionUID = 6317004442775639322L;
		private string nameInReferenceEpoch = null;
		private string nameInControlEpoch = null;

		public static CongruenceAnalysisRowDnD fromCongruenceAnalysisRow(CongruenceAnalysisRow row)
		{
			if (string.ReferenceEquals(row.NameInReferenceEpoch, null) || string.ReferenceEquals(row.NameInControlEpoch, null) || row.NameInReferenceEpoch.Trim().Length == 0 || row.NameInControlEpoch.Trim().Length == 0 || row.NameInReferenceEpoch.Equals(row.NameInControlEpoch))
			{
				return null;
			}

			CongruenceAnalysisRowDnD rowDnD = new CongruenceAnalysisRowDnD();

			rowDnD.Id = row.Id;
			rowDnD.GroupId = row.GroupId;
			rowDnD.Enable = row.Enable;

			rowDnD.nameInReferenceEpoch = row.NameInReferenceEpoch;
			rowDnD.nameInControlEpoch = row.NameInControlEpoch;

			return rowDnD;
		}

		public virtual CongruenceAnalysisRow toCongruenceAnalysisRow()
		{
			CongruenceAnalysisRow row = new CongruenceAnalysisRow();

			row.Id = this.Id;
			row.GroupId = this.GroupId;
			row.Enable = this.Enable;

			row.NameInReferenceEpoch = this.nameInReferenceEpoch;
			row.NameInControlEpoch = this.nameInControlEpoch;

			return row;
		}
	}
}