﻿/// <summary>
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
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using StrainParameter = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameter;
	using StrainParameterScaleZ = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterScaleZ;
	using StrainParameterTranslationZ = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterTranslationZ;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	using Matrix = no.uib.cipr.matrix.Matrix;

	public class StrainAnalysisEquations1D : StrainAnalysisEquations
	{

		internal override StrainParameter[] initStrainParameters()
		{
			return new StrainParameter[]
			{
				new StrainParameterTranslationZ(),
				new StrainParameterScaleZ()
			};
		}

		public override bool isSupportedRestriction(RestrictionType restriction)
		{
			return restriction == RestrictionType.FIXED_TRANSLATION_Z || restriction == RestrictionType.FIXED_SCALE_Z;
		}

		public override double diff(StrainParameter parameter, Point p1, Equation equation)
		{
			double elmA = 0;
			//double zC1 = this.centerPoints.getZ1();
			double z1 = p1.Z;

			//Schwerpunktreduktion
			//double zP = z1 - zC1;
			double zP = z1;

			if (equation == Equation.Z)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// z0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
					elmA = 1.0;
					break;
					// mz
				case ParameterType.InnerEnum.STRAIN_SCALE_Z:
					elmA = zP;
					break;
				default:
					elmA = 0.0;
					break;
				}
			}
			return elmA;
		}

		public override double diff(Point p1, Point p2, CoordinateComponent component, Equation equation)
		{
			// Transformationsparameter
			double mz = this.getParameterByType(ParameterType.STRAIN_SCALE_Z).Value;
			double elmB = 0.0;
			if (equation == Equation.Z)
			{
				switch (component)
				{
				// zS
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.Z1:
					elmB = mz;
					break;
					// zT
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.Z2:
					elmB = -1.0;
					break;
				default:
					elmB = 0.0;
					break;
				}
			}
			return elmB;
		}

		public override double getContradiction(Point p1, Point p2, Equation equation)
		{
			//		double zC1 = this.centerPoints.getZ1();
			//		double zC2 = this.centerPoints.getZ2();

			double z1 = p1.Z;
			double z2 = p2.Z;

			//Schwerpunktreduktion
			//		double zP = z1 - zC1;
			//		double ZP = z2 - zC2;
			double zP = z1;
			double ZP = z2;

			// Transformationsparameter
			double z0 = this.getParameterByType(ParameterType.STRAIN_TRANSLATION_Z).Value;
			double mz = this.getParameterByType(ParameterType.STRAIN_SCALE_Z).Value;

			return (z0 + mz * zP) - ZP;
		}

		public override double diff(StrainParameter parameter, RestrictionType restriction)
		{
			double elmR = 0.0;

			if (restriction == RestrictionType.FIXED_TRANSLATION_Z)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// z0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
					elmR = 1.0;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_SCALE_Z)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// mz
				case ParameterType.InnerEnum.STRAIN_SCALE_Z:
					elmR = 1.0;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			return elmR;
		}

		public override double getContradiction(RestrictionType restriction)
		{
			// Transformationsparameter
			double z0 = this.getParameterByType(ParameterType.STRAIN_TRANSLATION_Z).Value;
			double mz = this.getParameterByType(ParameterType.STRAIN_SCALE_Z).Value;

			switch (restriction.innerEnumValue)
			{
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_TRANSLATION_Z:
				return z0;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_SCALE_Z:
				return mz - 1.0;
			default:
				return 0.0;
			}
		}

		internal override void initDefaultRestictions()
		{
		}
		public override void expandParameters(double sigma2apost, Matrix Quu, bool applyAposterioriVarianceOfUnitWeight)
		{
			for (int i = 0; i < this.strainParameters.Length; i++)
			{
				StrainParameter param = this.strainParameters[i];
				this.setStochasticParameters(param, sigma2apost, Quu.get(i, i), applyAposterioriVarianceOfUnitWeight);
			}
		}

		public override int numberOfExpandedParameters()
		{
			return 0;
		}
	}

}