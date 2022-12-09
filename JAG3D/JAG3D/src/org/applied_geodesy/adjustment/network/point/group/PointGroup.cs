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

namespace org.applied_geodesy.adjustment.network.point.group
{

	using DefaultUncertainty = org.applied_geodesy.adjustment.network.DefaultUncertainty;
	using Point = org.applied_geodesy.adjustment.network.point.Point;
	using Point1D = org.applied_geodesy.adjustment.network.point.Point1D;
	using Point2D = org.applied_geodesy.adjustment.network.point.Point2D;
	using Point3D = org.applied_geodesy.adjustment.network.point.Point3D;

	public class PointGroup
	{
		private int id;
		private int dimension = -1; // Keine Dimension, wenn -1

		// doppelte Speicherung, um Index- und Punktnummerabfrage 
		// einfach zu realisieren
		private IDictionary<string, Point> pointHashMap = new LinkedHashMap<string, Point>();
		private IList<Point> pointArrayList = new List<Point>();

		public PointGroup(int id)
		{
			this.id = id;
		}

		public int Id
		{
			get
			{
				return this.id;
			}
		}

		public virtual bool add(Point point)
		{
			string name = point.Name;
			int pointDim = point.Dimension;

			if (this.dimension < 0)
			{
				this.dimension = pointDim;
			}

			if (this.dimension != pointDim || this.pointHashMap.ContainsKey(name))
			{
				return false;
			}

			if (this.dimension > 1)
			{
				point.StdX = point.StdX > 0 ? point.StdX : DefaultUncertainty.UncertaintyX;
				point.StdY = point.StdY > 0 ? point.StdY : DefaultUncertainty.UncertaintyY;
			}
			if (this.dimension != 2)
			{
				point.StdZ = point.StdZ > 0 ? point.StdZ : DefaultUncertainty.UncertaintyZ;
			}

			if (this.dimension == 3)
			{
				point.VerticalDeflectionX.Std = point.VerticalDeflectionX.Std > 0 ? point.VerticalDeflectionX.Std : DefaultUncertainty.UncertaintyDeflectionX;
				point.VerticalDeflectionY.Std = point.VerticalDeflectionY.Std > 0 ? point.VerticalDeflectionY.Std : DefaultUncertainty.UncertaintyDeflectionY;
			}

			this.pointHashMap[name] = point;
			this.pointArrayList.Add(point);

			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.applied_geodesy.adjustment.network.point.Point get(int index) throws ArrayIndexOutOfBoundsException
		public virtual Point get(int index)
		{
			return this.pointArrayList[index];
		}

		public virtual Point get(string pointId)
		{
			return this.pointHashMap[pointId];
		}

		public virtual int size()
		{
			return this.pointArrayList.Count;
		}

		public virtual int Dimension
		{
			get
			{
				return this.dimension;
			}
		}

		public override string ToString()
		{
			return new string(this.GetType() + " " + this.id + " Points in Group: " + this.size());
		}

		public virtual Point CenterPoint
		{
			get
			{
				double x = 0.0, y = 0.0, z = 0.0;
    
				for (int i = 0; i < this.size(); i++)
				{
					if (this.Dimension > 1)
					{
						x += this.get(i).getX();
						y += this.get(i).getY();
					}
					if (this.Dimension != 2)
					{
						z += this.get(i).getZ();
					}
				}
    
				if (this.Dimension == 1)
				{
					return new Point1D("c", z / this.size());
				}
    
				else if (this.Dimension == 2)
				{
					return new Point2D("c", x / this.size(), y / this.size());
				}
    
				else if (this.Dimension == 3)
				{
					return new Point3D("c", x / this.size(), y / this.size(), z / this.size());
				}
    
				return null;
			}
		}

	}

}