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

namespace org.applied_geodesy.adjustment.network.parameter
{
	using Epoch = org.applied_geodesy.adjustment.network.Epoch;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using ObservationGroup = org.applied_geodesy.adjustment.network.observation.group.ObservationGroup;

	public abstract class UnknownParameter
	{
		private ObservationGroup observationGroup = new ObservationGroup(-1, Double.NaN, Double.NaN, Double.NaN, Epoch.REFERENCE);

		// Position in Designmatrix; -1 entspricht nicht gesetzt	
		private int colInJacobiMatrix = -1;

		/// <summary>
		/// Liefert den Parametertyp </summary>
		/// <returns> typ </returns>
		public abstract ParameterType ParameterType {get;}

		/// <summary>
		/// Gibt die Spaltenposition
		/// in der Designmatrix A zurueck </summary>
		/// <returns> row </returns>
		public virtual int ColInJacobiMatrix
		{
			get
			{
				return this.colInJacobiMatrix;
			}
			set
			{
				this.colInJacobiMatrix = value;
			}
		}


		/// <summary>
		/// Setzt die Beobachtung, die zum Parameter gehoeren </summary>
		/// <param name="observation"> </param>
		public virtual Observation Observation
		{
			set
			{
				this.observationGroup.add(value);
			}
		}

		/// <summary>
		/// Liefert eine allg. Beobachtungsgruppe mit allen Beobachtungen,
		/// die zum Parameter. </summary>
		/// <returns> observationGroup </returns>
		public virtual ObservationGroup Observations
		{
			get
			{
				return this.observationGroup;
			}
		}

		/// <summary>
		/// Liefert die Anzahl der Beobachtungen des Parameters </summary>
		/// <returns> size </returns>
		public virtual int numberOfObservations()
		{
			return this.observationGroup.size();
		}

	}

}