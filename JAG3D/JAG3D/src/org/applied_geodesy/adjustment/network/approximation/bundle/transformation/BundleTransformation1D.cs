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

namespace org.applied_geodesy.adjustment.network.approximation.bundle.transformation
{

	using NormalEquationSystem = org.applied_geodesy.adjustment.NormalEquationSystem;
	using PointBundle = org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle;
	using Point = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point;

	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;
	using Vector = no.uib.cipr.matrix.Vector;
	using LinkedSparseMatrix = no.uib.cipr.matrix.sparse.LinkedSparseMatrix;

	public class BundleTransformation1D : BundleTransformation
	{

		public BundleTransformation1D(double threshold, IList<PointBundle> sourceSystems) : base(threshold, sourceSystems)
		{
		}

		public BundleTransformation1D(double threshold, PointBundle sourceSystem, PointBundle targetSystem) : base(threshold, sourceSystem, targetSystem)
		{
		}

		public BundleTransformation1D(double threshold, IList<PointBundle> sourceSystems, PointBundle targetSystem) : base(threshold, sourceSystems, targetSystem)
		{
		}

		public override Transformation1D getSimpleTransformationModel(PointBundle b1, PointBundle b2)
		{
			return new Transformation1D(b1, b2);
		}

		public override Transformation1D getSimpleTransformationModel(TransformationParameterSet transParameter)
		{
			return new Transformation1D(transParameter);
		}

		protected internal override NormalEquationSystem createNormalEquationSystem()
		{
			PointBundle targetSystem = this.TargetSystem;
			IList<PointBundle> sourceSystems = this.SourceSystems;

			Matrix A = new LinkedSparseMatrix(this.numberOfObservations(), this.numberOfUnknowns());
			Vector w = new DenseVector(A.numRows());

			double s0 = this.ScaleEstimate;

			foreach (PointBundle sourceSystem in sourceSystems)
			{
				// Transformationsparameter des lokalen Systems
				TransformationParameterSet parameters = sourceSystem.TransformationParameterSet;
				TransformationParameter m = parameters.get(TransformationParameterType.SCALE);
				TransformationParameter tZ = parameters.get(TransformationParameterType.TRANSLATION_Z);

				if (this.interrupt_Conflict)
				{
					return null;
				}

				for (int i = 0; i < sourceSystem.size(); i++)
				{
					Point pointSource = sourceSystem.get(i);
					string pointId = pointSource.Name;
					int row = pointSource.RowInJacobiMatrix;
					Point pointTarget = targetSystem.get(pointId);
					if (row < 0 || pointTarget == null)
					{
						continue;
					}

					double vz = (pointSource.Z - (m.Value * pointTarget.Z + tZ.Value));
					double pz = pointSource.WeightedZ > 0 && Math.Abs(vz) < s0 ? 1.0 : 0.0;

					if (pz == 0.0)
					{
						pointSource.isOutlier(true);
						pointSource.WeightedZ = pz;
						this.addOutlierPoint(pointSource);
					}
					else if (pointSource.Outlier)
					{
						pointSource.isOutlier(false);
						this.removeOutlierPoint(pointSource);
					}

					if (!m.Fixed)
					{
						int col = m.ColInJacobiMatrix;
						A.set(row, col, pz * pointTarget.Z);
					}

					if (!tZ.Fixed)
					{
						int col = tZ.ColInJacobiMatrix;
						A.set(row, col, pz * 1.0);
					}

					if (pointSource.ColInJacobiMatrix >= 0)
					{
						int col = pointTarget.ColInJacobiMatrix;
						A.set(row, col, pz * m.Value);
					}

					// Widerspruchsvektor
					w.set(row, pz * (pointSource.Z - (m.Value * pointTarget.Z + tZ.Value)));
				}
			}
			UpperSymmPackMatrix ATA = new UpperSymmPackMatrix(this.numberOfUnknowns());
			DenseVector ATw = new DenseVector(this.numberOfUnknowns());

			if (this.interrupt_Conflict)
			{
				return null;
			}

			if (this.numberOfUnknowns() > 0)
			{
				A.transAmult(A, ATA);
				A.transMult(w, ATw);
			}
			return new NormalEquationSystem(ATA, ATw);
		}
	}

}