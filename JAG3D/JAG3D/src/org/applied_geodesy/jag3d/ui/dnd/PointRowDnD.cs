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
	using PointRow = org.applied_geodesy.jag3d.ui.table.row.PointRow;

	[Serializable]
	public class PointRowDnD : TableRowDnD
	{
		private const long serialVersionUID = 7564914007654701928L;
		private string name = null;
		private string code = null;

		private double xApriori = 0.0;
		private double yApriori = 0.0;
		private double zApriori = 0.0;

		private double sigmaXapriori = -1.0;
		private double sigmaYapriori = -1.0;
		private double sigmaZapriori = -1.0;

		public static PointRowDnD fromPointRow(PointRow row)
		{
			if (row.XApriori == null || row.YApriori == null || row.ZApriori == null || string.ReferenceEquals(row.Name, null) || row.Name.Trim().Length == 0)
			{
				return null;
			}

			PointRowDnD rowDnD = new PointRowDnD();

			rowDnD.Id = row.Id;
			rowDnD.GroupId = row.GroupId;
			rowDnD.Enable = row.Enable.Value;

			rowDnD.name = row.Name;
			rowDnD.code = row.Code;

			rowDnD.xApriori = row.XApriori.Value;
			rowDnD.yApriori = row.YApriori.Value;
			rowDnD.zApriori = row.ZApriori.Value;

			rowDnD.sigmaXapriori = row.SigmaXapriori == null || row.SigmaXapriori < 0 ? -1 : row.SigmaXapriori;
			rowDnD.sigmaYapriori = row.SigmaYapriori == null || row.SigmaYapriori < 0 ? -1 : row.SigmaYapriori;
			rowDnD.sigmaZapriori = row.SigmaZapriori == null || row.SigmaZapriori < 0 ? -1 : row.SigmaZapriori;

			return rowDnD;
		}

		public virtual PointRow toPointRow()
		{
			PointRow row = new PointRow();

			row.Id = this.Id;
			row.GroupId = this.GroupId;
			row.Enable = this.Enable;

			row.Name = this.name;
			row.Code = this.code;

			row.XApriori = this.xApriori;
			row.YApriori = this.yApriori;
			row.ZApriori = this.zApriori;

			row.SigmaXapriori = this.sigmaXapriori < 0 ? null : this.sigmaXapriori;
			row.SigmaYapriori = this.sigmaYapriori < 0 ? null : this.sigmaYapriori;
			row.SigmaZapriori = this.sigmaZapriori < 0 ? null : this.sigmaZapriori;

			return row;
		}
	}


}