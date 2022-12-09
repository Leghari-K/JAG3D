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

namespace org.applied_geodesy.adjustment.network.observation.reduction
{

	using Ellipsoid = org.applied_geodesy.transformation.datum.Ellipsoid;
	using PrincipalPoint = org.applied_geodesy.transformation.datum.PrincipalPoint;

	public class Reduction
	{
		private ProjectionType projectionType = ProjectionType.LOCAL_CARTESIAN;
		private PrincipalPoint principalPoint = new PrincipalPoint();
		private Ellipsoid ellipsoid = Ellipsoid.SPHERE;
		private ISet<ReductionTaskType> reductionTypes = new HashSet<ReductionTaskType>(ReductionTaskType.values().Length);

		public virtual double EarthRadius
		{
			get
			{
				return this.ellipsoid.getRadiusOfConformalSphere(this.principalPoint.Latitude);
			}
		}

		public virtual PrincipalPoint PrincipalPoint
		{
			get
			{
				return this.principalPoint;
			}
		}

		public virtual Ellipsoid Ellipsoid
		{
			set
			{
				this.ellipsoid = value;
			}
			get
			{
				return this.ellipsoid;
			}
		}


		public virtual void clear()
		{
			this.reductionTypes.Clear();
		}

		public virtual bool applyReductionTask(ReductionTaskType type)
		{
			return this.reductionTypes.Contains(type);
		}

		public virtual bool addReductionTaskType(ReductionTaskType type)
		{
			if (type != null)
			{
				return this.reductionTypes.Add(type);
			}
			return false;
		}

		public virtual bool removeReductionTaskType(ReductionTaskType type)
		{
			return this.reductionTypes.remove(type);
		}

		public virtual ProjectionType ProjectionType
		{
			set
			{
				this.projectionType = value;
			}
			get
			{
				return this.projectionType;
			}
		}


		public virtual int size()
		{
			return this.reductionTypes.Count;
		}
	}

}