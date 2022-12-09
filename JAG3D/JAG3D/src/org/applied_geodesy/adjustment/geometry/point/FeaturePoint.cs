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

namespace org.applied_geodesy.adjustment.geometry.point
{

	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using TestStatistic = org.applied_geodesy.adjustment.geometry.TestStatistic;

	using ObjectBinding = javafx.beans.binding.ObjectBinding;
	using ObjectProperty = javafx.beans.property.ObjectProperty;
	using ReadOnlyObjectProperty = javafx.beans.property.ReadOnlyObjectProperty;
	using ReadOnlyObjectWrapper = javafx.beans.property.ReadOnlyObjectWrapper;
	using SimpleObjectProperty = javafx.beans.property.SimpleObjectProperty;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixEntry = no.uib.cipr.matrix.MatrixEntry;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using UnitUpperTriangBandMatrix = no.uib.cipr.matrix.UnitUpperTriangBandMatrix;
	using UpperSymmBandMatrix = no.uib.cipr.matrix.UpperSymmBandMatrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;

	public class FeaturePoint : Point, IEnumerable<GeometricPrimitive>
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			testStatistic = new ReadOnlyObjectWrapper<TestStatistic>(this, "testStatistic", new TestStatistic());
			enable = new SimpleObjectProperty<bool>(this, "enable", true);
			residualX = new SimpleObjectProperty<double>(this, "residualX", 0.0);
			residualY = new SimpleObjectProperty<double>(this, "residualY", 0.0);
			residualZ = new SimpleObjectProperty<double>(this, "residualZ", 0.0);
			redundancyX = new SimpleObjectProperty<double>(this, "redundancyX", 0.0);
			redundancyY = new SimpleObjectProperty<double>(this, "redundancyY", 0.0);
			redundancyZ = new SimpleObjectProperty<double>(this, "redundancyZ", 0.0);
			grossErrorX = new SimpleObjectProperty<double>(this, "grossErrorX", 0.0);
			grossErrorY = new SimpleObjectProperty<double>(this, "grossErrorY", 0.0);
			grossErrorZ = new SimpleObjectProperty<double>(this, "grossErrorZ", 0.0);
			minimalDetectableBiasX = new SimpleObjectProperty<double>(this, "minimalDetectableBiasX", 0.0);
			minimalDetectableBiasY = new SimpleObjectProperty<double>(this, "minimalDetectableBiasY", 0.0);
			minimalDetectableBiasZ = new SimpleObjectProperty<double>(this, "minimalDetectableBiasZ", 0.0);
			maximumTolerableBiasX = new SimpleObjectProperty<double>(this, "maximumTolerableBiasX", 0.0);
			maximumTolerableBiasY = new SimpleObjectProperty<double>(this, "maximumTolerableBiasY", 0.0);
			maximumTolerableBiasZ = new SimpleObjectProperty<double>(this, "maximumTolerableBiasZ", 0.0);
			cofactorX = new SimpleObjectProperty<double>(this, "cofactorX", 0.0);
			cofactorY = new SimpleObjectProperty<double>(this, "cofactorY", 0.0);
			cofactorZ = new SimpleObjectProperty<double>(this, "cofactorZ", 0.0);
			testStatisticApriori = new ReadOnlyObjectWrapper<double>(this, "testStatisticApriori", 0.0);
			testStatisticAposteriori = new ReadOnlyObjectWrapper<double>(this, "testStatisticAposteriori", 0.0);
			pValueApriori = new ReadOnlyObjectWrapper<double>(this, "pValueApriori", 0.0);
			pValueAposteriori = new ReadOnlyObjectWrapper<double>(this, "pValueAposteriori", 0.0);
			dispersionApriori = new SimpleObjectProperty<Matrix>(this, "dispersionApriori");
			fisherQuantileApriori = new SimpleObjectProperty<double>(this, "fisherQuantileApriori", double.MaxValue);
			fisherQuantileAposteriori = new SimpleObjectProperty<double>(this, "fisherQuantileAposteriori", double.MaxValue);
		}

		private ReadOnlyObjectProperty<TestStatistic> testStatistic;

		private ObjectProperty<bool> enable;

		private ObjectProperty<double> residualX;
		private ObjectProperty<double> residualY;
		private ObjectProperty<double> residualZ;

		private ObjectProperty<double> redundancyX;
		private ObjectProperty<double> redundancyY;
		private ObjectProperty<double> redundancyZ;

		private ObjectProperty<double> grossErrorX;
		private ObjectProperty<double> grossErrorY;
		private ObjectProperty<double> grossErrorZ;

		private ObjectProperty<double> minimalDetectableBiasX;
		private ObjectProperty<double> minimalDetectableBiasY;
		private ObjectProperty<double> minimalDetectableBiasZ;

		private ObjectProperty<double> maximumTolerableBiasX;
		private ObjectProperty<double> maximumTolerableBiasY;
		private ObjectProperty<double> maximumTolerableBiasZ;

		private ObjectProperty<double> cofactorX;
		private ObjectProperty<double> cofactorY;
		private ObjectProperty<double> cofactorZ;

		private ObjectBinding<double> x;
		private ObjectBinding<double> y;
		private ObjectBinding<double> z;

		private ObjectBinding<double> uncertaintyX;
		private ObjectBinding<double> uncertaintyY;
		private ObjectBinding<double> uncertaintyZ;

		private ReadOnlyObjectWrapper<double> testStatisticApriori;
		private ReadOnlyObjectWrapper<double> testStatisticAposteriori;

		private ReadOnlyObjectWrapper<double> pValueApriori;
		private ReadOnlyObjectWrapper<double> pValueAposteriori;

		private ObjectBinding<bool> significant;

		private ObjectProperty<Matrix> dispersionApriori;

		private ObjectProperty<double> fisherQuantileApriori;
		private ObjectProperty<double> fisherQuantileAposteriori;

		private ISet<GeometricPrimitive> geometries = new LinkedHashSet<GeometricPrimitive>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public FeaturePoint(String name, double x0, double y0) throws IllegalArgumentException
		public FeaturePoint(string name, double x0, double y0) : this(name, x0, y0, MathExtension.identity(2))
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public FeaturePoint(String name, double x0, double y0, no.uib.cipr.matrix.Matrix dispersion) throws IllegalArgumentException
		public FeaturePoint(string name, double x0, double y0, Matrix dispersion) : base(name, x0, y0)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.DispersionApriori = dispersion;
			this.init(this.Dimension);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public FeaturePoint(String name, double x0, double y0, double z0) throws IllegalArgumentException
		public FeaturePoint(string name, double x0, double y0, double z0) : this(name, x0, y0, z0, MathExtension.identity(3))
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public FeaturePoint(String name, double x0, double y0, double z0, no.uib.cipr.matrix.Matrix dispersion) throws IllegalArgumentException
		public FeaturePoint(string name, double x0, double y0, double z0, Matrix dispersion) : base(name, x0, y0, z0)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.DispersionApriori = dispersion;
			this.init(this.Dimension);
		}

		private void init(int dim)
		{
			this.x = new ObjectBindingAnonymousInnerClass(this);

			this.y = new ObjectBindingAnonymousInnerClass2(this);

			this.z = new ObjectBindingAnonymousInnerClass3(this);

			this.uncertaintyX = new ObjectBindingAnonymousInnerClass4(this);

			this.uncertaintyY = new ObjectBindingAnonymousInnerClass5(this);

			this.uncertaintyZ = new ObjectBindingAnonymousInnerClass6(this);

			this.testStatisticApriori.bind(this.testStatistic.get().testStatisticAprioriProperty());
			this.testStatisticAposteriori.bind(this.testStatistic.get().testStatisticAposterioriProperty());

			this.pValueApriori.bind(this.testStatistic.get().pValueAprioriProperty());
			this.pValueAposteriori.bind(this.testStatistic.get().pValueAposterioriProperty());

			this.significant = new ObjectBindingAnonymousInnerClass7(this);
		}

		private class ObjectBindingAnonymousInnerClass : ObjectBinding<double>
		{
			private readonly FeaturePoint outerInstance;

			public ObjectBindingAnonymousInnerClass(FeaturePoint outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.x0Property(), outerInstance.residualX);
			}


					protected internal override double? computeValue()
					{
						return outerInstance.x0Property().get() + outerInstance.residualX.get();
					}
		}

		private class ObjectBindingAnonymousInnerClass2 : ObjectBinding<double>
		{
			private readonly FeaturePoint outerInstance;

			public ObjectBindingAnonymousInnerClass2(FeaturePoint outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.y0Property(), outerInstance.residualY);
			}


					protected internal override double? computeValue()
					{
						return outerInstance.y0Property().get() + outerInstance.residualY.get();
					}
		}

		private class ObjectBindingAnonymousInnerClass3 : ObjectBinding<double>
		{
			private readonly FeaturePoint outerInstance;

			public ObjectBindingAnonymousInnerClass3(FeaturePoint outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.z0Property(), outerInstance.residualZ);
			}


					protected internal override double? computeValue()
					{
						return outerInstance.z0Property().get() + outerInstance.residualZ.get();
					}
		}

		private class ObjectBindingAnonymousInnerClass4 : ObjectBinding<double>
		{
			private readonly FeaturePoint outerInstance;

			public ObjectBindingAnonymousInnerClass4(FeaturePoint outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.cofactorX, outerInstance.testStatistic);
			}


					protected internal override double? computeValue()
					{
						if (outerInstance.testStatistic.get().varianceComponentProperty().get().isApplyAposterioriVarianceOfUnitWeight())
						{
							return Math.Sqrt(Math.Abs(outerInstance.cofactorX.get() * outerInstance.testStatistic.get().varianceComponentProperty().get().varianceProperty().get()));
						}
						return Math.Sqrt(Math.Abs(outerInstance.cofactorX.get()));
					}
		}

		private class ObjectBindingAnonymousInnerClass5 : ObjectBinding<double>
		{
			private readonly FeaturePoint outerInstance;

			public ObjectBindingAnonymousInnerClass5(FeaturePoint outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.cofactorY, outerInstance.testStatistic);
			}


					protected internal override double? computeValue()
					{
						if (outerInstance.testStatistic.get().varianceComponentProperty().get().isApplyAposterioriVarianceOfUnitWeight())
						{
							return Math.Sqrt(Math.Abs(outerInstance.cofactorY.get() * outerInstance.testStatistic.get().varianceComponentProperty().get().varianceProperty().get()));
						}
						return Math.Sqrt(Math.Abs(outerInstance.cofactorY.get()));
					}
		}

		private class ObjectBindingAnonymousInnerClass6 : ObjectBinding<double>
		{
			private readonly FeaturePoint outerInstance;

			public ObjectBindingAnonymousInnerClass6(FeaturePoint outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.cofactorZ, outerInstance.testStatistic);
			}


					protected internal override double? computeValue()
					{
						if (outerInstance.testStatistic.get().varianceComponentProperty().get().isApplyAposterioriVarianceOfUnitWeight())
						{
							return Math.Sqrt(Math.Abs(outerInstance.cofactorZ.get() * outerInstance.testStatistic.get().varianceComponentProperty().get().varianceProperty().get()));
						}
						return Math.Sqrt(Math.Abs(outerInstance.cofactorZ.get()));
					}
		}

		private class ObjectBindingAnonymousInnerClass7 : ObjectBinding<bool>
		{
			private readonly FeaturePoint outerInstance;

			public ObjectBindingAnonymousInnerClass7(FeaturePoint outerInstance)
			{
				this.outerInstance = outerInstance;

				base.bind(outerInstance.fisherQuantileApriori, outerInstance.testStatisticApriori, outerInstance.fisherQuantileAposteriori, outerInstance.testStatisticAposteriori);
			}


					protected internal override bool? computeValue()
					{
						bool significant = outerInstance.testStatisticApriori.get() > outerInstance.fisherQuantileApriori.get();

						if (outerInstance.testStatistic.get().varianceComponentProperty().get().isApplyAposterioriVarianceOfUnitWeight())
						{
							return significant || outerInstance.testStatisticAposteriori.get() > outerInstance.fisherQuantileAposteriori.get();
						}

						return significant;
					}
		}

		public virtual double X
		{
			get
			{
				return this.x.get();
			}
		}

		public virtual ObjectBinding<double> xProperty()
		{
			return this.x;
		}

		public virtual double Y
		{
			get
			{
				return this.y.get();
			}
		}

		public virtual ObjectBinding<double> yProperty()
		{
			return this.y;
		}

		public virtual double Z
		{
			get
			{
				return this.z.get();
			}
		}

		public virtual ObjectBinding<double> zProperty()
		{
			return this.z;
		}

		public virtual bool Enable
		{
			get
			{
				return this.enable.get();
			}
			set
			{
				this.enable.set(value);
			}
		}


		public virtual ObjectProperty<bool> enableProperty()
		{
			return this.enable;
		}

		public virtual double ResidualX
		{
			get
			{
				return this.residualX.get();
			}
			set
			{
				this.residualX.set(value);
			}
		}


		public virtual ObjectProperty<double> residualXProperty()
		{
			return this.residualX;
		}

		public virtual double ResidualY
		{
			get
			{
				return this.residualY.get();
			}
			set
			{
				this.residualY.set(value);
			}
		}


		public virtual ObjectProperty<double> residualYProperty()
		{
			return this.residualY;
		}

		public virtual double ResidualZ
		{
			get
			{
				return this.residualZ.get();
			}
			set
			{
				this.residualZ.set(value);
			}
		}


		public virtual ObjectProperty<double> residualZProperty()
		{
			return this.residualZ;
		}

		public virtual double RedundancyX
		{
			get
			{
				return this.redundancyX.get();
			}
			set
			{
				this.redundancyX.set(value);
			}
		}


		public virtual ObjectProperty<double> redundancyXProperty()
		{
			return this.redundancyX;
		}

		public virtual double RedundancyY
		{
			get
			{
				return this.redundancyY.get();
			}
			set
			{
				this.redundancyY.set(value);
			}
		}


		public virtual ObjectProperty<double> redundancyYProperty()
		{
			return this.redundancyY;
		}

		public virtual double RedundancyZ
		{
			get
			{
				return this.redundancyZ.get();
			}
			set
			{
				this.redundancyZ.set(value);
			}
		}


		public virtual ObjectProperty<double> redundancyZProperty()
		{
			return this.redundancyZ;
		}

		public virtual double MinimalDetectableBiasX
		{
			get
			{
				return this.minimalDetectableBiasX.get();
			}
			set
			{
				this.minimalDetectableBiasX.set(value);
			}
		}


		public virtual ObjectProperty<double> minimalDetectableBiasXProperty()
		{
			return this.minimalDetectableBiasX;
		}

		public virtual double MinimalDetectableBiasY
		{
			get
			{
				return this.minimalDetectableBiasY.get();
			}
			set
			{
				this.minimalDetectableBiasY.set(value);
			}
		}


		public virtual ObjectProperty<double> minimalDetectableBiasYProperty()
		{
			return this.minimalDetectableBiasY;
		}

		public virtual double MinimalDetectableBiasZ
		{
			get
			{
				return this.minimalDetectableBiasZ.get();
			}
			set
			{
				this.minimalDetectableBiasZ.set(value);
			}
		}


		public virtual ObjectProperty<double> minimalDetectableBiasZProperty()
		{
			return this.minimalDetectableBiasZ;
		}

		public virtual double MaximumTolerableBiasX
		{
			get
			{
				return this.maximumTolerableBiasX.get();
			}
			set
			{
				this.maximumTolerableBiasX.set(value);
			}
		}


		public virtual ObjectProperty<double> maximumTolerableBiasXProperty()
		{
			return this.maximumTolerableBiasX;
		}

		public virtual double MaximumTolerableBiasY
		{
			get
			{
				return this.maximumTolerableBiasY.get();
			}
			set
			{
				this.maximumTolerableBiasY.set(value);
			}
		}


		public virtual ObjectProperty<double> maximumTolerableBiasYProperty()
		{
			return this.maximumTolerableBiasY;
		}

		public virtual double MaximumTolerableBiasZ
		{
			get
			{
				return this.maximumTolerableBiasZ.get();
			}
			set
			{
				this.maximumTolerableBiasZ.set(value);
			}
		}


		public virtual ObjectProperty<double> maximumTolerableBiasZProperty()
		{
			return this.maximumTolerableBiasZ;
		}

		public virtual double GrossErrorX
		{
			get
			{
				return this.grossErrorX.get();
			}
			set
			{
				this.grossErrorX.set(value);
			}
		}


		public virtual ObjectProperty<double> grossErrorXProperty()
		{
			return this.grossErrorX;
		}

		public virtual double GrossErrorY
		{
			get
			{
				return this.grossErrorY.get();
			}
			set
			{
				this.grossErrorY.set(value);
			}
		}


		public virtual ObjectProperty<double> grossErrorYProperty()
		{
			return this.grossErrorY;
		}

		public virtual double GrossErrorZ
		{
			get
			{
				return this.grossErrorZ.get();
			}
			set
			{
				this.grossErrorZ.set(value);
			}
		}


		public virtual ObjectProperty<double> grossErrorZProperty()
		{
			return this.grossErrorZ;
		}

		public virtual ObjectProperty<Matrix> dispersionAprioriProperty()
		{
			return this.dispersionApriori;
		}

		public virtual Matrix DispersionApriori
		{
			get
			{
				return this.dispersionApriori.get();
			}
			set
			{
				if (!value.isSquare() || this.Dimension != value.numColumns())
				{
					throw new System.ArgumentException("Error, dispersion matrix must be a squared matrix of dimension " + this.Dimension + " x " + this.Dimension + "!");
				}
    
				if (!(value is UpperSymmBandMatrix) && !(value is UnitUpperTriangBandMatrix) && !(value is UpperSymmPackMatrix))
				{
					throw new System.ArgumentException("Error, dispersion matrix must be of type UpperSymmBandMatrix, UnitUpperTriangBandMatrix, or UpperSymmPackMatrix!");
				}
    
    
				if ((value is UpperSymmBandMatrix && ((UpperSymmBandMatrix)value).numSuperDiagonals() != 0) || (value is UnitUpperTriangBandMatrix) && ((UnitUpperTriangBandMatrix)value).numSuperDiagonals() != 0)
				{
					throw new System.ArgumentException("Error, dispersion matrix must be a diagonal matrix, if BandMatrix type is used!");
				}
    
				this.dispersionApriori.set(value);
			}
		}

		public virtual double UncertaintyX
		{
			get
			{
				return this.uncertaintyX.get();
			}
		}

		public virtual ObjectBinding<double> uncertaintyXProperty()
		{
			return this.uncertaintyX;
		}

		public virtual double UncertaintyY
		{
			get
			{
				return this.uncertaintyY.get();
			}
		}

		public virtual ObjectBinding<double> uncertaintyYProperty()
		{
			return this.uncertaintyY;
		}

		public virtual double UncertaintyZ
		{
			get
			{
				return this.uncertaintyZ.get();
			}
		}

		public virtual ObjectBinding<double> uncertaintyZProperty()
		{
			return this.uncertaintyZ;
		}

		public virtual double CofactorX
		{
			get
			{
				return this.cofactorX.get();
			}
			set
			{
				this.cofactorX.set(value);
			}
		}


		public virtual ObjectProperty<double> cofactorXProperty()
		{
			return this.cofactorX;
		}

		public virtual double CofactorY
		{
			get
			{
				return this.cofactorY.get();
			}
			set
			{
				this.cofactorY.set(value);
			}
		}


		public virtual ObjectProperty<double> cofactorYProperty()
		{
			return this.cofactorY;
		}

		public virtual double CofactorZ
		{
			get
			{
				return this.cofactorZ.get();
			}
			set
			{
				this.cofactorZ.set(value);
			}
		}


		public virtual ObjectProperty<double> cofactorZProperty()
		{
			return this.cofactorZ;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public no.uib.cipr.matrix.Matrix getInvertedDispersion(boolean inplace) throws MatrixSingularException, IllegalArgumentException
		public virtual Matrix getInvertedDispersion(bool inplace)
		{
			Matrix dispersionApriori = this.DispersionApriori;
			int size = dispersionApriori.numColumns();

			if (dispersionApriori is UnitUpperTriangBandMatrix)
			{
				return dispersionApriori; //inplace ? this.dispersion : new UnitUpperTriangBandMatrix(size, 0);
			}

			else if (dispersionApriori is UpperSymmBandMatrix)
			{
				Matrix W = inplace ? dispersionApriori : new UpperSymmBandMatrix(size, 0);
				foreach (MatrixEntry entry in dispersionApriori)
				{
					double value = entry.get();
					if (value <= 0)
					{
						throw new MatrixSingularException("Error, matrix is a singular matrix!");
					}
					W.set(entry.row(), entry.column(), 1.0 / value);
				}
				return W;
			}
			else if (dispersionApriori is UpperSymmPackMatrix)
			{
				UpperSymmPackMatrix W = inplace ? (UpperSymmPackMatrix)dispersionApriori : new UpperSymmPackMatrix(dispersionApriori, true);
				MathExtension.inv(W);
				return W;
			}

			throw new System.ArgumentException("Error, dispersion matrix must be of type UpperSymmBandMatrix, UnitUpperTriangBandMatrix, or UpperSymmPackMatrix!");
		}

		public virtual bool add(GeometricPrimitive geometry)
		{
			return this.geometries.Add(geometry);
		}

		public virtual bool remove(GeometricPrimitive geometry)
		{
			return this.geometries.remove(geometry);
		}

		public virtual int NumberOfGeomtries
		{
			get
			{
				return this.geometries.Count;
			}
		}

		public virtual TestStatistic TestStatistic
		{
			get
			{
				return this.testStatistic.get();
			}
		}

		public virtual ReadOnlyObjectProperty<TestStatistic> testStatisticProperty()
		{
			return this.testStatistic;
		}

		public virtual ReadOnlyObjectWrapper<double> testStatisticAprioriProperty()
		{
			return this.testStatisticApriori;
		}

		public virtual ReadOnlyObjectWrapper<double> testStatisticAposterioriProperty()
		{
			return this.testStatisticAposteriori;
		}

		public virtual ReadOnlyObjectWrapper<double> pValueAprioriProperty()
		{
			return this.pValueApriori;
		}

		public virtual ReadOnlyObjectWrapper<double> pValueAposterioriProperty()
		{
			return this.pValueAposteriori;
		}

		public virtual ObjectBinding<bool> significantProperty()
		{
			return this.significant;
		}

		public virtual bool Significant
		{
			get
			{
				return this.significant.get();
			}
		}

		public virtual double FisherQuantileApriori
		{
			set
			{
				this.fisherQuantileApriori.set(value);
			}
			get
			{
				return this.fisherQuantileApriori.get();
			}
		}


		public virtual ObjectProperty<double> fisherQuantileAprioriProperty()
		{
			return this.fisherQuantileApriori;
		}

		public virtual double FisherQuantileAposteriori
		{
			set
			{
				this.fisherQuantileAposteriori.set(value);
			}
			get
			{
				return this.fisherQuantileAposteriori.get();
			}
		}


		public virtual ObjectProperty<double> fisherQuantileAposterioriProperty()
		{
			return this.fisherQuantileAposteriori;
		}

		public virtual IEnumerator<GeometricPrimitive> GetEnumerator()
		{
			return this.geometries.GetEnumerator();
		}

		public virtual void reset()
		{
			this.ResidualX = 0;
			this.ResidualY = 0;
			this.ResidualZ = 0;

			this.RedundancyX = 0;
			this.RedundancyY = 0;
			this.RedundancyZ = 0;

			this.GrossErrorX = 0;
			this.GrossErrorY = 0;
			this.GrossErrorZ = 0;

			this.MinimalDetectableBiasX = 0;
			this.MinimalDetectableBiasY = 0;
			this.MinimalDetectableBiasZ = 0;

			this.CofactorX = 0;
			this.CofactorY = 0;
			this.CofactorZ = 0;

			this.testStatistic.get().setFisherTestNumerator(0);
			this.testStatistic.get().setDegreeOfFreedom(0);
		}

		public virtual void clear()
		{
			this.reset();
			this.geometries.Clear();
		}
	}

}