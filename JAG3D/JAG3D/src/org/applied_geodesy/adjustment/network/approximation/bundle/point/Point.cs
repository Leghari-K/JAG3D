using System;
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

namespace org.applied_geodesy.adjustment.network.approximation.bundle.point
{

	using PointBundle = org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle;
	using Transformation = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.Transformation;
	using TransformationParameterType = org.applied_geodesy.adjustment.network.approximation.bundle.transformation.TransformationParameterType;

	public abstract class Point
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			coordinates = new double[this.Dimension];
			weigths = new double[this.Dimension];
		}

		// Punktnummer
		private string name;
		// Spalte in Designmatrix; -1 entspricht nicht gesetzt
		private int colInJacobiMatrix = -1;
		// Zeile in Designmatrix; -1 entspricht nicht gesetzt
		private int rowInJacobiMatrix = -1;
//JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool isOutlier_Conflict = false;

		private IList<PointBundle> bundles = new List<PointBundle>();

		private double[] coordinates;
		private double[] weigths;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Point(String name) throws IllegalArgumentException
		public Point(string name)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (string.ReferenceEquals(name, null) || name.Trim().Length == 0)
			{
				throw new System.ArgumentException(this.GetType() + " Punktnummer ungueltig!");
			}
			this.name = name.Trim();
			for (int i = 0; i < this.Dimension; i++)
			{
				this.weigths[i] = 1.0;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		public abstract int Dimension {get;}

		public virtual double WeightedX
		{
			set
			{
				this.weigths[0] = value;
			}
			get
			{
				return this.weigths[0];
			}
		}

		public virtual double WeightedY
		{
			set
			{
				this.weigths[1] = value;
			}
			get
			{
				return this.weigths[1];
			}
		}

		public virtual double WeightedZ
		{
			set
			{
				this.weigths[this.Dimension - 1] = value;
			}
			get
			{
				return this.weigths[this.Dimension - 1];
			}
		}




		public virtual double X
		{
			set
			{
				this.coordinates[0] = value;
			}
			get
			{
				return this.coordinates[0];
			}
		}

		public virtual double Y
		{
			set
			{
				this.coordinates[1] = value;
			}
			get
			{
				return this.coordinates[1];
			}
		}

		public virtual double Z
		{
			set
			{
				this.coordinates[this.Dimension - 1] = value;
			}
			get
			{
				return this.coordinates[this.Dimension - 1];
			}
		}




		public virtual void join(Point p)
		{
			if (this.name.Equals(p.Name) && this.Dimension == p.Dimension)
			{
				if (this.Dimension == 1)
				{
					this.Z = 0.5 * (this.Z + p.Z);
				}
				else if (this.Dimension == 2)
				{
					this.X = 0.5 * (this.X + p.X);
					this.Y = 0.5 * (this.Y + p.Y);
				}
				else if (this.Dimension == 3)
				{
					this.X = 0.5 * (this.X + p.X);
					this.Y = 0.5 * (this.Y + p.Y);
					this.Z = 0.5 * (this.Z + p.Z);
				}
			}
		}

		public virtual double getDistance3D(Point p)
		{
		if (this.Dimension + p.Dimension < 6)
		{
			throw new System.ArgumentException("Raumstrecke nur zwischen 3D Punkten bestimmbar " + this.name + " " + this.Dimension + "D und " + p.Name + " " + p.Dimension + "D");
		}

			return Math.Sqrt(Math.Pow(this.X - p.X,2) + Math.Pow(this.Y - p.Y,2) + Math.Pow(this.Z - p.Z,2));
		}

		public virtual double getDistance2D(Point p)
		{
		if (this.Dimension == 1 || p.Dimension == 1)
		{
			throw new System.ArgumentException("Horizontalstrecke nicht mit 1D Punkte(n) bestimmbar " + this.name + " " + this.Dimension + "D und " + p.Name + " " + p.Dimension + "D");
		}
			return Math.Sqrt(Math.Pow(this.X - p.X,2) + Math.Pow(this.Y - p.Y,2));
		}

		public virtual int RowInJacobiMatrix
		{
			get
			{
				return this.rowInJacobiMatrix;
			}
			set
			{
				this.rowInJacobiMatrix = value;
			}
		}


		public virtual int ColInJacobiMatrix
		{
			get
			{
				return this.colInJacobiMatrix;
			}
			set
			{
				this.colInJacobiMatrix = value;
			}
		}


		public abstract Transformation getTransformation(PointBundle b1, PointBundle b2);

		public virtual PointBundle CurrentBundle
		{
			get
			{
				if (this.bundles.Count > 0)
				{
					return this.bundles[this.bundles.Count - 1];
				}
				this.addBundle();
				return this.CurrentBundle;
			}
		}

		public virtual IList<PointBundle> PointBundles
		{
			get
			{
				return this.bundles;
			}
		}

		public virtual void addBundle()
		{
			this.addBundle(false);
		}

		public virtual void addBundle(bool isInsersectionBundle)
		{
			this.bundles.Add(new PointBundle(this, isInsersectionBundle));
		}

		public virtual void removeEmptyBundles()
		{
			IList<PointBundle> newBundles = new List<PointBundle>();
			for (int i = 0; i < this.bundles.Count; i++)
			{
				PointBundle bundle = this.bundles[i];
				if (bundle.size() > 0 && !(bundle.size() == 1 && bundle.get(0) == this))
				{
					newBundles.Add(bundle);
				}
			}
			this.bundles = newBundles;
		}

		public virtual bool containsPointInBundle(string pointId)
		{
			foreach (PointBundle bundle in this.bundles)
			{
				if (bundle.contains(pointId))
				{
					return true;
				}
			}
			return false;
		}

		public virtual void joinBundles()
		{
			this.removeEmptyBundles();
			if (this.bundles.Count > 1)
			{
				Transformation trans = null;
				do
				{
					trans = null;
					PointBundle bundle1 = null, bundle2 = null;
					int numIdentPoints = -1;
					// permutiere ueber alle Bundle um das Optimale == meisten identischen Punkte zu ermitteln
					for (int i = 0; i < this.bundles.Count; i++)
					{
						PointBundle b1 = this.bundles[i];
						for (int j = 0; j < this.bundles.Count; j++)
						{
							if (i == j)
							{
								continue;
							}
							PointBundle b2 = this.bundles[j];
							Transformation t = this.getTransformation(b1, b2);

							if (t != null && t.numberOfIdenticalPoints() >= t.numberOfRequiredPoints() && t.numberOfIdenticalPoints() > numIdentPoints)
							{
								numIdentPoints = t.numberOfIdenticalPoints();
								bundle1 = b1;
								bundle2 = b2;
							}
						}
					}
					if (bundle1 != null && bundle2 != null)
					{
						if (bundle1.Intersection && !bundle2.Intersection)
						{
							trans = this.getTransformation(bundle1, bundle2);
						}
						else if (!bundle1.Intersection && bundle2.Intersection)
						{
							trans = this.getTransformation(bundle2, bundle1);
						}
						else if (bundle1.size() < bundle2.size())
						{
							trans = this.getTransformation(bundle1, bundle2);
						}
						else
						{
							trans = this.getTransformation(bundle2, bundle1);
						}
					}
					else
					{
						trans = null;
					}

					if (trans != null && trans.numberOfIdenticalPoints() >= trans.numberOfRequiredPoints())
					{
						//trans.setFixedParameter(TransformationParameterSet.SCALE, true);
						trans.setFixedParameter(TransformationParameterType.SCALE, !bundle1.Intersection && !bundle2.Intersection);
						if (trans.transformLMS())
						{
							PointBundle joinedBundle = trans.TransformdPoints;
							if (joinedBundle != null)
							{
								joinedBundle.Intersection = bundle1.Intersection && bundle2.Intersection;
								this.bundles.Remove(bundle1);
								this.bundles.Remove(bundle2);
								this.bundles.Add(joinedBundle);
							}
							else
							{
								trans = null;
							}
						}
						else
						{
							trans = null;
						}
					}
					else
					{
						trans = null;
					}
				} while (trans != null);
			}
		}
		public virtual void isOutlier(bool isOutlier)
		{
			this.isOutlier_Conflict = isOutlier;
		}
		public virtual bool Outlier
		{
			get
			{
				return isOutlier_Conflict;
			}
		}
	}
}