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

namespace org.applied_geodesy.adjustment.geometry.curve
{
	using CurveFeature = org.applied_geodesy.adjustment.geometry.CurveFeature;

	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;

	public class ModifiableCurveFeature : CurveFeature
	{

		public ModifiableCurveFeature() : base(false)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void deriveInitialGuess() throws MatrixSingularException, IllegalArgumentException, NotConvergedException, UnsupportedOperationException
		public override void deriveInitialGuess()
		{
			throw new System.NotSupportedException("Error, a modifiable feature does not provide a method for deriving an initial guess!");
		}

		public override ObjectProperty<bool> estimateInitialGuessProperty()
		{
			throw new System.NotSupportedException("Error, a modifiable feature does not provide a method for deriving an initial guess!");
		}

		public override bool EstimateInitialGuess
		{
			set
			{
				throw new System.NotSupportedException("Error, a modifiable feature does not provide a method for deriving an initial guess!");
			}
			get
			{
				return false;
			}
		}


		public override ObjectProperty<bool> estimateCenterOfMassProperty()
		{
			throw new System.NotSupportedException("Error, a modifiable feature does not provide center of mass reduction!");
		}

		public override bool EstimateCenterOfMass
		{
			set
			{
				throw new System.NotSupportedException("Error, a modifiable feature does not provide center of mass reduction!");
			}
			get
			{
				return false;
			}
		}

	}

}