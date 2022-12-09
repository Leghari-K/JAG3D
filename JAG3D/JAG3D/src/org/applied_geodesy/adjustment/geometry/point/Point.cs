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

namespace org.applied_geodesy.adjustment.geometry.point
{
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using ReadOnlyObjectProperty = javafx.beans.property.ReadOnlyObjectProperty;
	using ReadOnlyObjectWrapper = javafx.beans.property.ReadOnlyObjectWrapper;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;

	public class Point
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			name = new SimpleObjectProperty<string>(this, "name", "");
			x0 = new SimpleObjectProperty<double>(this, "x0", 0.0);
			y0 = new SimpleObjectProperty<double>(this, "y0", 0.0);
			z0 = new SimpleObjectProperty<double>(this, "z0", 0.0);
		}

		private static int ID_CNT = 0;
		private ReadOnlyObjectProperty<int> id;
		private ObjectProperty<string> name;
		private ObjectProperty<double> x0;
		private ObjectProperty<double> y0;
		private ObjectProperty<double> z0;
		private ReadOnlyObjectProperty<int> dimension;

		public Point(Point point) : this(point.Name, point.X0, point.Y0, point.Z0, point.Dimension)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public Point(string name, double x0, double y0) : this(name, x0, y0, 0.0, 2)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public Point(string name, double x0, double y0, double z0) : this(name, x0, y0, z0, 3)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private Point(string name, double x0, double y0, double z0, int dimension)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.id = new ReadOnlyObjectWrapper<int>(this, "id", ID_CNT++);
			this.dimension = new ReadOnlyObjectWrapper<int>(this, "dimension", dimension);
			this.Name = name;
			this.X0 = x0;
			this.Y0 = y0;
			this.Z0 = z0;
		}

		public int Id
		{
			get
			{
				return this.id.get();
			}
		}

		public virtual ReadOnlyObjectProperty<int> idProperty()
		{
			return this.id;
		}

		public virtual string Name
		{
			get
			{
				return this.name.get();
			}
			set
			{
				this.name.set(value);
			}
		}


		public virtual ObjectProperty<string> nameProperty()
		{
			return this.name;
		}

		public virtual int Dimension
		{
			get
			{
				return this.dimension.get();
			}
		}

		public virtual ReadOnlyObjectProperty<int> dimensionProperty()
		{
			return this.dimension;
		}

		public virtual double X0
		{
			get
			{
				return this.x0.get();
			}
			set
			{
				this.x0.set(value);
			}
		}


		public virtual ObjectProperty<double> x0Property()
		{
			return this.x0;
		}

		public virtual double Y0
		{
			get
			{
				return this.y0.get();
			}
			set
			{
				this.y0.set(value);
			}
		}


		public virtual ObjectProperty<double> y0Property()
		{
			return this.y0;
		}

		public virtual double Z0
		{
			get
			{
				return this.z0.get();
			}
			set
			{
				this.z0.set(value);
			}
		}


		public virtual ObjectProperty<double> z0Property()
		{
			return this.z0;
		}

		public override string ToString()
		{
			return "Point#" + id.get() + " [name=" + name.get() + ", x0=" + x0.get() + ", y0=" + y0.get() + ", z0=" + z0.get() + ", dimension=" + dimension.get() + "]";
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((dimension == null) ? 0 : dimension.get().GetHashCode());
			result = prime * result + ((id == null) ? 0 : id.get().GetHashCode());
			result = prime * result + ((x0 == null) ? 0 : x0.get().GetHashCode());
			result = prime * result + ((y0 == null) ? 0 : y0.get().GetHashCode());
			result = prime * result + ((z0 == null) ? 0 : z0.get().GetHashCode());
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			Point other = (Point) obj;
			if (this.Id != other.Id)
			{
				return false;
			}
			if (!this.Name.Equals(other.Name))
			{
				return false;
			}
			return this.equalsCoordinateComponents(other);
		}

		public virtual bool equalsCoordinateComponents(Point point)
		{
			if (this == point)
			{
				return true;
			}
			if (point == null)
			{
				return false;
			}
			if (this.Dimension != point.Dimension)
			{
				return false;
			}
			if (this.X0 != point.X0)
			{
				return false;
			}
			if (this.Y0 != point.Y0)
			{
				return false;
			}
			if (this.Z0 != point.Z0)
			{
				return false;
			}
			return true;
		}
	}

}