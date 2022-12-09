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

	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using EllipseFeature = org.applied_geodesy.adjustment.geometry.curve.EllipseFeature;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;

	public class EllipseTest : NISTTest
	{
		private EllipseTest()
		{
		}

		internal override Feature Feature
		{
			get
			{
				return new EllipseFeature();
			}
		}

		internal override void compare(IList<double> referenceResults, IList<UnknownParameter> unknownParameters)
		{
	//		Circles – 7 numbers
	//		3 numbers represent the center of the circle
	//		3 numbers represent the direction cosines of the normal of the plane containing
	//		the circle
	//		1 number represents the diameter of the circle

			int idxX = 0;
			int idxY = 1;

			if (CMP_TYPE == ComponentOrderType.XZ)
			{
				idxX = 0;
				idxY = 2;
			}
			else if (CMP_TYPE == ComponentOrderType.YZ)
			{
				idxX = 1;
				idxY = 2;
			}

			double x0Ref = referenceResults[idxX];
			double y0Ref = referenceResults[idxY];

			double rRef = 0.5 * referenceResults[6];

			double[] references = new double[] {x0Ref, y0Ref, rRef, rRef};

			IList<ParameterType> types = new List<ParameterType> {ParameterType.ORIGIN_COORDINATE_X, ParameterType.ORIGIN_COORDINATE_Y, ParameterType.MAJOR_AXIS_COEFFICIENT, ParameterType.MINOR_AXIS_COEFFICIENT};

			for (int i = 0; i < types.Count; i++)
			{
				foreach (UnknownParameter unknownParameter in unknownParameters)
				{
					if (unknownParameter.ParameterType == types[i])
					{
						double diff = Math.Abs(unknownParameter.Value) - Math.Abs(references[i]);
						Console.WriteLine(String.format(Locale.ENGLISH, TEMPLATE, unknownParameter.ParameterType, unknownParameter.Value, references[i], diff, Math.Abs(diff) > EPS ? "***" : ""));
						break;
					}
				}
			}
		}

		internal override int Dimension
		{
			get
			{
				return 2;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void main(String[] args) throws Exception
		public static void Main(string[] args)
		{
			System.setProperty("com.github.fommil.netlib.BLAS", "com.github.fommil.netlib.F2jBLAS");
			System.setProperty("com.github.fommil.netlib.LAPACK", "com.github.fommil.netlib.F2jLAPACK");
			System.setProperty("com.github.fommil.netlib.ARPACK", "com.github.fommil.netlib.F2jARPACK");

			NISTTest test = new EllipseTest();
			test.start("./nist/Circle2d/");
		}
	}

}