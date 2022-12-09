using System;
using System.Collections.Generic;
using System.IO;

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
	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using FeatureAdjustment = org.applied_geodesy.adjustment.geometry.FeatureAdjustment;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;

	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;

	public abstract class NISTTest
	{
		// https://www.nist.gov/pml/sensor-science/dimensional-metrology/algorithm-testing

		public enum ComponentOrderType
		{
			XY,
			XZ,
			YZ,
			XYZ
		}

		internal static readonly double EPS = Math.Sqrt(Constant.EPS);
		internal const string TEMPLATE = "%30s EST = %+25.16f REF = %+25.16f  DIFF = %+6.3e %s";
		internal static ComponentOrderType CMP_TYPE = ComponentOrderType.XYZ;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static java.util.List<double> readFittingResults(java.nio.file.Path path) throws IOException, NullPointerException, NumberFormatException
		public static IList<double> readFittingResults(Path path)
		{
			IList<double> values = new List<double>();
			using (StreamReader reader = Files.newBufferedReader(path, Charset.forName("UTF-8")))
			{
				string currentLine = null;
				while (!string.ReferenceEquals((currentLine = reader.ReadLine()), null))
				{
					double value = double.Parse(currentLine.Trim());
					values.Add(value);
				}
			}
			return values;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static java.util.List<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> readCoordinates(java.nio.file.Path path, int dim) throws IOException, NullPointerException, NumberFormatException
		public static IList<FeaturePoint> readCoordinates(Path path, int dim)
		{
			IList<FeaturePoint> points = new List<FeaturePoint>();

			if (dim == 3)
			{
				CMP_TYPE = ComponentOrderType.XYZ;
			}

			int cnt = 1;

			using (StreamReader reader = Files.newBufferedReader(path, Charset.forName("UTF-8")))
			{
				string currentLine = null;
				while (!string.ReferenceEquals((currentLine = reader.ReadLine()), null))
				{
					string[] data = currentLine.Split("\\s+", true);
					if (data.Length >= dim)
					{

						double x = double.Parse(data[0]);
						double y = double.Parse(data[1]);
						double z = double.Parse(data[2]);

						FeaturePoint point = new FeaturePoint(cnt.ToString(), x, y, z);

						if (point != null)
						{
							points.Add(point);
						}
					}
				}

				if (dim == 2)
				{
					IList<FeaturePoint> points2d = new List<FeaturePoint>(points.Count);
					FeaturePoint firstPoint = points[0];
					FeaturePoint lastPoint = points[points.Count - 1];

					if (Math.Abs(firstPoint.X0 - lastPoint.X0) < Math.Abs(firstPoint.Y0 - lastPoint.Y0) && Math.Abs(firstPoint.X0 - lastPoint.X0) < Math.Abs(firstPoint.Z0 - lastPoint.Z0))
					{
						CMP_TYPE = ComponentOrderType.YZ;
					}

					if (Math.Abs(firstPoint.Y0 - lastPoint.Y0) < Math.Abs(firstPoint.X0 - lastPoint.X0) && Math.Abs(firstPoint.Y0 - lastPoint.Y0) < Math.Abs(firstPoint.Z0 - lastPoint.Z0))
					{
						CMP_TYPE = ComponentOrderType.XZ;
					}

					if (Math.Abs(firstPoint.Z0 - lastPoint.Z0) < Math.Abs(firstPoint.X0 - lastPoint.X0) && Math.Abs(firstPoint.Z0 - lastPoint.Z0) < Math.Abs(firstPoint.Y0 - lastPoint.Y0))
					{
						CMP_TYPE = ComponentOrderType.XY;
					}

					foreach (FeaturePoint point3d in points)
					{
						if (CMP_TYPE == ComponentOrderType.YZ)
						{
							points2d.Add(new FeaturePoint(point3d.Name, point3d.Y0, point3d.Z0));
						}
						else if (CMP_TYPE == ComponentOrderType.XZ)
						{
							points2d.Add(new FeaturePoint(point3d.Name, point3d.X0, point3d.Z0));
						}
						else if (CMP_TYPE == ComponentOrderType.XY)
						{
							points2d.Add(new FeaturePoint(point3d.Name, point3d.X0, point3d.Y0));
						}
					}

					points = points2d;
				}
			}
			return points;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void start(String directory) throws Exception
		public virtual void start(string directory)
		{
			using (DirectoryStream<Path> directoryStream = Files.newDirectoryStream(Paths.get(directory), "*.ds"))
			{
				foreach (Path pointPath in directoryStream)
				{
					string pathName = pointPath.ToString();
					int pos = pathName.LastIndexOf(".", StringComparison.Ordinal);
					if (pos > 0 && pos < (pathName.Length - 1))
					{
						pathName = pathName.Substring(0, pos);
					}
					Path resultPath = Paths.get(pathName + ".fit");

					Console.WriteLine("\n==========================\n");
					Console.WriteLine(pointPath.getFileName());

					IList<FeaturePoint> featurePoints = readCoordinates(pointPath, this.Dimension);
					IList<double> referenceResults = readFittingResults(resultPath);

					Feature feature = this.adjust(featurePoints);
					if (feature == null)
					{
						Console.Error.WriteLine("Adjustment failed!");
						continue;
					}

					IList<UnknownParameter> unknownParameters = feature.UnknownParameters;

					this.compare(referenceResults, unknownParameters);
				}
			}
		}

		public virtual Feature adjust(IList<FeaturePoint> points)
		{
			FeatureAdjustment adjustment = new FeatureAdjustment();

			Feature feature = this.Feature;

			foreach (GeometricPrimitive geometricPrimitive in feature)
			{
				geometricPrimitive.FeaturePoints.addAll(points);
			}


			try
			{
				// derive parameters for warm start of adjustment
				if (feature.EstimateInitialGuess)
				{
					feature.deriveInitialGuess();
				}
				adjustment.LevenbergMarquardtDampingValue = this.Lambda;
				adjustment.Feature = feature;
				adjustment.init();
				EstimationStateType type = adjustment.estimateModel();

				if (type == EstimationStateType.ERROR_FREE_ESTIMATION)
				{
					return feature;
				}

			}
			catch (Exception e) when (e is MatrixSingularException || e is System.ArgumentException || e is System.NotSupportedException || e is NotConvergedException)
			{
				e.printStackTrace();
			}

			return null;
		}

		internal virtual double Lambda
		{
			get
			{
				return 0.0;
			}
		}

		internal abstract int Dimension {get;}

		internal abstract Feature Feature {get;}

		internal abstract void compare(IList<double> referenceResults, IList<UnknownParameter> unknownParameters);
	}

}