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

namespace org.applied_geodesy.adjustment
{
	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using UpperSymmBandMatrix = no.uib.cipr.matrix.UpperSymmBandMatrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class NormalEquationSystem
	{
		private readonly UpperSymmPackMatrix N;
		private readonly DenseVector n;
		private UpperSymmBandMatrix V;

		public NormalEquationSystem(UpperSymmPackMatrix N, DenseVector n) : this(N, n, null)
		{
		}

		public NormalEquationSystem(UpperSymmPackMatrix N, DenseVector n, UpperSymmBandMatrix V)
		{
			this.N = N;
			this.n = n;
			this.V = V;
		}

		/// <summary>
		/// Liefert die Normalgleichung 
		/// 
		/// N = A'*P*A  R'
		///     R       0 </summary>
		/// <returns> N </returns>
		public virtual UpperSymmPackMatrix Matrix
		{
			get
			{
				return this.N;
			}
		}

		/// <summary>
		/// Liefert den n-Vektor
		/// 
		/// n = A'*P*w
		///     r
		/// </summary>
		/// <returns> n </returns>
		public virtual DenseVector Vector
		{
			get
			{
				return this.n;
			}
		}

		public virtual UpperSymmBandMatrix Preconditioner
		{
			get
			{
				return this.V;
			}
		}
	}

}