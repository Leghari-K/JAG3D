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

namespace org.applied_geodesy.adjustment.network.congruence.strain.parameter
{
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;

	public class StrainParameterRotationX : StrainParameter
	{

		public StrainParameterRotationX() : base(0.0)
		{
		}

		public StrainParameterRotationX(double value0) : base(value0)
		{
		}

		public override ParameterType ParameterType
		{
			get
			{
				return ParameterType.STRAIN_ROTATION_X;
			}
		}

		public override double ExpectationValue
		{
			get
			{
				return 0;
			}
		}
	}
}