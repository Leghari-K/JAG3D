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

namespace org.applied_geodesy.adjustment.network.approximation.bundle
{

	using Point = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point;
	using Point1D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point1D;
	using Point2D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D;
	using Point3D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point3D;
	using TransformationParameterSet = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.TransformationParameterSet;

	public class PointBundle
	{
		private IList<Point> pointArrayList = new List<Point>();
		private IDictionary<string, Point> pointHashMap = new LinkedHashMap<string, Point>();
		private TransformationParameterSet transParameter = new TransformationParameterSet();
		private int dim = -1;
		private bool isIntersection = false;

		public PointBundle(int dim) : this(dim, false)
		{
		}

		public PointBundle(Point p) : this(p, false)
		{
		}

		public PointBundle(int dim, bool isIntersection)
		{
			this.dim = dim;
			this.isIntersection = isIntersection;
		}

		public PointBundle(Point p, bool isIntersection)
		{
			this.addPoint(p);
			this.isIntersection = isIntersection;
		}

		public virtual int size()
		{
			return this.pointArrayList.Count;
		}

		public virtual bool addPoint(Point p)
		{
			string pointId = p.Name;
			int dim = p.Dimension;
			if (this.dim < 0)
			{
				this.dim = dim;
			}

			if (this.dim != dim)
			{
				return false;
			}

			if (this.pointHashMap.ContainsKey(pointId))
			{
				Point pointInGroup = this.pointHashMap[pointId];
				pointInGroup.join(p);
			}
			else
			{
				this.add(p);
			}
			return true;
		}

		public virtual void removePoint(Point p)
		{
			this.pointHashMap.Remove(p.Name);
			this.pointArrayList.Remove(p);
		}

		private void add(Point p)
		{
			this.pointHashMap[p.Name] = p;
			this.pointArrayList.Add(p);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.applied_geodesy.adjustment.network.approximation.bundle.point.Point get(int index) throws ArrayIndexOutOfBoundsException
		public virtual Point get(int index)
		{
			return this.pointArrayList[index];
		}

		public virtual Point get(string pointId)
		{
			return this.pointHashMap[pointId];
		}

		public int Dimension
		{
			get
			{
				return this.dim;
			}
		}

		public virtual TransformationParameterSet TransformationParameterSet
		{
			get
			{
				return this.transParameter;
			}
			set
			{
				this.transParameter = value;
			}
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

		public virtual bool Intersection
		{
			set
			{
				this.isIntersection = this.dim > 1 && value;
			}
			get
			{
				return this.dim > 1 && this.isIntersection;
			}
		}


		public virtual bool contains(string pointId)
		{
			return this.pointHashMap.ContainsKey(pointId);
		}
		public override string ToString()
		{
			string str = "PointBundle = [";
			foreach (Point p in this.pointArrayList)
			{
				str += p + ", ";
			}
			str += "]";
			return str;
		}
	}

}