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

namespace org.applied_geodesy.adjustment.network.congruence.strain
{

	using Constant = org.applied_geodesy.adjustment.Constant;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using StrainParameter = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameter;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	using Matrix = no.uib.cipr.matrix.Matrix;


	public abstract class StrainAnalysisEquations
	{
		internal StrainParameter[] strainParameters = new StrainParameter[0];
		private IList<RestrictionType> restrictions = new List<RestrictionType>(10);
		private IDictionary<ParameterType, StrainParameter> strainParameterMap = new Dictionary<ParameterType, StrainParameter>();
		internal StrainAnalysisEquations()
		{
			this.initDefaultRestictions();
			this.strainParameters = this.initStrainParameters();
			foreach (StrainParameter param in this.strainParameters)
			{
				strainParameterMap[param.ParameterType] = param;
			}
		}

		internal virtual StrainParameter getParameterByType(ParameterType type)
		{
			return this.strainParameterMap[type];
		}

		/// <summary>
		/// Anzahl der Zusatzparameter, die aus den Hinlfsgroessen gewonnen werden bspw. Drehwinkel aus Quaternion </summary>
		/// <returns> u </returns>
		public abstract int numberOfExpandedParameters();

		/// <summary>
		/// Anzahl der Parameter </summary>
		/// <returns> u </returns>
		public virtual int numberOfParameters()
		{
			return this.strainParameters.Length;
		}

		/// <summary>
		/// Liefert den i-ten Parameter </summary>
		/// <param name="i"> </param>
		/// <returns> u </returns>
		public virtual StrainParameter get(int i)
		{
			return this.strainParameters[i];
		}

		public static Equation[] getEquations(int dim)
		{
			switch (dim)
			{
			case 1:
				return new Equation[] {Equation.Z};
			case 2:
				return new Equation[] {Equation.X, Equation.Y};
			case 3:
				return new Equation[] {Equation.X, Equation.Y, Equation.Z};
			}
			return null;
		}

		public static CoordinateComponent[] getCoordinateComponents(int dim)
		{
			switch (dim)
			{
			case 1:
				return new CoordinateComponent[] {CoordinateComponent.Z1, CoordinateComponent.Z2};
			case 2:
				return new CoordinateComponent[] {CoordinateComponent.X1, CoordinateComponent.Y1, CoordinateComponent.X2, CoordinateComponent.Y2};
			case 3:
				return new CoordinateComponent[] {CoordinateComponent.X1, CoordinateComponent.Y1, CoordinateComponent.Z1, CoordinateComponent.X2, CoordinateComponent.Y2, CoordinateComponent.Z2};
			}
			return null;
		}

	//	/**
	//	 * Anzahl der festgehaltenen Parameter
	//	 * @return u
	//	 */
	//	public int numberOfConstraintParameters() {
	//		int cnt = 0;
	//		for (Restriction restriction : this.restrictions) {
	//			if (restriction != Restriction.IDENT_SCALES_XY && restriction != Restriction.IDENT_SCALES_XZ && restriction != Restriction.IDENT_SCALES_YZ && restriction != Restriction.UNIT_QUATERNION)
	//				cnt++;
	//		}
	//		return cnt;
	//	}

		/// <summary>
		/// Liefert true, wenn Gruppenparameter bestimmt werden sollen </summary>
		/// <returns> hasUnconstraintParameters </returns>
		public virtual bool hasUnconstraintParameters()
		{
			return this.numberOfParameters() - this.numberOfRestrictions() > 0;
		}

		/// <summary>
		/// Anzahl der Restriktinen im Modell </summary>
		/// <returns> r </returns>
		public virtual int numberOfRestrictions()
		{
			return this.restrictions.Count;
		}

		/// <summary>
		/// Liefert <code>true</code>, wenn die <code>restriction</code> im Modell beruecksichtigt wird </summary>
		/// <param name="restriction"> </param>
		/// <returns> restricted  </returns>
		public virtual bool isRestricted(RestrictionType restriction)
		{
			return this.restrictions.Contains(restriction);
		}

		/// <summary>
		/// Liefert die Restriktion an der Stelle <code>index</code> </summary>
		/// <param name="index"> </param>
		/// <returns> restriction </returns>
		public virtual RestrictionType getRestriction(int index)
		{
			return this.restrictions[index];
		}

		/// <summary>
		/// Fuegt Restriktionen zum Modell hinzu </summary>
		/// <param name="restriction">  </param>
		public virtual void addRestriction(RestrictionType restriction)
		{
			if (this.isSupportedRestriction(restriction) && !this.restrictions.Contains(restriction))
			{
				if (restriction == RestrictionType.FIXED_SCALE_X)
				{
					this.restrictions.Remove(RestrictionType.IDENT_SCALES_XY);
					this.restrictions.Remove(RestrictionType.IDENT_SCALES_XZ);
				}
				else if (restriction == RestrictionType.FIXED_SCALE_Y)
				{
					this.restrictions.Remove(RestrictionType.IDENT_SCALES_XY);
					this.restrictions.Remove(RestrictionType.IDENT_SCALES_YZ);
				}
				else if (restriction == RestrictionType.FIXED_SCALE_Z)
				{
					this.restrictions.Remove(RestrictionType.IDENT_SCALES_XZ);
					this.restrictions.Remove(RestrictionType.IDENT_SCALES_YZ);
				}

				else if (restriction == RestrictionType.IDENT_SCALES_XY)
				{
					this.restrictions.Remove(RestrictionType.FIXED_SCALE_X);
					this.restrictions.Remove(RestrictionType.FIXED_SCALE_Y);
				}
				else if (restriction == RestrictionType.IDENT_SCALES_XZ)
				{
					this.restrictions.Remove(RestrictionType.FIXED_SCALE_X);
					this.restrictions.Remove(RestrictionType.FIXED_SCALE_Z);
				}
				else if (restriction == RestrictionType.IDENT_SCALES_YZ)
				{
					this.restrictions.Remove(RestrictionType.FIXED_SCALE_Y);
					this.restrictions.Remove(RestrictionType.FIXED_SCALE_Z);
				}

				this.restrictions.Add(restriction);

				// verhindere das Speichern von redundanten Information
				if (this.restrictions.Contains(RestrictionType.IDENT_SCALES_XY) && this.restrictions.Contains(RestrictionType.IDENT_SCALES_XZ) && this.restrictions.Contains(RestrictionType.IDENT_SCALES_YZ))
				{
					this.restrictions.Remove(RestrictionType.IDENT_SCALES_XY);
				}
			}
		}

		/// <summary>
		/// Bestimmt stochastische Kenngroessen der einzelnen Parameter und fuegt diese hinzu </summary>
		/// <param name="param"> </param>
		/// <param name="sigma2apost"> </param>
		/// <param name="qxxPrio"> </param>
		/// <returns> param </returns>
		internal virtual StrainParameter setStochasticParameters(StrainParameter param, double sigma2apost, double qxxPrio, bool applyAposterioriVarianceOfUnitWeight)
		{
			qxxPrio = Math.Abs(qxxPrio);
			double value = param.Value;

			if (param.ParameterType == ParameterType.STRAIN_ROTATION_X || param.ParameterType == ParameterType.STRAIN_ROTATION_Y || param.ParameterType == ParameterType.STRAIN_ROTATION_Z || param.ParameterType == ParameterType.STRAIN_SHEAR_X || param.ParameterType == ParameterType.STRAIN_SHEAR_X || param.ParameterType == ParameterType.STRAIN_SHEAR_Z)
			{
				value = MathExtension.MOD(value, 2.0 * Math.PI);
				if (Math.Abs(2.0 * Math.PI - value) < Math.Abs(value))
				{
					value = value - 2.0 * Math.PI;
				}
			}

			double qxxPost = sigma2apost * qxxPrio;
			double value0 = param.ExpectationValue;
			double nabla = value - value0;
			double tPrio = qxxPrio < Constant.EPS ? double.PositiveInfinity : nabla * nabla / qxxPrio;
			double tPost = qxxPost < Constant.EPS ? double.PositiveInfinity : nabla * nabla / qxxPost;
			double nabla0 = Math.Sign(nabla) * Math.Sqrt(qxxPrio);

			param.Tprio = tPrio;
			param.Tpost = applyAposterioriVarianceOfUnitWeight ? tPost : 0.0;
			param.Std = Math.Sqrt(qxxPost);
			param.GrossError = nabla;
			param.MinimalDetectableBias = nabla0;

			return param;
		}

		public abstract void expandParameters(double sigma2apost, Matrix Quu, bool applyAposterioriVarianceOfUnitWeight);
		internal abstract void initDefaultRestictions();
		internal abstract StrainParameter[] initStrainParameters();
		public abstract bool isSupportedRestriction(RestrictionType restriction);

		/// <summary>
		/// Liefert die partielle Ableitung der Funktion am entsprechenden Parameter zur Bildung der A-Matrix </summary>
		/// <param name="parameter"> </param>
		/// <param name="p1"> </param>
		/// <param name="equation"> </param>
		/// <returns> diff </returns>
		public abstract double diff(StrainParameter parameter, Point p1, Equation equation);
		/// <summary>
		/// Liefert die partielle Ableitung der Funktion am Punkt p1 bzw. p2 zur Bildung der A-Matrix </summary>
		/// <param name="p1"> </param>
		/// <param name="p2"> </param>
		/// <param name="component"> </param>
		/// <param name="equation"> </param>
		/// <returns> diff </returns>
		public abstract double diff(Point p1, Point p2, CoordinateComponent component, Equation equation);
		/// <summary>
		/// Liefert die partielle Ableitung zur Bildung der R-Matrix fuer die Bedingungen </summary>
		/// <param name="parameter"> </param>
		/// <param name="restriction"> </param>
		/// <returns> diff </returns>
		public abstract double diff(StrainParameter parameter, RestrictionType restriction);
		public abstract double getContradiction(Point p1, Point p2, Equation equation);
		public abstract double getContradiction(RestrictionType restriction);
	}

}