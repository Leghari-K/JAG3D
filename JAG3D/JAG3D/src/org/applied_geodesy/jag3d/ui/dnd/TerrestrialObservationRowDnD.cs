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
	using TerrestrialObservationRow = org.applied_geodesy.jag3d.ui.table.row.TerrestrialObservationRow;

	[Serializable]
	public class TerrestrialObservationRowDnD : ObservationRowDnD
	{
		private const long serialVersionUID = 6880080455876005180L;

		private double valueApriori;
		private double distanceApriori = -1.0;

		private double instrumentHeight = 0.0;
		private double reflectorHeight = 0.0;

		private double sigmaApriori = -1.0;

		private TerrestrialObservationRowDnD()
		{
		}

		public static TerrestrialObservationRowDnD fromTerrestrialObservationRow(TerrestrialObservationRow row)
		{
			if (row.ValueApriori == null || string.ReferenceEquals(row.StartPointName, null) || string.ReferenceEquals(row.EndPointName, null) || row.StartPointName.Trim().Length == 0 || row.EndPointName.Trim().Length == 0 || row.StartPointName.Equals(row.EndPointName))
			{
				return null;
			}

			TerrestrialObservationRowDnD rowDnD = new TerrestrialObservationRowDnD();

			rowDnD.Id = row.Id;
			rowDnD.GroupId = row.GroupId;
			rowDnD.Enable = row.Enable;

			rowDnD.StartPointName = row.StartPointName;
			rowDnD.EndPointName = row.EndPointName;

			rowDnD.instrumentHeight = row.InstrumentHeight == null ? 0 : row.InstrumentHeight.Value;
			rowDnD.reflectorHeight = row.ReflectorHeight == null ? 0 : row.ReflectorHeight.Value;

			rowDnD.valueApriori = row.ValueApriori.Value;
			rowDnD.distanceApriori = row.DistanceApriori == null || row.DistanceApriori < 0 ? -1 : row.DistanceApriori;
			rowDnD.sigmaApriori = row.SigmaApriori == null || row.SigmaApriori < 0 ? -1 : row.SigmaApriori;

			return rowDnD;
		}

		public virtual TerrestrialObservationRow toTerrestrialObservationRow()
		{
			TerrestrialObservationRow row = new TerrestrialObservationRow();

			row.Id = this.Id;
			row.GroupId = this.GroupId;
			row.Enable = this.Enable;

			row.StartPointName = this.StartPointName;
			row.EndPointName = this.EndPointName;

			row.InstrumentHeight = this.instrumentHeight;
			row.ReflectorHeight = this.reflectorHeight;

			row.ValueApriori = this.valueApriori;
			row.DistanceApriori = this.distanceApriori < 0 ? null : this.distanceApriori;
			row.SigmaApriori = this.sigmaApriori < 0 ? null : this.sigmaApriori;

			return row;
		}
	}

}