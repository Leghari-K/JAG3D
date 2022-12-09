using System.Collections.Generic;

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

	public class UnknownParameters
	{
		private int col = 0;
		private int lastPointColumn = -1, lastParamColumn = -1, leastParamColumn = -1;
		private IList<UnknownParameter> parameters = new List<UnknownParameter>();

		/// <summary>
		/// Sortiert die Parameter in der Liste nach {Punkte, Zusamtzparameter} ohne 
		/// die innere Reihenfolge der Punkte bzw. Parameter zu aendern 
		/// </summary>
		public virtual void resortParameters()
		{
			if (this.leastParamColumn < this.lastPointColumn && this.leastParamColumn >= 0)
			{
				IList<UnknownParameter> sortedParameters = new List<UnknownParameter>(this.parameters.Count);
				IList<UnknownParameter> nonPointParameters = new List<UnknownParameter>();
				this.col = 0;
				this.leastParamColumn = -1;
				this.lastParamColumn = -1;
				this.lastPointColumn = -1;

				foreach (UnknownParameter parameter in this.parameters)
				{
					switch (parameter.ParameterType)
					{
					case POINT1D:
					case POINT2D:
					case POINT3D:
						parameter.ColInJacobiMatrix = this.col;
						this.increaseColumnCount(parameter.ParameterType);
						sortedParameters.Add(parameter);
						break;
					default:
						nonPointParameters.Add(parameter);
						break;
					}
				}

				foreach (UnknownParameter parameter in nonPointParameters)
				{
					parameter.ColInJacobiMatrix = this.col;
					this.increaseColumnCount(parameter.ParameterType);
					sortedParameters.Add(parameter);
				}
				this.parameters = sortedParameters;
			}
		}

		public virtual bool add(UnknownParameter parameter)
		{
			if (!this.parameters.Contains(parameter))
			{
				parameter.ColInJacobiMatrix = this.col;
				this.increaseColumnCount(parameter.ParameterType);
				return this.parameters.Add(parameter);
			}
			return false;
		}

		public virtual int columnsOfAddionalParameters()
		{
			return this.lastParamColumn <= 0 ? 0 : this.lastParamColumn - this.lastPointColumn;
		}

		public virtual int columnsOfPoints()
		{
			return this.lastPointColumn;
		}

		public virtual int columnsInJacobi()
		{
			return this.col;
		}

		public virtual bool contains(object obj)
		{
			return this.parameters.Contains(obj);
		}

		private void increaseColumnCount(ParameterType type)
		{
			switch (type.innerEnumValue)
			{
				case ParameterType.InnerEnum.POINT1D:
					this.col += 1;
					this.lastPointColumn = this.col;
				break;
				case ParameterType.InnerEnum.POINT2D:
					this.col += 2;
					this.lastPointColumn = this.col;
				break;
				case ParameterType.InnerEnum.POINT3D:
					this.col += 3;
					this.lastPointColumn = this.col;
				break;
				default:
					this.col += 1;
					this.lastParamColumn = this.col;
					if (this.leastParamColumn < 0)
					{
						this.leastParamColumn = this.col;
					}
				break;
			}
		}

		public virtual UnknownParameter get(int i)
		{
			return this.parameters[i];
		}

		public virtual int size()
		{
			return this.parameters.Count;
		}
	}

}