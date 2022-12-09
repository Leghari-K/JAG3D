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

	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using PointBundle = org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle;
	using Point = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point;
	using Point2D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point2D;

	public class Transformation2D : Transformation
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			source = new PointBundle(this.Dimension);
			target = new PointBundle(this.Dimension);
			originalTarget = new PointBundle(this.Dimension);
			pointsToTransform = new PointBundle(this.Dimension);
		}

		private PointBundle source, target, originalTarget, pointsToTransform;
		private TransformationParameterSet transParameter = null;
		private double omega = 0.0;

		private IDictionary<TransformationParameterType, bool> fixedParameters = new IDictionary<TransformationParameterType, bool>
		{
			{TransformationParameterType.TRANSLATION_X, false},
			{TransformationParameterType.TRANSLATION_Y, false},
			{TransformationParameterType.TRANSLATION_Z, true},
			{TransformationParameterType.ROTATION_X, true},
			{TransformationParameterType.ROTATION_Y, true},
			{TransformationParameterType.ROTATION_Z, false},
			{TransformationParameterType.SCALE, false}
		};

		public Transformation2D(TransformationParameterSet transParameter)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.transParameter = transParameter;
		}

		public Transformation2D(PointBundle source, PointBundle target)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (source.Dimension != target.Dimension || source.Dimension != this.numberOfRequiredPoints())
			{
				throw new System.ArgumentException(this.GetType().Name + " Dimensionsfehler, benoetige d = " + this.Dimension);
			}
			this.init(source, target);
		}

		public virtual bool transformLMS()
		{
			TransformationParameterSet transParameter = null;
			double omega = double.MaxValue;
			for (int i = 0; i < this.numberOfIdenticalPoints(); i++)
			{
				Point pS1 = this.source.get(i);
				Point pT1 = this.target.get(pS1.Name);
				for (int j = i + 1; j < this.numberOfIdenticalPoints(); j++)
				{
					Point pS2 = this.source.get(j);
					Point pT2 = this.target.get(pS2.Name);

					PointBundle subSource = new PointBundle(this.Dimension);
					PointBundle subTarget = new PointBundle(this.Dimension);

					subSource.addPoint(pS1);
					subSource.addPoint(pS2);

					subTarget.addPoint(pT1);
					subTarget.addPoint(pT2);

					TransformationParameterSet subParameter = this.adjustTransformationsParameter(subSource, subTarget);

					double o = this.getOmega(subParameter, this.source, this.target, true);

					if (o < omega)
					{
						transParameter = subParameter;
						omega = o;
					}
				}
			}
			this.omega = omega;
			this.transParameter = transParameter;

			return this.transParameter != null;
		}

		public virtual bool transformL2Norm()
		{
			if (this.numberOfIdenticalPoints() < this.numberOfRequiredPoints())
			{
				return false;
			}

			this.transParameter = null;
			this.transParameter = this.adjustTransformationsParameter(this.source, this.target);
			this.omega = this.getOmega(this.transParameter, this.source, this.target, false);

			return this.transParameter != null;
		}

		private double getOmega(TransformationParameterSet transParameter, PointBundle source, PointBundle target, bool isLMS)
		{
			if (transParameter == null)
			{
				return double.MaxValue;
			}

			double m = transParameter.getParameterValue(TransformationParameterType.SCALE);
			double rZ = transParameter.getParameterValue(TransformationParameterType.ROTATION_Z);
			double tX = transParameter.getParameterValue(TransformationParameterType.TRANSLATION_X);
			double tY = transParameter.getParameterValue(TransformationParameterType.TRANSLATION_Y);

			double[] omegaLMS = new double[this.Dimension * source.size()];
			double omegaLS = 0.0;
			for (int i = 0, j = 0; i < source.size(); i++)
			{
				Point pS = source.get(i);
				Point pT = target.get(pS.Name);
				double x = m * (Math.Cos(rZ) * pS.X - Math.Sin(rZ) * pS.Y) + tX;
				double y = m * (Math.Sin(rZ) * pS.X + Math.Cos(rZ) * pS.Y) + tY;

				double vx = pT.X - x;
				double vy = pT.Y - y;

				omegaLMS[j++] = vx * vx;
				omegaLMS[j++] = vy * vy;

				//			omegaLMS[i] = vx*vx + vy*vy;
				omegaLS += vx * vx + vy * vy;
			}

			if (isLMS)
			{
				Array.Sort(omegaLMS);
				if (omegaLMS.Length == this.Dimension * this.numberOfRequiredPoints())
				{
					return omegaLMS[omegaLMS.Length - 1];
				}
				if (omegaLMS.Length < 2 * this.Dimension * this.numberOfRequiredPoints())
				{
					return omegaLMS[this.Dimension * this.numberOfRequiredPoints()];
				}
				if (omegaLMS.Length % 2 == 1)
				{
					return omegaLMS[(int)(omegaLMS.Length / 2)];
				}
				return 0.5 * (omegaLMS[(int)(omegaLMS.Length / 2) - 1] + omegaLMS[(int)(omegaLMS.Length / 2)]);
			}
			return omegaLS;
		}

		private TransformationParameterSet adjustTransformationsParameter(PointBundle source, PointBundle target)
		{
			Point centerPointS = source.CenterPoint;
			Point centerPointT = target.CenterPoint;

			double o = 0.0, a = 0.0, oa = 0.0;

			for (int i = 0; i < source.size(); i++)
			{
				Point pS = source.get(i);
				Point pT = target.get(i);

				o += (pS.X - centerPointS.X) * (pT.Y - centerPointT.Y) - (pS.Y - centerPointS.Y) * (pT.X - centerPointT.X);
				a += (pS.X - centerPointS.X) * (pT.X - centerPointT.X) + (pS.Y - centerPointS.Y) * (pT.Y - centerPointT.Y);

				oa += Math.Pow(pS.X - centerPointS.X,2) + Math.Pow(pS.Y - centerPointS.Y,2);
			}

			if (oa == 0)
			{
				return null;
			}

			o /= oa;
			a /= oa;

			//		double tX = centerPointT.getX()-a*centerPointS.getX() + o*centerPointS.getY();
			//		double tY = centerPointT.getY()-a*centerPointS.getY() - o*centerPointS.getX();
			double tX = 0.0;
			double tY = 0.0;
			double rZ = 0.0;
			double m = 1.0;

			TransformationParameterSet transParameter = new TransformationParameterSet();

			if (!this.fixedParameters[TransformationParameterType.ROTATION_Z])
			{
				rZ = MathExtension.MOD(Math.Atan2(o, a), 2.0 * Math.PI);
			}
			if (!this.fixedParameters[TransformationParameterType.SCALE])
			{
				m = Math.hypot(a, o);
			}
			if (!this.fixedParameters[TransformationParameterType.TRANSLATION_X])
			{
				tX = centerPointT.X - (m * (Math.Cos(rZ) * centerPointS.X - Math.Sin(rZ) * centerPointS.Y));
			}
			if (!this.fixedParameters[TransformationParameterType.TRANSLATION_Y])
			{
				tY = centerPointT.Y - (m * (Math.Sin(rZ) * centerPointS.X + Math.Cos(rZ) * centerPointS.Y));
			}


			if (!this.fixedParameters[TransformationParameterType.TRANSLATION_X])
			{
				transParameter.setParameterValue(TransformationParameterType.TRANSLATION_X, tX);
			}
			if (!this.fixedParameters[TransformationParameterType.TRANSLATION_Y])
			{
				transParameter.setParameterValue(TransformationParameterType.TRANSLATION_Y, tY);
			}
			if (!this.fixedParameters[TransformationParameterType.ROTATION_Z])
			{
				transParameter.setParameterValue(TransformationParameterType.ROTATION_Z, rZ);
			}
			if (!this.fixedParameters[TransformationParameterType.SCALE])
			{
				transParameter.setParameterValue(TransformationParameterType.SCALE, m);
			}

			return transParameter;
		}

		private void init(PointBundle source, PointBundle target)
		{
			this.originalTarget = target;
			for (int i = 0; i < source.size(); i++)
			{
				Point pS = source.get(i);
				Point tempPS = new Point2D(pS.Name, pS.X, pS.Y);

				this.pointsToTransform.addPoint(tempPS);

				for (int j = 0; j < target.size(); j++)
				{
					Point pT = target.get(j);

					// Suche identische Punkte
					if (pS.Name.Equals(pT.Name))
					{
						Point tempPT = new Point2D(pT.Name, pT.X, pT.Y);
						this.source.addPoint(tempPS);
						this.target.addPoint(tempPT);
						break;
					}
				}
			}
		}

		public virtual Point2D transformPoint2SourceSystem(Point point)
		{
			if (this.transParameter == null)
			{
				return null;
			}

			double m = this.transParameter.getParameterValue(TransformationParameterType.SCALE);
			double rZ = this.transParameter.getParameterValue(TransformationParameterType.ROTATION_Z);
			double tX = this.transParameter.getParameterValue(TransformationParameterType.TRANSLATION_X);
			double tY = this.transParameter.getParameterValue(TransformationParameterType.TRANSLATION_Y);

			double x = (Math.Cos(rZ) * (point.X - tX) + Math.Sin(rZ) * (point.Y - tY)) / m;
			double y = (-Math.Sin(rZ) * (point.X - tX) + Math.Cos(rZ) * (point.Y - tY)) / m;

			return new Point2D(point.Name, x, y);
		}

		public virtual Point2D transformPoint2TargetSystem(Point point)
		{
			if (this.transParameter == null || point == null)
			{
				return null;
			}

			double m = this.transParameter.getParameterValue(TransformationParameterType.SCALE);
			double rZ = this.transParameter.getParameterValue(TransformationParameterType.ROTATION_Z);
			double tX = this.transParameter.getParameterValue(TransformationParameterType.TRANSLATION_X);
			double tY = this.transParameter.getParameterValue(TransformationParameterType.TRANSLATION_Y);

			double x = m * (Math.Cos(rZ) * point.X - Math.Sin(rZ) * point.Y) + tX;
			double y = m * (Math.Sin(rZ) * point.X + Math.Cos(rZ) * point.Y) + tY;

			return new Point2D(point.Name, x, y);
		}

		public int Dimension
		{
			get
			{
				return 2;
			}
		}

		public virtual int numberOfIdenticalPoints()
		{
			return this.source.size();
		}

		public virtual int numberOfRequiredPoints()
		{
			return 2;
		}

		public virtual PointBundle TransformdPoints
		{
			get
			{
				if (this.transParameter == null)
				{
					return null;
				}
    
				PointBundle transformedPoints = new PointBundle(this.Dimension);
    
				for (int i = 0; i < this.originalTarget.size(); i++)
				{
					Point pT = this.originalTarget.get(i);
					transformedPoints.addPoint(pT);
				}
    
				for (int i = 0; i < this.pointsToTransform.size(); i++)
				{
					Point pS = this.pointsToTransform.get(i);
					Point pT = transformedPoints.get(pS.Name);
					if (pT == null)
					{
						pT = this.transformPoint2TargetSystem(pS);
						transformedPoints.addPoint(pT);
					}
				}
				transformedPoints.TransformationParameterSet = this.transParameter;
				return transformedPoints;
			}
		}

		public virtual TransformationParameterSet TransformationParameterSet
		{
			get
			{
				return this.transParameter;
			}
		}

		public virtual void setFixedParameter(TransformationParameterType type, bool @fixed)
		{
			this.fixedParameters[type] = @fixed;
		}

		public virtual double Omega
		{
			get
			{
				return this.omega;
			}
		}
	}

}