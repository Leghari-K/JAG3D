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

namespace org.applied_geodesy.adjustment.network.approximation.bundle.transformation
{
	public class TransformationParameter
	{
		private double value;
		private readonly TransformationParameterType type;
		private int col = -1;

		public TransformationParameter(TransformationParameterType type, double value)
		{
			this.value = value;
			this.type = type;
		}

		public virtual TransformationParameterType Type
		{
			get
			{
				return this.type;
			}
		}

		public virtual int ColInJacobiMatrix
		{
			set
			{
				this.col = value;
			}
			get
			{
				return this.col;
			}
		}


		public virtual bool Fixed
		{
			get
			{
				return this.col < 0;
			}
		}

		public virtual double Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

	}

}