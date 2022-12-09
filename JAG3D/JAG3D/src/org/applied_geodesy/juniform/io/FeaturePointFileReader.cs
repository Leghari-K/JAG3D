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

namespace org.applied_geodesy.juniform.io
{

	using FeatureType = org.applied_geodesy.adjustment.geometry.FeatureType;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using org.applied_geodesy.util;
	using org.applied_geodesy.util.io;

	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using UpperSymmBandMatrix = no.uib.cipr.matrix.UpperSymmBandMatrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class FeaturePointFileReader : SourceFileReader<ObservableUniqueList<FeaturePoint>>
	{
		private readonly FeatureType featureType;
		private ObservableUniqueList<FeaturePoint> points;

		public FeaturePointFileReader(FeatureType featureType)
		{
			this.featureType = featureType;
			this.reset();
		}

		public FeaturePointFileReader(string fileName, FeatureType featureType) : this((new File(fileName)).toPath(), featureType)
		{
		}

		public FeaturePointFileReader(File sf, FeatureType featureType) : this(sf.toPath(), featureType)
		{
		}

		public FeaturePointFileReader(Path path, FeatureType featureType) : base(path)
		{
			this.featureType = featureType;
			this.reset();
		}

		public virtual FeatureType FeatureType
		{
			get
			{
				return this.featureType;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public org.applied_geodesy.util.ObservableUniqueList<org.applied_geodesy.adjustment.geometry.point.FeaturePoint> readAndImport() throws IOException, java.sql.SQLException
		public override ObservableUniqueList<FeaturePoint> readAndImport()
		{
			this.ignoreLinesWhichStartWith("#");
			base.read();
			if (this.Interrupted)
			{
				this.points.clear();
			}
			return this.points;
		}

		public override void reset()
		{
			if (this.points == null)
			{
				 this.points = new ObservableUniqueList<FeaturePoint>(10000);
			}
			this.points.clear();
		}

		public override void parse(string line)
		{
			line = line.Trim();
			FeaturePoint point = null;
			switch (this.featureType)
			{
			case FeatureType.CURVE:
				point = scanCurvePoint(line);
				break;
			case FeatureType.SURFACE:
				point = scanSurfacePoint(line);
				break;
			}

			if (point != null)
			{
				this.points.add(point);
			}
		}

		private static FeaturePoint scanCurvePoint(string str)
		{
			FormatterOptions options = FormatterOptions.Instance;
			string[] columns = str.Trim().Split("[\\s;]+", true);

			if (columns.Length < 3)
			{
				return null;
			}

			string name = columns[0];
			double x = options.convertLengthToModel(double.Parse(columns[1].Replace(',', '.')));
			double y = options.convertLengthToModel(double.Parse(columns[2].Replace(',', '.')));

			FeaturePoint point = new FeaturePoint(name, x, y);
			if (columns.Length < 4)
			{
				return point;
			}

			double sigmaX, sigmaY;
			sigmaX = sigmaY = options.convertLengthToModel(double.Parse(columns[3].Replace(',', '.')));

			if (columns.Length < 5)
			{
				if (sigmaX <= 0 || sigmaY <= 0)
				{
					return point;
				}

				Matrix dispersion = new UpperSymmBandMatrix(point.Dimension, 0);
				dispersion.set(0, 0, sigmaX * sigmaX);
				dispersion.set(1, 1, sigmaY * sigmaY);
				point.DispersionApriori = dispersion;
				return point;
			}

			sigmaY = options.convertLengthToModel(double.Parse(columns[4].Replace(',', '.')));

			if (columns.Length < 6)
			{
				if (sigmaX <= 0 || sigmaY <= 0)
				{
					return point;
				}

				Matrix dispersion = new UpperSymmBandMatrix(point.Dimension, 0);
				dispersion.set(0, 0, sigmaX * sigmaX);
				dispersion.set(1, 1, sigmaY * sigmaY);
				point.DispersionApriori = dispersion;
				return point;
			}

			// first two values == first row/column
			double varX = options.convertLengthToModel(sigmaX);
			double covXY = options.convertLengthToModel(sigmaY);

			double varY = options.convertLengthToModel(options.convertLengthToModel(double.Parse(columns[5].Replace(',', '.'))));

			if (varX <= 0 || varY <= 0)
			{
				return point;
			}

			Matrix dispersion = new UpperSymmPackMatrix(point.Dimension);
			dispersion.set(0, 0, varX);
			dispersion.set(0, 1, covXY);
			dispersion.set(1, 1, varY);
			point.DispersionApriori = dispersion;
			return point;
		}

		private static FeaturePoint scanSurfacePoint(string str)
		{
			FormatterOptions options = FormatterOptions.Instance;
			string[] columns = str.Trim().Split("[\\s;]+", true);

			if (columns.Length < 4)
			{
				return null;
			}

			string name = columns[0];
			double x = options.convertLengthToModel(double.Parse(columns[1].Replace(',', '.')));
			double y = options.convertLengthToModel(double.Parse(columns[2].Replace(',', '.')));
			double z = options.convertLengthToModel(double.Parse(columns[3].Replace(',', '.')));

			FeaturePoint point = new FeaturePoint(name, x, y, z);
			if (columns.Length < 5)
			{
				return point;
			}

			double sigmaX, sigmaY, sigmaZ;
			sigmaX = sigmaY = sigmaZ = options.convertLengthToModel(double.Parse(columns[4].Replace(',', '.')));

			if (columns.Length < 6)
			{
				if (sigmaX <= 0 || sigmaY <= 0 || sigmaZ <= 0)
				{
					return point;
				}

				Matrix dispersion = new UpperSymmBandMatrix(point.Dimension, 0);
				dispersion.set(0, 0, sigmaX * sigmaX);
				dispersion.set(1, 1, sigmaY * sigmaY);
				dispersion.set(2, 2, sigmaZ * sigmaZ);
				point.DispersionApriori = dispersion;
				return point;
			}

			sigmaY = sigmaZ = options.convertLengthToModel(double.Parse(columns[5].Replace(',', '.')));

			if (columns.Length < 7)
			{
				if (sigmaX <= 0 || sigmaY <= 0 || sigmaZ <= 0)
				{
					return point;
				}

				Matrix dispersion = new UpperSymmBandMatrix(point.Dimension, 0);
				dispersion.set(0, 0, sigmaX * sigmaX);
				dispersion.set(1, 1, sigmaX * sigmaX); // if only two uncertainty colums are given, set sx = sy; sz
				dispersion.set(2, 2, sigmaZ * sigmaZ);
				point.DispersionApriori = dispersion;
				return point;
			}

			sigmaZ = options.convertLengthToModel(double.Parse(columns[6].Replace(',', '.')));

			if (columns.Length < 10)
			{
				if (sigmaX <= 0 || sigmaY <= 0 || sigmaZ <= 0)
				{
					return point;
				}

				Matrix dispersion = new UpperSymmBandMatrix(point.Dimension, 0);
				dispersion.set(0, 0, sigmaX * sigmaX);
				dispersion.set(1, 1, sigmaY * sigmaY);
				dispersion.set(2, 2, sigmaZ * sigmaZ);
				point.DispersionApriori = dispersion;
				return point;
			}

			// first three values == first row/column
			double varX = options.convertLengthToModel(sigmaX);
			double covXY = options.convertLengthToModel(sigmaY);
			double covXZ = options.convertLengthToModel(sigmaZ);

			double varY = options.convertLengthToModel(options.convertLengthToModel(double.Parse(columns[7].Replace(',', '.'))));
			double covYZ = options.convertLengthToModel(options.convertLengthToModel(double.Parse(columns[8].Replace(',', '.'))));

			double varZ = options.convertLengthToModel(options.convertLengthToModel(double.Parse(columns[9].Replace(',', '.'))));

			if (varX <= 0 || varY <= 0 || varZ <= 0)
			{
				return point;
			}

			Matrix dispersion = new UpperSymmPackMatrix(point.Dimension);
			dispersion.set(0, 0, varX);
			dispersion.set(0, 1, covXY);
			dispersion.set(0, 2, covXZ);
			dispersion.set(1, 1, varY);
			dispersion.set(1, 2, covYZ);
			dispersion.set(2, 2, varZ);
			point.DispersionApriori = dispersion;
			return point;
		}

	//	private static FeaturePoint scanCurvePoint(String str) {
	//		FormatterOptions options = FormatterOptions.getInstance();
	//		Scanner scanner = new Scanner( str.trim() );
	//		try {
	//			scanner.useLocale( Locale.ENGLISH );
	//			String name; 
	//			double y = 0.0, x = 0.0;
	//			double sigmaY = 0.0, sigmaX = 0.0;
	//
	//			// Name of point
	//			if (!scanner.hasNext())
	//				return null;
	//			name = scanner.next();
	//			
	//			// x-component 		
	//			if (!scanner.hasNextDouble())
	//				return null;
	//			x = options.convertLengthToModel(scanner.nextDouble());
	//
	//			// y-component 		
	//			if (!scanner.hasNextDouble())
	//				return null;
	//			y = options.convertLengthToModel(scanner.nextDouble());
	//			
	//			FeaturePoint point = new FeaturePoint(name, x, y);
	//
	//			// uncertainty: sigma X (or sigmaX/Y)
	//			if (!scanner.hasNextDouble()) 
	//				return point;
	//			
	//			sigmaY = sigmaX = options.convertLengthToModel(scanner.nextDouble());
	//
	//			// uncertainty: sigma Y
	//			if (!scanner.hasNextDouble()) {
	//				if (sigmaX <= 0 || sigmaY <= 0)
	//					return point;
	//				
	//				Matrix dispersion = new UpperSymmBandMatrix(point.getDimension(), 0);
	//				dispersion.set(0, 0, sigmaX * sigmaX);
	//				dispersion.set(1, 1, sigmaY * sigmaY);
	//				point.setDispersionApriori(dispersion);
	//				return point;
	//			}
	//			
	//			sigmaY = options.convertLengthToModel(scanner.nextDouble());
	//			
	//			// fully populated co-variance: varX covXY varY
	//			// correlation: XY
	//			if (!scanner.hasNextDouble()) {
	//				if (sigmaX <= 0 || sigmaY <= 0)
	//					return point;
	//				
	//				Matrix dispersion = new UpperSymmBandMatrix(point.getDimension(), 0);
	//				dispersion.set(0, 0, sigmaX * sigmaX);
	//				dispersion.set(1, 1, sigmaY * sigmaY);
	//				point.setDispersionApriori(dispersion);
	//				return point;
	//			}
	//			
	//			// first two values == first row/column
	//			double varX  = options.convertLengthToModel(sigmaX);
	//			double covXY = options.convertLengthToModel(sigmaY);
	//
	//			double varY  = options.convertLengthToModel(options.convertLengthToModel(scanner.nextDouble()));
	//			
	//			if (varX <= 0 || varY <= 0)
	//				return point;
	//			
	//			Matrix dispersion = new UpperSymmPackMatrix(point.getDimension());
	//			dispersion.set(0, 0, varX);
	//			dispersion.set(0, 1, covXY);
	//			dispersion.set(1, 1, varY);
	//			point.setDispersionApriori(dispersion);
	//			return point;
	//			
	//		}
	//		finally {
	//			scanner.close();
	//		}
	//	}

	//	private static FeaturePoint scanSurfacePoint(String str) {
	//		FormatterOptions options = FormatterOptions.getInstance();
	//		Scanner scanner = new Scanner( str.trim() );
	//		try {
	//			scanner.useLocale( Locale.ENGLISH );
	//			String name; 
	//			double y = 0.0, x = 0.0, z = 0.0;
	//			double sigmaY = 0.0, sigmaX = 0.0, sigmaZ = 0.0;
	//
	//			// Name of point
	//			if (!scanner.hasNext())
	//				return null;
	//			name = scanner.next();
	//			
	//			// x-component 		
	//			if (!scanner.hasNextDouble())
	//				return null;
	//			x = options.convertLengthToModel(scanner.nextDouble());
	//
	//			// y-component		
	//			if (!scanner.hasNextDouble())
	//				return null;
	//			y = options.convertLengthToModel(scanner.nextDouble());
	//
	//			// z-component 		
	//			if (!scanner.hasNextDouble())
	//				return null;
	//			z = options.convertLengthToModel(scanner.nextDouble());
	//
	//			FeaturePoint point = new FeaturePoint(name, x, y, z);
	//			
	//			// uncertainty: sigma X (or sigma x/y/z)
	//			if (!scanner.hasNextDouble()) 
	//				return point;
	//
	//			sigmaX = sigmaY = sigmaZ = options.convertLengthToModel(scanner.nextDouble());
	//			
	//			// uncertainty: sigma Y (or sigma Z)
	//			if (!scanner.hasNextDouble()) {
	//				if (sigmaX <= 0 || sigmaY <= 0 || sigmaZ <= 0)
	//					return point;
	//
	//				Matrix dispersion = new UpperSymmBandMatrix(point.getDimension(), 0);
	//				dispersion.set(0, 0, sigmaX * sigmaX);
	//				dispersion.set(1, 1, sigmaY * sigmaY);
	//				dispersion.set(2, 2, sigmaZ * sigmaZ);
	//				point.setDispersionApriori(dispersion);
	//				return point;
	//			}
	//
	//			sigmaY = sigmaZ = options.convertLengthToModel(scanner.nextDouble());
	//
	//			// uncertainty: sigma Z
	//			if (!scanner.hasNextDouble()) {
	//				if (sigmaX <= 0 || sigmaY <= 0 || sigmaZ <= 0)
	//					return point;
	//				
	//				Matrix dispersion = new UpperSymmBandMatrix(point.getDimension(), 0);
	//				dispersion.set(0, 0, sigmaX * sigmaX);
	//				dispersion.set(1, 1, sigmaX * sigmaX);
	//				dispersion.set(2, 2, sigmaZ * sigmaZ);
	//				point.setDispersionApriori(dispersion);
	//				return point;
	//			}
	//
	//			sigmaZ = options.convertLengthToModel(scanner.nextDouble());
	//
	//			// fully populated co-variance: varX covXY covXZ varY covYZ varZ
	//			// correlation: XYZ
	//			if (!scanner.hasNextDouble()) {
	//				if (sigmaX <= 0 || sigmaY <= 0 || sigmaZ <= 0)
	//					return point;
	//				
	//				Matrix dispersion = new UpperSymmBandMatrix(point.getDimension(), 0);
	//				dispersion.set(0, 0, sigmaX * sigmaX);
	//				dispersion.set(1, 1, sigmaY * sigmaY);
	//				dispersion.set(2, 2, sigmaZ * sigmaZ);
	//				point.setDispersionApriori(dispersion);
	//				return point;
	//			}
	//			
	//			// first three values == first row/column
	//			double varX  = options.convertLengthToModel(sigmaX);
	//			double covXY = options.convertLengthToModel(sigmaY);
	//			double covXZ = options.convertLengthToModel(sigmaZ);
	//			
	//			double varY  = options.convertLengthToModel(options.convertLengthToModel(scanner.nextDouble()));
	//			double covYZ = options.convertLengthToModel(options.convertLengthToModel(scanner.nextDouble()));
	//			
	//			double varZ  = options.convertLengthToModel(options.convertLengthToModel(scanner.nextDouble()));
	//
	//			if (varX <= 0 || varY <= 0 || varZ <= 0)
	//				return point;
	//			
	//			Matrix dispersion = new UpperSymmPackMatrix(point.getDimension());
	//			dispersion.set(0, 0, varX);
	//			dispersion.set(0, 1, covXY);
	//			dispersion.set(0, 2, covXZ);
	//			dispersion.set(1, 1, varY);
	//			dispersion.set(1, 2, covYZ);
	//			dispersion.set(2, 2, varZ);
	//			point.setDispersionApriori(dispersion);
	//			return point;
	//
	//		}
	//		finally {
	//			scanner.close();
	//		}
	//	}

		public static ExtensionFilter[] ExtensionFilters
		{
			get
			{
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("FeaturePointFileReader.extension.description", "All files"), "*.*")};
			}
		}
	}

}