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

namespace org.applied_geodesy.jag3d.ui.graphic.sql
{
	using WorldCoordinate = org.applied_geodesy.jag3d.ui.graphic.coordinate.WorldCoordinate;

	using BooleanProperty = javafx.beans.property.BooleanProperty;
	using SimpleBooleanProperty = javafx.beans.property.SimpleBooleanProperty;

	public class GraphicPoint
	{
		private WorldCoordinate coordinate;
		private double minorAxis = 0.0, majorAxis = 0.0, angle = 0.0, pPrio = 0;
		private double principalComponentX = 0.0, principalComponentY = 0.0, principalComponentZ = 0.0;
		private double residualX = 0.0, residualY = 0.0, residualZ = 0.0;
		private double minRedundancy = 0.0;
		private double maxInfluenceOnPosition = 0.0;
		private string name;
		private int dimension;
		private bool significant = false, grossErrorExceeded = false;
		private BooleanProperty visible = new SimpleBooleanProperty(true);

		public GraphicPoint(string name, int dimension, double x, double y) : this(name, dimension, x, y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, false, false)
		{
		}

		public GraphicPoint(string name, int dimension, double x, double y, double majorAxis, double minorAxis, double angle, double residualX, double residualY, double residualZ, double principalComponentX, double principalComponentY, double principalComponentZ, double minRedundancy, double maxInfluenceOnPosition, double pPrio, bool grossErrorExceeded, bool significant)
		{

			this.coordinate = new WorldCoordinate(x, y);
			this.name = name;
			this.dimension = dimension;
			this.majorAxis = Math.Max(majorAxis, minorAxis);
			this.minorAxis = Math.Min(majorAxis, minorAxis);
			this.principalComponentX = principalComponentX;
			this.principalComponentY = principalComponentY;
			this.principalComponentZ = principalComponentZ;
			this.residualX = residualX;
			this.residualY = residualY;
			this.residualZ = residualZ;
			this.minRedundancy = minRedundancy;
			this.maxInfluenceOnPosition = maxInfluenceOnPosition;
			this.angle = angle;
			this.pPrio = pPrio;
			this.grossErrorExceeded = grossErrorExceeded;
			this.significant = significant;
		}

		public virtual WorldCoordinate Coordinate
		{
			get
			{
				return this.coordinate;
			}
			set
			{
				this.coordinate = value;
			}
		}


		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}


		public virtual bool Significant
		{
			get
			{
				return this.significant;
			}
			set
			{
				this.significant = value;
			}
		}


		public virtual bool GrossErrorExceeded
		{
			get
			{
				return this.grossErrorExceeded;
			}
			set
			{
				this.grossErrorExceeded = value;
			}
		}


		public virtual double MinorAxis
		{
			get
			{
				return this.minorAxis;
			}
			set
			{
				this.minorAxis = value;
			}
		}


		public virtual double MajorAxis
		{
			get
			{
				return this.majorAxis;
			}
			set
			{
				this.majorAxis = value;
			}
		}


		public virtual double Angle
		{
			get
			{
				return this.angle;
			}
			set
			{
				this.angle = value;
			}
		}


		public virtual BooleanProperty visibleProperty()
		{
			return this.visible;
		}

		public virtual bool Visible
		{
			get
			{
				return this.visibleProperty().get();
			}
			set
			{
				this.visibleProperty().set(value);
			}
		}


		public virtual int Dimension
		{
			get
			{
				return this.dimension;
			}
		}

		public virtual double PrincipalComponentX
		{
			get
			{
				return this.principalComponentX;
			}
		}

		public virtual double PrincipalComponentY
		{
			get
			{
				return this.principalComponentY;
			}
		}

		public virtual double PrincipalComponentZ
		{
			get
			{
				return this.principalComponentZ;
			}
		}

		public virtual double ResidualX
		{
			get
			{
				return this.residualX;
			}
		}

		public virtual double ResidualY
		{
			get
			{
				return this.residualY;
			}
		}

		public virtual double ResidualZ
		{
			get
			{
				return this.residualZ;
			}
		}

		public virtual double MinRedundancy
		{
			get
			{
				return this.minRedundancy;
			}
		}

		public virtual double MaxInfluenceOnPosition
		{
			get
			{
				return this.maxInfluenceOnPosition;
			}
		}

		public virtual double Pprio
		{
			get
			{
				return this.pPrio;
			}
		}

		public override string ToString()
		{
			return this.name;
		}
	}

}