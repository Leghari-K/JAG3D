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

namespace org.applied_geodesy.juniform.test
{

	using Constant = org.applied_geodesy.adjustment.Constant;
	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using CircularConeFeature = org.applied_geodesy.adjustment.geometry.surface.CircularConeFeature;

	public class CircularConeTest : NISTTest
	{
		private CircularConeTest()
		{
		}

		internal override void compare(IList<double> referenceResults, IList<UnknownParameter> unknownParameters)
		{
	//		Cones – 8 numbers
	//		3 numbers represent a point on the cone axis
	//		3 numbers represent the direction cosines of the cone axis
	//		1 number represents the orthogonal distance from the reported point on the axis to
	//		the surface of the cone.
	//		1 number represents the full apex angle of the cone in degrees (less than 180)

			double x0Ref = referenceResults[0];
			double y0Ref = referenceResults[1];
			double z0Ref = referenceResults[2];

			double nxRef = referenceResults[3];
			double nyRef = referenceResults[4];
			double nzRef = referenceResults[5];

			double rRef = referenceResults[6];
			double phiRef = 0.5 * referenceResults[7] * Constant.RHO_DEG2RAD;

			double s = -rRef / Math.Sin(phiRef);

			double xApexRef = x0Ref + s * nxRef;
			double yApexRef = y0Ref + s * nyRef;
			double zApexRef = z0Ref + s * nzRef;

			double dRef = nxRef * x0Ref + nyRef * y0Ref + nzRef * z0Ref;

			// position closest to the origin
			x0Ref = x0Ref - dRef * nxRef;
			y0Ref = y0Ref - dRef * nyRef;
			z0Ref = z0Ref - dRef * nzRef;

			double[] references = new double[] {x0Ref, y0Ref, z0Ref, xApexRef, yApexRef, zApexRef, nxRef, nyRef, nzRef, phiRef, 1.0 / Math.Tan(phiRef)};

			double xApex = 0, yApex = 0, zApex = 0, x0 = 0, y0 = 0, z0 = 0, nx = 0, ny = 0, nz = 0, phi = 0, b = 0;
			foreach (UnknownParameter unknownParameter in unknownParameters)
			{
				if (unknownParameter.ParameterType == ParameterType.ORIGIN_COORDINATE_X)
				{
					xApex = unknownParameter.Value;
				}
				else if (unknownParameter.ParameterType == ParameterType.ORIGIN_COORDINATE_Y)
				{
					yApex = unknownParameter.Value;
				}
				else if (unknownParameter.ParameterType == ParameterType.ORIGIN_COORDINATE_Z)
				{
					zApex = unknownParameter.Value;
				}
				else if (unknownParameter.ParameterType == ParameterType.ROTATION_COMPONENT_R31)
				{
					nx = unknownParameter.Value;
				}
				else if (unknownParameter.ParameterType == ParameterType.ROTATION_COMPONENT_R32)
				{
					ny = unknownParameter.Value;
				}
				else if (unknownParameter.ParameterType == ParameterType.ROTATION_COMPONENT_R33)
				{
					nz = unknownParameter.Value;
				}
				else if (unknownParameter.ParameterType == ParameterType.ANGLE)
				{
					phi = unknownParameter.Value;
				}
				else if (unknownParameter.ParameterType == ParameterType.MAJOR_AXIS_COEFFICIENT)
				{
					b = unknownParameter.Value;
				}
			}

			double d = nx * xApex + ny * yApex + nz * zApex;

			// position closest to the origin
			x0 = xApex - d * nx;
			y0 = yApex - d * ny;
			z0 = zApex - d * nz;

			double[] solution = new double[] {x0, y0, z0, xApex, yApex, zApex, nx, ny, nz, phi, b};

			IList<ParameterType> types = new List<ParameterType> {ParameterType.COORDINATE_X, ParameterType.COORDINATE_Y, ParameterType.COORDINATE_Z, ParameterType.ORIGIN_COORDINATE_X, ParameterType.ORIGIN_COORDINATE_Y, ParameterType.ORIGIN_COORDINATE_Z, ParameterType.ROTATION_COMPONENT_R31, ParameterType.ROTATION_COMPONENT_R32, ParameterType.ROTATION_COMPONENT_R33, ParameterType.ANGLE, ParameterType.MAJOR_AXIS_COEFFICIENT};

			for (int i = 0; i < types.Count; i++)
			{
				double diff = Math.Abs(solution[i]) - Math.Abs(references[i]);
				Console.WriteLine(String.format(Locale.ENGLISH, TEMPLATE, types[i], solution[i], references[i], diff, Math.Abs(diff) > EPS ? "***" : ""));
			}
		}

		internal override Feature Feature
		{
			get
			{
				return new CircularConeFeature();
			}
		}

		internal override int Dimension
		{
			get
			{
				return 3;
			}
		}

		internal override double Lambda
		{
			get
			{
				return 10.0;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void main(String[] args) throws Exception
		public static void Main(string[] args)
		{
			System.setProperty("com.github.fommil.netlib.BLAS", "com.github.fommil.netlib.F2jBLAS");
			System.setProperty("com.github.fommil.netlib.LAPACK", "com.github.fommil.netlib.F2jLAPACK");
			System.setProperty("com.github.fommil.netlib.ARPACK", "com.github.fommil.netlib.F2jARPACK");

			NISTTest test = new CircularConeTest();
			test.start("./nist/Cone/");
		}
	}

}