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
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using SpatialLineFeature = org.applied_geodesy.adjustment.geometry.surface.SpatialLineFeature;

	public class SpatialLineTest : NISTTest
	{
		private SpatialLineTest()
		{
		}

		internal override void compare(IList<double> referenceResults, IList<UnknownParameter> unknownParameters)
		{
	//		Lines – 6 numbers
	//		3 numbers represent a point on the line
	//		3 numbers represent the direction cosines of the line

			double x0Ref = referenceResults[0];
			double y0Ref = referenceResults[1];
			double z0Ref = referenceResults[2];

			double nxRef = referenceResults[3];
			double nyRef = referenceResults[4];
			double nzRef = referenceResults[5];

			double d = nxRef * x0Ref + nyRef * y0Ref + nzRef * z0Ref;

			// position closest to the origin
			x0Ref = x0Ref - d * nxRef;
			y0Ref = y0Ref - d * nyRef;
			z0Ref = z0Ref - d * nzRef;

			double[] references = new double[] {x0Ref, y0Ref, z0Ref, nxRef, nyRef, nzRef};

			IList<ParameterType> types = new List<ParameterType> {ParameterType.ORIGIN_COORDINATE_X, ParameterType.ORIGIN_COORDINATE_Y, ParameterType.ORIGIN_COORDINATE_Z, ParameterType.VECTOR_X, ParameterType.VECTOR_Y, ParameterType.VECTOR_Z};

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

		internal override Feature Feature
		{
			get
			{
				return new SpatialLineFeature();
			}
		}

		internal override int Dimension
		{
			get
			{
				return 3;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void main(String[] args) throws Exception
		public static void Main(string[] args)
		{
			System.setProperty("com.github.fommil.netlib.BLAS", "com.github.fommil.netlib.F2jBLAS");
			System.setProperty("com.github.fommil.netlib.LAPACK", "com.github.fommil.netlib.F2jLAPACK");
			System.setProperty("com.github.fommil.netlib.ARPACK", "com.github.fommil.netlib.F2jARPACK");

			NISTTest test2d = new SpatialLineTest();
			test2d.start("./nist/Line2d/");

			NISTTest test3d = new SpatialLineTest();
			test3d.start("./nist/Line3d/");
		}
	}

}