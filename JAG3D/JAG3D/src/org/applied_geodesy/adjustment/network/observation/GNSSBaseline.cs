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

namespace org.applied_geodesy.adjustment.network.observation
{

	using RotationZ = org.applied_geodesy.adjustment.network.parameter.RotationZ;
	using Scale = org.applied_geodesy.adjustment.network.parameter.Scale;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	using Matrix = no.uib.cipr.matrix.Matrix;

	public abstract class GNSSBaseline : Observation
	{
		//private List<Observation> baselineComponents = new ArrayList<Observation>(3);

		private IDictionary<ComponentType, GNSSBaseline> baselineComponents = new LinkedHashMap<ComponentType, GNSSBaseline>(3);


		private Scale scale = new Scale();
		private RotationZ rz = new RotationZ();
		private Matrix subR;

		public GNSSBaseline(int id, Point startPoint, Point endPoint, double startPointHeight, double endPointHeight, double observation, double sigma) : base(id, startPoint, endPoint, startPointHeight, endPointHeight, observation, sigma, Math.Abs(observation))
		{
			this.baselineComponents[this.Component] = this;
		}

		/// <summary>
		/// Fuegt die uebrigen Komponenten der Basislinie hinzu </summary>
		/// <param name="baselineComp"> </param>
		public virtual void addAssociatedBaselineComponent(GNSSBaseline baseline)
		{
			if (this.Id == baseline.Id && !this.baselineComponents.ContainsKey(baseline.Component) && !this.baselineComponents.ContainsValue(baseline))
			{
				this.baselineComponents[baseline.Component] = baseline;
			}
		}

		/// <summary>
		/// Setzt bzw. ueberschreibt den Ma&szig;stab </summary>
		/// <param name="newScale"> Ma&szlig;stab </param>
		public virtual Scale Scale
		{
			set
			{
				this.scale = value;
				this.scale.Observation = this;
			}
			get
			{
				return this.scale;
			}
		}


		public override int ColInJacobiMatrixFromScale
		{
			get
			{
				return this.scale.ColInJacobiMatrix;
			}
		}

		/// <summary>
		/// Setzt bzw. ueberschreibt die Drehung </summary>
		/// <param name="r"> Drehung </param>
		public virtual RotationZ RotationZ
		{
			set
			{
				this.rz = value;
				this.rz.Observation = this;
			}
			get
			{
				return this.rz;
			}
		}


		public override int ColInJacobiMatrixFromRotationZ
		{
			get
			{
				return this.rz.ColInJacobiMatrix;
			}
		}

		/// <summary>
		/// Speichert die Matrix <code>R = Q<sub>vv</sub>P</code>, die zur Bestimmung der Testgroessen notwendig ist. 
		/// Uebergibt eine Referenz auf alle zugeordneten Beobachtungen (Basislinienteile).
		/// </summary>
		/// <param name="R"> </param>
		public virtual Matrix BaselineRedundancyMatrix
		{
			set
			{
				foreach (GNSSBaseline baseline in this.baselineComponents.Values)
				{
					baseline.RedundancyMatrix = value;
				}
			}
			get
			{
				return this.subR;
			}
		}

		protected internal virtual Matrix RedundancyMatrix
		{
			set
			{
				this.subR = value;
			}
		}


		/// <summary>
		/// Liefert die uebrigen Komponenten der Basislinie </summary>
		/// <returns> components </returns>
		public virtual IList<Observation> BaselineComponents
		{
			get
			{
				return new List<Observation>(this.baselineComponents.Values);
			}
		}

		/// <summary>
		/// Liefert die zurm Typ gehoerende Komponente oder null, wenn diese nicht existiert </summary>
		/// <returns> component </returns>
		public virtual GNSSBaseline getBaselineComponent(ComponentType type)
		{
			return this.baselineComponents[type];
		}

		/// <summary>
		/// Liefert die Dimension der Basislinie </summary>
		/// <returns> dim </returns>
		public abstract int Dimension {get;}

		/// <summary>
		/// Gibt Auskunft ueber den Anteil (X,Y,Z) der Basislinie </summary>
		/// <returns> omp </returns>
		public abstract ComponentType Component {get;}

		public override double diffVerticalDeflectionXs()
		{
			return 0;
		}

		public override double diffVerticalDeflectionYs()
		{
			return 0;
		}

		public override double diffVerticalDeflectionXe()
		{
			return 0;
		}

		public override double diffVerticalDeflectionYe()
		{
			return 0;
		}
	}

}