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
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	/// <summary>
	/// Klasse ist eine Huelle fuer die Y-Komponente. Die statistischen groessen werden 
	/// in der X-Komponente abgespeichert, da beide (X und Y) als ein Objekt zu interpretieren sind.
	/// 
	/// </summary>
	public class VerticalDeflectionY : VerticalDeflection
	{

		public VerticalDeflectionY(Point point) : base(point)
		{
		}

		public VerticalDeflectionY(Point point, double value) : base(point, value)
		{
		}

		public VerticalDeflectionY(Point point, double value, double std) : base(point, value, std)
		{
		}

		public override ParameterType ParameterType
		{
			get
			{
				return ParameterType.VERTICAL_DEFLECTION_Y;
			}
		}

		public override string ToString()
		{
			return "VerticalDeflectionY [point=" + this.Point + ", value0=" + this.Value0 + ", value=" + this.Value + "]";
		}
	}

}