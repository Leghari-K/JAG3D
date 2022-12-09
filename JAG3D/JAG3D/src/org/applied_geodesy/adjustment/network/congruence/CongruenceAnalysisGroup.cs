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

namespace org.applied_geodesy.adjustment.network.congruence
{

	using RestrictionType = org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType;
	using StrainAnalysisEquations = org.applied_geodesy.adjustment.network.congruence.strain.StrainAnalysisEquations;
	using StrainAnalysisEquations1D = org.applied_geodesy.adjustment.network.congruence.strain.StrainAnalysisEquations1D;
	using StrainAnalysisEquations2D = org.applied_geodesy.adjustment.network.congruence.strain.StrainAnalysisEquations2D;
	using StrainAnalysisEquations3D = org.applied_geodesy.adjustment.network.congruence.strain.StrainAnalysisEquations3D;

	public class CongruenceAnalysisGroup
	{
		private readonly int id;
		private readonly int dimension;
		private IDictionary<string, CongruenceAnalysisPointPair> commonPointPairMap = new LinkedHashMap<string, CongruenceAnalysisPointPair>();
		private IDictionary<string, CongruenceAnalysisPointPair> strainAnalysablePointPairMap = new LinkedHashMap<string, CongruenceAnalysisPointPair>();
		private IList<CongruenceAnalysisPointPair> commonPointPairList = new List<CongruenceAnalysisPointPair>();
		private IList<CongruenceAnalysisPointPair> strainAnalysablePointPairList = new List<CongruenceAnalysisPointPair>();
		private StrainAnalysisEquations strainAnalysisEquations;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public CongruenceAnalysisGroup(int id, int dimension) throws IllegalArgumentException
		public CongruenceAnalysisGroup(int id, int dimension)
		{
			this.id = id;
			this.dimension = dimension;
			if (dimension == 1)
			{
				this.strainAnalysisEquations = new StrainAnalysisEquations1D();
			}
			else if (dimension == 2)
			{
				this.strainAnalysisEquations = new StrainAnalysisEquations2D();
			}
			else if (dimension == 3)
			{
				this.strainAnalysisEquations = new StrainAnalysisEquations3D();
			}
			else
			{
				throw new System.ArgumentException("Error, invalid vector dimension. Dimesnion mus be between 1 and 3: " + dimension);
			}
		}

		public virtual RestrictionType StrainRestrictions
		{
			set
			{
				this.strainAnalysisEquations.addRestriction(value);
			}
		}

		public virtual bool isRestricted(RestrictionType restriction)
		{
			return this.strainAnalysisEquations.isRestricted(restriction);
		}

		public int Id
		{
			get
			{
				return this.id;
			}
		}

		public virtual bool add(CongruenceAnalysisPointPair nexus, bool analysablePointPair)
		{
			int dimesnion = nexus.Dimension;
			string startPointId = nexus.StartPoint.Name;
			string endPointId = nexus.EndPoint.Name;

			if (this.dimension != dimesnion || this.commonPointPairMap.ContainsKey(startPointId) || this.commonPointPairMap.ContainsKey(endPointId) || this.strainAnalysablePointPairMap.ContainsKey(startPointId) || this.strainAnalysablePointPairMap.ContainsKey(endPointId))
			{
				return false;
			}

			if (analysablePointPair)
			{
				this.strainAnalysablePointPairList.Add(nexus);
				this.strainAnalysablePointPairMap[startPointId] = nexus;
				this.strainAnalysablePointPairMap[endPointId] = nexus;
			}
			else
			{
				this.commonPointPairMap[startPointId] = nexus;
				this.commonPointPairMap[endPointId] = nexus;
				this.commonPointPairList.Add(nexus);
			}

			return true;
		}

		public virtual CongruenceAnalysisPointPair get(int index, bool analysablePointPair)
		{
			return analysablePointPair ? this.strainAnalysablePointPairList[index] : this.commonPointPairList[index];
		}

		public virtual int totalSize()
		{
			return this.strainAnalysablePointPairList.Count + this.commonPointPairList.Count;
		}

		public virtual int size(bool analysablePointPair)
		{
			return analysablePointPair ? this.strainAnalysablePointPairList.Count : this.commonPointPairList.Count;
		}

		public virtual int Dimension
		{
			get
			{
				return this.dimension;
			}
		}

		public virtual StrainAnalysisEquations StrainAnalysisEquations
		{
			get
			{
				return this.strainAnalysisEquations;
			}
		}

		public virtual bool Empty
		{
			get
			{
				return this.totalSize() == 0;
			}
		}

		public override string ToString()
		{
			return new string(this.GetType() + " " + this.id + " point pairs in group: " + this.totalSize());
		}
	}
}