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

	public class BundleTransformation2D : BundleTransformation
	{
		public BundleTransformation2D(double threshold, IList<PointBundle> sourceSystems) : base(threshold, sourceSystems)
		{
		}

		public BundleTransformation2D(double threshold, PointBundle sourceSystem, PointBundle targetSystem) : base(threshold, sourceSystem, targetSystem)
		{
		}

		public BundleTransformation2D(double threshold, IList<PointBundle> sourceSystems, PointBundle targetSystem) : base(threshold, sourceSystems, targetSystem)
		{
		}

		public override Transformation2D getSimpleTransformationModel(PointBundle b1, PointBundle b2)
		{
			return new Transformation2D(b1, b2);
		}

		public override Transformation2D getSimpleTransformationModel(TransformationParameterSet transParameter)
		{
			return new Transformation2D(transParameter);
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
				TransformationParameter rZ = parameters.get(TransformationParameterType.ROTATION_Z);
				TransformationParameter tX = parameters.get(TransformationParameterType.TRANSLATION_X);
				TransformationParameter tY = parameters.get(TransformationParameterType.TRANSLATION_Y);

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

					double vx = (pointSource.X - (Math.Cos(rZ.Value) * m.Value * pointTarget.X - Math.Sin(rZ.Value) * m.Value * pointTarget.Y + tX.Value));
					double vy = (pointSource.Y - (Math.Sin(rZ.Value) * m.Value * pointTarget.X + Math.Cos(rZ.Value) * m.Value * pointTarget.Y + tY.Value));

					double px = pointSource.WeightedX > 0 && Math.Abs(vx) < s0 ? 1.0 : 0.0;
					double py = pointSource.WeightedY > 0 && Math.Abs(vy) < s0 ? 1.0 : 0.0;

					if (px == 0.0 || py == 0.0)
					{
						pointSource.isOutlier(true);
						pointSource.WeightedX = px;
						pointSource.WeightedY = py;
						this.addOutlierPoint(pointTarget);
					}
					else if (pointSource.Outlier)
					{
						pointSource.isOutlier(false);
						this.removeOutlierPoint(pointTarget);
					}

					if (!m.Fixed)
					{
						int col = m.ColInJacobiMatrix;
						A.set(row, col, px * (Math.Cos(rZ.Value) * pointTarget.X - Math.Sin(rZ.Value) * pointTarget.Y));
						A.set(row + 1, col, py * (Math.Sin(rZ.Value) * pointTarget.X + Math.Cos(rZ.Value) * pointTarget.Y));
					}

					if (!rZ.Fixed)
					{
						int col = rZ.ColInJacobiMatrix;
						A.set(row, col, px * (-Math.Sin(rZ.Value) * m.Value * pointTarget.X - Math.Cos(rZ.Value) * m.Value * pointTarget.Y));
						A.set(row + 1, col, py * (Math.Cos(rZ.Value) * m.Value * pointTarget.X - Math.Sin(rZ.Value) * m.Value * pointTarget.Y));
					}

					if (!tX.Fixed)
					{
						int col = tX.ColInJacobiMatrix;
						A.set(row, col, px * 1.0);
					}

					if (!tY.Fixed)
					{
						int col = tY.ColInJacobiMatrix;
						A.set(row + 1, col, py * 1.0);
					}

					if (pointSource.ColInJacobiMatrix >= 0)
					{
						//int col = pointSource.getColInJacobiMatrix();					
						int col = pointTarget.ColInJacobiMatrix;
						A.set(row, col + 0, px * (Math.Cos(rZ.Value) * m.Value));
						A.set(row, col + 1, py * (-Math.Sin(rZ.Value) * m.Value));

						A.set(row + 1, col + 0, px * (Math.Sin(rZ.Value) * m.Value));
						A.set(row + 1, col + 1, py * (Math.Cos(rZ.Value) * m.Value));
					}

					// Widerspruchsvektor
					w.set(row, px * (pointSource.X - (Math.Cos(rZ.Value) * m.Value * pointTarget.X - Math.Sin(rZ.Value) * m.Value * pointTarget.Y + tX.Value)));
					w.set(row + 1, py * (pointSource.Y - (Math.Sin(rZ.Value) * m.Value * pointTarget.X + Math.Cos(rZ.Value) * m.Value * pointTarget.Y + tY.Value)));

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