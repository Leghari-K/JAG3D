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

namespace org.applied_geodesy.adjustment.network.congruence.strain
{
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using StrainParameter = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameter;
	using StrainParameterQ0 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterQ0;
	using StrainParameterQ1 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterQ1;
	using StrainParameterQ2 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterQ2;
	using StrainParameterQ3 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterQ3;
	using StrainParameterRotationX = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterRotationX;
	using StrainParameterRotationY = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterRotationY;
	using StrainParameterRotationZ = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterRotationZ;
	using StrainParameterS11 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterS11;
	using StrainParameterS12 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterS12;
	using StrainParameterS13 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterS13;
	using StrainParameterS22 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterS22;
	using StrainParameterS23 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterS23;
	using StrainParameterS33 = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterS33;
	using StrainParameterScaleX = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterScaleX;
	using StrainParameterScaleY = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterScaleY;
	using StrainParameterScaleZ = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterScaleZ;
	using StrainParameterShearX = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterShearX;
	using StrainParameterShearY = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterShearY;
	using StrainParameterShearZ = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterShearZ;
	using StrainParameterTranslationX = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterTranslationX;
	using StrainParameterTranslationY = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterTranslationY;
	using StrainParameterTranslationZ = org.applied_geodesy.adjustment.network.congruence.strain.parameter.StrainParameterTranslationZ;
	using Point = org.applied_geodesy.adjustment.network.point.Point;

	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class StrainAnalysisEquations3D : StrainAnalysisEquations
	{

		internal override StrainParameter[] initStrainParameters()
		{
			return new StrainParameter[]
			{
				new StrainParameterTranslationX(),
				new StrainParameterTranslationY(),
				new StrainParameterTranslationZ(),
				new StrainParameterQ0(),
				new StrainParameterQ1(),
				new StrainParameterQ2(),
				new StrainParameterQ3(),
				new StrainParameterS11(),
				new StrainParameterS12(),
				new StrainParameterS13(),
				new StrainParameterS22(),
				new StrainParameterS23(),
				new StrainParameterS33()
			};
		}

		public override bool isSupportedRestriction(RestrictionType restriction)
		{
			return restriction == RestrictionType.UNIT_QUATERNION || restriction == RestrictionType.FIXED_TRANSLATION_X || restriction == RestrictionType.FIXED_TRANSLATION_Y || restriction == RestrictionType.FIXED_TRANSLATION_Z || restriction == RestrictionType.FIXED_ROTATION_X || restriction == RestrictionType.FIXED_ROTATION_Y || restriction == RestrictionType.FIXED_ROTATION_Z || restriction == RestrictionType.FIXED_SHEAR_X || restriction == RestrictionType.FIXED_SHEAR_Y || restriction == RestrictionType.FIXED_SHEAR_Z || restriction == RestrictionType.FIXED_SCALE_X || restriction == RestrictionType.FIXED_SCALE_Y || restriction == RestrictionType.FIXED_SCALE_Z || restriction == RestrictionType.IDENT_SCALES_XY || restriction == RestrictionType.IDENT_SCALES_XZ || restriction == RestrictionType.IDENT_SCALES_YZ;
		}

		public override double diff(StrainParameter parameter, Point p1, Equation equation)
		{
			double elmA = 0;

			// Schwerpunkt
			//		double xC1 = this.centerPoints.getX1();
			//		double yC1 = this.centerPoints.getY1();
			//		double zC1 = this.centerPoints.getZ1();

			double x1 = p1.X;
			double y1 = p1.Y;
			double z1 = p1.Z;

			//Schwerpunktreduktion
			//		double xP = x1 - xC1;
			//		double yP = y1 - yC1;
			//		double zP = z1 - zC1;

			double xP = x1;
			double yP = y1;
			double zP = z1;

			// Transformationsparameter
			double q0 = this.getParameterByType(ParameterType.STRAIN_Q0).Value;
			double q1 = this.getParameterByType(ParameterType.STRAIN_Q1).Value;
			double q2 = this.getParameterByType(ParameterType.STRAIN_Q2).Value;
			double q3 = this.getParameterByType(ParameterType.STRAIN_Q3).Value;

			double s11 = this.getParameterByType(ParameterType.STRAIN_S11).Value;
			double s12 = this.getParameterByType(ParameterType.STRAIN_S12).Value;
			double s13 = this.getParameterByType(ParameterType.STRAIN_S13).Value;

			double s22 = this.getParameterByType(ParameterType.STRAIN_S22).Value;
			double s23 = this.getParameterByType(ParameterType.STRAIN_S23).Value;

			double s33 = this.getParameterByType(ParameterType.STRAIN_S33).Value;

			// Rotationsmatrix
			double r11 = 2.0 * q0 * q0 - 1.0 + 2.0 * q1 * q1;
			double r12 = 2.0 * (q1 * q2 - q0 * q3);
			double r13 = 2.0 * (q1 * q3 + q0 * q2);

			double r21 = 2.0 * (q1 * q2 + q0 * q3);
			double r22 = 2.0 * q0 * q0 - 1.0 + 2.0 * q2 * q2;
			double r23 = 2.0 * (q2 * q3 - q0 * q1);

			double r31 = 2.0 * (q1 * q3 - q0 * q2);
			double r32 = 2.0 * (q2 * q3 + q0 * q1);
			double r33 = 2.0 * q0 * q0 - 1.0 + 2.0 * q3 * q3;

			double smxP = s11 * xP + s12 * yP + s13 * zP;
			double smyP = s22 * yP + s23 * zP;
			double smzP = s33 * zP;

			if (equation == Equation.X)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// X0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_X:
					elmA = 1.0;
					break;
					// Y0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_Y:
					elmA = 0.0;
					break;
					// Z0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
					elmA = 0.0;
					break;
					// q0
				case ParameterType.InnerEnum.STRAIN_Q0:
					elmA = 4.0 * q0 * smxP - 2.0 * q3 * smyP + 2.0 * q2 * smzP;
					break;
					// q1
				case ParameterType.InnerEnum.STRAIN_Q1:
					elmA = 4.0 * q1 * smxP + 2.0 * q2 * smyP + 2.0 * q3 * smzP;
					break;
					// q2
				case ParameterType.InnerEnum.STRAIN_Q2:
					elmA = 2.0 * q1 * smyP + 2.0 * q0 * smzP;
					break;
					// q3
				case ParameterType.InnerEnum.STRAIN_Q3:
					elmA = -2.0 * q0 * smyP + 2.0 * q1 * smzP;
					break;
					// s11
				case ParameterType.InnerEnum.STRAIN_S11:
					elmA = r11 * xP;
					break;
					// s12
				case ParameterType.InnerEnum.STRAIN_S12:
					elmA = r11 * yP;
					break;
					// s13
				case ParameterType.InnerEnum.STRAIN_S13:
					elmA = r11 * zP;
					break;
					// s22
				case ParameterType.InnerEnum.STRAIN_S22:
					elmA = r12 * yP;
					break;
					// s23
				case ParameterType.InnerEnum.STRAIN_S23:
					elmA = r12 * zP;
					break;
					// s33
				case ParameterType.InnerEnum.STRAIN_S33:
					elmA = r13 * zP;
					break;
				default:
					elmA = 0.0;
					break;
				}
			}
			else if (equation == Equation.Y)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// X0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_X:
					elmA = 0.0;
					break;
					// Y0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_Y:
					elmA = 1.0;
					break;
					// Z0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
					elmA = 0.0;
					break;
					// q0
				case ParameterType.InnerEnum.STRAIN_Q0:
					elmA = 2.0 * q3 * smxP + 4.0 * q0 * smyP - 2.0 * q1 * smzP;
					break;
					// q1
				case ParameterType.InnerEnum.STRAIN_Q1:
					elmA = 2.0 * q2 * smxP - 2.0 * q0 * smzP;
					break;
					// q2
				case ParameterType.InnerEnum.STRAIN_Q2:
					elmA = 2.0 * q1 * smxP + 4.0 * q2 * smyP + 2.0 * q3 * smzP;
					break;
					// q3
				case ParameterType.InnerEnum.STRAIN_Q3:
					elmA = 2.0 * q0 * smxP + 2.0 * q2 * smzP;
					break;
					// s11
				case ParameterType.InnerEnum.STRAIN_S11:
					elmA = r21 * xP;
					break;
					// s12
				case ParameterType.InnerEnum.STRAIN_S12:
					elmA = r21 * yP;
					break;
					// s13
				case ParameterType.InnerEnum.STRAIN_S13:
					elmA = r21 * zP;
					break;
					// s22
				case ParameterType.InnerEnum.STRAIN_S22:
					elmA = r22 * yP;
					break;
					// s23
				case ParameterType.InnerEnum.STRAIN_S23:
					elmA = r22 * zP;
					break;
					// s33
				case ParameterType.InnerEnum.STRAIN_S33:
					elmA = r23 * zP;
					break;
				default:
					elmA = 0.0;
					break;
				}
			}
			else if (equation == Equation.Z)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// X0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_X:
					elmA = 0.0;
					break;
					// Y0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_Y:
					elmA = 0.0;
					break;
					// Z0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_Z:
					elmA = 1.0;
					break;
					// q0
				case ParameterType.InnerEnum.STRAIN_Q0:
					elmA = -2.0 * q2 * smxP + 2.0 * q1 * smyP + 4.0 * q0 * smzP;
					break;
					// q1
				case ParameterType.InnerEnum.STRAIN_Q1:
					elmA = 2.0 * q3 * smxP + 2.0 * q0 * smyP;
					break;
					// q2
				case ParameterType.InnerEnum.STRAIN_Q2:
					elmA = -2.0 * q0 * smxP + 2.0 * q3 * smyP;
					break;
					// q3
				case ParameterType.InnerEnum.STRAIN_Q3:
					elmA = 2.0 * q1 * smxP + 2.0 * q2 * smyP + 4.0 * q3 * smzP;
					break;
					// s11
				case ParameterType.InnerEnum.STRAIN_S11:
					elmA = r31 * xP;
					break;
					// s12
				case ParameterType.InnerEnum.STRAIN_S12:
					elmA = r31 * yP;
					break;
					// s13
				case ParameterType.InnerEnum.STRAIN_S13:
					elmA = r31 * zP;
					break;
					// s22
				case ParameterType.InnerEnum.STRAIN_S22:
					elmA = r32 * yP;
					break;
					// s23
				case ParameterType.InnerEnum.STRAIN_S23:
					elmA = r32 * zP;
					break;
					// s33
				case ParameterType.InnerEnum.STRAIN_S33:
					elmA = r33 * zP;
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
			double q0 = this.getParameterByType(ParameterType.STRAIN_Q0).Value;
			double q1 = this.getParameterByType(ParameterType.STRAIN_Q1).Value;
			double q2 = this.getParameterByType(ParameterType.STRAIN_Q2).Value;
			double q3 = this.getParameterByType(ParameterType.STRAIN_Q3).Value;

			double s11 = this.getParameterByType(ParameterType.STRAIN_S11).Value;
			double s12 = this.getParameterByType(ParameterType.STRAIN_S12).Value;
			double s13 = this.getParameterByType(ParameterType.STRAIN_S13).Value;

			double s22 = this.getParameterByType(ParameterType.STRAIN_S22).Value;
			double s23 = this.getParameterByType(ParameterType.STRAIN_S23).Value;

			double s33 = this.getParameterByType(ParameterType.STRAIN_S33).Value;

			// Rotationsmatrix
			double r11 = 2.0 * q0 * q0 - 1.0 + 2.0 * q1 * q1;
			double r12 = 2.0 * (q1 * q2 - q0 * q3);
			double r13 = 2.0 * (q1 * q3 + q0 * q2);

			double r21 = 2.0 * (q1 * q2 + q0 * q3);
			double r22 = 2.0 * q0 * q0 - 1.0 + 2.0 * q2 * q2;
			double r23 = 2.0 * (q2 * q3 - q0 * q1);

			double r31 = 2.0 * (q1 * q3 - q0 * q2);
			double r32 = 2.0 * (q2 * q3 + q0 * q1);
			double r33 = 2.0 * q0 * q0 - 1.0 + 2.0 * q3 * q3;

			double elmB = 0.0;
			if (equation == Equation.X)
			{
				switch (component)
				{
				// xS-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.X1:
					elmB = r11 * s11;
					break;
					// yS-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.Y1:
					elmB = r11 * s12 + r12 * s22;
					break;
					// zS-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.Z1:
					elmB = r11 * s13 + r12 * s23 + r13 * s33;
					break;
					// xT-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.X2:
					elmB = -1.0;
					break;
				default:
					elmB = 0.0;
					break;

				}
			}
			else if (equation == Equation.Y)
			{
				switch (component)
				{
				// xS-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.X1:
					elmB = r21 * s11;
					break;
					// yS-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.Y1:
					elmB = r21 * s12 + r22 * s22;
					break;
					// zS-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.Z1:
					elmB = r21 * s13 + r22 * s23 + r23 * s33;
					break;
					// yT-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.Y2:
					elmB = -1.0;
					break;
				default:
					elmB = 0.0;
					break;

				}
			}
			else if (equation == Equation.Z)
			{
				switch (component)
				{
				// xS-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.X1:
					elmB = r31 * s11;
					break;
					// yS-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.Y1:
					elmB = r31 * s12 + r32 * s22;
					break;
					// zS-Wert
				case org.applied_geodesy.adjustment.network.congruence.strain.CoordinateComponent.Z1:
					elmB = r31 * s13 + r32 * s23 + r33 * s33;
					break;
					// zT-Wert
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
			//		double xC1 = this.centerPoints.getX1();
			//		double yC1 = this.centerPoints.getY1();
			//		double zC1 = this.centerPoints.getZ1();
			//
			//		double xC2 = this.centerPoints.getX2();
			//		double yC2 = this.centerPoints.getY2();
			//		double zC2 = this.centerPoints.getZ2();

			double x1 = p1.X;
			double y1 = p1.Y;
			double z1 = p1.Z;

			double x2 = p2.X;
			double y2 = p2.Y;
			double z2 = p2.Z;

			//Schwerpunktreduktion
			//		double xP = x1 - xC1;
			//		double yP = y1 - yC1;
			//		double zP = z1 - zC1;
			//		
			//		double XP = x2 - xC2;
			//		double YP = y2 - yC2;
			//		double ZP = z2 - zC2;

			double xP = x1;
			double yP = y1;
			double zP = z1;

			double XP = x2;
			double YP = y2;
			double ZP = z2;

			// Transformationsparameter
			double x0 = this.getParameterByType(ParameterType.STRAIN_TRANSLATION_X).Value;
			double y0 = this.getParameterByType(ParameterType.STRAIN_TRANSLATION_Y).Value;
			double z0 = this.getParameterByType(ParameterType.STRAIN_TRANSLATION_Z).Value;

			double q0 = this.getParameterByType(ParameterType.STRAIN_Q0).Value;
			double q1 = this.getParameterByType(ParameterType.STRAIN_Q1).Value;
			double q2 = this.getParameterByType(ParameterType.STRAIN_Q2).Value;
			double q3 = this.getParameterByType(ParameterType.STRAIN_Q3).Value;

			double s11 = this.getParameterByType(ParameterType.STRAIN_S11).Value;
			double s12 = this.getParameterByType(ParameterType.STRAIN_S12).Value;
			double s13 = this.getParameterByType(ParameterType.STRAIN_S13).Value;

			double s22 = this.getParameterByType(ParameterType.STRAIN_S22).Value;
			double s23 = this.getParameterByType(ParameterType.STRAIN_S23).Value;

			double s33 = this.getParameterByType(ParameterType.STRAIN_S33).Value;

			// Rotationsmatrix
			double r11 = 2.0 * q0 * q0 - 1.0 + 2.0 * q1 * q1;
			double r12 = 2.0 * (q1 * q2 - q0 * q3);
			double r13 = 2.0 * (q1 * q3 + q0 * q2);

			double r21 = 2.0 * (q1 * q2 + q0 * q3);
			double r22 = 2.0 * q0 * q0 - 1.0 + 2.0 * q2 * q2;
			double r23 = 2.0 * (q2 * q3 - q0 * q1);

			double r31 = 2.0 * (q1 * q3 - q0 * q2);
			double r32 = 2.0 * (q2 * q3 + q0 * q1);
			double r33 = 2.0 * q0 * q0 - 1.0 + 2.0 * q3 * q3;

			double smxP = s11 * xP + s12 * yP + s13 * zP;
			double smyP = s22 * yP + s23 * zP;
			double smzP = s33 * zP;

			if (equation == Equation.X)
			{
				return (x0 + (r11 * smxP + r12 * smyP + r13 * smzP)) - XP;
			}
			else if (equation == Equation.Y)
			{
				return (y0 + (r21 * smxP + r22 * smyP + r23 * smzP)) - YP;
			}
			return (z0 + (r31 * smxP + r32 * smyP + r33 * smzP)) - ZP;
		}

		public override double diff(StrainParameter parameter, RestrictionType restriction)
		{
			double elmR = 0.0;

			// Transformationsparameter
			double q0 = this.getParameterByType(ParameterType.STRAIN_Q0).Value;
			double q1 = this.getParameterByType(ParameterType.STRAIN_Q1).Value;
			double q2 = this.getParameterByType(ParameterType.STRAIN_Q2).Value;
			double q3 = this.getParameterByType(ParameterType.STRAIN_Q3).Value;

			double s11 = this.getParameterByType(ParameterType.STRAIN_S11).Value;
			double s12 = this.getParameterByType(ParameterType.STRAIN_S12).Value;
			double s13 = this.getParameterByType(ParameterType.STRAIN_S13).Value;

			double s22 = this.getParameterByType(ParameterType.STRAIN_S22).Value;
			double s23 = this.getParameterByType(ParameterType.STRAIN_S23).Value;

			double s33 = this.getParameterByType(ParameterType.STRAIN_S33).Value;

			if (restriction == RestrictionType.UNIT_QUATERNION)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// q0
				case ParameterType.InnerEnum.STRAIN_Q0:
					elmR = 2.0 * q0;
					break;
					// q1
				case ParameterType.InnerEnum.STRAIN_Q1:
					elmR = 2.0 * q1;
					break;
					// q2
				case ParameterType.InnerEnum.STRAIN_Q2:
					elmR = 2.0 * q2;
					break;
					// q3
				case ParameterType.InnerEnum.STRAIN_Q3:
					elmR = 2.0 * q3;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_TRANSLATION_X)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// x0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_X:
					elmR = 1.0;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_TRANSLATION_Y)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// y0
				case ParameterType.InnerEnum.STRAIN_TRANSLATION_Y:
					elmR = 1.0;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_TRANSLATION_Z)
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
			else if (restriction == RestrictionType.FIXED_ROTATION_X)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// q0
				case ParameterType.InnerEnum.STRAIN_Q0:
					elmR = -q1;
					break;
					// q1
				case ParameterType.InnerEnum.STRAIN_Q1:
					elmR = -q0;
					break;
					// q2
				case ParameterType.InnerEnum.STRAIN_Q2:
					elmR = q3;
					break;
					// q3
				case ParameterType.InnerEnum.STRAIN_Q3:
					elmR = q2;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_ROTATION_Y)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// q0
				case ParameterType.InnerEnum.STRAIN_Q0:
					elmR = q2;
					break;
					// q1
				case ParameterType.InnerEnum.STRAIN_Q1:
					elmR = q3;
					break;
					// q2
				case ParameterType.InnerEnum.STRAIN_Q2:
					elmR = q0;
					break;
					// q3
				case ParameterType.InnerEnum.STRAIN_Q3:
					elmR = q1;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_ROTATION_Z)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// q0
				case ParameterType.InnerEnum.STRAIN_Q0:
					elmR = -q3;
					break;
					// q1
				case ParameterType.InnerEnum.STRAIN_Q1:
					elmR = q2;
					break;
					// q2
				case ParameterType.InnerEnum.STRAIN_Q2:
					elmR = q1;
					break;
					// q3
				case ParameterType.InnerEnum.STRAIN_Q3:
					elmR = -q0;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_SCALE_X)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// s11
				case ParameterType.InnerEnum.STRAIN_S11:
					elmR = 1.0;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_SCALE_Y)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// s12
				case ParameterType.InnerEnum.STRAIN_S12:
					elmR = 2.0 * s12;
					break;
					// s22
				case ParameterType.InnerEnum.STRAIN_S22:
					elmR = 2.0 * s22;
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
				// s13
				case ParameterType.InnerEnum.STRAIN_S13:
					elmR = 2.0 * s13;
					break;
					// s23
				case ParameterType.InnerEnum.STRAIN_S23:
					elmR = 2.0 * s23;
					break;
					// s33
				case ParameterType.InnerEnum.STRAIN_S33:
					elmR = 2.0 * s33;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_SHEAR_X)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// s23
				case ParameterType.InnerEnum.STRAIN_S23:
					elmR = 1.0;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_SHEAR_Y)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// s13
				case ParameterType.InnerEnum.STRAIN_S13:
					elmR = 1.0;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.FIXED_SHEAR_Z)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// s12
				case ParameterType.InnerEnum.STRAIN_S12:
					elmR = 1.0;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.IDENT_SCALES_XY)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// s11
				case ParameterType.InnerEnum.STRAIN_S11:
					elmR = -2.0 * s11;
					break;
					// s12
				case ParameterType.InnerEnum.STRAIN_S12:
					elmR = 2.0 * s12;
					break;
					// s22
				case ParameterType.InnerEnum.STRAIN_S22:
					elmR = 2.0 * s22;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.IDENT_SCALES_XZ)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// s11
				case ParameterType.InnerEnum.STRAIN_S11:
					elmR = -2.0 * s11;
					break;
					// s13
				case ParameterType.InnerEnum.STRAIN_S13:
					elmR = 2.0 * s13;
					break;
					// s23
				case ParameterType.InnerEnum.STRAIN_S23:
					elmR = 2.0 * s23;
					break;
					// s33
				case ParameterType.InnerEnum.STRAIN_S33:
					elmR = 2.0 * s33;
					break;
				default:
					elmR = 0.0;
					break;
				}
			}
			else if (restriction == RestrictionType.IDENT_SCALES_YZ)
			{
				switch (parameter.ParameterType.innerEnumValue)
				{
				// s12
				case ParameterType.InnerEnum.STRAIN_S12:
					elmR = -2.0 * s12;
					break;
					// s22
				case ParameterType.InnerEnum.STRAIN_S22:
					elmR = -2.0 * s22;
					break;

					// s13
				case ParameterType.InnerEnum.STRAIN_S13:
					elmR = 2.0 * s13;
					break;
					// s23
				case ParameterType.InnerEnum.STRAIN_S23:
					elmR = 2.0 * s23;
					break;
					// s33
				case ParameterType.InnerEnum.STRAIN_S33:
					elmR = 2.0 * s33;
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
			double x0 = this.getParameterByType(ParameterType.STRAIN_TRANSLATION_X).Value;
			double y0 = this.getParameterByType(ParameterType.STRAIN_TRANSLATION_Y).Value;
			double z0 = this.getParameterByType(ParameterType.STRAIN_TRANSLATION_Z).Value;

			double q0 = this.getParameterByType(ParameterType.STRAIN_Q0).Value;
			double q1 = this.getParameterByType(ParameterType.STRAIN_Q1).Value;
			double q2 = this.getParameterByType(ParameterType.STRAIN_Q2).Value;
			double q3 = this.getParameterByType(ParameterType.STRAIN_Q3).Value;

			double s11 = this.getParameterByType(ParameterType.STRAIN_S11).Value;
			double s12 = this.getParameterByType(ParameterType.STRAIN_S12).Value;
			double s13 = this.getParameterByType(ParameterType.STRAIN_S13).Value;

			double s22 = this.getParameterByType(ParameterType.STRAIN_S22).Value;
			double s23 = this.getParameterByType(ParameterType.STRAIN_S23).Value;

			double s33 = this.getParameterByType(ParameterType.STRAIN_S33).Value;

			switch (restriction.innerEnumValue)
			{
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.UNIT_QUATERNION:
				return (q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3) - 1.0;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_TRANSLATION_X:
				return x0;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_TRANSLATION_Y:
				return y0;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_TRANSLATION_Z:
				return z0;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_ROTATION_X:
				return q2 * q3 - q0 * q1;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_ROTATION_Y:
				return q1 * q3 + q0 * q2;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_ROTATION_Z:
				return q1 * q2 - q0 * q3;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_SCALE_X:
				return s11 - 1.0;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_SCALE_Y:
				return s12 * s12 + s22 * s22 - 1.0;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_SCALE_Z:
				return s13 * s13 + s23 * s23 + s33 * s33 - 1.0;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_SHEAR_X:
				return s23;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_SHEAR_Y:
				return s13;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.FIXED_SHEAR_Z:
				return s12;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.IDENT_SCALES_XY:
				return s12 * s12 + s22 * s22 - s11 * s11;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.IDENT_SCALES_XZ:
				return s13 * s13 + s23 * s23 + s33 * s33 - s11 * s11;
			case org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType.InnerEnum.IDENT_SCALES_YZ:
				return s13 * s13 + s23 * s23 + s33 * s33 - s12 * s12 - s22 * s22;
			default:
				return 0;
			}
		}

		internal override void initDefaultRestictions()
		{
			this.addRestriction(RestrictionType.UNIT_QUATERNION);
		}

		public override void expandParameters(double sigma2apost, Matrix Quu, bool applyAposterioriVarianceOfUnitWeight)
		{
			int addPar = this.numberOfExpandedParameters();
			// Transformationsparameter
			double q0 = this.getParameterByType(ParameterType.STRAIN_Q0).Value;
			double q1 = this.getParameterByType(ParameterType.STRAIN_Q1).Value;
			double q2 = this.getParameterByType(ParameterType.STRAIN_Q2).Value;
			double q3 = this.getParameterByType(ParameterType.STRAIN_Q3).Value;

			double s11 = this.getParameterByType(ParameterType.STRAIN_S11).Value;
			double s12 = this.getParameterByType(ParameterType.STRAIN_S12).Value;
			double s13 = this.getParameterByType(ParameterType.STRAIN_S13).Value;

			double s22 = this.getParameterByType(ParameterType.STRAIN_S22).Value;
			double s23 = this.getParameterByType(ParameterType.STRAIN_S23).Value;

			double s33 = this.getParameterByType(ParameterType.STRAIN_S33).Value;

			// Rotationsmatrix
			double r11 = 2.0 * q0 * q0 - 1.0 + 2.0 * q1 * q1;
			double r12 = 2.0 * (q1 * q2 - q0 * q3);
			double r13 = 2.0 * (q1 * q3 + q0 * q2);

			double r23 = 2.0 * (q2 * q3 - q0 * q1);

			double r33 = 2.0 * q0 * q0 - 1.0 + 2.0 * q3 * q3;

			double mx = Math.Abs(s11);
			double my = Math.hypot(s12, s22);
			double mz = Math.Sqrt(s13 * s13 + s23 * s23 + s33 * s33);

			double rx = MathExtension.MOD(r33 != 0 ? Math.Atan(r23 / r33) : 0.0, 2.0 * Math.PI);
			double ry = MathExtension.MOD(Math.Asin(-r13), 2.0 * Math.PI);
			double rz = MathExtension.MOD(r11 != 0 ? Math.Atan(r12 / r11) : 0.0, 2.0 * Math.PI);

			double sx = MathExtension.MOD(s33 != 0 ? Math.Atan(-s23 / s33) : 0.0, 2.0 * Math.PI);
			double sy = MathExtension.MOD(Math.hypot(s23, s33) > 0.0 ? Math.Atan(-s13 / Math.hypot(s23, s33)) : 0.0, 2.0 * Math.PI);
			double sz = MathExtension.MOD(s22 != 0 ? Math.Atan(-s12 / s22) : 0.0, 2.0 * Math.PI);

			int nou = this.numberOfParameters();
			Matrix A = new DenseMatrix(nou + addPar, nou);

			for (int i = 0; i < nou; i++)
			{
				A.set(i,i, 1.0);
			}

			// mx
			A.set(nou, 7, 1.0);

			// my
			A.set(nou + 1, 8, s12 / my);
			A.set(nou + 1, 10, s22 / my);

			// mz
			A.set(nou + 2, 9, s13 / mz);
			A.set(nou + 2, 11, s23 / mz);
			A.set(nou + 2, 12, s33 / mz);

			// rx
			A.set(nou + 3, 3, -2.0 * (q1 * r33 + 2.0 * r23 * q0) / (r33 * r33 + r23 * r23));
			A.set(nou + 3, 4, -2.0 * q0 * r33 / (r33 * r33 + r23 * r23));
			A.set(nou + 3, 5, 2.0 * q3 * r33 / (r33 * r33 + r23 * r23));
			A.set(nou + 3, 6, 2.0 * (q2 * r33 - 2.0 * r23 * q3) / (r33 * r33 + r23 * r23));

			// ry
			A.set(nou + 4, 3, -2.0 * q2 / Math.Sqrt(1.0 - (r13 * r13)));
			A.set(nou + 4, 4, -2.0 * q3 / Math.Sqrt(1.0 - (r13 * r13)));
			A.set(nou + 4, 5, -2.0 * q0 / Math.Sqrt(1.0 - (r13 * r13)));
			A.set(nou + 4, 6, -2.0 * q1 / Math.Sqrt(1.0 - (r13 * r13)));

			// rz
			A.set(nou + 5, 3, -2.0 * (q3 * r11 + 2.0 * r12 * q0) / (r11 * r11 + r12 * r12));
			A.set(nou + 5, 4, 2.0 * (q2 * r11 - 2.0 * r12 * q1) / (r11 * r11 + r12 * r12));
			A.set(nou + 5, 5, 2.0 * q1 * r11 / (r11 * r11 + r12 * r12));
			A.set(nou + 5, 6, -2.0 * q0 * r11 / (r11 * r11 + r12 * r12));

			// sx
			A.set(nou + 6, 11, -s33 / (s33 * s33 + s23 * s23));
			A.set(nou + 6, 12, s23 / (s33 * s33 + s23 * s23));

			// sy
			if ((s23 * s23 + s33 * s33 + s13 * s13) > 0)
			{
				double d1 = Math.hypot(s23, s33);
				double d2 = s23 * s23 + s33 * s33 + s13 * s13;

				A.set(nou + 7, 9, -d1 / d2);
				if (d1 > 0)
				{
					A.set(nou + 7, 11, s13 / d1 * s23 / d2);
					A.set(nou + 7, 12, s13 / d1 * s33 / d2);
				}
			}

			// sz
			A.set(nou + 8, 8, -s22 / (s22 * s22 + s12 * s12));
			A.set(nou + 8, 10, s12 / (s22 * s22 + s12 * s12));


			Matrix AQuu = new DenseMatrix(nou + addPar, nou);
			A.mult(Quu, AQuu);
			Matrix AQuuAT = new UpperSymmPackMatrix(nou + addPar);
			AQuu.transBmult(A, AQuuAT);

			StrainParameterScaleX scaleX = new StrainParameterScaleX(mx);
			StrainParameterScaleY scaleY = new StrainParameterScaleY(my);
			StrainParameterScaleZ scaleZ = new StrainParameterScaleZ(mz);

			StrainParameterRotationX rotatX = new StrainParameterRotationX(rx);
			StrainParameterRotationY rotatY = new StrainParameterRotationY(ry);
			StrainParameterRotationZ rotatZ = new StrainParameterRotationZ(rz);

			StrainParameterShearX shearX = new StrainParameterShearX(sx);
			StrainParameterShearY shearY = new StrainParameterShearY(sy);
			StrainParameterShearZ shearZ = new StrainParameterShearZ(sz);

			StrainParameter[] strainParameters = new StrainParameter[nou + addPar];
			Array.Copy(this.strainParameters, 0, strainParameters, 0, nou);
			strainParameters[nou + 0] = scaleX;
			strainParameters[nou + 1] = scaleY;
			strainParameters[nou + 2] = scaleZ;

			strainParameters[nou + 3] = rotatX;
			strainParameters[nou + 4] = rotatY;
			strainParameters[nou + 5] = rotatZ;

			strainParameters[nou + 6] = shearX;
			strainParameters[nou + 7] = shearY;
			strainParameters[nou + 8] = shearZ;

			this.strainParameters = strainParameters;

			for (int i = 0; i < this.strainParameters.Length; i++)
			{
				StrainParameter param = this.strainParameters[i];
				this.setStochasticParameters(param, sigma2apost, AQuuAT.get(i, i), applyAposterioriVarianceOfUnitWeight);
			}
		}

		public override int numberOfExpandedParameters()
		{
			return 9;
		}
	}

}